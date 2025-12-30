using System;
using System.Collections.Generic;
using UnityEngine;
using WorldGenerationEngineFinal;

// Token: 0x02001100 RID: 4352
public class POIMarkerToolManager
{
	// Token: 0x17000E45 RID: 3653
	// (get) Token: 0x060088B9 RID: 35001 RVA: 0x003758A3 File Offset: 0x00373AA3
	public static PrefabManagerData prefabManagerData
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			PrefabManagerData result;
			if ((result = POIMarkerToolManager.m_prefabManagerData) == null)
			{
				result = (POIMarkerToolManager.m_prefabManagerData = new PrefabManagerData());
			}
			return result;
		}
	}

	// Token: 0x060088BA RID: 35002 RVA: 0x003758BC File Offset: 0x00373ABC
	public static void CleanUp()
	{
		PrefabManagerData prefabManagerData = POIMarkerToolManager.m_prefabManagerData;
		if (prefabManagerData != null)
		{
			prefabManagerData.Cleanup();
		}
		POIMarkerToolManager.m_prefabManagerData = null;
		if (POIMarkerToolManager.POIMarkers != null)
		{
			foreach (KeyValuePair<Vector3i, List<POIMarkerToolManager.PrefabAndPos>> keyValuePair in POIMarkerToolManager.POIMarkers)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					Transform prefabTrans = keyValuePair.Value[i].prefabTrans;
					if (prefabTrans && prefabTrans.gameObject != null)
					{
						UnityEngine.Object.Destroy(prefabTrans.gameObject);
					}
				}
			}
			POIMarkerToolManager.POIMarkers.Clear();
		}
		POIMarkerToolManager.ClearPOIMarkers();
	}

	// Token: 0x060088BB RID: 35003 RVA: 0x00375984 File Offset: 0x00373B84
	public static void RegisterPOIMarker(SelectionBox _selBox)
	{
		if (POIMarkerToolManager.registeredPOIMarkers.Contains(_selBox))
		{
			return;
		}
		POIMarkerToolManager.registeredPOIMarkers.Add(_selBox);
		if (_selBox)
		{
			Prefab.Marker marker = _selBox.UserData as Prefab.Marker;
			if (marker != null && marker.MarkerType == Prefab.Marker.MarkerTypes.PartSpawn && marker.PartToSpawn != null && marker.PartToSpawn.Length > 0 && marker.PartDirty)
			{
				POIMarkerToolManager.spawnPrefabViz(marker, _selBox);
			}
		}
	}

	// Token: 0x060088BC RID: 35004 RVA: 0x003759F0 File Offset: 0x00373BF0
	public static void DisplayPrefabPreviewForMarker(SelectionBox _selBox)
	{
		if (POIMarkerToolManager.prefabManagerData.AllPrefabDatas.Count == 0)
		{
			ThreadManager.RunCoroutineSync(POIMarkerToolManager.prefabManagerData.LoadPrefabs());
			POIMarkerToolManager.prefabManagerData.ShufflePrefabData(GameRandomManager.Instance.BaseSeed);
		}
		Prefab.Marker marker = _selBox.UserData as Prefab.Marker;
		if (marker == null)
		{
			return;
		}
		string text = "ghosttown,countrytown";
		if (PrefabEditModeManager.Instance.VoxelPrefab != null)
		{
			text = PrefabEditModeManager.Instance.VoxelPrefab.PrefabName;
			text = text.Replace("rwg_tile_", "");
			text = text.Split('_', StringSplitOptions.None)[0];
		}
		bool useAnySizeSmaller = !Prefab.Marker.MarkerSizes.Contains(new Vector3i(marker.Size.x, 0, marker.Size.z));
		Prefab prefab = POIMarkerToolManager.prefabManagerData.GetPreviewPrefabWithAnyTags(FastTags<TagGroup.Poi>.Parse(text), -1, new Vector2i(marker.Size.x, marker.Size.z), useAnySizeSmaller);
		if (prefab == null)
		{
			return;
		}
		prefab = prefab.Clone(false);
		int x = prefab.size.x;
		int z = prefab.size.z;
		prefab.RotateY(true, (prefab.rotationToFaceNorth + (int)marker.Rotations) % 4);
		Transform transform = _selBox.transform.Find("PrefabPreview");
		if (transform != null)
		{
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		GameManager.Instance.StartCoroutine(prefab.ToTransform(true, true, true, false, _selBox.transform, "PrefabPreview", new Vector3(-((float)x / 2f), (float)prefab.yOffset + 0.15f, -((float)z / 2f)), 0));
	}

	// Token: 0x060088BD RID: 35005 RVA: 0x00375B83 File Offset: 0x00373D83
	public static void UnRegisterPOIMarker(SelectionBox _selBox)
	{
		if (!POIMarkerToolManager.registeredPOIMarkers.Contains(_selBox))
		{
			return;
		}
		POIMarkerToolManager.registeredPOIMarkers.Remove(_selBox);
	}

	// Token: 0x060088BE RID: 35006 RVA: 0x00375B9F File Offset: 0x00373D9F
	public static void ClearPOIMarkers()
	{
		POIMarkerToolManager.registeredPOIMarkers.Clear();
	}

	// Token: 0x060088BF RID: 35007 RVA: 0x00375BAC File Offset: 0x00373DAC
	public static void SelectionChanged(SelectionBox selBox)
	{
		if (selBox && selBox != POIMarkerToolManager.currentSelectionBox)
		{
			POIMarkerToolManager.previousSelectionBox = POIMarkerToolManager.currentSelectionBox;
			POIMarkerToolManager.currentSelectionBox = selBox;
		}
		POIMarkerToolManager.currentSelectionBox = selBox;
		if (XUiC_WoPropsPOIMarker.Instance != null && POIMarkerToolManager.currentSelectionBox != null && POIMarkerToolManager.currentSelectionBox.UserData is Prefab.Marker)
		{
			XUiC_WoPropsPOIMarker.Instance.CurrentMarker = (POIMarkerToolManager.currentSelectionBox.UserData as Prefab.Marker);
		}
		POIMarkerToolManager.UpdateAllColors();
	}

	// Token: 0x060088C0 RID: 35008 RVA: 0x00375C28 File Offset: 0x00373E28
	public static void spawnPrefabViz(Prefab.Marker _currentMarker, SelectionBox selBox)
	{
		Transform transform = selBox.transform.Find("PrefabPreview");
		if (transform != null)
		{
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		Prefab prefab = new Prefab();
		prefab.Load(_currentMarker.PartToSpawn, true, false, true, false);
		prefab.Init(0, 0);
		prefab.RotateY(false, (prefab.rotationToFaceNorth + (int)_currentMarker.Rotations) % 4);
		ValueTuple<SelectionCategory, SelectionBox>? valueTuple;
		GameManager.Instance.StartCoroutine(prefab.ToTransform(true, true, true, false, (SelectionBoxManager.Instance.Selection != null) ? valueTuple.GetValueOrDefault().Item2.transform : null, "PrefabPreview", new Vector3(-((float)prefab.size.x / 2f), 0.1f, -((float)prefab.size.z / 2f)), 0));
	}

	// Token: 0x060088C1 RID: 35009 RVA: 0x00375D04 File Offset: 0x00373F04
	public static void UpdateAllColors()
	{
		int num = 0;
		if (POIMarkerToolManager.currentSelectionBox)
		{
			Prefab.Marker marker = POIMarkerToolManager.currentSelectionBox.UserData as Prefab.Marker;
			if (marker != null)
			{
				num = marker.GroupId;
			}
		}
		for (int i = 0; i < POIMarkerToolManager.registeredPOIMarkers.Count; i++)
		{
			SelectionBox selectionBox = POIMarkerToolManager.registeredPOIMarkers[i];
			Prefab.Marker marker = selectionBox.UserData as Prefab.Marker;
			if (marker.GroupId == num)
			{
				selectionBox.SetAllFacesColor(marker.GroupColor + new Color(0.2f, 0.2f, 0.2f, 0f), true);
			}
			else
			{
				selectionBox.SetAllFacesColor(marker.GroupColor, true);
			}
		}
		if (POIMarkerToolManager.currentSelectionBox)
		{
			Prefab.Marker marker = POIMarkerToolManager.currentSelectionBox.UserData as Prefab.Marker;
			if (marker != null)
			{
				if (marker.MarkerType == Prefab.Marker.MarkerTypes.PartSpawn)
				{
					POIMarkerToolManager.currentSelectionBox.SetAllFacesColor(new Color(0f, 0f, 0f, 0f), true);
					return;
				}
				POIMarkerToolManager.currentSelectionBox.SetAllFacesColor(marker.GroupColor + new Color(0.5f, 0.5f, 0.5f, 0f), true);
			}
		}
	}

	// Token: 0x060088C2 RID: 35010 RVA: 0x00375E28 File Offset: 0x00374028
	public static void ShowPOIMarkers(bool bShow = true)
	{
		if (POIMarkerToolManager.POIMarkers == null)
		{
			return;
		}
		foreach (KeyValuePair<Vector3i, List<POIMarkerToolManager.PrefabAndPos>> keyValuePair in POIMarkerToolManager.POIMarkers)
		{
			for (int i = 0; i < keyValuePair.Value.Count; i++)
			{
				Transform prefabTrans = keyValuePair.Value[i].prefabTrans;
				if (prefabTrans && prefabTrans.gameObject != null)
				{
					prefabTrans.gameObject.SetActive(bShow);
				}
			}
		}
	}

	// Token: 0x04006AB3 RID: 27315
	[PublicizedFrom(EAccessModifier.Private)]
	public static PrefabManagerData m_prefabManagerData;

	// Token: 0x04006AB4 RID: 27316
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<Vector3i, List<POIMarkerToolManager.PrefabAndPos>> POIMarkers = null;

	// Token: 0x04006AB5 RID: 27317
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<SelectionBox> registeredPOIMarkers = new List<SelectionBox>();

	// Token: 0x04006AB6 RID: 27318
	[PublicizedFrom(EAccessModifier.Private)]
	public static Material markerMat;

	// Token: 0x04006AB7 RID: 27319
	public static SelectionBox currentSelectionBox;

	// Token: 0x04006AB8 RID: 27320
	[PublicizedFrom(EAccessModifier.Private)]
	public static SelectionBox previousSelectionBox;

	// Token: 0x02001101 RID: 4353
	[PublicizedFrom(EAccessModifier.Private)]
	public struct PrefabAndPos
	{
		// Token: 0x04006AB9 RID: 27321
		public Transform prefabTrans;

		// Token: 0x04006ABA RID: 27322
		public Vector3i position;
	}
}
