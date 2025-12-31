using System;

// Token: 0x020007D4 RID: 2004
public interface IProtocolManagerProtocolInterface
{
	// Token: 0x170005D0 RID: 1488
	// (get) Token: 0x060039B9 RID: 14777
	bool IsServer { get; }

	// Token: 0x170005D1 RID: 1489
	// (get) Token: 0x060039BA RID: 14778
	bool IsClient { get; }

	// Token: 0x060039BB RID: 14779
	void InvalidPasswordEv();

	// Token: 0x060039BC RID: 14780
	void ConnectionFailedEv(string _msg);

	// Token: 0x060039BD RID: 14781
	void DisconnectedFromServerEv(string _msg);
}
