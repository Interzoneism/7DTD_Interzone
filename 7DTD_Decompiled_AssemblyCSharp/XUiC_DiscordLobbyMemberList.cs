using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020002FE RID: 766
[Preserve]
public class XUiC_DiscordLobbyMemberList : XUiC_List<XUiC_DiscordLobbyMemberList.LobbyMember>
{
	// Token: 0x060015BA RID: 5562 RVA: 0x0007FCB8 File Offset: 0x0007DEB8
	public override void Init()
	{
		base.Init();
		DiscordManager.Instance.LobbyMembersChanged += this.InstanceOnLobbyMembersChanged;
		DiscordManager.Instance.CallChanged += this.InstanceOnCallChanged;
		DiscordManager.Instance.CallMembersChanged += this.InstanceOnCallMembersChanged;
		DiscordManager.Instance.VoiceStateChanged += this.InstanceOnVoiceStateChanged;
	}

	// Token: 0x060015BB RID: 5563 RVA: 0x0007FD24 File Offset: 0x0007DF24
	public override void Cleanup()
	{
		base.Cleanup();
		DiscordManager.Instance.LobbyMembersChanged -= this.InstanceOnLobbyMembersChanged;
		DiscordManager.Instance.CallChanged -= this.InstanceOnCallChanged;
		DiscordManager.Instance.CallMembersChanged -= this.InstanceOnCallMembersChanged;
		DiscordManager.Instance.VoiceStateChanged -= this.InstanceOnVoiceStateChanged;
	}

	// Token: 0x060015BC RID: 5564 RVA: 0x0007FD8F File Offset: 0x0007DF8F
	[PublicizedFrom(EAccessModifier.Private)]
	public void InstanceOnLobbyMembersChanged(DiscordManager.LobbyInfo _lobby)
	{
		this.RebuildList(false);
	}

	// Token: 0x060015BD RID: 5565 RVA: 0x0007FD8F File Offset: 0x0007DF8F
	[PublicizedFrom(EAccessModifier.Private)]
	public void InstanceOnCallChanged(DiscordManager.CallInfo _newCall)
	{
		this.RebuildList(false);
	}

	// Token: 0x060015BE RID: 5566 RVA: 0x0007FD8F File Offset: 0x0007DF8F
	[PublicizedFrom(EAccessModifier.Private)]
	public void InstanceOnCallMembersChanged(DiscordManager.CallInfo _call)
	{
		this.RebuildList(false);
	}

	// Token: 0x060015BF RID: 5567 RVA: 0x0007FD98 File Offset: 0x0007DF98
	[PublicizedFrom(EAccessModifier.Private)]
	public void InstanceOnVoiceStateChanged(bool _self, ulong _userId)
	{
		this.RefreshBindingsSelfAndChildren();
	}

	// Token: 0x060015C0 RID: 5568 RVA: 0x0007FDA0 File Offset: 0x0007DFA0
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		this.membersTempCopy.Clear();
		DiscordManager.LobbyInfo activeVoiceLobby = DiscordManager.Instance.ActiveVoiceLobby;
		if (activeVoiceLobby != null)
		{
			activeVoiceLobby.VoiceCall.GetMembers(this.membersTempCopy);
		}
		foreach (ulong userId in this.membersTempCopy)
		{
			this.allEntries.Add(new XUiC_DiscordLobbyMemberList.LobbyMember(DiscordManager.Instance.GetUser(userId)));
		}
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x060015C1 RID: 5569 RVA: 0x0007FE50 File Offset: 0x0007E050
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.allEntries.Count == 0)
		{
			this.RebuildList(false);
		}
	}

	// Token: 0x04000DD5 RID: 3541
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<ulong> membersTempCopy = new List<ulong>();

	// Token: 0x020002FF RID: 767
	[Preserve]
	public class LobbyMember : XUiListEntry<XUiC_DiscordLobbyMemberList.LobbyMember>
	{
		// Token: 0x060015C3 RID: 5571 RVA: 0x0007FE7F File Offset: 0x0007E07F
		public LobbyMember(DiscordManager.DiscordUser _discordUser)
		{
			this.User = _discordUser;
		}

		// Token: 0x060015C4 RID: 5572 RVA: 0x0007FE90 File Offset: 0x0007E090
		public override int CompareTo(XUiC_DiscordLobbyMemberList.LobbyMember _otherEntry)
		{
			if (_otherEntry == null)
			{
				return -1;
			}
			if (this.User.IsLocalAccount)
			{
				return -1;
			}
			if (_otherEntry.User.IsLocalAccount)
			{
				return 1;
			}
			return string.Compare(this.User.DisplayName, _otherEntry.User.DisplayName, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x060015C5 RID: 5573 RVA: 0x0007FEDC File Offset: 0x0007E0DC
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			if (_bindingName == "is_self")
			{
				_value = this.User.IsLocalAccount.ToString();
				return true;
			}
			if (_bindingName == "displayname")
			{
				_value = this.User.DisplayName;
				return true;
			}
			if (_bindingName == "is_speaking")
			{
				_value = this.User.IsSpeaking.ToString();
				return true;
			}
			if (_bindingName == "voice_muted")
			{
				_value = this.User.IsMuted.ToString();
				return true;
			}
			if (!(_bindingName == "output_muted"))
			{
				return false;
			}
			_value = this.User.IsDeafened.ToString();
			return true;
		}

		// Token: 0x060015C6 RID: 5574 RVA: 0x0007FF98 File Offset: 0x0007E198
		public override bool MatchesSearch(string _searchString)
		{
			return this.User.DisplayName.ContainsCaseInsensitive(_searchString);
		}

		// Token: 0x060015C7 RID: 5575 RVA: 0x0007FFAC File Offset: 0x0007E1AC
		[Preserve]
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			if (_bindingName == "is_self")
			{
				_value = false.ToString();
				return true;
			}
			if (_bindingName == "displayname")
			{
				_value = string.Empty;
				return true;
			}
			if (_bindingName == "is_speaking")
			{
				_value = false.ToString();
				return true;
			}
			if (_bindingName == "voice_muted")
			{
				_value = false.ToString();
				return true;
			}
			if (!(_bindingName == "output_muted"))
			{
				return false;
			}
			_value = false.ToString();
			return true;
		}

		// Token: 0x04000DD6 RID: 3542
		public readonly DiscordManager.DiscordUser User;
	}
}
