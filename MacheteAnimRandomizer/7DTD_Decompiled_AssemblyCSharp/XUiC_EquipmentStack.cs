using System;
using System.Globalization;
using Audio;
using InControl;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000CAE RID: 3246
[UnityEngine.Scripting.Preserve]
public class XUiC_EquipmentStack : XUiC_SelectableEntry
{
	// Token: 0x17000A3B RID: 2619
	// (get) Token: 0x06006441 RID: 25665 RVA: 0x00289D2A File Offset: 0x00287F2A
	// (set) Token: 0x06006442 RID: 25666 RVA: 0x00289D32 File Offset: 0x00287F32
	public EquipmentSlots EquipSlot
	{
		get
		{
			return this.equipSlot;
		}
		set
		{
			this.equipSlot = value;
			this.SetEmptySpriteNameAndTooltip();
		}
	}

	// Token: 0x17000A3C RID: 2620
	// (get) Token: 0x06006443 RID: 25667 RVA: 0x00289D41 File Offset: 0x00287F41
	// (set) Token: 0x06006444 RID: 25668 RVA: 0x00289D49 File Offset: 0x00287F49
	public int SlotNumber { get; set; }

	// Token: 0x140000A1 RID: 161
	// (add) Token: 0x06006445 RID: 25669 RVA: 0x00289D54 File Offset: 0x00287F54
	// (remove) Token: 0x06006446 RID: 25670 RVA: 0x00289D8C File Offset: 0x00287F8C
	public event XUiEvent_SlotChangedEventHandler SlotChangedEvent;

