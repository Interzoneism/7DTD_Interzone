using System;
using GUI_2;
using InControl;
using UnityEngine;

// Token: 0x02000EDA RID: 3802
public class XUi_FallThrough : MonoBehaviour
{
	// Token: 0x060077FE RID: 30718 RVA: 0x0030DA7B File Offset: 0x0030BC7B
	public void SetXUi(XUi _xui)
	{
		this.xui = _xui;
	}

	// Token: 0x060077FF RID: 30719 RVA: 0x0030DA84 File Offset: 0x0030BC84
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		UICamera.fallThrough = base.gameObject;
	}

	// Token: 0x06007800 RID: 30720 RVA: 0x0030DA94 File Offset: 0x0030BC94
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		bool flag = this.xui.dragAndDrop != null && !this.xui.dragAndDrop.CurrentStack.IsEmpty();
		bool flag2 = flag && UICamera.hoveredObject != null && UICamera.hoveredObject.name.EqualsCaseInsensitive("xui") && this.xui.dragAndDrop.CurrentStack.itemValue.ItemClassOrMissing.CanDrop(null);
		int num = (this.xui.dragAndDrop == null) ? 0 : this.xui.dragAndDrop.CurrentStack.count;
		bool flag3 = false;
		LocalPlayerUI playerUI = this.xui.playerUI;
		if (null != playerUI && null != playerUI.uiCamera && playerUI.playerInput != null && playerUI.playerInput.GUIActions != null)
		{
			PlayerActionsGUI guiactions = playerUI.playerInput.GUIActions;
			bool flag4 = false;
			bool flag5 = false;
			if (guiactions.LastInputType == BindingSourceType.DeviceBindingSource)
			{
				flag4 |= guiactions.Submit.WasReleased;
				flag5 |= guiactions.HalfStack.WasReleased;
			}
			else
			{
				flag4 |= playerUI.CursorController.GetMouseButtonDown(UICamera.MouseButton.LeftButton);
				flag5 |= playerUI.CursorController.GetMouseButtonDown(UICamera.MouseButton.RightButton);
			}
			if (flag2)
			{
				if (flag4 || (num == 1 && flag5))
				{
					this.xui.dragAndDrop.DropCurrentItem();
					flag3 = true;
				}
				else if (num > 1 && flag5)
				{
					this.xui.dragAndDrop.DropCurrentItem(1);
					num--;
					flag3 = true;
				}
			}
		}
		if (flag2 != this.canDrop || flag3)
		{
			this.canDrop = flag2;
			this.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuHoverAir);
			if (flag && this.canDrop)
			{
				if (num > 1)
				{
					this.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonSouth, "igcoDropAll", XUiC_GamepadCalloutWindow.CalloutType.MenuHoverAir);
					this.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonWest, "igcoDropOne", XUiC_GamepadCalloutWindow.CalloutType.MenuHoverAir);
				}
				else
				{
					this.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonSouth, "igcoDrop", XUiC_GamepadCalloutWindow.CalloutType.MenuHoverAir);
				}
				this.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuHoverItem);
				this.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuHoverAir, 0f);
				return;
			}
			this.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuHoverAir);
			this.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuHoverItem, 0f);
		}
	}

	// Token: 0x04005B84 RID: 23428
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public XUi xui;

	// Token: 0x04005B85 RID: 23429
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool canDrop;
}
