using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000241 RID: 577
[Preserve]
public class ConsoleCmdSetGamePref : ConsoleCmdAbstract
{
	// Token: 0x060010A8 RID: 4264 RVA: 0x0006A9DB File Offset: 0x00068BDB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"setgamepref",
			"sg"
		};
	}

	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x060010A9 RID: 4265 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060010AA RID: 4266 RVA: 0x0006A9F4 File Offset: 0x00068BF4
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count != 2)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Parameters: <game pref> <value>");
			return;
		}
		EnumGamePrefs enumGamePrefs;
		try
		{
			enumGamePrefs = EnumUtils.Parse<EnumGamePrefs>(_params[0], true);
		}
		catch (Exception)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error parsing parameter: " + _params[0]);
			return;
		}
		object obj;
		try
		{
			obj = GamePrefs.Parse(enumGamePrefs, _params[1]);
		}
		catch (Exception)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Error parsing value: " + _params[1]);
			return;
		}
		GamePrefs.SetObject(enumGamePrefs, obj);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(enumGamePrefs.ToStringCached<EnumGamePrefs>() + " set to " + ((obj != null) ? obj.ToString() : null));
	}

	// Token: 0x060010AB RID: 4267 RVA: 0x0006AAC4 File Offset: 0x00068CC4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "sets a game pref";
	}
}
