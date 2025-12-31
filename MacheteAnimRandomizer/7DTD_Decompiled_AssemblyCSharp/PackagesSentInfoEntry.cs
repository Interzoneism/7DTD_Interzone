using System;
using System.Collections.Generic;

// Token: 0x020006ED RID: 1773
public class PackagesSentInfoEntry
{
	// Token: 0x04002AD7 RID: 10967
	public List<NetPackageInfo> packages;

	// Token: 0x04002AD8 RID: 10968
	public int count;

	// Token: 0x04002AD9 RID: 10969
	public long uncompressedSize;

	// Token: 0x04002ADA RID: 10970
	public long compressedSize;

	// Token: 0x04002ADB RID: 10971
	public bool bCompressed;

	// Token: 0x04002ADC RID: 10972
	public float timestamp;

	// Token: 0x04002ADD RID: 10973
	public string client;
}
