using System;
using System.IO;

// Token: 0x02000A19 RID: 2585
[PublicizedFrom(EAccessModifier.Internal)]
public class ChunkMemoryStreamReader : MemoryStream
{
	// Token: 0x06004F3D RID: 20285 RVA: 0x001F5AA7 File Offset: 0x001F3CA7
	public ChunkMemoryStreamReader() : this(new byte[512000])
	{
	}

	// Token: 0x06004F3E RID: 20286 RVA: 0x001F5AB9 File Offset: 0x001F3CB9
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkMemoryStreamReader(byte[] _buffer) : base(_buffer)
	{
	}

	// Token: 0x06004F3F RID: 20287 RVA: 0x001F5AC2 File Offset: 0x001F3CC2
	public override void Close()
	{
		this.Seek(0L, SeekOrigin.Begin);
	}
}
