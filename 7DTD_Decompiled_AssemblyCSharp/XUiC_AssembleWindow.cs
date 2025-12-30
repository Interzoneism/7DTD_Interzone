using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C09 RID: 3081
[Preserve]
public class XUiC_AssembleWindow : XUiController
{
	// Token: 0x06005E6B RID: 24171 RVA: 0x0026483B File Offset: 0x00262A3B
	public override void Init()
	{
		base.Init();
		this.btnComplete = base.GetChildById("btnComplete");
		if (this.btnComplete != null)
		{
			this.btnComplete.OnPress += this.BtnComplete_OnPress;
		}
	}

	// Token: 0x170009BF RID: 2495
	// (get) Token: 0x06005E6C RID: 24172 RVA: 0x00264873 File Offset: 0x00262A73
	// (set) Token: 0x06005E6D RID: 24173 RVA: 0x0026487C File Offset: 0x00262A7C
	public virtual ItemStack ItemStack
	{
		get
		{
			return this.itemStack;
		}
		set
		{
			this.itemStack = value;
			if (!this.itemStack.IsEmpty())
			{
				this.itemClass = this.itemStack.itemValue.ItemClass;
				this.itemDisplayEntry = UIDisplayInfoManager.Current.GetDisplayStatsForTag(this.itemClass.DisplayType);
			}
			else
			{
				this.itemClass = null;
			}
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06005E6E RID: 24174 RVA: 0x002648DE File Offset: 0x00262ADE
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnComplete_OnPress(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.CloseAllOpenWindows(null, false);
	}

	// Token: 0x06005E6F RID: 24175 RVA: 0x002648F8 File Offset: 0x00262AF8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = !this.itemStack.IsEmpty();
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 3191456325U)
		{
			if (num <= 1607128273U)
			{
				if (num <= 619741203U)
				{
					if (num != 546263858U)
					{
						if (num == 619741203U)
						{
							if (bindingName == "itemqualitytitle")
							{
								value = Localization.Get("xuiQuality", false);
								return true;
							}
						}
					}
					else if (bindingName == "itemqualityfill")
					{
						value = (flag ? this.qualityfillFormatter.Format(((float)this.itemStack.itemValue.MaxUseTimes - this.itemStack.itemValue.UseTimes) / (float)this.itemStack.itemValue.MaxUseTimes) : "1");
						return true;
					}
				}
				else if (num != 1556795416U)
				{
					if (num != 1573573035U)
					{
						if (num == 1607128273U)
						{
							if (bindingName == "itemstattitle1")
							{
								value = (flag ? this.GetStatTitle(0) : "");
								return true;
							}
						}
					}
					else if (bindingName == "itemstattitle3")
					{
						value = (flag ? this.GetStatTitle(2) : "");
						return true;
					}
				}
				else if (bindingName == "itemstattitle2")
				{
					value = (flag ? this.GetStatTitle(1) : "");
					return true;
				}
			}
			else if (num <= 1640683511U)
			{
				if (num != 1623905892U)
				{
					if (num == 1640683511U)
					{
						if (bindingName == "itemstattitle7")
						{
							value = (flag ? this.GetStatTitle(6) : "");
							return true;
						}
					}
				}
				else if (bindingName == "itemstattitle6")
				{
					value = (flag ? this.GetStatTitle(5) : "");
					return true;
				}
			}
			else if (num != 1657461130U)
			{
				if (num != 1674238749U)
				{
					if (num == 3191456325U)
					{
						if (bindingName == "itemname")
						{
							value = (flag ? this.itemClass.GetLocalizedItemName() : "");
							return true;
						}
					}
				}
				else if (bindingName == "itemstattitle5")
				{
					value = (flag ? this.GetStatTitle(4) : "");
					return true;
				}
			}
			else if (bindingName == "itemstattitle4")
			{
				value = (flag ? this.GetStatTitle(3) : "");
				return true;
			}
		}
		else if (num <= 4140647608U)
		{
			if (num <= 3994216002U)
			{
				if (num != 3708628627U)
				{
					if (num == 3994216002U)
					{
						if (bindingName == "itemqualitycolor")
						{
							value = "255,255,255,255";
							if (flag)
							{
								Color32 v = QualityInfo.GetQualityColor((int)this.itemStack.itemValue.Quality);
								value = this.qualitycolorFormatter.Format(v);
							}
							return true;
						}
					}
				}
				else if (bindingName == "itemicon")
				{
					value = "";
					if (flag)
					{
						value = this.itemStack.itemValue.GetPropertyOverride("CustomIcon", (this.itemClass.CustomIcon != null) ? this.itemClass.CustomIcon.Value : this.itemClass.GetIconName());
					}
					return true;
				}
			}
			else if (num != 4053908414U)
			{
				if (num != 4113438435U)
				{
					if (num == 4140647608U)
					{
						if (bindingName == "itemstat4")
						{
							value = (flag ? this.GetStatValue(3) : "");
							return true;
						}
					}
				}
				else if (bindingName == "itemquality")
				{
					value = (flag ? this.qualityFormatter.Format((int)this.itemStack.itemValue.Quality) : "0");
					return true;
				}
			}
			else if (bindingName == "itemicontint")
			{
				Color32 v2 = Color.white;
				if (this.itemClass != null)
				{
					v2 = this.itemStack.itemValue.ItemClass.GetIconTint(this.itemStack.itemValue);
				}
				value = this.itemicontintcolorFormatter.Format(v2);
				return true;
			}
		}
		else if (num <= 4190980465U)
		{
			if (num != 4157425227U)
			{
				if (num != 4174202846U)
				{
					if (num == 4190980465U)
					{
						if (bindingName == "itemstat7")
						{
							value = (flag ? this.GetStatValue(6) : "");
							return true;
						}
					}
				}
				else if (bindingName == "itemstat6")
				{
					value = (flag ? this.GetStatValue(5) : "");
					return true;
				}
			}
			else if (bindingName == "itemstat5")
			{
				value = (flag ? this.GetStatValue(4) : "");
				return true;
			}
		}
		else if (num != 4224535703U)
		{
			if (num != 4241313322U)
			{
				if (num == 4258090941U)
				{
					if (bindingName == "itemstat3")
					{
						value = (flag ? this.GetStatValue(2) : "");
						return true;
					}
				}
			}
			else if (bindingName == "itemstat2")
			{
				value = (flag ? this.GetStatValue(1) : "");
				return true;
			}
		}
		else if (bindingName == "itemstat1")
		{
			value = (flag ? this.GetStatValue(0) : "");
			return true;
		}
		return false;
	}

