using System;
using System.Collections.Generic;
using Platform.LAN;
using Twitch;
using UnityEngine.Scripting;

namespace Platform.Steam
{
	// Token: 0x020018B6 RID: 6326
	[Preserve]
	[PlatformFactory(EPlatformIdentifier.Steam)]
	public class Factory : AbsPlatform
	{
		// Token: 0x0600BAB6 RID: 47798 RVA: 0x00472354 File Offset: 0x00470554
		public override void CreateInstances()
		{
			base.Api = new Api();
			base.User = new User();
			base.AuthenticationServer = new AuthenticationServer();
			base.ServerListAnnouncer = new MasterServerAnnouncer();
			base.LobbyHost = new LobbyHost();
			base.MultiplayerInvitationDialog = new MultiplayerInvitationDialogSteam();
			base.Utils = new Utils();
			if (!base.AsServerOnly && !GameManager.IsDedicatedServer)
			{
				base.AchievementManager = new AchievementManager();
				base.RichPresence = new RichPresence();
				base.InviteListeners = new List<IJoinSessionGameInviteListener>
				{
					new JoinSessionGameInviteListener(),
					DiscordInviteListener.ListenerInstance
				};
				base.EntitlementValidators = new List<IEntitlementValidator>
				{
					new DownloadableContentValidator(),
					new TwitchEntitlementManager()
				};
				base.AuthenticationClient = new AuthenticationClient();
				base.ServerListInterfaces = new List<IServerListInterface>
				{
					new LobbyListInternet(),
					new LobbyListFriends(),
					new LANServerList()
				};
				if (PlatformManager.CrossplatformPlatform == null)
				{
					base.ServerListInterfaces.Add(new MasterServerList(EServerRelationType.Internet));
					base.ServerListInterfaces.Add(new MasterServerList(EServerRelationType.LAN));
					base.ServerListInterfaces.Add(new MasterServerList(EServerRelationType.Friends));
					base.ServerListInterfaces.Add(new MasterServerList(EServerRelationType.Favorites));
					base.ServerListInterfaces.Add(new MasterServerList(EServerRelationType.History));
				}
				base.VirtualKeyboard = new VirtualKeyboard();
			}
			if (!base.AsServerOnly)
			{
				base.Input = new PlayerInputManager();
			}
		}

		// Token: 0x17001544 RID: 5444
		// (get) Token: 0x0600BAB7 RID: 47799 RVA: 0x004724CC File Offset: 0x004706CC
		public override string NetworkProtocolName
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return "SteamNetworking";
			}
		}

		// Token: 0x0600BAB8 RID: 47800 RVA: 0x004724D3 File Offset: 0x004706D3
		[PublicizedFrom(EAccessModifier.Protected)]
		public override IPlatformNetworkServer instantiateNetworkServer(ProtocolManager _protocolManager)
		{
			return new NetworkServerSteam(this, _protocolManager);
		}

		// Token: 0x0600BAB9 RID: 47801 RVA: 0x004724DC File Offset: 0x004706DC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override IPlatformNetworkClient instantiateNetworkClient(ProtocolManager _protocolManager)
		{
			return new NetworkClientSteam(this, _protocolManager);
		}
	}
}
