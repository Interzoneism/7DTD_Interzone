using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001BB RID: 443
[Preserve]
public class ConsoleCmdBuffPlayer : ConsoleCmdAbstract
{
	// Token: 0x06000D7F RID: 3455 RVA: 0x0005A958 File Offset: 0x00058B58
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Apply a buff to a player";
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x0005A95F File Offset: 0x00058B5F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\n   buffplayer <player name / steam id / entity id> <buff name>\nApply the given buff to the player given by the player name or entity id (as given by e.g. \"lpi\").";
	}

	// Token: 0x06000D81 RID: 3457 RVA: 0x0005A966 File Offset: 0x00058B66
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"buffplayer"
		};
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x0005A978 File Offset: 0x00058B78
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
			clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConsoleCmdClient>().Setup("buff " + str, true));
			return;
		}
		if (_senderInfo.IsLocalGame)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Use the \"buff\" command for the local player.");
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Playername or entity ID not found.");
	}
}
