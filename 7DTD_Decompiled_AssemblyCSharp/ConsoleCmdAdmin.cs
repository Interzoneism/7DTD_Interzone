using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020001AE RID: 430
[Preserve]
public class ConsoleCmdAdmin : ConsoleCmdAbstract
{
	// Token: 0x06000D2A RID: 3370 RVA: 0x00058567 File Offset: 0x00056767
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"admin"
		};
	}

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x06000D2B RID: 3371 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000FF RID: 255
	// (get) Token: 0x06000D2C RID: 3372 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x0005857B File Offset: 0x0005677B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Manage user permission levels";
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x00058582 File Offset: 0x00056782
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Set/get user permission levels. A level of 0 is maximum permission,\nusers without an explicitly set permission have a permission level of 1000.\nUsage:\n   admin add <name / entity id / platform + platform user id> <level> [displayname]\n   admin remove <name / entity / platform + platform user id>\n   admin addgroup <steam id> <level regular> <level mods> [displayname]\n   admin removegroup <steam id>\n   admin list\nTo use the add/remove sub commands with a name or entity ID the player has\nto be online, the variant with platform and platform ID can be used for currently offline\nusers too.\nDisplayname can be used to put a descriptive name to the ban list in addition\nto the user's platform ID. If used on an online user the player name is used by default.\nWhen adding groups you set two permission levels: The 'regular' level applies to\nnormal members of the group, the 'mods' level applies to moderators / officers of\nthe group.";
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x0005858C File Offset: 0x0005678C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (GameManager.Instance.adminTools == null)
		{
			return;
		}
		if (_params.Count < 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No sub command given.");
			return;
		}
		if (_params[0].EqualsCaseInsensitive("add"))
		{
			this.ExecuteAdd(_params);
			return;
		}
		if (_params[0].EqualsCaseInsensitive("remove"))
		{
			this.ExecuteRemove(_params);
			return;
		}
		if (_params[0].EqualsCaseInsensitive("addgroup"))
		{
			this.ExecuteAddGroup(_params);
			return;
		}
		if (_params[0].EqualsCaseInsensitive("removegroup"))
		{
			this.ExecuteRemoveGroup(_params);
			return;
		}
		if (_params[0].EqualsCaseInsensitive("list"))
		{
			this.ExecuteList();
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid sub command \"" + _params[0] + "\".");
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x00058668 File Offset: 0x00056868
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteAdd(List<string> _params)
	{
		if (_params.Count != 3 && _params.Count != 4)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 3 or 4, found " + _params.Count.ToString() + ".");
			return;
		}
		string text = null;
		if (_params.Count > 3)
		{
			text = _params[3];
		}
		PlatformUserIdentifierAbs platformUserIdentifierAbs;
		ClientInfo clientInfo;
		if (ConsoleHelper.ParseParamPartialNameOrId(_params[1], out platformUserIdentifierAbs, out clientInfo, true) != 1)
		{
			return;
		}
		if (clientInfo != null && text == null)
		{
			text = clientInfo.playerName;
		}
		int num;
		if (!int.TryParse(_params[2], out num))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("\"" + _params[2] + "\" is not a valid integer.");
			return;
		}
		GameManager.Instance.adminTools.Users.AddUser(text, platformUserIdentifierAbs, num);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("{0} added with permission level of {1}.", platformUserIdentifierAbs, num));
	}

	// Token: 0x06000D31 RID: 3377 RVA: 0x00058748 File Offset: 0x00056948
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteRemove(List<string> _params)
	{
		if (_params.Count != 2)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 2, found " + _params.Count.ToString() + ".");
			return;
		}
		PlatformUserIdentifierAbs platformUserIdentifierAbs;
		ClientInfo clientInfo;
		if (ConsoleHelper.ParseParamPartialNameOrId(_params[1], out platformUserIdentifierAbs, out clientInfo, true) != 1)
		{
			return;
		}
		if (GameManager.Instance.adminTools.Users.RemoveUser(platformUserIdentifierAbs, true))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("{0} removed from permissions list.", platformUserIdentifierAbs));
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("{0} was not on permissions list.", platformUserIdentifierAbs));
	}

	// Token: 0x06000D32 RID: 3378 RVA: 0x000587E0 File Offset: 0x000569E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteAddGroup(List<string> _params)
	{
		if (_params.Count != 4 && _params.Count != 5)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 4 or 5, found " + _params.Count.ToString() + ".");
			return;
		}
		string name = null;
		if (_params.Count > 4)
		{
			name = _params[4];
		}
		if (!ConsoleHelper.ParseParamSteamGroupIdValid(_params[1]))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("\"" + _params[1] + "\" is not a valid steam group id.");
			return;
		}
		int num;
		if (!int.TryParse(_params[2], out num))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("\"" + _params[2] + "\" is not a valid integer.");
			return;
		}
		int num2;
		if (!int.TryParse(_params[3], out num2))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("\"" + _params[2] + "\" is not a valid integer.");
			return;
		}
		GameManager.Instance.adminTools.Users.AddGroup(name, _params[1], num, num2);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Group {0} added with permission level of {1} for regulars, {2} for moderators.", _params[1], num, num2));
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x00058910 File Offset: 0x00056B10
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteRemoveGroup(List<string> _params)
	{
		if (_params.Count != 2)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 2, found " + _params.Count.ToString() + ".");
			return;
		}
		if (!ConsoleHelper.ParseParamSteamGroupIdValid(_params[1]))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("\"" + _params[1] + "\" is not a valid steam group id.");
			return;
		}
		if (GameManager.Instance.adminTools.Users.RemoveGroup(_params[1]))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Group " + _params[1] + " removed from permissions list.");
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Group " + _params[1] + " was not on permissions list.");
	}

	// Token: 0x06000D34 RID: 3380 RVA: 0x000589DC File Offset: 0x00056BDC
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteList()
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Defined User Permissions:");
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("  Level: UserID (Player name if online, stored name)");
		foreach (KeyValuePair<PlatformUserIdentifierAbs, AdminUsers.UserPermission> keyValuePair in GameManager.Instance.adminTools.Users.GetUsers())
		{
			AdminUsers.UserPermission value = keyValuePair.Value;
			ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForUserId(value.UserIdentifier);
			if (clientInfo != null)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("  {0,5}: {1} ({2}, stored name: {3})", new object[]
				{
					value.PermissionLevel,
					value.UserIdentifier,
					clientInfo.playerName,
					value.Name ?? ""
				}));
			}
			else
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("  {0,5}: {1} (stored name: {2})", value.PermissionLevel, value.UserIdentifier, value.Name ?? ""));
			}
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Defined Group Permissions:");
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("  Normal,  Mods: SteamID (Stored name)");
		foreach (KeyValuePair<string, AdminUsers.GroupPermission> keyValuePair2 in GameManager.Instance.adminTools.Users.GetGroups())
		{
			AdminUsers.GroupPermission value2 = keyValuePair2.Value;
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("  {0,6}, {1:5}: {2} (stored name: {3})", new object[]
			{
				value2.PermissionLevelNormal,
				value2.PermissionLevelMods,
				value2.SteamIdGroup,
				value2.Name ?? ""
			}));
		}
	}
}
