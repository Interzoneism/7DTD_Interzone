using System;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x0200176B RID: 5995
	public interface ISectionSelector : INotifiable<MusicActionType>, ISelector<SectionType>
	{
	}
}
