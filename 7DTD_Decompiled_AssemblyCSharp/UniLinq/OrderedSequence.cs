using System;
using System.Collections.Generic;

namespace UniLinq
{
	// Token: 0x020014E1 RID: 5345
	[PublicizedFrom(EAccessModifier.Internal)]
	public class OrderedSequence<TElement, TKey> : OrderedEnumerable<TElement>
	{
		// Token: 0x0600A5B8 RID: 42424 RVA: 0x0041914F File Offset: 0x0041734F
		[PublicizedFrom(EAccessModifier.Internal)]
		public OrderedSequence(IEnumerable<TElement> source, Func<TElement, TKey> key_selector, IComparer<TKey> comparer, SortDirection direction) : base(source)
		{
			this.selector = key_selector;
			this.comparer = (comparer ?? Comparer<TKey>.Default);
			this.direction = direction;
		}

		// Token: 0x0600A5B9 RID: 42425 RVA: 0x00419177 File Offset: 0x00417377
		[PublicizedFrom(EAccessModifier.Internal)]
		public OrderedSequence(OrderedEnumerable<TElement> parent, IEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, SortDirection direction) : this(source, keySelector, comparer, direction)
		{
			this.parent = parent;
		}

		// Token: 0x0600A5BA RID: 42426 RVA: 0x0041918C File Offset: 0x0041738C
		public override IEnumerator<TElement> GetEnumerator()
		{
			return base.GetEnumerator();
		}

		// Token: 0x0600A5BB RID: 42427 RVA: 0x00419194 File Offset: 0x00417394
		public override SortContext<TElement> CreateContext(SortContext<TElement> current)
		{
			SortContext<TElement> sortContext = new SortSequenceContext<TElement, TKey>(this.selector, this.comparer, this.direction, current);
			if (this.parent != null)
			{
				return this.parent.CreateContext(sortContext);
			}
			return sortContext;
		}

		// Token: 0x0600A5BC RID: 42428 RVA: 0x004191D0 File Offset: 0x004173D0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override IEnumerable<TElement> Sort(IEnumerable<TElement> source)
		{
			return QuickSort<TElement>.Sort(source, this.CreateContext(null));
		}

		// Token: 0x04007FFB RID: 32763
		[PublicizedFrom(EAccessModifier.Private)]
		public OrderedEnumerable<TElement> parent;

		// Token: 0x04007FFC RID: 32764
		[PublicizedFrom(EAccessModifier.Private)]
		public Func<TElement, TKey> selector;

		// Token: 0x04007FFD RID: 32765
		[PublicizedFrom(EAccessModifier.Private)]
		public IComparer<TKey> comparer;

		// Token: 0x04007FFE RID: 32766
		[PublicizedFrom(EAccessModifier.Private)]
		public SortDirection direction;
	}
}
