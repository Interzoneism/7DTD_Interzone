using System;
using UnityEngine.Scripting;

// Token: 0x02000DBA RID: 3514
[Preserve]
public class XUiC_QuestRewardsWindow : XUiController
{
	// Token: 0x17000B0D RID: 2829
	// (get) Token: 0x06006DF1 RID: 28145 RVA: 0x002CD251 File Offset: 0x002CB451
	// (set) Token: 0x06006DF2 RID: 28146 RVA: 0x002CD259 File Offset: 0x002CB459
	public Quest CurrentQuest
	{
		get
		{
			return this.currentQuest;
		}
		set
		{
			this.currentQuest = value;
			this.questClass = ((this.currentQuest != null) ? QuestClass.GetQuest(this.currentQuest.ID) : null);
			base.RefreshBindings(true);
		}
	}

	// Token: 0x06006DF3 RID: 28147 RVA: 0x002CD28A File Offset: 0x002CB48A
	public override void Init()
	{
		base.Init();
		this.rewardList = base.GetChildByType<XUiC_QuestRewardList>();
	}

	// Token: 0x06006DF4 RID: 28148 RVA: 0x002842C8 File Offset: 0x002824C8
	public override void OnOpen()
	{
		base.OnOpen();
		base.RefreshBindings(false);
	}

	// Token: 0x06006DF5 RID: 28149 RVA: 0x002CD2A0 File Offset: 0x002CB4A0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 2432099586U)
		{
			if (num <= 378804592U)
			{
				if (num != 264092286U)
				{
					if (num == 378804592U)
					{
						if (bindingName == "commonrewards")
						{
							value = "";
							if (this.currentQuest != null)
							{
								string questRewards = XUiM_Quest.GetQuestRewards(this.currentQuest, base.xui.playerUI.entityPlayer, false, this.rewardItemFormat, this.rewardItemBonusFormat, this.rewardNumberFormat, this.rewardNumberBonusFormat);
								if (questRewards == "")
								{
									value = Localization.Get("none", false);
								}
								else
								{
									value = questRewards;
								}
							}
							return true;
						}
					}
				}
				else if (bindingName == "chainrewards")
				{
					value = "";
					if (this.currentQuest != null)
					{
						string chainQuestRewards = XUiM_Quest.GetChainQuestRewards(this.currentQuest, base.xui.playerUI.entityPlayer, this.rewardItemFormat, this.rewardItemBonusFormat, this.rewardNumberFormat, this.rewardNumberBonusFormat);
						if (chainQuestRewards == "")
						{
							value = Localization.Get("none", false);
						}
						else
						{
							value = chainQuestRewards;
						}
					}
					return true;
				}
			}
			else if (num != 614962720U)
			{
				if (num != 986815653U)
				{
					if (num == 2432099586U)
					{
						if (bindingName == "commontitle")
						{
							value = ((this.currentQuest != null) ? (Localization.Get("xuiRewards", false) + ": ") : "");
							return true;
						}
					}
				}
				else if (bindingName == "chosenrewards")
				{
					value = "";
					if (this.currentQuest != null)
					{
						string questRewards2 = XUiM_Quest.GetQuestRewards(this.currentQuest, base.xui.playerUI.entityPlayer, true, this.rewardItemFormat, this.rewardItemBonusFormat, this.rewardNumberFormat, this.rewardNumberBonusFormat);
						if (questRewards2 == "")
						{
							value = Localization.Get("none", false);
						}
						else
						{
							value = questRewards2;
						}
					}
					return true;
				}
			}
			else if (bindingName == "chaintitle")
			{
				value = ((this.currentQuest != null) ? (Localization.Get("RewardTypeChainQuest", false) + ": ") : "");
				return true;
			}
		}
		else if (num <= 3033834579U)
		{
			if (num != 2646005107U)
			{
				if (num != 2730462270U)
				{
					if (num == 3033834579U)
					{
						if (bindingName == "finishtime")
						{
							value = "";
							if (this.currentQuest != null && this.currentQuest.FinishTime > 0UL)
							{
								ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements(this.currentQuest.FinishTime);
								int item = valueTuple.Item1;
								int item2 = valueTuple.Item2;
								int item3 = valueTuple.Item3;
								value = this.daytimeFormatter.Format((this.currentQuest.CurrentState == Quest.QuestState.Completed) ? Localization.Get("completed", false) : Localization.Get("failed", false), item, item2, item3);
							}
							return true;
						}
					}
				}
				else if (bindingName == "questname")
				{
					value = ((this.currentQuest != null) ? this.questClass.Name : "");
					return true;
				}
			}
			else if (bindingName == "chosentitle")
			{
				if (this.currentQuest == null)
				{
					value = "";
				}
				if (XUiM_Quest.HasQuestRewards(this.currentQuest, base.xui.playerUI.entityPlayer, true))
				{
					float value2 = EffectManager.GetValue(PassiveEffects.QuestRewardChoiceCount, null, 1f, base.xui.playerUI.entityPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
					value = ((value2 == 1f) ? Localization.Get("xuiChooseOne", false) : Localization.Get("xuiChooseTwo", false));
				}
				else
				{
					value = "";
				}
				return true;
			}
		}
		else if (num != 3047389681U)
		{
			if (num != 3231221182U)
			{
				if (num == 4060322893U)
				{
					if (bindingName == "showempty")
					{
						value = (this.currentQuest == null).ToString();
						return true;
					}
				}
			}
			else if (bindingName == "showquest")
			{
				value = (this.currentQuest != null).ToString();
				return true;
			}
		}
		else if (bindingName == "questtitle")
		{
			value = ((this.currentQuest != null) ? this.questClass.SubTitle : "");
			return true;
		}
		return false;
	}

