using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Token: 0x02000929 RID: 2345
public static class SdDirectory
{
	// Token: 0x0600460E RID: 17934 RVA: 0x001BE010 File Offset: 0x001BC210
	public static SdDirectoryInfo CreateDirectory(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdDirectory.ManagedCreateDirectory(path2);
		}
		return new SdDirectoryInfo(Directory.CreateDirectory(path));
	}

	// Token: 0x0600460F RID: 17935 RVA: 0x001BE039 File Offset: 0x001BC239
	[PublicizedFrom(EAccessModifier.Internal)]
	public static SdDirectoryInfo ManagedCreateDirectory(SaveDataManagedPath path)
	{
		return SaveDataUtils.SaveDataManager.ManagedDirectoryCreateDirectory(path);
	}

	// Token: 0x06004610 RID: 17936 RVA: 0x001BE048 File Offset: 0x001BC248
	public static DateTime GetLastWriteTime(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdDirectory.ManagedGetLastWriteTime(path2);
		}
		return Directory.GetLastWriteTime(path);
	}

	// Token: 0x06004611 RID: 17937 RVA: 0x001BE06C File Offset: 0x001BC26C
	[PublicizedFrom(EAccessModifier.Private)]
	public static DateTime ManagedGetLastWriteTime(SaveDataManagedPath path)
	{
		return SdDirectory.ManagedGetLastWriteTimeUtc(path).ToLocalTime();
	}

	// Token: 0x06004612 RID: 17938 RVA: 0x001BE088 File Offset: 0x001BC288
	public static DateTime GetLastWriteTimeUtc(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdDirectory.ManagedGetLastWriteTimeUtc(path2);
		}
		return Directory.GetLastWriteTimeUtc(path);
	}

	// Token: 0x06004613 RID: 17939 RVA: 0x001BE0AC File Offset: 0x001BC2AC
	[PublicizedFrom(EAccessModifier.Internal)]
	public static DateTime ManagedGetLastWriteTimeUtc(SaveDataManagedPath path)
	{
		return SaveDataUtils.SaveDataManager.ManagedDirectoryGetLastWriteTimeUtc(path);
	}

	// Token: 0x06004614 RID: 17940 RVA: 0x001BE0BC File Offset: 0x001BC2BC
	public static bool Exists(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdDirectory.ManagedExists(path2);
		}
		return Directory.Exists(path);
	}

	// Token: 0x06004615 RID: 17941 RVA: 0x001BE0E0 File Offset: 0x001BC2E0
	[PublicizedFrom(EAccessModifier.Internal)]
	public static bool ManagedExists(SaveDataManagedPath path)
	{
		return SaveDataUtils.SaveDataManager.ManagedDirectoryExists(path);
	}

	// Token: 0x06004616 RID: 17942 RVA: 0x001BE0ED File Offset: 0x001BC2ED
	public static string[] GetFiles(string path)
	{
		return SdDirectory.EnumerateFiles(path).ToArray<string>();
	}

	// Token: 0x06004617 RID: 17943 RVA: 0x001BE0FA File Offset: 0x001BC2FA
	public static string[] GetFiles(string path, string searchPattern)
	{
		return SdDirectory.EnumerateFiles(path, searchPattern).ToArray<string>();
	}

	// Token: 0x06004618 RID: 17944 RVA: 0x001BE108 File Offset: 0x001BC308
	public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
	{
		return SdDirectory.EnumerateFiles(path, searchPattern, searchOption).ToArray<string>();
	}

	// Token: 0x06004619 RID: 17945 RVA: 0x001BE117 File Offset: 0x001BC317
	public static string[] GetDirectories(string path)
	{
		return SdDirectory.EnumerateDirectories(path).ToArray<string>();
	}

	// Token: 0x0600461A RID: 17946 RVA: 0x001BE124 File Offset: 0x001BC324
	public static string[] GetDirectories(string path, string searchPattern)
	{
		return SdDirectory.EnumerateDirectories(path, searchPattern).ToArray<string>();
	}

	// Token: 0x0600461B RID: 17947 RVA: 0x001BE132 File Offset: 0x001BC332
	public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
	{
		return SdDirectory.EnumerateDirectories(path, searchPattern, searchOption).ToArray<string>();
	}

	// Token: 0x0600461C RID: 17948 RVA: 0x001BE141 File Offset: 0x001BC341
	public static string[] GetFileSystemEntries(string path)
	{
		return SdDirectory.EnumerateFileSystemEntries(path).ToArray<string>();
	}

	// Token: 0x0600461D RID: 17949 RVA: 0x001BE14E File Offset: 0x001BC34E
	public static string[] GetFileSystemEntries(string path, string searchPattern)
	{
		return SdDirectory.EnumerateFileSystemEntries(path, searchPattern).ToArray<string>();
	}

	// Token: 0x0600461E RID: 17950 RVA: 0x001BE15C File Offset: 0x001BC35C
	public static string[] GetFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
	{
		return SdDirectory.EnumerateFileSystemEntries(path, searchPattern, searchOption).ToArray<string>();
	}

	// Token: 0x0600461F RID: 17951 RVA: 0x001BE16C File Offset: 0x001BC36C
	public static IEnumerable<string> EnumerateDirectories(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return from x in SdDirectory.ManagedEnumerateDirectories(path2, "*", SearchOption.TopDirectoryOnly)
			select x.GetOriginalPath();
		}
		return Directory.EnumerateDirectories(path);
	}

	// Token: 0x06004620 RID: 17952 RVA: 0x001BE1BC File Offset: 0x001BC3BC
	public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return from x in SdDirectory.ManagedEnumerateDirectories(path2, searchPattern, SearchOption.TopDirectoryOnly)
			select x.GetOriginalPath();
		}
		return Directory.EnumerateDirectories(path, searchPattern);
	}

	// Token: 0x06004621 RID: 17953 RVA: 0x001BE208 File Offset: 0x001BC408
	public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return from x in SdDirectory.ManagedEnumerateDirectories(path2, searchPattern, searchOption)
			select x.GetOriginalPath();
		}
		return Directory.EnumerateDirectories(path, searchPattern, searchOption);
	}

	// Token: 0x06004622 RID: 17954 RVA: 0x001BE254 File Offset: 0x001BC454
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerable<SaveDataManagedPath> ManagedEnumerateDirectories(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
	{
		return SaveDataUtils.SaveDataManager.ManagedDirectoryEnumerateDirectories(path, searchPattern, searchOption);
	}

	// Token: 0x06004623 RID: 17955 RVA: 0x001BE264 File Offset: 0x001BC464
	public static IEnumerable<string> EnumerateFiles(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return from x in SdDirectory.ManagedEnumerateFiles(path2, "*", SearchOption.TopDirectoryOnly)
			select x.GetOriginalPath();
		}
		return Directory.EnumerateFiles(path);
	}

	// Token: 0x06004624 RID: 17956 RVA: 0x001BE2B4 File Offset: 0x001BC4B4
	public static IEnumerable<string> EnumerateFiles(string path, string searchPattern)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return from x in SdDirectory.ManagedEnumerateFiles(path2, searchPattern, SearchOption.TopDirectoryOnly)
			select x.GetOriginalPath();
		}
		return Directory.EnumerateFiles(path, searchPattern);
	}

	// Token: 0x06004625 RID: 17957 RVA: 0x001BE300 File Offset: 0x001BC500
	public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return from x in SdDirectory.ManagedEnumerateFiles(path2, searchPattern, searchOption)
			select x.GetOriginalPath();
		}
		return Directory.EnumerateFiles(path, searchPattern, searchOption);
	}

	// Token: 0x06004626 RID: 17958 RVA: 0x001BE34C File Offset: 0x001BC54C
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerable<SaveDataManagedPath> ManagedEnumerateFiles(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
	{
		return SaveDataUtils.SaveDataManager.ManagedDirectoryEnumerateFiles(path, searchPattern, searchOption);
	}

	// Token: 0x06004627 RID: 17959 RVA: 0x001BE35C File Offset: 0x001BC55C
	public static IEnumerable<string> EnumerateFileSystemEntries(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return from x in SdDirectory.ManagedEnumerateFileSystemEntries(path2, "*", SearchOption.TopDirectoryOnly)
			select x.GetOriginalPath();
		}
		return Directory.EnumerateFileSystemEntries(path);
	}

	// Token: 0x06004628 RID: 17960 RVA: 0x001BE3AC File Offset: 0x001BC5AC
	public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return from x in SdDirectory.ManagedEnumerateFileSystemEntries(path2, searchPattern, SearchOption.TopDirectoryOnly)
			select x.GetOriginalPath();
		}
		return Directory.EnumerateFileSystemEntries(path, searchPattern);
	}

	// Token: 0x06004629 RID: 17961 RVA: 0x001BE3F8 File Offset: 0x001BC5F8
	public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return from x in SdDirectory.ManagedEnumerateFileSystemEntries(path2, searchPattern, searchOption)
			select x.GetOriginalPath();
		}
		return Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);
	}

	// Token: 0x0600462A RID: 17962 RVA: 0x001BE444 File Offset: 0x001BC644
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerable<SaveDataManagedPath> ManagedEnumerateFileSystemEntries(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
	{
		return SaveDataUtils.SaveDataManager.ManagedDirectoryEnumerateFileSystemEntries(path, searchPattern, searchOption);
	}

	// Token: 0x0600462B RID: 17963 RVA: 0x001BE453 File Offset: 0x001BC653
	public static string[] GetLogicalDrives()
	{
		return Directory.GetLogicalDrives();
	}

	// Token: 0x0600462C RID: 17964 RVA: 0x001BE45A File Offset: 0x001BC65A
	public static string GetDirectoryRoot(string path)
	{
		return Directory.GetDirectoryRoot(path);
	}

	// Token: 0x0600462D RID: 17965 RVA: 0x001BE462 File Offset: 0x001BC662
	public static string GetCurrentDirectory()
	{
		return Directory.GetCurrentDirectory();
	}

	// Token: 0x0600462E RID: 17966 RVA: 0x001BE469 File Offset: 0x001BC669
	public static void SetCurrentDirectory(string path)
	{
		Directory.SetCurrentDirectory(path);
	}

	// Token: 0x0600462F RID: 17967 RVA: 0x001BE474 File Offset: 0x001BC674
	public static void Delete(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			SdDirectory.ManagedDelete(path2, false);
			return;
		}
		Directory.Delete(path);
	}

	// Token: 0x06004630 RID: 17968 RVA: 0x001BE49C File Offset: 0x001BC69C
	public static void Delete(string path, bool recursive)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			SdDirectory.ManagedDelete(path2, recursive);
			return;
		}
		Directory.Delete(path, recursive);
	}

	// Token: 0x06004631 RID: 17969 RVA: 0x001BE4C2 File Offset: 0x001BC6C2
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void ManagedDelete(SaveDataManagedPath path, bool recursive)
	{
		SaveDataUtils.SaveDataManager.ManagedDirectoryDelete(path, recursive);
	}
}
