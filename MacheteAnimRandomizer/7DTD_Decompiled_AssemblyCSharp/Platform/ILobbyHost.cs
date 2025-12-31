using System;

namespace Platform
{
	// Token: 0x020017EC RID: 6124
	public interface ILobbyHost
	{
		// Token: 0x17001495 RID: 5269
		// (get) Token: 0x0600B6B4 RID: 46772
		string LobbyId { get; }

		// Token: 0x17001496 RID: 5270
		// (get) Token: 0x0600B6B5 RID: 46773
		bool IsInLobby { get; }

		// Token: 0x17001497 RID: 5271
		// (get) Token: 0x0600B6B6 RID: 46774 RVA: 0x0000FB42 File Offset: 0x0000DD42
		bool AllowClientLobby
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600B6B7 RID: 46775
		void Init(IPlatform _owner);

		// Token: 0x0600B6B8 RID: 46776
		void UpdateLobby(GameServerInfo _gameServerInfo);

		// Token: 0x0600B6B9 RID: 46777
		void JoinLobby(string _lobbyId, Action<LobbyHostJoinResult> _onComplete);

		// Token: 0x0600B6BA RID: 46778
		void ExitLobby();

		// Token: 0x0600B6BB RID: 46779
		void UpdateGameTimePlayers(ulong _time, int _players);

		// Token: 0x0600B6BC RID: 46780 RVA: 0x0046788C File Offset: 0x00465A8C
		[PublicizedFrom(EAccessModifier.Protected)]
		public static void NotifyJoinedSession(string sessionId, bool overwriteHostLobby)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				PlatformLobbyId lobbyId = new PlatformLobbyId(PlatformManager.NativePlatform.PlatformIdentifier, sessionId);
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageLobbyRegisterClient>().Setup(lobbyId, overwriteHostLobby), false);
			}
		}
	}
}
