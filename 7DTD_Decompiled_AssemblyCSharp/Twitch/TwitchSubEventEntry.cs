using System;

namespace Twitch
{
	// Token: 0x02001564 RID: 5476
	public class TwitchSubEventEntry : TwitchEventEntry
	{
		// Token: 0x0600A838 RID: 43064 RVA: 0x004237CC File Offset: 0x004219CC
		public TwitchSubEventEntry()
		{
			this.RewardsBitPot = true;
		}

		// Token: 0x0600A839 RID: 43065 RVA: 0x004237DB File Offset: 0x004219DB
		public override bool IsValid(int amount = -1, string name = "", TwitchSubEventEntry.SubTierTypes subTier = TwitchSubEventEntry.SubTierTypes.Any)
		{
			return (this.StartAmount == -1 || amount >= this.StartAmount) && (this.EndAmount == -1 || amount <= this.EndAmount) && (this.SubTier == TwitchSubEventEntry.SubTierTypes.Any || this.SubTier == subTier);
		}

		// Token: 0x0600A83A RID: 43066 RVA: 0x00423816 File Offset: 0x00421A16
		public override string Description(TwitchEventActionEntry entry)
		{
			return this.EventTitle;
		}

		// Token: 0x0600A83B RID: 43067 RVA: 0x0042381E File Offset: 0x00421A1E
		public static TwitchSubEventEntry.SubTierTypes GetSubTier(string subPlan)
		{
			if (subPlan == "1000")
			{
				return TwitchSubEventEntry.SubTierTypes.Tier1;
			}
			if (subPlan == "2000")
			{
				return TwitchSubEventEntry.SubTierTypes.Tier2;
			}
			if (!(subPlan == "3000"))
			{
				return TwitchSubEventEntry.SubTierTypes.Prime;
			}
			return TwitchSubEventEntry.SubTierTypes.Tier3;
		}

		// Token: 0x04008282 RID: 33410
		public TwitchSubEventEntry.SubTierTypes SubTier;

		// Token: 0x02001565 RID: 5477
		public enum SubTierTypes
		{
			// Token: 0x04008284 RID: 33412
			Any,
			// Token: 0x04008285 RID: 33413
			Prime,
			// Token: 0x04008286 RID: 33414
			Tier1,
			// Token: 0x04008287 RID: 33415
			Tier2,
			// Token: 0x04008288 RID: 33416
			Tier3
		}
	}
}
