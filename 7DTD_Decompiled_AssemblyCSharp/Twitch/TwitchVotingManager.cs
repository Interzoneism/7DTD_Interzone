using System;
using System.Collections.Generic;
using Audio;
using Challenges;
using UniLinq;
using UnityEngine;

namespace Twitch
{
	// Token: 0x0200158C RID: 5516
	public class TwitchVotingManager
	{
		// Token: 0x170012D4 RID: 4820
		// (get) Token: 0x0600A9B4 RID: 43444 RVA: 0x00430963 File Offset: 0x0042EB63
		// (set) Token: 0x0600A9B5 RID: 43445 RVA: 0x0043096B File Offset: 0x0042EB6B
		public int MaxDailyVotes
		{
			get
			{
				return this.maxDailyVotes;
			}
			set
			{
				if (this.maxDailyVotes != value)
				{
					this.maxDailyVotes = value;
					this.lastGameDay = 1;
				}
			}
		}

		// Token: 0x170012D5 RID: 4821
		// (get) Token: 0x0600A9B6 RID: 43446 RVA: 0x00430984 File Offset: 0x0042EB84
		// (set) Token: 0x0600A9B7 RID: 43447 RVA: 0x0043098C File Offset: 0x0042EB8C
		public int CurrentVoteDayTimeRange
		{
			get
			{
				return this.currentVoteDayTimeRange;
			}
			set
			{
				if (this.currentVoteDayTimeRange != value)
				{
					this.currentVoteDayTimeRange = value;
					this.lastGameDay = 1;
				}
			}
		}

		// Token: 0x170012D6 RID: 4822
		// (get) Token: 0x0600A9B8 RID: 43448 RVA: 0x004309A5 File Offset: 0x0042EBA5
		public bool VotingEnabled
		{
			get
			{
				return !this.Owner.CurrentVotePreset.IsEmpty;
			}
		}

		// Token: 0x170012D7 RID: 4823
		// (get) Token: 0x0600A9B9 RID: 43449 RVA: 0x004309BA File Offset: 0x0042EBBA
		public bool VotingIsActive
		{
			get
			{
				return this.VotingEnabled && this.CurrentVoteState != TwitchVotingManager.VoteStateTypes.WaitingForNextVote && this.CurrentVoteState != TwitchVotingManager.VoteStateTypes.Init && this.CurrentVoteState != TwitchVotingManager.VoteStateTypes.ReadyForVoteStart && this.CurrentVoteState != TwitchVotingManager.VoteStateTypes.RequestedVoteStart && this.CurrentVoteState != TwitchVotingManager.VoteStateTypes.VoteReady;
			}
		}

		// Token: 0x170012D8 RID: 4824
		// (get) Token: 0x0600A9BA RID: 43450 RVA: 0x004309F5 File Offset: 0x0042EBF5
		public int VoteCount
		{
			get
			{
				if (this.voterlist == null)
				{
					return 0;
				}
				return this.voterlist.Count;
			}
		}

		// Token: 0x170012D9 RID: 4825
		// (get) Token: 0x0600A9BB RID: 43451 RVA: 0x00430A0C File Offset: 0x0042EC0C
		public string VoteTypeText
		{
			get
			{
				return this.CurrentVoteType.Title;
			}
		}

		// Token: 0x170012DA RID: 4826
		// (get) Token: 0x0600A9BC RID: 43452 RVA: 0x00430A19 File Offset: 0x0042EC19
		public string VoteTip
		{
			get
			{
				if (this.CurrentEvent != null)
				{
					return this.CurrentEvent.VoteClass.VoteTip;
				}
				return "";
			}
		}

		// Token: 0x170012DB RID: 4827
		// (get) Token: 0x0600A9BD RID: 43453 RVA: 0x00430A39 File Offset: 0x0042EC39
		public string VoteOffset
		{
			get
			{
				if (this.CurrentEvent != null)
				{
					return this.CurrentEvent.VoteClass.VoteHeight;
				}
				return "0";
			}
		}

		// Token: 0x170012DC RID: 4828
		// (get) Token: 0x0600A9BE RID: 43454 RVA: 0x00430A59 File Offset: 0x0042EC59
		public bool UseMystery
		{
			get
			{
				return this.CurrentVoteType.UseMystery;
			}
		}

