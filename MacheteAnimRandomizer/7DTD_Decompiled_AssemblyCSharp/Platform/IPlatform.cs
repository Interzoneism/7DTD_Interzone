using System;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x020017F6 RID: 6134
	public interface IPlatform
	{
		// Token: 0x170014A3 RID: 5283
		// (get) Token: 0x0600B6EA RID: 46826
		// (set) Token: 0x0600B6EB RID: 46827
		bool AsServerOnly { get; set; }

		// Token: 0x170014A4 RID: 5284
		// (get) Token: 0x0600B6EC RID: 46828
		// (set) Token: 0x0600B6ED RID: 46829
		bool IsCrossplatform { get; set; }

		// Token: 0x170014A5 RID: 5285
		// (get) Token: 0x0600B6EE RID: 46830
		EPlatformIdentifier PlatformIdentifier { get; }

		// Token: 0x170014A6 RID: 5286
		// (get) Token: 0x0600B6EF RID: 46831
		string PlatformDisplayName { get; }

		// Token: 0x0600B6F0 RID: 46832
		void CreateInstances();

		// Token: 0x0600B6F1 RID: 46833
		void Init();

		// Token: 0x0600B6F2 RID: 46834
		bool HasNetworkingEnabled(IList<string> _disabledProtocolNames);

		// Token: 0x0600B6F3 RID: 46835
		INetworkServer GetNetworkingServer(ProtocolManager _protocolManager);

		// Token: 0x0600B6F4 RID: 46836
		INetworkClient GetNetworkingClient(ProtocolManager _protocolManager);

		// Token: 0x0600B6F5 RID: 46837
		void UserAdded(PlatformUserIdentifierAbs _id, bool _isPrimary);

		// Token: 0x0600B6F6 RID: 46838
		string[] GetArgumentsForRelaunch();

		// Token: 0x0600B6F7 RID: 46839
		void Update();

		// Token: 0x0600B6F8 RID: 46840
		void LateUpdate();

		// Token: 0x0600B6F9 RID: 46841
		void Destroy();

		// Token: 0x170014A7 RID: 5287
		// (get) Token: 0x0600B6FA RID: 46842
		IPlatformApi Api { get; }

		// Token: 0x170014A8 RID: 5288
		// (get) Token: 0x0600B6FB RID: 46843
		IUserClient User { get; }

		// Token: 0x170014A9 RID: 5289
		// (get) Token: 0x0600B6FC RID: 46844
		IAuthenticationClient AuthenticationClient { get; }

		// Token: 0x170014AA RID: 5290
		// (get) Token: 0x0600B6FD RID: 46845
		IAuthenticationServer AuthenticationServer { get; }

		// Token: 0x170014AB RID: 5291
		// (get) Token: 0x0600B6FE RID: 46846
		IList<IServerListInterface> ServerListInterfaces { get; }

		// Token: 0x170014AC RID: 5292
		// (get) Token: 0x0600B6FF RID: 46847
		IServerListInterface ServerLookupInterface { get; }

		// Token: 0x170014AD RID: 5293
		// (get) Token: 0x0600B700 RID: 46848
		IMasterServerAnnouncer ServerListAnnouncer { get; }

		// Token: 0x170014AE RID: 5294
		// (get) Token: 0x0600B701 RID: 46849
		IList<IJoinSessionGameInviteListener> InviteListeners { get; }

		// Token: 0x170014AF RID: 5295
		// (get) Token: 0x0600B702 RID: 46850
		IMultiplayerInvitationDialog MultiplayerInvitationDialog { get; }

		// Token: 0x170014B0 RID: 5296
		// (get) Token: 0x0600B703 RID: 46851
		ILobbyHost LobbyHost { get; }

		// Token: 0x170014B1 RID: 5297
		// (get) Token: 0x0600B704 RID: 46852
		IPlayerInteractionsRecorder PlayerInteractionsRecorder { get; }

		// Token: 0x170014B2 RID: 5298
		// (get) Token: 0x0600B705 RID: 46853
		IGameplayNotifier GameplayNotifier { get; }

		// Token: 0x170014B3 RID: 5299
		// (get) Token: 0x0600B706 RID: 46854
		IPartyVoice PartyVoice { get; }

		// Token: 0x170014B4 RID: 5300
		// (get) Token: 0x0600B707 RID: 46855
		IUtils Utils { get; }

		// Token: 0x170014B5 RID: 5301
		// (get) Token: 0x0600B708 RID: 46856
		IPlatformMemory Memory { get; }

		// Token: 0x170014B6 RID: 5302
		// (get) Token: 0x0600B709 RID: 46857
		IAntiCheatClient AntiCheatClient { get; }

		// Token: 0x170014B7 RID: 5303
		// (get) Token: 0x0600B70A RID: 46858
		IAntiCheatServer AntiCheatServer { get; }

		// Token: 0x170014B8 RID: 5304
		// (get) Token: 0x0600B70B RID: 46859
		IUserIdentifierMappingService IdMappingService { get; }

		// Token: 0x170014B9 RID: 5305
		// (get) Token: 0x0600B70C RID: 46860
		IUserDetailsService UserDetailsService { get; }

		// Token: 0x170014BA RID: 5306
		// (get) Token: 0x0600B70D RID: 46861
		IPlayerReporting PlayerReporting { get; }

		// Token: 0x170014BB RID: 5307
		// (get) Token: 0x0600B70E RID: 46862
		ITextCensor TextCensor { get; }

		// Token: 0x170014BC RID: 5308
		// (get) Token: 0x0600B70F RID: 46863
		IRemoteFileStorage RemoteFileStorage { get; }

		// Token: 0x170014BD RID: 5309
		// (get) Token: 0x0600B710 RID: 46864
		IRemotePlayerFileStorage RemotePlayerFileStorage { get; }

		// Token: 0x170014BE RID: 5310
		// (get) Token: 0x0600B711 RID: 46865
		IList<IEntitlementValidator> EntitlementValidators { get; }

		// Token: 0x170014BF RID: 5311
		// (get) Token: 0x0600B712 RID: 46866
		PlayerInputManager Input { get; }

		// Token: 0x170014C0 RID: 5312
		// (get) Token: 0x0600B713 RID: 46867
		IVirtualKeyboard VirtualKeyboard { get; }

		// Token: 0x170014C1 RID: 5313
		// (get) Token: 0x0600B714 RID: 46868
		IAchievementManager AchievementManager { get; }

		// Token: 0x170014C2 RID: 5314
		// (get) Token: 0x0600B715 RID: 46869
		IRichPresence RichPresence { get; }

		// Token: 0x170014C3 RID: 5315
		// (get) Token: 0x0600B716 RID: 46870
		IApplicationStateController ApplicationState { get; }
	}
}
