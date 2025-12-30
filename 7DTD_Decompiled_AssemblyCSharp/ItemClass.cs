using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Audio;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;
using XMLData.Item;

// Token: 0x0200055D RID: 1373
[Preserve]
public class ItemClass : ItemData
{
	// Token: 0x17000467 RID: 1127
	// (get) Token: 0x06002C36 RID: 11318 RVA: 0x00127230 File Offset: 0x00125430
	public string[] Attachments
	{
		get
		{
			return this.attachments;
		}
	}

	// Token: 0x17000468 RID: 1128
	// (get) Token: 0x06002C37 RID: 11319 RVA: 0x00127238 File Offset: 0x00125438
	public bool HasQuality
	{
		get
		{
			return this.Effects != null && this.Effects.IsOwnerTiered();
		}
	}

	// Token: 0x17000469 RID: 1129
	// (get) Token: 0x06002C38 RID: 11320 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsEquipment
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700046A RID: 1130
	// (get) Token: 0x06002C39 RID: 11321 RVA: 0x00127250 File Offset: 0x00125450
	public RecipeUnlockData[] UnlockedBy
	{
		get
		{
			if (this.unlockedBy == null)
			{
				if (this.Properties.Values.ContainsKey(ItemClass.PropUnlockedBy))
				{
					string[] array = this.Properties.Values[ItemClass.PropUnlockedBy].Split(',', StringSplitOptions.None);
					if (array.Length != 0)
					{
						this.unlockedBy = new RecipeUnlockData[array.Length];
						for (int i = 0; i < array.Length; i++)
						{
							this.unlockedBy[i] = new RecipeUnlockData(array[i]);
						}
					}
				}
				else
				{
					this.unlockedBy = new RecipeUnlockData[0];
				}
			}
			return this.unlockedBy;
		}
	}

	// Token: 0x06002C3A RID: 11322 RVA: 0x001272E0 File Offset: 0x001254E0
	public ItemClass()
	{
		this.bCanHold = true;
		this.bCanDrop = true;
		this.bCraftingTool = false;
		this.Properties = new DynamicProperties();
		this.attachments = null;
		this.Actions = new ItemAction[5];
		for (int i = 0; i < this.Actions.Length; i++)
		{
			this.Actions[i] = null;
		}
	}

	// Token: 0x06002C3B RID: 11323 RVA: 0x001274B5 File Offset: 0x001256B5
	public void SetId(int _id)
	{
		this.pId = _id;
		if (this.Effects != null)
		{
			this.Effects.ParentPointer = _id;
		}
	}

