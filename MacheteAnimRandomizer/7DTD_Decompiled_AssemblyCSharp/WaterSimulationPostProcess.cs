using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

// Token: 0x02000B67 RID: 2919
[BurstCompile(CompileSynchronously = true)]
public struct WaterSimulationPostProcess : IJob
{
	// Token: 0x06005AC9 RID: 23241 RVA: 0x00246B2C File Offset: 0x00244D2C
	public void Execute()
	{
		for (int i = 0; i < this.nonFlowingChunks.Length; i++)
		{
			ChunkKey key = this.nonFlowingChunks[i];
			WaterDataHandle waterDataHandle;
			if (this.waterDataHandles.TryGetValue(key, out waterDataHandle))
			{
				this.activeChunks.Remove(this.nonFlowingChunks[i]);
			}
		}
		for (int j = 0; j < this.processingChunks.Length; j++)
		{
			ChunkKey chunkKey = this.processingChunks[j];
			WaterDataHandle waterDataHandle2;
			if (this.waterDataHandles.TryGetValue(chunkKey, out waterDataHandle2) && waterDataHandle2.activationsFromOtherChunks.Count > 0)
			{
				waterDataHandle2.ApplyEnqueuedActivations();
				this.activeChunks.Add(chunkKey);
			}
		}
	}

	// Token: 0x04004566 RID: 17766
	public NativeArray<ChunkKey> processingChunks;

	// Token: 0x04004567 RID: 17767
	public NativeList<ChunkKey> nonFlowingChunks;

	// Token: 0x04004568 RID: 17768
	public UnsafeParallelHashSet<ChunkKey> activeChunks;

	// Token: 0x04004569 RID: 17769
	public UnsafeParallelHashMap<ChunkKey, WaterDataHandle> waterDataHandles;
}
