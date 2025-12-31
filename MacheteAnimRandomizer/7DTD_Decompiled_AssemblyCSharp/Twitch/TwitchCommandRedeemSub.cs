using System;
using System.Collections.Generic;
using System.Globalization;

namespace Twitch
{
	// Token: 0x02001548 RID: 5448
	public class TwitchCommandRedeemSub : BaseTwitchCommand
	{
		// Token: 0x17001287 RID: 4743
		// (get) Token: 0x0600A7B5 RID: 42933 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001288 RID: 4744
		// (get) Token: 0x0600A7B6 RID: 42934 RVA: 0x00421BE7 File Offset: 0x0041FDE7
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#redeem_sub"
				};
			}
		}

		// Token: 0x17001289 RID: 4745
		// (get) Token: 0x0600A7B7 RID: 42935 RVA: 0x00421BF7 File Offset: 0x0041FDF7
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_RedeemSub", false)
				};
			}
		}

		// Token: 0x0600A7B8 RID: 42936 RVA: 0x00421C10 File Offset: 0x0041FE10
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
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
				TwitchManager.Current.HandleSubEvent(text, 1, TwitchSubEventEntry.SubTierTypes.Tier1);
				return;
			}
			if (array.Length == 3)
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
				int months = 0;
				if (StringParsers.TryParseSInt32(array[2], out months, 0, -1, NumberStyles.Integer))
				{
					TwitchManager.Current.HandleSubEvent(text2, months, TwitchSubEventEntry.SubTierTypes.Tier1);
					return;
				}
			}
			else if (array.Length == 4)
			{
				string text3 = array[1];
				if (text3.StartsWith("@"))
				{
					text3 = text3.Substring(1).ToLower();
				}
				else
				{
					text3 = text3.ToLower();
				}
				int months2 = 0;
				int num = 0;
				TwitchSubEventEntry.SubTierTypes tier = TwitchSubEventEntry.SubTierTypes.Tier1;
				StringParsers.TryParseSInt32(array[2], out months2, 0, -1, NumberStyles.Integer);
				if (array[3].Trim().ToLower() == "prime")
				{
					tier = TwitchSubEventEntry.SubTierTypes.Prime;
				}
				else
				{
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
				}
				TwitchManager.Current.HandleSubEvent(text3, months2, tier);
			}
		}

		// Token: 0x0600A7B9 RID: 42937 RVA: 0x00421D54 File Offset: 0x0041FF54
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
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
				TwitchManager.Current.HandleSubEvent(text, 1, TwitchSubEventEntry.SubTierTypes.Tier1);
				return;
			}
			if (arguments.Count == 3)
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
				int months = 0;
				if (StringParsers.TryParseSInt32(arguments[2], out months, 0, -1, NumberStyles.Integer))
				{
					TwitchManager.Current.HandleSubEvent(text2, months, TwitchSubEventEntry.SubTierTypes.Tier1);
					return;
				}
			}
			else if (arguments.Count == 4)
			{
				string text3 = arguments[1];
				if (text3.StartsWith("@"))
				{
					text3 = text3.Substring(1).ToLower();
				}
				else
				{
					text3 = text3.ToLower();
				}
				int months2 = 0;
				int num = 0;
				TwitchSubEventEntry.SubTierTypes tier = TwitchSubEventEntry.SubTierTypes.Tier1;
				StringParsers.TryParseSInt32(arguments[2], out months2, 0, -1, NumberStyles.Integer);
				if (arguments[3].Trim().ToLower() == "prime")
				{
					tier = TwitchSubEventEntry.SubTierTypes.Prime;
				}
				else
				{
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
				}
				TwitchManager.Current.HandleSubEvent(text3, months2, tier);
			}
		}
	}
}
