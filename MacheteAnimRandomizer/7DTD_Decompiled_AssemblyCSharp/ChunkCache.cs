using System;

// Token: 0x020009A7 RID: 2471
public class ChunkCache : IBlockAccess
{
	// Token: 0x06004B6D RID: 19309 RVA: 0x001DCED0 File Offset: 0x001DB0D0
	public ChunkCache(int _dim)
	{
		this.chunkArray = new Chunk[_dim, _dim];
	}

	// Token: 0x06004B6E RID: 19310 RVA: 0x001DCEE8 File Offset: 0x001DB0E8
	public void Init(World world, int x, int y, int z, int sx, int sy, int sz)
	{
		this.worldObj = world;
		this.chunkX = x >> 4;
		this.chunkZ = z >> 4;
		int num = sx >> 4;
		int num2 = sz >> 4;
		for (int i = this.chunkX; i <= num; i++)
		{
			for (int j = this.chunkZ; j <= num2; j++)
			{
				Chunk chunkSync = world.ChunkCache.GetChunkSync(i, j);
				if (chunkSync != null)
				{
					this.chunkArray[i - this.chunkX, j - this.chunkZ] = chunkSync;
				}
			}
		}
	}

	// Token: 0x06004B6F RID: 19311 RVA: 0x001DCF6B File Offset: 0x001DB16B
	public void Clear()
	{
		Array.Clear(this.chunkArray, 0, this.chunkArray.GetLength(0) * this.chunkArray.GetLength(1));
	}

	// Token: 0x06004B70 RID: 19312 RVA: 0x001DCF92 File Offset: 0x001DB192
	public BlockValue GetBlock(Vector3i _pos)
	{
		return this.GetBlock(_pos.x, _pos.y, _pos.z);
	}

	// Token: 0x06004B71 RID: 19313 RVA: 0x001DCFAC File Offset: 0x001DB1AC
	public BlockValue GetBlock(int _x, int _y, int _z)
	{
		if (_y < 0)
		{
			return BlockValue.Air;
		}
		if (_y >= 256)
		{
			return BlockValue.Air;
		}
		int num = (_x >> 4) - this.chunkX;
		int num2 = (_z >> 4) - this.chunkZ;
		if (num < 0 || num >= this.chunkArray.GetLength(0) || num2 < 0 || num2 >= this.chunkArray.GetLength(1))
		{
			Chunk chunkSync = this.worldObj.ChunkCache.GetChunkSync(World.toChunkXZ(_x), World.toChunkXZ(_z));
			if (chunkSync == null)
			{
				return BlockValue.Air;
			}
			if (!chunkSync.IsInitialized)
			{
				return BlockValue.Air;
			}
			return chunkSync.GetBlock(_x & 15, _y, _z & 15);
		}
		else
		{
			Chunk chunk = this.chunkArray[num, num2];
			if (chunk == null)
			{
				return BlockValue.Air;
			}
			if (!chunk.IsInitialized)
			{
				return BlockValue.Air;
			}
			return chunk.GetBlock(_x & 15, _y, _z & 15);
		}
	}

	// Token: 0x040039BA RID: 14778
	[PublicizedFrom(EAccessModifier.Private)]
	public int chunkX;

	// Token: 0x040039BB RID: 14779
	[PublicizedFrom(EAccessModifier.Private)]
	public int chunkZ;

	// Token: 0x040039BC RID: 14780
	[PublicizedFrom(EAccessModifier.Private)]
	public Chunk[,] chunkArray;

	// Token: 0x040039BD RID: 14781
	[PublicizedFrom(EAccessModifier.Private)]
	public World worldObj;
}
