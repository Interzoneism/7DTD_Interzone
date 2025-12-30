using System;
using UnityEngine.Scripting;

// Token: 0x02000C91 RID: 3217
[Preserve]
public class XUiC_DialogStatementWindow : XUiController
{
	// Token: 0x17000A21 RID: 2593
	// (get) Token: 0x06006344 RID: 25412 RVA: 0x00284925 File Offset: 0x00282B25
	// (set) Token: 0x06006345 RID: 25413 RVA: 0x0028492D File Offset: 0x00282B2D
	public Dialog CurrentDialog
	{
		get
		{
			return this.currentDialog;
		}
		set
		{
			this.currentDialog = value;
			base.RefreshBindings(true);
			this.IsDirty = true;
		}
	}

	// Token: 0x06006346 RID: 25414 RVA: 0x00284944 File Offset: 0x00282B44
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("statementLabel");
		if (childById != null)
		{
			this.label = (XUiV_Label)childById.ViewComponent;
		}
		childById = base.GetChildById("backgroundSprite");
		if (childById != null)
		{
			this.backgroundSprite = (XUiV_Sprite)childById.ViewComponent;
		}
	}

	// Token: 0x06006347 RID: 25415 RVA: 0x002842C8 File Offset: 0x002824C8
	public override void OnOpen()
	{
		base.OnOpen();
		base.RefreshBindings(false);
	}

	// Token: 0x06006348 RID: 25416 RVA: 0x00284997 File Offset: 0x00282B97
	public override void OnClose()
	{
		base.OnClose();
		this.currentDialog = null;
	}

	// Token: 0x06006349 RID: 25417 RVA: 0x002849A6 File Offset: 0x00282BA6
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			this.IsDirty = false;
		}
	}

	// Token: 0x0600634A RID: 25418 RVA: 0x002849BE File Offset: 0x00282BBE
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		if (bindingName == "statement")
		{
			value = ((this.currentDialog != null && this.currentDialog.CurrentStatement != null) ? this.currentDialog.CurrentStatement.Text : "");
			return true;
		}
		return false;
	}

	// Token: 0x0600634B RID: 25419 RVA: 0x00284341 File Offset: 0x00282541
	public void Refresh()
	{
		base.RefreshBindings(true);
		this.IsDirty = true;
	}

	// Token: 0x04004ACB RID: 19147
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label label;

	// Token: 0x04004ACC RID: 19148
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite backgroundSprite;

	// Token: 0x04004ACD RID: 19149
	[PublicizedFrom(EAccessModifier.Private)]
	public Dialog currentDialog;
}
