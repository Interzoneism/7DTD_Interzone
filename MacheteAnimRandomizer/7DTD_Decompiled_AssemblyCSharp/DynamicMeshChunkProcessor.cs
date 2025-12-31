using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

// Token: 0x02000369 RID: 873
public class DynamicMeshChunkProcessor
{
	// Token: 0x170002DF RID: 735
	// (get) Token: 0x060019C1 RID: 6593 RVA: 0x0009836F File Offset: 0x0009656F
	public static World world
	{
		get
		{
			return GameManager.Instance.World;
		}
	}

	// Token: 0x170002E0 RID: 736
	// (get) Token: 0x060019C2 RID: 6594 RVA: 0x0009D93C File Offset: 0x0009BB3C
	public double InactiveTime
	{
		get
		{
			return (DateTime.Now - this.LastActive).TotalSeconds;
		}
	}

	// Token: 0x170002E1 RID: 737
	// (get) Token: 0x060019C3 RID: 6595 RVA: 0x00091F4F File Offset: 0x0009014F
	public static bool IsServer
	{
		get
		{
			return SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer;
		}
	}

	// Token: 0x060019C4 RID: 6596 RVA: 0x0009D964 File Offset: 0x0009BB64
	public static bool IsChunkLoaded(Chunk c)
	{
		return c != null && !c.InProgressUnloading && !c.InProgressSaving && !c.InProgressDecorating && !c.InProgressCopying && !c.InProgressLighting && !c.InProgressNetworking && !c.InProgressRegeneration && c != null && !c.IsLocked && !c.IsLockedExceptUnloading;
	}

	// Token: 0x060019C5 RID: 6597 RVA: 0x0009D9D4 File Offset: 0x0009BBD4
	public int AddNewItem(DynamicMeshItem item, bool isPrimary)
	{
		if (this.Status != DynamicMeshBuilderStatus.Ready)
		{
			Log.Warning("Builder thread tried to start on item " + item.ToDebugLocation() + " when not ready. Current Status: " + this.Status.ToString());
			return 0;
		}
		if (GameManager.IsDedicatedServer)
		{
			DyMeshData fromCache = DyMeshData.GetFromCache();
			if (fromCache == null)
			{
				return -2;
			}
			this.ChunkData = fromCache;
		}
		this.Item = item;
		this.IsPrimaryQueue = isPrimary;
		this.LastActive = DateTime.Now;
		DynamicMeshThread.ChunkDataQueue.MarkAsUpdating(this.Item);
		this.Status = DynamicMeshBuilderStatus.StartingExport;
		return 1;
	}

	// Token: 0x060019C6 RID: 6598 RVA: 0x0009DA64 File Offset: 0x0009BC64
	public int AddRegenerateRegion(DynamicMeshThread.ThreadRegion region)
	{
		if (this.Status != DynamicMeshBuilderStatus.Ready)
		{
			Log.Warning("Builder thread tried to start on item " + region.ToDebugLocation() + " when not ready. Current Status: " + this.Status.ToString());
			return 0;
		}
		DyMeshData fromCache = DyMeshData.GetFromCache();
		if (fromCache == null)
		{
			return -2;
		}
		this.ChunkData = fromCache;
		this.Region = region;
		this.IsPrimaryQueue = false;
		this.LastActive = DateTime.Now;
		this.Status = DynamicMeshBuilderStatus.StartingRegionRegen;
		return 1;
	}

	// Token: 0x060019C7 RID: 6599 RVA: 0x0009DADC File Offset: 0x0009BCDC
	public int AddItemForMeshPreview(DynamicMeshItem item, ChunkPreviewData previewData)
	{
		if (this.Status != DynamicMeshBuilderStatus.Ready)
		{
			Log.Warning("Builder thread tried to start preview when not ready. Current Status: " + this.Status.ToString());
			return 0;
		}
		if (this.Item != null)
		{
			Log.Error("Item was not null on chunk mesh thread: " + this.Item.ToDebugLocation());
		}
		DyMeshData fromCache = DyMeshData.GetFromCache();
		if (fromCache == null)
		{
			return -2;
		}
		this.ChunkData = fromCache;
		this.Item = item;
		this.PreviewData = previewData;
		this.IsPrimaryQueue = false;
		this.LastActive = DateTime.Now;
		this.Status = DynamicMeshBuilderStatus.StartingPreview;
		return 1;
	}

	// Token: 0x060019C8 RID: 6600 RVA: 0x0009DB70 File Offset: 0x0009BD70
	public int AddItemForMeshGeneration(DynamicMeshItem item, bool isPrimary)
	{
		if (this.Status != DynamicMeshBuilderStatus.Ready)
		{
			Log.Warning("Builder thread tried to start on item " + item.ToDebugLocation() + " when not ready. Current Status: " + this.Status.ToString());
			return 0;
		}
		if (this.Item != null)
		{
			Log.Error("Item was not null on chunk mesh thread: " + this.Item.ToDebugLocation());
		}
		DyMeshData fromCache = DyMeshData.GetFromCache();
		if (fromCache == null)
		{
			return -2;
		}
		this.ChunkData = fromCache;
		this.Item = item;
		this.IsPrimaryQueue = isPrimary;
		this.LastActive = DateTime.Now;
		DynamicMeshThread.ChunkDataQueue.MarkAsGenerating(this.Item);
		this.Status = DynamicMeshBuilderStatus.StartingGeneration;
		return 1;
	}

	// Token: 0x060019C9 RID: 6601 RVA: 0x0009DC18 File Offset: 0x0009BE18
	public void CleanUp()
	{
		this.MeshLayer.Cleanup();
		this.MeshLayer = null;
	}

