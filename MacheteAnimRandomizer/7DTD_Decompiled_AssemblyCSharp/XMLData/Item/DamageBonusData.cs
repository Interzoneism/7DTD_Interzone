using System;
using System.Collections.Generic;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using UnityEngine.Scripting;
using XMLData.Exceptions;
using XMLData.Parsers;

namespace XMLData.Item
{
	// Token: 0x02001393 RID: 5011
	[Preserve]
	public class DamageBonusData : IXMLData
	{
		// Token: 0x17001077 RID: 4215
		// (get) Token: 0x06009CA7 RID: 40103 RVA: 0x003E2F9F File Offset: 0x003E119F
		// (set) Token: 0x06009CA8 RID: 40104 RVA: 0x003E2FA7 File Offset: 0x003E11A7
		public DataItem<float> Head
		{
			get
			{
				return this.pHead;
			}
			set
			{
				this.pHead = value;
			}
		}

		// Token: 0x17001078 RID: 4216
		// (get) Token: 0x06009CA9 RID: 40105 RVA: 0x003E2FB0 File Offset: 0x003E11B0
		// (set) Token: 0x06009CAA RID: 40106 RVA: 0x003E2FB8 File Offset: 0x003E11B8
		public DataItem<float> Glass
		{
			get
			{
				return this.pGlass;
			}
			set
			{
				this.pGlass = value;
			}
		}

		// Token: 0x17001079 RID: 4217
		// (get) Token: 0x06009CAB RID: 40107 RVA: 0x003E2FC1 File Offset: 0x003E11C1
		// (set) Token: 0x06009CAC RID: 40108 RVA: 0x003E2FC9 File Offset: 0x003E11C9
		public DataItem<float> Stone
		{
			get
			{
				return this.pStone;
			}
			set
			{
				this.pStone = value;
			}
		}

		// Token: 0x1700107A RID: 4218
		// (get) Token: 0x06009CAD RID: 40109 RVA: 0x003E2FD2 File Offset: 0x003E11D2
		// (set) Token: 0x06009CAE RID: 40110 RVA: 0x003E2FDA File Offset: 0x003E11DA
		public DataItem<float> Cloth
		{
			get
			{
				return this.pCloth;
			}
			set
			{
				this.pCloth = value;
			}
		}

		// Token: 0x1700107B RID: 4219
		// (get) Token: 0x06009CAF RID: 40111 RVA: 0x003E2FE3 File Offset: 0x003E11E3
		// (set) Token: 0x06009CB0 RID: 40112 RVA: 0x003E2FEB File Offset: 0x003E11EB
		public DataItem<float> Concrete
		{
			get
			{
				return this.pConcrete;
			}
			set
			{
				this.pConcrete = value;
			}
		}

		// Token: 0x1700107C RID: 4220
		// (get) Token: 0x06009CB1 RID: 40113 RVA: 0x003E2FF4 File Offset: 0x003E11F4
		// (set) Token: 0x06009CB2 RID: 40114 RVA: 0x003E2FFC File Offset: 0x003E11FC
		public DataItem<float> Boulder
		{
			get
			{
				return this.pBoulder;
			}
			set
			{
				this.pBoulder = value;
			}
		}

		// Token: 0x1700107D RID: 4221
		// (get) Token: 0x06009CB3 RID: 40115 RVA: 0x003E3005 File Offset: 0x003E1205
		// (set) Token: 0x06009CB4 RID: 40116 RVA: 0x003E300D File Offset: 0x003E120D
		public DataItem<float> Metal
		{
			get
			{
				return this.pMetal;
			}
			set
			{
				this.pMetal = value;
			}
		}

		// Token: 0x1700107E RID: 4222
		// (get) Token: 0x06009CB5 RID: 40117 RVA: 0x003E3016 File Offset: 0x003E1216
		// (set) Token: 0x06009CB6 RID: 40118 RVA: 0x003E301E File Offset: 0x003E121E
		public DataItem<float> Wood
		{
			get
			{
				return this.pWood;
			}
			set
			{
				this.pWood = value;
			}
		}

