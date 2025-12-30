using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007EC RID: 2028
public class TraderInfo
{
	// Token: 0x170005E9 RID: 1513
	// (get) Token: 0x06003A51 RID: 14929 RVA: 0x00177724 File Offset: 0x00175924
	// (set) Token: 0x06003A52 RID: 14930 RVA: 0x0017772B File Offset: 0x0017592B
	public static float BuyMarkup
	{
		get
		{
			return TraderInfo.buyMarkup;
		}
		set
		{
			TraderInfo.buyMarkup = value;
		}
	}

	// Token: 0x170005EA RID: 1514
	// (get) Token: 0x06003A53 RID: 14931 RVA: 0x00177733 File Offset: 0x00175933
	// (set) Token: 0x06003A54 RID: 14932 RVA: 0x0017773A File Offset: 0x0017593A
	public static float SellMarkdown
	{
		get
		{
			return TraderInfo.sellMarkdown;
		}
		set
		{
			TraderInfo.sellMarkdown = value;
		}
	}

	// Token: 0x170005EB RID: 1515
	// (get) Token: 0x06003A55 RID: 14933 RVA: 0x00177742 File Offset: 0x00175942
	// (set) Token: 0x06003A56 RID: 14934 RVA: 0x00177749 File Offset: 0x00175949
	public static float QualityMinMod
	{
		get
		{
			return TraderInfo.qualityMinMod;
		}
		set
		{
			TraderInfo.qualityMinMod = value;
		}
	}

	// Token: 0x170005EC RID: 1516
	// (get) Token: 0x06003A57 RID: 14935 RVA: 0x00177751 File Offset: 0x00175951
	// (set) Token: 0x06003A58 RID: 14936 RVA: 0x00177758 File Offset: 0x00175958
	public static float QualityMaxMod
	{
		get
		{
			return TraderInfo.qualityMaxMod;
		}
		set
		{
			TraderInfo.qualityMaxMod = value;
		}
	}

	// Token: 0x170005ED RID: 1517
	// (get) Token: 0x06003A59 RID: 14937 RVA: 0x00177760 File Offset: 0x00175960
	// (set) Token: 0x06003A5A RID: 14938 RVA: 0x00177767 File Offset: 0x00175967
	public static string CurrencyItem { get; set; }

	// Token: 0x170005EE RID: 1518
	// (get) Token: 0x06003A5B RID: 14939 RVA: 0x0017776F File Offset: 0x0017596F
	public int RentTimeInSeconds
	{
		get
		{
			return this.RentTimeInDays * 60 * GamePrefs.GetInt(EnumGamePrefs.DayNightLength);
		}
	}

	// Token: 0x170005EF RID: 1519
	// (get) Token: 0x06003A5C RID: 14940 RVA: 0x00177782 File Offset: 0x00175982
	public int RentTimeInTicks
	{
		get
		{
			return this.RentTimeInDays * 24000;
		}
	}

	// Token: 0x170005F0 RID: 1520
	// (get) Token: 0x06003A5D RID: 14941 RVA: 0x00177790 File Offset: 0x00175990
	public bool IsOpen
	{
		get
		{
			if (!this.UseOpenHours)
			{
				return true;
			}
			ulong num = GameManager.Instance.World.worldTime % 24000UL;
			if (this.OpenTime < this.CloseTime)
			{
				return this.OpenTime < num && num < this.CloseTime;
			}
			return num > this.OpenTime || num < this.CloseTime;
		}
	}

	// Token: 0x170005F1 RID: 1521
	// (get) Token: 0x06003A5E RID: 14942 RVA: 0x001777F8 File Offset: 0x001759F8
	public bool ShouldPlayOpenSound
	{
		get
		{
			ulong num = GameManager.Instance.World.worldTime % 24000UL;
			return num > this.OpenTime && num < this.OpenTime + 100UL;
		}
	}

	// Token: 0x170005F2 RID: 1522
	// (get) Token: 0x06003A5F RID: 14943 RVA: 0x00177834 File Offset: 0x00175A34
	public bool ShouldPlayCloseSound
	{
		get
		{
			ulong num = GameManager.Instance.World.worldTime % 24000UL;
			return num > this.CloseTime && num < this.CloseTime + 100UL;
		}
	}

