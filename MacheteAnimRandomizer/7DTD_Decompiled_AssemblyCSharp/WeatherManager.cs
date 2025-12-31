using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

// Token: 0x02001109 RID: 4361
public class WeatherManager : MonoBehaviour
{
	// Token: 0x060088E6 RID: 35046 RVA: 0x003770B3 File Offset: 0x003752B3
	public static void Init(World _world, GameObject _obj)
	{
		WeatherManager.Cleanup();
		WeatherManager.Instance = _obj.GetComponent<WeatherManager>();
		WeatherManager.Instance.Init(_world);
	}

	// Token: 0x060088E7 RID: 35047 RVA: 0x003770D0 File Offset: 0x003752D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Init(World _world)
	{
		this.world = _world;
		WeatherManager.gameRand = _world.GetGameRandom();
		string @string = GamePrefs.GetString(EnumGamePrefs.GameMode);
		this.isGameModeNormal = (!GameModeEditWorld.TypeName.Equals(@string) && !GameModeCreative.TypeName.Equals(@string));
		this.InitBiomeWeather();
		WeatherManager.currentWeather = new WeatherManager.BiomeWeather(this.world.Biomes.GetBiome(3));
		WeatherManager.currentWeather.parameters[2].name = "CloudThickness";
		WeatherManager.currentWeather.parameters[4].name = "Fog";
		WeatherManager.currentWeather.parameters[1].name = "Precipitation";
		WeatherManager.currentWeather.parameters[0].name = "Temperature";
		WeatherManager.currentWeather.parameters[3].name = "Wind";
		WeatherManager.currentWeather.rainParam.name = "Rain";
		WeatherManager.currentWeather.snowFallParam.name = "SnowFall";
		this.ApplyLoad();
	}

	// Token: 0x060088E8 RID: 35048 RVA: 0x003771D8 File Offset: 0x003753D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitBiomeWeather()
	{
		this.biomeWeather = new List<WeatherManager.BiomeWeather>();
		foreach (KeyValuePair<uint, BiomeDefinition> keyValuePair in this.world.Biomes.GetBiomeMap())
		{
			BiomeDefinition value = keyValuePair.Value;
			if (value.weatherGroups.Count > 0)
			{
				WeatherManager.BiomeWeather biomeWeather = new WeatherManager.BiomeWeather(value);
				this.biomeWeather.Add(biomeWeather);
				biomeWeather.Reset();
			}
		}
		int count = this.biomeWeather.Count;
		this.weatherPackages = new WeatherPackage[count];
		for (int i = 0; i < count; i++)
		{
			WeatherManager.BiomeWeather biomeWeather2 = this.biomeWeather[i];
			WeatherPackage weatherPackage = new WeatherPackage();
			weatherPackage.biomeId = biomeWeather2.biomeDefinition.m_Id;
			this.weatherPackages[i] = weatherPackage;
		}
	}

	// Token: 0x060088E9 RID: 35049 RVA: 0x003772C8 File Offset: 0x003754C8
	public static void Cleanup()
	{
		if (WeatherManager.Instance)
		{
			UnityEngine.Object.DestroyImmediate(WeatherManager.Instance.gameObject);
			WeatherManager.forceClouds = -1f;
			WeatherManager.forceRain = -1f;
			WeatherManager.forceSnowfall = -1f;
			WeatherManager.forceTemperature = -100f;
			WeatherManager.forceWind = -1f;
		}
	}

	// Token: 0x060088EA RID: 35050 RVA: 0x00377322 File Offset: 0x00375522
	public static void SetWorldTime(ulong _time)
	{
		if (WeatherManager.Instance)
		{
			int num = (int)_time;
			if (num < WeatherManager.worldTime)
			{
				WeatherManager.Instance.AdjustTimeRewind();
			}
			WeatherManager.worldTime = num;
		}
	}

	// Token: 0x060088EB RID: 35051 RVA: 0x0037734C File Offset: 0x0037554C
	[PublicizedFrom(EAccessModifier.Private)]
	public void AdjustTimeRewind()
	{
		this.weatherLastUpdateWorldTime = 0f;
		for (int i = 0; i < this.biomeWeather.Count; i++)
		{
			WeatherManager.BiomeWeather biomeWeather = this.biomeWeather[i];
			biomeWeather.stormWorldTime = 0;
			biomeWeather.stormDuration = 0;
			biomeWeather.nextRandWorldTime = 0;
		}
	}

	// Token: 0x060088EC RID: 35052 RVA: 0x0037739A File Offset: 0x0037559A
	public static void Load(IBinaryReaderOrWriter _RW, int _loadSize)
	{
		if (WeatherManager.loadData == null)
		{
			WeatherManager.loadData = new MemoryStream();
		}
		WeatherManager.loadData.SetLength((long)_loadSize);
		_RW.ReadWrite(WeatherManager.loadData.GetBuffer(), 0, _loadSize);
		WeatherManager.loadData.Position = 0L;
	}

	// Token: 0x060088ED RID: 35053 RVA: 0x003773D8 File Offset: 0x003755D8
	public void ApplyLoad()
	{
		if (WeatherManager.loadData != null)
		{
			using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
			{
				pooledBinaryReader.SetBaseStream(WeatherManager.loadData);
				this.ReadWriteData(pooledBinaryReader, true);
			}
			WeatherManager.loadData = null;
		}
	}

	// Token: 0x060088EE RID: 35054 RVA: 0x00377430 File Offset: 0x00375630
	public static void Save(IBinaryReaderOrWriter _RW)
	{
		if (WeatherManager.loadData != null)
		{
			_RW.ReadWrite(WeatherManager.loadData.GetBuffer(), 0, (int)WeatherManager.loadData.Length);
			return;
		}
		if (WeatherManager.Instance)
		{
			WeatherManager.Instance.ReadWriteData(_RW, false);
		}
	}

	// Token: 0x060088EF RID: 35055 RVA: 0x00377470 File Offset: 0x00375670
	[PublicizedFrom(EAccessModifier.Private)]
	public void ReadWriteData(IBinaryReaderOrWriter _RW, bool _load)
	{
		ushort num = _RW.ReadWrite(4);
		if (!_load)
		{
			_RW.ReadWrite((byte)GamePrefs.GetInt(EnumGamePrefs.DayNightLength));
			byte b = (byte)this.biomeWeather.Count;
			_RW.ReadWrite(b);
			for (int i = 0; i < (int)b; i++)
			{
				WeatherManager.BiomeWeather biomeWeather = this.biomeWeather[i];
				_RW.ReadWrite(biomeWeather.biomeDefinition.m_Id);
				_RW.ReadWrite((byte)biomeWeather.biomeDefinition.currentWeatherGroupIndex);
				_RW.ReadWrite(biomeWeather.stormWorldTime);
				_RW.ReadWrite((short)biomeWeather.stormDuration);
				_RW.ReadWrite(biomeWeather.nextRandWorldTime);
				for (int j = 0; j < 5; j++)
				{
					_RW.ReadWrite(biomeWeather.parameters[j].value);
				}
				_RW.ReadWrite(biomeWeather.rainParam.value);
				_RW.ReadWrite(biomeWeather.snowFallParam.value);
			}
			return;
		}
		if (num < 4)
		{
			return;
		}
		if ((int)_RW.ReadWrite(0) != GamePrefs.GetInt(EnumGamePrefs.DayNightLength))
		{
			return;
		}
		int num2 = (int)_RW.ReadWrite(0);
		for (int k = 0; k < num2; k++)
		{
			int id = (int)_RW.ReadWrite(0);
			WeatherManager.BiomeWeather biomeWeather2 = this.FindBiomeWeather(id);
			if (biomeWeather2 == null)
			{
				return;
			}
			int weatherGroup = (int)_RW.ReadWrite(0);
			biomeWeather2.biomeDefinition.SetWeatherGroup(weatherGroup);
			biomeWeather2.stormWorldTime = _RW.ReadWrite(0);
			biomeWeather2.stormDuration = (int)_RW.ReadWrite(0);
			biomeWeather2.nextRandWorldTime = _RW.ReadWrite(0);
			for (int l = 0; l < 5; l++)
			{
				float value = _RW.ReadWrite(0f);
				biomeWeather2.parameters[l].Set(value);
				biomeWeather2.biomeDefinition.WeatherSetValue((BiomeDefinition.Probabilities.ProbType)l, value);
			}
			biomeWeather2.rainParam.Set(_RW.ReadWrite(0f));
			biomeWeather2.snowFallParam.Set(_RW.ReadWrite(0f));
		}
	}

