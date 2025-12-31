using System;

// Token: 0x02001214 RID: 4628
public static class SizeUtils
{
	// Token: 0x06009062 RID: 36962 RVA: 0x0039931C File Offset: 0x0039751C
	public static string FormatSize(this int sizeBytes, bool includeOriginalBytes = false)
	{
		if (Math.Abs(sizeBytes) < 1024)
		{
			return string.Format("{0} B", sizeBytes);
		}
		double num = (double)sizeBytes / 1024.0;
		string arg = "kB";
		if (Math.Abs(num) >= 1024.0)
		{
			num /= 1024.0;
			arg = "MB";
		}
		if (Math.Abs(num) >= 1024.0)
		{
			num /= 1024.0;
			arg = "GB";
		}
		if (!includeOriginalBytes)
		{
			return string.Format("{0:F3} {1}", num, arg);
		}
		return string.Format("{0:F3} {1} ({2} B)", num, arg, sizeBytes);
	}

	// Token: 0x06009063 RID: 36963 RVA: 0x003993CC File Offset: 0x003975CC
	public static string FormatSize(this uint sizeBytes, bool includeOriginalBytes = false)
	{
		if (sizeBytes < 1024U)
		{
			return string.Format("{0} B", sizeBytes);
		}
		double num = sizeBytes / 1024.0;
		string arg = "kB";
		if (num >= 1024.0)
		{
			num /= 1024.0;
			arg = "MB";
		}
		if (num >= 1024.0)
		{
			num /= 1024.0;
			arg = "GB";
		}
		if (!includeOriginalBytes)
		{
			return string.Format("{0:F3} {1}", num, arg);
		}
		return string.Format("{0:F3} {1} ({2} B)", num, arg, sizeBytes);
	}

	// Token: 0x06009064 RID: 36964 RVA: 0x00399470 File Offset: 0x00397670
	public static string FormatSize(this long sizeBytes, bool includeOriginalBytes = false)
	{
		if (Math.Abs(sizeBytes) < 1024L)
		{
			return string.Format("{0} B", sizeBytes);
		}
		double num = (double)sizeBytes / 1024.0;
		string arg = "kB";
		if (Math.Abs(num) >= 1024.0)
		{
			num /= 1024.0;
			arg = "MB";
		}
		if (Math.Abs(num) >= 1024.0)
		{
			num /= 1024.0;
			arg = "GB";
		}
		if (Math.Abs(num) >= 1024.0)
		{
			num /= 1024.0;
			arg = "TB";
		}
		if (Math.Abs(num) >= 1024.0)
		{
			num /= 1024.0;
			arg = "PB";
		}
		if (Math.Abs(num) >= 1024.0)
		{
			num /= 1024.0;
			arg = "EB";
		}
		if (!includeOriginalBytes)
		{
			return string.Format("{0:F3} {1}", num, arg);
		}
		return string.Format("{0:F3} {1} ({2} B)", num, arg, sizeBytes);
	}

	// Token: 0x06009065 RID: 36965 RVA: 0x0039958C File Offset: 0x0039778C
	public static string FormatSize(this ulong sizeBytes, bool includeOriginalBytes = false)
	{
		if (sizeBytes < 1024UL)
		{
			return string.Format("{0} B", sizeBytes);
		}
		double num = sizeBytes / 1024.0;
		string arg = "kB";
		if (num >= 1024.0)
		{
			num /= 1024.0;
			arg = "MB";
		}
		if (num >= 1024.0)
		{
			num /= 1024.0;
			arg = "GB";
		}
		if (num >= 1024.0)
		{
			num /= 1024.0;
			arg = "TB";
		}
		if (num >= 1024.0)
		{
			num /= 1024.0;
			arg = "PB";
		}
		if (num >= 1024.0)
		{
			num /= 1024.0;
			arg = "EB";
		}
		if (!includeOriginalBytes)
		{
			return string.Format("{0:F3} {1}", num, arg);
		}
		return string.Format("{0:F3} {1} ({2} B)", num, arg, sizeBytes);
	}
}