	// Token: 0x170005F3 RID: 1523
	// (get) Token: 0x06003A60 RID: 14944 RVA: 0x00177870 File Offset: 0x00175A70
	public bool IsWarningTime
	{
		get
		{
			if (!this.UseOpenHours)
			{
				return false;
			}
			ulong num = GameManager.Instance.World.worldTime % 24000UL;
			if (this.OpenTime < this.WarningTime)
			{
				return this.WarningTime < num && num < this.WarningTime + 100UL;
			}
			return this.WarningTime > this.OpenTime || num < this.WarningTime + 100UL;
		}
	}

	// Token: 0x06003A62 RID: 14946 RVA: 0x0017794B File Offset: 0x00175B4B
	public static void InitStatic()
	{
		TraderInfo.traderInfoList = new TraderInfo[256];
		TraderInfo.traderItemGroups = new Dictionary<string, TraderInfo.TraderItemGroup>();
	}

	// Token: 0x06003A63 RID: 14947 RVA: 0x00177966 File Offset: 0x00175B66
	public void Init()
	{
		TraderInfo.traderInfoList[this.Id] = this;
	}

	// Token: 0x06003A64 RID: 14948 RVA: 0x00177975 File Offset: 0x00175B75
	public static void Cleanup()
	{
		TraderInfo.traderInfoList = null;
		TraderInfo.traderItemGroups = null;
	}

	// Token: 0x06003A65 RID: 14949 RVA: 0x00177984 File Offset: 0x00175B84
	[PublicizedFrom(EAccessModifier.Private)]
	public void applyRandomDegradation(ref ItemValue _itemValue)
	{
		_itemValue.Meta = ItemClass.GetForId(_itemValue.type).GetInitialMetadata(_itemValue);
		int maxUseTimes = _itemValue.MaxUseTimes;
		if (maxUseTimes == 0)
		{
			return;
		}
		float num = GameManager.Instance.World.GetGameRandom().RandomFloat * 0.6f + 0.2f;
		_itemValue.UseTimes = (float)((int)((float)maxUseTimes * num));
	}

	// Token: 0x06003A66 RID: 14950 RVA: 0x001779E8 File Offset: 0x00175BE8
	[PublicizedFrom(EAccessModifier.Private)]
	public void applyQuality(ref ItemValue _itemValue, int minQuality = 1, int maxQuality = 6)
	{
		if (ItemClass.list[_itemValue.type].HasQuality || ItemClass.list[_itemValue.type].HasSubItems)
		{
			_itemValue = new ItemValue(_itemValue.type, Mathf.Clamp(minQuality, 1, maxQuality), maxQuality, false, null, 1f);
		}
	}

	// Token: 0x06003A67 RID: 14951 RVA: 0x00177A3C File Offset: 0x00175C3C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpawnItem(TraderInfo.TraderItemEntry template, ItemValue item, int countToSpawn, List<ItemStack> spawnedItems)
	{
		if (countToSpawn < 1)
		{
			return;
		}
		if (item.ItemClass == null)
		{
			return;
		}
		ItemClass itemClass = item.ItemClass;
		countToSpawn = Math.Min(countToSpawn, itemClass.Stacknumber.Value);
		int num = itemClass.IsBlock() ? Block.list[item.type].EconomicBundleSize : itemClass.EconomicBundleSize;
		if (itemClass.EconomicValue == -1f)
		{
			return;
		}
		if (num > 1)
		{
			int num2 = countToSpawn % num;
			if (num2 > 0)
			{
				countToSpawn -= num2;
			}
			if (countToSpawn == 0)
			{
				countToSpawn = num;
			}
		}
		if (itemClass.CanStack())
		{
			int value = ItemClass.GetForId(item.type).Stacknumber.Value;
			for (int i = 0; i < spawnedItems.Count; i++)
			{
				ItemStack itemStack = spawnedItems[i];
				if (itemStack.itemValue.type == item.type)
				{
					if (itemStack.CanStack(countToSpawn))
					{
						itemStack.count += countToSpawn;
						spawnedItems[i] = itemStack;
						return;
					}
					int num3 = value - itemStack.count;
					itemStack.count = value;
					spawnedItems[i] = itemStack;
					countToSpawn -= num3;
				}
			}
		}
		int num4 = template.minQuality;
		int maxQuality = template.maxQuality;
		ItemValue itemValue = item.Clone();
		if (item.HasQuality)
		{
			if (num4 <= -1)
			{
				num4 = 1;
				maxQuality = 6;
			}
			if (template != null && template.parentGroup != null && template.parentGroup.modsToInstall.Length != 0)
			{
				itemValue = new ItemValue(item.type, num4, maxQuality, true, template.parentGroup.modsToInstall, template.parentGroup.modChance);
			}
			else
			{
				itemValue = new ItemValue(item.type, num4, maxQuality, true, template.modsToInstall, template.modChance);
			}
		}
		else
		{
			itemValue = new ItemValue(item.type, true);
		}
		if (itemValue.ItemClass != null && itemValue.ItemClass.Actions != null && itemValue.ItemClass.Actions.Length != 0 && itemValue.ItemClass.Actions[0] != null)
		{
			itemValue.Meta = 0;
		}
		ItemStack item2 = new ItemStack(itemValue, countToSpawn);
		spawnedItems.Add(item2);
	}

