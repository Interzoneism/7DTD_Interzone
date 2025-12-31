using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000A60 RID: 2656
public class UnityDistantTerrain
{
	// Token: 0x060050DF RID: 20703 RVA: 0x0020250C File Offset: 0x0020070C
	public void Init(GameObject _parent, UnityDistantTerrain.Config _terrainConfig, int _visibleChunks, Material _terrainMaterial, Material _waterMaterial, int _waterChunks16x16Width, byte[] _waterChunks16x16, ITileArea<float[,]> _heights, TileArea<Color32[]> _splat0, TileArea<Color32[]> _splat1, TileArea<Color32[]> _splat2)
	{
		Transform transform = _parent.transform;
		this.terrainConfig = _terrainConfig;
		this.visibleChunks = _visibleChunks;
		this.terrainMaterial = _terrainMaterial;
		this.waterMaterial = _waterMaterial;
		this.terrainHeights = _heights;
		this.splat0Arr = _splat0;
		this.splat1Arr = _splat1;
		this.splat2Arr = _splat2;
		this.waterChunks16x16Width = _waterChunks16x16Width;
		this.waterChunks16x16 = _waterChunks16x16;
		this.cacheParentObj = new GameObject("Cache");
		this.cacheParentT = this.cacheParentObj.transform;
		this.cacheParentT.SetParent(transform, false);
		this.cacheParentObj.SetActive(false);
		this.visibleParentObj = new GameObject("Terrain");
		this.visibleParentT = this.visibleParentObj.transform;
		this.visibleParentT.SetParent(transform, false);
		this.visibleParentT.localPosition = new Vector3(0.5f, 0.5f, 0.5f);
		this.terrainTiles = new TileArea<UnityDistantTerrain.TerrainAndWater>(this.terrainHeights.Config, null);
		for (int i = 0; i < this.visibleChunks * this.visibleChunks; i++)
		{
			this.AddTerrainAndWaterToCache();
		}
		if (this.splat0Arr != null)
		{
			for (int j = 0; j < this.splatMapCache.Length; j++)
			{
				this.splatMapCache[j] = new TileArea<Texture2D>(this.splat0Arr.config, null);
			}
		}
		this.waterPlane = new UnityDistantTerrainWaterPlane(_terrainConfig, _waterMaterial);
		Origin.Add(transform, -1);
		transform.position = -Origin.position;
	}

	// Token: 0x060050E0 RID: 20704 RVA: 0x00202684 File Offset: 0x00200884
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddTerrainAndWaterToCache()
	{
		GameObject gameObject = new GameObject("Terrain");
		gameObject.transform.SetParent(this.cacheParentT, false);
		Terrain terrain = gameObject.AddComponent<Terrain>();
		GameObject gameObject2 = new GameObject("Water");
		gameObject2.transform.SetParent(terrain.transform, false);
		gameObject2.transform.localPosition = new Vector3(0f, 0.25f, 0f);
		gameObject2.AddComponent<MeshFilter>();
		gameObject2.AddComponent<MeshRenderer>();
		this.terrainCache.Add(new UnityDistantTerrain.TerrainAndWater
		{
			terrain = terrain,
			waterPlane = gameObject2
		});
		TerrainData terrainData = new TerrainData();
		terrainData.heightmapResolution = this.terrainConfig.DataTileSize / this.terrainConfig.DataSteps + 1;
		terrainData.size = new Vector3((float)this.terrainConfig.DataTileSize, (float)this.terrainConfig.MaxHeight, (float)this.terrainConfig.DataTileSize);
		if (this.terrainMaterial != null)
		{
			terrain.materialTemplate = this.terrainMaterial;
		}
		terrain.terrainData = terrainData;
		terrain.drawInstanced = false;
		terrain.drawTreesAndFoliage = false;
		terrain.heightmapMaximumLOD = 0;
		terrain.heightmapPixelError = (float)this.terrainConfig.PixelError;
		int num = Mathf.Clamp(GamePrefs.GetInt(EnumGamePrefs.OptionsGfxTerrainQuality), 0, UnityDistantTerrain.basemapDistances.Length - 1);
		float basemapDistance = UnityDistantTerrain.basemapDistances[num];
		terrain.basemapDistance = basemapDistance;
		gameObject.SetActive(false);
	}

	// Token: 0x060050E1 RID: 20705 RVA: 0x002027EC File Offset: 0x002009EC
	public void Cleanup()
	{
		if (this.visibleParentT)
		{
			Origin.Remove(this.visibleParentT.parent);
		}
		for (int i = 0; i < this.splatMapCache.Length; i++)
		{
			TileArea<Texture2D> tileArea = this.splatMapCache[i];
			if (tileArea != null)
			{
				foreach (KeyValuePair<uint, Texture2D> keyValuePair in tileArea.Data)
				{
					UnityEngine.Object.Destroy(keyValuePair.Value);
				}
				this.splatMapCache[i] = null;
			}
		}
		UnityEngine.Object.Destroy(this.cacheParentObj);
		UnityEngine.Object.Destroy(this.visibleParentObj);
		this.terrainCache.Clear();
		UnityEngine.Object.Destroy(this.terrainClipTexture);
		this.terrainClipTexture = null;
		this.visibleVoxelChunks.Clear();
		this.waterPlane.Cleanup();
		this.isAnyTerrainDirty = false;
		this.terrainHeights.Cleanup();
	}

	// Token: 0x060050E2 RID: 20706 RVA: 0x002028E4 File Offset: 0x00200AE4
	public void FrameUpdate(EntityPlayerLocal _player)
	{
		this.BuildAroundPos(_player.position);
		this.UpdateChunks();
		this.UpdateTextureApply();
	}

