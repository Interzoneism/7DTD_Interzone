using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Token: 0x0200092B RID: 2347
public sealed class SdDirectoryInfo : SdFileSystemInfo
{
	// Token: 0x0600463D RID: 17981 RVA: 0x001BE4E4 File Offset: 0x001BC6E4
	public SdDirectoryInfo(string path) : this(new DirectoryInfo(path))
	{
	}

	// Token: 0x0600463E RID: 17982 RVA: 0x001BE4F2 File Offset: 0x001BC6F2
	[PublicizedFrom(EAccessModifier.Internal)]
	public SdDirectoryInfo(SaveDataManagedPath saveDataManagedPath) : this(new DirectoryInfo(saveDataManagedPath.GetOriginalPath()), saveDataManagedPath)
	{
	}

	// Token: 0x0600463F RID: 17983 RVA: 0x001BE506 File Offset: 0x001BC706
	[PublicizedFrom(EAccessModifier.Internal)]
	public SdDirectoryInfo(DirectoryInfo directoryInfo) : base(directoryInfo)
	{
		this.m_directoryInfo = directoryInfo;
	}

	// Token: 0x06004640 RID: 17984 RVA: 0x001BE516 File Offset: 0x001BC716
	[PublicizedFrom(EAccessModifier.Private)]
	public SdDirectoryInfo(DirectoryInfo directoryInfo, SaveDataManagedPath saveDataManagedPath) : base(directoryInfo, saveDataManagedPath)
	{
		this.m_directoryInfo = directoryInfo;
	}

	// Token: 0x17000758 RID: 1880
	// (get) Token: 0x06004641 RID: 17985 RVA: 0x001BE527 File Offset: 0x001BC727
	public override string Name
	{
		get
		{
			return this.m_directoryInfo.Name;
		}
	}

	// Token: 0x17000759 RID: 1881
	// (get) Token: 0x06004642 RID: 17986 RVA: 0x001BE534 File Offset: 0x001BC734
	public override string FullName
	{
		get
		{
			return this.m_directoryInfo.FullName;
		}
	}

	// Token: 0x1700075A RID: 1882
	// (get) Token: 0x06004643 RID: 17987 RVA: 0x001BE541 File Offset: 0x001BC741
	public SdDirectoryInfo Parent
	{
		get
		{
			return new SdDirectoryInfo(this.m_directoryInfo.Parent);
		}
	}

	// Token: 0x06004644 RID: 17988 RVA: 0x001BE553 File Offset: 0x001BC753
	public SdDirectoryInfo CreateSubdirectory(string path)
	{
		if (base.IsManaged)
		{
			return this.ManagedCreateSubdirectory(path);
		}
		return new SdDirectoryInfo(this.m_directoryInfo.CreateSubdirectory(path));
	}

	// Token: 0x06004645 RID: 17989 RVA: 0x001BE576 File Offset: 0x001BC776
	[PublicizedFrom(EAccessModifier.Private)]
	public SdDirectoryInfo ManagedCreateSubdirectory(string path)
	{
		return SdDirectory.CreateDirectory(Path.Combine(this.FullName, path));
	}

	// Token: 0x06004646 RID: 17990 RVA: 0x001BE589 File Offset: 0x001BC789
	public void Create()
	{
		if (base.IsManaged)
		{
			this.ManagedCreateDirectory();
			return;
		}
		this.m_directoryInfo.Create();
	}

	// Token: 0x06004647 RID: 17991 RVA: 0x001BE5A5 File Offset: 0x001BC7A5
	[PublicizedFrom(EAccessModifier.Private)]
	public void ManagedCreateDirectory()
	{
		SdDirectory.ManagedCreateDirectory(base.ManagedPath);
	}

	// Token: 0x1700075B RID: 1883
	// (get) Token: 0x06004648 RID: 17992 RVA: 0x001BE5B3 File Offset: 0x001BC7B3
	public override bool Exists
	{
		get
		{
			if (!base.IsManaged)
			{
				return this.m_directoryInfo.Exists;
			}
			return SdDirectory.ManagedExists(base.ManagedPath);
		}
	}

	// Token: 0x06004649 RID: 17993 RVA: 0x001BE5D4 File Offset: 0x001BC7D4
	public SdFileInfo[] GetFiles(string searchPattern)
	{
		return this.EnumerateFiles(searchPattern).ToArray<SdFileInfo>();
	}

	// Token: 0x0600464A RID: 17994 RVA: 0x001BE5E2 File Offset: 0x001BC7E2
	public SdFileInfo[] GetFiles(string searchPattern, SearchOption searchOption)
	{
		return this.EnumerateFiles(searchPattern, searchOption).ToArray<SdFileInfo>();
	}

	// Token: 0x0600464B RID: 17995 RVA: 0x001BE5F1 File Offset: 0x001BC7F1
	public SdFileInfo[] GetFiles()
	{
		return this.EnumerateFiles().ToArray<SdFileInfo>();
	}

