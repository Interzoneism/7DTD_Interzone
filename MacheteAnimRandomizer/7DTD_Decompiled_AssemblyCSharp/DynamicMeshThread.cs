using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ConcurrentCollections;
using UnityEngine;

// Token: 0x0200035C RID: 860
public class DynamicMeshThread
{
	// Token: 0x170002D5 RID: 725
	// (get) Token: 0x06001931 RID: 6449 RVA: 0x0009A295 File Offset: 0x00098495
	public static int MeshGenCount
	{
		get
		{
			return DynamicMeshThread.ChunkMeshGenRequests.Count + DynamicMeshThread.TempChunkMeshGenRequests.Count;
		}
	}

	// Token: 0x170002D6 RID: 726
	// (get) Token: 0x06001932 RID: 6450 RVA: 0x0009A2AC File Offset: 0x000984AC
	public static float time
	{
		get
		{
			return (float)(DateTime.Now - DynamicMeshThread.StartTime).TotalSeconds;
		}
	}

	// Token: 0x170002D7 RID: 727
	// (get) Token: 0x06001934 RID: 6452 RVA: 0x00091F4F File Offset: 0x0009014F
	public static bool IsServer
	{
		get
		{
			return SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer;
		}
	}

	// Token: 0x06001935 RID: 6453 RVA: 0x0009A2D4 File Offset: 0x000984D4
	public static void AddChunkGameObject(Chunk chunk)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (!chunk.NeedsOnlyCollisionMesh)
		{
			DynamicMeshManager.ChunkGameObjects.Add(chunk.Key);
			if (DynamicMeshManager.Instance != null)
			{
				DynamicMeshItem itemOrNull = DynamicMeshManager.Instance.GetItemOrNull(chunk.GetWorldPos());
				if (itemOrNull != null)
				{
					itemOrNull.ForceHide();
					itemOrNull.GetRegion().OnChunkVisible(itemOrNull);
				}
			}
		}
	}

	// Token: 0x06001936 RID: 6454 RVA: 0x0009A337 File Offset: 0x00098537
	public static void RemoveChunkGameObject(long key)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (DynamicMeshManager.Instance != null)
		{
			DynamicMeshManager.ChunkGameObjects.Remove(key);
			DynamicMeshManager.Instance.ShowChunk(key);
		}
	}

	// Token: 0x06001937 RID: 6455 RVA: 0x0009A368 File Offset: 0x00098568
	public static void CleanUp()
	{
		DynamicMeshThread.ChunksToProcess.Clear();
		DynamicMeshThread.ChunksToLoad.Clear();
		DynamicMeshThread.RequestThreadStop = true;
		DynamicMeshThread.BuilderManager.StopThreads(true);
		DynamicMeshServer.CleanUp();
		DynamicMeshThread.ServerUpdates.Clear();
		ConcurrentDictionary<long, DynamicMeshUpdateData> regionUpdates = DynamicMeshThread.RegionUpdates;
		if (regionUpdates != null)
		{
			regionUpdates.Clear();
		}
		DynamicMeshThread.ClearData();
		Queue<DynamicMeshItem> toGenerate = DynamicMeshThread.ToGenerate;
		if (toGenerate != null)
		{
			toGenerate.Clear();
		}
		LinkedList<DynamicMeshItem> needObservers = DynamicMeshThread.NeedObservers;
		if (needObservers != null)
		{
			needObservers.Clear();
		}
		LinkedList<DynamicMeshItem> ignoredChunks = DynamicMeshThread.IgnoredChunks;
		if (ignoredChunks != null)
		{
			ignoredChunks.Clear();
		}
		ConcurrentDictionary<long, DynamicMeshItem> primaryQueue = DynamicMeshThread.PrimaryQueue;
		if (primaryQueue != null)
		{
			primaryQueue.Clear();
		}
		ConcurrentDictionary<long, DynamicMeshItem> secondaryQueue = DynamicMeshThread.SecondaryQueue;
		if (secondaryQueue != null)
		{
			secondaryQueue.Clear();
		}
		List<ChunkGameObject> loadedGos = DynamicMeshThread.LoadedGos;
		if (loadedGos != null)
		{
			loadedGos.Clear();
		}
		Queue<ChunkGameObject> toRemoveGos = DynamicMeshThread.ToRemoveGos;
		if (toRemoveGos != null)
		{
			toRemoveGos.Clear();
		}
		ConcurrentDictionary<long, DynamicMeshThread.ThreadRegion> concurrentDictionary = DynamicMeshThread.threadRegions;
		if (concurrentDictionary != null)
		{
			concurrentDictionary.Clear();
		}
		DynamicMeshThread.nextChunks = new ConcurrentQueue<long>();
	}

	// Token: 0x06001938 RID: 6456 RVA: 0x0009A448 File Offset: 0x00098648
	public static bool AddChunkUpdateFromServer(DynamicMeshServerUpdates data)
	{
		DynamicMeshThread.ServerUpdates.Enqueue(data);
		DynamicMeshManager.Instance.AddChunkStub(new Vector3i(data.ChunkX, data.StartY, data.ChunkZ), null);
		return true;
	}

	// Token: 0x06001939 RID: 6457 RVA: 0x0009A478 File Offset: 0x00098678
	public static void AddRegionChunk(int worldX, int worldZ, long key)
	{
		DynamicMeshThread.GetThreadRegion(worldX, worldZ).AddLoadedChunk(key);
	}

	// Token: 0x0600193A RID: 6458 RVA: 0x0009A487 File Offset: 0x00098687
	public static void RemoveRegionChunk(int worldX, int worldZ, long key)
	{
		if (!DynamicMeshThread.GetThreadRegion(worldX, worldZ).RemoveLoadedChunk(key) && DynamicMeshManager.DoLog)
		{
			Log.Warning("Failed to remove threaded chunk");
		}
	}

	// Token: 0x0600193B RID: 6459 RVA: 0x0009A4AC File Offset: 0x000986AC
	public static bool AddRegionUpdateData(int worldX, int worldZ, bool isUrgent)
	{
		if (GameManager.IsDedicatedServer)
		{
			return false;
		}
		DynamicMeshThread.ThreadRegion threadRegion = DynamicMeshThread.GetThreadRegion(worldX, worldZ);
		DynamicMeshUpdateData dynamicMeshUpdateData;
		DynamicMeshThread.RegionUpdates.TryGetValue(threadRegion.Key, out dynamicMeshUpdateData);
		if (dynamicMeshUpdateData == null)
		{
			dynamicMeshUpdateData = new DynamicMeshUpdateData();
			dynamicMeshUpdateData.ChunkPosition.x = threadRegion.X;
			dynamicMeshUpdateData.ChunkPosition.z = threadRegion.Z;
			dynamicMeshUpdateData.Key = threadRegion.Key;
			dynamicMeshUpdateData.IsUrgent = false;
			DynamicMeshThread.RegionUpdates.TryAdd(dynamicMeshUpdateData.Key, dynamicMeshUpdateData);
		}
		dynamicMeshUpdateData.UpdateTime = DynamicMeshThread.time + (float)(isUrgent ? 0 : 3);
		dynamicMeshUpdateData.IsUrgent = (dynamicMeshUpdateData.IsUrgent || isUrgent);
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg(string.Concat(new string[]
			{
				"Adding thread region update ",
				threadRegion.ToDebugLocation(),
				" Time: ",
				dynamicMeshUpdateData.UpdateTime.ToString(),
				" urgent: ",
				isUrgent.ToString()
			}));
		}
		return true;
	}

	// Token: 0x0600193C RID: 6460 RVA: 0x0009A5A0 File Offset: 0x000987A0
	public static DynamicMeshThread.ThreadRegion GetThreadRegion(Vector3i worldPos)
	{
		return DynamicMeshThread.GetThreadRegionInternal(DynamicMeshUnity.GetRegionKeyFromWorldPosition(worldPos));
	}

	// Token: 0x0600193D RID: 6461 RVA: 0x0009A5AD File Offset: 0x000987AD
	public static DynamicMeshThread.ThreadRegion GetThreadRegion(int worldX, int worldZ)
	{
		return DynamicMeshThread.GetThreadRegionInternal(DynamicMeshUnity.GetRegionKeyFromWorldPosition(worldX, worldZ));
	}

	// Token: 0x0600193E RID: 6462 RVA: 0x0009A5BB File Offset: 0x000987BB
	public static DynamicMeshThread.ThreadRegion GetThreadRegion(long key)
	{
		return DynamicMeshThread.GetThreadRegionInternal(DynamicMeshUnity.GetRegionKeyFromItemKey(key));
	}

	// Token: 0x0600193F RID: 6463 RVA: 0x0009A5C8 File Offset: 0x000987C8
	[PublicizedFrom(EAccessModifier.Private)]
	public static DynamicMeshThread.ThreadRegion GetThreadRegionInternal(long key)
	{
		DynamicMeshThread.ThreadRegion threadRegion;
		if (!DynamicMeshThread.threadRegions.TryGetValue(key, out threadRegion))
		{
			threadRegion = new DynamicMeshThread.ThreadRegion(key);
			if (!DynamicMeshThread.threadRegions.TryAdd(key, threadRegion))
			{
				DynamicMeshThread.threadRegions.TryGetValue(key, out threadRegion);
			}
		}
		return threadRegion;
	}

	// Token: 0x06001940 RID: 6464 RVA: 0x0009A608 File Offset: 0x00098808
	public static void SetNextChunksFromQueues()
	{
		DynamicMeshThread.QueueUpdateOverride = true;
		foreach (KeyValuePair<long, DynamicMeshItem> keyValuePair in DynamicMeshThread.PrimaryQueue)
		{
			DynamicMeshThread.nextChunks.Enqueue(keyValuePair.Key);
		}
		foreach (KeyValuePair<long, DynamicMeshItem> keyValuePair2 in DynamicMeshThread.SecondaryQueue)
		{
			DynamicMeshThread.nextChunks.Enqueue(keyValuePair2.Key);
		}
	}

	// Token: 0x06001941 RID: 6465 RVA: 0x0009A6AC File Offset: 0x000988AC
	public static void SetNextChunks(long key)
	{
		if (DynamicMeshThread.nextChunks.Count > 512)
		{
			return;
		}
		int num = WorldChunkCache.extractX(key);
		int num2 = WorldChunkCache.extractZ(key);
		long item = WorldChunkCache.MakeChunkKey(num, num2 + 1);
		long item2 = WorldChunkCache.MakeChunkKey(num, num2 - 1);
		long item3 = WorldChunkCache.MakeChunkKey(num + 1, num2);
		long item4 = WorldChunkCache.MakeChunkKey(num + 1, num2 + 1);
		long item5 = WorldChunkCache.MakeChunkKey(num + 1, num2 - 1);
		long item6 = WorldChunkCache.MakeChunkKey(num - 1, num2);
		long item7 = WorldChunkCache.MakeChunkKey(num - 1, num2 + 1);
		long item8 = WorldChunkCache.MakeChunkKey(num - 1, num2 - 1);
		DynamicMeshThread.nextChunks.Enqueue(key);
		DynamicMeshThread.nextChunks.Enqueue(item);
		DynamicMeshThread.nextChunks.Enqueue(item2);
		DynamicMeshThread.nextChunks.Enqueue(item3);
		DynamicMeshThread.nextChunks.Enqueue(item4);
		DynamicMeshThread.nextChunks.Enqueue(item5);
		DynamicMeshThread.nextChunks.Enqueue(item6);
		DynamicMeshThread.nextChunks.Enqueue(item7);
		DynamicMeshThread.nextChunks.Enqueue(item8);
	}

	// Token: 0x06001942 RID: 6466 RVA: 0x0009A79C File Offset: 0x0009899C
	public static void RequestChunk(long key)
	{
		DynamicMeshThread.nextChunks.Enqueue(key);
	}

	// Token: 0x06001943 RID: 6467 RVA: 0x0009A7AC File Offset: 0x000989AC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SetNextChunkToLoad()
	{
		if (DynamicMeshThread.nextChunks.Count > 0)
		{
			return;
		}
		if (DynamicMeshThread.PrimaryQueue.Count == 0 && DynamicMeshThread.SecondaryQueue.Count == 0)
		{
			return;
		}
		DynamicMeshManager instance = DynamicMeshManager.Instance;
		if (instance == null)
		{
			return;
		}
		if (instance.NearestRegionWithUnloaded == null)
		{
			if (!instance.FindNearestUnloadedItems)
			{
				instance.FindNearestUnloadedItems = true;
				if (DynamicMeshThread.PrimaryQueue.Count > 0)
				{
					DynamicMeshItem dynamicMeshItem = (from d in DynamicMeshThread.PrimaryQueue
					where d.Value.WorldPosition != d.Value.GetRegionLocation()
					select d.Value).FirstOrDefault<DynamicMeshItem>();
					instance.PrimaryLocation = ((dynamicMeshItem != null) ? new Vector3i?(dynamicMeshItem.WorldPosition) : null);
					return;
				}
				if (instance.PrimaryLocation == null && DynamicMeshThread.SecondaryQueue.Count > 0)
				{
					DynamicMeshItem dynamicMeshItem2 = (from d in DynamicMeshThread.SecondaryQueue
					where d.Value.WorldPosition != d.Value.GetRegionLocation()
					select d.Value).FirstOrDefault<DynamicMeshItem>();
					instance.PrimaryLocation = ((dynamicMeshItem2 != null) ? new Vector3i?(dynamicMeshItem2.WorldPosition) : null);
				}
			}
			return;
		}
		DynamicMeshRegion nearestRegionWithUnloaded = instance.NearestRegionWithUnloaded;
		List<DynamicMeshItem> list = nearestRegionWithUnloaded.UnloadedItems;
		if (!DynamicMeshThread.QueueUpdateOverride && GameManager.Instance.World.ChunkCache.chunks.Count > 600)
		{
			GameManager.Instance.World.m_ChunkManager.ForceUpdate();
		}
		instance.NearestRegionWithUnloaded = null;
		for (int i = 0; i < list.Count; i++)
		{
			DynamicMeshItem dynamicMeshItem3 = list[i];
			if (dynamicMeshItem3 != null)
			{
				long key = dynamicMeshItem3.Key;
				if (DynamicMeshThread.SecondaryQueue.ContainsKey(dynamicMeshItem3.Key))
				{
					DynamicMeshThread.RequestPrimaryQueue(dynamicMeshItem3);
				}
				DynamicMeshThread.SetNextChunks(key);
			}
		}
		list = nearestRegionWithUnloaded.LoadedItems;
		for (int i = 0; i < list.Count; i++)
		{
			DynamicMeshItem dynamicMeshItem4 = list[i];
			long key2 = dynamicMeshItem4.Key;
			if (DynamicMeshThread.SecondaryQueue.ContainsKey(dynamicMeshItem4.Key))
			{
				DynamicMeshThread.RequestPrimaryQueue(dynamicMeshItem4);
			}
			DynamicMeshThread.SetNextChunks(key2);
		}
	}

	// Token: 0x06001944 RID: 6468 RVA: 0x0009AA00 File Offset: 0x00098C00
	public static ConcurrentDictionary<long, DynamicMeshItem> GetQueue(bool isPrimary)
	{
		if (!isPrimary)
		{
			return DynamicMeshThread.SecondaryQueue;
		}
		return DynamicMeshThread.PrimaryQueue;
	}

	// Token: 0x06001945 RID: 6469 RVA: 0x0009AA10 File Offset: 0x00098C10
	public static long GetNextChunkToLoad()
	{
		if (DynamicMeshThread.RequestThreadStop)
		{
			return long.MaxValue;
		}
		if (DynamicMeshThread.nextChunks.Count <= 0)
		{
			return long.MaxValue;
		}
		long result;
		if (!DynamicMeshThread.nextChunks.TryDequeue(out result))
		{
			return long.MaxValue;
		}
		return result;
	}

	// Token: 0x06001946 RID: 6470 RVA: 0x0009AA60 File Offset: 0x00098C60
	public static void RequestSecondaryQueue(DynamicMeshItem item)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			return;
		}
		long key = item.Key;
		if (DynamicMeshThread.PrimaryQueue.ContainsKey(key))
		{
			return;
		}
		if (!DynamicMeshThread.SecondaryQueue.ContainsKey(key))
		{
			DynamicMeshThread.GetThreadRegion(item.WorldPosition);
			DynamicMeshThread.SecondaryQueue.TryAdd(key, item);
			DynamicMeshThread.ChunksToLoad.Add(key);
			DynamicMeshThread.ChunksToProcess.Add(key);
		}
	}

	// Token: 0x06001947 RID: 6471 RVA: 0x0009AACC File Offset: 0x00098CCC
	public static void RequestPrimaryQueue(DynamicMeshItem item)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			return;
		}
		long key = item.Key;
		DynamicMeshThread.GetThreadRegion(item.WorldPosition);
		if (DynamicMeshThread.SecondaryQueue.ContainsKey(key))
		{
			DynamicMeshItem dynamicMeshItem;
			DynamicMeshThread.SecondaryQueue.TryRemove(key, out dynamicMeshItem);
		}
		if (DynamicMeshManager.Instance.PrefabCheck != PrefabCheckState.Run)
		{
			Vector3i regionLocation = item.GetRegionLocation();
			DynamicMeshThread.CheckSecondaryQueueForRegion(regionLocation, item.WorldPosition);
			DynamicMeshThread.CheckSecondaryQueueForRegion(regionLocation + new Vector3i(160, 0, 160), item.WorldPosition);
			DynamicMeshThread.CheckSecondaryQueueForRegion(regionLocation + new Vector3i(160, 0, -160), item.WorldPosition);
			DynamicMeshThread.CheckSecondaryQueueForRegion(regionLocation + new Vector3i(-160, 0, 160), item.WorldPosition);
			DynamicMeshThread.CheckSecondaryQueueForRegion(regionLocation + new Vector3i(-160, 0, -160), item.WorldPosition);
		}
		if (!DynamicMeshThread.PrimaryQueue.ContainsKey(key))
		{
			DynamicMeshThread.PrimaryQueue.TryAdd(key, item);
			DynamicMeshThread.ChunksToLoad.Add(key);
			DynamicMeshThread.ChunksToProcess.Add(key);
		}
	}

	// Token: 0x06001948 RID: 6472 RVA: 0x0009ABEC File Offset: 0x00098DEC
	public static void CheckSecondaryQueueForRegion(Vector3i regionPos, Vector3i itemPos)
	{
		for (int i = regionPos.x; i < regionPos.x + 160; i += 16)
		{
			for (int j = regionPos.z; j < regionPos.z + 160; j += 16)
			{
				long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(i), World.toChunkXZ(j));
				DynamicMeshItem value;
				if (DynamicMeshThread.SecondaryQueue.TryRemove(key, out value))
				{
					DynamicMeshThread.PrimaryQueue.TryAdd(key, value);
				}
			}
		}
	}

	// Token: 0x06001949 RID: 6473 RVA: 0x0009AC61 File Offset: 0x00098E61
	public static void SetDefaultThreads()
	{
		DynamicMeshBuilderManager.MaxBuilderThreads = Math.Min(Math.Min(8, Math.Max(SystemInfo.processorCount - 2, 1)), DynamicMeshSettings.MaxDyMeshData + 1);
	}

	// Token: 0x0600194A RID: 6474 RVA: 0x0009AC88 File Offset: 0x00098E88
	public static void StartThread()
	{
		DynamicMeshThread.StopThreadForce();
		DynamicMeshThread.ClearData();
		DynamicMeshThread.RequestThreadStop = false;
		DynamicMeshThread.ChunkDataQueue = new DynamicMeshChunkDataStorage<DynamicMeshItem>(DynamicMeshThread.CachePurgeInterval);
		if (DynamicMeshThread.ChunkDataQueue.MaxAllowedItems == 0)
		{
			DynamicMeshThread.ChunkDataQueue.MaxAllowedItems = 300;
		}
		DynamicMeshThread.ChunksToLoad.Clear();
		DynamicMeshThread.ChunksToProcess.Clear();
		DynamicMeshThread.SetDefaultThreads();
		DynamicMeshThread.StartTime = DateTime.Now;
		DynamicMeshThread.MeshThread = new Thread(delegate()
		{
			while (GameManager.Instance == null || GameManager.Instance.World == null)
			{
				Thread.Sleep(100);
			}
			Log.Out("Dynamic thread starting");
			DynamicMeshThread.RequestThreadStop = false;
			if (DynamicMeshManager.IsValidGameMode())
			{
				while (!DynamicMeshThread.RequestThreadStop)
				{
					DynamicMeshThread.GenerationThread();
				}
			}
			if (!DynamicMeshThread.RequestThreadStop)
			{
				Log.Error("Dynamic thread stopped");
			}
			DynamicMeshThread.ClearData();
		});
		DynamicMeshThread.MeshThread.Start();
	}

	// Token: 0x0600194B RID: 6475 RVA: 0x0009AD28 File Offset: 0x00098F28
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ClearData()
	{
		DynamicMeshData dynamicMeshData;
		while (DynamicMeshThread.ReadyForCollection.TryDequeue(out dynamicMeshData))
		{
		}
		Vector2i vector2i;
		while (DynamicMeshThread.ChunkReadyForCollection.TryRemoveFirst(out vector2i))
		{
		}
		DynamicMeshThread.RegionStorage.ClearQueues();
		while (DynamicMeshThread.ToGenerate.Count != 0 && DynamicMeshThread.ToGenerate.Dequeue() != null)
		{
		}
		DynamicMeshThread.PrimaryQueue.Clear();
		DynamicMeshThread.SecondaryQueue.Clear();
		DynamicMeshThread.NeedObservers.Clear();
	}

	// Token: 0x0600194C RID: 6476 RVA: 0x0009AD91 File Offset: 0x00098F91
	public static void StopThreadRequest()
	{
		DynamicMeshThread.RequestThreadStop = true;
	}

	// Token: 0x0600194D RID: 6477 RVA: 0x0009AD9C File Offset: 0x00098F9C
	public static void StopThreadForce()
	{
		if (DynamicMeshThread.MeshThread != null)
		{
			try
			{
				DynamicMeshThread.ChunkDataQueue.ClearQueues();
				DynamicMeshThread.MeshThread.Abort();
			}
			catch (Exception ex)
			{
				if (DynamicMeshManager.DoLog)
				{
					DynamicMeshManager.LogMsg("Dynamic Mesh Thread abort error " + ex.Message);
				}
			}
		}
	}

	// Token: 0x0600194E RID: 6478 RVA: 0x0009ADF8 File Offset: 0x00098FF8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void GetKeys(WorldChunkCache cache)
	{
		DynamicMeshThread.Keys.Clear();
		ReaderWriterLockSlim syncRoot = cache.GetSyncRoot();
		syncRoot.EnterReadLock();
		try
		{
			foreach (long item in cache.chunkKeys)
			{
				DynamicMeshThread.Keys.Add(item);
			}
		}
		finally
		{
			syncRoot.ExitReadLock();
		}
	}

	// Token: 0x0600194F RID: 6479 RVA: 0x0009AE7C File Offset: 0x0009907C
	public static void RemoveFromQueues(long key)
	{
		DynamicMeshItem dynamicMeshItem;
		DynamicMeshThread.PrimaryQueue.TryRemove(key, out dynamicMeshItem);
		DynamicMeshThread.SecondaryQueue.TryRemove(key, out dynamicMeshItem);
	}

	// Token: 0x06001950 RID: 6480 RVA: 0x0009AEA8 File Offset: 0x000990A8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void GenerationThread()
	{
		try
		{
			if (DateTime.Now < DynamicMeshThread.NextRun)
			{
				Thread.Sleep((int)Math.Max(1.0, (DynamicMeshThread.NextRun - DateTime.Now).TotalMilliseconds));
			}
			else if (DynamicMeshThread.Paused || DynamicMeshManager.Instance == null)
			{
				DynamicMeshThread.NextRun = DateTime.Now.AddMilliseconds(500.0);
			}
			else
			{
				if (DynamicMeshThread.ChunkDataQueue.ChunkData.Count < DynamicMeshThread.ChunkDataQueue.LiveItems)
				{
					DynamicMeshThread.ChunkDataQueue.LiveItems = DynamicMeshThread.ChunkDataQueue.ChunkData.Count;
				}
				while (DynamicMeshThread.NewlyLoadedGos.Count > 0)
				{
					DynamicMeshThread.LoadedGos.Add(DynamicMeshThread.NewlyLoadedGos.Dequeue());
				}
				while (DynamicMeshThread.ToRemoveGos.Count > 0)
				{
					DynamicMeshThread.LoadedGos.Remove(DynamicMeshThread.ToRemoveGos.Dequeue());
				}
				DynamicMeshThread.HandleRegionLoads();
				DynamicMeshThread.BuilderManager.CheckBuilders();
				DynamicMeshThread.AddRegionChecks = (DynamicMeshThread.PrimaryQueue.Count == 0 && DynamicMeshThread.SecondaryQueue.Count == 0);
				bool hasThreadAvailable = DynamicMeshThread.BuilderManager.HasThreadAvailable;
				if ((DynamicMeshThread.ChunkMeshGenRequests.Count == 0 && DynamicMeshThread.RegionUpdates.Count == 0 && DynamicMeshThread.PrimaryQueue.Count == 0 && DynamicMeshThread.SecondaryQueue.Count == 0 && DynamicMeshThread.ChunkMeshGenRequests.Count == 0) || !hasThreadAvailable)
				{
					if (DynamicMeshSettings.NewWorldFullRegen && hasThreadAvailable && DynamicMeshManager.Instance.PrefabCheck == PrefabCheckState.WaitingForCompleteCheck)
					{
						DynamicMeshThread.WriteChecksComplete();
						DynamicMeshServer.ProcessDelayedPackages();
					}
					DynamicMeshThread.NextRun = DateTime.Now.AddMilliseconds(300.0);
				}
				else
				{
					DynamicMeshThread.SetNextChunkToLoad();
					DynamicMeshThread.ProcessRegionRegenRequests();
					DynamicMeshThread.ProcessMeshGenerationRequests();
					bool flag = false;
					DynamicMeshThread.GetKeys(GameManager.Instance.World.ChunkCache);
					DynamicMeshThread.Queue = DynamicMeshThread.QueuePrimary;
					if (DynamicMeshThread.PrimaryQueue.Count > 0)
					{
						flag = DynamicMeshThread.ProcessQueue(DynamicMeshThread.PrimaryQueue);
					}
					if (!flag)
					{
						if (DynamicMeshThread.PrimaryQueue.Count > 0)
						{
							if (DynamicMeshManager.DoLog)
							{
								Log.Out("Setting chunks");
							}
							foreach (long num in DynamicMeshThread.PrimaryQueue.Keys)
							{
								if (!DynamicMeshThread.Keys.Contains(num) && !DynamicMeshThread.nextChunks.Contains(num))
								{
									DynamicMeshThread.SetNextChunks(num);
								}
							}
						}
						if (DynamicMeshThread.SecondaryQueue.Count > 0)
						{
							DynamicMeshThread.Queue = DynamicMeshThread.QueueSecondary;
							flag = DynamicMeshThread.ProcessQueue(DynamicMeshThread.SecondaryQueue);
						}
					}
					DynamicMeshThread.Queue = DynamicMeshThread.QueueNone;
				}
			}
		}
		catch (Exception ex)
		{
			if (DynamicMeshManager.DoLog)
			{
				DynamicMeshManager.LogMsg("Process requests " + ex.Message + "\n" + ex.StackTrace);
			}
		}
	}

	// Token: 0x06001951 RID: 6481 RVA: 0x0009B1B0 File Offset: 0x000993B0
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool ProcessQueue(ConcurrentDictionary<long, DynamicMeshItem> queue)
	{
		if (DynamicMeshThread.RequestThreadStop)
		{
			return false;
		}
		if (!DynamicMeshThread.BuilderManager.HasThreadAvailable)
		{
			return false;
		}
		bool result = false;
		int num = 0;
		int num2 = 0;
		bool flag = queue == DynamicMeshThread.PrimaryQueue;
		if (DynamicMeshManager.DoLog)
		{
			Log.Out(string.Format("Checking {0} keys", DynamicMeshThread.Keys.Count));
		}
		foreach (long num3 in DynamicMeshThread.Keys)
		{
			if (DynamicMeshThread.RequestThreadStop)
			{
				return false;
			}
			if (DynamicMeshThread.PrimaryQueue.Count > 0 && !flag)
			{
				break;
			}
			DynamicMeshItem dynamicMeshItem;
			if (queue.TryGetValue(num3, out dynamicMeshItem))
			{
				Chunk chunkSync = GameManager.Instance.World.ChunkCache.GetChunkSync(num3);
				if (chunkSync == null || !DynamicMeshChunkProcessor.IsChunkLoaded(chunkSync))
				{
					if (DynamicMeshManager.DoLog)
					{
						Log.Out(dynamicMeshItem.ToDebugLocation() + " not in world cache");
						DynamicMeshThread.nextChunks.Enqueue(num3);
					}
					num++;
				}
				else if (!GameManager.Instance.World.ChunkCache.HasNeighborChunks(chunkSync))
				{
					num2++;
					DynamicMeshThread.SetNextChunks(chunkSync.Key);
					if (DynamicMeshManager.DoLog)
					{
						Log.Out(dynamicMeshItem.ToDebugLocation() + " no neighbours");
					}
				}
				else
				{
					result = true;
					int num4 = DynamicMeshThread.BuilderManager.AddItemForExport(dynamicMeshItem, flag);
					if (num4 == 1)
					{
						DynamicMeshItem dynamicMeshItem2;
						queue.TryRemove(num3, out dynamicMeshItem2);
					}
					if (num4 != -1 && !DynamicMeshThread.BuilderManager.HasThreadAvailable)
					{
						break;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06001952 RID: 6482 RVA: 0x0009B35C File Offset: 0x0009955C
	public static void WriteChecksComplete()
	{
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg("All chunks checked. Disabling future checks");
		}
		SdFile.WriteAllText(DynamicMeshFile.MeshLocation + "!!ChunksChecked.info", DynamicMeshThread.time.ToString());
		DynamicMeshManager.Instance.PrefabCheck = PrefabCheckState.Run;
	}

	// Token: 0x06001953 RID: 6483 RVA: 0x0009B3A6 File Offset: 0x000995A6
	public static void AddChunkGenerationRequest(DynamicMeshItem item)
	{
		DynamicMeshThread.AddRegionChunk(item.WorldPosition.x, item.WorldPosition.z, item.Key);
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		DynamicMeshThread.ChunkMeshGenRequests.Enqueue(item);
	}

	// Token: 0x06001954 RID: 6484 RVA: 0x0009B3DC File Offset: 0x000995DC
	public static void AddRegionLoadRequest(DyMeshRegionLoadRequest request)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (DynamicMeshManager.DoLog)
		{
			Log.Out("Loading region go " + DynamicMeshUnity.GetDebugPositionFromKey(request.Key));
		}
		DynamicMeshThread.RegionFileLoadRequests.Enqueue(request);
	}

	// Token: 0x06001955 RID: 6485 RVA: 0x0009B414 File Offset: 0x00099614
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ProcessMeshGenerationRequests()
	{
		DynamicMeshItem dynamicMeshItem;
		if (!DynamicMeshThread.ChunkMeshGenRequests.TryDequeue(out dynamicMeshItem))
		{
			return;
		}
		DynamicMeshThread.TempChunkMeshGenRequests.Enqueue(dynamicMeshItem);
		DynamicMeshItem dynamicMeshItem2 = dynamicMeshItem;
		float num = dynamicMeshItem.DistanceToPlayer(DynamicMeshThread.PlayerPositionX, DynamicMeshThread.PlayerPositionZ);
		DynamicMeshItem dynamicMeshItem3;
		while (DynamicMeshThread.ChunkMeshGenRequests.TryDequeue(out dynamicMeshItem3))
		{
			if ((DynamicMeshThread.PlayerPositionX != 0f || DynamicMeshThread.PlayerPositionZ != 0f) && !DynamicMeshUnity.IsInBuffer(DynamicMeshThread.PlayerPositionX, DynamicMeshThread.PlayerPositionZ, DynamicMeshRegion.ItemLoadIndex, dynamicMeshItem3.WorldPosition.x / 160, dynamicMeshItem3.WorldPosition.z / 160))
			{
				dynamicMeshItem3.State = DynamicItemState.Waiting;
			}
			else
			{
				DynamicMeshThread.TempChunkMeshGenRequests.Enqueue(dynamicMeshItem3);
				float num2 = dynamicMeshItem3.DistanceToPlayer(DynamicMeshThread.PlayerPositionX, DynamicMeshThread.PlayerPositionZ);
				if (num2 < num)
				{
					num = num2;
					dynamicMeshItem2 = dynamicMeshItem3;
				}
			}
		}
		DynamicMeshItem dynamicMeshItem4;
		while (DynamicMeshThread.TempChunkMeshGenRequests.TryDequeue(out dynamicMeshItem4))
		{
			if (dynamicMeshItem4 != dynamicMeshItem2)
			{
				DynamicMeshThread.ChunkMeshGenRequests.Enqueue(dynamicMeshItem4);
			}
		}
		DynamicMeshThread.GetThreadRegion(dynamicMeshItem2.Key);
		if (!dynamicMeshItem2.FileExists())
		{
			dynamicMeshItem2.State = DynamicItemState.Empty;
			return;
		}
		if (DynamicMeshThread.BuilderManager.AddItemForMeshGeneration(dynamicMeshItem2, false) != 1)
		{
			DynamicMeshThread.ChunkMeshGenRequests.Enqueue(dynamicMeshItem2);
		}
	}

	// Token: 0x06001956 RID: 6486 RVA: 0x0009B540 File Offset: 0x00099740
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ProcessRegionRegenRequests()
	{
		bool useAllThreads = false;
		foreach (KeyValuePair<long, DynamicMeshUpdateData> keyValuePair in DynamicMeshThread.RegionUpdates)
		{
			if (DynamicMeshThread.RequestThreadStop)
			{
				break;
			}
			DynamicMeshUpdateData value = keyValuePair.Value;
			DynamicMeshThread.ThreadRegion threadRegionInternal = DynamicMeshThread.GetThreadRegionInternal(value.Key);
			if (threadRegionInternal.LoadedChunkCount > 0 && value.UpdateTime < DynamicMeshThread.time)
			{
				if (DynamicMeshThread.BuilderManager.RegenerateRegion(threadRegionInternal, useAllThreads) == 1)
				{
					DynamicMeshThread.RegionUpdates.TryRemove(keyValuePair.Key, out value);
					break;
				}
				break;
			}
		}
	}

	// Token: 0x06001957 RID: 6487 RVA: 0x0009B5E4 File Offset: 0x000997E4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void HandleRegionLoads()
	{
		DyMeshRegionLoadRequest dyMeshRegionLoadRequest;
		while (DynamicMeshThread.RegionFileLoadRequests.TryDequeue(out dyMeshRegionLoadRequest))
		{
			if (DynamicMeshManager.DoLog)
			{
				Log.Out("Loading region from storage " + DynamicMeshUnity.GetDebugPositionFromKey(dyMeshRegionLoadRequest.Key));
			}
			DynamicMeshThread.RegionStorage.LoadRegion(dyMeshRegionLoadRequest);
			DynamicMeshManager.Instance.RegionFileLoadRequests.Enqueue(dyMeshRegionLoadRequest);
		}
	}

	// Token: 0x04001025 RID: 4133
	public static bool Paused = false;

	// Token: 0x04001026 RID: 4134
	public static bool NoProcessing = false;

	// Token: 0x04001027 RID: 4135
	public static bool LockMeshesAfterGenerating = true;

	// Token: 0x04001028 RID: 4136
	[PublicizedFrom(EAccessModifier.Private)]
	public static string QueuePrimary = "Primary";

	// Token: 0x04001029 RID: 4137
	[PublicizedFrom(EAccessModifier.Private)]
	public static string QueueSecondary = "Secondary";

	// Token: 0x0400102A RID: 4138
	[PublicizedFrom(EAccessModifier.Private)]
	public static string QueueNone = "None";

	// Token: 0x0400102B RID: 4139
	public static string Queue;

	// Token: 0x0400102C RID: 4140
	public static string Processed;

	// Token: 0x0400102D RID: 4141
	public static ChunkQueue ChunksToLoad = new ChunkQueue();

	// Token: 0x0400102E RID: 4142
	public static ConcurrentHashSet<long> ChunksToProcess = new ConcurrentHashSet<long>();

	// Token: 0x0400102F RID: 4143
	public static ConcurrentQueue<long> nextChunks = new ConcurrentQueue<long>();

	// Token: 0x04001030 RID: 4144
	public static bool QueueUpdateOverride;

	// Token: 0x04001031 RID: 4145
	public static bool RequestThreadStop = false;

	// Token: 0x04001032 RID: 4146
	public static bool AddRegionChecks = false;

	// Token: 0x04001033 RID: 4147
	public static int CachePurgeInterval = 8;

	// Token: 0x04001034 RID: 4148
	public static List<List<Vector3i>> RegionsToCheck = null;

	// Token: 0x04001035 RID: 4149
	public static DynamicMeshRegionDataStorage RegionStorage = new DynamicMeshRegionDataStorage();

	// Token: 0x04001036 RID: 4150
	public static DynamicMeshChunkDataStorage<DynamicMeshItem> ChunkDataQueue = new DynamicMeshChunkDataStorage<DynamicMeshItem>(DynamicMeshThread.CachePurgeInterval);

	// Token: 0x04001037 RID: 4151
	public static ConcurrentQueue<DynamicMeshItem> ChunkMeshGenRequests = new ConcurrentQueue<DynamicMeshItem>();

	// Token: 0x04001038 RID: 4152
	[PublicizedFrom(EAccessModifier.Private)]
	public static ConcurrentQueue<DynamicMeshItem> TempChunkMeshGenRequests = new ConcurrentQueue<DynamicMeshItem>();

	// Token: 0x04001039 RID: 4153
	[PublicizedFrom(EAccessModifier.Private)]
	public static ConcurrentQueue<DyMeshRegionLoadRequest> RegionFileLoadRequests = new ConcurrentQueue<DyMeshRegionLoadRequest>();

	// Token: 0x0400103A RID: 4154
	public static DynamicMeshBuilderManager BuilderManager = DynamicMeshBuilderManager.GetOrCreate();

	// Token: 0x0400103B RID: 4155
	public static string RegionUpdatesDebug = "";

	// Token: 0x0400103C RID: 4156
	public static ConcurrentDictionary<long, DynamicMeshUpdateData> RegionUpdates = new ConcurrentDictionary<long, DynamicMeshUpdateData>();

	// Token: 0x0400103D RID: 4157
	public static ConcurrentQueue<DynamicMeshData> ReadyForCollection = new ConcurrentQueue<DynamicMeshData>();

	// Token: 0x0400103E RID: 4158
	public static ConcurrentHashSet<Vector2i> ChunkReadyForCollection = new ConcurrentHashSet<Vector2i>();

	// Token: 0x0400103F RID: 4159
	public static float PlayerPositionX;

	// Token: 0x04001040 RID: 4160
	public static float PlayerPositionZ;

	// Token: 0x04001041 RID: 4161
	public static Queue<DynamicMeshItem> ToGenerate = new Queue<DynamicMeshItem>();

	// Token: 0x04001042 RID: 4162
	[PublicizedFrom(EAccessModifier.Private)]
	public static LinkedList<DynamicMeshItem> NeedObservers = new LinkedList<DynamicMeshItem>();

	// Token: 0x04001043 RID: 4163
	[PublicizedFrom(EAccessModifier.Private)]
	public static LinkedList<DynamicMeshItem> IgnoredChunks = new LinkedList<DynamicMeshItem>();

	// Token: 0x04001044 RID: 4164
	public static ConcurrentDictionary<long, DynamicMeshItem> PrimaryQueue = new ConcurrentDictionary<long, DynamicMeshItem>();

	// Token: 0x04001045 RID: 4165
	public static ConcurrentDictionary<long, DynamicMeshItem> SecondaryQueue = new ConcurrentDictionary<long, DynamicMeshItem>();

	// Token: 0x04001046 RID: 4166
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<ChunkGameObject> LoadedGos = new List<ChunkGameObject>();

	// Token: 0x04001047 RID: 4167
	[PublicizedFrom(EAccessModifier.Private)]
	public static Queue<ChunkGameObject> NewlyLoadedGos = new Queue<ChunkGameObject>();

	// Token: 0x04001048 RID: 4168
	[PublicizedFrom(EAccessModifier.Private)]
	public static Queue<ChunkGameObject> ToRemoveGos = new Queue<ChunkGameObject>();

	// Token: 0x04001049 RID: 4169
	public static ConcurrentDictionary<long, DynamicMeshThread.ThreadRegion> threadRegions = new ConcurrentDictionary<long, DynamicMeshThread.ThreadRegion>();

	// Token: 0x0400104A RID: 4170
	public static Queue<DynamicMeshServerUpdates> ServerUpdates = new Queue<DynamicMeshServerUpdates>(20);

	// Token: 0x0400104B RID: 4171
	[PublicizedFrom(EAccessModifier.Private)]
	public static Thread MeshThread;

	// Token: 0x0400104C RID: 4172
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<long> Keys = new List<long>(50);

	// Token: 0x0400104D RID: 4173
	[PublicizedFrom(EAccessModifier.Private)]
	public static DateTime NextRun = DateTime.Now;

	// Token: 0x0400104E RID: 4174
	[PublicizedFrom(EAccessModifier.Private)]
	public static DateTime StartTime = DateTime.Now;

	// Token: 0x0200035D RID: 861
	public class ThreadRegion
	{
		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x06001959 RID: 6489 RVA: 0x0009B7A2 File Offset: 0x000999A2
		public int LoadedChunkCount
		{
			get
			{
				return this.LoadedChunks.Count;
			}
		}

		// Token: 0x0600195A RID: 6490 RVA: 0x0009B7B0 File Offset: 0x000999B0
		public void AddLoadedChunk(long key)
		{
			object obj = this.chunkListLock;
			lock (obj)
			{
				this.LoadedChunks.Add(key);
			}
		}

		// Token: 0x0600195B RID: 6491 RVA: 0x0009B7F8 File Offset: 0x000999F8
		public bool RemoveLoadedChunk(long key)
		{
			object obj = this.chunkListLock;
			bool result;
			lock (obj)
			{
				result = this.LoadedChunks.TryRemove(key);
			}
			return result;
		}

		// Token: 0x0600195C RID: 6492 RVA: 0x0009B840 File Offset: 0x00099A40
		public void CopyLoadedChunks(List<long> chunks)
		{
			chunks.Clear();
			object obj = this.chunkListLock;
			lock (obj)
			{
				chunks.AddRange(this.LoadedChunks);
			}
		}

		// Token: 0x0600195D RID: 6493 RVA: 0x0009B88C File Offset: 0x00099A8C
		public ThreadRegion(long key)
		{
			this.Key = key;
			this.xIndex = WorldChunkCache.extractX(key);
			this.zIndex = WorldChunkCache.extractZ(key);
			this.X = this.xIndex * 16;
			this.Z = this.zIndex * 16;
		}

		// Token: 0x0600195E RID: 6494 RVA: 0x0009B8FD File Offset: 0x00099AFD
		public Vector3i ToWorldPosition()
		{
			return new Vector3i(this.X, 0, this.Z);
		}

		// Token: 0x0600195F RID: 6495 RVA: 0x0009B911 File Offset: 0x00099B11
		public string ToDebugLocation()
		{
			return string.Format("R:{0} {1}", this.X, this.Z);
		}

		// Token: 0x06001960 RID: 6496 RVA: 0x0009B934 File Offset: 0x00099B34
		public bool IsInItemLoad(float playerX, float playerZ)
		{
			return !(GameManager.Instance == null) && GameManager.Instance.World != null && !(GameManager.Instance.World.GetPrimaryPlayer() == null) && DynamicMeshUnity.IsInBuffer(playerX, playerZ, DynamicMeshRegion.ItemLoadIndex, this.xIndex, this.zIndex);
		}

		// Token: 0x0400104F RID: 4175
		public int X;

		// Token: 0x04001050 RID: 4176
		public int Z;

		// Token: 0x04001051 RID: 4177
		public long Key;

		// Token: 0x04001052 RID: 4178
		[PublicizedFrom(EAccessModifier.Private)]
		public ConcurrentHashSet<long> LoadedChunks = new ConcurrentHashSet<long>();

		// Token: 0x04001053 RID: 4179
		[PublicizedFrom(EAccessModifier.Private)]
		public int xIndex;

		// Token: 0x04001054 RID: 4180
		[PublicizedFrom(EAccessModifier.Private)]
		public int zIndex;

		// Token: 0x04001055 RID: 4181
		public bool IsRegerating;

		// Token: 0x04001056 RID: 4182
		public float UpdateTime = float.MaxValue;

		// Token: 0x04001057 RID: 4183
		[PublicizedFrom(EAccessModifier.Private)]
		public object chunkListLock = new object();
	}
}
