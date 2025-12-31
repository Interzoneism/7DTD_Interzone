using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Platform;
using UnityEngine;

// Token: 0x02001158 RID: 4440
public static class DebugGameStats
{
	// Token: 0x06008B14 RID: 35604 RVA: 0x00383328 File Offset: 0x00381528
	public static void StartStatisticsUpdate(DebugGameStats.StatisticsUpdatedCallback callback)
	{
		if (DebugGameStats.m_memorySampler == null)
		{
			IPlatformMemory memory = PlatformManager.MultiPlatform.Memory;
			DebugGameStats.m_memorySampler = ((memory != null) ? memory.CreateSampler() : null);
		}
		IPlatformMemorySampler memorySampler = DebugGameStats.m_memorySampler;
		object obj;
		if (memorySampler == null)
		{
			obj = null;
		}
		else
		{
			IReadOnlyList<IPlatformMemoryStat> statistics = memorySampler.Statistics;
			if (statistics == null)
			{
				obj = null;
			}
			else
			{
				obj = statistics.FirstOrDefault((IPlatformMemoryStat s) => s.Name == "GameUsed");
			}
		}
		DebugGameStats.m_memoryGameUsedStat = (IPlatformMemoryStat<long>)obj;
		DebugGameStats.TryInitializeStatisticsDictionary();
		if (DebugGameStats.updateStatsCoroutine != null)
		{
			ThreadManager.StopCoroutine(DebugGameStats.updateStatsCoroutine);
		}
		if (DebugGameStats.updateDeltasCoroutine != null)
		{
			ThreadManager.StopCoroutine(DebugGameStats.updateDeltasCoroutine);
		}
		DebugGameStats.doStatisticsUpdate = true;
		DebugGameStats.updateStatsCoroutine = ThreadManager.StartCoroutine(DebugGameStats.UpdateStatisticsCo(callback));
		DebugGameStats.updateDeltasCoroutine = ThreadManager.StartCoroutine(DebugGameStats.UpdateDeltas());
	}

