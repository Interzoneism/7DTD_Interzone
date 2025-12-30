using System;

// Token: 0x02001063 RID: 4195
public interface Serializable
{
	// Token: 0x060084A4 RID: 33956
	byte[] Serialize();

	// Token: 0x17000DD0 RID: 3536
	// (get) Token: 0x060084A5 RID: 33957
	// (set) Token: 0x060084A6 RID: 33958
	bool IsDirty { get; set; }
}
