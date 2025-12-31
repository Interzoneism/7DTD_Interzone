using System;
using System.Collections.Generic;

// Token: 0x02000B42 RID: 2882
public class VideoData
{
	// Token: 0x060059CB RID: 22987 RVA: 0x002422CB File Offset: 0x002404CB
	public VideoData()
	{
		this.subtitles = new List<VideoSubtitle>();
	}

	// Token: 0x040044A7 RID: 17575
	public string name;

	// Token: 0x040044A8 RID: 17576
	public string url;

	// Token: 0x040044A9 RID: 17577
	public float defaultSubtitleDuration;

	// Token: 0x040044AA RID: 17578
	public List<VideoSubtitle> subtitles;
}
