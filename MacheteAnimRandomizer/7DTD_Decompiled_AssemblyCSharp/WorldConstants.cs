using System;
using UnityEngine;

// Token: 0x02000A93 RID: 2707
public class WorldConstants
{
	// Token: 0x06005398 RID: 21400 RVA: 0x002182D0 File Offset: 0x002164D0
	public static Rect MapBlockToUVRect(int _meshIndex, BlockValue _blockValue, BlockFace _blockFace)
	{
		int sideTextureId = _blockValue.Block.GetSideTextureId(_blockValue, _blockFace, 0);
		if (sideTextureId >= 0 && sideTextureId < MeshDescription.meshes[_meshIndex].textureAtlas.uvMapping.Length)
		{
			return MeshDescription.meshes[_meshIndex].textureAtlas.uvMapping[sideTextureId].uv;
		}
		return WorldConstants.uvRectZero;
	}

	// Token: 0x06005399 RID: 21401 RVA: 0x0021832C File Offset: 0x0021652C
	public static Rect MapDamageToUVRect(BlockValue _blockValue)
	{
		if (_blockValue.hasdecal || _blockValue.ischild || _blockValue.damage == 0)
		{
			return WorldConstants.uvRectZero;
		}
		return WorldConstants.rectCracks[Mathf.Min((int)((float)_blockValue.damage * 10f / (float)_blockValue.Block.MaxDamage), WorldConstants.rectCracks.Length - 1)];
	}

	// Token: 0x04003FAD RID: 16301
	public const int ChunkBlockXPow = 4;

	// Token: 0x04003FAE RID: 16302
	public const int ChunkBlockYPow = 8;

	// Token: 0x04003FAF RID: 16303
	public const int ChunkBlockZPow = 4;

	// Token: 0x04003FB0 RID: 16304
	public const int ChunkBlockLayerHeight = 4;

	// Token: 0x04003FB1 RID: 16305
	public const int ChunkBlockLayerHeightPow = 2;

	// Token: 0x04003FB2 RID: 16306
	public const int ChunkBlockLayerHeightMask = 3;

	// Token: 0x04003FB3 RID: 16307
	public const int ChunkBlockLayers = 64;

	// Token: 0x04003FB4 RID: 16308
	public const int ChunkBlockXDim = 16;

	// Token: 0x04003FB5 RID: 16309
	public const int ChunkBlockYDim = 256;

	// Token: 0x04003FB6 RID: 16310
	public const int ChunkBlockZDim = 16;

	// Token: 0x04003FB7 RID: 16311
	public const int ChunkBlockXDimM1 = 15;

	// Token: 0x04003FB8 RID: 16312
	public const int ChunkBlockYDimM1 = 255;

	// Token: 0x04003FB9 RID: 16313
	public const int ChunkBlockZDimM1 = 15;

	// Token: 0x04003FBA RID: 16314
	public const int ChunkAreaDim = 256;

	// Token: 0x04003FBB RID: 16315
	public const int ChunkVolumeDim = 65536;

	// Token: 0x04003FBC RID: 16316
	public const int ChunkBlockXMask = 15;

	// Token: 0x04003FBD RID: 16317
	public const int ChunkBlockYMask = 255;

	// Token: 0x04003FBE RID: 16318
	public const int ChunkBlockZMask = 15;

	// Token: 0x04003FBF RID: 16319
	public const int ChunkMeshLayerHeight = 16;

	// Token: 0x04003FC0 RID: 16320
	public const int ChunkMeshLayerShift = 4;

	// Token: 0x04003FC1 RID: 16321
	public const int ChunkMeshLayerHeightMask = 65535;

	// Token: 0x04003FC2 RID: 16322
	public const int ChunkDensityXPow = 4;

	// Token: 0x04003FC3 RID: 16323
	public const int ChunkDensityYPow = 8;

	// Token: 0x04003FC4 RID: 16324
	public const int ChunkDensityZPow = 4;

	// Token: 0x04003FC5 RID: 16325
	public const int ChunkDensityXDim = 16;

	// Token: 0x04003FC6 RID: 16326
	public const int ChunkDensityYDim = 256;

	// Token: 0x04003FC7 RID: 16327
	public const int ChunkDensityZDim = 16;

	// Token: 0x04003FC8 RID: 16328
	public const int ChunkDensityXMask = 15;

	// Token: 0x04003FC9 RID: 16329
	public const int ChunkDensityYMask = 255;

	// Token: 0x04003FCA RID: 16330
	public const int ChunkDensityZMask = 15;

	// Token: 0x04003FCB RID: 16331
	public const int NumberOfLightShades = 16;

	// Token: 0x04003FCC RID: 16332
	public const int MaxDarkness = 16;

	// Token: 0x04003FCD RID: 16333
	public const int cTimePerHour = 1000;

	// Token: 0x04003FCE RID: 16334
	public const int cTimePerDay = 24000;

	// Token: 0x04003FCF RID: 16335
	public const int cDuskHour = 22;

	// Token: 0x04003FD0 RID: 16336
	public static float WaterLevel = Block.cWaterLevel;

	// Token: 0x04003FD1 RID: 16337
	public static Rect uvRectZero = new Rect(0f, 0f, 0f, 0f);

	// Token: 0x04003FD2 RID: 16338
	[PublicizedFrom(EAccessModifier.Private)]
	public static Rect[] rectCracks = new Rect[]
	{
		new Rect(0f, 0f, 0.1f, 1f),
		new Rect(0.1f, 0f, 0.1f, 1f),
		new Rect(0.2f, 0f, 0.1f, 1f),
		new Rect(0.3f, 0f, 0.1f, 1f),
		new Rect(0.4f, 0f, 0.1f, 1f),
		new Rect(0.5f, 0f, 0.1f, 1f),
		new Rect(0.6f, 0f, 0.1f, 1f),
		new Rect(0.7f, 0f, 0.1f, 1f),
		new Rect(0.8f, 0f, 0.1f, 1f),
		new Rect(0.9f, 0f, 0.1f, 1f)
	};
}
