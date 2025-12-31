using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// Token: 0x02000927 RID: 2343
public interface ISaveDataManager
{
	// Token: 0x060045E0 RID: 17888
	void Init();

	// Token: 0x060045E1 RID: 17889
	void Cleanup();

	// Token: 0x060045E2 RID: 17890
	SaveDataWriteMode GetWriteMode();

	// Token: 0x060045E3 RID: 17891
	void SetWriteMode(SaveDataWriteMode writeMode);

	// Token: 0x060045E4 RID: 17892
	void RegisterRegionFileManager(RegionFileManager regionFileManager);

	// Token: 0x060045E5 RID: 17893
	void DeregisterRegionFileManager(RegionFileManager regionFileManager);

	// Token: 0x060045E6 RID: 17894
	void CommitAsync();

	// Token: 0x060045E7 RID: 17895
	void CommitSync();

	// Token: 0x060045E8 RID: 17896
	IEnumerator CommitCoroutine();

	// Token: 0x14000076 RID: 118
	// (add) Token: 0x060045E9 RID: 17897
	// (remove) Token: 0x060045EA RID: 17898
	event Action CommitStarted;

	// Token: 0x14000077 RID: 119
	// (add) Token: 0x060045EB RID: 17899
	// (remove) Token: 0x060045EC RID: 17900
	event Action CommitFinished;

	// Token: 0x060045ED RID: 17901
	bool ShouldLimitSize();

	// Token: 0x060045EE RID: 17902
	void UpdateSizes();

	// Token: 0x060045EF RID: 17903
	SaveDataSizes GetSizes();

	// Token: 0x060045F0 RID: 17904
	Stream ManagedFileOpen(SaveDataManagedPath path, FileMode mode, FileAccess access, FileShare share);

	// Token: 0x060045F1 RID: 17905
	void ManagedFileDelete(SaveDataManagedPath path);

	// Token: 0x060045F2 RID: 17906
	bool ManagedFileExists(SaveDataManagedPath path);

	// Token: 0x060045F3 RID: 17907
	DateTime ManagedFileGetLastWriteTimeUtc(SaveDataManagedPath path);

	// Token: 0x060045F4 RID: 17908
	SdDirectoryInfo ManagedDirectoryCreateDirectory(SaveDataManagedPath path);

	// Token: 0x060045F5 RID: 17909
	DateTime ManagedDirectoryGetLastWriteTimeUtc(SaveDataManagedPath path);

	// Token: 0x060045F6 RID: 17910
	bool ManagedDirectoryExists(SaveDataManagedPath path);

	// Token: 0x060045F7 RID: 17911
	IEnumerable<SaveDataManagedPath> ManagedDirectoryEnumerateDirectories(SaveDataManagedPath path, string searchPattern, SearchOption searchOption);

	// Token: 0x060045F8 RID: 17912
	IEnumerable<SaveDataManagedPath> ManagedDirectoryEnumerateFiles(SaveDataManagedPath path, string searchPattern, SearchOption searchOption);

	// Token: 0x060045F9 RID: 17913
	IEnumerable<SaveDataManagedPath> ManagedDirectoryEnumerateFileSystemEntries(SaveDataManagedPath path, string searchPattern, SearchOption searchOption);

	// Token: 0x060045FA RID: 17914
	void ManagedDirectoryDelete(SaveDataManagedPath path, bool recursive);

	// Token: 0x060045FB RID: 17915
	long ManagedFileInfoLength(SaveDataManagedPath path);

	// Token: 0x060045FC RID: 17916
	IEnumerable<SdDirectoryInfo> ManagedDirectoryInfoEnumerateDirectories(SaveDataManagedPath path, string searchPattern, SearchOption searchOption);

	// Token: 0x060045FD RID: 17917
	IEnumerable<SdFileInfo> ManagedDirectoryInfoEnumerateFiles(SaveDataManagedPath path, string searchPattern, SearchOption searchOption);

	// Token: 0x060045FE RID: 17918
	IEnumerable<SdFileSystemInfo> ManagedDirectoryInfoEnumerateFileSystemInfos(SaveDataManagedPath path, string searchPattern, SearchOption searchOption);
}