	// Token: 0x060088F0 RID: 35056 RVA: 0x0037766C File Offset: 0x0037586C
	[PublicizedFrom(EAccessModifier.Private)]
	public WeatherManager.BiomeWeather FindBiomeWeather(int _id)
	{
		for (int i = 0; i < this.biomeWeather.Count; i++)
		{
			WeatherManager.BiomeWeather biomeWeather = this.biomeWeather[i];
			if ((int)biomeWeather.biomeDefinition.m_Id == _id)
			{
				return biomeWeather;
			}
		}
		return null;
	}

	// Token: 0x060088F1 RID: 35057 RVA: 0x003776B0 File Offset: 0x003758B0
	[PublicizedFrom(EAccessModifier.Private)]
	public WeatherManager.BiomeWeather FindBiomeWeather(BiomeDefinition.BiomeType _type)
	{
		for (int i = 0; i < this.biomeWeather.Count; i++)
		{
			WeatherManager.BiomeWeather biomeWeather = this.biomeWeather[i];
			if (biomeWeather.biomeDefinition.m_BiomeType == _type)
			{
				return biomeWeather;
			}
		}
		return null;
	}

	// Token: 0x060088F2 RID: 35058 RVA: 0x003776F4 File Offset: 0x003758F4
	public bool IsStorming(BiomeDefinition.BiomeType _type)
	{
		WeatherManager.BiomeWeather biomeWeather = WeatherManager.Instance.FindBiomeWeather(_type);
		return biomeWeather != null && biomeWeather.stormState >= 2;
	}

	// Token: 0x060088F3 RID: 35059 RVA: 0x00002914 File Offset: 0x00000B14
	public static void ClearTemperatureOffSetHeights()
	{
	}

	// Token: 0x060088F4 RID: 35060 RVA: 0x00002914 File Offset: 0x00000B14
	public static void AddTemperatureOffSetHeight(float height, float degreesOffset)
	{
	}

	// Token: 0x060088F5 RID: 35061 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public static float SeaLevel()
	{
		return 0f;
	}

	// Token: 0x060088F6 RID: 35062 RVA: 0x0037771E File Offset: 0x0037591E
	public static float GetCloudThickness()
	{
		if (WeatherManager.forceClouds >= 0f)
		{
			return WeatherManager.forceClouds * 100f;
		}
		if (WeatherManager.currentWeather == null)
		{
			return 0f;
		}
		return WeatherManager.currentWeather.parameters[2].value;
	}

	// Token: 0x060088F7 RID: 35063 RVA: 0x00377756 File Offset: 0x00375956
	public static float GetTemperature()
	{
		if (WeatherManager.forceTemperature > -100f)
		{
			return WeatherManager.forceTemperature;
		}
		if (WeatherManager.currentWeather == null)
		{
			return 0f;
		}
		return WeatherManager.currentWeather.parameters[0].value;
	}

	// Token: 0x060088F8 RID: 35064 RVA: 0x00377788 File Offset: 0x00375988
	public static float GetWindSpeed()
	{
		if (WeatherManager.forceWind >= 0f)
		{
			return WeatherManager.forceWind;
		}
		if (WeatherManager.currentWeather == null)
		{
			return 0f;
		}
		return WeatherManager.currentWeather.parameters[3].value;
	}

	// Token: 0x060088F9 RID: 35065 RVA: 0x00002914 File Offset: 0x00000B14
	public static void EntityAddedToWorld(Entity entity)
	{
	}

	// Token: 0x060088FA RID: 35066 RVA: 0x00002914 File Offset: 0x00000B14
	public static void EntityRemovedFromWorld(Entity entity)
	{
	}

	// Token: 0x060088FB RID: 35067 RVA: 0x003777BA File Offset: 0x003759BA
	public float GetCurrentSnowfallPercent()
	{
		if (WeatherManager.forceSnowfall < 0f && WeatherManager.currentWeather != null)
		{
			return WeatherManager.currentWeather.snowFallParam.value;
		}
		return WeatherManager.forceSnowfall;
	}

	// Token: 0x060088FC RID: 35068 RVA: 0x003777E4 File Offset: 0x003759E4
	public float GetCurrentRainfallPercent()
	{
		if (WeatherManager.forceRain < 0f && WeatherManager.currentWeather != null)
		{
			return WeatherManager.currentWeather.rainParam.value;
		}
		return WeatherManager.forceRain;
	}

	// Token: 0x060088FD RID: 35069 RVA: 0x00377810 File Offset: 0x00375A10
	public float GetCurrentWetPercent(EntityAlive _ea)
	{
		float v = 0f;
		BiomeDefinition biomeStandingOn = _ea.biomeStandingOn;
		if (biomeStandingOn != null && biomeStandingOn.m_BiomeType == BiomeDefinition.BiomeType.Snow && biomeStandingOn.currentWeatherGroup.stormLevel >= 2)
		{
			v = 1f;
		}
		float v2 = Utils.FastMax(this.GetCurrentSnowfallPercent(), this.GetCurrentRainfallPercent());
		return Utils.FastMax(v, v2);
	}

	// Token: 0x060088FE RID: 35070 RVA: 0x00377863 File Offset: 0x00375A63
	public float GetCurrentCloudThicknessPercent()
	{
		return WeatherManager.GetCloudThickness() * 0.01f;
	}

	// Token: 0x060088FF RID: 35071 RVA: 0x00377870 File Offset: 0x00375A70
	public float GetCurrentTemperatureValue()
	{
		return WeatherManager.GetTemperature();
	}

	// Token: 0x06008900 RID: 35072 RVA: 0x00377877 File Offset: 0x00375A77
	public static void SetSimRandom(float _random)
	{
		WeatherManager.forceSimRandom = _random;
		if (WeatherManager.Instance)
		{
			WeatherManager.Instance.weatherLastUpdateWorldTime = 0f;
		}
	}

	// Token: 0x06008901 RID: 35073 RVA: 0x0037789A File Offset: 0x00375A9A
	[PublicizedFrom(EAccessModifier.Private)]
	public static void LoadSpectrums()
	{
		if (WeatherManager.atmosphereSpectrum == null)
		{
			WeatherManager.atmosphereSpectrum = new AtmosphereEffect[Enum.GetNames(typeof(SpectrumWeatherType)).Length - 1];
			WeatherManager.ReloadSpectrums();
		}
	}

	// Token: 0x06008902 RID: 35074 RVA: 0x003778C8 File Offset: 0x00375AC8
	public static void ReloadSpectrums()
	{
		WeatherManager.atmosphereSpectrum[1] = AtmosphereEffect.Load("Snowy", null);
		WeatherManager.atmosphereSpectrum[2] = AtmosphereEffect.Load("Stormy", null);
		WeatherManager.atmosphereSpectrum[3] = AtmosphereEffect.Load("Rainy", null);
		WeatherManager.atmosphereSpectrum[4] = AtmosphereEffect.Load("Foggy", null);
		WeatherManager.atmosphereSpectrum[5] = AtmosphereEffect.Load("BloodMoon", null);
	}

	// Token: 0x06008903 RID: 35075 RVA: 0x00377930 File Offset: 0x00375B30
	public void Start()
	{
		this.windZoneObj = GameObject.Find("WindZone");
		if (this.windZoneObj)
		{
			this.windZone = this.windZoneObj.GetComponent<WindZone>();
		}
		WeatherManager.LoadSpectrums();
		this.raycastMask = (LayerMask.GetMask(new string[]
		{
			"Water",
			"NoShadow",
			"Items",
			"CC Physics",
			"TerrainCollision",
			"CC Physics Dead",
			"CC Local Physics"
		}) | 1);
		string str = "@:Textures/Environment/Spectrums/default/";
		for (int i = 0; i < WeatherManager.strCloudTypes.Length; i++)
		{
			Texture texture = DataLoader.LoadAsset<Texture>(str + WeatherManager.strCloudTypes[i] + "Clouds.tga", false);
			WeatherManager.clouds[i] = texture;
		}
	}

	// Token: 0x06008904 RID: 35076 RVA: 0x003779F4 File Offset: 0x00375BF4
	public void PushTransitions()
	{
		this.spectrumBlend = 1f;
	}

