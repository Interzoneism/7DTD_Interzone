using System;
using Challenges;
using UnityEngine;

// Token: 0x02000297 RID: 663
public class RecipeUnlockData
{
	// Token: 0x170001F8 RID: 504
	// (get) Token: 0x060012E1 RID: 4833 RVA: 0x000752A9 File Offset: 0x000734A9
	// (set) Token: 0x060012E2 RID: 4834 RVA: 0x000752C9 File Offset: 0x000734C9
	public RecipeUnlockData.UnlockTypes UnlockType
	{
		get
		{
			if (this.unlockText != "")
			{
				this.Init();
			}
			return this.unlockType;
		}
		set
		{
			this.unlockType = value;
		}
	}

	// Token: 0x170001F9 RID: 505
	// (get) Token: 0x060012E3 RID: 4835 RVA: 0x000752D2 File Offset: 0x000734D2
	// (set) Token: 0x060012E4 RID: 4836 RVA: 0x000752DA File Offset: 0x000734DA
	public ItemClass Item
	{
		get
		{
			return this.item;
		}
		set
		{
			this.item = value;
			this.unlockType = RecipeUnlockData.UnlockTypes.Schematic;
		}
	}

	// Token: 0x170001FA RID: 506
	// (get) Token: 0x060012E5 RID: 4837 RVA: 0x000752EA File Offset: 0x000734EA
	// (set) Token: 0x060012E6 RID: 4838 RVA: 0x000752F2 File Offset: 0x000734F2
	public ProgressionClass Perk
	{
		get
		{
			return this.perk;
		}
		set
		{
			this.perk = value;
			if (this.perk.IsBook)
			{
				this.unlockType = RecipeUnlockData.UnlockTypes.Book;
				return;
			}
			if (this.perk.IsCrafting)
			{
				this.unlockType = RecipeUnlockData.UnlockTypes.Skill;
				return;
			}
			this.unlockType = RecipeUnlockData.UnlockTypes.Perk;
		}
	}

	// Token: 0x170001FB RID: 507
	// (get) Token: 0x060012E7 RID: 4839 RVA: 0x0007532C File Offset: 0x0007352C
	// (set) Token: 0x060012E8 RID: 4840 RVA: 0x00075334 File Offset: 0x00073534
	public ChallengeGroup ChallengeGroup
	{
		get
		{
			return this.challengeGroup;
		}
		set
		{
			this.challengeGroup = value;
			this.unlockType = RecipeUnlockData.UnlockTypes.ChallengeGroup;
		}
	}

	// Token: 0x170001FC RID: 508
	// (get) Token: 0x060012E9 RID: 4841 RVA: 0x00075344 File Offset: 0x00073544
	// (set) Token: 0x060012EA RID: 4842 RVA: 0x0007534C File Offset: 0x0007354C
	public ChallengeClass Challenge
	{
		get
		{
			return this.challenge;
		}
		set
		{
			this.challenge = value;
			this.unlockType = RecipeUnlockData.UnlockTypes.Challenge;
		}
	}

	// Token: 0x060012EB RID: 4843 RVA: 0x0007535C File Offset: 0x0007355C
	public RecipeUnlockData(string unlock)
	{
		this.unlockText = unlock;
	}

	// Token: 0x060012EC RID: 4844 RVA: 0x00075378 File Offset: 0x00073578
	public void Init()
	{
		if (Progression.ProgressionClasses.ContainsKey(this.unlockText))
		{
			this.Perk = Progression.ProgressionClasses[this.unlockText];
			return;
		}
		ChallengeGroup group;
		if ((group = ChallengeGroup.GetGroup(this.unlockText)) != null)
		{
			this.ChallengeGroup = group;
			return;
		}
		ChallengeClass challengeClass;
		if ((challengeClass = ChallengeClass.GetChallenge(this.unlockText)) != null)
		{
			this.Challenge = challengeClass;
			return;
		}
		ItemClass itemClass = ItemClass.GetItemClass(this.unlockText, true);
		if (itemClass != null)
		{
			this.Item = itemClass;
			return;
		}
		this.UnlockType = (GameManager.Instance.IsEditMode() ? RecipeUnlockData.UnlockTypes.PrefabEditorInvalid : RecipeUnlockData.UnlockTypes.None);
	}

