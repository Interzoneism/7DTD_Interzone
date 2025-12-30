using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DC7 RID: 3527
[Preserve]
public class XUiC_QuestTurnInEntry : XUiC_SelectableEntry
{
	// Token: 0x17000B1C RID: 2844
	// (get) Token: 0x06006E5B RID: 28251 RVA: 0x002CFD5C File Offset: 0x002CDF5C
	// (set) Token: 0x06006E5C RID: 28252 RVA: 0x002CFD64 File Offset: 0x002CDF64
	public BaseReward Reward
	{
		get
		{
			return this.reward;
		}
		set
		{
			this.reward = value;
			this.Refresh();
			if (!base.Selected)
			{
				this.background.Color = new Color32(64, 64, 64, byte.MaxValue);
			}
		}
	}

	// Token: 0x17000B1D RID: 2845
	// (get) Token: 0x06006E5D RID: 28253 RVA: 0x002CFD9C File Offset: 0x002CDF9C
	public ItemStack Item
	{
		get
		{
			if (this.reward != null && this.item.IsEmpty())
			{
				if (this.reward is RewardItem)
				{
					this.item = (this.reward as RewardItem).Item;
				}
				else if (this.reward is RewardLootItem)
				{
					this.item = (this.reward as RewardLootItem).Item;
				}
			}
			return this.item;
		}
	}

