using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

// Token: 0x020009C6 RID: 2502
public abstract class ChunkProviderGenerateWorld : ChunkProviderAbstract
{
	// Token: 0x06004CA1 RID: 19617 RVA: 0x001E5EA4 File Offset: 0x001E40A4
	public ChunkProviderGenerateWorld(ChunkCluster _cc, string _levelName, bool _bClientMode = false)
	{
		this.bClientMode = _bClientMode;
		this.levelName = _levelName;
		this.bDecorationsEnabled = true;
	}

	// Token: 0x06004CA2 RID: 19618 RVA: 0x001E5F04 File Offset: 0x001E4104
	public override DynamicPrefabDecorator GetDynamicPrefabDecorator()
	{
		return this.prefabDecorator;
	}

	// Token: 0x06004CA3 RID: 19619 RVA: 0x001E5F0C File Offset: 0x001E410C
	public override IEnumerator Init(World _world)
	{
		this.world = _world;
		PathAbstractions.AbstractedLocation worldLocation = PathAbstractions.WorldsSearchPaths.GetLocation(this.levelName, null, null);
		this.prefabDecorator = new DynamicPrefabDecorator();
		yield return this.prefabDecorator.Load(worldLocation.FullPath);
		yield return null;
		this.spawnPointManager = new SpawnPointManager(true);
		if (!this.bClientMode)
		{
			yield return null;
			this.spawnPointManager.Load(worldLocation.FullPath);
		}
		if (!this.bClientMode)
		{
			this.threadInfo = ThreadManager.StartThread("GenerateChunks", null, new ThreadManager.ThreadFunctionLoopDelegate(this.GenerateChunksThread), null, null, null, true, false);
		}
		yield return null;
		yield break;
	}

	// Token: 0x06004CA4 RID: 19620 RVA: 0x001E5F24 File Offset: 0x001E4124
	public override void SaveAll()
	{
		if (this.bClientMode)
		{
			return;
		}
		if (this.world.IsEditor())
		{
			PathAbstractions.AbstractedLocation location = PathAbstractions.WorldsSearchPaths.GetLocation(this.levelName, null, null);
			this.GetDynamicPrefabDecorator().Save(location.FullPath);
			this.spawnPointManager.Save(location.FullPath);
			return;
		}
		if (this.m_RegionFileManager != null)
		{
			this.m_RegionFileManager.MakePersistent(this.world.ChunkCache, false);
			this.m_RegionFileManager.WaitSaveDone();
		}
	}

	// Token: 0x06004CA5 RID: 19621 RVA: 0x001E5FAC File Offset: 0x001E41AC
	public override void SaveRandomChunks(int count, ulong _curWorldTimeInTicks, ArraySegment<long> _activeChunkSet)
	{
		if (this.bClientMode)
		{
			return;
		}
		GameRandom gameRandom = GameManager.Instance.World.GetGameRandom();
		int i = _activeChunkSet.Offset;
		while (i < _activeChunkSet.Count)
		{
			Chunk chunkSync = this.world.ChunkCache.GetChunkSync(_activeChunkSet.Array[i]);
			if (chunkSync != null && chunkSync.NeedsSaving && !chunkSync.NeedsDecoration && !chunkSync.InProgressDecorating && !chunkSync.NeedsLightCalculation && !chunkSync.InProgressLighting && _curWorldTimeInTicks - chunkSync.SavedInWorldTicks > 400UL && gameRandom.RandomFloat < 0.3f)
			{
				Chunk obj = chunkSync;
				lock (obj)
				{
					if (chunkSync.IsLocked)
					{
						goto IL_E5;
					}
					chunkSync.InProgressSaving = true;
				}
				this.m_RegionFileManager.SaveChunkSnapshot(chunkSync, false);
				count--;
				chunkSync.InProgressSaving = false;
				goto IL_E1;
			}
			goto IL_E1;
			IL_E5:
			i++;
			continue;
			IL_E1:
			if (count > 0)
			{
				goto IL_E5;
			}
			break;
		}
	}

	// Token: 0x06004CA6 RID: 19622 RVA: 0x001E60C0 File Offset: 0x001E42C0
	public override void ClearCaches()
	{
		if (this.bClientMode)
		{
			return;
		}
		this.m_RegionFileManager.ClearCaches();
	}

	// Token: 0x06004CA7 RID: 19623 RVA: 0x001E60D6 File Offset: 0x001E42D6
	public HashSetLong ResetAllChunks(ChunkProtectionLevel excludedProtectionLevels)
	{
		return this.m_RegionFileManager.ResetAllChunks(excludedProtectionLevels);
	}

