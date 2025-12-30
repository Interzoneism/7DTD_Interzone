using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020001FE RID: 510
[Preserve]
public class ConsoleCmdListPlayerIds : ConsoleCmdAbstract
{
	// Token: 0x06000F13 RID: 3859 RVA: 0x00062ACC File Offset: 0x00060CCC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"listplayerids",
			"lpi"
		};
	}

	// Token: 0x17000160 RID: 352
	// (get) Token: 0x06000F14 RID: 3860 RVA: 0x0005B5EB File Offset: 0x000597EB
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x17000161 RID: 353
	// (get) Token: 0x06000F15 RID: 3861 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06000F16 RID: 3862 RVA: 0x00062AE4 File Offset: 0x00060CE4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Lists all players with their IDs for ingame commands";
	}

	// Token: 0x06000F17 RID: 3863 RVA: 0x00062AEC File Offset: 0x00060CEC
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		World world = GameManager.Instance.World;
		int num = 0;
		foreach (KeyValuePair<int, EntityPlayer> keyValuePair in world.Players.dict)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("{0}. id={1}, {2}", ++num, keyValuePair.Value.entityId, keyValuePair.Value.EntityName));
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Total of " + world.Players.list.Count.ToString() + " in the game");
	}
}
