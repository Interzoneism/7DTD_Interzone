using System;

namespace UnityEngine.Networking
{
	// Token: 0x020015D7 RID: 5591
	public enum NetworkError
	{
		// Token: 0x040085C5 RID: 34245
		Ok,
		// Token: 0x040085C6 RID: 34246
		WrongHost,
		// Token: 0x040085C7 RID: 34247
		WrongConnection,
		// Token: 0x040085C8 RID: 34248
		WrongChannel,
		// Token: 0x040085C9 RID: 34249
		NoResources,
		// Token: 0x040085CA RID: 34250
		BadMessage,
		// Token: 0x040085CB RID: 34251
		Timeout,
		// Token: 0x040085CC RID: 34252
		MessageToLong,
		// Token: 0x040085CD RID: 34253
		WrongOperation,
		// Token: 0x040085CE RID: 34254
		VersionMismatch,
		// Token: 0x040085CF RID: 34255
		CRCMismatch,
		// Token: 0x040085D0 RID: 34256
		DNSFailure,
		// Token: 0x040085D1 RID: 34257
		UsageError
	}
}
