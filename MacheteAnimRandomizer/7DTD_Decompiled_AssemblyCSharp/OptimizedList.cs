using System;
using System.Collections.Generic;

// Token: 0x020011DE RID: 4574
[Serializable]
public class OptimizedList<T>
{
	// Token: 0x06008EC3 RID: 36547 RVA: 0x00391E00 File Offset: 0x00390000
	public OptimizedList() : this(10)
	{
		this.IsValueType = typeof(T).IsValueType;
	}

	// Token: 0x06008EC4 RID: 36548 RVA: 0x00391E1F File Offset: 0x0039001F
	public OptimizedList(int Capacity)
	{
		this.Count = 0;
		this.array = new T[Capacity];
		this.length = Capacity;
		this.DoubleSize = 0;
		this.IsValueType = typeof(T).IsValueType;
	}

	// Token: 0x06008EC5 RID: 36549 RVA: 0x00391E60 File Offset: 0x00390060
	public OptimizedList(OptimizedList<T> L)
	{
		if (L.Count == 0)
		{
			this.Count = 0;
			this.array = new T[2];
			this.length = 2;
			return;
		}
		this.Count = 0;
		this.array = new T[L.Count];
		this.length = L.Count;
		this.DoubleSize = 0;
		this.IsValueType = typeof(T).IsValueType;
		this.AddRange(L);
	}

	// Token: 0x06008EC6 RID: 36550 RVA: 0x00391EE0 File Offset: 0x003900E0
	public OptimizedList(T[] L)
	{
		if (L.Length == 0)
		{
			this.Count = 0;
			this.array = new T[2];
			this.length = 2;
			return;
		}
		this.Count = 0;
		this.array = new T[L.Length];
		this.length = L.Length;
		this.DoubleSize = 0;
		this.IsValueType = typeof(T).IsValueType;
		this.AddRange(L);
	}

	// Token: 0x06008EC7 RID: 36551 RVA: 0x00391F53 File Offset: 0x00390153
	public OptimizedList(int Capacity, int DoubleSize)
	{
		this.Count = 0;
		this.array = new T[Capacity];
		this.length = Capacity;
		this.DoubleSize = DoubleSize;
		this.IsValueType = typeof(T).IsValueType;
	}

	// Token: 0x06008EC8 RID: 36552 RVA: 0x00391F94 File Offset: 0x00390194
	public T Last()
	{
		if (this.Count > 0)
		{
			return this.array[this.Count - 1];
		}
		return default(T);
	}

	// Token: 0x06008EC9 RID: 36553 RVA: 0x00391FC8 File Offset: 0x003901C8
	public void Add(T value)
	{
		if (this.length == this.Count)
		{
			this.length = ((this.DoubleSize == 0) ? (this.Count * 2) : (this.Count + this.DoubleSize));
			T[] destinationArray = new T[this.length];
			Array.Copy(this.array, 0, destinationArray, 0, this.Count);
			this.array = destinationArray;
		}
		T[] array = this.array;
		int count = this.Count;
		this.Count = count + 1;
		array[count] = value;
	}

	// Token: 0x06008ECA RID: 36554 RVA: 0x0039204C File Offset: 0x0039024C
	public void Add(ref T value)
	{
		if (this.length == this.Count)
		{
			this.length = ((this.DoubleSize == 0) ? (this.Count * 2) : (this.Count + this.DoubleSize));
			T[] destinationArray = new T[this.length];
			Array.Copy(this.array, 0, destinationArray, 0, this.Count);
			this.array = destinationArray;
		}
		T[] array = this.array;
		int count = this.Count;
		this.Count = count + 1;
		array[count] = value;
	}

	// Token: 0x06008ECB RID: 36555 RVA: 0x003920D8 File Offset: 0x003902D8
	public void AddSafe(T value)
	{
		T[] array = this.array;
		int count = this.Count;
		this.Count = count + 1;
		array[count] = value;
	}

	// Token: 0x06008ECC RID: 36556 RVA: 0x00392104 File Offset: 0x00390304
	public void AddSafe(T valueA, T valueB, T valueC, T valueD)
	{
		if (this.length - (this.Count + 4) <= 0)
		{
			this.length = ((this.DoubleSize == 0) ? (this.Count * 2 + 4) : (this.Count + this.DoubleSize + 4));
			T[] destinationArray = new T[this.length];
			Array.Copy(this.array, 0, destinationArray, 0, this.Count);
			this.array = destinationArray;
		}
		T[] array = this.array;
		int count = this.Count;
		this.Count = count + 1;
		array[count] = valueA;
		T[] array2 = this.array;
		count = this.Count;
		this.Count = count + 1;
		array2[count] = valueB;
		T[] array3 = this.array;
		count = this.Count;
		this.Count = count + 1;
		array3[count] = valueC;
		T[] array4 = this.array;
		count = this.Count;
		this.Count = count + 1;
		array4[count] = valueD;
	}