		// Token: 0x170012DD RID: 4829
		// (get) Token: 0x0600A9BF RID: 43455 RVA: 0x00430A66 File Offset: 0x0042EC66
		// (set) Token: 0x0600A9C0 RID: 43456 RVA: 0x00430A6E File Offset: 0x0042EC6E
		public int NeededLines { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x170012DE RID: 4830
		// (get) Token: 0x0600A9C1 RID: 43457 RVA: 0x00430A77 File Offset: 0x0042EC77
		// (set) Token: 0x0600A9C2 RID: 43458 RVA: 0x00430A7F File Offset: 0x0042EC7F
		public TwitchVoteType CurrentVoteType { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600A9C3 RID: 43459 RVA: 0x00430A88 File Offset: 0x0042EC88
		public TwitchVotingManager(TwitchManager owner)
		{
			this.Owner = owner;
			this.SetupVoteDayTimeRanges();
		}

		// Token: 0x0600A9C4 RID: 43460 RVA: 0x00430B56 File Offset: 0x0042ED56
		public void CleanupData()
		{
			this.VoteTypes.Clear();
			this.VoteGroups.Clear();
		}

		// Token: 0x0600A9C5 RID: 43461 RVA: 0x00430B70 File Offset: 0x0042ED70
		public void SetupLocalization()
		{
			this.chatOutput_VoteStarted = Localization.Get("TwitchChat_VoteStarted", false);
			this.chatOutput_VoteFinished = Localization.Get("TwitchChat_VoteFinished", false);
			this.dayTimeRangeOutput = Localization.Get("xuiOptionsTwitchVoteDayTimeRangeDisplay", false);
			this.VoteOptionA = Localization.Get("TwitchVoteOption_A", false);
			this.VoteOptionB = Localization.Get("TwitchVoteOption_B", false);
			this.VoteOptionC = Localization.Get("TwitchVoteOption_C", false);
			this.VoteOptionD = Localization.Get("TwitchVoteOption_D", false);
			this.VoteOptionE = Localization.Get("TwitchVoteOption_E", false);
		}

		// Token: 0x0600A9C6 RID: 43462 RVA: 0x00430C08 File Offset: 0x0042EE08
		public void AddVoteType(TwitchVoteType voteType)
		{
			this.VoteTypes.Add(voteType.Name, voteType);
			for (int i = 0; i < this.VoteGroups.Count; i++)
			{
				if (this.VoteGroups[i].Name == voteType.Group)
				{
					this.VoteGroups[i].VoteTypes.Add(voteType);
					return;
				}
			}
			TwitchVoteGroup twitchVoteGroup = new TwitchVoteGroup(voteType.Group);
			twitchVoteGroup.VoteTypes.Add(voteType);
			this.VoteGroups.Add(twitchVoteGroup);
		}

		// Token: 0x0600A9C7 RID: 43463 RVA: 0x00430C97 File Offset: 0x0042EE97
		public TwitchVoteType GetVoteType(string voteTypeName)
		{
			if (this.VoteTypes.ContainsKey(voteTypeName))
			{
				return this.VoteTypes[voteTypeName];
			}
			return null;
		}

		// Token: 0x0600A9C8 RID: 43464 RVA: 0x00430CB8 File Offset: 0x0042EEB8
		public void AddVote(int index, string userName)
		{
			if (!this.voterlist.Contains(userName) && this.voteList.Count > index)
			{
				this.voteList[index].VoteCount++;
				Manager.PlayInsidePlayerHead("twitch_vote_received", -1, 0f, false, false);
				this.voterlist.Add(userName);
				this.UIDirty = true;
				for (int i = 0; i < this.voteList.Count; i++)
				{
					this.voteList[i].UIDirty = true;
				}
			}
		}

		// Token: 0x0600A9C9 RID: 43465 RVA: 0x00430D48 File Offset: 0x0042EF48
		public void ClearVotes()
		{
			for (int i = 0; i < this.voteList.Count; i++)
			{
				this.voteList[i].VoteCount = 0;
				this.voteList[i].VoterNames.Clear();
			}
			this.voterlist.Clear();
		}

		// Token: 0x0600A9CA RID: 43466 RVA: 0x00430DA0 File Offset: 0x0042EFA0
		[PublicizedFrom(EAccessModifier.Private)]
		public void CalculateVoteTimes()
		{
			this.DailyVoteTimes.Clear();
			GameRandom gameRandom = GameManager.Instance.World.GetGameRandom();
			TwitchVotingManager.VoteDayTimeRange voteDayTimeRange = this.VoteDayTimeRanges[this.CurrentVoteDayTimeRange];
			float num = (float)(voteDayTimeRange.EndHour - voteDayTimeRange.StartHour) / (float)this.MaxDailyVotes;
			float num2 = (float)voteDayTimeRange.StartHour;
			for (int i = 0; i < this.MaxDailyVotes; i++)
			{
				float num3 = -1f;
				if (i == 0)
				{
					gameRandom.RandomRange(0f, num);
				}
				else
				{
					gameRandom.RandomRange(num - 1f, num);
				}
				int num4 = (int)(num2 + num3);
				int minutes = gameRandom.RandomRange(0, 59);
				TwitchVotingManager.DailyVoteEntry dailyVoteEntry = new TwitchVotingManager.DailyVoteEntry();
				dailyVoteEntry.VoteStartTime = GameUtils.DayTimeToWorldTime(1, num4, minutes);
				dailyVoteEntry.VoteEndTime = GameUtils.DayTimeToWorldTime(1, num4 + 1, minutes);
				dailyVoteEntry.Index = i + 1;
				num2 += num;
				this.DailyVoteTimes.Add(dailyVoteEntry);
			}
		}

		// Token: 0x0600A9CB RID: 43467 RVA: 0x00430E98 File Offset: 0x0042F098
		[PublicizedFrom(EAccessModifier.Private)]
		public void ResetVoteTypeDay()
		{
			foreach (TwitchVoteType twitchVoteType in this.VoteTypes.Values)
			{
				twitchVoteType.CurrentDayCount = 0;
			}
			foreach (TwitchVote twitchVote in TwitchActionManager.TwitchVotes.Values)
			{
				twitchVote.CurrentDayCount = 0;
			}
		}

		// Token: 0x0600A9CC RID: 43468 RVA: 0x00430F34 File Offset: 0x0042F134
		public bool SetupVoteList(List<TwitchVoteEntry> voteList)
		{
			World world = GameManager.Instance.World;
			TwitchVoteType currentVoteType = this.CurrentVoteType;
			string name = currentVoteType.Name;
			TwitchVote twitchVote = null;
			int highestGameStage = this.Owner.HighestGameStage;
			this.ClearVotes();
			voteList.Clear();
			this.tempSortList.Clear();
			this.tempVoteGroupList.Clear();
			EntityPlayer localPlayer = this.Owner.LocalPlayer;
			if (currentVoteType.GuaranteedGroup != "")
			{
				foreach (TwitchVote twitchVote2 in TwitchActionManager.TwitchVotes.Values)
				{
					if (twitchVote2.Enabled && twitchVote2.IsInPreset(this.Owner.CurrentVotePreset) && twitchVote2.VoteTypes.Contains(name) && twitchVote2.Group == currentVoteType.GuaranteedGroup && twitchVote2.CanUse(this.hour, highestGameStage, localPlayer))
					{
						this.tempSortList.Add(twitchVote2);
					}
				}
				for (int i = 0; i < this.tempSortList.Count * 3; i++)
				{
					int num = world.GetGameRandom().RandomRange(this.tempSortList.Count);
					int num2 = world.GetGameRandom().RandomRange(this.tempSortList.Count);
					if (num != num2)
					{
						TwitchVote value = this.tempSortList[num];
						this.tempSortList[num] = this.tempSortList[num2];
						this.tempSortList[num2] = value;
					}
				}
				twitchVote = this.tempSortList[0];
				this.tempSortList.Clear();
			}
			foreach (TwitchVote twitchVote3 in TwitchActionManager.TwitchVotes.Values)
			{
				if (twitchVote3.Enabled && twitchVote3.IsInPreset(this.Owner.CurrentVotePreset) && twitchVote3.VoteTypes.Contains(name) && twitchVote3.CanUse(this.hour, highestGameStage, localPlayer))
				{
					this.tempSortList.Add(twitchVote3);
				}
			}
			for (int j = 0; j < this.tempSortList.Count * 3; j++)
			{
				int num3 = world.GetGameRandom().RandomRange(this.tempSortList.Count);
				int num4 = world.GetGameRandom().RandomRange(this.tempSortList.Count);
				if (num3 != num4)
				{
					TwitchVote value2 = this.tempSortList[num3];
					this.tempSortList[num3] = this.tempSortList[num4];
					this.tempSortList[num4] = value2;
				}
			}
			this.NeededLines = 1;
			int num5 = 0;
			if (twitchVote != null)
			{
				this.tempSortList.Insert(UnityEngine.Random.Range(0, 3), twitchVote);
			}
			for (int k = 0; k < this.tempSortList.Count; k++)
			{
				TwitchVote twitchVote4 = this.tempSortList[k];
				if (!(twitchVote4.Group != "") || !this.tempVoteGroupList.Contains(twitchVote4.Group))
				{
					if (twitchVote4.Group != "")
					{
						this.tempVoteGroupList.Add(twitchVote4.Group);
					}
					string voteCommand = this.VoteOptionA;
					switch (num5)
					{
					case 1:
						voteCommand = this.VoteOptionB;
						break;
					case 2:
						voteCommand = this.VoteOptionC;
						break;
					case 3:
						voteCommand = this.VoteOptionD;
						break;
					case 4:
						voteCommand = this.VoteOptionE;
						break;
					}
					if (twitchVote4.VoteLine1 != "" && this.NeededLines < 2)
					{
						this.NeededLines = 2;
					}
					if (twitchVote4.VoteLine2 != "" && this.NeededLines < 3)
					{
						this.NeededLines = 3;
					}
					voteList.Add(new TwitchVoteEntry(voteCommand, twitchVote4)
					{
						Owner = this,
						Index = num5
					});
					num5++;
					if (num5 == currentVoteType.VoteChoiceCount)
					{
						break;
					}
				}
			}
			return voteList.Count != 0;
		}

		// Token: 0x0600A9CD RID: 43469 RVA: 0x00431378 File Offset: 0x0042F578
		public TwitchVoteEntry GetVoteWinner()
		{
			this.tempVoteList.Clear();
			int num = -1;
			for (int i = 0; i < this.voteList.Count; i++)
			{
				if (this.voteList[i].VoteCount > num)
				{
					num = this.voteList[i].VoteCount;
					this.tempVoteList.Clear();
					this.tempVoteList.Add(this.voteList[i]);
				}
				else if (this.voteList[i].VoteCount == num)
				{
					this.tempVoteList.Add(this.voteList[i]);
				}
			}
			return this.tempVoteList[GameManager.Instance.World.GetGameRandom().RandomRange(0, this.tempVoteList.Count)];
		}

		// Token: 0x0600A9CE RID: 43470 RVA: 0x0043144C File Offset: 0x0042F64C
		public void ResetVoteOnDeath()
		{
			TwitchVotingManager.VoteStateTypes currentVoteState = this.CurrentVoteState;
			if (currentVoteState - TwitchVotingManager.VoteStateTypes.VoteStarted <= 1)
			{
				this.readyForVote = false;
				this.Owner.LocalPlayer.TwitchVoteLock = TwitchVoteLockTypes.None;
				this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.WaitingForNextVote;
				this.ResetVoteGroupsForVote();
				if (this.VoteEventEnded != null)
				{
					this.VoteEventEnded();
				}
			}
		}

		// Token: 0x0600A9CF RID: 43471 RVA: 0x004314A0 File Offset: 0x0042F6A0
		[PublicizedFrom(EAccessModifier.Private)]
		public void ResetVoteGroupsForVote()
		{
			for (int i = 0; i < this.VoteGroups.Count; i++)
			{
				this.VoteGroups[i].SkippedThisVote = false;
			}
		}

		// Token: 0x0600A9D0 RID: 43472 RVA: 0x004314D8 File Offset: 0x0042F6D8
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckAllVoteGroupsSkipped()
		{
			for (int i = 0; i < this.VoteGroups.Count; i++)
			{
				if (!this.VoteGroups[i].SkippedThisVote)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600A9D1 RID: 43473 RVA: 0x00431514 File Offset: 0x0042F714
		[PublicizedFrom(EAccessModifier.Private)]
		public bool AllowVoting()
		{
			return this.Owner.CooldownType == TwitchManager.CooldownTypes.None || ((this.Owner.CooldownType == TwitchManager.CooldownTypes.QuestCooldown || this.Owner.CooldownType == TwitchManager.CooldownTypes.QuestDisabled) && this.AllowVotesDuringQuests) || ((this.Owner.CooldownType == TwitchManager.CooldownTypes.BloodMoonCooldown || this.Owner.CooldownType == TwitchManager.CooldownTypes.BloodMoonDisabled) && this.AllowVotesDuringBloodmoon);
		}

		// Token: 0x0600A9D2 RID: 43474 RVA: 0x00431578 File Offset: 0x0042F778
		public void Update(float deltaTime)
		{
			switch (this.CurrentVoteState)
			{
			case TwitchVotingManager.VoteStateTypes.Init:
				GameEventManager.Current.GameEventApproved += this.Current_GameEventApproved;
				GameEventManager.Current.GameEventCompleted += this.Current_GameEventCompleted;
				GameEventManager.Current.GameEntitySpawned += this.Current_GameEntitySpawned;
				GameEventManager.Current.GameEntityKilled += this.Current_GameEntityKilled;
				this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.WaitingForNextVote;
				this.ShuffleVoteGroups();
				this.ShuffleVoteGroupVoteTypes();
				return;
			case TwitchVotingManager.VoteStateTypes.WaitingForNextVote:
			{
				if (this.QueuedVoteType != null)
				{
					this.CurrentVoteType = this.QueuedVoteType;
					if (this.SetupVoteList(this.voteList))
					{
						this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.ReadyForVoteStart;
						this.Owner.RefreshVoteLockedLevel();
					}
					this.QueuedVoteType = null;
				}
				if (this.VoteStartDelayTimeRemaining > 0f)
				{
					this.VoteStartDelayTimeRemaining -= deltaTime;
					return;
				}
				World world = GameManager.Instance.World;
				ulong num = world.worldTime;
				this.day = GameUtils.WorldTimeToDays(num);
				num %= 24000UL;
				this.hour = GameUtils.WorldTimeToHours(num);
				if (this.day == 1)
				{
					return;
				}
				TwitchVotingManager.VoteDayTimeRange voteDayTimeRange = this.VoteDayTimeRanges[this.CurrentVoteDayTimeRange];
				bool flag = !this.AllowVotesDuringBloodmoon && world.IsWorldEvent(World.WorldEvent.BloodMoon);
				if (this.VoteInProgress)
				{
					if (flag)
					{
						this.CancelVote();
					}
					if (this.hour < voteDayTimeRange.StartHour || this.hour > voteDayTimeRange.EndHour)
					{
						this.CancelVote();
					}
					return;
				}
				if (flag || this.hour < voteDayTimeRange.StartHour || this.hour > voteDayTimeRange.EndHour)
				{
					return;
				}
				if (this.day != this.lastGameDay)
				{
					this.CalculateVoteTimes();
					this.ResetVoteTypeDay();
					this.lastGameDay = this.day;
				}
				for (int i = 0; i < this.DailyVoteTimes.Count; i++)
				{
					if (this.DailyVoteTimes[i].LastVoteDay != this.day)
					{
						if (num > this.DailyVoteTimes[i].VoteStartTime && num < this.DailyVoteTimes[i].VoteEndTime)
						{
							if (!this.SetReadyForVote(this.DailyVoteTimes[i].Index))
							{
								if (this.CheckAllVoteGroupsSkipped())
								{
									this.DailyVoteTimes[i].LastVoteDay = this.day;
									this.ResetVoteGroupsForVote();
								}
								return;
							}
							this.ResetVoteGroupsForVote();
							if (this.SetupVoteList(this.voteList))
							{
								this.DailyVoteTimes[i].LastVoteDay = this.day;
								this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.ReadyForVoteStart;
								this.Owner.RefreshVoteLockedLevel();
							}
						}
						else if (num > this.DailyVoteTimes[i].VoteEndTime)
						{
							this.DailyVoteTimes[i].LastVoteDay = this.day;
						}
					}
				}
				return;
			}
			case TwitchVotingManager.VoteStateTypes.ReadyForVoteStart:
				if (this.VoteStartDelayTimeRemaining > 0f)
				{
					this.VoteStartDelayTimeRemaining -= deltaTime;
					return;
				}
				if (this.VotingEnabled && this.Owner.VoteLockedLevel == TwitchVoteLockTypes.None && this.AllowVoting() && (!this.CurrentVoteType.SpawnBlocked || this.Owner.ReadyForVote))
				{
					if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageTwitchVoteScheduling>().Setup(), false);
					}
					else
					{
						TwitchVoteScheduler.Current.AddParticipant(this.Owner.LocalPlayer.entityId);
					}
					this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.RequestedVoteStart;
					return;
				}
				break;
			case TwitchVotingManager.VoteStateTypes.RequestedVoteStart:
				break;
			case TwitchVotingManager.VoteStateTypes.VoteReady:
				if (this.VotingEnabled && this.Owner.VoteLockedLevel == TwitchVoteLockTypes.None && !this.CheckVoteLock() && this.AllowVoting() && (!this.CurrentVoteType.SpawnBlocked || this.Owner.ReadyForVote))
				{
					this.StartVote();
					return;
				}
				break;
			case TwitchVotingManager.VoteStateTypes.VoteStarted:
				if (this.VotingEnabled && this.AllowVoting())
				{
					this.VoteTimeRemaining -= Time.deltaTime;
					if (this.VoteTimeRemaining <= 0f)
					{
						this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.VoteFinished;
						return;
					}
				}
				break;
			case TwitchVotingManager.VoteStateTypes.VoteFinished:
				this.CurrentEvent = this.GetVoteWinner();
				this.CurrentEvent.ActiveSpawns.Clear();
				this.CurrentEvent.VoteClass.CurrentDayCount++;
				this.Owner.ircClient.SendChannelMessage(string.Format(this.chatOutput_VoteFinished, this.CurrentEvent.VoteClass.VoteDescription), true);
				this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.WaitingForActive;
				this.VoteTimeRemaining = 2f;
				QuestEventManager.Current.TwitchEventReceived(TwitchObjectiveTypes.VoteComplete, this.CurrentEvent.VoteClass.Group);
				return;
			case TwitchVotingManager.VoteStateTypes.WaitingForActive:
				if (this.VoteTimeRemaining > 0f)
				{
					this.VoteTimeRemaining -= deltaTime;
					return;
				}
				if (GameEventManager.Current.HandleAction(this.CurrentEvent.VoteClass.GameEvent, this.Owner.LocalPlayer, this.Owner.LocalPlayer, true, " ", "vote", this.Owner.AllowCrateSharing, true, "", null))
				{
					GameEventManager.Current.HandleGameEventApproved(this.CurrentEvent.VoteClass.GameEvent, this.Owner.LocalPlayer.entityId, " ", "vote");
					return;
				}
				this.VoteTimeRemaining = 10f;
				return;
			case TwitchVotingManager.VoteStateTypes.EventActive:
				if (this.VoteEventTimeRemaining < 0f)
				{
					if (this.VoteEventComplete)
					{
						this.HandleGameEventEnded(true);
						return;
					}
				}
				else
				{
					this.VoteEventTimeRemaining -= deltaTime;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x0600A9D3 RID: 43475 RVA: 0x00431B12 File Offset: 0x0042FD12
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckVoteLock()
		{
			return (!this.AllowVotesDuringQuests && QuestEventManager.Current.QuestBounds.width != 0f) || (!this.AllowVotesInSafeZone && this.Owner.IsSafe);
		}

		// Token: 0x0600A9D4 RID: 43476 RVA: 0x00431B4C File Offset: 0x0042FD4C
		public bool IsHighest(TwitchVoteEntry vote)
		{
			for (int i = 0; i < this.voteList.Count; i++)
			{
				if (i != vote.Index && this.voteList[i].VoteCount > vote.VoteCount)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600A9D5 RID: 43477 RVA: 0x00431B94 File Offset: 0x0042FD94
		public bool SetReadyForVote(int index)
		{
			return this.GetNextVoteType();
		}

		// Token: 0x0600A9D6 RID: 43478 RVA: 0x00431B9C File Offset: 0x0042FD9C
		[PublicizedFrom(EAccessModifier.Private)]
		public void ResetCurrentVote()
		{
			this.VoteTimeRemaining = this.VoteTime;
		}

		// Token: 0x0600A9D7 RID: 43479 RVA: 0x00431BAC File Offset: 0x0042FDAC
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetupVoteDayTimeRanges()
		{
			this.VoteDayTimeRanges.Clear();
			this.VoteDayTimeRanges.Add(new TwitchVotingManager.VoteDayTimeRange
			{
				Name = Localization.Get("TwitchVoteDayTimeRange_Short", false),
				StartHour = 8,
				EndHour = 16
			});
			this.VoteDayTimeRanges.Add(new TwitchVotingManager.VoteDayTimeRange
			{
				Name = Localization.Get("TwitchVoteDayTimeRange_Average", false),
				StartHour = 6,
				EndHour = 18
			});
			this.VoteDayTimeRanges.Add(new TwitchVotingManager.VoteDayTimeRange
			{
				Name = Localization.Get("TwitchVoteDayTimeRange_Extended", false),
				StartHour = 4,
				EndHour = 20
			});
			this.VoteDayTimeRanges.Add(new TwitchVotingManager.VoteDayTimeRange
			{
				Name = Localization.Get("TwitchVoteDayTimeRange_All", false),
				StartHour = 0,
				EndHour = 23
			});
		}

		// Token: 0x0600A9D8 RID: 43480 RVA: 0x00431C84 File Offset: 0x0042FE84
		[PublicizedFrom(EAccessModifier.Private)]
		public void ShuffleVoteGroups()
		{
			for (int i = 0; i <= this.VoteGroups.Count * this.VoteGroups.Count; i++)
			{
				int num = UnityEngine.Random.Range(0, this.VoteGroups.Count);
				int num2 = UnityEngine.Random.Range(0, this.VoteGroups.Count);
				if (num != num2)
				{
					TwitchVoteGroup value = this.VoteGroups[num];
					this.VoteGroups[num] = this.VoteGroups[num2];
					this.VoteGroups[num2] = value;
				}
			}
		}

		// Token: 0x0600A9D9 RID: 43481 RVA: 0x00431D10 File Offset: 0x0042FF10
		[PublicizedFrom(EAccessModifier.Private)]
		public void ShuffleVoteGroupVoteTypes()
		{
			for (int i = 0; i < this.VoteGroups.Count; i++)
			{
				this.VoteGroups[i].ShuffleVoteTypes();
			}
		}

		// Token: 0x0600A9DA RID: 43482 RVA: 0x00431D44 File Offset: 0x0042FF44
		public void CancelVote()
		{
			if (this.CurrentVoteState == TwitchVotingManager.VoteStateTypes.WaitingForNextVote && this.readyForVote)
			{
				this.readyForVote = false;
			}
		}

		// Token: 0x0600A9DB RID: 43483 RVA: 0x00431D5E File Offset: 0x0042FF5E
		public void RequestApprovedToStart()
		{
			this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.VoteReady;
		}

		// Token: 0x0600A9DC RID: 43484 RVA: 0x00431D68 File Offset: 0x0042FF68
		public void StartVote()
		{
			this.VoteTimeRemaining = this.VoteTime;
			this.voterlist.Clear();
			if (this.VoteStarted != null)
			{
				this.VoteStarted();
			}
			this.Owner.UIDirty = true;
			this.Owner.LocalPlayer.TwitchVoteLock = (this.CurrentVoteType.ActionLockout ? TwitchVoteLockTypes.ActionsLocked : TwitchVoteLockTypes.VoteLocked);
			this.Owner.ircClient.SendChannelMessage(this.chatOutput_VoteStarted, true);
			Manager.BroadcastPlay(this.Owner.LocalPlayer.position, "twitch_vote_started", 0f);
			this.readyForVote = false;
			this.CurrentVoteType.CurrentDayCount++;
			this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.VoteStarted;
		}

		// Token: 0x0600A9DD RID: 43485 RVA: 0x00431E24 File Offset: 0x00430024
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEntitySpawned(string gameEventID, int entityID, string tag)
		{
			if (this.CurrentEvent == null || tag != "vote")
			{
				return;
			}
			if (gameEventID == this.CurrentEvent.VoteClass.GameEvent)
			{
				this.CurrentEvent.ActiveSpawns.Add(entityID);
			}
		}

		// Token: 0x0600A9DE RID: 43486 RVA: 0x00431E70 File Offset: 0x00430070
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEntityKilled(int entityID)
		{
			if (this.CurrentEvent == null)
			{
				return;
			}
			if (this.CurrentEvent.ActiveSpawns.Contains(entityID))
			{
				this.CurrentEvent.ActiveSpawns.Remove(entityID);
			}
		}

		// Token: 0x0600A9DF RID: 43487 RVA: 0x00431EA0 File Offset: 0x004300A0
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEventCompleted(string gameEventID, int targetEntityID, string extraData, string tag)
		{
			if (this.CurrentEvent != null && this.CurrentEvent.VoteClass.GameEvent == gameEventID && tag == "vote")
			{
				this.VoteEventComplete = true;
			}
		}

		// Token: 0x0600A9E0 RID: 43488 RVA: 0x00431ED8 File Offset: 0x004300D8
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleGameEventEnded(bool playSound)
		{
			if (this.CurrentVoteType.CooldownOnEnd && this.Owner.AllowActions && this.Owner.CurrentCooldownPreset.CooldownType == CooldownPreset.CooldownTypes.Fill)
			{
				this.Owner.SetCooldown((float)this.Owner.CurrentCooldownPreset.NextCooldownTime, TwitchManager.CooldownTypes.MaxReached, false, true);
			}
			this.CurrentEvent.VoteClass.HandleVoteComplete();
			this.CurrentEvent = null;
			if (playSound)
			{
				Manager.BroadcastPlay(this.Owner.LocalPlayer.position, "twitch_vote_ended", 0f);
			}
			this.Owner.LocalPlayer.TwitchVoteLock = TwitchVoteLockTypes.None;
			this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.WaitingForNextVote;
			this.ResetVoteGroupsForVote();
			if (this.VoteEventEnded != null)
			{
				this.VoteEventEnded();
			}
			this.VoteStartDelayTimeRemaining = 10f;
		}

		// Token: 0x0600A9E1 RID: 43489 RVA: 0x00431FA8 File Offset: 0x004301A8
		[PublicizedFrom(EAccessModifier.Private)]
		public bool GetNextVoteType()
		{
			if (this.voteGroupIndex == -1)
			{
				this.voteGroupIndex = GameManager.Instance.World.GetGameRandom().RandomRange(this.VoteGroups.Count);
			}
			bool flag = GameManager.Instance.World.IsWorldEvent(World.WorldEvent.BloodMoon);
			TwitchVoteGroup twitchVoteGroup = this.VoteGroups[this.voteGroupIndex];
			if (!twitchVoteGroup.SkippedThisVote)
			{
				for (int i = 0; i < twitchVoteGroup.VoteTypes.Count; i++)
				{
					TwitchVoteType nextVoteType = twitchVoteGroup.GetNextVoteType();
					if (nextVoteType.IsInPreset(this.Owner.CurrentVotePreset.Name) && !nextVoteType.ManualStart && nextVoteType.CanUse() && this.hour >= nextVoteType.AllowedStartHour && this.hour <= nextVoteType.AllowedEndHour && (!nextVoteType.IsBoss || this.Owner.CurrentVotePreset.BossVoteSetting != TwitchVotingManager.BossVoteSettings.Disabled) && (nextVoteType.AllowedWithActions || !this.Owner.AllowActions) && ((nextVoteType.IsBoss && this.Owner.CurrentVotePreset.BossVoteSetting == TwitchVotingManager.BossVoteSettings.Daily) || ((nextVoteType.BloodMoonDay || this.Owner.nextBMDay != this.day) && (nextVoteType.BloodMoonAllowed || !flag))))
					{
						this.CurrentVoteType = nextVoteType;
						this.voteGroupIndex++;
						if (this.voteGroupIndex >= this.VoteGroups.Count)
						{
							this.voteGroupIndex = 0;
						}
						return true;
					}
				}
				twitchVoteGroup.SkippedThisVote = true;
			}
			this.voteGroupIndex++;
			if (this.voteGroupIndex >= this.VoteGroups.Count)
			{
				this.voteGroupIndex = 0;
			}
			return false;
		}

		// Token: 0x0600A9E2 RID: 43490 RVA: 0x0043215C File Offset: 0x0043035C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_GameEventApproved(string gameEventID, int targetEntityID, string extraData, string tag)
		{
			if (this.CurrentVoteState == TwitchVotingManager.VoteStateTypes.WaitingForActive && this.CurrentEvent.VoteClass.GameEvent == gameEventID)
			{
				this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.EventActive;
				this.VoteEventComplete = false;
				this.VoteEventTimeRemaining = 10f;
				this.CurrentEvent.VoterNames.AddRange(this.voterlist);
				this.Owner.AddVoteHistory(this.CurrentEvent.VoteClass);
				if (this.VoteEventStarted != null)
				{
					this.VoteEventStarted();
				}
			}
		}

		// Token: 0x0600A9E3 RID: 43491 RVA: 0x004321E4 File Offset: 0x004303E4
		public void HandleMessage(TwitchIRCClient.TwitchChatMessage message)
		{
			if (this.CurrentVoteState == TwitchVotingManager.VoteStateTypes.VoteStarted)
			{
				if (message.Message.EqualsCaseInsensitive(this.VoteOptionA))
				{
					this.AddVote(0, message.UserName);
					return;
				}
				if (message.Message.EqualsCaseInsensitive(this.VoteOptionB))
				{
					this.AddVote(1, message.UserName);
					return;
				}
				if (message.Message.EqualsCaseInsensitive(this.VoteOptionC))
				{
					this.AddVote(2, message.UserName);
					return;
				}
				if (message.Message.EqualsCaseInsensitive(this.VoteOptionD))
				{
					this.AddVote(3, message.UserName);
					return;
				}
				if (message.Message.EqualsCaseInsensitive(this.VoteOptionE))
				{
					this.AddVote(4, message.UserName);
				}
			}
		}

		// Token: 0x0600A9E4 RID: 43492 RVA: 0x004322A4 File Offset: 0x004304A4
		public List<string> HandleKiller(TwitchVoteEntry voteEntry)
		{
			if (this.CurrentEvent == null && voteEntry == null)
			{
				return null;
			}
			if (this.CurrentEvent != null)
			{
				List<string> voterNames = this.CurrentEvent.VoterNames;
				this.CurrentEvent.Complete = true;
				this.HandleGameEventEnded(false);
				return this.voterlist;
			}
			if (voteEntry != null)
			{
				return voteEntry.VoterNames;
			}
			return null;
		}

		// Token: 0x0600A9E5 RID: 43493 RVA: 0x004322F8 File Offset: 0x004304F8
		public string GetDayTimeRange(int tempVoteDayTimeRange)
		{
			TwitchVotingManager.VoteDayTimeRange voteDayTimeRange = this.VoteDayTimeRanges[tempVoteDayTimeRange];
			if (voteDayTimeRange.StartHour == 0 && voteDayTimeRange.EndHour == 23)
			{
				return voteDayTimeRange.Name;
			}
			return string.Format(this.dayTimeRangeOutput, voteDayTimeRange.StartHour, voteDayTimeRange.EndHour);
		}

		// Token: 0x0600A9E6 RID: 43494 RVA: 0x0043234C File Offset: 0x0043054C
		public void QueueVote(string voteType)
		{
			if (this.VoteTypes.ContainsKey(voteType))
			{
				this.QueuedVoteType = this.VoteTypes[voteType];
			}
		}

		// Token: 0x0600A9E7 RID: 43495 RVA: 0x0043236E File Offset: 0x0043056E
		public void ForceEndVote()
		{
			if (this.CurrentVoteState == TwitchVotingManager.VoteStateTypes.VoteStarted)
			{
				this.CurrentEvent = null;
				this.Owner.LocalPlayer.TwitchVoteLock = TwitchVoteLockTypes.None;
				this.CurrentVoteState = TwitchVotingManager.VoteStateTypes.WaitingForNextVote;
				if (this.VoteEventEnded != null)
				{
					this.VoteEventEnded();
				}
			}
		}

		// Token: 0x04008481 RID: 33921
		public TwitchManager Owner;

		// Token: 0x04008482 RID: 33922
		public TwitchVotingManager.VoteStateTypes CurrentVoteState;

		// Token: 0x04008483 RID: 33923
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_VoteStarted;

		// Token: 0x04008484 RID: 33924
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_VoteFinished;

		// Token: 0x04008485 RID: 33925
		[PublicizedFrom(EAccessModifier.Private)]
		public string dayTimeRangeOutput;

		// Token: 0x04008486 RID: 33926
		[PublicizedFrom(EAccessModifier.Private)]
		public string VoteOptionA;

		// Token: 0x04008487 RID: 33927
		[PublicizedFrom(EAccessModifier.Private)]
		public string VoteOptionB;

		// Token: 0x04008488 RID: 33928
		[PublicizedFrom(EAccessModifier.Private)]
		public string VoteOptionC;

		// Token: 0x04008489 RID: 33929
		[PublicizedFrom(EAccessModifier.Private)]
		public string VoteOptionD;

		// Token: 0x0400848A RID: 33930
		[PublicizedFrom(EAccessModifier.Private)]
		public string VoteOptionE;

		// Token: 0x0400848B RID: 33931
		[PublicizedFrom(EAccessModifier.Private)]
		public int maxDailyVotes = 4;

		// Token: 0x0400848C RID: 33932
		public int lastGameDay = 1;

		// Token: 0x0400848D RID: 33933
		public bool WinnerShowing;

		// Token: 0x0400848E RID: 33934
		[PublicizedFrom(EAccessModifier.Private)]
		public int currentVoteDayTimeRange = 2;

		// Token: 0x0400848F RID: 33935
		public List<TwitchVotingManager.VoteDayTimeRange> VoteDayTimeRanges = new List<TwitchVotingManager.VoteDayTimeRange>();

		// Token: 0x04008490 RID: 33936
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchVotingManager.DailyVoteEntry> DailyVoteTimes = new List<TwitchVotingManager.DailyVoteEntry>();

		// Token: 0x04008491 RID: 33937
		public bool AllowVotesDuringBloodmoon;

		// Token: 0x04008492 RID: 33938
		public bool AllowVotesDuringQuests;

		// Token: 0x04008493 RID: 33939
		public bool AllowVotesInSafeZone;

		// Token: 0x04008494 RID: 33940
		public List<TwitchVoteType> NextVotes = new List<TwitchVoteType>();

		// Token: 0x04008495 RID: 33941
		public bool VoteInProgress;

		// Token: 0x04008496 RID: 33942
		public float VoteTime = 60f;

		// Token: 0x04008497 RID: 33943
		public int ViewerDefeatReward = 250;

		// Token: 0x04008499 RID: 33945
		public float VoteStartDelayTimeRemaining;

		// Token: 0x0400849A RID: 33946
		public float VoteEventTimeRemaining;

		// Token: 0x0400849B RID: 33947
		public float VoteTimeRemaining;

		// Token: 0x0400849C RID: 33948
		public bool UIDirty;

		// Token: 0x0400849D RID: 33949
		public bool VoteEventComplete;

		// Token: 0x0400849E RID: 33950
		public List<TwitchVoteEntry> voteList = new List<TwitchVoteEntry>();

		// Token: 0x0400849F RID: 33951
		public List<string> voterlist = new List<string>();

		// Token: 0x040084A0 RID: 33952
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchVoteEntry> tempVoteList = new List<TwitchVoteEntry>();

		// Token: 0x040084A1 RID: 33953
		public OnGameEventVoteAction VoteStarted;

		// Token: 0x040084A2 RID: 33954
		public OnGameEventVoteAction VoteEventStarted;

		// Token: 0x040084A3 RID: 33955
		public OnGameEventVoteAction VoteEventEnded;

		// Token: 0x040084A4 RID: 33956
		public Dictionary<string, TwitchVoteType> VoteTypes = new Dictionary<string, TwitchVoteType>();

		// Token: 0x040084A5 RID: 33957
		[PublicizedFrom(EAccessModifier.Private)]
		public int voteGroupIndex = -1;

		// Token: 0x040084A6 RID: 33958
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchVoteGroup> VoteGroups = new List<TwitchVoteGroup>();

		// Token: 0x040084A7 RID: 33959
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchVoteType QueuedVoteType;

		// Token: 0x040084A9 RID: 33961
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchVote> tempSortList = new List<TwitchVote>();

		// Token: 0x040084AA RID: 33962
		[PublicizedFrom(EAccessModifier.Private)]
		public List<string> tempVoteGroupList = new List<string>();

		// Token: 0x040084AB RID: 33963
		[PublicizedFrom(EAccessModifier.Private)]
		public int day = -1;

		// Token: 0x040084AC RID: 33964
		[PublicizedFrom(EAccessModifier.Private)]
		public int hour = -1;

		// Token: 0x040084AD RID: 33965
		[PublicizedFrom(EAccessModifier.Private)]
		public bool readyForVote;

		// Token: 0x040084AE RID: 33966
		public TwitchVoteEntry CurrentEvent;

		// Token: 0x0200158D RID: 5517
		public enum VoteStateTypes
		{
			// Token: 0x040084B0 RID: 33968
			Init,
			// Token: 0x040084B1 RID: 33969
			WaitingForNextVote,
			// Token: 0x040084B2 RID: 33970
			ReadyForVoteStart,
			// Token: 0x040084B3 RID: 33971
			RequestedVoteStart,
			// Token: 0x040084B4 RID: 33972
			VoteReady,
			// Token: 0x040084B5 RID: 33973
			VoteStarted,
			// Token: 0x040084B6 RID: 33974
			VoteFinished,
			// Token: 0x040084B7 RID: 33975
			WaitingForActive,
			// Token: 0x040084B8 RID: 33976
			EventActive
		}

		// Token: 0x0200158E RID: 5518
		public enum BossVoteSettings
		{
			// Token: 0x040084BA RID: 33978
			Disabled,
			// Token: 0x040084BB RID: 33979
			Standard,
			// Token: 0x040084BC RID: 33980
			Daily
		}

		// Token: 0x0200158F RID: 5519
		public class DailyVoteEntry
		{
			// Token: 0x040084BD RID: 33981
			public ulong VoteStartTime;

			// Token: 0x040084BE RID: 33982
			public ulong VoteEndTime;

			// Token: 0x040084BF RID: 33983
			public int LastVoteDay;

			// Token: 0x040084C0 RID: 33984
			public int Index = -1;
		}

		// Token: 0x02001590 RID: 5520
		public class VoteDayTimeRange
		{
			// Token: 0x040084C1 RID: 33985
			public string Name;

			// Token: 0x040084C2 RID: 33986
			public int StartHour;

			// Token: 0x040084C3 RID: 33987
			public int EndHour;
		}
	}
}
