using System;
using System.Collections.Generic;

namespace UniLinq
{
	// Token: 0x020014E2 RID: 5346
	[PublicizedFrom(EAccessModifier.Internal)]
	public class QuickSort<TElement>
	{
		// Token: 0x0600A5BD RID: 42429 RVA: 0x004191E0 File Offset: 0x004173E0
		[PublicizedFrom(EAccessModifier.Private)]
		public QuickSort(IEnumerable<TElement> source, SortContext<TElement> context)
		{
			List<TElement> list = new List<TElement>();
			foreach (TElement item in source)
			{
				list.Add(item);
			}
			this.elements = list.ToArray();
			this.indexes = QuickSort<TElement>.CreateIndexes(this.elements.Length);
			this.context = context;
		}

		// Token: 0x0600A5BE RID: 42430 RVA: 0x0041925C File Offset: 0x0041745C
		[PublicizedFrom(EAccessModifier.Private)]
		public static int[] CreateIndexes(int length)
		{
			int[] array = new int[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = i;
			}
			return array;
		}

		// Token: 0x0600A5BF RID: 42431 RVA: 0x00419281 File Offset: 0x00417481
		[PublicizedFrom(EAccessModifier.Private)]
		public void PerformSort()
		{
			if (this.elements.Length <= 1)
			{
				return;
			}
			this.context.Initialize(this.elements);
			Array.Sort<int>(this.indexes, this.context);
		}

		// Token: 0x0600A5C0 RID: 42432 RVA: 0x004192B1 File Offset: 0x004174B1
		public static IEnumerable<TElement> Sort(IEnumerable<TElement> source, SortContext<TElement> context)
		{
			QuickSort<TElement> sorter = new QuickSort<TElement>(source, context);
			sorter.PerformSort();
			int num;
			for (int i = 0; i < sorter.elements.Length; i = num + 1)
			{
				yield return sorter.elements[sorter.indexes[i]];
				num = i;
			}
			yield break;
		}

		// Token: 0x04007FFF RID: 32767
		[PublicizedFrom(EAccessModifier.Private)]
		public TElement[] elements;

		// Token: 0x04008000 RID: 32768
		[PublicizedFrom(EAccessModifier.Private)]
		public int[] indexes;

		// Token: 0x04008001 RID: 32769
		[PublicizedFrom(EAccessModifier.Private)]
		public SortContext<TElement> context;
	}
}
