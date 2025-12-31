using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001540 RID: 5440
	public class TwitchCommandGamestage : BaseTwitchCommand
	{
		// Token: 0x17001270 RID: 4720
		// (get) Token: 0x0600A786 RID: 42886 RVA: 0x004214A0 File Offset: 0x0041F6A0
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#gamestage",
					"#gs"
				};
			}
		}

		// Token: 0x17001271 RID: 4721
		// (get) Token: 0x0600A787 RID: 42887 RVA: 0x004214B8 File Offset: 0x0041F6B8
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_Gamestage1", false),
					Localization.Get("TwitchCommand_Gamestage1", false)
				};
			}
		}

		// Token: 0x0600A788 RID: 42888 RVA: 0x004214DC File Offset: 0x0041F6DC
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			TwitchManager.Current.DisplayGameStage();
		}

		// Token: 0x0600A789 RID: 42889 RVA: 0x004214DC File Offset: 0x0041F6DC
		public override void ExecuteConsole(List<string> arguments)
		{
			TwitchManager.Current.DisplayGameStage();
		}
	}
}
