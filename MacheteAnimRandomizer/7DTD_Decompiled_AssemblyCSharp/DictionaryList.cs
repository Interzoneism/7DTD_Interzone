using System;
using System.Collections.Generic;

// Token: 0x02001174 RID: 4468
public class DictionaryList<T, S>
{
	// Token: 0x06008B9E RID: 35742 RVA: 0x00384FD7 File Offset: 0x003831D7
	public DictionaryList()
	{
		this.dict = new Dictionary<T, S>();
	}

	// Token: 0x06008B9F RID: 35743 RVA: 0x00384FF5 File Offset: 0x003831F5
	public DictionaryList(IEqualityComparer<T> _comparer)
	{
		this.dict = new Dictionary<T, S>(_comparer);
	}

	// Token: 0x06008BA0 RID: 35744 RVA: 0x00385014 File Offset: 0x00383214
	public void Add(T _key, S _value)
	{
		this.dict.Add(_key, _value);
		this.list.Add(_value);
	}

	// Token: 0x06008BA1 RID: 35745 RVA: 0x0038502F File Offset: 0x0038322F
	public void Set(T _key, S _value)
	{
		if (this.dict.ContainsKey(_key))
		{
			this.Remove(_key);
		}
		this.Add(_key, _value);
	}

	// Token: 0x06008BA2 RID: 35746 RVA: 0x00385050 File Offset: 0x00383250
	public bool Remove(T _key)
	{
		if (this.dict.ContainsKey(_key))
		{
			S item = this.dict[_key];
			this.dict.Remove(_key);
			this.list.Remove(item);
			return true;
		}
		return false;
	}

	// Token: 0x06008BA3 RID: 35747 RVA: 0x00385095 File Offset: 0x00383295
	public void Clear()
	{
		this.list.Clear();
		this.dict.Clear();
	}

	// Token: 0x17000E8B RID: 3723
	// (get) Token: 0x06008BA4 RID: 35748 RVA: 0x003850AD File Offset: 0x003832AD
	public int Count
	{
		get
		{
			return this.list.Count;
		}
	}

	// Token: 0x04006D0F RID: 27919
	public Dictionary<T, S> dict;

	// Token: 0x04006D10 RID: 27920
	public List<S> list = new List<S>();
}
