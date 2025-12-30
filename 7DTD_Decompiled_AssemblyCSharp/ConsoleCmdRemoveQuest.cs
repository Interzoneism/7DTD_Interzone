using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000236 RID: 566
[Preserve]
public class ConsoleCmdRemoveQuest : ConsoleCmdAbstract
{
	// Token: 0x0600107B RID: 4219 RVA: 0x0006A273 File Offset: 0x00068473
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"removequest"
		};
	}

	// Token: 0x1700019D RID: 413
	// (get) Token: 0x0600107C RID: 4220 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700019E RID: 414
	// (get) Token: 0x0600107D RID: 4221 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x1700019F RID: 415
	// (get) Token: 0x0600107E RID: 4222 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x0600107F RID: 4223 RVA: 0x0006A284 File Offset: 0x00068484
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (GameManager.IsDedicatedServer)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("cannot execute removequest on dedicated server, please execute as a client");
		}
		if (_params.Count < 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("remove requires quest id");
			return;
		}
		string text = _params[0];
		foreach (KeyValuePair<string, QuestClass> keyValuePair in QuestClass.s_Quests)
		{
			if (keyValuePair.Key.EqualsCaseInsensitive(text))
			{
				text = keyValuePair.Key;
				break;
			}
		}
		if (QuestClass.GetQuest(text) == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Quest '{0}' does not exist!", text));
			return;
		}
		XUiM_Player.GetPlayer().QuestJournal.ForceRemoveQuest(text);
	}

	// Token: 0x06001080 RID: 4224 RVA: 0x0006A350 File Offset: 0x00068550
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "usage: removequest questname";
	}
}
