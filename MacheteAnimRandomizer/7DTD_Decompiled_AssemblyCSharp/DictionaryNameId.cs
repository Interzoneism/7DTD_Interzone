using System;
using System.Collections.Generic;

// Token: 0x02001175 RID: 4469
public class DictionaryNameId<T>
{
	// Token: 0x06008BA5 RID: 35749 RVA: 0x003850BA File Offset: 0x003832BA
	public DictionaryNameId(DictionaryNameIdMapping _mapping)
	{
		this.mapping = _mapping;
	}

	// Token: 0x06008BA6 RID: 35750 RVA: 0x003850D4 File Offset: 0x003832D4
	public void Add(string _name, T _value)
	{
		int key = this.mapping.Add(_name);
		this.idsToValues[key] = _value;
	}

	// Token: 0x17000E8C RID: 3724
	// (get) Token: 0x06008BA7 RID: 35751 RVA: 0x003850FB File Offset: 0x003832FB
	public int Count
	{
		get
		{
			return this.idsToValues.Count;
		}
	}

	// Token: 0x17000E8D RID: 3725
	// (get) Token: 0x06008BA8 RID: 35752 RVA: 0x00385108 File Offset: 0x00383308
	public Dictionary<int, T> Dict
	{
		get
		{
			return this.idsToValues;
		}
	}

	// Token: 0x06008BA9 RID: 35753 RVA: 0x00385110 File Offset: 0x00383310
	public bool Contains(string _name)
	{
		int num = this.mapping.FindId(_name);
		return num != 0 && this.idsToValues.ContainsKey(num);
	}

	// Token: 0x06008BAA RID: 35754 RVA: 0x0038513C File Offset: 0x0038333C
	public T Get(int _id)
	{
		T result;
		this.idsToValues.TryGetValue(_id, out result);
		return result;
	}

	// Token: 0x06008BAB RID: 35755 RVA: 0x0038515C File Offset: 0x0038335C
	public T Get(string _name)
	{
		int num = this.mapping.FindId(_name);
		if (num == 0)
		{
			return default(T);
		}
		T result;
		this.idsToValues.TryGetValue(num, out result);
		return result;
	}

	// Token: 0x04006D11 RID: 27921
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly DictionaryNameIdMapping mapping;

	// Token: 0x04006D12 RID: 27922
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<int, T> idsToValues = new Dictionary<int, T>();
}
