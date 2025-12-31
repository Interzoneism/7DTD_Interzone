using System;
using InControl;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000CAC RID: 3244
[UnityEngine.Scripting.Preserve]
public class XUiC_EmptyInfoWindow : XUiC_InfoWindow
{
	// Token: 0x06006433 RID: 25651 RVA: 0x00289141 File Offset: 0x00287341
	public override void Init()
	{
		base.Init();
		this.descriptionText = (base.GetChildById("descriptionText").ViewComponent as XUiV_Label);
		base.RegisterForInputStyleChanges();
	}

	// Token: 0x06006434 RID: 25652 RVA: 0x0028916A File Offset: 0x0028736A
	public override void OnOpen()
	{
		base.OnOpen();
		this.UpdateDescriptionText();
	}

	// Token: 0x06006435 RID: 25653 RVA: 0x00289178 File Offset: 0x00287378
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InputStyleChanged(PlayerInputManager.InputStyle _oldStyle, PlayerInputManager.InputStyle _newStyle)
	{
		base.InputStyleChanged(_oldStyle, _newStyle);
		this.UpdateDescriptionText();
	}

	// Token: 0x06006436 RID: 25654 RVA: 0x00289188 File Offset: 0x00287388
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateDescriptionText()
	{
		if (!(DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent() && PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard)
		{
			this.descriptionText.Text = Localization.Get("xuiEmptyInfoPanelText", false);
			return;
		}
		PlayerInputManager.InputStyleFromSelectedIconStyle();
		string text = string.Format(Localization.Get("xuiEmptyInfoPanelTextController", false), new object[]
		{
			InControlExtensions.GetGamepadSourceString(InputControlType.Action1),
			InControlExtensions.GetGamepadSourceString(InputControlType.Action3),
			InControlExtensions.GetGamepadSourceString(InputControlType.Action3),
			InControlExtensions.GetGamepadSourceString(InputControlType.Action4),
			InControlExtensions.GetGamepadSourceString(InputControlType.Action4),
			InControlExtensions.GetGamepadSourceString(InputControlType.DPadUp),
			InControlExtensions.GetGamepadSourceString(InputControlType.DPadDown),
			InControlExtensions.GetGamepadSourceString(InputControlType.DPadLeft),
			InControlExtensions.GetGamepadSourceString(InputControlType.DPadRight),
			InControlExtensions.GetGamepadSourceString(InputControlType.RightStickButton),
			InControlExtensions.GetGamepadSourceString(InputControlType.LeftStickButton),
			InControlExtensions.GetGamepadSourceString(InputControlType.Back)
		});
		this.descriptionText.Text = text;
	}

	// Token: 0x04004B6A RID: 19306
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label descriptionText;
}
