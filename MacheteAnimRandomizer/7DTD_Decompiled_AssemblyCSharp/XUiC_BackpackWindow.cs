using System;
using UnityEngine.Scripting;

// Token: 0x02000C0D RID: 3085
[Preserve]
public class XUiC_BackpackWindow : XUiController
{
	// Token: 0x170009C4 RID: 2500
	// (get) Token: 0x06005E8F RID: 24207 RVA: 0x00265700 File Offset: 0x00263900
	// (set) Token: 0x06005E90 RID: 24208 RVA: 0x00265708 File Offset: 0x00263908
	public bool UserLockMode
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.userLockMode;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (value == this.userLockMode)
			{
				return;
			}
			if (this.userLockMode)
			{
				this.UpdateLockedSlots(this.standardControls);
			}
			XUiC_ContainerStandardControls xuiC_ContainerStandardControls = this.standardControls;
			if (xuiC_ContainerStandardControls != null)
			{
				xuiC_ContainerStandardControls.LockModeChanged(value);
			}
			this.userLockMode = value;
			base.WindowGroup.isEscClosable = !this.userLockMode;
			base.xui.playerUI.windowManager.GetModalWindow().isEscClosable = !this.userLockMode;
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06005E91 RID: 24209 RVA: 0x0026578C File Offset: 0x0026398C
	public override void Init()
	{
		base.Init();
		this.backpackGrid = base.GetChildByType<XUiC_Backpack>();
		this.standardControls = base.GetChildByType<XUiC_ContainerStandardControls>();
		if (this.standardControls != null)
		{
			this.standardControls.GetLockedSlotsFromStorage = (() => base.xui.playerUI.entityPlayer.bag.LockedSlots);
			this.standardControls.SetLockedSlotsToStorage = delegate(PackedBoolArray _slots)
			{
				base.xui.playerUI.entityPlayer.bag.LockedSlots = _slots;
			};
			this.standardControls.ApplyLockedSlotStates = new Action<PackedBoolArray>(this.ApplyLockedSlotStates);
			this.standardControls.UpdateLockedSlotStates = new Action<XUiC_ContainerStandardControls>(this.UpdateLockedSlots);
			this.standardControls.LockModeToggled = delegate()
			{
				this.UserLockMode = !this.UserLockMode;
			};
			this.standardControls.SortPressed = new Action<PackedBoolArray>(this.BtnSort_OnPress);
			this.standardControls.MoveAllowed = delegate(out XUiController _parentWindow, out XUiC_ItemStackGrid _grid, out IInventory _inventory)
			{
				_parentWindow = this;
				_grid = this.backpackGrid;
				return this.TryGetMoveDestinationInventory(out _inventory);
			};
		}
		XUiController childById = base.GetChildById("btnClearInventory");
		if (childById != null)
		{
			childById.OnPress += this.BtnClearInventory_OnPress;
		}
	}

	// Token: 0x06005E92 RID: 24210 RVA: 0x00265884 File Offset: 0x00263A84
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryGetMoveDestinationInventory(out IInventory _dstInventory)
	{
		_dstInventory = null;
		XUiM_AssembleItem assembleItem = base.xui.AssembleItem;
		if (((assembleItem != null) ? assembleItem.CurrentItem : null) != null)
		{
			return false;
		}
		bool flag = base.xui.vehicle != null && base.xui.vehicle.GetVehicle().HasStorage();
		bool flag2 = base.xui.lootContainer != null && base.xui.lootContainer.EntityId == -1;
		bool flag3 = base.xui.lootContainer != null && GameManager.Instance.World.GetEntity(base.xui.lootContainer.EntityId) is EntityDrone;
		if (!flag && !flag2 && !flag3)
		{
			return false;
		}
		if (flag && base.xui.FindWindowGroupByName(XUiC_VehicleStorageWindowGroup.ID).GetChildByType<XUiC_VehicleContainer>() == null)
		{
			return false;
		}
		if (flag3)
		{
			_dstInventory = base.xui.lootContainer;
		}
		else
		{
			IInventory inventory2;
			if (!flag2)
			{
				IInventory inventory = base.xui.vehicle.bag;
				inventory2 = inventory;
			}
			else
			{
				IInventory inventory = base.xui.lootContainer;
				inventory2 = inventory;
			}
			_dstInventory = inventory2;
		}
		return true;
	}

	// Token: 0x06005E93 RID: 24211 RVA: 0x00265997 File Offset: 0x00263B97
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnClearInventory_OnPress(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.entityPlayer.EmptyBackpack();
	}

	// Token: 0x06005E94 RID: 24212 RVA: 0x002659B0 File Offset: 0x00263BB0
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnSort_OnPress(PackedBoolArray _ignoredSlots)
	{
		ItemStack itemStack = null;
		if (base.xui.AssembleItem.CurrentItemStackController != null)
		{
			itemStack = base.xui.AssembleItem.CurrentItemStackController.ItemStack;
		}
		base.xui.PlayerInventory.SortStacks(0, _ignoredSlots);
		if (itemStack != null)
		{
			base.GetChildByType<XUiC_ItemStackGrid>().AssembleLockSingleStack(itemStack);
		}
	}

