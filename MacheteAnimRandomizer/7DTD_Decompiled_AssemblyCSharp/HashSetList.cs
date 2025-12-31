using System;
using System.Collections.Generic;

// Token: 0x020011A7 RID: 4519
public class HashSetList<T>
{
	// Token: 0x06008D47 RID: 36167 RVA: 0x0038C437 File Offset: 0x0038A637
	public void Add(T _value)
	{
		if (this.hashSet.Add(_value))
		{
			this.list.Add(_value);
		}
	}

	// Token: 0x06008D48 RID: 36168 RVA: 0x0038C453 File Offset: 0x0038A653
	public void Remove(T _value)
	{
		if (this.hashSet.Remove(_value))
		{
			this.list.Remove(_value);
		}
	}

	// Token: 0x06008D49 RID: 36169 RVA: 0x0038C470 File Offset: 0x0038A670
	public void Clear()
	{
		this.list.Clear();
		this.hashSet.Clear();
	}

	// Token: 0x04006DC0 RID: 28096
	public HashSet<T> hashSet = new HashSet<T>();

	// Token: 0x04006DC1 RID: 28097
	public List<T> list = new List<T>();
}
