using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

// Token: 0x020007D8 RID: 2008
[PublicizedFrom(EAccessModifier.Internal)]
public class ServerDateTimeRequest
{
	// Token: 0x060039E1 RID: 14817 RVA: 0x00175584 File Offset: 0x00173784
	public static void GetNtpTimeAsync(Action<ServerDateTimeResult> _onComplete, string _ntpServer = "pool.ntp.org", int _timeoutMilliseconds = 5000)
	{
		ServerDateTimeRequest.<>c__DisplayClass0_0 CS$<>8__locals1 = new ServerDateTimeRequest.<>c__DisplayClass0_0();
		CS$<>8__locals1._ntpServer = _ntpServer;
		CS$<>8__locals1._timeoutMilliseconds = _timeoutMilliseconds;
		CS$<>8__locals1._onComplete = _onComplete;
		Task.Run(delegate()
		{
			ServerDateTimeRequest.<>c__DisplayClass0_0.<<GetNtpTimeAsync>b__0>d <<GetNtpTimeAsync>b__0>d;
			<<GetNtpTimeAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<<GetNtpTimeAsync>b__0>d.<>4__this = CS$<>8__locals1;
			<<GetNtpTimeAsync>b__0>d.<>1__state = -1;
			<<GetNtpTimeAsync>b__0>d.<>t__builder.Start<ServerDateTimeRequest.<>c__DisplayClass0_0.<<GetNtpTimeAsync>b__0>d>(ref <<GetNtpTimeAsync>b__0>d);
			return <<GetNtpTimeAsync>b__0>d.<>t__builder.Task;
		});
	}

	// Token: 0x060039E2 RID: 14818 RVA: 0x001755B4 File Offset: 0x001737B4
	[PublicizedFrom(EAccessModifier.Private)]
	public static Task<ServerDateTimeResult> FetchNtpTimeAsync(string _ntpServer, int _timeoutMilliseconds)
	{
		ServerDateTimeRequest.<FetchNtpTimeAsync>d__1 <FetchNtpTimeAsync>d__;
		<FetchNtpTimeAsync>d__.<>t__builder = AsyncTaskMethodBuilder<ServerDateTimeResult>.Create();
		<FetchNtpTimeAsync>d__._ntpServer = _ntpServer;
		<FetchNtpTimeAsync>d__._timeoutMilliseconds = _timeoutMilliseconds;
		<FetchNtpTimeAsync>d__.<>1__state = -1;
		<FetchNtpTimeAsync>d__.<>t__builder.Start<ServerDateTimeRequest.<FetchNtpTimeAsync>d__1>(ref <FetchNtpTimeAsync>d__);
		return <FetchNtpTimeAsync>d__.<>t__builder.Task;
	}

	// Token: 0x060039E3 RID: 14819 RVA: 0x001755FF File Offset: 0x001737FF
	[PublicizedFrom(EAccessModifier.Private)]
	public static uint SwapEndianness(ulong _x)
	{
		return (uint)(((_x & 255UL) << 24) + ((_x & 65280UL) << 8) + ((_x & 16711680UL) >> 8) + ((_x & (ulong)-16777216) >> 24));
	}
}
