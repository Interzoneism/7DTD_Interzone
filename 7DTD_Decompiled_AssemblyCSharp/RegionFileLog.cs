using System;
using System.Diagnostics;

// Token: 0x02000A28 RID: 2600
public static class RegionFileLog
{
	// Token: 0x06004F7C RID: 20348 RVA: 0x001F6B4F File Offset: 0x001F4D4F
	[Conditional("DEBUG_REGIONLOG")]
	public static void Region(string _format = "", params object[] _args)
	{
		_format = string.Format("{0} Region {1}", GameManager.frameCount, _format);
		Log.Warning(_format, _args);
	}
}
