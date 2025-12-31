using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001D6 RID: 470
[Preserve]
public class ConsoleCmdDebugShot : ConsoleCmdAbstract
{
	// Token: 0x17000122 RID: 290
	// (get) Token: 0x06000E16 RID: 3606 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000123 RID: 291
	// (get) Token: 0x06000E17 RID: 3607 RVA: 0x0005B5EB File Offset: 0x000597EB
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x17000124 RID: 292
	// (get) Token: 0x06000E18 RID: 3608 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000E19 RID: 3609 RVA: 0x0005D580 File Offset: 0x0005B780
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"debugshot",
			"dbs"
		};
	}

	// Token: 0x06000E1A RID: 3610 RVA: 0x0005D598 File Offset: 0x0005B798
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		LocalPlayerUI.primaryUI.windowManager.Close(GUIWindowConsole.ID);
		bool savePerks = _params.Count > 0 && StringParsers.ParseBool(_params[0], 0, -1, true);
		ThreadManager.StartCoroutine(this.openWindowLater(savePerks));
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x0005D5E2 File Offset: 0x0005B7E2
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator openWindowLater(bool _savePerks)
	{
		yield return null;
		GUIWindowScreenshotText.Open(LocalPlayerUI.primaryUI, _savePerks);
		yield break;
	}

	// Token: 0x06000E1C RID: 3612 RVA: 0x0005D5F1 File Offset: 0x0005B7F1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Creates a screenshot with some debug information";
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x0005D5F8 File Offset: 0x0005B7F8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\n  debugshot [save perks]\nLets you make a screenshot that will have some generic info\non it and a custom text you can enter. Also stores a list\nof your current perk levels, buffs and cvars in a CSV file\nnext to it if the optional parameter 'save perks' is set to true";
	}
}
