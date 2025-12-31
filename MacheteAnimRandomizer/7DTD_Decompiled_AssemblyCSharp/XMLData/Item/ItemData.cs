using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using UnityEngine.Scripting;
using XMLData.Exceptions;
using XMLData.Parsers;

namespace XMLData.Item
{
	// Token: 0x0200139B RID: 5019
	[Preserve]
	public class ItemData : IXMLParserBase, IXMLData
	{
		// Token: 0x170010DE RID: 4318
		// (get) Token: 0x06009D82 RID: 40322 RVA: 0x003E9497 File Offset: 0x003E7697
		public string Name
		{
			get
			{
				return this.pName;
			}
		}

		// Token: 0x170010DF RID: 4319
		// (get) Token: 0x06009D83 RID: 40323 RVA: 0x003E949F File Offset: 0x003E769F
		public int Id
		{
			get
			{
				return this.pId;
			}
		}

		// Token: 0x170010E0 RID: 4320
		// (get) Token: 0x06009D84 RID: 40324 RVA: 0x003E94A7 File Offset: 0x003E76A7
		// (set) Token: 0x06009D85 RID: 40325 RVA: 0x003E94AF File Offset: 0x003E76AF
		public DataItem<bool> Active
		{
			get
			{
				return this.pActive;
			}
			set
			{
				this.pActive = value;
			}
		}

		// Token: 0x170010E1 RID: 4321
		// (get) Token: 0x06009D86 RID: 40326 RVA: 0x003E94B8 File Offset: 0x003E76B8
		// (set) Token: 0x06009D87 RID: 40327 RVA: 0x003E94C0 File Offset: 0x003E76C0
		public DataItem<bool> AlwaysActive
		{
			get
			{
				return this.pAlwaysActive;
			}
			set
			{
				this.pAlwaysActive = value;
			}
		}

		// Token: 0x170010E2 RID: 4322
		// (get) Token: 0x06009D88 RID: 40328 RVA: 0x003E94C9 File Offset: 0x003E76C9
		// (set) Token: 0x06009D89 RID: 40329 RVA: 0x003E94D1 File Offset: 0x003E76D1
		public DataItem<int> FuelValue
		{
			get
			{
				return this.pFuelValue;
			}
			set
			{
				this.pFuelValue = value;
			}
		}

		// Token: 0x170010E3 RID: 4323
		// (get) Token: 0x06009D8A RID: 40330 RVA: 0x003E94DA File Offset: 0x003E76DA
		// (set) Token: 0x06009D8B RID: 40331 RVA: 0x003E94E2 File Offset: 0x003E76E2
		public DataItem<string> ImageEffectOnActive
		{
			get
			{
				return this.pImageEffectOnActive;
			}
			set
			{
				this.pImageEffectOnActive = value;
			}
		}

		// Token: 0x170010E4 RID: 4324
		// (get) Token: 0x06009D8C RID: 40332 RVA: 0x003E94EB File Offset: 0x003E76EB
		// (set) Token: 0x06009D8D RID: 40333 RVA: 0x003E94F3 File Offset: 0x003E76F3
		public DataItem<PartsData> PartTypes
		{
			get
			{
				return this.pPartTypes;
			}
			set
			{
				this.pPartTypes = value;
			}
		}

		// Token: 0x170010E5 RID: 4325
		// (get) Token: 0x06009D8E RID: 40334 RVA: 0x003E94FC File Offset: 0x003E76FC
		// (set) Token: 0x06009D8F RID: 40335 RVA: 0x003E9504 File Offset: 0x003E7704
		public DataItem<string> PlaySoundOnActive
		{
			get
			{
				return this.pPlaySoundOnActive;
			}
			set
			{
				this.pPlaySoundOnActive = value;
			}
		}

		// Token: 0x170010E6 RID: 4326
		// (get) Token: 0x06009D90 RID: 40336 RVA: 0x003E950D File Offset: 0x003E770D
		// (set) Token: 0x06009D91 RID: 40337 RVA: 0x003E9515 File Offset: 0x003E7715
		public DataItem<int> Weight
		{
			get
			{
				return this.pWeight;
			}
			set
			{
				this.pWeight = value;
			}
		}

		// Token: 0x06009D92 RID: 40338 RVA: 0x003E9520 File Offset: 0x003E7720
		public List<IDataItem> GetDisplayValues(bool _recursive = true)
		{
			List<IDataItem> list = new List<IDataItem>();
			if (_recursive && this.pPartTypes != null)
			{
				list.AddRange(this.pPartTypes.Value.GetDisplayValues(true));
			}
			for (int i = 0; i < this.pAction.Length; i++)
			{
				if (_recursive && this.pAction[i] != null)
				{
					list.AddRange(this.pAction[i].Value.GetDisplayValues(true));
				}
			}
			if (_recursive && this.pArmor != null)
			{
				list.AddRange(this.pArmor.Value.GetDisplayValues(true));
			}
			if (_recursive && this.pPreview != null)
			{
				list.AddRange(this.pPreview.Value.GetDisplayValues(true));
			}
			if (_recursive && this.pAttributes != null)
			{
				list.AddRange(this.pAttributes.Value.GetDisplayValues(true));
			}
			if (_recursive && this.pExplosion != null)
			{
				list.AddRange(this.pExplosion.Value.GetDisplayValues(true));
			}
			if (_recursive && this.pUMA != null)
			{
				list.AddRange(this.pUMA.Value.GetDisplayValues(true));
			}
			return list;
		}

