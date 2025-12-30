using System;
using UnityEngine;

// Token: 0x02000852 RID: 2130
public static class BlockHighlighter
{
	// Token: 0x06003D37 RID: 15671 RVA: 0x0018861B File Offset: 0x0018681B
	public static void AddBlock(Vector3i _pos)
	{
		BlockHighlighter.EnforceGo();
		BlockHighlighter.EnforceTemplateLoaded();
		UnityEngine.Object.Instantiate<GameObject>(BlockHighlighter.blockPrefab, BlockHighlighter.topGameObject.transform).transform.position = _pos.ToVector3() + BlockHighlighter.halfBlockOffset;
	}

	// Token: 0x06003D38 RID: 15672 RVA: 0x00188656 File Offset: 0x00186856
	[PublicizedFrom(EAccessModifier.Private)]
	public static void EnforceGo()
	{
		if (BlockHighlighter.topGameObject != null)
		{
			return;
		}
		BlockHighlighter.topGameObject = new GameObject("BlockHighlighter");
	}

	// Token: 0x06003D39 RID: 15673 RVA: 0x00188675 File Offset: 0x00186875
	[PublicizedFrom(EAccessModifier.Private)]
	public static void EnforceTemplateLoaded()
	{
		if (BlockHighlighter.blockPrefab != null)
		{
			return;
		}
		BlockHighlighter.blockPrefab = DataLoader.LoadAsset<GameObject>("@:Entities/Misc/block_highlightPrefab.prefab", false);
	}

	// Token: 0x06003D3A RID: 15674 RVA: 0x00188695 File Offset: 0x00186895
	public static void Cleanup()
	{
		if (BlockHighlighter.topGameObject != null)
		{
			UnityEngine.Object.Destroy(BlockHighlighter.topGameObject);
			BlockHighlighter.topGameObject = null;
		}
	}

	// Token: 0x04003181 RID: 12673
	[PublicizedFrom(EAccessModifier.Private)]
	public const string TemplatePath = "@:Entities/Misc/block_highlightPrefab.prefab";

	// Token: 0x04003182 RID: 12674
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Vector3 halfBlockOffset = new Vector3(0.5f, 0.5f, 0.5f);

	// Token: 0x04003183 RID: 12675
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObject topGameObject;

	// Token: 0x04003184 RID: 12676
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObject blockPrefab;
}
