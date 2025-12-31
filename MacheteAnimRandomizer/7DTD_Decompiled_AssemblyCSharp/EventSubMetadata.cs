using System;
using Newtonsoft.Json;

// Token: 0x02000B21 RID: 2849
[Serializable]
public class EventSubMetadata
{
	// Token: 0x170008F3 RID: 2291
	// (get) Token: 0x060058B7 RID: 22711 RVA: 0x0023D538 File Offset: 0x0023B738
	// (set) Token: 0x060058B8 RID: 22712 RVA: 0x0023D540 File Offset: 0x0023B740
	[JsonProperty("message_id")]
	public string MessageId { get; set; } = string.Empty;

	// Token: 0x170008F4 RID: 2292
	// (get) Token: 0x060058B9 RID: 22713 RVA: 0x0023D549 File Offset: 0x0023B749
	// (set) Token: 0x060058BA RID: 22714 RVA: 0x0023D551 File Offset: 0x0023B751
	[JsonProperty("message_type")]
	public string MessageType { get; set; } = string.Empty;

	// Token: 0x170008F5 RID: 2293
	// (get) Token: 0x060058BB RID: 22715 RVA: 0x0023D55A File Offset: 0x0023B75A
	// (set) Token: 0x060058BC RID: 22716 RVA: 0x0023D562 File Offset: 0x0023B762
	[JsonProperty("message_timestamp")]
	public string MessageTimestamp { get; set; } = string.Empty;
}
