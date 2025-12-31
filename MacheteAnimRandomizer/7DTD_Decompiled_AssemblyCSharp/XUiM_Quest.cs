using System;
using Challenges;
using UnityEngine;

// Token: 0x02000EFA RID: 3834
public class XUiM_Quest : XUiModel
{
	// Token: 0x140000DC RID: 220
	// (add) Token: 0x060078D0 RID: 30928 RVA: 0x0031258C File Offset: 0x0031078C
	// (remove) Token: 0x060078D1 RID: 30929 RVA: 0x003125C4 File Offset: 0x003107C4
	public event XUiEvent_TrackedQuestChanged OnTrackedQuestChanged;

	// Token: 0x17000C31 RID: 3121
	// (get) Token: 0x060078D2 RID: 30930 RVA: 0x003125F9 File Offset: 0x003107F9
	// (set) Token: 0x060078D3 RID: 30931 RVA: 0x00312601 File Offset: 0x00310801
	public Quest TrackedQuest
	{
		get
		{
			return this.trackedQuest;
		}
		set
		{
			this.trackedQuest = value;
			if (this.OnTrackedQuestChanged != null)
			{
				this.OnTrackedQuestChanged();
			}
		}
	}

	// Token: 0x140000DD RID: 221
	// (add) Token: 0x060078D4 RID: 30932 RVA: 0x00312620 File Offset: 0x00310820
	// (remove) Token: 0x060078D5 RID: 30933 RVA: 0x00312658 File Offset: 0x00310858
	public event XUiEvent_TrackedQuestChanged OnTrackedChallengeChanged;

	// Token: 0x17000C32 RID: 3122
	// (get) Token: 0x060078D6 RID: 30934 RVA: 0x0031268D File Offset: 0x0031088D
	// (set) Token: 0x060078D7 RID: 30935 RVA: 0x00312698 File Offset: 0x00310898
	public Challenge TrackedChallenge
	{
		get
		{
			return this.trackedChallenge;
		}
		set
		{
			if (this.trackedChallenge != null)
			{
				this.trackedChallenge.IsTracked = false;
				this.trackedChallenge.RemovePrerequisiteHooks();
				this.trackedChallenge.HandleTrackingEnded();
			}
			this.trackedChallenge = value;
			if (this.trackedChallenge != null)
			{
				this.trackedChallenge.IsTracked = true;
				this.trackedChallenge.AddPrerequisiteHooks();
				this.trackedChallenge.HandleTrackingStarted();
				this.trackedChallenge.Owner.Player.PlayerUI.xui.Recipes.TrackedRecipe = null;
				this.trackedChallenge.Owner.Player.QuestJournal.TrackedQuest = null;
			}
			if (this.OnTrackedChallengeChanged != null)
			{
				this.OnTrackedChallengeChanged();
			}
		}
	}

	// Token: 0x060078D8 RID: 30936 RVA: 0x00312753 File Offset: 0x00310953
	public void HandleTrackedChallengeChanged()
	{
		if (this.OnTrackedChallengeChanged != null)
		{
			this.OnTrackedChallengeChanged();
		}
	}

	// Token: 0x060078D9 RID: 30937 RVA: 0x00312768 File Offset: 0x00310968
	public static string GetQuestItemRewards(Quest quest, EntityPlayer player, string rewardItemFormat, string rewardBonusItemFormat)
	{
		string text = "";
		if (quest != null)
		{
			for (int i = 0; i < quest.Rewards.Count; i++)
			{
				if ((quest.Rewards[i] is RewardItem || quest.Rewards[i] is RewardLootItem) && !quest.Rewards[i].isChosenReward)
				{
					ItemStack itemStack = null;
					if (quest.Rewards[i] is RewardItem)
					{
						itemStack = (quest.Rewards[i] as RewardItem).Item;
					}
					else if (quest.Rewards[i] is RewardLootItem)
					{
						itemStack = (quest.Rewards[i] as RewardLootItem).Item;
					}
					int count = itemStack.count;
					int count2 = quest.Rewards[i].GetRewardItem().count;
					if (count == count2)
					{
						text = text + string.Format(rewardItemFormat, count2, itemStack.itemValue.ItemClass.GetLocalizedItemName()) + ", ";
					}
					else
					{
						text = text + string.Format(rewardBonusItemFormat, new object[]
						{
							count2,
							itemStack.itemValue.ItemClass.GetLocalizedItemName(),
							count2 - count,
							Localization.Get("bonus", false)
						}) + ", ";
					}
				}
			}
			if (text != "")
			{
				text = text.Remove(text.Length - 2);
			}
		}
		return text;
	}

