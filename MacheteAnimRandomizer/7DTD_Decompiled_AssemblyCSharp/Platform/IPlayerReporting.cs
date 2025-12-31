using System;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x02001810 RID: 6160
	public interface IPlayerReporting
	{
		// Token: 0x0600B782 RID: 46978
		void Init(IPlatform _owner);

		// Token: 0x0600B783 RID: 46979
		IList<IPlayerReporting.PlayerReportCategory> ReportCategories();

		// Token: 0x0600B784 RID: 46980
		void ReportPlayer(PlatformUserIdentifierAbs _reportedUserCross, IPlayerReporting.PlayerReportCategory _reportCategory, string _message, Action<bool> _reportCompleteCallback);

		// Token: 0x0600B785 RID: 46981
		IPlayerReporting.PlayerReportCategory GetPlayerReportCategoryMapping(EnumReportCategory _reportCategory);

		// Token: 0x02001811 RID: 6161
		public abstract class PlayerReportCategory
		{
			// Token: 0x0600B786 RID: 46982
			public abstract override string ToString();

			// Token: 0x0600B787 RID: 46983 RVA: 0x0000A7E3 File Offset: 0x000089E3
			[PublicizedFrom(EAccessModifier.Protected)]
			public PlayerReportCategory()
			{
			}
		}
	}
}
