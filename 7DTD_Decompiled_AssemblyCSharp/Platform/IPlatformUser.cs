using System;

namespace Platform
{
	// Token: 0x0200185E RID: 6238
	public interface IPlatformUser
	{
		// Token: 0x17001506 RID: 5382
		// (get) Token: 0x0600B8FC RID: 47356
		PlatformUserIdentifierAbs PrimaryId { get; }

		// Token: 0x17001507 RID: 5383
		// (get) Token: 0x0600B8FD RID: 47357
		// (set) Token: 0x0600B8FE RID: 47358
		PlatformUserIdentifierAbs NativeId { get; set; }
	}
}
