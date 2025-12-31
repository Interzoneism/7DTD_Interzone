using System;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x0200176A RID: 5994
	public interface ISectionLayer : IPlayable, ILayerable, IConfigurable, ICleanable
	{
		// Token: 0x0600B3D7 RID: 46039
		void SetParentSection(SectionType _sectionType);
	}
}
