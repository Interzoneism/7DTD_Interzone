using System;

// Token: 0x02000A31 RID: 2609
public interface IRegionFilePlatformFactory
{
	// Token: 0x06004FCE RID: 20430
	RegionFileAccessAbstract CreateRegionFileAccess();

	// Token: 0x06004FCF RID: 20431
	IRegionFileChunkSnapshotUtil CreateSnapshotUtil(RegionFileAccessAbstract regionFileAccess);

	// Token: 0x06004FD0 RID: 20432
	IRegionFileDebugUtil CreateDebugUtil();
}
