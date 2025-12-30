using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace XMLEditing
{
	// Token: 0x020013BC RID: 5052
	public class BlockNodeMap : IEnumerable<KeyValuePair<string, BlockNode>>, IEnumerable
	{
		// Token: 0x06009E0D RID: 40461 RVA: 0x003EDEC4 File Offset: 0x003EC0C4
		public IEnumerator<KeyValuePair<string, BlockNode>> GetEnumerator()
		{
			return this.blockNodes.GetEnumerator();
		}

		// Token: 0x06009E0E RID: 40462 RVA: 0x003EDEC4 File Offset: 0x003EC0C4
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetEnumerator()
		{
			return this.blockNodes.GetEnumerator();
		}

		// Token: 0x06009E0F RID: 40463 RVA: 0x003EDED6 File Offset: 0x003EC0D6
		public bool TryGetValue(string targetName, out BlockNode blockNode)
		{
			return this.blockNodes.TryGetValue(targetName, out blockNode);
		}

		// Token: 0x17001103 RID: 4355
		// (get) Token: 0x06009E10 RID: 40464 RVA: 0x003EDEE5 File Offset: 0x003EC0E5
		public int Count
		{
			get
			{
				return this.blockNodes.Count;
			}
		}

		// Token: 0x06009E11 RID: 40465 RVA: 0x003EDEF4 File Offset: 0x003EC0F4
		public void PopulateFromFile(string blocksFilePath)
		{
			XDocument xdocument = XMLUtils.LoadXDocument(blocksFilePath);
			this.root = xdocument.Root;
			this.Refresh();
		}

		// Token: 0x06009E12 RID: 40466 RVA: 0x003EDF1A File Offset: 0x003EC11A
		public void PopulateFromRoot(XElement root)
		{
			this.root = root;
			this.Refresh();
		}

		// Token: 0x06009E13 RID: 40467 RVA: 0x003EDF2C File Offset: 0x003EC12C
		public void Refresh()
		{
			if (this.root == null)
			{
				Debug.LogError("Refresh failed: root element is null. This may occur if you have not called one of the PopulateFrom[...] methods prior to calling Refresh. Otherwise there may be an error in the source xml.");
				return;
			}
			if (!this.root.HasElements)
			{
				Debug.LogError("Refresh failed: root element has no child elements.");
				return;
			}
			this.blockNodes.Clear();
			foreach (XElement element in this.root.Elements(XNames.block))
			{
				string attribute = element.GetAttribute(XNames.name);
				BlockNode value = new BlockNode
				{
					Name = attribute,
					Element = element
				};
				this.blockNodes[attribute] = value;
			}
			foreach (BlockNode blockNode in this.blockNodes.Values)
			{
				foreach (XElement element2 in blockNode.Element.Elements(XNames.property))
				{
					string text = element2.GetAttribute(XNames.name);
					if (text == "Extends")
					{
						string attribute2 = element2.GetAttribute(XNames.value);
						BlockNode blockNode2;
						if (this.blockNodes.TryGetValue(attribute2, out blockNode2))
						{
							blockNode2.AddChild(blockNode);
							foreach (string key in element2.GetAttribute(XNames.param1).Split(new char[]
							{
								','
							}, StringSplitOptions.RemoveEmptyEntries))
							{
								BlockNode.ElementInfo elementInfo;
								if (!blockNode.ElementInfos.TryGetValue(key, out elementInfo))
								{
									elementInfo = new BlockNode.ElementInfo();
									blockNode.ElementInfos[key] = elementInfo;
								}
								elementInfo.CanInherit = false;
							}
							BlockNode.ElementInfo elementInfo2 = new BlockNode.ElementInfo();
							elementInfo2.CanInherit = false;
							elementInfo2.Element = element2;
							elementInfo2.IsClass = false;
							blockNode.ElementInfos["Extends"] = elementInfo2;
						}
						else
						{
							Debug.LogError(string.Concat(new string[]
							{
								"Failed to find parent BlockNode \"",
								attribute2,
								"\" for block \"",
								blockNode.Name,
								"\""
							}));
						}
					}
					else
					{
						bool isClass = false;
						if (string.IsNullOrWhiteSpace(text))
						{
							string attribute3 = element2.GetAttribute(XNames.class_);
							if (string.IsNullOrWhiteSpace(attribute3))
							{
								continue;
							}
							isClass = true;
							text = attribute3;
						}
						BlockNode.ElementInfo elementInfo3;
						if (!blockNode.ElementInfos.TryGetValue(text, out elementInfo3))
						{
							elementInfo3 = new BlockNode.ElementInfo();
							elementInfo3.CanInherit = (text != "CreativeMode");
							blockNode.ElementInfos[text] = elementInfo3;
						}
						elementInfo3.Element = element2;
						elementInfo3.IsClass = isClass;
					}
				}
			}
		}

		// Token: 0x040079E0 RID: 31200
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<string, BlockNode> blockNodes = new Dictionary<string, BlockNode>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x040079E1 RID: 31201
		[PublicizedFrom(EAccessModifier.Private)]
		public XElement root;
	}
}
