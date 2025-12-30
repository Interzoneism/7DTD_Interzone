using System;
using UnityEngine;

// Token: 0x02000EE4 RID: 3812
public class XUiM_Player : XUiModel
{
	// Token: 0x0600782D RID: 30765 RVA: 0x0030FEF6 File Offset: 0x0030E0F6
	public static int GetLevel(EntityPlayer player)
	{
		return player.Progression.GetLevel();
	}

	// Token: 0x0600782E RID: 30766 RVA: 0x0030FF03 File Offset: 0x0030E103
	public static float GetLevelPercent(EntityPlayer player)
	{
		return player.Progression.GetLevelProgressPercentage();
	}

	// Token: 0x0600782F RID: 30767 RVA: 0x0030FF10 File Offset: 0x0030E110
	public static int GetXPToNextLevel(EntityPlayer player)
	{
		return player.Progression.ExpToNextLevel;
	}

	// Token: 0x06007830 RID: 30768 RVA: 0x0030FF1D File Offset: 0x0030E11D
	public static float GetFood(EntityPlayer player)
	{
		return player.Stats.Food.Value;
	}

	// Token: 0x06007831 RID: 30769 RVA: 0x0030FF2F File Offset: 0x0030E12F
	public static float GetModifiedCurrentFood(EntityPlayer player)
	{
		return player.Stats.Food.Value + player.Buffs.GetCustomVar("$foodAmount");
	}

	// Token: 0x06007832 RID: 30770 RVA: 0x0030FF52 File Offset: 0x0030E152
	public static float GetFoodPercent(EntityPlayer player)
	{
		return 1f - player.Stats.Food.Value / player.Stats.Food.ModifiedMax;
	}

	// Token: 0x06007833 RID: 30771 RVA: 0x0030FF7B File Offset: 0x0030E17B
	public static int GetFoodMax(EntityPlayer player)
	{
		return (int)player.Stats.Food.Max;
	}

	// Token: 0x06007834 RID: 30772 RVA: 0x0030FF8E File Offset: 0x0030E18E
	public static float GetWater(EntityPlayer player)
	{
		return player.Stats.Water.Value;
	}

	// Token: 0x06007835 RID: 30773 RVA: 0x0030FFA0 File Offset: 0x0030E1A0
	public static float GetModifiedCurrentWater(EntityPlayer player)
	{
		return player.Stats.Water.Value + player.Buffs.GetCustomVar("$waterAmount");
	}

	// Token: 0x06007836 RID: 30774 RVA: 0x0030FFC3 File Offset: 0x0030E1C3
	public static float GetWaterPercent(EntityPlayer player)
	{
		return player.Stats.Water.ValuePercentUI * 100f;
	}

	// Token: 0x06007837 RID: 30775 RVA: 0x0030FFDB File Offset: 0x0030E1DB
	public static int GetWaterMax(EntityPlayer player)
	{
		return (int)player.Stats.Water.Max;
	}

	// Token: 0x06007838 RID: 30776 RVA: 0x0030FFEE File Offset: 0x0030E1EE
	public static string GetCoreTemp(EntityPlayer player)
	{
		return ValueDisplayFormatters.Temperature(70f + player.Buffs.GetCustomVar("_coretemp"), 2);
	}

	// Token: 0x06007839 RID: 30777 RVA: 0x0031000C File Offset: 0x0030E20C
	public static int GetZombieKills(EntityPlayer player)
	{
		return player.KilledZombies;
	}

	// Token: 0x0600783A RID: 30778 RVA: 0x00310014 File Offset: 0x0030E214
	public static int GetPlayerKills(EntityPlayer player)
	{
		return player.KilledPlayers;
	}

	// Token: 0x0600783B RID: 30779 RVA: 0x0031001C File Offset: 0x0030E21C
	public static int GetDeaths(EntityPlayer player)
	{
		return player.Died;
	}

	// Token: 0x0600783C RID: 30780 RVA: 0x00310024 File Offset: 0x0030E224
	public static string GetKMTraveled(EntityPlayer player)
	{
		return (player.distanceWalked / 1000f).ToCultureInvariantString("0.00") + " KM";
	}

