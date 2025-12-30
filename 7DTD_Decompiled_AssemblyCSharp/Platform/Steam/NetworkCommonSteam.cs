using System;
using Steamworks;

namespace Platform.Steam
{
	// Token: 0x020018C8 RID: 6344
	public static class NetworkCommonSteam
	{
		// Token: 0x020018C9 RID: 6345
		public enum ESteamNetChannels : byte
		{
			// Token: 0x04009277 RID: 37495
			NetpackageChannel0,
			// Token: 0x04009278 RID: 37496
			NetpackageChannel1,
			// Token: 0x04009279 RID: 37497
			Authentication = 50,
			// Token: 0x0400927A RID: 37498
			Ping = 60
		}

		// Token: 0x020018CA RID: 6346
		public readonly struct SendInfo
		{
			// Token: 0x0600BB5B RID: 47963 RVA: 0x00474A7A File Offset: 0x00472C7A
			public SendInfo(CSteamID _recipient, ArrayListMP<byte> _data)
			{
				this.Recipient = _recipient;
				this.Data = _data;
			}

			// Token: 0x0400927B RID: 37499
			public readonly CSteamID Recipient;

			// Token: 0x0400927C RID: 37500
			public readonly ArrayListMP<byte> Data;
		}
	}
}
