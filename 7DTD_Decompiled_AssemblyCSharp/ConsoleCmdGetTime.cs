using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020001E9 RID: 489
[Preserve]
public class ConsoleCmdGetTime : ConsoleCmdAbstract
{
	// Token: 0x06000E95 RID: 3733 RVA: 0x0005E868 File Offset: 0x0005CA68
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"gettime",
			"gt"
		};
	}

	// Token: 0x06000E96 RID: 3734 RVA: 0x0005E880 File Offset: 0x0005CA80
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Get the current game time";
	}

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06000E97 RID: 3735 RVA: 0x0005B5EB File Offset: 0x000597EB
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x06000E98 RID: 3736 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06000E99 RID: 3737 RVA: 0x0005E887 File Offset: 0x0005CA87
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(ValueDisplayFormatters.WorldTime(GameManager.Instance.World.worldTime, "Day {0}, {1:00}:{2:00}"));
	}
}
