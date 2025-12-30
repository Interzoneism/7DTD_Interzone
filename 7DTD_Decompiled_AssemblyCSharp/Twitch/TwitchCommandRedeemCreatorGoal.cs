using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001544 RID: 5444
	public class TwitchCommandRedeemCreatorGoal : BaseTwitchCommand
	{
		// Token: 0x1700127B RID: 4731
		// (get) Token: 0x0600A79D RID: 42909 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x1700127C RID: 4732
		// (get) Token: 0x0600A79E RID: 42910 RVA: 0x004217F5 File Offset: 0x0041F9F5
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#redeem_goal"
				};
			}
		}

		// Token: 0x1700127D RID: 4733
		// (get) Token: 0x0600A79F RID: 42911 RVA: 0x00421805 File Offset: 0x0041FA05
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_RedeemGoal", false)
				};
			}
		}

		// Token: 0x0600A7A0 RID: 42912 RVA: 0x0042181C File Offset: 0x0041FA1C
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				TwitchManager.Current.HandleCreatorGoalRedeem(array[1]);
			}
		}

		// Token: 0x0600A7A1 RID: 42913 RVA: 0x0042184B File Offset: 0x0041FA4B
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				TwitchManager.Current.HandleCreatorGoalRedeem(arguments[1]);
			}
		}
	}
}
