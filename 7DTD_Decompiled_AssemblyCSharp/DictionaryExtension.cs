using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x0200116E RID: 4462
public static class DictionaryExtension
{
	// Token: 0x06008B82 RID: 35714 RVA: 0x003849FC File Offset: 0x00382BFC
	public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dic, Func<TValue, bool> predicate)
	{
		foreach (TKey key in (from k in dic.Keys
		where predicate(dic[k])
		select k).ToList<TKey>())
		{
			dic.Remove(key);
		}
	}

	// Token: 0x06008B83 RID: 35715 RVA: 0x00384A84 File Offset: 0x00382C84
	public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dic, Func<TKey, bool> predicate)
	{
		foreach (TKey key in (from k in dic.Keys
		where predicate(k)
		select k).ToList<TKey>())
		{
			dic.Remove(key);
		}
	}

	// Token: 0x06008B84 RID: 35716 RVA: 0x00384AFC File Offset: 0x00382CFC
	public static void CopyTo<TKey, TValue>(this IDictionary<TKey, TValue> _src, IDictionary<TKey, TValue> _dest, bool _overwriteExisting = false)
	{
		if (_overwriteExisting)
		{
			using (IEnumerator<KeyValuePair<TKey, TValue>> enumerator = _src.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<TKey, TValue> keyValuePair = enumerator.Current;
					_dest[keyValuePair.Key] = keyValuePair.Value;
				}
				return;
			}
		}
		foreach (KeyValuePair<TKey, TValue> keyValuePair2 in _src)
		{
			_dest.Add(keyValuePair2.Key, keyValuePair2.Value);
		}
	}

	// Token: 0x06008B85 RID: 35717 RVA: 0x00384B98 File Offset: 0x00382D98
	public static void CopyKeysTo<TKey, TValue>(this IDictionary<TKey, TValue> _src, ICollection<TKey> _dest)
	{
		foreach (KeyValuePair<TKey, TValue> keyValuePair in _src)
		{
			_dest.Add(keyValuePair.Key);
		}
	}

	// Token: 0x06008B86 RID: 35718 RVA: 0x00384BE8 File Offset: 0x00382DE8
	public static void CopyKeysTo<TKey, TValue>(this IDictionary<TKey, TValue> _src, TKey[] _dest)
	{
		if (_dest.Length != _src.Count)
		{
			throw new ArgumentOutOfRangeException("_dest", "Target array does not have the same size as the dictionary");
		}
		int num = 0;
		foreach (KeyValuePair<TKey, TValue> keyValuePair in _src)
		{
			_dest[num++] = keyValuePair.Key;
		}
	}

	// Token: 0x06008B87 RID: 35719 RVA: 0x00384C58 File Offset: 0x00382E58
	public static void CopyValuesTo<TKey, TValue>(this IDictionary<TKey, TValue> _src, IList<TValue> _dest)
	{
		foreach (KeyValuePair<TKey, TValue> keyValuePair in _src)
		{
			_dest.Add(keyValuePair.Value);
		}
	}

	// Token: 0x06008B88 RID: 35720 RVA: 0x00384CA8 File Offset: 0x00382EA8
	public static void CopyValuesTo<TKey, TValue>(this IDictionary<TKey, TValue> _src, TValue[] _dest)
	{
		if (_dest.Length != _src.Count)
		{
			throw new ArgumentOutOfRangeException("_dest", "Target array does not have the same size as the dictionary");
		}
		int num = 0;
		foreach (KeyValuePair<TKey, TValue> keyValuePair in _src)
		{
			_dest[num++] = keyValuePair.Value;
		}
	}
}
