using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Audio;
using Challenges;
using Newtonsoft.Json.Linq;
using Twitch.PubSub;
using UniLinq;
using UnityEngine;

namespace Twitch
{
	// Token: 0x02001571 RID: 5489
	public class TwitchManager
	{
		// Token: 0x170012B2 RID: 4786
		// (get) Token: 0x0600A877 RID: 43127 RVA: 0x004247BB File Offset: 0x004229BB
		public static TwitchManager Current
		{
			get
			{
				if (TwitchManager.instance == null)
				{
					TwitchManager.instance = new TwitchManager();
				}
				return TwitchManager.instance;
			}
		}

		// Token: 0x170012B3 RID: 4787
		// (get) Token: 0x0600A878 RID: 43128 RVA: 0x004247D3 File Offset: 0x004229D3
		public static bool HasInstance
		{
			get
			{
				return TwitchManager.instance != null;
			}
		}

		// Token: 0x0600A879 RID: 43129 RVA: 0x004247E0 File Offset: 0x004229E0
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchManager()
		{
			this.ViewerData = new TwitchViewerData(this);
			this.VotingManager = new TwitchVotingManager(this);
			this.HighestGameStage = -1;
			this.UseProgression = true;
		}

		// Token: 0x0600A87A RID: 43130 RVA: 0x00424AB9 File Offset: 0x00422CB9
		public void Cleanup()
		{
			this.Disconnect();
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.InitState == TwitchManager.InitStates.Ready)
			{
				this.SaveViewerData();
			}
			TwitchManager.instance = null;
		}

		// Token: 0x170012B4 RID: 4788
		// (get) Token: 0x0600A87B RID: 43131 RVA: 0x00424AE2 File Offset: 0x00422CE2
		// (set) Token: 0x0600A87C RID: 43132 RVA: 0x00424AEA File Offset: 0x00422CEA
		public byte CurrentFileVersion { get; set; }

		// Token: 0x170012B5 RID: 4789
		// (get) Token: 0x0600A87D RID: 43133 RVA: 0x00424AF3 File Offset: 0x00422CF3
		// (set) Token: 0x0600A87E RID: 43134 RVA: 0x00424AFB File Offset: 0x00422CFB
		public byte CurrentMainFileVersion { get; set; }

		// Token: 0x170012B6 RID: 4790
		// (get) Token: 0x0600A87F RID: 43135 RVA: 0x00424B04 File Offset: 0x00422D04
		// (set) Token: 0x0600A880 RID: 43136 RVA: 0x00424B0C File Offset: 0x00422D0C
		public bool OverrideProgession
		{
			get
			{
				return this.overrideProgression;
			}
			set
			{
				if (this.overrideProgression != value)
				{
					this.overrideProgression = value;
					if (this.InitState == TwitchManager.InitStates.Ready)
					{
						this.resetCommandsNeeded = true;
					}
				}
			}
		}