	// Token: 0x0600783D RID: 30781 RVA: 0x00310046 File Offset: 0x0030E246
	public static int GetItemsCrafted(EntityPlayer player)
	{
		return (int)player.totalItemsCrafted;
	}

	// Token: 0x0600783E RID: 30782 RVA: 0x0031004E File Offset: 0x0030E24E
	public static string GetLongestLife(EntityPlayer player)
	{
		return XUiM_PlayerBuffs.GetTimeString((float)((int)player.longestLife) * 60f);
	}

	// Token: 0x0600783F RID: 30783 RVA: 0x00310063 File Offset: 0x0030E263
	public static string GetCurrentLife(EntityPlayer player)
	{
		return XUiM_PlayerBuffs.GetTimeString((float)((int)player.currentLife) * 60f);
	}

	// Token: 0x06007840 RID: 30784 RVA: 0x00310078 File Offset: 0x0030E278
	public static float GetHealth(EntityPlayer player)
	{
		return player.Stats.Health.Value;
	}

	// Token: 0x06007841 RID: 30785 RVA: 0x0031008A File Offset: 0x0030E28A
	public static float GetStamina(EntityPlayer player)
	{
		return player.Stats.Stamina.Value;
	}

	// Token: 0x06007842 RID: 30786 RVA: 0x0031009C File Offset: 0x0030E29C
	public static float GetMaxHealth(EntityPlayer player)
	{
		return player.Stats.Health.Max;
	}

	// Token: 0x06007843 RID: 30787 RVA: 0x003100AE File Offset: 0x0030E2AE
	public static float GetMaxStamina(EntityPlayer player)
	{
		return player.Stats.Stamina.Max;
	}

	// Token: 0x06007844 RID: 30788 RVA: 0x003100C0 File Offset: 0x0030E2C0
	public static bool GetHasFullHealth(EntityPlayer player)
	{
		return player.Stats.Health.Max == player.Stats.Health.Value;
	}

	// Token: 0x06007845 RID: 30789 RVA: 0x003100E4 File Offset: 0x0030E2E4
	public static EntityPlayer GetPlayer()
	{
		return GameManager.Instance.World.GetPrimaryPlayer();
	}

	// Token: 0x06007846 RID: 30790 RVA: 0x003100F5 File Offset: 0x0030E2F5
	public static EntityPlayer GetPlayer(int id)
	{
		if (GameManager.Instance != null && GameManager.Instance.World != null)
		{
			return GameManager.Instance.World.GetEntity(id) as EntityPlayer;
		}
		return null;
	}

	// Token: 0x06007847 RID: 30791 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public void CalcDisplayProtectionValues()
	{
	}

	// Token: 0x06007848 RID: 30792 RVA: 0x00310128 File Offset: 0x0030E328
	public static string GetStatValue(PassiveEffects effect, EntityPlayer player, DisplayInfoEntry entry, FastTags<TagGroup.Global> overrideMovementTag)
	{
		FastTags<TagGroup.Global> fastTags = player.generalTags;
		if (entry.TagsSet)
		{
			fastTags = entry.Tags;
		}
		if (overrideMovementTag.IsEmpty)
		{
			fastTags |= EntityAlive.MovementTagRunning;
		}
		else
		{
			fastTags |= overrideMovementTag;
		}
		float num = EffectManager.GetValue(effect, null, 0f, player, null, fastTags, true, true, true, true, true, 1, true, true);
		if (entry.DisplayType == DisplayInfoEntry.DisplayTypes.Percent)
		{
			num *= 100f;
			num = Mathf.Floor(num);
			if (entry.ShowInverted)
			{
				num -= 100f;
			}
			return num.ToString("0") + "%";
		}
		if (entry.DisplayType == DisplayInfoEntry.DisplayTypes.Time)
		{
			return XUiM_PlayerBuffs.GetCVarValueAsTimeString(num);
		}
		if (entry.DisplayType == DisplayInfoEntry.DisplayTypes.Integer)
		{
			num = Mathf.Floor(num);
		}
		else
		{
			num *= 100f;
			num = Mathf.Floor(num);
			num /= 100f;
		}
		if (entry.ShowInverted)
		{
			num -= 1f;
		}
		return num.ToString("0.##");
	}
}
