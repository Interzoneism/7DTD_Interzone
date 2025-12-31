using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

// Token: 0x02000BBB RID: 3003
public class QualityInfoFromXml : MonoBehaviour
{
	// Token: 0x06005C8D RID: 23693 RVA: 0x00255C9C File Offset: 0x00253E9C
	public static IEnumerator CreateQualityInfo(XmlFile _xmlFile)
	{
		XElement root = _xmlFile.XmlDoc.Root;
		if (!root.HasElements)
		{
			throw new Exception("No element <quality> found!");
		}
		using (IEnumerator<XElement> enumerator = root.Elements("quality").GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				XElement element = enumerator.Current;
				int num = -1;
				if (element.HasAttribute("key"))
				{
					num = int.Parse(element.GetAttribute("key"));
				}
				string hexColor = "#FFFFFF";
				if (element.HasAttribute("color"))
				{
					hexColor = element.GetAttribute("color");
				}
				if (num > -1)
				{
					QualityInfo.Add(num, hexColor);
				}
			}
			yield break;
		}
		yield break;
	}
}
