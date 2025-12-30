using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// Token: 0x020009BF RID: 2495
public class ChunkProviderDisc : ChunkProviderAbstract
{
	// Token: 0x06004C6D RID: 19565 RVA: 0x001E48C1 File Offset: 0x001E2AC1
	public ChunkProviderDisc(ChunkCluster _cc, string _worldName)
	{
		this.cc = _cc;
		this.worldName = _worldName;
	}

	// Token: 0x06004C6E RID: 19566 RVA: 0x001E48D7 File Offset: 0x001E2AD7
	public override IEnumerator Init(World _world)
	{
		this.world = _world;
		this.prefabDecorator = new DynamicPrefabDecorator();
		if (GameUtils.IsPlaytesting())
		{
			if (SdDirectory.Exists(GameIO.GetSaveGameRegionDir()))
			{
				SdDirectory.Delete(GameIO.GetSaveGameRegionDir(), true);
			}
			if (SdFile.Exists(GameIO.GetSaveGameDir() + "/decoration.7dt"))
			{
				SdFile.Delete(GameIO.GetSaveGameDir() + "/decoration.7dt");
			}
		}
		bool flag = false;
		if (_world.IsEditor() || !SdDirectory.Exists(GameIO.GetSaveGameRegionDir()))
		{
			flag = true;
		}
		this.m_RegionFileManager = new RegionFileManager(GameIO.GetSaveGameRegionDirDefault(), (!_world.IsEditor()) ? GameIO.GetSaveGameRegionDir() : GameIO.GetSaveGameRegionDirDefault(), 0, false);
		this.cc.IsFixedSize = true;
		long[] allChunkKeys = this.m_RegionFileManager.GetAllChunkKeys();
		for (int i = 0; i < allChunkKeys.Length; i++)
		{
			Chunk chunkSync = this.m_RegionFileManager.GetChunkSync(allChunkKeys[i]);
			if (chunkSync != null)
			{
				chunkSync.FillBiomeId(3);
				chunkSync.NeedsRegeneration = false;
				chunkSync.NeedsLightCalculation = false;
				this.cc.AddChunkSync(chunkSync, false);
			}
		}
		PathAbstractions.AbstractedLocation worldLocation = PathAbstractions.WorldsSearchPaths.GetLocation(this.worldName, null, null);
		if (flag)
		{
			yield return this.prefabDecorator.Load(worldLocation.FullPath);
			this.prefabDecorator.CopyAllPrefabsIntoWorld(this.world, false);
		}
		this.spawnPointManager = new SpawnPointManager(true);
		this.spawnPointManager.Load(worldLocation.FullPath);
		if (GameUtils.IsPlaytesting())
		{
			Prefab prefab = new Prefab();
			prefab.Load(GamePrefs.GetString(EnumGamePrefs.GameName), true, true, false, false);
			this.world.m_ChunkManager.RemoveAllChunks();
			int num = -1 * prefab.size.x / 2;
			int num2 = -1 * prefab.size.z / 2;
			int num3 = num + prefab.size.x;
			int num4 = num2 + prefab.size.z;
			this.cc.ChunkMinPos = new Vector2i((num - 1) / 16 - 1, (num2 - 1) / 16 - 1);
			this.cc.ChunkMinPos -= new Vector2i(2, 5);
			this.cc.ChunkMaxPos = new Vector2i(num3 / 16 + 1, num4 / 16 + 1);
			this.cc.ChunkMaxPos += new Vector2i(2, 2);
			for (int j = this.cc.ChunkMinPos.x; j <= this.cc.ChunkMaxPos.x; j++)
			{
				for (int k = this.cc.ChunkMinPos.y; k <= this.cc.ChunkMaxPos.y; k++)
				{
					Chunk chunk = MemoryPools.PoolChunks.AllocSync(true);
					chunk.X = j;
					chunk.Z = k;
					chunk.FillBiomeId(3);
					chunk.FillBlockRaw((int)WorldConstants.WaterLevel - 1, Block.GetBlockValue("terrainFiller", false));
					chunk.SetFullSunlight();
					chunk.NeedsLightCalculation = false;
					chunk.NeedsDecoration = false;
					chunk.NeedsRegeneration = false;
					this.cc.AddChunkSync(chunk, true);
				}
			}
			Vector3i position = new Vector3i((float)(-(float)prefab.size.x / 2), WorldConstants.WaterLevel + (float)prefab.yOffset, (float)(-(float)prefab.size.z / 2));
			PrefabInstance pi = new PrefabInstance(1, prefab.location, position, 0, prefab, 0);
			this.prefabDecorator.AddPrefab(pi, true);
			this.prefabDecorator.CopyAllPrefabsIntoWorld(this.world, false);
			int num5 = -prefab.size.z / 2 - 11;
			this.world.SetBlock(0, new Vector3i(2, (int)(this.world.GetHeight(0, num5) + 1), num5), Block.GetBlockValue("cntQuestTestLoot", false), false, false);
			Vector3 vector = new Vector3(-2f, (float)(this.world.GetHeight(0, num5) + 2), (float)num5);
			IChunk chunkFromWorldPos = this.world.GetChunkFromWorldPos(new Vector3i(vector));
			if (chunkFromWorldPos != null)
			{
				EntityCreationData entityCreationData = new EntityCreationData();
				entityCreationData.pos = vector;
				entityCreationData.rot = new Vector3(0f, 180f, 0f);
				entityCreationData.entityClass = EntityClass.FromString("npcTraderTest");
				((Chunk)chunkFromWorldPos).AddEntityStub(entityCreationData);
			}
			foreach (Chunk chunk2 in this.world.ChunkClusters[0].GetChunkArrayCopySync())
			{
				chunk2.ResetStability();
				chunk2.ResetLights(0);
				chunk2.RefreshSunlight();
				chunk2.NeedsLightCalculation = true;
			}
		}
		if (this.worldName == "Empty")
		{
			this.spawnPointManager.spawnPointList = new SpawnPointList
			{
				new SpawnPoint(new Vector3i(0, 10, 0))
			};
		}
		yield return null;
		yield break;
	}

