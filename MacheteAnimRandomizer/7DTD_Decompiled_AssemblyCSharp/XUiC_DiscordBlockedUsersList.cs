using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020002F0 RID: 752
[Preserve]
public class XUiC_DiscordBlockedUsersList : XUiC_List<XUiC_DiscordBlockedUsersList.Entry>
{
	// Token: 0x0600156A RID: 5482 RVA: 0x0007E594 File Offset: 0x0007C794
	public override void Init()
	{
		base.Init();
		DiscordManager.Instance.FriendsListChanged += this.discordFriendsListChanged;
	}

	// Token: 0x0600156B RID: 5483 RVA: 0x0007E5B2 File Offset: 0x0007C7B2
	public override void Cleanup()
	{
		base.Cleanup();
		DiscordManager.Instance.FriendsListChanged -= this.discordFriendsListChanged;
	}

	// Token: 0x0600156C RID: 5484 RVA: 0x0007E5D0 File Offset: 0x0007C7D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void discordFriendsListChanged()
	{
		this.UpdateList();
	}

	// Token: 0x0600156D RID: 5485 RVA: 0x0007E5D8 File Offset: 0x0007C7D8
	public void UpdateList()
	{
		int page = base.Page;
		this.RebuildList(false);
		base.Page = page;
	}

	// Token: 0x0600156E RID: 5486 RVA: 0x0007E5FC File Offset: 0x0007C7FC
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		this.users.Clear();
		DiscordManager.Instance.GetBlockedUsers(this.users);
		foreach (DiscordManager.DiscordUser discordUser in this.users)
		{
			XUiC_DiscordBlockedUsersList.Entry item = new XUiC_DiscordBlockedUsersList.Entry(discordUser);
			this.allEntries.Add(item);
		}
		this.users.Clear();
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x0600156F RID: 5487 RVA: 0x0007E69C File Offset: 0x0007C89C
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.allEntries.Count == 0)
		{
			this.RebuildList(false);
		}
	}

	// Token: 0x04000DB4 RID: 3508
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly HashSet<DiscordManager.DiscordUser> users = new HashSet<DiscordManager.DiscordUser>();

	// Token: 0x020002F1 RID: 753
	[Preserve]
	public class Entry : XUiListEntry<XUiC_DiscordBlockedUsersList.Entry>
	{
		// Token: 0x06001571 RID: 5489 RVA: 0x0007E6CB File Offset: 0x0007C8CB
		public Entry(DiscordManager.DiscordUser _discordUser)
		{
			this.User = _discordUser;
		}

		// Token: 0x06001572 RID: 5490 RVA: 0x0007E6DA File Offset: 0x0007C8DA
		public override int CompareTo(XUiC_DiscordBlockedUsersList.Entry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return -1;
			}
			if (_otherEntry.User == null)
			{
				return -1;
			}
			if (this.User == null)
			{
				return 1;
			}
			return string.Compare(this.User.DisplayName, _otherEntry.User.DisplayName, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x06001573 RID: 5491 RVA: 0x0007E711 File Offset: 0x0007C911
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			if (_bindingName == "displayname")
			{
				DiscordManager.DiscordUser user = this.User;
				_value = (((user != null) ? user.DisplayName : null) ?? "");
				return true;
			}
			return false;
		}

		// Token: 0x06001574 RID: 5492 RVA: 0x0007E740 File Offset: 0x0007C940
		public override bool MatchesSearch(string _searchString)
		{
			if (string.IsNullOrEmpty(_searchString))
			{
				return true;
			}
			DiscordManager.DiscordUser user = this.User;
			return user != null && user.DisplayName.ContainsCaseInsensitive(_searchString);
		}

		// Token: 0x06001575 RID: 5493 RVA: 0x0007E763 File Offset: 0x0007C963
		[Preserve]
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			if (_bindingName == "displayname")
			{
				_value = string.Empty;
				return true;
			}
			return false;
		}

		// Token: 0x04000DB5 RID: 3509
		public readonly DiscordManager.DiscordUser User;
	}

	// Token: 0x020002F2 RID: 754
	[Preserve]
	public class DiscordBlockedUsersListEntryController : XUiC_ListEntry<XUiC_DiscordBlockedUsersList.Entry>
	{
		// Token: 0x06001576 RID: 5494 RVA: 0x0007E77C File Offset: 0x0007C97C
		public override void Init()
		{
			base.Init();
			XUiController childById = base.GetChildById("avatar");
			XUiV_Texture xuiV_Texture = ((childById != null) ? childById.ViewComponent : null) as XUiV_Texture;
			if (xuiV_Texture != null)
			{
				this.avatarTexture = xuiV_Texture;
			}
			if (base.ViewComponent.EventOnPress)
			{
				base.OnPress += delegate(XUiController _, int _)
				{
					XUiC_DiscordBlockedUsersList.Entry entry = base.GetEntry();
					DiscordManager.DiscordUser user = entry.User;
					XUiC_PopupMenu currentPopupMenu = base.xui.currentPopupMenu;
					currentPopupMenu.Setup(new Vector2i(0, -26), base.ViewComponent);
					currentPopupMenu.AddItem(new XUiC_PopupMenuItem.Entry(Localization.Get("xuiDiscordAddGameFriend", false), "ui_game_symbol_add_game_friend", true, null, delegate(XUiC_PopupMenuItem.Entry _)
					{
						user.SendFriendRequest(true);
					}));
					string text = Localization.Get("xuiDiscordAddDiscordFriend", false);
					string iconName = "ui_game_symbol_add_discord_friend";
					DiscordManager.DiscordUser localUser = DiscordManager.Instance.LocalUser;
					currentPopupMenu.AddItem(new XUiC_PopupMenuItem.Entry(text, iconName, localUser == null || !localUser.IsProvisionalAccount, null, delegate(XUiC_PopupMenuItem.Entry _)
					{
						user.SendFriendRequest(false);
					}));
					currentPopupMenu.AddItem(new XUiC_PopupMenuItem.Entry(Localization.Get("xuiDiscordUnblockUser", false), "ui_game_symbol_modded", true, null, delegate(XUiC_PopupMenuItem.Entry _)
					{
						user.UnblockUser();
					}));
				};
			}
		}

		// Token: 0x06001577 RID: 5495 RVA: 0x0007E7D5 File Offset: 0x0007C9D5
		public override void SetEntry(XUiC_DiscordBlockedUsersList.Entry _data)
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

		// Token: 0x04000DB6 RID: 3510
		[PublicizedFrom(EAccessModifier.Private)]
		public XUiV_Texture avatarTexture;
	}
}
