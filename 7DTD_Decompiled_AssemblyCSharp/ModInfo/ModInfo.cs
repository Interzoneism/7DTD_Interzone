using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.WpfDesign.XamlDom;
using XMLData;
using XMLData.Exceptions;
using XMLData.Parsers;

namespace ModInfo
{
	// Token: 0x02001377 RID: 4983
	public class ModInfo : IXMLData
	{
		// Token: 0x1700104C RID: 4172
		// (get) Token: 0x06009C19 RID: 39961 RVA: 0x003E0D6C File Offset: 0x003DEF6C
		// (set) Token: 0x06009C1A RID: 39962 RVA: 0x003E0D74 File Offset: 0x003DEF74
		public DataItem<string> Name
		{
			get
			{
				return this.pName;
			}
			set
			{
				this.pName = value;
			}
		}

		// Token: 0x1700104D RID: 4173
		// (get) Token: 0x06009C1B RID: 39963 RVA: 0x003E0D7D File Offset: 0x003DEF7D
		// (set) Token: 0x06009C1C RID: 39964 RVA: 0x003E0D85 File Offset: 0x003DEF85
		public DataItem<string> Description
		{
			get
			{
				return this.pDescription;
			}
			set
			{
				this.pDescription = value;
			}
		}

		// Token: 0x1700104E RID: 4174
		// (get) Token: 0x06009C1D RID: 39965 RVA: 0x003E0D8E File Offset: 0x003DEF8E
		// (set) Token: 0x06009C1E RID: 39966 RVA: 0x003E0D96 File Offset: 0x003DEF96
		public DataItem<string> Author
		{
			get
			{
				return this.pAuthor;
			}
			set
			{
				this.pAuthor = value;
			}
		}

		// Token: 0x1700104F RID: 4175
		// (get) Token: 0x06009C1F RID: 39967 RVA: 0x003E0D9F File Offset: 0x003DEF9F
		// (set) Token: 0x06009C20 RID: 39968 RVA: 0x003E0DA7 File Offset: 0x003DEFA7
		public DataItem<string> Version
		{
			get
			{
				return this.pVersion;
			}
			set
			{
				this.pVersion = value;
			}
		}

		// Token: 0x17001050 RID: 4176
		// (get) Token: 0x06009C21 RID: 39969 RVA: 0x003E0DB0 File Offset: 0x003DEFB0
		// (set) Token: 0x06009C22 RID: 39970 RVA: 0x003E0DB8 File Offset: 0x003DEFB8
		public DataItem<string> Website
		{
			get
			{
				return this.pWebsite;
			}
			set
			{
				this.pWebsite = value;
			}
		}

		// Token: 0x040078CA RID: 30922
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pName;

		// Token: 0x040078CB RID: 30923
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pDescription;

		// Token: 0x040078CC RID: 30924
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pAuthor;

		// Token: 0x040078CD RID: 30925
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pVersion;

		// Token: 0x040078CE RID: 30926
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pWebsite;

