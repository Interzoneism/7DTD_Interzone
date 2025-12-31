using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000DE3 RID: 3555
[Preserve]
public class XUiC_ReportPlayer : XUiController
{
	// Token: 0x06006F84 RID: 28548 RVA: 0x002D86D0 File Offset: 0x002D68D0
	public static void Open(PlayerData _reportedPlayerData, string _windowOnClose = "")
	{
		if (LocalPlayerUI.primaryUI.windowManager.IsWindowOpen(XUiC_ReportPlayer.ID))
		{
			return;
		}
		XUiC_ReportPlayer childByType = ((XUiWindowGroup)LocalPlayerUI.primaryUI.windowManager.GetWindow(XUiC_ReportPlayer.ID)).Controller.GetChildByType<XUiC_ReportPlayer>();
		childByType.reportedPlayerData = _reportedPlayerData;
		childByType.windowOnClose = _windowOnClose;
		XUiC_ReportPlayer.initialText = "";
		LocalPlayerUI.primaryUI.windowManager.Open(XUiC_ReportPlayer.ID, true, false, true);
	}

	// Token: 0x06006F85 RID: 28549 RVA: 0x002D8745 File Offset: 0x002D6945
	public static void Open(PlayerData _reportedPlayerData, EnumReportCategory _initialCategory, string _intialText, string _windowOnClose = "")
	{
		XUiC_ReportPlayer.initialCategory = _initialCategory;
		XUiC_ReportPlayer.initialText = _intialText;
		XUiC_ReportPlayer.Open(_reportedPlayerData, _windowOnClose);
	}

	// Token: 0x06006F86 RID: 28550 RVA: 0x002D875C File Offset: 0x002D695C
	public override void Init()
	{
		base.Init();
		XUiC_ReportPlayer.ID = base.WindowGroup.ID;
		this.lblReportedPlayer = (XUiV_Label)base.GetChildById("lblReportedPlayer").ViewComponent;
		this.cbxCategory = (XUiC_ComboBoxList<IPlayerReporting.PlayerReportCategory>)base.GetChildById("cbxCategory");
		IPlayerReporting playerReporting = PlatformManager.MultiPlatform.PlayerReporting;
		IList<IPlayerReporting.PlayerReportCategory> list = (playerReporting != null) ? playerReporting.ReportCategories() : null;
		if (list != null)
		{
			foreach (IPlayerReporting.PlayerReportCategory item in list)
			{
				this.cbxCategory.Elements.Add(item);
			}
		}
		this.txtMessage = (XUiC_TextInput)base.GetChildById("txtMessage");
		this.txtMessage.OnInputErrorHandler += this.UpdateErrorMessage;
		((XUiC_SimpleButton)base.GetChildById("btnSend")).OnPressed += this.BtnSend_OnPressed;
		((XUiC_SimpleButton)base.GetChildById("btnCancel")).OnPressed += this.BtnCancel_OnPressed;
		this.btnKickPlayer = (XUiC_SimpleButton)base.GetChildById("btnKick");
		this.btnKickPlayer.OnPressed += this.BtnKick_OnPressed;
	}

