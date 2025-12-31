using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000226 RID: 550
[Preserve]
public class ConsoleCmdPPList : ConsoleCmdAbstract
{
	// Token: 0x1700018C RID: 396
	// (get) Token: 0x0600100C RID: 4108 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600100D RID: 4109 RVA: 0x00067997 File Offset: 0x00065B97
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"pplist"
		};
	}

	// Token: 0x0600100E RID: 4110 RVA: 0x000679A8 File Offset: 0x00065BA8
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		PersistentPlayerList persistentPlayers = GameManager.Instance.persistentPlayers;
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(persistentPlayers.Players.Count.ToString() + " Persistent Player(s)");
		foreach (KeyValuePair<PlatformUserIdentifierAbs, PersistentPlayerData> keyValuePair in persistentPlayers.Players)
		{
			SdtdConsole instance = SingletonMonoBehaviour<SdtdConsole>.Instance;
			string str = "   ";
			PlatformUserIdentifierAbs key = keyValuePair.Key;
			instance.Output(str + ((key != null) ? key.ToString() : null) + " -> " + keyValuePair.Value.EntityId.ToString());
		}
	}

	// Token: 0x0600100F RID: 4111 RVA: 0x00067A60 File Offset: 0x00065C60
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Lists all PersistentPlayer data";
	}
}
