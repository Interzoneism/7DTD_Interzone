using System;
using System.Collections.Generic;
using InControl;

namespace Platform
{
	// Token: 0x020017D3 RID: 6099
	public abstract class AbsPlatform : IPlatform
	{
		// Token: 0x0600B603 RID: 46595 RVA: 0x00466F94 File Offset: 0x00465194
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbsPlatform()
		{
			Type typeFromHandle = typeof(PlatformFactoryAttribute);
			object[] customAttributes = base.GetType().GetCustomAttributes(typeFromHandle, false);
			if (customAttributes.Length != 1)
			{
				throw new Exception("Platform has no PlatformFactory attribute");
			}
			PlatformFactoryAttribute platformFactoryAttribute = (PlatformFactoryAttribute)customAttributes[0];
			this.PlatformIdentifier = platformFactoryAttribute.TargetPlatform;
		}

		// Token: 0x1700146E RID: 5230
		// (get) Token: 0x0600B604 RID: 46596 RVA: 0x00466FE4 File Offset: 0x004651E4
		// (set) Token: 0x0600B605 RID: 46597 RVA: 0x00466FEC File Offset: 0x004651EC
		public bool AsServerOnly { get; set; }

		// Token: 0x1700146F RID: 5231
		// (get) Token: 0x0600B606 RID: 46598 RVA: 0x00466FF5 File Offset: 0x004651F5
		// (set) Token: 0x0600B607 RID: 46599 RVA: 0x00466FFD File Offset: 0x004651FD
		public bool IsCrossplatform { get; set; }

		// Token: 0x17001470 RID: 5232
		// (get) Token: 0x0600B608 RID: 46600 RVA: 0x00467006 File Offset: 0x00465206
		public string PlatformDisplayName
		{
			get
			{
				return PlatformManager.GetPlatformDisplayName(this.PlatformIdentifier);
			}
		}

		// Token: 0x0600B609 RID: 46601 RVA: 0x00467014 File Offset: 0x00465214
		public virtual void Init()
		{
			Log.Out("[Platform] Initializing " + this.PlatformIdentifier.ToString());
			IPlatformApi api = this.Api;
			if (api != null)
			{
				api.Init(this);
			}
			IUserClient user = this.User;
			if (user != null)
			{
				user.Init(this);
			}
			IAuthenticationClient authenticationClient = this.AuthenticationClient;
			if (authenticationClient != null)
			{
				authenticationClient.Init(this);
			}
			IAuthenticationServer authenticationServer = this.AuthenticationServer;
			if (authenticationServer != null)
			{
				authenticationServer.Init(this);
			}
			if (this.ServerListInterfaces != null)
			{
				foreach (IServerListInterface serverListInterface in this.ServerListInterfaces)
				{
					serverListInterface.Init(this);
				}
			}
			IMasterServerAnnouncer serverListAnnouncer = this.ServerListAnnouncer;
			if (serverListAnnouncer != null)
			{
				serverListAnnouncer.Init(this);
			}
			if (this.InviteListeners != null)
			{
				foreach (IJoinSessionGameInviteListener joinSessionGameInviteListener in this.InviteListeners)
				{
					if (joinSessionGameInviteListener != null)
					{
						joinSessionGameInviteListener.Init(this);
					}
				}
			}
			IMultiplayerInvitationDialog multiplayerInvitationDialog = this.MultiplayerInvitationDialog;
			if (multiplayerInvitationDialog != null)
			{
				multiplayerInvitationDialog.Init(this);
			}
			ILobbyHost lobbyHost = this.LobbyHost;
			if (lobbyHost != null)
			{
				lobbyHost.Init(this);
			}
			IPlayerInteractionsRecorder playerInteractionsRecorder = this.PlayerInteractionsRecorder;
			if (playerInteractionsRecorder != null)
			{
				playerInteractionsRecorder.Init(this);
			}
			IGameplayNotifier gameplayNotifier = this.GameplayNotifier;
			if (gameplayNotifier != null)
			{
				gameplayNotifier.Init(this);
			}
			IPartyVoice partyVoice = this.PartyVoice;
			if (partyVoice != null)
			{
				partyVoice.Init(this);
			}
			IUtils utils = this.Utils;
			if (utils != null)
			{
				utils.Init(this);
			}
			IUtils utils2 = this.Utils;
			if (utils2 != null)
			{
				utils2.ClearTempFiles();
			}
			if (this.Utils != null)
			{
				InputManager.OnDeviceDetached += this.Utils.ControllerDisconnected;
			}
			IAntiCheatClient antiCheatClient = this.AntiCheatClient;
			if (antiCheatClient != null)
			{
				antiCheatClient.Init(this);
			}
			IAntiCheatServer antiCheatServer = this.AntiCheatServer;
			if (antiCheatServer != null)
			{
				antiCheatServer.Init(this);
			}
			IPlayerReporting playerReporting = this.PlayerReporting;
			if (playerReporting != null)
			{
				playerReporting.Init(this);
			}
			ITextCensor textCensor = this.TextCensor;
			if (textCensor != null)
			{
				textCensor.Init(this);
			}
			IVirtualKeyboard virtualKeyboard = this.VirtualKeyboard;
			if (virtualKeyboard != null)
			{
				virtualKeyboard.Init(this);
			}
			IAchievementManager achievementManager = this.AchievementManager;
			if (achievementManager != null)
			{
				achievementManager.Init(this);
			}
			IRichPresence richPresence = this.RichPresence;
			if (richPresence != null)
			{
				richPresence.Init(this);
			}
			IRemoteFileStorage remoteFileStorage = this.RemoteFileStorage;
			if (remoteFileStorage != null)
			{
				remoteFileStorage.Init(this);
			}
			IRemotePlayerFileStorage remotePlayerFileStorage = this.RemotePlayerFileStorage;
			if (remotePlayerFileStorage != null)
			{
				remotePlayerFileStorage.Init(this);
			}
			if (this.EntitlementValidators != null)
			{
				foreach (IEntitlementValidator entitlementValidator in this.EntitlementValidators)
				{
					if (entitlementValidator != null)
					{
						entitlementValidator.Init(this);
					}
				}
			}
			IApplicationStateController applicationState = this.ApplicationState;
			if (applicationState != null)
			{
				applicationState.Init(this);
			}
			IUserDetailsService userDetailsService = this.UserDetailsService;
			if (userDetailsService != null)
			{
				userDetailsService.Init(this);
			}
			if (!GameManager.IsDedicatedServer && !this.AsServerOnly)
			{
				IPlatformApi api2 = this.Api;
				if (api2 != null)
				{
					api2.InitClientApis();
				}
			}
			if (GameManager.IsDedicatedServer)
			{
				IPlatformApi api3 = this.Api;
				if (api3 == null)
				{
					return;
				}
				api3.InitServerApis();
			}
		}

