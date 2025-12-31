using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200096B RID: 2411
public class ProgressionClass
{
	// Token: 0x170007A7 RID: 1959
	// (get) Token: 0x060048E2 RID: 18658 RVA: 0x001CD818 File Offset: 0x001CBA18
	public bool IsBookGroup
	{
		get
		{
			return this.Type == ProgressionType.BookGroup;
		}
	}

	// Token: 0x170007A8 RID: 1960
	// (get) Token: 0x060048E3 RID: 18659 RVA: 0x001CD823 File Offset: 0x001CBA23
	public bool IsBook
	{
		get
		{
			return this.Type == ProgressionType.Book;
		}
	}

	// Token: 0x060048E4 RID: 18660 RVA: 0x001CD830 File Offset: 0x001CBA30
	public bool ValidDisplay(ProgressionClass.DisplayTypes displayType)
	{
		switch (displayType)
		{
		case ProgressionClass.DisplayTypes.Standard:
			return this.Type != ProgressionType.BookGroup && this.Type != ProgressionType.Crafting;
		case ProgressionClass.DisplayTypes.Book:
			return this.Type == ProgressionType.BookGroup;
		case ProgressionClass.DisplayTypes.Crafting:
			return this.Type == ProgressionType.Crafting;
		default:
			return false;
		}
	}

	// Token: 0x170007A9 RID: 1961
	// (get) Token: 0x060048E5 RID: 18661 RVA: 0x001CD880 File Offset: 0x001CBA80
	public ProgressionCurrencyType CurrencyType
	{
		get
		{
			switch (this.Type)
			{
			case ProgressionType.Attribute:
				return ProgressionCurrencyType.SP;
			case ProgressionType.Skill:
				return ProgressionCurrencyType.XP;
			case ProgressionType.Perk:
				return ProgressionCurrencyType.SP;
			default:
				return ProgressionCurrencyType.None;
			}
		}
	}

	// Token: 0x170007AA RID: 1962
	// (get) Token: 0x060048E6 RID: 18662 RVA: 0x001CD8B4 File Offset: 0x001CBAB4
	public ProgressionClass Parent
	{
		get
		{
			if (this.ParentName == null)
			{
				return this;
			}
			ProgressionClass result;
			if (Progression.ProgressionClasses.TryGetValue(this.ParentName, out result))
			{
				return result;
			}
			return null;
		}
	}

	// Token: 0x170007AB RID: 1963
	// (get) Token: 0x060048E7 RID: 18663 RVA: 0x001CD8E2 File Offset: 0x001CBAE2
	public bool IsPerk
	{
		get
		{
			return this.Type == ProgressionType.Perk;
		}
	}

	// Token: 0x170007AC RID: 1964
	// (get) Token: 0x060048E8 RID: 18664 RVA: 0x001CD8ED File Offset: 0x001CBAED
	public bool IsSkill
	{
		get
		{
			return this.Type == ProgressionType.Skill;
		}
	}

	// Token: 0x170007AD RID: 1965
	// (get) Token: 0x060048E9 RID: 18665 RVA: 0x001CD8F8 File Offset: 0x001CBAF8
	public bool IsAttribute
	{
		get
		{
			return this.Type == ProgressionType.Attribute;
		}
	}

	// Token: 0x170007AE RID: 1966
	// (get) Token: 0x060048EA RID: 18666 RVA: 0x001CD903 File Offset: 0x001CBB03
	public bool IsCrafting
	{
		get
		{
			return this.Type == ProgressionType.Crafting;
		}
	}

	// Token: 0x170007AF RID: 1967
	// (get) Token: 0x060048EB RID: 18667 RVA: 0x001CD910 File Offset: 0x001CBB10
	// (set) Token: 0x060048EC RID: 18668 RVA: 0x001CD965 File Offset: 0x001CBB65
	public float ListSortOrder
	{
		get
		{
			if (this.IsPerk)
			{
				return this.Parent.ListSortOrder + this.listSortOrder * 0.001f;
			}
			if (this.IsSkill)
			{
				return this.Parent.ListSortOrder + this.listSortOrder;
			}
			return this.listSortOrder * 100f;
		}
		set
		{
			this.listSortOrder = value;
		}
	}

