using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Audio;
using DynamicMusic;
using DynamicMusic.Factories;
using GamePath;
using Platform;
using Twitch;
using UnityEngine;

// Token: 0x02000A75 RID: 2677
public class World : WorldBase, IBlockAccess, IChunkAccess, IChunkCallback
{
	// Token: 0x1400007F RID: 127
	// (add) Token: 0x06005195 RID: 20885 RVA: 0x0020C558 File Offset: 0x0020A758
	// (remove) Token: 0x06005196 RID: 20886 RVA: 0x0020C590 File Offset: 0x0020A790
	public event World.OnEntityLoadedDelegate EntityLoadedDelegates;

	// Token: 0x14000080 RID: 128
	// (add) Token: 0x06005197 RID: 20887 RVA: 0x0020C5C8 File Offset: 0x0020A7C8
	// (remove) Token: 0x06005198 RID: 20888 RVA: 0x0020C600 File Offset: 0x0020A800
	public event World.OnEntityUnloadedDelegate EntityUnloadedDelegates;

	// Token: 0x14000081 RID: 129
	// (add) Token: 0x06005199 RID: 20889 RVA: 0x0020C638 File Offset: 0x0020A838
	// (remove) Token: 0x0600519A RID: 20890 RVA: 0x0020C670 File Offset: 0x0020A870
	public event World.OnWorldChangedEvent OnWorldChanged;

	// Token: 0x0600519C RID: 20892 RVA: 0x0020C820 File Offset: 0x0020AA20
	public virtual void Init(IGameManager _gameManager, WorldBiomes _biomes)
	{
		this.gameManager = _gameManager;
		this.m_ChunkManager = new ChunkManager();
		this.m_ChunkManager.Init(this);
		this.m_SharedChunkObserverCache = new SharedChunkObserverCache(this.m_ChunkManager, 3, new NoThreadingSemantics());
		LightManager.Init();
		this.triggerManager = new TriggerManager();
		this.Biomes = _biomes;
		if (_biomes != null)
		{
			this.biomeSpawnManager = new SpawnManagerBiomes(this);
		}
		this.audioManager = Manager.Instance;
		this.BiomeAtmosphereEffects = new BiomeAtmosphereEffects();
		this.BiomeAtmosphereEffects.Init(this);
	}

