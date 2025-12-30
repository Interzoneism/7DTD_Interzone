using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200030F RID: 783
public class ChunkPreviewManager
{
	// Token: 0x17000278 RID: 632
	// (get) Token: 0x0600162C RID: 5676 RVA: 0x000813F0 File Offset: 0x0007F5F0
	public Prefab Prefab
	{
		get
		{
			return this.PreviewData.PrefabData;
		}
	}

	// Token: 0x17000279 RID: 633
	// (get) Token: 0x0600162D RID: 5677 RVA: 0x000813FD File Offset: 0x0007F5FD
	public Vector3i WorldPosition
	{
		get
		{
			return this.PreviewData.WorldPosition;
		}
	}

	// Token: 0x0600162E RID: 5678 RVA: 0x0008140C File Offset: 0x0007F60C
	public ChunkPreviewManager()
	{
		ChunkPreviewManager.Instance = this;
		this.PreviewData = new ChunkPreviewData();
		this.PreviewChunksContainer = GameObject.Find("PreviewChunks");
		if (this.PreviewChunksContainer == null)
		{
			this.PreviewChunksContainer = new GameObject("PreviewChunks");
		}
		DynamicMeshThread.SetDefaultThreads();
		this.ThreadData = new DynamicMeshPrefabPreviewThread();
		this.ThreadData.PreviewData = this.PreviewData;
		this.ThreadData.StartThread();
		GameManager.Instance.StartCoroutine(this.LoadPreviewMesh());
	}

	// Token: 0x0600162F RID: 5679 RVA: 0x000814BC File Offset: 0x0007F6BC
	public void AddChunkPreviewLoadData(DynamicMeshVoxelLoad loadData)
	{
		this.ChunkPreviewMeshData.Enqueue(loadData);
	}

	// Token: 0x06001630 RID: 5680 RVA: 0x000814CA File Offset: 0x0007F6CA
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator LoadPreviewMesh()
	{
		while (!this.StopRequested)
		{
			DynamicMeshVoxelLoad voxelData;
			if (!this.ChunkPreviewMeshData.TryDequeue(out voxelData))
			{
				yield return null;
			}
			else
			{
				while (voxelData.Item.State == DynamicItemState.Loading)
				{
					Log.Out("delaying load");
					yield return null;
				}
				voxelData.Item.DestroyChunk();
				yield return GameManager.Instance.StartCoroutine(voxelData.Item.CreateMeshFromVoxelCoroutine(true, null, voxelData));
				this.AddPreviewChunk(voxelData);
				if (voxelData.Item.ChunkObject != null)
				{
					voxelData.Item.ChunkObject.transform.parent = this.PreviewChunksContainer.transform;
				}
				voxelData.DisposeMeshes();
				voxelData = null;
			}
		}
		yield break;
	}

	// Token: 0x06001631 RID: 5681 RVA: 0x000814DC File Offset: 0x0007F6DC
	public void ClearAll()
	{
		foreach (DynamicMeshItem dynamicMeshItem in this.Items)
		{
			dynamicMeshItem.DestroyChunk();
		}
		this.Items.Clear();
	}

	// Token: 0x06001632 RID: 5682 RVA: 0x00081538 File Offset: 0x0007F738
	public void AddPreviewChunk(DynamicMeshVoxelLoad loadData)
	{
		DynamicMeshItem item = loadData.Item;
		if (this.IsPositionInArea(loadData.Item.WorldPosition))
		{
			this.SetChunkGoVisiblity(item.Key, false);
		}
		else
		{
			this.SetChunkGoVisiblity(item.Key, true);
			loadData.Item.DestroyChunk();
		}
		this.CheckItems();
	}

	// Token: 0x06001633 RID: 5683 RVA: 0x0008158D File Offset: 0x0007F78D
	public bool IsPositionInArea(Vector3 pos)
	{
		return this.IsPositionInArea(new Vector3i(pos));
	}

	// Token: 0x06001634 RID: 5684 RVA: 0x0008159C File Offset: 0x0007F79C
	public void ActivationChanged(PrefabInstance pi)
	{
		if (pi == null)
		{
			this.SetWorldPosition(new Vector3i(this.WorldPosition.x, -512, this.WorldPosition.z));
			this.CheckItems();
			return;
		}
		this.SetPrefab(pi.prefab, pi, pi.boundingBoxPosition);
	}

