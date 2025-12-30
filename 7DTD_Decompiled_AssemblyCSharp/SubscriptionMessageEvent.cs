using System;
using Newtonsoft.Json;

// Token: 0x02000B27 RID: 2855
public class SubscriptionMessageEvent : SubscriptionEventBase
{
	// Token: 0x17000900 RID: 2304
	// (get) Token: 0x060058D7 RID: 22743 RVA: 0x0023D67A File Offset: 0x0023B87A
	// (set) Token: 0x060058D8 RID: 22744 RVA: 0x0023D682 File Offset: 0x0023B882
	[JsonProperty("cumulative_months")]
	public int CumulativeMonths { get; set; }

	// Token: 0x17000901 RID: 2305
	// (get) Token: 0x060058D9 RID: 22745 RVA: 0x0023D68B File Offset: 0x0023B88B
	// (set) Token: 0x060058DA RID: 22746 RVA: 0x0023D693 File Offset: 0x0023B893
	[JsonProperty("streak_months")]
	public string StreakMonths { get; set; }

	// Token: 0x17000902 RID: 2306
	// (get) Token: 0x060058DB RID: 22747 RVA: 0x0023D69C File Offset: 0x0023B89C
	// (set) Token: 0x060058DC RID: 22748 RVA: 0x0023D6A4 File Offset: 0x0023B8A4
	[JsonProperty("duration_months")]
	public string DurationMonths { get; set; } = "";
}
