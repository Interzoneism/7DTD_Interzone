using System;

namespace Twitch
{
	// Token: 0x0200155F RID: 5471
	public class TwitchCreatorGoalEventEntry : BaseTwitchEventEntry
	{
		// Token: 0x0600A811 RID: 43025 RVA: 0x004230C0 File Offset: 0x004212C0
		public override bool IsValid(int amount = -1, string name = "", TwitchSubEventEntry.SubTierTypes subTier = TwitchSubEventEntry.SubTierTypes.Any)
		{
			return name == this.GoalType;
		}

		// Token: 0x04008269 RID: 33385
		public string GoalType = "Subs";

		// Token: 0x0400826A RID: 33386
		public int RewardAmount = 100;

		// Token: 0x0400826B RID: 33387
		public TwitchAction.PointTypes RewardType;
	}
}