	// Token: 0x17000A3D RID: 2621
	// (get) Token: 0x06006447 RID: 25671 RVA: 0x00289DC1 File Offset: 0x00287FC1
	// (set) Token: 0x06006448 RID: 25672 RVA: 0x00289DC9 File Offset: 0x00287FC9
	public float HoverIconGrow { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000A3E RID: 2622
	// (get) Token: 0x06006449 RID: 25673 RVA: 0x00289DD2 File Offset: 0x00287FD2
	// (set) Token: 0x0600644A RID: 25674 RVA: 0x00289DDC File Offset: 0x00287FDC
	public ItemValue ItemValue
	{
		get
		{
			return this.itemValue;
		}
		set
		{
			if (this.itemValue != value)
			{
				this.itemValue = value;
				this.itemStack.itemValue = this.itemValue;
				if (!this.itemStack.itemValue.IsEmpty())
				{
					this.itemStack.count = 1;
				}
				if (value.IsEmpty() && base.Selected)
				{
					base.Selected = false;
					if (this.InfoWindow != null)
					{
						this.InfoWindow.SetItemStack(null, true);
					}
				}
				if (this.SlotChangedEvent != null)
				{
					this.SlotChangedEvent(this.SlotNumber, this.itemStack);
				}
			}
			this.isDirty = true;
		}
	}

	// Token: 0x17000A3F RID: 2623
	// (get) Token: 0x0600644B RID: 25675 RVA: 0x00289E7D File Offset: 0x0028807D
	// (set) Token: 0x0600644C RID: 25676 RVA: 0x00289E85 File Offset: 0x00288085
	public ItemStack ItemStack
	{
		get
		{
			return this.itemStack;
		}
		set
		{
			if (this.itemStack != value)
			{
				this.itemStack = value.Clone();
				this.ItemValue = this.itemStack.itemValue.Clone();
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000A40 RID: 2624
	// (get) Token: 0x0600644D RID: 25677 RVA: 0x00289EB9 File Offset: 0x002880B9
	// (set) Token: 0x0600644E RID: 25678 RVA: 0x00289EC1 File Offset: 0x002880C1
	public XUiC_ItemInfoWindow InfoWindow { get; set; }

	// Token: 0x17000A41 RID: 2625
	// (get) Token: 0x0600644F RID: 25679 RVA: 0x00289ECA File Offset: 0x002880CA
	// (set) Token: 0x06006450 RID: 25680 RVA: 0x00289ED2 File Offset: 0x002880D2
	public XUiC_CharacterFrameWindow FrameWindow { get; set; }

	// Token: 0x06006451 RID: 25681 RVA: 0x00289EDB File Offset: 0x002880DB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SelectedChanged(bool isSelected)
	{
		this.SetBorderColor(isSelected ? this.selectedBorderColor : this.normalBorderColor);
	}

	// Token: 0x06006452 RID: 25682 RVA: 0x00289EF4 File Offset: 0x002880F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetBorderColor(Color32 color)
	{
		((XUiV_Sprite)this.background.ViewComponent).Color = color;
	}

	// Token: 0x06006453 RID: 25683 RVA: 0x00289F14 File Offset: 0x00288114
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetEmptySpriteNameAndTooltip()
	{
		switch (this.equipSlot)
		{
		case EquipmentSlots.Head:
			this.emptySpriteName = "apparelCowboyHat";
			this.emptyTooltipName = this.lblHeadgear;
			break;
		case EquipmentSlots.Chest:
			this.emptySpriteName = "armorSteelChest";
			this.emptyTooltipName = this.lblChestArmor;
			break;
		case EquipmentSlots.Hands:
			this.emptySpriteName = "armorLeatherGloves";
			this.emptyTooltipName = this.lblGloves;
			break;
		case EquipmentSlots.Feet:
			this.emptySpriteName = "apparelWornBoots";
			this.emptyTooltipName = this.lblFootwear;
			break;
		}
		if (this.emptyTooltipName != null)
		{
			this.emptyTooltipName = this.emptyTooltipName.ToUpper();
		}
	}

	// Token: 0x06006454 RID: 25684 RVA: 0x00289FBC File Offset: 0x002881BC
	public override void Init()
	{
		base.Init();
		this.stackValue = base.GetChildById("stackValue");
		this.background = base.GetChildById("background");
		this.SetBorderColor(this.normalBorderColor);
		this.itemIcon = base.GetChildById("itemIcon");
		this.durabilityBackground = base.GetChildById("durabilityBackground");
		this.durability = base.GetChildById("durability");
		this.tintedOverlay = base.GetChildById("tintedOverlay");
		this.highlightOverlay = base.GetChildById("highlightOverlay");
		this.lockTypeIcon = base.GetChildById("lockTypeIcon");
		this.overlay = base.GetChildById("overlay");
		this.itemStack.count = 1;
		this.tweenScale = this.itemIcon.ViewComponent.UiTransform.gameObject.AddComponent<TweenScale>();
		this.lblHeadgear = Localization.Get("lblHeadgear", false);
		this.lblChestArmor = Localization.Get("lblChestArmor", false);
		this.lblGloves = Localization.Get("lblGloves", false);
		this.lblFootwear = Localization.Get("lblFootwear", false);
		base.ViewComponent.UseSelectionBox = false;
	}

	// Token: 0x06006455 RID: 25685 RVA: 0x0028A0F0 File Offset: 0x002882F0
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (base.WindowGroup.isShowing)
		{
			PlayerActionsGUI guiactions = base.xui.playerUI.playerInput.GUIActions;
			CursorControllerAbs cursorController = base.xui.playerUI.CursorController;
			Vector3 a = cursorController.GetScreenPosition();
			bool mouseButtonUp = cursorController.GetMouseButtonUp(UICamera.MouseButton.LeftButton);
			bool mouseButtonDown = cursorController.GetMouseButtonDown(UICamera.MouseButton.LeftButton);
			bool mouseButton = cursorController.GetMouseButton(UICamera.MouseButton.LeftButton);
			if (this.isOver && UICamera.hoveredObject == base.ViewComponent.UiTransform.gameObject && base.ViewComponent.EventOnPress)
			{
				if (guiactions.LastInputType == BindingSourceType.DeviceBindingSource)
				{
					bool wasReleased = guiactions.Submit.WasReleased;
					bool wasPressed = guiactions.Inspect.WasPressed;
					bool wasReleased2 = guiactions.RightStick.WasReleased;
					if (base.xui.dragAndDrop.CurrentStack.IsEmpty() && !this.ItemStack.IsEmpty())
					{
						if (wasReleased)
						{
							this.SwapItem();
						}
						else if (wasReleased2)
						{
							this.HandleMoveToPreferredLocation();
							base.xui.PlayerEquipment.RefreshEquipment();
						}
						else if (wasPressed)
						{
							this.HandleItemInspect();
						}
						if (this.itemStack.IsEmpty())
						{
							((XUiV_Sprite)this.background.ViewComponent).Color = XUiC_EquipmentStack.backgroundColor;
						}
					}
					else if (wasReleased)
					{
						this.HandleStackSwap();
					}
				}
				else if (InputUtils.ShiftKeyPressed)
				{
					if (mouseButtonUp)
					{
						this.HandleMoveToPreferredLocation();
						base.xui.PlayerEquipment.RefreshEquipment();
					}
				}
				else if (mouseButton)
				{
					if (base.xui.dragAndDrop.CurrentStack.IsEmpty() && !this.ItemStack.IsEmpty())
					{
						if (!this.lastClicked)
						{
							this.startMousePos = a;
						}
						else if (Mathf.Abs((a - this.startMousePos).magnitude) > (float)this.PickupSnapDistance)
						{
							this.SwapItem();
							base.xui.PlayerEquipment.RefreshEquipment();
						}
						this.SetBorderColor(this.normalBorderColor);
					}
					if (mouseButtonDown)
					{
						this.lastClicked = true;
					}
				}
				else if (mouseButtonUp)
				{
					if (base.xui.dragAndDrop.CurrentStack.IsEmpty())
					{
						this.HandleItemInspect();
					}
					else if (this.lastClicked)
					{
						this.HandleStackSwap();
						base.xui.PlayerEquipment.RefreshEquipment();
					}
				}
				else
				{
					this.lastClicked = false;
				}
			}
			else
			{
				this.lastClicked = false;
				if (this.isOver || this.itemIcon.ViewComponent.UiTransform.localScale != Vector3.one)
				{
					if (this.tweenScale.value != Vector3.one && !this.itemStack.IsEmpty())
					{
						this.tweenScale.from = Vector3.one * 1.5f;
						this.tweenScale.to = Vector3.one;
						this.tweenScale.enabled = true;
						this.tweenScale.duration = 0.5f;
					}
					this.isOver = false;
				}
			}
		}
		if (this.isDirty)
		{
			bool flag = !this.itemValue.IsEmpty();
			ItemClass itemClass = null;
			if (flag)
			{
				itemClass = ItemClass.GetForId(this.itemValue.type);
			}
			if (this.itemIcon != null)
			{
				((XUiV_Sprite)this.itemIcon.ViewComponent).SpriteName = (flag ? this.itemStack.itemValue.GetPropertyOverride("CustomIcon", itemClass.GetIconName()) : this.emptySpriteName);
				((XUiV_Sprite)this.itemIcon.ViewComponent).UIAtlas = (flag ? "ItemIconAtlas" : "ItemIconAtlasGreyscale");
				((XUiV_Sprite)this.itemIcon.ViewComponent).Color = (flag ? Color.white : new Color(1f, 1f, 1f, 0.7f));
				string text = string.Empty;
				if (flag)
				{
					text = itemClass.GetLocalizedItemName();
				}
				base.ViewComponent.ToolTip = (flag ? text : this.emptyTooltipName);
			}
			if (itemClass != null)
			{
				((XUiV_Sprite)this.itemIcon.ViewComponent).Color = this.itemStack.itemValue.ItemClass.GetIconTint(this.itemStack.itemValue);
				if (itemClass.ShowQualityBar)
				{
					if (this.durability != null)
					{
						this.durability.ViewComponent.IsVisible = true;
						this.durabilityBackground.ViewComponent.IsVisible = true;
						XUiV_Sprite xuiV_Sprite = (XUiV_Sprite)this.durability.ViewComponent;
						xuiV_Sprite.Color = QualityInfo.GetQualityColor((int)this.itemValue.Quality);
						xuiV_Sprite.Fill = this.itemValue.PercentUsesLeft;
					}
					if (this.stackValue != null)
					{
						XUiV_Label xuiV_Label = (XUiV_Label)this.stackValue.ViewComponent;
						xuiV_Label.Alignment = NGUIText.Alignment.Center;
						xuiV_Label.Text = ((this.itemStack.itemValue.Quality > 0) ? this.itemStack.itemValue.Quality.ToString() : (this.itemStack.itemValue.IsMod ? "*" : ""));
					}
				}
				else if (this.durability != null)
				{
					this.durability.ViewComponent.IsVisible = false;
					this.durabilityBackground.ViewComponent.IsVisible = false;
				}
				if (this.lockTypeIcon != null)
				{
					if (this.itemStack.itemValue.HasMods())
					{
						(this.lockTypeIcon.ViewComponent as XUiV_Sprite).SpriteName = "ui_game_symbol_modded";
					}
					else
					{
						(this.lockTypeIcon.ViewComponent as XUiV_Sprite).SpriteName = "";
					}
				}
			}
			else
			{
				if (this.durability != null)
				{
					this.durability.ViewComponent.IsVisible = false;
				}
				if (this.durabilityBackground != null)
				{
					this.durabilityBackground.ViewComponent.IsVisible = false;
				}
				if (this.stackValue != null)
				{
					((XUiV_Label)this.stackValue.ViewComponent).Text = "";
				}
				if (this.lockTypeIcon != null)
				{
					(this.lockTypeIcon.ViewComponent as XUiV_Sprite).SpriteName = "";
				}
			}
			this.isDirty = false;
		}
		((XUiV_Label)this.stackValue.ViewComponent).Alignment = ((this.itemStack.itemValue.HasQuality || this.itemStack.itemValue.Modifications.Length != 0) ? NGUIText.Alignment.Center : NGUIText.Alignment.Right);
	}

	// Token: 0x06006456 RID: 25686 RVA: 0x0028A777 File Offset: 0x00288977
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleItemInspect()
	{
		if (!this.ItemStack.IsEmpty() && this.InfoWindow != null)
		{
			base.Selected = true;
			this.InfoWindow.SetItemStack(this, true);
		}
		this.HandleClickComplete();
	}

	// Token: 0x06006457 RID: 25687 RVA: 0x0028A7A8 File Offset: 0x002889A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleStackSwap()
	{
		ItemClass itemClass = base.xui.dragAndDrop.CurrentStack.itemValue.ItemClass;
		if (itemClass == null)
		{
			return;
		}
		ItemClassArmor itemClassArmor = itemClass as ItemClassArmor;
		if (itemClassArmor != null && itemClassArmor.EquipSlot == this.EquipSlot)
		{
			this.SwapItem();
		}
		base.Selected = false;
		this.HandleClickComplete();
	}

	// Token: 0x06006458 RID: 25688 RVA: 0x0028A7FF File Offset: 0x002889FF
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleClickComplete()
	{
		this.lastClicked = false;
		if (this.itemValue.IsEmpty())
		{
			this.SetBorderColor(this.normalBorderColor);
		}
	}

	// Token: 0x06006459 RID: 25689 RVA: 0x0028A824 File Offset: 0x00288A24
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		this.isOver = _isOver;
		if (_isOver)
		{
			if (!base.Selected)
			{
				this.SetBorderColor(this.hoverBorderColor);
			}
			if (!this.itemStack.IsEmpty())
			{
				this.tweenScale.from = Vector3.one;
				this.tweenScale.to = Vector3.one * 1.5f;
				this.tweenScale.enabled = true;
				this.tweenScale.duration = 0.5f;
			}
		}
		else
		{
			if (!base.Selected)
			{
				this.SetBorderColor(this.normalBorderColor);
			}
			this.tweenScale.from = Vector3.one * 1.5f;
			this.tweenScale.to = Vector3.one;
			this.tweenScale.enabled = true;
			this.tweenScale.duration = 0.5f;
		}
		bool canSwap = false;
		ItemStack currentStack = base.xui.dragAndDrop.CurrentStack;
		ItemClassArmor itemClassArmor = (currentStack.IsEmpty() ? null : currentStack.itemValue.ItemClass) as ItemClassArmor;
		if (itemClassArmor != null)
		{
			canSwap = (this.equipSlot == itemClassArmor.EquipSlot);
		}
		base.xui.calloutWindow.UpdateCalloutsForItemStack(base.ViewComponent.UiTransform.gameObject, this.ItemStack, this.isOver, canSwap, true, true);
		if (!_isOver && this.tweenScale.value != Vector3.one && !this.itemStack.IsEmpty())
		{
			this.tweenScale.from = Vector3.one * 1.5f;
			this.tweenScale.to = Vector3.one;
			this.tweenScale.enabled = true;
			this.tweenScale.duration = 0.5f;
		}
		base.OnHovered(_isOver);
	}

	// Token: 0x0600645A RID: 25690 RVA: 0x0028A9E8 File Offset: 0x00288BE8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SwapItem()
	{
		ItemStack currentStack = base.xui.dragAndDrop.CurrentStack;
		if (this.AllowRemoveItem)
		{
			base.xui.dragAndDrop.CurrentStack = this.itemStack.Clone();
			base.xui.dragAndDrop.PickUpType = XUiC_ItemStack.StackLocationTypes.Equipment;
		}
		else
		{
			if (currentStack.IsEmpty())
			{
				return;
			}
			if (!this.itemStack.IsEmpty())
			{
				ItemClassArmor itemClassArmor = this.itemStack.itemValue.ItemClass as ItemClassArmor;
				if (itemClassArmor != null && itemClassArmor.ReplaceByTag != null)
				{
					FastTags<TagGroup.Global> fastTags = FastTags<TagGroup.Global>.Parse(itemClassArmor.ReplaceByTag);
					ItemClassArmor itemClassArmor2 = currentStack.itemValue.ItemClass as ItemClassArmor;
					if (itemClassArmor2 != null && !fastTags.Test_AnySet(itemClassArmor2.ItemTags))
					{
						return;
					}
				}
			}
			base.xui.dragAndDrop.CurrentStack = ItemStack.Empty.Clone();
		}
		if (this.itemStack.IsEmpty())
		{
			if (this.placeSound != null)
			{
				Manager.PlayXUiSound(this.placeSound, 0.75f);
			}
		}
		else if (this.pickupSound != null)
		{
			Manager.PlayXUiSound(this.pickupSound, 0.75f);
		}
		this.ItemStack = currentStack.Clone();
		if (this.SlotChangedEvent != null)
		{
			this.SlotChangedEvent(this.SlotNumber, this.itemStack);
		}
	}

	// Token: 0x0600645B RID: 25691 RVA: 0x0028AB38 File Offset: 0x00288D38
	public void HandleMoveToPreferredLocation()
	{
		if (!this.AllowRemoveItem)
		{
			return;
		}
		ItemStack itemStack = this.ItemStack.Clone();
		if (base.xui.PlayerInventory.AddItemToBackpack(itemStack))
		{
			this.ItemValue = ItemStack.Empty.itemValue.Clone();
			if (this.placeSound != null)
			{
				Manager.PlayXUiSound(this.placeSound, 0.75f);
			}
			if (this.itemStack.IsEmpty() && base.Selected)
			{
				base.Selected = false;
				return;
			}
		}
		else if (base.xui.PlayerInventory.AddItemToToolbelt(itemStack))
		{
			this.ItemValue = ItemStack.Empty.itemValue.Clone();
			if (this.placeSound != null)
			{
				Manager.PlayXUiSound(this.placeSound, 0.75f);
			}
			if (this.SlotChangedEvent != null)
			{
				this.SlotChangedEvent(this.SlotNumber, this.itemStack);
			}
			if (this.itemStack.IsEmpty() && base.Selected)
			{
				base.Selected = false;
			}
		}
	}

	// Token: 0x0600645C RID: 25692 RVA: 0x0028AC44 File Offset: 0x00288E44
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(name, value, _parent);
		if (!flag)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
			if (num <= 1352047028U)
			{
				if (num <= 605893325U)
				{
					if (num != 507753414U)
					{
						if (num == 605893325U)
						{
							if (name == "pickup_sound")
							{
								base.xui.LoadData<AudioClip>(value, delegate(AudioClip o)
								{
									this.pickupSound = o;
								});
								return true;
							}
						}
					}
					else if (name == "normal_color")
					{
						this.normalBackgroundColor = StringParsers.ParseColor32(value);
						return true;
					}
				}
				else if (num != 783618599U)
				{
					if (num == 1352047028U)
					{
						if (name == "hover_border_color")
						{
							this.hoverBorderColor = StringParsers.ParseColor32(value);
							return true;
						}
					}
				}
				else if (name == "hover_icon_grow")
				{
					this.HoverIconGrow = StringParsers.ParseFloat(value, 0, -1, NumberStyles.Any);
					return true;
				}
			}
			else if (num <= 1619874765U)
			{
				if (num != 1563791203U)
				{
					if (num == 1619874765U)
					{
						if (name == "normal_border_color")
						{
							this.normalBorderColor = StringParsers.ParseColor32(value);
							return true;
						}
					}
				}
				else if (name == "allow_remove_item")
				{
					this.AllowRemoveItem = StringParsers.ParseBool(value, 0, -1, true);
					return true;
				}
			}
			else if (num != 1800731603U)
			{
				if (num != 3765930259U)
				{
					if (num == 3919060864U)
					{
						if (name == "place_sound")
						{
							base.xui.LoadData<AudioClip>(value, delegate(AudioClip o)
							{
								this.placeSound = o;
							});
							return true;
						}
					}
				}
				else if (name == "selected_border_color")
				{
					this.selectedBorderColor = StringParsers.ParseColor32(value);
					return true;
				}
			}
			else if (name == "normal_background_color")
			{
				XUiC_EquipmentStack.finalPressedColor = StringParsers.ParseColor32(value);
				return true;
			}
			return false;
		}
		return flag;
	}

