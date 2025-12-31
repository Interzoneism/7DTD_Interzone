using System;
using Audio;
using GUI_2;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D87 RID: 3463
[Preserve]
public class XUiC_PowerRangedAmmoSlots : XUiC_ItemStackGrid
{
	// Token: 0x17000AE1 RID: 2785
	// (get) Token: 0x06006C3C RID: 27708 RVA: 0x002C3E66 File Offset: 0x002C2066
	// (set) Token: 0x06006C3D RID: 27709 RVA: 0x002C3E6E File Offset: 0x002C206E
	public TileEntityPoweredRangedTrap TileEntity
	{
		get
		{
			return this.tileEntity;
		}
		set
		{
			this.tileEntity = value;
			this.SetSlots(this.tileEntity.ItemSlots);
		}
	}

	// Token: 0x06006C3E RID: 27710 RVA: 0x002C3E88 File Offset: 0x002C2088
	public override void HandleSlotChangedEvent(int slotNumber, ItemStack stack)
	{
		base.HandleSlotChangedEvent(slotNumber, stack);
		this.tileEntity.SetSendSlots();
		this.tileEntity.SetModified();
	}

	// Token: 0x06006C3F RID: 27711 RVA: 0x002C3EA8 File Offset: 0x002C20A8
	public override void Init()
	{
		base.Init();
		this.btnOn = this.windowGroup.Controller.GetChildById("windowPowerTrapSlots").GetChildById("btnOn");
		this.btnOn_Background = (XUiV_Button)this.btnOn.GetChildById("clickable").ViewComponent;
		this.btnOn_Background.Controller.OnPress += this.btnOn_OnPress;
		XUiController childById = this.btnOn.GetChildById("lblOnOff");
		if (childById != null)
		{
			this.lblOnOff = (XUiV_Label)childById.ViewComponent;
		}
		childById = this.btnOn.GetChildById("sprOnOff");
		if (childById != null)
		{
			this.sprOnOff = (XUiV_Sprite)childById.ViewComponent;
		}
		this.isDirty = true;
		this.turnOff = Localization.Get("xuiUnlockAmmo", false);
		this.turnOn = Localization.Get("xuiLockAmmo", false);
	}

	// Token: 0x06006C40 RID: 27712 RVA: 0x002C3F90 File Offset: 0x002C2190
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnOn_OnPress(XUiController _sender, int _mouseButton)
	{
		bool flag = false;
		for (int i = 0; i < this.TileEntity.ItemSlots.Length; i++)
		{
			if (!this.TileEntity.ItemSlots[i].IsEmpty())
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
			GameManager.ShowTooltip(base.xui.playerUI.localPlayer.entityPlayerLocal, Localization.Get("ttRequiresAmmo", false), false, false, 0f);
			return;
		}
		this.tileEntity.IsLocked = !this.tileEntity.IsLocked;
		this.tileEntity.SetModified();
		this.RefreshIsLocked(this.tileEntity.IsLocked);
	}

