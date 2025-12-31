using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200154A RID: 5450
	public class TwitchCommandResetCooldowns : BaseTwitchCommand
	{
		// Token: 0x1700128D RID: 4749
		// (get) Token: 0x0600A7C1 RID: 42945 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x1700128E RID: 4750
		// (get) Token: 0x0600A7C2 RID: 42946 RVA: 0x00421F23 File Offset: 0x00420123
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#reset_cooldowns"
				};
			}
		}

		// Token: 0x1700128F RID: 4751
		// (get) Token: 0x0600A7C3 RID: 42947 RVA: 0x00421F33 File Offset: 0x00420133
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_ResetCooldowns", false)
				};
			}
		}

		// Token: 0x0600A7C4 RID: 42948 RVA: 0x00421F4C File Offset: 0x0042014C
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			TwitchManager twitchManager = TwitchManager.Current;
			float currentUnityTime = twitchManager.CurrentUnityTime;
			foreach (string key in TwitchActionManager.TwitchActions.Keys)
			{
				TwitchActionManager.TwitchActions[key].ResetCooldown(currentUnityTime);
			}
			twitchManager.SetupAvailableCommands();
			twitchManager.SendChannelMessage("Action Cooldowns have been reset!", true);
		}

		// Token: 0x0600A7C5 RID: 42949 RVA: 0x00421FCC File Offset: 0x004201CC
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 1)
			{
				TwitchManager twitchManager = TwitchManager.Current;
				float currentUnityTime = twitchManager.CurrentUnityTime;
				foreach (string key in TwitchActionManager.TwitchActions.Keys)
				{
					TwitchActionManager.TwitchActions[key].ResetCooldown(currentUnityTime);
				}
				twitchManager.SetupAvailableCommands();
				twitchManager.SendChannelMessage("Action Cooldowns have been reset!", true);
			}
		}
	}
}
