using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000870 RID: 2160
public class PrefabEditModeManager
{
	// Token: 0x1400004F RID: 79
	// (add) Token: 0x06003EF2 RID: 16114 RVA: 0x00199348 File Offset: 0x00197548
	// (remove) Token: 0x06003EF3 RID: 16115 RVA: 0x00199380 File Offset: 0x00197580
	public event Action<PrefabInstance> OnPrefabChanged;

	// Token: 0x06003EF4 RID: 16116 RVA: 0x001993B5 File Offset: 0x001975B5
	public PrefabEditModeManager()
	{
		PrefabEditModeManager.Instance = this;
	}

	// Token: 0x06003EF5 RID: 16117 RVA: 0x001993E0 File Offset: 0x001975E0
	public void Init()
	{
		this.ReloadAllXmls();
		this.InitXmlWatcher();
		if (this.IsActive())
		{
			SelectionBoxManager.Instance.GetCategory("DynamicPrefabs").SetVisible(false);
		}
		this.NeedsSaving = false;
		GameManager.Instance.World.ChunkClusters[0].OnBlockChangedDelegates += this.blockChangeDelegate;
	}

	// Token: 0x06003EF6 RID: 16118 RVA: 0x00199443 File Offset: 0x00197643
	public void Update()
	{
		if (!this.IsActive())
		{
			return;
		}
		this.updateCompositionGrid();
	}

	// Token: 0x06003EF7 RID: 16119 RVA: 0x00199454 File Offset: 0x00197654
	public bool IsActive()
	{
		return GameManager.Instance.IsEditMode() && GamePrefs.GetString(EnumGamePrefs.GameWorld) == "Empty";
	}

	// Token: 0x06003EF8 RID: 16120 RVA: 0x00199478 File Offset: 0x00197678
	public void LoadRecentlyUsedOrCreateNew()
	{
		if (this.VoxelPrefab != null)
		{
			return;
		}
		string @string = GamePrefs.GetString(EnumGamePrefs.LastLoadedPrefab);
		PathAbstractions.AbstractedLocation abstractedLocation = PathAbstractions.AbstractedLocation.None;
		if (!string.IsNullOrEmpty(@string))
		{
			abstractedLocation = PathAbstractions.PrefabsSearchPaths.GetLocation(@string, null, null);
		}
		if (abstractedLocation.Exists())
		{
			ThreadManager.StartCoroutine(this.loadLastUsedPrefabLater());
			return;
		}
		this.NewVoxelPrefab();
	}

	// Token: 0x06003EF9 RID: 16121 RVA: 0x001994D4 File Offset: 0x001976D4
	public void LoadRecentlyUsed()
	{
		string @string = GamePrefs.GetString(EnumGamePrefs.LastLoadedPrefab);
		if (!string.IsNullOrEmpty(@string) && (this.VoxelPrefab == null || @string != this.VoxelPrefab.PrefabName))
		{
			ThreadManager.StartCoroutine(this.loadLastUsedPrefabLater());
		}
	}

	// Token: 0x06003EFA RID: 16122 RVA: 0x0019951B File Offset: 0x0019771B
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator loadLastUsedPrefabLater()
	{
		yield return new WaitForSeconds(1f);
		ChunkCluster cc = GameManager.Instance.World.ChunkCache;
		List<Chunk> chunkArrayCopySync = cc.GetChunkArrayCopySync();
		foreach (Chunk c in chunkArrayCopySync)
		{
			if (!cc.IsOnBorder(c))
			{
				if (!c.IsEmpty())
				{
					while (c.NeedsRegeneration || c.NeedsCopying)
					{
						yield return new WaitForSeconds(1f);
					}
					c = null;
				}
			}
		}
		List<Chunk>.Enumerator enumerator = default(List<Chunk>.Enumerator);
		PathAbstractions.AbstractedLocation location = PathAbstractions.PrefabsSearchPaths.GetLocation(GamePrefs.GetString(EnumGamePrefs.LastLoadedPrefab), null, null);
		this.LoadVoxelPrefab(location, false, false);
		yield break;
		yield break;
	}

	// Token: 0x06003EFB RID: 16123 RVA: 0x0019952C File Offset: 0x0019772C
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitXmlWatcher()
	{
		string gameDir = GameIO.GetGameDir("Data/Prefabs");
		Log.Out("Watching prefabs folder for XML changes: " + gameDir);
		this.xmlWatcher = new FileSystemWatcher(gameDir, "*.xml");
		this.xmlWatcher.IncludeSubdirectories = true;
		this.xmlWatcher.Changed += this.OnXmlFileChanged;
		this.xmlWatcher.Created += this.OnXmlFileChanged;
		this.xmlWatcher.Deleted += this.OnXmlFileChanged;
		this.xmlWatcher.EnableRaisingEvents = true;
	}

