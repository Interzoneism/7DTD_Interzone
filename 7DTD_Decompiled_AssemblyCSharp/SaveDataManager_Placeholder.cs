using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Token: 0x02000941 RID: 2369
public class SaveDataManager_Placeholder : ISaveDataManager
{
	// Token: 0x06004735 RID: 18229 RVA: 0x00002914 File Offset: 0x00000B14
	public void Init()
	{
	}

	// Token: 0x06004736 RID: 18230 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public SaveDataWriteMode GetWriteMode()
	{
		return SaveDataWriteMode.None;
	}

	// Token: 0x06004737 RID: 18231 RVA: 0x00002914 File Offset: 0x00000B14
	public void SetWriteMode(SaveDataWriteMode writeMode)
	{
	}

	// Token: 0x06004738 RID: 18232 RVA: 0x00002914 File Offset: 0x00000B14
	public void Cleanup()
	{
	}

	// Token: 0x06004739 RID: 18233 RVA: 0x00002914 File Offset: 0x00000B14
	public void RegisterRegionFileManager(RegionFileManager regionFileManager)
	{
	}

	// Token: 0x0600473A RID: 18234 RVA: 0x00002914 File Offset: 0x00000B14
	public void DeregisterRegionFileManager(RegionFileManager regionFileManager)
	{
	}

	// Token: 0x0600473B RID: 18235 RVA: 0x00002914 File Offset: 0x00000B14
	public void CommitAsync()
	{
	}

	// Token: 0x0600473C RID: 18236 RVA: 0x00002914 File Offset: 0x00000B14
	public void CommitSync()
	{
	}

	// Token: 0x0600473D RID: 18237 RVA: 0x001C0BD1 File Offset: 0x001BEDD1
	public IEnumerator CommitCoroutine()
	{
		yield break;
	}

	// Token: 0x14000078 RID: 120
	// (add) Token: 0x0600473E RID: 18238 RVA: 0x00002914 File Offset: 0x00000B14
	// (remove) Token: 0x0600473F RID: 18239 RVA: 0x00002914 File Offset: 0x00000B14
	public event Action CommitStarted
	{
		add
		{
		}
		remove
		{
		}
	}

	// Token: 0x14000079 RID: 121
	// (add) Token: 0x06004740 RID: 18240 RVA: 0x00002914 File Offset: 0x00000B14
	// (remove) Token: 0x06004741 RID: 18241 RVA: 0x00002914 File Offset: 0x00000B14
	public event Action CommitFinished
	{
		add
		{
		}
		remove
		{
		}
	}

	// Token: 0x06004742 RID: 18242 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool ShouldLimitSize()
	{
		return false;
	}

	// Token: 0x06004743 RID: 18243 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void UpdateSizes()
	{
	}

	// Token: 0x06004744 RID: 18244 RVA: 0x001C0BDC File Offset: 0x001BEDDC
	public virtual SaveDataSizes GetSizes()
	{
		return default(SaveDataSizes);
	}

	// Token: 0x06004745 RID: 18245 RVA: 0x001C0BF2 File Offset: 0x001BEDF2
	public virtual Stream ManagedFileOpen(SaveDataManagedPath path, FileMode mode, FileAccess access, FileShare share)
	{
		return File.Open(path.GetOriginalPath(), mode, access, share);
	}

	// Token: 0x06004746 RID: 18246 RVA: 0x001C0C03 File Offset: 0x001BEE03
	public virtual void ManagedFileDelete(SaveDataManagedPath path)
	{
		File.Delete(path.GetOriginalPath());
	}

	// Token: 0x06004747 RID: 18247 RVA: 0x001C0C10 File Offset: 0x001BEE10
	public virtual bool ManagedFileExists(SaveDataManagedPath path)
	{
		return File.Exists(path.GetOriginalPath());
	}

	// Token: 0x06004748 RID: 18248 RVA: 0x001C0C1D File Offset: 0x001BEE1D
	public virtual DateTime ManagedFileGetLastWriteTimeUtc(SaveDataManagedPath path)
	{
		return File.GetLastWriteTimeUtc(path.GetOriginalPath());
	}

	// Token: 0x06004749 RID: 18249 RVA: 0x001C0C2A File Offset: 0x001BEE2A
	public virtual SdDirectoryInfo ManagedDirectoryCreateDirectory(SaveDataManagedPath path)
	{
		return new SdDirectoryInfo(Directory.CreateDirectory(path.GetOriginalPath()));
	}

