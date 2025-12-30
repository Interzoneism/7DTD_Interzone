using System;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D0E RID: 3342
[Preserve]
public class XUiC_MainMenuButtons : XUiController
{
	// Token: 0x06006808 RID: 26632 RVA: 0x002A2E0C File Offset: 0x002A100C
	public override void Init()
	{
		base.Init();
		if (base.TryGetChildByIdAndType<XUiC_SimpleButton>("btnNewGame", out this.btnNewGame))
		{
			this.btnNewGame.OnPressed += this.btnNewGame_OnPressed;
		}
		if (base.TryGetChildByIdAndType<XUiC_SimpleButton>("btnContinueGame", out this.btnContinueGame))
		{
			this.btnContinueGame.OnPressed += this.btnContinueGame_OnPressed;
		}
		XUiC_SimpleButton xuiC_SimpleButton;
		if (base.TryGetChildByIdAndType<XUiC_SimpleButton>("btnConnectToServer", out xuiC_SimpleButton))
		{
			xuiC_SimpleButton.OnPressed += this.btnConnectToServer_OnPressed;
		}
		XUiC_SimpleButton xuiC_SimpleButton2;
		if (base.TryGetChildByIdAndType<XUiC_SimpleButton>("btnOptions", out xuiC_SimpleButton2))
		{
			xuiC_SimpleButton2.OnPressed += this.btnOptions_OnPressed;
		}
		XUiC_SimpleButton xuiC_SimpleButton3;
		if (base.TryGetChildByIdAndType<XUiC_SimpleButton>("btnCredits", out xuiC_SimpleButton3))
		{
			xuiC_SimpleButton3.OnPressed += this.btnCredits_OnPressed;
		}
		XUiC_SimpleButton xuiC_SimpleButton4;
		if (base.TryGetChildByIdAndType<XUiC_SimpleButton>("btnNews", out xuiC_SimpleButton4))
		{
			xuiC_SimpleButton4.OnPressed += this.btnNews_OnPressed;
		}
		XUiC_SimpleButton xuiC_SimpleButton5;
		if (base.TryGetChildByIdAndType<XUiC_SimpleButton>("btnQuit", out xuiC_SimpleButton5))
		{
			xuiC_SimpleButton5.OnPressed += this.btnQuit_OnPressed;
		}
		XUiC_SimpleButton xuiC_SimpleButton6;
		if (base.TryGetChildByIdAndType<XUiC_SimpleButton>("btnEditingTools", out xuiC_SimpleButton6))
		{
			xuiC_SimpleButton6.OnPressed += this.btnEditingTools_OnPressed;
		}
		XUiC_SimpleButton xuiC_SimpleButton7;
		if (base.TryGetChildByIdAndType<XUiC_SimpleButton>("btnRWG", out xuiC_SimpleButton7))
		{
			xuiC_SimpleButton7.OnPressed += this.btnRWG_OnPressed;
		}
		XUiC_SimpleButton xuiC_SimpleButton8;
		if (base.TryGetChildByIdAndType<XUiC_SimpleButton>("btnDlc", out xuiC_SimpleButton8))
		{
			xuiC_SimpleButton8.OnPressed += this.btnDlc_OnPressed;
		}
	}

	// Token: 0x06006809 RID: 26633 RVA: 0x002A2F7F File Offset: 0x002A117F
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnNewGame_OnPressed(XUiController _sender, int _mouseButton)
	{
		if (GameManager.HasAcceptedLatestEula())
		{
			XUiC_NewContinueGame.SetIsContinueGame(base.xui, false);
			this.CheckProfile(XUiC_NewContinueGame.ID);
			return;
		}
		XUiC_EulaWindow.Open(base.xui, false);
	}

