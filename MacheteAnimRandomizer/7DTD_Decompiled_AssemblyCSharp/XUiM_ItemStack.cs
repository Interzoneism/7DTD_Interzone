using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

// Token: 0x02000EE0 RID: 3808
public class XUiM_ItemStack : XUiModel
{
	// Token: 0x06007819 RID: 30745 RVA: 0x0030E498 File Offset: 0x0030C698
	public static bool HasItemStats(ItemStack itemStack)
	{
		if (itemStack.itemValue.ItemClass == null)
		{
			return false;
		}
		if (itemStack.itemValue.ItemClass.IsBlock())
		{
			return Block.list[itemStack.itemValue.type].DisplayType != "";
		}
		return itemStack.itemValue.ItemClass.DisplayType != "";
	}

	// Token: 0x0600781A RID: 30746 RVA: 0x0030E504 File Offset: 0x0030C704
	[PublicizedFrom(EAccessModifier.Private)]
	public static string BuffActionStrings(ItemAction itemAction, List<string> stringList)
	{
		if (itemAction.BuffActions == null || itemAction.BuffActions.Count == 0)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < itemAction.BuffActions.Count; i++)
		{
			BuffClass buff = BuffManager.GetBuff(itemAction.BuffActions[i]);
			if (buff != null && !string.IsNullOrEmpty(buff.Name))
			{
				stringList.Add(XUiM_ItemStack.StringFormatHandler(Localization.Get("lblEffect", false), string.Format("{0}", buff.Name)));
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600781B RID: 30747 RVA: 0x0011011A File Offset: 0x0010E31A
	[PublicizedFrom(EAccessModifier.Private)]
	public static string getColoredItemStat(string _title, float _value)
	{
		if (_value > 0f)
		{
			return string.Format("{0}: [00ff00]+{1}[-]", _title, _value.ToCultureInvariantString());
		}
		if (_value < 0f)
		{
			return string.Format("{0}: [ff0000]{1}[-]", _title, _value.ToCultureInvariantString());
		}
		return "";
	}

	// Token: 0x0600781C RID: 30748 RVA: 0x0030E598 File Offset: 0x0030C798
	[PublicizedFrom(EAccessModifier.Private)]
	public static string getColoredItemStatPercentage(string _title, float _value)
	{
		if (_value > 0f)
		{
			return string.Format("{0}: [00ff00]+{1}%[-]", _title, _value.ToCultureInvariantString("0.0"));
		}
		if (_value < 0f)
		{
			return string.Format("{0}: [ff0000]{1}%[-]", _title, _value.ToCultureInvariantString("0.0"));
		}
		return "";
	}

	// Token: 0x0600781D RID: 30749 RVA: 0x0011007A File Offset: 0x0010E27A
	[PublicizedFrom(EAccessModifier.Private)]
	public static string StringFormatHandler(string title, object value)
	{
		return string.Format("{0}: [REPLACE_COLOR]{1}[-]\n", title, value);
	}

	// Token: 0x0600781E RID: 30750 RVA: 0x0030E5E8 File Offset: 0x0030C7E8
	public static string GetStatItemValueTextWithModInfo(ItemStack itemStack, EntityPlayer player, DisplayInfoEntry infoEntry)
	{
		FastTags<TagGroup.Global> tags = infoEntry.TagsSet ? infoEntry.Tags : (XUiM_ItemStack.primaryFastTags | XUiM_ItemStack.physicalDamageFastTags);
		MinEventParams.CachedEventParam.ItemValue = itemStack.itemValue;
		MinEventParams.CachedEventParam.Seed = (int)itemStack.itemValue.Seed;
		float num;
		float num2;
		if (infoEntry.CustomName == "")
		{
			num = EffectManager.GetValue(infoEntry.StatType, itemStack.itemValue, 0f, player, null, tags, false, false, false, false, true, 1, false, false);
			num2 = EffectManager.GetValue(infoEntry.StatType, itemStack.itemValue, 0f, player, null, tags, false, false, false, false, true, 1, true, false);
		}
		else
		{
			num = XUiM_ItemStack.GetCustomValue(infoEntry, itemStack.itemValue, false);
			num2 = XUiM_ItemStack.GetCustomValue(infoEntry, itemStack.itemValue, true);
		}
		if (((infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal1 || infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal2 || infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Percent) && Mathf.Floor(num2 * 100f) != Mathf.Floor(num * 100f)) || (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Integer && Mathf.Floor(num2) != Mathf.Floor(num)))
		{
			if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Percent)
			{
				num *= 100f;
				num = Mathf.Floor(num);
				num2 *= 100f;
				num2 = Mathf.Floor(num2);
				if (infoEntry.ShowInverted)
				{
					num -= 100f;
					num2 -= 100f;
				}
				float num3 = num2 - num;
				bool flag = num3 > 0f;
				bool flag2 = infoEntry.NegativePreferred ? (!flag) : flag;
				string text = (num2 > 0f && infoEntry.DisplayLeadingPlus) ? "+" : "";
				return string.Concat(new string[]
				{
					text,
					num2.ToString(),
					"% (",
					flag2 ? "[00FF00]" : "[FF0000]",
					flag ? "+" : "",
					num3.ToString(),
					"%[-])"
				});
			}
			if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal1)
			{
				num2 *= 10f;
				num2 = Mathf.Floor(num2);
				num2 /= 10f;
				num *= 10f;
				num = Mathf.Floor(num);
				num /= 10f;
			}
			else if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal2)
			{
				num2 *= 100f;
				num2 = Mathf.Floor(num2);
				num2 /= 100f;
				num *= 100f;
				num = Mathf.Floor(num);
				num /= 100f;
			}
			else
			{
				num2 = Mathf.Floor(num2);
				num = Mathf.Floor(num);
			}
			if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Time)
			{
				float num4 = num2 - num;
				bool flag3 = num4 > 0f;
				bool flag4 = infoEntry.NegativePreferred ? (!flag3) : flag3;
				return string.Concat(new string[]
				{
					XUiM_PlayerBuffs.GetCVarValueAsTimeString(num2),
					" (",
					flag4 ? "[00FF00]" : "[FF0000]",
					flag3 ? "+" : "",
					XUiM_PlayerBuffs.GetCVarValueAsTimeString(num4),
					"[-])"
				});
			}
			if (infoEntry.ShowInverted)
			{
				num -= 1f;
				num2 -= 1f;
			}
			float num5 = num2 - num;
			bool flag5 = num5 > 0f;
			bool flag6 = infoEntry.NegativePreferred ? (!flag5) : flag5;
			string text2 = (num > 0f && infoEntry.DisplayLeadingPlus) ? "+" : "";
			return string.Concat(new string[]
			{
				text2,
				num2.ToString(),
				" (",
				flag6 ? "[00FF00]" : "[FF0000]",
				flag5 ? "+" : "",
				num5.ToString("0.##"),
				"[-])"
			});
		}
		else
		{
			if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Percent)
			{
				num2 *= 100f;
				num2 = Mathf.Floor(num2);
				if (infoEntry.ShowInverted)
				{
					num2 -= 100f;
				}
				return ((num2 > 0f && infoEntry.DisplayLeadingPlus) ? "+" : "") + num2.ToString("0") + "%";
			}
			if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal1)
			{
				num2 *= 10f;
				num2 = Mathf.Floor(num2);
				num2 /= 10f;
			}
			else if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal2)
			{
				num2 *= 100f;
				num2 = Mathf.Floor(num2);
				num2 /= 100f;
			}
			else
			{
				num2 = Mathf.Floor(num2);
			}
			if (infoEntry.ShowInverted)
			{
				num2 -= 1f;
			}
			if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Time)
			{
				return XUiM_PlayerBuffs.GetCVarValueAsTimeString(num2);
			}
			return ((num2 > 0f && infoEntry.DisplayLeadingPlus) ? "+" : "") + num2.ToString("0.##");
		}
	}

	// Token: 0x0600781F RID: 30751 RVA: 0x0030EA94 File Offset: 0x0030CC94
	public static string GetStatItemValueTextWithModColoring(ItemValue itemValue, EntityPlayer player, DisplayInfoEntry infoEntry)
	{
		FastTags<TagGroup.Global> tags = infoEntry.TagsSet ? infoEntry.Tags : (XUiM_ItemStack.primaryFastTags | XUiM_ItemStack.physicalDamageFastTags);
		MinEventParams.CachedEventParam.ItemValue = itemValue;
		MinEventParams.CachedEventParam.Seed = (int)itemValue.Seed;
		float num;
		float num2;
		if (infoEntry.CustomName == "")
		{
			MinEventParams.CachedEventParam.ItemValue = itemValue;
			MinEventParams.CachedEventParam.Seed = (int)itemValue.Seed;
			num = EffectManager.GetValue(infoEntry.StatType, itemValue, 0f, player, null, tags, false, false, false, false, true, 1, false, false);
			num2 = EffectManager.GetValue(infoEntry.StatType, itemValue, 0f, player, null, tags, false, false, false, false, true, 1, true, false);
		}
		else
		{
			num = XUiM_ItemStack.GetCustomValue(infoEntry, itemValue, false);
			num2 = XUiM_ItemStack.GetCustomValue(infoEntry, itemValue, true);
		}
		if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Bool)
		{
			if (num != num2)
			{
				bool flag = num2 > num;
				return ((infoEntry.NegativePreferred ? (!flag) : flag) ? "[00FF00]" : "[FF0000]") + XUiM_ItemStack.ShowLocalizedBool(Convert.ToBoolean(num2)) + "%[-]";
			}
			return XUiM_ItemStack.ShowLocalizedBool(Convert.ToBoolean(num));
		}
		else if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Time)
		{
			if (num != num2)
			{
				if (infoEntry.ShowInverted)
				{
					num -= 1f;
					num2 -= 1f;
				}
				bool flag2 = num2 - num > 0f;
				return ((infoEntry.NegativePreferred ? (!flag2) : flag2) ? "[00FF00]" : "[FF0000]") + XUiM_PlayerBuffs.GetCVarValueAsTimeString(num) + "[-])";
			}
			return XUiM_PlayerBuffs.GetCVarValueAsTimeString(num);
		}
		else if (((infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal1 || infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal2 || infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Percent) && Mathf.Floor(num2 * 100f) != Mathf.Floor(num * 100f)) || (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Integer && Mathf.Floor(num2) != Mathf.Floor(num)))
		{
			if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Percent)
			{
				num *= 100f;
				num = Mathf.Floor(num);
				num2 *= 100f;
				num2 = Mathf.Floor(num2);
				if (infoEntry.ShowInverted)
				{
					num -= 100f;
					num2 -= 100f;
				}
				bool flag3 = num2 - num > 0f;
				bool flag4 = infoEntry.NegativePreferred ? (!flag3) : flag3;
				string str = (num2 > 0f && infoEntry.DisplayLeadingPlus) ? "+" : "";
				return (flag4 ? "[00FF00]" : "[FF0000]") + str + num2.ToString() + "%[-]";
			}
			if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal1)
			{
				num2 *= 10f;
				num2 = Mathf.Floor(num2);
				num2 /= 10f;
				num *= 10f;
				num = Mathf.Floor(num);
				num /= 10f;
			}
			else if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal2)
			{
				num2 *= 100f;
				num2 = Mathf.Floor(num2);
				num2 /= 100f;
				num *= 100f;
				num = Mathf.Floor(num);
				num /= 100f;
			}
			else
			{
				num2 = Mathf.Floor(num2);
				num = Mathf.Floor(num);
			}
			if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Time)
			{
				bool flag5 = num2 - num > 0f;
				return ((infoEntry.NegativePreferred ? (!flag5) : flag5) ? "[00FF00]" : "[FF0000]") + XUiM_PlayerBuffs.GetCVarValueAsTimeString(num2) + "[-]";
			}
			if (infoEntry.ShowInverted)
			{
				num -= 1f;
				num2 -= 1f;
			}
			bool flag6 = num2 - num > 0f;
			bool flag7 = infoEntry.NegativePreferred ? (!flag6) : flag6;
			string str2 = (num > 0f && infoEntry.DisplayLeadingPlus) ? "+" : "";
			return (flag7 ? "[00FF00]" : "[FF0000]") + str2 + num2.ToString() + "[-]";
		}
		else
		{
			if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Percent)
			{
				num2 *= 100f;
				num2 = Mathf.Floor(num2);
				if (infoEntry.ShowInverted)
				{
					num2 -= 100f;
				}
				return ((num2 > 0f && infoEntry.DisplayLeadingPlus) ? "+" : "") + num2.ToString("0") + "%";
			}
			if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal1)
			{
				num2 *= 10f;
				num2 = Mathf.Floor(num2);
				num2 /= 10f;
			}
			else if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal2)
			{
				num2 *= 100f;
				num2 = Mathf.Floor(num2);
				num2 /= 100f;
			}
			else
			{
				num2 = Mathf.Floor(num2);
			}
			if (infoEntry.ShowInverted)
			{
				num2 -= 1f;
			}
			if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Time)
			{
				return XUiM_PlayerBuffs.GetCVarValueAsTimeString(num2);
			}
			return ((num2 > 0f && infoEntry.DisplayLeadingPlus) ? "+" : "") + num2.ToString("0.##");
		}
	}

	// Token: 0x06007820 RID: 30752 RVA: 0x0030EF3C File Offset: 0x0030D13C
	public static string GetStatItemValueTextWithCompareInfo(ItemValue itemValue, ItemValue compareValue, EntityPlayer player, DisplayInfoEntry infoEntry, bool flipCompare = false, bool useMods = true)
	{
		FastTags<TagGroup.Global> tags = infoEntry.TagsSet ? infoEntry.Tags : (XUiM_ItemStack.primaryFastTags | XUiM_ItemStack.physicalDamageFastTags);
		if (compareValue.IsEmpty() || compareValue.Equals(itemValue))
		{
			return XUiM_ItemStack.GetStatItemValueTextWithModColoring(itemValue, player, infoEntry);
		}
		float num;
		float num2;
		if (infoEntry.CustomName == "")
		{
			MinEventParams.CachedEventParam.ItemValue = itemValue;
			MinEventParams.CachedEventParam.Seed = (int)itemValue.Seed;
			num = EffectManager.GetValue(infoEntry.StatType, itemValue, 0f, player, null, tags, false, false, false, false, true, 1, useMods, false);
			num2 = EffectManager.GetValue(infoEntry.StatType, compareValue, 0f, player, null, tags, false, false, false, false, true, 1, useMods, false);
		}
		else
		{
			num = XUiM_ItemStack.GetCustomValue(infoEntry, itemValue, useMods);
			num2 = XUiM_ItemStack.GetCustomValue(infoEntry, compareValue, useMods);
		}
		if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Bool)
		{
			if (!compareValue.IsEmpty() && num != num2)
			{
				bool flag = num2 > num;
				bool flag2 = infoEntry.NegativePreferred ? (!flag) : flag;
				return string.Concat(new string[]
				{
					XUiM_ItemStack.ShowLocalizedBool(Convert.ToBoolean(num)),
					" (",
					flag2 ? "[00FF00]" : "[FF0000]",
					XUiM_ItemStack.ShowLocalizedBool(Convert.ToBoolean(num2)),
					"[-])"
				});
			}
			return XUiM_ItemStack.ShowLocalizedBool(Convert.ToBoolean(num));
		}
		else if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Time)
		{
			if (!compareValue.IsEmpty() && num != num2)
			{
				if (infoEntry.ShowInverted)
				{
					num -= 1f;
					num2 -= 1f;
				}
				float num3 = num2 - num;
				if (flipCompare)
				{
					num3 = num - num2;
				}
				bool flag3 = num3 > 0f;
				bool flag4 = infoEntry.NegativePreferred ? (!flag3) : flag3;
				return string.Concat(new string[]
				{
					XUiM_PlayerBuffs.GetCVarValueAsTimeString(num),
					" (",
					flag4 ? "[00FF00]" : "[FF0000]",
					flag3 ? "+" : "-",
					XUiM_PlayerBuffs.GetCVarValueAsTimeString(Mathf.Abs(num3)),
					"[-])"
				});
			}
			return XUiM_PlayerBuffs.GetCVarValueAsTimeString(num);
		}
		else if (!compareValue.IsEmpty() && (((infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal1 || infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal2 || infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Percent) && Mathf.Floor(num2 * 100f) != Mathf.Floor(num * 100f)) || (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Integer && Mathf.Floor(num2) != Mathf.Floor(num))))
		{
			if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Percent)
			{
				num *= 100f;
				num = Mathf.Floor(num);
				num2 *= 100f;
				num2 = Mathf.Floor(num2);
				if (infoEntry.ShowInverted)
				{
					num -= 100f;
					num2 -= 100f;
				}
				float num4 = num2 - num;
				if (flipCompare)
				{
					num4 = num - num2;
				}
				bool flag5 = num4 > 0f;
				bool flag6 = infoEntry.NegativePreferred ? (!flag5) : flag5;
				string text = (num > 0f && infoEntry.DisplayLeadingPlus) ? "+" : "";
				return string.Concat(new string[]
				{
					text,
					num.ToString(),
					"% (",
					flag6 ? "[00FF00]" : "[FF0000]",
					flag5 ? "+" : "",
					num4.ToString(),
					"%[-])"
				});
			}
			if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal1)
			{
				num2 *= 10f;
				num2 = Mathf.Floor(num2);
				num2 /= 10f;
				num *= 10f;
				num = Mathf.Floor(num);
				num /= 10f;
			}
			else if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal2)
			{
				num2 *= 100f;
				num2 = Mathf.Floor(num2);
				num2 /= 100f;
				num *= 100f;
				num = Mathf.Floor(num);
				num /= 100f;
			}
			else
			{
				num2 = Mathf.Floor(num2);
				num = Mathf.Floor(num);
			}
			if (infoEntry.ShowInverted)
			{
				num -= 1f;
				num2 -= 1f;
			}
			float num5 = num2 - num;
			if (flipCompare)
			{
				num5 = num - num2;
			}
			bool flag7 = num5 > 0f;
			bool flag8 = infoEntry.NegativePreferred ? (!flag7) : flag7;
			string text2 = (num > 0f && infoEntry.DisplayLeadingPlus) ? "+" : "";
			return string.Concat(new string[]
			{
				text2,
				num.ToString(),
				" (",
				flag8 ? "[00FF00]" : "[FF0000]",
				flag7 ? "+" : "",
				num5.ToString("0.##"),
				"[-])"
			});
		}
		else
		{
			if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Percent)
			{
				num *= 100f;
				num = Mathf.Floor(num);
				if (infoEntry.ShowInverted)
				{
					num -= 100f;
				}
				return ((num > 0f && infoEntry.DisplayLeadingPlus) ? "+" : "") + num.ToString("0") + "%";
			}
			if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal1)
			{
				num *= 10f;
				num = Mathf.Floor(num);
				num /= 10f;
			}
			else if (infoEntry.DisplayType == DisplayInfoEntry.DisplayTypes.Decimal2)
			{
				num *= 100f;
				num = Mathf.Floor(num);
				num /= 100f;
			}
			else
			{
				num = Mathf.Floor(num);
			}
			if (infoEntry.ShowInverted)
			{
				num -= 1f;
			}
			return ((num > 0f && infoEntry.DisplayLeadingPlus) ? "+" : "") + num.ToString("0.##");
		}
	}

	// Token: 0x06007821 RID: 30753 RVA: 0x0030F4BC File Offset: 0x0030D6BC
	public static string ShowLocalizedBool(bool value)
	{
		if (XUiM_ItemStack.localizedTrue == "")
		{
			XUiM_ItemStack.localizedTrue = Localization.Get("statTrue", false);
			XUiM_ItemStack.localizedFalse = Localization.Get("statFalse", false);
		}
		if (!value)
		{
			return XUiM_ItemStack.localizedFalse;
		}
		return XUiM_ItemStack.localizedTrue;
	}

	// Token: 0x06007822 RID: 30754 RVA: 0x0030F508 File Offset: 0x0030D708
	public static bool CanCompare(ItemClass item1, ItemClass item2)
	{
		if (item1 == null || item2 == null)
		{
			return false;
		}
		string displayType = item1.DisplayType;
		string displayType2 = item2.DisplayType;
		if (item1.IsBlock())
		{
			displayType = Block.list[item1.Id].DisplayType;
		}
		if (item2.IsBlock())
		{
			displayType2 = Block.list[item2.Id].DisplayType;
		}
		ItemDisplayEntry displayStatsForTag = UIDisplayInfoManager.Current.GetDisplayStatsForTag(displayType);
		ItemDisplayEntry displayStatsForTag2 = UIDisplayInfoManager.Current.GetDisplayStatsForTag(displayType2);
		return displayStatsForTag != null && displayStatsForTag2 != null && displayStatsForTag.DisplayGroup == displayStatsForTag2.DisplayGroup;
	}

	// Token: 0x06007823 RID: 30755 RVA: 0x0030F590 File Offset: 0x0030D790
	public static float GetCustomValue(DisplayInfoEntry entry, ItemValue itemValue, bool useMods)
	{
		Block block = (itemValue != null) ? itemValue.ToBlockValue().Block : null;
		if (block != null && block.SelectAlternates)
		{
			block = (block.GetAltBlockValue(itemValue.Meta).Block ?? block);
		}
		string customName = entry.CustomName;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(customName);
		if (num <= 2273019518U)
		{
			if (num <= 1180197617U)
			{
				if (num != 470662067U)
				{
					if (num != 944097337U)
					{
						if (num == 1180197617U)
						{
							if (customName == "Mass")
							{
								if (block != null)
								{
									return (float)block.blockMaterial.Mass.Value;
								}
								goto IL_468;
							}
						}
					}
					else if (customName == "RequiredPower")
					{
						BlockPowered blockPowered = block as BlockPowered;
						if (blockPowered != null)
						{
							return (float)blockPowered.RequiredPower;
						}
						goto IL_468;
					}
				}
				else if (customName == "Explosion.RadiusEntities")
				{
					if (block == null)
					{
						goto IL_468;
					}
					BlockMine blockMine = block as BlockMine;
					if (blockMine != null)
					{
						return (float)blockMine.Explosion.EntityRadius;
					}
					BlockCarExplode blockCarExplode = block as BlockCarExplode;
					if (blockCarExplode != null)
					{
						return (float)blockCarExplode.Explosion.EntityRadius;
					}
					BlockCarExplodeLoot blockCarExplodeLoot = block as BlockCarExplodeLoot;
					if (blockCarExplodeLoot != null)
					{
						return (float)blockCarExplodeLoot.Explosion.EntityRadius;
					}
					goto IL_468;
				}
			}
			else if (num != 1509954053U)
			{
				if (num != 2218964596U)
				{
					if (num == 2273019518U)
					{
						if (customName == "Explosion.RadiusBlocks")
						{
							if (block == null)
							{
								goto IL_468;
							}
							BlockMine blockMine2 = block as BlockMine;
							if (blockMine2 != null)
							{
								return blockMine2.Explosion.BlockRadius;
							}
							BlockCarExplode blockCarExplode2 = block as BlockCarExplode;
							if (blockCarExplode2 != null)
							{
								return blockCarExplode2.Explosion.BlockRadius;
							}
							BlockCarExplodeLoot blockCarExplodeLoot2 = block as BlockCarExplodeLoot;
							if (blockCarExplodeLoot2 != null)
							{
								return blockCarExplodeLoot2.Explosion.BlockRadius;
							}
							goto IL_468;
						}
					}
				}
				else if (customName == "Explosion.EntityDamage")
				{
					if (block == null)
					{
						goto IL_468;
					}
					BlockMine blockMine3 = block as BlockMine;
					if (blockMine3 != null)
					{
						return blockMine3.Explosion.EntityDamage;
					}
					BlockCarExplode blockCarExplode3 = block as BlockCarExplode;
					if (blockCarExplode3 != null)
					{
						return blockCarExplode3.Explosion.EntityDamage;
					}
					BlockCarExplodeLoot blockCarExplodeLoot3 = block as BlockCarExplodeLoot;
					if (blockCarExplodeLoot3 != null)
					{
						return blockCarExplodeLoot3.Explosion.EntityDamage;
					}
					goto IL_468;
				}
			}
			else if (customName == "StabilityGlue")
			{
				if (block != null)
				{
					return (float)block.blockMaterial.StabilityGlue;
				}
				goto IL_468;
			}
		}
		else if (num <= 2523555452U)
		{
			if (num != 2276058132U)
			{
				if (num != 2327184015U)
				{
					if (num == 2523555452U)
					{
						if (customName == "LightOpacity")
						{
							if (block != null)
							{
								return (float)block.lightOpacity;
							}
							goto IL_468;
						}
					}
				}
				else if (customName == "StabilitySupport")
				{
					if (block == null)
					{
						goto IL_468;
					}
					if (!block.StabilitySupport)
					{
						return 0f;
					}
					return 1f;
				}
			}
			else if (customName == "MaxDamage")
			{
				if (block != null)
				{
					return (float)block.MaxDamage;
				}
				goto IL_468;
			}
		}
		else if (num != 2927752580U)
		{
			if (num != 3550496702U)
			{
				if (num == 3706968837U)
				{
					if (customName == "ExplosionResistance")
					{
						if (block != null)
						{
							return block.GetExplosionResistance();
						}
						goto IL_468;
					}
				}
			}
			else if (customName == "Explosion.BlockDamage")
			{
				if (block == null)
				{
					goto IL_468;
				}
				BlockMine blockMine4 = block as BlockMine;
				if (blockMine4 != null)
				{
					return blockMine4.Explosion.BlockDamage;
				}
				BlockCarExplode blockCarExplode4 = block as BlockCarExplode;
				if (blockCarExplode4 != null)
				{
					return blockCarExplode4.Explosion.BlockDamage;
				}
				BlockCarExplodeLoot blockCarExplodeLoot4 = block as BlockCarExplodeLoot;
				if (blockCarExplodeLoot4 != null)
				{
					return blockCarExplodeLoot4.Explosion.BlockDamage;
				}
				goto IL_468;
			}
		}
		else if (customName == "FertileLevel")
		{
			if (block != null)
			{
				return (float)block.blockMaterial.FertileLevel;
			}
			goto IL_468;
		}
		float num2 = 0f;
		if (itemValue.ItemClass != null && itemValue.ItemClass.Effects != null && itemValue.ItemClass.Effects.EffectGroups != null)
		{
			num2 = XUiM_ItemStack.GetCustomDisplayValueForItem(itemValue, entry);
			if (useMods)
			{
				for (int i = 0; i < itemValue.Modifications.Length; i++)
				{
					if (itemValue.Modifications[i] != null && itemValue.Modifications[i].ItemClass is ItemClassModifier)
					{
						num2 += XUiM_ItemStack.GetCustomDisplayValueForItem(itemValue.Modifications[i], entry);
					}
				}
			}
		}
		return num2;
		IL_468:
		return 0f;
	}

	// Token: 0x06007824 RID: 30756 RVA: 0x0030FA0C File Offset: 0x0030DC0C
	[PublicizedFrom(EAccessModifier.Private)]
	public static float GetCustomDisplayValueForItem(ItemValue itemValue, DisplayInfoEntry entry)
	{
		XUiM_ItemStack.<>c__DisplayClass15_0 CS$<>8__locals1;
		CS$<>8__locals1.entry = entry;
		CS$<>8__locals1.newValue = 0f;
		MinEffectController effects = itemValue.ItemClass.Effects;
		List<MinEffectGroup> list = (effects != null) ? effects.EffectGroups : null;
		if (list == null)
		{
			return CS$<>8__locals1.newValue;
		}
		for (int i = 0; i < list.Count; i++)
		{
			MinEffectGroup minEffectGroup = list[i];
			MinEventParams.CachedEventParam.ItemValue = itemValue;
			MinEventParams.CachedEventParam.Seed = (int)itemValue.Seed;
			if (minEffectGroup.EffectDisplayValues.ContainsKey(CS$<>8__locals1.entry.CustomName) && minEffectGroup.EffectDisplayValues[CS$<>8__locals1.entry.CustomName].IsValid(MinEventParams.CachedEventParam))
			{
				CS$<>8__locals1.newValue += minEffectGroup.EffectDisplayValues[CS$<>8__locals1.entry.CustomName].GetValue((int)itemValue.Quality);
			}
			foreach (MinEventActionBase actionBase in minEffectGroup.GetTriggeredEffects(MinEventTypes.onSelfPrimaryActionEnd))
			{
				XUiM_ItemStack.<GetCustomDisplayValueForItem>g__AddValueForDisplayIfValid|15_0(actionBase, ref CS$<>8__locals1);
			}
			foreach (MinEventActionBase actionBase2 in minEffectGroup.GetTriggeredEffects(MinEventTypes.onSelfSecondaryActionEnd))
			{
				XUiM_ItemStack.<GetCustomDisplayValueForItem>g__AddValueForDisplayIfValid|15_0(actionBase2, ref CS$<>8__locals1);
			}
		}
		return CS$<>8__locals1.newValue;
	}

	// Token: 0x06007825 RID: 30757 RVA: 0x0030FB80 File Offset: 0x0030DD80
	public static bool CheckKnown(EntityPlayerLocal player, ItemClass itemClass, ItemValue itemValue = null)
	{
		string unlocks = itemClass.Unlocks;
		bool flag = false;
		if (unlocks != "")
		{
			if (player.GetCVar(unlocks) == 1f)
			{
				flag = true;
			}
			if (!flag)
			{
				ProgressionValue progressionValue = player.Progression.GetProgressionValue(unlocks);
				if (progressionValue != null)
				{
					if (progressionValue.ProgressionClass.IsCrafting)
					{
						if (progressionValue.Level == progressionValue.ProgressionClass.MaxLevel)
						{
							flag = true;
						}
					}
					else if (progressionValue.Level == 1)
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				Recipe recipe = CraftingManager.GetRecipe(unlocks);
				if (recipe != null && !recipe.scrapable && !recipe.wildcardForgeCategory && recipe.IsUnlocked(player))
				{
					flag = true;
				}
			}
		}
		return flag;
	}

	// Token: 0x06007828 RID: 30760 RVA: 0x0030FC54 File Offset: 0x0030DE54
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void <GetCustomDisplayValueForItem>g__AddValueForDisplayIfValid|15_0(MinEventActionBase actionBase, ref XUiM_ItemStack.<>c__DisplayClass15_0 A_1)
	{
		if (!(actionBase is MinEventActionModifyCVar))
		{
			return;
		}
		for (int i = 0; i < actionBase.Requirements.Count; i++)
		{
			if (!actionBase.Requirements[i].IsValid(MinEventParams.CachedEventParam))
			{
				return;
			}
		}
		MinEventActionModifyCVar minEventActionModifyCVar = actionBase as MinEventActionModifyCVar;
		if (minEventActionModifyCVar.cvarName == A_1.entry.CustomName && minEventActionModifyCVar.targetType == MinEventActionTargetedBase.TargetTypes.self)
		{
			A_1.newValue += minEventActionModifyCVar.GetValueForDisplay();
		}
	}

	// Token: 0x04005B95 RID: 23445
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> primaryFastTags = FastTags<TagGroup.Global>.Parse("primary");

	// Token: 0x04005B96 RID: 23446
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> physicalDamageFastTags = FastTags<TagGroup.Global>.Parse("piercing,bashing,slashing,crushing,none,corrosive");

	// Token: 0x04005B97 RID: 23447
	[PublicizedFrom(EAccessModifier.Private)]
	public static string localizedTrue = "";

	// Token: 0x04005B98 RID: 23448
	[PublicizedFrom(EAccessModifier.Private)]
	public static string localizedFalse = "";
}
