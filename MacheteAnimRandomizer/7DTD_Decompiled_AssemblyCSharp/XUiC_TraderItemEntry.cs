using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E76 RID: 3702
[Preserve]
public class XUiC_TraderItemEntry : XUiC_SelectableEntry
{
	// Token: 0x17000BD4 RID: 3028
	// (get) Token: 0x06007453 RID: 29779 RVA: 0x002F444E File Offset: 0x002F264E
	// (set) Token: 0x06007454 RID: 29780 RVA: 0x002F4456 File Offset: 0x002F2656
	public XUiC_TraderWindow TraderWindow { get; set; }

	// Token: 0x17000BD5 RID: 3029
	// (get) Token: 0x06007455 RID: 29781 RVA: 0x002F445F File Offset: 0x002F265F
	// (set) Token: 0x06007456 RID: 29782 RVA: 0x002F4467 File Offset: 0x002F2667
	public XUiC_ItemInfoWindow InfoWindow { get; set; }

	// Token: 0x17000BD6 RID: 3030
	// (get) Token: 0x06007457 RID: 29783 RVA: 0x002F4470 File Offset: 0x002F2670
	// (set) Token: 0x06007458 RID: 29784 RVA: 0x002F4478 File Offset: 0x002F2678
	public ItemStack Item
	{
		get
		{
			return this.item;
		}
		set
		{
			this.item = value;
			this.isDirty = true;
			this.itemClass = ((this.item == null) ? null : this.item.itemValue.ItemClass);
			base.ViewComponent.Enabled = (base.ViewComponent.IsNavigatable = (this.item != null));
			base.RefreshBindings(false);
			if (this.item == null)
			{
				this.background.Color = new Color32(64, 64, 64, byte.MaxValue);
			}
		}
	}

