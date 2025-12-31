using System;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x02001835 RID: 6197
	public interface IUserIdentifierMappingService
	{
		// Token: 0x0600B804 RID: 47108
		bool CanQuery(PlatformUserIdentifierAbs _id);

		// Token: 0x0600B805 RID: 47109
		void QueryMappedAccountDetails(PlatformUserIdentifierAbs _id, EPlatformIdentifier _platform, MappedAccountQueryCallback _callback);

		// Token: 0x0600B806 RID: 47110
		void QueryMappedAccountsDetails(IReadOnlyList<MappedAccountRequest> _requests, MappedAccountsQueryCallback _callback);

		// Token: 0x0600B807 RID: 47111
		bool CanReverseQuery(EPlatformIdentifier _platform, string _platformId);

		// Token: 0x0600B808 RID: 47112
		void ReverseQueryMappedAccountDetails(EPlatformIdentifier _platform, string _platformId, MappedAccountReverseQueryCallback _callback);

		// Token: 0x0600B809 RID: 47113
		void ReverseQueryMappedAccountsDetails(IReadOnlyList<MappedAccountReverseRequest> _requests, MappedAccountsReverseQueryCallback _callback);
	}
}