	// Token: 0x06004CA8 RID: 19624 RVA: 0x001E60E4 File Offset: 0x001E42E4
	public HashSetLong ResetRegion(int _regionX, int _regionZ, ChunkProtectionLevel excludedProtectionLevels)
	{
		return this.m_RegionFileManager.ResetRegion(_regionX, _regionZ, excludedProtectionLevels);
	}

	// Token: 0x06004CA9 RID: 19625 RVA: 0x001E60F4 File Offset: 0x001E42F4
	public void RequestChunkReset(long _chunkKey)
	{
		this.m_RegionFileManager.RequestChunkReset(_chunkKey);
	}

	// Token: 0x06004CAA RID: 19626 RVA: 0x001E6102 File Offset: 0x001E4302
	public void MainThreadCacheProtectedPositions()
	{
		this.m_RegionFileManager.MainThreadCacheProtectedPositions();
	}

	// Token: 0x06004CAB RID: 19627 RVA: 0x001E610F File Offset: 0x001E430F
	public void SaveChunkAgeDebugTexture(float rangeInDays)
	{
		this.m_RegionFileManager.SaveChunkAgeDebugTexture(rangeInDays);
	}

	// Token: 0x06004CAC RID: 19628 RVA: 0x001E611D File Offset: 0x001E431D
	public void IterateChunkExpiryTimes(Action<long, ulong> action)
	{
		this.m_RegionFileManager.IterateChunkExpiryTimes(action);
	}

	// Token: 0x170007E5 RID: 2021
	// (get) Token: 0x06004CAD RID: 19629 RVA: 0x001E612B File Offset: 0x001E432B
	public ReadOnlyCollection<HashSetLong> ChunkGroups
	{
		get
		{
			return this.m_RegionFileManager.ChunkGroups;
		}
	}

	// Token: 0x06004CAE RID: 19630 RVA: 0x001E6138 File Offset: 0x001E4338
	public override void RequestChunk(int _x, int _y)
	{
		if (this.bClientMode)
		{
			return;
		}
		object syncRoot = ((ICollection)this.m_ChunkQueue.list).SyncRoot;
		lock (syncRoot)
		{
			long num = WorldChunkCache.MakeChunkKey(_x, _y);
			if (this.m_ChunkQueue.hashSet.Contains(num))
			{
				return;
			}
			this.m_ChunkQueue.Add(num);
		}
		this.m_WaitHandle.Set();
	}

	// Token: 0x06004CAF RID: 19631 RVA: 0x001E61BC File Offset: 0x001E43BC
	public override HashSetList<long> GetRequestedChunks()
	{
		return this.m_ChunkQueue;
	}

	// Token: 0x06004CB0 RID: 19632 RVA: 0x001E61C4 File Offset: 0x001E43C4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void generateTerrain(World _world, Chunk _chunk, GameRandom _random)
	{
		this.m_TerrainGenerator.GenerateTerrain(_world, _chunk, _random, Vector3i.zero, Vector3i.zero, false, false);
	}

	// Token: 0x06004CB1 RID: 19633 RVA: 0x001E61E0 File Offset: 0x001E43E0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void generateTerrain(World _world, Chunk _chunk, GameRandom _random, Vector3i _areaStart, Vector3i _areaSize, bool _bFillEmptyBlocks, bool _isReset)
	{
		this.m_TerrainGenerator.GenerateTerrain(_world, _chunk, _random, _areaStart, _areaSize, _bFillEmptyBlocks, _isReset);
	}

