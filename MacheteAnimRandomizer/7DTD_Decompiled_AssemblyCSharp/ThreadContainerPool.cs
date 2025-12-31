using System;
using System.Collections.Generic;

// Token: 0x02000AC3 RID: 2755
public class ThreadContainerPool
{
	// Token: 0x17000865 RID: 2149
	// (get) Token: 0x060054D8 RID: 21720 RVA: 0x0022A938 File Offset: 0x00228B38
	public int Count
	{
		get
		{
			object poolLock = this._poolLock;
			int count;
			lock (poolLock)
			{
				count = this.pool.Count;
			}
			return count;
		}
	}

	// Token: 0x060054D9 RID: 21721 RVA: 0x0022A980 File Offset: 0x00228B80
	public ThreadContainerPool(int initialCapacity, int initialCount)
	{
		this._poolLock = new object();
		this.pool = new List<ThreadContainer>(initialCapacity);
		for (int i = 0; i < initialCount; i++)
		{
			this.pool.Add(new ThreadContainer());
		}
	}

	// Token: 0x060054DA RID: 21722 RVA: 0x0022A9C8 File Offset: 0x00228BC8
	public ThreadContainer GetObject(DistantTerrain _TerExt, DistantChunk _DChunk, DistantChunkBasicMesh _BMesh, bool _WasReset)
	{
		object poolLock = this._poolLock;
		ThreadContainer result;
		lock (poolLock)
		{
			if (this.pool.Count == 0)
			{
				result = new ThreadContainer(_TerExt, _DChunk, _BMesh, _WasReset);
			}
			else
			{
				ThreadContainer threadContainer = this.pool[this.pool.Count - 1];
				this.pool.RemoveAt(this.pool.Count - 1);
				threadContainer.Init(_TerExt, _DChunk, _BMesh, _WasReset);
				result = threadContainer;
			}
		}
		return result;
	}

	// Token: 0x060054DB RID: 21723 RVA: 0x0022AA5C File Offset: 0x00228C5C
	public void ReturnObject(ThreadContainer item, bool IsClearItem)
	{
		if (item == null)
		{
			throw new ArgumentNullException("ThreadContainer is null");
		}
		object poolLock = this._poolLock;
		lock (poolLock)
		{
			if (this.pool.Contains(item))
			{
				throw new InvalidOperationException("ThreadContainer already in pool");
			}
			item.Clear(IsClearItem);
			this.pool.Add(item);
		}
	}

	// Token: 0x040041B3 RID: 16819
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ThreadContainer> pool;

	// Token: 0x040041B4 RID: 16820
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly object _poolLock;
}
