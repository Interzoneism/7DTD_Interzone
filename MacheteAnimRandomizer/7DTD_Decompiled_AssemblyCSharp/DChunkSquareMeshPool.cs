using System;
using System.Collections.Generic;

// Token: 0x02000AB1 RID: 2737
public class DChunkSquareMeshPool
{
	// Token: 0x1700085F RID: 2143
	// (get) Token: 0x06005463 RID: 21603 RVA: 0x0021F570 File Offset: 0x0021D770
	public int Count
	{
		get
		{
			object poolLock = this._poolLock;
			int result;
			lock (poolLock)
			{
				int num = 0;
				for (int i = 0; i < this.pool.Length; i++)
				{
					num += this.pool[i].Count;
				}
				result = num;
			}
			return result;
		}
	}

	// Token: 0x06005464 RID: 21604 RVA: 0x0021F5D4 File Offset: 0x0021D7D4
	public DChunkSquareMeshPool(int initialCapacity, int NbLODLevel)
	{
		this._poolLock = new object();
		this.pool = new List<DChunkSquareMesh>[NbLODLevel];
		for (int i = 0; i < NbLODLevel; i++)
		{
			this.pool[i] = new List<DChunkSquareMesh>(initialCapacity);
		}
	}

	// Token: 0x06005465 RID: 21605 RVA: 0x0021F618 File Offset: 0x0021D818
	public DChunkSquareMesh GetObject(DistantChunkMap DCMap, int LODLevel)
	{
		object poolLock = this._poolLock;
		DChunkSquareMesh result;
		lock (poolLock)
		{
			List<DChunkSquareMesh> list = this.pool[LODLevel];
			if (list.Count == 0)
			{
				result = new DChunkSquareMesh(DCMap, LODLevel);
			}
			else
			{
				DChunkSquareMesh dchunkSquareMesh = list[list.Count - 1];
				list.RemoveAt(list.Count - 1);
				result = dchunkSquareMesh;
			}
		}
		return result;
	}

	// Token: 0x06005466 RID: 21606 RVA: 0x0021F690 File Offset: 0x0021D890
	public void ReturnObject(DChunkSquareMesh item, int LODLevel)
	{
		if (item == null)
		{
			throw new ArgumentNullException("DChunkSquareMesh is null");
		}
		object poolLock = this._poolLock;
		lock (poolLock)
		{
			if (this.pool[LODLevel].Contains(item))
			{
				throw new InvalidOperationException("ThreadProcessing already in pool");
			}
			this.pool[LODLevel].Add(item);
		}
	}

	// Token: 0x040040BD RID: 16573
	[PublicizedFrom(EAccessModifier.Private)]
	public List<DChunkSquareMesh>[] pool;

	// Token: 0x040040BE RID: 16574
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly object _poolLock;
}
