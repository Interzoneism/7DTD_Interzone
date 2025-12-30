using System;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D07 RID: 3335
[Preserve]
public class XUiC_LoginStandalone : XUiController
{
	// Token: 0x060067B9 RID: 26553 RVA: 0x002A0E1C File Offset: 0x0029F01C
	public override void Init()
	{
		base.Init();
		XUiC_LoginStandalone.ID = base.WindowGroup.ID;
		this.btnRetry = (XUiC_SimpleButton)base.GetChildById("btnRetry");
		this.btnRetry.OnPressed += this.BtnRetry_OnPressed;
		this.btnOffline = (XUiC_SimpleButton)base.GetChildById("btnOffline");
		this.btnOffline.OnPressed += this.BtnOffline_OnPressed;
		this.btnExit = (XUiC_SimpleButton)base.GetChildById("btnExit");
		this.btnExit.OnPressed += this.BtnExit_OnPressed;
	}

	// Token: 0x060067BA RID: 26554 RVA: 0x002A0EC8 File Offset: 0x0029F0C8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "title")
		{
			_value = ((this.offendingPlatform == null) ? "" : string.Format(Localization.Get("xuiSteamLogin", false), this.offendingPlatform.PlatformDisplayName));
			return true;
		}
		if (_bindingName == "caption")
		{
			_value = ((this.offendingPlatform == null) ? "" : string.Format(Localization.Get("xuiSteamLoginFailure", false), this.offendingPlatform.PlatformDisplayName));
			return true;
		}
		if (!(_bindingName == "reason"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = ((this.offendingPlatform == null) ? "" : string.Format(Localization.Get("xuiSteamLoginReason" + this.statusReason.ToStringCached<EApiStatusReason>(), false), this.offendingPlatform.PlatformDisplayName, this.statusReasonAdditionalText));
		return true;
	}

	// Token: 0x060067BB RID: 26555 RVA: 0x002A0FAC File Offset: 0x0029F1AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnRetry_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.btnOffline.Enabled = false;
		this.btnRetry.Enabled = false;
		this.btnExit.Enabled = false;
		this.offendingPlatform = null;
		base.RefreshBindings(false);
		_sender.xui.playerUI.nguiWindowManager.SetLabelText(EnumNGUIWindow.Loading, Localization.Get("xuiSteamLoginProgressSignIn", false) + "...", false);
		PlatformManager.MultiPlatform.User.Login(new LoginUserCallback(this.updateState));
	}

	// Token: 0x060067BC RID: 26556 RVA: 0x002A1034 File Offset: 0x0029F234
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnOffline_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.btnOffline.Enabled = false;
		this.btnRetry.Enabled = false;
		this.btnExit.Enabled = false;
		this.offendingPlatform = null;
		base.RefreshBindings(false);
		this.wantOffline = true;
		PlatformManager.MultiPlatform.User.PlayOffline(new LoginUserCallback(this.updateState));
	}

	// Token: 0x060067BD RID: 26557 RVA: 0x00073CE7 File Offset: 0x00071EE7
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnExit_OnPressed(XUiController _sender, int _mouseButton)
	{
		Application.Quit();
	}

	// Token: 0x060067BE RID: 26558 RVA: 0x002A1098 File Offset: 0x0029F298
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateState(IPlatform _platform, EApiStatusReason _statusReason, string _statusReasonAdditionalText)
	{
		if (_platform.User.UserStatus == EUserStatus.LoggedIn || (this.wantOffline && _platform.User.UserStatus == EUserStatus.OfflineMode))
		{
			base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
			Action action = this.onLoginComplete;
			if (action != null)
			{
				action();
			}
			this.onLoginComplete = null;
			return;
		}
		base.xui.playerUI.windowManager.Open(XUiC_LoginStandalone.ID, true, true, true);
		this.btnRetry.Enabled = (_platform.Api.ClientApiStatus != EApiStatus.PermanentError && !this.wantOffline);
		this.btnOffline.Enabled = (_platform.User.UserStatus == EUserStatus.OfflineMode);
		this.btnExit.Enabled = true;
		this.offendingPlatform = _platform;
		this.statusReason = _statusReason;
		this.statusReasonAdditionalText = _statusReasonAdditionalText;
		base.RefreshBindings(false);
	}

	// Token: 0x060067BF RID: 26559 RVA: 0x002A1184 File Offset: 0x0029F384
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Open(XUi _xuiInstance, IPlatform _platform, EApiStatusReason _statusReason, string _statusReasonAdditionalText, Action _onLoginComplete)
	{
		XUiC_LoginStandalone childByType = _xuiInstance.FindWindowGroupByName(XUiC_LoginStandalone.ID).GetChildByType<XUiC_LoginStandalone>();
		childByType.onLoginComplete = _onLoginComplete;
		childByType.updateState(_platform, _statusReason, _statusReasonAdditionalText);
	}

	// Token: 0x060067C0 RID: 26560 RVA: 0x002A11A8 File Offset: 0x0029F3A8
	public static void Login(XUi _xuiInstance, Action _onLoginComplete)
	{
		_xuiInstance.playerUI.nguiWindowManager.SetLabelText(EnumNGUIWindow.Loading, Localization.Get("xuiSteamLoginProgressSignIn", false) + "...", false);
		PlatformManager.MultiPlatform.User.Login(delegate(IPlatform _platform, EApiStatusReason _statusReason, string _statusReasonAdditionalText)
		{
			if (_platform.Api.ClientApiStatus != EApiStatus.Ok)
			{
				XUiC_LoginStandalone.Open(_xuiInstance, _platform, _statusReason, _statusReasonAdditionalText, _onLoginComplete);
				return;
			}
			if (_platform.User.UserStatus != EUserStatus.LoggedIn || _statusReason != EApiStatusReason.Ok)
			{
				XUiC_LoginStandalone.Open(_xuiInstance, _platform, _statusReason, _statusReasonAdditionalText, _onLoginComplete);
				return;
			}
			_onLoginComplete();
		});
	}

	// Token: 0x04004E42 RID: 20034
	[PublicizedFrom(EAccessModifier.Private)]
	public static string ID = "";

	// Token: 0x04004E43 RID: 20035
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnRetry;

	// Token: 0x04004E44 RID: 20036
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnOffline;

	// Token: 0x04004E45 RID: 20037
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnExit;

	// Token: 0x04004E46 RID: 20038
	[PublicizedFrom(EAccessModifier.Private)]
	public IPlatform offendingPlatform;

	// Token: 0x04004E47 RID: 20039
	[PublicizedFrom(EAccessModifier.Private)]
	public EApiStatusReason statusReason;

	// Token: 0x04004E48 RID: 20040
	[PublicizedFrom(EAccessModifier.Private)]
	public string statusReasonAdditionalText;

	// Token: 0x04004E49 RID: 20041
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wantOffline;

	// Token: 0x04004E4A RID: 20042
	[PublicizedFrom(EAccessModifier.Private)]
	public Action onLoginComplete;
}
