using System;
using System.Collections.Generic;

// Token: 0x02001171 RID: 4465
public class DictionaryKeyList<T, S>
{
	// Token: 0x06008B8E RID: 35726 RVA: 0x00384D5D File Offset: 0x00382F5D
	public void Add(T _key, S _value)
	{
		this.dict.Add(_key, _value);
		this.list.Add(_key);
	}

	// Token: 0x06008B8F RID: 35727 RVA: 0x00384D78 File Offset: 0x00382F78
	public void Remove(T _key)
	{
		this.list.Remove(_key);
		this.dict.Remove(_key);
	}

	// Token: 0x06008B90 RID: 35728 RVA: 0x00384D94 File Offset: 0x00382F94
	public void Replace(T _key, S _value)
	{
		if (this.dict.ContainsKey(_key))
		{
			this.Remove(_key);
		}
		this.Add(_key, _value);
	}

	// Token: 0x06008B91 RID: 35729 RVA: 0x00384DB3 File Offset: 0x00382FB3
	public void Clear()
	{
		this.list.Clear();
		this.dict.Clear();
	}

	// Token: 0x04006D07 RID: 27911
	public Dictionary<T, S> dict = new Dictionary<T, S>();

	// Token: 0x04006D08 RID: 27912
	public List<T> list = new List<T>();
}