	// Token: 0x06008905 RID: 35077 RVA: 0x00377A04 File Offset: 0x00375C04
	[PublicizedFrom(EAccessModifier.Private)]
	public void CurrentWeatherFromNearBiomesFrameUpdate()
	{
		if (this.world.BiomeAtmosphereEffects != null)
		{
			BiomeDefinition[] nearBiomes = this.world.BiomeAtmosphereEffects.nearBiomes;
			BiomeDefinition biomeDefinition = nearBiomes[0];
			WeatherManager.currentWeather.biomeDefinition = biomeDefinition;
			if (biomeDefinition == null)
			{
				return;
			}
			for (int i = 0; i < WeatherManager.currentWeather.parameters.Length; i++)
			{
				WeatherManager.currentWeather.parameters[i].target = 0f;
			}
			WeatherManager.inWeatherGracePeriod = ((WeatherManager.worldTime < 22000 || !this.isGameModeNormal) && this.CustomWeatherTime == -1f);
			if (WeatherManager.inWeatherGracePeriod)
			{
				WeatherManager.currentWeather.rainParam.Set(0f);
				WeatherManager.currentWeather.snowFallParam.Set(0f);
				int num = 70;
				BiomeDefinition.BiomeType biomeType = biomeDefinition.m_BiomeType;
				if (biomeType != BiomeDefinition.BiomeType.Snow)
				{
					if (biomeType - BiomeDefinition.BiomeType.Forest <= 1)
					{
						num = 60;
					}
				}
				else
				{
					num = 45;
				}
				WeatherManager.currentWeather.parameters[0].Set((float)num);
				WeatherManager.currentWeather.parameters[3].Set(8f);
				return;
			}
			WeatherManager.BiomeWeather biomeWeather = this.FindBiomeWeather(biomeDefinition.m_BiomeType);
			WeatherManager.currentWeather.remainingSeconds = biomeWeather.remainingSeconds;
			WeatherManager.currentWeather.rainParam.target = biomeWeather.rainParam.value;
			WeatherManager.currentWeather.snowFallParam.target = biomeWeather.snowFallParam.value;
			if (!this.isCurrentWeatherUpdatedFirstTime)
			{
				WeatherManager.currentWeather.rainParam.value = biomeWeather.rainParam.value;
				WeatherManager.currentWeather.snowFallParam.value = biomeWeather.snowFallParam.value;
			}
			float num2 = 0f;
			foreach (BiomeDefinition biomeDefinition2 in nearBiomes)
			{
				if (biomeDefinition2 != null)
				{
					num2 += biomeDefinition2.currentPlayerIntensity;
				}
			}
			foreach (BiomeDefinition biomeDefinition3 in nearBiomes)
			{
				if (biomeDefinition3 != null)
				{
					biomeWeather = this.FindBiomeWeather(biomeDefinition3.m_BiomeType);
					float num3 = Utils.FastClamp01(biomeDefinition3.currentPlayerIntensity / num2);
					for (int l = 0; l < WeatherManager.currentWeather.parameters.Length; l++)
					{
						WeatherManager.Param param = WeatherManager.currentWeather.parameters[l];
						param.target += biomeWeather.parameterFinals[l] * num3;
						if (!this.isCurrentWeatherUpdatedFirstTime)
						{
							param.value = param.target;
						}
					}
				}
			}
			this.isCurrentWeatherUpdatedFirstTime = true;
		}
	}

	// Token: 0x06008906 RID: 35078 RVA: 0x00377C7C File Offset: 0x00375E7C
	[PublicizedFrom(EAccessModifier.Private)]
	public void GenerateWeatherServerFrameUpdate()
	{
		if (WeatherManager.inWeatherGracePeriod)
		{
			this.GeneralReset();
			return;
		}
		if (this.CustomWeatherTime > 0f)
		{
			this.CustomWeatherTime -= Time.deltaTime;
			if (this.CustomWeatherTime <= 0f)
			{
				this.CustomWeatherName = "";
				this.CustomWeatherTime = -1f;
				this.GeneralReset();
				this.SetAllWeather("default");
				this.weatherLastUpdateWorldTime = (float)WeatherManager.worldTime;
				return;
			}
		}
		else
		{
			string text = this.CalcGlobalWeatherType();
			if (text != null)
			{
				this.SetAllWeather(text);
				return;
			}
			this.weatherAllName = null;
			if (Utils.FastAbs((float)WeatherManager.worldTime - this.weatherLastUpdateWorldTime) >= 5f)
			{
				this.weatherLastUpdateWorldTime = (float)WeatherManager.worldTime;
				float freq = (float)GameStats.GetInt(EnumGameStats.StormFreq) * 0.01f;
				for (int i = 0; i < this.biomeWeather.Count; i++)
				{
					this.biomeWeather[i].ServerTimeUpdate(WeatherManager.worldTime, freq);
				}
			}
		}
	}

	// Token: 0x06008907 RID: 35079 RVA: 0x00377D7C File Offset: 0x00375F7C
	[PublicizedFrom(EAccessModifier.Private)]
	public string CalcGlobalWeatherType()
	{
		if (SkyManager.IsBloodMoonVisible())
		{
			int num = 5000;
			for (int i = 0; i < this.biomeWeather.Count; i++)
			{
				WeatherManager.BiomeWeather biomeWeather = this.biomeWeather[i];
				if (biomeWeather.stormWorldTime - WeatherManager.worldTime < num)
				{
					biomeWeather.stormWorldTime = WeatherManager.worldTime + num;
				}
			}
			return "bloodMoon";
		}
		return null;
	}

	// Token: 0x06008908 RID: 35080 RVA: 0x00377DDC File Offset: 0x00375FDC
	public void ForceWeather(string _weatherName, float _duration)
	{
		this.CustomWeatherName = _weatherName;
		this.CustomWeatherTime = _duration;
		this.GeneralReset();
		this.SetAllWeather(_weatherName);
	}

	// Token: 0x06008909 RID: 35081 RVA: 0x00377DFC File Offset: 0x00375FFC
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetAllWeather(string _weatherName)
	{
		if (this.weatherAllName != _weatherName)
		{
			this.weatherAllName = _weatherName;
			for (int i = 0; i < this.biomeWeather.Count; i++)
			{
				this.biomeWeather[i].SetWeather(_weatherName);
			}
		}
	}

	// Token: 0x0600890A RID: 35082 RVA: 0x00377E48 File Offset: 0x00376048
	public void SetStorm(string _biomeName, int _duration)
	{
		for (int i = 0; i < this.biomeWeather.Count; i++)
		{
			WeatherManager.BiomeWeather biomeWeather = this.biomeWeather[i];
			if (_biomeName == null || biomeWeather.biomeDefinition.m_sBiomeName == _biomeName)
			{
				biomeWeather.stormWorldTime = WeatherManager.worldTime;
				biomeWeather.stormDuration = _duration;
			}
		}
	}

	// Token: 0x0600890B RID: 35083 RVA: 0x00377EA0 File Offset: 0x003760A0
	public void TriggerUpdate()
	{
		this.weatherAllName = null;
		this.weatherLastUpdateWorldTime = 0f;
	}

	// Token: 0x0600890C RID: 35084 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void GeneralReset()
	{
	}

	// Token: 0x0600890D RID: 35085 RVA: 0x00377EB4 File Offset: 0x003760B4
	public void FrameUpdate()
	{
		if (GameManager.Instance == null || SkyManager.random == null)
		{
			return;
		}
		if (this.world == null)
		{
			return;
		}
		float time = Time.time;
		EntityPlayerLocal primaryPlayer = this.world.GetPrimaryPlayer();
		if (time > this.checkPlayerMoveTime + 1f)
		{
			this.checkPlayerMoveTime = time;
			if (primaryPlayer)
			{
				Vector3 vector = primaryPlayer.position - this.playerPosition;
				if (vector.x * vector.x + vector.y * vector.y + vector.z * vector.z > 400f)
				{
					this.spectrumBlend = 1f;
				}
				this.playerPosition = primaryPlayer.position;
			}
		}
		int num = Time.frameCount;
		if (this.frameCount == num)
		{
			return;
		}
		this.frameCount = num;
		if (primaryPlayer)
		{
			this.ParticlesFrameUpdate(primaryPlayer);
			primaryPlayer.WeatherStatusFrameUpdate();
		}
		bool flag = GameManager.IsDedicatedServer || SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer;
		if (flag)
		{
			this.GenerateWeatherServerFrameUpdate();
		}
		for (int i = 0; i < this.biomeWeather.Count; i++)
		{
			this.biomeWeather[i].FrameUpdate(flag);
		}
		this.CurrentWeatherFromNearBiomesFrameUpdate();
		WeatherManager.currentWeather.ParamsFrameUpdate();
		this.thunderDelay -= Time.deltaTime;
		if (this.thunderDelay <= 0f)
		{
			this.thunderDelay = Utils.FastLerpUnclamped(30f, 60f, SkyManager.random.RandomFloat);
			if (((((WeatherManager.forceRain >= 0f) ? WeatherManager.forceRain : WeatherManager.currentWeather.rainParam.value) > 0.5f && ((WeatherManager.forceClouds >= 0f) ? (WeatherManager.forceClouds * 100f) : WeatherManager.currentWeather.CloudThickness()) >= 70f) || SkyManager.IsBloodMoonVisible()) && EnvironmentAudioManager.Instance != null)
			{
				EnvironmentAudioManager.Instance.TriggerThunder(this.world.GetPrimaryPlayer().position);
			}
		}
		if (WeatherManager.needToReUpdateWeatherSpectrums)
		{
			WeatherManager.needToReUpdateWeatherSpectrums = false;
			this.spectrumBlend = 1f;
		}
		this.SpectrumsFrameUpdate();
		this.CloudsFrameUpdate();
		this.WindFrameUpdate();
		TriggerEffectManager.UpdateDualSenseLightFromWeather(WeatherManager.currentWeather);
	}

