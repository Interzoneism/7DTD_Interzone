using System;
using System.Diagnostics;

// Token: 0x020011F5 RID: 4597
public static class ProfilerExt
{
	// Token: 0x06008FAB RID: 36779 RVA: 0x00395C95 File Offset: 0x00393E95
	[Conditional("ENABLE_PROFILER")]
	public static void BeginSampleThreadSafe(string _caption)
	{
		ThreadManager.IsMainThread();
	}

	// Token: 0x06008FAC RID: 36780 RVA: 0x00395C95 File Offset: 0x00393E95
	[Conditional("ENABLE_PROFILER")]
	public static void EndSampleThreadSafe()
	{
		ThreadManager.IsMainThread();
	}
}
