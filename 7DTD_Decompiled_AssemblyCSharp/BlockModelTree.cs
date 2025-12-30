using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000114 RID: 276
[Preserve]
public class BlockModelTree : BlockPlantGrowing
{
	// Token: 0x0600077E RID: 1918 RVA: 0x000351E0 File Offset: 0x000333E0
	public BlockModelTree()
	{
		this.fertileLevel = 1;
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x000351F0 File Offset: 0x000333F0
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("FallOver"))
		{
			this.bFallOver = StringParsers.ParseBool(base.Properties.Values["FallOver"], 0, -1, true);
		}
		else
		{
			this.bFallOver = true;
		}
		this.IsTerrainDecoration = true;
		this.CanDecorateOnSlopes = true;
		this.CanPlayersSpawnOn = false;
		this.CanMobsSpawnOn = false;
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x00035262 File Offset: 0x00033462
	public override bool UpdateTick(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bRandomTick, ulong _ticksIfLoaded, GameRandom _rnd)
	{
		return !_blockValue.ischild && base.UpdateTick(_world, _clrIdx, _blockPos, _blockValue, _bRandomTick, _ticksIfLoaded, _rnd);
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x00035280 File Offset: 0x00033480
	public override void OnNeighborBlockChange(WorldBase _world, int _clrIdx, Vector3i _myBlockPos, BlockValue _myBlockValue, Vector3i _blockPosThatChanged, BlockValue _newNeighborBlockValue, BlockValue _oldNeighborBlockValue)
	{
		if (!_myBlockValue.ischild)
		{
			base.OnNeighborBlockChange(_world, _clrIdx, _myBlockPos, _myBlockValue, _blockPosThatChanged, _newNeighborBlockValue, _oldNeighborBlockValue);
		}
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x0003529C File Offset: 0x0003349C
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		if (!_newBlockValue.ischild)
		{
			base.OnBlockValueChanged(_world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
		}
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x000352B6 File Offset: 0x000334B6
	public override bool CheckPlantAlive(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		return this.isPlantGrowingRandom || _world.IsRemote() || base.CheckPlantAlive(_world, _clrIdx, _blockPos, _blockValue);
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x000352D8 File Offset: 0x000334D8
	public override bool CanPlaceBlockAt(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bOmitCollideCheck = false)
	{
		if (!base.CanPlaceBlockAt(_world, _clrIdx, _blockPos, _blockValue, _bOmitCollideCheck))
		{
			return false;
		}
		for (int i = _blockPos.x - 3; i <= _blockPos.x + 3; i++)
		{
			for (int j = _blockPos.z - 3; j <= _blockPos.z + 3; j++)
			{
				for (int k = _blockPos.y - 6; k <= _blockPos.y + 6; k++)
				{
					if (_world.GetBlock(_clrIdx, new Vector3i(i, k, j)).Block is BlockModelTree)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x00035365 File Offset: 0x00033565
	public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
		this.removeAllTrunks(_world, _chunk, _blockPos, _blockValue, -1, false);
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x00035380 File Offset: 0x00033580
	[PublicizedFrom(EAccessModifier.Private)]
	public void removeAllTrunks(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, int _entityid, bool _bDropItemsAndStartParticle)
	{
		if (_chunk == null)
		{
			_chunk = (Chunk)_world.GetChunkFromWorldPos(_blockPos);
			if (_chunk == null)
			{
				return;
			}
		}
		if (_bDropItemsAndStartParticle)
		{
			this.dropItems(_world, _blockPos, _blockValue, _entityid);
			float lightBrightness = _world.GetLightBrightness(_blockPos);
			this.SpawnDestroyParticleEffect(_world, _blockValue, _blockPos, lightBrightness, base.GetColorForSide(_blockValue, BlockFace.Top), _entityid);
		}
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x000353D4 File Offset: 0x000335D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void dropItems(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, int _entityId)
	{
		_blockValue.Block.DropItemsOnEvent(_world, _blockValue, EnumDropEvent.Destroy, 1f, World.blockToTransformPos(_blockPos), new Vector3(0.5f, 0f, 0.5f), 0f, _entityId, true);
	}

	// Token: 0x06000788 RID: 1928 RVA: 0x00035417 File Offset: 0x00033617
	public override Block.DestroyedResult OnBlockDestroyedByExplosion(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _playerThatStartedExpl)
	{
		this.startToFall(_world, _clrIdx, _blockPos, _blockValue, -1);
		return Block.DestroyedResult.Keep;
	}

	// Token: 0x06000789 RID: 1929 RVA: 0x00035428 File Offset: 0x00033628
	public override void OnBlockStartsToFall(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (_blockValue.ischild)
		{
			return;
		}
		if (this.OnBlockDestroyedBy(_world, 0, _blockPos, _blockValue, -1, false) != Block.DestroyedResult.Keep)
		{
			base.OnBlockStartsToFall(_world, _blockPos, _blockValue);
			return;
		}
		float lightBrightness = _world.GetLightBrightness(_blockPos);
		this.SpawnDestroyParticleEffect(_world, _blockValue, _blockPos, lightBrightness, base.GetColorForSide(_blockValue, BlockFace.Top), -1);
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x00035472 File Offset: 0x00033672
	public override void SpawnDestroyParticleEffect(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, float _lightValue, Color _color, int _entityIdThatCaused)
	{
		base.SpawnDestroyParticleEffect(_world, _blockValue, _blockPos, _lightValue, _color, _entityIdThatCaused);
		_world.GetGameManager().PlaySoundAtPositionServer(_blockPos.ToVector3(), "trunkbreak", AudioRolloffMode.Logarithmic, 100, -1);
	}

	// Token: 0x0600078B RID: 1931 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool ShowModelOnFall()
	{
		return false;
	}

	// Token: 0x0600078C RID: 1932 RVA: 0x0003549E File Offset: 0x0003369E
	public override BlockValue OnBlockPlaced(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, GameRandom _rnd)
	{
		_blockValue.rotation = BiomeBlockDecoration.GetRandomRotation(_rnd.RandomFloat, 7);
		return _blockValue;
	}

	// Token: 0x0600078D RID: 1933 RVA: 0x000354B6 File Offset: 0x000336B6
	public override Block.DestroyedResult OnBlockDestroyedBy(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _entityId, bool _bUseHarvestTool)
	{
		if (!this.bFallOver)
		{
			return Block.DestroyedResult.Downgrade;
		}
		if (!this.startToFall(_world, _clrIdx, _blockPos, _blockValue, _entityId))
		{
			return Block.DestroyedResult.Downgrade;
		}
		return Block.DestroyedResult.Keep;
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x000354D4 File Offset: 0x000336D4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool startToFall(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _entityId)
	{
		Transform transform;
		if (!DecoManager.Instance.IsEnabled || !_blockValue.Block.IsDistantDecoration)
		{
			BlockEntityData blockEntity = ((Chunk)_world.GetChunkFromWorldPos(_blockPos)).GetBlockEntity(_blockPos);
			if (blockEntity == null || !blockEntity.bHasTransform)
			{
				return false;
			}
			transform = blockEntity.transform;
		}
		else
		{
			transform = DecoManager.Instance.GetDecorationTransform(_blockPos, false);
		}
		if (transform == null)
		{
			_world.SetBlockRPC(_blockPos, BlockValue.Air);
			return false;
		}
		_blockValue.damage = _blockValue.Block.MaxDamage;
		_world.SetBlocksRPC(new List<BlockChangeInfo>
		{
			new BlockChangeInfo(_blockPos, _blockValue, false, true)
		});
		Entity entity = _world.GetEntity(_entityId);
		EntityCreationData entityCreationData = new EntityCreationData();
		entityCreationData.entityClass = "fallingTree".GetHashCode();
		entityCreationData.blockPos = _blockPos;
		entityCreationData.fallTreeDir = ((entity != null) ? (transform.position + Origin.position - entity.GetPosition()) : new Vector3(_world.GetGameRandom().RandomFloat, 0f, _world.GetGameRandom().RandomFloat));
		entityCreationData.fallTreeDir.y = 0f;
		entityCreationData.fallTreeDir = entityCreationData.fallTreeDir.normalized;
		entityCreationData.pos = transform.position + Origin.position;
		entityCreationData.rot = transform.rotation.eulerAngles;
		entityCreationData.id = -1;
		_world.GetGameManager().RequestToSpawnEntityServer(entityCreationData);
		return true;
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x00035648 File Offset: 0x00033848
	public override int OnBlockDamaged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
	{
		if (!_world.IsRemote() && _blockValue.damage > _blockValue.Block.MaxDamage + 100)
		{
			_world.SetBlockRPC(_blockPos, BlockValue.Air);
		}
		return base.OnBlockDamaged(_world, _clrIdx, _blockPos, _blockValue, _damagePoints, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, _recDepth);
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsExplosionAffected()
	{
		return false;
	}

	// Token: 0x04000823 RID: 2083
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bFallOver;
}
