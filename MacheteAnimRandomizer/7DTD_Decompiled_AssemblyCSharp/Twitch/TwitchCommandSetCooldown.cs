using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200154C RID: 5452
	public class TwitchCommandSetCooldown : BaseTwitchCommand
	{
		// Token: 0x17001293 RID: 4755
		// (get) Token: 0x0600A7CD RID: 42957 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001294 RID: 4756
		// (get) Token: 0x0600A7CE RID: 42958 RVA: 0x004220EF File Offset: 0x004202EF
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#setcooldown"
				};
			}
		}

		// Token: 0x17001295 RID: 4757
		// (get) Token: 0x0600A7CF RID: 42959 RVA: 0x004220FF File Offset: 0x004202FF
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_SetCooldown", false)
				};
			}
		}

		// Token: 0x0600A7D0 RID: 42960 RVA: 0x00422118 File Offset: 0x00420318
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				int num = 0;
				if (int.TryParse(array[1], out num))
				{
					if (num < 0)
					{
						num = 0;
					}
					TwitchManager.Current.SetCooldown((float)num + 0.5f, TwitchManager.CooldownTypes.Time, false, true);
				}
			}
		}

		// Token: 0x0600A7D1 RID: 42961 RVA: 0x00422164 File Offset: 0x00420364
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				int num = 0;
				if (int.TryParse(arguments[1], out num))
				{
					if (num < 0)
					{
						num = 0;
					}
					TwitchManager.Current.SetCooldown((float)num + 0.5f, TwitchManager.CooldownTypes.Time, false, true);
				}
			}
		}
	}
}
