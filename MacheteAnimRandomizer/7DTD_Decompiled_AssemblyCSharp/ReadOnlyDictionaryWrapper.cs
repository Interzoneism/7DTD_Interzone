using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x020011FA RID: 4602
public class ReadOnlyDictionaryWrapper<TKey, TValueIn, TValueOut> : IReadOnlyDictionary<TKey, TValueOut>, IEnumerable<KeyValuePair<TKey, TValueOut>>, IEnumerable, IReadOnlyCollection<KeyValuePair<TKey, TValueOut>> where TValueIn : TValueOut
{
	// Token: 0x06008FB6 RID: 36790 RVA: 0x00395D18 File Offset: 0x00393F18
	public ReadOnlyDictionaryWrapper(IReadOnlyDictionary<TKey, TValueIn> dict)
	{
		this.m_dict = dict;
	}

	// Token: 0x06008FB7 RID: 36791 RVA: 0x00395D27 File Offset: 0x00393F27
	public IEnumerator<KeyValuePair<TKey, TValueOut>> GetEnumerator()
	{
		foreach (KeyValuePair<TKey, TValueIn> keyValuePair in this.m_dict)
		{
			TKey tkey;
			TValueIn tvalueIn;
			keyValuePair.Deconstruct(out tkey, out tvalueIn);
			TKey key = tkey;
			TValueIn tvalueIn2 = tvalueIn;
			yield return new KeyValuePair<TKey, TValueOut>(key, (TValueOut)((object)tvalueIn2));
		}
		IEnumerator<KeyValuePair<TKey, TValueIn>> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x06008FB8 RID: 36792 RVA: 0x00395D36 File Offset: 0x00393F36
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator GetEnumerator()
	{
		return this.m_dict.GetEnumerator();
	}

	// Token: 0x17000EDB RID: 3803
	// (get) Token: 0x06008FB9 RID: 36793 RVA: 0x00395D43 File Offset: 0x00393F43
	public int Count
	{
		get
		{
			return this.m_dict.Count;
		}
	}

	// Token: 0x06008FBA RID: 36794 RVA: 0x00395D50 File Offset: 0x00393F50
	public bool ContainsKey(TKey key)
	{
		return this.m_dict.ContainsKey(key);
	}

	// Token: 0x06008FBB RID: 36795 RVA: 0x00395D60 File Offset: 0x00393F60
	public bool TryGetValue(TKey key, out TValueOut value)
	{
		TValueIn tvalueIn;
		bool result = this.m_dict.TryGetValue(key, out tvalueIn);
		value = (TValueOut)((object)tvalueIn);
		return result;
	}

	// Token: 0x17000EDC RID: 3804
	public TValueOut this[TKey key]
	{
		get
		{
			return (TValueOut)((object)this.m_dict[key]);
		}
	}

	// Token: 0x17000EDD RID: 3805
	// (get) Token: 0x06008FBD RID: 36797 RVA: 0x00395DA4 File Offset: 0x00393FA4
	public IEnumerable<TKey> Keys
	{
		get
		{
			return this.m_dict.Keys;
		}
	}

	// Token: 0x17000EDE RID: 3806
	// (get) Token: 0x06008FBE RID: 36798 RVA: 0x00395DB1 File Offset: 0x00393FB1
	public IEnumerable<TValueOut> Values
	{
		get
		{
			foreach (TValueIn tvalueIn in this.m_dict.Values)
			{
				yield return (TValueOut)((object)tvalueIn);
			}
			IEnumerator<TValueIn> enumerator = null;
			yield break;
			yield break;
		}
	}

	// Token: 0x04006ECE RID: 28366
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly IReadOnlyDictionary<TKey, TValueIn> m_dict;
}
