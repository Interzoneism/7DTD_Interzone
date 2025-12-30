using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using ConcurrentCollections;
using Noemax.GZip;

// Token: 0x02000321 RID: 801
public class DynamicMeshDataQueue<T> where T : DynamicMeshContainer
{
	// Token: 0x06001701 RID: 5889 RVA: 0x0008843C File Offset: 0x0008663C
	public DynamicMeshDataQueue(bool isRegion, int purgeInterval)
	{
		this.MeshFolder = DynamicMeshFile.MeshLocation;
		this.IsRegionQueue = isRegion;
		this.PurgeInterval = purgeInterval;
		DynamicMeshDataQueue<T>.Pool.EnforceMaxSize = true;
	}

	// Token: 0x06001702 RID: 5890 RVA: 0x000884D7 File Offset: 0x000866D7
	public string GetQueueType()
	{
		if (!this.IsRegionQueue)
		{
			return "Item";
		}
		return "Region";
	}

	// Token: 0x17000291 RID: 657
	// (get) Token: 0x06001703 RID: 5891 RVA: 0x000884EC File Offset: 0x000866EC
	public long MBytesAllocated
	{
		get
		{
			return this.BytesAllocated / 1024L / 1024L;
		}
	}

	// Token: 0x17000292 RID: 658
	// (get) Token: 0x06001704 RID: 5892 RVA: 0x00088502 File Offset: 0x00086702
	public long MBytesReleased
	{
		get
		{
			return this.BytesReleased / 1024L / 1024L;
		}
	}

	// Token: 0x17000293 RID: 659
	// (get) Token: 0x06001705 RID: 5893 RVA: 0x00088518 File Offset: 0x00086718
	public long BytesLive
	{
		get
		{
			return this.BytesAllocated - this.BytesReleased;
		}
	}

	// Token: 0x17000294 RID: 660
	// (get) Token: 0x06001706 RID: 5894 RVA: 0x00088527 File Offset: 0x00086727
	public long MBytesLive
	{
		get
		{
			return this.BytesLive / 1024L / 1024L;
		}
	}

	// Token: 0x06001707 RID: 5895 RVA: 0x00088540 File Offset: 0x00086740
	public byte[] GetFromPool(int length)
	{
		byte[] array = DynamicMeshDataQueue<T>.Pool.Alloc(length);
		this.BytesAllocated += (long)array.Length;
		if (array.Length > this.LargestAllocation)
		{
			this.LargestAllocation = array.Length;
		}
		this.LiveItems++;
		return array;
	}

	// Token: 0x06001708 RID: 5896 RVA: 0x00088590 File Offset: 0x00086790
	public byte[] Clone(byte[] source)
	{
		byte[] fromPool = this.GetFromPool(source.Length);
		Array.Copy(source, fromPool, source.Length);
		return fromPool;
	}

	// Token: 0x06001709 RID: 5897 RVA: 0x000885B4 File Offset: 0x000867B4
	public void ClearQueues()
	{
		Log.Out("Clearing queues. IsRegion: " + this.IsRegionQueue.ToString());
		DynamicMeshData dynamicMeshData;
		while (this.LoadRequests.TryPop(out dynamicMeshData))
		{
		}
		this.CleanUpAndSave();
		this.Cache.Clear();
		Log.Out("Cleared queues. IsRegion: " + this.IsRegionQueue.ToString());
	}

	// Token: 0x17000295 RID: 661
	// (get) Token: 0x0600170A RID: 5898 RVA: 0x00088615 File Offset: 0x00086815
	public bool HasLoadRequests
	{
		get
		{
			return (this.LoadRequests.Count > 0 || this.ImportantLoad != null) && this.IsReadyThreaded();
		}
	}

