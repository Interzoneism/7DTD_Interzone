using System;
using System.Globalization;
using Audio;
using InControl;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C0F RID: 3087
[UnityEngine.Scripting.Preserve]
public class XUiC_BasePartStack : XUiC_SelectableEntry
{
	// Token: 0x170009C5 RID: 2501
	// (get) Token: 0x06005EA4 RID: 24228 RVA: 0x00265E44 File Offset: 0x00264044
	// (set) Token: 0x06005EA5 RID: 24229 RVA: 0x00265E4C File Offset: 0x0026404C
	public int SlotNumber { get; set; }

	// Token: 0x170009C6 RID: 2502
	// (get) Token: 0x06005EA6 RID: 24230 RVA: 0x00265E55 File Offset: 0x00264055
	// (set) Token: 0x06005EA7 RID: 24231 RVA: 0x00265E5D File Offset: 0x0026405D
	public XUiC_ItemStack.StackLocationTypes StackLocation { get; set; }

	// Token: 0x14000091 RID: 145
	// (add) Token: 0x06005EA8 RID: 24232 RVA: 0x00265E68 File Offset: 0x00264068
	// (remove) Token: 0x06005EA9 RID: 24233 RVA: 0x00265EA0 File Offset: 0x002640A0
	public event XUiEvent_SlotChangedEventHandler SlotChangingEvent;

	// Token: 0x14000092 RID: 146
	// (add) Token: 0x06005EAA RID: 24234 RVA: 0x00265ED8 File Offset: 0x002640D8
	// (remove) Token: 0x06005EAB RID: 24235 RVA: 0x00265F10 File Offset: 0x00264110
	public event XUiEvent_SlotChangedEventHandler SlotChangedEvent;

