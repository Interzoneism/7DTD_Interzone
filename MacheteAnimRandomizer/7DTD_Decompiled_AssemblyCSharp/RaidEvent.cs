using System;
using Newtonsoft.Json;

// Token: 0x02000B23 RID: 2851
public class RaidEvent
{
	// Token: 0x170008F7 RID: 2295
	// (get) Token: 0x060058C1 RID: 22721 RVA: 0x0023D5A5 File Offset: 0x0023B7A5
	// (set) Token: 0x060058C2 RID: 22722 RVA: 0x0023D5AD File Offset: 0x0023B7AD
	[JsonProperty("from_broadcaster_user_id")]
	public string RaiderID { get; set; }

	// Token: 0x170008F8 RID: 2296
	// (get) Token: 0x060058C3 RID: 22723 RVA: 0x0023D5B6 File Offset: 0x0023B7B6
	// (set) Token: 0x060058C4 RID: 22724 RVA: 0x0023D5BE File Offset: 0x0023B7BE
	[JsonProperty("from_broadcaster_user_login")]
	public string RaiderUserName { get; set; }

	// Token: 0x170008F9 RID: 2297
	// (get) Token: 0x060058C5 RID: 22725 RVA: 0x0023D5C7 File Offset: 0x0023B7C7
	// (set) Token: 0x060058C6 RID: 22726 RVA: 0x0023D5CF File Offset: 0x0023B7CF
	[JsonProperty("viewers")]
	public int viewerCount { get; set; }
}
