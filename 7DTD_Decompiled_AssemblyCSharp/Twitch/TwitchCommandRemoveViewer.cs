using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001549 RID: 5449
	public class TwitchCommandRemoveViewer : BaseTwitchCommand
	{
		// Token: 0x1700128A RID: 4746
		// (get) Token: 0x0600A7BB RID: 42939 RVA: 0x00075C39 File Offset: 0x00073E39
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x1700128B RID: 4747
		// (get) Token: 0x0600A7BC RID: 42940 RVA: 0x00421EA6 File Offset: 0x004200A6
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#remove_viewer"
				};
			}
		}

		// Token: 0x1700128C RID: 4748
		// (get) Token: 0x0600A7BD RID: 42941 RVA: 0x00421EB6 File Offset: 0x004200B6
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_RemoveViewer", false)
				};
			}
		}

		// Token: 0x0600A7BE RID: 42942 RVA: 0x00421ECC File Offset: 0x004200CC
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				TwitchManager.Current.ViewerData.RemoveViewerEntry(array[1]);
			}
		}

		// Token: 0x0600A7BF RID: 42943 RVA: 0x00421F01 File Offset: 0x00420101
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				TwitchManager.Current.ViewerData.RemoveViewerEntry(arguments[1]);
			}
		}
	}
}
