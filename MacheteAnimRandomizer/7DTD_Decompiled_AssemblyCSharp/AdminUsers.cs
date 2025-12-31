using System;
using System.Collections.Generic;
using System.Xml;

// Token: 0x02000079 RID: 121
public class AdminUsers : AdminSectionAbs
{
	// Token: 0x0600022F RID: 559 RVA: 0x00012448 File Offset: 0x00010648
	public AdminUsers(AdminTools _parent) : base(_parent, "users")
	{
	}

	// Token: 0x06000230 RID: 560 RVA: 0x0001246C File Offset: 0x0001066C
	public override void Clear()
	{
		this.userPermissions.Clear();
		this.groupPermissions.Clear();
	}

	// Token: 0x06000231 RID: 561 RVA: 0x00012484 File Offset: 0x00010684
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ParseElement(XmlElement _childElement)
	{
		AdminUsers.UserPermission userPermission;
		if (_childElement.Name == "group")
		{
			AdminUsers.GroupPermission groupPermission;
			if (AdminUsers.GroupPermission.TryParse(_childElement, out groupPermission))
			{
				this.groupPermissions[groupPermission.SteamIdGroup] = groupPermission;
				return;
			}
		}
		else if (AdminUsers.UserPermission.TryParse(_childElement, out userPermission))
		{
			this.userPermissions[userPermission.UserIdentifier] = userPermission;
		}
	}

	// Token: 0x06000232 RID: 562 RVA: 0x000124DC File Offset: 0x000106DC
	public override void Save(XmlElement _root)
	{
		XmlElement xmlElement = _root.AddXmlElement(this.SectionTypeName);
		xmlElement.AddXmlComment(" <user platform=\"Steam\" userid=\"76561198021925107\" name=\"Hint on who this user is\" permission_level=\"0\" /> ");
		xmlElement.AddXmlComment(" <group steamID=\"103582791434672565\" name=\"Steam Universe\" permission_level_default=\"1000\" permission_level_mod=\"0\" /> ");
		foreach (KeyValuePair<PlatformUserIdentifierAbs, AdminUsers.UserPermission> keyValuePair in this.userPermissions)
		{
			keyValuePair.Value.ToXml(xmlElement);
		}
		foreach (KeyValuePair<string, AdminUsers.GroupPermission> keyValuePair2 in this.groupPermissions)
		{
			keyValuePair2.Value.ToXml(xmlElement);
		}
	}

	// Token: 0x06000233 RID: 563 RVA: 0x000125AC File Offset: 0x000107AC
	public void AddUser(string _name, PlatformUserIdentifierAbs _identifier, int _permissionLevel)
	{
		AdminTools parent = this.Parent;
		lock (parent)
		{
			AdminUsers.UserPermission value = new AdminUsers.UserPermission(_name, _identifier, _permissionLevel);
			this.userPermissions[_identifier] = value;
			this.Parent.Save();
		}
	}

