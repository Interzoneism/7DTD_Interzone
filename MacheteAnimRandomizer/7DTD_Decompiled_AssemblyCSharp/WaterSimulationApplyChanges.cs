using System;
using System.Collections.Generic;
using Unity.Mathematics;

// Token: 0x02000B78 RID: 2936
public class WaterSimulationApplyChanges
{
	// Token: 0x06005B13 RID: 23315 RVA: 0x00247DF0 File Offset: 0x00245FF0
	public WaterSimulationApplyChanges(ChunkCluster _cc)
	{
		this.chunks = _cc;
		this.isServer = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer;
		this.applyThread = ThreadManager.StartThread("WaterSimulationApplyChanges", null, new ThreadManager.ThreadFunctionLoopDelegate(this.ThreadLoop), null, null, null, true, false);
	}

	// Token: 0x06005B14 RID: 23316 RVA: 0x00247E90 File Offset: 0x00246090
	public WaterSimulationApplyChanges.ChangesForChunk.Writer GetChangeWriter(long _chunkKey)
	{
		Dictionary<long, WaterSimulationApplyChanges.ChangesForChunk> obj = this.changeCache;
		WaterSimulationApplyChanges.ChangesForChunk.Writer result;
		lock (obj)
		{
			WaterSimulationApplyChanges.ChangesForChunk changesForChunk;
			if (!this.changeCache.TryGetValue(_chunkKey, out changesForChunk))
			{
				changesForChunk = this.changesPool.AllocSync(true);
				this.changeCache.Add(_chunkKey, changesForChunk);
				this.changedChunkList.AddLast(_chunkKey);
			}
			result = new WaterSimulationApplyChanges.ChangesForChunk.Writer(changesForChunk);
		}
		return result;
	}

	// Token: 0x06005B15 RID: 23317 RVA: 0x00247F0C File Offset: 0x0024610C
	public void DiscardChangesForChunks(List<long> _chunkKeys)
	{
		Dictionary<long, WaterSimulationApplyChanges.ChangesForChunk> obj = this.changeCache;
		lock (obj)
		{
			foreach (long num in _chunkKeys)
			{
				WaterSimulationApplyChanges.ChangesForChunk t;
				if (this.changeCache.TryGetValue(num, out t))
				{
					this.changesPool.FreeSync(t);
					this.changeCache.Remove(num);
					this.changedChunkList.Remove(num);
					Log.Out(string.Format("[DiscardChangesForChunks] Discarding pending water changes for chunk: {0}", num));
				}
			}
		}
	}

	// Token: 0x06005B16 RID: 23318 RVA: 0x00247FC8 File Offset: 0x002461C8
	[PublicizedFrom(EAccessModifier.Private)]
	public int ThreadLoop(ThreadManager.ThreadInfo _threadInfo)
	{
		if (_threadInfo.TerminationRequested())
		{
			return -1;
		}
		if (this.isServer && this.networkMaxBytesPerSecond > 0L)
		{
			NetPackageMeasure obj = this.networkMeasure;
			lock (obj)
			{
				this.networkMeasure.RecalculateTotals();
			}
		}
		Chunk chunk;
		WaterSimulationApplyChanges.ChangesForChunk changesForChunk;
		if (!this.TryFindChangeToApply(out chunk, out changesForChunk))
		{
			return 15;
		}
		this.ApplyChanges(chunk, changesForChunk.changedVoxels);
		chunk.EnterWriteLock();
		chunk.InProgressWaterSim = false;
		chunk.ExitWriteLock();
		this.changesPool.FreeSync(changesForChunk);
		return 0;
	}

	// Token: 0x06005B17 RID: 23319 RVA: 0x00248068 File Offset: 0x00246268
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryFindChangeToApply(out Chunk _chunk, out WaterSimulationApplyChanges.ChangesForChunk _changes)
	{
		Dictionary<long, WaterSimulationApplyChanges.ChangesForChunk> obj = this.changeCache;
		bool result;
		lock (obj)
		{
			if (this.changedChunkList.Count == 0)
			{
				_chunk = null;
				_changes = null;
				result = false;
			}
			else
			{
				LinkedListNode<long> linkedListNode = this.changedChunkList.First;
				while (linkedListNode != null)
				{
					long value = linkedListNode.Value;
					if (!this.changeCache.TryGetValue(value, out _changes))
					{
						LinkedListNode<long> node = linkedListNode;
						linkedListNode = linkedListNode.Next;
						this.changedChunkList.Remove(node);
					}
					else if (_changes.IsRecordingChanges)
					{
						linkedListNode = linkedListNode.Next;
					}
					else
					{
						if (WaterUtils.TryOpenChunkForUpdate(this.chunks, value, out _chunk))
						{
							LinkedListNode<long> node2 = linkedListNode;
							linkedListNode = linkedListNode.Next;
							this.changedChunkList.Remove(node2);
							this.changeCache.Remove(value);
							return true;
						}
						linkedListNode = linkedListNode.Next;
					}
				}
				_chunk = null;
				_changes = null;
				result = false;
			}
		}
		return result;
	}

