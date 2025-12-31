using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001B3 RID: 435
[Preserve]
public class ConsoleCmdAIDirectorSpawnHorde : ConsoleCmdAbstract
{
	// Token: 0x06000D49 RID: 3401 RVA: 0x00059252 File Offset: 0x00057452
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"spawnwandering",
			"spawnw"
		};
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x0005926A File Offset: 0x0005746A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Spawn wandering entities";
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x00059271 File Offset: 0x00057471
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Commands:\nb - bandits\nh - horde";
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x00059278 File Offset: 0x00057478
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
			return;
		}
		string text = _params[0].ToLower();
		if (text == "b")
		{
			GameManager.Instance.World.aiDirector.GetComponent<AIDirectorWanderingHordeComponent>().StartSpawning(AIWanderingHordeSpawner.SpawnType.Bandits);
			return;
		}
		if (!(text == "h"))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Unknown command " + text);
			return;
		}
		GameManager.Instance.World.aiDirector.GetComponent<AIDirectorWanderingHordeComponent>().StartSpawning(AIWanderingHordeSpawner.SpawnType.Horde);
	}
}
