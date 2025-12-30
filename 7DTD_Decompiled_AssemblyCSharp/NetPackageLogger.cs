using System;
using System.IO;
using UnityEngine;

// Token: 0x020006F3 RID: 1779
public static class NetPackageLogger
{
	// Token: 0x06003475 RID: 13429 RVA: 0x00160D4C File Offset: 0x0015EF4C
	public static void Init()
	{
		if (NetPackageLogger.logFilePathPrefix != null)
		{
			return;
		}
		if (string.IsNullOrEmpty(Application.consoleLogPath))
		{
			NetPackageLogger.logFilePathPrefix = "";
			NetPackageLogger.logEnabled = false;
			return;
		}
		NetPackageLogger.logFilePathPrefix = Path.GetDirectoryName(Application.consoleLogPath) + "/netpackages_";
		NetPackageLogger.logEnabled = (GameUtils.GetLaunchArgument("debugpackages") != null);
	}

	// Token: 0x06003476 RID: 13430 RVA: 0x00160DAC File Offset: 0x0015EFAC
	public static void BeginLog(bool _asServer)
	{
		if (!NetPackageLogger.logEnabled)
		{
			return;
		}
		NetPackageLogger.opened++;
		if (NetPackageLogger.logFileStream != null)
		{
			return;
		}
		NetPackageLogger.logFileStream = SdFile.CreateText(NetPackageLogger.logFilePathPrefix + (_asServer ? "S_" : "C_") + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv");
		NetPackageLogger.logFileStream.WriteLine("Time,Dir,Src/Tgt,PackageType,Chn,Len,Encrypted,Compressed,Pkg# in Msg,Pkgs in Msg");
	}

	// Token: 0x06003477 RID: 13431 RVA: 0x00160E20 File Offset: 0x0015F020
	public static void LogPackage(bool _dirIsOut, ClientInfo _clientInfo, NetPackage _packageType, int _channel, int _length, bool _encrypted, bool _compressed, int _pkgNumInMsg, int _pkgsInMsg)
	{
		if (NetPackageLogger.logFileStream == null)
		{
			return;
		}
		string text = (_clientInfo == null) ? "Server" : _clientInfo.InternalId.CombinedString;
		StreamWriter obj = NetPackageLogger.logFileStream;
		lock (obj)
		{
			NetPackageLogger.logFileStream.WriteLine(string.Format("{0:O},{1},{2},{3},{4},{5},{6},{7},{8},{9}", new object[]
			{
				DateTime.Now,
				_dirIsOut ? "Out" : "In",
				text,
				_packageType.GetType().Name,
				_channel,
				_length,
				_encrypted,
				_compressed,
				_pkgNumInMsg,
				_pkgsInMsg
			}));
		}
	}

	// Token: 0x06003478 RID: 13432 RVA: 0x00160F00 File Offset: 0x0015F100
	public static void EndLog()
	{
		if (NetPackageLogger.logFileStream == null)
		{
			return;
		}
		if (--NetPackageLogger.opened > 0)
		{
			return;
		}
		NetPackageLogger.logFileStream.Close();
		NetPackageLogger.logFileStream = null;
	}

	// Token: 0x04002AF7 RID: 10999
	[PublicizedFrom(EAccessModifier.Private)]
	public static int opened;

	// Token: 0x04002AF8 RID: 11000
	[PublicizedFrom(EAccessModifier.Private)]
	public static StreamWriter logFileStream;

	// Token: 0x04002AF9 RID: 11001
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool logEnabled;

	// Token: 0x04002AFA RID: 11002
	[PublicizedFrom(EAccessModifier.Private)]
	public static string logFilePathPrefix;
}
