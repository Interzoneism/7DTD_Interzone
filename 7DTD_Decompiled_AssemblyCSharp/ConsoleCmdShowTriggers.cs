using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000251 RID: 593
[Preserve]
public class ConsoleCmdShowTriggers : ConsoleCmdAbstract
{
	// Token: 0x06001107 RID: 4359 RVA: 0x0006BAB0 File Offset: 0x00069CB0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"showtriggers"
		};
	}

	// Token: 0x06001108 RID: 4360 RVA: 0x0006BAC0 File Offset: 0x00069CC0
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		bool flag = !GameManager.Instance.World.triggerManager.ShowNavObjects;
		GameManager.Instance.World.triggerManager.ShowNavObjects = flag;
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Show Triggers {0}!", new object[]
		{
			flag ? "Enabled" : "Disabled"
		});
	}

	// Token: 0x06001109 RID: 4361 RVA: 0x0006BB2E File Offset: 0x00069D2E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Sets the visibility of the block triggers.";
	}
}
