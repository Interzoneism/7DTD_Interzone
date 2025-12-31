using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

// Token: 0x02000832 RID: 2098
public class PhysicsBodyColliderConfiguration
{
	// Token: 0x06003C4F RID: 15439 RVA: 0x001841E4 File Offset: 0x001823E4
	public PhysicsBodyColliderConfiguration()
	{
	}

	// Token: 0x06003C50 RID: 15440 RVA: 0x00184248 File Offset: 0x00182448
	public PhysicsBodyColliderConfiguration(PhysicsBodyColliderConfiguration otherConfig)
	{
		this.Tag = otherConfig.Tag;
		this.CollisionLayer = otherConfig.CollisionLayer;
		this.RagdollLayer = otherConfig.RagdollLayer;
		this.CollisionScale = otherConfig.CollisionScale;
		this.CollisionOffset = otherConfig.CollisionOffset;
		this.RagdollScale = otherConfig.RagdollScale;
		this.RagdollOffset = otherConfig.RagdollOffset;
		this.Path = otherConfig.Path;
		this.Type = otherConfig.Type;
		this.EnabledFlags = otherConfig.EnabledFlags;
	}

	// Token: 0x06003C51 RID: 15441 RVA: 0x00184324 File Offset: 0x00182524
	public void Write(XmlElement _elem)
	{
		XmlElement node = _elem.AddXmlElement("collider");
		node.AddXmlKeyValueProperty("tag", this.Tag);
		node.AddXmlKeyValueProperty("path", this.Path);
		node.AddXmlKeyValueProperty("collisionLayer", this.CollisionLayer.ToString());
		node.AddXmlKeyValueProperty("ragdollLayer", this.RagdollLayer.ToString());
		node.AddXmlKeyValueProperty("collisionScale", PhysicsBodyColliderConfiguration.vecToString(this.CollisionScale));
		node.AddXmlKeyValueProperty("ragdollScale", PhysicsBodyColliderConfiguration.vecToString(this.RagdollScale));
		node.AddXmlKeyValueProperty("collisionOffset", PhysicsBodyColliderConfiguration.vecToString(this.CollisionOffset));
		node.AddXmlKeyValueProperty("ragdollOffset", PhysicsBodyColliderConfiguration.vecToString(this.RagdollOffset));
		node.AddXmlKeyValueProperty("type", this.Type.ToStringCached<EnumColliderType>());
		string text = "";
		if ((this.EnabledFlags & EnumColliderEnabledFlags.Collision) != EnumColliderEnabledFlags.Disabled)
		{
			text += "collision";
		}
		if ((this.EnabledFlags & EnumColliderEnabledFlags.Ragdoll) != EnumColliderEnabledFlags.Disabled)
		{
			if (text.Length == 0)
			{
				text = "ragdoll";
			}
			else
			{
				text += ";ragdoll";
			}
		}
		if (text.Length == 0)
		{
			text = "disabled";
		}
		node.AddXmlKeyValueProperty("flags", text);
	}

