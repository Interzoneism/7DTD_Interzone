using System;
using System.Text;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E34 RID: 3636
[Preserve]
public class XUiC_SkillCraftingInfoEntry : XUiController
{
	// Token: 0x17000B85 RID: 2949
	// (set) Token: 0x0600720B RID: 29195 RVA: 0x002E7337 File Offset: 0x002E5537
	public int ListIndex
	{
		set
		{
			if (value != this.listIndex)
			{
				this.listIndex = value;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x17000B86 RID: 2950
	// (get) Token: 0x0600720C RID: 29196 RVA: 0x002E7350 File Offset: 0x002E5550
	// (set) Token: 0x0600720D RID: 29197 RVA: 0x002E7358 File Offset: 0x002E5558
	public ProgressionClass.DisplayData Data
	{
		get
		{
			return this.data;
		}
		set
		{
			if (value != this.data)
			{
				this.data = value;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x17000B87 RID: 2951
	// (get) Token: 0x0600720E RID: 29198 RVA: 0x002E7371 File Offset: 0x002E5571
	// (set) Token: 0x0600720F RID: 29199 RVA: 0x002E7379 File Offset: 0x002E5579
	public bool IsSelected
	{
		get
		{
			return this.isSelected;
		}
		set
		{
			this.isSelected = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000B88 RID: 2952
	// (set) Token: 0x06007210 RID: 29200 RVA: 0x002E7389 File Offset: 0x002E5589
	public int MaxEntriesWithoutPaging
	{
		set
		{
			if (this.maxEntriesWithoutPaging != value)
			{
				this.maxEntriesWithoutPaging = value;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x17000B89 RID: 2953
	// (set) Token: 0x06007211 RID: 29201 RVA: 0x002E73A2 File Offset: 0x002E55A2
	public int HiddenEntriesWithPaging
	{
		set
		{
			if (this.hiddenEntriesWithPaging != value)
			{
				this.hiddenEntriesWithPaging = value;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x06007212 RID: 29202 RVA: 0x0028FAFA File Offset: 0x0028DCFA
	public override void Init()
	{
		base.Init();
	}

	// Token: 0x06007213 RID: 29203 RVA: 0x002E73BC File Offset: 0x002E55BC
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "color_bg_bought")
		{
			this.color_bg_bought = _value;
			return true;
		}
		if (_name == "color_bg_available")
		{
			this.color_bg_available = _value;
			return true;
		}
		if (_name == "color_bg_locked")
		{
			this.color_bg_locked = _value;
			return true;
		}
		if (_name == "color_lbl_available")
		{
			this.color_lbl_available = _value;
			return true;
		}
		if (!(_name == "color_lbl_locked"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		this.color_lbl_locked = _value;
		return true;
	}

	// Token: 0x06007214 RID: 29204 RVA: 0x002E7444 File Offset: 0x002E5644
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		bool flag = this.data != null;
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
		if (num <= 2155443558U)
		{
			if (num <= 375292317U)
			{
				if (num <= 94814074U)
				{
					if (num != 12870938U)
					{
						if (num == 94814074U)
						{
							if (_bindingName == "nextqualitytext")
							{
								if (!flag)
								{
									_value = "";
									return true;
								}
								int num2 = this.data.GetQualityLevel(entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level);
								if (num2 < this.data.QualityStarts.Length + 1)
								{
									num2++;
								}
								_value = num2.ToString();
								return true;
							}
						}
					}
					else if (_bindingName == "color_fg")
					{
						_value = this.color_lbl_available;
						if (flag)
						{
							_value = ((this.data.GetQualityLevel(entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level) == 0) ? this.color_lbl_locked : this.color_lbl_available);
						}
						return true;
					}
				}
				else if (num != 306828118U)
				{
					if (num != 372648664U)
					{
						if (num == 375292317U)
						{
							if (_bindingName == "showquality")
							{
								_value = (flag ? (this.data.HasQuality && this.data.GetQualityLevel(entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level) > 0).ToString() : "false");
								return true;
							}
						}
					}
					else if (_bindingName == "show_selected")
					{
						_value = this.isSelected.ToString();
						return true;
					}
				}
				else if (_bindingName == "currentqualitycolor")
				{
					if (!flag)
					{
						_value = "FFFFFF";
						return true;
					}
					Color32 v = QualityInfo.GetTierColor(this.data.GetQualityLevel(entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level));
					_value = this.durabilitycolorFormatter.Format(v);
					return true;
				}
			}
			else if (num <= 859026113U)
			{
				if (num != 850736759U)
				{
					if (num == 859026113U)
					{
						if (_bindingName == "notcomplete")
						{
							_value = (flag ? (!this.data.IsComplete(entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level)).ToString() : "false");
							return true;
						}
					}
				}
				else if (_bindingName == "nextpoints")
				{
					_value = (flag ? string.Format("{0}/{1}", entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level, this.data.GetNextPoints(entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level).ToString()) : "");
					return true;
				}
			}
			else if (num != 1566407741U)
			{
				if (num != 1701888201U)
				{
					if (num == 2155443558U)
					{
						if (_bindingName == "nothiddenbypager")
						{
							_value = "true";
							return true;
						}
					}
				}
				else if (_bindingName == "itemcolor")
				{
					_value = (flag ? this.data.GetIconTint(entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level) : "FFFFFF");
					return true;
				}
			}
			else if (_bindingName == "hasentry")
			{
				_value = flag.ToString();
				return true;
			}
		}
		else if (num <= 3708628627U)
		{
			if (num <= 3185987134U)
			{
				if (num != 2842844659U)
				{
					if (num == 3185987134U)
					{
						if (_bindingName == "text")
						{
							_value = (flag ? this.data.GetName(entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level) : "");
							return true;
						}
					}
				}
				else if (_bindingName == "showcomplete")
				{
					_value = (flag ? this.data.IsComplete(entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level).ToString() : "false");
					return true;
				}
			}
			else if (num != 3263401906U)
			{
				if (num != 3503204070U)
				{
					if (num == 3708628627U)
					{
						if (_bindingName == "itemicon")
						{
							_value = (flag ? this.data.GetIcon(entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level) : "");
							return true;
						}
					}
				}
				else if (_bindingName == "color_bg")
				{
					_value = this.color_bg_available;
					if (flag)
					{
						_value = ((this.data.GetQualityLevel(entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level) == 0) ? this.color_bg_locked : this.color_bg_available);
					}
					return true;
				}
			}
			else if (_bindingName == "notcompletequality")
			{
				_value = (flag ? (this.data.HasQuality && !this.data.IsComplete(entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level)).ToString() : "false");
				return true;
			}
		}
		else if (num <= 3933605619U)
		{
			if (num != 3924526227U)
			{
				if (num == 3933605619U)
				{
					if (_bindingName == "notcompletenoquality")
					{
						_value = (flag ? (!this.data.HasQuality && !this.data.IsComplete(entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level)).ToString() : "false");
						return true;
					}
				}
			}
			else if (_bindingName == "showlock")
			{
				_value = (flag ? (this.data.GetQualityLevel(entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level) == 0).ToString() : "false");
				return true;
			}
		}
		else if (num != 4168931239U)
		{
			if (num != 4220565952U)
			{
				if (num == 4277871100U)
				{
					if (_bindingName == "nextqualitycolor")
					{
						if (!flag)
						{
							_value = "";
							return true;
						}
						int num3 = this.data.GetQualityLevel(entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level);
						if (num3 < this.data.QualityStarts.Length + 1)
						{
							num3++;
						}
						_value = this.nextdurabilitycolorFormatter.Format(QualityInfo.GetTierColor(num3));
						return true;
					}
				}
			}
			else if (_bindingName == "currentqualitytext")
			{
				_value = (flag ? this.data.GetQualityLevel(entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level).ToString() : "");
				return true;
			}
		}
		else if (_bindingName == "iconatlas")
		{
			if (!flag)
			{
				_value = "ItemIconAtlas";
			}
			else
			{
				_value = ((entityPlayer.Progression.GetProgressionValue(this.data.Owner.Name).Level >= this.data.QualityStarts[0]) ? "ItemIconAtlas" : "ItemIconAtlasGreyscale");
			}
			return true;
		}
		return false;
	}

	// Token: 0x06007215 RID: 29205 RVA: 0x002E60A0 File Offset: 0x002E42A0
	public override void Update(float _dt)
	{
		if (this.IsDirty)
		{
			this.IsDirty = false;
			base.RefreshBindings(this.IsDirty);
		}
		base.Update(_dt);
	}

	// Token: 0x040056CE RID: 22222
	[PublicizedFrom(EAccessModifier.Private)]
	public string color_bg_bought;

	// Token: 0x040056CF RID: 22223
	[PublicizedFrom(EAccessModifier.Private)]
	public string color_bg_available;

	// Token: 0x040056D0 RID: 22224
	[PublicizedFrom(EAccessModifier.Private)]
	public string color_bg_locked;

	// Token: 0x040056D1 RID: 22225
	[PublicizedFrom(EAccessModifier.Private)]
	public string color_lbl_available;

	// Token: 0x040056D2 RID: 22226
	[PublicizedFrom(EAccessModifier.Private)]
	public string color_lbl_locked;

	// Token: 0x040056D3 RID: 22227
	[PublicizedFrom(EAccessModifier.Private)]
	public int listIndex;

	// Token: 0x040056D4 RID: 22228
	[PublicizedFrom(EAccessModifier.Private)]
	public ProgressionClass.DisplayData data;

	// Token: 0x040056D5 RID: 22229
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isSelected;

	// Token: 0x040056D6 RID: 22230
	[PublicizedFrom(EAccessModifier.Private)]
	public int maxEntriesWithoutPaging = 5;

	// Token: 0x040056D7 RID: 22231
	[PublicizedFrom(EAccessModifier.Private)]
	public int hiddenEntriesWithPaging = 2;

	// Token: 0x040056D8 RID: 22232
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<string, float, bool> attributeSubtractionFormatter = new CachedStringFormatter<string, float, bool>((string _s, float _f, bool _b) => string.Format("{0}: {1}", _s, _f.ToCultureInvariantString("0.#") + (_b ? "%" : "")));

	// Token: 0x040056D9 RID: 22233
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<string, float> attributeSetValueFormatter = new CachedStringFormatter<string, float>((string _s, float _f) => string.Format("{0}: {1}", _s, _f.ToCultureInvariantString("0.#")));

	// Token: 0x040056DA RID: 22234
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor durabilitycolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x040056DB RID: 22235
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor nextdurabilitycolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x040056DC RID: 22236
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly StringBuilder effectsStringBuilder = new StringBuilder();
}