		// Token: 0x0600B60A RID: 46602 RVA: 0x00467310 File Offset: 0x00465510
		public virtual bool HasNetworkingEnabled(IList<string> _disabledProtocolNames)
		{
			string networkProtocolName = this.NetworkProtocolName;
			if (string.IsNullOrEmpty(networkProtocolName))
			{
				return false;
			}
			string text = networkProtocolName.ToLowerInvariant();
			bool flag = GameUtils.GetLaunchArgument("no" + text) == null && !_disabledProtocolNames.Contains(text);
			if (!flag)
			{
				Log.Out("[NET] Disabling protocol: " + networkProtocolName);
			}
			return flag;
		}

		// Token: 0x0600B60B RID: 46603 RVA: 0x00019766 File Offset: 0x00017966
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual IPlatformNetworkServer instantiateNetworkServer(ProtocolManager _protocolManager)
		{
			return null;
		}

		// Token: 0x0600B60C RID: 46604 RVA: 0x00019766 File Offset: 0x00017966
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual IPlatformNetworkClient instantiateNetworkClient(ProtocolManager _protocolManager)
		{
			return null;
		}

		// Token: 0x0600B60D RID: 46605 RVA: 0x00467368 File Offset: 0x00465568
		public INetworkServer GetNetworkingServer(ProtocolManager _protocolManager)
		{
			IPlatformNetworkServer result;
			if ((result = this.NetworkServer) == null)
			{
				result = (this.NetworkServer = this.instantiateNetworkServer(_protocolManager));
			}
			return result;
		}

		// Token: 0x0600B60E RID: 46606 RVA: 0x00467390 File Offset: 0x00465590
		public INetworkClient GetNetworkingClient(ProtocolManager _protocolManager)
		{
			IPlatformNetworkClient result;
			if ((result = this.NetworkClient) == null)
			{
				result = (this.NetworkClient = this.instantiateNetworkClient(_protocolManager));
			}
			return result;
		}

		// Token: 0x0600B60F RID: 46607 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void UserAdded(PlatformUserIdentifierAbs _id, bool _isPrimary)
		{
		}

		// Token: 0x0600B610 RID: 46608 RVA: 0x004673B7 File Offset: 0x004655B7
		public virtual string[] GetArgumentsForRelaunch()
		{
			return new string[0];
		}

