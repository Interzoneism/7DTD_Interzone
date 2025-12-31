using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Audio;
using UnityEngine;

// Token: 0x020004A2 RID: 1186
public class Equipment
{
	// Token: 0x14000029 RID: 41
	// (add) Token: 0x060026B8 RID: 9912 RVA: 0x000FB718 File Offset: 0x000F9918
	// (remove) Token: 0x060026B9 RID: 9913 RVA: 0x000FB750 File Offset: 0x000F9950
	public event Action OnChanged;

	// Token: 0x1700040C RID: 1036
	// (get) Token: 0x060026BA RID: 9914 RVA: 0x000FB785 File Offset: 0x000F9985
	// (set) Token: 0x060026BB RID: 9915 RVA: 0x000FB78D File Offset: 0x000F998D
	public ItemClass[] CosmeticSlots
	{
		get
		{
			return this.m_cosmeticSlots;
		}
		set
		{
			this.m_cosmeticSlots = value;
		}
	}

	// Token: 0x1400002A RID: 42
	// (add) Token: 0x060026BC RID: 9916 RVA: 0x000FB798 File Offset: 0x000F9998
	// (remove) Token: 0x060026BD RID: 9917 RVA: 0x000FB7D0 File Offset: 0x000F99D0
	public event Equipment_CosmeticUnlocked CosmeticUnlocked;

	// Token: 0x060026BE RID: 9918 RVA: 0x000FB808 File Offset: 0x000F9A08
	public Equipment()
	{
		int num = 8;
		this.m_slots = new ItemValue[num];
		this.CosmeticSlots = new ItemClass[num];
		this.preferredItemSlots = new int[num];
	}

	// Token: 0x060026BF RID: 9919 RVA: 0x000FB869 File Offset: 0x000F9A69
	public Equipment(EntityAlive _entity) : this()
	{
		this.m_entity = _entity;
	}

	// Token: 0x060026C0 RID: 9920 RVA: 0x000FB878 File Offset: 0x000F9A78
	public ItemClass GetCosmeticSlot(int index, bool useTemporary)
	{
		if (useTemporary && this.tempCosmeticSlotIndex == index)
		{
			return this.tempCosmeticSlot;
		}
		return this.CosmeticSlots[index];
	}

	// Token: 0x060026C1 RID: 9921 RVA: 0x000FB898 File Offset: 0x000F9A98
	public void SetCosmeticSlot(ItemClassArmor itemClass)
	{
		if (itemClass.EquipSlot < EquipmentSlots.BiomeBadge)
		{
			if (!this.HasCosmeticUnlocked(itemClass).Item1)
			{
				return;
			}
			int equipSlot = (int)itemClass.EquipSlot;
			this.CosmeticSlots[equipSlot] = itemClass;
			if (this.m_entity && !this.m_entity.isEntityRemote)
			{
				this.m_entity.bPlayerStatsChanged = true;
			}
		}
	}

	// Token: 0x060026C2 RID: 9922 RVA: 0x000FB8F4 File Offset: 0x000F9AF4
	public void SetCosmeticSlot(int slotID, int id)
	{
		if (id == 0)
		{
			this.CosmeticSlots[slotID] = null;
			if (this.m_entity && !this.m_entity.isEntityRemote)
			{
				this.m_entity.bPlayerStatsChanged = true;
				return;
			}
		}
		else
		{
			ItemClass itemClass = ItemClass.GetItemClass(Equipment.CosmeticMappingIDString[id], false);
			ItemClassArmor itemClassArmor = itemClass as ItemClassArmor;
			if (itemClassArmor != null)
			{
				if (itemClassArmor.EquipSlot < EquipmentSlots.BiomeBadge)
				{
					this.CosmeticSlots[(int)itemClassArmor.EquipSlot] = itemClass;
					if (this.m_entity && !this.m_entity.isEntityRemote)
					{
						this.m_entity.bPlayerStatsChanged = true;
						return;
					}
				}
			}
			else
			{
				this.CosmeticSlots[slotID] = itemClass;
				if (this.m_entity && !this.m_entity.isEntityRemote)
				{
					this.m_entity.bPlayerStatsChanged = true;
				}
			}
		}
	}

	// Token: 0x060026C3 RID: 9923 RVA: 0x000FB9C4 File Offset: 0x000F9BC4
	public int[] GetCosmeticIDs()
	{
		int num = 8;
		int[] array = new int[num];
		for (int i = 0; i < num; i++)
		{
			if (this.m_cosmeticSlots[i] == null)
			{
				array[i] = 0;
			}
			else
			{
				array[i] = Equipment.CosmeticMappingStringID[this.m_cosmeticSlots[i].GetItemName()];
			}
		}
		return array;
	}

	// Token: 0x060026C4 RID: 9924 RVA: 0x000FBA14 File Offset: 0x000F9C14
	public void ClearCosmeticSlots()
	{
		for (int i = 0; i < this.m_cosmeticSlots.Length; i++)
		{
			this.m_cosmeticSlots[i] = null;
		}
		if (this.m_entity && !this.m_entity.isEntityRemote)
		{
			this.m_entity.bPlayerStatsChanged = true;
		}
	}

