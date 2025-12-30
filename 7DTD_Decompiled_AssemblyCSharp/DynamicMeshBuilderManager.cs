using System;
using System.Collections.Generic;
using System.Threading;

// Token: 0x02000314 RID: 788
public class DynamicMeshBuilderManager
{
	// Token: 0x06001659 RID: 5721 RVA: 0x00082170 File Offset: 0x00080370
	public static DynamicMeshBuilderManager GetOrCreate()
	{
		return DynamicMeshBuilderManager.Instance ?? new DynamicMeshBuilderManager();
	}

	// Token: 0x0600165A RID: 5722 RVA: 0x00082180 File Offset: 0x00080380
	public DynamicMeshBuilderManager()
	{
		DynamicMeshBuilderManager.Instance = this;
	}

	// Token: 0x0600165B RID: 5723 RVA: 0x000821A4 File Offset: 0x000803A4
	public void StartThreads()
	{
		Log.Out("Starting builder threads: " + DynamicMeshBuilderManager.MaxBuilderThreads.ToString());
		foreach (DynamicMeshChunkProcessor dynamicMeshChunkProcessor in this.BuilderThreads)
		{
			dynamicMeshChunkProcessor.RequestStop(false);
		}
		for (int i = 0; i < DynamicMeshBuilderManager.MaxBuilderThreads; i++)
		{
			this.AddBuilder();
		}
	}

	// Token: 0x0600165C RID: 5724 RVA: 0x00082228 File Offset: 0x00080428
	[PublicizedFrom(EAccessModifier.Private)]
	public DynamicMeshChunkProcessor AddBuilder()
	{
		DynamicMeshChunkProcessor dynamicMeshChunkProcessor = new DynamicMeshChunkProcessor();
		dynamicMeshChunkProcessor.Init(DynamicMeshBuilderManager.ThreadId++);
		dynamicMeshChunkProcessor.Status = DynamicMeshBuilderStatus.Ready;
		this.BuilderThreads.Add(dynamicMeshChunkProcessor);
		dynamicMeshChunkProcessor.StartThread();
		return dynamicMeshChunkProcessor;
	}

	// Token: 0x0600165D RID: 5725 RVA: 0x00082268 File Offset: 0x00080468
	public void MainThreadRunJobs()
	{
		foreach (DynamicMeshChunkProcessor dynamicMeshChunkProcessor in this.BuilderThreads)
		{
			if (dynamicMeshChunkProcessor != null)
			{
				dynamicMeshChunkProcessor.RunJob();
			}
		}
	}

	// Token: 0x0600165E RID: 5726 RVA: 0x000822C0 File Offset: 0x000804C0
	public void SetNewLimit(int limit)
	{
		DynamicMeshBuilderManager.MaxBuilderThreads = limit;
		this.StartThreads();
	}

	// Token: 0x0600165F RID: 5727 RVA: 0x000822D0 File Offset: 0x000804D0
	public void StopThreads(bool forceStop)
	{
		foreach (DynamicMeshChunkProcessor dynamicMeshChunkProcessor in this.BuilderThreads)
		{
			dynamicMeshChunkProcessor.RequestStop(forceStop);
		}
	}

	// Token: 0x06001660 RID: 5728 RVA: 0x00082324 File Offset: 0x00080524
	public DynamicMeshChunkProcessor GetRegionBuilder(bool useAllThreads)
	{
		if (!Monitor.TryEnter(this._lock, 1))
		{
			Log.Warning("Build region list locked");
			return null;
		}
		DynamicMeshChunkProcessor dynamicMeshChunkProcessor = null;
		for (int i = 0; i < this.BuilderThreads.Count; i++)
		{
			DynamicMeshChunkProcessor dynamicMeshChunkProcessor2 = this.BuilderThreads[i];
			if (!dynamicMeshChunkProcessor2.StopRequested && dynamicMeshChunkProcessor2.Status == DynamicMeshBuilderStatus.Ready)
			{
				dynamicMeshChunkProcessor = dynamicMeshChunkProcessor2;
				break;
			}
			if (!useAllThreads)
			{
				break;
			}
		}
		if (dynamicMeshChunkProcessor == null && useAllThreads && this.BuilderThreads.Count < DynamicMeshBuilderManager.MaxBuilderThreads)
		{
			dynamicMeshChunkProcessor = this.AddBuilder();
		}
		Monitor.Exit(this._lock);
		return dynamicMeshChunkProcessor;
	}

