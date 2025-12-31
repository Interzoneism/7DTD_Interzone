using System;
using UnityEngine.Scripting;

// Token: 0x02000DB7 RID: 3511
[Preserve]
public class XUiC_QuestRewardEntry : XUiController
{
	// Token: 0x17000B06 RID: 2822
	// (get) Token: 0x06006DDB RID: 28123 RVA: 0x002CCC10 File Offset: 0x002CAE10
	// (set) Token: 0x06006DDC RID: 28124 RVA: 0x002CCC18 File Offset: 0x002CAE18
	public BaseReward Reward
	{
		get
		{
			return this.reward;
		}
		set
		{
			this.reward = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000B07 RID: 2823
	// (get) Token: 0x06006DDD RID: 28125 RVA: 0x002CCC28 File Offset: 0x002CAE28
	// (set) Token: 0x06006DDE RID: 28126 RVA: 0x002CCC30 File Offset: 0x002CAE30
	public bool ChainQuest { get; set; }

	// Token: 0x17000B08 RID: 2824
	// (get) Token: 0x06006DDF RID: 28127 RVA: 0x002CCC39 File Offset: 0x002CAE39
	public static string ChainQuestTypeKeyword
	{
		get
		{
			if (XUiC_QuestRewardEntry.chainQuestTypeKeyword == "")
			{
				XUiC_QuestRewardEntry.chainQuestTypeKeyword = Localization.Get("RewardTypeChainQuest", false);
			}
			return XUiC_QuestRewardEntry.chainQuestTypeKeyword;
		}
	}

	// Token: 0x17000B09 RID: 2825
	// (get) Token: 0x06006DE0 RID: 28128 RVA: 0x002CCC61 File Offset: 0x002CAE61
	public static string QuestTypeKeyword
	{
		get
		{
			if (XUiC_QuestRewardEntry.questTypeKeyword == "")
			{
				XUiC_QuestRewardEntry.questTypeKeyword = Localization.Get("RewardTypeQuest", false);
			}
			return XUiC_QuestRewardEntry.questTypeKeyword;
		}
	}

	// Token: 0x17000B0A RID: 2826
	// (get) Token: 0x06006DE1 RID: 28129 RVA: 0x002CCC89 File Offset: 0x002CAE89
	public static string OptionalKeyword
	{
		get
		{
			if (XUiC_QuestRewardEntry.optionalKeyword == "")
			{
				XUiC_QuestRewardEntry.optionalKeyword = Localization.Get("optional", false);
			}
			return XUiC_QuestRewardEntry.optionalKeyword;
		}
	}

	// Token: 0x17000B0B RID: 2827
	// (get) Token: 0x06006DE2 RID: 28130 RVA: 0x002CCCB1 File Offset: 0x002CAEB1
	public static string BonusKeyword
	{
		get
		{
			if (XUiC_QuestRewardEntry.bonusKeyword == "")
			{
				XUiC_QuestRewardEntry.bonusKeyword = Localization.Get("bonus", false);
			}
			return XUiC_QuestRewardEntry.bonusKeyword;
		}
	}

	// Token: 0x06006DE3 RID: 28131 RVA: 0x002CCCDC File Offset: 0x002CAEDC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = this.reward != null;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 1771022199U)
		{
			if (num != 463969434U)
			{
				if (num != 1455818684U)
				{
					if (num == 1771022199U)
					{
						if (bindingName == "rewardicon")
						{
							value = (flag ? this.reward.Icon : "");
							return true;
						}
					}
				}
				else if (bindingName == "rewarddescription")
				{
					value = (flag ? this.reward.Description : "");
					return true;
				}
			}
			else if (bindingName == "rewardtype")
			{
				if (flag)
				{
					if (this.ChainQuest)
					{
						value = XUiC_QuestRewardEntry.ChainQuestTypeKeyword;
					}
					else
					{
						string v = this.reward.Optional ? XUiC_QuestRewardEntry.BonusKeyword : Localization.Get(QuestClass.GetQuest(this.Reward.OwnerQuest.ID).Category, false);
						value = this.rewardTypeFormatter.Format(v, XUiC_QuestRewardEntry.QuestTypeKeyword);
					}
				}
				else
				{
					value = "";
				}
				return true;
			}
		}
		else if (num <= 2048631988U)
		{
			if (num != 1864291747U)
			{
				if (num == 2048631988U)
				{
					if (bindingName == "hasreward")
					{
						value = flag.ToString();
						return true;
					}
				}
			}
			else if (bindingName == "rewardvalue")
			{
				value = (flag ? this.reward.ValueText : "");
				return true;
			}
		}
		else if (num != 3261050802U)
		{
			if (num == 3763801510U)
			{
				if (bindingName == "rewardiconatlas")
				{
					value = (flag ? this.reward.IconAtlas : "");
					return true;
				}
			}
		}
		else if (bindingName == "rewardoptional")
		{
			value = (flag ? (this.reward.Optional ? this.rewardOptionalFormatter.Format(XUiC_QuestRewardEntry.OptionalKeyword) : "") : "");
			return true;
		}
		return false;
	}

	// Token: 0x06006DE4 RID: 28132 RVA: 0x002CCEED File Offset: 0x002CB0ED
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnCountChanged(XUiController _sender, OnCountChangedEventArgs _e)
	{
		this.isDirty = true;
	}

	// Token: 0x06006DE5 RID: 28133 RVA: 0x002CCEF6 File Offset: 0x002CB0F6
	public override void Update(float _dt)
	{
		base.RefreshBindings(this.isDirty);
		this.isDirty = false;
		base.Update(_dt);
	}

	// Token: 0x0400536F RID: 21359
	[PublicizedFrom(EAccessModifier.Private)]
	public string completeIconName = "";

	// Token: 0x04005370 RID: 21360
	[PublicizedFrom(EAccessModifier.Private)]
	public string incompleteIconName = "";

	// Token: 0x04005371 RID: 21361
	[PublicizedFrom(EAccessModifier.Private)]
	public string completeColor = "0,255,0,255";

	// Token: 0x04005372 RID: 21362
	[PublicizedFrom(EAccessModifier.Private)]
	public string incompleteColor = "255,0,0,255";

	// Token: 0x04005373 RID: 21363
	[PublicizedFrom(EAccessModifier.Private)]
	public BaseReward reward;

	// Token: 0x04005374 RID: 21364
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x04005376 RID: 21366
	[PublicizedFrom(EAccessModifier.Private)]
	public static string chainQuestTypeKeyword = "";

	// Token: 0x04005377 RID: 21367
	[PublicizedFrom(EAccessModifier.Private)]
	public static string questTypeKeyword = "";

	// Token: 0x04005378 RID: 21368
	[PublicizedFrom(EAccessModifier.Private)]
	public static string optionalKeyword = "";

	// Token: 0x04005379 RID: 21369
	[PublicizedFrom(EAccessModifier.Private)]
	public static string bonusKeyword = "";

	// Token: 0x0400537A RID: 21370
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<string> rewardOptionalFormatter = new CachedStringFormatter<string>((string _s) => "(" + _s + ") ");

	// Token: 0x0400537B RID: 21371
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<string, string> rewardTypeFormatter = new CachedStringFormatter<string, string>((string _s1, string _s2) => _s1 + " " + _s2);
}
