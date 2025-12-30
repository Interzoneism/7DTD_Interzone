using System;

namespace Twitch
{
	// Token: 0x0200157C RID: 5500
	public class TwitchEventActionEntry
	{
		// Token: 0x0600A95B RID: 43355 RVA: 0x0042DEBD File Offset: 0x0042C0BD
		public bool HandleEvent(TwitchManager tm)
		{
			if (this.Event.HandleEvent(this.UserName, tm))
			{
				this.IsSent = true;
				return true;
			}
			return false;
		}

		// Token: 0x040083C4 RID: 33732
		public string UserName;

		// Token: 0x040083C5 RID: 33733
		public byte Tier;

		// Token: 0x040083C6 RID: 33734
		public short Count;

		// Token: 0x040083C7 RID: 33735
		public bool IsSent;

		// Token: 0x040083C8 RID: 33736
		public bool IsRetry;

		// Token: 0x040083C9 RID: 33737
		public bool ReadyForRemove;

		// Token: 0x040083CA RID: 33738
		public TwitchActionHistoryEntry HistoryEntry;

		// Token: 0x040083CB RID: 33739
		public BaseTwitchEventEntry Event;
	}
}
