using System;
using Audio;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000159 RID: 345
[Preserve]
public class TEFeatureLockPickable : TEFeatureAbs, ILockPickable
{
	// Token: 0x06000A2D RID: 2605 RVA: 0x00042AB8 File Offset: 0x00040CB8
	public override void Init(TileEntityComposite _parent, TileEntityFeatureData _featureData)
	{
		base.Init(_parent, _featureData);
		this.lockFeature = base.Parent.GetFeature<ILockable>();
		if (this.lockFeature == null)
		{
			Log.Error("Block with name " + base.Parent.TeData.Block.GetBlockName() + " does have a LockPickable but no Lockable feature");
		}
		DynamicProperties props = _featureData.Props;
		if (!props.Values.ContainsKey(BlockSecureLoot.PropLockPickItem))
		{
			Log.Error(string.Concat(new string[]
			{
				"Block with name ",
				base.Parent.TeData.Block.GetBlockName(),
				" does not have a ",
				BlockSecureLoot.PropLockPickItem,
				" property for LockPickable feature"
			}));
		}
		props.ParseString(BlockSecureLoot.PropLockPickItem, ref this.LockPickItem);
		props.ParseFloat(BlockSecureLoot.PropLockPickTime, ref this.LockPickTime);
		props.ParseFloat(BlockSecureLoot.PropLockPickBreakChance, ref this.LockPickBreakChance);
		props.ParseString(BlockSecureLoot.PropOnLockPickSuccessEvent, ref this.LockPickSuccessEvent);
		props.ParseString(BlockSecureLoot.PropOnLockPickFailedEvent, ref this.LockPickFailedEvent);
		if (props.Values.ContainsKey(Block.PropLockpickDowngradeBlock))
		{
			string text = props.Values[Block.PropLockpickDowngradeBlock];
			if (!string.IsNullOrEmpty(text))
			{
				this.LockpickDowngradeBlock = Block.GetBlockValue(text, false);
				if (this.LockpickDowngradeBlock.isair)
				{
					throw new Exception("Block with name '" + text + "' not found in block " + base.Parent.TeData.Block.GetBlockName());
				}
			}
		}
		else
		{
			this.LockpickDowngradeBlock = base.Parent.TeData.Block.DowngradeBlock;
		}
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x00042C50 File Offset: 0x00040E50
	public void ShowLockpickUi(EntityPlayerLocal _player)
	{
		if (_player == null)
		{
			return;
		}
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_player);
		uiforPlayer.windowManager.Open("timer", true, false, true);
		XUiC_Timer childByType = uiforPlayer.xui.GetChildByType<XUiC_Timer>();
		float alternateTime = -1f;
		float num = _player.rand.RandomRange(1f);
		float value = EffectManager.GetValue(PassiveEffects.LockPickTime, _player.inventory.holdingItemItemValue, this.LockPickTime, _player, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		if (num < EffectManager.GetValue(PassiveEffects.LockPickBreakChance, _player.inventory.holdingItemItemValue, this.LockPickBreakChance, _player, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false))
		{
			float num2 = value - ((this.PickTimeLeft == -1f) ? (value - 1f) : (this.PickTimeLeft + 1f));
			alternateTime = _player.rand.RandomRange(num2 + 1f, value - 1f);
		}
		TimerEventData timerEventData = new TimerEventData();
		timerEventData.CloseEvent += this.EventData_CloseEvent;
		timerEventData.Data = _player;
		timerEventData.Event += this.EventData_Event;
		timerEventData.alternateTime = alternateTime;
		timerEventData.AlternateEvent += this.EventData_CloseEvent;
		childByType.SetTimer(value, timerEventData, this.PickTimeLeft, "");
		Manager.BroadcastPlayByLocalPlayer(base.Parent.ToWorldPos().ToVector3() + Vector3.one * 0.5f, "Misc/unlocking");
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x00042DC8 File Offset: 0x00040FC8
	[PublicizedFrom(EAccessModifier.Private)]
	public void EventData_CloseEvent(TimerEventData _timerData)
	{
		EntityPlayerLocal entityPlayerLocal = (EntityPlayerLocal)_timerData.Data;
		Vector3i vector3i = base.Parent.ToWorldPos();
		ItemValue item = ItemClass.GetItem(this.LockPickItem, false);
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
		Manager.BroadcastPlayByLocalPlayer(vector3i.ToVector3() + Vector3.one * 0.5f, "Misc/locked");
		ItemStack itemStack = new ItemStack(item, 1);
		uiforPlayer.xui.PlayerInventory.RemoveItem(itemStack);
		GameManager.ShowTooltip(entityPlayerLocal, Localization.Get("ttLockpickBroken", false), false, false, 0f);
		uiforPlayer.xui.CollectedItemList.RemoveItemStack(itemStack);
		this.PickTimeLeft = Mathf.Max(this.LockPickTime * 0.25f, _timerData.timeLeft);
		if (this.LockPickFailedEvent != null)
		{
			GameEventManager.Current.HandleAction(this.LockPickFailedEvent, null, entityPlayerLocal, false, vector3i, "", "", false, true, "", null);
		}
		this.ResetEventData(_timerData);
		GameManager.Instance.TEUnlockServer(0, vector3i, base.Parent.EntityId, false);
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x00042ED8 File Offset: 0x000410D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void EventData_Event(TimerEventData _timerData)
	{
		World world = GameManager.Instance.World;
		EntityPlayerLocal entity = (EntityPlayerLocal)_timerData.Data;
		Vector3i vector3i = base.Parent.ToWorldPos();
		BlockValue block = world.GetBlock(vector3i);
		this.lockFeature.SetLocked(false);
		if (!this.LockpickDowngradeBlock.isair)
		{
			BlockValue blockValue = base.Parent.TeData.Block.LockpickDowngradeBlock;
			blockValue = BlockPlaceholderMap.Instance.Replace(blockValue, world.GetGameRandom(), vector3i.x, vector3i.z, false);
			blockValue.rotation = block.rotation;
			blockValue.meta = block.meta;
			world.SetBlockRPC(0, vector3i, blockValue, blockValue.Block.Density);
		}
		Manager.BroadcastPlayByLocalPlayer(vector3i.ToVector3() + Vector3.one * 0.5f, "Misc/unlocking");
		if (this.LockPickSuccessEvent != null)
		{
			GameEventManager.Current.HandleAction(this.LockPickSuccessEvent, null, entity, false, vector3i, "", "", false, true, "", null);
		}
		this.ResetEventData(_timerData);
		GameManager.Instance.TEUnlockServer(0, vector3i, base.Parent.EntityId, false);
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x0004300B File Offset: 0x0004120B
	[PublicizedFrom(EAccessModifier.Private)]
	public void ResetEventData(TimerEventData _timerData)
	{
		_timerData.AlternateEvent -= this.EventData_CloseEvent;
		_timerData.CloseEvent -= this.EventData_CloseEvent;
		_timerData.Event -= this.EventData_Event;
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x00043043 File Offset: 0x00041243
	public override string GetActivationText(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityFocusing, string _activateHotkeyMarkup, string _focusedTileEntityName)
	{
		base.GetActivationText(_world, _blockPos, _blockValue, _entityFocusing, _activateHotkeyMarkup, _focusedTileEntityName);
		if (!this.lockFeature.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier))
		{
			return string.Format(Localization.Get("tooltipLocked", false), _activateHotkeyMarkup, _focusedTileEntityName);
		}
		return null;
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x0004307D File Offset: 0x0004127D
	public override void InitBlockActivationCommands(Action<BlockActivationCommand, TileEntityComposite.EBlockCommandOrder, TileEntityFeatureData> _addCallback)
	{
		base.InitBlockActivationCommands(_addCallback);
		_addCallback(new BlockActivationCommand("pick", "unlock", false, false, null), TileEntityComposite.EBlockCommandOrder.First, base.FeatureData);
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x000430A8 File Offset: 0x000412A8
	public override void UpdateBlockActivationCommands(ref BlockActivationCommand _command, ReadOnlySpan<char> _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityFocusing)
	{
		base.UpdateBlockActivationCommands(ref _command, _commandName, _world, _blockPos, _blockValue, _entityFocusing);
		if (base.CommandIs(_commandName, "pick"))
		{
			_command.enabled = (this.lockFeature.IsLocked() && !this.lockFeature.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier));
			return;
		}
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x000430FC File Offset: 0x000412FC
	public override bool OnBlockActivated(ReadOnlySpan<char> _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		base.OnBlockActivated(_commandName, _world, _blockPos, _blockValue, _player);
		if (!base.CommandIs(_commandName, "pick"))
		{
			return false;
		}
		LocalPlayerUI playerUI = _player.PlayerUI;
		ItemValue item = ItemClass.GetItem(this.LockPickItem, false);
		if (playerUI.xui.PlayerInventory.GetItemCount(item) == 0)
		{
			playerUI.xui.CollectedItemList.AddItemStack(new ItemStack(item, 0), true);
			GameManager.ShowTooltip(_player, Localization.Get("ttLockpickMissing", false), false, false, 0f);
			return true;
		}
		_player.AimingGun = false;
		Vector3i blockPos = base.Parent.ToWorldPos();
		_world.GetGameManager().TELockServer(0, blockPos, base.Parent.EntityId, _player.entityId, "lockpick");
		return true;
	}

	// Token: 0x0400091D RID: 2333
	[PublicizedFrom(EAccessModifier.Private)]
	public ILockable lockFeature;

	// Token: 0x0400091E RID: 2334
	[PublicizedFrom(EAccessModifier.Protected)]
	public string LockPickItem;

	// Token: 0x0400091F RID: 2335
	[PublicizedFrom(EAccessModifier.Protected)]
	public float LockPickTime = 15f;

	// Token: 0x04000920 RID: 2336
	[PublicizedFrom(EAccessModifier.Protected)]
	public float LockPickBreakChance;

	// Token: 0x04000921 RID: 2337
	[PublicizedFrom(EAccessModifier.Protected)]
	public string LockPickSuccessEvent;

	// Token: 0x04000922 RID: 2338
	[PublicizedFrom(EAccessModifier.Protected)]
	public string LockPickFailedEvent;

	// Token: 0x04000923 RID: 2339
	[PublicizedFrom(EAccessModifier.Protected)]
	public BlockValue LockpickDowngradeBlock;

	// Token: 0x04000924 RID: 2340
	[PublicizedFrom(EAccessModifier.Protected)]
	public float PickTimeLeft = -1f;
}
