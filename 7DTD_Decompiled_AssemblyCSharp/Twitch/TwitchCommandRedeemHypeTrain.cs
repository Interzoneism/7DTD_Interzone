using System;
using System.Collections.Generic;
using System.Globalization;

namespace Twitch
{
	// Token: 0x02001546 RID: 5446
	public class TwitchCommandRedeemHypeTrain : BaseTwitchCommand
	{
		// Token: 0x17001281 RID: 4737
		// (get) Token: 0x0600A7A9 RID: 42921 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001282 RID: 4738
		// (get) Token: 0x0600A7AA RID: 42922 RVA: 0x00421A59 File Offset: 0x0041FC59
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#redeem_hypetrain"
				};
			}
		}

		// Token: 0x17001283 RID: 4739
		// (get) Token: 0x0600A7AB RID: 42923 RVA: 0x00421A69 File Offset: 0x0041FC69
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_RedeemHypeTrain", false)
				};
			}
		}

		// Token: 0x0600A7AC RID: 42924 RVA: 0x00421A80 File Offset: 0x0041FC80
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				int hypeTrainLevel = 0;
				if (StringParsers.TryParseSInt32(array[1], out hypeTrainLevel, 0, -1, NumberStyles.Integer))
				{
					TwitchManager.Current.HandleHypeTrainRedeem(hypeTrainLevel);
				}
			}
		}

		// Token: 0x0600A7AD RID: 42925 RVA: 0x00421AC0 File Offset: 0x0041FCC0
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				int hypeTrainLevel = 0;
				if (StringParsers.TryParseSInt32(arguments[1], out hypeTrainLevel, 0, -1, NumberStyles.Integer))
				{
					TwitchManager.Current.HandleHypeTrainRedeem(hypeTrainLevel);
				}
			}
		}
	}
}
