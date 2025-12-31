using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x02001167 RID: 4455
public sealed class DictionaryDebugWrapper<TKey, TValue> : CollectionDebugWrapper<KeyValuePair<TKey, TValue>>, IDictionary<!0, !1>, ICollection<KeyValuePair<!0, !1>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
{
	// Token: 0x06008B5F RID: 35679 RVA: 0x0038443A File Offset: 0x0038263A
	public DictionaryDebugWrapper() : this(new Dictionary<TKey, TValue>())
	{
	}

	// Token: 0x06008B60 RID: 35680 RVA: 0x00384447 File Offset: 0x00382647
	public DictionaryDebugWrapper(IDictionary<TKey, TValue> dictionary) : this(null, dictionary)
	{
	}

	// Token: 0x06008B61 RID: 35681 RVA: 0x00384451 File Offset: 0x00382651
	public DictionaryDebugWrapper(DebugWrapper parent, IDictionary<TKey, TValue> dictionary) : base(parent, dictionary)
	{
		this.m_dictionary = dictionary;
		this.Keys = new CollectionDebugWrapper<TKey>(this, this.m_dictionary.Keys);
		this.Values = new CollectionDebugWrapper<TValue>(this, this.m_dictionary.Values);
	}

	// Token: 0x06008B62 RID: 35682 RVA: 0x00384490 File Offset: 0x00382690
	public void Add(TKey key, TValue value)
	{
		using (base.DebugReadWriteScope())
		{
			this.m_dictionary.Add(key, value);
		}
	}

	// Token: 0x06008B63 RID: 35683 RVA: 0x003844D0 File Offset: 0x003826D0
	public bool ContainsKey(TKey key)
	{
		bool result;
		using (base.DebugReadScope())
		{
			result = this.m_dictionary.ContainsKey(key);
		}
		return result;
	}

	// Token: 0x06008B64 RID: 35684 RVA: 0x00384510 File Offset: 0x00382710
	public bool Remove(TKey key)
	{
		bool result;
		using (base.DebugReadWriteScope())
		{
			result = this.m_dictionary.Remove(key);
		}
		return result;
	}

	// Token: 0x06008B65 RID: 35685 RVA: 0x00384550 File Offset: 0x00382750
	public bool TryGetValue(TKey key, out TValue value)
	{
		bool result;
		using (base.DebugReadScope())
		{
			result = this.m_dictionary.TryGetValue(key, out value);
		}
		return result;
	}

	// Token: 0x17000E86 RID: 3718
	public TValue this[TKey key]
	{
		get
		{
			TValue result;
			using (base.DebugReadScope())
			{
				result = this.m_dictionary[key];
			}
			return result;
		}
		set
		{
			using (base.DebugReadWriteScope())
			{
				this.m_dictionary[key] = value;
			}
		}
	}

	// Token: 0x17000E87 RID: 3719
	// (get) Token: 0x06008B68 RID: 35688 RVA: 0x00384610 File Offset: 0x00382810
	public ICollection<TKey> Keys { get; }

	// Token: 0x17000E88 RID: 3720
	// (get) Token: 0x06008B69 RID: 35689 RVA: 0x00384618 File Offset: 0x00382818
	public ICollection<TValue> Values { get; }

	// Token: 0x04006CF6 RID: 27894
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly IDictionary<TKey, TValue> m_dictionary;
}
