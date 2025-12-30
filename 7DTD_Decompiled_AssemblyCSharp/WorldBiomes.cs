using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using UnityEngine;

// Token: 0x02000A8D RID: 2701
public class WorldBiomes
{
	// Token: 0x06005352 RID: 21330 RVA: 0x00215ACC File Offset: 0x00213CCC
	public WorldBiomes(XDocument _genxml, bool _instantiateReferences)
	{
		WorldBiomes.Instance = this;
		this.m_Color2BiomeMap = new Dictionary<uint, BiomeDefinition>();
		this.m_Id2BiomeArr = new BiomeDefinition[256];
		this.m_Name2BiomeMap = new CaseInsensitiveStringDictionary<BiomeDefinition>();
		this.m_PoiMap = new Dictionary<uint, PoiMapElement>();
		this.readXML(_genxml, _instantiateReferences);
	}

	// Token: 0x06005353 RID: 21331 RVA: 0x00002914 File Offset: 0x00000B14
	public void Cleanup()
	{
	}

	// Token: 0x06005354 RID: 21332 RVA: 0x00215B1E File Offset: 0x00213D1E
	public static void CleanupStatic()
	{
		if (WorldBiomes.Instance != null)
		{
			WorldBiomes.Instance.Cleanup();
		}
	}

	// Token: 0x06005355 RID: 21333 RVA: 0x00215B31 File Offset: 0x00213D31
	public int GetBiomeCount()
	{
		if (this.m_Color2BiomeMap == null)
		{
			return 0;
		}
		return this.m_Color2BiomeMap.Count;
	}

	// Token: 0x06005356 RID: 21334 RVA: 0x00215B48 File Offset: 0x00213D48
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Dictionary<uint, BiomeDefinition> GetBiomeMap()
	{
		return this.m_Color2BiomeMap;
	}

	// Token: 0x06005357 RID: 21335 RVA: 0x00215B50 File Offset: 0x00213D50
	public BiomeDefinition GetBiome(Color32 _color)
	{
		if (this.m_Color2BiomeMap.ContainsKey((uint)((int)_color.r << 16 | (int)_color.g << 8 | (int)_color.b)))
		{
			return this.m_Color2BiomeMap[(uint)((int)_color.r << 16 | (int)_color.g << 8 | (int)_color.b)];
		}
		return null;
	}

	// Token: 0x06005358 RID: 21336 RVA: 0x00215BA9 File Offset: 0x00213DA9
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BiomeDefinition GetBiome(byte _id)
	{
		return this.m_Id2BiomeArr[(int)_id];
	}

	// Token: 0x06005359 RID: 21337 RVA: 0x00215BB3 File Offset: 0x00213DB3
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGetBiome(byte _id, out BiomeDefinition _bd)
	{
		_bd = this.m_Id2BiomeArr[(int)_id];
		return _bd != null;
	}

	// Token: 0x0600535A RID: 21338 RVA: 0x00215BC4 File Offset: 0x00213DC4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BiomeDefinition GetBiome(string _name)
	{
		if (!this.m_Name2BiomeMap.ContainsKey(_name))
		{
			return null;
		}
		return this.m_Name2BiomeMap[_name];
	}

