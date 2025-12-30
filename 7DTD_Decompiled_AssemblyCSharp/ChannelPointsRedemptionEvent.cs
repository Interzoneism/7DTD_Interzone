using System;
using Newtonsoft.Json;

// Token: 0x02000B1A RID: 2842
public class ChannelPointsRedemptionEvent
{
	// Token: 0x170008E4 RID: 2276
	// (get) Token: 0x06005885 RID: 22661 RVA: 0x0023CB39 File Offset: 0x0023AD39
	// (set) Token: 0x06005886 RID: 22662 RVA: 0x0023CB41 File Offset: 0x0023AD41
	[JsonProperty("user_id")]
	public string UserId { get; set; } = "";

	// Token: 0x170008E5 RID: 2277
	// (get) Token: 0x06005887 RID: 22663 RVA: 0x0023CB4A File Offset: 0x0023AD4A
	// (set) Token: 0x06005888 RID: 22664 RVA: 0x0023CB52 File Offset: 0x0023AD52
	[JsonProperty("user_login")]
	public string UserLogin { get; set; } = "";

	// Token: 0x170008E6 RID: 2278
	// (get) Token: 0x06005889 RID: 22665 RVA: 0x0023CB5B File Offset: 0x0023AD5B
	// (set) Token: 0x0600588A RID: 22666 RVA: 0x0023CB63 File Offset: 0x0023AD63
	[JsonProperty("user_name")]
	public string UserName { get; set; } = "";

	// Token: 0x170008E7 RID: 2279
	// (get) Token: 0x0600588B RID: 22667 RVA: 0x0023CB6C File Offset: 0x0023AD6C
	// (set) Token: 0x0600588C RID: 22668 RVA: 0x0023CB74 File Offset: 0x0023AD74
	[JsonProperty("reward")]
	public Reward Reward { get; set; } = new Reward();
}
