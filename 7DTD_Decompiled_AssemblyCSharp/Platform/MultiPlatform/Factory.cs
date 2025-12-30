using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Platform.MultiPlatform
{
	// Token: 0x020018E1 RID: 6369
	[Preserve]
	public class Factory : IPlatform
	{
		// Token: 0x17001578 RID: 5496
		// (get) Token: 0x0600BC17 RID: 48151 RVA: 0x00477543 File Offset: 0x00475743
		// (set) Token: 0x0600BC18 RID: 48152 RVA: 0x0047754B File Offset: 0x0047574B
		public bool AsServerOnly { get; set; }

		// Token: 0x17001579 RID: 5497
		// (get) Token: 0x0600BC19 RID: 48153 RVA: 0x00477554 File Offset: 0x00475754
		// (set) Token: 0x0600BC1A RID: 48154 RVA: 0x0047755C File Offset: 0x0047575C
		public bool IsCrossplatform { get; set; }

		// Token: 0x1700157A RID: 5498
		// (get) Token: 0x0600BC1B RID: 48155 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public EPlatformIdentifier PlatformIdentifier
		{
			get
			{
				return EPlatformIdentifier.None;
			}
		}

		// Token: 0x1700157B RID: 5499
		// (get) Token: 0x0600BC1C RID: 48156 RVA: 0x00019766 File Offset: 0x00017966
		public string PlatformDisplayName
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600BC1D RID: 48157 RVA: 0x00477565 File Offset: 0x00475765
		public void Init()
		{
			this.User.Init(this);
			this.ServerListAnnouncer.Init(this);
			this.RichPresence.Init(this);
		}

		// Token: 0x0600BC1E RID: 48158 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public bool HasNetworkingEnabled(IList<string> _disabledProtocolNames)
		{
			return false;
		}

		// Token: 0x0600BC1F RID: 48159 RVA: 0x000424BD File Offset: 0x000406BD
		public INetworkServer GetNetworkingServer(ProtocolManager _protocolManager)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BC20 RID: 48160 RVA: 0x000424BD File Offset: 0x000406BD
		public INetworkClient GetNetworkingClient(ProtocolManager _protocolManager)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BC21 RID: 48161 RVA: 0x0047758B File Offset: 0x0047578B
		public void UserAdded(PlatformUserIdentifierAbs _id, bool _isPrimary)
		{
			PlatformManager.NativePlatform.UserAdded(_id, _isPrimary);
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			if (crossplatformPlatform == null)
			{
				return;
			}
			crossplatformPlatform.UserAdded(_id, _isPrimary);
		}

		// Token: 0x0600BC22 RID: 48162 RVA: 0x004673B7 File Offset: 0x004655B7
		public string[] GetArgumentsForRelaunch()
		{
			return new string[0];
		}

		// Token: 0x0600BC23 RID: 48163 RVA: 0x004775AA File Offset: 0x004757AA
		public void CreateInstances()
		{
			this.User = new User();
			this.ServerListAnnouncer = new ServerListAnnouncer();
			this.RichPresence = new RichPresence();
			this.PlayerInteractionsRecorder = new PlayerInteractionsRecorderMulti();
		}

		// Token: 0x0600BC24 RID: 48164 RVA: 0x00002914 File Offset: 0x00000B14
		public void Update()
		{
		}

		// Token: 0x0600BC25 RID: 48165 RVA: 0x00002914 File Offset: 0x00000B14
		public void LateUpdate()
		{
		}

		// Token: 0x0600BC26 RID: 48166 RVA: 0x004775D8 File Offset: 0x004757D8
		public void Destroy()
		{
			this.ServerListAnnouncer = null;
			IUserClient user = this.User;
			if (user != null)
			{
				user.Destroy();
			}
			this.User = null;
		}

		// Token: 0x1700157C RID: 5500
		// (get) Token: 0x0600BC27 RID: 48167 RVA: 0x00019766 File Offset: 0x00017966
		public IPlatformApi Api
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700157D RID: 5501
		// (get) Token: 0x0600BC28 RID: 48168 RVA: 0x004775F9 File Offset: 0x004757F9
		// (set) Token: 0x0600BC29 RID: 48169 RVA: 0x00477601 File Offset: 0x00475801
		public IUserClient User { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700157E RID: 5502
		// (get) Token: 0x0600BC2A RID: 48170 RVA: 0x00019766 File Offset: 0x00017966
		public IAuthenticationClient AuthenticationClient
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700157F RID: 5503
		// (get) Token: 0x0600BC2B RID: 48171 RVA: 0x00019766 File Offset: 0x00017966
		public IAuthenticationServer AuthenticationServer
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001580 RID: 5504
		// (get) Token: 0x0600BC2C RID: 48172 RVA: 0x0047760C File Offset: 0x0047580C
		public IList<IServerListInterface> ServerListInterfaces
		{
			get
			{
				if (this.serverListInterfaces != null)
				{
					return this.serverListInterfaces;
				}
				this.serverListInterfaces = new List<IServerListInterface>();
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				IList<IServerListInterface> list = (crossplatformPlatform != null) ? crossplatformPlatform.ServerListInterfaces : null;
				if (list != null)
				{
					this.serverListInterfaces.AddRange(list);
				}
				list = PlatformManager.NativePlatform.ServerListInterfaces;
				if (list != null)
				{
					this.serverListInterfaces.AddRange(list);
				}
				return this.serverListInterfaces;
			}
		}

		// Token: 0x17001581 RID: 5505
		// (get) Token: 0x0600BC2D RID: 48173 RVA: 0x00019766 File Offset: 0x00017966
		public IServerListInterface ServerLookupInterface
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001582 RID: 5506
		// (get) Token: 0x0600BC2E RID: 48174 RVA: 0x00477674 File Offset: 0x00475874
		// (set) Token: 0x0600BC2F RID: 48175 RVA: 0x0047767C File Offset: 0x0047587C
		public IMasterServerAnnouncer ServerListAnnouncer { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001583 RID: 5507
		// (get) Token: 0x0600BC30 RID: 48176 RVA: 0x00019766 File Offset: 0x00017966
		public ILobbyHost LobbyHost
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001584 RID: 5508
		// (get) Token: 0x0600BC31 RID: 48177 RVA: 0x00477685 File Offset: 0x00475885
		// (set) Token: 0x0600BC32 RID: 48178 RVA: 0x0047768D File Offset: 0x0047588D
		public IPlayerInteractionsRecorder PlayerInteractionsRecorder { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001585 RID: 5509
		// (get) Token: 0x0600BC33 RID: 48179 RVA: 0x00477696 File Offset: 0x00475896
		// (set) Token: 0x0600BC34 RID: 48180 RVA: 0x0047769E File Offset: 0x0047589E
		public IGameplayNotifier GameplayNotifier { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001586 RID: 5510
		// (get) Token: 0x0600BC35 RID: 48181 RVA: 0x004776A7 File Offset: 0x004758A7
		public IList<IJoinSessionGameInviteListener> InviteListeners
		{
			get
			{
				return PlatformManager.NativePlatform.InviteListeners;
			}
		}

		// Token: 0x17001587 RID: 5511
		// (get) Token: 0x0600BC36 RID: 48182 RVA: 0x004776B3 File Offset: 0x004758B3
		public IMultiplayerInvitationDialog MultiplayerInvitationDialog
		{
			get
			{
				return PlatformManager.NativePlatform.MultiplayerInvitationDialog;
			}
		}

		// Token: 0x17001588 RID: 5512
		// (get) Token: 0x0600BC37 RID: 48183 RVA: 0x004776BF File Offset: 0x004758BF
		public IPartyVoice PartyVoice
		{
			get
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				return ((crossplatformPlatform != null) ? crossplatformPlatform.PartyVoice : null) ?? PlatformManager.NativePlatform.PartyVoice;
			}
		}

		// Token: 0x17001589 RID: 5513
		// (get) Token: 0x0600BC38 RID: 48184 RVA: 0x00019766 File Offset: 0x00017966
		public IUtils Utils
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700158A RID: 5514
		// (get) Token: 0x0600BC39 RID: 48185 RVA: 0x004776E0 File Offset: 0x004758E0
		public IPlatformMemory Memory
		{
			get
			{
				return PlatformManager.NativePlatform.Memory;
			}
		}

		// Token: 0x1700158B RID: 5515
		// (get) Token: 0x0600BC3A RID: 48186 RVA: 0x004776EC File Offset: 0x004758EC
		public IAntiCheatClient AntiCheatClient
		{
			get
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				return ((crossplatformPlatform != null) ? crossplatformPlatform.AntiCheatClient : null) ?? PlatformManager.NativePlatform.AntiCheatClient;
			}
		}

		// Token: 0x1700158C RID: 5516
		// (get) Token: 0x0600BC3B RID: 48187 RVA: 0x0047770D File Offset: 0x0047590D
		public IAntiCheatServer AntiCheatServer
		{
			get
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				return ((crossplatformPlatform != null) ? crossplatformPlatform.AntiCheatServer : null) ?? PlatformManager.NativePlatform.AntiCheatServer;
			}
		}

		// Token: 0x1700158D RID: 5517
		// (get) Token: 0x0600BC3C RID: 48188 RVA: 0x00019766 File Offset: 0x00017966
		public IUserIdentifierMappingService IdMappingService
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700158E RID: 5518
		// (get) Token: 0x0600BC3D RID: 48189 RVA: 0x00019766 File Offset: 0x00017966
		public IUserDetailsService UserDetailsService
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700158F RID: 5519
		// (get) Token: 0x0600BC3E RID: 48190 RVA: 0x0047772E File Offset: 0x0047592E
		public IPlayerReporting PlayerReporting
		{
			get
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				return ((crossplatformPlatform != null) ? crossplatformPlatform.PlayerReporting : null) ?? PlatformManager.NativePlatform.PlayerReporting;
			}
		}

		// Token: 0x17001590 RID: 5520
		// (get) Token: 0x0600BC3F RID: 48191 RVA: 0x0047774F File Offset: 0x0047594F
		public ITextCensor TextCensor
		{
			get
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				ITextCensor result;
				if ((result = ((crossplatformPlatform != null) ? crossplatformPlatform.TextCensor : null)) == null)
				{
					IPlatform nativePlatform = PlatformManager.NativePlatform;
					if (nativePlatform == null)
					{
						return null;
					}
					result = nativePlatform.TextCensor;
				}
				return result;
			}
		}

		// Token: 0x17001591 RID: 5521
		// (get) Token: 0x0600BC40 RID: 48192 RVA: 0x00477776 File Offset: 0x00475976
		public IRemoteFileStorage RemoteFileStorage
		{
			get
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				return ((crossplatformPlatform != null) ? crossplatformPlatform.RemoteFileStorage : null) ?? PlatformManager.NativePlatform.RemoteFileStorage;
			}
		}

		// Token: 0x17001592 RID: 5522
		// (get) Token: 0x0600BC41 RID: 48193 RVA: 0x00477797 File Offset: 0x00475997
		public IRemotePlayerFileStorage RemotePlayerFileStorage
		{
			get
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				return ((crossplatformPlatform != null) ? crossplatformPlatform.RemotePlayerFileStorage : null) ?? PlatformManager.NativePlatform.RemotePlayerFileStorage;
			}
		}

		// Token: 0x17001593 RID: 5523
		// (get) Token: 0x0600BC42 RID: 48194 RVA: 0x004777B8 File Offset: 0x004759B8
		public IList<IEntitlementValidator> EntitlementValidators
		{
			get
			{
				return PlatformManager.NativePlatform.EntitlementValidators;
			}
		}

		// Token: 0x17001594 RID: 5524
		// (get) Token: 0x0600BC43 RID: 48195 RVA: 0x00019766 File Offset: 0x00017966
		public IPlatformNetworkServer NetworkServer
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001595 RID: 5525
		// (get) Token: 0x0600BC44 RID: 48196 RVA: 0x00019766 File Offset: 0x00017966
		public PlayerInputManager Input
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001596 RID: 5526
		// (get) Token: 0x0600BC45 RID: 48197 RVA: 0x00019766 File Offset: 0x00017966
		public IVirtualKeyboard VirtualKeyboard
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001597 RID: 5527
		// (get) Token: 0x0600BC46 RID: 48198 RVA: 0x00019766 File Offset: 0x00017966
		public IAchievementManager AchievementManager
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001598 RID: 5528
		// (get) Token: 0x0600BC47 RID: 48199 RVA: 0x004777C4 File Offset: 0x004759C4
		// (set) Token: 0x0600BC48 RID: 48200 RVA: 0x004777CC File Offset: 0x004759CC
		public IRichPresence RichPresence { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001599 RID: 5529
		// (get) Token: 0x0600BC49 RID: 48201 RVA: 0x004777D5 File Offset: 0x004759D5
		public IApplicationStateController ApplicationState
		{
			get
			{
				return PlatformManager.NativePlatform.ApplicationState;
			}
		}

		// Token: 0x040092E7 RID: 37607
		[PublicizedFrom(EAccessModifier.Private)]
		public List<IServerListInterface> serverListInterfaces;
	}
}
