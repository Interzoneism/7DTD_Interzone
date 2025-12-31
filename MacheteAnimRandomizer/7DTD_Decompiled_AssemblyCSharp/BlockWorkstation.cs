using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000154 RID: 340
[Preserve]
public class BlockWorkstation : BlockParticle
{
	// Token: 0x060009B7 RID: 2487 RVA: 0x00041830 File Offset: 0x0003FA30
	public BlockWorkstation()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x00041888 File Offset: 0x0003FA88
	public override void Init()
	{
		base.Init();
		this.TakeDelay = 2f;
		base.Properties.ParseFloat("TakeDelay", ref this.TakeDelay);
		string text = "1,2,3";
		base.Properties.ParseString("Workstation.ToolNames", ref text);
		this.toolTransformNames = text.Split(',', StringSplitOptions.None);
		this.WorkstationData = new WorkstationData(base.GetBlockName(), base.Properties);
		CraftingManager.AddWorkstationData(this.WorkstationData);
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x00041908 File Offset: 0x0003FB08
	public override void OnBlockAdded(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (_blockValue.ischild)
		{
			return;
		}
		_chunk.AddTileEntity(new TileEntityWorkstation(_chunk)
		{
			localChunkPos = World.toBlock(_blockPos)
		});
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x00041946 File Offset: 0x0003FB46
	public override void OnBlockRemoved(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
		_chunk.RemoveTileEntityAt<TileEntityWorkstation>((World)world, World.toBlock(_blockPos));
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x00041968 File Offset: 0x0003FB68
	public override void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		base.PlaceBlock(_world, _result, _ea);
		TileEntityWorkstation tileEntityWorkstation = (TileEntityWorkstation)_world.GetTileEntity(_result.blockPos);
		if (tileEntityWorkstation != null)
		{
			tileEntityWorkstation.IsPlayerPlaced = true;
		}
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x0004199C File Offset: 0x0003FB9C
	public override bool OnBlockActivated(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		TileEntityWorkstation tileEntityWorkstation = (TileEntityWorkstation)_world.GetTileEntity(_blockPos);
		if (tileEntityWorkstation == null)
		{
			return false;
		}
		_player.AimingGun = false;
		Vector3i blockPos = tileEntityWorkstation.ToWorldPos();
		_world.GetGameManager().TELockServer(_cIdx, blockPos, tileEntityWorkstation.entityId, _player.entityId, null);
		return true;
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x000419E6 File Offset: 0x0003FBE6
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
		this.checkParticles(_world, _clrIdx, _blockPos, _newBlockValue);
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override byte GetLightValue(BlockValue _blockValue)
	{
		return 0;
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x00041A04 File Offset: 0x0003FC04
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void checkParticles(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (_blockValue.ischild)
		{
			return;
		}
		bool flag = GameManager.Instance.HasBlockParticleEffect(_blockPos);
		if (_blockValue.meta != 0 && !flag)
		{
			this.addParticles(_world, _clrIdx, _blockPos.x, _blockPos.y, _blockPos.z, _blockValue);
			if (this.CraftingParticleLightIntensity > 0f)
			{
				this.UpdateVisible(_world, _blockPos);
				return;
			}
		}
		else if (_blockValue.meta == 0 && flag)
		{
			this.removeParticles(_world, _blockPos.x, _blockPos.y, _blockPos.z, _blockValue);
		}
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x00041A8E File Offset: 0x0003FC8E
	public static bool IsLit(BlockValue _blockValue)
	{
		return _blockValue.meta > 0;
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x00037485 File Offset: 0x00035685
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return Localization.Get("useWorkstation", false);
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x00041A9A File Offset: 0x0003FC9A
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_commandName == "open")
		{
			return this.OnBlockActivated(_world, _cIdx, _blockPos, _blockValue, _player);
		}
		if (!(_commandName == "take"))
		{
			return false;
		}
		this.TakeItemWithTimer(_cIdx, _blockPos, _blockValue, _player);
		return true;
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x00041AD8 File Offset: 0x0003FCD8
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool flag = _world.IsMyLandProtectedBlock(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false);
		TileEntityWorkstation tileEntityWorkstation = (TileEntityWorkstation)_world.GetTileEntity(_blockPos);
		bool flag2 = false;
		if (tileEntityWorkstation != null)
		{
			flag2 = tileEntityWorkstation.IsPlayerPlaced;
		}
		this.cmds[1].enabled = (flag && flag2 && this.TakeDelay > 0f);
		return this.cmds;
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x00041B40 File Offset: 0x0003FD40
	[PublicizedFrom(EAccessModifier.Private)]
	public void TakeItemWithTimer(int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _player)
	{
		if (_blockValue.damage > 0)
		{
			GameManager.ShowTooltip(_player as EntityPlayerLocal, Localization.Get("ttRepairBeforePickup", false), string.Empty, "ui_denied", null, false, false, 0f);
			return;
		}
		if (!(GameManager.Instance.World.GetTileEntity(_blockPos) as TileEntityWorkstation).IsEmpty)
		{
			GameManager.ShowTooltip(_player as EntityPlayerLocal, Localization.Get("ttWorkstationNotEmpty", false), string.Empty, "ui_denied", null, false, false, 0f);
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
		childByType.SetTimer(this.TakeDelay, timerEventData, -1f, "");
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x00041C4C File Offset: 0x0003FE4C
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
		if ((world.GetTileEntity(vector3i) as TileEntityWorkstation).IsUserAccessing())
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

	// Token: 0x060009C7 RID: 2503 RVA: 0x00041D85 File Offset: 0x0003FF85
	public override void OnBlockEntityTransformBeforeActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformBeforeActivated(_world, _blockPos, _blockValue, _ebcd);
		this.UpdateVisible(_world, _blockPos);
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x00041D9C File Offset: 0x0003FF9C
	public void UpdateVisible(WorldBase _world, Vector3i _blockPos)
	{
		TileEntityWorkstation tileEntityWorkstation = _world.GetTileEntity(_blockPos) as TileEntityWorkstation;
		if (tileEntityWorkstation != null)
		{
			this.UpdateVisible(tileEntityWorkstation);
		}
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x00041DC0 File Offset: 0x0003FFC0
	public void UpdateVisible(TileEntityWorkstation _te)
	{
		BlockEntityData blockEntity = _te.GetChunk().GetBlockEntity(_te.ToWorldPos());
		if (blockEntity == null)
		{
			return;
		}
		Transform transform = blockEntity.transform;
		if (transform)
		{
			ItemStack[] tools = _te.Tools;
			int num = Utils.FastMin(tools.Length, this.toolTransformNames.Length);
			for (int i = 0; i < num; i++)
			{
				Transform transform2 = transform.Find(this.toolTransformNames[i]);
				if (transform2)
				{
					transform2.gameObject.SetActive(!tools[i].IsEmpty());
				}
			}
			Transform transform3 = transform.Find("craft");
			if (transform3)
			{
				bool isCrafting = _te.IsCrafting;
				transform3.gameObject.SetActive(isCrafting);
				if (this.CraftingParticleLightIntensity > 0f)
				{
					Transform blockParticleEffect = GameManager.Instance.GetBlockParticleEffect(_te.ToWorldPos());
					if (blockParticleEffect)
					{
						Light componentInChildren = blockParticleEffect.GetComponentInChildren<Light>();
						if (componentInChildren)
						{
							componentInChildren.intensity = (isCrafting ? this.CraftingParticleLightIntensity : 1f);
							return;
						}
					}
					else if (isCrafting)
					{
						_te.SetVisibleChanged();
					}
				}
			}
		}
	}

	// Token: 0x04000910 RID: 2320
	[PublicizedFrom(EAccessModifier.Protected)]
	public float TakeDelay;

	// Token: 0x04000911 RID: 2321
	public WorkstationData WorkstationData;

	// Token: 0x04000912 RID: 2322
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] toolTransformNames;

	// Token: 0x04000913 RID: 2323
	[PublicizedFrom(EAccessModifier.Protected)]
	public float CraftingParticleLightIntensity;

	// Token: 0x04000914 RID: 2324
	[PublicizedFrom(EAccessModifier.Protected)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("open", "campfire", true, false, null),
		new BlockActivationCommand("take", "hand", false, false, null)
	};
}
