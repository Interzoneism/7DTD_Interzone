using System;
using System.Collections.Generic;
using System.Globalization;

namespace Twitch
{
	// Token: 0x02001545 RID: 5445
	public class TwitchCommandRedeemGiftSub : BaseTwitchCommand
	{
		// Token: 0x1700127E RID: 4734
		// (get) Token: 0x0600A7A3 RID: 42915 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x1700127F RID: 4735
		// (get) Token: 0x0600A7A4 RID: 42916 RVA: 0x00421867 File Offset: 0x0041FA67
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#redeem_giftsub"
				};
			}
		}

		// Token: 0x17001280 RID: 4736
		// (get) Token: 0x0600A7A5 RID: 42917 RVA: 0x00421877 File Offset: 0x0041FA77
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_RedeemGiftSubs", false)
				};
			}
		}

		// Token: 0x0600A7A6 RID: 42918 RVA: 0x00421890 File Offset: 0x0041FA90
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
				int giftCounts = 0;
				if (StringParsers.TryParseSInt32(array[2], out giftCounts, 0, -1, NumberStyles.Integer))
				{
					TwitchManager.Current.HandleGiftSubEvent(text, giftCounts, TwitchSubEventEntry.SubTierTypes.Tier1);
					return;
				}
			}
			else if (array.Length == 4)
			{
				string text2 = array[1];
				if (text2.StartsWith("@"))
				{
					text2 = text2.Substring(1).ToLower();
				}
				else
				{
					text2 = text2.ToLower();
				}
				int giftCounts2 = 0;
				int num = 0;
				TwitchSubEventEntry.SubTierTypes tier = TwitchSubEventEntry.SubTierTypes.Tier1;
				StringParsers.TryParseSInt32(array[2], out giftCounts2, 0, -1, NumberStyles.Integer);
				StringParsers.TryParseSInt32(array[3], out num, 0, -1, NumberStyles.Integer);
				if (num != 2)
				{
					if (num == 3)
					{
						tier = TwitchSubEventEntry.SubTierTypes.Tier3;
					}
				}
				else
				{
					tier = TwitchSubEventEntry.SubTierTypes.Tier2;
				}
				TwitchManager.Current.HandleGiftSubEvent(text2, giftCounts2, tier);
			}
		}

		// Token: 0x0600A7A7 RID: 42919 RVA: 0x00421970 File Offset: 0x0041FB70
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
				int giftCounts = 0;
				if (StringParsers.TryParseSInt32(arguments[2], out giftCounts, 0, -1, NumberStyles.Integer))
				{
					TwitchManager.Current.HandleGiftSubEvent(text, giftCounts, TwitchSubEventEntry.SubTierTypes.Tier1);
					return;
				}
			}
			else if (arguments.Count == 4)
			{
				string text2 = arguments[1];
				if (text2.StartsWith("@"))
				{
					text2 = text2.Substring(1).ToLower();
				}
				else
				{
					text2 = text2.ToLower();
				}
				int giftCounts2 = 0;
				int num = 0;
				TwitchSubEventEntry.SubTierTypes tier = TwitchSubEventEntry.SubTierTypes.Tier1;
				StringParsers.TryParseSInt32(arguments[2], out giftCounts2, 0, -1, NumberStyles.Integer);
				StringParsers.TryParseSInt32(arguments[3], out num, 0, -1, NumberStyles.Integer);
				if (num != 2)
				{
					if (num == 3)
					{
						tier = TwitchSubEventEntry.SubTierTypes.Tier3;
					}
				}
				else
				{
					tier = TwitchSubEventEntry.SubTierTypes.Tier2;
				}
				TwitchManager.Current.HandleGiftSubEvent(text2, giftCounts2, tier);
			}
		}
	}
}
