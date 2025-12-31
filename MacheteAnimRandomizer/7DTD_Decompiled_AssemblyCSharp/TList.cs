using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

// Token: 0x0200123C RID: 4668
public class TList<T> : ICollection<T>, IEnumerable<!0>, IEnumerable
{
	// Token: 0x17000F0D RID: 3853
	// (get) Token: 0x060091B0 RID: 37296 RVA: 0x003A1BFC File Offset: 0x0039FDFC
	// (set) Token: 0x060091B1 RID: 37297 RVA: 0x003A1C0C File Offset: 0x0039FE0C
	public bool Disposed
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return Thread.VolatileRead(ref this.m_Disposed) == 1;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			Thread.VolatileWrite(ref this.m_Disposed, value ? 1 : 0);
		}
	}

	// Token: 0x060091B2 RID: 37298 RVA: 0x003A1C20 File Offset: 0x0039FE20
	public TList()
	{
		this.m_TList = new List<T>();
	}

	// Token: 0x060091B3 RID: 37299 RVA: 0x003A1C3E File Offset: 0x0039FE3E
	public TList(int capacity)
	{
		this.m_TList = new List<T>(capacity);
	}

	// Token: 0x060091B4 RID: 37300 RVA: 0x003A1C5D File Offset: 0x0039FE5D
	public TList(IEnumerable<T> collection)
	{
		this.m_TList = new List<T>(collection);
	}

	// Token: 0x060091B5 RID: 37301 RVA: 0x003A1C7C File Offset: 0x0039FE7C
	public IEnumerator GetEnumerator()
	{
		this.LockList.EnterReadLock();
		List<T> list;
		try
		{
			list = new List<T>(this.m_TList);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		foreach (T t in list)
		{
			yield return t;
		}
		List<T>.Enumerator enumerator = default(List<T>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x060091B6 RID: 37302 RVA: 0x003A1C8B File Offset: 0x0039FE8B
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator<T> GetEnumerator()
	{
		this.LockList.EnterReadLock();
		List<T> list;
		try
		{
			list = new List<T>(this.m_TList);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		foreach (T t in list)
		{
			yield return t;
		}
		List<T>.Enumerator enumerator = default(List<T>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x060091B7 RID: 37303 RVA: 0x003A1C9A File Offset: 0x0039FE9A
	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	// Token: 0x060091B8 RID: 37304 RVA: 0x003A1CA9 File Offset: 0x0039FEA9
	[PublicizedFrom(EAccessModifier.Private)]
	public void Dispose(bool disposing)
	{
		if (this.Disposed)
		{
			return;
		}
		this.Disposed = true;
	}

	// Token: 0x060091B9 RID: 37305 RVA: 0x003A1CC0 File Offset: 0x0039FEC0
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~TList()
	{
		this.Dispose(false);
	}

	// Token: 0x060091BA RID: 37306 RVA: 0x003A1CF0 File Offset: 0x0039FEF0
	public void Add(T item)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.Add(item);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	// Token: 0x060091BB RID: 37307 RVA: 0x003A1D34 File Offset: 0x0039FF34
	public void AddRange(IEnumerable<T> collection)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.AddRange(collection);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	// Token: 0x060091BC RID: 37308 RVA: 0x003A1D78 File Offset: 0x0039FF78
	public bool AddIfNotExist(T item)
	{
		this.LockList.EnterWriteLock();
		bool result;
		try
		{
			if (this.m_TList.Contains(item))
			{
				result = false;
			}
			else
			{
				this.m_TList.Add(item);
				result = true;
			}
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
		return result;
	}

	// Token: 0x060091BD RID: 37309 RVA: 0x003A1DD0 File Offset: 0x0039FFD0
	public ReadOnlyCollection<T> AsReadOnly()
	{
		this.LockList.EnterReadLock();
		ReadOnlyCollection<T> result;
		try
		{
			result = this.m_TList.AsReadOnly();
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091BE RID: 37310 RVA: 0x003A1E14 File Offset: 0x003A0014
	public int BinarySearch(T item)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.BinarySearch(item);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091BF RID: 37311 RVA: 0x003A1E58 File Offset: 0x003A0058
	public int BinarySearch(T item, IComparer<T> comparer)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.BinarySearch(item, comparer);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091C0 RID: 37312 RVA: 0x003A1EA0 File Offset: 0x003A00A0
	public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.BinarySearch(index, count, item, comparer);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x17000F0E RID: 3854
	// (get) Token: 0x060091C1 RID: 37313 RVA: 0x003A1EE8 File Offset: 0x003A00E8
	// (set) Token: 0x060091C2 RID: 37314 RVA: 0x003A1F2C File Offset: 0x003A012C
	public int Capacity
	{
		get
		{
			this.LockList.EnterReadLock();
			int capacity;
			try
			{
				capacity = this.m_TList.Capacity;
			}
			finally
			{
				this.LockList.ExitReadLock();
			}
			return capacity;
		}
		set
		{
			this.LockList.EnterWriteLock();
			try
			{
				this.m_TList.Capacity = value;
			}
			finally
			{
				this.LockList.ExitWriteLock();
			}
		}
	}

	// Token: 0x060091C3 RID: 37315 RVA: 0x003A1F70 File Offset: 0x003A0170
	public void Clear()
	{
		this.LockList.EnterReadLock();
		try
		{
			this.m_TList.Clear();
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
	}

	// Token: 0x060091C4 RID: 37316 RVA: 0x003A1FB4 File Offset: 0x003A01B4
	public bool Contains(T item)
	{
		this.LockList.EnterReadLock();
		bool result;
		try
		{
			result = this.m_TList.Contains(item);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091C5 RID: 37317 RVA: 0x003A1FF8 File Offset: 0x003A01F8
	public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
	{
		this.LockList.EnterReadLock();
		List<TOutput> result;
		try
		{
			result = this.m_TList.ConvertAll<TOutput>(converter);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091C6 RID: 37318 RVA: 0x003A203C File Offset: 0x003A023C
	public void CopyTo(T[] array, int arrayIndex)
	{
		this.LockList.EnterReadLock();
		try
		{
			this.m_TList.CopyTo(array, arrayIndex);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
	}

	// Token: 0x17000F0F RID: 3855
	// (get) Token: 0x060091C7 RID: 37319 RVA: 0x003A2080 File Offset: 0x003A0280
	public int Count
	{
		get
		{
			this.LockList.EnterReadLock();
			int count;
			try
			{
				count = this.m_TList.Count;
			}
			finally
			{
				this.LockList.ExitReadLock();
			}
			return count;
		}
	}

	// Token: 0x060091C8 RID: 37320 RVA: 0x003A20C4 File Offset: 0x003A02C4
	public bool Exists(Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		bool result;
		try
		{
			result = this.m_TList.Exists(match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091C9 RID: 37321 RVA: 0x003A2108 File Offset: 0x003A0308
	public T Find(Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		T result;
		try
		{
			result = this.m_TList.Find(match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091CA RID: 37322 RVA: 0x003A214C File Offset: 0x003A034C
	public List<T> FindAll(Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		List<T> result;
		try
		{
			result = this.m_TList.FindAll(match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091CB RID: 37323 RVA: 0x003A2190 File Offset: 0x003A0390
	public int FindIndex(Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.FindIndex(match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091CC RID: 37324 RVA: 0x003A21D4 File Offset: 0x003A03D4
	public int FindIndex(int startIndex, Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.FindIndex(startIndex, match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091CD RID: 37325 RVA: 0x003A221C File Offset: 0x003A041C
	public int FindIndex(int startIndex, int count, Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.FindIndex(startIndex, count, match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091CE RID: 37326 RVA: 0x003A2264 File Offset: 0x003A0464
	public T FindLast(Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		T result;
		try
		{
			result = this.m_TList.FindLast(match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091CF RID: 37327 RVA: 0x003A22A8 File Offset: 0x003A04A8
	public int FindLastIndex(Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.FindLastIndex(match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091D0 RID: 37328 RVA: 0x003A22EC File Offset: 0x003A04EC
	public int FindLastIndex(int startIndex, Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.FindLastIndex(startIndex, match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091D1 RID: 37329 RVA: 0x003A2334 File Offset: 0x003A0534
	public int FindLastIndex(int startIndex, int count, Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.FindLastIndex(startIndex, count, match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091D2 RID: 37330 RVA: 0x003A237C File Offset: 0x003A057C
	public void ForEach(Action<T> action)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.ForEach(action);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	// Token: 0x060091D3 RID: 37331 RVA: 0x003A23C0 File Offset: 0x003A05C0
	public List<T> GetRange(int index, int count)
	{
		this.LockList.EnterReadLock();
		List<T> range;
		try
		{
			range = this.m_TList.GetRange(index, count);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return range;
	}

	// Token: 0x060091D4 RID: 37332 RVA: 0x003A2408 File Offset: 0x003A0608
	public int IndexOf(T item)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.IndexOf(item);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091D5 RID: 37333 RVA: 0x003A244C File Offset: 0x003A064C
	public int IndexOf(T item, int index)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.IndexOf(item, index);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091D6 RID: 37334 RVA: 0x003A2494 File Offset: 0x003A0694
	public int IndexOf(T item, int index, int count)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.IndexOf(item, index, count);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091D7 RID: 37335 RVA: 0x003A24DC File Offset: 0x003A06DC
	public void Insert(int index, T item)
	{
		this.LockList.ExitWriteLock();
		try
		{
			this.m_TList.Insert(index, item);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	// Token: 0x060091D8 RID: 37336 RVA: 0x003A2520 File Offset: 0x003A0720
	public void InsertRange(int index, IEnumerable<T> range)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.InsertRange(index, range);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	// Token: 0x17000F10 RID: 3856
	// (get) Token: 0x060091D9 RID: 37337 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public bool IsReadOnly
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060091DA RID: 37338 RVA: 0x003A2564 File Offset: 0x003A0764
	public int LastIndexOf(T item)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.LastIndexOf(item);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091DB RID: 37339 RVA: 0x003A25A8 File Offset: 0x003A07A8
	public int LastIndexOf(T item, int index)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.LastIndexOf(item, index);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091DC RID: 37340 RVA: 0x003A25F0 File Offset: 0x003A07F0
	public int LastIndexOf(T item, int index, int count)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.LastIndexOf(item, index, count);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091DD RID: 37341 RVA: 0x003A2638 File Offset: 0x003A0838
	public bool Remove(T item)
	{
		this.LockList.EnterWriteLock();
		bool result;
		try
		{
			result = this.m_TList.Remove(item);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
		return result;
	}

	// Token: 0x060091DE RID: 37342 RVA: 0x003A267C File Offset: 0x003A087C
	public int RemoveAll(Predicate<T> match)
	{
		this.LockList.EnterWriteLock();
		int result;
		try
		{
			result = this.m_TList.RemoveAll(match);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
		return result;
	}

	// Token: 0x060091DF RID: 37343 RVA: 0x003A26C0 File Offset: 0x003A08C0
	public void RemoveAt(int index)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.RemoveAt(index);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	// Token: 0x060091E0 RID: 37344 RVA: 0x003A2704 File Offset: 0x003A0904
	public void RemoveRange(int index, int count)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.RemoveRange(index, count);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	// Token: 0x060091E1 RID: 37345 RVA: 0x003A2748 File Offset: 0x003A0948
	public void Reverse()
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.Reverse();
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	// Token: 0x060091E2 RID: 37346 RVA: 0x003A278C File Offset: 0x003A098C
	public void Reverse(int index, int count)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.Reverse(index, count);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	// Token: 0x060091E3 RID: 37347 RVA: 0x003A27D0 File Offset: 0x003A09D0
	public void Sort()
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.Sort();
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	// Token: 0x060091E4 RID: 37348 RVA: 0x003A2814 File Offset: 0x003A0A14
	public void Sort(Comparison<T> comparison)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.Sort(comparison);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	// Token: 0x060091E5 RID: 37349 RVA: 0x003A2858 File Offset: 0x003A0A58
	public void Sort(IComparer<T> comparer)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.Sort(comparer);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	// Token: 0x060091E6 RID: 37350 RVA: 0x003A289C File Offset: 0x003A0A9C
	public void Sort(int index, int count, IComparer<T> comparer)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.Sort(index, count, comparer);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	// Token: 0x060091E7 RID: 37351 RVA: 0x003A28E0 File Offset: 0x003A0AE0
	public T[] ToArray()
	{
		this.LockList.EnterReadLock();
		T[] result;
		try
		{
			result = this.m_TList.ToArray();
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	// Token: 0x060091E8 RID: 37352 RVA: 0x003A2924 File Offset: 0x003A0B24
	public void TrimExcess()
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.TrimExcess();
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	// Token: 0x060091E9 RID: 37353 RVA: 0x003A2968 File Offset: 0x003A0B68
	public bool TrueForAll(Predicate<T> match)
	{
		this.LockList.EnterWriteLock();
		bool result;
		try
		{
			result = this.m_TList.TrueForAll(match);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
		return result;
	}

	// Token: 0x04006FD7 RID: 28631
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<T> m_TList;

	// Token: 0x04006FD8 RID: 28632
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly ReaderWriterLockSlim LockList = new ReaderWriterLockSlim();

	// Token: 0x04006FD9 RID: 28633
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_Disposed;
}
