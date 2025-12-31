using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Unity.Collections;
using UnityEngine;

// Token: 0x020009CA RID: 2506
public class ChunkProviderGenerateWorldFromRaw : ChunkProviderGenerateWorld
{
	// Token: 0x06004CDF RID: 19679 RVA: 0x001E7370 File Offset: 0x001E5570
	public ChunkProviderGenerateWorldFromRaw(ChunkCluster _cc, string _levelName, bool _bClientMode = false, bool _bFixedWaterLevel = false) : base(_cc, _levelName, _bClientMode)
	{
		this.bFixedWaterLevel = _bFixedWaterLevel;
	}

	// Token: 0x06004CE0 RID: 19680 RVA: 0x001E73CD File Offset: 0x001E55CD
	public override IEnumerator Init(World _world)
	{
		ChunkProviderGenerateWorldFromRaw.<>c__DisplayClass17_0 CS$<>8__locals1 = new ChunkProviderGenerateWorldFromRaw.<>c__DisplayClass17_0();
		yield return this.<>n__0(_world);
		this.world = _world;
		PathAbstractions.AbstractedLocation location = PathAbstractions.WorldsSearchPaths.GetLocation(this.levelName, null, null);
		string worldPath = location.FullPath;
		base.WorldInfo = GameUtils.WorldInfo.LoadWorldInfo(location);
		if (base.WorldInfo != null)
		{
			this.heightMapWidth = base.WorldInfo.HeightmapSize.x;
			this.heightMapHeight = base.WorldInfo.HeightmapSize.y;
			this.heightMapScale = base.WorldInfo.Scale;
			this.bFixedWaterLevel = base.WorldInfo.FixedWaterLevel;
		}
		MicroStopwatch ms = new MicroStopwatch();
		string dtmFilename = this.getFilenameDTM();
		if (!SdFile.Exists(dtmFilename + ".raw"))
		{
			if (SdFile.Exists(dtmFilename + ".tga"))
			{
				int num;
				int num2;
				Color32[] array = TGALoader.LoadTGAAsArray(dtmFilename + ".tga", out num, out num2, null);
				if (array != null)
				{
					float[,] data = HeightMapUtils.ConvertDTMToHeightData(array, this.heightMapWidth, this.heightMapHeight, true);
					HeightMapUtils.SaveHeightMapRAW(this.getFilenameDTM() + ".raw", this.heightMapWidth, this.heightMapHeight, data);
					HeightMapUtils.SaveHeightMapRAW(dtmFilename + ".raw", this.heightMapWidth, this.heightMapHeight, data);
				}
				Log.Out("Converting tga to dtm took " + ms.ElapsedMilliseconds.ToString() + "ms");
				ms.ResetAndRestart();
			}
			else
			{
				if (!SdFile.Exists(dtmFilename + ".png"))
				{
					throw new FileNotFoundException(string.Format("No height data found for world '{0}'", this.levelName));
				}
				float[,] data2 = HeightMapUtils.ConvertDTMToHeightDataExternal(this.levelName, true);
				HeightMapUtils.SmoothTerrain(7, data2);
				HeightMapUtils.SaveHeightMapRAW(dtmFilename + ".raw", this.heightMapWidth, this.heightMapHeight, data2);
				Log.Out("Converting tga to dtm took " + ms.ElapsedMilliseconds.ToString() + "ms");
				ms.ResetAndRestart();
			}
		}
		yield return GCUtils.UnloadAndCollectCo();
		yield return this.calcWorldFileCrcs(worldPath);
		CS$<>8__locals1.worldDecoratorPoiFromImage = null;
		CS$<>8__locals1.splat3Tex = null;
		CS$<>8__locals1.splat4Tex = null;
		CS$<>8__locals1.splat3Half = null;
		CS$<>8__locals1.splat4Half = null;
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.filesNeedProcessing(worldPath, dtmFilename))
		{
			yield return this.processFiles(worldPath, delegate(Texture2D _splat3Tex, Texture2D _splat3Half, Texture2D _splat4Tex, Texture2D _splat4Half, WorldDecoratorPOIFromImage _poiFromImage)
			{
				CS$<>8__locals1.splat3Tex = _splat3Tex;
				CS$<>8__locals1.splat3Half = _splat3Half;
				CS$<>8__locals1.splat4Tex = _splat4Tex;
				CS$<>8__locals1.splat4Half = _splat4Half;
				CS$<>8__locals1.worldDecoratorPoiFromImage = _poiFromImage;
			});
		}
		else
		{
			XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadHeightmap", false), true);
			this.loadDTM(null);
			Log.Out("Loading dtm raw file took " + ms.ElapsedMilliseconds.ToString() + "ms");
			ms.ResetAndRestart();
		}
		yield return GCUtils.UnloadAndCollectCo();
		XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadBiomes", false), true);
		this.m_BiomeProvider = new WorldBiomeProviderFromImage(this.levelName, this.world.Biomes, this.heightMapWidth * this.heightMapScale);
		yield return this.m_BiomeProvider.InitData();
		Log.Out("Loading and creating biomes took " + ms.ElapsedMilliseconds.ToString() + "ms");
		ms.ResetAndRestart();
		yield return GCUtils.UnloadAndCollectCo();
		XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadSplats", false), true);
		string text = worldPath + "/splat3_processed.png";
		if (SdFile.Exists(text))
		{
			if (CS$<>8__locals1.splat3Tex == null)
			{
				CS$<>8__locals1.splat3Tex = TextureUtils.LoadTexture(text, FilterMode.Point, false, false, null);
			}
			yield return GCUtils.UnloadAndCollectCo();
		}
		yield return null;
		text = worldPath + "/splat4_processed.png";
		if (SdFile.Exists(text))
		{
			if (CS$<>8__locals1.splat4Tex == null)
			{
				CS$<>8__locals1.splat4Tex = TextureUtils.LoadTexture(text, FilterMode.Point, false, false, null);
			}
			yield return GCUtils.UnloadAndCollectCo();
		}
		yield return null;
		int num3 = this.GetWorldSize().x / 8;
		int height = this.GetWorldSize().y / 8;
		this.procBiomeMask1 = new Texture2D(num3, height, TextureFormat.RGBA32, false);
		this.procBiomeMask2 = new Texture2D(num3, height, TextureFormat.RGBA32, false);
		NativeArray<Color32> pixelData = this.procBiomeMask1.GetPixelData<Color32>(0);
		NativeArray<Color32> pixelData2 = this.procBiomeMask2.GetPixelData<Color32>(0);
		Vector3i vector3i;
		Vector3i vector3i2;
		this.GetWorldExtent(out vector3i, out vector3i2);
		for (int i = vector3i.z; i < vector3i2.z; i += 8)
		{
			int num4 = (i - vector3i.z) / 8 * num3;
			for (int j = vector3i.x; j < vector3i2.x; j += 8)
			{
				BiomeDefinition biomeAt = this.m_BiomeProvider.GetBiomeAt(j, i);
				if (biomeAt != null)
				{
					Color32 value = default(Color32);
					Color32 value2 = default(Color32);
					int index = (j - vector3i.x) / 8 + num4;
					switch (biomeAt.m_Id)
					{
					case 1:
						value.r = byte.MaxValue;
						break;
					case 3:
						value.g = byte.MaxValue;
						break;
					case 5:
						value2.r = byte.MaxValue;
						break;
					case 8:
						value.a = byte.MaxValue;
						break;
					case 9:
						value.b = byte.MaxValue;
						break;
					}
					pixelData[index] = value;
					pixelData2[index] = value2;
				}
			}
		}
		yield return null;
		this.procBiomeMask1.filterMode = FilterMode.Bilinear;
		this.procBiomeMask1.Apply(false, true);
		this.procBiomeMask2.filterMode = FilterMode.Bilinear;
		this.procBiomeMask2.Apply(false, true);
		Log.Out("Loading and creating shader control textures took " + ms.ElapsedMilliseconds.ToString() + "ms");
		ms.ResetAndRestart();
		yield return GCUtils.UnloadAndCollectCo();
		this.m_TerrainGenerator = new TerrainFromRaw();
		if (this.heightMap == null)
		{
			this.heightMap = new HeightMap(this.heightMapWidth, this.heightMapHeight, 255f, this.heightData, this.heightMapWidth * this.heightMapScale);
		}
		yield return null;
		if (CS$<>8__locals1.worldDecoratorPoiFromImage == null)
		{
			ChunkProviderGenerateWorldFromRaw.<>c__DisplayClass17_0 CS$<>8__locals2 = CS$<>8__locals1;
			string levelName = this.levelName;
			DynamicPrefabDecorator dynamicPrefabDecorator = this.GetDynamicPrefabDecorator();
			int worldX = this.heightMapWidth;
			int worldZ = this.heightMapHeight;
			Texture2D splat3Tex = CS$<>8__locals1.splat3Tex;
			bool bChangeWaterDensity = false;
			Texture2D splat4Tex = CS$<>8__locals1.splat4Tex;
			CS$<>8__locals2.worldDecoratorPoiFromImage = new WorldDecoratorPOIFromImage(levelName, dynamicPrefabDecorator, worldX, worldZ, splat3Tex, bChangeWaterDensity, this.heightMapScale, this.heightMap, splat4Tex);
			this.m_Decorators.Add(CS$<>8__locals1.worldDecoratorPoiFromImage);
			yield return CS$<>8__locals1.worldDecoratorPoiFromImage.InitData();
		}
		this.m_Decorators.Add(new WorldDecoratorBlocksFromBiome(this.m_BiomeProvider, this.GetDynamicPrefabDecorator()));
		yield return null;
		this.poiFromImage = (this.m_Decorators[0] as WorldDecoratorPOIFromImage);
		Log.Out("Loading and parsing of generator took " + ms.ElapsedMilliseconds.ToString() + "ms");
		ms.ResetAndRestart();
		((TerrainFromRaw)this.m_TerrainGenerator).Init(this.heightMap, this.m_BiomeProvider, this.levelName, this.world.Seed);
		yield return null;
		string text2 = _world.IsEditor() ? null : GameIO.GetSaveGameRegionDir();
		if (!this.bClientMode)
		{
			this.m_RegionFileManager = new RegionFileManager(text2, text2, 0, !_world.IsEditor());
		}
		MultiBlockManager.Instance.Initialize(this.m_RegionFileManager);
		yield return null;
		if (GameOptionsManager.GetTextureQuality(-1) > 0)
		{
			UnityEngine.Object.Destroy(CS$<>8__locals1.splat3Tex);
			UnityEngine.Object.Destroy(CS$<>8__locals1.splat4Tex);
			text = worldPath + "/splat3_half.png";
			if (SdFile.Exists(text))
			{
				if (CS$<>8__locals1.splat3Half == null)
				{
					CS$<>8__locals1.splat3Half = TextureUtils.LoadTexture(text, FilterMode.Point, false, false, null);
				}
				yield return null;
				this.splats[0] = CS$<>8__locals1.splat3Half;
				this.splats[0].filterMode = FilterMode.Bilinear;
			}
			yield return null;
			text = worldPath + "/splat4_half.png";
			if (SdFile.Exists(text))
			{
				if (CS$<>8__locals1.splat4Half == null)
				{
					CS$<>8__locals1.splat4Half = TextureUtils.LoadTexture(text, FilterMode.Point, false, false, null);
				}
				yield return null;
				this.splats[1] = CS$<>8__locals1.splat4Half;
				this.splats[1].filterMode = FilterMode.Bilinear;
			}
		}
		else
		{
			UnityEngine.Object.Destroy(CS$<>8__locals1.splat3Half);
			UnityEngine.Object.Destroy(CS$<>8__locals1.splat4Half);
			this.splats[0] = CS$<>8__locals1.splat3Tex;
			this.splats[0].filterMode = FilterMode.Bilinear;
			this.splats[1] = CS$<>8__locals1.splat4Tex;
			this.splats[1].filterMode = FilterMode.Bilinear;
		}
		yield return null;
		this.splats[0].Compress(false);
		this.splats[0].Apply(false, true);
		yield return null;
		this.splats[1].Compress(false);
		this.splats[1].Apply(false, true);
		yield return GCUtils.UnloadAndCollectCo();
		yield break;
	}

	// Token: 0x06004CE1 RID: 19681 RVA: 0x001E73E4 File Offset: 0x001E55E4
	[PublicizedFrom(EAccessModifier.Private)]
	public void loadDTM(string _dtmFilename = null)
	{
		if (_dtmFilename == null)
		{
			_dtmFilename = this.getFilenameDTM();
		}
		IBackedArray<ushort> backedArray = this.heightData;
		if (backedArray != null)
		{
			backedArray.Dispose();
		}
		this.heightData = HeightMapUtils.LoadHeightMapRAW(_dtmFilename + ".raw", this.heightMapWidth, this.heightMapHeight, 1f, 250);
	}

	// Token: 0x06004CE2 RID: 19682 RVA: 0x001E743C File Offset: 0x001E563C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool filesNeedProcessing(string _worldPath, string _dtmFilename)
	{
		return !_dtmFilename.EndsWith("_processed") || !SdFile.Exists(_worldPath + "/splat3_processed.png") || !SdFile.Exists(_worldPath + "/splat4_processed.png") || !SdFile.Exists(_worldPath + "/splat3_half.png") || !SdFile.Exists(_worldPath + "/splat4_half.png") || !this.verifyFileHashes(_worldPath);
	}

	// Token: 0x170007EA RID: 2026
	// (get) Token: 0x06004CE3 RID: 19683 RVA: 0x001E74AA File Offset: 0x001E56AA
	// (set) Token: 0x06004CE4 RID: 19684 RVA: 0x001E74B2 File Offset: 0x001E56B2
	public long worldFileTotalSize { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06004CE5 RID: 19685 RVA: 0x001E74BB File Offset: 0x001E56BB
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator calcWorldFileCrcs(string _worldPath)
	{
		MicroStopwatch msw = new MicroStopwatch(true);
		this.worldFileTotalSize = 0L;
		this.worldFileCrcs.Clear();
		byte[] buffer = new byte[32768];
		string[] array = SdDirectory.GetFiles(_worldPath);
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			string filename = GameIO.GetFilenameFromPath(text);
			this.worldFileTotalSize += GameIO.FileSize(text);
			yield return IOUtils.CalcCrcCoroutine(text, delegate(uint _hash)
			{
				this.worldFileCrcs.Add(filename, _hash);
			}, Constants.cMaxLoadTimePerFrameMillis, buffer);
		}
		array = null;
		Log.Out("Calculating world hashes took {0} ms (world size {1} MiB)", new object[]
		{
			msw.ElapsedMilliseconds,
			this.worldFileTotalSize / 1024L / 1024L
		});
		yield break;
	}

	// Token: 0x06004CE6 RID: 19686 RVA: 0x001E74D4 File Offset: 0x001E56D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void saveFileHashes(string _worldPath, Dictionary<string, uint> _worldFileCrcs)
	{
		using (Stream stream = SdFile.Open(_worldPath + "/checksums.txt", FileMode.Create, FileAccess.Write))
		{
			using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
			{
				foreach (KeyValuePair<string, uint> keyValuePair in this.worldFileCrcs)
				{
					streamWriter.Write(keyValuePair.Key);
					streamWriter.Write("=");
					streamWriter.WriteLine(keyValuePair.Value);
				}
			}
		}
	}

	// Token: 0x06004CE7 RID: 19687 RVA: 0x001E7590 File Offset: 0x001E5790
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, uint> loadStoredFileHashes(string _worldPath)
	{
		if (!SdFile.Exists(_worldPath + "/checksums.txt"))
		{
			return null;
		}
		Dictionary<string, uint> dictionary = new CaseInsensitiveStringDictionary<uint>();
		using (StreamReader streamReader = SdFile.OpenText(_worldPath + "/checksums.txt"))
		{
			string text;
			while ((text = streamReader.ReadLine()) != null)
			{
				string[] array = text.Split('=', StringSplitOptions.None);
				if (array.Length != 2)
				{
					Log.Warning("Invalid line in checksums.txt: {0}", new object[]
					{
						text
					});
				}
				else
				{
					string key = array[0];
					uint value = StringParsers.ParseUInt32(array[1], 0, -1, NumberStyles.Integer);
					dictionary.Add(key, value);
				}
			}
		}
		return dictionary;
	}

	// Token: 0x06004CE8 RID: 19688 RVA: 0x001E7634 File Offset: 0x001E5834
	[PublicizedFrom(EAccessModifier.Private)]
	public bool verifyFileHashes(string _worldPath)
	{
		Dictionary<string, uint> dictionary = this.loadStoredFileHashes(_worldPath);
		if (dictionary == null)
		{
			Log.Warning("No hashes for world");
			return false;
		}
		foreach (string text in ChunkProviderGenerateWorldFromRaw.FilesUsedForProcessing)
		{
			if (!dictionary.ContainsKey(text) && SdFile.Exists(_worldPath + "/" + text))
			{
				Log.Warning("Missing hash for " + text);
				return false;
			}
			if (this.worldFileCrcs.ContainsKey(text) && this.worldFileCrcs[text] != dictionary[text])
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06004CE9 RID: 19689 RVA: 0x001E76C4 File Offset: 0x001E58C4
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator processFiles(string _worldPath, ChunkProviderGenerateWorldFromRaw.ProcessFilesCallback _callback)
	{
		XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadProcessWorldHeightmap", false), true);
		MicroStopwatch yieldMs = new MicroStopwatch(true);
		MicroStopwatch ms = new MicroStopwatch(true);
		Log.Out("Processing world files");
		string processedFileName = _worldPath + "/dtm_processed.raw";
		this.loadDTM(_worldPath + "/dtm");
		ushort[] topTexMap = new ushort[this.heightMapWidth * this.heightMapScale * this.heightMapHeight * this.heightMapScale];
		yield return this.GetDynamicPrefabDecorator().CopyPrefabHeightsIntoHeightMap(this.heightMapWidth, this.heightMapHeight, this.heightData, this.heightMapScale, topTexMap);
		ThreadManager.AddSingleTask(delegate(ThreadManager.TaskInfo _taskInfo)
		{
			HeightMapUtils.SaveHeightMapRAW(processedFileName, this.heightMapWidth, this.heightMapHeight, this.heightData);
		}, null, null, true);
		yield return null;
		XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadProcessWorldSplats", false), true);
		Texture2D splat3Tex = null;
		string str = _worldPath + "/splat3";
		if (SdFile.Exists(str + ".png"))
		{
			splat3Tex = TextureUtils.LoadTexture(str + ".png", FilterMode.Point, true, false, null);
		}
		else if (SdFile.Exists(str + ".tga"))
		{
			splat3Tex = TextureUtils.LoadTexture(str + ".tga", FilterMode.Point, false, false, null);
		}
		bool flag = splat3Tex.format == TextureFormat.ARGB32;
		if (!flag && splat3Tex.format != TextureFormat.RGBA32)
		{
			Log.Error("World's splat3 file is not in the correct format (needs to be either RGBA32 or ARGB32)!");
			yield break;
		}
		Texture2D splat4Tex = new Texture2D(this.heightMapWidth * this.heightMapScale, this.heightMapHeight * this.heightMapScale, TextureFormat.ARGB32, true, false);
		string str2 = _worldPath + "/splat4";
		bool splat4Loaded = false;
		if (SdFile.Exists(str2 + ".png"))
		{
			splat4Tex = TextureUtils.LoadTexture(str2 + ".png", FilterMode.Point, true, false, null);
			splat4Loaded = true;
		}
		else if (SdFile.Exists(str2 + ".tga"))
		{
			splat4Tex = TextureUtils.LoadTexture(str2 + ".tga", FilterMode.Point, false, false, null);
			splat4Loaded = true;
		}
		NativeArray<TextureUtils.ColorARGB32> splat4Cols = splat4Tex.GetRawTextureData<TextureUtils.ColorARGB32>();
		int num2;
		if (flag)
		{
			NativeArray<TextureUtils.ColorARGB32> splat3Cols = splat3Tex.GetRawTextureData<TextureUtils.ColorARGB32>();
			for (int i = 0; i < topTexMap.Length; i = num2 + 1)
			{
				if (i % Constants.cMaxLoadTimePixelsPerTest == 0 && yieldMs.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
				{
					yield return null;
					yieldMs.ResetAndRestart();
				}
				TextureUtils.ColorARGB32 value = splat3Cols[i];
				TextureUtils.ColorARGB32 value2 = default(TextureUtils.ColorARGB32);
				if (splat4Loaded)
				{
					value2 = splat4Cols[i];
				}
				ushort num = topTexMap[i];
				if (num <= 2)
				{
					if (num != 0)
					{
						if (num == 2)
						{
							value2.g = byte.MaxValue;
						}
					}
				}
				else
				{
					switch (num)
					{
					case 8:
						value.b = byte.MaxValue;
						break;
					case 9:
						break;
					case 10:
						value.r = byte.MaxValue;
						break;
					case 11:
						value.g = byte.MaxValue;
						break;
					default:
						if (num != 185)
						{
							if (num == 200)
							{
								value2.r = byte.MaxValue;
							}
						}
						else
						{
							value.a = byte.MaxValue;
						}
						break;
					}
				}
				splat3Cols[i] = value;
				splat4Cols[i] = value2;
				num2 = i;
			}
			splat3Cols = default(NativeArray<TextureUtils.ColorARGB32>);
		}
		else
		{
			NativeArray<Color32> splat3Cols2 = splat3Tex.GetRawTextureData<Color32>();
			for (int i = 0; i < topTexMap.Length; i = num2 + 1)
			{
				if (i % Constants.cMaxLoadTimePixelsPerTest == 0 && yieldMs.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
				{
					yield return null;
					yieldMs.ResetAndRestart();
				}
				Color32 value3 = splat3Cols2[i];
				TextureUtils.ColorARGB32 value4 = default(TextureUtils.ColorARGB32);
				if (splat4Loaded)
				{
					value4 = splat4Cols[i];
				}
				ushort num = topTexMap[i];
				if (num <= 2)
				{
					if (num != 0)
					{
						if (num == 2)
						{
							value4.g = byte.MaxValue;
						}
					}
				}
				else
				{
					switch (num)
					{
					case 8:
						value3.b = byte.MaxValue;
						break;
					case 9:
						break;
					case 10:
						value3.r = byte.MaxValue;
						break;
					case 11:
						value3.g = byte.MaxValue;
						break;
					default:
						if (num != 185)
						{
							if (num == 200)
							{
								value4.r = byte.MaxValue;
							}
						}
						else
						{
							value3.a = byte.MaxValue;
						}
						break;
					}
				}
				splat3Cols2[i] = value3;
				splat4Cols[i] = value4;
				num2 = i;
			}
			splat3Cols2 = default(NativeArray<Color32>);
		}
		yield return null;
		if (this.heightMap == null)
		{
			this.heightMap = new HeightMap(this.heightMapWidth, this.heightMapHeight, 255f, this.heightData, this.heightMapWidth * this.heightMapScale);
		}
		splat4Tex.SetPixelData<TextureUtils.ColorARGB32>(splat4Cols.ToArray(), 0, 0);
		XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadProcessWorldDecos", false), true);
		string levelName = this.levelName;
		DynamicPrefabDecorator dynamicPrefabDecorator = this.GetDynamicPrefabDecorator();
		int worldX = this.heightMapWidth;
		int worldZ = this.heightMapHeight;
		Texture2D splat3Tex2 = splat3Tex;
		bool bChangeWaterDensity = false;
		Texture2D splat4Tex2 = splat4Loaded ? splat4Tex : null;
		WorldDecoratorPOIFromImage worldDecoratorPoiFromImage = new WorldDecoratorPOIFromImage(levelName, dynamicPrefabDecorator, worldX, worldZ, splat3Tex2, bChangeWaterDensity, this.heightMapScale, this.heightMap, splat4Tex2);
		this.m_Decorators.Add(worldDecoratorPoiFromImage);
		yield return worldDecoratorPoiFromImage.InitData();
		GridCompressedData<byte> cols = worldDecoratorPoiFromImage.m_Poi.colors;
		int length = cols.width * cols.height;
		for (int i = 0; i < length; i = num2 + 1)
		{
			if (i % Constants.cMaxLoadTimePixelsPerTest == 0 && yieldMs.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
			{
				yield return null;
				yieldMs.ResetAndRestart();
			}
			if (cols.GetValue(i) >= 5)
			{
				TextureUtils.ColorARGB32 value5 = splat4Cols[i];
				if (Mathf.FloorToInt(this.heightMap.GetAt(i)) <= (int)(cols.GetValue(i) - 5 + 1))
				{
					value5.g = byte.MaxValue;
					if (cols.GetValue(i) > 5)
					{
						value5.b = cols.GetValue(i) - 5;
					}
				}
				splat4Cols[i] = value5;
			}
			num2 = i;
		}
		yield return null;
		splat3Tex.Apply(true, false);
		yield return null;
		splat4Tex.SetPixelData<TextureUtils.ColorARGB32>(splat4Cols.ToArray(), 0, 0);
		splat4Tex.Apply(true, false);
		yield return null;
		XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadProcessWorldWriting", false), true);
		SdFile.WriteAllBytes(_worldPath + "/splat3_processed.png", splat3Tex.EncodeToPNG());
		yield return GCUtils.UnloadAndCollectCo();
		SdFile.WriteAllBytes(_worldPath + "/splat4_processed.png", splat4Tex.EncodeToPNG());
		yield return GCUtils.UnloadAndCollectCo();
		Texture2D splat3Half = this.generateHalfResTexture(splat3Tex);
		Texture2D splat4Half = this.generateHalfResTexture(splat4Tex);
		yield return null;
		SdFile.WriteAllBytes(_worldPath + "/splat3_half.png", splat3Half.EncodeToPNG());
		yield return GCUtils.UnloadAndCollectCo();
		SdFile.WriteAllBytes(_worldPath + "/splat4_half.png", splat4Half.EncodeToPNG());
		yield return GCUtils.UnloadAndCollectCo();
		yield return this.calcWorldFileCrcs(_worldPath);
		this.saveFileHashes(_worldPath, this.worldFileCrcs);
		yield return GCUtils.UnloadAndCollectCo();
		Log.Out("Loading and creating dtm raw file took " + ms.ElapsedMilliseconds.ToString() + "ms");
		_callback(splat3Tex, splat3Half, splat4Tex, splat4Half, worldDecoratorPoiFromImage);
		yield break;
	}

	// Token: 0x06004CEA RID: 19690 RVA: 0x001E76E1 File Offset: 0x001E58E1
	[PublicizedFrom(EAccessModifier.Private)]
	public Texture2D generateHalfResTexture(Texture2D _tex)
	{
		if (_tex.mipmapCount < 1)
		{
			Log.Error("Attempted to generate half-res texture from a texture that does not have mip level 1. Returning the source texture instead.");
			return _tex;
		}
		Texture2D texture2D = new Texture2D(_tex.width >> 1, _tex.height >> 1);
		texture2D.SetPixels(_tex.GetPixels(1));
		texture2D.Apply();
		return texture2D;
	}

	// Token: 0x06004CEB RID: 19691 RVA: 0x001E7720 File Offset: 0x001E5920
	[PublicizedFrom(EAccessModifier.Private)]
	public string getFilenameDTM()
	{
		string fullPath = PathAbstractions.WorldsSearchPaths.GetLocation(this.levelName, null, null).FullPath;
		if (SdFile.Exists(fullPath + "/dtm_processed.raw"))
		{
			return fullPath + "/dtm_processed";
		}
		return fullPath + "/dtm";
	}

	// Token: 0x06004CEC RID: 19692 RVA: 0x001E7771 File Offset: 0x001E5971
	public override void ReloadAllChunks()
	{
		this.loadDTM(null);
		((TerrainFromRaw)this.m_TerrainGenerator).Init(this.heightMap, this.m_BiomeProvider, this.levelName, this.world.Seed);
		base.ReloadAllChunks();
	}

	// Token: 0x06004CED RID: 19693 RVA: 0x00075CC0 File Offset: 0x00073EC0
	public override EnumChunkProviderId GetProviderId()
	{
		return EnumChunkProviderId.ChunkDataDriven;
	}

	// Token: 0x06004CEE RID: 19694 RVA: 0x001E77B0 File Offset: 0x001E59B0
	public override bool GetWorldExtent(out Vector3i _minSize, out Vector3i _maxSize)
	{
		_minSize = new Vector3i(-this.heightMapWidth * this.heightMapScale / 2, 0, -this.heightMapHeight * this.heightMapScale / 2);
		_maxSize = new Vector3i(this.heightMapWidth * this.heightMapScale / 2, 255, this.heightMapHeight * this.heightMapScale / 2);
		return true;
	}

	// Token: 0x06004CEF RID: 19695 RVA: 0x001E7818 File Offset: 0x001E5A18
	public override Vector2i GetWorldSize()
	{
		return new Vector2i(this.heightMapWidth * this.heightMapScale, this.heightMapHeight * this.heightMapScale);
	}

	// Token: 0x06004CF0 RID: 19696 RVA: 0x001E783C File Offset: 0x001E5A3C
	public override int GetPOIBlockIdOverride(int x, int z)
	{
		if (this.poiFromImage == null)
		{
			return 0;
		}
		WorldGridCompressedData<byte> poi = this.poiFromImage.m_Poi;
		byte data;
		if (!poi.Contains(x, z) || (data = poi.GetData(x, z)) == 255 || data == 0)
		{
			return 0;
		}
		PoiMapElement poiForColor = this.world.Biomes.getPoiForColor((uint)data);
		if (poiForColor == null || (this.bFixedWaterLevel && poiForColor.m_BlockValue.Block.blockMaterial.IsLiquid))
		{
			return 0;
		}
		return poiForColor.m_BlockValue.type;
	}

	// Token: 0x06004CF1 RID: 19697 RVA: 0x001E78C2 File Offset: 0x001E5AC2
	public unsafe override IEnumerator FillOccupiedMap(int width, int height, DecoOccupiedMap _occupiedMap, List<PrefabInstance> overridePOIList = null)
	{
		MicroStopwatch mswYields = new MicroStopwatch();
		EnumDecoOccupied[] occupiedMap = _occupiedMap.GetData();
		int length = this.heightData.Length;
		mswYields.ResetAndRestart();
		int num3;
		if (this.m_Decorators[0] is WorldDecoratorPOIFromImage)
		{
			WorldDecoratorPOIFromImage worldDecoratorPOIFromImage = (WorldDecoratorPOIFromImage)this.m_Decorators[0];
			WorldGridCompressedData<byte> poi = worldDecoratorPOIFromImage.m_Poi;
			for (int y = 0; y < height; y = num3)
			{
				int num = y * width;
				int num2 = num + width;
				for (int i = num; i < num2; i++)
				{
					byte value = poi.colors.GetValue(i);
					if (value != 0 && value != 255)
					{
						occupiedMap[i] = EnumDecoOccupied.POI;
					}
				}
				if (mswYields.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
				{
					yield return null;
					mswYields.ResetAndRestart();
				}
				num3 = y + 1;
			}
			poi = null;
		}
		yield return null;
		DynamicPrefabDecorator dynamicPrefabDecorator = this.GetDynamicPrefabDecorator();
		overridePOIList = ((dynamicPrefabDecorator != null) ? dynamicPrefabDecorator.GetDynamicPrefabs() : overridePOIList);
		if (overridePOIList != null)
		{
			for (int j = 0; j < overridePOIList.Count; j++)
			{
				PrefabInstance prefabInstance = overridePOIList[j];
				_occupiedMap.SetArea(prefabInstance.boundingBoxPosition.x, prefabInstance.boundingBoxPosition.z, EnumDecoOccupied.POI, prefabInstance.boundingBoxSize.x, prefabInstance.boundingBoxSize.z);
			}
		}
		if (WorldBiomes.Instance.GetTotalBluffsCount() > 0)
		{
			IBiomeProvider biomeProvider = GameManager.Instance.World.ChunkCache.ChunkProvider.GetBiomeProvider();
			int num4 = -width / 2;
			int num5 = width / 2;
			int num6 = -height / 2;
			int num7 = height / 2;
			GameRandom gameRandom = Utils.RandomFromSeedOnPos(0, 0, GameManager.Instance.World.Seed);
			int num8 = 0;
			using (IBackedArrayView<ushort> backedArrayView = BackedArrays.CreateSingleView<ushort>(this.heightData, BackedArrayHandleMode.ReadWrite, 0))
			{
				for (int k = num6; k < num7; k++)
				{
					for (int l = num4; l < num5; l++)
					{
						float num9;
						BiomeDefinition biomeAt = biomeProvider.GetBiomeAt(l, k, out num9);
						if (biomeAt != null)
						{
							for (int m = 0; m < biomeAt.m_DecoBluffs.Count; m++)
							{
								BiomeBluffDecoration biomeBluffDecoration = biomeAt.m_DecoBluffs[m];
								if (gameRandom.RandomFloat <= biomeBluffDecoration.m_Prob)
								{
									ChunkProviderGenerateWorldFromRaw.Bluff bluff;
									if (!this.bluffs.TryGetValue(biomeAt.m_DecoBluffs[m].m_sName, out bluff))
									{
										bluff = ChunkProviderGenerateWorldFromRaw.Bluff.Load(biomeAt.m_DecoBluffs[m].m_sName);
										this.bluffs.Add(biomeAt.m_DecoBluffs[m].m_sName, bluff);
									}
									int num10 = gameRandom.RandomRange(3);
									Vector3i vector3i = new Vector3i(bluff.width, 1, bluff.height);
									for (int n = 0; n < num10; n++)
									{
										int x = vector3i.x;
										vector3i.x = vector3i.z;
										vector3i.z = x;
									}
									if (!_occupiedMap.CheckArea(l, k, EnumDecoOccupied.Stop_BigDeco, vector3i.x, vector3i.z))
									{
										_occupiedMap.SetArea(l, k, EnumDecoOccupied.Perimeter, vector3i.x, vector3i.z);
										float num11 = biomeAt.m_DecoBluffs[m].m_MinScale + gameRandom.RandomFloat * (biomeAt.m_DecoBluffs[m].m_MaxScale - biomeAt.m_DecoBluffs[m].m_MinScale);
										for (int num12 = 0; num12 < bluff.height; num12++)
										{
											for (int num13 = 0; num13 < bluff.width; num13++)
											{
												int num14;
												switch (num10)
												{
												case 1:
													num14 = num12 + (bluff.width - num13 - 1) * bluff.height;
													break;
												case 2:
													num14 = bluff.width - num13 - 1 + (bluff.height - num12 - 1) * bluff.width;
													break;
												case 3:
													num14 = bluff.height - num12 - 1 + num13 * bluff.height;
													break;
												default:
													num14 = num13 + num12 * bluff.width;
													break;
												}
												float num15 = bluff.data[num14] * num11;
												int i2 = (num13 + l + width / 2 + (num12 + k + height / 2) * width) % length;
												float num16 = (float)backedArrayView[i2] * 0.0038910506f + num15;
												if (num16 > 246f)
												{
													num16 = 246f;
												}
												backedArrayView[i2] = (ushort)(num16 / 0.0038910506f);
											}
										}
										num8++;
									}
								}
							}
						}
					}
				}
				GameRandomManager.Instance.FreeGameRandom(gameRandom);
			}
		}
		yield return null;
		mswYields.ResetAndRestart();
		int bigSlope = (int)(Mathf.Sin(1.134464f) / Mathf.Cos(1.134464f) / 1.5140275E-05f);
		int smallSlope = (int)(Mathf.Sin(0.83775806f) / Mathf.Cos(0.83775806f) / 1.5140275E-05f);
		ReadOnlyMemory<ushort> heightsRowMemory;
		IDisposable heightsRowMemoryHandle = this.heightData.GetReadOnlyMemory(0, width, out heightsRowMemory);
		IDisposable heightsNextRowMemoryHandle = null;
		int heightM = height - 1;
		int widthM = width - 1;
		for (int y = 0; y < heightM; y = num3)
		{
			int num17 = y * width;
			ReadOnlyMemory<ushort> readOnlyMemory;
			heightsNextRowMemoryHandle = this.heightData.GetReadOnlyMemory(num17 + width, width, out readOnlyMemory);
			ReadOnlySpan<ushort> span = heightsRowMemory.Span;
			ReadOnlySpan<ushort> span2 = readOnlyMemory.Span;
			Span<EnumDecoOccupied> span3 = occupiedMap.AsSpan(num17, widthM);
			int num18 = (int)(*span[0]);
			for (int num19 = 0; num19 < widthM; num19++)
			{
				int num20 = (int)(*span[num19 + 1]);
				int num21 = (int)(*span2[num19]);
				int num22 = num18 - num20;
				int num23 = num18 - num21;
				num18 = num20;
				int num24 = num22 * num22 + num23 * num23;
				if (num24 > smallSlope)
				{
					if (num24 > bigSlope)
					{
						*span3[num19] = EnumDecoOccupied.BigSlope;
					}
					else
					{
						*span3[num19] = EnumDecoOccupied.SmallSlope;
					}
				}
			}
			IDisposable disposable = heightsRowMemoryHandle;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			heightsRowMemory = readOnlyMemory;
			heightsRowMemoryHandle = heightsNextRowMemoryHandle;
			if (mswYields.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
			{
				yield return null;
				mswYields.ResetAndRestart();
			}
			num3 = y + 1;
		}
		IDisposable disposable2 = heightsRowMemoryHandle;
		if (disposable2 != null)
		{
			disposable2.Dispose();
		}
		IDisposable disposable3 = heightsNextRowMemoryHandle;
		if (disposable3 != null)
		{
			disposable3.Dispose();
		}
		yield break;
	}

	// Token: 0x06004CF2 RID: 19698 RVA: 0x001E78F0 File Offset: 0x001E5AF0
	public override float GetPOIHeightOverride(int x, int z)
	{
		WorldDecoratorPOIFromImage worldDecoratorPOIFromImage = this.m_Decorators[0] as WorldDecoratorPOIFromImage;
		if (worldDecoratorPOIFromImage == null)
		{
			return 0f;
		}
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

	// Token: 0x06004CF3 RID: 19699 RVA: 0x001E79A0 File Offset: 0x001E5BA0
	public float GetHeight(int x, int z)
	{
		float result;
		try
		{
			result = this.heightMap.GetAt(x, z);
		}
		catch (Exception)
		{
			Log.Error("Get Height Error x: {0} z: {1}", new object[]
			{
				x,
				z
			});
			result = 0f;
		}
		return result;
	}

	// Token: 0x06004CF4 RID: 19700 RVA: 0x001E79FC File Offset: 0x001E5BFC
	public override void Cleanup()
	{
		base.Cleanup();
		for (int i = 0; i < this.splats.Length; i++)
		{
			UnityEngine.Object.Destroy(this.splats[i]);
		}
		UnityEngine.Object.Destroy(this.procBiomeMask1);
		UnityEngine.Object.Destroy(this.procBiomeMask2);
		HeightMap heightMap = this.heightMap;
		if (heightMap != null)
		{
			heightMap.Dispose();
		}
		this.heightMap = null;
		IBackedArray<ushort> backedArray = this.heightData;
		if (backedArray != null)
		{
			backedArray.Dispose();
		}
		this.heightData = null;
		IBiomeProvider biomeProvider = this.m_BiomeProvider;
		if (biomeProvider == null)
		{
			return;
		}
		biomeProvider.Cleanup();
	}

	// Token: 0x06004CF5 RID: 19701 RVA: 0x001E7A88 File Offset: 0x001E5C88
	public void GetWaterChunks16x16(out int _water16x16ChunksW, out byte[] _water16x16Chunks)
	{
		WorldDecoratorPOIFromImage worldDecoratorPOIFromImage = this.m_Decorators[0] as WorldDecoratorPOIFromImage;
		if (worldDecoratorPOIFromImage == null)
		{
			_water16x16ChunksW = 0;
			_water16x16Chunks = null;
			return;
		}
		worldDecoratorPOIFromImage.GetWaterChunks16x16(out _water16x16ChunksW, out _water16x16Chunks);
	}

	// Token: 0x06004CF6 RID: 19702 RVA: 0x001E6A83 File Offset: 0x001E4C83
	public override ChunkProtectionLevel GetChunkProtectionLevel(Vector3i worldPos)
	{
		return this.m_RegionFileManager.GetChunkProtectionLevelForWorldPos(worldPos);
	}

	// Token: 0x04003AB6 RID: 15030
	public const string cRawProcessed = "_processed";

	// Token: 0x04003AB7 RID: 15031
	public const string cHalf = "_half";

	// Token: 0x04003AB8 RID: 15032
	public const int cMaxHeight = 255;

	// Token: 0x04003AB9 RID: 15033
	[PublicizedFrom(EAccessModifier.Private)]
	public int heightMapWidth = 4096;

	// Token: 0x04003ABA RID: 15034
	[PublicizedFrom(EAccessModifier.Private)]
	public int heightMapHeight = 4096;

	// Token: 0x04003ABB RID: 15035
	[PublicizedFrom(EAccessModifier.Private)]
	public int heightMapScale = 1;

	// Token: 0x04003ABC RID: 15036
	public const float hmFac = 0.0038910506f;

	// Token: 0x04003ABD RID: 15037
	public IBackedArray<ushort> heightData;

	// Token: 0x04003ABE RID: 15038
	public WorldDecoratorPOIFromImage poiFromImage;

	// Token: 0x04003ABF RID: 15039
	[PublicizedFrom(EAccessModifier.Private)]
	public HeightMap heightMap;

	// Token: 0x04003AC0 RID: 15040
	public Texture2D[] splats = new Texture2D[7];

	// Token: 0x04003AC1 RID: 15041
	public Texture2D procBiomeMask1;

	// Token: 0x04003AC2 RID: 15042
	public Texture2D procBiomeMask2;

	// Token: 0x04003AC3 RID: 15043
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, ChunkProviderGenerateWorldFromRaw.Bluff> bluffs = new Dictionary<string, ChunkProviderGenerateWorldFromRaw.Bluff>();

	// Token: 0x04003AC4 RID: 15044
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bFixedWaterLevel;

	// Token: 0x04003AC5 RID: 15045
	public readonly Dictionary<string, uint> worldFileCrcs = new CaseInsensitiveStringDictionary<uint>();

	// Token: 0x04003AC7 RID: 15047
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string[] FilesUsedForProcessing = new string[]
	{
		"prefabs.xml",
		"dtm.raw",
		"splat3.tga",
		"splat3.png",
		"water_info.xml",
		"main.ttw",
		"biomes.png"
	};

	// Token: 0x020009CB RID: 2507
	[PublicizedFrom(EAccessModifier.Private)]
	public class Bluff
	{
		// Token: 0x06004CF9 RID: 19705 RVA: 0x001E7B0C File Offset: 0x001E5D0C
		public static ChunkProviderGenerateWorldFromRaw.Bluff Load(string _name)
		{
			Texture2D texture2D = TextureUtils.LoadTexture(GameIO.GetGameDir("Data/Bluffs") + "/" + _name + ".tga", FilterMode.Point, false, false, null);
			Color32[] pixels = texture2D.GetPixels32();
			ChunkProviderGenerateWorldFromRaw.Bluff bluff = new ChunkProviderGenerateWorldFromRaw.Bluff();
			bluff.width = texture2D.width;
			bluff.height = texture2D.height;
			bluff.data = new float[bluff.width * bluff.height];
			for (int i = pixels.Length - 1; i >= 0; i--)
			{
				bluff.data[i] = (float)pixels[i].r;
			}
			UnityEngine.Object.Destroy(texture2D);
			return bluff;
		}

		// Token: 0x04003AC8 RID: 15048
		public int width;

		// Token: 0x04003AC9 RID: 15049
		public int height;

		// Token: 0x04003ACA RID: 15050
		public float[] data;
	}

	// Token: 0x020009CC RID: 2508
	// (Invoke) Token: 0x06004CFC RID: 19708
	[PublicizedFrom(EAccessModifier.Private)]
	public delegate void ProcessFilesCallback(Texture2D _splat3Tex, Texture2D _splat3Visual, Texture2D _splat4Tex, Texture2D _splat4Visual, WorldDecoratorPOIFromImage _decoratorPoiFromImage);
}
