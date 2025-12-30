using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001E3 RID: 483
[Preserve]
public class ConsoleCmdFov : ConsoleCmdAbstract
{
	// Token: 0x06000E67 RID: 3687 RVA: 0x0005E2C0 File Offset: 0x0005C4C0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"fov"
		};
	}

	// Token: 0x17000139 RID: 313
	// (get) Token: 0x06000E68 RID: 3688 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000E69 RID: 3689 RVA: 0x0005E2D0 File Offset: 0x0005C4D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Camera field of view";
	}

	// Token: 0x06000E6A RID: 3690 RVA: 0x0002B133 File Offset: 0x00029333
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "";
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x0005E2D8 File Offset: 0x0005C4D8
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		int value;
		if (_params.Count == 1 && int.TryParse(_params[0], out value))
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxFOV, value);
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Set FOV to " + value.ToString());
		}
	}
}
