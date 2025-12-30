using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000262 RID: 610
[Preserve]
public class ConsoleCmdStab : ConsoleCmdAbstract
{
	// Token: 0x06001163 RID: 4451 RVA: 0x0006DA27 File Offset: 0x0006BC27
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"stab"
		};
	}

	// Token: 0x06001164 RID: 4452 RVA: 0x0006DA37 File Offset: 0x0006BC37
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "stability";
	}

	// Token: 0x06001165 RID: 4453 RVA: 0x0002B133 File Offset: 0x00029333
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "";
	}

	// Token: 0x06001166 RID: 4454 RVA: 0x0006DA40 File Offset: 0x0006BC40
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Running stability");
		GameManager.Instance.CreateStabilityViewer();
		if (_params.Count == 0)
		{
			GameManager.Instance.stabilityViewer.StartSearch(100);
			return;
		}
		if (_params.Count != 1)
		{
			return;
		}
		if (_params[0].EqualsCaseInsensitive("Clear"))
		{
			GameManager.Instance.ClearStabilityViewer();
			return;
		}
		if (_params[0].EqualsCaseInsensitive("Redo"))
		{
			GameManager.Instance.stabilityViewer.StartSearch(100);
			return;
		}
		int asynCount = 31;
		int.TryParse(_params[0], out asynCount);
		GameManager.Instance.stabilityViewer.StartSearch(asynCount);
	}
}
