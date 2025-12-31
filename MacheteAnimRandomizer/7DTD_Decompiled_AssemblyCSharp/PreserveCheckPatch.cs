using System;
using System.Diagnostics;

// Token: 0x020011F4 RID: 4596
public static class PreserveCheckPatch
{
	// Token: 0x06008FA9 RID: 36777 RVA: 0x00002914 File Offset: 0x00000B14
	[Conditional("ENABLE_MONO")]
	public static void Enable()
	{
	}

	// Token: 0x06008FAA RID: 36778 RVA: 0x00002914 File Offset: 0x00000B14
	[Conditional("ENABLE_MONO")]
	public static void Disable()
	{
	}
}