	// Token: 0x0600474A RID: 18250 RVA: 0x001C0C3C File Offset: 0x001BEE3C
	public virtual DateTime ManagedDirectoryGetLastWriteTimeUtc(SaveDataManagedPath path)
	{
		return Directory.GetLastWriteTimeUtc(path.GetOriginalPath());
	}

	// Token: 0x0600474B RID: 18251 RVA: 0x001C0C49 File Offset: 0x001BEE49
	public virtual bool ManagedDirectoryExists(SaveDataManagedPath path)
	{
		return Directory.Exists(path.GetOriginalPath());
	}

	// Token: 0x0600474C RID: 18252 RVA: 0x001C0C56 File Offset: 0x001BEE56
	public virtual IEnumerable<SaveDataManagedPath> ManagedDirectoryEnumerateDirectories(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
	{
		return SaveDataManager_Placeholder.ConvertPathsToManagedPaths(Directory.EnumerateDirectories(path.GetOriginalPath(), searchPattern, searchOption));
	}

	// Token: 0x0600474D RID: 18253 RVA: 0x001C0C6A File Offset: 0x001BEE6A
	public virtual IEnumerable<SaveDataManagedPath> ManagedDirectoryEnumerateFiles(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
	{
		return SaveDataManager_Placeholder.ConvertPathsToManagedPaths(Directory.EnumerateFiles(path.GetOriginalPath(), searchPattern, searchOption));
	}

	// Token: 0x0600474E RID: 18254 RVA: 0x001C0C7E File Offset: 0x001BEE7E
	public virtual IEnumerable<SaveDataManagedPath> ManagedDirectoryEnumerateFileSystemEntries(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
	{
		return SaveDataManager_Placeholder.ConvertPathsToManagedPaths(Directory.EnumerateFileSystemEntries(path.GetOriginalPath(), searchPattern, searchOption));
	}

	// Token: 0x0600474F RID: 18255 RVA: 0x001C0C92 File Offset: 0x001BEE92
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerable<SaveDataManagedPath> ConvertPathsToManagedPaths(IEnumerable<string> paths)
	{
		using (IEnumerator<string> enumerator = paths.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				SaveDataManagedPath saveDataManagedPath;
				if (SaveDataUtils.TryGetManagedPath(enumerator.Current, out saveDataManagedPath))
				{
					yield return saveDataManagedPath;
				}
			}
		}
		IEnumerator<string> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x06004750 RID: 18256 RVA: 0x001C0CA2 File Offset: 0x001BEEA2
	public virtual void ManagedDirectoryDelete(SaveDataManagedPath path, bool recursive)
	{
		Directory.Delete(path.GetOriginalPath(), recursive);
	}

	// Token: 0x06004751 RID: 18257 RVA: 0x001C0CB0 File Offset: 0x001BEEB0
	public long ManagedFileInfoLength(SaveDataManagedPath path)
	{
		return new FileInfo(path.GetOriginalPath()).Length;
	}

	// Token: 0x06004752 RID: 18258 RVA: 0x001C0CC2 File Offset: 0x001BEEC2
	public virtual IEnumerable<SdDirectoryInfo> ManagedDirectoryInfoEnumerateDirectories(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
	{
		return from x in new DirectoryInfo(path.GetOriginalPath()).EnumerateDirectories(searchPattern, searchOption)
		select new SdDirectoryInfo(x);
	}

	// Token: 0x06004753 RID: 18259 RVA: 0x001C0CFA File Offset: 0x001BEEFA
	public virtual IEnumerable<SdFileInfo> ManagedDirectoryInfoEnumerateFiles(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
	{
		return from x in new DirectoryInfo(path.GetOriginalPath()).EnumerateFiles(searchPattern, searchOption)
		select new SdFileInfo(x);
	}

	// Token: 0x06004754 RID: 18260 RVA: 0x001C0D32 File Offset: 0x001BEF32
	public virtual IEnumerable<SdFileSystemInfo> ManagedDirectoryInfoEnumerateFileSystemInfos(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
	{
		return new DirectoryInfo(path.GetOriginalPath()).EnumerateFileSystemInfos(searchPattern, searchOption).Select(new Func<FileSystemInfo, SdFileSystemInfo>(SdDirectoryInfo.WrapFileSystemInfo));
	}

	// Token: 0x040036CB RID: 14027
	public static readonly SaveDataManager_Placeholder Instance = new SaveDataManager_Placeholder();
}
