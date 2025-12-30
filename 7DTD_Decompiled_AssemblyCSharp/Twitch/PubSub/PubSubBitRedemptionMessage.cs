using System;
using Newtonsoft.Json;

namespace Twitch.PubSub
{
	// Token: 0x02001598 RID: 5528
	public class PubSubBitRedemptionMessage : BasePubSubMessage
	{
		// Token: 0x0600AA0C RID: 43532 RVA: 0x004326A7 File Offset: 0x004308A7
		public static PubSubBitRedemptionMessage Deserialize(string message)
		{
			return JsonConvert.DeserializeObject<PubSubBitRedemptionMessage>(message);
		}

		// Token: 0x040084E6 RID: 34022
		public PubSubBitRedemptionMessage.BitRedemptionData data;

		// Token: 0x02001599 RID: 5529
		public class BitRedemptionData : EventArgs
		{
			// Token: 0x170012E1 RID: 4833
			// (get) Token: 0x0600AA0E RID: 43534 RVA: 0x004326B7 File Offset: 0x004308B7
			// (set) Token: 0x0600AA0F RID: 43535 RVA: 0x004326BF File Offset: 0x004308BF
			public string user_name { get; set; }

			// Token: 0x170012E2 RID: 4834
			// (get) Token: 0x0600AA10 RID: 43536 RVA: 0x004326C8 File Offset: 0x004308C8
			// (set) Token: 0x0600AA11 RID: 43537 RVA: 0x004326D0 File Offset: 0x004308D0
			public string channel_name { get; set; }

			// Token: 0x170012E3 RID: 4835
			// (get) Token: 0x0600AA12 RID: 43538 RVA: 0x004326D9 File Offset: 0x004308D9
			// (set) Token: 0x0600AA13 RID: 43539 RVA: 0x004326E1 File Offset: 0x004308E1
			public string user_id { get; set; }

			// Token: 0x170012E4 RID: 4836
			// (get) Token: 0x0600AA14 RID: 43540 RVA: 0x004326EA File Offset: 0x004308EA
			// (set) Token: 0x0600AA15 RID: 43541 RVA: 0x004326F2 File Offset: 0x004308F2
			public string channel_id { get; set; }

			// Token: 0x170012E5 RID: 4837
			// (get) Token: 0x0600AA16 RID: 43542 RVA: 0x004326FB File Offset: 0x004308FB
			// (set) Token: 0x0600AA17 RID: 43543 RVA: 0x00432703 File Offset: 0x00430903
			public string chat_message { get; set; }

			// Token: 0x170012E6 RID: 4838
			// (get) Token: 0x0600AA18 RID: 43544 RVA: 0x0043270C File Offset: 0x0043090C
			// (set) Token: 0x0600AA19 RID: 43545 RVA: 0x00432714 File Offset: 0x00430914
			public int bits_used { get; set; }

			// Token: 0x170012E7 RID: 4839
			// (get) Token: 0x0600AA1A RID: 43546 RVA: 0x0043271D File Offset: 0x0043091D
			// (set) Token: 0x0600AA1B RID: 43547 RVA: 0x00432725 File Offset: 0x00430925
			public int total_bits_used { get; set; }

			// Token: 0x170012E8 RID: 4840
			// (get) Token: 0x0600AA1C RID: 43548 RVA: 0x0043272E File Offset: 0x0043092E
			// (set) Token: 0x0600AA1D RID: 43549 RVA: 0x00432736 File Offset: 0x00430936
			public bool is_anonymous { get; set; }
		}
	}
}
