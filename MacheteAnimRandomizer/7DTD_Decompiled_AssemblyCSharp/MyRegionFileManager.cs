using System;

// Token: 0x020009D4 RID: 2516
[PublicizedFrom(EAccessModifier.Internal)]
public class MyRegionFileManager : RegionFileManager
{
	// Token: 0x06004D1D RID: 19741 RVA: 0x001E9C98 File Offset: 0x001E7E98
	public MyRegionFileManager(World _world, IChunkProvider _chunkProvider, RegionFileManager _terrainRegionCache, string _loadDirectory, string _saveDirectory, int _maxChunksInCache, bool _bAutoSaveOnChunkDrop) : base(_loadDirectory, _saveDirectory, _maxChunksInCache, _bAutoSaveOnChunkDrop)
	{
		this.terrainRegionManager = _terrainRegionCache;
		this.world = _world;
		this.chunkProvider = _chunkProvider;
	}

	// Token: 0x06004D1E RID: 19742 RVA: 0x001E9CC0 File Offset: 0x001E7EC0
	public override void SaveChunkSnapshot(Chunk _chunk, bool _saveIfUnchanged)
	{
		if (this.world.IsEditor() && !this.chunkProvider.IsDecorationsEnabled())
		{
			Chunk chunk = MemoryPools.PoolChunks.AllocSync(true);
			chunk.X = _chunk.X;
			chunk.Z = _chunk.Z;
			Chunk.ToTerrain(_chunk, chunk);
			chunk.NeedsDecoration = false;
			this.terrainRegionManager.AddChunkSync(chunk, false);
		}
		base.SaveChunkSnapshot(_chunk, _saveIfUnchanged);
	}

	// Token: 0x04003B0C RID: 15116
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionFileManager terrainRegionManager;

	// Token: 0x04003B0D RID: 15117
	[PublicizedFrom(EAccessModifier.Private)]
	public World world;

	// Token: 0x04003B0E RID: 15118
	[PublicizedFrom(EAccessModifier.Private)]
	public IChunkProvider chunkProvider;
}
