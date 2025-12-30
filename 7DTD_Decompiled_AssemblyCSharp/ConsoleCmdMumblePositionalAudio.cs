using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200020F RID: 527
[Preserve]
public class ConsoleCmdMumblePositionalAudio : ConsoleCmdAbstract
{
	// Token: 0x06000F80 RID: 3968 RVA: 0x0006506D File Offset: 0x0006326D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"mumblepositionalaudio",
			"mpa"
		};
	}

	// Token: 0x06000F81 RID: 3969 RVA: 0x00065085 File Offset: 0x00063285
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Mumble Positional Audio related tools";
	}

	// Token: 0x06000F82 RID: 3970 RVA: 0x0006508C File Offset: 0x0006328C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "\r\n\t\t\t|Usage:\r\n\t\t\t|  1. mpa enable\r\n\t\t\t|  2. mpa disable\r\n\t\t\t|  3. mpa reinit\r\n\t\t\t".Unindent(true);
	}

	// Token: 0x17000177 RID: 375
	// (get) Token: 0x06000F83 RID: 3971 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000F84 RID: 3972 RVA: 0x0006509C File Offset: 0x0006329C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count != 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 1, found " + _params.Count.ToString() + ".");
			return;
		}
		if (_params[0].EqualsCaseInsensitive("enable"))
		{
			if (SingletonMonoBehaviour<MumblePositionalAudio>.Instance != null)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Positional Audio already enabled");
				return;
			}
			MumblePositionalAudio.Init();
			return;
		}
		else if (_params[0].EqualsCaseInsensitive("disable"))
		{
			if (SingletonMonoBehaviour<MumblePositionalAudio>.Instance == null)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Positional Audio already disabled");
				return;
			}
			MumblePositionalAudio.Destroy();
			return;
		}
		else if (_params[0].EqualsCaseInsensitive("reinit"))
		{
			if (SingletonMonoBehaviour<MumblePositionalAudio>.Instance == null)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Positional Audio not enabled");
				return;
			}
			SingletonMonoBehaviour<MumblePositionalAudio>.Instance.ReinitShm();
			return;
		}
		else
		{
			if (!_params[0].EqualsCaseInsensitive("uiversion"))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid subcommand");
				return;
			}
			if (SingletonMonoBehaviour<MumblePositionalAudio>.Instance == null)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Positional Audio not enabled");
				return;
			}
			SingletonMonoBehaviour<MumblePositionalAudio>.Instance.printUiVersion();
			return;
		}
	}
}
