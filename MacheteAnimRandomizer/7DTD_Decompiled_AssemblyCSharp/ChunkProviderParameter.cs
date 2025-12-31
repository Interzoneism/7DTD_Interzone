using System;

// Token: 0x020009C5 RID: 2501
public class ChunkProviderParameter
{
	// Token: 0x06004CA0 RID: 19616 RVA: 0x001E5E74 File Offset: 0x001E4074
	public ChunkProviderParameter(int _id, string _name, float _val, float _minVal, float _maxVal)
	{
		this.id = _id;
		this.name = _name;
		this.val = _val;
		this.minVal = _minVal;
		this.maxVal = _maxVal;
	}

	// Token: 0x04003A92 RID: 14994
	public int id;

	// Token: 0x04003A93 RID: 14995
	public string name;

	// Token: 0x04003A94 RID: 14996
	public float val;

	// Token: 0x04003A95 RID: 14997
	public float minVal;

	// Token: 0x04003A96 RID: 14998
	public float maxVal;
}
