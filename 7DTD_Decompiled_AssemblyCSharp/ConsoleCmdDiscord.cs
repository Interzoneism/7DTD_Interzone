using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020002EA RID: 746
[Preserve]
public class ConsoleCmdDiscord : ConsoleCmdAbstract
{
	// Token: 0x06001544 RID: 5444 RVA: 0x0007E0B4 File Offset: 0x0007C2B4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"discord",
			"dc"
		};
	}

	// Token: 0x1700026A RID: 618
	// (get) Token: 0x06001545 RID: 5445 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700026B RID: 619
	// (get) Token: 0x06001546 RID: 5446 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700026C RID: 620
	// (get) Token: 0x06001547 RID: 5447 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06001548 RID: 5448 RVA: 0x0007E0CC File Offset: 0x0007C2CC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Toggle Discord debug window";
	}

	// Token: 0x06001549 RID: 5449 RVA: 0x0007E0D3 File Offset: 0x0007C2D3
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		LocalPlayerUI.primaryUI.windowManager.SwitchVisible(XUiC_DiscordWindow.ID, true, false);
	}
}
