using System;
using UnityEngine;

// Token: 0x020005A7 RID: 1447
public class MarchingCubes : IMarchingCubes
{
	// Token: 0x06002EA2 RID: 11938 RVA: 0x00002914 File Offset: 0x00000B14
	public void Polygonize(INeighborBlockCache _nBlocks, Vector3i _localPos, Vector3 _offsetPos, byte _sunLight, byte _blockLight, VoxelMesh _mesh)
	{
	}

	// Token: 0x06002EA3 RID: 11939 RVA: 0x001323D4 File Offset: 0x001305D4
	public static float GetDecorationOffsetY(sbyte _densY, sbyte _densYm1)
	{
		float num = (float)(_densY + _densYm1);
		return Utils.FastClamp(-0.0035f * num, -0.4f, 0.4f);
	}

	// Token: 0x040024AD RID: 9389
	public static readonly sbyte DensityAir = sbyte.MaxValue;

	// Token: 0x040024AE RID: 9390
	public static readonly sbyte DensityAirHi = 100;

	// Token: 0x040024AF RID: 9391
	public static readonly sbyte DensityTerrain = sbyte.MinValue;

	// Token: 0x040024B0 RID: 9392
	public static readonly sbyte DensityTerrainHi = -100;
}