	// Token: 0x06003A68 RID: 14952 RVA: 0x00177C44 File Offset: 0x00175E44
	[PublicizedFrom(EAccessModifier.Private)]
	public int RandomSpawnCount(GameRandom random, int min, int max)
	{
		if (min < 0)
		{
			return -1;
		}
		float num = random.RandomRange((float)min - 0.49f, (float)max + 0.49f);
		if (num <= (float)min)
		{
			return min;
		}
		if (num > (float)max)
		{
			num = (float)max;
		}
		int num2 = (int)num;
		float num3 = num - (float)num2;
		if (random.RandomFloat < num3)
		{
			num2++;
		}
		return num2;
	}

	// Token: 0x06003A69 RID: 14953 RVA: 0x00177C94 File Offset: 0x00175E94
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpawnAllItemsFromList(GameRandom random, List<TraderInfo.TraderItemEntry> itemSet, List<ItemStack> spawnedItems)
	{
		for (int i = 0; i < itemSet.Count; i++)
		{
			TraderInfo.TraderItemEntry traderItemEntry = itemSet[i];
			int num = this.RandomSpawnCount(random, traderItemEntry.minCount, traderItemEntry.maxCount);
			if (traderItemEntry.group != null)
			{
				this.SpawnItemsFromGroup(random, traderItemEntry.group, num, spawnedItems, traderItemEntry.uniqueOnly);
			}
			else
			{
				this.SpawnItem(traderItemEntry, traderItemEntry.item.itemValue, num, spawnedItems);
			}
		}
	}

	// Token: 0x06003A6A RID: 14954 RVA: 0x00177D04 File Offset: 0x00175F04
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpawnItemsFromGroup(GameRandom random, TraderInfo.TraderItemGroup group, int numToSpawn, List<ItemStack> spawnedItems, bool uniqueOnly)
	{
		List<int> usedIndices = null;
		if (group.uniqueOnly || uniqueOnly)
		{
			usedIndices = new List<int>();
		}
		for (int i = 0; i < numToSpawn; i++)
		{
			int numToSpawn2 = this.RandomSpawnCount(random, group.minCount, group.maxCount);
			this.SpawnLootItemsFromList(random, group.items, numToSpawn2, spawnedItems, usedIndices);
		}
	}

