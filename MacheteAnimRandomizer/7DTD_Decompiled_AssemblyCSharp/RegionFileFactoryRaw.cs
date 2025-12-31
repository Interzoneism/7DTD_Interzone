using System;

// Token: 0x02000A34 RID: 2612
public class RegionFileFactoryRaw : IRegionFilePlatformFactory
{
	// Token: 0x06004FD6 RID: 20438 RVA: 0x001FAA5D File Offset: 0x001F8C5D
	public RegionFileAccessAbstract CreateRegionFileAccess()
	{
		return new RegionFileAccessRaw();
	}

	// Token: 0x06004FD7 RID: 20439 RVA: 0x001FAA4E File Offset: 0x001F8C4E
	public IRegionFileChunkSnapshotUtil CreateSnapshotUtil(RegionFileAccessAbstract regionFileAccess)
	{
		return new ChunkSnapshotUtil(regionFileAccess);
	}

	// Token: 0x06004FD8 RID: 20440 RVA: 0x001FAA64 File Offset: 0x001F8C64
	public IRegionFileDebugUtil CreateDebugUtil()
	{
		return new RegionFileDebugUtilRaw();
	}
}
