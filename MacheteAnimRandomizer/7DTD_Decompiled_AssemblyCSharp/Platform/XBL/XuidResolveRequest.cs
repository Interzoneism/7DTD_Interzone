using System;

namespace Platform.XBL
{
	// Token: 0x02001892 RID: 6290
	public class XuidResolveRequest
	{
		// Token: 0x0600B9CE RID: 47566 RVA: 0x0046F098 File Offset: 0x0046D298
		public XuidResolveRequest(PlatformUserIdentifierAbs id)
		{
			this.Id = id;
		}

		// Token: 0x040091AA RID: 37290
		public readonly PlatformUserIdentifierAbs Id;

		// Token: 0x040091AB RID: 37291
		public bool IsSuccess;

		// Token: 0x040091AC RID: 37292
		public ulong Xuid;
	}
}
