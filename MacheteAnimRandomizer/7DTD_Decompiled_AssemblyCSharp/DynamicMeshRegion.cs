using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ConcurrentCollections;
using UniLinq;
using UnityEngine;

// Token: 0x0200034A RID: 842
public class DynamicMeshRegion : DynamicMeshContainer
{
	// Token: 0x170002BF RID: 703
	// (get) Token: 0x06001891 RID: 6289 RVA: 0x00096829 File Offset: 0x00094A29
	// (set) Token: 0x06001892 RID: 6290 RVA: 0x00096831 File Offset: 0x00094A31
	public Rect Rect { get; set; }

	// Token: 0x170002C0 RID: 704
	// (get) Token: 0x06001893 RID: 6291 RVA: 0x0009683A File Offset: 0x00094A3A
	// (set) Token: 0x06001894 RID: 6292 RVA: 0x00096842 File Offset: 0x00094A42
	public bool RegenRequired { get; set; }

	// Token: 0x170002C1 RID: 705
	// (get) Token: 0x06001895 RID: 6293 RVA: 0x0009684B File Offset: 0x00094A4B
	// (set) Token: 0x06001896 RID: 6294 RVA: 0x00096853 File Offset: 0x00094A53
	public int xIndex { get; set; }

	// Token: 0x170002C2 RID: 706
	// (get) Token: 0x06001897 RID: 6295 RVA: 0x0009685C File Offset: 0x00094A5C
	// (set) Token: 0x06001898 RID: 6296 RVA: 0x00096864 File Offset: 0x00094A64
	public int zIndex { get; set; }

	// Token: 0x170002C3 RID: 707
	// (get) Token: 0x06001899 RID: 6297 RVA: 0x0009686D File Offset: 0x00094A6D
	// (set) Token: 0x0600189A RID: 6298 RVA: 0x00096875 File Offset: 0x00094A75
	public List<PrefabInstance> Instances { get; set; }

	// Token: 0x170002C4 RID: 708
	// (get) Token: 0x0600189B RID: 6299 RVA: 0x0009687E File Offset: 0x00094A7E
	// (set) Token: 0x0600189C RID: 6300 RVA: 0x00096888 File Offset: 0x00094A88
	public GameObject RegionObject
	{
		get
		{
			return this._regionObject;
		}
		set
		{
			if (this._regionObject != null)
			{
				if (DynamicMeshManager.DoLog)
				{
					DynamicMeshManager.LogMsg(string.Concat(new string[]
					{
						"Removing old region mesh: ",
						base.ToDebugLocation(),
						" buff: ",
						this.InBuffer.ToString(),
						"  oldPos ",
						this._regionObject.transform.position.ToString(),
						" vs ",
						this._regionObject.transform.position.ToString()
					}));
				}
				DynamicMeshManager.MeshDestroy(this._regionObject);
			}
			this._regionObject = value;
		}
	}

	// Token: 0x170002C5 RID: 709
	// (get) Token: 0x0600189D RID: 6301 RVA: 0x0009694B File Offset: 0x00094B4B
	// (set) Token: 0x0600189E RID: 6302 RVA: 0x00096953 File Offset: 0x00094B53
	public bool IsMeshLoaded { get; set; }

	// Token: 0x0600189F RID: 6303 RVA: 0x0009695C File Offset: 0x00094B5C
	public DynamicMeshRegion(long key)
	{
		Vector3i vector3i = new Vector3i(WorldChunkCache.extractX(key) * 16, 0, WorldChunkCache.extractZ(key) * 16);
		this.WorldPosition = vector3i;
		this.Key = key;
		this.Rect = new Rect((float)vector3i.x, (float)vector3i.z, 160f, 160f);
		this.xIndex = (int)((double)vector3i.x / 160.0);
		this.zIndex = (int)((double)vector3i.z / 160.0);
	}

