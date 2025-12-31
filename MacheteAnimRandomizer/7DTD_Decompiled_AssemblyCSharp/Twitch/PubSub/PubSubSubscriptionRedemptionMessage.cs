using System;
using Newtonsoft.Json;

namespace Twitch.PubSub
{
	// Token: 0x020015A6 RID: 5542
	public class PubSubSubscriptionRedemptionMessage : BasePubSubMessage
	{
		// Token: 0x170012FA RID: 4858
		// (get) Token: 0x0600AA4F RID: 43599 RVA: 0x00432891 File Offset: 0x00430A91
		// (set) Token: 0x0600AA50 RID: 43600 RVA: 0x00432899 File Offset: 0x00430A99
		public int benefit_end_month { get; set; }

		// Token: 0x170012FB RID: 4859
		// (get) Token: 0x0600AA51 RID: 43601 RVA: 0x004328A2 File Offset: 0x00430AA2
		// (set) Token: 0x0600AA52 RID: 43602 RVA: 0x004328AA File Offset: 0x00430AAA
		public string user_name { get; set; }

		// Token: 0x170012FC RID: 4860
		// (get) Token: 0x0600AA53 RID: 43603 RVA: 0x004328B3 File Offset: 0x00430AB3
		// (set) Token: 0x0600AA54 RID: 43604 RVA: 0x004328BB File Offset: 0x00430ABB
		public string channel_name { get; set; }

		// Token: 0x170012FD RID: 4861
		// (get) Token: 0x0600AA55 RID: 43605 RVA: 0x004328C4 File Offset: 0x00430AC4
		// (set) Token: 0x0600AA56 RID: 43606 RVA: 0x004328CC File Offset: 0x00430ACC
		public string user_id { get; set; }

		// Token: 0x170012FE RID: 4862
		// (get) Token: 0x0600AA57 RID: 43607 RVA: 0x004328D5 File Offset: 0x00430AD5
		// (set) Token: 0x0600AA58 RID: 43608 RVA: 0x004328DD File Offset: 0x00430ADD
		public string channel_id { get; set; }

		// Token: 0x170012FF RID: 4863
		// (get) Token: 0x0600AA59 RID: 43609 RVA: 0x004328E6 File Offset: 0x00430AE6
		// (set) Token: 0x0600AA5A RID: 43610 RVA: 0x004328EE File Offset: 0x00430AEE
		public string sub_plan { get; set; }

		// Token: 0x17001300 RID: 4864
		// (get) Token: 0x0600AA5B RID: 43611 RVA: 0x004328F7 File Offset: 0x00430AF7
		// (set) Token: 0x0600AA5C RID: 43612 RVA: 0x004328FF File Offset: 0x00430AFF
		public string sub_plan_name { get; set; }

		// Token: 0x17001301 RID: 4865
		// (get) Token: 0x0600AA5D RID: 43613 RVA: 0x00432908 File Offset: 0x00430B08
		// (set) Token: 0x0600AA5E RID: 43614 RVA: 0x00432910 File Offset: 0x00430B10
		public int months { get; set; }

		// Token: 0x17001302 RID: 4866
		// (get) Token: 0x0600AA5F RID: 43615 RVA: 0x00432919 File Offset: 0x00430B19
		// (set) Token: 0x0600AA60 RID: 43616 RVA: 0x00432921 File Offset: 0x00430B21
		public int cumulative_months { get; set; }

		// Token: 0x17001303 RID: 4867
		// (get) Token: 0x0600AA61 RID: 43617 RVA: 0x0043292A File Offset: 0x00430B2A
		// (set) Token: 0x0600AA62 RID: 43618 RVA: 0x00432932 File Offset: 0x00430B32
		public string context { get; set; }

		// Token: 0x17001304 RID: 4868
		// (get) Token: 0x0600AA63 RID: 43619 RVA: 0x0043293B File Offset: 0x00430B3B
		// (set) Token: 0x0600AA64 RID: 43620 RVA: 0x00432943 File Offset: 0x00430B43
		public bool is_gift { get; set; }

		// Token: 0x17001305 RID: 4869
		// (get) Token: 0x0600AA65 RID: 43621 RVA: 0x0043294C File Offset: 0x00430B4C
		// (set) Token: 0x0600AA66 RID: 43622 RVA: 0x00432954 File Offset: 0x00430B54
		public int multi_month_duration { get; set; }

		// Token: 0x0600AA67 RID: 43623 RVA: 0x0043295D File Offset: 0x00430B5D
		public static PubSubSubscriptionRedemptionMessage Deserialize(string message)
		{
			return JsonConvert.DeserializeObject<PubSubSubscriptionRedemptionMessage>(message);
		}
	}
}
