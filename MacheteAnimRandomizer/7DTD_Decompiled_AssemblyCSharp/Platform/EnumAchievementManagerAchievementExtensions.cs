using System;

namespace Platform
{
	// Token: 0x020017B0 RID: 6064
	public static class EnumAchievementManagerAchievementExtensions
	{
		// Token: 0x0600B580 RID: 46464 RVA: 0x00463F86 File Offset: 0x00462186
		public static bool IsSupported(this EnumAchievementManagerAchievement _achievement)
		{
			return AchievementData.GetStat(_achievement).IsSupported();
		}
	}
}
