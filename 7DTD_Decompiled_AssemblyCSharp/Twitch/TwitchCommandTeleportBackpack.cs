using System;
using System.Collections.Generic;
using System.Globalization;

namespace Twitch
{
	// Token: 0x0200154E RID: 5454
	public class TwitchCommandTeleportBackpack : BaseTwitchCommand
	{
		// Token: 0x17001299 RID: 4761
		// (get) Token: 0x0600A7D9 RID: 42969 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x1700129A RID: 4762
		// (get) Token: 0x0600A7DA RID: 42970 RVA: 0x0042223F File Offset: 0x0042043F
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#tp_backpack",
					"#teleport_backpack"
				};
			}
		}

		// Token: 0x1700129B RID: 4763
		// (get) Token: 0x0600A7DB RID: 42971 RVA: 0x00422257 File Offset: 0x00420457
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_TeleportBackpack1", false),
					Localization.Get("TwitchCommand_TeleportBackpack2", false)
				};
			}
		}

		// Token: 0x0600A7DC RID: 42972 RVA: 0x0042227C File Offset: 0x0042047C
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			EntityPlayer entityPlayer = TwitchManager.Current.LocalPlayer;
			if (array.Length != 2)
			{
				GameEventManager.Current.HandleAction("action_teleport_backpack", entityPlayer, entityPlayer, false, "", "", false, true, "", null);
				return;
			}
			int index = -1;
			if (StringParsers.TryParseSInt32(array[1], out index, 0, -1, NumberStyles.Integer) && TwitchManager.Current.LocalPlayer.Party != null)
			{
				entityPlayer = TwitchManager.Current.LocalPlayer.Party.GetMemberAtIndex(index, TwitchManager.Current.LocalPlayer);
				entityPlayer == null;
				return;
			}
		}

		// Token: 0x0600A7DD RID: 42973 RVA: 0x0042231C File Offset: 0x0042051C
		public override void ExecuteConsole(List<string> arguments)
		{
			EntityPlayer entityPlayer = TwitchManager.Current.LocalPlayer;
			if (arguments.Count != 2)
			{
				GameEventManager.Current.HandleAction("action_teleport_backpack", entityPlayer, entityPlayer, false, "", "", false, true, "", null);
				return;
			}
			int index = -1;
			if (StringParsers.TryParseSInt32(arguments[1], out index, 0, -1, NumberStyles.Integer) && TwitchManager.Current.LocalPlayer.Party != null)
			{
				entityPlayer = TwitchManager.Current.LocalPlayer.Party.GetMemberAtIndex(index, TwitchManager.Current.LocalPlayer);
				entityPlayer == null;
				return;
			}
		}
	}
}
