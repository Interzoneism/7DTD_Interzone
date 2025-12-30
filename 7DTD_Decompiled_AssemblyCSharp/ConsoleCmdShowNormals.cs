using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200024E RID: 590
[Preserve]
public class ConsoleCmdShowNormals : ConsoleCmdAbstract
{
	// Token: 0x170001B9 RID: 441
	// (get) Token: 0x060010F8 RID: 4344 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x0006B96B File Offset: 0x00069B6B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"shownormals",
			"norms"
		};
	}

	// Token: 0x060010FA RID: 4346 RVA: 0x0006B983 File Offset: 0x00069B83
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		Polarizer.SetDebugView((Polarizer.GetDebugView() != Polarizer.ViewEnums.Normals) ? Polarizer.ViewEnums.Normals : Polarizer.ViewEnums.None);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("shownormals " + ((Polarizer.GetDebugView() == Polarizer.ViewEnums.Normals) ? "on" : "off"));
	}

	// Token: 0x060010FB RID: 4347 RVA: 0x0006B9BE File Offset: 0x00069BBE
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "enables/disables display of normal maps in gBuffer";
	}
}
