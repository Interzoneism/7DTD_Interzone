using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000A81 RID: 2689
public class BiomeDefinition
{
	// Token: 0x06005324 RID: 21284 RVA: 0x00214F71 File Offset: 0x00213171
	public static string LocalizedBiomeName(BiomeDefinition.BiomeType _biomeType)
	{
		return Localization.Get("biome_" + _biomeType.ToStringCached<BiomeDefinition.BiomeType>(), false);
	}

	// Token: 0x06005325 RID: 21285 RVA: 0x00214F8C File Offset: 0x0021318C
	public BiomeDefinition(byte _id, byte _subId, string _name, uint _color, int _radiationLevel, string _buff)
	{
		this.m_Id = _id;
		this.m_BiomeType = (BiomeDefinition.BiomeType)(Enum.IsDefined(typeof(BiomeDefinition.BiomeType), (int)this.m_Id) ? this.m_Id : 0);
		this.subId = _subId;
		this.m_sBiomeName = _name;
		this.LocalizedName = Localization.Get("biome_" + _name, false);
		this.m_SpectrumName = this.m_sBiomeName;
		this.m_uiColor = _color;
		this.m_RadiationLevel = _radiationLevel;
		this.m_Terrain = null;
		this.m_Layers = new List<BiomeLayer>();
		this.m_DecoBlocks = new List<BiomeBlockDecoration>();
		this.m_DistantDecoBlocks = new List<BiomeBlockDecoration>();
		this.m_DecoPrefabs = new List<BiomePrefabDecoration>();
		this.m_DecoBluffs = new List<BiomeBluffDecoration>();
		this.Buff = _buff;
		this.InitWeather();
	}

	// Token: 0x06005326 RID: 21286 RVA: 0x002150C1 File Offset: 0x002132C1
	public void AddLayer(BiomeLayer _layer)
	{
		this.m_Layers.Add(_layer);
		this.TotalLayerDepth += _layer.m_Depth;
	}

	// Token: 0x06005327 RID: 21287 RVA: 0x002150E4 File Offset: 0x002132E4
	public void AddDecoBlock(BiomeBlockDecoration _deco)
	{
		this.m_DecoBlocks.Add(_deco);
		if (Block.BlocksLoaded)
		{
			Block block = _deco.blockValues[0].Block;
			if (block != null && block.IsDistantDecoration)
			{
				this.m_DistantDecoBlocks.Add(_deco);
			}
		}
	}

	// Token: 0x06005328 RID: 21288 RVA: 0x0021512D File Offset: 0x0021332D
	public void AddDecoPrefab(BiomePrefabDecoration _deco)
	{
		this.m_DecoPrefabs.Add(_deco);
	}

	// Token: 0x06005329 RID: 21289 RVA: 0x0021513B File Offset: 0x0021333B
	public void AddBluff(BiomeBluffDecoration _deco)
	{
		this.m_DecoBluffs.Add(_deco);
	}

	// Token: 0x0600532A RID: 21290 RVA: 0x00215149 File Offset: 0x00213349
	public void AddReplacement(int _sourceId, int _targetId)
	{
		this.Replacements[_sourceId] = _targetId;
	}

	// Token: 0x0600532B RID: 21291 RVA: 0x00215158 File Offset: 0x00213358
	public void addSubBiome(BiomeDefinition _subbiome)
	{
		this.subbiomes.Add(_subbiome);
	}

	// Token: 0x0600532C RID: 21292 RVA: 0x00215166 File Offset: 0x00213366
	public override bool Equals(object obj)
	{
		return obj is BiomeDefinition && ((BiomeDefinition)obj).m_Id == this.m_Id;
	}

	// Token: 0x0600532D RID: 21293 RVA: 0x00215185 File Offset: 0x00213385
	public override int GetHashCode()
	{
		return (int)this.m_Id;
	}

	// Token: 0x0600532E RID: 21294 RVA: 0x0021518D File Offset: 0x0021338D
	public override string ToString()
	{
		return this.m_sBiomeName;
	}

