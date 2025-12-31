using System;

namespace Platform
{
	// Token: 0x02001861 RID: 6241
	public static class IPlatformUserBlockedDataExtensions
	{
		// Token: 0x0600B907 RID: 47367 RVA: 0x0046BF93 File Offset: 0x0046A193
		public static bool IsBlocked(this IPlatformUserBlockedData blockedData)
		{
			return blockedData.State.IsBlocked();
		}
	}
}
