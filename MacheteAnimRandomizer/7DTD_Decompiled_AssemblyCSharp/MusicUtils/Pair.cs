using System;

namespace MusicUtils
{
	// Token: 0x020016FD RID: 5885
	public struct Pair<T1, T2> where T1 : IComparable<T1> where T2 : IComparable<T2>
	{
		// Token: 0x0600B1EB RID: 45547 RVA: 0x00454E8A File Offset: 0x0045308A
		public Pair(T1 _item1, T2 _item2)
		{
			this.item1 = _item1;
			this.item2 = _item2;
		}

		// Token: 0x04008B5D RID: 35677
		public T1 item1;

		// Token: 0x04008B5E RID: 35678
		public T2 item2;
	}
}
