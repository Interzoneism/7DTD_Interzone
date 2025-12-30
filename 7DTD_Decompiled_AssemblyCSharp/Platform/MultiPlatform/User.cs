using System;
using System.Collections;
using System.Collections.Generic;

namespace Platform.MultiPlatform
{
	// Token: 0x020018E5 RID: 6373
	public class User : IUserClient
	{
		// Token: 0x0600BC5B RID: 48219 RVA: 0x00002914 File Offset: 0x00000B14
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x1700159B RID: 5531
		// (get) Token: 0x0600BC5C RID: 48220 RVA: 0x00477BEC File Offset: 0x00475DEC
		public EUserStatus UserStatus
		{
			get
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				if (((crossplatformPlatform != null) ? crossplatformPlatform.User : null) == null)
				{
					return PlatformManager.NativePlatform.User.UserStatus;
				}
				return PlatformManager.CrossplatformPlatform.User.UserStatus;
			}
		}

		// Token: 0x14000123 RID: 291
		// (add) Token: 0x0600BC5D RID: 48221 RVA: 0x00477C20 File Offset: 0x00475E20
		// (remove) Token: 0x0600BC5E RID: 48222 RVA: 0x00477C88 File Offset: 0x00475E88
		public event Action<IPlatform> UserLoggedIn
		{
			add
			{
				lock (this)
				{
					PlatformManager.NativePlatform.User.UserLoggedIn += value;
					IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
					if (((crossplatformPlatform != null) ? crossplatformPlatform.User : null) != null)
					{
						PlatformManager.CrossplatformPlatform.User.UserLoggedIn += value;
					}
				}
			}
			remove
			{
				lock (this)
				{
					PlatformManager.NativePlatform.User.UserLoggedIn -= value;
					IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
					if (((crossplatformPlatform != null) ? crossplatformPlatform.User : null) != null)
					{
						PlatformManager.CrossplatformPlatform.User.UserLoggedIn -= value;
					}
				}
			}
		}

		// Token: 0x14000124 RID: 292
		// (add) Token: 0x0600BC5F RID: 48223 RVA: 0x00477CF0 File Offset: 0x00475EF0
		// (remove) Token: 0x0600BC60 RID: 48224 RVA: 0x00477D2C File Offset: 0x00475F2C
		public event UserBlocksChangedCallback UserBlocksChanged
		{
			add
			{
				PlatformManager.NativePlatform.User.UserBlocksChanged += value;
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				IUserClient userClient = (crossplatformPlatform != null) ? crossplatformPlatform.User : null;
				if (userClient != null)
				{
					userClient.UserBlocksChanged += value;
				}
			}
			remove
			{
				PlatformManager.NativePlatform.User.UserBlocksChanged -= value;
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				IUserClient userClient = (crossplatformPlatform != null) ? crossplatformPlatform.User : null;
				if (userClient != null)
				{
					userClient.UserBlocksChanged -= value;
				}
			}
		}