	// Token: 0x06005E95 RID: 24213 RVA: 0x00265A08 File Offset: 0x00263C08
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplyLockedSlotStates(PackedBoolArray _lockedSlots)
	{
		XUiC_ItemStack[] itemStackControllers = this.backpackGrid.GetItemStackControllers();
		for (int i = 0; i < itemStackControllers.Length; i++)
		{
			itemStackControllers[i].UserLockedSlot = (_lockedSlots != null && i < _lockedSlots.Length && _lockedSlots[i]);
		}
	}

	// Token: 0x06005E96 RID: 24214 RVA: 0x00265A50 File Offset: 0x00263C50
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateLockedSlots(XUiC_ContainerStandardControls _csc)
	{
		if (_csc == null)
		{
			return;
		}
		int slotCount = base.xui.PlayerInventory.Backpack.SlotCount;
		PackedBoolArray packedBoolArray = _csc.LockedSlots ?? new PackedBoolArray(slotCount);
		if (packedBoolArray.Length != slotCount)
		{
			packedBoolArray.Length = slotCount;
		}
		XUiC_ItemStack[] itemStackControllers = this.backpackGrid.GetItemStackControllers();
		int num = 0;
		while (num < itemStackControllers.Length && num < packedBoolArray.Length)
		{
			packedBoolArray[num] = itemStackControllers[num].UserLockedSlot;
			num++;
		}
		_csc.LockedSlots = packedBoolArray;
	}

	// Token: 0x06005E97 RID: 24215 RVA: 0x00265AD4 File Offset: 0x00263CD4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		if (bindingName == "currencyamount")
		{
			value = "0";
			if (XUi.IsGameRunning() && base.xui != null && base.xui.PlayerInventory != null)
			{
				value = this.currencyFormatter.Format(base.xui.PlayerInventory.CurrencyAmount);
			}
			return true;
		}
		if (bindingName == "currencyicon")
		{
			value = TraderInfo.CurrencyItem;
			return true;
		}
		if (bindingName == "lootingorvehiclestorage")
		{
			bool flag = base.xui.vehicle != null && base.xui.vehicle.GetVehicle().HasStorage();
			bool flag2 = base.xui.lootContainer != null && base.xui.lootContainer.EntityId == -1;
			bool flag3 = base.xui.lootContainer != null && GameManager.Instance.World.GetEntity(base.xui.lootContainer.EntityId) is EntityDrone;
			value = (flag || flag2 || flag3).ToString();
			return true;
		}
		if (bindingName == "creativewindowopen")
		{
			value = base.xui.playerUI.windowManager.IsWindowOpen("creative").ToString();
			return true;
		}
		if (!(bindingName == "userlockmode"))
		{
			return false;
		}
		value = this.UserLockMode.ToString();
		return true;
	}

	// Token: 0x06005E98 RID: 24216 RVA: 0x00265C50 File Offset: 0x00263E50
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.localPlayer == null)
		{
			this.localPlayer = base.xui.playerUI.entityPlayer;
		}
		base.xui.PlayerInventory.RefreshCurrency();
		base.xui.PlayerInventory.OnCurrencyChanged += this.PlayerInventory_OnCurrencyChanged;
		base.RefreshBindings(false);
		if (!string.IsNullOrEmpty(XUiC_BackpackWindow.defaultSelectedElement))
		{
			base.GetChildById(XUiC_BackpackWindow.defaultSelectedElement).SelectCursorElement(true, false);
			XUiC_BackpackWindow.defaultSelectedElement = "";
		}
		GameManager.Instance.StartCoroutine(this.localPlayer.CancelInventoryActions(delegate
		{
		}, false));
	}

	// Token: 0x06005E99 RID: 24217 RVA: 0x00265D1C File Offset: 0x00263F1C
	public override void OnClose()
	{
		base.OnClose();
		this.UserLockMode = false;
		if (base.xui != null && base.xui.PlayerInventory != null)
		{
			base.xui.PlayerInventory.OnCurrencyChanged -= this.PlayerInventory_OnCurrencyChanged;
		}
	}

	// Token: 0x06005E9A RID: 24218 RVA: 0x00265D70 File Offset: 0x00263F70
	public override void UpdateInput()
	{
		base.UpdateInput();
		PlayerActionsLocal playerInput = base.xui.playerUI.playerInput;
		if (this.UserLockMode && (playerInput.GUIActions.Cancel.WasPressed || playerInput.PermanentActions.Cancel.WasPressed))
		{
			this.UserLockMode = false;
		}
	}

	// Token: 0x06005E9B RID: 24219 RVA: 0x00080679 File Offset: 0x0007E879
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayerInventory_OnCurrencyChanged()
	{
		base.RefreshBindings(false);
	}

	// Token: 0x0400474E RID: 18254
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal localPlayer;

	// Token: 0x0400474F RID: 18255
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Backpack backpackGrid;

	// Token: 0x04004750 RID: 18256
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isHidden;

	// Token: 0x04004751 RID: 18257
	[PublicizedFrom(EAccessModifier.Private)]
	public bool userLockMode;

	// Token: 0x04004752 RID: 18258
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ContainerStandardControls standardControls;

	// Token: 0x04004753 RID: 18259
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt currencyFormatter = new CachedStringFormatterInt();

	// Token: 0x04004754 RID: 18260
	public static string defaultSelectedElement;
}
