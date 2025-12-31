using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001D4 RID: 468
[Preserve]
public class ConsoleCmdDebugMenu : ConsoleCmdAbstract
{
	// Token: 0x17000120 RID: 288
	// (get) Token: 0x06000E0B RID: 3595 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000E0C RID: 3596 RVA: 0x0005D4B0 File Offset: 0x0005B6B0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"debugmenu",
			"dm"
		};
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x0005D4C8 File Offset: 0x0005B6C8
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		GamePrefs.Set(EnumGamePrefs.DebugMenuEnabled, !GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled));
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("debugmenu " + (GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled) ? "on" : "off"));
	}

	// Token: 0x06000E0E RID: 3598 RVA: 0x0005D504 File Offset: 0x0005B704
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "enables/disables the debugmenu ";
	}
}
