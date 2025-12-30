using System;

// Token: 0x02000A33 RID: 2611
public class RegionFileFactorySectorBased : IRegionFilePlatformFactory
{
	// Token: 0x06004FD2 RID: 20434 RVA: 0x001FAA47 File Offset: 0x001F8C47
	public RegionFileAccessAbstract CreateRegionFileAccess()
	{
		return new RegionFileAccessSectorBased();
	}

	// Token: 0x06004FD3 RID: 20435 RVA: 0x001FAA4E File Offset: 0x001F8C4E
	public IRegionFileChunkSnapshotUtil CreateSnapshotUtil(RegionFileAccessAbstract regionFileAccess)
	{
		return new ChunkSnapshotUtil(regionFileAccess);
	}

	// Token: 0x06004FD4 RID: 20436 RVA: 0x001FAA56 File Offset: 0x001F8C56
	public IRegionFileDebugUtil CreateDebugUtil()
	{
		return new RegionFileDebugUtilSectorBased();
	}
}
