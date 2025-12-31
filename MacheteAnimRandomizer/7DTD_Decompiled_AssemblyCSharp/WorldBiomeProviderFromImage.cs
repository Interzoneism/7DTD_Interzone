using System;
using System.Collections;
using Unity.Collections;
using UnityEngine;

// Token: 0x02000ADA RID: 2778
public class WorldBiomeProviderFromImage : IBiomeProvider
{
	// Token: 0x0600557E RID: 21886 RVA: 0x0022D419 File Offset: 0x0022B619
	public WorldBiomeProviderFromImage(string _levelName, WorldBiomes _biomes, int _worldSize = 4096)
	{
		this.worldName = _levelName;
		this.m_Biomes = _biomes;
		this.worldSize = _worldSize;
		this.worldSizeHalf = _worldSize / 2;
	}

	// Token: 0x0600557F RID: 21887 RVA: 0x0022D44A File Offset: 0x0022B64A
	public IEnumerator InitData()
	{
		PathAbstractions.AbstractedLocation worldLocation = PathAbstractions.WorldsSearchPaths.GetLocation(this.worldName, null, null);
		string str = worldLocation.FullPath + "/biomes";
		Texture2D biomesTex;
		if (SdFile.Exists(str + ".tga"))
		{
			biomesTex = TextureUtils.LoadTexture(str + ".tga", FilterMode.Point, false, false, null);
		}
		else
		{
			biomesTex = TextureUtils.LoadTexture(str + ".png", FilterMode.Point, false, false, null);
		}
		yield return null;
		this.biomeMapWidth = biomesTex.width;
		this.biomeMapHeight = biomesTex.height;
		this.biomesMapWidthHalf = biomesTex.width / 2;
		this.biomesMapHeightHalf = biomesTex.height / 2;
		Log.Out("Biomes image size w= " + biomesTex.width.ToString() + ", h = " + biomesTex.height.ToString());
		yield return null;
		BiomeImageLoader biomesLoader = new BiomeImageLoader(biomesTex, this.m_Biomes.GetBiomeMap());
		yield return biomesLoader.Load();
		this.m_BiomeMap = biomesLoader.biomeMap;
		biomesLoader = default(BiomeImageLoader);
		yield return null;
		this.biomesScaleDiv = this.worldSize / biomesTex.width;
		UnityEngine.Object.Destroy(biomesTex);
		yield return GCUtils.UnloadAndCollectCo();
		this.noiseGen = new PerlinNoise(this.worldName.GetStableHashCode());
		yield return null;
		Texture2D texture2D = null;
		str = worldLocation.FullPath + "/radiation";
		if (SdFile.Exists(str + ".tga"))
		{
			texture2D = TextureUtils.LoadTexture(str + ".tga", FilterMode.Point, false, false, null);
		}
		else if (SdFile.Exists(str + ".png"))
		{
			texture2D = TextureUtils.LoadTexture(str + ".png", FilterMode.Point, false, false, null);
		}
		if (texture2D != null)
		{
			this.radiationMapSize = texture2D.width;
			this.radiationMapScale = this.worldSize / this.radiationMapSize;
			if (texture2D.width <= 512 && texture2D.height <= 512)
			{
				this.radiationMapSmall = new byte[texture2D.width * texture2D.height];
				if (texture2D.format == TextureFormat.RGBA32)
				{
					using (NativeArray<Color32> pixelData = texture2D.GetPixelData<Color32>(0))
					{
						for (int i = 0; i < pixelData.Length; i++)
						{
							this.radiationMapSmall[i] = this.ProcessColor(pixelData[i]);
						}
						goto IL_47C;
					}
				}
				using (NativeArray<TextureUtils.ColorARGB32> pixelData2 = texture2D.GetPixelData<TextureUtils.ColorARGB32>(0))
				{
					for (int j = 0; j < pixelData2.Length; j++)
					{
						this.radiationMapSmall[j] = this.ProcessColor(pixelData2[j]);
					}
					goto IL_47C;
				}
			}
			this.radiationTilesX = this.radiationMapSize / 512;
			this.radiationTilesZ = this.radiationMapSize / 512;
			this.radiationTileAreaConfig = new TileAreaConfig
			{
				tileStart = new Vector2i(-this.radiationTilesX / 2, -this.radiationTilesZ / 2),
				tileEnd = new Vector2i(Utils.FastMax(0, this.radiationTilesX / 2 - 1), Utils.FastMax(0, this.radiationTilesZ / 2 - 1)),
				tileSizeInWorldUnits = 512,
				bWrapAroundX = false,
				bWrapAroundZ = false
			};
			if (PlatformOptimizations.FileBackedRadiationTiles)
			{
				Debug.Log("Loading RadiationMap to File");
				this.radiationMap = this.LoadRadiationMapToFile(texture2D, this.radiationTileAreaConfig);
			}
			else
			{
				this.radiationMap = this.LoadRadiationMap(texture2D, this.radiationTileAreaConfig);
			}
			IL_47C:
			UnityEngine.Object.Destroy(texture2D);
			yield return GCUtils.UnloadAndCollectCo();
		}
		yield break;
	}

