using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200154D RID: 5453
	public class TwitchCommandSetPot : BaseTwitchCommand
	{
		// Token: 0x17001296 RID: 4758
		// (get) Token: 0x0600A7D3 RID: 42963 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001297 RID: 4759
		// (get) Token: 0x0600A7D4 RID: 42964 RVA: 0x004221A7 File Offset: 0x004203A7
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#setpot"
				};
			}
		}

		// Token: 0x17001298 RID: 4760
		// (get) Token: 0x0600A7D5 RID: 42965 RVA: 0x004221B7 File Offset: 0x004203B7
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_SetPot", false)
				};
			}
		}

		// Token: 0x0600A7D6 RID: 42966 RVA: 0x004221D0 File Offset: 0x004203D0
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				int pot = 0;
				if (int.TryParse(array[1], out pot))
				{
					TwitchManager.Current.SetPot(pot);
				}
			}
		}

		// Token: 0x0600A7D7 RID: 42967 RVA: 0x0042220C File Offset: 0x0042040C
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				int pot = 0;
				if (int.TryParse(arguments[1], out pot))
				{
					TwitchManager.Current.SetPot(pot);
				}
			}
		}
	}
}
