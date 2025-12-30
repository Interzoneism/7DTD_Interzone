using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200153E RID: 5438
	public class TwitchCommandDisableCommand : BaseTwitchCommand
	{
		// Token: 0x1700126A RID: 4714
		// (get) Token: 0x0600A77A RID: 42874 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x1700126B RID: 4715
		// (get) Token: 0x0600A77B RID: 42875 RVA: 0x00421140 File Offset: 0x0041F340
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#disable"
				};
			}
		}

		// Token: 0x1700126C RID: 4716
		// (get) Token: 0x0600A77C RID: 42876 RVA: 0x00421150 File Offset: 0x0041F350
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_Disable", false)
				};
			}
		}

		// Token: 0x0600A77D RID: 42877 RVA: 0x00421168 File Offset: 0x0041F368
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
						twitchAction.Enabled = false;
						flag = true;
					}
				}
				if (flag)
				{
					twitchManager.SendChannelMessage("[7DTD]: Command Disabled: " + array[1], true);
					twitchManager.SetupAvailableCommands();
				}
			}
		}

		// Token: 0x0600A77E RID: 42878 RVA: 0x00421230 File Offset: 0x0041F430
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
						twitchAction.Enabled = false;
						flag = true;
					}
				}
				if (flag)
				{
					twitchManager.SendChannelMessage("[7DTD]: Command Disabled: " + arguments[1], true);
					twitchManager.SetupAvailableCommands();
				}
			}
		}
	}
}
