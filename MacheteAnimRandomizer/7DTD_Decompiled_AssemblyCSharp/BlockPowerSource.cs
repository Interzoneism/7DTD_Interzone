using System;
using System.Collections;
using System.Globalization;
using Audio;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000126 RID: 294
[Preserve]
public class BlockPowerSource : Block
{
	// Token: 0x0600082A RID: 2090 RVA: 0x00039368 File Offset: 0x00037568
	public BlockPowerSource()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x0600082B RID: 2091 RVA: 0x000393E4 File Offset: 0x000375E4
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("SlotItem"))
		{
			this.SlotItemName = base.Properties.Values["SlotItem"];
		}
		else
		{
			this.SlotItemName = "carBattery";
		}
		if (base.Properties.Values.ContainsKey("OutputPerStack"))
		{
			this.OutputPerStack = Convert.ToInt32(base.Properties.Values["OutputPerStack"]);
		}
		else
		{
			this.OutputPerStack = 25;
		}
		if (base.Properties.Values.ContainsKey("TakeDelay"))
		{
			this.TakeDelay = StringParsers.ParseFloat(base.Properties.Values["TakeDelay"], 0, -1, NumberStyles.Any);
			return;
		}
		this.TakeDelay = 2f;
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x000394C4 File Offset: 0x000376C4
	public override void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		base.PlaceBlock(_world, _result, _ea);
		TileEntityPowerSource tileEntityPowerSource = _world.GetTileEntity(_result.clrIdx, _result.blockPos) as TileEntityPowerSource;
		if (tileEntityPowerSource != null && _ea != null && _ea.entityType == EntityType.Player)
		{
			tileEntityPowerSource.SetOwner(PlatformManager.InternalLocalUserIdentifier);
			tileEntityPowerSource.IsPlayerPlaced = true;
		}
	}

	// Token: 0x0600082D RID: 2093 RVA: 0x0003951C File Offset: 0x0003771C
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (!_world.CanPlaceBlockAt(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false))
		{
			return "";
		}
		bool flag = _world.GetTileEntity(_clrIdx, _blockPos) is TileEntityPowerSource;
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		if (flag)
		{
			string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
			string localizedBlockName = _blockValue.Block.GetLocalizedBlockName();
			return string.Format(Localization.Get("vendingMachineActivate", false), arg, localizedBlockName);
		}
		return "";
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x000395AE File Offset: 0x000377AE
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return _world.GetTileEntity(_clrIdx, _blockPos) is TileEntityPowerSource;
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x000395C4 File Offset: 0x000377C4
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		TileEntityPowerSource tileEntityPowerSource = _world.GetTileEntity(_clrIdx, _blockPos) as TileEntityPowerSource;
		if (tileEntityPowerSource == null)
		{
			return BlockActivationCommand.Empty;
		}
		bool enabled = _world.CanPlaceBlockAt(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false);
		bool flag = _world.IsMyLandProtectedBlock(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false);
		this.cmds[0].icon = this.GetPowerSourceIcon();
		this.cmds[0].enabled = enabled;
		this.cmds[1].enabled = enabled;
		bool flag2 = false;
		if (tileEntityPowerSource != null)
		{
			flag2 = tileEntityPowerSource.IsPlayerPlaced;
		}
		this.cmds[2].enabled = (flag && flag2 && this.TakeDelay > 0f);
		return this.cmds;
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x00039688 File Offset: 0x00037888
	public override void OnBlockAdded(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (_blockValue.ischild)
		{
			return;
		}
		if (!(world.GetTileEntity(_chunk.ClrIdx, _blockPos) is TileEntityPowerSource))
		{
			TileEntityPowerSource tileEntityPowerSource = this.CreateTileEntity(_chunk);
			tileEntityPowerSource.localChunkPos = World.toBlock(_blockPos);
			tileEntityPowerSource.InitializePowerData();
			_chunk.AddTileEntity(tileEntityPowerSource);
		}
		_chunk.AddEntityBlockStub(new BlockEntityData(_blockValue, _blockPos)
		{
			bNeedsTemperature = true
		});
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x000396F8 File Offset: 0x000378F8
	public override void OnBlockRemoved(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
		TileEntityPowered tileEntityPowered = _chunk.GetTileEntity(World.toBlock(_blockPos)) as TileEntityPowered;
		if (tileEntityPowered != null)
		{
			if (!GameManager.IsDedicatedServer)
			{
				EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
				if (primaryPlayer.inventory.holdingItem.Actions[1] is ItemActionConnectPower)
				{
					(primaryPlayer.inventory.holdingItem.Actions[1] as ItemActionConnectPower).CheckForWireRemoveNeeded(primaryPlayer, _blockPos);
				}
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				PowerManager.Instance.RemovePowerNode(tileEntityPowered.GetPowerItem());
			}
			if (tileEntityPowered.GetParent().y != -9999)
			{
				IPowered powered = world.GetTileEntity(0, tileEntityPowered.GetParent()) as IPowered;
				if (powered != null && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					powered.SendWireData();
				}
			}
			tileEntityPowered.RemoveWires();
		}
		_chunk.RemoveTileEntityAt<TileEntityPowerSource>((World)world, World.toBlock(_blockPos));
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x000397E4 File Offset: 0x000379E4
	public override Block.DestroyedResult OnBlockDestroyedBy(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _entityId, bool _bUseHarvestTool)
	{
		TileEntityPowerSource tileEntityPowerSource = _world.GetTileEntity(_clrIdx, _blockPos) as TileEntityPowerSource;
		if (tileEntityPowerSource != null)
		{
			tileEntityPowerSource.OnDestroy();
		}
		return Block.DestroyedResult.Downgrade;
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x0003980C File Offset: 0x00037A0C
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.OnBlockActivated(_commandName, _world, _cIdx, parentPos, block, _player);
		}
		TileEntityPowerSource tileEntityPowerSource = _world.GetTileEntity(_cIdx, _blockPos) as TileEntityPowerSource;
		if (tileEntityPowerSource == null)
		{
			return false;
		}
		if (_commandName == "open")
		{
			_player.AimingGun = false;
			Vector3i blockPos = tileEntityPowerSource.ToWorldPos();
			_world.GetGameManager().TELockServer(_cIdx, blockPos, tileEntityPowerSource.entityId, _player.entityId, null);
			return true;
		}
		if (!(_commandName == "light"))
		{
			if (!(_commandName == "take"))
			{
				return false;
			}
			this.TakeItemWithTimer(_cIdx, _blockPos, _blockValue, _player);
			return true;
		}
		else
		{
			if (tileEntityPowerSource.MaxOutput == 0)
			{
				Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
				GameManager.ShowTooltip(_player, Localization.Get("ttRequiresOneComponent", false), false, false, 0f);
				return false;
			}
			if (tileEntityPowerSource.PowerItemType == PowerItem.PowerItemTypes.Generator && tileEntityPowerSource.CurrentFuel == 0)
			{
				Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
				GameManager.ShowTooltip(_player, Localization.Get("ttGeneratorRequiresFuel", false), false, false, 0f);
				return false;
			}
			bool flag = (_blockValue.meta & 2) > 0;
			if (!flag && (false | _world.IsWater(_blockPos.x, _blockPos.y + 1, _blockPos.z) | _world.IsWater(_blockPos.x + 1, _blockPos.y, _blockPos.z) | _world.IsWater(_blockPos.x - 1, _blockPos.y, _blockPos.z) | _world.IsWater(_blockPos.x, _blockPos.y, _blockPos.z + 1) | _world.IsWater(_blockPos.x, _blockPos.y, _blockPos.z - 1)))
			{
				Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
				GameManager.ShowTooltip(_player, Localization.Get("ttPowerSourceUnderwater", false), false, false, 0f);
				return false;
			}
			_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | ((!flag) ? 2 : 0));
			_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);
			return true;
		}
	}

	// Token: 0x06000834 RID: 2100 RVA: 0x00039A44 File Offset: 0x00037C44
	public void TakeItemWithTimer(int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _player)
	{
		if (_blockValue.damage > 0)
		{
			GameManager.ShowTooltip(_player as EntityPlayerLocal, Localization.Get("ttRepairBeforePickup", false), string.Empty, "ui_denied", null, false, false, 0f);
			return;
		}
		LocalPlayerUI playerUI = (_player as EntityPlayerLocal).PlayerUI;
		playerUI.windowManager.Open("timer", true, false, true);
		XUiC_Timer childByType = playerUI.xui.GetChildByType<XUiC_Timer>();
		TimerEventData timerEventData = new TimerEventData();
		timerEventData.Data = new object[]
		{
			_cIdx,
			_blockValue,
			_blockPos,
			_player
		};
		timerEventData.Event += this.EventData_Event;
		childByType.SetTimer(4f, timerEventData, -1f, "");
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x00039B08 File Offset: 0x00037D08
	[PublicizedFrom(EAccessModifier.Private)]
	public void EventData_Event(TimerEventData timerData)
	{
		World world = GameManager.Instance.World;
		object[] array = (object[])timerData.Data;
		int clrIdx = (int)array[0];
		BlockValue blockValue = (BlockValue)array[1];
		Vector3i vector3i = (Vector3i)array[2];
		BlockValue block = world.GetBlock(vector3i);
		EntityPlayerLocal entityPlayerLocal = array[3] as EntityPlayerLocal;
		if (block.damage > 0)
		{
			GameManager.ShowTooltip(entityPlayerLocal, Localization.Get("ttRepairBeforePickup", false), string.Empty, "ui_denied", null, false, false, 0f);
			return;
		}
		if (block.type != blockValue.type)
		{
			GameManager.ShowTooltip(entityPlayerLocal, Localization.Get("ttBlockMissingPickup", false), string.Empty, "ui_denied", null, false, false, 0f);
			return;
		}
		if ((world.GetTileEntity(clrIdx, vector3i) as TileEntityPowerSource).IsUserAccessing())
		{
			GameManager.ShowTooltip(entityPlayerLocal, Localization.Get("ttCantPickupInUse", false), string.Empty, "ui_denied", null, false, false, 0f);
			return;
		}
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
		ItemStack itemStack = new ItemStack(block.ToItemValue(), 1);
		if (!uiforPlayer.xui.PlayerInventory.AddItem(itemStack))
		{
			uiforPlayer.xui.PlayerInventory.DropItem(itemStack);
		}
		world.SetBlockRPC(clrIdx, vector3i, BlockValue.Air);
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsWaterBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, BlockFaceFlag _sides)
	{
		return true;
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x00039C44 File Offset: 0x00037E44
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
		TileEntityPowerSource tileEntityPowerSource = (TileEntityPowerSource)_world.GetTileEntity(_cIdx, _blockPos);
		if (tileEntityPowerSource == null)
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
			tileEntityPowerSource = this.CreateTileEntity(chunk);
			tileEntityPowerSource.localChunkPos = World.toBlock(_blockPos);
			tileEntityPowerSource.InitializePowerData();
			chunk.AddTileEntity(tileEntityPowerSource);
		}
		tileEntityPowerSource.BlockTransform = _ebcd.transform;
		GameManager.Instance.StartCoroutine(this.drawWiresLater(tileEntityPowerSource));
		if (tileEntityPowerSource.GetParent().y != -9999)
		{
			IPowered powered = _world.GetTileEntity(_cIdx, tileEntityPowerSource.GetParent()) as IPowered;
			if (powered != null)
			{
				GameManager.Instance.StartCoroutine(this.drawWiresLater(powered));
			}
		}
		this.updateState(_world, _cIdx, _blockPos, _blockValue, false);
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x00039D18 File Offset: 0x00037F18
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator drawWiresLater(IPowered powered)
	{
		yield return new WaitForSeconds(0.5f);
		powered.DrawWires();
		yield break;
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x00039D28 File Offset: 0x00037F28
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			TileEntityPowerSource tileEntityPowerSource = _world.GetTileEntity(_clrIdx, _blockPos) as TileEntityPowerSource;
			if (tileEntityPowerSource == null)
			{
				ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
				if (chunkCluster == null)
				{
					return;
				}
				Chunk chunk = (Chunk)chunkCluster.GetChunkFromWorldPos(_blockPos);
				if (chunk == null)
				{
					return;
				}
				tileEntityPowerSource = this.CreateTileEntity(chunk);
				tileEntityPowerSource.localChunkPos = World.toBlock(_blockPos);
				chunk.AddTileEntity(tileEntityPowerSource);
				string str = "TileEntityPowerSource not found (";
				Vector3i vector3i = _blockPos;
				Log.Out(str + vector3i.ToString() + ")");
			}
			PowerSource powerSource = tileEntityPowerSource.GetPowerItem() as PowerSource;
			if (powerSource == null)
			{
				powerSource = (PowerManager.Instance.GetPowerItemByWorldPos(tileEntityPowerSource.ToWorldPos()) as PowerSource);
				if (powerSource == null)
				{
					powerSource = (tileEntityPowerSource.CreatePowerItemForTileEntity((ushort)_newBlockValue.type) as PowerSource);
					tileEntityPowerSource.SetModified();
					powerSource.AddTileEntity(tileEntityPowerSource);
					string str2 = "PowerSource not found (";
					Vector3i vector3i = _blockPos;
					Log.Out(str2 + vector3i.ToString() + ")");
				}
			}
			bool isOn = (_newBlockValue.meta & 2) > 0;
			powerSource.IsOn = isOn;
		}
		this.updateState(_world, _clrIdx, _blockPos, _newBlockValue, false);
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x00039E60 File Offset: 0x00038060
	[PublicizedFrom(EAccessModifier.Private)]
	public bool updateState(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bSwitchLight = false)
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
		if (_bSwitchLight)
		{
			flag = !flag;
			_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | (flag ? 2 : 0));
			_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);
		}
		Transform transform = blockEntity.transform.Find("Activated");
		if (transform != null)
		{
			transform.gameObject.SetActive(flag);
		}
		return true;
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x00039F28 File Offset: 0x00038128
	public override bool ActivateBlock(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, bool isOn, bool isPowered)
	{
		if ((_blockValue.meta & 2) > 0 != isOn)
		{
			_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | (isOn ? 2 : 0));
			_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);
		}
		this.updateState(_world, _cIdx, _blockPos, _blockValue, false);
		return true;
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x00019766 File Offset: 0x00017966
	public virtual TileEntityPowerSource CreateTileEntity(Chunk chunk)
	{
		return null;
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x0002B133 File Offset: 0x00029333
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual string GetPowerSourceIcon()
	{
		return "";
	}

	// Token: 0x04000864 RID: 2148
	public string SlotItemName;

	// Token: 0x04000865 RID: 2149
	public int OutputPerStack;

	// Token: 0x04000866 RID: 2150
	[PublicizedFrom(EAccessModifier.Private)]
	public float TakeDelay = 2f;

	// Token: 0x04000867 RID: 2151
	[PublicizedFrom(EAccessModifier.Protected)]
	public ItemClass slotItem;

	// Token: 0x04000868 RID: 2152
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("open", "hand", false, false, null),
		new BlockActivationCommand("light", "electric_switch", false, false, null),
		new BlockActivationCommand("take", "hand", false, false, null)
	};
}
