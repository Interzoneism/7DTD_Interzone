using System;

// Token: 0x020009AA RID: 2474
public class ChunkCacheNeighborChunks
{
	// Token: 0x06004B86 RID: 19334 RVA: 0x001DD764 File Offset: 0x001DB964
	public ChunkCacheNeighborChunks(IChunkAccess _chunkAccess)
	{
		IChunk[,] array = new Chunk[3, 3];
		this.chunks = array;
		base..ctor();
		this.chunkAccess = _chunkAccess;
	}

	// Token: 0x06004B87 RID: 19335 RVA: 0x001DD790 File Offset: 0x001DB990
	public void Init(IChunk _chunk, IChunk[] _chunkArr)
	{
		this[0, 0] = _chunk;
		this[-1, 0] = _chunkArr[1];
		this[1, 0] = _chunkArr[0];
		this[0, -1] = _chunkArr[3];
		this[0, 1] = _chunkArr[2];
		this[-1, -1] = _chunkArr[5];
		this[1, -1] = _chunkArr[7];
		this[-1, 1] = _chunkArr[6];
		this[1, 1] = _chunkArr[4];
	}

	// Token: 0x06004B88 RID: 19336 RVA: 0x001DD7FE File Offset: 0x001DB9FE
	public void Clear()
	{
		Array.Clear(this.chunks, 0, this.chunks.GetLength(0) * this.chunks.GetLength(1));
	}

	// Token: 0x170007D6 RID: 2006
	public IChunk this[int x, int y]
	{
		get
		{
			return this.chunks[x + 1, y + 1];
		}
		set
		{
			this.chunks[x + 1, y + 1] = value;
		}
	}

	// Token: 0x040039C5 RID: 14789
	[PublicizedFrom(EAccessModifier.Private)]
	public IChunk[,] chunks;

	// Token: 0x040039C6 RID: 14790
	[PublicizedFrom(EAccessModifier.Private)]
	public IChunkAccess chunkAccess;
}
