using System;
using UnityEngine.Scripting;

// Token: 0x02000C1B RID: 3099
[Preserve]
public class XUiC_CamPositionAdd : XUiController
{
	// Token: 0x06005F25 RID: 24357 RVA: 0x00269E68 File Offset: 0x00268068
	public override void Init()
	{
		base.Init();
		this.parentListWindow = base.GetParentByType<XUiC_CamPositionsList>();
		XUiController childById = base.GetChildById("txtName");
		this.txtName = ((childById != null) ? childById.GetChildByType<XUiC_TextInput>() : null);
		XUiController childById2 = base.GetChildById("txtComment");
		this.txtComment = ((childById2 != null) ? childById2.GetChildByType<XUiC_TextInput>() : null);
		if (this.txtName != null)
		{
			this.txtName.OnChangeHandler += this.Name_OnChangeHandler;
			this.txtName.OnSubmitHandler += this.Inputs_OnSubmitHandler;
			this.txtName.SelectOnTab = this.txtComment;
		}
		if (this.txtComment != null)
		{
			this.txtComment.OnChangeHandler += this.Comment_OnChangeHandler;
			this.txtComment.OnSubmitHandler += this.Inputs_OnSubmitHandler;
			this.txtComment.SelectOnTab = this.txtName;
		}
		XUiC_SimpleButton xuiC_SimpleButton = base.GetChildById("btnAdd") as XUiC_SimpleButton;
		if (xuiC_SimpleButton != null)
		{
			xuiC_SimpleButton.OnPressed += delegate(XUiController _, int _)
			{
				this.add();
			};
		}
	}

	// Token: 0x06005F26 RID: 24358 RVA: 0x00269F75 File Offset: 0x00268175
	[PublicizedFrom(EAccessModifier.Private)]
	public void Name_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		this.name = _text;
		base.RefreshBindings(false);
	}

	// Token: 0x06005F27 RID: 24359 RVA: 0x00269F85 File Offset: 0x00268185
	[PublicizedFrom(EAccessModifier.Private)]
	public void Comment_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		this.comment = _text;
		base.RefreshBindings(false);
	}

	// Token: 0x06005F28 RID: 24360 RVA: 0x00269F95 File Offset: 0x00268195
	[PublicizedFrom(EAccessModifier.Private)]
	public void Inputs_OnSubmitHandler(XUiController _sender, string _text)
	{
		this.add();
	}

	// Token: 0x06005F29 RID: 24361 RVA: 0x00269F9D File Offset: 0x0026819D
	[PublicizedFrom(EAccessModifier.Private)]
	public void clear()
	{
		if (this.txtName != null)
		{
			this.txtName.Text = string.Empty;
		}
		if (this.txtComment != null)
		{
			this.txtComment.Text = string.Empty;
		}
	}

	// Token: 0x06005F2A RID: 24362 RVA: 0x00269FCF File Offset: 0x002681CF
	[PublicizedFrom(EAccessModifier.Private)]
	public void add()
	{
		if (!this.validateInput())
		{
			return;
		}
		this.parentListWindow.Add(this.name, this.comment);
		this.parentListWindow.ShowAddCamPositionWindow = false;
		this.clear();
	}

	// Token: 0x06005F2B RID: 24363 RVA: 0x0026A003 File Offset: 0x00268203
	[PublicizedFrom(EAccessModifier.Private)]
	public bool validateInput()
	{
		return this.name.Trim().Length > 0;
	}

	// Token: 0x06005F2C RID: 24364 RVA: 0x0026A018 File Offset: 0x00268218
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "valid_input")
		{
			_value = this.validateInput().ToString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x06005F2D RID: 24365 RVA: 0x0026A04C File Offset: 0x0026824C
	public override void OnVisibilityChanged(bool _isVisible)
	{
		base.OnVisibilityChanged(_isVisible);
		GUIWindow modalWindow = base.xui.playerUI.windowManager.GetModalWindow();
		if (modalWindow == null)
		{
			return;
		}
		if (!_isVisible)
		{
			if (this.modalWindowGroupEscCloseableBefore != null)
			{
				modalWindow.isEscClosable = this.modalWindowGroupEscCloseableBefore.Value;
			}
			return;
		}
		this.modalWindowGroupEscCloseableBefore = new bool?(modalWindow.isEscClosable);
		modalWindow.isEscClosable = false;
		XUiC_TextInput xuiC_TextInput = this.txtName;
		if (xuiC_TextInput == null)
		{
			return;
		}
		xuiC_TextInput.SelectOrVirtualKeyboard(false);
	}

	// Token: 0x06005F2E RID: 24366 RVA: 0x0026A0C5 File Offset: 0x002682C5
	public override void OnClose()
	{
		base.OnClose();
		this.modalWindowGroupEscCloseableBefore = null;
	}

	// Token: 0x06005F2F RID: 24367 RVA: 0x0026A0DC File Offset: 0x002682DC
	public override void UpdateInput()
	{
		base.UpdateInput();
		if (!this.viewComponent.IsVisible)
		{
			return;
		}
		if (base.xui.playerUI.playerInput.GUIActions.Cancel.WasPressed || base.xui.playerUI.playerInput.PermanentActions.Cancel.WasPressed)
		{
			this.parentListWindow.ShowAddCamPositionWindow = false;
		}
	}

	// Token: 0x040047D2 RID: 18386
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CamPositionsList parentListWindow;

	// Token: 0x040047D3 RID: 18387
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtName;

	// Token: 0x040047D4 RID: 18388
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtComment;

	// Token: 0x040047D5 RID: 18389
	[PublicizedFrom(EAccessModifier.Private)]
	public bool? modalWindowGroupEscCloseableBefore;

	// Token: 0x040047D6 RID: 18390
	[PublicizedFrom(EAccessModifier.Private)]
	public string name = string.Empty;

	// Token: 0x040047D7 RID: 18391
	[PublicizedFrom(EAccessModifier.Private)]
	public string comment = string.Empty;
}
