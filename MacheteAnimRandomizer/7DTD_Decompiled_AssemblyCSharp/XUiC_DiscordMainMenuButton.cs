using System;
using UnityEngine.Scripting;

// Token: 0x02000303 RID: 771
[Preserve]
public class XUiC_DiscordMainMenuButton : XUiController
{
	// Token: 0x060015DD RID: 5597 RVA: 0x00080594 File Offset: 0x0007E794
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("button");
		XUiV_Button xuiV_Button = ((childById != null) ? childById.ViewComponent : null) as XUiV_Button;
		if (xuiV_Button != null)
		{
			xuiV_Button.Controller.OnPress += delegate(XUiController _, int _)
			{
				XUiC_DiscordMainMenuFriends.ToggleWindow(base.xui);
				base.RefreshBindings(false);
			};
		}
	}

	// Token: 0x060015DE RID: 5598 RVA: 0x000805E0 File Offset: 0x0007E7E0
	public override void OnOpen()
	{
		base.OnOpen();
		DiscordManager.Instance.StatusChanged += this.discordStatusChanged;
		DiscordManager.Instance.PendingActionsUpdate += this.discordPendingActionsUpdate;
		this.discordPendingActionsUpdate(DiscordManager.Instance.GetPendingActionsCount());
	}

	// Token: 0x060015DF RID: 5599 RVA: 0x0008062F File Offset: 0x0007E82F
	public override void OnClose()
	{
		this.clearEvents();
		base.OnClose();
	}

	// Token: 0x060015E0 RID: 5600 RVA: 0x0008063D File Offset: 0x0007E83D
	public override void Cleanup()
	{
		this.clearEvents();
		base.Cleanup();
	}

	// Token: 0x060015E1 RID: 5601 RVA: 0x0008064B File Offset: 0x0007E84B
	[PublicizedFrom(EAccessModifier.Private)]
	public void clearEvents()
	{
		DiscordManager.Instance.StatusChanged -= this.discordStatusChanged;
		DiscordManager.Instance.PendingActionsUpdate -= this.discordPendingActionsUpdate;
	}

	// Token: 0x060015E2 RID: 5602 RVA: 0x00080679 File Offset: 0x0007E879
	[PublicizedFrom(EAccessModifier.Private)]
	public void discordStatusChanged(DiscordManager.EDiscordStatus _status)
	{
		base.RefreshBindings(false);
	}

	// Token: 0x060015E3 RID: 5603 RVA: 0x00080682 File Offset: 0x0007E882
	[PublicizedFrom(EAccessModifier.Private)]
	public void discordPendingActionsUpdate(int _pendingActionsCount)
	{
		this.pendingActions = _pendingActionsCount;
		base.RefreshBindings(false);
	}

	// Token: 0x060015E4 RID: 5604 RVA: 0x00080694 File Offset: 0x0007E894
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "discord_ready")
		{
			_value = DiscordManager.Instance.IsReady.ToString();
			return true;
		}
		if (_bindingName == "discord_open")
		{
			_value = XUiC_DiscordMainMenuFriends.IsOpen(base.xui).ToString();
			return true;
		}
		if (!(_bindingName == "pending_actions"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = this.pendingActions.ToString();
		return true;
	}

	// Token: 0x04000DE4 RID: 3556
	[PublicizedFrom(EAccessModifier.Private)]
	public int pendingActions;
}
