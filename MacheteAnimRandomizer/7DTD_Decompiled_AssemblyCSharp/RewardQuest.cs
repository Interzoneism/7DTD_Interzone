using System;
using UnityEngine.Scripting;

// Token: 0x0200091D RID: 2333
[Preserve]
public class RewardQuest : BaseReward
{
	// Token: 0x06004598 RID: 17816 RVA: 0x001BD31E File Offset: 0x001BB51E
	public override void SetupReward()
	{
		base.Description = Localization.Get("RewardQuest_keyword", false);
		base.ValueText = QuestClass.GetQuest(base.ID).Name;
		base.Icon = "ui_game_symbol_quest";
	}

	// Token: 0x06004599 RID: 17817 RVA: 0x001BD354 File Offset: 0x001BB554
	public override void GiveReward(EntityPlayer player)
	{
		Quest quest = QuestClass.CreateQuest(base.ID);
		if (base.OwnerQuest != null)
		{
			quest.PreviousQuest = QuestClass.GetQuest(base.OwnerQuest.ID).Name;
		}
		player.QuestJournal.AddQuest(quest, true);
	}

	// Token: 0x0600459A RID: 17818 RVA: 0x001BD3A0 File Offset: 0x001BB5A0
	public override BaseReward Clone()
	{
		RewardQuest rewardQuest = new RewardQuest();
		base.CopyValues(rewardQuest);
		rewardQuest.IsChainQuest = this.IsChainQuest;
		return rewardQuest;
	}

	// Token: 0x0600459B RID: 17819 RVA: 0x001BD3C8 File Offset: 0x001BB5C8
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		if (properties.Values.ContainsKey(RewardQuest.PropQuest))
		{
			base.ID = properties.Values[RewardQuest.PropQuest];
		}
		if (properties.Values.ContainsKey(RewardQuest.PropChainQuest))
		{
			this.IsChainQuest = Convert.ToBoolean(properties.Values[RewardQuest.PropChainQuest]);
		}
	}

	// Token: 0x0400366A RID: 13930
	public static string PropQuest = "quest";

	// Token: 0x0400366B RID: 13931
	public static string PropChainQuest = "chainquest";

	// Token: 0x0400366C RID: 13932
	public bool IsChainQuest = true;
}
