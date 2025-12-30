using System;
using UnityEngine;

namespace Twitch
{
	// Token: 0x02001581 RID: 5505
	public class ViewerEntry
	{
		// Token: 0x170012CF RID: 4815
		// (get) Token: 0x0600A98E RID: 43406 RVA: 0x0042F982 File Offset: 0x0042DB82
		public float CombinedPoints
		{
			get
			{
				return this.SpecialPoints + this.StandardPoints;
			}
		}

		// Token: 0x0600A98F RID: 43407 RVA: 0x0042F994 File Offset: 0x0042DB94
		public void RemovePoints(float usedPoints, TwitchAction.PointTypes pointType, TwitchActionEntry entry)
		{
			if (pointType == TwitchAction.PointTypes.SP)
			{
				this.SpecialPoints -= usedPoints;
				entry.SpecialPointsUsed = (int)usedPoints;
				return;
			}
			if (pointType == TwitchAction.PointTypes.PP)
			{
				float num = Mathf.Min(usedPoints, this.StandardPoints);
				entry.StandardPointsUsed = (int)num;
				this.StandardPoints -= num;
				num = usedPoints - num;
				if (num > 0f)
				{
					this.SpecialPoints -= num;
					entry.SpecialPointsUsed = (int)num;
					return;
				}
			}
			else if (pointType == TwitchAction.PointTypes.Bits)
			{
				int num2 = Utils.FastMin((int)usedPoints, (ExtensionManager.Version == "2.0.1") ? this.BitCredits : TwitchAction.GetAdjustedBitPriceFloor(this.BitCredits));
				this.BitCredits -= num2;
				entry.CreditsUsed = num2;
				entry.BitsUsed = (int)usedPoints;
				TwitchLeaderboardStats leaderboardStats = TwitchManager.LeaderboardStats;
				int num3 = (ExtensionManager.Version == "2.0.1") ? (entry.BitsUsed - num2) : TwitchAction.GetAdjustedBitPriceCeil(entry.BitsUsed - num2);
				if (num3 > 0)
				{
					leaderboardStats.TotalBits += num3;
					leaderboardStats.CheckMostBitsSpent(leaderboardStats.AddBitsUsed(entry.UserName, this.UserColor, num3));
				}
			}
		}

		// Token: 0x040083F1 RID: 33777
		public float SpecialPoints;

		// Token: 0x040083F2 RID: 33778
		public float StandardPoints;

		// Token: 0x040083F3 RID: 33779
		public int BitCredits;

		// Token: 0x040083F4 RID: 33780
		public int UserID = -1;

		// Token: 0x040083F5 RID: 33781
		public string UserColor = "FFFFFF";

		// Token: 0x040083F6 RID: 33782
		public float LastAction = -1f;

		// Token: 0x040083F7 RID: 33783
		public float addPointsUntil;

		// Token: 0x040083F8 RID: 33784
		public bool IsActive;

		// Token: 0x040083F9 RID: 33785
		public bool IsSub;
	}
}
