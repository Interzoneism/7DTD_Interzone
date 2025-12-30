using System;

namespace Platform
{
	// Token: 0x0200183B RID: 6203
	public class MappedAccountReverseRequest
	{
		// Token: 0x0600B817 RID: 47127 RVA: 0x004689F7 File Offset: 0x00466BF7
		public MappedAccountReverseRequest(EPlatformIdentifier _platform, string _id)
		{
			this.Platform = _platform;
			this.Id = _id;
		}

		// Token: 0x0400902F RID: 36911
		public readonly EPlatformIdentifier Platform;

		// Token: 0x04009030 RID: 36912
		public readonly string Id;

		// Token: 0x04009031 RID: 36913
		public MappedAccountQueryResult Result;

		// Token: 0x04009032 RID: 36914
		public PlatformUserIdentifierAbs PlatformId;
	}
}
