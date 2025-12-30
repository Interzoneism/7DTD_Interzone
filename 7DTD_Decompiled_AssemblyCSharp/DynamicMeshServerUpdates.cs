using System;
using System.Collections.Generic;

// Token: 0x0200032B RID: 811
public class DynamicMeshServerUpdates
{
	// Token: 0x0600178D RID: 6029 RVA: 0x000901CC File Offset: 0x0008E3CC
	public static void AddToPool(DynamicMeshServerUpdates data)
	{
		if (DynamicMeshServerUpdates.Pool.Count > 40)
		{
			data.Bytes = null;
			return;
		}
		DynamicMeshServerUpdates.Pool.Enqueue(data);
	}

	// Token: 0x0600178E RID: 6030 RVA: 0x000901EF File Offset: 0x0008E3EF
	public static DynamicMeshServerUpdates GetFromPool()
	{
		if (DynamicMeshServerUpdates.Pool.Count > 0)
		{
			return DynamicMeshServerUpdates.Pool.Dequeue();
		}
		return new DynamicMeshServerUpdates();
	}

	// Token: 0x0600178F RID: 6031 RVA: 0x0009020E File Offset: 0x0008E40E
	public long GetKey()
	{
		return WorldChunkCache.MakeChunkKey(World.toChunkXZ(DynamicMeshUnity.RoundChunk(this.ChunkX)), World.toChunkXZ(DynamicMeshUnity.RoundChunk(this.ChunkZ)));
	}

	// Token: 0x1700029C RID: 668
	// (get) Token: 0x06001790 RID: 6032 RVA: 0x00090235 File Offset: 0x0008E435
	// (set) Token: 0x06001791 RID: 6033 RVA: 0x0009023D File Offset: 0x0008E43D
	public int ChunkX { get; set; }

	// Token: 0x1700029D RID: 669
	// (get) Token: 0x06001792 RID: 6034 RVA: 0x00090246 File Offset: 0x0008E446
	// (set) Token: 0x06001793 RID: 6035 RVA: 0x0009024E File Offset: 0x0008E44E
	public int ChunkZ { get; set; }

	// Token: 0x1700029E RID: 670
	// (get) Token: 0x06001794 RID: 6036 RVA: 0x00090257 File Offset: 0x0008E457
	// (set) Token: 0x06001795 RID: 6037 RVA: 0x0009025F File Offset: 0x0008E45F
	public int StartY { get; set; }

	// Token: 0x1700029F RID: 671
	// (get) Token: 0x06001796 RID: 6038 RVA: 0x00090268 File Offset: 0x0008E468
	// (set) Token: 0x06001797 RID: 6039 RVA: 0x00090270 File Offset: 0x0008E470
	public int EndY { get; set; }

	// Token: 0x170002A0 RID: 672
	// (get) Token: 0x06001798 RID: 6040 RVA: 0x00090279 File Offset: 0x0008E479
	// (set) Token: 0x06001799 RID: 6041 RVA: 0x00090281 File Offset: 0x0008E481
	public int UpdateTime { get; set; }

	// Token: 0x170002A1 RID: 673
	// (get) Token: 0x0600179A RID: 6042 RVA: 0x0009028A File Offset: 0x0008E48A
	// (set) Token: 0x0600179B RID: 6043 RVA: 0x00090292 File Offset: 0x0008E492
	public List<byte> Bytes { get; set; }

	// Token: 0x0600179C RID: 6044 RVA: 0x0009029B File Offset: 0x0008E49B
	public DynamicMeshServerUpdates()
	{
		this.Bytes = new List<byte>();
	}

	// Token: 0x0600179D RID: 6045 RVA: 0x000902AE File Offset: 0x0008E4AE
	public void WriteAir(List<byte> tempArray)
	{
		tempArray.Add(0);
	}

	// Token: 0x0600179E RID: 6046 RVA: 0x000902B7 File Offset: 0x0008E4B7
	public void WriteLayer(List<byte> tempArray)
	{
		this.DataLayerCount++;
		this.Bytes.Add(byte.MaxValue);
		this.Bytes.AddRange(tempArray);
	}

	// Token: 0x0600179F RID: 6047 RVA: 0x000902E3 File Offset: 0x0008E4E3
	public void WriteEmptyLayer()
	{
		this.EmptyLayerCount++;
		this.Bytes.Add(128);
	}

	// Token: 0x060017A0 RID: 6048 RVA: 0x00090304 File Offset: 0x0008E504
	public void WriteBinaryBlock(List<byte> tempArray, BlockValue b, sbyte dens, long tex)
	{
		if (b.isair)
		{
			tempArray.Add(0);
			return;
		}
		byte[] bytes;
		if (!DynamicMeshServerUpdates.BlockBytes.TryGetValue(b.rawData, out bytes))
		{
			bytes = BitConverter.GetBytes(b.type);
			DynamicMeshServerUpdates.BlockBytes.Add(b.rawData, bytes);
		}
		tempArray.AddRange(bytes);
		if (!DynamicMeshServerUpdates.TexBytes.TryGetValue(tex, out bytes))
		{
			bytes = BitConverter.GetBytes(tex);
			DynamicMeshServerUpdates.TexBytes.Add(tex, bytes);
		}
		tempArray.AddRange(bytes);
		tempArray.Add((byte)dens);
	}

	// Token: 0x04000EE0 RID: 3808
	public const int DataLayer = 255;

	// Token: 0x04000EE1 RID: 3809
	public const byte EmptyLayer = 128;

	// Token: 0x04000EE2 RID: 3810
	public const int EmptyBlock = 0;

	// Token: 0x04000EE3 RID: 3811
	[PublicizedFrom(EAccessModifier.Private)]
	public static Queue<DynamicMeshServerUpdates> Pool = new Queue<DynamicMeshServerUpdates>(20);

	// Token: 0x04000EE4 RID: 3812
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<uint, byte[]> BlockBytes = new Dictionary<uint, byte[]>();

	// Token: 0x04000EE5 RID: 3813
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<long, byte[]> TexBytes = new Dictionary<long, byte[]>();

	// Token: 0x04000EEC RID: 3820
	public int EmptyLayerCount;

	// Token: 0x04000EED RID: 3821
	public int DataLayerCount;
}
