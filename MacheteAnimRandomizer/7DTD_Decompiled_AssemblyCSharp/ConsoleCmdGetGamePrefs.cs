using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001E5 RID: 485
[Preserve]
public class ConsoleCmdGetGamePrefs : ConsoleCmdAbstract
{
	// Token: 0x06000E74 RID: 3700 RVA: 0x0005E404 File Offset: 0x0005C604
	[PublicizedFrom(EAccessModifier.Private)]
	public bool prefAccessAllowed(EnumGamePrefs gp)
	{
		string text = gp.ToStringCached<EnumGamePrefs>();
		foreach (string value in this.forbiddenPrefs)
		{
			if (text.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000E75 RID: 3701 RVA: 0x0005E43F File Offset: 0x0005C63F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"getgamepref",
			"gg"
		};
	}

	// Token: 0x1700013D RID: 317
	// (get) Token: 0x06000E76 RID: 3702 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700013E RID: 318
	// (get) Token: 0x06000E77 RID: 3703 RVA: 0x0005B5EB File Offset: 0x000597EB
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x0005E457 File Offset: 0x0005C657
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Gets game preferences";
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x0005E45E File Offset: 0x0005C65E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Get all game preferences or only those matching a given substring";
	}

	// Token: 0x06000E7A RID: 3706 RVA: 0x0005E468 File Offset: 0x0005C668
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		string text = null;
		if (_params.Count > 0)
		{
			text = _params[0];
		}
		SortedList<string, string> sortedList = new SortedList<string, string>();
		foreach (EnumGamePrefs enumGamePrefs in EnumUtils.Values<EnumGamePrefs>())
		{
			if ((string.IsNullOrEmpty(text) || enumGamePrefs.ToStringCached<EnumGamePrefs>().ContainsCaseInsensitive(text)) && this.prefAccessAllowed(enumGamePrefs))
			{
				sortedList.Add(enumGamePrefs.ToStringCached<EnumGamePrefs>(), string.Format("GamePref.{0} = {1}", enumGamePrefs.ToStringCached<EnumGamePrefs>(), GamePrefs.GetObject(enumGamePrefs)));
			}
		}
		foreach (string key in sortedList.Keys)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(sortedList[key]);
		}
	}

	// Token: 0x04000AFA RID: 2810
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] forbiddenPrefs = new string[]
	{
		"telnet",
		"adminfilename",
		"controlpanel",
		"password",
		"historycache",
		"userdatafolder",
		"options",
		"last"
	};
}