		// Token: 0x02001378 RID: 4984
		public static class Parser
		{
			// Token: 0x06009C24 RID: 39972 RVA: 0x003E0DC4 File Offset: 0x003DEFC4
			public static ModInfo Parse(XElement _elem, Dictionary<PositionXmlElement, DataItem<ModInfo>> _updateLater)
			{
				ModInfo modInfo = new ModInfo();
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (XElement xelement in _elem.Elements())
				{
					if (!ModInfo.Parser.knownAttributesMultiplicity.ContainsKey(xelement.Name.LocalName))
					{
						string str = "Unknown element \"";
						XName name = xelement.Name;
						throw new UnexpectedElementException(str + ((name != null) ? name.ToString() : null) + "\" found while parsing ModInfo", ((IXmlLineInfo)xelement).LineNumber);
					}
					string localName = xelement.Name.LocalName;
					if (!(localName == "Name"))
					{
						if (!(localName == "Description"))
						{
							if (!(localName == "Author"))
							{
								if (!(localName == "Version"))
								{
									if (localName == "Website")
									{
										ModInfo.Parser.ParseFieldAttributeWebsite(modInfo, dictionary, xelement);
									}
								}
								else
								{
									ModInfo.Parser.ParseFieldAttributeVersion(modInfo, dictionary, xelement);
								}
							}
							else
							{
								ModInfo.Parser.ParseFieldAttributeAuthor(modInfo, dictionary, xelement);
							}
						}
						else
						{
							ModInfo.Parser.ParseFieldAttributeDescription(modInfo, dictionary, xelement);
						}
					}
					else
					{
						ModInfo.Parser.ParseFieldAttributeName(modInfo, dictionary, xelement);
					}
				}
				foreach (KeyValuePair<string, Range<int>> keyValuePair in ModInfo.Parser.knownAttributesMultiplicity)
				{
					int num = dictionary.ContainsKey(keyValuePair.Key) ? dictionary[keyValuePair.Key] : 0;
					if ((keyValuePair.Value.hasMin && num < keyValuePair.Value.min) || (keyValuePair.Value.hasMax && num > keyValuePair.Value.max))
					{
						string[] array = new string[6];
						array[0] = "Element has incorrect number of \"";
						array[1] = keyValuePair.Key;
						array[2] = "\" attribute instances, found ";
						array[3] = num.ToString();
						array[4] = ", expected ";
						int num2 = 5;
						Range<int> value = keyValuePair.Value;
						array[num2] = ((value != null) ? value.ToString() : null);
						throw new IncorrectAttributeOccurrenceException(string.Concat(array), ((IXmlLineInfo)_elem).LineNumber);
					}
				}
				return modInfo;
			}

			// Token: 0x06009C25 RID: 39973 RVA: 0x003E0FE4 File Offset: 0x003DF1E4
			[PublicizedFrom(EAccessModifier.Private)]
			public static void ParseFieldAttributeName(ModInfo _entry, Dictionary<string, int> _foundAttributes, XElement _elem)
			{
				string text = null;
				if (_elem != null)
				{
					text = ParserUtils.ParseStringAttribute(_elem, "value", true, null);
				}
				string startValue;
				try
				{
					startValue = stringParser.Parse(text);
				}
				catch (Exception innerException)
				{
					throw new InvalidValueException("Could not parse attribute \"Name\" value \"" + text + "\"", (_elem != null) ? ((IXmlLineInfo)_elem).LineNumber : -1, innerException);
				}
				DataItem<string> pName = new DataItem<string>("Name", startValue);
				_entry.pName = pName;
				if (_elem != null)
				{
					if (!_foundAttributes.ContainsKey("Name"))
					{
						_foundAttributes["Name"] = 0;
					}
					int num = _foundAttributes["Name"];
					_foundAttributes["Name"] = num + 1;
				}
			}

			// Token: 0x06009C26 RID: 39974 RVA: 0x003E1090 File Offset: 0x003DF290
			[PublicizedFrom(EAccessModifier.Private)]
			public static void ParseFieldAttributeDescription(ModInfo _entry, Dictionary<string, int> _foundAttributes, XElement _elem)
			{
				string text = null;
				if (_elem != null)
				{
					text = ParserUtils.ParseStringAttribute(_elem, "value", true, null);
				}
				string startValue;
				try
				{
					startValue = stringParser.Parse(text);
				}
				catch (Exception innerException)
				{
					throw new InvalidValueException("Could not parse attribute \"Description\" value \"" + text + "\"", (_elem != null) ? ((IXmlLineInfo)_elem).LineNumber : -1, innerException);
				}
				DataItem<string> pDescription = new DataItem<string>("Description", startValue);
				_entry.pDescription = pDescription;
				if (_elem != null)
				{
					if (!_foundAttributes.ContainsKey("Description"))
					{
						_foundAttributes["Description"] = 0;
					}
					int num = _foundAttributes["Description"];
					_foundAttributes["Description"] = num + 1;
				}
			}

