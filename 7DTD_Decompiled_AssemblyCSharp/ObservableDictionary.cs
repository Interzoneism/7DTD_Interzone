using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x020011D7 RID: 4567
public class ObservableDictionary<TKey, TValue> : IDictionary<!0, !1>, ICollection<KeyValuePair<!0, !1>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
{
	// Token: 0x06008E94 RID: 36500 RVA: 0x003918DC File Offset: 0x0038FADC
	public ObservableDictionary() : this(new Dictionary<TKey, TValue>())
	{
	}

	// Token: 0x06008E95 RID: 36501 RVA: 0x003918E9 File Offset: 0x0038FAE9
	public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
	{
		this._dictionary = dictionary;
	}

	// Token: 0x140000F8 RID: 248
	// (add) Token: 0x06008E96 RID: 36502 RVA: 0x003918F8 File Offset: 0x0038FAF8
	// (remove) Token: 0x06008E97 RID: 36503 RVA: 0x00391930 File Offset: 0x0038FB30
	public event DictionaryAddEventHandler<TKey, TValue> EntryAdded;

	// Token: 0x140000F9 RID: 249
	// (add) Token: 0x06008E98 RID: 36504 RVA: 0x00391968 File Offset: 0x0038FB68
	// (remove) Token: 0x06008E99 RID: 36505 RVA: 0x003919A0 File Offset: 0x0038FBA0
	public event DictionaryRemoveEventHandler<TKey, TValue> EntryRemoved;

	// Token: 0x140000FA RID: 250
	// (add) Token: 0x06008E9A RID: 36506 RVA: 0x003919D8 File Offset: 0x0038FBD8
	// (remove) Token: 0x06008E9B RID: 36507 RVA: 0x00391A10 File Offset: 0x0038FC10
	public event DictionaryUpdatedValueEventHandler<TKey, TValue> EntryUpdatedValue;

	// Token: 0x140000FB RID: 251
	// (add) Token: 0x06008E9C RID: 36508 RVA: 0x00391A48 File Offset: 0x0038FC48
	// (remove) Token: 0x06008E9D RID: 36509 RVA: 0x00391A80 File Offset: 0x0038FC80
	public event DictionaryEntryModifiedEventHandler<TKey, TValue> EntryModified;

	// Token: 0x06008E9E RID: 36510 RVA: 0x00391AB5 File Offset: 0x0038FCB5
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEntryModified(TKey key, TValue value, string action)
	{
		DictionaryEntryModifiedEventHandler<TKey, TValue> entryModified = this.EntryModified;
		if (entryModified == null)
		{
			return;
		}
		entryModified(this, new DictionaryChangedEventArgs<TKey, TValue>(key, value, action));
	}

	// Token: 0x06008E9F RID: 36511 RVA: 0x00391AD0 File Offset: 0x0038FCD0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEntryAdded(TKey key, TValue value)
	{
		DictionaryAddEventHandler<TKey, TValue> entryAdded = this.EntryAdded;
		if (entryAdded != null)
		{
			entryAdded(this, new DictionaryChangedEventArgs<TKey, TValue>(key, value, "Added"));
		}
		this.OnEntryModified(key, value, "Added");
	}

	// Token: 0x06008EA0 RID: 36512 RVA: 0x00391AFD File Offset: 0x0038FCFD
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEntryRemoved(TKey key, TValue value)
	{
		DictionaryRemoveEventHandler<TKey, TValue> entryRemoved = this.EntryRemoved;
		if (entryRemoved != null)
		{
			entryRemoved(this, new DictionaryChangedEventArgs<TKey, TValue>(key, value, "Removed"));
		}
		this.OnEntryModified(key, value, "Removed");
	}

	// Token: 0x06008EA1 RID: 36513 RVA: 0x00391B2A File Offset: 0x0038FD2A
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEntryUpdated(TKey key, TValue value)
	{
		DictionaryUpdatedValueEventHandler<TKey, TValue> entryUpdatedValue = this.EntryUpdatedValue;
		if (entryUpdatedValue != null)
		{
			entryUpdatedValue(this, new DictionaryChangedEventArgs<TKey, TValue>(key, value, "Updated"));
		}
		this.OnEntryModified(key, value, "Updated");
	}

