using System;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x0200185F RID: 6239
	public interface IPlatformUserData : IPlatformUser
	{
		// Token: 0x17001508 RID: 5384
		// (get) Token: 0x0600B8FF RID: 47359
		IReadOnlyDictionary<EBlockType, IPlatformUserBlockedData> Blocked { get; }

		// Token: 0x0600B900 RID: 47360
		void MarkBlockedStateChanged();

		// Token: 0x17001509 RID: 5385
		// (get) Token: 0x0600B901 RID: 47361
		string Name { get; }

		// Token: 0x0600B902 RID: 47362
		void RequestUserDetailsUpdate();
	}
}