	// Token: 0x0600680A RID: 26634 RVA: 0x002A2FAC File Offset: 0x002A11AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnContinueGame_OnPressed(XUiController _sender, int _mouseButton)
	{
		if (GameManager.HasAcceptedLatestEula())
		{
			XUiC_NewContinueGame.SetIsContinueGame(base.xui, true);
			this.CheckProfile(XUiC_NewContinueGame.ID);
			return;
		}
		XUiC_EulaWindow.Open(base.xui, false);
	}

	// Token: 0x0600680B RID: 26635 RVA: 0x002A2FDC File Offset: 0x002A11DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnConnectToServer_OnPressed(XUiController _sender, int _mouseButton)
	{
		if (!GameManager.HasAcceptedLatestEula())
		{
			XUiC_EulaWindow.Open(base.xui, false);
			return;
		}
		if (this.wdwMultiplayerPrivileges == null)
		{
			this.wdwMultiplayerPrivileges = XUiC_MultiplayerPrivilegeNotification.GetWindow();
		}
		XUiC_MultiplayerPrivilegeNotification xuiC_MultiplayerPrivilegeNotification = this.wdwMultiplayerPrivileges;
		if (xuiC_MultiplayerPrivilegeNotification == null)
		{
			return;
		}
		xuiC_MultiplayerPrivilegeNotification.ResolvePrivilegesWithDialog(EUserPerms.Multiplayer, delegate(bool result)
		{
			if (PermissionsManager.IsMultiplayerAllowed())
			{
				this.CheckProfile(XUiC_ServerBrowser.ID);
			}
		}, -1f, false, null);
	}

	// Token: 0x0600680C RID: 26636 RVA: 0x002A3038 File Offset: 0x002A1238
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnEditingTools_OnPressed(XUiController _sender, int _mouseButton)
	{
		if (GameManager.HasAcceptedLatestEula())
		{
			base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
			base.xui.playerUI.windowManager.Open(XUiC_EditingTools.ID, true, false, true);
			return;
		}
		XUiC_EulaWindow.Open(base.xui, false);
	}

	// Token: 0x0600680D RID: 26637 RVA: 0x002A3098 File Offset: 0x002A1298
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnRWG_OnPressed(XUiController _sender, int _mouseButton)
	{
		if (!GameManager.HasAcceptedLatestEula())
		{
			XUiC_EulaWindow.Open(base.xui, false);
			return;
		}
		base.xui.FindWindowGroupByName("rwgeditor").GetChildByType<XUiC_WorldGenerationWindowGroup>().LastWindowID = XUiC_MainMenu.ID;
		base.xui.playerUI.windowManager.Open("rwgeditor", true, false, true);
	}

