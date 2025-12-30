using System;
using System.Collections.Generic;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using XMLData.Exceptions;
using XMLData.Parsers;

namespace XMLData.Item
{
	// Token: 0x020013A3 RID: 5027
	public class UMAData : IXMLData
	{
		// Token: 0x170010F2 RID: 4338
		// (get) Token: 0x06009DBB RID: 40379 RVA: 0x003ECE8B File Offset: 0x003EB08B
		// (set) Token: 0x06009DBC RID: 40380 RVA: 0x003ECE93 File Offset: 0x003EB093
		public DataItem<string> Mesh
		{
			get
			{
				return this.pMesh;
			}
			set
			{
				this.pMesh = value;
			}
		}

		// Token: 0x170010F3 RID: 4339
		// (get) Token: 0x06009DBD RID: 40381 RVA: 0x003ECE9C File Offset: 0x003EB09C
		// (set) Token: 0x06009DBE RID: 40382 RVA: 0x003ECEA4 File Offset: 0x003EB0A4
		public DataItem<string> OverlayTints
		{
			get
			{
				return this.pOverlayTints;
			}
			set
			{
				this.pOverlayTints = value;
			}
		}

		// Token: 0x170010F4 RID: 4340
		// (get) Token: 0x06009DBF RID: 40383 RVA: 0x003ECEAD File Offset: 0x003EB0AD
		// (set) Token: 0x06009DC0 RID: 40384 RVA: 0x003ECEB5 File Offset: 0x003EB0B5
		public DataItem<string> Overlay
		{
			get
			{
				return this.pOverlay;
			}
			set
			{
				this.pOverlay = value;
			}
		}

		// Token: 0x170010F5 RID: 4341
		// (get) Token: 0x06009DC1 RID: 40385 RVA: 0x003ECEBE File Offset: 0x003EB0BE
		// (set) Token: 0x06009DC2 RID: 40386 RVA: 0x003ECEC6 File Offset: 0x003EB0C6
		public DataItem<int> Layer
		{
			get
			{
				return this.pLayer;
			}
			set
			{
				this.pLayer = value;
			}
		}

		// Token: 0x170010F6 RID: 4342
		// (get) Token: 0x06009DC3 RID: 40387 RVA: 0x003ECECF File Offset: 0x003EB0CF
		// (set) Token: 0x06009DC4 RID: 40388 RVA: 0x003ECED7 File Offset: 0x003EB0D7
		public DataItem<string> UISlot
		{
			get
			{
				return this.pUISlot;
			}
			set
			{
				this.pUISlot = value;
			}
		}

		// Token: 0x170010F7 RID: 4343
		// (get) Token: 0x06009DC5 RID: 40389 RVA: 0x003ECEE0 File Offset: 0x003EB0E0
		// (set) Token: 0x06009DC6 RID: 40390 RVA: 0x003ECEE8 File Offset: 0x003EB0E8
		public DataItem<bool> ShowHair
		{
			get
			{
				return this.pShowHair;
			}
			set
			{
				this.pShowHair = value;
			}
		}

		// Token: 0x06009DC7 RID: 40391 RVA: 0x003E1EE3 File Offset: 0x003E00E3
		public List<IDataItem> GetDisplayValues(bool _recursive = true)
		{
			return new List<IDataItem>();
		}

		// Token: 0x040079BE RID: 31166
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pMesh;

		// Token: 0x040079BF RID: 31167
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pOverlayTints;

		// Token: 0x040079C0 RID: 31168
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pOverlay;

		// Token: 0x040079C1 RID: 31169
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pLayer;

		// Token: 0x040079C2 RID: 31170
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pUISlot;

		// Token: 0x040079C3 RID: 31171
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<bool> pShowHair;

		// Token: 0x020013A4 RID: 5028
		public static class Parser
		{
			// Token: 0x06009DC9 RID: 40393 RVA: 0x003ECEF4 File Offset: 0x003EB0F4
			public static UMAData Parse(PositionXmlElement _elem, Dictionary<PositionXmlElement, DataItem<ItemClass>> _updateLater)
			{
				string text = _elem.HasAttribute("class") ? _elem.GetAttribute("class") : "UMAData";
				Type type = Type.GetType(typeof(UMAData.Parser).Namespace + "." + text);
				if (type == null)
				{
					type = Type.GetType(text);
					if (type == null)
					{
						throw new InvalidValueException("Specified class \"" + text + "\" not found", _elem.LineNumber);
					}
				}
				UMAData umadata = (UMAData)Activator.CreateInstance(type);
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (object obj in _elem.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlNodeType nodeType = xmlNode.NodeType;
					if (nodeType != XmlNodeType.Element)
					{
						if (nodeType != XmlNodeType.Comment)
						{
							throw new UnexpectedElementException("Unknown node \"" + xmlNode.NodeType.ToString() + "\" found while parsing UMA", ((IXmlLineInfo)xmlNode).LineNumber);
						}
					}
					else
					{
						PositionXmlElement positionXmlElement = (PositionXmlElement)xmlNode;
						if (!UMAData.Parser.knownAttributesMultiplicity.ContainsKey(positionXmlElement.Name))
						{
							throw new UnexpectedElementException("Unknown element \"" + xmlNode.Name + "\" found while parsing UMA", ((IXmlLineInfo)xmlNode).LineNumber);
						}
						string name = positionXmlElement.Name;
						if (!(name == "Mesh"))
						{
							if (!(name == "OverlayTints"))
							{
								if (!(name == "Overlay"))
								{
									if (!(name == "Layer"))
									{
										if (!(name == "UISlot"))
										{
											if (name == "ShowHair")
											{
												bool startValue;
												try
												{
													startValue = boolParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
												DataItem<bool> pShowHair = new DataItem<bool>("ShowHair", startValue);
												umadata.pShowHair = pShowHair;
											}
										}
										else
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
											DataItem<string> pUISlot = new DataItem<string>("UISlot", startValue2);
											umadata.pUISlot = pUISlot;
										}
									}
									else
									{
										int startValue3;
										try
										{
											startValue3 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
										DataItem<int> pLayer = new DataItem<int>("Layer", startValue3);
										umadata.pLayer = pLayer;
									}
								}
								else
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
									DataItem<string> pOverlay = new DataItem<string>("Overlay", startValue4);
									umadata.pOverlay = pOverlay;
								}
							}
							else
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
								DataItem<string> pOverlayTints = new DataItem<string>("OverlayTints", startValue5);
								umadata.pOverlayTints = pOverlayTints;
							}
						}
						else
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
							DataItem<string> pMesh = new DataItem<string>("Mesh", startValue6);
							umadata.pMesh = pMesh;
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
				foreach (KeyValuePair<string, Range<int>> keyValuePair in UMAData.Parser.knownAttributesMultiplicity)
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
				return umadata;
			}

			// Token: 0x040079C4 RID: 31172
			[PublicizedFrom(EAccessModifier.Private)]
			public static Dictionary<string, Range<int>> knownAttributesMultiplicity = new Dictionary<string, Range<int>>
			{
				{
					"Mesh",
					new Range<int>(true, 0, true, 1)
				},
				{
					"OverlayTints",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Overlay",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Layer",
					new Range<int>(true, 0, true, 1)
				},
				{
					"UISlot",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ShowHair",
					new Range<int>(true, 0, true, 1)
				}
			};
		}
	}
}
