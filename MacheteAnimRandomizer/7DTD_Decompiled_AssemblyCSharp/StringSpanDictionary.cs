using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x0200122D RID: 4653
public class StringSpanDictionary<T> : IDictionary<string, T>, ICollection<KeyValuePair<string, T>>, IEnumerable<KeyValuePair<string, T>>, IEnumerable, IReadOnlyDictionary<string, T>, IReadOnlyCollection<KeyValuePair<string, T>>
{
	// Token: 0x0600914B RID: 37195 RVA: 0x0039EEA4 File Offset: 0x0039D0A4
	public StringSpanDictionary() : this(new Dictionary<string, T>())
	{
	}

	// Token: 0x0600914C RID: 37196 RVA: 0x0039EEB1 File Offset: 0x0039D0B1
	public StringSpanDictionary(IDictionary<string, T> dict)
	{
		this.m_dict = dict;
		this.m_hashToKeys = new Dictionary<int, List<string>>();
	}

	// Token: 0x0600914D RID: 37197 RVA: 0x0039EECB File Offset: 0x0039D0CB
	[PublicizedFrom(EAccessModifier.Private)]
	public static int GenerateHash(StringSpan key)
	{
		return key.GetHashCode();
	}

	// Token: 0x0600914E RID: 37198 RVA: 0x0039EEDC File Offset: 0x0039D0DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddHash(string key)
	{
		int key2 = StringSpanDictionary<T>.GenerateHash(key);
		List<string> list;
		if (!this.m_hashToKeys.TryGetValue(key2, out list))
		{
			list = new List<string>();
			this.m_hashToKeys.Add(key2, list);
		}
		using (List<string>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == key)
				{
					return;
				}
			}
		}
		list.Add(key);
	}

	// Token: 0x0600914F RID: 37199 RVA: 0x0039EF64 File Offset: 0x0039D164
	[PublicizedFrom(EAccessModifier.Private)]
	public void RemoveHash(string key)
	{
		int key2 = StringSpanDictionary<T>.GenerateHash(key);
		List<string> list;
		if (!this.m_hashToKeys.TryGetValue(key2, out list))
		{
			return;
		}
		list.Remove(key);
		if (list.Count > 0)
		{
			return;
		}
		this.m_hashToKeys.Remove(key2);
	}

	// Token: 0x06009150 RID: 37200 RVA: 0x0039EFB0 File Offset: 0x0039D1B0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryGetStringFromHashedKeys(StringSpan spanKey, out string stringKey)
	{
		int key = StringSpanDictionary<T>.GenerateHash(spanKey);
		List<string> list;
		if (!this.m_hashToKeys.TryGetValue(key, out list))
		{
			stringKey = null;
			return false;
		}
		foreach (string text in list)
		{
			if (!(text != spanKey))
			{
				stringKey = text;
				return true;
			}
		}
		stringKey = null;
		return false;
	}

	// Token: 0x06009151 RID: 37201 RVA: 0x0039F030 File Offset: 0x0039D230
	public void Add(StringSpan key, T value)
	{
		string key2;
		if (!this.TryGetStringFromHashedKeys(key, out key2))
		{
			this.Add(key.ToString(), value);
			return;
		}
		this.Add(key2, value);
	}

	// Token: 0x06009152 RID: 37202 RVA: 0x0039F068 File Offset: 0x0039D268
	public bool ContainsKey(StringSpan key)
	{
		string key2;
		return this.TryGetStringFromHashedKeys(key, out key2) && this.ContainsKey(key2);
	}

	// Token: 0x06009153 RID: 37203 RVA: 0x0039F08C File Offset: 0x0039D28C
	public bool Remove(StringSpan key)
	{
		string key2;
		return this.TryGetStringFromHashedKeys(key, out key2) && this.Remove(key2);
	}

	// Token: 0x06009154 RID: 37204 RVA: 0x0039F0B0 File Offset: 0x0039D2B0
	public bool TryGetValue(StringSpan key, out T value)
	{
		string key2;
		if (!this.TryGetStringFromHashedKeys(key, out key2))
		{
			value = default(T);
			return false;
		}
		return this.TryGetValue(key2, out value);
	}

	// Token: 0x17000F04 RID: 3844
	public T this[StringSpan key]
	{
		get
		{
			string key2;
			if (!this.TryGetStringFromHashedKeys(key, out key2))
			{
				throw new KeyNotFoundException();
			}
			return this[key2];
		}
		set
		{
			string key2;
			if (!this.TryGetStringFromHashedKeys(key, out key2))
			{
				this[key.ToString()] = value;
				return;
			}
			this[key2] = value;
		}
	}

	// Token: 0x06009157 RID: 37207 RVA: 0x0039F139 File Offset: 0x0039D339
	public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
	{
		return this.m_dict.GetEnumerator();
	}

	// Token: 0x06009158 RID: 37208 RVA: 0x0039F139 File Offset: 0x0039D339
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator GetEnumerator()
	{
		return this.m_dict.GetEnumerator();
	}

	// Token: 0x06009159 RID: 37209 RVA: 0x0039F146 File Offset: 0x0039D346
	[PublicizedFrom(EAccessModifier.Private)]
	public void Add(KeyValuePair<string, T> item)
	{
		this.Add(item.Key, item.Value);
	}

	// Token: 0x0600915A RID: 37210 RVA: 0x0039F15C File Offset: 0x0039D35C
	public void Clear()
	{
		this.m_dict.Clear();
		this.m_hashToKeys.Clear();
	}

	// Token: 0x0600915B RID: 37211 RVA: 0x0039F174 File Offset: 0x0039D374
	public bool Contains(KeyValuePair<string, T> item)
	{
		return this.m_dict.Contains(item);
	}

	// Token: 0x0600915C RID: 37212 RVA: 0x0039F182 File Offset: 0x0039D382
	[PublicizedFrom(EAccessModifier.Private)]
	public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
	{
		this.m_dict.CopyTo(array, arrayIndex);
	}

	// Token: 0x0600915D RID: 37213 RVA: 0x0039F191 File Offset: 0x0039D391
	[PublicizedFrom(EAccessModifier.Private)]
	public bool Remove(KeyValuePair<string, T> item)
	{
		bool result = this.m_dict.Remove(item);
		if (!this.m_dict.ContainsKey(item.Key))
		{
			this.RemoveHash(item.Key);
		}
		return result;
	}

	// Token: 0x17000F05 RID: 3845
	// (get) Token: 0x0600915E RID: 37214 RVA: 0x0039F1C0 File Offset: 0x0039D3C0
	public int Count
	{
		get
		{
			return this.m_dict.Count;
		}
	}

	// Token: 0x17000F06 RID: 3846
	// (get) Token: 0x0600915F RID: 37215 RVA: 0x0039F1CD File Offset: 0x0039D3CD
	public bool IsReadOnly
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.m_dict.IsReadOnly;
		}
	}

	// Token: 0x06009160 RID: 37216 RVA: 0x0039F1DA File Offset: 0x0039D3DA
	public void Add(string key, T value)
	{
		this.m_dict.Add(key, value);
		this.AddHash(key);
	}

	// Token: 0x06009161 RID: 37217 RVA: 0x0039F1F0 File Offset: 0x0039D3F0
	public bool ContainsKey(string key)
	{
		return this.m_dict.ContainsKey(key);
	}

	// Token: 0x06009162 RID: 37218 RVA: 0x0039F1FE File Offset: 0x0039D3FE
	public bool Remove(string key)
	{
		bool result = this.m_dict.Remove(key);
		if (!this.m_dict.ContainsKey(key))
		{
			this.RemoveHash(key);
		}
		return result;
	}

	// Token: 0x06009163 RID: 37219 RVA: 0x0039F221 File Offset: 0x0039D421
	public bool TryGetValue(string key, out T value)
	{
		return this.m_dict.TryGetValue(key, out value);
	}

	// Token: 0x17000F07 RID: 3847
	public T this[string key]
	{
		get
		{
			return this.m_dict[key];
		}
		set
		{
			this.m_dict[key] = value;
			this.AddHash(key);
		}
	}

	// Token: 0x17000F08 RID: 3848
	// (get) Token: 0x06009166 RID: 37222 RVA: 0x0039F254 File Offset: 0x0039D454
	public ICollection<string> Keys
	{
		get
		{
			return this.m_dict.Keys;
		}
	}

	// Token: 0x17000F09 RID: 3849
	// (get) Token: 0x06009167 RID: 37223 RVA: 0x0039F261 File Offset: 0x0039D461
	public ICollection<T> Values
	{
		get
		{
			return this.m_dict.Values;
		}
	}

	// Token: 0x17000F0A RID: 3850
	// (get) Token: 0x06009168 RID: 37224 RVA: 0x0039F254 File Offset: 0x0039D454
	public IEnumerable<string> Keys
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.m_dict.Keys;
		}
	}

	// Token: 0x17000F0B RID: 3851
	// (get) Token: 0x06009169 RID: 37225 RVA: 0x0039F261 File Offset: 0x0039D461
	public IEnumerable<T> Values
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.m_dict.Values;
		}
	}

	// Token: 0x04006F8C RID: 28556
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly IDictionary<string, T> m_dict;

	// Token: 0x04006F8D RID: 28557
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<int, List<string>> m_hashToKeys;
}
