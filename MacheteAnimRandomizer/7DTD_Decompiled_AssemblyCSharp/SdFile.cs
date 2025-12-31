using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Token: 0x0200092F RID: 2351
public static class SdFile
{
	// Token: 0x06004670 RID: 18032 RVA: 0x001BEA88 File Offset: 0x001BCC88
	public static StreamReader OpenText(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedOpenText(path2);
		}
		return File.OpenText(path);
	}

	// Token: 0x06004671 RID: 18033 RVA: 0x001BEAAC File Offset: 0x001BCCAC
	[PublicizedFrom(EAccessModifier.Internal)]
	public static StreamReader ManagedOpenText(SaveDataManagedPath path)
	{
		return new StreamReader(SdFile.ManagedOpen(path, FileMode.Open, FileAccess.Read, FileShare.Read));
	}

	// Token: 0x06004672 RID: 18034 RVA: 0x001BEABC File Offset: 0x001BCCBC
	public static StreamWriter CreateText(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedCreateText(path2);
		}
		return File.CreateText(path);
	}

	// Token: 0x06004673 RID: 18035 RVA: 0x001BEAE0 File Offset: 0x001BCCE0
	[PublicizedFrom(EAccessModifier.Internal)]
	public static StreamWriter ManagedCreateText(SaveDataManagedPath path)
	{
		return new StreamWriter(SdFile.ManagedOpen(path, FileMode.Create, FileAccess.Write, FileShare.Read));
	}

	// Token: 0x06004674 RID: 18036 RVA: 0x001BEAF0 File Offset: 0x001BCCF0
	public static StreamWriter AppendText(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedAppendText(path2);
		}
		return File.AppendText(path);
	}

	// Token: 0x06004675 RID: 18037 RVA: 0x001BEB14 File Offset: 0x001BCD14
	[PublicizedFrom(EAccessModifier.Internal)]
	public static StreamWriter ManagedAppendText(SaveDataManagedPath path)
	{
		return new StreamWriter(SdFile.ManagedOpen(path, FileMode.Append, FileAccess.Write, FileShare.Read));
	}

	// Token: 0x06004676 RID: 18038 RVA: 0x001BEB24 File Offset: 0x001BCD24
	public static void Copy(string sourceFileName, string destFileName)
	{
		SaveDataManagedPath sourceFileName2;
		bool flag = SaveDataUtils.TryGetManagedPath(sourceFileName, out sourceFileName2);
		SaveDataManagedPath destFileName2;
		bool flag2 = SaveDataUtils.TryGetManagedPath(destFileName, out destFileName2);
		if (flag && flag2)
		{
			SdFile.ManagedToManagedCopy(sourceFileName2, destFileName2, false);
			return;
		}
		if (flag)
		{
			SdFile.ManagedToUnmanagedCopy(sourceFileName2, destFileName, false);
			return;
		}
		if (flag2)
		{
			SdFile.UnmanagedToManagedCopy(sourceFileName, destFileName2, false);
			return;
		}
		File.Copy(sourceFileName, destFileName);
	}

	// Token: 0x06004677 RID: 18039 RVA: 0x001BEB70 File Offset: 0x001BCD70
	public static void Copy(string sourceFileName, string destFileName, bool overwrite)
	{
		SaveDataManagedPath sourceFileName2;
		bool flag = SaveDataUtils.TryGetManagedPath(sourceFileName, out sourceFileName2);
		SaveDataManagedPath destFileName2;
		bool flag2 = SaveDataUtils.TryGetManagedPath(destFileName, out destFileName2);
		if (flag && flag2)
		{
			SdFile.ManagedToManagedCopy(sourceFileName2, destFileName2, overwrite);
			return;
		}
		if (flag)
		{
			SdFile.ManagedToUnmanagedCopy(sourceFileName2, destFileName, overwrite);
			return;
		}
		if (flag2)
		{
			SdFile.UnmanagedToManagedCopy(sourceFileName, destFileName2, overwrite);
			return;
		}
		File.Copy(sourceFileName, destFileName, overwrite);
	}

	// Token: 0x06004678 RID: 18040 RVA: 0x001BEBC0 File Offset: 0x001BCDC0
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void ManagedToManagedCopy(SaveDataManagedPath sourceFileName, SaveDataManagedPath destFileName, bool overwrite)
	{
		using (Stream stream = SdFile.ManagedOpen(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
		{
			using (Stream stream2 = SdFile.ManagedOpen(destFileName, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.Read))
			{
				StreamUtils.StreamCopy(stream, stream2, null, true);
			}
		}
	}

	// Token: 0x06004679 RID: 18041 RVA: 0x001BEC24 File Offset: 0x001BCE24
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void ManagedToUnmanagedCopy(SaveDataManagedPath sourceFileName, string destFileName, bool overwrite)
	{
		using (Stream stream = SdFile.ManagedOpen(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
		{
			using (FileStream fileStream = File.Open(destFileName, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.Read))
			{
				StreamUtils.StreamCopy(stream, fileStream, null, true);
			}
		}
	}

	// Token: 0x0600467A RID: 18042 RVA: 0x001BEC88 File Offset: 0x001BCE88
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void UnmanagedToManagedCopy(string sourceFileName, SaveDataManagedPath destFileName, bool overwrite)
	{
		using (FileStream fileStream = File.Open(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
		{
			using (Stream stream = SdFile.ManagedOpen(destFileName, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.Read))
			{
				StreamUtils.StreamCopy(fileStream, stream, null, true);
			}
		}
	}

	// Token: 0x0600467B RID: 18043 RVA: 0x001BECEC File Offset: 0x001BCEEC
	public static Stream Create(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedCreate(path2);
		}
		return File.Create(path);
	}

	// Token: 0x0600467C RID: 18044 RVA: 0x001BED10 File Offset: 0x001BCF10
	[PublicizedFrom(EAccessModifier.Internal)]
	public static Stream ManagedCreate(SaveDataManagedPath path)
	{
		return SdFile.ManagedOpen(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
	}

	// Token: 0x0600467D RID: 18045 RVA: 0x001BED1C File Offset: 0x001BCF1C
	public static void Delete(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			SdFile.ManagedDelete(path2);
			return;
		}
		File.Delete(path);
	}

	// Token: 0x0600467E RID: 18046 RVA: 0x001BED40 File Offset: 0x001BCF40
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void ManagedDelete(SaveDataManagedPath path)
	{
		SaveDataUtils.SaveDataManager.ManagedFileDelete(path);
	}

	// Token: 0x0600467F RID: 18047 RVA: 0x001BED50 File Offset: 0x001BCF50
	public static bool Exists(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedExists(path2);
		}
		return File.Exists(path);
	}

	// Token: 0x06004680 RID: 18048 RVA: 0x001BED74 File Offset: 0x001BCF74
	[PublicizedFrom(EAccessModifier.Internal)]
	public static bool ManagedExists(SaveDataManagedPath path)
	{
		return SaveDataUtils.SaveDataManager.ManagedFileExists(path);
	}

	// Token: 0x06004681 RID: 18049 RVA: 0x001BED84 File Offset: 0x001BCF84
	public static Stream Open(string path, FileMode mode)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedOpen(path2, mode, (mode == FileMode.Append) ? FileAccess.Write : FileAccess.ReadWrite, FileShare.None);
		}
		return File.Open(path, mode);
	}

	// Token: 0x06004682 RID: 18050 RVA: 0x001BEDB4 File Offset: 0x001BCFB4
	public static Stream Open(string path, FileMode mode, FileAccess access)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedOpen(path2, mode, access, FileShare.None);
		}
		return File.Open(path, mode, access);
	}

	// Token: 0x06004683 RID: 18051 RVA: 0x001BEDE0 File Offset: 0x001BCFE0
	public static Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedOpen(path2, mode, access, share);
		}
		return File.Open(path, mode, access, share);
	}

	// Token: 0x06004684 RID: 18052 RVA: 0x001BEE0A File Offset: 0x001BD00A
	[PublicizedFrom(EAccessModifier.Internal)]
	public static Stream ManagedOpen(SaveDataManagedPath path, FileMode mode, FileAccess access, FileShare share)
	{
		return SaveDataUtils.SaveDataManager.ManagedFileOpen(path, mode, access, share);
	}

	// Token: 0x06004685 RID: 18053 RVA: 0x001BEE1C File Offset: 0x001BD01C
	public static DateTime GetLastWriteTime(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedGetLastWriteTime(path2);
		}
		return File.GetLastWriteTime(path);
	}

	// Token: 0x06004686 RID: 18054 RVA: 0x001BEE40 File Offset: 0x001BD040
	[PublicizedFrom(EAccessModifier.Private)]
	public static DateTime ManagedGetLastWriteTime(SaveDataManagedPath path)
	{
		return SdFile.ManagedGetLastWriteTimeUtc(path).ToLocalTime();
	}

	// Token: 0x06004687 RID: 18055 RVA: 0x001BEE5C File Offset: 0x001BD05C
	public static DateTime GetLastWriteTimeUtc(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedGetLastWriteTimeUtc(path2);
		}
		return File.GetLastWriteTimeUtc(path);
	}

	// Token: 0x06004688 RID: 18056 RVA: 0x001BEE80 File Offset: 0x001BD080
	[PublicizedFrom(EAccessModifier.Internal)]
	public static DateTime ManagedGetLastWriteTimeUtc(SaveDataManagedPath path)
	{
		return SaveDataUtils.SaveDataManager.ManagedFileGetLastWriteTimeUtc(path);
	}

	// Token: 0x06004689 RID: 18057 RVA: 0x001BEE90 File Offset: 0x001BD090
	public static Stream OpenRead(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedOpen(path2, FileMode.Open, FileAccess.Read, FileShare.Read);
		}
		return File.OpenRead(path);
	}

	// Token: 0x0600468A RID: 18058 RVA: 0x001BEEB8 File Offset: 0x001BD0B8
	public static Stream OpenWrite(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedOpen(path2, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
		}
		return File.OpenWrite(path);
	}

	// Token: 0x0600468B RID: 18059 RVA: 0x001BEEE0 File Offset: 0x001BD0E0
	public static string ReadAllText(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedReadAllText(path2, Encoding.UTF8);
		}
		return File.ReadAllText(path);
	}

	// Token: 0x0600468C RID: 18060 RVA: 0x001BEF0C File Offset: 0x001BD10C
	public static string ReadAllText(string path, Encoding encoding)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedReadAllText(path2, encoding);
		}
		return File.ReadAllText(path, encoding);
	}

	// Token: 0x0600468D RID: 18061 RVA: 0x001BEF34 File Offset: 0x001BD134
	[PublicizedFrom(EAccessModifier.Internal)]
	public static string ManagedReadAllText(SaveDataManagedPath path, Encoding encoding)
	{
		string result;
		using (Stream stream = SdFile.ManagedOpen(path, FileMode.Open, FileAccess.Read, FileShare.Read))
		{
			using (StreamReader streamReader = new StreamReader(stream, encoding))
			{
				result = streamReader.ReadToEnd();
			}
		}
		return result;
	}

	// Token: 0x0600468E RID: 18062 RVA: 0x001BEF90 File Offset: 0x001BD190
	public static void WriteAllText(string path, string contents)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			SdFile.ManagedWriteAllText(path2, contents, SdFile.UTF8NoBom);
			return;
		}
		File.WriteAllText(path, contents);
	}

	// Token: 0x0600468F RID: 18063 RVA: 0x001BEFBC File Offset: 0x001BD1BC
	public static void WriteAllText(string path, string contents, Encoding encoding)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			SdFile.ManagedWriteAllText(path2, contents, encoding);
			return;
		}
		File.WriteAllText(path, contents, encoding);
	}

	// Token: 0x06004690 RID: 18064 RVA: 0x001BEFE4 File Offset: 0x001BD1E4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ManagedWriteAllText(SaveDataManagedPath path, string contents, Encoding encoding)
	{
		using (Stream stream = SdFile.ManagedOpen(path, FileMode.Create, FileAccess.Write, FileShare.Read))
		{
			using (StreamWriter streamWriter = new StreamWriter(stream, encoding))
			{
				streamWriter.Write(contents);
			}
		}
	}

	// Token: 0x06004691 RID: 18065 RVA: 0x001BF03C File Offset: 0x001BD23C
	public static byte[] ReadAllBytes(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedReadAllBytes(path2);
		}
		return File.ReadAllBytes(path);
	}

	// Token: 0x06004692 RID: 18066 RVA: 0x001BF060 File Offset: 0x001BD260
	[PublicizedFrom(EAccessModifier.Private)]
	public static byte[] ManagedReadAllBytes(SaveDataManagedPath path)
	{
		byte[] result;
		using (PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true))
		{
			using (Stream stream = SdFile.ManagedOpen(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				stream.CopyTo(pooledExpandableMemoryStream);
				result = pooledExpandableMemoryStream.ToArray();
			}
		}
		return result;
	}

	// Token: 0x06004693 RID: 18067 RVA: 0x001BF0C4 File Offset: 0x001BD2C4
	public static void WriteAllBytes(string path, byte[] bytes)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			SdFile.ManagedWriteAllBytes(path2, bytes);
			return;
		}
		File.WriteAllBytes(path, bytes);
	}

	// Token: 0x06004694 RID: 18068 RVA: 0x001BF0EC File Offset: 0x001BD2EC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ManagedWriteAllBytes(SaveDataManagedPath path, byte[] bytes)
	{
		using (Stream stream = SdFile.ManagedOpen(path, FileMode.Create, FileAccess.Write, FileShare.Read))
		{
			stream.Write(bytes, 0, bytes.Length);
		}
	}

	// Token: 0x06004695 RID: 18069 RVA: 0x001BF12C File Offset: 0x001BD32C
	public static string[] ReadAllLines(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedReadAllLines(path2, Encoding.UTF8);
		}
		return File.ReadAllLines(path);
	}

	// Token: 0x06004696 RID: 18070 RVA: 0x001BF158 File Offset: 0x001BD358
	public static string[] ReadAllLines(string path, Encoding encoding)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedReadAllLines(path2, encoding);
		}
		return File.ReadAllLines(path, encoding);
	}

	// Token: 0x06004697 RID: 18071 RVA: 0x001BF180 File Offset: 0x001BD380
	[PublicizedFrom(EAccessModifier.Private)]
	public static string[] ManagedReadAllLines(SaveDataManagedPath path, Encoding encoding)
	{
		List<string> list = new List<string>();
		string[] result;
		using (Stream stream = SdFile.ManagedOpen(path, FileMode.Open, FileAccess.Read, FileShare.Read))
		{
			using (StreamReader streamReader = new StreamReader(stream, encoding))
			{
				for (;;)
				{
					string text = streamReader.ReadLine();
					if (text == null)
					{
						break;
					}
					list.Add(text);
				}
				result = list.ToArray();
			}
		}
		return result;
	}

	// Token: 0x06004698 RID: 18072 RVA: 0x001BF1F4 File Offset: 0x001BD3F4
	public static IEnumerable<string> ReadLines(string path)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedReadLines(path2, Encoding.UTF8);
		}
		return File.ReadLines(path);
	}

	// Token: 0x06004699 RID: 18073 RVA: 0x001BF220 File Offset: 0x001BD420
	public static IEnumerable<string> ReadLines(string path, Encoding encoding)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			return SdFile.ManagedReadLines(path2, encoding);
		}
		return File.ReadLines(path, encoding);
	}

	// Token: 0x0600469A RID: 18074 RVA: 0x001BF246 File Offset: 0x001BD446
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerable<string> ManagedReadLines(SaveDataManagedPath path, Encoding encoding)
	{
		SdFile.<ManagedReadLines>d__43 <ManagedReadLines>d__ = new SdFile.<ManagedReadLines>d__43(-2);
		<ManagedReadLines>d__.<>3__path = path;
		<ManagedReadLines>d__.<>3__encoding = encoding;
		return <ManagedReadLines>d__;
	}

	// Token: 0x0600469B RID: 18075 RVA: 0x001BF260 File Offset: 0x001BD460
	public static void WriteAllLines(string path, string[] contents)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			SdFile.ManagedWriteAllLines(path2, contents, SdFile.UTF8NoBom);
			return;
		}
		File.WriteAllLines(path, contents);
	}

	// Token: 0x0600469C RID: 18076 RVA: 0x001BF28C File Offset: 0x001BD48C
	public static void WriteAllLines(string path, string[] contents, Encoding encoding)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			SdFile.ManagedWriteAllLines(path2, contents, encoding);
			return;
		}
		File.WriteAllLines(path, contents, encoding);
	}

	// Token: 0x0600469D RID: 18077 RVA: 0x001BF2B4 File Offset: 0x001BD4B4
	public static void WriteAllLines(string path, IEnumerable<string> contents)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			SdFile.ManagedWriteAllLines(path2, contents, SdFile.UTF8NoBom);
			return;
		}
		File.WriteAllLines(path, contents);
	}

	// Token: 0x0600469E RID: 18078 RVA: 0x001BF2E0 File Offset: 0x001BD4E0
	public static void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			SdFile.ManagedWriteAllLines(path2, contents, encoding);
			return;
		}
		File.WriteAllLines(path, contents, encoding);
	}

	// Token: 0x0600469F RID: 18079 RVA: 0x001BF308 File Offset: 0x001BD508
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ManagedWriteAllLines(SaveDataManagedPath path, IEnumerable<string> contents, Encoding encoding)
	{
		using (Stream stream = SdFile.ManagedOpen(path, FileMode.Create, FileAccess.Write, FileShare.Read))
		{
			using (StreamWriter streamWriter = new StreamWriter(stream, encoding))
			{
				SdFile.ManagedWriteAllLines(streamWriter, contents);
			}
		}
	}

	// Token: 0x060046A0 RID: 18080 RVA: 0x001BF360 File Offset: 0x001BD560
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ManagedWriteAllLines(TextWriter writer, IEnumerable<string> contents)
	{
		try
		{
			foreach (string value in contents)
			{
				writer.WriteLine(value);
			}
		}
		finally
		{
			if (writer != null)
			{
				((IDisposable)writer).Dispose();
			}
		}
	}

	// Token: 0x060046A1 RID: 18081 RVA: 0x001BF3C0 File Offset: 0x001BD5C0
	public static void AppendAllText(string path, string contents)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			SdFile.ManagedAppendAllText(path2, contents, SdFile.UTF8NoBom);
			return;
		}
		File.AppendAllText(path, contents);
	}

	// Token: 0x060046A2 RID: 18082 RVA: 0x001BF3EC File Offset: 0x001BD5EC
	public static void AppendAllText(string path, string contents, Encoding encoding)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			SdFile.ManagedAppendAllText(path2, contents, encoding);
			return;
		}
		File.AppendAllText(path, contents, encoding);
	}

	// Token: 0x060046A3 RID: 18083 RVA: 0x001BF414 File Offset: 0x001BD614
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ManagedAppendAllText(SaveDataManagedPath path, string contents, Encoding encoding)
	{
		using (Stream stream = SdFile.ManagedOpen(path, FileMode.Append, FileAccess.Write, FileShare.Read))
		{
			using (StreamWriter streamWriter = new StreamWriter(stream, encoding))
			{
				streamWriter.Write(contents);
			}
		}
	}

	// Token: 0x060046A4 RID: 18084 RVA: 0x001BF46C File Offset: 0x001BD66C
	public static void AppendAllLines(string path, IEnumerable<string> contents)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			SdFile.ManagedAppendAllLines(path2, contents, SdFile.UTF8NoBom);
			return;
		}
		File.AppendAllLines(path, contents);
	}

	// Token: 0x060046A5 RID: 18085 RVA: 0x001BF498 File Offset: 0x001BD698
	public static void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding)
	{
		SaveDataManagedPath path2;
		if (SaveDataUtils.TryGetManagedPath(path, out path2))
		{
			SdFile.ManagedAppendAllLines(path2, contents, encoding);
			return;
		}
		File.AppendAllLines(path, contents, encoding);
	}

	// Token: 0x060046A6 RID: 18086 RVA: 0x001BF4C0 File Offset: 0x001BD6C0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ManagedAppendAllLines(SaveDataManagedPath path, IEnumerable<string> contents, Encoding encoding)
	{
		using (Stream stream = SdFile.ManagedOpen(path, FileMode.Append, FileAccess.Write, FileShare.Read))
		{
			using (StreamWriter streamWriter = new StreamWriter(stream, encoding))
			{
				SdFile.ManagedWriteAllLines(streamWriter, contents);
			}
		}
	}

	// Token: 0x04003697 RID: 13975
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Encoding UTF8NoBom = new UTF8Encoding(false);
}
