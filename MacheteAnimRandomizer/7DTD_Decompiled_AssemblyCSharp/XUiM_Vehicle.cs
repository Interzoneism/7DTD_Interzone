using System;
using Audio;

// Token: 0x02000EFF RID: 3839
public class XUiM_Vehicle : XUiModel
{
	// Token: 0x06007904 RID: 30980 RVA: 0x00313AD6 File Offset: 0x00311CD6
	public static string GetEntityName(XUi _xui)
	{
		if (!(_xui.vehicle != null))
		{
			return "";
		}
		return _xui.vehicle.EntityName;
	}

	// Token: 0x06007905 RID: 30981 RVA: 0x00313AF7 File Offset: 0x00311CF7
	public static float GetSpeed(XUi _xui)
	{
		if (!(_xui.vehicle != null))
		{
			return 0f;
		}
		return _xui.vehicle.GetVehicle().MaxPossibleSpeed;
	}

	// Token: 0x06007906 RID: 30982 RVA: 0x00313B20 File Offset: 0x00311D20
	public static string GetNoise(XUi _xui)
	{
		if (_xui.vehicle == null)
		{
			return "";
		}
		XUiM_Vehicle.checkLocalization();
		float noise = _xui.vehicle.GetVehicle().GetNoise();
		if (noise <= 0.33f)
		{
			return XUiM_Vehicle.lblNoiseSoft;
		}
		if (noise <= 0.66f)
		{
			return XUiM_Vehicle.lblNoiseModerate;
		}
		return XUiM_Vehicle.lblNoiseLoud;
	}

	// Token: 0x06007907 RID: 30983 RVA: 0x00313B78 File Offset: 0x00311D78
	[PublicizedFrom(EAccessModifier.Private)]
	public static void checkLocalization()
	{
		if (!XUiM_Vehicle.HasLocalizationBeenCached)
		{
			XUiM_Vehicle.lblNoiseSoft = Localization.Get("xuiVehicleNoiseSoft", false);
			XUiM_Vehicle.lblNoiseModerate = Localization.Get("xuiVehicleNoiseModerate", false);
			XUiM_Vehicle.lblNoiseLoud = Localization.Get("xuiVehicleNoiseLoud", false);
			XUiM_Vehicle.lblSpeedNone = Localization.Get("xuiVehicleSpeedNone", false);
			XUiM_Vehicle.lblSpeedSlow = Localization.Get("xuiVehicleSpeedSlow", false);
			XUiM_Vehicle.lblSpeedNormal = Localization.Get("xuiVehicleSpeedNormal", false);
			XUiM_Vehicle.lblSpeedFast = Localization.Get("xuiVehicleSpeedFast", false);
			XUiM_Vehicle.HasLocalizationBeenCached = true;
		}
	}

	// Token: 0x06007908 RID: 30984 RVA: 0x00313C02 File Offset: 0x00311E02
	public static float GetProtection(XUi _xui)
	{
		if (_xui.vehicle == null)
		{
			return 0f;
		}
		return (1f - _xui.vehicle.GetVehicle().GetPlayerDamagePercent()) * 100f;
	}

	// Token: 0x06007909 RID: 30985 RVA: 0x00313C34 File Offset: 0x00311E34
	public static float GetFuelLevel(XUi _xui)
	{
		if (!(_xui.vehicle != null))
		{
			return 0f;
		}
		return _xui.vehicle.GetVehicle().GetFuelPercent() * 100f;
	}

	// Token: 0x0600790A RID: 30986 RVA: 0x00313C60 File Offset: 0x00311E60
	public static float GetFuelFill(XUi _xui)
	{
		if (!(_xui.vehicle != null))
		{
			return 0f;
		}
		return _xui.vehicle.GetVehicle().GetFuelPercent();
	}

	// Token: 0x0600790B RID: 30987 RVA: 0x00313C86 File Offset: 0x00311E86
	public static int GetPassengers(XUi _xui)
	{
		if (!(_xui.vehicle != null))
		{
			return 1;
		}
		return _xui.vehicle.GetAttachMaxCount();
	}

