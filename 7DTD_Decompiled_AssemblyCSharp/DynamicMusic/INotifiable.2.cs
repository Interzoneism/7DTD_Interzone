using System;

namespace DynamicMusic
{
	// Token: 0x02001760 RID: 5984
	public interface INotifiable<T>
	{
		// Token: 0x0600B3C3 RID: 46019
		void Notify(T _state);
	}
}
