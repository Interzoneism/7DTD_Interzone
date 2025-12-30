using System;

namespace SharpEXR
{
	// Token: 0x02001405 RID: 5125
	[Flags]
	public enum EXRVersionFlags
	{
		// Token: 0x04007AB3 RID: 31411
		IsSinglePartTiled = 512,
		// Token: 0x04007AB4 RID: 31412
		LongNames = 1024,
		// Token: 0x04007AB5 RID: 31413
		NonImageParts = 2048,
		// Token: 0x04007AB6 RID: 31414
		MultiPart = 4096
	}
}
