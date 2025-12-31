using System;

namespace Platform
{
	// Token: 0x020017B3 RID: 6067
	public static class EnumAchievementDataStatExtensions
	{
		// Token: 0x0600B581 RID: 46465 RVA: 0x00463F93 File Offset: 0x00462193
		public static bool IsSupported(this EnumAchievementDataStat _stat)
		{
			IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
			return achievementManager == null || achievementManager.IsAchievementStatSupported(_stat);
		}
	}
}
