using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

// Token: 0x02000315 RID: 789
public class DynamicMeshChunkData
{
	// Token: 0x1700027E RID: 638
	// (get) Token: 0x0600166B RID: 5739 RVA: 0x0008283A File Offset: 0x00080A3A
	// (set) Token: 0x0600166C RID: 5740 RVA: 0x00082842 File Offset: 0x00080A42
	public int X { get; set; }

	// Token: 0x1700027F RID: 639
	// (get) Token: 0x0600166D RID: 5741 RVA: 0x0008284B File Offset: 0x00080A4B
	// (set) Token: 0x0600166E RID: 5742 RVA: 0x00082853 File Offset: 0x00080A53
	public int OffsetY { get; set; }

	// Token: 0x17000280 RID: 640
	// (get) Token: 0x0600166F RID: 5743 RVA: 0x0008285C File Offset: 0x00080A5C
	// (set) Token: 0x06001670 RID: 5744 RVA: 0x00082864 File Offset: 0x00080A64
	public int Z { get; set; }

	// Token: 0x17000281 RID: 641
	// (get) Token: 0x06001671 RID: 5745 RVA: 0x0008286D File Offset: 0x00080A6D
	// (set) Token: 0x06001672 RID: 5746 RVA: 0x00082875 File Offset: 0x00080A75
	public int UpdateTime { get; set; }

	// Token: 0x17000282 RID: 642
	// (get) Token: 0x06001673 RID: 5747 RVA: 0x0008287E File Offset: 0x00080A7E
	// (set) Token: 0x06001674 RID: 5748 RVA: 0x00082886 File Offset: 0x00080A86
	public byte MainBiome { get; set; }

	// Token: 0x17000283 RID: 643
	// (get) Token: 0x06001675 RID: 5749 RVA: 0x0008288F File Offset: 0x00080A8F
	// (set) Token: 0x06001676 RID: 5750 RVA: 0x00082897 File Offset: 0x00080A97
	public int EndY { get; set; }

	// Token: 0x17000284 RID: 644
	// (get) Token: 0x06001677 RID: 5751 RVA: 0x000828A0 File Offset: 0x00080AA0
	// (set) Token: 0x06001678 RID: 5752 RVA: 0x000828A8 File Offset: 0x00080AA8
	public List<byte> TerrainHeight { get; set; } = new List<byte>();

	// Token: 0x17000285 RID: 645
	// (get) Token: 0x06001679 RID: 5753 RVA: 0x000828B1 File Offset: 0x00080AB1
	// (set) Token: 0x0600167A RID: 5754 RVA: 0x000828B9 File Offset: 0x00080AB9
	public List<byte> Height { get; set; } = new List<byte>();

	// Token: 0x17000286 RID: 646
	// (get) Token: 0x0600167B RID: 5755 RVA: 0x000828C2 File Offset: 0x00080AC2
	// (set) Token: 0x0600167C RID: 5756 RVA: 0x000828CA File Offset: 0x00080ACA
	public List<byte> TopSoil { get; set; } = new List<byte>();

	// Token: 0x17000287 RID: 647
	// (get) Token: 0x0600167D RID: 5757 RVA: 0x000828D3 File Offset: 0x00080AD3
	// (set) Token: 0x0600167E RID: 5758 RVA: 0x000828DB File Offset: 0x00080ADB
	public List<uint> BlockRaw { get; set; } = new List<uint>();

	// Token: 0x17000288 RID: 648
	// (get) Token: 0x0600167F RID: 5759 RVA: 0x000828E4 File Offset: 0x00080AE4
	// (set) Token: 0x06001680 RID: 5760 RVA: 0x000828EC File Offset: 0x00080AEC
	public List<sbyte> Densities { get; set; } = new List<sbyte>();

	// Token: 0x17000289 RID: 649
	// (get) Token: 0x06001681 RID: 5761 RVA: 0x000828F5 File Offset: 0x00080AF5
	// (set) Token: 0x06001682 RID: 5762 RVA: 0x000828FD File Offset: 0x00080AFD
	public List<long> Textures { get; set; } = new List<long>();

	// Token: 0x1700028A RID: 650
	// (get) Token: 0x06001683 RID: 5763 RVA: 0x00082906 File Offset: 0x00080B06
	public int TotalBlocks
	{
		get
		{
			return Math.Min((this.EndY - this.OffsetY + 1) * 256, this.BlockRaw.Count);
		}
	}

