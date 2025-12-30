using System;
using System.Collections.Generic;

// Token: 0x020011C3 RID: 4547
public class MemoryPooledArray<T> where T : new()
{
	// Token: 0x06008E1D RID: 36381 RVA: 0x0038F60C File Offset: 0x0038D80C
	public MemoryPooledArray()
	{
		this.pools = new List<T[]>[MemoryPooledArraySizes.poolElements.Length];
		for (int i = 0; i < this.pools.Length; i++)
		{
			this.pools[i] = new List<T[]>();
		}
		this.poolSize = new int[MemoryPooledArraySizes.poolElements.Length];
	}

	// Token: 0x06008E1E RID: 36382 RVA: 0x0038F664 File Offset: 0x0038D864
	public T[] Alloc(int _minSize = 0)
	{
		List<T[]>[] obj = this.pools;
		T[] result;
		lock (obj)
		{
			int num = this.sizeToIdx(_minSize);
			T[] array;
			if (this.poolSize[num] == 0)
			{
				array = new T[MemoryPooledArraySizes.poolElements[num]];
			}
			else
			{
				int[] array2 = this.poolSize;
				int num2 = num;
				int num3 = array2[num2] - 1;
				array2[num2] = num3;
				int index = num3;
				array = this.pools[num][index];
				this.pools[num][index] = null;
			}
			result = array;
		}
		return result;
	}

	// Token: 0x06008E1F RID: 36383 RVA: 0x0038F700 File Offset: 0x0038D900
	public T[] Grow(T[] _array)
	{
		return this.Grow(_array, _array.Length + 1);
	}

	// Token: 0x06008E20 RID: 36384 RVA: 0x0038F710 File Offset: 0x0038D910
	public T[] Grow(T[] _array, int _minSize)
	{
		T[] array = this.Alloc(_minSize);
		Array.Copy(_array, array, _array.Length);
		this.Free(_array);
		return array;
	}

	// Token: 0x06008E21 RID: 36385 RVA: 0x0038F738 File Offset: 0x0038D938
	public void Free(T[] _array)
	{
		if (_array == null)
		{
			return;
		}
		List<T[]>[] obj = this.pools;
		lock (obj)
		{
			int num = this.sizeToIdx(_array.Length);
			if (this.poolSize[num] >= this.pools[num].Count)
			{
				this.pools[num].Add(_array);
				this.poolSize[num]++;
			}
			else
			{
				List<T[]> list = this.pools[num];
				int[] array = this.poolSize;
				int num2 = num;
				int num3 = array[num2];
				array[num2] = num3 + 1;
				list[num3] = _array;
			}
		}
	}

	// Token: 0x06008E22 RID: 36386 RVA: 0x0038F7DC File Offset: 0x0038D9DC
	public void FreeAll()
	{
		List<T[]>[] obj = this.pools;
		lock (obj)
		{
			for (int i = 0; i < this.pools.Length; i++)
			{
				this.pools[i].Clear();
				this.poolSize[i] = 0;
			}
		}
	}

	// Token: 0x06008E23 RID: 36387 RVA: 0x0038F840 File Offset: 0x0038DA40
	[PublicizedFrom(EAccessModifier.Private)]
	public int sizeToIdx(int _size)
	{
		int num = -1;
		for (int i = 0; i < MemoryPooledArraySizes.poolElements.Length; i++)
		{
			if (MemoryPooledArraySizes.poolElements[i] >= _size)
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			throw new Exception("Array length in pool not supported " + _size.ToString());
		}
		return num;
	}

	// Token: 0x06008E24 RID: 36388 RVA: 0x0038F88C File Offset: 0x0038DA8C
	public int GetCount()
	{
		int num = 0;
		for (int i = 0; i < this.pools.Length; i++)
		{
			if (this.pools[i] != null)
			{
				num += this.pools[i].Count;
			}
		}
		return num;
	}

	// Token: 0x06008E25 RID: 36389 RVA: 0x0038F8C9 File Offset: 0x0038DAC9
	public int GetCount(int _poolIndex)
	{
		if (_poolIndex < 0 || _poolIndex >= this.pools.Length)
		{
			return 0;
		}
		List<T[]> list = this.pools[_poolIndex];
		if (list == null)
		{
			return 0;
		}
		return list.Count;
	}

	// Token: 0x06008E26 RID: 36390 RVA: 0x0038F8F0 File Offset: 0x0038DAF0
	public long GetElementsCount()
	{
		int num = 0;
		for (int i = 0; i < this.pools.Length; i++)
		{
			if (this.pools[i] != null)
			{
				num += this.pools[i].Count * MemoryPooledArraySizes.poolElements[i];
			}
		}
		return (long)num;
	}

	// Token: 0x04006E1C RID: 28188
	[PublicizedFrom(EAccessModifier.Private)]
	public List<T[]>[] pools;

	// Token: 0x04006E1D RID: 28189
	[PublicizedFrom(EAccessModifier.Private)]
	public int[] poolSize;

	// Token: 0x04006E1E RID: 28190
	[PublicizedFrom(EAccessModifier.Private)]
	public int maxCapacity;
}