	// Token: 0x06000234 RID: 564 RVA: 0x00012608 File Offset: 0x00010808
	public bool RemoveUser(PlatformUserIdentifierAbs _identifier, bool _save = true)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			bool flag2 = this.userPermissions.Remove(_identifier);
			if (flag2 && _save)
			{
				this.Parent.Save();
			}
			result = flag2;
		}
		return result;
	}

	// Token: 0x06000235 RID: 565 RVA: 0x00012660 File Offset: 0x00010860
	public bool HasEntry(ClientInfo _clientInfo)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			result = (this.userPermissions.ContainsKey(_clientInfo.PlatformId) || this.userPermissions.ContainsKey(_clientInfo.CrossplatformId));
		}
		return result;
	}

	// Token: 0x06000236 RID: 566 RVA: 0x000126C4 File Offset: 0x000108C4
	public Dictionary<PlatformUserIdentifierAbs, AdminUsers.UserPermission> GetUsers()
	{
		AdminTools parent = this.Parent;
		Dictionary<PlatformUserIdentifierAbs, AdminUsers.UserPermission> result;
		lock (parent)
		{
			result = this.userPermissions;
		}
		return result;
	}

	// Token: 0x06000237 RID: 567 RVA: 0x00012708 File Offset: 0x00010908
	public void AddGroup(string _name, string _steamId, int _permissionLevelDefault, int _permissionLevelMod)
	{
		AdminTools parent = this.Parent;
		lock (parent)
		{
			AdminUsers.GroupPermission value = new AdminUsers.GroupPermission(_name, _steamId, _permissionLevelDefault, _permissionLevelMod);
			this.groupPermissions[_steamId] = value;
			this.Parent.Save();
		}
	}

	// Token: 0x06000238 RID: 568 RVA: 0x00012768 File Offset: 0x00010968
	public bool RemoveGroup(string _steamId)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			bool flag2 = this.groupPermissions.Remove(_steamId);
			if (flag2)
			{
				this.Parent.Save();
			}
			result = flag2;
		}
		return result;
	}

	// Token: 0x06000239 RID: 569 RVA: 0x000127C0 File Offset: 0x000109C0
	public Dictionary<string, AdminUsers.GroupPermission> GetGroups()
	{
		AdminTools parent = this.Parent;
		Dictionary<string, AdminUsers.GroupPermission> result;
		lock (parent)
		{
			result = this.groupPermissions;
		}
		return result;
	}

	// Token: 0x0600023A RID: 570 RVA: 0x00012804 File Offset: 0x00010A04
	public int GetUserPermissionLevel(PlatformUserIdentifierAbs _userId)
	{
		AdminTools parent = this.Parent;
		int result;
		lock (parent)
		{
			AdminUsers.UserPermission userPermission;
			if (this.userPermissions.TryGetValue(_userId, out userPermission))
			{
				result = userPermission.PermissionLevel;
			}
			else
			{
				result = 1000;
			}
		}
		return result;
	}

	// Token: 0x0600023B RID: 571 RVA: 0x00012860 File Offset: 0x00010A60
	public int GetUserPermissionLevel(ClientInfo _clientInfo)
	{
		AdminTools parent = this.Parent;
		int result;
		lock (parent)
		{
			int num = 1000;
			AdminUsers.UserPermission userPermission;
			if (this.userPermissions.TryGetValue(_clientInfo.PlatformId, out userPermission))
			{
				num = userPermission.PermissionLevel;
			}
			AdminUsers.UserPermission userPermission2;
			if (_clientInfo.CrossplatformId != null && this.userPermissions.TryGetValue(_clientInfo.CrossplatformId, out userPermission2))
			{
				num = Math.Min(num, userPermission2.PermissionLevel);
			}
			if (this.groupPermissions.Count > 0 && _clientInfo.groupMemberships.Count > 0)
			{
				int num2 = int.MaxValue;
				foreach (KeyValuePair<string, int> keyValuePair in _clientInfo.groupMemberships)
				{
					AdminUsers.GroupPermission groupPermission;
					if (this.groupPermissions.TryGetValue(keyValuePair.Key, out groupPermission))
					{
						num2 = Math.Min(num2, (keyValuePair.Value == 2) ? groupPermission.PermissionLevelMods : groupPermission.PermissionLevelNormal);
					}
				}
				num = Math.Min(num, num2);
			}
			result = num;
		}
		return result;
	}

	// Token: 0x040002DC RID: 732
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<PlatformUserIdentifierAbs, AdminUsers.UserPermission> userPermissions = new Dictionary<PlatformUserIdentifierAbs, AdminUsers.UserPermission>();

	// Token: 0x040002DD RID: 733
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, AdminUsers.GroupPermission> groupPermissions = new Dictionary<string, AdminUsers.GroupPermission>();

	// Token: 0x0200007A RID: 122
	public readonly struct UserPermission
	{
		// Token: 0x0600023C RID: 572 RVA: 0x00012994 File Offset: 0x00010B94
		public UserPermission(string _name, PlatformUserIdentifierAbs _userIdentifier, int _permissionLevel)
		{
			this.Name = _name;
			this.UserIdentifier = _userIdentifier;
			this.PermissionLevel = _permissionLevel;
		}

		// Token: 0x0600023D RID: 573 RVA: 0x000129AC File Offset: 0x00010BAC
		public void ToXml(XmlElement _parent)
		{
			XmlElement xmlElement = _parent.AddXmlElement("user");
			this.UserIdentifier.ToXml(xmlElement, "");
			if (this.Name != null)
			{
				xmlElement.SetAttrib("name", this.Name);
			}
			xmlElement.SetAttrib("permission_level", this.PermissionLevel.ToString());
		}

		// Token: 0x0600023E RID: 574 RVA: 0x00012A0C File Offset: 0x00010C0C
		public static bool TryParse(XmlElement _element, out AdminUsers.UserPermission _result)
		{
			_result = default(AdminUsers.UserPermission);
			string text = _element.GetAttribute("name");
			if (text.Length == 0)
			{
				text = null;
			}
			if (!_element.HasAttribute("permission_level"))
			{
				Log.Warning("Ignoring admin-entry because of missing 'permission_level' attribute: " + _element.OuterXml);
				return false;
			}
			int permissionLevel;
			if (!int.TryParse(_element.GetAttribute("permission_level"), out permissionLevel))
			{
				Log.Warning("Ignoring admin-entry because of invalid (non-numeric) value for 'permission_level' attribute: " + _element.OuterXml);
				return false;
			}
			PlatformUserIdentifierAbs platformUserIdentifierAbs = AdminTools.ParseUserIdentifier(_element);
			if (platformUserIdentifierAbs == null)
			{
				return false;
			}
			_result = new AdminUsers.UserPermission(text, platformUserIdentifierAbs, permissionLevel);
			return true;
		}

		// Token: 0x040002DE RID: 734
		public readonly string Name;

		// Token: 0x040002DF RID: 735
		public readonly PlatformUserIdentifierAbs UserIdentifier;

		// Token: 0x040002E0 RID: 736
		public readonly int PermissionLevel;
	}

	// Token: 0x0200007B RID: 123
	public readonly struct GroupPermission
	{
		// Token: 0x0600023F RID: 575 RVA: 0x00012AA0 File Offset: 0x00010CA0
		public GroupPermission(string _name, string _steamIdGroup, int _permissionLevelNormal, int _permissionLevelMods)
		{
			this.Name = _name;
			this.SteamIdGroup = _steamIdGroup;
			this.PermissionLevelNormal = _permissionLevelNormal;
			this.PermissionLevelMods = _permissionLevelMods;
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00012AC0 File Offset: 0x00010CC0
		public void ToXml(XmlElement _parent)
		{
			XmlElement element = _parent.AddXmlElement("group");
			element.SetAttrib("steamID", this.SteamIdGroup);
			if (this.Name != null)
			{
				element.SetAttrib("name", this.Name);
			}
			element.SetAttrib("permission_level_default", this.PermissionLevelNormal.ToString());
			element.SetAttrib("permission_level_mod", this.PermissionLevelMods.ToString());
		}

		// Token: 0x06000241 RID: 577 RVA: 0x00012B3C File Offset: 0x00010D3C
		public static bool TryParse(XmlElement _element, out AdminUsers.GroupPermission _result)
		{
			_result = default(AdminUsers.GroupPermission);
			string text = _element.GetAttribute("name");
			if (text.Length == 0)
			{
				text = null;
			}
			if (!_element.HasAttribute("steamID"))
			{
				Log.Warning("Ignoring admin-entry because of missing 'steamID' attribute: " + _element.OuterXml);
				return false;
			}
			string attribute = _element.GetAttribute("steamID");
			if (!_element.HasAttribute("permission_level_default"))
			{
				Log.Warning("Ignoring admin-entry because of missing 'permission_level_default' attribute on group: " + _element.OuterXml);
				return false;
			}
			int permissionLevelNormal;
			if (!int.TryParse(_element.GetAttribute("permission_level_default"), out permissionLevelNormal))
			{
				Log.Warning("Ignoring admin-entry because of invalid (non-numeric) value for 'permission_level_default' attribute on group: " + _element.OuterXml);
				return false;
			}
			if (!_element.HasAttribute("permission_level_mod"))
			{
				Log.Warning("Ignoring admin-entry because of missing 'permission_level_mod' attribute on group: " + _element.OuterXml);
				return false;
			}
			int permissionLevelMods;
			if (!int.TryParse(_element.GetAttribute("permission_level_mod"), out permissionLevelMods))
			{
				Log.Warning("Ignoring admin-entry because of invalid (non-numeric) value for 'permission_level_mod' attribute on group: " + _element.OuterXml);
				return false;
			}
			_result = new AdminUsers.GroupPermission(text, attribute, permissionLevelNormal, permissionLevelMods);
			return true;
		}

		// Token: 0x040002E1 RID: 737
		public readonly string Name;

		// Token: 0x040002E2 RID: 738
		public readonly string SteamIdGroup;

		// Token: 0x040002E3 RID: 739
		public readonly int PermissionLevelNormal;

		// Token: 0x040002E4 RID: 740
		public readonly int PermissionLevelMods;
	}
}