	// Token: 0x06005580 RID: 21888 RVA: 0x0022D45C File Offset: 0x0022B65C
	[PublicizedFrom(EAccessModifier.Private)]
	public void FillRadiationResult<T>(NativeArray<T> radPixs, byte[,][,] result, Func<T, byte> processColor) where T : struct
	{
		for (int i = 0; i < result.GetLength(0); i++)
		{
			for (int j = 0; j < result.GetLength(1); j++)
			{
				result[j, i] = new byte[512, 512];
				for (int k = 0; k < 512; k++)
				{
					for (int l = 0; l < 512; l++)
					{
						int index = (i * 512 + k) * this.radiationMapSize + (j * 512 + l);
						T arg = radPixs[index];
						byte b = processColor(arg);
						result[j, i][l, k] = b;
					}
				}
			}
		}
	}

	// Token: 0x06005581 RID: 21889 RVA: 0x0022D510 File Offset: 0x0022B710
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe void FillRadiationFileBackedArray<T>(NativeArray<T> radPixs, FileBackedArray<byte> fba, Func<T, byte> processColor) where T : struct
	{
		for (int i = 0; i < this.radiationTilesZ; i++)
		{
			for (int j = 0; j < this.radiationTilesX; j++)
			{
				int start = i * 512 * 512 * this.radiationTilesX + j * 512 * 512;
				int num = 0;
				Span<byte> span2;
				using (fba.GetSpan(start, 262144, out span2))
				{
					for (int k = 0; k < 512; k++)
					{
						for (int l = 0; l < 512; l++)
						{
							int index = (i * 512 + l) * this.radiationMapSize + (j * 512 + k);
							T arg = radPixs[index];
							byte b = processColor(arg);
							*span2[num] = b;
							num++;
						}
					}
				}
			}
		}
	}

	// Token: 0x06005582 RID: 21890 RVA: 0x0022D608 File Offset: 0x0022B808
	[PublicizedFrom(EAccessModifier.Private)]
	public byte ProcessColor(Color32 pixel)
	{
		byte result = 0;
		if (pixel.g > 0)
		{
			result = 1;
		}
		if (pixel.b > 0)
		{
			result = 2;
		}
		if (pixel.r > 0)
		{
			result = 3;
		}
		return result;
	}

	// Token: 0x06005583 RID: 21891 RVA: 0x0022D63C File Offset: 0x0022B83C
	[PublicizedFrom(EAccessModifier.Private)]
	public byte ProcessColor(TextureUtils.ColorARGB32 pixel)
	{
		byte result = 0;
		if (pixel.g > 0)
		{
			result = 1;
		}
		if (pixel.b > 0)
		{
			result = 2;
		}
		if (pixel.r > 0)
		{
			result = 3;
		}
		return result;
	}

