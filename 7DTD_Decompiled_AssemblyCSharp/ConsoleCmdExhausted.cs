using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x020001DD RID: 477
[Preserve]
public class ConsoleCmdExhausted : ConsoleCmdAbstract
{
	// Token: 0x17000132 RID: 306
	// (get) Token: 0x06000E45 RID: 3653 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000E46 RID: 3654 RVA: 0x0005DB8B File Offset: 0x0005BD8B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"exhausted"
		};
	}

	// Token: 0x06000E47 RID: 3655 RVA: 0x0005DB9C File Offset: 0x0005BD9C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		EntityPlayer primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (primaryPlayer != null)
		{
			float num = 0f;
			if (_params.Count > 0)
			{
				num = StringParsers.ParseFloat(_params[0], 0, -1, NumberStyles.Any);
			}
			primaryPlayer.Stats.Stamina.Value = num * primaryPlayer.Stats.Stamina.Max;
		}
	}

	// Token: 0x06000E48 RID: 3656 RVA: 0x0005DC07 File Offset: 0x0005BE07
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Makes the player exhausted.";
	}
}
