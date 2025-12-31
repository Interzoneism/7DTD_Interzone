using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000113 RID: 275
[Preserve]
public class BlockMine : Block
{
	// Token: 0x17000078 RID: 120
	// (get) Token: 0x06000775 RID: 1909 RVA: 0x00034EAA File Offset: 0x000330AA
	public ExplosionData Explosion
	{
		get
		{
			return this.explosion;
		}
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x00034EB4 File Offset: 0x000330B4
	public override void Init()
	{
		base.Init();
		this.explosion = new ExplosionData(base.Properties, null);
		this.BaseEntityDamage = this.explosion.EntityDamage;
		if (base.Properties.Values.ContainsKey(BlockMine.PropTriggerDelay))
		{
			this.TriggerDelay = StringParsers.ParseFloat(base.Properties.Values[BlockMine.PropTriggerDelay], 0, -1, NumberStyles.Any);
		}
		if (base.Properties.Values.ContainsKey(BlockMine.PropTriggerSound))
		{
			this.TriggerSound = base.Properties.Values[BlockMine.PropTriggerSound];
		}
		base.Properties.ParseBool(BlockMine.PropNoImmunity, ref this.NoImmunity);
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x00034F70 File Offset: 0x00033170
	public override void OnEntityWalking(WorldBase _world, int _x, int _y, int _z, BlockValue _blockValue, Entity entity)
	{
		if (this.NoImmunity || EffectManager.GetValue(PassiveEffects.LandMineImmunity, null, 0f, entity as EntityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) == 0f)
		{
			if (entity as EntityPlayer != null)
			{
				if ((entity as EntityPlayer).IsSpectator)
				{
					return;
				}
				GameManager.Instance.PlaySoundAtPositionServer(new Vector3((float)_x, (float)_y, (float)_z), this.TriggerSound, AudioRolloffMode.Linear, 5, entity.entityId);
			}
			float num = this.TriggerDelay;
			if (entity as EntityAlive != null)
			{
				num = EffectManager.GetValue(PassiveEffects.LandMineTriggerDelay, null, this.TriggerDelay, entity as EntityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			}
			this.explosion.EntityDamage = EffectManager.GetValue(PassiveEffects.TrapIncomingDamage, null, this.BaseEntityDamage, entity as EntityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			_world.GetWBT().AddScheduledBlockUpdate((_world.GetChunkFromWorldPos(_x, _y, _z) as Chunk).ClrIdx, new Vector3i(_x, _y, _z), this.blockID, (ulong)(num * 20f));
		}
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x000350A8 File Offset: 0x000332A8
	public override int OnBlockDamaged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
	{
		if (_damagePoints >= 0)
		{
			float num = (float)Utils.FastClamp(_damagePoints, 1, _blockValue.Block.MaxDamage - 1);
			if (_world.GetGameRandom().RandomFloat <= num / (float)_blockValue.Block.MaxDamage)
			{
				this.explode(_world, _clrIdx, _blockPos.ToVector3(), _entityIdThatDamaged);
			}
		}
		else
		{
			base.OnBlockDamaged(_world, _clrIdx, _blockPos, _blockValue, _damagePoints, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, _recDepth);
		}
		return _blockValue.damage;
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x00035120 File Offset: 0x00033320
	public override Block.DestroyedResult OnBlockDestroyedByExplosion(WorldBase _world, int _clrIdx, Vector3i _pos, BlockValue _blockValue, int _playerIdx)
	{
		if (_world.GetGameRandom().RandomFloat < 0.33f)
		{
			this.explode(_world, _clrIdx, _pos.ToVector3(), _playerIdx);
			return Block.DestroyedResult.Remove;
		}
		return Block.DestroyedResult.Keep;
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x00035148 File Offset: 0x00033348
	[PublicizedFrom(EAccessModifier.Private)]
	public void explode(WorldBase _world, int _clrIdx, Vector3 _pos, int _entityId)
	{
		ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
		if (chunkCluster != null)
		{
			_pos = chunkCluster.ToWorldPosition(_pos + new Vector3(0.5f, 0.5f, 0.5f));
		}
		_world.GetGameManager().ExplosionServer(_clrIdx, _pos, World.worldToBlockPos(_pos), Quaternion.identity, this.explosion, -1, 0.1f, true, null);
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue blockDef, BlockFace face)
	{
		return false;
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x000351AD File Offset: 0x000333AD
	public override bool UpdateTick(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bRandomTick, ulong _ticksIfLoaded, GameRandom _rnd)
	{
		this.explode(_world, _clrIdx, _blockPos.ToVector3(), -1);
		return true;
	}

	// Token: 0x0400081B RID: 2075
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropTriggerDelay = "TriggerDelay";

	// Token: 0x0400081C RID: 2076
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropTriggerSound = "TriggerSound";

	// Token: 0x0400081D RID: 2077
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropNoImmunity = "NoImmunity";

	// Token: 0x0400081E RID: 2078
	[PublicizedFrom(EAccessModifier.Protected)]
	public ExplosionData explosion;

	// Token: 0x0400081F RID: 2079
	[PublicizedFrom(EAccessModifier.Private)]
	public float TriggerDelay = 0.6f;

	// Token: 0x04000820 RID: 2080
	[PublicizedFrom(EAccessModifier.Private)]
	public string TriggerSound = "landmine_trigger";

	// Token: 0x04000821 RID: 2081
	[PublicizedFrom(EAccessModifier.Private)]
	public float BaseEntityDamage;

	// Token: 0x04000822 RID: 2082
	[PublicizedFrom(EAccessModifier.Private)]
	public bool NoImmunity;
}
