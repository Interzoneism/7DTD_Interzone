using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C2A RID: 3114
[Preserve]
public class XUiC_CharacterCosmeticEntry : XUiController
{
	// Token: 0x170009E0 RID: 2528
	// (get) Token: 0x06005FC8 RID: 24520 RVA: 0x0026DB42 File Offset: 0x0026BD42
	// (set) Token: 0x06005FC9 RID: 24521 RVA: 0x0026DB4A File Offset: 0x0026BD4A
	public ItemClass ItemClass
	{
		get
		{
			return this.itemClass;
		}
		set
		{
			this.itemClass = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x170009E1 RID: 2529
	// (get) Token: 0x06005FCA RID: 24522 RVA: 0x0026DB5A File Offset: 0x0026BD5A
	// (set) Token: 0x06005FCB RID: 24523 RVA: 0x0026DB62 File Offset: 0x0026BD62
	public XUiC_CharacterCosmeticEntry.EntryTypes EntryType
	{
		get
		{
			return this.entryType;
		}
		set
		{
			this.entryType = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x170009E2 RID: 2530
	// (get) Token: 0x06005FCC RID: 24524 RVA: 0x0026DB72 File Offset: 0x0026BD72
	// (set) Token: 0x06005FCD RID: 24525 RVA: 0x0026DB7A File Offset: 0x0026BD7A
	public int Index
	{
		get
		{
			return this.index;
		}
		set
		{
			this.index = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x170009E3 RID: 2531
	// (get) Token: 0x06005FCE RID: 24526 RVA: 0x0026DB8A File Offset: 0x0026BD8A
	// (set) Token: 0x06005FCF RID: 24527 RVA: 0x0026DB92 File Offset: 0x0026BD92
	public new bool Selected
	{
		get
		{
			return this.selected;
		}
		set
		{
			this.selected = value;
		}
	}

	// Token: 0x06005FD0 RID: 24528 RVA: 0x0026DB9C File Offset: 0x0026BD9C
	public override void Init()
	{
		base.Init();
		this.IsDirty = true;
		XUiController childById = base.GetChildById("itemIcon");
		if (childById != null)
		{
			this.itemIconSprite = (childById.ViewComponent as XUiV_Sprite);
		}
		XUiController childById2 = base.GetChildById("backgroundTexture");
		if (childById2 != null)
		{
			this.backgroundTexture = (childById2.ViewComponent as XUiV_Texture);
			if (this.backgroundTexture != null)
			{
				this.backgroundTexture.CreateMaterial();
			}
		}
	}

	// Token: 0x06005FD1 RID: 24529 RVA: 0x0026DC09 File Offset: 0x0026BE09
	public override void OnOpen()
	{
		base.OnOpen();
		base.xui.PlayerEquipment.Equipment.CosmeticUnlocked += this.Equipment_CosmeticUnlocked;
	}

	// Token: 0x06005FD2 RID: 24530 RVA: 0x0026DC32 File Offset: 0x0026BE32
	public override void OnClose()
	{
		base.OnClose();
		base.xui.PlayerEquipment.Equipment.CosmeticUnlocked -= this.Equipment_CosmeticUnlocked;
	}

	// Token: 0x06005FD3 RID: 24531 RVA: 0x0026DC5C File Offset: 0x0026BE5C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Equipment_CosmeticUnlocked(string itemName)
	{
		if (this.entryType == XUiC_CharacterCosmeticEntry.EntryTypes.Item)
		{
			if (this.ItemClass == null)
			{
				return;
			}
			if (this.ItemClass.GetItemName() == itemName)
			{
				if (this.Owner.SelectedEntry == this)
				{
					this.Owner.Owner.RefreshBindings(false);
				}
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x170009E4 RID: 2532
	// (get) Token: 0x06005FD4 RID: 24532 RVA: 0x0026DCB3 File Offset: 0x0026BEB3
	// (set) Token: 0x06005FD5 RID: 24533 RVA: 0x0026DCBB File Offset: 0x0026BEBB
	public Color32 SelectionBorderColor
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.selectionBorderColor;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			if (!this.selectionBorderColor.ColorEquals(value))
			{
				this.selectionBorderColor = value;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x170009E5 RID: 2533
	// (get) Token: 0x06005FD6 RID: 24534 RVA: 0x0026DCD9 File Offset: 0x0026BED9
	// (set) Token: 0x06005FD7 RID: 24535 RVA: 0x0026DCE1 File Offset: 0x0026BEE1
	public bool IsDragAndDrop
	{
		get
		{
			return this.isDragAndDrop;
		}
		set
		{
			this.isDragAndDrop = value;
			if (!value)
			{
				return;
			}
			base.ViewComponent.EventOnPress = false;
			base.ViewComponent.EventOnHover = false;
		}
	}

	// Token: 0x06005FD8 RID: 24536 RVA: 0x0026DD08 File Offset: 0x0026BF08
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateBorderColor()
	{
		if (this.IsDragAndDrop)
		{
			this.SelectionBorderColor = Color.clear;
			return;
		}
		if (this.Selected)
		{
			this.SelectionBorderColor = this.selectColor;
			return;
		}
		if (this.isHovered)
		{
			this.SelectionBorderColor = this.highlightColor;
			return;
		}
		this.SelectionBorderColor = this.backgroundColor;
	}

	// Token: 0x06005FD9 RID: 24537 RVA: 0x0026DD64 File Offset: 0x0026BF64
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		base.OnHovered(_isOver);
		if (this.EntryType == XUiC_CharacterCosmeticEntry.EntryTypes.Item && this.itemClass == null)
		{
			this.isHovered = false;
			return;
		}
		if (this.isHovered != _isOver)
		{
			this.isHovered = _isOver;
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06005FDA RID: 24538 RVA: 0x0026DD9C File Offset: 0x0026BF9C
	public override void Update(float _dt)
	{
		if (this.IsDirty)
		{
			base.ViewComponent.IsNavigatable = (base.ViewComponent.IsSnappable = (this.entryType != XUiC_CharacterCosmeticEntry.EntryTypes.Item || (this.entryType == XUiC_CharacterCosmeticEntry.EntryTypes.Item && this.itemClass != null)));
			base.RefreshBindings(true);
			this.IsDirty = false;
		}
		this.updateBorderColor();
		base.Update(_dt);
	}

	// Token: 0x06005FDB RID: 24539 RVA: 0x0026DE04 File Offset: 0x0026C004
	[return: TupleElementNames(new string[]
	{
		"isUnlocked",
		"set"
	})]
	public ValueTuple<bool, EntitlementSetEnum> IsUnlocked()
	{
		if (this.entryType != XUiC_CharacterCosmeticEntry.EntryTypes.Item)
		{
			return new ValueTuple<bool, EntitlementSetEnum>(true, EntitlementSetEnum.None);
		}
		if (this.itemClass == null)
		{
			return new ValueTuple<bool, EntitlementSetEnum>(false, EntitlementSetEnum.None);
		}
		return base.xui.PlayerEquipment.Equipment.HasCosmeticUnlocked(this.itemClass);
	}

	// Token: 0x170009E6 RID: 2534
	// (get) Token: 0x06005FDC RID: 24540 RVA: 0x0026DE44 File Offset: 0x0026C044
	public string Name
	{
		get
		{
			switch (this.EntryType)
			{
			case XUiC_CharacterCosmeticEntry.EntryTypes.Item:
				if (this.itemClass != null)
				{
					return this.itemClass.GetLocalizedItemName();
				}
				break;
			case XUiC_CharacterCosmeticEntry.EntryTypes.Empty:
				return "Empty";
			case XUiC_CharacterCosmeticEntry.EntryTypes.Hide:
				return "Hide";
			}
			return "COSMETICS";
		}
	}

	// Token: 0x06005FDD RID: 24541 RVA: 0x0026DE94 File Offset: 0x0026C094
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		ValueTuple<bool, EntitlementSetEnum> valueTuple = this.IsUnlocked();
		bool item = valueTuple.Item1;
		EntitlementSetEnum item2 = valueTuple.Item2;
		if (bindingName == "itemicon")
		{
			value = "";
			switch (this.EntryType)
			{
			case XUiC_CharacterCosmeticEntry.EntryTypes.Item:
				if (this.itemClass != null)
				{
					value = this.itemClass.GetIconName();
				}
				break;
			case XUiC_CharacterCosmeticEntry.EntryTypes.Empty:
				value = "server_refresh";
				break;
			case XUiC_CharacterCosmeticEntry.EntryTypes.Hide:
				value = "ui_game_symbol_x";
				break;
			}
			return true;
		}
		if (bindingName == "iconatlas")
		{
			value = "ItemIconAtlas";
			if (this.EntryType != XUiC_CharacterCosmeticEntry.EntryTypes.Item)
			{
				value = "UIAtlas";
			}
			else
			{
				if (this.itemClass == null)
				{
					return true;
				}
				if (!item)
				{
					value = "ItemIconAtlasGreyscale";
				}
			}
			return true;
		}
		if (bindingName == "islocked")
		{
			value = "false";
			if (!item && item2 != EntitlementSetEnum.None)
			{
				value = "true";
			}
			return true;
		}
		if (bindingName == "selectionbordercolor")
		{
			value = this.selectionbordercolorFormatter.Format(this.SelectionBorderColor);
			return true;
		}
		if (bindingName == "backgroundcolor")
		{
			if (!item && this.itemClass != null)
			{
				value = this.backgroundcolorFormatter.Format(this.lockedBackgroundColor);
			}
			else
			{
				value = this.backgroundcolorFormatter.Format(this.backgroundColor);
			}
			return true;
		}
		if (!(bindingName == "tooltip"))
		{
			return false;
		}
		value = "";
		switch (this.EntryType)
		{
		case XUiC_CharacterCosmeticEntry.EntryTypes.Item:
			if (this.itemClass != null)
			{
				value = this.itemClass.GetLocalizedItemName();
			}
			break;
		case XUiC_CharacterCosmeticEntry.EntryTypes.Empty:
			value = "Empty";
			break;
		case XUiC_CharacterCosmeticEntry.EntryTypes.Hide:
			value = "Hide";
			break;
		}
		return true;
	}

	// Token: 0x06005FDE RID: 24542 RVA: 0x0026E03C File Offset: 0x0026C23C
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "select_color")
		{
			this.selectColor = StringParsers.ParseColor32(_value);
			return true;
		}
		if (_name == "press_color")
		{
			this.pressColor = StringParsers.ParseColor32(_value);
			return true;
		}
		if (_name == "background_color")
		{
			this.backgroundColor = StringParsers.ParseColor32(_value);
			return true;
		}
		if (_name == "locked_background_color")
		{
			this.backgroundColor = StringParsers.ParseColor32(_value);
			return true;
		}
		if (_name == "highlight_color")
		{
			this.highlightColor = StringParsers.ParseColor32(_value);
			return true;
		}
		if (!(_name == "holding_color"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		this.holdingColor = StringParsers.ParseColor32(_value);
		return true;
	}

	// Token: 0x04004828 RID: 18472
	public XUiC_CharacterCosmeticList Owner;

	// Token: 0x04004829 RID: 18473
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass itemClass;

	// Token: 0x0400482A RID: 18474
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CharacterCosmeticEntry.EntryTypes entryType;

	// Token: 0x0400482B RID: 18475
	[PublicizedFrom(EAccessModifier.Private)]
	public int index = -1;

	// Token: 0x0400482C RID: 18476
	[PublicizedFrom(EAccessModifier.Private)]
	public bool selected;

	// Token: 0x0400482D RID: 18477
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isHovered;

	// Token: 0x0400482E RID: 18478
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_Sprite itemIconSprite;

	// Token: 0x0400482F RID: 18479
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_Texture backgroundTexture;

	// Token: 0x04004830 RID: 18480
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 selectionBorderColor;

	// Token: 0x04004831 RID: 18481
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 selectColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	// Token: 0x04004832 RID: 18482
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 pressColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	// Token: 0x04004833 RID: 18483
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 backgroundColor = new Color32(96, 96, 96, byte.MaxValue);

	// Token: 0x04004834 RID: 18484
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 lockedBackgroundColor = new Color32(48, 48, 48, byte.MaxValue);

	// Token: 0x04004835 RID: 18485
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 highlightColor = new Color32(222, 206, 163, byte.MaxValue);

	// Token: 0x04004836 RID: 18486
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 holdingColor = new Color32(byte.MaxValue, 128, 0, byte.MaxValue);

	// Token: 0x04004837 RID: 18487
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDragAndDrop;

	// Token: 0x04004838 RID: 18488
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor backgroundcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04004839 RID: 18489
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor selectionbordercolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x02000C2B RID: 3115
	public enum EntryTypes
	{
		// Token: 0x0400483B RID: 18491
		Item,
		// Token: 0x0400483C RID: 18492
		Empty,
		// Token: 0x0400483D RID: 18493
		Hide
	}
}
