using System;
using Audio;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000108 RID: 264
[Preserve]
public class BlockLauncher : BlockPowered
{
	// Token: 0x06000702 RID: 1794 RVA: 0x000311F8 File Offset: 0x0002F3F8
	public BlockLauncher()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x00031250 File Offset: 0x0002F450
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("PlaySound"))
		{
			this.playSound = base.Properties.Values["PlaySound"];
		}
		if (base.Properties.Values.ContainsKey("StartSound"))
		{
			this.startSound = base.Properties.Values["StartSound"];
		}
		if (base.Properties.Values.ContainsKey("EndSound"))
		{
			this.endSound = base.Properties.Values["EndSound"];
		}
		if (base.Properties.Values.ContainsKey("AmmoItem"))
		{
			this.AmmoItemName = base.Properties.Values["AmmoItem"];
		}
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x0003132C File Offset: 0x0002F52C
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool enabled = _world.CanPlaceBlockAt(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false);
		bool flag = _world.IsMyLandProtectedBlock(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false);
		this.cmds[0].enabled = enabled;
		this.cmds[1].enabled = (flag && this.TakeDelay > 0f);
		return this.cmds;
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x000313A0 File Offset: 0x0002F5A0
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (!_world.CanPlaceBlockAt(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false))
		{
			return "";
		}
		bool flag = _world.GetTileEntity(_clrIdx, _blockPos) is TileEntityPoweredRangedTrap;
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		if (flag)
		{
			string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
			string localizedBlockName = _blockValue.Block.GetLocalizedBlockName();
			return string.Format(Localization.Get("vendingMachineActivate", false), arg, localizedBlockName);
		}
		return "";
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x00031434 File Offset: 0x0002F634
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.OnBlockActivated(_commandName, _world, _cIdx, parentPos, block, _player);
		}
		TileEntityPoweredRangedTrap tileEntityPoweredRangedTrap = _world.GetTileEntity(_cIdx, _blockPos) as TileEntityPoweredRangedTrap;
		if (tileEntityPoweredRangedTrap == null)
		{
			return false;
		}
		if (_commandName == "options")
		{
			_player.AimingGun = false;
			Vector3i blockPos = tileEntityPoweredRangedTrap.ToWorldPos();
			_world.GetGameManager().TELockServer(_cIdx, blockPos, tileEntityPoweredRangedTrap.entityId, _player.entityId, null);
			return true;
		}
		if (!(_commandName == "take"))
		{
			return false;
		}
		base.TakeItemWithTimer(_cIdx, _blockPos, _blockValue, _player);
		return true;
	}

	// Token: 0x06000708 RID: 1800 RVA: 0x000314E4 File Offset: 0x0002F6E4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool updateState(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue)
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
		TileEntityPoweredBlock tileEntityPoweredBlock = ((TileEntityPowered)_world.GetTileEntity(_cIdx, _blockPos)) as TileEntityPoweredBlock;
		if (flag)
		{
			if (tileEntityPoweredBlock != null && !PowerManager.Instance.ClientUpdateList.Contains(tileEntityPoweredBlock))
			{
				PowerManager.Instance.ClientUpdateList.Add(tileEntityPoweredBlock);
			}
		}
		else if (tileEntityPoweredBlock != null && PowerManager.Instance.ClientUpdateList.Contains(tileEntityPoweredBlock))
		{
			PowerManager.Instance.ClientUpdateList.Remove(tileEntityPoweredBlock);
		}
		Transform transform = blockEntity.transform.Find("Activated");
		if (transform != null)
		{
			transform.gameObject.SetActive(flag);
		}
		return true;
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x000315E8 File Offset: 0x0002F7E8
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
		TileEntityPowered tileEntityPowered = (TileEntityPowered)_world.GetTileEntity(_cIdx, _blockPos);
		if (tileEntityPowered == null)
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
			tileEntityPowered = this.CreateTileEntity(chunk);
			tileEntityPowered.localChunkPos = World.toBlock(_blockPos);
			tileEntityPowered.InitializePowerData();
			chunk.AddTileEntity(tileEntityPowered);
		}
		tileEntityPowered.BlockTransform = _ebcd.transform;
		tileEntityPowered.MarkWireDirty();
		if (tileEntityPowered.GetParent().y != -9999)
		{
			IPowered powered = _world.GetTileEntity(_cIdx, tileEntityPowered.GetParent()) as IPowered;
			if (powered != null)
			{
				powered.DrawWires();
			}
		}
		this.updateState(_world, _cIdx, _blockPos, _blockValue);
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x000316A3 File Offset: 0x0002F8A3
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
		this.updateState(_world, _clrIdx, _blockPos, _newBlockValue);
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x000316C4 File Offset: 0x0002F8C4
	public override bool ActivateBlock(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, bool isOn, bool isPowered)
	{
		byte b = (byte)(((int)_blockValue.meta & -3) | (isOn ? 2 : 0));
		if (_blockValue.meta != b)
		{
			_blockValue.meta = b;
			_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);
		}
		return true;
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x00031704 File Offset: 0x0002F904
	public override void OnBlockAdded(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (_blockValue.ischild)
		{
			return;
		}
		if (!(world.GetTileEntity(_chunk.ClrIdx, _blockPos) is TileEntityPoweredRangedTrap))
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

	// Token: 0x0600070D RID: 1805 RVA: 0x00031774 File Offset: 0x0002F974
	public override void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		base.PlaceBlock(_world, _result, _ea);
		TileEntityPoweredRangedTrap tileEntityPoweredRangedTrap = _world.GetTileEntity(_result.clrIdx, _result.blockPos) as TileEntityPoweredRangedTrap;
		if (tileEntityPoweredRangedTrap != null && _ea != null && _ea.entityType == EntityType.Player)
		{
			tileEntityPoweredRangedTrap.SetOwner(PlatformManager.InternalLocalUserIdentifier);
		}
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x000317C4 File Offset: 0x0002F9C4
	public override void OnBlockLoaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockLoaded(_world, _clrIdx, _blockPos, _blockValue);
		bool flag = (_blockValue.meta & 2) > 0;
		TileEntityPoweredBlock tileEntityPoweredBlock = ((TileEntityPowered)_world.GetTileEntity(_clrIdx, _blockPos)) as TileEntityPoweredBlock;
		if (flag)
		{
			if (tileEntityPoweredBlock != null && !PowerManager.Instance.ClientUpdateList.Contains(tileEntityPoweredBlock))
			{
				PowerManager.Instance.ClientUpdateList.Add(tileEntityPoweredBlock);
				return;
			}
		}
		else if (tileEntityPoweredBlock != null && PowerManager.Instance.ClientUpdateList.Contains(tileEntityPoweredBlock))
		{
			PowerManager.Instance.ClientUpdateList.Remove(tileEntityPoweredBlock);
		}
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x00031849 File Offset: 0x0002FA49
	public override TileEntityPowered CreateTileEntity(Chunk chunk)
	{
		return new TileEntityPoweredRangedTrap(chunk)
		{
			PowerItemType = PowerItem.PowerItemTypes.RangedTrap
		};
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x00031858 File Offset: 0x0002FA58
	public bool InstantiateProjectile(WorldBase _world, int _cIdx, Vector3i _blockPos)
	{
		TileEntityPoweredRangedTrap tileEntityPoweredRangedTrap = _world.GetTileEntity(_cIdx, _blockPos) as TileEntityPoweredRangedTrap;
		if (tileEntityPoweredRangedTrap == null)
		{
			return false;
		}
		if (!tileEntityPoweredRangedTrap.IsLocked)
		{
			return false;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && !tileEntityPoweredRangedTrap.DecrementAmmo())
		{
			tileEntityPoweredRangedTrap.IsLocked = false;
			tileEntityPoweredRangedTrap.SetModified();
			return false;
		}
		ItemClass ammoItem = tileEntityPoweredRangedTrap.AmmoItem;
		ItemClass.GetItem(ammoItem.GetItemName(), false);
		if (ammoItem == null)
		{
			return false;
		}
		ItemValue itemValue = new ItemValue(ammoItem.Id, 2, 2, false, null, 1f);
		Transform transform = ammoItem.CloneModel((World)_world, itemValue, Vector3.zero, null, BlockShape.MeshPurpose.World, default(TextureFullArray));
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
		Transform transform2 = blockEntity.transform;
		if (transform2 != null)
		{
			transform.parent = transform2;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}
		else
		{
			transform.parent = null;
		}
		Utils.SetLayerRecursively(transform.gameObject, (transform2 != null) ? transform2.gameObject.layer : 0);
		BlockProjectileMoveScript blockProjectileMoveScript = transform.gameObject.AddComponent<BlockProjectileMoveScript>();
		blockProjectileMoveScript.itemProjectile = ammoItem;
		blockProjectileMoveScript.itemValueProjectile = itemValue;
		blockProjectileMoveScript.itemValueLauncher = ItemValue.None.Clone();
		blockProjectileMoveScript.itemActionProjectile = (ItemActionProjectile)((ammoItem.Actions[0] is ItemActionProjectile) ? ammoItem.Actions[0] : ammoItem.Actions[1]);
		blockProjectileMoveScript.ProjectileOwnerID = tileEntityPoweredRangedTrap.OwnerEntityID;
		blockProjectileMoveScript.Fire(_blockPos.ToVector3() + new Vector3(0.5f, 0.5f, 0.5f), transform2.forward, null, 0, 0f, false);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			Manager.BroadcastPlay(_blockPos.ToVector3(), this.playSound, 0f);
		}
		return true;
	}

	// Token: 0x040007E0 RID: 2016
	[PublicizedFrom(EAccessModifier.Private)]
	public string playSound;

	// Token: 0x040007E1 RID: 2017
	[PublicizedFrom(EAccessModifier.Private)]
	public string startSound;

	// Token: 0x040007E2 RID: 2018
	[PublicizedFrom(EAccessModifier.Private)]
	public string endSound;

	// Token: 0x040007E3 RID: 2019
	public string AmmoItemName;

	// Token: 0x040007E4 RID: 2020
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("options", "tool", true, false, null),
		new BlockActivationCommand("take", "hand", false, false, null)
	};
}