	// Token: 0x0600535B RID: 21339 RVA: 0x00215BE4 File Offset: 0x00213DE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void readXML(XDocument _xml, bool _instantiateReferences)
	{
		Array.Clear(this.m_Id2BiomeArr, 0, this.m_Id2BiomeArr.Length);
		this.m_Color2BiomeMap.Clear();
		this.m_Name2BiomeMap.Clear();
		if (BiomeDefinition.nameToId == null)
		{
			BiomeDefinition.nameToId = new Dictionary<string, byte>();
		}
		foreach (XElement element in _xml.Descendants("biomemap"))
		{
			string attribute = element.GetAttribute("name");
			if (!BiomeDefinition.nameToId.ContainsKey(attribute))
			{
				BiomeDefinition.nameToId.Add(attribute, byte.Parse(element.GetAttribute("id")));
			}
		}
		foreach (XElement xelement in _xml.Descendants("biome"))
		{
			string attribute2 = xelement.GetAttribute("name");
			if (!BiomeDefinition.nameToId.ContainsKey(attribute2))
			{
				throw new Exception("Parsing biomes. Biome with name '" + attribute2 + "' also needs an entry in the biomemap");
			}
			byte id = BiomeDefinition.nameToId[attribute2];
			BiomeDefinition biomeDefinition = this.parseBiome(id, 0, attribute2, xelement, _instantiateReferences);
			this.m_Id2BiomeArr[(int)biomeDefinition.m_Id] = biomeDefinition;
			this.m_Color2BiomeMap.Add(biomeDefinition.m_uiColor, biomeDefinition);
			this.m_Name2BiomeMap.Add(biomeDefinition.m_sBiomeName, biomeDefinition);
		}
		BiomeParticleManager.RegistrationCompleted = true;
		foreach (XElement xelement2 in _xml.Descendants("pois"))
		{
			foreach (XElement xelement3 in xelement2.Descendants("poi"))
			{
				uint num = Convert.ToUInt32(xelement3.GetAttribute("poimapcolor").Substring(1), 16);
				int iSO = 0;
				int num2 = 0;
				if (xelement3.HasAttribute("surfaceoffset"))
				{
					iSO = Convert.ToInt32(xelement3.GetAttribute("surfaceoffset"));
				}
				if (xelement3.HasAttribute("smoothness"))
				{
					int num3 = Convert.ToInt32(xelement3.GetAttribute("smoothness"));
					int num4 = (num3 < 0) ? 0 : num3;
				}
				if (xelement3.HasAttribute("starttunnel"))
				{
					num2 = Convert.ToInt32(xelement3.GetAttribute("starttunnel"));
					num2 = ((num2 < 0) ? 0 : num2);
				}
				BlockValue blockValue = BlockValue.Air;
				if (xelement3.HasAttribute("blockname"))
				{
					blockValue = (_instantiateReferences ? WorldBiomes.GetBlockValueForName(xelement3.GetAttribute("blockname")) : BlockValue.Air);
				}
				BlockValue blockBelow = BlockValue.Air;
				if (xelement3.HasAttribute("blockbelow"))
				{
					blockBelow = (_instantiateReferences ? WorldBiomes.GetBlockValueForName(xelement3.GetAttribute("blockbelow")) : BlockValue.Air);
				}
				int ypos = -1;
				if (xelement3.HasAttribute("ypos"))
				{
					ypos = int.Parse(xelement3.GetAttribute("ypos"));
				}
				int yposFill = -1;
				if (xelement3.HasAttribute("yposfill"))
				{
					yposFill = int.Parse(xelement3.GetAttribute("yposfill"));
				}
				PoiMapElement poiMapElement = new PoiMapElement(num, xelement3.GetAttribute("prefab"), blockValue, blockBelow, iSO, ypos, yposFill, num2);
				this.m_PoiMap.Add(num, poiMapElement);
				foreach (XElement xelement4 in xelement3.Elements())
				{
					if (xelement4.Name == "decal")
					{
						int texIndex = Convert.ToInt32(xelement4.GetAttribute("texture"));
						BlockFace face = (BlockFace)Convert.ToInt32(xelement4.GetAttribute("face"));
						float prob = StringParsers.ParseFloat(xelement4.GetAttribute("prob"), 0, -1, NumberStyles.Any);
						poiMapElement.decals.Add(new PoiMapDecal(texIndex, face, prob));
					}
					if (xelement4.Name == "blockontop")
					{
						blockValue = (_instantiateReferences ? WorldBiomes.GetBlockValueForName(xelement4.GetAttribute("blockname")) : BlockValue.Air);
						float prob2 = StringParsers.ParseFloat(xelement4.GetAttribute("prob"), 0, -1, NumberStyles.Any);
						int offset = xelement4.HasAttribute("offset") ? int.Parse(xelement4.GetAttribute("offset")) : 0;
						poiMapElement.blocksOnTop.Add(new PoiMapBlock(blockValue, prob2, offset));
					}
				}
			}
		}
	}

