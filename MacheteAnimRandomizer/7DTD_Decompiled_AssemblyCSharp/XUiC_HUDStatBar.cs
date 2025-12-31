using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000CC0 RID: 3264
[Preserve]
public class XUiC_HUDStatBar : XUiController
{
	// Token: 0x17000A4C RID: 2636
	// (get) Token: 0x060064F0 RID: 25840 RVA: 0x0028E1FE File Offset: 0x0028C3FE
	// (set) Token: 0x060064F1 RID: 25841 RVA: 0x0028E206 File Offset: 0x0028C406
	public HUDStatGroups StatGroup
	{
		get
		{
			return this.statGroup;
		}
		set
		{
			this.statGroup = value;
		}
	}

	// Token: 0x17000A4D RID: 2637
	// (get) Token: 0x060064F2 RID: 25842 RVA: 0x0028E20F File Offset: 0x0028C40F
	// (set) Token: 0x060064F3 RID: 25843 RVA: 0x0028E217 File Offset: 0x0028C417
	public HUDStatTypes StatType
	{
		get
		{
			return this.statType;
		}
		set
		{
			this.statType = value;
			this.SetStatValues();
		}
	}

	// Token: 0x17000A4E RID: 2638
	// (get) Token: 0x060064F4 RID: 25844 RVA: 0x0028E226 File Offset: 0x0028C426
	// (set) Token: 0x060064F5 RID: 25845 RVA: 0x0028E22E File Offset: 0x0028C42E
	public EntityPlayerLocal LocalPlayer { get; [PublicizedFrom(EAccessModifier.Internal)] set; }

	// Token: 0x17000A4F RID: 2639
	// (get) Token: 0x060064F6 RID: 25846 RVA: 0x0028E237 File Offset: 0x0028C437
	// (set) Token: 0x060064F7 RID: 25847 RVA: 0x0028E23F File Offset: 0x0028C43F
	public EntityVehicle Vehicle { get; [PublicizedFrom(EAccessModifier.Internal)] set; }

	// Token: 0x060064F8 RID: 25848 RVA: 0x0028E248 File Offset: 0x0028C448
	public override void Init()
	{
		base.Init();
		this.IsDirty = true;
		XUiController childById = base.GetChildById("BarContent");
		if (childById != null)
		{
			this.barContent = (XUiV_Sprite)childById.ViewComponent;
		}
		XUiController childById2 = base.GetChildById("TextContent");
		if (childById2 != null)
		{
			this.textContent = (XUiV_Label)childById2.ViewComponent;
		}
		this.activeAmmoItemValue = ItemValue.None.Clone();
	}

	// Token: 0x060064F9 RID: 25849 RVA: 0x0028E2B4 File Offset: 0x0028C4B4
	public override void Update(float _dt)
	{
		base.Update(_dt);
		this.deltaTime = _dt;
		if (this.LocalPlayer == null && XUi.IsGameRunning())
		{
			this.LocalPlayer = base.xui.playerUI.entityPlayer;
		}
		GUIWindowManager windowManager = base.xui.playerUI.windowManager;
		if (windowManager.IsFullHUDDisabled())
		{
			this.viewComponent.IsVisible = false;
			return;
		}
		if (!base.xui.dragAndDrop.InMenu && windowManager.IsHUDPartialHidden())
		{
			this.viewComponent.IsVisible = false;
			return;
		}
		if (this.statGroup == HUDStatGroups.Vehicle && this.LocalPlayer != null)
		{
			if (this.Vehicle == null && this.LocalPlayer.AttachedToEntity != null && this.LocalPlayer.AttachedToEntity is EntityVehicle)
			{
				this.Vehicle = (EntityVehicle)this.LocalPlayer.AttachedToEntity;
				this.IsDirty = true;
				base.xui.CollectedItemList.SetYOffset(100);
			}
			else if (this.Vehicle != null && this.LocalPlayer.AttachedToEntity == null)
			{
				this.Vehicle = null;
				this.IsDirty = true;
			}
		}
		if (this.statType == HUDStatTypes.Stealth && this.LocalPlayer.IsCrouching != this.wasCrouching)
		{
			this.wasCrouching = this.LocalPlayer.IsCrouching;
			base.RefreshBindings(true);
			this.IsDirty = true;
		}
		if (this.statType == HUDStatTypes.ActiveItem)
		{
			if (this.currentSlotIndex != base.xui.PlayerInventory.Toolbelt.GetFocusedItemIdx())
			{
				this.currentSlotIndex = base.xui.PlayerInventory.Toolbelt.GetFocusedItemIdx();
				this.IsDirty = true;
			}
			if (this.HasChanged() || this.IsDirty)
			{
				this.SetupActiveItemEntry();
				this.updateActiveItemAmmo();
				base.RefreshBindings(true);
				this.IsDirty = false;
				return;
			}
		}
		else
		{
			this.RefreshFill();
			if (this.HasChanged() || this.IsDirty)
			{
				if (this.IsDirty)
				{
					this.IsDirty = false;
				}
				base.RefreshBindings(true);
			}
		}
	}

