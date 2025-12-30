using System;
using System.Collections.Generic;

// Token: 0x02001172 RID: 4466
public class DictionaryKeyValueList<T, S>
{
	// Token: 0x06008B93 RID: 35731 RVA: 0x00384DF4 File Offset: 0x00382FF4
	public void Add(T _key, S _value)
	{
		this.dict.Add(_key, _value);
		this.keyList.Add(_key);
		this.valueList.Add(_value);
	}

	// Token: 0x06008B94 RID: 35732 RVA: 0x00384E1B File Offset: 0x0038301B
	public void Set(T _key, S _value)
	{
		if (this.dict.ContainsKey(_key))
		{
			this.Remove(_key);
		}
		this.Add(_key, _value);
	}

	// Token: 0x06008B95 RID: 35733 RVA: 0x00384E3C File Offset: 0x0038303C
	public void Remove(T _key)
	{
		int num = this.keyList.IndexOf(_key);
		if (num >= 0)
		{
			this.keyList.RemoveAt(num);
			this.valueList.RemoveAt(num);
			this.dict.Remove(_key);
		}
	}

	// Token: 0x06008B96 RID: 35734 RVA: 0x00384E7F File Offset: 0x0038307F
	public void Clear()
	{
		this.keyList.Clear();
		this.valueList.Clear();
		this.dict.Clear();
	}

	// Token: 0x04006D09 RID: 27913
	public Dictionary<T, S> dict = new Dictionary<T, S>();

	// Token: 0x04006D0A RID: 27914
	public List<S> valueList = new List<S>();

	// Token: 0x04006D0B RID: 27915
	public List<T> keyList = new List<T>();
}
