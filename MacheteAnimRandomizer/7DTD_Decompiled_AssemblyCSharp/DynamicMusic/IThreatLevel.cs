using System;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x0200176D RID: 5997
	public interface IThreatLevel
	{
		// Token: 0x17001415 RID: 5141
		// (get) Token: 0x0600B3D9 RID: 46041
		ThreatLevelType Category { get; }

		// Token: 0x17001416 RID: 5142
		// (get) Token: 0x0600B3DA RID: 46042
		// (set) Token: 0x0600B3DB RID: 46043
		float Numeric { get; set; }
	}
}
