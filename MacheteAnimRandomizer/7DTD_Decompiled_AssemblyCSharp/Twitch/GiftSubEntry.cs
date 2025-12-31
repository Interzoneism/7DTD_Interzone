using System;

namespace Twitch
{
	// Token: 0x02001582 RID: 5506
	public class GiftSubEntry
	{
		// Token: 0x0600A991 RID: 43409 RVA: 0x0042FAD8 File Offset: 0x0042DCD8
		public GiftSubEntry(string userName, int userID, TwitchSubEventEntry.SubTierTypes tier)
		{
			this.UserName = userName;
			this.UserID = userID;
			this.TimeRemaining = 1f;
			this.SubCount = 1;
			this.Tier = tier;
		}

		// Token: 0x0600A992 RID: 43410 RVA: 0x0042FB36 File Offset: 0x0042DD36
		public void AddSub()
		{
			this.SubCount++;
			this.TimeRemaining = 1f;
		}

		// Token: 0x0600A993 RID: 43411 RVA: 0x0042FB51 File Offset: 0x0042DD51
		public bool Update(float deltaTime)
		{
			this.TimeRemaining -= deltaTime;
			return this.TimeRemaining <= 0f;
		}

		// Token: 0x040083FA RID: 33786
		public float TimeRemaining = 1f;

		// Token: 0x040083FB RID: 33787
		public string UserName = "";

		// Token: 0x040083FC RID: 33788
		public int UserID = -1;

		// Token: 0x040083FD RID: 33789
		public int SubCount;

		// Token: 0x040083FE RID: 33790
		public TwitchSubEventEntry.SubTierTypes Tier = TwitchSubEventEntry.SubTierTypes.Tier1;
	}
}
