using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000252 RID: 594
[Preserve]
public class ConsoleCmdShutdown : ConsoleCmdAbstract
{
	// Token: 0x0600110B RID: 4363 RVA: 0x0006BB35 File Offset: 0x00069D35
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"shutdown"
		};
	}

	// Token: 0x170001BC RID: 444
	// (get) Token: 0x0600110C RID: 4364 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600110D RID: 4365 RVA: 0x0006BB45 File Offset: 0x00069D45
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Shutting server down...");
		Application.Quit();
	}

	// Token: 0x0600110E RID: 4366 RVA: 0x0006BB5B File Offset: 0x00069D5B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "shuts down the game";
	}
}
