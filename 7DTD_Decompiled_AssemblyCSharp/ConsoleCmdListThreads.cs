using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000200 RID: 512
[Preserve]
public class ConsoleCmdListThreads : ConsoleCmdAbstract
{
	// Token: 0x06000F1D RID: 3869 RVA: 0x00062E4D File Offset: 0x0006104D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"listthreads",
			"lt"
		};
	}

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06000F1E RID: 3870 RVA: 0x0005B5EB File Offset: 0x000597EB
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x06000F1F RID: 3871 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x00062E68 File Offset: 0x00061068
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Threads:");
		int num = 0;
		foreach (KeyValuePair<string, ThreadManager.ThreadInfo> keyValuePair in ThreadManager.ActiveThreads)
		{
			SdtdConsole instance = SingletonMonoBehaviour<SdtdConsole>.Instance;
			int num2;
			num = (num2 = num + 1);
			instance.Output(num2.ToString() + ". " + keyValuePair.Key);
		}
	}

	// Token: 0x06000F21 RID: 3873 RVA: 0x00062EEC File Offset: 0x000610EC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "lists all threads";
	}
}