	// Token: 0x0600680E RID: 26638 RVA: 0x002A30F5 File Offset: 0x002A12F5
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnOptions_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		base.xui.playerUI.windowManager.Open(XUiC_OptionsMenu.ID, true, false, true);
	}

	// Token: 0x0600680F RID: 26639 RVA: 0x002A3134 File Offset: 0x002A1334
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnCredits_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		base.xui.playerUI.windowManager.Open(XUiC_Credits.ID, true, false, true);
	}

	// Token: 0x06006810 RID: 26640 RVA: 0x002A3173 File Offset: 0x002A1373
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnNews_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		XUiC_NewsScreen.Open(base.xui);
	}

	// Token: 0x06006811 RID: 26641 RVA: 0x002A31A0 File Offset: 0x002A13A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnDlc_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup, false);
		base.xui.playerUI.windowManager.Open(XUiC_DlcWindow.ID, true, false, true);
	}

	// Token: 0x06006812 RID: 26642 RVA: 0x00073CE7 File Offset: 0x00071EE7
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnQuit_OnPressed(XUiController _sender, int _mouseButton)
	{
		Application.Quit();
	}

	// Token: 0x06006813 RID: 26643 RVA: 0x002A31DC File Offset: 0x002A13DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckProfile(string _windowToOpen)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		if (ProfileSDF.CurrentProfileName().Length == 0)
		{
			XUiC_OptionsProfiles.Open(base.xui, delegate
			{
				this.xui.playerUI.windowManager.Open(_windowToOpen, true, false, true);
			});
			return;
		}
		base.xui.playerUI.windowManager.Open(_windowToOpen, true, false, true);
	}

	// Token: 0x06006814 RID: 26644 RVA: 0x002A325F File Offset: 0x002A145F
	public override void Update(float _dt)
	{
		base.Update(_dt);
		base.RefreshBindings(false);
		if (this.btnNewGame != null || this.btnContinueGame != null)
		{
			this.DoLoadSaveGameAutomation();
		}
	}

	// Token: 0x06006815 RID: 26645 RVA: 0x002A3288 File Offset: 0x002A1488
	[PublicizedFrom(EAccessModifier.Private)]
	public void DoLoadSaveGameAutomation()
	{
		EPlatformLoadSaveGameState loadSaveGameState = PlatformApplicationManager.GetLoadSaveGameState();
		if (loadSaveGameState != EPlatformLoadSaveGameState.NewGameOpen)
		{
			if (loadSaveGameState != EPlatformLoadSaveGameState.ContinueGameOpen)
			{
				return;
			}
			if (!this.btnContinueGame.Enabled)
			{
				PlatformApplicationManager.SetFailedLoadSaveGame();
				return;
			}
			this.btnContinueGame_OnPressed(this.btnContinueGame, -1);
			if (!base.xui.playerUI.windowManager.IsWindowOpen(XUiC_NewContinueGame.ID))
			{
				PlatformApplicationManager.SetFailedLoadSaveGame();
				return;
			}
			PlatformApplicationManager.AdvanceLoadSaveGameStateFrom(loadSaveGameState);
			return;
		}
		else
		{
			if (!this.btnNewGame.Enabled)
			{
				PlatformApplicationManager.SetFailedLoadSaveGame();
				return;
			}
			this.btnNewGame_OnPressed(this.btnNewGame, -1);
			if (!base.xui.playerUI.windowManager.IsWindowOpen(XUiC_NewContinueGame.ID))
			{
				PlatformApplicationManager.SetFailedLoadSaveGame();
				return;
			}
			PlatformApplicationManager.AdvanceLoadSaveGameStateFrom(loadSaveGameState);
			return;
		}
	}

	// Token: 0x06006816 RID: 26646 RVA: 0x002A3338 File Offset: 0x002A1538
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "has_saved_game")
		{
			_value = this.anySaveFilesExist.ToString();
			return true;
		}
		if (!(_bindingName == "online_mode"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		IPlatform multiPlatform = PlatformManager.MultiPlatform;
		EUserStatus? euserStatus;
		if (multiPlatform == null)
		{
			euserStatus = null;
		}
		else
		{
			IUserClient user = multiPlatform.User;
			euserStatus = ((user != null) ? new EUserStatus?(user.UserStatus) : null);
		}
		EUserStatus? euserStatus2 = euserStatus;
		_value = (euserStatus2.GetValueOrDefault() != EUserStatus.OfflineMode).ToString();
		return true;
	}

	// Token: 0x06006817 RID: 26647 RVA: 0x002A33C4 File Offset: 0x002A15C4
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.btnContinueGame != null)
		{
			this.anySaveFilesExist = (GameIO.GetPlayerSaves(null, false) > 0);
		}
		XUiC_SimpleButton xuiC_SimpleButton = this.btnNewGame;
		if (xuiC_SimpleButton == null)
		{
			return;
		}
		xuiC_SimpleButton.SelectCursorElement(true, false);
	}

	// Token: 0x04004E73 RID: 20083
	[PublicizedFrom(EAccessModifier.Private)]
	public bool anySaveFilesExist;

	// Token: 0x04004E74 RID: 20084
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnNewGame;

	// Token: 0x04004E75 RID: 20085
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnContinueGame;

	// Token: 0x04004E76 RID: 20086
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_MultiplayerPrivilegeNotification wdwMultiplayerPrivileges;
}
