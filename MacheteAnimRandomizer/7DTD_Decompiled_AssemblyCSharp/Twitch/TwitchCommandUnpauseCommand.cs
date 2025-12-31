using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200154F RID: 5455
	public class TwitchCommandUnpauseCommand : BaseTwitchCommand
	{
		// Token: 0x1700129C RID: 4764
		// (get) Token: 0x0600A7DF RID: 42975 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x1700129D RID: 4765
		// (get) Token: 0x0600A7E0 RID: 42976 RVA: 0x004223B2 File Offset: 0x004205B2
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#unpause"
				};
			}
		}

		// Token: 0x1700129E RID: 4766
		// (get) Token: 0x0600A7E1 RID: 42977 RVA: 0x004223C2 File Offset: 0x004205C2
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_Unpause", false)
				};
			}
		}

		// Token: 0x0600A7E2 RID: 42978 RVA: 0x004223D8 File Offset: 0x004205D8
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			if (message.Message.Split(' ', StringSplitOptions.None).Length == 1)
			{
				TwitchManager.Current.SetTwitchActive(true);
			}
		}

		// Token: 0x0600A7E3 RID: 42979 RVA: 0x004223F8 File Offset: 0x004205F8
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 1)
			{
				TwitchManager.Current.SetTwitchActive(true);
			}
		}
	}
}
