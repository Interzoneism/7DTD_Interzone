using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001539 RID: 5433
	public class TwitchCommandAddSpecialPoints : BaseTwitchCommand
	{
		// Token: 0x1700125D RID: 4701
		// (get) Token: 0x0600A75E RID: 42846 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x1700125E RID: 4702
		// (get) Token: 0x0600A75F RID: 42847 RVA: 0x00420C70 File Offset: 0x0041EE70
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#addsp"
				};
			}
		}

		// Token: 0x1700125F RID: 4703
		// (get) Token: 0x0600A760 RID: 42848 RVA: 0x00420C80 File Offset: 0x0041EE80
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_AddSpecialPoints", false)
				};
			}
		}

		// Token: 0x0600A761 RID: 42849 RVA: 0x00420C98 File Offset: 0x0041EE98
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 3)
			{
				int points = 0;
				if (int.TryParse(array[2], out points))
				{
					if (array[1].EqualsCaseInsensitive(BaseTwitchCommand.allText))
					{
						TwitchManager.Current.ViewerData.AddPoints("", points, true, true);
						return;
					}
					TwitchManager.Current.ViewerData.AddPoints(array[1], points, true, true);
				}
			}
		}

		// Token: 0x0600A762 RID: 42850 RVA: 0x00420D04 File Offset: 0x0041EF04
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 3)
			{
				int points = 0;
				if (int.TryParse(arguments[2], out points))
				{
					if (arguments[1].EqualsCaseInsensitive(BaseTwitchCommand.allText))
					{
						TwitchManager.Current.ViewerData.AddPoints("", points, true, true);
						return;
					}
					TwitchManager.Current.ViewerData.AddPoints(arguments[1], points, true, true);
				}
			}
		}
	}
}
