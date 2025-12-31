using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000581 RID: 1409
public class LootContainer
{
	// Token: 0x06002D9F RID: 11679 RVA: 0x00130289 File Offset: 0x0012E489
	public static void InitStatic()
	{
		LootContainer.Cleanup();
	}

	// Token: 0x06002DA0 RID: 11680 RVA: 0x00130290 File Offset: 0x0012E490
	public void Init()
	{
		LootContainer.lootContainers[this.Name] = this;
	}

	// Token: 0x06002DA1 RID: 11681 RVA: 0x001302A3 File Offset: 0x0012E4A3
	public static void Cleanup()
	{
		LootContainer.lootContainers.Clear();
		LootContainer.lootGroups.Clear();
		LootContainer.lootQualityTemplates.Clear();
		LootContainer.lootProbTemplates.Clear();
	}

	// Token: 0x06002DA2 RID: 11682 RVA: 0x001302CD File Offset: 0x0012E4CD
	public static bool IsLoaded()
	{
		return LootContainer.lootContainers.Count > 0;
	}

	// Token: 0x06002DA3 RID: 11683 RVA: 0x001302DC File Offset: 0x0012E4DC
	public static LootContainer GetLootContainer(string _name, bool _errorOnMiss = true)
	{
		if (string.IsNullOrEmpty(_name))
		{
			return null;
		}
		LootContainer result;
		if (LootContainer.lootContainers.TryGetValue(_name, out result))
		{
			return result;
		}
		if (_errorOnMiss)
		{
			Log.Error("LootContainer '" + _name + "' unknown");
		}
		return null;
	}

	// Token: 0x06002DA4 RID: 11684 RVA: 0x00130320 File Offset: 0x0012E520
	public static ItemStack GetRewardItem(string lootGroup, float questDifficulty)
	{
		if (!LootContainer.lootGroups.ContainsKey(lootGroup))
		{
			return ItemStack.Empty.Clone();
		}
		List<ItemStack> list = new List<ItemStack>();
		int num = 1;
		LootContainer.SpawnItemsFromGroup(GameManager.Instance.lootManager.Random, LootContainer.lootGroups[lootGroup], 1, 1f, list, ref num, questDifficulty, 0f, LootContainer.lootGroups[lootGroup].lootQualityTemplate, null, FastTags<TagGroup.Global>.none, true, true, false, null);
		if (list.Count == 0)
		{
			return ItemStack.Empty.Clone();
		}
		return list[0];
	}

