using System;
using System.Collections;
using System.Globalization;
using Audio;
using GUI_2;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C8B RID: 3211
[Preserve]
public class XUiC_DewCollectorWindow : XUiController
{
	// Token: 0x17000A1B RID: 2587
	// (get) Token: 0x0600630D RID: 25357 RVA: 0x002839A7 File Offset: 0x00281BA7
	public int ContainerSlotsCount
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.containerSlotsSize.x * this.containerSlotsSize.y;
		}
	}

	// Token: 0x0600630E RID: 25358 RVA: 0x002839C0 File Offset: 0x00281BC0
	public override void Init()
	{
		base.Init();
		this.container = base.GetChildByType<XUiC_DewCollectorContainer>();
		this.controls = base.GetChildByType<XUiC_ContainerStandardControls>();
		if (this.controls != null)
		{
			this.controls.SortPressed = delegate(PackedBoolArray _ignoredSlots)
			{
				ItemStack[] array = StackSortUtil.CombineAndSortStacks(this.te.items, 0, _ignoredSlots);
				for (int i = 0; i < array.Length; i++)
				{
					this.te.UpdateSlot(i, array[i]);
				}
				this.te.SetModified();
			};
			this.controls.MoveAllowed = delegate(out XUiController _parentWindow, out XUiC_ItemStackGrid _grid, out IInventory _inventory)
			{
				_parentWindow = this;
				_grid = this.container;
				_inventory = base.xui.PlayerInventory;
				return true;
			};
			this.controls.MoveAllDone = delegate(bool _allMoved, bool _anyMoved)
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
	}

	// Token: 0x0600630F RID: 25359 RVA: 0x00283A38 File Offset: 0x00281C38
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
		if (GameManager.Instance == null && GameManager.Instance.World == null)
		{
			return;
		}
		if (!this.isClosing && base.ViewComponent != null && base.ViewComponent.IsVisible && (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard || !base.xui.playerUI.windowManager.IsInputActive()) && (base.xui.playerUI.playerInput.GUIActions.LeftStick.WasPressed || base.xui.playerUI.playerInput.PermanentActions.Reload.WasPressed))
		{
			this.controls.MoveAll();
		}
	}

	// Token: 0x06006310 RID: 25360 RVA: 0x00283BB0 File Offset: 0x00281DB0
	public void SetTileEntity(TileEntityDewCollector _te)
	{
		this.te = _te;
		if (this.te != null)
		{
			this.containerSlotsSize = this.te.GetContainerSize();
			Vector2i vector2i = new Vector2i(this.containerSlotsSize.x * this.container.GridCellSize.x, this.containerSlotsSize.y * this.container.GridCellSize.y);
			this.windowWidth = vector2i.x + this.windowGridWidthDifference;
			this.te.HandleUpdate(GameManager.Instance.World);
			this.container.SetSlots(this.te, this.te.GetItems());
			base.RefreshBindings(true);
		}
	}

	// Token: 0x06006311 RID: 25361 RVA: 0x00283C6A File Offset: 0x00281E6A
	public override void OnOpen()
	{
		base.OnOpen();
		this.isClosing = false;
	}

	// Token: 0x06006312 RID: 25362 RVA: 0x00283C7C File Offset: 0x00281E7C
	public override void OnClose()
	{
		this.wasReleased = false;
		this.activeKeyDown = false;
		GameManager instance = GameManager.Instance;
		Vector3i blockPos = this.te.ToWorldPos();
		this.te.SetUserAccessing(false);
		this.te.SetModified();
		instance.TEUnlockServer(this.te.GetClrIdx(), blockPos, this.te.entityId, true);
		this.SetTileEntity(null);
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuLoot);
		base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuLoot);
		base.OnClose();
	}

	// Token: 0x06006313 RID: 25363 RVA: 0x00283D0C File Offset: 0x00281F0C
	public void OpenContainer()
	{
		this.container.SetSlots(this.te, this.te.GetItems());
		base.OnOpen();
		this.te.SetUserAccessing(true);
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuLoot);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.LeftStickButton, "igcoLootAll", XUiC_GamepadCalloutWindow.CalloutType.MenuLoot);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonNorth, "igcoInspect", XUiC_GamepadCalloutWindow.CalloutType.MenuLoot);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonEast, "igcoExit", XUiC_GamepadCalloutWindow.CalloutType.MenuLoot);
		base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuLoot, 0f);
		base.RefreshBindings(true);
	}

	// Token: 0x06006314 RID: 25364 RVA: 0x00283DBB File Offset: 0x00281FBB
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator closeInventoryLater()
	{
		this.isClosing = true;
		yield return null;
		base.xui.playerUI.windowManager.CloseIfOpen("dewcollector");
		this.isClosing = false;
		yield break;
	}

	// Token: 0x06006315 RID: 25365 RVA: 0x00283DCA File Offset: 0x00281FCA
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "window_grid_width_difference")
		{
			this.windowGridWidthDifference = StringParsers.ParseSInt32(_value, 0, -1, NumberStyles.Integer);
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x06006316 RID: 25366 RVA: 0x00283DF4 File Offset: 0x00281FF4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "windowWidth")
		{
			_value = this.windowWidth.ToString();
			return true;
		}
		if (_bindingName == "lootcontainer_name")
		{
			_value = Localization.Get("xuiDewCollector", false);
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
		if (!(_bindingName == "container_slots"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = this.ContainerSlotsCount.ToString();
		return true;
	}

	// Token: 0x04004AA5 RID: 19109
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityDewCollector te;

	// Token: 0x04004AA6 RID: 19110
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DewCollectorContainer container;

	// Token: 0x04004AA7 RID: 19111
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ContainerStandardControls controls;

	// Token: 0x04004AA8 RID: 19112
	[PublicizedFrom(EAccessModifier.Private)]
	public string lootContainerName;

	// Token: 0x04004AA9 RID: 19113
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i containerSlotsSize;

	// Token: 0x04004AAA RID: 19114
	[PublicizedFrom(EAccessModifier.Private)]
	public int windowWidth;

	// Token: 0x04004AAB RID: 19115
	[PublicizedFrom(EAccessModifier.Private)]
	public int windowGridWidthDifference;

	// Token: 0x04004AAC RID: 19116
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isClosing;

	// Token: 0x04004AAD RID: 19117
	[PublicizedFrom(EAccessModifier.Private)]
	public bool activeKeyDown;

	// Token: 0x04004AAE RID: 19118
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasReleased;
}
