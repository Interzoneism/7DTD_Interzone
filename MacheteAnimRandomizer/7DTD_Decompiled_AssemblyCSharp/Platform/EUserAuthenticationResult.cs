using System;

namespace Platform
{
	// Token: 0x020017E7 RID: 6119
	public enum EUserAuthenticationResult
	{
		// Token: 0x04008FA0 RID: 36768
		Ok,
		// Token: 0x04008FA1 RID: 36769
		UserNotConnectedToPlatform,
		// Token: 0x04008FA2 RID: 36770
		NoLicenseOrExpired,
		// Token: 0x04008FA3 RID: 36771
		PlatformBanned,
		// Token: 0x04008FA4 RID: 36772
		LoggedInElseWhere,
		// Token: 0x04008FA5 RID: 36773
		PlatformBanCheckTimedOut,
		// Token: 0x04008FA6 RID: 36774
		AuthTicketCanceled,
		// Token: 0x04008FA7 RID: 36775
		AuthTicketInvalidAlreadyUsed,
		// Token: 0x04008FA8 RID: 36776
		AuthTicketInvalid,
		// Token: 0x04008FA9 RID: 36777
		PublisherIssuedBan,
		// Token: 0x04008FAA RID: 36778
		EosTicketFailed = 50
	}
}
