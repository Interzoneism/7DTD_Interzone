using System;

// Token: 0x020009A9 RID: 2473
public class ChunkCacheNeighborBlocks : INeighborBlockCache
{
	// Token: 0x06004B7A RID: 19322 RVA: 0x001DD088 File Offset: 0x001DB288
	public ChunkCacheNeighborBlocks(ChunkCacheNeighborChunks _nChunks)
	{
		IChunk[] array = new Chunk[9];
		this.chunks = array;
		this.centerBX = int.MaxValue;
		this.centerBZ = int.MaxValue;
		this.curChunkX = int.MaxValue;
		this.curChunkZ = int.MaxValue;
		this.firstException = true;
		base..ctor();
		this.nChunks = _nChunks;
	}

	// Token: 0x06004B7B RID: 19323 RVA: 0x001DD0E4 File Offset: 0x001DB2E4
	public void Init(int _bX, int _bZ)
	{
		IChunk chunk = this.nChunks[0, 0];
		if (_bX == this.centerBX && _bZ == this.centerBZ && chunk.X == this.curChunkX && chunk.Z == this.curChunkZ)
		{
			return;
		}
		this.centerBX = _bX;
		this.centerBZ = _bZ;
		this.curChunkX = chunk.X;
		this.curChunkZ = chunk.Z;
		this.chunks[0] = ((_bX > 0) ? ((_bZ > 0) ? chunk : this.nChunks[0, -1]) : ((_bZ > 0) ? this.nChunks[-1, 0] : this.nChunks[-1, -1]));
		this.chunks[1] = ((_bZ > 0) ? chunk : this.nChunks[0, -1]);
		this.chunks[2] = ((_bX < 15) ? ((_bZ > 0) ? chunk : this.nChunks[0, -1]) : ((_bZ > 0) ? this.nChunks[1, 0] : this.nChunks[1, -1]));
		this.chunks[3] = ((_bX > 0) ? chunk : this.nChunks[-1, 0]);
		this.chunks[4] = chunk;
		this.chunks[5] = ((_bX < 15) ? chunk : this.nChunks[1, 0]);
		this.chunks[6] = ((_bX > 0) ? ((_bZ < 15) ? chunk : this.nChunks[0, 1]) : ((_bZ < 15) ? this.nChunks[-1, 0] : this.nChunks[-1, 1]));
		this.chunks[7] = ((_bZ < 15) ? chunk : this.nChunks[0, 1]);
		this.chunks[8] = ((_bX < 15) ? ((_bZ < 15) ? chunk : this.nChunks[0, 1]) : ((_bZ < 15) ? this.nChunks[1, 0] : this.nChunks[1, 1]));
	}

	// Token: 0x06004B7C RID: 19324 RVA: 0x001DD2E0 File Offset: 0x001DB4E0
	public void Clear()
	{
		this.centerBX = int.MaxValue;
		this.centerBZ = int.MaxValue;
		this.curChunkX = int.MaxValue;
		this.curChunkZ = int.MaxValue;
		Array.Clear(this.chunks, 0, this.chunks.Length);
	}

	// Token: 0x06004B7D RID: 19325 RVA: 0x001DD32D File Offset: 0x001DB52D
	public IChunk GetChunk(int x, int z)
	{
		return this.chunks[x + 1 + (z + 1) * 3];
	}

	// Token: 0x06004B7E RID: 19326 RVA: 0x001DD33F File Offset: 0x001DB53F
	public IChunk GetNeighborChunk(int x, int z)
	{
		return this.nChunks[x, z];
	}

	// Token: 0x06004B7F RID: 19327 RVA: 0x001DD350 File Offset: 0x001DB550
	public bool IsBlockInCache(int relx, int absy, int relz)
	{
		if (absy < 0 || absy > 255)
		{
			return false;
		}
		int num = relx + 1 + (relz + 1) * 3;
		return num <= 8 && this.chunks[num] != null;
	}

	// Token: 0x06004B80 RID: 19328 RVA: 0x001DD386 File Offset: 0x001DB586
	public Vector3i GetChunkPos(int relx, int absy, int relz)
	{
		return new Vector3i(this.centerBX + relx & 15, absy, this.centerBZ + relz & 15);
	}

