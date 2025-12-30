using System;

// Token: 0x0200040E RID: 1038
public static class DynamicRagdollFlagsExtension
{
	// Token: 0x06001F01 RID: 7937 RVA: 0x000C0C37 File Offset: 0x000BEE37
	public static bool HasFlag(this DynamicRagdollFlags flag, DynamicRagdollFlags checkFlag)
	{
		return (flag & checkFlag) == checkFlag;
	}
}
