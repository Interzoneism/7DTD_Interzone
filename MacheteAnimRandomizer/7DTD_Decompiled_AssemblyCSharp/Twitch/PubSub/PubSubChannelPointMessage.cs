using System;
using Newtonsoft.Json;

namespace Twitch.PubSub
{
	// Token: 0x0200159A RID: 5530
	public class PubSubChannelPointMessage : BasePubSubMessage
	{
		// Token: 0x0600AA1F RID: 43551 RVA: 0x0043273F File Offset: 0x0043093F
		public static PubSubChannelPointMessage Deserialize(string message)
		{
			return JsonConvert.DeserializeObject<PubSubChannelPointMessage>(message);
		}

		// Token: 0x040084EF RID: 34031
		public PubSubChannelPointMessage.ChannelRedemptionData data;

		// Token: 0x0200159B RID: 5531
		public class ChannelRedemptionData : EventArgs
		{
			// Token: 0x170012E9 RID: 4841
			// (get) Token: 0x0600AA21 RID: 43553 RVA: 0x00432747 File Offset: 0x00430947
			// (set) Token: 0x0600AA22 RID: 43554 RVA: 0x0043274F File Offset: 0x0043094F
			public PubSubChannelPointMessage.Redemption redemption { get; set; }
		}

		// Token: 0x0200159C RID: 5532
		public class Redemption
		{
			// Token: 0x170012EA RID: 4842
			// (get) Token: 0x0600AA24 RID: 43556 RVA: 0x00432758 File Offset: 0x00430958
			// (set) Token: 0x0600AA25 RID: 43557 RVA: 0x00432760 File Offset: 0x00430960
			public PubSubChannelPointMessage.User user { get; set; }

			// Token: 0x170012EB RID: 4843
			// (get) Token: 0x0600AA26 RID: 43558 RVA: 0x00432769 File Offset: 0x00430969
			// (set) Token: 0x0600AA27 RID: 43559 RVA: 0x00432771 File Offset: 0x00430971
			public PubSubChannelPointMessage.Reward reward { get; set; }
		}

		// Token: 0x0200159D RID: 5533
		public class User
		{
			// Token: 0x170012EC RID: 4844
			// (get) Token: 0x0600AA29 RID: 43561 RVA: 0x0043277A File Offset: 0x0043097A
			// (set) Token: 0x0600AA2A RID: 43562 RVA: 0x00432782 File Offset: 0x00430982
			public string login { get; set; }

			// Token: 0x170012ED RID: 4845
			// (get) Token: 0x0600AA2B RID: 43563 RVA: 0x0043278B File Offset: 0x0043098B
			// (set) Token: 0x0600AA2C RID: 43564 RVA: 0x00432793 File Offset: 0x00430993
			public string display_name { get; set; }
		}

		// Token: 0x0200159E RID: 5534
		public class Reward
		{
			// Token: 0x170012EE RID: 4846
			// (get) Token: 0x0600AA2E RID: 43566 RVA: 0x0043279C File Offset: 0x0043099C
			// (set) Token: 0x0600AA2F RID: 43567 RVA: 0x004327A4 File Offset: 0x004309A4
			public string title { get; set; }
		}
	}
}
