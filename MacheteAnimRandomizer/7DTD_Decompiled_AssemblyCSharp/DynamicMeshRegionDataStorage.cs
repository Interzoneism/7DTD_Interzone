using System;
using System.Collections.Concurrent;
using System.IO;
using Noemax.GZip;

// Token: 0x02000350 RID: 848
public class DynamicMeshRegionDataStorage
{
	// Token: 0x060018E1 RID: 6369 RVA: 0x0009853D File Offset: 0x0009673D
	public void ClearQueues()
	{
		Log.Out("Clearing queues.");
		this.CleanUpAndSave();
		this.ChunkData.Clear();
		Log.Out("Cleared queues.");
	}

	// Token: 0x060018E2 RID: 6370 RVA: 0x00098564 File Offset: 0x00096764
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryLoadItem(DynamicMeshRegionDataWrapper wrapper, bool releaseLock)
	{
		object @lock = this._lock;
		lock (@lock)
		{
			string text = wrapper.Path();
			if (!SdFile.Exists(text))
			{
				wrapper.StateInfo |= DynamicMeshStates.FileMissing;
				return false;
			}
			int num = wrapper.StateInfo.HasFlag(DynamicMeshStates.SaveRequired) ? 1 : 0;
			bool flag2 = false;
			if (num == 0 && flag2)
			{
				using (Stream stream = SdFile.OpenRead(text))
				{
					int num2 = 0;
					this.FileMemoryStream.Position = 0L;
					this.FileMemoryStream.SetLength(0L);
					using (DeflateInputStream deflateInputStream = new DeflateInputStream(stream))
					{
						int num3;
						do
						{
							num3 = deflateInputStream.Read(this.Buffer, 0, this.Buffer.Length);
							num2 += num3;
							if (num3 > 0)
							{
								this.FileMemoryStream.Write(this.Buffer, 0, num3);
							}
						}
						while (num3 == this.Buffer.Length);
						DynamicMeshChunkData.LoadFromStream(this.FileMemoryStream);
						if (DynamicMeshManager.DoLog)
						{
							Log.Out(string.Concat(new string[]
							{
								"LOAD FILE SIZE: ",
								text,
								" @ ",
								this.FileMemoryStream.Length.ToString(),
								" OR ",
								num2.ToString()
							}));
						}
					}
				}
			}
			if (releaseLock)
			{
				wrapper.TryExit("tryLoadItem");
			}
			wrapper.StateInfo &= ~DynamicMeshStates.LoadRequired;
			wrapper.StateInfo &= ~DynamicMeshStates.FileMissing;
			wrapper.StateInfo &= ~DynamicMeshStates.LoadBoosted;
		}
		return true;
	}

	// Token: 0x060018E3 RID: 6371 RVA: 0x0009875C File Offset: 0x0009695C
	public void AddSaveRequest(long key, DynamicMeshChunkData data, int length, bool requestRegionUpdate, bool unloadImmediately, bool loadInWorld)
	{
		if (!DynamicMeshManager.CONTENT_ENABLED)
		{
			return;
		}
		DynamicMeshRegionDataWrapper wrapper = this.GetWrapper(key);
		if (DynamicMeshManager.DoLog)
		{
			Log.Out("Adding Saving " + wrapper.ToDebugLocation() + ":" + length.ToString());
		}
		string debug = "addSave";
		if (!wrapper.GetLock(debug))
		{
			Log.Warning("Could not get lock on save request: " + wrapper.ToDebugLocation());
			return;
		}
		this.SaveItem(wrapper);
		if (requestRegionUpdate)
		{
			DynamicMeshThread.AddRegionUpdateData(wrapper.X, wrapper.Z, false);
		}
		if (loadInWorld)
		{
			DynamicMeshThread.ChunkReadyForCollection.Add(new Vector2i(wrapper.X, wrapper.Z));
		}
		this.ClearLock(wrapper, "_SAVERELEASE_");
	}

