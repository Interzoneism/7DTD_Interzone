using System;
using System.Collections.Generic;

// Token: 0x02000ECB RID: 3787
public class UIDisplayInfoManager
{
	// Token: 0x17000C24 RID: 3108
	// (get) Token: 0x060077C3 RID: 30659 RVA: 0x0030C32B File Offset: 0x0030A52B
	public static UIDisplayInfoManager Current
	{
		get
		{
			if (UIDisplayInfoManager.instance == null)
			{
				UIDisplayInfoManager.instance = new UIDisplayInfoManager();
			}
			return UIDisplayInfoManager.instance;
		}
	}

	// Token: 0x060077C4 RID: 30660 RVA: 0x0030C343 File Offset: 0x0030A543
	[PublicizedFrom(EAccessModifier.Private)]
	public UIDisplayInfoManager()
	{
	}

	// Token: 0x17000C25 RID: 3109
	// (get) Token: 0x060077C5 RID: 30661 RVA: 0x0030C382 File Offset: 0x0030A582
	public static bool HasInstance
	{
		get
		{
			return UIDisplayInfoManager.instance != null;
		}
	}

	// Token: 0x060077C6 RID: 30662 RVA: 0x0030C38C File Offset: 0x0030A58C
	public static void Reset()
	{
		if (UIDisplayInfoManager.HasInstance)
		{
			UIDisplayInfoManager.Current.Cleanup();
		}
	}

	// Token: 0x060077C7 RID: 30663 RVA: 0x0030C39F File Offset: 0x0030A59F
	public bool ContainsItemDisplayStats(string tag)
	{
		return this.ItemDisplayStats.ContainsKey(tag);
	}

	// Token: 0x060077C8 RID: 30664 RVA: 0x0030C3AD File Offset: 0x0030A5AD
	public bool ContainsCraftingCategoryList(string tag)
	{
		return this.CraftingCategoryDisplayLists.ContainsKey(tag);
	}

	// Token: 0x060077C9 RID: 30665 RVA: 0x0030C3BB File Offset: 0x0030A5BB
	public ItemDisplayEntry GetDisplayStatsForTag(string tag)
	{
		if (this.ItemDisplayStats.ContainsKey(tag))
		{
			return this.ItemDisplayStats[tag];
		}
		return null;
	}

	// Token: 0x060077CA RID: 30666 RVA: 0x0030C3D9 File Offset: 0x0030A5D9
	public void AddItemDisplayStats(string tag, string group)
	{
		if (!this.ItemDisplayStats.ContainsKey(tag))
		{
			this.ItemDisplayStats.Add(tag, new ItemDisplayEntry
			{
				DisplayGroup = group
			});
		}
	}

	// Token: 0x060077CB RID: 30667 RVA: 0x0030C404 File Offset: 0x0030A604
	public void AddItemDisplayInfo(string tag, DisplayInfoEntry displayInfo)
	{
		this.ItemDisplayStats[tag].DisplayStats.Add(displayInfo);
		if (!this.StatLocalizationDictionary.ContainsKey(displayInfo.StatType))
		{
			this.StatLocalizationDictionary.Add(displayInfo.StatType, Localization.Get(displayInfo.StatType.ToStringCached<PassiveEffects>(), false));
		}
	}

	// Token: 0x060077CC RID: 30668 RVA: 0x0030C45D File Offset: 0x0030A65D
	public void Cleanup()
	{
		this.ItemDisplayStats.Clear();
		this.CharacterDisplayStats.Clear();
		this.CraftingCategoryDisplayLists.Clear();
	}

	// Token: 0x060077CD RID: 30669 RVA: 0x0030C480 File Offset: 0x0030A680
	public string GetLocalizedName(PassiveEffects statType)
	{
		if (this.StatLocalizationDictionary.ContainsKey(statType))
		{
			return this.StatLocalizationDictionary[statType];
		}
		return "";
	}

	// Token: 0x060077CE RID: 30670 RVA: 0x0030C4A4 File Offset: 0x0030A6A4
	public void AddCharacterDisplayInfo(DisplayInfoEntry displayInfo)
	{
		this.CharacterDisplayStats.Add(displayInfo);
		if (!this.StatLocalizationDictionary.ContainsKey(displayInfo.StatType))
		{
			this.StatLocalizationDictionary.Add(displayInfo.StatType, Localization.Get(displayInfo.StatType.ToStringCached<PassiveEffects>(), false));
		}
	}

	// Token: 0x060077CF RID: 30671 RVA: 0x0030C4F2 File Offset: 0x0030A6F2
	public List<DisplayInfoEntry> GetCharacterDisplayInfo()
	{
		return this.CharacterDisplayStats;
	}

	// Token: 0x060077D0 RID: 30672 RVA: 0x0030C4FA File Offset: 0x0030A6FA
	public void AddCraftingCategoryDisplayItem(string categoryListName, CraftingCategoryDisplayEntry entry)
	{
		if (!this.CraftingCategoryDisplayLists.ContainsKey(categoryListName))
		{
			this.CraftingCategoryDisplayLists.Add(categoryListName, new List<CraftingCategoryDisplayEntry>());
		}
		this.CraftingCategoryDisplayLists[categoryListName].Add(entry);
	}

	// Token: 0x060077D1 RID: 30673 RVA: 0x0030C52D File Offset: 0x0030A72D
	public List<CraftingCategoryDisplayEntry> GetCraftingCategoryDisplayList(string categoryListName)
	{
		if (this.CraftingCategoryDisplayLists.ContainsKey(categoryListName))
		{
			return this.CraftingCategoryDisplayLists[categoryListName];
		}
		return null;
	}

	// Token: 0x060077D2 RID: 30674 RVA: 0x0030C54B File Offset: 0x0030A74B
	public void AddTraderCategoryDIsplayItem(CraftingCategoryDisplayEntry entry)
	{
		if (!this.TraderCategoryDisplayDict.ContainsKey(entry.Name))
		{
			this.TraderCategoryDisplayDict.Add(entry.Name, entry);
		}
	}

	// Token: 0x060077D3 RID: 30675 RVA: 0x0030C572 File Offset: 0x0030A772
	public CraftingCategoryDisplayEntry GetTraderCategoryDisplay(string name)
	{
		if (this.TraderCategoryDisplayDict.ContainsKey(name))
		{
			return this.TraderCategoryDisplayDict[name];
		}
		return null;
	}

	// Token: 0x04005B45 RID: 23365
	[PublicizedFrom(EAccessModifier.Private)]
	public static UIDisplayInfoManager instance;

	// Token: 0x04005B46 RID: 23366
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, ItemDisplayEntry> ItemDisplayStats = new Dictionary<string, ItemDisplayEntry>();

	// Token: 0x04005B47 RID: 23367
	[PublicizedFrom(EAccessModifier.Private)]
	public List<DisplayInfoEntry> CharacterDisplayStats = new List<DisplayInfoEntry>();

	// Token: 0x04005B48 RID: 23368
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, List<CraftingCategoryDisplayEntry>> CraftingCategoryDisplayLists = new Dictionary<string, List<CraftingCategoryDisplayEntry>>();

	// Token: 0x04005B49 RID: 23369
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, CraftingCategoryDisplayEntry> TraderCategoryDisplayDict = new Dictionary<string, CraftingCategoryDisplayEntry>();

	// Token: 0x04005B4A RID: 23370
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<PassiveEffects, string> StatLocalizationDictionary = new EnumDictionary<PassiveEffects, string>();
}
