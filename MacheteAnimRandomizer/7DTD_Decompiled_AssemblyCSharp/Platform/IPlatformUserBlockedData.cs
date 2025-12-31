using System;

namespace Platform
{
	// Token: 0x02001860 RID: 6240
	public interface IPlatformUserBlockedData
	{
		// Token: 0x1700150A RID: 5386
		// (get) Token: 0x0600B903 RID: 47363
		EBlockType Type { get; }

		// Token: 0x1700150B RID: 5387
		// (get) Token: 0x0600B904 RID: 47364
		EUserBlockState State { get; }

		// Token: 0x1700150C RID: 5388
		// (get) Token: 0x0600B905 RID: 47365
		// (set) Token: 0x0600B906 RID: 47366
		bool Locally { get; set; }
	}
}
