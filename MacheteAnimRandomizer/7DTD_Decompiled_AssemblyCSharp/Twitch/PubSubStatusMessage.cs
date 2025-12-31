using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200150D RID: 5389
	[Serializable]
	public class PubSubStatusMessage
	{
		// Token: 0x040080C2 RID: 32962
		public string opaqueId;

		// Token: 0x040080C3 RID: 32963
		public string displayName;

		// Token: 0x040080C4 RID: 32964
		public string status;

		// Token: 0x040080C5 RID: 32965
		public List<string> party;

		// Token: 0x040080C6 RID: 32966
		public string commands;

		// Token: 0x040080C7 RID: 32967
		public string language;

		// Token: 0x040080C8 RID: 32968
		public List<string> actionTypes;
	}
}
