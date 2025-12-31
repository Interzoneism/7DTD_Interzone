using System;
using System.Collections.Generic;
using Discord.Sdk;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020002F4 RID: 756
[Preserve]
public class XUiC_DiscordFriendsList : XUiC_List<XUiC_DiscordFriendsList.FriendEntry>
{
	// Token: 0x0600157E RID: 5502 RVA: 0x0007E914 File Offset: 0x0007CB14
	public override void Init()
	{
		base.Init();
		DiscordManager.Instance.FriendsListChanged += this.discordFriendsListChanged;
		DiscordManager.Instance.StatusChanged += this.discordStatusChanged;
		for (int i = 0; i < 4; i++)
		{
			XUiC_DiscordFriendsList.sectionsOpened[i] = (i != 3);
		}
		XUiC_SimpleButton xuiC_SimpleButton = base.GetChildById("btnDiscordLinkAccount") as XUiC_SimpleButton;
		if (xuiC_SimpleButton != null)
		{
			xuiC_SimpleButton.OnPressed += delegate(XUiController _, int _)
			{
				XUiC_DiscordLogin.Open(delegate
				{
				}, true, false, false, false, false);
				DiscordManager.Instance.AuthManager.LoginDiscordUser();
			};
		}
	}

	// Token: 0x0600157F RID: 5503 RVA: 0x0007E9A6 File Offset: 0x0007CBA6
	public override void Cleanup()
	{
		base.Cleanup();
		DiscordManager.Instance.FriendsListChanged -= this.discordFriendsListChanged;
		DiscordManager.Instance.StatusChanged -= this.discordStatusChanged;
	}

	// Token: 0x06001580 RID: 5504 RVA: 0x0007E9DA File Offset: 0x0007CBDA
	[PublicizedFrom(EAccessModifier.Private)]
	public void discordFriendsListChanged()
	{
		this.UpdateList();
	}

	// Token: 0x06001581 RID: 5505 RVA: 0x0007E9DA File Offset: 0x0007CBDA
	[PublicizedFrom(EAccessModifier.Private)]
	public void discordStatusChanged(DiscordManager.EDiscordStatus _obj)
	{
		this.UpdateList();
	}

	// Token: 0x06001582 RID: 5506 RVA: 0x0007E9E2 File Offset: 0x0007CBE2
	public void ToggleSectionVisibility(XUiC_DiscordFriendsList.FriendEntry.ESection _section)
	{
		XUiC_DiscordFriendsList.sectionsOpened[(int)_section] = !XUiC_DiscordFriendsList.sectionsOpened[(int)_section];
		this.UpdateList();
	}

	// Token: 0x06001583 RID: 5507 RVA: 0x0007E9FC File Offset: 0x0007CBFC
	public void UpdateList()
	{
		int page = base.Page;
		this.RebuildList(false);
		base.Page = page;
	}

	// Token: 0x06001584 RID: 5508 RVA: 0x0007EA20 File Offset: 0x0007CC20
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		Array.Clear(XUiC_DiscordFriendsList.sectionUserCounts, 0, XUiC_DiscordFriendsList.sectionUserCounts.Length);
		this.users.Clear();
		DiscordManager.Instance.GetFriends(this.users);
		DiscordManager.Instance.GetInServer(this.users);
		foreach (DiscordManager.DiscordUser discordUser in this.users)
		{
			XUiC_DiscordFriendsList.FriendEntry friendEntry = new XUiC_DiscordFriendsList.FriendEntry(discordUser);
			this.allEntries.Add(friendEntry);
			XUiC_DiscordFriendsList.sectionUserCounts[(int)friendEntry.SectionType]++;
		}
		this.users.Clear();
		this.allEntries.Add(this.entryHeaderInServer);
		this.allEntries.Add(this.entryHeaderInGame);
		this.allEntries.Add(this.entryHeaderOnline);
		this.allEntries.Add(this.entryHeaderOffline);
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x06001585 RID: 5509 RVA: 0x0007EB3C File Offset: 0x0007CD3C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void FilterResults(string _textMatch)
	{
		base.FilterResults(_textMatch);
		for (int i = 0; i < 4; i++)
		{
			if (!XUiC_DiscordFriendsList.sectionsOpened[i])
			{
				XUiC_DiscordFriendsList.FriendEntry.ESection sectionType = (XUiC_DiscordFriendsList.FriendEntry.ESection)i;
				this.filteredEntries.RemoveAll((XUiC_DiscordFriendsList.FriendEntry _entry) => _entry.EntryType == XUiC_DiscordFriendsList.FriendEntry.EEntryType.User && _entry.SectionType == sectionType);
			}
		}
	}