	// Token: 0x060018E4 RID: 6372 RVA: 0x00098814 File Offset: 0x00096A14
	[PublicizedFrom(EAccessModifier.Private)]
	public void SaveItem(DynamicMeshRegionDataWrapper wrapper)
	{
		DynamicMeshChunkData dynamicMeshChunkData = null;
		wrapper.GetLock("saveItem");
		wrapper.ClearUnloadMarks();
		wrapper.StateInfo &= ~DynamicMeshStates.SaveRequired;
		string path = wrapper.Path();
		if (dynamicMeshChunkData == null)
		{
			if (DynamicMeshManager.DoLog)
			{
				Log.Out("Deleting null bytes " + wrapper.ToDebugLocation());
			}
			if (SdFile.Exists(path))
			{
				SdFile.Delete(path);
				return;
			}
		}
		else
		{
			if (DynamicMeshManager.DoLog)
			{
				Log.Out("Saving to disk " + wrapper.ToDebugLocation());
			}
			int streamSize = dynamicMeshChunkData.GetStreamSize();
			int count = 0;
			byte[] fromPool = DynamicMeshThread.ChunkDataQueue.GetFromPool(streamSize);
			using (MemoryStream memoryStream = new MemoryStream(fromPool))
			{
				using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
				{
					pooledBinaryWriter.SetBaseStream(memoryStream);
					dynamicMeshChunkData.UpdateTime = (dynamicMeshChunkData.UpdateTime = (int)(DateTime.UtcNow - DynamicMeshFile.ItemMin).TotalSeconds);
					dynamicMeshChunkData.Write(pooledBinaryWriter);
					count = (int)pooledBinaryWriter.BaseStream.Position;
				}
			}
			DynamicMeshUnity.EnsureDMDirectoryExists();
			using (Stream stream = SdFile.Create(path))
			{
				using (DeflateOutputStream deflateOutputStream = new DeflateOutputStream(stream, 3, false))
				{
					deflateOutputStream.Write(fromPool, 0, count);
				}
			}
			wrapper.StateInfo &= ~DynamicMeshStates.FileMissing;
			DynamicMeshThread.ChunkDataQueue.ManuallyReleaseBytes(fromPool);
		}
	}

	// Token: 0x060018E5 RID: 6373 RVA: 0x00002914 File Offset: 0x00000B14
	public void CleanUpAndSave()
	{
	}

	// Token: 0x060018E6 RID: 6374 RVA: 0x000989B8 File Offset: 0x00096BB8
	public bool ClearLock(DynamicMeshRegionDataWrapper wrapper, string debug)
	{
		return wrapper.TryExit("ClearLock " + debug);
	}

	// Token: 0x060018E7 RID: 6375 RVA: 0x000989D4 File Offset: 0x00096BD4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool ClearLock(long worldPosition, string debug)
	{
		DynamicMeshRegionDataWrapper wrapper = this.GetWrapper(worldPosition);
		return this.ClearLock(wrapper, debug);
	}

	// Token: 0x060018E8 RID: 6376 RVA: 0x000989F4 File Offset: 0x00096BF4
	public DynamicMeshRegionDataWrapper GetWrapper(long key)
	{
		DynamicMeshRegionDataWrapper dynamicMeshRegionDataWrapper;
		if (!this.ChunkData.TryGetValue(key, out dynamicMeshRegionDataWrapper))
		{
			dynamicMeshRegionDataWrapper = DynamicMeshRegionDataWrapper.Create(key);
			if (!this.ChunkData.TryAdd(key, dynamicMeshRegionDataWrapper))
			{
				this.ChunkData.TryGetValue(key, out dynamicMeshRegionDataWrapper);
				Log.Error("Request failed to add data: " + DynamicMeshUnity.GetDebugPositionKey(key));
			}
		}
		return dynamicMeshRegionDataWrapper;
	}

