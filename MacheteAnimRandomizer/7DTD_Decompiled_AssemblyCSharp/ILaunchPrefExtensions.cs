using System;
using System.Globalization;

// Token: 0x02000FED RID: 4077
public static class ILaunchPrefExtensions
{
	// Token: 0x06008168 RID: 33128 RVA: 0x00346FD9 File Offset: 0x003451D9
	[PublicizedFrom(EAccessModifier.Private)]
	public static string CreateCommandLineArgument(ILaunchPref pref, string value)
	{
		return "-" + pref.Name + "=" + value;
	}

	// Token: 0x06008169 RID: 33129 RVA: 0x00346FF1 File Offset: 0x003451F1
	public static string ToCommandLine(this ILaunchPref<string> pref, string value)
	{
		return ILaunchPrefExtensions.CreateCommandLineArgument(pref, value);
	}

	// Token: 0x0600816A RID: 33130 RVA: 0x00346FFA File Offset: 0x003451FA
	public static string ToCommandLine(this ILaunchPref<long> pref, long value)
	{
		return ILaunchPrefExtensions.CreateCommandLineArgument(pref, value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x0600816B RID: 33131 RVA: 0x0034700E File Offset: 0x0034520E
	public static string ToCommandLine(this ILaunchPref<bool> pref, bool value)
	{
		return ILaunchPrefExtensions.CreateCommandLineArgument(pref, value.ToString(CultureInfo.InvariantCulture));
	}
}
