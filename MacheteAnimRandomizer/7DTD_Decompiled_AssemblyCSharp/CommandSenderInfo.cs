using System;

// Token: 0x02000281 RID: 641
public struct CommandSenderInfo
{
	// Token: 0x04000BE0 RID: 3040
	public bool IsLocalGame;

	// Token: 0x04000BE1 RID: 3041
	public ClientInfo RemoteClientInfo;

	// Token: 0x04000BE2 RID: 3042
	public IConsoleConnection NetworkConnection;
}