	// Token: 0x06001684 RID: 5764 RVA: 0x0008292D File Offset: 0x00080B2D
	public DynamicMeshChunkData.ChunkNeighbourData GetNeighbourData(int x, int z)
	{
		return this._neighbours[x + 1, z + 1];
	}

	// Token: 0x06001685 RID: 5765 RVA: 0x00082940 File Offset: 0x00080B40
	public void SetNeighbourData(int x, int z, DynamicMeshChunkData.ChunkNeighbourData data)
	{
		this._neighbours[x + 1, z + 1] = data;
	}

	// Token: 0x06001686 RID: 5766 RVA: 0x00082954 File Offset: 0x00080B54
	public void Copy(DynamicMeshChunkData other)
	{
		if (other == null)
		{
			return;
		}
		this.Reset();
		this.X = other.X;
		this.OffsetY = other.OffsetY;
		this.MinTerrainHeight = other.MinTerrainHeight;
		this.Z = other.Z;
		this.UpdateTime = other.UpdateTime;
		this.EndY = other.EndY;
		this.MainBiome = other.MainBiome;
		this.TerrainHeight.AddRange(other.TerrainHeight);
		this.Height.AddRange(other.Height);
		this.TopSoil.AddRange(other.TopSoil);
		int totalBlocks = other.TotalBlocks;
		for (int i = 0; i < totalBlocks; i++)
		{
			this.BlockRaw.Add(other.BlockRaw[i]);
		}
		this.Densities.AddRange(other.Densities);
		this.Textures.AddRange(other.Textures);
		for (int j = -1; j < 2; j++)
		{
			for (int k = -1; k < 2; k++)
			{
				if (j != 0 || k != 0)
				{
					this.GetNeighbourData(j, k).Copy(other.GetNeighbourData(j, k));
				}
			}
		}
	}

	// Token: 0x06001687 RID: 5767 RVA: 0x00082A74 File Offset: 0x00080C74
	public static DynamicMeshChunkData LoadFromStream(MemoryStream stream)
	{
		stream.Position = 0L;
		DynamicMeshChunkData fromCache = DynamicMeshChunkData.GetFromCache("_LoadStream_");
		using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
		{
			pooledBinaryReader.SetBaseStream(stream);
			fromCache.Read(pooledBinaryReader);
		}
		return fromCache;
	}

	// Token: 0x06001688 RID: 5768 RVA: 0x00082ACC File Offset: 0x00080CCC
	public void RecordCounts()
	{
		this.lastRaw = this.BlockRaw.Count;
		this.lastDen = this.Densities.Count;
		this.lastTex = this.Textures.Count;
	}

	// Token: 0x06001689 RID: 5769 RVA: 0x00082B04 File Offset: 0x00080D04
	public void ClearPreviousLayers()
	{
		if (this.lastRaw > 0)
		{
			this.BlockRaw.RemoveRange(0, this.lastRaw);
		}
		if (this.lastDen > 0)
		{
			this.Densities.RemoveRange(0, this.lastDen);
		}
		if (this.lastTex > 0)
		{
			this.Textures.RemoveRange(0, this.lastTex);
		}
	}

	// Token: 0x0600168A RID: 5770 RVA: 0x00082B64 File Offset: 0x00080D64
	public void Reset()
	{
		this.X = 0;
		this.OffsetY = 0;
		this.Z = 0;
		this.UpdateTime = 0;
		this.MainBiome = 0;
		this.EndY = 0;
		this.TerrainHeight.Clear();
		this.Height.Clear();
		this.TopSoil.Clear();
		this.BlockRaw.Clear();
		this.Densities.Clear();
		this.Textures.Clear();
		this.MinTerrainHeight = 500;
		this.GetNeighbourData(-1, -1).Clear();
		this.GetNeighbourData(-1, 0).Clear();
		this.GetNeighbourData(-1, 1).Clear();
		this.GetNeighbourData(1, 1).Clear();
		this.GetNeighbourData(1, 0).Clear();
		this.GetNeighbourData(1, -1).Clear();
		this.GetNeighbourData(0, -1).Clear();
		this.GetNeighbourData(0, 1).Clear();
	}