	// Token: 0x0600532F RID: 21295 RVA: 0x00215195 File Offset: 0x00213395
	public static uint GetBiomeColor(BiomeDefinition.BiomeType _type)
	{
		return BiomeDefinition.BiomeColors[(int)_type];
	}

	// Token: 0x06005330 RID: 21296 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitWeather()
	{
	}

	// Token: 0x06005331 RID: 21297 RVA: 0x002151A0 File Offset: 0x002133A0
	public BiomeDefinition.WeatherGroup AddWeatherGroup(string _name, float _prob, float _duration, Vector2 _delay, string _buffName)
	{
		BiomeDefinition.WeatherGroup weatherGroup = new BiomeDefinition.WeatherGroup();
		weatherGroup.name = _name;
		weatherGroup.stormLevel = ((!_name.StartsWith("storm")) ? 0 : (_name.Contains("build") ? 1 : 2));
		weatherGroup.prob = _prob;
		weatherGroup.duration = (int)(_duration * 1000f);
		weatherGroup.delay.x = (int)(_delay.x * 1000f);
		weatherGroup.delay.y = (int)(_delay.y * 1000f);
		if (!string.IsNullOrEmpty(_buffName))
		{
			weatherGroup.buffName = _buffName;
		}
		this.weatherGroups.Add(weatherGroup);
		return weatherGroup;
	}

	// Token: 0x06005332 RID: 21298 RVA: 0x00215248 File Offset: 0x00213448
	public void SetupWeather()
	{
		float num = 0f;
		for (int i = 0; i < this.weatherGroups.Count; i++)
		{
			BiomeDefinition.WeatherGroup weatherGroup = this.weatherGroups[i];
			num += weatherGroup.prob;
			weatherGroup.probabilities.Normalize();
		}
		num += 1E-06f;
		for (int j = 0; j < this.weatherGroups.Count; j++)
		{
			this.weatherGroups[j].prob /= num;
		}
	}

	// Token: 0x06005333 RID: 21299 RVA: 0x002152C9 File Offset: 0x002134C9
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float WeatherGetValue(BiomeDefinition.Probabilities.ProbType _type)
	{
		return this.weatherValues[(int)_type];
	}

	// Token: 0x06005334 RID: 21300 RVA: 0x002152D3 File Offset: 0x002134D3
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void WeatherSetValue(BiomeDefinition.Probabilities.ProbType _type, float _value)
	{
		this.weatherValues[(int)_type] = _value;
	}

	// Token: 0x06005335 RID: 21301 RVA: 0x002152E0 File Offset: 0x002134E0
	public void WeatherRandomize(float _rand)
	{
		float num = 0f;
		for (int i = 0; i < this.weatherGroups.Count; i++)
		{
			BiomeDefinition.WeatherGroup weatherGroup = this.weatherGroups[i];
			num += weatherGroup.prob;
			if (_rand < num)
			{
				this.SelectWeatherGroup(i);
				return;
			}
		}
	}

	// Token: 0x06005336 RID: 21302 RVA: 0x0021532C File Offset: 0x0021352C
	public bool WeatherRandomize(string _weatherGroup)
	{
		int num = this.FindWeatherGroupIndex(_weatherGroup);
		if (num >= 0)
		{
			this.SelectWeatherGroup(num);
			return true;
		}
		return false;
	}

	// Token: 0x06005337 RID: 21303 RVA: 0x00215350 File Offset: 0x00213550
	public int WeatherGetDuration(string _weatherGroup)
	{
		BiomeDefinition.WeatherGroup weatherGroup = this.FindWeatherGroup(_weatherGroup);
		if (weatherGroup != null)
		{
			return weatherGroup.duration;
		}
		return 0;
	}

	// Token: 0x06005338 RID: 21304 RVA: 0x00215370 File Offset: 0x00213570
	public int WeatherGetDuration(string _weatherGroup, out Vector2i _delay)
	{
		BiomeDefinition.WeatherGroup weatherGroup = this.FindWeatherGroup(_weatherGroup);
		if (weatherGroup != null)
		{
			_delay = weatherGroup.delay;
			return weatherGroup.duration;
		}
		_delay = Vector2i.zero;
		return 0;
	}