	// Token: 0x06003C52 RID: 15442 RVA: 0x00184460 File Offset: 0x00182660
	public static PhysicsBodyColliderConfiguration Read(XElement _e)
	{
		PhysicsBodyColliderConfiguration physicsBodyColliderConfiguration = new PhysicsBodyColliderConfiguration();
		DynamicProperties dynamicProperties = new DynamicProperties();
		foreach (XElement propertyNode in _e.Elements("property"))
		{
			dynamicProperties.Add(propertyNode, true, false);
		}
		physicsBodyColliderConfiguration.Tag = dynamicProperties.GetStringValue("tag");
		physicsBodyColliderConfiguration.Path = dynamicProperties.GetStringValue("path");
		if (dynamicProperties.Contains("collisionLayer"))
		{
			physicsBodyColliderConfiguration.CollisionLayer = int.Parse(dynamicProperties.GetStringValue("collisionLayer"));
			physicsBodyColliderConfiguration.RagdollLayer = int.Parse(dynamicProperties.GetStringValue("ragdollLayer"));
		}
		else
		{
			physicsBodyColliderConfiguration.CollisionLayer = int.Parse(dynamicProperties.GetStringValue("layer"));
			physicsBodyColliderConfiguration.RagdollLayer = physicsBodyColliderConfiguration.CollisionLayer;
		}
		physicsBodyColliderConfiguration.CollisionScale = PhysicsBodyColliderConfiguration.vecFromString(dynamicProperties.GetStringValue("collisionScale"));
		physicsBodyColliderConfiguration.RagdollScale = PhysicsBodyColliderConfiguration.vecFromString(dynamicProperties.GetStringValue("ragdollScale"));
		physicsBodyColliderConfiguration.CollisionOffset = PhysicsBodyColliderConfiguration.vecFromString(dynamicProperties.GetStringValue("collisionOffset"));
		physicsBodyColliderConfiguration.RagdollOffset = PhysicsBodyColliderConfiguration.vecFromString(dynamicProperties.GetStringValue("ragdollOffset"));
		physicsBodyColliderConfiguration.Type = EnumUtils.Parse<EnumColliderType>(dynamicProperties.GetStringValue("type"), false);
		physicsBodyColliderConfiguration.EnabledFlags = EnumColliderEnabledFlags.Disabled;
		string stringValue = dynamicProperties.GetStringValue("flags");
		if (stringValue != "disabled")
		{
			foreach (string a in stringValue.Split(';', StringSplitOptions.None))
			{
				if (a == "collision")
				{
					physicsBodyColliderConfiguration.EnabledFlags |= EnumColliderEnabledFlags.Collision;
				}
				else if (a == "ragdoll")
				{
					physicsBodyColliderConfiguration.EnabledFlags |= EnumColliderEnabledFlags.Ragdoll;
				}
			}
		}
		if (physicsBodyColliderConfiguration.RagdollLayer == 0 || physicsBodyColliderConfiguration.RagdollLayer == 27)
		{
			physicsBodyColliderConfiguration.RagdollLayer = 21;
		}
		return physicsBodyColliderConfiguration;
	}

	// Token: 0x06003C53 RID: 15443 RVA: 0x00184650 File Offset: 0x00182850
	[PublicizedFrom(EAccessModifier.Private)]
	public static string vecToString(Vector3 vec)
	{
		return string.Concat(new string[]
		{
			vec.x.ToCultureInvariantString(),
			" ",
			vec.y.ToCultureInvariantString(),
			" ",
			vec.z.ToCultureInvariantString()
		});
	}

	// Token: 0x06003C54 RID: 15444 RVA: 0x001846A4 File Offset: 0x001828A4
	[PublicizedFrom(EAccessModifier.Private)]
	public static Vector3 vecFromString(string str)
	{
		string[] array = str.Split(' ', StringSplitOptions.None);
		if (array.Length == 3)
		{
			return new Vector3(StringParsers.ParseFloat(array[0], 0, -1, NumberStyles.Any), StringParsers.ParseFloat(array[1], 0, -1, NumberStyles.Any), StringParsers.ParseFloat(array[2], 0, -1, NumberStyles.Any));
		}
		if (array.Length < 1)
		{
			throw new FormatException("Vector3 expected");
		}
		return new Vector3(StringParsers.ParseFloat(array[0], 0, -1, NumberStyles.Any), StringParsers.ParseFloat(array[0], 0, -1, NumberStyles.Any), StringParsers.ParseFloat(array[0], 0, -1, NumberStyles.Any));
	}

	// Token: 0x040030D0 RID: 12496
	public string Tag = "";

	// Token: 0x040030D1 RID: 12497
	public int CollisionLayer;

	// Token: 0x040030D2 RID: 12498
	public int RagdollLayer;

	// Token: 0x040030D3 RID: 12499
	public Vector3 CollisionScale = Vector3.one;

	// Token: 0x040030D4 RID: 12500
	public Vector3 RagdollScale = Vector3.one;

	// Token: 0x040030D5 RID: 12501
	public Vector3 CollisionOffset = Vector3.zero;

	// Token: 0x040030D6 RID: 12502
	public Vector3 RagdollOffset = Vector3.zero;

	// Token: 0x040030D7 RID: 12503
	public string Path = "";

	// Token: 0x040030D8 RID: 12504
	public EnumColliderType Type = EnumColliderType.Detail;

	// Token: 0x040030D9 RID: 12505
	public EnumColliderEnabledFlags EnabledFlags = EnumColliderEnabledFlags.All;
}
