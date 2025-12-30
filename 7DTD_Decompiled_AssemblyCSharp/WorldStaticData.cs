using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Challenges;
using DynamicMusic.Legacy.ObjectModel;
using Noemax.GZip;
using Platform;
using Twitch;
using UnityEngine;
using WorldGenerationEngineFinal;
using XMLData.Item;

// Token: 0x0200109F RID: 4255
public static class WorldStaticData
{
	// Token: 0x0600862E RID: 34350 RVA: 0x00367E54 File Offset: 0x00366054
	public static void InitSync(bool _bForce, bool _bDediServer, bool _cleanup = true)
	{
		if (_cleanup)
		{
			WorldStaticData.Cleanup();
		}
		LoadManager.InitSync();
		ThreadManager.RunCoroutineSync(WorldStaticData.Init(_bForce, _bDediServer, null));
	}

	// Token: 0x0600862F RID: 34351 RVA: 0x00367E70 File Offset: 0x00366070
	public static IEnumerator Init(bool _bForce, bool _bDediServer, WorldStaticData.ProgressDelegate _progressDelegate)
	{
		WorldStaticData.isDediServer = _bDediServer;
		if (!_bForce && WorldStaticData.bInitDone)
		{
			Log.Out("WorldStaticData.Init() was already done");
			yield break;
		}
		MicroStopwatch sw = new MicroStopwatch();
		if (_progressDelegate != null)
		{
			_progressDelegate(Localization.Get("loadActionParticles", false), 0f);
		}
		yield return null;
		yield return null;
		yield return ParticleEffect.LoadResources();
		List<LoadManager.AssetRequestTask<GameObject>> explosionPrefabLoadTasks = new List<LoadManager.AssetRequestTask<GameObject>>();
		int num;
		for (int i = 0; i < 100; i = num + 1)
		{
			explosionPrefabLoadTasks.Add(LoadManager.LoadAssetFromAddressables<GameObject>(string.Format("Prefabs/prefabExplosion{0}.prefab", i), null, null, false, false, false));
			yield return null;
			num = i;
		}
		yield return LoadManager.WaitAll(explosionPrefabLoadTasks);
		WorldStaticData.prefabExplosions = new Transform[100];
		for (int j = 0; j < 100; j++)
		{
			LoadManager.AssetRequestTask<GameObject> assetRequestTask = explosionPrefabLoadTasks[j];
			if (assetRequestTask.Asset)
			{
				WorldStaticData.prefabExplosions[j] = assetRequestTask.Asset.transform;
			}
		}
		if (_progressDelegate != null)
		{
			_progressDelegate(Localization.Get("loadActionBlockTextures", false), 0f);
		}
		yield return null;
		yield return null;
		if (!WorldStaticData.meshDescCol)
		{
			WorldStaticData.<>c__DisplayClass13_0 CS$<>8__locals1 = new WorldStaticData.<>c__DisplayClass13_0();
			bool loadSync = !Application.isPlaying;
			LoadManager.AssetRequestTask<GameObject> request = LoadManager.LoadAssetFromAddressables<GameObject>("BlockTextureAtlases", "prefabMeshDescriptionCollection.prefab", null, null, false, loadSync, false);
			yield return request;
			GameObject asset = request.Asset;
			if (asset == null)
			{
				throw new Exception("Missing resource bundle BlockTextureAtlases");
			}
			WorldStaticData.meshDescCol = asset.GetComponent<MeshDescriptionCollection>();
			WorldStaticData.meshDescCol.Init();
			CS$<>8__locals1.coroutineHadException = false;
			yield return ThreadManager.CoroutineWrapperWithExceptionCallback(WorldStaticData.meshDescCol.LoadTextureArraysForQuality(false), delegate(Exception _exception)
			{
				Log.Error("WSD.Init: Initializing MDC failed");
				Log.Exception(_exception);
				CS$<>8__locals1.coroutineHadException = true;
			});
			if (CS$<>8__locals1.coroutineHadException)
			{
				yield break;
			}
			CS$<>8__locals1 = null;
			request = null;
		}
		if (GameManager.IsDedicatedServer)
		{
			for (int k = 0; k < WorldStaticData.meshDescCol.meshes.Length; k++)
			{
				WorldStaticData.meshDescCol.meshes[k].TexDiffuse = null;
				WorldStaticData.meshDescCol.meshes[k].TexNormal = null;
				WorldStaticData.meshDescCol.meshes[k].TexSpecular = null;
				WorldStaticData.meshDescCol.meshes[k].TexEmission = null;
				WorldStaticData.meshDescCol.meshes[k].TexHeight = null;
				WorldStaticData.meshDescCol.meshes[k].TexOcclusion = null;
				WorldStaticData.meshDescCol.meshes[k].TexMask = null;
				WorldStaticData.meshDescCol.meshes[k].TexMaskNormal = null;
			}
		}
		yield return WorldStaticData.LoadAllXmlsCo(!_bForce, _progressDelegate);
		WorldStaticData.bInitDone = true;
		Log.Out(string.Format("WorldStaticData.Init() needed {0:F3}s", sw.ElapsedMilliseconds / 1000L));
		GameOptionsManager.TextureQualityChanged += delegate(int _obj)
		{
			ThreadManager.RunCoroutineSync(WorldStaticData.meshDescCol.LoadTextureArraysForQuality(true));
		};
		GameOptionsManager.TextureFilterChanged += delegate(int _obj)
		{
			WorldStaticData.meshDescCol.SetTextureArraysFilter();
		};
		yield break;
	}

	// Token: 0x06008630 RID: 34352 RVA: 0x00367E8D File Offset: 0x0036608D
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadTextureAtlases()
	{
		MeshDescription[] meshes = WorldStaticData.meshDescCol.Meshes;
		MeshDescription.meshes = new MeshDescription[meshes.Length];
		int num;
		for (int i = 0; i < meshes.Length; i = num + 1)
		{
			MeshDescription.meshes[i] = meshes[i];
			string textureAtlasClass = meshes[i].TextureAtlasClass;
			Type type = Type.GetType(textureAtlasClass);
			if (type == null)
			{
				Log.Error(string.Format("Could not find type '{0}' for texture atlas on mesh layer {1}", textureAtlasClass, i));
				yield break;
			}
			TextureAtlas textureAtlas = (TextureAtlas)Activator.CreateInstance(type);
			textureAtlas.LoadTextureAtlas(i, WorldStaticData.meshDescCol, !WorldStaticData.isDediServer);
			yield return MeshDescription.meshes[i].Init(i, textureAtlas);
			yield return null;
			num = i;
		}
		MeshDescription.SetGrassQuality();
		MeshDescription.SetWaterQuality();
		yield break;
	}

