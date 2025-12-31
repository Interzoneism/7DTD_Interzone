using System;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;

// Token: 0x02000BD6 RID: 3030
public class WeatherSurvivalParametersFromXml
{
	// Token: 0x06005D45 RID: 23877 RVA: 0x0025D844 File Offset: 0x0025BA44
	public static void Load(XmlFile _xmlFile)
	{
		XElement root = _xmlFile.XmlDoc.Root;
		if (!root.HasElements)
		{
			throw new Exception("No element <weathersurvival> found!");
		}
		DynamicProperties dynamicProperties = new DynamicProperties();
		foreach (XElement propertyNode in root.Elements("property"))
		{
			dynamicProperties.Add(propertyNode, true, false);
		}
		WeatherManager.ClearTemperatureOffSetHeights();
		foreach (XElement element in root.Descendants("TemperatureHeight"))
		{
			float height = StringParsers.ParseFloat(element.GetAttribute("height"), 0, -1, NumberStyles.Any);
			float degreesOffset = StringParsers.ParseFloat(element.GetAttribute("addDegrees"), 0, -1, NumberStyles.Any);
			WeatherManager.AddTemperatureOffSetHeight(height, degreesOffset);
		}
		foreach (FieldInfo fieldInfo in typeof(WeatherParams).GetFields(BindingFlags.Static | BindingFlags.Public))
		{
			if (fieldInfo.DeclaringType == typeof(WeatherParams) && fieldInfo.FieldType == typeof(float) && dynamicProperties.Contains(fieldInfo.Name))
			{
				fieldInfo.SetValue(null, dynamicProperties.GetFloat(fieldInfo.Name));
			}
		}
	}
}
