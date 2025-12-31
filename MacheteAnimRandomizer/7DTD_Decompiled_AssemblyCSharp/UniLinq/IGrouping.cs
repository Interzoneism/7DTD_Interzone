using System;
using System.Collections;
using System.Collections.Generic;

namespace UniLinq
{
	// Token: 0x020014DA RID: 5338
	public interface IGrouping<TKey, TElement> : IEnumerable<!1>, IEnumerable
	{
		// Token: 0x17001219 RID: 4633
		// (get) Token: 0x0600A596 RID: 42390
		TKey Key { get; }
	}
}