	// Token: 0x0600464C RID: 17996 RVA: 0x001BE5FE File Offset: 0x001BC7FE
	public SdDirectoryInfo[] GetDirectories()
	{
		return this.EnumerateDirectories().ToArray<SdDirectoryInfo>();
	}

	// Token: 0x0600464D RID: 17997 RVA: 0x001BE60B File Offset: 0x001BC80B
	public SdDirectoryInfo[] GetDirectories(string searchPattern)
	{
		return this.EnumerateDirectories(searchPattern).ToArray<SdDirectoryInfo>();
	}

	// Token: 0x0600464E RID: 17998 RVA: 0x001BE619 File Offset: 0x001BC819
	public SdDirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption)
	{
		return this.EnumerateDirectories(searchPattern, searchOption).ToArray<SdDirectoryInfo>();
	}

	// Token: 0x0600464F RID: 17999 RVA: 0x001BE628 File Offset: 0x001BC828
	public SdFileSystemInfo[] GetFileSystemInfos()
	{
		return this.EnumerateFileSystemInfos().ToArray<SdFileSystemInfo>();
	}

	// Token: 0x06004650 RID: 18000 RVA: 0x001BE635 File Offset: 0x001BC835
	public SdFileSystemInfo[] GetFileSystemInfos(string searchPattern)
	{
		return this.EnumerateFileSystemInfos(searchPattern).ToArray<SdFileSystemInfo>();
	}

	// Token: 0x06004651 RID: 18001 RVA: 0x001BE643 File Offset: 0x001BC843
	public SdFileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption)
	{
		return this.EnumerateFileSystemInfos(searchPattern, searchOption).ToArray<SdFileSystemInfo>();
	}

	// Token: 0x06004652 RID: 18002 RVA: 0x001BE654 File Offset: 0x001BC854
	public IEnumerable<SdDirectoryInfo> EnumerateDirectories()
	{
		if (base.IsManaged)
		{
			return this.ManagedEnumerateDirectories("*", SearchOption.TopDirectoryOnly);
		}
		return from x in this.m_directoryInfo.EnumerateDirectories()
		select new SdDirectoryInfo(x);
	}

	// Token: 0x06004653 RID: 18003 RVA: 0x001BE6A8 File Offset: 0x001BC8A8
	public IEnumerable<SdDirectoryInfo> EnumerateDirectories(string searchPattern)
	{
		if (base.IsManaged)
		{
			return this.ManagedEnumerateDirectories(searchPattern, SearchOption.TopDirectoryOnly);
		}
		return from x in this.m_directoryInfo.EnumerateDirectories(searchPattern)
		select new SdDirectoryInfo(x);
	}

	// Token: 0x06004654 RID: 18004 RVA: 0x001BE6F8 File Offset: 0x001BC8F8
	public IEnumerable<SdDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption)
	{
		if (base.IsManaged)
		{
			return this.ManagedEnumerateDirectories(searchPattern, searchOption);
		}
		return from x in this.m_directoryInfo.EnumerateDirectories(searchPattern, searchOption)
		select new SdDirectoryInfo(x);
	}

	// Token: 0x06004655 RID: 18005 RVA: 0x001BE747 File Offset: 0x001BC947
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerable<SdDirectoryInfo> ManagedEnumerateDirectories(string searchPattern, SearchOption searchOption)
	{
		return SaveDataUtils.SaveDataManager.ManagedDirectoryInfoEnumerateDirectories(base.ManagedPath, searchPattern, searchOption);
	}

	// Token: 0x06004656 RID: 18006 RVA: 0x001BE75C File Offset: 0x001BC95C
	public IEnumerable<SdFileInfo> EnumerateFiles()
	{
		if (base.IsManaged)
		{
			return this.ManagedEnumerateFiles("*", SearchOption.TopDirectoryOnly);
		}
		return from x in this.m_directoryInfo.EnumerateFiles()
		select new SdFileInfo(x);
	}

	// Token: 0x06004657 RID: 18007 RVA: 0x001BE7B0 File Offset: 0x001BC9B0
	public IEnumerable<SdFileInfo> EnumerateFiles(string searchPattern)
	{
		if (base.IsManaged)
		{
			return this.ManagedEnumerateFiles(searchPattern, SearchOption.TopDirectoryOnly);
		}
		return from x in this.m_directoryInfo.EnumerateFiles(searchPattern)
		select new SdFileInfo(x);
	}

	// Token: 0x06004658 RID: 18008 RVA: 0x001BE800 File Offset: 0x001BCA00
	public IEnumerable<SdFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption)
	{
		if (base.IsManaged)
		{
			return this.ManagedEnumerateFiles(searchPattern, searchOption);
		}
		return from x in this.m_directoryInfo.EnumerateFiles(searchPattern, searchOption)
		select new SdFileInfo(x);
	}

	// Token: 0x06004659 RID: 18009 RVA: 0x001BE84F File Offset: 0x001BCA4F
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerable<SdFileInfo> ManagedEnumerateFiles(string searchPattern, SearchOption searchOption)
	{
		return SaveDataUtils.SaveDataManager.ManagedDirectoryInfoEnumerateFiles(base.ManagedPath, searchPattern, searchOption);
	}

	// Token: 0x0600465A RID: 18010 RVA: 0x001BE863 File Offset: 0x001BCA63
	public IEnumerable<SdFileSystemInfo> EnumerateFileSystemInfos()
	{
		if (base.IsManaged)
		{
			return this.ManagedEnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly);
		}
		return this.m_directoryInfo.EnumerateFileSystemInfos().Select(new Func<FileSystemInfo, SdFileSystemInfo>(SdDirectoryInfo.WrapFileSystemInfo));
	}

	// Token: 0x0600465B RID: 18011 RVA: 0x001BE896 File Offset: 0x001BCA96
	public IEnumerable<SdFileSystemInfo> EnumerateFileSystemInfos(string searchPattern)
	{
		if (base.IsManaged)
		{
			return this.ManagedEnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly);
		}
		return this.m_directoryInfo.EnumerateFileSystemInfos(searchPattern).Select(new Func<FileSystemInfo, SdFileSystemInfo>(SdDirectoryInfo.WrapFileSystemInfo));
	}

	// Token: 0x0600465C RID: 18012 RVA: 0x001BE8C6 File Offset: 0x001BCAC6
	public IEnumerable<SdFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
	{
		if (base.IsManaged)
		{
			return this.ManagedEnumerateFileSystemInfos(searchPattern, searchOption);
		}
		return this.m_directoryInfo.EnumerateFileSystemInfos(searchPattern, searchOption).Select(new Func<FileSystemInfo, SdFileSystemInfo>(SdDirectoryInfo.WrapFileSystemInfo));
	}

	// Token: 0x0600465D RID: 18013 RVA: 0x001BE8F7 File Offset: 0x001BCAF7
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerable<SdFileSystemInfo> ManagedEnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
	{
		return SaveDataUtils.SaveDataManager.ManagedDirectoryInfoEnumerateFileSystemInfos(base.ManagedPath, searchPattern, searchOption);
	}

	// Token: 0x0600465E RID: 18014 RVA: 0x001BE90C File Offset: 0x001BCB0C
	[PublicizedFrom(EAccessModifier.Internal)]
	public static SdFileSystemInfo WrapFileSystemInfo(FileSystemInfo fileSystemInfo)
	{
		FileInfo fileInfo = fileSystemInfo as FileInfo;
		SdFileSystemInfo result;
		if (fileInfo == null)
		{
			DirectoryInfo directoryInfo = fileSystemInfo as DirectoryInfo;
			if (directoryInfo == null)
			{
				throw new NotImplementedException("Unsupported implementation of FileSystemInfo: " + fileSystemInfo.GetType().FullName + ".");
			}
			result = new SdDirectoryInfo(directoryInfo);
		}
		else
		{
			result = new SdFileInfo(fileInfo);
		}
		return result;
	}

	// Token: 0x0600465F RID: 18015 RVA: 0x001BE964 File Offset: 0x001BCB64
	public bool IsDirEmpty()
	{
		bool result;
		using (IEnumerator<SdFileSystemInfo> enumerator = this.EnumerateFileSystemInfos("*", SearchOption.AllDirectories).GetEnumerator())
		{
			result = !enumerator.MoveNext();
		}
		return result;
	}

	// Token: 0x1700075C RID: 1884
	// (get) Token: 0x06004660 RID: 18016 RVA: 0x001BE9AC File Offset: 0x001BCBAC
	public SdDirectoryInfo Root
	{
		get
		{
			return new SdDirectoryInfo(this.m_directoryInfo.Root);
		}
	}

	// Token: 0x06004661 RID: 18017 RVA: 0x001BE9BE File Offset: 0x001BCBBE
	public override void Delete()
	{
		if (base.IsManaged)
		{
			this.ManagedDelete(false);
			return;
		}
		this.m_directoryInfo.Delete();
	}

	// Token: 0x06004662 RID: 18018 RVA: 0x001BE9DB File Offset: 0x001BCBDB
	public void Delete(bool recursive)
	{
		if (base.IsManaged)
		{
			this.ManagedDelete(recursive);
			return;
		}
		this.m_directoryInfo.Delete(recursive);
	}

	// Token: 0x06004663 RID: 18019 RVA: 0x001BE9F9 File Offset: 0x001BCBF9
	[PublicizedFrom(EAccessModifier.Private)]
	public void ManagedDelete(bool recursive)
	{
		SdDirectory.ManagedDelete(base.ManagedPath, recursive);
	}

	// Token: 0x06004664 RID: 18020 RVA: 0x001BEA07 File Offset: 0x001BCC07
	public override string ToString()
	{
		return this.m_directoryInfo.ToString();
	}

	// Token: 0x0400368E RID: 13966
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly DirectoryInfo m_directoryInfo;
}
