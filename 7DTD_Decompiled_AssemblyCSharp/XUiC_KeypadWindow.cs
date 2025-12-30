using System;
using Audio;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000CE4 RID: 3300
[Preserve]
public class XUiC_KeypadWindow : XUiController
{
	// Token: 0x06006662 RID: 26210 RVA: 0x00299278 File Offset: 0x00297478
	public override void Init()
	{
		XUiC_KeypadWindow.ID = this.windowGroup.ID;
		base.Init();
		this.txtPassword = (XUiC_TextInput)base.GetChildById("txtPassword");
		this.txtPassword.OnSubmitHandler += this.TxtPassword_OnSubmitHandler;
		XUiC_SimpleButton xuiC_SimpleButton = (XUiC_SimpleButton)base.GetChildById("btnCancel");
		xuiC_SimpleButton.OnPressed += this.BtnCancel_OnPressed;
		xuiC_SimpleButton.ViewComponent.NavUpTarget = this.txtPassword.ViewComponent;
		XUiC_SimpleButton xuiC_SimpleButton2 = (XUiC_SimpleButton)base.GetChildById("btnOk");
		xuiC_SimpleButton2.OnPressed += this.BtnOk_OnPressed;
		xuiC_SimpleButton2.ViewComponent.NavUpTarget = this.txtPassword.ViewComponent;
		this.txtPassword.ViewComponent.NavDownTarget = xuiC_SimpleButton.ViewComponent;
	}

	// Token: 0x06006663 RID: 26211 RVA: 0x00269150 File Offset: 0x00267350
	[PublicizedFrom(EAccessModifier.Private)]
	public void TextInput_OnInputAbortedHandler(XUiController _sender)
	{
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
	}

	// Token: 0x06006664 RID: 26212 RVA: 0x0029934E File Offset: 0x0029754E
	[PublicizedFrom(EAccessModifier.Private)]
	public void TxtPassword_OnSubmitHandler(XUiController _sender, string _text)
	{
		this.BtnOk_OnPressed(_sender, -1);
	}

	// Token: 0x06006665 RID: 26213 RVA: 0x00299358 File Offset: 0x00297558
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnOk_OnPressed(XUiController _sender, int _mouseButton)
	{
		string text = this.txtPassword.Text;
		bool flag;
		if (this.LockedItem.CheckPassword(text, PlatformManager.InternalLocalUserIdentifier, out flag))
		{
			if (this.LockedItem.LocalPlayerIsOwner())
			{
				if (flag)
				{
					if (text.Length == 0)
					{
						GameManager.ShowTooltip(base.xui.playerUI.entityPlayer, "passcodeRemoved", false, false, 0f);
					}
					else
					{
						GameManager.ShowTooltip(base.xui.playerUI.entityPlayer, "passcodeSet", false, false, 0f);
					}
				}
				Manager.PlayInsidePlayerHead("Misc/password_set", -1, 0f, false, false);
			}
			else
			{
				GameManager.ShowTooltip(base.xui.playerUI.entityPlayer, "passcodeAccepted", false, false, 0f);
				Manager.PlayInsidePlayerHead("Misc/password_pass", -1, 0f, false, false);
			}
			base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
			return;
		}
		Manager.PlayInsidePlayerHead("Misc/password_fail", -1, 0f, false, false);
		GameManager.ShowTooltip(base.xui.playerUI.entityPlayer, "passcodeRejected", false, false, 0f);
	}

	// Token: 0x06006666 RID: 26214 RVA: 0x00269150 File Offset: 0x00267350
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnCancel_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
	}

	// Token: 0x06006667 RID: 26215 RVA: 0x00299480 File Offset: 0x00297680
	public override void OnOpen()
	{
		base.OnOpen();
		base.xui.playerUI.entityPlayer.PlayOneShot("open_sign", false, false, false, null);
		base.xui.playerUI.CursorController.SetNavigationLockView(this.viewComponent, this.txtPassword.ViewComponent);
	}

	// Token: 0x06006668 RID: 26216 RVA: 0x002994D8 File Offset: 0x002976D8
	public override void OnClose()
	{
		base.OnClose();
		base.xui.playerUI.entityPlayer.PlayOneShot("close_sign", false, false, false, null);
		this.LockedItem = null;
		base.xui.playerUI.CursorController.SetNavigationLockView(null, null);
	}

	// Token: 0x06006669 RID: 26217 RVA: 0x00299527 File Offset: 0x00297727
	public static void Open(LocalPlayerUI _playerUi, ILockable _lockedItem)
	{
		_playerUi.xui.FindWindowGroupByName(XUiC_KeypadWindow.ID).GetChildByType<XUiC_KeypadWindow>().LockedItem = _lockedItem;
		_playerUi.windowManager.Open(XUiC_KeypadWindow.ID, true, false, true);
	}

	// Token: 0x04004D45 RID: 19781
	public static string ID = "";

	// Token: 0x04004D46 RID: 19782
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtPassword;

	// Token: 0x04004D47 RID: 19783
	[PublicizedFrom(EAccessModifier.Private)]
	public ILockable LockedItem;
}
