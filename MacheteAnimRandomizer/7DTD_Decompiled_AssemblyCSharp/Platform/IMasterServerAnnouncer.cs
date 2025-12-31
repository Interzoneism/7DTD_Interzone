using System;

namespace Platform
{
	// Token: 0x020017ED RID: 6125
	public interface IMasterServerAnnouncer
	{
		// Token: 0x0600B6BD RID: 46781
		void Init(IPlatform _owner);

		// Token: 0x0600B6BE RID: 46782
		void Update();

		// Token: 0x17001498 RID: 5272
		// (get) Token: 0x0600B6BF RID: 46783
		bool GameServerInitialized { get; }

		// Token: 0x0600B6C0 RID: 46784
		string GetServerPorts();

		// Token: 0x0600B6C1 RID: 46785
		void AdvertiseServer(Action _onServerRegistered);

		// Token: 0x0600B6C2 RID: 46786
		void StopServer();
	}
}