			// Token: 0x06009C27 RID: 39975 RVA: 0x003E113C File Offset: 0x003DF33C
			[PublicizedFrom(EAccessModifier.Private)]
			public static void ParseFieldAttributeAuthor(ModInfo _entry, Dictionary<string, int> _foundAttributes, XElement _elem)
			{
				string text = null;
				if (_elem != null)
				{
					text = ParserUtils.ParseStringAttribute(_elem, "value", true, null);
				}
				string startValue;
				try
				{
					startValue = stringParser.Parse(text);
				}
				catch (Exception innerException)
				{
					throw new InvalidValueException("Could not parse attribute \"Author\" value \"" + text + "\"", (_elem != null) ? ((IXmlLineInfo)_elem).LineNumber : -1, innerException);
				}
				DataItem<string> pAuthor = new DataItem<string>("Author", startValue);
				_entry.pAuthor = pAuthor;
				if (_elem != null)
				{
					if (!_foundAttributes.ContainsKey("Author"))
					{
						_foundAttributes["Author"] = 0;
					}
					int num = _foundAttributes["Author"];
					_foundAttributes["Author"] = num + 1;
				}
			}

			// Token: 0x06009C28 RID: 39976 RVA: 0x003E11E8 File Offset: 0x003DF3E8
			[PublicizedFrom(EAccessModifier.Private)]
			public static void ParseFieldAttributeVersion(ModInfo _entry, Dictionary<string, int> _foundAttributes, XElement _elem)
			{
				string text = null;
				if (_elem != null)
				{
					text = ParserUtils.ParseStringAttribute(_elem, "value", true, null);
				}
				string startValue;
				try
				{
					startValue = stringParser.Parse(text);
				}
				catch (Exception innerException)
				{
					throw new InvalidValueException("Could not parse attribute \"Version\" value \"" + text + "\"", (_elem != null) ? ((IXmlLineInfo)_elem).LineNumber : -1, innerException);
				}
				DataItem<string> pVersion = new DataItem<string>("Version", startValue);
				_entry.pVersion = pVersion;
				if (_elem != null)
				{
					if (!_foundAttributes.ContainsKey("Version"))
					{
						_foundAttributes["Version"] = 0;
					}
					int num = _foundAttributes["Version"];
					_foundAttributes["Version"] = num + 1;
				}
			}

			// Token: 0x06009C29 RID: 39977 RVA: 0x003E1294 File Offset: 0x003DF494
			[PublicizedFrom(EAccessModifier.Private)]
			public static void ParseFieldAttributeWebsite(ModInfo _entry, Dictionary<string, int> _foundAttributes, XElement _elem)
			{
				string text = null;
				if (_elem != null)
				{
					text = ParserUtils.ParseStringAttribute(_elem, "value", true, null);
				}
				string startValue;
				try
				{
					startValue = stringParser.Parse(text);
				}
				catch (Exception innerException)
				{
					throw new InvalidValueException("Could not parse attribute \"Website\" value \"" + text + "\"", (_elem != null) ? ((IXmlLineInfo)_elem).LineNumber : -1, innerException);
				}
				DataItem<string> pWebsite = new DataItem<string>("Website", startValue);
				_entry.pWebsite = pWebsite;
				if (_elem != null)
				{
					if (!_foundAttributes.ContainsKey("Website"))
					{
						_foundAttributes["Website"] = 0;
					}
					int num = _foundAttributes["Website"];
					_foundAttributes["Website"] = num + 1;
				}
			}

			// Token: 0x040078CF RID: 30927
			[PublicizedFrom(EAccessModifier.Private)]
			public static readonly Dictionary<string, Range<int>> knownAttributesMultiplicity = new Dictionary<string, Range<int>>
			{
				{
					"Name",
					new Range<int>(true, 1, true, 1)
				},
				{
					"Description",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Author",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Version",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Website",
					new Range<int>(true, 0, true, 1)
				}
			};
		}
	}
}
