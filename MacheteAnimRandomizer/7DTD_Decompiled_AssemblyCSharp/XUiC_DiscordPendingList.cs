using System;
using System.Collections.Generic;
using Discord.Sdk;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000305 RID: 773
[Preserve]
public class XUiC_DiscordPendingList : XUiC_List<XUiC_DiscordPendingList.PendingEntry>
{
	// Token: 0x060015F1 RID: 5617 RVA: 0x0008087C File Offset: 0x0007EA7C
	public override void Init()
	{
		base.Init();
		DiscordManager.Instance.StatusChanged += this.discordStatusChanged;
		DiscordManager.Instance.RelationshipChanged += this.discordRelationshipChanged;
		DiscordManager.Instance.ActivityInviteReceived += this.discordActivityInviteReceived;
	}

	// Token: 0x060015F2 RID: 5618 RVA: 0x000808D4 File Offset: 0x0007EAD4
	public override void Cleanup()
	{
		base.Cleanup();
		DiscordManager.Instance.StatusChanged -= this.discordStatusChanged;
		DiscordManager.Instance.RelationshipChanged -= this.discordRelationshipChanged;
		DiscordManager.Instance.ActivityInviteReceived -= this.discordActivityInviteReceived;
	}

	// Token: 0x060015F3 RID: 5619 RVA: 0x00080929 File Offset: 0x0007EB29
	[PublicizedFrom(EAccessModifier.Private)]
	public void discordStatusChanged(DiscordManager.EDiscordStatus _status)
	{
		if (_status == DiscordManager.EDiscordStatus.Disconnected)
		{
			this.UpdateList();
		}
	}

	// Token: 0x060015F4 RID: 5620 RVA: 0x00080935 File Offset: 0x0007EB35
	[PublicizedFrom(EAccessModifier.Private)]
	public void discordActivityInviteReceived(DiscordManager.DiscordUser _user, bool _cleared, ActivityActionTypes _type)
	{
		this.UpdateList();
	}

	// Token: 0x060015F5 RID: 5621 RVA: 0x00080935 File Offset: 0x0007EB35
	[PublicizedFrom(EAccessModifier.Private)]
	public void discordRelationshipChanged(DiscordManager.DiscordUser _user)
	{
		this.UpdateList();
	}

	// Token: 0x060015F6 RID: 5622 RVA: 0x00080940 File Offset: 0x0007EB40
	public void UpdateList()
	{
		int page = base.Page;
		this.RebuildList(false);
		base.Page = page;
	}

