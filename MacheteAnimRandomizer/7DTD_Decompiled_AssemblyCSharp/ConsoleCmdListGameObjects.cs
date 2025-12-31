using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020001FD RID: 509
[Preserve]
public class ConsoleCmdListGameObjects : ConsoleCmdAbstract
{
	// Token: 0x06000F0D RID: 3853 RVA: 0x00062A6D File Offset: 0x00060C6D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"lgo",
			"listgameobjects"
		};
	}

	// Token: 0x1700015E RID: 350
	// (get) Token: 0x06000F0E RID: 3854 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700015F RID: 351
	// (get) Token: 0x06000F0F RID: 3855 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000F10 RID: 3856 RVA: 0x00062A85 File Offset: 0x00060C85
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "List all active game objects";
	}

	// Token: 0x06000F11 RID: 3857 RVA: 0x00062A8C File Offset: 0x00060C8C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		MicroStopwatch microStopwatch = new MicroStopwatch();
		int num = UnityEngine.Object.FindObjectsOfType<UnityEngine.Object>().Length;
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("GOs: {0}, took {1} ms", num, microStopwatch.ElapsedMilliseconds));
	}
}