	// Token: 0x06001586 RID: 5510 RVA: 0x0007EB8C File Offset: 0x0007CD8C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "self_activity")
		{
			_value = DiscordManager.Instance.Presence.JoinableActivitySet.ToString();
			return true;
		}
		if (_bindingName == "discord_is_ready")
		{
			_value = DiscordManager.Instance.IsReady.ToString();
			return true;
		}
		if (_bindingName == "discord_supports_full_accounts")
		{
			_value = DiscordManager.SupportsFullAccounts.ToString();
			return true;
		}
		if (!(_bindingName == "discordaccountlinked"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		DiscordManager.DiscordUser localUser = DiscordManager.Instance.LocalUser;
		_value = (localUser == null || !localUser.IsProvisionalAccount).ToString();
		return true;
	}

	// Token: 0x06001587 RID: 5511 RVA: 0x0007EC3E File Offset: 0x0007CE3E
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.allEntries.Count == 0)
		{
			this.RebuildList(false);
		}
	}

	// Token: 0x04000DB8 RID: 3512
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly bool[] sectionsOpened = new bool[4];

	// Token: 0x04000DB9 RID: 3513
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly int[] sectionUserCounts = new int[4];

	// Token: 0x04000DBA RID: 3514
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly HashSet<DiscordManager.DiscordUser> users = new HashSet<DiscordManager.DiscordUser>();

	// Token: 0x04000DBB RID: 3515
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly XUiC_DiscordFriendsList.FriendEntry entryHeaderInServer = new XUiC_DiscordFriendsList.FriendEntry(XUiC_DiscordFriendsList.FriendEntry.ESection.InServer);

	// Token: 0x04000DBC RID: 3516
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly XUiC_DiscordFriendsList.FriendEntry entryHeaderInGame = new XUiC_DiscordFriendsList.FriendEntry(XUiC_DiscordFriendsList.FriendEntry.ESection.OnlineInGame);

	// Token: 0x04000DBD RID: 3517
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly XUiC_DiscordFriendsList.FriendEntry entryHeaderOnline = new XUiC_DiscordFriendsList.FriendEntry(XUiC_DiscordFriendsList.FriendEntry.ESection.Online);

	// Token: 0x04000DBE RID: 3518
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly XUiC_DiscordFriendsList.FriendEntry entryHeaderOffline = new XUiC_DiscordFriendsList.FriendEntry(XUiC_DiscordFriendsList.FriendEntry.ESection.Offline);

	// Token: 0x020002F5 RID: 757
	[Preserve]
	public class FriendEntry : XUiListEntry<XUiC_DiscordFriendsList.FriendEntry>
	{
		// Token: 0x17000271 RID: 625
		// (get) Token: 0x0600158A RID: 5514 RVA: 0x0007ECC2 File Offset: 0x0007CEC2
		public XUiC_DiscordFriendsList.FriendEntry.EEntryType EntryType { get; }

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x0600158B RID: 5515 RVA: 0x0007ECCA File Offset: 0x0007CECA
		public XUiC_DiscordFriendsList.FriendEntry.ESection SectionType
		{
			get
			{
				if (this.EntryType != XUiC_DiscordFriendsList.FriendEntry.EEntryType.SectionHeader)
				{
					return this.getUserSectionType();
				}
				return this.sectionHeaderType;
			}
		}

		// Token: 0x0600158C RID: 5516 RVA: 0x0007ECE4 File Offset: 0x0007CEE4
		[PublicizedFrom(EAccessModifier.Private)]
		public XUiC_DiscordFriendsList.FriendEntry.ESection getUserSectionType()
		{
			if (this.User == null)
			{
				return XUiC_DiscordFriendsList.FriendEntry.ESection.Offline;
			}
			if (this.User.InSameSession)
			{
				return XUiC_DiscordFriendsList.FriendEntry.ESection.InServer;
			}
			if (this.User.InGame)
			{
				return XUiC_DiscordFriendsList.FriendEntry.ESection.OnlineInGame;
			}
			XUiC_DiscordFriendsList.FriendEntry.ESection result;
			switch (this.User.DiscordState)
			{
			case StatusType.Online:
				result = XUiC_DiscordFriendsList.FriendEntry.ESection.Online;
				break;
			case StatusType.Offline:
				result = XUiC_DiscordFriendsList.FriendEntry.ESection.Offline;
				break;
			case StatusType.Blocked:
				result = XUiC_DiscordFriendsList.FriendEntry.ESection.Online;
				break;
			case StatusType.Idle:
				result = XUiC_DiscordFriendsList.FriendEntry.ESection.Online;
				break;
			case StatusType.Dnd:
				result = XUiC_DiscordFriendsList.FriendEntry.ESection.Online;
				break;
			case StatusType.Invisible:
				result = XUiC_DiscordFriendsList.FriendEntry.ESection.Online;
				break;
			case StatusType.Streaming:
				result = XUiC_DiscordFriendsList.FriendEntry.ESection.Online;
				break;
			case StatusType.Unknown:
				result = XUiC_DiscordFriendsList.FriendEntry.ESection.Offline;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}

		// Token: 0x0600158D RID: 5517 RVA: 0x0007ED74 File Offset: 0x0007CF74
		public FriendEntry(XUiC_DiscordFriendsList.FriendEntry.ESection _sectionHeaderType)
		{
			this.EntryType = 0;
			this.sectionHeaderType = _sectionHeaderType;
		}

		// Token: 0x0600158E RID: 5518 RVA: 0x0007ED8A File Offset: 0x0007CF8A
		public FriendEntry(DiscordManager.DiscordUser _discordUser)
		{
			this.User = _discordUser;
			this.EntryType = 1;
		}

		// Token: 0x0600158F RID: 5519 RVA: 0x0007EDA0 File Offset: 0x0007CFA0
		public override int CompareTo(XUiC_DiscordFriendsList.FriendEntry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return -1;
			}
			int num = this.SectionType.CompareTo(_otherEntry.SectionType);
			if (num != 0)
			{
				return num;
			}
			num = this.EntryType.CompareTo(_otherEntry.EntryType);
			if (num != 0)
			{
				return num;
			}
			if (_otherEntry.User == null)
			{
				return -1;
			}
			if (this.User == null)
			{
				return 1;
			}
			num = -this.User.InSameSession.CompareTo(_otherEntry.User.InSameSession);
			if (num != 0)
			{
				return num;
			}
			num = -this.User.IsFriend.CompareTo(_otherEntry.User.IsFriend);
			if (num != 0)
			{
				return num;
			}
			num = this.User.DiscordState.CompareTo(_otherEntry.User.DiscordState);
			if (num != 0)
			{
				return num;
			}
			return string.Compare(this.User.DisplayName, _otherEntry.User.DisplayName, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x06001590 RID: 5520 RVA: 0x0007EEA8 File Offset: 0x0007D0A8
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
			if (num <= 2199569601U)
			{
				if (num <= 1339365942U)
				{
					if (num != 765356120U)
					{
						if (num == 1339365942U)
						{
							if (_bindingName == "displayname")
							{
								DiscordManager.DiscordUser user = this.User;
								_value = (((user != null) ? user.DisplayName : null) ?? "");
								return true;
							}
						}
					}
					else if (_bindingName == "has_joinable_activity")
					{
						DiscordManager.DiscordUser user2 = this.User;
						_value = (user2 != null && user2.JoinableActivity).ToString();
						return true;
					}
				}
				else if (num != 1939341032U)
				{
					if (num == 2199569601U)
					{
						if (_bindingName == "section_type")
						{
							_value = this.SectionType.ToStringCached<XUiC_DiscordFriendsList.FriendEntry.ESection>();
							return true;
						}
					}
				}
				else if (_bindingName == "discordstate_icon")
				{
					if (this.User == null)
					{
						_value = "";
					}
					else
					{
						string text;
						switch (this.User.DiscordState)
						{
						case StatusType.Online:
							text = "discord_status_available";
							break;
						case StatusType.Offline:
							text = "discord_status_offline";
							break;
						case StatusType.Blocked:
							text = "";
							break;
						case StatusType.Idle:
							text = "discord_status_idle";
							break;
						case StatusType.Dnd:
							text = "discord_status_dnd";
							break;
						case StatusType.Invisible:
							text = "";
							break;
						case StatusType.Streaming:
							text = "";
							break;
						case StatusType.Unknown:
							text = "";
							break;
						default:
							throw new ArgumentOutOfRangeException();
						}
						_value = text;
					}
					return true;
				}
			}
			else if (num <= 3195183512U)
			{
				if (num != 3070150709U)
				{
					if (num == 3195183512U)
					{
						if (_bindingName == "statustext")
						{
							if (this.User == null)
							{
								_value = "";
							}
							else if (this.User.InSameSession)
							{
								Activity activity = this.User.Activity;
								string text2;
								if (activity == null)
								{
									text2 = null;
								}
								else
								{
									ActivityAssets activityAssets = activity.Assets();
									text2 = ((activityAssets != null) ? activityAssets.LargeText() : null);
								}
								string text3 = text2;
								string text4 = Localization.Get("xuiDiscordSameSession", false);
								_value = (string.IsNullOrEmpty(text3) ? text4 : (text4 + " - " + text3));
							}
							else if (this.User.InGame)
							{
								_value = (this.User.Activity.Details() ?? "");
							}
							else
							{
								_value = this.User.DiscordStateLocalized;
							}
							return true;
						}
					}
				}
				else if (_bindingName == "section_open")
				{
					_value = XUiC_DiscordFriendsList.sectionsOpened[(int)this.SectionType].ToString();
					return true;
				}
			}
			else if (num != 3462032940U)
			{
				if (num == 3575926330U)
				{
					if (_bindingName == "section_user_count")
					{
						_value = XUiC_DiscordFriendsList.sectionUserCounts[(int)this.SectionType].ToString();
						return true;
					}
				}
			}
			else if (_bindingName == "entry_type")
			{
				_value = this.EntryType.ToStringCached<XUiC_DiscordFriendsList.FriendEntry.EEntryType>();
				return true;
			}
			return false;
		}

		// Token: 0x06001591 RID: 5521 RVA: 0x0007F1A3 File Offset: 0x0007D3A3
		public override bool MatchesSearch(string _searchString)
		{
			if (this.EntryType == XUiC_DiscordFriendsList.FriendEntry.EEntryType.SectionHeader)
			{
				return true;
			}
			if (string.IsNullOrEmpty(_searchString))
			{
				return true;
			}
			DiscordManager.DiscordUser user = this.User;
			return user != null && user.DisplayName.ContainsCaseInsensitive(_searchString);
		}

		// Token: 0x06001592 RID: 5522 RVA: 0x0007F1D0 File Offset: 0x0007D3D0
		[Preserve]
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
			if (num <= 2199569601U)
			{
				if (num <= 1339365942U)
				{
					if (num != 765356120U)
					{
						if (num == 1339365942U)
						{
							if (_bindingName == "displayname")
							{
								_value = string.Empty;
								return true;
							}
						}
					}
					else if (_bindingName == "has_joinable_activity")
					{
						_value = false.ToString();
						return true;
					}
				}
				else if (num != 1939341032U)
				{
					if (num == 2199569601U)
					{
						if (_bindingName == "section_type")
						{
							_value = string.Empty;
							return true;
						}
					}
				}
				else if (_bindingName == "discordstate_icon")
				{
					_value = string.Empty;
					return true;
				}
			}
			else if (num <= 3195183512U)
			{
				if (num != 3070150709U)
				{
					if (num == 3195183512U)
					{
						if (_bindingName == "statustext")
						{
							_value = string.Empty;
							return true;
						}
					}
				}
				else if (_bindingName == "section_open")
				{
					_value = true.ToString();
					return true;
				}
			}
			else if (num != 3462032940U)
			{
				if (num == 3575926330U)
				{
					if (_bindingName == "section_user_count")
					{
						_value = 0.ToString();
						return true;
					}
				}
			}
			else if (_bindingName == "entry_type")
			{
				_value = string.Empty;
				return true;
			}
			return false;
		}

		// Token: 0x04000DC0 RID: 3520
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly XUiC_DiscordFriendsList.FriendEntry.ESection sectionHeaderType;

		// Token: 0x04000DC1 RID: 3521
		public readonly DiscordManager.DiscordUser User;

		// Token: 0x020002F6 RID: 758
		public enum EEntryType
		{
			// Token: 0x04000DC3 RID: 3523
			SectionHeader,
			// Token: 0x04000DC4 RID: 3524
			User
		}

		// Token: 0x020002F7 RID: 759
		public enum ESection
		{
			// Token: 0x04000DC6 RID: 3526
			InServer,
			// Token: 0x04000DC7 RID: 3527
			OnlineInGame,
			// Token: 0x04000DC8 RID: 3528
			Online,
			// Token: 0x04000DC9 RID: 3529
			Offline,
			// Token: 0x04000DCA RID: 3530
			Count
		}
	}

	// Token: 0x020002F8 RID: 760
	[Preserve]
	public class DiscordFriendsListEntryController : XUiC_ListEntry<XUiC_DiscordFriendsList.FriendEntry>
	{
		// Token: 0x06001593 RID: 5523 RVA: 0x0007F338 File Offset: 0x0007D538
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
					XUiC_DiscordFriendsList.FriendEntry entry = base.GetEntry();
					if (entry.EntryType == XUiC_DiscordFriendsList.FriendEntry.EEntryType.SectionHeader)
					{
						((XUiC_DiscordFriendsList)this.List).ToggleSectionVisibility(entry.SectionType);
						return;
					}
					bool isPrimaryUI = base.xui.playerUI.isPrimaryUI;
					DiscordManager.DiscordUser user = entry.User;
					XUiC_PopupMenu currentPopupMenu = base.xui.currentPopupMenu;
					currentPopupMenu.Setup(new Vector2i(0, -26), base.ViewComponent);
					if (!isPrimaryUI)
					{
						currentPopupMenu.AddItem(new XUiC_PopupMenuItem.Entry(Localization.Get("xuiDiscordSendMessage", false), "ui_game_symbol_invite", GameManager.Instance.World != null, null, delegate(XUiC_PopupMenuItem.Entry _)
						{
							LocalPlayerUI uiforPrimaryPlayer = LocalPlayerUI.GetUIForPrimaryPlayer();
							uiforPrimaryPlayer.entityPlayer.AimingGun = false;
							uiforPrimaryPlayer.windowManager.Open(XUiC_Chat.ID, true, false, true);
							XUiC_Chat.SetChatTarget(uiforPrimaryPlayer.xui, EChatType.Discord, base.GetEntry().User.ID.ToString());
						}));
					}
					currentPopupMenu.AddItem(new XUiC_PopupMenuItem.Entry(Localization.Get("xuiDiscordSendJoinRequest", false), "ui_game_symbol_send_join_request", user.JoinableActivity && !user.InSameSession, null, delegate(XUiC_PopupMenuItem.Entry _)
					{
						user.SendJoinRequest();
					}));
					if (!isPrimaryUI)
					{
						currentPopupMenu.AddItem(new XUiC_PopupMenuItem.Entry(Localization.Get("xuiDiscordSendInvite", false), "ui_game_symbol_send_invite", DiscordManager.Instance.Presence.JoinableActivitySet && !user.InSameSession, null, delegate(XUiC_PopupMenuItem.Entry _)
						{
							user.SendInvite();
						}));
					}
					if (!isPrimaryUI)
					{
						currentPopupMenu.AddItem(new XUiC_PopupMenuItem.Entry(Localization.Get("xuiDiscordAddGameFriend", false), "ui_game_symbol_add_game_friend", !user.IsFriend && user.GameRelationship != RelationshipType.PendingOutgoing, null, delegate(XUiC_PopupMenuItem.Entry _)
						{
							user.SendFriendRequest(true);
						}));
						XUiC_PopupMenu xuiC_PopupMenu = currentPopupMenu;
						string text = Localization.Get("xuiDiscordAddDiscordFriend", false);
						string iconName = "ui_game_symbol_add_discord_friend";
						bool isEnabled;
						if (user.DiscordRelationship != RelationshipType.Friend && user.DiscordRelationship != RelationshipType.PendingOutgoing)
						{
							DiscordManager.DiscordUser localUser = DiscordManager.Instance.LocalUser;
							isEnabled = (localUser == null || !localUser.IsProvisionalAccount);
						}
						else
						{
							isEnabled = false;
						}
						xuiC_PopupMenu.AddItem(new XUiC_PopupMenuItem.Entry(text, iconName, isEnabled, null, delegate(XUiC_PopupMenuItem.Entry _)
						{
							user.SendFriendRequest(false);
						}));
					}
					currentPopupMenu.AddItem(new XUiC_PopupMenuItem.Entry(Localization.Get("xuiDiscordRemoveFriend", false), "ui_game_symbol_x", user.IsFriend, null, delegate(XUiC_PopupMenuItem.Entry _)
					{
						XUiC_MessageBoxWindowGroup.ShowMessageBox(this.xui, Localization.Get("xuiDiscordRemoveFriendConfirmationTitle", false), string.Format(Localization.Get("xuiDiscordRemoveFriendConfirmationText", false), user.DisplayName), XUiC_MessageBoxWindowGroup.MessageBoxTypes.OkCancel, new Action(user.RemoveFriend), null, false, false, false);
					}));
					currentPopupMenu.AddItem(new XUiC_PopupMenuItem.Entry(Localization.Get("xuiDiscordBlockUser", false), "ui_game_symbol_player_block", user.DiscordRelationship != RelationshipType.Blocked, null, delegate(XUiC_PopupMenuItem.Entry _)
					{
						XUiC_MessageBoxWindowGroup.ShowMessageBox(this.xui, Localization.Get("xuiDiscordBlockUserConfirmationTitle", false), string.Format(Localization.Get("xuiDiscordBlockUserConfirmationText", false), user.DisplayName), XUiC_MessageBoxWindowGroup.MessageBoxTypes.OkCancel, new Action(user.BlockUser), null, false, false, false);
					}));
					if (!isPrimaryUI)
					{
						currentPopupMenu.AddItem(new XUiC_PopupMenuItem.Entry(Localization.Get("xuiDiscordUserVolume", false), "ui_game_symbol_noise", user.Volume, user.InCurrentVoice, null, delegate(XUiC_PopupMenuItem.Entry _, double _value)
						{
							Log.Out(string.Format("[Discord UI] New output volume for user {0}: {1}", user.DisplayName, _value));
							user.Volume = _value;
						}));
					}
				};
			}
		}

		// Token: 0x06001594 RID: 5524 RVA: 0x0007F391 File Offset: 0x0007D591
		public override void SetEntry(XUiC_DiscordFriendsList.FriendEntry _data)
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

		// Token: 0x04000DCB RID: 3531
		[PublicizedFrom(EAccessModifier.Private)]
		public XUiV_Texture avatarTexture;
	}
}