	// Token: 0x0600890E RID: 35086 RVA: 0x003780EC File Offset: 0x003762EC
	public void CloudsFrameUpdateNow()
	{
		this.CloudsFrameUpdate();
		this.cloudThickness = this.cloudThicknessTarget;
	}

	// Token: 0x0600890F RID: 35087 RVA: 0x00378100 File Offset: 0x00376300
	public void CloudsFrameUpdate()
	{
		if (WeatherManager.currentWeather == null)
		{
			return;
		}
		float num = WeatherManager.currentWeather.CloudThickness();
		if (WeatherManager.forceClouds >= 0f)
		{
			num = WeatherManager.forceClouds * 100f;
		}
		this.cloudThicknessTarget = num;
		if (num < 20f)
		{
			this.cloudThicknessTarget = 0f;
		}
		this.cloudThickness = Mathf.MoveTowards(this.cloudThickness, this.cloudThicknessTarget, 0.05f);
		Texture mainTex;
		Texture blendTex;
		float num2;
		if (this.cloudThickness <= 40f)
		{
			mainTex = WeatherManager.clouds[0];
			blendTex = WeatherManager.clouds[1];
			num2 = this.cloudThickness / 40f;
		}
		else
		{
			mainTex = WeatherManager.clouds[2];
			blendTex = WeatherManager.clouds[1];
			num2 = (this.cloudThickness - 40f) / 50f;
			if (num2 >= 1f)
			{
				num2 = 1f;
			}
			num2 = 1f - num2;
		}
		SkyManager.SetCloudTextures(mainTex, blendTex);
		SkyManager.SetCloudTransition(num2);
	}

	// Token: 0x06008910 RID: 35088 RVA: 0x003781E0 File Offset: 0x003763E0
	public void WindFrameUpdate()
	{
		float deltaTime = Time.deltaTime;
		float num = WeatherManager.GetWindSpeed();
		float num2 = num * 0.01f;
		this.windGust += this.windGustStep * deltaTime;
		if (this.windGust <= 0f)
		{
			this.windGust = 0f;
			this.windGustTime -= deltaTime;
			if (this.windGustTime <= 0f)
			{
				GameRandom gameRandom = this.world.GetGameRandom();
				this.windGustTarget = (3f + num * 0.33f) * gameRandom.RandomFloat + 5f;
				this.windGustStep = 0.35f * this.windGustTarget;
				this.windGustTime = (1f + 5f * gameRandom.RandomFloat) * (1f - num2) + 0.5f;
			}
		}
		if (this.windGust > this.windGustTarget)
		{
			this.windGust = this.windGustTarget;
			this.windGustStep = -this.windGustStep;
		}
		num += this.windGust;
		num *= 0.01f;
		this.windZone.windMain = num * 1.5f;
		this.windSpeedPrevious = this.windSpeed;
		this.windSpeed = num;
		this.windTimePrevious = this.windTime;
		this.windTime += num * deltaTime;
		Shader.SetGlobalVector("_Wind", new Vector4(this.windSpeed, this.windTime, this.windSpeedPrevious, this.windTimePrevious));
	}

	// Token: 0x06008911 RID: 35089 RVA: 0x00378350 File Offset: 0x00376550
	public void InitParticles()
	{
		this.GetParticleParts("Rain", out this.rainParticleObj, out this.rainParticleMat, out this.rainParticleSys);
		this.rainParticleT = this.rainParticleObj.transform;
		this.rainEmissionMaxRate = this.rainParticleSys.emission.rateOverTime.constant;
		this.InitParticleData("Snow", this.snowData, false);
		for (int i = 0; i < 4; i++)
		{
			WeatherManager.ParticleData particleData = new WeatherManager.ParticleData();
			this.stormData[i] = particleData;
			particleData.weather = this.FindBiomeWeather(this.stormBiomes[i]);
			string name = "Storm" + this.stormNames[i];
			this.InitParticleData(name, particleData, true);
		}
	}

	// Token: 0x06008912 RID: 35090 RVA: 0x0037840C File Offset: 0x0037660C
	public void InitParticleData(string _name, WeatherManager.ParticleData _data, bool _isStorm)
	{
		this.GetParticleParts(_name, out _data.rootObj, out _data.rootParticleMat, out _data.rootParticleSys);
		_data.rootT = _data.rootObj.transform;
		_data.rootEmissionMaxRate = _data.rootParticleSys.emission.rateOverTime.constant;
		Transform transform = _data.rootT.Find("Near");
		if (transform)
		{
			_data.nearParticleSys = transform.GetComponent<ParticleSystem>();
			_data.nearEmissionMaxRate = _data.nearParticleSys.emission.rateOverTime.constant;
		}
		Transform transform2 = _data.rootT.Find("Top");
		if (transform2)
		{
			_data.topParticleSys = transform2.GetComponent<ParticleSystem>();
			_data.topEmissionMaxRate = _data.topParticleSys.emission.rateOverTime.constant;
		}
		Transform transform3 = _data.rootT.Find("Far");
		if (transform3)
		{
			if (_isStorm)
			{
				transform3.SetParent(_data.rootT.parent);
				transform3.gameObject.SetActive(false);
			}
			_data.farT = transform3;
			_data.farParticleSys = transform3.GetComponent<ParticleSystem>();
			_data.farBaseColor = _data.farParticleSys.main.startColor.color;
			Transform transform4 = transform3.Find("Ring");
			if (transform4)
			{
				_data.ringParticleSys = transform4.GetComponent<ParticleSystem>();
				_data.ringRadiusMax = _data.ringParticleSys.shape.radius;
			}
		}
		_data.rootObj.GetComponentsInChildren<ParticleSystem>(true, _data.particleSystems);
		for (int i = _data.particleSystems.Count - 1; i >= 0; i--)
		{
			if (_data.particleSystems[i] == _data.farParticleSys)
			{
				_data.particleSystems.RemoveAt(i);
			}
		}
		_data.psEmissionMaxRates = new float[_data.particleSystems.Count];
		for (int j = 0; j < _data.particleSystems.Count; j++)
		{
			ParticleSystem.EmissionModule emission = _data.particleSystems[j].emission;
			_data.psEmissionMaxRates[j] = emission.rateOverTime.constant;
		}
		_data.forceT = _data.rootT.Find("Force");
	}

	// Token: 0x06008913 RID: 35091 RVA: 0x0037866C File Offset: 0x0037686C
	[PublicizedFrom(EAccessModifier.Private)]
	public void GetParticleParts(string name, out GameObject obj, out Material mat, out ParticleSystem ps)
	{
		Transform transform = SkyManager.skyManager.transform.Find(name);
		obj = transform.gameObject;
		ps = obj.GetComponent<ParticleSystem>();
		Renderer component = ps.GetComponent<Renderer>();
		mat = component.material;
	}