	// Token: 0x06005339 RID: 21305 RVA: 0x002153A8 File Offset: 0x002135A8
	[PublicizedFrom(EAccessModifier.Private)]
	public BiomeDefinition.WeatherGroup FindWeatherGroup(string _name)
	{
		for (int i = 0; i < this.weatherGroups.Count; i++)
		{
			BiomeDefinition.WeatherGroup weatherGroup = this.weatherGroups[i];
			if (weatherGroup.name == _name)
			{
				return weatherGroup;
			}
		}
		return null;
	}

	// Token: 0x0600533A RID: 21306 RVA: 0x002153EC File Offset: 0x002135EC
	[PublicizedFrom(EAccessModifier.Private)]
	public int FindWeatherGroupIndex(string _name)
	{
		for (int i = 0; i < this.weatherGroups.Count; i++)
		{
			if (this.weatherGroups[i].name == _name)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x0600533B RID: 21307 RVA: 0x0021542C File Offset: 0x0021362C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SelectWeatherGroup(int _index)
	{
		BiomeDefinition.WeatherGroup weatherGroup = this.weatherGroups[_index];
		this.weatherName = weatherGroup.name;
		this.weatherSpectrum = weatherGroup.spectrum;
		this.currentWeatherGroupIndex = _index;
		this.currentWeatherGroup = weatherGroup;
		for (int i = 0; i < 5; i++)
		{
			float randomValue = weatherGroup.probabilities.GetRandomValue((BiomeDefinition.Probabilities.ProbType)i);
			this.weatherValues[i] = randomValue;
		}
	}

	// Token: 0x0600533C RID: 21308 RVA: 0x00215490 File Offset: 0x00213690
	public void SetWeatherGroup(int _index)
	{
		this.currentWeatherGroupIndex = _index;
		BiomeDefinition.WeatherGroup weatherGroup = this.weatherGroups[_index];
		this.currentWeatherGroup = weatherGroup;
		this.weatherSpectrum = weatherGroup.spectrum;
	}

	// Token: 0x04003F26 RID: 16166
	public float currentPlayerIntensity;

	// Token: 0x04003F27 RID: 16167
	public const string BiomeNameLocalizationPrefix = "biome_";

	// Token: 0x04003F28 RID: 16168
	public static string[] BiomeNames = new string[]
	{
		"any",
		"snow",
		"forest",
		"pine_forest",
		"plains",
		"desert",
		"water",
		"radiated",
		"wasteland",
		"burnt_forest",
		"city",
		"city_wasteland",
		"wasteland_hub",
		"caveFloor",
		"caveCeiling"
	};

	// Token: 0x04003F29 RID: 16169
	public static uint[] BiomeColors = new uint[]
	{
		0U,
		16777215U,
		0U,
		16384U,
		0U,
		16770167U,
		25599U,
		0U,
		16754688U,
		12189951U,
		8421504U,
		12632256U,
		10526880U,
		0U,
		0U
	};

	// Token: 0x04003F2A RID: 16170
	public readonly byte m_Id;

	// Token: 0x04003F2B RID: 16171
	public readonly BiomeDefinition.BiomeType m_BiomeType;

	// Token: 0x04003F2C RID: 16172
	public byte subId;

	// Token: 0x04003F2D RID: 16173
	public readonly string m_sBiomeName;

	// Token: 0x04003F2E RID: 16174
	public readonly string LocalizedName;

	// Token: 0x04003F2F RID: 16175
	public uint m_uiColor;

	// Token: 0x04003F30 RID: 16176
	public string m_SpectrumName;

	// Token: 0x04003F31 RID: 16177
	public static Dictionary<string, byte> nameToId;

	// Token: 0x04003F32 RID: 16178
	public int m_RadiationLevel;

	// Token: 0x04003F33 RID: 16179
	public int Difficulty = 1;

	// Token: 0x04003F34 RID: 16180
	public List<BiomeLayer> m_Layers;

	// Token: 0x04003F35 RID: 16181
	public List<BiomeBlockDecoration> m_DecoBlocks;

	// Token: 0x04003F36 RID: 16182
	public List<BiomeBlockDecoration> m_DistantDecoBlocks;

	// Token: 0x04003F37 RID: 16183
	public List<BiomePrefabDecoration> m_DecoPrefabs;

	// Token: 0x04003F38 RID: 16184
	public List<BiomeBluffDecoration> m_DecoBluffs;

	// Token: 0x04003F39 RID: 16185
	public List<BiomeDefinition.WeatherGroup> weatherGroups = new List<BiomeDefinition.WeatherGroup>();

	// Token: 0x04003F3A RID: 16186
	public string weatherName;

	// Token: 0x04003F3B RID: 16187
	public int currentWeatherGroupIndex;

	// Token: 0x04003F3C RID: 16188
	public BiomeDefinition.WeatherGroup currentWeatherGroup;

	// Token: 0x04003F3D RID: 16189
	public SpectrumWeatherType weatherSpectrum;

	// Token: 0x04003F3E RID: 16190
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] weatherValues = new float[5];

	// Token: 0x04003F3F RID: 16191
	public TGMAbstract m_Terrain;

	// Token: 0x04003F40 RID: 16192
	public int TotalLayerDepth;

	// Token: 0x04003F41 RID: 16193
	public List<BiomeDefinition> subbiomes = new List<BiomeDefinition>();

	// Token: 0x04003F42 RID: 16194
	public float noiseFreq = 0.03f;

	// Token: 0x04003F43 RID: 16195
	public float noiseMin = 0.2f;

	// Token: 0x04003F44 RID: 16196
	public float noiseMax = 1f;

	// Token: 0x04003F45 RID: 16197
	public Vector2 noiseOffset;

	// Token: 0x04003F46 RID: 16198
	public Dictionary<int, int> Replacements = new Dictionary<int, int>();

	// Token: 0x04003F47 RID: 16199
	public float GameStageMod;

	// Token: 0x04003F48 RID: 16200
	public float GameStageBonus;

	// Token: 0x04003F49 RID: 16201
	public float LootStageMod;

	// Token: 0x04003F4A RID: 16202
	public float LootStageBonus;

	// Token: 0x04003F4B RID: 16203
	public int LootStageMin = -1;

	// Token: 0x04003F4C RID: 16204
	public int LootStageMax = -1;

	// Token: 0x04003F4D RID: 16205
	public string Buff;

	// Token: 0x02000A82 RID: 2690
	public enum BiomeType
	{
		// Token: 0x04003F4F RID: 16207
		Any,
		// Token: 0x04003F50 RID: 16208
		Snow,
		// Token: 0x04003F51 RID: 16209
		Forest,
		// Token: 0x04003F52 RID: 16210
		PineForest,
		// Token: 0x04003F53 RID: 16211
		Plains,
		// Token: 0x04003F54 RID: 16212
		Desert,
		// Token: 0x04003F55 RID: 16213
		Water,
		// Token: 0x04003F56 RID: 16214
		Radiated,
		// Token: 0x04003F57 RID: 16215
		Wasteland,
		// Token: 0x04003F58 RID: 16216
		burnt_forest,
		// Token: 0x04003F59 RID: 16217
		city,
		// Token: 0x04003F5A RID: 16218
		city_wasteland,
		// Token: 0x04003F5B RID: 16219
		wasteland_hub,
		// Token: 0x04003F5C RID: 16220
		caveFloor,
		// Token: 0x04003F5D RID: 16221
		caveCeiling
	}

	// Token: 0x02000A83 RID: 2691
	public class Probabilities
	{
		// Token: 0x0600533E RID: 21310 RVA: 0x00215574 File Offset: 0x00213774
		public Probabilities()
		{
			this.probabilities = new List<Vector3>[5];
			for (int i = 0; i < 5; i++)
			{
				this.probabilities[i] = new List<Vector3>();
			}
		}

		// Token: 0x0600533F RID: 21311 RVA: 0x002155AC File Offset: 0x002137AC
		public void AddProbability(BiomeDefinition.Probabilities.ProbType _type, Vector2 _range, float _probability)
		{
			this.probabilities[(int)_type].Add(new Vector3(_range.x, _range.y, _probability));
		}

		// Token: 0x06005340 RID: 21312 RVA: 0x002155DC File Offset: 0x002137DC
		public Vector2 CalcMinMaxPossibleValue(BiomeDefinition.Probabilities.ProbType type)
		{
			Vector2 vector = new Vector2(float.MaxValue, float.MinValue);
			List<Vector3> list = this.probabilities[(int)type];
			for (int i = 0; i < list.Count; i++)
			{
				Vector3 vector2 = list[i];
				if (vector2.x < vector.x)
				{
					vector.x = vector2.x;
				}
				if (vector2.y > vector.y)
				{
					vector.y = vector2.y;
				}
			}
			return vector;
		}

		// Token: 0x06005341 RID: 21313 RVA: 0x00215654 File Offset: 0x00213854
		public float GetRandomValue(BiomeDefinition.Probabilities.ProbType _type)
		{
			GameRandom gameRandom = GameManager.Instance.World.GetGameRandom();
			float randomFloat = gameRandom.RandomFloat;
			float num = 0f;
			List<Vector3> list = this.probabilities[(int)_type];
			for (int i = 0; i < list.Count; i++)
			{
				Vector3 vector = list[i];
				num += vector.z;
				if (randomFloat < num)
				{
					float randomFloat2 = gameRandom.RandomFloat;
					return vector.x * randomFloat2 + vector.y * (1f - randomFloat2);
				}
			}
			return 0f;
		}

		// Token: 0x06005342 RID: 21314 RVA: 0x002156E0 File Offset: 0x002138E0
		public void Normalize()
		{
			for (int i = 0; i < 5; i++)
			{
				List<Vector3> list = this.probabilities[i];
				float num = 0f;
				for (int j = 0; j < list.Count; j++)
				{
					num += list[j].z;
				}
				for (int k = 0; k < list.Count; k++)
				{
					Vector3 value = list[k];
					value.z /= num;
					list[k] = value;
				}
			}
		}

		// Token: 0x04003F5E RID: 16222
		public const int ProbTypeCount = 5;

		// Token: 0x04003F5F RID: 16223
		[PublicizedFrom(EAccessModifier.Private)]
		public List<Vector3>[] probabilities;

		// Token: 0x02000A84 RID: 2692
		public enum ProbType
		{
			// Token: 0x04003F61 RID: 16225
			Temperature,
			// Token: 0x04003F62 RID: 16226
			Precipitation,
			// Token: 0x04003F63 RID: 16227
			CloudThickness,
			// Token: 0x04003F64 RID: 16228
			Wind,
			// Token: 0x04003F65 RID: 16229
			Fog,
			// Token: 0x04003F66 RID: 16230
			Count
		}
	}

	// Token: 0x02000A85 RID: 2693
	public class WeatherGroup
	{
		// Token: 0x06005343 RID: 21315 RVA: 0x0021575D File Offset: 0x0021395D
		public void AddProbability(BiomeDefinition.Probabilities.ProbType _type, Vector2 _range, float _probability)
		{
			this.probabilities.AddProbability(_type, _range, _probability);
		}

		// Token: 0x04003F67 RID: 16231
		public string name;

		// Token: 0x04003F68 RID: 16232
		public int stormLevel;

		// Token: 0x04003F69 RID: 16233
		public float prob;

		// Token: 0x04003F6A RID: 16234
		public int duration;

		// Token: 0x04003F6B RID: 16235
		public Vector2i delay;

		// Token: 0x04003F6C RID: 16236
		public string buffName;

		// Token: 0x04003F6D RID: 16237
		public SpectrumWeatherType spectrum;

		// Token: 0x04003F6E RID: 16238
		public BiomeDefinition.Probabilities probabilities = new BiomeDefinition.Probabilities();
	}
}
