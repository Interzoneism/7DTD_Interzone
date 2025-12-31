using System;
using Twitch;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016E8 RID: 5864
	[Preserve]
	public class ActionTwitchStartVote : BaseAction
	{
		// Token: 0x0600B1A0 RID: 45472 RVA: 0x00453EA8 File Offset: 0x004520A8
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (this.voteType == "")
			{
				return BaseAction.ActionCompleteStates.InCompleteRefund;
			}
			TwitchManager twitchManager = TwitchManager.Current;
			if (!twitchManager.TwitchActive || !twitchManager.VotingManager.VotingEnabled)
			{
				return BaseAction.ActionCompleteStates.InCompleteRefund;
			}
			twitchManager.VotingManager.QueueVote(this.voteType);
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B1A1 RID: 45473 RVA: 0x00453EF8 File Offset: 0x004520F8
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionTwitchStartVote.PropVoteType, ref this.voteType);
		}

		// Token: 0x0600B1A2 RID: 45474 RVA: 0x00453F12 File Offset: 0x00452112
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionTwitchStartVote
			{
				voteType = this.voteType
			};
		}

		// Token: 0x04008B11 RID: 35601
		[PublicizedFrom(EAccessModifier.Protected)]
		public string voteType = "";

		// Token: 0x04008B12 RID: 35602
		public static string PropVoteType = "vote_type";
	}
}