	// Token: 0x06008914 RID: 35092 RVA: 0x003786B0 File Offset: 0x003768B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ParticlesFrameUpdate(EntityPlayerLocal localPlayer)
	{
		if (this.mainCamera == null)
		{
			this.mainCamera = Camera.main;
			if (this.mainCamera == null)
			{
				return;
			}
		}
		float deltaTime = Time.deltaTime;
		Vector3 position = this.mainCamera.transform.position;
		position.y += 250f;
		RaycastHit raycastHit;
		if (Physics.SphereCast(new Ray(position, Vector3.down), 9f, out raycastHit, float.PositiveInfinity, this.raycastMask))
		{
			this.particleFallLastPos = raycastHit.point;
			Vector3 velocityPerSecond = localPlayer.GetVelocityPerSecond();
			this.particleFallLastPos.x = this.particleFallLastPos.x + velocityPerSecond.x * 2f;
			this.particleFallLastPos.z = this.particleFallLastPos.z + velocityPerSecond.z * 2f;
			if (velocityPerSecond.y < -5f)
			{
				velocityPerSecond.y = -5f;
			}
			this.particleFallLastPos.y = this.particleFallLastPos.y + velocityPerSecond.y;
		}
		this.particleFallPos = this.particleFallLastPos;
		this.particleFallPos.y = this.particleFallPos.y + 12f;
		this.rainParticleT.position = this.particleFallPos;
		this.snowData.rootT.position = this.particleFallPos;
		for (int i = 0; i < 4; i++)
		{
			WeatherManager.ParticleData particleData = this.stormData[i];
			particleData.rootT.position = this.particleFallPos;
			if (particleData.farT)
			{
				particleData.farT.position = this.particleFallPos;
			}
		}
		WeatherManager.BiomeWeather biomeWeather = (localPlayer.biomeStandingOn != null) ? this.FindBiomeWeather(localPlayer.biomeStandingOn.m_BiomeType) : WeatherManager.currentWeather;
		float num = (WeatherManager.forceRain >= 0f) ? WeatherManager.forceRain : biomeWeather.rainParam.value;
		if (num > 0f && this.rainParticleSys)
		{
			this.rainParticleSys.emission.rateOverTime = this.rainEmissionMaxRate * (num * 0.99f + 0.01f);
		}
		this.rainParticleObj.SetActive(num > 0f);
		float num2 = (WeatherManager.forceSnowfall >= 0f) ? WeatherManager.forceSnowfall : Utils.FastClamp01(biomeWeather.snowFallParam.value);
		if (num2 <= 0f)
		{
			this.snowData.rootObj.SetActive(false);
		}
		else
		{
			this.snowData.rootObj.SetActive(true);
			bool flag = SkyManager.GetFogDensity() < 0.3f;
			float num3 = num2 * 0.99f + 0.01f;
			ParticleSystem.EmissionModule emission = this.snowData.rootParticleSys.emission;
			emission.rateOverTime = this.snowData.rootEmissionMaxRate * num3;
			emission = this.snowData.nearParticleSys.emission;
			emission.rateOverTime = this.snowData.nearEmissionMaxRate * num3;
			emission = this.snowData.topParticleSys.emission;
			emission.rateOverTime = (flag ? (this.snowData.topEmissionMaxRate * num3) : 0f);
			ParticleSystem.MainModule main = this.snowData.farParticleSys.main;
			ParticleSystem.MinMaxGradient startColor = main.startColor;
			Color farBaseColor = this.snowData.farBaseColor;
			farBaseColor.a *= num2 * 0.95f + 0.05f;
			startColor.color = farBaseColor;
			main.startColor = startColor;
			Vector3 position2 = localPlayer.position - Origin.position;
			position2.y += 1f;
			this.snowData.forceT.position = position2;
		}
		this.stormFogTarget = 0f;
		for (int j = 0; j < 4; j++)
		{
			WeatherManager.ParticleData particleData2 = this.stormData[j];
			WeatherManager.BiomeWeather weather = particleData2.weather;
			int num4 = (weather.biomeDefinition != biomeWeather.biomeDefinition) ? 0 : weather.biomeDefinition.currentWeatherGroup.stormLevel;
			if (num4 == 0)
			{
				if (particleData2.intensity > -1f)
				{
					particleData2.intensity -= 0.2f * deltaTime;
					if (particleData2.intensity <= -1f)
					{
						particleData2.stormLevel = 0;
						particleData2.thunderDelay = 0f;
						if (particleData2.farT)
						{
							particleData2.farT.gameObject.SetActive(false);
						}
						particleData2.rootObj.SetActive(false);
					}
				}
			}
			else
			{
				if (particleData2.stormLevel != num4)
				{
					particleData2.stormLevel = num4;
					particleData2.intensity = 0f;
				}
				particleData2.intensity += 0.2f * deltaTime;
				particleData2.intensity = Utils.FastClamp01(particleData2.intensity);
				particleData2.thunderDelay -= deltaTime;
				if (particleData2.thunderDelay <= 0f)
				{
					particleData2.thunderDelay = (float)WeatherManager.gameRand.RandomRange(7, 12);
					if (EnvironmentAudioManager.Instance != null)
					{
						EnvironmentAudioManager.Instance.TriggerThunder(localPlayer.position);
					}
				}
			}
			if (particleData2.intensity > -1f)
			{
				float num5 = Utils.FastMax(0f, particleData2.intensity);
				float num6 = (60f - (float)weather.remainingSeconds) / 60f;
				if (num6 > 0f)
				{
					float v = Utils.FastLerp(0f, 0.2f, num6);
					this.stormFogTarget = Utils.FastMax(v, this.stormFogTarget);
				}
				if (num4 == 1)
				{
					if (particleData2.farT)
					{
						particleData2.farT.gameObject.SetActive(true);
						ParticleSystem.MainModule main2 = this.snowData.farParticleSys.main;
						ParticleSystem.MinMaxGradient startColor2 = main2.startColor;
						Color farBaseColor2 = particleData2.farBaseColor;
						farBaseColor2.a *= num5 * 0.95f + 0.05f;
						startColor2.color = farBaseColor2;
						main2.startColor = startColor2;
						if (particleData2.ringParticleSys)
						{
							particleData2.ringParticleSys.shape.radius = Utils.FastLerp(0.3f, 1f, (float)weather.remainingSeconds / 60f) * particleData2.ringRadiusMax;
						}
					}
					particleData2.rootObj.SetActive(false);
				}
				else
				{
					if (num5 < 0.2f && particleData2.ringParticleSys)
					{
						particleData2.ringParticleSys.Stop();
					}
					if (num5 > 0.99f && particleData2.farT)
					{
						particleData2.farT.gameObject.SetActive(false);
					}
					particleData2.rootObj.SetActive(true);
					if (localPlayer.isIndoorsCurrent)
					{
						num5 *= 0.5f;
					}
					for (int k = 0; k < particleData2.particleSystems.Count; k++)
					{
						particleData2.particleSystems[k].emission.rateOverTime = particleData2.psEmissionMaxRates[k] * num5;
					}
					Vector3 position3 = localPlayer.position - Origin.position;
					position3.y += 2f;
					particleData2.forceT.position = position3;
				}
			}
		}
		if (localPlayer.isIndoorsCurrent)
		{
			this.stormFogTarget *= 0.3f;
		}
		WeatherManager.stormFog = Utils.FastMoveTowards(WeatherManager.stormFog, this.stormFogTarget, deltaTime / 2.1f);
	}

	// Token: 0x06008915 RID: 35093 RVA: 0x00378E2C File Offset: 0x0037702C
	public string GetSpectrumInfo()
	{
		float value = 1f - this.spectrumBlend;
		float value2 = this.spectrumBlend;
		string text = this.spectrumSourceType.ToString();
		string text2 = this.spectrumTargetType.ToString();
		return string.Format("source {0} {1}, target {2} {3}", new object[]
		{
			text,
			value.ToCultureInvariantString(),
			text2,
			value2.ToCultureInvariantString()
		});
	}

	// Token: 0x06008916 RID: 35094 RVA: 0x00378E9B File Offset: 0x0037709B
	public static void SetForceSpectrum(SpectrumWeatherType type)
	{
		WeatherManager.forcedSpectrum = type;
	}

	// Token: 0x06008917 RID: 35095 RVA: 0x00378EA4 File Offset: 0x003770A4
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpectrumsFrameUpdate()
	{
		WeatherManager.LoadSpectrums();
		float num = SkyManager.BloodMoonVisiblePercent();
		if (num > 0f && this.spectrumTargetType == SpectrumWeatherType.BloodMoon)
		{
			this.spectrumBlend = num;
		}
		else if (this.spectrumBlend < 1f)
		{
			this.spectrumBlend += Time.deltaTime / 10f;
			if (this.spectrumBlend > 1f)
			{
				this.spectrumBlend = 1f;
			}
		}
		if (this.spectrumSourceType == this.spectrumTargetType)
		{
			this.spectrumBlend = 1f;
		}
		if (this.spectrumBlend >= 1f)
		{
			this.spectrumSourceType = this.spectrumTargetType;
			this.spectrumTargetType = SpectrumWeatherType.Biome;
			if (WeatherManager.currentWeather.biomeDefinition != null)
			{
				this.spectrumTargetType = WeatherManager.currentWeather.biomeDefinition.weatherSpectrum;
			}
			if (num > 0f)
			{
				this.spectrumTargetType = SpectrumWeatherType.BloodMoon;
			}
			if (this.spectrumSourceType != this.spectrumTargetType)
			{
				this.spectrumBlend = 0f;
			}
		}
	}

