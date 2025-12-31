using System;
using UnityEngine.Scripting;

// Token: 0x0200020C RID: 524
[Preserve]
public class ConsoleCmdMemCl : ConsoleCmdMem
{
	// Token: 0x1700016F RID: 367
	// (get) Token: 0x06000F6C RID: 3948 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000170 RID: 368
	// (get) Token: 0x06000F6D RID: 3949 RVA: 0x0005B5EB File Offset: 0x000597EB
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x06000F6E RID: 3950 RVA: 0x00064EC1 File Offset: 0x000630C1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"memcl"
		};
	}

	// Token: 0x06000F6F RID: 3951 RVA: 0x00064ED1 File Offset: 0x000630D1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Prints memory information on client and calls garbage collector";
	}
}