		// Token: 0x0600B611 RID: 46609
		public abstract void CreateInstances();

		// Token: 0x0600B612 RID: 46610 RVA: 0x004673C0 File Offset: 0x004655C0
		public virtual void Update()
		{
			IPlatformApi api = this.Api;
			if (api != null)
			{
				api.Update();
			}
			IMasterServerAnnouncer serverListAnnouncer = this.ServerListAnnouncer;
			if (serverListAnnouncer != null)
			{
				serverListAnnouncer.Update();
			}
			IAntiCheatServer antiCheatServer = this.AntiCheatServer;
			if (antiCheatServer != null)
			{
				antiCheatServer.Update();
			}
			PlayerInputManager input = this.Input;
			if (input != null)
			{
				input.Update();
			}
			ITextCensor textCensor = this.TextCensor;
			if (textCensor != null)
			{
				textCensor.Update();
			}
			IApplicationStateController applicationState = this.ApplicationState;
			if (applicationState == null)
			{
				return;
			}
			applicationState.Update();
		}

		// Token: 0x0600B613 RID: 46611 RVA: 0x00002914 File Offset: 0x00000B14
		public void LateUpdate()
		{
		}

		// Token: 0x0600B614 RID: 46612 RVA: 0x00467434 File Offset: 0x00465634
		public virtual void Destroy()
		{
			this.RichPresence = null;
			IAchievementManager achievementManager = this.AchievementManager;
			if (achievementManager != null)
			{
				achievementManager.Destroy();
			}
			this.AchievementManager = null;
			this.Input = null;
			IApplicationStateController applicationState = this.ApplicationState;
			if (applicationState != null)
			{
				applicationState.Destroy();
			}
			this.ApplicationState = null;
			IVirtualKeyboard virtualKeyboard = this.VirtualKeyboard;
			if (virtualKeyboard != null)
			{
				virtualKeyboard.Destroy();
			}
			this.VirtualKeyboard = null;
			this.NetworkClient = null;
			this.NetworkServer = null;
			this.PlayerReporting = null;
			this.TextCensor = null;
			IAntiCheatServer antiCheatServer = this.AntiCheatServer;
			if (antiCheatServer != null)
			{
				antiCheatServer.Destroy();
			}
			this.AntiCheatServer = null;
			IAntiCheatClient antiCheatClient = this.AntiCheatClient;
			if (antiCheatClient != null)
			{
				antiCheatClient.Destroy();
			}
			this.AntiCheatClient = null;
			this.Memory = null;
			IUtils utils = this.Utils;
			if (utils != null)
			{
				utils.ClearTempFiles();
			}
			this.Utils = null;
			IPartyVoice partyVoice = this.PartyVoice;
			if (partyVoice != null)
			{
				partyVoice.Destroy();
			}
			this.PartyVoice = null;
			this.LobbyHost = null;
			IPlayerInteractionsRecorder playerInteractionsRecorder = this.PlayerInteractionsRecorder;
			if (playerInteractionsRecorder != null)
			{
				playerInteractionsRecorder.Destroy();
			}
			this.PlayerInteractionsRecorder = null;
			this.InviteListeners = null;
			this.ServerListAnnouncer = null;
			this.ServerListInterfaces = null;
			this.AuthenticationServer = null;
			IAuthenticationClient authenticationClient = this.AuthenticationClient;
			if (authenticationClient != null)
			{
				authenticationClient.Destroy();
			}
			this.AuthenticationClient = null;
			IUserClient user = this.User;
			if (user != null)
			{
				user.Destroy();
			}
			this.User = null;
			IPlatformApi api = this.Api;
			if (api != null)
			{
				api.Destroy();
			}
			this.Api = null;
		}

		// Token: 0x17001471 RID: 5233
		// (get) Token: 0x0600B615 RID: 46613 RVA: 0x0046759D File Offset: 0x0046579D
		public EPlatformIdentifier PlatformIdentifier { get; }

