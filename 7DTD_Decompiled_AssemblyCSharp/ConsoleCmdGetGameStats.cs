using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001E6 RID: 486
[Preserve]
public class ConsoleCmdGetGameStats : ConsoleCmdAbstract
{
	// Token: 0x06000E7C RID: 3708 RVA: 0x0005E5B4 File Offset: 0x0005C7B4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool prefAccessAllowed(EnumGameStats gp)
	{
		string text = gp.ToStringCached<EnumGameStats>();
		foreach (string value in this.forbiddenPrefs)
		{
			if (text.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000E7D RID: 3709 RVA: 0x0005E5EF File Offset: 0x0005C7EF
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"getgamestat",
			"ggs"
		};
	}

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06000E7E RID: 3710 RVA: 0x0005B5EB File Offset: 0x000597EB
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x17000140 RID: 320
	// (get) Token: 0x06000E7F RID: 3711 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x0005E607 File Offset: 0x0005C807
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Gets game stats";
	}

	// Token: 0x06000E81 RID: 3713 RVA: 0x0005E60E File Offset: 0x0005C80E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Get all game stats or only those matching a given substring";
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x0005E618 File Offset: 0x0005C818
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		string text = null;
		if (_params.Count > 0)
		{
			text = _params[0];
		}
		SortedList<string, string> sortedList = new SortedList<string, string>();
		foreach (EnumGameStats enumGameStats in EnumUtils.Values<EnumGameStats>())
		{
			if ((string.IsNullOrEmpty(text) || enumGameStats.ToStringCached<EnumGameStats>().ContainsCaseInsensitive(text)) && this.prefAccessAllowed(enumGameStats))
			{
				sortedList.Add(enumGameStats.ToStringCached<EnumGameStats>(), string.Format("GameStat.{0} = {1}", enumGameStats.ToStringCached<EnumGameStats>(), GameStats.GetObject(enumGameStats)));
			}
		}
		foreach (string key in sortedList.Keys)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(sortedList[key]);
		}
	}

	// Token: 0x04000AFB RID: 2811
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] forbiddenPrefs = new string[]
	{
		"last"
	};
}
