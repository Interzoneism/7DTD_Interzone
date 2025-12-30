using System;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x02001834 RID: 6196
	public interface IUserDetailsService
	{
		// Token: 0x0600B802 RID: 47106
		void Init(IPlatform owner);

		// Token: 0x0600B803 RID: 47107
		void RequestUserDetailsUpdate(IReadOnlyList<UserDetailsRequest> requestedUsers, UserDetailsRequestCompleteHandler onComplete);
	}
}
