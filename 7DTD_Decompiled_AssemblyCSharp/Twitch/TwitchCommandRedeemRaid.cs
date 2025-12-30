using System;
using System.Collections.Generic;
using System.Globalization;

namespace Twitch
{
	// Token: 0x02001547 RID: 5447
	public class TwitchCommandRedeemRaid : BaseTwitchCommand
	{
		// Token: 0x17001284 RID: 4740
		// (get) Token: 0x0600A7AF RID: 42927 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001285 RID: 4741
		// (get) Token: 0x0600A7B0 RID: 42928 RVA: 0x00421AF6 File Offset: 0x0041FCF6
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#redeem_raid"
				};
			}
		}

		// Token: 0x17001286 RID: 4742
		// (get) Token: 0x0600A7B1 RID: 42929 RVA: 0x00421B06 File Offset: 0x0041FD06
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_RedeemRaid", false)
				};
			}
		}

		// Token: 0x0600A7B2 RID: 42930 RVA: 0x00421B1C File Offset: 0x0041FD1C
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 3)
			{
				string text = array[1];
				if (text.StartsWith("@"))
				{
					text = text.Substring(1).ToLower();
				}
				else
				{
					text = text.ToLower();
				}
				int viewerAmount = 0;
				if (StringParsers.TryParseSInt32(array[2], out viewerAmount, 0, -1, NumberStyles.Integer))
				{
					TwitchManager.Current.HandleRaidRedeem(text, viewerAmount, null);
				}
			}
		}

		// Token: 0x0600A7B3 RID: 42931 RVA: 0x00421B84 File Offset: 0x0041FD84
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 3)
			{
				string text = arguments[1];
				if (text.StartsWith("@"))
				{
					text = text.Substring(1).ToLower();
				}
				else
				{
					text = text.ToLower();
				}
				int viewerAmount = 0;
				if (StringParsers.TryParseSInt32(arguments[2], out viewerAmount, 0, -1, NumberStyles.Integer))
				{
					TwitchManager.Current.HandleRaidRedeem(text, viewerAmount, null);
				}
			}
		}
	}
}