	// Token: 0x06006F87 RID: 28551 RVA: 0x0028BEAC File Offset: 0x0028A0AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnCancel_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
	}

	// Token: 0x06006F88 RID: 28552 RVA: 0x002D88AC File Offset: 0x002D6AAC
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnKick_OnPressed(XUiController _sender, int _mouseButton)
	{
		if (this.reportedPlayerData != null)
		{
			PlatformUserIdentifierAbs platformUserIdentifierAbs;
			ClientInfo clientInfo;
			if (ConsoleHelper.ParseParamPartialNameOrId(this.reportedPlayerData.PlayerName.Text, out platformUserIdentifierAbs, out clientInfo, true) == 1)
			{
				DateTime maxValue = DateTime.MaxValue;
				string text = "";
				if (clientInfo != null)
				{
					ClientInfo cInfo = clientInfo;
					GameUtils.EKickReason kickReason = GameUtils.EKickReason.ManualKick;
					int apiResponseEnum = 0;
					string customReason = string.IsNullOrEmpty(text) ? "" : text;
					GameUtils.KickPlayerForClientInfo(cInfo, new GameUtils.KickPlayerData(kickReason, apiResponseEnum, maxValue, customReason));
				}
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("[xui] Kick Succeeded");
		}
		else
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("[xui] Failed to find player to kick");
		}
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
	}

	// Token: 0x06006F89 RID: 28553 RVA: 0x002D8950 File Offset: 0x002D6B50
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnSend_OnPressed(XUiController _sender, int _mouseButton)
	{
		if (this.reportedPlayerData == null)
		{
			this.ReportSentMessageBox(false);
			return;
		}
		string text = this.txtMessage.Text;
		if (!string.IsNullOrEmpty(XUiC_ReportPlayer.initialText))
		{
			text = text + "\n" + string.Format(Localization.Get("xuiReportAutomatedMessage", false), XUiC_ReportPlayer.initialText, XUiC_ReportPlayer.initialCategory);
		}
		IPlayerReporting playerReporting = PlatformManager.MultiPlatform.PlayerReporting;
		if (playerReporting != null)
		{
			playerReporting.ReportPlayer(this.reportedPlayerData.PrimaryId, this.cbxCategory.Value, text, new Action<bool>(this.ReportSentMessageBox));
		}
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
	}

	// Token: 0x06006F8A RID: 28554 RVA: 0x002D8A08 File Offset: 0x002D6C08
	[PublicizedFrom(EAccessModifier.Private)]
	public void ReportSentMessageBox(bool _success)
	{
		string text = _success ? Localization.Get("xuiReportPlayerSuccess", false) : Localization.Get("xuiReportPlayerFail", false);
		XUiC_MessageBoxWindowGroup.ShowMessageBox(base.xui, Localization.Get("xuiReportPlayerHeader", false), text, XUiC_MessageBoxWindowGroup.MessageBoxTypes.Ok, null, null, true, true, true);
	}

	// Token: 0x06006F8B RID: 28555 RVA: 0x002D8A50 File Offset: 0x002D6C50
	public override void OnOpen()
	{
		base.OnOpen();
		this.txtMessage.Text = XUiC_ReportPlayer.initialText;
		this.inputErrorMessage = null;
		XUiC_ComboBoxList<IPlayerReporting.PlayerReportCategory> xuiC_ComboBoxList = this.cbxCategory;
		List<IPlayerReporting.PlayerReportCategory> elements = this.cbxCategory.Elements;
		IPlayerReporting playerReporting = PlatformManager.MultiPlatform.PlayerReporting;
		xuiC_ComboBoxList.SelectedIndex = elements.IndexOf((playerReporting != null) ? playerReporting.GetPlayerReportCategoryMapping(XUiC_ReportPlayer.initialCategory) : null);
		if (this.cbxCategory.SelectedIndex == -1)
		{
			this.cbxCategory.SelectedIndex = 0;
		}
		if (this.reportedPlayerData != null)
		{
			this.lblReportedPlayer.Text = GeneratedTextManager.GetDisplayTextImmediately(this.reportedPlayerData.PlayerName, false, GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes);
			PersistentPlayerList persistentPlayers = GameManager.Instance.persistentPlayers;
			int? num;
			if (persistentPlayers == null)
			{
				num = null;
			}
			else
			{
				PersistentPlayerData playerData = persistentPlayers.GetPlayerData(this.reportedPlayerData.PrimaryId);
				num = ((playerData != null) ? new int?(playerData.EntityId) : null);
			}
			int num2 = num ?? -1;
			this.btnKickPlayer.IsVisible = (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && num2 != -1);
		}
		else
		{
			Log.Out("Sign does not have an author, cannot report player.");
			base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		}
		base.RefreshBindings(false);
	}

	// Token: 0x06006F8C RID: 28556 RVA: 0x002D8B9E File Offset: 0x002D6D9E
	public override void OnClose()
	{
		base.OnClose();
		this.reportedPlayerData = null;
		if (!string.IsNullOrEmpty(this.windowOnClose))
		{
			LocalPlayerUI.primaryUI.windowManager.Open(this.windowOnClose, true, false, true);
		}
	}

	// Token: 0x06006F8D RID: 28557 RVA: 0x002D8BD2 File Offset: 0x002D6DD2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "inputWarning")
		{
			_value = this.inputErrorMessage;
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x06006F8E RID: 28558 RVA: 0x002D8BF3 File Offset: 0x002D6DF3
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateErrorMessage(XUiController _sender, string _errorMessage)
	{
		this.inputErrorMessage = _errorMessage;
		base.RefreshBindings(false);
	}

	// Token: 0x040054A3 RID: 21667
	public static string ID;

	// Token: 0x040054A4 RID: 21668
	[PublicizedFrom(EAccessModifier.Private)]
	public static string initialText = "";

	// Token: 0x040054A5 RID: 21669
	[PublicizedFrom(EAccessModifier.Private)]
	public static EnumReportCategory initialCategory = EnumReportCategory.None;

	// Token: 0x040054A6 RID: 21670
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblReportedPlayer;

	// Token: 0x040054A7 RID: 21671
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<IPlayerReporting.PlayerReportCategory> cbxCategory;

	// Token: 0x040054A8 RID: 21672
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtMessage;

	// Token: 0x040054A9 RID: 21673
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnKickPlayer;

	// Token: 0x040054AA RID: 21674
	[PublicizedFrom(EAccessModifier.Private)]
	public string inputErrorMessage;

	// Token: 0x040054AB RID: 21675
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerData reportedPlayerData;

	// Token: 0x040054AC RID: 21676
	[PublicizedFrom(EAccessModifier.Private)]
	public string windowOnClose;
}
