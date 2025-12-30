using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001D2 RID: 466
[Preserve]
public class ConsoleCmdDebuffPlayer : ConsoleCmdAbstract
{
	// Token: 0x06000E00 RID: 3584 RVA: 0x0005D1FB File Offset: 0x0005B3FB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Remove a buff from a player";
	}

	// Token: 0x06000E01 RID: 3585 RVA: 0x0005D202 File Offset: 0x0005B402
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\n   debuffplayer <player name / steam id / entity id> <buff name>\nRemove the given buff from the player given by the player name or entity id (as given by e.g. \"lpi\").";
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x0005D209 File Offset: 0x0005B409
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"debuffplayer"
		};
	}

	// Token: 0x06000E03 RID: 3587 RVA: 0x0005D21C File Offset: 0x0005B41C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count != 2)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid arguments, requires a target player and a buff name");
			ConsoleCmdBuff.PrintAvailableBuffNames();
			return;
		}
		string str = _params[1];
		ClientInfo clientInfo = ConsoleHelper.ParseParamIdOrName(_params[0], true, false);
		if (clientInfo != null)
		{
			clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConsoleCmdClient>().Setup("debuff " + str, true));
			return;
		}
		if (_senderInfo.IsLocalGame)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Use the \"debuff\" command for the local player.");
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Playername or entity ID not found.");
	}
}
