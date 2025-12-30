using System;
using UnityEngine.Scripting;

// Token: 0x02000143 RID: 323
[Preserve]
public class BlockTallgrass : BlockPlant
{
	// Token: 0x06000909 RID: 2313 RVA: 0x0003F048 File Offset: 0x0003D248
	public BlockTallgrass()
	{
		this.IsRandomlyTick = false;
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x0003F057 File Offset: 0x0003D257
	public override bool CheckPlantAlive(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!this.CanPlantStay(_world, _clrIdx, _blockPos, _blockValue))
		{
			_world.SetBlockRPC(_clrIdx, _blockPos, BlockValue.Air);
			return false;
		}
		return true;
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x0003F076 File Offset: 0x0003D276
	public override void OnBlockPlaceBefore(WorldBase _world, ref BlockPlacement.Result _bpResult, EntityAlive _ea, GameRandom _rnd)
	{
		_bpResult.blockValue.meta2and1 = this.CalcMeta(_rnd);
		_bpResult.blockValue.rotation = (byte)_rnd.RandomRange(32);
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x0003F0A0 File Offset: 0x0003D2A0
	public override BlockValue OnBlockPlaced(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, GameRandom _rnd)
	{
		_blockValue.meta2and1 = this.CalcMeta(_rnd);
		_blockValue.rotation = (byte)_rnd.RandomRange(32);
		return _blockValue;
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x0003F0C4 File Offset: 0x0003D2C4
	[PublicizedFrom(EAccessModifier.Private)]
	public byte CalcMeta(GameRandom _rnd)
	{
		return (byte)(_rnd.RandomRange(6) | (_rnd.RandomRange(256) & -8));
	}
}