	// Token: 0x0600519D RID: 20893 RVA: 0x0020C8AA File Offset: 0x0020AAAA
	public IEnumerator LoadWorld(string _sWorldName, bool _fixedSizeCC = false)
	{
		Log.Out("World.Load: " + _sWorldName);
		GamePrefs.Set(EnumGamePrefs.GameWorld, _sWorldName);
		World.IsSplatMapAvailable = GameManager.IsSplatMapAvailable();
		this.DuskDawnInit();
		this.wcd = new WorldCreationData(GameIO.GetWorldDir());
		this.worldState = new WorldState();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && _sWorldName != null)
		{
			string text;
			if (this.IsEditor())
			{
				text = PathAbstractions.WorldsSearchPaths.GetLocation(_sWorldName, null, null).FullPath + "/main.ttw";
			}
			else
			{
				text = GameIO.GetSaveGameDir() + "/main.ttw";
				if (!SdFile.Exists(text))
				{
					if (!SdDirectory.Exists(GameIO.GetSaveGameDir()))
					{
						SdDirectory.CreateDirectory(GameIO.GetSaveGameDir());
					}
					Log.Out("Loading base world file header...");
					this.worldState.Load(GameIO.GetWorldDir() + "/main.ttw", false, true, false);
					this.worldState.GenerateNewGuid();
					this.Seed = GamePrefs.GetString(EnumGamePrefs.GameName).GetHashCode();
					this.worldState.SetFrom(this, this.worldState.providerId);
					this.worldState.worldTime = 7000UL;
					this.worldState.saveDataLimit = SaveDataLimit.GetLimitFromPref();
					this.worldState.Save(text);
				}
			}
			if (!this.worldState.Load(text, true, false, !this.IsEditor()))
			{
				Log.Error("Could not load file '" + text + "'!");
			}
			else
			{
				this.Seed = this.worldState.seed;
			}
		}
		this.wcd.Apply(this, this.worldState);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			this.Seed = GamePrefs.GetString(EnumGamePrefs.GameNameClient).GetHashCode();
		}
		GameRandomManager.Instance.SetBaseSeed(this.Seed);
		this.rand = GameRandomManager.Instance.CreateGameRandom();
		this.rand.SetLock();
		this.worldTime = ((!this.IsEditor()) ? this.worldState.worldTime : 12000UL);
		GameTimer.Instance.ticks = this.worldState.timeInTicks;
		EntityFactory.nextEntityID = this.worldState.nextEntityID;
		if (PlatformOptimizations.LimitedSaveData && this.worldState.saveDataLimit < 0L)
		{
			GameUtils.WorldInfo worldInfo = GameUtils.WorldInfo.LoadWorldInfo(PathAbstractions.WorldsSearchPaths.GetLocation(_sWorldName, null, null));
			SaveDataLimit.SetLimitToPref(SaveDataLimitType.VeryLong.CalculateTotalSize(worldInfo.WorldSize));
		}
		else
		{
			SaveDataLimit.SetLimitToPref(this.worldState.saveDataLimit);
		}
		this.clientLastEntityId = -2;
		if (_sWorldName != null)
		{
			this.EntitiesTransform = GameObject.Find("/Entities").transform;
			EntityFactory.Init(this.EntitiesTransform);
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.dynamicSpawnManager = new SpawnManagerDynamic(this, null);
			if (this.worldState.dynamicSpawnerState != null && this.worldState.dynamicSpawnerState.Length > 0L)
			{
				using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
				{
					pooledBinaryReader.SetBaseStream(this.worldState.dynamicSpawnerState);
					this.dynamicSpawnManager.Read(pooledBinaryReader);
				}
			}
			this.entityDistributer = new NetEntityDistribution(this, 0);
			this.worldBlockTicker = new WorldBlockTicker(this);
			this.aiDirector = new AIDirector(this);
			if (this.worldState.aiDirectorState != null)
			{
				using (PooledBinaryReader pooledBinaryReader2 = MemoryPools.poolBinaryReader.AllocSync(false))
				{
					pooledBinaryReader2.SetBaseStream(this.worldState.aiDirectorState);
					this.aiDirector.Load(pooledBinaryReader2);
				}
			}
			if (this.worldState.sleeperVolumeState != null)
			{
				using (PooledBinaryReader pooledBinaryReader3 = MemoryPools.poolBinaryReader.AllocSync(false))
				{
					pooledBinaryReader3.SetBaseStream(this.worldState.sleeperVolumeState);
					this.ReadSleeperVolumes(pooledBinaryReader3);
					goto IL_431;
				}
			}
			this.sleeperVolumes.Clear();
			IL_431:
			if (this.worldState.triggerVolumeState != null)
			{
				using (PooledBinaryReader pooledBinaryReader4 = MemoryPools.poolBinaryReader.AllocSync(false))
				{
					pooledBinaryReader4.SetBaseStream(this.worldState.triggerVolumeState);
					this.ReadTriggerVolumes(pooledBinaryReader4);
					goto IL_47E;
				}
			}
			this.triggerVolumes.Clear();
			IL_47E:
			if (this.worldState.wallVolumeState != null)
			{
				using (PooledBinaryReader pooledBinaryReader5 = MemoryPools.poolBinaryReader.AllocSync(false))
				{
					pooledBinaryReader5.SetBaseStream(this.worldState.wallVolumeState);
					this.ReadWallVolumes(pooledBinaryReader5);
					goto IL_4CB;
				}
			}
			this.wallVolumes.Clear();
			IL_4CB:
			SleeperVolume.WorldInit();
		}
		DecoManager.Instance.IsEnabled = (_sWorldName != "Empty");
		yield return null;
		ChunkCluster cc = null;
		yield return this.CreateChunkCluster(SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? this.worldState.providerId : EnumChunkProviderId.NetworkClient, GamePrefs.GetString(EnumGamePrefs.GameWorld), 0, _fixedSizeCC, delegate(ChunkCluster _cluster)
		{
			cc = _cluster;
		});
		yield return null;
		XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadWorldEnvironment", false), true);
		string typeName = "WorldEnvironment";
		if (this.wcd.Properties.Values.ContainsKey("WorldEnvironment.Class"))
		{
			typeName = this.wcd.Properties.Values["WorldEnvironment.Class"];
		}
		GameObject gameObject = new GameObject("WorldEnvironment");
		this.m_WorldEnvironment = (gameObject.AddComponent(Type.GetType(typeName)) as WorldEnvironment);
		this.m_WorldEnvironment.Init(this.wcd, this);
		DynamicPrefabDecorator dynamicPrefabDecorator = cc.ChunkProvider.GetDynamicPrefabDecorator();
		SelectionBoxManager.Instance.GetCategory("DynamicPrefabs").SetCallback(dynamicPrefabDecorator);
		if (GameManager.Instance.IsEditMode() && !PrefabEditModeManager.Instance.IsActive())
		{
			SelectionBoxManager.Instance.GetCategory("DynamicPrefabs").SetVisible(true);
		}
		if (DecoManager.Instance.IsEnabled)
		{
			IChunkProvider chunkProvider = this.ChunkCache.ChunkProvider;
			yield return DecoManager.Instance.OnWorldLoaded(chunkProvider.GetWorldSize().x, chunkProvider.GetWorldSize().y, this, chunkProvider);
			this.m_WorldEnvironment.CreateUnityTerrain();
		}
		if (!this.IsEditor())
		{
			(this.dmsConductor = Factory.CreateConductor()).Init(true);
			if (!GameManager.IsDedicatedServer)
			{
				yield return this.dmsConductor.PreloadRoutine();
			}
		}
		if (!GameManager.IsDedicatedServer)
		{
			foreach (IEntitlementValidator entitlementValidator in PlatformManager.MultiPlatform.EntitlementValidators)
			{
				TwitchEntitlementManager twitchEntitlementManager = entitlementValidator as TwitchEntitlementManager;
				if (twitchEntitlementManager != null)
				{
					twitchEntitlementManager.Init();
				}
			}
		}
		this.SetupTraders();
		this.SetupSleeperVolumes();
		this.SetupTriggerVolumes();
		this.SetupWallVolumes();
		if (!GameManager.IsDedicatedServer && GameManager.IsSplatMapAvailable())
		{
			if (UnityDistantTerrainTest.Instance == null)
			{
				UnityDistantTerrainTest.Create();
			}
			if (!this.isUnityTerrainConfigured)
			{
				this.isUnityTerrainConfigured = true;
				ChunkProviderGenerateWorldFromRaw chunkProviderGenerateWorldFromRaw = this.ChunkCache.ChunkProvider as ChunkProviderGenerateWorldFromRaw;
				UnityDistantTerrainTest instance = UnityDistantTerrainTest.Instance;
				if (chunkProviderGenerateWorldFromRaw != null)
				{
					instance.HeightMap = chunkProviderGenerateWorldFromRaw.heightData;
					instance.hmWidth = chunkProviderGenerateWorldFromRaw.GetWorldSize().x;
					instance.hmHeight = chunkProviderGenerateWorldFromRaw.GetWorldSize().y;
					instance.TerrainMaterial = MeshDescription.meshes[5].materialDistant;
					instance.TerrainMaterial.renderQueue = 2490;
					instance.WaterMaterial = MeshDescription.meshes[1].materialDistant;
					instance.WaterMaterial.SetVector("_WorldDim", new Vector4((float)chunkProviderGenerateWorldFromRaw.GetWorldSize().x, (float)chunkProviderGenerateWorldFromRaw.GetWorldSize().y, 0f, 0f));
					chunkProviderGenerateWorldFromRaw.GetWaterChunks16x16(out instance.WaterChunks16x16Width, out instance.WaterChunks16x16);
					instance.LoadTerrain();
				}
			}
		}
		if (this.OnWorldChanged != null)
		{
			this.OnWorldChanged(_sWorldName);
		}
		yield break;
	}

	// Token: 0x0600519E RID: 20894 RVA: 0x0020C8C8 File Offset: 0x0020AAC8
	public void Save()
	{
		this.worldState.SetFrom(this, this.ChunkCache.ChunkProvider.GetProviderId());
		if (this.IsEditor())
		{
			this.worldState.ResetDynamicData();
			this.worldState.nextEntityID = 171;
			this.worldState.Save(PathAbstractions.WorldsSearchPaths.GetLocation(GamePrefs.GetString(EnumGamePrefs.GameWorld), null, null).FullPath + "/main.ttw");
		}
		else
		{
			this.worldState.Save(GameIO.GetSaveGameDir() + "/main.ttw");
		}
		for (int i = 0; i < this.ChunkClusters.Count; i++)
		{
			ChunkCluster chunkCluster = this.ChunkClusters[i];
			if (chunkCluster != null)
			{
				chunkCluster.Save();
			}
		}
		this.SaveDecorations();
		SaveDataUtils.SaveDataManager.CommitAsync();
	}

	// Token: 0x0600519F RID: 20895 RVA: 0x0020C99E File Offset: 0x0020AB9E
	public void SaveDecorations()
	{
		DecoManager.Instance.Save();
	}

	// Token: 0x060051A0 RID: 20896 RVA: 0x0020C9AA File Offset: 0x0020ABAA
	public void SaveWorldState()
	{
		this.worldState.SetFrom(this, this.ChunkCache.ChunkProvider.GetProviderId());
		this.worldState.Save(GameIO.GetSaveGameDir() + "/main.ttw");
	}

	// Token: 0x060051A1 RID: 20897 RVA: 0x0020C9E4 File Offset: 0x0020ABE4
	public virtual void UnloadWorld(bool _bUnloadRespawnableEntities)
	{
		Log.Out("World.Unload");
		if (this.m_WorldEnvironment != null)
		{
			this.m_WorldEnvironment.Cleanup();
			UnityEngine.Object.Destroy(this.m_WorldEnvironment.gameObject);
			this.m_WorldEnvironment = null;
		}
		this.ChunkCache = null;
		this.ChunkClusters.Cleanup();
		this.UnloadEntities(this.Entities.list, true);
		EntityFactory.Cleanup();
		if (BlockToolSelection.Instance != null)
		{
			BlockToolSelection.Instance.SelectionActive = false;
		}
		SelectionBoxManager.Instance.GetCategory("DynamicPrefabs").Clear();
		SelectionBoxManager.Instance.GetCategory("TraderTeleport").Clear();
		SelectionBoxManager.Instance.GetCategory("SleeperVolume").Clear();
		SelectionBoxManager.Instance.GetCategory("TriggerVolume").Clear();
		DecoManager.Instance.OnWorldUnloaded();
		Block.OnWorldUnloaded();
		if (UnityDistantTerrainTest.Instance != null)
		{
			UnityDistantTerrainTest.Instance.Cleanup();
			this.isUnityTerrainConfigured = false;
		}
	}

	// Token: 0x060051A2 RID: 20898 RVA: 0x0020CAE0 File Offset: 0x0020ACE0
	public virtual void Cleanup()
	{
		Log.Out("World.Cleanup");
		if (this.m_ChunkManager != null)
		{
			this.m_ChunkManager.Cleanup();
			this.m_ChunkManager = null;
		}
		if (this.audioManager != null)
		{
			this.audioManager.Dispose();
			Manager.CleanUp();
			this.audioManager = null;
		}
		if (this.dmsConductor != null)
		{
			this.dmsConductor.CleanUp();
			this.dmsConductor.OnWorldExit();
			this.dmsConductor = null;
		}
		LightManager.Dispose();
		for (int i = 0; i < this.Entities.list.Count; i++)
		{
			UnityEngine.Object.Destroy(this.Entities.list[i].RootTransform.gameObject);
		}
		this.Entities.Clear();
		this.EntityAlives.Clear();
		if (this.Biomes != null)
		{
			this.Biomes.Cleanup();
		}
		if (this.entityDistributer != null)
		{
			this.entityDistributer.Cleanup();
			this.entityDistributer = null;
		}
		if (this.biomeSpawnManager != null)
		{
			this.biomeSpawnManager.Cleanup();
			this.biomeSpawnManager = null;
		}
		this.dynamicSpawnManager = null;
		if (this.worldBlockTicker != null)
		{
			this.worldBlockTicker.Cleanup();
			this.worldBlockTicker = null;
		}
		BlockShapeNew.Cleanup();
		this.Biomes = null;
		if (this.objectsOnMap != null)
		{
			this.objectsOnMap.Clear();
		}
		this.m_LocalPlayerEntity = null;
		this.aiDirector = null;
		PathFinderThread.Instance = null;
		this.wcd = null;
		this.worldState = null;
		this.BiomeAtmosphereEffects = null;
		DynamicMeshUnity.ClearCachedDynamicMeshChunksList();
		if (this.FlatAreaManager != null)
		{
			this.FlatAreaManager.Cleanup();
		}
	}

	// Token: 0x060051A3 RID: 20899 RVA: 0x0020CC74 File Offset: 0x0020AE74
	public void ClearCaches()
	{
		this.m_ChunkManager.FreePools();
		PathPoint.CompactPool();
		for (int i = 0; i < this.ChunkClusters.Count; i++)
		{
			ChunkCluster chunkCluster = this.ChunkClusters[i];
			if (chunkCluster != null)
			{
				chunkCluster.ChunkProvider.ClearCaches();
			}
		}
	}

	// Token: 0x1700084A RID: 2122
	// (get) Token: 0x060051A4 RID: 20900 RVA: 0x0020CCC2 File Offset: 0x0020AEC2
	public string Guid
	{
		get
		{
			if (this.worldState != null)
			{
				return this.worldState.Guid;
			}
			return null;
		}
	}

	// Token: 0x060051A5 RID: 20901 RVA: 0x0020CCD9 File Offset: 0x0020AED9
	public long GetNextChunkToProvide()
	{
		return this.m_ChunkManager.GetNextChunkToProvide();
	}

	// Token: 0x060051A6 RID: 20902 RVA: 0x0020CCE6 File Offset: 0x0020AEE6
	public virtual IEnumerator CreateChunkCluster(EnumChunkProviderId _chunkProviderId, string _clusterName, int _forceClrIdx, bool _bFixedSize, Action<ChunkCluster> _resultHandler)
	{
		ChunkCluster cc = new ChunkCluster(this, _clusterName, this.ChunkClusters.LayerMappingTable[0]);
		if (_forceClrIdx != -1)
		{
			this.ChunkClusters.AddFixed(cc, _forceClrIdx);
			this.ChunkCache = this.ChunkClusters.Cluster0;
		}
		cc.IsFixedSize = _bFixedSize;
		cc.AddChunkCallback(this);
		WaterSimulationNative.Instance.Init(cc);
		yield return cc.Init(_chunkProviderId);
		_resultHandler(cc);
		yield break;
	}

	// Token: 0x060051A7 RID: 20903 RVA: 0x0020CD1A File Offset: 0x0020AF1A
	public override void AddLocalPlayer(EntityPlayerLocal _localPlayer)
	{
		if (!this.m_LocalPlayerEntities.Contains(_localPlayer))
		{
			this.m_LocalPlayerEntities.Add(_localPlayer);
		}
		if (this.objectsOnMap == null)
		{
			this.objectsOnMap = new MapObjectManager();
		}
	}

	// Token: 0x060051A8 RID: 20904 RVA: 0x0020CD49 File Offset: 0x0020AF49
	public override void RemoveLocalPlayer(EntityPlayerLocal _localPlayer)
	{
		this.m_LocalPlayerEntities.Remove(_localPlayer);
	}

	// Token: 0x060051A9 RID: 20905 RVA: 0x0020CD58 File Offset: 0x0020AF58
	public override List<EntityPlayerLocal> GetLocalPlayers()
	{
		return this.m_LocalPlayerEntities;
	}

	// Token: 0x060051AA RID: 20906 RVA: 0x0020CD60 File Offset: 0x0020AF60
	public override bool IsLocalPlayer(int _playerId)
	{
		Entity entity = this.GetEntity(_playerId);
		return entity != null && entity is EntityPlayerLocal;
	}

	// Token: 0x060051AB RID: 20907 RVA: 0x0020CD89 File Offset: 0x0020AF89
	public override EntityPlayerLocal GetLocalPlayerFromID(int _playerId)
	{
		return this.GetEntity(_playerId) as EntityPlayerLocal;
	}

	// Token: 0x060051AC RID: 20908 RVA: 0x0020CD98 File Offset: 0x0020AF98
	public override EntityPlayerLocal GetClosestLocalPlayer(Vector3 _position)
	{
		EntityPlayerLocal result = this.GetPrimaryPlayer();
		if (this.m_LocalPlayerEntities.Count > 1)
		{
			float num = float.MaxValue;
			for (int i = 0; i < this.m_LocalPlayerEntities.Count; i++)
			{
				float sqrMagnitude = (this.m_LocalPlayerEntities[i].GetPosition() - _position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = this.m_LocalPlayerEntities[i];
				}
			}
		}
		return result;
	}

	// Token: 0x060051AD RID: 20909 RVA: 0x0020CE0D File Offset: 0x0020B00D
	public override Vector3 GetVectorToClosestLocalPlayer(Vector3 _position)
	{
		return this.GetClosestLocalPlayer(_position).GetPosition() - _position;
	}

	// Token: 0x060051AE RID: 20910 RVA: 0x0020CE24 File Offset: 0x0020B024
	public override float GetSquaredDistanceToClosestLocalPlayer(Vector3 _position)
	{
		return this.GetVectorToClosestLocalPlayer(_position).sqrMagnitude;
	}

	// Token: 0x060051AF RID: 20911 RVA: 0x0020CE40 File Offset: 0x0020B040
	public override float GetDistanceToClosestLocalPlayer(Vector3 _position)
	{
		return this.GetVectorToClosestLocalPlayer(_position).magnitude;
	}

	// Token: 0x060051B0 RID: 20912 RVA: 0x0020CE5C File Offset: 0x0020B05C
	public void SetLocalPlayer(EntityPlayerLocal _thePlayer)
	{
		this.m_LocalPlayerEntity = _thePlayer;
		this.audioManager.AttachLocalPlayer(_thePlayer, this);
		LightManager.AttachLocalPlayer(_thePlayer, this);
		OcclusionManager.Instance.SetSourceDepthCamera(_thePlayer.playerCamera);
	}

	// Token: 0x060051B1 RID: 20913 RVA: 0x0020CE89 File Offset: 0x0020B089
	public override EntityPlayerLocal GetPrimaryPlayer()
	{
		return this.m_LocalPlayerEntity;
	}

	// Token: 0x060051B2 RID: 20914 RVA: 0x0020CE91 File Offset: 0x0020B091
	public int GetPrimaryPlayerId()
	{
		if (!(this.m_LocalPlayerEntity != null))
		{
			return -1;
		}
		return this.m_LocalPlayerEntity.entityId;
	}

	// Token: 0x060051B3 RID: 20915 RVA: 0x0020CEAE File Offset: 0x0020B0AE
	public override List<EntityPlayer> GetPlayers()
	{
		return this.Players.list;
	}

	// Token: 0x060051B4 RID: 20916 RVA: 0x0020CEBC File Offset: 0x0020B0BC
	public void GetSunAndBlockColors(Vector3i _worldBlockPos, out byte sunLight, out byte blockLight)
	{
		sunLight = 0;
		blockLight = 0;
		IChunk chunkFromWorldPos = this.GetChunkFromWorldPos(_worldBlockPos);
		if (chunkFromWorldPos != null)
		{
			int x = World.toBlockXZ(_worldBlockPos.x);
			int y = World.toBlockY(_worldBlockPos.y);
			int z = World.toBlockXZ(_worldBlockPos.z);
			sunLight = chunkFromWorldPos.GetLight(x, y, z, Chunk.LIGHT_TYPE.SUN);
			blockLight = chunkFromWorldPos.GetLight(x, y, z, Chunk.LIGHT_TYPE.BLOCK);
		}
	}

	// Token: 0x060051B5 RID: 20917 RVA: 0x0020CF18 File Offset: 0x0020B118
	public override float GetLightBrightness(Vector3i blockPos)
	{
		IChunk chunkFromWorldPos = this.GetChunkFromWorldPos(blockPos);
		if (chunkFromWorldPos != null)
		{
			int x = World.toBlockXZ(blockPos.x);
			int y = World.toBlockY(blockPos.y);
			int z = World.toBlockXZ(blockPos.z);
			return chunkFromWorldPos.GetLightBrightness(x, y, z, 0);
		}
		if (!this.IsDaytime())
		{
			return 0.1f;
		}
		return 0.65f;
	}

	// Token: 0x060051B6 RID: 20918 RVA: 0x0020CF74 File Offset: 0x0020B174
	public override int GetBlockLightValue(int _clrIdx, Vector3i blockPos)
	{
		ChunkCluster chunkCluster = this.ChunkClusters[_clrIdx];
		if (chunkCluster == null)
		{
			return (int)MarchingCubes.DensityAir;
		}
		IChunk chunkFromWorldPos = chunkCluster.GetChunkFromWorldPos(blockPos);
		if (chunkFromWorldPos != null)
		{
			int x = World.toBlockXZ(blockPos.x);
			int y = World.toBlockY(blockPos.y);
			int z = World.toBlockXZ(blockPos.z);
			return chunkFromWorldPos.GetLightValue(x, y, z, 0);
		}
		return 0;
	}

	// Token: 0x060051B7 RID: 20919 RVA: 0x0020CFD4 File Offset: 0x0020B1D4
	public override sbyte GetDensity(int _clrIdx, Vector3i _blockPos)
	{
		return this.GetDensity(_clrIdx, _blockPos.x, _blockPos.y, _blockPos.z);
	}

	// Token: 0x060051B8 RID: 20920 RVA: 0x0020CFF0 File Offset: 0x0020B1F0
	public override sbyte GetDensity(int _clrIdx, int _x, int _y, int _z)
	{
		ChunkCluster chunkCluster = this.ChunkClusters[_clrIdx];
		if (chunkCluster == null)
		{
			return MarchingCubes.DensityAir;
		}
		IChunk chunkFromWorldPos = chunkCluster.GetChunkFromWorldPos(_x, _y, _z);
		if (chunkFromWorldPos != null)
		{
			return chunkFromWorldPos.GetDensity(World.toBlockXZ(_x), World.toBlockY(_y), World.toBlockXZ(_z));
		}
		return MarchingCubes.DensityAir;
	}

	// Token: 0x060051B9 RID: 20921 RVA: 0x0020D040 File Offset: 0x0020B240
	public void SetDensity(int _clrIdx, Vector3i _pos, sbyte _density, bool _bFoceDensity = false)
	{
		ChunkCluster chunkCluster = this.ChunkClusters[_clrIdx];
		if (chunkCluster == null)
		{
			return;
		}
		chunkCluster.SetDensity(_pos, _density, _bFoceDensity);
	}

	// Token: 0x060051BA RID: 20922 RVA: 0x0020D068 File Offset: 0x0020B268
	public long GetTexture(int _x, int _y, int _z, int channel = 0)
	{
		Chunk chunk = (Chunk)this.ChunkCache.GetChunkFromWorldPos(_x, _y, _z);
		if (chunk != null)
		{
			return chunk.GetTextureFull(World.toBlockXZ(_x), World.toBlockY(_y), World.toBlockXZ(_z), channel);
		}
		return 0L;
	}

	// Token: 0x060051BB RID: 20923 RVA: 0x0020D0AC File Offset: 0x0020B2AC
	public TextureFullArray GetTextureFullArray(int _x, int _y, int _z)
	{
		Chunk chunk = (Chunk)this.ChunkCache.GetChunkFromWorldPos(_x, _y, _z);
		if (chunk != null)
		{
			return chunk.GetTextureFullArray(World.toBlockXZ(_x), World.toBlockY(_y), World.toBlockXZ(_z), true);
		}
		return new TextureFullArray(0L);
	}

	// Token: 0x060051BC RID: 20924 RVA: 0x0020D0F1 File Offset: 0x0020B2F1
	public void SetTexture(int _clrIdx, int _x, int _y, int _z, long _tex, int channel = 0)
	{
		this.ChunkClusters[_clrIdx].SetTextureFull(new Vector3i(_x, _y, _z), _tex, channel);
	}

	// Token: 0x060051BD RID: 20925 RVA: 0x0020D114 File Offset: 0x0020B314
	public override byte GetStability(int worldX, int worldY, int worldZ)
	{
		IChunk chunkSync = this.GetChunkSync(World.toChunkXZ(worldX), World.toChunkXZ(worldZ));
		if (chunkSync != null)
		{
			return chunkSync.GetStability(World.toBlockXZ(worldX), World.toBlockY(worldY), World.toBlockXZ(worldZ));
		}
		return 0;
	}

	// Token: 0x060051BE RID: 20926 RVA: 0x0020D151 File Offset: 0x0020B351
	public override byte GetStability(Vector3i _pos)
	{
		return this.GetStability(_pos.x, _pos.y, _pos.z);
	}

	// Token: 0x060051BF RID: 20927 RVA: 0x0020D16C File Offset: 0x0020B36C
	public override void SetStability(int worldX, int worldY, int worldZ, byte stab)
	{
		IChunk chunkSync = this.GetChunkSync(World.toChunkXZ(worldX), World.toChunkXZ(worldZ));
		if (chunkSync != null)
		{
			chunkSync.SetStability(World.toBlockXZ(worldX), World.toBlockY(worldY), World.toBlockXZ(worldZ), stab);
		}
	}

	// Token: 0x060051C0 RID: 20928 RVA: 0x0020D1A9 File Offset: 0x0020B3A9
	public override void SetStability(Vector3i _pos, byte stab)
	{
		this.SetStability(_pos.x, _pos.y, _pos.z, stab);
	}

	// Token: 0x060051C1 RID: 20929 RVA: 0x0020D1C4 File Offset: 0x0020B3C4
	public override byte GetHeight(int worldX, int worldZ)
	{
		IChunk chunkSync = this.GetChunkSync(World.toChunkXZ(worldX), World.toChunkXZ(worldZ));
		if (chunkSync != null)
		{
			return chunkSync.GetHeight(World.toBlockXZ(worldX), World.toBlockXZ(worldZ));
		}
		return 0;
	}

	// Token: 0x060051C2 RID: 20930 RVA: 0x0020D1FC File Offset: 0x0020B3FC
	public byte GetTerrainHeight(int worldX, int worldZ)
	{
		Chunk chunk = (Chunk)this.GetChunkSync(World.toChunkXZ(worldX), World.toChunkXZ(worldZ));
		if (chunk != null)
		{
			return chunk.GetTerrainHeight(World.toBlockXZ(worldX), World.toBlockXZ(worldZ));
		}
		return 0;
	}

	// Token: 0x060051C3 RID: 20931 RVA: 0x0020D238 File Offset: 0x0020B438
	public float GetHeightAt(float worldX, float worldZ)
	{
		IChunkProvider chunkProvider = this.ChunkCache.ChunkProvider;
		ITerrainGenerator terrainGenerator = (chunkProvider != null) ? chunkProvider.GetTerrainGenerator() : null;
		if (terrainGenerator != null)
		{
			return terrainGenerator.GetTerrainHeightAt((int)worldX, (int)worldZ);
		}
		return 0f;
	}

	// Token: 0x060051C4 RID: 20932 RVA: 0x0020D274 File Offset: 0x0020B474
	public bool GetWaterAt(float worldX, float worldZ)
	{
		ChunkProviderGenerateWorldFromRaw chunkProviderGenerateWorldFromRaw = this.ChunkCache.ChunkProvider as ChunkProviderGenerateWorldFromRaw;
		if (chunkProviderGenerateWorldFromRaw == null)
		{
			return false;
		}
		WorldDecoratorPOIFromImage poiFromImage = chunkProviderGenerateWorldFromRaw.poiFromImage;
		if (poiFromImage == null)
		{
			return false;
		}
		if (!poiFromImage.m_Poi.Contains((int)worldX, (int)worldZ))
		{
			return false;
		}
		byte data = poiFromImage.m_Poi.GetData((int)worldX, (int)worldZ);
		if (data == 0)
		{
			return false;
		}
		PoiMapElement poiForColor = this.Biomes.getPoiForColor((uint)data);
		return poiForColor != null && poiForColor.m_BlockValue.type == 240;
	}

	// Token: 0x060051C5 RID: 20933 RVA: 0x0020D2F0 File Offset: 0x0020B4F0
	public override bool IsWater(int _x, int _y, int _z)
	{
		if (_y < 256)
		{
			IChunk chunkFromWorldPos = this.GetChunkFromWorldPos(_x, _y, _z);
			if (chunkFromWorldPos != null)
			{
				_x &= 15;
				_y &= 255;
				_z &= 15;
				return chunkFromWorldPos.IsWater(_x, _y, _z);
			}
		}
		return false;
	}

	// Token: 0x060051C6 RID: 20934 RVA: 0x0020D332 File Offset: 0x0020B532
	public override bool IsWater(Vector3i _pos)
	{
		return this.IsWater(_pos.x, _pos.y, _pos.z);
	}

	// Token: 0x060051C7 RID: 20935 RVA: 0x0020D34C File Offset: 0x0020B54C
	public override bool IsWater(Vector3 _pos)
	{
		return this.IsWater(World.worldToBlockPos(_pos));
	}

	// Token: 0x060051C8 RID: 20936 RVA: 0x0020D35C File Offset: 0x0020B55C
	public override bool IsAir(int _x, int _y, int _z)
	{
		if (_y < 256)
		{
			IChunk chunkFromWorldPos = this.GetChunkFromWorldPos(_x, _y, _z);
			if (chunkFromWorldPos != null)
			{
				_x &= 15;
				_y &= 255;
				_z &= 15;
				return chunkFromWorldPos.IsAir(_x, _y, _z);
			}
		}
		return true;
	}

	// Token: 0x060051C9 RID: 20937 RVA: 0x0020D3A0 File Offset: 0x0020B5A0
	public bool CheckForLevelNearbyHeights(float worldX, float worldZ, int distance)
	{
		IChunkProvider chunkProvider = this.ChunkCache.ChunkProvider;
		ITerrainGenerator terrainGenerator = (chunkProvider != null) ? chunkProvider.GetTerrainGenerator() : null;
		if (terrainGenerator != null)
		{
			float terrainHeightAt = terrainGenerator.GetTerrainHeightAt((int)worldX, (int)worldZ);
			float num = terrainHeightAt;
			float num2 = terrainHeightAt;
			terrainHeightAt = terrainGenerator.GetTerrainHeightAt((int)worldX + distance, (int)worldZ);
			if (terrainHeightAt < num)
			{
				num = terrainHeightAt;
			}
			else if (terrainHeightAt > num2)
			{
				num2 = terrainHeightAt;
			}
			terrainHeightAt = terrainGenerator.GetTerrainHeightAt((int)worldX - distance, (int)worldZ);
			if (terrainHeightAt < num)
			{
				num = terrainHeightAt;
			}
			else if (terrainHeightAt > num2)
			{
				num2 = terrainHeightAt;
			}
			terrainHeightAt = terrainGenerator.GetTerrainHeightAt((int)worldX, (int)worldZ + distance);
			if (terrainHeightAt < num)
			{
				num = terrainHeightAt;
			}
			else if (terrainHeightAt > num2)
			{
				num2 = terrainHeightAt;
			}
			terrainHeightAt = terrainGenerator.GetTerrainHeightAt((int)worldX, (int)worldZ - distance);
			if (terrainHeightAt < num)
			{
				num = terrainHeightAt;
			}
			else if (terrainHeightAt > num2)
			{
				num2 = terrainHeightAt;
			}
			return Mathf.Abs(num2 - num) <= 2f;
		}
		return false;
	}

	// Token: 0x060051CA RID: 20938 RVA: 0x0020D480 File Offset: 0x0020B680
	public bool FindRandomSpawnPointNearRandomPlayer(int maxLightValue, out int x, out int y, out int z)
	{
		if (this.Players.list.Count == 0)
		{
			x = (y = (z = 0));
			return false;
		}
		Entity entityPlayer = null;
		int num = this.GetGameRandom().RandomRange(this.Players.list.Count);
		for (int i = 0; i < this.Players.list.Count; i++)
		{
			entityPlayer = this.Players.list[i];
			if (num-- == 0)
			{
				break;
			}
		}
		return this.FindRandomSpawnPointNearPlayer(entityPlayer, maxLightValue, out x, out y, out z, 32);
	}

	// Token: 0x060051CB RID: 20939 RVA: 0x0020D510 File Offset: 0x0020B710
	public bool FindRandomSpawnPointNearPlayer(Entity _entityPlayer, int maxLightValue, out int x, out int y, out int z, int maxDistance)
	{
		return this.FindRandomSpawnPointNearPosition(_entityPlayer.GetPosition(), maxLightValue, out x, out y, out z, new Vector3((float)maxDistance, (float)maxDistance, (float)maxDistance), true, false);
	}

	// Token: 0x060051CC RID: 20940 RVA: 0x0020D540 File Offset: 0x0020B740
	public bool FindRandomSpawnPointNearPositionUnderground(Vector3 _pos, int maxLightValue, out int x, out int y, out int z, Vector3 maxDistance)
	{
		x = (y = (z = 0));
		for (int i = 0; i < 5; i++)
		{
			x = Utils.Fastfloor(_pos.x + this.RandomRange(-maxDistance.x / 2f, maxDistance.x / 2f));
			z = Utils.Fastfloor(_pos.z + this.RandomRange(-maxDistance.z / 2f, maxDistance.z / 2f));
			Chunk chunk = (Chunk)this.GetChunkFromWorldPos(x, z);
			if (chunk != null && this.IsInPlayfield(chunk))
			{
				int x2 = World.toBlockXZ(x);
				int z2 = World.toBlockXZ(z);
				int num = Utils.Fastfloor(_pos.y - maxDistance.y / 2f);
				int num2 = Utils.Fastfloor(_pos.y + maxDistance.y / 2f);
				int num3 = (int)_pos.y;
				if (num3 >= num && num3 <= num2 && chunk.CanMobsSpawnAtPos(x2, num3, z2, false, true))
				{
					y = num3;
					return true;
				}
				if (chunk.FindSpawnPointAtXZ(x2, z2, out y, maxLightValue, 0, num, num2, false))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060051CD RID: 20941 RVA: 0x0020D67C File Offset: 0x0020B87C
	public bool FindRandomSpawnPointNearPosition(Vector3 _pos, int maxLightValue, out int x, out int y, out int z, Vector3 maxDistance, bool _bOnGround, bool _bIgnoreCanMobsSpawnOn = false)
	{
		x = (y = (z = 0));
		for (int i = 0; i < 5; i++)
		{
			x = Utils.Fastfloor(_pos.x + this.RandomRange(-maxDistance.x / 2f, maxDistance.x / 2f));
			z = Utils.Fastfloor(_pos.z + this.RandomRange(-maxDistance.z / 2f, maxDistance.z / 2f));
			Chunk chunk = (Chunk)this.GetChunkFromWorldPos(x, z);
			if (chunk != null && this.IsInPlayfield(chunk))
			{
				if (!_bOnGround)
				{
					y = Utils.Fastfloor(_pos.y + this.RandomRange(-maxDistance.y / 2f, maxDistance.y / 2f));
					return true;
				}
				int x2 = World.toBlockXZ(x);
				int z2 = World.toBlockXZ(z);
				int num = Utils.Fastfloor(_pos.y - maxDistance.y / 2f);
				int num2 = Utils.Fastfloor(_pos.y + maxDistance.y / 2f);
				int num3 = (int)(chunk.GetHeight(x2, z2) + 1);
				if (num3 >= num && num3 <= num2 && chunk.CanMobsSpawnAtPos(x2, num3, z2, _bIgnoreCanMobsSpawnOn, true))
				{
					y = num3;
					return true;
				}
				if (chunk.FindSpawnPointAtXZ(x2, z2, out y, maxLightValue, 0, num, num2, _bIgnoreCanMobsSpawnOn))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060051CE RID: 20942 RVA: 0x0020D7F8 File Offset: 0x0020B9F8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isPositionInRangeOfBedrolls(Vector3 _position)
	{
		int num = GamePrefs.GetInt(EnumGamePrefs.BedrollDeadZoneSize);
		num *= num;
		for (int i = 0; i < this.Players.list.Count; i++)
		{
			EntityBedrollPositionList spawnPoints = this.Players.list[i].SpawnPoints;
			int count = spawnPoints.Count;
			for (int j = 0; j < count; j++)
			{
				if ((spawnPoints[j].ToVector3() - _position).sqrMagnitude < (float)num)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060051CF RID: 20943 RVA: 0x0020D884 File Offset: 0x0020BA84
	public bool GetRandomSpawnPositionMinMaxToRandomPlayer(int _minRange, int _maxRange, bool _bConsiderBedrolls, out EntityPlayer _player, out Vector3 _position)
	{
		_position = Vector3.zero;
		_player = null;
		if (this.Players.list.Count == 0)
		{
			return false;
		}
		if (_maxRange - _minRange <= 0)
		{
			return false;
		}
		int num = this.rand.RandomRange(this.Players.list.Count);
		for (int i = 0; i < this.Players.list.Count; i++)
		{
			if (num-- == 0)
			{
				_player = this.Players.list[i];
				break;
			}
		}
		int num2 = _minRange * _minRange;
		for (int j = 0; j < 10; j++)
		{
			Vector2 vector = Vector2.zero;
			do
			{
				vector = this.rand.RandomInsideUnitCircle * (float)(_maxRange - _minRange);
			}
			while ((double)vector.sqrMagnitude < 0.01);
			vector += vector * ((float)_minRange / vector.magnitude);
			_position = _player.GetPosition() + new Vector3(vector.x, 0f, vector.y);
			Vector3i vector3i = World.worldToBlockPos(_position);
			Chunk chunk = (Chunk)this.GetChunkFromWorldPos(vector3i);
			if (chunk != null)
			{
				int x = World.toBlockXZ(vector3i.x);
				int z = World.toBlockXZ(vector3i.z);
				vector3i.y = (int)(chunk.GetHeight(x, z) + 1);
				_position.y = (float)vector3i.y;
				if ((!_bConsiderBedrolls || !this.isPositionInRangeOfBedrolls(vector3i.ToVector3())) && chunk.CanMobsSpawnAtPos(x, Utils.Fastfloor(_position.y), z, false, true))
				{
					bool flag = true;
					for (int k = 0; k < this.Players.list.Count; k++)
					{
						EntityPlayer entityPlayer = this.Players.list[k];
						if (entityPlayer.GetDistanceSq(_position) < (float)num2)
						{
							flag = false;
							break;
						}
						if (entityPlayer.CanSee(_position))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						_position = vector3i.ToVector3() + new Vector3(0.5f, this.GetTerrainOffset(0, vector3i), 0.5f);
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x060051D0 RID: 20944 RVA: 0x0020DAC8 File Offset: 0x0020BCC8
	public bool GetMobRandomSpawnPosWithWater(Vector3 _targetPos, int _minRange, int _maxRange, int _minPlayerRange, bool _checkBedrolls, out Vector3 _position)
	{
		return this.GetRandomSpawnPositionMinMaxToPosition(_targetPos, _minRange, _maxRange, _minPlayerRange, _checkBedrolls, out _position, -1, true, 20, false, EnumLandClaimOwner.None, false) || this.GetRandomSpawnPositionMinMaxToPosition(_targetPos, _minRange, _maxRange, _minPlayerRange, _checkBedrolls, out _position, -1, false, 20, false, EnumLandClaimOwner.None, false);
	}

	// Token: 0x060051D1 RID: 20945 RVA: 0x0020DB0C File Offset: 0x0020BD0C
	public bool GetRandomSpawnPositionMinMaxToPosition(Vector3 _targetPos, int _minRange, int _maxRange, int _minPlayerRange, bool _checkBedrolls, out Vector3 _position, int _forPlayerEntityId = -1, bool _checkWater = true, int _retryCount = 50, bool _checkLandClaim = false, EnumLandClaimOwner _maxLandClaimType = EnumLandClaimOwner.None, bool _useSquareRadius = false)
	{
		_position = Vector3.zero;
		int num = _maxRange - _minRange;
		if (num <= 0)
		{
			return false;
		}
		PersistentPlayerData lpRelative = _checkLandClaim ? GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(_forPlayerEntityId) : null;
		for (int i = 0; i < _retryCount; i++)
		{
			if (_useSquareRadius)
			{
				int num2;
				int num3;
				do
				{
					num2 = this.rand.RandomRange(-_maxRange, _maxRange + 1);
					num3 = this.rand.RandomRange(-_maxRange, _maxRange + 1);
				}
				while (Mathf.Abs(num2) < _minRange && Mathf.Abs(num3) < _minRange);
				_position.x = _targetPos.x + (float)num2;
				_position.y = _targetPos.y;
				_position.z = _targetPos.z + (float)num3;
			}
			else
			{
				Vector2 vector;
				do
				{
					vector = this.rand.RandomInsideUnitCircle * (float)num;
				}
				while (vector.sqrMagnitude < 0.01f);
				vector += vector * ((float)_minRange / vector.magnitude);
				_position.x = _targetPos.x + vector.x;
				_position.y = _targetPos.y;
				_position.z = _targetPos.z + vector.y;
			}
			Vector3i vector3i = World.worldToBlockPos(_position);
			Chunk chunk = (Chunk)this.GetChunkFromWorldPos(vector3i);
			if (chunk != null)
			{
				int x = World.toBlockXZ(vector3i.x);
				int z = World.toBlockXZ(vector3i.z);
				vector3i.y = (int)(chunk.GetHeight(x, z) + 1);
				_position.y = (float)vector3i.y;
				if (!_checkBedrolls || !this.isPositionInRangeOfBedrolls(vector3i.ToVector3()))
				{
					if (_forPlayerEntityId == -1)
					{
						if (!chunk.CanMobsSpawnAtPos(x, Utils.Fastfloor(_position.y), z, false, _checkWater))
						{
							goto IL_256;
						}
					}
					else if (!chunk.CanPlayersSpawnAtPos(x, Utils.Fastfloor(_position.y), z, false) || !chunk.IsPositionOnTerrain(x, vector3i.y, z) || this.GetPOIAtPosition(_position, true) != null || (_checkWater && chunk.IsWater(x, vector3i.y - 1, z)) || (_checkLandClaim && this.GetLandClaimOwner(vector3i, lpRelative) > _maxLandClaimType))
					{
						goto IL_256;
					}
					if (this.isPositionFarFromPlayers(_position, _minPlayerRange))
					{
						_position = vector3i.ToVector3() + new Vector3(0.5f, this.GetTerrainOffset(0, vector3i), 0.5f);
						return true;
					}
				}
			}
			IL_256:;
		}
		_position = Vector3.zero;
		return false;
	}

	// Token: 0x060051D2 RID: 20946 RVA: 0x0020DD88 File Offset: 0x0020BF88
	public bool GetRandomSpawnPositionInAreaMinMaxToPlayers(Rect _area, int _minDistance, int UNUSED_maxDistance, bool _checkBedrolls, out Vector3 _position)
	{
		_position = Vector3.zero;
		if (this.Players.list.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < 10; i++)
		{
			_position.x = _area.x + this.RandomRange(0f, _area.width - 1f);
			_position.y = 0f;
			_position.z = _area.y + this.RandomRange(0f, _area.height - 1f);
			Vector3i vector3i = World.worldToBlockPos(_position);
			Chunk chunk = (Chunk)this.GetChunkFromWorldPos(vector3i);
			if (chunk != null)
			{
				int x = World.toBlockXZ(vector3i.x);
				int z = World.toBlockXZ(vector3i.z);
				vector3i.y = (int)(chunk.GetHeight(x, z) + 1);
				_position.y = (float)vector3i.y;
				if ((!_checkBedrolls || !this.isPositionInRangeOfBedrolls(vector3i.ToVector3())) && chunk.CanMobsSpawnAtPos(x, Utils.Fastfloor(_position.y), z, false, true))
				{
					bool flag = this.isPositionFarFromPlayers(_position, _minDistance);
					if (flag)
					{
						for (int j = 0; j < this.Players.list.Count; j++)
						{
							EntityPlayer entityPlayer = this.Players.list[j];
							if ((_position - entityPlayer.position).sqrMagnitude < 2500f && entityPlayer.IsInViewCone(_position))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							_position = vector3i.ToVector3() + new Vector3(0.5f, this.GetTerrainOffset(0, vector3i), 0.5f);
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x060051D3 RID: 20947 RVA: 0x0020DF60 File Offset: 0x0020C160
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isPositionFarFromPlayers(Vector3 _position, int _minDistance)
	{
		int num = _minDistance * _minDistance;
		for (int i = 0; i < this.Players.list.Count; i++)
		{
			if (this.Players.list[i].GetDistanceSq(_position) < (float)num)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060051D4 RID: 20948 RVA: 0x0020DFAC File Offset: 0x0020C1AC
	public Vector3 FindSupportingBlockPos(Vector3 pos)
	{
		Vector3i vector3i = World.worldToBlockPos(pos);
		BlockValue block = this.GetBlock(vector3i);
		Block block2 = block.Block;
		if (block2.IsMovementBlocked(this, vector3i, block, BlockFace.Top))
		{
			return pos;
		}
		if (block2.IsElevator())
		{
			return pos;
		}
		vector3i.y++;
		block = this.GetBlock(vector3i);
		block2 = block.Block;
		if (block2.IsElevator((int)block.rotation))
		{
			return pos;
		}
		vector3i.y -= 2;
		block = this.GetBlock(vector3i);
		block2 = block.Block;
		if (!block2.IsElevator() && !block2.IsMovementBlocked(this, vector3i, block, BlockFace.Top))
		{
			Vector3 b = new Vector3((float)vector3i.x + 0.5f, pos.y, (float)vector3i.z + 0.5f);
			Vector3 vector = pos - b;
			int num = Mathf.RoundToInt((Mathf.Atan2(vector.x, vector.z) * 57.29578f + 22.5f) / 45f) & 7;
			int[] array = World.supportOrder[num];
			Vector3i vector3i2;
			vector3i2.y = vector3i.y;
			for (int i = 0; i < 8; i++)
			{
				int num2 = array[i] * 2;
				vector3i2.x = vector3i.x + World.supportOffsets[num2];
				vector3i2.z = vector3i.z + World.supportOffsets[num2 + 1];
				block = this.GetBlock(vector3i2);
				block2 = block.Block;
				if (block2.IsMovementBlocked(this, vector3i2, block, BlockFace.Top))
				{
					pos.x = (float)vector3i2.x + 0.5f;
					pos.z = (float)vector3i2.z + 0.5f;
					break;
				}
			}
		}
		return pos;
	}

	// Token: 0x060051D5 RID: 20949 RVA: 0x0020E158 File Offset: 0x0020C358
	public float GetTerrainOffset(int _clrIdx, Vector3i _blockPos)
	{
		float result = 0f;
		if (this.GetBlock(_clrIdx, _blockPos - Vector3i.up).Block.shape.IsTerrain())
		{
			sbyte density = this.GetDensity(_clrIdx, _blockPos);
			sbyte density2 = this.GetDensity(_clrIdx, _blockPos - Vector3i.up);
			result = MarchingCubes.GetDecorationOffsetY(density, density2);
		}
		return result;
	}

	// Token: 0x060051D6 RID: 20950 RVA: 0x0020E1B4 File Offset: 0x0020C3B4
	public bool IsInPlayfield(Chunk _c)
	{
		ChunkCluster chunkCache = this.ChunkCache;
		return !chunkCache.IsFixedSize || this.IsEditor() || (_c.X > chunkCache.ChunkMinPos.x && _c.Z > chunkCache.ChunkMinPos.y && _c.X < chunkCache.ChunkMaxPos.x && _c.Z < chunkCache.ChunkMaxPos.y);
	}

	// Token: 0x060051D7 RID: 20951 RVA: 0x0020E22C File Offset: 0x0020C42C
	public override BlockValue GetBlock(Vector3i _pos)
	{
		ChunkCluster chunkCache = this.ChunkCache;
		if (chunkCache == null)
		{
			return BlockValue.Air;
		}
		return chunkCache.GetBlock(_pos);
	}

	// Token: 0x060051D8 RID: 20952 RVA: 0x0020E250 File Offset: 0x0020C450
	public override BlockValue GetBlock(int _clrIdx, Vector3i _pos)
	{
		ChunkCluster chunkCluster = this.ChunkClusters[_clrIdx];
		if (chunkCluster == null)
		{
			return BlockValue.Air;
		}
		return chunkCluster.GetBlock(_pos);
	}

	// Token: 0x060051D9 RID: 20953 RVA: 0x0020E27A File Offset: 0x0020C47A
	public override BlockValue GetBlock(int _x, int _y, int _z)
	{
		return this.GetBlock(new Vector3i(_x, _y, _z));
	}

	// Token: 0x060051DA RID: 20954 RVA: 0x0020E28C File Offset: 0x0020C48C
	public override BlockValue GetBlock(int _clrIdx, int _x, int _y, int _z)
	{
		ChunkCluster chunkCluster = this.ChunkClusters[_clrIdx];
		if (chunkCluster == null)
		{
			return BlockValue.Air;
		}
		return chunkCluster.GetBlock(new Vector3i(_x, _y, _z));
	}

	// Token: 0x060051DB RID: 20955 RVA: 0x0020E2BE File Offset: 0x0020C4BE
	public WaterValue GetWater(int _x, int _y, int _z)
	{
		return this.GetWater(new Vector3i(_x, _y, _z));
	}

	// Token: 0x060051DC RID: 20956 RVA: 0x0020E2D0 File Offset: 0x0020C4D0
	public WaterValue GetWater(Vector3i _pos)
	{
		ChunkCluster chunkCache = this.ChunkCache;
		if (chunkCache == null)
		{
			return WaterValue.Empty;
		}
		return chunkCache.GetWater(_pos);
	}

	// Token: 0x060051DD RID: 20957 RVA: 0x0020E2F4 File Offset: 0x0020C4F4
	public float GetWaterPercent(Vector3i _pos)
	{
		ChunkCluster chunkCache = this.ChunkCache;
		if (chunkCache == null)
		{
			return 0f;
		}
		return chunkCache.GetWater(_pos).GetMassPercent();
	}

	// Token: 0x060051DE RID: 20958 RVA: 0x0020E320 File Offset: 0x0020C520
	public void HandleWaterLevelChanged(Vector3i _pos, float _waterPercent)
	{
		GameLightManager instance = GameLightManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.HandleWaterLevelChanged();
	}

	// Token: 0x060051DF RID: 20959 RVA: 0x0020E331 File Offset: 0x0020C531
	public BiomeDefinition GetBiome(string _name)
	{
		return this.Biomes.GetBiome(_name);
	}

	// Token: 0x060051E0 RID: 20960 RVA: 0x0020E340 File Offset: 0x0020C540
	public BiomeDefinition GetBiome(int _x, int _z)
	{
		Chunk chunk = (Chunk)this.GetChunkFromWorldPos(_x, _z);
		if (chunk != null)
		{
			byte biomeId = chunk.GetBiomeId(World.toBlockXZ(_x), World.toBlockXZ(_z));
			return this.Biomes.GetBiome(biomeId);
		}
		return null;
	}

	// Token: 0x060051E1 RID: 20961 RVA: 0x0020E380 File Offset: 0x0020C580
	public BiomeDefinition GetBiomeInWorld(int _x, int _z)
	{
		ChunkCluster chunkCache = this.ChunkCache;
		IChunkProvider chunkProvider = (chunkCache != null) ? chunkCache.ChunkProvider : null;
		if (chunkProvider != null)
		{
			IBiomeProvider biomeProvider = chunkProvider.GetBiomeProvider();
			if (biomeProvider != null)
			{
				return biomeProvider.GetBiomeAt(_x, _z);
			}
		}
		return null;
	}

	// Token: 0x060051E2 RID: 20962 RVA: 0x0020E3B7 File Offset: 0x0020C5B7
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IChunk GetChunkFromWorldPos(int x, int z)
	{
		return this.GetChunkSync(World.toChunkXZ(x), World.toChunkXZ(z));
	}

	// Token: 0x060051E3 RID: 20963 RVA: 0x0020E3CB File Offset: 0x0020C5CB
	public override IChunk GetChunkFromWorldPos(int x, int y, int z)
	{
		return this.GetChunkSync(World.toChunkXZ(x), World.toChunkXZ(z));
	}

	// Token: 0x060051E4 RID: 20964 RVA: 0x0020E3DF File Offset: 0x0020C5DF
	public override IChunk GetChunkFromWorldPos(Vector3i _blockPos)
	{
		return this.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkXZ(_blockPos.z));
	}

	// Token: 0x060051E5 RID: 20965 RVA: 0x0020E3FD File Offset: 0x0020C5FD
	public override void GetChunkFromWorldPos(int _blockX, int _blockZ, ref IChunk _chunk)
	{
		_blockX >>= 4;
		_blockZ >>= 4;
		if (_chunk == null || _chunk.X != _blockX || _chunk.Z != _blockZ)
		{
			_chunk = this.GetChunkSync(_blockX, _blockZ);
		}
	}

	// Token: 0x060051E6 RID: 20966 RVA: 0x0020E42C File Offset: 0x0020C62C
	public override bool GetChunkFromWorldPos(Vector3i _blockPos, ref IChunk _chunk)
	{
		Vector3i vector3i = World.toChunkXYZ(_blockPos);
		if (_chunk == null || _chunk.ChunkPos != vector3i)
		{
			_chunk = this.GetChunkSync(vector3i);
		}
		return _chunk != null;
	}

	// Token: 0x060051E7 RID: 20967 RVA: 0x0020E461 File Offset: 0x0020C661
	public override IChunk GetChunkSync(Vector3i chunkPos)
	{
		return this.GetChunkSync(chunkPos.x, chunkPos.z);
	}

	// Token: 0x060051E8 RID: 20968 RVA: 0x0020E478 File Offset: 0x0020C678
	public IChunk GetChunkSync(int chunkX, int chunkZ)
	{
		ChunkCluster chunkCache = this.ChunkCache;
		if (chunkCache == null)
		{
			return null;
		}
		return chunkCache.GetChunkSync(chunkX, chunkZ);
	}

	// Token: 0x060051E9 RID: 20969 RVA: 0x0020E49C File Offset: 0x0020C69C
	public IChunk GetChunkSync(long _key)
	{
		ChunkCluster chunkCache = this.ChunkCache;
		if (chunkCache == null)
		{
			return null;
		}
		return chunkCache.GetChunkSync(_key);
	}

	// Token: 0x060051EA RID: 20970 RVA: 0x0020E4BC File Offset: 0x0020C6BC
	public bool IsChunkAreaLoaded(Vector3 _position)
	{
		return this.IsChunkAreaLoaded(Utils.Fastfloor(_position.x), 0, Utils.Fastfloor(_position.z));
	}

	// Token: 0x060051EB RID: 20971 RVA: 0x0020E4DC File Offset: 0x0020C6DC
	public bool IsChunkAreaLoaded(int _blockPosX, int _, int _blockPosZ)
	{
		int num = World.toChunkXZ(_blockPosX - 8);
		int num2 = World.toChunkXZ(_blockPosZ - 8);
		int num3 = World.toChunkXZ(_blockPosX + 8);
		int num4 = World.toChunkXZ(_blockPosZ + 8);
		for (int i = num; i <= num3; i++)
		{
			for (int j = num2; j <= num4; j++)
			{
				if (this.GetChunkSync(i, j) == null)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x060051EC RID: 20972 RVA: 0x0020E535 File Offset: 0x0020C735
	public bool IsChunkAreaCollidersLoaded(Vector3 _position)
	{
		return this.IsChunkAreaCollidersLoaded(Utils.Fastfloor(_position.x), Utils.Fastfloor(_position.z));
	}

	// Token: 0x060051ED RID: 20973 RVA: 0x0020E554 File Offset: 0x0020C754
	public bool IsChunkAreaCollidersLoaded(int _blockPosX, int _blockPosZ)
	{
		int num = World.toChunkXZ(_blockPosX - 8);
		int num2 = World.toChunkXZ(_blockPosZ - 8);
		int num3 = World.toChunkXZ(_blockPosX + 8);
		int num4 = World.toChunkXZ(_blockPosZ + 8);
		for (int i = num; i <= num3; i++)
		{
			for (int j = num2; j <= num4; j++)
			{
				Chunk chunk = (Chunk)this.GetChunkSync(i, j);
				if (chunk == null || !chunk.IsCollisionMeshGenerated)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x060051EE RID: 20974 RVA: 0x001DCAFB File Offset: 0x001DACFB
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int toChunkXZ(int _v)
	{
		return _v >> 4;
	}

	// Token: 0x060051EF RID: 20975 RVA: 0x0020E5C1 File Offset: 0x0020C7C1
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i toChunkXZ(Vector2i _v)
	{
		return new Vector2i(_v.x >> 4, _v.y >> 4);
	}

	// Token: 0x060051F0 RID: 20976 RVA: 0x0020E5D8 File Offset: 0x0020C7D8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i toChunkXZ(Vector3 _v)
	{
		return new Vector2i(Utils.Fastfloor(_v.x) >> 4, Utils.Fastfloor(_v.z) >> 4);
	}

	// Token: 0x060051F1 RID: 20977 RVA: 0x0020E5F9 File Offset: 0x0020C7F9
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i toChunkXZ(Vector3i _v)
	{
		return new Vector2i(_v.x >> 4, _v.z >> 4);
	}

	// Token: 0x060051F2 RID: 20978 RVA: 0x0020E610 File Offset: 0x0020C810
	public static Vector3i toChunkXYZCube(Vector3 _v)
	{
		return new Vector3i(Utils.Fastfloor(_v.x) >> 4, Utils.Fastfloor(_v.y) >> 4, Utils.Fastfloor(_v.z) >> 4);
	}

	// Token: 0x060051F3 RID: 20979 RVA: 0x0020E63E File Offset: 0x0020C83E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i toChunkXYZ(Vector3i _v)
	{
		return new Vector3i(_v.x >> 4, _v.y >> 8, _v.z >> 4);
	}

	// Token: 0x060051F4 RID: 20980 RVA: 0x0020E65D File Offset: 0x0020C85D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int toChunkY(int _v)
	{
		return _v >> 8;
	}

	// Token: 0x060051F5 RID: 20981 RVA: 0x0020E662 File Offset: 0x0020C862
	public static Vector3 toChunkXyzWorldPos(Vector3i _v)
	{
		return new Vector3((float)(_v.x & -16), (float)(_v.y & -256), (float)(_v.z & -16));
	}

	// Token: 0x060051F6 RID: 20982 RVA: 0x0020E68A File Offset: 0x0020C88A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i toBlock(Vector3i _p)
	{
		_p.x &= 15;
		_p.y &= 255;
		_p.z &= 15;
		return _p;
	}

	// Token: 0x060051F7 RID: 20983 RVA: 0x0020E6C0 File Offset: 0x0020C8C0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i toBlock(int _x, int _y, int _z)
	{
		return new Vector3i(_x & 15, _y & 255, _z & 15);
	}

	// Token: 0x060051F8 RID: 20984 RVA: 0x001DCAF5 File Offset: 0x001DACF5
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int toBlockXZ(int _v)
	{
		return _v & 15;
	}

	// Token: 0x060051F9 RID: 20985 RVA: 0x0020E6D6 File Offset: 0x0020C8D6
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int toBlockY(int _v)
	{
		return _v & 255;
	}

	// Token: 0x060051FA RID: 20986 RVA: 0x0020E6DF File Offset: 0x0020C8DF
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 blockToTransformPos(Vector3i _blockPos)
	{
		return new Vector3((float)_blockPos.x + 0.5f, (float)_blockPos.y, (float)_blockPos.z + 0.5f);
	}

	// Token: 0x060051FB RID: 20987 RVA: 0x0020E707 File Offset: 0x0020C907
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i worldToBlockPos(Vector3 _worldPos)
	{
		return new Vector3i(Utils.Fastfloor(_worldPos.x), Utils.Fastfloor(_worldPos.y), Utils.Fastfloor(_worldPos.z));
	}

	// Token: 0x060051FC RID: 20988 RVA: 0x0020E72F File Offset: 0x0020C92F
	public override void SetBlockRPC(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		this.gameManager.SetBlocksRPC(new List<BlockChangeInfo>
		{
			new BlockChangeInfo(_clrIdx, _blockPos, _blockValue, true)
		}, null);
	}

	// Token: 0x060051FD RID: 20989 RVA: 0x0020E751 File Offset: 0x0020C951
	public override void SetBlockRPC(Vector3i _blockPos, BlockValue _blockValue)
	{
		this.gameManager.SetBlocksRPC(new List<BlockChangeInfo>
		{
			new BlockChangeInfo(_blockPos, _blockValue, true, false)
		}, null);
	}

	// Token: 0x060051FE RID: 20990 RVA: 0x0020E773 File Offset: 0x0020C973
	public override void SetBlockRPC(Vector3i _blockPos, BlockValue _blockValue, sbyte _density)
	{
		this.gameManager.SetBlocksRPC(new List<BlockChangeInfo>
		{
			new BlockChangeInfo(_blockPos, _blockValue, _density)
		}, null);
	}

	// Token: 0x060051FF RID: 20991 RVA: 0x0020E794 File Offset: 0x0020C994
	public override void SetBlockRPC(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, sbyte _density)
	{
		this.gameManager.SetBlocksRPC(new List<BlockChangeInfo>
		{
			new BlockChangeInfo(_clrIdx, _blockPos, _blockValue, _density)
		}, null);
	}

	// Token: 0x06005200 RID: 20992 RVA: 0x0020E7B7 File Offset: 0x0020C9B7
	public override void SetBlockRPC(Vector3i _blockPos, sbyte _density)
	{
		this.gameManager.SetBlocksRPC(new List<BlockChangeInfo>
		{
			new BlockChangeInfo(0, _blockPos, _density, false)
		}, null);
	}

	// Token: 0x06005201 RID: 20993 RVA: 0x0020E7D9 File Offset: 0x0020C9D9
	public override void SetBlocksRPC(List<BlockChangeInfo> _blockChangeInfo)
	{
		this.gameManager.SetBlocksRPC(_blockChangeInfo, null);
	}

	// Token: 0x06005202 RID: 20994 RVA: 0x0020E7E8 File Offset: 0x0020C9E8
	public override void SetBlockRPC(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, sbyte _density, int _changingEntityId)
	{
		this.gameManager.SetBlocksRPC(new List<BlockChangeInfo>
		{
			new BlockChangeInfo(_clrIdx, _blockPos, _blockValue, _density, _changingEntityId)
		}, null);
	}

	// Token: 0x06005203 RID: 20995 RVA: 0x0020E80D File Offset: 0x0020CA0D
	public override void SetBlockRPC(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _changingEntityId)
	{
		this.gameManager.SetBlocksRPC(new List<BlockChangeInfo>
		{
			new BlockChangeInfo(_clrIdx, _blockPos, _blockValue, true, _changingEntityId)
		}, null);
	}

	// Token: 0x06005204 RID: 20996 RVA: 0x0020E831 File Offset: 0x0020CA31
	public BlockValue SetBlock(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool bNotify, bool updateLight)
	{
		return this.ChunkClusters[_clrIdx].SetBlock(_blockPos, _blockValue, bNotify, updateLight);
	}

	// Token: 0x06005205 RID: 20997 RVA: 0x0020E84A File Offset: 0x0020CA4A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsDaytime()
	{
		return !this.IsDark();
	}

	// Token: 0x06005206 RID: 20998 RVA: 0x0020E858 File Offset: 0x0020CA58
	public bool IsDark()
	{
		float num = this.worldTime % 24000UL / 1000f;
		return num < (float)this.DawnHour || num > (float)this.DuskHour;
	}

	// Token: 0x06005207 RID: 20999 RVA: 0x0020E891 File Offset: 0x0020CA91
	public override TileEntity GetTileEntity(int _clrIdx, Vector3i _pos)
	{
		return this.GetTileEntity(_pos);
	}

	// Token: 0x06005208 RID: 21000 RVA: 0x0020E89C File Offset: 0x0020CA9C
	public override TileEntity GetTileEntity(Vector3i _pos)
	{
		ChunkCluster chunkCache = this.ChunkCache;
		if (chunkCache == null)
		{
			return null;
		}
		Chunk chunk = (Chunk)chunkCache.GetChunkFromWorldPos(_pos);
		if (chunk == null)
		{
			return null;
		}
		Vector3i blockPosInChunk = new Vector3i(World.toBlockXZ(_pos.x), World.toBlockY(_pos.y), World.toBlockXZ(_pos.z));
		return chunk.GetTileEntity(blockPosInChunk);
	}

	// Token: 0x06005209 RID: 21001 RVA: 0x0020E8F8 File Offset: 0x0020CAF8
	public TileEntity GetTileEntity(int _entityId)
	{
		Entity entity = this.GetEntity(_entityId);
		if (entity == null)
		{
			return null;
		}
		if (entity is EntityTrader && entity.IsAlive())
		{
			return ((EntityTrader)entity).TileEntityTrader;
		}
		if (entity.lootContainer == null)
		{
			string lootList = entity.GetLootList();
			if (!string.IsNullOrEmpty(lootList))
			{
				entity.lootContainer = new TileEntityLootContainer(null);
				entity.lootContainer.entityId = entity.entityId;
				entity.lootContainer.lootListName = lootList;
				entity.lootContainer.SetContainerSize(LootContainer.GetLootContainer(lootList, true).size, true);
			}
		}
		return entity.lootContainer;
	}

	// Token: 0x0600520A RID: 21002 RVA: 0x0020E994 File Offset: 0x0020CB94
	public void RemoveTileEntity(TileEntity _te)
	{
		Chunk chunk = _te.GetChunk();
		if (chunk != null)
		{
			chunk.RemoveTileEntity(this, _te);
			return;
		}
		Log.Error("RemoveTileEntity: chunk not found!");
	}

	// Token: 0x0600520B RID: 21003 RVA: 0x0020E9C0 File Offset: 0x0020CBC0
	public BlockTrigger GetBlockTrigger(int _clrIdx, Vector3i _pos)
	{
		ChunkCluster chunkCluster = this.ChunkClusters[_clrIdx];
		if (chunkCluster == null)
		{
			return null;
		}
		Chunk chunk = (Chunk)chunkCluster.GetChunkFromWorldPos(_pos);
		if (chunk == null)
		{
			return null;
		}
		Vector3i blockPosInChunk = new Vector3i(World.toBlockXZ(_pos.x), World.toBlockY(_pos.y), World.toBlockXZ(_pos.z));
		return chunk.GetBlockTrigger(blockPosInChunk);
	}

	// Token: 0x0600520C RID: 21004 RVA: 0x0020EA20 File Offset: 0x0020CC20
	public void OnUpdateTick(float _partialTicks, ArraySegment<long> _activeChunks)
	{
		this.updateChunkAddedRemovedCallbacks();
		this.WorldEventUpdateTime();
		WaterSplashCubes.Update();
		DecoManager.Instance.UpdateTick(this);
		MultiBlockManager.Instance.MainThreadUpdate();
		if (!this.IsEditor())
		{
			this.dmsConductor.Update();
		}
		this.checkPOIUnculling();
		this.updateChunksToUncull();
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		this.worldBlockTicker.Tick(_activeChunks, this.m_LocalPlayerEntity, this.rand);
		if (GameTimer.Instance.ticks % 20UL == 0UL)
		{
			bool @bool = GameStats.GetBool(EnumGameStats.IsSpawnEnemies);
			int num = 0;
			ChunkCluster chunkCluster = this.ChunkClusters[num];
			bool flag = GameTimer.Instance.ticks % 40UL == 0UL;
			for (int i = _activeChunks.Offset; i < _activeChunks.Count; i++)
			{
				long num2 = _activeChunks.Array[i];
				if ((!flag || num2 % 2L == 0L) && (flag || num2 % 2L != 0L))
				{
					int num3 = WorldChunkCache.extractClrIdx(num2);
					if (num3 != num)
					{
						ChunkCluster chunkCluster2 = this.ChunkClusters[num3];
						if (chunkCluster2 == null)
						{
							goto IL_1EA;
						}
						chunkCluster = chunkCluster2;
						num = num3;
					}
					Chunk chunk = (chunkCluster != null) ? chunkCluster.GetChunkSync(num2) : null;
					if (chunk != null)
					{
						if (chunk.NeedsTicking)
						{
							chunk.UpdateTick(this, @bool);
						}
						if (!this.IsEditor() && chunk.IsAreaMaster() && chunk.IsAreaMasterDominantBiomeInitialized(chunkCluster))
						{
							ChunkAreaBiomeSpawnData chunkBiomeSpawnData = chunk.GetChunkBiomeSpawnData();
							if (chunkBiomeSpawnData != null && chunkBiomeSpawnData.IsSpawnNeeded(this.Biomes, this.worldTime) && chunk.IsAreaMasterCornerChunksLoaded(chunkCluster))
							{
								if (this.areaMasterChunksToLock.ContainsKey(chunk.Key))
								{
									chunk.isModified |= chunkBiomeSpawnData.DelayAllEnemySpawningUntil(this.areaMasterChunksToLock[chunk.Key], this.Biomes);
									this.areaMasterChunksToLock.Remove(chunk.Key);
								}
								else
								{
									this.biomeSpawnManager.Update(string.Empty, @bool, chunkBiomeSpawnData);
								}
							}
						}
					}
				}
				IL_1EA:;
			}
		}
		if (GameTimer.Instance.ticks % 16UL == 0UL && GamePrefs.GetString(EnumGamePrefs.DynamicSpawner).Length > 0)
		{
			this.dynamicSpawnManager.Update(GamePrefs.GetString(EnumGamePrefs.DynamicSpawner), GameStats.GetBool(EnumGameStats.IsSpawnEnemies), null);
		}
		this.aiDirector.Tick((double)(_partialTicks / 20f));
		this.TickSleeperVolumes();
	}

	// Token: 0x0600520D RID: 21005 RVA: 0x0020EC7D File Offset: 0x0020CE7D
	public bool UncullPOI(PrefabInstance _pi)
	{
		if (_pi.AddChunksToUncull(this, this.chunksToUncull))
		{
			Log.Out("Unculling POI {0} {1}", new object[]
			{
				_pi.location.Name,
				_pi.boundingBoxPosition
			});
			return true;
		}
		return false;
	}

	// Token: 0x0600520E RID: 21006 RVA: 0x0020ECBD File Offset: 0x0020CEBD
	public void UncullChunk(Chunk _c)
	{
		if (_c.IsInternalBlocksCulled)
		{
			this.chunksToUncull.Add(_c);
		}
	}

	// Token: 0x0600520F RID: 21007 RVA: 0x0020ECD4 File Offset: 0x0020CED4
	[PublicizedFrom(EAccessModifier.Private)]
	public void checkPOIUnculling()
	{
		if (GameTimer.Instance.ticks % 38UL != 0UL || GameStats.GetInt(EnumGameStats.OptionsPOICulling) == 0)
		{
			return;
		}
		List<EntityPlayer> list = GameManager.Instance.World.Players.list;
		for (int i = 0; i < list.Count; i++)
		{
			EntityPlayer entityPlayer = list[i];
			if (entityPlayer.Spawned)
			{
				Dictionary<int, PrefabInstance> prefabsAroundNear = entityPlayer.GetPrefabsAroundNear();
				if (prefabsAroundNear != null)
				{
					foreach (KeyValuePair<int, PrefabInstance> keyValuePair in prefabsAroundNear)
					{
						PrefabInstance value = keyValuePair.Value;
						if (value.Overlaps(entityPlayer.position, 6f))
						{
							this.UncullPOI(value);
						}
					}
				}
			}
		}
	}

	// Token: 0x06005210 RID: 21008 RVA: 0x0020EDA0 File Offset: 0x0020CFA0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateChunksToUncull()
	{
		if (this.chunksToUncull.list.Count == 0)
		{
			return;
		}
		this.msUnculling.ResetAndRestart();
		this.chunksToRegenerate.Clear();
		for (int i = this.chunksToUncull.list.Count - 1; i >= 0; i--)
		{
			Chunk chunk = this.chunksToUncull.list[i];
			if (chunk.InProgressUnloading)
			{
				this.chunksToUncull.Remove(chunk);
			}
			else
			{
				BlockFaceFlag blockFaceFlag = chunk.RestoreCulledBlocks(this);
				this.chunksToUncull.Remove(chunk);
				if (!this.chunksToRegenerate.hashSet.Contains(chunk))
				{
					this.chunksToRegenerate.Add(chunk);
				}
				Chunk chunk2;
				if ((blockFaceFlag & BlockFaceFlag.West) != BlockFaceFlag.None && (chunk2 = (Chunk)this.GetChunkSync(chunk.X - 1, chunk.Z)) != null && !this.chunksToRegenerate.hashSet.Contains(chunk2))
				{
					this.chunksToRegenerate.Add(chunk2);
				}
				if ((blockFaceFlag & BlockFaceFlag.East) != BlockFaceFlag.None && (chunk2 = (Chunk)this.GetChunkSync(chunk.X + 1, chunk.Z)) != null && !this.chunksToRegenerate.hashSet.Contains(chunk2))
				{
					this.chunksToRegenerate.Add(chunk2);
				}
				if ((blockFaceFlag & BlockFaceFlag.North) != BlockFaceFlag.None && (chunk2 = (Chunk)this.GetChunkSync(chunk.X, chunk.Z + 1)) != null && !this.chunksToRegenerate.hashSet.Contains(chunk2))
				{
					this.chunksToRegenerate.Add(chunk2);
				}
				if ((blockFaceFlag & BlockFaceFlag.South) != BlockFaceFlag.None && (chunk2 = (Chunk)this.GetChunkSync(chunk.X, chunk.Z - 1)) != null && !this.chunksToRegenerate.hashSet.Contains(chunk2))
				{
					this.chunksToRegenerate.Add(chunk2);
				}
				if (this.msUnculling.ElapsedMilliseconds > 5L)
				{
					break;
				}
			}
		}
		for (int j = this.chunksToRegenerate.list.Count - 1; j >= 0; j--)
		{
			this.chunksToRegenerate.list[j].NeedsRegeneration = true;
		}
	}

	// Token: 0x06005211 RID: 21009 RVA: 0x0020EFA0 File Offset: 0x0020D1A0
	public Vector3[] GetRandomSpawnPointPositions(int _count)
	{
		Vector3[] array = new Vector3[_count];
		List<Chunk> chunkArrayCopySync = this.ChunkCache.GetChunkArrayCopySync();
		int count = chunkArrayCopySync.Count;
		while (_count > 0)
		{
			for (int i = 0; i < chunkArrayCopySync.Count; i++)
			{
				Chunk chunk = chunkArrayCopySync[i];
				if (this.GetGameRandom().RandomRange(count) == 1)
				{
					Chunk[] neighbours = new Chunk[8];
					if (this.ChunkCache.GetNeighborChunks(chunk, neighbours))
					{
						int num;
						int num2;
						int num3;
						if (chunk.FindRandomTopSoilPoint(this, out num, out num2, out num3, 5))
						{
							array[array.Length - _count] = new Vector3((float)num, (float)num2, (float)num3);
							_count--;
						}
						if (_count == 0)
						{
							break;
						}
					}
				}
			}
		}
		return array;
	}

	// Token: 0x06005212 RID: 21010 RVA: 0x0020F044 File Offset: 0x0020D244
	public Vector3 ClipBoundsMove(Entity _entity, Bounds _aabb, Vector3 move, Vector3 expandDir, float stepHeight)
	{
		if (stepHeight > 0f)
		{
			move.y = stepHeight;
		}
		Bounds bounds = BoundsUtils.ExpandDirectional(_aabb, expandDir);
		int num = Utils.Fastfloor(bounds.min.x - 0.5f);
		int num2 = Utils.Fastfloor(bounds.max.x + 1.5f);
		int num3 = Utils.Fastfloor(bounds.min.y - 0.5f);
		int num4 = Utils.Fastfloor(bounds.max.y + 1f);
		int num5 = Utils.Fastfloor(bounds.min.z - 0.5f);
		int num6 = Utils.Fastfloor(bounds.max.z + 1.5f);
		World.ClipBlock.ResetStorage();
		int num7 = 0;
		int num8 = 0;
		Chunk chunk = null;
		Vector3 blockPos = default(Vector3);
		for (int i = num; i < num2; i++)
		{
			blockPos.x = (float)i;
			int j = num5;
			while (j < num6)
			{
				blockPos.z = (float)j;
				if (chunk != null && chunk.X == World.toChunkXZ(i) && chunk.Z == World.toChunkXZ(j))
				{
					goto IL_145;
				}
				chunk = (Chunk)this.GetChunkFromWorldPos(i, j);
				if (chunk != null)
				{
					if (!this.IsInPlayfield(chunk))
					{
						this._clipBounds[num8++] = chunk.GetAABB();
						goto IL_145;
					}
					goto IL_145;
				}
				IL_1B0:
				j++;
				continue;
				IL_145:
				for (int k = num3; k < num4; k++)
				{
					if (k > 0 && k < 256)
					{
						BlockValue block = this.GetBlock(i, k, j);
						Block block2 = block.Block;
						if (block2.IsCollideMovement)
						{
							float yDistort = 0f;
							blockPos.y = (float)k;
							this._clipBlocks[num7++] = World.ClipBlock.New(block, block2, yDistort, blockPos, _aabb);
						}
					}
				}
				goto IL_1B0;
			}
		}
		Vector3 min = _aabb.min;
		Vector3 max = _aabb.max;
		if (move.y != 0f && num7 > 0)
		{
			for (int l = 0; l < num7; l++)
			{
				World.ClipBlock clipBlock = this._clipBlocks[l];
				IList<Bounds> clipBoundsList = clipBlock.block.GetClipBoundsList(clipBlock.value, clipBlock.pos);
				move.y = BoundsUtils.ClipBoundsMoveY(clipBlock.bmins, clipBlock.bmaxs, move.y, clipBoundsList, clipBoundsList.Count);
				if (move.y == 0f)
				{
					break;
				}
			}
		}
		if (move.y != 0f)
		{
			if (num8 > 0)
			{
				move.y = BoundsUtils.ClipBoundsMoveY(min, max, move.y, this._clipBounds, num8);
			}
			min.y += move.y;
			max.y += move.y;
			for (int m = 0; m < num7; m++)
			{
				World.ClipBlock clipBlock2 = this._clipBlocks[m];
				clipBlock2.bmins.y = clipBlock2.bmins.y + move.y;
				clipBlock2.bmaxs.y = clipBlock2.bmaxs.y + move.y;
			}
		}
		if (move.x != 0f && num7 > 0)
		{
			for (int n = 0; n < num7; n++)
			{
				World.ClipBlock clipBlock3 = this._clipBlocks[n];
				IList<Bounds> clipBoundsList2 = clipBlock3.block.GetClipBoundsList(clipBlock3.value, clipBlock3.pos);
				move.x = BoundsUtils.ClipBoundsMoveX(clipBlock3.bmins, clipBlock3.bmaxs, move.x, clipBoundsList2, clipBoundsList2.Count);
				if (move.x == 0f)
				{
					break;
				}
			}
		}
		if (move.x != 0f)
		{
			if (num8 > 0)
			{
				move.x = BoundsUtils.ClipBoundsMoveX(min, max, move.x, this._clipBounds, num8);
			}
			min.x += move.x;
			max.x += move.x;
			for (int num9 = 0; num9 < num7; num9++)
			{
				World.ClipBlock clipBlock4 = this._clipBlocks[num9];
				clipBlock4.bmins.x = clipBlock4.bmins.x + move.x;
				clipBlock4.bmaxs.x = clipBlock4.bmaxs.x + move.x;
			}
		}
		if (move.z != 0f && num7 > 0)
		{
			for (int num10 = 0; num10 < num7; num10++)
			{
				World.ClipBlock clipBlock5 = this._clipBlocks[num10];
				IList<Bounds> clipBoundsList3 = clipBlock5.block.GetClipBoundsList(clipBlock5.value, clipBlock5.pos);
				move.z = BoundsUtils.ClipBoundsMoveZ(clipBlock5.bmins, clipBlock5.bmaxs, move.z, clipBoundsList3, clipBoundsList3.Count);
				if (move.z == 0f)
				{
					break;
				}
			}
		}
		if (move.z != 0f)
		{
			if (num8 > 0)
			{
				move.z = BoundsUtils.ClipBoundsMoveZ(min, max, move.z, this._clipBounds, num8);
			}
			min.z += move.z;
			max.z += move.z;
			for (int num11 = 0; num11 < num7; num11++)
			{
				World.ClipBlock clipBlock6 = this._clipBlocks[num11];
				clipBlock6.bmins.z = clipBlock6.bmins.z + move.z;
				clipBlock6.bmaxs.z = clipBlock6.bmaxs.z + move.z;
			}
		}
		if (stepHeight > 0f)
		{
			stepHeight = -stepHeight;
			if (num7 > 0)
			{
				for (int num12 = 0; num12 < num7; num12++)
				{
					World.ClipBlock clipBlock7 = this._clipBlocks[num12];
					IList<Bounds> clipBoundsList4 = clipBlock7.block.GetClipBoundsList(clipBlock7.value, clipBlock7.pos);
					stepHeight = BoundsUtils.ClipBoundsMoveY(clipBlock7.bmins, clipBlock7.bmaxs, stepHeight, clipBoundsList4, clipBoundsList4.Count);
					if (stepHeight == 0f)
					{
						break;
					}
				}
			}
			if (stepHeight != 0f && num8 > 0)
			{
				stepHeight = BoundsUtils.ClipBoundsMoveY(min, max, stepHeight, this._clipBounds, num8);
			}
			move.y += stepHeight;
		}
		return move;
	}

	// Token: 0x06005213 RID: 21011 RVA: 0x0020F620 File Offset: 0x0020D820
	public List<Bounds> GetCollidingBounds(Entity _entity, Bounds _aabb, List<Bounds> collidingBoundingBoxes)
	{
		int num = Utils.Fastfloor(_aabb.min.x - 0.5f);
		int num2 = Utils.Fastfloor(_aabb.max.x + 0.5f);
		int num3 = Utils.Fastfloor(_aabb.min.y - 1f);
		int num4 = Utils.Fastfloor(_aabb.max.y + 1f);
		int num5 = Utils.Fastfloor(_aabb.min.z - 0.5f);
		int num6 = Utils.Fastfloor(_aabb.max.z + 0.5f);
		Chunk chunk = null;
		int i = num - 1;
		int num7 = 0;
		while (i <= num2 + 1)
		{
			if (num7 >= 50)
			{
				Log.Warning(string.Format("1BB exceeded size {0}: BB={1}", 50, _aabb.ToCultureInvariantString()));
				return collidingBoundingBoxes;
			}
			int j = num5 - 1;
			int num8 = 0;
			while (j <= num6 + 1)
			{
				if (num8 >= 50)
				{
					Log.Warning(string.Format("2BB exceeded size {0}: BB={1}", 50, _aabb.ToCultureInvariantString()));
					return collidingBoundingBoxes;
				}
				if (chunk != null && chunk.X == World.toChunkXZ(i) && chunk.Z == World.toChunkXZ(j))
				{
					goto IL_14B;
				}
				chunk = (Chunk)this.GetChunkFromWorldPos(i, j);
				if (chunk != null)
				{
					if (!this.IsInPlayfield(chunk))
					{
						collidingBoundingBoxes.Add(chunk.GetAABB());
						goto IL_14B;
					}
					goto IL_14B;
				}
				IL_1EB:
				j++;
				num8++;
				continue;
				IL_14B:
				int x = World.toBlockXZ(i);
				int z = World.toBlockXZ(j);
				int k = num3;
				int num9 = 0;
				while (k < num4)
				{
					if (k > 0 && k < 255)
					{
						BlockValue block = chunk.GetBlock(x, k, z);
						if (num9 >= 50)
						{
							Log.Warning(string.Format("3BB exceeded size {0}: BB={1}", 50, _aabb.ToCultureInvariantString()));
							return collidingBoundingBoxes;
						}
						this.collBlockCache[num7, num9, num8] = block;
						this.collDensityCache[num7, num9, num8] = chunk.GetDensity(x, k, z);
					}
					k++;
					num9++;
				}
				goto IL_1EB;
			}
			i++;
			num7++;
		}
		int l = num;
		int num10 = 0;
		while (l <= num2)
		{
			if (num10 >= 50)
			{
				Log.Warning(string.Format("4BB exceeded size {0}: BB={1}", 50, _aabb.ToCultureInvariantString()));
				return collidingBoundingBoxes;
			}
			int m = num5;
			int num11 = 0;
			while (m <= num6)
			{
				if (num11 >= 50)
				{
					Log.Warning(string.Format("5BB exceeded size {0}: BB={1}", 50, _aabb.ToCultureInvariantString()));
					return collidingBoundingBoxes;
				}
				int n = num3;
				int num12 = 0;
				while (n < num4)
				{
					if (n > 0 && n < 255)
					{
						if (num12 >= 50)
						{
							Log.Warning(string.Format("6BB exceeded size {0}: BB={1}", 50, _aabb.ToCultureInvariantString()));
							return collidingBoundingBoxes;
						}
						BlockValue blockValue = this.collBlockCache[num10 + 1, num12, num11 + 1];
						Block block2 = blockValue.Block;
						if (block2.IsCollideMovement)
						{
							float distortedAddY = 0f;
							if (block2.shape.IsTerrain())
							{
								distortedAddY = MarchingCubes.GetDecorationOffsetY(this.collDensityCache[num10 + 1, num12 + 1, num11 + 1], this.collDensityCache[num10 + 1, num12, num11 + 1]);
							}
							block2.GetCollidingAABB(blockValue, l, n, m, distortedAddY, _aabb, collidingBoundingBoxes);
						}
					}
					n++;
					num12++;
				}
				m++;
				num11++;
			}
			l++;
			num10++;
		}
		Bounds aabbOfEntity = _aabb;
		aabbOfEntity.Expand(0.25f);
		List<Entity> entitiesInBounds = this.GetEntitiesInBounds(_entity, aabbOfEntity);
		for (int num13 = 0; num13 < entitiesInBounds.Count; num13++)
		{
			Bounds boundingBox = entitiesInBounds[num13].getBoundingBox();
			if (boundingBox.Intersects(_aabb))
			{
				collidingBoundingBoxes.Add(boundingBox);
			}
			boundingBox = _entity.getBoundingBox();
			if (boundingBox.Intersects(_aabb))
			{
				collidingBoundingBoxes.Add(boundingBox);
			}
		}
		return collidingBoundingBoxes;
	}

	// Token: 0x06005214 RID: 21012 RVA: 0x0020FA18 File Offset: 0x0020DC18
	public List<Entity> GetEntitiesInBounds(Entity _excludeEntity, Bounds _aabbOfEntity)
	{
		this.entitiesWithinAABBExcludingEntity.Clear();
		int num = Utils.Fastfloor((_aabbOfEntity.min.x - 5f) / 16f);
		int num2 = Utils.Fastfloor((_aabbOfEntity.max.x + 5f) / 16f);
		int num3 = Utils.Fastfloor((_aabbOfEntity.min.z - 5f) / 16f);
		int num4 = Utils.Fastfloor((_aabbOfEntity.max.z + 5f) / 16f);
		for (int i = num; i <= num2; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				Chunk chunkSync = this.ChunkCache.GetChunkSync(i, j);
				if (chunkSync != null)
				{
					chunkSync.GetEntitiesInBounds(_excludeEntity, _aabbOfEntity, this.entitiesWithinAABBExcludingEntity, true);
				}
			}
		}
		return this.entitiesWithinAABBExcludingEntity;
	}

	// Token: 0x06005215 RID: 21013 RVA: 0x0020FAEC File Offset: 0x0020DCEC
	public List<Entity> GetEntitiesInBounds(Entity _excludeEntity, Bounds _aabbOfEntity, bool _isAlive)
	{
		this.entitiesWithinAABBExcludingEntity.Clear();
		int num = Utils.Fastfloor((_aabbOfEntity.min.x - 5f) / 16f);
		int num2 = Utils.Fastfloor((_aabbOfEntity.max.x + 5f) / 16f);
		int num3 = Utils.Fastfloor((_aabbOfEntity.min.z - 5f) / 16f);
		int num4 = Utils.Fastfloor((_aabbOfEntity.max.z + 5f) / 16f);
		for (int i = num; i <= num2; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				Chunk chunkSync = this.ChunkCache.GetChunkSync(i, j);
				if (chunkSync != null)
				{
					chunkSync.GetEntitiesInBounds(_excludeEntity, _aabbOfEntity, this.entitiesWithinAABBExcludingEntity, _isAlive);
				}
			}
		}
		return this.entitiesWithinAABBExcludingEntity;
	}

	// Token: 0x06005216 RID: 21014 RVA: 0x0020FBC0 File Offset: 0x0020DDC0
	public List<EntityAlive> GetLivingEntitiesInBounds(EntityAlive _excludeEntity, Bounds _aabbOfEntity)
	{
		this.livingEntitiesWithinAABBExcludingEntity.Clear();
		int num = Utils.Fastfloor((_aabbOfEntity.min.x - 5f) / 16f);
		int num2 = Utils.Fastfloor((_aabbOfEntity.max.x + 5f) / 16f);
		int num3 = Utils.Fastfloor((_aabbOfEntity.min.z - 5f) / 16f);
		int num4 = Utils.Fastfloor((_aabbOfEntity.max.z + 5f) / 16f);
		for (int i = num; i <= num2; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				Chunk chunkSync = this.ChunkCache.GetChunkSync(i, j);
				if (chunkSync != null)
				{
					chunkSync.GetLivingEntitiesInBounds(_excludeEntity, _aabbOfEntity, this.livingEntitiesWithinAABBExcludingEntity);
				}
			}
		}
		return this.livingEntitiesWithinAABBExcludingEntity;
	}

	// Token: 0x06005217 RID: 21015 RVA: 0x0020FC94 File Offset: 0x0020DE94
	public void GetEntitiesInBounds(FastTags<TagGroup.Global> _tags, Bounds _bb, List<Entity> _list)
	{
		int num = Utils.Fastfloor((_bb.min.x - 5f) / 16f);
		int num2 = Utils.Fastfloor((_bb.max.x + 5f) / 16f);
		int num3 = Utils.Fastfloor((_bb.min.z - 5f) / 16f);
		int num4 = Utils.Fastfloor((_bb.max.z + 5f) / 16f);
		for (int i = num3; i <= num4; i++)
		{
			for (int j = num; j <= num2; j++)
			{
				Chunk chunk = (Chunk)this.GetChunkSync(j, i);
				if (chunk != null)
				{
					chunk.GetEntitiesInBounds(_tags, _bb, _list);
				}
			}
		}
	}

	// Token: 0x06005218 RID: 21016 RVA: 0x0020FD54 File Offset: 0x0020DF54
	public List<Entity> GetEntitiesInBounds(Type _class, Bounds _bb, List<Entity> _list)
	{
		int num = Utils.Fastfloor((_bb.min.x - 5f) / 16f);
		int num2 = Utils.Fastfloor((_bb.max.x + 5f) / 16f);
		int num3 = Utils.Fastfloor((_bb.min.z - 5f) / 16f);
		int num4 = Utils.Fastfloor((_bb.max.z + 5f) / 16f);
		for (int i = num3; i <= num4; i++)
		{
			for (int j = num; j <= num2; j++)
			{
				Chunk chunk = (Chunk)this.GetChunkSync(j, i);
				if (chunk != null)
				{
					chunk.GetEntitiesInBounds(_class, _bb, _list);
				}
			}
		}
		return _list;
	}

	// Token: 0x06005219 RID: 21017 RVA: 0x0020FE14 File Offset: 0x0020E014
	public void GetEntitiesAround(EntityFlags _mask, Vector3 _pos, float _radius, List<Entity> _list)
	{
		int num = Utils.Fastfloor((_pos.x - _radius) / 16f);
		int num2 = Utils.Fastfloor((_pos.x + _radius) / 16f);
		int num3 = Utils.Fastfloor((_pos.z - _radius) / 16f);
		int num4 = Utils.Fastfloor((_pos.z + _radius) / 16f);
		for (int i = num3; i <= num4; i++)
		{
			for (int j = num; j <= num2; j++)
			{
				Chunk chunk = (Chunk)this.GetChunkSync(j, i);
				if (chunk != null)
				{
					chunk.GetEntitiesAround(_mask, _pos, _radius, _list);
				}
			}
		}
	}

	// Token: 0x0600521A RID: 21018 RVA: 0x0020FEAC File Offset: 0x0020E0AC
	public void GetEntitiesAround(EntityFlags _flags, EntityFlags _mask, Vector3 _pos, float _radius, List<Entity> _list)
	{
		_flags &= _mask;
		int num = Utils.Fastfloor((_pos.x - _radius) / 16f);
		int num2 = Utils.Fastfloor((_pos.x + _radius) / 16f);
		int num3 = Utils.Fastfloor((_pos.z - _radius) / 16f);
		int num4 = Utils.Fastfloor((_pos.z + _radius) / 16f);
		for (int i = num3; i <= num4; i++)
		{
			for (int j = num; j <= num2; j++)
			{
				Chunk chunk = (Chunk)this.GetChunkSync(j, i);
				if (chunk != null)
				{
					chunk.GetEntitiesAround(_flags, _mask, _pos, _radius, _list);
				}
			}
		}
	}

	// Token: 0x0600521B RID: 21019 RVA: 0x0020FF50 File Offset: 0x0020E150
	public int GetEntityAliveCount(EntityFlags _flags, EntityFlags _mask)
	{
		int num = 0;
		int count = this.EntityAlives.Count;
		for (int i = 0; i < count; i++)
		{
			if ((this.EntityAlives[i].entityFlags & _mask) == _flags)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x0600521C RID: 21020 RVA: 0x0020FF94 File Offset: 0x0020E194
	public void GetPlayersAround(Vector3 _pos, float _radius, List<EntityPlayer> _list)
	{
		float num = _radius * _radius;
		for (int i = this.Players.list.Count - 1; i >= 0; i--)
		{
			EntityPlayer entityPlayer = this.Players.list[i];
			if ((entityPlayer.position - _pos).sqrMagnitude <= num)
			{
				_list.Add(entityPlayer);
			}
		}
	}

	// Token: 0x0600521D RID: 21021 RVA: 0x0020FFF4 File Offset: 0x0020E1F4
	public void SetEntitiesVisibleNearToLocalPlayer()
	{
		EntityPlayerLocal primaryPlayer = this.GetPrimaryPlayer();
		if (primaryPlayer == null)
		{
			return;
		}
		bool aimingGun = primaryPlayer.AimingGun;
		Vector3 b = primaryPlayer.cameraTransform.position + Origin.position;
		for (int i = this.Entities.list.Count - 1; i >= 0; i--)
		{
			Entity entity = this.Entities.list[i];
			if (entity != primaryPlayer)
			{
				entity.VisiblityCheck((entity.position - b).sqrMagnitude, aimingGun);
			}
		}
	}

	// Token: 0x0600521E RID: 21022 RVA: 0x00210088 File Offset: 0x0020E288
	public void TickEntities(float _partialTicks)
	{
		int frameCount = Time.frameCount;
		int num = frameCount - this.tickEntityFrameCount;
		if (num <= 0)
		{
			num = 1;
		}
		this.tickEntityFrameCount = frameCount;
		this.tickEntityFrameCountAverage = this.tickEntityFrameCountAverage * 0.8f + (float)num * 0.2f;
		this.tickEntityPartialTicks = _partialTicks;
		this.tickEntityIndex = 0;
		this.tickEntityList.Clear();
		Entity primaryPlayer = this.GetPrimaryPlayer();
		int count = this.Entities.list.Count;
		for (int i = 0; i < count; i++)
		{
			Entity entity = this.Entities.list[i];
			if (entity != primaryPlayer)
			{
				this.tickEntityList.Add(entity);
			}
		}
		if (primaryPlayer)
		{
			this.TickEntity(primaryPlayer, _partialTicks);
		}
		this.EntityActivityUpdate();
		int num2 = (int)(this.tickEntityFrameCountAverage + 0.4f) - 1;
		if (num2 <= 0)
		{
			this.TickEntitiesFlush();
			return;
		}
		int num3 = (this.tickEntityList.Count - 25) / (num2 + 1);
		if (num3 < 0)
		{
			num3 = 0;
		}
		this.tickEntitySliceCount = (this.tickEntityList.Count - num3) / num2 + 1;
	}

	// Token: 0x0600521F RID: 21023 RVA: 0x002101A1 File Offset: 0x0020E3A1
	public void TickEntitiesFlush()
	{
		this.TickEntitiesSlice(this.tickEntityList.Count);
	}

	// Token: 0x06005220 RID: 21024 RVA: 0x002101B4 File Offset: 0x0020E3B4
	public void TickEntitiesSlice()
	{
		this.TickEntitiesSlice(this.tickEntitySliceCount);
	}

	// Token: 0x06005221 RID: 21025 RVA: 0x002101C4 File Offset: 0x0020E3C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void TickEntitiesSlice(int count)
	{
		int num = Utils.FastMin(this.tickEntityIndex + count, this.tickEntityList.Count);
		for (int i = this.tickEntityIndex; i < num; i++)
		{
			Entity entity = this.tickEntityList[i];
			if (entity)
			{
				this.TickEntity(entity, this.tickEntityPartialTicks);
			}
		}
		this.tickEntityIndex = num;
	}

	// Token: 0x06005222 RID: 21026 RVA: 0x00210224 File Offset: 0x0020E424
	[PublicizedFrom(EAccessModifier.Private)]
	public void TickEntity(Entity e, float _partialTicks)
	{
		e.SetLastTickPos(e.position);
		e.OnUpdatePosition(_partialTicks);
		e.CheckPosition();
		if (e.IsSpawned() && !e.IsMarkedForUnload())
		{
			Chunk chunk = (Chunk)this.GetChunkSync(e.chunkPosAddedEntityTo.x, e.chunkPosAddedEntityTo.z);
			bool flag = false;
			if (chunk != null)
			{
				if (!chunk.hasEntities)
				{
					flag = true;
				}
				else
				{
					chunk.AdJustEntityTracking(e);
				}
			}
			int num = World.toChunkXZ(Utils.Fastfloor(e.position.x));
			int num2 = World.toChunkXZ(Utils.Fastfloor(e.position.z));
			if (flag || !e.addedToChunk || e.chunkPosAddedEntityTo.x != num || e.chunkPosAddedEntityTo.z != num2)
			{
				if (e.addedToChunk && chunk != null)
				{
					chunk.RemoveEntityFromChunk(e);
				}
				chunk = (Chunk)this.GetChunkSync(num, num2);
				if (chunk != null)
				{
					e.addedToChunk = true;
					chunk.AddEntityToChunk(e);
				}
				else
				{
					e.addedToChunk = false;
				}
			}
			if (e is EntityPlayer || this.IsChunkAreaLoaded(e.position))
			{
				if (e.CanUpdateEntity())
				{
					e.OnUpdateEntity();
				}
				else if (e is EntityAlive)
				{
					((EntityAlive)e).CheckDespawn();
				}
			}
			else
			{
				EntityAlive entityAlive = e as EntityAlive;
				if (entityAlive != null)
				{
					entityAlive.SetAttackTarget(null, 0);
					entityAlive.CheckDespawn();
				}
			}
		}
		if (e.IsMarkedForUnload() && !e.isEntityRemote && !e.bWillRespawn)
		{
			this.unloadEntity(e, e.IsDespawned ? EnumRemoveEntityReason.Despawned : EnumRemoveEntityReason.Killed);
		}
	}

	// Token: 0x06005223 RID: 21027 RVA: 0x002103B4 File Offset: 0x0020E5B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void TickEntityRemove(Entity e)
	{
		int num = this.tickEntityList.IndexOf(e);
		if (num >= this.tickEntityIndex)
		{
			this.tickEntityList[num] = null;
		}
	}

	// Token: 0x06005224 RID: 21028 RVA: 0x002103E4 File Offset: 0x0020E5E4
	public void EntityActivityUpdate()
	{
		List<EntityPlayer> list = this.Players.list;
		if (list.Count == 0)
		{
			return;
		}
		for (int i = list.Count - 1; i >= 0; i--)
		{
			list[i].aiClosest.Clear();
		}
		int count = this.EntityAlives.Count;
		for (int j = 0; j < count; j++)
		{
			EntityAlive entityAlive = this.EntityAlives[j];
			EntityPlayer closestPlayer = this.GetClosestPlayer(entityAlive.position, -1f, false);
			if (closestPlayer)
			{
				closestPlayer.aiClosest.Add(entityAlive);
				entityAlive.aiClosestPlayer = closestPlayer;
				entityAlive.aiClosestPlayerDistSq = (closestPlayer.position - entityAlive.position).sqrMagnitude;
			}
			else
			{
				entityAlive.aiClosestPlayer = null;
				entityAlive.aiClosestPlayerDistSq = float.MaxValue;
			}
		}
		Vector3 b = Vector3.zero;
		float num = 0f;
		if (this.m_LocalPlayerEntity)
		{
			b = this.m_LocalPlayerEntity.cameraTransform.position + Origin.position;
			this.m_LocalPlayerEntity.emodel.ClothSimOn(!this.m_LocalPlayerEntity.AttachedToEntity);
			num = 625f;
			if (this.m_LocalPlayerEntity.AimingGun)
			{
				num = 3025f;
			}
		}
		int num2 = Utils.FastClamp(60 / list.Count, 4, 20);
		for (int k = list.Count - 1; k >= 0; k--)
		{
			EntityPlayer entityPlayer = list[k];
			entityPlayer.aiClosest.Sort((EntityAlive e1, EntityAlive e2) => e1.aiClosestPlayerDistSq.CompareTo(e2.aiClosestPlayerDistSq));
			for (int l = 0; l < entityPlayer.aiClosest.Count; l++)
			{
				EntityAlive entityAlive2 = entityPlayer.aiClosest[l];
				if (l < num2 || entityAlive2.aiClosestPlayerDistSq < 64f)
				{
					entityAlive2.aiActiveScale = 1f;
					bool on = entityAlive2.aiClosestPlayerDistSq < 36f;
					entityAlive2.emodel.JiggleOn(on);
				}
				else
				{
					float aiActiveScale = (entityAlive2.aiClosestPlayerDistSq < 225f) ? 0.3f : 0.1f;
					entityAlive2.aiActiveScale = aiActiveScale;
					entityAlive2.emodel.JiggleOn(false);
				}
			}
			if (entityPlayer != this.m_LocalPlayerEntity)
			{
				bool on2 = !entityPlayer.AttachedToEntity && (entityPlayer.position - b).sqrMagnitude < num;
				entityPlayer.emodel.ClothSimOn(on2);
			}
		}
	}

	// Token: 0x06005225 RID: 21029 RVA: 0x00210690 File Offset: 0x0020E890
	[PublicizedFrom(EAccessModifier.Private)]
	public void addToChunk(Entity e)
	{
		if (!e.addedToChunk)
		{
			Chunk chunk = (Chunk)this.GetChunkFromWorldPos(e.GetBlockPosition());
			if (chunk != null)
			{
				chunk.AddEntityToChunk(e);
			}
		}
	}

	// Token: 0x06005226 RID: 21030 RVA: 0x002106C4 File Offset: 0x0020E8C4
	public override void UnloadEntities(List<Entity> _entityList, bool _forceUnload = false)
	{
		for (int i = _entityList.Count - 1; i >= 0; i--)
		{
			Entity entity = _entityList[i];
			if (_forceUnload || (!entity.bWillRespawn && (!(entity.AttachedMainEntity != null) || !entity.AttachedMainEntity.bWillRespawn)))
			{
				this.unloadEntity(entity, EnumRemoveEntityReason.Unloaded);
			}
		}
	}

	// Token: 0x06005227 RID: 21031 RVA: 0x0021071C File Offset: 0x0020E91C
	public override Entity RemoveEntity(int _entityId, EnumRemoveEntityReason _reason)
	{
		Entity entity = this.GetEntity(_entityId);
		if (entity != null)
		{
			entity.MarkToUnload();
			this.unloadEntity(entity, _reason);
		}
		return entity;
	}

	// Token: 0x06005228 RID: 21032 RVA: 0x0021074C File Offset: 0x0020E94C
	public void unloadEntity(Entity _e, EnumRemoveEntityReason _reason)
	{
		EnumRemoveEntityReason unloadReason = _e.unloadReason;
		_e.unloadReason = _reason;
		if (!this.Entities.dict.ContainsKey(_e.entityId))
		{
			Log.Warning("{0} World unloadEntity !dict {1}, {2}, was {3}", new object[]
			{
				GameManager.frameCount,
				_e,
				_reason,
				unloadReason
			});
			return;
		}
		if (this.EntityUnloadedDelegates != null)
		{
			this.EntityUnloadedDelegates(_e, _reason);
		}
		if (_e.NavObject != null)
		{
			if (_reason == EnumRemoveEntityReason.Unloaded && _e is EntitySupplyCrate)
			{
				_e.NavObject.TrackedPosition = _e.position;
			}
			else
			{
				NavObjectManager.Instance.UnRegisterNavObject(_e.NavObject);
			}
		}
		_e.OnEntityUnload();
		this.Entities.Remove(_e.entityId);
		this.TickEntityRemove(_e);
		EntityAlive entityAlive = _e as EntityAlive;
		if (entityAlive)
		{
			this.EntityAlives.Remove(entityAlive);
		}
		this.RemoveEntityFromMap(_e, _reason);
		if (_e.addedToChunk && _e.IsMarkedForUnload())
		{
			Chunk chunk = (Chunk)this.GetChunkSync(_e.chunkPosAddedEntityTo.x, _e.chunkPosAddedEntityTo.z);
			if (chunk != null && !chunk.InProgressUnloading)
			{
				chunk.RemoveEntityFromChunk(_e);
			}
		}
		if (!this.IsRemote())
		{
			if (VehicleManager.Instance != null)
			{
				EntityVehicle entityVehicle = _e as EntityVehicle;
				if (entityVehicle)
				{
					VehicleManager.Instance.RemoveTrackedVehicle(entityVehicle, _reason);
				}
			}
			if (DroneManager.Instance != null)
			{
				EntityDrone entityDrone = _e as EntityDrone;
				if (entityDrone)
				{
					DroneManager.Instance.RemoveTrackedDrone(entityDrone, _reason);
				}
			}
			if (TurretTracker.Instance != null)
			{
				EntityTurret entityTurret = _e as EntityTurret;
				if (entityTurret)
				{
					TurretTracker.Instance.RemoveTrackedTurret(entityTurret, _reason);
				}
			}
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.entityDistributer.Remove(_e, _reason);
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && _e is EntityAlive && PathFinderThread.Instance != null)
		{
			PathFinderThread.Instance.RemovePathsFor(_e.entityId);
		}
		if (_e is EntityPlayer)
		{
			this.Players.Remove(_e.entityId);
			this.gameManager.HandlePersistentPlayerDisconnected(_e.entityId);
			this.playerEntityUpdateCount++;
			NavObjectManager.Instance.UnRegisterNavObjectByOwnerEntity(_e, "sleeping_bag");
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.aiDirector.RemoveEntity(_e);
		}
		this.audioManager.EntityRemovedFromWorld(_e, this);
		WeatherManager.EntityRemovedFromWorld(_e);
		LightManager.EntityRemovedFromWorld(_e, this);
	}

	// Token: 0x06005229 RID: 21033 RVA: 0x002109BC File Offset: 0x0020EBBC
	public override Entity GetEntity(int _entityId)
	{
		Entity result;
		this.Entities.dict.TryGetValue(_entityId, out result);
		return result;
	}

	// Token: 0x0600522A RID: 21034 RVA: 0x002109E0 File Offset: 0x0020EBE0
	public override void ChangeClientEntityIdToServer(int _clientEntityId, int _serverEntityId)
	{
		Entity entity = this.GetEntity(_clientEntityId);
		if (entity)
		{
			this.Entities.Remove(_clientEntityId);
			entity.entityId = _serverEntityId;
			entity.clientEntityId = 0;
			this.Entities.Add(_serverEntityId, entity);
		}
	}

	// Token: 0x0600522B RID: 21035 RVA: 0x00210A28 File Offset: 0x0020EC28
	public void SpawnEntityInWorld(Entity _entity)
	{
		if (_entity == null)
		{
			Log.Warning("Ignore spawning of empty entity");
			return;
		}
		if (this.EntityLoadedDelegates != null)
		{
			this.EntityLoadedDelegates(_entity);
		}
		this.AddEntityToMap(_entity);
		this.Entities.Add(_entity.entityId, _entity);
		this.addToChunk(_entity);
		EntityPlayer entityPlayer = _entity as EntityPlayer;
		EntityAlive entityAlive = (!entityPlayer) ? (_entity as EntityAlive) : null;
		if (entityAlive)
		{
			this.EntityAlives.Add(entityAlive);
		}
		if (!this.IsRemote())
		{
			EntityVehicle entityVehicle = _entity as EntityVehicle;
			if (entityVehicle != null && VehicleManager.Instance != null)
			{
				VehicleManager.Instance.AddTrackedVehicle(entityVehicle);
			}
			EntityDrone entityDrone = _entity as EntityDrone;
			if (entityDrone != null)
			{
				if (DroneManager.Instance != null)
				{
					DroneManager.Instance.AddTrackedDrone(entityDrone);
				}
				if (entityDrone.OriginalItemValue == null)
				{
					entityDrone.InitDynamicSpawn();
				}
			}
			EntityTurret entityTurret = _entity as EntityTurret;
			if (entityTurret != null)
			{
				if (TurretTracker.Instance != null)
				{
					TurretTracker.Instance.AddTrackedTurret(entityTurret);
				}
				if (entityTurret.OriginalItemValue.ItemClass == null)
				{
					entityTurret.InitDynamicSpawn();
				}
			}
		}
		if (this.audioManager != null)
		{
			this.audioManager.EntityAddedToWorld(_entity, this);
		}
		WeatherManager.EntityAddedToWorld(_entity);
		LightManager.EntityAddedToWorld(_entity, this);
		_entity.OnAddedToWorld();
		if (_entity.position.y < 1f)
		{
			Log.Warning(string.Concat(new string[]
			{
				"Spawned entity with wrong pos: ",
				(_entity != null) ? _entity.ToString() : null,
				" id=",
				_entity.entityId.ToString(),
				" pos=",
				_entity.position.ToCultureInvariantString()
			}));
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.entityDistributer.Add(_entity);
		}
		if (entityPlayer)
		{
			this.Players.Add(_entity.entityId, entityPlayer);
			this.playerEntityUpdateCount++;
		}
		else if (entityAlive)
		{
			entityAlive.Spawned = true;
			GameEventManager.Current.HandleSpawnModifier(entityAlive);
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.aiDirector.AddEntity(_entity);
		}
	}

	// Token: 0x0600522C RID: 21036 RVA: 0x00210C30 File Offset: 0x0020EE30
	public void AddEntityToMap(Entity _entity)
	{
		if (_entity == null)
		{
			return;
		}
		if (_entity.HasUIIcon() && _entity.GetMapObjectType() == EnumMapObjectType.Entity)
		{
			EntityVehicle entityVehicle = _entity as EntityVehicle;
			if (entityVehicle != null)
			{
				EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
				if (primaryPlayer != null)
				{
					LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(primaryPlayer);
					if (uiforPlayer != null && uiforPlayer.xui != null && uiforPlayer.xui.GetWindow("mapArea") != null)
					{
						((XUiC_MapArea)uiforPlayer.xui.GetWindow("mapArea").Controller).RefreshVehiclePositionWaypoint(entityVehicle, false);
						return;
					}
				}
			}
			else
			{
				EntityDrone entityDrone = _entity as EntityDrone;
				if (entityDrone != null)
				{
					EntityPlayerLocal primaryPlayer2 = GameManager.Instance.World.GetPrimaryPlayer();
					if (primaryPlayer2 != null)
					{
						LocalPlayerUI uiforPlayer2 = LocalPlayerUI.GetUIForPlayer(primaryPlayer2);
						if (uiforPlayer2 != null && uiforPlayer2.xui != null && uiforPlayer2.xui.GetWindow("mapArea") != null)
						{
							((XUiC_MapArea)uiforPlayer2.xui.GetWindow("mapArea").Controller).RefreshDronePositionWaypoint(entityDrone, false);
							return;
						}
					}
				}
				else
				{
					if (_entity is EntityEnemy || _entity is EntityEnemyAnimal)
					{
						this.ObjectOnMapAdd(new MapObjectZombie(_entity));
						return;
					}
					if (_entity is EntityAnimal)
					{
						this.ObjectOnMapAdd(new MapObjectAnimal(_entity));
						return;
					}
					this.ObjectOnMapAdd(new MapObject(EnumMapObjectType.Entity, Vector3.zero, (long)_entity.entityId, _entity, false));
				}
			}
		}
	}

	// Token: 0x0600522D RID: 21037 RVA: 0x00210DB0 File Offset: 0x0020EFB0
	public void RemoveEntityFromMap(Entity _entity, EnumRemoveEntityReason _reason)
	{
		if (_entity == null)
		{
			return;
		}
		EnumMapObjectType mapObjectType = _entity.GetMapObjectType();
		if (mapObjectType == EnumMapObjectType.SupplyDrop)
		{
			if (_reason == EnumRemoveEntityReason.Killed)
			{
				this.ObjectOnMapRemove(_entity.GetMapObjectType(), _entity.entityId);
				return;
			}
		}
		else
		{
			EntityVehicle entityVehicle = _entity as EntityVehicle;
			if (entityVehicle != null)
			{
				EntityPlayerLocal primaryPlayer = this.GetPrimaryPlayer();
				if (primaryPlayer != null)
				{
					LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(primaryPlayer);
					if (uiforPlayer != null)
					{
						if (_reason == EnumRemoveEntityReason.Unloaded)
						{
							if (entityVehicle.GetOwner() != null && entityVehicle.LocalPlayerIsOwner())
							{
								((XUiC_MapArea)uiforPlayer.xui.GetWindow("mapArea").Controller).RefreshVehiclePositionWaypoint(entityVehicle, true);
							}
						}
						else if (_reason == EnumRemoveEntityReason.Killed || _reason == EnumRemoveEntityReason.Despawned)
						{
							((XUiC_MapArea)uiforPlayer.xui.GetWindow("mapArea").Controller).RemoveVehicleLastKnownWaypoint(entityVehicle);
						}
					}
				}
			}
			else
			{
				EntityDrone entityDrone = _entity as EntityDrone;
				if (entityDrone != null)
				{
					EntityPlayerLocal primaryPlayer2 = this.GetPrimaryPlayer();
					if (primaryPlayer2 != null)
					{
						LocalPlayerUI uiforPlayer2 = LocalPlayerUI.GetUIForPlayer(primaryPlayer2);
						if (uiforPlayer2 != null)
						{
							if (_reason == EnumRemoveEntityReason.Unloaded)
							{
								if (entityDrone.LocalPlayerIsOwner())
								{
									((XUiC_MapArea)uiforPlayer2.xui.GetWindow("mapArea").Controller).RefreshDronePositionWaypoint(entityDrone, true);
								}
							}
							else if (_reason == EnumRemoveEntityReason.Killed || _reason == EnumRemoveEntityReason.Despawned)
							{
								((XUiC_MapArea)uiforPlayer2.xui.GetWindow("mapArea").Controller).RemoveDronePositionWaypoint(entityDrone.entityId);
							}
						}
					}
				}
			}
			this.ObjectOnMapRemove(mapObjectType, _entity.entityId);
		}
	}

	// Token: 0x0600522E RID: 21038 RVA: 0x00210F34 File Offset: 0x0020F134
	public void RefreshEntitiesOnMap()
	{
		foreach (Entity entity in this.Entities.list)
		{
			this.RemoveEntityFromMap(entity, EnumRemoveEntityReason.Undef);
			this.AddEntityToMap(entity);
		}
	}

	// Token: 0x0600522F RID: 21039 RVA: 0x00210F94 File Offset: 0x0020F194
	public void LockAreaMasterChunksAround(Vector3i _blockPos, ulong _worldTimeToLock)
	{
		for (int i = -2; i <= 2; i++)
		{
			for (int j = -2; j <= 2; j++)
			{
				Vector3i vector3i = Chunk.ToAreaMasterChunkPos(new Vector3i(_blockPos.x + i * 80, 0, _blockPos.z + j * 80));
				Chunk chunk = (Chunk)this.GetChunkSync(vector3i.x, vector3i.z);
				if (chunk != null && chunk.GetChunkBiomeSpawnData() != null)
				{
					chunk.isModified |= chunk.GetChunkBiomeSpawnData().DelayAllEnemySpawningUntil(_worldTimeToLock, this.Biomes);
				}
				else
				{
					this.areaMasterChunksToLock[WorldChunkCache.MakeChunkKey(vector3i.x, vector3i.z)] = _worldTimeToLock;
				}
			}
		}
	}

	// Token: 0x06005230 RID: 21040 RVA: 0x0021104C File Offset: 0x0020F24C
	public bool IsWaterInBounds(Bounds _aabb)
	{
		Vector3 min = _aabb.min;
		Vector3 max = _aabb.max;
		int num = Utils.Fastfloor(min.x);
		int num2 = Utils.Fastfloor(max.x + 1f);
		int num3 = Utils.Fastfloor(min.y);
		int num4 = Utils.Fastfloor(max.y + 1f);
		int num5 = Utils.Fastfloor(min.z);
		int num6 = Utils.Fastfloor(max.z + 1f);
		for (int i = num; i < num2; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				for (int k = num5; k < num6; k++)
				{
					if (this.IsWater(i, j, k))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06005231 RID: 21041 RVA: 0x00211108 File Offset: 0x0020F308
	public bool IsMaterialInBounds(Bounds _aabb, MaterialBlock _material)
	{
		int num = Utils.Fastfloor(_aabb.min.x);
		int num2 = Utils.Fastfloor(_aabb.max.x + 1f);
		int num3 = Utils.Fastfloor(_aabb.min.y);
		int num4 = Utils.Fastfloor(_aabb.max.y + 1f);
		int num5 = Utils.Fastfloor(_aabb.min.z);
		int num6 = Utils.Fastfloor(_aabb.max.z + 1f);
		for (int i = num; i < num2; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				for (int k = num5; k < num6; k++)
				{
					if (this.GetBlock(i, j, k).Block.blockMaterial == _material)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x1700084B RID: 2123
	// (get) Token: 0x06005232 RID: 21042 RVA: 0x002111E3 File Offset: 0x0020F3E3
	public Dictionary<Vector3i, float> fallingBlocksHashSet
	{
		get
		{
			return this.fallingBlocksMap;
		}
	}

	// Token: 0x06005233 RID: 21043 RVA: 0x002111EC File Offset: 0x0020F3EC
	public override void AddFallingBlocks(IList<Vector3i> _list)
	{
		for (int i = 0; i < _list.Count; i++)
		{
			this.AddFallingBlock(_list[i], false);
		}
	}

	// Token: 0x06005234 RID: 21044 RVA: 0x00211218 File Offset: 0x0020F418
	public void AddFallingBlock(Vector3i _blockPos, bool includeOversized = false)
	{
		if (!this.fallingBlocksMap.ContainsKey(_blockPos))
		{
			BlockValue block = this.GetBlock(_blockPos);
			if (block.ischild || block.Block.StabilityIgnore || block.isair || (!includeOversized && block.Block.isOversized))
			{
				return;
			}
			DynamicMeshManager.AddFallingBlockObserver(_blockPos);
			this.fallingBlocks.Enqueue(_blockPos);
			this.fallingBlocksMap[_blockPos] = Time.time;
		}
	}

	// Token: 0x06005235 RID: 21045 RVA: 0x00211290 File Offset: 0x0020F490
	public void LetBlocksFall()
	{
		if (this.fallingBlocks.Count == 0)
		{
			return;
		}
		int num = 0;
		Vector3i zero = Vector3i.zero;
		while (this.fallingBlocks.Count > 0 && num < 2)
		{
			Vector3i vector3i = this.fallingBlocks.Dequeue();
			if (zero.Equals(vector3i))
			{
				this.fallingBlocks.Enqueue(vector3i);
				return;
			}
			this.fallingBlocksMap.Remove(vector3i);
			BlockValue block = this.GetBlock(vector3i.x, vector3i.y, vector3i.z);
			if (!block.isair)
			{
				TextureFullArray textureFullArray = this.GetTextureFullArray(vector3i.x, vector3i.y, vector3i.z);
				Block block2 = block.Block;
				block2.OnBlockStartsToFall(this, vector3i, block);
				DynamicMeshManager.ChunkChanged(vector3i, -1, block.type);
				if (block2.ShowModelOnFall())
				{
					Vector3 transformPos = new Vector3((float)vector3i.x + 0.5f + this.RandomRange(-0.1f, 0.1f), (float)vector3i.y + 0.5f, (float)vector3i.z + 0.5f + this.RandomRange(-0.1f, 0.1f));
					EntityFallingBlock entity = (EntityFallingBlock)EntityFactory.CreateEntity(EntityClass.FromString("fallingBlock"), -1, block, textureFullArray, 1, transformPos, Vector3.zero, -1f, -1, null, -1, "");
					this.SpawnEntityInWorld(entity);
					num++;
				}
			}
		}
	}

	// Token: 0x06005236 RID: 21046 RVA: 0x002113EF File Offset: 0x0020F5EF
	public override IGameManager GetGameManager()
	{
		return this.gameManager;
	}

	// Token: 0x06005237 RID: 21047 RVA: 0x002113F7 File Offset: 0x0020F5F7
	public override Manager GetAudioManager()
	{
		return this.audioManager;
	}

	// Token: 0x06005238 RID: 21048 RVA: 0x002113FF File Offset: 0x0020F5FF
	public override AIDirector GetAIDirector()
	{
		return this.aiDirector;
	}

	// Token: 0x06005239 RID: 21049 RVA: 0x00211407 File Offset: 0x0020F607
	public override bool IsRemote()
	{
		return !SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer;
	}

	// Token: 0x0600523A RID: 21050 RVA: 0x00211418 File Offset: 0x0020F618
	public EntityPlayer GetClosestPlayer(float _x, float _y, float _z, int _notFromThisTeam, double _maxDistance)
	{
		float num = -1f;
		EntityPlayer result = null;
		for (int i = 0; i < this.Players.list.Count; i++)
		{
			EntityPlayer entityPlayer = this.Players.list[i];
			if (!entityPlayer.IsDead() && entityPlayer.Spawned && (_notFromThisTeam == 0 || entityPlayer.TeamNumber != _notFromThisTeam))
			{
				float distanceSq = entityPlayer.GetDistanceSq(new Vector3(_x, _y, _z));
				if ((_maxDistance < 0.0 || (double)distanceSq < _maxDistance * _maxDistance) && (num == -1f || distanceSq < num))
				{
					num = distanceSq;
					result = entityPlayer;
				}
			}
		}
		return result;
	}

	// Token: 0x0600523B RID: 21051 RVA: 0x002114B2 File Offset: 0x0020F6B2
	public EntityPlayer GetClosestPlayer(Entity _entity, float _distMax, bool _isDead)
	{
		return this.GetClosestPlayer(_entity.position, _distMax, _isDead);
	}

	// Token: 0x0600523C RID: 21052 RVA: 0x002114C4 File Offset: 0x0020F6C4
	public EntityPlayer GetClosestPlayer(Vector3 _pos, float _distMax, bool _isDead)
	{
		if (_distMax < 0f)
		{
			_distMax = float.MaxValue;
		}
		float num = _distMax * _distMax;
		EntityPlayer result = null;
		float num2 = float.MaxValue;
		for (int i = this.Players.list.Count - 1; i >= 0; i--)
		{
			EntityPlayer entityPlayer = this.Players.list[i];
			if (entityPlayer.IsDead() == _isDead && entityPlayer.Spawned)
			{
				float distanceSq = entityPlayer.GetDistanceSq(_pos);
				if (distanceSq < num2 && distanceSq <= num)
				{
					num2 = distanceSq;
					result = entityPlayer;
				}
			}
		}
		return result;
	}

	// Token: 0x0600523D RID: 21053 RVA: 0x0021154C File Offset: 0x0020F74C
	public EntityPlayer GetClosestPlayerSeen(EntityAlive _entity, float _distMax, float lightMin)
	{
		Vector3 position = _entity.position;
		if (_distMax < 0f)
		{
			_distMax = float.MaxValue;
		}
		float num = _distMax * _distMax;
		EntityPlayer result = null;
		float num2 = float.MaxValue;
		for (int i = this.Players.list.Count - 1; i >= 0; i--)
		{
			EntityPlayer entityPlayer = this.Players.list[i];
			if (!entityPlayer.IsDead() && entityPlayer.Spawned)
			{
				float distanceSq = entityPlayer.GetDistanceSq(position);
				if (distanceSq < num2 && distanceSq <= num && entityPlayer.Stealth.lightLevel >= lightMin && _entity.CanSee(entityPlayer))
				{
					num2 = distanceSq;
					result = entityPlayer;
				}
			}
		}
		return result;
	}

	// Token: 0x0600523E RID: 21054 RVA: 0x002115F8 File Offset: 0x0020F7F8
	public bool IsPlayerAliveAndNear(Vector3 _pos, float _distMax)
	{
		float num = _distMax * _distMax;
		for (int i = this.Players.list.Count - 1; i >= 0; i--)
		{
			EntityPlayer entityPlayer = this.Players.list[i];
			if (!entityPlayer.IsDead() && entityPlayer.Spawned && (entityPlayer.position - _pos).sqrMagnitude <= num)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600523F RID: 21055 RVA: 0x00211662 File Offset: 0x0020F862
	public override WorldBlockTicker GetWBT()
	{
		return this.worldBlockTicker;
	}

	// Token: 0x06005240 RID: 21056 RVA: 0x0021166A File Offset: 0x0020F86A
	public override bool IsOpenSkyAbove(int _clrIdx, int _x, int _y, int _z)
	{
		return this.ChunkClusters[_clrIdx] == null || ((Chunk)this.GetChunkSync(_x >> 4, _z >> 4)).IsOpenSkyAbove(_x & 15, _y, _z & 15);
	}

	// Token: 0x06005241 RID: 21057 RVA: 0x00052DED File Offset: 0x00050FED
	public override bool IsEditor()
	{
		return GameManager.Instance.IsEditMode();
	}

	// Token: 0x06005242 RID: 21058 RVA: 0x0021169D File Offset: 0x0020F89D
	public int GetGameMode()
	{
		return GameStats.GetInt(EnumGameStats.GameModeId);
	}

	// Token: 0x06005243 RID: 21059 RVA: 0x002116A5 File Offset: 0x0020F8A5
	public SpawnManagerDynamic GetDynamiceSpawnManager()
	{
		return this.dynamicSpawnManager;
	}

	// Token: 0x06005244 RID: 21060 RVA: 0x002116B0 File Offset: 0x0020F8B0
	public override bool CanPlaceLandProtectionBlockAt(Vector3i blockPos, PersistentPlayerData lpRelative)
	{
		if (GameStats.GetInt(EnumGameStats.GameModeId) != 1)
		{
			return true;
		}
		if (this.InBoundsForPlayersPercent(blockPos.ToVector3CenterXZ()) < 0.5f)
		{
			return false;
		}
		this.m_lpChunkList.Clear();
		int num = GameStats.GetInt(EnumGameStats.LandClaimSize) - 1;
		int num2 = GameStats.GetInt(EnumGameStats.LandClaimDeadZone) + num;
		int num3 = num2 / 16 + 1;
		int num4 = num2 / 16 + 1;
		for (int i = -num3; i <= num3; i++)
		{
			int x = blockPos.x + i * 16;
			for (int j = -num4; j <= num4; j++)
			{
				int z = blockPos.z + j * 16;
				Chunk chunk = (Chunk)this.GetChunkFromWorldPos(new Vector3i(x, blockPos.y, z));
				if (chunk != null && !this.m_lpChunkList.Contains(chunk))
				{
					this.m_lpChunkList.Add(chunk);
					if (this.IsLandProtectedBlock(chunk, blockPos, lpRelative, num, num2, true))
					{
						this.m_lpChunkList.Clear();
						return false;
					}
				}
			}
		}
		int num5 = num2 / 2;
		Vector3i other = new Vector3i(num5, num5, num5);
		Vector3i minPos = blockPos - other;
		Vector3i maxPos = blockPos + other;
		if (this.IsWithinTraderArea(minPos, maxPos))
		{
			return false;
		}
		this.m_lpChunkList.Clear();
		return true;
	}

	// Token: 0x06005245 RID: 21061 RVA: 0x002117E8 File Offset: 0x0020F9E8
	public bool IsEmptyPosition(Vector3i blockPos)
	{
		if (this.IsWithinTraderArea(blockPos))
		{
			return false;
		}
		if (GameStats.GetInt(EnumGameStats.GameModeId) != 1)
		{
			return true;
		}
		this.m_lpChunkList.Clear();
		int @int = GameStats.GetInt(EnumGameStats.LandClaimSize);
		int num = (@int - 1) / 2;
		int num2 = @int / 16 + 1;
		int num3 = @int / 16 + 1;
		int num4 = blockPos.x - num;
		int num5 = blockPos.z - num;
		for (int i = -num2; i <= num2; i++)
		{
			int x = num4 + i * 16;
			for (int j = -num3; j <= num3; j++)
			{
				int z = num5 + j * 16;
				Chunk chunk = (Chunk)this.GetChunkFromWorldPos(new Vector3i(x, blockPos.y, z));
				if (chunk != null && !this.m_lpChunkList.Contains(chunk))
				{
					this.m_lpChunkList.Add(chunk);
					if (this.IsLandProtectedBlock(chunk, blockPos, null, num, num, false))
					{
						this.m_lpChunkList.Clear();
						return false;
					}
				}
			}
		}
		this.m_lpChunkList.Clear();
		return true;
	}

	// Token: 0x06005246 RID: 21062 RVA: 0x002118E2 File Offset: 0x0020FAE2
	public override bool CanPickupBlockAt(Vector3i blockPos, PersistentPlayerData lpRelative)
	{
		return !this.IsWithinTraderArea(blockPos) && this.CanPlaceBlockAt(blockPos, lpRelative, false);
	}

	// Token: 0x06005247 RID: 21063 RVA: 0x002118F8 File Offset: 0x0020FAF8
	public override bool CanPlaceBlockAt(Vector3i blockPos, PersistentPlayerData lpRelative, bool traderAllowed = false)
	{
		if (!traderAllowed && this.IsWithinTraderArea(blockPos))
		{
			return false;
		}
		if (this.InBoundsForPlayersPercent(blockPos.ToVector3CenterXZ()) < 0.5f)
		{
			return false;
		}
		if (GameStats.GetInt(EnumGameStats.GameModeId) != 1)
		{
			return true;
		}
		this.m_lpChunkList.Clear();
		int @int = GameStats.GetInt(EnumGameStats.LandClaimSize);
		int num = (@int - 1) / 2;
		int num2 = @int / 16 + 1;
		int num3 = @int / 16 + 1;
		int num4 = blockPos.x - num;
		int num5 = blockPos.z - num;
		for (int i = -num2; i <= num2; i++)
		{
			int x = num4 + i * 16;
			for (int j = -num3; j <= num3; j++)
			{
				int z = num5 + j * 16;
				Chunk chunk = (Chunk)this.GetChunkFromWorldPos(new Vector3i(x, blockPos.y, z));
				if (chunk != null && !this.m_lpChunkList.Contains(chunk))
				{
					this.m_lpChunkList.Add(chunk);
					if (this.IsLandProtectedBlock(chunk, blockPos, lpRelative, num, num, false))
					{
						this.m_lpChunkList.Clear();
						return false;
					}
				}
			}
		}
		this.m_lpChunkList.Clear();
		return true;
	}

	// Token: 0x06005248 RID: 21064 RVA: 0x00211A0C File Offset: 0x0020FC0C
	public override float GetLandProtectionHardnessModifier(Vector3i blockPos, EntityAlive lpRelative, PersistentPlayerData ppData)
	{
		if (GameStats.GetInt(EnumGameStats.GameModeId) != 1)
		{
			return 1f;
		}
		if (lpRelative is EntityEnemy || lpRelative == null)
		{
			return 1f;
		}
		float num = 1f;
		BlockValue block = this.GetBlock(blockPos);
		if (!block.Equals(BlockValue.Air))
		{
			num = block.Block.LPHardnessScale;
			if (num == 0f)
			{
				return 1f;
			}
		}
		this.m_lpChunkList.Clear();
		int @int = GameStats.GetInt(EnumGameStats.LandClaimSize);
		int num2 = (@int - 1) / 2;
		int num3 = @int / 16 + 1;
		int num4 = @int / 16 + 1;
		int num5 = blockPos.x - num2;
		int num6 = blockPos.z - num2;
		float num7 = 1f;
		for (int i = -num3; i <= num3; i++)
		{
			int x = num5 + i * 16;
			for (int j = -num4; j <= num4; j++)
			{
				int z = num6 + j * 16;
				Chunk chunk = (Chunk)this.GetChunkFromWorldPos(new Vector3i(x, blockPos.y, z));
				if (chunk != null && !this.m_lpChunkList.Contains(chunk))
				{
					this.m_lpChunkList.Add(chunk);
					float landProtectionHardnessModifier = this.GetLandProtectionHardnessModifier(chunk, blockPos, ppData, num2);
					if (landProtectionHardnessModifier < 1f)
					{
						this.m_lpChunkList.Clear();
						return landProtectionHardnessModifier;
					}
					num7 = Math.Max(num7, landProtectionHardnessModifier);
				}
			}
		}
		this.m_lpChunkList.Clear();
		if (num7 > 1f)
		{
			if (lpRelative is EntityVehicle)
			{
				num7 *= 2f;
			}
			return num7 * num;
		}
		return num7;
	}

	// Token: 0x06005249 RID: 21065 RVA: 0x00211B90 File Offset: 0x0020FD90
	[PublicizedFrom(EAccessModifier.Private)]
	public float GetLandProtectionHardnessModifier(Chunk chunk, Vector3i blockPos, PersistentPlayerData lpRelative, int halfClaimSize)
	{
		float num = 1f;
		PersistentPlayerList persistentPlayerList = this.gameManager.GetPersistentPlayerList();
		List<Vector3i> list = chunk.IndexedBlocks["lpblock"];
		if (list != null)
		{
			Vector3i worldPos = chunk.GetWorldPos();
			for (int i = 0; i < list.Count; i++)
			{
				Vector3i vector3i = list[i] + worldPos;
				if (BlockLandClaim.IsPrimary(chunk.GetBlock(list[i])))
				{
					PersistentPlayerData landProtectionBlockOwner = persistentPlayerList.GetLandProtectionBlockOwner(vector3i);
					if (landProtectionBlockOwner != null && (lpRelative == null || (landProtectionBlockOwner != lpRelative && (blockPos == vector3i || landProtectionBlockOwner.ACL == null || !landProtectionBlockOwner.ACL.Contains(lpRelative.PrimaryId)))))
					{
						int num2 = Math.Abs(vector3i.x - blockPos.x);
						int num3 = Math.Abs(vector3i.z - blockPos.z);
						if (num2 <= halfClaimSize && num3 <= halfClaimSize)
						{
							float landProtectionHardnessModifierForPlayer = this.GetLandProtectionHardnessModifierForPlayer(landProtectionBlockOwner);
							if (landProtectionHardnessModifierForPlayer < 1f)
							{
								return landProtectionHardnessModifierForPlayer;
							}
							num = Mathf.Max(num, landProtectionHardnessModifierForPlayer);
							if (lpRelative != null)
							{
								EntityPlayer entityPlayer = this.GetEntity(lpRelative.EntityId) as EntityPlayer;
								num = EffectManager.GetValue(PassiveEffects.LandClaimDamageModifier, entityPlayer.inventory.holdingItemItemValue, num, entityPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
							}
						}
					}
				}
			}
		}
		return num;
	}

	// Token: 0x0600524A RID: 21066 RVA: 0x00211CE8 File Offset: 0x0020FEE8
	public override bool IsMyLandProtectedBlock(Vector3i worldBlockPos, PersistentPlayerData lpRelative, bool traderAllowed = false)
	{
		if (GameStats.GetInt(EnumGameStats.GameModeId) != 1)
		{
			return true;
		}
		if (!traderAllowed && this.IsWithinTraderArea(worldBlockPos))
		{
			return false;
		}
		this.m_lpChunkList.Clear();
		int @int = GameStats.GetInt(EnumGameStats.LandClaimSize);
		int num = (@int - 1) / 2;
		int num2 = @int / 16 + 1;
		int num3 = @int / 16 + 1;
		int num4 = worldBlockPos.x - num;
		int num5 = worldBlockPos.z - num;
		for (int i = -num2; i <= num2; i++)
		{
			int x = num4 + i * 16;
			for (int j = -num3; j <= num3; j++)
			{
				int z = num5 + j * 16;
				Chunk chunk = (Chunk)this.GetChunkFromWorldPos(new Vector3i(x, worldBlockPos.y, z));
				if (chunk != null && !this.m_lpChunkList.Contains(chunk))
				{
					this.m_lpChunkList.Add(chunk);
					if (this.IsMyLandClaimInChunk(chunk, worldBlockPos, lpRelative, num, num, false))
					{
						this.m_lpChunkList.Clear();
						return true;
					}
				}
			}
		}
		this.m_lpChunkList.Clear();
		return false;
	}

	// Token: 0x0600524B RID: 21067 RVA: 0x00211DE8 File Offset: 0x0020FFE8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsLandProtectedBlock(Chunk chunk, Vector3i blockPos, PersistentPlayerData lpRelative, int claimSize, int deadZone, bool forKeystone)
	{
		PersistentPlayerList persistentPlayerList = this.gameManager.GetPersistentPlayerList();
		List<Vector3i> list = chunk.IndexedBlocks["lpblock"];
		if (list != null)
		{
			Vector3i worldPos = chunk.GetWorldPos();
			for (int i = 0; i < list.Count; i++)
			{
				Vector3i vector3i = list[i] + worldPos;
				if (BlockLandClaim.IsPrimary(chunk.GetBlock(list[i])))
				{
					int num = Math.Abs(vector3i.x - blockPos.x);
					int num2 = Math.Abs(vector3i.z - blockPos.z);
					if (num <= deadZone && num2 <= deadZone)
					{
						PersistentPlayerData landProtectionBlockOwner = persistentPlayerList.GetLandProtectionBlockOwner(vector3i);
						if (landProtectionBlockOwner != null)
						{
							bool flag = this.IsLandProtectionValidForPlayer(landProtectionBlockOwner);
							if (flag && lpRelative != null)
							{
								if (lpRelative == landProtectionBlockOwner)
								{
									flag = false;
								}
								else if (landProtectionBlockOwner.ACL != null && landProtectionBlockOwner.ACL.Contains(lpRelative.PrimaryId))
								{
									flag = (num <= claimSize && num2 <= claimSize && forKeystone);
								}
							}
							if (flag)
							{
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x0600524C RID: 21068 RVA: 0x00211EF8 File Offset: 0x002100F8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsMyLandClaimInChunk(Chunk chunk, Vector3i blockPos, PersistentPlayerData lpRelative, int claimSize, int deadZone, bool forKeystone)
	{
		PersistentPlayerList persistentPlayerList = this.gameManager.GetPersistentPlayerList();
		List<Vector3i> list = chunk.IndexedBlocks["lpblock"];
		if (list != null)
		{
			Vector3i worldPos = chunk.GetWorldPos();
			for (int i = 0; i < list.Count; i++)
			{
				Vector3i vector3i = list[i] + worldPos;
				if (BlockLandClaim.IsPrimary(chunk.GetBlock(list[i])))
				{
					int num = Math.Abs(vector3i.x - blockPos.x);
					int num2 = Math.Abs(vector3i.z - blockPos.z);
					if (num <= deadZone && num2 <= deadZone)
					{
						PersistentPlayerData landProtectionBlockOwner = persistentPlayerList.GetLandProtectionBlockOwner(vector3i);
						if (landProtectionBlockOwner != null)
						{
							bool flag = this.IsLandProtectionValidForPlayer(landProtectionBlockOwner);
							if (flag && lpRelative != null)
							{
								if (lpRelative == landProtectionBlockOwner)
								{
									flag = (num <= claimSize && num2 <= claimSize);
								}
								else
								{
									flag = (landProtectionBlockOwner.ACL != null && landProtectionBlockOwner.ACL.Contains(lpRelative.PrimaryId) && (num <= claimSize && num2 <= claimSize && forKeystone));
								}
							}
							if (flag)
							{
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x0600524D RID: 21069 RVA: 0x0021201C File Offset: 0x0021021C
	public override EnumLandClaimOwner GetLandClaimOwner(Vector3i worldBlockPos, PersistentPlayerData lpRelative)
	{
		if (GameStats.GetInt(EnumGameStats.GameModeId) != 1)
		{
			return EnumLandClaimOwner.Self;
		}
		if (this.IsWithinTraderArea(worldBlockPos))
		{
			return EnumLandClaimOwner.None;
		}
		this.m_lpChunkList.Clear();
		int @int = GameStats.GetInt(EnumGameStats.LandClaimSize);
		int num = (@int - 1) / 2;
		int num2 = @int / 16 + 1;
		int num3 = @int / 16 + 1;
		int num4 = worldBlockPos.x - num;
		int num5 = worldBlockPos.z - num;
		for (int i = -num2; i <= num2; i++)
		{
			int x = num4 + i * 16;
			for (int j = -num3; j <= num3; j++)
			{
				int z = num5 + j * 16;
				Chunk chunk = (Chunk)this.GetChunkFromWorldPos(new Vector3i(x, worldBlockPos.y, z));
				if (chunk != null && !this.m_lpChunkList.Contains(chunk))
				{
					this.m_lpChunkList.Add(chunk);
					EnumLandClaimOwner landClaimOwner = this.GetLandClaimOwner(chunk, worldBlockPos, lpRelative, num, num, false);
					if (landClaimOwner != EnumLandClaimOwner.None)
					{
						this.m_lpChunkList.Clear();
						return landClaimOwner;
					}
				}
			}
		}
		this.m_lpChunkList.Clear();
		return EnumLandClaimOwner.None;
	}

	// Token: 0x0600524E RID: 21070 RVA: 0x0021211C File Offset: 0x0021031C
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumLandClaimOwner GetLandClaimOwner(Chunk chunk, Vector3i blockPos, PersistentPlayerData lpRelative, int claimSize, int deadZone, bool forKeystone)
	{
		PersistentPlayerList persistentPlayerList = this.gameManager.GetPersistentPlayerList();
		List<Vector3i> list = chunk.IndexedBlocks["lpblock"];
		if (list != null)
		{
			Vector3i worldPos = chunk.GetWorldPos();
			for (int i = 0; i < list.Count; i++)
			{
				Vector3i vector3i = list[i] + worldPos;
				if (BlockLandClaim.IsPrimary(chunk.GetBlock(list[i])))
				{
					int num = Math.Abs(vector3i.x - blockPos.x);
					int num2 = Math.Abs(vector3i.z - blockPos.z);
					if (num <= deadZone && num2 <= deadZone)
					{
						PersistentPlayerData landProtectionBlockOwner = persistentPlayerList.GetLandProtectionBlockOwner(vector3i);
						if (landProtectionBlockOwner != null && this.IsLandProtectionValidForPlayer(landProtectionBlockOwner))
						{
							if (lpRelative == null)
							{
								return EnumLandClaimOwner.Other;
							}
							if (lpRelative == landProtectionBlockOwner)
							{
								return EnumLandClaimOwner.Self;
							}
							if (landProtectionBlockOwner.ACL != null && landProtectionBlockOwner.ACL.Contains(lpRelative.PrimaryId))
							{
								return EnumLandClaimOwner.Ally;
							}
							return EnumLandClaimOwner.Other;
						}
					}
				}
			}
		}
		return EnumLandClaimOwner.None;
	}

	// Token: 0x0600524F RID: 21071 RVA: 0x00212208 File Offset: 0x00210408
	public bool GetLandClaimOwnerInParty(EntityPlayer player, PersistentPlayerData lpRelative)
	{
		if (GameStats.GetInt(EnumGameStats.GameModeId) != 1)
		{
			return false;
		}
		Vector3i blockPosition = player.GetBlockPosition();
		if (this.IsWithinTraderArea(blockPosition))
		{
			return false;
		}
		this.m_lpChunkList.Clear();
		int @int = GameStats.GetInt(EnumGameStats.LandClaimSize);
		int num = (@int - 1) / 2;
		int num2 = @int / 16 + 1;
		int num3 = @int / 16 + 1;
		int num4 = blockPosition.x - num;
		int num5 = blockPosition.z - num;
		for (int i = -num2; i <= num2; i++)
		{
			int x = num4 + i * 16;
			for (int j = -num3; j <= num3; j++)
			{
				int z = num5 + j * 16;
				Chunk chunk = (Chunk)this.GetChunkFromWorldPos(new Vector3i(x, blockPosition.y, z));
				if (chunk != null && !this.m_lpChunkList.Contains(chunk))
				{
					this.m_lpChunkList.Add(chunk);
					if (this.GetLandClaimOwnerInParty(chunk, player, blockPosition, lpRelative, num, num))
					{
						this.m_lpChunkList.Clear();
						return true;
					}
				}
			}
		}
		this.m_lpChunkList.Clear();
		return false;
	}

	// Token: 0x06005250 RID: 21072 RVA: 0x0021230C File Offset: 0x0021050C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool GetLandClaimOwnerInParty(Chunk chunk, EntityPlayer player, Vector3i blockPos, PersistentPlayerData lpRelative, int claimSize, int deadZone)
	{
		PersistentPlayerList persistentPlayerList = this.gameManager.GetPersistentPlayerList();
		bool flag = player.Party != null;
		List<Vector3i> list = chunk.IndexedBlocks["lpblock"];
		if (list != null)
		{
			Vector3i worldPos = chunk.GetWorldPos();
			for (int i = 0; i < list.Count; i++)
			{
				Vector3i vector3i = list[i] + worldPos;
				if (BlockLandClaim.IsPrimary(chunk.GetBlock(list[i])))
				{
					int num = Math.Abs(vector3i.x - blockPos.x);
					int num2 = Math.Abs(vector3i.z - blockPos.z);
					if (num <= deadZone && num2 <= deadZone)
					{
						PersistentPlayerData landProtectionBlockOwner = persistentPlayerList.GetLandProtectionBlockOwner(vector3i);
						if (landProtectionBlockOwner != null)
						{
							if (lpRelative == null && player != null && landProtectionBlockOwner.EntityId == player.entityId)
							{
								lpRelative = landProtectionBlockOwner;
							}
							if (this.IsLandProtectionValidForPlayer(landProtectionBlockOwner) && lpRelative != null)
							{
								if (lpRelative == landProtectionBlockOwner)
								{
									if (num <= claimSize && num2 <= claimSize)
									{
										return true;
									}
								}
								else if (flag && landProtectionBlockOwner.ACL != null && landProtectionBlockOwner.ACL.Contains(lpRelative.PrimaryId) && player.Party.ContainsMember(landProtectionBlockOwner.EntityId) && num <= claimSize && num2 <= claimSize)
								{
									return true;
								}
							}
							return false;
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06005251 RID: 21073 RVA: 0x00212468 File Offset: 0x00210668
	public bool IsLandProtectionValidForPlayer(PersistentPlayerData ppData)
	{
		double num = (double)GameStats.GetInt(EnumGameStats.LandClaimExpiryTime) * 24.0;
		return ppData.OfflineHours <= num;
	}

	// Token: 0x06005252 RID: 21074 RVA: 0x00212494 File Offset: 0x00210694
	public float GetLandProtectionHardnessModifierForPlayer(PersistentPlayerData ppData)
	{
		float result = (float)GameStats.GetInt(EnumGameStats.LandClaimOnlineDurabilityModifier);
		if (ppData.EntityId != -1)
		{
			return result;
		}
		double offlineHours = ppData.OfflineHours;
		double offlineMinutes = ppData.OfflineMinutes;
		float num = (float)GameStats.GetInt(EnumGameStats.LandClaimOfflineDelay);
		if (num != 0f && offlineMinutes <= (double)num)
		{
			return result;
		}
		double num2 = (double)GameStats.GetInt(EnumGameStats.LandClaimExpiryTime) * 24.0;
		if (offlineHours > num2)
		{
			return 1f;
		}
		EnumLandClaimDecayMode @int = (EnumLandClaimDecayMode)GameStats.GetInt(EnumGameStats.LandClaimDecayMode);
		float num3 = (float)GameStats.GetInt(EnumGameStats.LandClaimOfflineDurabilityModifier);
		if (num3 == 0f)
		{
			return 0f;
		}
		if (@int == EnumLandClaimDecayMode.DecaySlowly)
		{
			double num4 = (offlineHours - 24.0) / (num2 - 24.0);
			return Mathf.Max(1f, (float)(1.0 - num4) * num3);
		}
		if (@int == EnumLandClaimDecayMode.BuffedUntilExpired)
		{
			return num3;
		}
		double num5 = (offlineHours - 24.0) / (num2 - 24.0);
		return Mathf.Max(1f, (float)((1.0 - num5) * (1.0 - num5)) * num3);
	}

	// Token: 0x06005253 RID: 21075 RVA: 0x002125A0 File Offset: 0x002107A0
	public float GetDecorationOffsetY(Vector3i _blockPos)
	{
		sbyte density = this.GetDensity(0, _blockPos);
		sbyte density2 = this.GetDensity(0, _blockPos - Vector3i.up);
		return MarchingCubes.GetDecorationOffsetY(density, density2);
	}

	// Token: 0x06005254 RID: 21076 RVA: 0x002125D0 File Offset: 0x002107D0
	public EnumDecoAllowed GetDecoAllowedAt(int _x, int _z)
	{
		Chunk chunk = (Chunk)this.GetChunkFromWorldPos(_x, _z);
		if (chunk != null)
		{
			return chunk.GetDecoAllowedAt(World.toBlockXZ(_x), World.toBlockXZ(_z));
		}
		return EnumDecoAllowed.Nothing;
	}

	// Token: 0x06005255 RID: 21077 RVA: 0x00212608 File Offset: 0x00210808
	public void SetDecoAllowedAt(int _x, int _z, EnumDecoAllowed _decoAllowed)
	{
		Chunk chunk = (Chunk)this.GetChunkFromWorldPos(_x, _z);
		if (chunk != null)
		{
			chunk.SetDecoAllowedAt(World.toBlockXZ(_x), World.toBlockXZ(_z), _decoAllowed);
		}
	}

	// Token: 0x06005256 RID: 21078 RVA: 0x0021263C File Offset: 0x0021083C
	public Vector3 GetTerrainNormalAt(int _x, int _z)
	{
		Chunk chunk = (Chunk)this.GetChunkFromWorldPos(_x, _z);
		if (chunk != null)
		{
			return chunk.GetTerrainNormal(World.toBlockXZ(_x), World.toBlockXZ(_z));
		}
		return Vector3.zero;
	}

	// Token: 0x06005257 RID: 21079 RVA: 0x00212672 File Offset: 0x00210872
	public bool GetWorldExtent(out Vector3i _minSize, out Vector3i _maxSize)
	{
		return this.ChunkCache.ChunkProvider.GetWorldExtent(out _minSize, out _maxSize);
	}

	// Token: 0x06005258 RID: 21080 RVA: 0x00212688 File Offset: 0x00210888
	public virtual bool IsPositionAvailable(int _clrIdx, Vector3 _position)
	{
		ChunkCluster chunkCluster = this.ChunkClusters[_clrIdx];
		if (chunkCluster == null)
		{
			return false;
		}
		Vector3i one = World.worldToBlockPos(_position);
		for (int i = 0; i < Vector3i.MIDDLE_AND_HORIZONTAL_DIRECTIONS_DIAGONAL.Length; i++)
		{
			Vector3i other = Vector3i.MIDDLE_AND_HORIZONTAL_DIRECTIONS_DIAGONAL[i] * 16;
			IChunk chunkFromWorldPos = chunkCluster.GetChunkFromWorldPos(one + other);
			if (chunkFromWorldPos == null || !chunkFromWorldPos.GetAvailable())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06005259 RID: 21081 RVA: 0x002126F4 File Offset: 0x002108F4
	public bool GetBiomeIntensity(Vector3i _position, out BiomeIntensity _biomeIntensity)
	{
		Chunk chunk = (Chunk)this.GetChunkFromWorldPos(_position);
		if (chunk != null && !chunk.NeedsLightCalculation)
		{
			_biomeIntensity = chunk.GetBiomeIntensity(World.toBlockXZ(_position.x), World.toBlockXZ(_position.z));
			return true;
		}
		_biomeIntensity = BiomeIntensity.Default;
		return false;
	}

	// Token: 0x0600525A RID: 21082 RVA: 0x0021274C File Offset: 0x0021094C
	public bool CanMobsSpawnAtPos(Vector3 _pos)
	{
		Vector3i vector3i = World.worldToBlockPos(_pos);
		Chunk chunk = (Chunk)this.GetChunkFromWorldPos(vector3i);
		return chunk != null && chunk.CanMobsSpawnAtPos(World.toBlockXZ(vector3i.x), World.toBlockY(vector3i.y), World.toBlockXZ(vector3i.z), false, true);
	}

	// Token: 0x0600525B RID: 21083 RVA: 0x0021279C File Offset: 0x0021099C
	public bool CanSleeperSpawnAtPos(Vector3 _pos, bool _checkBelow)
	{
		Vector3i vector3i = World.worldToBlockPos(_pos);
		Chunk chunk = (Chunk)this.GetChunkFromWorldPos(vector3i);
		return chunk != null && chunk.CanSleeperSpawnAtPos(World.toBlockXZ(vector3i.x), World.toBlockY(vector3i.y), World.toBlockXZ(vector3i.z), _checkBelow);
	}

	// Token: 0x0600525C RID: 21084 RVA: 0x002127EC File Offset: 0x002109EC
	public bool CanPlayersSpawnAtPos(Vector3 _pos, bool _bAllowToSpawnOnAirPos = false)
	{
		Vector3i vector3i = World.worldToBlockPos(_pos);
		Chunk chunk = (Chunk)this.GetChunkFromWorldPos(vector3i);
		return chunk != null && chunk.CanPlayersSpawnAtPos(World.toBlockXZ(vector3i.x), World.toBlockY(vector3i.y), World.toBlockXZ(vector3i.z), _bAllowToSpawnOnAirPos);
	}

	// Token: 0x0600525D RID: 21085 RVA: 0x0021283C File Offset: 0x00210A3C
	public void CheckEntityCollisionWithBlocks(Entity _entity)
	{
		if (!_entity.CanCollideWithBlocks())
		{
			return;
		}
		for (int i = 0; i < this.ChunkClusters.Count; i++)
		{
			ChunkCluster chunkCluster = this.ChunkClusters[i];
			if (chunkCluster != null && chunkCluster.Overlaps(_entity.boundingBox))
			{
				chunkCluster.CheckCollisionWithBlocks(_entity);
			}
		}
	}

	// Token: 0x0600525E RID: 21086 RVA: 0x00212890 File Offset: 0x00210A90
	public void OnChunkAdded(Chunk _c)
	{
		List<long> obj = this.newlyLoadedChunksThisUpdate;
		lock (obj)
		{
			this.newlyLoadedChunksThisUpdate.Add(_c.Key);
		}
	}

	// Token: 0x0600525F RID: 21087 RVA: 0x002128DC File Offset: 0x00210ADC
	public void OnChunkBeforeRemove(Chunk _c)
	{
		List<long> obj = this.newlyLoadedChunksThisUpdate;
		lock (obj)
		{
			this.newlyLoadedChunksThisUpdate.Remove(_c.Key);
		}
		if (this.worldBlockTicker != null)
		{
			this.worldBlockTicker.OnChunkRemoved(_c);
		}
		GameManager.Instance.prefabLODManager.TriggerUpdate();
	}

	// Token: 0x06005260 RID: 21088 RVA: 0x0021294C File Offset: 0x00210B4C
	public void OnChunkBeforeSave(Chunk _c)
	{
		if (this.worldBlockTicker != null)
		{
			this.worldBlockTicker.OnChunkBeforeSave(_c);
		}
	}

	// Token: 0x06005261 RID: 21089 RVA: 0x00212964 File Offset: 0x00210B64
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateChunkAddedRemovedCallbacks()
	{
		List<long> obj = this.newlyLoadedChunksThisUpdate;
		lock (obj)
		{
			int num = 0;
			ChunkCluster chunkCluster = this.ChunkClusters[num];
			for (int i = this.newlyLoadedChunksThisUpdate.Count - 1; i >= 0; i--)
			{
				long key = this.newlyLoadedChunksThisUpdate[i];
				int num2 = WorldChunkCache.extractClrIdx(key);
				if (num2 != num)
				{
					num = num2;
					chunkCluster = this.ChunkClusters[num];
				}
				if (chunkCluster != null)
				{
					Chunk chunkSync = chunkCluster.GetChunkSync(key);
					if (chunkSync != null && !chunkSync.NeedsDecoration)
					{
						chunkSync.OnLoad(this);
						if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
						{
							this.worldBlockTicker.OnChunkAdded(this, chunkSync, this.rand);
						}
						this.newlyLoadedChunksThisUpdate.RemoveAt(i);
					}
				}
			}
		}
	}

	// Token: 0x06005262 RID: 21090 RVA: 0x00212A50 File Offset: 0x00210C50
	public override ulong GetWorldTime()
	{
		return this.worldTime;
	}

	// Token: 0x06005263 RID: 21091 RVA: 0x00212A58 File Offset: 0x00210C58
	public override WorldCreationData GetWorldCreationData()
	{
		return this.wcd;
	}

	// Token: 0x06005264 RID: 21092 RVA: 0x00212A60 File Offset: 0x00210C60
	public bool IsEntityInRange(int _entityId, int _refEntity, int _range)
	{
		Entity entity;
		Entity other;
		return _entityId == _refEntity || (this.Entities.dict.TryGetValue(_entityId, out entity) && this.Entities.dict.TryGetValue(_refEntity, out other) && entity.GetDistanceSq(other) <= (float)(_range * _range));
	}

	// Token: 0x06005265 RID: 21093 RVA: 0x00212AB0 File Offset: 0x00210CB0
	public bool IsEntityInRange(int _entityId, Vector3 _position, int _range)
	{
		Entity entity;
		return this.Entities.dict.TryGetValue(_entityId, out entity) && entity.GetDistanceSq(_position) <= (float)(_range * _range);
	}

	// Token: 0x06005266 RID: 21094 RVA: 0x00212AE4 File Offset: 0x00210CE4
	public bool IsPositionInBounds(Vector3 position)
	{
		Vector3i vector3i;
		Vector3i vector3i2;
		this.GetWorldExtent(out vector3i, out vector3i2);
		if (GamePrefs.GetString(EnumGamePrefs.GameWorld) == "Navezgane")
		{
			vector3i = new Vector3i(-2400, vector3i.y, -2400);
			vector3i2 = new Vector3i(2400, vector3i2.y, 2400);
		}
		else if (!GameUtils.IsPlaytesting())
		{
			vector3i = new Vector3i(vector3i.x + 320, vector3i.y, vector3i.z + 320);
			vector3i2 = new Vector3i(vector3i2.x - 320, vector3i2.y, vector3i2.z - 320);
		}
		Vector3Int vector3Int = vector3i;
		Vector3Int a = vector3i2;
		BoundsInt boundsInt = new BoundsInt(vector3Int, a - vector3Int);
		return boundsInt.Contains(Vector3Int.RoundToInt(position));
	}

	// Token: 0x06005267 RID: 21095 RVA: 0x00212BBC File Offset: 0x00210DBC
	public float InBoundsForPlayersPercent(Vector3 _pos)
	{
		Vector3i vector3i;
		Vector3i vector3i2;
		this.GetWorldExtent(out vector3i, out vector3i2);
		if (vector3i2.x - vector3i.x < 1024)
		{
			return 1f;
		}
		Vector2 vector;
		vector.x = (float)(vector3i.x + vector3i2.x) * 0.5f;
		vector.y = (float)(vector3i.z + vector3i2.z) * 0.5f;
		float num;
		if (_pos.x < vector.x)
		{
			num = (_pos.x - ((float)vector3i.x + 50f)) / 80f;
		}
		else
		{
			num = ((float)vector3i2.x - 50f - _pos.x) / 80f;
		}
		num = Utils.FastClamp01(num);
		float num2;
		if (_pos.z < vector.y)
		{
			num2 = (_pos.z - ((float)vector3i.z + 50f)) / 80f;
		}
		else
		{
			num2 = ((float)vector3i2.z - 50f - _pos.z) / 80f;
		}
		num2 = Utils.FastClamp01(num2);
		return Utils.FastMin(num, num2);
	}

	// Token: 0x06005268 RID: 21096 RVA: 0x00212CCC File Offset: 0x00210ECC
	public bool AdjustBoundsForPlayers(ref Vector3 _pos, float _padPercent)
	{
		Vector3i vector3i;
		Vector3i vector3i2;
		this.GetWorldExtent(out vector3i, out vector3i2);
		if (vector3i2.x - vector3i.x < 1024)
		{
			return false;
		}
		if (vector3i2.x == 0)
		{
			return false;
		}
		int num = (int)(50f + 80f * _padPercent);
		vector3i.x += num;
		vector3i.z += num;
		vector3i2.x -= num;
		vector3i2.z -= num;
		bool result = false;
		if (_pos.x < (float)vector3i.x)
		{
			_pos.x = (float)vector3i.x;
			result = true;
		}
		else if (_pos.x > (float)vector3i2.x)
		{
			_pos.x = (float)vector3i2.x;
			result = true;
		}
		if (_pos.z < (float)vector3i.z)
		{
			_pos.z = (float)vector3i.z;
			result = true;
		}
		else if (_pos.z > (float)vector3i2.z)
		{
			_pos.z = (float)vector3i2.z;
			result = true;
		}
		return result;
	}

	// Token: 0x06005269 RID: 21097 RVA: 0x00212DC4 File Offset: 0x00210FC4
	public bool IsPositionRadiated(Vector3 position)
	{
		IChunkProvider chunkProvider = this.ChunkCache.ChunkProvider;
		IBiomeProvider biomeProvider;
		return chunkProvider != null && (biomeProvider = chunkProvider.GetBiomeProvider()) != null && biomeProvider.GetRadiationAt((int)position.x, (int)position.z) > 0f;
	}

	// Token: 0x0600526A RID: 21098 RVA: 0x00212E07 File Offset: 0x00211007
	public bool IsPositionWithinPOI(Vector3 position, int offset)
	{
		return this.ChunkCache.ChunkProvider.GetDynamicPrefabDecorator().GetPrefabFromWorldPosInsideWithOffset((int)position.x, (int)position.z, offset) != null;
	}

	// Token: 0x0600526B RID: 21099 RVA: 0x00212E30 File Offset: 0x00211030
	public PrefabInstance GetPOIAtPosition(Vector3 _position, bool _checkTags = true)
	{
		DynamicPrefabDecorator dynamicPrefabDecorator = this.ChunkCache.ChunkProvider.GetDynamicPrefabDecorator();
		if (dynamicPrefabDecorator == null)
		{
			return null;
		}
		return dynamicPrefabDecorator.GetPrefabAtPosition(_position, _checkTags);
	}

	// Token: 0x0600526C RID: 21100 RVA: 0x00212E5C File Offset: 0x0021105C
	public void GetPOIsAtXZ(int _xMin, int _xMax, int _zMin, int _zMax, List<PrefabInstance> _list)
	{
		DynamicPrefabDecorator dynamicPrefabDecorator = this.ChunkCache.ChunkProvider.GetDynamicPrefabDecorator();
		if (dynamicPrefabDecorator != null)
		{
			dynamicPrefabDecorator.GetPrefabsAtXZ(_xMin, _xMax, _zMin, _zMax, _list);
		}
	}

	// Token: 0x0600526D RID: 21101 RVA: 0x00212E8C File Offset: 0x0021108C
	public Vector3 ClampToValidWorldPos(Vector3 position)
	{
		Vector3i vector3i;
		Vector3i vector3i2;
		this.GetWorldExtent(out vector3i, out vector3i2);
		if (GamePrefs.GetString(EnumGamePrefs.GameWorld) == "Navezgane")
		{
			vector3i = new Vector3i(-2400, vector3i.y, -2400);
			vector3i2 = new Vector3i(2400, vector3i2.y, 2400);
		}
		else if (!GameUtils.IsPlaytesting())
		{
			vector3i = new Vector3i(vector3i.x + 320, vector3i.y, vector3i.z + 320);
			vector3i2 = new Vector3i(vector3i2.x - 320, vector3i2.y, vector3i2.z - 320);
		}
		float x = Mathf.Clamp(position.x, (float)vector3i.x, (float)vector3i2.x);
		float y = Mathf.Clamp(position.y, (float)vector3i.y, (float)vector3i2.y);
		float z = Mathf.Clamp(position.z, (float)vector3i.z, (float)vector3i2.z);
		return new Vector3(x, y, z);
	}

	// Token: 0x0600526E RID: 21102 RVA: 0x00212F8C File Offset: 0x0021118C
	public Vector3 ClampToValidWorldPosForMap(Vector2 position)
	{
		Vector3i vector3i;
		Vector3i vector3i2;
		this.GetWorldExtent(out vector3i, out vector3i2);
		if (GamePrefs.GetString(EnumGamePrefs.GameWorld) == "Navezgane")
		{
			vector3i = new Vector3i(-2550, vector3i.y, -2550);
			vector3i2 = new Vector3i(2550, vector3i2.y, 2550);
		}
		float x = Mathf.Clamp(position.x, (float)vector3i.x, (float)vector3i2.x);
		float y = Mathf.Clamp(position.y, (float)vector3i.z, (float)vector3i2.z);
		return new Vector2(x, y);
	}

	// Token: 0x0600526F RID: 21103 RVA: 0x00213023 File Offset: 0x00211223
	public void ObjectOnMapAdd(MapObject _mo)
	{
		if (this.objectsOnMap != null)
		{
			this.objectsOnMap.Add(_mo);
		}
	}

	// Token: 0x06005270 RID: 21104 RVA: 0x00213039 File Offset: 0x00211239
	public void ObjectOnMapRemove(EnumMapObjectType _type, int _key)
	{
		if (this.objectsOnMap != null)
		{
			this.objectsOnMap.Remove(_type, _key);
		}
	}

	// Token: 0x06005271 RID: 21105 RVA: 0x00213050 File Offset: 0x00211250
	public void ObjectOnMapRemove(EnumMapObjectType _type, Vector3 _position)
	{
		if (this.objectsOnMap != null)
		{
			this.objectsOnMap.RemoveByPosition(_type, _position);
		}
	}

	// Token: 0x06005272 RID: 21106 RVA: 0x00213067 File Offset: 0x00211267
	public void ObjectOnMapRemove(EnumMapObjectType _type)
	{
		if (this.objectsOnMap != null)
		{
			this.objectsOnMap.RemoveByType(_type);
		}
	}

	// Token: 0x06005273 RID: 21107 RVA: 0x0021307D File Offset: 0x0021127D
	public List<MapObject> GetObjectOnMapList(EnumMapObjectType _type)
	{
		if (this.objectsOnMap != null)
		{
			return this.objectsOnMap.GetList(_type);
		}
		return new List<MapObject>();
	}

	// Token: 0x06005274 RID: 21108 RVA: 0x0021309C File Offset: 0x0021129C
	public void DebugAddSpawnedEntity(Entity entity)
	{
		if (this.GetPrimaryPlayer() == null || !(entity is EntityAlive))
		{
			return;
		}
		EntityAlive entityAlive = (EntityAlive)entity;
		SSpawnedEntity item = default(SSpawnedEntity);
		item.distanceToLocalPlayer = (entityAlive.GetPosition() - this.GetPrimaryPlayer().GetPosition()).magnitude;
		item.name = entityAlive.EntityName;
		item.pos = entityAlive.GetPosition();
		item.timeSpawned = Time.time;
		this.Last4Spawned.Add(item);
		if (this.Last4Spawned.Count > 4)
		{
			this.Last4Spawned.RemoveAt(0);
		}
	}

	// Token: 0x06005275 RID: 21109 RVA: 0x00213140 File Offset: 0x00211340
	public static void SetWorldAreas(List<TraderArea> _traders)
	{
		World.traderAreas = _traders;
	}

	// Token: 0x06005276 RID: 21110 RVA: 0x00213148 File Offset: 0x00211348
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupTraders()
	{
		if (World.traderAreas != null)
		{
			DynamicPrefabDecorator dynamicPrefabDecorator = this.ChunkCache.ChunkProvider.GetDynamicPrefabDecorator();
			if (dynamicPrefabDecorator != null)
			{
				dynamicPrefabDecorator.ClearTraders();
				for (int i = 0; i < World.traderAreas.Count; i++)
				{
					dynamicPrefabDecorator.AddTrader(World.traderAreas[i]);
				}
			}
			World.traderAreas = null;
		}
	}

	// Token: 0x1700084C RID: 2124
	// (get) Token: 0x06005277 RID: 21111 RVA: 0x002131A4 File Offset: 0x002113A4
	public List<TraderArea> TraderAreas
	{
		get
		{
			DynamicPrefabDecorator dynamicPrefabDecorator = this.ChunkCache.ChunkProvider.GetDynamicPrefabDecorator();
			if (dynamicPrefabDecorator == null)
			{
				return null;
			}
			return dynamicPrefabDecorator.GetTraderAreas();
		}
	}

	// Token: 0x06005278 RID: 21112 RVA: 0x002131CD File Offset: 0x002113CD
	public bool IsWithinTraderArea(Vector3i _worldBlockPos)
	{
		return this.GetTraderAreaAt(_worldBlockPos) != null;
	}

	// Token: 0x06005279 RID: 21113 RVA: 0x002131DC File Offset: 0x002113DC
	public bool IsWithinTraderPlacingProtection(Vector3i _worldBlockPos)
	{
		DynamicPrefabDecorator dynamicPrefabDecorator = this.ChunkCache.ChunkProvider.GetDynamicPrefabDecorator();
		return dynamicPrefabDecorator != null && dynamicPrefabDecorator.GetTraderAtPosition(_worldBlockPos, 2) != null;
	}

	// Token: 0x0600527A RID: 21114 RVA: 0x0021320C File Offset: 0x0021140C
	public bool IsWithinTraderPlacingProtection(Bounds _bounds)
	{
		DynamicPrefabDecorator dynamicPrefabDecorator = this.ChunkCache.ChunkProvider.GetDynamicPrefabDecorator();
		if (dynamicPrefabDecorator != null)
		{
			_bounds.Expand(4f);
			Vector3i minPos = World.worldToBlockPos(_bounds.min);
			Vector3i maxPos = World.worldToBlockPos(_bounds.max);
			return dynamicPrefabDecorator.IsWithinTraderArea(minPos, maxPos);
		}
		return false;
	}

	// Token: 0x0600527B RID: 21115 RVA: 0x00213260 File Offset: 0x00211460
	public bool IsWithinTraderArea(Vector3i _minPos, Vector3i _maxPos)
	{
		DynamicPrefabDecorator dynamicPrefabDecorator = this.ChunkCache.ChunkProvider.GetDynamicPrefabDecorator();
		return dynamicPrefabDecorator != null && dynamicPrefabDecorator.IsWithinTraderArea(_minPos, _maxPos);
	}

	// Token: 0x0600527C RID: 21116 RVA: 0x0021328C File Offset: 0x0021148C
	public TraderArea GetTraderAreaAt(Vector3i _pos)
	{
		DynamicPrefabDecorator dynamicPrefabDecorator = this.ChunkCache.ChunkProvider.GetDynamicPrefabDecorator();
		if (dynamicPrefabDecorator != null)
		{
			return dynamicPrefabDecorator.GetTraderAtPosition(_pos, 0);
		}
		return null;
	}

	// Token: 0x0600527D RID: 21117 RVA: 0x002132B8 File Offset: 0x002114B8
	public override int AddSleeperVolume(SleeperVolume _sleeperVolume)
	{
		List<SleeperVolume> obj = this.sleeperVolumes;
		int result;
		lock (obj)
		{
			this.sleeperVolumes.Add(_sleeperVolume);
			List<int> list;
			if (!this.sleeperVolumeMap.TryGetValue(_sleeperVolume.BoxMin, out list))
			{
				list = new List<int>();
				this.sleeperVolumeMap.Add(_sleeperVolume.BoxMin, list);
			}
			list.Add(this.sleeperVolumes.Count - 1);
			result = this.sleeperVolumes.Count - 1;
		}
		return result;
	}

	// Token: 0x0600527E RID: 21118 RVA: 0x00213350 File Offset: 0x00211550
	public override int FindSleeperVolume(Vector3i mins, Vector3i maxs)
	{
		List<int> list;
		if (this.sleeperVolumeMap.TryGetValue(mins, out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				int num = list[i];
				if (this.sleeperVolumes[num].BoxMax == maxs)
				{
					return num;
				}
			}
		}
		return -1;
	}

	// Token: 0x0600527F RID: 21119 RVA: 0x002133A2 File Offset: 0x002115A2
	public override int GetSleeperVolumeCount()
	{
		return this.sleeperVolumes.Count;
	}

	// Token: 0x06005280 RID: 21120 RVA: 0x002133AF File Offset: 0x002115AF
	public override SleeperVolume GetSleeperVolume(int index)
	{
		return this.sleeperVolumes[index];
	}

	// Token: 0x06005281 RID: 21121 RVA: 0x002133C0 File Offset: 0x002115C0
	public void CheckSleeperVolumeTouching(EntityPlayer _player)
	{
		if (!GameStats.GetBool(EnumGameStats.IsSpawnEnemies))
		{
			return;
		}
		Vector3i blockPosition = _player.GetBlockPosition();
		Chunk chunk = (Chunk)this.GetChunkFromWorldPos(blockPosition);
		if (chunk != null)
		{
			List<int> list = chunk.GetSleeperVolumes();
			List<SleeperVolume> obj = this.sleeperVolumes;
			lock (obj)
			{
				for (int i = 0; i < list.Count; i++)
				{
					int num = list[i];
					if (num < this.sleeperVolumes.Count)
					{
						this.sleeperVolumes[num].CheckTouching(this, _player);
					}
				}
			}
		}
	}

	// Token: 0x06005282 RID: 21122 RVA: 0x00213468 File Offset: 0x00211668
	public void CheckSleeperVolumeNoise(Vector3 position)
	{
		if (!GameStats.GetBool(EnumGameStats.IsSpawnEnemies))
		{
			return;
		}
		position.y += 0.1f;
		Chunk chunk = (Chunk)this.GetChunkFromWorldPos(World.worldToBlockPos(position));
		if (chunk != null)
		{
			List<int> list = chunk.GetSleeperVolumes();
			List<SleeperVolume> obj = this.sleeperVolumes;
			lock (obj)
			{
				for (int i = 0; i < list.Count; i++)
				{
					int num = list[i];
					if (num < this.sleeperVolumes.Count)
					{
						this.sleeperVolumes[num].CheckNoise(this, position);
					}
				}
			}
		}
	}

	// Token: 0x06005283 RID: 21123 RVA: 0x0021351C File Offset: 0x0021171C
	public void WriteSleeperVolumes(BinaryWriter _bw)
	{
		_bw.Write(this.sleeperVolumes.Count);
		for (int i = 0; i < this.sleeperVolumes.Count; i++)
		{
			this.sleeperVolumes[i].Write(_bw);
		}
	}

	// Token: 0x06005284 RID: 21124 RVA: 0x00213564 File Offset: 0x00211764
	[PublicizedFrom(EAccessModifier.Private)]
	public void ReadSleeperVolumes(BinaryReader _br)
	{
		this.sleeperVolumes.Clear();
		this.sleeperVolumeMap.Clear();
		int num = _br.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			SleeperVolume sleeperVolume = SleeperVolume.Read(_br);
			this.sleeperVolumes.Add(sleeperVolume);
			List<int> list;
			if (!this.sleeperVolumeMap.TryGetValue(sleeperVolume.BoxMin, out list))
			{
				list = new List<int>();
				this.sleeperVolumeMap.Add(sleeperVolume.BoxMin, list);
			}
			list.Add(this.sleeperVolumes.Count - 1);
		}
	}

	// Token: 0x06005285 RID: 21125 RVA: 0x002135F0 File Offset: 0x002117F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupSleeperVolumes()
	{
		for (int i = 0; i < this.sleeperVolumes.Count; i++)
		{
			this.sleeperVolumes[i].AddToPrefabInstance();
		}
	}

	// Token: 0x06005286 RID: 21126 RVA: 0x00213624 File Offset: 0x00211824
	public void NotifySleeperVolumesEntityDied(EntityAlive entity)
	{
		List<SleeperVolume> obj = this.sleeperVolumes;
		lock (obj)
		{
			for (int i = 0; i < this.sleeperVolumes.Count; i++)
			{
				this.sleeperVolumes[i].EntityDied(entity);
			}
		}
	}

	// Token: 0x06005287 RID: 21127 RVA: 0x00213688 File Offset: 0x00211888
	[PublicizedFrom(EAccessModifier.Private)]
	public void TickSleeperVolumes()
	{
		if (!GameStats.GetBool(EnumGameStats.IsSpawnEnemies))
		{
			return;
		}
		List<SleeperVolume> obj = this.sleeperVolumes;
		lock (obj)
		{
			SleeperVolume.TickSpawnCount = 0;
			for (int i = 0; i < this.sleeperVolumes.Count; i++)
			{
				this.sleeperVolumes[i].Tick(this);
			}
		}
	}

	// Token: 0x06005288 RID: 21128 RVA: 0x002136FC File Offset: 0x002118FC
	public override int AddTriggerVolume(TriggerVolume _triggerVolume)
	{
		List<TriggerVolume> obj = this.triggerVolumes;
		int result;
		lock (obj)
		{
			this.triggerVolumes.Add(_triggerVolume);
			List<int> list;
			if (!this.triggerVolumeMap.TryGetValue(_triggerVolume.BoxMin, out list))
			{
				list = new List<int>();
				this.triggerVolumeMap.Add(_triggerVolume.BoxMin, list);
			}
			list.Add(this.triggerVolumes.Count - 1);
			result = this.triggerVolumes.Count - 1;
		}
		return result;
	}

	// Token: 0x06005289 RID: 21129 RVA: 0x00213794 File Offset: 0x00211994
	public override void ResetTriggerVolumes(long chunkKey)
	{
		Vector2i vector2i = WorldChunkCache.extractXZ(chunkKey);
		Bounds bounds = Chunk.CalculateAABB(vector2i.x, 0, vector2i.y);
		foreach (TriggerVolume triggerVolume in this.triggerVolumes)
		{
			if (triggerVolume.Intersects(bounds))
			{
				triggerVolume.Reset();
			}
		}
	}

	// Token: 0x0600528A RID: 21130 RVA: 0x0021380C File Offset: 0x00211A0C
	public override void ResetSleeperVolumes(long chunkKey)
	{
		Vector2i vector2i = WorldChunkCache.extractXZ(chunkKey);
		Bounds bounds = Chunk.CalculateAABB(vector2i.x, 0, vector2i.y);
		foreach (SleeperVolume sleeperVolume in this.sleeperVolumes)
		{
			if (sleeperVolume.Intersects(bounds))
			{
				sleeperVolume.DespawnAndReset(this);
			}
		}
	}

	// Token: 0x0600528B RID: 21131 RVA: 0x00213884 File Offset: 0x00211A84
	public override int FindTriggerVolume(Vector3i mins, Vector3i maxs)
	{
		List<int> list;
		if (this.triggerVolumeMap.TryGetValue(mins, out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				int num = list[i];
				if (this.triggerVolumes[num].BoxMax == maxs)
				{
					return num;
				}
			}
		}
		return -1;
	}

	// Token: 0x0600528C RID: 21132 RVA: 0x002138D6 File Offset: 0x00211AD6
	public override int GetTriggerVolumeCount()
	{
		return this.triggerVolumes.Count;
	}

	// Token: 0x0600528D RID: 21133 RVA: 0x002138E3 File Offset: 0x00211AE3
	public override TriggerVolume GetTriggerVolume(int index)
	{
		return this.triggerVolumes[index];
	}

	// Token: 0x0600528E RID: 21134 RVA: 0x002138F4 File Offset: 0x00211AF4
	public void CheckTriggerVolumeTrigger(EntityPlayer _player)
	{
		Vector3i blockPosition = _player.GetBlockPosition();
		Chunk chunk = (Chunk)this.GetChunkFromWorldPos(blockPosition);
		if (chunk != null)
		{
			List<int> list = chunk.GetTriggerVolumes();
			List<TriggerVolume> obj = this.triggerVolumes;
			lock (obj)
			{
				for (int i = 0; i < list.Count; i++)
				{
					int num = list[i];
					if (num < this.triggerVolumes.Count)
					{
						this.triggerVolumes[num].CheckTouching(this, _player);
					}
				}
			}
		}
	}

	// Token: 0x0600528F RID: 21135 RVA: 0x00213990 File Offset: 0x00211B90
	public void WriteTriggerVolumes(BinaryWriter _bw)
	{
		_bw.Write(this.triggerVolumes.Count);
		for (int i = 0; i < this.triggerVolumes.Count; i++)
		{
			this.triggerVolumes[i].Write(_bw);
		}
	}

	// Token: 0x06005290 RID: 21136 RVA: 0x002139D8 File Offset: 0x00211BD8
	[PublicizedFrom(EAccessModifier.Private)]
	public void ReadTriggerVolumes(BinaryReader _br)
	{
		this.triggerVolumes.Clear();
		this.triggerVolumeMap.Clear();
		int num = _br.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			TriggerVolume triggerVolume = TriggerVolume.Read(_br);
			this.triggerVolumes.Add(triggerVolume);
			List<int> list;
			if (!this.triggerVolumeMap.TryGetValue(triggerVolume.BoxMin, out list))
			{
				list = new List<int>();
				this.triggerVolumeMap.Add(triggerVolume.BoxMin, list);
			}
			list.Add(this.triggerVolumes.Count - 1);
		}
	}

	// Token: 0x06005291 RID: 21137 RVA: 0x00213A64 File Offset: 0x00211C64
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupTriggerVolumes()
	{
		for (int i = 0; i < this.triggerVolumes.Count; i++)
		{
			this.triggerVolumes[i].AddToPrefabInstance();
		}
	}

	// Token: 0x06005292 RID: 21138 RVA: 0x00213A98 File Offset: 0x00211C98
	public override int AddWallVolume(WallVolume _wallVolume)
	{
		List<WallVolume> obj = this.wallVolumes;
		int result;
		lock (obj)
		{
			this.wallVolumes.Add(_wallVolume);
			List<int> list;
			if (!this.wallVolumeMap.TryGetValue(_wallVolume.BoxMin, out list))
			{
				list = new List<int>();
				this.wallVolumeMap.Add(_wallVolume.BoxMin, list);
			}
			list.Add(this.wallVolumes.Count - 1);
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				NetPackageWallVolume package = NetPackageManager.GetPackage<NetPackageWallVolume>();
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package.Setup(_wallVolume), false, -1, -1, -1, null, 192, false);
			}
			result = this.wallVolumes.Count - 1;
		}
		return result;
	}

	// Token: 0x06005293 RID: 21139 RVA: 0x00213B68 File Offset: 0x00211D68
	public override int FindWallVolume(Vector3i mins, Vector3i maxs)
	{
		List<int> list;
		if (this.wallVolumeMap.TryGetValue(mins, out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				int num = list[i];
				if (this.wallVolumes[num].BoxMax == maxs)
				{
					return num;
				}
			}
		}
		return -1;
	}

	// Token: 0x06005294 RID: 21140 RVA: 0x00213BBA File Offset: 0x00211DBA
	public override int GetWallVolumeCount()
	{
		return this.wallVolumes.Count;
	}

	// Token: 0x06005295 RID: 21141 RVA: 0x00213BC8 File Offset: 0x00211DC8
	public override WallVolume GetWallVolume(int index)
	{
		if (index >= this.wallVolumes.Count)
		{
			Debug.LogWarning(string.Format("Wall Volume Error: Index {0} | wallVolumeCount: {1}", index, this.wallVolumes.Count));
		}
		return this.wallVolumes[index];
	}

	// Token: 0x06005296 RID: 21142 RVA: 0x00213C14 File Offset: 0x00211E14
	public override List<WallVolume> GetAllWallVolumes()
	{
		return this.wallVolumes;
	}

	// Token: 0x06005297 RID: 21143 RVA: 0x00213C1C File Offset: 0x00211E1C
	public void WriteWallVolumes(BinaryWriter _bw)
	{
		_bw.Write(this.wallVolumes.Count);
		for (int i = 0; i < this.wallVolumes.Count; i++)
		{
			this.wallVolumes[i].Write(_bw);
		}
	}

	// Token: 0x06005298 RID: 21144 RVA: 0x00213C64 File Offset: 0x00211E64
	[PublicizedFrom(EAccessModifier.Private)]
	public void ReadWallVolumes(BinaryReader _br)
	{
		this.wallVolumes.Clear();
		this.wallVolumeMap.Clear();
		int num = _br.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			WallVolume wallVolume = WallVolume.Read(_br);
			this.wallVolumes.Add(wallVolume);
			List<int> list;
			if (!this.wallVolumeMap.TryGetValue(wallVolume.BoxMin, out list))
			{
				list = new List<int>();
				this.wallVolumeMap.Add(wallVolume.BoxMin, list);
			}
			list.Add(this.wallVolumes.Count - 1);
		}
	}

	// Token: 0x06005299 RID: 21145 RVA: 0x00213CF0 File Offset: 0x00211EF0
	public void SetWallVolumesForClient(List<WallVolume> wallVolumeData)
	{
		this.wallVolumes.Clear();
		this.wallVolumeMap.Clear();
		foreach (WallVolume wallVolume in wallVolumeData)
		{
			this.wallVolumes.Add(wallVolume);
			List<int> list;
			if (!this.wallVolumeMap.TryGetValue(wallVolume.BoxMin, out list))
			{
				list = new List<int>();
				this.wallVolumeMap.Add(wallVolume.BoxMin, list);
			}
			list.Add(this.wallVolumes.Count - 1);
		}
	}

	// Token: 0x0600529A RID: 21146 RVA: 0x00213D9C File Offset: 0x00211F9C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupWallVolumes()
	{
		for (int i = 0; i < this.wallVolumes.Count; i++)
		{
			this.wallVolumes[i].AddToPrefabInstance();
		}
	}

	// Token: 0x0600529B RID: 21147 RVA: 0x00213DD0 File Offset: 0x00211FD0
	public void AddBlockData(Vector3i v3i, object bd)
	{
		this.blockData.Add(v3i, bd);
	}

	// Token: 0x0600529C RID: 21148 RVA: 0x00213DE0 File Offset: 0x00211FE0
	public object GetBlockData(Vector3i v3i)
	{
		object result;
		if (!this.blockData.TryGetValue(v3i, out result))
		{
			result = null;
		}
		return result;
	}

	// Token: 0x0600529D RID: 21149 RVA: 0x00213E00 File Offset: 0x00212000
	public void ClearBlockData(Vector3i v3i)
	{
		this.blockData.Remove(v3i);
	}

	// Token: 0x0600529E RID: 21150 RVA: 0x00213E0F File Offset: 0x0021200F
	public void RebuildTerrain(HashSetLong _chunks, Vector3i _areaStart, Vector3i _areaSize, bool _bStopStabilityUpdate, bool _bRegenerateChunk, bool _bFillEmptyBlocks, bool _isReset = false)
	{
		this.ChunkCache.ChunkProvider.RebuildTerrain(_chunks, _areaStart, _areaSize, _bStopStabilityUpdate, _bRegenerateChunk, _bFillEmptyBlocks, _isReset);
	}

	// Token: 0x0600529F RID: 21151 RVA: 0x00213E2C File Offset: 0x0021202C
	public override GameRandom GetGameRandom()
	{
		return this.rand;
	}

	// Token: 0x060052A0 RID: 21152 RVA: 0x00213E34 File Offset: 0x00212034
	public float RandomRange(float _min, float _max)
	{
		return this.rand.RandomFloat * (_max - _min) + _min;
	}

	// Token: 0x060052A1 RID: 21153 RVA: 0x00213E48 File Offset: 0x00212048
	[PublicizedFrom(EAccessModifier.Private)]
	public void DuskDawnInit()
	{
		ValueTuple<int, int> valueTuple = GameUtils.CalcDuskDawnHours(GameStats.GetInt(EnumGameStats.DayLightLength));
		this.DuskHour = valueTuple.Item1;
		this.DawnHour = valueTuple.Item2;
	}

	// Token: 0x060052A2 RID: 21154 RVA: 0x00213E7A File Offset: 0x0021207A
	public void SetTime(ulong _time)
	{
		this.worldTime = _time;
		if (this.m_WorldEnvironment)
		{
			this.m_WorldEnvironment.WorldTimeChanged();
		}
	}

	// Token: 0x060052A3 RID: 21155 RVA: 0x00213E9B File Offset: 0x0021209B
	public void SetTimeJump(ulong _time, bool _isSeek = false)
	{
		this.SetTime(_time);
		SkyManager.bUpdateSunMoonNow = true;
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.aiDirector.BloodMoonComponent.TimeChanged(_isSeek);
		}
	}

	// Token: 0x060052A4 RID: 21156 RVA: 0x00213EC7 File Offset: 0x002120C7
	public bool IsWorldEvent(World.WorldEvent _event)
	{
		return _event == World.WorldEvent.BloodMoon && this.isEventBloodMoon;
	}

	// Token: 0x060052A5 RID: 21157 RVA: 0x00213ED4 File Offset: 0x002120D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void WorldEventUpdateTime()
	{
		this.WorldDay = GameUtils.WorldTimeToDays(this.worldTime);
		this.WorldHour = GameUtils.WorldTimeToHours(this.worldTime);
		int num = GameUtils.WorldTimeToDays(this.eventWorldTime);
		int num2 = GameUtils.WorldTimeToHours(this.eventWorldTime);
		if (num == this.WorldDay && num2 == this.WorldHour)
		{
			return;
		}
		this.eventWorldTime = this.worldTime;
		ValueTuple<int, int> valueTuple = GameUtils.CalcDuskDawnHours(GameStats.GetInt(EnumGameStats.DayLightLength));
		int item = valueTuple.Item1;
		int item2 = valueTuple.Item2;
		bool flag = this.isEventBloodMoon;
		this.isEventBloodMoon = false;
		int @int = GameStats.GetInt(EnumGameStats.BloodMoonDay);
		if (this.WorldDay == @int)
		{
			if (this.WorldHour >= item)
			{
				this.isEventBloodMoon = true;
			}
		}
		else if (this.WorldDay > 1 && this.WorldDay == @int + 1 && this.WorldHour < item2)
		{
			this.isEventBloodMoon = true;
		}
		if (flag != this.isEventBloodMoon)
		{
			EntityPlayerLocal primaryPlayer = this.GetPrimaryPlayer();
			if (primaryPlayer)
			{
				if (this.isEventBloodMoon && this.WorldHour == item)
				{
					primaryPlayer.BloodMoonParticipation = true;
					return;
				}
				if (!this.isEventBloodMoon && this.WorldHour == item2 && primaryPlayer.BloodMoonParticipation)
				{
					QuestEventManager.Current.BloodMoonSurvived();
					primaryPlayer.BloodMoonParticipation = false;
				}
			}
		}
	}

	// Token: 0x060052A6 RID: 21158 RVA: 0x00214004 File Offset: 0x00212204
	public override void AddPendingDowngradeBlock(Vector3i _blockPos)
	{
		this.pendingUpgradeDowngradeBlocks.Add(_blockPos);
	}

	// Token: 0x060052A7 RID: 21159 RVA: 0x00214013 File Offset: 0x00212213
	public override bool TryRetrieveAndRemovePendingDowngradeBlock(Vector3i _blockPos)
	{
		if (this.pendingUpgradeDowngradeBlocks.Contains(_blockPos))
		{
			this.pendingUpgradeDowngradeBlocks.Remove(_blockPos);
			return true;
		}
		return false;
	}

	// Token: 0x060052A8 RID: 21160 RVA: 0x00214033 File Offset: 0x00212233
	public IEnumerator ResetPOIS(List<PrefabInstance> prefabInstances, FastTags<TagGroup.Global> questTags, int entityID, int[] sharedWith, QuestClass questClass)
	{
		int num;
		for (int i = 0; i < prefabInstances.Count; i = num + 1)
		{
			PrefabInstance prefabInstance = prefabInstances[i];
			yield return prefabInstance.ResetTerrain(this);
			num = i;
		}
		for (int j = 0; j < prefabInstances.Count; j++)
		{
			PrefabInstance prefabInstance2 = prefabInstances[j];
			this.triggerManager.RemoveFromUpdateList(prefabInstance2);
			prefabInstance2.LastQuestClass = questClass;
			prefabInstance2.ResetBlocksAndRebuild(this, questTags);
			for (int k = 0; k < prefabInstance2.prefab.SleeperVolumes.Count; k++)
			{
				Vector3i startPos = prefabInstance2.prefab.SleeperVolumes[k].startPos;
				Vector3i size = prefabInstance2.prefab.SleeperVolumes[k].size;
				int num2 = GameManager.Instance.World.FindSleeperVolume(prefabInstance2.boundingBoxPosition + startPos, prefabInstance2.boundingBoxPosition + startPos + size);
				if (num2 != -1)
				{
					this.GetSleeperVolume(num2).DespawnAndReset(this);
				}
			}
			for (int l = 0; l < prefabInstance2.prefab.TriggerVolumes.Count; l++)
			{
				Vector3i startPos2 = prefabInstance2.prefab.TriggerVolumes[l].startPos;
				Vector3i size2 = prefabInstance2.prefab.TriggerVolumes[l].size;
				int num3 = GameManager.Instance.World.FindTriggerVolume(prefabInstance2.boundingBoxPosition + startPos2, prefabInstance2.boundingBoxPosition + startPos2 + size2);
				if (num3 != -1)
				{
					this.GetTriggerVolume(num3).Reset();
				}
			}
			this.triggerManager.RefreshTriggers(prefabInstance2, questTags);
			if (prefabInstance2.prefab.GetQuestTag(questTags) && (prefabInstance2.lockInstance == null || prefabInstance2.lockInstance.CheckQuestLock()))
			{
				prefabInstance2.lockInstance = new QuestLockInstance(entityID);
				if (sharedWith != null)
				{
					prefabInstance2.lockInstance.AddQuesters(sharedWith);
				}
			}
		}
		bool finished = false;
		while (!finished)
		{
			int num4 = 0;
			while (num4 < prefabInstances.Count && prefabInstances[num4].bPrefabCopiedIntoWorld)
			{
				num4++;
			}
			finished = (num4 >= prefabInstances.Count);
			if (!finished)
			{
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x04003EAE RID: 16046
	public const int cCollisionBlocks = 5;

	// Token: 0x04003EB2 RID: 16050
	public ulong worldTime;

	// Token: 0x04003EB3 RID: 16051
	public int DawnHour;

	// Token: 0x04003EB4 RID: 16052
	public int DuskHour;

	// Token: 0x04003EB5 RID: 16053
	public float Gravity = 0.08f;

	// Token: 0x04003EB6 RID: 16054
	public DictionaryList<int, Entity> Entities = new DictionaryList<int, Entity>();

	// Token: 0x04003EB7 RID: 16055
	public DictionaryList<int, EntityPlayer> Players = new DictionaryList<int, EntityPlayer>();

	// Token: 0x04003EB8 RID: 16056
	public List<EntityAlive> EntityAlives = new List<EntityAlive>();

	// Token: 0x04003EB9 RID: 16057
	public NetEntityDistribution entityDistributer;

	// Token: 0x04003EBA RID: 16058
	public AIDirector aiDirector;

	// Token: 0x04003EBB RID: 16059
	public Manager audioManager;

	// Token: 0x04003EBC RID: 16060
	public Conductor dmsConductor;

	// Token: 0x04003EBD RID: 16061
	public IGameManager gameManager;

	// Token: 0x04003EBE RID: 16062
	public int Seed;

	// Token: 0x04003EBF RID: 16063
	public WorldBiomes Biomes;

	// Token: 0x04003EC0 RID: 16064
	public SpawnManagerBiomes biomeSpawnManager;

	// Token: 0x04003EC1 RID: 16065
	public BiomeIntensity LocalPlayerBiomeIntensityStandingOn = BiomeIntensity.Default;

	// Token: 0x04003EC2 RID: 16066
	public WorldCreationData wcd;

	// Token: 0x04003EC3 RID: 16067
	[PublicizedFrom(EAccessModifier.Private)]
	public WorldState worldState;

	// Token: 0x04003EC4 RID: 16068
	public ChunkManager m_ChunkManager;

	// Token: 0x04003EC5 RID: 16069
	public SharedChunkObserverCache m_SharedChunkObserverCache;

	// Token: 0x04003EC6 RID: 16070
	public WorldEnvironment m_WorldEnvironment;

	// Token: 0x04003EC7 RID: 16071
	public BiomeAtmosphereEffects BiomeAtmosphereEffects;

	// Token: 0x04003EC8 RID: 16072
	public FlatAreaManager FlatAreaManager;

	// Token: 0x04003EC9 RID: 16073
	public static bool IsSplatMapAvailable;

	// Token: 0x04003ECA RID: 16074
	public List<SSpawnedEntity> Last4Spawned = new List<SSpawnedEntity>();

	// Token: 0x04003ECB RID: 16075
	public int playerEntityUpdateCount;

	// Token: 0x04003ECC RID: 16076
	public int clientLastEntityId;

	// Token: 0x04003ECD RID: 16077
	public Transform EntitiesTransform;

	// Token: 0x04003ECE RID: 16078
	[PublicizedFrom(EAccessModifier.Private)]
	public GameRandom rand;

	// Token: 0x04003ECF RID: 16079
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isUnityTerrainConfigured;

	// Token: 0x04003ED0 RID: 16080
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityPlayerLocal> m_LocalPlayerEntities = new List<EntityPlayerLocal>();

	// Token: 0x04003ED1 RID: 16081
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal m_LocalPlayerEntity;

	// Token: 0x04003ED2 RID: 16082
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Entity> entitiesWithinAABBExcludingEntity = new List<Entity>();

	// Token: 0x04003ED3 RID: 16083
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityAlive> livingEntitiesWithinAABBExcludingEntity = new List<EntityAlive>();

	// Token: 0x04003ED4 RID: 16084
	[PublicizedFrom(EAccessModifier.Private)]
	public MapObjectManager objectsOnMap;

	// Token: 0x04003ED5 RID: 16085
	[PublicizedFrom(EAccessModifier.Private)]
	public SpawnManagerDynamic dynamicSpawnManager;

	// Token: 0x04003ED6 RID: 16086
	[PublicizedFrom(EAccessModifier.Private)]
	public WorldBlockTicker worldBlockTicker;

	// Token: 0x04003ED7 RID: 16087
	[PublicizedFrom(EAccessModifier.Private)]
	public List<SleeperVolume> sleeperVolumes = new List<SleeperVolume>();

	// Token: 0x04003ED8 RID: 16088
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<Vector3i, List<int>> sleeperVolumeMap = new Dictionary<Vector3i, List<int>>();

	// Token: 0x04003ED9 RID: 16089
	[PublicizedFrom(EAccessModifier.Private)]
	public List<TriggerVolume> triggerVolumes = new List<TriggerVolume>();

	// Token: 0x04003EDA RID: 16090
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<Vector3i, List<int>> triggerVolumeMap = new Dictionary<Vector3i, List<int>>();

	// Token: 0x04003EDB RID: 16091
	[PublicizedFrom(EAccessModifier.Private)]
	public List<WallVolume> wallVolumes = new List<WallVolume>();

	// Token: 0x04003EDC RID: 16092
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<Vector3i, List<int>> wallVolumeMap = new Dictionary<Vector3i, List<int>>();

	// Token: 0x04003EDD RID: 16093
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<Vector3i, object> blockData = new Dictionary<Vector3i, object>();

	// Token: 0x04003EDE RID: 16094
	[PublicizedFrom(EAccessModifier.Private)]
	public List<long> newlyLoadedChunksThisUpdate = new List<long>();

	// Token: 0x04003EDF RID: 16095
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<long, ulong> areaMasterChunksToLock = new Dictionary<long, ulong>();

	// Token: 0x04003EE0 RID: 16096
	public TriggerManager triggerManager;

	// Token: 0x04003EE1 RID: 16097
	[PublicizedFrom(EAccessModifier.Private)]
	public static int[][] supportOrder = new int[][]
	{
		new int[]
		{
			0,
			7,
			1,
			6,
			2,
			4,
			3,
			5
		},
		new int[]
		{
			0,
			2,
			1,
			7,
			3,
			4,
			6,
			5
		},
		new int[]
		{
			2,
			1,
			3,
			0,
			4,
			6,
			5,
			7
		},
		new int[]
		{
			2,
			4,
			3,
			1,
			5,
			6,
			0,
			7
		},
		new int[]
		{
			4,
			3,
			5,
			2,
			6,
			0,
			7,
			1
		},
		new int[]
		{
			4,
			6,
			5,
			3,
			7,
			0,
			2,
			1
		},
		new int[]
		{
			6,
			5,
			7,
			4,
			0,
			2,
			1,
			3
		},
		new int[]
		{
			6,
			0,
			7,
			5,
			1,
			2,
			4,
			3
		}
	};

	// Token: 0x04003EE2 RID: 16098
	[PublicizedFrom(EAccessModifier.Private)]
	public static int[] supportOffsets = new int[]
	{
		0,
		1,
		1,
		1,
		1,
		0,
		1,
		-1,
		0,
		-1,
		-1,
		-1,
		-1,
		0,
		-1,
		1
	};

	// Token: 0x04003EE3 RID: 16099
	[PublicizedFrom(EAccessModifier.Private)]
	public MicroStopwatch msUnculling = new MicroStopwatch();

	// Token: 0x04003EE4 RID: 16100
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSetList<Chunk> chunksToUncull = new HashSetList<Chunk>();

	// Token: 0x04003EE5 RID: 16101
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSetList<Chunk> chunksToRegenerate = new HashSetList<Chunk>();

	// Token: 0x04003EE6 RID: 16102
	[PublicizedFrom(EAccessModifier.Private)]
	public World.ClipBlock[] _clipBlocks = new World.ClipBlock[32];

	// Token: 0x04003EE7 RID: 16103
	[PublicizedFrom(EAccessModifier.Private)]
	public Bounds[] _clipBounds = new Bounds[16];

	// Token: 0x04003EE8 RID: 16104
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cCollCacheSize = 50;

	// Token: 0x04003EE9 RID: 16105
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue[,,] collBlockCache = new BlockValue[50, 50, 50];

	// Token: 0x04003EEA RID: 16106
	[PublicizedFrom(EAccessModifier.Private)]
	public sbyte[,,] collDensityCache = new sbyte[50, 50, 50];

	// Token: 0x04003EEB RID: 16107
	[PublicizedFrom(EAccessModifier.Private)]
	public int tickEntityFrameCount;

	// Token: 0x04003EEC RID: 16108
	[PublicizedFrom(EAccessModifier.Private)]
	public float tickEntityFrameCountAverage = 1f;

	// Token: 0x04003EED RID: 16109
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Entity> tickEntityList = new List<Entity>();

	// Token: 0x04003EEE RID: 16110
	[PublicizedFrom(EAccessModifier.Private)]
	public float tickEntityPartialTicks;

	// Token: 0x04003EEF RID: 16111
	[PublicizedFrom(EAccessModifier.Private)]
	public int tickEntityIndex;

	// Token: 0x04003EF0 RID: 16112
	[PublicizedFrom(EAccessModifier.Private)]
	public int tickEntitySliceCount;

	// Token: 0x04003EF1 RID: 16113
	[PublicizedFrom(EAccessModifier.Protected)]
	public Queue<Vector3i> fallingBlocks = new Queue<Vector3i>();

	// Token: 0x04003EF2 RID: 16114
	[PublicizedFrom(EAccessModifier.Protected)]
	public Dictionary<Vector3i, float> fallingBlocksMap = new Dictionary<Vector3i, float>();

	// Token: 0x04003EF3 RID: 16115
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Chunk> m_lpChunkList = new List<Chunk>();

	// Token: 0x04003EF4 RID: 16116
	public const float cEdgeHard = 50f;

	// Token: 0x04003EF5 RID: 16117
	public const float cEdgeSoft = 80f;

	// Token: 0x04003EF6 RID: 16118
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cEdgeMinWorldSize = 1024;

	// Token: 0x04003EF7 RID: 16119
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<TraderArea> traderAreas;

	// Token: 0x04003EF8 RID: 16120
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cTraderPlacingProtection = 2;

	// Token: 0x04003EF9 RID: 16121
	public bool isEventBloodMoon;

	// Token: 0x04003EFA RID: 16122
	public ulong eventWorldTime;

	// Token: 0x04003EFB RID: 16123
	public int WorldDay;

	// Token: 0x04003EFC RID: 16124
	public int WorldHour;

	// Token: 0x04003EFD RID: 16125
	[PublicizedFrom(EAccessModifier.Protected)]
	public HashSet<Vector3i> pendingUpgradeDowngradeBlocks = new HashSet<Vector3i>();

	// Token: 0x02000A76 RID: 2678
	// (Invoke) Token: 0x060052AB RID: 21163
	public delegate void OnEntityLoadedDelegate(Entity _entity);

	// Token: 0x02000A77 RID: 2679
	// (Invoke) Token: 0x060052AF RID: 21167
	public delegate void OnEntityUnloadedDelegate(Entity _entity, EnumRemoveEntityReason _reason);

	// Token: 0x02000A78 RID: 2680
	// (Invoke) Token: 0x060052B3 RID: 21171
	public delegate void OnWorldChangedEvent(string _sWorldName);

	// Token: 0x02000A79 RID: 2681
	[PublicizedFrom(EAccessModifier.Private)]
	public class ClipBlock
	{
		// Token: 0x060052B6 RID: 21174 RVA: 0x00214137 File Offset: 0x00212337
		public static void ResetStorage()
		{
			World.ClipBlock._storageIndex = 0;
		}

		// Token: 0x060052B7 RID: 21175 RVA: 0x00214140 File Offset: 0x00212340
		public static World.ClipBlock New(BlockValue _value, Block _block, float _yDistort, Vector3 _blockPos, Bounds _bounds)
		{
			World.ClipBlock clipBlock = World.ClipBlock._storage[World.ClipBlock._storageIndex];
			if (clipBlock == null)
			{
				clipBlock = new World.ClipBlock();
				World.ClipBlock._storage[World.ClipBlock._storageIndex] = clipBlock;
			}
			clipBlock.Init(_value, _block, _yDistort, _blockPos, _bounds);
			World.ClipBlock._storageIndex++;
			return clipBlock;
		}

		// Token: 0x060052B8 RID: 21176 RVA: 0x00214188 File Offset: 0x00212388
		[PublicizedFrom(EAccessModifier.Private)]
		public void Init(BlockValue _value, Block _block, float _yDistort, Vector3 _blockPos, Bounds _bounds)
		{
			this.value = _value;
			this.block = _block;
			this.pos = _blockPos;
			Bounds bounds = _bounds;
			bounds.center -= _blockPos;
			bounds.min -= new Vector3(0f, _yDistort, 0f);
			this.bmins = bounds.min;
			this.bmaxs = bounds.max;
		}

		// Token: 0x04003EFE RID: 16126
		public const int kMaxBlocks = 32;

		// Token: 0x04003EFF RID: 16127
		public BlockValue value;

		// Token: 0x04003F00 RID: 16128
		public Vector3 pos;

		// Token: 0x04003F01 RID: 16129
		public Block block;

		// Token: 0x04003F02 RID: 16130
		public Vector3 bmins;

		// Token: 0x04003F03 RID: 16131
		public Vector3 bmaxs;

		// Token: 0x04003F04 RID: 16132
		[PublicizedFrom(EAccessModifier.Private)]
		public static int _storageIndex = 0;

		// Token: 0x04003F05 RID: 16133
		[PublicizedFrom(EAccessModifier.Private)]
		public static World.ClipBlock[] _storage = new World.ClipBlock[32];
	}

	// Token: 0x02000A7A RID: 2682
	public enum WorldEvent
	{
		// Token: 0x04003F07 RID: 16135
		BloodMoon
	}
}
