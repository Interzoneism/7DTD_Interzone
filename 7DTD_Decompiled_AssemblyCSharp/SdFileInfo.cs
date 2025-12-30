using System;
using System.IO;

// Token: 0x02000931 RID: 2353
public class SdFileInfo : SdFileSystemInfo
{
	// Token: 0x060046B2 RID: 18098 RVA: 0x001BF6EB File Offset: 0x001BD8EB
	public SdFileInfo(string fileName) : this(new FileInfo(fileName))
	{
	}

	// Token: 0x060046B3 RID: 18099 RVA: 0x001BF6F9 File Offset: 0x001BD8F9
	[PublicizedFrom(EAccessModifier.Internal)]
	public SdFileInfo(SaveDataManagedPath managedPath) : this(new FileInfo(managedPath.GetOriginalPath()), managedPath)
	{
	}

	// Token: 0x060046B4 RID: 18100 RVA: 0x001BF70D File Offset: 0x001BD90D
	[PublicizedFrom(EAccessModifier.Internal)]
	public SdFileInfo(FileInfo fileInfo) : base(fileInfo)
	{
		this.m_fileInfo = fileInfo;
	}

	// Token: 0x060046B5 RID: 18101 RVA: 0x001BF71D File Offset: 0x001BD91D
	[PublicizedFrom(EAccessModifier.Private)]
	public SdFileInfo(FileInfo fileInfo, SaveDataManagedPath managedPath) : base(fileInfo, managedPath)
	{
		this.m_fileInfo = fileInfo;
	}

	// Token: 0x060046B6 RID: 18102 RVA: 0x001BF72E File Offset: 0x001BD92E
	[PublicizedFrom(EAccessModifier.Private)]
	public void Reinitialize(FileInfo fileInfo)
	{
		base.Reinitialize(fileInfo);
		this.m_fileInfo = fileInfo;
	}

	// Token: 0x1700075F RID: 1887
	// (get) Token: 0x060046B7 RID: 18103 RVA: 0x001BF73E File Offset: 0x001BD93E
	public override string Name
	{
		get
		{
			return this.m_fileInfo.Name;
		}
	}

	// Token: 0x17000760 RID: 1888
	// (get) Token: 0x060046B8 RID: 18104 RVA: 0x001BF74B File Offset: 0x001BD94B
	public long Length
	{
		get
		{
			if (base.IsManaged)
			{
				return SaveDataUtils.SaveDataManager.ManagedFileInfoLength(base.ManagedPath);
			}
			return this.m_fileInfo.Length;
		}
	}

	// Token: 0x17000761 RID: 1889
	// (get) Token: 0x060046B9 RID: 18105 RVA: 0x001BF771 File Offset: 0x001BD971
	public string DirectoryName
	{
		get
		{
			return this.m_fileInfo.DirectoryName;
		}
	}

	// Token: 0x17000762 RID: 1890
	// (get) Token: 0x060046BA RID: 18106 RVA: 0x001BF77E File Offset: 0x001BD97E
	public SdDirectoryInfo Directory
	{
		get
		{
			return new SdDirectoryInfo(this.m_fileInfo.Directory);
		}
	}

	// Token: 0x060046BB RID: 18107 RVA: 0x001BF790 File Offset: 0x001BD990
	public StreamReader OpenText()
	{
		if (!base.IsManaged)
		{
			return this.m_fileInfo.OpenText();
		}
		return SdFile.ManagedOpenText(base.ManagedPath);
	}

	// Token: 0x060046BC RID: 18108 RVA: 0x001BF7B1 File Offset: 0x001BD9B1
	public StreamWriter CreateText()
	{
		if (!base.IsManaged)
		{
			return this.m_fileInfo.CreateText();
		}
		return SdFile.ManagedCreateText(base.ManagedPath);
	}

	// Token: 0x060046BD RID: 18109 RVA: 0x001BF7D2 File Offset: 0x001BD9D2
	public StreamWriter AppendText()
	{
		if (!base.IsManaged)
		{
			return this.m_fileInfo.AppendText();
		}
		return SdFile.ManagedAppendText(base.ManagedPath);
	}

	// Token: 0x060046BE RID: 18110 RVA: 0x001BF7F4 File Offset: 0x001BD9F4
	public SdFileInfo CopyTo(string destFileName)
	{
		bool isManaged = base.IsManaged;
		SaveDataManagedPath destFileName2;
		bool flag = SaveDataUtils.TryGetManagedPath(destFileName, out destFileName2);
		if (isManaged && flag)
		{
			SdFile.ManagedToManagedCopy(base.ManagedPath, destFileName2, false);
		}
		else if (isManaged)
		{
			SdFile.ManagedToUnmanagedCopy(base.ManagedPath, destFileName, false);
		}
		else if (flag)
		{
			SdFile.UnmanagedToManagedCopy(this.FullName, destFileName2, false);
		}
		else
		{
			this.m_fileInfo.CopyTo(destFileName, false);
		}
		return new SdFileInfo(destFileName);
	}