	// Token: 0x060018A0 RID: 6304 RVA: 0x00096A4C File Offset: 0x00094C4C
	public DynamicMeshRegion(Vector3i worldPos)
	{
		this.WorldPosition = DynamicMeshUnity.GetRegionPositionFromWorldPosition(worldPos);
		this.Key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(this.WorldPosition.x), World.toChunkXZ(this.WorldPosition.z));
		this.Rect = new Rect((float)this.WorldPosition.x, (float)this.WorldPosition.z, 160f, 160f);
		this.xIndex = (int)((double)worldPos.x / 160.0);
		this.zIndex = (int)((double)worldPos.z / 160.0);
	}

	// Token: 0x060018A1 RID: 6305 RVA: 0x00096B52 File Offset: 0x00094D52
	public void AddToLoadingQueue(DynamicMeshItem item)
	{
		this.OnLoadingQueue.Add(item);
		if (this.OnLoadingQueue.Count == 0)
		{
			this.SetVisibleNew(false, "LoadingQueueEmpty", true);
		}
	}

	// Token: 0x060018A2 RID: 6306 RVA: 0x00096B7B File Offset: 0x00094D7B
	public void RemoveFromLoadingQueue(DynamicMeshItem item)
	{
		this.OnLoadingQueue.Remove(item);
	}

	// Token: 0x060018A3 RID: 6307 RVA: 0x00096B8A File Offset: 0x00094D8A
	public override GameObject GetGameObject()
	{
		return this.RegionObject;
	}

	// Token: 0x060018A4 RID: 6308 RVA: 0x00096B94 File Offset: 0x00094D94
	public bool IsInBuffer()
	{
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		return !(primaryPlayer == null) && DynamicMeshUnity.IsInBuffer(primaryPlayer.position.x, primaryPlayer.position.z, DynamicMeshRegion.BufferIndexSize, this.xIndex, this.zIndex);
	}

	// Token: 0x060018A5 RID: 6309 RVA: 0x00096BE8 File Offset: 0x00094DE8
	public static bool IsInBuffer(int x, int z)
	{
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		return !(primaryPlayer == null) && Math.Abs((int)(primaryPlayer.position.x / 160f) - x / 160) <= DynamicMeshRegion.BufferIndexSize && Math.Abs((int)(primaryPlayer.position.x / 160f) - z / 160) <= DynamicMeshRegion.BufferIndexSize;
	}

	// Token: 0x060018A6 RID: 6310 RVA: 0x00096C64 File Offset: 0x00094E64
	public bool IsInItemLoad()
	{
		if (GameManager.Instance == null)
		{
			return false;
		}
		if (GameManager.Instance.World == null)
		{
			return false;
		}
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		return !(primaryPlayer == null) && DynamicMeshUnity.IsInBuffer(primaryPlayer.position.x, primaryPlayer.position.z, DynamicMeshRegion.ItemLoadIndex, this.xIndex, this.zIndex);
	}

	// Token: 0x060018A7 RID: 6311 RVA: 0x00096CD5 File Offset: 0x00094ED5
	public bool IsInItemLoad(float x, float z)
	{
		return DynamicMeshUnity.IsInBuffer(x, z, DynamicMeshRegion.ItemLoadIndex, this.xIndex, this.zIndex);
	}

	// Token: 0x060018A8 RID: 6312 RVA: 0x00096CF0 File Offset: 0x00094EF0
	public bool IsInItemUnload()
	{
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		return !(primaryPlayer == null) && !DynamicMeshUnity.IsInBuffer(primaryPlayer.position.x, primaryPlayer.position.z, DynamicMeshRegion.ItemUnloadIndex, this.xIndex, this.zIndex);
	}

	// Token: 0x060018A9 RID: 6313 RVA: 0x00096D47 File Offset: 0x00094F47
	public bool FileExists()
	{
		return SdFile.Exists(this.Path);
	}

	// Token: 0x170002C6 RID: 710
	// (get) Token: 0x060018AA RID: 6314 RVA: 0x00096D54 File Offset: 0x00094F54
	public string Path
	{
		get
		{
			return DynamicMeshFile.MeshLocation + this.Key.ToString() + ".group";
		}
	}

	// Token: 0x060018AB RID: 6315 RVA: 0x00096D70 File Offset: 0x00094F70
	public static DynamicMeshRegion GetRegionFromWorldPosition(Vector3i worldPos)
	{
		return DynamicMeshManager.Instance.GetRegion(worldPos);
	}

	// Token: 0x060018AC RID: 6316 RVA: 0x00096D7D File Offset: 0x00094F7D
	public static DynamicMeshRegion GetRegionFromWorldPosition(float worldX, float worldZ)
	{
		return DynamicMeshRegion.GetRegionFromWorldPosition((int)worldX, (int)worldZ);
	}

	// Token: 0x060018AD RID: 6317 RVA: 0x00096D88 File Offset: 0x00094F88
	public static DynamicMeshRegion GetRegionFromWorldPosition(int worldX, int worldZ)
	{
		long regionKeyFromWorldPosition = DynamicMeshUnity.GetRegionKeyFromWorldPosition(worldX, worldZ);
		DynamicMeshRegion result;
		DynamicMeshRegion.Regions.TryGetValue(regionKeyFromWorldPosition, out result);
		return result;
	}

	// Token: 0x060018AE RID: 6318 RVA: 0x00096DAC File Offset: 0x00094FAC
	public bool AddItemToLoadedList(DynamicMeshItem item)
	{
		if (item == null)
		{
			return false;
		}
		for (int i = 0; i < this.UnloadedItems.Count; i++)
		{
			DynamicMeshItem dynamicMeshItem = this.UnloadedItems[i];
			long? num = (dynamicMeshItem != null) ? new long?(dynamicMeshItem.Key) : null;
			long key = item.Key;
			if (num.GetValueOrDefault() == key & num != null)
			{
				this.UnloadedItems.RemoveAt(i);
				this.LoadedItems.Add(item);
				return true;
			}
		}
		return false;
	}

	// Token: 0x060018AF RID: 6319 RVA: 0x00096E30 File Offset: 0x00095030
	public bool HideIfAllLoaded()
	{
		if (this.RegionObject == null)
		{
			return false;
		}
		if (this.UnloadedItems.Count > 0)
		{
			if (this.LoadedItems.Count <= 0 || this.UnloadedItems.Count != 1 || this.UnloadedItems[0].WorldPosition.x != this.WorldPosition.x || this.UnloadedItems[0].WorldPosition.z != this.WorldPosition.z)
			{
				return false;
			}
			if (DynamicMeshManager.DoLog)
			{
				DynamicMeshManager.LogMsg("Override for single item " + base.ToDebugLocation());
			}
		}
		else
		{
			if (!this.RegionObject.activeSelf)
			{
				return false;
			}
			if (this.LoadedItems.Count == 0)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060018B0 RID: 6320 RVA: 0x00096EFF File Offset: 0x000950FF
	public bool IsVisible()
	{
		return this.RegionObject != null && this.RegionObject.activeSelf;
	}

	// Token: 0x060018B1 RID: 6321 RVA: 0x00096F1C File Offset: 0x0009511C
	public bool AddChunk(int x, int z)
	{
		if (this.HasChunk(x, z))
		{
			return false;
		}
		this.LoadedChunks.Add(new Vector3i(x, 0, z));
		return true;
	}

	// Token: 0x060018B2 RID: 6322 RVA: 0x00096F3E File Offset: 0x0009513E
	public bool AddChunk(Vector3i chunk)
	{
		if (this.HasChunk(chunk.x, chunk.z))
		{
			return false;
		}
		this.LoadedChunks.Add(chunk);
		return true;
	}

	// Token: 0x060018B3 RID: 6323 RVA: 0x00096F63 File Offset: 0x00095163
	public bool AddThreadedChunk(int x, int z)
	{
		this.AddChunksThreaded.Enqueue(new Vector3i(x, 0, z));
		return true;
	}

	// Token: 0x060018B4 RID: 6324 RVA: 0x00096F7C File Offset: 0x0009517C
	public bool HasChunk(int x, int z)
	{
		for (int i = 0; i < this.LoadedChunks.Count; i++)
		{
			if (this.LoadedChunks[i].x == x && this.LoadedChunks[i].z == z)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060018B5 RID: 6325 RVA: 0x00096FCC File Offset: 0x000951CC
	public bool HasChunkAny(int x, int z)
	{
		for (int i = 0; i < this.LoadedItems.Count; i++)
		{
			Vector3i worldPosition = this.LoadedItems[i].WorldPosition;
			if (worldPosition.x == x && worldPosition.z == z)
			{
				return true;
			}
		}
		for (int j = 0; j < this.UnloadedItems.Count; j++)
		{
			Vector3i worldPosition2 = this.UnloadedItems[j].WorldPosition;
			if (worldPosition2.x == x && worldPosition2.z == z)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060018B6 RID: 6326 RVA: 0x00097054 File Offset: 0x00095254
	public bool IsRegionLoadedAndActive(bool doDebug)
	{
		if (DynamicMeshManager.Instance.PrefabCheck == PrefabCheckState.Run)
		{
			return true;
		}
		if (!(this.RegionObject == null) && this.LoadedItems.Count != 0)
		{
			if (!this.LoadedItems.Any((DynamicMeshItem d) => d.State != DynamicItemState.Loaded && d.State != DynamicItemState.ReadyToDelete && d.State != DynamicItemState.Empty))
			{
				return true;
			}
		}
		if (this.UnloadedItems.Count > 0)
		{
			this.LoadItems(true, !this.IsVisible(), true, "IsRegionLoadedAndActive");
			if (this.UnloadedItems.Count == 1)
			{
				DynamicMeshItem dynamicMeshItem = this.UnloadedItems[0];
				if (dynamicMeshItem.WorldPosition.x == this.WorldPosition.x && dynamicMeshItem.WorldPosition.z == this.WorldPosition.z)
				{
					return true;
				}
			}
		}
		if (doDebug)
		{
			if (this.RegionObject == null)
			{
				DynamicMeshRegion.LogMsg("LoadedAndActive Failed: regionObject null on " + base.ToDebugLocation());
			}
			if (this.UnloadedItems.Count > 0)
			{
				DynamicMeshRegion.LogMsg("LoadedAndActive Failed: unloaded items on " + base.ToDebugLocation());
			}
			if (this.LoadedItems.Count == 0)
			{
				DynamicMeshRegion.LogMsg("LoadedAndActive Failed: no loaded items on " + base.ToDebugLocation());
			}
			if (this.LoadedItems.Any((DynamicMeshItem d) => d.State != DynamicItemState.Loaded && d.State != DynamicItemState.Empty))
			{
				DynamicMeshRegion.LogMsg("LoadedAndActive Failed: loaded or empty");
			}
		}
		return false;
	}

	// Token: 0x060018B7 RID: 6327 RVA: 0x000971D4 File Offset: 0x000953D4
	public bool ContainsPrefab(PrefabInstance p)
	{
		return this.Intersects(p.boundingBoxPosition.x, p.boundingBoxPosition.z, p.boundingBoxPosition.x + p.boundingBoxSize.x, p.boundingBoxPosition.z + p.boundingBoxSize.z) || this.Intersects(p.boundingBoxPosition.x, p.boundingBoxPosition.z + p.boundingBoxSize.z, p.boundingBoxPosition.x + p.boundingBoxSize.x, p.boundingBoxPosition.z);
	}

	// Token: 0x060018B8 RID: 6328 RVA: 0x0009727C File Offset: 0x0009547C
	public bool Intersects(int x1, int y1, int x2, int y2)
	{
		int num = Math.Min(x1, x2);
		int num2 = Math.Max(x1, x2);
		int num3 = Math.Min(y1, y2);
		int num4 = Math.Max(y1, y2);
		if (this.Rect.xMin > (float)num2 || this.Rect.xMax < (float)num)
		{
			return false;
		}
		if (this.Rect.yMin > (float)num4 || this.Rect.yMax < (float)num3)
		{
			return false;
		}
		if (this.Rect.xMin < (float)num && (float)num2 < this.Rect.xMax)
		{
			return true;
		}
		if (this.Rect.yMin < (float)num3 && (float)num4 < this.Rect.yMax)
		{
			return true;
		}
		Func<float, float> func = (float x) => (float)y1 - (x - (float)x1) * (float)((y1 - y2) / (x2 - x1));
		float num5 = func(this.Rect.xMin);
		float num6 = func(this.Rect.xMax);
		return (this.Rect.yMax >= num5 || this.Rect.yMax >= num6) && (this.Rect.yMin <= num5 || this.Rect.yMin <= num6);
	}

	// Token: 0x060018B9 RID: 6329 RVA: 0x00097421 File Offset: 0x00095621
	public void OnChunkVisible(DynamicMeshItem item)
	{
		this.VisibleChunks += 1;
		this.ShowItems();
		this.HideRegion("onChunkVisible");
	}

	// Token: 0x060018BA RID: 6330 RVA: 0x00097444 File Offset: 0x00095644
	public void OnChunkUnloaded(DynamicMeshItem item)
	{
		if (this.VisibleChunks > 0)
		{
			this.VisibleChunks -= 1;
		}
		if (this.VisibleChunks == 0)
		{
			bool active = true;
			string str = "All chunks unloaded on ";
			Vector3i worldPosition = this.WorldPosition;
			this.SetVisibleNew(active, str + worldPosition.ToString(), true);
			this.HideItems();
		}
	}

	// Token: 0x060018BB RID: 6331 RVA: 0x000974A0 File Offset: 0x000956A0
	public void SetVisibleNew(bool active, string reason, bool updateItems = true)
	{
		if (this.RegionObject != null && active != this.RegionObject.activeSelf)
		{
			if (active && this.IsPlayerInRegion())
			{
				return;
			}
			if (DynamicMeshManager.DoLog)
			{
				Log.Out(string.Concat(new string[]
				{
					"Changing view state for ",
					base.ToDebugLocation(),
					" to visible: ",
					active.ToString(),
					"      Reason: ",
					reason
				}));
			}
			this.RegionObject.SetActive(active);
			if (DynamicMeshManager.DebugItemPositions)
			{
				this.RegionObject.name = base.ToDebugLocation() + ": " + reason;
			}
		}
	}

	// Token: 0x060018BC RID: 6332 RVA: 0x00097550 File Offset: 0x00095750
	public void HideItems()
	{
		foreach (DynamicMeshItem dynamicMeshItem in this.LoadedItems)
		{
			dynamicMeshItem.SetVisible(false, "Region hide");
		}
	}

	// Token: 0x060018BD RID: 6333 RVA: 0x000975A8 File Offset: 0x000957A8
	public void ShowItems()
	{
		foreach (DynamicMeshItem dynamicMeshItem in this.LoadedItems)
		{
			dynamicMeshItem.SetVisible(!dynamicMeshItem.IsChunkInGame, "Region show");
		}
		if (this.OnLoadingQueue.Count == 0)
		{
			this.SetVisibleNew(false, "ShowItems HideRegion", true);
		}
	}

	// Token: 0x060018BE RID: 6334 RVA: 0x00097620 File Offset: 0x00095820
	public bool LoadItems(bool urgent, bool visible, bool includeUnloaded, string reason)
	{
		if (!this.IsInItemLoad())
		{
			if (DynamicMeshManager.DoLog)
			{
				DynamicMeshManager.LogMsg("Load items ignore as outside " + base.ToDebugLocation());
			}
			this.HideItems();
			return false;
		}
		bool flag = false;
		for (int i = 0; i < this.LoadedItems.Count; i++)
		{
			DynamicMeshItem dynamicMeshItem = this.LoadedItems[i];
			if (dynamicMeshItem != null)
			{
				flag = (dynamicMeshItem.LoadIfEmpty("region load '" + reason + "'", urgent, this.InBuffer) || flag);
			}
		}
		if (includeUnloaded)
		{
			for (int j = 0; j < this.UnloadedItems.Count; j++)
			{
				DynamicMeshItem dynamicMeshItem2 = this.UnloadedItems[j];
				if (dynamicMeshItem2 != null)
				{
					flag = (dynamicMeshItem2.LoadIfEmpty("region load unloaded", urgent, this.InBuffer) || flag);
				}
			}
		}
		return this.UnloadedItems.Count > 0 || flag;
	}

	// Token: 0x060018BF RID: 6335 RVA: 0x000976F8 File Offset: 0x000958F8
	public void ShowDebug()
	{
		if (DynamicMeshManager.DoLog)
		{
			string str = "Region: ";
			Vector3i worldPosition = this.WorldPosition;
			DynamicMeshManager.LogMsg(str + worldPosition.ToString() + "  Object: " + ((this.RegionObject == null) ? "null" : this.RegionObject.activeSelf.ToString()));
		}
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg("Chunks: " + this.LoadedChunks.Count.ToString());
		}
		foreach (Vector3i vector3i in this.LoadedChunks)
		{
			Log.Out(vector3i.ToString());
		}
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg("Items: " + this.LoadedItems.Count.ToString() + " vs Unloaded: " + this.UnloadedItems.Count.ToString());
		}
		foreach (DynamicMeshItem dynamicMeshItem in this.LoadedItems)
		{
			if (DynamicMeshManager.DoLog)
			{
				string[] array = new string[5];
				int num = 0;
				Vector3i worldPosition = dynamicMeshItem.WorldPosition;
				array[num] = worldPosition.ToString();
				array[1] = "  Object: ";
				array[2] = ((dynamicMeshItem.ChunkObject == null) ? "null" : dynamicMeshItem.ChunkObject.activeSelf.ToString());
				array[3] = "  State: ";
				array[4] = dynamicMeshItem.State.ToString();
				DynamicMeshManager.LogMsg(string.Concat(array));
			}
		}
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg("--unloaded--");
		}
		foreach (DynamicMeshItem dynamicMeshItem2 in this.UnloadedItems)
		{
			if (DynamicMeshManager.DoLog)
			{
				string[] array2 = new string[5];
				int num2 = 0;
				Vector3i worldPosition = dynamicMeshItem2.WorldPosition;
				array2[num2] = worldPosition.ToString();
				array2[1] = "  Object: ";
				array2[2] = ((dynamicMeshItem2.ChunkObject == null) ? "null" : dynamicMeshItem2.ChunkObject.activeSelf.ToString());
				array2[3] = "  State: ";
				array2[4] = dynamicMeshItem2.State.ToString();
				DynamicMeshManager.LogMsg(string.Concat(array2));
			}
		}
	}

	// Token: 0x060018C0 RID: 6336 RVA: 0x000979B4 File Offset: 0x00095BB4
	public void OnCorrupted()
	{
		if (DynamicMeshManager.DoLog)
		{
			string str = "Corrupted region. Adding for regen ";
			Vector3i worldPosition = this.WorldPosition;
			DynamicMeshManager.LogMsg(str + worldPosition.ToString());
		}
		foreach (DynamicMeshItem dynamicMeshItem in this.LoadedItems)
		{
			DynamicMeshManager.Instance.AddChunk(dynamicMeshItem.WorldPosition, true);
		}
	}

	// Token: 0x170002C7 RID: 711
	// (get) Token: 0x060018C1 RID: 6337 RVA: 0x00097A3C File Offset: 0x00095C3C
	public int Triangles
	{
		get
		{
			int num = 0;
			if (this.RegionObject != null && this.RegionObject.GetComponent<MeshFilter>().mesh.isReadable)
			{
				num += this.RegionObject.GetComponent<MeshFilter>().mesh.triangles.Length;
				foreach (object obj in this.RegionObject.transform)
				{
					Transform transform = (Transform)obj;
					num += transform.gameObject.GetComponent<MeshFilter>().mesh.triangles.Length;
				}
			}
			return num;
		}
	}

	// Token: 0x170002C8 RID: 712
	// (get) Token: 0x060018C2 RID: 6338 RVA: 0x00097AF4 File Offset: 0x00095CF4
	public int Vertices
	{
		get
		{
			int num = 0;
			if (this.RegionObject != null && this.RegionObject.GetComponent<MeshFilter>().mesh.isReadable)
			{
				num += this.RegionObject.GetComponent<MeshFilter>().mesh.vertexCount;
				foreach (object obj in this.RegionObject.transform)
				{
					Transform transform = (Transform)obj;
					num += transform.gameObject.GetComponent<MeshFilter>().mesh.vertexCount;
				}
			}
			return num;
		}
	}

	// Token: 0x170002C9 RID: 713
	// (get) Token: 0x060018C3 RID: 6339 RVA: 0x00097BA8 File Offset: 0x00095DA8
	public int RegionObjects
	{
		get
		{
			int result = 0;
			if (this.RegionObject != null)
			{
				result = this.RegionObject.transform.childCount + 1;
			}
			return result;
		}
	}

	// Token: 0x060018C4 RID: 6340 RVA: 0x00097BDC File Offset: 0x00095DDC
	public void SetPosition()
	{
		if (this.RegionObject != null)
		{
			Vector3 vector = this.WorldPosition.ToVector3() - Origin.position;
			if (this.RegionObject.transform.position != vector)
			{
				this.RegionObject.transform.position = vector;
			}
		}
	}

	// Token: 0x060018C5 RID: 6341 RVA: 0x00097C36 File Offset: 0x00095E36
	public void HideRegion(string debugReason)
	{
		this.SetVisibleNew(false, debugReason, true);
		this.ShowItems();
	}

	// Token: 0x060018C6 RID: 6342 RVA: 0x00097C48 File Offset: 0x00095E48
	public void SetViewStats(bool inBuffer, bool shouldLoadItems, bool shouldUnloadItems, bool isOutsideMaxRegionArea)
	{
		this.OutsideLoadArea = isOutsideMaxRegionArea;
		if (shouldLoadItems)
		{
			this.LoadItems(false, true, true, "setViewStatsShouldLoadItem");
		}
		if (this.IsPlayerInRegion())
		{
			this.HideRegion("set view stats all items loaded");
		}
		if (!inBuffer && !isOutsideMaxRegionArea)
		{
			this.SetVisibleNew(true, "SetViewStats visible", true);
		}
		if (this.RegionObject == null && this.FileExists())
		{
			if (isOutsideMaxRegionArea)
			{
				this.SetState(DynamicRegionState.Unloaded, false);
			}
			else if (this.State != DynamicRegionState.StartLoad)
			{
				this.SetState(DynamicRegionState.StartLoad, false);
				DynamicMeshManager.AddRegionLoadMeshes(this.Key);
			}
		}
		if (inBuffer != this.InBuffer)
		{
			this.InBuffer = inBuffer;
			if (inBuffer)
			{
				if (!this.LoadItems(true, true, true, "setViewInBuffer"))
				{
					this.SetState(DynamicRegionState.Loaded, false);
				}
			}
			else
			{
				this.SetVisibleNew(true, "SetViewStats leftBuffer", true);
			}
		}
		if (shouldUnloadItems && this.LoadedItems.Count > 0)
		{
			this.ClearItems();
		}
	}

	// Token: 0x170002CA RID: 714
	// (get) Token: 0x060018C7 RID: 6343 RVA: 0x00097D28 File Offset: 0x00095F28
	public EntityPlayer GetPlayer
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (GameManager.Instance == null)
			{
				return null;
			}
			if (GameManager.Instance.World == null)
			{
				return null;
			}
			if (!GameManager.IsDedicatedServer)
			{
				return GameManager.Instance.World.GetPrimaryPlayer();
			}
			if (GameManager.Instance.World.Players.Count <= 0)
			{
				return null;
			}
			return GameManager.Instance.World.Players.list[0];
		}
	}

	// Token: 0x060018C8 RID: 6344 RVA: 0x00097D9C File Offset: 0x00095F9C
	public float DistanceToPlayer()
	{
		EntityPlayer getPlayer = this.GetPlayer;
		if (getPlayer == null)
		{
			return 999999f;
		}
		Vector3 position = getPlayer.position;
		int num = 80;
		return Math.Abs(Mathf.Sqrt(Mathf.Pow(position.x - (float)(this.WorldPosition.x + num), 2f) + Mathf.Pow(position.z - (float)(this.WorldPosition.z + num), 2f)));
	}

	// Token: 0x060018C9 RID: 6345 RVA: 0x00097E12 File Offset: 0x00096012
	public void SetState(DynamicRegionState newState, bool forceChange)
	{
		if (!forceChange && newState == DynamicRegionState.Unloading && this.State == DynamicRegionState.Unloaded)
		{
			if (DynamicMeshManager.DoLog)
			{
				DynamicMeshManager.LogMsg("Can't change state from unloading to unloaded");
			}
			return;
		}
		this.State = newState;
	}

	// Token: 0x060018CA RID: 6346 RVA: 0x00097E3C File Offset: 0x0009603C
	public void RemoveChunk(int x, int z, string reason, bool removedFromWorld)
	{
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg(string.Concat(new string[]
			{
				"Removing chunk ",
				x.ToString(),
				",",
				z.ToString(),
				": ",
				reason
			}));
		}
		DynamicMeshThread.ChunkDataQueue.MarkForDeletion(DynamicMeshUnity.GetRegionKeyFromWorldPosition(x, z));
		int i = 0;
		while (i < this.UnloadedItems.Count)
		{
			DynamicMeshItem dynamicMeshItem = this.UnloadedItems[i];
			if (dynamicMeshItem != null && dynamicMeshItem.WorldPosition.x == x && dynamicMeshItem.WorldPosition.z == z)
			{
				dynamicMeshItem.DestroyChunk();
				if (removedFromWorld)
				{
					this.UnloadedItems.RemoveAt(i);
					break;
				}
				dynamicMeshItem.State = DynamicItemState.ReadyToDelete;
				break;
			}
			else
			{
				i++;
			}
		}
		int j = 0;
		while (j < this.LoadedItems.Count)
		{
			DynamicMeshItem dynamicMeshItem2 = this.LoadedItems[j];
			if (dynamicMeshItem2 != null && dynamicMeshItem2.WorldPosition.x == x && dynamicMeshItem2.WorldPosition.z == z)
			{
				dynamicMeshItem2.DestroyChunk();
				if (removedFromWorld)
				{
					this.LoadedItems.RemoveAt(j);
					break;
				}
				dynamicMeshItem2.State = DynamicItemState.ReadyToDelete;
				break;
			}
			else
			{
				j++;
			}
		}
		this.LoadedChunks.Remove(new Vector3i(x, 0, z));
	}

	// Token: 0x060018CB RID: 6347 RVA: 0x00097F80 File Offset: 0x00096180
	public void AddItem(DynamicMeshItem item)
	{
		int num = 0;
		int num2 = 0;
		try
		{
			num = 1;
			num2 = ((item == null) ? 1 : 0);
			if (item == null)
			{
				Log.Error("null item tried to be added");
			}
			else
			{
				num = 2;
				bool flag = false;
				for (int i = 0; i < this.LoadedItems.Count; i++)
				{
					DynamicMeshItem dynamicMeshItem = this.LoadedItems[i];
					num = 3;
					if (dynamicMeshItem != null)
					{
						num = 4;
						num2 = ((item == null) ? 1 : 0) + ((dynamicMeshItem == null) ? 10 : 0);
						if (dynamicMeshItem.Key == item.Key)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					num = 5;
					for (int j = 0; j < this.UnloadedItems.Count; j++)
					{
						num = 6;
						DynamicMeshItem dynamicMeshItem2 = this.UnloadedItems[j];
						num2 = ((item == null) ? 1 : 0);
						if (dynamicMeshItem2 != null)
						{
							num = 7;
							num2 = ((item == null) ? 1 : 0) + ((dynamicMeshItem2 == null) ? 10 : 0);
							if (dynamicMeshItem2.Key == item.Key)
							{
								flag = true;
								break;
							}
						}
					}
				}
				if (!flag)
				{
					num = 8;
					if (GameManager.IsDedicatedServer)
					{
						num = 9;
						this.LoadedItems.Add(item);
					}
					else
					{
						num = 10;
						this.UnloadedItems.Add(item);
					}
				}
				num = 11;
			}
		}
		catch (Exception)
		{
			Log.Error("Add Item error at stage: " + num.ToString() + " nulls: " + num2.ToString());
		}
	}

	// Token: 0x060018CC RID: 6348 RVA: 0x000980D8 File Offset: 0x000962D8
	public int GetStreamLength()
	{
		int val = 8 + this.LoadedChunks.Distinct<Vector3i>().Count<Vector3i>() * 8 + 8 + 12 + 1 + 4;
		return Math.Max(10240, val);
	}

	// Token: 0x060018CD RID: 6349 RVA: 0x0009810E File Offset: 0x0009630E
	public void CleanUp()
	{
		if (this.RegionObject != null)
		{
			DynamicMeshManager.MeshDestroy(this.RegionObject);
			this.RegionObject = null;
		}
		this.ClearMeshes();
		this.State = DynamicRegionState.Unloaded;
		this.IsMeshLoaded = false;
	}

	// Token: 0x060018CE RID: 6350 RVA: 0x00098144 File Offset: 0x00096344
	public bool IsPlayerInRegion()
	{
		EntityPlayerLocal player = DynamicMeshManager.player;
		return !(player == null) && DynamicMeshManager.Instance.GetRegion((int)player.position.x, (int)player.position.z).WorldPosition == this.WorldPosition;
	}

	// Token: 0x060018CF RID: 6351 RVA: 0x00098194 File Offset: 0x00096394
	public void DistanceChecks()
	{
		float num = this.DistanceToPlayer();
		bool inBuffer = this.IsInBuffer();
		bool shouldLoadItems = this.IsInItemLoad();
		bool shouldUnloadItems = this.IsInItemUnload();
		bool flag = num >= (float)DynamicMeshSettings.MaxViewDistance || DynamicMeshManager.IsOutsideDistantTerrain(this);
		this.SetPosition();
		this.SetViewStats(inBuffer, shouldLoadItems, shouldUnloadItems, flag);
		if (this.State == DynamicRegionState.Unloaded && num < (float)DynamicMeshSettings.MaxViewDistance && this.RegionObject == null)
		{
			bool outsideLoadArea = this.OutsideLoadArea;
		}
		if (!(this.RegionObject == null) || this.State == DynamicRegionState.Unloaded)
		{
		}
		if (this.RegionObject != null && flag && DynamicMeshFile.CurrentlyLoadingRegionPosition != this.WorldPosition)
		{
			this.CleanUp();
		}
	}

	// Token: 0x060018D0 RID: 6352 RVA: 0x0009824B File Offset: 0x0009644B
	public static void LogMsg(string msg)
	{
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg(msg);
		}
	}

	// Token: 0x060018D1 RID: 6353 RVA: 0x0009825C File Offset: 0x0009645C
	public void ClearItems()
	{
		foreach (DynamicMeshItem dynamicMeshItem in this.LoadedItems)
		{
			dynamicMeshItem.CleanUp();
			this.UnloadedItems.Add(dynamicMeshItem);
		}
		this.LoadedItems.Clear();
		this.SetVisibleNew(true, "itemsUnloaded", true);
	}

	// Token: 0x060018D2 RID: 6354 RVA: 0x00002914 File Offset: 0x00000B14
	public void ClearMeshes()
	{
	}

	// Token: 0x04000FB3 RID: 4019
	public static ConcurrentDictionary<long, DynamicMeshRegion> Regions = new ConcurrentDictionary<long, DynamicMeshRegion>();

	// Token: 0x04000FB4 RID: 4020
	public static int BufferIndexSize = 1;

	// Token: 0x04000FB5 RID: 4021
	public static int ItemLoadIndex = 3;

	// Token: 0x04000FB6 RID: 4022
	public static int ItemUnloadIndex = DynamicMeshRegion.ItemLoadIndex + 1;

	// Token: 0x04000FBB RID: 4027
	public byte VisibleChunks;

	// Token: 0x04000FBD RID: 4029
	public List<DynamicMeshItem> LoadedItems = new List<DynamicMeshItem>();

	// Token: 0x04000FBE RID: 4030
	public List<DynamicMeshItem> UnloadedItems = new List<DynamicMeshItem>();

	// Token: 0x04000FBF RID: 4031
	public HashSet<DynamicMeshItem> OnLoadingQueue = new HashSet<DynamicMeshItem>();

	// Token: 0x04000FC0 RID: 4032
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject _regionObject;

	// Token: 0x04000FC1 RID: 4033
	public ConcurrentQueue<Vector3i> AddChunksThreaded = new ConcurrentQueue<Vector3i>();

	// Token: 0x04000FC2 RID: 4034
	public ConcurrentHashSet<Vector3i> LoadedChunksThreaded = new ConcurrentHashSet<Vector3i>();

	// Token: 0x04000FC3 RID: 4035
	public List<Vector3i> LoadedChunks = new List<Vector3i>();

	// Token: 0x04000FC5 RID: 4037
	public DateTime CreateDate = DateTime.Now;

	// Token: 0x04000FC6 RID: 4038
	public DateTime NextLoadTime = DateTime.Now;

	// Token: 0x04000FC7 RID: 4039
	public DynamicRegionState State;

	// Token: 0x04000FC8 RID: 4040
	public bool InBuffer;

	// Token: 0x04000FC9 RID: 4041
	public bool FastTrackLoaded;

	// Token: 0x04000FCA RID: 4042
	public bool MarkedForDeletion;

	// Token: 0x04000FCB RID: 4043
	public bool OutsideLoadArea = true;

	// Token: 0x04000FCC RID: 4044
	public bool IsThreadedRegion;
}