	// Token: 0x06008B15 RID: 35605 RVA: 0x003833EC File Offset: 0x003815EC
	public static void TryInitializeStatisticsDictionary()
	{
		if (DebugGameStats.statisticsDictionary.Keys.Count > 0)
		{
			return;
		}
		foreach (FieldInfo fieldInfo in typeof(DebugGameStats.Statistics).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy))
		{
			if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly && fieldInfo.FieldType == typeof(string))
			{
				string key = (string)fieldInfo.GetValue(null);
				if (!DebugGameStats.statisticsDictionary.ContainsKey(key))
				{
					DebugGameStats.statisticsDictionary[key] = string.Empty;
				}
			}
		}
	}

	// Token: 0x06008B16 RID: 35606 RVA: 0x00383481 File Offset: 0x00381681
	public static void StopStatisticsUpdate()
	{
		DebugGameStats.doStatisticsUpdate = false;
		DebugGameStats.updateStatsCoroutine = null;
		DebugGameStats.updateDeltasCoroutine = null;
	}

	// Token: 0x06008B17 RID: 35607 RVA: 0x00383498 File Offset: 0x00381698
	public static string GetHeader(char separator)
	{
		DebugGameStats.TryInitializeStatisticsDictionary();
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, string> keyValuePair in DebugGameStats.statisticsDictionary)
		{
			stringBuilder.Append(keyValuePair.Key);
			stringBuilder.Append(separator);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06008B18 RID: 35608 RVA: 0x0038350C File Offset: 0x0038170C
	public static string GetCurrentStatsString(char separator = ',')
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, string> keyValuePair in DebugGameStats.statisticsDictionary)
		{
			stringBuilder.Append(keyValuePair.Value);
			stringBuilder.Append(separator);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06008B19 RID: 35609 RVA: 0x0038357C File Offset: 0x0038177C
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator UpdateDeltas()
	{
		while (DebugGameStats.doStatisticsUpdate)
		{
			DebugGameStats.deltaTextureMemory += (long)(Texture.currentTextureMemory - (ulong)DebugGameStats.m_textureMemoryPrevFrame);
			DebugGameStats.m_textureMemoryPrevFrame = (long)Texture.currentTextureMemory;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06008B1A RID: 35610 RVA: 0x00383584 File Offset: 0x00381784
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator UpdateStatisticsCo(DebugGameStats.StatisticsUpdatedCallback callback)
	{
		long num = 0L;
		while (DebugGameStats.doStatisticsUpdate)
		{
			DebugGameStats.statisticsDictionary["gamestats.ingametime"] = (GameTimer.Instance.ticksSincePlayfieldLoaded / 20UL).ToString();
			if (DebugGameStats.m_memorySampler != null)
			{
				DebugGameStats.m_memorySampler.Sample();
				if (DebugGameStats.m_memoryGameUsedStat != null && DebugGameStats.m_memoryGameUsedStat.TryGet(MemoryStatColumn.Current, out num))
				{
					DebugGameStats.statisticsDictionary["gamestats.nativegameused"] = (num / 1024L).ToString();
				}
			}
			DebugGameStats.statisticsDictionary["gamestats.entityinstances"] = Entity.InstanceCount.ToString();
			DebugGameStats.statisticsDictionary["gamestats.maxusedchunks"] = Chunk.InstanceCount.ToString();
			DebugGameStats.statisticsDictionary["gamestats.displayedprefabs"] = GameManager.Instance.prefabLODManager.displayedPrefabs.Count.ToString();
			long currentTextureMemory = (long)Texture.currentTextureMemory;
			DebugGameStats.statisticsDictionary["gamestats.texturememorydelta60"] = (DebugGameStats.deltaTextureMemory / 1024L).ToString();
			DebugGameStats.deltaTextureMemory = 0L;
			DebugGameStats.statisticsDictionary["gamestats.texturememorycurrent"] = (currentTextureMemory / 1024L).ToString();
			DebugGameStats.statisticsDictionary["gamestats.textureMemorydesired"] = (Texture.desiredTextureMemory / 1024UL).ToString();
			Log.Out(string.Concat(new string[]
			{
				"60sec delta: ",
				DebugGameStats.statisticsDictionary["gamestats.texturememorydelta60"],
				",current: ",
				DebugGameStats.statisticsDictionary["gamestats.texturememorycurrent"],
				",desired: ",
				DebugGameStats.statisticsDictionary["gamestats.textureMemorydesired"]
			}));
			int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxUpscalerMode);
			if (@int - 1 <= 1)
			{
				DebugGameStats.statisticsDictionary["gamestats.VideoScalingSetting"] = string.Format("{0}, Quality preset {1}, FSR preset {2}", GameOptionsPlatforms.UpscalerMode.ToString(@int), GamePrefs.GetInt(EnumGamePrefs.OptionsGfxQualityPreset), GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFSRPreset));
			}
			else
			{
				DebugGameStats.statisticsDictionary["gamestats.VideoScalingSetting"] = string.Format("{0}, Quality preset {1}", GameOptionsPlatforms.UpscalerMode.ToString(@int), GamePrefs.GetInt(EnumGamePrefs.OptionsGfxQualityPreset));
			}
			if (GameManager.Instance.World != null)
			{
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
				{
					DebugGameStats.statisticsDictionary["gamestats.ConnectionStatus"] = "Connected";
				}
				else
				{
					DebugGameStats.statisticsDictionary["gamestats.ConnectionStatus"] = "Disconnected";
				}
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsSinglePlayer)
				{
					DebugGameStats.statisticsDictionary["gamestats.HostStatus"] = "SinglePlayer";
				}
				else if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					DebugGameStats.statisticsDictionary["gamestats.HostStatus"] = "MultiplayerHostOrServer";
				}
				else if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
				{
					DebugGameStats.statisticsDictionary["gamestats.HostStatus"] = "Client";
				}
				else
				{
					DebugGameStats.statisticsDictionary["gamestats.HostStatus"] = "Unknown";
				}
				Dictionary<string, string> dictionary = DebugGameStats.statisticsDictionary;
				string key = "gamestats.gameMode";
				GameMode gameMode = GameManager.Instance.GetGameStateManager().GetGameMode();
				dictionary[key] = ((gameMode != null) ? gameMode.GetName() : null);
				DebugGameStats.statisticsDictionary["gamestats.PlayerCount"] = string.Format("Clients: {0}", SingletonMonoBehaviour<ConnectionManager>.Instance.ClientCount());
				DebugGameStats.statisticsDictionary["gamestats.worldentities"] = GameManager.Instance.World.Entities.Count.ToString();
				DebugGameStats.statisticsDictionary["gamestats.chunkobservers"] = GameManager.Instance.World.m_ChunkManager.m_ObservedEntities.Count.ToString();
				DebugGameStats.statisticsDictionary["gamestats.syncedchunks"] = GameManager.Instance.World.ChunkCache.chunks.list.Count.ToString();
				DebugGameStats.statisticsDictionary["gamestats.chunkgameobjects"] = GameManager.Instance.World.m_ChunkManager.GetDisplayedChunkGameObjectsCount().ToString();
				DebugGameStats.statisticsDictionary["gamestats.worldtime"] = ValueDisplayFormatters.WorldTime(GameManager.Instance.World.worldTime, "Day {0}, {1:00}:{2:00}");
				DebugGameStats.statisticsDictionary["gamestats.isbloodmoon"] = GameManager.Instance.World.isEventBloodMoon.ToString();
				EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
				if (primaryPlayer)
				{
					DebugGameStats.statisticsDictionary["gamestats.currentPlayerBiome"] = string.Format("Player biome {0}", primaryPlayer.biomeStandingOn);
				}
				if (WeatherManager.Instance)
				{
					Color fogColor = SkyManager.GetFogColor();
					WeatherManager.BiomeWeather currentWeather = WeatherManager.currentWeather;
					bool flag = currentWeather != null;
					DebugGameStats.statisticsDictionary["gamestats.weathersystemstatus"] = string.Concat(new string[]
					{
						"WM status: ",
						WeatherManager.Instance.ToString(),
						"\n PlayerCurrent: <Temp: ",
						WeatherManager.GetTemperature().ToCultureInvariantString(),
						" Clouds ",
						(WeatherManager.GetCloudThickness() * 0.01f).ToCultureInvariantString(),
						" Fog density ",
						SkyManager.GetFogDensity().ToCultureInvariantString(),
						", start ",
						SkyManager.GetFogStart().ToCultureInvariantString(),
						", end ",
						SkyManager.GetFogEnd().ToCultureInvariantString(),
						" FogColor ",
						fogColor.r.ToCultureInvariantString(),
						" ",
						fogColor.g.ToCultureInvariantString(),
						" ",
						fogColor.b.ToCultureInvariantString(),
						" Rain  ",
						((WeatherManager.forceRain >= 0f) ? WeatherManager.forceRain : (flag ? currentWeather.rainParam.value : 0f)).ToCultureInvariantString(),
						" Snowfall  ",
						((WeatherManager.forceSnowfall >= 0f) ? WeatherManager.forceSnowfall : (flag ? currentWeather.snowFallParam.value : 0f)).ToCultureInvariantString(),
						" Temperature ",
						WeatherManager.GetTemperature().ToCultureInvariantString(),
						" Wind ",
						WeatherManager.GetWindSpeed().ToCultureInvariantString(),
						">"
					});
				}
				if (GameManager.Instance.World.GetLocalPlayers().Count > 0)
				{
					if (!DiscordManager.Instance.IsInitialized)
					{
						DebugGameStats.statisticsDictionary["gamestats.discordstatus"] = (DiscordManager.Instance.Settings.DiscordDisabled ? "Disabled and not initialized" : "Not initialized");
					}
					else if (!DiscordManager.Instance.IsReady)
					{
						DebugGameStats.statisticsDictionary["gamestats.discordstatus"] = (DiscordManager.Instance.Settings.DiscordDisabled ? "Disabled and not ready" : "Not ready");
					}
					else
					{
						DebugGameStats.statisticsDictionary["gamestats.discordstatus"] = "Discord enabled initialized and ready";
						DebugGameStats.statisticsDictionary["gamestats.discordsettings"] = SdPlayerPrefs.GetString("DiscordSettings");
					}
					DebugGameStats.statisticsDictionary["gamestats.localplayerpos"] = GameManager.Instance.World.GetLocalPlayers()[0].position.ToString();
				}
			}
			Log.Out("[Backtrace] Updated Statistics");
			callback(DebugGameStats.statisticsDictionary);
			yield return new WaitForSeconds(60f);
		}
		yield break;
	}

	// Token: 0x04006CBD RID: 27837
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, string> statisticsDictionary = new Dictionary<string, string>();

	// Token: 0x04006CBE RID: 27838
	[PublicizedFrom(EAccessModifier.Private)]
	public static IPlatformMemorySampler m_memorySampler;

	// Token: 0x04006CBF RID: 27839
	[PublicizedFrom(EAccessModifier.Private)]
	public static IPlatformMemoryStat<long> m_memoryGameUsedStat;

	// Token: 0x04006CC0 RID: 27840
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool doStatisticsUpdate = false;

	// Token: 0x04006CC1 RID: 27841
	[PublicizedFrom(EAccessModifier.Private)]
	public static Coroutine updateStatsCoroutine;

	// Token: 0x04006CC2 RID: 27842
	[PublicizedFrom(EAccessModifier.Private)]
	public static Coroutine updateDeltasCoroutine;

	// Token: 0x04006CC3 RID: 27843
	[PublicizedFrom(EAccessModifier.Private)]
	public static long deltaTextureMemory = 0L;

	// Token: 0x04006CC4 RID: 27844
	[PublicizedFrom(EAccessModifier.Private)]
	public static long m_textureMemoryPrevFrame = 0L;

	// Token: 0x02001159 RID: 4441
	[PublicizedFrom(EAccessModifier.Private)]
	public static class Statistics
	{
		// Token: 0x04006CC5 RID: 27845
		public const string InGameTimeKey = "gamestats.ingametime";

		// Token: 0x04006CC6 RID: 27846
		public const string ManagedHeapSizeKey = "gamestats.managedheap";

		// Token: 0x04006CC7 RID: 27847
		public const string NativeGameUsedKey = "gamestats.nativegameused";

		// Token: 0x04006CC8 RID: 27848
		public const string TextureMemoryDesiredKey = "gamestats.textureMemorydesired";

		// Token: 0x04006CC9 RID: 27849
		public const string TextureMemoryCurrentKey = "gamestats.texturememorycurrent";

		// Token: 0x04006CCA RID: 27850
		public const string TextureMemoryDelta60Key = "gamestats.texturememorydelta60";

		// Token: 0x04006CCB RID: 27851
		public const string WorldEntitiesCountKey = "gamestats.worldentities";

		// Token: 0x04006CCC RID: 27852
		public const string EntityInstanceCountKey = "gamestats.entityinstances";

		// Token: 0x04006CCD RID: 27853
		public const string ChunkObserverCountKey = "gamestats.chunkobservers";

		// Token: 0x04006CCE RID: 27854
		public const string MaxUsedChunkCountKey = "gamestats.maxusedchunks";

		// Token: 0x04006CCF RID: 27855
		public const string SyncedChunkCountKey = "gamestats.syncedchunks";

		// Token: 0x04006CD0 RID: 27856
		public const string ChunkGameObjectCountKey = "gamestats.chunkgameobjects";

		// Token: 0x04006CD1 RID: 27857
		public const string DisplayedPrefabCountKey = "gamestats.displayedprefabs";

		// Token: 0x04006CD2 RID: 27858
		public const string LocalPlayerPositionKey = "gamestats.localplayerpos";

		// Token: 0x04006CD3 RID: 27859
		public const string WorldTimeKey = "gamestats.worldtime";

		// Token: 0x04006CD4 RID: 27860
		public const string BloodMoonKey = "gamestats.isbloodmoon";

		// Token: 0x04006CD5 RID: 27861
		public const string GameModeKey = "gamestats.gameMode";

		// Token: 0x04006CD6 RID: 27862
		public const string PlayerCountKey = "gamestats.PlayerCount";

		// Token: 0x04006CD7 RID: 27863
		public const string ConnectStatusKey = "gamestats.ConnectionStatus";

		// Token: 0x04006CD8 RID: 27864
		public const string HostStatusKey = "gamestats.HostStatus";

		// Token: 0x04006CD9 RID: 27865
		public const string CurrentBiomeKey = "gamestats.currentPlayerBiome";

		// Token: 0x04006CDA RID: 27866
		public const string WeatherStatusKey = "gamestats.weathersystemstatus";

		// Token: 0x04006CDB RID: 27867
		public const string VideoScalingSetting = "gamestats.VideoScalingSetting";

		// Token: 0x04006CDC RID: 27868
		public const string DiscordPluginStatusKey = "gamestats.discordstatus";

		// Token: 0x04006CDD RID: 27869
		public const string DiscordPluginSettingsKey = "gamestats.discordsettings";
	}

	// Token: 0x0200115A RID: 4442
	// (Invoke) Token: 0x06008B1D RID: 35613
	public delegate void StatisticsUpdatedCallback(Dictionary<string, string> statisticsDictionary);
}
