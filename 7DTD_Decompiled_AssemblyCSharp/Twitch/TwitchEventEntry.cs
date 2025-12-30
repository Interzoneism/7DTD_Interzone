using System;

namespace Twitch
{
	// Token: 0x02001560 RID: 5472
	public class TwitchEventEntry : BaseTwitchEventEntry
	{
		// Token: 0x0600A813 RID: 43027 RVA: 0x004230E9 File Offset: 0x004212E9
		public override bool IsValid(int amount = -1, string name = "", TwitchSubEventEntry.SubTierTypes subTier = TwitchSubEventEntry.SubTierTypes.Any)
		{
			return (this.StartAmount == -1 || amount >= this.StartAmount) && (this.EndAmount == -1 || amount <= this.EndAmount);
		}

		// Token: 0x0400826C RID: 33388
		public int StartAmount = -1;

		// Token: 0x0400826D RID: 33389
		public int EndAmount = -1;
	}
}
