using System;

namespace Twitch
{
	// Token: 0x02001519 RID: 5401
	public class TwitchActionHistoryEntry
	{
		// Token: 0x0600A6C3 RID: 42691 RVA: 0x0041D6EC File Offset: 0x0041B8EC
		public TwitchActionHistoryEntry(string username, string usercolor, TwitchAction action, TwitchVote vote, TwitchEventActionEntry eventEntry)
		{
			this.UserName = username;
			this.Action = action;
			this.Vote = vote;
			this.EventEntry = eventEntry;
			this.UserColor = usercolor;
			ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements(GameManager.Instance.World.worldTime);
			int item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			int item3 = valueTuple.Item3;
			this.ActionTime = string.Format("{0} {1}, {2:00}:{3:00}", new object[]
			{
				Localization.Get("xuiDay", false),
				item,
				item2,
				item3
			});
		}

		// Token: 0x1700123F RID: 4671
		// (get) Token: 0x0600A6C4 RID: 42692 RVA: 0x0041D78C File Offset: 0x0041B98C
		public string Command
		{
			get
			{
				if (this.Action != null)
				{
					return string.Format("{0}({1})", this.Action.Command, this.PointsSpent);
				}
				if (this.Vote != null)
				{
					return this.Vote.VoteDescription;
				}
				if (this.EventEntry != null)
				{
					return this.EventEntry.Event.Description(this.EventEntry);
				}
				return "";
			}
		}

		// Token: 0x17001240 RID: 4672
		// (get) Token: 0x0600A6C5 RID: 42693 RVA: 0x0041D7FC File Offset: 0x0041B9FC
		public string Title
		{
			get
			{
				if (this.Action != null)
				{
					return this.Action.Title;
				}
				if (this.Vote != null)
				{
					return this.Vote.VoteDescription;
				}
				if (this.EventEntry != null)
				{
					return this.EventEntry.Event.EventTitle;
				}
				return "";
			}
		}

		// Token: 0x17001241 RID: 4673
		// (get) Token: 0x0600A6C6 RID: 42694 RVA: 0x0041D850 File Offset: 0x0041BA50
		public string Description
		{
			get
			{
				if (this.Action != null)
				{
					return this.Action.Description;
				}
				if (this.Vote != null)
				{
					return this.Vote.Description;
				}
				if (this.EventEntry != null)
				{
					return this.EventEntry.Event.EventTitle;
				}
				return "";
			}
		}

		// Token: 0x17001242 RID: 4674
		// (get) Token: 0x0600A6C7 RID: 42695 RVA: 0x0041D8A3 File Offset: 0x0041BAA3
		public string HistoryType
		{
			get
			{
				if (this.Action != null)
				{
					return "action";
				}
				if (this.Vote != null)
				{
					return "vote";
				}
				if (this.EventEntry != null)
				{
					return "event";
				}
				return "";
			}
		}

		// Token: 0x17001243 RID: 4675
		// (get) Token: 0x0600A6C8 RID: 42696 RVA: 0x0041D8D4 File Offset: 0x0041BAD4
		public bool IsRefunded
		{
			get
			{
				return this.EntryState == TwitchActionHistoryEntry.EntryStates.Refunded;
			}
		}

		// Token: 0x0600A6C9 RID: 42697 RVA: 0x0041D8DF File Offset: 0x0041BADF
		[PublicizedFrom(EAccessModifier.Internal)]
		public bool IsValid()
		{
			return this.UserName != null && ((this.Action != null && this.Action.Command != null) || this.Vote != null || this.EventEntry != null);
		}

		// Token: 0x0600A6CA RID: 42698 RVA: 0x0041D914 File Offset: 0x0041BB14
		[PublicizedFrom(EAccessModifier.Internal)]
		public void Refund()
		{
			if (this.EntryState != TwitchActionHistoryEntry.EntryStates.Refunded)
			{
				TwitchManager twitchManager = TwitchManager.Current;
				twitchManager.ViewerData.ReimburseAction(this.UserName, this.PointsSpent, this.Action);
				twitchManager.LocalPlayer.PlayOneShot("ui_vending_purchase", false, false, false, null);
				this.EntryState = TwitchActionHistoryEntry.EntryStates.Refunded;
			}
		}

