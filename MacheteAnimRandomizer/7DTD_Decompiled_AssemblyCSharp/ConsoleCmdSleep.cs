using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine.Scripting;

// Token: 0x02000253 RID: 595
[Preserve]
public class ConsoleCmdSleep : ConsoleCmdAbstract
{
	// Token: 0x06001110 RID: 4368 RVA: 0x0006BB62 File Offset: 0x00069D62
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"sleep"
		};
	}

	// Token: 0x06001111 RID: 4369 RVA: 0x0006BB72 File Offset: 0x00069D72
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Makes the main thread sleep for the given number of seconds (allows decimals)";
	}

	// Token: 0x06001112 RID: 4370 RVA: 0x0006BB7C File Offset: 0x00069D7C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		float num = 1f;
		if (_params.Count >= 1 && !StringParsers.TryParseFloat(_params[0], out num, 0, -1, NumberStyles.Any))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Argument is not a valid float");
			return;
		}
		Thread.Sleep((int)(num * 1000f));
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Slept for {0} seconds", num));
	}
}