	// Token: 0x060018E9 RID: 6377 RVA: 0x00098A4C File Offset: 0x00096C4C
	public void LoadRegion(DyMeshRegionLoadRequest load)
	{
		DynamicMeshRegionDataWrapper wrapper = this.GetWrapper(load.Key);
		wrapper.GetLock("loadRegion");
		string text = wrapper.Path();
		if (SdFile.Exists(text))
		{
			try
			{
				using (Stream stream = SdFile.OpenRead(text))
				{
					int num = 0;
					this.FileMemoryStream.Position = 0L;
					this.FileMemoryStream.SetLength(0L);
					using (DeflateInputStream deflateInputStream = new DeflateInputStream(stream))
					{
						int num2;
						do
						{
							num2 = deflateInputStream.Read(this.Buffer, 0, this.Buffer.Length);
							num += num2;
							if (num2 > 0)
							{
								this.FileMemoryStream.Write(this.Buffer, 0, num2);
							}
						}
						while (num2 == this.Buffer.Length);
						this.FileMemoryStream.Position = 0L;
						using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
						{
							pooledBinaryReader.SetBaseStream(this.FileMemoryStream);
							double totalSeconds = (DateTime.UtcNow - DynamicMeshFile.ItemMin).TotalSeconds;
							DynamicMeshVoxelRegionLoad.LoadRegionFromFile(pooledBinaryReader, load);
						}
						if (DynamicMeshManager.DoLog)
						{
							Log.Out(string.Concat(new string[]
							{
								"LOAD FILE SIZE: ",
								text,
								" @ ",
								this.FileMemoryStream.Length.ToString(),
								" OR ",
								num.ToString()
							}));
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Out("Read file error on dymesh: " + ex.Message + ". Deleting corrupted file.");
				try
				{
					SdFile.Delete(text);
				}
				catch (Exception)
				{
					Log.Out("Unable to delete dymesh file. You should manually delete: " + text);
				}
			}
		}
		this.ClearLock(wrapper, "loadRegionRelease");
	}

	// Token: 0x060018EA RID: 6378 RVA: 0x00098C80 File Offset: 0x00096E80
	public void SaveRegion(DynamicMeshThread.ThreadRegion region, Vector3i worldPosition, VoxelMesh opaque, VoxelMeshTerrain terrain)
	{
		long key = region.Key;
		DynamicMeshRegionDataWrapper wrapper = this.GetWrapper(key);
		wrapper.GetLock("saveRegion");
		DynamicMeshUnity.EnsureDMDirectoryExists();
		using (Stream stream = SdFile.Open(wrapper.Path(), FileMode.Create, FileAccess.Write, FileShare.Read))
		{
			using (DeflateOutputStream deflateOutputStream = new DeflateOutputStream(stream, 3, false))
			{
				using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
				{
					pooledBinaryWriter.SetBaseStream(deflateOutputStream);
					int updateTime = (int)(DateTime.UtcNow - DynamicMeshFile.ItemMin).TotalSeconds;
					DynamicMeshVoxelRegionLoad.SaveRegionToFile(pooledBinaryWriter, opaque, terrain, worldPosition, updateTime, null, null);
				}
			}
		}
		this.ClearLock(wrapper, "saveRegionRelease");
	}

	// Token: 0x04000FEC RID: 4076
	public ConcurrentDictionary<long, DynamicMeshRegionDataWrapper> ChunkData = new ConcurrentDictionary<long, DynamicMeshRegionDataWrapper>();

	// Token: 0x04000FED RID: 4077
	[PublicizedFrom(EAccessModifier.Private)]
	public object _lock = new object();

	// Token: 0x04000FEE RID: 4078
	[PublicizedFrom(EAccessModifier.Private)]
	public MemoryStream FileMemoryStream = new MemoryStream();

	// Token: 0x04000FEF RID: 4079
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] Buffer = new byte[2048];

	// Token: 0x04000FF0 RID: 4080
	public int MaxAllowedItems;
}
