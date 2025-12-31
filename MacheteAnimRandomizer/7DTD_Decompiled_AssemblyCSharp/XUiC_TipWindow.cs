using System;
using UnityEngine.Scripting;

// Token: 0x02000E70 RID: 3696
[Preserve]
public class XUiC_TipWindow : XUiController
{
	// Token: 0x17000BC9 RID: 3017
	// (get) Token: 0x06007414 RID: 29716 RVA: 0x002F3390 File Offset: 0x002F1590
	// (set) Token: 0x06007415 RID: 29717 RVA: 0x002F3398 File Offset: 0x002F1598
	public string TipText
	{
		get
		{
			return this.tipText;
		}
		set
		{
			this.tipText = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000BCA RID: 3018
	// (get) Token: 0x06007416 RID: 29718 RVA: 0x002F33A8 File Offset: 0x002F15A8
	// (set) Token: 0x06007417 RID: 29719 RVA: 0x002F33B0 File Offset: 0x002F15B0
	public string TipTitle
	{
		get
		{
			return this.tipTitle;
		}
		set
		{
			this.tipTitle = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000BCB RID: 3019
	// (get) Token: 0x06007418 RID: 29720 RVA: 0x002F33C0 File Offset: 0x002F15C0
	// (set) Token: 0x06007419 RID: 29721 RVA: 0x002F33C8 File Offset: 0x002F15C8
	public ToolTipEvent CloseEvent { get; set; }

	// Token: 0x0600741A RID: 29722 RVA: 0x002F33D1 File Offset: 0x002F15D1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		if (bindingName == "tiptext")
		{
			value = this.TipText;
			return true;
		}
		if (!(bindingName == "tiptitle"))
		{
			return false;
		}
		value = this.TipTitle;
		return true;
	}

	// Token: 0x0600741B RID: 29723 RVA: 0x002F3404 File Offset: 0x002F1604
	public override void Init()
	{
		base.Init();
		((XUiV_Button)base.GetChildById("clickable").ViewComponent).Controller.OnPress += this.closeButton_OnPress;
	}

	// Token: 0x0600741C RID: 29724 RVA: 0x00269150 File Offset: 0x00267350
	[PublicizedFrom(EAccessModifier.Private)]
	public void closeButton_OnPress(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
	}

	// Token: 0x0600741D RID: 29725 RVA: 0x0028C056 File Offset: 0x0028A256
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(true);
			this.IsDirty = false;
		}
	}

	// Token: 0x0600741E RID: 29726 RVA: 0x002F3438 File Offset: 0x002F1638
	public static void ShowTip(string _tip, string _title, EntityPlayerLocal _localPlayer, ToolTipEvent _closeEvent)
	{
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_localPlayer);
		if (uiforPlayer != null && uiforPlayer.xui != null && uiforPlayer.windowManager.IsHUDEnabled())
		{
			XUiC_TipWindow childByType = uiforPlayer.xui.FindWindowGroupByName("tipWindow").GetChildByType<XUiC_TipWindow>();
			childByType.TipText = Localization.Get(_tip, false);
			childByType.TipTitle = Localization.Get(_title, false);
			childByType.CloseEvent = _closeEvent;
			uiforPlayer.windowManager.Open("tipWindow", true, false, true);
			uiforPlayer.windowManager.CloseIfOpen("windowpaging");
			uiforPlayer.windowManager.CloseIfOpen("toolbelt");
		}
	}

	// Token: 0x0600741F RID: 29727 RVA: 0x002F34DB File Offset: 0x002F16DB
	public override void OnClose()
	{
		base.OnClose();
		if (this.CloseEvent != null)
		{
			this.CloseEvent.HandleEvent();
			this.CloseEvent = null;
		}
	}

	// Token: 0x04005849 RID: 22601
	[PublicizedFrom(EAccessModifier.Private)]
	public string tipText = "";

	// Token: 0x0400584A RID: 22602
	[PublicizedFrom(EAccessModifier.Private)]
	public string tipTitle = "";

	// Token: 0x0400584B RID: 22603
	[PublicizedFrom(EAccessModifier.Private)]
	public string nextTip = "";
}