	// Token: 0x060048ED RID: 18669 RVA: 0x001CD970 File Offset: 0x001CBB70
	public ProgressionClass.DisplayData AddDisplayData(string _item, int[] _qualityStarts, string[] _customIcon, string[] _customIconTint, string[] _customName, bool _customHasQuality)
	{
		if (this.DisplayDataList == null)
		{
			this.DisplayDataList = new List<ProgressionClass.DisplayData>();
		}
		ProgressionClass.DisplayData displayData = new ProgressionClass.DisplayData
		{
			ItemName = _item,
			QualityStarts = _qualityStarts,
			Owner = this,
			CustomIcon = _customIcon,
			CustomIconTint = _customIconTint,
			CustomName = _customName,
			CustomHasQuality = _customHasQuality
		};
		this.DisplayDataList.Add(displayData);
		return displayData;
	}

	// Token: 0x060048EE RID: 18670 RVA: 0x001CD9D8 File Offset: 0x001CBBD8
	public ProgressionClass(string _name)
	{
		this.Name = _name;
		this.NameKey = this.Name;
		this.NameTag = FastTags<TagGroup.Global>.GetTag(_name);
		this.DescKey = "";
		this.ListSortOrder = float.MaxValue;
		this.ParentName = null;
		this.Type = ProgressionType.None;
	}

	// Token: 0x060048EF RID: 18671 RVA: 0x001CDA4F File Offset: 0x001CBC4F
	public void ModifyValue(EntityAlive _ea, ProgressionValue _pv, PassiveEffects _effect, ref float _base_value, ref float _perc_value, FastTags<TagGroup.Global> _tags = default(FastTags<TagGroup.Global>))
	{
		if (this.Effects == null)
		{
			return;
		}
		this.Effects.ModifyValue(_ea, _effect, ref _base_value, ref _perc_value, _pv.GetCalculatedLevel(_ea), _tags, 1);
	}

	// Token: 0x060048F0 RID: 18672 RVA: 0x001CDA78 File Offset: 0x001CBC78
	public void GetModifiedValueData(List<EffectManager.ModifierValuesAndSources> _modValueSources, EffectManager.ModifierValuesAndSources.ValueSourceType _sourceType, EntityAlive _ea, ProgressionValue _pv, PassiveEffects _effect, ref float _base_value, ref float _perc_value, FastTags<TagGroup.Global> _tags = default(FastTags<TagGroup.Global>))
	{
		if (this.Effects == null)
		{
			return;
		}
		this.Effects.GetModifiedValueData(_modValueSources, _sourceType, _ea, _effect, ref _base_value, ref _perc_value, _pv.GetCalculatedLevel(_ea), _tags, 1);
	}

	// Token: 0x060048F1 RID: 18673 RVA: 0x001CDAAD File Offset: 0x001CBCAD
	public bool HasEvents()
	{
		return this.Effects != null && this.Effects.HasEvents();
	}

	// Token: 0x060048F2 RID: 18674 RVA: 0x001CDAC4 File Offset: 0x001CBCC4
	public void FireEvent(MinEventTypes _eventType, MinEventParams _params)
	{
		if (this.Effects != null)
		{
			this.Effects.FireEvent(_eventType, _params);
		}
	}

