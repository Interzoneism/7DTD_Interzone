using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200036B RID: 875
public class DynamicMeshVoxelLoad
{
	// Token: 0x170002E2 RID: 738
	// (get) Token: 0x060019E1 RID: 6625 RVA: 0x0009F87C File Offset: 0x0009DA7C
	// (set) Token: 0x060019E2 RID: 6626 RVA: 0x0009F884 File Offset: 0x0009DA84
	public DynamicMeshItem Item { get; set; }

	// Token: 0x170002E3 RID: 739
	// (get) Token: 0x060019E3 RID: 6627 RVA: 0x0009F88D File Offset: 0x0009DA8D
	// (set) Token: 0x060019E4 RID: 6628 RVA: 0x0009F895 File Offset: 0x0009DA95
	public DyMeshData Data { get; set; }

	// Token: 0x060019E5 RID: 6629 RVA: 0x0009F89E File Offset: 0x0009DA9E
	public void DisposeMeshes()
	{
		this.Data = DyMeshData.AddToCache(this.Data);
	}

	// Token: 0x060019E6 RID: 6630 RVA: 0x0009F8B1 File Offset: 0x0009DAB1
	public IEnumerator CreateMeshCoroutine(DynamicMeshItem item)
	{
		DateTime start = DateTime.Now;
		if (item.Key != this.Item.Key)
		{
			Log.Error("mismatching items in Mesh creation");
		}
		GameObject child = DynamicMeshItem.GetItemMeshRendererFromPool();
		child.SetActive(false);
		bool hasMeshes = false;
		VoxelMesh opaqueMesh = this.Data.OpaqueMesh;
		if (opaqueMesh.Vertices.Count > 0)
		{
			hasMeshes = true;
			MeshFilter component = child.GetComponent<MeshFilter>();
			Mesh mesh;
			if (component.sharedMesh == null)
			{
				mesh = new Mesh();
			}
			else
			{
				mesh = component.sharedMesh;
			}
			mesh.indexFormat = ((opaqueMesh.Vertices.Count > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16);
			component.sharedMesh = mesh;
			opaqueMesh.copyToMesh(mesh, opaqueMesh.Vertices, opaqueMesh.Indices, opaqueMesh.Uvs, opaqueMesh.UvsCrack, opaqueMesh.Normals, opaqueMesh.Tangents, opaqueMesh.ColorVertices, null);
			if ((DateTime.Now - start).TotalMilliseconds > 3.0)
			{
				start = DateTime.Now;
				yield return null;
			}
		}
		if ((DateTime.Now - start).TotalMilliseconds > 3.0)
		{
			start = DateTime.Now;
			yield return null;
		}
		VoxelMeshTerrain terrainMesh = this.Data.TerrainMesh;
		if (terrainMesh != null && terrainMesh.Vertices != null && terrainMesh.Vertices.Count != 0)
		{
			hasMeshes = true;
			GameObject terrainMeshRendererFromPool = DynamicMeshItem.GetTerrainMeshRendererFromPool();
			terrainMeshRendererFromPool.SetActive(true);
			terrainMeshRendererFromPool.transform.parent = child.transform;
			MeshFilter component2 = terrainMeshRendererFromPool.GetComponent<MeshFilter>();
			Mesh mesh2;
			if (component2.sharedMesh == null)
			{
				mesh2 = new Mesh();
			}
			else
			{
				mesh2 = component2.sharedMesh;
			}
			mesh2.indexFormat = ((mesh2.vertexCount > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16);
			component2.sharedMesh = mesh2;
			DynamicMeshVoxelLoad.CopyTerrain(terrainMesh, mesh2, component2, new MeshTiming(), this.Item);
			if ((DateTime.Now - start).TotalMilliseconds > 3.0)
			{
				start = DateTime.Now;
				yield return null;
			}
		}
		if (hasMeshes)
		{
			item.ChunkObject = child;
		}
		else
		{
			DynamicMeshManager.MeshDestroy(child);
		}
		yield break;
	}

	// Token: 0x060019E7 RID: 6631 RVA: 0x0009F8C8 File Offset: 0x0009DAC8
	public static void CopyTerrain(VoxelMeshTerrain terrain, Mesh mesh, MeshFilter filter, MeshTiming time, DynamicMeshItem item)
	{
		time.Reset();
		MeshUnsafeCopyHelper.CopyVertices(terrain.Vertices, mesh);
		time.CopyVerts = time.GetTime();
		time.Reset();
		MeshUnsafeCopyHelper.CopyUV(terrain.Uvs, mesh);
		time.CopyUv = time.time;
		time.Reset();
		MeshUnsafeCopyHelper.CopyUV2(terrain.UvsCrack, mesh);
		time.CopyUv2 = time.time;
		if (((terrain != null) ? terrain.Uvs3.Items : null) != null)
		{
			time.Reset();
			MeshUnsafeCopyHelper.CopyUV3(terrain.Uvs3, mesh);
			time.CopyUv3 = time.time;
		}
		if (((terrain != null) ? terrain.Uvs4.Items : null) != null)
		{
			time.Reset();
			MeshUnsafeCopyHelper.CopyUV4(terrain.Uvs4, mesh);
			time.CopyUv4 = time.time;
		}
		time.Reset();
		MeshUnsafeCopyHelper.CopyColors(terrain.ColorVertices, mesh);
		time.CopyColours = time.time;
		time.Reset();
		if (terrain.Indices.Count > 0)
		{
			MeshUnsafeCopyHelper.CopyTriangles(terrain.Indices, mesh);
		}
		else
		{
			mesh.subMeshCount = ((terrain.Indices.Count > 0) ? 1 : terrain.submeshes.Count);
			for (int i = 0; i < terrain.submeshes.Count; i++)
			{
				MeshUnsafeCopyHelper.CopyTriangles(terrain.submeshes[i].triangles, mesh, i);
			}
		}
		time.CopyTriangles = time.time;
		if (terrain.Normals.Count == 0)
		{
			time.Reset();
			mesh.RecalculateNormals();
			time.NormalRecalc = time.time;
		}
		else
		{
			time.Reset();
			MeshUnsafeCopyHelper.CopyNormals(terrain.Normals, mesh);
			time.CopyNormals = time.time;
		}
		time.Reset();
		time.CopyTangents = time.time;
		time.Reset();
		GameUtils.SetMeshVertexAttributes(mesh, true);
		mesh.UploadMeshData(false);
		time.UploadMesh = time.time;
		Renderer component = filter.GetComponent<Renderer>();
		int num = component.sharedMaterials.Length;
		if (!DynamicMeshFile.TerrainSharedMaterials.ContainsKey(num))
		{
			Material[] array = new Material[num];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = MeshDescription.meshes[5].material;
			}
			DynamicMeshFile.TerrainSharedMaterials.Add(num, array);
		}
		component.sharedMaterials = DynamicMeshFile.TerrainSharedMaterials[num];
	}

	// Token: 0x060019E8 RID: 6632 RVA: 0x0009FB07 File Offset: 0x0009DD07
	public static DynamicMeshVoxelLoad Create(DynamicMeshItem item, DyMeshData data)
	{
		return new DynamicMeshVoxelLoad
		{
			Item = item,
			Data = data
		};
	}

	// Token: 0x040010B2 RID: 4274
	public static ConcurrentQueue<VoxelMeshLayer> LayerCache = new ConcurrentQueue<VoxelMeshLayer>();
}
