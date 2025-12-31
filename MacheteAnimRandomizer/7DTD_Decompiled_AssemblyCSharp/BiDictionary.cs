using System;
using System.Collections.Generic;

// Token: 0x02001133 RID: 4403
public sealed class BiDictionary<TKey, TValue>
{
	// Token: 0x06008A5C RID: 35420 RVA: 0x0037FE90 File Offset: 0x0037E090
	public void Add(TKey key, TValue value)
	{
		if (this.m_keyToValue.ContainsKey(key))
		{
			throw new ArgumentException("Key already in dictionary.", "key");
		}
		if (this.m_valueToKey.ContainsKey(value))
		{
			throw new ArgumentException("Value already in dictionary.", "value");
		}
		this.m_keyToValue.Add(key, value);
		this.m_valueToKey.Add(value, key);
	}

	// Token: 0x06008A5D RID: 35421 RVA: 0x0037FEF3 File Offset: 0x0037E0F3
	public bool ContainsKey(TKey key)
	{
		return this.m_keyToValue.ContainsKey(key);
	}

	// Token: 0x06008A5E RID: 35422 RVA: 0x0037FF01 File Offset: 0x0037E101
	public bool ContainsValue(TValue value)
	{
		return this.m_valueToKey.ContainsKey(value);
	}

	// Token: 0x06008A5F RID: 35423 RVA: 0x0037FF0F File Offset: 0x0037E10F
	public bool TryGetByKey(TKey key, out TValue value)
	{
		return this.m_keyToValue.TryGetValue(key, out value);
	}

	// Token: 0x06008A60 RID: 35424 RVA: 0x0037FF1E File Offset: 0x0037E11E
	public bool TryGetByValue(TValue value, out TKey key)
	{
		return this.m_valueToKey.TryGetValue(value, out key);
	}

	// Token: 0x06008A61 RID: 35425 RVA: 0x0037FF30 File Offset: 0x0037E130
	public bool RemoveByKey(TKey key)
	{
		TValue key2;
		if (!this.m_keyToValue.Remove(key, out key2))
		{
			return false;
		}
		this.m_valueToKey.Remove(key2);
		return true;
	}

	// Token: 0x06008A62 RID: 35426 RVA: 0x0037FF60 File Offset: 0x0037E160
	public bool RemoveByValue(TValue value)
	{
		TKey key;
		if (!this.m_valueToKey.Remove(value, out key))
		{
			return false;
		}
		this.m_keyToValue.Remove(key);
		return true;
	}

	// Token: 0x04006C43 RID: 27715
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<TKey, TValue> m_keyToValue = new Dictionary<TKey, TValue>();

	// Token: 0x04006C44 RID: 27716
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<TValue, TKey> m_valueToKey = new Dictionary<TValue, TKey>();
}
