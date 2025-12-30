using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000C96 RID: 3222
[Preserve]
public class XUiC_DlcWindow : XUiController
{
	// Token: 0x06006366 RID: 25446 RVA: 0x00285084 File Offset: 0x00283284
	public override void Init()
	{
		base.Init();
		XUiC_DlcWindow.ID = base.WindowGroup.ID;
		XUiC_SimpleButton xuiC_SimpleButton;
		if (base.TryGetChildByIdAndType<XUiC_SimpleButton>("btnBack", out xuiC_SimpleButton))
		{
			xuiC_SimpleButton.OnPressed += this.BtnBack_OnPressed;
		}
		this.dlcList = base.GetChildByType<XUiC_DlcList>();
		if (this.dlcList != null)
		{
			this.dlcList.ListEntryClicked += this.entryClicked;
		}
		base.RegisterForInputStyleChanges();
	}

	// Token: 0x06006367 RID: 25447 RVA: 0x002850F9 File Offset: 0x002832F9
	[PublicizedFrom(EAccessModifier.Private)]
	public void entryClicked(XUiC_ListEntry<XUiC_DlcList.DlcEntry> _entry)
	{
		Log.Out("DLC list entry clicked: " + _entry.GetEntry().Name);
	}

	// Token: 0x06006368 RID: 25448 RVA: 0x00285115 File Offset: 0x00283315
	public override void OnOpen()
	{
		base.OnOpen();
		this.windowGroup.openWindowOnEsc = XUiC_MainMenu.ID;
		this.UpdatePagingButtonIcons(PlatformManager.NativePlatform.Input.CurrentInputStyle);
	}

	// Token: 0x06006369 RID: 25449 RVA: 0x00285142 File Offset: 0x00283342
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnBack_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup, true);
	}

	// Token: 0x0600636A RID: 25450 RVA: 0x00285160 File Offset: 0x00283360
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (base.xui.playerUI.playerInput.GUIActions.WindowPagingRight.WasPressed)
		{
			this.dlcList.PageUp();
			return;
		}
		if (base.xui.playerUI.playerInput.GUIActions.WindowPagingLeft.WasPressed)
		{
			this.dlcList.PageDown();
		}
	}

	// Token: 0x0600636B RID: 25451 RVA: 0x002851CD File Offset: 0x002833CD
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InputStyleChanged(PlayerInputManager.InputStyle _oldStyle, PlayerInputManager.InputStyle _newStyle)
	{
		this.UpdatePagingButtonIcons(_newStyle);
	}

	// Token: 0x0600636C RID: 25452 RVA: 0x002851D8 File Offset: 0x002833D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdatePagingButtonIcons(PlayerInputManager.InputStyle inputStyle)
	{
		bool isVisible = inputStyle != PlayerInputManager.InputStyle.Keyboard && this.dlcList.AllEntries().Count > this.dlcList.PageLength;
		base.GetChildById("LB_Icon").ViewComponent.IsVisible = (base.GetChildById("RB_Icon").ViewComponent.IsVisible = isVisible);
	}

	// Token: 0x04004AD7 RID: 19159
	public static string ID = "";

	// Token: 0x04004AD8 RID: 19160
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DlcList dlcList;
}