	// Token: 0x06005584 RID: 21892 RVA: 0x0022D670 File Offset: 0x0022B870
	[PublicizedFrom(EAccessModifier.Private)]
	public void RadiationTileArrayFileFromTexture(FileBackedArray<byte> fba, Texture2D radiationTexture)
	{
		if (radiationTexture.format == TextureFormat.RGBA32)
		{
			using (NativeArray<Color32> pixelData = radiationTexture.GetPixelData<Color32>(0))
			{
				this.FillRadiationFileBackedArray<Color32>(pixelData, fba, new Func<Color32, byte>(this.ProcessColor));
				return;
			}
		}
		using (NativeArray<TextureUtils.ColorARGB32> pixelData2 = radiationTexture.GetPixelData<TextureUtils.ColorARGB32>(0))
		{
			this.FillRadiationFileBackedArray<TextureUtils.ColorARGB32>(pixelData2, fba, new Func<TextureUtils.ColorARGB32, byte>(this.ProcessColor));
		}
	}

	// Token: 0x06005585 RID: 21893 RVA: 0x0022D6FC File Offset: 0x0022B8FC
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[,][,] RadiationTileArrayFromTexture(Texture2D radiationTexture)
	{
		int num = radiationTexture.width / 512;
		int num2 = radiationTexture.height / 512;
		byte[,][,] result = new byte[num, num2][,];
		if (radiationTexture.format == TextureFormat.RGBA32)
		{
			using (NativeArray<Color32> pixelData = radiationTexture.GetPixelData<Color32>(0))
			{
				this.FillRadiationResult<Color32>(pixelData, result, new Func<Color32, byte>(this.ProcessColor));
				return result;
			}
		}
		using (NativeArray<TextureUtils.ColorARGB32> pixelData2 = radiationTexture.GetPixelData<TextureUtils.ColorARGB32>(0))
		{
			this.FillRadiationResult<TextureUtils.ColorARGB32>(pixelData2, result, new Func<TextureUtils.ColorARGB32, byte>(this.ProcessColor));
		}
		return result;
	}

	// Token: 0x06005586 RID: 21894 RVA: 0x0022D7A8 File Offset: 0x0022B9A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void worldCoordsToTileCoords(int worldX, int worldZ, TileAreaConfig tileAreaConfig, out int tileX, out int tileZ, out int posX, out int posZ)
	{
		tileX = (worldX + this.worldSizeHalf) / 512 + tileAreaConfig.tileStart.x;
		tileZ = (worldZ + this.worldSizeHalf) / 512 + tileAreaConfig.tileStart.y;
		posX = worldX - tileX * 512;
		posZ = worldZ - tileZ * 512;
	}

	// Token: 0x06005587 RID: 21895 RVA: 0x0022D80C File Offset: 0x0022BA0C
	[PublicizedFrom(EAccessModifier.Private)]
	public ITileArea<byte[,]> LoadRadiationMap(Texture2D radiationTex, TileAreaConfig tileAreaConfig)
	{
		byte[,][,] data = this.RadiationTileArrayFromTexture(radiationTex);
		return new TileArea<byte[,]>(tileAreaConfig, data);
	}

	// Token: 0x06005588 RID: 21896 RVA: 0x0022D828 File Offset: 0x0022BA28
	[PublicizedFrom(EAccessModifier.Private)]
	public ITileArea<byte[,]> LoadRadiationMapToFile(Texture2D radiationTex, TileAreaConfig tileAreaConfig)
	{
		FileBackedArray<byte> fileBackedArray = new FileBackedArray<byte>(radiationTex.width * radiationTex.height);
		this.RadiationTileArrayFileFromTexture(fileBackedArray, radiationTex);
		TileFile<byte> tileFile = new TileFile<byte>(fileBackedArray, 512, this.radiationTilesX, this.radiationTilesZ);
		return new TileAreaCache<byte>(tileAreaConfig, tileFile, 9);
	}

