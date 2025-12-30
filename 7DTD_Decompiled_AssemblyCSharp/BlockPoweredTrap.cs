using System;
using System.Collections.Generic;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000124 RID: 292
[Preserve]
public class BlockPoweredTrap : BlockPowered
{
	// Token: 0x06000817 RID: 2071 RVA: 0x00038C78 File Offset: 0x00036E78
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey(BlockPoweredTrap.PropDamage))
		{
			int.TryParse(base.Properties.Values[BlockPoweredTrap.PropDamage], out this.damage);
		}
		else
		{
			this.damage = 0;
		}
		if (base.Properties.Values.ContainsKey(BlockPoweredTrap.PropDamageReceived))
		{
			int.TryParse(base.Properties.Values[BlockPoweredTrap.PropDamageReceived], out this.damageReceived);
			return;
		}
		this.damageReceived = 0;
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x00038D0C File Offset: 0x00036F0C
	public override void GetCollisionAABB(BlockValue _blockValue, int _x, int _y, int _z, float _distortedY, List<Bounds> _result)
	{
		base.GetCollisionAABB(_blockValue, _x, _y, _z, _distortedY, _result);
		Vector3 b = new Vector3(0.05f, 0.05f, 0.05f);
		for (int i = 0; i < _result.Count; i++)
		{
			Bounds value = _result[i];
			value.SetMinMax(value.min - b, value.max + b);
			_result[i] = value;
		}
	}

	// Token: 0x06000819 RID: 2073 RVA: 0x0002D787 File Offset: 0x0002B987
	public override IList<Bounds> GetClipBoundsList(BlockValue _blockValue, Vector3 _blockPos)
	{
		Block.staticList_IntersectRayWithBlockList.Clear();
		this.GetCollisionAABB(_blockValue, (int)_blockPos.x, (int)_blockPos.y, (int)_blockPos.z, 0f, Block.staticList_IntersectRayWithBlockList);
		return Block.staticList_IntersectRayWithBlockList;
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x00038D84 File Offset: 0x00036F84
	[PublicizedFrom(EAccessModifier.Private)]
	public bool updateTrapState(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bSwitchTrap = false)
	{
		ChunkCluster chunkCluster = _world.ChunkClusters[_cIdx];
		if (chunkCluster == null)
		{
			return false;
		}
		IChunk chunkSync = chunkCluster.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z));
		if (chunkSync == null)
		{
			return false;
		}
		BlockEntityData blockEntity = chunkSync.GetBlockEntity(_blockPos);
		if (blockEntity == null || !blockEntity.bHasTransform)
		{
			return false;
		}
		bool flag = (_blockValue.meta & 2) > 0;
		if (_bSwitchTrap)
		{
			flag = !flag;
			_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | (flag ? 2 : 0));
			_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);
		}
		this.ActivateTrap(blockEntity, flag);
		TileEntityPoweredMeleeTrap tileEntityPoweredMeleeTrap = (TileEntityPoweredMeleeTrap)_world.GetTileEntity(_cIdx, _blockPos);
		if (tileEntityPoweredMeleeTrap != null)
		{
			SpinningBladeTrapController component = blockEntity.transform.gameObject.GetComponent<SpinningBladeTrapController>();
			if (component != null)
			{
				component.BladeController.OwnerTE = tileEntityPoweredMeleeTrap;
			}
		}
		return true;
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x00038E68 File Offset: 0x00037068
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
		if (_newBlockValue.ischild)
		{
			return;
		}
		this.updateTrapState(_world, _clrIdx, _blockPos, _newBlockValue, false);
		((World)_world).ChunkClusters[_clrIdx].GetBlockEntity(_blockPos);
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x000359BF File Offset: 0x00033BBF
	public static bool IsOn(byte _metadata)
	{
		return (_metadata & 2) > 0;
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x00038EB8 File Offset: 0x000370B8
	public override void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		base.PlaceBlock(_world, _result, _ea);
		TileEntityPoweredMeleeTrap tileEntityPoweredMeleeTrap = _world.GetTileEntity(_result.clrIdx, _result.blockPos) as TileEntityPoweredMeleeTrap;
		if (tileEntityPoweredMeleeTrap != null && _ea != null && _ea.entityType == EntityType.Player)
		{
			tileEntityPoweredMeleeTrap.SetOwner(PlatformManager.InternalLocalUserIdentifier);
		}
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x00038F08 File Offset: 0x00037108
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
		SpinningBladeTrapController component = _ebcd.transform.gameObject.GetComponent<SpinningBladeTrapController>();
		if (component != null)
		{
			component.Cleanup();
		}
		this.ActivateTrap(_ebcd, false);
		TileEntityPoweredMeleeTrap tileEntityPoweredMeleeTrap = (TileEntityPoweredMeleeTrap)_world.GetTileEntity(_cIdx, _blockPos);
		if (tileEntityPoweredMeleeTrap == null)
		{
			ChunkCluster chunkCluster = _world.ChunkClusters[_cIdx];
			if (chunkCluster == null)
			{
				return;
			}
			Chunk chunk = (Chunk)chunkCluster.GetChunkFromWorldPos(_blockPos);
			if (chunk == null)
			{
				return;
			}
			tileEntityPoweredMeleeTrap = (TileEntityPoweredMeleeTrap)this.CreateTileEntity(chunk);
			tileEntityPoweredMeleeTrap.localChunkPos = World.toBlock(_blockPos);
			tileEntityPoweredMeleeTrap.InitializePowerData();
			chunk.AddTileEntity(tileEntityPoweredMeleeTrap);
		}
		if (tileEntityPoweredMeleeTrap != null)
		{
			bool flag = (_blockValue.meta & 2) > 0;
			_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | (flag ? 2 : 0));
			this.updateTrapState(_world, _cIdx, _blockPos, _blockValue, false);
			if (component != null)
			{
				component.BladeController.OwnerTE = tileEntityPoweredMeleeTrap;
			}
		}
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x00038FF4 File Offset: 0x000371F4
	public override void OnBlockAdded(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (_blockValue.ischild)
		{
			return;
		}
		if (!(world.GetTileEntity(_chunk.ClrIdx, _blockPos) is TileEntityPoweredMeleeTrap))
		{
			TileEntityPowered tileEntityPowered = this.CreateTileEntity(_chunk);
			tileEntityPowered.localChunkPos = World.toBlock(_blockPos);
			tileEntityPowered.InitializePowerData();
			_chunk.AddTileEntity(tileEntityPowered);
		}
		_chunk.AddEntityBlockStub(new BlockEntityData(_blockValue, _blockPos)
		{
			bNeedsTemperature = true
		});
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x00039064 File Offset: 0x00037264
	public override bool ActivateBlock(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, bool isOn, bool isPowered)
	{
		if (_blockValue.ischild)
		{
			return false;
		}
		_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | (isOn ? 2 : 0));
		_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);
		this.updateTrapState(_world, _cIdx, _blockPos, _blockValue, false);
		return true;
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool ActivateTrap(BlockEntityData blockEntity, bool isOn)
	{
		return false;
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x000390A4 File Offset: 0x000372A4
	public override TileEntityPowered CreateTileEntity(Chunk chunk)
	{
		return new TileEntityPoweredMeleeTrap(chunk);
	}

	// Token: 0x0400085D RID: 2141
	[PublicizedFrom(EAccessModifier.Protected)]
	public new static string PropDamage = "Damage";

	// Token: 0x0400085E RID: 2142
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropDamageReceived = "Damage_received";

	// Token: 0x0400085F RID: 2143
	[PublicizedFrom(EAccessModifier.Protected)]
	public int damage;

	// Token: 0x04000860 RID: 2144
	[PublicizedFrom(EAccessModifier.Protected)]
	public int damageReceived;
}
