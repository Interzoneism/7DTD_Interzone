using System;
using GUI_2;
using InControl;
using Platform;

// Token: 0x02000F05 RID: 3845
public class XUiV_GamepadIcon : XUiV_Sprite
{
	// Token: 0x060079C9 RID: 31177 RVA: 0x00317DFC File Offset: 0x00315FFC
	public XUiV_GamepadIcon(string _id) : base(_id)
	{
	}

	// Token: 0x060079CA RID: 31178 RVA: 0x00317E0C File Offset: 0x0031600C
	public override void InitView()
	{
		base.InitView();
		base.UIAtlas = UIUtils.IconAtlas.name;
		PlatformManager.NativePlatform.Input.OnLastInputStyleChanged += this.OnLastInputStyleChanged;
		this.curInput = PlatformManager.NativePlatform.Input.CurrentInputStyle;
	}

	// Token: 0x060079CB RID: 31179 RVA: 0x00317E5F File Offset: 0x0031605F
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnLastInputStyleChanged(PlayerInputManager.InputStyle _style)
	{
		this.curInput = _style;
	}

	// Token: 0x060079CC RID: 31180 RVA: 0x00317E68 File Offset: 0x00316068
	public override void Cleanup()
	{
		base.Cleanup();
		IPlatform nativePlatform = PlatformManager.NativePlatform;
		if (((nativePlatform != null) ? nativePlatform.Input : null) != null)
		{
			PlatformManager.NativePlatform.Input.OnLastInputStyleChanged -= this.OnLastInputStyleChanged;
		}
	}

	// Token: 0x060079CD RID: 31181 RVA: 0x00317E9E File Offset: 0x0031609E
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.curInput != this.lastInputStyle)
		{
			this.lastInputStyle = this.curInput;
			base.UIAtlas = UIUtils.IconAtlas.name;
		}
	}

	// Token: 0x060079CE RID: 31182 RVA: 0x00317ED4 File Offset: 0x003160D4
	public override bool ParseAttribute(string _attribute, string _value, XUiController _parent)
	{
		if (_attribute == "action")
		{
			PlayerAction playerActionByName = base.xui.playerUI.playerInput.GUIActions.GetPlayerActionByName(_value);
			base.SpriteName = UIUtils.GetSpriteName(UIUtils.GetButtonIconForAction(playerActionByName));
			return true;
		}
		return base.ParseAttribute(_attribute, _value, _parent);
	}

	// Token: 0x04005C64 RID: 23652
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerInputManager.InputStyle lastInputStyle = PlayerInputManager.InputStyle.Count;

	// Token: 0x04005C65 RID: 23653
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerInputManager.InputStyle curInput;
}
