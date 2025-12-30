using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000242 RID: 578
[Preserve]
public class ConsoleCmdSetGameStat : ConsoleCmdAbstract
{
	// Token: 0x060010AD RID: 4269 RVA: 0x0006AACB File Offset: 0x00068CCB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"setgamestat",
			"sgs"
		};
	}

	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x060010AE RID: 4270 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060010AF RID: 4271 RVA: 0x0006AAE4 File Offset: 0x00068CE4
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count != 2)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Parameters: <game stat> <value>");
			return;
		}
		EnumGameStats enumGameStats;
		try
		{
			enumGameStats = EnumUtils.Parse<EnumGameStats>(_params[0], true);
		}
		catch (Exception)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error parsing parameter: " + _params[0]);
			return;
		}
		object obj;
		try
		{
			obj = GameStats.Parse(enumGameStats, _params[1]);
		}
		catch (Exception)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error parsing value: " + _params[1]);
			return;
		}
		GameStats.SetObject(enumGameStats, obj);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(enumGameStats.ToStringCached<EnumGameStats>() + " set to " + ((obj != null) ? obj.ToString() : null));
	}

	// Token: 0x060010B0 RID: 4272 RVA: 0x0006ABB4 File Offset: 0x00068DB4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "sets a game stat";
	}
}