	// Token: 0x0600535C RID: 21340 RVA: 0x0021617C File Offset: 0x0021437C
	[PublicizedFrom(EAccessModifier.Private)]
	public BiomeDefinition parseBiome(byte id, byte subId, string name, XElement biomeElement, bool _instantiateReferences)
	{
		uint color = 0U;
		if (biomeElement.HasAttribute("biomemapcolor"))
		{
			color = Convert.ToUInt32(biomeElement.GetAttribute("biomemapcolor").Substring(1), 16);
		}
		int radiationLevel = 0;
		if (biomeElement.HasAttribute("radiationlevel"))
		{
			radiationLevel = int.Parse(biomeElement.GetAttribute("radiationlevel"));
		}
		string buff = null;
		if (biomeElement.HasAttribute("buff"))
		{
			buff = biomeElement.GetAttribute("buff");
		}
		BiomeDefinition biomeDefinition = new BiomeDefinition(id, subId, name, color, radiationLevel, buff);
		string attribute = biomeElement.GetAttribute("noise");
		if (attribute.Length > 0)
		{
			Vector3 vector = StringParsers.ParseVector3(attribute, 0, -1);
			biomeDefinition.noiseFreq = vector.x;
			biomeDefinition.noiseMin = vector.y;
			biomeDefinition.noiseMax = vector.z;
		}
		attribute = biomeElement.GetAttribute("noiseoffset");
		if (attribute.Length > 0)
		{
			biomeDefinition.noiseOffset = StringParsers.ParseVector2(attribute);
		}
		if (biomeElement.HasAttribute("gamestage_modifier"))
		{
			biomeDefinition.GameStageMod = StringParsers.ParseFloat(biomeElement.GetAttribute("gamestage_modifier"), 0, -1, NumberStyles.Any);
		}
		if (biomeElement.HasAttribute("gamestage_bonus"))
		{
			biomeDefinition.GameStageBonus = StringParsers.ParseFloat(biomeElement.GetAttribute("gamestage_bonus"), 0, -1, NumberStyles.Any);
		}
		if (biomeElement.HasAttribute("lootstage_modifier"))
		{
			biomeDefinition.LootStageMod = StringParsers.ParseFloat(biomeElement.GetAttribute("lootstage_modifier"), 0, -1, NumberStyles.Any);
		}
		if (biomeElement.HasAttribute("lootstage_bonus"))
		{
			biomeDefinition.LootStageBonus = StringParsers.ParseFloat(biomeElement.GetAttribute("lootstage_bonus"), 0, -1, NumberStyles.Any);
		}
		if (biomeElement.HasAttribute("lootstage_min"))
		{
			biomeDefinition.LootStageMin = StringParsers.ParseSInt32(biomeElement.GetAttribute("lootstage_min"), 0, -1, NumberStyles.Integer);
		}
		if (biomeElement.HasAttribute("lootstage_max"))
		{
			biomeDefinition.LootStageMax = StringParsers.ParseSInt32(biomeElement.GetAttribute("lootstage_max"), 0, -1, NumberStyles.Integer);
		}
		if (biomeElement.HasAttribute("difficulty"))
		{
			biomeDefinition.Difficulty = StringParsers.ParseSInt32(biomeElement.GetAttribute("difficulty"), 0, -1, NumberStyles.Integer);
		}
		foreach (XElement xelement in biomeElement.Elements())
		{
			if (xelement.Name == "subbiome")
			{
				subId += 1;
				BiomeDefinition biomeDefinition2 = this.parseBiome(id, subId, name, xelement, _instantiateReferences);
				biomeDefinition.addSubBiome(biomeDefinition2);
				if (biomeDefinition2.m_DecoBlocks.Count == 0 && biomeDefinition2.m_DecoPrefabs.Count == 0)
				{
					biomeDefinition2.m_DecoBlocks = biomeDefinition.m_DecoBlocks;
					biomeDefinition2.m_DistantDecoBlocks = biomeDefinition.m_DistantDecoBlocks;
					biomeDefinition2.m_DecoPrefabs = biomeDefinition.m_DecoPrefabs;
				}
			}
			else if (xelement.Name == "terrain")
			{
				if (!xelement.HasAttribute("class"))
				{
					throw new Exception("Attribute class missing on terrain in biome " + name);
				}
				string attribute2 = xelement.GetAttribute("class");
				Type typeWithPrefix = ReflectionHelpers.GetTypeWithPrefix("TGM", attribute2);
				if (typeWithPrefix != null)
				{
					TGMAbstract tgmabstract = (TGMAbstract)Activator.CreateInstance(typeWithPrefix);
					if (tgmabstract == null)
					{
						throw new Exception("Class '" + attribute2 + "' not found!");
					}
					foreach (XElement propertyNode in xelement.Elements("property"))
					{
						tgmabstract.properties.Add(propertyNode, true, false);
					}
					tgmabstract.Init();
					biomeDefinition.m_Terrain = tgmabstract;
				}
			}
			else if (xelement.Name == "spectrum")
			{
				if (xelement.HasAttribute("name"))
				{
					string attribute3 = xelement.GetAttribute("name");
					biomeDefinition.m_SpectrumName = attribute3;
				}
			}
			else if (xelement.Name == "weather")
			{
				this.ParseWeather(biomeDefinition, xelement);
			}
			else
			{
				if (xelement.Name == "layers")
				{
					using (IEnumerator<XElement> enumerator2 = xelement.Elements("layer").GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							XElement xelement2 = enumerator2.Current;
							int depth = -1;
							if (xelement2.HasAttribute("depth") && !xelement2.GetAttribute("depth").Equals("*"))
							{
								depth = int.Parse(xelement2.GetAttribute("depth"));
							}
							string attribute4 = xelement2.GetAttribute("blockname");
							BiomeLayer biomeLayer = new BiomeLayer(depth, new BiomeBlockDecoration(attribute4, 1f, 1f, _instantiateReferences, 0, int.MaxValue));
							biomeDefinition.AddLayer(biomeLayer);
							foreach (XElement element in xelement2.Descendants("resource"))
							{
								float prob = StringParsers.ParseFloat(element.GetAttribute("prob"), 0, -1, NumberStyles.Any);
								BiomeBlockDecoration res = new BiomeBlockDecoration(Convert.ToString(element.GetAttribute("blockname")), prob, 0f, _instantiateReferences, 0, int.MaxValue);
								biomeLayer.AddResource(res);
							}
						}
						continue;
					}
				}
				if (xelement.Name == "decorations")
				{
					using (IEnumerator<XElement> enumerator2 = xelement.Elements("decoration").GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							XElement element2 = enumerator2.Current;
							string attribute5 = element2.GetAttribute("type");
							if (attribute5.Equals("block"))
							{
								float prob2 = StringParsers.ParseFloat(element2.GetAttribute("prob"), 0, -1, NumberStyles.Any);
								float clusprob = 0f;
								string attribute6 = element2.GetAttribute("blockname");
								int randomRotateMax = element2.HasAttribute("rotatemax") ? int.Parse(element2.GetAttribute("rotatemax")) : 0;
								int checkResource = element2.HasAttribute("checkresource") ? int.Parse(element2.GetAttribute("checkresource")) : int.MaxValue;
								biomeDefinition.AddDecoBlock(new BiomeBlockDecoration(attribute6, prob2, clusprob, _instantiateReferences, randomRotateMax, checkResource));
							}
							else if (attribute5.Equals("prefab"))
							{
								float prob3 = StringParsers.ParseFloat(element2.GetAttribute("prob"), 0, -1, NumberStyles.Any);
								string attribute7 = element2.GetAttribute("name");
								if (string.IsNullOrEmpty(attribute7))
								{
									throw new Exception("Parsing biomes. No model name specified on prefab in biome '" + name + "'");
								}
								int checkResource2 = element2.HasAttribute("checkresource") ? int.Parse(element2.GetAttribute("checkresource")) : 10000;
								bool isDecorateOnSlopes = element2.HasAttribute("onslopes") && StringParsers.ParseBool(element2.GetAttribute("onslopes"), 0, -1, true);
								biomeDefinition.AddDecoPrefab(new BiomePrefabDecoration(attribute7, prob3, isDecorateOnSlopes, checkResource2));
							}
							else
							{
								if (!attribute5.Equals("terrain"))
								{
									throw new Exception("Unknown decoration type " + attribute5);
								}
								float prob4 = StringParsers.ParseFloat(element2.GetAttribute("prob"), 0, -1, NumberStyles.Any);
								string attribute8 = element2.GetAttribute("name");
								if (string.IsNullOrEmpty(attribute8))
								{
									throw new Exception("Parsing biomes. No name specified on terrain in biome '" + name + "'");
								}
								if (_instantiateReferences && !SdFile.Exists(GameIO.GetGameDir("Data/Bluffs") + "/" + attribute8 + ".tga"))
								{
									throw new Exception("Parsing biomes. Prefab with name '" + attribute8 + ".tga' not found!");
								}
								float minScale = 1f;
								float maxScale = 1f;
								string text = element2.HasAttribute("scale") ? element2.GetAttribute("scale") : null;
								if (text != null && text.IndexOf(',') > 0)
								{
									string[] array = text.Split(',', StringSplitOptions.None);
									minScale = StringParsers.ParseFloat(array[0], 0, -1, NumberStyles.Any);
									maxScale = StringParsers.ParseFloat(array[1], 0, -1, NumberStyles.Any);
								}
								else if (text != null)
								{
									maxScale = (minScale = StringParsers.ParseFloat(text, 0, -1, NumberStyles.Any));
								}
								biomeDefinition.AddBluff(new BiomeBluffDecoration(attribute8, prob4, minScale, maxScale));
							}
						}
						continue;
					}
				}
				if (xelement.Name == "replacements")
				{
					foreach (XElement element3 in xelement.Elements("replace"))
					{
						string attribute9 = element3.GetAttribute("source");
						string attribute10 = element3.GetAttribute("target");
						biomeDefinition.AddReplacement(Block.GetBlockValue(attribute9, false).type, Block.GetBlockValue(attribute10, false).type);
					}
				}
			}
		}
		biomeDefinition.SetupWeather();
		return biomeDefinition;
	}

	// Token: 0x0600535D RID: 21341 RVA: 0x00216C34 File Offset: 0x00214E34
	[PublicizedFrom(EAccessModifier.Private)]
	public void ParseWeather(BiomeDefinition _bd, XElement _xe)
	{
		string name = "?";
		_xe.ParseAttribute("name", ref name);
		float prob = 1f;
		_xe.ParseAttribute("prob", ref prob);
		float duration = 3f;
		_xe.ParseAttribute("duration", ref duration);
		Vector2 zero = Vector2.zero;
		_xe.ParseAttribute("delay", ref zero);
		string attribute = _xe.GetAttribute("buff");
		BiomeDefinition.WeatherGroup weatherGroup = _bd.AddWeatherGroup(name, prob, duration, zero, attribute);
		foreach (XElement xelement in _xe.Elements())
		{
			string localName = xelement.Name.LocalName;
			BiomeDefinition.Probabilities.ProbType probType = BiomeDefinition.Probabilities.ProbType.Count;
			Vector2 range;
			range.x = 0f;
			range.y = 100f;
			if (localName.EqualsCaseInsensitive("temperature"))
			{
				probType = BiomeDefinition.Probabilities.ProbType.Temperature;
				range.x = -50f;
				range.y = 150f;
			}
			else if (localName.EqualsCaseInsensitive("cloudthickness"))
			{
				probType = BiomeDefinition.Probabilities.ProbType.CloudThickness;
			}
			else if (localName.EqualsCaseInsensitive("precipitation"))
			{
				probType = BiomeDefinition.Probabilities.ProbType.Precipitation;
			}
			else if (localName.EqualsCaseInsensitive("fog"))
			{
				probType = BiomeDefinition.Probabilities.ProbType.Fog;
			}
			else if (localName.EqualsCaseInsensitive("wind"))
			{
				probType = BiomeDefinition.Probabilities.ProbType.Wind;
			}
			else if (localName.EqualsCaseInsensitive("particleeffect"))
			{
				string prefabName = (xelement.GetAttribute("prefab").Length > 0) ? xelement.GetAttribute("prefab") : "error";
				int num = (int)((xelement.GetAttribute("ChunkMargin").Length > 0) ? StringParsers.ParseFloat(xelement.GetAttribute("ChunkMargin"), 0, -1, NumberStyles.Any) : 8f);
				BiomeParticleManager.RegisterEffect(_bd.m_sBiomeName, prefabName, (float)num);
			}
			else if (localName.EqualsCaseInsensitive("spectrum"))
			{
				string attribute2 = xelement.GetAttribute("name");
				weatherGroup.spectrum = EnumUtils.Parse<SpectrumWeatherType>(attribute2, SpectrumWeatherType.Biome, true);
			}
			if (probType != BiomeDefinition.Probabilities.ProbType.Count)
			{
				xelement.ParseAttribute("min", ref range.x);
				xelement.ParseAttribute("max", ref range.y);
				xelement.ParseAttribute("range", ref range);
				float probability = 1f;
				xelement.ParseAttribute("prob", ref probability);
				weatherGroup.AddProbability(probType, range, probability);
			}
		}
	}

	// Token: 0x0600535E RID: 21342 RVA: 0x00216F00 File Offset: 0x00215100
	public static BlockValue GetBlockValueForName(string blockname)
	{
		ItemValue item = ItemClass.GetItem(blockname, false);
		if (item.IsEmpty())
		{
			throw new Exception("Block with name '" + blockname + "' not found!");
		}
		return item.ToBlockValue();
	}

	// Token: 0x0600535F RID: 21343 RVA: 0x00216F2C File Offset: 0x0021512C
	public PoiMapElement getPoiForColor(uint uiColor)
	{
		PoiMapElement result;
		if (this.m_PoiMap.TryGetValue(uiColor, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06005360 RID: 21344 RVA: 0x00216F4C File Offset: 0x0021514C
	public void AddPoiMapElement(PoiMapElement _newElement)
	{
		if (!this.m_PoiMap.ContainsKey(_newElement.m_uColorId))
		{
			this.m_PoiMap.Add(_newElement.m_uColorId, _newElement);
		}
	}

	// Token: 0x06005361 RID: 21345 RVA: 0x00216F74 File Offset: 0x00215174
	public int GetTotalBluffsCount()
	{
		int num = 0;
		for (int i = 0; i < this.m_Id2BiomeArr.Length; i++)
		{
			if (this.m_Id2BiomeArr[i] != null)
			{
				num += this.m_Id2BiomeArr[i].m_DecoBluffs.Count;
			}
		}
		return num;
	}

	// Token: 0x04003F8F RID: 16271
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<uint, BiomeDefinition> m_Color2BiomeMap;

	// Token: 0x04003F90 RID: 16272
	[PublicizedFrom(EAccessModifier.Private)]
	public BiomeDefinition[] m_Id2BiomeArr;

	// Token: 0x04003F91 RID: 16273
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, BiomeDefinition> m_Name2BiomeMap;

	// Token: 0x04003F92 RID: 16274
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<uint, PoiMapElement> m_PoiMap;

	// Token: 0x04003F93 RID: 16275
	public static WorldBiomes Instance;
}