	// Token: 0x06001661 RID: 5729 RVA: 0x000823B4 File Offset: 0x000805B4
	public DynamicMeshChunkProcessor GetNextBuilder()
	{
		if (!Monitor.TryEnter(this._lock, 1))
		{
			Log.Warning("Build list locked");
			return null;
		}
		DynamicMeshChunkProcessor dynamicMeshChunkProcessor = null;
		foreach (DynamicMeshChunkProcessor dynamicMeshChunkProcessor2 in this.BuilderThreads)
		{
			if (!dynamicMeshChunkProcessor2.StopRequested && dynamicMeshChunkProcessor2.Status == DynamicMeshBuilderStatus.Ready)
			{
				dynamicMeshChunkProcessor = dynamicMeshChunkProcessor2;
				break;
			}
		}
		if (dynamicMeshChunkProcessor == null && this.BuilderThreads.Count < DynamicMeshBuilderManager.MaxBuilderThreads)
		{
			dynamicMeshChunkProcessor = this.AddBuilder();
		}
		Monitor.Exit(this._lock);
		return dynamicMeshChunkProcessor;
	}

	// Token: 0x1700027D RID: 637
	// (get) Token: 0x06001662 RID: 5730 RVA: 0x00082458 File Offset: 0x00080658
	public bool HasThreadAvailable
	{
		get
		{
			return this.GetNextBuilder() != null;
		}
	}

	// Token: 0x06001663 RID: 5731 RVA: 0x00082464 File Offset: 0x00080664
	public int AddItemForExport(DynamicMeshItem item, bool isPrimary)
	{
		DynamicMeshChunkProcessor nextBuilder = this.GetNextBuilder();
		if (nextBuilder == null)
		{
			return 0;
		}
		if (DynamicMeshThread.ChunkDataQueue.IsUpdating(item))
		{
			return -1;
		}
		return nextBuilder.AddNewItem(item, isPrimary);
	}

	// Token: 0x06001664 RID: 5732 RVA: 0x00082494 File Offset: 0x00080694
	public int AddItemForMeshGeneration(DynamicMeshItem item, bool isPrimary)
	{
		DynamicMeshChunkProcessor nextBuilder = this.GetNextBuilder();
		if (nextBuilder == null)
		{
			return 0;
		}
		if (DynamicMeshThread.ChunkDataQueue.IsUpdating(item))
		{
			return -1;
		}
		DynamicMeshThread.GetThreadRegion(item.WorldPosition);
		return nextBuilder.AddItemForMeshGeneration(item, isPrimary);
	}

	// Token: 0x06001665 RID: 5733 RVA: 0x000824D0 File Offset: 0x000806D0
	public int AddItemForPreview(DynamicMeshItem item, ChunkPreviewData previewData)
	{
		DynamicMeshChunkProcessor nextBuilder = this.GetNextBuilder();
		if (nextBuilder == null)
		{
			return 0;
		}
		return nextBuilder.AddItemForMeshPreview(item, previewData);
	}

	// Token: 0x06001666 RID: 5734 RVA: 0x000824F4 File Offset: 0x000806F4
	public int RegenerateRegion(DynamicMeshThread.ThreadRegion region, bool useAllThreads)
	{
		DynamicMeshChunkProcessor regionBuilder = this.GetRegionBuilder(useAllThreads);
		if (regionBuilder == null)
		{
			return 0;
		}
		DynamicMeshThread.GetThreadRegion(region.Key);
		return regionBuilder.AddRegenerateRegion(region);
	}

	// Token: 0x06001667 RID: 5735 RVA: 0x00082524 File Offset: 0x00080724
	public void CheckBuilders()
	{
		for (int i = this.BuilderThreads.Count - 1; i >= 0; i--)
		{
			DynamicMeshChunkProcessor dynamicMeshChunkProcessor = this.BuilderThreads[i];
			if (dynamicMeshChunkProcessor == null)
			{
				this.BuilderThreads.RemoveAt(i);
				return;
			}
			if (dynamicMeshChunkProcessor.Status == DynamicMeshBuilderStatus.Complete)
			{
				this.HandleResult(dynamicMeshChunkProcessor);
			}
			else if (dynamicMeshChunkProcessor.Status == DynamicMeshBuilderStatus.Stopped)
			{
				dynamicMeshChunkProcessor.CleanUp();
				this.BuilderThreads.Remove(dynamicMeshChunkProcessor);
			}
			else if (dynamicMeshChunkProcessor.Status == DynamicMeshBuilderStatus.Error)
			{
				dynamicMeshChunkProcessor.CleanUp();
				this.BuilderThreads.Remove(dynamicMeshChunkProcessor);
			}
		}
	}

