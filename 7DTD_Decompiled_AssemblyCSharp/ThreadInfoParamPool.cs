using System;
using System.Collections.Generic;

// Token: 0x02000ABF RID: 2751
public class ThreadInfoParamPool
{
	// Token: 0x17000862 RID: 2146
	// (get) Token: 0x060054BE RID: 21694 RVA: 0x0022A0EC File Offset: 0x002282EC
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

	// Token: 0x17000863 RID: 2147
	// (get) Token: 0x060054BF RID: 21695 RVA: 0x0022A134 File Offset: 0x00228334
	public int CountBig
	{
		get
		{
			object poolLock = this._poolLock;
			int count;
			lock (poolLock)
			{
				count = this.poolBig.Count;
			}
			return count;
		}
	}

	// Token: 0x060054C0 RID: 21696 RVA: 0x0022A17C File Offset: 0x0022837C
	public ThreadInfoParamPool(int initialCapacity, int initialCapacityBig, int initialCount)
	{
		this._poolLock = new object();
		this.pool = new List<ThreadInfoParam>(initialCapacity);
		this.poolBig = new List<ThreadInfoParam>(initialCapacityBig);
		for (int i = 0; i < initialCount; i++)
		{
			this.pool.Add(new ThreadInfoParam());
			this.poolBig.Add(new ThreadInfoParam());
		}
	}

	// Token: 0x060054C1 RID: 21697 RVA: 0x0022A1E0 File Offset: 0x002283E0
	public ThreadInfoParam GetObject(DistantChunkMap _CMap, int _ResLevel, int _OutId)
	{
		object poolLock = this._poolLock;
		ThreadInfoParam result;
		lock (poolLock)
		{
			if (this.pool.Count == 0)
			{
				result = new ThreadInfoParam(_CMap, _ResLevel, _OutId, false);
			}
			else
			{
				ThreadInfoParam threadInfoParam = this.pool[this.pool.Count - 1];
				this.pool.RemoveAt(this.pool.Count - 1);
				threadInfoParam.Init(_CMap, _ResLevel, _OutId, false);
				result = threadInfoParam;
			}
		}
		return result;
	}

	// Token: 0x060054C2 RID: 21698 RVA: 0x0022A270 File Offset: 0x00228470
	public ThreadInfoParam GetObjectBig(DistantChunkMap _CMap, int _ResLevel, int _OutId)
	{
		object poolLock = this._poolLock;
		ThreadInfoParam result;
		lock (poolLock)
		{
			if (this.poolBig.Count == 0)
			{
				result = new ThreadInfoParam(_CMap, _ResLevel, _OutId, true);
			}
			else
			{
				ThreadInfoParam threadInfoParam = this.poolBig[this.poolBig.Count - 1];
				this.poolBig.RemoveAt(this.poolBig.Count - 1);
				threadInfoParam.Init(_CMap, _ResLevel, _OutId, true);
				result = threadInfoParam;
			}
		}
		return result;
	}

	// Token: 0x060054C3 RID: 21699 RVA: 0x0022A300 File Offset: 0x00228500
	public void ReturnObject(ThreadInfoParam item, ThreadContainerPool TmpThContPool = null)
	{
		if (item == null)
		{
			throw new ArgumentNullException("ThreadInfoParam is null");
		}
		item.ClearAll(TmpThContPool);
		object poolLock = this._poolLock;
		lock (poolLock)
		{
			if (item.IsBigCapacity)
			{
				if (this.poolBig.Contains(item))
				{
					throw new InvalidOperationException("ThreadInfoParam Big already in pool");
				}
				this.poolBig.Add(item);
			}
			else
			{
				if (this.pool.Contains(item))
				{
					throw new InvalidOperationException("ThreadInfoParam already in pool");
				}
				this.pool.Add(item);
			}
		}
	}

	// Token: 0x040041A4 RID: 16804
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ThreadInfoParam> pool;

	// Token: 0x040041A5 RID: 16805
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ThreadInfoParam> poolBig;

	// Token: 0x040041A6 RID: 16806
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly object _poolLock;
}
