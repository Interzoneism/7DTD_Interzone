using System;
using System.Collections;
using System.Xml;
using System.Xml.Linq;

// Token: 0x02000BB9 RID: 3001
public class PhysicsBodiesFromXml
{
	// Token: 0x06005C84 RID: 23684 RVA: 0x00255B8D File Offset: 0x00253D8D
	public static IEnumerator Load(XmlFile xmlFile)
	{
		try
		{
			XElement root = xmlFile.XmlDoc.Root;
			if (!root.HasElements)
			{
				yield break;
			}
			foreach (XElement e in root.Elements("body"))
			{
				PhysicsBodyLayout.Read(e);
			}
			yield break;
		}
		catch (Exception)
		{
			PhysicsBodyLayout.Reset();
			throw;
		}
		yield break;
	}

	// Token: 0x06005C85 RID: 23685 RVA: 0x00255B9C File Offset: 0x00253D9C
	public static void Save(string path)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.CreateXmlDeclaration();
		XmlElement elem = xmlDocument.AddXmlElement("bodies");
		PhysicsBodyLayout[] bodyLayouts = PhysicsBodyLayout.BodyLayouts;
		for (int i = 0; i < bodyLayouts.Length; i++)
		{
			bodyLayouts[i].Write(elem);
		}
		xmlDocument.SdSave(path);
	}
}
