using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001BE RID: 446
[Preserve]
public class ConsoleCmdCensor : ConsoleCmdAbstract
{
	// Token: 0x1700010C RID: 268
	// (get) Token: 0x06000D95 RID: 3477 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x0005AD84 File Offset: 0x00058F84
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"testCensor",
			"tcc"
		};
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x0005AD9C File Offset: 0x00058F9C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Censorship testing toggle.";
	}

	// Token: 0x1700010D RID: 269
	// (get) Token: 0x06000D98 RID: 3480 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x0005ADA3 File Offset: 0x00058FA3
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled))
		{
			return;
		}
		if (_params.Count == 0)
		{
			GameManager.DebugCensorship = !GameManager.DebugCensorship;
			Log.Out("Censor testing enabled: " + GameManager.DebugCensorship.ToString());
			return;
		}
	}
}
