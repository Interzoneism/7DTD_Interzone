using System;

// Token: 0x02000A21 RID: 2593
public class ChunkSnapshotUtil : IRegionFileChunkSnapshotUtil
{
	// Token: 0x06004F66 RID: 20326 RVA: 0x001F67D8 File Offset: 0x001F49D8
	public ChunkSnapshotUtil(RegionFileAccessAbstract regionFileAccess)
	{
		this.regionFileAccess = regionFileAccess;
		this.chunkReader = new RegionFileChunkReader(regionFileAccess);
		this.chunkWriter = new RegionFileChunkWriter(regionFileAccess);
	}

	// Token: 0x06004F67 RID: 20327 RVA: 0x001F680F File Offset: 0x001F4A0F
	public IRegionFileChunkSnapshot TakeSnapshot(Chunk chunk, bool saveIfUnchanged)
	{
		RegionFileChunkSnapshot regionFileChunkSnapshot = this.poolSnapshots.AllocSync(true);
		regionFileChunkSnapshot.Update(chunk, saveIfUnchanged);
		return regionFileChunkSnapshot;
	}

	// Token: 0x06004F68 RID: 20328 RVA: 0x001F6825 File Offset: 0x001F4A25
	public void WriteSnapshot(IRegionFileChunkSnapshot snapshot, string dir, int chunkX, int chunkZ)
	{
		snapshot.Write(this.chunkWriter, dir, chunkX, chunkZ);
	}

	// Token: 0x06004F69 RID: 20329 RVA: 0x001F6838 File Offset: 0x001F4A38
	public Chunk LoadChunk(string dir, long key)
	{
		int chunkX = WorldChunkCache.extractX(key);
		int chunkZ = WorldChunkCache.extractZ(key);
		try
		{
			uint version;
			PooledBinaryReader pooledBinaryReader = this.chunkReader.readIntoLoadStream(dir, chunkX, chunkZ, "7rg", out version);
			if (pooledBinaryReader == null)
			{
				return null;
			}
			Chunk chunk = MemoryPools.PoolChunks.AllocSync(true);
			chunk.load(pooledBinaryReader, version);
			chunk.NeedsRegeneration = true;
			return chunk;
		}
		catch (Exception e)
		{
			Log.Error(string.Concat(new string[]
			{
				"EXCEPTION: In load chunk (chunkX=",
				chunkX.ToString(),
				" chunkZ=",
				chunkZ.ToString(),
				")"
			}));
			Log.Exception(e);
			try
			{
				this.chunkReader.WriteBackup(dir, chunkX, chunkZ);
			}
			catch (Exception e2)
			{
				Log.Error("Error backing up data:");
				Log.Exception(e2);
			}
		}
		try
		{
			this.regionFileAccess.Remove(dir, chunkX, chunkZ);
		}
		catch (Exception ex)
		{
			Log.Error(string.Concat(new string[]
			{
				"In remove chunk (chunkX=",
				chunkX.ToString(),
				" chunkZ=",
				chunkZ.ToString(),
				"):",
				ex.Message
			}));
		}
		return null;
	}

	// Token: 0x06004F6A RID: 20330 RVA: 0x001F6984 File Offset: 0x001F4B84
	public void Free(IRegionFileChunkSnapshot iSnapshot)
	{
		if (iSnapshot == null)
		{
			return;
		}
		RegionFileChunkSnapshot regionFileChunkSnapshot = iSnapshot as RegionFileChunkSnapshot;
		if (regionFileChunkSnapshot != null)
		{
			this.poolSnapshots.FreeSync(regionFileChunkSnapshot);
			return;
		}
		Log.Error("Attempting to free snapshot of wrong type. Expected: RegionFileChunkSnapshot, Actual: " + iSnapshot.GetType().Name);
	}

	// Token: 0x06004F6B RID: 20331 RVA: 0x001F69C6 File Offset: 0x001F4BC6
	public void Cleanup()
	{
		this.poolSnapshots.Cleanup();
	}

	// Token: 0x04003CB6 RID: 15542
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionFileAccessAbstract regionFileAccess;

	// Token: 0x04003CB7 RID: 15543
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionFileChunkReader chunkReader;

	// Token: 0x04003CB8 RID: 15544
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionFileChunkWriter chunkWriter;

	// Token: 0x04003CB9 RID: 15545
	[PublicizedFrom(EAccessModifier.Private)]
	public MemoryPooledObject<RegionFileChunkSnapshot> poolSnapshots = new MemoryPooledObject<RegionFileChunkSnapshot>(255);
}
