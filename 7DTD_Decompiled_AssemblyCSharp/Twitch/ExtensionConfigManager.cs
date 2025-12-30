using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniLinq;
using UnityEngine;
using UnityEngine.Networking;

namespace Twitch
{
	// Token: 0x020014F9 RID: 5369
	public class ExtensionConfigManager
	{
		// Token: 0x0600A61D RID: 42525 RVA: 0x0041A5E4 File Offset: 0x004187E4
		public void Init()
		{
			TwitchManager.Current.CommandsChanged += this.pushConfig;
			TwitchVotingManager votingManager = TwitchManager.Current.VotingManager;
			votingManager.VoteEventEnded = (OnGameEventVoteAction)Delegate.Combine(votingManager.VoteEventEnded, new OnGameEventVoteAction(this.pushConfig));
			TwitchVotingManager votingManager2 = TwitchManager.Current.VotingManager;
			votingManager2.VoteStarted = (OnGameEventVoteAction)Delegate.Combine(votingManager2.VoteStarted, new OnGameEventVoteAction(this.pushConfig));
			this.pushConfig();
		}

		// Token: 0x0600A61E RID: 42526 RVA: 0x0041A663 File Offset: 0x00418863
		public bool UpdatedConfig()
		{
			if (this.hasUpdated)
			{
				this.hasUpdated = false;
				return true;
			}
			return false;
		}

		// Token: 0x0600A61F RID: 42527 RVA: 0x0041A678 File Offset: 0x00418878
		public void Cleanup()
		{
			TwitchManager.Current.CommandsChanged -= this.pushConfig;
			TwitchVotingManager votingManager = TwitchManager.Current.VotingManager;
			votingManager.VoteEventEnded = (OnGameEventVoteAction)Delegate.Remove(votingManager.VoteEventEnded, new OnGameEventVoteAction(this.pushConfig));
			TwitchVotingManager votingManager2 = TwitchManager.Current.VotingManager;
			votingManager2.VoteStarted = (OnGameEventVoteAction)Delegate.Remove(votingManager2.VoteStarted, new OnGameEventVoteAction(this.pushConfig));
		}

		// Token: 0x0600A620 RID: 42528 RVA: 0x0041A6F1 File Offset: 0x004188F1
		public void OnPartyChanged()
		{
			this.pushConfig();
		}

		// Token: 0x0600A621 RID: 42529 RVA: 0x0041A6F9 File Offset: 0x004188F9
		[PublicizedFrom(EAccessModifier.Private)]
		public void pushConfig()
		{
			this.lastPushTime = Time.time;
			if (!this.waitingToPush)
			{
				this.waitingToPush = true;
				GameManager.Instance.StartCoroutine(this.pushConfigAfterTimeout());
			}
		}

		// Token: 0x0600A622 RID: 42530 RVA: 0x0041A726 File Offset: 0x00418926
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator pushConfigAfterTimeout()
		{
			while (Time.time - this.lastPushTime < 1f)
			{
				yield return null;
			}
			this.waitingToPush = false;
			yield return this.UpdateConfig();
			yield break;
		}

		// Token: 0x0600A623 RID: 42531 RVA: 0x0041A735 File Offset: 0x00418935
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator UpdateConfig()
		{
			if (this.displayName == string.Empty)
			{
				yield return this.GetDisplayName();
			}
			if (TwitchManager.Current == null || TwitchManager.Current.Authentication == null)
			{
				Log.Warning("attempted to updated config with no Auth object");
				yield break;
			}
			Dictionary<string, List<ExtensionConfigManager.CommandModel>> commands = this.GetCommands();
			List<string> activeCategories = this.GetActiveCategories(commands.Keys.ToList<string>());
			string bodyData = JsonConvert.SerializeObject(new ExtensionConfigManager.ConfigModel
			{
				displayName = TwitchManager.Current.Authentication.userName,
				party = this.GetPlayers(),
				categories = activeCategories,
				commands = commands
			});
			using (UnityWebRequest req = UnityWebRequest.Put("https://2v3d0ewjcg.execute-api.us-east-1.amazonaws.com/prod/broadcaster/config", bodyData))
			{
				req.SetRequestHeader("Authorization", TwitchManager.Current.Authentication.userID + " " + TwitchManager.Current.Authentication.oauth.Substring(6));
				req.SetRequestHeader("Content-Type", "application/json");
				yield return req.SendWebRequest();
				if (req.result != UnityWebRequest.Result.Success)
				{
					Log.Warning(string.Format("Could not update config on backend: {0}", req.result));
				}
				else
				{
					Log.Out("Successfully updated broadcaster config");
					this.hasUpdated = true;
				}
			}
			UnityWebRequest req = null;
			yield break;
			yield break;
		}