	// Token: 0x060064FA RID: 25850 RVA: 0x0028E4D4 File Offset: 0x0028C6D4
	public bool HasChanged()
	{
		bool result = false;
		switch (this.statType)
		{
		case HUDStatTypes.Health:
			result = true;
			break;
		case HUDStatTypes.Stamina:
			result = true;
			break;
		case HUDStatTypes.Water:
			result = (this.oldValue != this.LocalPlayer.Stats.Water.ValuePercentUI);
			this.oldValue = this.LocalPlayer.Stats.Water.ValuePercentUI;
			break;
		case HUDStatTypes.Food:
			result = (this.oldValue != this.LocalPlayer.Stats.Food.ValuePercentUI);
			this.oldValue = this.LocalPlayer.Stats.Food.ValuePercentUI;
			break;
		case HUDStatTypes.Stealth:
			result = (this.oldValue != this.lastValue);
			this.oldValue = this.lastValue;
			break;
		case HUDStatTypes.ActiveItem:
		{
			ItemAction itemAction = this.LocalPlayer.inventory.holdingItemItemValue.ItemClass.Actions[0];
			if (itemAction != null && itemAction.IsEditingTool())
			{
				result = itemAction.IsStatChanged();
			}
			break;
		}
		case HUDStatTypes.VehicleHealth:
		{
			if (this.Vehicle == null)
			{
				return false;
			}
			int health = this.Vehicle.GetVehicle().GetHealth();
			result = (this.oldValue != (float)health);
			this.oldValue = (float)health;
			break;
		}
		case HUDStatTypes.VehicleFuel:
			if (this.Vehicle == null)
			{
				return false;
			}
			result = (this.oldValue != this.Vehicle.GetVehicle().GetFuelLevel());
			this.oldValue = this.Vehicle.GetVehicle().GetFuelLevel();
			break;
		case HUDStatTypes.VehicleBattery:
			if (this.Vehicle == null)
			{
				return false;
			}
			result = (this.oldValue != this.Vehicle.GetVehicle().GetBatteryLevel());
			this.oldValue = this.Vehicle.GetVehicle().GetBatteryLevel();
			break;
		}
		return result;
	}

