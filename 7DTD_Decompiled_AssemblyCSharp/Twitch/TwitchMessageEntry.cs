using System;

namespace Twitch
{
	// Token: 0x0200157E RID: 5502
	public class TwitchMessageEntry
	{
		// Token: 0x0600A95E RID: 43358 RVA: 0x0042DEF7 File Offset: 0x0042C0F7
		public TwitchMessageEntry(string msg, string sound)
		{
			this.Message = msg;
			this.Sound = sound;
		}

		// Token: 0x040083CE RID: 33742
		public string Message = "";

		// Token: 0x040083CF RID: 33743
		public string Sound = "";
	}
}