		// Token: 0x1700159C RID: 5532
		// (get) Token: 0x0600BC61 RID: 48225 RVA: 0x004697F3 File Offset: 0x004679F3
		public PlatformUserIdentifierAbs PlatformUserId
		{
			get
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				PlatformUserIdentifierAbs platformUserIdentifierAbs;
				if (crossplatformPlatform == null)
				{
					platformUserIdentifierAbs = null;
				}
				else
				{
					IUserClient user = crossplatformPlatform.User;
					platformUserIdentifierAbs = ((user != null) ? user.PlatformUserId : null);
				}
				return platformUserIdentifierAbs ?? PlatformManager.NativePlatform.User.PlatformUserId;
			}
		}

		// Token: 0x0600BC62 RID: 48226 RVA: 0x00477D68 File Offset: 0x00475F68
		public void Login(LoginUserCallback _delegate)
		{
			PlatformManager.NativePlatform.User.Login(delegate(IPlatform _nativePlatform, EApiStatusReason _nativeReason, string _statusReasonAdditionalText)
			{
				if (_nativePlatform.Api.ClientApiStatus != EApiStatus.Ok || _nativePlatform.User.UserStatus != EUserStatus.LoggedIn)
				{
					_delegate(_nativePlatform, _nativeReason, _statusReasonAdditionalText);
					return;
				}
				if (_nativeReason != EApiStatusReason.Ok)
				{
					_delegate(_nativePlatform, _nativeReason, _statusReasonAdditionalText);
					return;
				}
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				if (((crossplatformPlatform != null) ? crossplatformPlatform.User : null) == null)
				{
					_delegate(_nativePlatform, _nativeReason, _statusReasonAdditionalText);
					return;
				}
				PlatformManager.CrossplatformPlatform.User.Login(_delegate);
			});
		}

		// Token: 0x0600BC63 RID: 48227 RVA: 0x00477DA0 File Offset: 0x00475FA0
		public void PlayOffline(LoginUserCallback _delegate)
		{
			PlatformManager.NativePlatform.User.PlayOffline(delegate(IPlatform _nativePlatform, EApiStatusReason _nativeReason, string _statusReasonAdditionalText)
			{
				if (_nativePlatform.Api.ClientApiStatus != EApiStatus.Ok || _nativePlatform.User.UserStatus != EUserStatus.OfflineMode)
				{
					_delegate(_nativePlatform, _nativeReason, _statusReasonAdditionalText);
					return;
				}
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				if (((crossplatformPlatform != null) ? crossplatformPlatform.User : null) == null)
				{
					_delegate(_nativePlatform, _nativeReason, _statusReasonAdditionalText);
					return;
				}
				PlatformManager.CrossplatformPlatform.User.PlayOffline(_delegate);
			});
		}

		// Token: 0x0600BC64 RID: 48228 RVA: 0x00477DD5 File Offset: 0x00475FD5
		public void StartAdvertisePlaying(GameServerInfo _serverInfo)
		{
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			if (crossplatformPlatform != null)
			{
				IUserClient user = crossplatformPlatform.User;
				if (user != null)
				{
					user.StartAdvertisePlaying(_serverInfo);
				}
			}
			PlatformManager.NativePlatform.User.StartAdvertisePlaying(_serverInfo);
		}

		// Token: 0x0600BC65 RID: 48229 RVA: 0x00477E03 File Offset: 0x00476003
		public void StopAdvertisePlaying()
		{
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			if (crossplatformPlatform != null)
			{
				IUserClient user = crossplatformPlatform.User;
				if (user != null)
				{
					user.StopAdvertisePlaying();
				}
			}
			PlatformManager.NativePlatform.User.StopAdvertisePlaying();
		}

		// Token: 0x0600BC66 RID: 48230 RVA: 0x000424BD File Offset: 0x000406BD
		public void GetLoginTicket(Action<bool, byte[], string> _callback)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BC67 RID: 48231 RVA: 0x000424BD File Offset: 0x000406BD
		public string GetFriendName(PlatformUserIdentifierAbs _playerId)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BC68 RID: 48232 RVA: 0x00477E30 File Offset: 0x00476030
		public bool IsFriend(PlatformUserIdentifierAbs _playerId)
		{
			if (_playerId == null)
			{
				return false;
			}
			IPlatform platform = PlatformManager.InstanceForPlatformIdentifier(_playerId.PlatformIdentifier);
			if (platform == null)
			{
				return false;
			}
			IUserClient user = platform.User;
			return user != null && user.IsFriend(_playerId);
		}

		// Token: 0x0600BC69 RID: 48233 RVA: 0x00477E68 File Offset: 0x00476068
		public bool CanShowProfile(PlatformUserIdentifierAbs _playerId)
		{
			if (!PlatformManager.NativePlatform.User.CanShowProfile(_playerId))
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				bool? flag;
				if (crossplatformPlatform == null)
				{
					flag = null;
				}
				else
				{
					IUserClient user = crossplatformPlatform.User;
					flag = ((user != null) ? new bool?(user.CanShowProfile(_playerId)) : null);
				}
				bool? flag2 = flag;
				return flag2.GetValueOrDefault();
			}
			return true;
		}

		// Token: 0x0600BC6A RID: 48234 RVA: 0x00477EC4 File Offset: 0x004760C4
		public void ShowProfile(PlatformUserIdentifierAbs _playerId)
		{
			if (PlatformManager.NativePlatform.User.CanShowProfile(_playerId))
			{
				PlatformManager.NativePlatform.User.ShowProfile(_playerId);
				return;
			}
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			bool? flag;
			if (crossplatformPlatform == null)
			{
				flag = null;
			}
			else
			{
				IUserClient user = crossplatformPlatform.User;
				flag = ((user != null) ? new bool?(user.CanShowProfile(_playerId)) : null);
			}
			bool? flag2 = flag;
			if (flag2.GetValueOrDefault())
			{
				PlatformManager.CrossplatformPlatform.User.ShowProfile(_playerId);
			}
		}

		// Token: 0x1700159D RID: 5533
		// (get) Token: 0x0600BC6B RID: 48235 RVA: 0x00477F44 File Offset: 0x00476144
		public EUserPerms Permissions
		{
			get
			{
				if (GameManager.IsDedicatedServer)
				{
					return EUserPerms.All;
				}
				EUserPerms permissions = PlatformManager.NativePlatform.User.Permissions;
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				EUserPerms? euserPerms;
				if (crossplatformPlatform == null)
				{
					euserPerms = null;
				}
				else
				{
					IUserClient user = crossplatformPlatform.User;
					euserPerms = ((user != null) ? new EUserPerms?(user.Permissions) : null);
				}
				return permissions & (euserPerms ?? EUserPerms.All);
			}
		}

		// Token: 0x0600BC6C RID: 48236 RVA: 0x00477FB4 File Offset: 0x004761B4
		public string GetPermissionDenyReason(EUserPerms _perms)
		{
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			string text;
			if (crossplatformPlatform == null)
			{
				text = null;
			}
			else
			{
				IUserClient user = crossplatformPlatform.User;
				text = ((user != null) ? user.GetPermissionDenyReason(_perms) : null);
			}
			string text2 = text;
			if (!string.IsNullOrEmpty(text2))
			{
				return text2;
			}
			string permissionDenyReason = PlatformManager.NativePlatform.User.GetPermissionDenyReason(_perms);
			if (!string.IsNullOrEmpty(permissionDenyReason))
			{
				return permissionDenyReason;
			}
			return null;
		}

		// Token: 0x0600BC6D RID: 48237 RVA: 0x00478006 File Offset: 0x00476206
		public IEnumerator ResolvePermissions(EUserPerms _perms, bool _canPrompt, CoroutineCancellationToken _cancellationToken = null)
		{
			if (_canPrompt && this.UserStatus != EUserStatus.LoggedIn)
			{
				Log.Out("[MultiPlatform] ResolvePermissions: Attempting Login as we're allowed to prompt.");
				bool loginAttemptDone = false;
				this.Login(delegate(IPlatform platform, EApiStatusReason reason, string text)
				{
					CoroutineCancellationToken cancellationToken2 = _cancellationToken;
					if (cancellationToken2 != null && cancellationToken2.IsCancelled())
					{
						return;
					}
					loginAttemptDone = true;
					EUserStatus userStatus = this.UserStatus;
					((userStatus == EUserStatus.LoggedIn) ? new Action<string>(Log.Out) : new Action<string>(Log.Warning))(string.Format("[MultiPlatform] {0}: Login Attempt Completed. Status: {1}, Platform: {2}, Reason: {3}, Additional Reason: '{4}'.", new object[]
					{
						"ResolvePermissions",
						userStatus,
						platform,
						reason,
						text
					}));
				});
				while (!loginAttemptDone)
				{
					yield return null;
					CoroutineCancellationToken cancellationToken = _cancellationToken;
					if (cancellationToken != null && cancellationToken.IsCancelled())
					{
						yield break;
					}
				}
			}
			yield return PlatformManager.NativePlatform.User.ResolvePermissions(_perms, _canPrompt, _cancellationToken);
			_perms &= PlatformManager.NativePlatform.User.Permissions;
			if (_perms == (EUserPerms)0)
			{
				yield break;
			}
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			object obj;
			if (crossplatformPlatform == null)
			{
				obj = null;
			}
			else
			{
				IUserClient user = crossplatformPlatform.User;
				obj = ((user != null) ? user.ResolvePermissions(_perms, _canPrompt, _cancellationToken) : null);
			}
			yield return obj;
			yield break;
		}

		// Token: 0x0600BC6E RID: 48238 RVA: 0x0047802A File Offset: 0x0047622A
		public IEnumerator ResolveUserBlocks(IReadOnlyList<IPlatformUserBlockedResults> _results)
		{
			if (GameManager.IsDedicatedServer)
			{
				yield break;
			}
			if (!this.Permissions.HasCommunication())
			{
				PlatformUserIdentifierAbs platformUserId = this.PlatformUserId;
				foreach (IPlatformUserBlockedResults platformUserBlockedResults in _results)
				{
					if (!object.Equals(platformUserId, platformUserBlockedResults.User.PrimaryId))
					{
						platformUserBlockedResults.Block(EBlockType.TextChat);
						platformUserBlockedResults.Block(EBlockType.VoiceChat);
					}
				}
			}
			yield return PlatformManager.NativePlatform.User.ResolveUserBlocks(_results);
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			object obj;
			if (crossplatformPlatform == null)
			{
				obj = null;
			}
			else
			{
				IUserClient user = crossplatformPlatform.User;
				obj = ((user != null) ? user.ResolveUserBlocks(_results) : null);
			}
			yield return obj;
			yield break;
		}

		// Token: 0x0600BC6F RID: 48239 RVA: 0x00478040 File Offset: 0x00476240
		public EMatchmakingGroup GetMatchmakingGroup()
		{
			IUserClient user = PlatformManager.NativePlatform.User;
			if (user == null)
			{
				return EMatchmakingGroup.Dev;
			}
			return user.GetMatchmakingGroup();
		}

		// Token: 0x0600BC70 RID: 48240 RVA: 0x00002914 File Offset: 0x00000B14
		public void Destroy()
		{
		}
	}
}