	// Token: 0x06007459 RID: 29785 RVA: 0x002F4508 File Offset: 0x002F2708
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SelectedChanged(bool isSelected)
	{
		if (this.background != null)
		{
			this.background.Color = (isSelected ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue) : new Color32(64, 64, 64, byte.MaxValue));
			this.background.SpriteName = (isSelected ? "ui_game_select_row" : "menu_empty");
		}
	}

	// Token: 0x0600745A RID: 29786 RVA: 0x002F4578 File Offset: 0x002F2778
	public override void Init()
	{
		base.Init();
		for (int i = 0; i < this.children.Count; i++)
		{
			XUiView viewComponent = this.children[i].ViewComponent;
			if (viewComponent.ID.EqualsCaseInsensitive("background"))
			{
				this.background = (viewComponent as XUiV_Sprite);
			}
		}
		this.isDirty = true;
	}

	// Token: 0x0600745B RID: 29787 RVA: 0x002F45D8 File Offset: 0x002F27D8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		this.isHovered = _isOver;
		if (this.background != null && this.item != null && !base.Selected)
		{
			if (_isOver)
			{
				this.background.Color = new Color32(96, 96, 96, byte.MaxValue);
			}
			else
			{
				this.background.Color = new Color32(64, 64, 64, byte.MaxValue);
			}
		}
		base.OnHovered(_isOver);
	}

	// Token: 0x0600745C RID: 29788 RVA: 0x002F4650 File Offset: 0x002F2850
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 2200816055U)
		{
			if (num <= 1062608009U)
			{
				if (num != 847165955U)
				{
					if (num != 968607449U)
					{
						if (num == 1062608009U)
						{
							if (bindingName == "durabilitycolor")
							{
								if (this.item != null && !this.item.IsEmpty() && this.item.itemValue.ItemClass.ShowQualityBar)
								{
									Color32 v = QualityInfo.GetTierColor((int)this.item.itemValue.Quality);
									value = this.durabilityColorFormatter.Format(v);
								}
								else
								{
									value = "0,0,0,0";
								}
								return true;
							}
						}
					}
					else if (bindingName == "statecolor")
					{
						if (this.item != null)
						{
							Color32 v2 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
							value = this.stateColorFormatter.Format(v2);
						}
						else
						{
							value = "255,255,255,255";
						}
						return true;
					}
				}
				else if (bindingName == "itemtypeicon")
				{
					if (this.item == null)
					{
						value = "";
					}
					else if (this.item.itemValue.ItemClass.IsBlock())
					{
						value = Block.list[this.item.itemValue.type].ItemTypeIcon;
					}
					else
					{
						if (this.item.itemValue.ItemClass.AltItemTypeIcon != null && this.item.itemValue.ItemClass.Unlocks != "" && XUiM_ItemStack.CheckKnown(base.xui.playerUI.entityPlayer, this.item.itemValue.ItemClass, this.item.itemValue))
						{
							value = this.item.itemValue.ItemClass.AltItemTypeIcon;
							return true;
						}
						value = this.item.itemValue.ItemClass.ItemTypeIcon;
					}
					return true;
				}
			}
			else if (num <= 1388578781U)
			{
				if (num != 1159116676U)
				{
					if (num == 1388578781U)
					{
						if (bindingName == "hasitemtypeicon")
						{
							if (this.item == null)
							{
								value = "false";
							}
							else if (this.item.itemValue.ItemClass.IsBlock())
							{
								value = (Block.list[this.item.itemValue.type].ItemTypeIcon != "").ToString();
							}
							else
							{
								value = (this.item.itemValue.ItemClass.ItemTypeIcon != "").ToString();
							}
							return true;
						}
					}
				}
				else if (bindingName == "hasitem")
				{
					value = (this.item != null).ToString();
					return true;
				}
			}
			else if (num != 1580050147U)
			{
				if (num == 2200816055U)
				{
					if (bindingName == "itemprice")
					{
						if (this.item != null)
						{
							int count = this.itemClass.IsBlock() ? Block.list[this.item.itemValue.type].EconomicBundleSize : this.itemClass.EconomicBundleSize;
							value = this.itemPriceFormatter.Format(XUiM_Trader.GetBuyPrice(base.xui, this.item.itemValue, count, null, this.SlotIndex));
						}
						else
						{
							value = "";
						}
						return true;
					}
				}
			}
			else if (bindingName == "currencyicon")
			{
				if (this.item != null)
				{
					value = TraderInfo.CurrencyItem;
				}
				else
				{
					value = "";
				}
				return true;
			}
		}
		else if (num <= 3868891837U)
		{
			if (num <= 3191456325U)
			{
				if (num != 2944858628U)
				{
					if (num == 3191456325U)
					{
						if (bindingName == "itemname")
						{
							value = "";
							if (this.item != null)
							{
								if (this.item.count == 1)
								{
									value = this.itemClass.GetLocalizedItemName();
								}
								else
								{
									value = this.itemNameFormatter.Format(this.itemClass.GetLocalizedItemName(), this.item.count);
								}
							}
							return true;
						}
					}
				}
				else if (bindingName == "hasdurability")
				{
					value = (this.item != null && !this.item.IsEmpty() && this.item.itemValue.ItemClass.ShowQualityBar).ToString();
					return true;
				}
			}
			else if (num != 3708628627U)
			{
				if (num == 3868891837U)
				{
					if (bindingName == "pricecolor")
					{
						value = "255,255,255,255";
						if (this.item != null && base.xui.Trader.TraderTileEntity is TileEntityVendingMachine && (base.xui.Trader.Trader.TraderInfo.PlayerOwned || base.xui.Trader.Trader.TraderInfo.Rentable))
						{
							int markupByIndex = base.xui.Trader.Trader.GetMarkupByIndex(this.SlotIndex);
							if (markupByIndex > 0)
							{
								value = "255,0,0,255";
							}
							else if (markupByIndex < 0)
							{
								value = "0,255,0,255";
							}
						}
						return true;
					}
				}
			}
			else if (bindingName == "itemicon")
			{
				if (this.item != null)
				{
					value = this.item.itemValue.GetPropertyOverride("CustomIcon", this.itemClass.GetIconName());
				}
				else
				{
					value = "";
				}
				return true;
			}
		}
		else if (num <= 4053908414U)
		{
			if (num != 4049247086U)
			{
				if (num == 4053908414U)
				{
					if (bindingName == "itemicontint")
					{
						Color32 v3 = Color.white;
						if (this.item != null)
						{
							v3 = this.item.itemValue.ItemClass.GetIconTint(this.item.itemValue);
						}
						value = this.itemicontintcolorFormatter.Format(v3);
						return true;
					}
				}
			}
			else if (bindingName == "itemtypeicontint")
			{
				value = "255,255,255,255";
				if (this.item != null && this.item.itemValue.ItemClass.Unlocks != "" && XUiM_ItemStack.CheckKnown(base.xui.playerUI.entityPlayer, this.item.itemValue.ItemClass, this.item.itemValue))
				{
					value = this.altitemtypeiconcolorFormatter.Format(this.item.itemValue.ItemClass.AltItemTypeIconColor);
				}
				return true;
			}
		}
		else if (num != 4107624007U)
		{
			if (num == 4172540779U)
			{
				if (bindingName == "durabilityfill")
				{
					value = ((this.item != null && !this.item.IsEmpty()) ? ((this.item.itemValue.MaxUseTimes == 0) ? "1" : this.durabilityFillFormatter.Format(((float)this.item.itemValue.MaxUseTimes - this.item.itemValue.UseTimes) / (float)this.item.itemValue.MaxUseTimes)) : "1");
					return true;
				}
			}
		}
		else if (bindingName == "durabilityvalue")
		{
			value = "";
			if (this.item != null)
			{
				if (this.item.itemValue.HasQuality || this.itemClass.HasSubItems)
				{
					value = ((this.item.itemValue.Quality > 0) ? this.durabilityValueFormatter.Format((int)this.item.itemValue.Quality) : "-");
				}
				else
				{
					value = "-";
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x0600745D RID: 29789 RVA: 0x002F4E58 File Offset: 0x002F3058
	public void Refresh()
	{
		if (this.item.count == 0)
		{
			this.Item = null;
			this.itemClass = null;
			this.TraderWindow.RefreshTraderItems();
		}
		if (base.Selected)
		{
			this.InfoWindow.SetItemStack(this, false);
		}
		this.isDirty = true;
		base.RefreshBindings(false);
	}

	// Token: 0x04005872 RID: 22642
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack item;

	// Token: 0x04005873 RID: 22643
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass itemClass;

	// Token: 0x04005874 RID: 22644
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x04005875 RID: 22645
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isHovered;

	// Token: 0x04005876 RID: 22646
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasIngredients;

	// Token: 0x04005877 RID: 22647
	public int SlotIndex;

	// Token: 0x04005878 RID: 22648
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite background;

	// Token: 0x0400587B RID: 22651
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<string, int> itemNameFormatter = new CachedStringFormatter<string, int>((string _s, int _i) => string.Format("{0} ({1})", _s, _i));

	// Token: 0x0400587C RID: 22652
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor itemicontintcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x0400587D RID: 22653
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor durabilityColorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x0400587E RID: 22654
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt durabilityValueFormatter = new CachedStringFormatterInt();

	// Token: 0x0400587F RID: 22655
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat durabilityFillFormatter = new CachedStringFormatterFloat(null);

	// Token: 0x04005880 RID: 22656
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor stateColorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04005881 RID: 22657
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt itemPriceFormatter = new CachedStringFormatterInt();

	// Token: 0x04005882 RID: 22658
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor altitemtypeiconcolorFormatter = new CachedStringFormatterXuiRgbaColor();
}
