using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000250 RID: 592
[Preserve]
public class ConsoleCmdShowSwings : ConsoleCmdAbstract
{
	// Token: 0x170001BB RID: 443
	// (get) Token: 0x06001102 RID: 4354 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001103 RID: 4355 RVA: 0x0006BA1F File Offset: 0x00069C1F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"showswings"
		};
	}

	// Token: 0x06001104 RID: 4356 RVA: 0x0006BA30 File Offset: 0x00069C30
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		EntityAlive.ShowDebugDisplayHit = !EntityAlive.ShowDebugDisplayHit;
		ItemActionDynamic.ShowDebugSwing = EntityAlive.ShowDebugDisplayHit;
		if (EntityAlive.ShowDebugDisplayHit)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Show Swings (ON)");
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Show Swings (OFF)");
		for (int i = 0; i < ItemActionDynamic.DebugDisplayHits.Count; i++)
		{
			UnityEngine.Object.DestroyImmediate(ItemActionDynamic.DebugDisplayHits[i]);
		}
		ItemActionDynamic.DebugDisplayHits.Clear();
	}

	// Token: 0x06001105 RID: 4357 RVA: 0x0006BAA9 File Offset: 0x00069CA9
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Show melee swing arc rays";
	}
}
