using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x0200116A RID: 4458
public sealed class ListDebugWrapper<T> : CollectionDebugWrapper<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	// Token: 0x06008B70 RID: 35696 RVA: 0x0038467B File Offset: 0x0038287B
	public ListDebugWrapper() : this(new List<T>())
	{
	}

	// Token: 0x06008B71 RID: 35697 RVA: 0x00384688 File Offset: 0x00382888
	public ListDebugWrapper(IList<T> list) : this(null, list)
	{
	}

	// Token: 0x06008B72 RID: 35698 RVA: 0x00384692 File Offset: 0x00382892
	public ListDebugWrapper(DebugWrapper parent, IList<T> list) : base(parent, list)
	{
		this.m_list = list;
	}

	// Token: 0x06008B73 RID: 35699 RVA: 0x003846A4 File Offset: 0x003828A4
	public int IndexOf(T item)
	{
		int result;
		using (base.DebugReadScope())
		{
			result = this.m_list.IndexOf(item);
		}
		return result;
	}

	// Token: 0x06008B74 RID: 35700 RVA: 0x003846E4 File Offset: 0x003828E4
	public void Insert(int index, T item)
	{
		using (base.DebugReadWriteScope())
		{
			this.m_list.Insert(index, item);
		}
	}

	// Token: 0x06008B75 RID: 35701 RVA: 0x00384724 File Offset: 0x00382924
	public void RemoveAt(int index)
	{
		using (base.DebugReadWriteScope())
		{
			this.m_list.RemoveAt(index);
		}
	}

	// Token: 0x17000E89 RID: 3721
	public T this[int index]
	{
		get
		{
			T result;
			using (base.DebugReadScope())
			{
				result = this.m_list[index];
			}
			return result;
		}
		set
		{
			using (base.DebugReadWriteScope())
			{
				this.m_list[index] = value;
			}
		}
	}

	// Token: 0x04006CFB RID: 27899
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly IList<T> m_list;
}
