using System;
using System.Collections.Generic;

// Token: 0x02000AC7 RID: 2759
public class UtilList<T> where T : class
{
	// Token: 0x17000866 RID: 2150
	// (get) Token: 0x060054EE RID: 21742 RVA: 0x0022B456 File Offset: 0x00229656
	public int Capacity
	{
		get
		{
			return this.capacity;
		}
	}

	// Token: 0x17000867 RID: 2151
	// (get) Token: 0x060054EF RID: 21743 RVA: 0x0022B45E File Offset: 0x0022965E
	public int Count
	{
		get
		{
			return this.count;
		}
	}

	// Token: 0x060054F0 RID: 21744 RVA: 0x0022B468 File Offset: 0x00229668
	public UtilList(int _capacity, T _NullValue)
	{
		this.capacity = _capacity;
		this.StartId = 0;
		this.EndId = 0;
		this.count = 0;
		this.IsStandardState = true;
		this.IsFull = false;
		this.InternArray = new T[_capacity];
		this.NullValue = _NullValue;
	}

	// Token: 0x060054F1 RID: 21745 RVA: 0x0022B4B8 File Offset: 0x002296B8
	public void Clear()
	{
		this.StartId = 0;
		this.EndId = 0;
		this.count = 0;
		this.IsFull = false;
		this.IsStandardState = true;
	}

	// Token: 0x060054F2 RID: 21746 RVA: 0x0022B4E0 File Offset: 0x002296E0
	public void Add(T NewItem)
	{
		if (this.IsFull)
		{
			throw new ArgumentException("Overflow : UtilList is full");
		}
		this.InternArray[this.EndId] = NewItem;
		int num = this.EndId + 1;
		this.EndId = num;
		if (num == this.capacity)
		{
			this.EndId = 0;
			this.IsStandardState = false;
		}
		if (this.EndId == this.StartId)
		{
			this.IsFull = true;
		}
		this.count++;
	}

	// Token: 0x060054F3 RID: 21747 RVA: 0x0022B55C File Offset: 0x0022975C
	public T Peek()
	{
		return this.InternArray[this.StartId];
	}

	// Token: 0x060054F4 RID: 21748 RVA: 0x0022B570 File Offset: 0x00229770
	public T Dequeue()
	{
		if (this.StartId == this.EndId && !this.IsFull)
		{
			throw new ArgumentException("UtilList is EMPTY");
		}
		int startId = this.StartId;
		this.StartId++;
		if (this.StartId == this.capacity)
		{
			this.StartId = 0;
			this.IsStandardState = true;
		}
		this.IsFull = false;
		this.count--;
		return this.InternArray[startId];
	}

	// Token: 0x060054F5 RID: 21749 RVA: 0x0022B5F0 File Offset: 0x002297F0
	public void RemoveNFirst(int N)
	{
		if (this.count < N)
		{
			throw new ArgumentException("UtilList Out of range");
		}
		this.StartId += N;
		if (this.StartId >= this.capacity)
		{
			this.StartId -= this.capacity;
			this.IsStandardState = true;
		}
		this.count -= N;
		this.IsFull = (this.count == this.capacity);
	}

	// Token: 0x060054F6 RID: 21750 RVA: 0x0022B66C File Offset: 0x0022986C
	public void RemoveAt(List<int> IdList)
	{
		int num = IdList.Count;
		for (int i = 0; i < num; i++)
		{
			this[IdList[i]] = this.NullValue;
		}
		num = 0;
		for (int j = 0; j < this.Count; j++)
		{
			if (this[j] != null)
			{
				this[num++] = this[j];
			}
		}
		this.count = num;
		this.EndId = this.StartId + this.count;
		if (this.EndId >= this.capacity)
		{
			this.EndId -= this.capacity;
			this.IsStandardState = false;
		}
		else
		{
			this.IsStandardState = true;
		}
		this.IsFull = (this.count == this.capacity);
	}

	// Token: 0x17000868 RID: 2152
	public T this[int Id]
	{
		get
		{
			if (this.Count == 0)
			{
				throw new ArgumentException("UtilList is EMPTY");
			}
			int num = this.StartId + Id;
			if (this.IsStandardState)
			{
				if (num >= this.EndId)
				{
					throw new ArgumentException("UtilList index is out of range");
				}
			}
			else if (num >= this.capacity)
			{
				num -= this.capacity;
				if (num >= this.EndId)
				{
					throw new ArgumentException("UtilList Index is out of range");
				}
			}
			return this.InternArray[num];
		}
		set
		{
			if (this.Count == 0)
			{
				throw new ArgumentException("UtilList is EMPTY");
			}
			int num = this.StartId + Id;
			if (this.IsStandardState)
			{
				if (num >= this.EndId)
				{
					throw new ArgumentException("UtilList : Index is out of range");
				}
			}
			else if (num >= this.capacity)
			{
				num -= this.capacity;
				if (num >= this.EndId)
				{
					throw new ArgumentException("UtilList : Index is out of range");
				}
			}
			this.InternArray[num] = value;
		}
	}

	// Token: 0x040041C7 RID: 16839
	[PublicizedFrom(EAccessModifier.Private)]
	public int capacity;

	// Token: 0x040041C8 RID: 16840
	[PublicizedFrom(EAccessModifier.Private)]
	public int StartId;

	// Token: 0x040041C9 RID: 16841
	[PublicizedFrom(EAccessModifier.Private)]
	public int EndId;

	// Token: 0x040041CA RID: 16842
	[PublicizedFrom(EAccessModifier.Private)]
	public int count;

	// Token: 0x040041CB RID: 16843
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsStandardState;

	// Token: 0x040041CC RID: 16844
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsFull;

	// Token: 0x040041CD RID: 16845
	[PublicizedFrom(EAccessModifier.Private)]
	public T[] InternArray;

	// Token: 0x040041CE RID: 16846
	[PublicizedFrom(EAccessModifier.Private)]
	public T NullValue;
}