	// Token: 0x060048F3 RID: 18675 RVA: 0x001CDADC File Offset: 0x001CBCDC
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool canRun(List<IRequirement> Requirements, MinEventParams _params)
	{
		if (Requirements != null)
		{
			int count = Requirements.Count;
			for (int i = 0; i < count; i++)
			{
				if (!Requirements[i].IsValid(_params))
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x060048F4 RID: 18676 RVA: 0x001CDB14 File Offset: 0x001CBD14
	public LevelRequirement GetRequirementsForLevel(int _level)
	{
		if (this.LevelRequirements.Count <= 0)
		{
			return new LevelRequirement(_level);
		}
		LevelRequirement result;
		if (this.LevelRequirements.dict.TryGetValue(_level, out result))
		{
			return result;
		}
		return new LevelRequirement(_level);
	}

	// Token: 0x060048F5 RID: 18677 RVA: 0x001CDB53 File Offset: 0x001CBD53
	public void AddLevelRequirement(LevelRequirement _lr)
	{
		this.LevelRequirements.Add(_lr.Level, _lr);
	}

	// Token: 0x060048F6 RID: 18678 RVA: 0x001CDB67 File Offset: 0x001CBD67
	public void PostInit()
	{
		this.LevelRequirements.list.Sort((LevelRequirement a, LevelRequirement b) => a.Level - b.Level);
	}

	// Token: 0x060048F7 RID: 18679 RVA: 0x001CDB98 File Offset: 0x001CBD98
	public static int GetCalculatedMaxLevel(EntityAlive _ea, ProgressionValue _pv)
	{
		ProgressionClass progressionClass = _pv.ProgressionClass;
		int num = 0;
		if (progressionClass.LevelRequirements.Count > 0)
		{
			List<LevelRequirement> list = progressionClass.LevelRequirements.list;
			int i = 0;
			int num2 = list.Count;
			while (i < num2)
			{
				int num3 = (num2 + i) / 2;
				LevelRequirement levelRequirement = list[num3];
				if (ProgressionClass.canRun(levelRequirement.Requirements, _ea.MinEventContext))
				{
					num = levelRequirement.Level;
					i = num3 + 1;
				}
				else
				{
					num2 = num3;
				}
			}
			if (!progressionClass.IsAttribute)
			{
				if (num < progressionClass.MinLevel)
				{
					num = progressionClass.MinLevel;
				}
				if (num > progressionClass.MaxLevel)
				{
					num = progressionClass.MaxLevel;
				}
			}
		}
		else if (progressionClass.IsAttribute)
		{
			num = 20;
		}
		else
		{
			num = progressionClass.MaxLevel;
		}
		return num;
	}

	// Token: 0x060048F8 RID: 18680 RVA: 0x001CDC54 File Offset: 0x001CBE54
	public int CalculatedCostForLevel(int _level)
	{
		if (this.OverrideCost == null)
		{
			return (int)(Mathf.Pow(this.CostMultiplier, (float)_level) * (float)this.BaseCostToLevel);
		}
		if (_level - 1 < this.OverrideCost.Length)
		{
			return this.OverrideCost[_level - 1];
		}
		return 0;
	}

	// Token: 0x060048F9 RID: 18681 RVA: 0x001CDC90 File Offset: 0x001CBE90
	public float GetPercentThisLevel(ProgressionValue _pv)
	{
		if (this.Type != ProgressionType.Skill)
		{
			return 0f;
		}
		if (_pv.Level == this.MaxLevel)
		{
			return 0f;
		}
		float num = (float)((int)(Mathf.Pow(this.CostMultiplier, (float)_pv.Level) * (float)this.BaseCostToLevel) - _pv.CostForNextLevel) / (Mathf.Pow(this.CostMultiplier, (float)_pv.Level) * (float)this.BaseCostToLevel);
		if (!float.IsNaN(num))
		{
			return num;
		}
		return 0f;
	}

	// Token: 0x060048FA RID: 18682 RVA: 0x001CDD10 File Offset: 0x001CBF10
	public void HandleCheckCrafting(EntityPlayerLocal _player, int _oldLevel, int _newLevel)
	{
		if (this.DisplayDataList != null)
		{
			for (int i = 0; i < this.DisplayDataList.Count; i++)
			{
				this.DisplayDataList[i].HandleCheckCrafting(_player, _oldLevel, _newLevel);
			}
		}
	}

	// Token: 0x060048FB RID: 18683 RVA: 0x001CDD4F File Offset: 0x001CBF4F
	public override string ToString()
	{
		return string.Format("{0}, {1}, lvl {2} to {3}", new object[]
		{
			base.ToString(),
			this.Name,
			this.MinLevel,
			this.MaxLevel
		});
	}

	// Token: 0x04003826 RID: 14374
	public readonly string Name;

	// Token: 0x04003827 RID: 14375
	public readonly FastTags<TagGroup.Global> NameTag;

	// Token: 0x04003828 RID: 14376
	public float ParentMaxLevelRatio = 1f;

	// Token: 0x04003829 RID: 14377
	public string NameKey;

	// Token: 0x0400382A RID: 14378
	public string DescKey;

	// Token: 0x0400382B RID: 14379
	public string LongDescKey;

	// Token: 0x0400382C RID: 14380
	public string Icon;

	// Token: 0x0400382D RID: 14381
	public int MinLevel;

	// Token: 0x0400382E RID: 14382
	public int MaxLevel;

	// Token: 0x0400382F RID: 14383
	public int BaseCostToLevel;

	// Token: 0x04003830 RID: 14384
	public float CostMultiplier;

	// Token: 0x04003831 RID: 14385
	public bool Hidden;

	// Token: 0x04003832 RID: 14386
	public int[] OverrideCost;

	// Token: 0x04003833 RID: 14387
	public ProgressionClass.DisplayTypes DisplayType;

	// Token: 0x04003834 RID: 14388
	public MinEffectController Effects;

	// Token: 0x04003835 RID: 14389
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly DictionaryList<int, LevelRequirement> LevelRequirements = new DictionaryList<int, LevelRequirement>();

	// Token: 0x04003836 RID: 14390
	public readonly List<ProgressionClass> Children = new List<ProgressionClass>();

	// Token: 0x04003837 RID: 14391
	public string ParentName;

	// Token: 0x04003838 RID: 14392
	public ProgressionType Type;

	// Token: 0x04003839 RID: 14393
	[PublicizedFrom(EAccessModifier.Private)]
	public float listSortOrder;

	// Token: 0x0400383A RID: 14394
	public List<ProgressionClass.DisplayData> DisplayDataList;

	// Token: 0x0200096C RID: 2412
	public enum DisplayTypes
	{
		// Token: 0x0400383C RID: 14396
		Standard,
		// Token: 0x0400383D RID: 14397
		Book,
		// Token: 0x0400383E RID: 14398
		Crafting
	}

	// Token: 0x0200096D RID: 2413
	public class ListSortOrderComparer : IComparer<ProgressionValue>
	{
		// Token: 0x060048FC RID: 18684 RVA: 0x0000A7E3 File Offset: 0x000089E3
		[PublicizedFrom(EAccessModifier.Private)]
		public ListSortOrderComparer()
		{
		}

		// Token: 0x060048FD RID: 18685 RVA: 0x001CDD90 File Offset: 0x001CBF90
		public int Compare(ProgressionValue _x, ProgressionValue _y)
		{
			return _x.ProgressionClass.ListSortOrder.CompareTo(_y.ProgressionClass.ListSortOrder);
		}

		// Token: 0x0400383F RID: 14399
		public static ProgressionClass.ListSortOrderComparer Instance = new ProgressionClass.ListSortOrderComparer();
	}

	// Token: 0x0200096E RID: 2414
	public class DisplayData
	{
		// Token: 0x170007B0 RID: 1968
		// (get) Token: 0x060048FF RID: 18687 RVA: 0x001CDDC7 File Offset: 0x001CBFC7
		public ItemClass Item
		{
			get
			{
				if (this.item == null)
				{
					this.item = ItemClass.GetItemClass(this.ItemName, false);
				}
				return this.item;
			}
		}

		// Token: 0x170007B1 RID: 1969
		// (get) Token: 0x06004900 RID: 18688 RVA: 0x001CDDE9 File Offset: 0x001CBFE9
		public bool HasQuality
		{
			get
			{
				if (this.ItemName != "")
				{
					return this.Item.HasQuality;
				}
				return this.CustomHasQuality;
			}
		}

		// Token: 0x06004901 RID: 18689 RVA: 0x001CDE10 File Offset: 0x001CC010
		public string GetName(int level)
		{
			if (this.ItemName != "")
			{
				return this.Item.GetLocalizedItemName();
			}
			if (this.CustomName == null)
			{
				return "";
			}
			int num = this.GetQualityLevel(level);
			if (num >= this.CustomName.Length)
			{
				num = 0;
			}
			return this.CustomName[num];
		}

		// Token: 0x06004902 RID: 18690 RVA: 0x001CDE68 File Offset: 0x001CC068
		public string GetIcon(int level)
		{
			if (this.ItemName != "")
			{
				return this.Item.GetIconName();
			}
			if (this.CustomIcon == null)
			{
				return "";
			}
			int num = this.GetQualityLevel(level);
			if (num >= this.CustomIcon.Length)
			{
				num = 0;
			}
			return this.CustomIcon[num];
		}

		// Token: 0x06004903 RID: 18691 RVA: 0x001CDEC0 File Offset: 0x001CC0C0
		public string GetIconTint(int level)
		{
			if (this.ItemName != "")
			{
				return Utils.ColorToHex(this.Item.GetIconTint(null));
			}
			if (this.CustomIconTint == null)
			{
				return "FFFFFF";
			}
			int num = this.GetQualityLevel(level);
			if (num >= this.CustomName.Length)
			{
				num = 0;
			}
			return this.CustomIconTint[num];
		}

		// Token: 0x06004904 RID: 18692 RVA: 0x001CDF24 File Offset: 0x001CC124
		public int GetQualityLevel(int level)
		{
			for (int i = 0; i < this.QualityStarts.Length; i++)
			{
				if (this.QualityStarts[i] > level)
				{
					return i;
				}
			}
			return this.QualityStarts.Length;
		}

		// Token: 0x06004905 RID: 18693 RVA: 0x001CDF5C File Offset: 0x001CC15C
		public int GetNextPoints(int level)
		{
			for (int i = 0; i < this.QualityStarts.Length; i++)
			{
				if (this.QualityStarts[i] > level)
				{
					return this.QualityStarts[i];
				}
			}
			return 0;
		}

		// Token: 0x06004906 RID: 18694 RVA: 0x001CDF94 File Offset: 0x001CC194
		public bool IsComplete(int level)
		{
			for (int i = 0; i < this.QualityStarts.Length; i++)
			{
				if (this.QualityStarts[i] > level)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004907 RID: 18695 RVA: 0x001CDFC2 File Offset: 0x001CC1C2
		public void AddUnlockData(string itemName, int unlockTier, string[] recipeList)
		{
			if (this.UnlockDataList == null)
			{
				this.UnlockDataList = new List<ProgressionClass.DisplayData.UnlockData>();
			}
			this.UnlockDataList.Add(new ProgressionClass.DisplayData.UnlockData
			{
				ItemName = itemName,
				UnlockTier = unlockTier,
				RecipeList = recipeList
			});
		}

		// Token: 0x06004908 RID: 18696 RVA: 0x001CDFFC File Offset: 0x001CC1FC
		public ItemClass GetUnlockItem(int index)
		{
			if (this.UnlockDataList == null)
			{
				return null;
			}
			if (index >= this.UnlockDataList.Count)
			{
				return null;
			}
			return this.UnlockDataList[index].Item;
		}

		// Token: 0x06004909 RID: 18697 RVA: 0x001CE029 File Offset: 0x001CC229
		[PublicizedFrom(EAccessModifier.Private)]
		public ProgressionClass.DisplayData.UnlockData GetUnlockData(int index)
		{
			if (this.UnlockDataList == null)
			{
				return null;
			}
			if (index >= this.UnlockDataList.Count)
			{
				return null;
			}
			return this.UnlockDataList[index];
		}

		// Token: 0x0600490A RID: 18698 RVA: 0x001CE054 File Offset: 0x001CC254
		public string GetUnlockItemIconName(int index)
		{
			ItemClass unlockItem = this.GetUnlockItem(index);
			if (unlockItem != null)
			{
				return unlockItem.GetIconName();
			}
			return "";
		}

		// Token: 0x0600490B RID: 18699 RVA: 0x001CE078 File Offset: 0x001CC278
		public string GetUnlockItemName(int index)
		{
			ItemClass unlockItem = this.GetUnlockItem(index);
			if (unlockItem != null)
			{
				return unlockItem.GetLocalizedItemName();
			}
			return "";
		}

		// Token: 0x0600490C RID: 18700 RVA: 0x001CE09C File Offset: 0x001CC29C
		public List<int> GetUnlockItemRecipes(int index)
		{
			if (this.UnlockDataList == null)
			{
				return null;
			}
			if (index >= this.UnlockDataList.Count)
			{
				return null;
			}
			List<int> list = new List<int>();
			if (this.Item != null)
			{
				list.Add(this.Item.Id);
			}
			else
			{
				ProgressionClass.DisplayData.UnlockData unlockData = this.UnlockDataList[index];
				if (unlockData != null)
				{
					if (unlockData.RecipeList != null)
					{
						for (int i = 0; i < unlockData.RecipeList.Length; i++)
						{
							list.Add(ItemClass.GetItemClass(unlockData.RecipeList[i], true).Id);
						}
					}
					else if (unlockData.Item != null)
					{
						list.Add(unlockData.Item.Id);
					}
				}
			}
			return list;
		}

		// Token: 0x0600490D RID: 18701 RVA: 0x001CE144 File Offset: 0x001CC344
		public string GetUnlockItemIconAtlas(EntityPlayerLocal player, int index)
		{
			ProgressionClass.DisplayData.UnlockData unlockData = this.GetUnlockData(index);
			if (unlockData == null)
			{
				return "ItemIconAtlas";
			}
			if (this.GetQualityLevel(player.Progression.GetProgressionValue(this.Owner.Name).Level) <= unlockData.UnlockTier)
			{
				return "ItemIconAtlasGreyscale";
			}
			return "ItemIconAtlas";
		}

		// Token: 0x0600490E RID: 18702 RVA: 0x001CE198 File Offset: 0x001CC398
		public bool GetUnlockItemLocked(EntityPlayerLocal player, int index)
		{
			ProgressionClass.DisplayData.UnlockData unlockData = this.GetUnlockData(index);
			return unlockData != null && this.GetQualityLevel(player.Progression.GetProgressionValue(this.Owner.Name).Level) <= unlockData.UnlockTier;
		}

		// Token: 0x0600490F RID: 18703 RVA: 0x001CE1E0 File Offset: 0x001CC3E0
		public void HandleCheckCrafting(EntityPlayerLocal _player, int _oldLevel, int _newLevel)
		{
			if (this.UnlockDataList == null)
			{
				return;
			}
			for (int i = 0; i < this.QualityStarts.Length; i++)
			{
				int num = this.QualityStarts[i];
				if (_oldLevel < num && _newLevel >= num)
				{
					if (this.HasQuality)
					{
						GameManager.ShowTooltip(_player, Localization.Get("ttCraftingSkillUnlockQuality", false), new string[]
						{
							Localization.Get(this.Owner.NameKey, false),
							this.GetName(_newLevel),
							(i + 1).ToString()
						}, ProgressionClass.DisplayData.CompletionSound, null, false, false, 0f);
					}
					else
					{
						GameManager.ShowTooltip(_player, Localization.Get("ttCraftingSkillUnlock", false), new string[]
						{
							Localization.Get(this.Owner.NameKey, false),
							this.GetName(_oldLevel)
						}, ProgressionClass.DisplayData.CompletionSound, null, false, false, 0f);
					}
				}
			}
		}

		// Token: 0x04003840 RID: 14400
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemClass item;

		// Token: 0x04003841 RID: 14401
		public string ItemName = "";

		// Token: 0x04003842 RID: 14402
		public string[] CustomIcon;

		// Token: 0x04003843 RID: 14403
		public string[] CustomIconTint;

		// Token: 0x04003844 RID: 14404
		public string[] CustomName;

		// Token: 0x04003845 RID: 14405
		public bool CustomHasQuality;

		// Token: 0x04003846 RID: 14406
		public int[] QualityStarts;

		// Token: 0x04003847 RID: 14407
		public List<ProgressionClass.DisplayData.UnlockData> UnlockDataList = new List<ProgressionClass.DisplayData.UnlockData>();

		// Token: 0x04003848 RID: 14408
		public static string CompletionSound = "";

		// Token: 0x04003849 RID: 14409
		public ProgressionClass Owner;

		// Token: 0x0200096F RID: 2415
		public class UnlockData
		{
			// Token: 0x170007B2 RID: 1970
			// (get) Token: 0x06004912 RID: 18706 RVA: 0x001CE2EC File Offset: 0x001CC4EC
			public ItemClass Item
			{
				get
				{
					if (this.item == null)
					{
						this.item = ItemClass.GetItemClass(this.ItemName, false);
					}
					return this.item;
				}
			}

			// Token: 0x0400384A RID: 14410
			[PublicizedFrom(EAccessModifier.Private)]
			public ItemClass item;

			// Token: 0x0400384B RID: 14411
			public string[] RecipeList;

			// Token: 0x0400384C RID: 14412
			public string ItemName = "";

			// Token: 0x0400384D RID: 14413
			public int UnlockTier;
		}
	}
}
