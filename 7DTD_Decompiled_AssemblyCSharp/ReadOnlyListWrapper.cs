using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x020011FD RID: 4605
public class ReadOnlyListWrapper<TIn, TOut> : IReadOnlyList<TOut>, IEnumerable<TOut>, IEnumerable, IReadOnlyCollection<TOut> where TIn : TOut
{
	// Token: 0x06008FCF RID: 36815 RVA: 0x0039608B File Offset: 0x0039428B
	public ReadOnlyListWrapper(IList<TIn> list)
	{
		this.m_list = list;
	}

	// Token: 0x06008FD0 RID: 36816 RVA: 0x0039609A File Offset: 0x0039429A
	public IEnumerator<TOut> GetEnumerator()
	{
		return (IEnumerator<TOut>)this.m_list.GetEnumerator();
	}

	// Token: 0x06008FD1 RID: 36817 RVA: 0x003960AC File Offset: 0x003942AC
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator GetEnumerator()
	{
		return this.m_list.GetEnumerator();
	}

	// Token: 0x17000EE3 RID: 3811
	// (get) Token: 0x06008FD2 RID: 36818 RVA: 0x003960B9 File Offset: 0x003942B9
	public int Count
	{
		get
		{
			return this.m_list.Count;
		}
	}

	// Token: 0x17000EE4 RID: 3812
	public TOut this[int index]
	{
		get
		{
			return (TOut)((object)this.m_list[index]);
		}
	}

	// Token: 0x04006ED8 RID: 28376
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly IList<TIn> m_list;
}
