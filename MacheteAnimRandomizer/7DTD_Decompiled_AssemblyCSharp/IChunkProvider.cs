using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020009DC RID: 2524
public interface IChunkProvider
{
	// Token: 0x06004D40 RID: 19776
	IEnumerator Init(World _worldData);

	// Token: 0x06004D41 RID: 19777
	void Update();

	// Token: 0x06004D42 RID: 19778
	void StopUpdate();

	// Token: 0x06004D43 RID: 19779
	void Cleanup();

	// Token: 0x06004D44 RID: 19780
	void RequestChunk(int _x, int _y);

	// Token: 0x06004D45 RID: 19781
	HashSetList<long> GetRequestedChunks();

	// Token: 0x06004D46 RID: 19782
	void SaveAll();

	// Token: 0x06004D47 RID: 19783
	void SaveRandomChunks(int count, ulong _curWorldTimeInTicks, ArraySegment<long> _activeChunkSet);

	// Token: 0x06004D48 RID: 19784
	void ReloadAllChunks();

	// Token: 0x06004D49 RID: 19785
	void ClearCaches();

	// Token: 0x06004D4A RID: 19786
	EnumChunkProviderId GetProviderId();

	// Token: 0x06004D4B RID: 19787
	void UnloadChunk(Chunk _chunk);

	// Token: 0x06004D4C RID: 19788
	DynamicPrefabDecorator GetDynamicPrefabDecorator();

	// Token: 0x06004D4D RID: 19789
	SpawnPointList GetSpawnPointList();

	// Token: 0x06004D4E RID: 19790
	void SetSpawnPointList(SpawnPointList _spawnPointList);

	// Token: 0x06004D4F RID: 19791
	bool GetOverviewMap(Vector2i _startPos, Vector2i _size, Color[] mapColors);

	// Token: 0x06004D50 RID: 19792
	void SetDecorationsEnabled(bool _bEnable);

	// Token: 0x06004D51 RID: 19793
	bool IsDecorationsEnabled();

	// Token: 0x06004D52 RID: 19794
	bool GetWorldExtent(out Vector3i _minSize, out Vector3i _maxSize);

	// Token: 0x06004D53 RID: 19795
	BoundsInt GetWorldBounds();

	// Token: 0x06004D54 RID: 19796
	Vector2i GetWorldSize();

	// Token: 0x06004D55 RID: 19797
	IBiomeProvider GetBiomeProvider();

	// Token: 0x06004D56 RID: 19798
	ITerrainGenerator GetTerrainGenerator();

	// Token: 0x06004D57 RID: 19799
	int GetPOIBlockIdOverride(int x, int z);

	// Token: 0x06004D58 RID: 19800
	float GetPOIHeightOverride(int x, int z);

	// Token: 0x06004D59 RID: 19801
	IEnumerator FillOccupiedMap(int xStart, int zStart, DecoOccupiedMap occupiedMap, List<PrefabInstance> overridePOIList = null);

	// Token: 0x06004D5A RID: 19802
	void RebuildTerrain(HashSetLong _chunks, Vector3i _areaStart, Vector3i _areaSize, bool _isStopStabilityCalc, bool _isRegenChunk, bool _isFillEmptyBlocks, bool _isReset);

	// Token: 0x06004D5B RID: 19803
	ChunkProtectionLevel GetChunkProtectionLevel(Vector3i worldPos);

	// Token: 0x170007F5 RID: 2037
	// (get) Token: 0x06004D5C RID: 19804
	GameUtils.WorldInfo WorldInfo { get; }
}
