using System;
using System.Collections.Generic;
using System.Globalization;

namespace Twitch
{
	// Token: 0x02001543 RID: 5443
	public class TwitchCommandRedeemCharity : BaseTwitchCommand
	{
		// Token: 0x17001278 RID: 4728
		// (get) Token: 0x0600A797 RID: 42903 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001279 RID: 4729
		// (get) Token: 0x0600A798 RID: 42904 RVA: 0x0042169D File Offset: 0x0041F89D
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#redeem_charity"
				};
			}
		}

		// Token: 0x1700127A RID: 4730
		// (get) Token: 0x0600A799 RID: 42905 RVA: 0x004216AD File Offset: 0x0041F8AD
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_RedeemCharity", false)
				};
			}
		}

		// Token: 0x0600A79A RID: 42906 RVA: 0x004216C4 File Offset: 0x0041F8C4
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				int charityAmount = 0;
				if (StringParsers.TryParseSInt32(array[1], out charityAmount, 0, -1, NumberStyles.Integer))
				{
					TwitchManager.Current.HandleCharityRedeem(message.UserName, charityAmount, null);
					return;
				}
			}
			else if (array.Length == 3)
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
				int charityAmount2 = 0;
				if (StringParsers.TryParseSInt32(array[2], out charityAmount2, 0, -1, NumberStyles.Integer))
				{
					TwitchManager.Current.HandleCharityRedeem(text, charityAmount2, null);
				}
			}
		}

		// Token: 0x0600A79B RID: 42907 RVA: 0x00421758 File Offset: 0x0041F958
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				int charityAmount = 0;
				if (StringParsers.TryParseSInt32(arguments[1], out charityAmount, 0, -1, NumberStyles.Integer))
				{
					TwitchManager.Current.HandleCharityRedeem(TwitchManager.Current.Authentication.userName, charityAmount, null);
					return;
				}
			}
			else if (arguments.Count == 3)
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
				int charityAmount2 = 0;
				if (StringParsers.TryParseSInt32(arguments[2], out charityAmount2, 0, -1, NumberStyles.Integer))
				{
					TwitchManager.Current.HandleCharityRedeem(text, charityAmount2, null);
				}
			}
		}
	}
}
