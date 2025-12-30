using System;
using System.Collections;
using System.Collections.Generic;

namespace UniLinq
{
	// Token: 0x020014DB RID: 5339
	public interface ILookup<TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>, IEnumerable
	{
		// Token: 0x1700121A RID: 4634
		// (get) Token: 0x0600A597 RID: 42391
		int Count { get; }

		// Token: 0x1700121B RID: 4635
		IEnumerable<TElement> this[TKey key]
		{
			get;
		}

		// Token: 0x0600A599 RID: 42393
		bool Contains(TKey key);
	}
}
