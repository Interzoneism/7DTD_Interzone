using System;
using System.IO;

// Token: 0x02000A18 RID: 2584
[PublicizedFrom(EAccessModifier.Internal)]
public class ChunkMemoryStreamWriter : MemoryStream
{
	// Token: 0x06004F39 RID: 20281 RVA: 0x001F5A22 File Offset: 0x001F3C22
	public ChunkMemoryStreamWriter() : this(new byte[512000])
	{
	}

	// Token: 0x06004F3A RID: 20282 RVA: 0x001F5A34 File Offset: 0x001F3C34
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkMemoryStreamWriter(byte[] _buffer) : base(_buffer)
	{
		this.buffer = _buffer;
	}

	// Token: 0x06004F3B RID: 20283 RVA: 0x001F5A44 File Offset: 0x001F3C44
	public void Init(RegionFileAccessMultipleChunks _regionFileAccess, string _dir, int _x, int _z, string _ext)
	{
		this.dir = _dir;
		this.chunkX = _x;
		this.chunkZ = _z;
		this.ext = _ext;
		this.regionFileAccess = _regionFileAccess;
		this.Seek(0L, SeekOrigin.Begin);
	}

	// Token: 0x06004F3C RID: 20284 RVA: 0x001F5A75 File Offset: 0x001F3C75
	public override void Close()
	{
		this.regionFileAccess.Write(this.dir, this.chunkX, this.chunkZ, this.ext, this.buffer, (int)this.Position);
	}

	// Token: 0x04003CA2 RID: 15522
	[PublicizedFrom(EAccessModifier.Private)]
	public string dir;

	// Token: 0x04003CA3 RID: 15523
	[PublicizedFrom(EAccessModifier.Private)]
	public int chunkX;

	// Token: 0x04003CA4 RID: 15524
	[PublicizedFrom(EAccessModifier.Private)]
	public int chunkZ;

	// Token: 0x04003CA5 RID: 15525
	[PublicizedFrom(EAccessModifier.Private)]
	public string ext;

	// Token: 0x04003CA6 RID: 15526
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionFileAccessMultipleChunks regionFileAccess;

	// Token: 0x04003CA7 RID: 15527
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] buffer;
}