	// Token: 0x060046BF RID: 18111 RVA: 0x001BF860 File Offset: 0x001BDA60
	public SdFileInfo CopyTo(string destFileName, bool overwrite)
	{
		bool isManaged = base.IsManaged;
		SaveDataManagedPath destFileName2;
		bool flag = SaveDataUtils.TryGetManagedPath(destFileName, out destFileName2);
		if (isManaged && flag)
		{
			SdFile.ManagedToManagedCopy(base.ManagedPath, destFileName2, overwrite);
		}
		else if (isManaged)
		{
			SdFile.ManagedToUnmanagedCopy(base.ManagedPath, destFileName, overwrite);
		}
		else if (flag)
		{
			SdFile.UnmanagedToManagedCopy(this.FullName, destFileName2, overwrite);
		}
		else
		{
			this.m_fileInfo.CopyTo(destFileName, overwrite);
		}
		return new SdFileInfo(destFileName);
	}

	// Token: 0x060046C0 RID: 18112 RVA: 0x001BF8C9 File Offset: 0x001BDAC9
	public Stream Create()
	{
		if (!base.IsManaged)
		{
			return this.m_fileInfo.Create();
		}
		return SdFile.ManagedCreate(base.ManagedPath);
	}

	// Token: 0x060046C1 RID: 18113 RVA: 0x001BF8EA File Offset: 0x001BDAEA
	public override void Delete()
	{
		if (base.IsManaged)
		{
			SdFile.ManagedDelete(base.ManagedPath);
			return;
		}
		this.m_fileInfo.Delete();
	}

	// Token: 0x17000763 RID: 1891
	// (get) Token: 0x060046C2 RID: 18114 RVA: 0x001BF90B File Offset: 0x001BDB0B
	public override bool Exists
	{
		get
		{
			if (!base.IsManaged)
			{
				return this.m_fileInfo.Exists;
			}
			return SdFile.ManagedExists(base.ManagedPath);
		}
	}

	// Token: 0x060046C3 RID: 18115 RVA: 0x001BF92C File Offset: 0x001BDB2C
	public Stream Open(FileMode mode)
	{
		if (base.IsManaged)
		{
			return SdFile.ManagedOpen(base.ManagedPath, mode, FileAccess.ReadWrite, FileShare.None);
		}
		return this.m_fileInfo.Open(mode);
	}

	// Token: 0x060046C4 RID: 18116 RVA: 0x001BF951 File Offset: 0x001BDB51
	public Stream Open(FileMode mode, FileAccess access)
	{
		if (base.IsManaged)
		{
			return SdFile.ManagedOpen(base.ManagedPath, mode, access, FileShare.None);
		}
		return this.m_fileInfo.Open(mode, access);
	}

	// Token: 0x060046C5 RID: 18117 RVA: 0x001BF977 File Offset: 0x001BDB77
	public Stream Open(FileMode mode, FileAccess access, FileShare share)
	{
		if (base.IsManaged)
		{
			return SdFile.ManagedOpen(base.ManagedPath, mode, access, share);
		}
		return this.m_fileInfo.Open(mode, access, share);
	}

	// Token: 0x060046C6 RID: 18118 RVA: 0x001BF99E File Offset: 0x001BDB9E
	public Stream OpenRead()
	{
		if (base.IsManaged)
		{
			return SdFile.ManagedOpen(base.ManagedPath, FileMode.Open, FileAccess.Read, FileShare.Read);
		}
		return this.m_fileInfo.OpenRead();
	}

	// Token: 0x060046C7 RID: 18119 RVA: 0x001BF9C2 File Offset: 0x001BDBC2
	public Stream OpenWrite()
	{
		if (base.IsManaged)
		{
			return SdFile.ManagedOpen(base.ManagedPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
		}
		return this.m_fileInfo.OpenWrite();
	}

	// Token: 0x060046C8 RID: 18120 RVA: 0x001BF9E6 File Offset: 0x001BDBE6
	public override string ToString()
	{
		return this.m_fileInfo.ToString();
	}

	// Token: 0x040036A1 RID: 13985
	[PublicizedFrom(EAccessModifier.Private)]
	public FileInfo m_fileInfo;
}