	// Token: 0x06008918 RID: 35096 RVA: 0x00378F98 File Offset: 0x00377198
	public Color GetWeatherSpectrum(Color regularSpectrum, AtmosphereEffect.ESpecIdx type, float dayTimeScalar)
	{
		if (WeatherManager.forcedSpectrum == SpectrumWeatherType.None)
		{
			Color a = regularSpectrum;
			Color a2 = regularSpectrum;
			if (this.isGameModeNormal)
			{
				if (this.spectrumSourceType != SpectrumWeatherType.Biome)
				{
					ColorSpectrum colorSpectrum = WeatherManager.atmosphereSpectrum[(int)this.spectrumSourceType].spectrums[(int)type];
					if (colorSpectrum != null)
					{
						a = colorSpectrum.GetValue(dayTimeScalar);
					}
				}
				if (this.spectrumTargetType != SpectrumWeatherType.Biome)
				{
					ColorSpectrum colorSpectrum2 = WeatherManager.atmosphereSpectrum[(int)this.spectrumTargetType].spectrums[(int)type];
					if (colorSpectrum2 != null)
					{
						a2 = colorSpectrum2.GetValue(dayTimeScalar);
					}
				}
			}
			return a * (1f - this.spectrumBlend) + a2 * this.spectrumBlend;
		}
		int num = (int)WeatherManager.forcedSpectrum;
		AtmosphereEffect atmosphereEffect = WeatherManager.atmosphereSpectrum[num];
		if (atmosphereEffect == null)
		{
			return regularSpectrum;
		}
		ColorSpectrum colorSpectrum3 = atmosphereEffect.spectrums[(int)type];
		if (colorSpectrum3 == null)
		{
			return regularSpectrum;
		}
		return colorSpectrum3.GetValue(dayTimeScalar);
	}

	// Token: 0x06008919 RID: 35097 RVA: 0x00379060 File Offset: 0x00377260
	public void TriggerThunder(int _playWorldTime, Vector3 _pos)
	{
		EnvironmentAudioManager.Instance.TriggerThunder(_pos);
	}

	// Token: 0x0600891A RID: 35098 RVA: 0x00379070 File Offset: 0x00377270
	public void ClientProcessPackages(WeatherPackage[] _packages)
	{
		int num = Time.frameCount;
		if (this.processingPackageFrame == num)
		{
			return;
		}
		this.processingPackageFrame = num;
		foreach (WeatherPackage weatherPackage in _packages)
		{
			BiomeDefinition biomeDefinition;
			if (WorldBiomes.Instance.TryGetBiome(weatherPackage.biomeId, out biomeDefinition))
			{
				biomeDefinition.SetWeatherGroup((int)weatherPackage.groupIndex);
				WeatherManager.BiomeWeather biomeWeather = this.FindBiomeWeather((int)weatherPackage.biomeId);
				if (biomeWeather != null)
				{
					weatherPackage.CopyTo(biomeWeather);
				}
			}
		}
	}

