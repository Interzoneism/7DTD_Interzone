using System;
using System.Collections;
using System.Globalization;
using Audio;
using GUI_2;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D0A RID: 3338
[Preserve]
public class XUiC_LootWindow : XUiController
{
	// Token: 0x17000A9F RID: 2719
	// (get) Token: 0x060067D2 RID: 26578 RVA: 0x002A15F8 File Offset: 0x0029F7F8
	public int ContainerSlotsCount
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.containerSlotsSize.x * this.containerSlotsSize.y;
		}
	}

	// Token: 0x17000AA0 RID: 2720
	// (get) Token: 0x060067D3 RID: 26579 RVA: 0x002A1611 File Offset: 0x0029F811
	// (set) Token: 0x060067D4 RID: 26580 RVA: 0x002A161C File Offset: 0x0029F81C
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

	// Token: 0x060067D5 RID: 26581 RVA: 0x002A167C File Offset: 0x0029F87C
	public override void Init()
	{
		base.Init();
		this.lootContainer = base.GetChildByType<XUiC_LootContainer>();
		this.standardControls = base.GetChildByType<XUiC_ContainerStandardControls>();
		if (this.standardControls != null)
		{
			this.standardControls.GetLockedSlotsFromStorage = delegate()
			{
				if (!this.te.HasSlotLocksSupport)
				{
					return null;
				}
				return this.te.SlotLocks;
			};
			this.standardControls.SetLockedSlotsToStorage = delegate(PackedBoolArray _slots)
			{
				if (this.te.HasSlotLocksSupport)
				{
					this.te.SlotLocks = _slots;
				}
			};
			this.standardControls.ApplyLockedSlotStates = new Action<PackedBoolArray>(this.ApplyLockedSlotStates);
			this.standardControls.UpdateLockedSlotStates = new Action<XUiC_ContainerStandardControls>(this.UpdateLockedSlots);
			this.standardControls.LockModeToggled = delegate()
			{
				this.UserLockMode = !this.UserLockMode;
			};
			this.standardControls.SortPressed = delegate(PackedBoolArray _ignoredSlots)
			{
				ItemStack[] array = StackSortUtil.CombineAndSortStacks(this.te.items, 0, _ignoredSlots);
				for (int i = 0; i < array.Length; i++)
				{
					this.te.UpdateSlot(i, array[i]);
				}
				this.te.SetModified();
			};
			this.standardControls.MoveAllowed = delegate(out XUiController _parentWindow, out XUiC_ItemStackGrid _grid, out IInventory _inventory)
			{
				_parentWindow = this;
				_grid = this.lootContainer;
				_inventory = base.xui.PlayerInventory;
				return true;
			};
			this.standardControls.MoveAllDone = delegate(bool _allMoved, bool _anyMoved)
			{
				if (_anyMoved)
				{
					Manager.BroadcastPlayByLocalPlayer(this.te.ToWorldPos().ToVector3() + Vector3.one * 0.5f, "UseActions/takeall1");
				}
				if (_allMoved)
				{
					ThreadManager.StartCoroutine(this.closeInventoryLater());
				}
			};
		}
		base.RegisterForInputStyleChanges();
	}

	// Token: 0x060067D6 RID: 26582 RVA: 0x002A1770 File Offset: 0x0029F970
	public override void Update(float _dt)
	{
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
					base.xui.playerUI.windowManager.CloseAllOpenWindows(null, false);
				}
			}
		}
		if (this.te != null)
		{
			Vector3 vector = this.te.ToWorldCenterPos();
			if (vector != Vector3.zero)
			{
				float num = Constants.cCollectItemDistance + 30f;
				float sqrMagnitude = (base.xui.playerUI.entityPlayer.position - vector).sqrMagnitude;
				if (sqrMagnitude > num * num)
				{
					Log.Out("Loot Window closed at distance {0}", new object[]
					{
						Mathf.Sqrt(sqrMagnitude)
					});
					base.xui.playerUI.windowManager.CloseAllOpenWindows(null, false);
					this.CloseContainer(false);
				}
			}
		}
		if (GameManager.Instance == null && GameManager.Instance.World == null)
		{
			return;
		}
		if (!this.isClosing && base.ViewComponent != null && base.ViewComponent.IsVisible && (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard || !base.xui.playerUI.windowManager.IsInputActive()) && (base.xui.playerUI.playerInput.GUIActions.LeftStick.WasPressed || base.xui.playerUI.playerInput.PermanentActions.Reload.WasPressed))
		{
			this.standardControls.MoveAll();
		}
	}

	// Token: 0x060067D7 RID: 26583 RVA: 0x002A1980 File Offset: 0x0029FB80
	public override void UpdateInput()
	{
		base.UpdateInput();
		PlayerActionsLocal playerInput = base.xui.playerUI.playerInput;
		if (this.UserLockMode && (playerInput.GUIActions.Cancel.WasPressed || playerInput.PermanentActions.Cancel.WasPressed))
		{
			this.UserLockMode = false;
		}
	}

	// Token: 0x060067D8 RID: 26584 RVA: 0x002A19D8 File Offset: 0x0029FBD8
	public void SetTileEntityChest(string _lootContainerName, ITileEntityLootable _te)
	{
		this.te = _te;
		this.lootContainerName = _lootContainerName;
		if (this.te != null)
		{
			this.containerSlotsSize = this.te.GetContainerSize();
			Vector2i vector2i = new Vector2i(this.containerSlotsSize.x * this.lootContainer.GridCellSize.x, this.containerSlotsSize.y * this.lootContainer.GridCellSize.y);
			this.windowWidth = vector2i.x + this.windowGridWidthDifference;
			this.lootContainer.SetSlots(this.te, this.te.items);
			ITileEntitySignable selfOrFeature = _te.GetSelfOrFeature<ITileEntitySignable>();
			if (selfOrFeature != null)
			{
				GeneratedTextManager.GetDisplayText(selfOrFeature.GetAuthoredText(), delegate(string containerName)
				{
					if (!string.IsNullOrEmpty(containerName))
					{
						this.lootContainerName = containerName;
					}
					base.RefreshBindings(true);
				}, true, true, GeneratedTextManager.TextFilteringMode.FilterWithSafeString, GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes);
				return;
			}
			base.RefreshBindings(true);
		}
	}

	// Token: 0x060067D9 RID: 26585 RVA: 0x002A1AAA File Offset: 0x0029FCAA
	public override void OnOpen()
	{
		base.OnOpen();
		this.isClosing = false;
	}

	// Token: 0x060067DA RID: 26586 RVA: 0x002A1ABC File Offset: 0x0029FCBC
	public override void OnClose()
	{
		this.wasReleased = false;
		this.activeKeyDown = false;
		this.UserLockMode = false;
		EntityPlayer entityPlayer = base.xui.playerUI.entityPlayer;
		entityPlayer.MinEventContext.TileEntity = this.te;
		if (this.te.EntityId == -1)
		{
			entityPlayer.MinEventContext.BlockValue = this.te.blockValue;
		}
		entityPlayer.FireEvent(MinEventTypes.onSelfCloseLootContainer, true);
	}

	// Token: 0x060067DB RID: 26587 RVA: 0x002A1B30 File Offset: 0x0029FD30
	public void OpenContainer()
	{
		this.lootContainer.SetSlots(this.te, this.te.items);
		base.OnOpen();
		this.te.SetUserAccessing(true);
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuLoot);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.LeftStickButton, "igcoLootAll", XUiC_GamepadCalloutWindow.CalloutType.MenuLoot);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonNorth, "igcoInspect", XUiC_GamepadCalloutWindow.CalloutType.MenuLoot);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonEast, "igcoExit", XUiC_GamepadCalloutWindow.CalloutType.MenuLoot);
		base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuLoot, 0f);
		base.RefreshBindings(true);
		this.lootContainer.SelectCursorElement(true, false);
	}

	// Token: 0x060067DC RID: 26588 RVA: 0x002A1BF0 File Offset: 0x0029FDF0
	public void CloseContainer(bool ignoreCloseSound)
	{
		if (this.te == null)
		{
			return;
		}
		GameManager instance = GameManager.Instance;
		if (!ignoreCloseSound)
		{
			LootContainer lootContainer = LootContainer.GetLootContainer(this.te.lootListName, true);
			if (lootContainer != null && lootContainer.soundClose != null)
			{
				Vector3 position = this.te.ToWorldPos().ToVector3() + Vector3.one * 0.5f;
				if (this.te.EntityId != -1 && GameManager.Instance.World != null)
				{
					Entity entity = GameManager.Instance.World.GetEntity(this.te.EntityId);
					if (entity != null)
					{
						position = entity.GetPosition();
					}
				}
				Manager.BroadcastPlayByLocalPlayer(position, lootContainer.soundClose);
			}
		}
		Vector3i blockPos = this.te.ToWorldPos();
		ITileEntityLootable selfOrFeature = GameManager.Instance.World.GetTileEntity(this.te.GetClrIdx(), blockPos).GetSelfOrFeature<ITileEntityLootable>();
		if ((selfOrFeature == null || !selfOrFeature.IsRemoving) && selfOrFeature == this.te)
		{
			this.te.SetModified();
		}
		this.te.SetUserAccessing(false);
		instance.TEUnlockServer(this.te.GetClrIdx(), blockPos, this.te.EntityId, true);
		this.SetTileEntityChest("", null);
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuLoot);
		base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuLoot);
		base.OnClose();
	}

	// Token: 0x060067DD RID: 26589 RVA: 0x002A1D57 File Offset: 0x0029FF57
	public PreferenceTracker GetPreferenceTrackerFromTileEntity()
	{
		ITileEntityLootable tileEntityLootable = this.te;
		if (tileEntityLootable == null)
		{
			return null;
		}
		return tileEntityLootable.preferences;
	}

	// Token: 0x060067DE RID: 26590 RVA: 0x002A1D6A File Offset: 0x0029FF6A
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator closeInventoryLater()
	{
		this.isClosing = true;
		yield return null;
		base.xui.playerUI.windowManager.CloseIfOpen("looting");
		this.isClosing = false;
		yield break;
	}

	// Token: 0x060067DF RID: 26591 RVA: 0x002A1D79 File Offset: 0x0029FF79
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "window_grid_width_difference")
		{
			this.windowGridWidthDifference = StringParsers.ParseSInt32(_value, 0, -1, NumberStyles.Integer);
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x060067E0 RID: 26592 RVA: 0x002A1DA4 File Offset: 0x0029FFA4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
		if (num <= 775821220U)
		{
			if (num != 58930246U)
			{
				if (num != 610671175U)
				{
					if (num == 775821220U)
					{
						if (_bindingName == "lootcontainer_name")
						{
							_value = ((!string.IsNullOrEmpty(this.lootContainerName)) ? this.lootContainerName : Localization.Get("xuiLoot", false));
							return true;
						}
					}
				}
				else if (_bindingName == "windowWidth")
				{
					_value = this.windowWidth.ToString();
					return true;
				}
			}
			else if (_bindingName == "container_slots")
			{
				_value = this.ContainerSlotsCount.ToString();
				return true;
			}
		}
		else if (num <= 1899514017U)
		{
			if (num != 1249315359U)
			{
				if (num == 1899514017U)
				{
					if (_bindingName == "hasslotlocks")
					{
						ITileEntityLootable tileEntityLootable = this.te;
						_value = (tileEntityLootable != null && tileEntityLootable.HasSlotLocksSupport).ToString();
						return true;
					}
				}
			}
			else if (_bindingName == "buttons_visible")
			{
				_value = (this.windowWidth >= 450).ToString();
				return true;
			}
		}
		else if (num != 2305669482U)
		{
			if (num == 4041168338U)
			{
				if (_bindingName == "take_all_tooltip")
				{
					if (base.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard)
					{
						_value = string.Format(Localization.Get("xuiLootTakeAllTooltip", false), "[action:permanent:Reload:emptystring:KeyboardWithAngleBrackets]");
					}
					else
					{
						_value = string.Format(Localization.Get("xuiLootTakeAllTooltip", false), base.xui.playerUI.playerInput.GUIActions.LeftStick.GetBindingString(true, PlatformManager.NativePlatform.Input.CurrentControllerInputStyle, XUiUtils.EmptyBindingStyle.LocalizedNone, XUiUtils.DisplayStyle.Plain, false, null));
					}
					return true;
				}
			}
		}
		else if (_bindingName == "userlockmode")
		{
			_value = this.UserLockMode.ToString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x060067E1 RID: 26593 RVA: 0x002A1FA8 File Offset: 0x002A01A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplyLockedSlotStates(PackedBoolArray _lockedSlots)
	{
		XUiC_ItemStack[] itemStackControllers = this.lootContainer.GetItemStackControllers();
		for (int i = 0; i < itemStackControllers.Length; i++)
		{
			itemStackControllers[i].UserLockedSlot = (_lockedSlots != null && i < _lockedSlots.Length && _lockedSlots[i]);
		}
	}

	// Token: 0x060067E2 RID: 26594 RVA: 0x002A1FF0 File Offset: 0x002A01F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateLockedSlots(XUiC_ContainerStandardControls _csc)
	{
		if (_csc == null)
		{
			return;
		}
		int containerSlotsCount = this.ContainerSlotsCount;
		PackedBoolArray packedBoolArray = _csc.LockedSlots ?? new PackedBoolArray(containerSlotsCount);
		if (packedBoolArray.Length != containerSlotsCount)
		{
			packedBoolArray.Length = containerSlotsCount;
		}
		XUiC_ItemStack[] itemStackControllers = this.lootContainer.GetItemStackControllers();
		int num = 0;
		while (num < itemStackControllers.Length && num < packedBoolArray.Length)
		{
			packedBoolArray[num] = itemStackControllers[num].UserLockedSlot;
			num++;
		}
		_csc.LockedSlots = packedBoolArray;
	}

	// Token: 0x04004E50 RID: 20048
	[PublicizedFrom(EAccessModifier.Private)]
	public ITileEntityLootable te;

	// Token: 0x04004E51 RID: 20049
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_LootContainer lootContainer;

	// Token: 0x04004E52 RID: 20050
	[PublicizedFrom(EAccessModifier.Private)]
	public bool userLockMode;

	// Token: 0x04004E53 RID: 20051
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ContainerStandardControls standardControls;

	// Token: 0x04004E54 RID: 20052
	[PublicizedFrom(EAccessModifier.Private)]
	public string lootContainerName;

	// Token: 0x04004E55 RID: 20053
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i containerSlotsSize;

	// Token: 0x04004E56 RID: 20054
	[PublicizedFrom(EAccessModifier.Private)]
	public int windowWidth;

	// Token: 0x04004E57 RID: 20055
	[PublicizedFrom(EAccessModifier.Private)]
	public int windowGridWidthDifference;

	// Token: 0x04004E58 RID: 20056
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isClosing;

	// Token: 0x04004E59 RID: 20057
	[PublicizedFrom(EAccessModifier.Private)]
	public bool activeKeyDown;

	// Token: 0x04004E5A RID: 20058
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasReleased;
}
