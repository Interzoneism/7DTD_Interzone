using System;
using System.Collections.Generic;

// Token: 0x02000AC1 RID: 2753
public class ThreadProcessingPool
{
	// Token: 0x17000864 RID: 2148
	// (get) Token: 0x060054CE RID: 21710 RVA: 0x0022A6EC File Offset: 0x002288EC
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

	// Token: 0x060054CF RID: 21711 RVA: 0x0022A734 File Offset: 0x00228934
	public ThreadProcessingPool(int initialCapacity, int initialCount)
	{
		this._poolLock = new object();
		this.pool = new List<ThreadProcessing>(initialCapacity);
		for (int i = 0; i < initialCount; i++)
		{
			this.pool.Add(new ThreadProcessing());
		}
	}

	// Token: 0x060054D0 RID: 21712 RVA: 0x0022A77C File Offset: 0x0022897C
	public ThreadProcessing GetObject(List<ThreadInfoParam> _JobList)
	{
		object poolLock = this._poolLock;
		ThreadProcessing result;
		lock (poolLock)
		{
			if (this.pool.Count == 0)
			{
				result = new ThreadProcessing(_JobList);
			}
			else
			{
				ThreadProcessing threadProcessing = this.pool[this.pool.Count - 1];
				this.pool.RemoveAt(this.pool.Count - 1);
				threadProcessing.Init(_JobList);
				result = threadProcessing;
			}
		}
		return result;
	}

	// Token: 0x060054D1 RID: 21713 RVA: 0x0022A808 File Offset: 0x00228A08
	public void ReturnObject(ThreadProcessing item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("ThreadProcessing is null");
		}
		object poolLock = this._poolLock;
		lock (poolLock)
		{
			if (this.pool.Contains(item))
			{
				throw new InvalidOperationException("ThreadProcessing already in pool");
			}
			this.pool.Add(item);
		}
	}

	// Token: 0x040041AC RID: 16812
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ThreadProcessing> pool;

	// Token: 0x040041AD RID: 16813
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly object _poolLock;
}
