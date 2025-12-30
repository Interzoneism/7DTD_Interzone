using System;

// Token: 0x0200127A RID: 4730
public static class MetricConversion
{
	// Token: 0x06009411 RID: 37905 RVA: 0x003B1694 File Offset: 0x003AF894
	public static string ToShortestBytesString(long bytes)
	{
		if (bytes < 1024L)
		{
			return string.Format("{0} B", bytes);
		}
		if (bytes < 1048576L)
		{
			return string.Format("{0:F2} KB", 0.0009765625 * (double)bytes);
		}
		if (bytes < 1073741824L)
		{
			return string.Format("{0:F2} MB", 9.5367431640625E-07 * (double)bytes);
		}
		return string.Format("{0:F2} GB", 9.313225746154785E-10 * (double)bytes);
	}

	// Token: 0x0400713F RID: 28991
	public const double nanoToMilli = 1E-06;

	// Token: 0x04007140 RID: 28992
	public const int kilobyte = 1024;

	// Token: 0x04007141 RID: 28993
	public const int megabyte = 1048576;

	// Token: 0x04007142 RID: 28994
	public const int gigabyte = 1073741824;

	// Token: 0x04007143 RID: 28995
	public const double bytesToKilobyte = 0.0009765625;

	// Token: 0x04007144 RID: 28996
	public const double bytesToMegabyte = 9.5367431640625E-07;

	// Token: 0x04007145 RID: 28997
	public const double bytesToGigabyte = 9.313225746154785E-10;
}
