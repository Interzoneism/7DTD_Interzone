using System;
using System.Collections;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x02001827 RID: 6183
	public interface IUserClient
	{
		// Token: 0x170014D6 RID: 5334
		// (get) Token: 0x0600B7D2 RID: 47058
		EUserStatus UserStatus { get; }

		// Token: 0x14000114 RID: 276
		// (add) Token: 0x0600B7D3 RID: 47059
		// (remove) Token: 0x0600B7D4 RID: 47060
		event Action<IPlatform> UserLoggedIn;

		// Token: 0x14000115 RID: 277
		// (add) Token: 0x0600B7D5 RID: 47061
		// (remove) Token: 0x0600B7D6 RID: 47062
		event UserBlocksChangedCallback UserBlocksChanged;

		// Token: 0x170014D7 RID: 5335
		// (get) Token: 0x0600B7D7 RID: 47063
		PlatformUserIdentifierAbs PlatformUserId { get; }

		// Token: 0x0600B7D8 RID: 47064
		void Init(IPlatform _owner);

		// Token: 0x0600B7D9 RID: 47065
		void Login(LoginUserCallback _delegate);

		// Token: 0x0600B7DA RID: 47066
		void PlayOffline(LoginUserCallback _delegate);

		// Token: 0x0600B7DB RID: 47067 RVA: 0x0000FB42 File Offset: 0x0000DD42
		EMatchmakingGroup GetMatchmakingGroup()
		{
			return EMatchmakingGroup.Dev;
		}

		// Token: 0x0600B7DC RID: 47068
		void StartAdvertisePlaying(GameServerInfo _serverInfo);

		// Token: 0x0600B7DD RID: 47069
		void StopAdvertisePlaying();

		// Token: 0x0600B7DE RID: 47070
		void GetLoginTicket(Action<bool, byte[], string> _callback);

		// Token: 0x0600B7DF RID: 47071
		string GetFriendName(PlatformUserIdentifierAbs _playerId);

		// Token: 0x0600B7E0 RID: 47072
		bool IsFriend(PlatformUserIdentifierAbs _playerId);

		// Token: 0x0600B7E1 RID: 47073 RVA: 0x0000FB42 File Offset: 0x0000DD42
		bool CanShowProfile(PlatformUserIdentifierAbs _playerId)
		{
			return false;
		}

		// Token: 0x0600B7E2 RID: 47074 RVA: 0x00002914 File Offset: 0x00000B14
		void ShowProfile(PlatformUserIdentifierAbs _playerId)
		{
		}

		// Token: 0x170014D8 RID: 5336
		// (get) Token: 0x0600B7E3 RID: 47075
		EUserPerms Permissions { get; }

		// Token: 0x0600B7E4 RID: 47076
		string GetPermissionDenyReason(EUserPerms _perms);

		// Token: 0x0600B7E5 RID: 47077
		IEnumerator ResolvePermissions(EUserPerms _perms, bool _canPrompt, CoroutineCancellationToken cancellationToken = null);

		// Token: 0x0600B7E6 RID: 47078
		IEnumerator ResolveUserBlocks(IReadOnlyList<IPlatformUserBlockedResults> _results);

		// Token: 0x0600B7E7 RID: 47079
		void Destroy();
	}
}