	// Token: 0x06006C41 RID: 27713 RVA: 0x002C4048 File Offset: 0x002C2248
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetRequirements()
	{
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			XUiC_RequiredItemStack xuiC_RequiredItemStack = this.itemControllers[i] as XUiC_RequiredItemStack;
			if (xuiC_RequiredItemStack != null)
			{
				xuiC_RequiredItemStack.RequiredType = XUiC_RequiredItemStack.RequiredTypes.ItemClass;
				xuiC_RequiredItemStack.RequiredItemClass = this.tileEntity.AmmoItem;
			}
		}
	}

	// Token: 0x17000AE2 RID: 2786
	// (get) Token: 0x06006C42 RID: 27714 RVA: 0x00076E19 File Offset: 0x00075019
	public override XUiC_ItemStack.StackLocationTypes StackLocation
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return XUiC_ItemStack.StackLocationTypes.Workstation;
		}
	}

	// Token: 0x17000AE3 RID: 2787
	// (get) Token: 0x06006C43 RID: 27715 RVA: 0x002C4091 File Offset: 0x002C2291
	// (set) Token: 0x06006C44 RID: 27716 RVA: 0x002C4099 File Offset: 0x002C2299
	public XUiC_PowerRangedTrapWindowGroup Owner { get; set; }

	// Token: 0x06006C45 RID: 27717 RVA: 0x002C40A2 File Offset: 0x002C22A2
	public virtual void SetSlots(ItemStack[] stacks)
	{
		this.items = stacks;
		base.SetStacks(stacks);
	}

	// Token: 0x06006C46 RID: 27718 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool HasRequirement(Recipe recipe)
	{
		return true;
	}

	// Token: 0x06006C47 RID: 27719 RVA: 0x002C40B4 File Offset: 0x002C22B4
	public override void OnOpen()
	{
		base.OnOpen();
		this.tileEntity.SetUserAccessing(true);
		if (base.ViewComponent != null && !base.ViewComponent.IsVisible)
		{
			base.ViewComponent.OnOpen();
			base.ViewComponent.IsVisible = true;
		}
		this.IsDirty = true;
		this.SetRequirements();
		bool isLocked = this.tileEntity.IsLocked;
		this.RefreshIsLocked(isLocked);
		base.xui.powerAmmoSlots = this;
		XUiC_PowerRangedAmmoSlots.Current = this;
		this.IsDormant = false;
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.RightBumper, "igcoPoweredTrapLockAmmo", XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts);
		base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts, 0f);
	}

	// Token: 0x06006C48 RID: 27720 RVA: 0x002C4168 File Offset: 0x002C2368
	public override void OnClose()
	{
		base.OnClose();
		GameManager instance = GameManager.Instance;
		if (base.ViewComponent != null && base.ViewComponent.IsVisible)
		{
			base.ViewComponent.OnClose();
			base.ViewComponent.IsVisible = false;
		}
		Vector3i blockPos = this.tileEntity.ToWorldPos();
		if (!XUiC_CameraWindow.hackyIsOpeningMaximizedWindow)
		{
			this.tileEntity.SetUserAccessing(false);
			instance.TEUnlockServer(this.tileEntity.GetClrIdx(), blockPos, this.tileEntity.entityId, true);
			this.tileEntity.SetModified();
		}
		this.IsDirty = true;
		this.tileEntity = null;
		XUiC_PowerRangedAmmoSlots.Current = (base.xui.powerAmmoSlots = null);
		this.IsDormant = true;
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts);
	}

	// Token: 0x06006C49 RID: 27721 RVA: 0x002C4230 File Offset: 0x002C2430
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshIsLocked(bool isOn)
	{
		if (isOn)
		{
			this.lblOnOff.Text = this.turnOff;
			if (this.sprOnOff != null)
			{
				this.sprOnOff.Color = this.onColor;
				this.sprOnOff.SpriteName = "ui_game_symbol_lock";
			}
		}
		else
		{
			this.lblOnOff.Text = this.turnOn;
			if (this.sprOnOff != null)
			{
				this.sprOnOff.Color = this.offColor;
				this.sprOnOff.SpriteName = "ui_game_symbol_unlock";
			}
		}
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			XUiC_RequiredItemStack xuiC_RequiredItemStack = this.itemControllers[i] as XUiC_RequiredItemStack;
			if (xuiC_RequiredItemStack != null)
			{
				xuiC_RequiredItemStack.ToolLock = isOn;
			}
		}
	}

	// Token: 0x06006C4A RID: 27722 RVA: 0x002C42EC File Offset: 0x002C24EC
	[PublicizedFrom(EAccessModifier.Internal)]
	public void SetLocked(bool isOn)
	{
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			XUiC_RequiredItemStack xuiC_RequiredItemStack = this.itemControllers[i] as XUiC_RequiredItemStack;
			if (xuiC_RequiredItemStack != null)
			{
				xuiC_RequiredItemStack.ToolLock = isOn;
			}
		}
	}

	// Token: 0x06006C4B RID: 27723 RVA: 0x002C4324 File Offset: 0x002C2524
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateBackend(ItemStack[] stackList)
	{
		base.UpdateBackend(stackList);
		this.tileEntity.ItemSlots = stackList;
		this.tileEntity.SetSendSlots();
		this.windowGroup.Controller.SetAllChildrenDirty(false);
	}

	// Token: 0x06006C4C RID: 27724 RVA: 0x002C4358 File Offset: 0x002C2558
	public override void Update(float _dt)
	{
		if (GameManager.Instance == null && GameManager.Instance.World == null)
		{
			return;
		}
		if (this.tileEntity == null)
		{
			return;
		}
		base.Update(_dt);
		if (this.lastLocked != this.tileEntity.IsLocked)
		{
			this.lastLocked = this.tileEntity.IsLocked;
			this.RefreshIsLocked(this.tileEntity.IsLocked);
		}
		if (this.tileEntity.IsLocked)
		{
			this.SetSlots(this.tileEntity.ItemSlots);
		}
		if (base.xui.playerUI.playerInput.GUIActions.WindowPagingRight.WasPressed && PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard)
		{
			this.btnOn_OnPress(null, 0);
		}
		base.RefreshBindings(false);
	}

	// Token: 0x06006C4D RID: 27725 RVA: 0x002C4424 File Offset: 0x002C2624
	public void Refresh()
	{
		this.SetSlots(this.tileEntity.ItemSlots);
	}

	// Token: 0x06006C4E RID: 27726 RVA: 0x002C4437 File Offset: 0x002C2637
	[PublicizedFrom(EAccessModifier.Internal)]
	public bool TryAddItemToSlot(ItemClass itemClass, ItemStack itemStack)
	{
		if (itemClass != this.tileEntity.AmmoItem)
		{
			return false;
		}
		this.tileEntity.TryStackItem(itemStack);
		this.SetSlots(this.tileEntity.ItemSlots);
		return itemStack.count == 0;
	}

	// Token: 0x04005256 RID: 21078
	public static XUiC_PowerRangedAmmoSlots Current;

	// Token: 0x04005257 RID: 21079
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnOn;

	// Token: 0x04005258 RID: 21080
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnOn_Background;

	// Token: 0x04005259 RID: 21081
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblOnOff;

	// Token: 0x0400525A RID: 21082
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite sprOnOff;

	// Token: 0x0400525B RID: 21083
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 onColor = new Color32(250, byte.MaxValue, 163, byte.MaxValue);

	// Token: 0x0400525C RID: 21084
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 offColor = Color.white;

	// Token: 0x0400525D RID: 21085
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lastLocked;

	// Token: 0x0400525E RID: 21086
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x0400525F RID: 21087
	[PublicizedFrom(EAccessModifier.Private)]
	public string turnOff;

	// Token: 0x04005260 RID: 21088
	[PublicizedFrom(EAccessModifier.Private)]
	public string turnOn;

	// Token: 0x04005261 RID: 21089
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityPoweredRangedTrap tileEntity;
}