		// Token: 0x0600A624 RID: 42532 RVA: 0x0041A744 File Offset: 0x00418944
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetDisplayName()
		{
			using (UnityWebRequest req = UnityWebRequest.Get("https://api.twitch.tv/helix/users"))
			{
				req.SetRequestHeader("Content-Type", "application/json");
				req.SetRequestHeader("Client-Id", TwitchAuthentication.client_id);
				req.SetRequestHeader("Authorization", "Bearer " + TwitchManager.Current.Authentication.oauth.Substring(6));
				yield return req.SendWebRequest();
				if (req.result != UnityWebRequest.Result.Success)
				{
					Log.Warning(string.Format("Could not get user data from Twitch: {0}", req.result));
				}
				else
				{
					Log.Out("Successfully retrieved user data from Twitch");
					JObject jobject = JObject.Parse(req.downloadHandler.text);
					this.displayName = jobject["data"][0]["display_name"].ToString();
					this.broadcasterType = jobject["data"][0]["broadcaster_type"].ToString();
				}
			}
			UnityWebRequest req = null;
			yield break;
			yield break;
		}

		// Token: 0x0600A625 RID: 42533 RVA: 0x0041A754 File Offset: 0x00418954
		[PublicizedFrom(EAccessModifier.Private)]
		public List<string> GetActiveCategories(List<string> categories)
		{
			return (from c in TwitchActionManager.Current.CategoryList
			select c.Name into name
			where categories.Contains(name)
			select name).ToList<string>();
		}

		// Token: 0x0600A626 RID: 42534 RVA: 0x0041A7B4 File Offset: 0x004189B4
		[PublicizedFrom(EAccessModifier.Private)]
		public List<string> GetPlayers()
		{
			if (TwitchManager.Current.LocalPlayer != null && TwitchManager.Current.LocalPlayer.Party != null && TwitchManager.Current.LocalPlayer.Party.MemberList != null)
			{
				return (from m in TwitchManager.Current.LocalPlayer.Party.MemberList
				where !(m is EntityPlayerLocal) && !m.TwitchEnabled
				select m into e
				select e.EntityName).ToList<string>();
			}
			return ExtensionConfigManager.emptyParty;
		}

		// Token: 0x0600A627 RID: 42535 RVA: 0x0041A864 File Offset: 0x00418A64
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<string, List<ExtensionConfigManager.CommandModel>> GetCommands()
		{
			Dictionary<string, List<ExtensionConfigManager.CommandModel>> dictionary = new Dictionary<string, List<ExtensionConfigManager.CommandModel>>();
			TwitchAction[] array = (from a in TwitchManager.Current.AvailableCommands.Values
			where a.HasExtraConditions() && (this.CanUseBitCommands() || a.PointType != TwitchAction.PointTypes.Bits)
			select a).ToArray<TwitchAction>();
			int num = 0;
			foreach (TwitchAction twitchAction in array)
			{
				ExtensionConfigManager.CommandModel item = new ExtensionConfigManager.CommandModel
				{
					name = twitchAction.Command.Replace("#", string.Empty).Replace("_", " ").ToUpper(),
					baseCommand = twitchAction.BaseCommand,
					command = twitchAction.Command,
					isPositive = twitchAction.IsPositive,
					spends = twitchAction.PointType.ToString(),
					cost = twitchAction.CurrentCost,
					cooldownType = (twitchAction.WaitingBlocked ? "wait" : (twitchAction.CooldownBlocked ? "regular" : "full")),
					cooldownIndex = num / 32,
					bitPosition = (byte)(num % 32),
					streamerOnly = twitchAction.StreamerOnly
				};
				List<ExtensionConfigManager.CommandModel> list;
				if (!dictionary.TryGetValue(twitchAction.MainCategory.Name, out list))
				{
					dictionary.Add(twitchAction.MainCategory.Name, list = new List<ExtensionConfigManager.CommandModel>());
				}
				list.Add(item);
				num++;
			}
			return dictionary;
		}

		// Token: 0x0600A628 RID: 42536 RVA: 0x0041A9C6 File Offset: 0x00418BC6
		public bool CanUseBitCommands()
		{
			return this.broadcasterType == "affiliate" || this.broadcasterType == "partner";
		}

		// Token: 0x0400804B RID: 32843
		[PublicizedFrom(EAccessModifier.Private)]
		public static List<string> emptyParty = new List<string>(0);

		// Token: 0x0400804C RID: 32844
		[PublicizedFrom(EAccessModifier.Private)]
		public string displayName = string.Empty;

		// Token: 0x0400804D RID: 32845
		[PublicizedFrom(EAccessModifier.Private)]
		public string broadcasterType = string.Empty;

		// Token: 0x0400804E RID: 32846
		[PublicizedFrom(EAccessModifier.Private)]
		public string jwt = string.Empty;

		// Token: 0x0400804F RID: 32847
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasUpdated;

		// Token: 0x04008050 RID: 32848
		[PublicizedFrom(EAccessModifier.Private)]
		public const float pushConfigTimeout = 1f;

		// Token: 0x04008051 RID: 32849
		[PublicizedFrom(EAccessModifier.Private)]
		public float lastPushTime = float.NegativeInfinity;

		// Token: 0x04008052 RID: 32850
		[PublicizedFrom(EAccessModifier.Private)]
		public bool waitingToPush;

