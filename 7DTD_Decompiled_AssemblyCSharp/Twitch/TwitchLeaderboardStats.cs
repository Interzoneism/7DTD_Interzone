using System;
using System.Collections.Generic;
using Challenges;
using UniLinq;

namespace Twitch
{
	// Token: 0x0200156B RID: 5483
	public class TwitchLeaderboardStats
	{
		// Token: 0x140000FD RID: 253
		// (add) Token: 0x0600A852 RID: 43090 RVA: 0x00424084 File Offset: 0x00422284
		// (remove) Token: 0x0600A853 RID: 43091 RVA: 0x004240BC File Offset: 0x004222BC
		public event OnLeaderboardStatsChanged StatsChanged;

		// Token: 0x140000FE RID: 254
		// (add) Token: 0x0600A854 RID: 43092 RVA: 0x004240F4 File Offset: 0x004222F4
		// (remove) Token: 0x0600A855 RID: 43093 RVA: 0x0042412C File Offset: 0x0042232C
		public event OnLeaderboardStatsChanged LeaderboardChanged;

		// Token: 0x170012B1 RID: 4785
		// (get) Token: 0x0600A856 RID: 43094 RVA: 0x00424161 File Offset: 0x00422361
		// (set) Token: 0x0600A857 RID: 43095 RVA: 0x00424169 File Offset: 0x00422369
		public int GoodRewardTime
		{
			get
			{
				return this.goodRewardTime;
			}
			set
			{
				if (this.goodRewardTime != value)
				{
					this.goodRewardTime = value;
					this.nextGoodTime = (float)(this.goodRewardTime * 60);
				}
			}
		}

		// Token: 0x0600A858 RID: 43096 RVA: 0x0042418B File Offset: 0x0042238B
		public void SetupLocalization()
		{
			this.chatOutput_GoodReward = Localization.Get("TwitchChat_GoodReward", false);
			this.ingameOutput_GoodReward = Localization.Get("TwitchInGame_GoodReward", false);
		}

		// Token: 0x0600A859 RID: 43097 RVA: 0x004241AF File Offset: 0x004223AF
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleStatsChanged()
		{
			if (this.StatsChanged != null)
			{
				this.StatsChanged();
			}
		}

		// Token: 0x0600A85A RID: 43098 RVA: 0x004241C4 File Offset: 0x004223C4
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleLeaderboardChanged()
		{
			if (this.LeaderboardChanged != null)
			{
				this.LeaderboardChanged();
			}
		}

		// Token: 0x0600A85B RID: 43099 RVA: 0x004241DC File Offset: 0x004223DC
		public void UpdateStats(float deltaTime)
		{
			if (this.CurrentGoodDirty)
			{
				this.lastTime -= deltaTime;
				if (this.lastTime <= 0f)
				{
					List<TwitchLeaderboardStats.StatEntry> list = (from entry in this.StatEntries.Values
					where entry.CurrentGoodActions > 0
					orderby entry.CurrentGoodActions descending
					select entry).ToList<TwitchLeaderboardStats.StatEntry>();
					if (list.Count > 0)
					{
						if (this.CurrentGoodViewer != list[0])
						{
							this.CurrentGoodViewer = list[0];
							this.HandleStatsChanged();
						}
					}
					else if (this.CurrentGoodViewer != null)
					{
						this.CurrentGoodViewer = null;
						this.HandleStatsChanged();
					}
					this.CurrentGoodDirty = false;
					this.lastTime = 1f;
				}
			}
			if (this.nextGoodTime == -1f)
			{
				this.nextGoodTime = (float)(this.GoodRewardTime * 60);
			}
			if (this.CurrentGoodViewer == null)
			{
				return;
			}
			this.nextGoodTime -= deltaTime;
			if (this.nextGoodTime <= 0f)
			{
				TwitchManager twitchManager = TwitchManager.Current;
				if (twitchManager.CurrentActionPreset.UseHelperReward && this.CurrentGoodViewer != null)
				{
					ViewerEntry viewerEntry = TwitchManager.Current.ViewerData.GetViewerEntry(this.CurrentGoodViewer.Name.ToLower());
					viewerEntry.StandardPoints += (float)this.GoodRewardAmount;
					twitchManager.ircClient.SendChannelMessage(string.Format(this.chatOutput_GoodReward, new object[]
					{
						this.CurrentGoodViewer.Name,
						viewerEntry.CombinedPoints,
						this.GoodRewardAmount,
						Localization.Get("TwitchPoints_PP", false)
					}), true);
					twitchManager.AddToInGameChatQueue(string.Format(this.ingameOutput_GoodReward, new object[]
					{
						viewerEntry.UserColor,
						this.CurrentGoodViewer.Name,
						this.GoodRewardAmount,
						Localization.Get("TwitchPoints_PP", false)
					}), null);
					twitchManager.LocalPlayer.PlayOneShot("twitch_top_helper", false, false, false, null);
					this.ClearAllCurrentGood();
					QuestEventManager.Current.TwitchEventReceived(TwitchObjectiveTypes.HelperReward, "");
				}
				this.nextGoodTime = (float)(this.GoodRewardTime * 60);
			}
		}