	// Token: 0x0600170B RID: 5899 RVA: 0x00088638 File Offset: 0x00086838
	public bool LoadItem()
	{
		if (this.ImportantLoad != null)
		{
			if (!this.TryLoadItem(this.ImportantLoad))
			{
				this.ImportantLoad.StateInfo |= DynamicMeshStates.FileMissing;
				Log.Out("Important load FAILED: " + this.ImportantLoad.ToDebugLocation());
				this.ImportantLoad = null;
			}
			this.ImportantLoad = null;
		}
		DynamicMeshData dynamicMeshData;
		while (this.MainThreadLoadRequests.TryPop(out dynamicMeshData))
		{
			if (!this.TryLoadItem(dynamicMeshData))
			{
				Log.Out("Failed main thread load " + dynamicMeshData.ToDebugLocation());
			}
		}
		if (this.LiveItems >= this.MaxAllowedItems)
		{
			return false;
		}
		if (this.LoadRequests.Count == 0)
		{
			return false;
		}
		if (!this.LoadRequests.TryPop(out dynamicMeshData))
		{
			return false;
		}
		if (dynamicMeshData.StateInfo.HasFlag(DynamicMeshStates.ThreadUpdating))
		{
			return false;
		}
		if (!this.TryLoadItem(dynamicMeshData))
		{
			this.LoadRequests.Push(dynamicMeshData);
		}
		return true;
	}

	// Token: 0x0600170C RID: 5900 RVA: 0x00088729 File Offset: 0x00086929
	public bool IsReadyThreaded()
	{
		return this.MaxAllowedItems == 0 || this.LiveItems < this.MaxAllowedItems - 1;
	}

	// Token: 0x0600170D RID: 5901 RVA: 0x00088745 File Offset: 0x00086945
	[PublicizedFrom(EAccessModifier.Private)]
	public bool ForceLoadItem(DynamicMeshData data, out byte[] bytes)
	{
		if (!data.GetLock("forceLoad"))
		{
			bytes = null;
			return false;
		}
		this.TryLoadItem(data);
		data.TryGetBytes(out bytes, "forceTry");
		return true;
	}

