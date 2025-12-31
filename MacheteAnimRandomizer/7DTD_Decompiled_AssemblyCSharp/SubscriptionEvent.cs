using System;
using Newtonsoft.Json;

// Token: 0x02000B24 RID: 2852
public class SubscriptionEvent : SubscriptionEventBase
{
	// Token: 0x170008FA RID: 2298
	// (get) Token: 0x060058C8 RID: 22728 RVA: 0x0023D5D8 File Offset: 0x0023B7D8
	// (set) Token: 0x060058C9 RID: 22729 RVA: 0x0023D5E0 File Offset: 0x0023B7E0
	[JsonProperty("is_gift")]
	public bool IsGift { get; set; }
}