		// Token: 0x04007982 RID: 31106
		[PublicizedFrom(EAccessModifier.Private)]
		public static Dictionary<string, ItemClass> pElementMap = new Dictionary<string, ItemClass>();

		// Token: 0x04007983 RID: 31107
		[PublicizedFrom(EAccessModifier.Private)]
		public static ItemClass[] pElementIndexed;

		// Token: 0x04007984 RID: 31108
		[PublicizedFrom(EAccessModifier.Protected)]
		public string pName;

		// Token: 0x04007985 RID: 31109
		[PublicizedFrom(EAccessModifier.Protected)]
		public int pId;

		// Token: 0x04007986 RID: 31110
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<bool> pAlwaysActive;

		// Token: 0x04007987 RID: 31111
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<bool> pActive;

		// Token: 0x04007988 RID: 31112
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pPlaySoundOnActive;

		// Token: 0x04007989 RID: 31113
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pImageEffectOnActive;

		// Token: 0x0400798A RID: 31114
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pMeshfile;

		// Token: 0x0400798B RID: 31115
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pMaterial;

		// Token: 0x0400798C RID: 31116
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pHoldType;

		// Token: 0x0400798D RID: 31117
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<bool> pCanhold;

		// Token: 0x0400798E RID: 31118
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pStacknumber;

		// Token: 0x0400798F RID: 31119
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pDegradation;

		// Token: 0x04007990 RID: 31120
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<bool> pDegradationBreaksAfter;

		// Token: 0x04007991 RID: 31121
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pFuelValue;

		// Token: 0x04007992 RID: 31122
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pCritChance;

		// Token: 0x04007993 RID: 31123
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pGroup;

		// Token: 0x04007994 RID: 31124
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pDamageEntityMin;

		// Token: 0x04007995 RID: 31125
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pDamageEntityMax;

		// Token: 0x04007996 RID: 31126
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSmell;

		// Token: 0x04007997 RID: 31127
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pDropScale;

		// Token: 0x04007998 RID: 31128
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pCustomIcon;

		// Token: 0x04007999 RID: 31129
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pCustomIconTint;

		// Token: 0x0400799A RID: 31130
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<EPartType> pPartType;

		// Token: 0x0400799B RID: 31131
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pWeight;

		// Token: 0x0400799C RID: 31132
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<bool> pCandrop;

		// Token: 0x0400799D RID: 31133
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<bool> pUserHidden;

		// Token: 0x0400799E RID: 31134
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pLightSource;

		// Token: 0x0400799F RID: 31135
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<bool> pThrowableDecoy;

		// Token: 0x040079A0 RID: 31136
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pFuseTime;

		// Token: 0x040079A1 RID: 31137
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<ItemClass> pMoldTarget;

		// Token: 0x040079A2 RID: 31138
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<EquipmentSlots> pEquipSlot;

		// Token: 0x040079A3 RID: 31139
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pActivateObject;

		// Token: 0x040079A4 RID: 31140
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemData.DataItemArrayRepairTools pRepairTools;

		// Token: 0x040079A5 RID: 31141
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pRepairTime;

		// Token: 0x040079A6 RID: 31142
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pRepairAmount;

		// Token: 0x040079A7 RID: 31143
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSoundDestroy;

		// Token: 0x040079A8 RID: 31144
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSoundIdle;

		// Token: 0x040079A9 RID: 31145
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSoundJammed;

		// Token: 0x040079AA RID: 31146
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<PartsData> pPartTypes;

		// Token: 0x040079AB RID: 31147
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemData.DataItemArrayAction pAction;

		// Token: 0x040079AC RID: 31148
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<ArmorData> pArmor;

		// Token: 0x040079AD RID: 31149
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<PreviewData> pPreview;

		// Token: 0x040079AE RID: 31150
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<AttributesData> pAttributes;

		// Token: 0x040079AF RID: 31151
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<ExplosionData> pExplosion;

		// Token: 0x040079B0 RID: 31152
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<UMAData> pUMA;

		// Token: 0x0200139C RID: 5020
		public class DataItemArrayRepairTools
		{
			// Token: 0x170010E7 RID: 4327
			public DataItem<string> this[int index]
			{
				get
				{
					if (index >= this.pRepairTools.Length)
					{
						throw new ArgumentOutOfRangeException("index", "index " + index.ToString() + " greater/equal than array length " + this.pRepairTools.Length.ToString());
					}
					return this.pRepairTools[index];
				}
				set
				{
					if (index >= this.pRepairTools.Length)
					{
						throw new ArgumentOutOfRangeException("index", "index " + index.ToString() + " greater/equal than array length " + this.pRepairTools.Length.ToString());
					}
					this.pRepairTools[index] = value;
				}
			}

			// Token: 0x170010E8 RID: 4328
			// (get) Token: 0x06009D97 RID: 40343 RVA: 0x003E9722 File Offset: 0x003E7922
			public int Length
			{
				get
				{
					return this.pRepairTools.Length;
				}
			}

