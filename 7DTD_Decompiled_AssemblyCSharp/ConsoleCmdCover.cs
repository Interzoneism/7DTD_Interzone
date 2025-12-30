using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001CA RID: 458
[Preserve]
public class ConsoleCmdCover : ConsoleCmdAbstract
{
	// Token: 0x17000119 RID: 281
	// (get) Token: 0x06000DD8 RID: 3544 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x0005C9FC File Offset: 0x0005ABFC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"tcs",
			"testCoverSystem"
		};
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x0005CA14 File Offset: 0x0005AC14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "CoverSystem queries.";
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x0005CA1C File Offset: 0x0005AC1C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		GameManager.Instance.World.GetPrimaryPlayer();
		if (_params.Count == 0)
		{
			EntityCoverManager.DebugModeEnabled = !EntityCoverManager.DebugModeEnabled;
			Log.Warning("coverSystem" + string.Format(" - enabled:{0}", EntityCoverManager.DebugModeEnabled));
			return;
		}
		if (_params[0].ContainsCaseInsensitive("help"))
		{
			Log.Out("coverSystem help:" + Environment.NewLine);
			return;
		}
	}
}