		// Token: 0x17001472 RID: 5234
		// (get) Token: 0x0600B616 RID: 46614 RVA: 0x004675A5 File Offset: 0x004657A5
		// (set) Token: 0x0600B617 RID: 46615 RVA: 0x004675AD File Offset: 0x004657AD
		public IPlatformApi Api { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001473 RID: 5235
		// (get) Token: 0x0600B618 RID: 46616 RVA: 0x004675B6 File Offset: 0x004657B6
		// (set) Token: 0x0600B619 RID: 46617 RVA: 0x004675BE File Offset: 0x004657BE
		public IUserClient User { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001474 RID: 5236
		// (get) Token: 0x0600B61A RID: 46618 RVA: 0x004675C7 File Offset: 0x004657C7
		// (set) Token: 0x0600B61B RID: 46619 RVA: 0x004675CF File Offset: 0x004657CF
		public IAuthenticationClient AuthenticationClient { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001475 RID: 5237
		// (get) Token: 0x0600B61C RID: 46620 RVA: 0x004675D8 File Offset: 0x004657D8
		// (set) Token: 0x0600B61D RID: 46621 RVA: 0x004675E0 File Offset: 0x004657E0
		public IAuthenticationServer AuthenticationServer { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001476 RID: 5238
		// (get) Token: 0x0600B61E RID: 46622 RVA: 0x004675E9 File Offset: 0x004657E9
		// (set) Token: 0x0600B61F RID: 46623 RVA: 0x004675F1 File Offset: 0x004657F1
		public IList<IServerListInterface> ServerListInterfaces { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001477 RID: 5239
		// (get) Token: 0x0600B620 RID: 46624 RVA: 0x004675FA File Offset: 0x004657FA
		// (set) Token: 0x0600B621 RID: 46625 RVA: 0x00467602 File Offset: 0x00465802
		public IServerListInterface ServerLookupInterface { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001478 RID: 5240
		// (get) Token: 0x0600B622 RID: 46626 RVA: 0x0046760B File Offset: 0x0046580B
		// (set) Token: 0x0600B623 RID: 46627 RVA: 0x00467613 File Offset: 0x00465813
		public IMasterServerAnnouncer ServerListAnnouncer { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001479 RID: 5241
		// (get) Token: 0x0600B624 RID: 46628 RVA: 0x0046761C File Offset: 0x0046581C
		// (set) Token: 0x0600B625 RID: 46629 RVA: 0x00467624 File Offset: 0x00465824
		public ILobbyHost LobbyHost { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700147A RID: 5242
		// (get) Token: 0x0600B626 RID: 46630 RVA: 0x0046762D File Offset: 0x0046582D
		// (set) Token: 0x0600B627 RID: 46631 RVA: 0x00467635 File Offset: 0x00465835
		public IPlayerInteractionsRecorder PlayerInteractionsRecorder { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700147B RID: 5243
		// (get) Token: 0x0600B628 RID: 46632 RVA: 0x0046763E File Offset: 0x0046583E
		// (set) Token: 0x0600B629 RID: 46633 RVA: 0x00467646 File Offset: 0x00465846
		public IGameplayNotifier GameplayNotifier { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700147C RID: 5244
		// (get) Token: 0x0600B62A RID: 46634 RVA: 0x0046764F File Offset: 0x0046584F
		// (set) Token: 0x0600B62B RID: 46635 RVA: 0x00467657 File Offset: 0x00465857
		public IList<IJoinSessionGameInviteListener> InviteListeners { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700147D RID: 5245
		// (get) Token: 0x0600B62C RID: 46636 RVA: 0x00467660 File Offset: 0x00465860
		// (set) Token: 0x0600B62D RID: 46637 RVA: 0x00467668 File Offset: 0x00465868
		public IMultiplayerInvitationDialog MultiplayerInvitationDialog { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700147E RID: 5246
		// (get) Token: 0x0600B62E RID: 46638 RVA: 0x00467671 File Offset: 0x00465871
		// (set) Token: 0x0600B62F RID: 46639 RVA: 0x00467679 File Offset: 0x00465879
		public IPartyVoice PartyVoice { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700147F RID: 5247
		// (get) Token: 0x0600B630 RID: 46640 RVA: 0x00467682 File Offset: 0x00465882
		// (set) Token: 0x0600B631 RID: 46641 RVA: 0x0046768A File Offset: 0x0046588A
		public IUtils Utils { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001480 RID: 5248
		// (get) Token: 0x0600B632 RID: 46642 RVA: 0x00467693 File Offset: 0x00465893
		// (set) Token: 0x0600B633 RID: 46643 RVA: 0x0046769B File Offset: 0x0046589B
		public IPlatformMemory Memory { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001481 RID: 5249
		// (get) Token: 0x0600B634 RID: 46644 RVA: 0x004676A4 File Offset: 0x004658A4
		// (set) Token: 0x0600B635 RID: 46645 RVA: 0x004676AC File Offset: 0x004658AC
		public IAntiCheatClient AntiCheatClient { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001482 RID: 5250
		// (get) Token: 0x0600B636 RID: 46646 RVA: 0x004676B5 File Offset: 0x004658B5
		// (set) Token: 0x0600B637 RID: 46647 RVA: 0x004676BD File Offset: 0x004658BD
		public IAntiCheatServer AntiCheatServer { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001483 RID: 5251
		// (get) Token: 0x0600B638 RID: 46648 RVA: 0x004676C6 File Offset: 0x004658C6
		// (set) Token: 0x0600B639 RID: 46649 RVA: 0x004676CE File Offset: 0x004658CE
		public IUserIdentifierMappingService IdMappingService { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001484 RID: 5252
		// (get) Token: 0x0600B63A RID: 46650 RVA: 0x004676D7 File Offset: 0x004658D7
		// (set) Token: 0x0600B63B RID: 46651 RVA: 0x004676DF File Offset: 0x004658DF
		public IUserDetailsService UserDetailsService { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001485 RID: 5253
		// (get) Token: 0x0600B63C RID: 46652 RVA: 0x004676E8 File Offset: 0x004658E8
		// (set) Token: 0x0600B63D RID: 46653 RVA: 0x004676F0 File Offset: 0x004658F0
		public IPlayerReporting PlayerReporting { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001486 RID: 5254
		// (get) Token: 0x0600B63E RID: 46654 RVA: 0x004676F9 File Offset: 0x004658F9
		// (set) Token: 0x0600B63F RID: 46655 RVA: 0x00467701 File Offset: 0x00465901
		public ITextCensor TextCensor { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001487 RID: 5255
		// (get) Token: 0x0600B640 RID: 46656 RVA: 0x0046770A File Offset: 0x0046590A
		// (set) Token: 0x0600B641 RID: 46657 RVA: 0x00467712 File Offset: 0x00465912
		public IRemoteFileStorage RemoteFileStorage { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001488 RID: 5256
		// (get) Token: 0x0600B642 RID: 46658 RVA: 0x0046771B File Offset: 0x0046591B
		// (set) Token: 0x0600B643 RID: 46659 RVA: 0x00467723 File Offset: 0x00465923
		public IRemotePlayerFileStorage RemotePlayerFileStorage { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001489 RID: 5257
		// (get) Token: 0x0600B644 RID: 46660 RVA: 0x0046772C File Offset: 0x0046592C
		// (set) Token: 0x0600B645 RID: 46661 RVA: 0x00467734 File Offset: 0x00465934
		public IList<IEntitlementValidator> EntitlementValidators { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700148A RID: 5258
		// (get) Token: 0x0600B646 RID: 46662 RVA: 0x0046773D File Offset: 0x0046593D
		// (set) Token: 0x0600B647 RID: 46663 RVA: 0x00467745 File Offset: 0x00465945
		public PlayerInputManager Input { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700148B RID: 5259
		// (get) Token: 0x0600B648 RID: 46664 RVA: 0x0046774E File Offset: 0x0046594E
		// (set) Token: 0x0600B649 RID: 46665 RVA: 0x00467756 File Offset: 0x00465956
		public IVirtualKeyboard VirtualKeyboard { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700148C RID: 5260
		// (get) Token: 0x0600B64A RID: 46666 RVA: 0x0046775F File Offset: 0x0046595F
		// (set) Token: 0x0600B64B RID: 46667 RVA: 0x00467767 File Offset: 0x00465967
		public IAchievementManager AchievementManager { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700148D RID: 5261
		// (get) Token: 0x0600B64C RID: 46668 RVA: 0x00467770 File Offset: 0x00465970
		// (set) Token: 0x0600B64D RID: 46669 RVA: 0x00467778 File Offset: 0x00465978
		public IRichPresence RichPresence { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700148E RID: 5262
		// (get) Token: 0x0600B64E RID: 46670 RVA: 0x00467781 File Offset: 0x00465981
		// (set) Token: 0x0600B64F RID: 46671 RVA: 0x00467789 File Offset: 0x00465989
		public IApplicationStateController ApplicationState { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700148F RID: 5263
		// (get) Token: 0x0600B650 RID: 46672 RVA: 0x00467792 File Offset: 0x00465992
		public virtual string NetworkProtocolName { [PublicizedFrom(EAccessModifier.Protected)] get; }

		// Token: 0x04008F8C RID: 36748
		[PublicizedFrom(EAccessModifier.Protected)]
		public IPlatformNetworkServer NetworkServer;

		// Token: 0x04008F8D RID: 36749
		[PublicizedFrom(EAccessModifier.Protected)]
		public IPlatformNetworkClient NetworkClient;
	}
}