	// Token: 0x060026C5 RID: 9925 RVA: 0x000FBA64 File Offset: 0x000F9C64
	public void UnlockCosmeticItem(ItemClass itemClass)
	{
		if (itemClass == null)
		{
			return;
		}
		string itemName = itemClass.GetItemName();
		if (Equipment.CosmeticMappingStringID.ContainsKey(itemName))
		{
			int item = Equipment.CosmeticMappingStringID[itemName];
			if (!this.m_unlockedCosmetics.Contains(item))
			{
				this.m_unlockedCosmetics.Add(item);
				if (this.CosmeticUnlocked != null)
				{
					this.CosmeticUnlocked(itemName);
				}
			}
		}
	}

	// Token: 0x060026C6 RID: 9926 RVA: 0x000FBAC4 File Offset: 0x000F9CC4
	public void ModifyValue(ItemValue _originalItemValue, PassiveEffects _passiveEffect, ref float _base_val, ref float _perc_val, FastTags<TagGroup.Global> tags, bool _useDurability = false)
	{
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue itemValue = this.m_slots[i];
			if (itemValue != null && !itemValue.Equals(_originalItemValue) && itemValue.ItemClass != null)
			{
				itemValue.ModifyValue(this.m_entity, _originalItemValue, _passiveEffect, ref _base_val, ref _perc_val, tags, true, _useDurability);
			}
		}
	}

	// Token: 0x060026C7 RID: 9927 RVA: 0x000FBB18 File Offset: 0x000F9D18
	public void GetModifiedValueData(List<EffectManager.ModifierValuesAndSources> _modValueSources, EffectManager.ModifierValuesAndSources.ValueSourceType _sourceType, ItemValue _originalItemValue, PassiveEffects _passiveEffect, ref float _base_val, ref float _perc_val, FastTags<TagGroup.Global> tags)
	{
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue itemValue = this.m_slots[i];
			if (itemValue != null && !itemValue.Equals(_originalItemValue) && itemValue.ItemClass != null)
			{
				itemValue.GetModifiedValueData(_modValueSources, _sourceType, this.m_entity, _originalItemValue, _passiveEffect, ref _base_val, ref _perc_val, tags);
			}
		}
	}

	// Token: 0x060026C8 RID: 9928 RVA: 0x000FBB6C File Offset: 0x000F9D6C
	[return: TupleElementNames(new string[]
	{
		"isUnlocked",
		"set"
	})]
	public ValueTuple<bool, EntitlementSetEnum> HasCosmeticUnlocked(ItemClass itemClass)
	{
		if (itemClass == null)
		{
			return new ValueTuple<bool, EntitlementSetEnum>(false, EntitlementSetEnum.None);
		}
		string itemName = itemClass.GetItemName();
		if (!Equipment.CosmeticMappingStringID.ContainsKey(itemName))
		{
			return new ValueTuple<bool, EntitlementSetEnum>(false, EntitlementSetEnum.None);
		}
		EntitlementSetEnum entitlementSetEnum = EntitlementSetEnum.None;
		if (itemClass.SDCSData != null)
		{
			entitlementSetEnum = EntitlementManager.Instance.GetSetForAsset(itemClass.SDCSData.PrefabName);
			if (entitlementSetEnum != EntitlementSetEnum.None && EntitlementManager.Instance.HasEntitlement(entitlementSetEnum))
			{
				return new ValueTuple<bool, EntitlementSetEnum>(true, entitlementSetEnum);
			}
		}
		return new ValueTuple<bool, EntitlementSetEnum>(this.m_unlockedCosmetics.Contains(Equipment.CosmeticMappingStringID[itemClass.GetItemName()]), entitlementSetEnum);
	}

	// Token: 0x060026C9 RID: 9929 RVA: 0x000FBBFC File Offset: 0x000F9DFC
	public void FireEvent(MinEventTypes _eventType, MinEventParams _params)
	{
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue itemValue = this.m_slots[i];
			if (itemValue != null && itemValue.ItemClass != null)
			{
				itemValue.FireEvent(_eventType, _params);
			}
		}
	}

	// Token: 0x060026CA RID: 9930 RVA: 0x000FBC38 File Offset: 0x000F9E38
	public void DropItems()
	{
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue itemValue = this.m_slots[i];
			if (itemValue != null)
			{
				this.DropItemOnGround(itemValue);
				this.SetSlotItem(i, null, true);
			}
		}
		this.updateInsulation();
	}

	// Token: 0x060026CB RID: 9931 RVA: 0x000FBC7C File Offset: 0x000F9E7C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateInsulation()
	{
		this.waterProof = 0f;
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue itemValue = this.m_slots[i];
			if (itemValue != null)
			{
				this.waterProof += itemValue.ItemClass.WaterProof;
			}
		}
	}

	// Token: 0x060026CC RID: 9932 RVA: 0x000FBCCC File Offset: 0x000F9ECC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void DropItemOnGround(ItemValue _itemValue)
	{
		this.m_entity.world.GetGameManager().ItemDropServer(new ItemStack(_itemValue, 1), this.m_entity.GetPosition(), new Vector3(0.5f, 0f, 0.5f), this.m_entity.belongsPlayerId, 60f, false);
	}

	// Token: 0x060026CD RID: 9933 RVA: 0x000FBD25 File Offset: 0x000F9F25
	public float GetTotalInsulation()
	{
		return this.insulation;
	}

	// Token: 0x060026CE RID: 9934 RVA: 0x000FBD2D File Offset: 0x000F9F2D
	public float GetTotalWaterproof()
	{
		return this.waterProof;
	}

	// Token: 0x060026CF RID: 9935 RVA: 0x000FBD35 File Offset: 0x000F9F35
	public int GetSlotCount()
	{
		return this.m_slots.Length;
	}

	// Token: 0x060026D0 RID: 9936 RVA: 0x000FBD3F File Offset: 0x000F9F3F
	public ItemValue[] GetItems()
	{
		return this.m_slots;
	}

	// Token: 0x060026D1 RID: 9937 RVA: 0x000FBD47 File Offset: 0x000F9F47
	public ItemValue GetSlotItem(int index)
	{
		return this.m_slots[index];
	}

	// Token: 0x060026D2 RID: 9938 RVA: 0x000FBD54 File Offset: 0x000F9F54
	public ItemValue GetSlotItemOrNone(int index)
	{
		ItemValue itemValue = this.m_slots[index];
		if (itemValue == null)
		{
			return ItemValue.None;
		}
		return itemValue;
	}

	// Token: 0x060026D3 RID: 9939 RVA: 0x000FBD74 File Offset: 0x000F9F74
	public bool HasAnyItems(int ignoreIndex = -1)
	{
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			if (i < ignoreIndex && this.m_slots[i] != null)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060026D4 RID: 9940 RVA: 0x000FBDA5 File Offset: 0x000F9FA5
	public bool IsNaked()
	{
		return !this.HasAnyItems(4);
	}

	// Token: 0x060026D5 RID: 9941 RVA: 0x000FBDB1 File Offset: 0x000F9FB1
	public void SetTempCosmeticSlot(int index, ItemClass itemClass)
	{
		this.tempCosmeticSlotIndex = index;
		this.tempCosmeticSlot = itemClass;
	}

	// Token: 0x060026D6 RID: 9942 RVA: 0x000FBDC1 File Offset: 0x000F9FC1
	public void ClearTempCosmeticSlot()
	{
		this.tempCosmeticSlotIndex = -1;
		this.tempCosmeticSlot = null;
	}

	// Token: 0x060026D7 RID: 9943 RVA: 0x000FBDD4 File Offset: 0x000F9FD4
	public void ApplyTempCosmeticSlot()
	{
		if (this.tempCosmeticSlotIndex != -1)
		{
			this.m_cosmeticSlots[this.tempCosmeticSlotIndex] = this.tempCosmeticSlot;
			if (this.m_entity && !this.m_entity.isEntityRemote)
			{
				this.m_entity.bPlayerStatsChanged = true;
			}
		}
	}

	// Token: 0x060026D8 RID: 9944 RVA: 0x000FBE24 File Offset: 0x000FA024
	public void CalcDamage(ref int entityDamageTaken, ref int armorDamageTaken, FastTags<TagGroup.Global> damageTypeTag, EntityAlive attacker, ItemValue attackingItem)
	{
		armorDamageTaken = entityDamageTaken;
		if (damageTypeTag.Test_AnySet(Equipment.physicalDamageTypes))
		{
			if (entityDamageTaken > 0)
			{
				float num = this.GetTotalPhysicalArmorRating(attacker, attackingItem) / 100f;
				armorDamageTaken = Utils.FastMax((num > 0f) ? 1 : 0, Utils.FastRoundToInt((float)entityDamageTaken * num));
				entityDamageTaken -= armorDamageTaken;
				return;
			}
		}
		else
		{
			entityDamageTaken = Utils.FastRoundToInt(Utils.FastMax(0f, (float)entityDamageTaken * (1f - EffectManager.GetValue(PassiveEffects.ElementalDamageResist, null, 0f, this.m_entity, null, damageTypeTag, true, true, true, true, true, 1, true, false) / 100f)));
			armorDamageTaken = Utils.FastRoundToInt((float)Utils.FastMax(0, armorDamageTaken - entityDamageTaken));
		}
	}

	// Token: 0x060026D9 RID: 9945 RVA: 0x000FBED4 File Offset: 0x000FA0D4
	public float GetTotalPhysicalArmorRating(EntityAlive attacker, ItemValue attackingItem)
	{
		FastTags<TagGroup.Global> fastTags = Equipment.coreDamageResist;
		if (attackingItem != null)
		{
			fastTags |= attackingItem.ItemClassOrMissing.ItemTags;
		}
		float value = EffectManager.GetValue(PassiveEffects.PhysicalDamageResist, null, 0f, this.m_entity, null, fastTags, true, true, true, true, true, 1, true, true);
		return EffectManager.GetValue(PassiveEffects.TargetArmor, attackingItem, value, attacker, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
	}

	// Token: 0x060026DA RID: 9946 RVA: 0x000FBF3C File Offset: 0x000FA13C
	public List<ItemValue> GetArmor()
	{
		List<ItemValue> list = new List<ItemValue>();
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue itemValue = this.m_slots[i];
			if (itemValue != null)
			{
				float num = 0f;
				float num2 = 1f;
				itemValue.ModifyValue(this.m_entity, null, PassiveEffects.PhysicalDamageResist, ref num, ref num2, FastTags<TagGroup.Global>.all, true, false);
				if (num != 0f)
				{
					list.Add(itemValue);
				}
			}
		}
		return list;
	}

	// Token: 0x060026DB RID: 9947 RVA: 0x000FBFA8 File Offset: 0x000FA1A8
	public bool CheckBreakUseItems()
	{
		bool result = false;
		this.CurrentLowestDurability = 1f;
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue itemValue = this.m_slots[i];
			if (itemValue != null)
			{
				ItemClass forId = ItemClass.GetForId(itemValue.type);
				float percentUsesLeft = itemValue.PercentUsesLeft;
				if (percentUsesLeft < this.CurrentLowestDurability)
				{
					this.CurrentLowestDurability = percentUsesLeft;
				}
				if (forId != null && itemValue.MaxUseTimes > 0 && forId.MaxUseTimesBreaksAfter.Value && itemValue.UseTimes > (float)itemValue.MaxUseTimes)
				{
					this.SetSlotItem(i, null, true);
					if (this.m_entity != null && forId.Properties.Values.ContainsKey(ItemClass.PropSoundDestroy))
					{
						Manager.BroadcastPlay(this.m_entity, forId.Properties.Values[ItemClass.PropSoundDestroy], false);
					}
					result = true;
				}
			}
		}
		return result;
	}

	// Token: 0x060026DC RID: 9948 RVA: 0x000FC08C File Offset: 0x000FA28C
	public void SetSlotItem(int index, ItemValue value, bool isLocal = true)
	{
		if (value != null && value.IsEmpty())
		{
			value = null;
		}
		ItemValue itemValue = this.m_slots[index];
		if (value == null && itemValue == null)
		{
			return;
		}
		this.m_entity.IsEquipping = true;
		bool flag = false;
		if (itemValue != null && itemValue.Equals(value))
		{
			this.m_slots[index] = value;
			this.m_entity.MinEventContext.ItemValue = value;
			value.FireEvent(MinEventTypes.onSelfEquipStart, this.m_entity.MinEventContext);
		}
		else
		{
			flag = true;
			if (itemValue != null)
			{
				if (itemValue.ItemClass.HasTrigger(MinEventTypes.onSelfItemActivate) && itemValue.Activated != 0)
				{
					this.m_entity.MinEventContext.ItemValue = itemValue;
					itemValue.FireEvent(MinEventTypes.onSelfItemDeactivate, this.m_entity.MinEventContext);
					itemValue.Activated = 0;
				}
				for (int i = 0; i < itemValue.Modifications.Length; i++)
				{
					ItemValue itemValue2 = itemValue.Modifications[i];
					if (itemValue2 != null)
					{
						ItemClass itemClass = itemValue2.ItemClass;
						if (itemClass != null && itemClass.HasTrigger(MinEventTypes.onSelfItemActivate) && itemValue2.Activated != 0)
						{
							this.m_entity.MinEventContext.ItemValue = itemValue2;
							itemValue2.FireEvent(MinEventTypes.onSelfItemDeactivate, this.m_entity.MinEventContext);
							itemValue2.Activated = 0;
						}
					}
				}
				this.m_entity.MinEventContext.ItemValue = itemValue;
				itemValue.FireEvent(MinEventTypes.onSelfEquipStop, this.m_entity.MinEventContext);
			}
			this.preferredItemSlots[index] = ((value == null) ? 0 : value.type);
			this.m_slots[index] = value;
			this.slotsSetFlags |= 1 << index;
			this.slotsChangedFlags |= 1 << index;
		}
		if (flag)
		{
			if (this.m_entity && !this.m_entity.isEntityRemote)
			{
				this.m_entity.bPlayerStatsChanged = true;
			}
			this.ResetArmorGroups();
			if (this.OnChanged != null)
			{
				this.OnChanged();
			}
		}
		this.m_entity.IsEquipping = false;
	}

	// Token: 0x060026DD RID: 9949 RVA: 0x000FC26B File Offset: 0x000FA46B
	public void SetSlotItemRaw(int index, ItemValue _iv)
	{
		if (_iv != null && _iv.IsEmpty())
		{
			_iv = null;
		}
		this.m_slots[index] = _iv;
	}

	// Token: 0x060026DE RID: 9950 RVA: 0x000FC284 File Offset: 0x000FA484
	public void FireEventsForSlots(MinEventTypes _event, int _flags = -1)
	{
		this.m_entity.MinEventContext.Self = this.m_entity;
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			if ((_flags & 1 << i) > 0)
			{
				ItemValue itemValue = this.m_slots[i];
				if (itemValue != null && itemValue.ItemClass != null)
				{
					this.m_entity.MinEventContext.ItemValue = itemValue;
					itemValue.FireEvent(_event, this.m_entity.MinEventContext);
					for (int j = 0; j < itemValue.Modifications.Length; j++)
					{
						ItemValue itemValue2 = itemValue.Modifications[j];
						if (itemValue2 != null && itemValue2.ItemClass != null)
						{
							itemValue2.FireEvent(_event, this.m_entity.MinEventContext);
						}
					}
				}
			}
		}
	}

	// Token: 0x060026DF RID: 9951 RVA: 0x000FC338 File Offset: 0x000FA538
	public void FireEventsForSetSlots()
	{
		this.FireEventsForSlots(MinEventTypes.onSelfEquipStart, this.slotsSetFlags);
		this.slotsSetFlags = 0;
	}

	// Token: 0x060026E0 RID: 9952 RVA: 0x000FC350 File Offset: 0x000FA550
	public void FireEventsForChangedSlots()
	{
		if (this.slotsChangedFlags == 0)
		{
			return;
		}
		this.m_entity.IsEquipping = true;
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			if ((this.slotsChangedFlags & 1 << i) > 0)
			{
				ItemValue itemValue = this.m_slots[i];
				if (itemValue != null)
				{
					ItemClass itemClass = itemValue.ItemClass;
					if (itemClass != null)
					{
						this.m_entity.MinEventContext.Self = this.m_entity;
						this.m_entity.MinEventContext.ItemValue = itemValue;
						itemValue.FireEvent(MinEventTypes.onSelfEquipChanged, this.m_entity.MinEventContext);
						if (itemClass.HasTrigger(MinEventTypes.onSelfItemActivate))
						{
							if (itemValue.Activated == 0)
							{
								itemValue.FireEvent(MinEventTypes.onSelfItemDeactivate, this.m_entity.MinEventContext);
							}
							else
							{
								itemValue.FireEvent(MinEventTypes.onSelfItemActivate, this.m_entity.MinEventContext);
							}
						}
						for (int j = 0; j < itemValue.Modifications.Length; j++)
						{
							ItemValue itemValue2 = itemValue.Modifications[j];
							if (itemValue2 != null && itemValue2.ItemClass != null && itemValue2.ItemClass.HasTrigger(MinEventTypes.onSelfItemActivate))
							{
								this.m_entity.MinEventContext.ItemValue = itemValue2;
								if (itemValue2.Activated == 0)
								{
									itemValue2.FireEvent(MinEventTypes.onSelfItemDeactivate, this.m_entity.MinEventContext);
								}
								else
								{
									itemValue2.FireEvent(MinEventTypes.onSelfItemActivate, this.m_entity.MinEventContext);
								}
							}
						}
					}
				}
			}
		}
		this.slotsChangedFlags = 0;
		this.m_entity.IsEquipping = false;
	}

	// Token: 0x060026E1 RID: 9953 RVA: 0x000FC4C0 File Offset: 0x000FA6C0
	public void Update()
	{
		if (Time.time - this.lastUpdateTime >= 1f)
		{
			for (int i = 0; i < this.m_slots.Length; i++)
			{
				ItemValue itemValue = this.m_slots[i];
				if (itemValue != null)
				{
					this.m_entity.MinEventContext.ItemValue = itemValue;
					itemValue.FireEvent(MinEventTypes.onSelfEquipUpdate, this.m_entity.MinEventContext);
				}
			}
			this.lastUpdateTime = Time.time;
		}
	}

	// Token: 0x060026E2 RID: 9954 RVA: 0x000FC52E File Offset: 0x000FA72E
	public void SetPreferredItemSlot(int _slot, ItemValue _itemValue)
	{
		if (_slot >= 0 && _slot < this.preferredItemSlots.Length)
		{
			this.preferredItemSlots[_slot] = _itemValue.type;
		}
	}

	// Token: 0x060026E3 RID: 9955 RVA: 0x000FC550 File Offset: 0x000FA750
	public int PreferredItemSlot(ItemValue _itemValue)
	{
		for (int i = 0; i < this.preferredItemSlots.Length; i++)
		{
			if (_itemValue.type == this.preferredItemSlots[i])
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060026E4 RID: 9956 RVA: 0x000FC584 File Offset: 0x000FA784
	public int PreferredItemSlot(ItemStack _itemStack)
	{
		for (int i = 0; i < this.preferredItemSlots.Length; i++)
		{
			if (_itemStack.itemValue.type == this.preferredItemSlots[i])
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060026E5 RID: 9957 RVA: 0x000FC5BC File Offset: 0x000FA7BC
	public void Write(BinaryWriter writer)
	{
		writer.Write(3);
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue.Write(this.m_slots[i], writer);
		}
		for (int j = 0; j < this.m_cosmeticSlots.Length; j++)
		{
			if (this.m_cosmeticSlots[j] == null)
			{
				writer.Write(0);
			}
			else
			{
				writer.Write(Equipment.CosmeticMappingStringID[this.m_cosmeticSlots[j].GetItemName()]);
			}
		}
		writer.Write(this.m_unlockedCosmetics.Count);
		for (int k = 0; k < this.m_unlockedCosmetics.Count; k++)
		{
			writer.Write(this.m_unlockedCosmetics[k]);
		}
	}

	// Token: 0x060026E6 RID: 9958 RVA: 0x000FC670 File Offset: 0x000FA870
	public static Equipment Read(BinaryReader reader)
	{
		int num = (int)reader.ReadByte();
		Equipment equipment = new Equipment();
		int num2 = equipment.m_slots.Length;
		if (num <= 2)
		{
			num2 = 5;
		}
		for (int i = 0; i < num2; i++)
		{
			equipment.m_slots[i] = ItemValue.ReadOrNull(reader);
		}
		if (num >= 2)
		{
			for (int j = 0; j < num2; j++)
			{
				int num3 = reader.ReadInt32();
				if (num3 == 0)
				{
					equipment.m_cosmeticSlots[j] = null;
				}
				else
				{
					equipment.m_cosmeticSlots[j] = ItemClass.GetItemClass(Equipment.CosmeticMappingIDString[num3], false);
				}
			}
			equipment.m_unlockedCosmetics.Clear();
			int num4 = reader.ReadInt32();
			for (int k = 0; k < num4; k++)
			{
				equipment.m_unlockedCosmetics.Add(reader.ReadInt32());
			}
		}
		return equipment;
	}

	// Token: 0x060026E7 RID: 9959 RVA: 0x000FC730 File Offset: 0x000FA930
	public virtual bool ReturnItem(ItemStack _itemStack, bool isLocal = true)
	{
		int num = this.PreferredItemSlot(_itemStack);
		if (num < 0 || num >= this.m_slots.Length)
		{
			return false;
		}
		if (this.m_slots[num] == null)
		{
			this.SetSlotItem(num, _itemStack.itemValue, isLocal);
			return true;
		}
		return false;
	}

	// Token: 0x060026E8 RID: 9960 RVA: 0x000FC774 File Offset: 0x000FA974
	public void Apply(Equipment eq, bool isLocal = true)
	{
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			this.SetSlotItem(i, eq.m_slots[i], isLocal);
		}
		for (int j = 0; j < this.m_cosmeticSlots.Length; j++)
		{
			this.m_cosmeticSlots[j] = eq.m_cosmeticSlots[j];
		}
		this.m_unlockedCosmetics.Clear();
		for (int k = 0; k < eq.m_unlockedCosmetics.Count; k++)
		{
			this.m_unlockedCosmetics.Add(eq.m_unlockedCosmetics[k]);
		}
		this.FireEventsForSetSlots();
		EModelSDCS emodelSDCS = this.m_entity.emodel as EModelSDCS;
		if (emodelSDCS != null)
		{
			emodelSDCS.UpdateEquipment();
		}
		this.FireEventsForChangedSlots();
	}

	// Token: 0x060026E9 RID: 9961 RVA: 0x000FC825 File Offset: 0x000FAA25
	public void InitializeEquipmentTransforms()
	{
		this.FireEventsForSlots(MinEventTypes.onSelfEquipStop, -1);
		this.FireEventsForSlots(MinEventTypes.onSelfEquipStart, -1);
		this.slotsSetFlags = 0;
		this.slotsChangedFlags = -1;
		this.FireEventsForChangedSlots();
	}

	// Token: 0x060026EA RID: 9962 RVA: 0x000FC850 File Offset: 0x000FAA50
	public Equipment Clone()
	{
		Equipment equipment = new Equipment();
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			if (this.m_slots[i] != null)
			{
				equipment.m_slots[i] = this.m_slots[i].Clone();
			}
		}
		for (int j = 0; j < this.m_cosmeticSlots.Length; j++)
		{
			ItemClass itemClass = this.m_cosmeticSlots[j];
			equipment.m_cosmeticSlots[j] = this.m_cosmeticSlots[j];
		}
		for (int k = 0; k < this.m_unlockedCosmetics.Count; k++)
		{
			if (equipment.m_unlockedCosmetics == null)
			{
				equipment.m_unlockedCosmetics = new List<int>();
			}
			equipment.m_unlockedCosmetics.Add(this.m_unlockedCosmetics[k]);
		}
		return equipment;
	}

	// Token: 0x060026EB RID: 9963 RVA: 0x000FC904 File Offset: 0x000FAB04
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddArmorGroup(string armorGroup, int quality)
	{
		if (this.ArmorGroupEquipped.ContainsKey(armorGroup))
		{
			Equipment.ArmorGroupInfo armorGroupInfo = this.ArmorGroupEquipped[armorGroup];
			armorGroupInfo.Count++;
			if (armorGroupInfo.LowestQuality > quality)
			{
				armorGroupInfo.LowestQuality = quality;
				return;
			}
		}
		else
		{
			this.ArmorGroupEquipped[armorGroup] = new Equipment.ArmorGroupInfo
			{
				Count = 1,
				LowestQuality = quality
			};
		}
	}

	// Token: 0x060026EC RID: 9964 RVA: 0x000FC96C File Offset: 0x000FAB6C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ResetArmorGroups()
	{
		this.ArmorGroupEquipped.Clear();
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue itemValue = this.m_slots[i];
			if (itemValue != null)
			{
				ItemClassArmor itemClassArmor = itemValue.ItemClass as ItemClassArmor;
				if (itemClassArmor != null)
				{
					for (int j = 0; j < itemClassArmor.ArmorGroup.Length; j++)
					{
						this.AddArmorGroup(itemClassArmor.ArmorGroup[j], (int)itemValue.Quality);
					}
				}
			}
		}
	}

	// Token: 0x060026ED RID: 9965 RVA: 0x000FC9D9 File Offset: 0x000FABD9
	public int GetArmorGroupCount(string armorGroup)
	{
		if (this.ArmorGroupEquipped.ContainsKey(armorGroup))
		{
			return this.ArmorGroupEquipped[armorGroup].Count;
		}
		return 0;
	}

	// Token: 0x060026EE RID: 9966 RVA: 0x000FC9FC File Offset: 0x000FABFC
	public int GetArmorGroupLowestQuality(string armorGroup)
	{
		if (this.ArmorGroupEquipped.ContainsKey(armorGroup))
		{
			return this.ArmorGroupEquipped[armorGroup].LowestQuality;
		}
		return 0;
	}

	// Token: 0x060026EF RID: 9967 RVA: 0x000FCA1F File Offset: 0x000FAC1F
	public static void AddCosmeticMapping(int ID, string name)
	{
		Equipment.CosmeticMappingIDString.Add(ID, name);
		Equipment.CosmeticMappingStringID.Add(name, ID);
	}

	// Token: 0x060026F0 RID: 9968 RVA: 0x000FCA3C File Offset: 0x000FAC3C
	public static void SetupCosmeticMapping()
	{
		Equipment.CosmeticMappingIDString.Clear();
		Equipment.CosmeticMappingStringID.Clear();
		Equipment.AddCosmeticMapping(-1, "missingItem");
		Equipment.AddCosmeticMapping(1, "armorPrimitiveHelmet");
		Equipment.AddCosmeticMapping(2, "armorPrimitiveOutfit");
		Equipment.AddCosmeticMapping(3, "armorPrimitiveGloves");
		Equipment.AddCosmeticMapping(4, "armorPrimitiveBoots");
		Equipment.AddCosmeticMapping(5, "armorLumberjackHelmet");
		Equipment.AddCosmeticMapping(6, "armorLumberjackOutfit");
		Equipment.AddCosmeticMapping(7, "armorLumberjackGloves");
		Equipment.AddCosmeticMapping(8, "armorLumberjackBoots");
		Equipment.AddCosmeticMapping(9, "armorPreacherHelmet");
		Equipment.AddCosmeticMapping(10, "armorPreacherOutfit");
		Equipment.AddCosmeticMapping(11, "armorPreacherGloves");
		Equipment.AddCosmeticMapping(12, "armorPreacherBoots");
		Equipment.AddCosmeticMapping(13, "armorRogueHelmet");
		Equipment.AddCosmeticMapping(14, "armorRogueOutfit");
		Equipment.AddCosmeticMapping(15, "armorRogueGloves");
		Equipment.AddCosmeticMapping(16, "armorRogueBoots");
		Equipment.AddCosmeticMapping(17, "armorAthleticHelmet");
		Equipment.AddCosmeticMapping(18, "armorAthleticOutfit");
		Equipment.AddCosmeticMapping(19, "armorAthleticGloves");
		Equipment.AddCosmeticMapping(20, "armorAthleticBoots");
		Equipment.AddCosmeticMapping(21, "armorEnforcerHelmet");
		Equipment.AddCosmeticMapping(22, "armorEnforcerOutfit");
		Equipment.AddCosmeticMapping(23, "armorEnforcerGloves");
		Equipment.AddCosmeticMapping(24, "armorEnforcerBoots");
		Equipment.AddCosmeticMapping(25, "armorFarmerHelmet");
		Equipment.AddCosmeticMapping(26, "armorFarmerOutfit");
		Equipment.AddCosmeticMapping(27, "armorFarmerGloves");
		Equipment.AddCosmeticMapping(28, "armorFarmerBoots");
		Equipment.AddCosmeticMapping(29, "armorBikerHelmet");
		Equipment.AddCosmeticMapping(30, "armorBikerOutfit");
		Equipment.AddCosmeticMapping(31, "armorBikerGloves");
		Equipment.AddCosmeticMapping(32, "armorBikerBoots");
		Equipment.AddCosmeticMapping(33, "armorScavengerHelmet");
		Equipment.AddCosmeticMapping(34, "armorScavengerOutfit");
		Equipment.AddCosmeticMapping(35, "armorScavengerGloves");
		Equipment.AddCosmeticMapping(36, "armorScavengerBoots");
		Equipment.AddCosmeticMapping(37, "armorRangerHelmet");
		Equipment.AddCosmeticMapping(38, "armorRangerOutfit");
		Equipment.AddCosmeticMapping(39, "armorRangerGloves");
		Equipment.AddCosmeticMapping(40, "armorRangerBoots");
		Equipment.AddCosmeticMapping(41, "armorCommandoHelmet");
		Equipment.AddCosmeticMapping(42, "armorCommandoOutfit");
		Equipment.AddCosmeticMapping(43, "armorCommandoGloves");
		Equipment.AddCosmeticMapping(44, "armorCommandoBoots");
		Equipment.AddCosmeticMapping(45, "armorAssassinHelmet");
		Equipment.AddCosmeticMapping(46, "armorAssassinOutfit");
		Equipment.AddCosmeticMapping(47, "armorAssassinGloves");
		Equipment.AddCosmeticMapping(48, "armorAssassinBoots");
		Equipment.AddCosmeticMapping(49, "armorMinerHelmet");
		Equipment.AddCosmeticMapping(50, "armorMinerOutfit");
		Equipment.AddCosmeticMapping(51, "armorMinerGloves");
		Equipment.AddCosmeticMapping(52, "armorMinerBoots");
		Equipment.AddCosmeticMapping(53, "armorNomadHelmet");
		Equipment.AddCosmeticMapping(54, "armorNomadOutfit");
		Equipment.AddCosmeticMapping(55, "armorNomadGloves");
		Equipment.AddCosmeticMapping(56, "armorNomadBoots");
		Equipment.AddCosmeticMapping(57, "armorNerdHelmet");
		Equipment.AddCosmeticMapping(58, "armorNerdOutfit");
		Equipment.AddCosmeticMapping(59, "armorNerdGloves");
		Equipment.AddCosmeticMapping(60, "armorNerdBoots");
		Equipment.AddCosmeticMapping(61, "armorRaiderHelmet");
		Equipment.AddCosmeticMapping(62, "armorRaiderOutfit");
		Equipment.AddCosmeticMapping(63, "armorRaiderGloves");
		Equipment.AddCosmeticMapping(64, "armorRaiderBoots");
		Equipment.AddCosmeticMapping(65, "armorDesertHelmet");
		Equipment.AddCosmeticMapping(66, "armorDesertOutfit");
		Equipment.AddCosmeticMapping(67, "armorDesertGloves");
		Equipment.AddCosmeticMapping(68, "armorDesertBoots");
		Equipment.AddCosmeticMapping(69, "armorHoarderHelmet");
		Equipment.AddCosmeticMapping(70, "armorHoarderOutfit");
		Equipment.AddCosmeticMapping(71, "armorHoarderGloves");
		Equipment.AddCosmeticMapping(72, "armorHoarderBoots");
		Equipment.AddCosmeticMapping(73, "armorMarauderHelmet");
		Equipment.AddCosmeticMapping(74, "armorMarauderOutfit");
		Equipment.AddCosmeticMapping(75, "armorMarauderGloves");
		Equipment.AddCosmeticMapping(76, "armorMarauderBoots");
		Equipment.AddCosmeticMapping(77, "armorCrimsonWarlordHelmet");
		Equipment.AddCosmeticMapping(78, "armorCrimsonWarlordOutfit");
		Equipment.AddCosmeticMapping(79, "armorCrimsonWarlordGloves");
		Equipment.AddCosmeticMapping(80, "armorCrimsonWarlordBoots");
		Equipment.AddCosmeticMapping(81, "armorSamuraiHelmet");
		Equipment.AddCosmeticMapping(82, "armorSamuraiOutfit");
		Equipment.AddCosmeticMapping(83, "armorSamuraiGloves");
		Equipment.AddCosmeticMapping(84, "armorSamuraiBoots");
		Equipment.AddCosmeticMapping(85, "armorPimpHatBlueHelmet");
		Equipment.AddCosmeticMapping(86, "armorPimpHatPurpleHelmet");
		Equipment.AddCosmeticMapping(87, "armorWatcherHelmet");
		Equipment.AddCosmeticMapping(88, "armorWatcherOutfit");
		Equipment.AddCosmeticMapping(89, "armorWatcherGloves");
		Equipment.AddCosmeticMapping(90, "armorWatcherBoots");
		Equipment.AddCosmeticMapping(91, "armorCrackABookHelmet");
		Equipment.AddCosmeticMapping(92, "armorCrackABookOutfit");
		Equipment.AddCosmeticMapping(93, "armorMoPowerHelmet");
		Equipment.AddCosmeticMapping(94, "armorMoPowerOutfit");
		Equipment.AddCosmeticMapping(95, "armorPassNGasHelmet");
		Equipment.AddCosmeticMapping(96, "armorPassNGasOutfit");
		Equipment.AddCosmeticMapping(97, "armorPopNPillsHelmet");
		Equipment.AddCosmeticMapping(98, "armorPopNPillsOutfit");
		Equipment.AddCosmeticMapping(99, "armorSavageCountryHelmet");
		Equipment.AddCosmeticMapping(100, "armorSavageCountryOutfit");
		Equipment.AddCosmeticMapping(101, "armorShamwayHelmet");
		Equipment.AddCosmeticMapping(102, "armorShamwayOutfit");
		Equipment.AddCosmeticMapping(103, "armorShotgunMessiahHelmet");
		Equipment.AddCosmeticMapping(104, "armorShotgunMessiahOutfit");
		Equipment.AddCosmeticMapping(105, "armorWorkingStiffsHelmet");
		Equipment.AddCosmeticMapping(106, "armorWorkingStiffsOutfit");
	}

	// Token: 0x04001DB0 RID: 7600
	[PublicizedFrom(EAccessModifier.Private)]
	public const int CurrentSaveVersion = 3;

	// Token: 0x04001DB2 RID: 7602
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue[] m_slots;

	// Token: 0x04001DB3 RID: 7603
	[PublicizedFrom(EAccessModifier.Private)]
	public int slotsSetFlags;

	// Token: 0x04001DB4 RID: 7604
	[PublicizedFrom(EAccessModifier.Private)]
	public int slotsChangedFlags;

	// Token: 0x04001DB5 RID: 7605
	[PublicizedFrom(EAccessModifier.Private)]
	public int[] preferredItemSlots;

	// Token: 0x04001DB6 RID: 7606
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass[] m_cosmeticSlots;

	// Token: 0x04001DB7 RID: 7607
	public List<int> m_unlockedCosmetics = new List<int>();

	// Token: 0x04001DB8 RID: 7608
	public int tempCosmeticSlotIndex = -1;

	// Token: 0x04001DB9 RID: 7609
	public ItemClass tempCosmeticSlot;

	// Token: 0x04001DBA RID: 7610
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive m_entity;

	// Token: 0x04001DBB RID: 7611
	[PublicizedFrom(EAccessModifier.Private)]
	public float insulation;

	// Token: 0x04001DBC RID: 7612
	[PublicizedFrom(EAccessModifier.Private)]
	public float waterProof;

	// Token: 0x04001DBD RID: 7613
	public float CurrentLowestDurability = 1f;

	// Token: 0x04001DBE RID: 7614
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, Equipment.ArmorGroupInfo> ArmorGroupEquipped = new Dictionary<string, Equipment.ArmorGroupInfo>();

	// Token: 0x04001DBF RID: 7615
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastUpdateTime;

	// Token: 0x04001DC1 RID: 7617
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> physicalDamageTypes = FastTags<TagGroup.Global>.Parse("piercing,bashing,slashing,crushing,none,corrosive");

	// Token: 0x04001DC2 RID: 7618
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> coreDamageResist = FastTags<TagGroup.Global>.Parse("coredamageresist");

	// Token: 0x04001DC3 RID: 7619
	public static Dictionary<int, string> CosmeticMappingIDString = new Dictionary<int, string>();

	// Token: 0x04001DC4 RID: 7620
	public static Dictionary<string, int> CosmeticMappingStringID = new Dictionary<string, int>();

	// Token: 0x020004A3 RID: 1187
	[PublicizedFrom(EAccessModifier.Private)]
	public class ArmorGroupInfo
	{
		// Token: 0x04001DC5 RID: 7621
		public int Count;

		// Token: 0x04001DC6 RID: 7622
		public int LowestQuality;
	}
}
