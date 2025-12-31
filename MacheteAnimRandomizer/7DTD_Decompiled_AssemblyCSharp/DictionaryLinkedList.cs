using System;
using System.Collections.Generic;

// Token: 0x02001173 RID: 4467
public class DictionaryLinkedList<T, S>
{
	// Token: 0x06008B97 RID: 35735 RVA: 0x00384EA2 File Offset: 0x003830A2
	public DictionaryLinkedList()
	{
		this.dict = new Dictionary<T, S>();
	}

	// Token: 0x06008B98 RID: 35736 RVA: 0x00384ECB File Offset: 0x003830CB
	public DictionaryLinkedList(IEqualityComparer<T> _comparer)
	{
		this.dict = new Dictionary<T, S>(_comparer);
	}

	// Token: 0x06008B99 RID: 35737 RVA: 0x00384EF8 File Offset: 0x003830F8
	public void Add(T _key, S _value)
	{
		this.dict.Add(_key, _value);
		LinkedListNode<S> value = this.list.AddLast(_value);
		this.indices.Add(_key, value);
	}

	// Token: 0x06008B9A RID: 35738 RVA: 0x00384F2C File Offset: 0x0038312C
	public void Set(T _key, S _value)
	{
		if (this.dict.ContainsKey(_key))
		{
			this.Remove(_key);
		}
		this.Add(_key, _value);
	}

	// Token: 0x06008B9B RID: 35739 RVA: 0x00384F4C File Offset: 0x0038314C
	public void Remove(T _key)
	{
		if (this.dict.ContainsKey(_key))
		{
			S s = this.dict[_key];
			this.dict.Remove(_key);
			LinkedListNode<S> node = this.indices[_key];
			this.list.Remove(node);
			this.indices.Remove(_key);
		}
	}

	// Token: 0x06008B9C RID: 35740 RVA: 0x00384FA7 File Offset: 0x003831A7
	public void Clear()
	{
		this.list.Clear();
		this.dict.Clear();
		this.indices.Clear();
	}

	// Token: 0x17000E8A RID: 3722
	// (get) Token: 0x06008B9D RID: 35741 RVA: 0x00384FCA File Offset: 0x003831CA
	public int Count
	{
		get
		{
			return this.list.Count;
		}
	}

	// Token: 0x04006D0C RID: 27916
	public Dictionary<T, S> dict;

	// Token: 0x04006D0D RID: 27917
	public LinkedList<S> list = new LinkedList<S>();

	// Token: 0x04006D0E RID: 27918
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<T, LinkedListNode<S>> indices = new Dictionary<T, LinkedListNode<S>>();
}
