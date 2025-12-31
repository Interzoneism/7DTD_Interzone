using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200153F RID: 5439
	public class TwitchCommandEnableCommand : BaseTwitchCommand
	{
		// Token: 0x1700126D RID: 4717
		// (get) Token: 0x0600A780 RID: 42880 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x1700126E RID: 4718
		// (get) Token: 0x0600A781 RID: 42881 RVA: 0x004212F0 File Offset: 0x0041F4F0
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#enable"
				};
			}
		}

		// Token: 0x1700126F RID: 4719
		// (get) Token: 0x0600A782 RID: 42882 RVA: 0x00421300 File Offset: 0x0041F500
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_Enable", false)
				};
			}
		}

		// Token: 0x0600A783 RID: 42883 RVA: 0x00421318 File Offset: 0x0041F518
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				TwitchManager twitchManager = TwitchManager.Current;
				bool flag = false;
				foreach (string key in TwitchActionManager.TwitchActions.Keys)
				{
					TwitchAction twitchAction = TwitchActionManager.TwitchActions[key];
					if (twitchAction.IsInPreset(twitchManager.CurrentActionPreset) && twitchAction.Command.Equals(array[1]))
					{
						twitchAction.Enabled = true;
						flag = true;
					}
				}
				if (flag)
				{
					twitchManager.SendChannelMessage("[7DTD]: Command Enabled: " + array[1], true);
					twitchManager.SetupAvailableCommands();
				}
			}
		}

		// Token: 0x0600A784 RID: 42884 RVA: 0x004213E0 File Offset: 0x0041F5E0
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				TwitchManager twitchManager = TwitchManager.Current;
				bool flag = false;
				foreach (string key in TwitchActionManager.TwitchActions.Keys)
				{
					TwitchAction twitchAction = TwitchActionManager.TwitchActions[key];
					if (twitchAction.IsInPreset(twitchManager.CurrentActionPreset) && twitchAction.Command.Equals(arguments[1]))
					{
						twitchAction.Enabled = true;
						flag = true;
					}
				}
				if (flag)
				{
					twitchManager.SendChannelMessage("[7DTD]: Command Enabled: " + arguments[1], true);
					twitchManager.SetupAvailableCommands();
				}
			}
		}
	}
}
