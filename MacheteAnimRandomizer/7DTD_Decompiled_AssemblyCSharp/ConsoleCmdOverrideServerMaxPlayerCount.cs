using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000215 RID: 533
[Preserve]
public class ConsoleCmdOverrideServerMaxPlayerCount : ConsoleCmdAbstract
{
	// Token: 0x1700017C RID: 380
	// (get) Token: 0x06000FA0 RID: 4000 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000FA1 RID: 4001 RVA: 0x0006558A File Offset: 0x0006378A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"overridemaxplayercount"
		};
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x0006559A File Offset: 0x0006379A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Override Max Server Player Count";
	}

	// Token: 0x06000FA3 RID: 4003 RVA: 0x000655A4 File Offset: 0x000637A4
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		int num;
		if (_params.Count >= 1 && int.TryParse(_params[0], out num))
		{
			GameModeSurvival.OverrideMaxPlayerCount = num;
			Log.Out(string.Format("Survival Max Player Count Override set to {0}", num));
			return;
		}
		Log.Out("Incorrect param, expected an integer for max player count.");
	}
}
