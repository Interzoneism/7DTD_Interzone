using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200153C RID: 5436
	public class TwitchCommandCommands : BaseTwitchCommand
	{
		// Token: 0x17001264 RID: 4708
		// (get) Token: 0x0600A76E RID: 42862 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001265 RID: 4709
		// (get) Token: 0x0600A76F RID: 42863 RVA: 0x00421061 File Offset: 0x0041F261
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#commands"
				};
			}
		}

		// Token: 0x17001266 RID: 4710
		// (get) Token: 0x0600A770 RID: 42864 RVA: 0x00421071 File Offset: 0x0041F271
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_Commands", false)
				};
			}
		}

		// Token: 0x0600A771 RID: 42865 RVA: 0x00421087 File Offset: 0x0041F287
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			TwitchManager.Current.DisplayCommands(message.isBroadcaster, message.isMod, message.isVIP, message.isSub);
		}

		// Token: 0x0600A772 RID: 42866 RVA: 0x004210AB File Offset: 0x0041F2AB
		public override void ExecuteConsole(List<string> arguments)
		{
			TwitchManager.Current.DisplayCommands(true, true, true, true);
		}
	}
}
