using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000D32 RID: 3378
[Preserve]
public class XUiC_MultiplayerWindows : XUiController
{
	// Token: 0x0600693B RID: 26939 RVA: 0x002AB705 File Offset: 0x002A9905
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			this.IsDirty = false;
			base.RefreshBindings(false);
		}
		bool isVisible = base.ViewComponent.IsVisible;
	}

	// Token: 0x0600693C RID: 26940 RVA: 0x002AB730 File Offset: 0x002A9930
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "is_multiplayer")
		{
			_value = (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsSinglePlayer).ToString();
			return true;
		}
		if (_bindingName == "blocked_players_available")
		{
			_value = (BlockedPlayerList.Instance != null).ToString();
			return true;
		}
		if (!(_bindingName == "discord_ready"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = DiscordManager.Instance.IsReady.ToString();
		return true;
	}

	// Token: 0x0600693D RID: 26941 RVA: 0x002AB7B4 File Offset: 0x002A99B4
	public override void UpdateInput()
	{
		base.UpdateInput();
		if (base.xui.playerUI.playerInput.GUIActions.Cancel.WasPressed || base.xui.playerUI.playerInput.PermanentActions.Cancel.WasPressed)
		{
			if (base.xui.currentPopupMenu.ViewComponent.IsVisible)
			{
				base.xui.currentPopupMenu.ClearItems();
				return;
			}
			base.xui.playerUI.windowManager.CloseAllOpenWindows(null, true);
		}
	}
}