	// Token: 0x060064FB RID: 25851 RVA: 0x0028E6CC File Offset: 0x0028C8CC
	public void RefreshFill()
	{
		if (this.barContent == null || this.LocalPlayer == null || (this.statGroup == HUDStatGroups.Vehicle && this.Vehicle == null))
		{
			return;
		}
		float t = Time.deltaTime * 3f;
		float b = 0f;
		switch (this.statType)
		{
		case HUDStatTypes.Health:
			b = Mathf.Clamp01(this.LocalPlayer.Stats.Health.ValuePercentUI);
			break;
		case HUDStatTypes.Stamina:
			b = Mathf.Clamp01(this.LocalPlayer.Stats.Stamina.ValuePercentUI);
			break;
		case HUDStatTypes.Water:
			b = this.LocalPlayer.Stats.Water.ValuePercentUI;
			break;
		case HUDStatTypes.Food:
			b = this.LocalPlayer.Stats.Food.ValuePercentUI;
			break;
		case HUDStatTypes.Stealth:
			b = this.LocalPlayer.Stealth.ValuePercentUI;
			break;
		case HUDStatTypes.ActiveItem:
			b = (float)this.LocalPlayer.inventory.holdingItemItemValue.Meta / EffectManager.GetValue(PassiveEffects.MagazineSize, this.LocalPlayer.inventory.holdingItemItemValue, (float)this.attackAction.BulletsPerMagazine, this.LocalPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			break;
		case HUDStatTypes.VehicleHealth:
			b = this.Vehicle.GetVehicle().GetHealthPercent();
			break;
		case HUDStatTypes.VehicleFuel:
			b = this.Vehicle.GetVehicle().GetFuelPercent();
			break;
		case HUDStatTypes.VehicleBattery:
			b = this.Vehicle.GetVehicle().GetBatteryLevel();
			break;
		}
		float fill = Math.Max(this.lastValue, 0f);
		this.lastValue = Mathf.Lerp(this.lastValue, b, t);
		this.barContent.Fill = fill;
	}

	// Token: 0x060064FC RID: 25852 RVA: 0x0028E898 File Offset: 0x0028CA98
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 2825508620U)
		{
			if (num <= 1542587592U)
			{
				if (num != 669092238U)
				{
					if (num != 1122103630U)
					{
						if (num == 1542587592U)
						{
							if (bindingName == "statcurrent")
							{
								if (this.LocalPlayer == null || (this.statGroup == HUDStatGroups.Vehicle && this.Vehicle == null))
								{
									value = "";
									return true;
								}
								switch (this.statType)
								{
								case HUDStatTypes.Health:
									value = this.statcurrentFormatterInt.Format(this.LocalPlayer.Health);
									break;
								case HUDStatTypes.Stamina:
									value = this.statcurrentFormatterFloat.Format(this.LocalPlayer.Stamina);
									break;
								case HUDStatTypes.Water:
									value = this.statcurrentFormatterInt.Format((int)(this.LocalPlayer.Stats.Water.ValuePercentUI * 100f));
									break;
								case HUDStatTypes.Food:
									value = this.statcurrentFormatterInt.Format((int)(this.LocalPlayer.Stats.Food.ValuePercentUI * 100f));
									break;
								case HUDStatTypes.Stealth:
									value = this.statcurrentFormatterFloat.Format((float)((int)(this.LocalPlayer.Stealth.ValuePercentUI * 100f)));
									break;
								case HUDStatTypes.ActiveItem:
									if (this.attackAction is ItemActionTextureBlock)
									{
										value = this.currentPaintAmmoFormatter.Format(this.currentAmmoCount);
									}
									else
									{
										value = this.statcurrentFormatterInt.Format(this.LocalPlayer.inventory.holdingItemItemValue.Meta);
									}
									break;
								case HUDStatTypes.VehicleHealth:
									value = this.statcurrentFormatterInt.Format(this.Vehicle.GetVehicle().GetHealth());
									break;
								case HUDStatTypes.VehicleFuel:
									value = this.statcurrentFormatterFloat.Format(this.Vehicle.GetVehicle().GetFuelLevel());
									break;
								case HUDStatTypes.VehicleBattery:
									value = this.statcurrentFormatterFloat.Format(this.Vehicle.GetVehicle().GetBatteryLevel());
									break;
								}
								return true;
							}
						}
					}
					else if (bindingName == "statcurrentwithmax")
					{
						if (this.LocalPlayer == null || (this.statGroup == HUDStatGroups.Vehicle && this.Vehicle == null))
						{
							value = "";
							return true;
						}
						switch (this.statType)
						{
						case HUDStatTypes.Health:
							value = this.statcurrentWMaxFormatterAOfB.Format((int)this.LocalPlayer.Stats.Health.Value, (int)this.LocalPlayer.Stats.Health.Max);
							break;
						case HUDStatTypes.Stamina:
							value = this.statcurrentWMaxFormatterAOfB.Format((int)XUiM_Player.GetStamina(this.LocalPlayer), (int)this.LocalPlayer.Stats.Stamina.Max);
							break;
						case HUDStatTypes.Water:
							value = this.statcurrentWMaxFormatterOf100.Format((int)(this.LocalPlayer.Stats.Water.ValuePercentUI * 100f));
							break;
						case HUDStatTypes.Food:
							value = this.statcurrentWMaxFormatterOf100.Format((int)(this.LocalPlayer.Stats.Food.ValuePercentUI * 100f));
							break;
						case HUDStatTypes.Stealth:
							value = this.statcurrentWMaxFormatterOf100.Format((int)(this.LocalPlayer.Stealth.ValuePercentUI * 100f));
							break;
						case HUDStatTypes.ActiveItem:
							if (this.attackAction is ItemActionTextureBlock)
							{
								value = this.currentPaintAmmoFormatter.Format(this.currentAmmoCount);
							}
							else if (this.attackAction != null && this.attackAction.IsEditingTool())
							{
								ItemActionData itemActionDataInSlot = this.LocalPlayer.inventory.GetItemActionDataInSlot(this.currentSlotIndex, 1);
								value = this.attackAction.GetStat(itemActionDataInSlot);
							}
							else
							{
								value = this.statcurrentWMaxFormatterAOfB.Format(this.LocalPlayer.inventory.GetItem(this.currentSlotIndex).itemValue.Meta, this.currentAmmoCount);
							}
							break;
						case HUDStatTypes.VehicleHealth:
							value = this.statcurrentWMaxFormatterPercent.Format((int)(this.Vehicle.GetVehicle().GetHealthPercent() * 100f));
							break;
						case HUDStatTypes.VehicleFuel:
							value = this.statcurrentWMaxFormatterPercent.Format((int)(this.Vehicle.GetVehicle().GetFuelPercent() * 100f));
							break;
						case HUDStatTypes.VehicleBattery:
							value = this.statcurrentWMaxFormatterPercent.Format((int)(this.Vehicle.GetVehicle().GetBatteryLevel() * 100f));
							break;
						}
						return true;
					}
				}
				else if (bindingName == "statfill")
				{
					if (this.LocalPlayer == null || (this.statGroup == HUDStatGroups.Vehicle && this.Vehicle == null))
					{
						value = "0";
						return true;
					}
					float t = this.deltaTime * 3f;
					float b = 0f;
					switch (this.statType)
					{
					case HUDStatTypes.Health:
						b = this.LocalPlayer.Stats.Health.ValuePercentUI;
						break;
					case HUDStatTypes.Stamina:
						b = this.LocalPlayer.Stats.Stamina.ValuePercentUI;
						break;
					case HUDStatTypes.Water:
						b = this.LocalPlayer.Stats.Water.ValuePercentUI;
						break;
					case HUDStatTypes.Food:
						b = this.LocalPlayer.Stats.Food.ValuePercentUI;
						break;
					case HUDStatTypes.Stealth:
						b = this.LocalPlayer.Stealth.ValuePercentUI;
						break;
					case HUDStatTypes.ActiveItem:
						b = (float)this.LocalPlayer.inventory.holdingItemItemValue.Meta / EffectManager.GetValue(PassiveEffects.MagazineSize, this.LocalPlayer.inventory.holdingItemItemValue, (float)this.attackAction.BulletsPerMagazine, this.LocalPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
						break;
					case HUDStatTypes.VehicleHealth:
						b = this.Vehicle.GetVehicle().GetHealthPercent();
						break;
					case HUDStatTypes.VehicleFuel:
						b = this.Vehicle.GetVehicle().GetFuelPercent();
						break;
					case HUDStatTypes.VehicleBattery:
						b = this.Vehicle.GetVehicle().GetBatteryLevel();
						break;
					}
					float v = Math.Max(this.lastValue, 0f) * 1.01f;
					value = this.statfillFormatter.Format(v);
					this.lastValue = Mathf.Lerp(this.lastValue, b, t);
					return true;
				}
			}
			else if (num != 1822678806U)
			{
				if (num != 2758588565U)
				{
					if (num == 2825508620U)
					{
						if (bindingName == "statimage")
						{
							value = this.statImage;
							return true;
						}
					}
				}
				else if (bindingName == "staticonatlas")
				{
					value = this.statAtlas;
					return true;
				}
			}
			else if (bindingName == "staticon")
			{
				if (this.statType == HUDStatTypes.ActiveItem)
				{
					value = ((this.itemClass != null) ? this.itemClass.GetIconName() : "");
				}
				else if (this.statType == HUDStatTypes.VehicleHealth)
				{
					value = ((this.Vehicle != null) ? this.Vehicle.GetMapIcon() : "");
				}
				else
				{
					value = this.statIcon;
				}
				return true;
			}
		}
		else if (num <= 3799067675U)
		{
			if (num != 3007315583U)
			{
				if (num != 3150708601U)
				{
					if (num == 3799067675U)
					{
						if (bindingName == "statvisible")
						{
							if (this.LocalPlayer == null)
							{
								value = "true";
								return true;
							}
							value = "true";
							if (this.LocalPlayer.IsDead())
							{
								value = "false";
								return true;
							}
							if (this.statGroup == HUDStatGroups.Vehicle)
							{
								if (this.statType == HUDStatTypes.VehicleFuel)
								{
									value = (this.Vehicle != null && this.Vehicle.GetVehicle().HasEnginePart()).ToString();
								}
								else
								{
									value = (this.Vehicle != null).ToString();
								}
							}
							else if (this.statType == HUDStatTypes.ActiveItem)
							{
								if (this.attackAction != null && (this.attackAction.IsEditingTool() || (int)EffectManager.GetValue(PassiveEffects.MagazineSize, this.LocalPlayer.inventory.holdingItemItemValue, 0f, this.LocalPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0))
								{
									value = "true";
								}
								else
								{
									value = "false";
								}
							}
							else if (this.statType == HUDStatTypes.Stealth)
							{
								if (this.LocalPlayer.Crouching)
								{
									base.xui.BuffPopoutList.SetYOffset(52);
									value = "true";
								}
								else
								{
									base.xui.BuffPopoutList.SetYOffset(0);
									value = "false";
								}
							}
							return true;
						}
					}
				}
				else if (bindingName == "staticoncolor")
				{
					Color32 v2 = Color.white;
					if (this.statType == HUDStatTypes.ActiveItem && this.itemClass != null)
					{
						v2 = this.itemClass.GetIconTint(null);
					}
					value = this.staticoncolorFormatter.Format(v2);
					return true;
				}
			}
			else if (bindingName == "sprintactive")
			{
				if (this.LocalPlayer == null)
				{
					value = "false";
				}
				else if (this.LocalPlayer.MovementRunning || this.LocalPlayer.MoveController.RunToggleActive)
				{
					value = "true";
				}
				else
				{
					value = "false";
				}
				return true;
			}
		}
		else if (num != 3888153342U)
		{
			if (num != 3905392387U)
			{
				if (num == 3907838626U)
				{
					if (bindingName == "statmodifiedmax")
					{
						if (this.LocalPlayer == null || (this.statGroup == HUDStatGroups.Vehicle && this.Vehicle == null))
						{
							value = "0";
							return true;
						}
						switch (this.statType)
						{
						case HUDStatTypes.Health:
							value = this.statmodifiedmaxFormatter.Format(this.LocalPlayer.Stats.Health.ModifiedMax, this.LocalPlayer.Stats.Health.Max);
							break;
						case HUDStatTypes.Stamina:
							value = this.statmodifiedmaxFormatter.Format(this.LocalPlayer.Stats.Stamina.ModifiedMax, this.LocalPlayer.Stats.Stamina.Max);
							break;
						case HUDStatTypes.Water:
							value = this.statmodifiedmaxFormatter.Format(this.LocalPlayer.Stats.Water.ModifiedMax, this.LocalPlayer.Stats.Water.Max);
							break;
						case HUDStatTypes.Food:
							value = this.statmodifiedmaxFormatter.Format(this.LocalPlayer.Stats.Food.ModifiedMax, this.LocalPlayer.Stats.Food.Max);
							break;
						}
						return true;
					}
				}
			}
			else if (bindingName == "stealthcolor")
			{
				EntityPlayerLocal localPlayer = this.LocalPlayer;
				value = this.stealthColorFormatter.Format(localPlayer ? localPlayer.Stealth.ValueColorUI : default(Color32));
				return true;
			}
		}
		else if (bindingName == "statregenrate")
		{
			if (this.LocalPlayer == null || (this.statGroup == HUDStatGroups.Vehicle && this.Vehicle == null))
			{
				value = "0";
				return true;
			}
			switch (this.statType)
			{
			case HUDStatTypes.Health:
				value = this.statregenrateFormatter.Format(this.LocalPlayer.Stats.Health.RegenerationAmountUI);
				break;
			case HUDStatTypes.Stamina:
				value = this.statregenrateFormatter.Format(this.LocalPlayer.Stats.Stamina.RegenerationAmountUI);
				break;
			case HUDStatTypes.Water:
				value = this.statregenrateFormatter.Format(this.LocalPlayer.Stats.Water.RegenerationAmountUI);
				break;
			case HUDStatTypes.Food:
				value = this.statregenrateFormatter.Format(this.LocalPlayer.Stats.Food.RegenerationAmountUI);
				break;
			}
			return true;
		}
		return false;
	}

	// Token: 0x060064FD RID: 25853 RVA: 0x0028F4F0 File Offset: 0x0028D6F0
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(name, value, _parent);
		if (flag)
		{
			return flag;
		}
		if (name == "stat_type")
		{
			this.StatType = EnumUtils.Parse<HUDStatTypes>(value, true);
			return true;
		}
		return false;
	}

