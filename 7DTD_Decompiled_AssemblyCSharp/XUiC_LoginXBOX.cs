using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000061 RID: 97
[Preserve]
public class XUiC_LoginXBOX : XUiController
{
	// Token: 0x060001D3 RID: 467 RVA: 0x0000FF98 File Offset: 0x0000E198
	public override void Init()
	{
		base.Init();
		XUiC_LoginXBOX.ID = base.WindowGroup.ID;
		this.btnRetry = (XUiC_SimpleButton)base.GetChildById("btnRetry");
		this.btnRetry.OnPressed += this.BtnRetry_OnPressed;
		this.btnOffline = (XUiC_SimpleButton)base.GetChildById("btnOffline");
		this.btnOffline.OnPressed += this.BtnOffline_OnPressed;
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x00010018 File Offset: 0x0000E218
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

	// Token: 0x060001D5 RID: 469 RVA: 0x000100FC File Offset: 0x0000E2FC
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

	// Token: 0x060001D6 RID: 470 RVA: 0x00010180 File Offset: 0x0000E380
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

	// Token: 0x060001D7 RID: 471 RVA: 0x000101D8 File Offset: 0x0000E3D8
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
		base.xui.playerUI.windowManager.Open(XUiC_LoginXBOX.ID, true, true, true);
		this.btnRetry.Enabled = (_platform.Api.ClientApiStatus != EApiStatus.PermanentError);
		this.btnOffline.Enabled = (_platform.User.UserStatus == EUserStatus.OfflineMode);
		this.offendingPlatform = _platform;
		this.statusReason = _statusReason;
		this.statusReasonAdditionalText = _statusReasonAdditionalText;
		base.RefreshBindings(false);
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x000102AF File Offset: 0x0000E4AF
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Open(XUi _xuiInstance, IPlatform _platform, EApiStatusReason _statusReason, string _statusReasonAdditionalText, Action _onLoginComplete)
	{
		XUiC_LoginXBOX childByType = _xuiInstance.FindWindowGroupByName(XUiC_LoginXBOX.ID).GetChildByType<XUiC_LoginXBOX>();
		childByType.onLoginComplete = _onLoginComplete;
		childByType.updateState(_platform, _statusReason, _statusReasonAdditionalText);
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x000102D4 File Offset: 0x0000E4D4
	public static void Login(XUi _xuiInstance, Action _onLoginComplete)
	{
		_xuiInstance.playerUI.nguiWindowManager.SetLabelText(EnumNGUIWindow.Loading, Localization.Get("xuiSteamLoginProgressSignIn", false) + "...", false);
		PlatformManager.MultiPlatform.User.Login(delegate(IPlatform _platform, EApiStatusReason _statusReason, string _statusReasonAdditionalText)
		{
			if (_platform.Api.ClientApiStatus != EApiStatus.Ok)
			{
				XUiC_LoginXBOX.Open(_xuiInstance, _platform, _statusReason, _statusReasonAdditionalText, _onLoginComplete);
				return;
			}
			if (_platform.User.UserStatus != EUserStatus.LoggedIn || _statusReason != EApiStatusReason.Ok)
			{
				XUiC_LoginXBOX.Open(_xuiInstance, _platform, _statusReason, _statusReasonAdditionalText, _onLoginComplete);
				return;
			}
			_onLoginComplete();
		});
	}

	// Token: 0x04000282 RID: 642
	[PublicizedFrom(EAccessModifier.Private)]
	public static string ID = "";

	// Token: 0x04000283 RID: 643
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnRetry;

	// Token: 0x04000284 RID: 644
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnOffline;

	// Token: 0x04000285 RID: 645
	[PublicizedFrom(EAccessModifier.Private)]
	public IPlatform offendingPlatform;

	// Token: 0x04000286 RID: 646
	[PublicizedFrom(EAccessModifier.Private)]
	public EApiStatusReason statusReason;

	// Token: 0x04000287 RID: 647
	[PublicizedFrom(EAccessModifier.Private)]
	public string statusReasonAdditionalText;

	// Token: 0x04000288 RID: 648
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wantOffline;

	// Token: 0x04000289 RID: 649
	[PublicizedFrom(EAccessModifier.Private)]
	public Action onLoginComplete;
}