	// Token: 0x060050E3 RID: 20707 RVA: 0x002028FF File Offset: 0x00200AFF
	[PublicizedFrom(EAccessModifier.Private)]
	public void BuildAroundPos(Vector3 _position)
	{
		this.tempPositions.Clear();
		this.tempPositions.Add(_position);
		this.BuildAroundPos(this.tempPositions);
	}

	// Token: 0x060050E4 RID: 20708 RVA: 0x00202924 File Offset: 0x00200B24
	[PublicizedFrom(EAccessModifier.Private)]
	public void BuildAroundPos(List<Vector3> _positions)
	{
		int chunkWorldSize = this.terrainConfig.ChunkWorldSize;
		float num = 0f;
		int num2 = (this.visibleChunks - 1) / 2;
		int num3 = num2;
		if ((this.visibleChunks & 1) == 0)
		{
			num = (float)chunkWorldSize * -0.5f;
			num3++;
		}
		this.visibleTerrainTilesOfObservers.Clear();
		for (int i = 0; i < _positions.Count; i++)
		{
			int num4 = Utils.Fastfloor((_positions[i].x + num) / (float)chunkWorldSize);
			int num5 = Utils.Fastfloor((_positions[i].z + num) / (float)chunkWorldSize);
			int num6 = num4 - num2;
			int num7 = num4 + num3;
			int num8 = num5 - num2;
			int num9 = num5 + num3;
			for (int j = num6; j <= num7; j++)
			{
				for (int k = num8; k <= num9; k++)
				{
					this.visibleTerrainTilesOfObservers.Add(TileAreaUtils.MakeKey(j, k));
				}
			}
		}
		this.tempKeysToRemove.Clear();
		foreach (KeyValuePair<uint, UnityDistantTerrain.TerrainAndWater> keyValuePair in this.terrainTiles.Data)
		{
			if (!this.visibleTerrainTilesOfObservers.Contains(keyValuePair.Key))
			{
				this.tempKeysToRemove.Add(keyValuePair.Key);
			}
		}
		for (int l = 0; l < this.tempKeysToRemove.Count; l++)
		{
			uint key = this.tempKeysToRemove[l];
			UnityDistantTerrain.TerrainAndWater terrainAndWater = this.terrainTiles[key];
			this.terrainTiles.Remove(key);
			this.terrainCache.Add(terrainAndWater);
			terrainAndWater.waterPlane.gameObject.SetActive(false);
			terrainAndWater.terrain.gameObject.SetActive(false);
			terrainAndWater.terrain.transform.SetParent(this.cacheParentT, false);
		}
		bool flag = false;
		int num10 = -(this.terrainConfig.DataWidth / this.terrainConfig.DataTileSize) / 2;
		int num11 = this.terrainConfig.DataWidth / this.terrainConfig.DataTileSize / 2 - 1;
		num11 = Utils.FastMax(0, num11);
		int num12 = -(this.terrainConfig.DataHeight / this.terrainConfig.DataTileSize) / 2;
		int num13 = this.terrainConfig.DataHeight / this.terrainConfig.DataTileSize / 2 - 1;
		num13 = Utils.FastMax(0, num13);
		float num14 = (float)chunkWorldSize * 0.5f;
		float num15 = UnityDistantTerrain.pixelErrorDistanceDiv[GamePrefs.GetInt(EnumGamePrefs.OptionsGfxTerrainQuality)];
		foreach (uint key2 in this.visibleTerrainTilesOfObservers)
		{
			int tileXPos = TileAreaUtils.GetTileXPos(key2);
			int tileZPos = TileAreaUtils.GetTileZPos(key2);
			UnityDistantTerrain.TerrainAndWater terrainAndWater2 = this.terrainTiles[key2];
			if (terrainAndWater2 != null)
			{
				float num16 = float.MaxValue;
				for (int m = 0; m < _positions.Count; m++)
				{
					float num17 = _positions[m].x - ((float)(tileXPos * chunkWorldSize) + num14);
					float num18 = _positions[m].z - ((float)(tileZPos * chunkWorldSize) + num14);
					float num19 = num17 * num17 + num18 * num18;
					if (num19 < num16)
					{
						num16 = num19;
					}
				}
				float num20 = 5f;
				if (num16 > 102400f)
				{
					float num21 = Mathf.Sqrt(num16);
					num20 += (num21 - 320f) / num15;
				}
				terrainAndWater2.terrain.heightmapPixelError = (float)((int)num20);
			}
			else if (tileXPos >= num10 && tileXPos <= num11 && tileZPos >= num12 && tileZPos <= num13)
			{
				terrainAndWater2 = this.CreateAndConfigureTerrain(tileXPos, tileZPos);
				terrainAndWater2.terrain.transform.SetParent(this.visibleParentT, false);
				this.terrainTiles[key2] = terrainAndWater2;
				flag = true;
				this.isNeighborsDirty = true;
				break;
			}
		}
		if (!flag && this.isNeighborsDirty)
		{
			this.isNeighborsDirty = false;
			foreach (uint key3 in this.visibleTerrainTilesOfObservers)
			{
				UnityDistantTerrain.TerrainAndWater terrainAndWater3 = this.terrainTiles[key3];
				if (terrainAndWater3 != null)
				{
					int tileXPos2 = TileAreaUtils.GetTileXPos(key3);
					int tileZPos2 = TileAreaUtils.GetTileZPos(key3);
					UnityDistantTerrain.TerrainAndWater terrainAndWater4 = this.terrainTiles[TileAreaUtils.MakeKey(tileXPos2 - 1, tileZPos2)];
					UnityDistantTerrain.TerrainAndWater terrainAndWater5 = this.terrainTiles[TileAreaUtils.MakeKey(tileXPos2, tileZPos2 + 1)];
					UnityDistantTerrain.TerrainAndWater terrainAndWater6 = this.terrainTiles[TileAreaUtils.MakeKey(tileXPos2 + 1, tileZPos2)];
					UnityDistantTerrain.TerrainAndWater terrainAndWater7 = this.terrainTiles[TileAreaUtils.MakeKey(tileXPos2, tileZPos2 - 1)];
					terrainAndWater3.terrain.SetNeighbors((terrainAndWater4 != null) ? terrainAndWater4.terrain : null, (terrainAndWater5 != null) ? terrainAndWater5.terrain : null, (terrainAndWater6 != null) ? terrainAndWater6.terrain : null, (terrainAndWater7 != null) ? terrainAndWater7.terrain : null);
				}
			}
		}
		DynamicMeshManager.UpdateDistantTerrainBounds(this.terrainTiles, this.terrainConfig);
	}

