using System;

// Token: 0x02001115 RID: 4373
public class ArrayListMP<T> where T : new()
{
	// Token: 0x0600897B RID: 35195 RVA: 0x0037BF56 File Offset: 0x0037A156
	public ArrayListMP(MemoryPooledArray<T> _pool, int _minSize = 0)
	{
		this.pool = _pool;
		this.Count = 0;
		if (_minSize > 0)
		{
			this.Items = this.pool.Alloc(_minSize);
		}
	}

	// Token: 0x0600897C RID: 35196 RVA: 0x0037BF84 File Offset: 0x0037A184
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~ArrayListMP()
	{
		if (this.Items != null)
		{
			this.pool.Free(this.Items);
			this.Items = null;
		}
	}

	// Token: 0x0600897D RID: 35197 RVA: 0x0037BFCC File Offset: 0x0037A1CC
	public void Add(T _item)
	{
		if (this.Items == null)
		{
			this.Items = this.pool.Alloc(0);
		}
		if (this.Count >= this.Items.Length)
		{
			this.Items = this.pool.Grow(this.Items);
		}
		T[] items = this.Items;
		int count = this.Count;
		this.Count = count + 1;
		items[count] = _item;
	}

	// Token: 0x17000E4E RID: 3662
	public T this[int idx]
	{
		get
		{
			return this.Items[idx];
		}
		set
		{
			this.Items[idx] = value;
		}
	}

	// Token: 0x06008980 RID: 35200 RVA: 0x0037C054 File Offset: 0x0037A254
	public void Clear()
	{
		this.Count = 0;
		if (this.Items != null)
		{
			this.pool.Free(this.Items);
			this.Items = null;
		}
	}

	// Token: 0x06008981 RID: 35201 RVA: 0x0037C080 File Offset: 0x0037A280
	public T[] ToArray()
	{
		if (this.Items == null)
		{
			return new T[0];
		}
		T[] array = new T[this.Count];
		Array.Copy(this.Items, array, this.Count);
		return array;
	}

	// Token: 0x06008982 RID: 35202 RVA: 0x0037C0BB File Offset: 0x0037A2BB
	public void AddRange(T[] _range)
	{
		this.AddRange(_range, 0, _range.Length);
	}

	// Token: 0x06008983 RID: 35203 RVA: 0x0037C0C8 File Offset: 0x0037A2C8
	public void AddRange(T[] _range, int _offs, int _count)
	{
		if (_range == null || _range.Length == 0)
		{
			return;
		}
		if (this.Items == null)
		{
			this.Items = this.pool.Alloc(_count);
		}
		if (this.Count + _count >= this.Items.Length)
		{
			this.Items = this.pool.Grow(this.Items, this.Count + _count);
		}
		Array.Copy(_range, _offs, this.Items, this.Count, _count);
		this.Count += _count;
	}

	// Token: 0x06008984 RID: 35204 RVA: 0x0037C14C File Offset: 0x0037A34C
	public int Alloc(int _count)
	{
		if (this.Items == null)
		{
			this.Items = this.pool.Alloc(_count);
		}
		else if (this.Count + _count > this.Items.Length)
		{
			this.Items = this.pool.Grow(this.Items, this.Count + _count);
		}
		int count = this.Count;
		this.Count += _count;
		return count;
	}

	// Token: 0x06008985 RID: 35205 RVA: 0x0037C1BA File Offset: 0x0037A3BA
	public void Grow(int newSize)
	{
		if (this.Items == null)
		{
			this.Items = this.pool.Alloc(newSize);
			return;
		}
		if (newSize > this.Items.Length)
		{
			this.Items = this.pool.Grow(this.Items, newSize);
		}
	}

	// Token: 0x04006BD3 RID: 27603
	public readonly MemoryPooledArray<T> pool;

	// Token: 0x04006BD4 RID: 27604
	public T[] Items;

	// Token: 0x04006BD5 RID: 27605
	public int Count;
}
