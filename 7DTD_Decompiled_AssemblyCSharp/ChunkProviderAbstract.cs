using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020009DD RID: 2525
public abstract class ChunkProviderAbstract : IChunkProvider
{
	// Token: 0x06004D5D RID: 19805 RVA: 0x001EA357 File Offset: 0x001E8557
	public virtual IEnumerator Init(World _worldData)
	{
		yield return null;
		yield break;
	}

	// Token: 0x06004D5E RID: 19806 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Update()
	{
	}

	// Token: 0x06004D5F RID: 19807 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void StopUpdate()
	{
	}

	// Token: 0x06004D60 RID: 19808 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Cleanup()
	{
	}

	// Token: 0x06004D61 RID: 19809 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void RequestChunk(int _x, int _y)
	{
	}

	// Token: 0x06004D62 RID: 19810 RVA: 0x00019766 File Offset: 0x00017966
	public virtual HashSetList<long> GetRequestedChunks()
	{
		return null;
	}

	// Token: 0x06004D63 RID: 19811 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SaveAll()
	{
	}

	// Token: 0x06004D64 RID: 19812 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SaveRandomChunks(int count, ulong _curWorldTimeInTicks, ArraySegment<long> _activeChunkSet)
	{
	}

	// Token: 0x06004D65 RID: 19813 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void ReloadAllChunks()
	{
	}

	// Token: 0x06004D66 RID: 19814 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void ClearCaches()
	{
	}

	// Token: 0x06004D67 RID: 19815 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual EnumChunkProviderId GetProviderId()
	{
		return EnumChunkProviderId.None;
	}

	// Token: 0x06004D68 RID: 19816 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void UnloadChunk(Chunk _chunk)
	{
	}

	// Token: 0x06004D69 RID: 19817 RVA: 0x00019766 File Offset: 0x00017966
	public virtual DynamicPrefabDecorator GetDynamicPrefabDecorator()
	{
		return null;
	}

	// Token: 0x06004D6A RID: 19818 RVA: 0x00019766 File Offset: 0x00017966
	public virtual SpawnPointList GetSpawnPointList()
	{
		return null;
	}

	// Token: 0x06004D6B RID: 19819 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetSpawnPointList(SpawnPointList _spawnPointList)
	{
	}

	// Token: 0x06004D6C RID: 19820 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool GetOverviewMap(Vector2i _startPos, Vector2i _size, Color[] mapColors)
	{
		return false;
	}

	// Token: 0x06004D6D RID: 19821 RVA: 0x001EA35F File Offset: 0x001E855F
	public virtual void SetDecorationsEnabled(bool _bEnable)
	{
		this.bDecorationsEnabled = _bEnable;
	}

	// Token: 0x06004D6E RID: 19822 RVA: 0x001EA368 File Offset: 0x001E8568
	public virtual bool IsDecorationsEnabled()
	{
		return this.bDecorationsEnabled;
	}

	// Token: 0x06004D6F RID: 19823 RVA: 0x001EA370 File Offset: 0x001E8570
	public virtual bool GetWorldExtent(out Vector3i _minSize, out Vector3i _maxSize)
	{
		_minSize = Vector3i.zero;
		_maxSize = Vector3i.zero;
		return false;
	}

	// Token: 0x06004D70 RID: 19824 RVA: 0x001EA38C File Offset: 0x001E858C
	public virtual BoundsInt GetWorldBounds()
	{
		Vector3i vector3i;
		Vector3i vector3i2;
		this.GetWorldExtent(out vector3i, out vector3i2);
		return new BoundsInt(vector3i.x, vector3i.y, vector3i.z, vector3i2.x, vector3i2.y, vector3i2.z);
	}

	// Token: 0x06004D71 RID: 19825 RVA: 0x001EA3D0 File Offset: 0x001E85D0
	public virtual Vector2i GetWorldSize()
	{
		Vector3i vector3i;
		Vector3i vector3i2;
		this.GetWorldExtent(out vector3i, out vector3i2);
		return new Vector2i(vector3i2.x - vector3i.x + 1, vector3i2.y - vector3i.y + 1);
	}

	// Token: 0x06004D72 RID: 19826 RVA: 0x00019766 File Offset: 0x00017966
	public virtual IBiomeProvider GetBiomeProvider()
	{
		return null;
	}

	// Token: 0x06004D73 RID: 19827 RVA: 0x00019766 File Offset: 0x00017966
	public virtual ITerrainGenerator GetTerrainGenerator()
	{
		return null;
	}

	// Token: 0x06004D74 RID: 19828 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual int GetPOIBlockIdOverride(int x, int z)
	{
		return 0;
	}

	// Token: 0x06004D75 RID: 19829 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public virtual float GetPOIHeightOverride(int x, int z)
	{
		return 0f;
	}

	// Token: 0x06004D76 RID: 19830 RVA: 0x001EA40B File Offset: 0x001E860B
	public virtual IEnumerator FillOccupiedMap(int w, int h, DecoOccupiedMap occupiedMap, List<PrefabInstance> overridePOIList = null)
	{
		yield break;
	}

	// Token: 0x06004D77 RID: 19831 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void RebuildTerrain(HashSetLong _chunks, Vector3i _areaStart, Vector3i _areaSize, bool _isStopStabilityCalc, bool _isRegenChunk, bool _isFillEmptyBlocks, bool _isReset)
	{
	}

	// Token: 0x06004D78 RID: 19832 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual ChunkProtectionLevel GetChunkProtectionLevel(Vector3i worldPos)
	{
		return ChunkProtectionLevel.None;
	}

	// Token: 0x170007F6 RID: 2038
	// (get) Token: 0x06004D79 RID: 19833 RVA: 0x001EA413 File Offset: 0x001E8613
	// (set) Token: 0x06004D7A RID: 19834 RVA: 0x001EA41B File Offset: 0x001E861B
	public GameUtils.WorldInfo WorldInfo { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x06004D7B RID: 19835 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public ChunkProviderAbstract()
	{
	}

	// Token: 0x04003B23 RID: 15139
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bDecorationsEnabled;
}
