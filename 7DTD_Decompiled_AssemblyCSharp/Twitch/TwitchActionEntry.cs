using System;

namespace Twitch
{
	// Token: 0x02001526 RID: 5414
	public class TwitchActionEntry
	{
		// Token: 0x17001249 RID: 4681
		// (get) Token: 0x0600A6FD RID: 42749 RVA: 0x0041F199 File Offset: 0x0041D399
		public int ActionCost
		{
			get
			{
				return this.Action.CurrentCost;
			}
		}

		// Token: 0x0600A6FE RID: 42750 RVA: 0x0041F1A8 File Offset: 0x0041D3A8
		public TwitchActionHistoryEntry SetupHistoryEntry(ViewerEntry viewerEntry)
		{
			string target = (this.Target != null) ? this.Target.EntityName : "";
			this.HistoryEntry = new TwitchActionHistoryEntry(this.UserName, viewerEntry.UserColor, this.Action, null, null)
			{
				UserID = viewerEntry.UserID,
				PointsSpent = this.Action.CurrentCost,
				Target = target
			};
			this.HistoryEntry.ActionEntry = this;
			return this.HistoryEntry;
		}

		// Token: 0x040081B6 RID: 33206
		public string UserName;

		// Token: 0x040081B7 RID: 33207
		public EntityPlayer Target;

		// Token: 0x040081B8 RID: 33208
		public bool ReadyForRemove;

		// Token: 0x040081B9 RID: 33209
		public TwitchVoteEntry VoteEntry;

		// Token: 0x040081BA RID: 33210
		public TwitchAction Action;

		// Token: 0x040081BB RID: 33211
		public bool IsSent;

		// Token: 0x040081BC RID: 33212
		public bool ChannelNotify = true;

		// Token: 0x040081BD RID: 33213
		public bool IsBitAction;

		// Token: 0x040081BE RID: 33214
		public bool IsReRun;

		// Token: 0x040081BF RID: 33215
		public bool IsRespawn;

		// Token: 0x040081C0 RID: 33216
		public int SpecialPointsUsed;

		// Token: 0x040081C1 RID: 33217
		public int StandardPointsUsed;

		// Token: 0x040081C2 RID: 33218
		public int BitsUsed;

		// Token: 0x040081C3 RID: 33219
		public int CreditsUsed;

		// Token: 0x040081C4 RID: 33220
		public TwitchActionHistoryEntry HistoryEntry;
	}
}
