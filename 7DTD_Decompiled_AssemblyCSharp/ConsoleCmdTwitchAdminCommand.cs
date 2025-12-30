using System;
using System.Collections.Generic;
using Platform;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x02000271 RID: 625
[Preserve]
public class ConsoleCmdTwitchAdminCommand : ConsoleCmdAbstract
{
	// Token: 0x060011B6 RID: 4534 RVA: 0x0006F31C File Offset: 0x0006D51C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"twitchadmin"
		};
	}

	// Token: 0x170001D2 RID: 466
	// (get) Token: 0x060011B7 RID: 4535 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001D3 RID: 467
	// (get) Token: 0x060011B8 RID: 4536 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x170001D4 RID: 468
	// (get) Token: 0x060011B9 RID: 4537 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x060011BA RID: 4538 RVA: 0x0006F32C File Offset: 0x0006D52C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!TwitchManager.Current.IsReady)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Twitch must be active to use this command!");
		}
		if (_params.Count < 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.getHelp());
			return;
		}
		string a;
		if (_params.Count != 1)
		{
			if (_params.Count == 3 && _params[0] == "reset" && _params[1] == "all")
			{
				a = _params[2];
				if (a == "both")
				{
					TwitchManager.Current.ViewerData.ResetAllPoints();
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Resetting all Twitch PP and SP");
					return;
				}
				if (a == "sp")
				{
					TwitchManager.Current.ViewerData.ResetAllSpecialPoints();
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Resetting all Twitch SP");
					return;
				}
				if (!(a == "pp"))
				{
					return;
				}
				TwitchManager.Current.ViewerData.ResetAllStandardPoints();
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Resetting all Twitch PP");
			}
			return;
		}
		a = _params[0];
		if (a == "cleanup")
		{
			TwitchManager.Current.ViewerData.Cleanup();
			return;
		}
		if (a == "export")
		{
			TwitchManager.Current.SaveExportViewerData();
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Exporting Viewer Data");
			return;
		}
		if (a == "import")
		{
			TwitchManager.Current.LoadExportViewerData();
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Importing Viewer Data");
			return;
		}
		if (!(a == "pointstotal"))
		{
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Twitch Viewer Data Point Totals:\n" + TwitchManager.Current.ViewerData.GetPointTotals());
	}

	// Token: 0x060011BB RID: 4539 RVA: 0x0006F4E3 File Offset: 0x0006D6E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Twitch Admin Commands";
	}

	// Token: 0x060011BC RID: 4540 RVA: 0x0006F4EA File Offset: 0x0006D6EA
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\n  1. twitchadmin cleanup\n  2. twitchadmin export\n  3. twitchadmin import\n  4. twitchadmin pointtotals\n  5. twitchadmin reset all both\n  6. twitchadmin reset all pp\n  7. twitchadmin reset all sp\n1. Cleans up duplicate Viewer Data.\n2. Exports all the users to a comma delimited twitchusers.xml\n3. Imports all the users to a comma delimited twitchusers.xml\n4. Prints out the totals for PP, SP, and BitCredit\n5. Clears all PP and SP for all users\n6. Clears all PP for all users\n7. Clears all SP for all users";
	}
}
