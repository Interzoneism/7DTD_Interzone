using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x0200005C RID: 92
[Preserve]
public class XUiC_LoginPS5 : XUiController
{
	// Token: 0x060001AE RID: 430 RVA: 0x0000F6D4 File Offset: 0x0000D8D4
	public override void Init()
	{
		base.Init();
		XUiC_LoginPS5.ID = base.WindowGroup.ID;
		this.btnRetry = (XUiC_SimpleButton)base.GetChildById("btnRetry");
		this.btnRetry.OnPressed += this.BtnRetry_OnPressed;
		this.btnOffline = (XUiC_SimpleButton)base.GetChildById("btnOffline");
		this.btnOffline.OnPressed += this.BtnOffline_OnPressed;
	}

	// Token: 0x060001AF RID: 431 RVA: 0x0000F754 File Offset: 0x0000D954
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "title")
		{
			_value = ((this.offendingPlatform == null) ? "" : string.Format(Localization.Get("xuiPSNLogin", false), this.offendingPlatform.PlatformDisplayName));
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

	// Token: 0x060001B0 RID: 432 RVA: 0x0000F838 File Offset: 0x0000DA38
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnRetry_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.btnOffline.Enabled = false;
		this.btnRetry.Enabled = false;
		this.offendingPlatform = null;
		this.wantOffline = false;
		base.RefreshBindings(false);
		_sender.xui.playerUI.nguiWindowManager.SetLabelText(EnumNGUIWindow.Loading, Localization.Get("xuiSteamLoginProgressSignIn", false) + "...", false);
		PlatformManager.MultiPlatform.User.Login(new LoginUserCallback(this.updateState));
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x0000F8BC File Offset: 0x0000DABC
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnOffline_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.btnOffline.Enabled = false;
		this.btnRetry.Enabled = false;
		this.offendingPlatform = null;
		base.RefreshBindings(false);
		this.wantOffline = true;
		PlatformManager.MultiPlatform.User.PlayOffline(new LoginUserCallback(this.updateState));
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x0000F914 File Offset: 0x0000DB14
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
		base.xui.playerUI.windowManager.Open(XUiC_LoginPS5.ID, true, true, true);
		this.btnRetry.Enabled = (_platform.Api.ClientApiStatus != EApiStatus.PermanentError);
		this.btnOffline.Enabled = (_platform.User.UserStatus == EUserStatus.OfflineMode);
		this.offendingPlatform = _platform;
		this.statusReason = _statusReason;
		this.statusReasonAdditionalText = _statusReasonAdditionalText;
		base.RefreshBindings(false);
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x0000F9EB File Offset: 0x0000DBEB
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Open(XUi _xuiInstance, IPlatform _platform, EApiStatusReason _statusReason, string _statusReasonAdditionalText, Action _onLoginComplete)
	{
		XUiC_LoginPS5 childByType = _xuiInstance.FindWindowGroupByName(XUiC_LoginPS5.ID).GetChildByType<XUiC_LoginPS5>();
		childByType.onLoginComplete = _onLoginComplete;
		childByType.updateState(_platform, _statusReason, _statusReasonAdditionalText);
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x0000FA10 File Offset: 0x0000DC10
	public static void Login(XUi _xuiInstance, Action _onLoginComplete)
	{
		_xuiInstance.playerUI.nguiWindowManager.SetLabelText(EnumNGUIWindow.Loading, Localization.Get("xuiSteamLoginProgressSignIn", false) + "...", false);
		PlatformManager.MultiPlatform.User.Login(delegate(IPlatform _platform, EApiStatusReason _statusReason, string _statusReasonAdditionalText)
		{
			if (_platform.Api.ClientApiStatus != EApiStatus.Ok)
			{
				XUiC_LoginPS5.Open(_xuiInstance, _platform, _statusReason, _statusReasonAdditionalText, _onLoginComplete);
				return;
			}
			if (_platform.User.UserStatus != EUserStatus.LoggedIn || _statusReason != EApiStatusReason.Ok)
			{
				XUiC_LoginPS5.Open(_xuiInstance, _platform, _statusReason, _statusReasonAdditionalText, _onLoginComplete);
				return;
			}
			_onLoginComplete();
		});
	}

	// Token: 0x04000275 RID: 629
	[PublicizedFrom(EAccessModifier.Private)]
	public static string ID = "";

	// Token: 0x04000276 RID: 630
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnRetry;

	// Token: 0x04000277 RID: 631
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnOffline;

	// Token: 0x04000278 RID: 632
	[PublicizedFrom(EAccessModifier.Private)]
	public IPlatform offendingPlatform;

	// Token: 0x04000279 RID: 633
	[PublicizedFrom(EAccessModifier.Private)]
	public EApiStatusReason statusReason;

	// Token: 0x0400027A RID: 634
	[PublicizedFrom(EAccessModifier.Private)]
	public string statusReasonAdditionalText;

	// Token: 0x0400027B RID: 635
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wantOffline;

	// Token: 0x0400027C RID: 636
	[PublicizedFrom(EAccessModifier.Private)]
	public Action onLoginComplete;
}
