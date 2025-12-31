using System;

// Token: 0x02000A23 RID: 2595
public interface IRegionFileChunkSnapshotUtil
{
	// Token: 0x06004F70 RID: 20336
	IRegionFileChunkSnapshot TakeSnapshot(Chunk chunk, bool saveIfUnchanged);

	// Token: 0x06004F71 RID: 20337
	void WriteSnapshot(IRegionFileChunkSnapshot snapshot, string dir, int chunkX, int chunkZ);

	// Token: 0x06004F72 RID: 20338
	Chunk LoadChunk(string dir, long key);

	// Token: 0x06004F73 RID: 20339
	void Free(IRegionFileChunkSnapshot snapshot);

	// Token: 0x06004F74 RID: 20340
	void Cleanup();
}
