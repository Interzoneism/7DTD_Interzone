using System;

namespace Twitch.PubSub
{
	// Token: 0x020015A4 RID: 5540
	public class PubSubListenMessage : BasePubSubMessage
	{
		// Token: 0x0600AA49 RID: 43593 RVA: 0x0043285C File Offset: 0x00430A5C
		public PubSubListenMessage()
		{
			base.type = "LISTEN";
		}

		// Token: 0x040084FF RID: 34047
		public PubSubListenMessage.PubSubListenData data;

		// Token: 0x020015A5 RID: 5541
		public class PubSubListenData
		{
			// Token: 0x170012F8 RID: 4856
			// (get) Token: 0x0600AA4A RID: 43594 RVA: 0x0043286F File Offset: 0x00430A6F
			// (set) Token: 0x0600AA4B RID: 43595 RVA: 0x00432877 File Offset: 0x00430A77
			public string[] topics { get; set; }

			// Token: 0x170012F9 RID: 4857
			// (get) Token: 0x0600AA4C RID: 43596 RVA: 0x00432880 File Offset: 0x00430A80
			// (set) Token: 0x0600AA4D RID: 43597 RVA: 0x00432888 File Offset: 0x00430A88
			public string auth_token { get; set; }
		}
	}
}
