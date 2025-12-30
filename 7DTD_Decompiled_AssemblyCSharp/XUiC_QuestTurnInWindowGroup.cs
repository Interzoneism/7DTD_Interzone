using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DCA RID: 3530
[Preserve]
public class XUiC_QuestTurnInWindowGroup : XUiController
{
	// Token: 0x06006E7C RID: 28284 RVA: 0x002D1647 File Offset: 0x002CF847
	public override void Init()
	{
		base.Init();
		this.nonPagingHeaderWindow = base.GetChildByType<XUiC_WindowNonPagingHeader>();
		this.detailsWindow = base.GetChildByType<XUiC_QuestTurnInDetailsWindow>();
		this.rewardsWindow = base.GetChildByType<XUiC_QuestTurnInRewardsWindow>();
	}

	// Token: 0x06006E7D RID: 28285 RVA: 0x002D1674 File Offset: 0x002CF874
	public override void OnOpen()
	{
		if (base.xui.Dialog.Respondent != null)
		{
			this.NPC = base.xui.Dialog.Respondent;
		}
		else
		{
			this.NPC = base.xui.Trader.TraderEntity;
		}
		this.detailsWindow.NPC = (this.rewardsWindow.NPC = this.NPC);
		XUiC_ItemInfoWindow childByType = base.xui.GetChildByType<XUiC_ItemInfoWindow>();
		this.rewardsWindow.InfoWindow = childByType;
		base.OnOpen();
		base.xui.playerUI.entityPlayer.OverrideFOV = 30f;
		base.xui.playerUI.entityPlayer.OverrideLookAt = this.NPC.getHeadPosition();
		GUIWindowManager windowManager = base.xui.playerUI.windowManager;
		if (this.nonPagingHeaderWindow != null)
		{
			this.nonPagingHeaderWindow.SetHeader("QUEST COMPLETE");
		}
		windowManager.CloseIfOpen("windowpaging");
		base.xui.dragAndDrop.InMenu = true;
	}

	// Token: 0x06006E7E RID: 28286 RVA: 0x002D1784 File Offset: 0x002CF984
	public override void OnClose()
	{
		base.OnClose();
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		if (GameManager.Instance.World == null)
		{
			return;
		}
		EntityTrader entityTrader = base.xui.Trader.TraderEntity as EntityTrader;
		if (entityTrader != null)
		{
			GameManager.Instance.StartCoroutine(this.startTrading(entityTrader, entityPlayer));
			return;
		}
		if (Vector3.Distance(base.xui.Dialog.Respondent.position, entityPlayer.position) > 5f)
		{
			base.xui.Dialog.Respondent = null;
			base.xui.playerUI.entityPlayer.OverrideFOV = -1f;
			return;
		}
		base.xui.playerUI.windowManager.Open("dialog", true, false, true);
	}

	// Token: 0x06006E7F RID: 28287 RVA: 0x002D1858 File Offset: 0x002CFA58
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator startTrading(EntityTrader trader, EntityPlayer player)
	{
		yield return null;
		trader.StartTrading(player);
		yield break;
	}

	// Token: 0x06006E80 RID: 28288 RVA: 0x002D1870 File Offset: 0x002CFA70
	public void TryNextComplete()
	{
		Quest nextCompletedQuest = base.xui.playerUI.entityPlayer.QuestJournal.GetNextCompletedQuest(base.xui.Dialog.QuestTurnIn, this.NPC.entityId);
		if (nextCompletedQuest == null)
		{
			base.xui.playerUI.windowManager.CloseAllOpenWindows(null, false);
			return;
		}
		base.xui.Dialog.QuestTurnIn = nextCompletedQuest;
		this.detailsWindow.CurrentQuest = (this.rewardsWindow.CurrentQuest = nextCompletedQuest);
		base.RefreshBindings(false);
	}

	// Token: 0x04005400 RID: 21504
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestTurnInDetailsWindow detailsWindow;

	// Token: 0x04005401 RID: 21505
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestTurnInRewardsWindow rewardsWindow;

	// Token: 0x04005402 RID: 21506
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WindowNonPagingHeader nonPagingHeaderWindow;

	// Token: 0x04005403 RID: 21507
	public static string ID = "questTurnIn";

	// Token: 0x04005404 RID: 21508
	public EntityNPC NPC;
}
