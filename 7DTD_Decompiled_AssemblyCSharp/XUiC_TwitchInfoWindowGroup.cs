using System;
using GUI_2;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x02000E8E RID: 3726
[Preserve]
public class XUiC_TwitchInfoWindowGroup : XUiController
{
	// Token: 0x06007578 RID: 30072 RVA: 0x002FDA2B File Offset: 0x002FBC2B
	public override void Init()
	{
		base.Init();
		this.descriptionWindow = base.GetChildByType<XUiC_TwitchEntryDescriptionWindow>();
		this.entryListWindow = base.GetChildByType<XUiC_TwitchEntryListWindow>();
		this.descriptionWindow.OwnerList = this.entryListWindow;
	}

	// Token: 0x06007579 RID: 30073 RVA: 0x002FDA5C File Offset: 0x002FBC5C
	public void SetEntry(XUiC_TwitchActionEntry ta)
	{
		this.descriptionWindow.SetTwitchAction(ta);
	}

	// Token: 0x0600757A RID: 30074 RVA: 0x002FDA6A File Offset: 0x002FBC6A
	public void SetEntry(XUiC_TwitchVoteInfoEntry tv)
	{
		this.descriptionWindow.SetTwitchVote(tv);
	}

	// Token: 0x0600757B RID: 30075 RVA: 0x002FDA78 File Offset: 0x002FBC78
	public void SetEntry(XUiC_TwitchActionHistoryEntry th)
	{
		this.descriptionWindow.SetTwitchHistory(th);
	}

	// Token: 0x0600757C RID: 30076 RVA: 0x002FDA86 File Offset: 0x002FBC86
	public void ClearEntries()
	{
		this.descriptionWindow.ClearEntries();
	}

	// Token: 0x0600757D RID: 30077 RVA: 0x00272183 File Offset: 0x00270383
	public override void Update(float _dt)
	{
		base.Update(_dt);
	}

	// Token: 0x0600757E RID: 30078 RVA: 0x002FDA94 File Offset: 0x002FBC94
	public override void OnOpen()
	{
		base.OnOpen();
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonSouth, "igcoSelect", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonEast, "igcoExit", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu, 0f);
		base.GetChildByType<XUiC_WindowNonPagingHeader>();
		TwitchManager.Current.HasViewedSettings = true;
	}

	// Token: 0x0600757F RID: 30079 RVA: 0x002FDB0E File Offset: 0x002FBD0E
	public override void OnClose()
	{
		base.OnClose();
		base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.playerUI.windowManager.CloseIfOpen("twitchwindowpaging");
	}

	// Token: 0x0400599B RID: 22939
	public XUiC_TwitchEntryDescriptionWindow descriptionWindow;

	// Token: 0x0400599C RID: 22940
	public XUiC_TwitchEntryListWindow entryListWindow;
}