	// Token: 0x06004CB2 RID: 19634 RVA: 0x001E61F8 File Offset: 0x001E43F8
	public bool GenerateSingleChunk(ChunkCluster cc, long key, bool _forceRebuild = false)
	{
		this.currentGeneratingChunk = key;
		this.chunkGenerationTimer.Restart();
		if (!_forceRebuild && cc.ContainsChunkSync(key))
		{
			this.currentGeneratingChunk = 0L;
			return false;
		}
		Chunk chunk = null;
		if (this.m_RegionFileManager.ContainsChunkSync(key))
		{
			chunk = this.m_RegionFileManager.GetChunkSync(key);
			this.m_RegionFileManager.RemoveChunkSync(key);
		}
		if (_forceRebuild)
		{
			chunk = cc.GetChunkSync(key);
			if (chunk != null)
			{
				chunk.RemoveBlockEntityTransforms();
				chunk.Reset();
			}
		}
		if (_forceRebuild || chunk == null)
		{
			int x = WorldChunkCache.extractX(key);
			int num = WorldChunkCache.extractZ(key);
			if (chunk == null)
			{
				chunk = MemoryPools.PoolChunks.AllocSync(true);
			}
			if (chunk != null)
			{
				chunk.X = x;
				chunk.Z = num;
				GameRandom gameRandom = Utils.RandomFromSeedOnPos(x, num, this.world.Seed);
				this.generateTerrain(this.world, chunk, gameRandom);
				GameRandomManager.Instance.FreeGameRandom(gameRandom);
				if (!this.bDecorationsEnabled)
				{
					chunk.NeedsDecoration = false;
					chunk.NeedsLightCalculation = false;
					chunk.NeedsRegeneration = true;
				}
				if (this.bDecorationsEnabled)
				{
					chunk.NeedsDecoration = true;
					chunk.NeedsLightCalculation = true;
					if (this.GetDynamicPrefabDecorator() != null)
					{
						this.GetDynamicPrefabDecorator().DecorateChunk(this.world, chunk);
					}
				}
			}
		}
		bool flag = false;
		if (chunk != null)
		{
			if (!_forceRebuild)
			{
				flag = cc.AddChunkSync(chunk, false);
			}
			else
			{
				ReaderWriterLockSlim syncRoot = cc.GetSyncRoot();
				syncRoot.EnterUpgradeableReadLock();
				if (cc.ContainsChunkSync(key))
				{
					cc.RemoveChunkSync(key);
				}
				flag = cc.AddChunkSync(chunk, false);
				syncRoot.ExitUpgradeableReadLock();
			}
			if (flag)
			{
				if (!chunk.NeedsDecoration)
				{
					ChunkProviderGenerateWorld.OnChunkSyncedAndDecorated(chunk);
				}
				this.updateDecorationsWherePossible(chunk);
				if (_forceRebuild)
				{
					chunk.isModified = true;
				}
			}
			else
			{
				MemoryPools.PoolChunks.FreeSync(chunk);
			}
		}
		return flag;
	}

	// Token: 0x06004CB3 RID: 19635 RVA: 0x001E639D File Offset: 0x001E459D
	[PublicizedFrom(EAccessModifier.Private)]
	public static void OnChunkSyncedAndDecorated(Chunk chunk)
	{
		WaterSimulationNative.Instance.InitializeChunk(chunk);
	}

	// Token: 0x06004CB4 RID: 19636 RVA: 0x001E63AC File Offset: 0x001E45AC
	[PublicizedFrom(EAccessModifier.Private)]
	public int GenerateChunksThread(ThreadManager.ThreadInfo _threadInfo)
	{
		if (_threadInfo.TerminationRequested())
		{
			return -1;
		}
		if (this.m_RegionFileManager == null)
		{
			return 15;
		}
		long num = this.world.GetNextChunkToProvide();
		if (num == 9223372036854775807L)
		{
			num = DynamicMeshThread.GetNextChunkToLoad();
			if (num == 9223372036854775807L)
			{
				return 15;
			}
		}
		ChunkCluster chunkCache = this.world.ChunkCache;
		this.GenerateSingleChunk(chunkCache, num, false);
		return 0;
	}

	// Token: 0x06004CB5 RID: 19637 RVA: 0x001E6412 File Offset: 0x001E4612
	[PublicizedFrom(EAccessModifier.Private)]
	public void tryToDecorate(Chunk _chunk)
	{
		if (_chunk == null)
		{
			return;
		}
		if (!_chunk.NeedsDecoration)
		{
			return;
		}
		if (_chunk.IsLocked)
		{
			return;
		}
		this.decorate(_chunk);
	}