	// Token: 0x06005589 RID: 21897 RVA: 0x0022D874 File Offset: 0x0022BA74
	[PublicizedFrom(EAccessModifier.Private)]
	public void loadSplatMaps(string _levelName, int _worldWidth)
	{
		string fullPath = PathAbstractions.WorldsSearchPaths.GetLocation(_levelName, null, null).FullPath;
		string text = fullPath + "/splat1.png";
		if (!SdFile.Exists(text))
		{
			return;
		}
		Texture2D texture2D = TextureUtils.LoadTexture(text, FilterMode.Point, false, false, null);
		Color32[] pixels = texture2D.GetPixels32();
		Color32[] array = null;
		Color32[] array2 = null;
		this.splatW = texture2D.width;
		int height = texture2D.height;
		this.splatScaleDiv = _worldWidth / this.splatW;
		if (Application.isEditor)
		{
			UnityEngine.Object.DestroyImmediate(texture2D);
		}
		else
		{
			UnityEngine.Object.Destroy(texture2D);
		}
		this.cntSplatChannels += 4;
		string text2 = fullPath + "/splat2.png";
		if (SdFile.Exists(text2))
		{
			Texture2D texture2D2 = TextureUtils.LoadTexture(text2, FilterMode.Point, false, false, null);
			array = texture2D2.GetPixels32();
			if (Application.isEditor)
			{
				UnityEngine.Object.DestroyImmediate(texture2D2);
			}
			else
			{
				UnityEngine.Object.Destroy(texture2D2);
			}
			this.cntSplatChannels += 4;
		}
		string text3 = fullPath + "/splat3.png";
		if (SdFile.Exists(text3))
		{
			Texture2D texture2D3 = TextureUtils.LoadTexture(text3, FilterMode.Point, false, false, null);
			array2 = texture2D3.GetPixels32();
			if (Application.isEditor)
			{
				UnityEngine.Object.DestroyImmediate(texture2D3);
			}
			else
			{
				UnityEngine.Object.Destroy(texture2D3);
			}
			this.cntSplatChannels += 4;
		}
		this.splatMapMaxValue = new byte[pixels.Length];
		Color32 color = new Color32(0, 0, 0, 0);
		Color32 color2 = new Color32(0, 0, 0, 0);
		Color32 color3 = new Color32(0, 0, 0, 0);
		for (int i = pixels.Length - 1; i >= 0; i--)
		{
			color = pixels[i];
			if (this.cntSplatChannels > 4)
			{
				color2 = array[i];
			}
			if (this.cntSplatChannels > 8)
			{
				color3 = array2[i];
			}
			int num = 0;
			if (color.r >= color.g && color.r >= color.b && color.r >= color.a && color.r >= color2.r && color.r >= color2.g && color.r >= color2.b && color.r >= color2.a && color.r >= color3.r && color.r >= color3.g && color.r >= color3.b && color.r >= color3.a)
			{
				num = 0;
			}
			else if (color.g >= color.r && color.g >= color.b && color.g >= color.a && color.g >= color2.r && color.g >= color2.g && color.g >= color2.b && color.g >= color2.a && color.g >= color3.r && color.g >= color3.g && color.g >= color3.b && color.g >= color3.a)
			{
				num = 1;
			}
			else if (color.b >= color.r && color.b >= color.g && color.b >= color.a && color.b >= color2.r && color.b >= color2.g && color.b >= color2.b && color.b >= color2.a && color.b >= color3.r && color.b >= color3.g && color.b >= color3.b && color.b >= color3.a)
			{
				num = 2;
			}
			else if (color.a >= color.r && color.a >= color.g && color.a >= color.b && color.a >= color2.r && color.a >= color2.g && color.a >= color2.b && color.a >= color2.a && color.a >= color3.r && color.a >= color3.g && color.a >= color3.b && color.a >= color3.a)
			{
				num = 3;
			}
			else if (color2.r >= color2.g && color2.r >= color2.b && color2.r >= color2.a && color2.r >= color.r && color2.r >= color.g && color2.r >= color.b && color2.r >= color.a && color2.r >= color3.r && color2.r >= color3.g && color2.r >= color3.b && color2.r >= color3.a)
			{
				num = 4;
			}
			else if (color2.g >= color2.r && color2.g >= color2.b && color2.g >= color2.a && color2.g >= color.r && color2.g >= color.g && color2.g >= color.b && color2.g >= color.a && color2.g >= color3.r && color2.g >= color3.g && color2.g >= color3.b && color2.g >= color3.a)
			{
				num = 5;
			}
			else if (color2.b >= color2.r && color2.b >= color2.g && color2.b >= color2.a && color2.b >= color.r && color2.b >= color.g && color2.b >= color.b && color2.b >= color.a && color2.b >= color3.r && color2.b >= color3.g && color2.b >= color3.b && color2.b >= color3.a)
			{
				num = 6;
			}
			else if (color2.a >= color2.r && color2.a >= color2.g && color2.a >= color2.b && color2.a >= color.r && color2.a >= color.g && color2.a >= color.b && color2.a >= color.a && color2.a >= color3.r && color2.a >= color3.g && color2.a >= color3.b && color2.a >= color3.a)
			{
				num = 7;
			}
			else if (color3.r >= color3.g && color3.r >= color3.b && color3.r >= color3.a && color3.r >= color.r && color3.r >= color.g && color3.r >= color.b && color3.r >= color.a && color3.r >= color2.r && color3.r >= color2.g && color3.r >= color2.b && color3.r >= color2.a)
			{
				num = 8;
			}
			else if (color3.g >= color3.r && color3.g >= color3.b && color3.g >= color3.a && color3.g >= color.r && color3.g >= color.g && color3.g >= color.b && color3.g >= color.a && color3.g >= color2.r && color3.g >= color2.g && color3.g >= color2.b && color3.g >= color2.a)
			{
				num = 9;
			}
			else if (color3.b >= color3.r && color3.b >= color3.g && color3.b >= color3.a && color3.b >= color.r && color3.b >= color.g && color3.b >= color.b && color3.b >= color.a && color3.b >= color2.r && color3.b >= color2.g && color3.b >= color2.b && color3.b >= color3.a)
			{
				num = 10;
			}
			else if (color3.a >= color3.r && color3.a >= color3.g && color3.a >= color3.b && color3.a >= color.r && color3.a >= color.g && color3.a >= color.b && color3.a >= color.a && color3.a >= color2.r && color3.a >= color2.g && color3.a >= color2.b && color3.a >= color2.a)
			{
				num = 11;
			}
			this.splatMapMaxValue[i] = (byte)num;
		}
	}

