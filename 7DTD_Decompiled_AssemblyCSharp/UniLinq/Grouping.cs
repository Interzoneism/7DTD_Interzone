using System;
using System.Collections;
using System.Collections.Generic;

namespace UniLinq
{
	// Token: 0x020014D9 RID: 5337
	[PublicizedFrom(EAccessModifier.Internal)]
	public class Grouping<K, T> : IGrouping<K, T>, IEnumerable<!1>, IEnumerable
	{
		// Token: 0x0600A591 RID: 42385 RVA: 0x00418C4B File Offset: 0x00416E4B
		public Grouping(K key, IEnumerable<T> group)
		{
			this.group = group;
			this.key = key;
		}

		// Token: 0x17001218 RID: 4632
		// (get) Token: 0x0600A592 RID: 42386 RVA: 0x00418C61 File Offset: 0x00416E61
		// (set) Token: 0x0600A593 RID: 42387 RVA: 0x00418C69 File Offset: 0x00416E69
		public K Key
		{
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
			}
		}

		// Token: 0x0600A594 RID: 42388 RVA: 0x00418C72 File Offset: 0x00416E72
		public IEnumerator<T> GetEnumerator()
		{
			return this.group.GetEnumerator();
		}

		// Token: 0x0600A595 RID: 42389 RVA: 0x00418C72 File Offset: 0x00416E72
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetEnumerator()
		{
			return this.group.GetEnumerator();
		}

		// Token: 0x04007FEB RID: 32747
		[PublicizedFrom(EAccessModifier.Private)]
		public K key;

		// Token: 0x04007FEC RID: 32748
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerable<T> group;
	}
}
