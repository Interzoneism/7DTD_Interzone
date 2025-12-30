using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000264 RID: 612
[Preserve]
public class ConsoleCmdSwitchView : ConsoleCmdAbstract
{
	// Token: 0x0600116D RID: 4461 RVA: 0x0006DB8E File Offset: 0x0006BD8E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"switchview",
			"sv"
		};
	}

	// Token: 0x0600116E RID: 4462 RVA: 0x0006DBA6 File Offset: 0x0006BDA6
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Switch between fpv and tpv";
	}

	// Token: 0x0600116F RID: 4463 RVA: 0x0006DBB0 File Offset: 0x0006BDB0
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (GameManager.Instance.World.GetPrimaryPlayer() == null)
		{
			return;
		}
		GameManager.Instance.World.GetPrimaryPlayer().SwitchFirstPersonViewFromInput();
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Switched to " + (GameManager.Instance.World.GetPrimaryPlayer().bFirstPersonView ? "FPV" : "TPV"));
	}
}
