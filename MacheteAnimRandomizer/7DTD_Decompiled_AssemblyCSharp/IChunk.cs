using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000997 RID: 2455
public interface IChunk
{
	// Token: 0x170007C2 RID: 1986
	// (get) Token: 0x060049E2 RID: 18914
	// (set) Token: 0x060049E3 RID: 18915
	int X { get; set; }

	// Token: 0x170007C3 RID: 1987
	// (get) Token: 0x060049E4 RID: 18916
	int Y { get; }

	// Token: 0x170007C4 RID: 1988
	// (get) Token: 0x060049E5 RID: 18917
	// (set) Token: 0x060049E6 RID: 18918
	int Z { get; set; }

	// Token: 0x170007C5 RID: 1989
	// (get) Token: 0x060049E7 RID: 18919
	// (set) Token: 0x060049E8 RID: 18920
	Vector3i ChunkPos { get; set; }

	// Token: 0x060049E9 RID: 18921
	bool GetAvailable();

	// Token: 0x060049EA RID: 18922
	BlockValue GetBlock(int _x, int _y, int _z);

	// Token: 0x060049EB RID: 18923
	BlockValue GetBlockNoDamage(int _x, int _y, int _z);

	// Token: 0x060049EC RID: 18924
	bool IsOnlyTerrain(int _y);

	// Token: 0x060049ED RID: 18925
	bool IsOnlyTerrainLayer(int _idx);

	// Token: 0x060049EE RID: 18926
	bool IsEmpty();

	// Token: 0x060049EF RID: 18927
	bool IsEmpty(int _y);

	// Token: 0x060049F0 RID: 18928
	bool IsEmptyLayer(int _idx);

	// Token: 0x060049F1 RID: 18929
	byte GetStability(int _x, int _y, int _z);

	// Token: 0x060049F2 RID: 18930
	void SetStability(int _x, int _y, int _z, byte _v);

	// Token: 0x060049F3 RID: 18931
	byte GetLight(int x, int y, int z, Chunk.LIGHT_TYPE type);

	// Token: 0x060049F4 RID: 18932
	int GetLightValue(int x, int y, int z, int _darknessV);

	// Token: 0x060049F5 RID: 18933
	float GetLightBrightness(int x, int y, int z, int _ss);

	// Token: 0x060049F6 RID: 18934
	Vector3i GetWorldPos();

	// Token: 0x060049F7 RID: 18935
	byte GetHeight(int _blockOffset);

	// Token: 0x060049F8 RID: 18936
	byte GetHeight(int _x, int _z);

	// Token: 0x060049F9 RID: 18937
	sbyte GetDensity(int _x, int _y, int _z);

	// Token: 0x060049FA RID: 18938
	bool HasSameDensityValue(int _y);

	// Token: 0x060049FB RID: 18939
	sbyte GetSameDensityValue(int _y);

	// Token: 0x060049FC RID: 18940
	int GetBlockFaceTexture(int _x, int _y, int _z, BlockFace _blockFace, int channel);

	// Token: 0x060049FD RID: 18941
	long GetTextureFull(int _x, int _y, int _z, int channel = 0);

	// Token: 0x060049FE RID: 18942 RVA: 0x001D3DE8 File Offset: 0x001D1FE8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	TextureFullArray GetTextureFullArray(int _x, int _y, int _z)
	{
		TextureFullArray result;
		for (int i = 0; i < 1; i++)
		{
			result[i] = this.GetTextureFull(_x, _y, _z, i);
		}
		return result;
	}

	// Token: 0x060049FF RID: 18943
	BlockEntityData GetBlockEntity(Vector3i _blockPos);

	// Token: 0x06004A00 RID: 18944
	BlockEntityData GetBlockEntity(Transform _transform);

	// Token: 0x06004A01 RID: 18945
	void SetTopSoilBroken(int _x, int _z);

	// Token: 0x06004A02 RID: 18946
	bool IsTopSoil(int _x, int _z);

	// Token: 0x06004A03 RID: 18947
	byte GetTerrainHeight(int _x, int _z);

	// Token: 0x06004A04 RID: 18948
	WaterValue GetWater(int _x, int _y, int _z);

	// Token: 0x06004A05 RID: 18949
	bool IsWater(int _x, int _y, int _z);

	// Token: 0x06004A06 RID: 18950
	bool IsAir(int _x, int _y, int _z);
}
