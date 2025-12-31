using System;
using UnityEngine;

// Token: 0x02000AD0 RID: 2768
public interface ITerrainGenerator
{
	// Token: 0x06005539 RID: 21817
	void Init(World _world, IBiomeProvider _biomeProvider, int _seed);

	// Token: 0x0600553A RID: 21818
	void GenerateTerrain(World worldData, Chunk chunk, GameRandom _random);

	// Token: 0x0600553B RID: 21819
	void GenerateTerrain(World worldData, Chunk chunk, GameRandom _random, Vector3i _areaStart, Vector3i _areaSize, bool _bFillEmptyBlocks, bool _isReset);

	// Token: 0x0600553C RID: 21820
	byte GetTerrainHeightByteAt(int _xWorld, int _zWorld);

	// Token: 0x0600553D RID: 21821
	sbyte GetDensityAt(int _xWorld, int _yWorld, int _zWorld, BiomeDefinition _bd, float _biomeIntensity);

	// Token: 0x0600553E RID: 21822
	Vector3 GetTerrainNormalAt(int _xWorld, int _zWorld, BiomeDefinition _bd, float _biomeIntensity);

	// Token: 0x0600553F RID: 21823
	float GetTerrainHeightAt(int _xWorld, int _zWorld);
}
