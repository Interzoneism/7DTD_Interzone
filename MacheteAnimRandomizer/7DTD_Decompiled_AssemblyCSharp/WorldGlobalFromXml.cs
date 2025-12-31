using System;
using System.Collections;
using System.Xml.Linq;

// Token: 0x02000BD7 RID: 3031
public class WorldGlobalFromXml
{
	// Token: 0x06005D47 RID: 23879 RVA: 0x0025D9D0 File Offset: 0x0025BBD0
	public static IEnumerator Load(XmlFile _xmlFile)
	{
		XElement root = _xmlFile.XmlDoc.Root;
		if (!root.HasElements)
		{
			throw new Exception("No element <world> found!");
		}
		foreach (XContainer xcontainer in root.Elements("environment"))
		{
			DynamicProperties dynamicProperties = new DynamicProperties();
			foreach (XElement propertyNode in xcontainer.Elements("property"))
			{
				dynamicProperties.Add(propertyNode, true, false);
			}
			WorldEnvironment.Properties = dynamicProperties;
		}
		WorldEnvironment.OnXMLChanged();
		yield break;
	}

	// Token: 0x06005D48 RID: 23880 RVA: 0x0025D9DF File Offset: 0x0025BBDF
	public static void Reload(XmlFile xmlFile)
	{
		ThreadManager.RunCoroutineSync(WorldGlobalFromXml.Load(xmlFile));
	}
}
