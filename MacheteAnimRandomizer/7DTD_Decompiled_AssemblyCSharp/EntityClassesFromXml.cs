using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

// Token: 0x02000B9A RID: 2970
public class EntityClassesFromXml
{
	// Token: 0x06005BD7 RID: 23511 RVA: 0x0024D858 File Offset: 0x0024BA58
	public static IEnumerator LoadEntityClasses(XmlFile _xmlFile)
	{
		MicroStopwatch msw = new MicroStopwatch(true);
		EntityClass.list.Clear();
		EntityClassesFromXml.sEntityClassElements = new Dictionary<int, XElement>();
		EntityClass.sColors.Clear();
		XElement root = _xmlFile.XmlDoc.Root;
		if (!root.HasElements)
		{
			throw new Exception("No element <entity_classes> found!");
		}
		foreach (XElement xelement in root.Elements())
		{
			if (xelement.Name == "color")
			{
				XElement element = xelement;
				string attribute = element.GetAttribute("name");
				string attribute2 = element.GetAttribute("value");
				EntityClass.sColors.Add(attribute, StringParsers.ParseColor(attribute2));
			}
			else
			{
				if (xelement.Name == "replace_properties")
				{
					EntityClassesFromXml.sReplaceProperties.Clear();
					using (IEnumerator<XElement> enumerator2 = xelement.Elements().GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							XElement xelement2 = enumerator2.Current;
							if (xelement2.Name == "property")
							{
								EntityClassesFromXml.sReplaceProperties.Add("^" + xelement2.GetAttribute("name"), xelement2.GetAttribute("value"));
							}
						}
						goto IL_22F;
					}
				}
				if (xelement.Name == "replace_passive_effect")
				{
					EntityClassesFromXml.sReplacePassiveEffects.Clear();
					foreach (XElement xelement3 in xelement.Elements())
					{
						if (xelement3.Name == "property")
						{
							EntityClassesFromXml.sReplacePassiveEffects.Add("^" + xelement3.GetAttribute("name"), xelement3.GetAttribute("value"));
						}
					}
				}
			}
			IL_22F:
			if (xelement.Name == "entity_class")
			{
				XElement xelement4 = xelement;
				EntityClass entityClass = new EntityClass();
				string attribute3 = xelement4.GetAttribute("name");
				if (attribute3.Length == 0)
				{
					throw new Exception("Attribute 'name' missing on property in entity_class");
				}
				entityClass.entityClassName = attribute3;
				int num = EntityClass.FromString(entityClass.entityClassName);
				if (!EntityClassesFromXml.sEntityClassElements.TryAdd(num, xelement4))
				{
					string attribute4 = EntityClassesFromXml.sEntityClassElements[num].GetAttribute("name");
					throw new ArgumentException(string.Concat(new string[]
					{
						"Can not add entity '",
						attribute3,
						"' with conflicting hash to existing entity '",
						attribute4,
						"'"
					}));
				}
				string attribute5 = xelement4.GetAttribute("extends");
				if (attribute5.Length > 0)
				{
					int num2 = EntityClass.FromString(attribute5);
					if (!EntityClass.list.ContainsKey(num2))
					{
						throw new Exception("Did not find 'extends' entity '" + attribute5 + "'");
					}
					HashSet<string> hashSet = new HashSet<string>();
					if (xelement4.HasAttribute("ignore"))
					{
						foreach (string text in xelement4.GetAttribute("ignore").Split(',', StringSplitOptions.None))
						{
							hashSet.Add(text.Trim());
						}
					}
					hashSet.Add("HideInSpawnMenu");
					entityClass.CopyFrom(EntityClass.list[num2], hashSet);
					entityClass.Effects = MinEffectController.ParseXml(xelement4, EntityClassesFromXml.sEntityClassElements[num2], MinEffectController.SourceParentType.EntityClass, num);
				}
				else
				{
					entityClass.Effects = MinEffectController.ParseXml(xelement4, null, MinEffectController.SourceParentType.EntityClass, num);
				}
				foreach (XElement xelement5 in xelement4.Elements())
				{
					if (xelement5.Name == "property")
					{
						entityClass.Properties.Add(xelement5, true, true);
					}
					if (xelement5.Name == "drop")
					{
						XElement element2 = xelement5;
						int minCount = 1;
						int maxCount = 1;
						if (element2.HasAttribute("count"))
						{
							StringParsers.ParseMinMaxCount(element2.GetAttribute("count"), out minCount, out maxCount);
						}
						float prob = 1f;
						if (element2.HasAttribute("prob"))
						{
							prob = StringParsers.ParseFloat(element2.GetAttribute("prob"), 0, -1, NumberStyles.Any);
						}
						string attribute6 = element2.GetAttribute("name");
						EnumDropEvent eEvent = EnumDropEvent.Destroy;
						if (element2.HasAttribute("event"))
						{
							eEvent = EnumUtils.Parse<EnumDropEvent>(element2.GetAttribute("event"), false);
						}
						float stickChance = 0f;
						if (element2.HasAttribute("stick_chance"))
						{
							stickChance = StringParsers.ParseFloat(element2.GetAttribute("stick_chance"), 0, -1, NumberStyles.Any);
						}
						string toolCategory = null;
						if (element2.HasAttribute("tool_category"))
						{
							toolCategory = element2.GetAttribute("tool_category");
						}
						string tag = "";
						if (element2.HasAttribute("tag"))
						{
							tag = element2.GetAttribute("tag");
						}
						entityClass.AddDroppedId(eEvent, attribute6, minCount, maxCount, prob, stickChance, toolCategory, tag);
					}
				}
				EntityClass.list[num] = entityClass;
				entityClass.Init();
			}
			if (msw.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
			{
				yield return null;
				msw.ResetAndRestart();
			}
		}
		IEnumerator<XElement> enumerator = null;
		EntityClassesFromXml.sEntityClassElements.Clear();
		EntityClassesFromXml.sEntityClassElements = null;
		yield break;
		yield break;
	}

	// Token: 0x06005BD8 RID: 23512 RVA: 0x0024D867 File Offset: 0x0024BA67
	public static string ReplaceProperty(string _value)
	{
		if (_value.Length > 0 && _value[0] == '^')
		{
			_value = EntityClassesFromXml.sReplaceProperties[_value];
		}
		return _value;
	}

	// Token: 0x0400460F RID: 17935
	public const char cReplaceChar = '^';

	// Token: 0x04004610 RID: 17936
	public static Dictionary<string, string> sReplaceProperties = new Dictionary<string, string>();

	// Token: 0x04004611 RID: 17937
	public static Dictionary<string, string> sReplacePassiveEffects = new Dictionary<string, string>();

	// Token: 0x04004612 RID: 17938
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<int, XElement> sEntityClassElements;
}