	// Token: 0x060012ED RID: 4845 RVA: 0x00075410 File Offset: 0x00073610
	public string GetName()
	{
		if (this.unlockText != "")
		{
			this.Init();
		}
		if (this.unlockType == RecipeUnlockData.UnlockTypes.Schematic)
		{
			return this.item.GetLocalizedItemName();
		}
		if (this.unlockType == RecipeUnlockData.UnlockTypes.ChallengeGroup)
		{
			return this.ChallengeGroup.Title;
		}
		if (this.unlockType == RecipeUnlockData.UnlockTypes.Challenge)
		{
			return this.Challenge.Title;
		}
		return Localization.Get(this.perk.NameKey, false);
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x00075488 File Offset: 0x00073688
	public string GetIcon()
	{
		if (this.unlockText != "")
		{
			this.Init();
		}
		if (this.unlockType == RecipeUnlockData.UnlockTypes.Schematic)
		{
			return "ui_game_symbol_book";
		}
		if (this.unlockType == RecipeUnlockData.UnlockTypes.Perk)
		{
			return "ui_game_symbol_skills";
		}
		if (this.unlockType == RecipeUnlockData.UnlockTypes.Skill)
		{
			return "ui_game_symbol_hammer";
		}
		if (this.unlockType == RecipeUnlockData.UnlockTypes.ChallengeGroup || this.unlockType == RecipeUnlockData.UnlockTypes.Challenge)
		{
			return "ui_game_symbol_challenge";
		}
		return "ui_game_symbol_book";
	}

	// Token: 0x060012EF RID: 4847 RVA: 0x000754F8 File Offset: 0x000736F8
	public string GetLevel(EntityPlayerLocal player, string recipeName)
	{
		if (this.unlockText != "")
		{
			this.Init();
		}
		if (this.unlockType == RecipeUnlockData.UnlockTypes.Skill)
		{
			for (int i = 0; i < this.perk.DisplayDataList.Count; i++)
			{
				ProgressionClass.DisplayData displayData = this.perk.DisplayDataList[i];
				for (int j = 0; j < displayData.UnlockDataList.Count; j++)
				{
					ProgressionClass.DisplayData.UnlockData unlockData = displayData.UnlockDataList[j];
					if (unlockData.ItemName == recipeName || (unlockData.RecipeList != null && unlockData.RecipeList.ContainsCaseInsensitive(recipeName)))
					{
						int unlockTier = unlockData.UnlockTier;
						ProgressionValue progressionValue = player.Progression.GetProgressionValue(this.perk.Name);
						return string.Format("{0}/{1}", progressionValue.Level, displayData.QualityStarts[unlockTier].ToString());
					}
				}
			}
			return "--";
		}
		return "--";
	}

	// Token: 0x060012F0 RID: 4848 RVA: 0x000755FA File Offset: 0x000737FA
	public string GetIconAtlas()
	{
		return "UIAtlas";
	}

	// Token: 0x060012F1 RID: 4849 RVA: 0x00075601 File Offset: 0x00073801
	public Color GetItemTint()
	{
		if (this.unlockText != "")
		{
			this.Init();
		}
		if (this.unlockType == RecipeUnlockData.UnlockTypes.Schematic && this.item != null)
		{
			return this.item.GetIconTint(null);
		}
		return Color.white;
	}

	// Token: 0x04000C67 RID: 3175
	[PublicizedFrom(EAccessModifier.Private)]
	public RecipeUnlockData.UnlockTypes unlockType;

	// Token: 0x04000C68 RID: 3176
	[PublicizedFrom(EAccessModifier.Private)]
	public string unlockText = "";

	// Token: 0x04000C69 RID: 3177
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass item;

	// Token: 0x04000C6A RID: 3178
	[PublicizedFrom(EAccessModifier.Private)]
	public ProgressionClass perk;

	// Token: 0x04000C6B RID: 3179
	[PublicizedFrom(EAccessModifier.Private)]
	public ChallengeGroup challengeGroup;

	// Token: 0x04000C6C RID: 3180
	[PublicizedFrom(EAccessModifier.Private)]
	public ChallengeClass challenge;

	// Token: 0x02000298 RID: 664
	public enum UnlockTypes
	{
		// Token: 0x04000C6E RID: 3182
		None,
		// Token: 0x04000C6F RID: 3183
		Perk,
		// Token: 0x04000C70 RID: 3184
		Book,
		// Token: 0x04000C71 RID: 3185
		Skill,
		// Token: 0x04000C72 RID: 3186
		Schematic,
		// Token: 0x04000C73 RID: 3187
		ChallengeGroup,
		// Token: 0x04000C74 RID: 3188
		Challenge,
		// Token: 0x04000C75 RID: 3189
		PrefabEditorInvalid
	}
}
