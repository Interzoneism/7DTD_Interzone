using System;
using UnityEngine.Scripting;

// Token: 0x0200011B RID: 283
[Preserve]
public class BlockPlant : Block
{
	// Token: 0x060007BB RID: 1979 RVA: 0x00036690 File Offset: 0x00034890
	public BlockPlant()
	{
		this.IsRandomlyTick = true;
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x000366A6 File Offset: 0x000348A6
	public override bool CanPlaceBlockAt(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bOmitCollideCheck = false)
	{
		return base.CanPlaceBlockAt(_world, _clrIdx, _blockPos, _blockValue, _bOmitCollideCheck) && this.CanGrowOn(_world, _clrIdx, _blockPos - Vector3i.up, _blockValue);
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x000366D0 File Offset: 0x000348D0
	public virtual bool CanGrowOn(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValueOfPlant)
	{
		return GameManager.Instance.IsEditMode() || this.fertileLevel == 0 || _world.GetBlock(_clrIdx, _blockPos).Block.blockMaterial.FertileLevel >= this.fertileLevel;
	}

	// Token: 0x060007BE RID: 1982 RVA: 0x0003671A File Offset: 0x0003491A
	public override void OnNeighborBlockChange(WorldBase _world, int _clrIdx, Vector3i _myBlockPos, BlockValue _myBlockValue, Vector3i _blockPosThatChanged, BlockValue _newNeighborBlockValue, BlockValue _oldNeighborBlockValue)
	{
		base.OnNeighborBlockChange(_world, _clrIdx, _myBlockPos, _myBlockValue, _blockPosThatChanged, _newNeighborBlockValue, _oldNeighborBlockValue);
		if (!_myBlockValue.ischild)
		{
			this.CheckPlantAlive(_world, _clrIdx, _myBlockPos, _myBlockValue);
		}
	}

	// Token: 0x060007BF RID: 1983 RVA: 0x00036742 File Offset: 0x00034942
	public override bool UpdateTick(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bRandomTick, ulong _ticksIfLoaded, GameRandom _rnd)
	{
		this.CheckPlantAlive(_world, _clrIdx, _blockPos, _blockValue);
		return false;
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x00036751 File Offset: 0x00034951
	public virtual bool CheckPlantAlive(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (GameManager.Instance.IsEditMode())
		{
			return true;
		}
		if (!this.CanPlantStay(_world, _clrIdx, _blockPos, _blockValue))
		{
			_world.SetBlockRPC(_clrIdx, _blockPos, BlockValue.Air);
			return false;
		}
		return true;
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x00036780 File Offset: 0x00034980
	public override bool CanPlantStay(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		return GameManager.Instance.IsEditMode() || ((this.lightLevelStay == 0 || _world.GetBlockLightValue(_clrIdx, _blockPos) >= this.lightLevelStay || _world.GetBlockLightValue(_clrIdx, _blockPos + Vector3i.up) >= this.lightLevelStay || _world.IsOpenSkyAbove(_clrIdx, _blockPos.x, _blockPos.y, _blockPos.z)) && this.CanGrowOn(_world, _clrIdx, _blockPos - Vector3i.up, _blockValue));
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x000367FF File Offset: 0x000349FF
	public override BlockValue OnBlockPlaced(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, GameRandom _rnd)
	{
		_blockValue.rotation = (byte)_rnd.RandomRange(4);
		return _blockValue;
	}

	// Token: 0x04000834 RID: 2100
	[PublicizedFrom(EAccessModifier.Protected)]
	public int lightLevelStay;

	// Token: 0x04000835 RID: 2101
	[PublicizedFrom(EAccessModifier.Protected)]
	public int fertileLevel = 1;
}
