using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

// Token: 0x02000B8E RID: 2958
public class BlockTexturesFromXML
{
	// Token: 0x06005B93 RID: 23443 RVA: 0x0024B784 File Offset: 0x00249984
	public static IEnumerator CreateBlockTextures(XmlFile _xmlFile)
	{
		BlockTextureData.InitStatic();
		XElement root = _xmlFile.XmlDoc.Root;
		if (!root.HasElements)
		{
			throw new Exception("No element <block_textures> found!");
		}
		using (IEnumerator<XElement> enumerator = root.Elements("paint").GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				XElement xelement = enumerator.Current;
				DynamicProperties dynamicProperties = new DynamicProperties();
				foreach (XElement propertyNode in xelement.Elements("property"))
				{
					dynamicProperties.Add(propertyNode, true, false);
				}
				BlockTextureData blockTextureData = new BlockTextureData();
				blockTextureData.Name = xelement.GetAttribute("name");
				blockTextureData.LocalizedName = Localization.Get(blockTextureData.Name, false);
				blockTextureData.ID = int.Parse(xelement.GetAttribute("id"));
				if (dynamicProperties.Values.ContainsKey("Group"))
				{
					blockTextureData.Group = dynamicProperties.Values["Group"];
				}
				if (dynamicProperties.Values.ContainsKey("PaintCost"))
				{
					blockTextureData.PaintCost = Convert.ToUInt16(dynamicProperties.Values["PaintCost"]);
				}
				else
				{
					blockTextureData.PaintCost = 1;
				}
				if (dynamicProperties.Values.ContainsKey("TextureId"))
				{
					blockTextureData.TextureID = Convert.ToUInt16(dynamicProperties.Values["TextureId"]);
				}
				if (dynamicProperties.Values.ContainsKey("Hidden"))
				{
					blockTextureData.Hidden = Convert.ToBoolean(dynamicProperties.Values["Hidden"]);
				}
				if (dynamicProperties.Values.ContainsKey("SortIndex"))
				{
					blockTextureData.SortIndex = Convert.ToByte(dynamicProperties.Values["SortIndex"]);
				}
				blockTextureData.Init();
			}
			yield break;
		}
		yield break;
	}
}
