using System;
using UnityEngine.Scripting;

// Token: 0x02000E18 RID: 3608
[Preserve]
public class XUiC_ServerPasswordWindow : XUiController
{
	// Token: 0x060070F4 RID: 28916 RVA: 0x002E0A77 File Offset: 0x002DEC77
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		return _bindingName == "msgText";
	}

	// Token: 0x060070F5 RID: 28917 RVA: 0x002E0A8C File Offset: 0x002DEC8C
	public override void Init()
	{
		base.Init();
		XUiC_ServerPasswordWindow.ID = base.WindowGroup.ID;
		this.lblPassNormal = (XUiV_Label)base.GetChildById("lblPassNormal").ViewComponent;
		this.lblPassIncorrect = (XUiV_Label)base.GetChildById("lblPassIncorrect").ViewComponent;
		this.txtPassword = (XUiC_TextInput)base.GetChildById("txtPassword");
		this.txtPassword.OnSubmitHandler += this.TxtPassword_OnSubmitHandler;
		((XUiC_SimpleButton)base.GetChildById("btnCancel")).OnPressed += this.BtnCancel_OnPressed;
		((XUiC_SimpleButton)base.GetChildById("btnSubmit")).OnPressed += this.BtnSubmit_OnPressed;
	}

	// Token: 0x060070F6 RID: 28918 RVA: 0x002E0B54 File Offset: 0x002DED54
	[PublicizedFrom(EAccessModifier.Private)]
	public void TxtPassword_OnSubmitHandler(XUiController _sender, string _text)
	{
		this.BtnSubmit_OnPressed(_sender, -1);
	}

	// Token: 0x060070F7 RID: 28919 RVA: 0x002E0B5E File Offset: 0x002DED5E
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnSubmit_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.onCancel = null;
		this.onSubmit(this.txtPassword.Text);
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
	}

	// Token: 0x060070F8 RID: 28920 RVA: 0x002E0B9D File Offset: 0x002DED9D
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnCancel_OnPressed(XUiController _sender, int _mouseButton)
	{
		Action action = this.onCancel;
		this.onCancel = null;
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x060070F9 RID: 28921 RVA: 0x0028C056 File Offset: 0x0028A256
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(true);
			this.IsDirty = false;
		}
	}

	// Token: 0x060070FA RID: 28922 RVA: 0x002E0BD6 File Offset: 0x002DEDD6
	public override void OnOpen()
	{
		base.OnOpen();
		base.xui.playerUI.CursorController.SetNavigationLockView(this.viewComponent, null);
	}

	// Token: 0x060070FB RID: 28923 RVA: 0x002E0BFA File Offset: 0x002DEDFA
	public override void OnClose()
	{
		base.OnClose();
		if (this.onCancel != null)
		{
			this.BtnCancel_OnPressed(this, -1);
		}
		base.xui.playerUI.CursorController.SetNavigationLockView(null, null);
		this.txtPassword.Text = "";
	}

	// Token: 0x060070FC RID: 28924 RVA: 0x002E0C3C File Offset: 0x002DEE3C
	public override void UpdateInput()
	{
		base.UpdateInput();
		if (base.xui.playerUI.playerInput != null && base.xui.playerUI.playerInput.PermanentActions.Cancel.WasReleased)
		{
			base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		}
	}

	// Token: 0x060070FD RID: 28925 RVA: 0x002E0CA4 File Offset: 0x002DEEA4
	public static void OpenPasswordWindow(XUi _xuiInstance, bool _badPassword, string _currentPwd, bool _modal, Action<string> _onSubmitDelegate, Action _onCancelDelegate)
	{
		XUiC_ServerPasswordWindow childByType = _xuiInstance.FindWindowGroupByName(XUiC_ServerPasswordWindow.ID).GetChildByType<XUiC_ServerPasswordWindow>();
		childByType.txtPassword.Text = _currentPwd;
		_xuiInstance.playerUI.windowManager.Open(XUiC_ServerPasswordWindow.ID, _modal, false, true);
		childByType.onSubmit = _onSubmitDelegate;
		childByType.onCancel = _onCancelDelegate;
		if (_badPassword)
		{
			childByType.lblPassNormal.IsVisible = false;
			childByType.lblPassIncorrect.IsVisible = true;
			return;
		}
		childByType.lblPassNormal.IsVisible = true;
		childByType.lblPassIncorrect.IsVisible = false;
	}

	// Token: 0x040055D1 RID: 21969
	public static string ID = "";

	// Token: 0x040055D2 RID: 21970
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtPassword;

	// Token: 0x040055D3 RID: 21971
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblPassNormal;

	// Token: 0x040055D4 RID: 21972
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblPassIncorrect;

	// Token: 0x040055D5 RID: 21973
	[PublicizedFrom(EAccessModifier.Private)]
	public Action<string> onSubmit;

	// Token: 0x040055D6 RID: 21974
	[PublicizedFrom(EAccessModifier.Private)]
	public Action onCancel;
}
