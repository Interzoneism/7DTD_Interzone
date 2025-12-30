using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001537 RID: 5431
	public class TwitchCommandAddBitCredit : BaseTwitchCommand
	{
		// Token: 0x17001257 RID: 4695
		// (get) Token: 0x0600A752 RID: 42834 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001258 RID: 4696
		// (get) Token: 0x0600A753 RID: 42835 RVA: 0x00420ABA File Offset: 0x0041ECBA
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#addcredit"
				};
			}
		}

		// Token: 0x17001259 RID: 4697
		// (get) Token: 0x0600A754 RID: 42836 RVA: 0x00420ACA File Offset: 0x0041ECCA
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_AddBitCredit", false)
				};
			}
		}

		// Token: 0x0600A755 RID: 42837 RVA: 0x00420AE0 File Offset: 0x0041ECE0
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 3)
			{
				int credit = 0;
				if (int.TryParse(array[2], out credit))
				{
					TwitchManager.Current.ViewerData.AddCredit(array[1], credit, true);
				}
			}
		}

		// Token: 0x0600A756 RID: 42838 RVA: 0x00420B28 File Offset: 0x0041ED28
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 3)
			{
				int credit = 0;
				if (int.TryParse(arguments[2], out credit))
				{
					TwitchManager.Current.ViewerData.AddCredit(arguments[1], credit, true);
				}
			}
		}
	}
}
