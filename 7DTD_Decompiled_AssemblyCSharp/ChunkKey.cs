using System;

// Token: 0x02000B46 RID: 2886
public struct ChunkKey : IEquatable<ChunkKey>
{
	// Token: 0x060059DB RID: 23003 RVA: 0x002426E8 File Offset: 0x002408E8
	public ChunkKey(IChunk _chunk)
	{
		this.x = _chunk.X;
		this.z = _chunk.Z;
	}

	// Token: 0x060059DC RID: 23004 RVA: 0x00242702 File Offset: 0x00240902
	public ChunkKey(int _x, int _z)
	{
		this.x = _x;
		this.z = _z;
	}

	// Token: 0x060059DD RID: 23005 RVA: 0x00242712 File Offset: 0x00240912
	public override int GetHashCode()
	{
		return WaterUtils.GetVoxelKey2D(this.x, this.z);
	}

	// Token: 0x060059DE RID: 23006 RVA: 0x00242725 File Offset: 0x00240925
	public override bool Equals(object obj)
	{
		return base.Equals((ChunkKey)obj);
	}

	// Token: 0x060059DF RID: 23007 RVA: 0x00242742 File Offset: 0x00240942
	public bool Equals(ChunkKey other)
	{
		return this.x == other.x && this.z == other.z;
	}

	// Token: 0x040044B1 RID: 17585
	public int x;

	// Token: 0x040044B2 RID: 17586
	public int z;
}