	// Token: 0x060064FE RID: 25854 RVA: 0x0028F52C File Offset: 0x0028D72C
	public void SetStatValues()
	{
		switch (this.statType)
		{
		case HUDStatTypes.Health:
			this.statImage = "ui_game_stat_bar_health";
			this.statIcon = "ui_game_symbol_add";
			this.statGroup = HUDStatGroups.Player;
			return;
		case HUDStatTypes.Stamina:
			this.statImage = "ui_game_stat_bar_stamina";
			this.statIcon = "ui_game_symbol_run";
			this.statGroup = HUDStatGroups.Player;
			return;
		case HUDStatTypes.Water:
			this.statImage = "ui_game_stat_bar_stamina";
			this.statIcon = "ui_game_symbol_water";
			this.statGroup = HUDStatGroups.Player;
			return;
		case HUDStatTypes.Food:
			this.statImage = "ui_game_stat_bar_health";
			this.statIcon = "ui_game_symbol_hunger";
			this.statGroup = HUDStatGroups.Player;
			return;
		case HUDStatTypes.Stealth:
			this.statImage = "ui_game_stat_bar_health";
			this.statIcon = "ui_game_symbol_stealth";
			this.statGroup = HUDStatGroups.Player;
			return;
		case HUDStatTypes.ActiveItem:
			this.statImage = "ui_game_popup";
			this.statIcon = "ui_game_symbol_battery";
			this.statGroup = HUDStatGroups.Player;
			this.statAtlas = "ItemIconAtlas";
			return;
		case HUDStatTypes.VehicleHealth:
			this.statImage = "ui_game_stat_bar_health";
			this.statIcon = "ui_game_symbol_minibike";
			this.statGroup = HUDStatGroups.Vehicle;
			return;
		case HUDStatTypes.VehicleFuel:
			this.statImage = "ui_game_stat_bar_stamina";
			this.statIcon = "ui_game_symbol_gas";
			this.statGroup = HUDStatGroups.Vehicle;
			return;
		case HUDStatTypes.VehicleBattery:
			this.statImage = "ui_game_popup";
			this.statIcon = "ui_game_symbol_battery";
			this.statGroup = HUDStatGroups.Vehicle;
			return;
		default:
			return;
		}
	}