	// Token: 0x0600558A RID: 21898 RVA: 0x0022E33F File Offset: 0x0022C53F
	public string GetWorldName()
	{
		return this.worldName;
	}

	// Token: 0x0600558B RID: 21899 RVA: 0x00002914 File Offset: 0x00000B14
	public void Init(int _seed, string _worldName, WorldBiomes _biomes, string _params1, string _params2)
	{
	}

	// Token: 0x0600558C RID: 21900 RVA: 0x0022E347 File Offset: 0x0022C547
	public BiomeDefinition GetBiomeAt(int x, int z, out float _intensity)
	{
		_intensity = 1f;
		return this.GetBiomeAt(x, z);
	}

	// Token: 0x0600558D RID: 21901 RVA: 0x0022E358 File Offset: 0x0022C558
	public BiomeDefinition GetBiomeAt(int x, int z)
	{
		if (this.biomesScaleDiv == 0)
		{
			return null;
		}
		int num = x / this.biomesScaleDiv + this.biomesMapWidthHalf;
		if (num < 0 || num >= this.m_BiomeMap.width)
		{
			return null;
		}
		int num2 = z / this.biomesScaleDiv + this.biomesMapHeightHalf;
		if (num2 < 0 || num2 >= this.m_BiomeMap.height)
		{
			return null;
		}
		byte value = this.m_BiomeMap.GetValue(num, num2);
		if (value == 255)
		{
			return null;
		}
		return this.m_Biomes.GetBiome(value);
	}

	// Token: 0x0600558E RID: 21902 RVA: 0x0022E3DC File Offset: 0x0022C5DC
	public BiomeDefinition GetBiomeOrSubAt(int x, int z)
	{
		BiomeDefinition biomeDefinition = this.GetBiomeAt(x, z);
		if (biomeDefinition != null)
		{
			int subBiomeIdxAt = this.GetSubBiomeIdxAt(biomeDefinition, x, 0, z);
			if (subBiomeIdxAt >= 0)
			{
				biomeDefinition = biomeDefinition.subbiomes[subBiomeIdxAt];
			}
		}
		return biomeDefinition;
	}

