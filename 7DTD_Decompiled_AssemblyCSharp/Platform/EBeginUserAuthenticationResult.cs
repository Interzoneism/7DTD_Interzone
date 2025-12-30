using System;

namespace Platform
{
	// Token: 0x020017E6 RID: 6118
	public enum EBeginUserAuthenticationResult
	{
		// Token: 0x04008F99 RID: 36761
		Ok,
		// Token: 0x04008F9A RID: 36762
		InvalidTicket,
		// Token: 0x04008F9B RID: 36763
		DuplicateRequest,
		// Token: 0x04008F9C RID: 36764
		InvalidVersion,
		// Token: 0x04008F9D RID: 36765
		GameMismatch,
		// Token: 0x04008F9E RID: 36766
		ExpiredTicket
	}
}
