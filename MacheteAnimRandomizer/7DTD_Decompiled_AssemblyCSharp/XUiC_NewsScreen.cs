using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000D3C RID: 3388
[Preserve]
public class XUiC_NewsScreen : XUiController
{
	// Token: 0x06006985 RID: 27013 RVA: 0x002ADDB8 File Offset: 0x002ABFB8
	public override void Init()
	{
		base.Init();
		XUiC_NewsScreen.ID = base.WindowGroup.ID;
		XUiController childById = base.GetChildById("btnContinue");
		if (childById != null && childById.ViewComponent is XUiV_Button)
		{
			childById.OnPress += this.BtnContinue_OnPress;
		}
		this.controllerContinueLabel = (base.GetChildById("continueButtonLabelController").ViewComponent as XUiV_Label);
		if (DeviceFlag.PS5.IsCurrent())
		{
			this.UpdateContinueLabel(PlayerInputManager.InputStyle.PS4);
			return;
		}
		if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX).IsCurrent())
		{
			this.UpdateContinueLabel(PlayerInputManager.InputStyle.XB1);
		}
	}

	// Token: 0x06006986 RID: 27014 RVA: 0x002ADE45 File Offset: 0x002AC045
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InputStyleChanged(PlayerInputManager.InputStyle _oldStyle, PlayerInputManager.InputStyle _newStyle)
	{
		base.InputStyleChanged(_oldStyle, _newStyle);
		base.RefreshBindings(true);
		this.UpdateContinueLabel(_newStyle);
	}

	// Token: 0x06006987 RID: 27015 RVA: 0x002ADE60 File Offset: 0x002AC060
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateContinueLabel(PlayerInputManager.InputStyle _style)
	{
		if (_style != PlayerInputManager.InputStyle.Keyboard)
		{
			string arg;
			if (PlatformManager.NativePlatform.Input.CurrentControllerInputStyle == PlayerInputManager.InputStyle.PS4)
			{
				arg = "[sp=PS5_Button_Options]";
			}
			else
			{
				arg = "[sp=XB_Button_Menu]";
			}
			string text = Localization.Get("xuiNewsContinueController", false).ToUpper();
			text = string.Format(text, arg);
			this.controllerContinueLabel.Text = text;
		}
	}

	// Token: 0x06006988 RID: 27016 RVA: 0x002ADEB6 File Offset: 0x002AC0B6
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnContinue_OnPress(XUiController _sender, int _mousebutton)
	{
		base.xui.playerUI.windowManager.Close(base.WindowGroup, false);
		XUiC_MainMenu.Open(base.xui);
	}

	// Token: 0x06006989 RID: 27017 RVA: 0x002ADEE0 File Offset: 0x002AC0E0
	public override void UpdateInput()
	{
		base.UpdateInput();
		PlayerActionsGUI guiactions = base.xui.playerUI.playerInput.GUIActions;
		if ((guiactions.Apply.WasReleased || guiactions.Cancel.WasReleased) && !base.xui.playerUI.windowManager.IsWindowOpen(XUiC_MessageBoxWindowGroup.ID))
		{
			this.BtnContinue_OnPress(this, -1);
		}
	}

	// Token: 0x0600698A RID: 27018 RVA: 0x00282E9A File Offset: 0x0028109A
	public override void OnOpen()
	{
		base.OnOpen();
		base.RefreshBindings(true);
	}

	// Token: 0x0600698B RID: 27019 RVA: 0x002ADF47 File Offset: 0x002AC147
	public static void Open(XUi _xuiInstance)
	{
		_xuiInstance.playerUI.windowManager.Open(XUiC_NewsScreen.ID, true, true, true);
	}

	// Token: 0x0600698C RID: 27020 RVA: 0x002ADF61 File Offset: 0x002AC161
	public override void OnClose()
	{
		base.OnClose();
		base.xui.playerUI.windowManager.CloseIfOpen(XUiC_MessageBoxWindowGroup.ID);
	}

	// Token: 0x04004F8D RID: 20365
	[PublicizedFrom(EAccessModifier.Private)]
	public static string ID = "";

	// Token: 0x04004F8E RID: 20366
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label controllerContinueLabel;
}
