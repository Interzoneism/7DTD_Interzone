using System;
using System.Collections.Generic;
using Platform;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x02000272 RID: 626
[Preserve]
public class ConsoleCmdTwitchCommand : ConsoleCmdAbstract
{
	// Token: 0x060011BE RID: 4542 RVA: 0x0006F4F1 File Offset: 0x0006D6F1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"twitch"
		};
	}

	// Token: 0x170001D5 RID: 469
	// (get) Token: 0x060011BF RID: 4543 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001D6 RID: 470
	// (get) Token: 0x060011C0 RID: 4544 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x170001D7 RID: 471
	// (get) Token: 0x060011C1 RID: 4545 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x060011C2 RID: 4546 RVA: 0x0006F504 File Offset: 0x0006D704
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!TwitchManager.Current.IsReady)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Twitch must be active to use this command!");
		}
		if (_params.Count < 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("twitch commands available:");
			for (int i = 0; i < TwitchManager.Current.TwitchCommandList.Count; i++)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("   " + TwitchManager.Current.TwitchCommandList[i].CommandText[0]);
			}
			return;
		}
		if (_params[0] == "refresh")
		{
			GameManager.Instance.StartCoroutine(TwitchManager.Current.Authentication.RefreshToken(0f, true));
			return;
		}
		TwitchManager.Current.HandleConsoleAction(_params);
	}

	// Token: 0x060011C3 RID: 4547 RVA: 0x0006F5C9 File Offset: 0x0006D7C9
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "usage: twitch <command> <params>";
	}
}
