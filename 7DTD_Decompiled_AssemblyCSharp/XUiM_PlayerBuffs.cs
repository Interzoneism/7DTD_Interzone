using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x02000EE5 RID: 3813
public class XUiM_PlayerBuffs : XUiModel
{
	// Token: 0x0600784A RID: 30794 RVA: 0x00310218 File Offset: 0x0030E418
	public static List<string> GetInfoFromBuffNotification(EntityUINotification notification, BuffValue overridenBuff, XUi _xui)
	{
		EntityPlayerLocal entityPlayer = _xui.playerUI.entityPlayer;
		List<string> list = new List<string>();
		string buffDisplayInfo = XUiM_PlayerBuffs.GetBuffDisplayInfo(notification, null);
		if (buffDisplayInfo != null)
		{
			list.Add(XUiM_PlayerBuffs.StringFormatHandler(buffDisplayInfo, XUiM_PlayerBuffs.lblDuration));
		}
		BuffValue buff = notification.Buff;
		if (buff.BuffClass != null && buff.BuffClass.Effects != null && buff.BuffClass.Effects.EffectGroups != null && buff.BuffClass.Effects.EffectGroups.Count > 0)
		{
			for (int i = 0; i < buff.BuffClass.Effects.EffectGroups.Count; i++)
			{
				for (int j = 0; j < buff.BuffClass.Effects.EffectGroups[i].PassiveEffects.Count; j++)
				{
					PassiveEffect passiveEffect = buff.BuffClass.Effects.EffectGroups[i].PassiveEffects[j];
					if (passiveEffect != null)
					{
						passiveEffect.AddColoredInfoStrings(ref list, -1f);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x0600784B RID: 30795 RVA: 0x00310330 File Offset: 0x0030E530
	public static string GetInfoFromBuff(EntityPlayerLocal _localPlayer, EntityUINotification notification, BuffValue overridenBuff)
	{
		if (!XUiM_PlayerBuffs.HasLocalizationBeenCached)
		{
			XUiM_PlayerBuffs.lblDuration = Localization.Get("xuiBuffStatDuration", false);
			XUiM_PlayerBuffs.lblTimeLeft = Localization.Get("xuiBuffStatTimeLeft", false);
			XUiM_PlayerBuffs.lblHealth = Localization.Get("xuiBuffStatHealth", false);
			XUiM_PlayerBuffs.lblStamina = Localization.Get("xuiBuffStatStamina", false);
			XUiM_PlayerBuffs.lblGassiness = Localization.Get("xuiBuffStatGassiness", false);
			XUiM_PlayerBuffs.lblSickness = Localization.Get("xuiBuffStatSickness", false);
			XUiM_PlayerBuffs.lblMovementSpeed = Localization.Get("xuiBuffStatMovementSpeed", false);
			XUiM_PlayerBuffs.lblWellness = Localization.Get("xuiBuffStatWellness", false);
			XUiM_PlayerBuffs.lblCoreTemp = Localization.Get("xuiBuffStatCoreTemp", false);
			XUiM_PlayerBuffs.lblHydration = Localization.Get("lblHydration", false);
			XUiM_PlayerBuffs.lblFullness = Localization.Get("lblFullness", false);
			XUiM_PlayerBuffs.lblStatModifierValueOTForSeconds = Localization.Get("xuiBuffStatModifierValueOTForSeconds", false);
			XUiM_PlayerBuffs.lblStatModifierValueOT = Localization.Get("xuiBuffStatModifierValueOT", false);
			XUiM_PlayerBuffs.lblStatModifierSetValueForSecondsInc = Localization.Get("xuiBuffStatModifierSetValueForSecondsInc", false);
			XUiM_PlayerBuffs.lblStatModifierSetValueForSecondsDec = Localization.Get("xuiBuffStatModifierSetValueForSecondsDec", false);
			XUiM_PlayerBuffs.lblStatModifierSetValueInc = Localization.Get("xuiBuffStatModifierSetValueInc", false);
			XUiM_PlayerBuffs.lblStatModifierSetValueDec = Localization.Get("xuiBuffStatModifierSetValueDec", false);
			XUiM_PlayerBuffs.lblStatModifierMaxForSeconds = Localization.Get("xuiBuffStatModifierMaxForSeconds", false);
			XUiM_PlayerBuffs.lblStatModifierMax = Localization.Get("xuiBuffStatModifierMax", false);
			XUiM_PlayerBuffs.lblHours = Localization.Get("xuiBuffStatHours", false);
			XUiM_PlayerBuffs.lblDays = Localization.Get("xuiBuffStatDays", false);
			XUiM_PlayerBuffs.HasLocalizationBeenCached = true;
		}
		StringBuilder stringBuilder = new StringBuilder();
		BuffValue buff = notification.Buff;
		List<string> list = new List<string>();
		if (buff.BuffClass != null && buff.BuffClass.Effects != null && buff.BuffClass.Effects.EffectGroups != null && buff.BuffClass.Effects.EffectGroups.Count > 0)
		{
			for (int i = 0; i < buff.BuffClass.Effects.EffectGroups.Count; i++)
			{
				for (int j = 0; j < buff.BuffClass.Effects.EffectGroups[i].PassiveEffects.Count; j++)
				{
					PassiveEffect passiveEffect = buff.BuffClass.Effects.EffectGroups[i].PassiveEffects[j];
					if (passiveEffect != null)
					{
						passiveEffect.AddColoredInfoStrings(ref list, buff.DurationInSeconds);
					}
				}
			}
		}
		string newValue = Utils.ColorToHex(new Color32(222, 206, 163, byte.MaxValue));
		for (int k = 0; k < list.Count; k++)
		{
			stringBuilder.Append(list[k]);
		}
		return stringBuilder.ToString().Replace("REPLACE_COLOR", newValue);
	}

	// Token: 0x0600784C RID: 30796 RVA: 0x003105E0 File Offset: 0x0030E7E0
	public static string GetTimeString(float currentTime)
	{
		int num = (int)Math.Floor((double)(currentTime / 3600f));
		int num2 = (int)Math.Floor((double)((currentTime - (float)(num * 3600)) / 60f));
		int num3 = (int)Math.Floor((double)(currentTime % 60f));
		if (num3 == 0 && num2 == 0 && num == 0)
		{
			return string.Format("<1{0}", Localization.Get("timeAbbreviationMinutes", false));
		}
		return string.Format("{0}{1}{2}", (num > 0) ? string.Format("{0}{1} ", num, Localization.Get("timeAbbreviationHours", false)) : "", (num2 > 0) ? string.Format("{0}{1} ", num2, Localization.Get("timeAbbreviationMinutes", false)) : "", (num3 > 0) ? string.Format("{0}{1} ", num3, Localization.Get("timeAbbreviationSeconds", false)) : "");
	}

	// Token: 0x0600784D RID: 30797 RVA: 0x003106BE File Offset: 0x0030E8BE
	public static string GetBuffTimerDurationString(float duration)
	{
		return XUiM_PlayerBuffs.GetTimeString((float)Mathf.FloorToInt(duration * 20f));
	}

	// Token: 0x0600784E RID: 30798 RVA: 0x003106D2 File Offset: 0x0030E8D2
	public static string GetBuffTimerTimeLeftString(float duration, float maxDuration)
	{
		return XUiM_PlayerBuffs.GetTimeString((float)((int)((maxDuration - duration) * 20f)));
	}

	// Token: 0x0600784F RID: 30799 RVA: 0x003106E4 File Offset: 0x0030E8E4
	public static string GetBuffTimeLeftString(BuffValue _buff)
	{
		if (_buff.BuffClass == null)
		{
			return "";
		}
		if (_buff.BuffClass.DurationMax <= 0f)
		{
			return "";
		}
		int num = (int)(_buff.BuffClass.DurationMax * (float)((_buff.BuffClass.StackType == BuffEffectStackTypes.Duration) ? _buff.StackEffectMultiplier : 1) - _buff.DurationInSeconds + 0.9f);
		int num2 = num / 60;
		int num3 = num2 / 60;
		if (num3 > 0)
		{
			return string.Format("{0}H", num3);
		}
		if (num2 > 0)
		{
			return string.Format("{0}M", num2);
		}
		return string.Format("{0}S", num);
	}

	// Token: 0x06007850 RID: 30800 RVA: 0x00310790 File Offset: 0x0030E990
	public static string FormatWorldTimeString(int duration)
	{
		int num = duration / 24000;
		duration -= num * 24000;
		if (num > 0)
		{
			if (duration >= 23000)
			{
				return string.Format("{0}.{1} {2}", num, 9, XUiM_PlayerBuffs.lblDays);
			}
			return string.Format("{0}.{1} {2}", num, (int)Mathf.Floor((float)duration / 24000f * 10f + 0.5f), XUiM_PlayerBuffs.lblDays);
		}
		else
		{
			int num2 = duration / 1000;
			duration -= num2 * 1000;
			if (duration >= 900)
			{
				return string.Format("{0}.{1} {2}", num2, 9, XUiM_PlayerBuffs.lblHours);
			}
			return string.Format("{0}.{1} {2}", num2, (int)Mathf.Floor((float)duration / 1000f * 10f + 0.5f), XUiM_PlayerBuffs.lblHours);
		}
	}

	// Token: 0x06007851 RID: 30801 RVA: 0x0031087C File Offset: 0x0030EA7C
	public static string GetBuffDisplayInfo(EntityUINotification notification, BuffValue overridenBuff = null)
	{
		if (notification.Buff != null)
		{
			BuffValue buff = notification.Buff;
			string buffTimeLeftString = XUiM_PlayerBuffs.GetBuffTimeLeftString(buff);
			if (buffTimeLeftString != null && buff.BuffClass.DurationMax != 0f)
			{
				return buffTimeLeftString;
			}
			if (notification.DisplayMode == EnumEntityUINotificationDisplayMode.IconPlusCurrentValue)
			{
				string units = notification.Units;
				if (units == "%")
				{
					return ((int)(notification.CurrentValue * 100f)).ToString() + "%";
				}
				if (units == "°")
				{
					return ValueDisplayFormatters.Temperature(notification.CurrentValue, 0);
				}
				if (!(units == "cvar"))
				{
					if (units == "duration")
					{
						return XUiM_PlayerBuffs.GetCVarValueAsTimeString(notification.Buff.BuffClass.DurationMax - notification.Buff.DurationInSeconds);
					}
					if (notification.Buff.BuffClass.DisplayValueKey == null)
					{
						return ((int)notification.CurrentValue).ToString();
					}
					if (notification.Buff.BuffClass.DisplayValueFormat == BuffClass.CVarDisplayFormat.Time)
					{
						return string.Format(Localization.Get(notification.Buff.BuffClass.DisplayValueKey, false), XUiM_PlayerBuffs.GetCVarValueAsTimeString(notification.CurrentValue));
					}
					return string.Format(Localization.Get(notification.Buff.BuffClass.DisplayValueKey, false), notification.CurrentValue);
				}
				else if (notification.Buff.BuffClass.DisplayValueKey != null)
				{
					if (notification.Buff.BuffClass.DisplayValueFormat == BuffClass.CVarDisplayFormat.Time)
					{
						return string.Format(Localization.Get(notification.Buff.BuffClass.DisplayValueKey, false), XUiM_PlayerBuffs.GetCVarValueAsTimeString(notification.CurrentValue));
					}
					return string.Format(Localization.Get(notification.Buff.BuffClass.DisplayValueKey, false), notification.CurrentValue);
				}
				else
				{
					if (notification.Buff.BuffClass.DisplayValueFormat == BuffClass.CVarDisplayFormat.Time)
					{
						return XUiM_PlayerBuffs.GetCVarValueAsTimeString(notification.CurrentValue);
					}
					return ((int)notification.CurrentValue).ToString();
				}
			}
		}
		return "";
	}

	// Token: 0x06007852 RID: 30802 RVA: 0x00310A84 File Offset: 0x0030EC84
	public static string GetCVarValueAsTimeString(float cvarValue)
	{
		if (cvarValue == 0f)
		{
			return "";
		}
		if (XUiM_PlayerBuffs.hourAbbrev == null)
		{
			XUiM_PlayerBuffs.hourAbbrev = Localization.Get("timeAbbreviationHours", false);
			XUiM_PlayerBuffs.minuteAbbrev = Localization.Get("timeAbbreviationMinutes", false);
			XUiM_PlayerBuffs.secondAbbrev = Localization.Get("timeAbbreviationSeconds", false);
		}
		int num = (int)Math.Floor((double)(cvarValue / 3600f));
		int num2 = (int)Math.Floor((double)((cvarValue - (float)(num * 3600)) / 60f));
		int num3 = (int)Math.Floor((double)(cvarValue % 60f));
		if (num > 0)
		{
			if (num >= 5 || num2 == 0)
			{
				return string.Format("{0}{1}", num, XUiM_PlayerBuffs.hourAbbrev);
			}
			return string.Format("{0}{2} {1}{3}", new object[]
			{
				num,
				num2,
				XUiM_PlayerBuffs.hourAbbrev,
				XUiM_PlayerBuffs.minuteAbbrev
			});
		}
		else
		{
			if (num2 <= 0)
			{
				return string.Format("{0}{1}", num3, XUiM_PlayerBuffs.secondAbbrev);
			}
			if (num2 >= 5 || num3 == 0)
			{
				return string.Format("{0}{1}", num2, XUiM_PlayerBuffs.minuteAbbrev);
			}
			return string.Format("{0}{2} {1}{3}", new object[]
			{
				num2,
				num3,
				XUiM_PlayerBuffs.minuteAbbrev,
				XUiM_PlayerBuffs.secondAbbrev
			});
		}
	}

	// Token: 0x06007853 RID: 30803 RVA: 0x00310BCC File Offset: 0x0030EDCC
	public static string ConvertToTimeString(float timeSeconds)
	{
		if (timeSeconds == 0f)
		{
			return "";
		}
		if (XUiM_PlayerBuffs.hourAbbrev == null)
		{
			XUiM_PlayerBuffs.hourAbbrev = Localization.Get("timeAbbreviationHours", false);
			XUiM_PlayerBuffs.minuteAbbrev = Localization.Get("timeAbbreviationMinutes", false);
			XUiM_PlayerBuffs.secondAbbrev = Localization.Get("timeAbbreviationSeconds", false);
		}
		int num = (int)Math.Floor((double)(timeSeconds / 3600f));
		int num2 = (int)Math.Floor((double)((timeSeconds - (float)(num * 3600)) / 60f));
		int num3 = (int)Math.Floor((double)(timeSeconds % 60f));
		if (num > 0)
		{
			if (num2 == 0)
			{
				return string.Format("{0}{1}", num, XUiM_PlayerBuffs.hourAbbrev);
			}
			return string.Format("{0}{2} {1}{3}", new object[]
			{
				num,
				num2,
				XUiM_PlayerBuffs.hourAbbrev,
				XUiM_PlayerBuffs.minuteAbbrev
			});
		}
		else
		{
			if (num2 <= 0)
			{
				return string.Format("{0}{1}", num3, XUiM_PlayerBuffs.secondAbbrev);
			}
			if (num3 == 0)
			{
				return string.Format("{0}{1}", num2, XUiM_PlayerBuffs.minuteAbbrev);
			}
			return string.Format("{0}{2} {1}{3}", new object[]
			{
				num2,
				num3,
				XUiM_PlayerBuffs.minuteAbbrev,
				XUiM_PlayerBuffs.secondAbbrev
			});
		}
	}

	// Token: 0x06007854 RID: 30804 RVA: 0x0011007A File Offset: 0x0010E27A
	[PublicizedFrom(EAccessModifier.Private)]
	public static string StringFormatHandler(string title, object value)
	{
		return string.Format("{0}: [REPLACE_COLOR]{1}[-]\n", title, value);
	}

	// Token: 0x04005BA2 RID: 23458
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblDuration;

	// Token: 0x04005BA3 RID: 23459
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblTimeLeft;

	// Token: 0x04005BA4 RID: 23460
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblHealth;

	// Token: 0x04005BA5 RID: 23461
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblStamina;

	// Token: 0x04005BA6 RID: 23462
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblGassiness;

	// Token: 0x04005BA7 RID: 23463
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblSickness;

	// Token: 0x04005BA8 RID: 23464
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblMovementSpeed;

	// Token: 0x04005BA9 RID: 23465
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblWellness;

	// Token: 0x04005BAA RID: 23466
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblCoreTemp;

	// Token: 0x04005BAB RID: 23467
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblHydration;

	// Token: 0x04005BAC RID: 23468
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblFullness;

	// Token: 0x04005BAD RID: 23469
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblStatModifierValueOTForSeconds;

	// Token: 0x04005BAE RID: 23470
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblStatModifierValueOT;

	// Token: 0x04005BAF RID: 23471
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblStatModifierSetValueForSecondsInc;

	// Token: 0x04005BB0 RID: 23472
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblStatModifierSetValueForSecondsDec;

	// Token: 0x04005BB1 RID: 23473
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblStatModifierSetValueInc;

	// Token: 0x04005BB2 RID: 23474
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblStatModifierSetValueDec;

	// Token: 0x04005BB3 RID: 23475
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblStatModifierMaxForSeconds;

	// Token: 0x04005BB4 RID: 23476
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblStatModifierMax;

	// Token: 0x04005BB5 RID: 23477
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblHours;

	// Token: 0x04005BB6 RID: 23478
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblDays;

	// Token: 0x04005BB7 RID: 23479
	public static bool HasLocalizationBeenCached;

	// Token: 0x04005BB8 RID: 23480
	[PublicizedFrom(EAccessModifier.Private)]
	public static string minuteAbbrev;

	// Token: 0x04005BB9 RID: 23481
	[PublicizedFrom(EAccessModifier.Private)]
	public static string secondAbbrev;

	// Token: 0x04005BBA RID: 23482
	[PublicizedFrom(EAccessModifier.Private)]
	public static string hourAbbrev;
}
