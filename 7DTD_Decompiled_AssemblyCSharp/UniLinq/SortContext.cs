using System;
using System.Collections.Generic;

namespace UniLinq
{
	// Token: 0x020014E4 RID: 5348
	[PublicizedFrom(EAccessModifier.Internal)]
	public abstract class SortContext<TElement> : IComparer<int>
	{
		// Token: 0x0600A5C9 RID: 42441 RVA: 0x004193FF File Offset: 0x004175FF
		[PublicizedFrom(EAccessModifier.Protected)]
		public SortContext(SortDirection direction, SortContext<TElement> child_context)
		{
			this.direction = direction;
			this.child_context = child_context;
		}

		// Token: 0x0600A5CA RID: 42442
		public abstract void Initialize(TElement[] elements);

		// Token: 0x0600A5CB RID: 42443
		public abstract int Compare(int first_index, int second_index);

		// Token: 0x0400800B RID: 32779
		[PublicizedFrom(EAccessModifier.Protected)]
		public SortDirection direction;

		// Token: 0x0400800C RID: 32780
		[PublicizedFrom(EAccessModifier.Protected)]
		public SortContext<TElement> child_context;
	}
}