	// Token: 0x06002DA5 RID: 11685 RVA: 0x001303B0 File Offset: 0x0012E5B0
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool SpawnItem(GameRandom random, LootContainer.LootEntry template, ItemValue lootItemValue, int countToSpawn, List<ItemStack> spawnedItems, ref int slotsLeft, float gameStage, string lootQualityTemplate, EntityPlayer player, FastTags<TagGroup.Global> containerTags, bool _forceStacking)
	{
		if (lootItemValue.ItemClass == null)
		{
			return false;
		}
		if (player != null)
		{
			countToSpawn = Math.Min((int)EffectManager.GetValue(PassiveEffects.LootQuantity, player.inventory.holdingItemItemValue, (float)countToSpawn, player, null, lootItemValue.ItemClass.ItemTags | containerTags, true, true, true, true, true, 1, true, false), lootItemValue.ItemClass.Stacknumber.Value);
		}
		if (countToSpawn < 1)
		{
			return false;
		}
		if (lootItemValue.ItemClass.CanStack())
		{
			int value = lootItemValue.ItemClass.Stacknumber.Value;
			for (int i = 0; i < spawnedItems.Count; i++)
			{
				ItemStack itemStack = spawnedItems[i];
				if (itemStack.itemValue.type == lootItemValue.type)
				{
					if (itemStack.CanStack(countToSpawn) || _forceStacking)
					{
						itemStack.count += countToSpawn;
						return true;
					}
					int num = value - itemStack.count;
					itemStack.count = value;
					countToSpawn -= num;
				}
			}
		}
		if (slotsLeft < 1)
		{
			return false;
		}
		int num2 = template.minQuality;
		int maxQuality = template.maxQuality;
		string text = lootQualityTemplate;
		if (!string.IsNullOrEmpty(text))
		{
			LootContainer.LootGroup parentGroup = template.parentGroup;
			if (((parentGroup != null) ? parentGroup.lootQualityTemplate : null) == null)
			{
				goto IL_13C;
			}
		}
		LootContainer.LootGroup parentGroup2 = template.parentGroup;
		text = ((parentGroup2 != null) ? parentGroup2.lootQualityTemplate : null);
		IL_13C:
		if (!string.IsNullOrEmpty(text))
		{
			bool flag = false;
			for (int j = 0; j < LootContainer.lootQualityTemplates[text].templates.Count; j++)
			{
				float randomFloat = random.RandomFloat;
				LootContainer.LootGroup lootGroup = LootContainer.lootQualityTemplates[text].templates[j];
				num2 = lootGroup.minQuality;
				maxQuality = lootGroup.maxQuality;
				if (lootGroup.minLevel <= gameStage && lootGroup.maxLevel >= gameStage)
				{
					for (int k = 0; k < lootGroup.items.Count; k++)
					{
						LootContainer.LootEntry lootEntry = lootGroup.items[k];
						if (random.RandomFloat <= lootEntry.prob)
						{
							num2 = lootEntry.minQuality;
							maxQuality = lootEntry.maxQuality;
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
		}
		string[] modsToInstall = template.modsToInstall;
		float modChance = template.modChance;
		if (template.parentGroup != null && template.parentGroup.modsToInstall.Length != 0)
		{
			modsToInstall = template.parentGroup.modsToInstall;
			modChance = template.parentGroup.modChance;
		}
		ItemValue itemValue;
		if (lootItemValue.HasQuality)
		{
			if (num2 <= -1)
			{
				num2 = 1;
				maxQuality = 6;
			}
			itemValue = new ItemValue(lootItemValue.type, num2, maxQuality, true, modsToInstall, modChance);
		}
		else
		{
			itemValue = new ItemValue(lootItemValue.type, 1, 6, true, modsToInstall, modChance);
		}
		ItemClass itemClass = itemValue.ItemClass;
		if (itemClass != null)
		{
			if (itemClass.Actions != null && itemClass.Actions.Length != 0 && itemClass.Actions[0] != null)
			{
				itemValue.Meta = 0;
			}
			if (itemValue.MaxUseTimes > 0)
			{
				if (template.randomDurability)
				{
					itemValue.UseTimes = (float)((int)((float)itemValue.MaxUseTimes * random.RandomRange(0.2f, 0.8f)));
				}
				else
				{
					itemValue.UseTimes = 0f;
				}
			}
		}
		ItemStack item;
		if (player != null)
		{
			if (!LootContainer.OverrideItems.ContainsKey(player))
			{
				item = new ItemStack(itemValue, countToSpawn);
			}
			else
			{
				string[] array = LootContainer.OverrideItems[player];
				item = new ItemStack(ItemClass.GetItem(array[random.RandomRange(array.Length)], false), 1);
			}
		}
		else
		{
			item = new ItemStack(itemValue, countToSpawn);
		}
		spawnedItems.Add(item);
		slotsLeft--;
		return true;
	}

	// Token: 0x06002DA6 RID: 11686 RVA: 0x00130720 File Offset: 0x0012E920
	public static int RandomSpawnCount(GameRandom random, int min, int max, float abundance)
	{
		if (min < 0)
		{
			return -1;
		}
		float num = random.RandomRange((float)min - 0.49f, (float)max + 0.49f);
		if (num < (float)min)
		{
			num = (float)min;
		}
		if (num > (float)max)
		{
			num = (float)max;
		}
		num *= abundance;
		int num2 = (int)num;
		float num3 = num - (float)num2;
		if (random.RandomFloat < num3)
		{
			num2++;
		}
		return num2;
	}

	// Token: 0x06002DA7 RID: 11687 RVA: 0x00130774 File Offset: 0x0012E974
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool SpawnAllItemsFromList(GameRandom random, List<LootContainer.LootEntry> itemSet, float abundance, List<ItemStack> spawnedItems, ref int slotsLeft, float playerLevelPercentage, float rareLootChance, string lootQualityTemplate, EntityPlayer player, FastTags<TagGroup.Global> containerTags, bool uniqueItems, bool ignoreLootProb, bool _forceStacking, List<string> _buffsToAdd)
	{
		bool flag = false;
		for (int i = 0; i < itemSet.Count; i++)
		{
			LootContainer.LootEntry lootEntry = itemSet[i];
			bool flag2 = false;
			if (!(player != null) || lootEntry.HasRequirements(player))
			{
				if (!lootEntry.forceProb || random.RandomFloat <= LootContainer.getProbability(player, lootEntry, playerLevelPercentage, ignoreLootProb))
				{
					int num = LootContainer.RandomSpawnCount(random, lootEntry.minCount, lootEntry.maxCount, (lootEntry.group == null) ? abundance : 1f);
					if (lootEntry.group != null)
					{
						if (lootEntry.group.minLevel <= playerLevelPercentage && lootEntry.group.maxLevel >= playerLevelPercentage)
						{
							flag2 = LootContainer.SpawnItemsFromGroup(random, lootEntry.group, num, abundance, spawnedItems, ref slotsLeft, playerLevelPercentage, rareLootChance, lootQualityTemplate, player, containerTags, uniqueItems, ignoreLootProb, _forceStacking, _buffsToAdd);
						}
					}
					else
					{
						flag2 = LootContainer.SpawnItem(random, lootEntry, lootEntry.item.itemValue, num, spawnedItems, ref slotsLeft, playerLevelPercentage, lootQualityTemplate, player, containerTags, _forceStacking);
					}
				}
				flag = (flag || flag2);
				if (flag2 && lootEntry.buffsToAdd != null)
				{
					_buffsToAdd.AddRange(lootEntry.buffsToAdd);
				}
			}
		}
		return flag;
	}

	// Token: 0x06002DA8 RID: 11688 RVA: 0x0013088C File Offset: 0x0012EA8C
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool SpawnItemsFromGroup(GameRandom random, LootContainer.LootGroup group, int numToSpawn, float abundance, List<ItemStack> spawnedItems, ref int slotsLeft, float gameStage, float rareLootChance, string lootQualityTemplate, EntityPlayer player, FastTags<TagGroup.Global> containerTags, bool uniqueItems, bool ignoreLootProb, bool _forceStacking, List<string> _buffsToAdd)
	{
		bool flag = false;
		int num = 0;
		while (num < numToSpawn && slotsLeft > 0)
		{
			flag |= LootContainer.SpawnLootItemsFromList(random, group.items, LootContainer.RandomSpawnCount(random, group.minCount, group.maxCount, 1f), abundance, spawnedItems, ref slotsLeft, gameStage, rareLootChance, lootQualityTemplate, player, containerTags, uniqueItems, ignoreLootProb, _forceStacking, _buffsToAdd);
			num++;
		}
		return flag;
	}

	// Token: 0x06002DA9 RID: 11689 RVA: 0x001308EC File Offset: 0x0012EAEC
	public static bool SpawnLootItemsFromList(GameRandom random, List<LootContainer.LootEntry> itemSet, int numToSpawn, float abundance, List<ItemStack> spawnedItems, ref int slotsLeft, float lootStage, float rareLootChance, string lootQualityTemplate, EntityPlayer player, FastTags<TagGroup.Global> containerTags, bool uniqueItems, bool ignoreLootProb, bool _forceStacking, List<string> _buffsToAdd)
	{
		if (numToSpawn < 1)
		{
			return numToSpawn == -1 && LootContainer.SpawnAllItemsFromList(random, itemSet, abundance, spawnedItems, ref slotsLeft, lootStage, rareLootChance, lootQualityTemplate, player, containerTags, uniqueItems, ignoreLootProb, _forceStacking, _buffsToAdd);
		}
		float num = 0f;
		for (int i = 0; i < itemSet.Count; i++)
		{
			LootContainer.LootEntry lootEntry = itemSet[i];
			if (!lootEntry.forceProb)
			{
				num += LootContainer.getProbability(player, lootEntry, lootStage, ignoreLootProb);
			}
		}
		if (num == 0f)
		{
			return false;
		}
		List<int> list = new List<int>();
		bool flag = false;
		for (int j = 0; j < numToSpawn; j++)
		{
			float num2 = 0f;
			float randomFloat = random.RandomFloat;
			for (int k = 0; k < itemSet.Count; k++)
			{
				bool flag2 = false;
				LootContainer.LootEntry lootEntry2 = itemSet[k];
				if (!list.Contains(k) || (!lootEntry2.forceProb && !uniqueItems))
				{
					float probability = LootContainer.getProbability(player, lootEntry2, lootStage, ignoreLootProb);
					bool flag3;
					if (lootEntry2.forceProb)
					{
						flag3 = (random.RandomFloat <= probability);
					}
					else
					{
						num2 += probability / num;
						flag3 = (randomFloat <= num2 + rareLootChance);
					}
					if (flag3)
					{
						list.Add(k);
						if (uniqueItems)
						{
							num -= LootContainer.getProbability(player, lootEntry2, lootStage, ignoreLootProb);
						}
						int num3 = LootContainer.RandomSpawnCount(random, lootEntry2.minCount, lootEntry2.maxCount, (lootEntry2.group == null) ? abundance : 1f);
						num3 += Mathf.RoundToInt((float)num3 * (lootEntry2.lootstageCountMod * lootStage));
						if (lootEntry2.group != null)
						{
							if (lootEntry2.group.minLevel <= lootStage && lootEntry2.group.maxLevel >= lootStage)
							{
								flag2 = LootContainer.SpawnItemsFromGroup(random, lootEntry2.group, num3, abundance, spawnedItems, ref slotsLeft, lootStage, rareLootChance, lootQualityTemplate, player, containerTags, uniqueItems, ignoreLootProb, _forceStacking, _buffsToAdd);
							}
						}
						else
						{
							flag2 = LootContainer.SpawnItem(random, lootEntry2, lootEntry2.item.itemValue, num3, spawnedItems, ref slotsLeft, lootStage, lootQualityTemplate, player, containerTags, _forceStacking);
						}
						flag = (flag || flag2);
						if (flag2 && lootEntry2.buffsToAdd != null)
						{
							_buffsToAdd.AddRange(lootEntry2.buffsToAdd);
							break;
						}
						break;
					}
				}
			}
		}
		return flag;
	}

	// Token: 0x06002DAA RID: 11690 RVA: 0x00130B14 File Offset: 0x0012ED14
	[PublicizedFrom(EAccessModifier.Private)]
	public static float getProbability(EntityPlayer _player, LootContainer.LootEntry _item, float _lootstage, bool _ignoreLootProb)
	{
		if (_player != null && !_item.HasRequirements(_player))
		{
			return 0f;
		}
		if (_item.lootProbTemplate != string.Empty && LootContainer.lootProbTemplates.ContainsKey(_item.lootProbTemplate))
		{
			LootContainer.LootProbabilityTemplate lootProbabilityTemplate = LootContainer.lootProbTemplates[_item.lootProbTemplate];
			int i = 0;
			while (i < lootProbabilityTemplate.templates.Count)
			{
				LootContainer.LootEntry lootEntry = lootProbabilityTemplate.templates[i];
				if (lootEntry.minLevel <= _lootstage && lootEntry.maxLevel >= _lootstage)
				{
					if (_item.item != null && !_item.item.itemValue.ItemClass.ItemTags.IsEmpty)
					{
						if (_ignoreLootProb)
						{
							return lootEntry.prob;
						}
						return EffectManager.GetValue(PassiveEffects.LootProb, null, lootEntry.prob, _player, null, _item.item.itemValue.ItemClass.ItemTags, true, true, true, true, true, 1, true, false);
					}
					else
					{
						if (_item.tags.IsEmpty)
						{
							return lootEntry.prob;
						}
						return EffectManager.GetValue(PassiveEffects.LootProb, null, lootEntry.prob, _player, null, _item.tags, true, true, true, true, true, 1, true, false);
					}
				}
				else
				{
					i++;
				}
			}
		}
		if (_item.item != null && !_item.item.itemValue.ItemClass.ItemTags.IsEmpty)
		{
			if (_ignoreLootProb)
			{
				return _item.prob;
			}
			return EffectManager.GetValue(PassiveEffects.LootProb, null, _item.prob, _player, null, _item.item.itemValue.ItemClass.ItemTags, true, true, true, true, true, 1, true, false);
		}
		else
		{
			if (_item.tags.IsEmpty)
			{
				return _item.prob;
			}
			return EffectManager.GetValue(PassiveEffects.LootProb, null, _item.prob, _player, null, _item.tags, true, true, true, true, true, 1, true, false);
		}
	}

	// Token: 0x06002DAB RID: 11691 RVA: 0x00130CD4 File Offset: 0x0012EED4
	public void ExecuteBuffActions(int instigatorId, EntityAlive target)
	{
		if (this.BuffActions != null)
		{
			for (int i = 0; i < this.BuffActions.Count; i++)
			{
				target.Buffs.AddBuff(this.BuffActions[i], -1, true, false, -1f);
			}
		}
	}

	// Token: 0x06002DAC RID: 11692 RVA: 0x00130D20 File Offset: 0x0012EF20
	public IList<ItemStack> Spawn(GameRandom random, int _maxItems, float playerLevelPercentage, float rareLootChance, EntityPlayer player, FastTags<TagGroup.Global> containerTags, bool uniqueItems, bool ignoreLootProb)
	{
		List<ItemStack> list = new List<ItemStack>();
		int numToSpawn = Mathf.Min(LootContainer.RandomSpawnCount(random, this.minCount, this.maxCount, 1f), _maxItems);
		float abundance = 1f;
		if (!this.ignoreLootAbundance)
		{
			abundance = (float)GamePrefs.GetInt(EnumGamePrefs.LootAbundance) * 0.01f;
		}
		List<string> list2 = new List<string>();
		if (LootContainer.SpawnLootItemsFromList(random, this.itemsToSpawn, numToSpawn, abundance, list, ref _maxItems, playerLevelPercentage, rareLootChance, this.lootQualityTemplate, player, containerTags, uniqueItems, ignoreLootProb, false, list2))
		{
			foreach (string name in list2)
			{
				player.Buffs.AddBuff(name, -1, true, false, -1f);
			}
		}
		return list;
	}

	// Token: 0x04002418 RID: 9240
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<string, LootContainer> lootContainers = new CaseInsensitiveStringDictionary<LootContainer>();

	// Token: 0x04002419 RID: 9241
	public static readonly Dictionary<string, LootContainer.LootGroup> lootGroups = new Dictionary<string, LootContainer.LootGroup>();

	// Token: 0x0400241A RID: 9242
	public static readonly Dictionary<string, LootContainer.LootQualityTemplate> lootQualityTemplates = new Dictionary<string, LootContainer.LootQualityTemplate>();

	// Token: 0x0400241B RID: 9243
	public static readonly Dictionary<string, LootContainer.LootProbabilityTemplate> lootProbTemplates = new Dictionary<string, LootContainer.LootProbabilityTemplate>();

	// Token: 0x0400241C RID: 9244
	public string Name;

	// Token: 0x0400241D RID: 9245
	public string soundOpen;

	// Token: 0x0400241E RID: 9246
	public string soundClose;

	// Token: 0x0400241F RID: 9247
	public Vector2i size;

	// Token: 0x04002420 RID: 9248
	public float openTime;

	// Token: 0x04002421 RID: 9249
	public int minCount;

	// Token: 0x04002422 RID: 9250
	public int maxCount;

	// Token: 0x04002423 RID: 9251
	public LootContainer.DestroyOnClose destroyOnClose;

	// Token: 0x04002424 RID: 9252
	public string lootQualityTemplate;

	// Token: 0x04002425 RID: 9253
	public List<string> BuffActions;

	// Token: 0x04002426 RID: 9254
	public string OnOpenEvent = "";

	// Token: 0x04002427 RID: 9255
	public bool ignoreLootAbundance;

	// Token: 0x04002428 RID: 9256
	public bool useUnmodifiedLootstage;

	// Token: 0x04002429 RID: 9257
	public bool UniqueItems;

	// Token: 0x0400242A RID: 9258
	public bool IgnoreLootProb;

	// Token: 0x0400242B RID: 9259
	public readonly List<LootContainer.LootEntry> itemsToSpawn = new List<LootContainer.LootEntry>();

	// Token: 0x0400242C RID: 9260
	public static Dictionary<EntityPlayer, string[]> OverrideItems = new Dictionary<EntityPlayer, string[]>();

	// Token: 0x02000582 RID: 1410
	public enum DestroyOnClose
	{
		// Token: 0x0400242E RID: 9262
		False,
		// Token: 0x0400242F RID: 9263
		True,
		// Token: 0x04002430 RID: 9264
		Empty
	}

	// Token: 0x02000583 RID: 1411
	public class LootItem
	{
		// Token: 0x04002431 RID: 9265
		public ItemValue itemValue;
	}

	// Token: 0x02000584 RID: 1412
	public class LootGroup
	{
		// Token: 0x04002432 RID: 9266
		public string name;

		// Token: 0x04002433 RID: 9267
		public string lootQualityTemplate;

		// Token: 0x04002434 RID: 9268
		public int minCount;

		// Token: 0x04002435 RID: 9269
		public int maxCount;

		// Token: 0x04002436 RID: 9270
		public int minQuality = -1;

		// Token: 0x04002437 RID: 9271
		public int maxQuality = -1;

		// Token: 0x04002438 RID: 9272
		public float minLevel;

		// Token: 0x04002439 RID: 9273
		public float maxLevel;

		// Token: 0x0400243A RID: 9274
		public string[] modsToInstall;

		// Token: 0x0400243B RID: 9275
		public float modChance = 1f;

		// Token: 0x0400243C RID: 9276
		public readonly List<LootContainer.LootEntry> items = new List<LootContainer.LootEntry>();
	}

	// Token: 0x02000585 RID: 1413
	public class LootEntry
	{
		// Token: 0x06002DB1 RID: 11697 RVA: 0x00130E70 File Offset: 0x0012F070
		public bool HasRequirements(EntityPlayer player)
		{
			if (this.Requirements != null)
			{
				for (int i = 0; i < this.Requirements.Count; i++)
				{
					if (!this.Requirements[i].CheckRequirement(player))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x0400243D RID: 9277
		public string lootProbTemplate;

		// Token: 0x0400243E RID: 9278
		public int minCount;

		// Token: 0x0400243F RID: 9279
		public int maxCount;

		// Token: 0x04002440 RID: 9280
		public int minQuality;

		// Token: 0x04002441 RID: 9281
		public int maxQuality;

		// Token: 0x04002442 RID: 9282
		public float minLevel;

		// Token: 0x04002443 RID: 9283
		public float maxLevel;

		// Token: 0x04002444 RID: 9284
		public float prob;

		// Token: 0x04002445 RID: 9285
		public bool forceProb;

		// Token: 0x04002446 RID: 9286
		public string[] modsToInstall;

		// Token: 0x04002447 RID: 9287
		public float modChance = 1f;

		// Token: 0x04002448 RID: 9288
		public float lootstageCountMod;

		// Token: 0x04002449 RID: 9289
		public LootContainer.LootItem item;

		// Token: 0x0400244A RID: 9290
		public LootContainer.LootGroup group;

		// Token: 0x0400244B RID: 9291
		public LootContainer.LootGroup parentGroup;

		// Token: 0x0400244C RID: 9292
		public FastTags<TagGroup.Global> tags;

		// Token: 0x0400244D RID: 9293
		public string[] buffsToAdd;

		// Token: 0x0400244E RID: 9294
		public bool randomDurability = true;

		// Token: 0x0400244F RID: 9295
		public List<BaseLootEntryRequirement> Requirements;
	}

	// Token: 0x02000586 RID: 1414
	public class LootProbabilityTemplate
	{
		// Token: 0x04002450 RID: 9296
		public string name;

		// Token: 0x04002451 RID: 9297
		public readonly List<LootContainer.LootEntry> templates = new List<LootContainer.LootEntry>();
	}

	// Token: 0x02000587 RID: 1415
	public class LootQualityTemplate
	{
		// Token: 0x04002452 RID: 9298
		public string name;

		// Token: 0x04002453 RID: 9299
		public readonly List<LootContainer.LootGroup> templates = new List<LootContainer.LootGroup>();
	}
}
