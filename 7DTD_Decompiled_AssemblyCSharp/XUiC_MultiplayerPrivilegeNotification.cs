using System;
using System.Collections;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D2F RID: 3375
[Preserve]
public class XUiC_MultiplayerPrivilegeNotification : XUiController
{
	// Token: 0x06006920 RID: 26912 RVA: 0x002AAF44 File Offset: 0x002A9144
	public static XUiC_MultiplayerPrivilegeNotification GetWindow()
	{
		if (GameManager.Instance.gameStateManager.IsGameStarted())
		{
			return (XUiC_MultiplayerPrivilegeNotification)((XUiWindowGroup)LocalPlayerUI.GetUIForPrimaryPlayer().windowManager.GetWindow(XUiC_MultiplayerPrivilegeNotification.InGameWindowID)).Controller;
		}
		return (XUiC_MultiplayerPrivilegeNotification)((XUiWindowGroup)LocalPlayerUI.primaryUI.windowManager.GetWindow(XUiC_MultiplayerPrivilegeNotification.MenuWindowID)).Controller;
	}

	// Token: 0x06006921 RID: 26913 RVA: 0x002AAFAC File Offset: 0x002A91AC
	public static void Close()
	{
		XUiC_MultiplayerPrivilegeNotification window = XUiC_MultiplayerPrivilegeNotification.GetWindow();
		string text = (window != null) ? window.ID : null;
		if (!string.IsNullOrEmpty(text))
		{
			LocalPlayerUI.primaryUI.windowManager.Close(text);
		}
	}

	// Token: 0x06006922 RID: 26914 RVA: 0x002AAFE4 File Offset: 0x002A91E4
	public override void Init()
	{
		base.Init();
		if (base.WindowGroup.ID.Contains("menu", StringComparison.OrdinalIgnoreCase))
		{
			XUiC_MultiplayerPrivilegeNotification.MenuWindowID = base.WindowGroup.ID;
		}
		else
		{
			if (!base.WindowGroup.ID.Contains("ingame", StringComparison.OrdinalIgnoreCase))
			{
				throw new Exception("Found Window Group for XUiC_MultiplayerPrivilegeNotification, name didn't contain \"menu\" or \"ingame\"");
			}
			XUiC_MultiplayerPrivilegeNotification.InGameWindowID = base.WindowGroup.ID;
		}
		this.ID = base.WindowGroup.ID;
		this.btnCancel = (XUiC_SimpleButton)base.GetChildById("btnCancel");
		this.btnCancel.OnPressed += this.BtnCancel_OnPressed;
		this.btnClose = (XUiC_SimpleButton)base.GetChildById("btnClose");
		this.btnClose.OnPressed += this.BtnClose_OnPressed;
		this.lblResolvingPrivileges = (XUiV_Label)base.GetChildById("lblResolvingPrivileges").ViewComponent;
		this.lblInvalidPrivileges = (XUiV_Label)base.GetChildById("lblInvalidPrivileges").ViewComponent;
		this.header = (XUiV_Panel)base.GetChildById("header").ViewComponent;
		this.content = (XUiV_Panel)base.GetChildById("content").ViewComponent;
		this.buttons = (XUiV_Panel)base.GetChildById("buttons").ViewComponent;
	}

	// Token: 0x06006923 RID: 26915 RVA: 0x002AB148 File Offset: 0x002A9348
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnCancel_OnPressed(XUiController _sender, int _mouseButton)
	{
		if (this.cancellationToken != null)
		{
			this.cancellationToken.Cancel();
		}
		else
		{
			this.CloseWindow(false);
		}
		this.btnCancel.Enabled = false;
	}

