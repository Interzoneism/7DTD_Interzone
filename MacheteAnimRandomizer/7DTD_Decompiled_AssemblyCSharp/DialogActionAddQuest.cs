using System;
using UnityEngine.Scripting;

// Token: 0x0200029E RID: 670
[Preserve]
public class DialogActionAddQuest : BaseDialogAction
{
	// Token: 0x17000204 RID: 516
	// (get) Token: 0x06001307 RID: 4871 RVA: 0x000282C0 File Offset: 0x000264C0
	public override BaseDialogAction.ActionTypes ActionType
	{
		get
		{
			return BaseDialogAction.ActionTypes.AddQuest;
		}
	}

	// Token: 0x06001308 RID: 4872 RVA: 0x00075AEC File Offset: 0x00073CEC
	public override void PerformAction(EntityPlayer player)
	{
		if (this.Quest != null)
		{
			QuestClass questClass = this.Quest.QuestClass;
			if (questClass != null)
			{
				Quest quest = player.QuestJournal.FindNonSharedQuest(this.Quest.QuestCode);
				if (quest == null || (questClass.Repeatable && !quest.Active))
				{
					LocalPlayerUI playerUI = LocalPlayerUI.GetUIForPlayer(player as EntityPlayerLocal);
					XUiC_QuestOfferWindow.OpenQuestOfferWindow(playerUI.xui, this.Quest, this.ListIndex, XUiC_QuestOfferWindow.OfferTypes.Dialog, playerUI.xui.Dialog.Respondent.entityId, delegate(EntityNPC npc)
					{
						playerUI.xui.Dialog.Respondent = npc;
						playerUI.xui.Dialog.ReturnStatement = ((DialogResponseQuest)this.Owner).LastStatementID;
						playerUI.windowManager.Open("dialog", true, false, true);
					});
					return;
				}
				GameManager.ShowTooltip((EntityPlayerLocal)player, Localization.Get("questunavailable", false), false, false, 0f);
			}
		}
	}

	// Token: 0x04000C8C RID: 3212
	[PublicizedFrom(EAccessModifier.Private)]
	public string name = "";

	// Token: 0x04000C8D RID: 3213
	public Quest Quest;

	// Token: 0x04000C8E RID: 3214
	public int ListIndex;
}
