using System;
using System.Collections.Generic;
using Platform;
using UnityEngine;
using WorldGenerationEngineFinal;

// Token: 0x02000EC5 RID: 3781
public class PrefabPreviewManager
{
	// Token: 0x17000C23 RID: 3107
	// (get) Token: 0x0600779A RID: 30618 RVA: 0x0030A236 File Offset: 0x00308436
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

	// Token: 0x0600779B RID: 30619 RVA: 0x0030AF20 File Offset: 0x00309120
	public void InitPrefabs()
	{
		if (this.initialized)
		{
			return;
		}
		this.prefabsAround.Clear();
		PrefabPreviewManager.worldBuilder.PrefabManager.GetPrefabsAround(XUiC_WorldGenerationWindowGroup.Instance.PreviewWindow.GetCameraPosition(), 100000f, this.prefabsAround);
		this.UpdatePrefabsAround(this.prefabsAround);
		this.initialized = true;
	}

	// Token: 0x0600779C RID: 30620 RVA: 0x0030AF7D File Offset: 0x0030917D
	public void RemovePrefabs()
	{
		this.ClearDisplayedPrefabs();
		this.displayedPrefabs.Clear();
		this.prefabsAround.Clear();
		this.initialized = false;
	}

	// Token: 0x0600779D RID: 30621 RVA: 0x0030AFA2 File Offset: 0x003091A2
	public void Update()
	{
		if (Time.time - this.lastDisplayUpdate < 2f)
		{
			return;
		}
		this.lastDisplayUpdate = Time.time;
		this.ForceUpdate();
	}

	// Token: 0x0600779E RID: 30622 RVA: 0x0030AFCC File Offset: 0x003091CC
	public void ForceUpdate()
	{
		this.UpdateDisplay();
		if (Time.time - this.lastTime < 2f)
		{
			return;
		}
		this.lastTime = Time.time;
		if (PrefabPreviewManager.worldBuilder == null)
		{
			return;
		}
		if (XUiC_WorldGenerationWindowGroup.Instance == null)
		{
			return;
		}
		if (XUiC_WorldGenerationWindowGroup.Instance.PreviewWindow == null)
		{
			return;
		}
		if (PrefabPreviewManager.worldBuilder.PrefabManager.UsedPrefabsWorld == null)
		{
			return;
		}
		this.InitPrefabs();
	}

