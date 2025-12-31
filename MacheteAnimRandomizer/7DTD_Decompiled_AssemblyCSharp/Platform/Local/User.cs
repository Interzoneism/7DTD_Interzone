using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Platform.Local
{
	// Token: 0x020018EF RID: 6383
	public class User : IUserClient
	{
		// Token: 0x170015A5 RID: 5541
		// (get) Token: 0x0600BC9D RID: 48285 RVA: 0x00478628 File Offset: 0x00476828
		// (set) Token: 0x0600BC9E RID: 48286 RVA: 0x00478630 File Offset: 0x00476830
		public EUserStatus UserStatus { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x14000126 RID: 294
		// (add) Token: 0x0600BC9F RID: 48287 RVA: 0x0047863C File Offset: 0x0047683C
		// (remove) Token: 0x0600BCA0 RID: 48288 RVA: 0x00002914 File Offset: 0x00000B14
		public event Action<IPlatform> UserLoggedIn
		{
			add
			{
				lock (this)
				{
					value(this.owner);
				}
			}
			remove
			{
			}
		}

		// Token: 0x14000127 RID: 295
		// (add) Token: 0x0600BCA1 RID: 48289 RVA: 0x00002914 File Offset: 0x00000B14
		// (remove) Token: 0x0600BCA2 RID: 48290 RVA: 0x00002914 File Offset: 0x00000B14
		public event UserBlocksChangedCallback UserBlocksChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		// Token: 0x0600BCA3 RID: 48291 RVA: 0x00478680 File Offset: 0x00476880
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
			GamePrefs.OnGamePrefChanged += delegate(EnumGamePrefs _pref)
			{
				if (_pref == EnumGamePrefs.PlayerName)
				{
					this.platformUserId = new UserIdentifierLocal(GamePrefs.GetString(EnumGamePrefs.PlayerName));
				}
			};
		}

		// Token: 0x170015A6 RID: 5542
		// (get) Token: 0x0600BCA4 RID: 48292 RVA: 0x0047869A File Offset: 0x0047689A
		public PlatformUserIdentifierAbs PlatformUserId
		{
			get
			{
				return this.platformUserId;
			}
		}

		// Token: 0x0600BCA5 RID: 48293 RVA: 0x004786A2 File Offset: 0x004768A2
		public void Login(LoginUserCallback _delegate)
		{
			this.platformUserId = new UserIdentifierLocal(GamePrefs.GetString(EnumGamePrefs.PlayerName));
			_delegate(this.owner, EApiStatusReason.Ok, null);
		}

		// Token: 0x0600BCA6 RID: 48294 RVA: 0x004786C5 File Offset: 0x004768C5
		public void PlayOffline(LoginUserCallback _delegate)
		{
			this.UserStatus = EUserStatus.OfflineMode;
			_delegate(this.owner, EApiStatusReason.Ok, null);
		}

		// Token: 0x0600BCA7 RID: 48295 RVA: 0x00002914 File Offset: 0x00000B14
		public void StartAdvertisePlaying(GameServerInfo _serverInfo)
		{
		}

		// Token: 0x0600BCA8 RID: 48296 RVA: 0x00002914 File Offset: 0x00000B14
		public void StopAdvertisePlaying()
		{
		}

		// Token: 0x0600BCA9 RID: 48297 RVA: 0x000424BD File Offset: 0x000406BD
		public void GetLoginTicket(Action<bool, byte[], string> _callback)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BCAA RID: 48298 RVA: 0x00019766 File Offset: 0x00017966
		public string GetFriendName(PlatformUserIdentifierAbs _playerId)
		{
			return null;
		}

		// Token: 0x0600BCAB RID: 48299 RVA: 0x000197A5 File Offset: 0x000179A5
		public bool IsFriend(PlatformUserIdentifierAbs _playerId)
		{
			return true;
		}

		// Token: 0x170015A7 RID: 5543
		// (get) Token: 0x0600BCAC RID: 48300 RVA: 0x00198AC2 File Offset: 0x00196CC2
		public EUserPerms Permissions
		{
			get
			{
				return EUserPerms.All;
			}
		}

		// Token: 0x0600BCAD RID: 48301 RVA: 0x00019766 File Offset: 0x00017966
		public string GetPermissionDenyReason(EUserPerms _perms)
		{
			return null;
		}

		// Token: 0x0600BCAE RID: 48302 RVA: 0x004762E4 File Offset: 0x004744E4
		public IEnumerator ResolvePermissions(EUserPerms _perms, bool _canPrompt, CoroutineCancellationToken _cancellationToken = null)
		{
			return Enumerable.Empty<object>().GetEnumerator();
		}

		// Token: 0x0600BCAF RID: 48303 RVA: 0x00002914 File Offset: 0x00000B14
		public void UserAdded(PlatformUserIdentifierAbs _userId, bool _isPrimary)
		{
		}

		// Token: 0x0600BCB0 RID: 48304 RVA: 0x004762E4 File Offset: 0x004744E4
		public IEnumerator ResolveUserBlocks(IReadOnlyList<IPlatformUserBlockedResults> _results)
		{
			return Enumerable.Empty<object>().GetEnumerator();
		}

		// Token: 0x0600BCB1 RID: 48305 RVA: 0x00002914 File Offset: 0x00000B14
		public void Destroy()
		{
		}

		// Token: 0x040092FF RID: 37631
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x04009300 RID: 37632
		[PublicizedFrom(EAccessModifier.Private)]
		public UserIdentifierLocal platformUserId;
	}
}
