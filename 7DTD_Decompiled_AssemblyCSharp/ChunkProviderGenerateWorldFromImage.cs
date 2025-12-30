using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020009C8 RID: 2504
public class ChunkProviderGenerateWorldFromImage : ChunkProviderGenerateWorld
{
	// Token: 0x06004CCF RID: 19663 RVA: 0x001E6C54 File Offset: 0x001E4E54
	public ChunkProviderGenerateWorldFromImage(ChunkCluster _cc, string _levelName, bool _bLoadDTM = true) : base(_cc, _levelName, false)
	{
		this.bLoadDTM = _bLoadDTM;
	}

	// Token: 0x06004CD0 RID: 19664 RVA: 0x001E6C66 File Offset: 0x001E4E66
	public override void Cleanup()
	{
		HeightMap heightMap = this.heightMap;
		if (heightMap != null)
		{
			heightMap.Dispose();
		}
		this.heightMap = null;
		IBackedArray<ushort> backedArray = this.rawdata;
		if (backedArray != null)
		{
			backedArray.Dispose();
		}
		this.rawdata = null;
		base.Cleanup();
	}

	// Token: 0x06004CD1 RID: 19665 RVA: 0x001E6C9E File Offset: 0x001E4E9E
	public override IEnumerator Init(World _world)
	{
		if (this.bLoadDTM)
		{
			yield return this.<>n__0(_world);
		}
		else
		{
			this.world = _world;
		}
		Stopwatch sw = new Stopwatch();
		sw.Start();
		if (this.bLoadDTM)
		{
			this.loadDTM();
		}
		Log.Out("Loading and parsing of dtm.png took " + sw.ElapsedMilliseconds.ToString() + "ms");
		sw.Reset();
		sw.Start();
		HeightMap heightMap = this.heightMap;
		if (heightMap != null)
		{
			heightMap.Dispose();
		}
		this.heightMap = new HeightMap(this.m_Dtm.DimX, this.m_Dtm.DimY, 255f, this.rawdata, 0);
		this.m_BiomeProvider = new WorldBiomeProviderFromImage(this.levelName, this.world.Biomes, 4096);
		WorldDecoratorPOIFromImage worldDecoratorPOIFromImage = new WorldDecoratorPOIFromImage(this.levelName, this.GetDynamicPrefabDecorator(), this.m_Dtm.DimX, this.m_Dtm.DimY, null, false, 1, this.heightMap, null);
		this.m_Decorators.Add(worldDecoratorPOIFromImage);
		yield return worldDecoratorPOIFromImage.InitData();
		this.m_Decorators.Add(new WorldDecoratorBlocksFromBiome(this.m_BiomeProvider, this.GetDynamicPrefabDecorator()));
		Log.Out("Loading and parsing of generator took " + sw.ElapsedMilliseconds.ToString() + "ms");
		sw.Reset();
		sw.Start();
		if (this.bLoadDTM)
		{
			this.m_TerrainGenerator = new TerrainFromDTM();
			((TerrainFromDTM)this.m_TerrainGenerator).Init(this.m_Dtm, this.m_BiomeProvider, this.levelName, this.world.Seed);
			string text = _world.IsEditor() ? null : GameIO.GetSaveGameRegionDir();
			this.m_RegionFileManager = new RegionFileManager(text, text, 0, !_world.IsEditor());
			MultiBlockManager.Instance.Initialize(this.m_RegionFileManager);
		}
		yield return null;
		yield break;
	}