	// Token: 0x06002C3C RID: 11324 RVA: 0x001274D8 File Offset: 0x001256D8
	public virtual void Init()
	{
		string itemName = this.GetItemName();
		ItemClass.nameToItem[itemName] = this;
		ItemClass.nameToItemCaseInsensitive[itemName] = this;
		ItemClass.itemNames.Add(itemName);
		if (this.Properties.Values.ContainsKey(ItemClass.PropTags))
		{
			this.ItemTags = FastTags<TagGroup.Global>.Parse(this.Properties.Values[ItemClass.PropTags]);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropDistractionTags))
		{
			this.DistractionTags = FastTags<TagGroup.Global>.Parse(this.Properties.Values[ItemClass.PropDistractionTags]);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropFuelValue))
		{
			int startValue = 0;
			int.TryParse(this.Properties.Values[ItemClass.PropFuelValue], out startValue);
			base.FuelValue = new DataItem<int>(startValue);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropWeight))
		{
			int startValue2;
			int.TryParse(this.Properties.Values[ItemClass.PropWeight], out startValue2);
			base.Weight = new DataItem<int>(startValue2);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropImageEffectOnActive))
		{
			string startValue3 = this.Properties.Values[ItemClass.PropImageEffectOnActive].ToString();
			base.ImageEffectOnActive = new DataItem<string>(startValue3);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropActive))
		{
			bool startValue4 = false;
			StringParsers.TryParseBool(this.Properties.Values[ItemClass.PropActive], out startValue4, 0, -1, true);
			base.Active = new DataItem<bool>(startValue4);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropAlwaysActive))
		{
			bool startValue5 = false;
			StringParsers.TryParseBool(this.Properties.Values[ItemClass.PropAlwaysActive], out startValue5, 0, -1, true);
			base.AlwaysActive = new DataItem<bool>(startValue5);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropPlaySoundOnActive))
		{
			string startValue6 = this.Properties.Values[ItemClass.PropPlaySoundOnActive].ToString();
			base.PlaySoundOnActive = new DataItem<string>(startValue6);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropLightSource))
		{
			this.LightSource = new DataItem<string>(this.Properties.Values[ItemClass.PropLightSource]);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropLightValue))
		{
			this.lightValue = StringParsers.ParseFloat(this.Properties.Values[ItemClass.PropLightValue], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropSoundSightIn))
		{
			this.soundSightIn = this.Properties.Values[ItemClass.PropSoundSightIn].ToString();
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropSoundSightOut))
		{
			this.soundSightOut = this.Properties.Values[ItemClass.PropSoundSightOut].ToString();
		}
		this.Properties.ParseBool(ItemClass.PropIgnoreKeystoneSound, ref this.ignoreKeystoneSound);
		if (this.Properties.Values.ContainsKey(ItemClass.PropActivateObject))
		{
			this.ActivateObject = new DataItem<string>(this.Properties.Values[ItemClass.PropActivateObject]);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropThrowableDecoy))
		{
			bool startValue7;
			StringParsers.TryParseBool(this.Properties.Values[ItemClass.PropThrowableDecoy], out startValue7, 0, -1, true);
			this.ThrowableDecoy = new DataItem<bool>(startValue7);
		}
		else
		{
			this.ThrowableDecoy = new DataItem<bool>(false);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropCustomIcon))
		{
			this.CustomIcon = new DataItem<string>(this.Properties.Values[ItemClass.PropCustomIcon]);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropCustomIconTint))
		{
			this.CustomIconTint = StringParsers.ParseHexColor(this.Properties.Values[ItemClass.PropCustomIconTint]);
		}
		else
		{
			this.CustomIconTint = Color.white;
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropGroupName))
		{
			string[] array = this.Properties.Values[ItemClass.PropGroupName].Split(',', StringSplitOptions.None);
			if (array.Length != 0)
			{
				this.Groups = new string[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					this.Groups[i] = array[i].Trim();
				}
			}
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropCritChance))
		{
			this.CritChance = new DataItem<float>(StringParsers.ParseFloat(this.Properties.Values[ItemClass.PropCritChance], 0, -1, NumberStyles.Any));
		}
		else
		{
			this.CritChance = new DataItem<float>(0f);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropVehicleSlotType))
		{
			this.VehicleSlotType = this.Properties.Values[ItemClass.PropVehicleSlotType];
		}
		else
		{
			this.VehicleSlotType = string.Empty;
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropHoldingItemHidden))
		{
			this.HoldingItemHidden = StringParsers.ParseBool(this.Properties.Values[ItemClass.PropHoldingItemHidden], 0, -1, true);
		}
		else
		{
			this.HoldingItemHidden = false;
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropCraftExpValue))
		{
			StringParsers.TryParseFloat(this.Properties.Values[ItemClass.PropCraftExpValue], out this.CraftComponentExp, 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropRepairExpMultiplier))
		{
			StringParsers.TryParseFloat(this.Properties.Values[ItemClass.PropRepairExpMultiplier], out this.RepairExpMultiplier, 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropCraftTimeValue))
		{
			StringParsers.TryParseFloat(this.Properties.Values[ItemClass.PropCraftTimeValue], out this.CraftComponentTime, 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropLootExpValue))
		{
			StringParsers.TryParseFloat(this.Properties.Values[ItemClass.PropLootExpValue], out this.LootExp, 0, -1, NumberStyles.Any);
		}
		this.Properties.ParseFloat(ItemClass.PropEconomicValue, ref this.EconomicValue);
		this.Properties.ParseFloat(ItemClass.PropEconomicSellScale, ref this.EconomicSellScale);
		this.Properties.ParseInt(ItemClass.PropEconomicBundleSize, ref this.EconomicBundleSize);
		this.Properties.ParseBool(ItemClass.PropSellableToTrader, ref this.SellableToTrader);
		if (this.Properties.Values.ContainsKey(ItemClass.PropCreativeMode))
		{
			this.CreativeMode = EnumUtils.Parse<EnumCreativeMode>(this.Properties.Values[ItemClass.PropCreativeMode], false);
		}
		this.SortOrder = this.Properties.GetString(ItemClass.PropCreativeSort1);
		this.SortOrder += this.Properties.GetString(ItemClass.PropCreativeSort2);
		if (this.Properties.Values.ContainsKey(ItemClass.PropCraftingSkillExp) && !int.TryParse(this.Properties.Values[ItemClass.PropCraftingSkillExp], out this.CraftingSkillExp))
		{
			this.CraftingSkillExp = 10;
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropActionSkillExp) && !int.TryParse(this.Properties.Values[ItemClass.PropActionSkillExp], out this.ActionSkillExp))
		{
			this.ActionSkillExp = 10;
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropInsulation) && !StringParsers.TryParseFloat(this.Properties.Values[ItemClass.PropInsulation], out this.Insulation, 0, -1, NumberStyles.Any))
		{
			this.Insulation = 0f;
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropWaterproof) && !StringParsers.TryParseFloat(this.Properties.Values[ItemClass.PropWaterproof], out this.WaterProof, 0, -1, NumberStyles.Any))
		{
			this.WaterProof = 0f;
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropEncumbrance) && !StringParsers.TryParseFloat(this.Properties.Values[ItemClass.PropEncumbrance], out this.Encumbrance, 0, -1, NumberStyles.Any))
		{
			this.Encumbrance = 0f;
		}
		this.Properties.ParseString(ItemClass.PropSoundPickup, ref this.SoundPickup);
		this.Properties.ParseString(ItemClass.PropSoundPlace, ref this.SoundPlace);
		this.Properties.ParseString(ItemClass.PropSoundHolster, ref this.SoundHolster);
		this.Properties.ParseString(ItemClass.PropSoundUnholster, ref this.SoundUnholster);
		this.Properties.ParseString(ItemClass.PropSoundStick, ref this.SoundStick);
		this.Properties.ParseString(ItemClass.PropSoundTick, ref this.SoundTick);
		this.Properties.ParseBool(ItemClass.PropHasReloadAnim, ref this.HasReloadAnim);
		if (this.SoundTick != null)
		{
			string[] array2 = this.SoundTick.Split(',', StringSplitOptions.None);
			this.SoundTick = array2[0];
			if (array2.Length >= 2)
			{
				this.SoundTickDelay = StringParsers.ParseFloat(array2[1], 0, -1, NumberStyles.Any);
			}
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropDescriptionKey))
		{
			this.DescriptionKey = this.Properties.Values[ItemClass.PropDescriptionKey];
		}
		else
		{
			this.DescriptionKey = base.Name + "Desc";
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropResourceUnit))
		{
			this.IsResourceUnit = StringParsers.ParseBool(this.Properties.Values[ItemClass.PropResourceUnit], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropMeltTimePerUnit))
		{
			this.MeltTimePerUnit = StringParsers.ParseFloat(this.Properties.Values[ItemClass.PropMeltTimePerUnit], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropActionSkillGroup))
		{
			this.ActionSkillGroup = this.Properties.Values[ItemClass.PropActionSkillGroup];
		}
		else
		{
			this.ActionSkillGroup = "";
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropCraftingSkillGroup))
		{
			this.CraftingSkillGroup = this.Properties.Values[ItemClass.PropCraftingSkillGroup];
		}
		else
		{
			this.CraftingSkillGroup = "";
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropCrosshairOnAim))
		{
			this.bShowCrosshairOnAiming = StringParsers.ParseBool(this.Properties.Values[ItemClass.PropCrosshairOnAim], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropCrosshairUpAfterShot))
		{
			this.bCrosshairUpAfterShot = StringParsers.ParseBool(this.Properties.Values[ItemClass.PropCrosshairUpAfterShot], 0, -1, true);
		}
		else
		{
			this.bCrosshairUpAfterShot = true;
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropUsableUnderwater))
		{
			this.UsableUnderwater = StringParsers.ParseBool(this.Properties.Values[ItemClass.PropUsableUnderwater], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropItemTypeIcon))
		{
			this.ItemTypeIcon = this.Properties.Values[ItemClass.PropItemTypeIcon];
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropAltItemTypeIcon))
		{
			this.AltItemTypeIcon = this.Properties.Values[ItemClass.PropAltItemTypeIcon];
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropAltItemTypeIconColor))
		{
			this.AltItemTypeIconColor = StringParsers.ParseHexColor(this.Properties.Values[ItemClass.PropAltItemTypeIconColor]);
		}
		else
		{
			this.AltItemTypeIconColor = Color.white;
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropUnlocks))
		{
			this.Unlocks = this.Properties.Values[ItemClass.PropUnlocks];
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropNavObject))
		{
			this.NavObject = this.Properties.Values[ItemClass.PropNavObject];
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropQuestItem))
		{
			this.IsQuestItem = StringParsers.ParseBool(this.Properties.Values[ItemClass.PropQuestItem], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropShowQuality))
		{
			this.ShowQualityBar = StringParsers.ParseBool(this.Properties.Values[ItemClass.PropShowQuality], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropNoScrapping))
		{
			this.NoScrapping = StringParsers.ParseBool(this.Properties.Values[ItemClass.PropNoScrapping], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropScrapTimeOverride))
		{
			this.ScrapTimeOverride = StringParsers.ParseFloat(this.Properties.Values[ItemClass.PropScrapTimeOverride], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Classes.ContainsKey("SDCS"))
		{
			this.SDCSData = new SDCSUtils.SlotData();
			if (this.Properties.Values.ContainsKey("SDCS.Prefab"))
			{
				this.SDCSData.PrefabName = this.Properties.Values["SDCS.Prefab"];
			}
			if (this.Properties.Values.ContainsKey("SDCS.TransformName"))
			{
				this.SDCSData.PartName = this.Properties.Values["SDCS.TransformName"];
			}
			if (this.Properties.Values.ContainsKey("SDCS.Excludes"))
			{
				this.SDCSData.BaseToTurnOff = this.Properties.Values["SDCS.Excludes"];
			}
			if (this.Properties.Values.ContainsKey("SDCS.CullDistFPV"))
			{
				this.SDCSData.CullDistance = StringParsers.ParseFloat(this.Properties.Values["SDCS.CullDistFPV"], 0, -1, NumberStyles.Any);
			}
			if (this.Properties.Values.ContainsKey("SDCS.HairMaskType"))
			{
				this.SDCSData.HairMaskType = (SDCSUtils.SlotData.HairMaskTypes)Enum.Parse(typeof(SDCSUtils.SlotData.HairMaskTypes), this.Properties.Values["SDCS.HairMaskType"]);
			}
			if (this.Properties.Values.ContainsKey("SDCS.FacialHairMaskType"))
			{
				this.SDCSData.FacialHairMaskType = (SDCSUtils.SlotData.HairMaskTypes)Enum.Parse(typeof(SDCSUtils.SlotData.HairMaskTypes), this.Properties.Values["SDCS.FacialHairMaskType"]);
			}
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropDisplayType))
		{
			this.DisplayType = this.Properties.Values[ItemClass.PropDisplayType];
		}
		else
		{
			this.DisplayType = "";
		}
		this.Properties.ParseString(ItemClass.PropTraderStageTemplate, ref this.TraderStageTemplate);
		this.Properties.ParseString(ItemClass.PropTrackerIndexName, ref this.TrackerIndexName);
		this.Properties.ParseString(ItemClass.PropTrackerNavObject, ref this.TrackerNavObject);
	}

	// Token: 0x06002C3D RID: 11325 RVA: 0x00128464 File Offset: 0x00126664
	public void LateInit()
	{
		if (this.Properties.Values.ContainsKey(ItemClass.PropSmell))
		{
			AIDirectorData.FindSmell(this.Properties.Values[ItemClass.PropSmell], out this.Smell);
		}
		if (this.HasQuality)
		{
			this.Stacknumber.Value = 1;
		}
	}

	// Token: 0x06002C3E RID: 11326 RVA: 0x001284C0 File Offset: 0x001266C0
	public static void InitStatic()
	{
		ItemClass.list = new ItemClass[ItemClass.MAX_ITEMS];
		ItemClass.itemActionNames = new string[5];
		for (int i = 0; i < 5; i++)
		{
			ItemClass.itemActionNames[i] = "Action" + i.ToString();
		}
	}

	// Token: 0x06002C3F RID: 11327 RVA: 0x0012850C File Offset: 0x0012670C
	public static void LateInitAll()
	{
		for (int i = 0; i < ItemClass.MAX_ITEMS; i++)
		{
			if (ItemClass.list[i] != null)
			{
				ItemClass.list[i].LateInit();
			}
		}
	}

	// Token: 0x06002C40 RID: 11328 RVA: 0x0012853E File Offset: 0x0012673E
	public static void Cleanup()
	{
		ItemClass.list = null;
		ItemClass.nameToItem.Clear();
		ItemClass.nameToItemCaseInsensitive.Clear();
		ItemClass.itemNames.Clear();
		ItemClass.itemActionNames = null;
	}

	// Token: 0x06002C41 RID: 11329 RVA: 0x0012856A File Offset: 0x0012676A
	public virtual int GetInitialMetadata(ItemValue _itemValue)
	{
		if (this.Actions[0] == null)
		{
			return 0;
		}
		return this.Actions[0].GetInitialMeta(_itemValue);
	}

	// Token: 0x06002C42 RID: 11330 RVA: 0x00128586 File Offset: 0x00126786
	public static ItemClass GetForId(int _id)
	{
		if (ItemClass.list == null || (ulong)_id >= (ulong)((long)ItemClass.list.Length))
		{
			return null;
		}
		return ItemClass.list[_id];
	}

	// Token: 0x06002C43 RID: 11331 RVA: 0x001285A4 File Offset: 0x001267A4
	public static ItemValue GetItem(string _itemName, bool _caseInsensitive = false)
	{
		ItemClass itemClass = ItemClass.GetItemClass(_itemName, _caseInsensitive);
		if (itemClass != null)
		{
			return new ItemValue(itemClass.Id, false);
		}
		return ItemValue.None.Clone();
	}

	// Token: 0x06002C44 RID: 11332 RVA: 0x001285D4 File Offset: 0x001267D4
	public static ItemClass GetItemClass(string _itemName, bool _caseInsensitive = false)
	{
		ItemClass result;
		if (_caseInsensitive)
		{
			ItemClass.nameToItemCaseInsensitive.TryGetValue(_itemName, out result);
		}
		else
		{
			ItemClass.nameToItem.TryGetValue(_itemName, out result);
		}
		return result;
	}

	// Token: 0x06002C45 RID: 11333 RVA: 0x00128604 File Offset: 0x00126804
	public static void GetItemsAndBlocks(List<ItemClass> _targetList, int _idStart = -1, int _idEndExcl = -1, ItemClass.FilterItem[] _filterExprs = null, string _nameFilter = null, bool _bShowUserHidden = false, EnumCreativeMode _currentCreativeMode = EnumCreativeMode.Player, bool _showFavorites = false, bool _sortBySortOrder = false, XUi _xui = null)
	{
		_targetList.Clear();
		if (_idStart < 0)
		{
			_idStart = 0;
		}
		if (_idEndExcl < 0)
		{
			_idEndExcl = ItemClass.list.Length;
		}
		if (string.IsNullOrEmpty(_nameFilter))
		{
			_nameFilter = null;
		}
		int num = -1;
		if (_nameFilter != null)
		{
			int.TryParse(_nameFilter, out num);
		}
		int i = _idStart;
		while (i < _idEndExcl)
		{
			Block block = null;
			if (i >= Block.ItemsStartHere)
			{
				goto IL_55;
			}
			block = Block.list[i];
			if (block != null)
			{
				goto IL_55;
			}
			IL_14C:
			i++;
			continue;
			IL_55:
			ItemClass forId = ItemClass.GetForId(i);
			if (forId == null)
			{
				goto IL_14C;
			}
			EnumCreativeMode creativeMode = forId.CreativeMode;
			if (creativeMode != EnumCreativeMode.None && creativeMode != EnumCreativeMode.Test && (creativeMode == EnumCreativeMode.All || _currentCreativeMode == creativeMode || _bShowUserHidden) && (creativeMode != EnumCreativeMode.Console || (DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent()) && (!_showFavorites || !(_xui != null) || _xui.playerUI.entityPlayer.favoriteCreativeStacks.Contains((ushort)i)))
			{
				if (_filterExprs != null)
				{
					bool flag = false;
					for (int j = 0; j < _filterExprs.Length; j++)
					{
						if (_filterExprs[j] != null)
						{
							flag = _filterExprs[j](forId, block);
							if (flag)
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						goto IL_14C;
					}
				}
				if (_nameFilter != null)
				{
					string a = forId.GetLocalizedItemName() ?? Localization.Get(forId.Name, false);
					if ((num < 0 || forId.Id != num) && !forId.Name.ContainsCaseInsensitive(_nameFilter) && !a.ContainsCaseInsensitive(_nameFilter))
					{
						goto IL_14C;
					}
				}
				_targetList.Add(forId);
				goto IL_14C;
			}
			goto IL_14C;
		}
		if (_sortBySortOrder)
		{
			_targetList.Sort(delegate(ItemClass _icA, ItemClass _icB)
			{
				int num2 = string.CompareOrdinal(_icA.SortOrder, _icB.SortOrder);
				if (num2 != 0)
				{
					return num2;
				}
				return _icA.Id.CompareTo(_icB.Id);
			});
		}
	}

	// Token: 0x06002C46 RID: 11334 RVA: 0x00128794 File Offset: 0x00126994
	public static void CreateItemStacks(IEnumerable<ItemClass> _itemClassList, List<ItemStack> _targetList)
	{
		_targetList.Clear();
		foreach (ItemClass itemClass in _itemClassList)
		{
			ItemValue itemValue = new ItemValue(itemClass.Id, true);
			itemValue.Meta = itemValue.ItemClass.GetInitialMetadata(itemValue);
			ItemStack item = new ItemStack(itemValue, itemClass.Stacknumber.Value);
			_targetList.Add(item);
		}
	}

	// Token: 0x06002C47 RID: 11335 RVA: 0x00128814 File Offset: 0x00126A14
	public virtual bool IsHUDDisabled(ItemInventoryData _data)
	{
		return (this.Actions[0] != null && this.Actions[0].IsHUDDisabled((_data != null) ? _data.actionData[0] : null)) || (this.Actions[1] != null && this.Actions[1].IsHUDDisabled((_data != null) ? _data.actionData[1] : null));
	}

	// Token: 0x06002C48 RID: 11336 RVA: 0x0012887C File Offset: 0x00126A7C
	public virtual bool IsLightSource()
	{
		return this.LightSource != null;
	}

	// Token: 0x06002C49 RID: 11337 RVA: 0x0012888A File Offset: 0x00126A8A
	public virtual Transform CloneModel(GameObject _reuseThisGO, World _world, BlockValue _blockValue, Vector3[] _vertices, Vector3 _position, Transform _parent, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World, TextureFullArray _textureFullArray = default(TextureFullArray))
	{
		return this.CloneModel(_world, _blockValue.ToItemValue(), _position, _parent, _purpose, _textureFullArray);
	}

	// Token: 0x06002C4A RID: 11338 RVA: 0x001288A4 File Offset: 0x00126AA4
	public virtual Transform CloneModel(World _world, ItemValue _itemValue, Vector3 _position, Transform _parent, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World, TextureFullArray _textureFullArray = default(TextureFullArray))
	{
		GameObject gameObject = null;
		if (this.CanHold())
		{
			string text = null;
			if (_purpose == BlockShape.MeshPurpose.Drop)
			{
				text = _itemValue.GetPropertyOverride("DropMeshFile", this.DropMeshFile);
			}
			if (_purpose == BlockShape.MeshPurpose.Hold)
			{
				text = _itemValue.GetPropertyOverride("HandMeshfile", this.HandMeshFile);
			}
			if (text == null)
			{
				text = _itemValue.GetPropertyOverride("Meshfile", this.MeshFile);
			}
			string text2 = (text != null) ? GameIO.GetFilenameFromPathWithoutExtension(text) : null;
			if (this.renderGameObject == null || (text != null && !text2.Equals(this.renderGameObject.name)))
			{
				this.renderGameObject = DataLoader.LoadAsset<GameObject>(text, false);
			}
			gameObject = this.renderGameObject;
			if (gameObject == null)
			{
				gameObject = LoadManager.LoadAsset<GameObject>("@:Other/Items/Crafting/leather.fbx", null, null, false, true).Asset;
			}
		}
		if (gameObject == null)
		{
			return null;
		}
		try
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
			Transform transform = gameObject2.transform;
			transform.SetParent(_parent, false);
			transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			gameObject2.SetActive(false);
			if (_purpose == BlockShape.MeshPurpose.Hold)
			{
				Collider[] componentsInChildren = gameObject2.GetComponentsInChildren<Collider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = false;
				}
				if (gameObject2 != null)
				{
					Animator[] componentsInChildren2 = gameObject2.GetComponentsInChildren<Animator>();
					for (int i = 0; i < componentsInChildren2.Length; i++)
					{
						componentsInChildren2[i].writeDefaultValuesOnDisable = true;
					}
				}
			}
			UpdateLightOnAllMaterials updateLightOnAllMaterials = gameObject2.GetComponent<UpdateLightOnAllMaterials>();
			if (updateLightOnAllMaterials == null)
			{
				updateLightOnAllMaterials = gameObject2.AddComponent<UpdateLightOnAllMaterials>();
			}
			string originalValue = "255,255,255";
			this.Properties.ParseString(Block.PropTintColor, ref originalValue);
			Vector3 tintColorForItem = Block.StringToVector3(_itemValue.GetPropertyOverride(Block.PropTintColor, originalValue));
			updateLightOnAllMaterials.SetTintColorForItem(tintColorForItem);
			return transform;
		}
		catch (Exception ex)
		{
			Log.Error("Instantiate of '" + this.MeshFile + "' led to error: " + ex.Message);
			Log.Error(ex.StackTrace);
		}
		return null;
	}

	// Token: 0x06002C4B RID: 11339 RVA: 0x00128A98 File Offset: 0x00126C98
	public void setLocalizedItemName(string _localizedName)
	{
		this.localizedName = _localizedName;
	}

	// Token: 0x06002C4C RID: 11340 RVA: 0x00128AA1 File Offset: 0x00126CA1
	public virtual string GetLocalizedItemName()
	{
		return this.localizedName;
	}

	// Token: 0x06002C4D RID: 11341 RVA: 0x00128AA9 File Offset: 0x00126CA9
	public void SetName(string _name)
	{
		this.pName = _name;
	}

	// Token: 0x06002C4E RID: 11342 RVA: 0x00128AB2 File Offset: 0x00126CB2
	public virtual string GetItemName()
	{
		return base.Name;
	}

	// Token: 0x06002C4F RID: 11343 RVA: 0x00128ABA File Offset: 0x00126CBA
	public virtual string GetItemDescriptionKey()
	{
		return this.DescriptionKey;
	}

	// Token: 0x06002C50 RID: 11344 RVA: 0x00128AC2 File Offset: 0x00126CC2
	public virtual string GetIconName()
	{
		if (this.CustomIcon != null && this.CustomIcon.Value.Length > 0)
		{
			return this.CustomIcon.Value;
		}
		return base.Name;
	}

	// Token: 0x06002C51 RID: 11345 RVA: 0x00128AF8 File Offset: 0x00126CF8
	public virtual Color GetIconTint(ItemValue _instance = null)
	{
		if (_instance != null)
		{
			string text = "NONE";
			string propertyOverride = _instance.GetPropertyOverride("CustomIconTint", text);
			if (!propertyOverride.Equals(text))
			{
				return StringParsers.ParseHexColor(propertyOverride);
			}
		}
		return this.CustomIconTint;
	}

	// Token: 0x06002C52 RID: 11346 RVA: 0x00128B31 File Offset: 0x00126D31
	public virtual bool IsGun()
	{
		return this.Actions[0] is ItemActionAttack;
	}

	// Token: 0x06002C53 RID: 11347 RVA: 0x00128B43 File Offset: 0x00126D43
	public virtual bool IsDynamicMelee()
	{
		return this.Actions[0] is ItemActionDynamic;
	}

	// Token: 0x06002C54 RID: 11348 RVA: 0x00128B55 File Offset: 0x00126D55
	public virtual bool CanStack()
	{
		return this.Stacknumber.Value > 1;
	}

	// Token: 0x06002C55 RID: 11349 RVA: 0x00128B65 File Offset: 0x00126D65
	public void SetCanHold(bool _b)
	{
		this.bCanHold = _b;
	}

	// Token: 0x06002C56 RID: 11350 RVA: 0x00128B6E File Offset: 0x00126D6E
	public virtual bool CanHold()
	{
		return this.bCanHold;
	}

	// Token: 0x06002C57 RID: 11351 RVA: 0x00128B76 File Offset: 0x00126D76
	public void SetCanDrop(bool _b)
	{
		this.bCanDrop = _b;
	}

	// Token: 0x06002C58 RID: 11352 RVA: 0x00128B7F File Offset: 0x00126D7F
	public virtual bool CanDrop(ItemValue _iv = null)
	{
		return this.bCanDrop;
	}

	// Token: 0x06002C59 RID: 11353 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Deactivate(ItemValue _iv)
	{
	}

	// Token: 0x06002C5A RID: 11354 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool KeepOnDeath()
	{
		return false;
	}

	// Token: 0x06002C5B RID: 11355 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool CanPlaceInContainer()
	{
		return true;
	}

	// Token: 0x06002C5C RID: 11356 RVA: 0x00128B88 File Offset: 0x00126D88
	public virtual string CanInteract(ItemInventoryData _data)
	{
		ItemAction itemAction = this.Actions[2];
		if (itemAction == null)
		{
			return null;
		}
		return itemAction.CanInteract(_data.actionData[2]);
	}

	// Token: 0x06002C5D RID: 11357 RVA: 0x00128BB5 File Offset: 0x00126DB5
	public void Interact(ItemInventoryData _data)
	{
		this.ExecuteAction(2, _data, false, null);
		this.ExecuteAction(2, _data, true, null);
	}

	// Token: 0x06002C5E RID: 11358 RVA: 0x00128BCC File Offset: 0x00126DCC
	public bool CanExecuteAction(int actionIdx, EntityAlive holdingEntity, ItemValue itemValue)
	{
		bool flag = true;
		ItemAction itemAction = this.Actions[actionIdx];
		if (itemAction != null)
		{
			List<IRequirement> executionRequirements = itemAction.ExecutionRequirements;
			if (executionRequirements != null)
			{
				holdingEntity.MinEventContext.ItemValue = itemValue;
				for (int i = 0; i < executionRequirements.Count; i++)
				{
					flag &= executionRequirements[i].IsValid(holdingEntity.MinEventContext);
					if (!flag)
					{
						break;
					}
				}
			}
		}
		return flag;
	}

	// Token: 0x06002C5F RID: 11359 RVA: 0x00128C28 File Offset: 0x00126E28
	public virtual void ExecuteAction(int _actionIdx, ItemInventoryData _data, bool _bReleased, PlayerActionsLocal _playerActions)
	{
		ItemAction curAction = this.Actions[_actionIdx];
		if (curAction == null)
		{
			return;
		}
		if (curAction is ItemActionDynamicMelee)
		{
			bool flag = _bReleased;
			if (this.Actions.Length >= 2 && this.Actions[0] != null && this.Actions[1] != null)
			{
				flag = (!this.Actions[1].IsActionRunning(_data.actionData[1]) && !this.Actions[0].IsActionRunning(_data.actionData[0]));
			}
			if (!flag)
			{
				return;
			}
			if (!_bReleased)
			{
				flag &= curAction.CanExecute(_data.actionData[_actionIdx]);
			}
			if (!flag)
			{
				return;
			}
			if (_data != null && _data.holdingEntity.emodel != null && _data.holdingEntity.emodel.avatarController != null)
			{
				_data.holdingEntity.emodel.avatarController.UpdateInt(AvatarController.itemActionIndexHash, _actionIdx, true);
			}
			curAction.ExecuteAction((_data != null) ? _data.actionData[_actionIdx] : null, _bReleased);
			return;
		}
		else
		{
			global::ItemActionData actionData = _data.actionData[_actionIdx];
			bool flag2 = _bReleased || !curAction.IsActionRunning(actionData);
			if (!flag2)
			{
				return;
			}
			if (!_bReleased)
			{
				flag2 &= curAction.CanExecute(actionData);
			}
			if (!flag2)
			{
				GameManager.ShowTooltip(_data.holdingEntity as EntityPlayerLocal, Localization.Get("ttCannotUseAtThisTime", false), string.Empty, "ui_denied", null, false, false, 0f);
				return;
			}
			if (_data != null && _data.holdingEntity.emodel != null && _data.holdingEntity.emodel.avatarController != null)
			{
				_data.holdingEntity.emodel.avatarController.UpdateInt(AvatarController.itemActionIndexHash, _actionIdx, true);
			}
			if (!_bReleased)
			{
				if (!actionData.HasExecuted)
				{
					_data.holdingEntity.MinEventContext.ItemValue = _data.itemValue;
					_data.holdingEntity.FireEvent(MinEvent.Start[_actionIdx], true);
				}
				if (curAction is ItemActionRanged && !(curAction is ItemActionLauncher) && !(curAction is ItemActionCatapult))
				{
					actionData.HasExecuted = true;
					curAction.ExecuteAction(actionData, _bReleased);
					return;
				}
				if (!actionData.HasExecuted)
				{
					ItemActionEat itemActionEat = curAction as ItemActionEat;
					if (itemActionEat != null)
					{
						LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_data.holdingEntity as EntityPlayerLocal);
						XUi xui = (uiforPlayer != null) ? uiforPlayer.xui : null;
						if (itemActionEat.UsePrompt && !xui.isUsingItemActionEntryPromptComplete)
						{
							XUiC_MessageBoxWindowGroup.ShowMessageBox(xui, Localization.Get(itemActionEat.PromptTitle, false), Localization.Get(itemActionEat.PromptDescription, false), XUiC_MessageBoxWindowGroup.MessageBoxTypes.OkCancel, delegate()
							{
								_data.holdingEntity.MinEventContext.ItemValue = _data.holdingEntity.inventory.holdingItemItemValue;
								actionData.HasExecuted = false;
								curAction.ExecuteAction(actionData, true);
								xui.isUsingItemActionEntryPromptComplete = false;
							}, null, true, true, true);
							return;
						}
						xui.isUsingItemActionEntryPromptComplete = false;
					}
					_data.holdingEntity.MinEventContext.ItemValue = _data.holdingEntity.inventory.holdingItemItemValue;
					actionData.HasExecuted = true;
					curAction.ExecuteAction(actionData, _bReleased);
					return;
				}
			}
			else if (actionData.HasExecuted)
			{
				if (!(curAction is ItemActionUseOther))
				{
					ItemValue itemValue = _data.itemValue;
					if (!curAction.IsEndDelayed() || !curAction.UseAnimation)
					{
						_data.holdingEntity.MinEventContext.ItemValue = itemValue;
						_data.holdingEntity.FireEvent(MinEvent.End[_actionIdx], true);
					}
					if (_data.holdingEntity as EntityPlayerLocal != null)
					{
						ItemClass itemClass = itemValue.ItemClass;
						if (itemClass != null && itemClass.HasAnyTags(this.stopBleed) && _data.holdingEntity.Buffs.HasBuff("buffInjuryBleeding"))
						{
							IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
							if (achievementManager != null)
							{
								achievementManager.SetAchievementStat(EnumAchievementDataStat.BleedOutStopped, 1);
							}
						}
					}
				}
				if (curAction is ItemActionActivate && _bReleased && !(flag2 & curAction.CanExecute(actionData)))
				{
					GameManager.ShowTooltip(_data.holdingEntity as EntityPlayerLocal, Localization.Get("ttCannotUseAtThisTime", false), string.Empty, "ui_denied", null, false, false, 0f);
					actionData.HasExecuted = true;
					return;
				}
				_data.holdingEntity.MinEventContext.ItemValue = _data.holdingEntity.inventory.holdingItemItemValue;
				actionData.HasExecuted = false;
				curAction.ExecuteAction(actionData, _bReleased);
			}
			return;
		}
	}

	// Token: 0x06002C60 RID: 11360 RVA: 0x00129174 File Offset: 0x00127374
	public virtual bool IsActionRunning(ItemInventoryData _data)
	{
		for (int i = 0; i < 3; i++)
		{
			ItemAction itemAction = this.Actions[i];
			if (itemAction != null && itemAction.IsActionRunning(_data.actionData[i]))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002C61 RID: 11361 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnHoldingItemActivated(ItemInventoryData _data)
	{
	}

	// Token: 0x06002C62 RID: 11362 RVA: 0x001291B0 File Offset: 0x001273B0
	public virtual void StartHolding(ItemInventoryData _data, Transform _modelTransform)
	{
		for (int i = 0; i < 3; i++)
		{
			ItemAction itemAction = this.Actions[i];
			if (itemAction != null)
			{
				itemAction.StartHolding(_data.actionData[i]);
			}
		}
		if (this.Actions[0] != null || this.Actions[1] != null)
		{
			_data.holdingEntitySoundID = -1;
		}
	}

	// Token: 0x06002C63 RID: 11363 RVA: 0x00129204 File Offset: 0x00127404
	public virtual void CleanupHoldingActions(ItemInventoryData _data)
	{
		if (_data == null)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			ItemAction itemAction = this.Actions[i];
			if (itemAction != null)
			{
				itemAction.Cleanup(_data.actionData[i]);
			}
		}
	}

	// Token: 0x06002C64 RID: 11364 RVA: 0x00129240 File Offset: 0x00127440
	public virtual void OnHoldingUpdate(ItemInventoryData _data)
	{
		EntityAlive holdingEntity = _data.holdingEntity;
		for (int i = 0; i < 3; i++)
		{
			ItemAction itemAction = this.Actions[i];
			if (itemAction != null)
			{
				holdingEntity.MinEventContext.ItemValue = holdingEntity.inventory.holdingItemItemValue;
				holdingEntity.MinEventContext.ItemActionData = _data.actionData[i];
				holdingEntity.FireEvent(MinEvent.Update[i], true);
				itemAction.OnHoldingUpdate((_data != null) ? _data.actionData[i] : null);
			}
		}
		if (this.Properties.Values.ContainsKey(ItemClass.PropSoundIdle) && !_data.holdingEntity.isEntityRemote)
		{
			if (_data.holdingEntitySoundID == 0 && _data.itemValue.Meta == 0)
			{
				Manager.BroadcastStop(_data.holdingEntity.entityId, this.Properties.Values[ItemClass.PropSoundIdle]);
				return;
			}
			if (_data.holdingEntitySoundID == -1 && _data.itemValue.Meta > 0)
			{
				Manager.BroadcastPlay(_data.holdingEntity, this.Properties.Values[ItemClass.PropSoundIdle], false);
				_data.holdingEntitySoundID = 0;
			}
		}
	}

	// Token: 0x06002C65 RID: 11365 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnHoldingReset(ItemInventoryData _data)
	{
	}

	// Token: 0x06002C66 RID: 11366 RVA: 0x00129360 File Offset: 0x00127560
	public void StopHoldingAudio(ItemInventoryData _data)
	{
		if (this.Properties.Values[ItemClass.PropSoundIdle] != null && _data.holdingEntitySoundID == 0)
		{
			Manager.BroadcastStop(_data.holdingEntity.entityId, this.Properties.Values[ItemClass.PropSoundIdle]);
		}
		_data.holdingEntitySoundID = -2;
	}

	// Token: 0x06002C67 RID: 11367 RVA: 0x001293BC File Offset: 0x001275BC
	public virtual void StopHolding(ItemInventoryData _data, Transform _modelTransform)
	{
		this.StopHoldingAudio(_data);
		if (_data.holdingEntity is EntityPlayer && !_data.holdingEntity.isEntityRemote && _data.holdingEntity.AimingGun)
		{
			_data.holdingEntity.AimingGun = false;
		}
		for (int i = 0; i < 3; i++)
		{
			ItemAction itemAction = this.Actions[i];
			if (itemAction != null)
			{
				itemAction.StopHolding(_data.actionData[i]);
			}
		}
	}

	// Token: 0x06002C68 RID: 11368 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnMeshCreated(ItemWorldData _data)
	{
	}

	// Token: 0x06002C69 RID: 11369 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnDroppedUpdate(ItemWorldData _data)
	{
	}

	// Token: 0x06002C6A RID: 11370 RVA: 0x0004712B File Offset: 0x0004532B
	public virtual BlockValue OnConvertToBlockValue(ItemValue _itemValue, BlockValue _blueprintBlockValue)
	{
		return _blueprintBlockValue;
	}

	// Token: 0x06002C6B RID: 11371 RVA: 0x00129430 File Offset: 0x00127630
	public ItemInventoryData CreateInventoryData(ItemStack _itemStack, IGameManager _gameManager, EntityAlive _holdingEntity, int _slotIdxInInventory)
	{
		ItemInventoryData itemInventoryData = this.createItemInventoryData(_itemStack, _gameManager, _holdingEntity, _slotIdxInInventory);
		itemInventoryData.actionData[0] = ((this.Actions[0] != null) ? this.Actions[0].CreateModifierData(itemInventoryData, 0) : null);
		itemInventoryData.actionData[1] = ((this.Actions[1] != null) ? this.Actions[1].CreateModifierData(itemInventoryData, 1) : null);
		if (this.Actions[2] != null)
		{
			itemInventoryData.actionData.Add(this.Actions[2].CreateModifierData(itemInventoryData, 2));
		}
		return itemInventoryData;
	}

	// Token: 0x06002C6C RID: 11372 RVA: 0x001294BE File Offset: 0x001276BE
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual ItemInventoryData createItemInventoryData(ItemStack _itemStack, IGameManager _gameManager, EntityAlive _holdingEntity, int _slotIdxInInventory)
	{
		return new ItemInventoryData(this, _itemStack, _gameManager, _holdingEntity, _slotIdxInInventory);
	}

	// Token: 0x06002C6D RID: 11373 RVA: 0x001294CB File Offset: 0x001276CB
	public virtual ItemWorldData CreateWorldData(IGameManager _gm, EntityItem _entityItem, ItemValue _itemValue, int _belongsEntityId)
	{
		return new ItemWorldData(_gm, _itemValue, _entityItem, _belongsEntityId);
	}

	// Token: 0x06002C6E RID: 11374 RVA: 0x001294D8 File Offset: 0x001276D8
	public virtual void OnHUD(ItemInventoryData _data, int _x, int _y)
	{
		if (this.Actions[0] != null)
		{
			this.Actions[0].OnHUD(_data.actionData[0], _x, _y);
		}
		if (this.Actions[1] != null)
		{
			this.Actions[1].OnHUD(_data.actionData[1], _x, _y);
		}
	}

	// Token: 0x06002C6F RID: 11375 RVA: 0x00129530 File Offset: 0x00127730
	public virtual void OnScreenOverlay(ItemInventoryData _data)
	{
		if (this.Actions[0] != null)
		{
			this.Actions[0].OnScreenOverlay(_data.actionData[0]);
		}
		if (this.Actions[1] != null)
		{
			this.Actions[1].OnScreenOverlay(_data.actionData[1]);
		}
	}

	// Token: 0x06002C70 RID: 11376 RVA: 0x00129584 File Offset: 0x00127784
	public virtual RenderCubeType GetFocusType(ItemInventoryData _data)
	{
		if (!this.CanHold())
		{
			return RenderCubeType.None;
		}
		RenderCubeType renderCubeType = RenderCubeType.None;
		if (this.Actions[0] != null)
		{
			renderCubeType = this.Actions[0].GetFocusType((_data != null) ? _data.actionData[0] : null);
		}
		RenderCubeType renderCubeType2 = RenderCubeType.None;
		if (this.Actions[1] != null)
		{
			renderCubeType2 = this.Actions[1].GetFocusType((_data != null) ? _data.actionData[1] : null);
		}
		if (renderCubeType <= renderCubeType2)
		{
			return renderCubeType2;
		}
		return renderCubeType;
	}

	// Token: 0x06002C71 RID: 11377 RVA: 0x001295FA File Offset: 0x001277FA
	public virtual float GetFocusRange()
	{
		if (this.Actions[0] != null && this.Actions[0] is ItemActionAttack)
		{
			return ((ItemActionAttack)this.Actions[0]).Range;
		}
		return 0f;
	}

	// Token: 0x06002C72 RID: 11378 RVA: 0x00129630 File Offset: 0x00127830
	public virtual bool IsFocusBlockInside()
	{
		bool flag = false;
		if (this.Actions[0] != null)
		{
			flag = this.Actions[0].IsFocusBlockInside();
		}
		bool flag2 = false;
		if (this.Actions[1] != null)
		{
			flag2 = this.Actions[1].IsFocusBlockInside();
		}
		return flag2 && flag;
	}

	// Token: 0x06002C73 RID: 11379 RVA: 0x00129674 File Offset: 0x00127874
	public virtual bool ConsumeScrollWheel(ItemInventoryData _data, float _scrollWheelInput, PlayerActionsLocal _playerInput)
	{
		bool flag = false;
		if (this.Actions[0] != null)
		{
			flag = this.Actions[0].ConsumeScrollWheel(_data.actionData[0], _scrollWheelInput, _playerInput);
		}
		if (!flag && this.Actions[1] != null)
		{
			flag = this.Actions[1].ConsumeScrollWheel(_data.actionData[1], _scrollWheelInput, _playerInput);
		}
		return flag;
	}

	// Token: 0x06002C74 RID: 11380 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void CheckKeys(ItemInventoryData _data, WorldRayHitInfo _hitInfo)
	{
	}

	// Token: 0x06002C75 RID: 11381 RVA: 0x001296D3 File Offset: 0x001278D3
	public virtual float GetLifetimeOnDrop()
	{
		return 60f;
	}

	// Token: 0x06002C76 RID: 11382 RVA: 0x00019766 File Offset: 0x00017966
	public virtual Block GetBlock()
	{
		return null;
	}

	// Token: 0x06002C77 RID: 11383 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsBlock()
	{
		return false;
	}

	// Token: 0x06002C78 RID: 11384 RVA: 0x001296DC File Offset: 0x001278DC
	public virtual ItemClass.EnumCrosshairType GetCrosshairType(ItemInventoryData _holdingData)
	{
		ItemClass.EnumCrosshairType enumCrosshairType = ItemClass.EnumCrosshairType.Plus;
		ItemClass.EnumCrosshairType enumCrosshairType2 = ItemClass.EnumCrosshairType.Plus;
		if (this.Actions[0] != null)
		{
			enumCrosshairType = this.Actions[0].GetCrosshairType(_holdingData.actionData[0]);
		}
		if (this.Actions[1] != null)
		{
			enumCrosshairType2 = this.Actions[1].GetCrosshairType(_holdingData.actionData[1]);
		}
		ItemClass.EnumCrosshairType enumCrosshairType3 = (enumCrosshairType > enumCrosshairType2) ? enumCrosshairType : enumCrosshairType2;
		string propertyOverride = _holdingData.itemValue.GetPropertyOverride(ItemClass.PropCrosshairOnAim, string.Empty);
		if (propertyOverride.Length > 0)
		{
			this.bShowCrosshairOnAiming = StringParsers.ParseBool(propertyOverride, 0, -1, true);
		}
		if (enumCrosshairType3 == ItemClass.EnumCrosshairType.Crosshair && this.bShowCrosshairOnAiming)
		{
			enumCrosshairType3 = ItemClass.EnumCrosshairType.CrosshairOnAiming;
		}
		return enumCrosshairType3;
	}

	// Token: 0x06002C79 RID: 11385 RVA: 0x0012977C File Offset: 0x0012797C
	public virtual void GetIronSights(ItemInventoryData _invData, out float _fov)
	{
		if (this.Actions[0] != null)
		{
			this.Actions[0].GetIronSights(_invData.actionData[0], out _fov);
			if (_fov != 0f)
			{
				return;
			}
		}
		if (this.Actions[1] != null)
		{
			this.Actions[1].GetIronSights(_invData.actionData[1], out _fov);
			if (_fov != 0f)
			{
				return;
			}
		}
		_fov = (float)_invData.holdingEntity.GetCameraFOV();
	}

	// Token: 0x06002C7A RID: 11386 RVA: 0x001297F4 File Offset: 0x001279F4
	public virtual EnumCameraShake GetCameraShakeType(ItemInventoryData _invData)
	{
		EnumCameraShake enumCameraShake = EnumCameraShake.None;
		EnumCameraShake enumCameraShake2 = EnumCameraShake.None;
		if (this.Actions[0] != null)
		{
			enumCameraShake = this.Actions[0].GetCameraShakeType(_invData.actionData[0]);
		}
		if (this.Actions[1] != null)
		{
			enumCameraShake2 = this.Actions[1].GetCameraShakeType(_invData.actionData[1]);
		}
		if (enumCameraShake > enumCameraShake2)
		{
			return enumCameraShake;
		}
		return enumCameraShake2;
	}

	// Token: 0x06002C7B RID: 11387 RVA: 0x00129854 File Offset: 0x00127A54
	public virtual TriggerEffectManager.ControllerTriggerEffect GetControllerTriggerEffectPull()
	{
		if (this.Actions[0] != null)
		{
			return this.Actions[0].GetControllerTriggerEffectPull();
		}
		if (this.Actions[1] != null)
		{
			return this.Actions[1].GetControllerTriggerEffectPull();
		}
		return TriggerEffectManager.NoneEffect;
	}

	// Token: 0x06002C7C RID: 11388 RVA: 0x0012988B File Offset: 0x00127A8B
	public virtual TriggerEffectManager.ControllerTriggerEffect GetControllerTriggerEffectShoot()
	{
		if (this.Actions[0] != null)
		{
			return this.Actions[0].GetControllerTriggerEffectShoot();
		}
		if (this.Actions[1] != null)
		{
			return this.Actions[1].GetControllerTriggerEffectShoot();
		}
		return TriggerEffectManager.NoneEffect;
	}

	// Token: 0x06002C7D RID: 11389 RVA: 0x001298C2 File Offset: 0x00127AC2
	public virtual bool IsActivated(ItemValue _value)
	{
		return _value.Activated > 0;
	}

	// Token: 0x06002C7E RID: 11390 RVA: 0x001298CD File Offset: 0x00127ACD
	public virtual void SetActivated(ref ItemValue _value, bool _activated)
	{
		_value.Activated = (_activated ? 1 : 0);
	}

	// Token: 0x06002C7F RID: 11391 RVA: 0x001298DE File Offset: 0x00127ADE
	public virtual Vector3 GetDroppedCorrectionRotation()
	{
		return new Vector3(-90f, 0f, 0f);
	}

	// Token: 0x06002C80 RID: 11392 RVA: 0x000470CA File Offset: 0x000452CA
	public virtual Vector3 GetCorrectionRotation()
	{
		return Vector3.zero;
	}

	// Token: 0x06002C81 RID: 11393 RVA: 0x000470CA File Offset: 0x000452CA
	public virtual Vector3 GetCorrectionPosition()
	{
		return Vector3.zero;
	}

	// Token: 0x06002C82 RID: 11394 RVA: 0x000470CA File Offset: 0x000452CA
	public virtual Vector3 GetCorrectionScale()
	{
		return Vector3.zero;
	}

	// Token: 0x06002C83 RID: 11395 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnDamagedByExplosion(ItemWorldData _data)
	{
	}

	// Token: 0x06002C84 RID: 11396 RVA: 0x001298F4 File Offset: 0x00127AF4
	public static int GetFuelValue(ItemValue _itemValue)
	{
		ItemClass itemClass = ItemClass.list[_itemValue.type];
		if (itemClass == null)
		{
			return 0;
		}
		if (itemClass.IsBlock())
		{
			return Block.list[_itemValue.type].FuelValue;
		}
		if (itemClass.FuelValue == null)
		{
			return 0;
		}
		return itemClass.FuelValue.Value;
	}

	// Token: 0x06002C85 RID: 11397 RVA: 0x00129942 File Offset: 0x00127B42
	public int GetWeight()
	{
		if (this.IsBlock())
		{
			return this.GetBlock().GetWeight();
		}
		if (base.Weight != null)
		{
			return base.Weight.Value;
		}
		return 0;
	}

	// Token: 0x06002C86 RID: 11398 RVA: 0x0012996D File Offset: 0x00127B6D
	public string GetImageEffect()
	{
		if (base.ImageEffectOnActive != null)
		{
			return base.ImageEffectOnActive.Value;
		}
		return "";
	}

	// Token: 0x06002C87 RID: 11399 RVA: 0x0012998E File Offset: 0x00127B8E
	public bool GetActive()
	{
		return base.Active != null && base.Active.Value;
	}

	// Token: 0x06002C88 RID: 11400 RVA: 0x001299A5 File Offset: 0x00127BA5
	public string GetSoundOnActive()
	{
		if (base.PlaySoundOnActive != null)
		{
			return base.PlaySoundOnActive.Value;
		}
		return "";
	}

	// Token: 0x06002C89 RID: 11401 RVA: 0x001299C6 File Offset: 0x00127BC6
	public void SetWeight(int _w)
	{
		if (this.IsBlock())
		{
			this.GetBlock().Weight = new DataItem<int>(_w);
			return;
		}
		base.Weight = new DataItem<int>(_w);
	}

	// Token: 0x06002C8A RID: 11402 RVA: 0x001299EE File Offset: 0x00127BEE
	public void SetImageEffect(string _str)
	{
		base.ImageEffectOnActive = new DataItem<string>(_str);
	}

	// Token: 0x06002C8B RID: 11403 RVA: 0x001299FC File Offset: 0x00127BFC
	public void SetActive(bool _bOn)
	{
		base.Active = new DataItem<bool>(_bOn);
	}

	// Token: 0x06002C8C RID: 11404 RVA: 0x00129A0A File Offset: 0x00127C0A
	public void SetSoundOnActive(string _str)
	{
		base.PlaySoundOnActive = new DataItem<string>(_str);
	}

	// Token: 0x06002C8D RID: 11405 RVA: 0x00129A18 File Offset: 0x00127C18
	public int AutoCalcWeight(Dictionary<string, List<Recipe>> _recipesByName)
	{
		Block block = this.IsBlock() ? this.GetBlock() : null;
		if (block != null)
		{
			if (block.Weight != null)
			{
				if (block.Weight.Value != -1)
				{
					return block.Weight.Value;
				}
				return 0;
			}
			else
			{
				block.Weight = new DataItem<int>(-1);
			}
		}
		else if (base.Weight != null)
		{
			if (base.Weight.Value != -1)
			{
				return base.Weight.Value;
			}
			return 0;
		}
		else
		{
			base.Weight = new DataItem<int>(-1);
		}
		int num = 0;
		int num2 = 0;
		List<Recipe> list;
		if (_recipesByName.TryGetValue(this.GetItemName(), out list))
		{
			Recipe recipe = list[0];
			for (int i = 0; i < recipe.ingredients.Count; i++)
			{
				ItemStack itemStack = recipe.ingredients[i];
				ItemClass forId = ItemClass.GetForId(itemStack.itemValue.type);
				ItemClass forId2 = ItemClass.GetForId(recipe.itemValueType);
				if (recipe.materialBasedRecipe)
				{
					if (forId2.MadeOfMaterial.ForgeCategory != null && forId.MadeOfMaterial.ForgeCategory != null && forId2.MadeOfMaterial.ForgeCategory.EqualsCaseInsensitive(forId.MadeOfMaterial.ForgeCategory))
					{
						num += forId.AutoCalcWeight(_recipesByName) * itemStack.count;
						num2++;
						break;
					}
				}
				else if (forId2.MadeOfMaterial.ForgeCategory != null && forId.MadeOfMaterial.ForgeCategory != null && forId2.MadeOfMaterial.ForgeCategory.EqualsCaseInsensitive(forId.MadeOfMaterial.ForgeCategory))
				{
					if (ItemClass.GetForId(itemStack.itemValue.type).GetWeight() > 0)
					{
						num += ItemClass.GetForId(itemStack.itemValue.type).GetWeight() * itemStack.count;
						num2++;
					}
					else
					{
						num += forId.AutoCalcWeight(_recipesByName) * itemStack.count;
						num2++;
					}
				}
			}
			num /= ((num2 > 1) ? recipe.count : 1);
		}
		if (block != null)
		{
			block.Weight = new DataItem<int>(num);
		}
		else
		{
			base.Weight = new DataItem<int>(num);
		}
		return num;
	}

	// Token: 0x06002C8E RID: 11406 RVA: 0x00129C3B File Offset: 0x00127E3B
	public virtual bool HasAnyTags(FastTags<TagGroup.Global> _tags)
	{
		return this.ItemTags.Test_AnySet(_tags);
	}

	// Token: 0x06002C8F RID: 11407 RVA: 0x00129C49 File Offset: 0x00127E49
	public virtual bool HasAllTags(FastTags<TagGroup.Global> _tags)
	{
		return this.ItemTags.Test_AllSet(_tags);
	}

	// Token: 0x06002C90 RID: 11408 RVA: 0x00129C58 File Offset: 0x00127E58
	public static ItemClass GetItemWithTag(FastTags<TagGroup.Global> _tags)
	{
		if (ItemClass.list != null)
		{
			for (int i = 0; i < ItemClass.list.Length; i++)
			{
				if (ItemClass.list[i] != null && ItemClass.list[i].HasAllTags(_tags))
				{
					return ItemClass.list[i];
				}
			}
		}
		return null;
	}

	// Token: 0x06002C91 RID: 11409 RVA: 0x00129CA0 File Offset: 0x00127EA0
	public static List<ItemClass> GetItemsWithTag(FastTags<TagGroup.Global> _tags)
	{
		List<ItemClass> list = new List<ItemClass>();
		if (ItemClass.list != null)
		{
			for (int i = 0; i < ItemClass.list.Length; i++)
			{
				if (ItemClass.list[i] != null && ItemClass.list[i].HasAllTags(_tags))
				{
					list.Add(ItemClass.list[i]);
				}
			}
		}
		return list;
	}

	// Token: 0x06002C92 RID: 11410 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool CanCollect(ItemValue _iv)
	{
		return true;
	}

	// Token: 0x06002C93 RID: 11411 RVA: 0x00129CF4 File Offset: 0x00127EF4
	public float AutoCalcEcoVal(Dictionary<string, List<Recipe>> _recipesByName, List<string> _recipeCalcStack)
	{
		string itemName = this.GetItemName();
		if (_recipeCalcStack.ContainsWithComparer(itemName, StringComparer.Ordinal))
		{
			return -1f;
		}
		Block block = this.IsBlock() ? this.GetBlock() : null;
		float num = (block != null) ? block.EconomicValue : this.EconomicValue;
		if (num > 0f)
		{
			return num;
		}
		if ((double)num < -0.1)
		{
			return 0f;
		}
		_recipeCalcStack.Add(itemName);
		float num2 = 0f;
		int num3 = 0;
		List<Recipe> list;
		if (_recipesByName.TryGetValue(itemName, out list))
		{
			foreach (Recipe recipe in list)
			{
				for (int i = 0; i < recipe.ingredients.Count; i++)
				{
					ItemStack itemStack = recipe.ingredients[i];
					float num4 = ItemClass.GetForId(itemStack.itemValue.type).AutoCalcEcoVal(_recipesByName, _recipeCalcStack);
					if (num4 < 0f)
					{
						num2 = -1f;
						break;
					}
					num2 += (float)itemStack.count * num4;
					num3++;
				}
				if (num2 >= 0f)
				{
					num2 /= (float)((num3 > 1) ? recipe.count : 1);
					break;
				}
			}
		}
		_recipeCalcStack.RemoveAt(_recipeCalcStack.Count - 1);
		if (num2 < 0f)
		{
			return -1f;
		}
		if (num2 == 0f)
		{
			num2 = 1f;
		}
		if (block != null)
		{
			block.EconomicValue = num2;
		}
		this.EconomicValue = num2;
		return num2;
	}

	// Token: 0x06002C94 RID: 11412 RVA: 0x00129E84 File Offset: 0x00128084
	public void FireEvent(MinEventTypes _eventType, MinEventParams _eventParms)
	{
		if (this.Effects != null)
		{
			this.Effects.FireEvent(_eventType, _eventParms);
		}
	}

	// Token: 0x1700046B RID: 1131
	// (get) Token: 0x06002C95 RID: 11413 RVA: 0x00129E9B File Offset: 0x0012809B
	public bool IsEatDistraction
	{
		get
		{
			return this.DistractionTags.Test_AnySet(ItemClass.EatDistractionTag);
		}
	}

	// Token: 0x1700046C RID: 1132
	// (get) Token: 0x06002C96 RID: 11414 RVA: 0x00129EAD File Offset: 0x001280AD
	public bool IsRequireContactDistraction
	{
		get
		{
			return this.DistractionTags.Test_AnySet(ItemClass.RequiresContactDistractionTag);
		}
	}

	// Token: 0x06002C97 RID: 11415 RVA: 0x00129EBF File Offset: 0x001280BF
	public bool HasTrigger(MinEventTypes _eventType)
	{
		return this.Effects != null && this.Effects.HasTrigger(_eventType);
	}

	// Token: 0x06002C98 RID: 11416 RVA: 0x00129ED8 File Offset: 0x001280D8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void assignIdsFromXml()
	{
		Log.Out("ItemIDs from XML");
		foreach (KeyValuePair<string, ItemClass> keyValuePair in ItemClass.nameToItem)
		{
			if (!keyValuePair.Value.IsBlock())
			{
				ItemClass.assignId(keyValuePair.Value, keyValuePair.Value.Id, null);
			}
		}
	}

	// Token: 0x06002C99 RID: 11417 RVA: 0x00129F54 File Offset: 0x00128154
	[PublicizedFrom(EAccessModifier.Private)]
	public static void assignIdsLinear()
	{
		Log.Out("ItemIDs linear");
		bool[] usedIds = new bool[ItemClass.MAX_ITEMS];
		List<ItemClass> list = new List<ItemClass>(ItemClass.nameToItem.Count);
		ItemClass.nameToItem.CopyValuesTo(list);
		ItemClass.assignLeftOverItems(usedIds, list);
	}

	// Token: 0x06002C9A RID: 11418 RVA: 0x00129F96 File Offset: 0x00128196
	[PublicizedFrom(EAccessModifier.Private)]
	public static void assignId(ItemClass _b, int _id, bool[] _usedIds)
	{
		ItemClass.list[_id] = _b;
		_b.SetId(_id);
		if (_usedIds != null)
		{
			_usedIds[_id] = true;
		}
	}

	// Token: 0x06002C9B RID: 11419 RVA: 0x00129FB0 File Offset: 0x001281B0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void assignLeftOverItems(bool[] _usedIds, List<ItemClass> _unassignedItems)
	{
		foreach (KeyValuePair<string, int> keyValuePair in ItemClass.fixedItemIds)
		{
			if (ItemClass.nameToItem.ContainsKey(keyValuePair.Key))
			{
				ItemClass itemClass = ItemClass.nameToItem[keyValuePair.Key];
				if (_unassignedItems.Contains(itemClass))
				{
					_unassignedItems.Remove(itemClass);
					ItemClass.assignId(itemClass, keyValuePair.Value + Block.ItemsStartHere, _usedIds);
				}
			}
		}
		int num = Block.ItemsStartHere;
		foreach (ItemClass itemClass2 in _unassignedItems)
		{
			if (!itemClass2.IsBlock())
			{
				while (_usedIds[++num])
				{
				}
				ItemClass.assignId(itemClass2, num, _usedIds);
			}
		}
		Log.Out("ItemClass assignLeftOverItems {0} of {1}", new object[]
		{
			num,
			ItemClass.MAX_ITEMS
		});
	}

	// Token: 0x06002C9C RID: 11420 RVA: 0x0012A0C4 File Offset: 0x001282C4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void assignIdsFromMapping()
	{
		Log.Out("ItemIDs from Mapping");
		List<ItemClass> list = new List<ItemClass>();
		bool[] usedIds = new bool[ItemClass.MAX_ITEMS];
		foreach (KeyValuePair<string, ItemClass> keyValuePair in ItemClass.nameToItem)
		{
			if (!keyValuePair.Value.IsBlock())
			{
				int idForName = ItemClass.nameIdMapping.GetIdForName(keyValuePair.Key);
				if (idForName >= 0)
				{
					ItemClass.assignId(keyValuePair.Value, idForName, usedIds);
				}
				else
				{
					list.Add(keyValuePair.Value);
				}
			}
		}
		ItemClass.assignLeftOverItems(usedIds, list);
	}

	// Token: 0x06002C9D RID: 11421 RVA: 0x0012A178 File Offset: 0x00128378
	[PublicizedFrom(EAccessModifier.Private)]
	public static void createFullMappingForClients()
	{
		NameIdMapping nameIdMapping = new NameIdMapping(null, ItemClass.MAX_ITEMS);
		foreach (KeyValuePair<string, ItemClass> keyValuePair in ItemClass.nameToItem)
		{
			nameIdMapping.AddMapping(keyValuePair.Value.Id, keyValuePair.Key, false);
		}
		ItemClass.fullMappingDataForClients = nameIdMapping.SaveToArray();
	}

	// Token: 0x06002C9E RID: 11422 RVA: 0x0012A1F4 File Offset: 0x001283F4
	public static void AssignIds()
	{
		if (ItemClass.nameIdMapping != null)
		{
			Log.Out("Item IDs with mapping");
			ItemClass.assignIdsFromMapping();
		}
		else
		{
			Log.Out("Item IDs withOUT mapping");
			ItemClass.assignIdsLinear();
		}
		ItemClass.createFullMappingForClients();
	}

	// Token: 0x1700046D RID: 1133
	// (get) Token: 0x06002C9F RID: 11423 RVA: 0x0012A222 File Offset: 0x00128422
	public static ItemClass MissingItem
	{
		get
		{
			return ItemClass.GetItemClass("missingItem", false);
		}
	}

	// Token: 0x0400227A RID: 8826
	public static readonly FastTags<TagGroup.Global> EatDistractionTag = FastTags<TagGroup.Global>.GetTag("eat");

	// Token: 0x0400227B RID: 8827
	public static readonly FastTags<TagGroup.Global> RequiresContactDistractionTag = FastTags<TagGroup.Global>.GetTag("requires_contact");

	// Token: 0x0400227C RID: 8828
	public static string PropSoundIdle = "SoundIdle";

	// Token: 0x0400227D RID: 8829
	public static string PropSoundDestroy = "SoundDestroy";

	// Token: 0x0400227E RID: 8830
	public static string PropSoundJammed = "SoundJammed";

	// Token: 0x0400227F RID: 8831
	public static string PropSoundHolster = "SoundHolster";

	// Token: 0x04002280 RID: 8832
	public static string PropSoundUnholster = "SoundUnholster";

	// Token: 0x04002281 RID: 8833
	public static string PropSoundStick = "SoundStick";

	// Token: 0x04002282 RID: 8834
	public static string PropSoundTick = "SoundTick";

	// Token: 0x04002283 RID: 8835
	public static string PropSoundPickup = "SoundPickup";

	// Token: 0x04002284 RID: 8836
	public static string PropSoundPlace = "SoundPlace";

	// Token: 0x04002285 RID: 8837
	public static string PropHasReloadAnim = "HasReloadAnim";

	// Token: 0x04002286 RID: 8838
	public static string PropFuelValue = "FuelValue";

	// Token: 0x04002287 RID: 8839
	public static string PropWeight = "Weight";

	// Token: 0x04002288 RID: 8840
	public static string PropMoldTarget = "MoldTarget";

	// Token: 0x04002289 RID: 8841
	public static string PropSmell = "Smell";

	// Token: 0x0400228A RID: 8842
	public static string PropLightSource = "LightSource";

	// Token: 0x0400228B RID: 8843
	public static string PropLightValue = "LightValue";

	// Token: 0x0400228C RID: 8844
	public static string PropMatEmission = "MatEmission";

	// Token: 0x0400228D RID: 8845
	public static string PropActivateObject = "ActivateObject";

	// Token: 0x0400228E RID: 8846
	public static string PropThrowableDecoy = "ThrowableDecoy";

	// Token: 0x0400228F RID: 8847
	public static string PropGroupName = "Group";

	// Token: 0x04002290 RID: 8848
	public static string PropCritChance = "CritChance";

	// Token: 0x04002291 RID: 8849
	public static string PropCustomIcon = "CustomIcon";

	// Token: 0x04002292 RID: 8850
	public static string PropCustomIconTint = "CustomIconTint";

	// Token: 0x04002293 RID: 8851
	public static string PropPartType = "PartType";

	// Token: 0x04002294 RID: 8852
	public static string PropImageEffectOnActive = "ImageEffectOnActive";

	// Token: 0x04002295 RID: 8853
	public static string PropPlaySoundOnActive = "PlaySoundOnActive";

	// Token: 0x04002296 RID: 8854
	public static string PropActive = "Active";

	// Token: 0x04002297 RID: 8855
	public static string PropAlwaysActive = "AlwaysActive";

	// Token: 0x04002298 RID: 8856
	public static string PropHoldingItemHidden = "HoldingItemHidden";

	// Token: 0x04002299 RID: 8857
	public static string PropVehicleSlotType = "VehicleSlotType";

	// Token: 0x0400229A RID: 8858
	public static string PropGetQualityFromWeapon = "GetQualityFromWeapon";

	// Token: 0x0400229B RID: 8859
	public static string PropAttributes = "Attributes";

	// Token: 0x0400229C RID: 8860
	public static string PropCraftExpValue = "CraftingIngredientExp";

	// Token: 0x0400229D RID: 8861
	public static string PropCraftTimeValue = "CraftingIngredientTime";

	// Token: 0x0400229E RID: 8862
	public static string PropLootExpValue = "LootExpValue";

	// Token: 0x0400229F RID: 8863
	public static string PropEconomicValue = "EconomicValue";

	// Token: 0x040022A0 RID: 8864
	public static string PropEconomicSellScale = "EconomicSellScale";

	// Token: 0x040022A1 RID: 8865
	public static string PropEconomicBundleSize = "EconomicBundleSize";

	// Token: 0x040022A2 RID: 8866
	public static string PropSellableToTrader = "SellableToTrader";

	// Token: 0x040022A3 RID: 8867
	public static string PropCraftingSkillExp = "CraftingSkillExp";

	// Token: 0x040022A4 RID: 8868
	public static string PropActionSkillExp = "ActionSkillExp";

	// Token: 0x040022A5 RID: 8869
	public static string PropInsulation = "Insulation";

	// Token: 0x040022A6 RID: 8870
	public static string PropWaterproof = "Waterproof";

	// Token: 0x040022A7 RID: 8871
	public static string PropEncumbrance = "Encumbrance";

	// Token: 0x040022A8 RID: 8872
	public static string PropDescriptionKey = "DescriptionKey";

	// Token: 0x040022A9 RID: 8873
	public static string PropResourceUnit = "ResourceUnit";

	// Token: 0x040022AA RID: 8874
	public static string PropMeltTimePerUnit = "MeltTimePerUnit";

	// Token: 0x040022AB RID: 8875
	public static string PropActionSkillGroup = "ActionSkillGroup";

	// Token: 0x040022AC RID: 8876
	public static string PropCraftingSkillGroup = "CraftingSkillGroup";

	// Token: 0x040022AD RID: 8877
	public static string PropCrosshairOnAim = "CrosshairOnAim";

	// Token: 0x040022AE RID: 8878
	public static string PropCrosshairUpAfterShot = "CrosshairUpAfterShot";

	// Token: 0x040022AF RID: 8879
	public static string PropUsableUnderwater = "UsableUnderwater";

	// Token: 0x040022B0 RID: 8880
	public static string PropRepairExpMultiplier = "RepairExpMultiplier";

	// Token: 0x040022B1 RID: 8881
	public static string PropTags = "Tags";

	// Token: 0x040022B2 RID: 8882
	public static string PropShowQuality = "ShowQuality";

	// Token: 0x040022B3 RID: 8883
	public static string PropSoundSightIn = "Sound_Sight_In";

	// Token: 0x040022B4 RID: 8884
	public static string PropSoundSightOut = "Sound_Sight_Out";

	// Token: 0x040022B5 RID: 8885
	public static string PropIgnoreKeystoneSound = "IgnoreKeystoneSound";

	// Token: 0x040022B6 RID: 8886
	public static string PropCreativeMode = "CreativeMode";

	// Token: 0x040022B7 RID: 8887
	public static string PropCreativeSort1 = "SortOrder1";

	// Token: 0x040022B8 RID: 8888
	public static string PropCreativeSort2 = "SortOrder2";

	// Token: 0x040022B9 RID: 8889
	public static string PropDistractionTags = "DistractionTags";

	// Token: 0x040022BA RID: 8890
	public static string PropIsSticky = "IsSticky";

	// Token: 0x040022BB RID: 8891
	public static string PropDisplayType = "DisplayType";

	// Token: 0x040022BC RID: 8892
	public static string PropItemTypeIcon = "ItemTypeIcon";

	// Token: 0x040022BD RID: 8893
	public static string PropAltItemTypeIcon = "AltItemTypeIcon";

	// Token: 0x040022BE RID: 8894
	public static string PropAltItemTypeIconColor = "AltItemTypeIconColor";

	// Token: 0x040022BF RID: 8895
	public static string PropUnlockedBy = "UnlockedBy";

	// Token: 0x040022C0 RID: 8896
	public static string PropUnlocks = "Unlocks";

	// Token: 0x040022C1 RID: 8897
	public static string PropNoScrapping = "NoScrapping";

	// Token: 0x040022C2 RID: 8898
	public static string PropScrapTimeOverride = "ScrapTimeOverride";

	// Token: 0x040022C3 RID: 8899
	public static string PropNavObject = "NavObject";

	// Token: 0x040022C4 RID: 8900
	public static string PropQuestItem = "IsQuestItem";

	// Token: 0x040022C5 RID: 8901
	public static string PropTrackerIndexName = "TrackerIndexName";

	// Token: 0x040022C6 RID: 8902
	public static string PropTrackerNavObject = "TrackerNavObject";

	// Token: 0x040022C7 RID: 8903
	public static string PropTraderStageTemplate = "TraderStageTemplate";

	// Token: 0x040022C8 RID: 8904
	public static int MAX_ITEMS = Block.MAX_BLOCKS + 16384;

	// Token: 0x040022C9 RID: 8905
	public static NameIdMapping nameIdMapping;

	// Token: 0x040022CA RID: 8906
	public static byte[] fullMappingDataForClients;

	// Token: 0x040022CB RID: 8907
	public static ItemClass[] list;

	// Token: 0x040022CC RID: 8908
	public static string[] itemActionNames;

	// Token: 0x040022CD RID: 8909
	public const int cMaxActionNames = 5;

	// Token: 0x040022CE RID: 8910
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Dictionary<string, ItemClass> nameToItem = new Dictionary<string, ItemClass>();

	// Token: 0x040022CF RID: 8911
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Dictionary<string, ItemClass> nameToItemCaseInsensitive = new CaseInsensitiveStringDictionary<ItemClass>();

	// Token: 0x040022D0 RID: 8912
	[PublicizedFrom(EAccessModifier.Protected)]
	public static List<string> itemNames = new List<string>();

	// Token: 0x040022D1 RID: 8913
	public static readonly ReadOnlyCollection<string> ItemNames = new ReadOnlyCollection<string>(ItemClass.itemNames);

	// Token: 0x040022D2 RID: 8914
	public DynamicProperties Properties;

	// Token: 0x040022D3 RID: 8915
	[PublicizedFrom(EAccessModifier.Private)]
	public string localizedName;

	// Token: 0x040022D4 RID: 8916
	public AIDirectorData.Smell Smell;

	// Token: 0x040022D5 RID: 8917
	public string MeshFile;

	// Token: 0x040022D6 RID: 8918
	public string DropMeshFile;

	// Token: 0x040022D7 RID: 8919
	public string HandMeshFile;

	// Token: 0x040022D8 RID: 8920
	public string StickyMaterial;

	// Token: 0x040022D9 RID: 8921
	public float StickyOffset = 0.15f;

	// Token: 0x040022DA RID: 8922
	public int StickyColliderUp = -1;

	// Token: 0x040022DB RID: 8923
	public float StickyColliderRadius = 0.2f;

	// Token: 0x040022DC RID: 8924
	public float StickyColliderLength = -1f;

	// Token: 0x040022DD RID: 8925
	public bool IsSticky;

	// Token: 0x040022DE RID: 8926
	public const int cActionUpdateCount = 3;

	// Token: 0x040022DF RID: 8927
	public ItemAction[] Actions;

	// Token: 0x040022E0 RID: 8928
	public MaterialBlock MadeOfMaterial;

	// Token: 0x040022E1 RID: 8929
	public DataItem<int> HoldType = new DataItem<int>(0);

	// Token: 0x040022E2 RID: 8930
	public DataItem<int> Stacknumber = new DataItem<int>(500);

	// Token: 0x040022E3 RID: 8931
	public DataItem<int> MaxUseTimes = new DataItem<int>(0);

	// Token: 0x040022E4 RID: 8932
	public DataItem<bool> MaxUseTimesBreaksAfter = new DataItem<bool>(false);

	// Token: 0x040022E5 RID: 8933
	public ItemClass MoldTarget;

	// Token: 0x040022E6 RID: 8934
	public DataItem<string> LightSource;

	// Token: 0x040022E7 RID: 8935
	public DataItem<string> ActivateObject;

	// Token: 0x040022E8 RID: 8936
	public DataItem<bool> ThrowableDecoy;

	// Token: 0x040022E9 RID: 8937
	public ItemData.DataItemArrayRepairTools RepairTools;

	// Token: 0x040022EA RID: 8938
	public DataItem<int> RepairAmount;

	// Token: 0x040022EB RID: 8939
	public DataItem<float> RepairTime;

	// Token: 0x040022EC RID: 8940
	public DataItem<float> CritChance;

	// Token: 0x040022ED RID: 8941
	public string[] Groups = new string[]
	{
		"Decor/Miscellaneous"
	};

	// Token: 0x040022EE RID: 8942
	public DataItem<string> CustomIcon;

	// Token: 0x040022EF RID: 8943
	public Color CustomIconTint;

	// Token: 0x040022F0 RID: 8944
	public DataItem<bool> UserHidden = new DataItem<bool>(false);

	// Token: 0x040022F1 RID: 8945
	public string VehicleSlotType;

	// Token: 0x040022F2 RID: 8946
	public bool GetQualityFromWeapon;

	// Token: 0x040022F3 RID: 8947
	public string DescriptionKey;

	// Token: 0x040022F4 RID: 8948
	public bool IsResourceUnit;

	// Token: 0x040022F5 RID: 8949
	public float MeltTimePerUnit;

	// Token: 0x040022F6 RID: 8950
	public string ActionSkillGroup = "";

	// Token: 0x040022F7 RID: 8951
	public string CraftingSkillGroup = "";

	// Token: 0x040022F8 RID: 8952
	public bool UsableUnderwater = true;

	// Token: 0x040022F9 RID: 8953
	public float lightValue;

	// Token: 0x040022FA RID: 8954
	public string soundSightIn = "";

	// Token: 0x040022FB RID: 8955
	public string soundSightOut = "";

	// Token: 0x040022FC RID: 8956
	public bool ignoreKeystoneSound;

	// Token: 0x040022FD RID: 8957
	public bool HoldingItemHidden;

	// Token: 0x040022FE RID: 8958
	public List<int> PartParentId;

	// Token: 0x040022FF RID: 8959
	public PreviewData Preview;

	// Token: 0x04002300 RID: 8960
	[PublicizedFrom(EAccessModifier.Protected)]
	public GameObject renderGameObject;

	// Token: 0x04002301 RID: 8961
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bCanHold;

	// Token: 0x04002302 RID: 8962
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bCanDrop;

	// Token: 0x04002303 RID: 8963
	public bool HasSubItems;

	// Token: 0x04002304 RID: 8964
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] attachments;

	// Token: 0x04002305 RID: 8965
	public bool bCraftingTool;

	// Token: 0x04002306 RID: 8966
	public float CraftComponentExp = 3f;

	// Token: 0x04002307 RID: 8967
	public float CraftComponentTime = 1f;

	// Token: 0x04002308 RID: 8968
	public float LootExp = 1f;

	// Token: 0x04002309 RID: 8969
	public float EconomicValue;

	// Token: 0x0400230A RID: 8970
	public float EconomicSellScale = 1f;

	// Token: 0x0400230B RID: 8971
	public int EconomicBundleSize = 1;

	// Token: 0x0400230C RID: 8972
	public bool SellableToTrader = true;

	// Token: 0x0400230D RID: 8973
	public string TraderStageTemplate;

	// Token: 0x0400230E RID: 8974
	public int CraftingSkillExp = 10;

	// Token: 0x0400230F RID: 8975
	public int ActionSkillExp = 10;

	// Token: 0x04002310 RID: 8976
	public float RepairExpMultiplier = 10f;

	// Token: 0x04002311 RID: 8977
	public float Insulation;

	// Token: 0x04002312 RID: 8978
	public float WaterProof;

	// Token: 0x04002313 RID: 8979
	public float Encumbrance;

	// Token: 0x04002314 RID: 8980
	public string SoundUnholster = "generic_unholster";

	// Token: 0x04002315 RID: 8981
	public string SoundHolster = "generic_holster";

	// Token: 0x04002316 RID: 8982
	public string SoundStick;

	// Token: 0x04002317 RID: 8983
	public string SoundTick;

	// Token: 0x04002318 RID: 8984
	public float SoundTickDelay = 1f;

	// Token: 0x04002319 RID: 8985
	public string SoundPickup = "craft_take_item";

	// Token: 0x0400231A RID: 8986
	public string SoundPlace = "craft_place_item";

	// Token: 0x0400231B RID: 8987
	public bool HasReloadAnim = true;

	// Token: 0x0400231C RID: 8988
	public bool bShowCrosshairOnAiming;

	// Token: 0x0400231D RID: 8989
	public bool bCrosshairUpAfterShot;

	// Token: 0x0400231E RID: 8990
	public EnumCreativeMode CreativeMode;

	// Token: 0x0400231F RID: 8991
	public string SortOrder;

	// Token: 0x04002320 RID: 8992
	public FastTags<TagGroup.Global> ItemTags;

	// Token: 0x04002321 RID: 8993
	public MinEffectController Effects;

	// Token: 0x04002322 RID: 8994
	public FastTags<TagGroup.Global> DistractionTags;

	// Token: 0x04002323 RID: 8995
	public SDCSUtils.SlotData SDCSData;

	// Token: 0x04002324 RID: 8996
	public string DisplayType;

	// Token: 0x04002325 RID: 8997
	public string ItemTypeIcon = "";

	// Token: 0x04002326 RID: 8998
	public string AltItemTypeIcon;

	// Token: 0x04002327 RID: 8999
	public Color AltItemTypeIconColor;

	// Token: 0x04002328 RID: 9000
	public bool ShowQualityBar;

	// Token: 0x04002329 RID: 9001
	public bool NoScrapping;

	// Token: 0x0400232A RID: 9002
	public float ScrapTimeOverride;

	// Token: 0x0400232B RID: 9003
	public string Unlocks = "";

	// Token: 0x0400232C RID: 9004
	public string NavObject = "";

	// Token: 0x0400232D RID: 9005
	public bool IsQuestItem;

	// Token: 0x0400232E RID: 9006
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> stopBleed = FastTags<TagGroup.Global>.Parse("stopsBleeding");

	// Token: 0x0400232F RID: 9007
	public string TrackerIndexName;

	// Token: 0x04002330 RID: 9008
	public string TrackerNavObject;

	// Token: 0x04002331 RID: 9009
	[PublicizedFrom(EAccessModifier.Private)]
	public RecipeUnlockData[] unlockedBy;

	// Token: 0x04002332 RID: 9010
	[PublicizedFrom(EAccessModifier.Private)]
	public static string[] ActionProfilerNames = new string[]
	{
		"action0",
		"action1",
		"action2"
	};

	// Token: 0x04002333 RID: 9011
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<string, int> fixedItemIds = new Dictionary<string, int>();

	// Token: 0x0200055E RID: 1374
	public enum EnumCrosshairType
	{
		// Token: 0x04002335 RID: 9013
		None,
		// Token: 0x04002336 RID: 9014
		Plus,
		// Token: 0x04002337 RID: 9015
		Crosshair,
		// Token: 0x04002338 RID: 9016
		CrosshairOnAiming,
		// Token: 0x04002339 RID: 9017
		Damage,
		// Token: 0x0400233A RID: 9018
		Upgrade,
		// Token: 0x0400233B RID: 9019
		Repair,
		// Token: 0x0400233C RID: 9020
		PowerSource,
		// Token: 0x0400233D RID: 9021
		Heal,
		// Token: 0x0400233E RID: 9022
		PowerItem
	}

	// Token: 0x0200055F RID: 1375
	// (Invoke) Token: 0x06002CA2 RID: 11426
	public delegate bool FilterItem(ItemClass _class, Block _block);
}
