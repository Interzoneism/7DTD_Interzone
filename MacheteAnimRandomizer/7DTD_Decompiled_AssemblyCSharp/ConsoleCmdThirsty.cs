using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200026D RID: 621
[Preserve]
public class ConsoleCmdThirsty : ConsoleCmdAbstract
{
	// Token: 0x170001CF RID: 463
	// (get) Token: 0x060011A0 RID: 4512 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060011A1 RID: 4513 RVA: 0x0006EF4C File Offset: 0x0006D14C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"thirsty"
		};
	}

	// Token: 0x060011A2 RID: 4514 RVA: 0x0006EF5C File Offset: 0x0006D15C
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
			primaryPlayer.Stats.Water.Value = (float)Mathf.CeilToInt(num / 100f * primaryPlayer.Stats.Water.ModifiedMax);
		}
	}

	// Token: 0x060011A3 RID: 4515 RVA: 0x0006EFD3 File Offset: 0x0006D1D3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Makes the player thirsty (optionally specify the amount of water you want to have in percent).";
	}
}
