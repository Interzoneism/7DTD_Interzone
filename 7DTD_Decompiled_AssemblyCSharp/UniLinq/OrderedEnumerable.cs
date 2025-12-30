using System;
using System.Collections;
using System.Collections.Generic;

namespace UniLinq
{
	// Token: 0x020014E0 RID: 5344
	[PublicizedFrom(EAccessModifier.Internal)]
	public abstract class OrderedEnumerable<TElement> : IOrderedEnumerable<TElement>, IEnumerable<!0>, IEnumerable
	{
		// Token: 0x0600A5B2 RID: 42418 RVA: 0x0041910E File Offset: 0x0041730E
		[PublicizedFrom(EAccessModifier.Protected)]
		public OrderedEnumerable(IEnumerable<TElement> source)
		{
			this.source = source;
		}

		// Token: 0x0600A5B3 RID: 42419 RVA: 0x0041911D File Offset: 0x0041731D
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x0600A5B4 RID: 42420 RVA: 0x00419125 File Offset: 0x00417325
		public virtual IEnumerator<TElement> GetEnumerator()
		{
			return this.Sort(this.source).GetEnumerator();
		}

		// Token: 0x0600A5B5 RID: 42421
		public abstract SortContext<TElement> CreateContext(SortContext<TElement> current);

		// Token: 0x0600A5B6 RID: 42422
		[PublicizedFrom(EAccessModifier.Protected)]
		public abstract IEnumerable<TElement> Sort(IEnumerable<TElement> source);

		// Token: 0x0600A5B7 RID: 42423 RVA: 0x00419138 File Offset: 0x00417338
		public IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> selector, IComparer<TKey> comparer, bool descending)
		{
			return new OrderedSequence<TElement, TKey>(this, this.source, selector, comparer, descending ? SortDirection.Descending : SortDirection.Ascending);
		}

		// Token: 0x04007FFA RID: 32762
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerable<TElement> source;
	}
}