		// Token: 0x1700107F RID: 4223
		// (get) Token: 0x06009CB7 RID: 40119 RVA: 0x003E3027 File Offset: 0x003E1227
		// (set) Token: 0x06009CB8 RID: 40120 RVA: 0x003E302F File Offset: 0x003E122F
		public DataItem<float> Earth
		{
			get
			{
				return this.pEarth;
			}
			set
			{
				this.pEarth = value;
			}
		}

		// Token: 0x17001080 RID: 4224
		// (get) Token: 0x06009CB9 RID: 40121 RVA: 0x003E3038 File Offset: 0x003E1238
		// (set) Token: 0x06009CBA RID: 40122 RVA: 0x003E3040 File Offset: 0x003E1240
		public DataItem<float> Snow
		{
			get
			{
				return this.pSnow;
			}
			set
			{
				this.pSnow = value;
			}
		}

		// Token: 0x17001081 RID: 4225
		// (get) Token: 0x06009CBB RID: 40123 RVA: 0x003E3049 File Offset: 0x003E1249
		// (set) Token: 0x06009CBC RID: 40124 RVA: 0x003E3051 File Offset: 0x003E1251
		public DataItem<float> Plants
		{
			get
			{
				return this.pPlants;
			}
			set
			{
				this.pPlants = value;
			}
		}

		// Token: 0x17001082 RID: 4226
		// (get) Token: 0x06009CBD RID: 40125 RVA: 0x003E305A File Offset: 0x003E125A
		// (set) Token: 0x06009CBE RID: 40126 RVA: 0x003E3062 File Offset: 0x003E1262
		public DataItem<float> Leaves
		{
			get
			{
				return this.pLeaves;
			}
			set
			{
				this.pLeaves = value;
			}
		}

		// Token: 0x06009CBF RID: 40127 RVA: 0x003E1EE3 File Offset: 0x003E00E3
		public List<IDataItem> GetDisplayValues(bool _recursive = true)
		{
			return new List<IDataItem>();
		}

		// Token: 0x0400790E RID: 30990
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pHead;

		// Token: 0x0400790F RID: 30991
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pGlass;

		// Token: 0x04007910 RID: 30992
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pStone;

		// Token: 0x04007911 RID: 30993
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pCloth;

		// Token: 0x04007912 RID: 30994
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pConcrete;

		// Token: 0x04007913 RID: 30995
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pBoulder;

		// Token: 0x04007914 RID: 30996
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pMetal;

		// Token: 0x04007915 RID: 30997
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pWood;

		// Token: 0x04007916 RID: 30998
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pEarth;

		// Token: 0x04007917 RID: 30999
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pSnow;

		// Token: 0x04007918 RID: 31000
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pPlants;

		// Token: 0x04007919 RID: 31001
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pLeaves;