			// Token: 0x06009D98 RID: 40344 RVA: 0x003E972C File Offset: 0x003E792C
			public DataItemArrayRepairTools(int _size)
			{
				this.pRepairTools = new DataItem<string>[_size];
			}

			// Token: 0x06009D99 RID: 40345 RVA: 0x003E9740 File Offset: 0x003E7940
			public DataItemArrayRepairTools(DataItem<string>[] _init)
			{
				this.pRepairTools = _init;
			}

			// Token: 0x040079B1 RID: 31153
			[PublicizedFrom(EAccessModifier.Private)]
			public DataItem<string>[] pRepairTools;
		}

		// Token: 0x0200139D RID: 5021
		public class DataItemArrayAction
		{
			// Token: 0x170010E9 RID: 4329
			public DataItem<ItemAction> this[int index]
			{
				get
				{
					if (index >= this.pAction.Length)
					{
						throw new ArgumentOutOfRangeException("index", "index " + index.ToString() + " greater/equal than array length " + this.pAction.Length.ToString());
					}
					return this.pAction[index];
				}
				set
				{
					if (index >= this.pAction.Length)
					{
						throw new ArgumentOutOfRangeException("index", "index " + index.ToString() + " greater/equal than array length " + this.pAction.Length.ToString());
					}
					this.pAction[index] = value;
				}
			}

			// Token: 0x170010EA RID: 4330
			// (get) Token: 0x06009D9C RID: 40348 RVA: 0x003E97F6 File Offset: 0x003E79F6
			public int Length
			{
				get
				{
					return this.pAction.Length;
				}
			}

			// Token: 0x06009D9D RID: 40349 RVA: 0x003E9800 File Offset: 0x003E7A00
			public DataItemArrayAction(int _size)
			{
				this.pAction = new DataItem<ItemAction>[_size];
			}

			// Token: 0x06009D9E RID: 40350 RVA: 0x003E9814 File Offset: 0x003E7A14
			public DataItemArrayAction(DataItem<ItemAction>[] _init)
			{
				this.pAction = _init;
			}

			// Token: 0x040079B2 RID: 31154
			[PublicizedFrom(EAccessModifier.Private)]
			public DataItem<ItemAction>[] pAction;
		}

