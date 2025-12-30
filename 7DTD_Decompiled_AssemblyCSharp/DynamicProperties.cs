using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using UnityEngine;

// Token: 0x02001180 RID: 4480
[MemoryPackable(GenerateType.Object)]
public class DynamicProperties : IMemoryPackable<DynamicProperties>, IMemoryPackFormatterRegister
{
	// Token: 0x06008BDF RID: 35807 RVA: 0x0038574C File Offset: 0x0038394C
	public DictionarySave<string, string> ParseKeyData(string key)
	{
		try
		{
			string data;
			if (this.Data.TryGetValue(key, out data))
			{
				return DynamicProperties.ParseData(data);
			}
		}
		catch (Exception ex)
		{
			Log.Error("ParseKeyData error parsing key {0}, {1}", new object[]
			{
				key,
				ex
			});
		}
		return null;
	}

	// Token: 0x06008BE0 RID: 35808 RVA: 0x003857A4 File Offset: 0x003839A4
	public static DictionarySave<string, string> ParseData(string data)
	{
		DictionarySave<string, string> dictionarySave = null;
		try
		{
			dictionarySave = new DictionarySave<string, string>();
			if (data.IndexOf(';') < 0)
			{
				string[] array = data.Split(DynamicProperties.equalSeparator, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length >= 2)
				{
					dictionarySave[array[0]] = array[1];
				}
			}
			else
			{
				string[] array2 = data.Split(DynamicProperties.semicolonSeparator, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < array2.Length; i++)
				{
					string[] array3 = array2[i].Split(DynamicProperties.equalSeparator, StringSplitOptions.RemoveEmptyEntries);
					if (array3.Length >= 2)
					{
						dictionarySave[array3[0]] = array3[1];
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.Error("ParseData error parsing {0}, {1}", new object[]
			{
				data,
				ex
			});
		}
		return dictionarySave;
	}

	// Token: 0x06008BE1 RID: 35809 RVA: 0x00385858 File Offset: 0x00383A58
	public bool Load(string _directory, string _name, bool _addClassesToMain = true)
	{
		try
		{
			foreach (XElement propertyNode in new XmlFile(_directory, _name, false, false).XmlDoc.Root.Elements(XNames.property))
			{
				this.Add(propertyNode, _addClassesToMain, false);
			}
			return true;
		}
		catch (Exception e)
		{
			Log.Exception(e);
		}
		return false;
	}

	// Token: 0x06008BE2 RID: 35810 RVA: 0x003858D8 File Offset: 0x00383AD8
	public bool Save(string _rootNodeName, Stream stream)
	{
		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
			xmlDocument.InsertBefore(newChild, xmlDocument.DocumentElement);
			XmlNode parent = xmlDocument.AppendChild(xmlDocument.CreateElement(_rootNodeName));
			this.toXml(xmlDocument, parent);
			xmlDocument.Save(stream);
			return true;
		}
		catch (Exception e)
		{
			Log.Exception(e);
		}
		return false;
	}

	// Token: 0x06008BE3 RID: 35811 RVA: 0x00385948 File Offset: 0x00383B48
	public bool Save(string _rootNodeName, string _path, string _name)
	{
		try
		{
			using (Stream stream = SdFile.Create(Path.Join(_path, _name + ".xml")))
			{
				return this.Save(_rootNodeName, stream);
			}
		}
		catch (Exception e)
		{
			Log.Exception(e);
		}
		return false;
	}

	// Token: 0x06008BE4 RID: 35812 RVA: 0x003859B4 File Offset: 0x00383BB4
	[PublicizedFrom(EAccessModifier.Private)]
	public void toXml(XmlDocument _doc, XmlNode _parent)
	{
		foreach (KeyValuePair<string, string> keyValuePair in this.Values.Dict)
		{
			XmlElement xmlElement = _doc.CreateElement("property");
			XmlAttribute xmlAttribute = _doc.CreateAttribute("name");
			xmlAttribute.Value = keyValuePair.Key;
			xmlElement.Attributes.Append(xmlAttribute);
			XmlAttribute xmlAttribute2 = _doc.CreateAttribute("value");
			xmlAttribute2.Value = this.Values[keyValuePair.Key];
			xmlElement.Attributes.Append(xmlAttribute2);
			if (this.Params1.ContainsKey(keyValuePair.Key))
			{
				XmlAttribute xmlAttribute3 = _doc.CreateAttribute("param1");
				xmlAttribute3.Value = this.Params1[keyValuePair.Key];
				xmlElement.Attributes.Append(xmlAttribute3);
			}
			if (this.Data.ContainsKey(keyValuePair.Key))
			{
				XmlAttribute xmlAttribute4 = _doc.CreateAttribute("fields");
				xmlAttribute4.Value = this.Data[keyValuePair.Key];
				xmlElement.Attributes.Append(xmlAttribute4);
			}
			_parent.AppendChild(xmlElement);
		}
		if (this.Classes.Count > 0)
		{
			foreach (KeyValuePair<string, DynamicProperties> keyValuePair2 in this.Classes.Dict)
			{
				XmlElement xmlElement2 = _doc.CreateElement("property");
				XmlAttribute xmlAttribute5 = _doc.CreateAttribute("class");
				xmlAttribute5.Value = keyValuePair2.Key;
				xmlElement2.Attributes.Append(xmlAttribute5);
				keyValuePair2.Value.toXml(_doc, xmlElement2);
				_parent.AppendChild(xmlElement2);
			}
		}
	}

	// Token: 0x06008BE5 RID: 35813 RVA: 0x00385BC4 File Offset: 0x00383DC4
	public void Add(XElement _propertyNode, bool _addClassesToMain = true, bool _doValueReplace = false)
	{
		this.Parse(null, _propertyNode, this, _addClassesToMain, _doValueReplace);
	}

	// Token: 0x06008BE6 RID: 35814 RVA: 0x00385BD4 File Offset: 0x00383DD4
	public void Parse(string _className, XElement elementProperty, DynamicProperties _mainProperties, bool _addClassesToMain, bool _doValueReplace = false)
	{
		if (elementProperty.HasAttribute(XNames.class_))
		{
			string attribute = elementProperty.GetAttribute(XNames.class_);
			string text = (_className != null) ? (_className + "." + attribute) : attribute;
			DynamicProperties dynamicProperties;
			if (!this.Classes.TryGetValue(attribute, out dynamicProperties))
			{
				dynamicProperties = new DynamicProperties();
				this.Classes.Add(attribute, dynamicProperties);
				if (_addClassesToMain && _mainProperties != this)
				{
					_mainProperties.Classes[text] = dynamicProperties;
				}
			}
			using (IEnumerator<XElement> enumerator = elementProperty.Elements().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					XElement elementProperty2 = enumerator.Current;
					dynamicProperties.Parse(text, elementProperty2, _mainProperties, _addClassesToMain, false);
				}
				return;
			}
		}
		string attribute2 = elementProperty.GetAttribute(XNames.name);
		if (attribute2.Length == 0)
		{
			throw new Exception("Attribute 'name' missing on property");
		}
		string text2 = elementProperty.GetAttribute(XNames.value);
		if (_doValueReplace)
		{
			text2 = EntityClassesFromXml.ReplaceProperty(text2);
		}
		string text3 = null;
		if (elementProperty.HasAttribute(XNames.param1))
		{
			text3 = elementProperty.GetAttribute(XNames.param1);
		}
		string text4 = null;
		if (elementProperty.HasAttribute(XNames.param2))
		{
			text4 = elementProperty.GetAttribute(XNames.param2);
		}
		string text5 = (_className != null) ? (_className + "." + attribute2) : attribute2;
		if (text3 != null)
		{
			this.Params1[attribute2] = text3;
			if (_addClassesToMain && _mainProperties != this)
			{
				_mainProperties.Params1[text5] = text3;
			}
		}
		if (text4 != null)
		{
			this.Params2[attribute2] = text4;
			if (_addClassesToMain && _mainProperties != this)
			{
				_mainProperties.Params2[text5] = text4;
			}
		}
		if (text2 != null)
		{
			string attribute3 = elementProperty.GetAttribute(XNames.data);
			if (attribute3.Length > 0)
			{
				this.Data[attribute2] = attribute3;
				if (_addClassesToMain && _mainProperties != this)
				{
					_mainProperties.Data[text5] = attribute3;
				}
			}
		}
		this.Values[attribute2] = text2;
		if (_addClassesToMain && _mainProperties != this)
		{
			_mainProperties.Values[text5] = text2;
		}
		if (elementProperty.HasAttribute(XNames.display) && StringParsers.ParseBool(elementProperty.GetAttribute(XNames.display), 0, -1, true))
		{
			this.Display.Add(attribute2);
			if (_addClassesToMain && _mainProperties != this)
			{
				_mainProperties.Display.Add(text5);
			}
		}
	}

	// Token: 0x06008BE7 RID: 35815 RVA: 0x00385E20 File Offset: 0x00384020
	public void Clear()
	{
		this.Values.Clear();
		this.Params1.Clear();
		this.Params2.Clear();
	}

	// Token: 0x06008BE8 RID: 35816 RVA: 0x00385E43 File Offset: 0x00384043
	public bool Contains(string _propName)
	{
		return this.Values.ContainsKey(_propName);
	}

	// Token: 0x06008BE9 RID: 35817 RVA: 0x00385E54 File Offset: 0x00384054
	public bool Contains(string _className, string _propName)
	{
		DynamicProperties dynamicProperties;
		return this.Classes.TryGetValue(_className, out dynamicProperties) && dynamicProperties.Contains(_propName);
	}

	// Token: 0x06008BEA RID: 35818 RVA: 0x00385E7C File Offset: 0x0038407C
	public bool GetBool(string _propName)
	{
		bool result;
		StringParsers.TryParseBool(this.Values[_propName], out result, 0, -1, true);
		return result;
	}

	// Token: 0x06008BEB RID: 35819 RVA: 0x00385EA4 File Offset: 0x003840A4
	public float GetFloat(string _propName)
	{
		float result;
		StringParsers.TryParseFloat(this.Values[_propName], out result, 0, -1, NumberStyles.Any);
		return result;
	}

	// Token: 0x06008BEC RID: 35820 RVA: 0x00385ED0 File Offset: 0x003840D0
	public int GetInt(string _propName)
	{
		int result;
		int.TryParse(this.Values[_propName], out result);
		return result;
	}

	// Token: 0x06008BED RID: 35821 RVA: 0x00385EF2 File Offset: 0x003840F2
	public string GetStringValue(string _propName)
	{
		return this.Values[_propName].ToString();
	}

	// Token: 0x06008BEE RID: 35822 RVA: 0x00385F08 File Offset: 0x00384108
	public string GetString(string _propName)
	{
		string result;
		if (this.Values.TryGetValue(_propName, out result))
		{
			return result;
		}
		return string.Empty;
	}

	// Token: 0x06008BEF RID: 35823 RVA: 0x00385F2C File Offset: 0x0038412C
	public string GetString(string _className, string _propName)
	{
		DynamicProperties dynamicProperties;
		if (this.Classes.TryGetValue(_className, out dynamicProperties))
		{
			return dynamicProperties.GetString(_propName);
		}
		return string.Empty;
	}

	// Token: 0x06008BF0 RID: 35824 RVA: 0x00385F56 File Offset: 0x00384156
	public void SetValue(string _propName, string _value)
	{
		if (this.Values.ContainsKey(_propName))
		{
			this.Values[_propName] = _value;
			return;
		}
		this.Values.Add(_propName, _value);
	}

	// Token: 0x06008BF1 RID: 35825 RVA: 0x00385F84 File Offset: 0x00384184
	public void SetValue(string _className, string _propName, string _value)
	{
		DynamicProperties dynamicProperties;
		if (!this.Classes.TryGetValue(_className, out dynamicProperties))
		{
			dynamicProperties = new DynamicProperties();
			this.Classes[_className] = dynamicProperties;
		}
		dynamicProperties.SetValue(_propName, _value);
		string propName = string.Format("{0}.{1}", _className, _propName);
		this.SetValue(propName, _value);
	}

	// Token: 0x06008BF2 RID: 35826 RVA: 0x00385FD4 File Offset: 0x003841D4
	public void SetParam1(string _propName, string _param1)
	{
		if (!this.Values.ContainsKey(_propName))
		{
			this.Values.Add(_propName, null);
		}
		if (this.Params1.ContainsKey(_propName))
		{
			this.Params1[_propName] = _param1;
			return;
		}
		this.Params1.Add(_propName, _param1);
	}

	// Token: 0x06008BF3 RID: 35827 RVA: 0x00386028 File Offset: 0x00384228
	public void ParseString(string _propName, ref string optionalValue)
	{
		string text;
		if (this.Values.TryGetValue(_propName, out text))
		{
			optionalValue = text;
		}
	}

	// Token: 0x06008BF4 RID: 35828 RVA: 0x00386048 File Offset: 0x00384248
	public string GetLocalizedString(string _propName)
	{
		string key;
		if (this.Values.TryGetValue(_propName, out key))
		{
			return Localization.Get(key, false);
		}
		return string.Empty;
	}

	// Token: 0x06008BF5 RID: 35829 RVA: 0x00386074 File Offset: 0x00384274
	public void ParseLocalizedString(string _propName, ref string optionalValue)
	{
		string key;
		if (this.Values.TryGetValue(_propName, out key))
		{
			optionalValue = Localization.Get(key, false);
		}
	}

	// Token: 0x06008BF6 RID: 35830 RVA: 0x0038609C File Offset: 0x0038429C
	public void ParseBool(string _propName, ref bool optionalValue)
	{
		string text;
		if (this.Values.TryGetValue(_propName, out text))
		{
			bool flag;
			if (StringParsers.TryParseBool(text, out flag, 0, -1, true))
			{
				optionalValue = flag;
				return;
			}
			Log.Warning("Can't parse bool {0} '{1}'", new object[]
			{
				_propName,
				text
			});
		}
	}

	// Token: 0x06008BF7 RID: 35831 RVA: 0x003860E4 File Offset: 0x003842E4
	public void ParseColor(string _propName, ref Color optionalValue)
	{
		string input;
		if (this.Values.TryGetValue(_propName, out input))
		{
			optionalValue = StringParsers.ParseColor32(input);
		}
	}

	// Token: 0x06008BF8 RID: 35832 RVA: 0x00386110 File Offset: 0x00384310
	public void ParseColorHex(string _propName, ref Color optionalValue)
	{
		string input;
		if (this.Values.TryGetValue(_propName, out input))
		{
			optionalValue = StringParsers.ParseHexColor(input);
		}
	}

	// Token: 0x06008BF9 RID: 35833 RVA: 0x0038613C File Offset: 0x0038433C
	public void ParseEnum<T>(string _propName, ref T optionalValue) where T : struct, IConvertible
	{
		string name;
		T t;
		if (this.Values.TryGetValue(_propName, out name) && EnumUtils.TryParse<T>(name, out t, true))
		{
			optionalValue = t;
		}
	}

	// Token: 0x06008BFA RID: 35834 RVA: 0x0038616C File Offset: 0x0038436C
	public void ParseFloat(string _propName, ref float optionalValue)
	{
		string text;
		if (this.Values.TryGetValue(_propName, out text))
		{
			float num;
			if (StringParsers.TryParseFloat(text, out num, 0, -1, NumberStyles.Any))
			{
				optionalValue = num;
				return;
			}
			Log.Warning("Can't parse float {0} '{1}'", new object[]
			{
				_propName,
				text
			});
		}
	}

	// Token: 0x06008BFB RID: 35835 RVA: 0x003861B8 File Offset: 0x003843B8
	public static void ParseFloat(XElement _e, string _propName, ref float optionalValue)
	{
		XAttribute xattribute = _e.Attribute(_propName);
		if (xattribute != null)
		{
			string value = xattribute.Value;
			float num;
			if (StringParsers.TryParseFloat(value, out num, 0, -1, NumberStyles.Any))
			{
				optionalValue = num;
				return;
			}
			Log.Warning("Can't parse float {0} '{1}'", new object[]
			{
				_propName,
				value
			});
		}
	}

	// Token: 0x06008BFC RID: 35836 RVA: 0x0038620C File Offset: 0x0038440C
	public void ParseInt(string _propName, ref int optionalValue)
	{
		string text;
		if (this.Values.TryGetValue(_propName, out text))
		{
			int num;
			if (StringParsers.TryParseSInt32(text, out num, 0, -1, NumberStyles.Integer))
			{
				optionalValue = num;
				return;
			}
			Log.Warning("Can't parse int {0} '{1}'", new object[]
			{
				_propName,
				text
			});
		}
	}

	// Token: 0x06008BFD RID: 35837 RVA: 0x00386254 File Offset: 0x00384454
	public void ParseVec(string _propName, ref Vector2 optionalValue)
	{
		string input;
		if (this.Values.TryGetValue(_propName, out input))
		{
			optionalValue = StringParsers.ParseVector2(input);
		}
	}

	// Token: 0x06008BFE RID: 35838 RVA: 0x00386280 File Offset: 0x00384480
	public void ParseVec(string _propName, ref Vector2 optionalValue, float _defaultValue)
	{
		string input;
		if (this.Values.TryGetValue(_propName, out input))
		{
			optionalValue = StringParsers.ParseVector2(input, _defaultValue);
		}
	}

	// Token: 0x06008BFF RID: 35839 RVA: 0x003862AC File Offset: 0x003844AC
	public void ParseVec(string _propName, ref Vector3 optionalValue)
	{
		string input;
		if (this.Values.TryGetValue(_propName, out input))
		{
			optionalValue = StringParsers.ParseVector3(input, 0, -1);
		}
	}

	// Token: 0x06008C00 RID: 35840 RVA: 0x003862D8 File Offset: 0x003844D8
	public void ParseVec(string _propName, ref Vector3 optionalValue, float _defaultValue)
	{
		string input;
		if (this.Values.TryGetValue(_propName, out input))
		{
			optionalValue = StringParsers.ParseVector3(input, _defaultValue);
		}
	}

	// Token: 0x06008C01 RID: 35841 RVA: 0x00386304 File Offset: 0x00384504
	public void ParseVec(string _propName, ref Vector3i optionalValue)
	{
		string input;
		if (this.Values.TryGetValue(_propName, out input))
		{
			optionalValue = StringParsers.ParseVector3i(input, 0, -1, false);
		}
	}

	// Token: 0x06008C02 RID: 35842 RVA: 0x00386330 File Offset: 0x00384530
	public void ParseVec(string _propName, ref float optionalValue1, ref float optionalValue2)
	{
		string input;
		if (this.Values.TryGetValue(_propName, out input))
		{
			Vector2 vector = StringParsers.ParseVector2(input);
			optionalValue1 = vector.x;
			optionalValue2 = vector.y;
		}
	}

	// Token: 0x06008C03 RID: 35843 RVA: 0x00386364 File Offset: 0x00384564
	public void ParseVec(string _propName, ref float optionalValue1, ref float optionalValue2, ref float optionalValue3)
	{
		string input;
		if (this.Values.TryGetValue(_propName, out input))
		{
			Vector3 vector = StringParsers.ParseVector3(input, 0, -1);
			optionalValue1 = vector.x;
			optionalValue2 = vector.y;
			optionalValue3 = vector.z;
		}
	}

	// Token: 0x06008C04 RID: 35844 RVA: 0x003863A4 File Offset: 0x003845A4
	public void ParseVec(string _propName, ref float optionalValue1, ref float optionalValue2, ref float optionalValue3, ref float optionalValue4)
	{
		string input;
		if (this.Values.TryGetValue(_propName, out input))
		{
			Vector4 vector = StringParsers.ParseVector4(input);
			optionalValue1 = vector.x;
			optionalValue2 = vector.y;
			optionalValue3 = vector.z;
			optionalValue4 = vector.w;
		}
	}

	// Token: 0x06008C05 RID: 35845 RVA: 0x003863EC File Offset: 0x003845EC
	public void CopyFrom(DynamicProperties _other, HashSet<string> _exclude = null)
	{
		DynamicProperties.copyDict(_other.Values, this.Values, _exclude);
		DynamicProperties.copyDict(_other.Params1, this.Params1, _exclude);
		DynamicProperties.copyDict(_other.Params2, this.Params2, _exclude);
		DynamicProperties.copyDict(_other.Data, this.Data, _exclude);
		foreach (string text in _other.Display)
		{
			if (DynamicProperties.copyKey(text, _exclude))
			{
				this.Display.Add(text);
			}
		}
		foreach (KeyValuePair<string, DynamicProperties> keyValuePair in _other.Classes.Dict)
		{
			if (DynamicProperties.copyKey(keyValuePair.Key, _exclude))
			{
				DynamicProperties dynamicProperties = this.Classes.ContainsKey(keyValuePair.Key) ? this.Classes[keyValuePair.Key] : new DynamicProperties();
				this.Classes[keyValuePair.Key] = dynamicProperties;
				dynamicProperties.CopyFrom(keyValuePair.Value, null);
			}
		}
	}

	// Token: 0x06008C06 RID: 35846 RVA: 0x00386538 File Offset: 0x00384738
	[PublicizedFrom(EAccessModifier.Private)]
	public static void copyDict(DictionarySave<string, string> _source, DictionarySave<string, string> _dest, HashSet<string> _exclude)
	{
		foreach (KeyValuePair<string, string> keyValuePair in _source.Dict)
		{
			if (DynamicProperties.copyKey(keyValuePair.Key, _exclude))
			{
				_dest[keyValuePair.Key] = keyValuePair.Value;
			}
		}
	}

	// Token: 0x06008C07 RID: 35847 RVA: 0x003865A8 File Offset: 0x003847A8
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool copyKey(string _key, HashSet<string> _exclude)
	{
		if (_exclude == null)
		{
			return true;
		}
		if (_exclude.Contains(_key))
		{
			return false;
		}
		if (_key.IndexOf('.') <= 0)
		{
			return true;
		}
		foreach (string str in _exclude)
		{
			if (_key.StartsWith(str + "."))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06008C08 RID: 35848 RVA: 0x00386628 File Offset: 0x00384828
	public string PrettyPrint()
	{
		StringBuilder stringBuilder = new StringBuilder();
		this.PrettyPrint(stringBuilder, "");
		return stringBuilder.ToString();
	}

	// Token: 0x06008C09 RID: 35849 RVA: 0x00386650 File Offset: 0x00384850
	[PublicizedFrom(EAccessModifier.Private)]
	public void PrettyPrint(StringBuilder sb, string indent)
	{
		sb.AppendFormat("{0}Properties:\n", indent);
		foreach (KeyValuePair<string, string> keyValuePair in from kvp in this.Values.Dict
		orderby kvp.Key
		select kvp)
		{
			sb.AppendFormat("{2}    name={0}, value={1}", keyValuePair.Key, this.Values[keyValuePair.Key], indent);
			if (this.Params1.ContainsKey(keyValuePair.Key))
			{
				sb.AppendFormat(", param1={0}", this.Params1[keyValuePair.Key]);
			}
			if (this.Data.ContainsKey(keyValuePair.Key))
			{
				sb.AppendFormat(", fields={0}", this.Params1[keyValuePair.Key]);
			}
			sb.AppendLine();
		}
		if (this.Classes.Count > 0)
		{
			sb.AppendFormat("{0}Classes:\n", indent);
			foreach (KeyValuePair<string, DynamicProperties> keyValuePair2 in this.Classes.Dict)
			{
				sb.AppendFormat("{1}    class={0}\n", keyValuePair2.Key, indent);
				keyValuePair2.Value.PrettyPrint(sb, string.Format("{0}    ", indent));
			}
		}
	}

	// Token: 0x06008C0A RID: 35850 RVA: 0x003867EC File Offset: 0x003849EC
	[PublicizedFrom(EAccessModifier.Private)]
	static DynamicProperties()
	{
		DynamicProperties.RegisterFormatter();
	}

	// Token: 0x06008C0B RID: 35851 RVA: 0x00386813 File Offset: 0x00384A13
	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<DynamicProperties>())
		{
			MemoryPackFormatterProvider.Register<DynamicProperties>(new DynamicProperties.DynamicPropertiesFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<DynamicProperties[]>())
		{
			MemoryPackFormatterProvider.Register<DynamicProperties[]>(new ArrayFormatter<DynamicProperties>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<HashSet<string>>())
		{
			MemoryPackFormatterProvider.Register<HashSet<string>>(new HashSetFormatter<string>());
		}
	}

	// Token: 0x06008C0C RID: 35852 RVA: 0x00386848 File Offset: 0x00384A48
	[NullableContext(2)]
	[Preserve]
	public static void Serialize(ref MemoryPackWriter writer, ref DynamicProperties value)
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteObjectHeader(6);
		writer.WritePackable<DictionarySave<string, string>>(value.Params1);
		writer.WritePackable<DictionarySave<string, string>>(value.Params2);
		writer.WritePackable<DictionarySave<string, string>>(value.Data);
		writer.WriteValue<HashSet<string>>(value.Display);
		writer.WritePackable<DictionarySave<string, DynamicProperties>>(value.Classes);
		writer.WritePackable<DictionarySave<string, string>>(value.Values);
	}

	// Token: 0x06008C0D RID: 35853 RVA: 0x003868B8 File Offset: 0x00384AB8
	[NullableContext(2)]
	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref DynamicProperties value)
	{
		byte b;
		if (!reader.TryReadObjectHeader(out b))
		{
			value = null;
			return;
		}
		DictionarySave<string, string> @params;
		DictionarySave<string, string> params2;
		DictionarySave<string, string> data;
		HashSet<string> display;
		DictionarySave<string, DynamicProperties> classes;
		DictionarySave<string, string> values;
		if (b == 6)
		{
			if (value == null)
			{
				@params = reader.ReadPackable<DictionarySave<string, string>>();
				params2 = reader.ReadPackable<DictionarySave<string, string>>();
				data = reader.ReadPackable<DictionarySave<string, string>>();
				display = reader.ReadValue<HashSet<string>>();
				classes = reader.ReadPackable<DictionarySave<string, DynamicProperties>>();
				values = reader.ReadPackable<DictionarySave<string, string>>();
				goto IL_194;
			}
			@params = value.Params1;
			params2 = value.Params2;
			data = value.Data;
			display = value.Display;
			classes = value.Classes;
			values = value.Values;
			reader.ReadPackable<DictionarySave<string, string>>(ref @params);
			reader.ReadPackable<DictionarySave<string, string>>(ref params2);
			reader.ReadPackable<DictionarySave<string, string>>(ref data);
			reader.ReadValue<HashSet<string>>(ref display);
			reader.ReadPackable<DictionarySave<string, DynamicProperties>>(ref classes);
			reader.ReadPackable<DictionarySave<string, string>>(ref values);
		}
		else
		{
			if (b > 6)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(DynamicProperties), 6, b);
				return;
			}
			if (value == null)
			{
				@params = null;
				params2 = null;
				data = null;
				display = null;
				classes = null;
				values = null;
			}
			else
			{
				@params = value.Params1;
				params2 = value.Params2;
				data = value.Data;
				display = value.Display;
				classes = value.Classes;
				values = value.Values;
			}
			if (b != 0)
			{
				reader.ReadPackable<DictionarySave<string, string>>(ref @params);
				if (b != 1)
				{
					reader.ReadPackable<DictionarySave<string, string>>(ref params2);
					if (b != 2)
					{
						reader.ReadPackable<DictionarySave<string, string>>(ref data);
						if (b != 3)
						{
							reader.ReadValue<HashSet<string>>(ref display);
							if (b != 4)
							{
								reader.ReadPackable<DictionarySave<string, DynamicProperties>>(ref classes);
								if (b != 5)
								{
									reader.ReadPackable<DictionarySave<string, string>>(ref values);
								}
							}
						}
					}
				}
			}
			if (value == null)
			{
				goto IL_194;
			}
		}
		value.Params1 = @params;
		value.Params2 = params2;
		value.Data = data;
		value.Display = display;
		value.Classes = classes;
		value.Values = values;
		return;
		IL_194:
		value = new DynamicProperties
		{
			Params1 = @params,
			Params2 = params2,
			Data = data,
			Display = display,
			Classes = classes,
			Values = values
		};
	}

	// Token: 0x04006D1D RID: 27933
	public DictionarySave<string, string> Params1 = new DictionarySave<string, string>();

	// Token: 0x04006D1E RID: 27934
	public DictionarySave<string, string> Params2 = new DictionarySave<string, string>();

	// Token: 0x04006D1F RID: 27935
	public DictionarySave<string, string> Data = new DictionarySave<string, string>();

	// Token: 0x04006D20 RID: 27936
	public HashSet<string> Display = new HashSet<string>();

	// Token: 0x04006D21 RID: 27937
	public DictionarySave<string, DynamicProperties> Classes = new DictionarySave<string, DynamicProperties>();

	// Token: 0x04006D22 RID: 27938
	public DictionarySave<string, string> Values = new DictionarySave<string, string>();

	// Token: 0x04006D23 RID: 27939
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly char[] semicolonSeparator = new char[]
	{
		';'
	};

	// Token: 0x04006D24 RID: 27940
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly char[] equalSeparator = new char[]
	{
		'='
	};

	// Token: 0x02001181 RID: 4481
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	[Preserve]
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class DynamicPropertiesFormatter : MemoryPackFormatter<DynamicProperties>
	{
		// Token: 0x06008C0F RID: 35855 RVA: 0x00386AE5 File Offset: 0x00384CE5
		[Preserve]
		public override void Serialize(ref MemoryPackWriter writer, ref DynamicProperties value)
		{
			DynamicProperties.Serialize(ref writer, ref value);
		}

		// Token: 0x06008C10 RID: 35856 RVA: 0x00386AEE File Offset: 0x00384CEE
		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref DynamicProperties value)
		{
			DynamicProperties.Deserialize(ref reader, ref value);
		}
	}
}
