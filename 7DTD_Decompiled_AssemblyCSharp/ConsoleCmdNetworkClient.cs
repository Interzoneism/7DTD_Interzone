using System;
using UnityEngine.Scripting;

// Token: 0x02000210 RID: 528
[Preserve]
public class ConsoleCmdNetworkClient : ConsoleCmdNetworkServer
{
	// Token: 0x17000178 RID: 376
	// (get) Token: 0x06000F86 RID: 3974 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000F87 RID: 3975 RVA: 0x000651CB File Offset: 0x000633CB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"networkclient",
			"netc"
		};
	}

	// Token: 0x06000F88 RID: 3976 RVA: 0x000651E3 File Offset: 0x000633E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Client side network commands";
	}
}