	// Token: 0x06008EA2 RID: 36514 RVA: 0x00391B57 File Offset: 0x0038FD57
	public void Add(TKey key, TValue value)
	{
		this._dictionary.Add(key, value);
		this.OnEntryAdded(key, value);
	}

	// Token: 0x06008EA3 RID: 36515 RVA: 0x00391B70 File Offset: 0x0038FD70
	public bool Remove(TKey key)
	{
		TValue value;
		if (this._dictionary.TryGetValue(key, out value) && this._dictionary.Remove(key))
		{
			this.OnEntryRemoved(key, value);
			return true;
		}
		return false;
	}

	// Token: 0x17000EC8 RID: 3784
	public TValue this[TKey key]
	{
		get
		{
			return this._dictionary[key];
		}
		set
		{
			if (this._dictionary.ContainsKey(key))
			{
				this._dictionary[key] = value;
				this.OnEntryUpdated(key, value);
				return;
			}
			this._dictionary[key] = value;
			this.OnEntryAdded(key, value);
		}
	}

	// Token: 0x17000EC9 RID: 3785
	// (get) Token: 0x06008EA6 RID: 36518 RVA: 0x00391BEF File Offset: 0x0038FDEF
	public ICollection<TKey> Keys
	{
		get
		{
			return this._dictionary.Keys;
		}
	}

	// Token: 0x17000ECA RID: 3786
	// (get) Token: 0x06008EA7 RID: 36519 RVA: 0x00391BFC File Offset: 0x0038FDFC
	public ICollection<TValue> Values
	{
		get
		{
			return this._dictionary.Values;
		}
	}

	// Token: 0x06008EA8 RID: 36520 RVA: 0x00391C09 File Offset: 0x0038FE09
	public bool ContainsKey(TKey key)
	{
		return this._dictionary.ContainsKey(key);
	}

	// Token: 0x06008EA9 RID: 36521 RVA: 0x00391C17 File Offset: 0x0038FE17
	public bool TryGetValue(TKey key, out TValue value)
	{
		return this._dictionary.TryGetValue(key, out value);
	}

	// Token: 0x06008EAA RID: 36522 RVA: 0x00391C26 File Offset: 0x0038FE26
	public void Add(KeyValuePair<TKey, TValue> item)
	{
		this.Add(item.Key, item.Value);
	}

	// Token: 0x06008EAB RID: 36523 RVA: 0x00391C3C File Offset: 0x0038FE3C
	public bool Remove(KeyValuePair<TKey, TValue> item)
	{
		return this.Remove(item.Key);
	}

	// Token: 0x06008EAC RID: 36524 RVA: 0x00391C4B File Offset: 0x0038FE4B
	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		return this._dictionary.ContainsKey(item.Key) && EqualityComparer<TValue>.Default.Equals(this._dictionary[item.Key], item.Value);
	}

	// Token: 0x06008EAD RID: 36525 RVA: 0x00391C86 File Offset: 0x0038FE86
	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		this._dictionary.CopyTo(array, arrayIndex);
	}

	// Token: 0x17000ECB RID: 3787
	// (get) Token: 0x06008EAE RID: 36526 RVA: 0x00391C95 File Offset: 0x0038FE95
	public int Count
	{
		get
		{
			return this._dictionary.Count;
		}
	}

	// Token: 0x17000ECC RID: 3788
	// (get) Token: 0x06008EAF RID: 36527 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public bool IsReadOnly
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06008EB0 RID: 36528 RVA: 0x00391CA4 File Offset: 0x0038FEA4
	public void Clear()
	{
		foreach (TKey key in new List<TKey>(this._dictionary.Keys))
		{
			this.Remove(key);
		}
	}

	// Token: 0x06008EB1 RID: 36529 RVA: 0x00391D04 File Offset: 0x0038FF04
	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return this._dictionary.GetEnumerator();
	}

	// Token: 0x06008EB2 RID: 36530 RVA: 0x00391D04 File Offset: 0x0038FF04
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator GetEnumerator()
	{
		return this._dictionary.GetEnumerator();
	}

	// Token: 0x04006E69 RID: 28265
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly IDictionary<TKey, TValue> _dictionary;
}
