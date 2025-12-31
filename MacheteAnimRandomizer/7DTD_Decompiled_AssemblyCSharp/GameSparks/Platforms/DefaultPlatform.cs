using System;
using GameSparks.Core;

namespace GameSparks.Platforms
{
	// Token: 0x02001992 RID: 6546
	public class DefaultPlatform : PlatformBase
	{
		// Token: 0x0600C092 RID: 49298 RVA: 0x0048F3EB File Offset: 0x0048D5EB
		public override IGameSparksTimer GetTimer()
		{
			return new GameSparksTimer();
		}

		// Token: 0x0600C093 RID: 49299 RVA: 0x0048F3F2 File Offset: 0x0048D5F2
		public override string MakeHmac(string stringToHmac, string secret)
		{
			return GameSparksUtil.MakeHmac(stringToHmac, secret);
		}

		// Token: 0x0600C094 RID: 49300 RVA: 0x0048F3FB File Offset: 0x0048D5FB
		public override IGameSparksWebSocket GetSocket(string url, Action<string> messageReceived, Action closed, Action opened, Action<string> error)
		{
			GameSparksWebSocket gameSparksWebSocket = new GameSparksWebSocket();
			gameSparksWebSocket.Initialize(url, messageReceived, closed, opened, error);
			return gameSparksWebSocket;
		}

		// Token: 0x0600C095 RID: 49301 RVA: 0x000424BD File Offset: 0x000406BD
		public override IGameSparksWebSocket GetBinarySocket(string url, Action<byte[]> messageReceived, Action closed, Action opened, Action<string> error)
		{
			throw new NotImplementedException();
		}
	}
}
