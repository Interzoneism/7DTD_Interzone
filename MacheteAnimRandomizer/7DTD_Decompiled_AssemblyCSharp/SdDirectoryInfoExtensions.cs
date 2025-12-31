using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x0200092D RID: 2349
public static class SdDirectoryInfoExtensions
{
	// Token: 0x0600466D RID: 18029 RVA: 0x001BEA30 File Offset: 0x001BCC30
	public static bool IsDirEmpty(this SdDirectoryInfo possiblyEmptyDir)
	{
		bool result;
		using (IEnumerator<SdFileSystemInfo> enumerator = possiblyEmptyDir.EnumerateFileSystemInfos("*", SearchOption.AllDirectories).GetEnumerator())
		{
			result = !enumerator.MoveNext();
		}
		return result;
	}
}
