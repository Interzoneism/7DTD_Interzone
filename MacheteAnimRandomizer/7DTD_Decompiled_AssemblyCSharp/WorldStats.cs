using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

// Token: 0x02000A98 RID: 2712
public class WorldStats
{
	// Token: 0x17000854 RID: 2132
	// (get) Token: 0x060053C6 RID: 21446 RVA: 0x0021A74C File Offset: 0x0021894C
	public int TotalVertices
	{
		get
		{
			int num = 0;
			if (this.BlockEntities != null)
			{
				num = this.BlockEntities.Vertices;
			}
			if (this.ChunkMeshes != null)
			{
				foreach (WorldStats.MeshStats meshStats in this.ChunkMeshes)
				{
					if (meshStats != null)
					{
						num += meshStats.Vertices;
					}
				}
			}
			return num;
		}
	}

	// Token: 0x17000855 RID: 2133
	// (get) Token: 0x060053C7 RID: 21447 RVA: 0x0021A7A0 File Offset: 0x002189A0
	public int TotalTriangles
	{
		get
		{
			int num = 0;
			if (this.BlockEntities != null)
			{
				num = this.BlockEntities.Triangles;
			}
			if (this.ChunkMeshes != null)
			{
				foreach (WorldStats.MeshStats meshStats in this.ChunkMeshes)
				{
					if (meshStats != null)
					{
						num += meshStats.Triangles;
					}
				}
			}
			return num;
		}
	}

	// Token: 0x060053C8 RID: 21448 RVA: 0x0021A7F1 File Offset: 0x002189F1
	[PublicizedFrom(EAccessModifier.Private)]
	public WorldStats(WorldStats.MeshStats _blockEntities, WorldStats.MeshStats[] _chunkMeshes, float _lightsVolume)
	{
		this.BlockEntities = _blockEntities;
		this.ChunkMeshes = _chunkMeshes;
		this.LightsVolume = _lightsVolume;
	}

	// Token: 0x060053C9 RID: 21449 RVA: 0x0021A810 File Offset: 0x00218A10
	public static WorldStats FromProperties(DynamicProperties _props)
	{
		WorldStats.MeshStats blockEntities = null;
		DynamicProperties props;
		if (_props.Classes.TryGetValue("BlockEntities", out props))
		{
			blockEntities = WorldStats.MeshStats.FromProperties(props);
		}
		WorldStats.MeshStats[] array = new WorldStats.MeshStats[(MeshDescription.meshes.Length != 0) ? MeshDescription.meshes.Length : 20];
		for (int i = 0; i < array.Length; i++)
		{
			if (_props.Classes.TryGetValue("ChunkMeshes" + i.ToString(), out props))
			{
				array[i] = WorldStats.MeshStats.FromProperties(props);
			}
		}
		float lightsVolume = 0f;
		string text;
		if (_props.Values.TryGetValue("LightsVolume", out text) && !StringParsers.TryParseFloat(text, out lightsVolume, 0, -1, NumberStyles.Any))
		{
			Log.Warning("Could not parse LightsVolume string '" + text + "'");
		}
		return new WorldStats(blockEntities, array, lightsVolume);
	}

	// Token: 0x060053CA RID: 21450 RVA: 0x0021A8DC File Offset: 0x00218ADC
	public DynamicProperties ToProperties()
	{
		DynamicProperties dynamicProperties = new DynamicProperties();
		if (this.BlockEntities != null)
		{
			dynamicProperties.Classes["BlockEntities"] = this.BlockEntities.ToProperties();
		}
		if (this.ChunkMeshes != null)
		{
			for (int i = 0; i < this.ChunkMeshes.Length; i++)
			{
				if (this.ChunkMeshes[i] != null)
				{
					dynamicProperties.Classes["ChunkMeshes" + i.ToString()] = this.ChunkMeshes[i].ToProperties();
				}
			}
		}
		dynamicProperties.Values["LightsVolume"] = this.LightsVolume.ToCultureInvariantString();
		dynamicProperties.Values["TotalVertices"] = this.TotalVertices.ToString();
		dynamicProperties.Values["TotalTriangles"] = this.TotalTriangles.ToString();
		return dynamicProperties;
	}

