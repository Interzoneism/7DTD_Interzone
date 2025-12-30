using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Scripting;

// Token: 0x020001D9 RID: 473
[Preserve]
public class ConsoleCmdDecoMgr : ConsoleCmdAbstract
{
	// Token: 0x06000E2D RID: 3629 RVA: 0x0005D6E4 File Offset: 0x0005B8E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"decomgr"
		};
	}

	// Token: 0x06000E2E RID: 3630 RVA: 0x0005D6F4 File Offset: 0x0005B8F4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "\"decomgr\": Saves a debug texture visualising the DecoOccupiedMap.\n\"decomgr state\": Saves a debug texture visualising the location/state of all of the DecoObjects saved in decorations.7dtd.";
	}

	// Token: 0x1700012B RID: 299
	// (get) Token: 0x06000E2F RID: 3631 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700012C RID: 300
	// (get) Token: 0x06000E30 RID: 3632 RVA: 0x0005B5EB File Offset: 0x000597EB
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x06000E31 RID: 3633 RVA: 0x0005D6FC File Offset: 0x0005B8FC
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count > 0 && _params[0] == "state")
		{
			DecoManager.Instance.SaveStateDebugTexture(Path.Join(GameIO.GetApplicationTempPath(), "decostate.png"));
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Saved decostate.png to temp directory.");
			return;
		}
		DecoManager.Instance.SaveDebugTexture(Path.Join(GameIO.GetApplicationTempPath(), "deco.png"), false);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Saved deco.png to temp directory.");
	}
}
