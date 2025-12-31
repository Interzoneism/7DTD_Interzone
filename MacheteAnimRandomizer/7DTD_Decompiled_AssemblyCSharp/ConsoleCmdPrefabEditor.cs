using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000229 RID: 553
[Preserve]
public class ConsoleCmdPrefabEditor : ConsoleCmdAbstract
{
	// Token: 0x06001025 RID: 4133 RVA: 0x000688A2 File Offset: 0x00066AA2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"prefabeditor",
			"prefabedit",
			"predit"
		};
	}

	// Token: 0x1700018F RID: 399
	// (get) Token: 0x06001026 RID: 4134 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x000688C2 File Offset: 0x00066AC2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Open the Prefab Editor";
	}

	// Token: 0x06001028 RID: 4136 RVA: 0x000688C9 File Offset: 0x00066AC9
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "prefabeditor";
	}

	// Token: 0x06001029 RID: 4137 RVA: 0x000688D0 File Offset: 0x00066AD0
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (PrefabEditModeManager.Instance.IsActive())
		{
			Log.Out("You are already in the prefab editor.");
			return;
		}
		if (!GameManager.Instance.IsSafeToConnect())
		{
			Log.Warning("Please return to the main menu before using this command.");
			return;
		}
		XUiC_EditingTools.OpenPrefabEditor(null);
	}

	// Token: 0x04000B64 RID: 2916
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_connectingToPrefabEditor;
}
