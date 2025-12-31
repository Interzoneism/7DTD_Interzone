using System;
using System.Collections.Generic;

namespace DynamicMusic
{
	// Token: 0x02001759 RID: 5977
	public interface IFilter<T>
	{
		// Token: 0x0600B3BA RID: 46010
		List<T> Filter(List<T> _list);
	}
}
