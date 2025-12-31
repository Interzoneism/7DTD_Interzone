using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200024F RID: 591
[Preserve]
public class ConsoleCmdShowSpecular : ConsoleCmdAbstract
{
	// Token: 0x170001BA RID: 442
	// (get) Token: 0x060010FD RID: 4349 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060010FE RID: 4350 RVA: 0x0006B9C5 File Offset: 0x00069BC5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"showspecular",
			"spec"
		};
	}

	// Token: 0x060010FF RID: 4351 RVA: 0x0006B9DD File Offset: 0x00069BDD
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		Polarizer.SetDebugView((Polarizer.GetDebugView() != Polarizer.ViewEnums.Specular) ? Polarizer.ViewEnums.Specular : Polarizer.ViewEnums.None);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("showspecular " + ((Polarizer.GetDebugView() == Polarizer.ViewEnums.Specular) ? "on" : "off"));
	}

	// Token: 0x06001100 RID: 4352 RVA: 0x0006BA18 File Offset: 0x00069C18
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "enables/disables display of specular values in gBuffer";
	}
}
