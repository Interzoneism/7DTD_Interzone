using System;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x020008E1 RID: 2273
[Preserve]
public class ObjectiveTwitchVote : BaseObjective
{
	// Token: 0x170006FE RID: 1790
	// (get) Token: 0x06004306 RID: 17158 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool useUpdateLoop
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return true;
		}
	}

	// Token: 0x170006FF RID: 1791
	// (get) Token: 0x06004307 RID: 17159 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool ShowInQuestLog
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06004308 RID: 17160 RVA: 0x001A92BD File Offset: 0x001A74BD
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveAssemble_keyword", false);
	}

	// Token: 0x06004309 RID: 17161 RVA: 0x001B1262 File Offset: 0x001AF462
	public override void SetupDisplay()
	{
		base.Description = "";
		this.StatusText = "";
	}

	// Token: 0x0600430A RID: 17162 RVA: 0x001B127C File Offset: 0x001AF47C
	public override void Update(float updateTime)
	{
		TwitchManager twitchManager = TwitchManager.Current;
		switch (this.GameEventState)
		{
		case ObjectiveTwitchVote.TwitchVoteStates.Start:
		{
			if (this.voteType == "" || !twitchManager.IsReady || !twitchManager.VotingManager.VotingEnabled)
			{
				base.CurrentValue = 1;
				this.Refresh();
				return;
			}
			TwitchVotingManager votingManager = twitchManager.VotingManager;
			votingManager.VoteStarted = (OnGameEventVoteAction)Delegate.Combine(votingManager.VoteStarted, new OnGameEventVoteAction(this.VoteStarted));
			twitchManager.VotingManager.QueueVote(this.voteType);
			this.GameEventState = ObjectiveTwitchVote.TwitchVoteStates.Waiting;
			return;
		}
		case ObjectiveTwitchVote.TwitchVoteStates.Waiting:
			break;
		case ObjectiveTwitchVote.TwitchVoteStates.Complete:
			base.CurrentValue = 1;
			this.Refresh();
			break;
		default:
			return;
		}
	}

	// Token: 0x0600430B RID: 17163 RVA: 0x001B132B File Offset: 0x001AF52B
	[PublicizedFrom(EAccessModifier.Private)]
	public void VoteStarted()
	{
		if (TwitchManager.Current.VotingManager.CurrentVoteType.Name == this.voteType)
		{
			this.GameEventState = ObjectiveTwitchVote.TwitchVoteStates.Complete;
		}
	}

	// Token: 0x0600430C RID: 17164 RVA: 0x001A9400 File Offset: 0x001A7600
	public override void Refresh()
	{
		if (base.Complete)
		{
			return;
		}
		base.Complete = (base.CurrentValue == 1);
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x0600430D RID: 17165 RVA: 0x001B1358 File Offset: 0x001AF558
	public override BaseObjective Clone()
	{
		ObjectiveTwitchVote objectiveTwitchVote = new ObjectiveTwitchVote();
		this.CopyValues(objectiveTwitchVote);
		objectiveTwitchVote.voteType = this.voteType;
		return objectiveTwitchVote;
	}

	// Token: 0x0600430E RID: 17166 RVA: 0x001B137F File Offset: 0x001AF57F
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		properties.ParseString(ObjectiveTwitchVote.PropVoteType, ref this.voteType);
	}

	// Token: 0x04003518 RID: 13592
	[PublicizedFrom(EAccessModifier.Protected)]
	public string voteType = "";

	// Token: 0x04003519 RID: 13593
	[PublicizedFrom(EAccessModifier.Protected)]
	public float timeRemaining = 2f;

	// Token: 0x0400351A RID: 13594
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropVoteType = "vote_type";

	// Token: 0x0400351B RID: 13595
	[PublicizedFrom(EAccessModifier.Protected)]
	public ObjectiveTwitchVote.TwitchVoteStates GameEventState;

	// Token: 0x020008E2 RID: 2274
	[PublicizedFrom(EAccessModifier.Protected)]
	public enum TwitchVoteStates
	{
		// Token: 0x0400351D RID: 13597
		Start,
		// Token: 0x0400351E RID: 13598
		Waiting,
		// Token: 0x0400351F RID: 13599
		Complete
	}
}
