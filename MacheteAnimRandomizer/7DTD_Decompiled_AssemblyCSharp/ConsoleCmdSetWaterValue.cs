using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000247 RID: 583
[Preserve]
public class ConsoleCmdSetWaterValue : ConsoleCmdAbstract
{
	// Token: 0x170001AF RID: 431
	// (get) Token: 0x060010CC RID: 4300 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060010CD RID: 4301 RVA: 0x0006B100 File Offset: 0x00069300
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"setwatervalue",
			"swv"
		};
	}

	// Token: 0x060010CE RID: 4302 RVA: 0x0006B118 File Offset: 0x00069318
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Sets the water value for all flow-permitting blocks within the current selection area, specified in the range of 0 (empty) to 1 (full).";
	}

	// Token: 0x060010CF RID: 4303 RVA: 0x0006B11F File Offset: 0x0006931F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "'swv [0.0 - 1.0]' Sets water value between empty (0.0) and full (1.0) for all flow-permitting blocks within the current selection bounds. \nE.g. 'swv 0.5' will set all affected blocks to be half-full. \nBlocks which do not permit flow are unchanged.";
	}

	// Token: 0x060010D0 RID: 4304 RVA: 0x0006B128 File Offset: 0x00069328
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		float num;
		if (_params.Count != 1 || !float.TryParse(_params[0], out num) || num < 0f || num > 1f)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
			return;
		}
		BlockToolSelection instance = BlockToolSelection.Instance;
		if (!instance.SelectionActive)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No selection active. Running this command requires an active selection box.");
			return;
		}
		BlockTools.CubeWaterRPC(GameManager.Instance, instance.SelectionStart, instance.SelectionEnd, new WaterValue((int)(num * 19500f)));
	}
}
