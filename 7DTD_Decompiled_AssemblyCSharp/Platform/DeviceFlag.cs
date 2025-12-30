using System;

namespace Platform
{
	// Token: 0x020017C5 RID: 6085
	[Flags]
	public enum DeviceFlag
	{
		// Token: 0x04008F2E RID: 36654
		None = 0,
		// Token: 0x04008F2F RID: 36655
		StandaloneWindows = 1,
		// Token: 0x04008F30 RID: 36656
		StandaloneLinux = 2,
		// Token: 0x04008F31 RID: 36657
		StandaloneOSX = 4,
		// Token: 0x04008F32 RID: 36658
		XBoxSeriesS = 8,
		// Token: 0x04008F33 RID: 36659
		XBoxSeriesX = 16,
		// Token: 0x04008F34 RID: 36660
		PS5 = 32
	}
}