	// Token: 0x06008631 RID: 34353 RVA: 0x00367E95 File Offset: 0x00366095
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadMaterials(XmlFile _xmlFile)
	{
		AIDirectorData.InitStatic();
		yield return MaterialsFromXml.CreateMaterials(_xmlFile);
		yield break;
	}

	// Token: 0x06008632 RID: 34354 RVA: 0x00367EA4 File Offset: 0x003660A4
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadBlocks(XmlFile _xmlFile)
	{
		Block.InitStatic();
		yield return BlocksFromXml.CreateBlocks(_xmlFile, false, GameManager.Instance != null && GameManager.Instance.IsEditMode());
		Block.AssignIds();
		yield break;
	}

	// Token: 0x06008633 RID: 34355 RVA: 0x00367EB3 File Offset: 0x003660B3
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadMisc(XmlFile _xmlFile)
	{
		AnimationDelayData.InitStatic();
		AnimationGunjointOffsetData.InitStatic();
		yield return MiscFromXml.Create(_xmlFile);
		yield break;
	}

	// Token: 0x06008634 RID: 34356 RVA: 0x00367EC2 File Offset: 0x003660C2
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ReloadMisc(XmlFile _xmlFile)
	{
		AnimationDelayData.InitStatic();
		AnimationGunjointOffsetData.InitStatic();
		ThreadManager.RunCoroutineSync(MiscFromXml.Create(_xmlFile));
	}

	// Token: 0x06008635 RID: 34357 RVA: 0x00367ED9 File Offset: 0x003660D9
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ReloadItems(XmlFile _xmlFile)
	{
		ItemClass.Cleanup();
		ThreadManager.RunCoroutineSync(WorldStaticData.LoadItems(_xmlFile));
	}

	// Token: 0x06008636 RID: 34358 RVA: 0x00367EEC File Offset: 0x003660EC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ReloadItemModifiers(XmlFile _xmlFile)
	{
		ThreadManager.RunCoroutineSync(WorldStaticData.LoadItemModifiers(_xmlFile));
		ThreadManager.RunCoroutineSync(WorldStaticData.LateInitItems());
		if (GameManager.Instance != null && GameManager.Instance.World != null && GameManager.Instance.World.GetPrimaryPlayer() != null)
		{
			GameManager.Instance.World.GetPrimaryPlayer().inventory.ForceHoldingItemUpdate();
		}
	}

	// Token: 0x06008637 RID: 34359 RVA: 0x00367F57 File Offset: 0x00366157
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadItems(XmlFile _xmlFile)
	{
		CraftingManager.ClearLockedData();
		Equipment.SetupCosmeticMapping();
		ItemClass.InitStatic();
		ItemClassesFromXml.CreateItemsFromBlocks();
		yield return null;
		yield return ItemClassesFromXml.CreateItems(_xmlFile);
		ItemData.Parser.idOffset = Block.ItemsStartHere;
		yield break;
	}

	// Token: 0x06008638 RID: 34360 RVA: 0x00367F66 File Offset: 0x00366166
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LateInitItems()
	{
		ItemClass.AssignIds();
		Block.LateInitAll();
		ItemClass.LateInitAll();
		yield break;
	}

	// Token: 0x06008639 RID: 34361 RVA: 0x00367F6E File Offset: 0x0036616E
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadItemModifiers(XmlFile _xmlFile)
	{
		yield return ItemModificationsFromXml.Load(_xmlFile);
		yield break;
	}