	// Token: 0x06004CB6 RID: 19638 RVA: 0x001E6434 File Offset: 0x001E4634
	[PublicizedFrom(EAccessModifier.Private)]
	public void decorate(Chunk _chunk)
	{
		int x = _chunk.X;
		int z = _chunk.Z;
		Chunk chunk;
		Chunk chunk2;
		Chunk chunk3;
		if ((chunk = (Chunk)this.world.GetChunkSync(x + 1, z + 1)) == null || (chunk2 = (Chunk)this.world.GetChunkSync(x, z + 1)) == null || (chunk3 = (Chunk)this.world.GetChunkSync(x + 1, z)) == null)
		{
			return;
		}
		chunk.InProgressDecorating = true;
		chunk2.InProgressDecorating = true;
		chunk3.InProgressDecorating = true;
		_chunk.InProgressDecorating = true;
		this.updateDecosAllowedForChunk(_chunk, chunk3, chunk2);
		for (int i = 0; i < this.m_Decorators.Count; i++)
		{
			this.m_Decorators[i].DecorateChunkOverlapping(this.world, _chunk, chunk3, chunk2, chunk, this.world.Seed);
		}
		_chunk.OnDecorated();
		_chunk.ResetStability();
		_chunk.RefreshSunlight();
		_chunk.NeedsDecoration = false;
		_chunk.NeedsLightCalculation = true;
		chunk.InProgressDecorating = false;
		chunk2.InProgressDecorating = false;
		chunk3.InProgressDecorating = false;
		_chunk.InProgressDecorating = false;
		ChunkProviderGenerateWorld.OnChunkSyncedAndDecorated(_chunk);
	}