	// Token: 0x06004B81 RID: 19329 RVA: 0x001DD3A4 File Offset: 0x001DB5A4
	public BlockValue Get(int relx, int absy, int relz)
	{
		int num = relx + 1 + (relz + 1) * 3;
		if (absy < 0 || num >= 9)
		{
			return BlockValue.Air;
		}
		BlockValue result;
		try
		{
			BlockValue blockValue = this.chunks[num].GetBlock(this.centerBX + relx & 15, absy, this.centerBZ + relz & 15);
			if (!GameManager.bShowDecorBlocks && blockValue.Block.IsDecoration)
			{
				blockValue = BlockValue.Air;
			}
			if (!GameManager.bShowLootBlocks)
			{
				if (blockValue.Block is BlockLoot)
				{
					blockValue = BlockValue.Air;
				}
				else
				{
					BlockCompositeTileEntity blockCompositeTileEntity = blockValue.Block as BlockCompositeTileEntity;
					if (blockCompositeTileEntity != null && blockCompositeTileEntity.CompositeData.HasFeature<ITileEntityLootable>())
					{
						blockValue = BlockValue.Air;
					}
				}
			}
			result = blockValue;
		}
		catch (Exception)
		{
			if (this.firstException)
			{
				Log.Error(string.Concat(new string[]
				{
					"ChunkCacheNeighborBlocks.Get: relX=",
					relx.ToString(),
					" relz=",
					relz.ToString(),
					" len=",
					this.chunks.Length.ToString()
				}));
				this.firstException = false;
			}
			result = BlockValue.Air;
		}
		return result;
	}

	// Token: 0x06004B82 RID: 19330 RVA: 0x001DD4C8 File Offset: 0x001DB6C8
	public byte GetStab(int relx, int absy, int relz)
	{
		byte result = 0;
		try
		{
			result = this.chunks[relx + 1 + (relz + 1) * 3].GetStability(this.centerBX + relx & 15, absy, this.centerBZ + relz & 15);
		}
		catch (Exception ex)
		{
			Log.Out(string.Concat(new string[]
			{
				"Bad ChunkCacheNeighborBlocks index (",
				relx.ToString(),
				", ",
				absy.ToString(),
				", ",
				relz.ToString(),
				"), \nException: ",
				ex.ToString()
			}));
		}
		return result;
	}

	// Token: 0x06004B83 RID: 19331 RVA: 0x001DD574 File Offset: 0x001DB774
	public bool IsWater(int relx, int absy, int relz)
	{
		int num = relx + 1 + (relz + 1) * 3;
		if (absy < 0 || num >= 9)
		{
			return false;
		}
		bool result = false;
		try
		{
			result = this.chunks[num].IsWater(this.centerBX + relx & 15, absy, this.centerBZ + relz & 15);
		}
		catch (Exception ex)
		{
			Log.Out(string.Concat(new string[]
			{
				"Bad ChunkCacheNeighborBlocks index (",
				relx.ToString(),
				", ",
				absy.ToString(),
				", ",
				relz.ToString(),
				"), \nException: ",
				ex.ToString()
			}));
		}
		return result;
	}

	// Token: 0x06004B84 RID: 19332 RVA: 0x001DD62C File Offset: 0x001DB82C
	public bool IsAir(int relx, int absy, int relz)
	{
		int num = relx + 1 + (relz + 1) * 3;
		if (absy < 0 || num >= 9)
		{
			return false;
		}
		int x = this.centerBX + relx & 15;
		int z = this.centerBZ + relz & 15;
		bool result = false;
		try
		{
			IChunk chunk = this.chunks[num];
			return chunk.GetBlock(x, absy, z).isair && !chunk.IsWater(x, absy, z);
		}
		catch (Exception ex)
		{
			Log.Out(string.Concat(new string[]
			{
				"Bad ChunkCacheNeighborBlocks index (",
				relx.ToString(),
				", ",
				absy.ToString(),
				", ",
				relz.ToString(),
				"), \nException: ",
				ex.ToString()
			}));
		}
		return result;
	}

	// Token: 0x06004B85 RID: 19333 RVA: 0x001DD70C File Offset: 0x001DB90C
	public override string ToString()
	{
		return string.Format("BlockCache -- Chunk Pos ({0}, {1}) -- Block Pos ({2}, {3})", new object[]
		{
			this.curChunkX,
			this.curChunkZ,
			this.centerBX,
			this.centerBZ
		});
	}

	// Token: 0x040039BE RID: 14782
	[PublicizedFrom(EAccessModifier.Private)]
	public IChunk[] chunks;

	// Token: 0x040039BF RID: 14783
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkCacheNeighborChunks nChunks;

	// Token: 0x040039C0 RID: 14784
	[PublicizedFrom(EAccessModifier.Private)]
	public int centerBX;

	// Token: 0x040039C1 RID: 14785
	[PublicizedFrom(EAccessModifier.Private)]
	public int centerBZ;

	// Token: 0x040039C2 RID: 14786
	[PublicizedFrom(EAccessModifier.Private)]
	public int curChunkX;

	// Token: 0x040039C3 RID: 14787
	[PublicizedFrom(EAccessModifier.Private)]
	public int curChunkZ;

	// Token: 0x040039C4 RID: 14788
	[PublicizedFrom(EAccessModifier.Private)]
	public bool firstException;
}