	// Token: 0x060019CA RID: 6602 RVA: 0x0009DC2C File Offset: 0x0009BE2C
	public void RequestStop(bool forceStop = false)
	{
		this.StopRequested = true;
		if (forceStop)
		{
			try
			{
				Thread thread = this.thread;
				if (thread != null)
				{
					thread.Abort();
				}
			}
			catch (Exception)
			{
			}
		}
		this.Status = DynamicMeshBuilderStatus.Stopped;
	}

	// Token: 0x060019CB RID: 6603 RVA: 0x0009DC70 File Offset: 0x0009BE70
	public void StartThread()
	{
		if (DynamicMeshChunkProcessor.DebugOnMainThread)
		{
			return;
		}
		this.thread = new Thread(delegate()
		{
			try
			{
				while (!(GameManager.Instance == null) && GameManager.Instance.World != null)
				{
					if ((this.Item == null && this.Region == null) || this.Status == DynamicMeshBuilderStatus.Ready || this.Status == DynamicMeshBuilderStatus.Complete)
					{
						if (this.StopRequested)
						{
							break;
						}
						Thread.Sleep(100);
					}
					else
					{
						this.RunJob();
					}
				}
			}
			catch (Exception ex)
			{
				string str = "Builder error: ";
				string message = ex.Message;
				string str2 = "\n";
				string stackTrace = ex.StackTrace;
				this.Error = str + message + str2 + ((stackTrace != null) ? stackTrace.ToString() : null);
				if (!this.StopRequested)
				{
					Log.Error(this.Error);
				}
			}
			this.Status = DynamicMeshBuilderStatus.Stopped;
		});
		this.thread.Name = this.ThreadName;
		this.thread.Priority = System.Threading.ThreadPriority.Lowest;
		this.thread.Start();
	}

	// Token: 0x060019CC RID: 6604 RVA: 0x0009DCC4 File Offset: 0x0009BEC4
	public void RunJob()
	{
		if (this.Status == DynamicMeshBuilderStatus.Ready || this.Status == DynamicMeshBuilderStatus.Complete || this.Status == DynamicMeshBuilderStatus.PreviewComplete || this.Status == DynamicMeshBuilderStatus.Error)
		{
			return;
		}
		if (this.Status != DynamicMeshBuilderStatus.StartingExport && this.Status != DynamicMeshBuilderStatus.StartingGeneration && this.Status != DynamicMeshBuilderStatus.StartingPreview && this.Status != DynamicMeshBuilderStatus.StartingRegionRegen)
		{
			this.Status = DynamicMeshBuilderStatus.Error;
			return;
		}
		if (this.Status == DynamicMeshBuilderStatus.StartingExport)
		{
			this.ExportChunk(this.Item);
			return;
		}
		if (this.Status == DynamicMeshBuilderStatus.StartingGeneration)
		{
			this.CreateMesh(this.Item);
			return;
		}
		if (this.Status == DynamicMeshBuilderStatus.StartingPreview)
		{
			this.CreatePreviewMeshJob();
			return;
		}
		if (this.Status == DynamicMeshBuilderStatus.StartingRegionRegen)
		{
			this.RegenerateRegion(this.Region);
		}
	}

	// Token: 0x060019CD RID: 6605 RVA: 0x0009DD78 File Offset: 0x0009BF78
	[PublicizedFrom(EAccessModifier.Protected)]
	public ExportMeshResult SetResultPreview(ExportMeshResult result)
	{
		this.Result = result;
		this.ChunkData = DyMeshData.AddToCache(this.ChunkData);
		this.Status = DynamicMeshBuilderStatus.PreviewComplete;
		this.LastActive = DateTime.Now;
		return result;
	}

	// Token: 0x060019CE RID: 6606 RVA: 0x0009DDA8 File Offset: 0x0009BFA8
	[PublicizedFrom(EAccessModifier.Protected)]
	public ExportMeshResult SetResult(ExportMeshResult result)
	{
		this.Result = result;
		this.Status = DynamicMeshBuilderStatus.Complete;
		this.LastActive = DateTime.Now;
		if (this.Item != null)
		{
			DynamicMeshThread.ChunkDataQueue.MarkAsUpdated(this.Item);
			DynamicMeshThread.ChunkDataQueue.MarkAsGenerated(this.Item);
		}
		return result;
	}

