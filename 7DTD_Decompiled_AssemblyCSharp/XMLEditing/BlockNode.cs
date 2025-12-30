using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace XMLEditing
{
	// Token: 0x020013B9 RID: 5049
	public class BlockNode
	{
		// Token: 0x170010FB RID: 4347
		// (get) Token: 0x06009DF8 RID: 40440 RVA: 0x003EDB5A File Offset: 0x003EBD5A
		// (set) Token: 0x06009DF9 RID: 40441 RVA: 0x003EDB62 File Offset: 0x003EBD62
		public string Name { get; set; }

		// Token: 0x170010FC RID: 4348
		// (get) Token: 0x06009DFA RID: 40442 RVA: 0x003EDB6B File Offset: 0x003EBD6B
		// (set) Token: 0x06009DFB RID: 40443 RVA: 0x003EDB73 File Offset: 0x003EBD73
		public XElement Element { get; set; }

		// Token: 0x170010FD RID: 4349
		// (get) Token: 0x06009DFC RID: 40444 RVA: 0x003EDB7C File Offset: 0x003EBD7C
		// (set) Token: 0x06009DFD RID: 40445 RVA: 0x003EDB84 File Offset: 0x003EBD84
		public BlockNode Parent { get; set; }

		// Token: 0x170010FE RID: 4350
		// (get) Token: 0x06009DFE RID: 40446 RVA: 0x003EDB8D File Offset: 0x003EBD8D
		public List<BlockNode> Children { get; } = new List<BlockNode>();

		// Token: 0x170010FF RID: 4351
		// (get) Token: 0x06009DFF RID: 40447 RVA: 0x003EDB95 File Offset: 0x003EBD95
		// (set) Token: 0x06009E00 RID: 40448 RVA: 0x003EDB9D File Offset: 0x003EBD9D
		public Dictionary<string, BlockNode.ElementInfo> ElementInfos { get; [PublicizedFrom(EAccessModifier.Private)] set; } = new Dictionary<string, BlockNode.ElementInfo>();

		// Token: 0x06009E01 RID: 40449 RVA: 0x003EDBA6 File Offset: 0x003EBDA6
		public void AddChild(BlockNode child)
		{
			child.Parent = this;
			this.Children.Add(child);
		}

		// Token: 0x06009E02 RID: 40450 RVA: 0x003EDBBC File Offset: 0x003EBDBC
		public bool TryGetPropertyParent(string targetPropertyName, out BlockNode propertyParentBlockNode, out BlockNode.ElementInfo propertyElementInfo, out int depth)
		{
			depth = 0;
			BlockNode blockNode = this;
			while (blockNode != null)
			{
				if (depth >= 100)
				{
					Debug.LogError("Max recursion depth exceeded!");
					break;
				}
				BlockNode.ElementInfo elementInfo;
				if (blockNode.ElementInfos.TryGetValue(targetPropertyName, out elementInfo))
				{
					if (elementInfo.Element != null)
					{
						propertyParentBlockNode = blockNode;
						propertyElementInfo = elementInfo;
						return true;
					}
					if (!elementInfo.CanInherit)
					{
						propertyParentBlockNode = null;
						propertyElementInfo = null;
						return false;
					}
				}
				blockNode = blockNode.Parent;
				depth++;
			}
			propertyParentBlockNode = null;
			propertyElementInfo = null;
			return false;
		}

		// Token: 0x06009E03 RID: 40451 RVA: 0x003EDC30 File Offset: 0x003EBE30
		public bool TryGetModelOffset(out Vector3 modelOffset, out int depth, out BlockNode.ModelOffsetType modelOffsetType)
		{
			BlockNode blockNode;
			BlockNode.ElementInfo elementInfo;
			if (this.TryGetPropertyParent("ModelOffset", out blockNode, out elementInfo, out depth))
			{
				modelOffsetType = BlockNode.ModelOffsetType.Explicit;
				modelOffset = StringParsers.ParseVector3(elementInfo.Element.GetAttribute(XNames.value), 0, -1);
				return true;
			}
			BlockNode.ElementInfo elementInfo2;
			if (!this.TryGetPropertyParent("Shape", out blockNode, out elementInfo2, out depth))
			{
				depth = 0;
				BlockNode blockNode2 = this;
				while (blockNode2.Parent != null)
				{
					blockNode2 = blockNode2.Parent;
					depth++;
				}
				modelOffsetType = BlockNode.ModelOffsetType.DefaultShapeNew;
				modelOffset = new Vector3(1f, 0f, 1f);
				return true;
			}
			string text = elementInfo2.Element.GetAttribute(XNames.value).Trim();
			Type typeWithPrefix = ReflectionHelpers.GetTypeWithPrefix("BlockShape", text);
			if (typeWithPrefix == null)
			{
				modelOffsetType = BlockNode.ModelOffsetType.None;
				Debug.LogError("Failed to create shape type \"BlockShape" + text + "\" for block: " + this.Name);
				modelOffset = Vector3.zero;
				return false;
			}
			if (typeof(BlockShapeNew).IsAssignableFrom(typeWithPrefix))
			{
				modelOffsetType = BlockNode.ModelOffsetType.ShapeNew;
				modelOffset = new Vector3(1f, 0f, 1f);
				return true;
			}
			if (typeof(BlockShapeModelEntity).IsAssignableFrom(typeWithPrefix))
			{
				modelOffsetType = BlockNode.ModelOffsetType.ShapeModelEntity;
				modelOffset = new Vector3(0f, 0.5f, 0f);
				return true;
			}
			if (typeof(BlockShapeExt3dModel).IsAssignableFrom(typeWithPrefix))
			{
				modelOffsetType = BlockNode.ModelOffsetType.ShapeExt3dModel;
				modelOffset = Vector3.zero;
				return true;
			}
			modelOffsetType = BlockNode.ModelOffsetType.None;
			modelOffset = Vector3.zero;
			return false;
		}

		// Token: 0x06009E04 RID: 40452 RVA: 0x003EDDB4 File Offset: 0x003EBFB4
		public bool ShapeSupportsModelOffset()
		{
			BlockNode blockNode;
			BlockNode.ElementInfo elementInfo;
			int num;
			if (this.TryGetPropertyParent("Shape", out blockNode, out elementInfo, out num))
			{
				string text = elementInfo.Element.GetAttribute(XNames.value).Trim();
				Type typeWithPrefix = ReflectionHelpers.GetTypeWithPrefix("BlockShape", text);
				if (typeWithPrefix == null)
				{
					Debug.LogError("Failed to create shape type \"BlockShape" + text + "\" for block: " + this.Name);
					return false;
				}
				if (!typeof(BlockShapeNew).IsAssignableFrom(typeWithPrefix) && !typeof(BlockShapeModelEntity).IsAssignableFrom(typeWithPrefix) && !typeof(BlockShapeExt3dModel).IsAssignableFrom(typeWithPrefix))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x020013BA RID: 5050
		public enum ModelOffsetType
		{
			// Token: 0x040079D6 RID: 31190
			None,
			// Token: 0x040079D7 RID: 31191
			Explicit,
			// Token: 0x040079D8 RID: 31192
			ShapeNew,
			// Token: 0x040079D9 RID: 31193
			ShapeModelEntity,
			// Token: 0x040079DA RID: 31194
			ShapeExt3dModel,
			// Token: 0x040079DB RID: 31195
			ShapeOther,
			// Token: 0x040079DC RID: 31196
			DefaultShapeNew
		}

		// Token: 0x020013BB RID: 5051
		public class ElementInfo
		{
			// Token: 0x17001100 RID: 4352
			// (get) Token: 0x06009E06 RID: 40454 RVA: 0x003EDE7B File Offset: 0x003EC07B
			// (set) Token: 0x06009E07 RID: 40455 RVA: 0x003EDE83 File Offset: 0x003EC083
			public bool CanInherit { get; set; } = true;

			// Token: 0x17001101 RID: 4353
			// (get) Token: 0x06009E08 RID: 40456 RVA: 0x003EDE8C File Offset: 0x003EC08C
			// (set) Token: 0x06009E09 RID: 40457 RVA: 0x003EDE94 File Offset: 0x003EC094
			public bool IsClass { get; set; } = true;

			// Token: 0x17001102 RID: 4354
			// (get) Token: 0x06009E0A RID: 40458 RVA: 0x003EDE9D File Offset: 0x003EC09D
			// (set) Token: 0x06009E0B RID: 40459 RVA: 0x003EDEA5 File Offset: 0x003EC0A5
			public XElement Element { get; set; }
		}
	}
}
