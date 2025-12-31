using System;
using Newtonsoft.Json;

// Token: 0x02000B1B RID: 2843
public class Reward
{
	// Token: 0x170008E8 RID: 2280
	// (get) Token: 0x0600588E RID: 22670 RVA: 0x0023CBB1 File Offset: 0x0023ADB1
	// (set) Token: 0x0600588F RID: 22671 RVA: 0x0023CBB9 File Offset: 0x0023ADB9
	[JsonProperty("id")]
	public string Id { get; set; } = "";

	// Token: 0x170008E9 RID: 2281
	// (get) Token: 0x06005890 RID: 22672 RVA: 0x0023CBC2 File Offset: 0x0023ADC2
	// (set) Token: 0x06005891 RID: 22673 RVA: 0x0023CBCA File Offset: 0x0023ADCA
	[JsonProperty("title")]
	public string Title { get; set; } = "";

	// Token: 0x170008EA RID: 2282
	// (get) Token: 0x06005892 RID: 22674 RVA: 0x0023CBD3 File Offset: 0x0023ADD3
	// (set) Token: 0x06005893 RID: 22675 RVA: 0x0023CBDB File Offset: 0x0023ADDB
	[JsonProperty("cost")]
	public int Cost { get; set; }
}
