using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005BC RID: 1468
public static class EffectManager
{
	// Token: 0x06002F2C RID: 12076 RVA: 0x001430E4 File Offset: 0x001412E4
	public static float GetValue(PassiveEffects _passiveEffect, ItemValue _originalItemValue = null, float _originalValue = 0f, EntityAlive _entity = null, Recipe _recipe = null, FastTags<TagGroup.Global> tags = default(FastTags<TagGroup.Global>), bool calcEquipment = true, bool calcHoldingItem = true, bool calcProgression = true, bool calcBuffs = true, bool calcChallenges = true, int craftingTier = 1, bool useMods = true, bool _useDurability = false)
	{
		float num = 1f;
		if (_entity != null)
		{
			MinEventParams.CopyTo(_entity.MinEventContext, MinEventParams.CachedEventParam);
		}
		if (_originalItemValue != null)
		{
			if (_entity != null && _entity.MinEventContext.ItemValue == null)
			{
				_entity.MinEventContext.ItemValue = _originalItemValue;
			}
			MinEventParams.CachedEventParam.ItemValue = _originalItemValue;
			if (_originalItemValue.type != 0 && tags.IsEmpty)
			{
				ItemClass itemClass = _originalItemValue.ItemClass;
				if (itemClass != null)
				{
					tags = itemClass.ItemTags;
				}
			}
		}
		if (_entity == null)
		{
			if (_recipe != null)
			{
				_recipe.ModifyValue(_passiveEffect, ref _originalValue, ref num, tags, craftingTier);
			}
			if (_originalItemValue != null && _originalItemValue.type != 0 && _originalItemValue.ItemClass != null)
			{
				_originalItemValue.ModifyValue(_entity, null, _passiveEffect, ref _originalValue, ref num, tags, true, false);
			}
		}
		else
		{
			if (GameManager.Instance == null || GameManager.Instance.gameStateManager == null || !GameManager.Instance.gameStateManager.IsGameStarted())
			{
				return _originalValue;
			}
			EntityClass entityClass;
			if (EntityClass.list.TryGetValue(_entity.entityClass, out entityClass) && entityClass.Effects != null)
			{
				entityClass.Effects.ModifyValue(_entity, _passiveEffect, ref _originalValue, ref num, 0f, tags, 1);
			}
			if (_originalItemValue != null && _originalItemValue.type != 0 && _originalItemValue.ItemClass != null)
			{
				_originalItemValue.ModifyValue(_entity, null, _passiveEffect, ref _originalValue, ref num, tags, useMods, false);
			}
			else
			{
				EntityVehicle entityVehicle = _entity as EntityVehicle;
				if (entityVehicle != null)
				{
					Vehicle vehicle = entityVehicle.GetVehicle();
					if (vehicle != null)
					{
						vehicle.GetUpdatedItemValue().ModifyValue(_entity, null, _passiveEffect, ref _originalValue, ref num, tags, true, false);
					}
				}
				else if (calcHoldingItem && _entity.inventory != null && _entity.inventory.holdingItemItemValue != _originalItemValue && !_entity.inventory.holdingItemItemValue.IsMod)
				{
					_entity.inventory.ModifyValue(_originalItemValue, _passiveEffect, ref _originalValue, ref num, tags);
				}
			}
			if (calcEquipment && _entity.equipment != null)
			{
				_entity.equipment.ModifyValue(_originalItemValue, _passiveEffect, ref _originalValue, ref num, tags, _useDurability);
			}
			if (_originalItemValue != null)
			{
				if (_entity != null)
				{
					_entity.MinEventContext.ItemValue = _originalItemValue;
				}
				MinEventParams.CachedEventParam.ItemValue = _originalItemValue;
			}
			if (calcProgression && _entity.Progression != null)
			{
				_entity.Progression.ModifyValue(_passiveEffect, ref _originalValue, ref num, tags);
			}
			if (calcChallenges && _entity.challengeJournal != null)
			{
				_entity.challengeJournal.ModifyValue(_passiveEffect, ref _originalValue, ref num, tags);
			}
			if (_recipe != null)
			{
				_recipe.ModifyValue(_passiveEffect, ref _originalValue, ref num, tags, craftingTier);
			}
			EntityPlayerLocal entityPlayerLocal = _entity as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				if (EffectManager.slotsCached == null || _entity.entityId != EffectManager.slotsQueriedForEntity || EffectManager.slotsQueriedFrame != Time.frameCount)
				{
					LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
					EffectManager.slotsCached = ((uiforPlayer.xui.currentWorkstationToolGrid != null) ? uiforPlayer.xui.currentWorkstationToolGrid.GetSlots() : null);
					EffectManager.slotsQueriedFrame = Time.frameCount;
					EffectManager.slotsQueriedForEntity = _entity.entityId;
				}
				if (EffectManager.slotsCached != null)
				{
					for (int i = 0; i < EffectManager.slotsCached.Length; i++)
					{
						if (!EffectManager.slotsCached[i].IsEmpty())
						{
							EffectManager.slotsCached[i].itemValue.ModifyValue(_entity, null, _passiveEffect, ref _originalValue, ref num, tags, true, false);
						}
					}
				}
			}
			if (calcBuffs && _entity.Buffs != null)
			{
				_entity.Buffs.ModifyValue(_passiveEffect, ref _originalValue, ref num, tags);
			}
		}
		if (_originalItemValue != null && _originalItemValue.ItemClass != null && _originalItemValue.Quality > 0 && useMods && _originalItemValue.ItemClass.Effects != null)
		{
			for (int j = 0; j < _originalItemValue.Modifications.Length; j++)
			{
				if (_originalItemValue.Modifications[j] != null && _originalItemValue.Modifications[j].ItemClass is ItemClassModifier)
				{
					_originalItemValue.ItemClass.Effects.ModifyValue(_entity, PassiveEffects.ModPowerBonus, ref _originalValue, ref num, (float)_originalItemValue.Quality, FastTags<TagGroup.Global>.Parse(_passiveEffect.ToStringCached<PassiveEffects>()), 1);
				}
			}
		}
		return _originalValue * num;
	}

	// Token: 0x06002F2D RID: 12077 RVA: 0x001434B0 File Offset: 0x001416B0
	public static float GetItemValue(PassiveEffects _passiveEffect, ItemValue _originalItemValue, float _originalValue = 0f)
	{
		float num = 1f;
		if (_originalItemValue != null && _originalItemValue.type != 0 && _originalItemValue.ItemClass != null)
		{
			MinEventParams.CachedEventParam.ItemValue = _originalItemValue;
			_originalItemValue.ModifyValue(null, null, _passiveEffect, ref _originalValue, ref num, _originalItemValue.ItemClass.ItemTags, true, false);
		}
		return _originalValue * num;
	}

	// Token: 0x06002F2E RID: 12078 RVA: 0x00143500 File Offset: 0x00141700
	public static float GetDisplayValues(PassiveEffects _passiveEffect, out float baseValueChange, out float percValueMultiplier, ItemValue _originalItemValue = null, float _originalValue = 0f, EntityAlive _entity = null, Recipe _recipe = null, FastTags<TagGroup.Global> tags = default(FastTags<TagGroup.Global>), int craftingTier = 1)
	{
		float num = _originalValue;
		baseValueChange = 0f;
		percValueMultiplier = 1f;
		if (GameManager.Instance == null || GameManager.Instance.gameStateManager == null || !GameManager.Instance.gameStateManager.IsGameStarted())
		{
			return _originalValue;
		}
		if (_entity == null)
		{
			if (_recipe != null)
			{
				_recipe.ModifyValue(_passiveEffect, ref baseValueChange, ref percValueMultiplier, tags, craftingTier);
			}
			if (_originalItemValue != null && _originalItemValue.type != 0 && _originalItemValue.ItemClass != null)
			{
				_originalItemValue.ModifyValue(_entity, null, _passiveEffect, ref _originalValue, ref percValueMultiplier, tags, true, false);
			}
		}
		else
		{
			if (EntityClass.list.ContainsKey(_entity.entityClass) && EntityClass.list[_entity.entityClass].Effects != null)
			{
				EntityClass.list[_entity.entityClass].Effects.ModifyValue(_entity, _passiveEffect, ref _originalValue, ref percValueMultiplier, 0f, tags, 1);
			}
			if (_originalItemValue != null && _originalItemValue.type != 0 && _originalItemValue.ItemClass != null)
			{
				_originalItemValue.ModifyValue(_entity, null, _passiveEffect, ref _originalValue, ref percValueMultiplier, tags, true, false);
			}
			else
			{
				if (_entity.inventory != null && _entity.inventory.holdingItemItemValue != _originalItemValue)
				{
					_entity.inventory.ModifyValue(_originalItemValue, _passiveEffect, ref _originalValue, ref percValueMultiplier, tags);
				}
				if (_entity.equipment != null)
				{
					_entity.equipment.ModifyValue(_originalItemValue, _passiveEffect, ref _originalValue, ref percValueMultiplier, tags, false);
				}
			}
			if (_entity.Progression != null)
			{
				_entity.Progression.ModifyValue(_passiveEffect, ref _originalValue, ref percValueMultiplier, tags);
			}
			if (_recipe != null)
			{
				_recipe.ModifyValue(_passiveEffect, ref baseValueChange, ref percValueMultiplier, tags, craftingTier);
			}
			if (_entity.Buffs != null)
			{
				_entity.Buffs.ModifyValue(_passiveEffect, ref _originalValue, ref percValueMultiplier, tags);
			}
		}
		if (_originalItemValue != null && _originalItemValue.ItemClass != null && _originalItemValue.Quality > 0 && _originalItemValue.ItemClass.Effects != null)
		{
			for (int i = 0; i < _originalItemValue.Modifications.Length; i++)
			{
				if (_originalItemValue.Modifications[i] != null && _originalItemValue.Modifications[i].ItemClass is ItemClassModifier)
				{
					_originalItemValue.ItemClass.Effects.ModifyValue(_entity, PassiveEffects.ModPowerBonus, ref _originalValue, ref percValueMultiplier, (float)_originalItemValue.Quality, FastTags<TagGroup.Global>.Parse(_passiveEffect.ToStringCached<PassiveEffects>()), 1);
				}
			}
		}
		baseValueChange = _originalValue - num;
		return _originalValue * percValueMultiplier;
	}

	// Token: 0x06002F2F RID: 12079 RVA: 0x0014372C File Offset: 0x0014192C
	public static string GetInfoString(PassiveEffects _gAttribute, ItemValue _itemValue, EntityAlive _ea = null, float modAmount = 0f)
	{
		return string.Format("{0}: {1}\n", _gAttribute.ToStringCached<PassiveEffects>(), (modAmount + EffectManager.GetValue(_gAttribute, _itemValue, 0f, _ea, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false)).ToCultureInvariantString("0.0"));
	}

	// Token: 0x06002F30 RID: 12080 RVA: 0x00143774 File Offset: 0x00141974
	public static string GetColoredInfoString(PassiveEffects _passiveEffect, ItemValue _itemValue, EntityAlive _ea = null)
	{
		EffectManager.GetDisplayValues(_passiveEffect, out EffectManager.cInfoStringBaseValue, out EffectManager.cInfoStringPercValue, _itemValue, 0f, _ea, null, default(FastTags<TagGroup.Global>), 1);
		return string.Format("{0}: [REPLACE_COLOR]{1}*{2}[-]\n", _passiveEffect.ToStringCached<PassiveEffects>(), EffectManager.cInfoStringBaseValue.ToCultureInvariantString("0.0"), EffectManager.cInfoStringPercValue.ToCultureInvariantString("0.0"));
	}

	// Token: 0x06002F31 RID: 12081 RVA: 0x001437D4 File Offset: 0x001419D4
	public static List<EffectManager.ModifierValuesAndSources> GetValuesAndSources(PassiveEffects _passiveEffect, ItemValue _originalItemValue = null, float _originalValue = 0f, EntityAlive _entity = null, Recipe _recipe = null, FastTags<TagGroup.Global> tags = default(FastTags<TagGroup.Global>), bool calcEquipment = true, bool calcHoldingItem = true)
	{
		float num = 1f;
		List<EffectManager.ModifierValuesAndSources> list = new List<EffectManager.ModifierValuesAndSources>();
		if (_entity == null)
		{
			if (_originalItemValue != null && _originalItemValue.type != 0 && _originalItemValue.ItemClass != null)
			{
				_originalItemValue.GetModifiedValueData(list, EffectManager.ModifierValuesAndSources.ValueSourceType.Self, _entity, null, _passiveEffect, ref _originalValue, ref num, tags);
			}
		}
		else
		{
			if (GameManager.Instance == null || GameManager.Instance.gameStateManager == null || !GameManager.Instance.gameStateManager.IsGameStarted())
			{
				return list;
			}
			if (EntityClass.list.ContainsKey(_entity.entityClass) && EntityClass.list[_entity.entityClass].Effects != null)
			{
				EntityClass.list[_entity.entityClass].Effects.GetModifiedValueData(list, EffectManager.ModifierValuesAndSources.ValueSourceType.Base, _entity, _passiveEffect, ref _originalValue, ref num, 0f, tags, 1);
			}
			if (_originalItemValue != null && _originalItemValue.type != 0 && _originalItemValue.ItemClass != null)
			{
				_originalItemValue.GetModifiedValueData(list, EffectManager.ModifierValuesAndSources.ValueSourceType.Self, _entity, null, _passiveEffect, ref _originalValue, ref num, tags);
			}
			else
			{
				if (calcHoldingItem && _entity.inventory != null && _entity.inventory.holdingItemItemValue != _originalItemValue && !_entity.inventory.holdingItemItemValue.IsMod)
				{
					_entity.inventory.holdingItemItemValue.GetModifiedValueData(list, EffectManager.ModifierValuesAndSources.ValueSourceType.Held, _entity, _originalItemValue, _passiveEffect, ref _originalValue, ref num, tags);
				}
				if (calcEquipment && _entity.equipment != null)
				{
					_entity.equipment.GetModifiedValueData(list, EffectManager.ModifierValuesAndSources.ValueSourceType.Worn, _originalItemValue, _passiveEffect, ref _originalValue, ref num, tags);
				}
			}
			if (_entity.Progression != null)
			{
				_entity.Progression.GetModifiedValueData(list, EffectManager.ModifierValuesAndSources.ValueSourceType.Progression, _passiveEffect, ref _originalValue, ref num, tags);
			}
			if (_entity.Buffs != null)
			{
				_entity.Buffs.GetModifiedValueData(list, EffectManager.ModifierValuesAndSources.ValueSourceType.Buff, _passiveEffect, ref _originalValue, ref num, tags);
			}
		}
		if (_originalItemValue != null && _originalItemValue.ItemClass != null && _originalItemValue.Quality > 0 && _originalItemValue.ItemClass.Effects != null)
		{
			for (int i = 0; i < _originalItemValue.Modifications.Length; i++)
			{
				if (_originalItemValue.Modifications[i] != null && _originalItemValue.Modifications[i].ItemClass is ItemClassModifier)
				{
					_originalItemValue.ItemClass.Effects.GetModifiedValueData(list, EffectManager.ModifierValuesAndSources.ValueSourceType.ModBonus, _entity, PassiveEffects.ModPowerBonus, ref _originalValue, ref num, (float)_originalItemValue.Quality, FastTags<TagGroup.Global>.Parse(_passiveEffect.ToStringCached<PassiveEffects>()), 1);
				}
			}
		}
		return list;
	}

	// Token: 0x04002621 RID: 9761
	public static FastEnumIntEqualityComparer<PassiveEffects> PassiveEffectsComparer = new FastEnumIntEqualityComparer<PassiveEffects>();

	// Token: 0x04002622 RID: 9762
	[PublicizedFrom(EAccessModifier.Private)]
	public static int slotsQueriedFrame;

	// Token: 0x04002623 RID: 9763
	[PublicizedFrom(EAccessModifier.Private)]
	public static int slotsQueriedForEntity;

	// Token: 0x04002624 RID: 9764
	[PublicizedFrom(EAccessModifier.Private)]
	public static ItemStack[] slotsCached;

	// Token: 0x04002625 RID: 9765
	[PublicizedFrom(EAccessModifier.Private)]
	public static float cInfoStringBaseValue;

	// Token: 0x04002626 RID: 9766
	[PublicizedFrom(EAccessModifier.Private)]
	public static float cInfoStringPercValue;

	// Token: 0x020005BD RID: 1469
	public class ModifierValuesAndSources
	{
		// Token: 0x04002627 RID: 9767
		public PassiveEffects PassiveEffectName;

		// Token: 0x04002628 RID: 9768
		public MinEffectController.SourceParentType ParentType;

		// Token: 0x04002629 RID: 9769
		public EffectManager.ModifierValuesAndSources.ValueSourceType ValueSource;

		// Token: 0x0400262A RID: 9770
		public EffectManager.ModifierValuesAndSources.ValueTypes ValueType;

		// Token: 0x0400262B RID: 9771
		public FastTags<TagGroup.Global> Tags;

		// Token: 0x0400262C RID: 9772
		public object Source;

		// Token: 0x0400262D RID: 9773
		public float Value;

		// Token: 0x0400262E RID: 9774
		public PassiveEffect.ValueModifierTypes ModifierType;

		// Token: 0x0400262F RID: 9775
		public int ModItemSource;

		// Token: 0x020005BE RID: 1470
		public enum ValueSourceType
		{
			// Token: 0x04002631 RID: 9777
			None,
			// Token: 0x04002632 RID: 9778
			Self,
			// Token: 0x04002633 RID: 9779
			Held,
			// Token: 0x04002634 RID: 9780
			Worn,
			// Token: 0x04002635 RID: 9781
			Attribute,
			// Token: 0x04002636 RID: 9782
			Skill,
			// Token: 0x04002637 RID: 9783
			Perk,
			// Token: 0x04002638 RID: 9784
			Mod,
			// Token: 0x04002639 RID: 9785
			CosmeticMod,
			// Token: 0x0400263A RID: 9786
			Fault,
			// Token: 0x0400263B RID: 9787
			Buff,
			// Token: 0x0400263C RID: 9788
			Progression,
			// Token: 0x0400263D RID: 9789
			Base,
			// Token: 0x0400263E RID: 9790
			Ammo,
			// Token: 0x0400263F RID: 9791
			ModBonus
		}

		// Token: 0x020005BF RID: 1471
		public enum ValueTypes
		{
			// Token: 0x04002641 RID: 9793
			None,
			// Token: 0x04002642 RID: 9794
			BaseValue,
			// Token: 0x04002643 RID: 9795
			PercentValue
		}

		// Token: 0x020005C0 RID: 1472
		public enum ModTypes
		{
			// Token: 0x04002645 RID: 9797
			None,
			// Token: 0x04002646 RID: 9798
			Base,
			// Token: 0x04002647 RID: 9799
			Percentage
		}
	}
}