	// Token: 0x060050E5 RID: 20709 RVA: 0x00202E70 File Offset: 0x00201070
	[PublicizedFrom(EAccessModifier.Private)]
	public UnityDistantTerrain.TerrainAndWater GetFromCacheOrCreate()
	{
		if (this.terrainCache.Count == 0)
		{
			this.AddTerrainAndWaterToCache();
		}
		UnityDistantTerrain.TerrainAndWater result = this.terrainCache[this.terrainCache.Count - 1];
		this.terrainCache.RemoveAt(this.terrainCache.Count - 1);
		return result;
	}

	// Token: 0x060050E6 RID: 20710 RVA: 0x00202EC0 File Offset: 0x002010C0
	[PublicizedFrom(EAccessModifier.Private)]
	public UnityDistantTerrain.TerrainAndWater CreateAndConfigureTerrain(int cx, int cz)
	{
		UnityDistantTerrain.TerrainAndWater fromCacheOrCreate = this.GetFromCacheOrCreate();
		Terrain terrain = fromCacheOrCreate.terrain;
		GameObject gameObject = terrain.gameObject;
		gameObject.name = "Terrain " + cx.ToString() + "/" + cz.ToString();
		terrain.transform.localPosition = new Vector3((float)(cx * this.terrainConfig.ChunkWorldSize), 0f, (float)(cz * this.terrainConfig.ChunkWorldSize));
		float[,] heights = this.terrainHeights[cx, cz];
		terrain.terrainData.SetHeights(0, 0, heights);
		gameObject.SetActive(true);
		if (this.splat0Arr != null)
		{
			Material materialTemplate = terrain.materialTemplate;
			int num = this.terrainConfig.DataTileSize / this.terrainConfig.SplatSteps + 2;
			int height = num;
			Texture2D texture2D = this.splatMapCache[0][cx, cz];
			if (texture2D == null)
			{
				texture2D = new Texture2D(num, height, TextureFormat.RGBA32, true);
				texture2D.SetPixels32(this.splat0Arr[cx, cz]);
				texture2D.filterMode = FilterMode.Bilinear;
				texture2D.wrapMode = TextureWrapMode.Clamp;
				texture2D.name = "Splat0 " + cx.ToString() + "/" + cz.ToString();
				texture2D.Apply(true, true);
				this.splatMapCache[0][cx, cz] = texture2D;
			}
			Texture2D texture2D2 = this.splatMapCache[1][cx, cz];
			if (texture2D2 == null)
			{
				texture2D2 = new Texture2D(num, height, TextureFormat.RGBA32, true);
				texture2D2.SetPixels32(this.splat1Arr[cx, cz]);
				texture2D2.filterMode = FilterMode.Bilinear;
				texture2D2.wrapMode = TextureWrapMode.Clamp;
				texture2D2.name = "Splat1 " + cx.ToString() + "/" + cz.ToString();
				texture2D2.Apply(true, true);
				this.splatMapCache[1][cx, cz] = texture2D2;
			}
			Texture2D texture2D3 = this.splatMapCache[2][cx, cz];
			if (texture2D3 == null)
			{
				texture2D3 = new Texture2D(num, height, TextureFormat.RGBA32, true);
				texture2D3.SetPixels32(this.splat2Arr[cx, cz]);
				texture2D3.filterMode = FilterMode.Bilinear;
				texture2D3.wrapMode = TextureWrapMode.Clamp;
				texture2D3.name = "Splat2 " + cx.ToString() + "/" + cz.ToString();
				texture2D3.Apply(true, true);
				this.splatMapCache[2][cx, cz] = texture2D3;
			}
			materialTemplate.SetTexture("_CustomControl0", texture2D);
			materialTemplate.SetTexture("_CustomControl1", texture2D2);
			materialTemplate.SetTexture("_CustomControl2", texture2D3);
		}
		if (this.waterChunks16x16Width != 0)
		{
			this.waterPlane.createDynamicWaterPlane_Step1((cx - this.terrainHeights.Config.tileStart.x) * this.terrainConfig.ChunkWorldSize, (cz - this.terrainHeights.Config.tileStart.y) * this.terrainConfig.ChunkWorldSize, this.terrainConfig.ChunkWorldSize, this.waterChunks16x16Width, this.waterChunks16x16);
			this.waterPlane.createDynamicWaterPlane_Step2(fromCacheOrCreate.waterPlane, terrain.transform.parent, null);
		}
		return fromCacheOrCreate;
	}

