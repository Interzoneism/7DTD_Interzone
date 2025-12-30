using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200153B RID: 5435
	public class TwitchCommandCheckPoints : BaseTwitchCommand
	{
		// Token: 0x17001262 RID: 4706
		// (get) Token: 0x0600A769 RID: 42857 RVA: 0x00420EDD File Offset: 0x0041F0DD
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#checkpoints",
					"#cp"
				};
			}
		}

		// Token: 0x17001263 RID: 4707
		// (get) Token: 0x0600A76A RID: 42858 RVA: 0x00420EF5 File Offset: 0x0041F0F5
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_CheckPoints1", false),
					Localization.Get("TwitchCommand_CheckPoints2", false)
				};
			}
		}

		// Token: 0x0600A76B RID: 42859 RVA: 0x00420F1C File Offset: 0x0041F11C
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
						twitchManager.SendChannelPointOutputMessage(text);
						return;
					}
					twitchManager.ircClient.SendChannelMessage(string.Format("[7DTD]: No viewer data for {0}.", array[1]), true);
					return;
				}
			}
			else if (array.Length == 1)
			{
				TwitchManager.Current.SendChannelPointOutputMessage(message.UserName);
			}
		}

		// Token: 0x0600A76C RID: 42860 RVA: 0x00420FC4 File Offset: 0x0041F1C4
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
