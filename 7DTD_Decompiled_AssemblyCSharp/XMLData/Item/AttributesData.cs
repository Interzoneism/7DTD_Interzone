using System;
using System.Collections.Generic;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using UnityEngine.Scripting;
using XMLData.Exceptions;
using XMLData.Parsers;

namespace XMLData.Item
{
	// Token: 0x02001391 RID: 5009
	[Preserve]
	public class AttributesData : IXMLData
	{
		// Token: 0x1700106F RID: 4207
		// (get) Token: 0x06009C93 RID: 40083 RVA: 0x003E258B File Offset: 0x003E078B
		// (set) Token: 0x06009C94 RID: 40084 RVA: 0x003E2593 File Offset: 0x003E0793
		public DataItem<string> EntityDamage
		{
			get
			{
				return this.pEntityDamage;
			}
			set
			{
				this.pEntityDamage = value;
			}
		}

		// Token: 0x17001070 RID: 4208
		// (get) Token: 0x06009C95 RID: 40085 RVA: 0x003E259C File Offset: 0x003E079C
		// (set) Token: 0x06009C96 RID: 40086 RVA: 0x003E25A4 File Offset: 0x003E07A4
		public DataItem<string> BlockDamage
		{
			get
			{
				return this.pBlockDamage;
			}
			set
			{
				this.pBlockDamage = value;
			}
		}

		// Token: 0x17001071 RID: 4209
		// (get) Token: 0x06009C97 RID: 40087 RVA: 0x003E25AD File Offset: 0x003E07AD
		// (set) Token: 0x06009C98 RID: 40088 RVA: 0x003E25B5 File Offset: 0x003E07B5
		public DataItem<string> Accuracy
		{
			get
			{
				return this.pAccuracy;
			}
			set
			{
				this.pAccuracy = value;
			}
		}

		// Token: 0x17001072 RID: 4210
		// (get) Token: 0x06009C99 RID: 40089 RVA: 0x003E25BE File Offset: 0x003E07BE
		// (set) Token: 0x06009C9A RID: 40090 RVA: 0x003E25C6 File Offset: 0x003E07C6
		public DataItem<string> FalloffRange
		{
			get
			{
				return this.pFalloffRange;
			}
			set
			{
				this.pFalloffRange = value;
			}
		}

		// Token: 0x17001073 RID: 4211
		// (get) Token: 0x06009C9B RID: 40091 RVA: 0x003E25CF File Offset: 0x003E07CF
		// (set) Token: 0x06009C9C RID: 40092 RVA: 0x003E25D7 File Offset: 0x003E07D7
		public DataItem<string> GainHealth
		{
			get
			{
				return this.pGainHealth;
			}
			set
			{
				this.pGainHealth = value;
			}
		}

		// Token: 0x17001074 RID: 4212
		// (get) Token: 0x06009C9D RID: 40093 RVA: 0x003E25E0 File Offset: 0x003E07E0
		// (set) Token: 0x06009C9E RID: 40094 RVA: 0x003E25E8 File Offset: 0x003E07E8
		public DataItem<string> GainFood
		{
			get
			{
				return this.pGainFood;
			}
			set
			{
				this.pGainFood = value;
			}
		}

		// Token: 0x17001075 RID: 4213
		// (get) Token: 0x06009C9F RID: 40095 RVA: 0x003E25F1 File Offset: 0x003E07F1
		// (set) Token: 0x06009CA0 RID: 40096 RVA: 0x003E25F9 File Offset: 0x003E07F9
		public DataItem<string> GainWater
		{
			get
			{
				return this.pGainWater;
			}
			set
			{
				this.pGainWater = value;
			}
		}

		// Token: 0x17001076 RID: 4214
		// (get) Token: 0x06009CA1 RID: 40097 RVA: 0x003E2602 File Offset: 0x003E0802
		// (set) Token: 0x06009CA2 RID: 40098 RVA: 0x003E260A File Offset: 0x003E080A
		public DataItem<string> DegradationRate
		{
			get
			{
				return this.pDegradationRate;
			}
			set
			{
				this.pDegradationRate = value;
			}
		}

		// Token: 0x06009CA3 RID: 40099 RVA: 0x003E1EE3 File Offset: 0x003E00E3
		public List<IDataItem> GetDisplayValues(bool _recursive = true)
		{
			return new List<IDataItem>();
		}

		// Token: 0x04007905 RID: 30981
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pEntityDamage;

		// Token: 0x04007906 RID: 30982
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pBlockDamage;

		// Token: 0x04007907 RID: 30983
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pAccuracy;

		// Token: 0x04007908 RID: 30984
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFalloffRange;

		// Token: 0x04007909 RID: 30985
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pGainHealth;

		// Token: 0x0400790A RID: 30986
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pGainFood;

		// Token: 0x0400790B RID: 30987
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pGainWater;

		// Token: 0x0400790C RID: 30988
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pDegradationRate;

