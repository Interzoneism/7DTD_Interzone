using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Scripting;

// Token: 0x020001C9 RID: 457
[Preserve]
public class ConsoleCmdConfig : ConsoleCmdAbstract
{
	// Token: 0x17000118 RID: 280
	// (get) Token: 0x06000DD2 RID: 3538 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x0005C948 File Offset: 0x0005AB48
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"config"
		};
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x0005C958 File Offset: 0x0005AB58
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Import/export config data from/to external file";
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x0005C95F File Offset: 0x0005AB5F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Imports/exports config data from/to external file\nUsage:\n   config import [filename]\n   config export [filename]\n";
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x0005C968 File Offset: 0x0005AB68
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		try
		{
			string text = _params[1].ToLower();
			if (!string.IsNullOrEmpty(text))
			{
				string a = _params[0];
				if (!(a == "import"))
				{
					if (a == "export")
					{
						if (File.Exists(text))
						{
							File.Delete(text);
						}
						GameOptionsManager.SaveControls();
						File.WriteAllText(text, SdPlayerPrefs.GetString("Controls"));
					}
				}
				else if (File.Exists(text))
				{
					GameOptionsManager.LoadControls(File.ReadAllText(text));
				}
			}
		}
		catch
		{
		}
	}
}
