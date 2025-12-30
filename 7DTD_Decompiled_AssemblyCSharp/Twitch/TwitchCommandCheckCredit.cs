using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200153A RID: 5434
	public class TwitchCommandCheckCredit : BaseTwitchCommand
	{
		// Token: 0x17001260 RID: 4704
		// (get) Token: 0x0600A764 RID: 42852 RVA: 0x00420D70 File Offset: 0x0041EF70
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#checkcredit"
				};
			}
		}

		// Token: 0x17001261 RID: 4705
		// (get) Token: 0x0600A765 RID: 42853 RVA: 0x00420D80 File Offset: 0x0041EF80
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_CheckCredit", false)
				};
			}
		}

		// Token: 0x0600A766 RID: 42854 RVA: 0x00420D98 File Offset: 0x0041EF98
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				if (message.isMod || message.isBroadcaster)
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
					TwitchManager twitchManager = TwitchManager.Current;
					if (twitchManager.ViewerData.HasViewerEntry(text))
					{
						twitchManager.SendChannelCreditOutputMessage(text);
						return;
					}
					twitchManager.ircClient.SendChannelMessage(string.Format("[7DTD]: No viewer data for {0}.", array[1]), true);
					return;
				}
			}
			else if (array.Length == 1)
			{
				TwitchManager.Current.SendChannelCreditOutputMessage(message.UserName);
			}
		}

		// Token: 0x0600A767 RID: 42855 RVA: 0x00420E40 File Offset: 0x0041F040
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count != 2)
			{
				if (arguments.Count == 1)
				{
					TwitchManager.Current.SendChannelPointOutputMessage(TwitchManager.Current.Authentication.userName);
				}
				return;
			}
			string text = arguments[1];
			if (text.StartsWith("@"))
			{
				text = text.Substring(1).ToLower();
			}
			else
			{
				text = text.ToLower();
			}
			TwitchManager twitchManager = TwitchManager.Current;
			if (twitchManager.ViewerData.HasViewerEntry(text))
			{
				twitchManager.SendChannelPointOutputMessage(text);
				return;
			}
			twitchManager.ircClient.SendChannelMessage(string.Format("[7DTD]: No viewer data for {0}.", arguments[1]), true);
		}
	}
}
