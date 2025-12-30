using System;
using UnityEngine.Scripting;

// Token: 0x0200010D RID: 269
[Preserve]
public class BlockLiquidSource : Block
{
	// Token: 0x0600073D RID: 1853 RVA: 0x00032F34 File Offset: 0x00031134
	public BlockLiquidSource()
	{
		Vector3i[,] array = new Vector3i[8, 4];
		array[0, 0] = new Vector3i(-1, 0, 0);
		array[0, 1] = new Vector3i(0, 0, -1);
		array[0, 2] = new Vector3i(1, 0, 0);
		array[0, 3] = new Vector3i(0, 0, 1);
		array[1, 0] = new Vector3i(1, 0, 0);
		array[1, 1] = new Vector3i(0, 0, -1);
		array[1, 2] = new Vector3i(0, 0, 1);
		array[1, 3] = new Vector3i(-1, 0, 0);
		array[2, 0] = new Vector3i(0, 0, 1);
		array[2, 1] = new Vector3i(-1, 0, 0);
		array[2, 2] = new Vector3i(0, 0, -1);
		array[2, 3] = new Vector3i(1, 0, 0);
		array[3, 0] = new Vector3i(0, 0, -1);
		array[3, 1] = new Vector3i(0, 0, 1);
		array[3, 2] = new Vector3i(1, 0, 0);
		array[3, 3] = new Vector3i(-1, 0, 0);
		array[4, 0] = new Vector3i(-1, 0, 0);
		array[4, 1] = new Vector3i(1, 0, 0);
		array[4, 2] = new Vector3i(0, 0, -1);
		array[4, 3] = new Vector3i(0, 0, 1);
		array[5, 0] = new Vector3i(1, 0, 0);
		array[5, 1] = new Vector3i(-1, 0, 0);
		array[5, 2] = new Vector3i(0, 0, 1);
		array[5, 3] = new Vector3i(0, 0, -1);
		array[6, 0] = new Vector3i(0, 0, 1);
		array[6, 1] = new Vector3i(-1, 0, 0);
		array[6, 2] = new Vector3i(1, 0, 0);
		array[6, 3] = new Vector3i(0, 0, -1);
		array[7, 0] = new Vector3i(1, 0, 0);
		array[7, 1] = new Vector3i(-1, 0, 0);
		array[7, 2] = new Vector3i(0, 0, 1);
		array[7, 3] = new Vector3i(0, 0, -1);
		this.fallDirsSet = array;
		base..ctor();
		this.IsRandomlyTick = false;
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x0003315C File Offset: 0x0003135C
	public override void LateInit()
	{
		base.LateInit();
		ItemValue item = ItemClass.GetItem("water", false);
		if (item != null)
		{
			this.waterBlock = item.ToBlockValue();
			return;
		}
		this.waterBlock = BlockValue.Air;
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x00033198 File Offset: 0x00031398
	public override void OnNeighborBlockChange(WorldBase world, int _clrIdx, Vector3i _myBlockPos, BlockValue _myBlockValue, Vector3i _blockPosThatChanged, BlockValue _newNeighborBlockValue, BlockValue _oldNeighborBlockValue)
	{
		base.OnNeighborBlockChange(world, _clrIdx, _myBlockPos, _myBlockValue, _blockPosThatChanged, _newNeighborBlockValue, _oldNeighborBlockValue);
		if (_newNeighborBlockValue.isair)
		{
			_myBlockValue.meta = 1 - _myBlockValue.meta;
			world.SetBlockRPC(_clrIdx, _myBlockPos, _myBlockValue);
			world.GetWBT().AddScheduledBlockUpdate(_clrIdx, _myBlockPos, this.blockID, 1UL);
		}
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsMovementBlocked(IBlockAccess world, Vector3i _blockPos, BlockValue _blockValue, BlockFace crossingFace)
	{
		return false;
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x000331EF File Offset: 0x000313EF
	public override ulong GetTickRate()
	{
		return 20UL;
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x000331F4 File Offset: 0x000313F4
	public override bool UpdateTick(WorldBase world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bRandomTick, ulong _ticksIfLoaded, GameRandom _rnd)
	{
		BlockValue blockValue;
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (i != 0 || j != 0)
				{
					this.emitionPos = _blockPos;
					this.emitionPos.x = this.emitionPos.x + j;
					this.emitionPos.z = this.emitionPos.z + i;
					BlockValue block = world.GetBlock(_clrIdx, this.emitionPos.x, this.emitionPos.y, this.emitionPos.z);
					if (block.isair || block.Block.blockMaterial.IsPlant)
					{
						blockValue = new BlockValue((uint)this.waterBlock.type);
						blockValue.meta = 14;
						blockValue.meta2 = 8;
						world.SetBlockRPC(this.emitionPos, blockValue);
						world.GetWBT().AddScheduledBlockUpdate(_clrIdx, this.emitionPos, BlockValue.Air.type, 1UL);
					}
				}
			}
		}
		blockValue = new BlockValue((uint)this.waterBlock.type)
		{
			meta = 14,
			meta2 = 0
		};
		world.SetBlockRPC(_blockPos, blockValue);
		world.GetWBT().AddScheduledBlockUpdate(0, _blockPos, this.blockID, 1UL);
		return true;
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x0003332C File Offset: 0x0003152C
	public override void OnBlockAdded(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (!world.IsRemote())
		{
			_blockValue.damage = this.Count;
			world.SetBlockRPC(_blockPos, _blockValue);
			world.GetWBT().AddScheduledBlockUpdate(0, _blockPos, this.blockID, 1UL);
		}
	}

	// Token: 0x040007F7 RID: 2039
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue waterBlock;

	// Token: 0x040007F8 RID: 2040
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i[,] fallDirsSet;

	// Token: 0x040007F9 RID: 2041
	[PublicizedFrom(EAccessModifier.Private)]
	public static int fallSet;

	// Token: 0x040007FA RID: 2042
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i emitionPos;
}
