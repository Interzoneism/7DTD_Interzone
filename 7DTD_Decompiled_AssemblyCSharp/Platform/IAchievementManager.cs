using System;

namespace Platform
{
	// Token: 0x020017D5 RID: 6101
	public interface IAchievementManager
	{
		// Token: 0x0600B653 RID: 46675
		void Init(IPlatform _owner);

		// Token: 0x0600B654 RID: 46676
		void ShowAchievementsUi();

		// Token: 0x0600B655 RID: 46677
		bool IsAchievementStatSupported(EnumAchievementDataStat _stat);

		// Token: 0x0600B656 RID: 46678
		void SetAchievementStat(EnumAchievementDataStat _stat, int _value);

		// Token: 0x0600B657 RID: 46679
		void SetAchievementStat(EnumAchievementDataStat _stat, float _value);

		// Token: 0x0600B658 RID: 46680
		void ResetStats(bool _andAchievements);

		// Token: 0x0600B659 RID: 46681
		void UnlockAllAchievements();

		// Token: 0x0600B65A RID: 46682
		void Destroy();
	}
}