	// Token: 0x060019CF RID: 6607 RVA: 0x0009DDF8 File Offset: 0x0009BFF8
	public void Init(int id)
	{
		this.LastActive = DateTime.Now;
		this.cacheNeighbourChunks = new ChunkCacheNeighborChunks(GameManager.Instance.World.ChunkCache);
		this.cacheBlocks = new ChunkCacheNeighborBlocks(this.cacheNeighbourChunks);
		this.meshGen = new MeshGeneratorMC2(this.cacheBlocks, this.cacheNeighbourChunks);
		this.ThreadName = "DymeshThread" + id.ToString();
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				for (int k = 0; k < 256; k++)
				{
					Chunk[] fakeChunks = this.FakeChunks;
					for (int l = 0; l < fakeChunks.Length; l++)
					{
						fakeChunks[l].SetLight(i, k, j, 15, Chunk.LIGHT_TYPE.SUN);
					}
				}
			}
		}
	}

	// Token: 0x060019D0 RID: 6608 RVA: 0x0009DEBC File Offset: 0x0009C0BC
	public ExportMeshResult CreatePreviewMeshJob()
	{
		ExportMeshResult resultPreview = this.CreatePreviewMesh();
		return this.SetResultPreview(resultPreview);
	}

	// Token: 0x060019D1 RID: 6609 RVA: 0x0009DED8 File Offset: 0x0009C0D8
	public ExportMeshResult CreateMesh(DynamicMeshItem item)
	{
		ExportMeshResult result = this.CreateMesh(item.Key, false);
		return this.SetResult(result);
	}

	// Token: 0x060019D2 RID: 6610 RVA: 0x0009DEFC File Offset: 0x0009C0FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void RenderChunkMeshToData(Vector3i worldPosition, Chunk chunk, DyMeshData data, bool fullHeight)
	{
		chunk.NeedsRegeneration = true;
		bool flag = false;
		for (int i = 0; i < 16; i++)
		{
			if ((chunk.NeedsRegenerationAt & 1 << i) != 0)
			{
				if (this.meshGen.IsLayerEmpty(i))
				{
					chunk.ClearNeedsRegenerationAt(i);
					this.MeshLayer.idx = -1;
				}
				else
				{
					this.MeshLayer.idx = i;
				}
			}
			if (this.MeshLayer.idx != -1)
			{
				chunk.ClearNeedsRegenerationAt(this.MeshLayer.idx);
				this.meshGen.GenerateMesh(worldPosition, this.MeshLayer.idx, this.MeshLayer.meshes);
				VoxelMesh voxelMesh = this.MeshLayer.meshes[0];
				if (voxelMesh.Vertices.Count != 0)
				{
					flag = true;
					DynamicMeshChunkProcessor.LoadChunkIntoRegion(worldPosition.x, worldPosition.z, worldPosition, null, voxelMesh, this.ChunkData.OpaqueMesh, this.ChunkData.TerrainMesh, 0);
				}
				VoxelMesh voxelMesh2 = this.MeshLayer.meshes[5];
				if (fullHeight || (flag && voxelMesh2.Vertices.Count != 0 && this.MeshLayer.idx * 16 + 16 >= this.MinTerrainHeight))
				{
					DynamicMeshChunkProcessor.LoadChunkIntoRegion(worldPosition.x, worldPosition.z, worldPosition, null, voxelMesh2, this.ChunkData.OpaqueMesh, this.ChunkData.TerrainMesh, 0);
				}
				DyMeshData.ResetMeshLayer(this.MeshLayer);
			}
		}
	}

	// Token: 0x060019D3 RID: 6611 RVA: 0x0009E060 File Offset: 0x0009C260
	[PublicizedFrom(EAccessModifier.Private)]
	public ExportMeshResult CreateMesh(long key, bool isRegionRegen)
	{
		Vector3i worldPosFromKey = DynamicMeshUnity.GetWorldPosFromKey(key);
		if (DynamicMeshThread.NoProcessing)
		{
			return ExportMeshResult.Success;
		}
		DateTime now = DateTime.Now;
		int num = 0;
		DynamicMeshChunkDataWrapper dynamicMeshChunkDataWrapper;
		string str;
		while (!DynamicMeshThread.ChunkDataQueue.CollectItem(key, out dynamicMeshChunkDataWrapper, out str) && num++ <= 5)
		{
			Log.Out(DynamicMeshUnity.GetWorldPosFromKey(key).ToString() + " Waiting for chunk: " + str);
			Thread.Sleep(100);
		}
		DynamicMeshChunkData data = dynamicMeshChunkDataWrapper.Data;
		if (data == null)
		{
			DynamicMeshThread.ChunkDataQueue.ClearLock(dynamicMeshChunkDataWrapper, "_CREATEDATAMISSING_");
			if (!isRegionRegen)
			{
				this.ChunkData = DyMeshData.AddToCache(this.ChunkData);
			}
			return ExportMeshResult.ChunkMissing;
		}
		ChunkCacheNeighborChunks chunkCacheNeighborChunks = this.cacheNeighbourChunks;
		IChunk chunk = this.chunk;
		IChunk[] fakeChunks = this.FakeChunks;
		chunkCacheNeighborChunks.Init(chunk, fakeChunks);
		data.ApplyToChunk(this.chunk, this.cacheNeighbourChunks);
		byte mainBiome = data.MainBiome;
		this.MinTerrainHeight = data.MinTerrainHeight;
		this.yOffset = data.OffsetY;
		this.EndY = data.EndY;
		DynamicMeshThread.ChunkDataQueue.ClearLock(dynamicMeshChunkDataWrapper, "_CREATEMESHRELEASE_");
		this.RenderChunkMeshToData(worldPosFromKey, this.chunk, this.ChunkData, false);
		this.cacheNeighbourChunks.Clear();
		if (this.ChunkData.OpaqueMesh.Vertices.Count == 0 && !isRegionRegen)
		{
			string itemPath = DynamicMeshUnity.GetItemPath(this.Item.Key);
			DynamicMeshThread.ChunkDataQueue.MarkForDeletion(key);
			num = 0;
			while (!DynamicMeshThread.ChunkDataQueue.CollectItem(key, out dynamicMeshChunkDataWrapper, out str))
			{
				if (num++ > 5)
				{
					Log.Warning("Failed to take lock for chunk deletion");
					break;
				}
				Log.Out(DynamicMeshUnity.GetWorldPosFromKey(key).ToString() + " Waiting for chunk deletion: " + str);
				Thread.Sleep(100);
			}
			if (SdFile.Exists(itemPath))
			{
				SdFile.Delete(itemPath);
			}
			DynamicMeshThread.ChunkDataQueue.ClearLock(dynamicMeshChunkDataWrapper, "_CreateMeshDeleteFile");
			this.Item.DestroyMesh();
			if (DynamicMeshChunkProcessor.IsServer)
			{
				DynamicMeshServer.SendToAllClients(this.Item, true);
			}
			DynamicMeshManager.Instance.ArrangeChunkRemoval(worldPosFromKey.x, worldPosFromKey.z);
			DynamicMeshThread.RemoveRegionChunk(worldPosFromKey.x, worldPosFromKey.z, this.Item.Key);
			this.ChunkData = DyMeshData.AddToCache(this.ChunkData);
			return ExportMeshResult.SuccessNoLoad;
		}
		if (GameManager.IsDedicatedServer)
		{
			return ExportMeshResult.Success;
		}
		if (this.ChunkData.OpaqueMesh.Vertices.Count > 0 && this.ChunkData.OpaqueMesh.Normals.Count == 0)
		{
			MeshCalculations.RecalculateNormals(this.ChunkData.OpaqueMesh.Vertices, this.ChunkData.OpaqueMesh.Indices, this.ChunkData.OpaqueMesh.Normals, 60f);
		}
		if (this.ChunkData.OpaqueMesh.Vertices.Count > 0 && this.ChunkData.OpaqueMesh.Tangents.Count == 0)
		{
			MeshCalculations.CalculateMeshTangents(this.ChunkData.OpaqueMesh.Vertices, this.ChunkData.OpaqueMesh.Indices, this.ChunkData.OpaqueMesh.Normals, this.ChunkData.OpaqueMesh.Uvs, this.ChunkData.OpaqueMesh.Tangents, false);
		}
		if (this.ChunkData.TerrainMesh.Vertices.Count > 0)
		{
			int count = this.ChunkData.TerrainMesh.Normals.Count;
		}
		this.ExportTime = (DateTime.Now - now).TotalMilliseconds;
		if (!isRegionRegen)
		{
			DynamicMeshVoxelLoad loadData = DynamicMeshVoxelLoad.Create(this.Item, this.ChunkData);
			this.ChunkData = null;
			DynamicMeshManager.Instance.AddChunkLoadData(loadData);
		}
		return ExportMeshResult.Success;
	}

	// Token: 0x060019D4 RID: 6612 RVA: 0x0009E40C File Offset: 0x0009C60C
	public ExportMeshResult CreatePreviewMesh()
	{
		WorldBase world = GameManager.Instance.World;
		DateTime now = DateTime.Now;
		Vector3i worldPosition = this.Item.WorldPosition;
		Chunk chunk = (Chunk)world.GetChunkFromWorldPos(worldPosition);
		if (chunk == null)
		{
			return ExportMeshResult.PreviewMissing;
		}
		if (!GameManager.Instance.World.ChunkCache.GetNeighborChunks(chunk, this.neighbours))
		{
			DynamicMeshThread.SetNextChunks(chunk.Key);
			return ExportMeshResult.PreviewDelay;
		}
		this.chunk.X = chunk.X;
		this.chunk.Z = chunk.Z;
		for (int i = 0; i < this.FakeChunks.Length; i++)
		{
			this.CopyPreviewChunk(this.neighbours[i], this.FakeChunks[i], true);
		}
		ChunkCacheNeighborChunks chunkCacheNeighborChunks = this.cacheNeighbourChunks;
		IChunk chunk2 = chunk;
		IChunk[] fakeChunks = this.FakeChunks;
		chunkCacheNeighborChunks.Init(chunk2, fakeChunks);
		Chunk chunk3 = this.CopyPreviewChunk(chunk, this.chunk, false);
		ChunkCacheNeighborChunks chunkCacheNeighborChunks2 = this.cacheNeighbourChunks;
		IChunk chunk4 = chunk3;
		fakeChunks = this.FakeChunks;
		chunkCacheNeighborChunks2.Init(chunk4, fakeChunks);
		for (int j = -1; j < 2; j++)
		{
			int k = -1;
			while (k < 2)
			{
				if (this.cacheNeighbourChunks[j, k] == null)
				{
					if (!DynamicMeshChunkProcessor.IsChunkLoaded((Chunk)this.cacheNeighbourChunks[j, k]))
					{
						return ExportMeshResult.PreviewDelay;
					}
					return ExportMeshResult.PreviewMissing;
				}
				else
				{
					if (!DynamicMeshChunkProcessor.IsChunkLoaded((Chunk)this.cacheNeighbourChunks[j, k]))
					{
						return ExportMeshResult.PreviewDelay;
					}
					k++;
				}
			}
		}
		this.RenderChunkMeshToData(worldPosition, this.chunk, this.ChunkData, true);
		if (this.ChunkData.OpaqueMesh.Vertices.Count > 0 && this.ChunkData.OpaqueMesh.Normals.Count == 0)
		{
			MeshCalculations.RecalculateNormals(this.ChunkData.OpaqueMesh.Vertices, this.ChunkData.OpaqueMesh.Indices, this.ChunkData.OpaqueMesh.Normals, 60f);
		}
		if (this.ChunkData.OpaqueMesh.Vertices.Count > 0 && this.ChunkData.OpaqueMesh.Tangents.Count == 0)
		{
			MeshCalculations.CalculateMeshTangents(this.ChunkData.OpaqueMesh.Vertices, this.ChunkData.OpaqueMesh.Indices, this.ChunkData.OpaqueMesh.Normals, this.ChunkData.OpaqueMesh.Uvs, this.ChunkData.OpaqueMesh.Tangents, false);
		}
		if (this.ChunkData.TerrainMesh.Vertices.Count > 0)
		{
			int count = this.ChunkData.TerrainMesh.Normals.Count;
		}
		DynamicMeshVoxelLoad loadData = DynamicMeshVoxelLoad.Create(this.Item, this.ChunkData);
		this.ChunkData = null;
		ChunkPreviewManager instance = ChunkPreviewManager.Instance;
		if (instance != null)
		{
			instance.AddChunkPreviewLoadData(loadData);
		}
		return ExportMeshResult.PreviewSuccess;
	}

	// Token: 0x060019D5 RID: 6613 RVA: 0x0009E6C8 File Offset: 0x0009C8C8
	public ExportMeshResult ExportChunk(DynamicMeshItem item)
	{
		WorldBase world = GameManager.Instance.World;
		DateTime now = DateTime.Now;
		Vector3i worldPosition = item.WorldPosition;
		Chunk chunk = (Chunk)world.GetChunkFromWorldPos(worldPosition);
		if (chunk == null)
		{
			return this.SetResult(ExportMeshResult.Missing);
		}
		if (!GameManager.Instance.World.ChunkCache.GetNeighborChunks(chunk, this.neighbours))
		{
			DynamicMeshThread.SetNextChunks(chunk.Key);
			return this.SetResult(ExportMeshResult.Delay);
		}
		ChunkCacheNeighborChunks chunkCacheNeighborChunks = this.cacheNeighbourChunks;
		IChunk chunk2 = chunk;
		IChunk[] chunkArr = this.neighbours;
		chunkCacheNeighborChunks.Init(chunk2, chunkArr);
		for (int i = -1; i < 2; i++)
		{
			int j = -1;
			while (j < 2)
			{
				if (this.cacheNeighbourChunks[i, j] == null)
				{
					if (!DynamicMeshChunkProcessor.IsChunkLoaded((Chunk)this.cacheNeighbourChunks[i, j]))
					{
						return this.SetResult(ExportMeshResult.Delay);
					}
					return this.SetResult(ExportMeshResult.Missing);
				}
				else
				{
					if (!DynamicMeshChunkProcessor.IsChunkLoaded((Chunk)this.cacheNeighbourChunks[i, j]))
					{
						return this.SetResult(ExportMeshResult.Delay);
					}
					j++;
				}
			}
		}
		DynamicMeshManager.ThreadDistance = item.DistanceToPlayer();
		DynamicMeshChunkData fromCache = DynamicMeshChunkData.GetFromCache("_EXPORT_");
		bool flag = this.CopyChunkFromWorld(chunk, fromCache, false);
		this.MeshDataTime = (DateTime.Now - now).TotalMilliseconds;
		if (!flag || fromCache.BlockRaw.Count == 0 || fromCache.Height.Count == 0)
		{
			string path = DynamicMeshFile.MeshLocation + item.Key.ToString() + ".update";
			DynamicMeshThread.ChunkDataQueue.MarkForDeletion(item.Key);
			if (SdFile.Exists(path))
			{
				SdFile.Delete(path);
			}
			item.DestroyMesh();
			if (DynamicMeshChunkProcessor.IsServer)
			{
				DynamicMeshServer.SendToAllClients(item, true);
			}
			DynamicMeshManager.Instance.ArrangeChunkRemoval(item.WorldPosition.x, item.WorldPosition.z);
			DynamicMeshChunkData.AddToCache(fromCache, "_noDataRelease_");
			return this.SetResult(ExportMeshResult.SuccessNoLoad);
		}
		DynamicMeshThread.ChunkDataQueue.AddSaveRequest(item.Key, fromCache);
		DynamicMeshThread.AddRegionUpdateData(item.WorldPosition.x, item.WorldPosition.z, false);
		ExportMeshResult exportMeshResult = GameManager.IsDedicatedServer ? this.CreateMesh(item) : ExportMeshResult.Success;
		if (DynamicMeshChunkProcessor.IsServer && exportMeshResult == ExportMeshResult.Success)
		{
			DynamicMeshServer.SendToAllClients(item, false);
		}
		return this.SetResult(ExportMeshResult.Success);
	}

	// Token: 0x060019D6 RID: 6614 RVA: 0x0009E900 File Offset: 0x0009CB00
	public Chunk CopyPreviewChunk(Chunk chunkInWorld, Chunk previewChunk, bool isNeighbour)
	{
		Vector3i worldPos = chunkInWorld.GetWorldPos();
		Vector3i worldPosition = this.PreviewData.WorldPosition;
		Prefab prefabData = this.PreviewData.PrefabData;
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				for (int k = 0; k < 16; k++)
				{
					if (!isNeighbour || j == 0 || j == 1 || j == 14 || j == 15 || k == 0 || k == 1 || k == 14 || k == 15)
					{
						int num = worldPos.x + j - worldPosition.x;
						int num2 = i - worldPosition.y;
						int num3 = worldPos.z + k - worldPosition.z;
						BlockValue block;
						WaterValue water;
						sbyte density;
						TextureFullArray texturefullArray;
						if (num >= 0 && num2 >= 0 && num3 >= 0 && num < prefabData.size.x && num3 < prefabData.size.z && num2 < prefabData.size.y)
						{
							block = prefabData.GetBlock(num, num2, num3);
							water = prefabData.GetWater(num, num2, num3);
							density = prefabData.GetDensity(num, num2, num3);
							texturefullArray = prefabData.GetTexture(num, num2, num3);
						}
						else
						{
							block = chunkInWorld.GetBlock(j, i, k);
							water = chunkInWorld.GetWater(j, i, k);
							density = chunkInWorld.GetDensity(j, i, k);
							texturefullArray = chunkInWorld.GetTextureFullArray(j, i, k, true);
						}
						previewChunk.SetBlockRaw(j, i, k, block);
						previewChunk.SetWater(j, i, k, water);
						previewChunk.SetDensity(j, i, k, density);
						previewChunk.GetSetTextureFullArray(j, i, k, texturefullArray);
					}
				}
			}
		}
		return previewChunk;
	}

	// Token: 0x060019D7 RID: 6615 RVA: 0x0009EABC File Offset: 0x0009CCBC
	public bool CopyChunkFromWorld(Chunk chunkInWorld, DynamicMeshChunkData data, bool fullCopy)
	{
		data.Reset();
		bool flag = false;
		bool flag2 = false;
		this.MinTerrainHeight = 500;
		data.X = chunkInWorld.X * 16;
		data.Z = chunkInWorld.Z * 16;
		data.MainBiome = chunkInWorld.DominantBiome;
		data.SetTopSoil(chunkInWorld.GetTopSoil());
		for (int i = 0; i < 256; i++)
		{
			data.RecordCounts();
			bool flag3 = false;
			for (int j = 0; j < 16; j++)
			{
				for (int k = 0; k < 16; k++)
				{
					if (i == 0)
					{
						byte terrainHeight = chunkInWorld.GetTerrainHeight(j, k);
						byte height = chunkInWorld.GetHeight(j, k);
						byte b = Math.Min(terrainHeight, height);
						data.TerrainHeight.Add(b);
						data.Height.Add(b);
						this.MinTerrainHeight = (data.MinTerrainHeight = Math.Min(this.MinTerrainHeight, (int)b));
					}
					bool flag4 = i > this.MinTerrainHeight;
					if (j == 0)
					{
						DynamicMeshChunkData.ChunkNeighbourData neighbourData = data.GetNeighbourData(-1, 0);
						IChunk chunk = this.cacheNeighbourChunks[-1, 0];
						BlockValue blockValue = flag4 ? chunk.GetBlock(15, i, k) : BlockValue.Air;
						sbyte density = flag4 ? chunk.GetDensity(15, i, k) : 0;
						long texture = flag4 ? chunk.GetTextureFull(15, i, k, 0) : 0L;
						neighbourData.SetData(blockValue.rawData, density, texture);
						if (k == 0)
						{
							DynamicMeshChunkData.ChunkNeighbourData neighbourData2 = data.GetNeighbourData(-1, -1);
							IChunk chunk2 = this.cacheNeighbourChunks[-1, -1];
							BlockValue block = chunk2.GetBlock(15, i, 15);
							sbyte density2 = chunk2.GetDensity(15, i, 15);
							long textureFull = chunk2.GetTextureFull(15, i, 15, 0);
							neighbourData2.SetData(block.rawData, density2, textureFull);
						}
						if (k == 15)
						{
							DynamicMeshChunkData.ChunkNeighbourData neighbourData3 = data.GetNeighbourData(-1, 1);
							IChunk chunk3 = this.cacheNeighbourChunks[-1, 1];
							BlockValue block2 = chunk3.GetBlock(15, i, 0);
							sbyte density3 = chunk3.GetDensity(15, i, 0);
							long textureFull2 = chunk3.GetTextureFull(15, i, 0, 0);
							neighbourData3.SetData(block2.rawData, density3, textureFull2);
						}
					}
					else if (j == 15)
					{
						DynamicMeshChunkData.ChunkNeighbourData neighbourData4 = data.GetNeighbourData(1, 0);
						IChunk chunk4 = this.cacheNeighbourChunks[1, 0];
						BlockValue blockValue2 = flag4 ? chunk4.GetBlock(0, i, k) : BlockValue.Air;
						sbyte density4 = flag4 ? chunk4.GetDensity(0, i, k) : 0;
						long texture2 = flag4 ? chunk4.GetTextureFull(0, i, k, 0) : 0L;
						neighbourData4.SetData(blockValue2.rawData, density4, texture2);
						if (k == 0)
						{
							DynamicMeshChunkData.ChunkNeighbourData neighbourData5 = data.GetNeighbourData(1, -1);
							IChunk chunk5 = this.cacheNeighbourChunks[1, -1];
							BlockValue block3 = chunk5.GetBlock(0, i, 0);
							sbyte density5 = chunk5.GetDensity(0, i, 0);
							long textureFull3 = chunk5.GetTextureFull(0, i, 0, 0);
							neighbourData5.SetData(block3.rawData, density5, textureFull3);
						}
						if (k == 15)
						{
							DynamicMeshChunkData.ChunkNeighbourData neighbourData6 = data.GetNeighbourData(1, 1);
							IChunk chunk6 = this.cacheNeighbourChunks[1, 1];
							BlockValue block4 = chunk6.GetBlock(0, i, 0);
							sbyte density6 = chunk6.GetDensity(0, i, 0);
							long textureFull4 = chunk6.GetTextureFull(0, i, 0, 0);
							neighbourData6.SetData(block4.rawData, density6, textureFull4);
						}
					}
					if (k == 0)
					{
						DynamicMeshChunkData.ChunkNeighbourData neighbourData7 = data.GetNeighbourData(0, -1);
						IChunk chunk7 = this.cacheNeighbourChunks[0, -1];
						BlockValue blockValue3 = flag4 ? chunk7.GetBlock(j, i, 15) : BlockValue.Air;
						sbyte density7 = flag4 ? chunk7.GetDensity(j, i, 15) : 0;
						long texture3 = flag4 ? chunk7.GetTextureFull(j, i, 15, 0) : 0L;
						neighbourData7.SetData(blockValue3.rawData, density7, texture3);
					}
					else if (k == 15)
					{
						DynamicMeshChunkData.ChunkNeighbourData neighbourData8 = data.GetNeighbourData(0, 1);
						IChunk chunk8 = this.cacheNeighbourChunks[0, 1];
						BlockValue blockValue4 = flag4 ? chunk8.GetBlock(j, i, 0) : BlockValue.Air;
						sbyte density8 = flag4 ? chunk8.GetDensity(j, i, 0) : 0;
						long texture4 = flag4 ? chunk8.GetTextureFull(j, i, 0, 0) : 0L;
						neighbourData8.SetData(blockValue4.rawData, density8, texture4);
					}
					BlockValue block5 = chunkInWorld.GetBlock(j, i, k);
					Block block6 = Block.list[block5.type];
					bool flag5 = fullCopy || DynamicMeshBlockSwap.OpaqueBlocks.Contains(block5.type);
					bool flag6 = DynamicMeshBlockSwap.TerrainBlocks.Contains(block5.type);
					flag2 = (flag2 || flag5);
					bool flag7 = DynamicMeshBlockSwap.DoorBlocks.Contains(block5.type);
					bool flag8 = flag5 || flag7 || (flag2 && flag6);
					if (flag8)
					{
						this.EndY = i + 1;
						if (block5.type == 0)
						{
							data.BlockRaw.Add(0U);
						}
						else if (flag7)
						{
							data.BlockRaw.Add(DynamicMeshBlockSwap.DoorReplacement.rawData);
							data.Densities.Add(MarchingCubes.DensityAir);
							data.Textures.Add(0L);
						}
						else
						{
							long item = block6.shape.IsTerrain() ? 0L : chunkInWorld.GetTextureFull(j, i, k, 0);
							sbyte item2 = block6.shape.IsTerrain() ? chunkInWorld.GetDensity(j, i, k) : 0;
							if (block5.type == 0)
							{
								data.BlockRaw.Add(0U);
							}
							else
							{
								data.BlockRaw.Add(block5.rawData);
								data.Densities.Add(item2);
								data.Textures.Add(item);
							}
						}
						if (!flag && flag5)
						{
							this.yOffset = i;
							flag = true;
						}
					}
					else
					{
						data.BlockRaw.Add(0U);
					}
					flag3 = (flag3 || flag8);
				}
			}
			if (!flag2)
			{
				data.ClearPreviousLayers();
			}
		}
		data.OffsetY = (this.yOffset = Math.Max(0, this.yOffset));
		data.EndY = this.EndY;
		return flag2;
	}

	// Token: 0x060019D8 RID: 6616 RVA: 0x0009F098 File Offset: 0x0009D298
	public ExportMeshResult RegenerateRegion(DynamicMeshThread.ThreadRegion region)
	{
		DateTime now = DateTime.Now;
		Vector3i worldPosition = region.ToWorldPosition();
		if (DynamicMeshChunkProcessor.RegionMeshData.OpaqueMesh.Vertices.Count > 0)
		{
			Log.Warning("Region object was already in use. Only one region can be regenerated at a time");
		}
		DyMeshData regionMeshData = DynamicMeshChunkProcessor.RegionMeshData;
		region.CopyLoadedChunks(this.ChunkKeys);
		foreach (long key in this.ChunkKeys)
		{
			Vector3i worldPosFromKey = DynamicMeshUnity.GetWorldPosFromKey(key);
			this.ChunkData.Reset();
			if (this.CreateMesh(key, true) == ExportMeshResult.Success)
			{
				DynamicMeshChunkProcessor.LoadChunkIntoRegion(region.X, region.Z, worldPosFromKey, region, this.ChunkData.OpaqueMesh, regionMeshData.OpaqueMesh, regionMeshData.TerrainMesh, 128);
				DynamicMeshChunkProcessor.LoadChunkIntoRegion(region.X, region.Z, worldPosFromKey, region, this.ChunkData.TerrainMesh, regionMeshData.OpaqueMesh, regionMeshData.TerrainMesh, 128);
			}
		}
		MeshCalculations.RecalculateNormals(regionMeshData.TerrainMesh.Vertices, regionMeshData.TerrainMesh.Indices, regionMeshData.TerrainMesh.Normals, 60f);
		DynamicMeshThread.RegionStorage.SaveRegion(region, worldPosition, regionMeshData.OpaqueMesh, regionMeshData.TerrainMesh);
		DynamicMeshManager.AddRegionLoadMeshes(region.Key);
		regionMeshData.Reset();
		this.ChunkData = DyMeshData.AddToCache(this.ChunkData);
		DynamicMeshThread.RegionUpdatesDebug = string.Concat(new string[]
		{
			region.ToDebugLocation(),
			" with ",
			region.LoadedChunkCount.ToString(),
			" took ",
			((int)(DateTime.Now - now).TotalMilliseconds).ToString(),
			"ms"
		});
		return this.SetResult(ExportMeshResult.Success);
	}

	// Token: 0x060019D9 RID: 6617 RVA: 0x0009F27C File Offset: 0x0009D47C
	public static void LoadChunkIntoRegion(int regionX, int regionZ, Vector3i chunkPos, DynamicMeshThread.ThreadRegion region, VoxelMesh voxelMeshToCopyFrom, VoxelMesh opaqueMesh, VoxelMeshTerrain terrainMesh, byte biomeId)
	{
		if (voxelMeshToCopyFrom == null || voxelMeshToCopyFrom.Vertices.Count == 0)
		{
			return;
		}
		int num = chunkPos.x - regionX;
		int num2 = 0;
		int num3 = chunkPos.z - regionZ;
		Vector3 b = new Vector3((float)num, (float)num2, (float)num3);
		if (voxelMeshToCopyFrom is VoxelMeshTerrain)
		{
			VoxelMeshTerrain voxelMeshTerrain = voxelMeshToCopyFrom as VoxelMeshTerrain;
			int count = terrainMesh.Vertices.Count;
			terrainMesh.Vertices.Grow(count + voxelMeshTerrain.Vertices.Count);
			for (int i = 0; i < voxelMeshTerrain.Vertices.Count; i++)
			{
				terrainMesh.Vertices.Add(voxelMeshTerrain.Vertices[i] + b);
			}
			terrainMesh.Uvs.AddRange(voxelMeshTerrain.Uvs.Items, 0, voxelMeshTerrain.Uvs.Count);
			terrainMesh.UvsCrack.AddRange(voxelMeshTerrain.UvsCrack.Items, 0, voxelMeshTerrain.UvsCrack.Count);
			terrainMesh.Uvs3.AddRange(voxelMeshTerrain.Uvs3.Items, 0, voxelMeshTerrain.Uvs3.Count);
			terrainMesh.Uvs4.AddRange(voxelMeshTerrain.Uvs4.Items, 0, voxelMeshTerrain.Uvs4.Count);
			terrainMesh.ColorVertices.AddRange(voxelMeshTerrain.ColorVertices.Items, 0, voxelMeshTerrain.ColorVertices.Count);
			terrainMesh.Normals.AddRange(voxelMeshTerrain.Normals.Items, 0, voxelMeshTerrain.Normals.Count);
			terrainMesh.Tangents.AddRange(voxelMeshTerrain.Tangents.Items, 0, voxelMeshTerrain.Tangents.Count);
			if (voxelMeshTerrain.Indices.Count > 0)
			{
				terrainMesh.Indices.Grow(terrainMesh.Indices.Count + voxelMeshTerrain.Indices.Count);
				for (int j = 0; j < voxelMeshTerrain.Indices.Count; j++)
				{
					terrainMesh.Indices.Add(voxelMeshTerrain.Indices[j] + count);
				}
				return;
			}
			terrainMesh.Indices.Grow(terrainMesh.Indices.Count + voxelMeshTerrain.submeshes.Sum((TerrainSubMesh d) => d.triangles.Count));
			using (List<TerrainSubMesh>.Enumerator enumerator = voxelMeshTerrain.submeshes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TerrainSubMesh terrainSubMesh = enumerator.Current;
					for (int k = 0; k < terrainSubMesh.triangles.Count; k++)
					{
						terrainMesh.Indices.Add(terrainSubMesh.triangles[k] + count);
					}
				}
				return;
			}
		}
		int count2 = opaqueMesh.Vertices.Count;
		opaqueMesh.Vertices.Grow(count2 + voxelMeshToCopyFrom.Vertices.Count);
		for (int l = 0; l < voxelMeshToCopyFrom.Vertices.Count; l++)
		{
			opaqueMesh.Vertices.Add(voxelMeshToCopyFrom.Vertices[l] + b);
		}
		opaqueMesh.Uvs.AddRange(voxelMeshToCopyFrom.Uvs.Items, 0, voxelMeshToCopyFrom.Uvs.Count);
		opaqueMesh.UvsCrack.AddRange(voxelMeshToCopyFrom.UvsCrack.Items, 0, voxelMeshToCopyFrom.UvsCrack.Count);
		opaqueMesh.ColorVertices.AddRange(voxelMeshToCopyFrom.ColorVertices.Items, 0, voxelMeshToCopyFrom.ColorVertices.Count);
		opaqueMesh.Normals.AddRange(voxelMeshToCopyFrom.Normals.Items, 0, voxelMeshToCopyFrom.Normals.Count);
		opaqueMesh.Tangents.AddRange(voxelMeshToCopyFrom.Tangents.Items, 0, voxelMeshToCopyFrom.Tangents.Count);
		opaqueMesh.Indices.Grow(opaqueMesh.Indices.Count + voxelMeshToCopyFrom.Indices.Count);
		for (int m = 0; m < voxelMeshToCopyFrom.Indices.Count; m++)
		{
			opaqueMesh.Indices.Add(voxelMeshToCopyFrom.Indices[m] + count2);
		}
	}

	// Token: 0x060019DA RID: 6618 RVA: 0x0009F6E4 File Offset: 0x0009D8E4
	public void ResetAfterJob()
	{
		this.Item = null;
		this.Region = null;
		this.Status = DynamicMeshBuilderStatus.Ready;
	}

	// Token: 0x04001095 RID: 4245
	public static DyMeshData RegionMeshData = DyMeshData.Create(1048000, 63000);

	// Token: 0x04001096 RID: 4246
	public static bool DebugOnMainThread = false;

	// Token: 0x04001097 RID: 4247
	[PublicizedFrom(EAccessModifier.Protected)]
	public string ThreadName;

	// Token: 0x04001098 RID: 4248
	public string Error;

	// Token: 0x04001099 RID: 4249
	public bool StopRequested;

	// Token: 0x0400109A RID: 4250
	public DynamicMeshBuilderStatus Status;

	// Token: 0x0400109B RID: 4251
	public DyMeshData ChunkData;

	// Token: 0x0400109C RID: 4252
	public ExportMeshResult Result = ExportMeshResult.Missing;

	// Token: 0x0400109D RID: 4253
	[PublicizedFrom(EAccessModifier.Protected)]
	public Chunk chunk = new Chunk();

	// Token: 0x0400109E RID: 4254
	public DynamicMeshItem Item;

	// Token: 0x0400109F RID: 4255
	public DynamicMeshThread.ThreadRegion Region;

	// Token: 0x040010A0 RID: 4256
	public bool IsPrimaryQueue;

	// Token: 0x040010A1 RID: 4257
	[PublicizedFrom(EAccessModifier.Private)]
	public VoxelMeshLayer MeshLayer = MemoryPools.poolVML.AllocSync(true);

	// Token: 0x040010A2 RID: 4258
	public int yOffset;

	// Token: 0x040010A3 RID: 4259
	public int EndY;

	// Token: 0x040010A4 RID: 4260
	[PublicizedFrom(EAccessModifier.Protected)]
	public ChunkCacheNeighborChunks cacheNeighbourChunks;

	// Token: 0x040010A5 RID: 4261
	[PublicizedFrom(EAccessModifier.Protected)]
	public ChunkCacheNeighborBlocks cacheBlocks;

	// Token: 0x040010A6 RID: 4262
	[PublicizedFrom(EAccessModifier.Protected)]
	public Chunk[] neighbours = new Chunk[9];

	// Token: 0x040010A7 RID: 4263
	[PublicizedFrom(EAccessModifier.Protected)]
	public MeshGeneratorMC2 meshGen;

	// Token: 0x040010A8 RID: 4264
	public double MeshDataTime;

	// Token: 0x040010A9 RID: 4265
	public double ExportTime;

	// Token: 0x040010AA RID: 4266
	public int MinTerrainHeight;

	// Token: 0x040010AB RID: 4267
	[PublicizedFrom(EAccessModifier.Protected)]
	public DateTime LastActive;

	// Token: 0x040010AC RID: 4268
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkPreviewData PreviewData;

	// Token: 0x040010AD RID: 4269
	[PublicizedFrom(EAccessModifier.Private)]
	public Chunk[] FakeChunks = new Chunk[]
	{
		new Chunk(),
		new Chunk(),
		new Chunk(),
		new Chunk(),
		new Chunk(),
		new Chunk(),
		new Chunk(),
		new Chunk()
	};

	// Token: 0x040010AE RID: 4270
	[PublicizedFrom(EAccessModifier.Private)]
	public Thread thread;

	// Token: 0x040010AF RID: 4271
	[PublicizedFrom(EAccessModifier.Private)]
	public List<long> ChunkKeys = new List<long>(20);
}
