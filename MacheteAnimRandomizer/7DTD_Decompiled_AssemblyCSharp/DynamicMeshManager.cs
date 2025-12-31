using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ConcurrentCollections;
using UniLinq;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000332 RID: 818
public class DynamicMeshManager : MonoBehaviour
{
	// Token: 0x170002AC RID: 684
	// (get) Token: 0x060017D6 RID: 6102 RVA: 0x0009127F File Offset: 0x0008F47F
	public static Transform ParentTransform
	{
		get
		{
			if (!(DynamicMeshManager.Instance == null))
			{
				return DynamicMeshManager.Instance.transform;
			}
			return null;
		}
	}

	// Token: 0x060017D7 RID: 6103 RVA: 0x0009129A File Offset: 0x0008F49A
	public void ForceOrphanChecks()
	{
		base.StartCoroutine(this.DoubleOrphanCheck());
	}

	// Token: 0x060017D8 RID: 6104 RVA: 0x000912A9 File Offset: 0x0008F4A9
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator DoubleOrphanCheck()
	{
		yield return base.StartCoroutine(this.CheckForOrphans(false));
		yield return base.StartCoroutine(this.CheckForOrphans(false));
		yield break;
	}

	// Token: 0x060017D9 RID: 6105 RVA: 0x000912B8 File Offset: 0x0008F4B8
	public IEnumerator CheckForOrphans(bool debugLogOnly)
	{
		int returns = 0;
		DateTime t = DateTime.Now.AddMilliseconds(2.0);
		this.checkForOrphans.Clear();
		for (int i = 0; i < DynamicMeshManager.ParentTransform.childCount; i++)
		{
			Transform child = DynamicMeshManager.ParentTransform.GetChild(i);
			this.checkForOrphans.Add(child.gameObject);
		}
		foreach (DynamicMeshItem dynamicMeshItem in this.ItemsDictionary.Values)
		{
			if (!(dynamicMeshItem.ChunkObject == null))
			{
				this.checkForOrphans.Remove(dynamicMeshItem.ChunkObject);
				this.potentialOrphans.Remove(dynamicMeshItem.ChunkObject);
				if (t < DateTime.Now)
				{
					int num = returns;
					returns = num + 1;
					yield return null;
					t = DateTime.Now.AddMilliseconds(2.0);
				}
			}
		}
		IEnumerator<DynamicMeshItem> enumerator = null;
		foreach (DynamicMeshRegion dynamicMeshRegion in DynamicMeshRegion.Regions.Values)
		{
			if (!(dynamicMeshRegion.RegionObject == null))
			{
				this.checkForOrphans.Remove(dynamicMeshRegion.RegionObject);
				this.potentialOrphans.Remove(dynamicMeshRegion.RegionObject);
				if (t < DateTime.Now)
				{
					int num = returns;
					returns = num + 1;
					yield return null;
					t = DateTime.Now.AddMilliseconds(2.0);
				}
			}
		}
		IEnumerator<DynamicMeshRegion> enumerator2 = null;
		using (HashSet<GameObject>.Enumerator enumerator3 = this.checkForOrphans.GetEnumerator())
		{
			while (enumerator3.MoveNext())
			{
				GameObject gameObject = enumerator3.Current;
				if (this.potentialOrphans.Contains(gameObject))
				{
					Log.Warning("Found orphaned mesh in dymesh parent " + gameObject.name);
					if (!debugLogOnly)
					{
						UnityEngine.Object.Destroy(gameObject);
					}
					this.potentialOrphans.Remove(gameObject);
				}
				else
				{
					this.potentialOrphans.Add(gameObject);
				}
			}
			yield break;
		}
		yield break;
		yield break;
	}

	// Token: 0x060017DA RID: 6106 RVA: 0x000912CE File Offset: 0x0008F4CE
	public static void EnabledChanged(bool newvalue)
	{
		if (GameManager.Instance == null)
		{
			return;
		}
		if (GameManager.Instance.World == null)
		{
			return;
		}
		DynamicMeshManager.OnWorldUnload();
		if (newvalue)
		{
			DynamicMeshManager.Init();
		}
	}

	// Token: 0x060017DB RID: 6107 RVA: 0x000912F8 File Offset: 0x0008F4F8
	public void AddObjectForDestruction(GameObject go)
	{
		this.ToBeDestroyed.Enqueue(go);
	}

