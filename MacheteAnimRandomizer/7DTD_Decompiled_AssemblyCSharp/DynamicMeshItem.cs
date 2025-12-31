using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// Token: 0x0200032D RID: 813
public class DynamicMeshItem : DynamicMeshContainer, IEquatable<DynamicMeshItem>
{
	// Token: 0x170002A2 RID: 674
	// (get) Token: 0x060017A5 RID: 6053 RVA: 0x000903DD File Offset: 0x0008E5DD
	// (set) Token: 0x060017A6 RID: 6054 RVA: 0x000903E5 File Offset: 0x0008E5E5
	public Rect Rect { get; set; }

	// Token: 0x060017A7 RID: 6055 RVA: 0x000903EE File Offset: 0x0008E5EE
	public override GameObject GetGameObject()
	{
		return this.ChunkObject;
	}

	// Token: 0x060017A8 RID: 6056 RVA: 0x000903F8 File Offset: 0x0008E5F8
	public DynamicMeshItem(Vector3i pos)
	{
		this.WorldPosition = pos;
		this.Rect = new Rect((float)pos.x, (float)pos.z, 16f, 16f);
		this.Key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(pos.x), World.toChunkXZ(pos.z));
	}

	// Token: 0x060017A9 RID: 6057 RVA: 0x0009045D File Offset: 0x0008E65D
	public static void AddToMeshPool(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		DynamicMeshManager.MeshDestroy(go);
	}

	// Token: 0x060017AA RID: 6058 RVA: 0x00090470 File Offset: 0x0008E670
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool AddToPoolInternal(GameObject go)
	{
		using (HashSet<GameObject>.Enumerator enumerator = DynamicMeshItem.MeshPool.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.GetInstanceID() == go.GetInstanceID())
				{
					Log.Warning("Duplicate pool add. Name: " + go.name);
					return false;
				}
			}
		}
		DynamicMeshItem.MeshPool.Add(go);
		go.transform.parent = null;
		go.SetActive(false);
		go.GetComponent<MeshFilter>().mesh.Clear(false);
		return true;
	}

	// Token: 0x060017AB RID: 6059 RVA: 0x00090514 File Offset: 0x0008E714
	public static GameObject GetItemMeshRendererFromPool()
	{
		GameObject gameObject;
		if (DynamicMeshItem.MeshPool.Count > 0)
		{
			gameObject = DynamicMeshItem.MeshPool.Last<GameObject>();
			DynamicMeshItem.MeshPool.Remove(gameObject);
		}
		else
		{
			gameObject = DynamicMeshFile.CreateMeshObject(string.Empty, false);
		}
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.parent = DynamicMeshManager.ParentTransform;
		return gameObject;
	}

	// Token: 0x060017AC RID: 6060 RVA: 0x00090574 File Offset: 0x0008E774
	public static GameObject GetRegionMeshRendererFromPool()
	{
		GameObject gameObject;
		if (DynamicMeshItem.MeshPool.Count > 0)
		{
			gameObject = DynamicMeshItem.MeshPool.Last<GameObject>();
			DynamicMeshItem.MeshPool.Remove(gameObject);
		}
		else
		{
			gameObject = DynamicMeshFile.CreateMeshObject(string.Empty, true);
		}
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.parent = DynamicMeshManager.ParentTransform;
		return gameObject;
	}

	// Token: 0x060017AD RID: 6061 RVA: 0x000905D4 File Offset: 0x0008E7D4
	public static GameObject GetTerrainMeshRendererFromPool()
	{
		GameObject gameObject;
		if (DynamicMeshItem.TerrainMeshPool.Count > 0)
		{
			gameObject = DynamicMeshItem.TerrainMeshPool.Last<GameObject>();
			DynamicMeshItem.TerrainMeshPool.Remove(gameObject);
		}
		else
		{
			gameObject = DynamicMeshFile.CreateTerrainMeshObject(string.Empty);
		}
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.parent = DynamicMeshManager.ParentTransform;
		return gameObject;
	}

	// Token: 0x060017AE RID: 6062 RVA: 0x00090633 File Offset: 0x0008E833
	public void CleanUp()
	{
		if (this.ChunkObject != null)
		{
			DynamicMeshManager.MeshDestroy(this.ChunkObject);
		}
		this.State = DynamicItemState.UpdateRequired;
	}

	// Token: 0x170002A3 RID: 675
	// (get) Token: 0x060017AF RID: 6063 RVA: 0x00090658 File Offset: 0x0008E858
	public int Triangles
	{
		get
		{
			int num = 0;
			if (this.ChunkObject != null)
			{
				num += this.ChunkObject.GetComponent<MeshFilter>().mesh.triangles.Length;
				foreach (object obj in this.ChunkObject.transform)
				{
					Transform transform = (Transform)obj;
					num += transform.gameObject.GetComponent<MeshFilter>().mesh.triangles.Length;
				}
			}
			return num;
		}
	}

	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x060017B0 RID: 6064 RVA: 0x000906F4 File Offset: 0x0008E8F4
	public int Vertices
	{
		get
		{
			int num = 0;
			if (this.ChunkObject != null)
			{
				num += this.ChunkObject.GetComponent<MeshFilter>().mesh.vertexCount;
				foreach (object obj in this.ChunkObject.transform)
				{
					Transform transform = (Transform)obj;
					num += transform.gameObject.GetComponent<MeshFilter>().mesh.vertexCount;
				}
			}
			return num;
		}
	}

	// Token: 0x060017B1 RID: 6065 RVA: 0x0009078C File Offset: 0x0008E98C
	public Vector3i GetRegionLocation()
	{
		return DynamicMeshUnity.GetRegionPositionFromWorldPosition(this.WorldPosition);
	}

	// Token: 0x060017B2 RID: 6066 RVA: 0x00090799 File Offset: 0x0008E999
	public long GetRegionKey()
	{
		return WorldChunkCache.MakeChunkKey(World.toChunkXZ(DynamicMeshUnity.RoundRegion(this.WorldPosition.x)), World.toChunkXZ(DynamicMeshUnity.RoundRegion(this.WorldPosition.z)));
	}

	// Token: 0x060017B3 RID: 6067 RVA: 0x000907CC File Offset: 0x0008E9CC
	public int ReadUpdateTimeFromFile()
	{
		int result;
		using (Stream stream = SdFile.OpenRead(this.Path))
		{
			using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
			{
				pooledBinaryReader.SetBaseStream(stream);
				result = pooledBinaryReader.ReadInt32();
			}
		}
		return result;
	}

	// Token: 0x060017B4 RID: 6068 RVA: 0x00090834 File Offset: 0x0008EA34
	public DynamicMeshRegion GetRegion()
	{
		DynamicMeshRegion.GetRegionFromWorldPosition(this.WorldPosition);
		DynamicMeshRegion result;
		DynamicMeshRegion.Regions.TryGetValue(this.GetRegionKey(), out result);
		return result;
	}

	// Token: 0x170002A5 RID: 677
	// (get) Token: 0x060017B5 RID: 6069 RVA: 0x00090861 File Offset: 0x0008EA61
	public bool IsVisible
	{
		get
		{
			return this.ChunkObject != null && this.ChunkObject.activeSelf;
		}
	}

	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x060017B6 RID: 6070 RVA: 0x00090880 File Offset: 0x0008EA80
	public bool IsChunkInView
	{
		get
		{
			if (GameManager.IsDedicatedServer)
			{
				return false;
			}
			if (DynamicMeshManager.Instance == null)
			{
				return false;
			}
			Vector3 position = DynamicMeshItem.player.position;
			int viewSize = DynamicMeshManager.GetViewSize();
			int num = World.toChunkXZ(Utils.Fastfloor(position.x)) * 16;
			int num2 = World.toChunkXZ(Utils.Fastfloor(position.z)) * 16;
			int num3 = num - viewSize;
			int num4 = num + viewSize;
			int num5 = num2 - viewSize;
			int num6 = num2 + viewSize;
			return this.WorldPosition.x > num3 && this.WorldPosition.x <= num4 && this.WorldPosition.z > num5 && this.WorldPosition.z <= num6;
		}
	}

	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x060017B7 RID: 6071 RVA: 0x0009092D File Offset: 0x0008EB2D
	public bool IsChunkInGame
	{
		get
		{
			return !GameManager.IsDedicatedServer && DynamicMeshManager.ChunkGameObjects.Contains(this.Key);
		}
	}

	// Token: 0x060017B8 RID: 6072 RVA: 0x00090948 File Offset: 0x0008EB48
	public void SetVisible(bool active, string reason)
	{
		if (this.ChunkObject == null)
		{
			return;
		}
		if (active != this.ChunkObject.activeSelf)
		{
			if (DynamicMeshManager.DebugItemPositions)
			{
				this.ChunkObject.name = string.Concat(new string[]
				{
					"C ",
					base.ToDebugLocation(),
					": ",
					reason,
					" (",
					active.ToString(),
					")"
				});
			}
			this.ChunkObject.SetActive(active);
			if (DynamicMeshManager.DoLog)
			{
				Log.Out(string.Concat(new string[]
				{
					"Chunk ",
					this.WorldPosition.x.ToString(),
					",",
					this.WorldPosition.z.ToString(),
					" Visible: ",
					active.ToString(),
					" reason: ",
					reason,
					" inview: ",
					this.IsChunkInView.ToString()
				}));
			}
		}
	}

	// Token: 0x060017B9 RID: 6073 RVA: 0x00090A5C File Offset: 0x0008EC5C
	public void ForceHide()
	{
		if (this.ChunkObject == null || !this.ChunkObject.activeSelf)
		{
			return;
		}
		if (DynamicMeshManager.DebugItemPositions)
		{
			this.ChunkObject.name = "C " + base.ToDebugLocation() + ": forceHide";
		}
		this.ChunkObject.SetActive(false);
		if (DynamicMeshManager.DoLog)
		{
			Log.Out(string.Concat(new string[]
			{
				"Chunk ",
				this.WorldPosition.x.ToString(),
				",",
				this.WorldPosition.z.ToString(),
				" ForceHide"
			}));
		}
	}

	// Token: 0x060017BA RID: 6074 RVA: 0x00090B10 File Offset: 0x0008ED10
	public void OnCorrupted()
	{
		if (DynamicMeshManager.DoLog)
		{
			string str = "Corrupted item. Adding for regen ";
			Vector3i worldPosition = this.WorldPosition;
			DynamicMeshManager.LogMsg(str + worldPosition.ToString());
		}
		DynamicMeshManager.Instance.AddChunk(this.WorldPosition, true);
	}

	// Token: 0x060017BB RID: 6075 RVA: 0x00090B5C File Offset: 0x0008ED5C
	public bool LoadIfEmpty(string caller, bool urgentLoad, bool regionInBuffer)
	{
		if (this.ChunkObject != null)
		{
			return false;
		}
		if (this.State == DynamicItemState.ReadyToDelete)
		{
			return false;
		}
		if (this.State == DynamicItemState.Empty)
		{
			return false;
		}
		if (this.State == DynamicItemState.LoadRequested)
		{
			return false;
		}
		if (!DynamicMeshManager.Instance.IsInLoadableArea(this.Key))
		{
			return false;
		}
		this.State = DynamicItemState.LoadRequested;
		DynamicMeshManager.Instance.AddItemLoadRequest(this, urgentLoad);
		return true;
	}

	// Token: 0x060017BC RID: 6076 RVA: 0x00090BC2 File Offset: 0x0008EDC2
	public bool Load(string caller, bool urgentLoad, bool regionInBuffer)
	{
		if (this.State == DynamicItemState.ReadyToDelete)
		{
			return false;
		}
		if (this.State == DynamicItemState.LoadRequested)
		{
			return false;
		}
		this.State = DynamicItemState.LoadRequested;
		DynamicMeshManager.Instance.AddItemLoadRequest(this, urgentLoad);
		return true;
	}

	// Token: 0x060017BD RID: 6077 RVA: 0x00090BF0 File Offset: 0x0008EDF0
	public float DistanceToPlayer(Vector3i playerPos)
	{
		return Math.Abs(Mathf.Sqrt(Mathf.Pow((float)(playerPos.x - this.WorldPosition.x), 2f) + Mathf.Pow((float)(playerPos.z - this.WorldPosition.z), 2f)));
	}

	// Token: 0x060017BE RID: 6078 RVA: 0x00090C44 File Offset: 0x0008EE44
	public float DistanceToPlayer()
	{
		EntityPlayerLocal player = DynamicMeshItem.player;
		Vector3 vector = (player == null) ? Vector3.zero : player.position;
		return Math.Abs(Mathf.Sqrt(Mathf.Pow(vector.x - (float)this.WorldPosition.x, 2f) + Mathf.Pow(vector.z - (float)this.WorldPosition.z, 2f)));
	}

	// Token: 0x060017BF RID: 6079 RVA: 0x00090CB3 File Offset: 0x0008EEB3
	public float DistanceToPlayer(float x, float z)
	{
		return Math.Abs(Mathf.Sqrt(Mathf.Pow(x - (float)this.WorldPosition.x, 2f) + Mathf.Pow(z - (float)this.WorldPosition.z, 2f)));
	}

	// Token: 0x060017C0 RID: 6080 RVA: 0x00090CF0 File Offset: 0x0008EEF0
	public bool DestroyChunk()
	{
		bool result = false;
		if (this.ChunkObject != null)
		{
			result = true;
			if (DynamicMeshManager.DoLog)
			{
				DynamicMeshManager.LogMsg("Destroying chunk " + base.ToDebugLocation());
			}
			DynamicMeshManager.MeshDestroy(this.ChunkObject);
		}
		return result;
	}

	// Token: 0x060017C1 RID: 6081 RVA: 0x00090D37 File Offset: 0x0008EF37
	public void DestroyMesh()
	{
		DynamicMeshManager.Instance.AddObjectForDestruction(this.ChunkObject);
		this.ChunkObject = null;
	}

	// Token: 0x060017C2 RID: 6082 RVA: 0x00090D50 File Offset: 0x0008EF50
	public bool CreateMeshSync(bool isVisible)
	{
		if (this.ChunkObject != null)
		{
			this.SetVisible(isVisible, "Create mesh exists");
			return false;
		}
		string path = this.Path;
		if (!SdFile.Exists(path))
		{
			this.State = DynamicItemState.Empty;
			return false;
		}
		using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
		{
			using (Stream readStream = DynamicMeshFile.GetReadStream(path))
			{
				pooledBinaryReader.SetBaseStream(readStream);
				if (pooledBinaryReader.BaseStream.Position == pooledBinaryReader.BaseStream.Length)
				{
					this.State = DynamicItemState.Empty;
				}
				else
				{
					DynamicMeshFile.ReadItemMesh(pooledBinaryReader, this, isVisible);
				}
			}
		}
		if (this.ChunkObject != null)
		{
			this.SetVisible(isVisible, "create mesh complete");
			this.SetPosition();
			Quaternion identity = Quaternion.identity;
			this.ChunkObject.transform.parent = DynamicMeshManager.ParentTransform;
			this.ChunkObject.transform.rotation = identity;
		}
		this.PackageLength = this.GetStreamLength();
		this.State = DynamicItemState.Loaded;
		return true;
	}

	// Token: 0x060017C3 RID: 6083 RVA: 0x00090E68 File Offset: 0x0008F068
	public void SetPosition()
	{
		if (this.ChunkObject == null)
		{
			return;
		}
		this.ChunkObject.transform.position = this.WorldPosition.ToVector3() - Origin.position;
	}

	// Token: 0x170002A8 RID: 680
	// (get) Token: 0x060017C4 RID: 6084 RVA: 0x00090E9E File Offset: 0x0008F09E
	public string Path
	{
		get
		{
			return DynamicMeshUnity.GetItemPath(this.Key);
		}
	}

	// Token: 0x060017C5 RID: 6085 RVA: 0x00090EAB File Offset: 0x0008F0AB
	public bool FileExists()
	{
		return SdFile.Exists(this.Path);
	}

	// Token: 0x060017C6 RID: 6086 RVA: 0x00090EB8 File Offset: 0x0008F0B8
	public IEnumerator CreateMeshFromVoxelCoroutine(bool isVisible, MicroStopwatch stop, DynamicMeshVoxelLoad data)
	{
		GameObject oldMesh = this.ChunkObject;
		this.ChunkObject = null;
		this.State = DynamicItemState.Loading;
		yield return GameManager.Instance.StartCoroutine(data.CreateMeshCoroutine(this));
		if (this.ChunkObject != null)
		{
			Quaternion identity = Quaternion.identity;
			this.ChunkObject.transform.parent = DynamicMeshManager.ParentTransform;
			this.ChunkObject.transform.rotation = identity;
			this.SetVisible(isVisible, "create mesh complete coroutine");
			this.SetPosition();
			this.State = DynamicItemState.Loaded;
		}
		else
		{
			this.State = DynamicItemState.Empty;
		}
		if (oldMesh != null)
		{
			DynamicMeshItem.AddToMeshPool(oldMesh);
		}
		yield break;
	}

	// Token: 0x060017C7 RID: 6087 RVA: 0x00090ED5 File Offset: 0x0008F0D5
	public override int GetHashCode()
	{
		return this.Key.GetHashCode();
	}

	// Token: 0x060017C8 RID: 6088 RVA: 0x00090EE4 File Offset: 0x0008F0E4
	public override bool Equals(object obj)
	{
		DynamicMeshItem dynamicMeshItem = obj as DynamicMeshItem;
		return dynamicMeshItem != null && dynamicMeshItem.Key == this.Key;
	}

	// Token: 0x060017C9 RID: 6089 RVA: 0x00090F0C File Offset: 0x0008F10C
	public int GetStreamLength()
	{
		int num = 20;
		if (this.ChunkObject == null)
		{
			return num;
		}
		MeshFilter component = this.ChunkObject.GetComponent<MeshFilter>();
		num += 12 + component.mesh.vertexCount * 6 + component.mesh.vertexCount * 8 + component.mesh.triangles.Length * 2;
		foreach (object obj in this.ChunkObject.transform)
		{
			component = ((Transform)obj).gameObject.GetComponent<MeshFilter>();
			num += 12 + component.mesh.vertexCount * 6 + component.mesh.vertexCount * 8 + component.mesh.triangles.Length * 2;
		}
		return num;
	}

	// Token: 0x060017CA RID: 6090 RVA: 0x00090FF0 File Offset: 0x0008F1F0
	public int GetStreamLength(List<VoxelMesh> meshes, List<VoxelMeshTerrain> terrainMeshes)
	{
		int num = 20;
		foreach (VoxelMesh m in meshes)
		{
			num += m.GetByteLength();
		}
		foreach (VoxelMeshTerrain m2 in terrainMeshes)
		{
			num += m2.GetByteLength();
		}
		return num;
	}

	// Token: 0x060017CB RID: 6091 RVA: 0x00091088 File Offset: 0x0008F288
	public bool Equals(DynamicMeshItem other)
	{
		return other.Key == this.Key;
	}

	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x060017CC RID: 6092 RVA: 0x00091098 File Offset: 0x0008F298
	public static EntityPlayerLocal player
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (GameManager.Instance.World != null)
			{
				return GameManager.Instance.World.GetPrimaryPlayer();
			}
			return null;
		}
	}

	// Token: 0x04000EF0 RID: 3824
	public static HashSet<GameObject> MeshPool = new HashSet<GameObject>();

	// Token: 0x04000EF1 RID: 3825
	public static HashSet<GameObject> TerrainMeshPool = new HashSet<GameObject>();

	// Token: 0x04000EF2 RID: 3826
	[PublicizedFrom(EAccessModifier.Private)]
	public static int cacheId = 0;

	// Token: 0x04000EF4 RID: 3828
	public GameObject ChunkObject;

	// Token: 0x04000EF5 RID: 3829
	public int UpdateTime;

	// Token: 0x04000EF6 RID: 3830
	public int PackageLength;

	// Token: 0x04000EF7 RID: 3831
	public DynamicItemState State = DynamicItemState.UpdateRequired;
}