	// Token: 0x060053CB RID: 21451 RVA: 0x0021A9B8 File Offset: 0x00218BB8
	public static WorldStats CaptureWorldStats()
	{
		MicroStopwatch microStopwatch = new MicroStopwatch(true);
		IEnumerable<ChunkGameObject> displayedChunkGameObjects = GameManager.Instance.World.m_ChunkManager.GetDisplayedChunkGameObjects();
		int vertices = 0;
		int triangles = 0;
		float lightsVolume = 0f;
		int[] array = new int[MeshDescription.meshes.Length];
		int[] array2 = new int[MeshDescription.meshes.Length];
		foreach (ChunkGameObject chunkGameObject in displayedChunkGameObjects)
		{
			Chunk chunk = chunkGameObject.GetChunk();
			if (!chunkGameObject.blockEntitiesParentT)
			{
				Log.Warning("{0} WorldStats CaptureWorldStats cgo {1}, null", new object[]
				{
					Time.frameCount,
					chunk
				});
			}
			else
			{
				foreach (object obj in chunkGameObject.blockEntitiesParentT)
				{
					Transform transform = (Transform)obj;
					WorldStats.calcLightsVolume(transform.gameObject, ref lightsVolume);
					WorldStats.calcComplexityGameObject(transform.gameObject, ref vertices, ref triangles, true, chunk);
				}
				for (int i = 0; i < 16; i++)
				{
					ChunkGameObjectLayer layer = chunkGameObject.GetLayer(i);
					if (layer != null)
					{
						for (int j = 0; j < layer.m_MeshFilter.Length; j++)
						{
							MeshFilter[] array3 = layer.m_MeshFilter[j];
							if (array3 != null)
							{
								foreach (MeshFilter meshFilter in array3)
								{
									if (meshFilter != null)
									{
										Mesh sharedMesh = meshFilter.sharedMesh;
										if (sharedMesh != null)
										{
											array[j] += sharedMesh.vertexCount;
											array2[j] += sharedMesh.triangles.Length / 3;
										}
									}
								}
							}
						}
					}
				}
			}
		}
		WorldStats.MeshStats[] array5 = new WorldStats.MeshStats[array.Length];
		for (int l = 0; l < array.Length; l++)
		{
			array5[l] = new WorldStats.MeshStats(array[l], array2[l]);
		}
		WorldStats result = new WorldStats(new WorldStats.MeshStats(vertices, triangles), array5, lightsVolume);
		Log.Out("Measuring took {0} ms", new object[]
		{
			microStopwatch.ElapsedMilliseconds
		});
		return result;
	}

	// Token: 0x060053CC RID: 21452 RVA: 0x0021AC20 File Offset: 0x00218E20
	[PublicizedFrom(EAccessModifier.Private)]
	public static void calcLightsVolume(GameObject _go, ref float _lightsVolume)
	{
		LightLOD componentInChildren = _go.GetComponentInChildren<LightLOD>();
		if (componentInChildren == null)
		{
			return;
		}
		if (!componentInChildren.bSwitchedOn)
		{
			return;
		}
		_lightsVolume += 4.1887903f * Mathf.Pow(componentInChildren.lightRange, 3f);
	}

