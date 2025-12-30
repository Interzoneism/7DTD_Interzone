using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001550 RID: 5456
	public class TwitchCommandUseProgression : BaseTwitchCommand
	{
		// Token: 0x1700129F RID: 4767
		// (get) Token: 0x0600A7E5 RID: 42981 RVA: 0x00075CC0 File Offset: 0x00073EC0
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Broadcaster;
			}
		}

		// Token: 0x170012A0 RID: 4768
		// (get) Token: 0x0600A7E6 RID: 42982 RVA: 0x0042240E File Offset: 0x0042060E
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#useprogression"
				};
			}
		}

		// Token: 0x170012A1 RID: 4769
		// (get) Token: 0x0600A7E7 RID: 42983 RVA: 0x0042241E File Offset: 0x0042061E
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_UseProgression", false)
				};
			}
		}

		// Token: 0x0600A7E8 RID: 42984 RVA: 0x00422434 File Offset: 0x00420634
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				bool useProgression = false;
				if (bool.TryParse(array[1], out useProgression))
				{
					TwitchManager.Current.SetUseProgression(useProgression);
				}
				TwitchManager.Current.SendChannelMessage("[7DTD]: Use Progression Enabled: " + (TwitchManager.Current.UseProgression ? "Yes" : "No"), true);
			}
		}

		// Token: 0x0600A7E9 RID: 42985 RVA: 0x0042249C File Offset: 0x0042069C
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				bool useProgression = false;
				if (bool.TryParse(arguments[1], out useProgression))
				{
					TwitchManager.Current.SetUseProgression(useProgression);
				}
				TwitchManager.Current.SendChannelMessage("[7DTD]: Use Progression Enabled: " + (TwitchManager.Current.UseProgression ? "Yes" : "No"), true);
			}
		}
	}
}