	// Token: 0x06001635 RID: 5685 RVA: 0x000815EC File Offset: 0x0007F7EC
	public bool IsActivePrefab(PrefabInstance pi)
	{
		return pi.prefab == this.PreviewData.PrefabData && this.IsPositionInArea(pi.boundingBoxPosition);
	}

	// Token: 0x06001636 RID: 5686 RVA: 0x00081614 File Offset: 0x0007F814
	public bool IsPositionInArea(Vector3i fullposition)
	{
		if (this.WorldPosition.y < -256)
		{
			return false;
		}
		Vector3i chunkPositionFromWorldPosition = DynamicMeshUnity.GetChunkPositionFromWorldPosition(fullposition);
		Vector3i chunkPositionFromWorldPosition2 = DynamicMeshUnity.GetChunkPositionFromWorldPosition(this.PreviewData.WorldPosition);
		Vector3i chunkPositionFromWorldPosition3 = DynamicMeshUnity.GetChunkPositionFromWorldPosition(this.PreviewData.WorldPosition + this.PreviewData.PrefabData.size);
		int x = chunkPositionFromWorldPosition2.x;
		int x2 = chunkPositionFromWorldPosition3.x;
		int z = chunkPositionFromWorldPosition2.z;
		int z2 = chunkPositionFromWorldPosition3.z;
		return chunkPositionFromWorldPosition.x >= x && chunkPositionFromWorldPosition.x <= x2 && chunkPositionFromWorldPosition.z >= z && chunkPositionFromWorldPosition.z <= z2;
	}

	// Token: 0x06001637 RID: 5687 RVA: 0x000816B8 File Offset: 0x0007F8B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckItems()
	{
		for (int i = this.Items.Count - 1; i >= 0; i--)
		{
			DynamicMeshItem dynamicMeshItem = this.Items[i];
			if (!this.IsPositionInArea(dynamicMeshItem.WorldPosition))
			{
				this.SetChunkGoVisiblity(dynamicMeshItem.Key, true);
				dynamicMeshItem.DestroyChunk();
			}
		}
	}

	// Token: 0x06001638 RID: 5688 RVA: 0x0008170C File Offset: 0x0007F90C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetChunkGoVisiblity(long chunkKey, bool isVisible)
	{
		string b = string.Format("Chunk_{0},{1}", DynamicMeshUnity.GetChunkSectionX(chunkKey), DynamicMeshUnity.GetChunkSectionZ(chunkKey));
		foreach (ChunkGameObject chunkGameObject in GameManager.Instance.World.m_ChunkManager.GetUsedChunkGameObjects())
		{
			Transform transform = chunkGameObject.transform;
			if (transform.name == b)
			{
				if (isVisible)
				{
					transform.gameObject.transform.localScale = Vector3.one;
				}
				else
				{
					transform.gameObject.transform.localScale = Vector3.zero;
				}
			}
		}
	}

	// Token: 0x06001639 RID: 5689 RVA: 0x000817CC File Offset: 0x0007F9CC
	public void CleanUp()
	{
		DynamicMeshPrefabPreviewThread threadData = this.ThreadData;
		if (threadData == null)
		{
			return;
		}
		threadData.StopThread();
	}

	// Token: 0x0600163A RID: 5690 RVA: 0x000817E0 File Offset: 0x0007F9E0
	public void SetWorldPosition(Vector3i worldPosition)
	{
		this.PreviewData.WorldPosition = worldPosition;
		if (this.Prefab != null)
		{
			int num = DynamicMeshUnity.RoundChunk(worldPosition.x + this.Prefab.size.x) + 16;
			int num2 = DynamicMeshUnity.RoundChunk(worldPosition.z + this.Prefab.size.z) + 16;
			for (int i = worldPosition.x; i <= num; i += 16)
			{
				for (int j = worldPosition.z; j <= num2; j += 16)
				{
					this.StartChunkPreview(new Vector3i(i, 0, j));
				}
			}
		}
		this.CheckItems();
	}