	// Token: 0x060053CD RID: 21453 RVA: 0x0021AC64 File Offset: 0x00218E64
	[PublicizedFrom(EAccessModifier.Private)]
	public static void calcComplexityGameObject(GameObject _go, ref int _verts, ref int _tris, bool _onlyActive = true, Chunk _onChunk = null)
	{
		int num = 0;
		int num2 = 0;
		BlockEntityData blockEntity = _onChunk.GetBlockEntity(_go.transform);
		if (blockEntity == null)
		{
			Log.Warning("GameObject without BlockEntity: " + _go.GetGameObjectPath());
		}
		else if (blockEntity.blockValue.Block.IsSleeperBlock)
		{
			return;
		}
		HashSet<MeshRenderer> hashSet = new HashSet<MeshRenderer>();
		foreach (LODGroup lodgroup in _go.transform.GetComponentsInChildren<LODGroup>(!_onlyActive))
		{
			if (!(lodgroup == null))
			{
				LOD[] lods = lodgroup.GetLODs();
				for (int j = 0; j < lods.Length; j++)
				{
					foreach (Renderer renderer in lods[j].renderers)
					{
						if (!(renderer == null))
						{
							MeshRenderer meshRenderer = renderer as MeshRenderer;
							if (!(meshRenderer == null))
							{
								hashSet.Add(meshRenderer);
								if (j == 0 && meshRenderer.gameObject.activeInHierarchy)
								{
									MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
									if (!(component == null))
									{
										Mesh sharedMesh = component.sharedMesh;
										if (sharedMesh)
										{
											if (!sharedMesh.isReadable)
											{
												EntityMeshCache component2 = _go.GetComponent<EntityMeshCache>();
												CachedMeshData cachedMeshData;
												if (component2 != null && component2.TryGetMeshData(sharedMesh.name, out cachedMeshData))
												{
													num += cachedMeshData.vertexCount;
													num2 += cachedMeshData.triCount;
												}
											}
											else
											{
												num += sharedMesh.vertexCount;
												num2 += sharedMesh.triangles.Length;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		foreach (MeshRenderer meshRenderer2 in _go.transform.GetComponentsInChildren<MeshRenderer>(!_onlyActive))
		{
			if (!hashSet.Contains(meshRenderer2))
			{
				MeshFilter component3 = meshRenderer2.GetComponent<MeshFilter>();
				if (!(component3 == null))
				{
					Mesh sharedMesh2 = component3.sharedMesh;
					if (sharedMesh2)
					{
						if (!sharedMesh2.isReadable)
						{
							EntityMeshCache component4 = _go.GetComponent<EntityMeshCache>();
							CachedMeshData cachedMeshData2;
							if (component4 != null && component4.TryGetMeshData(sharedMesh2.name, out cachedMeshData2))
							{
								num += cachedMeshData2.vertexCount;
								num2 += cachedMeshData2.triCount;
							}
						}
						else
						{
							num += sharedMesh2.vertexCount;
							num2 += sharedMesh2.triangles.Length;
						}
					}
				}
			}
		}
		num2 /= 3;
		_verts += num;
		_tris += num2;
	}

	// Token: 0x060053CE RID: 21454 RVA: 0x0021AECE File Offset: 0x002190CE
	public static IEnumerator CaptureCameraStatsCo(XUi _xui)
	{
		PrefabEditModeManager.Instance.UpdateMinMax();
		Vector3i minPos = PrefabEditModeManager.Instance.minPos;
		Vector3i maxPos = PrefabEditModeManager.Instance.maxPos;
		BoundsInt boundsInt = new BoundsInt(minPos, maxPos);
		GameObject uiRootObj = UnityEngine.Object.FindObjectOfType<UIRoot>().gameObject;
		Camera playerCam = _xui.playerUI.entityPlayer.playerCamera;
		GameObject camObj = new GameObject("StatsCam");
		Camera camera = camObj.AddComponent<Camera>();
		camera.CopyFrom(playerCam);
		camera.farClipPlane = 10000f;
		camObj.AddComponent<AudioListener>();
		Transform transform = camera.transform;
		transform.position = new Vector3(boundsInt.center.x, (float)(boundsInt.yMax + 2000), boundsInt.center.z);
		transform.eulerAngles = new Vector3(90f, 0f, 0f);
		float oldLodBias = QualitySettings.lodBias;
		ShadowQuality oldShadowQuality = QualitySettings.shadows;
		float oldShadowDistance = QualitySettings.shadowDistance;
		bool oldDisableChunkLoDs = GamePrefs.GetBool(EnumGamePrefs.OptionsDisableChunkLODs);
		ReflectionManager.ApplyOptions(true);
		playerCam.gameObject.SetActive(false);
		uiRootObj.SetActive(false);
		QualitySettings.lodBias = 1000000f;
		QualitySettings.shadows = ShadowQuality.Disable;
		QualitySettings.shadowDistance = 1000000f;
		SkyManager.skyManager.gameObject.SetActive(false);
		GamePrefs.Set(EnumGamePrefs.OptionsDisableChunkLODs, true);
		IList<ChunkGameObject> displayedChunkGameObjects = GameManager.Instance.World.m_ChunkManager.GetDisplayedChunkGameObjects();
		for (int i = 0; i < displayedChunkGameObjects.Count; i++)
		{
			displayedChunkGameObjects[i].CheckLODs(-1);
		}
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		ReflectionManager.ApplyOptions(false);
		playerCam.gameObject.SetActive(true);
		uiRootObj.SetActive(true);
		QualitySettings.lodBias = oldLodBias;
		QualitySettings.shadows = oldShadowQuality;
		QualitySettings.shadowDistance = oldShadowDistance;
		SkyManager.skyManager.gameObject.SetActive(true);
		GamePrefs.Set(EnumGamePrefs.OptionsDisableChunkLODs, oldDisableChunkLoDs);
		UnityEngine.Object.Destroy(camObj);
		yield break;
	}

	// Token: 0x04004011 RID: 16401
	public readonly WorldStats.MeshStats BlockEntities;

	// Token: 0x04004012 RID: 16402
	public readonly WorldStats.MeshStats[] ChunkMeshes;

	// Token: 0x04004013 RID: 16403
	public readonly float LightsVolume;

	// Token: 0x02000A99 RID: 2713
	public class MeshStats
	{
		// Token: 0x060053CF RID: 21455 RVA: 0x0021AEDD File Offset: 0x002190DD
		public MeshStats(int _vertices, int _triangles)
		{
			this.Vertices = _vertices;
			this.Triangles = _triangles;
		}

		// Token: 0x060053D0 RID: 21456 RVA: 0x0021AEF4 File Offset: 0x002190F4
		public static WorldStats.MeshStats FromProperties(DynamicProperties _props)
		{
			int @int = _props.GetInt("Vertices");
			int int2 = _props.GetInt("Triangles");
			return new WorldStats.MeshStats(@int, int2);
		}

		// Token: 0x060053D1 RID: 21457 RVA: 0x0021AF20 File Offset: 0x00219120
		public DynamicProperties ToProperties()
		{
			DynamicProperties dynamicProperties = new DynamicProperties();
			dynamicProperties.Values["Vertices"] = this.Vertices.ToString();
			dynamicProperties.Values["Triangles"] = this.Triangles.ToString();
			return dynamicProperties;
		}

		// Token: 0x04004014 RID: 16404
		public readonly int Vertices;

		// Token: 0x04004015 RID: 16405
		public readonly int Triangles;
	}
}
