using System;
using System.Collections.Generic;
using System.Xml;

// Token: 0x0200007C RID: 124
public class AdminWhitelist : AdminSectionAbs
{
	// Token: 0x06000242 RID: 578 RVA: 0x00012C44 File Offset: 0x00010E44
	public AdminWhitelist(AdminTools _parent) : base(_parent, "whitelist")
	{
	}

	// Token: 0x06000243 RID: 579 RVA: 0x00012C68 File Offset: 0x00010E68
	public override void Clear()
	{
		this.whitelistedUsers.Clear();
		this.whitelistedGroups.Clear();
	}

	// Token: 0x06000244 RID: 580 RVA: 0x00012C80 File Offset: 0x00010E80
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ParseElement(XmlElement _childElement)
	{
		AdminWhitelist.WhitelistUser whitelistUser;
		if (_childElement.Name == "group")
		{
			AdminWhitelist.WhitelistGroup whitelistGroup;
			if (AdminWhitelist.WhitelistGroup.TryParse(_childElement, out whitelistGroup))
			{
				this.whitelistedGroups[whitelistGroup.SteamIdGroup] = whitelistGroup;
				return;
			}
		}
		else if (AdminWhitelist.WhitelistUser.TryParse(_childElement, out whitelistUser))
		{
			this.whitelistedUsers[whitelistUser.UserIdentifier] = whitelistUser;
		}
	}

	// Token: 0x06000245 RID: 581 RVA: 0x00012CD8 File Offset: 0x00010ED8
	public override void Save(XmlElement _root)
	{
		XmlElement xmlElement = _root.AddXmlElement(this.SectionTypeName);
		xmlElement.AddXmlComment(" ONLY PUT ITEMS IN WHITELIST IF YOU WANT WHITELIST ONLY ENABLED!!! ");
		xmlElement.AddXmlComment(" If there are any items in the whitelist, the whitelist only mode is enabled ");
		xmlElement.AddXmlComment(" Nobody can join that ISN'T in the whitelist or admins once whitelist only mode is enabled ");
		xmlElement.AddXmlComment(" Name is optional for display purposes only ");
		xmlElement.AddXmlComment(" <user platform=\"\" userid=\"\" name=\"\" /> ");
		xmlElement.AddXmlComment(" <group steamID=\"\" name=\"\" /> ");
		foreach (KeyValuePair<PlatformUserIdentifierAbs, AdminWhitelist.WhitelistUser> keyValuePair in this.whitelistedUsers)
		{
			keyValuePair.Value.ToXml(xmlElement);
		}
		foreach (KeyValuePair<string, AdminWhitelist.WhitelistGroup> keyValuePair2 in this.whitelistedGroups)
		{
			keyValuePair2.Value.ToXml(xmlElement);
		}
	}

	// Token: 0x06000246 RID: 582 RVA: 0x00012DD8 File Offset: 0x00010FD8
	public void AddUser(string _name, PlatformUserIdentifierAbs _identifier)
	{
		AdminTools parent = this.Parent;
		lock (parent)
		{
			AdminWhitelist.WhitelistUser value = new AdminWhitelist.WhitelistUser(_name, _identifier);
			this.whitelistedUsers[_identifier] = value;
			this.Parent.Save();
		}
	}

