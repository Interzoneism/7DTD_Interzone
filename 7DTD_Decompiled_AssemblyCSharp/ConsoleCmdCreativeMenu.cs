using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020001CC RID: 460
[Preserve]
public class ConsoleCmdCreativeMenu : ConsoleCmdAbstract
{
	// Token: 0x1700011B RID: 283
	// (get) Token: 0x06000DE6 RID: 3558 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x06000DE7 RID: 3559 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x0005CCE3 File Offset: 0x0005AEE3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"creativemenu",
			"cm"
		};
	}

	// Token: 0x06000DE9 RID: 3561 RVA: 0x0005CCFB File Offset: 0x0005AEFB
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		GamePrefs.Set(EnumGamePrefs.CreativeMenuEnabled, !GamePrefs.GetBool(EnumGamePrefs.CreativeMenuEnabled));
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("creativemenu " + (GamePrefs.GetBool(EnumGamePrefs.CreativeMenuEnabled) ? "on" : "off"));
	}

	// Token: 0x06000DEA RID: 3562 RVA: 0x0005CD37 File Offset: 0x0005AF37
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "enables/disables the creativemenu";
	}
}
