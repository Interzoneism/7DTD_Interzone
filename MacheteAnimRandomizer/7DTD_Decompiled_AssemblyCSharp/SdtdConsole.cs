using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

// Token: 0x02000286 RID: 646
public class SdtdConsole : SingletonMonoBehaviour<SdtdConsole>
{
	// Token: 0x170001F1 RID: 497
	// (get) Token: 0x06001252 RID: 4690 RVA: 0x00071FB4 File Offset: 0x000701B4
	public int HideCommandExecutionLog
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (this.hideCommandExecutionLog < 0)
			{
				this.hideCommandExecutionLog = GamePrefs.GetInt(EnumGamePrefs.HideCommandExecutionLog);
			}
			return this.hideCommandExecutionLog;
		}
	}

	// Token: 0x06001253 RID: 4691 RVA: 0x00071FD5 File Offset: 0x000701D5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void singletonAwake()
	{
		Log.LogCallbacksExtended += this.LogCallback;
		this.m_CommandsReadOnly = new ReadOnlyCollection<IConsoleCommand>(this.m_Commands);
		PreserveCheckPatch.Enable();
	}

	// Token: 0x06001254 RID: 4692 RVA: 0x00072000 File Offset: 0x00070200
	[PublicizedFrom(EAccessModifier.Private)]
	public void LogCallback(string _formattedMessage, string _plainMessage, string _trace, LogType _type, DateTime _timestamp, long _uptime)
	{
		if (GameManager.IsDedicatedServer)
		{
			for (int i = 0; i < this.m_Servers.Count; i++)
			{
				IConsoleServer consoleServer = this.m_Servers[i];
				try
				{
					consoleServer.SendLog(_formattedMessage, _plainMessage, _trace, _type, _timestamp, _uptime);
				}
				catch (Exception e)
				{
					Log.Error("Error sending to console server:");
					Log.Exception(e);
				}
			}
		}
	}

	// Token: 0x06001255 RID: 4693 RVA: 0x0007206C File Offset: 0x0007026C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Update()
	{
		if (this.m_commandsToExecuteAsync.Count > 0)
		{
			List<SdtdConsole.CommandInstance> commandsToExecuteAsync = this.m_commandsToExecuteAsync;
			lock (commandsToExecuteAsync)
			{
				try
				{
					CommandSenderInfo senderInfo = new CommandSenderInfo
					{
						IsLocalGame = false,
						RemoteClientInfo = null,
						NetworkConnection = this.m_commandsToExecuteAsync[0].sender
					};
					List<string> output = this.executeCommand(this.m_commandsToExecuteAsync[0].command, senderInfo);
					this.m_commandsToExecuteAsync[0].sender.SendLines(output);
				}
				catch (Exception e)
				{
					Log.Exception(e);
				}
				this.m_commandsToExecuteAsync.RemoveAt(0);
			}
		}
	}

	// Token: 0x06001256 RID: 4694 RVA: 0x0007213C File Offset: 0x0007033C
	public void Output(string _line)
	{
		List<string> currentCommandOutputList = this.m_currentCommandOutputList;
		if (currentCommandOutputList == null)
		{
			return;
		}
		currentCommandOutputList.Add(_line);
	}

	// Token: 0x06001257 RID: 4695 RVA: 0x0007214F File Offset: 0x0007034F
	public void Output(string _format, params object[] _args)
	{
		this.Output(string.Format(_format, _args));
	}

	// Token: 0x06001258 RID: 4696 RVA: 0x00072160 File Offset: 0x00070360
	[PublicizedFrom(EAccessModifier.Private)]
	public void RegisterCommand(SortedList<string, IConsoleCommand> _commandsList, string _className, IConsoleCommand _command)
	{
		string[] commands = _command.GetCommands();
		string text = commands[0];
		if (_commandsList.ContainsKey(text))
		{
			Log.Warning("Command with name \"" + text + "\" already loaded, not loading from class " + _className);
			return;
		}
		_commandsList.Add(text, _command);
		if (GameManager.Instance.adminTools == null)
		{
			return;
		}
		if (!GameManager.Instance.adminTools.Commands.IsPermissionDefined(commands) && _command.DefaultPermissionLevel != 0)
		{
			Log.Out("Command \"{0}\" has no explicit permission level, but a default permission of {1}, adding to permission list", new object[]
			{
				text,
				_command.DefaultPermissionLevel
			});
			GameManager.Instance.adminTools.Commands.AddCommand(text, _command.DefaultPermissionLevel, false);
		}
	}

	// Token: 0x06001259 RID: 4697 RVA: 0x0007220C File Offset: 0x0007040C
	public void RegisterCommands()
	{
		SortedList<string, IConsoleCommand> commandsList = new SortedList<string, IConsoleCommand>();
		ReflectionHelpers.FindTypesImplementingBase(typeof(IConsoleCommand), delegate(Type _type)
		{
			IConsoleCommand consoleCommand3 = ReflectionHelpers.Instantiate<IConsoleCommand>(_type);
			if (consoleCommand3 != null)
			{
				this.RegisterCommand(commandsList, _type.Name, consoleCommand3);
			}
		}, false);
		try
		{
			foreach (IConsoleCommand consoleCommand in commandsList.Values)
			{
				this.m_Commands.Add(consoleCommand);
				for (int i = 0; i < consoleCommand.GetCommands().Length; i++)
				{
					string text = consoleCommand.GetCommands()[i];
					if (!string.IsNullOrEmpty(text))
					{
						IConsoleCommand consoleCommand2;
						if (this.m_CommandsAllVariants.TryGetValue(text, out consoleCommand2))
						{
							Log.Warning(string.Concat(new string[]
							{
								"Command with alias \"",
								text,
								"\" already registered from ",
								consoleCommand2.GetType().Name,
								", not registering for class ",
								consoleCommand.GetType().Name
							}));
						}
						else
						{
							this.m_CommandsAllVariants.Add(text, consoleCommand);
						}
					}
				}
			}
			ConsoleCmdHelp.ValidateNoCommandOverlap();
		}
		catch (Exception e)
		{
			Log.Error("Error registering commands");
			Log.Exception(e);
		}
		AdminTools adminTools = GameManager.Instance.adminTools;
		if (adminTools == null)
		{
			return;
		}
		adminTools.Save();
	}

	// Token: 0x0600125A RID: 4698 RVA: 0x0007236C File Offset: 0x0007056C
	public void RegisterServer(IConsoleServer _server)
	{
		if (_server != null)
		{
			this.m_Servers.Add(_server);
		}
	}

	// Token: 0x0600125B RID: 4699 RVA: 0x00072380 File Offset: 0x00070580
	public void Cleanup()
	{
		for (int i = 0; i < this.m_Servers.Count; i++)
		{
			IConsoleServer consoleServer = this.m_Servers[i];
			try
			{
				consoleServer.Disconnect();
			}
			catch (Exception e)
			{
				Log.Error("Error sending to console server:");
				Log.Exception(e);
			}
		}
		this.m_Servers.Clear();
	}

	// Token: 0x0600125C RID: 4700 RVA: 0x000723E4 File Offset: 0x000705E4
	public IConsoleCommand GetCommand(string _command, bool _alreadyTokenized = false)
	{
		if (!_alreadyTokenized)
		{
			int num = _command.IndexOf(' ');
			if (num >= 0)
			{
				_command = _command.Substring(0, num);
			}
		}
		IConsoleCommand result;
		if (this.m_CommandsAllVariants.TryGetValue(_command, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x0600125D RID: 4701 RVA: 0x0007241E File Offset: 0x0007061E
	public IList<IConsoleCommand> GetCommands()
	{
		return this.m_CommandsReadOnly;
	}

	// Token: 0x0600125E RID: 4702 RVA: 0x00072428 File Offset: 0x00070628
	public void ExecuteAsync(string _command, IConsoleConnection _sender)
	{
		SdtdConsole.CommandInstance item = new SdtdConsole.CommandInstance(_command, _sender);
		List<SdtdConsole.CommandInstance> commandsToExecuteAsync = this.m_commandsToExecuteAsync;
		lock (commandsToExecuteAsync)
		{
			this.m_commandsToExecuteAsync.Add(item);
		}
	}

	// Token: 0x0600125F RID: 4703 RVA: 0x00072478 File Offset: 0x00070678
	public List<string> ExecuteSync(string _command, ClientInfo _cInfo)
	{
		CommandSenderInfo senderInfo = new CommandSenderInfo
		{
			IsLocalGame = (_cInfo == null),
			RemoteClientInfo = _cInfo,
			NetworkConnection = null
		};
		return this.executeCommand(_command, senderInfo);
	}

	// Token: 0x06001260 RID: 4704 RVA: 0x000724B4 File Offset: 0x000706B4
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> executeCommand(string _command, CommandSenderInfo _senderInfo)
	{
		if (string.IsNullOrEmpty(_command))
		{
			return null;
		}
		this.m_currentCommandOutputList.Clear();
		List<string> list = this.tokenizeCommand(_command);
		if (list != null)
		{
			if (list[0] == string.Empty)
			{
				return null;
			}
			IConsoleCommand command = this.GetCommand(list[0], true);
			if (command != null)
			{
				if (!command.CanExecuteForDevice)
				{
					this.m_currentCommandOutputList.Add("*** ERROR: Command '" + list[0] + "' can not be executed on this device type.");
					return this.m_currentCommandOutputList;
				}
				if (GameManager.Instance.World == null && !command.AllowedInMainMenu)
				{
					this.m_currentCommandOutputList.Add("*** ERROR: Command '" + list[0] + "' can only be executed when a game is started.");
					return this.m_currentCommandOutputList;
				}
				if (_senderInfo.IsLocalGame)
				{
					if (this.HideCommandExecutionLog < 3)
					{
						Log.Out("Executing command '" + _command + "'");
					}
				}
				else if (_senderInfo.RemoteClientInfo != null)
				{
					if (this.HideCommandExecutionLog < 2)
					{
						string str = "Executing command '";
						string str2 = "' from client ";
						ClientInfo remoteClientInfo = _senderInfo.RemoteClientInfo;
						Log.Out(str + _command + str2 + ((remoteClientInfo != null) ? remoteClientInfo.ToString() : null));
					}
				}
				else if (this.HideCommandExecutionLog < 1)
				{
					Log.Out("Executing command '" + _command + "' by " + _senderInfo.NetworkConnection.GetDescription());
				}
				try
				{
					command.Execute(list.GetRange(1, list.Count - 1), _senderInfo);
					goto IL_1AE;
				}
				catch (Exception ex)
				{
					this.m_currentCommandOutputList.Add("*** ERROR: Executing command '" + list[0] + "' failed: " + ex.Message);
					Log.Exception(ex);
					goto IL_1AE;
				}
			}
			this.m_currentCommandOutputList.Add("*** ERROR: unknown command '" + list[0] + "'");
		}
		IL_1AE:
		return this.m_currentCommandOutputList;
	}

	// Token: 0x06001261 RID: 4705 RVA: 0x00072688 File Offset: 0x00070888
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> tokenizeCommand(string _command)
	{
		List<string> list = this.tokenizedCommandList;
		list.Clear();
		bool flag = false;
		int num = 0;
		for (int i = 0; i < _command.Length; i++)
		{
			if (!flag)
			{
				if (_command[i] == '"')
				{
					if (i - num > 0)
					{
						list.Add(_command.Substring(num, i - num));
					}
					num = i + 1;
					flag = true;
				}
				else if (_command[i] == ' ' || _command[i] == '\t')
				{
					if (i - num > 0)
					{
						list.Add(_command.Substring(num, i - num));
					}
					num = i + 1;
				}
			}
			else if (_command[i] == '"')
			{
				if (i + 1 < _command.Length && _command[i + 1] == '"')
				{
					i++;
				}
				else
				{
					string text = _command.Substring(num, i - num);
					text = text.Replace("\"\"", "\"");
					list.Add(text);
					num = i + 1;
					flag = false;
				}
			}
		}
		if (flag)
		{
			this.m_currentCommandOutputList.Add("*** ERROR: Quotation started at position " + num.ToString() + " was not closed");
			return null;
		}
		if (num < _command.Length)
		{
			list.Add(_command.Substring(num, _command.Length - num));
		}
		return list;
	}

	// Token: 0x04000BE4 RID: 3044
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<IConsoleCommand> m_Commands = new List<IConsoleCommand>();

	// Token: 0x04000BE5 RID: 3045
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly Dictionary<string, IConsoleCommand> m_CommandsAllVariants = new CaseInsensitiveStringDictionary<IConsoleCommand>();

	// Token: 0x04000BE6 RID: 3046
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ReadOnlyCollection<IConsoleCommand> m_CommandsReadOnly;

	// Token: 0x04000BE7 RID: 3047
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<IConsoleServer> m_Servers = new List<IConsoleServer>();

	// Token: 0x04000BE8 RID: 3048
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<SdtdConsole.CommandInstance> m_commandsToExecuteAsync = new List<SdtdConsole.CommandInstance>();

	// Token: 0x04000BE9 RID: 3049
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<string> m_currentCommandOutputList = new List<string>();

	// Token: 0x04000BEA RID: 3050
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int hideCommandExecutionLog = -1;

	// Token: 0x04000BEB RID: 3051
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<string> tokenizedCommandList = new List<string>(16);

	// Token: 0x02000287 RID: 647
	[PublicizedFrom(EAccessModifier.Private)]
	public class CommandInstance
	{
		// Token: 0x06001263 RID: 4707 RVA: 0x0007281A File Offset: 0x00070A1A
		public CommandInstance(string _command, IConsoleConnection _sender)
		{
			this.command = _command;
			this.sender = _sender;
		}

		// Token: 0x04000BEC RID: 3052
		public readonly string command;

		// Token: 0x04000BED RID: 3053
		public readonly IConsoleConnection sender;
	}
}
