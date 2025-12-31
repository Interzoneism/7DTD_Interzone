using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001B1 RID: 433
[Preserve]
public class ConsoleCmdAIDirectorShowNextWanderingHordeTime : ConsoleCmdAbstract
{
	// Token: 0x06000D41 RID: 3393 RVA: 0x000591DC File Offset: 0x000573DC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"shownexthordetime"
		};
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x000591EC File Offset: 0x000573EC
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (GameManager.Instance.World.aiDirector != null)
		{
			GameManager.Instance.World.aiDirector.GetComponent<AIDirectorWanderingHordeComponent>().LogTimes();
		}
	}

	// Token: 0x06000D43 RID: 3395 RVA: 0x00059218 File Offset: 0x00057418
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Displays the wandering horde time";
	}
}