	// Token: 0x0600645D RID: 25693 RVA: 0x0028AE6B File Offset: 0x0028906B
	public override void OnOpen()
	{
		base.OnOpen();
		this.isDirty = true;
	}

	// Token: 0x04004B81 RID: 19329
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack itemStack = ItemStack.Empty.Clone();

	// Token: 0x04004B82 RID: 19330
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue itemValue = new ItemValue();

	// Token: 0x04004B83 RID: 19331
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty = true;

	// Token: 0x04004B84 RID: 19332
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bHighlightEnabled;

	// Token: 0x04004B85 RID: 19333
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bDropEnabled = true;

	// Token: 0x04004B86 RID: 19334
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bDisabled;

	// Token: 0x04004B87 RID: 19335
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOver;

	// Token: 0x04004B88 RID: 19336
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 selectedBorderColor = new Color32(222, 206, 163, byte.MaxValue);

	// Token: 0x04004B89 RID: 19337
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 hoverBorderColor = new Color32(182, 166, 123, byte.MaxValue);

	// Token: 0x04004B8A RID: 19338
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 normalBorderColor = new Color32(96, 96, 96, byte.MaxValue);

	// Token: 0x04004B8B RID: 19339
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 normalBackgroundColor = new Color32(96, 96, 96, 96);

