using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

// Token: 0x02000B64 RID: 2916
public class WaterSimulationNative
{
	// Token: 0x17000940 RID: 2368
	// (get) Token: 0x06005AB0 RID: 23216 RVA: 0x00245F6A File Offset: 0x0024416A
	// (set) Token: 0x06005AB1 RID: 23217 RVA: 0x00245F71 File Offset: 0x00244171
	public static WaterSimulationNative Instance { get; [PublicizedFrom(EAccessModifier.Private)] set; } = new WaterSimulationNative();

	// Token: 0x17000941 RID: 2369
	// (get) Token: 0x06005AB2 RID: 23218 RVA: 0x00245F79 File Offset: 0x00244179
	// (set) Token: 0x06005AB3 RID: 23219 RVA: 0x00245F81 File Offset: 0x00244181
	public bool IsInitialized { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000942 RID: 2370
	// (get) Token: 0x06005AB4 RID: 23220 RVA: 0x00245F8A File Offset: 0x0024418A
	public bool IsPaused
	{
		get
		{
			return this.isPaused;
		}
	}

	// Token: 0x06005AB5 RID: 23221 RVA: 0x00245F94 File Offset: 0x00244194
	public void Init(ChunkCluster _cc)
	{
		this.changeApplier = new WaterSimulationApplyChanges(_cc);
		if (!this.ShouldEnable)
		{
			return;
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		if (this.waterDataHandles.IsCreated || this.modifiedChunks.IsCreated)
		{
			Debug.LogError("Last water simulation data was disposed of and may have leaked");
		}
		this.activeHandles = new UnsafeParallelHashSet<ChunkKey>(500, AllocatorManager.Persistent);
		this.waterDataHandles = new UnsafeParallelHashMap<ChunkKey, WaterDataHandle>(500, AllocatorManager.Persistent);
		this.modifiedChunks = new UnsafeParallelHashSet<ChunkKey>(500, AllocatorManager.Persistent);
		this.groundWaterHeightMap = new GroundWaterHeightMap(GameManager.Instance.World);
		this.IsInitialized = true;
		this.isPaused = false;
	}

	// Token: 0x06005AB6 RID: 23222 RVA: 0x0024604C File Offset: 0x0024424C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsChunkInWorldBounds(Chunk _c)
	{
		Vector3i vector3i;
		Vector3i vector3i2;
		GameManager.Instance.World.GetWorldExtent(out vector3i, out vector3i2);
		return _c.X * 16 >= vector3i.x && _c.X * 16 < vector3i2.x && _c.Z * 16 >= vector3i.z && _c.Z * 16 < vector3i2.z;
	}

	// Token: 0x06005AB7 RID: 23223 RVA: 0x002460B4 File Offset: 0x002442B4
	public void InitializeChunk(Chunk _c)
	{
		if (!this.IsInitialized)
		{
			return;
		}
		if (!this.IsChunkInWorldBounds(_c))
		{
			return;
		}
		NativeSafeHandle<WaterDataHandle> safeHandle;
		WaterDataHandle waterDataHandle;
		if (this.freeHandles.TryDequeue(out safeHandle))
		{
			waterDataHandle = safeHandle.Target;
			waterDataHandle.Clear();
		}
		else
		{
			waterDataHandle = WaterDataHandle.AllocateNew(Allocator.Persistent);
			safeHandle = new NativeSafeHandle<WaterDataHandle>(ref waterDataHandle, Allocator.Persistent);
		}
		waterDataHandle.InitializeFromChunk(_c, this.groundWaterHeightMap);
		this.newInitializedHandles.Enqueue(new WaterSimulationNative.HandleInitRequest
		{
			chunkKey = new ChunkKey(_c),
			safeHandle = safeHandle
		});
		_c.AssignWaterSimHandle(new WaterSimulationNative.ChunkHandle(this, _c));
	}

	// Token: 0x06005AB8 RID: 23224 RVA: 0x00246148 File Offset: 0x00244348
	[PublicizedFrom(EAccessModifier.Private)]
	public void CopyInitializedChunksToNative()
	{
		WaterSimulationNative.HandleInitRequest handleInitRequest;
		while (this.newInitializedHandles.TryDequeue(out handleInitRequest))
		{
			ChunkKey chunkKey = handleInitRequest.chunkKey;
			NativeSafeHandle<WaterDataHandle> safeHandle = handleInitRequest.safeHandle;
			NativeSafeHandle<WaterDataHandle> item;
			if (this.usedHandles.TryGetValue(chunkKey, out item))
			{
				this.freeHandles.Enqueue(item);
				this.usedHandles[chunkKey] = safeHandle;
			}
			else
			{
				this.usedHandles.Add(chunkKey, safeHandle);
			}
			WaterDataHandle target = safeHandle.Target;
			this.waterDataHandles[chunkKey] = target;
			if (target.HasActiveWater)
			{
				this.activeHandles.Add(chunkKey);
			}
		}
	}

	// Token: 0x06005AB9 RID: 23225 RVA: 0x002461DC File Offset: 0x002443DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void ProcessPendingRemoves()
	{
		ChunkKey chunkKey;
		while (this.handlesToRemove.TryDequeue(out chunkKey))
		{
			NativeSafeHandle<WaterDataHandle> item;
			if (this.usedHandles.TryGetValue(chunkKey, out item))
			{
				this.usedHandles.Remove(chunkKey);
				this.freeHandles.Enqueue(item);
			}
			this.waterDataHandles.Remove(chunkKey);
			this.activeHandles.Remove(chunkKey);
		}
	}

	// Token: 0x06005ABA RID: 23226 RVA: 0x0024623D File Offset: 0x0024443D
	public void SetPaused(bool _isPaused)
	{
		this.isPaused = _isPaused;
	}

	// Token: 0x06005ABB RID: 23227 RVA: 0x00246246 File Offset: 0x00244446
	public void Step()
	{
		if (!this.isPaused)
		{
			this.SetPaused(true);
			return;
		}
		this.isPaused = false;
		this.Update();
		this.isPaused = true;
	}

	// Token: 0x06005ABC RID: 23228 RVA: 0x0024626C File Offset: 0x0024446C
	public unsafe void Update()
	{
		if (!this.IsInitialized)
		{
			return;
		}
		this.ProcessPendingRemoves();
		this.CopyInitializedChunksToNative();
		if (this.isPaused)
		{
			return;
		}
		if (this.changeApplier.HasNetWorkLimitBeenReached())
		{
			return;
		}
		WaterStats waterStats = default(WaterStats);
		if (!this.modifiedChunks.IsEmpty || !this.activeHandles.IsEmpty)
		{
			new WaterSimulationPreProcess
			{
				activeChunks = this.activeHandles,
				waterDataHandles = this.waterDataHandles,
				modifiedChunks = this.modifiedChunks
			}.Run<WaterSimulationPreProcess>();
			if (!this.activeHandles.IsEmpty)
			{
				NativeArray<ChunkKey> processingChunks = this.activeHandles.ToNativeArray(Allocator.TempJob);
				NativeList<ChunkKey> nonFlowingChunks = new NativeList<ChunkKey>(processingChunks.Length, AllocatorManager.TempJob);
				NativeArray<WaterStats> nativeArray = new NativeArray<WaterStats>(processingChunks.Length, Allocator.TempJob, NativeArrayOptions.ClearMemory);
				WaterSimulationCalcFlows jobData = new WaterSimulationCalcFlows
				{
					processingChunks = processingChunks,
					waterStats = nativeArray,
					waterDataHandles = this.waterDataHandles
				};
				WaterSimulationApplyFlows jobData2 = new WaterSimulationApplyFlows
				{
					processingChunks = processingChunks,
					nonFlowingChunks = nonFlowingChunks.AsParallelWriter(),
					waterStats = nativeArray,
					waterDataHandles = this.waterDataHandles,
					activeChunkSet = this.activeHandles.AsParallelWriter()
				};
				WaterSimulationPostProcess jobData3 = new WaterSimulationPostProcess
				{
					processingChunks = processingChunks,
					nonFlowingChunks = nonFlowingChunks,
					activeChunks = this.activeHandles,
					waterDataHandles = this.waterDataHandles
				};
				int innerloopBatchCount = processingChunks.Length / JobsUtility.JobWorkerCount + 1;
				JobHandle dependsOn = jobData.Schedule(processingChunks.Length, innerloopBatchCount, default(JobHandle));
				JobHandle dependsOn2 = jobData2.Schedule(processingChunks.Length, innerloopBatchCount, dependsOn);
				JobHandle jobHandle = jobData3.Schedule(dependsOn2);
				JobHandle.ScheduleBatchedJobs();
				jobHandle.Complete();
				waterStats += WaterStats.Sum(nativeArray);
				nativeArray.Dispose();
				processingChunks.Dispose();
				nonFlowingChunks.Dispose();
			}
		}
		this.ProcessPendingRemoves();
		foreach (KeyValue<ChunkKey, WaterDataHandle> keyValue in this.waterDataHandles)
		{
			ChunkKey key = keyValue.Key;
			UnsafeParallelHashMap<ChunkKey, WaterDataHandle>.Enumerator enumerator;
			keyValue = enumerator.Current;
			WaterDataHandle waterDataHandle = *keyValue.Value;
			if (waterDataHandle.HasFlows)
			{
				using (WaterSimulationApplyChanges.ChangesForChunk.Writer changeWriter = this.changeApplier.GetChangeWriter(WorldChunkCache.MakeChunkKey(key.x, key.z)))
				{
					UnsafeParallelHashMap<int, int>.Enumerator flowVoxels = waterDataHandle.FlowVoxels;
					while (flowVoxels.MoveNext())
					{
						KeyValue<int, int> keyValue2 = flowVoxels.Current;
						int key2 = keyValue2.Key;
						int mass = waterDataHandle.voxelData.Get(key2);
						WaterValue waterValue = new WaterValue(mass);
						changeWriter.RecordChange(key2, waterValue);
					}
					waterDataHandle.flowVoxels.Clear();
				}
			}
		}
		WaterStatsProfiler.SampleTick(waterStats);
	}

	// Token: 0x06005ABD RID: 23229 RVA: 0x0024655C File Offset: 0x0024475C
	public void Clear()
	{
		if (!this.IsInitialized)
		{
			return;
		}
		foreach (NativeSafeHandle<WaterDataHandle> item in this.usedHandles.Values)
		{
			this.freeHandles.Enqueue(item);
		}
		this.usedHandles.Clear();
		this.waterDataHandles.Clear();
		WaterSimulationNative.HandleInitRequest handleInitRequest;
		while (this.newInitializedHandles.TryDequeue(out handleInitRequest))
		{
			this.freeHandles.Enqueue(handleInitRequest.safeHandle);
		}
		this.handlesToRemove = new ConcurrentQueue<ChunkKey>();
		this.activeHandles.Clear();
		this.modifiedChunks.Clear();
	}

	// Token: 0x06005ABE RID: 23230 RVA: 0x0024661C File Offset: 0x0024481C
	public void Cleanup()
	{
		WaterSimulationApplyChanges waterSimulationApplyChanges = this.changeApplier;
		if (waterSimulationApplyChanges != null)
		{
			waterSimulationApplyChanges.Cleanup();
		}
		this.changeApplier = null;
		this.groundWaterHeightMap = null;
		if (!this.IsInitialized)
		{
			return;
		}
		this.Clear();
		NativeSafeHandle<WaterDataHandle> nativeSafeHandle;
		while (this.freeHandles.TryDequeue(out nativeSafeHandle))
		{
			nativeSafeHandle.Dispose();
		}
		foreach (NativeSafeHandle<WaterDataHandle> nativeSafeHandle2 in this.usedHandles.Values)
		{
			nativeSafeHandle2.Dispose();
		}
		this.usedHandles.Clear();
		if (this.activeHandles.IsCreated)
		{
			this.activeHandles.Dispose();
		}
		if (this.waterDataHandles.IsCreated)
		{
			this.waterDataHandles.Dispose();
		}
		if (this.modifiedChunks.IsCreated)
		{
			this.modifiedChunks.Dispose();
		}
		this.IsInitialized = false;
	}

	// Token: 0x06005ABF RID: 23231 RVA: 0x00246714 File Offset: 0x00244914
	public string GetMemoryStats()
	{
		int count = this.usedHandles.Count;
		int count2 = this.freeHandles.Count;
		int count3 = this.newInitializedHandles.Count;
		int num = 0;
		foreach (NativeSafeHandle<WaterDataHandle> nativeSafeHandle in this.usedHandles.Values)
		{
			num += nativeSafeHandle.Target.CalculateOwnedBytes();
		}
		foreach (NativeSafeHandle<WaterDataHandle> nativeSafeHandle2 in this.freeHandles)
		{
			num += nativeSafeHandle2.Target.CalculateOwnedBytes();
		}
		foreach (WaterSimulationNative.HandleInitRequest handleInitRequest in this.newInitializedHandles)
		{
			int num2 = num;
			NativeSafeHandle<WaterDataHandle> safeHandle = handleInitRequest.safeHandle;
			num = num2 + safeHandle.Target.CalculateOwnedBytes();
		}
		int num3 = ProfilerUtils.CalculateUnsafeParallelHashSetBytes<ChunkKey>(this.activeHandles);
		num3 += ProfilerUtils.CalculateUnsafeParallelHashMapBytes<ChunkKey, WaterDataHandle>(this.waterDataHandles);
		return string.Format("Allocated Handles: {0}, Used Handles: {1}, Free Handles: {2}, Pending Handles: {3}, Handle Contents (MB): {4:F2}, Other Memory (MB): {5:F2}, Total Memory (MB): {6:F2}", new object[]
		{
			count + count2 + count3,
			count,
			count2,
			count3,
			(double)num * 9.5367431640625E-07,
			(double)num3 * 9.5367431640625E-07,
			(double)(num + num3) * 9.5367431640625E-07
		});
	}

	// Token: 0x06005AC0 RID: 23232 RVA: 0x002468DC File Offset: 0x00244ADC
	public string GetMemoryStatsDetailed()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Used Handles:");
		foreach (KeyValuePair<ChunkKey, NativeSafeHandle<WaterDataHandle>> keyValuePair in this.usedHandles)
		{
			stringBuilder.AppendFormat("Chunk ({0},{1}): {2}\n", keyValuePair.Key.x, keyValuePair.Key.z, keyValuePair.Value.Target.GetMemoryStats());
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04004556 RID: 17750
	public bool ShouldEnable = true;

	// Token: 0x04004558 RID: 17752
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isPaused;

	// Token: 0x04004559 RID: 17753
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<ChunkKey, NativeSafeHandle<WaterDataHandle>> usedHandles = new Dictionary<ChunkKey, NativeSafeHandle<WaterDataHandle>>();

	// Token: 0x0400455A RID: 17754
	[PublicizedFrom(EAccessModifier.Private)]
	public ConcurrentQueue<NativeSafeHandle<WaterDataHandle>> freeHandles = new ConcurrentQueue<NativeSafeHandle<WaterDataHandle>>();

	// Token: 0x0400455B RID: 17755
	[PublicizedFrom(EAccessModifier.Private)]
	public ConcurrentQueue<WaterSimulationNative.HandleInitRequest> newInitializedHandles = new ConcurrentQueue<WaterSimulationNative.HandleInitRequest>();

	// Token: 0x0400455C RID: 17756
	[PublicizedFrom(EAccessModifier.Private)]
	public ConcurrentQueue<ChunkKey> handlesToRemove = new ConcurrentQueue<ChunkKey>();

	// Token: 0x0400455D RID: 17757
	[PublicizedFrom(EAccessModifier.Private)]
	public UnsafeParallelHashSet<ChunkKey> activeHandles;

	// Token: 0x0400455E RID: 17758
	[PublicizedFrom(EAccessModifier.Private)]
	public UnsafeParallelHashMap<ChunkKey, WaterDataHandle> waterDataHandles;

	// Token: 0x0400455F RID: 17759
	[PublicizedFrom(EAccessModifier.Private)]
	public UnsafeParallelHashSet<ChunkKey> modifiedChunks;

	// Token: 0x04004560 RID: 17760
	[PublicizedFrom(EAccessModifier.Private)]
	public GroundWaterHeightMap groundWaterHeightMap;

	// Token: 0x04004561 RID: 17761
	public WaterSimulationApplyChanges changeApplier;

	// Token: 0x02000B65 RID: 2917
	[PublicizedFrom(EAccessModifier.Private)]
	public struct HandleInitRequest
	{
		// Token: 0x04004562 RID: 17762
		public ChunkKey chunkKey;

		// Token: 0x04004563 RID: 17763
		public NativeSafeHandle<WaterDataHandle> safeHandle;
	}

	// Token: 0x02000B66 RID: 2918
	public struct ChunkHandle
	{
		// Token: 0x17000943 RID: 2371
		// (get) Token: 0x06005AC3 RID: 23235 RVA: 0x002469CF File Offset: 0x00244BCF
		public bool IsValid
		{
			get
			{
				return this.sim != null;
			}
		}

		// Token: 0x06005AC4 RID: 23236 RVA: 0x002469DA File Offset: 0x00244BDA
		public ChunkHandle(WaterSimulationNative _sim, Chunk _chunk)
		{
			this.sim = _sim;
			this.chunkKey = new ChunkKey(_chunk);
		}

		// Token: 0x06005AC5 RID: 23237 RVA: 0x002469F0 File Offset: 0x00244BF0
		public void SetWaterMass(int _x, int _y, int _z, int _mass)
		{
			if (!this.IsValid)
			{
				return;
			}
			WaterDataHandle waterDataHandle;
			if (this.sim.waterDataHandles.TryGetValue(this.chunkKey, out waterDataHandle))
			{
				int voxelIndex = WaterDataHandle.GetVoxelIndex(_x, _y, _z);
				waterDataHandle.SetVoxelMass(voxelIndex, _mass);
				this.sim.activeHandles.Add(this.chunkKey);
			}
		}

		// Token: 0x06005AC6 RID: 23238 RVA: 0x00246A4C File Offset: 0x00244C4C
		public void SetVoxelSolid(int _x, int _y, int _z, BlockFaceFlag _flags)
		{
			if (!this.IsValid)
			{
				return;
			}
			WaterDataHandle waterDataHandle;
			if (this.sim.waterDataHandles.TryGetValue(this.chunkKey, out waterDataHandle))
			{
				waterDataHandle.SetVoxelSolid(_x, _y, _z, _flags);
				if (_flags != BlockFaceFlag.All)
				{
					this.WakeNeighbours(_x, _y, _z);
				}
			}
		}

		// Token: 0x06005AC7 RID: 23239 RVA: 0x00246A98 File Offset: 0x00244C98
		public void WakeNeighbours(int _x, int _y, int _z)
		{
			if (!this.IsValid)
			{
				return;
			}
			WaterDataHandle waterDataHandle;
			if (this.sim.waterDataHandles.TryGetValue(this.chunkKey, out waterDataHandle))
			{
				waterDataHandle.EnqueueVoxelWakeup(_x, _y, _z);
			}
			this.sim.modifiedChunks.Add(this.chunkKey);
		}

		// Token: 0x06005AC8 RID: 23240 RVA: 0x00246AE9 File Offset: 0x00244CE9
		public void Reset()
		{
			if (this.IsValid && this.sim.IsInitialized)
			{
				this.sim.handlesToRemove.Enqueue(this.chunkKey);
			}
			this.sim = null;
			this.chunkKey = default(ChunkKey);
		}

		// Token: 0x04004564 RID: 17764
		[PublicizedFrom(EAccessModifier.Private)]
		public WaterSimulationNative sim;

		// Token: 0x04004565 RID: 17765
		[PublicizedFrom(EAccessModifier.Private)]
		public ChunkKey chunkKey;
	}
}
