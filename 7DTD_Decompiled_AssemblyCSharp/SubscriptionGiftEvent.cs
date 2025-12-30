using System;
using Newtonsoft.Json;

// Token: 0x02000B26 RID: 2854
public class SubscriptionGiftEvent : SubscriptionEventBase
{
	// Token: 0x170008FF RID: 2303
	// (get) Token: 0x060058D4 RID: 22740 RVA: 0x0023D669 File Offset: 0x0023B869
	// (set) Token: 0x060058D5 RID: 22741 RVA: 0x0023D671 File Offset: 0x0023B871
	[JsonProperty("is_anonymous")]
	public bool IsAnonymous { get; set; }
}
