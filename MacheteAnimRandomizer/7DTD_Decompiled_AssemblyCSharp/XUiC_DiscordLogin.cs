using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

// Token: 0x02000300 RID: 768
[Preserve]
public class XUiC_DiscordLogin : XUiController
{
	// Token: 0x060015C8 RID: 5576 RVA: 0x0008003C File Offset: 0x0007E23C
	public override void Init()
	{
		base.Init();
		XUiC_DiscordLogin.ID = base.WindowGroup.ID;
		XUiC_SimpleButton xuiC_SimpleButton = base.GetChildById("btnOk") as XUiC_SimpleButton;
		if (xuiC_SimpleButton != null)
		{
			xuiC_SimpleButton.OnPressed += delegate(XUiController _, int _)
			{
				this.closeAndOpenNextWindow();
			};
		}
		XUiC_SimpleButton xuiC_SimpleButton2 = base.GetChildById("btnCancel") as XUiC_SimpleButton;
		if (xuiC_SimpleButton2 != null)
		{
			xuiC_SimpleButton2.OnPressed += delegate(XUiController _, int _)
			{
				DiscordManager.Instance.AuthManager.AbortAuth();
			};
		}
		XUiC_SimpleButton xuiC_SimpleButton3 = base.GetChildById("btnSettings") as XUiC_SimpleButton;
		if (xuiC_SimpleButton3 != null)
		{
			xuiC_SimpleButton3.OnPressed += delegate(XUiController _, int _)
			{
				base.xui.playerUI.windowManager.Close(XUiC_DiscordLogin.ID);
				XUiC_OptionsAudio childByType = base.xui.GetChildByType<XUiC_OptionsAudio>();
				if (childByType == null)
				{
					return;
				}
				childByType.OpenAtTab("xuiOptionsAudioDiscord");
			};
		}
	}

	// Token: 0x060015C9 RID: 5577 RVA: 0x000800E4 File Offset: 0x0007E2E4
	[PublicizedFrom(EAccessModifier.Private)]
	public void closeAndOpenNextWindow()
	{
		base.xui.playerUI.windowManager.Close(XUiC_DiscordLogin.ID);
		if (this.onCloseWithOk == null)
		{
			base.xui.playerUI.windowManager.Open(XUiC_MainMenu.ID, true, false, true);
			return;
		}
		this.onCloseWithOk();
	}

	// Token: 0x060015CA RID: 5578 RVA: 0x0008013C File Offset: 0x0007E33C
	public override void OnOpen()
	{
		base.OnOpen();
		base.xui.playerUI.nguiWindowManager.Show(EnumNGUIWindow.Loading, false);
		this.windowGroup.isEscClosable = false;
		base.RefreshBindings(false);
	}

	// Token: 0x060015CB RID: 5579 RVA: 0x0008016E File Offset: 0x0007E36E
	public override void OnClose()
	{
		base.OnClose();
		DiscordManager.Instance.UserAuthorizationResult -= this.authResult;
	}

	// Token: 0x060015CC RID: 5580 RVA: 0x0008018C File Offset: 0x0007E38C
	public override void UpdateInput()
	{
		base.UpdateInput();
		if (!this.isDone)
		{
			return;
		}
		PlayerActionsGUI guiactions = base.xui.playerUI.playerInput.GUIActions;
		if (guiactions.Apply.WasReleased || guiactions.Cancel.WasReleased)
		{
			this.closeAndOpenNextWindow();
		}
	}

	// Token: 0x060015CD RID: 5581 RVA: 0x000801E0 File Offset: 0x0007E3E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void authResult(bool _isDone, DiscordManager.EFullAccountLoginResult _fullAccResult, DiscordManager.EProvisionalAccountLoginResult _provisionalAccResult, bool _isExpectedSuccess)
	{
		LocalPlayerUI playerUI = base.xui.playerUI;
		playerUI.nguiWindowManager.Show(EnumNGUIWindow.Loading, false);
		if (!this.windowGroup.isShowing && (_isDone || _fullAccResult == DiscordManager.EFullAccountLoginResult.RequestingAuth))
		{
			if (this.skipOnSuccess && _isExpectedSuccess)
			{
				DiscordManager.Instance.UserAuthorizationResult -= this.authResult;
				this.closeAndOpenNextWindow();
				return;
			}
			XUiC_DiscordLogin.openNow(playerUI, this.openModal);
		}
		this.updateUi(_isDone, _fullAccResult, _provisionalAccResult, _isExpectedSuccess);
	}

