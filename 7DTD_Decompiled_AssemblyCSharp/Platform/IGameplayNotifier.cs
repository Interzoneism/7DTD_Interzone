using System;

namespace Platform
{
	// Token: 0x020017E9 RID: 6121
	public interface IGameplayNotifier
	{
		// Token: 0x0600B6AC RID: 46764
		void Init(IPlatform platform);

		// Token: 0x0600B6AD RID: 46765
		void GameplayStart(bool isOnlineMultiplayer, bool isCrossplayEnabled);

		// Token: 0x0600B6AE RID: 46766
		void EndOnlineMultiplayer();

		// Token: 0x0600B6AF RID: 46767
		void GameplayEnd();
	}
}