	// Token: 0x0600168B RID: 5771 RVA: 0x00082C50 File Offset: 0x00080E50
	public void SetTopSoil(byte[] soil)
	{
		this.TopSoil.AddRange(soil);
	}

	// Token: 0x0600168C RID: 5772 RVA: 0x00082C60 File Offset: 0x00080E60
	public int GetStreamSize()
	{
		int num = 81 + this.TerrainHeight.Count + this.Height.Count + this.TopSoil.Count + 4 * this.BlockRaw.Count + this.Densities.Count + 8 * this.Textures.Count;
		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				if (i != 0 || j != 0)
				{
					DynamicMeshChunkData.ChunkNeighbourData neighbourData = this.GetNeighbourData(i, j);
					num += 4 * neighbourData.BlockRaw.Count + neighbourData.Densities.Count + 8 * neighbourData.Textures.Count;
				}
			}
		}
		return num;
	}

	// Token: 0x0600168D RID: 5773 RVA: 0x00082D10 File Offset: 0x00080F10
	public void Write(BinaryWriter writer)
	{
		writer.Write(this.X);
		writer.Write(this.OffsetY);
		writer.Write(this.Z);
		writer.Write(this.EndY);
		writer.Write(this.MinTerrainHeight);
		writer.Write(this.UpdateTime);
		writer.Write(this.MainBiome);
		writer.Write(this.TerrainHeight.Count);
		foreach (byte value in this.TerrainHeight)
		{
			writer.Write(value);
		}
		writer.Write(this.Height.Count);
		foreach (byte value2 in this.Height)
		{
			writer.Write(value2);
		}
		writer.Write(this.TopSoil.Count);
		foreach (byte value3 in this.TopSoil)
		{
			writer.Write(value3);
		}
		int totalBlocks = this.TotalBlocks;
		writer.Write(totalBlocks);
		for (int i = 0; i < totalBlocks; i++)
		{
			writer.Write(this.BlockRaw[i]);
		}
		writer.Write(this.Densities.Count);
		foreach (sbyte value4 in this.Densities)
		{
			writer.Write(value4);
		}
		writer.Write(this.Textures.Count);
		foreach (long value5 in this.Textures)
		{
			writer.Write(value5);
		}
		for (int j = -1; j < 2; j++)
		{
			for (int k = -1; k < 2; k++)
			{
				if (j != 0 || k != 0)
				{
					this.GetNeighbourData(j, k).Write(writer);
				}
			}
		}
	}

	// Token: 0x0600168E RID: 5774 RVA: 0x00082F88 File Offset: 0x00081188
	public void Read(PooledBinaryReader reader)
	{
		this.X = reader.ReadInt32();
		this.OffsetY = reader.ReadInt32();
		this.Z = reader.ReadInt32();
		this.EndY = reader.ReadInt32();
		this.MinTerrainHeight = reader.ReadInt32();
		this.UpdateTime = reader.ReadInt32();
		this.MainBiome = reader.ReadByte();
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			this.TerrainHeight.Add(reader.ReadByte());
		}
		num = reader.ReadInt32();
		for (int j = 0; j < num; j++)
		{
			this.Height.Add(reader.ReadByte());
		}
		num = reader.ReadInt32();
		for (int k = 0; k < num; k++)
		{
			this.TopSoil.Add(reader.ReadByte());
		}
		num = reader.ReadInt32();
		for (int l = 0; l < num; l++)
		{
			this.BlockRaw.Add(reader.ReadUInt32());
		}
		num = reader.ReadInt32();
		for (int m = 0; m < num; m++)
		{
			this.Densities.Add(reader.ReadSByte());
		}
		num = reader.ReadInt32();
		for (int n = 0; n < num; n++)
		{
			this.Textures.Add(reader.ReadInt64());
		}
		for (int num2 = -1; num2 < 2; num2++)
		{
			for (int num3 = -1; num3 < 2; num3++)
			{
				if (num2 != 0 || num3 != 0)
				{
					this.GetNeighbourData(num2, num3).Read(reader);
				}
			}
		}
	}

	// Token: 0x0600168F RID: 5775 RVA: 0x00083108 File Offset: 0x00081308
	public void ApplyToChunk(Chunk chunk, ChunkCacheNeighborChunks cacheNeighbourChunks)
	{
		int index = 0;
		chunk.X = this.X;
		chunk.Z = this.Z;
		chunk.SetTopSoil(this.TopSoil);
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				chunk.SetHeight(i, j, this.Height[index]);
				chunk.SetTerrainHeight(i, j, this.TerrainHeight[index++]);
			}
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		BlockValue blockValue = default(BlockValue);
		for (int k = 0; k < 256; k++)
		{
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					chunk.SetLight(i, k, j, 15, Chunk.LIGHT_TYPE.SUN);
					if (k < this.OffsetY - 1 || k >= this.EndY)
					{
						chunk.SetBlockRaw(i, k, j, BlockValue.Air);
						chunk.SetDensity(i, k, j, MarchingCubes.DensityAir);
						chunk.SetTextureFull(i, k, j, 0L, 0);
					}
					else
					{
						blockValue.rawData = this.BlockRaw[num];
						bool flag;
						if (flag = (DynamicMeshSettings.UseImposterValues && DynamicMeshBlockSwap.BlockSwaps.TryGetValue(blockValue.type, out num3)))
						{
							if (num3 == 0)
							{
								blockValue.rawData = 0U;
								num2++;
							}
							else
							{
								blockValue.type = num3;
							}
						}
						if (blockValue.rawData == 0U)
						{
							chunk.SetBlockRaw(i, k, j, BlockValue.Air);
							chunk.SetDensity(i, k, j, MarchingCubes.DensityAir);
							chunk.SetTextureFull(i, k, j, 0L, 0);
						}
						else
						{
							long num4 = Block.list[blockValue.type].shape.IsTerrain() ? 0L : this.Textures[num2];
							if (flag)
							{
								long num5;
								DynamicMeshBlockSwap.TextureSwaps.TryGetValue(num3, out num5);
								if (num4 == 0L && num5 != 0L)
								{
									num4 = (num5 | num5 << 8 | num5 << 16 | num5 << 24 | num5 << 32 | num5 << 40);
								}
							}
							chunk.SetBlockRaw(i, k, j, blockValue);
							chunk.SetDensity(i, k, j, this.Densities[num2]);
							chunk.SetTextureFull(i, k, j, num4, 0);
							num2++;
						}
						num++;
					}
				}
			}
		}
		this.SetXNeighbour(15, (Chunk)cacheNeighbourChunks[-1, 0], this.GetNeighbourData(-1, 0));
		this.SetXNeighbour(0, (Chunk)cacheNeighbourChunks[1, 0], this.GetNeighbourData(1, 0));
		this.SetZNeighbour(15, (Chunk)cacheNeighbourChunks[0, -1], this.GetNeighbourData(0, -1));
		this.SetZNeighbour(0, (Chunk)cacheNeighbourChunks[0, 1], this.GetNeighbourData(0, 1));
		this.SetNeighbourCorner(15, 15, (Chunk)cacheNeighbourChunks[-1, -1], this.GetNeighbourData(-1, -1));
		this.SetNeighbourCorner(0, 15, (Chunk)cacheNeighbourChunks[1, -1], this.GetNeighbourData(1, -1));
		this.SetNeighbourCorner(15, 0, (Chunk)cacheNeighbourChunks[-1, 1], this.GetNeighbourData(-1, 1));
		this.SetNeighbourCorner(0, 0, (Chunk)cacheNeighbourChunks[1, 1], this.GetNeighbourData(1, 1));
	}

	// Token: 0x06001690 RID: 5776 RVA: 0x00083434 File Offset: 0x00081634
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetNeighbourCorner(int x, int z, Chunk chunk, DynamicMeshChunkData.ChunkNeighbourData data)
	{
		BlockValue blockValue = new BlockValue(0U);
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < 256; i++)
		{
			blockValue.rawData = data.BlockRaw[num];
			num++;
			chunk.SetBlockRaw(x, i, z, blockValue);
			if (blockValue.rawData != 0U)
			{
				chunk.SetDensity(x, i, z, data.Densities[num2]);
				chunk.SetTextureFull(x, i, z, (long)data.Densities[num2], 0);
				num2++;
			}
			else
			{
				chunk.SetDensity(x, i, z, MarchingCubes.DensityAir);
				chunk.SetTextureFull(x, i, z, 0L, 0);
			}
		}
	}

	// Token: 0x06001691 RID: 5777 RVA: 0x000834DC File Offset: 0x000816DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetXNeighbour(int x, Chunk chunk, DynamicMeshChunkData.ChunkNeighbourData data)
	{
		BlockValue blockValue = new BlockValue(0U);
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				blockValue.rawData = data.BlockRaw[num];
				num++;
				chunk.SetBlockRaw(x, i, j, blockValue);
				if (blockValue.rawData != 0U)
				{
					chunk.SetDensity(x, i, j, data.Densities[num2]);
					chunk.SetTextureFull(x, i, j, (long)data.Densities[num2], 0);
					num2++;
				}
				else
				{
					chunk.SetDensity(x, i, j, MarchingCubes.DensityAir);
					chunk.SetTextureFull(x, i, j, 0L, 0);
				}
			}
		}
	}

	// Token: 0x06001692 RID: 5778 RVA: 0x000835A0 File Offset: 0x000817A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetZNeighbour(int z, Chunk chunk, DynamicMeshChunkData.ChunkNeighbourData data)
	{
		BlockValue blockValue = new BlockValue(0U);
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				blockValue.rawData = data.BlockRaw[num];
				num++;
				chunk.SetBlockRaw(j, i, z, blockValue);
				if (blockValue.rawData != 0U)
				{
					chunk.SetDensity(j, i, z, data.Densities[num2]);
					chunk.SetTextureFull(j, i, z, (long)data.Densities[num2], 0);
					num2++;
				}
				else
				{
					chunk.SetDensity(j, i, z, MarchingCubes.DensityAir);
					chunk.SetTextureFull(j, i, z, 0L, 0);
				}
			}
		}
	}

	// Token: 0x06001693 RID: 5779 RVA: 0x00083664 File Offset: 0x00081864
	public static DynamicMeshChunkData GetFromCache(string debug)
	{
		DynamicMeshChunkData result;
		if (!DynamicMeshChunkData.Cache.TryDequeue(out result))
		{
			result = DynamicMeshChunkData.Creates();
		}
		DynamicMeshChunkData.ActiveDataItems++;
		return result;
	}

	// Token: 0x06001694 RID: 5780 RVA: 0x00083692 File Offset: 0x00081892
	public static void AddToCache(DynamicMeshChunkData data, string debug)
	{
		data.Reset();
		DynamicMeshChunkData.Cache.Enqueue(data);
		DynamicMeshChunkData.ActiveDataItems--;
	}

	// Token: 0x06001695 RID: 5781 RVA: 0x000836B4 File Offset: 0x000818B4
	public static DynamicMeshChunkData Creates()
	{
		DynamicMeshChunkData dynamicMeshChunkData = new DynamicMeshChunkData
		{
			BlockRaw = new List<uint>(),
			Densities = new List<sbyte>(),
			Textures = new List<long>(),
			TopSoil = new List<byte>(32),
			Height = new List<byte>(256),
			TerrainHeight = new List<byte>(256)
		};
		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				if (i != 0 || j != 0)
				{
					dynamicMeshChunkData.SetNeighbourData(i, j, DynamicMeshChunkData.ChunkNeighbourData.Create());
				}
			}
		}
		return dynamicMeshChunkData;
	}

	// Token: 0x04000E2F RID: 3631
	public int MinTerrainHeight;

	// Token: 0x04000E30 RID: 3632
	[PublicizedFrom(EAccessModifier.Private)]
	public DynamicMeshChunkData.ChunkNeighbourData[,] _neighbours = new DynamicMeshChunkData.ChunkNeighbourData[3, 3];

	// Token: 0x04000E31 RID: 3633
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastRaw;

	// Token: 0x04000E32 RID: 3634
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastDen;

	// Token: 0x04000E33 RID: 3635
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastTex;

	// Token: 0x04000E34 RID: 3636
	public static int ActiveDataItems = 0;

	// Token: 0x04000E35 RID: 3637
	public static ConcurrentQueue<DynamicMeshChunkData> Cache = new ConcurrentQueue<DynamicMeshChunkData>();

	// Token: 0x02000316 RID: 790
	public class ChunkNeighbourData
	{
		// Token: 0x1700028B RID: 651
		// (get) Token: 0x06001698 RID: 5784 RVA: 0x000837B8 File Offset: 0x000819B8
		// (set) Token: 0x06001699 RID: 5785 RVA: 0x000837C0 File Offset: 0x000819C0
		public List<uint> BlockRaw { get; set; }

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x0600169A RID: 5786 RVA: 0x000837C9 File Offset: 0x000819C9
		// (set) Token: 0x0600169B RID: 5787 RVA: 0x000837D1 File Offset: 0x000819D1
		public List<sbyte> Densities { get; set; }

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x0600169C RID: 5788 RVA: 0x000837DA File Offset: 0x000819DA
		// (set) Token: 0x0600169D RID: 5789 RVA: 0x000837E2 File Offset: 0x000819E2
		public List<long> Textures { get; set; }

		// Token: 0x0600169E RID: 5790 RVA: 0x000837EB File Offset: 0x000819EB
		public void Clear()
		{
			this.BlockRaw.Clear();
			this.Densities.Clear();
			this.Textures.Clear();
		}

		// Token: 0x0600169F RID: 5791 RVA: 0x0008380E File Offset: 0x00081A0E
		public void SetData(uint blockraw, sbyte density, long texture)
		{
			this.BlockRaw.Add(blockraw);
			if (blockraw == 0U)
			{
				return;
			}
			this.Densities.Add(density);
			this.Textures.Add(texture);
		}

		// Token: 0x060016A0 RID: 5792 RVA: 0x00083838 File Offset: 0x00081A38
		public void Copy(DynamicMeshChunkData.ChunkNeighbourData other)
		{
			this.BlockRaw.Clear();
			this.Densities.Clear();
			this.Textures.Clear();
			this.BlockRaw.AddRange(other.BlockRaw);
			this.Densities.AddRange(other.Densities);
			this.Textures.AddRange(other.Textures);
		}

		// Token: 0x060016A1 RID: 5793 RVA: 0x0008389C File Offset: 0x00081A9C
		public void Write(BinaryWriter writer)
		{
			writer.Write(this.BlockRaw.Count);
			foreach (uint value in this.BlockRaw)
			{
				writer.Write(value);
			}
			writer.Write(this.Densities.Count);
			foreach (sbyte value2 in this.Densities)
			{
				writer.Write(value2);
			}
			writer.Write(this.Textures.Count);
			foreach (long value3 in this.Textures)
			{
				writer.Write(value3);
			}
		}

		// Token: 0x060016A2 RID: 5794 RVA: 0x000839AC File Offset: 0x00081BAC
		public void Read(PooledBinaryReader reader)
		{
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				this.BlockRaw.Add(reader.ReadUInt32());
			}
			num = reader.ReadInt32();
			for (int j = 0; j < num; j++)
			{
				this.Densities.Add(reader.ReadSByte());
			}
			num = reader.ReadInt32();
			for (int k = 0; k < num; k++)
			{
				this.Textures.Add(reader.ReadInt64());
			}
		}

		// Token: 0x060016A3 RID: 5795 RVA: 0x00083A25 File Offset: 0x00081C25
		public static DynamicMeshChunkData.ChunkNeighbourData Create()
		{
			return new DynamicMeshChunkData.ChunkNeighbourData
			{
				BlockRaw = new List<uint>(),
				Densities = new List<sbyte>(),
				Textures = new List<long>()
			};
		}

		// Token: 0x060016A4 RID: 5796 RVA: 0x00083A4D File Offset: 0x00081C4D
		public static DynamicMeshChunkData.ChunkNeighbourData CreateMax()
		{
			return new DynamicMeshChunkData.ChunkNeighbourData
			{
				BlockRaw = new List<uint>(65280),
				Densities = new List<sbyte>(65280),
				Textures = new List<long>(65280)
			};
		}

		// Token: 0x060016A5 RID: 5797 RVA: 0x00083A84 File Offset: 0x00081C84
		public static DynamicMeshChunkData.ChunkNeighbourData CreateCorner()
		{
			return new DynamicMeshChunkData.ChunkNeighbourData
			{
				BlockRaw = new List<uint>(255),
				Densities = new List<sbyte>(255),
				Textures = new List<long>(255)
			};
		}
	}
}