	// Token: 0x060015F7 RID: 5623 RVA: 0x00080964 File Offset: 0x0007EB64
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		this.users.Clear();
		DiscordManager.Instance.GetUsersWithPendingAction(this.users);
		foreach (DiscordManager.DiscordUser discordUser in this.users)
		{
			if (discordUser.PendingFriendRequest)
			{
				this.allEntries.Add(new XUiC_DiscordPendingList.PendingEntry(discordUser, (discordUser.GameRelationship == RelationshipType.PendingIncoming) ? XUiC_DiscordPendingList.PendingEntry.EEntryType.FriendRequestGame : XUiC_DiscordPendingList.PendingEntry.EEntryType.FriendRequestDiscord));
			}
			if (discordUser.PendingIncomingJoinRequest)
			{
				this.allEntries.Add(new XUiC_DiscordPendingList.PendingEntry(discordUser, XUiC_DiscordPendingList.PendingEntry.EEntryType.JoinRequest));
			}
			if (discordUser.PendingIncomingInvite)
			{
				this.allEntries.Add(new XUiC_DiscordPendingList.PendingEntry(discordUser, XUiC_DiscordPendingList.PendingEntry.EEntryType.Invite));
			}
		}
		this.users.Clear();
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x060015F8 RID: 5624 RVA: 0x00080A50 File Offset: 0x0007EC50
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x060015F9 RID: 5625 RVA: 0x00080A5A File Offset: 0x0007EC5A
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.allEntries.Count == 0)
		{
			this.RebuildList(false);
		}
	}

	// Token: 0x04000DE7 RID: 3559
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly HashSet<DiscordManager.DiscordUser> users = new HashSet<DiscordManager.DiscordUser>();

	// Token: 0x02000306 RID: 774
	[Preserve]
	public class PendingEntry : XUiListEntry<XUiC_DiscordPendingList.PendingEntry>
	{
		// Token: 0x060015FB RID: 5627 RVA: 0x00080A89 File Offset: 0x0007EC89
		public PendingEntry(DiscordManager.DiscordUser _discordUser, XUiC_DiscordPendingList.PendingEntry.EEntryType _type)
		{
			this.User = _discordUser;
			this.EntryType = _type;
		}

		// Token: 0x060015FC RID: 5628 RVA: 0x00080AA0 File Offset: 0x0007ECA0
		public override int CompareTo(XUiC_DiscordPendingList.PendingEntry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return -1;
			}
			int num = this.EntryType.CompareTo(_otherEntry.EntryType);
			if (num != 0)
			{
				return num;
			}
			return string.Compare(this.User.DisplayName, _otherEntry.User.DisplayName, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x060015FD RID: 5629 RVA: 0x00080AF0 File Offset: 0x0007ECF0
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			if (_bindingName == "displayname")
			{
				DiscordManager.DiscordUser user = this.User;
				_value = (((user != null) ? user.DisplayName : null) ?? "");
				return true;
			}
			if (!(_bindingName == "entry_type"))
			{
				return false;
			}
			_value = this.EntryType.ToStringCached<XUiC_DiscordPendingList.PendingEntry.EEntryType>();
			return true;
		}

		// Token: 0x060015FE RID: 5630 RVA: 0x00080B48 File Offset: 0x0007ED48
		public override bool MatchesSearch(string _searchString)
		{
			if (string.IsNullOrEmpty(_searchString))
			{
				return true;
			}
			DiscordManager.DiscordUser user = this.User;
			return user != null && user.DisplayName.ContainsCaseInsensitive(_searchString);
		}

		// Token: 0x060015FF RID: 5631 RVA: 0x00080B6B File Offset: 0x0007ED6B
		[Preserve]
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			if (_bindingName == "displayname")
			{
				_value = string.Empty;
				return true;
			}
			if (!(_bindingName == "entry_type"))
			{
				return false;
			}
			_value = string.Empty;
			return true;
		}

		// Token: 0x04000DE8 RID: 3560
		public readonly XUiC_DiscordPendingList.PendingEntry.EEntryType EntryType;

		// Token: 0x04000DE9 RID: 3561
		public readonly DiscordManager.DiscordUser User;

		// Token: 0x02000307 RID: 775
		public enum EEntryType
		{
			// Token: 0x04000DEB RID: 3563
			JoinRequest,
			// Token: 0x04000DEC RID: 3564
			Invite,
			// Token: 0x04000DED RID: 3565
			FriendRequestGame,
			// Token: 0x04000DEE RID: 3566
			FriendRequestDiscord
		}
	}

	// Token: 0x02000308 RID: 776
	[Preserve]
	public class DiscordPendingListEntryController : XUiC_ListEntry<XUiC_DiscordPendingList.PendingEntry>
	{
		// Token: 0x06001600 RID: 5632 RVA: 0x00080B9C File Offset: 0x0007ED9C
		public override void Init()
		{
			base.Init();
			XUiController childById = base.GetChildById("avatar");
			XUiV_Texture xuiV_Texture = ((childById != null) ? childById.ViewComponent : null) as XUiV_Texture;
			if (xuiV_Texture != null)
			{
				this.avatarTexture = xuiV_Texture;
			}
			XUiController childById2 = base.GetChildById("btnAccept");
			XUiV_Button xuiV_Button = ((childById2 != null) ? childById2.ViewComponent : null) as XUiV_Button;
			if (xuiV_Button != null)
			{
				xuiV_Button.Controller.OnPress += delegate(XUiController _, int _)
				{
					XUiC_DiscordPendingList.PendingEntry entry = base.GetEntry();
					DiscordManager.DiscordUser user = entry.User;
					switch (entry.EntryType)
					{
					case XUiC_DiscordPendingList.PendingEntry.EEntryType.JoinRequest:
						user.SendInvite();
						return;
					case XUiC_DiscordPendingList.PendingEntry.EEntryType.Invite:
						user.AcceptInvite(null);
						return;
					case XUiC_DiscordPendingList.PendingEntry.EEntryType.FriendRequestGame:
					case XUiC_DiscordPendingList.PendingEntry.EEntryType.FriendRequestDiscord:
						user.SendFriendRequest(entry.EntryType == XUiC_DiscordPendingList.PendingEntry.EEntryType.FriendRequestGame);
						return;
					default:
						throw new ArgumentOutOfRangeException();
					}
				};
			}
			XUiController childById3 = base.GetChildById("btnDecline");
			XUiV_Button xuiV_Button2 = ((childById3 != null) ? childById3.ViewComponent : null) as XUiV_Button;
			if (xuiV_Button2 != null)
			{
				xuiV_Button2.Controller.OnPress += delegate(XUiController _, int _)
				{
					XUiC_DiscordPendingList.PendingEntry entry = base.GetEntry();
					DiscordManager.DiscordUser user = entry.User;
					switch (entry.EntryType)
					{
					case XUiC_DiscordPendingList.PendingEntry.EEntryType.JoinRequest:
						user.DeclineJoinRequest();
						return;
					case XUiC_DiscordPendingList.PendingEntry.EEntryType.Invite:
						user.DeclineInvite();
						return;
					case XUiC_DiscordPendingList.PendingEntry.EEntryType.FriendRequestGame:
					case XUiC_DiscordPendingList.PendingEntry.EEntryType.FriendRequestDiscord:
						user.DeclineFriendRequest(entry.EntryType == XUiC_DiscordPendingList.PendingEntry.EEntryType.FriendRequestGame);
						return;
					default:
						throw new ArgumentOutOfRangeException();
					}
				};
			}
		}

		// Token: 0x06001601 RID: 5633 RVA: 0x00080C44 File Offset: 0x0007EE44
		public override void SetEntry(XUiC_DiscordPendingList.PendingEntry _data)
		{
			base.SetEntry(_data);
			if (this.avatarTexture == null)
			{
				return;
			}
			XUiV_Texture xuiV_Texture = this.avatarTexture;
			Texture texture;
			if (_data == null)
			{
				texture = null;
			}
			else
			{
				DiscordManager.DiscordUser user = _data.User;
				texture = ((user != null) ? user.Avatar : null);
			}
			xuiV_Texture.Texture = texture;
		}

		// Token: 0x04000DEF RID: 3567
		[PublicizedFrom(EAccessModifier.Private)]
		public XUiV_Texture avatarTexture;
	}
}