	// Token: 0x0600863A RID: 34362 RVA: 0x00367F7D File Offset: 0x0036617D
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadRecipes(XmlFile _xmlFile)
	{
		yield return RecipesFromXml.LoadRecipies(_xmlFile);
		List<Recipe> allRecipes = CraftingManager.GetAllRecipes();
		Dictionary<string, List<Recipe>> recipesByName = new Dictionary<string, List<Recipe>>();
		foreach (Recipe recipe in allRecipes)
		{
			string name = recipe.GetName();
			List<Recipe> list;
			if (!recipesByName.TryGetValue(name, out list))
			{
				list = new List<Recipe>();
				recipesByName[name] = list;
			}
			list.Add(recipe);
		}
		MicroStopwatch msw = new MicroStopwatch(true);
		List<string> recipeCalcStack = new List<string>();
		foreach (ItemClass itemClass in ItemClass.list)
		{
			if (itemClass != null)
			{
				recipeCalcStack.Clear();
				itemClass.AutoCalcWeight(recipesByName);
				if (itemClass.AutoCalcEcoVal(recipesByName, recipeCalcStack) < 0f)
				{
					Log.Warning("Loading recipes: Could not calculate eco value for item " + itemClass.GetItemName() + ": Only recursive recipes found");
				}
				if (recipeCalcStack.Count > 0)
				{
					Log.Warning("Loading recipes: Eco value calculation stack not empty for item " + itemClass.GetItemName() + ": " + string.Join(" > ", recipeCalcStack));
				}
				if (msw.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
				{
					yield return null;
					msw.ResetAndRestart();
				}
			}
		}
		ItemClass[] array = null;
		yield break;
	}

	// Token: 0x0600863B RID: 34363 RVA: 0x00367F8C File Offset: 0x0036618C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ReloadRecipes(XmlFile _xmlFile)
	{
		CraftingManager.ClearAllRecipes();
		ThreadManager.RunCoroutineSync(WorldStaticData.LoadRecipes(_xmlFile));
	}

	// Token: 0x0600863C RID: 34364 RVA: 0x00367F9E File Offset: 0x0036619E
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadLoot(XmlFile _xmlFile)
	{
		LootContainer.InitStatic();
		yield return LootFromXml.LoadLootContainers(_xmlFile);
		yield break;
	}

	// Token: 0x0600863D RID: 34365 RVA: 0x00367FAD File Offset: 0x003661AD
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ReloadLoot(XmlFile _xmlFile)
	{
		LootContainer.Cleanup();
		ThreadManager.RunCoroutineSync(WorldStaticData.LoadLoot(_xmlFile));
	}

	// Token: 0x0600863E RID: 34366 RVA: 0x00367FBF File Offset: 0x003661BF
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadWeather(XmlFile _xmlFile)
	{
		WeatherSurvivalParametersFromXml.Load(_xmlFile);
		WorldStaticData.LinkBuffs();
		yield break;
	}

	// Token: 0x0600863F RID: 34367 RVA: 0x00367FCE File Offset: 0x003661CE
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadSDCSArchetypes(XmlFile _xmlFile)
	{
		SDCSArchetypesFromXml.Load(_xmlFile);
		yield break;
	}

	// Token: 0x06008640 RID: 34368 RVA: 0x00367FDD File Offset: 0x003661DD
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadTraders(XmlFile _xmlFile)
	{
		TraderInfo.InitStatic();
		yield return TradersFromXml.LoadTraderInfo(_xmlFile);
		yield break;
	}

	// Token: 0x06008641 RID: 34369 RVA: 0x00367FEC File Offset: 0x003661EC
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadNpc(XmlFile _xmlFile)
	{
		NPCInfo.InitStatic();
		yield return NPCsFromXml.LoadNPCInfo(_xmlFile);
		yield break;
	}

	// Token: 0x06008642 RID: 34370 RVA: 0x00367FFB File Offset: 0x003661FB
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadMusic(XmlFile _xmlFile)
	{
		MusicGroup.InitStatic();
		yield return MusicDataFromXml.Load(_xmlFile);
		yield break;
	}

	// Token: 0x06008643 RID: 34371 RVA: 0x0036800A File Offset: 0x0036620A
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadUIDisplayInfo(XmlFile _xmlFile)
	{
		UIDisplayInfoManager.Reset();
		yield return UIDisplayInfoFromXml.Load(_xmlFile);
		yield break;
	}

	// Token: 0x06008644 RID: 34372 RVA: 0x00368019 File Offset: 0x00366219
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadBiomes(XmlFile _xmlFile)
	{
		new WorldBiomes(_xmlFile.XmlDoc, true);
		yield break;
	}

	// Token: 0x06008645 RID: 34373 RVA: 0x00368028 File Offset: 0x00366228
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadSpawning(XmlFile _xmlFile)
	{
		EntitySpawnerClassesFromXml.LoadEntitySpawnerClasses(_xmlFile.XmlDoc);
		yield return BiomeSpawningFromXml.Load(_xmlFile);
		yield break;
	}

	// Token: 0x06008646 RID: 34374 RVA: 0x00368037 File Offset: 0x00366237
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CleanupBlocks()
	{
		AIDirectorData.Cleanup();
		Block.Cleanup();
		TileEntityCompositeData.Cleanup();
	}

	// Token: 0x06008647 RID: 34375 RVA: 0x00368048 File Offset: 0x00366248
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CleanupGamestages()
	{
		GameStageDefinition.Clear();
		GameStageGroup.Clear();
	}

	// Token: 0x06008648 RID: 34376 RVA: 0x00368054 File Offset: 0x00366254
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CleanupChallenges()
	{
		ChallengeClass.Cleanup();
	}

	// Token: 0x06008649 RID: 34377 RVA: 0x0036805B File Offset: 0x0036625B
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CleanupGameEvents()
	{
		if (GameEventManager.HasInstance)
		{
			GameEventManager.Current.Cleanup();
		}
	}

	// Token: 0x0600864A RID: 34378 RVA: 0x0036806E File Offset: 0x0036626E
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CleanupTwitch()
	{
		if (TwitchActionManager.HasInstance)
		{
			TwitchActionManager.Current.Cleanup();
		}
	}

	// Token: 0x0600864B RID: 34379 RVA: 0x00368081 File Offset: 0x00366281
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CleanupTwitchEvents()
	{
		if (TwitchManager.HasInstance)
		{
			TwitchManager.Current.CleanupEventData();
		}
	}

	// Token: 0x0600864C RID: 34380 RVA: 0x00368094 File Offset: 0x00366294
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CleanupSpawning()
	{
		EntitySpawnerClass.Cleanup();
		BiomeSpawningClass.Cleanup();
	}

	// Token: 0x0600864D RID: 34381 RVA: 0x003680A0 File Offset: 0x003662A0
	public static void LoadPhysicsBodies()
	{
		WorldStaticData.Reset("physicsbodies");
	}

	// Token: 0x0600864E RID: 34382 RVA: 0x003680AC File Offset: 0x003662AC
	public static void SavePhysicsBodies()
	{
		PhysicsBodiesFromXml.Save(GameIO.GetGameDir("Data/Config") + "/physicsbodies.xml");
	}

	// Token: 0x0600864F RID: 34383 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public static void LinkBuffs()
	{
	}

	// Token: 0x06008650 RID: 34384 RVA: 0x003680C8 File Offset: 0x003662C8
	public static void Cleanup(string _xmlNameContaining)
	{
		foreach (WorldStaticData.XmlLoadInfo xmlLoadInfo in WorldStaticData.xmlsToLoad)
		{
			if (string.IsNullOrEmpty(_xmlNameContaining) || xmlLoadInfo.XmlName.ContainsCaseInsensitive(_xmlNameContaining))
			{
				Action cleanupMethod = xmlLoadInfo.CleanupMethod;
				if (cleanupMethod != null)
				{
					cleanupMethod();
				}
			}
		}
	}

	// Token: 0x06008651 RID: 34385 RVA: 0x00368114 File Offset: 0x00366314
	public static void Cleanup()
	{
		WorldStaticData.Cleanup(null);
		if (WorldStaticData.meshDescCol)
		{
			WorldStaticData.meshDescCol.Cleanup();
			WorldStaticData.meshDescCol = null;
		}
		MeshDescription.Cleanup();
		AssetBundles.Cleanup();
		WorldStaticData.bInitDone = false;
	}

	// Token: 0x06008652 RID: 34386 RVA: 0x00002914 File Offset: 0x00000B14
	public static void QuitCleanup()
	{
	}

	// Token: 0x06008653 RID: 34387 RVA: 0x00368148 File Offset: 0x00366348
	public static void Reset(string _xmlNameContaining)
	{
		WorldStaticData.Cleanup(_xmlNameContaining);
		MemoryStream memoryStream = new MemoryStream();
		DeflateOutputStream zipStream = new DeflateOutputStream(memoryStream, 3);
		foreach (WorldStaticData.XmlLoadInfo xmlLoadInfo in WorldStaticData.xmlsToLoad)
		{
			if (string.IsNullOrEmpty(_xmlNameContaining) || xmlLoadInfo.XmlName.ContainsCaseInsensitive(_xmlNameContaining))
			{
				if (!xmlLoadInfo.XmlFileExists())
				{
					if (!xmlLoadInfo.IgnoreMissingFile)
					{
						Log.Error("XML loader: XML is missing: " + xmlLoadInfo.XmlName);
					}
				}
				else
				{
					ThreadManager.RunCoroutineSync(WorldStaticData.loadSingleXml(xmlLoadInfo, memoryStream, zipStream));
				}
			}
		}
	}

	// Token: 0x06008654 RID: 34388 RVA: 0x003681D1 File Offset: 0x003663D1
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator cacheSingleXml(WorldStaticData.XmlLoadInfo _loadInfo, XmlFile _origXml, MemoryStream _memStream, DeflateOutputStream _zipStream)
	{
		MicroStopwatch timer = new MicroStopwatch(true);
		_memStream.SetLength(0L);
		_origXml.SerializeToStream(_zipStream, true, null);
		_zipStream.Restart();
		if (timer.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
		{
			yield return null;
			timer.ResetAndRestart();
		}
		_loadInfo.CompressedXmlData = _memStream.ToArray();
		_memStream.SetLength(0L);
		yield break;
	}

	// Token: 0x06008655 RID: 34389 RVA: 0x003681F5 File Offset: 0x003663F5
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator loadSingleXml(WorldStaticData.XmlLoadInfo _loadInfo, MemoryStream _memStream, DeflateOutputStream _zipStream)
	{
		MicroStopwatch timer = new MicroStopwatch(true);
		bool coroutineHadException = false;
		XmlFile xmlFile = null;
		yield return XmlPatcher.LoadAndPatchConfig(_loadInfo.XmlName, delegate(XmlFile _file)
		{
			xmlFile = _file;
		});
		if (xmlFile == null)
		{
			yield break;
		}
		yield return XmlPatcher.ApplyConditionalXmlBlocks(_loadInfo.XmlName, xmlFile, timer, XmlPatcher.EEvaluator.Host, delegate
		{
			coroutineHadException = true;
		});
		if (coroutineHadException)
		{
			yield break;
		}
		if (timer.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
		{
			yield return null;
			timer.ResetAndRestart();
		}
		yield return WorldStaticData.DumpPatchedXml(_loadInfo, xmlFile, timer);
		xmlFile.RemoveComments();
		yield return WorldStaticData.CachePatchedXml(_loadInfo, xmlFile, timer, _memStream, _zipStream, delegate
		{
			coroutineHadException = true;
		});
		if (coroutineHadException)
		{
			yield break;
		}
		if (timer.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
		{
			yield return null;
			timer.ResetAndRestart();
		}
		yield return XmlPatcher.ApplyConditionalXmlBlocks(_loadInfo.XmlName, xmlFile, timer, XmlPatcher.EEvaluator.Client, delegate
		{
			coroutineHadException = true;
		});
		if (coroutineHadException)
		{
			yield break;
		}
		yield return ThreadManager.CoroutineWrapperWithExceptionCallback(_loadInfo.LoadMethod(xmlFile), delegate(Exception _exception)
		{
			Log.Error("XML loader: Loading and parsing '" + xmlFile.Filename + "' failed");
			Log.Exception(_exception);
			coroutineHadException = true;
		});
		if (coroutineHadException)
		{
			yield break;
		}
		if (_loadInfo.ExecuteAfterLoad != null)
		{
			if (timer.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
			{
				yield return null;
				timer.ResetAndRestart();
			}
			yield return ThreadManager.CoroutineWrapperWithExceptionCallback(_loadInfo.ExecuteAfterLoad(), delegate(Exception _exception)
			{
				Log.Error("XML loader: Executing post load step on '" + xmlFile.Filename + "' failed");
				Log.Exception(_exception);
				coroutineHadException = true;
			});
			if (coroutineHadException)
			{
				yield break;
			}
		}
		xmlFile = null;
		Log.Out("Loaded (local): {0} in {1}", new object[]
		{
			_loadInfo.XmlName,
			((float)timer.ElapsedMicroseconds * 1E-06f).ToCultureInvariantString("f2")
		});
		yield break;
	}

	// Token: 0x06008656 RID: 34390 RVA: 0x00368212 File Offset: 0x00366412
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator DumpPatchedXml(WorldStaticData.XmlLoadInfo _loadInfo, XmlFile _xmlFile, MicroStopwatch _timer)
	{
		if (!ThreadManager.IsMainThread() || !Application.isPlaying)
		{
			yield break;
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			yield break;
		}
		if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
		{
			yield break;
		}
		if (_timer.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
		{
			yield return null;
			_timer.ResetAndRestart();
		}
		string path = GameIO.GetSaveGameDir() + "/ConfigsDump/" + _loadInfo.XmlName + ".xml";
		string directoryName = System.IO.Path.GetDirectoryName(path);
		if (!SdDirectory.Exists(directoryName))
		{
			SdDirectory.CreateDirectory(directoryName);
		}
		_xmlFile.SerializeToFile(path, false, null);
		yield break;
	}

	// Token: 0x06008657 RID: 34391 RVA: 0x0036822F File Offset: 0x0036642F
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator CachePatchedXml(WorldStaticData.XmlLoadInfo _loadInfo, XmlFile _xmlFile, MicroStopwatch _timer, MemoryStream _memStream, DeflateOutputStream _zipStream, Action _errorCallback)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			yield break;
		}
		if (_timer.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
		{
			yield return null;
			_timer.ResetAndRestart();
		}
		bool coroutineHadException = false;
		yield return ThreadManager.CoroutineWrapperWithExceptionCallback(WorldStaticData.cacheSingleXml(_loadInfo, _xmlFile, _memStream, _zipStream), delegate(Exception _exception)
		{
			Log.Error("XML loader: Compressing XML data for '" + _xmlFile.Filename + "' failed");
			Log.Exception(_exception);
			coroutineHadException = true;
		});
		if (coroutineHadException)
		{
			if (_errorCallback != null)
			{
				_errorCallback();
			}
		}
		yield break;
	}

	// Token: 0x06008658 RID: 34392 RVA: 0x00368263 File Offset: 0x00366463
	[Conditional("UNITY_GAMECORE")]
	[Conditional("UNITY_XBOXONE")]
	[Conditional("UNITY_PS4")]
	[Conditional("UNITY_PS5")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CollectGarbage()
	{
		GC.Collect();
	}

	// Token: 0x06008659 RID: 34393 RVA: 0x0036826A File Offset: 0x0036646A
	public static IEnumerator LoadAllXmlsCo(bool _isStartup, WorldStaticData.ProgressDelegate _progressDelegate)
	{
		WorldStaticData.LoadAllXmlsCoComplete = false;
		MemoryStream memStream = new MemoryStream();
		DeflateOutputStream zipStream = new DeflateOutputStream(memStream, 3);
		foreach (WorldStaticData.XmlLoadInfo xmlLoadInfo in WorldStaticData.xmlsToLoad)
		{
			if (!xmlLoadInfo.XmlFileExists())
			{
				if (!xmlLoadInfo.IgnoreMissingFile)
				{
					Log.Error("XML loader: XML is missing: " + xmlLoadInfo.XmlName);
				}
			}
			else if (!_isStartup || xmlLoadInfo.LoadAtStartup)
			{
				if (_progressDelegate != null && xmlLoadInfo.LoadStepLocalizationKey != null)
				{
					_progressDelegate(Localization.Get(xmlLoadInfo.LoadStepLocalizationKey, false), 0f);
				}
				yield return WorldStaticData.loadSingleXml(xmlLoadInfo, memStream, zipStream);
			}
		}
		WorldStaticData.XmlLoadInfo[] array = null;
		WorldStaticData.LoadAllXmlsCoComplete = true;
		yield break;
	}

	// Token: 0x0600865A RID: 34394 RVA: 0x00368280 File Offset: 0x00366480
	public static void ReloadAllXmlsSync()
	{
		WorldStaticData.Cleanup(null);
		ThreadManager.RunCoroutineSync(WorldStaticData.LoadAllXmlsCo(false, null));
	}

	// Token: 0x0600865B RID: 34395 RVA: 0x00368294 File Offset: 0x00366494
	public static void SendXmlsToClient(ClientInfo _cInfo)
	{
		foreach (WorldStaticData.XmlLoadInfo xmlLoadInfo in WorldStaticData.xmlsToLoad)
		{
			if (xmlLoadInfo.SendToClients && (xmlLoadInfo.LoadClientFile || xmlLoadInfo.CompressedXmlData != null))
			{
				_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConfigFile>().Setup(xmlLoadInfo.XmlName, xmlLoadInfo.LoadClientFile ? null : xmlLoadInfo.CompressedXmlData));
			}
		}
	}

	// Token: 0x0600865C RID: 34396 RVA: 0x003682F8 File Offset: 0x003664F8
	public static void SaveXmlsToFolder(string _exportPath)
	{
		foreach (WorldStaticData.XmlLoadInfo xmlLoadInfo in WorldStaticData.xmlsToLoad)
		{
			if (xmlLoadInfo.CompressedXmlData != null)
			{
				byte[] bytes;
				using (MemoryStream memoryStream = new MemoryStream(xmlLoadInfo.CompressedXmlData))
				{
					using (DeflateInputStream deflateInputStream = new DeflateInputStream(memoryStream))
					{
						using (MemoryStream memoryStream2 = new MemoryStream())
						{
							StreamUtils.StreamCopy(deflateInputStream, memoryStream2, null, true);
							bytes = memoryStream2.ToArray();
						}
					}
				}
				string path = _exportPath + "/" + xmlLoadInfo.XmlName + ".xml";
				if (xmlLoadInfo.XmlName.IndexOf('/') >= 0)
				{
					string directoryName = System.IO.Path.GetDirectoryName(path);
					if (!SdDirectory.Exists(directoryName))
					{
						SdDirectory.CreateDirectory(directoryName);
					}
				}
				SdFile.WriteAllBytes(path, bytes);
			}
		}
	}

	// Token: 0x0600865D RID: 34397 RVA: 0x003683F8 File Offset: 0x003665F8
	[PublicizedFrom(EAccessModifier.Private)]
	public static WorldStaticData.XmlLoadInfo getLoadInfoForName(string _xmlName, out int _arrayIndex)
	{
		_arrayIndex = -1;
		for (int i = 0; i < WorldStaticData.xmlsToLoad.Length; i++)
		{
			WorldStaticData.XmlLoadInfo xmlLoadInfo = WorldStaticData.xmlsToLoad[i];
			if (xmlLoadInfo.XmlName.EqualsCaseInsensitive(_xmlName))
			{
				_arrayIndex = i;
				return xmlLoadInfo;
			}
		}
		return null;
	}

	// Token: 0x0600865E RID: 34398 RVA: 0x00368436 File Offset: 0x00366636
	public static void WaitForConfigsFromServer()
	{
		if (WorldStaticData.receivedConfigsHandlerCoroutine != null)
		{
			ThreadManager.StopCoroutine(WorldStaticData.receivedConfigsHandlerCoroutine);
		}
		WorldStaticData.receivedConfigsHandlerCoroutine = ThreadManager.StartCoroutine(WorldStaticData.handleReceivedConfigs());
	}

	// Token: 0x0600865F RID: 34399 RVA: 0x00368458 File Offset: 0x00366658
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator handleReceivedConfigs()
	{
		MicroStopwatch timer = new MicroStopwatch(true);
		WorldStaticData.highestReceivedIndex = -1;
		WorldStaticData.XmlLoadInfo[] array = WorldStaticData.xmlsToLoad;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].WasReceivedFromServer = WorldStaticData.EClientFileState.None;
		}
		while (string.IsNullOrEmpty(GamePrefs.GetString(EnumGamePrefs.GameWorld)))
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
			{
				WorldStaticData.receivedConfigsHandlerCoroutine = null;
				yield break;
			}
			yield return null;
		}
		int waitingFor = 0;
		while (waitingFor < WorldStaticData.xmlsToLoad.Length)
		{
			WorldStaticData.<>c__DisplayClass61_0 CS$<>8__locals1 = new WorldStaticData.<>c__DisplayClass61_0();
			CS$<>8__locals1.loadInfo = WorldStaticData.xmlsToLoad[waitingFor];
			if (!CS$<>8__locals1.loadInfo.SendToClients)
			{
				int i = waitingFor;
				waitingFor = i + 1;
			}
			else if (CS$<>8__locals1.loadInfo.WasReceivedFromServer == WorldStaticData.EClientFileState.None)
			{
				if (CS$<>8__locals1.loadInfo.IgnoreMissingFile && WorldStaticData.highestReceivedIndex > waitingFor)
				{
					int i = waitingFor;
					waitingFor = i + 1;
				}
				else
				{
					yield return null;
				}
			}
			else
			{
				int i = waitingFor;
				waitingFor = i + 1;
				WorldStaticData.Cleanup(CS$<>8__locals1.loadInfo.XmlName);
				if (CS$<>8__locals1.loadInfo.WasReceivedFromServer == WorldStaticData.EClientFileState.LoadLocal)
				{
					yield return WorldStaticData.loadSingleXml(CS$<>8__locals1.loadInfo, null, null);
				}
				else
				{
					XmlFile xmlFile;
					using (MemoryStream memoryStream = new MemoryStream(CS$<>8__locals1.loadInfo.CompressedXmlData))
					{
						using (DeflateInputStream deflateInputStream = new DeflateInputStream(memoryStream))
						{
							xmlFile = new XmlFile(deflateInputStream);
						}
					}
					yield return null;
					CS$<>8__locals1.coroutineHadException = false;
					yield return XmlPatcher.ApplyConditionalXmlBlocks(CS$<>8__locals1.loadInfo.XmlName, xmlFile, timer, XmlPatcher.EEvaluator.Client, delegate
					{
						CS$<>8__locals1.coroutineHadException = true;
					});
					if (CS$<>8__locals1.coroutineHadException)
					{
						continue;
					}
					yield return ThreadManager.CoroutineWrapperWithExceptionCallback(CS$<>8__locals1.loadInfo.LoadMethod(xmlFile), delegate(Exception _exception)
					{
						Log.Error("XML loader: Loading and parsing '" + CS$<>8__locals1.loadInfo.XmlName + "' failed");
						Log.Exception(_exception);
						CS$<>8__locals1.coroutineHadException = true;
					});
					if (CS$<>8__locals1.coroutineHadException)
					{
						continue;
					}
					if (CS$<>8__locals1.loadInfo.ExecuteAfterLoad != null)
					{
						yield return null;
						yield return ThreadManager.CoroutineWrapperWithExceptionCallback(CS$<>8__locals1.loadInfo.ExecuteAfterLoad(), delegate(Exception _exception)
						{
							Log.Error("XML loader: Executing post load step on '" + CS$<>8__locals1.loadInfo.XmlName + "' failed");
							Log.Exception(_exception);
							CS$<>8__locals1.coroutineHadException = true;
						});
						if (CS$<>8__locals1.coroutineHadException)
						{
							continue;
						}
					}
					Log.Out("Loaded (received): " + CS$<>8__locals1.loadInfo.XmlName);
					yield return null;
					xmlFile = null;
				}
				CS$<>8__locals1 = null;
			}
		}
		WorldStaticData.receivedConfigsHandlerCoroutine = null;
		yield break;
	}

	// Token: 0x06008660 RID: 34400 RVA: 0x00368460 File Offset: 0x00366660
	public static bool AllConfigsReceivedAndLoaded()
	{
		return WorldStaticData.receivedConfigsHandlerCoroutine == null;
	}

	// Token: 0x06008661 RID: 34401 RVA: 0x0036846C File Offset: 0x0036666C
	public static void ReceivedConfigFile(string _name, byte[] _data)
	{
		if (_data != null)
		{
			Log.Out(string.Format("Received config file '{0}' from server. Len: {1}", _name, _data.Length));
		}
		else
		{
			Log.Out("Loading config '" + _name + "' from local files");
		}
		int b;
		WorldStaticData.XmlLoadInfo loadInfoForName = WorldStaticData.getLoadInfoForName(_name, out b);
		if (loadInfoForName == null)
		{
			Log.Warning("XML loader: Received unknown config from server: " + _name);
			return;
		}
		loadInfoForName.CompressedXmlData = _data;
		loadInfoForName.WasReceivedFromServer = ((_data != null) ? WorldStaticData.EClientFileState.Received : WorldStaticData.EClientFileState.LoadLocal);
		WorldStaticData.highestReceivedIndex = MathUtils.Max(WorldStaticData.highestReceivedIndex, b);
	}

	// Token: 0x06008662 RID: 34402 RVA: 0x003684EC File Offset: 0x003666EC
	public static void ReloadTextureArrays()
	{
		ThreadManager.RunCoroutineSync(WorldStaticData.meshDescCol.LoadTextureArrays(true));
	}

	// Token: 0x06008663 RID: 34403 RVA: 0x00368500 File Offset: 0x00366700
	public static void ReloadInGameXML()
	{
		MicroStopwatch microStopwatch = new MicroStopwatch(true);
		foreach (WorldStaticData.XmlLoadInfo xmlLoadInfo in WorldStaticData.xmlsToLoad)
		{
			if (xmlLoadInfo.AllowReloadDuringGame)
			{
				Log.Out("-- Reloading {0} --\n", new object[]
				{
					xmlLoadInfo.XmlName
				});
				XmlFile xmlFile = new XmlFile(xmlLoadInfo.XmlName);
				if (xmlLoadInfo.ReloadDuringGameMethod != null)
				{
					xmlLoadInfo.ReloadDuringGameMethod(xmlFile);
				}
				else
				{
					ThreadManager.RunCoroutineSync(xmlLoadInfo.LoadMethod(xmlFile));
				}
			}
		}
		World world = GameManager.Instance.World;
		if (world != null)
		{
			List<Entity> list = world.Entities.list;
			for (int j = 0; j < list.Count; j++)
			{
				list[j].OnXMLChanged();
			}
		}
		Log.Out("-- ReloadInGameXML {0} seconds --\n", new object[]
		{
			((float)microStopwatch.ElapsedMicroseconds * 1E-06f).ToCultureInvariantString()
		});
	}

	// Token: 0x04006843 RID: 26691
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cExplosionPrefabMax = 100;

	// Token: 0x04006844 RID: 26692
	public static bool LoadAllXmlsCoComplete;

	// Token: 0x04006845 RID: 26693
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly WorldStaticData.XmlLoadInfo[] xmlsToLoad = new WorldStaticData.XmlLoadInfo[]
	{
		new WorldStaticData.XmlLoadInfo("events", true, true, new Func<XmlFile, IEnumerator>(EventsFromXml.Load), new Action(EventsFromXml.Cleanup), null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("materials", false, true, new Func<XmlFile, IEnumerator>(WorldStaticData.LoadMaterials), new Action(MaterialBlock.Cleanup), new Func<IEnumerator>(WorldStaticData.LoadTextureAtlases), false, null, false, "loadActionMaterials"),
		new WorldStaticData.XmlLoadInfo("physicsbodies", false, true, new Func<XmlFile, IEnumerator>(PhysicsBodiesFromXml.Load), new Action(PhysicsBodyLayout.Reset), null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("painting", false, true, new Func<XmlFile, IEnumerator>(BlockTexturesFromXML.CreateBlockTextures), new Action(BlockTextureData.Cleanup), null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("shapes", false, true, new Func<XmlFile, IEnumerator>(ShapesFromXml.LoadShapes), null, null, true, null, false, null),
		new WorldStaticData.XmlLoadInfo("blocks", false, true, new Func<XmlFile, IEnumerator>(WorldStaticData.LoadBlocks), new Action(WorldStaticData.CleanupBlocks), null, true, null, false, "loadActionBlocks"),
		new WorldStaticData.XmlLoadInfo("progression", false, true, new Func<XmlFile, IEnumerator>(ProgressionFromXml.Load), new Action(Progression.Cleanup), null, true, null, false, null),
		new WorldStaticData.XmlLoadInfo("buffs", false, true, new Func<XmlFile, IEnumerator>(BuffsFromXml.CreateBuffs), new Action(BuffManager.Cleanup), null, true, new Action<XmlFile>(BuffsFromXml.Reload), false, null),
		new WorldStaticData.XmlLoadInfo("misc", false, true, new Func<XmlFile, IEnumerator>(WorldStaticData.LoadMisc), new Action(AnimationDelayData.Cleanup), null, true, new Action<XmlFile>(WorldStaticData.ReloadMisc), false, null),
		new WorldStaticData.XmlLoadInfo("items", false, true, new Func<XmlFile, IEnumerator>(WorldStaticData.LoadItems), new Action(ItemClass.Cleanup), null, true, new Action<XmlFile>(WorldStaticData.ReloadItems), false, "loadActionItems"),
		new WorldStaticData.XmlLoadInfo("item_modifiers", false, true, new Func<XmlFile, IEnumerator>(WorldStaticData.LoadItemModifiers), null, new Func<IEnumerator>(WorldStaticData.LateInitItems), true, new Action<XmlFile>(WorldStaticData.ReloadItemModifiers), false, null),
		new WorldStaticData.XmlLoadInfo("entityclasses", false, true, new Func<XmlFile, IEnumerator>(EntityClassesFromXml.LoadEntityClasses), new Action(EntityClass.Cleanup), null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("qualityinfo", false, true, new Func<XmlFile, IEnumerator>(QualityInfoFromXml.CreateQualityInfo), new Action(QualityInfo.Cleanup), null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("sounds", false, true, new Func<XmlFile, IEnumerator>(SoundsFromXml.CreateSounds), null, null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("recipes", false, true, new Func<XmlFile, IEnumerator>(WorldStaticData.LoadRecipes), new Action(CraftingManager.ClearAllRecipes), null, true, new Action<XmlFile>(WorldStaticData.ReloadRecipes), false, null),
		new WorldStaticData.XmlLoadInfo("blockplaceholders", false, true, new Func<XmlFile, IEnumerator>(BlockPlaceholdersFromXml.Load), new Action(BlockPlaceholderMap.Cleanup), null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("loot", false, true, new Func<XmlFile, IEnumerator>(WorldStaticData.LoadLoot), new Action(LootContainer.Cleanup), null, true, new Action<XmlFile>(WorldStaticData.ReloadLoot), false, null),
		new WorldStaticData.XmlLoadInfo("entitygroups", false, true, new Func<XmlFile, IEnumerator>(EntityGroupsFromXml.LoadEntityGroups), new Action(EntityGroups.Cleanup), null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("utilityai", false, true, new Func<XmlFile, IEnumerator>(UAIFromXml.Load), new Action(UAIFromXml.Cleanup), null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("vehicles", false, true, new Func<XmlFile, IEnumerator>(VehiclesFromXml.Load), new Action(Vehicle.Cleanup), null, true, new Action<XmlFile>(VehiclesFromXml.Reload), false, null),
		new WorldStaticData.XmlLoadInfo("rwgmixer", true, false, new Func<XmlFile, IEnumerator>(WorldGenerationFromXml.Load), new Action(WorldGenerationFromXml.Cleanup), null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("weathersurvival", false, true, new Func<XmlFile, IEnumerator>(WorldStaticData.LoadWeather), null, null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("archetypes", true, true, new Func<XmlFile, IEnumerator>(WorldStaticData.LoadSDCSArchetypes), null, null, false, null, true, null),
		new WorldStaticData.XmlLoadInfo("challenges", false, true, new Func<XmlFile, IEnumerator>(ChallengesFromXml.CreateChallenges), new Action(WorldStaticData.CleanupChallenges), null, true, null, false, null),
		new WorldStaticData.XmlLoadInfo("quests", false, true, new Func<XmlFile, IEnumerator>(QuestsFromXml.CreateQuests), null, null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("traders", false, true, new Func<XmlFile, IEnumerator>(WorldStaticData.LoadTraders), new Action(TraderInfo.Cleanup), null, true, null, false, null),
		new WorldStaticData.XmlLoadInfo("npc", false, true, new Func<XmlFile, IEnumerator>(WorldStaticData.LoadNpc), null, null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("dialogs", false, true, new Func<XmlFile, IEnumerator>(DialogFromXml.Load), new Action(Dialog.Cleanup), null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("ui_display", false, true, new Func<XmlFile, IEnumerator>(WorldStaticData.LoadUIDisplayInfo), new Action(UIDisplayInfoManager.Reset), null, true, null, false, null),
		new WorldStaticData.XmlLoadInfo("nav_objects", false, true, new Func<XmlFile, IEnumerator>(NavObjectClassesFromXml.Load), new Action(NavObjectClass.Reset), null, true, null, false, null),
		new WorldStaticData.XmlLoadInfo("gamestages", false, false, new Func<XmlFile, IEnumerator>(GameStagesFromXml.Load), new Action(WorldStaticData.CleanupGamestages), null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("gameevents", false, true, new Func<XmlFile, IEnumerator>(GameEventsFromXml.CreateGameEvents), new Action(WorldStaticData.CleanupGameEvents), null, true, null, false, null),
		new WorldStaticData.XmlLoadInfo("twitch", false, true, new Func<XmlFile, IEnumerator>(TwitchActionsFromXml.CreateTwitchActions), new Action(WorldStaticData.CleanupTwitch), null, true, null, false, null),
		new WorldStaticData.XmlLoadInfo("twitch_events", false, true, new Func<XmlFile, IEnumerator>(TwitchActionsFromXml.CreateTwitchEvents), new Action(WorldStaticData.CleanupTwitchEvents), null, true, null, false, null)
		{
			LoadClientFile = true
		},
		new WorldStaticData.XmlLoadInfo("dmscontent", false, true, new Func<XmlFile, IEnumerator>(DMSContentFromXml.Load), null, null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("XUi_Common/styles", false, true, new Func<XmlFile, IEnumerator>(XUiFromXml.Load), null, null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("XUi_Common/controls", false, true, new Func<XmlFile, IEnumerator>(XUiFromXml.Load), null, null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("XUi/styles", false, true, new Func<XmlFile, IEnumerator>(XUiFromXml.Load), null, null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("XUi/controls", false, true, new Func<XmlFile, IEnumerator>(XUiFromXml.Load), null, null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("XUi/windows", false, true, new Func<XmlFile, IEnumerator>(XUiFromXml.Load), null, null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("XUi/xui", false, true, new Func<XmlFile, IEnumerator>(XUiFromXml.Load), null, null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("biomes", false, true, new Func<XmlFile, IEnumerator>(WorldStaticData.LoadBiomes), new Action(WorldBiomes.CleanupStatic), null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("worldglobal", false, true, new Func<XmlFile, IEnumerator>(WorldGlobalFromXml.Load), null, null, true, new Action<XmlFile>(WorldGlobalFromXml.Reload), false, null),
		new WorldStaticData.XmlLoadInfo("spawning", false, false, new Func<XmlFile, IEnumerator>(WorldStaticData.LoadSpawning), new Action(WorldStaticData.CleanupSpawning), null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("loadingscreen", true, false, new Func<XmlFile, IEnumerator>(XUiC_LoadingScreen.LoadXml), null, null, true, null, false, null),
		new WorldStaticData.XmlLoadInfo("subtitles", true, false, new Func<XmlFile, IEnumerator>(SoundsFromXml.LoadSubtitleXML), null, null, false, null, false, null),
		new WorldStaticData.XmlLoadInfo("videos", true, false, new Func<XmlFile, IEnumerator>(VideoFromXML.CreateVideos), null, null, false, null, false, null)
	};

	// Token: 0x04006846 RID: 26694
	public static Transform[] prefabExplosions;

	// Token: 0x04006847 RID: 26695
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool bInitDone;

	// Token: 0x04006848 RID: 26696
	[PublicizedFrom(EAccessModifier.Private)]
	public static MeshDescriptionCollection meshDescCol;

	// Token: 0x04006849 RID: 26697
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool isDediServer;

	// Token: 0x0400684A RID: 26698
	[PublicizedFrom(EAccessModifier.Private)]
	public static Coroutine receivedConfigsHandlerCoroutine;

	// Token: 0x0400684B RID: 26699
	[PublicizedFrom(EAccessModifier.Private)]
	public static int highestReceivedIndex = -1;

	// Token: 0x020010A0 RID: 4256
	[PublicizedFrom(EAccessModifier.Private)]
	public enum EClientFileState
	{
		// Token: 0x0400684D RID: 26701
		None,
		// Token: 0x0400684E RID: 26702
		Received,
		// Token: 0x0400684F RID: 26703
		LoadLocal
	}

	// Token: 0x020010A1 RID: 4257
	[PublicizedFrom(EAccessModifier.Private)]
	public class XmlLoadInfo
	{
		// Token: 0x06008665 RID: 34405 RVA: 0x00368E04 File Offset: 0x00367004
		public bool XmlFileExists()
		{
			return SdFile.Exists(GameIO.GetGameDir("Data/Config") + "/" + this.XmlName + ".xml");
		}

		// Token: 0x06008666 RID: 34406 RVA: 0x00368E2C File Offset: 0x0036702C
		public XmlLoadInfo(string _xmlName, bool _loadAtStartup, bool _sendToClients, Func<XmlFile, IEnumerator> _loadMethod, Action _cleanupMethod, Func<IEnumerator> _executeAfterLoad = null, bool _allowReloadDuringGame = false, Action<XmlFile> _reloadDuringGameMethod = null, bool _ignoreMissingFile = false, string _loadStepLocalizationKey = null)
		{
			this.XmlName = _xmlName;
			this.LoadStepLocalizationKey = _loadStepLocalizationKey;
			this.LoadAtStartup = _loadAtStartup;
			this.SendToClients = _sendToClients;
			this.IgnoreMissingFile = _ignoreMissingFile;
			this.AllowReloadDuringGame = _allowReloadDuringGame;
			this.LoadMethod = _loadMethod;
			this.CleanupMethod = _cleanupMethod;
			this.ExecuteAfterLoad = _executeAfterLoad;
			this.ReloadDuringGameMethod = _reloadDuringGameMethod;
			if (this.LoadMethod == null)
			{
				throw new ArgumentNullException("_loadMethod");
			}
		}

		// Token: 0x04006850 RID: 26704
		public readonly string XmlName;

		// Token: 0x04006851 RID: 26705
		public readonly string LoadStepLocalizationKey;

		// Token: 0x04006852 RID: 26706
		public readonly bool LoadAtStartup;

		// Token: 0x04006853 RID: 26707
		public readonly bool SendToClients;

		// Token: 0x04006854 RID: 26708
		public readonly bool IgnoreMissingFile;

		// Token: 0x04006855 RID: 26709
		public readonly bool AllowReloadDuringGame;

		// Token: 0x04006856 RID: 26710
		public readonly Func<XmlFile, IEnumerator> LoadMethod;

		// Token: 0x04006857 RID: 26711
		public readonly Action CleanupMethod;

		// Token: 0x04006858 RID: 26712
		public readonly Func<IEnumerator> ExecuteAfterLoad;

		// Token: 0x04006859 RID: 26713
		public readonly Action<XmlFile> ReloadDuringGameMethod;

		// Token: 0x0400685A RID: 26714
		public byte[] CompressedXmlData;

		// Token: 0x0400685B RID: 26715
		public bool LoadClientFile;

		// Token: 0x0400685C RID: 26716
		public WorldStaticData.EClientFileState WasReceivedFromServer;
	}

	// Token: 0x020010A2 RID: 4258
	// (Invoke) Token: 0x06008668 RID: 34408
	public delegate void ProgressDelegate(string _progressText, float _percentage);
}
