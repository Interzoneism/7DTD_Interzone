using System;
using System.Collections;
using System.Globalization;
using Audio;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E9B RID: 3739
[Preserve]
public class XUiC_VehicleContainer : XUiC_ItemStackGrid
{
	// Token: 0x17000C06 RID: 3078
	// (get) Token: 0x06007603 RID: 30211 RVA: 0x00300FA1 File Offset: 0x002FF1A1
	public int containerSlotsCount
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.containerSlotsSize.x * this.containerSlotsSize.y;
		}
	}

	// Token: 0x17000C07 RID: 3079
	// (get) Token: 0x06007604 RID: 30212 RVA: 0x00300FBA File Offset: 0x002FF1BA
	// (set) Token: 0x06007605 RID: 30213 RVA: 0x00300FC2 File Offset: 0x002FF1C2
	public Vector2i GridCellSize { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000C08 RID: 3080
	// (get) Token: 0x06007606 RID: 30214 RVA: 0x00075E2B File Offset: 0x0007402B
	public override XUiC_ItemStack.StackLocationTypes StackLocation
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return XUiC_ItemStack.StackLocationTypes.Vehicle;
		}
	}

	// Token: 0x17000C09 RID: 3081
	// (get) Token: 0x06007607 RID: 30215 RVA: 0x00300FCB File Offset: 0x002FF1CB
	// (set) Token: 0x06007608 RID: 30216 RVA: 0x00300FD4 File Offset: 0x002FF1D4
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
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06007609 RID: 30217 RVA: 0x0027CA52 File Offset: 0x0027AC52
	public override ItemStack[] GetSlots()
	{
		return this.items;
	}

	// Token: 0x0600760A RID: 30218 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateBackend(ItemStack[] stackList)
	{
	}

	// Token: 0x0600760B RID: 30219 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SetStacks(ItemStack[] stackList)
	{
	}

	// Token: 0x0600760C RID: 30220 RVA: 0x00301034 File Offset: 0x002FF234
	public override void Init()
	{
		base.Init();
		this.window = (XUiV_Window)this.viewComponent;
		this.grid = (XUiV_Grid)base.GetChildById("queue").ViewComponent;
		this.GridCellSize = new Vector2i(this.grid.CellWidth, this.grid.CellHeight);
		this.standardControls = base.GetChildByType<XUiC_ContainerStandardControls>();
		if (this.standardControls != null)
		{
			this.standardControls.GetLockedSlotsFromStorage = delegate()
			{
				Bag bag = this.currentVehicleEntity.bag;
				PackedBoolArray result;
				if ((result = bag.LockedSlots) == null)
				{
					result = (bag.LockedSlots = new PackedBoolArray(this.containerSlotsCount));
				}
				return result;
			};
			this.standardControls.SetLockedSlotsToStorage = delegate(PackedBoolArray _slots)
			{
				this.currentVehicleEntity.bag.LockedSlots = _slots;
			};
			this.standardControls.ApplyLockedSlotStates = new Action<PackedBoolArray>(this.ApplyLockedSlotStates);
			this.standardControls.UpdateLockedSlotStates = new Action<XUiC_ContainerStandardControls>(this.UpdateLockedSlots);
			this.standardControls.LockModeToggled = delegate()
			{
				this.UserLockMode = !this.UserLockMode;
			};
			this.standardControls.SortPressed = new Action<PackedBoolArray>(this.btnSort_OnPress);
			this.standardControls.MoveAllowed = delegate(out XUiController _parentWindow, out XUiC_ItemStackGrid _grid, out IInventory _inventory)
			{
				_parentWindow = this;
				_grid = this;
				_inventory = base.xui.PlayerInventory;
				return true;
			};
			this.standardControls.MoveAllDone = delegate(bool _allMoved, bool _anyMoved)
			{
				if (_anyMoved)
				{
					Manager.BroadcastPlayByLocalPlayer(this.currentVehicleEntity.position + Vector3.one * 0.5f, "UseActions/takeall1");
				}
				if (_allMoved)
				{
					ThreadManager.StartCoroutine(this.closeInventoryLater());
				}
			};
		}
	}

	// Token: 0x0600760D RID: 30221 RVA: 0x00301164 File Offset: 0x002FF364
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnSort_OnPress(PackedBoolArray _ignoredSlos)
	{
		if (base.xui.vehicle.GetVehicle() == null)
		{
			return;
		}
		ItemStack[] slots = StackSortUtil.CombineAndSortStacks(base.xui.vehicle.bag.GetSlots(), 0, _ignoredSlos);
		base.xui.vehicle.bag.SetSlots(slots);
	}

	// Token: 0x0600760E RID: 30222 RVA: 0x003011B8 File Offset: 0x002FF3B8
	public void SetSlots(ItemStack[] stackList)
	{
		if (stackList == null)
		{
			return;
		}
		if (base.xui.vehicle.GetVehicle() == null)
		{
			return;
		}
		this.currentVehicleEntity = base.xui.vehicle;
		this.containerSlotsSize = this.currentVehicleEntity.lootContainer.GetContainerSize();
		Vector2i vector2i = new Vector2i(this.containerSlotsSize.x * this.GridCellSize.x, this.containerSlotsSize.y * this.GridCellSize.y);
		this.windowWidth = vector2i.x + this.windowGridWidthDifference;
		base.xui.vehicle.bag.OnBackpackItemsChangedInternal += this.OnBagItemChangedInternal;
		this.items = stackList;
		XUiC_ItemInfoWindow childByType = base.xui.GetChildByType<XUiC_ItemInfoWindow>();
		this.grid.Columns = this.containerSlotsSize.x;
		this.grid.Rows = this.containerSlotsSize.y;
		int num = stackList.Length;
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			XUiC_ItemStack xuiC_ItemStack = this.itemControllers[i];
			xuiC_ItemStack.SlotNumber = i;
			xuiC_ItemStack.SlotChangedEvent -= this.HandleLootSlotChangedEvent;
			xuiC_ItemStack.InfoWindow = childByType;
			xuiC_ItemStack.StackLocation = XUiC_ItemStack.StackLocationTypes.LootContainer;
			xuiC_ItemStack.UnlockStack();
			if (i < num)
			{
				xuiC_ItemStack.ForceSetItemStack(this.items[i]);
				this.itemControllers[i].ViewComponent.IsVisible = true;
				xuiC_ItemStack.SlotChangedEvent += this.HandleLootSlotChangedEvent;
			}
			else
			{
				xuiC_ItemStack.ItemStack = ItemStack.Empty.Clone();
				this.itemControllers[i].ViewComponent.IsVisible = false;
			}
		}
		base.RefreshBindings(true);
	}

	// Token: 0x0600760F RID: 30223 RVA: 0x0030136C File Offset: 0x002FF56C
	public override void UpdateInput()
	{
		base.UpdateInput();
		PlayerActionsLocal playerInput = base.xui.playerUI.playerInput;
		if (this.UserLockMode && (playerInput.GUIActions.Cancel.WasPressed || playerInput.PermanentActions.Cancel.WasPressed))
		{
			this.UserLockMode = false;
		}
	}

	// Token: 0x06007610 RID: 30224 RVA: 0x003013C4 File Offset: 0x002FF5C4
	public override void Update(float _dt)
	{
		if (GameManager.Instance == null && GameManager.Instance.World == null)
		{
			return;
		}
		if (this.IsDirty)
		{
			this.hasStorage = base.xui.vehicle.GetVehicle().HasStorage();
			base.ViewComponent.IsVisible = this.hasStorage;
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
		base.Update(_dt);
		if (this.windowGroup.isShowing)
		{
			if (!base.xui.playerUI.playerInput.PermanentActions.Activate.IsPressed)
			{
				this.wasReleased = true;
			}
			if (this.wasReleased)
			{
				if (base.xui.playerUI.playerInput.PermanentActions.Activate.IsPressed)
				{
					this.activeKeyDown = true;
				}
				if (base.xui.playerUI.playerInput.PermanentActions.Activate.WasReleased && this.activeKeyDown)
				{
					this.activeKeyDown = false;
					this.OnClose();
					base.xui.playerUI.windowManager.CloseAllOpenWindows(null, false);
				}
			}
			if (!this.isClosing && base.ViewComponent != null && base.ViewComponent.IsVisible && this.items != null && (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard || !base.xui.playerUI.windowManager.IsInputActive()) && (base.xui.playerUI.playerInput.GUIActions.LeftStick.WasPressed || base.xui.playerUI.playerInput.PermanentActions.Reload.WasPressed))
			{
				this.standardControls.MoveAll();
			}
		}
	}

	// Token: 0x06007611 RID: 30225 RVA: 0x0030158B File Offset: 0x002FF78B
	public void HandleLootSlotChangedEvent(int slotNumber, ItemStack stack)
	{
		if (base.xui.vehicle == null)
		{
			return;
		}
		base.xui.vehicle.bag.SetSlot(slotNumber, stack, true);
	}

	// Token: 0x06007612 RID: 30226 RVA: 0x003015BC File Offset: 0x002FF7BC
	public void OnBagItemChangedInternal()
	{
		if (base.xui.vehicle == null)
		{
			return;
		}
		ItemStack[] slots = base.xui.vehicle.bag.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			this.SetItemInSlot(i, slots[i]);
		}
		base.xui.vehicle.SetBagModified();
	}

	// Token: 0x06007613 RID: 30227 RVA: 0x0030161B File Offset: 0x002FF81B
	public void SetItemInSlot(int i, ItemStack stack)
	{
		if (i >= this.itemControllers.Length)
		{
			return;
		}
		this.itemControllers[i].ItemStack = stack;
	}

	// Token: 0x06007614 RID: 30228 RVA: 0x00301637 File Offset: 0x002FF837
	public override void OnOpen()
	{
		base.OnOpen();
		this.window.TargetAlpha = 1f;
		base.ViewComponent.OnOpen();
		base.ViewComponent.IsVisible = true;
		this.IsDirty = true;
	}

	// Token: 0x06007615 RID: 30229 RVA: 0x00301670 File Offset: 0x002FF870
	public override void OnClose()
	{
		base.OnClose();
		this.UserLockMode = false;
		this.currentVehicleEntity = null;
		if (base.xui.vehicle == null)
		{
			return;
		}
		base.xui.vehicle.bag.OnBackpackItemsChangedInternal -= this.OnBagItemChangedInternal;
		this.window.TargetAlpha = 0f;
		base.xui.vehicle = null;
	}

	// Token: 0x06007616 RID: 30230 RVA: 0x003016E2 File Offset: 0x002FF8E2
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator closeInventoryLater()
	{
		yield return null;
		base.xui.playerUI.windowManager.CloseIfOpen("vehicleStorage");
		this.isClosing = false;
		yield break;
	}

	// Token: 0x06007617 RID: 30231 RVA: 0x003016F1 File Offset: 0x002FF8F1
	public bool AddItem(ItemStack itemStack)
	{
		base.xui.vehicle.bag.TryStackItem(0, itemStack);
		return itemStack.count > 0 && base.xui.vehicle.bag.AddItem(itemStack);
	}

	// Token: 0x06007618 RID: 30232 RVA: 0x0030172F File Offset: 0x002FF92F
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "window_grid_width_difference")
		{
			this.windowGridWidthDifference = StringParsers.ParseSInt32(_value, 0, -1, NumberStyles.Integer);
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x06007619 RID: 30233 RVA: 0x00301758 File Offset: 0x002FF958
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "windowWidth")
		{
			_value = this.windowWidth.ToString();
			return true;
		}
		if (_bindingName == "take_all_tooltip")
		{
			_value = string.Format(Localization.Get("xuiLootTakeAllTooltip", false), "[action:permanent:Reload:emptystring:KeyboardWithAngleBrackets]");
			return true;
		}
		if (_bindingName == "buttons_visible")
		{
			_value = (this.windowWidth >= 450).ToString();
			return true;
		}
		if (_bindingName == "container_slots")
		{
			_value = this.containerSlotsCount.ToString();
			return true;
		}
		if (_bindingName == "userlockmode")
		{
			_value = this.UserLockMode.ToString();
			return true;
		}
		if (!(_bindingName == "hasslotlocks"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		XUiC_ContainerStandardControls xuiC_ContainerStandardControls = this.standardControls;
		_value = (((xuiC_ContainerStandardControls != null) ? xuiC_ContainerStandardControls.LockedSlots : null) != null).ToString();
		return true;
	}

	// Token: 0x0600761A RID: 30234 RVA: 0x0030184C File Offset: 0x002FFA4C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplyLockedSlotStates(PackedBoolArray _lockedSlots)
	{
		XUiC_ItemStack[] itemStackControllers = base.GetItemStackControllers();
		for (int i = 0; i < itemStackControllers.Length; i++)
		{
			itemStackControllers[i].UserLockedSlot = (_lockedSlots != null && i < _lockedSlots.Length && _lockedSlots[i]);
		}
	}

	// Token: 0x0600761B RID: 30235 RVA: 0x0030188C File Offset: 0x002FFA8C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateLockedSlots(XUiC_ContainerStandardControls _csc)
	{
		if (_csc == null)
		{
			return;
		}
		int containerSlotsCount = this.containerSlotsCount;
		PackedBoolArray packedBoolArray = _csc.LockedSlots ?? new PackedBoolArray(containerSlotsCount);
		if (packedBoolArray.Length != containerSlotsCount)
		{
			packedBoolArray.Length = containerSlotsCount;
		}
		XUiC_ItemStack[] itemStackControllers = base.GetItemStackControllers();
		int num = 0;
		while (num < itemStackControllers.Length && num < packedBoolArray.Length)
		{
			packedBoolArray[num] = itemStackControllers[num].UserLockedSlot;
			num++;
		}
		_csc.LockedSlots = packedBoolArray;
	}

	// Token: 0x04005A06 RID: 23046
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isClosing;

	// Token: 0x04005A07 RID: 23047
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasStorage;

	// Token: 0x04005A08 RID: 23048
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Window window;

	// Token: 0x04005A09 RID: 23049
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Grid grid;

	// Token: 0x04005A0A RID: 23050
	[PublicizedFrom(EAccessModifier.Private)]
	public bool userLockMode;

	// Token: 0x04005A0B RID: 23051
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ContainerStandardControls standardControls;

	// Token: 0x04005A0C RID: 23052
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i containerSlotsSize;

	// Token: 0x04005A0D RID: 23053
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityVehicle currentVehicleEntity;

	// Token: 0x04005A0F RID: 23055
	[PublicizedFrom(EAccessModifier.Private)]
	public int windowWidth;

	// Token: 0x04005A10 RID: 23056
	[PublicizedFrom(EAccessModifier.Private)]
	public int windowGridWidthDifference;

	// Token: 0x04005A11 RID: 23057
	[PublicizedFrom(EAccessModifier.Private)]
	public bool activeKeyDown;

	// Token: 0x04005A12 RID: 23058
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasReleased;
}
