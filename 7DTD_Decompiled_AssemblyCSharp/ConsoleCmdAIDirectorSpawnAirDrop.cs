using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001B2 RID: 434
[Preserve]
public class ConsoleCmdAIDirectorSpawnAirDrop : ConsoleCmdAbstract
{
	// Token: 0x06000D45 RID: 3397 RVA: 0x0005921F File Offset: 0x0005741F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"spawnairdrop"
		};
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x0005922F File Offset: 0x0005742F
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		GameManager.Instance.World.aiDirector.GetComponent<AIDirectorAirDropComponent>().SpawnAirDrop();
	}

	// Token: 0x06000D47 RID: 3399 RVA: 0x0005924B File Offset: 0x0005744B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Spawns an air drop";
	}
}
