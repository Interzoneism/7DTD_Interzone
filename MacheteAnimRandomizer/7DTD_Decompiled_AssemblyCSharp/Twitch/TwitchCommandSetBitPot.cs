using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200154B RID: 5451
	public class TwitchCommandSetBitPot : BaseTwitchCommand
	{
		// Token: 0x17001290 RID: 4752
		// (get) Token: 0x0600A7C7 RID: 42951 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001291 RID: 4753
		// (get) Token: 0x0600A7C8 RID: 42952 RVA: 0x00422058 File Offset: 0x00420258
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#setbitpot"
				};
			}
		}

		// Token: 0x17001292 RID: 4754
		// (get) Token: 0x0600A7C9 RID: 42953 RVA: 0x00422068 File Offset: 0x00420268
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_SetBitPot", false)
				};
			}
		}

		// Token: 0x0600A7CA RID: 42954 RVA: 0x00422080 File Offset: 0x00420280
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				int bitPot = 0;
				if (int.TryParse(array[1], out bitPot))
				{
					TwitchManager.Current.SetBitPot(bitPot);
				}
			}
		}

		// Token: 0x0600A7CB RID: 42955 RVA: 0x004220BC File Offset: 0x004202BC
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				int bitPot = 0;
				if (int.TryParse(arguments[1], out bitPot))
				{
					TwitchManager.Current.SetBitPot(bitPot);
				}
			}
		}
	}
}
