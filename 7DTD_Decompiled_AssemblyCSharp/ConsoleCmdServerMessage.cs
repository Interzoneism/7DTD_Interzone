using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000240 RID: 576
[Preserve]
public class ConsoleCmdServerMessage : ConsoleCmdAbstract
{
	// Token: 0x170001A5 RID: 421
	// (get) Token: 0x060010A3 RID: 4259 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x060010A4 RID: 4260 RVA: 0x0006A96A File Offset: 0x00068B6A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"say"
		};
	}

	// Token: 0x060010A5 RID: 4261 RVA: 0x0006A97A File Offset: 0x00068B7A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Sends a message to all connected clients";
	}

	// Token: 0x060010A6 RID: 4262 RVA: 0x0006A984 File Offset: 0x00068B84
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count < 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 1, found " + _params.Count.ToString() + ".");
			return;
		}
		string msg = _params[0];
		GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, msg, null, EMessageSender.Server, GeneratedTextManager.BbCodeSupportMode.Supported);
	}
}