	// Token: 0x06004C6F RID: 19567 RVA: 0x001E48ED File Offset: 0x001E2AED
	public override DynamicPrefabDecorator GetDynamicPrefabDecorator()
	{
		return this.prefabDecorator;
	}

	// Token: 0x06004C70 RID: 19568 RVA: 0x001E48F5 File Offset: 0x001E2AF5
	public override SpawnPointList GetSpawnPointList()
	{
		return this.spawnPointManager.spawnPointList;
	}

	// Token: 0x06004C71 RID: 19569 RVA: 0x001E4902 File Offset: 0x001E2B02
	public override void SetSpawnPointList(SpawnPointList _spawnPointList)
	{
		this.spawnPointManager.spawnPointList = _spawnPointList;
	}

	// Token: 0x06004C72 RID: 19570 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool GetOverviewMap(Vector2i _startPos, Vector2i _size, Color[] mapColors)
	{
		return false;
	}

	// Token: 0x06004C73 RID: 19571 RVA: 0x000197A5 File Offset: 0x000179A5
	public override EnumChunkProviderId GetProviderId()
	{
		return EnumChunkProviderId.Disc;
	}

	// Token: 0x06004C74 RID: 19572 RVA: 0x001E4910 File Offset: 0x001E2B10
	public override void Cleanup()
	{
		Thread.Sleep(200);
		this.m_RegionFileManager.Cleanup();
	}

	// Token: 0x06004C75 RID: 19573 RVA: 0x001E4928 File Offset: 0x001E2B28
	public override void SaveAll()
	{
		if (this.world.IsEditor())
		{
			PathAbstractions.AbstractedLocation location = PathAbstractions.WorldsSearchPaths.GetLocation(this.worldName, null, null);
			if (location.Type == PathAbstractions.EAbstractedLocationType.None)
			{
				SdDirectory.CreateDirectory(GameIO.GetGameDir("Data/Worlds") + "/" + this.worldName);
				location = PathAbstractions.WorldsSearchPaths.GetLocation(this.worldName, null, null);
			}
			this.prefabDecorator.Save(location.FullPath);
			this.prefabDecorator.CleanAllPrefabsFromWorld(this.world);
			this.m_RegionFileManager.MakePersistent(this.cc, true);
			this.m_RegionFileManager.WaitSaveDone();
			this.prefabDecorator.CopyAllPrefabsIntoWorld(this.world, false);
			this.spawnPointManager.Save(location.FullPath);
			return;
		}
		this.m_RegionFileManager.MakePersistent(this.world.ChunkCache, false);
		this.m_RegionFileManager.WaitSaveDone();
	}

	// Token: 0x06004C76 RID: 19574 RVA: 0x00002914 File Offset: 0x00000B14
	public override void SaveRandomChunks(int count, ulong _curWorldTimeInTicks, ArraySegment<long> _activeChunkSet)
	{
	}

