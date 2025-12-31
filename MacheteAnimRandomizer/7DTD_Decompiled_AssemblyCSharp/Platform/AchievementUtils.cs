using System;

namespace Platform
{
	// Token: 0x020017D6 RID: 6102
	public static class AchievementUtils
	{
		// Token: 0x0600B65B RID: 46683 RVA: 0x0046779A File Offset: 0x0046599A
		public static bool IsCreativeModeActive()
		{
			return GamePrefs.GetString(EnumGamePrefs.GameMode).Equals(GameModeCreative.TypeName) || GameStats.GetBool(EnumGameStats.IsCreativeMenuEnabled) || GamePrefs.GetBool(EnumGamePrefs.CreativeMenuEnabled) || GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled);
		}
	}
}
