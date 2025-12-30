using System;
using Discord.Sdk;
using UnityEngine.Scripting;

// Token: 0x02000309 RID: 777
[Preserve]
public class XUiC_DiscordVoiceControls : XUiController
{
	// Token: 0x06001605 RID: 5637 RVA: 0x00080D44 File Offset: 0x0007EF44
	public override void Init()
	{
		base.Init();
		DiscordManager.Instance.LobbyStateChanged += this.onLobbyStateChanged;
		DiscordManager.Instance.CallChanged += this.onCallChanged;
		DiscordManager.Instance.CallStatusChanged += this.onCallStatusChanged;
		DiscordManager.Instance.SelfMuteStateChanged += this.onSelfMuteStateChanged;
		DiscordManager.Instance.VoiceStateChanged += this.onVoiceStateChanged;
		XUiController childById = base.GetChildById("btnMuteMic");
		XUiV_Button xuiV_Button = ((childById != null) ? childById.ViewComponent : null) as XUiV_Button;
		if (xuiV_Button != null)
		{
			xuiV_Button.Controller.OnPress += delegate(XUiController _, int _)
			{
				DiscordManager.Instance.Mute = !DiscordManager.Instance.Mute;
			};
		}
		XUiController childById2 = base.GetChildById("btnMuteOutput");
		XUiV_Button xuiV_Button2 = ((childById2 != null) ? childById2.ViewComponent : null) as XUiV_Button;
		if (xuiV_Button2 != null)
		{
			xuiV_Button2.Controller.OnPress += delegate(XUiController _, int _)
			{
				DiscordManager.Instance.Deaf = !DiscordManager.Instance.Deaf;
			};
		}
	}

	// Token: 0x06001606 RID: 5638 RVA: 0x00080E59 File Offset: 0x0007F059
	[PublicizedFrom(EAccessModifier.Private)]
	public void onVoiceStateChanged(bool _self, ulong _userId)
	{
		if (!_self)
		{
			return;
		}
		this.IsDirty = true;
	}

	// Token: 0x06001607 RID: 5639 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Private)]
	public void onSelfMuteStateChanged(bool _selfMute, bool _selfDeaf)
	{
		this.IsDirty = true;
	}

	// Token: 0x06001608 RID: 5640 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Private)]
	public void onLobbyStateChanged(DiscordManager.LobbyInfo _lobby, bool _isReady, bool _isJoined)
	{
		this.IsDirty = true;
	}

	// Token: 0x06001609 RID: 5641 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Private)]
	public void onCallChanged(DiscordManager.CallInfo _newCall)
	{
		this.IsDirty = true;
	}

	// Token: 0x0600160A RID: 5642 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Private)]
	public void onCallStatusChanged(DiscordManager.CallInfo _call, Call.Status _callStatus)
	{
		this.IsDirty = true;
	}

	// Token: 0x0600160B RID: 5643 RVA: 0x0007FB71 File Offset: 0x0007DD71
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
	}

	// Token: 0x0600160C RID: 5644 RVA: 0x00080E68 File Offset: 0x0007F068
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		DiscordManager instance = DiscordManager.Instance;
		if (_bindingName == "in_voice")
		{
			_value = (instance.ActiveVoiceLobby != null).ToString();
			return true;
		}
		if (_bindingName == "current_voice_lobby")
		{
			DiscordManager.LobbyInfo activeVoiceLobby = instance.ActiveVoiceLobby;
			_value = (((activeVoiceLobby != null) ? activeVoiceLobby.LobbyType.ToStringCached<DiscordManager.ELobbyType>() : null) ?? "None");
			return true;
		}
		if (_bindingName == "voice_muted")
		{
			_value = instance.Mute.ToString();
			return true;
		}
		if (_bindingName == "output_muted")
		{
			_value = instance.Deaf.ToString();
			return true;
		}
		if (!(_bindingName == "is_speaking"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		DiscordManager.LobbyInfo activeVoiceLobby2 = instance.ActiveVoiceLobby;
		_value = ((activeVoiceLobby2 != null) ? activeVoiceLobby2.VoiceCall.IsSpeaking.ToString() : null);
		return true;
	}
}