		// Token: 0x0600A85C RID: 43100 RVA: 0x00424432 File Offset: 0x00422632
		public void CheckTopKiller(TwitchLeaderboardStats.StatEntry newData)
		{
			if (this.TopKillerViewer == null || this.TopKillerViewer.Kills < newData.Kills)
			{
				this.TopKillerViewer = newData;
				this.HandleStatsChanged();
				return;
			}
			if (this.TopKillerViewer == newData)
			{
				this.HandleStatsChanged();
			}
		}

		// Token: 0x0600A85D RID: 43101 RVA: 0x0042446C File Offset: 0x0042266C
		public void CheckTopGood(TwitchLeaderboardStats.StatEntry newData)
		{
			if (this.TopGoodViewer == null || this.TopGoodViewer.GoodActions < newData.GoodActions)
			{
				this.TopGoodViewer = newData;
				this.HandleStatsChanged();
			}
			else if (this.TopGoodViewer == newData)
			{
				this.HandleStatsChanged();
			}
			this.HandleLeaderboardChanged();
			this.CurrentGoodDirty = true;
		}

		// Token: 0x0600A85E RID: 43102 RVA: 0x004244C0 File Offset: 0x004226C0
		public void CheckTopBad(TwitchLeaderboardStats.StatEntry newData)
		{
			if (this.TopBadViewer == null || this.TopBadViewer.BadActions < newData.BadActions)
			{
				this.TopBadViewer = newData;
				this.HandleStatsChanged();
			}
			else if (this.TopBadViewer == newData)
			{
				this.HandleStatsChanged();
			}
			this.HandleLeaderboardChanged();
			this.CurrentGoodDirty = true;
		}

		// Token: 0x0600A85F RID: 43103 RVA: 0x00424514 File Offset: 0x00422714
		public void CheckMostBitsSpent(TwitchLeaderboardStats.StatEntry newData)
		{
			if (this.MostBitsSpentViewer == null || this.MostBitsSpentViewer.BitsUsed < newData.BitsUsed)
			{
				this.MostBitsSpentViewer = newData;
				this.HandleStatsChanged();
			}
			else if (this.MostBitsSpentViewer == newData)
			{
				this.HandleStatsChanged();
			}
			this.HandleLeaderboardChanged();
		}

		// Token: 0x0600A860 RID: 43104 RVA: 0x00424560 File Offset: 0x00422760
		public TwitchLeaderboardStats.StatEntry AddKill(string name, string userColor)
		{
			if (!this.StatEntries.ContainsKey(name))
			{
				this.StatEntries.Add(name, new TwitchLeaderboardStats.StatEntry());
			}
			TwitchLeaderboardStats.StatEntry statEntry = this.StatEntries[name];
			statEntry.Name = name;
			statEntry.UserColor = userColor;
			statEntry.Kills++;
			return statEntry;
		}

		// Token: 0x0600A861 RID: 43105 RVA: 0x004245B4 File Offset: 0x004227B4
		public TwitchLeaderboardStats.StatEntry AddGoodActionUsed(string name, string userColor, bool isBits)
		{
			if (!this.StatEntries.ContainsKey(name))
			{
				this.StatEntries.Add(name, new TwitchLeaderboardStats.StatEntry());
			}
			int num = isBits ? 2 : 1;
			TwitchLeaderboardStats.StatEntry statEntry = this.StatEntries[name];
			statEntry.Name = name;
			statEntry.UserColor = userColor;
			statEntry.GoodActions += num;
			statEntry.CurrentGoodActions += num;
			statEntry.CurrentActions++;
			return statEntry;
		}

