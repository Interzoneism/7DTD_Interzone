using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000212 RID: 530
[Preserve]
public class ConsoleCmdNewAvatarTest : ConsoleCmdAbstract
{
	// Token: 0x06000F90 RID: 3984 RVA: 0x00065313 File Offset: 0x00063513
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Test new HD stuff.";
	}

	// Token: 0x06000F91 RID: 3985 RVA: 0x0006531A File Offset: 0x0006351A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\n  1. na\n";
	}

	// Token: 0x06000F92 RID: 3986 RVA: 0x00065321 File Offset: 0x00063521
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"na"
		};
	}

	// Token: 0x06000F93 RID: 3987 RVA: 0x00065331 File Offset: 0x00063531
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		Log.Warning("No New Avatar!");
	}
}
