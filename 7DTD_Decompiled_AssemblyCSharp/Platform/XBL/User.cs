using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Platform.EOS;
using Unity.XGamingRuntime;

namespace Platform.XBL
{
	// Token: 0x020018A0 RID: 6304
	public class User : XblUser, IUserClient
	{
		// Token: 0x1700152A RID: 5418
		// (get) Token: 0x0600BA0F RID: 47631 RVA: 0x0046FF92 File Offset: 0x0046E192
		// (set) Token: 0x0600BA10 RID: 47632 RVA: 0x0046FF9A File Offset: 0x0046E19A
		public SocialManagerXbl SocialManager { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700152B RID: 5419
		// (get) Token: 0x0600BA11 RID: 47633 RVA: 0x0046FFA3 File Offset: 0x0046E1A3
		// (set) Token: 0x0600BA12 RID: 47634 RVA: 0x0046FFAB File Offset: 0x0046E1AB
		public FriendsListXbl FriendsList { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700152C RID: 5420
		// (get) Token: 0x0600BA13 RID: 47635 RVA: 0x0046FFB4 File Offset: 0x0046E1B4
		// (set) Token: 0x0600BA14 RID: 47636 RVA: 0x0046FFBC File Offset: 0x0046E1BC
		public XblSandboxHelper SandboxHelper { get; [PublicizedFrom(EAccessModifier.Private)] set; } = new XblSandboxHelper();

		// Token: 0x1700152D RID: 5421
		// (get) Token: 0x0600BA15 RID: 47637 RVA: 0x0046FFC5 File Offset: 0x0046E1C5
		// (set) Token: 0x0600BA16 RID: 47638 RVA: 0x0046FFCD File Offset: 0x0046E1CD
		public MultiplayerActivityQueryManager MultiplayerActivityQueryManager { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600BA17 RID: 47639 RVA: 0x0046FFD8 File Offset: 0x0046E1D8
		[PublicizedFrom(EAccessModifier.Private)]
		static User()
		{
			Dictionary<EBlockType, XblPermission> dictionary = EnumUtils.Values<EBlockType>().ToDictionary((EBlockType blockType) => blockType, delegate(EBlockType blockType)
			{
				XblPermission result;
				switch (blockType)
				{
				case EBlockType.TextChat:
					result = XblPermission.CommunicateUsingText;
					break;
				case EBlockType.VoiceChat:
					result = XblPermission.CommunicateUsingVoice;
					break;
				case EBlockType.Play:
					result = XblPermission.PlayMultiplayer;
					break;
				default:
					throw new NotImplementedException(string.Format("Mapping from {0}.{1} to {2} not implemented!", "EBlockType", blockType, "XblPermission"));
				}
				return result;
			});
			User.userBlockedPermissions = dictionary.Values.ToArray<XblPermission>();
			User.xblPermissionToBlockType = new EnumDictionary<XblPermission, EBlockType>();
			foreach (KeyValuePair<EBlockType, XblPermission> keyValuePair in dictionary)
			{
				EBlockType eblockType;
				XblPermission xblPermission;
				keyValuePair.Deconstruct(out eblockType, out xblPermission);
				EBlockType value = eblockType;
				XblPermission key = xblPermission;
				User.xblPermissionToBlockType.Add(key, value);
			}
			User.userBlockedAnonymousTypes = new XblAnonymousUserType[]
			{
				XblAnonymousUserType.CrossNetworkFriend,
				XblAnonymousUserType.CrossNetworkUser
			};
		}

		// Token: 0x0600BA18 RID: 47640 RVA: 0x004700BC File Offset: 0x0046E2BC
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
			this.owner.Api.ClientApiInitialized += this.apiInitialized;
			if (!GameManager.IsDedicatedServer)
			{
				PlatformManager.CrossplatformPlatform.User.UserLoggedIn += this.CrossLoginDone;
				XblXuidMapper.XuidMapped += this.OnXuidMapped;
			}
		}

		// Token: 0x1700152E RID: 5422
		// (get) Token: 0x0600BA19 RID: 47641 RVA: 0x0047011F File Offset: 0x0046E31F
		// (set) Token: 0x0600BA1A RID: 47642 RVA: 0x00470127 File Offset: 0x0046E327
		public EUserStatus UserStatus { get; [PublicizedFrom(EAccessModifier.Private)] set; } = EUserStatus.NotAttempted;

		// Token: 0x1400011D RID: 285
		// (add) Token: 0x0600BA1B RID: 47643 RVA: 0x00470130 File Offset: 0x0046E330
		// (remove) Token: 0x0600BA1C RID: 47644 RVA: 0x00470190 File Offset: 0x0046E390
		public event Action<IPlatform> UserLoggedIn
		{
			add
			{
				lock (this)
				{
					this.userLoggedIn = (Action<IPlatform>)Delegate.Combine(this.userLoggedIn, value);
					if (this.UserStatus == EUserStatus.LoggedIn)
					{
						value(this.owner);
					}
				}
			}
			remove
			{
				lock (this)
				{
					this.userLoggedIn = (Action<IPlatform>)Delegate.Remove(this.userLoggedIn, value);
				}
			}
		}

		// Token: 0x1400011E RID: 286
		// (add) Token: 0x0600BA1D RID: 47645 RVA: 0x004701DC File Offset: 0x0046E3DC
		// (remove) Token: 0x0600BA1E RID: 47646 RVA: 0x00470214 File Offset: 0x0046E414
		public event UserBlocksChangedCallback UserBlocksChanged;

		// Token: 0x1700152F RID: 5423
		// (get) Token: 0x0600BA1F RID: 47647 RVA: 0x00470249 File Offset: 0x0046E449
		// (set) Token: 0x0600BA20 RID: 47648 RVA: 0x00470251 File Offset: 0x0046E451
		public XUserHandle GdkUserHandle { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001530 RID: 5424
		// (get) Token: 0x0600BA21 RID: 47649 RVA: 0x0047025A File Offset: 0x0046E45A
		// (set) Token: 0x0600BA22 RID: 47650 RVA: 0x00470262 File Offset: 0x0046E462
		public XblContextHandle XblContextHandle { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001531 RID: 5425
		// (get) Token: 0x0600BA23 RID: 47651 RVA: 0x0047026B File Offset: 0x0046E46B
		public PlatformUserIdentifierAbs PlatformUserId
		{
			get
			{
				return this.userIdentifier;
			}
		}

		// Token: 0x0600BA24 RID: 47652 RVA: 0x00470274 File Offset: 0x0046E474
		public void Login(LoginUserCallback _delegate)
		{
			if (this.loginActualUserStatus == EUserStatus.LoggedIn)
			{
				Log.Out("[XBL] Already logged in.");
				this.UserStatus = EUserStatus.LoggedIn;
				Action<IPlatform> action = this.userLoggedIn;
				if (action != null)
				{
					action(this.owner);
				}
				if (_delegate != null)
				{
					_delegate(this.owner, EApiStatusReason.Ok, null);
				}
				return;
			}
			Log.Out("[XBL] Login");
			this.loginUserCallback = _delegate;
			SDK.XUserAddAsync(XUserAddOptions.AddDefaultUserAllowingUI, new XUserAddCompleted(this.AddUserComplete));
		}

		// Token: 0x0600BA25 RID: 47653 RVA: 0x004702E8 File Offset: 0x0046E4E8
		public void PlayOffline(LoginUserCallback _delegate)
		{
			if (this.UserStatus != EUserStatus.LoggedIn)
			{
				throw new Exception("Can not explicitly set XBL to offline mode");
			}
			this.UserStatus = EUserStatus.OfflineMode;
			Action<IPlatform> action = this.userLoggedIn;
			if (action != null)
			{
				action(this.owner);
			}
			_delegate(this.owner, EApiStatusReason.Ok, null);
		}

		// Token: 0x0600BA26 RID: 47654 RVA: 0x00470338 File Offset: 0x0046E538
		[PublicizedFrom(EAccessModifier.Private)]
		public void updateAdvertisment(GameServerInfo _serverInfo)
		{
			string value = _serverInfo.GetValue(GameInfoString.UniqueId);
			uint value2 = (uint)_serverInfo.GetValue(GameInfoInt.CurrentPlayers);
			int value3 = _serverInfo.GetValue(GameInfoInt.ServerVisibility);
			uint value4 = (uint)_serverInfo.GetValue(GameInfoInt.MaxPlayers);
			if (value4 < 2U)
			{
				return;
			}
			if (string.IsNullOrEmpty(value))
			{
				return;
			}
			XblContextHandle xblContextHandle = this.XblContextHandle;
			XblMultiplayerActivityInfo xblMultiplayerActivityInfo = new XblMultiplayerActivityInfo();
			xblMultiplayerActivityInfo.ConnectionString = value;
			xblMultiplayerActivityInfo.CurrentPlayers = value2;
			xblMultiplayerActivityInfo.GroupId = "Dummy";
			XblMultiplayerActivityInfo xblMultiplayerActivityInfo2 = xblMultiplayerActivityInfo;
			XblMultiplayerActivityJoinRestriction joinRestriction;
			if (value3 != 1)
			{
				if (value3 == 2)
				{
					joinRestriction = XblMultiplayerActivityJoinRestriction.Public;
				}
				else
				{
					joinRestriction = XblMultiplayerActivityJoinRestriction.InviteOnly;
				}
			}
			else
			{
				joinRestriction = XblMultiplayerActivityJoinRestriction.Followed;
			}
			xblMultiplayerActivityInfo2.JoinRestriction = joinRestriction;
			xblMultiplayerActivityInfo.MaxPlayers = value4;
			xblMultiplayerActivityInfo.Platform = XblMultiplayerActivityPlatform.All;
			xblMultiplayerActivityInfo.Xuid = this.userXuid;
			SDK.XBL.XblMultiplayerActivitySetActivityAsync(xblContextHandle, xblMultiplayerActivityInfo, true, delegate(int _hresult)
			{
				XblHelpers.Succeeded(_hresult, "Set Activity", true, false);
			});
		}

		// Token: 0x0600BA27 RID: 47655 RVA: 0x0047040C File Offset: 0x0046E60C
		public void StartAdvertisePlaying(GameServerInfo _serverInfo)
		{
			_serverInfo.OnChangedString += delegate(GameServerInfo _info, GameInfoString _key)
			{
				if (_key == GameInfoString.UniqueId)
				{
					this.updateAdvertisment(_serverInfo);
				}
			};
			_serverInfo.OnChangedInt += delegate(GameServerInfo _info, GameInfoInt _key)
			{
				if (_key == GameInfoInt.CurrentPlayers || _key == GameInfoInt.ServerVisibility || _key == GameInfoInt.MaxPlayers)
				{
					this.updateAdvertisment(_serverInfo);
				}
			};
		}

		// Token: 0x0600BA28 RID: 47656 RVA: 0x0047045B File Offset: 0x0046E65B
		public void StopAdvertisePlaying()
		{
			SDK.XBL.XblMultiplayerActivityDeleteActivityAsync(this.XblContextHandle, delegate(int _hresult)
			{
				if (XblHelpers.Succeeded(_hresult, "Delete Activity", true, false))
				{
					Log.Out("[XBL] Activity cleared");
				}
			});
		}

		// Token: 0x0600BA29 RID: 47657 RVA: 0x00470488 File Offset: 0x0046E688
		public void GetLoginTicket(Action<bool, byte[], string> _callback)
		{
			User.<>c__DisplayClass59_0 CS$<>8__locals1 = new User.<>c__DisplayClass59_0();
			CS$<>8__locals1._callback = _callback;
			SDK.XUserGetTokenAndSignatureUtf16Async(this.GdkUserHandle, XUserGetTokenAndSignatureOptions.None, "GET", "https://eos.epicgames.com/", User.eosRelyingPartyRequestHeaders, null, new XUserGetTokenAndSignatureUtf16Result(CS$<>8__locals1.<GetLoginTicket>g__CompletionRoutine|0));
		}

		// Token: 0x0600BA2A RID: 47658 RVA: 0x000424BD File Offset: 0x000406BD
		public string GetFriendName(PlatformUserIdentifierAbs _playerId)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BA2B RID: 47659 RVA: 0x004704CC File Offset: 0x0046E6CC
		public bool IsFriend(PlatformUserIdentifierAbs _playerId)
		{
			ulong xuid = XblXuidMapper.GetXuid(_playerId);
			if (xuid == 0UL)
			{
				return false;
			}
			FriendsListXbl friendsList = this.FriendsList;
			return friendsList != null && friendsList.IsFriend(xuid);
		}

		// Token: 0x0600BA2C RID: 47660 RVA: 0x004704F7 File Offset: 0x0046E6F7
		public bool CanShowProfile(PlatformUserIdentifierAbs _playerId)
		{
			return XblXuidMapper.GetXuid(_playerId) > 0UL;
		}

		// Token: 0x0600BA2D RID: 47661 RVA: 0x00470504 File Offset: 0x0046E704
		public void ShowProfile(PlatformUserIdentifierAbs _playerId)
		{
			ulong xuid = XblXuidMapper.GetXuid(_playerId);
			if (xuid != 0UL)
			{
				SDK.XGameUiShowPlayerProfileCardAsync(this.GdkUserHandle, xuid, delegate(int hr)
				{
					if (!XblHelpers.Succeeded(hr, "XGameUiShowPlayerProfileCardAsync", true, false))
					{
						Log.Error("[XBL] Showing Player Profile Failed.");
						return;
					}
					Log.Out("[XBL] Showing Player Profile Succeeded.");
				});
			}
		}

		// Token: 0x17001532 RID: 5426
		// (get) Token: 0x0600BA2E RID: 47662 RVA: 0x00470548 File Offset: 0x0046E748
		public EUserPerms Permissions
		{
			get
			{
				if (this.privilegeHelper == null)
				{
					return (EUserPerms)0;
				}
				EUserPerms euserPerms = (EUserPerms)0;
				if (this.privilegeHelper.MultiplayerAllowed.Has())
				{
					euserPerms |= (EUserPerms.Multiplayer | EUserPerms.HostMultiplayer);
				}
				if (this.privilegeHelper.CommunicationAllowed.Has())
				{
					euserPerms |= EUserPerms.Communication;
				}
				if (this.privilegeHelper.CrossPlayAllowed.Has())
				{
					euserPerms |= EUserPerms.Crossplay;
				}
				return euserPerms;
			}
		}

		// Token: 0x0600BA2F RID: 47663 RVA: 0x00019766 File Offset: 0x00017966
		public string GetPermissionDenyReason(EUserPerms _perms)
		{
			return null;
		}

		// Token: 0x0600BA30 RID: 47664 RVA: 0x004705A5 File Offset: 0x0046E7A5
		public IEnumerator ResolvePermissions(EUserPerms _perms, bool _canPrompt, CoroutineCancellationToken _cancellationToken = null)
		{
			Log.Out(string.Format("[XBL] {0}({1}: [{2}], {3}: {4})", new object[]
			{
				"ResolvePermissions",
				"_perms",
				_perms,
				"_canPrompt",
				_canPrompt
			}));
			if (this.privilegeHelper == null)
			{
				yield break;
			}
			yield return this.privilegeHelper.ResolvePermissions(_perms, _canPrompt, _cancellationToken);
			yield break;
		}

		// Token: 0x0600BA31 RID: 47665 RVA: 0x004705C9 File Offset: 0x0046E7C9
		public void UserAdded(PlatformUserIdentifierAbs _userId, bool _isPrimary)
		{
			if (!_isPrimary)
			{
				XblXuidMapper.GetXuid(_userId);
			}
		}

		// Token: 0x0600BA32 RID: 47666 RVA: 0x004705D5 File Offset: 0x0046E7D5
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnXuidMapped(IReadOnlyCollection<PlatformUserIdentifierAbs> userIds, ulong xuid)
		{
			UserBlocksChangedCallback userBlocksChanged = this.UserBlocksChanged;
			if (userBlocksChanged == null)
			{
				return;
			}
			userBlocksChanged(userIds);
		}

		// Token: 0x0600BA33 RID: 47667 RVA: 0x004705E8 File Offset: 0x0046E7E8
		public IEnumerator ResolveUserBlocks(IReadOnlyList<IPlatformUserBlockedResults> _results)
		{
			User.<>c__DisplayClass70_0 CS$<>8__locals1 = new User.<>c__DisplayClass70_0();
			CS$<>8__locals1._results = _results;
			CS$<>8__locals1.<>4__this = this;
			this.userBlockedXuidToResultsTemp.Clear();
			this.userBlockedAnonymousResultsTemp.Clear();
			PlatformUserIdentifierAbs platformUserId = this.PlatformUserId;
			foreach (IPlatformUserBlockedResults platformUserBlockedResults in CS$<>8__locals1._results)
			{
				PlatformUserIdentifierAbs nativeId = platformUserBlockedResults.User.NativeId;
				if (!object.Equals(platformUserId, nativeId))
				{
					UserIdentifierXbl userIdentifierXbl = nativeId as UserIdentifierXbl;
					ulong xuid;
					if (userIdentifierXbl == null || (xuid = userIdentifierXbl.Xuid) == 0UL)
					{
						this.userBlockedAnonymousResultsTemp.Add(platformUserBlockedResults);
					}
					else
					{
						this.userBlockedXuidToResultsTemp[xuid] = platformUserBlockedResults;
					}
				}
			}
			CS$<>8__locals1.running = true;
			SDK.XBL.XblPrivacyBatchCheckPermissionAsync(this.XblContextHandle, User.userBlockedPermissions, this.userBlockedXuidToResultsTemp.Keys.ToArray<ulong>(), (this.userBlockedAnonymousResultsTemp.Count > 0) ? User.userBlockedAnonymousTypes : Array.Empty<XblAnonymousUserType>(), new XblPrivacyBatchCheckPermissionCompleted(CS$<>8__locals1.<ResolveUserBlocks>g__CompletionRoutine|0));
			while (CS$<>8__locals1.running)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x0600BA34 RID: 47668 RVA: 0x00470600 File Offset: 0x0046E800
		public EMatchmakingGroup GetMatchmakingGroup()
		{
			string sandboxId = this.SandboxHelper.SandboxId;
			if (sandboxId == null)
			{
				EMatchmakingGroup ematchmakingGroup = EMatchmakingGroup.Retail;
				Log.Warning(string.Format("[XBL] {0} no sandbox id. Defaulting to {1}", "GetMatchmakingGroup", ematchmakingGroup));
				return ematchmakingGroup;
			}
			return XblSandboxHelper.SandboxIdToMatchmakingGroup(sandboxId);
		}

		// Token: 0x0600BA35 RID: 47669 RVA: 0x00470640 File Offset: 0x0046E840
		public void Destroy()
		{
			SDK.XBL.XblMultiplayerActivityDeleteActivityAsync(this.XblContextHandle, delegate(int _hresult)
			{
				if (XblHelpers.Succeeded(_hresult, "Delete Activity", true, false))
				{
					Log.Out("[XBL] Activity deleted");
				}
			});
		}

		// Token: 0x0600BA36 RID: 47670 RVA: 0x0047066C File Offset: 0x0046E86C
		[PublicizedFrom(EAccessModifier.Private)]
		public void apiInitialized()
		{
			this.SandboxHelper.RefreshSandboxId();
		}

		// Token: 0x0600BA37 RID: 47671 RVA: 0x0047067C File Offset: 0x0046E87C
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddUserComplete(int _hresult, XUserHandle _userHandle)
		{
			if (!XblHelpers.Succeeded(_hresult, "Sign in", true, false))
			{
				this.DoLoginUserCallback(EUserStatus.TemporaryError, EApiStatusReason.Unknown, string.Format("Error code: 0x{0:X8}", _hresult));
				return;
			}
			this.GdkUserHandle = _userHandle;
			this.privilegeHelper = new UserPrivilegeHelper(this.GdkUserHandle);
			this.privilegeHelper.AllAllowed.ResolveSilent();
			this.SocialManager = new SocialManagerXbl(this.GdkUserHandle);
			this.FriendsList = new FriendsListXbl(this.SocialManager);
			string text;
			if (!XblHelpers.Succeeded(SDK.XUserGetGamertag(this.GdkUserHandle, XUserGamertagComponent.Classic, out text), "Get gamertag", true, false))
			{
				this.DoLoginUserCallback(EUserStatus.TemporaryError, EApiStatusReason.NoFriendsName, string.Format("Error code: 0x{0:X8}", _hresult));
				return;
			}
			ulong num;
			if (!XblHelpers.Succeeded(SDK.XUserGetId(this.GdkUserHandle, out num), "Get user id", true, false))
			{
				this.DoLoginUserCallback(EUserStatus.TemporaryError, EApiStatusReason.Unknown, string.Format("Error code: 0x{0:X8}", _hresult));
				return;
			}
			Log.Out(string.Format("[XBL] Signed in, id: {0} gamertag: {1}", num, text));
			XblContextHandle xblContextHandle;
			if (!XblHelpers.Succeeded(SDK.XBL.XblContextCreateHandle(this.GdkUserHandle, out xblContextHandle), "Create Xbox Live context", true, false))
			{
				this.DoLoginUserCallback(EUserStatus.TemporaryError, EApiStatusReason.Unknown, string.Format("Error code: 0x{0:X8}", _hresult));
				return;
			}
			this.XblContextHandle = xblContextHandle;
			this.MultiplayerActivityQueryManager = new MultiplayerActivityQueryManager(this.XblContextHandle);
			GamePrefs.Set(EnumGamePrefs.PlayerName, text);
			this.userXuid = num;
			Dictionary<ulong, UserIdentifierXbl> dictionary = this.loadUserMappings();
			UserIdentifierXbl userId;
			if (dictionary != null && dictionary.TryGetValue(num, out userId))
			{
				this.userIdentifier = userId;
				XblXuidMapper.SetXuid(userId, this.userXuid);
			}
			this.DoLoginUserCallback(EUserStatus.LoggedIn, EApiStatusReason.Ok, null);
		}

		// Token: 0x0600BA38 RID: 47672 RVA: 0x0047080C File Offset: 0x0046EA0C
		[PublicizedFrom(EAccessModifier.Private)]
		public void DoLoginUserCallback(EUserStatus userStatus, EApiStatusReason reason, string reasonAdditional)
		{
			this.loginActualUserStatus = userStatus;
			this.UserStatus = userStatus;
			if (userStatus == EUserStatus.LoggedIn)
			{
				Action<IPlatform> action = this.userLoggedIn;
				if (action != null)
				{
					action(this.owner);
				}
			}
			LoginUserCallback loginUserCallback = this.loginUserCallback;
			if (loginUserCallback == null)
			{
				return;
			}
			loginUserCallback(this.owner, reason, reasonAdditional);
		}

		// Token: 0x0600BA39 RID: 47673 RVA: 0x0047085C File Offset: 0x0046EA5C
		[PublicizedFrom(EAccessModifier.Private)]
		public void CrossLoginDone(IPlatform _sender)
		{
			if (this.userIdentifier != null)
			{
				return;
			}
			PlatformUserIdentifierAbs nativePlatformUserId = ((User)_sender.User).NativePlatformUserId;
			if (nativePlatformUserId.PlatformIdentifier != EPlatformIdentifier.XBL)
			{
				Log.Error("[XBL] EOS detected different native platform: " + nativePlatformUserId.PlatformIdentifierString);
				return;
			}
			this.userIdentifier = (UserIdentifierXbl)nativePlatformUserId;
			XblXuidMapper.SetXuid(this.userIdentifier, this.userXuid);
			this.saveUserMapping();
		}

		// Token: 0x0600BA3A RID: 47674 RVA: 0x004708C8 File Offset: 0x0046EAC8
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<ulong, UserIdentifierXbl> loadUserMappings()
		{
			if (!SdPlayerPrefs.HasKey("XblMappings"))
			{
				Log.Warning("[XBL] No XUID -> PXUID mappings found");
				return null;
			}
			Dictionary<ulong, UserIdentifierXbl> dictionary = new Dictionary<ulong, UserIdentifierXbl>();
			string[] array = SdPlayerPrefs.GetString("XblMappings").Split(';', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length != 0)
				{
					string[] array2 = array[i].Split('=', StringSplitOptions.None);
					ulong key;
					if (array2.Length != 2)
					{
						Log.Warning("[XBL] Malformed user mapping entry: '" + array[i] + "'");
					}
					else if (!ulong.TryParse(array2[0], out key))
					{
						Log.Warning("[XBL] Malformed user identifier entry: '" + array2[0] + "'");
					}
					else
					{
						PlatformUserIdentifierAbs platformUserIdentifierAbs = PlatformUserIdentifierAbs.FromCombinedString(array2[1], true);
						if (platformUserIdentifierAbs == null)
						{
							Log.Warning("[XBL] Malformed user identifier XBL mapping entry: '" + array2[1] + "'");
						}
						else if (platformUserIdentifierAbs.PlatformIdentifier != EPlatformIdentifier.XBL)
						{
							Log.Warning("[XBL] Stored user identifier XBL mapping not an XBL identifier: '" + array2[1] + "'");
						}
						else
						{
							dictionary.Add(key, (UserIdentifierXbl)platformUserIdentifierAbs);
						}
					}
				}
			}
			return dictionary;
		}

		// Token: 0x0600BA3B RID: 47675 RVA: 0x004709D4 File Offset: 0x0046EBD4
		[PublicizedFrom(EAccessModifier.Private)]
		public void saveUserMapping()
		{
			Dictionary<ulong, UserIdentifierXbl> dictionary = this.loadUserMappings() ?? new Dictionary<ulong, UserIdentifierXbl>();
			dictionary[this.userXuid] = this.userIdentifier;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<ulong, UserIdentifierXbl> keyValuePair in dictionary)
			{
				stringBuilder.Append(keyValuePair.Key.ToString() + "=" + keyValuePair.Value.CombinedString + ";");
			}
			SdPlayerPrefs.SetString("XblMappings", stringBuilder.ToString());
			SdPlayerPrefs.Save();
		}

		// Token: 0x040091D8 RID: 37336
		[PublicizedFrom(EAccessModifier.Private)]
		public const string xblMappingsPrefName = "XblMappings";

		// Token: 0x040091D9 RID: 37337
		[PublicizedFrom(EAccessModifier.Private)]
		public const string eosRelyingPartyUrl = "https://eos.epicgames.com/";

		// Token: 0x040091DA RID: 37338
		[PublicizedFrom(EAccessModifier.Private)]
		public const string eosRelyingPartyHttpMethod = "GET";

		// Token: 0x040091DB RID: 37339
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly XUserGetTokenAndSignatureUtf16HttpHeader[] eosRelyingPartyRequestHeaders = new XUserGetTokenAndSignatureUtf16HttpHeader[]
		{
			new XUserGetTokenAndSignatureUtf16HttpHeader
			{
				Name = "X-XBL-Contract-Version",
				Value = "2"
			}
		};

		// Token: 0x040091DC RID: 37340
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly EnumDictionary<XblPermission, EBlockType> xblPermissionToBlockType;

		// Token: 0x040091DD RID: 37341
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly XblPermission[] userBlockedPermissions;

		// Token: 0x040091DE RID: 37342
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly XblAnonymousUserType[] userBlockedAnonymousTypes;

		// Token: 0x040091E3 RID: 37347
		[PublicizedFrom(EAccessModifier.Private)]
		public UserPrivilegeHelper privilegeHelper;

		// Token: 0x040091E4 RID: 37348
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x040091E5 RID: 37349
		[PublicizedFrom(EAccessModifier.Private)]
		public EUserStatus loginActualUserStatus = EUserStatus.NotAttempted;

		// Token: 0x040091E6 RID: 37350
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<ulong, IPlatformUserBlockedResults> userBlockedXuidToResultsTemp = new Dictionary<ulong, IPlatformUserBlockedResults>();

		// Token: 0x040091E7 RID: 37351
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<IPlatformUserBlockedResults> userBlockedAnonymousResultsTemp = new List<IPlatformUserBlockedResults>();

		// Token: 0x040091E9 RID: 37353
		[PublicizedFrom(EAccessModifier.Private)]
		public Action<IPlatform> userLoggedIn;

		// Token: 0x040091ED RID: 37357
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong userXuid;

		// Token: 0x040091EE RID: 37358
		[PublicizedFrom(EAccessModifier.Private)]
		public UserIdentifierXbl userIdentifier;

		// Token: 0x040091EF RID: 37359
		[PublicizedFrom(EAccessModifier.Private)]
		public LoginUserCallback loginUserCallback;
	}
}
