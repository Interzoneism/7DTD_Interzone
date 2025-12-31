using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

// Token: 0x0200123F RID: 4671
public class TQueue<T> : ICollection, IEnumerable
{
	// Token: 0x060091F8 RID: 37368 RVA: 0x003A2C4B File Offset: 0x003A0E4B
	public TQueue()
	{
		this.m_Queue = new Queue<T>();
	}

	// Token: 0x060091F9 RID: 37369 RVA: 0x003A2C74 File Offset: 0x003A0E74
	public TQueue(int capacity)
	{
		this.m_Queue = new Queue<T>(capacity);
	}

	// Token: 0x060091FA RID: 37370 RVA: 0x003A2C9E File Offset: 0x003A0E9E
	public TQueue(IEnumerable<T> collection)
	{
		this.m_Queue = new Queue<T>(collection);
	}

	// Token: 0x060091FB RID: 37371 RVA: 0x003A2CC8 File Offset: 0x003A0EC8
	public IEnumerator<T> GetEnumerator()
	{
		this.LockQ.EnterReadLock();
		Queue<T> queue;
		try
		{
			queue = new Queue<T>(this.m_Queue);
		}
		finally
		{
			this.LockQ.ExitReadLock();
		}
		foreach (T t in queue)
		{
			yield return t;
		}
		Queue<T>.Enumerator enumerator = default(Queue<T>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x060091FC RID: 37372 RVA: 0x003A2CD7 File Offset: 0x003A0ED7
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator GetEnumerator()
	{
		this.LockQ.EnterReadLock();
		Queue<T> queue;
		try
		{
			queue = new Queue<T>(this.m_Queue);
		}
		finally
		{
			this.LockQ.ExitReadLock();
		}
		foreach (T t in queue)
		{
			yield return t;
		}
		Queue<T>.Enumerator enumerator = default(Queue<T>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x060091FD RID: 37373 RVA: 0x003A2CE8 File Offset: 0x003A0EE8
	public void CopyTo(Array array, int index)
	{
		this.LockQ.EnterReadLock();
		try
		{
			this.m_Queue.ToArray().CopyTo(array, index);
		}
		finally
		{
			this.LockQ.ExitReadLock();
		}
	}

	// Token: 0x060091FE RID: 37374 RVA: 0x003A2D30 File Offset: 0x003A0F30
	public void CopyTo(T[] array, int index)
	{
		this.LockQ.EnterReadLock();
		try
		{
			this.m_Queue.CopyTo(array, index);
		}
		finally
		{
			this.LockQ.ExitReadLock();
		}
	}

	// Token: 0x17000F15 RID: 3861
	// (get) Token: 0x060091FF RID: 37375 RVA: 0x003A2D74 File Offset: 0x003A0F74
	public int Count
	{
		get
		{
			this.LockQ.EnterReadLock();
			int count;
			try
			{
				count = this.m_Queue.Count;
			}
			finally
			{
				this.LockQ.ExitReadLock();
			}
			return count;
		}
	}

	// Token: 0x17000F16 RID: 3862
	// (get) Token: 0x06009200 RID: 37376 RVA: 0x000197A5 File Offset: 0x000179A5
	public bool IsSynchronized
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000F17 RID: 3863
	// (get) Token: 0x06009201 RID: 37377 RVA: 0x003A2DB8 File Offset: 0x003A0FB8
	public object SyncRoot
	{
		get
		{
			return this.objSyncRoot;
		}
	}

	// Token: 0x06009202 RID: 37378 RVA: 0x003A2DC0 File Offset: 0x003A0FC0
	public void Enqueue(T item)
	{
		this.LockQ.EnterWriteLock();
		try
		{
			this.m_Queue.Enqueue(item);
		}
		finally
		{
			this.LockQ.ExitWriteLock();
		}
	}

	// Token: 0x06009203 RID: 37379 RVA: 0x003A2E04 File Offset: 0x003A1004
	public T Dequeue()
	{
		this.LockQ.EnterWriteLock();
		T result;
		try
		{
			result = this.m_Queue.Dequeue();
		}
		finally
		{
			this.LockQ.ExitWriteLock();
		}
		return result;
	}

	// Token: 0x06009204 RID: 37380 RVA: 0x003A2E48 File Offset: 0x003A1048
	public void EnqueueAll(IEnumerable<T> ItemsToQueue)
	{
		this.LockQ.EnterWriteLock();
		try
		{
			foreach (T item in ItemsToQueue)
			{
				this.m_Queue.Enqueue(item);
			}
		}
		finally
		{
			this.LockQ.ExitWriteLock();
		}
	}

	// Token: 0x06009205 RID: 37381 RVA: 0x003A2EB8 File Offset: 0x003A10B8
	public void EnqueueAll(TList<T> ItemsToQueue)
	{
		this.LockQ.EnterWriteLock();
		try
		{
			foreach (object obj in ItemsToQueue)
			{
				T item = (T)((object)obj);
				this.m_Queue.Enqueue(item);
			}
		}
		finally
		{
			this.LockQ.ExitWriteLock();
		}
	}

	// Token: 0x06009206 RID: 37382 RVA: 0x003A2F34 File Offset: 0x003A1134
	public TList<T> DequeueAll()
	{
		this.LockQ.EnterWriteLock();
		TList<T> result;
		try
		{
			TList<T> tlist = new TList<T>();
			while (this.m_Queue.Count > 0)
			{
				tlist.Add(this.m_Queue.Dequeue());
			}
			result = tlist;
		}
		finally
		{
			this.LockQ.ExitWriteLock();
		}
		return result;
	}

	// Token: 0x06009207 RID: 37383 RVA: 0x003A2F94 File Offset: 0x003A1194
	public void Clear()
	{
		this.LockQ.EnterWriteLock();
		try
		{
			this.m_Queue.Clear();
		}
		finally
		{
			this.LockQ.ExitWriteLock();
		}
	}

	// Token: 0x06009208 RID: 37384 RVA: 0x003A2FD8 File Offset: 0x003A11D8
	public bool Contains(T item)
	{
		this.LockQ.EnterReadLock();
		bool result;
		try
		{
			result = this.m_Queue.Contains(item);
		}
		finally
		{
			this.LockQ.ExitReadLock();
		}
		return result;
	}

	// Token: 0x06009209 RID: 37385 RVA: 0x003A301C File Offset: 0x003A121C
	public T Peek()
	{
		this.LockQ.EnterReadLock();
		T result;
		try
		{
			result = this.m_Queue.Peek();
		}
		finally
		{
			this.LockQ.ExitReadLock();
		}
		return result;
	}

	// Token: 0x0600920A RID: 37386 RVA: 0x003A3060 File Offset: 0x003A1260
	public T[] ToArray()
	{
		this.LockQ.EnterReadLock();
		T[] result;
		try
		{
			result = this.m_Queue.ToArray();
		}
		finally
		{
			this.LockQ.ExitReadLock();
		}
		return result;
	}

	// Token: 0x0600920B RID: 37387 RVA: 0x003A30A4 File Offset: 0x003A12A4
	public void TrimExcess()
	{
		this.LockQ.EnterWriteLock();
		try
		{
			this.m_Queue.TrimExcess();
		}
		finally
		{
			this.LockQ.ExitWriteLock();
		}
	}

	// Token: 0x04006FE2 RID: 28642
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Queue<T> m_Queue;

	// Token: 0x04006FE3 RID: 28643
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly ReaderWriterLockSlim LockQ = new ReaderWriterLockSlim();

	// Token: 0x04006FE4 RID: 28644
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly object objSyncRoot = new object();
}
