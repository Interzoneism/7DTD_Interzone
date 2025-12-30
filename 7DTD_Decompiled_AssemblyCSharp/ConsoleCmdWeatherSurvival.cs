using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200027A RID: 634
[Preserve]
public class ConsoleCmdWeatherSurvival : ConsoleCmdAbstract
{
	// Token: 0x060011FD RID: 4605 RVA: 0x000708C5 File Offset: 0x0006EAC5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"weathersurvival"
		};
	}

	// Token: 0x060011FE RID: 4606 RVA: 0x000708D5 File Offset: 0x0006EAD5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Enables/disables weather survival";
	}

	// Token: 0x060011FF RID: 4607 RVA: 0x000708DC File Offset: 0x0006EADC
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count > 0)
		{
			if (_params.Count > 1 || (_params[0] != "on" && _params[0] != "off"))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: weathersurvival [on/off]");
				return;
			}
			EntityStats.WeatherSurvivalEnabled = (_params[0] == "on");
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Weather survival is " + (EntityStats.WeatherSurvivalEnabled ? "on" : "off"));
	}
}
