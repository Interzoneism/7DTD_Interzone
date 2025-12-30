using System;

namespace GameSparks.Platforms
{
	// Token: 0x02001994 RID: 6548
	public interface IControlledWebSocket : IGameSparksWebSocket
	{
		// Token: 0x0600C098 RID: 49304
		void TriggerOnClose();

		// Token: 0x0600C099 RID: 49305
		void TriggerOnOpen();

		// Token: 0x0600C09A RID: 49306
		void TriggerOnError(string message);

		// Token: 0x0600C09B RID: 49307
		void TriggerOnMessage(string message);

		// Token: 0x0600C09C RID: 49308
		bool Update();

		// Token: 0x1700161A RID: 5658
		// (get) Token: 0x0600C09D RID: 49309
		int SocketId { get; }
	}
}
