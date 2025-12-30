using System;

// Token: 0x02001176 RID: 4470
public class DictionaryNameIdMapping
{
	// Token: 0x06008BAC RID: 35756 RVA: 0x00385194 File Offset: 0x00383394
	public int Add(string _name)
	{
		int num;
		this.namesToIds.TryGetValue(_name, out num);
		if (num == 0)
		{
			int num2 = this.nextId + 1;
			this.nextId = num2;
			num = num2;
			this.namesToIds[_name] = num;
		}
		return num;
	}

	// Token: 0x06008BAD RID: 35757 RVA: 0x003851D3 File Offset: 0x003833D3
	public void Clear()
	{
		this.nextId = 0;
		this.namesToIds.Clear();
	}

	// Token: 0x06008BAE RID: 35758 RVA: 0x003851E8 File Offset: 0x003833E8
	public int FindId(string _name)
	{
		int result;
		this.namesToIds.TryGetValue(_name, out result);
		return result;
	}

	// Token: 0x04006D13 RID: 27923
	public const int cIDNone = 0;

	// Token: 0x04006D14 RID: 27924
	[PublicizedFrom(EAccessModifier.Private)]
	public int nextId;

	// Token: 0x04006D15 RID: 27925
	[PublicizedFrom(EAccessModifier.Private)]
	public CaseInsensitiveStringDictionary<int> namesToIds = new CaseInsensitiveStringDictionary<int>();
}