	// Token: 0x06008ECD RID: 36557 RVA: 0x003921E8 File Offset: 0x003903E8
	public void AddRange(T[] values)
	{
		int num = values.Length;
		if (this.length - (this.Count + num) <= 0)
		{
			T[] destinationArray = new T[this.Count * 2 + num];
			Array.Copy(this.array, 0, destinationArray, 0, this.Count);
			this.array = destinationArray;
			this.length = this.Count * 2 + num;
		}
		Array.Copy(values, 0, this.array, this.Count, num);
		this.Count += num;
	}

	// Token: 0x06008ECE RID: 36558 RVA: 0x00392268 File Offset: 0x00390468
	public void AddRange(OptimizedList<T> values)
	{
		if (values == null || values.Count == 0)
		{
			return;
		}
		int num = values.Length;
		if (this.length - (this.Count + num) <= 0)
		{
			this.length = this.Count * 2 + num;
			T[] destinationArray = new T[this.length];
			Array.Copy(this.array, 0, destinationArray, 0, this.Count);
			this.array = destinationArray;
		}
		Array.Copy(values.array, 0, this.array, this.Count, num);
		this.Count += num;
	}

	// Token: 0x06008ECF RID: 36559 RVA: 0x003922F8 File Offset: 0x003904F8
	public bool Remove(T obj)
	{
		int num = Array.IndexOf<T>(this.array, obj, 0, this.Count);
		if (num >= 0)
		{
			this.Count--;
			if (num < this.Count)
			{
				Array.Copy(this.array, num + 1, this.array, num, this.Count - num);
			}
			this.array[this.Count] = default(T);
			return true;
		}
		return false;
	}

	// Token: 0x06008ED0 RID: 36560 RVA: 0x00392370 File Offset: 0x00390570
	public void RemoveAt(int index)
	{
		if (index >= this.Count)
		{
			return;
		}
		this.Count--;
		if (index < this.Count)
		{
			Array.Copy(this.array, index + 1, this.array, index, this.Count - index);
		}
		this.array[this.Count] = default(T);
	}

	// Token: 0x06008ED1 RID: 36561 RVA: 0x003923D5 File Offset: 0x003905D5
	public bool Contains(T obj)
	{
		return Array.IndexOf<T>(this.array, obj, 0, this.Count) >= 0;
	}

	// Token: 0x06008ED2 RID: 36562 RVA: 0x003923F0 File Offset: 0x003905F0
	public void Set(T[] values)
	{
		this.array = values;
		this.length = values.Length;
		this.Count = this.length;
	}

	// Token: 0x06008ED3 RID: 36563 RVA: 0x0039240E File Offset: 0x0039060E
	public void Clear()
	{
		if (this.Count > 0)
		{
			Array.Clear(this.array, 0, this.array.Length);
		}
		this.Count = 0;
	}

	// Token: 0x06008ED4 RID: 36564 RVA: 0x00392434 File Offset: 0x00390634
	public T[] ToArray()
	{
		if (this.Count == 0)
		{
			return null;
		}
		T[] array = new T[this.Count];
		Array.Copy(this.array, array, this.Count);
		return array;
	}

	// Token: 0x06008ED5 RID: 36565 RVA: 0x0039246C File Offset: 0x0039066C
	public void CheckArray(int Size)
	{
		if (this.length - (this.Count + Size) <= 0)
		{
			this.length = ((this.DoubleSize == 0) ? (this.Count * 2 + Size) : (this.Count + this.DoubleSize + Size));
			T[] destinationArray = new T[this.length];
			Array.Copy(this.array, 0, destinationArray, 0, this.Count);
			this.array = destinationArray;
		}
	}

	// Token: 0x06008ED6 RID: 36566 RVA: 0x003924DB File Offset: 0x003906DB
	public void Sort(IComparer<T> comparer)
	{
		this.Sort(0, this.Count, comparer);
	}

	// Token: 0x06008ED7 RID: 36567 RVA: 0x003924EB File Offset: 0x003906EB
	public void Sort(int index, int count, IComparer<T> comparer)
	{
		if (this.length - index < count)
		{
			return;
		}
		Array.Sort<T>(this.array, index, count, comparer);
	}

	// Token: 0x17000ECD RID: 3789
	// (get) Token: 0x06008ED8 RID: 36568 RVA: 0x00392507 File Offset: 0x00390707
	public int Length
	{
		get
		{
			return this.Count;
		}
	}

	// Token: 0x04006E72 RID: 28274
	public T[] array;

	// Token: 0x04006E73 RID: 28275
	public int Count;

	// Token: 0x04006E74 RID: 28276
	public int length;

	// Token: 0x04006E75 RID: 28277
	public int DoubleSize;

	// Token: 0x04006E76 RID: 28278
	public bool IsValueType;
}
