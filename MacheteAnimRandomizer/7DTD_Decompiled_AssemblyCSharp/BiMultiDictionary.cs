using System;
using System.Collections.Generic;

// Token: 0x02001134 RID: 4404
public sealed class BiMultiDictionary<TKey, TValue>
{
	// Token: 0x06008A64 RID: 35428 RVA: 0x0037FFAB File Offset: 0x0037E1AB
	public BiMultiDictionary()
	{
		this.m_keyToValues = new Dictionary<TKey, HashSet<TValue>>();
		this.m_valueToKey = new Dictionary<TValue, TKey>();
	}

	// Token: 0x06008A65 RID: 35429 RVA: 0x0037FFCC File Offset: 0x0037E1CC
	public void Add(TKey key, TValue value)
	{
		if (this.m_valueToKey.ContainsKey(value))
		{
			throw new ArgumentException("Value already in dictionary.", "value");
		}
		HashSet<TValue> hashSet;
		if (!this.m_keyToValues.TryGetValue(key, out hashSet))
		{
			this.m_keyToValues.Add(key, hashSet = new HashSet<TValue>());
		}
		hashSet.Add(value);
		this.m_valueToKey.Add(value, key);
	}

	// Token: 0x06008A66 RID: 35430 RVA: 0x0038002F File Offset: 0x0037E22F
	public bool ContainsKey(TKey key)
	{
		return this.m_keyToValues.ContainsKey(key);
	}

	// Token: 0x06008A67 RID: 35431 RVA: 0x0038003D File Offset: 0x0037E23D
	public bool ContainsValue(TValue value)
	{
		return this.m_valueToKey.ContainsKey(value);
	}

	// Token: 0x06008A68 RID: 35432 RVA: 0x0038004C File Offset: 0x0037E24C
	public bool TryGetByKey(TKey key, out IReadOnlyCollection<TValue> values)
	{
		HashSet<TValue> hashSet;
		if (!this.m_keyToValues.TryGetValue(key, out hashSet))
		{
			values = null;
			return false;
		}
		values = hashSet;
		return true;
	}

	// Token: 0x06008A69 RID: 35433 RVA: 0x00380074 File Offset: 0x0037E274
	public int TryGetByKey(TKey key, ICollection<TValue> valuesOut)
	{
		HashSet<TValue> hashSet;
		if (!this.m_keyToValues.TryGetValue(key, out hashSet))
		{
			return 0;
		}
		int num = 0;
		foreach (TValue item in hashSet)
		{
			valuesOut.Add(item);
			num++;
		}
		return num;
	}

	// Token: 0x06008A6A RID: 35434 RVA: 0x003800DC File Offset: 0x0037E2DC
	public unsafe int TryGetByKey(TKey key, Span<TValue> valuesOut)
	{
		HashSet<TValue> hashSet;
		if (!this.m_keyToValues.TryGetValue(key, out hashSet))
		{
			return 0;
		}
		int num = 0;
		foreach (TValue tvalue in hashSet)
		{
			if (num >= valuesOut.Length)
			{
				break;
			}
			*valuesOut[num] = tvalue;
			num++;
		}
		return num;
	}

	// Token: 0x06008A6B RID: 35435 RVA: 0x00380158 File Offset: 0x0037E358
	public bool TryGetByValue(TValue value, out TKey key)
	{
		return this.m_valueToKey.TryGetValue(value, out key);
	}

	// Token: 0x06008A6C RID: 35436 RVA: 0x00380168 File Offset: 0x0037E368
	public bool RemoveByValue(TValue value)
	{
		TKey key;
		if (!this.m_valueToKey.TryGetValue(value, out key))
		{
			return false;
		}
		HashSet<TValue> hashSet;
		if (!this.m_keyToValues.TryGetValue(key, out hashSet))
		{
			return false;
		}
		this.m_valueToKey.Remove(value);
		hashSet.Remove(value);
		if (hashSet.Count == 0)
		{
			this.m_keyToValues.Remove(key);
		}
		return true;
	}

	// Token: 0x04006C45 RID: 27717
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<TKey, HashSet<TValue>> m_keyToValues;

	// Token: 0x04006C46 RID: 27718
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<TValue, TKey> m_valueToKey;
}
