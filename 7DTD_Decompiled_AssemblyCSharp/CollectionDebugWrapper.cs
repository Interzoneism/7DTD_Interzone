using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x0200115F RID: 4447
public class CollectionDebugWrapper<T> : EnumerableDebugWrapper<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	// Token: 0x06008B33 RID: 35635 RVA: 0x00383E1A File Offset: 0x0038201A
	public CollectionDebugWrapper(ICollection<T> collection) : this(null, collection)
	{
	}

	// Token: 0x06008B34 RID: 35636 RVA: 0x00383E24 File Offset: 0x00382024
	public CollectionDebugWrapper(DebugWrapper parent, ICollection<T> collection) : base(parent, collection)
	{
		this.m_collection = collection;
	}

	// Token: 0x06008B35 RID: 35637 RVA: 0x00383E38 File Offset: 0x00382038
	public void Add(T item)
	{
		using (base.DebugReadWriteScope())
		{
			this.m_collection.Add(item);
		}
	}

	// Token: 0x06008B36 RID: 35638 RVA: 0x00383E74 File Offset: 0x00382074
	public void Clear()
	{
		using (base.DebugReadWriteScope())
		{
			this.m_collection.Clear();
		}
	}

	// Token: 0x06008B37 RID: 35639 RVA: 0x00383EB0 File Offset: 0x003820B0
	public bool Contains(T item)
	{
		bool result;
		using (base.DebugReadScope())
		{
			result = this.m_collection.Contains(item);
		}
		return result;
	}

	// Token: 0x06008B38 RID: 35640 RVA: 0x00383EF0 File Offset: 0x003820F0
	public void CopyTo(T[] array, int arrayIndex)
	{
		using (base.DebugReadScope())
		{
			this.m_collection.CopyTo(array, arrayIndex);
		}
	}

	// Token: 0x06008B39 RID: 35641 RVA: 0x00383F30 File Offset: 0x00382130
	public bool Remove(T item)
	{
		bool result;
		using (base.DebugReadWriteScope())
		{
			result = this.m_collection.Remove(item);
		}
		return result;
	}

	// Token: 0x17000E7E RID: 3710
	// (get) Token: 0x06008B3A RID: 35642 RVA: 0x00383F70 File Offset: 0x00382170
	public int Count
	{
		get
		{
			return this.m_collection.Count;
		}
	}

	// Token: 0x17000E7F RID: 3711
	// (get) Token: 0x06008B3B RID: 35643 RVA: 0x00383F7D File Offset: 0x0038217D
	public bool IsReadOnly
	{
		get
		{
			return this.m_collection.IsReadOnly;
		}
	}

	// Token: 0x04006CE6 RID: 27878
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly ICollection<T> m_collection;
}
