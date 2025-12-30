using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000243 RID: 579
[Preserve]
public class ConsoleCmdSetTargetFps : ConsoleCmdAbstract
{
	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x060010B2 RID: 4274 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x060010B3 RID: 4275 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060010B4 RID: 4276 RVA: 0x0006ABBB File Offset: 0x00068DBB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"settargetfps"
		};
	}

	// Token: 0x060010B5 RID: 4277 RVA: 0x0006ABCB File Offset: 0x00068DCB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Set the target FPS the game should run at (upper limit)";
	}

	// Token: 0x060010B6 RID: 4278 RVA: 0x0006ABD2 File Offset: 0x00068DD2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Set the target FPS the game should run at (upper limit).\nUsage:\n  1. settargetfps\n  2. settargetfps <fps>\n1. gets the current target FPS.\n2. sets the target FPS to the given integer value, 0 disables the FPS limiter.";
	}

	// Token: 0x060010B7 RID: 4279 RVA: 0x0006ABDC File Offset: 0x00068DDC
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			int targetFPS = GameManager.Instance.waitForTargetFPS.TargetFPS;
			if (targetFPS > 0)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Current FPS limit is " + targetFPS.ToString());
				return;
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("FPS limiter is currently disabled");
			return;
		}
		else
		{
			if (_params.Count != 1)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 0 or 1, found " + _params.Count.ToString() + ".");
				return;
			}
			int num;
			if (!int.TryParse(_params[0], out num))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("\"" + _params[0] + "\" is not a valid integer.");
				return;
			}
			if (num < 0)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("FPS must be >= 0");
				return;
			}
			GameManager.Instance.waitForTargetFPS.TargetFPS = num;
			if (num > 0)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Set FPS limit to " + num.ToString());
				return;
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Disabled target FPS limiter");
			return;
		}
	}
}
