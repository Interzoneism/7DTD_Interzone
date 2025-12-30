using System;
using System.Collections.Generic;

namespace UniLinq
{
	// Token: 0x020014E6 RID: 5350
	[PublicizedFrom(EAccessModifier.Internal)]
	public class SortSequenceContext<TElement, TKey> : SortContext<TElement>
	{
		// Token: 0x0600A5CC RID: 42444 RVA: 0x00419415 File Offset: 0x00417615
		public SortSequenceContext(Func<TElement, TKey> selector, IComparer<TKey> comparer, SortDirection direction, SortContext<TElement> child_context) : base(direction, child_context)
		{
			this.selector = selector;
			this.comparer = comparer;
		}

		// Token: 0x0600A5CD RID: 42445 RVA: 0x00419430 File Offset: 0x00417630
		public override void Initialize(TElement[] elements)
		{
			if (this.child_context != null)
			{
				this.child_context.Initialize(elements);
			}
			this.keys = new TKey[elements.Length];
			for (int i = 0; i < this.keys.Length; i++)
			{
				this.keys[i] = this.selector(elements[i]);
			}
		}

		// Token: 0x0600A5CE RID: 42446 RVA: 0x00419490 File Offset: 0x00417690
		public override int Compare(int first_index, int second_index)
		{
			int num = this.comparer.Compare(this.keys[first_index], this.keys[second_index]);
			if (num == 0)
			{
				if (this.child_context != null)
				{
					return this.child_context.Compare(first_index, second_index);
				}
				num = ((this.direction == SortDirection.Descending) ? (second_index - first_index) : (first_index - second_index));
			}
			if (this.direction != SortDirection.Descending)
			{
				return num;
			}
			return -num;
		}

		// Token: 0x04008010 RID: 32784
		[PublicizedFrom(EAccessModifier.Private)]
		public Func<TElement, TKey> selector;

		// Token: 0x04008011 RID: 32785
		[PublicizedFrom(EAccessModifier.Private)]
		public IComparer<TKey> comparer;

		// Token: 0x04008012 RID: 32786
		[PublicizedFrom(EAccessModifier.Private)]
		public TKey[] keys;
	}
}
