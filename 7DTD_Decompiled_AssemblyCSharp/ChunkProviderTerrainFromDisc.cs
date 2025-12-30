using System;
using System.Collections;
using UnityEngine;

// Token: 0x020009D5 RID: 2517
public class ChunkProviderTerrainFromDisc : ChunkProviderGenerateWorld
{
	// Token: 0x06004D1F RID: 19743 RVA: 0x001E9D31 File Offset: 0x001E7F31
	public ChunkProviderTerrainFromDisc(ChunkCluster _cc, string _levelName) : base(_cc, _levelName, false)
	{
		this.SetDecorationsEnabled(true);
	}

	// Token: 0x06004D20 RID: 19744 RVA: 0x001E9D43 File Offset: 0x001E7F43
	public override IEnumerator Init(World _world)
	{
		yield return this.<>n__0(_world);
		this.m_BiomeProvider = new WorldBiomeProviderFromImage(this.levelName, _world.Biomes, 4096);
		WorldDecoratorPOIFromImage worldDecoratorPOIFromImage = new WorldDecoratorPOIFromImage(this.levelName, this.GetDynamicPrefabDecorator(), 6144, 6144, null, false, 1, null, null);
		this.m_Decorators.Add(worldDecoratorPOIFromImage);
		yield return worldDecoratorPOIFromImage.InitData();
		this.m_Decorators.Add(new WorldDecoratorBlocksFromBiome(this.m_BiomeProvider, this.GetDynamicPrefabDecorator()));
		string saveGameRegionDirDefault = GameIO.GetSaveGameRegionDirDefault();
		string saveDirectory = _world.IsEditor() ? GameIO.GetSaveGameRegionDirDefault() : null;
		this.terrainRegionFileManager = new RegionFileManager(saveGameRegionDirDefault, saveDirectory, 0, true);
		this.m_TerrainGenerator = new TerrainFromChunk();
		((TerrainFromChunk)this.m_TerrainGenerator).Init(this.terrainRegionFileManager, this.m_BiomeProvider, _world.Seed);
		string text = _world.IsEditor() ? null : GameIO.GetSaveGameRegionDir();
		this.m_RegionFileManager = new MyRegionFileManager(_world, this, this.terrainRegionFileManager, text, text, 0, !_world.IsEditor());
		yield return null;
		yield break;
	}

	// Token: 0x06004D21 RID: 19745 RVA: 0x001E9D59 File Offset: 0x001E7F59
	public override void Cleanup()
	{
		base.Cleanup();
		this.terrainRegionFileManager.Cleanup();
	}

	// Token: 0x06004D22 RID: 19746 RVA: 0x001E9D6C File Offset: 0x001E7F6C
	public override void SaveAll()
	{
		base.SaveAll();
		if (this.world.IsEditor())
		{
			this.m_RegionFileManager.MakePersistent(this.world.ChunkCache, false);
			this.m_RegionFileManager.WaitSaveDone();
			this.terrainRegionFileManager.MakePersistent(null, false);
			this.terrainRegionFileManager.WaitSaveDone();
		}
	}

	// Token: 0x06004D23 RID: 19747 RVA: 0x001E9DC8 File Offset: 0x001E7FC8
	public override bool GetOverviewMap(Vector2i _startPos, Vector2i _size, Color[] mapColors)
	{
		this.terrainRegionFileManager.SetCacheSize(1000);
		bool overviewMap = base.GetOverviewMap(_startPos, _size, mapColors);
		this.terrainRegionFileManager.SetCacheSize(0);
		return overviewMap;
	}

	// Token: 0x06004D24 RID: 19748 RVA: 0x001E9DF0 File Offset: 0x001E7FF0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void generateTerrain(World _world, Chunk _chunk, GameRandom _random)
	{
		long key = _chunk.Key;
		if (!this.terrainRegionFileManager.ContainsChunkSync(key))
		{
			return;
		}
		Chunk chunkSync = this.terrainRegionFileManager.GetChunkSync(key);
		if (chunkSync == null)
		{
			return;
		}
		((TerrainFromChunk)this.m_TerrainGenerator).SetTerrainChunk(chunkSync);
		this.m_TerrainGenerator.GenerateTerrain(_world, _chunk, _random);
		((TerrainFromChunk)this.m_TerrainGenerator).SetTerrainChunk(null);
		_chunk.CopyLightsFrom(chunkSync);
		_chunk.isModified = false;
		this.terrainRegionFileManager.RemoveChunkSync(key);
		MemoryPools.PoolChunks.FreeSync(chunkSync);
	}

	// Token: 0x06004D25 RID: 19749 RVA: 0x00075CC0 File Offset: 0x00073EC0
	public override EnumChunkProviderId GetProviderId()
	{
		return EnumChunkProviderId.ChunkDataDriven;
	}

	// Token: 0x06004D26 RID: 19750 RVA: 0x001E9E7C File Offset: 0x001E807C
	public override bool GetWorldExtent(out Vector3i _minSize, out Vector3i _maxSize)
	{
		Vector2i minPos = ((WorldDecoratorPOIFromImage)this.m_Decorators[0]).m_Poi.MinPos;
		Vector2i maxPos = ((WorldDecoratorPOIFromImage)this.m_Decorators[0]).m_Poi.MaxPos;
		_minSize = new Vector3i(minPos.x, 0, minPos.y) + new Vector3i(80, 0, 80);
		_maxSize = new Vector3i(maxPos.x, 255, maxPos.y) - new Vector3i(80, 0, 80);
		return true;
	}

	// Token: 0x06004D27 RID: 19751 RVA: 0x001E9F14 File Offset: 0x001E8114
	public override int GetPOIBlockIdOverride(int _x, int _z)
	{
		WorldGridCompressedData<byte> poi = ((WorldDecoratorPOIFromImage)this.m_Decorators[0]).m_Poi;
		if (!poi.Contains(_x, _z))
		{
			return 0;
		}
		byte data = poi.GetData(_x, _z);
		if (data == 0 || data == 255)
		{
			return 0;
		}
		PoiMapElement poiForColor = this.world.Biomes.getPoiForColor((uint)data);
		if (poiForColor == null)
		{
			return 0;
		}
		return poiForColor.m_BlockValue.type;
	}

	// Token: 0x06004D28 RID: 19752 RVA: 0x001E9F7C File Offset: 0x001E817C
	public override float GetPOIHeightOverride(int x, int z)
	{
		WorldGridCompressedData<byte> poi = ((WorldDecoratorPOIFromImage)this.m_Decorators[0]).m_Poi;
		byte data;
		if (!poi.Contains(x, z) || (data = poi.GetData(x, z)) == 255 || data == 0)
		{
			return 0f;
		}
		PoiMapElement poiForColor = this.world.Biomes.getPoiForColor((uint)data);
		if (poiForColor == null)
		{
			return 0f;
		}
		if (!poiForColor.m_BlockValue.Block.blockMaterial.IsLiquid)
		{
			return 0f;
		}
		return (float)poiForColor.m_YPosFill;
	}

	// Token: 0x04003B0F RID: 15119
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionFileManager terrainRegionFileManager;
}
