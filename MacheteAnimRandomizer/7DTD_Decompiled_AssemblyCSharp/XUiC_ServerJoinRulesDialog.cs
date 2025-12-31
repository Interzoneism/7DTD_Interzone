using System;
using UnityEngine.Scripting;

// Token: 0x02000E17 RID: 3607
[Preserve]
public class XUiC_ServerJoinRulesDialog : XUiController
{
	// Token: 0x060070ED RID: 28909 RVA: 0x002E0910 File Offset: 0x002DEB10
	public override void Init()
	{
		base.Init();
		XUiC_ServerJoinRulesDialog.ID = base.WindowGroup.ID;
		this.labelConfirmationText = (XUiV_Label)base.GetChildById("labelConfirmationText").ViewComponent;
		((XUiC_SimpleButton)base.GetChildById("btnSpawn")).OnPressed += this.BtnSpawn_OnPressed;
		((XUiC_SimpleButton)base.GetChildById("btnLeave")).OnPressed += this.BtnLeave_OnPressed;
	}

	// Token: 0x060070EE RID: 28910 RVA: 0x002E0990 File Offset: 0x002DEB90
	public override void OnOpen()
	{
		base.OnOpen();
		base.xui.playerUI.CursorController.SetCursorHidden(false);
		base.GetChildById("btnLeave").SelectCursorElement(false, false);
	}

	// Token: 0x060070EF RID: 28911 RVA: 0x002E09C1 File Offset: 0x002DEBC1
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnLeave_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		SingletonMonoBehaviour<ConnectionManager>.Instance.Disconnect();
	}

	// Token: 0x060070F0 RID: 28912 RVA: 0x002E09ED File Offset: 0x002DEBED
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnSpawn_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		GameManager.Instance.DoSpawn();
	}

	// Token: 0x060070F1 RID: 28913 RVA: 0x002E0A1C File Offset: 0x002DEC1C
	public static void Show(LocalPlayerUI _playerUi, string _confirmationText)
	{
		_playerUi.xui.FindWindowGroupByName(XUiC_ServerJoinRulesDialog.ID).GetChildByType<XUiC_ServerJoinRulesDialog>().labelConfirmationText.Text = _confirmationText.Replace("\\n", "\n");
		_playerUi.windowManager.Open(XUiC_ServerJoinRulesDialog.ID, true, true, true);
	}

	// Token: 0x040055CF RID: 21967
	public static string ID = "";

	// Token: 0x040055D0 RID: 21968
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label labelConfirmationText;
}