	// Token: 0x06001668 RID: 5736 RVA: 0x000825B4 File Offset: 0x000807B4
	public void CheckPreviews()
	{
		for (int i = this.BuilderThreads.Count - 1; i >= 0; i--)
		{
			DynamicMeshChunkProcessor dynamicMeshChunkProcessor = this.BuilderThreads[i];
			if (dynamicMeshChunkProcessor == null)
			{
				this.BuilderThreads.RemoveAt(i);
				return;
			}
			if (dynamicMeshChunkProcessor.Status == DynamicMeshBuilderStatus.PreviewComplete)
			{
				this.HandleResult(dynamicMeshChunkProcessor);
			}
		}
	}

	// Token: 0x06001669 RID: 5737 RVA: 0x00082608 File Offset: 0x00080808
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleResult(DynamicMeshChunkProcessor builder)
	{
		ExportMeshResult result = builder.Result;
		DynamicMeshItem item = builder.Item;
		DynamicMeshThread.ThreadRegion region = builder.Region;
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshItem item2 = builder.Item;
			string str = ((item2 != null) ? item2.ToDebugLocation() : null) ?? builder.Region.ToDebugLocation();
			Log.Out("Export result: " + str + ": " + result.ToString());
		}
		if (GameManager.IsDedicatedServer && builder.ChunkData != null)
		{
			builder.ChunkData = DyMeshData.AddToCache(builder.ChunkData);
		}
		if (builder.ChunkData != null)
		{
			string text;
			if ((text = ((item != null) ? item.ToDebugLocation() : null)) == null)
			{
				text = (((region != null) ? region.ToDebugLocation() : null) ?? "null");
			}
			string str2 = text;
			Log.Warning("Chunk data was not cleaned up by thread! " + str2 + ": " + result.ToString());
			builder.ChunkData = DyMeshData.AddToCache(builder.ChunkData);
		}
		if (item != null)
		{
			long key = item.Key;
			if (result == ExportMeshResult.Success)
			{
				DynamicMeshThread.ChunksToProcess.TryRemove(key);
				DynamicMeshThread.ChunksToLoad.Remove(key);
			}
			else if (result != ExportMeshResult.PreviewSuccess)
			{
				if (result == ExportMeshResult.PreviewDelay || result == ExportMeshResult.PreviewMissing)
				{
					DynamicMeshThread.SetNextChunks(item.Key);
					DynamicMeshPrefabPreviewThread.Instance.AddChunk(item);
				}
				else if (result == ExportMeshResult.SuccessNoLoad)
				{
					item.State = DynamicItemState.Empty;
					DynamicMeshThread.ChunksToProcess.TryRemove(key);
					DynamicMeshThread.ChunksToLoad.Remove(key);
				}
				else if (result == ExportMeshResult.Delay)
				{
					DynamicMeshThread.RequestPrimaryQueue(builder.Item);
				}
				else if (result == ExportMeshResult.ChunkMissing)
				{
					item.State = DynamicItemState.Empty;
					Log.Warning("chunk missing " + item.ToDebugLocation());
				}
				else
				{
					item.State = DynamicItemState.Empty;
					DynamicMeshThread.ChunksToProcess.TryRemove(key);
					DynamicMeshThread.ChunksToLoad.Remove(key);
					Log.Error("Failed to export " + item.ToDebugLocation() + ":" + result.ToString());
				}
			}
		}
		else if (result == ExportMeshResult.Delay)
		{
			Log.Out("Re-adding region regen???: " + region.ToDebugLocation());
			DynamicMeshThread.AddRegionUpdateData(region.X, region.Z, false);
		}
		builder.ResetAfterJob();
	}

	// Token: 0x04000E1D RID: 3613
	public static DynamicMeshBuilderManager Instance;

	// Token: 0x04000E1E RID: 3614
	public static int MaxBuilderThreads = 1;

	// Token: 0x04000E1F RID: 3615
	[PublicizedFrom(EAccessModifier.Private)]
	public static int ThreadId = 1;

	// Token: 0x04000E20 RID: 3616
	[PublicizedFrom(EAccessModifier.Private)]
	public const double MaxInactiveTime = 10.0;

	// Token: 0x04000E21 RID: 3617
	public List<DynamicMeshChunkProcessor> BuilderThreads = new List<DynamicMeshChunkProcessor>();

	// Token: 0x04000E22 RID: 3618
	[PublicizedFrom(EAccessModifier.Private)]
	public object _lock = new object();
}
