using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001541 RID: 5441
	public class TwitchCommandPauseCommand : BaseTwitchCommand
	{
		// Token: 0x17001272 RID: 4722
		// (get) Token: 0x0600A78B RID: 42891 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001273 RID: 4723
		// (get) Token: 0x0600A78C RID: 42892 RVA: 0x004214E8 File Offset: 0x0041F6E8
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#pause"
				};
			}
		}

		// Token: 0x17001274 RID: 4724
		// (get) Token: 0x0600A78D RID: 42893 RVA: 0x004214F8 File Offset: 0x0041F6F8
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_Pause", false)
				};
			}
		}

		// Token: 0x0600A78E RID: 42894 RVA: 0x0042150E File Offset: 0x0041F70E
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			if (message.Message.Split(' ', StringSplitOptions.None).Length == 1)
			{
				TwitchManager.Current.SetTwitchActive(false);
			}
		}

		// Token: 0x0600A78F RID: 42895 RVA: 0x0042152E File Offset: 0x0041F72E
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 1)
			{
				TwitchManager.Current.SetTwitchActive(false);
			}
		}
	}
}
