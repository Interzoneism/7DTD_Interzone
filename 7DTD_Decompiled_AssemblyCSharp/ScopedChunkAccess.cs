using System;

// Token: 0x02000B6C RID: 2924
public static class ScopedChunkAccess
{
	// Token: 0x06005AD4 RID: 23252 RVA: 0x00247080 File Offset: 0x00245280
	public static ScopedChunkReadAccess GetChunkReadAccess(ChunkCluster chunks, int x, int z)
	{
		return new ScopedChunkReadAccess(chunks.GetChunkSync(x, z));
	}

	// Token: 0x06005AD5 RID: 23253 RVA: 0x0024708F File Offset: 0x0024528F
	public static ScopedChunkWriteAccess GetChunkWriteAccess(ChunkCluster chunks, int x, int z)
	{
		return new ScopedChunkWriteAccess(chunks.GetChunkSync(x, z));
	}

	// Token: 0x06005AD6 RID: 23254 RVA: 0x0024709E File Offset: 0x0024529E
	public static ScopedChunkWriteAccess GetChunkWriteAccess(ChunkCluster chunks, long key)
	{
		return new ScopedChunkWriteAccess(chunks.GetChunkSync(key));
	}
}
