using System;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x0200180F RID: 6159
	public interface IPlayerInteractionsRecorder
	{
		// Token: 0x0600B77E RID: 46974
		void Init(IPlatform owner);

		// Token: 0x0600B77F RID: 46975
		void RecordPlayerInteraction(PlayerInteraction interaction);

		// Token: 0x0600B780 RID: 46976
		void RecordPlayerInteractions(IEnumerable<PlayerInteraction> interactions);

		// Token: 0x0600B781 RID: 46977
		void Destroy();
	}
}
