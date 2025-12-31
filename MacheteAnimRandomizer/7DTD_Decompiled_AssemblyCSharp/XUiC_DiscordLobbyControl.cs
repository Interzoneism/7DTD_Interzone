using System;
using Discord.Sdk;
using UnityEngine.Scripting;

// Token: 0x020002FD RID: 765
[Preserve]
public class XUiC_DiscordLobbyControl : XUiController
{
	// Token: 0x060015B1 RID: 5553 RVA: 0x0007FA50 File Offset: 0x0007DC50
	public override void Init()
	{
		base.Init();
		DiscordManager.Instance.LobbyStateChanged += this.onLobbyStateChanged;
		DiscordManager.Instance.CallStatusChanged += this.onCallStatusChanged;
		XUiC_SimpleButton xuiC_SimpleButton = base.GetChildById("btnJoinVoice") as XUiC_SimpleButton;
		if (xuiC_SimpleButton != null)
		{
			xuiC_SimpleButton.OnPressed += this.btnJoinPressed;
		}
		XUiC_SimpleButton xuiC_SimpleButton2 = base.GetChildById("btnLeaveVoice") as XUiC_SimpleButton;
		if (xuiC_SimpleButton2 != null)
		{
			xuiC_SimpleButton2.OnPressed += this.btnLeavePressed;
		}
		XUiController childById = base.GetChildById("btnJoinVoice");
		XUiV_Button xuiV_Button = ((childById != null) ? childById.ViewComponent : null) as XUiV_Button;
		if (xuiV_Button != null)
		{
			xuiV_Button.Controller.OnPress += this.btnJoinPressed;
		}
		XUiController childById2 = base.GetChildById("btnLeaveVoice");
		XUiV_Button xuiV_Button2 = ((childById2 != null) ? childById2.ViewComponent : null) as XUiV_Button;
		if (xuiV_Button2 != null)
		{
			xuiV_Button2.Controller.OnPress += this.btnLeavePressed;
		}
	}

	// Token: 0x060015B2 RID: 5554 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Private)]
	public void onLobbyStateChanged(DiscordManager.LobbyInfo _lobby, bool _isReady, bool _isJoined)
	{
		this.IsDirty = true;
	}

	// Token: 0x060015B3 RID: 5555 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Private)]
	public void onCallStatusChanged(DiscordManager.CallInfo _call, Call.Status _callStatus)
	{
		this.IsDirty = true;
	}

	// Token: 0x060015B4 RID: 5556 RVA: 0x0007FB52 File Offset: 0x0007DD52
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnJoinPressed(XUiController _sender, int _mouseButton)
	{
		DiscordManager.Instance.JoinLobbyVoice(this.lobbyType);
	}

	// Token: 0x060015B5 RID: 5557 RVA: 0x0007FB64 File Offset: 0x0007DD64
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnLeavePressed(XUiController _sender, int _mouseButton)
	{
		DiscordManager.Instance.LeaveLobbyVoice(true);
	}

	// Token: 0x060015B6 RID: 5558 RVA: 0x0007FB71 File Offset: 0x0007DD71
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
	}

	// Token: 0x060015B7 RID: 5559 RVA: 0x0007FB90 File Offset: 0x0007DD90
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "lobby_ready")
		{
			_value = this.lobby.IsReady.ToString();
			return true;
		}
		if (_bindingName == "lobby_joined")
		{
			_value = this.lobby.IsJoined.ToString();
			return true;
		}
		if (_bindingName == "voice_status")
		{
			_value = this.lobby.VoiceCall.Status.ToString();
			return true;
		}
		if (_bindingName == "in_other_voice")
		{
			DiscordManager.LobbyInfo activeVoiceLobby = DiscordManager.Instance.ActiveVoiceLobby;
			_value = (activeVoiceLobby != null && activeVoiceLobby != this.lobby).ToString();
			return true;
		}
		if (!(_bindingName == "any_lobby_unstable_state"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = DiscordManager.Instance.AnyLobbyInUnstableVoiceState.ToString();
		return true;
	}

	// Token: 0x060015B8 RID: 5560 RVA: 0x0007FC7A File Offset: 0x0007DE7A
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "lobby_type")
		{
			this.lobbyType = EnumUtils.Parse<DiscordManager.ELobbyType>(_value, true);
			this.lobby = DiscordManager.Instance.GetLobby(this.lobbyType);
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x04000DD3 RID: 3539
	[PublicizedFrom(EAccessModifier.Private)]
	public DiscordManager.ELobbyType lobbyType;

	// Token: 0x04000DD4 RID: 3540
	[PublicizedFrom(EAccessModifier.Private)]
	public DiscordManager.LobbyInfo lobby;
}