		// Token: 0x020014FA RID: 5370
		public class ConfigModel
		{
			// Token: 0x04008053 RID: 32851
			public string displayName;

			// Token: 0x04008054 RID: 32852
			public List<string> party;

			// Token: 0x04008055 RID: 32853
			public List<string> categories;

			// Token: 0x04008056 RID: 32854
			public string IdentityGrantHeader = Localization.Get("TwitchInfo_IdentityGrantHeader", false);

			// Token: 0x04008057 RID: 32855
			public string IdentityGrantSubtext = Localization.Get("TwitchInfo_IdentityGrantSubtext", false);

			// Token: 0x04008058 RID: 32856
			public string LoadingText = Localization.Get("loadActionLoading", false);

			// Token: 0x04008059 RID: 32857
			public string OfflineHeader = Localization.Get("TwitchInfo_OfflineHeader", false);

			// Token: 0x0400805A RID: 32858
			public string OfflineSubtext1 = Localization.Get("TwitchInfo_OfflineSubtext1", false);

			// Token: 0x0400805B RID: 32859
			public string OfflineSubtext2 = Localization.Get("TwitchInfo_OfflineSubtext2", false);

			// Token: 0x0400805C RID: 32860
			public string CommandDescriptionsText = Localization.Get("TwitchInfo_CommandDescriptionsText", false);

			// Token: 0x0400805D RID: 32861
			public string ChatPromptHeader = Localization.Get("TwitchInfo_ChatPromptHeader", false);

			// Token: 0x0400805E RID: 32862
			public string ChatPromptSubtext = Localization.Get("TwitchInfo_ChatPromptSubtext", false);

			// Token: 0x0400805F RID: 32863
			public string ActionsOffText = Localization.Get("TwitchInfo_ActionsOffText", false);

			// Token: 0x04008060 RID: 32864
			public string CooldownText = Localization.Get("TwitchInfo_CooldownText", false);

			// Token: 0x04008061 RID: 32865
			public string PausedText = Localization.Get("TwitchCooldownStatus_Paused", false);

			// Token: 0x04008062 RID: 32866
			public string ActionPresetLabel = Localization.Get("xuiOptionsTwitchActionPreset", false);

			// Token: 0x04008063 RID: 32867
			public string VotePresetLabel = Localization.Get("xuiOptionsTwitchVotePreset", false);

			// Token: 0x04008064 RID: 32868
			public string EventPresetLabel = Localization.Get("xuiOptionsTwitchCustomEvents", false);

			// Token: 0x04008065 RID: 32869
			public string TopKillerLabel = Localization.Get("TwitchInfo_TopKiller", false);

			// Token: 0x04008066 RID: 32870
			public string TopGoodLabel = Localization.Get("TwitchInfo_TopGood", false);

			// Token: 0x04008067 RID: 32871
			public string TopBadLabel = Localization.Get("TwitchInfo_TopEvil", false);

			// Token: 0x04008068 RID: 32872
			public string BestHelperLabel = string.Format(Localization.Get("TwitchInfo_CurrentGood", false), TwitchManager.LeaderboardStats.GoodRewardTime);

			// Token: 0x04008069 RID: 32873
			public string TotalGoodActionsLabel = Localization.Get("TwitchInfo_TotalGood", false);

			// Token: 0x0400806A RID: 32874
			public string TotalBadActionsLabel = Localization.Get("TwitchInfo_TotalBad", false);

			// Token: 0x0400806B RID: 32875
			public string LargestPimpPotLabel = Localization.Get("TwitchInfo_LargestPimpPot", false);

			// Token: 0x0400806C RID: 32876
			public string DifficultyLabel = Localization.Get("goDifficultyShort", false);

			// Token: 0x0400806D RID: 32877
			public string DayCycleLabel = Localization.Get("goDayLength", false);

			// Token: 0x0400806E RID: 32878
			public string ModdedLabel = Localization.Get("goModded", false);

			// Token: 0x0400806F RID: 32879
			public string PPRateLabel = Localization.Get("TwitchInfo_PPRate", false);

			// Token: 0x04008070 RID: 32880
			public Dictionary<string, List<ExtensionConfigManager.CommandModel>> commands;
		}

		// Token: 0x020014FB RID: 5371
		public class CommandModel
		{
			// Token: 0x04008071 RID: 32881
			public string name;

			// Token: 0x04008072 RID: 32882
			public string baseCommand;

			// Token: 0x04008073 RID: 32883
			public string command;

			// Token: 0x04008074 RID: 32884
			public bool isPositive;

			// Token: 0x04008075 RID: 32885
			public string spends;

			// Token: 0x04008076 RID: 32886
			public int cost;

			// Token: 0x04008077 RID: 32887
			public string cooldownType;

			// Token: 0x04008078 RID: 32888
			public int cooldownIndex;

			// Token: 0x04008079 RID: 32889
			public byte bitPosition;

			// Token: 0x0400807A RID: 32890
			public bool streamerOnly;
		}
	}
}
