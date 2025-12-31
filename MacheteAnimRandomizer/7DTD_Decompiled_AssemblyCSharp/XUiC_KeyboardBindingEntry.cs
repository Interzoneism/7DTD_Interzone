using System;
using InControl;
using UnityEngine.Scripting;

// Token: 0x02000D4A RID: 3402
[UnityEngine.Scripting.Preserve]
public class XUiC_KeyboardBindingEntry : XUiController
{
	// Token: 0x06006A2A RID: 27178 RVA: 0x002B3100 File Offset: 0x002B1300
	public override void Init()
	{
		base.Init();
		this.label = (base.GetChildById("label").ViewComponent as XUiV_Label);
		this.value = (base.GetChildById("value").ViewComponent as XUiV_Label);
		this.unbind = (base.GetChildById("unbind").ViewComponent as XUiV_Button);
		this.button = (base.GetChildById("background").ViewComponent as XUiV_Button);
	}

	// Token: 0x06006A2B RID: 27179 RVA: 0x002B3180 File Offset: 0x002B1380
	public void SetAction(PlayerAction _action)
	{
		this.action = _action;
		PlayerActionData.ActionUserData actionUserData = (PlayerActionData.ActionUserData)_action.UserData;
		base.ViewComponent.UiTransform.gameObject.name = "Entry_" + actionUserData.LocalizedName;
		this.label.Text = actionUserData.LocalizedName;
		this.button.ToolTip = actionUserData.LocalizedDescription;
		if (actionUserData.allowRebind)
		{
			this.unbind.ToolTip = Localization.Get("xuiRemoveBinding", false);
			return;
		}
		this.unbind.ForceHide = true;
		this.unbind.IsNavigatable = (this.unbind.IsSnappable = (this.unbind.IsVisible = false));
		this.unbind.UiTransform.gameObject.SetActive(false);
		this.button.ForceHide = true;
		this.button.IsNavigatable = (this.button.IsSnappable = (this.button.IsVisible = false));
		this.button.UiTransform.gameObject.SetActive(false);
	}

	// Token: 0x06006A2C RID: 27180 RVA: 0x002B329C File Offset: 0x002B149C
	public void Hide()
	{
		base.ViewComponent.UiTransform.gameObject.name = "Hidden Entry";
		this.unbind.ForceHide = true;
		this.unbind.IsNavigatable = (this.unbind.IsSnappable = (this.unbind.IsVisible = false));
		this.button.ForceHide = true;
		this.button.IsNavigatable = (this.button.IsSnappable = (this.button.IsVisible = false));
		this.button.UiTransform.gameObject.SetActive(false);
		this.unbind.UiTransform.gameObject.SetActive(false);
	}

	// Token: 0x04005012 RID: 20498
	public PlayerAction action;

	// Token: 0x04005013 RID: 20499
	public XUiV_Label label;

	// Token: 0x04005014 RID: 20500
	public XUiV_Label value;

	// Token: 0x04005015 RID: 20501
	public XUiV_Button unbind;

	// Token: 0x04005016 RID: 20502
	public XUiV_Button button;
}
