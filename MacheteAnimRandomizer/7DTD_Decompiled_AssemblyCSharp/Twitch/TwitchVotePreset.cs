using System;

namespace Twitch
{
	// Token: 0x02001587 RID: 5511
	public class TwitchVotePreset
	{
		// Token: 0x0400844A RID: 33866
		public string Name;

		// Token: 0x0400844B RID: 33867
		public bool IsDefault;

		// Token: 0x0400844C RID: 33868
		public bool IsEmpty;

		// Token: 0x0400844D RID: 33869
		public string Title;

		// Token: 0x0400844E RID: 33870
		public string Description;

		// Token: 0x0400844F RID: 33871
		public TwitchVotingManager.BossVoteSettings BossVoteSetting = TwitchVotingManager.BossVoteSettings.Standard;
	}
}
