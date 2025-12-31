using System;
using System.Collections.Generic;

// Token: 0x020011C5 RID: 4549
public class MemoryPooledObject<T> where T : IMemoryPoolableObject, new()
{
	// Token: 0x06008E29 RID: 36393 RVA: 0x0038F936 File Offset: 0x0038DB36
	public MemoryPooledObject(int _maxCapacity)
	{
		this.pool = new List<T>(_maxCapacity);
		this.maxCapacity = _maxCapacity;
	}

	// Token: 0x06008E2A RID: 36394 RVA: 0x0038F951 File Offset: 0x0038DB51
	public void SetCapacity(int _maxCapacity)
	{
		this.pool.Capacity = _maxCapacity;
		this.maxCapacity = _maxCapacity;
	}

	// Token: 0x06008E2B RID: 36395 RVA: 0x0038F968 File Offset: 0x0038DB68
	public T AllocSync(bool _bReset)
	{
		List<T> obj = this.pool;
		T result;
		lock (obj)
		{
			result = this.Alloc(_bReset);
		}
		return result;
	}

	// Token: 0x06008E2C RID: 36396 RVA: 0x0038F9AC File Offset: 0x0038DBAC
	public T Alloc(bool _bReset)
	{
		T result;
		if (this.poolSize == 0)
		{
			result = Activator.CreateInstance<T>();
		}
		else
		{
			this.poolSize--;
			result = this.pool[this.poolSize];
			this.pool[this.poolSize] = default(T);
		}
		if (_bReset)
		{
			result.Reset();
		}
		return result;
	}

	// Token: 0x06008E2D RID: 36397 RVA: 0x0038FA14 File Offset: 0x0038DC14
	public void FreeSync(IList<T> _array)
	{
		List<T> obj = this.pool;
		lock (obj)
		{
			for (int i = 0; i < _array.Count; i++)
			{
				T t = _array[i];
				if (t != null)
				{
					this.Free(t);
					_array[i] = default(T);
				}
			}
		}
	}

	// Token: 0x06008E2E RID: 36398 RVA: 0x0038FA88 File Offset: 0x0038DC88
	public void FreeSync(Queue<T> _queue)
	{
		List<T> obj = this.pool;
		lock (obj)
		{
			while (_queue.Count > 0)
			{
				this.Free(_queue.Dequeue());
			}
		}
	}

	// Token: 0x06008E2F RID: 36399 RVA: 0x0038FADC File Offset: 0x0038DCDC
	public void FreeSync(T _t)
	{
		List<T> obj = this.pool;
		lock (obj)
		{
			this.Free(_t);
		}
	}

	// Token: 0x06008E30 RID: 36400 RVA: 0x0038FB20 File Offset: 0x0038DD20
	public void Free(T[] _array)
	{
		for (int i = 0; i < _array.Length; i++)
		{
			T t = _array[i];
			if (t != null)
			{
				this.Free(t);
				_array[i] = default(T);
			}
		}
	}

	// Token: 0x06008E31 RID: 36401 RVA: 0x0038FB64 File Offset: 0x0038DD64
	public void Free(List<T> _list)
	{
		for (int i = 0; i < _list.Count; i++)
		{
			T t = _list[i];
			if (t != null)
			{
				this.Free(t);
			}
		}
		_list.Clear();
	}

	// Token: 0x06008E32 RID: 36402 RVA: 0x0038FBA0 File Offset: 0x0038DDA0
	public void Cleanup()
	{
		List<T> obj = this.pool;
		lock (obj)
		{
			for (int i = 0; i < this.poolSize; i++)
			{
				T t = this.pool[i];
				if (t != null)
				{
					t.Cleanup();
				}
			}
			this.pool.Clear();
			this.poolSize = 0;
		}
	}

	// Token: 0x06008E33 RID: 36403 RVA: 0x0038FC20 File Offset: 0x0038DE20
	public void Free(T _t)
	{
		if (this.poolSize >= this.pool.Count && this.poolSize < this.maxCapacity)
		{
			_t.Reset();
			this.pool.Add(_t);
			this.poolSize++;
			return;
		}
		if (this.poolSize < this.maxCapacity)
		{
			_t.Reset();
			List<T> list = this.pool;
			int num = this.poolSize;
			this.poolSize = num + 1;
			list[num] = _t;
			return;
		}
		_t.Cleanup();
	}

	// Token: 0x06008E34 RID: 36404 RVA: 0x0038FCBC File Offset: 0x0038DEBC
	public int GetPoolSize()
	{
		return this.poolSize;
	}

	// Token: 0x04006E1F RID: 28191
	[PublicizedFrom(EAccessModifier.Private)]
	public List<T> pool;

	// Token: 0x04006E20 RID: 28192
	[PublicizedFrom(EAccessModifier.Private)]
	public int poolSize;

	// Token: 0x04006E21 RID: 28193
	[PublicizedFrom(EAccessModifier.Private)]
	public int maxCapacity;
}
