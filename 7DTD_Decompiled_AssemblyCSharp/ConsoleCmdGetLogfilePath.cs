using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020001E7 RID: 487
[Preserve]
public class ConsoleCmdGetLogfilePath : ConsoleCmdAbstract
{
	// Token: 0x06000E84 RID: 3716 RVA: 0x0005E720 File Offset: 0x0005C920
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"getlogpath",
			"glp"
		};
	}

	// Token: 0x17000141 RID: 321
	// (get) Token: 0x06000E85 RID: 3717 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000142 RID: 322
	// (get) Token: 0x06000E86 RID: 3718 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000143 RID: 323
	// (get) Token: 0x06000E87 RID: 3719 RVA: 0x0005B5EB File Offset: 0x000597EB
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x06000E88 RID: 3720 RVA: 0x0005E738 File Offset: 0x0005C938
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Get the path of the logfile the game currently writes to";
	}

	// Token: 0x06000E89 RID: 3721 RVA: 0x0005E73F File Offset: 0x0005C93F
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(Application.consoleLogPath);
	}
}
