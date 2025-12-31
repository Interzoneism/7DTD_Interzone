using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

// Token: 0x02000B61 RID: 2913
public struct WaterNeighborCacheNative
{
	// Token: 0x06005AA1 RID: 23201 RVA: 0x0024541C File Offset: 0x0024361C
	public static WaterNeighborCacheNative InitializeCache(UnsafeParallelHashMap<ChunkKey, WaterDataHandle> _handles)
	{
		return new WaterNeighborCacheNative
		{
			waterDataHandles = _handles
		};
	}

	// Token: 0x06005AA2 RID: 23202 RVA: 0x0024543A File Offset: 0x0024363A
	public void SetChunk(ChunkKey _chunk)
	{
		this.chunkKey = _chunk;
		this.center = this.waterDataHandles[_chunk];
	}

	// Token: 0x06005AA3 RID: 23203 RVA: 0x00245455 File Offset: 0x00243655
	public void SetVoxel(int _x, int _y, int _z)
	{
		this.voxelX = _x;
		this.voxelY = _y;
		this.voxelZ = _z;
	}

	// Token: 0x06005AA4 RID: 23204 RVA: 0x0024546C File Offset: 0x0024366C
	public bool TryGetNeighbor(int2 _xzOffset, out ChunkKey _chunkKey, out WaterDataHandle _dataHandle, out int _x, out int _y, out int _z)
	{
		_x = this.voxelX + _xzOffset.x;
		_y = this.voxelY;
		_z = this.voxelZ + _xzOffset.y;
		if (!WaterUtils.IsVoxelOutsideChunk(_x, _z))
		{
			_chunkKey = this.chunkKey;
			_dataHandle = this.center;
			return true;
		}
		int num = _x & 15;
		int num2 = _z & 15;
		int x = this.chunkKey.x + (_x - num) / 16;
		int z = this.chunkKey.z + (_z - num2) / 16;
		_chunkKey = new ChunkKey(x, z);
		if (this.waterDataHandles.TryGetValue(_chunkKey, out _dataHandle))
		{
			_x = num;
			_z = num2;
			return true;
		}
		_chunkKey = default(ChunkKey);
		_dataHandle = default(WaterDataHandle);
		return false;
	}

	// Token: 0x0400453F RID: 17727
	public static readonly int2 X_NEG = new int2(-1, 0);

	// Token: 0x04004540 RID: 17728
	public static readonly int2 X_POS = new int2(1, 0);

	// Token: 0x04004541 RID: 17729
	public static readonly int2 Z_NEG = new int2(0, -1);

	// Token: 0x04004542 RID: 17730
	public static readonly int2 Z_POS = new int2(0, 1);

	// Token: 0x04004543 RID: 17731
	[PublicizedFrom(EAccessModifier.Private)]
	public UnsafeParallelHashMap<ChunkKey, WaterDataHandle> waterDataHandles;

	// Token: 0x04004544 RID: 17732
	public ChunkKey chunkKey;

	// Token: 0x04004545 RID: 17733
	public int voxelX;

	// Token: 0x04004546 RID: 17734
	public int voxelY;

	// Token: 0x04004547 RID: 17735
	public int voxelZ;

	// Token: 0x04004548 RID: 17736
	public WaterDataHandle center;
}
