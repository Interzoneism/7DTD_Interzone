using System;
using System.IO;
using Noemax.GZip;

// Token: 0x02000A1F RID: 2591
public class RegionFileChunkReader
{
	// Token: 0x06004F5D RID: 20317 RVA: 0x001F63A4 File Offset: 0x001F45A4
	public RegionFileChunkReader(RegionFileAccessAbstract regionFileAccess)
	{
		this.regionFileAccess = regionFileAccess;
		this.loadChunkMemoryStream = new MemoryStream(65536);
		this.loadChunkReader = new PooledBinaryReader();
		this.loadChunkReader.SetBaseStream(this.loadChunkMemoryStream);
	}

	// Token: 0x06004F5E RID: 20318 RVA: 0x001F6408 File Offset: 0x001F4608
	public PooledBinaryReader readIntoLoadStream(string _dir, int chunkX, int chunkZ, string ext, out uint version)
	{
		Stream inputStream = this.regionFileAccess.GetInputStream(_dir, chunkX, chunkZ, ext);
		if (inputStream == null)
		{
			version = 0U;
			return null;
		}
		using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
		{
			pooledBinaryReader.SetBaseStream(inputStream);
			for (int i = 0; i < this.magicBytes.Length; i++)
			{
				this.magicBytes[i] = pooledBinaryReader.ReadByte();
			}
			if (this.magicBytes[0] != 116 || this.magicBytes[1] != 116 || this.magicBytes[2] != 99 || this.magicBytes[3] != 0)
			{
				throw new Exception("Wrong chunk header!");
			}
			version = pooledBinaryReader.ReadUInt32();
		}
		if (this.zipLoadStream == null || this.innerLoadStream != inputStream)
		{
			if (this.zipLoadStream != null)
			{
				Log.Warning("RFM.Load: Creating new DeflateStream, underlying Stream changed");
			}
			this.zipLoadStream = new DeflateInputStream(inputStream, true);
			this.innerLoadStream = inputStream;
		}
		this.zipLoadStream.Restart();
		Stream source = this.zipLoadStream;
		this.loadChunkMemoryStream.SetLength(0L);
		StreamUtils.StreamCopy(source, this.loadChunkMemoryStream, this.loadBuffer, true);
		this.loadChunkMemoryStream.Position = 0L;
		inputStream.Close();
		return this.loadChunkReader;
	}

	// Token: 0x06004F5F RID: 20319 RVA: 0x001F6544 File Offset: 0x001F4744
	public void WriteBackup(string dir, int chunkX, int chunkZ)
	{
		using (Stream stream = SdFile.OpenWrite(string.Concat(new string[]
		{
			dir,
			"/error_backup_",
			chunkX.ToString(),
			"_",
			chunkZ.ToString(),
			".comp.bak"
		})))
		{
			this.innerLoadStream.Position = 0L;
			StreamUtils.StreamCopy(this.innerLoadStream, stream, null, true);
		}
		using (Stream stream2 = SdFile.OpenWrite(string.Concat(new string[]
		{
			dir,
			"/error_backup_",
			chunkX.ToString(),
			"_",
			chunkZ.ToString(),
			".uncomp.bak"
		})))
		{
			this.loadChunkMemoryStream.Position = 0L;
			StreamUtils.StreamCopy(this.loadChunkMemoryStream, stream2, null, true);
		}
	}

	// Token: 0x04003CAD RID: 15533
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionFileAccessAbstract regionFileAccess;

	// Token: 0x04003CAE RID: 15534
	[PublicizedFrom(EAccessModifier.Private)]
	public Stream innerLoadStream;

	// Token: 0x04003CAF RID: 15535
	[PublicizedFrom(EAccessModifier.Private)]
	public DeflateInputStream zipLoadStream;

	// Token: 0x04003CB0 RID: 15536
	[PublicizedFrom(EAccessModifier.Private)]
	public MemoryStream loadChunkMemoryStream;

	// Token: 0x04003CB1 RID: 15537
	[PublicizedFrom(EAccessModifier.Private)]
	public PooledBinaryReader loadChunkReader;

	// Token: 0x04003CB2 RID: 15538
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] loadBuffer = new byte[4096];

	// Token: 0x04003CB3 RID: 15539
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] magicBytes = new byte[4];
}
