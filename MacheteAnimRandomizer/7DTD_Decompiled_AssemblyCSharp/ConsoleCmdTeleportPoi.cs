using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000268 RID: 616
[Preserve]
public class ConsoleCmdTeleportPoi : ConsoleCmdAbstract
{
	// Token: 0x06001183 RID: 4483 RVA: 0x0006E2C0 File Offset: 0x0006C4C0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"tppoi"
		};
	}

	// Token: 0x170001CB RID: 459
	// (get) Token: 0x06001184 RID: 4484 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001185 RID: 4485 RVA: 0x0006E2D0 File Offset: 0x0006C4D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Open POI Teleporter window";
	}

	// Token: 0x06001186 RID: 4486 RVA: 0x0006E2D8 File Offset: 0x0006C4D8
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!_senderInfo.IsLocalGame)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Command can only be used on game clients");
			return;
		}
		if (_params.Count == 0)
		{
			GameManager.Instance.SetConsoleWindowVisible(false);
			LocalPlayerUI.GetUIForPrimaryPlayer().windowManager.Open(XUiC_PoiTeleportMenu.ID, true, false, true);
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Not implemented yet");
	}
}