	// Token: 0x060050E7 RID: 20711 RVA: 0x002031D8 File Offset: 0x002013D8
	public void OnChunkUpdate(int _chunkX, int _chunkZ, bool _visible)
	{
		int num = this.terrainConfig.DataWidth / 16;
		if (_chunkX < -num / 2 || _chunkX >= num / 2)
		{
			return;
		}
		int num2 = this.terrainConfig.DataHeight / 16;
		if (_chunkZ < -num2 / 2 || _chunkZ >= num2 / 2)
		{
			return;
		}
		uint num3 = TileAreaUtils.MakeKey(_chunkX, _chunkZ);
		UnityDistantTerrain.VoxeChunkInfo voxeChunkInfo;
		if (!this.dictChunkHeightsArr.TryGetValue(num3, out voxeChunkInfo))
		{
			voxeChunkInfo = new UnityDistantTerrain.VoxeChunkInfo();
			this.dictChunkHeightsArr[num3] = voxeChunkInfo;
		}
		if (_visible)
		{
			this.visibleVoxelChunks.Add(num3);
			voxeChunkInfo.a3 = 1;
		}
		else
		{
			this.visibleVoxelChunks.Remove(num3);
			voxeChunkInfo.a3 = 0;
		}
		voxeChunkInfo.isDirty = true;
		this.isAnyTerrainDirty = true;
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < this.posAround.Length; j++)
			{
				this.UpdateVoxelChunkInfo(_chunkX + this.posAround[j].x, _chunkZ + this.posAround[j].z, this.visibleVoxelChunks);
			}
		}
		this.UpdateTerrainTextureData(_chunkX, _chunkZ, _visible);
	}

	// Token: 0x060050E8 RID: 20712 RVA: 0x002032EC File Offset: 0x002014EC
	public void UpdateChunks()
	{
		if (!this.isAnyTerrainDirty)
		{
			return;
		}
		this.isAnyTerrainDirty = false;
		this.toRemove.Clear();
		this.toUpdateTerrain.Clear();
		foreach (KeyValuePair<uint, UnityDistantTerrain.VoxeChunkInfo> keyValuePair in this.dictChunkHeightsArr)
		{
			UnityDistantTerrain.VoxeChunkInfo value = keyValuePair.Value;
			if (value.isDirty)
			{
				int tileXPos = TileAreaUtils.GetTileXPos(keyValuePair.Key);
				int tileZPos = TileAreaUtils.GetTileZPos(keyValuePair.Key);
				if (this.UpdateChunkHeights(tileXPos, tileZPos, value))
				{
					value.isDirty = false;
				}
			}
			if (!value.isDirty && value.IsEmpty())
			{
				this.toRemove.Add(keyValuePair.Key);
			}
		}
		for (int i = 0; i < this.toRemove.Count; i++)
		{
			this.dictChunkHeightsArr.Remove(this.toRemove[i]);
		}
		foreach (Terrain terrain in this.toUpdateTerrain)
		{
			terrain.terrainData.SyncHeightmap();
		}
		this.toUpdateTerrain.Clear();
	}

	// Token: 0x060050E9 RID: 20713 RVA: 0x00203444 File Offset: 0x00201644
	[PublicizedFrom(EAccessModifier.Private)]
	public bool UpdateTerrainTextureData(int chunkX, int chunkZ, bool _bVisible)
	{
		int num = this.terrainConfig.DataWidth / 16;
		int num2 = this.terrainConfig.DataHeight / 16;
		if (this.terrainClipTexture == null)
		{
			this.terrainClipTexture = new Texture2D(num, num2, TextureFormat.RGB24, false);
			this.terrainClipTexture.wrapMode = TextureWrapMode.Clamp;
			this.terrainClipTexture.filterMode = FilterMode.Point;
			this.terrainClipCols = new Color32[num * num2];
			for (int i = 0; i < this.terrainClipCols.Length; i++)
			{
				this.terrainClipCols[i] = this.colNoClip;
			}
			this.waterMaterial.SetTexture("_ClipChunks", this.terrainClipTexture);
		}
		int num3 = num / 2;
		int num4 = num2 / 2;
		foreach (KeyValuePair<uint, UnityDistantTerrain.VoxeChunkInfo> keyValuePair in this.dictChunkHeightsArr)
		{
			UnityDistantTerrain.VoxeChunkInfo value = keyValuePair.Value;
			Color32 color;
			if (value.a0 == 2 && value.a1 == 2 && value.a2 == 2 && value.a3 == 2)
			{
				color = this.colClipTerrainAndWater;
			}
			else if (value.a3 != 0)
			{
				color = this.colClipWater;
			}
			else
			{
				color = this.colNoClip;
			}
			int tileXPos = TileAreaUtils.GetTileXPos(keyValuePair.Key);
			int tileZPos = TileAreaUtils.GetTileZPos(keyValuePair.Key);
			this.terrainClipCols[tileXPos + num3 + (tileZPos + num4) * num2] = color;
		}
		this.isClipTextureDirty = true;
		return true;
	}

	// Token: 0x060050EA RID: 20714 RVA: 0x002035D4 File Offset: 0x002017D4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool UpdateTextureApply()
	{
		if (!this.isClipTextureDirty)
		{
			return false;
		}
		this.isClipTextureDirty = false;
		this.terrainClipTexture.SetPixels32(this.terrainClipCols);
		this.terrainClipTexture.Apply();
		return true;
	}

	// Token: 0x060050EB RID: 20715 RVA: 0x00203604 File Offset: 0x00201804
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateVoxelChunkInfo(int _chunkX, int _chunkZ, HashSet<uint> _visibleVoxelChunks)
	{
		int num = this.terrainConfig.DataWidth / 16;
		int num2 = this.terrainConfig.DataHeight / 16;
		if (_chunkX < -num / 2 || _chunkZ < -num2 / 2 || _chunkX >= num / 2 || _chunkZ >= num2 / 2)
		{
			return;
		}
		uint key = TileAreaUtils.MakeKey(_chunkX, _chunkZ);
		UnityDistantTerrain.VoxeChunkInfo voxeChunkInfo;
		if (!this.dictChunkHeightsArr.TryGetValue(key, out voxeChunkInfo))
		{
			voxeChunkInfo = new UnityDistantTerrain.VoxeChunkInfo();
			this.dictChunkHeightsArr[key] = voxeChunkInfo;
		}
		int tileX = _chunkX - 1;
		int tileX2 = _chunkX + 1;
		int tileZ = _chunkZ - 1;
		int tileZ2 = _chunkZ + 1;
		bool flag = voxeChunkInfo.a3 > 0 && _visibleVoxelChunks.Contains(TileAreaUtils.MakeKey(tileX, _chunkZ));
		if (flag)
		{
			if (voxeChunkInfo.a2 == 0)
			{
				voxeChunkInfo.a2 = 1;
				voxeChunkInfo.isDirty = true;
				this.isAnyTerrainDirty = true;
			}
		}
		else if (voxeChunkInfo.a2 != 0)
		{
			voxeChunkInfo.a2 = 0;
			voxeChunkInfo.isDirty = true;
			this.isAnyTerrainDirty = true;
		}
		bool flag2 = voxeChunkInfo.a3 > 0 && _visibleVoxelChunks.Contains(TileAreaUtils.MakeKey(_chunkX, tileZ));
		if (flag2)
		{
			if (voxeChunkInfo.a1 == 0)
			{
				voxeChunkInfo.a1 = 1;
				voxeChunkInfo.isDirty = (this.isAnyTerrainDirty = true);
			}
		}
		else if (voxeChunkInfo.a1 != 0)
		{
			voxeChunkInfo.a1 = 0;
			voxeChunkInfo.isDirty = (this.isAnyTerrainDirty = true);
		}
		if (voxeChunkInfo.a3 > 0 && flag2 && flag && _visibleVoxelChunks.Contains(TileAreaUtils.MakeKey(tileX, tileZ)))
		{
			if (voxeChunkInfo.a0 == 0)
			{
				voxeChunkInfo.a0 = 1;
				voxeChunkInfo.isDirty = (this.isAnyTerrainDirty = true);
			}
		}
		else if (voxeChunkInfo.a0 != 0)
		{
			voxeChunkInfo.a0 = 0;
			voxeChunkInfo.isDirty = (this.isAnyTerrainDirty = true);
		}
		UnityDistantTerrain.VoxeChunkInfo voxeChunkInfo2;
		UnityDistantTerrain.VoxeChunkInfo voxeChunkInfo3;
		UnityDistantTerrain.VoxeChunkInfo voxeChunkInfo4;
		if (voxeChunkInfo.a0 >= 1 && voxeChunkInfo.a1 > 0 && voxeChunkInfo.a2 > 0 && voxeChunkInfo.a3 > 0 && this.dictChunkHeightsArr.TryGetValue(TileAreaUtils.MakeKey(tileX, _chunkZ), out voxeChunkInfo2) && voxeChunkInfo2.a1 > 0 && voxeChunkInfo2.a3 > 0 && this.dictChunkHeightsArr.TryGetValue(TileAreaUtils.MakeKey(tileX, tileZ), out voxeChunkInfo3) && voxeChunkInfo3.a3 > 0 && this.dictChunkHeightsArr.TryGetValue(TileAreaUtils.MakeKey(_chunkX, tileZ), out voxeChunkInfo4) && voxeChunkInfo4.a2 > 0 && voxeChunkInfo4.a3 > 0)
		{
			if (voxeChunkInfo.a0 != 2)
			{
				voxeChunkInfo.a0 = 2;
				voxeChunkInfo.isDirty = (this.isAnyTerrainDirty = true);
			}
		}
		else if (voxeChunkInfo.a0 == 2)
		{
			voxeChunkInfo.a0 = 1;
			voxeChunkInfo.isDirty = (this.isAnyTerrainDirty = true);
		}
		UnityDistantTerrain.VoxeChunkInfo voxeChunkInfo5;
		UnityDistantTerrain.VoxeChunkInfo voxeChunkInfo6;
		flag2 = (voxeChunkInfo.a1 >= 1 && voxeChunkInfo.a0 > 0 && voxeChunkInfo.a2 > 0 && voxeChunkInfo.a3 > 0 && this.dictChunkHeightsArr.TryGetValue(TileAreaUtils.MakeKey(tileX2, _chunkZ), out voxeChunkInfo5) && voxeChunkInfo5.a0 > 0 && voxeChunkInfo5.a2 > 0 && this.dictChunkHeightsArr.TryGetValue(TileAreaUtils.MakeKey(_chunkX, tileZ), out voxeChunkInfo4) && voxeChunkInfo4.a2 > 0 && voxeChunkInfo4.a3 > 0 && this.dictChunkHeightsArr.TryGetValue(TileAreaUtils.MakeKey(tileX2, tileZ), out voxeChunkInfo6) && voxeChunkInfo6.a2 > 0);
		if (flag2)
		{
			if (voxeChunkInfo.a1 != 2)
			{
				voxeChunkInfo.a1 = 2;
				voxeChunkInfo.isDirty = (this.isAnyTerrainDirty = true);
			}
		}
		else if (voxeChunkInfo.a1 == 2)
		{
			voxeChunkInfo.a1 = 1;
			voxeChunkInfo.isDirty = (this.isAnyTerrainDirty = true);
		}
		UnityDistantTerrain.VoxeChunkInfo voxeChunkInfo7;
		UnityDistantTerrain.VoxeChunkInfo voxeChunkInfo8;
		flag = (voxeChunkInfo.a2 >= 1 && voxeChunkInfo.a0 > 0 && voxeChunkInfo.a1 > 0 && voxeChunkInfo.a3 > 0 && this.dictChunkHeightsArr.TryGetValue(TileAreaUtils.MakeKey(tileX, _chunkZ), out voxeChunkInfo2) && voxeChunkInfo2.a3 > 0 && voxeChunkInfo2.a1 > 0 && this.dictChunkHeightsArr.TryGetValue(TileAreaUtils.MakeKey(tileX, tileZ2), out voxeChunkInfo7) && voxeChunkInfo7.a1 > 0 && this.dictChunkHeightsArr.TryGetValue(TileAreaUtils.MakeKey(_chunkX, tileZ2), out voxeChunkInfo8) && voxeChunkInfo8.a0 > 0 && voxeChunkInfo8.a1 > 0);
		if (flag)
		{
			if (voxeChunkInfo.a2 != 2)
			{
				voxeChunkInfo.a2 = 2;
				voxeChunkInfo.isDirty = (this.isAnyTerrainDirty = true);
			}
		}
		else if (voxeChunkInfo.a2 == 2)
		{
			voxeChunkInfo.a2 = 1;
			voxeChunkInfo.isDirty = (this.isAnyTerrainDirty = true);
		}
		UnityDistantTerrain.VoxeChunkInfo voxeChunkInfo9;
		if (voxeChunkInfo.a3 >= 1 && voxeChunkInfo.a0 > 0 && voxeChunkInfo.a1 > 0 && voxeChunkInfo.a2 > 0 && this.dictChunkHeightsArr.TryGetValue(TileAreaUtils.MakeKey(tileX2, _chunkZ), out voxeChunkInfo5) && voxeChunkInfo5.a2 > 0 && voxeChunkInfo5.a0 > 0 && this.dictChunkHeightsArr.TryGetValue(TileAreaUtils.MakeKey(tileX2, tileZ2), out voxeChunkInfo9) && voxeChunkInfo9.a0 > 0 && this.dictChunkHeightsArr.TryGetValue(TileAreaUtils.MakeKey(_chunkX, tileZ2), out voxeChunkInfo8) && voxeChunkInfo8.a0 > 0 && voxeChunkInfo8.a1 > 0)
		{
			if (voxeChunkInfo.a3 != 2)
			{
				voxeChunkInfo.a3 = 2;
				voxeChunkInfo.isDirty = (this.isAnyTerrainDirty = true);
				return;
			}
		}
		else if (voxeChunkInfo.a3 == 2)
		{
			voxeChunkInfo.a3 = 1;
			voxeChunkInfo.isDirty = (this.isAnyTerrainDirty = true);
		}
	}

	// Token: 0x060050EC RID: 20716 RVA: 0x00203B8C File Offset: 0x00201D8C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool UpdateChunkHeights(int _chunkX, int _chunkZ, UnityDistantTerrain.VoxeChunkInfo _vciMiddle)
	{
		int num = _chunkX * 16;
		int num2 = _chunkZ * 16;
		int chunkWorldSize = this.terrainConfig.ChunkWorldSize;
		int num3 = Utils.Fastfloor((float)num / (float)chunkWorldSize);
		int num4 = Utils.Fastfloor((float)num2 / (float)chunkWorldSize);
		uint key = TileAreaUtils.MakeKey(num3, num4);
		UnityDistantTerrain.TerrainAndWater terrainAndWater = this.terrainTiles[key];
		if (terrainAndWater == null)
		{
			return false;
		}
		float[,] array = this.terrainHeights[num3, num4];
		if (array == null)
		{
			return false;
		}
		int num5 = num - num3 * chunkWorldSize;
		int num6 = num2 - num4 * chunkWorldSize;
		int num7 = num5 / this.terrainConfig.MetersPerHeightPix;
		num6 /= this.terrainConfig.MetersPerHeightPix;
		int num8 = 16 / this.terrainConfig.MetersPerHeightPix;
		int num9 = num7 / num8 * num8;
		int num10 = num6 / num8 * num8;
		UnityDistantTerrain.VoxeChunkInfo voxeChunkInfo = null;
		UnityDistantTerrain.VoxeChunkInfo voxeChunkInfo2 = null;
		UnityDistantTerrain.VoxeChunkInfo voxeChunkInfo3 = null;
		float num11 = array[num10, num9];
		if (_vciMiddle.a0 == 1)
		{
			num11 -= 0.0025f;
		}
		else if (_vciMiddle.a0 == 2)
		{
			num11 = 0f;
		}
		this.heights8x8[0, 0] = num11;
		for (int i = 1; i < 8; i++)
		{
			num11 = array[num10, num9 + i];
			if (_vciMiddle.a1 == 1)
			{
				num11 -= 0.0025f;
			}
			else if (_vciMiddle.a1 == 2)
			{
				num11 = 0f;
			}
			this.heights8x8[0, i] = num11;
		}
		num11 = array[num10, num9 + 8];
		if (voxeChunkInfo != null || this.dictChunkHeightsArr.TryGetValue(TileAreaUtils.MakeKey(_chunkX + 1, _chunkZ), out voxeChunkInfo))
		{
			if (voxeChunkInfo.a0 == 1)
			{
				num11 -= 0.0025f;
			}
			else if (voxeChunkInfo.a0 == 2)
			{
				num11 = 0f;
			}
		}
		this.heights8x8[0, 8] = num11;
		for (int j = 1; j < 8; j++)
		{
			num11 = array[num10 + j, num9];
			if (_vciMiddle.a2 == 1)
			{
				num11 -= 0.0025f;
			}
			else if (_vciMiddle.a2 == 2)
			{
				num11 = 0f;
			}
			this.heights8x8[j, 0] = num11;
		}
		for (int k = 1; k < 8; k++)
		{
			for (int l = 1; l < 8; l++)
			{
				num11 = array[num10 + k, num9 + l];
				if (_vciMiddle.a3 == 1)
				{
					num11 -= 0.0025f;
				}
				else if (_vciMiddle.a3 == 2)
				{
					num11 = 0f;
				}
				this.heights8x8[k, l] = num11;
			}
		}
		for (int m = 1; m < 8; m++)
		{
			num11 = array[num10 + m, num9 + 8];
			if (voxeChunkInfo != null)
			{
				if (voxeChunkInfo.a2 == 1)
				{
					num11 -= 0.0025f;
				}
				else if (voxeChunkInfo.a2 == 2)
				{
					num11 = 0f;
				}
			}
			this.heights8x8[m, 8] = num11;
		}
		num11 = array[num10 + 8, num9];
		if (voxeChunkInfo3 != null || this.dictChunkHeightsArr.TryGetValue(TileAreaUtils.MakeKey(_chunkX, _chunkZ + 1), out voxeChunkInfo3))
		{
			if (voxeChunkInfo3.a0 == 1)
			{
				num11 -= 0.0025f;
			}
			else if (voxeChunkInfo3.a0 == 2)
			{
				num11 = 0f;
			}
		}
		this.heights8x8[8, 0] = num11;
		for (int n = 1; n < 8; n++)
		{
			num11 = array[num10 + 8, num9 + n];
			if (voxeChunkInfo3 != null)
			{
				if (voxeChunkInfo3.a1 == 1)
				{
					num11 -= 0.0025f;
				}
				else if (voxeChunkInfo3.a1 == 2)
				{
					num11 = 0f;
				}
			}
			this.heights8x8[8, n] = num11;
		}
		num11 = array[num10 + 8, num9 + 8];
		if (voxeChunkInfo2 != null || this.dictChunkHeightsArr.TryGetValue(TileAreaUtils.MakeKey(_chunkX + 1, _chunkZ + 1), out voxeChunkInfo2))
		{
			if (voxeChunkInfo2.a0 == 1)
			{
				num11 -= 0.0025f;
			}
			else if (voxeChunkInfo2.a0 == 2)
			{
				num11 = 0f;
			}
		}
		this.heights8x8[8, 8] = num11;
		terrainAndWater.terrain.terrainData.SetHeightsDelayLOD(num9, num10, this.heights8x8);
		this.toUpdateTerrain.Add(terrainAndWater.terrain);
		return true;
	}

	// Token: 0x04003DF0 RID: 15856
	public const int cSplatBorderSize = 1;

	// Token: 0x04003DF1 RID: 15857
	[PublicizedFrom(EAccessModifier.Private)]
	public ITileArea<float[,]> terrainHeights;

	// Token: 0x04003DF2 RID: 15858
	[PublicizedFrom(EAccessModifier.Private)]
	public TileArea<Color32[]> splat0Arr;

	// Token: 0x04003DF3 RID: 15859
	[PublicizedFrom(EAccessModifier.Private)]
	public TileArea<Color32[]> splat1Arr;

	// Token: 0x04003DF4 RID: 15860
	[PublicizedFrom(EAccessModifier.Private)]
	public TileArea<Color32[]> splat2Arr;

	// Token: 0x04003DF5 RID: 15861
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject cacheParentObj;

	// Token: 0x04003DF6 RID: 15862
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform cacheParentT;

	// Token: 0x04003DF7 RID: 15863
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject visibleParentObj;

	// Token: 0x04003DF8 RID: 15864
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform visibleParentT;

	// Token: 0x04003DF9 RID: 15865
	[PublicizedFrom(EAccessModifier.Private)]
	public Material terrainMaterial;

	// Token: 0x04003DFA RID: 15866
	[PublicizedFrom(EAccessModifier.Private)]
	public Material waterMaterial;

	// Token: 0x04003DFB RID: 15867
	[PublicizedFrom(EAccessModifier.Private)]
	public UnityDistantTerrain.Config terrainConfig;

	// Token: 0x04003DFC RID: 15868
	[PublicizedFrom(EAccessModifier.Private)]
	public int visibleChunks;

	// Token: 0x04003DFD RID: 15869
	[PublicizedFrom(EAccessModifier.Private)]
	public TileArea<UnityDistantTerrain.TerrainAndWater> terrainTiles;

	// Token: 0x04003DFE RID: 15870
	[PublicizedFrom(EAccessModifier.Private)]
	public List<UnityDistantTerrain.TerrainAndWater> terrainCache = new List<UnityDistantTerrain.TerrainAndWater>();

	// Token: 0x04003DFF RID: 15871
	[PublicizedFrom(EAccessModifier.Private)]
	public TileArea<Texture2D>[] splatMapCache = new TileArea<Texture2D>[3];

	// Token: 0x04003E00 RID: 15872
	[PublicizedFrom(EAccessModifier.Private)]
	public List<uint> tempKeysToRemove = new List<uint>();

	// Token: 0x04003E01 RID: 15873
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Vector3> tempPositions = new List<Vector3>();

	// Token: 0x04003E02 RID: 15874
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isAnyTerrainDirty;

	// Token: 0x04003E03 RID: 15875
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isNeighborsDirty;

	// Token: 0x04003E04 RID: 15876
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isClipTextureDirty;

	// Token: 0x04003E05 RID: 15877
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<uint> visibleTerrainTilesOfObservers = new HashSet<uint>();

	// Token: 0x04003E06 RID: 15878
	[PublicizedFrom(EAccessModifier.Private)]
	public Texture2D terrainClipTexture;

	// Token: 0x04003E07 RID: 15879
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32[] terrainClipCols;

	// Token: 0x04003E08 RID: 15880
	[PublicizedFrom(EAccessModifier.Private)]
	public UnityDistantTerrainWaterPlane waterPlane;

	// Token: 0x04003E09 RID: 15881
	[PublicizedFrom(EAccessModifier.Private)]
	public int waterChunks16x16Width;

	// Token: 0x04003E0A RID: 15882
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] waterChunks16x16;

	// Token: 0x04003E0B RID: 15883
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<uint> visibleVoxelChunks = new HashSet<uint>();

	// Token: 0x04003E0C RID: 15884
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly float[] pixelErrorDistanceDiv = new float[]
	{
		60f,
		75f,
		90f,
		160f,
		300f
	};

	// Token: 0x04003E0D RID: 15885
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly float[] basemapDistances = new float[]
	{
		300f,
		300f,
		400f,
		550f,
		800f
	};

	// Token: 0x04003E0E RID: 15886
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 colClipTerrainAndWater = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	// Token: 0x04003E0F RID: 15887
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 colClipWater = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	// Token: 0x04003E10 RID: 15888
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 colNoClip = new Color32(0, 0, 0, 0);

	// Token: 0x04003E11 RID: 15889
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<uint, UnityDistantTerrain.VoxeChunkInfo> dictChunkHeightsArr = new Dictionary<uint, UnityDistantTerrain.VoxeChunkInfo>();

	// Token: 0x04003E12 RID: 15890
	[PublicizedFrom(EAccessModifier.Private)]
	public List<uint> toRemove = new List<uint>();

	// Token: 0x04003E13 RID: 15891
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<Terrain> toUpdateTerrain = new HashSet<Terrain>();

	// Token: 0x04003E14 RID: 15892
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i[] posAround = new Vector3i[]
	{
		new Vector3i(0, 0, 0),
		new Vector3i(-1, 0, -1),
		new Vector3i(0, 0, -1),
		new Vector3i(1, 0, -1),
		new Vector3i(-1, 0, 0),
		new Vector3i(1, 0, 0),
		new Vector3i(-1, 0, 1),
		new Vector3i(0, 0, 1),
		new Vector3i(1, 0, 1)
	};

	// Token: 0x04003E15 RID: 15893
	[PublicizedFrom(EAccessModifier.Private)]
	public float[,] heights8x8 = new float[9, 9];

	// Token: 0x02000A61 RID: 2657
	public struct Config
	{
		// Token: 0x1700083C RID: 2108
		// (get) Token: 0x060050EE RID: 20718 RVA: 0x00203FE1 File Offset: 0x002021E1
		public int ChunkWorldSize
		{
			get
			{
				return this.DataTileSize;
			}
		}

		// Token: 0x04003E16 RID: 15894
		public int DataWidth;

		// Token: 0x04003E17 RID: 15895
		public int DataHeight;

		// Token: 0x04003E18 RID: 15896
		public int DataTileSize;

		// Token: 0x04003E19 RID: 15897
		public int DataSteps;

		// Token: 0x04003E1A RID: 15898
		public int SplatSteps;

		// Token: 0x04003E1B RID: 15899
		public int MetersPerHeightPix;

		// Token: 0x04003E1C RID: 15900
		public int MaxHeight;

		// Token: 0x04003E1D RID: 15901
		public int PixelError;
	}

	// Token: 0x02000A62 RID: 2658
	[PublicizedFrom(EAccessModifier.Internal)]
	public class TerrainAndWater
	{
		// Token: 0x04003E1E RID: 15902
		public Terrain terrain;

		// Token: 0x04003E1F RID: 15903
		public GameObject waterPlane;
	}

	// Token: 0x02000A63 RID: 2659
	[PublicizedFrom(EAccessModifier.Private)]
	public class VoxeChunkInfo
	{
		// Token: 0x060050F0 RID: 20720 RVA: 0x00203FE9 File Offset: 0x002021E9
		public bool IsEmpty()
		{
			return this.a0 == 0 && this.a1 == 0 && this.a2 == 0 && this.a3 == 0;
		}

		// Token: 0x04003E20 RID: 15904
		public bool isDirty;

		// Token: 0x04003E21 RID: 15905
		public byte a0;

		// Token: 0x04003E22 RID: 15906
		public byte a1;

		// Token: 0x04003E23 RID: 15907
		public byte a2;

		// Token: 0x04003E24 RID: 15908
		public byte a3;
	}
}
