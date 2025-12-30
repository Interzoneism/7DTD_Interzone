using System;
using System.Collections.Generic;
using System.Globalization;

namespace Twitch
{
	// Token: 0x02001542 RID: 5442
	public class TwitchCommandRedeemBits : BaseTwitchCommand
	{
		// Token: 0x17001275 RID: 4725
		// (get) Token: 0x0600A791 RID: 42897 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001276 RID: 4726
		// (get) Token: 0x0600A792 RID: 42898 RVA: 0x00421544 File Offset: 0x0041F744
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#redeem_bits"
				};
			}
		}

		// Token: 0x17001277 RID: 4727
		// (get) Token: 0x0600A793 RID: 42899 RVA: 0x00421554 File Offset: 0x0041F754
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_RedeemBits", false)
				};
			}
		}

		// Token: 0x0600A794 RID: 42900 RVA: 0x0042156C File Offset: 0x0041F76C
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				int bitAmount = 0;
				if (StringParsers.TryParseSInt32(array[1], out bitAmount, 0, -1, NumberStyles.Integer))
				{
					TwitchManager.Current.HandleBitRedeem(message.UserName, bitAmount, null);
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
				int bitAmount2 = 0;
				if (StringParsers.TryParseSInt32(array[2], out bitAmount2, 0, -1, NumberStyles.Integer))
				{
					TwitchManager.Current.HandleBitRedeem(text, bitAmount2, null);
				}
			}
		}

		// Token: 0x0600A795 RID: 42901 RVA: 0x00421600 File Offset: 0x0041F800
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				int bitAmount = 0;
				if (StringParsers.TryParseSInt32(arguments[1], out bitAmount, 0, -1, NumberStyles.Integer))
				{
					TwitchManager.Current.HandleBitRedeem(TwitchManager.Current.Authentication.userName, bitAmount, null);
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
				int bitAmount2 = 0;
				if (StringParsers.TryParseSInt32(arguments[2], out bitAmount2, 0, -1, NumberStyles.Integer))
				{
					TwitchManager.Current.HandleBitRedeem(text, bitAmount2, null);
				}
			}
		}
	}
}