	// Token: 0x0600163B RID: 5691 RVA: 0x0008187C File Offset: 0x0007FA7C
	public void StartChunkPreview(Vector3i chunkPos)
	{
		DynamicMeshItem item = this.Get(chunkPos);
		this.ThreadData.AddChunk(item);
	}

	// Token: 0x0600163C RID: 5692 RVA: 0x0008189D File Offset: 0x0007FA9D
	public void SetPrefab(PrefabInstance prefab)
	{
		this.SetPrefab(prefab.prefab, prefab, this.WorldPosition);
	}

	// Token: 0x0600163D RID: 5693 RVA: 0x000818B2 File Offset: 0x0007FAB2
	public void SetPrefab(Prefab prefab)
	{
		this.SetPrefab(prefab, null, this.WorldPosition);
	}

	// Token: 0x0600163E RID: 5694 RVA: 0x000818C4 File Offset: 0x0007FAC4
	public void SetPrefab(Prefab prefab, PrefabInstance instance, Vector3i worldPosition)
	{
		PrefabInstance prefabInstance = this.PreviewData.PrefabInstance;
		if (prefabInstance != null)
		{
			PrefabLODManager.PrefabGameObject instance2 = GameManager.Instance.prefabLODManager.GetInstance(prefabInstance.id);
			if (instance2 != null)
			{
				instance2.go.SetActive(true);
			}
		}
		this.PreviewData.PrefabData = prefab;
		this.PreviewData.PrefabInstance = instance;
		if (instance != null)
		{
			PrefabLODManager.PrefabGameObject instance3 = GameManager.Instance.prefabLODManager.GetInstance(instance.id);
			if (instance3 != null)
			{
				instance3.go.SetActive(false);
			}
		}
		this.SetWorldPosition(worldPosition);
	}

	// Token: 0x0600163F RID: 5695 RVA: 0x0008194C File Offset: 0x0007FB4C
	public DynamicMeshItem Get(Vector3i worldPos)
	{
		worldPos = DynamicMeshUnity.GetChunkPositionFromWorldPosition(worldPos);
		foreach (DynamicMeshItem dynamicMeshItem in this.Items)
		{
			if (dynamicMeshItem.WorldPosition.x == worldPos.x && dynamicMeshItem.WorldPosition.z == worldPos.z)
			{
				return dynamicMeshItem;
			}
		}
		DynamicMeshItem dynamicMeshItem2 = new DynamicMeshItem(worldPos);
		this.Items.Add(dynamicMeshItem2);
		return dynamicMeshItem2;
	}

	// Token: 0x06001640 RID: 5696 RVA: 0x000819E0 File Offset: 0x0007FBE0
	public void Update()
	{
		if (this.NextUpdate > DateTime.Now)
		{
			return;
		}
		this.NextUpdate = DateTime.Now.AddSeconds(1.0);
		foreach (DynamicMeshItem dynamicMeshItem in this.Items)
		{
			this.SetChunkGoVisiblity(dynamicMeshItem.Key, dynamicMeshItem.ChunkObject == null || !this.IsPositionInArea(dynamicMeshItem.WorldPosition));
		}
	}

	// Token: 0x04000DFD RID: 3581
	public static ChunkPreviewManager Instance;

	// Token: 0x04000DFE RID: 3582
	public List<DynamicMeshItem> Items = new List<DynamicMeshItem>();

	// Token: 0x04000DFF RID: 3583
	public ChunkPreviewData PreviewData;

	// Token: 0x04000E00 RID: 3584
	[PublicizedFrom(EAccessModifier.Private)]
	public DynamicMeshPrefabPreviewThread ThreadData;

	// Token: 0x04000E01 RID: 3585
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject PreviewChunksContainer;

	// Token: 0x04000E02 RID: 3586
	public ConcurrentQueue<DynamicMeshVoxelLoad> ChunkPreviewMeshData = new ConcurrentQueue<DynamicMeshVoxelLoad>();

	// Token: 0x04000E03 RID: 3587
	[PublicizedFrom(EAccessModifier.Private)]
	public bool StopRequested;

	// Token: 0x04000E04 RID: 3588
	[PublicizedFrom(EAccessModifier.Private)]
	public DateTime NextUpdate = DateTime.Now;
}
