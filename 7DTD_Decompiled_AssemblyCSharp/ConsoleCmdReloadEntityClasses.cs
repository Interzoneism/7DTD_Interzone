using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000235 RID: 565
[Preserve]
public class ConsoleCmdReloadEntityClasses : ConsoleCmdAbstract
{
	// Token: 0x1700019C RID: 412
	// (get) Token: 0x06001076 RID: 4214 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001077 RID: 4215 RVA: 0x0006A248 File Offset: 0x00068448
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"reloadentityclasses",
			"rec"
		};
	}

	// Token: 0x06001078 RID: 4216 RVA: 0x0006A260 File Offset: 0x00068460
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "reloads entityclasses xml data.";
	}

	// Token: 0x06001079 RID: 4217 RVA: 0x0006A267 File Offset: 0x00068467
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		WorldStaticData.Reset("entityclasses");
	}
}