	// Token: 0x06004CB7 RID: 19639 RVA: 0x001E655C File Offset: 0x001E475C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateDecosAllowedForChunk(Chunk _chunk, Chunk _c10, Chunk _c01)
	{
		Vector3 lhs = new Vector3(0f, 0f, 1f);
		Vector3 rhs = new Vector3(1f, 0f, 0f);
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				int terrainHeight = (int)_chunk.GetTerrainHeight(j, i);
				int num = (int)((j < 15) ? _chunk.GetTerrainHeight(j + 1, i) : _c10.GetTerrainHeight(0, i));
				int num2 = (int)((i < 15) ? _chunk.GetTerrainHeight(j, i + 1) : _c01.GetTerrainHeight(j, 0));
				if (terrainHeight >= 253 || num >= 253 || num2 >= 253)
				{
					_chunk.SetDecoAllowedAt(j, i, EnumDecoAllowed.Nothing);
				}
				else
				{
					float num3 = (float)_chunk.GetDensity(j, terrainHeight, i) / -128f;
					float num4 = (j < 15) ? ((float)_chunk.GetDensity(j + 1, num, i) / -128f) : ((float)_c10.GetDensity(0, num, i) / -128f);
					float num5 = (i < 15) ? ((float)_chunk.GetDensity(j, num2, i + 1) / -128f) : ((float)_c01.GetDensity(j, num2, 0) / -128f);
					float num6 = (float)_chunk.GetDensity(j, terrainHeight + 1, i) / 127f;
					float num7 = (j < 15) ? ((float)_chunk.GetDensity(j + 1, num + 1, i) / 127f) : ((float)_c10.GetDensity(0, num + 1, i) / 127f);
					float num8 = (i < 15) ? ((float)_chunk.GetDensity(j, num2 + 1, i + 1) / 127f) : ((float)_c01.GetDensity(j, num2 + 1, 0) / 127f);
					if (num3 > 0.999f && num6 > 0.999f)
					{
						num3 = 0.5f;
					}
					if (num4 > 0.999f && num7 > 0.999f)
					{
						num4 = 0.5f;
					}
					if (num5 > 0.999f && num8 > 0.999f)
					{
						num5 = 0.5f;
					}
					float num9 = (float)terrainHeight + num3;
					float num10 = (float)num + num4;
					float num11 = (float)num2 + num5;
					lhs.y = num11 - num9;
					rhs.y = num10 - num9;
					Vector3 normalized = Vector3.Cross(lhs, rhs).normalized;
					_chunk.SetTerrainNormal(j, i, normalized);
					if (normalized.y < 0.55f)
					{
						_chunk.SetDecoAllowedSlopeAt(j, i, EnumDecoAllowedSlope.Steep);
					}
					else if (normalized.y < 0.65f)
					{
						_chunk.SetDecoAllowedSlopeAt(j, i, EnumDecoAllowedSlope.Sloped);
					}
					if (terrainHeight <= 1 || terrainHeight >= 255 || _chunk.IsWater(j, terrainHeight, i) || _chunk.IsWater(j, terrainHeight + 1, i))
					{
						_chunk.SetDecoAllowedAt(j, i, EnumDecoAllowed.Nothing);
					}
				}
			}
		}
	}

	// Token: 0x06004CB8 RID: 19640 RVA: 0x001E6808 File Offset: 0x001E4A08
	public void UpdateDecorations(Chunk _chunk)
	{
		this.decorate(_chunk);
	}

	// Token: 0x06004CB9 RID: 19641 RVA: 0x001E6814 File Offset: 0x001E4A14
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateDecorationsWherePossible(Chunk chunk)
	{
		World world = this.world;
		int x = chunk.X;
		int z = chunk.Z;
		this.tryToDecorate(chunk);
		this.tryToDecorate((Chunk)world.GetChunkSync(x - 1, z));
		this.tryToDecorate((Chunk)world.GetChunkSync(x, z - 1));
		this.tryToDecorate((Chunk)world.GetChunkSync(x - 1, z - 1));
	}

	// Token: 0x06004CBA RID: 19642 RVA: 0x001E687E File Offset: 0x001E4A7E
	public override void UnloadChunk(Chunk _chunk)
	{
		if (this.bClientMode)
		{
			MemoryPools.PoolChunks.FreeSync(_chunk);
			return;
		}
		this.m_RegionFileManager.AddChunkSync(_chunk, false);
	}

	// Token: 0x06004CBB RID: 19643 RVA: 0x001E68A4 File Offset: 0x001E4AA4
	public override void ReloadAllChunks()
	{
		object syncRoot = ((ICollection)this.m_ChunkQueue.list).SyncRoot;
		lock (syncRoot)
		{
			this.m_ChunkQueue.Clear();
		}
		this.m_RegionFileManager.Clear();
	}

	// Token: 0x06004CBC RID: 19644 RVA: 0x001E6900 File Offset: 0x001E4B00
	public List<ChunkProviderParameter> GetParameters()
	{
		return this.m_Parameters;
	}

	// Token: 0x06004CBD RID: 19645 RVA: 0x00002914 File Offset: 0x00000B14
	public override void Update()
	{
	}

	// Token: 0x06004CBE RID: 19646 RVA: 0x001E6908 File Offset: 0x001E4B08
	public override void StopUpdate()
	{
		if (this.threadInfo != null)
		{
			this.threadInfo.WaitForEnd(30);
			this.threadInfo = null;
		}
	}

	// Token: 0x06004CBF RID: 19647 RVA: 0x001E6926 File Offset: 0x001E4B26
	public override void Cleanup()
	{
		this.StopUpdate();
		if (this.spawnPointManager != null)
		{
			this.spawnPointManager.Cleanup();
		}
		if (this.m_RegionFileManager != null)
		{
			this.m_RegionFileManager.Cleanup();
		}
		MultiBlockManager.Instance.Cleanup();
	}

	// Token: 0x06004CC0 RID: 19648 RVA: 0x001E695E File Offset: 0x001E4B5E
	public override bool GetOverviewMap(Vector2i _startPos, Vector2i _size, Color[] mapColors)
	{
		this.m_RegionFileManager.SetCacheSize(1000);
		new TerrainMapGenerator().GenerateTerrain(this);
		this.m_RegionFileManager.SetCacheSize(0);
		return true;
	}

	// Token: 0x06004CC1 RID: 19649 RVA: 0x001E6988 File Offset: 0x001E4B88
	public override IBiomeProvider GetBiomeProvider()
	{
		return this.m_BiomeProvider;
	}

	// Token: 0x06004CC2 RID: 19650 RVA: 0x001E6990 File Offset: 0x001E4B90
	public override ITerrainGenerator GetTerrainGenerator()
	{
		return this.m_TerrainGenerator;
	}

	// Token: 0x06004CC3 RID: 19651 RVA: 0x001E6998 File Offset: 0x001E4B98
	public override SpawnPointList GetSpawnPointList()
	{
		return this.spawnPointManager.spawnPointList;
	}

	// Token: 0x06004CC4 RID: 19652 RVA: 0x001E69A5 File Offset: 0x001E4BA5
	public override void SetSpawnPointList(SpawnPointList _spawnPointList)
	{
		this.spawnPointManager.spawnPointList = _spawnPointList;
	}

	// Token: 0x06004CC5 RID: 19653 RVA: 0x001E69B4 File Offset: 0x001E4BB4
	public override void RebuildTerrain(HashSetLong _chunks, Vector3i _areaStart, Vector3i _areaSize, bool _isStopStabilityCalc, bool _isRegenChunk, bool _isFillEmptyBlocks, bool _isReset)
	{
		ChunkCluster chunkCluster = this.world.ChunkClusters[0];
		foreach (long key in _chunks)
		{
			Chunk chunkSync = chunkCluster.GetChunkSync(key);
			if (chunkSync != null)
			{
				GameRandom gameRandom = Utils.RandomFromSeedOnPos(chunkSync.X, chunkSync.Z, this.world.Seed);
				chunkSync.StopStabilityCalculation = _isStopStabilityCalc;
				this.generateTerrain(this.world, chunkSync, gameRandom, _areaStart, _areaSize, _isFillEmptyBlocks, _isReset);
				GameRandomManager.Instance.FreeGameRandom(gameRandom);
				if (_isRegenChunk)
				{
					chunkSync.NeedsRegeneration = true;
				}
			}
		}
	}

	// Token: 0x06004CC6 RID: 19654 RVA: 0x001E6A6C File Offset: 0x001E4C6C
	public void RemoveChunks(HashSetLong _chunks)
	{
		if (this.m_RegionFileManager != null)
		{
			this.m_RegionFileManager.RemoveChunks(_chunks, true);
		}
	}

	// Token: 0x06004CC7 RID: 19655 RVA: 0x001E6A83 File Offset: 0x001E4C83
	public override ChunkProtectionLevel GetChunkProtectionLevel(Vector3i worldPos)
	{
		return this.m_RegionFileManager.GetChunkProtectionLevelForWorldPos(worldPos);
	}

	// Token: 0x06004CC8 RID: 19656 RVA: 0x001E6A94 File Offset: 0x001E4C94
	public void LogCurrentChunkGeneration()
	{
		Log.Error(string.Format("ChunkProvider Current Generating Chunk - Key: {0}, Pos: {1}/{2}, Duration {3}", new object[]
		{
			this.currentGeneratingChunk,
			WorldChunkCache.extractX(this.currentGeneratingChunk) << 4,
			WorldChunkCache.extractZ(this.currentGeneratingChunk) << 4,
			this.chunkGenerationTimer.Elapsed.TotalSeconds
		}));
	}

	// Token: 0x04003A97 RID: 14999
	[PublicizedFrom(EAccessModifier.Protected)]
	public const int cCacheChunks = 0;

	// Token: 0x04003A98 RID: 15000
	[PublicizedFrom(EAccessModifier.Protected)]
	public World world;

	// Token: 0x04003A99 RID: 15001
	[PublicizedFrom(EAccessModifier.Protected)]
	public string levelName;

	// Token: 0x04003A9A RID: 15002
	[PublicizedFrom(EAccessModifier.Protected)]
	public string gameName;

	// Token: 0x04003A9B RID: 15003
	[PublicizedFrom(EAccessModifier.Protected)]
	public DynamicPrefabDecorator prefabDecorator;

	// Token: 0x04003A9C RID: 15004
	[PublicizedFrom(EAccessModifier.Protected)]
	public SpawnPointManager spawnPointManager;

	// Token: 0x04003A9D RID: 15005
	[PublicizedFrom(EAccessModifier.Protected)]
	public RegionFileManager m_RegionFileManager;

	// Token: 0x04003A9E RID: 15006
	[PublicizedFrom(EAccessModifier.Protected)]
	public IBiomeProvider m_BiomeProvider;

	// Token: 0x04003A9F RID: 15007
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<ChunkProviderParameter> m_Parameters = new List<ChunkProviderParameter>();

	// Token: 0x04003AA0 RID: 15008
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<IWorldDecorator> m_Decorators = new List<IWorldDecorator>();

	// Token: 0x04003AA1 RID: 15009
	[PublicizedFrom(EAccessModifier.Protected)]
	public ITerrainGenerator m_TerrainGenerator;

	// Token: 0x04003AA2 RID: 15010
	public HashSetList<long> m_ChunkQueue = new HashSetList<long>();

	// Token: 0x04003AA3 RID: 15011
	[PublicizedFrom(EAccessModifier.Private)]
	public AutoResetEvent m_WaitHandle = new AutoResetEvent(false);

	// Token: 0x04003AA4 RID: 15012
	[PublicizedFrom(EAccessModifier.Private)]
	public ThreadManager.ThreadInfo threadInfo;

	// Token: 0x04003AA5 RID: 15013
	[PublicizedFrom(EAccessModifier.Private)]
	public Stopwatch chunkGenerationTimer = new Stopwatch();

	// Token: 0x04003AA6 RID: 15014
	[PublicizedFrom(EAccessModifier.Private)]
	public long currentGeneratingChunk;

	// Token: 0x04003AA7 RID: 15015
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bClientMode;
}