	// Token: 0x0600891B RID: 35099 RVA: 0x003790E4 File Offset: 0x003772E4
	public void SendPackages()
	{
		this.CalcPackages();
		NetPackageWeather package = NetPackageManager.GetPackage<NetPackageWeather>();
		package.Setup(this.weatherPackages);
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, true, -1, -1, -1, null, 192, false);
	}

	// Token: 0x0600891C RID: 35100 RVA: 0x00379128 File Offset: 0x00377328
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalcPackages()
	{
		int count = this.biomeWeather.Count;
		for (int i = 0; i < count; i++)
		{
			WeatherManager.BiomeWeather biomeWeather = this.biomeWeather[i];
			WeatherPackage weatherPackage = this.weatherPackages[i];
			weatherPackage.biomeId = biomeWeather.biomeDefinition.m_Id;
			weatherPackage.groupIndex = (byte)biomeWeather.biomeDefinition.currentWeatherGroupIndex;
			weatherPackage.remainingSeconds = biomeWeather.remainingSeconds;
			int num = 0;
			while (num < biomeWeather.parameters.Length && num < weatherPackage.param.Length)
			{
				weatherPackage.param[num] = biomeWeather.parameterFinals[num];
				num++;
			}
		}
	}

	// Token: 0x0600891D RID: 35101 RVA: 0x003791CC File Offset: 0x003773CC
	public override string ToString()
	{
		string text = string.Format("#{0}", this.biomeWeather.Count);
		for (int i = 0; i < this.biomeWeather.Count; i++)
		{
			WeatherManager.BiomeWeather biomeWeather = this.biomeWeather[i];
			text = text + "\n" + biomeWeather.ToString();
		}
		return text;
	}

	// Token: 0x0600891E RID: 35102 RVA: 0x0037922A File Offset: 0x0037742A
	[Conditional("DEBUG_WEATHERNET")]
	public void LogNet(string _format = "", params object[] _args)
	{
		_format = string.Format("{0} WeatherManager net {1}", GameManager.frameCount, _format);
		Log.Warning(_format, _args);
	}

	// Token: 0x04006B02 RID: 27394
	public const ushort cVersion = 4;

	// Token: 0x04006B03 RID: 27395
	public static WeatherManager Instance;

	// Token: 0x04006B04 RID: 27396
	public const float cStormWarningDuration = 60f;

	// Token: 0x04006B05 RID: 27397
	public const int BaseTemperature = 70;

	// Token: 0x04006B06 RID: 27398
	public List<WeatherManager.BiomeWeather> biomeWeather;

	// Token: 0x04006B07 RID: 27399
	public static float forceClouds = -1f;

	// Token: 0x04006B08 RID: 27400
	public static float forceRain = -1f;

	// Token: 0x04006B09 RID: 27401
	public static float forceSnowfall = -1f;

	// Token: 0x04006B0A RID: 27402
	public const float cForceTempDefault = -100f;

	// Token: 0x04006B0B RID: 27403
	public static float forceTemperature = -100f;

	// Token: 0x04006B0C RID: 27404
	public static float forceWind = -1f;

	// Token: 0x04006B0D RID: 27405
	public static bool needToReUpdateWeatherSpectrums;

	// Token: 0x04006B0E RID: 27406
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float forceSimRandom = -1f;

	// Token: 0x04006B0F RID: 27407
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cGracePeriodWorldTime = 22000;

	// Token: 0x04006B10 RID: 27408
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int worldTime;

	// Token: 0x04006B11 RID: 27409
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public World world;

	// Token: 0x04006B12 RID: 27410
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static GameRandom gameRand;

	// Token: 0x04006B13 RID: 27411
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isGameModeNormal;

	// Token: 0x04006B14 RID: 27412
	public static bool inWeatherGracePeriod = true;

	// Token: 0x04006B15 RID: 27413
	public static WeatherManager.BiomeWeather currentWeather;

	// Token: 0x04006B16 RID: 27414
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public WeatherPackage[] weatherPackages;

	// Token: 0x04006B17 RID: 27415
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cLightningDelayMin = 30f;

	// Token: 0x04006B18 RID: 27416
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cLightningDelayMax = 60f;

	// Token: 0x04006B19 RID: 27417
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float thunderDelay = 30f;

	// Token: 0x04006B1A RID: 27418
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cWeatherTransitionSeconds = 10f;

	// Token: 0x04006B1B RID: 27419
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int frameCount = -1;

	// Token: 0x04006B1C RID: 27420
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float stormFog;

	// Token: 0x04006B1D RID: 27421
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float stormFogTarget;

	// Token: 0x04006B1E RID: 27422
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static string[] strCloudTypes = new string[]
	{
		"Whispy",
		"Fluffy",
		"ThickOvercast"
	};

	// Token: 0x04006B1F RID: 27423
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Texture[] clouds = new Texture[WeatherManager.strCloudTypes.Length];

	// Token: 0x04006B20 RID: 27424
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isCurrentWeatherUpdatedFirstTime;

	// Token: 0x04006B21 RID: 27425
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cWindMax = 100f;

	// Token: 0x04006B22 RID: 27426
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cWindScale = 0.01f;

	// Token: 0x04006B23 RID: 27427
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject windZoneObj;

	// Token: 0x04006B24 RID: 27428
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public WindZone windZone;

	// Token: 0x04006B25 RID: 27429
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float windSpeed;

	// Token: 0x04006B26 RID: 27430
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float windSpeedPrevious;

	// Token: 0x04006B27 RID: 27431
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float windTime;

	// Token: 0x04006B28 RID: 27432
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float windTimePrevious;

	// Token: 0x04006B29 RID: 27433
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float windGust;

	// Token: 0x04006B2A RID: 27434
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float windGustStep;

	// Token: 0x04006B2B RID: 27435
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float windGustTarget;

	// Token: 0x04006B2C RID: 27436
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float windGustTime;

	// Token: 0x04006B2D RID: 27437
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Camera mainCamera;

	// Token: 0x04006B2E RID: 27438
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int raycastMask;

	// Token: 0x04006B2F RID: 27439
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject rainParticleObj;

	// Token: 0x04006B30 RID: 27440
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform rainParticleT;

	// Token: 0x04006B31 RID: 27441
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ParticleSystem rainParticleSys;

	// Token: 0x04006B32 RID: 27442
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material rainParticleMat;

	// Token: 0x04006B33 RID: 27443
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float rainEmissionMaxRate = 1f;

	// Token: 0x04006B34 RID: 27444
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cIntensityOff = -1f;

	// Token: 0x04006B35 RID: 27445
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public WeatherManager.ParticleData snowData = new WeatherManager.ParticleData();

	// Token: 0x04006B36 RID: 27446
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cStormTypeCount = 4;

	// Token: 0x04006B37 RID: 27447
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string[] stormNames = new string[]
	{
		"Burnt",
		"Desert",
		"Snow",
		"Wasteland"
	};

	// Token: 0x04006B38 RID: 27448
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BiomeDefinition.BiomeType[] stormBiomes = new BiomeDefinition.BiomeType[]
	{
		BiomeDefinition.BiomeType.burnt_forest,
		BiomeDefinition.BiomeType.Desert,
		BiomeDefinition.BiomeType.Snow,
		BiomeDefinition.BiomeType.Wasteland
	};

	// Token: 0x04006B39 RID: 27449
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public WeatherManager.ParticleData[] stormData = new WeatherManager.ParticleData[4];

	// Token: 0x04006B3A RID: 27450
	public float spectrumBlend = 1f;

	// Token: 0x04006B3B RID: 27451
	public static SpectrumWeatherType forcedSpectrum = SpectrumWeatherType.None;

	// Token: 0x04006B3C RID: 27452
	public SpectrumWeatherType spectrumSourceType;

	// Token: 0x04006B3D RID: 27453
	public SpectrumWeatherType spectrumTargetType;

	// Token: 0x04006B3E RID: 27454
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static AtmosphereEffect[] atmosphereSpectrum;

	// Token: 0x04006B3F RID: 27455
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 playerPosition;

	// Token: 0x04006B40 RID: 27456
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float checkPlayerMoveTime;

	// Token: 0x04006B41 RID: 27457
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static MemoryStream loadData;

	// Token: 0x04006B42 RID: 27458
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string weatherAllName;

	// Token: 0x04006B43 RID: 27459
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float weatherLastUpdateWorldTime;

	// Token: 0x04006B44 RID: 27460
	public string CustomWeatherName = "";

	// Token: 0x04006B45 RID: 27461
	public float CustomWeatherTime = -1f;

	// Token: 0x04006B46 RID: 27462
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float cloudThickness;

	// Token: 0x04006B47 RID: 27463
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float cloudThicknessTarget;

	// Token: 0x04006B48 RID: 27464
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 particleFallLastPos;

	// Token: 0x04006B49 RID: 27465
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 particleFallPos;

	// Token: 0x04006B4A RID: 27466
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int processingPackageFrame;

	// Token: 0x0200110A RID: 4362
	[PublicizedFrom(EAccessModifier.Private)]
	public enum CloudTypes
	{
		// Token: 0x04006B4C RID: 27468
		Whispy,
		// Token: 0x04006B4D RID: 27469
		Fluffy,
		// Token: 0x04006B4E RID: 27470
		ThickOvercast
	}

	// Token: 0x0200110B RID: 4363
	public class ParticleData
	{
		// Token: 0x04006B4F RID: 27471
		public WeatherManager.BiomeWeather weather;

		// Token: 0x04006B50 RID: 27472
		public int stormLevel;

		// Token: 0x04006B51 RID: 27473
		public float intensity = -1f;

		// Token: 0x04006B52 RID: 27474
		public float thunderDelay;

		// Token: 0x04006B53 RID: 27475
		public GameObject rootObj;

		// Token: 0x04006B54 RID: 27476
		public Transform rootT;

		// Token: 0x04006B55 RID: 27477
		public Material rootParticleMat;

		// Token: 0x04006B56 RID: 27478
		public ParticleSystem rootParticleSys;

		// Token: 0x04006B57 RID: 27479
		public float rootEmissionMaxRate = 1f;

		// Token: 0x04006B58 RID: 27480
		public ParticleSystem nearParticleSys;

		// Token: 0x04006B59 RID: 27481
		public float nearEmissionMaxRate = 1f;

		// Token: 0x04006B5A RID: 27482
		public ParticleSystem topParticleSys;

		// Token: 0x04006B5B RID: 27483
		public float topEmissionMaxRate = 1f;

		// Token: 0x04006B5C RID: 27484
		public Transform farT;

		// Token: 0x04006B5D RID: 27485
		public ParticleSystem farParticleSys;

		// Token: 0x04006B5E RID: 27486
		public Color farBaseColor;

		// Token: 0x04006B5F RID: 27487
		public ParticleSystem ringParticleSys;

		// Token: 0x04006B60 RID: 27488
		public float ringRadiusMax;

		// Token: 0x04006B61 RID: 27489
		public List<ParticleSystem> particleSystems = new List<ParticleSystem>();

		// Token: 0x04006B62 RID: 27490
		public float[] psEmissionMaxRates;

		// Token: 0x04006B63 RID: 27491
		public Transform forceT;
	}

	// Token: 0x0200110C RID: 4364
	[Serializable]
	public class Param
	{
		// Token: 0x06008922 RID: 35106 RVA: 0x003793C0 File Offset: 0x003775C0
		public Param(float _value, float _step1Time = 0.25f)
		{
			this.name = "Param";
			this.value = _value;
			this.target = _value;
			this.step1Time = _step1Time;
		}

		// Token: 0x06008923 RID: 35107 RVA: 0x003793E8 File Offset: 0x003775E8
		public void FrameUpdate()
		{
			float time = Time.time;
			if (this.value == this.target)
			{
				this.lastTime = time;
				return;
			}
			float num = time - this.lastTime;
			if (num < 0f)
			{
				this.lastTime = time;
			}
			if (num >= 0.01f)
			{
				if (num > 1f)
				{
					num = 1f;
				}
				float num2 = num / this.step1Time;
				if (this.value > this.target)
				{
					this.value -= num2;
					if (this.value < this.target)
					{
						this.value = this.target;
					}
				}
				else
				{
					this.value += num2;
					if (this.value > this.target)
					{
						this.value = this.target;
					}
				}
				this.lastTime = time;
			}
		}

		// Token: 0x06008924 RID: 35108 RVA: 0x003794AF File Offset: 0x003776AF
		public void Clamp()
		{
			this.value = Utils.FastClamp01(this.value);
			this.target = Utils.FastClamp01(this.target);
		}

		// Token: 0x06008925 RID: 35109 RVA: 0x003794D3 File Offset: 0x003776D3
		public void Set(float _value)
		{
			this.value = _value;
			this.target = _value;
		}

		// Token: 0x06008926 RID: 35110 RVA: 0x003794E3 File Offset: 0x003776E3
		public void SetTarget(float _target)
		{
			if (this.target == _target)
			{
				return;
			}
			this.target = _target;
			this.lastTime = Time.time;
		}

		// Token: 0x04006B64 RID: 27492
		public string name;

		// Token: 0x04006B65 RID: 27493
		public float value;

		// Token: 0x04006B66 RID: 27494
		public float target;

		// Token: 0x04006B67 RID: 27495
		public float step1Time;

		// Token: 0x04006B68 RID: 27496
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public float lastTime;
	}

	// Token: 0x0200110D RID: 4365
	[Serializable]
	public class BiomeWeather
	{
		// Token: 0x06008927 RID: 35111 RVA: 0x00379504 File Offset: 0x00377704
		public BiomeWeather(BiomeDefinition _definition)
		{
			this.biomeDefinition = _definition;
			for (int i = 0; i < 5; i++)
			{
				this.parameters[i] = new WeatherManager.Param(0f, 0.3f);
			}
			this.parameters[0].step1Time = 0.15f;
		}

		// Token: 0x06008928 RID: 35112 RVA: 0x00379595 File Offset: 0x00377795
		public float CloudThickness()
		{
			return this.parameters[2].value;
		}

		// Token: 0x06008929 RID: 35113 RVA: 0x003795A4 File Offset: 0x003777A4
		public float FogPercent()
		{
			return this.parameters[4].value * 0.01f + WeatherManager.stormFog;
		}

		// Token: 0x0600892A RID: 35114 RVA: 0x003795BF File Offset: 0x003777BF
		public float Wind()
		{
			return this.parameters[3].value;
		}

		// Token: 0x0600892B RID: 35115 RVA: 0x003795D0 File Offset: 0x003777D0
		public void FrameUpdate(bool _isServer)
		{
			WeatherManager.Param[] array = this.parameters;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].FrameUpdate();
			}
			float value = this.parameters[1].value;
			float num = this.parameters[2].value;
			float num2 = this.parameters[0].value;
			this.CalcGlobalTemperature(value, num, ref num2);
			if (_isServer)
			{
				if (WeatherManager.forceClouds >= 0f)
				{
					num = WeatherManager.forceClouds * 100f;
				}
				if (WeatherManager.forceTemperature > -100f)
				{
					num2 = WeatherManager.forceTemperature;
				}
			}
			this.parameterFinals[1] = value;
			this.parameterFinals[2] = num;
			this.parameterFinals[0] = num2;
			this.parameterFinals[3] = this.parameters[3].value;
			this.parameterFinals[4] = this.parameters[4].value;
			float num3 = (value * 0.01f - 0.3f) / 0.7f;
			num3 = Utils.FastMax(0f, num3);
			this.rainParam.target = ((num2 > 32f) ? num3 : 0f);
			this.snowFallParam.target = ((num2 <= 32f) ? num3 : 0f);
			this.rainParam.FrameUpdate();
			this.snowFallParam.FrameUpdate();
		}

		// Token: 0x0600892C RID: 35116 RVA: 0x00379714 File Offset: 0x00377914
		[PublicizedFrom(EAccessModifier.Private)]
		public void CalcGlobalTemperature(float _precipitation, float _cloudThickness, ref float _temperature)
		{
			float num = Utils.FastClamp(_precipitation, 0f, 100f);
			_temperature -= num * 0.1f;
			float num2 = SkyManager.GetSunPercent();
			if (num2 > 0f)
			{
				num2 *= 1f - _cloudThickness * 0.01f;
			}
			_temperature += num2 * 10f;
		}

		// Token: 0x0600892D RID: 35117 RVA: 0x00379768 File Offset: 0x00377968
		public void ParamsFrameUpdate()
		{
			for (int i = 0; i < this.parameters.Length; i++)
			{
				WeatherManager.Param param = this.parameters[i];
				param.FrameUpdate();
				this.parameterFinals[i] = param.value;
			}
			this.rainParam.FrameUpdate();
			this.snowFallParam.FrameUpdate();
		}

		// Token: 0x0600892E RID: 35118 RVA: 0x003797BC File Offset: 0x003779BC
		public void Reset()
		{
			this.stormWorldTime = 0;
			this.nextRandWorldTime = 0;
			this.biomeDefinition.WeatherRandomize(0f);
			for (int i = 0; i < 5; i++)
			{
				this.parameters[i].Set(this.biomeDefinition.WeatherGetValue((BiomeDefinition.Probabilities.ProbType)i));
			}
			this.rainParam.Set(0f);
			this.snowFallParam.Set(0f);
		}

		// Token: 0x0600892F RID: 35119 RVA: 0x0037982C File Offset: 0x00377A2C
		public void ServerTimeUpdate(int _worldTime, float _freq)
		{
			if (_freq == 0f)
			{
				if (this.stormWorldTime != 2147483647)
				{
					this.stormWorldTime = int.MaxValue;
					this.nextRandWorldTime = 0;
				}
			}
			else if (this.stormWorldTime == 2147483647)
			{
				this.stormWorldTime = 0;
			}
			int num = _worldTime - this.stormWorldTime;
			if (num >= 0)
			{
				float num2 = 60f / (float)GamePrefs.GetInt(EnumGamePrefs.DayNightLength);
				int num3 = this.biomeDefinition.WeatherGetDuration("stormbuild");
				num3 = (int)((float)num3 * num2);
				int num4 = num3 - num;
				if (num4 > 0)
				{
					if (this.stormState != 1)
					{
						this.stormState = 1;
						this.SetWeather("stormbuild");
					}
					int @int = GameStats.GetInt(EnumGameStats.TimeOfDayIncPerSec);
					if (@int > 0)
					{
						this.remainingSeconds = (byte)Utils.FastMin(num4 / @int, 255);
					}
					return;
				}
				if (WeatherManager.worldTime < this.stormWorldTime + this.stormDuration)
				{
					if (this.stormState != 2)
					{
						this.stormState = 2;
						this.SetWeather("storm");
					}
					return;
				}
				this.stormState = 0;
				Vector2i vector2i;
				int num5 = this.biomeDefinition.WeatherGetDuration("storm", out vector2i);
				this.stormWorldTime = WeatherManager.worldTime + (int)((float)WeatherManager.gameRand.RandomRange(vector2i.x, vector2i.y) / _freq);
				int num6 = num5 / 8;
				num5 += WeatherManager.gameRand.RandomRange(num5 - num6, num5 + num6);
				num5 = (int)((float)num5 * num2);
				this.stormDuration = num3 + num5;
			}
			if (_worldTime >= this.nextRandWorldTime)
			{
				float rand = WeatherManager.gameRand.RandomFloat;
				if (WeatherManager.forceSimRandom >= 0f)
				{
					rand = WeatherManager.forceSimRandom;
				}
				this.SetWeather(_worldTime, rand);
			}
		}

		// Token: 0x06008930 RID: 35120 RVA: 0x003799C8 File Offset: 0x00377BC8
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetWeather(int _worldTime, float _rand)
		{
			if (this.biomeDefinition != null)
			{
				this.biomeDefinition.WeatherRandomize(_rand);
				for (int i = 0; i < 5; i++)
				{
					BiomeDefinition.Probabilities.ProbType type = (BiomeDefinition.Probabilities.ProbType)i;
					this.parameters[i].target = this.biomeDefinition.WeatherGetValue(type);
				}
				this.nextRandWorldTime = _worldTime + this.biomeDefinition.currentWeatherGroup.duration;
			}
		}

		// Token: 0x06008931 RID: 35121 RVA: 0x00379A28 File Offset: 0x00377C28
		public void SetWeather(string name)
		{
			if (this.biomeDefinition != null)
			{
				this.biomeDefinition.WeatherRandomize(name);
				for (int i = 0; i < 5; i++)
				{
					BiomeDefinition.Probabilities.ProbType type = (BiomeDefinition.Probabilities.ProbType)i;
					this.parameters[i].target = this.biomeDefinition.WeatherGetValue(type);
				}
				this.nextRandWorldTime = 0;
			}
		}

		// Token: 0x06008932 RID: 35122 RVA: 0x00379A78 File Offset: 0x00377C78
		public override string ToString()
		{
			string str = string.Format("{0}: {1}, nxtT{2}, ", this.biomeDefinition.m_sBiomeName, this.biomeDefinition.weatherName, this.nextRandWorldTime);
			for (int i = 0; i < 5; i++)
			{
				BiomeDefinition.Probabilities.ProbType probType = (BiomeDefinition.Probabilities.ProbType)i;
				str += string.Format("{0} {1}, ", probType, this.biomeDefinition.WeatherGetValue(probType));
			}
			str += string.Format("rain {0}, ", this.rainParam.value);
			str += string.Format("snow {0}, ", this.snowFallParam.value);
			return str + string.Format("storm WT {0}, dur {1}, state {2}", this.stormWorldTime, this.stormDuration, this.stormState);
		}

		// Token: 0x04006B69 RID: 27497
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public const float cStep1Time = 0.15f;

		// Token: 0x04006B6A RID: 27498
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public const float cPrecipitationPercentVisibleMin = 0.3f;

		// Token: 0x04006B6B RID: 27499
		public BiomeDefinition biomeDefinition;

		// Token: 0x04006B6C RID: 27500
		public int stormWorldTime;

		// Token: 0x04006B6D RID: 27501
		public int stormDuration;

		// Token: 0x04006B6E RID: 27502
		public int stormState;

		// Token: 0x04006B6F RID: 27503
		public int nextRandWorldTime;

		// Token: 0x04006B70 RID: 27504
		public byte remainingSeconds;

		// Token: 0x04006B71 RID: 27505
		public WeatherManager.Param[] parameters = new WeatherManager.Param[5];

		// Token: 0x04006B72 RID: 27506
		public float[] parameterFinals = new float[5];

		// Token: 0x04006B73 RID: 27507
		public WeatherManager.Param rainParam = new WeatherManager.Param(0f, 0.25f);

		// Token: 0x04006B74 RID: 27508
		public WeatherManager.Param snowFallParam = new WeatherManager.Param(0f, 0.25f);
	}
}
