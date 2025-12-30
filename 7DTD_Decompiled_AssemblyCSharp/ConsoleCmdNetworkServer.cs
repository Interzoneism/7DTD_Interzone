using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000211 RID: 529
[Preserve]
public class ConsoleCmdNetworkServer : ConsoleCmdAbstract
{
	// Token: 0x06000F8A RID: 3978 RVA: 0x000651F2 File Offset: 0x000633F2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"networkserver",
			"nets"
		};
	}

	// Token: 0x17000179 RID: 377
	// (get) Token: 0x06000F8B RID: 3979 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000F8C RID: 3980 RVA: 0x0006520A File Offset: 0x0006340A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Server side network commands";
	}

	// Token: 0x06000F8D RID: 3981 RVA: 0x00065211 File Offset: 0x00063411
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Commands:\nlatencysim <min> <max> - sets simulation in millisecs (0 min disables)\npacketlosssim <chance> - sets simulation in percent (0 - 50)";
	}

	// Token: 0x06000F8E RID: 3982 RVA: 0x00065218 File Offset: 0x00063418
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
			return;
		}
		string text = _params[0].ToLower();
		if (text == "ls" || text == "latencysim")
		{
			int num = 0;
			int max = 100;
			if (_params.Count >= 2)
			{
				int.TryParse(_params[1], out num);
			}
			if (_params.Count >= 3)
			{
				int.TryParse(_params[2], out max);
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SetLatencySimulation(num > 0, num, max);
			return;
		}
		if (!(text == "pls") && !(text == "packetlosssim"))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Unknown command " + text + ".");
			return;
		}
		int num2 = 0;
		if (_params.Count >= 2)
		{
			int.TryParse(_params[1], out num2);
		}
		if (num2 > 50)
		{
			num2 = 50;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SetPacketLossSimulation(num2 > 0, num2);
	}
}
