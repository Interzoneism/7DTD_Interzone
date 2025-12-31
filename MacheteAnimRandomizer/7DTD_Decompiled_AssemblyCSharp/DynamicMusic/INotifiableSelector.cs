using System;

namespace DynamicMusic
{
	// Token: 0x02001763 RID: 5987
	[PublicizedFrom(EAccessModifier.Internal)]
	public interface INotifiableSelector<T1, T2> : INotifiable<T1>, ISelector<T2>
	{
	}
}
