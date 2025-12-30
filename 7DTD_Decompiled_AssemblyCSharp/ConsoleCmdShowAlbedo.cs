using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200024A RID: 586
[Preserve]
public class ConsoleCmdShowAlbedo : ConsoleCmdAbstract
{
	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x060010DF RID: 4319 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x060010E0 RID: 4320 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060010E1 RID: 4321 RVA: 0x0006B4FD File Offset: 0x000696FD
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"showalbedo",
			"albedo"
		};
	}

	// Token: 0x060010E2 RID: 4322 RVA: 0x0006B515 File Offset: 0x00069715
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		Polarizer.SetDebugView((Polarizer.GetDebugView() != Polarizer.ViewEnums.Albedo) ? Polarizer.ViewEnums.Albedo : Polarizer.ViewEnums.None);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("showalbedo " + ((Polarizer.GetDebugView() == Polarizer.ViewEnums.Albedo) ? "on" : "off"));
	}

	// Token: 0x060010E3 RID: 4323 RVA: 0x0006B550 File Offset: 0x00069750
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "enables/disables display of albedo in gBuffer";
	}
}
