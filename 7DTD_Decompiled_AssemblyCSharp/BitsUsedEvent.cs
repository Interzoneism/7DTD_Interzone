using System;
using Newtonsoft.Json;

// Token: 0x02000B19 RID: 2841
public class BitsUsedEvent
{
	// Token: 0x170008DC RID: 2268
	// (get) Token: 0x06005874 RID: 22644 RVA: 0x0023CA5C File Offset: 0x0023AC5C
	// (set) Token: 0x06005875 RID: 22645 RVA: 0x0023CA64 File Offset: 0x0023AC64
	[JsonProperty("is_anonymous")]
	public bool IsAnonymous { get; set; }

	// Token: 0x170008DD RID: 2269
	// (get) Token: 0x06005876 RID: 22646 RVA: 0x0023CA6D File Offset: 0x0023AC6D
	// (set) Token: 0x06005877 RID: 22647 RVA: 0x0023CA75 File Offset: 0x0023AC75
	[JsonProperty("user_id")]
	public string UserId { get; set; } = "";

	// Token: 0x170008DE RID: 2270
	// (get) Token: 0x06005878 RID: 22648 RVA: 0x0023CA7E File Offset: 0x0023AC7E
	// (set) Token: 0x06005879 RID: 22649 RVA: 0x0023CA86 File Offset: 0x0023AC86
	[JsonProperty("user_login")]
	public string UserLogin { get; set; } = "";

	// Token: 0x170008DF RID: 2271
	// (get) Token: 0x0600587A RID: 22650 RVA: 0x0023CA8F File Offset: 0x0023AC8F
	// (set) Token: 0x0600587B RID: 22651 RVA: 0x0023CA97 File Offset: 0x0023AC97
	[JsonProperty("user_name")]
	public string UserName { get; set; } = "";

	// Token: 0x170008E0 RID: 2272
	// (get) Token: 0x0600587C RID: 22652 RVA: 0x0023CAA0 File Offset: 0x0023ACA0
	// (set) Token: 0x0600587D RID: 22653 RVA: 0x0023CAA8 File Offset: 0x0023ACA8
	[JsonProperty("broadcaster_user_id")]
	public string BroadcasterUserId { get; set; } = "";

	// Token: 0x170008E1 RID: 2273
	// (get) Token: 0x0600587E RID: 22654 RVA: 0x0023CAB1 File Offset: 0x0023ACB1
	// (set) Token: 0x0600587F RID: 22655 RVA: 0x0023CAB9 File Offset: 0x0023ACB9
	[JsonProperty("broadcaster_user_login")]
	public string BroadcasterUserLogin { get; set; } = "";

	// Token: 0x170008E2 RID: 2274
	// (get) Token: 0x06005880 RID: 22656 RVA: 0x0023CAC2 File Offset: 0x0023ACC2
	// (set) Token: 0x06005881 RID: 22657 RVA: 0x0023CACA File Offset: 0x0023ACCA
	[JsonProperty("broadcaster_user_name")]
	public string BroadcasterUserName { get; set; } = "";

	// Token: 0x170008E3 RID: 2275
	// (get) Token: 0x06005882 RID: 22658 RVA: 0x0023CAD3 File Offset: 0x0023ACD3
	// (set) Token: 0x06005883 RID: 22659 RVA: 0x0023CADB File Offset: 0x0023ACDB
	[JsonProperty("bits")]
	public int Bits { get; set; }
}