	// Token: 0x0600779F RID: 30623 RVA: 0x0030B034 File Offset: 0x00309234
	public void ClearDisplayedPrefabs()
	{
		if (this.displayedPrefabs == null || this.displayedPrefabs.Count == 0)
		{
			return;
		}
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, PrefabPreviewManager.PrefabGameObject> keyValuePair in this.displayedPrefabs)
		{
			list.Add(keyValuePair.Key);
		}
		foreach (int key in list)
		{
			if (!(this.displayedPrefabs[key].go == null))
			{
				MeshFilter[] componentsInChildren = this.displayedPrefabs[key].go.GetComponentsInChildren<MeshFilter>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					UnityEngine.Object.Destroy(componentsInChildren[i].mesh);
				}
				Renderer[] componentsInChildren2 = this.displayedPrefabs[key].go.GetComponentsInChildren<Renderer>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					UnityEngine.Object.Destroy(componentsInChildren2[j].material);
				}
				UnityEngine.Object.Destroy(this.displayedPrefabs[key].go);
				this.displayedPrefabs.Remove(key);
			}
		}
	}

	// Token: 0x060077A0 RID: 30624 RVA: 0x0030B1A0 File Offset: 0x003093A0
	public void UpdatePrefabsAround(Dictionary<int, PrefabDataInstance> _prefabsAround)
	{
		foreach (KeyValuePair<int, PrefabDataInstance> keyValuePair in _prefabsAround)
		{
			PrefabDataInstance value = keyValuePair.Value;
			if (!this.displayedPrefabs.ContainsKey(value.id))
			{
				string name = value.location.Name;
				if (PathAbstractions.PrefabImpostersSearchPaths.GetLocation(name, null, null).Type != PathAbstractions.EAbstractedLocationType.None)
				{
					PrefabPreviewManager.PrefabGameObject prefabGameObject = new PrefabPreviewManager.PrefabGameObject();
					prefabGameObject.prefabInstance = value;
					this.displayedPrefabs.Add(value.id, prefabGameObject);
				}
			}
		}
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, PrefabPreviewManager.PrefabGameObject> keyValuePair2 in this.displayedPrefabs)
		{
			if (!_prefabsAround.ContainsKey(keyValuePair2.Key))
			{
				list.Add(keyValuePair2.Key);
			}
		}
		foreach (int key in list)
		{
			if (!(this.displayedPrefabs[key].go == null))
			{
				MeshFilter[] componentsInChildren = this.displayedPrefabs[key].go.GetComponentsInChildren<MeshFilter>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					UnityEngine.Object.Destroy(componentsInChildren[i].mesh);
				}
				Renderer[] componentsInChildren2 = this.displayedPrefabs[key].go.GetComponentsInChildren<Renderer>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					UnityEngine.Object.Destroy(componentsInChildren2[j].material);
				}
				UnityEngine.Object.Destroy(this.displayedPrefabs[key].go);
				this.displayedPrefabs.Remove(key);
			}
		}
	}

	// Token: 0x060077A1 RID: 30625 RVA: 0x0030B39C File Offset: 0x0030959C
	public void UpdateDisplay()
	{
		if (XUiC_WorldGenerationWindowGroup.Instance.PreviewQualityLevel == XUiC_WorldGenerationWindowGroup.PreviewQuality.NoPreview)
		{
			return;
		}
		MicroStopwatch microStopwatch = new MicroStopwatch();
		if (this.parentTransform == null)
		{
			this.parentTransform = new GameObject("PrefabsLOD").transform;
			this.parentTransform.gameObject.layer = 11;
		}
		foreach (KeyValuePair<int, PrefabPreviewManager.PrefabGameObject> keyValuePair in this.displayedPrefabs)
		{
			PrefabPreviewManager.PrefabGameObject value = keyValuePair.Value;
			PrefabDataInstance prefabInstance = value.prefabInstance;
			Vector3 vector = prefabInstance.boundingBoxPosition.ToVector3();
			Vector3 a = prefabInstance.boundingBoxPosition.ToVector3();
			Vector3 vector2 = prefabInstance.boundingBoxSize.ToVector3();
			if (prefabInstance.rotation % 2 == 0)
			{
				vector += new Vector3((float)prefabInstance.boundingBoxSize.x * 0.5f, 0f, (float)prefabInstance.boundingBoxSize.z * 0.5f);
				a += new Vector3((float)prefabInstance.boundingBoxSize.x * 0.5f, 0f, (float)prefabInstance.boundingBoxSize.z * 0.5f);
			}
			else
			{
				vector += new Vector3((float)prefabInstance.boundingBoxSize.z * 0.5f, 0f, (float)prefabInstance.boundingBoxSize.x * 0.5f);
				a += new Vector3((float)prefabInstance.boundingBoxSize.z * 0.5f, 0f, (float)prefabInstance.boundingBoxSize.x * 0.5f);
				vector2 = new Vector3(vector2.z, vector2.y, vector2.x);
			}
			Vector3 zero = Vector3.zero;
			Quaternion rotation = Quaternion.identity;
			switch (prefabInstance.rotation)
			{
			case 0:
				zero = new Vector3(-0.5f, 0f, -0.5f);
				break;
			case 1:
				zero = new Vector3(0.5f, 0f, -0.5f);
				rotation = Quaternion.Euler(0f, 270f, 0f);
				break;
			case 2:
				zero = new Vector3(0.5f, 0f, 0.5f);
				rotation = Quaternion.Euler(0f, 180f, 0f);
				break;
			case 3:
				zero = new Vector3(-0.5f, 0f, 0.5f);
				rotation = Quaternion.Euler(0f, 90f, 0f);
				break;
			}
			if (Utils.FastAbs(vector.x - (float)((int)vector.x)) > 0.001f)
			{
				vector.x += zero.x;
			}
			if (Utils.FastAbs(vector.z - (float)((int)vector.z)) > 0.001f)
			{
				vector.z += zero.z;
			}
			float num = 0f;
			Utils.DrawBounds(new Bounds(a + new Vector3(0f, (float)prefabInstance.boundingBoxSize.y * 0.5f + 0.1f + num, 0f) - Origin.position, vector2), Color.green, 2f);
			if (!value.go)
			{
				XUiC_WorldGenerationWindowGroup.PreviewQuality previewQuality = XUiC_WorldGenerationWindowGroup.Instance.PreviewQualityLevel;
				if (((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent() || SystemInfo.systemMemorySize < 1300) && previewQuality > XUiC_WorldGenerationWindowGroup.PreviewQuality.Default)
				{
					previewQuality = XUiC_WorldGenerationWindowGroup.PreviewQuality.Default;
				}
				GameObject gameObject;
				if (previewQuality == XUiC_WorldGenerationWindowGroup.PreviewQuality.Highest)
				{
					string name = prefabInstance.location.Name;
					gameObject = SimpleMeshFile.ReadGameObject(PathAbstractions.PrefabImpostersSearchPaths.GetLocation(name, null, null), 0f, null, true, false, null, null);
					if (gameObject == null)
					{
						continue;
					}
					Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].gameObject.layer = 11;
					}
				}
				else if (previewQuality >= XUiC_WorldGenerationWindowGroup.PreviewQuality.Low && previewQuality <= XUiC_WorldGenerationWindowGroup.PreviewQuality.High)
				{
					gameObject = new GameObject();
					if (!prefabInstance.prefab.Name.Contains("rwg_tile") && !prefabInstance.prefab.Name.Contains("part_driveway"))
					{
						Transform transform = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
						transform.SetParent(gameObject.transform);
						transform.localPosition = new Vector3(0f, (float)(prefabInstance.boundingBoxSize.y / 2), 0f);
						transform.localScale = prefabInstance.boundingBoxSize.ToVector3();
						if ((int)(prefabInstance.previewColor.r + prefabInstance.previewColor.g + prefabInstance.previewColor.b) != 765)
						{
							transform.GetComponent<Renderer>().material.color = prefabInstance.previewColor;
						}
						Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							componentsInChildren[i].gameObject.layer = 11;
						}
					}
				}
				else
				{
					gameObject = new GameObject();
				}
				value.go = gameObject;
				gameObject.layer = 11;
				Transform transform2 = gameObject.transform;
				transform2.name = prefabInstance.location.Name;
				transform2.SetParent(this.parentTransform, false);
				Vector3 position = vector + new Vector3(0f, 0.01f + num - 4f, 0f) - Origin.position;
				transform2.SetPositionAndRotation(position, rotation);
				GameObject gameObject2 = new GameObject(prefabInstance.prefab.Name);
				Transform transform3 = gameObject2.transform;
				transform3.SetParent(transform2);
				transform3.rotation = Quaternion.Euler(90f, gameObject.transform.rotation.eulerAngles.y, 0f);
				transform3.localPosition = new Vector3(0f, (float)(prefabInstance.boundingBoxSize.y + prefabInstance.prefab.yOffset) + 0.25f, 0f);
				gameObject2.layer = 11;
				Vector2i vector2i = new Vector2i(((int)vector.x + PrefabPreviewManager.worldBuilder.WorldSize / 2) / 150, ((int)vector.z + PrefabPreviewManager.worldBuilder.WorldSize / 2) / 150);
				TextMesh textMesh = gameObject2.AddMissingComponent<TextMesh>();
				textMesh.alignment = TextAlignment.Center;
				textMesh.anchor = TextAnchor.MiddleCenter;
				textMesh.fontSize = (prefabInstance.prefab.Name.Contains("trader") ? 100 : 20);
				textMesh.color = (prefabInstance.prefab.Name.Contains("trader") ? Color.red : Color.green);
				textMesh.text = string.Concat(new string[]
				{
					prefabInstance.prefab.Name,
					Environment.NewLine,
					string.Format("pos {0}{1}", prefabInstance.boundingBoxPosition, Environment.NewLine),
					string.Format("yoffset {0}{1}", prefabInstance.prefab.yOffset, Environment.NewLine),
					string.Format("rots to north {0}, total left {1}{2}", prefabInstance.prefab.RotationsToNorth, prefabInstance.rotation, Environment.NewLine),
					string.Format("tile pos {0}{1}", vector2i, Environment.NewLine),
					string.Format("score {0}", prefabInstance.prefab.DensityScore)
				});
				if (microStopwatch.ElapsedMilliseconds > 50L)
				{
					this.lastDisplayUpdate = 0f;
					return;
				}
			}
		}
		foreach (KeyValuePair<int, PrefabPreviewManager.PrefabGameObject> keyValuePair2 in this.displayedPrefabs)
		{
			if (!(keyValuePair2.Value.go == null))
			{
				Transform transform4 = keyValuePair2.Value.go.transform;
				for (int j = 0; j < transform4.childCount; j++)
				{
					transform4.GetChild(j).gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x060077A2 RID: 30626 RVA: 0x0030BC0C File Offset: 0x00309E0C
	public void ClearOldPreview()
	{
		this.RemovePrefabs();
	}

	// Token: 0x060077A3 RID: 30627 RVA: 0x0030BC14 File Offset: 0x00309E14
	public void Cleanup()
	{
		this.RemovePrefabs();
		if (this.parentTransform)
		{
			UnityEngine.Object.Destroy(this.parentTransform.gameObject);
			this.parentTransform = null;
		}
	}

	// Token: 0x04005B26 RID: 23334
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cPrefabYPosition = 4f;

	// Token: 0x04005B27 RID: 23335
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDisplayUpdateDelay = 2f;

	// Token: 0x04005B28 RID: 23336
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cPrefabListUpdateDelay = 2f;

	// Token: 0x04005B29 RID: 23337
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cLodPoiDistance = 100000;

	// Token: 0x04005B2A RID: 23338
	public static bool ReadyToDisplay;

	// Token: 0x04005B2B RID: 23339
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<int, PrefabDataInstance> prefabsAround = new Dictionary<int, PrefabDataInstance>();

	// Token: 0x04005B2C RID: 23340
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<int, PrefabPreviewManager.PrefabGameObject> displayedPrefabs = new Dictionary<int, PrefabPreviewManager.PrefabGameObject>();

	// Token: 0x04005B2D RID: 23341
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform parentTransform;

	// Token: 0x04005B2E RID: 23342
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastTime;

	// Token: 0x04005B2F RID: 23343
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastDisplayUpdate;

	// Token: 0x04005B30 RID: 23344
	public bool initialized;

	// Token: 0x02000EC6 RID: 3782
	[PublicizedFrom(EAccessModifier.Private)]
	public class PrefabGameObject
	{
		// Token: 0x04005B31 RID: 23345
		public PrefabDataInstance prefabInstance;

		// Token: 0x04005B32 RID: 23346
		public GameObject go;
	}
}