		// Token: 0x0600A862 RID: 43106 RVA: 0x0042462C File Offset: 0x0042282C
		public TwitchLeaderboardStats.StatEntry AddBadActionUsed(string name, string userColor, bool isBits)
		{
			if (!this.StatEntries.ContainsKey(name))
			{
				this.StatEntries.Add(name, new TwitchLeaderboardStats.StatEntry());
			}
			int num = isBits ? 2 : 1;
			TwitchLeaderboardStats.StatEntry statEntry = this.StatEntries[name];
			statEntry.Name = name;
			statEntry.UserColor = userColor;
			statEntry.BadActions += num;
			statEntry.CurrentGoodActions -= num;
			statEntry.CurrentActions++;
			return statEntry;
		}

		// Token: 0x0600A863 RID: 43107 RVA: 0x004246A4 File Offset: 0x004228A4
		public TwitchLeaderboardStats.StatEntry AddBitsUsed(string name, string userColor, int amount)
		{
			if (!this.StatEntries.ContainsKey(name))
			{
				this.StatEntries.Add(name, new TwitchLeaderboardStats.StatEntry());
			}
			TwitchLeaderboardStats.StatEntry statEntry = this.StatEntries[name];
			statEntry.Name = name;
			statEntry.UserColor = userColor;
			statEntry.BitsUsed += amount;
			return statEntry;
		}

		// Token: 0x0600A864 RID: 43108 RVA: 0x004246F8 File Offset: 0x004228F8
		public void ClearAllCurrentGood()
		{
			foreach (TwitchLeaderboardStats.StatEntry statEntry in this.StatEntries.Values)
			{
				statEntry.CurrentGoodActions = 0;
				statEntry.CurrentActions = 0;
			}
			this.CurrentGoodViewer = null;
			this.HandleStatsChanged();
			this.HandleLeaderboardChanged();
		}

		// Token: 0x040082AE RID: 33454
		public int LargestPimpPot;

		// Token: 0x040082AF RID: 33455
		public int LargestBitPot;

		// Token: 0x040082B0 RID: 33456
		public int TotalGood;

		// Token: 0x040082B1 RID: 33457
		public int TotalBad;

		// Token: 0x040082B2 RID: 33458
		public int TotalActions;

		// Token: 0x040082B3 RID: 33459
		public int TotalBits;

		// Token: 0x040082B4 RID: 33460
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_GoodReward;

		// Token: 0x040082B5 RID: 33461
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_GoodReward;

		// Token: 0x040082B8 RID: 33464
		public TwitchLeaderboardStats.StatEntry TopKillerViewer;

		// Token: 0x040082B9 RID: 33465
		public TwitchLeaderboardStats.StatEntry TopGoodViewer;

		// Token: 0x040082BA RID: 33466
		public TwitchLeaderboardStats.StatEntry TopBadViewer;

		// Token: 0x040082BB RID: 33467
		public TwitchLeaderboardStats.StatEntry MostBitsSpentViewer;

		// Token: 0x040082BC RID: 33468
		public TwitchLeaderboardStats.StatEntry CurrentGoodViewer;

		// Token: 0x040082BD RID: 33469
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CurrentGoodDirty;

		// Token: 0x040082BE RID: 33470
		[PublicizedFrom(EAccessModifier.Private)]
		public float lastTime = 1f;

		// Token: 0x040082BF RID: 33471
		public float nextGoodTime = -1f;

		// Token: 0x040082C0 RID: 33472
		public int GoodRewardAmount = 1000;

		// Token: 0x040082C1 RID: 33473
		[PublicizedFrom(EAccessModifier.Private)]
		public int goodRewardTime = 15;

		// Token: 0x040082C2 RID: 33474
		public Dictionary<string, TwitchLeaderboardStats.StatEntry> StatEntries = new Dictionary<string, TwitchLeaderboardStats.StatEntry>();

		// Token: 0x0200156C RID: 5484
		public class StatEntry
		{
			// Token: 0x040082C3 RID: 33475
			public string Name;

			// Token: 0x040082C4 RID: 33476
			public string UserColor;

			// Token: 0x040082C5 RID: 33477
			public int Kills;

			// Token: 0x040082C6 RID: 33478
			public int GoodActions;

			// Token: 0x040082C7 RID: 33479
			public int BadActions;

			// Token: 0x040082C8 RID: 33480
			public int BitsUsed;

			// Token: 0x040082C9 RID: 33481
			public int CurrentGoodActions;

			// Token: 0x040082CA RID: 33482
			public int CurrentActions;
		}
	}
}