	// Token: 0x17000B1E RID: 2846
	// (get) Token: 0x06006E5E RID: 28254 RVA: 0x002CFE0C File Offset: 0x002CE00C
	// (set) Token: 0x06006E5F RID: 28255 RVA: 0x002CFE14 File Offset: 0x002CE014
	public bool Chosen
	{
		get
		{
			return this.chosen;
		}
		set
		{
			this.chosen = value;
			this.RefreshBackground();
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06006E60 RID: 28256 RVA: 0x002CFE2A File Offset: 0x002CE02A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SelectedChanged(bool isSelected)
	{
		this.RefreshBackground();
	}

	// Token: 0x06006E61 RID: 28257 RVA: 0x002CFE34 File Offset: 0x002CE034
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshBackground()
	{
		if (this.background != null)
		{
			this.background.Color = (base.Selected ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue) : new Color32(64, 64, 64, byte.MaxValue));
			this.background.SpriteName = (base.Selected ? "ui_game_select_row" : "menu_empty");
		}
	}

	// Token: 0x06006E62 RID: 28258 RVA: 0x002CFEAC File Offset: 0x002CE0AC
	public void SetBaseReward(BaseReward reward)
	{
		this.Reward = reward;
		this.item = ItemStack.Empty;
		this.isDirty = true;
		base.RefreshBindings(false);
		base.ViewComponent.Enabled = (reward != null);
		if (reward == null)
		{
			this.background.Color = new Color32(64, 64, 64, byte.MaxValue);
		}
	}

	// Token: 0x06006E63 RID: 28259 RVA: 0x002CFF0C File Offset: 0x002CE10C
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

	// Token: 0x06006E64 RID: 28260 RVA: 0x002CFF6C File Offset: 0x002CE16C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		this.isHovered = _isOver;
		if (this.background != null && this.reward != null && !base.Selected)
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

	// Token: 0x06006E65 RID: 28261 RVA: 0x002CFFE4 File Offset: 0x002CE1E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 1388578781U)
		{
			if (num <= 847165955U)
			{
				if (num != 392967384U)
				{
					if (num != 602070729U)
					{
						if (num == 847165955U)
						{
							if (bindingName == "itemtypeicon")
							{
								if (this.reward != null)
								{
									if (this.item != null && !this.item.IsEmpty())
									{
										if (this.item.itemValue.ItemClass.IsBlock())
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
									}
									else
									{
										value = "";
									}
								}
								else
								{
									value = "";
								}
								return true;
							}
						}
					}
					else if (bindingName == "namecolor")
					{
						value = ((this.reward != null && this.chosen) ? this.rewardchosencolorFormatter.Format(this.selectedColor) : this.rewardchosencolorFormatter.Format(this.defaultColor));
						return true;
					}
				}
				else if (bindingName == "hasotherreward")
				{
					if (this.reward != null)
					{
						if (this.item != null && !this.item.IsEmpty())
						{
							value = "false";
						}
						else
						{
							value = "true";
						}
					}
					else
					{
						value = "true";
					}
					return true;
				}
			}
			else if (num <= 1062608009U)
			{
				if (num != 899280930U)
				{
					if (num == 1062608009U)
					{
						if (bindingName == "durabilitycolor")
						{
							if (this.Item != null && !this.Item.IsEmpty() && this.Item.itemValue.ItemClass.ShowQualityBar)
							{
								Color32 v = QualityInfo.GetTierColor((int)this.Item.itemValue.Quality);
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
				else if (bindingName == "chosenicon")
				{
					value = ((this.reward != null && this.chosen) ? "ui_game_symbol_check" : "");
					return true;
				}
			}
			else if (num != 1290001761U)
			{
				if (num == 1388578781U)
				{
					if (bindingName == "hasitemtypeicon")
					{
						if (this.reward != null)
						{
							if (this.item != null && !this.item.IsEmpty())
							{
								if (this.item.itemValue.ItemClass.IsBlock())
								{
									value = (Block.list[this.item.itemValue.type].ItemTypeIcon != "").ToString();
								}
								else
								{
									value = (this.item.itemValue.ItemClass.ItemTypeIcon != "").ToString();
								}
							}
							else
							{
								value = "false";
							}
						}
						else
						{
							value = "false";
						}
						return true;
					}
				}
			}
			else if (bindingName == "rewardname")
			{
				value = ((this.reward != null) ? this.reward.GetRewardText() : "");
				return true;
			}
		}
		else if (num <= 2485383123U)
		{
			if (num <= 2048631988U)
			{
				if (num != 1771022199U)
				{
					if (num == 2048631988U)
					{
						if (bindingName == "hasreward")
						{
							value = (this.reward != null).ToString();
							return true;
						}
					}
				}
				else if (bindingName == "rewardicon")
				{
					if (this.reward != null)
					{
						if (this.item != null && !this.item.IsEmpty())
						{
							value = this.item.itemValue.ItemClass.GetIconName();
						}
						else
						{
							value = this.reward.Icon;
						}
					}
					else
					{
						value = "";
					}
					return true;
				}
			}
			else if (num != 2400639434U)
			{
				if (num == 2485383123U)
				{
					if (bindingName == "hasitemreward")
					{
						if (this.reward != null)
						{
							if (this.item != null && !this.item.IsEmpty())
							{
								value = "true";
							}
							else
							{
								value = "false";
							}
						}
						else
						{
							value = "false";
						}
						return true;
					}
				}
			}
			else if (bindingName == "rewardicontint")
			{
				if (this.reward != null)
				{
					if (this.item != null && !this.item.IsEmpty())
					{
						value = this.rewardiconcolorFormatter.Format(this.item.itemValue.ItemClass.GetIconTint(this.item.itemValue));
					}
					else
					{
						value = "[iconColor]";
					}
				}
				else
				{
					value = "[iconColor]";
				}
				return true;
			}
		}
		else if (num <= 4049247086U)
		{
			if (num != 2944858628U)
			{
				if (num == 4049247086U)
				{
					if (bindingName == "itemtypeicontint")
					{
						value = "255,255,255,255";
						if (this.item != null && !this.item.IsEmpty() && this.item.itemValue.ItemClass.Unlocks != "" && XUiM_ItemStack.CheckKnown(base.xui.playerUI.entityPlayer, this.item.itemValue.ItemClass, this.item.itemValue))
						{
							value = this.altitemtypeiconcolorFormatter.Format(this.item.itemValue.ItemClass.AltItemTypeIconColor);
						}
						return true;
					}
				}
			}
			else if (bindingName == "hasdurability")
			{
				value = (this.Item != null && !this.Item.IsEmpty() && this.Item.itemValue.ItemClass.ShowQualityBar).ToString();
				return true;
			}
		}
		else if (num != 4107624007U)
		{
			if (num == 4172540779U)
			{
				if (bindingName == "durabilityfill")
				{
					value = ((this.Item != null && !this.Item.IsEmpty()) ? ((this.Item.itemValue.MaxUseTimes == 0) ? "1" : this.durabilityFillFormatter.Format(this.Item.itemValue.PercentUsesLeft)) : "0");
					return true;
				}
			}
		}
		else if (bindingName == "durabilityvalue")
		{
			value = "";
			if (this.Item != null && !this.Item.IsEmpty())
			{
				if (this.Item.itemValue.HasQuality || (!this.Item.IsEmpty() && this.Item.itemValue.ItemClass.HasSubItems))
				{
					value = ((this.Item.itemValue.Quality > 0) ? this.durabilityValueFormatter.Format((int)this.Item.itemValue.Quality) : "-");
				}
				else
				{
					value = "";
				}
			}
			else
			{
				value = "";
			}
			return true;
		}
		return false;
	}

	// Token: 0x06006E66 RID: 28262 RVA: 0x002D07BC File Offset: 0x002CE9BC
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(name, value, _parent);
		if (!flag)
		{
			if (!(name == "default_color"))
			{
				if (!(name == "selected_color"))
				{
					return false;
				}
				this.selectedColor = StringParsers.ParseColor32(value);
			}
			else
			{
				this.defaultColor = StringParsers.ParseColor32(value);
			}
			return true;
		}
		return flag;
	}

	// Token: 0x06006E67 RID: 28263 RVA: 0x002D0813 File Offset: 0x002CEA13
	public void Refresh()
	{
		this.isDirty = true;
		base.RefreshBindings(false);
	}

	// Token: 0x040053DB RID: 21467
	[PublicizedFrom(EAccessModifier.Private)]
	public BaseReward reward;

	// Token: 0x040053DC RID: 21468
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isHovered;

	// Token: 0x040053DD RID: 21469
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x040053DE RID: 21470
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite background;

	// Token: 0x040053DF RID: 21471
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack item = ItemStack.Empty;

	// Token: 0x040053E0 RID: 21472
	[PublicizedFrom(EAccessModifier.Private)]
	public bool chosen;

	// Token: 0x040053E1 RID: 21473
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor recipeicontintcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x040053E2 RID: 21474
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor hasingredientsstatecolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x040053E3 RID: 21475
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor unlockstatecolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x040053E4 RID: 21476
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor durabilityColorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x040053E5 RID: 21477
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt durabilityValueFormatter = new CachedStringFormatterInt();

	// Token: 0x040053E6 RID: 21478
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat durabilityFillFormatter = new CachedStringFormatterFloat(null);

	// Token: 0x040053E7 RID: 21479
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor rewardiconcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x040053E8 RID: 21480
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor rewardchosencolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x040053E9 RID: 21481
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor altitemtypeiconcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x040053EA RID: 21482
	[PublicizedFrom(EAccessModifier.Private)]
	public Color defaultColor;

	// Token: 0x040053EB RID: 21483
	[PublicizedFrom(EAccessModifier.Private)]
	public Color selectedColor;
}
