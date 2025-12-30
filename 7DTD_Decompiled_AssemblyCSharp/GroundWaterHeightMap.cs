using System;

// Token: 0x02000B54 RID: 2900
public class GroundWaterHeightMap
{
	// Token: 0x06005A4A RID: 23114 RVA: 0x00243E5B File Offset: 0x0024205B
	public GroundWaterHeightMap(World _world)
	{
		this.world = _world;
	}

	// Token: 0x06005A4B RID: 23115 RVA: 0x00243E6C File Offset: 0x0024206C
	public bool TryInit()
	{
		if (this.poiColors != null && this.biomes != null)
		{
			return true;
		}
		ChunkProviderGenerateWorldFromRaw chunkProviderGenerateWorldFromRaw = this.world.ChunkCache.ChunkProvider as ChunkProviderGenerateWorldFromRaw;
		if (chunkProviderGenerateWorldFromRaw == null)
		{
			return false;
		}
		WorldDecoratorPOIFromImage poiFromImage = chunkProviderGenerateWorldFromRaw.poiFromImage;
		if (poiFromImage == null)
		{
			return false;
		}
		this.poiColors = poiFromImage.m_Poi;
		this.biomes = this.world.Biomes;
		return this.poiColors != null && this.biomes != null;
	}

	// Token: 0x06005A4C RID: 23116 RVA: 0x00243EE4 File Offset: 0x002420E4
	[PublicizedFrom(EAccessModifier.Private)]
	public PoiMapElement GetPoiMapElement(int _worldX, int _worldZ)
	{
		if (!this.poiColors.Contains(_worldX, _worldZ))
		{
			return null;
		}
		byte data = this.poiColors.GetData(_worldX, _worldZ);
		if (data == 0)
		{
			return null;
		}
		return this.biomes.getPoiForColor((uint)data);
	}

	// Token: 0x06005A4D RID: 23117 RVA: 0x00243F24 File Offset: 0x00242124
	public bool TryGetWaterHeightAt(int _worldX, int _worldZ, out int _height)
	{
		PoiMapElement poiMapElement = this.GetPoiMapElement(_worldX, _worldZ);
		if (poiMapElement == null)
		{
			_height = 0;
			return false;
		}
		if (poiMapElement.m_BlockValue.type != 240)
		{
			_height = 0;
			return false;
		}
		_height = poiMapElement.m_YPosFill;
		return true;
	}

	// Token: 0x04004515 RID: 17685
	[PublicizedFrom(EAccessModifier.Private)]
	public World world;

	// Token: 0x04004516 RID: 17686
	[PublicizedFrom(EAccessModifier.Private)]
	public WorldGridCompressedData<byte> poiColors;

	// Token: 0x04004517 RID: 17687
	[PublicizedFrom(EAccessModifier.Private)]
	public WorldBiomes biomes;
}
