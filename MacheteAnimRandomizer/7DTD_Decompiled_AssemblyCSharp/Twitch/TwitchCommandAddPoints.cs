using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001538 RID: 5432
	public class TwitchCommandAddPoints : BaseTwitchCommand
	{
		// Token: 0x1700125A RID: 4698
		// (get) Token: 0x0600A758 RID: 42840 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x1700125B RID: 4699
		// (get) Token: 0x0600A759 RID: 42841 RVA: 0x00420B71 File Offset: 0x0041ED71
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#addpp"
				};
			}
		}

		// Token: 0x1700125C RID: 4700
		// (get) Token: 0x0600A75A RID: 42842 RVA: 0x00420B81 File Offset: 0x0041ED81
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_AddPoints", false)
				};
			}
		}

		// Token: 0x0600A75B RID: 42843 RVA: 0x00420B98 File Offset: 0x0041ED98
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
						TwitchManager.Current.ViewerData.AddPoints("", points, false, true);
						return;
					}
					TwitchManager.Current.ViewerData.AddPoints(array[1], points, false, true);
				}
			}
		}

		// Token: 0x0600A75C RID: 42844 RVA: 0x00420C04 File Offset: 0x0041EE04
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 3)
			{
				int points = 0;
				if (int.TryParse(arguments[2], out points))
				{
					if (arguments[1].EqualsCaseInsensitive(BaseTwitchCommand.allText))
					{
						TwitchManager.Current.ViewerData.AddPoints("", points, false, true);
						return;
					}
					TwitchManager.Current.ViewerData.AddPoints(arguments[1], points, false, true);
				}
			}
		}
	}
}
