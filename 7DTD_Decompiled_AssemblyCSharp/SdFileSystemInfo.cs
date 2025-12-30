using System;
using System.IO;

// Token: 0x02000932 RID: 2354
public abstract class SdFileSystemInfo
{
	// Token: 0x17000764 RID: 1892
	// (get) Token: 0x060046C9 RID: 18121 RVA: 0x001BF9F3 File Offset: 0x001BDBF3
	// (set) Token: 0x060046CA RID: 18122 RVA: 0x001BF9FB File Offset: 0x001BDBFB
	public bool IsManaged { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000765 RID: 1893
	// (get) Token: 0x060046CB RID: 18123 RVA: 0x001BFA04 File Offset: 0x001BDC04
	// (set) Token: 0x060046CC RID: 18124 RVA: 0x001BFA0C File Offset: 0x001BDC0C
	public SaveDataManagedPath ManagedPath { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x060046CD RID: 18125 RVA: 0x001BFA15 File Offset: 0x001BDC15
	[PublicizedFrom(EAccessModifier.Protected)]
	public SdFileSystemInfo(FileSystemInfo fileSystemInfo)
	{
		this.Reinitialize(fileSystemInfo);
	}

	// Token: 0x060046CE RID: 18126 RVA: 0x001BFA24 File Offset: 0x001BDC24
	[PublicizedFrom(EAccessModifier.Protected)]
	public SdFileSystemInfo(FileSystemInfo fileSystemInfo, SaveDataManagedPath managedPath)
	{
		this.m_fileSystemInfo = fileSystemInfo;
		this.IsManaged = true;
		this.ManagedPath = managedPath;
	}

	// Token: 0x060046CF RID: 18127 RVA: 0x001BFA44 File Offset: 0x001BDC44
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Reinitialize(FileSystemInfo fileSystemInfo)
	{
		this.m_fileSystemInfo = fileSystemInfo;
		SaveDataManagedPath managedPath;
		this.IsManaged = SaveDataUtils.TryGetManagedPath(fileSystemInfo.FullName, out managedPath);
		this.ManagedPath = managedPath;
	}

	// Token: 0x17000766 RID: 1894
	// (get) Token: 0x060046D0 RID: 18128 RVA: 0x001BFA72 File Offset: 0x001BDC72
	public virtual string FullName
	{
		get
		{
			return this.m_fileSystemInfo.FullName;
		}
	}

	// Token: 0x17000767 RID: 1895
	// (get) Token: 0x060046D1 RID: 18129 RVA: 0x001BFA7F File Offset: 0x001BDC7F
	public string Extension
	{
		get
		{
			return this.m_fileSystemInfo.Extension;
		}
	}

	// Token: 0x17000768 RID: 1896
	// (get) Token: 0x060046D2 RID: 18130
	public abstract string Name { get; }

	// Token: 0x17000769 RID: 1897
	// (get) Token: 0x060046D3 RID: 18131
	public abstract bool Exists { get; }

	// Token: 0x060046D4 RID: 18132
	public abstract void Delete();

	// Token: 0x1700076A RID: 1898
	// (get) Token: 0x060046D5 RID: 18133 RVA: 0x001BFA8C File Offset: 0x001BDC8C
	public DateTime LastWriteTime
	{
		get
		{
			return this.LastWriteTimeUtc.ToLocalTime();
		}
	}

	// Token: 0x1700076B RID: 1899
	// (get) Token: 0x060046D6 RID: 18134 RVA: 0x001BFAA7 File Offset: 0x001BDCA7
	public DateTime LastWriteTimeUtc
	{
		get
		{
			if (!this.IsManaged)
			{
				return this.m_fileSystemInfo.LastWriteTimeUtc;
			}
			if (this.m_fileSystemInfo is DirectoryInfo)
			{
				return SdDirectory.ManagedGetLastWriteTimeUtc(this.ManagedPath);
			}
			return SdFile.ManagedGetLastWriteTimeUtc(this.ManagedPath);
		}
	}

	// Token: 0x060046D7 RID: 18135 RVA: 0x001BFAE1 File Offset: 0x001BDCE1
	public void Refresh()
	{
		this.m_fileSystemInfo.Refresh();
		this.Reinitialize(this.m_fileSystemInfo);
	}

	// Token: 0x040036A2 RID: 13986
	[PublicizedFrom(EAccessModifier.Private)]
	public FileSystemInfo m_fileSystemInfo;
}
