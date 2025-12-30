using System;

// Token: 0x02000B7E RID: 2942
public static class WaterUtils
{
	// Token: 0x06005B2E RID: 23342 RVA: 0x00248780 File Offset: 0x00246980
	public static int GetVoxelKey2D(int _x, int _z)
	{
		return _x * 8976890 + _z * 981131;
	}

	// Token: 0x06005B2F RID: 23343 RVA: 0x00248791 File Offset: 0x00246991
	public static int GetVoxelKey(int _x, int _y, int _z = 0)
	{
		return _x * 8976890 + _y * 981131 + _z;
	}

	// Token: 0x06005B30 RID: 23344 RVA: 0x002487A4 File Offset: 0x002469A4
	public static bool IsChunkSafeToUpdate(Chunk chunk)
	{
		return chunk != null && !chunk.NeedsDecoration && !chunk.NeedsCopying && !chunk.IsLocked;
	}

	// Token: 0x06005B31 RID: 23345 RVA: 0x002487C8 File Offset: 0x002469C8
	public static bool TryOpenChunkForUpdate(ChunkCluster _chunks, long _key, out Chunk _chunk)
	{
		bool result;
		using (ScopedChunkWriteAccess chunkWriteAccess = ScopedChunkAccess.GetChunkWriteAccess(_chunks, _key))
		{
			Chunk chunk = chunkWriteAccess.Chunk;
			if (!WaterUtils.IsChunkSafeToUpdate(chunk))
			{
				_chunk = null;
				result = false;
			}
			else
			{
				_chunk = chunk;
				_chunk.InProgressWaterSim = true;
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06005B32 RID: 23346 RVA: 0x00248824 File Offset: 0x00246A24
	public static bool CanWaterFlowThrough(BlockValue _bv)
	{
		Block block = _bv.Block;
		return block != null && block.WaterFlowMask != BlockFaceFlag.All;
	}

	// Token: 0x06005B33 RID: 23347 RVA: 0x0024884C File Offset: 0x00246A4C
	public static bool CanWaterFlowThrough(int _blockId)
	{
		Block block = Block.list[_blockId];
		return block != null && block.WaterFlowMask != BlockFaceFlag.All;
	}

	// Token: 0x06005B34 RID: 23348 RVA: 0x00248873 File Offset: 0x00246A73
	public static int GetWaterLevel(WaterValue waterValue)
	{
		if (waterValue.GetMass() > 195)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06005B35 RID: 23349 RVA: 0x00248886 File Offset: 0x00246A86
	public static bool IsVoxelOutsideChunk(int _neighborX, int _neighborZ)
	{
		return _neighborX < 0 || _neighborX > 15 || _neighborZ < 0 || _neighborZ > 15;
	}
}