		// Token: 0x0200139E RID: 5022
		public static class Parser
		{
			// Token: 0x06009D9F RID: 40351 RVA: 0x003E9824 File Offset: 0x003E7A24
			public static ItemClass Parse(PositionXmlElement _elem, Dictionary<PositionXmlElement, DataItem<ItemClass>> _updateLater)
			{
				string pName = ParserUtils.ParseStringAttribute(_elem, "name", true, null);
				int pId = ParserUtils.ParseIntAttribute(_elem, "id", true, 0) + ItemData.Parser.idOffset;
				string text = _elem.HasAttribute("class") ? _elem.GetAttribute("class") : "ItemClass";
				Type type = Type.GetType(typeof(ItemData.Parser).Namespace + "." + text);
				if (type == null)
				{
					type = Type.GetType(text);
					if (type == null)
					{
						throw new InvalidValueException("Specified class \"" + text + "\" not found", _elem.LineNumber);
					}
				}
				ItemClass itemClass = (ItemClass)Activator.CreateInstance(type);
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				itemClass.pName = pName;
				itemClass.pId = pId;
				List<DataItem<string>> list = new List<DataItem<string>>();
				List<DataItem<ItemAction>> list2 = new List<DataItem<ItemAction>>();
				foreach (object obj in _elem.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlNodeType nodeType = xmlNode.NodeType;
					if (nodeType != XmlNodeType.Element)
					{
						if (nodeType != XmlNodeType.Comment)
						{
							throw new UnexpectedElementException("Unknown node \"" + xmlNode.NodeType.ToString() + "\" found while parsing Item", ((IXmlLineInfo)xmlNode).LineNumber);
						}
					}
					else
					{
						PositionXmlElement positionXmlElement = (PositionXmlElement)xmlNode;
						if (!ItemData.Parser.knownAttributesMultiplicity.ContainsKey(positionXmlElement.Name))
						{
							throw new UnexpectedElementException("Unknown element \"" + xmlNode.Name + "\" found while parsing Item", ((IXmlLineInfo)xmlNode).LineNumber);
						}
						string name = positionXmlElement.Name;
						uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
						if (num <= 2292684416U)
						{
							if (num <= 955858345U)
							{
								if (num <= 529077071U)
								{
									if (num <= 175614239U)
									{
										if (num != 91525164U)
										{
											if (num == 175614239U)
											{
												if (name == "Action")
												{
													ItemAction startValue = ItemActionData.Parser.Parse(positionXmlElement, _updateLater);
													DataItem<ItemAction> item = new DataItem<ItemAction>("Action", startValue);
													list2.Add(item);
												}
											}
										}
										else if (name == "Group")
										{
											string startValue2;
											try
											{
												startValue2 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
											DataItem<string> pGroup = new DataItem<string>("Group", startValue2);
											itemClass.pGroup = pGroup;
										}
									}
									else if (num != 372600678U)
									{
										if (num != 461563084U)
										{
											if (num == 529077071U)
											{
												if (name == "Active")
												{
													bool startValue3;
													try
													{
														startValue3 = boolParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
													DataItem<bool> active = new DataItem<bool>("Active", startValue3);
													itemClass.Active = active;
												}
											}
										}
										else if (name == "Candrop")
										{
											bool startValue4;
											try
											{
												startValue4 = boolParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
											DataItem<bool> pCandrop = new DataItem<bool>("Candrop", startValue4);
											itemClass.pCandrop = pCandrop;
										}
									}
									else if (name == "DropScale")
									{
										int startValue5;
										try
										{
											startValue5 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
										DataItem<int> pDropScale = new DataItem<int>("DropScale", startValue5);
										itemClass.pDropScale = pDropScale;
									}
								}
								else if (num <= 531232375U)
								{
									if (num != 530817272U)
									{
										if (num == 531232375U)
										{
											if (name == "DegradationBreaksAfter")
											{
												bool startValue6;
												try
												{
													startValue6 = boolParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
												DataItem<bool> pDegradationBreaksAfter = new DataItem<bool>("DegradationBreaksAfter", startValue6);
												itemClass.pDegradationBreaksAfter = pDegradationBreaksAfter;
											}
										}
									}
									else if (name == "Canhold")
									{
										bool startValue7;
										try
										{
											startValue7 = boolParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
										DataItem<bool> pCanhold = new DataItem<bool>("Canhold", startValue7);
										itemClass.pCanhold = pCanhold;
									}
								}
								else if (num != 759865468U)
								{
									if (num != 942707696U)
									{
										if (num == 955858345U)
										{
											if (name == "FuseTime")
											{
												float startValue8;
												try
												{
													startValue8 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
												DataItem<float> pFuseTime = new DataItem<float>("FuseTime", startValue8);
												itemClass.pFuseTime = pFuseTime;
											}
										}
									}
									else if (name == "MoldTarget")
									{
										ItemClass startValue9;
										try
										{
											startValue9 = null;
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
										DataItem<ItemClass> dataItem = new DataItem<ItemClass>("MoldTarget", startValue9);
										_updateLater.Add(positionXmlElement, dataItem);
										itemClass.pMoldTarget = dataItem;
									}
								}
								else if (name == "PartType")
								{
									EPartType startValue10;
									try
									{
										startValue10 = EnumParser.Parse<EPartType>(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
									DataItem<EPartType> pPartType = new DataItem<EPartType>("PartType", startValue10);
									itemClass.pPartType = pPartType;
								}
							}
							else if (num <= 1432876798U)
							{
								if (num <= 1012402666U)
								{
									if (num != 960815908U)
									{
										if (num == 1012402666U)
										{
											if (name == "SoundJammed")
											{
												string startValue11;
												try
												{
													startValue11 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
												DataItem<string> pSoundJammed = new DataItem<string>("SoundJammed", startValue11);
												itemClass.pSoundJammed = pSoundJammed;
											}
										}
									}
									else if (name == "ImageEffectOnActive")
									{
										string startValue12;
										try
										{
											startValue12 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
										DataItem<string> pMeshfile = new DataItem<string>("ImageEffectOnActive", startValue12);
										itemClass.pMeshfile = pMeshfile;
									}
								}
								else if (num != 1053088598U)
								{
									if (num != 1082430025U)
									{
										if (num == 1432876798U)
										{
											if (name == "SoundIdle")
											{
												string startValue13;
												try
												{
													startValue13 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
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
												DataItem<string> pSoundIdle = new DataItem<string>("SoundIdle", startValue13);
												itemClass.pSoundIdle = pSoundIdle;
											}
										}
									}
									else if (name == "CritChance")
									{
										float startValue14;
										try
										{
											startValue14 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException13)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException13);
										}
										DataItem<float> pCritChance = new DataItem<float>("CritChance", startValue14);
										itemClass.pCritChance = pCritChance;
									}
								}
								else if (name == "CustomIconTint")
								{
									string startValue15;
									try
									{
										startValue15 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException14)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException14);
									}
									DataItem<string> pCustomIconTint = new DataItem<string>("CustomIconTint", startValue15);
									itemClass.pCustomIconTint = pCustomIconTint;
								}
							}
							else if (num <= 2079154250U)
							{
								if (num != 1534719901U)
								{
									if (num == 2079154250U)
									{
										if (name == "Meshfile")
										{
											string startValue16;
											try
											{
												startValue16 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException15)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException15);
											}
											DataItem<string> pMeshfile2 = new DataItem<string>("Meshfile", startValue16);
											itemClass.pMeshfile = pMeshfile2;
										}
									}
								}
								else if (name == "PartTypes")
								{
									PartsData startValue17 = PartsData.Parser.Parse(positionXmlElement, _updateLater);
									DataItem<PartsData> pPartTypes = new DataItem<PartsData>("PartTypes", startValue17);
									itemClass.pPartTypes = pPartTypes;
								}
							}
							else if (num != 2104753456U)
							{
								if (num != 2226667892U)
								{
									if (num == 2292684416U)
									{
										if (name == "Explosion")
										{
											ExplosionData startValue18 = ExplosionData.Parser.Parse(positionXmlElement, _updateLater);
											DataItem<ExplosionData> pExplosion = new DataItem<ExplosionData>("Explosion", startValue18);
											itemClass.pExplosion = pExplosion;
										}
									}
								}
								else if (name == "Armor")
								{
									ArmorData startValue19 = ArmorData.Parser.Parse(positionXmlElement, _updateLater);
									DataItem<ArmorData> pArmor = new DataItem<ArmorData>("Armor", startValue19);
									itemClass.pArmor = pArmor;
								}
							}
							else if (name == "HoldType")
							{
								int startValue20;
								try
								{
									startValue20 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException16)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException16);
								}
								DataItem<int> pHoldType = new DataItem<int>("HoldType", startValue20);
								itemClass.pHoldType = pHoldType;
							}
						}
						else if (num <= 3419754368U)
						{
							if (num <= 2924404423U)
							{
								if (num <= 2570065113U)
								{
									if (num != 2517612320U)
									{
										if (num == 2570065113U)
										{
											if (name == "Weight")
											{
												int startValue21;
												try
												{
													startValue21 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException17)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException17);
												}
												DataItem<int> pWeight = new DataItem<int>("Weight", startValue21);
												itemClass.pWeight = pWeight;
											}
										}
									}
									else if (name == "Smell")
									{
										string startValue22;
										try
										{
											startValue22 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException18)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException18);
										}
										DataItem<string> pSmell = new DataItem<string>("Smell", startValue22);
										itemClass.pSmell = pSmell;
									}
								}
								else if (num != 2613839563U)
								{
									if (num != 2887776777U)
									{
										if (num == 2924404423U)
										{
											if (name == "RepairTools")
											{
												string startValue23;
												try
												{
													startValue23 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException19)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException19);
												}
												DataItem<string> item2 = new DataItem<string>("RepairTools", startValue23);
												list.Add(item2);
											}
										}
									}
									else if (name == "ThrowableDecoy")
									{
										bool startValue24;
										try
										{
											startValue24 = boolParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException20)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException20);
										}
										DataItem<bool> pThrowableDecoy = new DataItem<bool>("ThrowableDecoy", startValue24);
										itemClass.pThrowableDecoy = pThrowableDecoy;
									}
								}
								else if (name == "CustomIcon")
								{
									string startValue25;
									try
									{
										startValue25 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException21)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException21);
									}
									DataItem<string> pCustomIcon = new DataItem<string>("CustomIcon", startValue25);
									itemClass.pCustomIcon = pCustomIcon;
								}
							}
							else if (num <= 3131847555U)
							{
								if (num != 2948774643U)
								{
									if (num == 3131847555U)
									{
										if (name == "Degradation")
										{
											int startValue26;
											try
											{
												startValue26 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException22)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException22);
											}
											DataItem<int> pDegradation = new DataItem<int>("Degradation", startValue26);
											itemClass.pDegradation = pDegradation;
										}
									}
								}
								else if (name == "DamageEntityMax")
								{
									int startValue27;
									try
									{
										startValue27 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException23)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException23);
									}
									DataItem<int> pDamageEntityMax = new DataItem<int>("DamageEntityMax", startValue27);
									itemClass.pDamageEntityMax = pDamageEntityMax;
								}
							}
							else if (num != 3319162189U)
							{
								if (num != 3365715102U)
								{
									if (num == 3419754368U)
									{
										if (name == "Material")
										{
											string startValue28;
											try
											{
												startValue28 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException24)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException24);
											}
											DataItem<string> pMaterial = new DataItem<string>("Material", startValue28);
											itemClass.pMaterial = pMaterial;
										}
									}
								}
								else if (name == "Stacknumber")
								{
									int startValue29;
									try
									{
										startValue29 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException25)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException25);
									}
									DataItem<int> pStacknumber = new DataItem<int>("Stacknumber", startValue29);
									itemClass.pStacknumber = pStacknumber;
								}
							}
							else if (name == "DamageEntityMin")
							{
								int startValue30;
								try
								{
									startValue30 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException26)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException26);
								}
								DataItem<int> pDamageEntityMin = new DataItem<int>("DamageEntityMin", startValue30);
								itemClass.pDamageEntityMin = pDamageEntityMin;
							}
						}
						else if (num <= 3694724757U)
						{
							if (num <= 3463560063U)
							{
								if (num != 3430864052U)
								{
									if (num == 3463560063U)
									{
										if (name == "EquipSlot")
										{
											EquipmentSlots startValue31;
											try
											{
												startValue31 = EnumParser.Parse<EquipmentSlots>(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException27)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException27);
											}
											DataItem<EquipmentSlots> pEquipSlot = new DataItem<EquipmentSlots>("EquipSlot", startValue31);
											itemClass.pEquipSlot = pEquipSlot;
										}
									}
								}
								else if (name == "Attributes")
								{
									AttributesData startValue32 = AttributesData.Parser.Parse(positionXmlElement, _updateLater);
									DataItem<AttributesData> pAttributes = new DataItem<AttributesData>("Attributes", startValue32);
									itemClass.pAttributes = pAttributes;
								}
							}
							else if (num != 3519818983U)
							{
								if (num != 3646476408U)
								{
									if (num == 3694724757U)
									{
										if (name == "RepairTime")
										{
											float startValue33;
											try
											{
												startValue33 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException28)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException28);
											}
											DataItem<float> pRepairTime = new DataItem<float>("RepairTime", startValue33);
											itemClass.pRepairTime = pRepairTime;
										}
									}
								}
								else if (name == "RepairAmount")
								{
									int startValue34;
									try
									{
										startValue34 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException29)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException29);
									}
									DataItem<int> pRepairAmount = new DataItem<int>("RepairAmount", startValue34);
									itemClass.pRepairAmount = pRepairAmount;
								}
							}
							else if (name == "ActivateObject")
							{
								string startValue35;
								try
								{
									startValue35 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException30)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException30);
								}
								DataItem<string> pActivateObject = new DataItem<string>("ActivateObject", startValue35);
								itemClass.pActivateObject = pActivateObject;
							}
						}
						else if (num <= 3912538170U)
						{
							if (num != 3912144422U)
							{
								if (num == 3912538170U)
								{
									if (name == "UserHidden")
									{
										bool startValue36;
										try
										{
											startValue36 = boolParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException31)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException31);
										}
										DataItem<bool> pUserHidden = new DataItem<bool>("UserHidden", startValue36);
										itemClass.pUserHidden = pUserHidden;
									}
								}
							}
							else if (name == "SoundDestroy")
							{
								string startValue37;
								try
								{
									startValue37 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException32)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException32);
								}
								DataItem<string> pSoundDestroy = new DataItem<string>("SoundDestroy", startValue37);
								itemClass.pSoundDestroy = pSoundDestroy;
							}
						}
						else if (num != 4235238322U)
						{
							if (num != 4258942199U)
							{
								if (num == 4293145386U)
								{
									if (name == "LightSource")
									{
										string startValue38;
										try
										{
											startValue38 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException33)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException33);
										}
										DataItem<string> pLightSource = new DataItem<string>("LightSource", startValue38);
										itemClass.pLightSource = pLightSource;
									}
								}
							}
							else if (name == "Preview")
							{
								PreviewData startValue39 = PreviewData.Parser.Parse(positionXmlElement, _updateLater);
								DataItem<PreviewData> pPreview = new DataItem<PreviewData>("Preview", startValue39);
								itemClass.pPreview = pPreview;
							}
						}
						else if (name == "FuelValue")
						{
							int startValue40;
							try
							{
								startValue40 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
							}
							catch (Exception innerException34)
							{
								throw new InvalidValueException(string.Concat(new string[]
								{
									"Could not parse attribute \"",
									positionXmlElement.Name,
									"\" value \"",
									ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
									"\""
								}), positionXmlElement.LineNumber, innerException34);
							}
							DataItem<int> pFuelValue = new DataItem<int>("FuelValue", startValue40);
							itemClass.pFuelValue = pFuelValue;
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
				if (!dictionary.ContainsKey("Stacknumber"))
				{
					int startValue41;
					try
					{
						startValue41 = intParser.Parse("64");
					}
					catch (Exception)
					{
						throw new InvalidValueException("Default value \"64\" for attribute \"Stacknumber\" could not be parsed", -1);
					}
					DataItem<int> pStacknumber2 = new DataItem<int>("Stacknumber", startValue41);
					itemClass.pStacknumber = pStacknumber2;
					dictionary["Stacknumber"] = 1;
				}
				if (!dictionary.ContainsKey("Degradation"))
				{
					int startValue42;
					try
					{
						startValue42 = intParser.Parse("0");
					}
					catch (Exception)
					{
						throw new InvalidValueException("Default value \"0\" for attribute \"Degradation\" could not be parsed", -1);
					}
					DataItem<int> pDegradation2 = new DataItem<int>("Degradation", startValue42);
					itemClass.pDegradation = pDegradation2;
					dictionary["Degradation"] = 1;
				}
				if (!dictionary.ContainsKey("DegradationBreaksAfter"))
				{
					bool startValue43;
					try
					{
						startValue43 = boolParser.Parse("true");
					}
					catch (Exception)
					{
						throw new InvalidValueException("Default value \"true\" for attribute \"DegradationBreaksAfter\" could not be parsed", -1);
					}
					DataItem<bool> pDegradationBreaksAfter2 = new DataItem<bool>("DegradationBreaksAfter", startValue43);
					itemClass.pDegradationBreaksAfter = pDegradationBreaksAfter2;
					dictionary["DegradationBreaksAfter"] = 1;
				}
				if (!dictionary.ContainsKey("CritChance"))
				{
					float startValue44;
					try
					{
						startValue44 = floatParser.Parse("0");
					}
					catch (Exception)
					{
						throw new InvalidValueException("Default value \"0\" for attribute \"CritChance\" could not be parsed", -1);
					}
					DataItem<float> pCritChance2 = new DataItem<float>("CritChance", startValue44);
					itemClass.pCritChance = pCritChance2;
					dictionary["CritChance"] = 1;
				}
				if (!dictionary.ContainsKey("Group"))
				{
					string startValue45;
					try
					{
						startValue45 = stringParser.Parse("Miscellaneous");
					}
					catch (Exception)
					{
						throw new InvalidValueException("Default value \"Miscellaneous\" for attribute \"Group\" could not be parsed", -1);
					}
					DataItem<string> pGroup2 = new DataItem<string>("Group", startValue45);
					itemClass.pGroup = pGroup2;
					dictionary["Group"] = 1;
				}
				if (!dictionary.ContainsKey("PartType"))
				{
					EPartType startValue46;
					try
					{
						startValue46 = EnumParser.Parse<EPartType>("None");
					}
					catch (Exception)
					{
						throw new InvalidValueException("Default value \"None\" for attribute \"PartType\" could not be parsed", -1);
					}
					DataItem<EPartType> pPartType2 = new DataItem<EPartType>("PartType", startValue46);
					itemClass.pPartType = pPartType2;
					dictionary["PartType"] = 1;
				}
				if (!dictionary.ContainsKey("UserHidden"))
				{
					bool startValue47;
					try
					{
						startValue47 = boolParser.Parse("false");
					}
					catch (Exception)
					{
						throw new InvalidValueException("Default value \"false\" for attribute \"UserHidden\" could not be parsed", -1);
					}
					DataItem<bool> pUserHidden2 = new DataItem<bool>("UserHidden", startValue47);
					itemClass.pUserHidden = pUserHidden2;
					dictionary["UserHidden"] = 1;
				}
				if (!dictionary.ContainsKey("ThrowableDecoy"))
				{
					bool startValue48;
					try
					{
						startValue48 = boolParser.Parse("false");
					}
					catch (Exception)
					{
						throw new InvalidValueException("Default value \"false\" for attribute \"ThrowableDecoy\" could not be parsed", -1);
					}
					DataItem<bool> pThrowableDecoy2 = new DataItem<bool>("ThrowableDecoy", startValue48);
					itemClass.pThrowableDecoy = pThrowableDecoy2;
					dictionary["ThrowableDecoy"] = 1;
				}
				if (!dictionary.ContainsKey("EquipSlot"))
				{
					EquipmentSlots startValue49;
					try
					{
						startValue49 = EnumParser.Parse<EquipmentSlots>("None");
					}
					catch (Exception)
					{
						throw new InvalidValueException("Default value \"None\" for attribute \"EquipSlot\" could not be parsed", -1);
					}
					DataItem<EquipmentSlots> pEquipSlot2 = new DataItem<EquipmentSlots>("EquipSlot", startValue49);
					itemClass.pEquipSlot = pEquipSlot2;
					dictionary["EquipSlot"] = 1;
				}
				foreach (KeyValuePair<string, Range<int>> keyValuePair in ItemData.Parser.knownAttributesMultiplicity)
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
				itemClass.pRepairTools = new ItemData.DataItemArrayRepairTools(list.ToArray());
				itemClass.pAction = new ItemData.DataItemArrayAction(list2.ToArray());
				return itemClass;
			}

			// Token: 0x06009DA0 RID: 40352 RVA: 0x003EBA90 File Offset: 0x003E9C90
			public static List<ItemClass> ParseXml(string _filename, string _content, bool _clearFirst = true, bool _validateOnly = false)
			{
				PositionXmlDocument positionXmlDocument = new PositionXmlDocument();
				if (_clearFirst && !_validateOnly)
				{
					ItemData.Parser.Clear();
				}
				try
				{
					using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(_content ?? "")))
					{
						positionXmlDocument.Load(XmlReader.Create(stream));
					}
				}
				catch (XmlException e)
				{
					Log.Error("Failed parsing " + _filename + ":");
					Log.Exception(e);
					return null;
				}
				XmlNode documentElement = positionXmlDocument.DocumentElement;
				List<ItemClass> list = new List<ItemClass>();
				Dictionary<PositionXmlElement, DataItem<ItemClass>> updateLater = new Dictionary<PositionXmlElement, DataItem<ItemClass>>();
				foreach (object obj in documentElement.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlNodeType nodeType = xmlNode.NodeType;
					if (nodeType != XmlNodeType.Element)
					{
						if (nodeType != XmlNodeType.Comment)
						{
							Log.Error("Unexpected XML node: " + xmlNode.NodeType.ToString() + " at line " + ((IXmlLineInfo)xmlNode).LineNumber.ToString());
						}
					}
					else if (xmlNode.Name == "Item")
					{
						ItemClass itemClass = ItemData.Parser.Parse((PositionXmlElement)xmlNode, updateLater);
						if (itemClass != null)
						{
							list.Add(itemClass);
						}
					}
					else
					{
						Log.Error(string.Format("Unknown element found: {0} (file {1}, line {2})", xmlNode.Name, _filename, ((IXmlLineInfo)xmlNode).LineNumber));
					}
				}
				if (!_validateOnly)
				{
					ItemData.Parser.FillLists(_filename, list, _clearFirst);
					ItemData.Parser.UpdateXmlRefs(_filename, updateLater);
				}
				return list;
			}

			// Token: 0x06009DA1 RID: 40353 RVA: 0x003EBC48 File Offset: 0x003E9E48
			[PublicizedFrom(EAccessModifier.Private)]
			public static void Clear()
			{
				ItemData.pElementMap.Clear();
				ItemData.pElementIndexed = null;
			}

			// Token: 0x06009DA2 RID: 40354 RVA: 0x003EBC5C File Offset: 0x003E9E5C
			[PublicizedFrom(EAccessModifier.Private)]
			public static void FillLists(string _filename, List<ItemClass> _entries, bool _clearFirst)
			{
				int num = -1;
				HashSet<int> hashSet = new HashSet<int>();
				foreach (ItemClass itemClass in _entries)
				{
					if (itemClass.Id > num)
					{
						num = itemClass.Id;
					}
					if (hashSet.Contains(itemClass.Id))
					{
						Log.Error(string.Format("Duplicate index: {0} in {1}", itemClass.Id, _filename));
					}
					hashSet.Add(itemClass.Id);
				}
				if (!_clearFirst && num >= ItemData.pElementIndexed.Length)
				{
					ItemClass[] array = new ItemClass[num + 1];
					Array.Copy(ItemData.pElementIndexed, array, ItemData.pElementIndexed.Length);
					ItemData.pElementIndexed = array;
				}
				else if (_clearFirst)
				{
					ItemData.pElementIndexed = new ItemClass[num + 1];
				}
				foreach (ItemClass itemClass2 in _entries)
				{
					if (ItemData.pElementIndexed[itemClass2.Id] != null)
					{
						Log.Warning(string.Format("Overwriting existing element index: {0} in {1}", itemClass2.Id, _filename));
					}
					ItemData.pElementIndexed[itemClass2.Id] = itemClass2;
					if (ItemData.pElementMap.ContainsKey(itemClass2.Name))
					{
						Log.Warning(string.Format("Overwriting existing element name: {0} in {1}", itemClass2.Name, _filename));
					}
					ItemData.pElementMap[itemClass2.Name] = itemClass2;
				}
			}

			// Token: 0x06009DA3 RID: 40355 RVA: 0x003EBDE8 File Offset: 0x003E9FE8
			[PublicizedFrom(EAccessModifier.Private)]
			public static void UpdateXmlRefs(string _filename, Dictionary<PositionXmlElement, DataItem<ItemClass>> _updateLater)
			{
				foreach (KeyValuePair<PositionXmlElement, DataItem<ItemClass>> keyValuePair in _updateLater)
				{
					string text = ParserUtils.ParseStringAttribute(keyValuePair.Key, "value", true, null);
					if (!ItemData.pElementMap.ContainsKey(text))
					{
						throw new InvalidValueException(string.Concat(new string[]
						{
							"Element with name \"",
							text,
							"\" for attribute \"",
							keyValuePair.Value.Name,
							"\" not found (referencing an XML entry by name which is not defined)"
						}), keyValuePair.Key.LineNumber);
					}
					keyValuePair.Value.Value = ItemData.pElementMap[text];
				}
			}

			// Token: 0x040079B3 RID: 31155
			public static int idOffset = 0;

			// Token: 0x040079B4 RID: 31156
			[PublicizedFrom(EAccessModifier.Private)]
			public static Dictionary<string, Range<int>> knownAttributesMultiplicity = new Dictionary<string, Range<int>>
			{
				{
					"Active",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ImageEffectOnActive",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Meshfile",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Material",
					new Range<int>(true, 1, true, 1)
				},
				{
					"HoldType",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Canhold",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Stacknumber",
					new Range<int>(true, 1, true, 1)
				},
				{
					"Degradation",
					new Range<int>(true, 1, true, 1)
				},
				{
					"DegradationBreaksAfter",
					new Range<int>(true, 1, true, 1)
				},
				{
					"FuelValue",
					new Range<int>(true, 0, true, 1)
				},
				{
					"CritChance",
					new Range<int>(true, 1, true, 1)
				},
				{
					"Group",
					new Range<int>(true, 1, true, 1)
				},
				{
					"DamageEntityMin",
					new Range<int>(true, 0, true, 1)
				},
				{
					"DamageEntityMax",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Smell",
					new Range<int>(true, 0, true, 1)
				},
				{
					"DropScale",
					new Range<int>(true, 0, true, 1)
				},
				{
					"CustomIcon",
					new Range<int>(true, 0, true, 1)
				},
				{
					"CustomIconTint",
					new Range<int>(true, 0, true, 1)
				},
				{
					"PartType",
					new Range<int>(true, 1, true, 1)
				},
				{
					"Weight",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Candrop",
					new Range<int>(true, 0, true, 1)
				},
				{
					"UserHidden",
					new Range<int>(true, 1, true, 1)
				},
				{
					"LightSource",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ThrowableDecoy",
					new Range<int>(true, 1, true, 1)
				},
				{
					"FuseTime",
					new Range<int>(true, 0, true, 1)
				},
				{
					"MoldTarget",
					new Range<int>(true, 0, true, 1)
				},
				{
					"EquipSlot",
					new Range<int>(true, 1, true, 1)
				},
				{
					"ActivateObject",
					new Range<int>(true, 0, true, 1)
				},
				{
					"RepairTools",
					new Range<int>(true, 0, false, 0)
				},
				{
					"RepairTime",
					new Range<int>(true, 0, true, 1)
				},
				{
					"RepairAmount",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SoundDestroy",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SoundIdle",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SoundJammed",
					new Range<int>(true, 0, true, 1)
				},
				{
					"PartTypes",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Action",
					new Range<int>(true, 0, true, 5)
				},
				{
					"Armor",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Preview",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Attributes",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Explosion",
					new Range<int>(true, 0, true, 1)
				},
				{
					"UMA",
					new Range<int>(true, 0, true, 1)
				}
			};
		}
	}
}
