using System;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x0200175E RID: 5982
	public interface IMultiNotifiableFilter : INotifiable, INotifiableFilter<MusicActionType, SectionType>, INotifiable<MusicActionType>, IFilter<SectionType>
	{
	}
}