	// Token: 0x06003EFC RID: 16124 RVA: 0x001995C4 File Offset: 0x001977C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnXmlFileChanged(object _sender, FileSystemEventArgs _e)
	{
		Log.Out(string.Format("Prefab XML {0}: {1}", _e.ChangeType, _e.Name));
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_e.Name);
		PathAbstractions.AbstractedLocation abstractedLocation = new PathAbstractions.AbstractedLocation(PathAbstractions.EAbstractedLocationType.GameData, fileNameWithoutExtension, Path.ChangeExtension(_e.FullPath, ".tts"), null, false, null);
		if (_e.ChangeType == WatcherChangeTypes.Deleted)
		{
			Dictionary<PathAbstractions.AbstractedLocation, Prefab> obj = this.loadedPrefabHeaders;
			lock (obj)
			{
				this.loadedPrefabHeaders.Remove(abstractedLocation);
				return;
			}
		}
		this.LoadXml(abstractedLocation);
		if (this.VoxelPrefab != null && this.VoxelPrefab.location == abstractedLocation)
		{
			Log.Out("Applying XML changes to loaded prefab");
			this.VoxelPrefab.LoadXMLData(this.VoxelPrefab.location);
			return;
		}
		if (this.VoxelPrefab != null)
		{
			Log.Out(string.Format("XML changed not related to loaded prefab. (Loaded: {0}, FP {1}; Changed: {2}, FP {3})", new object[]
			{
				this.VoxelPrefab.location,
				this.VoxelPrefab.location.FullPath,
				abstractedLocation,
				abstractedLocation.FullPath
			}));
		}
	}

	// Token: 0x06003EFD RID: 16125 RVA: 0x001996FC File Offset: 0x001978FC
	public void ReloadAllXmls()
	{
		Dictionary<PathAbstractions.AbstractedLocation, Prefab> obj = this.loadedPrefabHeaders;
		lock (obj)
		{
			this.loadedPrefabHeaders.Clear();
			foreach (PathAbstractions.AbstractedLocation location in PathAbstractions.PrefabsSearchPaths.GetAvailablePathsList(null, null, null, false))
			{
				this.LoadXml(location);
			}
		}
	}

	// Token: 0x06003EFE RID: 16126 RVA: 0x0019978C File Offset: 0x0019798C
	public void LoadXml(PathAbstractions.AbstractedLocation _location)
	{
		Prefab prefab = new Prefab();
		prefab.LoadXMLData(_location);
		Dictionary<PathAbstractions.AbstractedLocation, Prefab> obj = this.loadedPrefabHeaders;
		lock (obj)
		{
			this.loadedPrefabHeaders[_location] = prefab;
		}
	}

	// Token: 0x06003EFF RID: 16127 RVA: 0x001997E4 File Offset: 0x001979E4
	public void Cleanup()
	{
		if (this.xmlWatcher != null)
		{
			this.xmlWatcher.EnableRaisingEvents = false;
			this.xmlWatcher = null;
		}
		Dictionary<PathAbstractions.AbstractedLocation, Prefab> obj = this.loadedPrefabHeaders;
		lock (obj)
		{
			this.loadedPrefabHeaders.Clear();
		}
		if (this.groundGrid)
		{
			UnityEngine.Object.Destroy(this.groundGrid);
			this.groundGrid = null;
		}
		if (this.prefabInstanceId != -1)
		{
			DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
			dynamicPrefabDecorator.RemovePrefabAndSelection(GameManager.Instance.World, dynamicPrefabDecorator.GetPrefab(this.prefabInstanceId), false);
			this.prefabInstanceId = -1;
		}
		GameManager.Instance.World.ChunkClusters[0].OnBlockChangedDelegates -= this.blockChangeDelegate;
		this.ClearImposterPrefab();
		this.ClearVoxelPrefab();
		this.OnPrefabChanged = null;
		this.HighlightBlocks(null);
	}

	// Token: 0x06003F00 RID: 16128 RVA: 0x001998DC File Offset: 0x00197ADC
	[PublicizedFrom(EAccessModifier.Private)]
	public void blockChangeDelegate(Vector3i pos, BlockValue bvOld, sbyte oldDens, TextureFullArray oldTex, BlockValue bvNew)
	{
		this.NeedsSaving = true;
	}

	// Token: 0x06003F01 RID: 16129 RVA: 0x001998E8 File Offset: 0x00197AE8
	public void FindPrefabs(string _group, List<PathAbstractions.AbstractedLocation> _result)
	{
		Dictionary<PathAbstractions.AbstractedLocation, Prefab> obj = this.loadedPrefabHeaders;
		lock (obj)
		{
			foreach (KeyValuePair<PathAbstractions.AbstractedLocation, Prefab> keyValuePair in this.loadedPrefabHeaders)
			{
				if (_group == null)
				{
					_result.Add(keyValuePair.Key);
				}
				else if (_group.Length == 0)
				{
					if (keyValuePair.Value.editorGroups == null || keyValuePair.Value.editorGroups.Count == 0)
					{
						_result.Add(keyValuePair.Key);
					}
				}
				else if (keyValuePair.Value.editorGroups != null)
				{
					for (int i = 0; i < keyValuePair.Value.editorGroups.Count; i++)
					{
						if (string.Compare(keyValuePair.Value.editorGroups[i], _group, StringComparison.OrdinalIgnoreCase) == 0)
						{
							_result.Add(keyValuePair.Key);
							break;
						}
					}
				}
			}
		}
	}

	// Token: 0x06003F02 RID: 16130 RVA: 0x00199A08 File Offset: 0x00197C08
	public void GetAllTags(List<string> _result, Prefab _considerLoadedPrefab = null)
	{
		Dictionary<PathAbstractions.AbstractedLocation, Prefab> obj = this.loadedPrefabHeaders;
		lock (obj)
		{
			foreach (KeyValuePair<PathAbstractions.AbstractedLocation, Prefab> keyValuePair in this.loadedPrefabHeaders)
			{
				if (!keyValuePair.Value.Tags.IsEmpty)
				{
					foreach (string item in keyValuePair.Value.Tags.GetTagNames())
					{
						if (!_result.ContainsCaseInsensitive(item))
						{
							_result.Add(item);
						}
					}
				}
			}
		}
		if (_considerLoadedPrefab != null && !_considerLoadedPrefab.Tags.IsEmpty)
		{
			foreach (string item2 in _considerLoadedPrefab.Tags.GetTagNames())
			{
				if (!_result.ContainsCaseInsensitive(item2))
				{
					_result.Add(item2);
				}
			}
		}
	}

	// Token: 0x06003F03 RID: 16131 RVA: 0x00199B60 File Offset: 0x00197D60
	public void GetAllThemeTags(List<string> _result, Prefab _considerLoadedPrefab = null)
	{
		Dictionary<PathAbstractions.AbstractedLocation, Prefab> obj = this.loadedPrefabHeaders;
		lock (obj)
		{
			foreach (KeyValuePair<PathAbstractions.AbstractedLocation, Prefab> keyValuePair in this.loadedPrefabHeaders)
			{
				if (!keyValuePair.Value.ThemeTags.IsEmpty)
				{
					foreach (string item in keyValuePair.Value.ThemeTags.GetTagNames())
					{
						if (!_result.ContainsCaseInsensitive(item))
						{
							_result.Add(item);
						}
					}
				}
			}
		}
		if (_considerLoadedPrefab != null && !_considerLoadedPrefab.ThemeTags.IsEmpty)
		{
			foreach (string item2 in _considerLoadedPrefab.ThemeTags.GetTagNames())
			{
				if (!_result.ContainsCaseInsensitive(item2))
				{
					_result.Add(item2);
				}
			}
		}
	}

	// Token: 0x06003F04 RID: 16132 RVA: 0x00199CB8 File Offset: 0x00197EB8
	public void GetAllGroups(List<string> _result, Prefab _considerLoadedPrefab = null)
	{
		Dictionary<PathAbstractions.AbstractedLocation, Prefab> obj = this.loadedPrefabHeaders;
		lock (obj)
		{
			foreach (KeyValuePair<PathAbstractions.AbstractedLocation, Prefab> keyValuePair in this.loadedPrefabHeaders)
			{
				if (keyValuePair.Value.editorGroups != null)
				{
					foreach (string item in keyValuePair.Value.editorGroups)
					{
						if (!_result.ContainsCaseInsensitive(item))
						{
							_result.Add(item);
						}
					}
				}
			}
		}
		if (((_considerLoadedPrefab != null) ? _considerLoadedPrefab.editorGroups : null) != null)
		{
			foreach (string item2 in _considerLoadedPrefab.editorGroups)
			{
				if (!_result.ContainsCaseInsensitive(item2))
				{
					_result.Add(item2);
				}
			}
		}
	}

	// Token: 0x06003F05 RID: 16133 RVA: 0x00199DEC File Offset: 0x00197FEC
	public void GetAllZones(List<string> _result, Prefab _considerLoadedPrefab = null)
	{
		Dictionary<PathAbstractions.AbstractedLocation, Prefab> obj = this.loadedPrefabHeaders;
		lock (obj)
		{
			foreach (KeyValuePair<PathAbstractions.AbstractedLocation, Prefab> keyValuePair in this.loadedPrefabHeaders)
			{
				string[] allowedZones = keyValuePair.Value.GetAllowedZones();
				if (allowedZones != null)
				{
					foreach (string item in allowedZones)
					{
						if (!_result.ContainsCaseInsensitive(item))
						{
							_result.Add(item);
						}
					}
				}
			}
		}
		if (_considerLoadedPrefab != null)
		{
			string[] allowedZones2 = _considerLoadedPrefab.GetAllowedZones();
			if (allowedZones2 != null)
			{
				foreach (string item2 in allowedZones2)
				{
					if (!_result.ContainsCaseInsensitive(item2))
					{
						_result.Add(item2);
					}
				}
			}
		}
	}

	// Token: 0x06003F06 RID: 16134 RVA: 0x00199EE0 File Offset: 0x001980E0
	public void GetAllQuestTags(List<string> _result, Prefab _considerLoadedPrefab = null)
	{
		string[] array;
		if (_considerLoadedPrefab != null)
		{
			array = _considerLoadedPrefab.GetQuestTags().ToString().Split(',', StringSplitOptions.None);
			Array.Sort<string>(array);
			for (int i = 0; i < array.Length; i++)
			{
				string item = array[i].Trim();
				if (!_result.ContainsCaseInsensitive(item))
				{
					_result.Add(item);
				}
			}
		}
		array = QuestEventManager.allQuestTags.ToString().Split(',', StringSplitOptions.None);
		Array.Sort<string>(array);
		for (int j = 0; j < array.Length; j++)
		{
			string item2 = array[j].Trim();
			if (!_result.ContainsCaseInsensitive(item2))
			{
				_result.Add(item2);
			}
		}
	}

	// Token: 0x06003F07 RID: 16135 RVA: 0x00199F88 File Offset: 0x00198188
	public bool HasPrefabImposter(PathAbstractions.AbstractedLocation _location)
	{
		return SdFile.Exists(_location.FullPathNoExtension + ".mesh");
	}

	// Token: 0x06003F08 RID: 16136 RVA: 0x00199FA0 File Offset: 0x001981A0
	public void ClearImposterPrefab()
	{
		UnityEngine.Object.Destroy(this.ImposterPrefab);
		this.ImposterPrefab = null;
	}

	// Token: 0x06003F09 RID: 16137 RVA: 0x00199FB4 File Offset: 0x001981B4
	public bool LoadImposterPrefab(PathAbstractions.AbstractedLocation _location)
	{
		this.ClearImposterPrefab();
		this.ClearVoxelPrefab();
		if (!SdFile.Exists(_location.FullPathNoExtension + ".mesh"))
		{
			return false;
		}
		this.LoadedPrefab = _location;
		bool bTextureArray = MeshDescription.meshes[0].bTextureArray;
		this.ImposterPrefab = SimpleMeshFile.ReadGameObject(_location.FullPathNoExtension + ".mesh", 0f, null, bTextureArray, false, null, null);
		this.ImposterPrefab.transform.name = _location.Name;
		this.ImposterPrefab.transform.position = new Vector3(0f, -3f, 0f);
		return true;
	}

	// Token: 0x06003F0A RID: 16138 RVA: 0x0019A05C File Offset: 0x0019825C
	public bool IsShowingImposterPrefab()
	{
		return this.ImposterPrefab != null;
	}

	// Token: 0x06003F0B RID: 16139 RVA: 0x0019A06A File Offset: 0x0019826A
	[PublicizedFrom(EAccessModifier.Private)]
	public void removeAllChunks()
	{
		GameManager.Instance.World.m_ChunkManager.RemoveAllChunks();
		GameManager.Instance.World.ChunkCache.Clear();
		WaterSimulationNative.Instance.Clear();
	}

	// Token: 0x06003F0C RID: 16140 RVA: 0x0019A0A0 File Offset: 0x001982A0
	public void ClearVoxelPrefab()
	{
		SelectionBoxManager.Instance.Unselect();
		SelectionBoxManager.Instance.GetCategory("DynamicPrefabs").Clear();
		SelectionBoxManager.Instance.GetCategory("TraderTeleport").Clear();
		SelectionBoxManager.Instance.GetCategory("SleeperVolume").Clear();
		SelectionBoxManager.Instance.GetCategory("InfoVolume").Clear();
		SelectionBoxManager.Instance.GetCategory("WallVolume").Clear();
		SelectionBoxManager.Instance.GetCategory("TriggerVolume").Clear();
		SelectionBoxManager.Instance.GetCategory("POIMarker").Clear();
		SleeperVolumeToolManager.CleanUp();
		this.prefabInstanceId = -1;
		this.LoadedPrefab = PathAbstractions.AbstractedLocation.None;
		this.VoxelPrefab = null;
		this.removeAllChunks();
		DecoManager.Instance.OnWorldUnloaded();
		ThreadManager.RunCoroutineSync(DecoManager.Instance.OnWorldLoaded(1024, 1024, GameManager.Instance.World, null));
		this.TogglePrefabFacing(false);
		this.HighlightQuestLoot = this.HighlightQuestLoot;
		this.HighlightBlockTriggers = this.HighlightBlockTriggers;
		this.showCompositionGrid = false;
	}

	// Token: 0x06003F0D RID: 16141 RVA: 0x0019A1BC File Offset: 0x001983BC
	public bool NewVoxelPrefab()
	{
		this.ClearImposterPrefab();
		this.ClearVoxelPrefab();
		this.VoxelPrefab = new Prefab();
		DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
		if (this.prefabInstanceId != -1)
		{
			dynamicPrefabDecorator.RemovePrefabAndSelection(GameManager.Instance.World, dynamicPrefabDecorator.GetPrefab(this.prefabInstanceId), false);
		}
		dynamicPrefabDecorator.ClearAllPrefabs();
		this.prefabInstanceId = dynamicPrefabDecorator.CreateNewPrefabAndActivate(Prefab.LocationForNewPrefab("New Prefab", null), Vector3i.zero, this.VoxelPrefab, true).id;
		SelectionBoxManager.Instance.GetCategory("DynamicPrefabs").SetVisible(false);
		this.curGridYPos = this.VoxelPrefab.yOffset;
		if (this.groundGrid)
		{
			this.groundGrid.transform.position = new Vector3(0f, (float)(1 - this.curGridYPos) - 0.01f, 0f);
		}
		this.ToggleGroundGrid(true);
		for (int i = -6; i <= 6; i++)
		{
			for (int j = -6; j <= 6; j++)
			{
				Chunk chunk = MemoryPools.PoolChunks.AllocSync(true);
				chunk.X = i;
				chunk.Z = j;
				chunk.ResetBiomeIntensity(BiomeIntensity.Default);
				chunk.NeedsRegeneration = true;
				chunk.NeedsLightCalculation = false;
				chunk.NeedsDecoration = false;
				chunk.ResetLights(byte.MaxValue);
				GameManager.Instance.World.ChunkCache.AddChunkSync(chunk, false);
				WaterSimulationNative.Instance.InitializeChunk(chunk);
			}
		}
		this.NeedsSaving = false;
		GamePrefs.Set(EnumGamePrefs.LastLoadedPrefab, string.Empty);
		GameManager.Instance.World.m_ChunkManager.RemoveAllChunksOnAllClients();
		WaterSimulationNative.Instance.SetPaused(true);
		return true;
	}

	// Token: 0x06003F0E RID: 16142 RVA: 0x0019A364 File Offset: 0x00198564
	public bool LoadVoxelPrefab(PathAbstractions.AbstractedLocation _location, bool _bBulk = false, bool _bIgnoreExcludeImposterCheck = false)
	{
		this.ClearImposterPrefab();
		this.ClearVoxelPrefab();
		this.highlightBlocks(0);
		if (_location.Type == PathAbstractions.EAbstractedLocationType.None)
		{
			Log.Out("No prefab found to load!");
			return false;
		}
		this.VoxelPrefab = new Prefab();
		if (!this.VoxelPrefab.Load(_location, true, true, true, false))
		{
			Log.Out(string.Format("Error loading prefab {0}", _location));
			this.VoxelPrefab = null;
			return false;
		}
		if (!_bIgnoreExcludeImposterCheck && _bBulk && this.VoxelPrefab.bExcludeDistantPOIMesh)
		{
			this.VoxelPrefab = null;
			return false;
		}
		int num = this.VoxelPrefab.size.x * this.VoxelPrefab.size.y * this.VoxelPrefab.size.z;
		if (!_bIgnoreExcludeImposterCheck && _bBulk && ((this.VoxelPrefab.size.y <= 6 && num < 1500) || (this.VoxelPrefab.size.y > 6 && num < 100)))
		{
			this.VoxelPrefab = null;
			return false;
		}
		this.LoadedPrefab = _location;
		this.curGridYPos = this.VoxelPrefab.yOffset;
		if (this.groundGrid)
		{
			this.groundGrid.transform.position = new Vector3(0f, (float)(1 - this.curGridYPos) - 0.01f, 0f);
		}
		ChunkCluster chunkCache = GameManager.Instance.World.ChunkCache;
		this.removeAllChunks();
		int num2 = -1 * this.VoxelPrefab.size.x / 2;
		int num3 = -1 * this.VoxelPrefab.size.z / 2;
		int num4 = num2 + this.VoxelPrefab.size.x;
		int num5 = num3 + this.VoxelPrefab.size.z;
		chunkCache.ChunkMinPos = new Vector2i((num2 - 1) / 16 - 1, (num3 - 1) / 16 - 1);
		chunkCache.ChunkMinPos -= new Vector2i(2, 2);
		chunkCache.ChunkMaxPos = new Vector2i(num4 / 16 + 1, num5 / 16 + 1);
		chunkCache.ChunkMaxPos += new Vector2i(2, 2);
		List<Chunk> list = new List<Chunk>();
		for (int i = chunkCache.ChunkMinPos.x; i <= chunkCache.ChunkMaxPos.x; i++)
		{
			for (int j = chunkCache.ChunkMinPos.y; j <= chunkCache.ChunkMaxPos.y; j++)
			{
				Chunk chunk = MemoryPools.PoolChunks.AllocSync(true);
				chunk.X = i;
				chunk.Z = j;
				chunk.SetFullSunlight();
				chunk.NeedsLightCalculation = false;
				chunk.NeedsDecoration = false;
				chunk.NeedsRegeneration = false;
				chunkCache.AddChunkSync(chunk, true);
				list.Add(chunk);
			}
		}
		Vector3i vector3i = new Vector3i(num2, 1, num3);
		this.VoxelPrefab.CopyIntoLocal(chunkCache, vector3i, true, false, FastTags<TagGroup.Global>.none);
		for (int k = 0; k < list.Count; k++)
		{
			Chunk chunk2 = list[k];
			chunk2.NeedsLightCalculation = false;
			chunk2.NeedsRegeneration = true;
			WaterSimulationNative.Instance.InitializeChunk(chunk2);
		}
		DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
		if (this.prefabInstanceId != -1)
		{
			dynamicPrefabDecorator.RemovePrefabAndSelection(GameManager.Instance.World, dynamicPrefabDecorator.GetPrefab(this.prefabInstanceId), false);
		}
		dynamicPrefabDecorator.ClearAllPrefabs();
		this.prefabInstanceId = dynamicPrefabDecorator.CreateNewPrefabAndActivate(this.VoxelPrefab.location, vector3i, this.VoxelPrefab, false).id;
		this.NeedsSaving = false;
		GamePrefs.Set(EnumGamePrefs.LastLoadedPrefab, _location.Name);
		GameManager.Instance.World.m_ChunkManager.RemoveAllChunksOnAllClients();
		this.HighlightQuestLoot = this.HighlightQuestLoot;
		this.HighlightBlockTriggers = this.HighlightBlockTriggers;
		WaterSimulationNative.Instance.SetPaused(true);
		this.highlightBlocks(this.highlightBlockId);
		return true;
	}

	// Token: 0x06003F0F RID: 16143 RVA: 0x0019A74C File Offset: 0x0019894C
	public bool SaveVoxelPrefab()
	{
		if (this.VoxelPrefab == null)
		{
			return false;
		}
		bool ignorePaintTextures = Chunk.IgnorePaintTextures;
		Chunk.IgnorePaintTextures = false;
		this.updatePrefabBounds();
		Chunk.IgnorePaintTextures = ignorePaintTextures;
		EnumInsideOutside[] eInsideOutside = this.VoxelPrefab.UpdateInsideOutside(this.minPos, this.maxPos);
		this.VoxelPrefab.RecalcInsideDevices(eInsideOutside);
		bool flag = this.VoxelPrefab.Save(this.VoxelPrefab.location, true);
		if (flag)
		{
			this.LoadedPrefab = this.VoxelPrefab.location;
			this.LoadXml(this.VoxelPrefab.location);
			GamePrefs.Set(EnumGamePrefs.LastLoadedPrefab, this.VoxelPrefab.PrefabName);
		}
		this.NeedsSaving = false;
		return flag;
	}

	// Token: 0x06003F10 RID: 16144 RVA: 0x0019A7F8 File Offset: 0x001989F8
	public void UpdateMinMax()
	{
		if (this.VoxelPrefab == null)
		{
			return;
		}
		Vector3i zero = new Vector3i(int.MaxValue, int.MaxValue, int.MaxValue);
		Vector3i zero2 = new Vector3i(int.MinValue, int.MinValue, int.MinValue);
		foreach (Chunk chunk in GameManager.Instance.World.ChunkCache.GetChunkArrayCopySync())
		{
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					for (int k = 0; k < 256; k++)
					{
						int blockId = chunk.GetBlockId(i, k, j);
						WaterValue water = chunk.GetWater(i, k, j);
						if (blockId != 0 || water.HasMass() || chunk.GetDensity(i, k, j) < 0)
						{
							Vector3i vector3i = chunk.ToWorldPos(new Vector3i(i, k, j));
							if (zero.x > vector3i.x)
							{
								zero.x = vector3i.x;
							}
							if (zero.y > vector3i.y)
							{
								zero.y = vector3i.y;
							}
							if (zero.z > vector3i.z)
							{
								zero.z = vector3i.z;
							}
							if (zero2.x < vector3i.x)
							{
								zero2.x = vector3i.x;
							}
							if (zero2.y < vector3i.y)
							{
								zero2.y = vector3i.y;
							}
							if (zero2.z < vector3i.z)
							{
								zero2.z = vector3i.z;
							}
						}
					}
				}
			}
		}
		if (zero.x == 2147483647)
		{
			zero = Vector3i.zero;
			zero2 = Vector3i.zero;
		}
		this.minPos = zero;
		this.maxPos = zero2;
	}

	// Token: 0x06003F11 RID: 16145 RVA: 0x0019AA04 File Offset: 0x00198C04
	[PublicizedFrom(EAccessModifier.Private)]
	public void updatePrefabBounds()
	{
		if (this.VoxelPrefab == null)
		{
			return;
		}
		this.VoxelPrefab.yOffset = this.curGridYPos;
		this.UpdateMinMax();
		this.VoxelPrefab.CopyFromWorldWithEntities(GameManager.Instance.World, this.minPos, this.maxPos, new List<int>());
		if (this.prefabInstanceId != -1)
		{
			GameManager.Instance.GetDynamicPrefabDecorator().GetPrefab(this.prefabInstanceId).UpdateBoundingBoxPosAndScale(this.minPos, this.VoxelPrefab.size, true);
		}
	}

	// Token: 0x06003F12 RID: 16146 RVA: 0x0019AA90 File Offset: 0x00198C90
	public void SetGroundLevel(int _yOffset)
	{
		this.curGridYPos = _yOffset;
		if (this.groundGrid)
		{
			this.groundGrid.transform.position = new Vector3(0f, (float)(1 - this.curGridYPos) - 0.01f, 0f);
		}
	}

	// Token: 0x06003F13 RID: 16147 RVA: 0x0019AAE0 File Offset: 0x00198CE0
	public void ToggleGroundGrid(bool _bForceOn = false)
	{
		if (this.groundGrid == null)
		{
			this.groundGrid = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/GroundGrid/GroundGrid"));
			this.groundGrid.transform.position = new Vector3(0f, (float)(1 - this.curGridYPos) + 0.01f, 0f);
			for (int i = 0; i < this.groundGrid.transform.childCount; i++)
			{
				this.groundGrid.transform.GetChild(i).gameObject.tag = "B_Mesh";
			}
			return;
		}
		this.groundGrid.SetActive(_bForceOn || !this.groundGrid.activeSelf);
	}

	// Token: 0x06003F14 RID: 16148 RVA: 0x0019AB99 File Offset: 0x00198D99
	public bool IsGroundGrid()
	{
		return this.groundGrid != null && this.groundGrid.activeSelf;
	}

	// Token: 0x06003F15 RID: 16149 RVA: 0x0019ABB8 File Offset: 0x00198DB8
	public void ToggleCompositionGrid()
	{
		this.showCompositionGrid = !this.showCompositionGrid;
		if (this.showCompositionGrid)
		{
			SelectionCategory category = SelectionBoxManager.Instance.GetCategory("DynamicPrefabs");
			bool flag = category.IsVisible();
			this.UpdatePrefabBounds();
			if (!flag)
			{
				category.SetVisible(false);
			}
		}
	}

	// Token: 0x06003F16 RID: 16150 RVA: 0x0019AC01 File Offset: 0x00198E01
	public bool IsCompositionGrid()
	{
		return this.IsActive() && this.showCompositionGrid;
	}

	// Token: 0x06003F17 RID: 16151 RVA: 0x0019AC14 File Offset: 0x00198E14
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateCompositionGrid()
	{
		Color color = new Color32(0, byte.MaxValue, 40, byte.MaxValue);
		Color color2 = new Color32(0, byte.MaxValue, 200, byte.MaxValue);
		Color color3 = new Color32(0, byte.MaxValue, 200, byte.MaxValue);
		if (!this.showCompositionGrid)
		{
			return;
		}
		Vector3i vector3i = this.maxPos + Vector3i.one;
		Vector3i vector3i2 = vector3i - this.minPos;
		if (vector3i2 == Vector3i.one)
		{
			return;
		}
		World world = GameManager.Instance.World;
		EntityPlayerLocal entityPlayerLocal = (world != null) ? world.GetPrimaryPlayer() : null;
		if (entityPlayerLocal == null)
		{
			return;
		}
		float y = (float)(1 - this.curGridYPos) + 0.05f;
		float num = (float)vector3i2.x / 1.618034f;
		int num2 = Mathf.RoundToInt(num);
		int num3 = Mathf.RoundToInt(((float)vector3i2.x - num) / 2f);
		float num4 = (float)vector3i2.z / 1.618034f;
		int num5 = Mathf.RoundToInt(num4);
		int num6 = Mathf.RoundToInt(((float)vector3i2.z - num4) / 2f);
		float num7 = (float)vector3i2.x / 2f;
		float num8 = (float)vector3i2.z / 2f;
		Vector3 pos = new Vector3((float)(this.minPos.x + num2), y, (float)this.minPos.z);
		Vector3 pos2 = new Vector3((float)(this.minPos.x + num2), y, (float)vector3i.z);
		DebugLines.Create("GoldenRatio_X1", entityPlayerLocal.RootTransform, pos, pos2, color, color, 0.1f, 0.1f, 0.1f);
		pos = new Vector3((float)(vector3i.x - num2), y, (float)this.minPos.z);
		pos2 = new Vector3((float)(vector3i.x - num2), y, (float)vector3i.z);
		DebugLines.Create("GoldenRatio_X2", entityPlayerLocal.RootTransform, pos, pos2, color, color, 0.1f, 0.1f, 0.1f);
		pos = new Vector3((float)this.minPos.x, y, (float)(this.minPos.z + num5));
		pos2 = new Vector3((float)vector3i.x, y, (float)(this.minPos.z + num5));
		DebugLines.Create("GoldenRatio_Z1", entityPlayerLocal.RootTransform, pos, pos2, color, color, 0.1f, 0.1f, 0.1f);
		pos = new Vector3((float)this.minPos.x, y, (float)(vector3i.z - num5));
		pos2 = new Vector3((float)vector3i.x, y, (float)(vector3i.z - num5));
		DebugLines.Create("GoldenRatio_Z2", entityPlayerLocal.RootTransform, pos, pos2, color, color, 0.1f, 0.1f, 0.1f);
		pos = new Vector3((float)this.minPos.x + num7, y, (float)this.minPos.z);
		pos2 = new Vector3((float)this.minPos.x + num7, y, (float)vector3i.z);
		DebugLines.Create("GoldenRatio_InnerX", entityPlayerLocal.RootTransform, pos, pos2, color2, color2, 0.03f, 0.03f, 0.1f);
		pos = new Vector3((float)this.minPos.x, y, (float)this.minPos.z + num8);
		pos2 = new Vector3((float)vector3i.x, y, (float)this.minPos.z + num8);
		DebugLines.Create("GoldenRatio_InnerZ", entityPlayerLocal.RootTransform, pos, pos2, color2, color2, 0.03f, 0.03f, 0.1f);
		pos = new Vector3((float)(this.minPos.x + num3), y, (float)this.minPos.z);
		pos2 = new Vector3((float)(this.minPos.x + num3), y, (float)vector3i.z);
		DebugLines.Create("GoldenRatio_OuterX1", entityPlayerLocal.RootTransform, pos, pos2, color3, color3, 0.03f, 0.03f, 0.1f);
		pos = new Vector3((float)(vector3i.x - num3), y, (float)this.minPos.z);
		pos2 = new Vector3((float)(vector3i.x - num3), y, (float)vector3i.z);
		DebugLines.Create("GoldenRatio_OuterX2", entityPlayerLocal.RootTransform, pos, pos2, color3, color3, 0.03f, 0.03f, 0.1f);
		pos = new Vector3((float)this.minPos.x, y, (float)(this.minPos.z + num6));
		pos2 = new Vector3((float)vector3i.x, y, (float)(this.minPos.z + num6));
		DebugLines.Create("GoldenRatio_OuterZ1", entityPlayerLocal.RootTransform, pos, pos2, color3, color3, 0.03f, 0.03f, 0.1f);
		pos = new Vector3((float)this.minPos.x, y, (float)(vector3i.z - num6));
		pos2 = new Vector3((float)vector3i.x, y, (float)(vector3i.z - num6));
		DebugLines.Create("GoldenRatio_OuterZ2", entityPlayerLocal.RootTransform, pos, pos2, color3, color3, 0.03f, 0.03f, 0.1f);
	}

	// Token: 0x06003F18 RID: 16152 RVA: 0x0019B158 File Offset: 0x00199358
	public void UpdatePrefabBounds()
	{
		DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
		if (this.prefabInstanceId == -1)
		{
			this.VoxelPrefab = new Prefab();
			this.prefabInstanceId = dynamicPrefabDecorator.CreateNewPrefabAndActivate(this.VoxelPrefab.location, Vector3i.zero, this.VoxelPrefab, true).id;
		}
		if (this.prefabInstanceId != -1)
		{
			SelectionBoxManager.Instance.GetCategory("DynamicPrefabs").GetBox(dynamicPrefabDecorator.GetPrefab(this.prefabInstanceId).name).SetVisible(true);
			SelectionBoxManager.Instance.GetCategory("DynamicPrefabs").SetVisible(true);
		}
		this.updatePrefabBounds();
	}

	// Token: 0x06003F19 RID: 16153 RVA: 0x0019B1FB File Offset: 0x001993FB
	public bool IsPrefabFacing()
	{
		return this.bShowFacing;
	}

	// Token: 0x06003F1A RID: 16154 RVA: 0x0019B204 File Offset: 0x00199404
	public void TogglePrefabFacing(bool _bShow)
	{
		if (this.VoxelPrefab == null)
		{
			return;
		}
		if (_bShow && this.boxShowFacing == null)
		{
			this.boxShowFacing = SelectionBoxManager.Instance.GetCategory("PrefabFacing").AddBox("single", Vector3i.zero, Vector3i.one, true, false);
		}
		this.bShowFacing = _bShow;
		this.updateFacing();
		if (this.boxShowFacing != null)
		{
			if (this.bShowFacing)
			{
				this.boxShowFacing.SetPositionAndSize(new Vector3(0f, 2f, (float)(-(float)this.VoxelPrefab.size.z / 2 - 3)), Vector3i.one);
			}
			SelectionBoxManager.Instance.SetActive("PrefabFacing", "single", this.bShowFacing);
			SelectionBoxManager.Instance.GetCategory("PrefabFacing").GetBox("single").SetVisible(this.bShowFacing);
			SelectionBoxManager.Instance.GetCategory("PrefabFacing").SetVisible(this.bShowFacing);
		}
	}

	// Token: 0x06003F1B RID: 16155 RVA: 0x0019B30E File Offset: 0x0019950E
	public void RotatePrefabFacing()
	{
		if (this.VoxelPrefab == null)
		{
			return;
		}
		this.VoxelPrefab.rotationToFaceNorth++;
		this.VoxelPrefab.rotationToFaceNorth &= 3;
		this.updateFacing();
		this.NeedsSaving = true;
	}

	// Token: 0x06003F1C RID: 16156 RVA: 0x0019B34C File Offset: 0x0019954C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateFacing()
	{
		if (this.VoxelPrefab == null)
		{
			return;
		}
		float facing = 0f;
		switch (this.VoxelPrefab.rotationToFaceNorth)
		{
		case 1:
			facing = 90f;
			break;
		case 2:
			facing = 180f;
			break;
		case 3:
			facing = 270f;
			break;
		}
		SelectionBoxManager.Instance.SetFacingDirection("PrefabFacing", "single", facing);
	}

	// Token: 0x06003F1D RID: 16157 RVA: 0x0019B3B8 File Offset: 0x001995B8
	public void MoveGroundGridUpOrDown(int _deltaY)
	{
		if (this.groundGrid && this.groundGrid.activeSelf)
		{
			this.curGridYPos = Utils.FastClamp(this.curGridYPos - _deltaY, -200, 0);
			if (this.VoxelPrefab != null)
			{
				this.VoxelPrefab.yOffset = this.curGridYPos;
				if (this.prefabInstanceId != -1 && this.OnPrefabChanged != null)
				{
					PrefabInstance prefab = GameManager.Instance.GetDynamicPrefabDecorator().GetPrefab(this.prefabInstanceId);
					this.OnPrefabChanged(prefab);
				}
			}
			if (this.groundGrid)
			{
				this.groundGrid.transform.position = new Vector3(0f, (float)(1 - this.curGridYPos) - 0.01f, 0f);
			}
		}
		this.NeedsSaving = true;
	}

	// Token: 0x06003F1E RID: 16158 RVA: 0x0019B48C File Offset: 0x0019968C
	public void MovePrefabUpOrDown(int _deltaY)
	{
		this.updatePrefabBounds();
		Vector3i vector3i = this.minPos + _deltaY * Vector3i.up;
		if (vector3i.y < 1 || this.maxPos.y + _deltaY > 250)
		{
			return;
		}
		ChunkCluster chunkCache = GameManager.Instance.World.ChunkCache;
		List<Chunk> chunkArrayCopySync = chunkCache.GetChunkArrayCopySync();
		foreach (Chunk chunk in chunkArrayCopySync)
		{
			chunk.RemoveAllTileEntities();
			if (!chunk.IsEmpty())
			{
				for (int i = 0; i < 16; i++)
				{
					for (int j = 0; j < 16; j++)
					{
						for (int k = 0; k < 254; k++)
						{
							chunk.SetWater(i, k, j, WaterValue.Empty);
							BlockValue block = chunk.GetBlock(i, k, j);
							if (!block.isair && !block.ischild)
							{
								chunkCache.SetBlock(chunk.ToWorldPos(new Vector3i(i, k, j)), BlockValue.Air, true, false);
								chunk.SetDensity(i, k, j, MarchingCubes.DensityAir);
								chunk.SetTextureFull(i, k, j, 0L, 0);
							}
						}
					}
				}
			}
		}
		this.VoxelPrefab.CopyIntoLocal(chunkCache, vector3i, true, false, FastTags<TagGroup.Global>.none);
		foreach (Chunk chunk2 in chunkArrayCopySync)
		{
			chunk2.NeedsRegeneration = true;
		}
		GameManager.Instance.World.m_ChunkManager.RemoveAllChunksOnAllClients();
		this.UpdateMinMax();
		if (this.prefabInstanceId != -1)
		{
			GameManager.Instance.GetDynamicPrefabDecorator().GetPrefab(this.prefabInstanceId).UpdateBoundingBoxPosAndScale(this.minPos, this.VoxelPrefab.size, false);
		}
	}

	// Token: 0x06003F1F RID: 16159 RVA: 0x0019B698 File Offset: 0x00199898
	[PublicizedFrom(EAccessModifier.Private)]
	public bool checkLayerEmpty(int _y, List<Chunk> chunks)
	{
		bool bAllEmpty = true;
		ChunkBlockLayer.LoopBlocksDelegate <>9__0;
		for (int i = 0; i < chunks.Count; i++)
		{
			Chunk chunk = chunks[i];
			ChunkBlockLayer.LoopBlocksDelegate @delegate;
			if ((@delegate = <>9__0) == null)
			{
				@delegate = (<>9__0 = delegate(int x, int y, int z, BlockValue bv)
				{
					if (y == _y)
					{
						bAllEmpty = false;
					}
				});
			}
			chunk.LoopOverAllBlocks(@delegate, false, false);
		}
		return bAllEmpty;
	}

	// Token: 0x06003F20 RID: 16160 RVA: 0x0019B700 File Offset: 0x00199900
	public void StripTextures()
	{
		HashSet<Chunk> changedChunks = new HashSet<Chunk>();
		List<Chunk> chunkArrayCopySync = GameManager.Instance.World.ChunkCache.GetChunkArrayCopySync();
		bool bUseSelection = BlockToolSelection.Instance.SelectionActive;
		Vector3i selStart = BlockToolSelection.Instance.SelectionMin;
		Vector3i selEnd = selStart + BlockToolSelection.Instance.SelectionSize - Vector3i.one;
		for (int i = 0; i < chunkArrayCopySync.Count; i++)
		{
			Chunk chunk = chunkArrayCopySync[i];
			chunk.LoopOverAllBlocks(delegate(int x, int y, int z, BlockValue bv)
			{
				if (bUseSelection)
				{
					Vector3i vector3i = chunk.ToWorldPos(new Vector3i(x, y, z));
					if (vector3i.x < selStart.x || vector3i.x > selEnd.x || vector3i.y < selStart.y || vector3i.y > selEnd.y || vector3i.z < selStart.z || vector3i.z > selEnd.z)
					{
						return;
					}
				}
				if (chunk.GetTextureFull(x, y, z, 0) != 0L)
				{
					chunk.SetTextureFull(x, y, z, 0L, 0);
					changedChunks.Add(chunk);
				}
			}, false, false);
		}
		foreach (Chunk chunk2 in changedChunks)
		{
			chunk2.NeedsRegeneration = true;
		}
		if (changedChunks.Count > 0)
		{
			this.NeedsSaving = true;
		}
		GameManager.Instance.World.m_ChunkManager.RemoveAllChunksOnAllClients();
	}

	// Token: 0x06003F21 RID: 16161 RVA: 0x0019B82C File Offset: 0x00199A2C
	public void StripInternalTextures()
	{
		HashSet<Chunk> changedChunks = new HashSet<Chunk>();
		World world = GameManager.Instance.World;
		List<Chunk> chunkArrayCopySync = world.ChunkCache.GetChunkArrayCopySync();
		bool bUseSelection = BlockToolSelection.Instance.SelectionActive;
		Vector3i selStart = BlockToolSelection.Instance.SelectionMin;
		Vector3i selEnd = selStart + BlockToolSelection.Instance.SelectionSize - Vector3i.one;
		for (int i = 0; i < chunkArrayCopySync.Count; i++)
		{
			Chunk chunk = chunkArrayCopySync[i];
			chunk.LoopOverAllBlocks(delegate(int x, int y, int z, BlockValue bv)
			{
				if (bUseSelection)
				{
					Vector3i vector3i = chunk.ToWorldPos(new Vector3i(x, y, z));
					if (vector3i.x < selStart.x || vector3i.x > selEnd.x || vector3i.y < selStart.y || vector3i.y > selEnd.y || vector3i.z < selStart.z || vector3i.z > selEnd.z)
					{
						return;
					}
				}
				if (chunk.GetTextureFull(x, y, z, 0) == 0L)
				{
					return;
				}
				BlockShapeNew blockShapeNew = bv.Block.shape as BlockShapeNew;
				if (blockShapeNew == null)
				{
					return;
				}
				for (int j = 0; j < 6; j++)
				{
					BlockFace blockFace = (BlockFace)j;
					Vector3i other = new Vector3i(BlockFaceFlags.OffsetForFace(blockFace));
					Vector3i pos = chunk.ToWorldPos(new Vector3i(x, y, z)) + other;
					BlockValue block = world.GetBlock(pos);
					if (!block.isair)
					{
						BlockShapeNew blockShapeNew2 = block.Block.shape as BlockShapeNew;
						if (blockShapeNew2 != null)
						{
							BlockFace face = BlockFaceFlags.OppositeFace(blockFace);
							BlockShapeNew.EnumFaceOcclusionInfo faceInfo = blockShapeNew.GetFaceInfo(bv, blockFace);
							BlockShapeNew.EnumFaceOcclusionInfo faceInfo2 = blockShapeNew2.GetFaceInfo(block, face);
							if ((faceInfo == BlockShapeNew.EnumFaceOcclusionInfo.Full && faceInfo2 == BlockShapeNew.EnumFaceOcclusionInfo.Full) || (faceInfo == BlockShapeNew.EnumFaceOcclusionInfo.Part && faceInfo2 == BlockShapeNew.EnumFaceOcclusionInfo.Full) || (faceInfo == BlockShapeNew.EnumFaceOcclusionInfo.Part && faceInfo2 == BlockShapeNew.EnumFaceOcclusionInfo.Part && blockShapeNew == blockShapeNew2 && bv.rotation == block.rotation))
							{
								blockFace = blockShapeNew.GetRotatedBlockFace(bv, blockFace);
								for (int k = 0; k < 1; k++)
								{
									world.ChunkCache.SetBlockFaceTexture(chunk.ToWorldPos(new Vector3i(x, y, z)), blockFace, 0, k);
								}
								changedChunks.Add(chunk);
							}
						}
					}
				}
			}, false, false);
		}
		foreach (Chunk chunk2 in changedChunks)
		{
			chunk2.NeedsRegeneration = true;
		}
		if (changedChunks.Count > 0)
		{
			this.NeedsSaving = true;
		}
		GameManager.Instance.World.m_ChunkManager.RemoveAllChunksOnAllClients();
	}

	// Token: 0x06003F22 RID: 16162 RVA: 0x0019B964 File Offset: 0x00199B64
	public void GetLootAndFetchLootContainerCount(out int _loot, out int _fetchLoot, out int _restorePower)
	{
		int tempLoot = 0;
		int tempFetchLoot = 0;
		int tempRestorePower = 0;
		List<Chunk> chunkArrayCopySync = GameManager.Instance.World.ChunkCache.GetChunkArrayCopySync();
		ChunkBlockLayer.LoopBlocksDelegate <>9__0;
		for (int i = 0; i < chunkArrayCopySync.Count; i++)
		{
			Chunk chunk = chunkArrayCopySync[i];
			ChunkBlockLayer.LoopBlocksDelegate @delegate;
			if ((@delegate = <>9__0) == null)
			{
				@delegate = (<>9__0 = delegate(int _, int _, int _, BlockValue bv)
				{
					Block block = bv.Block;
					if (block == null)
					{
						return;
					}
					bool flag = block.IndexName != null;
					if (flag && block.IndexName == Constants.cQuestLootFetchContainerIndexName)
					{
						int num = tempFetchLoot;
						tempFetchLoot = num + 1;
						return;
					}
					if (flag && block.IndexName == Constants.cQuestRestorePowerIndexName)
					{
						int num = tempRestorePower;
						tempRestorePower = num + 1;
						return;
					}
					if (block is BlockLoot)
					{
						int num = tempLoot;
						tempLoot = num + 1;
						return;
					}
					BlockCompositeTileEntity blockCompositeTileEntity = block as BlockCompositeTileEntity;
					if (blockCompositeTileEntity != null && blockCompositeTileEntity.CompositeData.HasFeature<ITileEntityLootable>())
					{
						int num = tempLoot;
						tempLoot = num + 1;
					}
				});
			}
			chunk.LoopOverAllBlocks(@delegate, false, false);
		}
		_loot = tempLoot;
		_fetchLoot = tempFetchLoot;
		_restorePower = tempRestorePower;
	}

	// Token: 0x17000685 RID: 1669
	// (get) Token: 0x06003F23 RID: 16163 RVA: 0x0019B9F7 File Offset: 0x00199BF7
	// (set) Token: 0x06003F24 RID: 16164 RVA: 0x0019B9FF File Offset: 0x00199BFF
	public bool HighlightingBlocks { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06003F25 RID: 16165 RVA: 0x0019BA08 File Offset: 0x00199C08
	public void HighlightBlocks(Block _blockClass)
	{
		this.highlightBlockId = ((_blockClass != null) ? _blockClass.blockID : 0);
		this.HighlightingBlocks = (this.highlightBlockId > 0);
		this.highlightBlocks(this.highlightBlockId);
	}

	// Token: 0x06003F26 RID: 16166 RVA: 0x0019BA37 File Offset: 0x00199C37
	public void ToggleHighlightBlocks()
	{
		this.HighlightingBlocks = !this.HighlightingBlocks;
		this.highlightBlocks(this.HighlightingBlocks ? this.highlightBlockId : 0);
	}

	// Token: 0x06003F27 RID: 16167 RVA: 0x0019BA60 File Offset: 0x00199C60
	[PublicizedFrom(EAccessModifier.Private)]
	public void highlightBlocks(int _blockId)
	{
		BlockHighlighter.Cleanup();
		if (_blockId <= 0)
		{
			return;
		}
		List<Chunk> chunkArrayCopySync = GameManager.Instance.World.ChunkCache.GetChunkArrayCopySync();
		for (int i = 0; i < chunkArrayCopySync.Count; i++)
		{
			Chunk chunk = chunkArrayCopySync[i];
			chunk.LoopOverAllBlocks(delegate(int _x, int _y, int _z, BlockValue _bv)
			{
				if (_bv.type == _blockId)
				{
					BlockHighlighter.AddBlock(chunk.worldPosIMin + new Vector3i(_x, _y, _z));
				}
			}, false, false);
		}
	}

	// Token: 0x17000686 RID: 1670
	// (get) Token: 0x06003F28 RID: 16168 RVA: 0x0019BAE2 File Offset: 0x00199CE2
	// (set) Token: 0x06003F29 RID: 16169 RVA: 0x0019BAEC File Offset: 0x00199CEC
	public bool HighlightQuestLoot
	{
		get
		{
			return this.bShowQuestLoot;
		}
		set
		{
			this.bShowQuestLoot = value;
			NavObjectManager.Instance.UnRegisterNavObjectByClass("editor_quest_loot_container");
			if (value)
			{
				foreach (Chunk chunk in GameManager.Instance.World.ChunkCache.GetChunkArrayCopySync())
				{
					List<Vector3i> list;
					if (chunk.IndexedBlocks.TryGetValue(Constants.cQuestLootFetchContainerIndexName, out list))
					{
						Vector3i worldPos = chunk.GetWorldPos();
						foreach (Vector3i other in list)
						{
							NavObjectManager.Instance.RegisterNavObject("editor_quest_loot_container", (worldPos + other).ToVector3Center(), "", false, -1, null);
						}
					}
				}
			}
		}
	}

	// Token: 0x17000687 RID: 1671
	// (get) Token: 0x06003F2A RID: 16170 RVA: 0x0019BBE4 File Offset: 0x00199DE4
	// (set) Token: 0x06003F2B RID: 16171 RVA: 0x0019BBEC File Offset: 0x00199DEC
	public bool HighlightBlockTriggers
	{
		get
		{
			return this.bShowBlockTriggers;
		}
		set
		{
			this.bShowBlockTriggers = value;
			NavObjectManager.Instance.UnRegisterNavObjectByClass("editor_block_trigger");
			if (value)
			{
				foreach (Chunk chunk in GameManager.Instance.World.ChunkCache.GetChunkArrayCopySync())
				{
					List<BlockTrigger> list = chunk.GetBlockTriggers().list;
					Vector3i worldPos = chunk.GetWorldPos();
					for (int i = 0; i < list.Count; i++)
					{
						NavObject navObject = NavObjectManager.Instance.RegisterNavObject("editor_block_trigger", (worldPos + list[i].LocalChunkPos).ToVector3Center(), "", false, -1, null);
						navObject.name = list[i].TriggerDisplay();
						navObject.OverrideColor = ((list[i].TriggeredByIndices.Count > 0) ? Color.blue : Color.red);
					}
				}
			}
		}
	}

	// Token: 0x040032C2 RID: 12994
	public const int cGroundGridYDefault = 1;

	// Token: 0x040032C3 RID: 12995
	[PublicizedFrom(EAccessModifier.Private)]
	public const string SingleFacingBoxName = "single";

	// Token: 0x040032C4 RID: 12996
	public static PrefabEditModeManager Instance;

	// Token: 0x040032C6 RID: 12998
	public PathAbstractions.AbstractedLocation LoadedPrefab;

	// Token: 0x040032C7 RID: 12999
	public Prefab VoxelPrefab;

	// Token: 0x040032C8 RID: 13000
	public GameObject ImposterPrefab;

	// Token: 0x040032C9 RID: 13001
	public bool NeedsSaving;

	// Token: 0x040032CA RID: 13002
	public Vector3i minPos;

	// Token: 0x040032CB RID: 13003
	public Vector3i maxPos;

	// Token: 0x040032CC RID: 13004
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<PathAbstractions.AbstractedLocation, Prefab> loadedPrefabHeaders = new Dictionary<PathAbstractions.AbstractedLocation, Prefab>();

	// Token: 0x040032CD RID: 13005
	[PublicizedFrom(EAccessModifier.Private)]
	public int curGridYPos;

	// Token: 0x040032CE RID: 13006
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject groundGrid;

	// Token: 0x040032CF RID: 13007
	[PublicizedFrom(EAccessModifier.Private)]
	public int prefabInstanceId = -1;

	// Token: 0x040032D0 RID: 13008
	[PublicizedFrom(EAccessModifier.Private)]
	public FileSystemWatcher xmlWatcher;

	// Token: 0x040032D1 RID: 13009
	[PublicizedFrom(EAccessModifier.Private)]
	public SelectionBox boxShowFacing;

	// Token: 0x040032D2 RID: 13010
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bShowFacing;

	// Token: 0x040032D3 RID: 13011
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bShowQuestLoot;

	// Token: 0x040032D4 RID: 13012
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bShowBlockTriggers;

	// Token: 0x040032D5 RID: 13013
	public List<byte> TriggerLayers = new List<byte>();

	// Token: 0x040032D6 RID: 13014
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showCompositionGrid;

	// Token: 0x040032D8 RID: 13016
	[PublicizedFrom(EAccessModifier.Private)]
	public int highlightBlockId;
}