	// Token: 0x170009C7 RID: 2503
	// (get) Token: 0x06005EAC RID: 24236 RVA: 0x00265F45 File Offset: 0x00264145
	// (set) Token: 0x06005EAD RID: 24237 RVA: 0x00265F4D File Offset: 0x0026414D
	public float HoverIconGrow { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x170009C8 RID: 2504
	// (get) Token: 0x06005EAE RID: 24238 RVA: 0x00265F56 File Offset: 0x00264156
	// (set) Token: 0x06005EAF RID: 24239 RVA: 0x00265F5E File Offset: 0x0026415E
	public string SlotType
	{
		get
		{
			return this.slotType;
		}
		set
		{
			this.slotType = value;
			this.SetEmptySpriteName();
		}
	}

	// Token: 0x170009C9 RID: 2505
	// (get) Token: 0x06005EB0 RID: 24240 RVA: 0x00265F6D File Offset: 0x0026416D
	// (set) Token: 0x06005EB1 RID: 24241 RVA: 0x00265F78 File Offset: 0x00264178
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
				this.itemStack = value;
				this.isDirty = true;
				if (this.itemStack.IsEmpty())
				{
					base.Selected = false;
				}
				if (base.Selected)
				{
					this.InfoWindow.SetItemStack(this, true);
				}
				if (this.SlotChangedEvent != null)
				{
					this.SlotChangedEvent(this.SlotNumber, this.itemStack);
				}
				this.itemClass = this.itemStack.itemValue.ItemClass;
				base.RefreshBindings(false);
			}
		}
	}

	// Token: 0x170009CA RID: 2506
	// (get) Token: 0x06005EB2 RID: 24242 RVA: 0x00266001 File Offset: 0x00264201
	public ItemClass ItemClass
	{
		get
		{
			return this.itemClass;
		}
	}

	// Token: 0x170009CB RID: 2507
	// (get) Token: 0x06005EB3 RID: 24243 RVA: 0x00266009 File Offset: 0x00264209
	// (set) Token: 0x06005EB4 RID: 24244 RVA: 0x00266011 File Offset: 0x00264211
	public XUiC_ItemInfoWindow InfoWindow { get; set; }

	// Token: 0x06005EB5 RID: 24245 RVA: 0x0026601A File Offset: 0x0026421A
	public virtual string GetAtlas()
	{
		if (this.itemClass == null)
		{
			return "ItemIconAtlasGreyscale";
		}
		return "ItemIconAtlas";
	}

	// Token: 0x06005EB6 RID: 24246 RVA: 0x0026602F File Offset: 0x0026422F
	public virtual string GetPartName()
	{
		if (this.itemClass == null)
		{
			return string.Format("[MISSING {0}]", this.SlotType);
		}
		return this.itemClass.GetLocalizedItemName();
	}

	// Token: 0x06005EB7 RID: 24247 RVA: 0x00266058 File Offset: 0x00264258
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 2285454806U)
		{
			if (num <= 776984821U)
			{
				if (num != 289581667U)
				{
					if (num == 776984821U)
					{
						if (bindingName == "partquality")
						{
							value = ((this.itemClass != null && this.itemStack != null) ? this.qualityFormatter.Format((int)this.itemStack.itemValue.Quality) : "");
							return true;
						}
					}
				}
				else if (bindingName == "partatlas")
				{
					value = this.GetAtlas();
					return true;
				}
			}
			else if (num != 1419324424U)
			{
				if (num == 2285454806U)
				{
					if (bindingName == "partvisible")
					{
						value = ((this.itemClass != null) ? "true" : "false");
						return true;
					}
				}
			}
			else if (bindingName == "particoncolor")
			{
				if (this.itemClass == null)
				{
					value = "255, 255, 255, 178";
				}
				else
				{
					Color32 color = this.itemStack.itemValue.ItemClass.GetIconTint(this.itemStack.itemValue);
					value = string.Format("{0},{1},{2},{3}", new object[]
					{
						color.r,
						color.g,
						color.b,
						color.a
					});
				}
				return true;
			}
		}
		else if (num <= 2552573645U)
		{
			if (num != 2498838587U)
			{
				if (num == 2552573645U)
				{
					if (bindingName == "partfill")
					{
						value = ((this.itemStack.itemValue.MaxUseTimes == 0) ? "1" : this.partfillFormatter.Format(((float)this.itemStack.itemValue.MaxUseTimes - this.itemStack.itemValue.UseTimes) / (float)this.itemStack.itemValue.MaxUseTimes));
						return true;
					}
				}
			}
			else if (bindingName == "partcolor")
			{
				if (this.itemClass != null)
				{
					Color32 v = QualityInfo.GetQualityColor((int)this.itemStack.itemValue.Quality);
					value = this.partcolorFormatter.Format(v);
				}
				else
				{
					value = "255, 255, 255, 0";
				}
				return true;
			}
		}
		else if (num != 2733906447U)
		{
			if (num != 3045999413U)
			{
				if (num == 3130438080U)
				{
					if (bindingName == "emptyvisible")
					{
						value = ((this.itemClass == null) ? "true" : "false");
						return true;
					}
				}
			}
			else if (bindingName == "particon")
			{
				if (this.itemClass == null)
				{
					value = this.emptySpriteName;
				}
				else
				{
					value = this.itemStack.itemValue.GetPropertyOverride("CustomIcon", (this.itemClass.CustomIcon != null) ? this.itemClass.CustomIcon.Value : this.itemClass.GetIconName());
				}
				return true;
			}
		}
		else if (bindingName == "partname")
		{
			value = this.GetPartName();
			return true;
		}
		return false;
	}

	// Token: 0x06005EB8 RID: 24248 RVA: 0x002663A1 File Offset: 0x002645A1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SelectedChanged(bool isSelected)
	{
		this.SetColor(isSelected ? this.selectColor : XUiC_BasePartStack.backgroundColor);
		((XUiV_Sprite)this.background.ViewComponent).SpriteName = (isSelected ? "ui_game_select_row" : "menu_empty");
	}

	// Token: 0x06005EB9 RID: 24249 RVA: 0x002663DD File Offset: 0x002645DD
	[PublicizedFrom(EAccessModifier.Protected)]
	public void SetColor(Color32 color)
	{
		((XUiV_Sprite)this.background.ViewComponent).Color = color;
	}

	// Token: 0x06005EBA RID: 24250 RVA: 0x002663FA File Offset: 0x002645FA
	public override void Init()
	{
		base.Init();
		this.itemIcon = base.GetChildById("itemIcon");
		this.background = base.GetChildById("background");
		base.RefreshBindings(false);
	}

	// Token: 0x06005EBB RID: 24251 RVA: 0x0026642C File Offset: 0x0026462C
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
					if (base.xui.dragAndDrop.CurrentStack.IsEmpty())
					{
						if (!this.ItemStack.IsEmpty())
						{
							if (guiactions.Submit.WasReleased && this.CanRemove())
							{
								this.SwapItem();
								this.currentColor = XUiC_BasePartStack.backgroundColor;
								if (this.itemStack.IsEmpty())
								{
									((XUiV_Sprite)this.background.ViewComponent).Color = XUiC_BasePartStack.backgroundColor;
									return;
								}
							}
							else
							{
								if (guiactions.RightStick.WasReleased)
								{
									this.HandleMoveToPreferredLocation();
									return;
								}
								if (guiactions.Inspect.WasReleased)
								{
									this.HandleItemInspect();
									return;
								}
							}
						}
					}
					else if (guiactions.Submit.WasReleased)
					{
						this.HandleStackSwap();
						return;
					}
				}
				else if (InputUtils.ShiftKeyPressed)
				{
					if (mouseButtonUp)
					{
						this.HandleMoveToPreferredLocation();
						return;
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
						else if (this.CanRemove() && Mathf.Abs((a - this.startMousePos).magnitude) > (float)this.PickupSnapDistance)
						{
							this.SwapItem();
							this.currentColor = XUiC_BasePartStack.backgroundColor;
							if (this.itemStack.IsEmpty())
							{
								((XUiV_Sprite)this.background.ViewComponent).Color = XUiC_BasePartStack.backgroundColor;
							}
						}
					}
					if (mouseButtonDown)
					{
						this.lastClicked = true;
						return;
					}
				}
				else
				{
					if (!mouseButtonUp)
					{
						this.lastClicked = false;
						return;
					}
					if (base.xui.dragAndDrop.CurrentStack.IsEmpty())
					{
						this.HandleItemInspect();
						return;
					}
					if (this.lastClicked)
					{
						this.HandleStackSwap();
						return;
					}
				}
			}
			else
			{
				this.currentColor = XUiC_BasePartStack.backgroundColor;
				if (!base.Selected)
				{
					((XUiV_Sprite)this.background.ViewComponent).Color = this.currentColor;
				}
				this.lastClicked = false;
			}
		}
	}

	// Token: 0x06005EBC RID: 24252 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SetEmptySpriteName()
	{
	}

	// Token: 0x06005EBD RID: 24253 RVA: 0x002666DA File Offset: 0x002648DA
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleItemInspect()
	{
		if (!this.ItemStack.IsEmpty() && this.InfoWindow != null)
		{
			base.Selected = true;
			this.InfoWindow.SetItemStack(this, true);
		}
		this.HandleClickComplete();
	}

	// Token: 0x06005EBE RID: 24254 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool CanSwap(ItemStack stack)
	{
		return false;
	}

	// Token: 0x06005EBF RID: 24255 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool CanRemove()
	{
		return true;
	}

	// Token: 0x06005EC0 RID: 24256 RVA: 0x0026670C File Offset: 0x0026490C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleStackSwap()
	{
		ItemStack currentStack = base.xui.dragAndDrop.CurrentStack;
		if (!currentStack.IsEmpty())
		{
			if (this.CanSwap(currentStack))
			{
				this.SwapItem();
			}
			else
			{
				Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
			}
		}
		else if (this.CanRemove())
		{
			this.SwapItem();
		}
		else
		{
			Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
		}
		base.Selected = false;
		this.HandleClickComplete();
	}

	// Token: 0x06005EC1 RID: 24257 RVA: 0x00266786 File Offset: 0x00264986
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleClickComplete()
	{
		this.lastClicked = false;
		this.currentColor = XUiC_BasePartStack.backgroundColor;
		if (this.itemStack.IsEmpty())
		{
			((XUiV_Sprite)this.background.ViewComponent).Color = XUiC_BasePartStack.backgroundColor;
		}
	}

	// Token: 0x06005EC2 RID: 24258 RVA: 0x002667C8 File Offset: 0x002649C8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		this.isOver = _isOver;
		if (!base.Selected)
		{
			if (_isOver)
			{
				((XUiV_Sprite)this.background.ViewComponent).Color = XUiC_BasePartStack.highlightColor;
			}
			else
			{
				((XUiV_Sprite)this.background.ViewComponent).Color = XUiC_BasePartStack.backgroundColor;
			}
		}
		ItemStack currentStack = base.xui.dragAndDrop.CurrentStack;
		bool canSwap = !currentStack.IsEmpty() && this.CanSwap(currentStack);
		base.xui.calloutWindow.UpdateCalloutsForItemStack(base.ViewComponent.UiTransform.gameObject, this.ItemStack, this.isOver, canSwap, this.CanRemove(), true);
		base.OnHovered(_isOver);
	}

	// Token: 0x06005EC3 RID: 24259 RVA: 0x00266888 File Offset: 0x00264A88
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SwapItem()
	{
		ItemStack currentStack = base.xui.dragAndDrop.CurrentStack;
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
		if (this.SlotChangingEvent != null)
		{
			this.SlotChangingEvent(this.SlotNumber, this.itemStack);
		}
		base.xui.dragAndDrop.CurrentStack = this.itemStack.Clone();
		base.xui.dragAndDrop.PickUpType = this.StackLocation;
		this.ItemStack = currentStack.Clone();
	}

	// Token: 0x06005EC4 RID: 24260 RVA: 0x00266950 File Offset: 0x00264B50
	public void HandleMoveToPreferredLocation()
	{
		if (this.ItemStack.IsEmpty() || !this.CanRemove())
		{
			return;
		}
		if (this.SlotChangingEvent != null)
		{
			this.SlotChangingEvent(this.SlotNumber, this.ItemStack);
		}
		if (base.xui.PlayerInventory.AddItemToBackpack(this.ItemStack))
		{
			if (base.xui.currentSelectedEntry == this)
			{
				base.xui.currentSelectedEntry.Selected = false;
				this.InfoWindow.SetItemStack(null, false);
			}
			this.ItemStack = ItemStack.Empty.Clone();
			if (this.placeSound != null)
			{
				Manager.PlayXUiSound(this.placeSound, 0.75f);
			}
			if (this.SlotChangedEvent != null)
			{
				this.SlotChangedEvent(this.SlotNumber, this.itemStack);
				return;
			}
		}
		else if (base.xui.PlayerInventory.AddItemToToolbelt(this.ItemStack))
		{
			if (base.xui.currentSelectedEntry == this)
			{
				base.xui.currentSelectedEntry.Selected = false;
				this.InfoWindow.SetItemStack(null, false);
			}
			this.ItemStack = ItemStack.Empty.Clone();
			if (this.placeSound != null)
			{
				Manager.PlayXUiSound(this.placeSound, 0.75f);
			}
			if (this.SlotChangedEvent != null)
			{
				this.SlotChangedEvent(this.SlotNumber, this.itemStack);
			}
		}
	}

	// Token: 0x06005EC5 RID: 24261 RVA: 0x00266AB8 File Offset: 0x00264CB8
	public void ClearSelectedInfoWindow()
	{
		if (base.Selected)
		{
			this.InfoWindow.SetItemStack(null, true);
		}
	}

	// Token: 0x06005EC6 RID: 24262 RVA: 0x00266AD0 File Offset: 0x00264CD0
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(name, value, _parent);
		if (!flag)
		{
			if (!(name == "background_color"))
			{
				if (!(name == "highlight_color"))
				{
					if (!(name == "pickup_snap_distance"))
					{
						if (!(name == "hover_icon_grow"))
						{
							if (!(name == "pickup_sound"))
							{
								if (!(name == "place_sound"))
								{
									return false;
								}
								base.xui.LoadData<AudioClip>(value, delegate(AudioClip o)
								{
									this.placeSound = o;
								});
							}
							else
							{
								base.xui.LoadData<AudioClip>(value, delegate(AudioClip o)
								{
									this.pickupSound = o;
								});
							}
						}
						else
						{
							this.HoverIconGrow = StringParsers.ParseFloat(value, 0, -1, NumberStyles.Any);
						}
					}
					else
					{
						this.PickupSnapDistance = int.Parse(value);
					}
				}
				else
				{
					XUiC_BasePartStack.highlightColor = StringParsers.ParseColor32(value);
				}
			}
			else
			{
				XUiC_BasePartStack.backgroundColor = StringParsers.ParseColor32(value);
			}
			return true;
		}
		return flag;
	}

	// Token: 0x04004757 RID: 18263
	[PublicizedFrom(EAccessModifier.Protected)]
	public ItemStack itemStack = ItemStack.Empty.Clone();

	// Token: 0x04004758 RID: 18264
	[PublicizedFrom(EAccessModifier.Protected)]
	public ItemClass itemClass;

	// Token: 0x04004759 RID: 18265
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isDirty = true;

	// Token: 0x0400475A RID: 18266
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bHighlightEnabled;

	// Token: 0x0400475B RID: 18267
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bDropEnabled = true;

	// Token: 0x0400475C RID: 18268
	[PublicizedFrom(EAccessModifier.Protected)]
	public AudioClip pickupSound;

	// Token: 0x0400475D RID: 18269
	[PublicizedFrom(EAccessModifier.Protected)]
	public AudioClip placeSound;

	// Token: 0x0400475E RID: 18270
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isOver;

	// Token: 0x0400475F RID: 18271
	[PublicizedFrom(EAccessModifier.Protected)]
	public string slotType;

	// Token: 0x04004760 RID: 18272
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 currentColor;

	// Token: 0x04004761 RID: 18273
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 selectColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	// Token: 0x04004762 RID: 18274
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 pressColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	// Token: 0x04004763 RID: 18275
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool lastClicked;

	// Token: 0x04004766 RID: 18278
	public int PickupSnapDistance = 4;

	// Token: 0x04004767 RID: 18279
	public static Color32 finalPressedColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	// Token: 0x04004768 RID: 18280
	public static Color32 backgroundColor = new Color32(96, 96, 96, byte.MaxValue);

	// Token: 0x04004769 RID: 18281
	public static Color32 highlightColor = new Color32(128, 128, 128, byte.MaxValue);

	// Token: 0x0400476A RID: 18282
	public Color32 holdingColor = new Color32(byte.MaxValue, 128, 0, byte.MaxValue);

	// Token: 0x0400476B RID: 18283
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController stackValue;

	// Token: 0x0400476C RID: 18284
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController itemIcon;

	// Token: 0x0400476D RID: 18285
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController durability;

	// Token: 0x0400476E RID: 18286
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController durabilityBackground;

	// Token: 0x0400476F RID: 18287
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController tintedOverlay;

	// Token: 0x04004770 RID: 18288
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController highlightOverlay;

	// Token: 0x04004771 RID: 18289
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController background;

	// Token: 0x04004772 RID: 18290
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController lblItemName;

	// Token: 0x04004776 RID: 18294
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt qualityFormatter = new CachedStringFormatterInt();

	// Token: 0x04004777 RID: 18295
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor partcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04004778 RID: 18296
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat partfillFormatter = new CachedStringFormatterFloat(null);

	// Token: 0x04004779 RID: 18297
	[PublicizedFrom(EAccessModifier.Protected)]
	public string emptySpriteName = "";

	// Token: 0x0400477B RID: 18299
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 startMousePos = Vector3.zero;
}
