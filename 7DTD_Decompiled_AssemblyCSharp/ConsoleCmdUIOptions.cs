using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000273 RID: 627
[Preserve]
public class ConsoleCmdUIOptions : ConsoleCmdAbstract
{
	// Token: 0x060011C5 RID: 4549 RVA: 0x0006F5D0 File Offset: 0x0006D7D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"uioptions",
			"uio"
		};
	}

	// Token: 0x170001D8 RID: 472
	// (get) Token: 0x060011C6 RID: 4550 RVA: 0x0005B5EB File Offset: 0x000597EB
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x170001D9 RID: 473
	// (get) Token: 0x060011C7 RID: 4551 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001DA RID: 474
	// (get) Token: 0x060011C8 RID: 4552 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060011C9 RID: 4553 RVA: 0x0006F5E8 File Offset: 0x0006D7E8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Allows overriding of some options that control the presentation of the UI";
	}

	// Token: 0x060011CA RID: 4554 RVA: 0x0006F5EF File Offset: 0x0006D7EF
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Commands:\noptionsvideowindow <value> - set the options window to use for video settings\n[no parameters] - toggles video settings between simplified and detailed modes\n";
	}

	// Token: 0x060011CB RID: 4555 RVA: 0x0006F5F8 File Offset: 0x0006D7F8
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			UIOptions.OptionsVideoWindow = ((UIOptions.OptionsVideoWindow == OptionsVideoWindowMode.Simplified) ? OptionsVideoWindowMode.Detailed : OptionsVideoWindowMode.Simplified);
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Set UIOptions.OptionsVideoWindow: {0}", UIOptions.OptionsVideoWindow));
			return;
		}
		if (_params[0].ToLowerInvariant() == "optionsvideowindow")
		{
			bool flag = false;
			if (_params.Count > 1)
			{
				OptionsVideoWindowMode optionsVideoWindow;
				if (EnumUtils.TryParse<OptionsVideoWindowMode>(_params[1], out optionsVideoWindow, true))
				{
					UIOptions.OptionsVideoWindow = optionsVideoWindow;
					flag = true;
				}
				else
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Unknown window type " + _params[1]);
				}
			}
			if (!flag)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Valid values: " + string.Join<OptionsVideoWindowMode>(',', EnumUtils.Values<OptionsVideoWindowMode>()));
			}
		}
	}
}
