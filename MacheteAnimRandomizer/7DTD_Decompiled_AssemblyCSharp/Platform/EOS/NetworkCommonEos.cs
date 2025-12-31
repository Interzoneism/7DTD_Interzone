using System;

namespace Platform.EOS
{
	// Token: 0x0200191C RID: 6428
	public static class NetworkCommonEos
	{
		// Token: 0x040093CD RID: 37837
		public const int MaxUsedPacketSize = 1120;

		// Token: 0x0200191D RID: 6429
		public enum ESteamNetChannels : byte
		{
			// Token: 0x040093CF RID: 37839
			NetpackageChannel0,
			// Token: 0x040093D0 RID: 37840
			NetpackageChannel1,
			// Token: 0x040093D1 RID: 37841
			Authentication = 50,
			// Token: 0x040093D2 RID: 37842
			Ping = 60
		}

		// Token: 0x0200191E RID: 6430
		public readonly struct SendInfo
		{
			// Token: 0x0600BDCF RID: 48591 RVA: 0x0047EDC6 File Offset: 0x0047CFC6
			public SendInfo(ClientInfo _clientInfo, ArrayListMP<byte> _data)
			{
				this.Recipient = _clientInfo;
				this.Data = _data;
			}

			// Token: 0x040093D3 RID: 37843
			public readonly ClientInfo Recipient;

			// Token: 0x040093D4 RID: 37844
			public readonly ArrayListMP<byte> Data;
		}
	}
}
