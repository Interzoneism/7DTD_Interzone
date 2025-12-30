using System;

namespace Platform
{
	// Token: 0x02001831 RID: 6193
	public class UserDetailsRequest
	{
		// Token: 0x0600B7F8 RID: 47096 RVA: 0x004689B0 File Offset: 0x00466BB0
		public UserDetailsRequest(PlatformUserIdentifierAbs id)
		{
			this.Id = id;
			this.NativePlatform = id.PlatformIdentifier;
		}

		// Token: 0x0600B7F9 RID: 47097 RVA: 0x004689CB File Offset: 0x00466BCB
		public UserDetailsRequest(PlatformUserIdentifierAbs id, EPlatformIdentifier platform)
		{
			this.Id = id;
			this.NativePlatform = platform;
		}

		// Token: 0x04009022 RID: 36898
		public readonly PlatformUserIdentifierAbs Id;

		// Token: 0x04009023 RID: 36899
		public readonly EPlatformIdentifier NativePlatform;

		// Token: 0x04009024 RID: 36900
		public PlatformUserDetails details;

		// Token: 0x04009025 RID: 36901
		public bool IsSuccess;
	}
}
