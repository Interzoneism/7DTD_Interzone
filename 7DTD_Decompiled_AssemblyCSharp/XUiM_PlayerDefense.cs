using System;

// Token: 0x02000EE6 RID: 3814
public class XUiM_PlayerDefense : XUiModel
{
	// Token: 0x06007856 RID: 30806 RVA: 0x00310D09 File Offset: 0x0030EF09
	public static string GetBashing(EntityPlayer player)
	{
		return string.Format("{0}%", XUiM_PlayerDefense.GetDefenseFromPlayer(player, EnumDamageTypes.Bashing));
	}

	// Token: 0x06007857 RID: 30807 RVA: 0x00310D1C File Offset: 0x0030EF1C
	public static string GetPiercing(EntityPlayer player)
	{
		return string.Format("{0}%", XUiM_PlayerDefense.GetDefenseFromPlayer(player, EnumDamageTypes.Piercing));
	}

	// Token: 0x06007858 RID: 30808 RVA: 0x00310D2F File Offset: 0x0030EF2F
	public static string GetRadiation(EntityPlayer player)
	{
		return string.Format("{0}%", XUiM_PlayerDefense.GetDefenseFromPlayer(player, EnumDamageTypes.Radiation));
	}

	// Token: 0x06007859 RID: 30809 RVA: 0x00310D42 File Offset: 0x0030EF42
	public static string GetWaterproof(EntityPlayer player)
	{
		return string.Format("{0}%", (int)(player.equipment.GetTotalWaterproof() * 100f));
	}

	// Token: 0x0600785A RID: 30810 RVA: 0x00310D65 File Offset: 0x0030EF65
	public static string GetFireproof(EntityPlayer player)
	{
		return string.Format("{0}%", XUiM_PlayerDefense.GetDefenseFromPlayer(player, EnumDamageTypes.Heat));
	}

	// Token: 0x0600785B RID: 30811 RVA: 0x00310D78 File Offset: 0x0030EF78
	public static string GetElectrical(EntityPlayer player)
	{
		return string.Format("{0}%", XUiM_PlayerDefense.GetDefenseFromPlayer(player, EnumDamageTypes.Electrical));
	}

	// Token: 0x0600785C RID: 30812 RVA: 0x00310D8C File Offset: 0x0030EF8C
	public static string GetInsulation(EntityPlayer player)
	{
		return ValueDisplayFormatters.TemperatureRelative(player.equipment.GetTotalInsulation(), 0);
	}

	// Token: 0x0600785D RID: 30813 RVA: 0x00310DA0 File Offset: 0x0030EFA0
	public static string GetWeight(EntityPlayer player)
	{
		float num = 0f;
		int slotCount = player.equipment.GetSlotCount();
		for (int i = 0; i < slotCount; i++)
		{
			ItemValue slotItem = player.equipment.GetSlotItem(i);
			if (slotItem != null)
			{
				ItemClass itemClass = slotItem.ItemClass;
				if (itemClass != null)
				{
					num += itemClass.Encumbrance * 100f;
				}
			}
		}
		return string.Format("{0}%", num.ToCultureInvariantString());
	}

	// Token: 0x0600785E RID: 30814 RVA: 0x00310E08 File Offset: 0x0030F008
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetDefenseFromPlayer(EntityPlayer _player, EnumDamageTypes _armorType)
	{
		float value = 0f;
		if (_armorType != EnumDamageTypes.None && _armorType < EnumDamageTypes.Disease)
		{
			if (_armorType <= EnumDamageTypes.Crushing)
			{
				value = EffectManager.GetValue(PassiveEffects.PhysicalDamageResist, null, 0f, _player, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			}
			else if (_armorType <= EnumDamageTypes.Electrical)
			{
				value = EffectManager.GetValue(PassiveEffects.ElementalDamageResist, null, 0f, _player, null, FastTags<TagGroup.Global>.Parse(_armorType.ToStringCached<EnumDamageTypes>()), true, true, true, true, true, 1, true, false);
			}
		}
		return value.ToCultureInvariantString("00");
	}

	// Token: 0x0600785F RID: 30815 RVA: 0x00310E7D File Offset: 0x0030F07D
	public static string GetBashing(ItemValue itemValue, XUi _xui)
	{
		return string.Format("{0}%", XUiM_PlayerDefense.GetDefenseFromItemValue(itemValue, EnumDamageTypes.Bashing, _xui));
	}

	// Token: 0x06007860 RID: 30816 RVA: 0x00310E91 File Offset: 0x0030F091
	public static string GetPiercing(ItemValue itemValue, XUi _xui)
	{
		return string.Format("{0}%", XUiM_PlayerDefense.GetDefenseFromItemValue(itemValue, EnumDamageTypes.Piercing, _xui));
	}

	// Token: 0x06007861 RID: 30817 RVA: 0x00310EA5 File Offset: 0x0030F0A5
	public static string GetRadiation(ItemValue itemValue, XUi _xui)
	{
		return string.Format("{0}%", XUiM_PlayerDefense.GetDefenseFromItemValue(itemValue, EnumDamageTypes.Radiation, _xui));
	}

	// Token: 0x06007862 RID: 30818 RVA: 0x00310EBC File Offset: 0x0030F0BC
	public static string GetWaterproof(ItemValue itemValue)
	{
		float num = 0f;
		ItemClass itemClass = itemValue.ItemClass;
		if (itemClass != null)
		{
			num = itemClass.WaterProof;
		}
		return string.Format("{0}%", (int)(num * 100f));
	}

	// Token: 0x06007863 RID: 30819 RVA: 0x00310EF7 File Offset: 0x0030F0F7
	public static string GetFireproof(ItemValue itemValue, XUi _xui)
	{
		return string.Format("{0}%", XUiM_PlayerDefense.GetDefenseFromItemValue(itemValue, EnumDamageTypes.Heat, _xui));
	}

	// Token: 0x06007864 RID: 30820 RVA: 0x00310F0B File Offset: 0x0030F10B
	public static string GetElectrical(ItemValue itemValue, XUi _xui)
	{
		return string.Format("{0}%", XUiM_PlayerDefense.GetDefenseFromItemValue(itemValue, EnumDamageTypes.Electrical, _xui));
	}

	// Token: 0x06007865 RID: 30821 RVA: 0x00310F20 File Offset: 0x0030F120
	public static string GetInsulation(ItemValue itemValue)
	{
		float fahrenheit = 0f;
		ItemClass itemClass = itemValue.ItemClass;
		if (itemClass != null)
		{
			fahrenheit = itemClass.Insulation;
		}
		return ValueDisplayFormatters.TemperatureRelative(fahrenheit, 0);
	}

	// Token: 0x06007866 RID: 30822 RVA: 0x00310F4C File Offset: 0x0030F14C
	public static string GetWeight(ItemValue itemValue)
	{
		float value = 0f;
		ItemClass itemClass = itemValue.ItemClass;
		if (itemClass != null)
		{
			value = itemClass.Encumbrance * 100f;
		}
		return string.Format("{0}%", value.ToCultureInvariantString());
	}

	// Token: 0x06007867 RID: 30823 RVA: 0x00310F88 File Offset: 0x0030F188
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetDefenseFromItemValue(ItemValue _itemValue, EnumDamageTypes _damageType, XUi _xui)
	{
		return (EffectManager.GetValue(PassiveEffects.PhysicalDamageResist, _itemValue, 0f, _xui.playerUI.entityPlayer, null, FastTags<TagGroup.Global>.Parse(_damageType.ToStringCached<EnumDamageTypes>()), true, true, true, true, true, 1, true, false) * 100f).ToCultureInvariantString("00");
	}
}
