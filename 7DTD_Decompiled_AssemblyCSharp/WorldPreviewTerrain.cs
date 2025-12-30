using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using WorldGenerationEngineFinal;

// Token: 0x02000EC4 RID: 3780
public class WorldPreviewTerrain : MonoBehaviour
{
	// Token: 0x17000C22 RID: 3106
	// (get) Token: 0x06007792 RID: 30610 RVA: 0x0030A236 File Offset: 0x00308436
	public static WorldBuilder worldBuilder
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			XUiC_WorldGenerationWindowGroup instance = XUiC_WorldGenerationWindowGroup.Instance;
			if (instance == null)
			{
				return null;
			}
			return instance.worldBuilder;
		}
	}

	// Token: 0x06007793 RID: 30611 RVA: 0x0030AAA8 File Offset: 0x00308CA8
	public static void GenerateTerrain(Transform _parentTransform)
	{
		int metersPerDetailUnit = 1;
		switch (XUiC_WorldGenerationWindowGroup.Instance.PreviewQualityLevel)
		{
		case XUiC_WorldGenerationWindowGroup.PreviewQuality.NoPreview:
			return;
		case XUiC_WorldGenerationWindowGroup.PreviewQuality.Lowest:
			metersPerDetailUnit = 16;
			break;
		case XUiC_WorldGenerationWindowGroup.PreviewQuality.Low:
			metersPerDetailUnit = 8;
			break;
		case XUiC_WorldGenerationWindowGroup.PreviewQuality.Default:
			metersPerDetailUnit = 4;
			break;
		case XUiC_WorldGenerationWindowGroup.PreviewQuality.High:
			metersPerDetailUnit = 2;
			break;
		case XUiC_WorldGenerationWindowGroup.PreviewQuality.Highest:
			metersPerDetailUnit = 1;
			break;
		}
		WorldPreviewTerrain.TerrainPreviewMaterial = (Resources.Load("Materials/TerrainPreview", typeof(Material)) as Material);
		WorldPreviewTerrain.TerrainPreviewMaterial.mainTexture = WorldPreviewTerrain.worldBuilder.PreviewImage;
		int num = WorldPreviewTerrain.worldBuilder.WorldSize / 512;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				GameObject gameObject = new GameObject(new Vector2i(i, j).ToString());
				gameObject.transform.SetParent(_parentTransform);
				WorldPreviewTerrain worldPreviewTerrain = gameObject.AddComponent<WorldPreviewTerrain>();
				worldPreviewTerrain.DrawMeshSector(new Vector2i(i, j), metersPerDetailUnit);
				worldPreviewTerrain.meshRenderer.sharedMaterial = WorldPreviewTerrain.TerrainPreviewMaterial;
			}
		}
		_parentTransform.localPosition = new Vector3(-((float)WorldPreviewTerrain.worldBuilder.WorldSize * 0.5f), 0f, -((float)WorldPreviewTerrain.worldBuilder.WorldSize * 0.5f));
	}

	// Token: 0x06007794 RID: 30612 RVA: 0x0030ABD4 File Offset: 0x00308DD4
	public static void Cleanup(GameObject _rootObj)
	{
		MeshFilter[] componentsInChildren = _rootObj.GetComponentsInChildren<MeshFilter>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UnityEngine.Object.Destroy(componentsInChildren[i].sharedMesh);
		}
		Renderer[] componentsInChildren2 = _rootObj.GetComponentsInChildren<Renderer>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			UnityEngine.Object.Destroy(componentsInChildren2[j]);
		}
		int childCount = _rootObj.transform.childCount;
		for (int k = 0; k < childCount; k++)
		{
			UnityEngine.Object.Destroy(_rootObj.transform.GetChild(k).gameObject);
		}
		if (WorldPreviewTerrain.TerrainPreviewMaterial)
		{
			Resources.UnloadAsset(WorldPreviewTerrain.TerrainPreviewMaterial);
			WorldPreviewTerrain.TerrainPreviewMaterial = null;
			GCUtils.UnloadAndCollectStart();
		}
	}

	// Token: 0x06007795 RID: 30613 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnDestroy()
	{
	}

	// Token: 0x06007796 RID: 30614 RVA: 0x0030AC7C File Offset: 0x00308E7C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.meshFilter = base.gameObject.AddComponent<MeshFilter>();
		this.meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
		this.meshRenderer.receiveShadows = false;
		this.meshRenderer.lightProbeUsage = LightProbeUsage.Off;
		this.meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		this.meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		this.meshFilter.sharedMesh = (this.mesh = new Mesh());
		base.gameObject.layer = 11;
	}

	// Token: 0x06007797 RID: 30615 RVA: 0x0030AD04 File Offset: 0x00308F04
	[PublicizedFrom(EAccessModifier.Private)]
	public void DrawMeshSector(Vector2i terrainSectorIndex, int metersPerDetailUnit)
	{
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		Vector2i vector2i = new Vector2i(terrainSectorIndex.x * 512, terrainSectorIndex.y * 512);
		int worldSize = WorldPreviewTerrain.worldBuilder.WorldSize;
		for (int i = vector2i.y; i <= vector2i.y + 512; i += metersPerDetailUnit)
		{
			Vector3 item;
			item.z = (float)i;
			for (int j = vector2i.x; j <= vector2i.x + 512; j += metersPerDetailUnit)
			{
				item.x = (float)j;
				item.y = WorldPreviewTerrain.worldBuilder.GetHeight(j, i);
				WorldPreviewTerrain.vertices.Add(item);
				Vector2 item2;
				item2.x = (float)j / (float)worldSize;
				item2.y = (float)i / (float)worldSize;
				WorldPreviewTerrain.uvs.Add(item2);
			}
		}
		int num = 0;
		int num2 = 512 / metersPerDetailUnit;
		for (int k = 0; k < 512; k += metersPerDetailUnit)
		{
			for (int l = 0; l < 512; l += metersPerDetailUnit)
			{
				WorldPreviewTerrain.triangles.Add(num);
				WorldPreviewTerrain.triangles.Add(num + num2 + 1);
				WorldPreviewTerrain.triangles.Add(num + 1);
				WorldPreviewTerrain.triangles.Add(num + 1);
				WorldPreviewTerrain.triangles.Add(num + num2 + 1);
				WorldPreviewTerrain.triangles.Add(num + num2 + 2);
				num++;
			}
			num++;
		}
		this.mesh.indexFormat = IndexFormat.UInt32;
		this.mesh.SetVertices(WorldPreviewTerrain.vertices);
		this.mesh.SetTriangles(WorldPreviewTerrain.triangles, 0);
		this.mesh.SetUVs(0, WorldPreviewTerrain.uvs);
		this.mesh.RecalculateNormals();
		WorldPreviewTerrain.vertices.Clear();
		WorldPreviewTerrain.triangles.Clear();
		WorldPreviewTerrain.uvs.Clear();
	}

	// Token: 0x04005B1E RID: 23326
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int TerrainSectorSize = 512;

	// Token: 0x04005B1F RID: 23327
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MeshFilter meshFilter;

	// Token: 0x04005B20 RID: 23328
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MeshRenderer meshRenderer;

	// Token: 0x04005B21 RID: 23329
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Mesh mesh;

	// Token: 0x04005B22 RID: 23330
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<Vector3> vertices = new List<Vector3>();

	// Token: 0x04005B23 RID: 23331
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<int> triangles = new List<int>();

	// Token: 0x04005B24 RID: 23332
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<Vector2> uvs = new List<Vector2>();

	// Token: 0x04005B25 RID: 23333
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Material TerrainPreviewMaterial;
}
