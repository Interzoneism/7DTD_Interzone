using System;
using Newtonsoft.Json;

// Token: 0x02000B25 RID: 2853
public class SubscriptionEventBase
{
	// Token: 0x170008FB RID: 2299
	// (get) Token: 0x060058CB RID: 22731 RVA: 0x0023D5F1 File Offset: 0x0023B7F1
	// (set) Token: 0x060058CC RID: 22732 RVA: 0x0023D5F9 File Offset: 0x0023B7F9
	[JsonProperty("user_id")]
	public string UserId { get; set; } = "";

	// Token: 0x170008FC RID: 2300
	// (get) Token: 0x060058CD RID: 22733 RVA: 0x0023D602 File Offset: 0x0023B802
	// (set) Token: 0x060058CE RID: 22734 RVA: 0x0023D60A File Offset: 0x0023B80A
	[JsonProperty("user_login")]
	public string UserLogin { get; set; } = "";

	// Token: 0x170008FD RID: 2301
	// (get) Token: 0x060058CF RID: 22735 RVA: 0x0023D613 File Offset: 0x0023B813
	// (set) Token: 0x060058D0 RID: 22736 RVA: 0x0023D61B File Offset: 0x0023B81B
	[JsonProperty("user_name")]
	public string UserName { get; set; } = "";

	// Token: 0x170008FE RID: 2302
	// (get) Token: 0x060058D1 RID: 22737 RVA: 0x0023D624 File Offset: 0x0023B824
	// (set) Token: 0x060058D2 RID: 22738 RVA: 0x0023D62C File Offset: 0x0023B82C
	[JsonProperty("tier")]
	public string Tier { get; set; } = "";
}
