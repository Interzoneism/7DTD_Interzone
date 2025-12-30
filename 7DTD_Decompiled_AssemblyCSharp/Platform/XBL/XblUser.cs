using System;

namespace Platform.XBL
{
	// Token: 0x02001889 RID: 6281
	public interface XblUser : IUserClient
	{
		// Token: 0x17001525 RID: 5413
		// (get) Token: 0x0600B9A8 RID: 47528
		SocialManagerXbl SocialManager { get; }

		// Token: 0x17001526 RID: 5414
		// (get) Token: 0x0600B9A9 RID: 47529
		MultiplayerActivityQueryManager MultiplayerActivityQueryManager { get; }
	}
}