	// Token: 0x06003A6B RID: 14955 RVA: 0x00177D58 File Offset: 0x00175F58
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpawnLootItemsFromList(GameRandom random, List<TraderInfo.TraderItemEntry> itemSet, int numToSpawn, List<ItemStack> spawnedItems, List<int> usedIndices)
	{
		if (numToSpawn < 1)
		{
			if (numToSpawn == -1)
			{
				this.SpawnAllItemsFromList(random, itemSet, spawnedItems);
			}
			return;
		}
		float num = 0f;
		for (int i = 0; i < itemSet.Count; i++)
		{
			TraderInfo.TraderItemEntry traderItemEntry = itemSet[i];
			if (usedIndices == null || !usedIndices.Contains(i))
			{
				num += traderItemEntry.prob;
			}
		}
		if (num == 0f)
		{
			return;
		}
		for (int j = 0; j < numToSpawn; j++)
		{
			float num2 = 0f;
			float randomFloat = random.RandomFloat;
			for (int k = 0; k < itemSet.Count; k++)
			{
				TraderInfo.TraderItemEntry traderItemEntry2 = itemSet[k];
				if (usedIndices == null || !usedIndices.Contains(k))
				{
					num2 += traderItemEntry2.prob / num;
					if (randomFloat <= num2)
					{
						int num3 = this.RandomSpawnCount(random, traderItemEntry2.minCount, traderItemEntry2.maxCount);
						if (usedIndices != null)
						{
							usedIndices.Add(k);
						}
						if (traderItemEntry2.group != null)
						{
							this.SpawnItemsFromGroup(random, traderItemEntry2.group, num3, spawnedItems, traderItemEntry2.uniqueOnly);
							break;
						}
						this.SpawnItem(traderItemEntry2, traderItemEntry2.item.itemValue, num3, spawnedItems);
						break;
					}
				}
			}
		}
	}

	// Token: 0x06003A6C RID: 14956 RVA: 0x00177E84 File Offset: 0x00176084
	public List<ItemStack> Spawn(GameRandom random)
	{
		List<ItemStack> list = new List<ItemStack>();
		this.SpawnLootItemsFromList(random, this.traderItems, -1, list, null);
		return list;
	}

	// Token: 0x06003A6D RID: 14957 RVA: 0x00177EA8 File Offset: 0x001760A8
	public List<ItemStack> SpawnTierGroup(GameRandom random, int tierGroupIndex)
	{
		List<ItemStack> list = new List<ItemStack>();
		this.Shuffle<TraderInfo.TraderItemEntry>((int)DateTime.Now.Ticks, ref this.TierItemGroups[tierGroupIndex].traderItems);
		int numToSpawn = this.RandomSpawnCount(random, this.minCount, this.maxCount);
		this.SpawnLootItemsFromList(random, this.TierItemGroups[tierGroupIndex].traderItems, numToSpawn, list, null);
		return list;
	}

	// Token: 0x06003A6E RID: 14958 RVA: 0x00177F10 File Offset: 0x00176110
	[PublicizedFrom(EAccessModifier.Private)]
	public void Shuffle<T>(int seed, ref List<T> list)
	{
		int i = list.Count;
		GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(seed);
		while (i > 1)
		{
			i--;
			int index = gameRandom.RandomRange(0, i) % i;
			T value = list[index];
			list[index] = list[i];
			list[i] = value;
		}
		GameRandomManager.Instance.FreeGameRandom(gameRandom);
	}

	// Token: 0x04002F28 RID: 12072
	public static TraderInfo[] traderInfoList;

	// Token: 0x04002F29 RID: 12073
	public static Dictionary<string, TraderInfo.TraderItemGroup> traderItemGroups;

	// Token: 0x04002F2A RID: 12074
	[PublicizedFrom(EAccessModifier.Private)]
	public static float buyMarkup;

	// Token: 0x04002F2B RID: 12075
	[PublicizedFrom(EAccessModifier.Private)]
	public static float sellMarkdown;

	// Token: 0x04002F2C RID: 12076
	[PublicizedFrom(EAccessModifier.Private)]
	public static float qualityMinMod;

	// Token: 0x04002F2D RID: 12077
	[PublicizedFrom(EAccessModifier.Private)]
	public static float qualityMaxMod;

	// Token: 0x04002F2F RID: 12079
	public int Id;

	// Token: 0x04002F30 RID: 12080
	public float SalesMarkup;

	// Token: 0x04002F31 RID: 12081
	public int ResetInterval = 1;

	// Token: 0x04002F32 RID: 12082
	public int ResetIntervalInTicks = 24000;

	// Token: 0x04002F33 RID: 12083
	public int MaxItems = 50;