	// Token: 0x04004B8C RID: 19340
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lastClicked;

	// Token: 0x04004B8D RID: 19341
	[PublicizedFrom(EAccessModifier.Private)]
	public string emptySpriteName = "";

	// Token: 0x04004B8E RID: 19342
	[PublicizedFrom(EAccessModifier.Private)]
	public string emptyTooltipName = "";

	// Token: 0x04004B8F RID: 19343
	[PublicizedFrom(EAccessModifier.Private)]
	public AudioClip pickupSound;

	// Token: 0x04004B90 RID: 19344
	[PublicizedFrom(EAccessModifier.Private)]
	public AudioClip placeSound;

	// Token: 0x04004B91 RID: 19345
	[PublicizedFrom(EAccessModifier.Private)]
	public EquipmentSlots equipSlot;

	// Token: 0x04004B93 RID: 19347
	public int PickupSnapDistance = 4;

	// Token: 0x04004B94 RID: 19348
	public static Color32 finalPressedColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	// Token: 0x04004B95 RID: 19349
	public static Color32 backgroundColor = new Color32(96, 96, 96, byte.MaxValue);

	// Token: 0x04004B96 RID: 19350
	public static Color32 highlightColor = new Color32(222, 206, 163, byte.MaxValue);

	// Token: 0x04004B97 RID: 19351
	public Color32 holdingColor = new Color32(byte.MaxValue, 128, 0, byte.MaxValue);

