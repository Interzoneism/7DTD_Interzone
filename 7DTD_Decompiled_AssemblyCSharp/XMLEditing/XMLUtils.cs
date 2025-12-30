using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace XMLEditing
{
	// Token: 0x020013C3 RID: 5059
	public static class XMLUtils
	{
		// Token: 0x1700110E RID: 4366
		// (get) Token: 0x06009E3B RID: 40507 RVA: 0x003EF2F3 File Offset: 0x003ED4F3
		public static string DefaultBlocksFilePath
		{
			get
			{
				return GameIO.GetGameDir("Data/Config") + "/blocks.xml";
			}
		}

		// Token: 0x06009E3C RID: 40508 RVA: 0x003EF30C File Offset: 0x003ED50C
		public static XDocument LoadXDocument(string filePath)
		{
			XDocument result;
			using (Stream stream = SdFile.OpenRead(filePath))
			{
				result = XDocument.Load(stream, LoadOptions.PreserveWhitespace);
			}
			return result;
		}

		// Token: 0x06009E3D RID: 40509 RVA: 0x003EF348 File Offset: 0x003ED548
		public static XElement SetProperty(XElement _element, string _propertyName, XName _attribName, string _value)
		{
			XElement xelement = (from e in _element.Elements(XNames.property)
			where e.GetAttribute(XNames.name) == _propertyName
			select e).FirstOrDefault<XElement>();
			if (xelement == null)
			{
				xelement = new XElement(XNames.property, new XAttribute(XNames.name, _propertyName));
				_element.Add("\t");
				_element.Add(xelement);
				_element.Add("\r\n");
				_element.Add("\t");
			}
			xelement.SetAttributeValue(_attribName, _value);
			return xelement;
		}

		// Token: 0x06009E3E RID: 40510 RVA: 0x003EF3D4 File Offset: 0x003ED5D4
		public static void SaveXDocument(XDocument doc, string filePath, bool omitXmlDeclaration = false)
		{
			using (Stream stream = SdFile.Create(filePath))
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings
				{
					Encoding = Encoding.UTF8,
					Indent = true,
					IndentChars = "\t",
					NewLineChars = "\r\n",
					NewLineHandling = NewLineHandling.Replace,
					OmitXmlDeclaration = omitXmlDeclaration
				}))
				{
					doc.WriteTo(xmlWriter);
				}
			}
		}

		// Token: 0x06009E3F RID: 40511 RVA: 0x003EF468 File Offset: 0x003ED668
		public static void CleanAndRepairBlocksXML()
		{
			string text = SdFile.ReadAllText(XMLUtils.DefaultBlocksFilePath);
			string pattern = "<block [^>]*>[\\s\\S]*?</block>";
			text = Regex.Replace(text, pattern, (Match m) => Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(m.Value, "\r\n\\s*\r\n", "\r\n"), "/>\r\n\t(\t)?(<property name=\"(ModelOffset|OversizedBounds)\")", "/>\r\n\t$2"), "\r\n\t</block>", "\r\n</block>"), "\r\n\t <!--[\\s\\S]*?-->\r\n", "\r\n"), "(name=\"MeshDamage\" value=\")([^\"]+)\"", delegate(Match match)
			{
				string str = Regex.Replace(match.Groups[2].Value, " {2,3}", "\r\n\t\t");
				return match.Groups[1].Value + str + "\"";
			}), " />", "/>"), RegexOptions.Singleline);
			SdFile.WriteAllText(XMLUtils.DefaultBlocksFilePath, text);
		}

		// Token: 0x06009E40 RID: 40512 RVA: 0x003EF4BC File Offset: 0x003ED6BC
		public static HashSet<string> ParseStringList(string targetListString, char separator)
		{
			string[] array = targetListString.Split(new char[]
			{
				separator
			}, StringSplitOptions.RemoveEmptyEntries);
			HashSet<string> hashSet = new HashSet<string>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string item = array2[i].Trim();
				hashSet.Add(item);
			}
			return hashSet;
		}

		// Token: 0x06009E41 RID: 40513 RVA: 0x003EF504 File Offset: 0x003ED704
		public static HashSet<string> GetReplacementBlockNames(HashSet<string> targetNames)
		{
			XElement root = XMLUtils.LoadXDocument(GameIO.GetGameDir("Data/Config") + "/blockplaceholders.xml").Root;
			if (root == null || !root.HasElements)
			{
				throw new Exception("No element <blockplaceholders> found!");
			}
			Dictionary<string, XElement> dictionary = new CaseInsensitiveStringDictionary<XElement>();
			foreach (XElement xelement in root.Elements("placeholder"))
			{
				string attribute = xelement.GetAttribute(XNames.name);
				dictionary[attribute] = xelement;
			}
			HashSet<string> hashSet = new HashSet<string>();
			foreach (string text in targetNames)
			{
				string key = text.Trim();
				XElement xelement2;
				if (dictionary.TryGetValue(key, out xelement2))
				{
					foreach (XElement element in xelement2.Elements(XNames.block))
					{
						hashSet.Add(element.GetAttribute(XNames.name).Trim());
					}
				}
			}
			return hashSet;
		}

		// Token: 0x06009E42 RID: 40514 RVA: 0x003EF64C File Offset: 0x003ED84C
		public static bool AllAttributesAreEqual(XElement elementA, XElement elementB, StringComparison comparisonType)
		{
			if (elementB.Attributes().Count<XAttribute>() != elementA.Attributes().Count<XAttribute>())
			{
				return false;
			}
			foreach (XAttribute xattribute in elementA.Attributes())
			{
				XAttribute xattribute2 = elementB.Attribute(xattribute.Name);
				if (xattribute2 == null)
				{
					return false;
				}
				string a = xattribute.Value.Trim();
				string b = xattribute2.Value.Trim();
				if (!string.Equals(a, b, comparisonType))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06009E43 RID: 40515 RVA: 0x003EF6EC File Offset: 0x003ED8EC
		public static void PopulateReplacementMap(Dictionary<string, HashSet<string>> replacementMap)
		{
			replacementMap.Clear();
			XElement root = XMLUtils.LoadXDocument(GameIO.GetGameDir("Data/Config") + "/blockplaceholders.xml").Root;
			if (root == null || !root.HasElements)
			{
				throw new Exception("No element <blockplaceholders> found!");
			}
			foreach (XElement xelement in root.Elements("placeholder"))
			{
				string key = xelement.GetAttribute(XNames.name).Trim();
				HashSet<string> hashSet = new HashSet<string>();
				replacementMap[key] = hashSet;
				foreach (XElement element in xelement.Elements(XNames.block))
				{
					hashSet.Add(element.GetAttribute(XNames.name).Trim());
				}
			}
		}

		// Token: 0x040079F4 RID: 31220
		public const string IndentChars = "\t";

		// Token: 0x040079F5 RID: 31221
		public const string NewLineChars = "\r\n";
	}
}
