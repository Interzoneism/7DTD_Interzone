using System;
using UnityEngine.Scripting;

// Token: 0x020002B2 RID: 690
[Preserve]
public class DialogRequirementQuestStatus : BaseDialogRequirement
{
	// Token: 0x1700021B RID: 539
	// (get) Token: 0x06001360 RID: 4960 RVA: 0x000197A5 File Offset: 0x000179A5
	public override BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.QuestStatus;
		}
	}

	// Token: 0x06001361 RID: 4961 RVA: 0x0002B133 File Offset: 0x00029333
	public override string GetRequiredDescription(EntityPlayer player)
	{
		return "";
	}

	// Token: 0x06001362 RID: 4962 RVA: 0x00076A58 File Offset: 0x00074C58
	public override bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		DialogRequirementQuestStatus.QuestStatuses questStatuses = EnumUtils.Parse<DialogRequirementQuestStatus.QuestStatuses>(base.Value, false);
		string id = base.ID;
		if (string.IsNullOrEmpty(base.ID))
		{
			EntityNPC respondent = LocalPlayerUI.GetUIForPlayer(GameManager.Instance.World.GetPrimaryPlayer()).xui.Dialog.Respondent;
			if (respondent != null)
			{
				int entityId = respondent.entityId;
				if (questStatuses == DialogRequirementQuestStatus.QuestStatuses.NotStarted)
				{
					Quest quest = player.QuestJournal.FindActiveQuestByGiver(entityId, base.Tag);
					return quest == null;
				}
				if (questStatuses == DialogRequirementQuestStatus.QuestStatuses.InProgress)
				{
					Quest quest = player.QuestJournal.FindActiveQuestByGiver(entityId, base.Tag);
					return quest != null;
				}
			}
		}
		else
		{
			switch (questStatuses)
			{
			case DialogRequirementQuestStatus.QuestStatuses.NotStarted:
			{
				Quest quest = player.QuestJournal.FindNonSharedQuest(id);
				if (quest == null || quest.CurrentState == Quest.QuestState.Completed)
				{
					return true;
				}
				break;
			}
			case DialogRequirementQuestStatus.QuestStatuses.InProgress:
			{
				Quest quest = player.QuestJournal.FindNonSharedQuest(id);
				if (quest != null && quest.Active)
				{
					for (int i = 0; i < quest.Objectives.Count; i++)
					{
						if (!quest.Objectives[i].Complete)
						{
							return true;
						}
					}
				}
				break;
			}
			case DialogRequirementQuestStatus.QuestStatuses.TurnInReady:
			{
				Quest quest = player.QuestJournal.FindQuest(id, (int)talkingTo.NPCInfo.QuestFaction);
				if (quest != null && quest.Active)
				{
					for (int j = 0; j < quest.Objectives.Count; j++)
					{
						if (!quest.Objectives[j].Complete)
						{
							return false;
						}
					}
					return true;
				}
				break;
			}
			case DialogRequirementQuestStatus.QuestStatuses.Completed:
			{
				Quest quest = player.QuestJournal.FindQuest(id, (int)talkingTo.NPCInfo.QuestFaction);
				if (quest.CurrentState == Quest.QuestState.Completed)
				{
					return true;
				}
				break;
			}
			case DialogRequirementQuestStatus.QuestStatuses.CanReceive:
			{
				Quest quest2 = player.QuestJournal.FindLatestNonSharedQuest(id);
				if (quest2 == null)
				{
					return true;
				}
				if (quest2.CurrentState == Quest.QuestState.Completed)
				{
					int num = (int)(GameManager.Instance.World.worldTime / 24000UL);
					int num2 = (int)(quest2.FinishTime / 24000UL);
					if (num != num2)
					{
						return true;
					}
				}
				break;
			}
			case DialogRequirementQuestStatus.QuestStatuses.CannotReceive:
			{
				Quest quest3 = player.QuestJournal.FindLatestNonSharedQuest(id);
				if (quest3 != null)
				{
					if (quest3.CurrentState != Quest.QuestState.Completed)
					{
						return true;
					}
					int num3 = (int)(GameManager.Instance.World.worldTime / 24000UL);
					int num4 = (int)(quest3.FinishTime / 24000UL);
					if (num3 == num4)
					{
						return true;
					}
				}
				break;
			}
			}
		}
		return false;
	}

	// Token: 0x020002B3 RID: 691
	[PublicizedFrom(EAccessModifier.Private)]
	public enum QuestStatuses
	{
		// Token: 0x04000CCA RID: 3274
		NotStarted,
		// Token: 0x04000CCB RID: 3275
		InProgress,
		// Token: 0x04000CCC RID: 3276
		TurnInReady,
		// Token: 0x04000CCD RID: 3277
		Completed,
		// Token: 0x04000CCE RID: 3278
		CanReceive,
		// Token: 0x04000CCF RID: 3279
		CannotReceive
	}
}
