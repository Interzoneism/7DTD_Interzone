using System;
using System.Collections.Generic;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using UnityEngine.Scripting;
using XMLData.Exceptions;
using XMLData.Parsers;

namespace XMLData.Item
{
	// Token: 0x0200138F RID: 5007
	[Preserve]
	public class ArmorData : IXMLData
	{
		// Token: 0x1700106A RID: 4202
		// (get) Token: 0x06009C85 RID: 40069 RVA: 0x003E1E8E File Offset: 0x003E008E
		// (set) Token: 0x06009C86 RID: 40070 RVA: 0x003E1E96 File Offset: 0x003E0096
		public DataItem<float> Melee
		{
			get
			{
				return this.pMelee;
			}
			set
			{
				this.pMelee = value;
			}
		}

		// Token: 0x1700106B RID: 4203
		// (get) Token: 0x06009C87 RID: 40071 RVA: 0x003E1E9F File Offset: 0x003E009F
		// (set) Token: 0x06009C88 RID: 40072 RVA: 0x003E1EA7 File Offset: 0x003E00A7
		public DataItem<float> Bullet
		{
			get
			{
				return this.pBullet;
			}
			set
			{
				this.pBullet = value;
			}
		}

		// Token: 0x1700106C RID: 4204
		// (get) Token: 0x06009C89 RID: 40073 RVA: 0x003E1EB0 File Offset: 0x003E00B0
		// (set) Token: 0x06009C8A RID: 40074 RVA: 0x003E1EB8 File Offset: 0x003E00B8
		public DataItem<float> Puncture
		{
			get
			{
				return this.pPuncture;
			}
			set
			{
				this.pPuncture = value;
			}
		}

		// Token: 0x1700106D RID: 4205
		// (get) Token: 0x06009C8B RID: 40075 RVA: 0x003E1EC1 File Offset: 0x003E00C1
		// (set) Token: 0x06009C8C RID: 40076 RVA: 0x003E1EC9 File Offset: 0x003E00C9
		public DataItem<float> Blunt
		{
			get
			{
				return this.pBlunt;
			}
			set
			{
				this.pBlunt = value;
			}
		}

		// Token: 0x1700106E RID: 4206
		// (get) Token: 0x06009C8D RID: 40077 RVA: 0x003E1ED2 File Offset: 0x003E00D2
		// (set) Token: 0x06009C8E RID: 40078 RVA: 0x003E1EDA File Offset: 0x003E00DA
		public DataItem<float> Explosive
		{
			get
			{
				return this.pExplosive;
			}
			set
			{
				this.pExplosive = value;
			}
		}

		// Token: 0x06009C8F RID: 40079 RVA: 0x003E1EE3 File Offset: 0x003E00E3
		public List<IDataItem> GetDisplayValues(bool _recursive = true)
		{
			return new List<IDataItem>();
		}

		// Token: 0x040078FF RID: 30975
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pMelee;

		// Token: 0x04007900 RID: 30976
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pBullet;

		// Token: 0x04007901 RID: 30977
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pPuncture;

		// Token: 0x04007902 RID: 30978
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pBlunt;

		// Token: 0x04007903 RID: 30979
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pExplosive;

