using System;
using System.Collections.Generic;
using System.Xml;

// Token: 0x02000075 RID: 117
public class AdminCommands : AdminSectionAbs
{
	// Token: 0x0600020F RID: 527 RVA: 0x0001177F File Offset: 0x0000F97F
	public AdminCommands(AdminTools _parent) : base(_parent, "commands")
	{
	}

	// Token: 0x06000210 RID: 528 RVA: 0x000117A9 File Offset: 0x0000F9A9
	public override void Clear()
	{
		this.commands.Clear();
	}

	// Token: 0x06000211 RID: 529 RVA: 0x000117B8 File Offset: 0x0000F9B8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ParseElement(XmlElement _childElement)
	{
		AdminCommands.CommandPermission commandPermission;
		if (AdminCommands.CommandPermission.TryParse(_childElement, out commandPermission))
		{
			this.commands[commandPermission.Command] = commandPermission;
		}
	}

	// Token: 0x06000212 RID: 530 RVA: 0x000117E4 File Offset: 0x0000F9E4
	public override void Save(XmlElement _root)
	{
		XmlElement xmlElement = _root.AddXmlElement(this.SectionTypeName);
		xmlElement.AddXmlComment(" <permission cmd=\"dm\" permission_level=\"0\" /> ");
		xmlElement.AddXmlComment(" <permission cmd=\"kick\" permission_level=\"1\" /> ");
		xmlElement.AddXmlComment(" <permission cmd=\"say\" permission_level=\"1000\" /> ");
		foreach (KeyValuePair<string, AdminCommands.CommandPermission> keyValuePair in this.commands)
		{
			keyValuePair.Value.ToXml(xmlElement);
		}
	}

	// Token: 0x06000213 RID: 531 RVA: 0x00011874 File Offset: 0x0000FA74
	public void AddCommand(string _cmd, int _permissionLevel, bool _save = true)
	{
		AdminTools parent = this.Parent;
		lock (parent)
		{
			AdminCommands.CommandPermission value = new AdminCommands.CommandPermission(_cmd, _permissionLevel);
			this.commands[_cmd] = value;
			if (_save)
			{
				this.Parent.Save();
			}
		}
	}

	// Token: 0x06000214 RID: 532 RVA: 0x000118D4 File Offset: 0x0000FAD4
	public bool RemoveCommand(string[] _cmds)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			bool flag2 = false;
			foreach (string key in _cmds)
			{
				flag2 |= this.commands.Remove(key);
			}
			if (flag2)
			{
				this.Parent.Save();
			}
			result = flag2;
		}
		return result;
	}

	// Token: 0x06000215 RID: 533 RVA: 0x00011950 File Offset: 0x0000FB50
	public bool IsPermissionDefined(string[] _cmds)
	{
		AdminTools parent = this.Parent;
		bool result;
		lock (parent)
		{
			foreach (string key in _cmds)
			{
				if (this.commands.ContainsKey(key))
				{
					return true;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x06000216 RID: 534 RVA: 0x000119B8 File Offset: 0x0000FBB8
	public Dictionary<string, AdminCommands.CommandPermission> GetCommands()
	{
		AdminTools parent = this.Parent;
		Dictionary<string, AdminCommands.CommandPermission> result;
		lock (parent)
		{
			result = this.commands;
		}
		return result;
	}

	// Token: 0x06000217 RID: 535 RVA: 0x000119FC File Offset: 0x0000FBFC
	public int GetCommandPermissionLevel(string[] _cmdNames)
	{
		AdminTools parent = this.Parent;
		int permissionLevel;
		lock (parent)
		{
			permissionLevel = this.GetAdminToolsCommandPermission(_cmdNames).PermissionLevel;
		}
		return permissionLevel;
	}

	// Token: 0x06000218 RID: 536 RVA: 0x00011A44 File Offset: 0x0000FC44
	public AdminCommands.CommandPermission GetAdminToolsCommandPermission(string[] _cmdNames)
	{
		AdminTools parent = this.Parent;
		AdminCommands.CommandPermission result;
		lock (parent)
		{
			foreach (string text in _cmdNames)
			{
				if (!string.IsNullOrEmpty(text) && this.commands.ContainsKey(text))
				{
					return this.commands[text];
				}
			}
			result = this.defaultCommandPermission;
		}
		return result;
	}

	// Token: 0x040002CD RID: 717
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, AdminCommands.CommandPermission> commands = new Dictionary<string, AdminCommands.CommandPermission>();

	// Token: 0x040002CE RID: 718
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly AdminCommands.CommandPermission defaultCommandPermission = new AdminCommands.CommandPermission("", 0);

	// Token: 0x02000076 RID: 118
	public readonly struct CommandPermission
	{
		// Token: 0x06000219 RID: 537 RVA: 0x00011AC8 File Offset: 0x0000FCC8
		public CommandPermission(string _cmd, int _permissionLevel)
		{
			this.Command = _cmd;
			this.PermissionLevel = _permissionLevel;
		}

		// Token: 0x0600021A RID: 538 RVA: 0x00011AD8 File Offset: 0x0000FCD8
		public void ToXml(XmlElement _parent)
		{
			_parent.AddXmlElement("permission").SetAttrib("cmd", this.Command).SetAttrib("permission_level", this.PermissionLevel.ToString());
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00011B1C File Offset: 0x0000FD1C
		public static bool TryParse(XmlElement _element, out AdminCommands.CommandPermission _result)
		{
			_result = default(AdminCommands.CommandPermission);
			string attribute = _element.GetAttribute("cmd");
			if (string.IsNullOrEmpty(attribute))
			{
				Log.Warning("Ignoring permission-entry because of missing or empty 'cmd' attribute: " + _element.OuterXml);
				return false;
			}
			if (!_element.HasAttribute("permission_level"))
			{
				Log.Warning("Ignoring permission-entry because of missing 'permission_level' attribute: " + _element.OuterXml);
				return false;
			}
			int permissionLevel;
			if (!int.TryParse(_element.GetAttribute("permission_level"), out permissionLevel))
			{
				Log.Warning("Ignoring permission-entry because of invalid (non-numeric) value for 'permission_level' attribute: " + _element.OuterXml);
				return false;
			}
			_result = new AdminCommands.CommandPermission(attribute, permissionLevel);
			return true;
		}

		// Token: 0x040002CF RID: 719
		public readonly string Command;

		// Token: 0x040002D0 RID: 720
		public readonly int PermissionLevel;
	}
}
