using System;

namespace Twitch
{
	// Token: 0x02001569 RID: 5481
	public class TwitchLeaderboardEntry
	{
		// Token: 0x0600A84D RID: 43085 RVA: 0x0042405A File Offset: 0x0042225A
		public TwitchLeaderboardEntry(string username, string usercolor, int kills)
		{
			this.UserName = username;
			this.UserColor = ((usercolor == null) ? "FFFFFF" : usercolor);
			this.Kills = kills;
		}

		// Token: 0x040082AB RID: 33451
		public string UserName;

		// Token: 0x040082AC RID: 33452
		public string UserColor;

		// Token: 0x040082AD RID: 33453
		public int Kills;
	}
}