	// Token: 0x060015CE RID: 5582 RVA: 0x0008025C File Offset: 0x0007E45C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateUi(bool _isDone, DiscordManager.EFullAccountLoginResult _fullAccResult, DiscordManager.EProvisionalAccountLoginResult _provisionalAccResult, bool _isExpectedSuccess)
	{
		XUiC_DiscordLogin.<>c__DisplayClass14_0 CS$<>8__locals1;
		CS$<>8__locals1._isDone = _isDone;
		CS$<>8__locals1._fullAccResult = _fullAccResult;
		CS$<>8__locals1._provisionalAccResult = _provisionalAccResult;
		this.isDone = CS$<>8__locals1._isDone;
		this.cancellable = (CS$<>8__locals1._fullAccResult == DiscordManager.EFullAccountLoginResult.RequestingAuth);
		this.text = XUiC_DiscordLogin.<updateUi>g__BuildText|14_0(ref CS$<>8__locals1);
		base.RefreshBindings(false);
	}

	// Token: 0x060015CF RID: 5583 RVA: 0x000802B0 File Offset: 0x0007E4B0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "status_text")
		{
			_value = (this.text ?? "");
			return true;
		}
		if (_bindingName == "show_settings_button")
		{
			_value = this.showSettingsButton.ToString();
			return true;
		}
		if (_bindingName == "done")
		{
			_value = this.isDone.ToString();
			return true;
		}
		if (!(_bindingName == "cancellable"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = this.cancellable.ToString();
		return true;
	}

	// Token: 0x060015D0 RID: 5584 RVA: 0x0008033C File Offset: 0x0007E53C
	public static void Open(Action _onCloseWithOk = null, bool _showSettingsButton = true, bool _waitForResultToShow = false, bool _skipOnSuccess = false, bool _modal = true, bool _cancellable = false)
	{
		XUi xui = LocalPlayerUI.primaryUI.xui;
		LocalPlayerUI playerUI = xui.playerUI;
		XUiC_DiscordLogin childByType = xui.GetWindowGroupById(XUiC_DiscordLogin.ID).Controller.GetChildByType<XUiC_DiscordLogin>();
		DiscordManager.Instance.UserAuthorizationResult += childByType.authResult;
		childByType.text = Localization.Get("xuiDiscordLoginLoggingIn", false);
		childByType.isDone = false;
		childByType.cancellable = _cancellable;
		childByType.onCloseWithOk = _onCloseWithOk;
		childByType.showSettingsButton = _showSettingsButton;
		childByType.skipOnSuccess = _skipOnSuccess;
		childByType.openModal = _modal;
		if (_waitForResultToShow)
		{
			playerUI.nguiWindowManager.SetLabelText(EnumNGUIWindow.Loading, Localization.Get("xuiDiscordLoginProgress", false) + "...", false);
			return;
		}
		XUiC_DiscordLogin.openNow(playerUI, _modal);
	}

	// Token: 0x060015D1 RID: 5585 RVA: 0x000803F0 File Offset: 0x0007E5F0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void openNow(LocalPlayerUI _playerUi, bool _modal = true)
	{
		_playerUi.windowManager.Open(XUiC_DiscordLogin.ID, _modal, true, true);
	}

	// Token: 0x060015D6 RID: 5590 RVA: 0x00080450 File Offset: 0x0007E650
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static string <updateUi>g__BuildText|14_0(ref XUiC_DiscordLogin.<>c__DisplayClass14_0 A_0)
	{
		if (!A_0._isDone)
		{
			switch (A_0._fullAccResult)
			{
			case DiscordManager.EFullAccountLoginResult.RequestingAuth:
				return Localization.Get("xuiDiscordLoginFullAccountRequestingAuth", false);
			case DiscordManager.EFullAccountLoginResult.AuthAccepted:
				return Localization.Get("xuiDiscordLoginFullAccountAuthAccepted", false);
			case DiscordManager.EFullAccountLoginResult.AuthCancelled:
				return Localization.Get("xuiDiscordLoginFullAccountAuthCancelledTryingProvisional", false);
			case DiscordManager.EFullAccountLoginResult.AuthFailed:
				return Localization.Get("xuiDiscordLoginFullAccountAuthFailedTryingProvisional", false);
			}
		}
		if (A_0._fullAccResult == DiscordManager.EFullAccountLoginResult.None)
		{
			return XUiC_DiscordLogin.<updateUi>g__BuildProvisionalOnlyText|14_3(ref A_0);
		}
		if (A_0._fullAccResult == DiscordManager.EFullAccountLoginResult.Success)
		{
			return Localization.Get("xuiDiscordLoginFullAccountSuccess", false);
		}
		return XUiC_DiscordLogin.<updateUi>g__BuildFullAccFailureText|14_1(ref A_0);
	}

	// Token: 0x060015D7 RID: 5591 RVA: 0x000804E0 File Offset: 0x0007E6E0
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static string <updateUi>g__BuildFullAccFailureText|14_1(ref XUiC_DiscordLogin.<>c__DisplayClass14_0 A_0)
	{
		string text = Localization.Get("xuiDiscordLoginFullAccount" + A_0._fullAccResult.ToStringCached<DiscordManager.EFullAccountLoginResult>(), false);
		if (A_0._provisionalAccResult == DiscordManager.EProvisionalAccountLoginResult.None || A_0._provisionalAccResult == DiscordManager.EProvisionalAccountLoginResult.NotSupported)
		{
			return text;
		}
		return XUiC_DiscordLogin.<updateUi>g__BuildProvisionalFallbackText|14_2(text, ref A_0);
	}

	// Token: 0x060015D8 RID: 5592 RVA: 0x00080524 File Offset: 0x0007E724
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static string <updateUi>g__BuildProvisionalFallbackText|14_2(string _fullAccFailureText, ref XUiC_DiscordLogin.<>c__DisplayClass14_0 A_1)
	{
		string str = Localization.Get("xuiDiscordLoginFallback" + A_1._provisionalAccResult.ToStringCached<DiscordManager.EProvisionalAccountLoginResult>(), false);
		return _fullAccFailureText + "\n\n" + str;
	}

	// Token: 0x060015D9 RID: 5593 RVA: 0x00080559 File Offset: 0x0007E759
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static string <updateUi>g__BuildProvisionalOnlyText|14_3(ref XUiC_DiscordLogin.<>c__DisplayClass14_0 A_0)
	{
		return Localization.Get("xuiDiscordLoginProvisional" + A_0._provisionalAccResult.ToStringCached<DiscordManager.EProvisionalAccountLoginResult>(), false);
	}

	// Token: 0x04000DD7 RID: 3543
	[PublicizedFrom(EAccessModifier.Private)]
	public static string ID = "";

	// Token: 0x04000DD8 RID: 3544
	[PublicizedFrom(EAccessModifier.Private)]
	public Action onCloseWithOk;

	// Token: 0x04000DD9 RID: 3545
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showSettingsButton;

	// Token: 0x04000DDA RID: 3546
	[PublicizedFrom(EAccessModifier.Private)]
	public bool skipOnSuccess;

	// Token: 0x04000DDB RID: 3547
	[PublicizedFrom(EAccessModifier.Private)]
	public bool openModal;

	// Token: 0x04000DDC RID: 3548
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDone;

	// Token: 0x04000DDD RID: 3549
	[PublicizedFrom(EAccessModifier.Private)]
	public bool cancellable;

	// Token: 0x04000DDE RID: 3550
	[PublicizedFrom(EAccessModifier.Private)]
	public string text;
}
