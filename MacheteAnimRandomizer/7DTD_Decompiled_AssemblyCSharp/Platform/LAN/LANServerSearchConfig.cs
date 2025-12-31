using System;
using System.Net;

namespace Platform.LAN
{
	// Token: 0x020018FB RID: 6395
	public static class LANServerSearchConfig
	{
		// Token: 0x0400932C RID: 37676
		public static readonly IPAddress MulticastGroupIp = IPAddress.Parse("239.192.0.1");

		// Token: 0x0400932D RID: 37677
		public const int DefaultPort = 11000;
	}
}
