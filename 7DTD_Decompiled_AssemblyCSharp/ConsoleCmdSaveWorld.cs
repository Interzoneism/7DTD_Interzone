using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x0200023A RID: 570
[Preserve]
public class ConsoleCmdSaveWorld : ConsoleCmdAbstract
{
	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x06001092 RID: 4242 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06001093 RID: 4243 RVA: 0x0006A674 File Offset: 0x00068874
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"saveworld",
			"sa"
		};
	}

	// Token: 0x06001094 RID: 4244 RVA: 0x0006A68C File Offset: 0x0006888C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		GameManager.Instance.SaveLocalPlayerData();
		GameManager.Instance.SaveWorld();
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("World saved");
	}

	// Token: 0x06001095 RID: 4245 RVA: 0x0006A6BE File Offset: 0x000688BE
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Saves the world manually.";
	}
}
