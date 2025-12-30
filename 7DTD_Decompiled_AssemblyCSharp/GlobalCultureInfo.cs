using System;
using System.Globalization;
using System.Threading;

// Token: 0x020011A2 RID: 4514
public class GlobalCultureInfo
{
	// Token: 0x06008D14 RID: 36116 RVA: 0x0038AF6D File Offset: 0x0038916D
	public static bool SetDefaultCulture(CultureInfo _culture)
	{
		Thread.CurrentThread.CurrentCulture = _culture;
		CultureInfo.DefaultThreadCurrentCulture = _culture;
		return true;
	}
}