	// Token: 0x06004C77 RID: 19575 RVA: 0x001E4A20 File Offset: 0x001E2C20
	public override bool GetWorldExtent(out Vector3i _minSize, out Vector3i _maxSize)
	{
		_minSize = new Vector3i(this.world.ChunkCache.ChunkMinPos.x * 16, 0, this.world.ChunkCache.ChunkMinPos.y * 16);
		_maxSize = new Vector3i(this.world.ChunkCache.ChunkMaxPos.x * 16, 255, this.world.ChunkCache.ChunkMaxPos.y * 16);
		return true;
	}

	// Token: 0x06004C78 RID: 19576 RVA: 0x001E4AAC File Offset: 0x001E2CAC
	public override void RebuildTerrain(HashSetLong _chunks, Vector3i _areaStart, Vector3i _areaSize, bool _bStopStabilityUpdate, bool _bRegenerateChunk, bool _bFillEmptyBlocks, bool _isReset)
	{
		PathAbstractions.AbstractedLocation location = PathAbstractions.WorldsSearchPaths.GetLocation(this.worldName, null, null);
		ThreadManager.RunCoroutineSync(this.prefabDecorator.Load(location.FullPath));
		this.prefabDecorator.CopyAllPrefabsIntoWorld(this.world, true);
		Chunk[] neighbours = new Chunk[8];
		foreach (Chunk chunk in this.world.ChunkClusters[0].GetChunkArrayCopySync())
		{
			chunk.ResetStability();
			chunk.ResetLights(0);
			chunk.RefreshSunlight();
			if (this.cc.GetNeighborChunks(chunk, neighbours))
			{
				this.cc.LightChunk(chunk, neighbours);
			}
			List<TileEntity> list = chunk.GetTileEntities().list;
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Reset(FastTags<TagGroup.Global>.none);
			}
		}
		List<EntityPlayerLocal> localPlayers = this.world.GetLocalPlayers();
		foreach (long num in _chunks)
		{
			Chunk chunkSync = this.cc.GetChunkSync(num);
			if (chunkSync != null)
			{
				for (int j = 0; j < this.world.m_ChunkManager.m_ObservedEntities.Count; j++)
				{
					if (!this.world.m_ChunkManager.m_ObservedEntities[j].bBuildVisualMeshAround)
					{
						bool flag = false;
						int num2 = 0;
						while (!flag && num2 < localPlayers.Count)
						{
							if (this.world.m_ChunkManager.m_ObservedEntities[j].entityIdToSendChunksTo == localPlayers[num2].entityId)
							{
								flag = true;
							}
							num2++;
						}
						if (!flag && this.world.m_ChunkManager.m_ObservedEntities[j].chunksLoaded.Contains(num))
						{
							SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageChunk>().Setup(chunkSync, true), false, this.world.m_ChunkManager.m_ObservedEntities[j].entityIdToSendChunksTo, -1, -1, null, 192, false);
						}
					}
				}
			}
		}
	}

	// Token: 0x06004C79 RID: 19577 RVA: 0x001E4D34 File Offset: 0x001E2F34
	public override ChunkProtectionLevel GetChunkProtectionLevel(Vector3i worldPos)
	{
		return this.m_RegionFileManager.GetChunkProtectionLevelForWorldPos(worldPos);
	}

	// Token: 0x04003A70 RID: 14960
	[PublicizedFrom(EAccessModifier.Protected)]
	public World world;

	// Token: 0x04003A71 RID: 14961
	[PublicizedFrom(EAccessModifier.Protected)]
	public string worldName;

	// Token: 0x04003A72 RID: 14962
	[PublicizedFrom(EAccessModifier.Protected)]
	public string gameName;

	// Token: 0x04003A73 RID: 14963
	[PublicizedFrom(EAccessModifier.Protected)]
	public WorldState m_HeaderInfo;

	// Token: 0x04003A74 RID: 14964
	[PublicizedFrom(EAccessModifier.Protected)]
	public DynamicPrefabDecorator prefabDecorator;

	// Token: 0x04003A75 RID: 14965
	[PublicizedFrom(EAccessModifier.Protected)]
	public RegionFileManager m_RegionFileManager;

	// Token: 0x04003A76 RID: 14966
	[PublicizedFrom(EAccessModifier.Private)]
	public IChunkProviderIndicator chunkManager;

	// Token: 0x04003A77 RID: 14967
	[PublicizedFrom(EAccessModifier.Protected)]
	public ChunkCluster cc;

	// Token: 0x04003A78 RID: 14968
	[PublicizedFrom(EAccessModifier.Protected)]
	public SpawnPointManager spawnPointManager;
}