	// Token: 0x060064FF RID: 25855 RVA: 0x0028F684 File Offset: 0x0028D884
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupActiveItemEntry()
	{
		this.itemClass = null;
		this.attackAction = null;
		this.activeAmmoItemValue = ItemValue.None.Clone();
		EntityPlayer localPlayer = this.LocalPlayer;
		if (localPlayer)
		{
			Inventory inventory = localPlayer.inventory;
			ItemValue itemValue = inventory.GetItem(this.currentSlotIndex).itemValue;
			this.itemClass = itemValue.ItemClass;
			if (this.itemClass != null)
			{
				ItemActionAttack itemActionAttack = this.itemClass.Actions[0] as ItemActionAttack;
				if (itemActionAttack != null && itemActionAttack.IsEditingTool())
				{
					this.attackAction = itemActionAttack;
					base.xui.CollectedItemList.SetYOffset(46);
					return;
				}
				if (itemActionAttack == null || itemActionAttack is ItemActionMelee || !this.itemClass.IsGun() || (itemActionAttack.InfiniteAmmo && !itemActionAttack.ForceShowAmmo) || (int)EffectManager.GetValue(PassiveEffects.MagazineSize, inventory.holdingItemItemValue, 0f, localPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) <= 0)
				{
					this.currentAmmoCount = 0;
					base.xui.CollectedItemList.SetYOffset((localPlayer.AttachedToEntity is EntityVehicle) ? 100 : 0);
					return;
				}
				this.attackAction = itemActionAttack;
				if (itemActionAttack.MagazineItemNames != null && itemActionAttack.MagazineItemNames.Length != 0)
				{
					this.lastAmmoName = itemActionAttack.MagazineItemNames[(int)itemValue.SelectedAmmoTypeIndex];
					this.activeAmmoItemValue = ItemClass.GetItem(this.lastAmmoName, false);
					this.itemClass = ItemClass.GetItemClass(this.lastAmmoName, false);
				}
				base.xui.CollectedItemList.SetYOffset(46);
				return;
			}
			else
			{
				this.currentAmmoCount = 0;
				base.xui.CollectedItemList.SetYOffset((localPlayer.AttachedToEntity is EntityVehicle) ? 100 : 0);
			}
		}
	}

