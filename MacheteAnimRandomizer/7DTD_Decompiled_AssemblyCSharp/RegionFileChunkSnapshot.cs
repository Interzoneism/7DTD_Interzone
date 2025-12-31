using System;

// Token: 0x02000A20 RID: 2592
public class RegionFileChunkSnapshot : IRegionFileChunkSnapshot, IMemoryPoolableObject
{
	// Token: 0x17000820 RID: 2080
	// (get) Token: 0x06004F60 RID: 20320 RVA: 0x001F6640 File Offset: 0x001F4840
	public long Size
	{
		get
		{
			PooledMemoryStream pooledMemoryStream = this.stream;
			if (pooledMemoryStream == null)
			{
				return 0L;
			}
			return pooledMemoryStream.Length;
		}
	}

	// Token: 0x06004F61 RID: 20321 RVA: 0x001F6654 File Offset: 0x001F4854
	public void Update(Chunk chunk, bool saveIfUnchanged)
	{
		if (saveIfUnchanged || chunk.NeedsSaving)
		{
			if (this.stream != null)
			{
				this.stream.SetLength(0L);
			}
			else
			{
				this.stream = MemoryPools.poolMS.AllocSync(true);
			}
			try
			{
				using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
				{
					pooledBinaryWriter.SetBaseStream(this.stream);
					pooledBinaryWriter.Write(116);
					pooledBinaryWriter.Write(116);
					pooledBinaryWriter.Write(99);
					pooledBinaryWriter.Write(0);
					pooledBinaryWriter.Write(Chunk.CurrentSaveVersion);
					chunk.save(pooledBinaryWriter);
					this.stream.Position = 0L;
				}
			}
			catch (Exception ex)
			{
				Log.Error(string.Concat(new string[]
				{
					"Error writing blocks to stream (chunkX=",
					chunk.X.ToString(),
					" chunkZ=",
					chunk.Z.ToString(),
					"): ",
					ex.Message,
					"\nStackTrace: ",
					ex.StackTrace
				}));
				MemoryPools.poolMS.FreeSync(this.stream);
			}
		}
	}

	// Token: 0x06004F62 RID: 20322 RVA: 0x001F6790 File Offset: 0x001F4990
	public void Write(RegionFileChunkWriter writer, string dir, int chunkX, int chunkZ)
	{
		if (this.stream != null)
		{
			writer.WriteStreamCompressed(dir, chunkX, chunkZ, "7rg", this.stream);
		}
	}

	// Token: 0x06004F63 RID: 20323 RVA: 0x001F67AF File Offset: 0x001F49AF
	public void Cleanup()
	{
		this.Reset();
	}

	// Token: 0x06004F64 RID: 20324 RVA: 0x001F67B7 File Offset: 0x001F49B7
	public void Reset()
	{
		if (this.stream != null)
		{
			MemoryPools.poolMS.FreeSync(this.stream);
			this.stream = null;
		}
	}

	// Token: 0x04003CB4 RID: 15540
	public const string EXT = "7rg";

	// Token: 0x04003CB5 RID: 15541
	[PublicizedFrom(EAccessModifier.Private)]
	public PooledMemoryStream stream;
}
