using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000146 RID: 326
[Preserve]
public class BlockTNT : Block
{
	// Token: 0x0600091C RID: 2332 RVA: 0x0003F41E File Offset: 0x0003D61E
	public override void Init()
	{
		base.Init();
		this.explosion = new ExplosionData(base.Properties, null);
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x0003F438 File Offset: 0x0003D638
	public override int OnBlockDamaged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _damagePoints, int _entityId, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
	{
		if (_world.GetGameRandom().RandomFloat <= (float)_damagePoints / (float)_blockValue.Block.MaxDamage)
		{
			this.explode(_world, _clrIdx, _blockPos, _entityId, 0.1f);
		}
		ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
		if (chunkCluster != null)
		{
			chunkCluster.InvokeOnBlockDamagedDelegates(_blockPos, _blockValue, _damagePoints, _entityId);
		}
		return _blockValue.damage;
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x0003F497 File Offset: 0x0003D697
	public override Block.DestroyedResult OnBlockDestroyedByExplosion(WorldBase _world, int _clrIdx, Vector3i _pos, BlockValue _blockValue, int _playerIdx)
	{
		base.OnBlockDestroyedByExplosion(_world, _clrIdx, _pos, _blockValue, _playerIdx);
		this.explode(_world, _clrIdx, _pos, _playerIdx, _world.GetGameRandom().RandomFloat * 0.5f + 0.3f);
		return Block.DestroyedResult.Remove;
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x0003F4CC File Offset: 0x0003D6CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void explode(WorldBase _world, int _clrIdx, Vector3i _expBlockPos, int _entityId, float _delay)
	{
		ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
		Vector3 worldPos = _expBlockPos.ToVector3();
		if (chunkCluster != null)
		{
			worldPos = chunkCluster.ToWorldPosition(_expBlockPos.ToVector3() + new Vector3(0.5f, 0.5f, 0.5f));
		}
		_world.GetGameManager().ExplosionServer(_clrIdx, worldPos, _expBlockPos, Quaternion.identity, this.explosion, _entityId, _delay, true, null);
	}

	// Token: 0x040008CA RID: 2250
	[PublicizedFrom(EAccessModifier.Private)]
	public ExplosionData explosion;
}
