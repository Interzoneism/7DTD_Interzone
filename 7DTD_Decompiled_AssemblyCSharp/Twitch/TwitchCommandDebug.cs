using System;
using System.Collections.Generic;
using System.Text;

namespace Twitch
{
	// Token: 0x0200153D RID: 5437
	public class TwitchCommandDebug : BaseTwitchCommand
	{
		// Token: 0x17001267 RID: 4711
		// (get) Token: 0x0600A774 RID: 42868 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001268 RID: 4712
		// (get) Token: 0x0600A775 RID: 42869 RVA: 0x004210BB File Offset: 0x0041F2BB
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#debug"
				};
			}
		}

		// Token: 0x17001269 RID: 4713
		// (get) Token: 0x0600A776 RID: 42870 RVA: 0x004210CB File Offset: 0x0041F2CB
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("#debug", false)
				};
			}
		}

		// Token: 0x0600A777 RID: 42871 RVA: 0x004210E1 File Offset: 0x0041F2E1
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			TwitchManager.Current.DisplayDebug(message.Message);
		}

		// Token: 0x0600A778 RID: 42872 RVA: 0x004210F4 File Offset: 0x0041F2F4
		public override void ExecuteConsole(List<string> arguments)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < arguments.Count; i++)
			{
				stringBuilder.Append(arguments[i] + " ");
			}
			TwitchManager.Current.DisplayDebug(stringBuilder.ToString());
		}
	}
}
