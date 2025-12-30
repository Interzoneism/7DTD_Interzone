using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200014F RID: 335
[Preserve]
public class BlockUpgradeRated : Block
{
	// Token: 0x0600094E RID: 2382 RVA: 0x00040150 File Offset: 0x0003E350
	public override void LateInit()
	{
		base.LateInit();
		if (base.Properties.Values.ContainsKey(BlockUpgradeRated.PropUpgradeToBlock))
		{
			ItemValue item = ItemClass.GetItem(base.Properties.Values[BlockUpgradeRated.PropUpgradeToBlock], false);
			this.upgradeBlock = item.ToBlockValue();
		}
		if (base.Properties.Values.ContainsKey(BlockUpgradeRated.PropUpgradeBlockCombined))
		{
			this.upgradeBlockCombined = ItemClass.GetItem(base.Properties.Values[BlockUpgradeRated.PropUpgradeBlockCombined], false).ToBlockValue();
			if (this.upgradeBlockCombined.Equals(BlockValue.Air))
			{
				throw new Exception("Block with name '" + base.Properties.Values[BlockUpgradeRated.PropUpgradeBlockCombined] + "' not found!");
			}
		}
		if (base.Properties.Values.ContainsKey(BlockUpgradeRated.PropUpgradeRate))
		{
			this.upgradeRate = int.Parse(base.Properties.Values[BlockUpgradeRated.PropUpgradeRate]);
		}
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x00040254 File Offset: 0x0003E454
	public override void OnNeighborBlockChange(WorldBase world, int _clrIdx, Vector3i _myBlockPos, BlockValue _myBlockValue, Vector3i _blockPosThatChanged, BlockValue _newNeighborBlockValue, BlockValue _oldNeighborBlockValue)
	{
		if (this.upgradeBlockCombined.isair)
		{
			return;
		}
		Block block = _newNeighborBlockValue.Block;
		if (block is BlockUpgradeRated && ((BlockUpgradeRated)block).upgradeBlockCombined.type == this.upgradeBlockCombined.type)
		{
			world.SetBlockRPC(_clrIdx, _blockPosThatChanged, this.upgradeBlockCombined);
			world.SetBlockRPC(_clrIdx, _myBlockPos, this.upgradeBlockCombined);
		}
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x000402BC File Offset: 0x0003E4BC
	public override bool UpdateTick(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bRandomTick, ulong _ticksIfLoaded, GameRandom _rnd)
	{
		base.UpdateTick(_world, _clrIdx, _blockPos, _blockValue, _bRandomTick, _ticksIfLoaded, _rnd);
		BlockValue blockValue = BlockPlaceholderMap.Instance.Replace(this.upgradeBlock, _world.GetGameRandom(), _blockPos.x, _blockPos.z, false);
		blockValue.rotation = _blockValue.rotation;
		blockValue.meta = 0;
		_blockValue = blockValue;
		Block block = _blockValue.Block;
		if (_blockValue.damage >= block.blockMaterial.MaxDamage)
		{
			_blockValue.damage = block.blockMaterial.MaxDamage - 1;
		}
		if (block.shape.IsTerrain())
		{
			_world.SetBlockRPC(_clrIdx, _blockPos, _blockValue, block.Density);
		}
		else
		{
			_world.SetBlockRPC(_clrIdx, _blockPos, _blockValue);
		}
		return true;
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x00040374 File Offset: 0x0003E574
	public override void CheckUpdate(BlockValue _oldBV, BlockValue _newBV, out bool bUpdateMesh, out bool bUpdateNotify, out bool bUpdateLight)
	{
		if (_oldBV.type == _newBV.type && _oldBV.damage == _newBV.damage)
		{
			bUpdateMesh = (bUpdateNotify = (bUpdateLight = false));
			return;
		}
		bUpdateMesh = (bUpdateNotify = (bUpdateLight = true));
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x000403C0 File Offset: 0x0003E5C0
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (!_world.IsRemote())
		{
			int num = this.upgradeRate;
			int num2 = num / 4;
			int b = num / 2;
			GameRandom gameRandom = _world.GetGameRandom();
			int num3;
			int num4;
			do
			{
				float randomGaussian = gameRandom.RandomGaussian;
				num3 = Mathf.RoundToInt((float)num + (float)num2 * randomGaussian);
				num4 = Mathf.Max(num3, b);
			}
			while (num4 != num3);
			_world.GetWBT().AddScheduledBlockUpdate(_chunk.ClrIdx, _blockPos, this.blockID, (ulong)((long)num4));
		}
	}

	// Token: 0x040008DC RID: 2268
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropUpgradeToBlock = "UpgradeRated.ToBlock";

	// Token: 0x040008DD RID: 2269
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropUpgradeBlockCombined = "UpgradeRated.BlockCombined";

	// Token: 0x040008DE RID: 2270
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropUpgradeRate = "UpgradeRated.Rate";

	// Token: 0x040008DF RID: 2271
	[PublicizedFrom(EAccessModifier.Protected)]
	public BlockValue upgradeBlock;

	// Token: 0x040008E0 RID: 2272
	[PublicizedFrom(EAccessModifier.Protected)]
	public BlockValue upgradeBlockCombined;

	// Token: 0x040008E1 RID: 2273
	[PublicizedFrom(EAccessModifier.Protected)]
	public int upgradeRate;
}
