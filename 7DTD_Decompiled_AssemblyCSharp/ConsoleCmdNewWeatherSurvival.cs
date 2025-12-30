using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000213 RID: 531
[Preserve]
public class ConsoleCmdNewWeatherSurvival : ConsoleCmdAbstract
{
	// Token: 0x06000F95 RID: 3989 RVA: 0x0006533D File Offset: 0x0006353D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"newweathersurvival"
		};
	}

	// Token: 0x06000F96 RID: 3990 RVA: 0x0006534D File Offset: 0x0006354D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Enables/disables new weather survival";
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x00065354 File Offset: 0x00063554
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count > 0)
		{
			if (_params.Count > 1 || (_params[0] != "on" && _params[0] != "off"))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: newweathersurvival [on/off]");
				return;
			}
			EntityStats.NewWeatherSurvivalEnabled = (_params[0] == "on");
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("New Weather survival is " + (EntityStats.NewWeatherSurvivalEnabled ? "on" : "off"));
	}
}