		// Token: 0x0600A6CB RID: 42699 RVA: 0x0041D968 File Offset: 0x0041BB68
		public void Retry()
		{
			if (!this.HasRetried)
			{
				if (this.Action != null)
				{
					TwitchManager.Current.HandleExtensionMessage(this.UserID, string.Format("{0} {1}", this.Action.Command, this.Target), true, 0, 0);
				}
				else if (this.EventEntry != null)
				{
					this.EventEntry.IsSent = false;
					this.EventEntry.IsRetry = true;
					TwitchManager.Current.EventQueue.Add(this.EventEntry);
				}
				this.HasRetried = true;
			}
		}

		// Token: 0x0600A6CC RID: 42700 RVA: 0x0041D9F4 File Offset: 0x0041BBF4
		public bool CanRetry()
		{
			if (this.HasRetried)
			{
				return false;
			}
			TwitchManager.CooldownTypes cooldownType = TwitchManager.Current.CooldownType;
			if (this.Action != null)
			{
				if (TwitchManager.Current.VotingManager.VotingIsActive)
				{
					return false;
				}
				if (!this.Action.IgnoreCooldown)
				{
					return cooldownType != TwitchManager.CooldownTypes.BloodMoonDisabled && cooldownType != TwitchManager.CooldownTypes.Time && cooldownType != TwitchManager.CooldownTypes.QuestDisabled && ((cooldownType != TwitchManager.CooldownTypes.MaxReachedWaiting && cooldownType != TwitchManager.CooldownTypes.SafeCooldown) || !this.Action.WaitingBlocked) && !this.HasRetried;
				}
				return !this.HasRetried;
			}
			else
			{
				if (this.EventEntry != null)
				{
					if (!this.EventEntry.Event.CooldownAllowed)
					{
						if (cooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled || cooldownType == TwitchManager.CooldownTypes.Time || cooldownType == TwitchManager.CooldownTypes.QuestDisabled)
						{
							return false;
						}
						if (cooldownType == TwitchManager.CooldownTypes.MaxReachedWaiting)
						{
							return false;
						}
					}
					return (this.EventEntry.Event.StartingCooldownAllowed || cooldownType != TwitchManager.CooldownTypes.Startup) && (this.EventEntry.Event.VoteEventAllowed || !TwitchManager.Current.VotingManager.VotingIsActive) && !this.HasRetried;
				}
				return false;
			}
		}

		// Token: 0x0600A6CD RID: 42701 RVA: 0x0041DAEE File Offset: 0x0041BCEE
		public bool CanRefund()
		{
			return this.PointsSpent > 0 && this.EntryState != TwitchActionHistoryEntry.EntryStates.Refunded && this.EntryState != TwitchActionHistoryEntry.EntryStates.Reimbursed && this.Action != null;
		}

		// Token: 0x0400810C RID: 33036
		public string UserName;

		// Token: 0x0400810D RID: 33037
		public string UserColor;

		// Token: 0x0400810E RID: 33038
		public string Target;

		// Token: 0x0400810F RID: 33039
		public int UserID;

		// Token: 0x04008110 RID: 33040
		public TwitchAction Action;

		// Token: 0x04008111 RID: 33041
		public TwitchVote Vote;

		// Token: 0x04008112 RID: 33042
		public TwitchActionEntry ActionEntry;

		// Token: 0x04008113 RID: 33043
		public TwitchEventActionEntry EventEntry;

		// Token: 0x04008114 RID: 33044
		public int PointsSpent;

		// Token: 0x04008115 RID: 33045
		public bool HasRetried;

		// Token: 0x04008116 RID: 33046
		public string ActionTime;

		// Token: 0x04008117 RID: 33047
		public TwitchActionHistoryEntry.EntryStates EntryState;

		// Token: 0x0200151A RID: 5402
		public enum EntryStates
		{
			// Token: 0x04008119 RID: 33049
			Waiting,
			// Token: 0x0400811A RID: 33050
			Completed,
			// Token: 0x0400811B RID: 33051
			Reimbursed,
			// Token: 0x0400811C RID: 33052
			Despawned,
			// Token: 0x0400811D RID: 33053
			Refunded
		}
	}
}
