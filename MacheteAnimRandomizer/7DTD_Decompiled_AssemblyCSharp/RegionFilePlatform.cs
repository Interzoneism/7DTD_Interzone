using System;

// Token: 0x02000A32 RID: 2610
public static class RegionFilePlatform
{
	// Token: 0x06004FD1 RID: 20433 RVA: 0x001FAA40 File Offset: 0x001F8C40
	public static IRegionFilePlatformFactory CreateFactory()
	{
		return new RegionFileFactorySectorBased();
	}
}