	// Token: 0x0600790C RID: 30988 RVA: 0x00313CA4 File Offset: 0x00311EA4
	public static string GetSpeedText(XUi _xui)
	{
		float num = (_xui.vehicle != null) ? _xui.vehicle.GetVehicle().MaxPossibleSpeed : 0f;
		XUiM_Vehicle.checkLocalization();
		if (num <= 0f)
		{
			return XUiM_Vehicle.lblSpeedNone;
		}
		if (num <= 9f)
		{
			return XUiM_Vehicle.lblSpeedSlow;
		}
		if (num <= 12f)
		{
			return XUiM_Vehicle.lblSpeedNormal;
		}
		return XUiM_Vehicle.lblSpeedFast;
	}

	// Token: 0x0600790D RID: 30989 RVA: 0x00313D0B File Offset: 0x00311F0B
	public bool SetPart(XUi _xui, string vehicleSlotName, ItemStack stack, out ItemStack resultStack)
	{
		Log.Warning("XUiM_Vehicle SetPart {0}", new object[]
		{
			vehicleSlotName
		});
		_xui.vehicle == null;
		resultStack = stack;
		return false;
	}

	// Token: 0x0600790E RID: 30990 RVA: 0x00002914 File Offset: 0x00000B14
	public void RefreshVehicle()
	{
	}

	// Token: 0x0600790F RID: 30991 RVA: 0x00313D34 File Offset: 0x00311F34
	public static bool RepairVehicle(XUi _xui, Vehicle vehicle = null)
	{
		if (vehicle == null)
		{
			vehicle = _xui.vehicle.GetVehicle();
		}
		ItemValue item = ItemClass.GetItem("resourceRepairKit", false);
		if (item.ItemClass != null)
		{
			EntityPlayerLocal entityPlayer = _xui.playerUI.entityPlayer;
			LocalPlayerUI playerUI = _xui.playerUI;
			int itemCount = entityPlayer.bag.GetItemCount(item, -1, -1, true);
			int itemCount2 = entityPlayer.inventory.GetItemCount(item, false, -1, -1, true);
			int repairAmountNeeded = vehicle.GetRepairAmountNeeded();
			if (itemCount + itemCount2 > 0 && repairAmountNeeded > 0)
			{
				float num = 0f;
				ProgressionValue progressionValue = entityPlayer.Progression.GetProgressionValue("perkGreaseMonkey");
				if (progressionValue != null)
				{
					num += (float)progressionValue.Level * 0.1f;
				}
				vehicle.RepairParts(1000, num);
				if (itemCount2 > 0)
				{
					entityPlayer.inventory.DecItem(item, 1, false, null);
				}
				else
				{
					entityPlayer.bag.DecItem(item, 1, false, null);
				}
				playerUI.xui.CollectedItemList.RemoveItemStack(new ItemStack(item, 1));
				Manager.PlayInsidePlayerHead("craft_complete_item", -1, 0f, false, false);
				return true;
			}
			if (repairAmountNeeded > itemCount + itemCount2)
			{
				Manager.PlayInsidePlayerHead("misc/missingitemtorepair", -1, 0f, false, false);
			}
		}
		return false;
	}

	// Token: 0x04005BE0 RID: 23520
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cRepairBase = 1000;

	// Token: 0x04005BE1 RID: 23521
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cRepairPercent = 0f;

	// Token: 0x04005BE2 RID: 23522
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cRepairPerkPercent = 0.1f;

	// Token: 0x04005BE3 RID: 23523
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblNoiseSoft;

	// Token: 0x04005BE4 RID: 23524
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblNoiseModerate;

	// Token: 0x04005BE5 RID: 23525
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblNoiseLoud;

	// Token: 0x04005BE6 RID: 23526
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblSpeedNone;

	// Token: 0x04005BE7 RID: 23527
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblSpeedSlow;

	// Token: 0x04005BE8 RID: 23528
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblSpeedNormal;

	// Token: 0x04005BE9 RID: 23529
	[PublicizedFrom(EAccessModifier.Private)]
	public static string lblSpeedFast;

	// Token: 0x04005BEA RID: 23530
	public static bool HasLocalizationBeenCached;
}
