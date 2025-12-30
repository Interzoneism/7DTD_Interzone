using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

// Token: 0x02000072 RID: 114
public class AdminBlacklist : AdminSectionAbs
{
	// Token: 0x06000201 RID: 513 RVA: 0x00011370 File Offset: 0x0000F570
	public AdminBlacklist(AdminTools _parent) : base(_parent, "blacklist")
	{
	}

	// Token: 0x06000202 RID: 514 RVA: 0x00011389 File Offset: 0x0000F589
	public override void Clear()
	{
		this.bannedUsers.Clear();
	}

	// Token: 0x06000203 RID: 515 RVA: 0x00011398 File Offset: 0x0000F598
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ParseElement(XmlElement _childElement)
	{
		AdminBlacklist.BannedUser bannedUser;
		if (AdminBlacklist.BannedUser.TryParse(_childElement, out bannedUser))
		{
			this.bannedUsers[bannedUser.UserIdentifier] = bannedUser;
		}
	}

	// Token: 0x06000204 RID: 516 RVA: 0x000113C4 File Offset: 0x0000F5C4
	public override void Save(XmlElement _root)
	{
		XmlElement xmlElement = _root.AddXmlElement("blacklist");
		xmlElement.AddXmlComment(" <blacklisted platform=\"\" userid=\"\" name=\"\" unbandate=\"\" reason=\"\" /> ");
		foreach (KeyValuePair<PlatformUserIdentifierAbs, AdminBlacklist.BannedUser> keyValuePair in this.bannedUsers)
		{
			keyValuePair.Value.ToXml(xmlElement);
		}
	}

	// Token: 0x06000205 RID: 517 RVA: 0x00011438 File Offset: 0x0000F638
	public void AddBan(string _name, PlatformUserIdentifierAbs _identifier, DateTime _banUntil, string _banReason)
	{
		AdminTools parent = this.Parent;
		lock (parent)
		{
			AdminBlacklist.BannedUser value = new AdminBlacklist.BannedUser(_name, _identifier, _banUntil, _banReason);
			this.bannedUsers[_identifier] = value;
			if (_banUntil > DateTime.Now)
			{
				this.Parent.Users.RemoveUser(_identifier, false);
			}
			this.Parent.Save();
		}
	}

	// Token: 0x06000206 RID: 518 RVA: 0x000114B8 File Offset: 0x0000F6B8
	public bool RemoveBan(PlatformUserIdentifierAbs _identifier)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			bool flag2 = this.bannedUsers.Remove(_identifier);
			if (flag2)
			{
				this.Parent.Save();
			}
			result = flag2;
		}
		return result;
	}

	// Token: 0x06000207 RID: 519 RVA: 0x00011510 File Offset: 0x0000F710
	public bool IsBanned(PlatformUserIdentifierAbs _identifier, out DateTime _bannedUntil, out string _reason)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			if (this.bannedUsers.ContainsKey(_identifier))
			{
				AdminBlacklist.BannedUser bannedUser = this.bannedUsers[_identifier];
				if (bannedUser.BannedUntil > DateTime.Now)
				{
					_bannedUntil = bannedUser.BannedUntil;
					_reason = bannedUser.BanReason;
					return true;
				}
			}
			_bannedUntil = DateTime.Now;
			_reason = string.Empty;
			result = false;
		}
		return result;
	}

	// Token: 0x06000208 RID: 520 RVA: 0x000115A4 File Offset: 0x0000F7A4
	public List<AdminBlacklist.BannedUser> GetBanned()
	{
		AdminTools parent = this.Parent;
		List<AdminBlacklist.BannedUser> result;
		lock (parent)
		{
			result = (from _b in this.bannedUsers.Values
			where _b.BannedUntil > DateTime.Now
			select _b).ToList<AdminBlacklist.BannedUser>();
		}
		return result;
	}

	// Token: 0x040002C6 RID: 710
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<PlatformUserIdentifierAbs, AdminBlacklist.BannedUser> bannedUsers = new Dictionary<PlatformUserIdentifierAbs, AdminBlacklist.BannedUser>();

	// Token: 0x02000073 RID: 115
	public readonly struct BannedUser
	{
		// Token: 0x06000209 RID: 521 RVA: 0x00011614 File Offset: 0x0000F814
		public BannedUser(string _name, PlatformUserIdentifierAbs _userIdentifier, DateTime _banUntil, string _banReason)
		{
			this.Name = _name;
			this.UserIdentifier = _userIdentifier;
			this.BannedUntil = _banUntil;
			this.BanReason = (_banReason ?? string.Empty);
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0001163C File Offset: 0x0000F83C
		public void ToXml(XmlElement _parent)
		{
			XmlElement xmlElement = _parent.AddXmlElement("blacklisted");
			this.UserIdentifier.ToXml(xmlElement, "");
			if (this.Name != null)
			{
				xmlElement.SetAttrib("name", this.Name);
			}
			xmlElement.SetAttrib("unbandate", this.BannedUntil.ToCultureInvariantString());
			xmlElement.SetAttrib("reason", this.BanReason);
		}

		// Token: 0x0600020B RID: 523 RVA: 0x000116AC File Offset: 0x0000F8AC
		public static bool TryParse(XmlElement _element, out AdminBlacklist.BannedUser _result)
		{
			_result = default(AdminBlacklist.BannedUser);
			string text = _element.GetAttribute("name");
			if (text.Length == 0)
			{
				text = null;
			}
			if (!_element.HasAttribute("unbandate"))
			{
				Log.Warning("Ignoring blacklist-entry because of missing 'unbandate' attribute: " + _element.OuterXml);
				return false;
			}
			DateTime banUntil;
			if (!StringParsers.TryParseDateTime(_element.GetAttribute("unbandate"), out banUntil) && !DateTime.TryParse(_element.GetAttribute("unbandate"), out banUntil))
			{
				Log.Warning("Ignoring blacklist-entry because of invalid value for 'unbandate' attribute: " + _element.OuterXml);
				return false;
			}
			PlatformUserIdentifierAbs platformUserIdentifierAbs = AdminTools.ParseUserIdentifier(_element);
			if (platformUserIdentifierAbs == null)
			{
				return false;
			}
			string attribute = _element.GetAttribute("reason");
			_result = new AdminBlacklist.BannedUser(text, platformUserIdentifierAbs, banUntil, attribute);
			return true;
		}

		// Token: 0x040002C7 RID: 711
		public readonly string Name;

		// Token: 0x040002C8 RID: 712
		public readonly PlatformUserIdentifierAbs UserIdentifier;

		// Token: 0x040002C9 RID: 713
		public readonly DateTime BannedUntil;

		// Token: 0x040002CA RID: 714
		public readonly string BanReason;
	}
}