	// Token: 0x04002F34 RID: 12084
	public int minCount;

	// Token: 0x04002F35 RID: 12085
	public int maxCount;

	// Token: 0x04002F36 RID: 12086
	public bool AllowBuy = true;

	// Token: 0x04002F37 RID: 12087
	public bool AllowSell = true;

	// Token: 0x04002F38 RID: 12088
	public float OverrideBuyMarkup = -1f;

	// Token: 0x04002F39 RID: 12089
	public float OverrideSellMarkdown = -1f;

	// Token: 0x04002F3A RID: 12090
	public bool UseOpenHours;

	// Token: 0x04002F3B RID: 12091
	public ulong OpenTime;

	// Token: 0x04002F3C RID: 12092
	public ulong CloseTime;

	// Token: 0x04002F3D RID: 12093
	public ulong WarningTime;

	// Token: 0x04002F3E RID: 12094
	public bool PlayerOwned;

	// Token: 0x04002F3F RID: 12095
	public bool Rentable;

	// Token: 0x04002F40 RID: 12096
	public int RentCost;

	// Token: 0x04002F41 RID: 12097
	public int RentTimeInDays;

	// Token: 0x04002F42 RID: 12098
	public List<TraderInfo.TierItemGroup> TierItemGroups = new List<TraderInfo.TierItemGroup>();

	// Token: 0x04002F43 RID: 12099
	public List<TraderInfo.TraderItemEntry> traderItems = new List<TraderInfo.TraderItemEntry>();

	// Token: 0x020007ED RID: 2029
	public class TraderItem
	{
		// Token: 0x04002F44 RID: 12100
		public ItemValue itemValue;
	}

	// Token: 0x020007EE RID: 2030
	public class TraderItemGroup
	{
		// Token: 0x04002F45 RID: 12101
		public string name;

		// Token: 0x04002F46 RID: 12102
		public int minCount;

		// Token: 0x04002F47 RID: 12103
		public int maxCount;

		// Token: 0x04002F48 RID: 12104
		public int minQuality = -1;

		// Token: 0x04002F49 RID: 12105
		public int maxQuality = -1;

		// Token: 0x04002F4A RID: 12106
		public string[] modsToInstall;

		// Token: 0x04002F4B RID: 12107
		public float modChance = 1f;

		// Token: 0x04002F4C RID: 12108
		public bool uniqueOnly;

		// Token: 0x04002F4D RID: 12109
		public List<TraderInfo.TraderItemEntry> items = new List<TraderInfo.TraderItemEntry>();
	}

	// Token: 0x020007EF RID: 2031
	public class TraderItemEntry
	{
		// Token: 0x04002F4E RID: 12110
		public int minCount;

		// Token: 0x04002F4F RID: 12111
		public int maxCount;

		// Token: 0x04002F50 RID: 12112
		public int minQuality;

		// Token: 0x04002F51 RID: 12113
		public int maxQuality;

		// Token: 0x04002F52 RID: 12114
		public float prob;

		// Token: 0x04002F53 RID: 12115
		public string[] modsToInstall;

		// Token: 0x04002F54 RID: 12116
		public float modChance = 1f;

		// Token: 0x04002F55 RID: 12117
		public bool uniqueOnly;

		// Token: 0x04002F56 RID: 12118
		public TraderInfo.TraderItem item;

		// Token: 0x04002F57 RID: 12119
		public TraderInfo.TraderItemGroup group;

		// Token: 0x04002F58 RID: 12120
		public TraderInfo.TraderItemGroup parentGroup;
	}

	// Token: 0x020007F0 RID: 2032
	public class TierItemGroup
	{
		// Token: 0x04002F59 RID: 12121
		public int minLevel;

		// Token: 0x04002F5A RID: 12122
		public int maxLevel;

		// Token: 0x04002F5B RID: 12123
		public int minCount;

		// Token: 0x04002F5C RID: 12124
		public int maxCount;

		// Token: 0x04002F5D RID: 12125
		public List<TraderInfo.TraderItemEntry> traderItems = new List<TraderInfo.TraderItemEntry>();
	}
}
