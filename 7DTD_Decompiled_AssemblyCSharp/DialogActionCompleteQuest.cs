using System;
using UnityEngine.Scripting;

// Token: 0x020002A0 RID: 672
[Preserve]
public class DialogActionCompleteQuest : BaseDialogAction
{
	// Token: 0x17000205 RID: 517
	// (get) Token: 0x0600130C RID: 4876 RVA: 0x00075C39 File Offset: 0x00073E39
	public override BaseDialogAction.ActionTypes ActionType
	{
		get
		{
			return BaseDialogAction.ActionTypes.CompleteQuest;
		}
	}

	// Token: 0x0600130D RID: 4877 RVA: 0x00075C3C File Offset: 0x00073E3C
	public override void PerformAction(EntityPlayer player)
	{
		if (base.ID != "")
		{
			Quest quest = player.QuestJournal.FindQuest(base.ID, (int)base.OwnerDialog.CurrentOwner.NPCInfo.QuestFaction);
			Convert.ToInt32(base.Value);
			if (quest != null && quest.Active)
			{
				QuestEventManager.Current.NPCInteracted(base.OwnerDialog.CurrentOwner);
				quest.RefreshQuestCompletion(QuestClass.CompletionTypes.TurnIn, null, true, null);
			}
		}
	}
}