	// Token: 0x0600558F RID: 21903 RVA: 0x0022E414 File Offset: 0x0022C614
	public int GetSubBiomeIdxAt(BiomeDefinition bd, int _x, int _y, int _z)
	{
		double num = -3.4028234663852886E+38;
		double num2 = 0.0;
		Vector2 vector = Vector2.zero;
		for (int i = 0; i < bd.subbiomes.Count; i++)
		{
			BiomeDefinition biomeDefinition = bd.subbiomes[i];
			if (num != (double)biomeDefinition.noiseFreq || vector != biomeDefinition.noiseOffset)
			{
				num = (double)biomeDefinition.noiseFreq;
				vector = biomeDefinition.noiseOffset;
				num2 = (double)((float)this.noiseGen.FBM((double)((float)_x + vector.x), (double)((float)_z + vector.y), num));
				num2 = num2 * 0.5 + 0.5;
			}
			if (num2 >= (double)biomeDefinition.noiseMin && num2 < (double)biomeDefinition.noiseMax)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06005590 RID: 21904 RVA: 0x0022E4E3 File Offset: 0x0022C6E3
	public Vector2i GetSize()
	{
		return new Vector2i(this.biomeMapWidth, this.biomeMapHeight);
	}

	// Token: 0x06005591 RID: 21905 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public float GetHumidityAt(int x, int z)
	{
		return 0f;
	}

	// Token: 0x06005592 RID: 21906 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public float GetTemperatureAt(int x, int z)
	{
		return 0f;
	}

	// Token: 0x06005593 RID: 21907 RVA: 0x0022E4F8 File Offset: 0x0022C6F8
	public float GetRadiationAt(int x, int z)
	{
		if (this.radiationMapSmall != null)
		{
			int num = (x + this.worldSizeHalf) / this.radiationMapScale + (z + this.worldSizeHalf) / this.radiationMapScale * this.radiationMapSize;
			if (num >= 0 && num < this.radiationMapSmall.Length)
			{
				return (float)this.radiationMapSmall[num];
			}
			return 0f;
		}
		else
		{
			if (this.radiationMap == null)
			{
				return 0f;
			}
			int worldX = x / this.radiationMapScale;
			int worldZ = z / this.radiationMapScale;
			int num2;
			int num3;
			int num4;
			int num5;
			this.worldCoordsToTileCoords(worldX, worldZ, this.radiationTileAreaConfig, out num2, out num3, out num4, out num5);
			if (num2 < this.radiationTileAreaConfig.tileStart.x || num3 < this.radiationTileAreaConfig.tileStart.y || num2 > this.radiationTileAreaConfig.tileEnd.x || num3 > this.radiationTileAreaConfig.tileEnd.y || num4 < 0 || num5 < 0 || num4 > 512 || num5 > 512)
			{
				return 0f;
			}
			return (float)this.radiationMap[num2, num3][num4, num5];
		}
	}

	// Token: 0x06005594 RID: 21908 RVA: 0x0022E614 File Offset: 0x0022C814
	public BlockValue GetTopmostBlockValue(int xWorld, int zWorld)
	{
		this.bvReturn.type = 0;
		if (xWorld < -this.worldSizeHalf || xWorld >= this.worldSizeHalf || zWorld < -this.worldSizeHalf || zWorld >= this.worldSizeHalf)
		{
			return this.bvReturn;
		}
		if (this.cntSplatChannels > 0)
		{
			switch (this.splatMapMaxValue[(xWorld + this.worldSizeHalf) / this.splatScaleDiv + (zWorld + this.worldSizeHalf) / this.splatScaleDiv * this.splatW])
			{
			case 0:
				this.bvReturn.type = 14;
				break;
			case 1:
				this.bvReturn.type = 1;
				break;
			case 2:
				this.bvReturn.type = 9;
				break;
			case 3:
				this.bvReturn.type = 8;
				break;
			case 4:
				this.bvReturn.type = 12;
				break;
			case 5:
				this.bvReturn.type = 13;
				break;
			case 6:
				this.bvReturn.type = 16;
				break;
			case 7:
				this.bvReturn.type = 11;
				break;
			case 8:
				this.bvReturn.type = 3;
				break;
			case 9:
				this.bvReturn.type = 29;
				break;
			case 10:
				this.bvReturn.type = 28;
				break;
			case 11:
				this.bvReturn.type = 2;
				break;
			}
		}
		return this.bvReturn;
	}

	// Token: 0x06005595 RID: 21909 RVA: 0x0022E78C File Offset: 0x0022C98C
	public void Cleanup()
	{
		ITileArea<byte[,]> tileArea = this.radiationMap;
		if (tileArea == null)
		{
			return;
		}
		tileArea.Cleanup();
	}

	// Token: 0x0400420A RID: 16906
	[PublicizedFrom(EAccessModifier.Private)]
	public GridCompressedData<byte> m_BiomeMap;

	// Token: 0x0400420B RID: 16907
	[PublicizedFrom(EAccessModifier.Private)]
	public WorldBiomes m_Biomes;

	// Token: 0x0400420C RID: 16908
	[PublicizedFrom(EAccessModifier.Private)]
	public PerlinNoise noiseGen;

	// Token: 0x0400420D RID: 16909
	[PublicizedFrom(EAccessModifier.Private)]
	public string worldName;

	// Token: 0x0400420E RID: 16910
	[PublicizedFrom(EAccessModifier.Private)]
	public int biomeMapWidth;

	// Token: 0x0400420F RID: 16911
	[PublicizedFrom(EAccessModifier.Private)]
	public int biomesMapWidthHalf;

	// Token: 0x04004210 RID: 16912
	[PublicizedFrom(EAccessModifier.Private)]
	public int biomeMapHeight;

	// Token: 0x04004211 RID: 16913
	[PublicizedFrom(EAccessModifier.Private)]
	public int biomesMapHeightHalf;

	// Token: 0x04004212 RID: 16914
	[PublicizedFrom(EAccessModifier.Private)]
	public int biomesScaleDiv;

	// Token: 0x04004213 RID: 16915
	[PublicizedFrom(EAccessModifier.Private)]
	public int radiationMapSize;

	// Token: 0x04004214 RID: 16916
	[PublicizedFrom(EAccessModifier.Private)]
	public int radiationMapScale;

	// Token: 0x04004215 RID: 16917
	[PublicizedFrom(EAccessModifier.Private)]
	public ITileArea<byte[,]> radiationMap;

	// Token: 0x04004216 RID: 16918
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] radiationMapSmall;

	// Token: 0x04004217 RID: 16919
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cRadiationTileSize = 512;

	// Token: 0x04004218 RID: 16920
	[PublicizedFrom(EAccessModifier.Private)]
	public int radiationTilesX;

	// Token: 0x04004219 RID: 16921
	[PublicizedFrom(EAccessModifier.Private)]
	public int radiationTilesZ;

	// Token: 0x0400421A RID: 16922
	[PublicizedFrom(EAccessModifier.Private)]
	public TileAreaConfig radiationTileAreaConfig;

	// Token: 0x0400421B RID: 16923
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] splatMapMaxValue;

	// Token: 0x0400421C RID: 16924
	[PublicizedFrom(EAccessModifier.Private)]
	public int cntSplatChannels;

	// Token: 0x0400421D RID: 16925
	[PublicizedFrom(EAccessModifier.Private)]
	public int splatScaleDiv;

	// Token: 0x0400421E RID: 16926
	[PublicizedFrom(EAccessModifier.Private)]
	public int splatW;

	// Token: 0x0400421F RID: 16927
	[PublicizedFrom(EAccessModifier.Private)]
	public int worldSize;

	// Token: 0x04004220 RID: 16928
	[PublicizedFrom(EAccessModifier.Private)]
	public int worldSizeHalf;

	// Token: 0x04004221 RID: 16929
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue bvReturn = BlockValue.Air;
}
