using System;
using UnityEngine.Scripting;

// Token: 0x020000FA RID: 250
[Preserve]
public class BlockDewCollector : Block
{
	// Token: 0x06000674 RID: 1652 RVA: 0x0002D9A0 File Offset: 0x0002BBA0
	public BlockDewCollector()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x0002DA2C File Offset: 0x0002BC2C
	public override void Init()
	{
		base.Init();
		base.Properties.ParseString(BlockDewCollector.PropConvertToItem, ref this.ConvertToItem);
		base.Properties.ParseString(BlockDewCollector.PropModdedConvertToItem, ref this.ModdedConvertToItem);
		base.Properties.ParseFloat(BlockDewCollector.PropMinTime, ref this.MinConvertTime);
		base.Properties.ParseFloat(BlockDewCollector.PropMaxTime, ref this.MaxConvertTime);
		base.Properties.ParseFloat(BlockDewCollector.PropModdedSpeed, ref this.ModdedConvertSpeed);
		base.Properties.ParseInt(BlockDewCollector.PropModdedCount, ref this.ModdedConvertCount);
		base.Properties.ParseString(BlockDewCollector.PropOpenSound, ref this.OpenSound);
		base.Properties.ParseString(BlockDewCollector.PropCloseSound, ref this.CloseSound);
		base.Properties.ParseString(BlockDewCollector.PropConvertSound, ref this.ConvertSound);
		base.Properties.ParseFloat(BlockDewCollector.PropTakeDelay, ref this.TakeDelay);
		string text = "1,2,3";
		base.Properties.ParseString("ModTransformNames", ref text);
		this.modTransformNames = text.Split(',', StringSplitOptions.None);
		text = "Count,Speed,Type";
		base.Properties.ParseString("ModTypes", ref text);
		string[] array = text.Split(',', StringSplitOptions.None);
		this.ModTypes = new BlockDewCollector.ModEffectTypes[array.Length];
		int num = 0;
		while (num < array.Length && num < this.ModTypes.Length)
		{
			this.ModTypes[num] = Enum.Parse<BlockDewCollector.ModEffectTypes>(array[num]);
			num++;
		}
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x0002DB9C File Offset: 0x0002BD9C
	public override void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		base.PlaceBlock(_world, _result, _ea);
		TileEntityDewCollector tileEntityDewCollector = _world.GetTileEntity(_result.clrIdx, _result.blockPos) as TileEntityDewCollector;
		if (tileEntityDewCollector == null)
		{
			return;
		}
		if (_ea != null && _ea.entityType == EntityType.Player)
		{
			tileEntityDewCollector.worldTimeTouched = _world.GetWorldTime();
			tileEntityDewCollector.SetEmpty();
		}
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x0002DBF4 File Offset: 0x0002BDF4
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		TileEntityDewCollector tileEntityDewCollector = _world.GetTileEntity(_clrIdx, _blockPos) as TileEntityDewCollector;
		if (tileEntityDewCollector == null)
		{
			return string.Empty;
		}
		string localizedBlockName = _blockValue.Block.GetLocalizedBlockName();
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		if (tileEntityDewCollector.IsWaterEmpty())
		{
			return string.Format(Localization.Get("dewCollectorEmpty", false), arg, localizedBlockName);
		}
		if (tileEntityDewCollector.IsModdedConvertItem)
		{
			return string.Format(Localization.Get("dewCollectorHasWater", false), arg, localizedBlockName);
		}
		return string.Format(Localization.Get("dewCollectorHasDirtyWater", false), arg, localizedBlockName);
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x0002DCA2 File Offset: 0x0002BEA2
	public override void OnBlockAdded(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (_blockValue.ischild)
		{
			return;
		}
		this.addTileEntity(world, _chunk, _blockPos, _blockValue);
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x0002DCC8 File Offset: 0x0002BEC8
	public override void OnBlockRemoved(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
		TileEntityDewCollector tileEntityDewCollector = world.GetTileEntity(_chunk.ClrIdx, _blockPos) as TileEntityDewCollector;
		if (tileEntityDewCollector != null)
		{
			tileEntityDewCollector.OnDestroy();
		}
		this.removeTileEntity(world, _chunk, _blockPos, _blockValue);
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x0002DD08 File Offset: 0x0002BF08
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void addTileEntity(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		TileEntityDewCollector tileEntityDewCollector = new TileEntityDewCollector(_chunk);
		tileEntityDewCollector.localChunkPos = World.toBlock(_blockPos);
		tileEntityDewCollector.SetWorldTime();
		_chunk.AddTileEntity(tileEntityDewCollector);
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x0002DD35 File Offset: 0x0002BF35
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void removeTileEntity(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		_chunk.RemoveTileEntityAt<TileEntityDewCollector>((World)world, World.toBlock(_blockPos));
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x0002DD4C File Offset: 0x0002BF4C
	public override Block.DestroyedResult OnBlockDestroyedBy(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _entityId, bool _bUseHarvestTool)
	{
		TileEntityDewCollector tileEntityDewCollector = _world.GetTileEntity(_clrIdx, _blockPos) as TileEntityDewCollector;
		if (tileEntityDewCollector != null)
		{
			tileEntityDewCollector.OnDestroy();
		}
		if (!GameManager.IsDedicatedServer)
		{
			XUiC_DewCollectorWindowGroup.CloseIfOpenAtPos(_blockPos, null);
		}
		return Block.DestroyedResult.Downgrade;
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x0002DD80 File Offset: 0x0002BF80
	public override bool OnBlockActivated(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_player.inventory.IsHoldingItemActionRunning())
		{
			return false;
		}
		TileEntityDewCollector tileEntityDewCollector = _world.GetTileEntity(_cIdx, _blockPos) as TileEntityDewCollector;
		if (tileEntityDewCollector == null)
		{
			return false;
		}
		_player.AimingGun = false;
		Vector3i blockPos = tileEntityDewCollector.ToWorldPos();
		_world.GetGameManager().TELockServer(_cIdx, blockPos, tileEntityDewCollector.entityId, _player.entityId, null);
		return true;
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x0002DDDB File Offset: 0x0002BFDB
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_commandName == "Search")
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

	// Token: 0x0600067F RID: 1663 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x0002DE18 File Offset: 0x0002C018
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool flag = _world.IsMyLandProtectedBlock(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false);
		this.cmds[1].enabled = (flag && this.TakeDelay > 0f);
		return this.cmds;
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x0002DE64 File Offset: 0x0002C064
	public void TakeItemWithTimer(int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _player)
	{
		if (_blockValue.damage > 0)
		{
			GameManager.ShowTooltip(_player as EntityPlayerLocal, Localization.Get("ttRepairBeforePickup", false), string.Empty, "ui_denied", null, false, false, 0f);
			return;
		}
		if (!(GameManager.Instance.World.GetTileEntity(_blockPos) as TileEntityDewCollector).IsEmpty())
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

	// Token: 0x06000682 RID: 1666 RVA: 0x0002DF70 File Offset: 0x0002C170
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
		if ((world.GetTileEntity(clrIdx, vector3i) as TileEntityDewCollector).IsUserAccessing())
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

	// Token: 0x06000683 RID: 1667 RVA: 0x0002E0AA File Offset: 0x0002C2AA
	public void UpdateVisible(TileEntityDewCollector _te)
	{
		if (_te.GetChunk().GetBlockEntity(_te.ToWorldPos()).transform)
		{
			ItemStack[] modSlots = _te.ModSlots;
		}
	}

	// Token: 0x0400078E RID: 1934
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropConvertToItem = "ConvertToItem";

	// Token: 0x0400078F RID: 1935
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropModdedConvertToItem = "ModdedConvertToItem";

	// Token: 0x04000790 RID: 1936
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropMinTime = "MinConvertTime";

	// Token: 0x04000791 RID: 1937
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropMaxTime = "MaxConvertTime";

	// Token: 0x04000792 RID: 1938
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropModdedSpeed = "ModdedConvertSpeed";

	// Token: 0x04000793 RID: 1939
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropModdedCount = "ModdedConvertCount";

	// Token: 0x04000794 RID: 1940
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropOpenSound = "OpenSound";

	// Token: 0x04000795 RID: 1941
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCloseSound = "CloseSound";

	// Token: 0x04000796 RID: 1942
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropConvertSound = "ConvertSound";

	// Token: 0x04000797 RID: 1943
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropTakeDelay = "TakeDelay";

	// Token: 0x04000798 RID: 1944
	public string ConvertToItem;

	// Token: 0x04000799 RID: 1945
	public string ModdedConvertToItem;

	// Token: 0x0400079A RID: 1946
	public float MinConvertTime = 21600f;

	// Token: 0x0400079B RID: 1947
	public float MaxConvertTime = 43200f;

	// Token: 0x0400079C RID: 1948
	public float ModdedConvertSpeed = 0.5f;

	// Token: 0x0400079D RID: 1949
	public int ModdedConvertCount = 2;

	// Token: 0x0400079E RID: 1950
	public string OpenSound;

	// Token: 0x0400079F RID: 1951
	public string CloseSound;

	// Token: 0x040007A0 RID: 1952
	public string ConvertSound;

	// Token: 0x040007A1 RID: 1953
	[PublicizedFrom(EAccessModifier.Private)]
	public float TakeDelay = 2f;

	// Token: 0x040007A2 RID: 1954
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] modNames;

	// Token: 0x040007A3 RID: 1955
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] modTransformNames;

	// Token: 0x040007A4 RID: 1956
	public BlockDewCollector.ModEffectTypes[] ModTypes;

	// Token: 0x040007A5 RID: 1957
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("Search", "search", true, false, null),
		new BlockActivationCommand("take", "hand", false, false, null)
	};

	// Token: 0x020000FB RID: 251
	public enum ModEffectTypes
	{
		// Token: 0x040007A7 RID: 1959
		Type,
		// Token: 0x040007A8 RID: 1960
		Speed,
		// Token: 0x040007A9 RID: 1961
		Count
	}
}
