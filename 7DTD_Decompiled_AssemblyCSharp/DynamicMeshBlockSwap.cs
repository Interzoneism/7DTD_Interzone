using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ConcurrentCollections;

// Token: 0x02000313 RID: 787
public class DynamicMeshBlockSwap
{
	// Token: 0x06001655 RID: 5717 RVA: 0x00081F10 File Offset: 0x00080110
	public static bool IsValidBlock(int type)
	{
		return DynamicMeshBlockSwap.OpaqueBlocks.Contains(type) || DynamicMeshBlockSwap.TerrainBlocks.Contains(type);
	}

	// Token: 0x06001656 RID: 5718 RVA: 0x00081F2C File Offset: 0x0008012C
	public static void Init()
	{
		DynamicMeshBlockSwap.BlockSwaps.Clear();
		DynamicMeshBlockSwap.TextureSwaps.Clear();
		DynamicMeshBlockSwap.OpaqueBlocks.Clear();
		DynamicMeshBlockSwap.DoorBlocks.Clear();
		DynamicMeshBlockSwap.TerrainBlocks.Clear();
		DynamicMeshBlockSwap.DoorReplacement = Block.GetBlockValue("imposterBlock", true);
		if (DynamicMeshBlockSwap.DoorReplacement.isair)
		{
			Log.Warning("Dynamic mesh door replacement block not found");
		}
		else
		{
			Log.Out("Dymesh door replacement: " + DynamicMeshBlockSwap.DoorReplacement.Block.GetBlockName());
		}
		Type typeFromHandle = typeof(BlockDoorSecure);
		Type typeFromHandle2 = typeof(BlockDoor);
		foreach (Block block in Block.list)
		{
			if (block != null && block.blockID != 0)
			{
				bool flag = typeFromHandle2.IsAssignableFrom(block.GetType()) || typeFromHandle.IsAssignableFrom(block.GetType());
				if (block.MeshIndex == 0 || flag)
				{
					int type = Block.GetBlockValue(block.GetBlockName(), false).type;
					bool flag2 = block is BlockModelTree;
					bool flag3 = block.shape is BlockShapeModelEntity;
					if (!flag2 && !block.IsPlant() && !flag3)
					{
						DynamicMeshBlockSwap.OpaqueBlocks.Add(type);
					}
					if (flag)
					{
						DynamicMeshBlockSwap.DoorBlocks.Add(type);
					}
					if (block.bImposterExcludeAndStop || block.bImposterExclude || (block.IsTerrainDecoration && block.ImposterExchange == 0))
					{
						DynamicMeshBlockSwap.BlockSwaps.TryAdd(block.blockID, 0);
					}
					else if (block.ImposterExchange != 0)
					{
						DynamicMeshBlockSwap.BlockSwaps.TryAdd(block.blockID, block.ImposterExchange);
						DynamicMeshBlockSwap.TextureSwaps.TryAdd(block.blockID, (long)((ulong)block.ImposterExchangeTexIdx));
					}
				}
				else if (block.MeshIndex == 5)
				{
					int type2 = Block.GetBlockValue(block.GetBlockName(), false).type;
					DynamicMeshBlockSwap.TerrainBlocks.Add(type2);
				}
			}
		}
	}

	// Token: 0x04000E16 RID: 3606
	public static BlockValue DoorReplacement;

	// Token: 0x04000E17 RID: 3607
	public static ConcurrentHashSet<int> DoorBlocks = new ConcurrentHashSet<int>();

	// Token: 0x04000E18 RID: 3608
	public static ConcurrentHashSet<int> OpaqueBlocks = new ConcurrentHashSet<int>();

	// Token: 0x04000E19 RID: 3609
	public static ConcurrentHashSet<int> TerrainBlocks = new ConcurrentHashSet<int>();

	// Token: 0x04000E1A RID: 3610
	public static ConcurrentDictionary<int, int> BlockSwaps = new ConcurrentDictionary<int, int>();

	// Token: 0x04000E1B RID: 3611
	public static ConcurrentDictionary<int, long> TextureSwaps = new ConcurrentDictionary<int, long>();

	// Token: 0x04000E1C RID: 3612
	public static HashSet<int> InvalidPaintIds = new HashSet<int>();
}
