using System;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x02001769 RID: 5993
	public interface ISection : IPlayable, IFadeable, ICleanable
	{
		// Token: 0x17001413 RID: 5139
		// (get) Token: 0x0600B3D4 RID: 46036
		bool IsInitialized { get; }

		// Token: 0x17001414 RID: 5140
		// (get) Token: 0x0600B3D5 RID: 46037
		// (set) Token: 0x0600B3D6 RID: 46038
		SectionType Sect { get; set; }
	}
}
