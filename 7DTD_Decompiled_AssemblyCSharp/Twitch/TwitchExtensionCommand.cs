using System;
using System.Globalization;
using System.Net;

namespace Twitch
{
	// Token: 0x02001509 RID: 5385
	public class TwitchExtensionCommand
	{
		// Token: 0x0600A67B RID: 42619 RVA: 0x0041C45C File Offset: 0x0041A65C
		public TwitchExtensionCommand(HttpListenerRequest _req)
		{
			this.userId = StringParsers.ParseSInt32(_req.QueryString.Get(0), 0, -1, NumberStyles.Integer);
			this.command = "#" + _req.QueryString.Get(1);
			this.id = StringParsers.ParseSInt32(_req.QueryString.Get(2), 0, -1, NumberStyles.Integer);
			this.isRerun = StringParsers.ParseBool(_req.QueryString.Get(3), 0, -1, true);
			Log.Out(string.Format("{0}: {1} : {2}", this.userId, this.command, this.id));
		}

		// Token: 0x040080B2 RID: 32946
		public int userId;

		// Token: 0x040080B3 RID: 32947
		public string command;

		// Token: 0x040080B4 RID: 32948
		public int id;

		// Token: 0x040080B5 RID: 32949
		public bool isRerun;
	}
}
