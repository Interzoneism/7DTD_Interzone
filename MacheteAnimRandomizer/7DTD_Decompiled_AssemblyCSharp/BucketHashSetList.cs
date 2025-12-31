using System;
using System.Collections.Generic;

// Token: 0x02001140 RID: 4416
public class BucketHashSetList
{
	// Token: 0x06008AA7 RID: 35495 RVA: 0x0038165F File Offset: 0x0037F85F
	public BucketHashSetList()
	{
		this.buckets = new OptimizedList<HashSetLong>(4);
	}

	// Token: 0x06008AA8 RID: 35496 RVA: 0x0038168C File Offset: 0x0037F88C
	public BucketHashSetList(int _noBuckets)
	{
		this.buckets = new OptimizedList<HashSetLong>(_noBuckets);
		for (int i = 0; i < _noBuckets; i++)
		{
			this.buckets.Add(new HashSetLong());
		}
	}

	// Token: 0x06008AA9 RID: 35497 RVA: 0x003816DD File Offset: 0x0037F8DD
	public void Add(int _bucketIdx, long _value)
	{
		this.buckets.array[_bucketIdx].Add(_value);
	}

	// Token: 0x06008AAA RID: 35498 RVA: 0x003816F3 File Offset: 0x0037F8F3
	public void Add(int _bucketIdx, HashSetLong _otherBucket)
	{
		this.buckets.array[_bucketIdx].UnionWithHashSetLong(_otherBucket);
	}

	// Token: 0x06008AAB RID: 35499 RVA: 0x00381708 File Offset: 0x0037F908
	public bool Contains(long _value)
	{
		if (!this.IsRecalc)
		{
			return false;
		}
		for (int i = 0; i < this.buckets.Count; i++)
		{
			if (this.buckets.array[i].Contains(_value))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06008AAC RID: 35500 RVA: 0x00381750 File Offset: 0x0037F950
	public void Remove(long _value)
	{
		this.list.Remove(_value);
		for (int i = 0; i < this.buckets.Count; i++)
		{
			if (this.buckets.array[i].Contains(_value))
			{
				this.buckets.array[i].Remove(_value);
				return;
			}
		}
	}

	// Token: 0x06008AAD RID: 35501 RVA: 0x003817AC File Offset: 0x0037F9AC
	public void Clear()
	{
		for (int i = 0; i < this.buckets.Count; i++)
		{
			this.buckets.array[i].Clear();
		}
		this.list.Clear();
		this.IsRecalc = false;
	}

	// Token: 0x06008AAE RID: 35502 RVA: 0x003817F4 File Offset: 0x0037F9F4
	public void ExceptTarget(HashSetLong hash)
	{
		if (!this.IsRecalc)
		{
			return;
		}
		for (int i = 0; i < this.buckets.Count; i++)
		{
			hash.ExceptWithHashSetLong(this.buckets.array[i]);
		}
	}

	// Token: 0x06008AAF RID: 35503 RVA: 0x00381834 File Offset: 0x0037FA34
	public void RecalcHashSetList()
	{
		this.list.Clear();
		this.elementsInList.Clear();
		this.IsRecalc = true;
		for (int i = 0; i < this.buckets.Count; i++)
		{
			foreach (long item in this.buckets.array[i])
			{
				if (this.elementsInList.Add(item))
				{
					this.list.Add(item);
				}
			}
		}
	}

	// Token: 0x06008AB0 RID: 35504 RVA: 0x003818D4 File Offset: 0x0037FAD4
	public BucketHashSetList Clone()
	{
		BucketHashSetList bucketHashSetList = new BucketHashSetList(this.buckets.Count);
		for (int i = 0; i < this.buckets.Count; i++)
		{
			bucketHashSetList.buckets.array[i].UnionWithHashSetLong(this.buckets.array[i]);
		}
		return bucketHashSetList;
	}

	// Token: 0x04006C6D RID: 27757
	public List<long> list = new List<long>();

	// Token: 0x04006C6E RID: 27758
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<long> elementsInList = new HashSet<long>();

	// Token: 0x04006C6F RID: 27759
	public OptimizedList<HashSetLong> buckets;

	// Token: 0x04006C70 RID: 27760
	public bool IsRecalc;
}
