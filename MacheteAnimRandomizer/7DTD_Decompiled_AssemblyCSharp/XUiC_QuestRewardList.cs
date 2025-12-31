using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000DB9 RID: 3513
[Preserve]
public class XUiC_QuestRewardList : XUiController
{
	// Token: 0x17000B0C RID: 2828
	// (get) Token: 0x06006DEC RID: 28140 RVA: 0x002CCFEB File Offset: 0x002CB1EB
	// (set) Token: 0x06006DED RID: 28141 RVA: 0x002CCFF3 File Offset: 0x002CB1F3
	public Quest Quest
	{
		get
		{
			return this.quest;
		}
		set
		{
			this.quest = value;
			this.isDirty = true;
		}
	}

	// Token: 0x06006DEE RID: 28142 RVA: 0x002CD004 File Offset: 0x002CB204
	public override void Init()
	{
		base.Init();
		XUiController[] childrenByType = base.GetChildrenByType<XUiC_QuestRewardEntry>(null);
		XUiController[] array = childrenByType;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				this.rewardEntries.Add(array[i]);
			}
		}
	}

	// Token: 0x06006DEF RID: 28143 RVA: 0x002CD044 File Offset: 0x002CB244
	public override void Update(float _dt)
	{
		if (this.isDirty)
		{
			if (this.quest != null)
			{
				List<BaseReward> list = new List<BaseReward>();
				Quest quest2;
				for (Quest quest = this.quest; quest != null; quest = quest2)
				{
					quest2 = null;
					for (int i = 0; i < quest.Rewards.Count; i++)
					{
						if (quest.Rewards[i] is RewardQuest)
						{
							quest2 = QuestClass.CreateQuest(quest.Rewards[i].ID);
						}
						if (!quest.Rewards[i].HiddenReward && quest.Rewards[i].ReceiveStage == BaseReward.ReceiveStages.QuestCompletion && (quest == this.quest || !(quest.Rewards[i] is RewardQuest)))
						{
							list.Add(quest.Rewards[i]);
						}
					}
				}
				int count = this.rewardEntries.Count;
				int count2 = list.Count;
				int num = 0;
				for (int j = 0; j < count; j++)
				{
					if (this.rewardEntries[num] is XUiC_QuestRewardEntry)
					{
						if (j < count2)
						{
							((XUiC_QuestRewardEntry)this.rewardEntries[num]).Reward = list[j];
							((XUiC_QuestRewardEntry)this.rewardEntries[num]).ChainQuest = (list[j].OwnerQuest != this.Quest);
							num++;
						}
						else
						{
							((XUiC_QuestRewardEntry)this.rewardEntries[num]).Reward = null;
							num++;
						}
					}
				}
			}
			else
			{
				int count3 = this.rewardEntries.Count;
				for (int k = 0; k < count3; k++)
				{
					if (this.rewardEntries[k] is XUiC_QuestRewardEntry)
					{
						((XUiC_QuestRewardEntry)this.rewardEntries[k]).Reward = null;
					}
				}
			}
			this.isDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x0400537F RID: 21375
	[PublicizedFrom(EAccessModifier.Private)]
	public Quest quest;

	// Token: 0x04005380 RID: 21376
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiController> rewardEntries = new List<XUiController>();

	// Token: 0x04005381 RID: 21377
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;
}