		// Token: 0x02001390 RID: 5008
		public static class Parser
		{
			// Token: 0x06009C91 RID: 40081 RVA: 0x003E1EEC File Offset: 0x003E00EC
			public static ArmorData Parse(PositionXmlElement _elem, Dictionary<PositionXmlElement, DataItem<ItemClass>> _updateLater)
			{
				string text = _elem.HasAttribute("class") ? _elem.GetAttribute("class") : "ArmorData";
				Type type = Type.GetType(typeof(ArmorData.Parser).Namespace + "." + text);
				if (type == null)
				{
					type = Type.GetType(text);
					if (type == null)
					{
						throw new InvalidValueException("Specified class \"" + text + "\" not found", _elem.LineNumber);
					}
				}
				ArmorData armorData = (ArmorData)Activator.CreateInstance(type);
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (object obj in _elem.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlNodeType nodeType = xmlNode.NodeType;
					if (nodeType != XmlNodeType.Element)
					{
						if (nodeType != XmlNodeType.Comment)
						{
							throw new UnexpectedElementException("Unknown node \"" + xmlNode.NodeType.ToString() + "\" found while parsing Armor", ((IXmlLineInfo)xmlNode).LineNumber);
						}
					}
					else
					{
						PositionXmlElement positionXmlElement = (PositionXmlElement)xmlNode;
						if (!ArmorData.Parser.knownAttributesMultiplicity.ContainsKey(positionXmlElement.Name))
						{
							throw new UnexpectedElementException("Unknown element \"" + xmlNode.Name + "\" found while parsing Armor", ((IXmlLineInfo)xmlNode).LineNumber);
						}
						string name = positionXmlElement.Name;
						if (!(name == "Melee"))
						{
							if (!(name == "Bullet"))
							{
								if (!(name == "Puncture"))
								{
									if (!(name == "Blunt"))
									{
										if (name == "Explosive")
										{
											float startValue;
											try
											{
												startValue = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException);
											}
											DataItem<float> pExplosive = new DataItem<float>("Explosive", startValue);
											armorData.pExplosive = pExplosive;
										}
									}
									else
									{
										float startValue2;
										try
										{
											startValue2 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException2)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException2);
										}
										DataItem<float> pBlunt = new DataItem<float>("Blunt", startValue2);
										armorData.pBlunt = pBlunt;
									}
								}
								else
								{
									float startValue3;
									try
									{
										startValue3 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException3)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException3);
									}
									DataItem<float> pPuncture = new DataItem<float>("Puncture", startValue3);
									armorData.pPuncture = pPuncture;
								}
							}
							else
							{
								float startValue4;
								try
								{
									startValue4 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException4)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException4);
								}
								DataItem<float> pBullet = new DataItem<float>("Bullet", startValue4);
								armorData.pBullet = pBullet;
							}
						}
						else
						{
							float startValue5;
							try
							{
								startValue5 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
							}
							catch (Exception innerException5)
							{
								throw new InvalidValueException(string.Concat(new string[]
								{
									"Could not parse attribute \"",
									positionXmlElement.Name,
									"\" value \"",
									ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
									"\""
								}), positionXmlElement.LineNumber, innerException5);
							}
							DataItem<float> pMelee = new DataItem<float>("Melee", startValue5);
							armorData.pMelee = pMelee;
						}
						if (!dictionary.ContainsKey(positionXmlElement.Name))
						{
							dictionary[positionXmlElement.Name] = 0;
						}
						Dictionary<string, int> dictionary2 = dictionary;
						name = positionXmlElement.Name;
						int num = dictionary2[name];
						dictionary2[name] = num + 1;
					}
				}
				foreach (KeyValuePair<string, Range<int>> keyValuePair in ArmorData.Parser.knownAttributesMultiplicity)
				{
					int num2 = dictionary.ContainsKey(keyValuePair.Key) ? dictionary[keyValuePair.Key] : 0;
					if ((keyValuePair.Value.hasMin && num2 < keyValuePair.Value.min) || (keyValuePair.Value.hasMax && num2 > keyValuePair.Value.max))
					{
						throw new IncorrectAttributeOccurrenceException(string.Concat(new string[]
						{
							"Element has incorrect number of \"",
							keyValuePair.Key,
							"\" attribute instances, found ",
							num2.ToString(),
							", expected ",
							keyValuePair.Value.ToString()
						}), _elem.LineNumber);
					}
				}
				return armorData;
			}

			// Token: 0x04007904 RID: 30980
			[PublicizedFrom(EAccessModifier.Private)]
			public static Dictionary<string, Range<int>> knownAttributesMultiplicity = new Dictionary<string, Range<int>>
			{
				{
					"Melee",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Bullet",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Puncture",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Blunt",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Explosive",
					new Range<int>(true, 0, true, 1)
				}
			};
		}
	}
}