	// Token: 0x0600170E RID: 5902 RVA: 0x00088770 File Offset: 0x00086970
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryLoadItem(DynamicMeshData data)
	{
		object @lock = this._lock;
		lock (@lock)
		{
			string text = data.Path(this.IsRegionQueue);
			if (!SdFile.Exists(text))
			{
				data.StateInfo |= DynamicMeshStates.FileMissing;
				return false;
			}
			int num = data.StateInfo.HasFlag(DynamicMeshStates.SaveRequired) ? 1 : 0;
			byte[] fromPool;
			bool flag2 = data.TryGetBytes(out fromPool, "tryLoadItem");
			if (num == 0 && flag2)
			{
				if (DynamicMeshManager.CompressFiles)
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
							fromPool = this.GetFromPool((int)this.FileMemoryStream.Length);
							this.FileMemoryStream.Position = 0L;
							this.FileMemoryStream.Read(fromPool, 0, (int)this.FileMemoryStream.Length);
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
							this.ReplaceBytes(data, fromPool, "loadItem");
							data.StreamLength = (int)this.FileMemoryStream.Length;
							goto IL_231;
						}
					}
				}
				using (Stream stream2 = SdFile.OpenRead(text))
				{
					using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
					{
						pooledBinaryReader.SetBaseStream(stream2);
						int num4 = (int)pooledBinaryReader.BaseStream.Length;
						fromPool = this.GetFromPool(num4);
						pooledBinaryReader.Read(fromPool, 0, num4);
						this.ReplaceBytes(data, fromPool, "loadUncompressed");
						data.StreamLength = num4;
					}
				}
			}
			IL_231:
			data.TryExit("tryLoadItem");
			data.StateInfo &= ~DynamicMeshStates.LoadRequired;
			data.StateInfo &= ~DynamicMeshStates.FileMissing;
			data.StateInfo &= ~DynamicMeshStates.LoadBoosted;
		}
		return true;
	}

	// Token: 0x0600170F RID: 5903 RVA: 0x00088A78 File Offset: 0x00086C78
	public string GetCacheSize()
	{
		int num;
		return (this.IsRegionQueue ? "Region Queue\n" : "Item Queue\n") + DynamicMeshDataQueue<T>.Pool.GetSize(out num);
	}

	// Token: 0x06001710 RID: 5904 RVA: 0x00088AAA File Offset: 0x00086CAA
	public bool IsUpdating(T item)
	{
		return this.GetData(item.WorldPosition).StateInfo.HasFlag(DynamicMeshStates.ThreadUpdating);
	}

	// Token: 0x06001711 RID: 5905 RVA: 0x00088AD2 File Offset: 0x00086CD2
	public void MarkAsUpdating(T item)
	{
		this.GetData(item.WorldPosition).StateInfo |= DynamicMeshStates.ThreadUpdating;
	}

	// Token: 0x06001712 RID: 5906 RVA: 0x00088AF2 File Offset: 0x00086CF2
	public void MarkAsUpdated(T item)
	{
		if (item == null)
		{
			return;
		}
		this.GetData(item.WorldPosition).StateInfo &= ~DynamicMeshStates.ThreadUpdating;
	}

	// Token: 0x06001713 RID: 5907 RVA: 0x00088B1C File Offset: 0x00086D1C
	public void ClearMainThreadTag(T item)
	{
		if (item == null)
		{
			return;
		}
		this.GetData(item.WorldPosition).StateInfo &= ~DynamicMeshStates.MainThreadLoadRequest;
	}

	// Token: 0x06001714 RID: 5908 RVA: 0x00088B49 File Offset: 0x00086D49
	public void ResetData(T item)
	{
		this.AddSaveRequest(item, null, 0, true, true, false);
	}

	// Token: 0x06001715 RID: 5909 RVA: 0x00088B57 File Offset: 0x00086D57
	public void AddSaveRequest(T item, byte[] bytes, int length, bool requestRegionUpdate, bool unloadImmediately, bool loadInWorld)
	{
		this.AddSaveRequest(item.WorldPosition, bytes, length, requestRegionUpdate, unloadImmediately, loadInWorld);
	}

	// Token: 0x06001716 RID: 5910 RVA: 0x00088B74 File Offset: 0x00086D74
	public void AddSaveRequest(Vector3i worldPosition, byte[] bytes, int length, bool requestRegionUpdate, bool unloadImmediately, bool loadInWorld)
	{
		if (!DynamicMeshManager.CONTENT_ENABLED)
		{
			return;
		}
		DynamicMeshData data = this.GetData(worldPosition);
		if (DynamicMeshManager.DoLog)
		{
			Log.Out(string.Concat(new string[]
			{
				"Adding Saving ",
				this.GetQueueType(),
				" ",
				data.ToDebugLocation(),
				":",
				length.ToString()
			}));
		}
		string debug = "addSave";
		if (!data.GetLock(debug))
		{
			Log.Warning("Could not get lock on save request: " + data.ToDebugLocation());
			return;
		}
		data.StreamLength = length;
		this.ReplaceBytes(data, bytes, "saveRequest");
		data.StateInfo |= DynamicMeshStates.SaveRequired;
		data.StateInfo &= ~DynamicMeshStates.MarkedForDelete;
		if (unloadImmediately)
		{
			data.StateInfo |= DynamicMeshStates.UnloadMark1;
			data.StateInfo |= DynamicMeshStates.UnloadMark2;
			data.StateInfo |= DynamicMeshStates.UnloadMark3;
		}
		this.SaveRequests.Add(data);
		data.TryExit(debug);
		if (this.IsRegionQueue)
		{
			DynamicMeshManager.Instance.AddUpdateData(data.X, data.Z, false, false);
		}
	}

	// Token: 0x06001717 RID: 5911 RVA: 0x00088C9C File Offset: 0x00086E9C
	public void SaveNetworkPackage(Vector3i worldPosition, byte[] bytes, int length)
	{
		try
		{
			string path = DynamicMeshFile.MeshLocation + string.Format("{0},{1}.mesh", worldPosition.x, worldPosition.z);
			if (bytes == null)
			{
				if (SdFile.Exists(path))
				{
					SdFile.Delete(path);
				}
			}
			else
			{
				using (Stream stream = SdFile.Create(path))
				{
					using (DeflateOutputStream deflateOutputStream = new DeflateOutputStream(stream, 3, false))
					{
						deflateOutputStream.Write(bytes, 0, length);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.Error("Data queue error: " + ex.Message);
		}
	}

	// Token: 0x06001718 RID: 5912 RVA: 0x00088D5C File Offset: 0x00086F5C
	public bool TrySave(bool forceSave = false)
	{
		if (this.SaveRequests.Count == 0)
		{
			return false;
		}
		try
		{
			DynamicMeshData dynamicMeshData;
			if (!this.SaveRequests.TryRemoveFirst(out dynamicMeshData))
			{
				return false;
			}
			if (!forceSave && dynamicMeshData.StateInfo.HasFlag(DynamicMeshStates.ThreadUpdating))
			{
				Log.Out("Skipping save on updating item " + dynamicMeshData.ToDebugLocation());
				this.SaveRequests.Add(dynamicMeshData);
				return false;
			}
			this.SaveItem(dynamicMeshData);
			dynamicMeshData.TryExit("saveItem");
			return true;
		}
		catch (Exception ex)
		{
			Log.Error("Data queue error: " + ex.Message);
		}
		return false;
	}

	// Token: 0x06001719 RID: 5913 RVA: 0x00088E10 File Offset: 0x00087010
	public void SaveItem(DynamicMeshData data)
	{
		data.GetLock("saveItem");
		byte[] array;
		data.TryGetBytes(out array, "saveItemTry");
		data.ClearUnloadMarks();
		data.StateInfo &= ~DynamicMeshStates.SaveRequired;
		if (DynamicMeshManager.DoLog)
		{
			Log.Out(string.Concat(new string[]
			{
				"Saving ",
				this.GetQueueType(),
				" ",
				data.ToDebugLocation(),
				":",
				(array != null) ? array.Length.ToString() : null
			}));
		}
		string path = data.Path(this.IsRegionQueue);
		if (array != null)
		{
			if (DynamicMeshManager.DoLog)
			{
				Log.Out(string.Concat(new string[]
				{
					"Saving ",
					this.GetQueueType(),
					" to disk ",
					data.ToDebugLocation(),
					": ",
					data.StreamLength.ToString()
				}));
			}
			if (DynamicMeshManager.CompressFiles)
			{
				using (Stream stream = SdFile.Create(path))
				{
					using (DeflateOutputStream deflateOutputStream = new DeflateOutputStream(stream, 3, false))
					{
						deflateOutputStream.Write(array, 0, data.StreamLength);
						return;
					}
				}
			}
			throw new NotImplementedException("Compression is not active");
		}
		if (DynamicMeshManager.DoLog)
		{
			Log.Out("Deleting null bytes " + this.GetQueueType() + " " + data.ToDebugLocation());
		}
		if (SdFile.Exists(path))
		{
			SdFile.Delete(path);
			return;
		}
	}

	// Token: 0x0600171A RID: 5914 RVA: 0x00088FA0 File Offset: 0x000871A0
	public void CleanUpAndSave()
	{
		while (this.SaveRequests.Count > 0)
		{
			this.TrySave(true);
		}
	}

	// Token: 0x0600171B RID: 5915 RVA: 0x00088FBC File Offset: 0x000871BC
	public void TryRelease()
	{
		if (this.NextCachePurge > DateTime.Now)
		{
			return;
		}
		this.NextCachePurge = DateTime.Now.AddSeconds((double)this.PurgeInterval);
		foreach (KeyValuePair<Vector3i, DynamicMeshData> keyValuePair in this.Cache)
		{
			DynamicMeshData value = keyValuePair.Value;
			if (value.X != 0 || value.Z != 0)
			{
				if (value.StateInfo.HasFlag(DynamicMeshStates.SaveRequired) && this.SaveRequests.Count > 0)
				{
					if (DynamicMeshManager.DebugReleases)
					{
						Log.Out(string.Format("{0},{1} save required", value.X, value.Z));
					}
				}
				else if (value.StateInfo.HasFlag(DynamicMeshStates.ThreadUpdating))
				{
					if (DynamicMeshManager.DebugReleases)
					{
						Log.Out(string.Format("{0},{1} thread updating", value.X, value.Z));
					}
				}
				else if (!value.StateInfo.HasFlag(DynamicMeshStates.UnloadMark1) && !value.StateInfo.HasFlag(DynamicMeshStates.MarkedForDelete))
				{
					value.StateInfo |= DynamicMeshStates.UnloadMark1;
				}
				else
				{
					this.ReleaseData(keyValuePair.Key);
				}
			}
		}
	}

	// Token: 0x0600171C RID: 5916 RVA: 0x00089150 File Offset: 0x00087350
	public void MarkForDeletion(Vector3i worldPosition)
	{
		this.GetData(worldPosition).StateInfo |= DynamicMeshStates.MarkedForDelete;
	}

	// Token: 0x0600171D RID: 5917 RVA: 0x00089167 File Offset: 0x00087367
	public void MarkForUnload(Vector3i worldPosition)
	{
		DynamicMeshData data = this.GetData(worldPosition);
		data.StateInfo |= DynamicMeshStates.UnloadMark1;
		data.StateInfo |= DynamicMeshStates.UnloadMark2;
		data.StateInfo |= DynamicMeshStates.UnloadMark3;
	}

	// Token: 0x0600171E RID: 5918 RVA: 0x0008919C File Offset: 0x0008739C
	public bool ReplaceBytes(DynamicMeshData data, byte[] bytes, string debug)
	{
		if (!data.GetLock("ReplaceBytes"))
		{
			Log.Out("ReplaceBytesLockFailed");
			return false;
		}
		while (!this.ReleaseBytes(data))
		{
			Log.Out("Waiting for bytes to be released: " + data.ToDebugLocation());
		}
		data.Bytes = bytes;
		return true;
	}

	// Token: 0x0600171F RID: 5919 RVA: 0x000891E8 File Offset: 0x000873E8
	public bool ClearLock(T item, string debug, bool releasePool)
	{
		if (item == null)
		{
			return true;
		}
		DynamicMeshData data = this.GetData(item.WorldPosition);
		if (releasePool && !data.StateInfo.HasFlag(DynamicMeshStates.SaveRequired))
		{
			this.ReleaseData(item.WorldPosition);
		}
		return data.TryExit("ClearLock " + debug);
	}

	// Token: 0x06001720 RID: 5920 RVA: 0x00089254 File Offset: 0x00087454
	public bool ClearLock(Vector3i worldPosition, string debug)
	{
		return this.GetData(worldPosition).TryExit(debug);
	}

	// Token: 0x06001721 RID: 5921 RVA: 0x00089268 File Offset: 0x00087468
	public void LogMemoryUsage()
	{
		int num;
		DynamicMeshDataQueue<T>.Pool.GetSize(out num);
		Log.Out(string.Format("{0}   Allocated: {1}   Released: {2}   {3}x Live: {4}   {5}x CacheMb: {6}   Longest: {7}", new object[]
		{
			this.GetQueueType(),
			this.MBytesAllocated,
			this.MBytesReleased,
			this.LiveItems,
			this.MBytesLive,
			DynamicMeshDataQueue<T>.Pool.GetTotalItems(),
			num / 1024 / 1024,
			this.LargestAllocation
		}));
	}

	// Token: 0x06001722 RID: 5922 RVA: 0x0008930D File Offset: 0x0008750D
	public void FreeMemory()
	{
		DynamicMeshDataQueue<T>.Pool.FreeAll();
	}

	// Token: 0x06001723 RID: 5923 RVA: 0x0008931C File Offset: 0x0008751C
	public bool ReleaseBytes(DynamicMeshData data)
	{
		if (!data.ThreadHasLock())
		{
			Log.Error("You can not release bytes if you do not have the lock: " + data.ToDebugLocation());
			return false;
		}
		byte[] array;
		if (data.TryGetBytes(out array, "releaseBytes"))
		{
			if (array != null)
			{
				DynamicMeshDataQueue<T>.Pool.Free(array);
				this.BytesReleased += (long)array.Length;
				this.LiveItems--;
				data.Bytes = null;
			}
			return true;
		}
		return false;
	}

	// Token: 0x06001724 RID: 5924 RVA: 0x0008938E File Offset: 0x0008758E
	public bool ManuallyReleaseBytes(byte[] bytes)
	{
		if (bytes != null)
		{
			DynamicMeshDataQueue<T>.Pool.Free(bytes);
			this.BytesReleased += (long)bytes.Length;
			return true;
		}
		return false;
	}

	// Token: 0x06001725 RID: 5925 RVA: 0x000893B4 File Offset: 0x000875B4
	public bool ReleaseData(Vector3i worldPosition)
	{
		worldPosition.y = 0;
		DynamicMeshData data = this.GetData(worldPosition);
		if (!data.TryTakeLock("releaseDataQueue"))
		{
			Log.Out("Could not clear from queue because item is locked by " + data.lastLock);
			return false;
		}
		if (DynamicMeshManager.DoLog)
		{
			Log.Out("Releasing " + this.GetQueueType() + " " + data.ToDebugLocation());
		}
		bool result = this.ReleaseBytes(data);
		if (!this.Cache.TryRemove(worldPosition, out data))
		{
			string str = "Could not remove item from cache: ";
			Vector3i vector3i = worldPosition;
			Log.Out(str + vector3i.ToString());
		}
		if (data.StateInfo.HasFlag(DynamicMeshStates.MarkedForDelete))
		{
			if (DynamicMeshManager.DoLog)
			{
				Log.Out("Deleting " + this.GetQueueType() + " file from disk " + data.ToDebugLocation());
			}
			string path = data.Path(this.IsRegionQueue);
			if (SdFile.Exists(path))
			{
				SdFile.Delete(path);
			}
		}
		return result;
	}

	// Token: 0x06001726 RID: 5926 RVA: 0x000894AC File Offset: 0x000876AC
	public bool CollectItem(T item, bool forceLoad, bool mainThreadLoad, bool waitForLock, out byte[] bytes, out int length)
	{
		string text;
		return this.CollectItem(item, forceLoad, mainThreadLoad, waitForLock, out bytes, out text, out length);
	}

	// Token: 0x06001727 RID: 5927 RVA: 0x000894CA File Offset: 0x000876CA
	public bool CollectItem(T item, bool forceLoad, bool mainThreadLoad, bool waitForLock, out byte[] bytes, out string debugMessage, out int length)
	{
		if (item == null)
		{
			bytes = null;
			debugMessage = "null";
			length = 0;
			return false;
		}
		return this.CollectItem(item.WorldPosition, forceLoad, mainThreadLoad, waitForLock, out bytes, out debugMessage, out length);
	}

	// Token: 0x06001728 RID: 5928 RVA: 0x00089504 File Offset: 0x00087704
	public bool CollectItem(Vector3i worldPosition, bool forceLoad, bool mainThreadLoad, bool waitForLock, out byte[] bytes, out string debugMessage, out int length)
	{
		debugMessage = string.Empty;
		DynamicMeshData data = this.GetData(worldPosition);
		data.ClearUnloadMarks();
		if (data.IsAvailableToLoad())
		{
			while (!data.TryGetBytes(out bytes, "collectItem"))
			{
				if (!waitForLock)
				{
					debugMessage = "locked";
					length = 0;
					return false;
				}
				if (DynamicMeshManager.DoLog)
				{
					Log.Out(string.Concat(new string[]
					{
						"Waiting for lock on CollectItem ",
						data.ToDebugLocation(),
						" (",
						debugMessage,
						")"
					}));
				}
			}
			length = data.StreamLength;
			if (bytes != null || !data.Exists(this.IsRegionQueue))
			{
				return true;
			}
		}
		if (data.StateInfo.HasFlag(DynamicMeshStates.MarkedForDelete))
		{
			debugMessage = "toDelete";
			bytes = null;
			length = 0;
			return true;
		}
		if (data.StateInfo.HasFlag(DynamicMeshStates.ThreadUpdating))
		{
			debugMessage = "threadUpdating";
			bytes = null;
			length = 0;
			return false;
		}
		if (forceLoad)
		{
			this.ForceLoadItem(data, out bytes);
			length = data.StreamLength;
			return true;
		}
		if (data.StateInfo.HasFlag(DynamicMeshStates.FileMissing))
		{
			debugMessage = DynamicMeshManager.FileMissing;
			bytes = null;
			length = 0;
			return false;
		}
		if (mainThreadLoad)
		{
			this.ImportantLoad = data;
		}
		else
		{
			this.LoadRequests.Push(data);
		}
		if (!data.StateInfo.HasFlag(DynamicMeshStates.LoadRequired))
		{
			data.StateInfo |= DynamicMeshStates.LoadRequired;
		}
		else if (data.StateInfo.HasFlag(DynamicMeshStates.LoadRequired) && this.LoadRequests.Count > 0 && !data.StateInfo.HasFlag(DynamicMeshStates.LoadBoosted))
		{
			data.StateInfo |= DynamicMeshStates.LoadBoosted;
			this.LoadRequests.Push(data);
		}
		debugMessage = "dunno";
		length = 0;
		bytes = null;
		return false;
	}

	// Token: 0x06001729 RID: 5929 RVA: 0x000896FC File Offset: 0x000878FC
	public DynamicMeshData TryGetData(Vector3i worldPosition)
	{
		worldPosition.y = 0;
		DynamicMeshData result = null;
		this.Cache.TryGetValue(worldPosition, out result);
		return result;
	}

	// Token: 0x0600172A RID: 5930 RVA: 0x00089724 File Offset: 0x00087924
	public DynamicMeshData GetData(Vector3i worldPosition)
	{
		worldPosition.y = 0;
		DynamicMeshData dynamicMeshData = null;
		if (!this.Cache.TryGetValue(worldPosition, out dynamicMeshData))
		{
			dynamicMeshData = DynamicMeshData.Create(worldPosition.x, worldPosition.z, this.IsRegionQueue);
			if (!this.Cache.TryAdd(worldPosition, dynamicMeshData))
			{
				this.Cache.TryGetValue(worldPosition, out dynamicMeshData);
				Log.Error("Request failed to add data: " + worldPosition.ToDebugLocation());
			}
		}
		return dynamicMeshData;
	}

	// Token: 0x0600172B RID: 5931 RVA: 0x00089798 File Offset: 0x00087998
	public void AddToLoadListMainThread(Vector3i worldPosition)
	{
		DynamicMeshData data = this.GetData(worldPosition);
		if (!data.Exists(false))
		{
			return;
		}
		data.ClearUnloadMarks();
		data.StateInfo |= DynamicMeshStates.MainThreadLoadRequest;
		if (!data.StateInfo.HasFlag(DynamicMeshStates.LoadRequired))
		{
			data.StateInfo |= DynamicMeshStates.LoadRequired;
			this.MainThreadLoadRequests.Push(data);
		}
	}

	// Token: 0x0600172C RID: 5932 RVA: 0x00089801 File Offset: 0x00087A01
	public bool IsReadyToCollect(Vector3i worldPosition)
	{
		return this.GetData(worldPosition).Bytes != null;
	}

	// Token: 0x04000E68 RID: 3688
	public ConcurrentDictionary<Vector3i, DynamicMeshData> Cache = new ConcurrentDictionary<Vector3i, DynamicMeshData>();

	// Token: 0x04000E69 RID: 3689
	public ConcurrentStack<DynamicMeshData> MainThreadLoadRequests = new ConcurrentStack<DynamicMeshData>();

	// Token: 0x04000E6A RID: 3690
	public ConcurrentStack<DynamicMeshData> LoadRequests = new ConcurrentStack<DynamicMeshData>();

	// Token: 0x04000E6B RID: 3691
	public ConcurrentHashSet<DynamicMeshData> SaveRequests = new ConcurrentHashSet<DynamicMeshData>();

	// Token: 0x04000E6C RID: 3692
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly LoosePool<byte> Pool = new LoosePool<byte>();

	// Token: 0x04000E6D RID: 3693
	public DynamicMeshData ImportantLoad;

	// Token: 0x04000E6E RID: 3694
	[PublicizedFrom(EAccessModifier.Private)]
	public string MeshFolder;

	// Token: 0x04000E6F RID: 3695
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsRegionQueue;

	// Token: 0x04000E70 RID: 3696
	[PublicizedFrom(EAccessModifier.Private)]
	public DateTime NextCachePurge = DateTime.MinValue;

	// Token: 0x04000E71 RID: 3697
	[PublicizedFrom(EAccessModifier.Private)]
	public int PurgeInterval = 3;

	// Token: 0x04000E72 RID: 3698
	public long BytesAllocated;

	// Token: 0x04000E73 RID: 3699
	public long BytesReleased;

	// Token: 0x04000E74 RID: 3700
	public int LiveItems;

	// Token: 0x04000E75 RID: 3701
	public int LargestAllocation;

	// Token: 0x04000E76 RID: 3702
	[PublicizedFrom(EAccessModifier.Private)]
	public object _lock = new object();

	// Token: 0x04000E77 RID: 3703
	[PublicizedFrom(EAccessModifier.Private)]
	public MemoryStream FileMemoryStream = new MemoryStream();

	// Token: 0x04000E78 RID: 3704
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] Buffer = new byte[2048];

	// Token: 0x04000E79 RID: 3705
	public int MaxAllowedItems;
}
