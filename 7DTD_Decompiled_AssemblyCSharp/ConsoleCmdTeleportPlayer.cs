using System;
using System.Collections.Generic;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000267 RID: 615
[Preserve]
public class ConsoleCmdTeleportPlayer : ConsoleCmdTeleportsAbs
{
	// Token: 0x170001CA RID: 458
	// (get) Token: 0x0600117D RID: 4477 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x0600117E RID: 4478 RVA: 0x0006E199 File Offset: 0x0006C399
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Teleport a given player";
	}

	// Token: 0x0600117F RID: 4479 RVA: 0x0006E1A0 File Offset: 0x0006C3A0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\n  1. teleportplayer <user id / player name / entity id> <x> <y> <z> [view direction]\n  2. teleportplayer <user id / player name / entity id> <target user id / player name / entity id>\n1. Teleports the player given by his UserID, player name or entity id (as given by e.g. \"lpi\")\n   to the specified location. Use y = -1 to spawn on ground. Optional argument view direction\n   allows you to speify a direction you want to look in after teleporting. This can be either\n   of n, ne, e, se, s, sw, w, nw or north, northeast, etc.\n2. As 1, but destination given by another (online) player";
	}

	// Token: 0x06001180 RID: 4480 RVA: 0x0006E1A7 File Offset: 0x0006C3A7
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"teleportplayer",
			"tele"
		};
	}

	// Token: 0x06001181 RID: 4481 RVA: 0x0006E1C0 File Offset: 0x0006C3C0
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count != 2 && _params.Count != 4 && _params.Count != 5)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 2, 4 or 5, found " + _params.Count.ToString() + ".");
			return;
		}
		ClientInfo clientInfo = ConsoleHelper.ParseParamIdOrName(_params[0], true, false);
		Vector3 destPos = default(Vector3);
		Vector3? viewDirection = null;
		if (clientInfo == null && (GameManager.IsDedicatedServer || !ConsoleHelper.ParamIsLocalPlayer(_params[0], true, false)))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Playername or entity/userid id not found.");
			return;
		}
		if (_params.Count >= 4)
		{
			Vector3i v3i;
			if (!base.TryParseV3i(_params, 1, out v3i))
			{
				return;
			}
			destPos = v3i;
			viewDirection = ((_params.Count == 5) ? base.TryParseViewDirection(_params[4]) : null);
		}
		else if (_params.Count == 2 && !base.TryGetDestinationFromPlayer(_params[1], out destPos))
		{
			return;
		}
		base.ExecuteTeleport(clientInfo, destPos, viewDirection);
	}
}
