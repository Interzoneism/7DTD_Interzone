using System;

namespace Twitch
{
	// Token: 0x0200157B RID: 5499
	public class TwitchRecentlyRemovedEntityEntry
	{
		// Token: 0x0600A95A RID: 43354 RVA: 0x0042DE5C File Offset: 0x0042C05C
		public TwitchRecentlyRemovedEntityEntry(TwitchSpawnedEntityEntry entry)
		{
			this.SpawnedEntity = entry.SpawnedEntity;
			this.SpawnedEntityID = entry.SpawnedEntityID;
			this.Action = entry.Action;
			this.Event = entry.Event;
			this.Vote = entry.Vote;
			this.TimeRemaining = 60f;
		}

		// Token: 0x040083BE RID: 33726
		public Entity SpawnedEntity;

		// Token: 0x040083BF RID: 33727
		public int SpawnedEntityID = -1;

		// Token: 0x040083C0 RID: 33728
		public TwitchActionEntry Action;

		// Token: 0x040083C1 RID: 33729
		public TwitchEventActionEntry Event;

		// Token: 0x040083C2 RID: 33730
		public TwitchVoteEntry Vote;

		// Token: 0x040083C3 RID: 33731
		public float TimeRemaining;
	}
}
