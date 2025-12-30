using System;
using UnityEngine.Scripting;

// Token: 0x02000304 RID: 772
[Preserve]
public class XUiC_DiscordMainMenuFriends : XUiController
{
	// Token: 0x060015E7 RID: 5607 RVA: 0x00080722 File Offset: 0x0007E922
	public override void Init()
	{
		base.Init();
		XUiC_DiscordMainMenuFriends.id = base.WindowGroup.ID;
	}

	// Token: 0x060015E8 RID: 5608 RVA: 0x0008073C File Offset: 0x0007E93C
	public override void OnOpen()
	{
		base.OnOpen();
		DiscordManager.Instance.PendingActionsUpdate += this.discordPendingActionsUpdate;
		DiscordManager.Instance.ActivityJoining += this.discordActivityJoining;
		this.discordPendingActionsUpdate(DiscordManager.Instance.GetPendingActionsCount());
	}

	// Token: 0x060015E9 RID: 5609 RVA: 0x0008078B File Offset: 0x0007E98B
	public override void OnClose()
	{
		DiscordManager.Instance.PendingActionsUpdate -= this.discordPendingActionsUpdate;
		DiscordManager.Instance.ActivityJoining -= this.discordActivityJoining;
		base.OnClose();
	}

	// Token: 0x060015EA RID: 5610 RVA: 0x000807BF File Offset: 0x0007E9BF
	[PublicizedFrom(EAccessModifier.Private)]
	public void discordPendingActionsUpdate(int _pendingActionsCount)
	{
		this.hasPendingActions = (_pendingActionsCount > 0);
		base.RefreshBindings(false);
	}

	// Token: 0x060015EB RID: 5611 RVA: 0x000807D2 File Offset: 0x0007E9D2
	[PublicizedFrom(EAccessModifier.Private)]
	public void discordActivityJoining()
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup, false);
	}

	// Token: 0x060015EC RID: 5612 RVA: 0x000807F0 File Offset: 0x0007E9F0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "has_pending_actions")
		{
			_value = this.hasPendingActions.ToString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x060015ED RID: 5613 RVA: 0x00080818 File Offset: 0x0007EA18
	public static void ToggleWindow(XUi _xui)
	{
		GUIWindowManager windowManager = _xui.playerUI.windowManager;
		if (windowManager.IsWindowOpen(XUiC_DiscordMainMenuFriends.id))
		{
			windowManager.Close(XUiC_DiscordMainMenuFriends.id);
			return;
		}
		windowManager.Open(XUiC_DiscordMainMenuFriends.id, false, false, true);
	}

	// Token: 0x060015EE RID: 5614 RVA: 0x00080858 File Offset: 0x0007EA58
	public new static bool IsOpen(XUi _xui)
	{
		return _xui.playerUI.windowManager.IsWindowOpen(XUiC_DiscordMainMenuFriends.id);
	}

	// Token: 0x04000DE5 RID: 3557
	[PublicizedFrom(EAccessModifier.Private)]
	public static string id = "";

	// Token: 0x04000DE6 RID: 3558
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasPendingActions;
}