	// Token: 0x060078DA RID: 30938 RVA: 0x003128F0 File Offset: 0x00310AF0
	public static bool HasQuestRewards(Quest quest, EntityPlayer player, bool isChosen)
	{
		if (quest != null)
		{
			for (int i = 0; i < quest.Rewards.Count; i++)
			{
				if (quest.Rewards[i].isChosenReward == isChosen && !(quest.Rewards[i] is RewardQuest))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060078DB RID: 30939 RVA: 0x00312940 File Offset: 0x00310B40
	public static string GetQuestRewards(Quest quest, EntityPlayer player, bool isChosen, string rewardItemFormat, string rewardItemBonusFormat, string rewardNumberFormat, string rewardNumberBonusFormat)
	{
		string text = "";
		if (quest != null)
		{
			int num = isChosen ? ((int)EffectManager.GetValue(PassiveEffects.QuestRewardOptionCount, null, 1f, player, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) + 1) : -1;
			for (int i = 0; i < quest.Rewards.Count; i++)
			{
				if (quest.Rewards[i].isChosenReward == isChosen)
				{
					if (isChosen && num-- <= 0)
					{
						break;
					}
					if (quest.Rewards[i] is RewardItem)
					{
						RewardItem rewardItem = quest.Rewards[i] as RewardItem;
						int count = rewardItem.Item.count;
						int count2 = quest.Rewards[i].GetRewardItem().count;
						if (count == count2)
						{
							text = text + string.Format(rewardItemFormat, count2, rewardItem.Item.itemValue.ItemClass.GetLocalizedItemName()) + ", ";
						}
						else
						{
							text = text + string.Format(rewardItemBonusFormat, new object[]
							{
								count2,
								rewardItem.Item.itemValue.ItemClass.GetLocalizedItemName(),
								count2 - count,
								Localization.Get("bonus", false)
							}) + ", ";
						}
					}
					if (quest.Rewards[i] is RewardLootItem)
					{
						RewardLootItem rewardLootItem = quest.Rewards[i] as RewardLootItem;
						int count3 = rewardLootItem.Item.count;
						text = text + string.Format(rewardItemFormat, rewardLootItem.Item.count, rewardLootItem.Item.itemValue.ItemClass.GetLocalizedItemName()) + ", ";
					}
					else if (quest.Rewards[i] is RewardExp)
					{
						BaseReward baseReward = quest.Rewards[i] as RewardExp;
						int num2 = 0;
						int num3 = Convert.ToInt32(baseReward.Value) * GameStats.GetInt(EnumGameStats.XPMultiplier) / 100;
						num2 += Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.PlayerExpGain, null, (float)num3, player, null, XUiM_Quest.QuestTag, true, true, true, true, true, 1, true, false));
						text = text + string.Format(rewardNumberFormat, num2, Localization.Get("RewardXP_keyword", false)) + ", ";
					}
					else if (quest.Rewards[i] is RewardSkillPoints)
					{
						int num4 = Convert.ToInt32((quest.Rewards[i] as RewardSkillPoints).Value);
						text = text + string.Format(rewardNumberFormat, num4, Localization.Get("RewardSkillPoints_keyword", false)) + ", ";
					}
				}
			}
			if (text != "")
			{
				text = text.Remove(text.Length - 2);
			}
		}
		return text;
	}

	// Token: 0x060078DC RID: 30940 RVA: 0x00312C10 File Offset: 0x00310E10
	public static bool HasChainQuestRewards(Quest quest, EntityPlayer player)
	{
		Quest quest2 = quest;
		while (quest != null)
		{
			Quest quest3 = null;
			for (int i = 0; i < quest.Rewards.Count; i++)
			{
				if (!quest.Rewards[i].isChosenReward && quest.Rewards[i].isChainReward && quest != quest2)
				{
					return true;
				}
				if (quest.Rewards[i] is RewardQuest)
				{
					RewardQuest rewardQuest = quest.Rewards[i] as RewardQuest;
					if (rewardQuest.IsChainQuest)
					{
						quest3 = QuestClass.CreateQuest(rewardQuest.ID);
					}
				}
			}
			quest = quest3;
		}
		return false;
	}

	// Token: 0x060078DD RID: 30941 RVA: 0x00312CB0 File Offset: 0x00310EB0
	public static string GetChainQuestRewards(Quest quest, EntityPlayer player, string rewardItemFormat, string rewardItemBonusFormat, string rewardNumberFormat, string rewardNumberBonusFormat)
	{
		string text = "";
		Quest quest2 = quest;
		while (quest != null)
		{
			Quest quest3 = null;
			for (int i = 0; i < quest.Rewards.Count; i++)
			{
				if (!quest.Rewards[i].isChosenReward && quest.Rewards[i].isChainReward && quest != quest2)
				{
					if (quest.Rewards[i] is RewardItem)
					{
						RewardItem rewardItem = quest.Rewards[i] as RewardItem;
						int count = rewardItem.Item.count;
						int count2 = quest.Rewards[i].GetRewardItem().count;
						if (count == count2)
						{
							text = text + string.Format(rewardItemFormat, count2, rewardItem.Item.itemValue.ItemClass.GetLocalizedItemName()) + ", ";
						}
						else
						{
							text = text + string.Format(rewardItemBonusFormat, new object[]
							{
								count2,
								rewardItem.Item.itemValue.ItemClass.GetLocalizedItemName(),
								count2 - count,
								Localization.Get("bonus", false)
							}) + ", ";
						}
					}
					else if (quest.Rewards[i] is RewardExp)
					{
						BaseReward baseReward = quest.Rewards[i] as RewardExp;
						int num = 0;
						int num2 = Convert.ToInt32(baseReward.Value) * GameStats.GetInt(EnumGameStats.XPMultiplier) / 100;
						num += Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.PlayerExpGain, null, (float)num2, player, null, XUiM_Quest.QuestTag, true, true, true, true, true, 1, true, false));
						text = text + string.Format(rewardNumberFormat, num, Localization.Get("RewardXP_keyword", false)) + ", ";
					}
					else if (quest.Rewards[i] is RewardSkillPoints)
					{
						int num3 = Convert.ToInt32((quest.Rewards[i] as RewardSkillPoints).Value);
						text = text + string.Format(rewardNumberFormat, num3, Localization.Get("RewardSkillPoints_keyword", false)) + ", ";
					}
				}
				if (quest.Rewards[i] is RewardQuest)
				{
					RewardQuest rewardQuest = quest.Rewards[i] as RewardQuest;
					if (rewardQuest.IsChainQuest)
					{
						quest3 = QuestClass.CreateQuest(rewardQuest.ID);
					}
				}
			}
			quest = quest3;
			if (text != "")
			{
				text = text.Remove(text.Length - 2);
			}
		}
		return text;
	}

	// Token: 0x04005BD2 RID: 23506
	[PublicizedFrom(EAccessModifier.Private)]
	public Quest trackedQuest;

	// Token: 0x04005BD4 RID: 23508
	[PublicizedFrom(EAccessModifier.Private)]
	public Challenge trackedChallenge;

	// Token: 0x04005BD5 RID: 23509
	public static FastTags<TagGroup.Global> QuestTag = FastTags<TagGroup.Global>.Parse("quest");
}
