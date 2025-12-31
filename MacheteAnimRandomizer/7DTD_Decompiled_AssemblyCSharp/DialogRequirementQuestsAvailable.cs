using System;
using UnityEngine.Scripting;

// Token: 0x020002B1 RID: 689
[Preserve]
public class DialogRequirementQuestsAvailable : BaseDialogRequirement
{
	// Token: 0x1700021A RID: 538
	// (get) Token: 0x0600135C RID: 4956 RVA: 0x000282C0 File Offset: 0x000264C0
	public override BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.QuestsAvailable;
		}
	}

	// Token: 0x0600135D RID: 4957 RVA: 0x0002B133 File Offset: 0x00029333
	public override string GetRequiredDescription(EntityPlayer player)
	{
		return "";
	}

	// Token: 0x0600135E RID: 4958 RVA: 0x00076948 File Offset: 0x00074B48
	public override bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(GameManager.Instance.World.GetPrimaryPlayer());
		EntityTrader entityTrader = uiforPlayer.xui.Dialog.Respondent as EntityTrader;
		bool result = false;
		if (entityTrader.activeQuests != null)
		{
			int currentFactionTier = uiforPlayer.entityPlayer.QuestJournal.GetCurrentFactionTier(entityTrader.NPCInfo.QuestFaction, 0, false);
			for (int i = 0; i < entityTrader.activeQuests.Count; i++)
			{
				if (entityTrader.activeQuests[i].QuestClass.QuestType == base.Value && (int)entityTrader.activeQuests[i].QuestClass.DifficultyTier <= currentFactionTier && (entityTrader.activeQuests[i].QuestClass.Repeatable || uiforPlayer.entityPlayer.QuestJournal.FindActiveOrCompleteQuest(entityTrader.activeQuests[i].ID, (int)entityTrader.NPCInfo.QuestFaction) == null))
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}
}
