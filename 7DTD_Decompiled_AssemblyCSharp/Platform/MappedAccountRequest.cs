using System;

namespace Platform
{
	// Token: 0x02001838 RID: 6200
	public class MappedAccountRequest
	{
		// Token: 0x0600B80E RID: 47118 RVA: 0x004689E1 File Offset: 0x00466BE1
		public MappedAccountRequest(PlatformUserIdentifierAbs _id, EPlatformIdentifier _platform)
		{
			this.Id = _id;
			this.Platform = _platform;
		}

		// Token: 0x0400902A RID: 36906
		public readonly PlatformUserIdentifierAbs Id;

		// Token: 0x0400902B RID: 36907
		public readonly EPlatformIdentifier Platform;

		// Token: 0x0400902C RID: 36908
		public string MappedAccountId;

		// Token: 0x0400902D RID: 36909
		public string DisplayName;

		// Token: 0x0400902E RID: 36910
		public MappedAccountQueryResult Result;
	}
}
