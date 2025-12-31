using System;
using System.Collections;
using System.Collections.Generic;

namespace UniLinq
{
	// Token: 0x020014DC RID: 5340
	public interface IOrderedEnumerable<TElement> : IEnumerable<!0>, IEnumerable
	{
		// Token: 0x0600A59A RID: 42394
		IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending);
	}
}