		// Token: 0x170012B7 RID: 4791
		// (get) Token: 0x0600A881 RID: 43137 RVA: 0x00424B2E File Offset: 0x00422D2E
		// (set) Token: 0x0600A882 RID: 43138 RVA: 0x00424B36 File Offset: 0x00422D36
		public bool UseProgression { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x170012B8 RID: 4792
		// (get) Token: 0x0600A883 RID: 43139 RVA: 0x00424B3F File Offset: 0x00422D3F
		// (set) Token: 0x0600A884 RID: 43140 RVA: 0x00424B47 File Offset: 0x00422D47
		public int HighestGameStage { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x170012B9 RID: 4793
		// (get) Token: 0x0600A885 RID: 43141 RVA: 0x00424B50 File Offset: 0x00422D50
		public static bool BossHordeActive
		{
			get
			{
				return TwitchManager.HasInstance && TwitchManager.Current.IsBossHordeActive;
			}
		}

		// Token: 0x170012BA RID: 4794
		// (get) Token: 0x0600A886 RID: 43142 RVA: 0x00424B65 File Offset: 0x00422D65
		// (set) Token: 0x0600A887 RID: 43143 RVA: 0x00424B6D File Offset: 0x00422D6D
		public TwitchManager.IntegrationSettings IntegrationSetting
		{
			get
			{
				return this.integrationSetting;
			}
			set
			{
				if (this.integrationSetting != value)
				{
					this.integrationSetting = value;
					this.IntegrationTypeChanged();
				}
			}
		}

		// Token: 0x170012BB RID: 4795
		// (get) Token: 0x0600A888 RID: 43144 RVA: 0x00424B85 File Offset: 0x00422D85
		// (set) Token: 0x0600A889 RID: 43145 RVA: 0x00424B8D File Offset: 0x00422D8D
		public float ActionCooldownModifier
		{
			get
			{
				return this.actionCooldownModifier;
			}
			set
			{
				if (value != this.actionCooldownModifier)
				{
					this.actionCooldownModifier = value;
					this.UpdateActionCooldowns(value);
				}
			}
		}

		// Token: 0x170012BC RID: 4796
		// (get) Token: 0x0600A88A RID: 43146 RVA: 0x00424BA6 File Offset: 0x00422DA6
		public bool AllowActions
		{
			get
			{
				return !this.CurrentActionPreset.IsEmpty;
			}
		}

		// Token: 0x170012BD RID: 4797
		// (get) Token: 0x0600A88B RID: 43147 RVA: 0x00424BB6 File Offset: 0x00422DB6
		public bool AllowEvents
		{
			get
			{
				return !this.CurrentEventPreset.IsEmpty;
			}
		}

		// Token: 0x170012BE RID: 4798
		// (get) Token: 0x0600A88C RID: 43148 RVA: 0x00424BC6 File Offset: 0x00422DC6
		public bool OnCooldown
		{
			get
			{
				return this.CooldownTime > 0f || this.CurrentCooldownPreset.CooldownType == CooldownPreset.CooldownTypes.Always;
			}
		}

		// Token: 0x170012BF RID: 4799
		// (get) Token: 0x0600A88D RID: 43149 RVA: 0x00424BE5 File Offset: 0x00422DE5
		// (set) Token: 0x0600A88E RID: 43150 RVA: 0x00424BED File Offset: 0x00422DED
		public float BitPriceMultiplier
		{
			get
			{
				return this.bitPriceMultiplier;
			}
			set
			{
				if (this.bitPriceMultiplier != value)
				{
					this.bitPriceMultiplier = value;
					this.ResetPrices();
				}
			}
		}

		// Token: 0x170012C0 RID: 4800
		// (get) Token: 0x0600A88F RID: 43151 RVA: 0x00424C05 File Offset: 0x00422E05
		// (set) Token: 0x0600A890 RID: 43152 RVA: 0x00424C0D File Offset: 0x00422E0D
		public TwitchVotingManager VotingManager { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x170012C1 RID: 4801
		// (get) Token: 0x0600A891 RID: 43153 RVA: 0x00424C16 File Offset: 0x00422E16
		// (set) Token: 0x0600A892 RID: 43154 RVA: 0x00424C1E File Offset: 0x00422E1E
		public TwitchViewerData ViewerData { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x170012C2 RID: 4802
		// (get) Token: 0x0600A893 RID: 43155 RVA: 0x00424C27 File Offset: 0x00422E27
		// (set) Token: 0x0600A894 RID: 43156 RVA: 0x00424C2F File Offset: 0x00422E2F
		public string BroadcasterType
		{
			get
			{
				return this.broadcasterType;
			}
			set
			{
				this.broadcasterType = value;
				this.UIDirty = true;
				if (this.CommandsChanged != null)
				{
					this.CommandsChanged();
				}
			}
		}

		// Token: 0x170012C3 RID: 4803
		// (get) Token: 0x0600A895 RID: 43157 RVA: 0x00424C52 File Offset: 0x00422E52
		// (set) Token: 0x0600A896 RID: 43158 RVA: 0x00424C5C File Offset: 0x00422E5C
		public TwitchManager.InitStates InitState
		{
			get
			{
				return this.initState;
			}
			[PublicizedFrom(EAccessModifier.Private)]
			set
			{
				if (this.initState != value)
				{
					TwitchManager.InitStates oldState = this.initState;
					this.initState = value;
					if (this.ConnectionStateChanged != null)
					{
						this.ConnectionStateChanged(oldState, this.initState);
					}
				}
			}
		}

		// Token: 0x170012C4 RID: 4804
		// (get) Token: 0x0600A897 RID: 43159 RVA: 0x00424C9C File Offset: 0x00422E9C
		public string StateText
		{
			get
			{
				switch (this.initState)
				{
				case TwitchManager.InitStates.Setup:
				case TwitchManager.InitStates.WaitingForOAuth:
				case TwitchManager.InitStates.Authenticating:
				case TwitchManager.InitStates.Authenticated:
					return Localization.Get("xuiTwitchStatus_Connecting", false);
				case TwitchManager.InitStates.WaitingForPermission:
					return Localization.Get("xuiTwitchStatus_RequestPermission", false);
				case TwitchManager.InitStates.PermissionDenied:
					return Localization.Get("xuiTwitchStatus_PermissionDenied", false);
				case TwitchManager.InitStates.Ready:
					return string.Format(Localization.Get("xuiTwitchStatus_Connected", false), this.Authentication.userName);
				case TwitchManager.InitStates.ExtensionNotInstalled:
					return Localization.Get("xuiTwitchStatus_ExtensionDenied", false);
				case TwitchManager.InitStates.Failed:
					return Localization.Get("xuiTwitchStatus_ConnectionFailed", false);
				}
				return "";
			}
		}

		// Token: 0x170012C5 RID: 4805
		// (get) Token: 0x0600A898 RID: 43160 RVA: 0x00424D41 File Offset: 0x00422F41
		public bool IsReady
		{
			get
			{
				return this.initState == TwitchManager.InitStates.Ready;
			}
		}

		// Token: 0x170012C6 RID: 4806
		// (get) Token: 0x0600A899 RID: 43161 RVA: 0x00424D4C File Offset: 0x00422F4C
		public bool IsVoting
		{
			get
			{
				return this.initState == TwitchManager.InitStates.Ready && this.VotingManager.VotingIsActive;
			}
		}

		// Token: 0x170012C7 RID: 4807
		// (get) Token: 0x0600A89A RID: 43162 RVA: 0x00424D64 File Offset: 0x00422F64
		public bool IsBossHordeActive
		{
			get
			{
				return this.initState == TwitchManager.InitStates.Ready && (this.VotingManager.CurrentVoteState == TwitchVotingManager.VoteStateTypes.EventActive || this.VotingManager.CurrentVoteState == TwitchVotingManager.VoteStateTypes.WaitingForActive);
			}
		}

		// Token: 0x170012C8 RID: 4808
		// (get) Token: 0x0600A89B RID: 43163 RVA: 0x00424D8F File Offset: 0x00422F8F
		public bool ReadyForVote
		{
			get
			{
				return this.actionSpawnLiveList.Count == 0;
			}
		}

		// Token: 0x170012C9 RID: 4809
		// (get) Token: 0x0600A89C RID: 43164 RVA: 0x00424D9F File Offset: 0x00422F9F
		// (set) Token: 0x0600A89D RID: 43165 RVA: 0x00424DA7 File Offset: 0x00422FA7
		public bool IsSafe { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x140000FF RID: 255
		// (add) Token: 0x0600A89E RID: 43166 RVA: 0x00424DB0 File Offset: 0x00422FB0
		// (remove) Token: 0x0600A89F RID: 43167 RVA: 0x00424DE8 File Offset: 0x00422FE8
		public event OnTwitchConnectionStateChange ConnectionStateChanged;

		// Token: 0x14000100 RID: 256
		// (add) Token: 0x0600A8A0 RID: 43168 RVA: 0x00424E20 File Offset: 0x00423020
		// (remove) Token: 0x0600A8A1 RID: 43169 RVA: 0x00424E58 File Offset: 0x00423058
		public event OnCommandsChanged CommandsChanged;

		// Token: 0x14000101 RID: 257
		// (add) Token: 0x0600A8A2 RID: 43170 RVA: 0x00424E90 File Offset: 0x00423090
		// (remove) Token: 0x0600A8A3 RID: 43171 RVA: 0x00424EC8 File Offset: 0x004230C8
		public event OnHistoryAdded ActionHistoryAdded;

		// Token: 0x14000102 RID: 258
		// (add) Token: 0x0600A8A4 RID: 43172 RVA: 0x00424F00 File Offset: 0x00423100
		// (remove) Token: 0x0600A8A5 RID: 43173 RVA: 0x00424F38 File Offset: 0x00423138
		public event OnHistoryAdded VoteHistoryAdded;

		// Token: 0x14000103 RID: 259
		// (add) Token: 0x0600A8A6 RID: 43174 RVA: 0x00424F70 File Offset: 0x00423170
		// (remove) Token: 0x0600A8A7 RID: 43175 RVA: 0x00424FA8 File Offset: 0x004231A8
		public event OnHistoryAdded EventHistoryAdded;

		// Token: 0x170012CA RID: 4810
		// (get) Token: 0x0600A8A8 RID: 43176 RVA: 0x00424FDD File Offset: 0x004231DD
		public bool HasCustomEvents
		{
			get
			{
				return this.EventPresets.Count > 0;
			}
		}

		// Token: 0x0600A8A9 RID: 43177 RVA: 0x00424FF0 File Offset: 0x004231F0
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetupLocalization()
		{
			this.chatOutput_ActivatedAction = Localization.Get("TwitchChat_ActivatedAction", false);
			this.chatOutput_ActivatedBitAction = Localization.Get("TwitchChat_ActivatedBitAction", false);
			this.chatOutput_BitCredits = Localization.Get("TwitchChat_BitCredits", false);
			this.chatOutput_BitEvent = Localization.Get("TwitchChat_BitEvent", false);
			this.chatOutput_BitPotBalance = Localization.Get("TwitchChat_BitPotBalance", false);
			this.chatOutput_ChannelPointEvent = Localization.Get("TwitchChat_ChannelPointEvent", false);
			this.chatOutput_CharityEvent = Localization.Get("TwitchChat_CharityEvent", false);
			this.chatOutput_Commands = Localization.Get("TwitchChat_Commands", false);
			this.chatOutput_CooldownComplete = Localization.Get("TwitchChat_CooldownComplete", false);
			this.chatOutput_CooldownStarted = Localization.Get("TwitchChat_CooldownStarted", false);
			this.chatOutput_CooldownTime = Localization.Get("TwitchChat_CooldownTime", false);
			this.chatOutput_CreatorGoalEvent = Localization.Get("TwitchChat_CreatorGoalEvent", false);
			this.chatOutput_DonateBits = Localization.Get("TwitchChat_DonateBits", false);
			this.chatOutput_DonateCharity = Localization.Get("TwitchChat_DonateCharity", false);
			this.chatOutput_Gamestage = Localization.Get("TwitchChat_Gamestage", false);
			this.chatOutput_GiftSubEvent = Localization.Get("TwitchChat_GiftedSubEvent", false);
			this.chatOutput_GiftSubs = Localization.Get("TwitchChat_GiftedSubs", false);
			this.chatOutput_HypeTrainEvent = Localization.Get("TwitchChat_HypeTrainEvent", false);
			this.chatOutput_KilledParty = Localization.Get("TwitchChat_KilledParty", false);
			this.chatOutput_KilledStreamer = Localization.Get("TwitchChat_KilledStreamer", false);
			this.chatOutput_KilledByBits = Localization.Get("TwitchChat_KilledByBits", false);
			this.chatOutput_KilledByHypeTrain = Localization.Get("TwitchChat_KilledByHypeTrain", false);
			this.chatOutput_KilledByVote = Localization.Get("TwitchChat_KilledByVote", false);
			this.chatOutput_NewActions = Localization.Get("TwitchChat_NewActions", false);
			this.chatOutput_PimpPotBalance = Localization.Get("TwitchChat_PimpPotBalance", false);
			this.chatOutput_PointsWithSpecial = Localization.Get("TwitchChat_PointsWithSpecial", false);
			this.chatOutput_PointsWithoutSpecial = Localization.Get("TwitchChat_PointsWithoutSpecial", false);
			this.chatOutput_QueuedBitAction = Localization.Get("TwitchChat_QueuedBitAction", false);
			this.chatOutput_RaidEvent = Localization.Get("TwitchChat_RaidEvent", false);
			this.chatOutput_RaidPoints = Localization.Get("TwitchChat_RaidPoints", false);
			this.chatOutput_SubEvent = Localization.Get("TwitchChat_SubEvent", false);
			this.chatOutput_Subscribed = Localization.Get("TwitchChat_Subscribed", false);
			this.ingameOutput_ActivatedAction = Localization.Get("TwitchInGame_ActivatedAction", false);
			this.ingameOutput_BitRespawns = Localization.Get("TwitchInGame_BitRespawns", false);
			this.ingameOutput_DonateBits = Localization.Get("TwitchInGame_DonateBits", false);
			this.ingameOutput_DonateCharity = Localization.Get("TwitchInGame_DonateCharity", false);
			this.ingameOutput_GiftSubs = Localization.Get("TwitchInGame_GiftedSubs", false);
			this.ingameOutput_KilledParty = Localization.Get("TwitchInGame_KilledParty", false);
			this.ingameOutput_KilledStreamer = Localization.Get("TwitchInGame_KilledStreamer", false);
			this.ingameOutput_KilledByBits = Localization.Get("TwitchInGame_KilledByBits", false);
			this.ingameOutput_KilledByHypeTrain = Localization.Get("TwitchInGame_KilledByHypeTrain", false);
			this.ingameOutput_KilledByVote = Localization.Get("TwitchInGame_KilledByVote", false);
			this.ingameOutput_RaidPoints = Localization.Get("TwitchInGame_RaidPoints", false);
			this.ingameOutput_RefundedAction = Localization.Get("TwitchInGame_RefundedAction", false);
			this.ingameOutput_Subscribed = Localization.Get("TwitchInGame_Subscribed", false);
			this.ingameDeathScreen_Message = Localization.Get("TwitchDeathMessage", false);
			this.ingameBitsDeathScreen_Message = Localization.Get("TwitchBitsDeathMessage", false);
			this.ingameHypeTrainDeathScreen_Message = Localization.Get("TwitchHypeTrainDeathMessage", false);
			this.ingameVoteDeathScreen_Message = Localization.Get("TwitchVoteDeathMessage", false);
			this.subPointDisplay = Localization.Get("xuiOptionsTwitchSubPointDisplay", false);
			this.ViewerData.SetupLocalization();
			this.VotingManager.SetupLocalization();
			TwitchManager.LeaderboardStats.SetupLocalization();
		}

		// Token: 0x0600A8AA RID: 43178 RVA: 0x00425370 File Offset: 0x00423570
		public void SetupClient(string twitchChannel, string password)
		{
			this.ircClient = new TwitchIRCClient("irc.twitch.tv", 6667, twitchChannel, password);
			if (this.EventSub == null)
			{
				this.EventSub = new EventSubClient(this.Authentication.userID, this.Authentication.oauth.Substring(6), TwitchAuthentication.client_id);
			}
		}

		// Token: 0x0600A8AB RID: 43179 RVA: 0x004253C8 File Offset: 0x004235C8
		public void IntegrationTypeChanged()
		{
			if (this.IsReady && this.extensionManager == null)
			{
				this.extensionManager = new ExtensionManager();
				this.extensionManager.Init();
			}
		}

		// Token: 0x0600A8AC RID: 43180 RVA: 0x004253F0 File Offset: 0x004235F0
		public void CleanupData()
		{
			BaseTwitchCommand.ClearCommandPermissionOverrides();
			this.VotingManager.CleanupData();
			this.CooldownPresets.Clear();
			this.tipTitleList.Clear();
			this.tipDescriptionList.Clear();
			this.ActionPresets.Clear();
			this.VotePresets.Clear();
			this.CurrentActionPreset = null;
			this.CurrentVotePreset = null;
			this.CleanupEventData();
		}

		// Token: 0x0600A8AD RID: 43181 RVA: 0x00425458 File Offset: 0x00423658
		[PublicizedFrom(EAccessModifier.Private)]
		public void RemoveChannelPointRedeems()
		{
			if (this.CurrentEventPreset != null)
			{
				this.CurrentEventPreset.RemoveChannelPointRedemptions(null);
			}
		}

		// Token: 0x0600A8AE RID: 43182 RVA: 0x0042546E File Offset: 0x0042366E
		public void CleanupEventData()
		{
			this.RemoveChannelPointRedeems();
			this.CurrentEventPreset = null;
			this.EventPresets.Clear();
		}

		// Token: 0x0600A8AF RID: 43183 RVA: 0x00425488 File Offset: 0x00423688
		[PublicizedFrom(EAccessModifier.Private)]
		public void Disconnect()
		{
			this.RemoveChannelPointRedeems();
			if (this.EventSub != null)
			{
				this.EventSub.Disconnect();
				this.EventSub.Cleanup();
				this.EventSub = null;
			}
			if (this.ircClient != null)
			{
				this.ircClient.Disconnect();
				this.ircClient = null;
			}
			if (this.extensionManager != null)
			{
				this.extensionManager.Cleanup();
				this.extensionManager = null;
			}
			this.Authentication = null;
			if (this.LocalPlayer != null && this.LocalPlayer.PlayerUI != null && this.LocalPlayer.PlayerUI.windowManager.IsWindowOpen("twitch"))
			{
				this.LocalPlayer.PlayerUI.windowManager.Close("twitch");
			}
		}

		// Token: 0x0600A8B0 RID: 43184 RVA: 0x00425552 File Offset: 0x00423752
		public void AddRandomGroup(string name, int randomCount)
		{
			if (!this.randomGroups.ContainsKey(name))
			{
				this.randomGroups.Add(name, new TwitchRandomActionGroup
				{
					Name = name,
					RandomCount = randomCount
				});
			}
		}

		// Token: 0x0600A8B1 RID: 43185 RVA: 0x00425584 File Offset: 0x00423784
		[PublicizedFrom(EAccessModifier.Private)]
		public void ResetDailyCommands(int currentDay, int lastGameStage = -1)
		{
			this.randomKeys.Clear();
			foreach (string key in this.AvailableCommands.Keys)
			{
				TwitchAction twitchAction = this.AvailableCommands[key];
				if (twitchAction.IsInPreset(this.CurrentActionPreset))
				{
					if (twitchAction.RandomDaily)
					{
						if (!this.randomKeys.ContainsKey(twitchAction.RandomGroup))
						{
							this.randomKeys.Add(twitchAction.RandomGroup, new List<TwitchAction>());
						}
						this.randomKeys[twitchAction.RandomGroup].Add(twitchAction);
					}
					else if (twitchAction.SingleDayUse)
					{
						twitchAction.AllowedDay = currentDay;
					}
				}
			}
			foreach (string key2 in this.randomKeys.Keys)
			{
				int num = 1;
				if (this.randomGroups.ContainsKey(key2))
				{
					num = this.randomGroups[key2].RandomCount;
				}
				List<TwitchAction> list = this.randomKeys[key2];
				if (lastGameStage != -1)
				{
					bool flag = false;
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].StartGameStage > lastGameStage)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						continue;
					}
				}
				for (int j = 0; j < list.Count; j++)
				{
					int index = UnityEngine.Random.Range(0, list.Count);
					int index2 = UnityEngine.Random.Range(0, list.Count);
					TwitchAction value = list[index];
					list[index] = list[index2];
					list[index2] = value;
				}
				for (int k = 0; k < list.Count; k++)
				{
					list[k].AllowedDay = ((k < num) ? currentDay : -1);
				}
			}
			if (this.CommandsChanged != null)
			{
				this.CommandsChanged();
			}
		}

		// Token: 0x0600A8B2 RID: 43186 RVA: 0x004257C4 File Offset: 0x004239C4
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetupTwitchCommands()
		{
			this.TwitchCommandList.Clear();
			this.TwitchCommandList.Add(new TwitchCommandAddBitCredit());
			this.TwitchCommandList.Add(new TwitchCommandAddPoints());
			this.TwitchCommandList.Add(new TwitchCommandAddSpecialPoints());
			this.TwitchCommandList.Add(new TwitchCommandCheckCredit());
			this.TwitchCommandList.Add(new TwitchCommandCheckPoints());
			this.TwitchCommandList.Add(new TwitchCommandCommands());
			this.TwitchCommandList.Add(new TwitchCommandDebug());
			this.TwitchCommandList.Add(new TwitchCommandDisableCommand());
			this.TwitchCommandList.Add(new TwitchCommandGamestage());
			this.TwitchCommandList.Add(new TwitchCommandPauseCommand());
			this.TwitchCommandList.Add(new TwitchCommandUnpauseCommand());
			this.TwitchCommandList.Add(new TwitchCommandRemoveViewer());
			this.TwitchCommandList.Add(new TwitchCommandResetCooldowns());
			this.TwitchCommandList.Add(new TwitchCommandSetBitPot());
			this.TwitchCommandList.Add(new TwitchCommandSetCooldown());
			this.TwitchCommandList.Add(new TwitchCommandSetPot());
			this.TwitchCommandList.Add(new TwitchCommandTeleportBackpack());
			if (this.CurrentEventPreset.HasBitEvents && this.AllowBitEvents)
			{
				this.TwitchCommandList.Add(new TwitchCommandRedeemBits());
			}
			if (this.CurrentEventPreset.HasSubEvents && this.AllowSubEvents)
			{
				this.TwitchCommandList.Add(new TwitchCommandRedeemSub());
			}
			if (this.CurrentEventPreset.HasGiftSubEvents && this.AllowGiftSubEvents)
			{
				this.TwitchCommandList.Add(new TwitchCommandRedeemGiftSub());
			}
			if (this.CurrentEventPreset.HasRaidEvents && this.AllowRaidEvents)
			{
				this.TwitchCommandList.Add(new TwitchCommandRedeemRaid());
			}
			if (this.CurrentEventPreset.HasCharityEvents && this.AllowCharityEvents)
			{
				this.TwitchCommandList.Add(new TwitchCommandRedeemCharity());
			}
			if (this.CurrentEventPreset.HasHypeTrainEvents && this.AllowHypeTrainEvents)
			{
				this.TwitchCommandList.Add(new TwitchCommandRedeemHypeTrain());
			}
			if (this.CurrentEventPreset.HasCreatorGoalEvents && this.AllowCreatorGoalEvents)
			{
				this.TwitchCommandList.Add(new TwitchCommandRedeemCreatorGoal());
			}
			this.TwitchCommandList.Add(new TwitchCommandUseProgression());
		}

		// Token: 0x0600A8B3 RID: 43187 RVA: 0x00425A00 File Offset: 0x00423C00
		public void StartTwitchIntegration()
		{
			this.updateTime = 60f;
			try
			{
				if (this.Authentication == null)
				{
					this.Authentication = new TwitchAuthentication();
				}
				this.Authentication.StopListener();
				this.Authentication.GetToken();
			}
			catch (Exception ex)
			{
				Log.Out("Twitch integration failed to start with message " + ex.Message);
				this.updateTime = 5f;
			}
			this.InitialCooldownSet = false;
			this.InitState = TwitchManager.InitStates.WaitingForOAuth;
		}

		// Token: 0x0600A8B4 RID: 43188 RVA: 0x00425A84 File Offset: 0x00423C84
		public void StopTwitchIntegration(TwitchManager.InitStates initState = TwitchManager.InitStates.None)
		{
			this.resetClientAttempts = 0;
			this.Disconnect();
			this.TwitchDisconnectPartyUpdate();
			this.ClearEventHandlers();
			if (this.LocalPlayer != null)
			{
				this.LocalPlayer.TwitchEnabled = false;
				this.LocalPlayer.TwitchActionsEnabled = EntityPlayer.TwitchActionsStates.Enabled;
			}
			if (this.Authentication != null)
			{
				this.Authentication.StopListener();
			}
			this.InitState = initState;
		}

		// Token: 0x0600A8B5 RID: 43189 RVA: 0x00425AEA File Offset: 0x00423CEA
		public void WaitForOAuth()
		{
			this.updateTime = 10f;
			this.InitState = TwitchManager.InitStates.WaitingForOAuth;
		}

		// Token: 0x0600A8B6 RID: 43190 RVA: 0x00425AFE File Offset: 0x00423CFE
		public void WaitForPermission()
		{
			this.updateTime = 10f;
			this.InitState = TwitchManager.InitStates.WaitingForPermission;
		}

		// Token: 0x0600A8B7 RID: 43191 RVA: 0x00425B12 File Offset: 0x00423D12
		public void DeniedPermission()
		{
			this.InitState = TwitchManager.InitStates.PermissionDenied;
		}

		// Token: 0x0600A8B8 RID: 43192 RVA: 0x00425B1C File Offset: 0x00423D1C
		[PublicizedFrom(EAccessModifier.Private)]
		public void ClearEventHandlers()
		{
			GameEventManager gameEventManager = GameEventManager.Current;
			gameEventManager.GameEntitySpawned -= this.Current_GameEntitySpawned;
			gameEventManager.GameEntityDespawned -= this.Current_GameEntityDespawned;
			gameEventManager.GameEntityKilled -= this.Current_GameEntityKilled;
			gameEventManager.GameBlocksAdded -= this.Current_GameBlocksAdded;
			gameEventManager.GameBlocksRemoved -= this.Current_GameBlocksRemoved;
			gameEventManager.GameBlockRemoved -= this.Current_GameBlockRemoved;
			gameEventManager.GameEventApproved -= this.Current_GameEventApproved;
			gameEventManager.TwitchPartyGameEventApproved -= this.Current_TwitchPartyGameEventApproved;
			gameEventManager.TwitchRefundNeeded -= this.Current_TwitchRefundNeeded;
			gameEventManager.GameEventDenied -= this.Current_GameEventDenied;
			gameEventManager.GameEventCompleted -= this.Current_GameEventCompleted;
			if (this.LocalPlayer != null)
			{
				this.LocalPlayer.PartyLeave -= this.LocalPlayer_PartyLeave;
				this.LocalPlayer.PartyJoined -= this.LocalPlayer_PartyJoined;
				this.LocalPlayer.PartyChanged -= this.LocalPlayer_PartyChanged;
				if (this.LocalPlayer.Party != null)
				{
					this.LocalPlayer.Party.PartyMemberAdded -= this.Party_PartyMemberAdded;
					this.LocalPlayer.Party.PartyMemberRemoved -= this.Party_PartyMemberRemoved;
				}
			}
		}

		// Token: 0x0600A8B9 RID: 43193 RVA: 0x00425C90 File Offset: 0x00423E90
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_TwitchPartyGameEventApproved(string gameEventID, int targetEntityID, string extraData, string tag)
		{
			foreach (TwitchAction twitchAction in TwitchActionManager.TwitchActions.Values)
			{
				if (twitchAction.IsInPreset(this.CurrentActionPreset) && twitchAction.EventName == gameEventID)
				{
					this.AddCooldownForAction(twitchAction);
					break;
				}
			}
		}

		// Token: 0x0600A8BA RID: 43194 RVA: 0x00425D08 File Offset: 0x00423F08
		public void Update(float deltaTime)
		{
			GameManager gameManager = GameManager.Instance;
			if (gameManager.World == null || gameManager.World.Players == null || gameManager.World.Players.Count == 0)
			{
				return;
			}
			this.CurrentUnityTime = Time.time;
			switch (this.InitState)
			{
			case TwitchManager.InitStates.Setup:
			{
				GameEventManager gameEventManager = GameEventManager.Current;
				gameEventManager.GameEventAccessApproved -= this.Current_GameEventAccessApproved;
				gameEventManager.GameEventAccessApproved += this.Current_GameEventAccessApproved;
				this.SetupLocalization();
				if (!this.isLoaded)
				{
					this.isLoaded = true;
					this.LoadViewerData();
				}
				if (!this.LoadLatestMainViewerData() && !this.LoadMainViewerData())
				{
					this.LoadSpecialViewerData();
				}
				this.InitState = TwitchManager.InitStates.None;
				break;
			}
			case TwitchManager.InitStates.WaitingForPermission:
				this.updateTime -= deltaTime;
				if (this.updateTime <= 0f)
				{
					this.StopTwitchIntegration(TwitchManager.InitStates.None);
					Log.Warning("Twitch: login failed in " + this.InitState.ToString() + " state");
					this.InitState = TwitchManager.InitStates.Failed;
					return;
				}
				break;
			case TwitchManager.InitStates.WaitingForOAuth:
				this.updateTime -= deltaTime;
				if (this.updateTime <= 0f)
				{
					this.StopTwitchIntegration(TwitchManager.InitStates.None);
					Log.Warning("Twitch: Login failed in " + this.InitState.ToString() + " state");
					this.InitState = TwitchManager.InitStates.Failed;
					return;
				}
				if (this.Authentication.oauth != "" && this.Authentication.userName != "" && this.Authentication.userID != "")
				{
					this.SetupClient(this.Authentication.userName, this.Authentication.oauth);
					this.EventSub.OnEventReceived -= this.EventSubMessageReceived;
					this.EventSub.OnEventReceived += this.EventSubMessageReceived;
					this.EventSub.Connect();
					this.updateTime = 3f;
					Log.Out("retrieved oauth. Waiting for IRC to post auth...");
					this.InitState = TwitchManager.InitStates.Authenticating;
				}
				break;
			case TwitchManager.InitStates.Authenticating:
				this.updateTime -= deltaTime;
				if (this.updateTime <= 0f)
				{
					if (this.Authentication.oauth != "" && this.Authentication.userName != "" && this.Authentication.userID != "")
					{
						int num = this.resetClientAttempts;
						this.resetClientAttempts = num + 1;
						if (num < 5)
						{
							this.SetupClient(this.Authentication.userName, this.Authentication.oauth);
							this.updateTime = 2.5f;
							Log.Out("attempting to reset client...");
							break;
						}
					}
					Log.Warning("Twitch: Login failed in " + this.InitState.ToString() + " state");
					this.StopTwitchIntegration(TwitchManager.InitStates.Failed);
					this.InitState = TwitchManager.InitStates.Failed;
					return;
				}
				break;
			case TwitchManager.InitStates.Authenticated:
			{
				this.ClearEventHandlers();
				GameEventManager gameEventManager2 = GameEventManager.Current;
				gameEventManager2.GameEntitySpawned += this.Current_GameEntitySpawned;
				gameEventManager2.GameEntityDespawned += this.Current_GameEntityDespawned;
				gameEventManager2.GameEntityKilled += this.Current_GameEntityKilled;
				gameEventManager2.GameBlocksAdded += this.Current_GameBlocksAdded;
				gameEventManager2.GameBlockRemoved += this.Current_GameBlockRemoved;
				gameEventManager2.GameBlocksRemoved += this.Current_GameBlocksRemoved;
				gameEventManager2.GameEventApproved += this.Current_GameEventApproved;
				gameEventManager2.TwitchPartyGameEventApproved += this.Current_TwitchPartyGameEventApproved;
				gameEventManager2.TwitchRefundNeeded += this.Current_TwitchRefundNeeded;
				gameEventManager2.GameEventDenied += this.Current_GameEventDenied;
				gameEventManager2.GameEventCompleted += this.Current_GameEventCompleted;
				this.world = gameManager.World;
				if (this.extensionManager == null)
				{
					this.extensionManager = new ExtensionManager();
					this.extensionManager.Init();
				}
				this.InitState = TwitchManager.InitStates.Ready;
				QuestEventManager.Current.TwitchEventReceived(TwitchObjectiveTypes.Enabled, "");
				break;
			}
			case TwitchManager.InitStates.CheckingForExtension:
				if (!this.AllowActions)
				{
					this.InitState = TwitchManager.InitStates.Authenticated;
				}
				else if (!this.checkingExtensionInstalled)
				{
					this.checkingExtensionInstalled = true;
					ExtensionManager.CheckExtensionInstalled(delegate(bool IsInstalled)
					{
						this.checkingExtensionInstalled = false;
						if (!IsInstalled)
						{
							XUiC_MessageBoxWindowGroup.ShowMessageBox(this.LocalPlayerXUi, Localization.Get("xuiTwitchPopup_ExtensionNeededHeader", false), Localization.Get("xuiTwitchPopup_ExtensionNeeded", false), XUiC_MessageBoxWindowGroup.MessageBoxTypes.Ok, null, null, true, true, true);
							Application.OpenURL("https://dashboard.twitch.tv/extensions/k6ji189bf7i4ge8il4iczzw7kpgmjt");
						}
						this.InitState = TwitchManager.InitStates.Authenticated;
					});
				}
				break;
			case TwitchManager.InitStates.Ready:
				if (this.LocalPlayer == null)
				{
					this.SetupTwitchCommands();
					this.LocalPlayer = (XUiM_Player.GetPlayer() as EntityPlayerLocal);
					this.RefreshPartyInfo();
					this.HighestGameStage = this.LocalPlayer.unModifiedGameStage;
					this.ActionMessages.Clear();
					this.GetCooldownMax();
					this.SetCooldown((float)this.CurrentCooldownPreset.NextCooldownTime, TwitchManager.CooldownTypes.Startup, false, false);
					this.LocalPlayer.PartyLeave += this.LocalPlayer_PartyLeave;
					this.LocalPlayer.PartyJoined += this.LocalPlayer_PartyJoined;
					this.LocalPlayer.PartyChanged += this.LocalPlayer_PartyChanged;
					if (this.LocalPlayer.Party != null)
					{
						this.LocalPlayer.Party.PartyMemberAdded += this.Party_PartyMemberAdded;
						this.LocalPlayer.Party.PartyMemberRemoved += this.Party_PartyMemberRemoved;
					}
					if (!this.InitialCooldownSet)
					{
						if (this.CurrentCooldownPreset.StartCooldownTime > 0)
						{
							this.SetCooldown(100000f, TwitchManager.CooldownTypes.Startup, false, true);
						}
						if (this.CurrentCooldownPreset.CooldownType != CooldownPreset.CooldownTypes.Fill)
						{
							this.SetCooldown(0f, TwitchManager.CooldownTypes.None, false, false);
						}
						this.CurrentActionPreset.HandleCooldowns();
						this.InitialCooldownSet = true;
					}
				}
				this.LocalPlayer.TwitchEnabled = true;
				this.LocalPlayerInLandClaim = GameManager.Instance.World.GetLandClaimOwnerInParty(this.LocalPlayer, this.LocalPlayer.persistentPlayerData);
				if (!this.ircClient.IsConnected)
				{
					Log.Out("Reached 'Ready' but waiting for IRC to post auth message...");
					this.ircClient.Reconnect();
					this.InitState = TwitchManager.InitStates.Authenticating;
					this.updateTime = 30f;
				}
				TwitchManager.LeaderboardStats.UpdateStats(deltaTime);
				if (this.resetCommandsNeeded)
				{
					this.ResetCommands();
				}
				if (!XUi.InGameMenuOpen && this.AllowActions && this.ExtensionCheckTime < 0f)
				{
					this.ExtensionCheckTime = 30f;
					ExtensionManager.CheckExtensionInstalled(delegate(bool IsInstalled)
					{
						if (IsInstalled)
						{
							this.extensionActiveCheckFailures = 0;
							return;
						}
						if (this.extensionActiveCheckFailures < 3)
						{
							this.extensionActiveCheckFailures++;
							return;
						}
						this.extensionActiveCheckFailures = 0;
						LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.LocalPlayer);
						this.ircClient.SendChannelMessage(Localization.Get("TwitchChat_ExtensionNotInstalled", false), false);
						XUiC_ChatOutput.AddMessage(uiforPlayer.xui, EnumGameMessages.PlainTextLocal, Localization.Get("TwitchChat_ExtensionNotInstalled", false), EChatType.Global, EChatDirection.Inbound, -1, null, null, EMessageSender.None, GeneratedTextManager.TextFilteringMode.None, GeneratedTextManager.BbCodeSupportMode.Supported);
						this.StopTwitchIntegration(TwitchManager.InitStates.None);
						this.InitState = TwitchManager.InitStates.ExtensionNotInstalled;
						Application.OpenURL("https://dashboard.twitch.tv/extensions/k6ji189bf7i4ge8il4iczzw7kpgmjt");
					});
				}
				this.ExtensionCheckTime -= deltaTime;
				if (this.LocalPlayer.Buffs.HasBuff("twitch_extensionneeded"))
				{
					this.updateTime -= deltaTime;
					if (this.updateTime <= 0f)
					{
						ExtensionManager.CheckExtensionInstalled(delegate(bool IsInstalled)
						{
							if (IsInstalled)
							{
								this.LocalPlayer.Buffs.RemoveBuff("twitch_extensionneeded", true);
							}
						});
						this.updateTime = 5f;
					}
				}
				break;
			}
			if (this.extensionManager != null)
			{
				if (this.extensionManager.HasCommand())
				{
					ExtensionAction command = this.extensionManager.GetCommand();
					int userId = int.Parse(command.username);
					string command2 = command.command;
					bool isRerun = false;
					int creditUsed = command.creditUsed;
					ExtensionBitAction extensionBitAction = command as ExtensionBitAction;
					this.HandleExtensionMessage(userId, command2, isRerun, creditUsed, (extensionBitAction != null) ? extensionBitAction.cost : 0);
				}
				this.extensionManager.Update();
			}
			if (this.ircClient != null)
			{
				this.ircClient.Update(deltaTime);
				if (this.ircClient.AvailableMessage())
				{
					this.HandleMessage(this.ircClient.ReadMessage());
				}
				this.ViewerData.Update(deltaTime);
				if (this.LocalPlayer == null)
				{
					return;
				}
				bool flag = false;
				for (int i = this.LiveActionEntries.Count - 1; i >= 0; i--)
				{
					if (this.LiveActionEntries[i].ReadyForRemove)
					{
						this.LiveActionEntries.RemoveAt(i);
					}
					else if (this.LiveActionEntries[i].Action.CooldownBlocked)
					{
						flag = true;
					}
				}
				for (int j = this.actionSpawnLiveList.Count - 1; j >= 0; j--)
				{
					if (this.actionSpawnLiveList[j].SpawnedEntity == null)
					{
						this.actionSpawnLiveList.RemoveAt(j);
					}
				}
				for (int k = this.LiveEvents.Count - 1; k >= 0; k--)
				{
					if (this.LiveEvents[k].ReadyForRemove)
					{
						this.LiveEvents.RemoveAt(k);
					}
				}
				if (this.LocalPlayer.IsAlive() && this.CooldownTime > 0f && this.TwitchActive)
				{
					if (this.CooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting && this.actionSpawnLiveList.Count == 0 && !flag)
					{
						this.SetCooldown(this.CooldownTime, TwitchManager.CooldownTypes.MaxReached, false, true);
					}
					if (this.CooldownType == TwitchManager.CooldownTypes.MaxReached || this.CooldownType == TwitchManager.CooldownTypes.Time || this.CooldownType == TwitchManager.CooldownTypes.Startup || this.CooldownType == TwitchManager.CooldownTypes.SafeCooldownExit)
					{
						float cooldownTime = this.CooldownTime;
						this.CooldownTime -= Time.deltaTime;
						if (cooldownTime >= 15f && this.CooldownTime < 15f && this.CooldownTime > 0f && this.CooldownType != TwitchManager.CooldownTypes.SafeCooldownExit)
						{
							this.LocalPlayer.TwitchActionsEnabled = EntityPlayer.TwitchActionsStates.TempDisabledEnding;
						}
					}
					if (this.CooldownTime <= 0f)
					{
						if (this.CooldownType == TwitchManager.CooldownTypes.SafeCooldownExit)
						{
							this.HandleEndCooldownStateChanging();
						}
						else
						{
							this.HandleEndCooldown();
						}
						this.VotingManager.VoteStartDelayTimeRemaining = 10f;
					}
				}
				for (int l = this.liveList.Count - 1; l >= 0; l--)
				{
					if (this.liveList[l].SpawnedEntity == null)
					{
						this.liveList[l].SpawnedEntity = this.world.GetEntity(this.liveList[l].SpawnedEntityID);
						this.liveList[l].SpawnedEntity == null;
					}
				}
				for (int m = this.recentlyDeadList.Count - 1; m >= 0; m--)
				{
					this.recentlyDeadList[m].TimeRemaining -= deltaTime;
					if (this.recentlyDeadList[m].TimeRemaining <= 0f)
					{
						this.recentlyDeadList.RemoveAt(m);
					}
				}
				for (int n = this.liveBlockList.Count - 1; n >= 0; n--)
				{
					TwitchSpawnedBlocksEntry twitchSpawnedBlocksEntry = this.liveBlockList[n];
					if (twitchSpawnedBlocksEntry.TimeRemaining > 0f)
					{
						twitchSpawnedBlocksEntry.TimeRemaining -= deltaTime;
						if (twitchSpawnedBlocksEntry.TimeRemaining <= 0f)
						{
							this.liveBlockList.RemoveAt(n);
						}
					}
				}
				int num2 = GameUtils.WorldTimeToDays(this.world.worldTime);
				if (num2 != this.lastGameDay)
				{
					this.SetupAvailableCommands();
					this.ResetDailyCommands(num2, -1);
					this.HandleCooldownActionLocking();
					this.lastGameDay = num2;
				}
				if (this.CooldownType != TwitchManager.CooldownTypes.Startup && this.TwitchActive && !gameManager.IsPaused() && this.InitState == TwitchManager.InitStates.Ready)
				{
					this.VotingManager.Update(deltaTime);
				}
				this.HandleEventQueue();
				if (this.LocalPlayer.IsAlive() && this.CooldownType != TwitchManager.CooldownTypes.Time)
				{
					int num3 = 0;
					while (num3 < this.QueuedActionEntries.Count)
					{
						if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
						{
							if (!this.QueuedActionEntries[num3].IsSent)
							{
								SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageGameEventRequest>().Setup(this.QueuedActionEntries[num3].Action.EventName, this.QueuedActionEntries[num3].Target.entityId, true, Vector3.zero, this.QueuedActionEntries[num3].UserName, "action", this.AllowCrateSharing, true, ""), false);
								this.QueuedActionEntries[num3].IsSent = true;
								break;
							}
							num3++;
						}
						else
						{
							if (GameEventManager.Current.HandleAction(this.QueuedActionEntries[num3].Action.EventName, this.LocalPlayer, this.QueuedActionEntries[num3].Target, true, this.QueuedActionEntries[num3].UserName, "action", this.AllowCrateSharing, true, "", null))
							{
								if (this.LocalPlayer.Party != null)
								{
									for (int num4 = 0; num4 < this.LocalPlayer.Party.MemberList.Count; num4++)
									{
										EntityPlayer entityPlayer = this.LocalPlayer.Party.MemberList[num4];
										if (entityPlayer != this.LocalPlayer && entityPlayer.TwitchEnabled)
										{
											SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(this.QueuedActionEntries[num3].Action.EventName, this.QueuedActionEntries[num3].Target.entityId, this.QueuedActionEntries[num3].UserName, "action", NetPackageGameEventResponse.ResponseTypes.TwitchPartyActionApproved, -1, -1, false), false, entityPlayer.entityId, -1, -1, null, 192, false);
										}
									}
								}
								GameEventManager.Current.HandleGameEventApproved(this.QueuedActionEntries[num3].Action.EventName, this.QueuedActionEntries[num3].Target.entityId, this.QueuedActionEntries[num3].UserName, "action");
								break;
							}
							TwitchActionEntry twitchActionEntry = this.QueuedActionEntries[num3];
							ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(twitchActionEntry.UserName);
							this.AddActionHistory(twitchActionEntry, viewerEntry, TwitchActionHistoryEntry.EntryStates.Reimbursed);
							this.ShowReimburseMessage(twitchActionEntry, viewerEntry);
							this.ViewerData.ReimburseAction(twitchActionEntry);
							this.QueuedActionEntries.RemoveAt(num3);
							break;
						}
					}
				}
				this.saveTime -= Time.deltaTime;
				if (this.saveTime <= 0f && (this.dataSaveThreadInfo == null || this.dataSaveThreadInfo.HasTerminated()))
				{
					this.saveTime = 30f;
					if (this.HasDataChanges)
					{
						this.SaveViewerData();
						this.HasDataChanges = false;
					}
				}
				this.updateTime -= deltaTime;
				if (this.updateTime <= 0f)
				{
					this.updateTime = 2f;
					if (this.UseProgression && !this.OverrideProgession && this.commandsAvailable == -1)
					{
						this.commandsAvailable = this.GetCommandCount();
					}
					this.RefreshCommands(true);
					int @int = GameStats.GetInt(EnumGameStats.BloodMoonDay);
					if (this.nextBMDay != @int)
					{
						this.nextBMDay = @int;
						if (num2 != this.currentBMDayEnd)
						{
							this.currentBMDayEnd = this.nextBMDay + 1;
						}
						this.SetupBloodMoonData();
					}
					this.RefreshVoteLockedLevel();
					this.IsSafe = this.LocalPlayer.TwitchSafe;
					this.isBMActive = false;
					if (this.CooldownType != TwitchManager.CooldownTypes.Time && !this.IsVoting)
					{
						if (this.UseActionsDuringBloodmoon != 1)
						{
							if (this.WithinBloodMoonPeriod())
							{
								this.isBMActive = true;
								if (this.CooldownType != TwitchManager.CooldownTypes.BloodMoonDisabled && this.CooldownType != TwitchManager.CooldownTypes.BloodMoonCooldown)
								{
									this.SetCooldown(5f, (this.UseActionsDuringBloodmoon == 0) ? TwitchManager.CooldownTypes.BloodMoonDisabled : TwitchManager.CooldownTypes.BloodMoonCooldown, false, true);
								}
							}
							else if (this.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || this.CooldownType == TwitchManager.CooldownTypes.BloodMoonCooldown)
							{
								this.SetCooldown(5f, TwitchManager.CooldownTypes.Time, false, true);
								this.currentBMDayEnd = this.nextBMDay + 1;
								this.VotingManager.VoteStartDelayTimeRemaining += 35f;
							}
						}
						else if (this.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || this.CooldownType == TwitchManager.CooldownTypes.BloodMoonCooldown)
						{
							this.SetCooldown(5f, TwitchManager.CooldownTypes.Time, false, true);
						}
						if (this.AllowActions && this.CooldownType != TwitchManager.CooldownTypes.BloodMoonDisabled && this.CooldownType != TwitchManager.CooldownTypes.BloodMoonCooldown)
						{
							if (this.UseActionsDuringQuests != 1 && this.CooldownType != TwitchManager.CooldownTypes.Time)
							{
								if (QuestEventManager.Current.QuestBounds.width != 0f)
								{
									if (this.UseActionsDuringQuests == 0 && this.CooldownType != TwitchManager.CooldownTypes.QuestDisabled)
									{
										this.SetCooldown(5f, TwitchManager.CooldownTypes.QuestDisabled, false, false);
									}
									else if (this.UseActionsDuringQuests == 2 && this.CooldownType != TwitchManager.CooldownTypes.QuestCooldown)
									{
										this.SetCooldown(5f, TwitchManager.CooldownTypes.QuestCooldown, false, false);
									}
								}
								else if (this.CooldownType == TwitchManager.CooldownTypes.QuestCooldown || this.CooldownType == TwitchManager.CooldownTypes.QuestDisabled)
								{
									this.SetCooldown(60f, TwitchManager.CooldownTypes.Time, false, true);
									this.CurrentCooldownFill = 0f;
								}
							}
							else if (this.UseActionsDuringQuests == 1 && (this.CooldownType == TwitchManager.CooldownTypes.QuestCooldown || this.CooldownType == TwitchManager.CooldownTypes.QuestDisabled))
							{
								this.SetCooldown(60f, TwitchManager.CooldownTypes.Time, false, true);
							}
							if (this.CooldownType != TwitchManager.CooldownTypes.Time && this.CooldownType != TwitchManager.CooldownTypes.Startup && this.CooldownType != TwitchManager.CooldownTypes.MaxReached && this.CooldownType != TwitchManager.CooldownTypes.MaxReachedWaiting && this.CooldownType != TwitchManager.CooldownTypes.QuestCooldown && this.CooldownType != TwitchManager.CooldownTypes.QuestDisabled)
							{
								if (this.CooldownType != TwitchManager.CooldownTypes.SafeCooldown && this.LocalPlayer.TwitchSafe)
								{
									this.SetCooldown(5f, TwitchManager.CooldownTypes.SafeCooldown, false, false);
								}
								else if (this.CooldownType == TwitchManager.CooldownTypes.SafeCooldown && !this.LocalPlayer.TwitchSafe)
								{
									this.SetCooldown(5f, TwitchManager.CooldownTypes.SafeCooldownExit, false, true);
								}
							}
						}
						else if (this.CooldownType == TwitchManager.CooldownTypes.QuestCooldown || this.CooldownType == TwitchManager.CooldownTypes.QuestDisabled)
						{
							this.SetCooldown(60f, TwitchManager.CooldownTypes.Time, false, true);
						}
					}
					for (int num5 = this.RespawnEntries.Count - 1; num5 >= 0; num5--)
					{
						TwitchRespawnEntry twitchRespawnEntry = this.RespawnEntries[num5];
						if (twitchRespawnEntry.CanRespawn(this))
						{
							EntityPlayer target = twitchRespawnEntry.Target;
							if (!this.PartyInfo.ContainsKey(target) || this.PartyInfo[target].Cooldown <= 0f)
							{
								if (target.Buffs.HasBuff("twitch_pausedspawns"))
								{
									target.Buffs.RemoveBuff("twitch_pausedspawns", true);
								}
								target.PlayOneShot("twitch_unpause", false, false, false, null);
								this.QueuedActionEntries.Add(twitchRespawnEntry.RespawnAction());
							}
						}
					}
				}
				if (this.lastAlive && this.LocalPlayer.IsDead())
				{
					this.CurrentCooldownFill = 0f;
					if (this.CurrentCooldownPreset.CooldownType == CooldownPreset.CooldownTypes.Fill)
					{
						this.SetCooldown((float)this.CurrentCooldownPreset.AfterDeathCooldownTime, TwitchManager.CooldownTypes.Time, false, true);
					}
					this.KillAllSpawnsForPlayer(this.LocalPlayer);
				}
				if (this.lastAlive && this.LocalPlayer.IsAlive())
				{
					TwitchManager.DeathText = "";
				}
				if (!this.lastAlive && this.LocalPlayer.IsAlive())
				{
					this.respawnEventNeeded = true;
				}
				if (this.respawnEventNeeded && this.CheckCanRespawnEvent(this.LocalPlayer))
				{
					if (this.OnPlayerRespawnEvent != "")
					{
						GameEventManager.Current.HandleAction(this.OnPlayerRespawnEvent, this.LocalPlayer, this.LocalPlayer, false, "", "", false, true, "", null);
					}
					this.respawnEventNeeded = false;
				}
				this.lastAlive = this.LocalPlayer.IsAlive();
				this.twitchPlayerDeathsThisFrame.Clear();
				this.UpdatePartyInfo(deltaTime);
			}
			this.HandleInGameChatQueue();
		}

		// Token: 0x0600A8BB RID: 43195 RVA: 0x0042701D File Offset: 0x0042521D
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleEndCooldown()
		{
			this.CurrentCooldownFill = 0f;
			Manager.BroadcastPlayByLocalPlayer(this.LocalPlayer.position, "twitch_cooldown_ended");
			this.ircClient.SendChannelMessage(this.chatOutput_CooldownComplete, true);
			this.HandleEndCooldownStateChanging();
		}

		// Token: 0x0600A8BC RID: 43196 RVA: 0x00427058 File Offset: 0x00425258
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleEndCooldownStateChanging()
		{
			this.CooldownType = TwitchManager.CooldownTypes.None;
			if (this.LocalPlayer.TwitchActionsEnabled == EntityPlayer.TwitchActionsStates.TempDisabled || this.LocalPlayer.TwitchActionsEnabled == EntityPlayer.TwitchActionsStates.TempDisabledEnding)
			{
				this.LocalPlayer.TwitchActionsEnabled = EntityPlayer.TwitchActionsStates.Enabled;
			}
			if (this.ConnectionStateChanged != null)
			{
				this.ConnectionStateChanged(this.initState, this.initState);
			}
			this.HandleCooldownActionLocking();
		}

		// Token: 0x0600A8BD RID: 43197 RVA: 0x004270B9 File Offset: 0x004252B9
		public void AddToInGameChatQueue(string msg, string sound = null)
		{
			this.inGameChatQueue.Add(new TwitchMessageEntry(msg, sound));
		}

		// Token: 0x0600A8BE RID: 43198 RVA: 0x004270D0 File Offset: 0x004252D0
		public void HandleInGameChatQueue()
		{
			if (this.inGameChatQueue.Count > 0 && this.LocalPlayer != null && this.LocalPlayer.IsAlive())
			{
				TwitchMessageEntry twitchMessageEntry = this.inGameChatQueue[0];
				XUiC_ChatOutput.AddMessage(this.LocalPlayerXUi, EnumGameMessages.PlainTextLocal, twitchMessageEntry.Message, EChatType.Global, EChatDirection.Inbound, -1, null, null, EMessageSender.Server, GeneratedTextManager.TextFilteringMode.None, GeneratedTextManager.BbCodeSupportMode.Supported);
				if (twitchMessageEntry.Sound != null)
				{
					this.LocalPlayer.PlayOneShot(twitchMessageEntry.Sound, false, false, false, null);
				}
				this.inGameChatQueue.RemoveAt(0);
			}
		}

		// Token: 0x0600A8BF RID: 43199 RVA: 0x00427156 File Offset: 0x00425356
		public void RefreshVoteLockedLevel()
		{
			this.VoteLockedLevel = this.LocalPlayer.HasTwitchVoteLockMember();
		}

		// Token: 0x0600A8C0 RID: 43200 RVA: 0x0042716C File Offset: 0x0042536C
		public void SetupBloodMoonData()
		{
			ValueTuple<int, int> valueTuple = GameUtils.CalcDuskDawnHours(GameStats.GetInt(EnumGameStats.DayLightLength));
			int item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			this.BMCooldownStart = item - this.CurrentCooldownPreset.BMStartOffset;
			this.BMCooldownEnd = item2 + this.CurrentCooldownPreset.BMEndOffset;
		}

		// Token: 0x0600A8C1 RID: 43201 RVA: 0x004271B8 File Offset: 0x004253B8
		public bool WithinBloodMoonPeriod()
		{
			ulong worldTime = this.world.worldTime;
			int num = GameUtils.WorldTimeToDays(worldTime);
			int num2 = GameUtils.WorldTimeToHours(worldTime);
			if (num == this.nextBMDay)
			{
				if (num2 >= this.BMCooldownStart)
				{
					return true;
				}
			}
			else if (num > 1 && num == this.currentBMDayEnd && num2 < this.BMCooldownEnd)
			{
				return true;
			}
			return false;
		}

		// Token: 0x0600A8C2 RID: 43202 RVA: 0x0042720A File Offset: 0x0042540A
		[PublicizedFrom(EAccessModifier.Private)]
		public void Party_PartyMemberAdded(EntityPlayer player)
		{
			this.GetCooldownMax();
			if (!this.PartyInfo.ContainsKey(player))
			{
				this.PartyInfo.Add(player, new TwitchManager.TwitchPartyMemberInfo());
				ExtensionManager extensionManager = this.extensionManager;
				if (extensionManager == null)
				{
					return;
				}
				extensionManager.OnPartyChanged();
			}
		}

		// Token: 0x0600A8C3 RID: 43203 RVA: 0x00427241 File Offset: 0x00425441
		[PublicizedFrom(EAccessModifier.Private)]
		public void Party_PartyMemberRemoved(EntityPlayer player)
		{
			this.GetCooldownMax();
			if (this.PartyInfo.ContainsKey(player))
			{
				this.PartyInfo.Remove(player);
				ExtensionManager extensionManager = this.extensionManager;
				if (extensionManager == null)
				{
					return;
				}
				extensionManager.OnPartyChanged();
			}
		}

		// Token: 0x0600A8C4 RID: 43204 RVA: 0x00427274 File Offset: 0x00425474
		[PublicizedFrom(EAccessModifier.Protected)]
		public void RefreshPartyInfo()
		{
			if (this.LocalPlayer == null)
			{
				return;
			}
			if (this.LocalPlayer.Party == null)
			{
				this.PartyInfo.Clear();
				return;
			}
			this.PartyInfo.Clear();
			for (int i = 0; i < this.LocalPlayer.Party.MemberList.Count; i++)
			{
				EntityPlayer entityPlayer = this.LocalPlayer.Party.MemberList[i];
				if (!(entityPlayer == this.LocalPlayer) && !this.PartyInfo.ContainsKey(entityPlayer))
				{
					this.PartyInfo.Add(entityPlayer, new TwitchManager.TwitchPartyMemberInfo());
				}
			}
		}

		// Token: 0x0600A8C5 RID: 43205 RVA: 0x00427318 File Offset: 0x00425518
		[PublicizedFrom(EAccessModifier.Protected)]
		public void UpdatePartyInfo(float deltaTime)
		{
			bool flag = false;
			foreach (EntityPlayer entityPlayer in this.PartyInfo.Keys)
			{
				TwitchManager.TwitchPartyMemberInfo twitchPartyMemberInfo = this.PartyInfo[entityPlayer];
				if (!twitchPartyMemberInfo.LastAlive && entityPlayer.IsAlive() && this.PartyRespawnEvent != "")
				{
					GameEventManager.Current.HandleAction(this.PartyRespawnEvent, this.LocalPlayer, entityPlayer, false, "", "", false, true, "", null);
				}
				if (twitchPartyMemberInfo.LastOptedOut != (entityPlayer.TwitchActionsEnabled == EntityPlayer.TwitchActionsStates.Disabled))
				{
					twitchPartyMemberInfo.LastOptedOut = (entityPlayer.TwitchActionsEnabled == EntityPlayer.TwitchActionsStates.Disabled);
					flag = true;
				}
				if (twitchPartyMemberInfo.Cooldown > 0f)
				{
					twitchPartyMemberInfo.Cooldown -= deltaTime;
				}
				if (twitchPartyMemberInfo.LastAlive && !entityPlayer.IsAlive())
				{
					this.KillAllSpawnsForPlayer(entityPlayer);
					twitchPartyMemberInfo.Cooldown = 60f;
				}
				if (!twitchPartyMemberInfo.LastAlive && entityPlayer.IsAlive())
				{
					twitchPartyMemberInfo.NeedsRespawnEvent = true;
				}
				if (twitchPartyMemberInfo.NeedsRespawnEvent && this.CheckCanRespawnEvent(entityPlayer) && twitchPartyMemberInfo.Cooldown <= 0f)
				{
					if (this.OnPlayerRespawnEvent != "")
					{
						GameEventManager.Current.HandleAction(this.OnPlayerRespawnEvent, this.LocalPlayer, entityPlayer, false, "", "", false, true, "", null);
					}
					twitchPartyMemberInfo.NeedsRespawnEvent = false;
				}
				twitchPartyMemberInfo.LastAlive = entityPlayer.IsAlive();
			}
			if (flag)
			{
				this.GetCooldownMax();
			}
		}

		// Token: 0x0600A8C6 RID: 43206 RVA: 0x004274C4 File Offset: 0x004256C4
		[PublicizedFrom(EAccessModifier.Protected)]
		public void TwitchDisconnectPartyUpdate()
		{
			this.RespawnEntries.Clear();
			if (this.OnPlayerRespawnEvent != "")
			{
				GameEventManager.Current.HandleAction(this.OnPlayerRespawnEvent, this.LocalPlayer, this.LocalPlayer, false, "", "", false, true, "", null);
			}
			if (this.LocalPlayer != null && this.LocalPlayer.Buffs.HasBuff("twitch_pausedspawns"))
			{
				this.LocalPlayer.Buffs.RemoveBuff("twitch_pausedspawns", true);
			}
			foreach (EntityPlayer entityPlayer in this.PartyInfo.Keys)
			{
				TwitchManager.TwitchPartyMemberInfo twitchPartyMemberInfo = this.PartyInfo[entityPlayer];
				if (twitchPartyMemberInfo.NeedsRespawnEvent)
				{
					if (this.OnPlayerRespawnEvent != "")
					{
						GameEventManager.Current.HandleAction(this.OnPlayerRespawnEvent, this.LocalPlayer, entityPlayer, false, "", "", false, true, "", null);
					}
					twitchPartyMemberInfo.NeedsRespawnEvent = false;
				}
				if (entityPlayer.Buffs.HasBuff("twitch_pausedspawns"))
				{
					entityPlayer.Buffs.RemoveBuff("twitch_pausedspawns", true);
				}
			}
		}

		// Token: 0x0600A8C7 RID: 43207 RVA: 0x00427620 File Offset: 0x00425820
		public bool CheckCanRespawnEvent(EntityPlayer player)
		{
			return player != null && player.IsAlive() && player.TwitchActionsEnabled == EntityPlayer.TwitchActionsStates.Enabled && !player.TwitchSafe;
		}

		// Token: 0x0600A8C8 RID: 43208 RVA: 0x00427648 File Offset: 0x00425848
		[PublicizedFrom(EAccessModifier.Protected)]
		public void KillAllSpawnsForPlayer(EntityPlayer player)
		{
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = this.RespawnEntries.Count - 1; i >= 0; i--)
			{
				TwitchRespawnEntry twitchRespawnEntry = this.RespawnEntries[i];
				if (twitchRespawnEntry != null && twitchRespawnEntry.Target == player)
				{
					twitchRespawnEntry.NeedsRespawn = (twitchRespawnEntry.SpawnedEntities.Count > twitchRespawnEntry.Action.RespawnThreshold);
					if (twitchRespawnEntry.NeedsRespawn)
					{
						if (flag)
						{
							stringBuilder.Append(", ");
						}
						flag = true;
						stringBuilder.Append(twitchRespawnEntry.Action.Command);
					}
					else
					{
						Debug.LogWarning(string.Format("Respawn Entry removed '{0}' because count {1} was less than {2}", twitchRespawnEntry.Action.Command, twitchRespawnEntry.SpawnedEntities.Count, twitchRespawnEntry.Action.RespawnThreshold));
						this.RespawnEntries.RemoveAt(i);
					}
				}
			}
			if (flag)
			{
				if (!player.Buffs.HasBuff("twitch_pausedspawns"))
				{
					player.Buffs.AddBuff("twitch_pausedspawns", -1, true, false, -1f);
				}
				string text = stringBuilder.ToString();
				string msg = string.Format(this.ingameOutput_BitRespawns, text);
				this.AddToInGameChatQueue(msg, "twitch_pause");
				Debug.LogWarning(string.Format("Respawns Found for {0}: {1}", player.EntityName, text));
			}
			else
			{
				Debug.LogWarning("No Respawns Found!");
				if (player.Buffs.HasBuff("twitch_pausedspawns"))
				{
					player.Buffs.RemoveBuff("twitch_pausedspawns", true);
				}
			}
			if (this.OnPlayerDeathEvent != "")
			{
				GameEventManager.Current.HandleAction(this.OnPlayerDeathEvent, this.LocalPlayer, player, false, "", "", false, true, "", null);
			}
			for (int j = this.LiveActionEntries.Count - 1; j >= 0; j--)
			{
				if (this.LiveActionEntries[j] != null && this.LiveActionEntries[j].Target == player)
				{
					TwitchActionEntry twitchActionEntry = this.LiveActionEntries[j];
					twitchActionEntry.ReadyForRemove = true;
					if (twitchActionEntry.HistoryEntry != null)
					{
						twitchActionEntry.HistoryEntry.EntryState = TwitchActionHistoryEntry.EntryStates.Despawned;
					}
				}
			}
		}

		// Token: 0x0600A8C9 RID: 43209 RVA: 0x00427877 File Offset: 0x00425A77
		[PublicizedFrom(EAccessModifier.Private)]
		public void LocalPlayer_PartyChanged(Party _affectedParty, EntityPlayer _player)
		{
			if (this.extensionManager != null)
			{
				this.extensionManager.OnPartyChanged();
			}
			this.GetCooldownMax();
		}

		// Token: 0x0600A8CA RID: 43210 RVA: 0x00427877 File Offset: 0x00425A77
		[PublicizedFrom(EAccessModifier.Private)]
		public void LocalPlayer_PartyLeave(Party _affectedParty, EntityPlayer _player)
		{
			if (this.extensionManager != null)
			{
				this.extensionManager.OnPartyChanged();
			}
			this.GetCooldownMax();
		}

		// Token: 0x0600A8CB RID: 43211 RVA: 0x00427894 File Offset: 0x00425A94
		[PublicizedFrom(EAccessModifier.Private)]
		public void LocalPlayer_PartyJoined(Party _affectedParty, EntityPlayer _player)
		{
			if (this.LocalPlayer.Party != null)
			{
				this.LocalPlayer.Party.PartyMemberAdded += this.Party_PartyMemberAdded;
				this.LocalPlayer.Party.PartyMemberRemoved += this.Party_PartyMemberRemoved;
			}
			if (this.extensionManager != null)
			{
				this.extensionManager.OnPartyChanged();
			}
			this.GetCooldownMax();
			this.RefreshPartyInfo();
		}

		// Token: 0x0600A8CC RID: 43212 RVA: 0x00427908 File Offset: 0x00425B08
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEntityKilled(int entityID)
		{
			for (int i = this.liveList.Count - 1; i >= 0; i--)
			{
				TwitchSpawnedEntityEntry twitchSpawnedEntityEntry = this.liveList[i];
				if (twitchSpawnedEntityEntry.SpawnedEntityID == entityID)
				{
					if (twitchSpawnedEntityEntry.RespawnEntry != null)
					{
						TwitchRespawnEntry respawnEntry = twitchSpawnedEntityEntry.RespawnEntry;
						if (respawnEntry.RemoveSpawnedEntry(entityID, true) && respawnEntry.ReadyForRemove)
						{
							this.RespawnEntries.Remove(respawnEntry);
						}
					}
					this.actionSpawnLiveList.Remove(twitchSpawnedEntityEntry);
					this.recentlyDeadList.Add(new TwitchRecentlyRemovedEntityEntry(twitchSpawnedEntityEntry));
					this.liveList.RemoveAt(i);
					return;
				}
			}
		}

		// Token: 0x0600A8CD RID: 43213 RVA: 0x0042799C File Offset: 0x00425B9C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEntityDespawned(int entityID)
		{
			for (int i = this.liveList.Count - 1; i >= 0; i--)
			{
				if (this.liveList[i].SpawnedEntityID == entityID)
				{
					TwitchSpawnedEntityEntry twitchSpawnedEntityEntry = this.liveList[i];
					if (twitchSpawnedEntityEntry.Action != null && twitchSpawnedEntityEntry.Action.UserName != null)
					{
						this.ViewerData.AddPoints(twitchSpawnedEntityEntry.Action.UserName, (int)((float)twitchSpawnedEntityEntry.Action.ActionCost * 0.25f), false, false);
						if (twitchSpawnedEntityEntry.Action.HistoryEntry != null)
						{
							twitchSpawnedEntityEntry.Action.HistoryEntry.EntryState = TwitchActionHistoryEntry.EntryStates.Despawned;
						}
						if (twitchSpawnedEntityEntry.RespawnEntry != null)
						{
							TwitchRespawnEntry respawnEntry = twitchSpawnedEntityEntry.RespawnEntry;
							if (respawnEntry.RemoveSpawnedEntry(entityID, false) && respawnEntry.RespawnsLeft == 0)
							{
								this.RespawnEntries.Remove(respawnEntry);
							}
						}
					}
					else if (twitchSpawnedEntityEntry.Event != null && twitchSpawnedEntityEntry.Event.HistoryEntry != null)
					{
						twitchSpawnedEntityEntry.Event.HistoryEntry.EntryState = TwitchActionHistoryEntry.EntryStates.Despawned;
					}
					this.actionSpawnLiveList.Remove(twitchSpawnedEntityEntry);
					this.recentlyDeadList.Add(new TwitchRecentlyRemovedEntityEntry(twitchSpawnedEntityEntry));
					this.liveList.RemoveAt(i);
					return;
				}
			}
		}

		// Token: 0x0600A8CE RID: 43214 RVA: 0x00427ACD File Offset: 0x00425CCD
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEventAccessApproved()
		{
			if (this.InitState == TwitchManager.InitStates.None || this.InitState == TwitchManager.InitStates.WaitingForPermission)
			{
				this.StartTwitchIntegration();
			}
		}

		// Token: 0x0600A8CF RID: 43215 RVA: 0x00427AE8 File Offset: 0x00425CE8
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEventApproved(string gameEventID, int targetEntityID, string viewerName, string tag)
		{
			if (tag == "action")
			{
				for (int i = 0; i < this.QueuedActionEntries.Count; i++)
				{
					if (this.QueuedActionEntries[i].Action.EventName == gameEventID && this.QueuedActionEntries[i].Target.entityId == targetEntityID && this.QueuedActionEntries[i].UserName == viewerName)
					{
						this.ConfirmAction(this.QueuedActionEntries[i]);
						this.LiveActionEntries.Add(this.QueuedActionEntries[i]);
						this.QueuedActionEntries.RemoveAt(i);
						return;
					}
				}
				return;
			}
			if (tag == "event")
			{
				for (int j = 0; j < this.EventQueue.Count; j++)
				{
					if (this.EventQueue[j].UserName == viewerName && this.EventQueue[j].Event.EventName == gameEventID)
					{
						this.LiveEvents.Add(this.EventQueue[j]);
						this.EventQueue.RemoveAt(j);
						return;
					}
				}
			}
		}

		// Token: 0x0600A8D0 RID: 43216 RVA: 0x00427C28 File Offset: 0x00425E28
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEventDenied(string gameEventID, int targetEntityID, string viewerName, string tag)
		{
			if (tag == "action")
			{
				for (int i = 0; i < this.QueuedActionEntries.Count; i++)
				{
					if (this.QueuedActionEntries[i].Action.EventName == gameEventID && this.QueuedActionEntries[i].Target.entityId == targetEntityID && this.QueuedActionEntries[i].UserName == viewerName)
					{
						this.ViewerData.ReimburseAction(this.QueuedActionEntries[i]);
						this.QueuedActionEntries.RemoveAt(i);
						return;
					}
				}
			}
		}

		// Token: 0x0600A8D1 RID: 43217 RVA: 0x00427CD4 File Offset: 0x00425ED4
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_TwitchRefundNeeded(string gameEventID, int targetEntityID, string viewerName, string tag)
		{
			if (tag == "action")
			{
				for (int i = 0; i < this.LiveActionEntries.Count; i++)
				{
					TwitchActionEntry twitchActionEntry = this.LiveActionEntries[i];
					if (twitchActionEntry.Action.EventName == gameEventID && twitchActionEntry.Target.entityId == targetEntityID && twitchActionEntry.UserName == viewerName)
					{
						this.ViewerData.ReimburseAction(this.LiveActionEntries[i]);
						this.ShowReimburseMessage(twitchActionEntry, null);
						Debug.LogWarning(string.Format("TwitchAction {0} refunded for {1}.", twitchActionEntry.Action.Name, viewerName));
						if (twitchActionEntry.HistoryEntry != null)
						{
							twitchActionEntry.HistoryEntry.EntryState = TwitchActionHistoryEntry.EntryStates.Reimbursed;
						}
						if (twitchActionEntry.Action.tempCooldown > 30f)
						{
							twitchActionEntry.Action.SetCooldown(this.CurrentUnityTime, 30f);
						}
						this.LiveActionEntries.RemoveAt(i);
						return;
					}
				}
				return;
			}
			if (tag == "event")
			{
				int j = 0;
				while (j < this.LiveEvents.Count)
				{
					TwitchEventActionEntry twitchEventActionEntry = this.LiveEvents[j];
					if (twitchEventActionEntry.Event.EventName == gameEventID && twitchEventActionEntry.UserName == viewerName)
					{
						Debug.LogWarning(string.Format("Twitch Debug: Live Event: Refunded {0} for {1}.", twitchEventActionEntry.Event.EventTitle, viewerName));
						if (twitchEventActionEntry.HistoryEntry != null)
						{
							twitchEventActionEntry.HistoryEntry.EntryState = TwitchActionHistoryEntry.EntryStates.Reimbursed;
							return;
						}
						break;
					}
					else
					{
						j++;
					}
				}
			}
		}

		// Token: 0x0600A8D2 RID: 43218 RVA: 0x00427E5C File Offset: 0x0042605C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEventCompleted(string gameEventID, int targetEntityID, string viewerName, string tag)
		{
			if (tag == "action")
			{
				for (int i = 0; i < this.LiveActionEntries.Count; i++)
				{
					TwitchActionEntry twitchActionEntry = this.LiveActionEntries[i];
					if (!twitchActionEntry.ReadyForRemove && twitchActionEntry.Action.EventName == gameEventID && twitchActionEntry.Target.entityId == targetEntityID && twitchActionEntry.UserName == viewerName)
					{
						twitchActionEntry.ReadyForRemove = true;
						if (twitchActionEntry.HistoryEntry != null)
						{
							twitchActionEntry.HistoryEntry.EntryState = TwitchActionHistoryEntry.EntryStates.Completed;
						}
						return;
					}
				}
				return;
			}
			if (tag == "event")
			{
				for (int j = 0; j < this.LiveEvents.Count; j++)
				{
					TwitchEventActionEntry twitchEventActionEntry = this.LiveEvents[j];
					if (!twitchEventActionEntry.ReadyForRemove && twitchEventActionEntry.UserName == viewerName && twitchEventActionEntry.Event.EventName == gameEventID)
					{
						if (twitchEventActionEntry.HistoryEntry != null)
						{
							twitchEventActionEntry.HistoryEntry.EntryState = TwitchActionHistoryEntry.EntryStates.Completed;
						}
						twitchEventActionEntry.ReadyForRemove = true;
						return;
					}
				}
			}
		}

		// Token: 0x0600A8D3 RID: 43219 RVA: 0x00427F64 File Offset: 0x00426164
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEntitySpawned(string gameEventID, int entityID, string tag)
		{
			if (tag == "action")
			{
				int i = 0;
				while (i < this.LiveActionEntries.Count)
				{
					if (!this.LiveActionEntries[i].ReadyForRemove && this.LiveActionEntries[i].Action.EventName == gameEventID)
					{
						Entity entity = null;
						if (!this.LiveActionEntries[i].Action.AddsToCooldown)
						{
							return;
						}
						TwitchSpawnedEntityEntry twitchSpawnedEntityEntry = new TwitchSpawnedEntityEntry
						{
							Action = this.LiveActionEntries[i],
							SpawnedEntityID = entityID
						};
						this.liveList.Add(twitchSpawnedEntityEntry);
						if (entity == null)
						{
							entity = this.world.GetEntity(entityID);
						}
						twitchSpawnedEntityEntry.SpawnedEntity = entity;
						if (twitchSpawnedEntityEntry.Action.Action.RespawnCountType != TwitchAction.RespawnCountTypes.None)
						{
							TwitchRespawnEntry respawnEntry = this.GetRespawnEntry(twitchSpawnedEntityEntry.Action.UserName, twitchSpawnedEntityEntry.Action.Target, twitchSpawnedEntityEntry.Action.Action);
							respawnEntry.SpawnedEntities.Add(entityID);
							twitchSpawnedEntityEntry.RespawnEntry = respawnEntry;
						}
						this.actionSpawnLiveList.Add(twitchSpawnedEntityEntry);
						return;
					}
					else
					{
						i++;
					}
				}
				return;
			}
			if (tag == "event")
			{
				int j = 0;
				while (j < this.LiveEvents.Count)
				{
					if (!this.LiveEvents[j].ReadyForRemove && this.LiveEvents[j].Event.EventName == gameEventID)
					{
						Entity entity2 = null;
						if (entity2 == null)
						{
							entity2 = this.world.GetEntity(entityID);
						}
						if (!(entity2 is EntityAlive))
						{
							return;
						}
						TwitchSpawnedEntityEntry twitchSpawnedEntityEntry2 = new TwitchSpawnedEntityEntry
						{
							Event = this.LiveEvents[j],
							SpawnedEntityID = entityID
						};
						this.liveList.Add(twitchSpawnedEntityEntry2);
						twitchSpawnedEntityEntry2.SpawnedEntity = entity2;
						this.actionSpawnLiveList.Add(twitchSpawnedEntityEntry2);
						return;
					}
					else
					{
						j++;
					}
				}
				return;
			}
			if (!(tag == "vote") || this.VotingManager.CurrentEvent == null || !(this.VotingManager.CurrentEvent.VoteClass.GameEvent == gameEventID))
			{
				return;
			}
			Entity entity3 = null;
			if (entity3 == null)
			{
				entity3 = this.world.GetEntity(entityID);
			}
			if (!(entity3 is EntityAlive))
			{
				return;
			}
			TwitchSpawnedEntityEntry twitchSpawnedEntityEntry3 = new TwitchSpawnedEntityEntry
			{
				Vote = this.VotingManager.CurrentEvent,
				SpawnedEntityID = entityID
			};
			this.liveList.Add(twitchSpawnedEntityEntry3);
			twitchSpawnedEntityEntry3.SpawnedEntity = entity3;
			this.VotingManager.CurrentEvent.ActiveSpawns.Add(entityID);
			this.actionSpawnLiveList.Add(twitchSpawnedEntityEntry3);
		}

		// Token: 0x0600A8D4 RID: 43220 RVA: 0x00428220 File Offset: 0x00426420
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameBlocksAdded(string gameEventID, int blockGroupID, List<Vector3i> blockList, string tag)
		{
			if (tag == "action")
			{
				int i = 0;
				while (i < this.LiveActionEntries.Count)
				{
					if (!this.LiveActionEntries[i].ReadyForRemove && this.LiveActionEntries[i].Action.EventName == gameEventID)
					{
						if (!this.LiveActionEntries[i].Action.AddsToCooldown)
						{
							return;
						}
						TwitchSpawnedBlocksEntry twitchSpawnedBlocksEntry = new TwitchSpawnedBlocksEntry
						{
							BlockGroupID = blockGroupID,
							Action = this.LiveActionEntries[i],
							blocks = blockList.ToList<Vector3i>()
						};
						this.liveBlockList.Add(twitchSpawnedBlocksEntry);
						if (twitchSpawnedBlocksEntry.Action.Action.RespawnCountType != TwitchAction.RespawnCountTypes.None)
						{
							TwitchRespawnEntry respawnEntry = this.GetRespawnEntry(twitchSpawnedBlocksEntry.Action.UserName, twitchSpawnedBlocksEntry.Action.Target, twitchSpawnedBlocksEntry.Action.Action);
							respawnEntry.SpawnedBlocks.AddRange(blockList);
							twitchSpawnedBlocksEntry.RespawnEntry = respawnEntry;
						}
						return;
					}
					else
					{
						i++;
					}
				}
				return;
			}
			if (tag == "event")
			{
				for (int j = 0; j < this.LiveEvents.Count; j++)
				{
					if (!this.LiveEvents[j].ReadyForRemove && this.LiveEvents[j].Event.EventName == gameEventID)
					{
						TwitchSpawnedBlocksEntry item = new TwitchSpawnedBlocksEntry
						{
							BlockGroupID = blockGroupID,
							Event = this.LiveEvents[j],
							blocks = blockList.ToList<Vector3i>()
						};
						this.liveBlockList.Add(item);
						return;
					}
				}
				return;
			}
			if (tag == "vote" && this.VotingManager.CurrentEvent != null && this.VotingManager.CurrentEvent.VoteClass.GameEvent == gameEventID)
			{
				TwitchSpawnedBlocksEntry item2 = new TwitchSpawnedBlocksEntry
				{
					BlockGroupID = blockGroupID,
					Vote = this.VotingManager.CurrentEvent,
					blocks = blockList.ToList<Vector3i>()
				};
				this.liveBlockList.Add(item2);
				return;
			}
		}

		// Token: 0x0600A8D5 RID: 43221 RVA: 0x00428434 File Offset: 0x00426634
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameBlocksRemoved(int blockGroupID, bool isDespawn)
		{
			for (int i = 0; i < this.liveBlockList.Count; i++)
			{
				if (this.liveBlockList[i].BlockGroupID == blockGroupID)
				{
					if (this.liveBlockList[i].RespawnEntry != null)
					{
						TwitchRespawnEntry respawnEntry = this.liveBlockList[i].RespawnEntry;
						if (respawnEntry.RemoveAllSpawnedBlock(!isDespawn) && respawnEntry.ReadyForRemove)
						{
							this.RespawnEntries.Remove(respawnEntry);
						}
					}
					this.liveBlockList.RemoveAt(i);
					return;
				}
			}
		}

		// Token: 0x0600A8D6 RID: 43222 RVA: 0x004284C0 File Offset: 0x004266C0
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameBlockRemoved(Vector3i blockRemoved)
		{
			for (int i = 0; i < this.liveBlockList.Count; i++)
			{
				if (this.liveBlockList[i].RemoveBlock(blockRemoved))
				{
					this.liveBlockList[i].TimeRemaining = 5f;
					return;
				}
			}
		}

		// Token: 0x0600A8D7 RID: 43223 RVA: 0x00428510 File Offset: 0x00426710
		public void HandleConsoleAction(List<string> consoleParams)
		{
			for (int i = 0; i < this.TwitchCommandList.Count; i++)
			{
				for (int j = 0; j < this.TwitchCommandList[i].CommandText.Length; j++)
				{
					if (consoleParams[0].StartsWith(this.TwitchCommandList[i].CommandText[j]))
					{
						this.TwitchCommandList[i].ExecuteConsole(consoleParams);
						return;
					}
				}
			}
		}

		// Token: 0x0600A8D8 RID: 43224 RVA: 0x00428588 File Offset: 0x00426788
		public bool IsActionAvailable(string actionName)
		{
			if (!this.VotingManager.VotingIsActive)
			{
				if (actionName[0] != '#')
				{
					actionName = "#" + actionName.ToLower();
				}
				if (this.twitchActive && this.VoteLockedLevel != TwitchVoteLockTypes.ActionsLocked && this.AllowActions && this.AvailableCommands.ContainsKey(actionName))
				{
					TwitchAction twitchAction = this.AvailableCommands[actionName];
					if (!twitchAction.IgnoreCooldown)
					{
						if (this.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || this.CooldownType == TwitchManager.CooldownTypes.Time || this.CooldownType == TwitchManager.CooldownTypes.QuestDisabled)
						{
							return false;
						}
						if ((this.CooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting || this.CooldownType == TwitchManager.CooldownTypes.SafeCooldown) && twitchAction.WaitingBlocked)
						{
							return false;
						}
					}
					if (this.UseProgression && !this.OverrideProgession && twitchAction.StartGameStage != -1 && twitchAction.StartGameStage > this.HighestGameStage)
					{
						return false;
					}
					if (!twitchAction.CanUse)
					{
						return false;
					}
					if ((twitchAction.IgnoreCooldown || !twitchAction.CooldownBlocked || !this.OnCooldown) && twitchAction.IsReady(this))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600A8D9 RID: 43225 RVA: 0x0042869C File Offset: 0x0042689C
		public void HandleExtensionMessage(int userId, string message, bool isRerun, int creditUsed, int bitsUsed)
		{
			string text;
			if (!this.ViewerData.IdToUsername.TryGetValue(userId, out text))
			{
				return;
			}
			bool flag = false;
			if (creditUsed < 0)
			{
				creditUsed = 0;
			}
			string[] array = message.Split(' ', StringSplitOptions.None);
			string text2 = array[0];
			TwitchAction twitchAction = null;
			if (this.AvailableCommands.ContainsKey(text2))
			{
				twitchAction = this.AvailableCommands[text2];
			}
			else
			{
				foreach (TwitchAction twitchAction2 in TwitchActionManager.TwitchActions.Values)
				{
					if (twitchAction2.IsInPreset(this.CurrentActionPreset) && twitchAction2.Command == text2)
					{
						twitchAction = twitchAction2;
						break;
					}
				}
			}
			if (twitchAction == null)
			{
				return;
			}
			bool flag2 = twitchAction.PointType == TwitchAction.PointTypes.Bits;
			if (!this.VotingManager.VotingIsActive && this.twitchActive && (flag2 || (this.VoteLockedLevel != TwitchVoteLockTypes.ActionsLocked && this.AllowActions)))
			{
				if (!isRerun && !flag2)
				{
					if (!twitchAction.IgnoreCooldown)
					{
						if (this.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || this.CooldownType == TwitchManager.CooldownTypes.Time || this.CooldownType == TwitchManager.CooldownTypes.QuestDisabled)
						{
							return;
						}
						if ((this.CooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting || this.CooldownType == TwitchManager.CooldownTypes.SafeCooldown) && twitchAction.WaitingBlocked)
						{
							return;
						}
					}
					if (this.UseProgression && !this.OverrideProgession && twitchAction.StartGameStage != -1 && twitchAction.StartGameStage > this.HighestGameStage)
					{
						return;
					}
					if (!twitchAction.CanUse)
					{
						return;
					}
				}
				if (flag2 || isRerun || twitchAction.IgnoreCooldown || !twitchAction.CooldownBlocked || !this.OnCooldown || ((this.CooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting || this.CooldownType == TwitchManager.CooldownTypes.SafeCooldown) && !twitchAction.WaitingBlocked))
				{
					TwitchActionEntry twitchActionEntry = null;
					if ((isRerun || twitchAction.IsReady(this)) && this.ViewerData.HandleInitialActionEntrySetup(text, twitchAction, isRerun, flag2, out twitchActionEntry))
					{
						twitchActionEntry.UserName = text;
						twitchActionEntry.ChannelNotify = (twitchAction.PointType == TwitchAction.PointTypes.Bits);
						twitchActionEntry.IsBitAction = flag2;
						twitchActionEntry.IsReRun = isRerun;
						twitchActionEntry.Action = twitchAction;
						EntityPlayer entityPlayer = this.LocalPlayer;
						if (array.Length > 1 && this.LocalPlayer.Party != null)
						{
							string b = message.Substring(array[0].Length + 1).ToLower();
							for (int i = 0; i < this.LocalPlayer.Party.MemberList.Count; i++)
							{
								if (this.LocalPlayer.Party.MemberList[i].EntityName.ToLower() == b)
								{
									entityPlayer = this.LocalPlayer.Party.MemberList[i];
									break;
								}
							}
							if (entityPlayer != this.LocalPlayer && entityPlayer.TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Enabled)
							{
								if (flag2)
								{
									ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(twitchActionEntry.UserName);
									twitchActionEntry.Target = entityPlayer;
									this.AddActionHistory(twitchActionEntry, viewerEntry, TwitchActionHistoryEntry.EntryStates.Reimbursed);
									this.ShowReimburseMessage(twitchActionEntry, viewerEntry);
								}
								this.ViewerData.ReimburseAction(twitchActionEntry);
								return;
							}
							if (this.PartyInfo.ContainsKey(entityPlayer) && this.PartyInfo[entityPlayer].Cooldown > 0f)
							{
								if (flag2)
								{
									ViewerEntry viewerEntry2 = this.ViewerData.GetViewerEntry(twitchActionEntry.UserName);
									twitchActionEntry.Target = entityPlayer;
									this.AddActionHistory(twitchActionEntry, viewerEntry2, TwitchActionHistoryEntry.EntryStates.Reimbursed);
									this.ShowReimburseMessage(twitchActionEntry, viewerEntry2);
								}
								this.ViewerData.ReimburseAction(twitchActionEntry);
								return;
							}
						}
						twitchActionEntry.Target = entityPlayer;
						if (twitchActionEntry.CreditsUsed != creditUsed)
						{
							Debug.LogWarning(string.Format("Twitch Bit Credit usage is invalid: {0} used {1} when their balance was {2}. They were credited the amount they spent in bits.", twitchActionEntry.UserName, creditUsed, twitchActionEntry.CreditsUsed));
							this.ViewerData.AddCredit(text, creditUsed + bitsUsed, false);
							ViewerEntry viewerEntry3 = this.ViewerData.GetViewerEntry(twitchActionEntry.UserName);
							this.AddActionHistory(twitchActionEntry, viewerEntry3, TwitchActionHistoryEntry.EntryStates.Reimbursed);
							this.ShowReimburseMessage(twitchActionEntry, viewerEntry3);
							return;
						}
						if (twitchAction.ModifiedCooldown > 0f)
						{
							twitchAction.tempCooldownSet = this.CurrentUnityTime;
							twitchAction.tempCooldown = 1f;
						}
						if (twitchActionEntry.CreditsUsed > 0)
						{
							ViewerEntry viewerEntry4 = this.ViewerData.GetViewerEntry(twitchActionEntry.UserName);
							this.PushBalanceToExtensionQueue(viewerEntry4.UserID.ToString(), viewerEntry4.BitCredits);
						}
						this.QueuedActionEntries.Add(twitchActionEntry);
						flag = true;
					}
				}
			}
			if (!flag && flag2)
			{
				ViewerEntry viewerEntry5 = this.ViewerData.AddCredit(text, twitchAction.CurrentCost - creditUsed, false);
				if (viewerEntry5 != null)
				{
					this.ShowReimburseMessage(text, creditUsed, twitchAction, viewerEntry5);
				}
			}
		}

		// Token: 0x0600A8DA RID: 43226 RVA: 0x00428B4C File Offset: 0x00426D4C
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleMessage(TwitchIRCClient.TwitchChatMessage message)
		{
			switch (message.MessageType)
			{
			case TwitchIRCClient.TwitchChatMessage.MessageTypes.Invalid:
				return;
			case TwitchIRCClient.TwitchChatMessage.MessageTypes.Message:
			{
				if (this.InitState != TwitchManager.InitStates.Ready)
				{
					return;
				}
				string text = message.Message.ToLower();
				ViewerEntry entry = this.ViewerData.UpdateViewerEntry(message.UserID, message.UserName, message.UserNameColor, message.isSub);
				if (message.isBroadcaster)
				{
					if (text.StartsWith("#cooldowninfo"))
					{
						this.ircClient.SendChannelMessage(string.Format("[7DTD]: Cooldown is at {0}/{1}.", this.CurrentCooldownFill, this.CurrentCooldownPreset.CooldownFillMax), true);
					}
					else if (text.StartsWith("#reset "))
					{
						this.actionSpawnLiveList.Clear();
						this.LiveActionEntries.Clear();
						this.ircClient.SendChannelMessage("[7DTD]: Action Live list Cleared!", true);
					}
				}
				for (int i = 0; i < this.TwitchCommandList.Count; i++)
				{
					for (int j = 0; j < this.TwitchCommandList[i].CommandTextList.Count; j++)
					{
						if (text.StartsWith(this.TwitchCommandList[i].CommandTextList[j]) && this.TwitchCommandList[i].CheckAllowed(message))
						{
							this.TwitchCommandList[i].Execute(entry, message);
							break;
						}
					}
				}
				this.VotingManager.HandleMessage(message);
				if (!this.VotingManager.VotingIsActive)
				{
					if (this.IntegrationSetting == TwitchManager.IntegrationSettings.ExtensionOnly)
					{
						return;
					}
					string[] array = text.Split(' ', StringSplitOptions.None);
					string key = array[0];
					if (this.AlternateCommands.ContainsKey(key))
					{
						key = this.AlternateCommands[key];
					}
					if (this.twitchActive && this.VoteLockedLevel != TwitchVoteLockTypes.ActionsLocked && this.AllowActions && this.AvailableCommands.ContainsKey(key))
					{
						TwitchAction twitchAction = this.AvailableCommands[key];
						if (twitchAction.PointType == TwitchAction.PointTypes.Bits)
						{
							return;
						}
						if (!twitchAction.IgnoreCooldown)
						{
							if (this.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || this.CooldownType == TwitchManager.CooldownTypes.Time || this.CooldownType == TwitchManager.CooldownTypes.Startup || this.CooldownType == TwitchManager.CooldownTypes.QuestDisabled)
							{
								return;
							}
							if ((this.CooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting || this.CooldownType == TwitchManager.CooldownTypes.SafeCooldown) && twitchAction.WaitingBlocked)
							{
								return;
							}
						}
						if (this.UseProgression && !this.OverrideProgession && twitchAction.StartGameStage != -1 && twitchAction.StartGameStage > this.HighestGameStage)
						{
							return;
						}
						if (!twitchAction.CanUse || !twitchAction.CheckUsable(message))
						{
							return;
						}
						if (twitchAction.IgnoreCooldown || !twitchAction.CooldownBlocked || !this.OnCooldown || ((this.CooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting || this.CooldownType == TwitchManager.CooldownTypes.SafeCooldown) && !twitchAction.WaitingBlocked))
						{
							TwitchActionEntry twitchActionEntry = null;
							if (twitchAction.IsReady(this) && this.ViewerData.HandleInitialActionEntrySetup(message.UserName, twitchAction, false, false, out twitchActionEntry))
							{
								twitchActionEntry.UserName = message.UserName;
								EntityPlayer entityPlayer = this.LocalPlayer;
								if (array.Length > 1 && this.LocalPlayer.Party != null)
								{
									int index = -1;
									if (StringParsers.TryParseSInt32(array[1], out index, 0, -1, NumberStyles.Integer))
									{
										entityPlayer = this.LocalPlayer.Party.GetMemberAtIndex(index, this.LocalPlayer);
										if (entityPlayer == null)
										{
											this.ViewerData.ReimburseAction(twitchActionEntry);
											return;
										}
										if (entityPlayer.TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Enabled)
										{
											this.ViewerData.ReimburseAction(twitchActionEntry);
											return;
										}
									}
									else
									{
										string b = text.Substring(array[0].Length + 1);
										bool flag = false;
										for (int k = 0; k < this.LocalPlayer.Party.MemberList.Count; k++)
										{
											if (this.LocalPlayer.Party.MemberList[k].EntityName.ToLower() == b)
											{
												entityPlayer = this.LocalPlayer.Party.MemberList[k];
												flag = true;
												break;
											}
										}
										if (!flag)
										{
											this.ViewerData.ReimburseAction(twitchActionEntry);
											return;
										}
										if (entityPlayer != this.LocalPlayer && entityPlayer.TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Enabled)
										{
											this.ViewerData.ReimburseAction(twitchActionEntry);
											return;
										}
										if (this.PartyInfo.ContainsKey(entityPlayer) && this.PartyInfo[entityPlayer].Cooldown > 0f)
										{
											this.ViewerData.ReimburseAction(twitchActionEntry);
											return;
										}
									}
								}
								if (twitchAction.StreamerOnly && entityPlayer != this.LocalPlayer)
								{
									this.ViewerData.ReimburseAction(twitchActionEntry);
									return;
								}
								twitchActionEntry.Target = entityPlayer;
								twitchActionEntry.Action = twitchAction;
								this.QueuedActionEntries.Add(twitchActionEntry);
								return;
							}
						}
					}
				}
				break;
			}
			case TwitchIRCClient.TwitchChatMessage.MessageTypes.Output:
				break;
			case TwitchIRCClient.TwitchChatMessage.MessageTypes.Authenticated:
				if (this.InitState != TwitchManager.InitStates.Ready)
				{
					List<string> list = new List<string>();
					list.Add("CAP REQ :twitch.tv/membership");
					list.Add("CAP REQ :twitch.tv/tags");
					list.Add("CAP REQ :twitch.tv/commands");
					this.ircClient.SendIrcMessages(list, false);
					this.InitState = TwitchManager.InitStates.CheckingForExtension;
					TwitchAuthentication.bFirstLogin = false;
					return;
				}
				break;
			case TwitchIRCClient.TwitchChatMessage.MessageTypes.Raid:
			{
				int viewerAmount = StringParsers.ParseSInt32(message.Message, 0, -1, NumberStyles.Integer);
				this.HandleRaid(message.UserName.ToLower(), message.UserID, viewerAmount);
				return;
			}
			case TwitchIRCClient.TwitchChatMessage.MessageTypes.Charity:
			{
				int charityAmount = StringParsers.ParseSInt32(message.Message, 0, -1, NumberStyles.Integer);
				this.HandleCharity(message.UserName.ToLower(), message.UserID, charityAmount);
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x0600A8DB RID: 43227 RVA: 0x004290C4 File Offset: 0x004272C4
		public void DisplayDebug(string message)
		{
			Debug.LogWarning("Called: " + message);
			Debug.LogWarning(string.Format("[7DTD]: Spawns Alive: {0}  Blocks Alive: {1}  ActionLiveList: {2}.", this.actionSpawnLiveList.Count, this.liveBlockList.Count, this.LiveActionEntries.Count));
			for (int i = 0; i < this.actionSpawnLiveList.Count; i++)
			{
				if (this.actionSpawnLiveList[i].SpawnedEntity != null)
				{
					Debug.LogWarning(string.Format("Spawn Alive: {0}", this.actionSpawnLiveList[i].SpawnedEntity.name));
				}
			}
			for (int j = 0; j < this.LiveActionEntries.Count; j++)
			{
				Debug.LogWarning(string.Format("Action: {0} Target: {1} Viewer: {2}", this.LiveActionEntries[j].Action.Name, this.LiveActionEntries[j].Target.EntityName, this.LiveActionEntries[j].UserName));
			}
			for (int k = 0; k < this.EventQueue.Count; k++)
			{
				Debug.LogWarning(string.Format("Event: {0} User: {1} Sent: {2}", this.EventQueue[k].Event.EventTitle, this.EventQueue[k].UserName, this.EventQueue[k].IsSent));
			}
			this.ircClient.SendChannelMessage("[7DTD]: Debug Complete!", true);
		}

		// Token: 0x0600A8DC RID: 43228 RVA: 0x0042924C File Offset: 0x0042744C
		public void AddToPot(int amount)
		{
			this.RewardPot += amount;
			if (this.RewardPot < 0)
			{
				this.RewardPot = 0;
			}
			if (this.RewardPot > TwitchManager.LeaderboardStats.LargestPimpPot)
			{
				TwitchManager.LeaderboardStats.LargestPimpPot = this.RewardPot;
			}
		}

		// Token: 0x0600A8DD RID: 43229 RVA: 0x0042929C File Offset: 0x0042749C
		public void AddToBitPot(int amount)
		{
			this.BitPot += amount;
			if (this.BitPot < 0)
			{
				this.BitPot = 0;
			}
			if (this.BitPot > TwitchManager.LeaderboardStats.LargestBitPot)
			{
				TwitchManager.LeaderboardStats.LargestBitPot = this.BitPot;
			}
		}

		// Token: 0x0600A8DE RID: 43230 RVA: 0x004292EC File Offset: 0x004274EC
		public void SetPot(int newPot)
		{
			if (newPot < 0)
			{
				newPot = 0;
			}
			this.RewardPot = newPot;
			this.ircClient.SendChannelMessage(string.Format(this.chatOutput_PimpPotBalance, this.RewardPot), true);
			if (this.RewardPot > TwitchManager.LeaderboardStats.LargestPimpPot)
			{
				TwitchManager.LeaderboardStats.LargestPimpPot = this.RewardPot;
			}
		}

		// Token: 0x0600A8DF RID: 43231 RVA: 0x0042934C File Offset: 0x0042754C
		public void SetBitPot(int newPot)
		{
			if (newPot < 0)
			{
				newPot = 0;
			}
			this.BitPot = newPot;
			this.ircClient.SendChannelMessage(string.Format(this.chatOutput_BitPotBalance, this.BitPot), true);
			if (this.BitPot > TwitchManager.LeaderboardStats.LargestBitPot)
			{
				TwitchManager.LeaderboardStats.LargestBitPot = this.BitPot;
			}
		}

		// Token: 0x0600A8E0 RID: 43232 RVA: 0x004293AC File Offset: 0x004275AC
		public void SetCooldown(float newCooldownTime, TwitchManager.CooldownTypes newCooldownType, bool displayToChannel = false, bool playCooldownSound = true)
		{
			if (this.LocalPlayer == null)
			{
				return;
			}
			if (this.CooldownType == newCooldownType && this.CooldownTime == newCooldownTime)
			{
				return;
			}
			if (newCooldownType != TwitchManager.CooldownTypes.MaxReachedWaiting && newCooldownType != TwitchManager.CooldownTypes.SafeCooldown && newCooldownType != TwitchManager.CooldownTypes.SafeCooldownExit && newCooldownType != TwitchManager.CooldownTypes.None)
			{
				this.LocalPlayer.HandleTwitchActionsTempEnabled((newCooldownTime > 15f) ? EntityPlayer.TwitchActionsStates.TempDisabled : EntityPlayer.TwitchActionsStates.TempDisabledEnding);
			}
			else if (newCooldownType == TwitchManager.CooldownTypes.None)
			{
				this.LocalPlayer.HandleTwitchActionsTempEnabled(EntityPlayer.TwitchActionsStates.Enabled);
			}
			this.CooldownType = newCooldownType;
			this.CooldownTime = newCooldownTime;
			if (displayToChannel)
			{
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_CooldownTime, newCooldownTime), true);
			}
			this.HandleCooldownActionLocking();
			if (playCooldownSound && this.LocalPlayer != null && newCooldownType != TwitchManager.CooldownTypes.None)
			{
				Manager.BroadcastPlayByLocalPlayer(this.LocalPlayer.position, "twitch_cooldown_started");
			}
		}

		// Token: 0x0600A8E1 RID: 43233 RVA: 0x00429474 File Offset: 0x00427674
		public bool ForceEndCooldown(bool playEndSound = true)
		{
			if (this.IsReady && (this.CooldownType == TwitchManager.CooldownTypes.MaxReached || this.CooldownType == TwitchManager.CooldownTypes.Time))
			{
				this.SetCooldown(0f, TwitchManager.CooldownTypes.None, false, true);
				this.CurrentCooldownFill = 0f;
				if (playEndSound)
				{
					Manager.BroadcastPlayByLocalPlayer(this.LocalPlayer.position, "twitch_end_cooldown");
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600A8E2 RID: 43234 RVA: 0x004294D0 File Offset: 0x004276D0
		[PublicizedFrom(EAccessModifier.Private)]
		public void ConfirmAction(TwitchActionEntry entry)
		{
			TwitchAction action = entry.Action;
			if (!entry.IsRespawn)
			{
				action.SetQueued();
			}
			if (!entry.IsRespawn)
			{
				if (!entry.IsBitAction && this.PimpPotType != TwitchManager.PimpPotSettings.Disabled)
				{
					int num = (int)EffectManager.GetValue(PassiveEffects.TwitchAddPimpPot, null, (float)action.ModifiedCost * this.ActionPotPercentage, this.LocalPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
					if (num > 0)
					{
						this.RewardPot += num;
						if (this.RewardPot > TwitchManager.LeaderboardStats.LargestPimpPot)
						{
							TwitchManager.LeaderboardStats.LargestPimpPot = this.RewardPot;
						}
					}
				}
				if (entry.IsBitAction && entry.BitsUsed > 0)
				{
					int num2 = (int)((float)(entry.BitsUsed - entry.CreditsUsed) * this.BitPotPercentage);
					if (num2 > 0)
					{
						this.BitPot += num2;
						if (this.BitPot > TwitchManager.LeaderboardStats.LargestBitPot)
						{
							TwitchManager.LeaderboardStats.LargestBitPot = this.BitPot;
						}
					}
				}
			}
			this.AddCooldownForAction(action);
			if (!entry.IsRespawn)
			{
				ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(entry.UserName);
				if (EffectManager.GetValue(PassiveEffects.DisableGameEventNotify, null, 0f, this.LocalPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) == 0f)
				{
					if (action.DelayNotify)
					{
						GameManager.Instance.StartCoroutine(this.onDelayedActionNotify(entry, viewerEntry));
					}
					else
					{
						this.DisplayActionNotification(entry, viewerEntry);
					}
				}
				this.AddActionHistory(entry, viewerEntry, TwitchActionHistoryEntry.EntryStates.Waiting);
			}
		}

		// Token: 0x0600A8E3 RID: 43235 RVA: 0x00429654 File Offset: 0x00427854
		public void ShowReimburseMessage(string userName, int bitsUsed, TwitchAction action, ViewerEntry viewerEntry = null)
		{
			if (bitsUsed > 0)
			{
				if (viewerEntry == null)
				{
					viewerEntry = this.ViewerData.GetViewerEntry(userName);
				}
				if (viewerEntry == null)
				{
					return;
				}
				string msg = string.Format(this.ingameOutput_RefundedAction, new object[]
				{
					viewerEntry.UserColor,
					userName,
					bitsUsed,
					action.Command
				});
				Debug.LogWarning(string.Format("{0} has been refunded {1} bits for {2}", userName, bitsUsed, action.Command));
				this.AddToInGameChatQueue(msg, "twitch_refund");
			}
		}

		// Token: 0x0600A8E4 RID: 43236 RVA: 0x004296D6 File Offset: 0x004278D6
		public void ShowReimburseMessage(TwitchActionEntry entry, ViewerEntry viewerEntry = null)
		{
			this.ShowReimburseMessage(entry.UserName, entry.BitsUsed, entry.Action, viewerEntry);
		}

		// Token: 0x0600A8E5 RID: 43237 RVA: 0x004296F4 File Offset: 0x004278F4
		public TwitchActionHistoryEntry AddActionHistory(TwitchActionEntry entry, ViewerEntry viewerEntry, TwitchActionHistoryEntry.EntryStates startState = TwitchActionHistoryEntry.EntryStates.Waiting)
		{
			if (entry.IsReRun)
			{
				return null;
			}
			if (entry.HistoryEntry == null)
			{
				TwitchActionHistoryEntry twitchActionHistoryEntry = entry.SetupHistoryEntry(viewerEntry);
				twitchActionHistoryEntry.EntryState = startState;
				entry.HistoryEntry = twitchActionHistoryEntry;
				this.ActionHistory.Insert(0, twitchActionHistoryEntry);
				if (this.ActionHistory.Count > 500)
				{
					this.ActionHistory.RemoveAt(this.ActionHistory.Count - 1);
				}
				if (this.ActionHistoryAdded != null)
				{
					this.ActionHistoryAdded();
				}
				return twitchActionHistoryEntry;
			}
			entry.HistoryEntry.EntryState = startState;
			return entry.HistoryEntry;
		}

		// Token: 0x0600A8E6 RID: 43238 RVA: 0x00429788 File Offset: 0x00427988
		public void AddVoteHistory(TwitchVote vote)
		{
			TwitchActionHistoryEntry twitchActionHistoryEntry = new TwitchActionHistoryEntry("Vote", "FFFFFF", null, vote, null);
			this.VoteHistory.Insert(0, twitchActionHistoryEntry);
			twitchActionHistoryEntry.EntryState = TwitchActionHistoryEntry.EntryStates.Completed;
			if (this.VoteHistory.Count > 500)
			{
				this.VoteHistory.RemoveAt(this.VoteHistory.Count - 1);
			}
			if (this.VoteHistoryAdded != null)
			{
				this.VoteHistoryAdded();
			}
		}

		// Token: 0x0600A8E7 RID: 43239 RVA: 0x004297FC File Offset: 0x004279FC
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddCooldownForAction(TwitchAction action)
		{
			if (action.AddsToCooldown)
			{
				int num = (int)EffectManager.GetValue(PassiveEffects.TwitchAddCooldown, null, (float)action.CooldownAddAmount, this.LocalPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
				if (num > 0)
				{
					this.AddCooldownAmount(num);
				}
			}
		}

		// Token: 0x0600A8E8 RID: 43240 RVA: 0x00429848 File Offset: 0x00427A48
		public void AddCooldownAmount(int amount)
		{
			if (this.CurrentCooldownPreset == null)
			{
				this.GetCooldownMax();
			}
			if (this.CooldownType == TwitchManager.CooldownTypes.QuestCooldown || this.CooldownType == TwitchManager.CooldownTypes.QuestDisabled || this.CooldownType == TwitchManager.CooldownTypes.BloodMoonCooldown || this.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled)
			{
				return;
			}
			if (this.IsVoting)
			{
				return;
			}
			if (this.CurrentCooldownPreset != null && this.CurrentCooldownPreset.CooldownType == CooldownPreset.CooldownTypes.Fill)
			{
				if (this.CurrentCooldownFill < this.CurrentCooldownPreset.CooldownFillMax)
				{
					this.CurrentCooldownFill += (float)amount;
					if (this.CurrentCooldownFill >= this.CurrentCooldownPreset.CooldownFillMax)
					{
						this.SetCooldown((float)this.CurrentCooldownPreset.NextCooldownTime, TwitchManager.CooldownTypes.MaxReachedWaiting, false, true);
						if (this.ircClient != null)
						{
							this.ircClient.SendChannelMessage(this.chatOutput_CooldownStarted, true);
						}
					}
				}
				else if (this.CooldownType != TwitchManager.CooldownTypes.MaxReachedWaiting && this.CooldownType != TwitchManager.CooldownTypes.MaxReached)
				{
					this.SetCooldown((float)this.CurrentCooldownPreset.NextCooldownTime, TwitchManager.CooldownTypes.MaxReachedWaiting, false, true);
					if (this.ircClient != null)
					{
						this.ircClient.SendChannelMessage(this.chatOutput_CooldownStarted, true);
					}
				}
			}
			this.UIDirty = true;
		}

		// Token: 0x0600A8E9 RID: 43241 RVA: 0x0042995B File Offset: 0x00427B5B
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator onDelayedActionNotify(TwitchActionEntry entry, ViewerEntry viewerEntry)
		{
			yield return new WaitForSeconds(5f);
			if (this.ircClient != null)
			{
				this.DisplayActionNotification(entry, viewerEntry);
			}
			yield break;
		}

		// Token: 0x0600A8EA RID: 43242 RVA: 0x00429978 File Offset: 0x00427B78
		[PublicizedFrom(EAccessModifier.Private)]
		public void DisplayActionNotification(TwitchActionEntry entry, ViewerEntry viewerEntry)
		{
			if (entry.HistoryEntry != null && entry.HistoryEntry.EntryState == TwitchActionHistoryEntry.EntryStates.Reimbursed)
			{
				return;
			}
			TwitchAction action = entry.Action;
			if (entry.ChannelNotify && action.TwitchNotify)
			{
				string message;
				if (action.PointType == TwitchAction.PointTypes.Bits)
				{
					message = string.Format(this.chatOutput_ActivatedBitAction, new object[]
					{
						entry.UserName,
						action.Command,
						viewerEntry.CombinedPoints,
						entry.Target.EntityName,
						entry.Action.CurrentCost
					});
					if (action.PlayBitSound)
					{
						Manager.PlayInsidePlayerHead(action.IsPositive ? "twitch_donation" : "twitch_donation_bad", this.LocalPlayer.entityId, 0f, false, false);
					}
				}
				else
				{
					message = string.Format(this.chatOutput_ActivatedAction, new object[]
					{
						entry.UserName,
						action.Command,
						viewerEntry.CombinedPoints,
						entry.Target.EntityName
					});
				}
				this.ircClient.SendChannelMessage(message, true);
			}
			string text = string.Format(this.ingameOutput_ActivatedAction, new object[]
			{
				viewerEntry.UserColor,
				entry.UserName,
				action.Command,
				entry.Target.EntityName
			});
			this.SendServerChatMessage(text);
			this.AddToInGameChatQueue(text, null);
		}

		// Token: 0x0600A8EB RID: 43243 RVA: 0x00429AEC File Offset: 0x00427CEC
		[PublicizedFrom(EAccessModifier.Private)]
		public void SendServerChatMessage(string serverMsg)
		{
			if (this.LocalPlayer.IsInParty())
			{
				List<int> memberIdList = this.LocalPlayer.Party.GetMemberIdList(this.LocalPlayer);
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					if (memberIdList == null)
					{
						return;
					}
					using (List<int>.Enumerator enumerator = memberIdList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							int entityId = enumerator.Current;
							ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(entityId);
							if (clientInfo != null)
							{
								clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageSimpleChat>().Setup(serverMsg));
							}
						}
						return;
					}
				}
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageSimpleChat>().Setup(serverMsg, memberIdList), false);
			}
		}

		// Token: 0x0600A8EC RID: 43244 RVA: 0x00429BA8 File Offset: 0x00427DA8
		[PublicizedFrom(EAccessModifier.Private)]
		public void RefreshCommands(bool displayMessage)
		{
			if (this.LocalPlayer.unModifiedGameStage != this.HighestGameStage)
			{
				int highestGameStage = this.HighestGameStage;
				this.HighestGameStage = this.LocalPlayer.unModifiedGameStage;
				this.GetCooldownMax();
				if (this.UseProgression && !this.OverrideProgession)
				{
					this.ActionMessages.Clear();
					this.SetupAvailableCommandsWithOutput(highestGameStage, displayMessage);
					this.ResetDailyCommands(this.lastGameDay, highestGameStage);
					this.HandleCooldownActionLocking();
					this.commandsAvailable = this.AvailableCommands.Count;
				}
			}
		}

		// Token: 0x0600A8ED RID: 43245 RVA: 0x00429C2D File Offset: 0x00427E2D
		public void ToggleTwitchActive()
		{
			this.twitchActive = !this.twitchActive;
			this.ActionMessages.Clear();
			this.HandleCooldownActionLocking();
		}

		// Token: 0x0600A8EE RID: 43246 RVA: 0x00429C4F File Offset: 0x00427E4F
		public void SetTwitchActive(bool newActive)
		{
			if (this.twitchActive != newActive)
			{
				this.twitchActive = newActive;
				this.ActionMessages.Clear();
				this.HandleCooldownActionLocking();
			}
		}

		// Token: 0x0600A8EF RID: 43247 RVA: 0x00429C74 File Offset: 0x00427E74
		public void ResetPrices()
		{
			bool flag = false;
			using (Dictionary<string, TwitchAction>.ValueCollection.Enumerator enumerator = TwitchActionManager.TwitchActions.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.UpdateCost(this.BitPriceMultiplier))
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.resetCommandsNeeded = true;
			}
		}

		// Token: 0x0600A8F0 RID: 43248 RVA: 0x00429CE0 File Offset: 0x00427EE0
		public void ResetPricesToDefault()
		{
			bool flag = false;
			foreach (TwitchAction twitchAction in TwitchActionManager.TwitchActions.Values)
			{
				twitchAction.ResetToDefaultCost();
				if (twitchAction.UpdateCost(this.BitPriceMultiplier))
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.resetCommandsNeeded = true;
			}
		}

		// Token: 0x0600A8F1 RID: 43249 RVA: 0x00429D50 File Offset: 0x00427F50
		[PublicizedFrom(EAccessModifier.Private)]
		public void ResetCommands()
		{
			this.ActionMessages.Clear();
			this.SetupAvailableCommands();
			if (this.UseProgression && !this.OverrideProgession)
			{
				this.ResetDailyCommands(this.lastGameDay, -1);
			}
			this.HandleCooldownActionLocking();
			this.resetCommandsNeeded = false;
		}

		// Token: 0x0600A8F2 RID: 43250 RVA: 0x00429D8D File Offset: 0x00427F8D
		public void SetUseProgression(bool useProgression)
		{
			if (this.UseProgression != useProgression)
			{
				this.UseProgression = useProgression;
				if (this.InitState == TwitchManager.InitStates.Ready)
				{
					this.resetCommandsNeeded = true;
				}
			}
		}

		// Token: 0x0600A8F3 RID: 43251 RVA: 0x00429DB0 File Offset: 0x00427FB0
		[PublicizedFrom(EAccessModifier.Private)]
		public int GetCommandCount()
		{
			int num = 0;
			if (this.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || this.CooldownType == TwitchManager.CooldownTypes.Time || this.CooldownType == TwitchManager.CooldownTypes.QuestDisabled)
			{
				return 0;
			}
			foreach (string key in TwitchActionManager.TwitchActions.Keys)
			{
				TwitchAction twitchAction = TwitchActionManager.TwitchActions[key];
				if (twitchAction.IsInPreset(this.CurrentActionPreset))
				{
					if (this.CooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting)
					{
						if (twitchAction.WaitingBlocked)
						{
							continue;
						}
					}
					else if (twitchAction.CooldownBlocked && this.CooldownTime > 0f)
					{
						continue;
					}
					int startGameStage = twitchAction.StartGameStage;
					if (startGameStage == -1 || startGameStage <= this.HighestGameStage)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x0600A8F4 RID: 43252 RVA: 0x00429E80 File Offset: 0x00428080
		public TwitchRespawnEntry GetRespawnEntry(string username, EntityPlayer target, TwitchAction action)
		{
			for (int i = 0; i < this.RespawnEntries.Count; i++)
			{
				if (this.RespawnEntries[i].CheckRespawn(username, target, action))
				{
					return this.RespawnEntries[i];
				}
			}
			TwitchRespawnEntry twitchRespawnEntry = new TwitchRespawnEntry(username, GameEventManager.Current.Random.RandomRange(action.MinRespawnCount, action.MaxRespawnCount), target, action);
			this.RespawnEntries.Add(twitchRespawnEntry);
			return twitchRespawnEntry;
		}

		// Token: 0x0600A8F5 RID: 43253 RVA: 0x00429EF8 File Offset: 0x004280F8
		public void CheckKiller(EntityPlayer player, EntityAlive killer, Vector3i pos)
		{
			if (this.LocalPlayer == null)
			{
				return;
			}
			if (this.twitchPlayerDeathsThisFrame.Contains(player))
			{
				return;
			}
			if (player == this.LocalPlayer && this.VotingManager != null && this.VotingManager.VotingEnabled && this.VotingManager.CurrentEvent != null)
			{
				this.HandleVoteKill(null);
				this.VotingManager.ResetVoteOnDeath();
				this.twitchPlayerDeathsThisFrame.Add(player);
				return;
			}
			bool flag;
			if (player == this.LocalPlayer)
			{
				flag = true;
			}
			else
			{
				if (this.LocalPlayer.Party == null || !this.LocalPlayer.Party.ContainsMember(player))
				{
					return;
				}
				flag = false;
			}
			TwitchActionEntry twitchActionEntry = null;
			TwitchEventActionEntry twitchEventActionEntry = null;
			TwitchVoteEntry twitchVoteEntry = null;
			if (killer == null)
			{
				int i = this.liveBlockList.Count - 1;
				while (i >= 0)
				{
					if (this.liveBlockList[i].CheckPos(pos))
					{
						TwitchSpawnedBlocksEntry twitchSpawnedBlocksEntry = this.liveBlockList[i];
						twitchActionEntry = twitchSpawnedBlocksEntry.Action;
						twitchEventActionEntry = twitchSpawnedBlocksEntry.Event;
						twitchVoteEntry = twitchSpawnedBlocksEntry.Vote;
						if (twitchSpawnedBlocksEntry.RespawnEntry != null && (flag || twitchActionEntry.Target == player))
						{
							this.RespawnEntries.Remove(twitchSpawnedBlocksEntry.RespawnEntry);
							break;
						}
						break;
					}
					else
					{
						i--;
					}
				}
			}
			else
			{
				int j = this.liveList.Count - 1;
				while (j >= 0)
				{
					if (this.liveList[j].SpawnedEntity == killer)
					{
						TwitchSpawnedEntityEntry twitchSpawnedEntityEntry = this.liveList[j];
						twitchActionEntry = twitchSpawnedEntityEntry.Action;
						twitchEventActionEntry = twitchSpawnedEntityEntry.Event;
						twitchVoteEntry = twitchSpawnedEntityEntry.Vote;
						if (twitchSpawnedEntityEntry.RespawnEntry != null && (flag || twitchSpawnedEntityEntry.Action.Target == player))
						{
							this.RespawnEntries.Remove(twitchSpawnedEntityEntry.RespawnEntry);
							break;
						}
						break;
					}
					else
					{
						j--;
					}
				}
			}
			if (twitchActionEntry == null && twitchEventActionEntry == null && twitchVoteEntry == null)
			{
				for (int k = this.recentlyDeadList.Count - 1; k >= 0; k--)
				{
					if (this.recentlyDeadList[k].SpawnedEntity == killer)
					{
						twitchActionEntry = this.recentlyDeadList[k].Action;
						twitchEventActionEntry = this.recentlyDeadList[k].Event;
						twitchVoteEntry = this.recentlyDeadList[k].Vote;
						break;
					}
				}
			}
			if (twitchActionEntry != null || twitchEventActionEntry != null)
			{
				string text = (twitchActionEntry != null) ? twitchActionEntry.UserName : twitchEventActionEntry.UserName;
				string text2 = (twitchActionEntry != null) ? twitchActionEntry.Action.Command : twitchEventActionEntry.Event.EventTitle;
				if (twitchEventActionEntry != null && (twitchEventActionEntry.Event.EventType == BaseTwitchEventEntry.EventTypes.HypeTrain || twitchEventActionEntry.Event.EventType == BaseTwitchEventEntry.EventTypes.CreatorGoal))
				{
					if (flag)
					{
						int rewardAmount;
						bool flag2;
						if (twitchEventActionEntry.Event.EventType == BaseTwitchEventEntry.EventTypes.HypeTrain)
						{
							TwitchHypeTrainEventEntry twitchHypeTrainEventEntry = (TwitchHypeTrainEventEntry)twitchEventActionEntry.Event;
							rewardAmount = twitchHypeTrainEventEntry.RewardAmount;
							flag2 = (twitchHypeTrainEventEntry.RewardType > TwitchAction.PointTypes.PP);
						}
						else
						{
							TwitchCreatorGoalEventEntry twitchCreatorGoalEventEntry = (TwitchCreatorGoalEventEntry)twitchEventActionEntry.Event;
							rewardAmount = twitchCreatorGoalEventEntry.RewardAmount;
							flag2 = (twitchCreatorGoalEventEntry.RewardType > TwitchAction.PointTypes.PP);
						}
						this.ViewerData.AddPointsAll((!flag2) ? rewardAmount : 0, flag2 ? rewardAmount : 0, false);
						string arg = flag2 ? Localization.Get("TwitchPoints_SP", false) : Localization.Get("TwitchPoints_PP", false);
						string text3 = string.Format(this.ingameOutput_KilledByHypeTrain, rewardAmount, arg, this.LocalPlayer.EntityName);
						GameManager.ShowTooltip(this.LocalPlayer, text3, false, false, 0f);
						GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, Utils.CreateGameMessage(this.Authentication.userName, text3), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported);
						this.ircClient.SendChannelMessage(string.Format(this.chatOutput_KilledByHypeTrain, rewardAmount, arg, this.LocalPlayer.EntityName), true);
						TwitchManager.DeathText = string.Format(this.ingameHypeTrainDeathScreen_Message, rewardAmount, arg);
					}
					this.twitchPlayerDeathsThisFrame.Add(player);
					return;
				}
				if (flag)
				{
					ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(text);
					if ((twitchActionEntry != null && twitchActionEntry.IsBitAction) || (twitchEventActionEntry != null && twitchEventActionEntry.Event.RewardsBitPot))
					{
						if (this.BitPot > 0)
						{
							viewerEntry.BitCredits += this.BitPot;
						}
						string text4 = (this.PimpPotType == TwitchManager.PimpPotSettings.EnabledSP) ? Localization.Get("TwitchPoints_SP", false) : Localization.Get("TwitchPoints_PP", false);
						if (this.PimpPotType != TwitchManager.PimpPotSettings.Disabled)
						{
							if (this.PimpPotType == TwitchManager.PimpPotSettings.EnabledSP)
							{
								viewerEntry.SpecialPoints += (float)this.RewardPot;
							}
							else
							{
								viewerEntry.StandardPoints += (float)this.RewardPot;
							}
						}
						this.ircClient.SendChannelMessage(string.Format(this.chatOutput_KilledByBits, new object[]
						{
							text,
							viewerEntry.BitCredits,
							this.BitPot,
							player.EntityName,
							this.RewardPot,
							text4
						}), true);
						string text5 = string.Format(this.ingameOutput_KilledByBits, new object[]
						{
							viewerEntry.UserColor,
							text,
							this.BitPot,
							player.EntityName,
							this.RewardPot,
							text4
						});
						GameManager.ShowTooltip(this.LocalPlayer, text5, false, false, 0f);
						GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, Utils.CreateGameMessage(this.Authentication.userName, text5), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported);
						TwitchManager.DeathText = string.Format(this.ingameBitsDeathScreen_Message, new object[]
						{
							viewerEntry.UserColor,
							text,
							text2,
							this.BitPot,
							this.RewardPot,
							text4
						});
						QuestEventManager.Current.TwitchEventReceived(TwitchObjectiveTypes.BitPot, "");
						this.BitPot = 0;
						this.RewardPot = TwitchManager.PimpPotDefault;
					}
					else
					{
						string text6 = (this.PimpPotType == TwitchManager.PimpPotSettings.EnabledSP) ? Localization.Get("TwitchPoints_SP", false) : Localization.Get("TwitchPoints_PP", false);
						if (this.PimpPotType != TwitchManager.PimpPotSettings.Disabled)
						{
							if (this.PimpPotType == TwitchManager.PimpPotSettings.EnabledSP)
							{
								viewerEntry.SpecialPoints += (float)this.RewardPot;
							}
							else
							{
								viewerEntry.StandardPoints += (float)this.RewardPot;
							}
							this.ircClient.SendChannelMessage(string.Format(this.chatOutput_KilledStreamer, new object[]
							{
								text,
								viewerEntry.CombinedPoints,
								this.RewardPot,
								text6,
								player.EntityName
							}), true);
							string text7 = string.Format(this.ingameOutput_KilledStreamer, new object[]
							{
								viewerEntry.UserColor,
								text,
								this.RewardPot,
								text6,
								player.EntityName
							});
							GameManager.ShowTooltip(TwitchManager.Current.LocalPlayer, text7, false, false, 0f);
							GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, Utils.CreateGameMessage(this.Authentication.userName, text7), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported);
							QuestEventManager.Current.TwitchEventReceived(TwitchObjectiveTypes.PimpPot, "");
						}
						TwitchManager.DeathText = string.Format(this.ingameDeathScreen_Message, new object[]
						{
							viewerEntry.UserColor,
							text,
							text2,
							this.RewardPot,
							text6
						});
						this.RewardPot = TwitchManager.PimpPotDefault;
					}
					TwitchManager.LeaderboardStats.CheckTopKiller(TwitchManager.LeaderboardStats.AddKill(text, viewerEntry.UserColor));
					this.AddKillToLeaderboard(text, viewerEntry.UserColor);
					this.twitchPlayerDeathsThisFrame.Add(player);
				}
				else
				{
					if (this.PimpPotType != TwitchManager.PimpPotSettings.Disabled)
					{
						ViewerEntry viewerEntry2 = this.ViewerData.GetViewerEntry(text);
						int num = Mathf.Min(this.PartyKillRewardMax, this.RewardPot);
						viewerEntry2.StandardPoints += (float)num;
						this.ircClient.SendChannelMessage(string.Format(this.chatOutput_KilledParty, new object[]
						{
							text,
							viewerEntry2.CombinedPoints,
							num,
							player.EntityName
						}), true);
						string text8 = string.Format(this.ingameOutput_KilledParty, new object[]
						{
							viewerEntry2.UserColor,
							text,
							num,
							player.EntityName
						});
						GameManager.ShowTooltip(TwitchManager.Current.LocalPlayer, text8, false, false, 0f);
						GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, Utils.CreateGameMessage(this.Authentication.userName, text8), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported);
					}
					this.twitchPlayerDeathsThisFrame.Add(player);
				}
			}
			else if (twitchVoteEntry != null && flag && this.VotingManager != null && !twitchVoteEntry.Complete)
			{
				this.HandleVoteKill(twitchVoteEntry);
			}
			if (flag)
			{
				this.VotingManager.ResetVoteOnDeath();
			}
		}

		// Token: 0x0600A8F6 RID: 43254 RVA: 0x0042A7E8 File Offset: 0x004289E8
		[PublicizedFrom(EAccessModifier.Protected)]
		public void HandleVoteKill(TwitchVoteEntry voteEntry)
		{
			List<string> list = this.VotingManager.HandleKiller(voteEntry);
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.ViewerData.GetViewerEntry(list[i]).StandardPoints += (float)this.VotingManager.ViewerDefeatReward;
				}
				string text = string.Format(this.ingameOutput_KilledByVote, this.VotingManager.ViewerDefeatReward, this.LocalPlayer.EntityName);
				GameManager.ShowTooltip(TwitchManager.Current.LocalPlayer, text, false, false, 0f);
				GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, Utils.CreateGameMessage(this.Authentication.userName, text), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_KilledByVote, this.VotingManager.ViewerDefeatReward, this.LocalPlayer.EntityName), true);
				TwitchManager.DeathText = string.Format(this.ingameVoteDeathScreen_Message, this.VotingManager.ViewerDefeatReward);
				list.Clear();
			}
			if (voteEntry != null)
			{
				voteEntry.Complete = true;
			}
		}

		// Token: 0x0600A8F7 RID: 43255 RVA: 0x0042A910 File Offset: 0x00428B10
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddKillToLeaderboard(string username, string usercolor)
		{
			bool flag = false;
			for (int i = 0; i < this.Leaderboard.Count; i++)
			{
				if (this.Leaderboard[i].UserName == username)
				{
					this.Leaderboard[i].Kills++;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.Leaderboard.Add(new TwitchLeaderboardEntry(username, usercolor, 1));
			}
		}

		// Token: 0x0600A8F8 RID: 43256 RVA: 0x0042A981 File Offset: 0x00428B81
		public void ClearLeaderboard()
		{
			this.Leaderboard.Clear();
		}

		// Token: 0x0600A8F9 RID: 43257 RVA: 0x0042A98E File Offset: 0x00428B8E
		public void AddCooldownPreset(CooldownPreset preset)
		{
			if (this.CooldownPresets == null)
			{
				this.CooldownPresets = new List<CooldownPreset>();
			}
			if (preset.IsDefault)
			{
				this.CooldownPresetIndex = this.CooldownPresets.Count;
			}
			this.CooldownPresets.Add(preset);
		}

		// Token: 0x0600A8FA RID: 43258 RVA: 0x0042A9C8 File Offset: 0x00428BC8
		public void SetCooldownPreset(int index)
		{
			if (this.InitState == TwitchManager.InitStates.Ready)
			{
				bool flag = this.CurrentCooldownPreset.CooldownType != this.CooldownPresets[index].CooldownType;
				this.CooldownPresetIndex = index;
				this.GetCooldownMax();
				if (flag)
				{
					this.resetCommandsNeeded = true;
					return;
				}
			}
			else
			{
				this.CooldownPresetIndex = index;
			}
		}

		// Token: 0x0600A8FB RID: 43259 RVA: 0x0042AA20 File Offset: 0x00428C20
		public void SetToDefaultCooldown()
		{
			for (int i = 0; i < this.CooldownPresets.Count; i++)
			{
				if (this.CooldownPresets[i].IsDefault)
				{
					this.SetCooldownPreset(i);
					return;
				}
			}
		}

		// Token: 0x0600A8FC RID: 43260 RVA: 0x0042AA5E File Offset: 0x00428C5E
		public void GetCooldownMax()
		{
			this.CurrentCooldownPreset = this.CooldownPresets[this.CooldownPresetIndex];
			this.CurrentCooldownPreset.SetupCooldownInfo(this.HighestGameStage, this.LocalPlayer);
			this.SetupBloodMoonData();
		}

		// Token: 0x0600A8FD RID: 43261 RVA: 0x0042AA94 File Offset: 0x00428C94
		public void AddTwitchActionPreset(TwitchActionPreset preset)
		{
			if (this.ActionPresets == null)
			{
				this.ActionPresets = new List<TwitchActionPreset>();
			}
			this.ActionPresets.Add(preset);
			if (preset.IsDefault)
			{
				this.ActionPresetIndex = this.ActionPresets.Count - 1;
				this.CurrentActionPreset = this.ActionPresets[this.ActionPresetIndex];
			}
		}

		// Token: 0x0600A8FE RID: 43262 RVA: 0x0042AAF4 File Offset: 0x00428CF4
		public void SetTwitchActionPreset(int index)
		{
			if (this.ActionPresetIndex == index)
			{
				return;
			}
			this.ActionPresets[this.ActionPresetIndex].AddedActions.Clear();
			this.ActionPresets[this.ActionPresetIndex].RemovedActions.Clear();
			this.ActionPresetIndex = index;
			this.CurrentActionPreset = this.ActionPresets[this.ActionPresetIndex];
			this.CurrentActionPreset.HandleCooldowns();
			if (this.InitState == TwitchManager.InitStates.Ready)
			{
				this.resetCommandsNeeded = true;
			}
		}

		// Token: 0x0600A8FF RID: 43263 RVA: 0x0042AB7C File Offset: 0x00428D7C
		public void SetToDefaultActionPreset()
		{
			for (int i = 0; i < this.ActionPresets.Count; i++)
			{
				if (this.ActionPresets[i].IsDefault)
				{
					this.SetTwitchActionPreset(i);
					return;
				}
			}
		}

		// Token: 0x0600A900 RID: 43264 RVA: 0x0042ABBC File Offset: 0x00428DBC
		public void AddTwitchVotePreset(TwitchVotePreset preset)
		{
			if (this.VotePresets == null)
			{
				this.VotePresets = new List<TwitchVotePreset>();
			}
			this.VotePresets.Add(preset);
			if (preset.IsDefault)
			{
				this.VotePresetIndex = this.VotePresets.Count - 1;
				this.CurrentVotePreset = this.VotePresets[this.VotePresetIndex];
			}
		}

		// Token: 0x0600A901 RID: 43265 RVA: 0x0042AC1C File Offset: 0x00428E1C
		public void SetTwitchVotePreset(int index)
		{
			if (this.VotePresetIndex == index)
			{
				return;
			}
			this.VotePresetIndex = index;
			this.CurrentVotePreset = this.VotePresets[this.VotePresetIndex];
			if (this.CurrentVotePreset.IsEmpty)
			{
				this.VotingManager.ForceEndVote();
			}
			this.SetupAvailableCommands();
		}

		// Token: 0x0600A902 RID: 43266 RVA: 0x0042AC70 File Offset: 0x00428E70
		public void SetToDefaultVotePreset()
		{
			for (int i = 0; i < this.VotePresets.Count; i++)
			{
				if (this.VotePresets[i].IsDefault)
				{
					this.SetTwitchVotePreset(i);
					return;
				}
			}
		}

		// Token: 0x0600A903 RID: 43267 RVA: 0x0042ACB0 File Offset: 0x00428EB0
		public void AddTwitchEventPreset(TwitchEventPreset preset)
		{
			if (this.EventPresets == null)
			{
				this.EventPresets = new List<TwitchEventPreset>();
			}
			this.EventPresets.Add(preset);
			if (preset.IsDefault)
			{
				this.EventPresetIndex = this.EventPresets.Count - 1;
				this.CurrentEventPreset = this.EventPresets[this.EventPresetIndex];
			}
		}

		// Token: 0x0600A904 RID: 43268 RVA: 0x0042AD10 File Offset: 0x00428F10
		public void SetTwitchEventPreset(int index, bool oldAllowChannelPointRedeems)
		{
			TwitchEventPreset currentEventPreset = this.CurrentEventPreset;
			this.EventPresetIndex = index;
			this.CurrentEventPreset = this.EventPresets[this.EventPresetIndex];
			if (currentEventPreset != null)
			{
				currentEventPreset.RemoveChannelPointRedemptions(this.AllowChannelPointRedemptions ? this.CurrentEventPreset : null);
			}
			this.SetupTwitchCommands();
			if (this.AllowChannelPointRedemptions && this.CurrentEventPreset != null)
			{
				this.CurrentEventPreset.AddChannelPointRedemptions();
			}
		}

		// Token: 0x0600A905 RID: 43269 RVA: 0x0042AD80 File Offset: 0x00428F80
		public TwitchEventPreset GetEventPreset(string name)
		{
			for (int i = 0; i < this.EventPresets.Count; i++)
			{
				if (this.EventPresets[i].Name.EqualsCaseInsensitive(name))
				{
					return this.EventPresets[i];
				}
			}
			return null;
		}

		// Token: 0x0600A906 RID: 43270 RVA: 0x0042ADCC File Offset: 0x00428FCC
		public void SetToDefaultEventPreset()
		{
			for (int i = 0; i < this.EventPresets.Count; i++)
			{
				if (this.EventPresets[i].IsDefault)
				{
					this.SetTwitchEventPreset(i, this.AllowChannelPointRedemptions);
					return;
				}
			}
		}

		// Token: 0x0600A907 RID: 43271 RVA: 0x0042AE10 File Offset: 0x00429010
		[PublicizedFrom(EAccessModifier.Private)]
		public void PubSub_OnSubscriptionRedeemed(object sender, PubSubSubscriptionRedemptionMessage e)
		{
			if (e.user_name == null)
			{
				return;
			}
			TwitchSubEventEntry.SubTierTypes subTier = TwitchSubEventEntry.GetSubTier(e.sub_plan);
			if (e.is_gift)
			{
				if (e.user_name != "ananonymousgifter")
				{
					this.ViewerData.AddGiftSubEntry(e.user_name, StringParsers.ParseSInt32(e.user_id, 0, -1, NumberStyles.Integer), subTier);
				}
				return;
			}
			ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(e.user_name);
			viewerEntry.UserID = StringParsers.ParseSInt32(e.user_id, 0, -1, NumberStyles.Integer);
			int num = this.ViewerData.GetSubTierPoints(subTier) * this.SubPointModifier;
			if (num > 0)
			{
				viewerEntry.SpecialPoints += (float)num;
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_Subscribed, new object[]
				{
					e.user_name,
					viewerEntry.CombinedPoints,
					this.GetTierName(subTier),
					num
				}), true);
				string msg = string.Format(this.ingameOutput_Subscribed, e.user_name, this.GetTierName(subTier), num);
				this.AddToInGameChatQueue(msg, null);
			}
			this.HandleSubEvent(e.user_name, e.cumulative_months, subTier);
		}

		// Token: 0x0600A908 RID: 43272 RVA: 0x0042AF3C File Offset: 0x0042913C
		[PublicizedFrom(EAccessModifier.Private)]
		public void EventSub_OnSubscriptionRedeemed(SubscriptionEventBase e)
		{
			if (e.UserName == null)
			{
				return;
			}
			string text = e.UserName.ToLower();
			TwitchSubEventEntry.SubTierTypes subTier = TwitchSubEventEntry.GetSubTier(e.Tier);
			ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(text);
			viewerEntry.UserID = StringParsers.ParseSInt32(e.UserId, 0, -1, NumberStyles.Integer);
			int num = this.ViewerData.GetSubTierPoints(subTier) * this.SubPointModifier;
			if (num > 0)
			{
				viewerEntry.SpecialPoints += (float)num;
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_Subscribed, new object[]
				{
					text,
					viewerEntry.CombinedPoints,
					this.GetTierName(subTier),
					num
				}), true);
				string msg = string.Format(this.ingameOutput_Subscribed, text, this.GetTierName(subTier), num);
				this.AddToInGameChatQueue(msg, null);
			}
			SubscriptionMessageEvent subscriptionMessageEvent = e as SubscriptionMessageEvent;
			if (subscriptionMessageEvent != null)
			{
				this.HandleSubEvent(text, subscriptionMessageEvent.CumulativeMonths, subTier);
				return;
			}
			this.HandleSubEvent(text, 1, subTier);
		}

		// Token: 0x0600A909 RID: 43273 RVA: 0x0042B040 File Offset: 0x00429240
		[PublicizedFrom(EAccessModifier.Private)]
		public void EventSub_OnSubGifted(SubscriptionGiftEvent e)
		{
			if (e.UserName != null && !e.IsAnonymous)
			{
				this.ViewerData.AddGiftSubEntry(e.UserName.ToLower(), StringParsers.ParseSInt32(e.UserId, 0, -1, NumberStyles.Integer), TwitchSubEventEntry.GetSubTier(e.Tier));
			}
		}

		// Token: 0x0600A90A RID: 43274 RVA: 0x0042B08C File Offset: 0x0042928C
		public string GetTierName(TwitchSubEventEntry.SubTierTypes tier)
		{
			switch (tier)
			{
			case TwitchSubEventEntry.SubTierTypes.Prime:
				return "Prime";
			case TwitchSubEventEntry.SubTierTypes.Tier1:
				return "1";
			case TwitchSubEventEntry.SubTierTypes.Tier2:
				return "2";
			case TwitchSubEventEntry.SubTierTypes.Tier3:
				return "3";
			default:
				return "1";
			}
		}

		// Token: 0x0600A90B RID: 43275 RVA: 0x0042B0C8 File Offset: 0x004292C8
		public string GetSubTierRewards(int subModifier)
		{
			if (subModifier == 0)
			{
				return Localization.Get("xuiLightPropShadowsNone", false);
			}
			return string.Format(this.subPointDisplay, this.ViewerData.GetSubTierPoints(TwitchSubEventEntry.SubTierTypes.Tier1) * subModifier, this.ViewerData.GetSubTierPoints(TwitchSubEventEntry.SubTierTypes.Tier2) * subModifier, this.ViewerData.GetSubTierPoints(TwitchSubEventEntry.SubTierTypes.Tier3) * subModifier);
		}

		// Token: 0x0600A90C RID: 43276 RVA: 0x0042B128 File Offset: 0x00429328
		public string GetGiftSubTierRewards(int subModifier)
		{
			if (subModifier == 0)
			{
				return Localization.Get("xuiLightPropShadowsNone", false);
			}
			return string.Format(this.subPointDisplay, this.ViewerData.GetGiftSubTierPoints(TwitchSubEventEntry.SubTierTypes.Tier1) * subModifier, this.ViewerData.GetGiftSubTierPoints(TwitchSubEventEntry.SubTierTypes.Tier2) * subModifier, this.ViewerData.GetGiftSubTierPoints(TwitchSubEventEntry.SubTierTypes.Tier3) * subModifier);
		}

		// Token: 0x0600A90D RID: 43277 RVA: 0x0042B188 File Offset: 0x00429388
		public void HandleSubEvent(string username, int months, TwitchSubEventEntry.SubTierTypes tier)
		{
			if (!this.AllowEvents)
			{
				return;
			}
			TwitchSubEventEntry twitchSubEventEntry = this.CurrentEventPreset.HandleSubEvent(months, tier);
			if (twitchSubEventEntry != null)
			{
				ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(username);
				TwitchEventActionEntry twitchEventActionEntry = new TwitchEventActionEntry();
				twitchEventActionEntry.UserName = username;
				twitchEventActionEntry.Event = twitchSubEventEntry;
				this.EventQueue.Add(twitchEventActionEntry);
				twitchEventActionEntry.Event.HandleInstant(username, this);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_SubEvent, twitchSubEventEntry.EventTitle, username, viewerEntry.CombinedPoints), true);
			}
		}

		// Token: 0x0600A90E RID: 43278 RVA: 0x0042B214 File Offset: 0x00429414
		public void HandleGiftSubEvent(string username, int giftCounts, TwitchSubEventEntry.SubTierTypes tier)
		{
			if (!this.AllowEvents)
			{
				return;
			}
			TwitchSubEventEntry twitchSubEventEntry = this.CurrentEventPreset.HandleGiftSubEvent(giftCounts, tier);
			if (twitchSubEventEntry != null)
			{
				ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(username);
				TwitchEventActionEntry twitchEventActionEntry = new TwitchEventActionEntry();
				twitchEventActionEntry.UserName = username;
				twitchEventActionEntry.Event = twitchSubEventEntry;
				this.EventQueue.Add(twitchEventActionEntry);
				twitchEventActionEntry.Event.HandleInstant(username, this);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_GiftSubEvent, twitchSubEventEntry.EventTitle, username, viewerEntry.CombinedPoints), true);
			}
		}

		// Token: 0x0600A90F RID: 43279 RVA: 0x0042B2A0 File Offset: 0x004294A0
		[PublicizedFrom(EAccessModifier.Private)]
		public void EventSubMessageReceived(JObject payload)
		{
			JToken jtoken = payload["subscription"];
			string text;
			if (jtoken == null)
			{
				text = null;
			}
			else
			{
				JToken jtoken2 = jtoken["type"];
				text = ((jtoken2 != null) ? jtoken2.ToString() : null);
			}
			string text2 = text ?? "unknown";
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
			if (num <= 765010691U)
			{
				if (num <= 303371570U)
				{
					if (num != 62361312U)
					{
						if (num == 303371570U)
						{
							if (text2 == "channel.subscribe")
							{
								SubscriptionEvent subscriptionEvent = payload["event"].ToObject<SubscriptionEvent>();
								if (subscriptionEvent != null && !subscriptionEvent.IsGift)
								{
									this.EventSub_OnSubscriptionRedeemed(subscriptionEvent);
									return;
								}
								return;
							}
						}
					}
					else if (text2 == "channel.hype_train.end")
					{
						this.EndHypeTrain();
						return;
					}
				}
				else if (num != 336644660U)
				{
					if (num == 765010691U)
					{
						if (text2 == "channel.bits.use")
						{
							BitsUsedEvent bitsUsedEvent = payload["event"].ToObject<BitsUsedEvent>();
							if (bitsUsedEvent != null)
							{
								this.EventSub_OnBitsRedeemed(bitsUsedEvent);
								return;
							}
							return;
						}
					}
				}
				else if (text2 == "channel.hype_train.begin")
				{
					if (this.HypeTrainLevel == 0)
					{
						this.StartHypeTrain();
						return;
					}
					return;
				}
			}
			else if (num <= 2096046674U)
			{
				if (num != 1906819046U)
				{
					if (num == 2096046674U)
					{
						if (text2 == "channel.raid")
						{
							RaidEvent raidEvent = payload["event"].ToObject<RaidEvent>();
							int userID;
							if (int.TryParse(raidEvent.RaiderID, out userID))
							{
								this.HandleRaid(raidEvent.RaiderUserName.ToLower(), userID, raidEvent.viewerCount);
								return;
							}
							Log.Warning("Failed to run Raid Event because RaiderID could not be parsed as an int");
							return;
						}
					}
				}
				else if (text2 == "channel.subscription.message")
				{
					SubscriptionMessageEvent subscriptionMessageEvent = payload["event"].ToObject<SubscriptionMessageEvent>();
					if (subscriptionMessageEvent != null)
					{
						this.EventSub_OnSubscriptionRedeemed(subscriptionMessageEvent);
						return;
					}
					return;
				}
			}
			else if (num != 2529875513U)
			{
				if (num != 3677527194U)
				{
					if (num == 3760536684U)
					{
						if (text2 == "channel.hype_train.progress")
						{
							HypeTrainProgressEvent hypeTrainProgressEvent = payload["event"].ToObject<HypeTrainProgressEvent>();
							if (this.HypeTrainLevel < hypeTrainProgressEvent.Level)
							{
								this.IncrementHypeTrainLevel();
								return;
							}
							return;
						}
					}
				}
				else if (text2 == "channel.channel_points_custom_reward_redemption.add")
				{
					ChannelPointsRedemptionEvent channelPointsRedemptionEvent = payload["event"].ToObject<ChannelPointsRedemptionEvent>();
					if (channelPointsRedemptionEvent != null)
					{
						this.EventSub_OnChannelPointsRedeemed(channelPointsRedemptionEvent);
						return;
					}
					return;
				}
			}
			else if (text2 == "channel.subscription.gift")
			{
				SubscriptionGiftEvent e = payload["event"].ToObject<SubscriptionGiftEvent>();
				this.EventSub_OnSubGifted(e);
				return;
			}
			Log.Warning("Unhandled event: " + text2);
		}

		// Token: 0x0600A910 RID: 43280 RVA: 0x0042B574 File Offset: 0x00429774
		[PublicizedFrom(EAccessModifier.Private)]
		public void PubSub_OnBitsRedeemed(object sender, PubSubBitRedemptionMessage.BitRedemptionData e)
		{
			if (e.user_name == null)
			{
				return;
			}
			ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(e.user_name);
			int num = e.bits_used * this.BitPointModifier;
			viewerEntry.UserID = StringParsers.ParseSInt32(e.user_id, 0, -1, NumberStyles.Integer);
			viewerEntry.SpecialPoints += (float)num;
			this.ircClient.SendChannelMessage(string.Format(this.chatOutput_DonateBits, new object[]
			{
				e.user_name,
				viewerEntry.CombinedPoints,
				e.bits_used,
				num
			}), true);
			string msg = string.Format(this.ingameOutput_DonateBits, e.user_name, e.bits_used, num);
			this.AddToInGameChatQueue(msg, null);
			this.HandleBitRedeem(e.user_name, e.bits_used, viewerEntry);
		}

		// Token: 0x0600A911 RID: 43281 RVA: 0x0042B658 File Offset: 0x00429858
		[PublicizedFrom(EAccessModifier.Private)]
		public void EventSub_OnBitsRedeemed(BitsUsedEvent e)
		{
			if (e.UserName == null)
			{
				return;
			}
			string text = e.UserName.ToLower();
			ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(text);
			int num = e.Bits * this.BitPointModifier;
			viewerEntry.UserID = StringParsers.ParseSInt32(e.UserId, 0, -1, NumberStyles.Integer);
			viewerEntry.SpecialPoints += (float)num;
			this.ircClient.SendChannelMessage(string.Format(this.chatOutput_DonateBits, new object[]
			{
				text,
				viewerEntry.CombinedPoints,
				e.Bits,
				num
			}), true);
			string msg = string.Format(this.ingameOutput_DonateBits, text, e.Bits, num);
			this.AddToInGameChatQueue(msg, null);
			this.HandleBitRedeem(text, e.Bits, viewerEntry);
		}

		// Token: 0x0600A912 RID: 43282 RVA: 0x0042B734 File Offset: 0x00429934
		public void HandleBitRedeem(string userName, int bitAmount, ViewerEntry viewerEntry = null)
		{
			if (!this.AllowEvents)
			{
				return;
			}
			TwitchEventEntry twitchEventEntry = this.CurrentEventPreset.HandleBitRedeem(bitAmount);
			if (twitchEventEntry != null)
			{
				if (viewerEntry == null)
				{
					viewerEntry = this.ViewerData.GetViewerEntry(userName);
				}
				TwitchEventActionEntry twitchEventActionEntry = new TwitchEventActionEntry();
				twitchEventActionEntry.UserName = userName;
				twitchEventActionEntry.Event = twitchEventEntry;
				this.EventQueue.Add(twitchEventActionEntry);
				twitchEventActionEntry.Event.HandleInstant(userName, this);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_BitEvent, twitchEventEntry.EventTitle, userName, viewerEntry.CombinedPoints), true);
			}
		}

		// Token: 0x0600A913 RID: 43283 RVA: 0x0042B7C1 File Offset: 0x004299C1
		[PublicizedFrom(EAccessModifier.Private)]
		public void PubSub_OnChannelPointsRedeemed(object sender, PubSubChannelPointMessage.ChannelRedemptionData e)
		{
			this.HandleChannelPointsRedeem(e.redemption.reward.title, e.redemption.user.display_name.ToLower());
		}

		// Token: 0x0600A914 RID: 43284 RVA: 0x0042B7EE File Offset: 0x004299EE
		[PublicizedFrom(EAccessModifier.Private)]
		public void EventSub_OnChannelPointsRedeemed(ChannelPointsRedemptionEvent e)
		{
			this.HandleChannelPointsRedeem(e.Reward.Title, e.UserName.ToLower());
		}

		// Token: 0x0600A915 RID: 43285 RVA: 0x0042B80C File Offset: 0x00429A0C
		public void HandleChannelPointsRedeem(string title, string userName)
		{
			if (!this.AllowEvents)
			{
				return;
			}
			TwitchChannelPointEventEntry twitchChannelPointEventEntry = this.CurrentEventPreset.HandleChannelPointsRedeem(title);
			if (twitchChannelPointEventEntry != null)
			{
				ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(userName);
				TwitchEventActionEntry twitchEventActionEntry = new TwitchEventActionEntry();
				twitchEventActionEntry.UserName = userName;
				twitchEventActionEntry.Event = twitchChannelPointEventEntry;
				this.EventQueue.Add(twitchEventActionEntry);
				twitchEventActionEntry.Event.HandleInstant(userName, this);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_ChannelPointEvent, twitchChannelPointEventEntry.EventTitle, userName, viewerEntry.CombinedPoints), true);
				QuestEventManager.Current.TwitchEventReceived(TwitchObjectiveTypes.ChannelPointRedeems, twitchChannelPointEventEntry.EventName);
			}
		}

		// Token: 0x0600A916 RID: 43286 RVA: 0x0042B8A8 File Offset: 0x00429AA8
		public void HandleRaid(string userName, int userID, int viewerAmount)
		{
			ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(userName);
			if (viewerAmount >= this.RaidViewerMinimum && this.RaidPointAdd > 0)
			{
				viewerEntry.UserID = userID;
				viewerEntry.SpecialPoints += (float)this.RaidPointAdd;
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_RaidPoints, new object[]
				{
					userName,
					viewerEntry.CombinedPoints,
					viewerAmount,
					this.RaidPointAdd
				}), true);
				string msg = string.Format(this.ingameOutput_RaidPoints, userName, viewerAmount, this.RaidPointAdd);
				this.AddToInGameChatQueue(msg, null);
			}
			this.HandleRaidRedeem(userName, viewerAmount, viewerEntry);
		}

		// Token: 0x0600A917 RID: 43287 RVA: 0x0042B96C File Offset: 0x00429B6C
		public void HandleRaidRedeem(string userName, int viewerAmount, ViewerEntry viewerEntry = null)
		{
			if (!this.AllowEvents)
			{
				return;
			}
			TwitchEventEntry twitchEventEntry = this.CurrentEventPreset.HandleRaid(viewerAmount);
			if (twitchEventEntry != null)
			{
				if (viewerEntry == null)
				{
					viewerEntry = this.ViewerData.GetViewerEntry(userName);
				}
				TwitchEventActionEntry twitchEventActionEntry = new TwitchEventActionEntry();
				twitchEventActionEntry.UserName = userName;
				twitchEventActionEntry.Event = twitchEventEntry;
				this.EventQueue.Add(twitchEventActionEntry);
				twitchEventActionEntry.Event.HandleInstant(userName, this);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_RaidEvent, new object[]
				{
					twitchEventEntry.EventTitle,
					userName,
					viewerEntry.CombinedPoints,
					viewerAmount
				}), true);
			}
		}

		// Token: 0x0600A918 RID: 43288 RVA: 0x0042BA14 File Offset: 0x00429C14
		public void HandleCharity(string userName, int userID, int charityAmount)
		{
			ViewerEntry viewerEntry = this.ViewerData.GetViewerEntry(userName);
			int num = charityAmount * this.BitPointModifier;
			viewerEntry.UserID = userID;
			viewerEntry.SpecialPoints += (float)num;
			this.ircClient.SendChannelMessage(string.Format(this.chatOutput_DonateCharity, new object[]
			{
				userName,
				viewerEntry.CombinedPoints,
				charityAmount,
				num
			}), true);
			string msg = string.Format(this.ingameOutput_DonateCharity, userName, charityAmount, num);
			this.AddToInGameChatQueue(msg, null);
			this.HandleCharityRedeem(userName, charityAmount, viewerEntry);
		}

		// Token: 0x0600A919 RID: 43289 RVA: 0x0042BAB8 File Offset: 0x00429CB8
		public void HandleCharityRedeem(string userName, int charityAmount, ViewerEntry viewerEntry = null)
		{
			if (!this.AllowEvents)
			{
				return;
			}
			TwitchEventEntry twitchEventEntry = this.CurrentEventPreset.HandleCharityRedeem(charityAmount);
			if (twitchEventEntry != null)
			{
				if (viewerEntry == null)
				{
					viewerEntry = this.ViewerData.GetViewerEntry(userName);
				}
				TwitchEventActionEntry twitchEventActionEntry = new TwitchEventActionEntry();
				twitchEventActionEntry.UserName = userName;
				twitchEventActionEntry.Event = twitchEventEntry;
				this.EventQueue.Add(twitchEventActionEntry);
				twitchEventActionEntry.Event.HandleInstant(userName, this);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_CharityEvent, twitchEventEntry.EventTitle, userName, viewerEntry.CombinedPoints), true);
			}
		}

		// Token: 0x0600A91A RID: 43290 RVA: 0x0042BB45 File Offset: 0x00429D45
		public void StartHypeTrain()
		{
			this.HypeTrainLevel = 1;
			this.HandleHypeTrainRedeem(this.HypeTrainLevel);
		}

		// Token: 0x0600A91B RID: 43291 RVA: 0x0042BB5A File Offset: 0x00429D5A
		public void IncrementHypeTrainLevel()
		{
			this.HypeTrainLevel++;
			this.HandleHypeTrainRedeem(this.HypeTrainLevel);
		}

		// Token: 0x0600A91C RID: 43292 RVA: 0x0042BB76 File Offset: 0x00429D76
		public void EndHypeTrain()
		{
			this.HypeTrainLevel = 0;
		}

		// Token: 0x0600A91D RID: 43293 RVA: 0x0042BB80 File Offset: 0x00429D80
		public void HandleHypeTrainRedeem(int hypeTrainLevel)
		{
			if (!this.AllowEvents)
			{
				return;
			}
			TwitchEventEntry twitchEventEntry = this.CurrentEventPreset.HandleHypeTrainRedeem(hypeTrainLevel);
			if (twitchEventEntry != null)
			{
				TwitchEventActionEntry twitchEventActionEntry = new TwitchEventActionEntry();
				twitchEventActionEntry.UserName = " ";
				twitchEventActionEntry.Event = twitchEventEntry;
				this.EventQueue.Add(twitchEventActionEntry);
				twitchEventActionEntry.Event.HandleInstant(twitchEventActionEntry.UserName, this);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_HypeTrainEvent, twitchEventEntry.EventTitle, hypeTrainLevel), true);
			}
		}

		// Token: 0x0600A91E RID: 43294 RVA: 0x0042BBFF File Offset: 0x00429DFF
		[PublicizedFrom(EAccessModifier.Private)]
		public void PubSub_OnGoalAchieved(object sender, PubSubGoalMessage.Goal e)
		{
			this.HandleCreatorGoalRedeem(e.contributionType.ToLower());
		}

		// Token: 0x0600A91F RID: 43295 RVA: 0x0042BC14 File Offset: 0x00429E14
		public void HandleCreatorGoalRedeem(string goalType)
		{
			if (!this.AllowEvents)
			{
				return;
			}
			TwitchCreatorGoalEventEntry twitchCreatorGoalEventEntry = this.CurrentEventPreset.HandleCreatorGoalEvent(goalType);
			if (twitchCreatorGoalEventEntry != null)
			{
				TwitchEventActionEntry twitchEventActionEntry = new TwitchEventActionEntry();
				twitchEventActionEntry.UserName = " ";
				twitchEventActionEntry.Event = twitchCreatorGoalEventEntry;
				this.EventQueue.Add(twitchEventActionEntry);
				twitchEventActionEntry.Event.HandleInstant(twitchEventActionEntry.UserName, this);
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_CreatorGoalEvent, twitchCreatorGoalEventEntry.EventTitle), true);
			}
		}

		// Token: 0x0600A920 RID: 43296 RVA: 0x0042BC90 File Offset: 0x00429E90
		public void HandleEventQueue()
		{
			if (!this.twitchActive)
			{
				return;
			}
			for (int i = 0; i < this.EventQueue.Count; i++)
			{
				TwitchEventActionEntry twitchEventActionEntry = this.EventQueue[i];
				if (!twitchEventActionEntry.IsSent && twitchEventActionEntry.HandleEvent(this))
				{
					Manager.BroadcastPlayByLocalPlayer(this.LocalPlayer.position, "twitch_custom_event");
					if (!twitchEventActionEntry.IsRetry)
					{
						TwitchActionHistoryEntry twitchActionHistoryEntry = new TwitchActionHistoryEntry(twitchEventActionEntry.UserName, "FFFFFF", null, null, twitchEventActionEntry);
						twitchActionHistoryEntry.EventEntry = twitchEventActionEntry;
						twitchEventActionEntry.HistoryEntry = twitchActionHistoryEntry;
						this.EventHistory.Insert(0, twitchActionHistoryEntry);
						if (this.EventHistory.Count > 500)
						{
							this.EventHistory.RemoveAt(this.EventHistory.Count - 1);
						}
						if (this.EventHistoryAdded != null)
						{
							this.EventHistoryAdded();
						}
					}
					return;
				}
			}
		}

		// Token: 0x170012CB RID: 4811
		// (get) Token: 0x0600A921 RID: 43297 RVA: 0x0042BD70 File Offset: 0x00429F70
		public bool TwitchActive
		{
			get
			{
				return this.twitchActive;
			}
		}

		// Token: 0x0600A922 RID: 43298 RVA: 0x0042BD78 File Offset: 0x00429F78
		public void SaveExportViewerData()
		{
			string arg = GameIO.GetUserGameDataDir() + "/Twitch/" + TwitchManager.MainFileVersion.ToString();
			string savePath = string.Format("{0}/{1}", arg, "twitchexport.txt");
			this.ViewerData.WriteExport(savePath);
		}

		// Token: 0x0600A923 RID: 43299 RVA: 0x0042BDBC File Offset: 0x00429FBC
		public void LoadExportViewerData()
		{
			string arg = GameIO.GetUserGameDataDir() + "/Twitch/" + TwitchManager.MainFileVersion.ToString();
			string path = string.Format("{0}/{1}", arg, "twitchexport.txt");
			if (SdFile.Exists(path))
			{
				using (StreamReader streamReader = File.OpenText(path))
				{
					this.ViewerData.LoadExport(streamReader);
				}
			}
		}

		// Token: 0x0600A924 RID: 43300 RVA: 0x0042BE2C File Offset: 0x0042A02C
		[PublicizedFrom(EAccessModifier.Private)]
		public int saveViewerDataThreaded(ThreadManager.ThreadInfo _threadInfo)
		{
			PooledExpandableMemoryStream[] array = (PooledExpandableMemoryStream[])_threadInfo.parameter;
			string arg = SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient ? GameIO.GetSaveGameLocalDir() : GameIO.GetSaveGameDir();
			string text = string.Format("{0}/{1}", arg, "twitch.dat");
			if (SdFile.Exists(text))
			{
				SdFile.Copy(text, string.Format("{0}/{1}", arg, "twitch.dat.bak"), true);
			}
			array[0].Position = 0L;
			StreamUtils.WriteStreamToFile(array[0], text);
			MemoryPools.poolMemoryStream.FreeSync(array[0]);
			string arg2 = GameIO.GetUserGameDataDir() + "/Twitch/" + TwitchManager.MainFileVersion.ToString();
			text = string.Format("{0}/{1}", arg2, "twitch_main.dat");
			if (SdFile.Exists(text))
			{
				SdFile.Copy(text, string.Format("{0}/{1}", arg2, "twitch_main.dat.bak"), true);
			}
			array[1].Position = 0L;
			StreamUtils.WriteStreamToFile(array[1], text);
			MemoryPools.poolMemoryStream.FreeSync(array[1]);
			return -1;
		}

		// Token: 0x0600A925 RID: 43301 RVA: 0x0042BF1C File Offset: 0x0042A11C
		public void LoadViewerData()
		{
			string arg = SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient ? GameIO.GetSaveGameLocalDir() : GameIO.GetSaveGameDir();
			string path = string.Format("{0}/{1}", arg, "twitch.dat");
			if (SdFile.Exists(path))
			{
				try
				{
					using (Stream stream = SdFile.OpenRead(path))
					{
						using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
						{
							pooledBinaryReader.SetBaseStream(stream);
							this.Read(pooledBinaryReader);
						}
					}
				}
				catch (Exception)
				{
					path = string.Format("{0}/{1}", arg, "twitch.dat.bak");
					if (SdFile.Exists(path))
					{
						using (Stream stream2 = SdFile.OpenRead(path))
						{
							using (PooledBinaryReader pooledBinaryReader2 = MemoryPools.poolBinaryReader.AllocSync(false))
							{
								pooledBinaryReader2.SetBaseStream(stream2);
								this.Read(pooledBinaryReader2);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A926 RID: 43302 RVA: 0x0042C038 File Offset: 0x0042A238
		public void LoadSpecialViewerData()
		{
			string path = string.Format("{0}/{1}", GameIO.GetSaveGameRootDir(), "twitch_special.dat");
			if (SdFile.Exists(path))
			{
				try
				{
					using (Stream stream = SdFile.OpenRead(path))
					{
						using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
						{
							pooledBinaryReader.SetBaseStream(stream);
							this.ReadSpecial(pooledBinaryReader);
						}
					}
				}
				catch (Exception)
				{
					path = string.Format("{0}/{1}", GameIO.GetSaveGameRootDir(), "twitch_special.dat.bak");
					if (SdFile.Exists(path))
					{
						using (Stream stream2 = SdFile.OpenRead(path))
						{
							using (PooledBinaryReader pooledBinaryReader2 = MemoryPools.poolBinaryReader.AllocSync(false))
							{
								pooledBinaryReader2.SetBaseStream(stream2);
								this.ReadSpecial(pooledBinaryReader2);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A927 RID: 43303 RVA: 0x0042C140 File Offset: 0x0042A340
		public bool LoadMainViewerData()
		{
			string path = string.Format("{0}/{1}", GameIO.GetSaveGameRootDir(), "twitch_main.dat");
			if (SdFile.Exists(path))
			{
				try
				{
					using (Stream stream = SdFile.OpenRead(path))
					{
						using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
						{
							pooledBinaryReader.SetBaseStream(stream);
							this.ReadMain(pooledBinaryReader);
						}
					}
				}
				catch (Exception)
				{
					path = string.Format("{0}/{1}", GameIO.GetSaveGameRootDir(), "twitch_main.dat.bak");
					if (SdFile.Exists(path))
					{
						using (Stream stream2 = SdFile.OpenRead(path))
						{
							using (PooledBinaryReader pooledBinaryReader2 = MemoryPools.poolBinaryReader.AllocSync(false))
							{
								pooledBinaryReader2.SetBaseStream(stream2);
								this.ReadMain(pooledBinaryReader2);
							}
						}
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600A928 RID: 43304 RVA: 0x0042C24C File Offset: 0x0042A44C
		public bool LoadLatestMainViewerData()
		{
			for (int i = (int)TwitchManager.MainFileVersion; i >= 2; i--)
			{
				if (this.LoadLatestMainViewerData(i))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600A929 RID: 43305 RVA: 0x0042C278 File Offset: 0x0042A478
		public bool LoadLatestMainViewerData(int version)
		{
			string text = GameIO.GetUserGameDataDir() + "/Twitch/" + version.ToString();
			if (!SdDirectory.Exists(text))
			{
				SdDirectory.CreateDirectory(text);
			}
			string path = string.Format("{0}/{1}", text, "twitch_main.dat");
			if (SdFile.Exists(path))
			{
				try
				{
					using (Stream stream = SdFile.OpenRead(path))
					{
						using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
						{
							pooledBinaryReader.SetBaseStream(stream);
							this.ReadMain(pooledBinaryReader);
						}
					}
				}
				catch (Exception)
				{
					path = string.Format("{0}/{1}", text, "twitch_main.dat.bak");
					if (SdFile.Exists(path))
					{
						using (Stream stream2 = SdFile.OpenRead(path))
						{
							using (PooledBinaryReader pooledBinaryReader2 = MemoryPools.poolBinaryReader.AllocSync(false))
							{
								pooledBinaryReader2.SetBaseStream(stream2);
								this.ReadMain(pooledBinaryReader2);
							}
						}
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600A92A RID: 43306 RVA: 0x0042C3A4 File Offset: 0x0042A5A4
		public void SaveViewerData()
		{
			if (this.dataSaveThreadInfo == null || !ThreadManager.ActiveThreads.ContainsKey("viewerDataSave"))
			{
				PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true);
				using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
				{
					pooledBinaryWriter.SetBaseStream(pooledExpandableMemoryStream);
					this.Write(pooledBinaryWriter);
				}
				PooledExpandableMemoryStream pooledExpandableMemoryStream2 = MemoryPools.poolMemoryStream.AllocSync(true);
				using (PooledBinaryWriter pooledBinaryWriter2 = MemoryPools.poolBinaryWriter.AllocSync(false))
				{
					pooledBinaryWriter2.SetBaseStream(pooledExpandableMemoryStream2);
					this.WriteMain(pooledBinaryWriter2);
				}
				this.dataSaveThreadInfo = ThreadManager.StartThread("viewerDataSave", null, new ThreadManager.ThreadFunctionLoopDelegate(this.saveViewerDataThreaded), null, new PooledExpandableMemoryStream[]
				{
					pooledExpandableMemoryStream,
					pooledExpandableMemoryStream2
				}, null, false, true);
			}
		}

		// Token: 0x0600A92B RID: 43307 RVA: 0x0042C47C File Offset: 0x0042A67C
		public void Write(BinaryWriter bw)
		{
			bw.Write(TwitchManager.FileVersion);
			bw.Write(this.UseProgression);
			this.ViewerData.Write(bw);
			bw.Write(this.Leaderboard.Count);
			for (int i = 0; i < this.Leaderboard.Count; i++)
			{
				TwitchLeaderboardEntry twitchLeaderboardEntry = this.Leaderboard[i];
				bw.Write(twitchLeaderboardEntry.UserName);
				bw.Write(twitchLeaderboardEntry.Kills);
				bw.Write(twitchLeaderboardEntry.UserColor);
			}
			bw.Write(this.UseActionsDuringBloodmoon);
			bw.Write(this.RewardPot);
			bw.Write(this.ViewerData.PointRate);
			bw.Write(this.CooldownPresetIndex);
			bw.Write((byte)this.PimpPotType);
			bw.Write(this.AllowCrateSharing);
			bw.Write(this.BitPointModifier);
			bw.Write(this.RaidPointAdd);
			bw.Write(this.RaidViewerMinimum);
			bw.Write(this.SubPointModifier);
			bw.Write(this.GiftSubPointModifier);
			bw.Write((byte)this.VotingManager.MaxDailyVotes);
			bw.Write(this.VotingManager.VoteTime);
			bw.Write((byte)this.VotingManager.CurrentVoteDayTimeRange);
			bw.Write(this.VotingManager.ViewerDefeatReward);
			bw.Write(this.VotingManager.AllowVotesDuringBloodmoon);
			bw.Write(this.ViewerData.ActionSpamDelay);
			bw.Write(this.ViewerData.StartingPoints);
			bw.Write(this.UseActionsDuringQuests);
			bw.Write(this.VotingManager.AllowVotesDuringQuests);
			bw.Write(this.VotingManager.AllowVotesInSafeZone);
			bw.Write(this.changedEnabledVoteList.Count);
			for (int j = 0; j < this.changedEnabledVoteList.Count; j++)
			{
				bw.Write(this.changedEnabledVoteList[j]);
				bw.Write(TwitchActionManager.TwitchVotes[this.changedEnabledVoteList[j]].Enabled);
			}
			bw.Write((byte)this.integrationSetting);
			bw.Write(this.EventPresetIndex);
			bw.Write(this.ActionPresetIndex);
			bw.Write(this.VotePresetIndex);
			bw.Write(this.AllowBitEvents);
			bw.Write(this.AllowSubEvents);
			bw.Write(this.AllowGiftSubEvents);
			bw.Write(this.AllowCharityEvents);
			bw.Write(this.AllowRaidEvents);
			bw.Write(this.AllowHypeTrainEvents);
			bw.Write(this.AllowChannelPointRedemptions);
			bw.Write(TwitchManager.LeaderboardStats.GoodRewardTime);
			bw.Write(TwitchManager.LeaderboardStats.GoodRewardAmount);
			int num = 0;
			for (int k = 0; k < this.ActionPresets.Count; k++)
			{
				TwitchActionPreset twitchActionPreset = this.ActionPresets[k];
				if (twitchActionPreset.AddedActions.Count > 0 || twitchActionPreset.RemovedActions.Count > 0)
				{
					num++;
				}
			}
			bw.Write(num);
			for (int l = 0; l < this.ActionPresets.Count; l++)
			{
				TwitchActionPreset twitchActionPreset2 = this.ActionPresets[l];
				if (twitchActionPreset2.AddedActions.Count > 0 || twitchActionPreset2.RemovedActions.Count > 0)
				{
					bw.Write(twitchActionPreset2.Name);
					bw.Write(twitchActionPreset2.AddedActions.Count);
					for (int m = 0; m < twitchActionPreset2.AddedActions.Count; m++)
					{
						bw.Write(twitchActionPreset2.AddedActions[m]);
					}
					bw.Write(twitchActionPreset2.RemovedActions.Count);
					for (int n = 0; n < twitchActionPreset2.RemovedActions.Count; n++)
					{
						bw.Write(twitchActionPreset2.RemovedActions[n]);
					}
				}
			}
			bw.Write(this.bitPriceMultiplier);
			bw.Write(this.AllowCreatorGoalEvents);
			bw.Write(this.changedActionList.Count);
			for (int num2 = 0; num2 < this.changedActionList.Count; num2++)
			{
				bw.Write(this.changedActionList[num2]);
				bw.Write(TwitchActionManager.TwitchActions[this.changedActionList[num2]].ModifiedCost);
			}
			bw.Write(this.BitPot);
			bw.Write(this.BitPotPercentage);
		}

		// Token: 0x0600A92C RID: 43308 RVA: 0x0042C8F8 File Offset: 0x0042AAF8
		public void HandleChangedPropertyList()
		{
			this.changedActionList.Clear();
			this.changedEnabledVoteList.Clear();
			foreach (string text in TwitchActionManager.TwitchActions.Keys)
			{
				TwitchAction twitchAction = TwitchActionManager.TwitchActions[text];
				if (twitchAction.DefaultCost != twitchAction.ModifiedCost)
				{
					this.changedActionList.Add(text);
				}
			}
			foreach (string text2 in TwitchActionManager.TwitchVotes.Keys)
			{
				TwitchVote twitchVote = TwitchActionManager.TwitchVotes[text2];
				if (twitchVote.Enabled != twitchVote.OriginalEnabled)
				{
					this.changedEnabledVoteList.Add(text2);
				}
			}
		}

		// Token: 0x0600A92D RID: 43309 RVA: 0x0042C9F0 File Offset: 0x0042ABF0
		public void WriteSpecial(BinaryWriter bw)
		{
			this.ViewerData.WriteSpecial(bw);
		}

		// Token: 0x0600A92E RID: 43310 RVA: 0x0042C9FE File Offset: 0x0042ABFE
		public void WriteMain(BinaryWriter bw)
		{
			bw.Write(TwitchManager.MainFileVersion);
			bw.Write(this.HasViewedSettings);
			this.ViewerData.WriteSpecial(bw);
		}

		// Token: 0x0600A92F RID: 43311 RVA: 0x0042CA24 File Offset: 0x0042AC24
		public void Read(BinaryReader br)
		{
			this.CurrentFileVersion = br.ReadByte();
			if (this.CurrentFileVersion > 1)
			{
				this.UseProgression = br.ReadBoolean();
			}
			this.ViewerData.Read(br, this.CurrentFileVersion);
			if (this.CurrentFileVersion > 3)
			{
				int num = br.ReadInt32();
				this.Leaderboard.Clear();
				for (int i = 0; i < num; i++)
				{
					string username;
					int kills;
					string usercolor;
					if (this.CurrentFileVersion > 10)
					{
						username = br.ReadString();
						kills = br.ReadInt32();
						usercolor = br.ReadString();
					}
					else
					{
						username = br.ReadString();
						usercolor = br.ReadString();
						kills = br.ReadInt32();
					}
					this.Leaderboard.Add(new TwitchLeaderboardEntry(username, usercolor, kills));
				}
			}
			if (this.CurrentFileVersion > 4)
			{
				this.UseActionsDuringBloodmoon = br.ReadInt32();
			}
			if (this.CurrentFileVersion > 5)
			{
				this.RewardPot = br.ReadInt32();
				if (this.RewardPot <= 0)
				{
					this.RewardPot = 0;
				}
				if (this.RewardPot > TwitchManager.LeaderboardStats.LargestPimpPot)
				{
					TwitchManager.LeaderboardStats.LargestPimpPot = this.RewardPot;
				}
			}
			if (this.CurrentFileVersion > 6)
			{
				this.ViewerData.PointRate = br.ReadSingle();
				this.CooldownPresetIndex = br.ReadInt32();
				if (this.CurrentFileVersion <= 18)
				{
					this.ActionCooldownModifier = br.ReadSingle();
				}
				this.PimpPotType = (TwitchManager.PimpPotSettings)br.ReadByte();
				this.AllowCrateSharing = br.ReadBoolean();
			}
			if (this.CurrentFileVersion > 7)
			{
				if (this.CurrentFileVersion <= 17)
				{
					br.ReadBoolean();
					br.ReadBoolean();
					br.ReadBoolean();
				}
				this.BitPointModifier = br.ReadInt32();
				this.RaidPointAdd = br.ReadInt32();
				this.RaidViewerMinimum = br.ReadInt32();
				this.SubPointModifier = br.ReadInt32();
				this.GiftSubPointModifier = br.ReadInt32();
				if (this.CurrentFileVersion <= 20)
				{
					int num2 = br.ReadInt32();
					this.changedActionList.Clear();
					for (int j = 0; j < num2; j++)
					{
						string text = br.ReadString();
						bool enabled = br.ReadBoolean();
						if (TwitchActionManager.TwitchActions.ContainsKey(text))
						{
							this.changedActionList.Add(text);
							TwitchActionManager.TwitchActions[text].Enabled = enabled;
						}
					}
				}
			}
			if (this.CurrentFileVersion > 8)
			{
				this.VotingManager.MaxDailyVotes = (int)br.ReadByte();
				this.VotingManager.VoteTime = br.ReadSingle();
				this.VotingManager.CurrentVoteDayTimeRange = (int)br.ReadByte();
				this.VotingManager.ViewerDefeatReward = br.ReadInt32();
				this.VotingManager.AllowVotesDuringBloodmoon = br.ReadBoolean();
			}
			if (this.CurrentFileVersion > 9)
			{
				this.ViewerData.ActionSpamDelay = br.ReadSingle();
			}
			if (this.CurrentFileVersion > 10)
			{
				this.ViewerData.StartingPoints = br.ReadInt32();
			}
			if (this.CurrentFileVersion > 11)
			{
				this.UseActionsDuringQuests = br.ReadInt32();
				this.VotingManager.AllowVotesDuringQuests = br.ReadBoolean();
			}
			if (this.CurrentFileVersion > 12)
			{
				this.VotingManager.AllowVotesInSafeZone = br.ReadBoolean();
				if (this.CurrentFileVersion <= 17)
				{
					br.ReadByte();
				}
			}
			if (this.CurrentFileVersion > 13)
			{
				int num3 = br.ReadInt32();
				this.changedEnabledVoteList.Clear();
				for (int k = 0; k < num3; k++)
				{
					string text2 = br.ReadString();
					this.changedEnabledVoteList.Add(text2);
					TwitchActionManager.TwitchVotes[text2].Enabled = br.ReadBoolean();
				}
			}
			if (this.CurrentFileVersion >= 16)
			{
				byte b = br.ReadByte();
				if (b > 1)
				{
					b = 1;
				}
				this.IntegrationSetting = (TwitchManager.IntegrationSettings)b;
			}
			if (this.CurrentFileVersion >= 17)
			{
				this.EventPresetIndex = br.ReadInt32();
				this.CurrentEventPreset = this.EventPresets[this.EventPresetIndex];
			}
			if (this.CurrentFileVersion >= 18)
			{
				this.ActionPresetIndex = br.ReadInt32();
				this.CurrentActionPreset = this.ActionPresets[this.ActionPresetIndex];
				this.VotePresetIndex = br.ReadInt32();
				this.CurrentVotePreset = this.VotePresets[this.VotePresetIndex];
				this.AllowBitEvents = br.ReadBoolean();
				this.AllowSubEvents = br.ReadBoolean();
				this.AllowGiftSubEvents = br.ReadBoolean();
				this.AllowCharityEvents = br.ReadBoolean();
				this.AllowRaidEvents = br.ReadBoolean();
				this.AllowHypeTrainEvents = br.ReadBoolean();
				this.AllowChannelPointRedemptions = br.ReadBoolean();
			}
			if (this.CurrentFileVersion >= 20)
			{
				TwitchManager.LeaderboardStats.GoodRewardTime = br.ReadInt32();
				TwitchManager.LeaderboardStats.GoodRewardAmount = br.ReadInt32();
			}
			if (this.CurrentFileVersion >= 21)
			{
				int num4 = br.ReadInt32();
				for (int l = 0; l < num4; l++)
				{
					string b2 = br.ReadString();
					TwitchActionPreset twitchActionPreset = null;
					for (int m = 0; m < this.ActionPresets.Count; m++)
					{
						if (this.ActionPresets[m].Name == b2)
						{
							twitchActionPreset = this.ActionPresets[m];
							break;
						}
					}
					int num5 = br.ReadInt32();
					if (twitchActionPreset != null)
					{
						twitchActionPreset.AddedActions.Clear();
						twitchActionPreset.RemovedActions.Clear();
					}
					for (int n = 0; n < num5; n++)
					{
						if (twitchActionPreset != null)
						{
							twitchActionPreset.AddedActions.Add(br.ReadString());
						}
					}
					num5 = br.ReadInt32();
					for (int num6 = 0; num6 < num5; num6++)
					{
						if (twitchActionPreset != null)
						{
							twitchActionPreset.RemovedActions.Add(br.ReadString());
						}
					}
				}
			}
			if (this.CurrentFileVersion >= 22)
			{
				this.BitPriceMultiplier = br.ReadSingle();
			}
			if (this.CurrentFileVersion >= 23)
			{
				this.AllowCreatorGoalEvents = br.ReadBoolean();
			}
			if (this.CurrentFileVersion >= 24)
			{
				int num7 = br.ReadInt32();
				this.changedActionList.Clear();
				for (int num8 = 0; num8 < num7; num8++)
				{
					string text3 = br.ReadString();
					int modifiedCost = br.ReadInt32();
					if (TwitchActionManager.TwitchActions.ContainsKey(text3))
					{
						this.changedActionList.Add(text3);
						TwitchActionManager.TwitchActions[text3].ModifiedCost = modifiedCost;
					}
				}
			}
			if (this.CurrentFileVersion >= 25)
			{
				this.BitPot = br.ReadInt32();
				if (this.BitPot <= 0)
				{
					this.BitPot = 0;
				}
				if (this.BitPot > TwitchManager.LeaderboardStats.LargestBitPot)
				{
					TwitchManager.LeaderboardStats.LargestBitPot = this.BitPot;
				}
			}
			if (this.CurrentFileVersion >= 26)
			{
				this.BitPotPercentage = br.ReadSingle();
			}
		}

		// Token: 0x0600A930 RID: 43312 RVA: 0x0042D0B7 File Offset: 0x0042B2B7
		public void ReadSpecial(BinaryReader br)
		{
			this.ViewerData.ReadSpecial(br, 1);
		}

		// Token: 0x0600A931 RID: 43313 RVA: 0x0042D0C6 File Offset: 0x0042B2C6
		public void ReadMain(BinaryReader br)
		{
			this.CurrentMainFileVersion = br.ReadByte();
			this.HasViewedSettings = br.ReadBoolean();
			this.ViewerData.ReadSpecial(br, this.CurrentMainFileVersion);
		}

		// Token: 0x0600A932 RID: 43314 RVA: 0x0042D0F4 File Offset: 0x0042B2F4
		public void SendChannelPointOutputMessage(string name, ViewerEntry entry)
		{
			if (entry.SpecialPoints == 0f)
			{
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_PointsWithoutSpecial, name, entry.CombinedPoints), true);
				return;
			}
			this.ircClient.SendChannelMessage(string.Format(this.chatOutput_PointsWithSpecial, name, entry.CombinedPoints, entry.SpecialPoints), true);
		}

		// Token: 0x0600A933 RID: 43315 RVA: 0x0042D160 File Offset: 0x0042B360
		public void SendChannelCreditOutputMessage(string name, ViewerEntry entry)
		{
			this.ircClient.SendChannelMessage(string.Format(this.chatOutput_BitCredits, name, entry.BitCredits), true);
		}

		// Token: 0x0600A934 RID: 43316 RVA: 0x0042D185 File Offset: 0x0042B385
		public void SendChannelPointOutputMessage(string name)
		{
			this.SendChannelPointOutputMessage(name, this.ViewerData.GetViewerEntry(name));
		}

		// Token: 0x0600A935 RID: 43317 RVA: 0x0042D19A File Offset: 0x0042B39A
		public void SendChannelCreditOutputMessage(string name)
		{
			this.SendChannelCreditOutputMessage(name, this.ViewerData.GetViewerEntry(name));
		}

		// Token: 0x0600A936 RID: 43318 RVA: 0x0042D1AF File Offset: 0x0042B3AF
		public void SendChannelMessage(string message, bool useQueue = true)
		{
			this.ircClient.SendChannelMessage(message, useQueue);
		}

		// Token: 0x0600A937 RID: 43319 RVA: 0x0042D1C0 File Offset: 0x0042B3C0
		[PublicizedFrom(EAccessModifier.Private)]
		public void UpdateActionCooldowns(float modifier)
		{
			foreach (TwitchAction twitchAction in TwitchActionManager.TwitchActions.Values)
			{
				if (twitchAction.IsInPreset(this.CurrentActionPreset))
				{
					twitchAction.UpdateModifiedCooldown(modifier);
				}
			}
		}

		// Token: 0x0600A938 RID: 43320 RVA: 0x0042D228 File Offset: 0x0042B428
		public void SetupAvailableCommands()
		{
			this.AvailableCommands.Clear();
			this.AlternateCommands.Clear();
			TwitchAction[] array = (from a in TwitchActionManager.TwitchActions.Values
			where a.CanUse && a.IsInPreset(this.CurrentActionPreset)
			orderby a.Command
			orderby a.PointType
			select a).ToArray<TwitchAction>();
			List<string> list = null;
			for (int i = 0; i < array.Count<TwitchAction>(); i++)
			{
				TwitchAction twitchAction = array[i];
				if (this.UseProgression && !this.OverrideProgession)
				{
					int startGameStage = twitchAction.StartGameStage;
					if (startGameStage == -1 || startGameStage <= this.HighestGameStage)
					{
						if (twitchAction.Replaces != "")
						{
							string item = twitchAction.Replaces;
							if (this.AlternateCommands.ContainsKey(twitchAction.Replaces))
							{
								item = this.AlternateCommands[twitchAction.Replaces];
							}
							if (list == null)
							{
								list = new List<string>();
							}
							list.Add(item);
						}
						if (this.AvailableCommands.ContainsKey(twitchAction.Command))
						{
							this.AvailableCommands[twitchAction.Command] = twitchAction;
						}
						else
						{
							this.AvailableCommands.Add(twitchAction.Command, twitchAction);
						}
						if (!this.AlternateCommands.ContainsKey(twitchAction.BaseCommand))
						{
							this.AlternateCommands.Add(twitchAction.BaseCommand, twitchAction.Command);
						}
					}
				}
				else
				{
					if (twitchAction.RandomDaily)
					{
						twitchAction.AllowedDay = this.lastGameDay;
					}
					if (twitchAction.Replaces != "")
					{
						string item2 = twitchAction.Replaces;
						if (this.AlternateCommands.ContainsKey(twitchAction.Replaces))
						{
							item2 = this.AlternateCommands[twitchAction.Replaces];
						}
						if (list == null)
						{
							list = new List<string>();
						}
						list.Add(item2);
					}
					if (this.AvailableCommands.ContainsKey(twitchAction.Command))
					{
						this.AvailableCommands[twitchAction.Command] = twitchAction;
					}
					else
					{
						this.AvailableCommands.Add(twitchAction.Command, twitchAction);
					}
					if (!this.AlternateCommands.ContainsKey(twitchAction.BaseCommand))
					{
						this.AlternateCommands.Add(twitchAction.BaseCommand, twitchAction.Command);
					}
				}
			}
			if (list != null)
			{
				for (int j = 0; j < list.Count; j++)
				{
					string key = list[j];
					if (this.AvailableCommands.ContainsKey(key))
					{
						this.AvailableCommands.Remove(key);
					}
				}
			}
		}

		// Token: 0x0600A939 RID: 43321 RVA: 0x0042D4C4 File Offset: 0x0042B6C4
		public void SetupAvailableCommandsWithOutput(int lastGameStage, bool displayMessage)
		{
			this.AvailableCommands.Clear();
			this.AlternateCommands.Clear();
			StringBuilder stringBuilder = null;
			TwitchAction[] array = (from a in TwitchActionManager.TwitchActions.Values
			where a.CanUse && a.IsInPreset(this.CurrentActionPreset)
			orderby a.Command
			orderby a.PointType
			select a).ToArray<TwitchAction>();
			List<string> list = null;
			for (int i = 0; i < array.Count<TwitchAction>(); i++)
			{
				TwitchAction twitchAction = array[i];
				if (this.UseProgression && !this.OverrideProgession)
				{
					int startGameStage = twitchAction.StartGameStage;
					if (startGameStage == -1 || startGameStage <= this.HighestGameStage)
					{
						if (twitchAction.Replaces != "")
						{
							string item = twitchAction.Replaces;
							if (this.AlternateCommands.ContainsKey(twitchAction.Replaces))
							{
								item = this.AlternateCommands[twitchAction.Replaces];
							}
							if (list == null)
							{
								list = new List<string>();
							}
							list.Add(item);
						}
						if (this.AvailableCommands.ContainsKey(twitchAction.Command))
						{
							this.AvailableCommands[twitchAction.Command] = twitchAction;
							if (startGameStage > lastGameStage)
							{
								if (stringBuilder == null)
								{
									stringBuilder = new StringBuilder();
									stringBuilder.Append("*" + twitchAction.Command);
								}
								else
								{
									stringBuilder.Append(", " + twitchAction.Command);
								}
							}
						}
						else
						{
							this.AvailableCommands.Add(twitchAction.Command, twitchAction);
							if (startGameStage > lastGameStage)
							{
								if (stringBuilder == null)
								{
									stringBuilder = new StringBuilder();
									stringBuilder.Append(twitchAction.Command);
								}
								else
								{
									stringBuilder.Append(", " + twitchAction.Command);
								}
							}
						}
						if (!this.AlternateCommands.ContainsKey(twitchAction.BaseCommand))
						{
							this.AlternateCommands.Add(twitchAction.BaseCommand, twitchAction.Command);
						}
					}
				}
				else
				{
					if (twitchAction.RandomDaily)
					{
						twitchAction.AllowedDay = this.lastGameDay;
					}
					if (this.AvailableCommands.ContainsKey(twitchAction.Command))
					{
						this.AvailableCommands[twitchAction.Command] = twitchAction;
					}
					else
					{
						this.AvailableCommands.Add(twitchAction.Command, twitchAction);
					}
					if (!this.AlternateCommands.ContainsKey(twitchAction.BaseCommand))
					{
						this.AlternateCommands.Add(twitchAction.BaseCommand, twitchAction.Command);
					}
				}
			}
			if (list != null)
			{
				for (int j = 0; j < list.Count; j++)
				{
					string key = list[j];
					if (this.AvailableCommands.ContainsKey(key))
					{
						this.AvailableCommands.Remove(key);
					}
				}
			}
			if (displayMessage && stringBuilder != null && this.AllowActions && this.CurrentActionPreset.ShowNewCommands)
			{
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_NewActions, stringBuilder), true);
				Manager.BroadcastPlayByLocalPlayer(this.LocalPlayer.position, "twitch_new_commands");
			}
		}

		// Token: 0x0600A93A RID: 43322 RVA: 0x0042D7EC File Offset: 0x0042B9EC
		public void HandleCooldownActionLocking()
		{
			foreach (string key in this.AvailableCommands.Keys)
			{
				TwitchAction twitchAction = this.AvailableCommands[key];
				if (twitchAction.IsInPreset(this.CurrentActionPreset))
				{
					if (this.OnCooldown)
					{
						if (this.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || this.CooldownType == TwitchManager.CooldownTypes.Time)
						{
							twitchAction.OnCooldown = true;
						}
						else if (this.CooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting || this.CooldownType == TwitchManager.CooldownTypes.SafeCooldown)
						{
							twitchAction.OnCooldown = twitchAction.WaitingBlocked;
						}
						else
						{
							twitchAction.OnCooldown = twitchAction.CooldownBlocked;
						}
					}
					else
					{
						twitchAction.OnCooldown = false;
					}
				}
			}
			if (this.CommandsChanged != null)
			{
				this.CommandsChanged();
			}
		}

		// Token: 0x0600A93B RID: 43323 RVA: 0x0042D8C8 File Offset: 0x0042BAC8
		public void PushBalanceToExtensionQueue(string userID, int creditBalance)
		{
			if (this.extensionManager != null)
			{
				this.extensionManager.PushUserBalance(new ValueTuple<string, int>(userID, creditBalance));
			}
		}

		// Token: 0x0600A93C RID: 43324 RVA: 0x0042D8E4 File Offset: 0x0042BAE4
		public void DisplayActions()
		{
			int count = this.ActionMessages.Count;
			if (this.CurrentUnityTime > this.nextDisplayCommandsTime)
			{
				this.nextDisplayCommandsTime = this.CurrentUnityTime + 15f;
				this.ircClient.SendChannelMessages(this.ActionMessages, true);
			}
		}

		// Token: 0x0600A93D RID: 43325 RVA: 0x0042D924 File Offset: 0x0042BB24
		public void DisplayCommands(bool isBroadcaster, bool isMod, bool isVIP, bool isSub)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.TwitchCommandList.Count; i++)
			{
				if (!(this.TwitchCommandList[i] is TwitchCommandCommands))
				{
					bool flag = false;
					switch (BaseTwitchCommand.GetPermission(this.TwitchCommandList[i]))
					{
					case BaseTwitchCommand.PermissionLevels.Everyone:
						flag = true;
						break;
					case BaseTwitchCommand.PermissionLevels.VIP:
						flag = isVIP;
						break;
					case BaseTwitchCommand.PermissionLevels.Sub:
						flag = isSub;
						break;
					case BaseTwitchCommand.PermissionLevels.Mod:
						flag = isMod;
						break;
					case BaseTwitchCommand.PermissionLevels.Broadcaster:
						flag = isBroadcaster;
						break;
					}
					if (flag)
					{
						for (int j = 0; j < this.TwitchCommandList[i].LocalizedCommandNames.Length; j++)
						{
							if (stringBuilder.Length != 0)
							{
								stringBuilder.Append(", ");
							}
							stringBuilder.Append(this.TwitchCommandList[i].LocalizedCommandNames[j]);
						}
					}
				}
			}
			this.ircClient.SendChannelMessage(string.Format(this.chatOutput_Commands, stringBuilder.ToString()), true);
		}

		// Token: 0x0600A93E RID: 43326 RVA: 0x0042DA20 File Offset: 0x0042BC20
		public void AddTip(string tipname)
		{
			string text = Localization.Get(tipname, false);
			string item = Localization.Get(tipname + "Desc", false);
			if (text != "")
			{
				this.tipTitleList.Add(text);
				this.tipDescriptionList.Add(item);
			}
		}

		// Token: 0x0600A93F RID: 43327 RVA: 0x0042DA6C File Offset: 0x0042BC6C
		public void DisplayGameStage()
		{
			if (this.LocalPlayer != null)
			{
				this.ircClient.SendChannelMessage(string.Format(this.chatOutput_Gamestage, this.LocalPlayer.unModifiedGameStage), true);
			}
		}

		// Token: 0x0600A940 RID: 43328 RVA: 0x0042DAA3 File Offset: 0x0042BCA3
		public bool CheckIfTwitchKill(EntityPlayer player)
		{
			return this.twitchPlayerDeathsThisFrame.Contains(player);
		}

		// Token: 0x0600A941 RID: 43329 RVA: 0x0042DAB4 File Offset: 0x0042BCB4
		public bool LiveListContains(int entityID)
		{
			for (int i = 0; i < this.liveList.Count; i++)
			{
				if (this.liveList[i].SpawnedEntityID == entityID)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040082CE RID: 33486
		[PublicizedFrom(EAccessModifier.Private)]
		public static TwitchManager instance = null;

		// Token: 0x040082CF RID: 33487
		[PublicizedFrom(EAccessModifier.Private)]
		public const float SAVE_TIME_SEC = 30f;

		// Token: 0x040082D0 RID: 33488
		public float saveTime;

		// Token: 0x040082D1 RID: 33489
		[PublicizedFrom(EAccessModifier.Private)]
		public ThreadManager.ThreadInfo dataSaveThreadInfo;

		// Token: 0x040082D2 RID: 33490
		public static byte FileVersion = 26;

		// Token: 0x040082D4 RID: 33492
		public static byte MainFileVersion = 3;

		// Token: 0x040082D6 RID: 33494
		public TwitchIRCClient ircClient;

		// Token: 0x040082D7 RID: 33495
		public ExtensionManager extensionManager;

		// Token: 0x040082D8 RID: 33496
		[PublicizedFrom(EAccessModifier.Private)]
		public int resetClientAttempts;

		// Token: 0x040082D9 RID: 33497
		[PublicizedFrom(EAccessModifier.Private)]
		public bool overrideProgression;

		// Token: 0x040082DC RID: 33500
		[PublicizedFrom(EAccessModifier.Private)]
		public int commandsAvailable = -1;

		// Token: 0x040082DD RID: 33501
		public Dictionary<string, TwitchAction> AvailableCommands = new Dictionary<string, TwitchAction>();

		// Token: 0x040082DE RID: 33502
		public Dictionary<string, string> AlternateCommands = new Dictionary<string, string>();

		// Token: 0x040082DF RID: 33503
		public TwitchManager.PimpPotSettings PimpPotType = TwitchManager.PimpPotSettings.EnabledSP;

		// Token: 0x040082E0 RID: 33504
		public static int PimpPotDefault = 500;

		// Token: 0x040082E1 RID: 33505
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchManager.IntegrationSettings integrationSetting = TwitchManager.IntegrationSettings.Both;

		// Token: 0x040082E2 RID: 33506
		[PublicizedFrom(EAccessModifier.Private)]
		public float actionCooldownModifier = 1f;

		// Token: 0x040082E3 RID: 33507
		[PublicizedFrom(EAccessModifier.Private)]
		public const int HistoryItemMax = 500;

		// Token: 0x040082E4 RID: 33508
		public float ActionPotPercentage = 0.15f;

		// Token: 0x040082E5 RID: 33509
		public float BitPotPercentage = 0.25f;

		// Token: 0x040082E6 RID: 33510
		public int RewardPot = TwitchManager.PimpPotDefault;

		// Token: 0x040082E7 RID: 33511
		public int BitPot;

		// Token: 0x040082E8 RID: 33512
		public int PartyKillRewardMax = 250;

		// Token: 0x040082E9 RID: 33513
		public EntityPlayerLocal LocalPlayer;

		// Token: 0x040082EA RID: 33514
		public bool LocalPlayerInLandClaim;

		// Token: 0x040082EB RID: 33515
		public TwitchManager.CooldownTypes CooldownType = TwitchManager.CooldownTypes.Startup;

		// Token: 0x040082EC RID: 33516
		public float CooldownTime = 300f;

		// Token: 0x040082ED RID: 33517
		public float CurrentCooldownFill;

		// Token: 0x040082EE RID: 33518
		public float CooldownFillMax = 50f;

		// Token: 0x040082EF RID: 33519
		public int NextCooldownTime = 180;

		// Token: 0x040082F0 RID: 33520
		public bool AllowCrateSharing;

		// Token: 0x040082F1 RID: 33521
		public bool AllowBitEvents = true;

		// Token: 0x040082F2 RID: 33522
		public bool AllowSubEvents = true;

		// Token: 0x040082F3 RID: 33523
		public bool AllowGiftSubEvents = true;

		// Token: 0x040082F4 RID: 33524
		public bool AllowCharityEvents = true;

		// Token: 0x040082F5 RID: 33525
		public bool AllowRaidEvents = true;

		// Token: 0x040082F6 RID: 33526
		public bool AllowHypeTrainEvents = true;

		// Token: 0x040082F7 RID: 33527
		public bool AllowCreatorGoalEvents = true;

		// Token: 0x040082F8 RID: 33528
		public bool AllowChannelPointRedemptions = true;

		// Token: 0x040082F9 RID: 33529
		public List<CooldownPreset> CooldownPresets = new List<CooldownPreset>();

		// Token: 0x040082FA RID: 33530
		public int CooldownPresetIndex;

		// Token: 0x040082FB RID: 33531
		public CooldownPreset CurrentCooldownPreset;

		// Token: 0x040082FC RID: 33532
		public List<TwitchActionPreset> ActionPresets = new List<TwitchActionPreset>();

		// Token: 0x040082FD RID: 33533
		public List<TwitchVotePreset> VotePresets = new List<TwitchVotePreset>();

		// Token: 0x040082FE RID: 33534
		public List<TwitchEventPreset> EventPresets = new List<TwitchEventPreset>();

		// Token: 0x040082FF RID: 33535
		public int ActionPresetIndex;

		// Token: 0x04008300 RID: 33536
		public int VotePresetIndex;

		// Token: 0x04008301 RID: 33537
		public int EventPresetIndex;

		// Token: 0x04008302 RID: 33538
		public TwitchActionPreset CurrentActionPreset;

		// Token: 0x04008303 RID: 33539
		public TwitchVotePreset CurrentVotePreset;

		// Token: 0x04008304 RID: 33540
		public TwitchEventPreset CurrentEventPreset;

		// Token: 0x04008305 RID: 33541
		public bool UIDirty;

		// Token: 0x04008306 RID: 33542
		[PublicizedFrom(EAccessModifier.Private)]
		public float updateTime = 1f;

		// Token: 0x04008307 RID: 33543
		public float ExtensionCheckTime;

		// Token: 0x04008308 RID: 33544
		[PublicizedFrom(EAccessModifier.Private)]
		public World world;

		// Token: 0x04008309 RID: 33545
		public int lastGameDay = -1;

		// Token: 0x0400830A RID: 33546
		public int currentBMDayEnd = -1;

		// Token: 0x0400830B RID: 33547
		public int nextBMDay = -1;

		// Token: 0x0400830C RID: 33548
		public int BMCooldownStart;

		// Token: 0x0400830D RID: 33549
		public int BMCooldownEnd;

		// Token: 0x0400830E RID: 33550
		public int BitPointModifier = 1;

		// Token: 0x0400830F RID: 33551
		public int SubPointModifier = 1;

		// Token: 0x04008310 RID: 33552
		public int GiftSubPointModifier = 2;

		// Token: 0x04008311 RID: 33553
		public int RaidPointAdd = 1000;

		// Token: 0x04008312 RID: 33554
		public int RaidViewerMinimum = 10;

		// Token: 0x04008313 RID: 33555
		public int HypeTrainLevel;

		// Token: 0x04008314 RID: 33556
		[PublicizedFrom(EAccessModifier.Private)]
		public float bitPriceMultiplier = 1f;

		// Token: 0x04008315 RID: 33557
		public static TwitchLeaderboardStats LeaderboardStats = new TwitchLeaderboardStats();

		// Token: 0x04008316 RID: 33558
		public bool isBMActive;

		// Token: 0x04008317 RID: 33559
		public TwitchVoteLockTypes VoteLockedLevel;

		// Token: 0x04008318 RID: 33560
		public List<TwitchActionHistoryEntry> ActionHistory = new List<TwitchActionHistoryEntry>();

		// Token: 0x04008319 RID: 33561
		public List<TwitchActionHistoryEntry> VoteHistory = new List<TwitchActionHistoryEntry>();

		// Token: 0x0400831A RID: 33562
		public List<TwitchActionHistoryEntry> EventHistory = new List<TwitchActionHistoryEntry>();

		// Token: 0x0400831B RID: 33563
		public List<TwitchLeaderboardEntry> Leaderboard = new List<TwitchLeaderboardEntry>();

		// Token: 0x0400831C RID: 33564
		public List<TwitchRespawnEntry> RespawnEntries = new List<TwitchRespawnEntry>();

		// Token: 0x0400831D RID: 33565
		public int UseActionsDuringBloodmoon = 2;

		// Token: 0x0400831E RID: 33566
		public int UseActionsDuringQuests = 2;

		// Token: 0x0400831F RID: 33567
		public bool InitialCooldownSet;

		// Token: 0x04008322 RID: 33570
		public List<EntityPlayer> twitchPlayerDeathsThisFrame = new List<EntityPlayer>();

		// Token: 0x04008323 RID: 33571
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchManager.InitStates initState;

		// Token: 0x04008324 RID: 33572
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isLoaded;

		// Token: 0x04008325 RID: 33573
		public XUi LocalPlayerXUi;

		// Token: 0x04008326 RID: 33574
		public float CurrentUnityTime;

		// Token: 0x04008327 RID: 33575
		[PublicizedFrom(EAccessModifier.Private)]
		public bool resetCommandsNeeded;

		// Token: 0x04008328 RID: 33576
		[PublicizedFrom(EAccessModifier.Private)]
		public bool respawnEventNeeded;

		// Token: 0x04008329 RID: 33577
		[PublicizedFrom(EAccessModifier.Private)]
		public bool checkingExtensionInstalled;

		// Token: 0x0400832A RID: 33578
		[PublicizedFrom(EAccessModifier.Private)]
		public string broadcasterType = "";

		// Token: 0x0400832B RID: 33579
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchMessageEntry> inGameChatQueue = new List<TwitchMessageEntry>();

		// Token: 0x04008332 RID: 33586
		public TwitchAuthentication Authentication;

		// Token: 0x04008333 RID: 33587
		public EventSubClient EventSub;

		// Token: 0x04008334 RID: 33588
		public bool HasViewedSettings;

		// Token: 0x04008335 RID: 33589
		public string DeniedCrateEvent = "";

		// Token: 0x04008336 RID: 33590
		public string StealingCrateEvent = "";

		// Token: 0x04008337 RID: 33591
		public string PartyRespawnEvent = "";

		// Token: 0x04008338 RID: 33592
		public string OnPlayerDeathEvent = "";

		// Token: 0x04008339 RID: 33593
		public string OnPlayerRespawnEvent = "";

		// Token: 0x0400833A RID: 33594
		public List<string> tipTitleList = new List<string>();

		// Token: 0x0400833B RID: 33595
		public List<string> tipDescriptionList = new List<string>();

		// Token: 0x0400833C RID: 33596
		[PublicizedFrom(EAccessModifier.Private)]
		public int extensionActiveCheckFailures;

		// Token: 0x0400833D RID: 33597
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_ActivatedAction;

		// Token: 0x0400833E RID: 33598
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_ActivatedBitAction;

		// Token: 0x0400833F RID: 33599
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_BitCredits;

		// Token: 0x04008340 RID: 33600
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_BitEvent;

		// Token: 0x04008341 RID: 33601
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_BitPotBalance;

		// Token: 0x04008342 RID: 33602
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_ChannelPointEvent;

		// Token: 0x04008343 RID: 33603
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_CharityEvent;

		// Token: 0x04008344 RID: 33604
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_CooldownComplete;

		// Token: 0x04008345 RID: 33605
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_CooldownStarted;

		// Token: 0x04008346 RID: 33606
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_CooldownTime;

		// Token: 0x04008347 RID: 33607
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_Commands;

		// Token: 0x04008348 RID: 33608
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_CreatorGoalEvent;

		// Token: 0x04008349 RID: 33609
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_DonateBits;

		// Token: 0x0400834A RID: 33610
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_DonateCharity;

		// Token: 0x0400834B RID: 33611
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_Gamestage;

		// Token: 0x0400834C RID: 33612
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_GiftSubEvent;

		// Token: 0x0400834D RID: 33613
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_GiftSubs;

		// Token: 0x0400834E RID: 33614
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_HypeTrainEvent;

		// Token: 0x0400834F RID: 33615
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_KilledParty;

		// Token: 0x04008350 RID: 33616
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_KilledStreamer;

		// Token: 0x04008351 RID: 33617
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_KilledByBits;

		// Token: 0x04008352 RID: 33618
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_KilledByHypeTrain;

		// Token: 0x04008353 RID: 33619
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_KilledByVote;

		// Token: 0x04008354 RID: 33620
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_NewActions;

		// Token: 0x04008355 RID: 33621
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_PimpPotBalance;

		// Token: 0x04008356 RID: 33622
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_PointsWithSpecial;

		// Token: 0x04008357 RID: 33623
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_PointsWithoutSpecial;

		// Token: 0x04008358 RID: 33624
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_QueuedBitAction;

		// Token: 0x04008359 RID: 33625
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_RaidEvent;

		// Token: 0x0400835A RID: 33626
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_RaidPoints;

		// Token: 0x0400835B RID: 33627
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_SubEvent;

		// Token: 0x0400835C RID: 33628
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_Subscribed;

		// Token: 0x0400835D RID: 33629
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_ActivatedAction;

		// Token: 0x0400835E RID: 33630
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_BitRespawns;

		// Token: 0x0400835F RID: 33631
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_DonateBits;

		// Token: 0x04008360 RID: 33632
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_DonateCharity;

		// Token: 0x04008361 RID: 33633
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_GiftSubs;

		// Token: 0x04008362 RID: 33634
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_KilledParty;

		// Token: 0x04008363 RID: 33635
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_KilledStreamer;

		// Token: 0x04008364 RID: 33636
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_KilledByBits;

		// Token: 0x04008365 RID: 33637
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_KilledByHypeTrain;

		// Token: 0x04008366 RID: 33638
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_KilledByVote;

		// Token: 0x04008367 RID: 33639
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_RaidPoints;

		// Token: 0x04008368 RID: 33640
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_RefundedAction;

		// Token: 0x04008369 RID: 33641
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_Subscribed;

		// Token: 0x0400836A RID: 33642
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameDeathScreen_Message;

		// Token: 0x0400836B RID: 33643
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameBitsDeathScreen_Message;

		// Token: 0x0400836C RID: 33644
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameHypeTrainDeathScreen_Message;

		// Token: 0x0400836D RID: 33645
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameVoteDeathScreen_Message;

		// Token: 0x0400836E RID: 33646
		[PublicizedFrom(EAccessModifier.Private)]
		public string subPointDisplay;

		// Token: 0x0400836F RID: 33647
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<string, TwitchRandomActionGroup> randomGroups = new Dictionary<string, TwitchRandomActionGroup>();

		// Token: 0x04008370 RID: 33648
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<string, List<TwitchAction>> randomKeys = new Dictionary<string, List<TwitchAction>>();

		// Token: 0x04008371 RID: 33649
		public List<BaseTwitchCommand> TwitchCommandList = new List<BaseTwitchCommand>();

		// Token: 0x04008372 RID: 33650
		[PublicizedFrom(EAccessModifier.Protected)]
		public Dictionary<EntityPlayer, TwitchManager.TwitchPartyMemberInfo> PartyInfo = new Dictionary<EntityPlayer, TwitchManager.TwitchPartyMemberInfo>();

		// Token: 0x04008373 RID: 33651
		[PublicizedFrom(EAccessModifier.Private)]
		public bool lastAlive;

		// Token: 0x04008374 RID: 33652
		public static string DeathText = "";

		// Token: 0x04008375 RID: 33653
		[PublicizedFrom(EAccessModifier.Private)]
		public bool twitchActive = true;

		// Token: 0x04008376 RID: 33654
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchActionEntry> QueuedActionEntries = new List<TwitchActionEntry>();

		// Token: 0x04008377 RID: 33655
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchActionEntry> LiveActionEntries = new List<TwitchActionEntry>();

		// Token: 0x04008378 RID: 33656
		public List<TwitchEventActionEntry> EventQueue = new List<TwitchEventActionEntry>();

		// Token: 0x04008379 RID: 33657
		public List<TwitchEventActionEntry> LiveEvents = new List<TwitchEventActionEntry>();

		// Token: 0x0400837A RID: 33658
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchSpawnedEntityEntry> liveList = new List<TwitchSpawnedEntityEntry>();

		// Token: 0x0400837B RID: 33659
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchSpawnedBlocksEntry> liveBlockList = new List<TwitchSpawnedBlocksEntry>();

		// Token: 0x0400837C RID: 33660
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchRecentlyRemovedEntityEntry> recentlyDeadList = new List<TwitchRecentlyRemovedEntityEntry>();

		// Token: 0x0400837D RID: 33661
		public List<TwitchSpawnedEntityEntry> actionSpawnLiveList = new List<TwitchSpawnedEntityEntry>();

		// Token: 0x0400837E RID: 33662
		public bool HasDataChanges;

		// Token: 0x0400837F RID: 33663
		public List<string> changedActionList = new List<string>();

		// Token: 0x04008380 RID: 33664
		public List<string> changedEnabledVoteList = new List<string>();

		// Token: 0x04008381 RID: 33665
		[PublicizedFrom(EAccessModifier.Private)]
		public List<string> ActionMessages = new List<string>();

		// Token: 0x04008382 RID: 33666
		[PublicizedFrom(EAccessModifier.Private)]
		public float nextDisplayCommandsTime;

		// Token: 0x02001572 RID: 5490
		public enum PimpPotSettings
		{
			// Token: 0x04008384 RID: 33668
			Disabled,
			// Token: 0x04008385 RID: 33669
			EnabledSP,
			// Token: 0x04008386 RID: 33670
			EnabledPP
		}

		// Token: 0x02001573 RID: 5491
		public enum IntegrationSettings
		{
			// Token: 0x04008388 RID: 33672
			ExtensionOnly,
			// Token: 0x04008389 RID: 33673
			Both
		}

		// Token: 0x02001574 RID: 5492
		public enum CooldownTypes
		{
			// Token: 0x0400838B RID: 33675
			None,
			// Token: 0x0400838C RID: 33676
			Startup,
			// Token: 0x0400838D RID: 33677
			Time,
			// Token: 0x0400838E RID: 33678
			MaxReached,
			// Token: 0x0400838F RID: 33679
			MaxReachedWaiting,
			// Token: 0x04008390 RID: 33680
			BloodMoonDisabled,
			// Token: 0x04008391 RID: 33681
			BloodMoonCooldown,
			// Token: 0x04008392 RID: 33682
			QuestDisabled,
			// Token: 0x04008393 RID: 33683
			QuestCooldown,
			// Token: 0x04008394 RID: 33684
			SafeCooldown,
			// Token: 0x04008395 RID: 33685
			SafeCooldownExit
		}

		// Token: 0x02001575 RID: 5493
		public enum InitStates
		{
			// Token: 0x04008397 RID: 33687
			Setup,
			// Token: 0x04008398 RID: 33688
			None,
			// Token: 0x04008399 RID: 33689
			WaitingForPermission,
			// Token: 0x0400839A RID: 33690
			PermissionDenied,
			// Token: 0x0400839B RID: 33691
			WaitingForOAuth,
			// Token: 0x0400839C RID: 33692
			Authenticating,
			// Token: 0x0400839D RID: 33693
			Authenticated,
			// Token: 0x0400839E RID: 33694
			CheckingForExtension,
			// Token: 0x0400839F RID: 33695
			Ready,
			// Token: 0x040083A0 RID: 33696
			ExtensionNotInstalled,
			// Token: 0x040083A1 RID: 33697
			Failed
		}

		// Token: 0x02001576 RID: 5494
		public class TwitchPartyMemberInfo
		{
			// Token: 0x040083A2 RID: 33698
			public bool LastOptedOut;

			// Token: 0x040083A3 RID: 33699
			public bool LastAlive = true;

			// Token: 0x040083A4 RID: 33700
			public float Cooldown;

			// Token: 0x040083A5 RID: 33701
			public bool NeedsRespawnEvent;
		}
	}
}
