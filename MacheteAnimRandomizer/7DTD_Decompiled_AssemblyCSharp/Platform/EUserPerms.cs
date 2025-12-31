using System;

namespace Platform
{
	// Token: 0x0200182C RID: 6188
	[Flags]
	public enum EUserPerms
	{
		// Token: 0x04009018 RID: 36888
		Multiplayer = 1,
		// Token: 0x04009019 RID: 36889
		Communication = 2,
		// Token: 0x0400901A RID: 36890
		Crossplay = 4,
		// Token: 0x0400901B RID: 36891
		HostMultiplayer = 8,
		// Token: 0x0400901C RID: 36892
		All = 15
	}
}