	// Token: 0x06006DF6 RID: 28150 RVA: 0x002CD72B File Offset: 0x002CB92B
	public void SetQuest(XUiC_QuestEntry questEntry)
	{
		this.entry = questEntry;
		if (this.entry != null)
		{
			this.CurrentQuest = this.entry.Quest;
			return;
		}
		this.CurrentQuest = null;
	}

	// Token: 0x04005382 RID: 21378
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestEntry entry;

	// Token: 0x04005383 RID: 21379
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestRewardList rewardList;

	// Token: 0x04005384 RID: 21380
	[PublicizedFrom(EAccessModifier.Private)]
	public QuestClass questClass;

	// Token: 0x04005385 RID: 21381
	[PublicizedFrom(EAccessModifier.Private)]
	public Quest currentQuest;

	// Token: 0x04005386 RID: 21382
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<string, int, int, int> daytimeFormatter = new CachedStringFormatter<string, int, int, int>((string _status, int _day, int _hour, int _min) => string.Format("{0} {1} {2}, {3:00}:{4:00}", new object[]
	{
		_status,
		Localization.Get("xuiDay", false),
		_day,
		_hour,
		_min
	}));

	// Token: 0x04005387 RID: 21383
	[PublicizedFrom(EAccessModifier.Private)]
	public string rewardNumberFormat = "[DECEA3]{0}[-] {1}";

	// Token: 0x04005388 RID: 21384
	[PublicizedFrom(EAccessModifier.Private)]
	public string rewardNumberBonusFormat = "[DECEA3]{0}[-] {1} ([DECEA3]{2}[-] {3})";

	// Token: 0x04005389 RID: 21385
	[PublicizedFrom(EAccessModifier.Private)]
	public string rewardItemFormat = "[DECEA3]{0}[-] {1}";

	// Token: 0x0400538A RID: 21386
	[PublicizedFrom(EAccessModifier.Private)]
	public string rewardItemBonusFormat = "[DECEA3]{0}[-] {1} ([DECEA3]{2}[-] {3})";
}