	// Token: 0x06006924 RID: 26916 RVA: 0x002AB172 File Offset: 0x002A9372
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnClose_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.CloseWindow(false);
	}

	// Token: 0x06006925 RID: 26917 RVA: 0x002AB17C File Offset: 0x002A937C
	public override void OnClose()
	{
		if (this.cancellationToken != null)
		{
			this.cancellationToken.Cancel();
		}
		this.lblResolvingPrivileges.IsVisible = false;
		this.lblInvalidPrivileges.IsVisible = false;
		this.btnCancel.IsVisible = false;
		this.btnClose.IsVisible = false;
		base.OnClose();
		if (XUiC_ProgressWindow.IsWindowOpen())
		{
			this.CloseWindow(false);
		}
	}

	// Token: 0x06006926 RID: 26918 RVA: 0x002AB1E0 File Offset: 0x002A93E0
	public bool ResolvePrivilegesWithDialog(EUserPerms _permissions, Action<bool> _resolutionComplete, float _delayDisplay = -1f, bool _usingProgressWindow = false, Action _cancellationCleanupAction = null)
	{
		if (this.resolving)
		{
			return false;
		}
		if (_permissions == (EUserPerms)0)
		{
			Log.Error("No privileges specified.");
			return false;
		}
		this.resolving = true;
		this.cancellationToken = new CoroutineCancellationToken();
		this.cancellationCleanupAction = _cancellationCleanupAction;
		if (_usingProgressWindow)
		{
			string text = Localization.Get("lblResolvingPrivileges", false) + "\n\n[FFFFFF]" + Utils.GetCancellationMessage();
			XUiC_ProgressWindow.Open(LocalPlayerUI.primaryUI, text, delegate
			{
				this.cancellationToken.Cancel();
			}, true, true, true, false);
		}
		else
		{
			base.xui.playerUI.windowManager.Open(this.ID, false, false, true);
			this.SetContentVisibility(false);
			this.btnCancel.Enabled = true;
			this.btnCancel.IsVisible = true;
			this.btnClose.Enabled = false;
			this.btnClose.IsVisible = false;
			this.lblResolvingPrivileges.IsVisible = true;
			this.lblInvalidPrivileges.IsVisible = false;
		}
		if (_delayDisplay < 0f)
		{
			_delayDisplay = this.GetDefaultPlatformDelay();
		}
		ThreadManager.StartCoroutine(this.DelayPanelVisibility(_delayDisplay));
		ThreadManager.StartCoroutine(this.ResolvePrivilegesCoroutine(_permissions, _resolutionComplete, _usingProgressWindow));
		return true;
	}

	// Token: 0x06006927 RID: 26919 RVA: 0x002AB2F7 File Offset: 0x002A94F7
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ResolvePrivilegesCoroutine(EUserPerms _permissions, Action<bool> _resolutionComplete, bool _usingProgressWindow)
	{
		if (_permissions != (EUserPerms)0)
		{
			yield return PermissionsManager.ResolvePermissions(_permissions, true, this.cancellationToken);
		}
		this.resolving = false;
		CoroutineCancellationToken coroutineCancellationToken = this.cancellationToken;
		if (coroutineCancellationToken != null && coroutineCancellationToken.IsCancelled())
		{
			Action action = this.cancellationCleanupAction;
			if (action != null)
			{
				action();
			}
			this.CloseWindow(false);
			yield break;
		}
		this.eulaAccepted = GameManager.HasAcceptedLatestEula();
		bool flag = this.eulaAccepted && (PermissionsManager.GetPermissions(PermissionsManager.PermissionSources.All) & _permissions) == _permissions;
		if (_usingProgressWindow)
		{
			if (flag)
			{
				this.CloseWindow(true);
			}
			else
			{
				string text = (!this.eulaAccepted) ? Localization.Get("uiPermissionsEula", false) : (PermissionsManager.GetPermissionDenyReason(_permissions, PermissionsManager.PermissionSources.All) ?? Localization.Get("lblInvalidPrivileges", false));
				text = text + "\n\n[FFFFFF]" + Utils.GetCancellationMessage();
				XUiC_ProgressWindow.Open(LocalPlayerUI.primaryUI, text, delegate
				{
					this.CloseWindow(false);
				}, true, true, true, false);
			}
		}
		else if (flag)
		{
			this.CloseWindow(true);
		}
		else
		{
			this.lblResolvingPrivileges.IsVisible = false;
			string text2;
			if (!this.eulaAccepted)
			{
				text2 = Localization.Get("uiPermissionsEula", false);
			}
			else
			{
				text2 = PermissionsManager.GetPermissionDenyReason(_permissions, PermissionsManager.PermissionSources.All);
			}
			if (!string.IsNullOrEmpty(text2))
			{
				this.btnCancel.Enabled = false;
				this.btnCancel.IsVisible = false;
				this.btnClose.Enabled = true;
				this.btnClose.IsVisible = true;
				this.lblInvalidPrivileges.Text = text2;
				this.lblInvalidPrivileges.IsVisible = true;
				this.SetContentVisibility(true);
			}
			else
			{
				this.CloseWindow(false);
			}
		}
		if (_resolutionComplete != null)
		{
			_resolutionComplete(flag);
		}
		yield break;
	}

	// Token: 0x06006928 RID: 26920 RVA: 0x002AB31C File Offset: 0x002A951C
	[PublicizedFrom(EAccessModifier.Private)]
	public void CloseWindow(bool _success)
	{
		if (!XUiC_ProgressWindow.IsWindowOpen())
		{
			base.xui.playerUI.windowManager.Close(this.ID);
			return;
		}
		XUiC_ProgressWindow.Close(LocalPlayerUI.primaryUI);
		if (_success || GameManager.Instance.gameStateManager.IsGameStarted())
		{
			return;
		}
		LocalPlayerUI.primaryUI.windowManager.Open(XUiC_MainMenu.ID, true, false, true);
		if (!GameManager.HasAcceptedLatestEula())
		{
			XUiC_EulaWindow.Open(base.xui, false);
		}
	}

	// Token: 0x06006929 RID: 26921 RVA: 0x0012DE4B File Offset: 0x0012C04B
	[PublicizedFrom(EAccessModifier.Private)]
	public float GetDefaultPlatformDelay()
	{
		return 0.2f;
	}

	// Token: 0x0600692A RID: 26922 RVA: 0x002AB398 File Offset: 0x002A9598
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetContentVisibility(bool _visible)
	{
		this.header.IsVisible = _visible;
		this.content.IsVisible = _visible;
		this.buttons.IsVisible = _visible;
		if (_visible)
		{
			if (this.btnClose.IsVisible)
			{
				base.xui.playerUI.CursorController.SetNavigationLockView(this.buttons, this.btnClose.ViewComponent);
				this.btnClose.SelectCursorElement(true, false);
				return;
			}
			if (this.btnCancel.IsVisible)
			{
				base.xui.playerUI.CursorController.SetNavigationLockView(this.buttons, this.btnCancel.ViewComponent);
				this.btnCancel.SelectCursorElement(true, false);
				return;
			}
		}
		else
		{
			base.xui.playerUI.CursorController.SetNavigationLockView(null, null);
		}
	}

	// Token: 0x0600692B RID: 26923 RVA: 0x002AB46A File Offset: 0x002A966A
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator DelayPanelVisibility(float _delay)
	{
		yield return new WaitForSeconds(_delay);
		if (this.resolving)
		{
			this.SetContentVisibility(true);
		}
		yield break;
	}

	// Token: 0x04004F4C RID: 20300
	[PublicizedFrom(EAccessModifier.Private)]
	public static string MenuWindowID;

	// Token: 0x04004F4D RID: 20301
	[PublicizedFrom(EAccessModifier.Private)]
	public static string InGameWindowID;

	// Token: 0x04004F4E RID: 20302
	[PublicizedFrom(EAccessModifier.Private)]
	public string ID;

	// Token: 0x04004F4F RID: 20303
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnCancel;

	// Token: 0x04004F50 RID: 20304
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnClose;

	// Token: 0x04004F51 RID: 20305
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblResolvingPrivileges;

	// Token: 0x04004F52 RID: 20306
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblInvalidPrivileges;

	// Token: 0x04004F53 RID: 20307
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Panel header;

	// Token: 0x04004F54 RID: 20308
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Panel content;

	// Token: 0x04004F55 RID: 20309
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Panel buttons;

	// Token: 0x04004F56 RID: 20310
	[PublicizedFrom(EAccessModifier.Private)]
	public bool resolving;

	// Token: 0x04004F57 RID: 20311
	[PublicizedFrom(EAccessModifier.Private)]
	public bool eulaAccepted;

	// Token: 0x04004F58 RID: 20312
	[PublicizedFrom(EAccessModifier.Private)]
	public CoroutineCancellationToken cancellationToken;

	// Token: 0x04004F59 RID: 20313
	[PublicizedFrom(EAccessModifier.Private)]
	public Action cancellationCleanupAction;
}
