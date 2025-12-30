using System;
using Audio;
using UnityEngine.Scripting;

// Token: 0x02000125 RID: 293
[Preserve]
public class BlockBladeTrap : BlockPoweredTrap
{
	// Token: 0x06000824 RID: 2084 RVA: 0x000390C4 File Offset: 0x000372C4
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("RunningSound"))
		{
			this.runningSound = base.Properties.Values["RunningSound"];
		}
		if (base.Properties.Values.ContainsKey("RunningSoundBreaking"))
		{
			this.runningSoundPartlyBroken = base.Properties.Values["RunningSoundBreaking"];
		}
		if (base.Properties.Values.ContainsKey("RunningSoundBroken"))
		{
			this.runningSoundBroken = base.Properties.Values["RunningSoundBroken"];
		}
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x00039170 File Offset: 0x00037370
	public override bool ActivateTrap(BlockEntityData blockEntity, bool isOn)
	{
		SpinningBladeTrapController component = blockEntity.transform.gameObject.GetComponent<SpinningBladeTrapController>();
		if (component == null)
		{
			return false;
		}
		component.Init(base.Properties, this);
		component.BlockPosition = blockEntity.pos;
		component.HealthRatio = 1f - (float)blockEntity.blockValue.damage / (float)blockEntity.blockValue.Block.MaxDamage;
		component.IsOn = isOn;
		return true;
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x000391E4 File Offset: 0x000373E4
	public override int OnBlockDamaged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
	{
		ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
		if (chunkCluster != null)
		{
			IChunk chunkSync = chunkCluster.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z));
			if (chunkSync != null)
			{
				BlockEntityData blockEntity = chunkSync.GetBlockEntity(_blockPos);
				if (blockEntity != null && blockEntity.bHasTransform)
				{
					SpinningBladeTrapController component = blockEntity.transform.gameObject.GetComponent<SpinningBladeTrapController>();
					if (component != null)
					{
						component.HealthRatio = 1f - (float)blockEntity.blockValue.damage / (float)blockEntity.blockValue.Block.MaxDamage;
					}
				}
			}
		}
		return base.OnBlockDamaged(_world, _clrIdx, _blockPos, _blockValue, _damagePoints, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, _recDepth);
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x000392A0 File Offset: 0x000374A0
	public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		Manager.BroadcastStop(_blockPos.ToVector3(), this.runningSound);
		Manager.BroadcastStop(_blockPos.ToVector3(), this.runningSoundPartlyBroken);
		Manager.BroadcastStop(_blockPos.ToVector3(), this.runningSoundBroken);
		base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x000392F0 File Offset: 0x000374F0
	public override void OnBlockUnloaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		Manager.Stop(_blockPos.ToVector3(), this.runningSound);
		Manager.Stop(_blockPos.ToVector3(), this.runningSoundPartlyBroken);
		Manager.Stop(_blockPos.ToVector3(), this.runningSoundBroken);
		base.OnBlockUnloaded(_world, _clrIdx, _blockPos, _blockValue);
	}

	// Token: 0x04000861 RID: 2145
	[PublicizedFrom(EAccessModifier.Private)]
	public string runningSound = "Electricity/BladeTrap/bladetrap_fire_lp";

	// Token: 0x04000862 RID: 2146
	[PublicizedFrom(EAccessModifier.Private)]
	public string runningSoundPartlyBroken = "Electricity/BladeTrap/bladetrap_dm1_lp";

	// Token: 0x04000863 RID: 2147
	[PublicizedFrom(EAccessModifier.Private)]
	public string runningSoundBroken = "Electricity/BladeTrap/bladetrap_dm2_lp";
}
