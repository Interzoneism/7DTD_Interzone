using System;

// Token: 0x02000A22 RID: 2594
public interface IRegionFileChunkSnapshot
{
	// Token: 0x17000821 RID: 2081
	// (get) Token: 0x06004F6C RID: 20332
	long Size { get; }

	// Token: 0x06004F6D RID: 20333
	void Update(Chunk chunk, bool saveIfUnchanged);

	// Token: 0x06004F6E RID: 20334
	void Write(RegionFileChunkWriter writer, string dir, int chunkX, int chunkZ);

	// Token: 0x06004F6F RID: 20335
	void Reset();
}
