using System;
using Twitch;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016E9 RID: 5865
	[Preserve]
	public class ActionTwitchVoteDelay : BaseAction
	{
		// Token: 0x0600B1A5 RID: 45477 RVA: 0x00453F44 File Offset: 0x00452144
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			TwitchManager twitchManager = TwitchManager.Current;
			if (twitchManager.VotingManager.CurrentVoteState == TwitchVotingManager.VoteStateTypes.WaitingForNextVote)
			{
				float floatValue = GameEventManager.GetFloatValue(base.Owner.Target as EntityAlive, this.delayTimeText, 5f);
				twitchManager.VotingManager.VoteStartDelayTimeRemaining += floatValue;
			}
			else
			{
				Debug.LogWarning("Error: VoteDelay set in wrong state. " + twitchManager.VotingManager.CurrentVoteState.ToString());
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B1A6 RID: 45478 RVA: 0x00453FC1 File Offset: 0x004521C1
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionTwitchVoteDelay.PropTime, ref this.delayTimeText);
		}

		// Token: 0x0600B1A7 RID: 45479 RVA: 0x00453FDB File Offset: 0x004521DB
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionTwitchVoteDelay
			{
				delayTimeText = this.delayTimeText
			};
		}

		// Token: 0x04008B13 RID: 35603
		[PublicizedFrom(EAccessModifier.Protected)]
		public string delayTimeText;

		// Token: 0x04008B14 RID: 35604
		public static string PropTime = "time";
	}
}