	// Token: 0x06000247 RID: 583 RVA: 0x00012E34 File Offset: 0x00011034
	public bool RemoveUser(PlatformUserIdentifierAbs _identifier)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			bool flag2 = this.whitelistedUsers.Remove(_identifier);
			if (flag2)
			{
				this.Parent.Save();
			}
			result = flag2;
		}
		return result;
	}

	// Token: 0x06000248 RID: 584 RVA: 0x00012E8C File Offset: 0x0001108C
	public Dictionary<PlatformUserIdentifierAbs, AdminWhitelist.WhitelistUser> GetUsers()
	{
		AdminTools parent = this.Parent;
		Dictionary<PlatformUserIdentifierAbs, AdminWhitelist.WhitelistUser> result;
		lock (parent)
		{
			result = this.whitelistedUsers;
		}
		return result;
	}

	// Token: 0x06000249 RID: 585 RVA: 0x00012ED0 File Offset: 0x000110D0
	public void AddGroup(string _name, string _steamId)
	{
		AdminTools parent = this.Parent;
		lock (parent)
		{
			AdminWhitelist.WhitelistGroup value = new AdminWhitelist.WhitelistGroup(_name, _steamId);
			this.whitelistedGroups[_steamId] = value;
			this.Parent.Save();
		}
	}

	// Token: 0x0600024A RID: 586 RVA: 0x00012F2C File Offset: 0x0001112C
	public bool RemoveGroup(string _steamId)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			bool flag2 = this.whitelistedGroups.Remove(_steamId);
			if (flag2)
			{
				this.Parent.Save();
			}
			result = flag2;
		}
		return result;
	}

	// Token: 0x0600024B RID: 587 RVA: 0x00012F84 File Offset: 0x00011184
	public Dictionary<string, AdminWhitelist.WhitelistGroup> GetGroups()
	{
		AdminTools parent = this.Parent;
		Dictionary<string, AdminWhitelist.WhitelistGroup> result;
		lock (parent)
		{
			result = this.whitelistedGroups;
		}
		return result;
	}

	// Token: 0x0600024C RID: 588 RVA: 0x00012FC8 File Offset: 0x000111C8
	public bool IsWhitelisted(ClientInfo _clientInfo)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			if (this.whitelistedUsers.ContainsKey(_clientInfo.PlatformId) || this.whitelistedUsers.ContainsKey(_clientInfo.CrossplatformId))
			{
				result = true;
			}
			else
			{
				foreach (KeyValuePair<string, int> keyValuePair in _clientInfo.groupMemberships)
				{
					if (this.whitelistedGroups.ContainsKey(keyValuePair.Key))
					{
						return true;
					}
				}
				result = false;
			}
		}
		return result;
	}

	// Token: 0x0600024D RID: 589 RVA: 0x00013084 File Offset: 0x00011284
	public bool IsWhiteListEnabled()
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			result = (this.whitelistedUsers.Count > 0 || this.whitelistedGroups.Count > 0);
		}
		return result;
	}

	// Token: 0x040002E5 RID: 741
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<PlatformUserIdentifierAbs, AdminWhitelist.WhitelistUser> whitelistedUsers = new Dictionary<PlatformUserIdentifierAbs, AdminWhitelist.WhitelistUser>();

	// Token: 0x040002E6 RID: 742
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, AdminWhitelist.WhitelistGroup> whitelistedGroups = new Dictionary<string, AdminWhitelist.WhitelistGroup>();

	// Token: 0x0200007D RID: 125
	public readonly struct WhitelistUser
	{
		// Token: 0x0600024E RID: 590 RVA: 0x000130E0 File Offset: 0x000112E0
		public WhitelistUser(string _name, PlatformUserIdentifierAbs _userIdentifier)
		{
			this.Name = _name;
			this.UserIdentifier = _userIdentifier;
		}

		// Token: 0x0600024F RID: 591 RVA: 0x000130F0 File Offset: 0x000112F0
		public void ToXml(XmlElement _parent)
		{
			XmlElement xmlElement = _parent.AddXmlElement("user");
			this.UserIdentifier.ToXml(xmlElement, "");
			if (this.Name != null)
			{
				xmlElement.SetAttrib("name", this.Name);
			}
		}

		// Token: 0x06000250 RID: 592 RVA: 0x00013134 File Offset: 0x00011334
		public static bool TryParse(XmlElement _element, out AdminWhitelist.WhitelistUser _result)
		{
			_result = default(AdminWhitelist.WhitelistUser);
			string text = _element.GetAttribute("name");
			if (text.Length == 0)
			{
				text = null;
			}
			PlatformUserIdentifierAbs platformUserIdentifierAbs = AdminTools.ParseUserIdentifier(_element);
			if (platformUserIdentifierAbs == null)
			{
				return false;
			}
			_result = new AdminWhitelist.WhitelistUser(text, platformUserIdentifierAbs);
			return true;
		}

		// Token: 0x040002E7 RID: 743
		public readonly string Name;

		// Token: 0x040002E8 RID: 744
		public readonly PlatformUserIdentifierAbs UserIdentifier;
	}

	// Token: 0x0200007E RID: 126
	public readonly struct WhitelistGroup
	{
		// Token: 0x06000251 RID: 593 RVA: 0x00013178 File Offset: 0x00011378
		public WhitelistGroup(string _name, string _steamIdGroup)
		{
			this.Name = _name;
			this.SteamIdGroup = _steamIdGroup;
		}

		// Token: 0x06000252 RID: 594 RVA: 0x00013188 File Offset: 0x00011388
		public void ToXml(XmlElement _parent)
		{
			XmlElement element = _parent.AddXmlElement("group");
			element.SetAttrib("steamID", this.SteamIdGroup);
			if (this.Name != null)
			{
				element.SetAttrib("name", this.Name);
			}
		}

		// Token: 0x06000253 RID: 595 RVA: 0x000131D0 File Offset: 0x000113D0
		public static bool TryParse(XmlElement _element, out AdminWhitelist.WhitelistGroup _result)
		{
			_result = default(AdminWhitelist.WhitelistGroup);
			string text = _element.GetAttribute("name");
			if (text.Length == 0)
			{
				text = null;
			}
			if (!_element.HasAttribute("steamID"))
			{
				Log.Warning("Ignoring whitelist-entry because of missing 'steamID' attribute: " + _element.OuterXml);
				return false;
			}
			string attribute = _element.GetAttribute("steamID");
			_result = new AdminWhitelist.WhitelistGroup(text, attribute);
			return true;
		}

		// Token: 0x040002E9 RID: 745
		public readonly string Name;

		// Token: 0x040002EA RID: 746
		public readonly string SteamIdGroup;
	}
}
