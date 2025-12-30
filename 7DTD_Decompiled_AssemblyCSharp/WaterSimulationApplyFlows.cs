using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

// Token: 0x02000B62 RID: 2914
[BurstCompile(CompileSynchronously = true)]
public struct WaterSimulationApplyFlows : IJobParallelFor
{
	// Token: 0x06005AA6 RID: 23206 RVA: 0x00245570 File Offset: 0x00243770
	public unsafe void Execute(int chunkIndex)
	{
		this.neighborCache = WaterNeighborCacheNative.InitializeCache(this.waterDataHandles);
		ChunkKey chunkKey = this.processingChunks[chunkIndex];
		this.stats = this.waterStats[chunkIndex];
		WaterDataHandle waterDataHandle;
		if (this.waterDataHandles.TryGetValue(chunkKey, out waterDataHandle))
		{
			waterDataHandle.ApplyEnqueuedFlows();
			if (waterDataHandle.flowVoxels.IsEmpty)
			{
				this.nonFlowingChunks.AddNoResize(chunkKey);
			}
			else
			{
				this.neighborCache.SetChunk(chunkKey);
				foreach (KeyValue<int, int> keyValue in waterDataHandle.flowVoxels)
				{
					int key = keyValue.Key;
					UnsafeParallelHashMap<int, int>.Enumerator enumerator;
					keyValue = enumerator.Current;
					int num = *keyValue.Value;
					int3 voxelCoords = WaterDataHandle.GetVoxelCoords(key);
					int num2 = waterDataHandle.voxelData.Get(key);
					if (waterDataHandle.IsInGroundWater(voxelCoords.x, voxelCoords.y, voxelCoords.z))
					{
						num2 = math.min(num, 19500);
					}
					else
					{
						if (num == 0)
						{
							continue;
						}
						num2 += num;
					}
					waterDataHandle.voxelData.Set(key, num2);
					waterDataHandle.SetVoxelActive(key);
					this.neighborCache.SetVoxel(voxelCoords.x, voxelCoords.y, voxelCoords.z);
					this.WakeNeighbor(WaterNeighborCacheNative.X_NEG);
					this.WakeNeighbor(WaterNeighborCacheNative.X_POS);
					this.WakeNeighbor(1);
					this.WakeNeighbor(-1);
					this.WakeNeighbor(WaterNeighborCacheNative.Z_NEG);
					this.WakeNeighbor(WaterNeighborCacheNative.Z_POS);
				}
				this.activeChunkSet.Add(chunkKey);
			}
		}
		this.waterStats[chunkIndex] = this.stats;
	}

	// Token: 0x06005AA7 RID: 23207 RVA: 0x00245714 File Offset: 0x00243914
	[PublicizedFrom(EAccessModifier.Private)]
	public void WakeNeighbor(int _yOffset)
	{
		int num = this.neighborCache.voxelY + _yOffset;
		if (num < 0 || num > 255)
		{
			return;
		}
		this.neighborCache.center.SetVoxelActive(this.neighborCache.voxelX, num, this.neighborCache.voxelZ);
		this.stats.NumVoxelsWokeUp = this.stats.NumVoxelsWokeUp + 1;
	}

	// Token: 0x06005AA8 RID: 23208 RVA: 0x00245774 File Offset: 0x00243974
	[PublicizedFrom(EAccessModifier.Private)]
	public void WakeNeighbor(int2 _xzOffset)
	{
		ChunkKey item;
		WaterDataHandle waterDataHandle;
		int x;
		int y;
		int z;
		if (this.neighborCache.TryGetNeighbor(_xzOffset, out item, out waterDataHandle, out x, out y, out z))
		{
			if (item.Equals(this.neighborCache.chunkKey))
			{
				waterDataHandle.SetVoxelActive(x, y, z);
			}
			else
			{
				waterDataHandle.EnqueueVoxelActive(x, y, z);
				this.activeChunkSet.Add(item);
			}
			this.stats.NumVoxelsWokeUp = this.stats.NumVoxelsWokeUp + 1;
		}
	}

	// Token: 0x04004549 RID: 17737
	public NativeArray<ChunkKey> processingChunks;

	// Token: 0x0400454A RID: 17738
	public NativeList<ChunkKey>.ParallelWriter nonFlowingChunks;

	// Token: 0x0400454B RID: 17739
	public UnsafeParallelHashSet<ChunkKey>.ParallelWriter activeChunkSet;

	// Token: 0x0400454C RID: 17740
	public UnsafeParallelHashMap<ChunkKey, WaterDataHandle> waterDataHandles;

	// Token: 0x0400454D RID: 17741
	public NativeArray<WaterStats> waterStats;

	// Token: 0x0400454E RID: 17742
	[PublicizedFrom(EAccessModifier.Private)]
	public WaterNeighborCacheNative neighborCache;

	// Token: 0x0400454F RID: 17743
	[PublicizedFrom(EAccessModifier.Private)]
	public WaterStats stats;
}
