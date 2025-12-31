using System;
using System.Runtime.CompilerServices;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000D2D RID: 3373
[Preserve]
public class XUiC_MessageBoxWindowGroup : XUiController
{
	// Token: 0x17000AB1 RID: 2737
	// (get) Token: 0x06006906 RID: 26886 RVA: 0x002AA6C4 File Offset: 0x002A88C4
	// (set) Token: 0x06006907 RID: 26887 RVA: 0x002AA6CC File Offset: 0x002A88CC
	public string Title
	{
		get
		{
			return this.title;
		}
		set
		{
			this.title = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000AB2 RID: 2738
	// (get) Token: 0x06006908 RID: 26888 RVA: 0x002AA6DC File Offset: 0x002A88DC
	// (set) Token: 0x06006909 RID: 26889 RVA: 0x002AA6E4 File Offset: 0x002A88E4
	public string Text
	{
		get
		{
			return this.text;
		}
		set
		{
			this.text = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x140000AA RID: 170
	// (add) Token: 0x0600690A RID: 26890 RVA: 0x002AA6F4 File Offset: 0x002A88F4
	// (remove) Token: 0x0600690B RID: 26891 RVA: 0x002AA72C File Offset: 0x002A892C
	public event Action OnLeftButtonEvent;

	// Token: 0x140000AB RID: 171
	// (add) Token: 0x0600690C RID: 26892 RVA: 0x002AA764 File Offset: 0x002A8964
	// (remove) Token: 0x0600690D RID: 26893 RVA: 0x002AA79C File Offset: 0x002A899C
	public event Action OnRightButtonEvent;

	// Token: 0x0600690E RID: 26894 RVA: 0x002AA7D4 File Offset: 0x002A89D4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "msgTitle")
		{
			_value = this.title;
			return true;
		}
		if (_bindingName == "msgText")
		{
			_value = this.text;
			return true;
		}
		if (_bindingName == "showleftbutton")
		{
			_value = (this.MessageBoxType == XUiC_MessageBoxWindowGroup.MessageBoxTypes.OkCancel).ToString();
			return true;
		}
		if (_bindingName == "rightbuttontext")
		{
			_value = ((this.MessageBoxType == XUiC_MessageBoxWindowGroup.MessageBoxTypes.Ok) ? "xuiOk" : "xuiCancel");
			return true;
		}
		if (!(_bindingName == "leftbuttontext"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = ((this.MessageBoxType == XUiC_MessageBoxWindowGroup.MessageBoxTypes.Ok) ? "" : "xuiOk");
		return true;
	}

	// Token: 0x0600690F RID: 26895 RVA: 0x002AA888 File Offset: 0x002A8A88
	public override void Init()
	{
		base.Init();
		XUiC_MessageBoxWindowGroup.ID = base.WindowGroup.ID;
		this.btnLeft = base.GetChildById("clickable2");
		if (this.btnLeft != null)
		{
			((XUiV_Button)this.btnLeft.ViewComponent).Controller.OnPress += this.leftButton_OnPress;
		}
		this.btnRight = base.GetChildById("clickable");
		if (this.btnRight != null)
		{
			((XUiV_Button)this.btnRight.ViewComponent).Controller.OnPress += this.rightButton_OnPress;
		}
		this.leftButtonPressed = false;
		this.rightButtonPressed = false;
	}

	// Token: 0x06006910 RID: 26896 RVA: 0x002AA937 File Offset: 0x002A8B37
	[PublicizedFrom(EAccessModifier.Private)]
	public void leftButton_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.OnLeftButtonEvent != null)
		{
			this.OnLeftButtonEvent();
		}
		this.leftButtonPressed = true;
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
	}

	// Token: 0x06006911 RID: 26897 RVA: 0x002AA973 File Offset: 0x002A8B73
	[PublicizedFrom(EAccessModifier.Private)]
	public void rightButton_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.OnRightButtonEvent != null)
		{
			this.OnRightButtonEvent();
		}
		this.rightButtonPressed = true;
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
	}

	// Token: 0x06006912 RID: 26898 RVA: 0x0028C056 File Offset: 0x0028A256
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(true);
			this.IsDirty = false;
		}
	}

	// Token: 0x06006913 RID: 26899 RVA: 0x002AA9AF File Offset: 0x002A8BAF
	public override void UpdateInput()
	{
		base.UpdateInput();
		if (base.xui.playerUI.playerInput.GUIActions.Cancel.WasReleased)
		{
			this.rightButton_OnPress(this, -1);
		}
	}

	// Token: 0x06006914 RID: 26900 RVA: 0x002AA9E0 File Offset: 0x002A8BE0
	public override void OnOpen()
	{
		base.OnOpen();
		this.leftButtonPressed = false;
		this.rightButtonPressed = false;
		this.windowGroup.isEscClosable = false;
		base.xui.playerUI.CursorController.Locked = false;
		base.xui.playerUI.CursorController.SetCursorHidden(false);
		base.xui.playerUI.CursorController.SetNavigationLockView(null, null);
	}

	// Token: 0x06006915 RID: 26901 RVA: 0x002AAA50 File Offset: 0x002A8C50
	public override void OnClose()
	{
		base.OnClose();
		base.xui.playerUI.CursorController.SetNavigationLockView(null, null);
		if (!this.OpenMainMenuOnClose)
		{
			base.xui.playerUI.CursorController.SetNavigationTargetLater(XUiC_MessageBoxWindowGroup.returnNavigationTarget);
		}
		if (this.MessageBoxType == XUiC_MessageBoxWindowGroup.MessageBoxTypes.OkCancel && !this.rightButtonPressed && !this.leftButtonPressed)
		{
			Action onRightButtonEvent = this.OnRightButtonEvent;
			if (onRightButtonEvent != null)
			{
				onRightButtonEvent();
			}
		}
		if (GameManager.Instance.World == null)
		{
			if (this.OpenMainMenuOnClose)
			{
				base.xui.playerUI.windowManager.Open(XUiC_MainMenu.ID, true, false, true);
			}
			ThreadManager.StartCoroutine(PlatformApplicationManager.CheckRestartCoroutine(false));
		}
	}

	// Token: 0x06006916 RID: 26902 RVA: 0x002AAB04 File Offset: 0x002A8D04
	public void ShowMessage(string _title, string _text, XUiC_MessageBoxWindowGroup.MessageBoxTypes _messageBoxType = XUiC_MessageBoxWindowGroup.MessageBoxTypes.Ok, Action _onLeftButton = null, Action _onRightButton = null, bool _openMainMenuOnClose = true, bool _modal = true, bool _bCloseAllOpenWindows = true)
	{
		this.Text = _text;
		this.Title = _title;
		this.MessageBoxType = _messageBoxType;
		this.OnLeftButtonEvent = _onLeftButton;
		this.OnRightButtonEvent = _onRightButton;
		this.OpenMainMenuOnClose = _openMainMenuOnClose;
		if (this.windowGroup.isShowing)
		{
			this.OnOpen();
			return;
		}
		base.xui.playerUI.windowManager.Open(base.WindowGroup.ID, _modal, false, _bCloseAllOpenWindows);
	}

	// Token: 0x06006917 RID: 26903 RVA: 0x002AAB78 File Offset: 0x002A8D78
	public void ShowNetworkError(NetworkConnectionError _error)
	{
		string arg = _error.ToStringCached<NetworkConnectionError>();
		switch (_error)
		{
		case NetworkConnectionError.InternalDirectConnectFailed:
		case NetworkConnectionError.EmptyConnectTarget:
		case NetworkConnectionError.IncorrectParameters:
		case NetworkConnectionError.AlreadyConnectedToAnotherServer:
			break;
		case NetworkConnectionError.CreateSocketOrThreadFailure:
			arg = string.Format(Localization.Get("mmLblErrorSocketFailure", false), SingletonMonoBehaviour<ConnectionManager>.Instance.GetRequiredPortsString());
			goto IL_CD;
		default:
			switch (_error)
			{
			case NetworkConnectionError.ConnectionFailed:
			case NetworkConnectionError.AlreadyConnectedToServer:
			case (NetworkConnectionError)17:
			case NetworkConnectionError.TooManyConnectedPlayers:
			case (NetworkConnectionError)19:
			case (NetworkConnectionError)20:
			case NetworkConnectionError.RSAPublicKeyMismatch:
			case NetworkConnectionError.ConnectionBanned:
				break;
			case NetworkConnectionError.InvalidPassword:
				arg = Localization.Get("mmLblErrorWrongPassword", false);
				goto IL_CD;
			default:
				switch (_error)
				{
				case NetworkConnectionError.RestartRequired:
					arg = string.Format(Localization.Get("app_restartRequired", false), Array.Empty<object>());
					goto IL_CD;
				}
				break;
			}
			break;
		}
		arg = string.Format(Localization.Get("mmLblErrorUnknown", false), arg);
		IL_CD:
		this.ShowMessage(Localization.Get("mmLblErrorServerInit", false), arg, XUiC_MessageBoxWindowGroup.MessageBoxTypes.Ok, null, null, true, true, true);
	}

	// Token: 0x06006918 RID: 26904 RVA: 0x002AAC6C File Offset: 0x002A8E6C
	public void ShowUrlConfirmationDialog(string _url, string _displayUrl, bool _modal = false, Func<string, bool> _browserOpenMethod = null, string _title = null, string _text = null)
	{
		if (string.IsNullOrEmpty(_url))
		{
			return;
		}
		if (!Utils.IsValidWebUrl(ref _url))
		{
			return;
		}
		this.urlBoxData.Item1 = base.xui.playerUI.windowManager.GetModalWindow();
		this.urlBoxData.Item2 = _url;
		if (_browserOpenMethod == null)
		{
			_browserOpenMethod = ((PlatformManager.NativePlatform.Utils != null) ? new Func<string, bool>(PlatformManager.NativePlatform.Utils.OpenBrowser) : null);
		}
		this.urlBoxData.Item3 = _browserOpenMethod;
		if (_title == null)
		{
			_title = Localization.Get("xuiOpenUrlConfirmationTitle", false);
		}
		if (_text == null)
		{
			_text = Localization.Get("xuiOpenUrlConfirmationText", false);
		}
		_text = string.Format(_text, _displayUrl);
		bool openMainMenuOnClose = false;
		if (this.windowGroup.isShowing)
		{
			_modal = this.windowGroup.isModal;
			openMainMenuOnClose = this.OpenMainMenuOnClose;
		}
		this.ShowMessage(_title, _text, XUiC_MessageBoxWindowGroup.MessageBoxTypes.OkCancel, new Action(this.openPage), new Action(this.cancelOpenPage), openMainMenuOnClose, _modal, true);
		base.xui.playerUI.playerInput.PermanentActions.Cancel.Enabled = false;
	}

	// Token: 0x06006919 RID: 26905 RVA: 0x002AAD88 File Offset: 0x002A8F88
	[PublicizedFrom(EAccessModifier.Private)]
	public void openPage()
	{
		this.urlBoxData.Item1 = null;
		base.xui.playerUI.playerInput.PermanentActions.Cancel.Enabled = true;
		Func<string, bool> item = this.urlBoxData.Item3;
		if (item == null)
		{
			return;
		}
		item(this.urlBoxData.Item2);
	}

	// Token: 0x0600691A RID: 26906 RVA: 0x002AADE4 File Offset: 0x002A8FE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void cancelOpenPage()
	{
		this.urlBoxData.Item1 = null;
		XUi xui = base.xui;
		bool flag;
		if (xui == null)
		{
			flag = (null != null);
		}
		else
		{
			LocalPlayerUI playerUI = xui.playerUI;
			if (playerUI == null)
			{
				flag = (null != null);
			}
			else
			{
				PlayerActionsLocal playerInput = playerUI.playerInput;
				flag = (((playerInput != null) ? playerInput.PermanentActions : null) != null);
			}
		}
		if (flag)
		{
			base.xui.playerUI.playerInput.PermanentActions.Cancel.Enabled = true;
		}
	}

	// Token: 0x0600691B RID: 26907 RVA: 0x002AAE4C File Offset: 0x002A904C
	public static void ShowMessageBox(XUi _xuiInstance, string _title, string _text, XUiC_MessageBoxWindowGroup.MessageBoxTypes _messageBoxType = XUiC_MessageBoxWindowGroup.MessageBoxTypes.Ok, Action _onOk = null, Action _onCancel = null, bool _openMainMenuOnClose = true, bool _modal = true, bool _bCloseAllOpenWindows = true)
	{
		XUiC_MessageBoxWindowGroup.returnNavigationTarget = _xuiInstance.playerUI.CursorController.navigationTarget;
		((XUiC_MessageBoxWindowGroup)_xuiInstance.FindWindowGroupByName(XUiC_MessageBoxWindowGroup.ID)).ShowMessage(_title, _text, _messageBoxType, _onOk, _onCancel, _openMainMenuOnClose, _modal, _bCloseAllOpenWindows);
	}

	// Token: 0x0600691C RID: 26908 RVA: 0x002AAE90 File Offset: 0x002A9090
	public static void ShowMessageBox(XUi _xuiInstance, string _title, string _text, Action _onOk = null, bool _openMainMenuOnClose = true, bool _modal = true)
	{
		XUiC_MessageBoxWindowGroup.returnNavigationTarget = _xuiInstance.playerUI.CursorController.navigationTarget;
		((XUiC_MessageBoxWindowGroup)_xuiInstance.FindWindowGroupByName(XUiC_MessageBoxWindowGroup.ID)).ShowMessage(_title, _text, XUiC_MessageBoxWindowGroup.MessageBoxTypes.Ok, null, _onOk, _openMainMenuOnClose, _modal, true);
	}

	// Token: 0x0600691D RID: 26909 RVA: 0x002AAED1 File Offset: 0x002A90D1
	public static void ShowUrlConfirmationDialog(XUi _xuiInstance, string _url, bool _modal = false, Func<string, bool> _browserOpenMethod = null, string _title = null, string _text = null, string _displayUrl = null)
	{
		XUiC_MessageBoxWindowGroup.returnNavigationTarget = _xuiInstance.playerUI.CursorController.navigationTarget;
		XUiC_MessageBoxWindowGroup xuiC_MessageBoxWindowGroup = (XUiC_MessageBoxWindowGroup)_xuiInstance.FindWindowGroupByName(XUiC_MessageBoxWindowGroup.ID);
		if (_displayUrl == null)
		{
			_displayUrl = _url;
		}
		xuiC_MessageBoxWindowGroup.ShowUrlConfirmationDialog(_url, _displayUrl, _modal, _browserOpenMethod, _title, _text);
	}

	// Token: 0x04004F3C RID: 20284
	[PublicizedFrom(EAccessModifier.Private)]
	public string title = "";

	// Token: 0x04004F3D RID: 20285
	[PublicizedFrom(EAccessModifier.Private)]
	public string text = "";

	// Token: 0x04004F3E RID: 20286
	public static string ID = "";

	// Token: 0x04004F41 RID: 20289
	public bool OpenMainMenuOnClose = true;

	// Token: 0x04004F42 RID: 20290
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_MessageBoxWindowGroup.MessageBoxTypes MessageBoxType;

	// Token: 0x04004F43 RID: 20291
	[PublicizedFrom(EAccessModifier.Private)]
	public bool leftButtonPressed;

	// Token: 0x04004F44 RID: 20292
	[PublicizedFrom(EAccessModifier.Private)]
	public bool rightButtonPressed;

	// Token: 0x04004F45 RID: 20293
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnLeft;

	// Token: 0x04004F46 RID: 20294
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnRight;

	// Token: 0x04004F47 RID: 20295
	[PublicizedFrom(EAccessModifier.Private)]
	public static XUiView returnNavigationTarget = null;

	// Token: 0x04004F48 RID: 20296
	[TupleElementNames(new string[]
	{
		"prevModalWindow",
		"url",
		"browserOpenMethod"
	})]
	[PublicizedFrom(EAccessModifier.Private)]
	public ValueTuple<GUIWindow, string, Func<string, bool>> urlBoxData;

	// Token: 0x02000D2E RID: 3374
	public enum MessageBoxTypes
	{
		// Token: 0x04004F4A RID: 20298
		Ok,
		// Token: 0x04004F4B RID: 20299
		OkCancel
	}
}
