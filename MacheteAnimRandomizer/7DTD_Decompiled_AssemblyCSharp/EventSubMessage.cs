using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Token: 0x02000B20 RID: 2848
[Serializable]
public class EventSubMessage
{
	// Token: 0x170008F1 RID: 2289
	// (get) Token: 0x060058B2 RID: 22706 RVA: 0x0023D503 File Offset: 0x0023B703
	// (set) Token: 0x060058B3 RID: 22707 RVA: 0x0023D50B File Offset: 0x0023B70B
	[JsonProperty("metadata")]
	public EventSubMetadata Metadata { get; set; } = new EventSubMetadata();

	// Token: 0x170008F2 RID: 2290
	// (get) Token: 0x060058B4 RID: 22708 RVA: 0x0023D514 File Offset: 0x0023B714
	// (set) Token: 0x060058B5 RID: 22709 RVA: 0x0023D51C File Offset: 0x0023B71C
	[JsonProperty("payload")]
	public JObject Payload { get; set; }
}
