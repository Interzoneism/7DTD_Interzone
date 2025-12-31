using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001BC RID: 444
[Preserve]
public class ConsoleCmdBugReportOcclusionManager : ConsoleCmdAbstract
{
	// Token: 0x17000109 RID: 265
	// (get) Token: 0x06000D84 RID: 3460 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000D85 RID: 3461 RVA: 0x0005AA01 File Offset: 0x00058C01
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"testoccreport",
			"toccr"
		};
	}

	// Token: 0x06000D86 RID: 3462 RVA: 0x0005AA19 File Offset: 0x00058C19
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Test the occlusion manager self reporting to backtrace, requires Backtrace to be enabled at build creation";
	}

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x06000D87 RID: 3463 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool AllowedInMainMenu
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000D88 RID: 3464 RVA: 0x0005AA20 File Offset: 0x00058C20
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled))
		{
			return;
		}
		List<string> list;
		if (OcclusionManager.Instance.WriteListToDisk(out list))
		{
			BacktraceUtils.SendErrorReport("OcclusionManagerUsedUpAllEntries", "Occlusion Manager used all entries", list);
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("testoccreport: Wrote Files to disk: " + string.Join(",", list));
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("testoccreport: Posted report to backtrace");
		}
	}
}
