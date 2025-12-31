using System;
using UnityEngine;

// Token: 0x02000A64 RID: 2660
public class UnityDistantTerrainTest
{
	// Token: 0x060050F2 RID: 20722 RVA: 0x0020400E File Offset: 0x0020220E
	public static void Create()
	{
		UnityDistantTerrainTest.Instance = new UnityDistantTerrainTest();
	}

	// Token: 0x060050F3 RID: 20723 RVA: 0x0020401C File Offset: 0x0020221C
	public void LoadTerrain()
	{
		if (!this.parentObj)
		{
			this.parentObj = new GameObject("DistantUnityTerrain");
		}
		UnityDistantTerrain.Config config = new UnityDistantTerrain.Config
		{
			DataWidth = this.hmWidth,
			DataHeight = this.hmHeight,
			DataTileSize = 512,
			DataSteps = 2,
			SplatSteps = 1,
			MetersPerHeightPix = 2,
			MaxHeight = 256,
			PixelError = 5
		};
		int visibleChunks = Mathf.CeilToInt((float)(Mathf.Clamp(GamePrefs.GetInt(EnumGamePrefs.OptionsGfxTerrainQuality), 2, 4) * 512) / (float)config.ChunkWorldSize * 2f) + 1;
		int num = config.DataWidth / config.DataTileSize;
		int num2 = config.DataHeight / config.DataTileSize;
		TileAreaConfig tileAreaConfig = new TileAreaConfig
		{
			tileStart = new Vector2i(-num / 2, -num2 / 2),
			tileEnd = new Vector2i(Utils.FastMax(0, num / 2 - 1), Utils.FastMax(0, num2 / 2 - 1)),
			tileSizeInWorldUnits = config.ChunkWorldSize,
			bWrapAroundX = false,
			bWrapAroundZ = false
		};
		TileAreaConfig tileAreaConfig2 = tileAreaConfig;
		tileAreaConfig2.tileSizeInWorldUnits = config.DataTileSize;
		ITileArea<float[,]> heights = this.LoadTerrainHeightTiles(tileAreaConfig, this.HeightMap, config.DataWidth, config.DataHeight, config.DataTileSize);
		this.unityTerrain = new UnityDistantTerrain();
		this.unityTerrain.Init(this.parentObj, config, visibleChunks, this.TerrainMaterial, this.WaterMaterial, this.WaterChunks16x16Width, this.WaterChunks16x16, heights, null, null, null);
	}

	// Token: 0x060050F4 RID: 20724 RVA: 0x002041BC File Offset: 0x002023BC
	public ITileArea<float[,]> LoadTerrainHeightTiles(TileAreaConfig _config, IBackedArray<ushort> _rawHeightMap, int _heightMapWidth, int _heightMapHeight, int _sliceAtPix)
	{
		if (PlatformOptimizations.FileBackedTerrainTiles)
		{
			TileFile<float> tileFile = HeightMapUtils.ConvertAndSliceUnityHeightmapQuarteredToFile(_rawHeightMap, _heightMapWidth, _heightMapHeight, _sliceAtPix);
			return new TileAreaCache<float>(_config, tileFile, 9);
		}
		float[,][,] data = HeightMapUtils.ConvertAndSliceUnityHeightmapQuartered(_rawHeightMap, _heightMapWidth, _heightMapHeight, _sliceAtPix);
		return new TileArea<float[,]>(_config, data);
	}

	// Token: 0x060050F5 RID: 20725 RVA: 0x002041F9 File Offset: 0x002023F9
	public void FrameUpdate(EntityPlayerLocal _player)
	{
		if (this.unityTerrain != null)
		{
			this.unityTerrain.FrameUpdate(_player);
		}
	}

	// Token: 0x060050F6 RID: 20726 RVA: 0x0020420F File Offset: 0x0020240F
	public void OnChunkVisible(int _chunkX, int _chunkZ, bool _visible)
	{
		if (this.unityTerrain != null)
		{
			this.unityTerrain.OnChunkUpdate(_chunkX, _chunkZ, _visible);
		}
	}

	// Token: 0x060050F7 RID: 20727 RVA: 0x00204227 File Offset: 0x00202427
	public void Cleanup()
	{
		if (this.unityTerrain != null)
		{
			this.unityTerrain.Cleanup();
			this.unityTerrain = null;
		}
		UnityEngine.Object.Destroy(this.parentObj);
	}

	// Token: 0x04003E25 RID: 15909
	public const string cGameObjectName = "DistantUnityTerrain";

	// Token: 0x04003E26 RID: 15910
	public static UnityDistantTerrainTest Instance;

	// Token: 0x04003E27 RID: 15911
	public UnityDistantTerrain unityTerrain;

	// Token: 0x04003E28 RID: 15912
	public IBackedArray<ushort> HeightMap;

	// Token: 0x04003E29 RID: 15913
	public int hmWidth = 6144;

	// Token: 0x04003E2A RID: 15914
	public int hmHeight = 6144;

	// Token: 0x04003E2B RID: 15915
	public Material TerrainMaterial;

	// Token: 0x04003E2C RID: 15916
	public Material WaterMaterial;

	// Token: 0x04003E2D RID: 15917
	public int WaterChunks16x16Width;

	// Token: 0x04003E2E RID: 15918
	public byte[] WaterChunks16x16;

	// Token: 0x04003E2F RID: 15919
	public GameObject parentObj;

	// Token: 0x04003E30 RID: 15920
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cTileSize = 512;
}