		// Token: 0x02001392 RID: 5010
		public static class Parser
		{
			// Token: 0x06009CA5 RID: 40101 RVA: 0x003E2614 File Offset: 0x003E0814
			public static AttributesData Parse(PositionXmlElement _elem, Dictionary<PositionXmlElement, DataItem<ItemClass>> _updateLater)
			{
				string text = _elem.HasAttribute("class") ? _elem.GetAttribute("class") : "AttributesData";
				Type type = Type.GetType(typeof(AttributesData.Parser).Namespace + "." + text);
				if (type == null)
				{
					type = Type.GetType(text);
					if (type == null)
					{
						throw new InvalidValueException("Specified class \"" + text + "\" not found", _elem.LineNumber);
					}
				}
				AttributesData attributesData = (AttributesData)Activator.CreateInstance(type);
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (object obj in _elem.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlNodeType nodeType = xmlNode.NodeType;
					if (nodeType != XmlNodeType.Element)
					{
						if (nodeType != XmlNodeType.Comment)
						{
							throw new UnexpectedElementException("Unknown node \"" + xmlNode.NodeType.ToString() + "\" found while parsing Attributes", ((IXmlLineInfo)xmlNode).LineNumber);
						}
					}
					else
					{
						PositionXmlElement positionXmlElement = (PositionXmlElement)xmlNode;
						if (!AttributesData.Parser.knownAttributesMultiplicity.ContainsKey(positionXmlElement.Name))
						{
							throw new UnexpectedElementException("Unknown element \"" + xmlNode.Name + "\" found while parsing Attributes", ((IXmlLineInfo)xmlNode).LineNumber);
						}
						string name = positionXmlElement.Name;
						uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
						if (num <= 2055669289U)
						{
							if (num <= 711561591U)
							{
								if (num != 535561810U)
								{
									if (num == 711561591U)
									{
										if (name == "EntityDamage")
										{
											string startValue;
											try
											{
												startValue = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
											DataItem<string> pEntityDamage = new DataItem<string>("EntityDamage", startValue);
											attributesData.pEntityDamage = pEntityDamage;
										}
									}
								}
								else if (name == "FalloffRange")
								{
									string startValue2;
									try
									{
										startValue2 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
									DataItem<string> pFalloffRange = new DataItem<string>("FalloffRange", startValue2);
									attributesData.pFalloffRange = pFalloffRange;
								}
							}
							else if (num != 1827041476U)
							{
								if (num == 2055669289U)
								{
									if (name == "DegradationRate")
									{
										string startValue3;
										try
										{
											startValue3 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
										DataItem<string> pDegradationRate = new DataItem<string>("DegradationRate", startValue3);
										attributesData.pDegradationRate = pDegradationRate;
									}
								}
							}
							else if (name == "Accuracy")
							{
								string startValue4;
								try
								{
									startValue4 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
								DataItem<string> pAccuracy = new DataItem<string>("Accuracy", startValue4);
								attributesData.pAccuracy = pAccuracy;
							}
						}
						else if (num <= 2614118511U)
						{
							if (num != 2391273097U)
							{
								if (num == 2614118511U)
								{
									if (name == "BlockDamage")
									{
										string startValue5;
										try
										{
											startValue5 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
										DataItem<string> pBlockDamage = new DataItem<string>("BlockDamage", startValue5);
										attributesData.pBlockDamage = pBlockDamage;
									}
								}
							}
							else if (name == "GainWater")
							{
								string startValue6;
								try
								{
									startValue6 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException6)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException6);
								}
								DataItem<string> pGainWater = new DataItem<string>("GainWater", startValue6);
								attributesData.pGainWater = pGainWater;
							}
						}
						else if (num != 3036802414U)
						{
							if (num == 3448204316U)
							{
								if (name == "GainHealth")
								{
									string startValue7;
									try
									{
										startValue7 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException7)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException7);
									}
									DataItem<string> pGainHealth = new DataItem<string>("GainHealth", startValue7);
									attributesData.pGainHealth = pGainHealth;
								}
							}
						}
						else if (name == "GainFood")
						{
							string startValue8;
							try
							{
								startValue8 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
							}
							catch (Exception innerException8)
							{
								throw new InvalidValueException(string.Concat(new string[]
								{
									"Could not parse attribute \"",
									positionXmlElement.Name,
									"\" value \"",
									ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
									"\""
								}), positionXmlElement.LineNumber, innerException8);
							}
							DataItem<string> pGainFood = new DataItem<string>("GainFood", startValue8);
							attributesData.pGainFood = pGainFood;
						}
						if (!dictionary.ContainsKey(positionXmlElement.Name))
						{
							dictionary[positionXmlElement.Name] = 0;
						}
						Dictionary<string, int> dictionary2 = dictionary;
						name = positionXmlElement.Name;
						int num2 = dictionary2[name];
						dictionary2[name] = num2 + 1;
					}
				}
				foreach (KeyValuePair<string, Range<int>> keyValuePair in AttributesData.Parser.knownAttributesMultiplicity)
				{
					int num3 = dictionary.ContainsKey(keyValuePair.Key) ? dictionary[keyValuePair.Key] : 0;
					if ((keyValuePair.Value.hasMin && num3 < keyValuePair.Value.min) || (keyValuePair.Value.hasMax && num3 > keyValuePair.Value.max))
					{
						throw new IncorrectAttributeOccurrenceException(string.Concat(new string[]
						{
							"Element has incorrect number of \"",
							keyValuePair.Key,
							"\" attribute instances, found ",
							num3.ToString(),
							", expected ",
							keyValuePair.Value.ToString()
						}), _elem.LineNumber);
					}
				}
				return attributesData;
			}

			// Token: 0x0400790D RID: 30989
			[PublicizedFrom(EAccessModifier.Private)]
			public static Dictionary<string, Range<int>> knownAttributesMultiplicity = new Dictionary<string, Range<int>>
			{
				{
					"EntityDamage",
					new Range<int>(true, 0, true, 1)
				},
				{
					"BlockDamage",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Accuracy",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FalloffRange",
					new Range<int>(true, 0, true, 1)
				},
				{
					"GainHealth",
					new Range<int>(true, 0, true, 1)
				},
				{
					"GainFood",
					new Range<int>(true, 0, true, 1)
				},
				{
					"GainWater",
					new Range<int>(true, 0, true, 1)
				},
				{
					"DegradationRate",
					new Range<int>(true, 0, true, 1)
				}
			};
		}
	}
}