	// Token: 0x06006500 RID: 25856 RVA: 0x0028F830 File Offset: 0x0028DA30
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.statType == HUDStatTypes.ActiveItem)
		{
			base.xui.PlayerInventory.OnBackpackItemsChanged += this.PlayerInventory_OnBackpackItemsChanged;
			base.xui.PlayerInventory.OnToolbeltItemsChanged += this.PlayerInventory_OnToolbeltItemsChanged;
		}
		this.IsDirty = true;
		base.RefreshBindings(true);
	}

	// Token: 0x06006501 RID: 25857 RVA: 0x0028F892 File Offset: 0x0028DA92
	public override void OnClose()
	{
		base.OnClose();
		base.xui.PlayerInventory.OnBackpackItemsChanged -= this.PlayerInventory_OnBackpackItemsChanged;
		base.xui.PlayerInventory.OnToolbeltItemsChanged -= this.PlayerInventory_OnToolbeltItemsChanged;
	}

	// Token: 0x06006502 RID: 25858 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayerInventory_OnToolbeltItemsChanged()
	{
		this.IsDirty = true;
	}

	// Token: 0x06006503 RID: 25859 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayerInventory_OnBackpackItemsChanged()
	{
		this.IsDirty = true;
	}

	// Token: 0x06006504 RID: 25860 RVA: 0x0028F8D4 File Offset: 0x0028DAD4
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateActiveItemAmmo()
	{
		if (this.activeAmmoItemValue.type == 0)
		{
			return;
		}
		this.currentAmmoCount = this.LocalPlayer.inventory.GetItemCount(this.activeAmmoItemValue, false, -1, -1, true);
		this.currentAmmoCount += this.LocalPlayer.bag.GetItemCount(this.activeAmmoItemValue, -1, -1, true);
		this.IsDirty = true;
	}

	// Token: 0x04004C2E RID: 19502
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastValue;

	// Token: 0x04004C2F RID: 19503
	[PublicizedFrom(EAccessModifier.Private)]
	public bool flipped;

	// Token: 0x04004C30 RID: 19504
	[PublicizedFrom(EAccessModifier.Private)]
	public HUDStatGroups statGroup;

	// Token: 0x04004C31 RID: 19505
	[PublicizedFrom(EAccessModifier.Private)]
	public HUDStatTypes statType;

	// Token: 0x04004C34 RID: 19508
	[PublicizedFrom(EAccessModifier.Private)]
	public string statImage = "";

	// Token: 0x04004C35 RID: 19509
	[PublicizedFrom(EAccessModifier.Private)]
	public string statIcon = "";

	// Token: 0x04004C36 RID: 19510
	[PublicizedFrom(EAccessModifier.Private)]
	public string statAtlas = "UIAtlas";

	// Token: 0x04004C37 RID: 19511
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite barContent;

	// Token: 0x04004C38 RID: 19512
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label textContent;

	// Token: 0x04004C39 RID: 19513
	[PublicizedFrom(EAccessModifier.Private)]
	public DateTime updateTime;

	// Token: 0x04004C3A RID: 19514
	[PublicizedFrom(EAccessModifier.Private)]
	public float deltaTime;

	// Token: 0x04004C3B RID: 19515
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasCrouching;

	// Token: 0x04004C3C RID: 19516
	[PublicizedFrom(EAccessModifier.Private)]
	public int currentSlotIndex = -1;

	// Token: 0x04004C3D RID: 19517
	[PublicizedFrom(EAccessModifier.Private)]
	public string lastAmmoName = "";

	// Token: 0x04004C3E RID: 19518
	[PublicizedFrom(EAccessModifier.Private)]
	public int currentAmmoCount;

	// Token: 0x04004C3F RID: 19519
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue activeAmmoItemValue;

	// Token: 0x04004C40 RID: 19520
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemActionAttack attackAction;

	// Token: 0x04004C41 RID: 19521
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass itemClass;

	// Token: 0x04004C42 RID: 19522
	[PublicizedFrom(EAccessModifier.Private)]
	public float oldValue;

	// Token: 0x04004C43 RID: 19523
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt statcurrentFormatterInt = new CachedStringFormatterInt();

	// Token: 0x04004C44 RID: 19524
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat statcurrentFormatterFloat = new CachedStringFormatterFloat(null);

	// Token: 0x04004C45 RID: 19525
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt currentPaintAmmoFormatter = new CachedStringFormatterInt();

	// Token: 0x04004C46 RID: 19526
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<int, int> statcurrentWMaxFormatterAOfB = new CachedStringFormatter<int, int>((int _i, int _i1) => string.Format("{0}/{1}", _i, _i1));

	// Token: 0x04004C47 RID: 19527
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<int> statcurrentWMaxFormatterOf100 = new CachedStringFormatter<int>((int _i) => _i.ToString() + "/100");

	// Token: 0x04004C48 RID: 19528
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<int> statcurrentWMaxFormatterPercent = new CachedStringFormatter<int>((int _i) => _i.ToString() + "%");

	// Token: 0x04004C49 RID: 19529
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<float, float> statmodifiedmaxFormatter = new CachedStringFormatter<float, float>((float _f1, float _f2) => (_f1 / _f2).ToCultureInvariantString());

	// Token: 0x04004C4A RID: 19530
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<float> statregenrateFormatter = new CachedStringFormatter<float>((float _f) => ((_f >= 0f) ? "+" : "") + _f.ToCultureInvariantString("0.00"));

	// Token: 0x04004C4B RID: 19531
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat statfillFormatter = new CachedStringFormatterFloat(null);

	// Token: 0x04004C4C RID: 19532
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor staticoncolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04004C4D RID: 19533
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor stealthColorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04004C4E RID: 19534
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastRegenAmount;
}
