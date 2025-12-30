using System;
using UnityEngine.Scripting;

// Token: 0x02000C5D RID: 3165
[Preserve]
public class XUiC_ConfirmationPrompt : XUiController
{
	// Token: 0x17000A08 RID: 2568
	// (get) Token: 0x06006174 RID: 24948 RVA: 0x00279418 File Offset: 0x00277618
	public bool IsVisible
	{
		get
		{
			return this.viewComponent.IsVisible;
		}
	}

	// Token: 0x06006175 RID: 24949 RVA: 0x00279428 File Offset: 0x00277628
	public override void Init()
	{
		base.Init();
		this.viewComponent.IsVisible = false;
		this.btnCancel = (XUiC_SimpleButton)base.GetChildById("btnPromptCancel");
		this.btnConfirm = (XUiC_SimpleButton)base.GetChildById("btnPromptConfirm");
		this.btnCancel.Button.Controller.OnPress += this.BtnCancel_OnPress;
		this.btnConfirm.Button.Controller.OnPress += this.BtnConfirm_OnPress;
	}

	// Token: 0x06006176 RID: 24950 RVA: 0x002794B5 File Offset: 0x002776B5
	public override void OnOpen()
	{
		base.OnOpen();
		this.viewComponent.IsVisible = false;
	}

	// Token: 0x06006177 RID: 24951 RVA: 0x002794C9 File Offset: 0x002776C9
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnConfirm_OnPress(XUiController _sender, int _mouseButton)
	{
		this.Confirm();
	}

	// Token: 0x06006178 RID: 24952 RVA: 0x002794D1 File Offset: 0x002776D1
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnCancel_OnPress(XUiController _sender, int _mouseButton)
	{
		this.Cancel();
	}

	// Token: 0x06006179 RID: 24953 RVA: 0x002794D9 File Offset: 0x002776D9
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (!this.IsDirty)
		{
			return;
		}
		base.RefreshBindings(true);
		this.IsDirty = false;
	}

	// Token: 0x0600617A RID: 24954 RVA: 0x002794FC File Offset: 0x002776FC
	public void ShowPrompt(string headerText, string bodyText, string cancelText, string confirmText, Action<XUiC_ConfirmationPrompt.Result> callback)
	{
		this.headerText = headerText;
		this.bodyText = bodyText;
		this.cancelText = cancelText;
		this.confirmText = confirmText;
		this.resultHandler = callback;
		this.viewComponent.IsVisible = true;
		base.xui.playerUI.CursorController.SetNavigationLockView(this.viewComponent, this.btnCancel.ViewComponent);
		this.IsDirty = true;
	}

	// Token: 0x0600617B RID: 24955 RVA: 0x00279568 File Offset: 0x00277768
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "headertext")
		{
			_value = this.headerText;
			return true;
		}
		if (_bindingName == "bodytext")
		{
			_value = this.bodyText;
			return true;
		}
		if (_bindingName == "canceltext")
		{
			_value = this.cancelText;
			return true;
		}
		if (_bindingName == "confirmtext")
		{
			_value = this.confirmText;
			return true;
		}
		if (!(_bindingName == "confirmvisible"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = (!string.IsNullOrWhiteSpace(this.confirmText)).ToString();
		return true;
	}

	// Token: 0x0600617C RID: 24956 RVA: 0x00279602 File Offset: 0x00277802
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClosePrompt()
	{
		base.xui.playerUI.CursorController.SetNavigationLockView(null, null);
		this.viewComponent.IsVisible = false;
	}

	// Token: 0x0600617D RID: 24957 RVA: 0x00279627 File Offset: 0x00277827
	public void Confirm()
	{
		if (!this.IsVisible)
		{
			return;
		}
		this.ClosePrompt();
		this.resultHandler(XUiC_ConfirmationPrompt.Result.Confirmed);
	}

	// Token: 0x0600617E RID: 24958 RVA: 0x00279644 File Offset: 0x00277844
	public void Cancel()
	{
		if (!this.IsVisible)
		{
			return;
		}
		this.ClosePrompt();
		this.resultHandler(XUiC_ConfirmationPrompt.Result.Cancelled);
	}

	// Token: 0x0400494B RID: 18763
	[PublicizedFrom(EAccessModifier.Private)]
	public Action<XUiC_ConfirmationPrompt.Result> resultHandler;

	// Token: 0x0400494C RID: 18764
	[PublicizedFrom(EAccessModifier.Private)]
	public string headerText;

	// Token: 0x0400494D RID: 18765
	[PublicizedFrom(EAccessModifier.Private)]
	public string bodyText;

	// Token: 0x0400494E RID: 18766
	[PublicizedFrom(EAccessModifier.Private)]
	public string cancelText;

	// Token: 0x0400494F RID: 18767
	[PublicizedFrom(EAccessModifier.Private)]
	public string confirmText;

	// Token: 0x04004950 RID: 18768
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnCancel;

	// Token: 0x04004951 RID: 18769
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnConfirm;

	// Token: 0x02000C5E RID: 3166
	public enum Result
	{
		// Token: 0x04004953 RID: 18771
		Cancelled,
		// Token: 0x04004954 RID: 18772
		Confirmed
	}
}