	// Token: 0x06005E70 RID: 24176 RVA: 0x00264EB8 File Offset: 0x002630B8
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetStatTitle(int index)
	{
		if (this.itemDisplayEntry == null || this.itemDisplayEntry.DisplayStats.Count <= index)
		{
			return "";
		}
		if (this.itemDisplayEntry.DisplayStats[index].TitleOverride != null)
		{
			return this.itemDisplayEntry.DisplayStats[index].TitleOverride;
		}
		return UIDisplayInfoManager.Current.GetLocalizedName(this.itemDisplayEntry.DisplayStats[index].StatType);
	}

	// Token: 0x06005E71 RID: 24177 RVA: 0x00264F38 File Offset: 0x00263138
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetStatValue(int index)
	{
		if (this.itemDisplayEntry == null || this.itemDisplayEntry.DisplayStats.Count <= index)
		{
			return "";
		}
		DisplayInfoEntry infoEntry = this.itemDisplayEntry.DisplayStats[index];
		return XUiM_ItemStack.GetStatItemValueTextWithModInfo(this.itemStack, base.xui.playerUI.entityPlayer, infoEntry);
	}

	// Token: 0x06005E72 RID: 24178 RVA: 0x00264F94 File Offset: 0x00263194
	public override void Update(float _dt)
	{
		if (GameManager.Instance == null && GameManager.Instance.World == null)
		{
			return;
		}
		if (this.isDirty)
		{
			base.RefreshBindings(false);
			this.isDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x06005E73 RID: 24179 RVA: 0x00264FCD File Offset: 0x002631CD
	public virtual void OnChanged()
	{
		XUiC_AssembleWindowGroup.GetWindowGroup(base.xui).ItemStack = this.ItemStack;
	}

	// Token: 0x04004739 RID: 18233
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack itemStack = ItemStack.Empty.Clone();

	// Token: 0x0400473A RID: 18234
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass itemClass;

	// Token: 0x0400473B RID: 18235
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnComplete;

	// Token: 0x0400473C RID: 18236
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isDirty;

	// Token: 0x0400473D RID: 18237
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor qualitycolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x0400473E RID: 18238
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat qualityfillFormatter = new CachedStringFormatterFloat(null);

	// Token: 0x0400473F RID: 18239
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt qualityFormatter = new CachedStringFormatterInt();

	// Token: 0x04004740 RID: 18240
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor itemicontintcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04004741 RID: 18241
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemDisplayEntry itemDisplayEntry;
}
