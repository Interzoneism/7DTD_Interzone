using System;
using UnityEngine.Scripting;

// Token: 0x02000102 RID: 258
[Preserve]
public class BlockGore : BlockLoot
{
	// Token: 0x060006CA RID: 1738 RVA: 0x0002FE1E File Offset: 0x0002E01E
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("GoreTime"))
		{
			int.TryParse(base.Properties.Values["GoreTime"], out this.timeInMinutes);
		}
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsWaterBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, BlockFaceFlag _sides)
	{
		return false;
	}

	// Token: 0x060006CC RID: 1740 RVA: 0x0002FE60 File Offset: 0x0002E060
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void addTileEntity(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		TileEntityGoreBlock tileEntityGoreBlock = new TileEntityGoreBlock(_chunk);
		tileEntityGoreBlock.localChunkPos = World.toBlock(_blockPos);
		tileEntityGoreBlock.lootListName = this.lootList;
		tileEntityGoreBlock.SetContainerSize(LootContainer.GetLootContainer(this.lootList, true).size, true);
		if (this.timeInMinutes > 0)
		{
			tileEntityGoreBlock.tickTimeToRemove = GameTimer.Instance.ticks + (ulong)((long)(1200 * this.timeInMinutes));
		}
		_chunk.AddTileEntity(tileEntityGoreBlock);
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x0002FED2 File Offset: 0x0002E0D2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void removeTileEntity(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		_chunk.RemoveTileEntityAt<TileEntityGoreBlock>((World)world, World.toBlock(_blockPos));
	}

	// Token: 0x040007C7 RID: 1991
	public const int cDefaultTimeMinutes = 50;

	// Token: 0x040007C8 RID: 1992
	[PublicizedFrom(EAccessModifier.Private)]
	public int timeInMinutes;
}