	// Token: 0x060017DC RID: 6108 RVA: 0x00091308 File Offset: 0x0008F508
	public static void AddRegionLoadMeshes(long key)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		using (LinkedList<long>.Enumerator enumerator = DynamicMeshManager.Instance.RegionsAvailableToLoad.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == key)
				{
					return;
				}
			}
		}
		DynamicMeshManager.Instance.RegionsAvailableToLoad.AddLast(key);
	}

	// Token: 0x060017DD RID: 6109 RVA: 0x00091378 File Offset: 0x0008F578
	public void AddChunkLoadData(DynamicMeshVoxelLoad loadData)
	{
		object chunkMeshDataLock = this._chunkMeshDataLock;
		lock (chunkMeshDataLock)
		{
			DynamicMeshManager.Instance.ChunkMeshData.AddLast(loadData);
		}
	}

	// Token: 0x060017DF RID: 6111 RVA: 0x000914A4 File Offset: 0x0008F6A4
	public static bool IsOutsideDistantTerrain(DynamicMeshRegion region)
	{
		return DynamicMeshManager.CONTENT_ENABLED && !(DynamicMeshManager.Instance == null) && (region.Rect.xMin < (float)DynamicMeshManager.dtMinX || region.Rect.xMax > (float)DynamicMeshManager.dtMaxX || region.Rect.yMin < (float)DynamicMeshManager.dtMinZ || region.Rect.yMax > (float)DynamicMeshManager.dtMaxZ);
	}

	// Token: 0x060017E0 RID: 6112 RVA: 0x00091521 File Offset: 0x0008F721
	public static bool IsOutsideDistantTerrain(float minx, float maxx, float minz, float maxz)
	{
		return DynamicMeshManager.CONTENT_ENABLED && !(DynamicMeshManager.Instance == null) && (minx < (float)DynamicMeshManager.dtMinX || maxx > (float)DynamicMeshManager.dtMaxX || minz < (float)DynamicMeshManager.dtMinZ || maxz > (float)DynamicMeshManager.dtMaxZ);
	}

	// Token: 0x060017E1 RID: 6113 RVA: 0x00091560 File Offset: 0x0008F760
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void UpdateDistantTerrainBounds(TileArea<UnityDistantTerrain.TerrainAndWater> data, UnityDistantTerrain.Config terrainConfig)
	{
		if (DynamicMeshManager.Instance == null)
		{
			return;
		}
		int num = int.MaxValue;
		int num2 = int.MaxValue;
		int num3 = int.MinValue;
		int num4 = int.MinValue;
		if (data.Data.Count == 0)
		{
			DynamicMeshManager.dtMinX = int.MinValue;
			DynamicMeshManager.dtMinZ = int.MinValue;
			DynamicMeshManager.dtMaxX = -2147482624;
			DynamicMeshManager.dtMaxZ = -2147482624;
			return;
		}
		foreach (uint key in data.Data.Keys)
		{
			int tileXPos = TileAreaUtils.GetTileXPos(key);
			int tileZPos = TileAreaUtils.GetTileZPos(key);
			num = Math.Min(num, tileXPos);
			num2 = Math.Min(num2, tileZPos);
			num3 = Math.Max(num3, tileXPos);
			num4 = Math.Max(num4, tileZPos);
		}
		int dataTileSize = terrainConfig.DataTileSize;
		num *= dataTileSize;
		num2 *= dataTileSize;
		num3 *= dataTileSize;
		num4 *= dataTileSize;
		num3 += dataTileSize;
		num4 += dataTileSize;
		if (num != DynamicMeshManager.dtMinX || num3 != DynamicMeshManager.dtMaxX || DynamicMeshManager.dtMinZ != num2 || DynamicMeshManager.dtMaxZ != num4)
		{
			DynamicMeshManager.dtMinX = num;
			DynamicMeshManager.dtMaxX = num3;
			DynamicMeshManager.dtMinZ = num2;
			DynamicMeshManager.dtMaxZ = num4;
			DynamicMeshManager.Instance.ShowOrHidePrefabs();
			GameManager.Instance.prefabLODManager.TriggerUpdate();
		}
	}

	// Token: 0x060017E2 RID: 6114 RVA: 0x000916B8 File Offset: 0x0008F8B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void DestroyObjects()
	{
		GameObject gameObject;
		while (this.ToBeDestroyed.TryDequeue(out gameObject))
		{
			if (!(gameObject == null))
			{
				DynamicMeshContainer currentlyLoadingItem = DynamicMeshFile.CurrentlyLoadingItem;
				if (((currentlyLoadingItem != null) ? currentlyLoadingItem.GetGameObject() : null) == gameObject)
				{
					Log.Warning("Object in use when destroying... delaying");
					this.ToBeDestroyed.Enqueue(gameObject);
					return;
				}
				gameObject.SetActive(false);
				DynamicMeshManager.MeshDestroy(gameObject);
			}
		}
	}

	// Token: 0x060017E3 RID: 6115 RVA: 0x00002914 File Offset: 0x00000B14
	public static void WarmUp()
	{
	}

	// Token: 0x060017E4 RID: 6116 RVA: 0x0009171C File Offset: 0x0008F91C
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator SendClientMessage()
	{
		while (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
		{
			yield return new WaitForSeconds(1f);
		}
		this.ClientMessage = DynamicMeshServerStatus.ClientMessageSent;
		NetPackageDynamicClientArrive package = NetPackageManager.GetPackage<NetPackageDynamicClientArrive>();
		package.BuildData();
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
		DynamicMeshManager.LogMsg("Sending client arrive message. Items: " + package.Items.Count.ToString());
		yield break;
	}

	// Token: 0x060017E5 RID: 6117 RVA: 0x0009172B File Offset: 0x0008F92B
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ProcessChunkRegionRequests()
	{
		this.ProcessRegionReady = false;
		this.ForceNextRegion = DateTime.Now.AddSeconds(60.0);
		DyMeshRegionLoadRequest load = null;
		if (this.RegionFileLoadRequests.Count != 0 && this.RegionFileLoadRequests.TryDequeue(out load))
		{
			if (this.PrefabCheck == PrefabCheckState.Waiting)
			{
				this.PrefabCheck = PrefabCheckState.Ready;
			}
			DynamicMeshRegion region = this.GetRegion(load.Key);
			if (DynamicMeshManager.DoLog)
			{
				Log.Out("Loading region go-process " + region.ToDebugLocation());
			}
			if (region != null)
			{
				if (region.OutsideLoadArea)
				{
					region.SetState(DynamicRegionState.Unloaded, false);
				}
				else
				{
					yield return base.StartCoroutine(load.CreateMeshCoroutine(region));
					if (region.RegionObject != null)
					{
						region.SetState(DynamicRegionState.Loaded, false);
						bool flag = !region.InBuffer && region.LoadedItems.Count > 0 && region.UnloadedItems.Count == 0 && region.RegionObject != null && region.RegionObject.activeSelf;
						region.LoadItems(false, !flag, false, "regionLoadRequest");
						this.UpdateDynamicPrefabDecoratorRegions(region);
					}
					region = null;
				}
			}
		}
		if (load != null)
		{
			MeshLists.ReturnList(load.OpaqueMesh);
			MeshLists.ReturnList(load.TerrainMesh);
		}
		this.AvailableRegionLoadRequests = Math.Min(DynamicMeshSettings.MaxRegionMeshData, this.AvailableRegionLoadRequests + 1);
		this.ProcessRegionReady = true;
		yield break;
	}

	// Token: 0x060017E6 RID: 6118 RVA: 0x0009173A File Offset: 0x0008F93A
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ProcessItemMeshGeneration()
	{
		this.ProcessItemLoadReady = false;
		this.ForceNextItemLoad = DateTime.Now.AddSeconds(30.0);
		DynamicMeshVoxelLoad voxelData = null;
		if (this.ChunkMeshData.Count != 0)
		{
			bool flag = false;
			Monitor.TryEnter(this._chunkMeshDataLock, 1, ref flag);
			if (flag)
			{
				float num = 9999999f;
				foreach (DynamicMeshVoxelLoad dynamicMeshVoxelLoad in this.ChunkMeshData)
				{
					float num2 = dynamicMeshVoxelLoad.Item.DistanceToPlayer();
					if (num2 < num)
					{
						voxelData = dynamicMeshVoxelLoad;
						num = num2;
						if (num2 < 50f)
						{
							break;
						}
					}
				}
				if (voxelData != null)
				{
					this.ChunkMeshData.Remove(voxelData);
				}
				Monitor.Exit(this._chunkMeshDataLock);
				if (voxelData != null)
				{
					DynamicMeshItem item = this.GetItemOrNull(voxelData.Item.Key);
					if (item != null)
					{
						DynamicMeshRegion region = this.GetRegion(item);
						region.RemoveFromLoadingQueue(item);
						if (!region.IsInItemLoad())
						{
							item.State = DynamicItemState.Waiting;
						}
						else
						{
							item.State = DynamicItemState.Loading;
							if (DynamicMeshManager.DoLog)
							{
								Log.Out("Item " + item.ToDebugLocation() + " loading request");
							}
							DynamicMeshManager.ItemLoadDistance = item.DistanceToPlayer();
							yield return base.StartCoroutine(item.CreateMeshFromVoxelCoroutine(false, DynamicMeshManager.MeshLoadStop, voxelData));
							region.AddChunk(item.WorldPosition.x, item.WorldPosition.z);
							if (region.WorldPosition.x != DynamicMeshUnity.RoundRegion(item.WorldPosition.x))
							{
								string[] array = new string[6];
								array[0] = "Region mismatch: ";
								int num3 = 1;
								Vector3i worldPosition = region.WorldPosition;
								array[num3] = worldPosition.ToString();
								array[2] = " vs ";
								array[3] = (item.WorldPosition.x / 16 * 16).ToString();
								array[4] = " for  ";
								int num4 = 5;
								worldPosition = item.WorldPosition;
								array[num4] = worldPosition.ToString();
								Log.Error(string.Concat(array));
							}
							if (item.State != DynamicItemState.ReadyToDelete)
							{
								region.AddItemToLoadedList(item);
								bool flag2 = region.IsVisible();
								item.SetVisible(!flag2 && !item.IsChunkInView, "loadItemRequest");
								item.State = DynamicItemState.Loaded;
							}
						}
					}
				}
			}
		}
		this.ProcessItemLoadReady = true;
		if (voxelData != null)
		{
			voxelData.DisposeMeshes();
		}
		yield break;
	}

	// Token: 0x060017E7 RID: 6119 RVA: 0x0009174C File Offset: 0x0008F94C
	public void ForceLoadDataAroundPosition(Vector3i pos, int regionRadius)
	{
		if (!DynamicMeshManager.CONTENT_ENABLED)
		{
			return;
		}
		DateTime now = DateTime.Now;
		DynamicMeshManager.CONTENT_ENABLED = false;
		foreach (DynamicMeshRegion dynamicMeshRegion in DynamicMeshRegion.Regions.Values)
		{
			if (dynamicMeshRegion != null)
			{
				dynamicMeshRegion.DistanceChecks();
			}
		}
		DynamicMeshManager.CONTENT_ENABLED = true;
		DyMeshRegionLoadRequest dyMeshRegionLoadRequest = DyMeshRegionLoadRequest.Create(0L);
		foreach (long key in this.RegionsAvailableToLoad)
		{
			dyMeshRegionLoadRequest.Key = key;
			DynamicMeshRegion region = this.GetRegion(dyMeshRegionLoadRequest.Key);
			if (region != null)
			{
				if (region.OutsideLoadArea || region.DistanceToPlayer() > 1000f)
				{
					region.SetState(DynamicRegionState.Unloaded, false);
				}
				else
				{
					dyMeshRegionLoadRequest.OpaqueMesh.Reset();
					dyMeshRegionLoadRequest.TerrainMesh.Reset();
					DynamicMeshThread.RegionStorage.LoadRegion(dyMeshRegionLoadRequest);
					dyMeshRegionLoadRequest.CreateMeshSync(region);
					if (region.RegionObject != null)
					{
						region.SetState(DynamicRegionState.Loaded, false);
						bool flag = !region.InBuffer && region.LoadedItems.Count > 0 && region.UnloadedItems.Count == 0 && region.RegionObject != null && region.RegionObject.activeSelf;
						region.LoadItems(false, !flag, false, "regionLoadRequest");
						this.UpdateDynamicPrefabDecoratorRegions(region);
					}
				}
			}
		}
		MeshLists.ReturnList(dyMeshRegionLoadRequest.OpaqueMesh);
		MeshLists.ReturnList(dyMeshRegionLoadRequest.TerrainMesh);
		this.RegionsAvailableToLoad.Clear();
		foreach (ChunkGameObject chunkGameObject in GameManager.Instance.World.ChunkClusters[0].DisplayedChunkGameObjects.Dict.Values)
		{
			if (!DynamicMeshManager.ChunkGameObjects.Contains(chunkGameObject.chunk.Key))
			{
				DynamicMeshThread.AddChunkGameObject(chunkGameObject.chunk);
			}
		}
		Log.Out("Force load took " + ((int)(DateTime.Now - now).TotalSeconds).ToString() + " seconds");
	}

	// Token: 0x060017E8 RID: 6120 RVA: 0x000919E4 File Offset: 0x0008FBE4
	public void ClearPrefabs()
	{
		if (base.gameObject != null)
		{
			foreach (object obj in base.gameObject.transform)
			{
				UnityEngine.Object.Destroy(((Transform)obj).gameObject);
			}
		}
		if (this.ItemsDictionary != null)
		{
			this.ItemsDictionary.Clear();
			DynamicMeshRegion.Regions.Clear();
		}
	}

	// Token: 0x060017E9 RID: 6121 RVA: 0x00091A70 File Offset: 0x0008FC70
	public void Cleanup()
	{
		DynamicMeshThread.StopThreadRequest();
		this.ClearPrefabs();
		base.StopAllCoroutines();
		Vector2i vector2i;
		while (this.ChunksToRemove.TryDequeue(out vector2i))
		{
		}
		foreach (DynamicMeshRegion dynamicMeshRegion in DynamicMeshRegion.Regions.Values)
		{
			dynamicMeshRegion.CleanUp();
		}
		this.ItemsDictionary.Clear();
		this.UpdateData.Clear();
		this.ChunkMeshData.Clear();
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			this.ToBeDestroyed.Enqueue(transform.gameObject);
		}
		this.DestroyObjects();
		this.disabledImposterChunkManager.Dispose();
	}

	// Token: 0x060017EA RID: 6122 RVA: 0x00091B68 File Offset: 0x0008FD68
	public bool HalEventChunkChanged(object chunkObject)
	{
		if (chunkObject == null || !(chunkObject is Chunk))
		{
			return false;
		}
		DynamicMeshManager.ChunkChanged(((Chunk)chunkObject).GetWorldPos(), -1, 1);
		return true;
	}

	// Token: 0x060017EB RID: 6123 RVA: 0x00091B8C File Offset: 0x0008FD8C
	public static bool IsValidGameMode()
	{
		GameManager instance = GameManager.Instance;
		if (((instance != null) ? instance.World : null) == null)
		{
			Log.Out("GM or World is not initialised yet for dynamic mesh");
			return false;
		}
		EnumGameMode gameMode = (EnumGameMode)GameManager.Instance.World.GetGameMode();
		return !GameManager.Instance.IsEditMode() && !GameUtils.IsPlaytesting() && gameMode != EnumGameMode.Creative && gameMode != EnumGameMode.EditWorld;
	}

	// Token: 0x060017EC RID: 6124 RVA: 0x00091BE8 File Offset: 0x0008FDE8
	public void Awake()
	{
		DynamicMeshManager.LogMsg("Awake");
		DynamicMeshManager.CONTENT_ENABLED = GamePrefs.GetBool(EnumGamePrefs.DynamicMeshEnabled);
		if (!DynamicMeshManager.CONTENT_ENABLED)
		{
			Log.Out("Dynamic mesh disabled");
			return;
		}
		if (DynamicMeshManager.backgroundTexture == null)
		{
			DynamicMeshManager.backgroundTexture = Texture2D.blackTexture;
		}
		DynamicMeshFile.TerrainSharedMaterials.Clear();
		if (DynamicMeshManager.Instance != null)
		{
			DynamicMeshManager.Instance.StopAllCoroutines();
			this.ClearPrefabs();
			if (DynamicMeshManager.Instance != this)
			{
				UnityEngine.Object.Destroy(DynamicMeshManager.Instance);
			}
			DynamicMeshManager.Instance = null;
		}
		if (!DynamicMeshManager.IsValidGameMode())
		{
			Log.Out("Dynamic Mesh will not run in this game mode");
			return;
		}
		DynamicMeshManager.Instance = this;
		DynamicMeshFile.MeshLocation = (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? (GameIO.GetSaveGameDir() + "/DynamicMeshes/") : (GameIO.GetSaveGameLocalDir() + "/DynamicMeshes/"));
		DynamicMeshManager.LogMsg("Mesh location: " + DynamicMeshFile.MeshLocation);
		this.BufferRegionLoadRequests.Clear();
		this.AvailableRegionLoadRequests = DynamicMeshSettings.MaxRegionMeshData;
		DyMeshData.ActiveItems = 0;
		this.ChunkMeshData.Clear();
		this.ChunkMeshLoadRequests.Clear();
		ConnectionManager.OnClientDisconnected -= DynamicMeshServer.OnClientDisconnect;
		ConnectionManager.OnClientDisconnected += DynamicMeshServer.OnClientDisconnect;
		this.UpdateData.Clear();
		this.ItemsDictionary = new ConcurrentDictionary<long, DynamicMeshItem>();
		DynamicMeshThread.StopThreadForce();
		if (!DynamicMeshManager.CONTENT_ENABLED)
		{
			DynamicMeshManager.LogMsg("Disabled");
			return;
		}
		DateTime now = DateTime.Now;
		this.LoadItemsDedicated();
		DynamicMeshManager.LogMsg("Loading all items took: " + (DateTime.Now - now).TotalSeconds.ToString() + " seconds.");
		this.disabledImposterChunkManager = new DynamicMeshManager.DisabledImposterChunkManager(this);
		if (!GameManager.IsDedicatedServer)
		{
			if (DynamicMeshManager.player != null)
			{
				DynamicMeshThread.PlayerPositionX = DynamicMeshManager.player.position.x;
				DynamicMeshThread.PlayerPositionZ = DynamicMeshManager.player.position.z;
				this.ForceLoadDataAroundPosition(new Vector3i(DynamicMeshManager.player.position), DynamicMeshSettings.MaxViewDistance / 2);
			}
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				if (DynamicMeshManager.DoLog)
				{
					DynamicMeshManager.LogMsg("Saying hello to server. Regions: " + DynamicMeshRegion.Regions.Values.Count.ToString());
				}
				this.ShowOrHidePrefabs();
			}
		}
		DynamicMeshThread.StartThread();
		this.PrefabCheck = (SdFile.Exists(DynamicMeshFile.MeshLocation + "!!ChunksChecked.info") ? PrefabCheckState.Run : PrefabCheckState.Waiting);
		if (this.PrefabCheck != PrefabCheckState.Run)
		{
			this.CheckPrefabs("thread Started", false);
		}
		MeshDescription.meshes[0].bTextureArray = true;
	}

	// Token: 0x060017ED RID: 6125 RVA: 0x00091E84 File Offset: 0x00090084
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ShowErrorStackTraces(string _msg, string _trace, LogType _type)
	{
		if (_type == LogType.Error || _type == LogType.Exception)
		{
			Log.Out(string.Concat(new string[]
			{
				"Callback ",
				_type.ToString(),
				":",
				_msg,
				" | ",
				_trace
			}));
		}
	}

	// Token: 0x060017EE RID: 6126 RVA: 0x00091ED8 File Offset: 0x000900D8
	public static void OnWorldUnload()
	{
		DynamicMeshThread.StopThreadRequest();
		DateTime t = DateTime.Now.AddSeconds(1.0);
		while (DynamicMeshThread.RequestThreadStop && DateTime.Now < t)
		{
			Thread.Sleep(100);
		}
		DynamicMeshThread.CleanUp();
		DynamicMeshManager.ChunkGameObjects.Clear();
		if (DynamicMeshManager.Instance != null)
		{
			DynamicMeshManager.Instance.Cleanup();
		}
		DynamicMeshFile.CleanUp();
		DynamicMeshFile.MeshLocation = null;
	}

	// Token: 0x170002AD RID: 685
	// (get) Token: 0x060017EF RID: 6127 RVA: 0x00091F4F File Offset: 0x0009014F
	public static bool IsServer
	{
		get
		{
			return SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer;
		}
	}

	// Token: 0x060017F0 RID: 6128 RVA: 0x00091F5C File Offset: 0x0009015C
	public void CheckPrefabsInRegion(int x, int z)
	{
		int xMin = DynamicMeshUnity.RoundRegion(x);
		int zMin = DynamicMeshUnity.RoundRegion(z);
		int xMax = xMin + 160;
		int zMax = zMin + 160;
		DynamicMeshManager.LogMsg("Checking prefabs in region");
		this.PrefabCheck = PrefabCheckState.Warming;
		DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.World.ChunkClusters[0].ChunkProvider.GetDynamicPrefabDecorator();
		EntityPlayerLocal player = DynamicMeshManager.player;
		if (dynamicPrefabDecorator == null)
		{
			return;
		}
		if (player == null)
		{
			return;
		}
		Vector3 playerPos = new Vector3((float)x, 0f, (float)z);
		List<PrefabInstance> list = (from d in dynamicPrefabDecorator.allPrefabs
		where d.boundingBoxPosition.x >= xMin && d.boundingBoxPosition.x + d.boundingBoxSize.x < xMax && d.boundingBoxPosition.z >= zMin && d.boundingBoxPosition.z + d.boundingBoxSize.z < zMax
		orderby Math.Abs(Vector3.Distance(playerPos, d.boundingBoxPosition.ToVector3()))
		select d).ToList<PrefabInstance>();
		DynamicMeshManager.LogMsg("Found prefabs: " + list.Count.ToString() + "   Already loaded: " + this.ItemsDictionary.Count.ToString());
		Dictionary<Vector3i, List<Vector3i>> dictionary = new Dictionary<Vector3i, List<Vector3i>>();
		foreach (PrefabInstance p2 in list)
		{
			DynamicMeshManager.Instance.CheckPrefab(p2, dictionary);
		}
		List<Vector3i> list2 = (from d in dictionary.Keys
		orderby DynamicMeshUnity.Distance(d, playerPos)
		select d).ToList<Vector3i>();
		DynamicMeshThread.RegionsToCheck = new List<List<Vector3i>>();
		int num = 0;
		foreach (Vector3i key in list2)
		{
			using (List<Vector3i>.Enumerator enumerator3 = dictionary[key].GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					Vector3i p = enumerator3.Current;
					DynamicMeshRegion regionFromWorldPosition = DynamicMeshRegion.GetRegionFromWorldPosition(p.x, p.z);
					if (regionFromWorldPosition == null || !regionFromWorldPosition.LoadedChunks.Any((Vector3i d) => d.x == p.x && d.z == p.z))
					{
						num++;
						this.AddChunk(p, false);
					}
				}
			}
		}
		DynamicMeshManager.LogMsg("Keys: " + list2.Count.ToString() + " loaded: " + num.ToString());
		this.PrefabCheck = PrefabCheckState.WaitingForCompleteCheck;
	}

	// Token: 0x060017F1 RID: 6129 RVA: 0x000921F8 File Offset: 0x000903F8
	public void CheckPrefabs(string source, bool forceRegen = false)
	{
		if (!DynamicMeshManager.IsServer)
		{
			return;
		}
		if (!forceRegen && (DynamicMeshSettings.OnlyPlayerAreas || !DynamicMeshSettings.NewWorldFullRegen))
		{
			return;
		}
		DynamicMeshManager.LogMsg("Checking prefabs " + source);
		if (GameManager.Instance.World.ChunkClusters.Count == 0)
		{
			DynamicMeshManager.LogMsg("Clusters zero");
			this.PrefabCheck = PrefabCheckState.WaitingForCompleteCheck;
			return;
		}
		this.PrefabCheck = PrefabCheckState.Warming;
		DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.World.ChunkClusters[0].ChunkProvider.GetDynamicPrefabDecorator();
		EntityPlayerLocal player = DynamicMeshManager.player;
		if (dynamicPrefabDecorator == null)
		{
			DynamicMeshManager.LogMsg("No deco found");
			return;
		}
		Vector3 playerPos = DynamicMeshManager.IsServer ? Vector3.zero : DynamicMeshManager.player.GetPosition();
		List<PrefabInstance> list = (from d in dynamicPrefabDecorator.allPrefabs
		orderby Math.Abs(Vector3.Distance(playerPos, d.boundingBoxPosition.ToVector3()))
		select d).ToList<PrefabInstance>();
		DynamicMeshManager.LogMsg("Prefabs: " + list.Count.ToString());
		DynamicMeshManager.LogMsg("Found prefabs: " + list.Count.ToString() + "   Already loaded: " + this.ItemsDictionary.Count.ToString());
		Dictionary<Vector3i, List<Vector3i>> dictionary = new Dictionary<Vector3i, List<Vector3i>>();
		foreach (PrefabInstance p2 in list)
		{
			DynamicMeshManager.Instance.CheckPrefab(p2, dictionary);
		}
		List<Vector3i> list2 = (from d in dictionary.Keys
		orderby DynamicMeshUnity.Distance(d, playerPos)
		select d).ToList<Vector3i>();
		DynamicMeshThread.RegionsToCheck = new List<List<Vector3i>>();
		int num = 0;
		foreach (Vector3i key in list2)
		{
			using (List<Vector3i>.Enumerator enumerator3 = dictionary[key].GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					Vector3i p = enumerator3.Current;
					DynamicMeshRegion regionFromWorldPosition = DynamicMeshRegion.GetRegionFromWorldPosition(p.x, p.z);
					if (regionFromWorldPosition == null || !regionFromWorldPosition.LoadedChunks.Any((Vector3i d) => d.x == p.x && d.z == p.z))
					{
						num++;
						this.AddChunk(p, false);
					}
				}
			}
		}
		DynamicMeshManager.LogMsg("Keys: " + list2.Count.ToString() + " loaded: " + num.ToString());
		this.PrefabCheck = PrefabCheckState.WaitingForCompleteCheck;
	}

	// Token: 0x060017F2 RID: 6130 RVA: 0x000924B4 File Offset: 0x000906B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckPrefab(PrefabInstance p, Dictionary<Vector3i, List<Vector3i>> positions)
	{
		Vector3i boundingBoxPosition = p.boundingBoxPosition;
		Vector3i boundingBoxSize = p.boundingBoxSize;
		int itemPosition = DynamicMeshUnity.GetItemPosition(boundingBoxPosition.x);
		int num = DynamicMeshUnity.GetItemPosition(boundingBoxPosition.x + boundingBoxSize.x) + 16;
		int itemPosition2 = DynamicMeshUnity.GetItemPosition(boundingBoxPosition.z);
		int num2 = DynamicMeshUnity.GetItemPosition(boundingBoxPosition.z + boundingBoxSize.z) + 16;
		for (int i = itemPosition; i <= num; i += 16)
		{
			for (int j = itemPosition2; j <= num2; j += 16)
			{
				long itemKey = DynamicMeshUnity.GetItemKey(i, j);
				DynamicMeshItem dynamicMeshItem;
				this.ItemsDictionary.TryGetValue(itemKey, out dynamicMeshItem);
				if (dynamicMeshItem == null || !dynamicMeshItem.FileExists())
				{
					Vector3i regionPositionFromWorldPosition = DynamicMeshUnity.GetRegionPositionFromWorldPosition(i, j);
					if (!positions.ContainsKey(regionPositionFromWorldPosition))
					{
						positions.Add(regionPositionFromWorldPosition, new List<Vector3i>());
					}
					List<Vector3i> list = positions[regionPositionFromWorldPosition];
					bool flag = false;
					for (int k = 0; k < list.Count; k++)
					{
						if (list[k].x == i && list[k].z == j)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						Vector3i item = new Vector3i(i, 0, j);
						if (DynamicMeshManager.DoLog)
						{
							DynamicMeshManager.LogMsg("Adding chunk " + i.ToString() + "," + j.ToString());
						}
						list.Add(item);
					}
				}
			}
		}
	}

	// Token: 0x060017F3 RID: 6131 RVA: 0x0009261C File Offset: 0x0009081C
	public void RefreshAll()
	{
		foreach (DynamicMeshItem item in this.ItemsDictionary.Values)
		{
			DynamicMeshThread.ToGenerate.Enqueue(item);
		}
	}

	// Token: 0x060017F4 RID: 6132 RVA: 0x00092674 File Offset: 0x00090874
	public void AddChunkStub(Vector3i worldPos, DynamicMeshRegion region)
	{
		long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(worldPos.x), World.toChunkXZ(worldPos.z));
		this.AddChunk(key, false, false, region);
	}

	// Token: 0x060017F5 RID: 6133 RVA: 0x000926A8 File Offset: 0x000908A8
	public static void AddDataFromServer(int x, int z)
	{
		if (DynamicMeshManager.Instance == null)
		{
			return;
		}
		Vector3i worldPos = new Vector3i(x, 0, z);
		DynamicMeshRegion region = DynamicMeshManager.Instance.GetRegion(x, z);
		bool flag = region.IsInItemLoad();
		DynamicMeshManager.Instance.AddChunkStub(worldPos, region);
		DynamicMeshItem itemFromWorldPosition = DynamicMeshManager.Instance.GetItemFromWorldPosition(x, z);
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg(string.Concat(new string[]
			{
				"Add data from server ",
				x.ToString(),
				",",
				z.ToString(),
				" itemLoad: ",
				flag.ToString(),
				"  state: ",
				itemFromWorldPosition.State.ToString()
			}));
		}
		itemFromWorldPosition.State = DynamicItemState.UpdateRequired;
		if (flag)
		{
			itemFromWorldPosition.Load("data from server", true, region.InBuffer);
		}
	}

	// Token: 0x060017F6 RID: 6134 RVA: 0x00092780 File Offset: 0x00090980
	public DynamicMeshItem AddChunk(Vector3i worldPos, bool primary)
	{
		long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(worldPos.x), World.toChunkXZ(worldPos.z));
		return this.AddChunk(key, true, primary, null);
	}

	// Token: 0x060017F7 RID: 6135 RVA: 0x000927B4 File Offset: 0x000909B4
	public DynamicMeshItem AddChunk(long key, bool addToThread, bool primary, DynamicMeshRegion region)
	{
		Vector3i vector3i = new Vector3i(WorldChunkCache.extractX(key) * 16, 0, WorldChunkCache.extractZ(key) * 16);
		DynamicMeshThread.AddRegionChunk(vector3i.x, vector3i.z, key);
		DynamicMeshItem dynamicMeshItem;
		if (!this.ItemsDictionary.TryGetValue(key, out dynamicMeshItem))
		{
			dynamicMeshItem = new DynamicMeshItem(vector3i);
			if (region == null)
			{
				region = this.GetRegion(dynamicMeshItem);
			}
			if (region == null)
			{
				return dynamicMeshItem;
			}
			region.AddItem(dynamicMeshItem);
			this.disabledImposterChunksDirty |= this.ItemsDictionary.TryAdd(key, dynamicMeshItem);
		}
		DynamicMeshUnity.AddDisabledImposterChunk(dynamicMeshItem.Key);
		if (addToThread)
		{
			if (primary)
			{
				DynamicMeshThread.RequestPrimaryQueue(dynamicMeshItem);
			}
			else
			{
				DynamicMeshThread.RequestSecondaryQueue(dynamicMeshItem);
			}
		}
		return dynamicMeshItem;
	}

	// Token: 0x060017F8 RID: 6136 RVA: 0x00092859 File Offset: 0x00090A59
	public static void LogMsg(string msg)
	{
		Log.Out("Dymesh: {0}", new object[]
		{
			msg
		});
	}

	// Token: 0x060017F9 RID: 6137 RVA: 0x00092870 File Offset: 0x00090A70
	public static void MeshDestroy(GameObject go)
	{
		MeshFilter component = go.GetComponent<MeshFilter>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component.sharedMesh);
			component.sharedMesh = null;
		}
		MeshRenderer component2 = go.GetComponent<MeshRenderer>();
		if (component2 != null)
		{
			component2.sharedMaterial = null;
			UnityEngine.Object.Destroy(component2);
		}
		foreach (object obj in go.transform)
		{
			DynamicMeshManager.MeshDestroy(((Transform)obj).gameObject);
		}
		UnityEngine.Object.Destroy(go);
	}

	// Token: 0x060017FA RID: 6138 RVA: 0x00092910 File Offset: 0x00090B10
	public void UpdateDynamicPrefabDecoratorRegions(DynamicMeshRegion region)
	{
		this.UpdateDynamicPrefabDecoratorRegion(region);
		DynamicMeshRegion region2 = this.GetRegion(region.WorldPosition + new Vector3i(160, 0, -160));
		this.UpdateDynamicPrefabDecoratorRegion(region2);
		region2 = this.GetRegion(region.WorldPosition + new Vector3i(-160, 0, 160));
		this.UpdateDynamicPrefabDecoratorRegion(region2);
		region2 = this.GetRegion(region.WorldPosition + new Vector3i(160, 0, 160));
		this.UpdateDynamicPrefabDecoratorRegion(region2);
		region2 = this.GetRegion(region.WorldPosition + new Vector3i(-160, 0, -160));
		this.UpdateDynamicPrefabDecoratorRegion(region2);
	}

	// Token: 0x060017FB RID: 6139 RVA: 0x000929C8 File Offset: 0x00090BC8
	public void UpdateDynamicPrefabDecoratorRegion(DynamicMeshRegion region)
	{
		if (DynamicMeshManager.DisableLOD)
		{
			return;
		}
		if (region == null)
		{
			return;
		}
		if (region.RegionObject == null)
		{
			return;
		}
		if (region.LoadedItems.Any((DynamicMeshItem d) => d.State != DynamicItemState.Loaded && d.State != DynamicItemState.Empty && d.State != DynamicItemState.ReadyToDelete))
		{
			return;
		}
		DateTime now = DateTime.Now;
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (GameManager.Instance == null)
			{
				DynamicMeshManager.LogMsg("GM Null");
			}
			if (GameManager.Instance.World == null)
			{
				DynamicMeshManager.LogMsg("world Null");
			}
			if (GameManager.Instance.World.ChunkClusters == null)
			{
				DynamicMeshManager.LogMsg("cluster Null");
			}
			if (GameManager.Instance.World.ChunkClusters.Count == 0)
			{
				DynamicMeshManager.LogMsg("cluster zero");
			}
			if (GameManager.Instance.World.ChunkClusters[0].ChunkProvider == null)
			{
				DynamicMeshManager.LogMsg("Provider Null");
			}
			DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.World.ChunkClusters[0].ChunkProvider.GetDynamicPrefabDecorator();
			if (dynamicPrefabDecorator == null)
			{
				DynamicMeshManager.LogMsg("dec Null");
				return;
			}
			if (region.Instances == null)
			{
				region.Instances = new List<PrefabInstance>();
				List<PrefabInstance> dynamicPrefabs = dynamicPrefabDecorator.GetDynamicPrefabs();
				if (dynamicPrefabs == null || dynamicPrefabs.Count == 0)
				{
					DynamicMeshManager.LogMsg("prefabs Null or empty");
					return;
				}
				for (int i = dynamicPrefabs.Count - 1; i >= 0; i--)
				{
					PrefabInstance prefabInstance = dynamicPrefabs[i];
					if (region.ContainsPrefab(prefabInstance))
					{
						region.Instances.Add(prefabInstance);
					}
				}
			}
		}
	}

	// Token: 0x060017FC RID: 6140 RVA: 0x00092B50 File Offset: 0x00090D50
	public bool PointInRectAndRegionExists(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, int x, int y)
	{
		bool flag = x >= topLeftX && x <= bottomRightX && y >= bottomRightY && y <= topLeftY;
		if (flag)
		{
			DynamicMeshRegion regionFromWorldPosition = DynamicMeshRegion.GetRegionFromWorldPosition(x, y);
			if (regionFromWorldPosition == null || regionFromWorldPosition.RegionObject == null)
			{
				return false;
			}
		}
		return flag;
	}

	// Token: 0x060017FD RID: 6141 RVA: 0x00092B9C File Offset: 0x00090D9C
	public bool IsRegionLoadedAndActive(int x, int y, bool doDebug)
	{
		if (DynamicMeshManager.Instance.PrefabCheck == PrefabCheckState.Run)
		{
			return true;
		}
		DynamicMeshRegion regionFromWorldPosition = DynamicMeshRegion.GetRegionFromWorldPosition(x, y);
		if (regionFromWorldPosition != null)
		{
			return regionFromWorldPosition.IsRegionLoadedAndActive(doDebug);
		}
		if (doDebug)
		{
			Log.Out("LoadedAndActive Failed: region is null");
		}
		return false;
	}

	// Token: 0x060017FE RID: 6142 RVA: 0x00092BD9 File Offset: 0x00090DD9
	public void ArrangeChunkRemoval(int x, int z)
	{
		this.ChunksToRemove.Enqueue(new Vector2i(x, z));
	}

	// Token: 0x060017FF RID: 6143 RVA: 0x00092BF0 File Offset: 0x00090DF0
	public void DisableLodGO(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(go.GetComponent<MeshRenderer>());
		UnityEngine.Object.Destroy(go.GetComponent<MeshFilter>());
		foreach (object obj in go.transform)
		{
			UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
	}

	// Token: 0x06001800 RID: 6144 RVA: 0x00092C6C File Offset: 0x00090E6C
	public bool StartObserver(Vector3 pos, Vector3 next)
	{
		this.ObserverRequestInfo = ObserverRequest.Start;
		this.ObserverPos = pos;
		this.ObserverPosNext = next;
		return true;
	}

	// Token: 0x06001801 RID: 6145 RVA: 0x00092C84 File Offset: 0x00090E84
	public void StopObserver()
	{
		if (this.Observer != null && this.Observer.Observer != null && this.ObserverRequestInfo != ObserverRequest.Stop)
		{
			this.ObserverRequestInfo = ObserverRequest.Stop;
		}
	}

	// Token: 0x06001802 RID: 6146 RVA: 0x00092CAC File Offset: 0x00090EAC
	public void HideChunk(Vector3i worldPos)
	{
		long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(worldPos.x), World.toChunkXZ(worldPos.z));
		DynamicMeshItem dynamicMeshItem;
		if (this.ItemsDictionary.TryGetValue(key, out dynamicMeshItem))
		{
			dynamicMeshItem.SetVisible(false, "Hide chunk world pos");
		}
	}

	// Token: 0x06001803 RID: 6147 RVA: 0x00092CF4 File Offset: 0x00090EF4
	public void HideChunk(long key)
	{
		DynamicMeshItem dynamicMeshItem;
		if (this.ItemsDictionary.TryGetValue(key, out dynamicMeshItem))
		{
			dynamicMeshItem.SetVisible(false, "Hide chunk world pos");
		}
	}

	// Token: 0x06001804 RID: 6148 RVA: 0x00092D20 File Offset: 0x00090F20
	public void ShowChunk(long key)
	{
		DynamicMeshItem dynamicMeshItem;
		if (this.ItemsDictionary.TryGetValue(key, out dynamicMeshItem))
		{
			dynamicMeshItem.GetRegion().OnChunkUnloaded(dynamicMeshItem);
			if (dynamicMeshItem.ChunkObject != null)
			{
				dynamicMeshItem.SetVisible(true, "Show chunk force");
			}
		}
	}

	// Token: 0x06001805 RID: 6149 RVA: 0x00092D64 File Offset: 0x00090F64
	public void LoadItemsDedicated()
	{
		DynamicMeshManager.LogMsg("Loading Items: " + DynamicMeshFile.MeshLocation);
		string[] files = SdDirectory.GetFiles(DynamicMeshFile.MeshLocation);
		SdFileInfo[] array = (from fi in new SdDirectoryInfo(DynamicMeshFile.MeshLocation).GetFiles("*.*")
		where fi.Length == 0L
		select fi).ToArray<SdFileInfo>();
		if (array.Length != 0)
		{
			DynamicMeshManager.LogMsg("Found " + array.Length.ToString() + " files @ zero size. Deleting...");
			SdFileInfo[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Delete();
			}
		}
		foreach (string text in files)
		{
			if (text.EndsWith(".update"))
			{
				long key = long.Parse(Path.GetFileNameWithoutExtension(text));
				DynamicMeshItem dynamicMeshItem = this.AddChunk(key, false, false, null);
				dynamicMeshItem.UpdateTime = dynamicMeshItem.ReadUpdateTimeFromFile();
			}
		}
		DynamicMeshManager.LogMsg("Loaded Items: " + this.ItemsDictionary.Count.ToString());
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			base.StartCoroutine(this.SendClientMessage());
		}
		if (files.Length == 0)
		{
			this.CheckPrefabs(" (load items dedicated)", false);
		}
	}

	// Token: 0x06001806 RID: 6150 RVA: 0x00092EA0 File Offset: 0x000910A0
	public void LoadItemsChunksDedicated()
	{
		DynamicMeshManager.LogMsg("Loading Items: " + DynamicMeshFile.MeshLocation);
		string[] files = SdDirectory.GetFiles(DynamicMeshFile.MeshLocation, "*.chunk");
		new Dictionary<string, Vector3>();
		string[] array = files;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = Path.GetFileNameWithoutExtension(array[i]).Split(',', StringSplitOptions.None);
			float x = float.Parse(array2[0]);
			float z = float.Parse(array2[1]);
			this.AddChunkStub(new Vector3i(x, 0f, z), null);
		}
		if (this.ItemsDictionary.Count == 0)
		{
			this.CheckPrefabs("LoadItemsChunksDedicated", false);
		}
		DynamicMeshManager.LogMsg("Loaded Items: " + this.ItemsDictionary.Count.ToString());
	}

	// Token: 0x06001807 RID: 6151 RVA: 0x00092F54 File Offset: 0x00091154
	public static int GetViewSize()
	{
		return GamePrefs.GetInt(EnumGamePrefs.OptionsGfxViewDistance) * 16;
	}

	// Token: 0x06001808 RID: 6152 RVA: 0x00092F5F File Offset: 0x0009115F
	public static int GetBufferSize()
	{
		return GamePrefs.GetInt(EnumGamePrefs.OptionsGfxViewDistance) * 16 * 3;
	}

	// Token: 0x06001809 RID: 6153 RVA: 0x00092F6C File Offset: 0x0009116C
	public static bool RectContainsRect(Rect r1, Rect r2)
	{
		float xMin = r1.xMin;
		float xMax = r1.xMax;
		float yMin = r1.yMin;
		float yMax = r1.yMax;
		float xMin2 = r2.xMin;
		float xMax2 = r2.xMax;
		float yMin2 = r2.yMin;
		float yMax2 = r2.yMax;
		return xMax2 >= xMin && xMin2 <= xMax && yMax2 >= yMin && yMin2 <= yMax;
	}

	// Token: 0x0600180A RID: 6154 RVA: 0x00092FD4 File Offset: 0x000911D4
	public static bool RectContainsPoint(Rect r1, int x, int z)
	{
		float xMin = r1.xMin;
		float xMax = r1.xMax;
		float yMin = r1.yMin;
		float yMax = r1.yMax;
		return (float)x >= xMin && (float)x <= xMax && (float)z >= yMin && (float)z <= yMax;
	}

	// Token: 0x0600180B RID: 6155 RVA: 0x0009301C File Offset: 0x0009121C
	public static void OriginUpdate()
	{
		if (DynamicMeshManager.Instance == null)
		{
			return;
		}
		DynamicMeshManager.Instance.ShowOrHidePrefabs();
		DynamicMeshManager.Instance.SetItemPositions();
	}

	// Token: 0x0600180C RID: 6156 RVA: 0x00093040 File Offset: 0x00091240
	public void SetItemPositions()
	{
		foreach (long key in this.ItemsDictionary.Keys.ToArray<long>())
		{
			this.ItemsDictionary[key].SetPosition();
		}
	}

	// Token: 0x0600180D RID: 6157 RVA: 0x00093084 File Offset: 0x00091284
	public void ShowOrHidePrefabs()
	{
		if (DynamicMeshManager.player == null)
		{
			return;
		}
		if (DynamicMeshRegion.Regions == null)
		{
			return;
		}
		if (DynamicMeshManager.DebugReport)
		{
			DynamicMeshManager.LogMsg("Regions to process: " + DynamicMeshRegion.Regions.Count.ToString());
			string str = "Player Position: ";
			Vector3 position = DynamicMeshManager.player.position;
			DynamicMeshManager.LogMsg(str + position.ToString());
		}
		Vector3i vector3i = new Vector3i(DynamicMeshManager.player.position);
		IChunk chunkFromWorldPos = GameManager.Instance.World.GetChunkFromWorldPos(vector3i);
		if (chunkFromWorldPos == null)
		{
			return;
		}
		Vector3i worldPos = chunkFromWorldPos.GetWorldPos();
		int viewSize = DynamicMeshManager.GetViewSize();
		GameManager instance = GameManager.Instance;
		DictionarySave<long, ChunkGameObject> dictionarySave = (instance != null) ? instance.World.ChunkClusters[0].DisplayedChunkGameObjects : null;
		if (dictionarySave == null)
		{
			return;
		}
		for (int i = worldPos.x - viewSize; i <= worldPos.x + viewSize; i += 16)
		{
			for (int j = worldPos.z - viewSize; j <= worldPos.z + viewSize; j += 16)
			{
				Vector3i vector3i2 = new Vector3i(i, 0, j);
				DynamicMeshItem itemOrNull = this.GetItemOrNull(vector3i2);
				if (itemOrNull != null && dictionarySave.ContainsKey(itemOrNull.Key))
				{
					itemOrNull.SetVisible(false, "showHide");
				}
				if (DynamicMeshManager.DebugReport)
				{
					string str2 = "World pos: ";
					Vector3i vector3i3 = vector3i2;
					DynamicMeshManager.LogMsg(str2 + vector3i3.ToString());
					DynamicMeshManager.LogMsg("Chunk Item: " + ((itemOrNull == null) ? "null" : itemOrNull.IsVisible.ToString()));
				}
			}
		}
		DynamicMeshItem itemOrNull2 = this.GetItemOrNull(vector3i);
		DynamicMeshRegion dynamicMeshRegion = (itemOrNull2 != null) ? itemOrNull2.GetRegion() : null;
		if (dynamicMeshRegion != null)
		{
			dynamicMeshRegion.SetVisibleNew(false, "showHide", true);
			foreach (DynamicMeshItem dynamicMeshItem in dynamicMeshRegion.LoadedItems)
			{
				dynamicMeshItem.SetVisible(!dynamicMeshItem.IsChunkInGame, "Region updateItems");
			}
		}
		foreach (DynamicMeshRegion dynamicMeshRegion2 in DynamicMeshRegion.Regions.Values)
		{
			if (dynamicMeshRegion2 != null)
			{
				dynamicMeshRegion2.DistanceChecks();
			}
		}
		DynamicMeshManager.DebugReport = false;
	}

	// Token: 0x0600180E RID: 6158 RVA: 0x000932F0 File Offset: 0x000914F0
	public bool AddRegionChecks()
	{
		if (DynamicMeshThread.RegionsToCheck == null)
		{
			return false;
		}
		if (DynamicMeshThread.RegionsToCheck.Count == 0)
		{
			return false;
		}
		DynamicMeshThread.AddRegionChecks = false;
		List<Vector3i> list = DynamicMeshThread.RegionsToCheck[0];
		DynamicMeshThread.RegionsToCheck.RemoveAt(0);
		DynamicMeshManager.LogMsg(string.Concat(new string[]
		{
			"Adding new region to check: ",
			list.Count.ToString(),
			"  at ",
			Time.time.ToString(),
			"  remaining: ",
			DynamicMeshThread.RegionsToCheck.Count.ToString()
		}));
		foreach (Vector3i worldPos in list)
		{
			this.AddChunk(worldPos, false);
		}
		if (DynamicMeshThread.RegionsToCheck.Count == 0)
		{
			DynamicMeshThread.RegionsToCheck = null;
		}
		return true;
	}

	// Token: 0x0600180F RID: 6159 RVA: 0x000933E8 File Offset: 0x000915E8
	public void OnGUI()
	{
		if (!DynamicMeshManager.CONTENT_ENABLED)
		{
			return;
		}
		if (!DynamicMeshManager.ShowGui || GameManager.Instance == null || GameManager.Instance.World == null)
		{
			return;
		}
		if (DynamicMeshManager.player == null)
		{
			return;
		}
		Vector3i regionPositionFromWorldPosition = DynamicMeshUnity.GetRegionPositionFromWorldPosition(DynamicMeshManager.player.position);
		Vector3i vector3i = new Vector3i(World.toChunkXZ((int)DynamicMeshManager.player.position.x) * 16, 0, World.toChunkXZ((int)DynamicMeshManager.player.position.z) * 16);
		string text = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? "SP /p2p Host" : "Dedi / p2p";
		try
		{
			string[] array = new string[71];
			array[0] = "p: ";
			array[1] = DynamicMeshManager.player.position.ToString();
			array[2] = " - ";
			array[3] = DynamicMeshManager.player.transform.position.ToString();
			array[4] = "\nc: ";
			array[5] = vector3i.x.ToString();
			array[6] = ",";
			array[7] = vector3i.z.ToString();
			array[8] = "\nr:";
			array[9] = regionPositionFromWorldPosition.x.ToString();
			array[10] = ",";
			array[11] = regionPositionFromWorldPosition.z.ToString();
			array[12] = "\n Buff : ";
			array[13] = this.BufferRegionLoadRequests.Count.ToString();
			array[14] = "\n Items: ";
			array[15] = this.ChunkMeshData.Count.ToString();
			array[16] = "\n ItemGen: ";
			array[17] = DynamicMeshThread.MeshGenCount.ToString();
			array[18] = "\n Reg  : ";
			array[19] = this.RegionsAvailableToLoad.Count.ToString();
			array[20] = string.Format(" ({0})", this.AvailableRegionLoadRequests);
			array[21] = "\n Server : ";
			array[22] = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer.ToString();
			array[23] = " / ";
			array[24] = SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient.ToString();
			array[25] = "\n Thread P/S : ";
			array[26] = DynamicMeshThread.PrimaryQueue.Count.ToString();
			array[27] = " / ";
			array[28] = DynamicMeshThread.SecondaryQueue.Count.ToString();
			array[29] = "\n Game: ";
			array[30] = text;
			array[31] = "\n Packets: ";
			array[32] = NetPackageDynamicMesh.Count.ToString();
			array[33] = "\n ThreadDistance: ";
			array[34] = DynamicMeshManager.ThreadDistance.ToString();
			array[35] = "\n ObserverDistance: ";
			array[36] = DynamicMeshManager.ObserverDistance.ToString();
			array[37] = "\n ItemLoadDistance: ";
			array[38] = DynamicMeshManager.ItemLoadDistance.ToString();
			array[39] = "\n ItemCache: ";
			array[40] = DynamicMeshThread.ChunkDataQueue.ChunkData.Count.ToString();
			array[41] = " (";
			array[42] = DynamicMeshThread.ChunkDataQueue.LiveItems.ToString();
			array[43] = " live)\n WorldChunks: ";
			array[44] = GameManager.Instance.World.ChunkCache.chunks.list.Count.ToString();
			array[45] = "\n ThreadNext: ";
			array[46] = DynamicMeshThread.nextChunks.Count.ToString();
			array[47] = "\n ThreadQueue: ";
			array[48] = DynamicMeshThread.Queue;
			array[49] = "\n RegionUpdates: ";
			array[50] = DynamicMeshThread.RegionUpdates.Count.ToString();
			array[51] = "\n RegionUpdatesDebug: ";
			array[52] = DynamicMeshThread.RegionUpdatesDebug;
			array[53] = "\n SyncPackets: ";
			array[54] = DynamicMeshServer.SyncRequests.Count.ToString();
			array[55] = " (";
			array[56] = DynamicMeshServer.ActiveSyncs.Count.ToString();
			array[57] = ")\n DataSaveQueue: ";
			array[58] = DynamicMeshThread.ChunkDataQueue.ChunkData.Count.ToString();
			array[59] = "\n DataCache: ";
			array[60] = DynamicMeshChunkData.ActiveDataItems.ToString();
			array[61] = "/";
			array[62] = DynamicMeshChunkData.Cache.Count.ToString();
			array[63] = " (";
			array[64] = DynamicMeshChunkData.Cache.Sum((DynamicMeshChunkData d) => (double)d.GetStreamSize() / 1024.0 / 1024.0).ToString();
			array[65] = "MB)\n vMeshCache: ";
			array[66] = DynamicMeshVoxelLoad.LayerCache.Count.ToString();
			array[67] = "\n DymeshData: ";
			array[68] = DyMeshData.ActiveItems.ToString();
			array[69] = "/";
			array[70] = DyMeshData.TotalItems.ToString();
			string text2 = string.Concat(array);
			foreach (DynamicMeshChunkProcessor dynamicMeshChunkProcessor in DynamicMeshThread.BuilderManager.BuilderThreads.ToList<DynamicMeshChunkProcessor>())
			{
				if (dynamicMeshChunkProcessor != null)
				{
					string[] array2 = new string[9];
					array2[0] = text2;
					array2[1] = "\n Thread: ";
					int num = 2;
					DynamicMeshItem item = dynamicMeshChunkProcessor.Item;
					array2[num] = ((item != null) ? item.ToDebugLocation() : null);
					array2[3] = " Data ";
					array2[4] = ((int)dynamicMeshChunkProcessor.MeshDataTime).ToString();
					array2[5] = "ms Mesh: ";
					array2[6] = ((int)dynamicMeshChunkProcessor.ExportTime).ToString();
					array2[7] = " Inactive: ";
					array2[8] = ((int)dynamicMeshChunkProcessor.InactiveTime).ToString();
					text2 = string.Concat(array2);
				}
			}
			Color contentColor = GUI.contentColor;
			Color color = GUI.color;
			int depth = GUI.depth;
			GUI.depth = 0;
			GUI.DrawTexture(new Rect(0f, (float)Screen.height, 500f, 500f), Texture2D.blackTexture, ScaleMode.StretchToFill);
			GUI.color = DynamicMeshManager.DebugStyle.normal.textColor;
			GUI.contentColor = DynamicMeshManager.DebugStyle.normal.textColor;
			GUI.Label(new Rect(0f, (float)DynamicMeshManager.GuiY, 500f, 500f), text2, DynamicMeshManager.DebugStyle);
			GUI.contentColor = contentColor;
			GUI.color = color;
			GUI.depth = depth;
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06001810 RID: 6160 RVA: 0x00093A6C File Offset: 0x00091C6C
	public DynamicMeshRegion GetNearestUnloadedRegion()
	{
		if (this.PrimaryLocation != null)
		{
			this.NearestRegionWithUnloaded = this.GetRegion(this.PrimaryLocation.Value);
		}
		if (this.NearestRegionWithUnloaded == null)
		{
			DynamicMeshRegion nearestRegionWithUnloaded = (from d in DynamicMeshRegion.Regions.Values
			where !d.FastTrackLoaded && d.UnloadedItems.Any<DynamicMeshItem>()
			orderby d.DistanceToPlayer()
			select d).ToList<DynamicMeshRegion>().FirstOrDefault<DynamicMeshRegion>();
			(from d in DynamicMeshRegion.Regions.Values
			where d.UnloadedItems.Any<DynamicMeshItem>()
			orderby d.DistanceToPlayer()
			select d).FirstOrDefault<DynamicMeshRegion>();
			this.NearestRegionWithUnloaded = nearestRegionWithUnloaded;
			if (this.NearestRegionWithUnloaded != null)
			{
				this.NearestRegionWithUnloaded.FastTrackLoaded = true;
			}
		}
		this.PrimaryLocation = null;
		this.FindNearestUnloadedItems = false;
		return this.NearestRegionWithUnloaded;
	}

	// Token: 0x06001811 RID: 6161 RVA: 0x00093B94 File Offset: 0x00091D94
	[PublicizedFrom(EAccessModifier.Private)]
	public void MonitorGC()
	{
		for (int i = 0; i < 3; i++)
		{
			int num = GC.CollectionCount(i);
			if (num != this.gcGenCount[i])
			{
				this.gcGenCount[i] = num;
				Log.Out(string.Format("Gen {0} has fired: ", i) + num.ToString());
			}
		}
	}

	// Token: 0x06001812 RID: 6162 RVA: 0x00093BEC File Offset: 0x00091DEC
	public void Update()
	{
		if (!DynamicMeshManager.CONTENT_ENABLED)
		{
			return;
		}
		if (DynamicMeshManager.Instance == null)
		{
			return;
		}
		this.time = Time.time;
		if (this.FindNearestUnloadedItems)
		{
			this.GetNearestUnloadedRegion();
		}
		while (this.ChunksToRemove.Count > 0)
		{
			Vector2i vector2i;
			if (this.ChunksToRemove.TryDequeue(out vector2i))
			{
				if (DynamicMeshManager.DoLog)
				{
					DynamicMeshManager.LogMsg("Remove chunk from region " + vector2i.x.ToString() + "," + vector2i.y.ToString());
				}
				DynamicMeshItem itemFromWorldPosition = this.GetItemFromWorldPosition(vector2i.x, vector2i.y);
				this.RemoveItem(itemFromWorldPosition, true);
			}
		}
		if (this.ProcessItemLoadReady || this.ForceNextItemLoad < DateTime.Now)
		{
			if (this.ForceNextItemLoad < DateTime.Now)
			{
				Log.Warning("Forcing mesh processing after large delay");
			}
			base.StartCoroutine(this.ProcessItemMeshGeneration());
		}
		if (this.RegionFileLoadRequests.Count > 0)
		{
			base.StartCoroutine(this.ProcessChunkRegionRequests());
		}
		this.DestroyObjects();
		this.CheckFallingObservers();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			DynamicMeshServer.Update();
		}
		if (this.Observer.StopTime < Time.time)
		{
			this.Observer.Stop();
			this.ObserverPrep.Stop();
		}
		if (this.nextShowHide < Time.time)
		{
			this.ShowOrHidePrefabs();
			this.nextShowHide = Time.time + (float)(DynamicMeshManager.ShowHideCheckTime / 1000);
			this.CheckGameObjects();
		}
		if (this.ObserverRequestInfo != ObserverRequest.None)
		{
			if (this.ObserverRequestInfo == ObserverRequest.Start)
			{
				this.Observer.Start(this.ObserverPos);
			}
			else if (this.ObserverRequestInfo == ObserverRequest.Stop)
			{
				this.Observer.StopTime = Time.time + 3f;
			}
			this.ObserverRequestInfo = ObserverRequest.None;
		}
		while (DynamicMeshThread.ReadyForCollection.Count > 0)
		{
			DynamicMeshData dynamicMeshData;
			if (DynamicMeshThread.ReadyForCollection.TryDequeue(out dynamicMeshData))
			{
				DynamicMeshItem itemFromWorldPosition2 = this.GetItemFromWorldPosition(dynamicMeshData.X, dynamicMeshData.Z);
				DynamicMeshRegion region = itemFromWorldPosition2.GetRegion();
				if (itemFromWorldPosition2.ChunkObject != null || (region != null && region.IsInItemLoad()))
				{
					this.AddItemLoadRequest(itemFromWorldPosition2, false);
				}
			}
		}
		while (DynamicMeshThread.ChunkReadyForCollection.Count > 0)
		{
			Vector2i vector2i2;
			if (DynamicMeshThread.ChunkReadyForCollection.TryRemoveFirst(out vector2i2))
			{
				DynamicMeshItem itemFromWorldPosition3 = this.GetItemFromWorldPosition(vector2i2.x, vector2i2.y);
				DynamicMeshRegion region = itemFromWorldPosition3.GetRegion();
				if (itemFromWorldPosition3.ChunkObject != null || (region != null && region.IsInItemLoad()))
				{
					DynamicMeshThread.AddChunkGenerationRequest(itemFromWorldPosition3);
				}
			}
		}
		while (this.AvailableRegionLoadRequests > 0 && this.RegionsAvailableToLoad.Count > 0)
		{
			LinkedListNode<long> linkedListNode = null;
			float num = 99999f;
			for (LinkedListNode<long> linkedListNode2 = this.RegionsAvailableToLoad.First; linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
			{
				long value = linkedListNode2.Value;
				DynamicMeshRegion region = this.GetRegion(value);
				if (region.OutsideLoadArea)
				{
					if (linkedListNode == linkedListNode2)
					{
						linkedListNode = null;
					}
					this.RegionsAvailableToLoad.Remove(linkedListNode2);
				}
				else
				{
					float num2 = region.DistanceToPlayer();
					if (num2 < num)
					{
						linkedListNode = linkedListNode2;
						num = num2;
						if (num < 100f)
						{
							break;
						}
					}
				}
			}
			if (linkedListNode == null)
			{
				break;
			}
			long value2 = linkedListNode.Value;
			this.RegionsAvailableToLoad.Remove(linkedListNode);
			DynamicMeshThread.AddRegionLoadRequest(DyMeshRegionLoadRequest.Create(value2));
			this.AvailableRegionLoadRequests--;
		}
		List<DynamicMeshUpdateData> list = new List<DynamicMeshUpdateData>();
		for (int i = 0; i < this.UpdateData.Count; i++)
		{
			DynamicMeshUpdateData dynamicMeshUpdateData = this.UpdateData[i];
			if (dynamicMeshUpdateData.UpdateTime < Time.time || dynamicMeshUpdateData.IsUrgent || dynamicMeshUpdateData.MaxTime < Time.time)
			{
				this.AddChunk(dynamicMeshUpdateData.Key, dynamicMeshUpdateData.AddToThread, true, null);
				list.Add(dynamicMeshUpdateData);
			}
		}
		foreach (DynamicMeshUpdateData item in list)
		{
			this.UpdateData.Remove(item);
		}
		if (this.nextUpdate > Time.time)
		{
			return;
		}
		if (DynamicMeshThread.RegionsToCheck != null && DynamicMeshThread.AddRegionChecks)
		{
			this.AddRegionChecks();
		}
		this.nextUpdate = Time.time + 1f;
		EntityPlayerLocal player = DynamicMeshManager.player;
		if (player != null)
		{
			DynamicMeshThread.PlayerPositionX = player.position.x;
			DynamicMeshThread.PlayerPositionZ = player.position.z;
		}
		if (DynamicMeshChunkProcessor.DebugOnMainThread)
		{
			DynamicMeshThread.BuilderManager.MainThreadRunJobs();
		}
		if (this.nextOrphanCheck < DateTime.Now)
		{
			this.nextOrphanCheck = DateTime.Now.AddSeconds(10.0);
			base.StartCoroutine(this.CheckForOrphans(true));
		}
		if (this.disabledImposterChunksDirty)
		{
			this.disabledImposterChunkManager.Update();
			this.disabledImposterChunksDirty = false;
		}
	}

	// Token: 0x06001813 RID: 6163 RVA: 0x000940C4 File Offset: 0x000922C4
	public void AddItemLoadRequest(DynamicMeshItem item, bool urgent)
	{
		item.GetRegion().AddToLoadingQueue(item);
		DynamicMeshThread.AddChunkGenerationRequest(item);
	}

	// Token: 0x06001814 RID: 6164 RVA: 0x000940D8 File Offset: 0x000922D8
	public bool IsInLoadableArea(long key)
	{
		return !DynamicMeshSettings.OnlyPlayerAreas || this.HandlePlayerOnlyAreas(key, DynamicMeshUnity.GetWorldPosFromKey(key));
	}

	// Token: 0x06001815 RID: 6165 RVA: 0x000940F8 File Offset: 0x000922F8
	public DynamicMeshItem GetItemOrNull(Vector3i worldPos)
	{
		long itemKey = DynamicMeshUnity.GetItemKey(worldPos.x, worldPos.z);
		DynamicMeshItem result;
		if (this.ItemsDictionary.TryGetValue(itemKey, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06001816 RID: 6166 RVA: 0x0009412C File Offset: 0x0009232C
	public DynamicMeshItem GetItemOrNull(long key)
	{
		DynamicMeshItem result;
		if (this.ItemsDictionary.TryGetValue(key, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06001817 RID: 6167 RVA: 0x0009414C File Offset: 0x0009234C
	public DynamicMeshItem GetItemFromWorldPosition(int x, int z)
	{
		long itemKey = DynamicMeshUnity.GetItemKey(x, z);
		DynamicMeshItem result;
		if (this.ItemsDictionary.TryGetValue(itemKey, out result))
		{
			return result;
		}
		result = this.AddChunk(itemKey, false, false, null);
		return result;
	}

	// Token: 0x06001818 RID: 6168 RVA: 0x0009417F File Offset: 0x0009237F
	public DynamicMeshRegion GetRegion(int x, int z)
	{
		return this.GetRegion(DynamicMeshUnity.GetRegionKeyFromWorldPosition(x, z));
	}

	// Token: 0x06001819 RID: 6169 RVA: 0x0009418E File Offset: 0x0009238E
	public DynamicMeshRegion GetRegion(Vector3i worldPos)
	{
		return this.GetRegion(DynamicMeshUnity.GetRegionKeyFromWorldPosition(worldPos.x, worldPos.z));
	}

	// Token: 0x0600181A RID: 6170 RVA: 0x000941A7 File Offset: 0x000923A7
	public DynamicMeshRegion GetRegion(DynamicMeshItem item)
	{
		return this.GetRegion(item.GetRegionKey());
	}

	// Token: 0x0600181B RID: 6171 RVA: 0x000941B8 File Offset: 0x000923B8
	public DynamicMeshRegion GetRegion(long key)
	{
		DynamicMeshRegion dynamicMeshRegion;
		DynamicMeshRegion.Regions.TryGetValue(key, out dynamicMeshRegion);
		if (dynamicMeshRegion == null)
		{
			dynamicMeshRegion = new DynamicMeshRegion(key);
			if (!DynamicMeshRegion.Regions.TryAdd(key, dynamicMeshRegion))
			{
				DynamicMeshRegion.Regions.TryGetValue(key, out dynamicMeshRegion);
			}
		}
		return dynamicMeshRegion;
	}

	// Token: 0x0600181C RID: 6172 RVA: 0x000941FA File Offset: 0x000923FA
	public static void Init()
	{
		DynamicMeshManager.CONTENT_ENABLED = GamePrefs.GetBool(EnumGamePrefs.DynamicMeshEnabled);
		DynamicMeshManager.DisabledImposterChunkManager.DisableShaderKeyword();
		GameManager.Instance.StopCoroutine(DynamicMeshManager.DelayStartForWorldLoad());
		GameManager.Instance.StartCoroutine(DynamicMeshManager.DelayStartForWorldLoad());
	}

	// Token: 0x170002AE RID: 686
	// (get) Token: 0x0600181D RID: 6173 RVA: 0x00091098 File Offset: 0x0008F298
	public static EntityPlayerLocal player
	{
		get
		{
			if (GameManager.Instance.World != null)
			{
				return GameManager.Instance.World.GetPrimaryPlayer();
			}
			return null;
		}
	}

	// Token: 0x0600181E RID: 6174 RVA: 0x0009422F File Offset: 0x0009242F
	public static void EnableErrorCallstackLogs()
	{
		Log.LogCallbacks -= DynamicMeshManager.ShowErrorStackTraces;
		Log.LogCallbacks += DynamicMeshManager.ShowErrorStackTraces;
	}

	// Token: 0x0600181F RID: 6175 RVA: 0x00094253 File Offset: 0x00092453
	public static void DisableErrorCallstackLogs()
	{
		Log.LogCallbacks -= DynamicMeshManager.ShowErrorStackTraces;
	}

	// Token: 0x06001820 RID: 6176 RVA: 0x00094266 File Offset: 0x00092466
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator DelayStartForWorldLoad()
	{
		while (GameManager.Instance == null || GameManager.Instance.World == null)
		{
			Log.Out("Dynamic mesh waiting for world");
			yield return DynamicMeshFile.WaitOne;
		}
		if (!DynamicMeshManager.CONTENT_ENABLED)
		{
			Log.Out("Dynamic mesh disabled on world start");
			yield break;
		}
		DynamicMeshManager.LogMsg("Prepping dynamic mesh. Resend Default: " + DynamicMeshServer.ResendPackages.ToString());
		DynamicMeshFile.MeshLocation = (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? (GameIO.GetSaveGameDir() + "/DynamicMeshes/") : (GameIO.GetSaveGameLocalDir() + "/DynamicMeshes/"));
		DynamicMeshManager.LogMsg("Mesh location: " + DynamicMeshFile.MeshLocation);
		if (!SdDirectory.Exists(DynamicMeshFile.MeshLocation))
		{
			SdDirectory.CreateDirectory(DynamicMeshFile.MeshLocation);
		}
		yield return new WaitForSeconds(1f);
		if (DynamicMeshManager.Parent != null)
		{
			UnityEngine.Object.Destroy(DynamicMeshManager.Parent);
			DynamicMeshManager.Parent = null;
		}
		DynamicMeshManager.DebugStyle.fontSize = ((Screen.width > 3000) ? 30 : 14);
		DynamicMeshManager.DebugStyle.normal.textColor = Color.magenta;
		if (DynamicMeshManager.Instance)
		{
			DynamicMeshManager.OnWorldUnload();
			DynamicMeshManager.Instance.ClearPrefabs();
		}
		DynamicMeshManager.LogMsg("Warming dynamic mesh");
		if (DynamicMeshManager.Instance == null || DynamicMeshManager.Parent == null)
		{
			DynamicMeshManager.LogMsg("Creating dynamic mesh manager");
			DynamicMeshManager.Parent = new GameObject("DynamicMeshes");
			DynamicMeshManager.Parent.AddComponent<DynamicMeshManager>();
		}
		DynamicMeshBlockSwap.Init();
		DynamicMeshSettings.Validate();
		if (!GameManager.IsDedicatedServer && Application.isEditor && SdDirectory.Exists("D:\\7DaysToDie\\trunkCode"))
		{
			DynamicMeshConsoleCmd.DebugAll();
		}
		yield break;
	}

	// Token: 0x06001821 RID: 6177 RVA: 0x00094270 File Offset: 0x00092470
	public void ReorderGameObjects()
	{
		List<Transform> regions = new List<Transform>();
		List<Transform> list = new List<Transform>();
		foreach (object obj in DynamicMeshManager.Parent.transform)
		{
			Transform item = (Transform)obj;
			list.Add(item);
		}
		regions = (from d in list
		where !d.gameObject.name.StartsWith("C")
		orderby d.gameObject.name
		select d).ToList<Transform>();
		list.RemoveAll((Transform d) => regions.Contains(d));
		int num = 0;
		foreach (Transform transform in regions)
		{
			transform.SetSiblingIndex(num++);
			if (!(transform.gameObject.name == string.Empty))
			{
				string[] array = transform.gameObject.name.Replace("(sync)", "").Replace("R ", "").Split(',', StringSplitOptions.None);
				int num2 = int.Parse(array[0]);
				int num3 = int.Parse(array[1]);
				foreach (Transform transform2 in list)
				{
					string[] array2 = transform2.gameObject.name.Substring(2, transform2.gameObject.name.IndexOf(":") - 2).Split(',', StringSplitOptions.None);
					int num4 = int.Parse(array2[0]);
					int num5 = int.Parse(array2[1]);
					if (num4 >= num2 && num5 >= num3 && num4 < num2 + 160 && num5 < num3 + 160)
					{
						transform2.SetSiblingIndex(num++);
					}
				}
			}
		}
	}

	// Token: 0x06001822 RID: 6178 RVA: 0x000944D8 File Offset: 0x000926D8
	public void RemoveItem(DynamicMeshItem item, bool removedFromWorld)
	{
		if (item == null)
		{
			return;
		}
		this.GetRegion(item).RemoveChunk(item.WorldPosition.x, item.WorldPosition.z, "removeItem", removedFromWorld);
		if (removedFromWorld && !GameManager.IsDedicatedServer)
		{
			DynamicMeshItem dynamicMeshItem;
			this.disabledImposterChunksDirty |= this.ItemsDictionary.TryRemove(item.Key, out dynamicMeshItem);
		}
		DynamicMeshThread.RemoveRegionChunk(item.WorldPosition.x, item.WorldPosition.z, item.Key);
		DynamicMeshUnity.RemoveDisabledImposterChunk(item.Key);
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg("Item removed: " + item.ToDebugLocation());
		}
	}

	// Token: 0x06001823 RID: 6179 RVA: 0x00094584 File Offset: 0x00092784
	public static void ChunkChanged(Vector3i worldPos, int entityId, int blockType)
	{
		if (DynamicMeshManager.Instance != null)
		{
			if (blockType != -1 && !DynamicMeshBlockSwap.IsValidBlock(blockType))
			{
				return;
			}
			if (ThreadManager.IsMainThread() && DynamicMeshManager.player != null && DynamicMeshManager.player.entityId == entityId)
			{
				DynamicMeshRegion region = DynamicMeshManager.Instance.GetRegion(worldPos);
				if (region.IsInItemLoad())
				{
					region.SetVisibleNew(false, DynamicMeshManager.ChunkChangedInItemLoad, true);
				}
			}
			if (DynamicMeshManager.IsServer)
			{
				DynamicMeshManager.Instance.AddUpdateData(worldPos, false, true);
				int num = worldPos.x & 15;
				if (num == 0)
				{
					DynamicMeshManager.Instance.AddUpdateData(worldPos + new Vector3i(-16, 0, 0), false, true);
				}
				else if (num == 15)
				{
					DynamicMeshManager.Instance.AddUpdateData(worldPos + new Vector3i(16, 0, 0), false, true);
				}
				if ((worldPos.z & 15) == 0)
				{
					DynamicMeshManager.Instance.AddUpdateData(worldPos + new Vector3i(0, 0, -16), false, true);
					return;
				}
				if (num == 15)
				{
					DynamicMeshManager.Instance.AddUpdateData(worldPos + new Vector3i(0, 0, 16), false, true);
				}
			}
		}
	}

	// Token: 0x06001824 RID: 6180 RVA: 0x000946A0 File Offset: 0x000928A0
	public bool AddUpdateData(Vector3i worldPos, bool isUrgent, bool addToThread)
	{
		long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(worldPos.x), World.toChunkXZ(worldPos.z));
		return this.AddUpdateData(key, isUrgent, addToThread, true, 1);
	}

	// Token: 0x06001825 RID: 6181 RVA: 0x000946D4 File Offset: 0x000928D4
	public bool AddUpdateData(int worldX, int worldZ, bool isUrgent, bool addToThread)
	{
		long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(worldX), World.toChunkXZ(worldZ));
		return this.AddUpdateData(key, isUrgent, addToThread, true, 1);
	}

	// Token: 0x06001826 RID: 6182 RVA: 0x00094700 File Offset: 0x00092900
	public bool AddUpdateData(long key, bool isUrgent, bool addToThread, bool checkPlayerArea, int delayScale = 1)
	{
		if (DynamicMeshThread.ChunkDataQueue == null || DynamicMeshManager.Instance == null)
		{
			return false;
		}
		DynamicMeshUpdateData dynamicMeshUpdateData = null;
		Vector3i chunkPosition = new Vector3i(WorldChunkCache.extractX(key) * 16, 0, WorldChunkCache.extractZ(key) * 16);
		Vector3 position = chunkPosition.ToVector3();
		if (checkPlayerArea && !this.HandlePlayerOnlyAreas(key, position))
		{
			return false;
		}
		int i = 0;
		while (i < this.UpdateData.Count)
		{
			DynamicMeshUpdateData dynamicMeshUpdateData2 = this.UpdateData[i++];
			if (dynamicMeshUpdateData2.Key == key)
			{
				dynamicMeshUpdateData = dynamicMeshUpdateData2;
				break;
			}
		}
		if (dynamicMeshUpdateData == null)
		{
			dynamicMeshUpdateData = new DynamicMeshUpdateData();
			dynamicMeshUpdateData.ChunkPosition = chunkPosition;
			dynamicMeshUpdateData.Key = key;
			dynamicMeshUpdateData.MaxTime = this.time + DynamicMeshManager.MaxRebuildTime;
			dynamicMeshUpdateData.AddToThread = addToThread;
			this.UpdateData.Add(dynamicMeshUpdateData);
		}
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg(string.Concat(new string[]
			{
				"Adding update ",
				key.ToString(),
				"  Time: ",
				dynamicMeshUpdateData.UpdateTime.ToString(),
				" pos: ",
				dynamicMeshUpdateData.ToDebugLocation()
			}));
		}
		dynamicMeshUpdateData.UpdateTime = this.time + (float)(this.QueueDelay * delayScale);
		dynamicMeshUpdateData.IsUrgent = (dynamicMeshUpdateData.IsUrgent || isUrgent);
		dynamicMeshUpdateData.AddToThread = (dynamicMeshUpdateData.AddToThread || addToThread);
		return true;
	}

	// Token: 0x06001827 RID: 6183 RVA: 0x0009484A File Offset: 0x00092A4A
	public bool HandlePlayerOnlyAreas(long key, Vector3 position)
	{
		return !DynamicMeshSettings.OnlyPlayerAreas || this.IsPositionInRange(position);
	}

	// Token: 0x06001828 RID: 6184 RVA: 0x00094864 File Offset: 0x00092A64
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsPositionInRange(Vector3 _position)
	{
		float x = _position.x;
		float z = _position.z;
		int num = Math.Max(16, (DynamicMeshSettings.PlayerAreaChunkBuffer - 1) / 2 * 16);
		DictionaryList<int, EntityPlayer> players = GameManager.Instance.World.Players;
		for (int i = 0; i < players.list.Count; i++)
		{
			for (int j = 0; j < players.list[i].SpawnPoints.Count; j++)
			{
				Vector3i vector3i = players.list[i].SpawnPoints[j];
				int num2 = vector3i.x - num;
				int num3 = vector3i.x + num + 16;
				int num4 = vector3i.z - num;
				int num5 = vector3i.z + num + 16;
				if (x >= (float)num2 && x < (float)num3 && z >= (float)num4 && z < (float)num5)
				{
					return true;
				}
			}
		}
		foreach (PersistentPlayerData persistentPlayerData in GameManager.Instance.persistentPlayers.m_lpBlockMap.Values)
		{
			for (int k = 0; k < persistentPlayerData.LPBlocks.Count; k++)
			{
				Vector3i vector3i2 = persistentPlayerData.LPBlocks[k];
				int num2 = DynamicMeshUnity.GetItemPosition(vector3i2.x) - num;
				int num3 = DynamicMeshUnity.GetItemPosition(vector3i2.x) + num + 16;
				int num4 = DynamicMeshUnity.GetItemPosition(vector3i2.z) - num;
				int num5 = DynamicMeshUnity.GetItemPosition(vector3i2.z) + num + 16;
				if (x >= (float)num2 && x < (float)num3 && z >= (float)num4 && z < (float)num5)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001829 RID: 6185 RVA: 0x00094A34 File Offset: 0x00092C34
	public static void ImportVox(string name, Vector3 pos, int blockId)
	{
		if (blockId == 0)
		{
			blockId = 502;
		}
		if (DynamicMeshManager.Instance == null || GameManager.Instance == null || GameManager.Instance.World == null)
		{
			return;
		}
		GameManager.bPhysicsActive = false;
		string text = DynamicMeshFile.MeshLocation + name + ".vox";
		if (!SdFile.Exists(text))
		{
			Log.Out("File " + text + " does not exist. Cancelling import");
			return;
		}
		byte[] buffer = Convert.FromBase64String(SdFile.ReadAllText(text));
		BlockValue blockValue = default(BlockValue);
		blockValue.type = blockId;
		BlockValue blockValue2 = default(BlockValue);
		blockValue2.type = 1;
		HashSet<Vector3i> hashSet = new HashSet<Vector3i>();
		int num;
		int num3;
		List<BlockChangeInfo> list;
		using (MemoryStream memoryStream = new MemoryStream(buffer))
		{
			using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
			{
				pooledBinaryReader.SetBaseStream(memoryStream);
				num = (int)pooledBinaryReader.ReadByte();
				int num2 = (int)pooledBinaryReader.ReadByte();
				num3 = (int)pooledBinaryReader.ReadByte();
				list = new List<BlockChangeInfo>(num * num2 * num3 / 2);
				while (pooledBinaryReader.BaseStream.Position != pooledBinaryReader.BaseStream.Length)
				{
					byte b = pooledBinaryReader.ReadByte();
					byte b2 = pooledBinaryReader.ReadByte();
					byte b3 = pooledBinaryReader.ReadByte();
					float num4 = pos.x + (float)b;
					float y = pos.y + (float)b2;
					float num5 = pos.z + (float)b3;
					BlockChangeInfo blockChangeInfo = new BlockChangeInfo(new Vector3i(num4, y, num5), blockValue, 0);
					if (!hashSet.Contains(blockChangeInfo.pos))
					{
						list.Add(blockChangeInfo);
						hashSet.Add(blockChangeInfo.pos);
					}
					blockChangeInfo = new BlockChangeInfo(new Vector3i(num4 + 1f, y, num5), blockValue2, 0);
					if (!hashSet.Contains(blockChangeInfo.pos))
					{
						list.Add(blockChangeInfo);
					}
					blockChangeInfo = new BlockChangeInfo(new Vector3i(num4 - 1f, y, num5), blockValue2, 0);
					if (!hashSet.Contains(blockChangeInfo.pos))
					{
						list.Add(blockChangeInfo);
					}
					blockChangeInfo = new BlockChangeInfo(new Vector3i(num4, y, num5 + 1f), blockValue2, 0);
					if (!hashSet.Contains(blockChangeInfo.pos))
					{
						list.Add(blockChangeInfo);
					}
					blockChangeInfo = new BlockChangeInfo(new Vector3i(num4, y, num5 - 1f), blockValue2, 0);
					if (!hashSet.Contains(blockChangeInfo.pos))
					{
						list.Add(blockChangeInfo);
					}
				}
			}
		}
		Log.Out("Setting " + list.Count.ToString() + " blocks");
		GameManager.Instance.ChangeBlocks(null, list);
		Log.Out(name + " imported");
		GameManager.bPhysicsActive = true;
		int num6 = (num + 32) / 2;
		int num7 = (num3 + 32) / 2;
		int num8 = (int)pos.x - num6;
		while ((float)num8 < pos.x + (float)num6)
		{
			int num9 = (int)pos.z - num7;
			while ((float)num9 < pos.z + (float)num7)
			{
				DynamicMeshManager.Instance.AddChunk(new Vector3i(num8, 0, num9), true);
				num9 += 16;
			}
			num8 += 16;
		}
	}

	// Token: 0x0600182A RID: 6186 RVA: 0x00094D80 File Offset: 0x00092F80
	public static void AddFallingBlockObserver(Vector3i pos)
	{
		if (DynamicMeshManager.Instance == null)
		{
			return;
		}
		if (!DynamicMeshManager.IsServer)
		{
			return;
		}
		using (List<DynamicObserver>.Enumerator enumerator = DynamicMeshManager.Instance.Observers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.ContainsPoint(pos))
				{
					return;
				}
			}
		}
		DynamicObserver dynamicObserver = new DynamicObserver();
		dynamicObserver.Start(pos.ToVector3());
		DynamicMeshManager.Instance.Observers.Add(dynamicObserver);
		DynamicMeshManager.Instance.NextFallingCheck = DateTime.Now.AddSeconds(5.0);
	}

	// Token: 0x0600182B RID: 6187 RVA: 0x00094E34 File Offset: 0x00093034
	public void CheckGameObjects()
	{
		int num = 0;
		foreach (object obj in DynamicMeshManager.ParentTransform)
		{
			GameObject gameObject = ((Transform)obj).gameObject;
			if (gameObject.name.StartsWith("C ") && !(gameObject.name == ""))
			{
				string[] array = gameObject.name.Substring(2, gameObject.name.IndexOf(" ", 3) - 3).Split(',', StringSplitOptions.None);
				int x = int.Parse(array[0]);
				int z = int.Parse(array[1]);
				DynamicMeshItem itemFromWorldPosition = this.GetItemFromWorldPosition(x, z);
				if (itemFromWorldPosition.ChunkObject == null)
				{
					this.AddObjectForDestruction(gameObject);
					num++;
				}
				if (!itemFromWorldPosition.GetRegion().IsInItemLoad())
				{
					num++;
					this.AddObjectForDestruction(gameObject);
				}
				if (itemFromWorldPosition.ChunkObject != gameObject)
				{
					num++;
					this.AddObjectForDestruction(gameObject);
				}
			}
		}
	}

	// Token: 0x0600182C RID: 6188 RVA: 0x00094F4C File Offset: 0x0009314C
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckFallingObservers()
	{
		if (this.NextFallingCheck > DateTime.Now)
		{
			return;
		}
		this.NextFallingCheck = DateTime.Now.AddSeconds(5.0);
		for (int i = this.Observers.Count - 1; i >= 0; i--)
		{
			DynamicObserver dynamicObserver = this.Observers[i];
			if (!dynamicObserver.HasFallingBlocks())
			{
				dynamicObserver.Stop();
				this.Observers.RemoveAt(i);
			}
		}
	}

	// Token: 0x04000F08 RID: 3848
	public int DebugOption;

	// Token: 0x04000F09 RID: 3849
	public float DebugY = -0.7f;

	// Token: 0x04000F0A RID: 3850
	public float TestZ = -1f;

	// Token: 0x04000F0B RID: 3851
	public static int DebugX = 1840;

	// Token: 0x04000F0C RID: 3852
	public static int DebugZ = 672;

	// Token: 0x04000F0D RID: 3853
	public static bool DebugItemPositions = false;

	// Token: 0x04000F0E RID: 3854
	public static bool DebugReleases = false;

	// Token: 0x04000F0F RID: 3855
	public int QueueDelay = 5;

	// Token: 0x04000F10 RID: 3856
	public static bool ForceMeshGeneration;

	// Token: 0x04000F11 RID: 3857
	public static string FileMissing = "FM";

	// Token: 0x04000F12 RID: 3858
	public static bool Allow32BitMeshes = true;

	// Token: 0x04000F13 RID: 3859
	public static GUIStyle DebugStyle = new GUIStyle();

	// Token: 0x04000F14 RID: 3860
	public static bool CONTENT_ENABLED = true;

	// Token: 0x04000F15 RID: 3861
	public static bool CompressFiles = true;

	// Token: 0x04000F16 RID: 3862
	public static bool DisableScopeTexture = true;

	// Token: 0x04000F17 RID: 3863
	public static int GuiY = 0;

	// Token: 0x04000F18 RID: 3864
	public static float MaxRebuildTime = 300f;

	// Token: 0x04000F19 RID: 3865
	public PrefabCheckState PrefabCheck;

	// Token: 0x04000F1A RID: 3866
	public static Rect ViewRect = default(Rect);

	// Token: 0x04000F1B RID: 3867
	public int QuadSize = 10000;

	// Token: 0x04000F1C RID: 3868
	public ConcurrentDictionary<long, DynamicMeshItem> ItemsDictionary;

	// Token: 0x04000F1D RID: 3869
	public static bool DisableLOD = false;

	// Token: 0x04000F1E RID: 3870
	public DynamicObserver Observer = new DynamicObserver();

	// Token: 0x04000F1F RID: 3871
	public DynamicObserver ObserverPrep = new DynamicObserver();

	// Token: 0x04000F20 RID: 3872
	public Vector3 ObserverPos;

	// Token: 0x04000F21 RID: 3873
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 ObserverPosNext;

	// Token: 0x04000F22 RID: 3874
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ObserverRequest ObserverRequestInfo;

	// Token: 0x04000F23 RID: 3875
	public static DynamicMeshManager Instance;

	// Token: 0x04000F24 RID: 3876
	public static bool DisableMoveVerts = false;

	// Token: 0x04000F25 RID: 3877
	public static bool ShowDebug = true;

	// Token: 0x04000F26 RID: 3878
	public Queue ToRemove = new Queue();

	// Token: 0x04000F27 RID: 3879
	public static int CombineType = 1;

	// Token: 0x04000F28 RID: 3880
	public static HashSet<long> ChunkGameObjects = new HashSet<long>();

	// Token: 0x04000F29 RID: 3881
	public List<DynamicObserver> Observers = new List<DynamicObserver>();

	// Token: 0x04000F2A RID: 3882
	public DynamicMeshServerStatus ClientMessage;

	// Token: 0x04000F2B RID: 3883
	public ConcurrentHashSet<DynamicMeshVoxelLoad> BufferRegionLoadRequests = new ConcurrentHashSet<DynamicMeshVoxelLoad>();

	// Token: 0x04000F2C RID: 3884
	public LinkedList<DynamicMeshItem> ChunkMeshLoadRequests = new LinkedList<DynamicMeshItem>();

	// Token: 0x04000F2D RID: 3885
	public object _chunkMeshDataLock = new object();

	// Token: 0x04000F2E RID: 3886
	public LinkedList<DynamicMeshVoxelLoad> ChunkMeshData = new LinkedList<DynamicMeshVoxelLoad>();

	// Token: 0x04000F2F RID: 3887
	public LinkedList<long> RegionsAvailableToLoad = new LinkedList<long>();

	// Token: 0x04000F30 RID: 3888
	public int AvailableRegionLoadRequests;

	// Token: 0x04000F31 RID: 3889
	public ConcurrentQueue<DyMeshRegionLoadRequest> RegionFileLoadRequests = new ConcurrentQueue<DyMeshRegionLoadRequest>();

	// Token: 0x04000F32 RID: 3890
	public static MicroStopwatch MeshLoadStop = new MicroStopwatch();

	// Token: 0x04000F33 RID: 3891
	public int LongestRegionLoad;

	// Token: 0x04000F34 RID: 3892
	public static bool DebugReport = false;

	// Token: 0x04000F35 RID: 3893
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float nextUpdate;

	// Token: 0x04000F36 RID: 3894
	public static bool ShowGui = false;

	// Token: 0x04000F37 RID: 3895
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float nextShowHide;

	// Token: 0x04000F38 RID: 3896
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool ProcessItemLoadReady = true;

	// Token: 0x04000F39 RID: 3897
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DateTime ForceNextItemLoad = DateTime.Now.AddDays(1.0);

	// Token: 0x04000F3A RID: 3898
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool ProcessRegionReady = true;

	// Token: 0x04000F3B RID: 3899
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DateTime ForceNextRegion = DateTime.Now.AddDays(1.0);

	// Token: 0x04000F3C RID: 3900
	public static bool testMessage = true;

	// Token: 0x04000F3D RID: 3901
	public static GameObject Parent;

	// Token: 0x04000F3E RID: 3902
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LinkedListNode<DynamicMeshItem> CachedItem;

	// Token: 0x04000F3F RID: 3903
	public float time;

	// Token: 0x04000F40 RID: 3904
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Texture2D backgroundTexture = null;

	// Token: 0x04000F41 RID: 3905
	public static float ThreadDistance;

	// Token: 0x04000F42 RID: 3906
	public static float ObserverDistance;

	// Token: 0x04000F43 RID: 3907
	public static float ItemLoadDistance;

	// Token: 0x04000F44 RID: 3908
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DynamicMeshManager.DisabledImposterChunkManager disabledImposterChunkManager;

	// Token: 0x04000F45 RID: 3909
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool disabledImposterChunksDirty;

	// Token: 0x04000F46 RID: 3910
	public List<DynamicMeshUpdateData> UpdateData = new List<DynamicMeshUpdateData>(10);

	// Token: 0x04000F47 RID: 3911
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly int PointerSize = 8;

	// Token: 0x04000F48 RID: 3912
	public ConcurrentQueue<GameObject> ToBeDestroyed = new ConcurrentQueue<GameObject>();

	// Token: 0x04000F49 RID: 3913
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DateTime nextOrphanCheck;

	// Token: 0x04000F4A RID: 3914
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public HashSet<GameObject> potentialOrphans = new HashSet<GameObject>();

	// Token: 0x04000F4B RID: 3915
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public HashSet<GameObject> checkForOrphans = new HashSet<GameObject>();

	// Token: 0x04000F4C RID: 3916
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int dtMinX;

	// Token: 0x04000F4D RID: 3917
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int dtMaxX;

	// Token: 0x04000F4E RID: 3918
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int dtMinZ;

	// Token: 0x04000F4F RID: 3919
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int dtMaxZ;

	// Token: 0x04000F50 RID: 3920
	public static bool DoLog = false;

	// Token: 0x04000F51 RID: 3921
	public static bool DoLogNet = false;

	// Token: 0x04000F52 RID: 3922
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ConcurrentQueue<Vector2i> ChunksToRemove = new ConcurrentQueue<Vector2i>();

	// Token: 0x04000F53 RID: 3923
	public DynamicMeshRegion NearestRegionWithUnloaded;

	// Token: 0x04000F54 RID: 3924
	public bool FindNearestUnloadedItems;

	// Token: 0x04000F55 RID: 3925
	public Vector3i? PrimaryLocation;

	// Token: 0x04000F56 RID: 3926
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<long, HashSet<long>> RequestKeys = new Dictionary<long, HashSet<long>>();

	// Token: 0x04000F57 RID: 3927
	public static int ShowHideCheckTime = 3000;

	// Token: 0x04000F58 RID: 3928
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int[] gcGenCount = new int[3];

	// Token: 0x04000F59 RID: 3929
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static string ChunkChangedInItemLoad = "ChunkChangedInItemLoad";

	// Token: 0x04000F5A RID: 3930
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DateTime NextFallingCheck = DateTime.Now;

	// Token: 0x02000333 RID: 819
	[PublicizedFrom(EAccessModifier.Private)]
	public class DisabledImposterChunkManager : IDisposable
	{
		// Token: 0x0600182E RID: 6190 RVA: 0x00095114 File Offset: 0x00093314
		public DisabledImposterChunkManager(DynamicMeshManager dynamicMeshManager)
		{
			this.dynamicMeshManager = dynamicMeshManager;
			Vector2i worldSize = GameManager.Instance.World.ChunkCache.ChunkProvider.GetWorldSize();
			this.worldChunkDimensions = worldSize.x / 16;
			int num = this.worldChunkDimensions * this.worldChunkDimensions;
			this.chunkClipValues = new ComputeBuffer(num, 4);
			this.chunkClipValuesArray = new int[num];
			Shader.SetGlobalInteger(DynamicMeshManager.DisabledImposterChunkManager.chunkDimensionsID, this.worldChunkDimensions);
			Shader.EnableKeyword(DynamicMeshManager.DisabledImposterChunkManager.featureKeyword);
			this.Update();
		}

		// Token: 0x0600182F RID: 6191 RVA: 0x000951A0 File Offset: 0x000933A0
		public void Update()
		{
			if (this.chunkClipValuesArray != null)
			{
				int num = this.worldChunkDimensions / 2;
				for (int i = 0; i < this.chunkClipValuesArray.Length; i++)
				{
					int x = i % this.worldChunkDimensions - num;
					int y = i / this.worldChunkDimensions - num;
					long key = WorldChunkCache.MakeChunkKey(x, y);
					if (this.dynamicMeshManager.ItemsDictionary.ContainsKey(key))
					{
						this.chunkClipValuesArray[i] = -1;
					}
					else
					{
						this.chunkClipValuesArray[i] = 1;
					}
				}
				this.chunkClipValues.SetData(this.chunkClipValuesArray);
				Shader.SetGlobalBuffer(DynamicMeshManager.DisabledImposterChunkManager.chunkBufferID, this.chunkClipValues);
			}
		}

		// Token: 0x06001830 RID: 6192 RVA: 0x00095238 File Offset: 0x00093438
		public static void DisableShaderKeyword()
		{
			Shader.DisableKeyword(DynamicMeshManager.DisabledImposterChunkManager.featureKeyword);
		}

		// Token: 0x06001831 RID: 6193 RVA: 0x00095244 File Offset: 0x00093444
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (disposing)
				{
					DynamicMeshManager.DisabledImposterChunkManager.DisableShaderKeyword();
					this.chunkClipValues.Dispose();
				}
				this.chunkClipValuesArray = null;
				this.disposedValue = true;
			}
		}

		// Token: 0x06001832 RID: 6194 RVA: 0x0009526F File Offset: 0x0009346F
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x04000F5B RID: 3931
		[PublicizedFrom(EAccessModifier.Private)]
		public const string ClippingOnKeyword = "PREFAB_CLIPPING_ON";

		// Token: 0x04000F5C RID: 3932
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly int chunkBufferID = Shader.PropertyToID("_ChunkClipValues");

		// Token: 0x04000F5D RID: 3933
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly int chunkDimensionsID = Shader.PropertyToID("_WorldChunkDimensions");

		// Token: 0x04000F5E RID: 3934
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly GlobalKeyword featureKeyword = GlobalKeyword.Create("PREFAB_CLIPPING_ON");

		// Token: 0x04000F5F RID: 3935
		[PublicizedFrom(EAccessModifier.Private)]
		public DynamicMeshManager dynamicMeshManager;

		// Token: 0x04000F60 RID: 3936
		[PublicizedFrom(EAccessModifier.Private)]
		public ComputeBuffer chunkClipValues;

		// Token: 0x04000F61 RID: 3937
		[PublicizedFrom(EAccessModifier.Private)]
		public int[] chunkClipValuesArray;

		// Token: 0x04000F62 RID: 3938
		[PublicizedFrom(EAccessModifier.Private)]
		public int worldChunkDimensions;

		// Token: 0x04000F63 RID: 3939
		[PublicizedFrom(EAccessModifier.Private)]
		public bool disposedValue;
	}
}