	// Token: 0x06004CD2 RID: 19666 RVA: 0x001E6CB4 File Offset: 0x001E4EB4
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe void loadDTM()
	{
		Texture2D texture2D = null;
		PathAbstractions.AbstractedLocation location = PathAbstractions.WorldsSearchPaths.GetLocation(this.levelName, null, null);
		if (SdFile.Exists(location.FullPath + "/dtm.png"))
		{
			texture2D = TextureUtils.LoadTexture(location.FullPath + "/dtm.png", FilterMode.Point, false, false, null);
			Log.Out("Loading local DTM");
		}
		else if (SdFile.Exists(location.FullPath + "/dtm.tga"))
		{
			texture2D = TextureUtils.LoadTexture(location.FullPath + "/dtm.tga", FilterMode.Point, false, false, null);
			Log.Out("Loading local DTM");
		}
		else
		{
			texture2D = (Resources.Load("Data/Worlds/" + this.levelName + "/dtm", typeof(Texture2D)) as Texture2D);
		}
		Log.Out("DTM image size w= " + texture2D.width.ToString() + ", h = " + texture2D.height.ToString());
		Color[] pixels = texture2D.GetPixels();
		this.m_Dtm = new ArrayWithOffset<byte>(texture2D.width, texture2D.height);
		IBackedArray<ushort> backedArray = this.rawdata;
		if (backedArray != null)
		{
			backedArray.Dispose();
		}
		this.rawdata = BackedArrays.Create<ushort>(pixels.Length);
		int width = texture2D.width;
		int height = texture2D.height;
		for (int i = 0; i < height; i++)
		{
			Span<ushort> span2;
			using (this.rawdata.GetSpan(i * width, width, out span2))
			{
				for (int j = 0; j < width; j++)
				{
					this.m_Dtm[j + this.m_Dtm.MinPos.x, i + this.m_Dtm.MinPos.y] = (byte)(pixels[width * i + j].grayscale * 255f);
					*span2[j] = (ushort)((int)((byte)(pixels[width * i + j].grayscale * 255f)) | (int)((byte)((pixels[width * i + j].grayscale * 255f - 255f) * 255f)) << 8);
				}
			}
		}
		Resources.UnloadAsset(texture2D);
	}

	// Token: 0x06004CD3 RID: 19667 RVA: 0x001E6EF8 File Offset: 0x001E50F8
	public override void ReloadAllChunks()
	{
		this.loadDTM();
		((TerrainFromDTM)this.m_TerrainGenerator).Init(this.m_Dtm, this.m_BiomeProvider, this.levelName, this.world.Seed);
		base.ReloadAllChunks();
	}

	// Token: 0x06004CD4 RID: 19668 RVA: 0x000282C0 File Offset: 0x000264C0
	public override EnumChunkProviderId GetProviderId()
	{
		return EnumChunkProviderId.GenerateFromDtm;
	}

	// Token: 0x06004CD5 RID: 19669 RVA: 0x001E6F34 File Offset: 0x001E5134
	public override bool GetWorldExtent(out Vector3i _minSize, out Vector3i _maxSize)
	{
		_minSize = new Vector3i(this.m_Dtm.MinPos.x, 0, this.m_Dtm.MinPos.y);
		_maxSize = new Vector3i(this.m_Dtm.MaxPos.x, 255, this.m_Dtm.MaxPos.y);
		return true;
	}

	// Token: 0x06004CD6 RID: 19670 RVA: 0x001E6FA0 File Offset: 0x001E51A0
	public override int GetPOIBlockIdOverride(int _x, int _z)
	{
		WorldDecoratorPOIFromImage worldDecoratorPOIFromImage = (WorldDecoratorPOIFromImage)this.m_Decorators[0];
		WorldGridCompressedData<byte> poi = worldDecoratorPOIFromImage.m_Poi;
		int x = _x / worldDecoratorPOIFromImage.worldScale;
		int y = _z / worldDecoratorPOIFromImage.worldScale;
		if (!poi.Contains(x, y))
		{
			return 0;
		}
		byte data = poi.GetData(x, y);
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

	// Token: 0x06004CD7 RID: 19671 RVA: 0x001E7024 File Offset: 0x001E5224
	public override float GetPOIHeightOverride(int x, int z)
	{
		WorldDecoratorPOIFromImage worldDecoratorPOIFromImage = (WorldDecoratorPOIFromImage)this.m_Decorators[0];
		WorldGridCompressedData<byte> poi = worldDecoratorPOIFromImage.m_Poi;
		byte data;
		if (!poi.Contains(x / worldDecoratorPOIFromImage.worldScale, z / worldDecoratorPOIFromImage.worldScale) || (data = poi.GetData(x / worldDecoratorPOIFromImage.worldScale, z / worldDecoratorPOIFromImage.worldScale)) == 255 || data == 0)
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

	// Token: 0x04003AAD RID: 15021
	[PublicizedFrom(EAccessModifier.Private)]
	public ArrayWithOffset<byte> m_Dtm;

	// Token: 0x04003AAE RID: 15022
	public IBackedArray<ushort> rawdata;

	// Token: 0x04003AAF RID: 15023
	[PublicizedFrom(EAccessModifier.Private)]
	public HeightMap heightMap;

	// Token: 0x04003AB0 RID: 15024
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bLoadDTM;
}
