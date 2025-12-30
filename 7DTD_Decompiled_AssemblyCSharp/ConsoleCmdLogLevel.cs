using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000203 RID: 515
[Preserve]
public class ConsoleCmdLogLevel : ConsoleCmdAbstract
{
	// Token: 0x06000F39 RID: 3897 RVA: 0x00063A17 File Offset: 0x00061C17
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"loglevel"
		};
	}

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x06000F3A RID: 3898 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x00063A27 File Offset: 0x00061C27
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Telnet/Web only: Select which types of log messages are shown";
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x00063A2E File Offset: 0x00061C2E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Select which types of log messages are shown on the connection\nyou enter this command. By default all log messages are printed\non every connection.\nUsage: loglevel <loglevel name> <true/false>\nLog levels: INF, WRN, ERR, EXC or ALL\nExample: Disable display of WRN messages: loglevel WRN false";
	}

	// Token: 0x06000F3D RID: 3901 RVA: 0x00063A38 File Offset: 0x00061C38
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_senderInfo.IsLocalGame || _senderInfo.RemoteClientInfo != null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("This command can only be used through a management interface like Telnet or the Web Panel.");
			return;
		}
		if (_params.Count != 2)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments.");
			return;
		}
		LogType type = LogType.Assert;
		bool flag = false;
		if (_params[0].EqualsCaseInsensitive("all"))
		{
			flag = true;
		}
		else if (_params[0].EqualsCaseInsensitive("inf"))
		{
			type = LogType.Log;
		}
		else if (_params[0].EqualsCaseInsensitive("wrn"))
		{
			type = LogType.Warning;
		}
		else if (_params[0].EqualsCaseInsensitive("err"))
		{
			type = LogType.Error;
		}
		else
		{
			if (!_params[0].EqualsCaseInsensitive("exc"))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("\"" + _params[0] + "\" is not a valid loglevel name.");
				return;
			}
			type = LogType.Exception;
		}
		bool flag2;
		try
		{
			flag2 = ConsoleHelper.ParseParamBool(_params[1], false);
		}
		catch (ArgumentException)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("\"" + _params[1] + "\" is not a valid boolean value.");
			return;
		}
		if (flag)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output((flag2 ? "Enabling" : "Disabling") + " all loglevels on this connection.");
			_senderInfo.NetworkConnection.EnableLogLevel(LogType.Assert, flag2);
			_senderInfo.NetworkConnection.EnableLogLevel(LogType.Error, flag2);
			_senderInfo.NetworkConnection.EnableLogLevel(LogType.Exception, flag2);
			_senderInfo.NetworkConnection.EnableLogLevel(LogType.Log, flag2);
			_senderInfo.NetworkConnection.EnableLogLevel(LogType.Warning, flag2);
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output((flag2 ? "Enabling" : "Disabling") + " loglevel \"" + type.ToString() + "\" on this connection.");
		_senderInfo.NetworkConnection.EnableLogLevel(type, flag2);
	}
}