	// Token: 0x04004B98 RID: 19352
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController timer;

	// Token: 0x04004B99 RID: 19353
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController stackValue;

	// Token: 0x04004B9A RID: 19354
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController itemIcon;

	// Token: 0x04004B9B RID: 19355
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController durability;

	// Token: 0x04004B9C RID: 19356
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController durabilityBackground;

	// Token: 0x04004B9D RID: 19357
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController lockTypeIcon;

	// Token: 0x04004B9E RID: 19358
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController tintedOverlay;

	// Token: 0x04004B9F RID: 19359
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController highlightOverlay;

	// Token: 0x04004BA0 RID: 19360
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController overlay;

	// Token: 0x04004BA1 RID: 19361
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController background;

	// Token: 0x04004BA4 RID: 19364
	public bool AllowRemoveItem = true;

	// Token: 0x04004BA5 RID: 19365
	public string AllowedItems;

	// Token: 0x04004BA8 RID: 19368
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblHeadgear;

	// Token: 0x04004BA9 RID: 19369
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblChestArmor;

	// Token: 0x04004BAA RID: 19370
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblGloves;

	// Token: 0x04004BAB RID: 19371
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblFootwear;

	// Token: 0x04004BAC RID: 19372
	[PublicizedFrom(EAccessModifier.Private)]
	public TweenScale tweenScale;

	// Token: 0x04004BAD RID: 19373
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 startMousePos = Vector3.zero;
}
