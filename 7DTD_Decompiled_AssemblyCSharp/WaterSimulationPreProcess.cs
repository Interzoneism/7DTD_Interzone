using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000B68 RID: 2920
[BurstCompile(CompileSynchronously = true)]
public struct WaterSimulationPreProcess : IJob
{
	// Token: 0x06005ACA RID: 23242 RVA: 0x00246BE0 File Offset: 0x00244DE0
	public void Execute()
	{
		this.neighborCache = WaterNeighborCacheNative.InitializeCache(this.waterDataHandles);
		using (UnsafeParallelHashSet<ChunkKey>.Enumerator enumerator = this.modifiedChunks.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ChunkKey chunkKey = enumerator.Current;
				WaterDataHandle waterDataHandle;
				if (this.waterDataHandles.TryGetValue(chunkKey, out waterDataHandle))
				{
					this.neighborCache.SetChunk(chunkKey);
					int num = 0;
					using (UnsafeParallelHashSet<int>.Enumerator enumerator2 = waterDataHandle.voxelsToWakeup.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (++num > 65536)
							{
								Debug.LogError(string.Format("[WaterSimulationPreProcess] Number of wakeups for chunk ({0}, {1}) has exceeded the volume of a chunk {2}.", chunkKey.x, chunkKey.z, 65536));
								break;
							}
							int3 voxelCoords = WaterDataHandle.GetVoxelCoords(enumerator2.Current);
							waterDataHandle.SetVoxelActive(voxelCoords.x, voxelCoords.y, voxelCoords.z);
							this.neighborCache.SetVoxel(voxelCoords.x, voxelCoords.y, voxelCoords.z);
							this.WakeNeighbor(WaterNeighborCacheNative.X_NEG);
							this.WakeNeighbor(WaterNeighborCacheNative.X_POS);
							this.WakeNeighbor(1);
							this.WakeNeighbor(-1);
							this.WakeNeighbor(WaterNeighborCacheNative.Z_NEG);
							this.WakeNeighbor(WaterNeighborCacheNative.Z_POS);
						}
						waterDataHandle.voxelsToWakeup.Clear();
						this.activeChunks.Add(chunkKey);
					}
				}
			}
			this.modifiedChunks.Clear();
			using (NativeArray<ChunkKey> nativeArray = this.activeChunks.ToNativeArray(Allocator.Temp))
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					ChunkKey chunkKey2 = nativeArray[i];
					this.TryTrackChunk(chunkKey2.x + 1, chunkKey2.z);
					this.TryTrackChunk(chunkKey2.x - 1, chunkKey2.z);
					this.TryTrackChunk(chunkKey2.x, chunkKey2.z + 1);
					this.TryTrackChunk(chunkKey2.x, chunkKey2.z - 1);
				}
			}
		}
	}

	// Token: 0x06005ACB RID: 23243 RVA: 0x00246E54 File Offset: 0x00245054
	[PublicizedFrom(EAccessModifier.Private)]
	public void WakeNeighbor(int _yOffset)
	{
		int num = this.neighborCache.voxelY + _yOffset;
		if (num < 0 || num > 255)
		{
			return;
		}
		this.neighborCache.center.SetVoxelActive(this.neighborCache.voxelX, num, this.neighborCache.voxelZ);
	}

	// Token: 0x06005ACC RID: 23244 RVA: 0x00246EA4 File Offset: 0x002450A4
	[PublicizedFrom(EAccessModifier.Private)]
	public void WakeNeighbor(int2 _xzOffset)
	{
		ChunkKey chunkKey;
		WaterDataHandle waterDataHandle;
		int x;
		int y;
		int z;
		if (this.neighborCache.TryGetNeighbor(_xzOffset, out chunkKey, out waterDataHandle, out x, out y, out z))
		{
			waterDataHandle.SetVoxelActive(x, y, z);
		}
	}

	// Token: 0x06005ACD RID: 23245 RVA: 0x00246ED4 File Offset: 0x002450D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void TryTrackChunk(int _chunkX, int _chunkZ)
	{
		ChunkKey chunkKey = new ChunkKey(_chunkX, _chunkZ);
		if (this.waterDataHandles.ContainsKey(chunkKey))
		{
			this.activeChunks.Add(chunkKey);
		}
	}

	// Token: 0x0400456A RID: 17770
	public UnsafeParallelHashSet<ChunkKey> activeChunks;

	// Token: 0x0400456B RID: 17771
	public UnsafeParallelHashMap<ChunkKey, WaterDataHandle> waterDataHandles;

	// Token: 0x0400456C RID: 17772
	public UnsafeParallelHashSet<ChunkKey> modifiedChunks;

	// Token: 0x0400456D RID: 17773
	[PublicizedFrom(EAccessModifier.Private)]
	public WaterNeighborCacheNative neighborCache;
}
