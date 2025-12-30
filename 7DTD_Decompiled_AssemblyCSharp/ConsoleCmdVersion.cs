using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000274 RID: 628
[Preserve]
public class ConsoleCmdVersion : ConsoleCmdAbstract
{
	// Token: 0x060011CD RID: 4557 RVA: 0x0006F6B8 File Offset: 0x0006D8B8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"version"
		};
	}

	// Token: 0x170001DB RID: 475
	// (get) Token: 0x060011CE RID: 4558 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001DC RID: 476
	// (get) Token: 0x060011CF RID: 4559 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x060011D0 RID: 4560 RVA: 0x0006F6C8 File Offset: 0x0006D8C8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Get the currently running version of the game and loaded mods";
	}

	// Token: 0x060011D1 RID: 4561 RVA: 0x0006F6D0 File Offset: 0x0006D8D0
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Game version: " + Constants.cVersionInformation.LongString + " Compatibility Version: " + Constants.cVersionInformation.LongStringNoBuild);
		for (int i = 0; i < ModManager.GetLoadedMods().Count; i++)
		{
			Mod mod = ModManager.GetLoadedMods()[i];
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Mod " + mod.Name + ": " + (mod.VersionString ?? "<unknown version>"));
		}
	}
}