	// Token: 0x06005B18 RID: 23320 RVA: 0x00248164 File Offset: 0x00246364
	public void ApplyChanges(Chunk _chunk, Dictionary<int, WaterValue> changedVoxels)
	{
		NetPackageWaterSimChunkUpdate netPackageWaterSimChunkUpdate = null;
		if (this.isServer && SingletonMonoBehaviour<ConnectionManager>.Instance.ClientCount() > 0)
		{
			netPackageWaterSimChunkUpdate = this.SetupForSend(_chunk);
		}
		int num = 0;
		int num2 = 0;
		foreach (KeyValuePair<int, WaterValue> keyValuePair in changedVoxels)
		{
			int key = keyValuePair.Key;
			int3 voxelCoords = WaterDataHandle.GetVoxelCoords(key);
			WaterValue value = keyValuePair.Value;
			WaterValue waterValue;
			_chunk.SetWaterSimUpdate(voxelCoords.x, voxelCoords.y, voxelCoords.z, value, out waterValue);
			if (value.GetMass() != waterValue.GetMass())
			{
				if (WaterUtils.GetWaterLevel(waterValue) != WaterUtils.GetWaterLevel(value))
				{
					num2 |= 1 << voxelCoords.y / 16;
				}
				if (netPackageWaterSimChunkUpdate != null)
				{
					netPackageWaterSimChunkUpdate.AddChange((ushort)key, value);
				}
			}
		}
		if (netPackageWaterSimChunkUpdate != null)
		{
			netPackageWaterSimChunkUpdate.FinalizeSend();
			num += this.SendUpdateToClients(netPackageWaterSimChunkUpdate);
		}
		if (num2 != 0)
		{
			lock (_chunk)
			{
				int needsRegenerationAt = _chunk.NeedsRegenerationAt;
				_chunk.SetNeedsRegenerationRaw(needsRegenerationAt | num2);
			}
		}
		if (num > 0)
		{
			if (this.networkMaxBytesPerSecond > 0L)
			{
				NetPackageMeasure obj = this.networkMeasure;
				lock (obj)
				{
					this.networkMeasure.AddSample((long)num);
				}
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.FlushClientSendQueues();
		}
	}

	// Token: 0x06005B19 RID: 23321 RVA: 0x002482F8 File Offset: 0x002464F8
	public bool HasNetWorkLimitBeenReached()
	{
		if (this.isServer && this.networkMaxBytesPerSecond > 0L && SingletonMonoBehaviour<ConnectionManager>.Instance.ClientCount() > 0)
		{
			NetPackageMeasure obj = this.networkMeasure;
			lock (obj)
			{
				return this.networkMeasure.totalSent > this.networkMaxBytesPerSecond;
			}
			return false;
		}
		return false;
	}

	// Token: 0x06005B1A RID: 23322 RVA: 0x00248368 File Offset: 0x00246568
	public NetPackageWaterSimChunkUpdate SetupForSend(Chunk _chunk)
	{
		this.clientsNearChunkBuffer.Clear();
		long key = _chunk.Key;
		foreach (ClientInfo clientInfo in SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.List)
		{
			EntityPlayer entityPlayer;
			if (GameManager.Instance.World.Players.dict.TryGetValue(clientInfo.entityId, out entityPlayer) && entityPlayer.ChunkObserver.chunksAround.Contains(key))
			{
				this.clientsNearChunkBuffer.Add(clientInfo);
			}
		}
		NetPackageWaterSimChunkUpdate package = NetPackageManager.GetPackage<NetPackageWaterSimChunkUpdate>();
		package.SetupForSend(_chunk);
		return package;
	}

	// Token: 0x06005B1B RID: 23323 RVA: 0x00248418 File Offset: 0x00246618
	public int SendUpdateToClients(NetPackageWaterSimChunkUpdate _package)
	{
		int num = 0;
		_package.RegisterSendQueue();
		foreach (ClientInfo clientInfo in this.clientsNearChunkBuffer)
		{
			clientInfo.SendPackage(_package);
			num += _package.GetLength();
		}
		_package.SendQueueHandled();
		return num;
	}

	// Token: 0x06005B1C RID: 23324 RVA: 0x00248484 File Offset: 0x00246684
	public void Cleanup()
	{
		this.applyThread.WaitForEnd(30);
	}

	// Token: 0x040045A5 RID: 17829
	[PublicizedFrom(EAccessModifier.Private)]
	public MemoryPooledObject<WaterSimulationApplyChanges.ChangesForChunk> changesPool = new MemoryPooledObject<WaterSimulationApplyChanges.ChangesForChunk>(300);

	// Token: 0x040045A6 RID: 17830
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkCluster chunks;

	// Token: 0x040045A7 RID: 17831
	[PublicizedFrom(EAccessModifier.Private)]
	public ThreadManager.ThreadInfo applyThread;

	// Token: 0x040045A8 RID: 17832
	[PublicizedFrom(EAccessModifier.Private)]
	public const int noWorkPauseDurationMs = 15;

	// Token: 0x040045A9 RID: 17833
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<long, WaterSimulationApplyChanges.ChangesForChunk> changeCache = new Dictionary<long, WaterSimulationApplyChanges.ChangesForChunk>();

	// Token: 0x040045AA RID: 17834
	[PublicizedFrom(EAccessModifier.Private)]
	public LinkedList<long> changedChunkList = new LinkedList<long>();

	// Token: 0x040045AB RID: 17835
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageMeasure networkMeasure = new NetPackageMeasure(1.0);

	// Token: 0x040045AC RID: 17836
	public long networkMaxBytesPerSecond = 524288L;

	// Token: 0x040045AD RID: 17837
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ClientInfo> clientsNearChunkBuffer = new List<ClientInfo>();

	// Token: 0x040045AE RID: 17838
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isServer;

	// Token: 0x02000B79 RID: 2937
	public class ChangesForChunk : IMemoryPoolableObject
	{
		// Token: 0x1700094E RID: 2382
		// (get) Token: 0x06005B1D RID: 23325 RVA: 0x00248494 File Offset: 0x00246694
		public bool IsRecordingChanges
		{
			get
			{
				bool result;
				lock (this)
				{
					result = this.isRecordingChanges;
				}
				return result;
			}
		}

		// Token: 0x06005B1E RID: 23326 RVA: 0x002484D4 File Offset: 0x002466D4
		public void Cleanup()
		{
			this.Reset();
		}

		// Token: 0x06005B1F RID: 23327 RVA: 0x002484DC File Offset: 0x002466DC
		public void Reset()
		{
			this.isRecordingChanges = false;
			this.changedVoxels.Clear();
		}

		// Token: 0x040045AF RID: 17839
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isRecordingChanges;

		// Token: 0x040045B0 RID: 17840
		public Dictionary<int, WaterValue> changedVoxels = new Dictionary<int, WaterValue>();

		// Token: 0x02000B7A RID: 2938
		public struct Writer : IDisposable
		{
			// Token: 0x06005B21 RID: 23329 RVA: 0x00248504 File Offset: 0x00246704
			public Writer(WaterSimulationApplyChanges.ChangesForChunk _changes)
			{
				lock (_changes)
				{
					_changes.isRecordingChanges = true;
				}
				this.changes = _changes;
			}

			// Token: 0x06005B22 RID: 23330 RVA: 0x00248548 File Offset: 0x00246748
			public void RecordChange(int _voxelIndex, WaterValue _waterValue)
			{
				this.changes.changedVoxels[_voxelIndex] = _waterValue;
			}

			// Token: 0x06005B23 RID: 23331 RVA: 0x0024855C File Offset: 0x0024675C
			public void Dispose()
			{
				WaterSimulationApplyChanges.ChangesForChunk obj = this.changes;
				lock (obj)
				{
					this.changes.isRecordingChanges = false;
				}
			}

			// Token: 0x040045B1 RID: 17841
			[PublicizedFrom(EAccessModifier.Private)]
			public WaterSimulationApplyChanges.ChangesForChunk changes;
		}
	}
}
