using System;
using UnityEngine.Scripting;

// Token: 0x02000C8E RID: 3214
[Preserve]
public class XUiC_DialogRespondentName : XUiController
{
	// Token: 0x17000A1E RID: 2590
	// (get) Token: 0x0600632B RID: 25387 RVA: 0x002842A9 File Offset: 0x002824A9
	// (set) Token: 0x0600632C RID: 25388 RVA: 0x002842B1 File Offset: 0x002824B1
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

	// Token: 0x0600632D RID: 25389 RVA: 0x002842C8 File Offset: 0x002824C8
	public override void OnOpen()
	{
		base.OnOpen();
		base.RefreshBindings(false);
	}

	// Token: 0x0600632E RID: 25390 RVA: 0x002842D7 File Offset: 0x002824D7
	public override void OnClose()
	{
		base.OnClose();
		this.currentDialog = null;
	}

	// Token: 0x0600632F RID: 25391 RVA: 0x002842E8 File Offset: 0x002824E8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		if (bindingName == "respondentname")
		{
			value = ((base.xui.Dialog.Respondent != null) ? Localization.Get(base.xui.Dialog.Respondent.EntityName, false) : "");
			return true;
		}
		return false;
	}

	// Token: 0x06006330 RID: 25392 RVA: 0x00284341 File Offset: 0x00282541
	public void Refresh()
	{
		base.RefreshBindings(true);
		this.IsDirty = true;
	}

	// Token: 0x04004ABA RID: 19130
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label label;

	// Token: 0x04004ABB RID: 19131
	[PublicizedFrom(EAccessModifier.Private)]
	public Dialog currentDialog;
}
