using System;

namespace Twitch
{
	// Token: 0x02001579 RID: 5497
	public class TwitchSpawnedEntityEntry
	{
		// Token: 0x040083B0 RID: 33712
		public Entity SpawnedEntity;

		// Token: 0x040083B1 RID: 33713
		public int SpawnedEntityID = -1;

		// Token: 0x040083B2 RID: 33714
		public TwitchActionEntry Action;

		// Token: 0x040083B3 RID: 33715
		public TwitchEventActionEntry Event;

		// Token: 0x040083B4 RID: 33716
		public TwitchVoteEntry Vote;

		// Token: 0x040083B5 RID: 33717
		public TwitchRespawnEntry RespawnEntry;
	}
}