		// Token: 0x02001394 RID: 5012
		public static class Parser
		{
			// Token: 0x06009CC1 RID: 40129 RVA: 0x003E306C File Offset: 0x003E126C
			public static DamageBonusData Parse(PositionXmlElement _elem, Dictionary<PositionXmlElement, DataItem<ItemClass>> _updateLater)
			{
				string text = _elem.HasAttribute("class") ? _elem.GetAttribute("class") : "DamageBonusData";
				Type type = Type.GetType(typeof(DamageBonusData.Parser).Namespace + "." + text);
				if (type == null)
				{
					type = Type.GetType(text);
					if (type == null)
					{
						throw new InvalidValueException("Specified class \"" + text + "\" not found", _elem.LineNumber);
					}
				}
				DamageBonusData damageBonusData = (DamageBonusData)Activator.CreateInstance(type);
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (object obj in _elem.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlNodeType nodeType = xmlNode.NodeType;
					if (nodeType != XmlNodeType.Element)
					{
						if (nodeType != XmlNodeType.Comment)
						{
							throw new UnexpectedElementException("Unknown node \"" + xmlNode.NodeType.ToString() + "\" found while parsing DamageBonus", ((IXmlLineInfo)xmlNode).LineNumber);
						}
					}
					else
					{
						PositionXmlElement positionXmlElement = (PositionXmlElement)xmlNode;
						if (!DamageBonusData.Parser.knownAttributesMultiplicity.ContainsKey(positionXmlElement.Name))
						{
							throw new UnexpectedElementException("Unknown element \"" + xmlNode.Name + "\" found while parsing DamageBonus", ((IXmlLineInfo)xmlNode).LineNumber);
						}
						string name = positionXmlElement.Name;
						uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
						if (num <= 2545987019U)
						{
							if (num <= 1307099730U)
							{
								if (num != 78706450U)
								{
									if (num != 81868168U)
									{
										if (num == 1307099730U)
										{
											if (name == "Boulder")
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
												DataItem<float> pBoulder = new DataItem<float>("Boulder", startValue);
												damageBonusData.pBoulder = pBoulder;
											}
										}
									}
									else if (name == "Wood")
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
										DataItem<float> pWood = new DataItem<float>("Wood", startValue2);
										damageBonusData.pWood = pWood;
									}
								}
								else if (name == "Snow")
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
									DataItem<float> pSnow = new DataItem<float>("Snow", startValue3);
									damageBonusData.pSnow = pSnow;
								}
							}
							else if (num != 1842662042U)
							{
								if (num != 1858281043U)
								{
									if (num == 2545987019U)
									{
										if (name == "Glass")
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
											DataItem<float> pGlass = new DataItem<float>("Glass", startValue4);
											damageBonusData.pGlass = pGlass;
										}
									}
								}
								else if (name == "Plants")
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
									DataItem<float> pPlants = new DataItem<float>("Plants", startValue5);
									damageBonusData.pPlants = pPlants;
								}
							}
							else if (name == "Stone")
							{
								float startValue6;
								try
								{
									startValue6 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
								DataItem<float> pStone = new DataItem<float>("Stone", startValue6);
								damageBonusData.pStone = pStone;
							}
						}
						else if (num <= 2995012523U)
						{
							if (num != 2553495518U)
							{
								if (num != 2840670588U)
								{
									if (num == 2995012523U)
									{
										if (name == "Cloth")
										{
											float startValue7;
											try
											{
												startValue7 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
											DataItem<float> pCloth = new DataItem<float>("Cloth", startValue7);
											damageBonusData.pCloth = pCloth;
										}
									}
								}
								else if (name == "Metal")
								{
									float startValue8;
									try
									{
										startValue8 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
									DataItem<float> pMetal = new DataItem<float>("Metal", startValue8);
									damageBonusData.pMetal = pMetal;
								}
							}
							else if (name == "Concrete")
							{
								float startValue9;
								try
								{
									startValue9 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException9)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException9);
								}
								DataItem<float> pConcrete = new DataItem<float>("Concrete", startValue9);
								damageBonusData.pConcrete = pConcrete;
							}
						}
						else if (num != 2996251363U)
						{
							if (num != 3947615209U)
							{
								if (num == 4159608695U)
								{
									if (name == "Earth")
									{
										float startValue10;
										try
										{
											startValue10 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException10)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException10);
										}
										DataItem<float> pEarth = new DataItem<float>("Earth", startValue10);
										damageBonusData.pEarth = pEarth;
									}
								}
							}
							else if (name == "Leaves")
							{
								float startValue11;
								try
								{
									startValue11 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException11)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException11);
								}
								DataItem<float> pLeaves = new DataItem<float>("Leaves", startValue11);
								damageBonusData.pLeaves = pLeaves;
							}
						}
						else if (name == "Head")
						{
							float startValue12;
							try
							{
								startValue12 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
							}
							catch (Exception innerException12)
							{
								throw new InvalidValueException(string.Concat(new string[]
								{
									"Could not parse attribute \"",
									positionXmlElement.Name,
									"\" value \"",
									ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
									"\""
								}), positionXmlElement.LineNumber, innerException12);
							}
							DataItem<float> pHead = new DataItem<float>("Head", startValue12);
							damageBonusData.pHead = pHead;
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
				foreach (KeyValuePair<string, Range<int>> keyValuePair in DamageBonusData.Parser.knownAttributesMultiplicity)
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
				return damageBonusData;
			}

			// Token: 0x0400791A RID: 31002
			[PublicizedFrom(EAccessModifier.Private)]
			public static Dictionary<string, Range<int>> knownAttributesMultiplicity = new Dictionary<string, Range<int>>
			{
				{
					"Head",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Glass",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Stone",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Cloth",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Concrete",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Boulder",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Metal",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Wood",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Earth",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Snow",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Plants",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Leaves",
					new Range<int>(true, 0, true, 1)
				}
			};
		}
	}
}
