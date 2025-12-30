using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

// Token: 0x02000834 RID: 2100
public class PhysicsBodyLayout
{
	// Token: 0x1700062B RID: 1579
	// (get) Token: 0x06003C58 RID: 15448 RVA: 0x00184737 File Offset: 0x00182937
	public string Name
	{
		get
		{
			return this.name;
		}
	}

	// Token: 0x1700062C RID: 1580
	// (get) Token: 0x06003C59 RID: 15449 RVA: 0x0018473F File Offset: 0x0018293F
	public List<PhysicsBodyColliderConfiguration> Colliders
	{
		get
		{
			return this.colliders;
		}
	}

	// Token: 0x06003C5A RID: 15450 RVA: 0x00184748 File Offset: 0x00182948
	[PublicizedFrom(EAccessModifier.Private)]
	public static PhysicsBodyLayout New(string _name)
	{
		if (PhysicsBodyLayout.bodyLayouts.ContainsKey(_name))
		{
			throw new Exception("duplicate physics body!");
		}
		PhysicsBodyLayout physicsBodyLayout = new PhysicsBodyLayout();
		physicsBodyLayout.name = _name;
		PhysicsBodyLayout.bodyLayouts[_name] = physicsBodyLayout;
		return physicsBodyLayout;
	}

	// Token: 0x06003C5B RID: 15451 RVA: 0x00184788 File Offset: 0x00182988
	public static PhysicsBodyLayout New()
	{
		int num = 0;
		string key;
		for (;;)
		{
			key = string.Format("unnamed{0}", num);
			if (!PhysicsBodyLayout.bodyLayouts.ContainsKey(key))
			{
				break;
			}
			num++;
		}
		return PhysicsBodyLayout.New(key);
	}

	// Token: 0x06003C5C RID: 15452 RVA: 0x001847C1 File Offset: 0x001829C1
	public bool Rename(string newName)
	{
		if (PhysicsBodyLayout.bodyLayouts.ContainsKey(newName))
		{
			return false;
		}
		PhysicsBodyLayout.bodyLayouts.Remove(this.name);
		PhysicsBodyLayout.bodyLayouts[newName] = this;
		this.name = newName;
		return true;
	}

	// Token: 0x06003C5D RID: 15453 RVA: 0x001847F7 File Offset: 0x001829F7
	public static bool Remove(string _name)
	{
		return PhysicsBodyLayout.bodyLayouts.Remove(_name);
	}

	// Token: 0x06003C5E RID: 15454 RVA: 0x00184804 File Offset: 0x00182A04
	public static PhysicsBodyLayout Find(string _name)
	{
		PhysicsBodyLayout result = null;
		PhysicsBodyLayout.bodyLayouts.TryGetValue(_name, out result);
		return result;
	}

	// Token: 0x1700062D RID: 1581
	// (get) Token: 0x06003C5F RID: 15455 RVA: 0x00184824 File Offset: 0x00182A24
	public static PhysicsBodyLayout[] BodyLayouts
	{
		get
		{
			PhysicsBodyLayout[] array = new PhysicsBodyLayout[PhysicsBodyLayout.bodyLayouts.Count];
			PhysicsBodyLayout.bodyLayouts.CopyValuesTo(array);
			return array;
		}
	}

	// Token: 0x06003C60 RID: 15456 RVA: 0x0018484D File Offset: 0x00182A4D
	public static void Reset()
	{
		PhysicsBodyLayout.bodyLayouts.Clear();
	}

	// Token: 0x06003C61 RID: 15457 RVA: 0x0018485C File Offset: 0x00182A5C
	public static PhysicsBodyLayout Read(XElement _e)
	{
		if (!_e.HasAttribute("name"))
		{
			throw new Exception("Physics body needs a name");
		}
		PhysicsBodyLayout physicsBodyLayout = PhysicsBodyLayout.New(_e.GetAttribute("name"));
		foreach (XElement e in _e.Elements("collider"))
		{
			physicsBodyLayout.colliders.Add(PhysicsBodyColliderConfiguration.Read(e));
		}
		return physicsBodyLayout;
	}

	// Token: 0x06003C62 RID: 15458 RVA: 0x001848F4 File Offset: 0x00182AF4
	public void Write(XmlElement _elem)
	{
		XmlElement elem = _elem.AddXmlElement("body").SetAttrib("name", this.name);
		for (int i = 0; i < this.colliders.Count; i++)
		{
			this.colliders[i].Write(elem);
		}
	}

	// Token: 0x040030DA RID: 12506
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, PhysicsBodyLayout> bodyLayouts = new Dictionary<string, PhysicsBodyLayout>();

	// Token: 0x040030DB RID: 12507
	[PublicizedFrom(EAccessModifier.Private)]
	public List<PhysicsBodyColliderConfiguration> colliders = new List<PhysicsBodyColliderConfiguration>();

	// Token: 0x040030DC RID: 12508
	[PublicizedFrom(EAccessModifier.Private)]
	public string name;
}
