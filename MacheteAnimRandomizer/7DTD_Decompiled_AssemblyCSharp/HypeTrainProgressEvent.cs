using System;
using Newtonsoft.Json;

// Token: 0x02000B22 RID: 2850
public class HypeTrainProgressEvent
{
	// Token: 0x170008F6 RID: 2294
	// (get) Token: 0x060058BE RID: 22718 RVA: 0x0023D594 File Offset: 0x0023B794
	// (set) Token: 0x060058BF RID: 22719 RVA: 0x0023D59C File Offset: 0x0023B79C
	[JsonProperty("level")]
	public int Level { get; set; }
}
