using System;
using System.Xml;

// Token: 0x02000077 RID: 119
public abstract class AdminSectionAbs
{
	// Token: 0x0600021C RID: 540 RVA: 0x00011BB8 File Offset: 0x0000FDB8
	[PublicizedFrom(EAccessModifier.Protected)]
	public AdminSectionAbs(AdminTools _parent, string _sectionTypeName)
	{
		this.Parent = _parent;
		this.SectionTypeName = _sectionTypeName;
	}

	// Token: 0x0600021D RID: 541
	public abstract void Clear();

	// Token: 0x0600021E RID: 542 RVA: 0x00011BD0 File Offset: 0x0000FDD0
	public virtual void Parse(XmlNode _parentNode)
	{
		foreach (object obj in _parentNode.ChildNodes)
		{
			XmlNode xmlNode = (XmlNode)obj;
			if (xmlNode.NodeType != XmlNodeType.Comment)
			{
				if (xmlNode.NodeType != XmlNodeType.Element)
				{
					Log.Warning("Unexpected XML node found in '" + this.SectionTypeName + "' section: " + xmlNode.OuterXml);
				}
				else
				{
					XmlElement childElement = (XmlElement)xmlNode;
					this.ParseElement(childElement);
				}
			}
		}
	}

	// Token: 0x0600021F RID: 543
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void ParseElement(XmlElement _childElement);

	// Token: 0x06000220 RID: 544
	public abstract void Save(XmlElement _root);

	// Token: 0x040002D1 RID: 721
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly AdminTools Parent;

	// Token: 0x040002D2 RID: 722
	public readonly string SectionTypeName;
}
