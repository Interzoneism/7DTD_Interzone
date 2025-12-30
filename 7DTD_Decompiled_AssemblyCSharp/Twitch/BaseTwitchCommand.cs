using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001535 RID: 5429
	public class BaseTwitchCommand
	{
		// Token: 0x17001254 RID: 4692
		// (get) Token: 0x0600A746 RID: 42822 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public virtual BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Everyone;
			}
		}

		// Token: 0x17001255 RID: 4693
		// (get) Token: 0x0600A747 RID: 42823 RVA: 0x00019766 File Offset: 0x00017966
		public virtual string[] CommandText
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001256 RID: 4694
		// (get) Token: 0x0600A748 RID: 42824 RVA: 0x00019766 File Offset: 0x00017966
		public virtual string[] LocalizedCommandNames
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600A74A RID: 42826 RVA: 0x0042098E File Offset: 0x0041EB8E
		public static void ClearCommandPermissionOverrides()
		{
			BaseTwitchCommand.CommandPermissionOverrides.Clear();
		}

		// Token: 0x0600A74B RID: 42827 RVA: 0x0042099A File Offset: 0x0041EB9A
		public static void AddCommandPermissionOverride(string commandName, BaseTwitchCommand.PermissionLevels permissionLevel)
		{
			if (!BaseTwitchCommand.CommandPermissionOverrides.ContainsKey(commandName))
			{
				BaseTwitchCommand.CommandPermissionOverrides.Add(commandName, permissionLevel);
			}
		}

		// Token: 0x0600A74C RID: 42828 RVA: 0x004209B5 File Offset: 0x0041EBB5
		public static BaseTwitchCommand.PermissionLevels GetPermission(BaseTwitchCommand cmd)
		{
			if (BaseTwitchCommand.CommandPermissionOverrides.ContainsKey(cmd.CommandText[0]))
			{
				return BaseTwitchCommand.CommandPermissionOverrides[cmd.CommandText[0]];
			}
			return cmd.RequiredPermission;
		}

		// Token: 0x0600A74D RID: 42829 RVA: 0x004209E4 File Offset: 0x0041EBE4
		public BaseTwitchCommand()
		{
			this.SetupCommandTextList();
			if (BaseTwitchCommand.allText == "")
			{
				BaseTwitchCommand.allText = Localization.Get("lblAll", false);
			}
		}

		// Token: 0x0600A74E RID: 42830 RVA: 0x00420A20 File Offset: 0x0041EC20
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetupCommandTextList()
		{
			this.CommandTextList.AddRange(this.CommandText);
			string[] localizedCommandNames = this.LocalizedCommandNames;
			for (int i = 0; i < localizedCommandNames.Length; i++)
			{
				if (!this.CommandTextList.Contains(localizedCommandNames[i]))
				{
					this.CommandTextList.Add(localizedCommandNames[i]);
				}
			}
		}

		// Token: 0x0600A74F RID: 42831 RVA: 0x00420A74 File Offset: 0x0041EC74
		public virtual bool CheckAllowed(TwitchIRCClient.TwitchChatMessage message)
		{
			BaseTwitchCommand.PermissionLevels permission = BaseTwitchCommand.GetPermission(this);
			if (permission == BaseTwitchCommand.PermissionLevels.Everyone)
			{
				return true;
			}
			if (permission == BaseTwitchCommand.PermissionLevels.Mod)
			{
				return message.isMod;
			}
			if (permission == BaseTwitchCommand.PermissionLevels.Broadcaster)
			{
				return message.isBroadcaster;
			}
			if (permission == BaseTwitchCommand.PermissionLevels.VIP)
			{
				return message.isVIP;
			}
			return permission == BaseTwitchCommand.PermissionLevels.Sub && message.isSub;
		}

		// Token: 0x0600A750 RID: 42832 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
		}

		// Token: 0x0600A751 RID: 42833 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void ExecuteConsole(List<string> arguments)
		{
		}

		// Token: 0x04008209 RID: 33289
		public List<string> CommandTextList = new List<string>();

		// Token: 0x0400820A RID: 33290
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string allText = "";

		// Token: 0x0400820B RID: 33291
		[PublicizedFrom(EAccessModifier.Protected)]
		public static Dictionary<string, BaseTwitchCommand.PermissionLevels> CommandPermissionOverrides = new Dictionary<string, BaseTwitchCommand.PermissionLevels>();

		// Token: 0x02001536 RID: 5430
		public enum PermissionLevels
		{
			// Token: 0x0400820D RID: 33293
			Everyone,
			// Token: 0x0400820E RID: 33294
			VIP,
			// Token: 0x0400820F RID: 33295
			Sub,
			// Token: 0x04008210 RID: 33296
			Mod,
			// Token: 0x04008211 RID: 33297
			Broadcaster
		}
	}
}
