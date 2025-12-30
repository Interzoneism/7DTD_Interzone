using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B82 RID: 2946
public class FlatArea
{
	// Token: 0x17000950 RID: 2384
	// (get) Token: 0x06005B4A RID: 23370 RVA: 0x00248C2E File Offset: 0x00246E2E
	public int Height
	{
		get
		{
			return this.position.y;
		}
	}

	// Token: 0x17000951 RID: 2385
	// (get) Token: 0x06005B4B RID: 23371 RVA: 0x00248C3B File Offset: 0x00246E3B
	public int MinX
	{
		get
		{
			return this.position.x;
		}
	}

	// Token: 0x17000952 RID: 2386
	// (get) Token: 0x06005B4C RID: 23372 RVA: 0x00248C48 File Offset: 0x00246E48
	public int MaxX
	{
		get
		{
			return this.position.x + this.size - 1;
		}
	}

	// Token: 0x17000953 RID: 2387
	// (get) Token: 0x06005B4D RID: 23373 RVA: 0x00248C5E File Offset: 0x00246E5E
	public int MinZ
	{
		get
		{
			return this.position.z;
		}
	}

	// Token: 0x17000954 RID: 2388
	// (get) Token: 0x06005B4E RID: 23374 RVA: 0x00248C6B File Offset: 0x00246E6B
	public int MaxZ
	{
		get
		{
			return this.position.z + this.size - 1;
		}
	}

	// Token: 0x17000955 RID: 2389
	// (get) Token: 0x06005B4F RID: 23375 RVA: 0x00248C81 File Offset: 0x00246E81
	public Vector3 Center
	{
		get
		{
			return this.position + new Vector3((float)(this.size / 2), 0f, (float)(this.size / 2));
		}
	}

	// Token: 0x06005B50 RID: 23376 RVA: 0x00248CAF File Offset: 0x00246EAF
	public FlatArea(Vector3i _position, int _size)
	{
		this.position = _position;
		this.size = _size;
	}

	// Token: 0x06005B51 RID: 23377 RVA: 0x00248CC8 File Offset: 0x00246EC8
	public bool IsValid(World world, BiomeFilterTypes biomeFilter = BiomeFilterTypes.AnyBiome, string[] biomeNames = null, ChunkProtectionLevel maxAllowedChunkProtectionLevel = ChunkProtectionLevel.NearLandClaim)
	{
		this.maxChunkProtectionLevel = ChunkProtectionLevel.None;
		int num = this.MinX >> 4 << 4;
		int num2 = this.MaxX >> 4 << 4;
		int num3 = this.MinZ >> 4 << 4;
		int num4 = this.MaxZ >> 4 << 4;
		for (int i = num; i <= num2; i += 16)
		{
			for (int j = num3; j <= num4; j += 16)
			{
				ChunkProtectionLevel chunkProtectionLevel = world.ChunkCache.ChunkProvider.GetChunkProtectionLevel(new Vector3i(i, this.position.y, j));
				if (chunkProtectionLevel > this.maxChunkProtectionLevel)
				{
					this.maxChunkProtectionLevel = chunkProtectionLevel;
				}
			}
		}
		if (this.maxChunkProtectionLevel > maxAllowedChunkProtectionLevel)
		{
			return false;
		}
		if (biomeFilter != BiomeFilterTypes.AnyBiome)
		{
			BiomeDefinition biomeInWorld = GameManager.Instance.World.GetBiomeInWorld((int)this.Center.x, (int)this.Center.z);
			if (biomeInWorld == null)
			{
				return false;
			}
			if (biomeFilter == BiomeFilterTypes.OnlyBiome)
			{
				if (biomeInWorld.m_sBiomeName != biomeNames[0])
				{
					return false;
				}
			}
			else if (biomeFilter == BiomeFilterTypes.ExcludeBiome)
			{
				bool flag = false;
				for (int k = 0; k < biomeNames.Length; k++)
				{
					if (biomeInWorld.m_sBiomeName == biomeNames[k])
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					return false;
				}
			}
			else if (biomeFilter == BiomeFilterTypes.SameBiome && biomeInWorld.m_sBiomeName != biomeNames[0])
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06005B52 RID: 23378 RVA: 0x00248E01 File Offset: 0x00247001
	public bool IsInArea(int _x, int _z)
	{
		return _x >= this.MinX && _x <= this.MaxX && _z >= this.MinZ && _z <= this.MaxZ;
	}

	// Token: 0x06005B53 RID: 23379 RVA: 0x00248E2C File Offset: 0x0024702C
	public List<Vector2i> GetPositions()
	{
		List<Vector2i> list = new List<Vector2i>();
		for (int i = this.MinX; i <= this.MaxX; i++)
		{
			for (int j = this.MinZ; j <= this.MaxZ; j++)
			{
				list.Add(new Vector2i(i, j));
			}
		}
		return list;
	}

	// Token: 0x06005B54 RID: 23380 RVA: 0x00248E7C File Offset: 0x0024707C
	public Vector3 GetRandomPosition(float margin = 0f)
	{
		return new Vector3(UnityEngine.Random.Range((float)this.MinX + margin, (float)this.MaxX - margin + 1f), (float)this.Height, UnityEngine.Random.Range((float)this.MinZ + margin, (float)this.MaxZ - margin + 1f));
	}

	// Token: 0x06005B55 RID: 23381 RVA: 0x00248ED0 File Offset: 0x002470D0
	public override bool Equals(object obj)
	{
		FlatArea flatArea = obj as FlatArea;
		return flatArea != null && this.position == flatArea.position && this.size == flatArea.size;
	}

	// Token: 0x06005B56 RID: 23382 RVA: 0x0000FEB7 File Offset: 0x0000E0B7
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	// Token: 0x040045D4 RID: 17876
	public Vector3i position;

	// Token: 0x040045D5 RID: 17877
	public int size;

	// Token: 0x040045D6 RID: 17878
	public ChunkProtectionLevel maxChunkProtectionLevel;
}
