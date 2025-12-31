using System;
using UnityEngine.Scripting;

// Token: 0x02000C92 RID: 3218
[Preserve]
public class XUiC_DialogWindowGroup : XUiController
{
	// Token: 0x17000A22 RID: 2594
	// (get) Token: 0x0600634D RID: 25421 RVA: 0x002849FE File Offset: 0x00282BFE
	// (set) Token: 0x0600634E RID: 25422 RVA: 0x00284A06 File Offset: 0x00282C06
	public Dialog CurrentDialog { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x0600634F RID: 25423 RVA: 0x00284A0F File Offset: 0x00282C0F
	public override void Init()
	{
		base.Init();
		this.responseWindow = base.GetChildByType<XUiC_DialogResponseList>();
	}

	// Token: 0x06006350 RID: 25424 RVA: 0x00272183 File Offset: 0x00270383
	public override void Update(float _dt)
	{
		base.Update(_dt);
	}

	// Token: 0x06006351 RID: 25425 RVA: 0x00284A24 File Offset: 0x00282C24
	public override void OnOpen()
	{
		base.OnOpen();
		base.xui.Dialog.DialogWindowGroup = this;
		base.xui.playerUI.entityPlayer.OverrideFOV = 30f;
		base.xui.playerUI.entityPlayer.OverrideLookAt = base.xui.Dialog.Respondent.getHeadPosition();
		base.xui.playerUI.windowManager.CloseIfOpen("windowpaging");
		base.xui.playerUI.windowManager.CloseIfOpen("toolbelt");
		this.CurrentDialog = Dialog.DialogList[base.xui.Dialog.Respondent.NPCInfo.DialogID];
		this.CurrentDialog.CurrentOwner = base.xui.Dialog.Respondent;
		if (base.xui.Dialog.ReturnStatement == "" || this.CurrentDialog.CurrentStatement == null)
		{
			this.CurrentDialog.RestartDialog(base.xui.playerUI.entityPlayer);
		}
		else if (base.xui.Dialog.ReturnStatement != "")
		{
			this.CurrentDialog.CurrentStatement = this.CurrentDialog.GetStatement(base.xui.Dialog.ReturnStatement);
			this.CurrentDialog.ChildDialog = null;
		}
		base.xui.Dialog.ReturnStatement = "";
		this.responseWindow.CurrentDialog = this.CurrentDialog;
		GameManager.Instance.SetToolTipPause(base.xui.playerUI.nguiWindowManager, true);
	}

	// Token: 0x06006352 RID: 25426 RVA: 0x00284BDC File Offset: 0x00282DDC
	public override void OnClose()
	{
		base.OnClose();
		base.xui.playerUI.windowManager.CloseIfOpen("questOffer");
		base.xui.playerUI.windowManager.OpenIfNotOpen("toolbelt", false, false, true);
		base.xui.Dialog.Respondent = null;
		GameManager.Instance.SetToolTipPause(base.xui.playerUI.nguiWindowManager, false);
		if (!base.xui.Dialog.keepZoomOnClose)
		{
			base.xui.playerUI.entityPlayer.OverrideFOV = -1f;
		}
	}

	// Token: 0x06006353 RID: 25427 RVA: 0x00284C7E File Offset: 0x00282E7E
	public void RefreshDialog()
	{
		if (this.CurrentDialog.CurrentStatement != null)
		{
			this.responseWindow.Refresh();
			return;
		}
		base.xui.playerUI.windowManager.Close("dialog");
	}

	// Token: 0x06006354 RID: 25428 RVA: 0x00284CB3 File Offset: 0x00282EB3
	public void ShowResponseWindow(bool isVisible)
	{
		this.responseWindow.ViewComponent.IsVisible = isVisible;
	}

	// Token: 0x04004ACF RID: 19151
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DialogResponseList responseWindow;
}
