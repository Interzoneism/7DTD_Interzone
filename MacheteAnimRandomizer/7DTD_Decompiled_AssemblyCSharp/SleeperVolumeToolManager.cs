using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020010FE RID: 4350
public class SleeperVolumeToolManager
{
	// Token: 0x060088A5 RID: 34981 RVA: 0x00374D5C File Offset: 0x00372F5C
	public static void RegisterSleeperBlock(BlockValue _bv, Transform prefabTrans, Vector3i position)
	{
		BlockSleeper blockSleeper = _bv.Block as BlockSleeper;
		if (blockSleeper == null)
		{
			Log.Warning("SleeperVolumeToolManager RegisterSleeperBlock not sleeper {0}", new object[]
			{
				_bv
			});
			return;
		}
		if (SleeperVolumeToolManager.sleepers == null)
		{
			SleeperVolumeToolManager.sleepers = new Dictionary<Vector3i, List<SleeperVolumeToolManager.BlockData>>();
			Shader shader = Shader.Find("Game/UI/Sleeper");
			for (int i = 0; i < SleeperVolumeToolManager.typeColors.Length; i++)
			{
				Material material = new Material(shader);
				material.renderQueue = 4001;
				material.color = SleeperVolumeToolManager.typeColors[i];
				SleeperVolumeToolManager.typeMats.Add(material);
			}
		}
		Vector3i worldPos = GameManager.Instance.World.ChunkClusters[0].GetChunkFromWorldPos(position).GetWorldPos();
		SleeperVolumeToolManager.BlockData blockData = new SleeperVolumeToolManager.BlockData();
		blockData.block = blockSleeper;
		blockData.prefabT = prefabTrans;
		blockData.position = position;
		prefabTrans.position = prefabTrans.position + position.ToVector3() + Vector3.one * 0.5f + Vector3.up * 0.01f;
		if (SleeperVolumeToolManager.GroupGameObject == null)
		{
			SleeperVolumeToolManager.GroupGameObject = new GameObject();
			SleeperVolumeToolManager.GroupGameObject.name = "SleeperVolumeToolManagerPrefabs";
		}
		prefabTrans.parent = SleeperVolumeToolManager.GroupGameObject.transform;
		List<SleeperVolumeToolManager.BlockData> list;
		if (!SleeperVolumeToolManager.sleepers.TryGetValue(worldPos, out list))
		{
			list = new List<SleeperVolumeToolManager.BlockData>();
			SleeperVolumeToolManager.sleepers.Add(worldPos, list);
		}
		list.Add(blockData);
		SleeperVolumeToolManager.UpdateSleeperVisuals(blockData);
	}

	// Token: 0x060088A6 RID: 34982 RVA: 0x00374EDC File Offset: 0x003730DC
	public static void UnRegisterSleeperBlock(Vector3i position)
	{
		if (SleeperVolumeToolManager.sleepers == null)
		{
			return;
		}
		Vector3i worldPos = GameManager.Instance.World.ChunkClusters[0].GetChunkFromWorldPos(position).GetWorldPos();
		List<SleeperVolumeToolManager.BlockData> list;
		if (SleeperVolumeToolManager.sleepers.TryGetValue(worldPos, out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].position == position)
				{
					UnityEngine.Object.Destroy(list[i].prefabT.gameObject);
					list.RemoveAt(i);
					return;
				}
			}
		}
	}

	// Token: 0x060088A7 RID: 34983 RVA: 0x00374F64 File Offset: 0x00373164
	public static void CleanUp()
	{
		if (SleeperVolumeToolManager.sleepers != null)
		{
			foreach (KeyValuePair<Vector3i, List<SleeperVolumeToolManager.BlockData>> keyValuePair in SleeperVolumeToolManager.sleepers)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					Transform prefabT = keyValuePair.Value[i].prefabT;
					if (prefabT)
					{
						UnityEngine.Object.Destroy(prefabT.gameObject);
					}
				}
			}
			SleeperVolumeToolManager.sleepers.Clear();
		}
		SleeperVolumeToolManager.ClearSleeperVolumes();
	}

	// Token: 0x060088A8 RID: 34984 RVA: 0x00375004 File Offset: 0x00373204
	public static void RegisterSleeperVolume(SelectionBox _selBox)
	{
		if (SleeperVolumeToolManager.registeredSleeperVolumes.Contains(_selBox))
		{
			return;
		}
		SleeperVolumeToolManager.registeredSleeperVolumes.Add(_selBox);
	}

	// Token: 0x060088A9 RID: 34985 RVA: 0x0037501F File Offset: 0x0037321F
	public static void UnRegisterSleeperVolume(SelectionBox _selBox)
	{
		if (!SleeperVolumeToolManager.registeredSleeperVolumes.Contains(_selBox))
		{
			return;
		}
		SleeperVolumeToolManager.registeredSleeperVolumes.Remove(_selBox);
	}

	// Token: 0x060088AA RID: 34986 RVA: 0x0037503B File Offset: 0x0037323B
	public static void ClearSleeperVolumes()
	{
		SleeperVolumeToolManager.registeredSleeperVolumes.Clear();
	}

	// Token: 0x060088AB RID: 34987 RVA: 0x00375048 File Offset: 0x00373248
	public static void CheckKeys()
	{
		if (Input.GetKeyDown(KeyCode.RightBracket) && SleeperVolumeToolManager.currentSelectionBox)
		{
			Prefab.PrefabSleeperVolume prefabSleeperVolume = (Prefab.PrefabSleeperVolume)SleeperVolumeToolManager.currentSelectionBox.UserData;
			if (prefabSleeperVolume != null)
			{
				short num = -1;
				if (InputUtils.ShiftKeyPressed)
				{
					num = 0;
				}
				else if (SleeperVolumeToolManager.previousSelectionBox)
				{
					Prefab.PrefabSleeperVolume prefabSleeperVolume2 = (Prefab.PrefabSleeperVolume)SleeperVolumeToolManager.previousSelectionBox.UserData;
					if (prefabSleeperVolume2 != null)
					{
						num = prefabSleeperVolume2.groupId;
						if (num == 0)
						{
							PrefabInstance selectedPrefabInstance = XUiC_WoPropsSleeperVolume.selectedPrefabInstance;
							if (selectedPrefabInstance != null)
							{
								num = selectedPrefabInstance.prefab.FindSleeperVolumeFreeGroupId();
								prefabSleeperVolume2.groupId = num;
								Log.Out("Set sleeper volume {0} to new group ID {1}", new object[]
								{
									prefabSleeperVolume2.startPos,
									num
								});
							}
						}
					}
				}
				if (num >= 0)
				{
					prefabSleeperVolume.groupId = num;
					SleeperVolumeToolManager.SelectionChanged(SleeperVolumeToolManager.currentSelectionBox);
					Log.Out("Set sleeper volume {0} to group ID {1}", new object[]
					{
						prefabSleeperVolume.startPos,
						num
					});
				}
			}
		}
	}

	// Token: 0x060088AC RID: 34988 RVA: 0x0037513B File Offset: 0x0037333B
	public static void SetVisible(bool _visible)
	{
		if (_visible)
		{
			SleeperVolumeToolManager.SelectionChanged(null);
			return;
		}
		SleeperVolumeToolManager.ShowSleepers(false);
	}

	// Token: 0x060088AD RID: 34989 RVA: 0x0037514D File Offset: 0x0037334D
	public static void SelectionChanged(SelectionBox selBox)
	{
		if (selBox && selBox != SleeperVolumeToolManager.currentSelectionBox)
		{
			SleeperVolumeToolManager.previousSelectionBox = SleeperVolumeToolManager.currentSelectionBox;
		}
		SleeperVolumeToolManager.currentSelectionBox = selBox;
		SleeperVolumeToolManager.UpdateSleeperVisuals();
		SleeperVolumeToolManager.UpdateVolumeColors();
	}

	// Token: 0x060088AE RID: 34990 RVA: 0x00375180 File Offset: 0x00373380
	public static void UpdateVolumeColors()
	{
		int num = 0;
		if (SleeperVolumeToolManager.currentSelectionBox)
		{
			num = (int)((Prefab.PrefabSleeperVolume)SleeperVolumeToolManager.currentSelectionBox.UserData).groupId;
		}
		for (int i = 0; i < SleeperVolumeToolManager.registeredSleeperVolumes.Count; i++)
		{
			SelectionBox selectionBox = SleeperVolumeToolManager.registeredSleeperVolumes[i];
			Prefab.PrefabSleeperVolume prefabSleeperVolume = (Prefab.PrefabSleeperVolume)selectionBox.UserData;
			if (prefabSleeperVolume.groupId != 0)
			{
				if ((int)prefabSleeperVolume.groupId == num)
				{
					selectionBox.SetAllFacesColor(SleeperVolumeToolManager.groupSelectedColor, true);
				}
				else
				{
					selectionBox.SetAllFacesColor(SleeperVolumeToolManager.groupColors[(int)prefabSleeperVolume.groupId % SleeperVolumeToolManager.groupColors.Length], true);
				}
			}
			else
			{
				selectionBox.SetAllFacesColor(SelectionBoxManager.ColSleeperVolumeInactive, true);
			}
		}
		if (SleeperVolumeToolManager.currentSelectionBox)
		{
			SleeperVolumeToolManager.currentSelectionBox.SetAllFacesColor(SelectionBoxManager.ColSleeperVolume, true);
		}
	}

	// Token: 0x060088AF RID: 34991 RVA: 0x00375248 File Offset: 0x00373448
	public static void ShowSleepers(bool bShow = true)
	{
		if (SleeperVolumeToolManager.sleepers == null)
		{
			return;
		}
		foreach (KeyValuePair<Vector3i, List<SleeperVolumeToolManager.BlockData>> keyValuePair in SleeperVolumeToolManager.sleepers)
		{
			for (int i = 0; i < keyValuePair.Value.Count; i++)
			{
				Transform prefabT = keyValuePair.Value[i].prefabT;
				if (prefabT)
				{
					prefabT.gameObject.SetActive(bShow);
				}
			}
		}
	}

	// Token: 0x060088B0 RID: 34992 RVA: 0x003752DC File Offset: 0x003734DC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void UpdateSleeperVisuals()
	{
		if (SleeperVolumeToolManager.sleepers == null)
		{
			return;
		}
		foreach (KeyValuePair<Vector3i, List<SleeperVolumeToolManager.BlockData>> keyValuePair in SleeperVolumeToolManager.sleepers)
		{
			List<SleeperVolumeToolManager.BlockData> value = keyValuePair.Value;
			for (int i = 0; i < value.Count; i++)
			{
				SleeperVolumeToolManager.UpdateSleeperVisuals(value[i]);
			}
		}
	}

	// Token: 0x060088B1 RID: 34993 RVA: 0x00375354 File Offset: 0x00373554
	[PublicizedFrom(EAccessModifier.Private)]
	public static void UpdateSleeperVisuals(SleeperVolumeToolManager.BlockData data)
	{
		Transform prefabT = data.prefabT;
		if (!SelectionBoxManager.Instance.GetCategory("SleeperVolume").IsVisible() || (SleeperVolumeToolManager.currentSelectionBox == null && !SleeperVolumeToolManager.xRayOn))
		{
			prefabT.gameObject.SetActive(false);
			return;
		}
		Vector3i vector3i = Vector3i.min;
		Vector3i vector3i2 = Vector3i.min;
		SelectionBox selectionBox = SleeperVolumeToolManager.currentSelectionBox;
		if (selectionBox != null)
		{
			vector3i = Vector3i.FromVector3Rounded(selectionBox.bounds.min);
			vector3i2 = Vector3i.FromVector3Rounded(selectionBox.bounds.max);
		}
		Vector3i position = data.position;
		if (position.x >= vector3i.x && position.x < vector3i2.x && position.y >= vector3i.y && position.y < vector3i2.y && position.z >= vector3i.z && position.z < vector3i2.z)
		{
			int index = 0;
			PrefabInstance selectedPrefabInstance = XUiC_WoPropsSleeperVolume.selectedPrefabInstance;
			Vector3i pos = position - selectedPrefabInstance.boundingBoxPosition;
			Prefab.PrefabSleeperVolume prefabSleeperVolume = selectedPrefabInstance.prefab.FindSleeperVolume(pos);
			if (prefabSleeperVolume != null && prefabSleeperVolume.isPriority)
			{
				index = 1;
			}
			if (data.block.spawnMode == BlockSleeper.eMode.Bandit)
			{
				index = 4;
			}
			if (data.block.spawnMode == BlockSleeper.eMode.Infested)
			{
				index = 5;
			}
			prefabT.gameObject.SetActive(true);
			SleeperVolumeToolManager.SetMats(prefabT, SleeperVolumeToolManager.typeMats[index]);
			return;
		}
		if (!SleeperVolumeToolManager.InAnyVolume(position))
		{
			prefabT.gameObject.SetActive(true);
			SleeperVolumeToolManager.SetMats(prefabT, SleeperVolumeToolManager.typeMats[2]);
			return;
		}
		if (SleeperVolumeToolManager.xRayOn && SleeperVolumeToolManager.currentSelectionBox == null)
		{
			prefabT.gameObject.SetActive(true);
			SleeperVolumeToolManager.SetMats(prefabT, SleeperVolumeToolManager.typeMats[3]);
			return;
		}
		prefabT.gameObject.SetActive(false);
	}

	// Token: 0x060088B2 RID: 34994 RVA: 0x00375530 File Offset: 0x00373730
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool InAnyVolume(Vector3i pos)
	{
		Vector3i zero = Vector3i.zero;
		Vector3i zero2 = Vector3i.zero;
		for (int i = 0; i < SleeperVolumeToolManager.registeredSleeperVolumes.Count; i++)
		{
			SelectionBox selectionBox = SleeperVolumeToolManager.registeredSleeperVolumes[i];
			zero.RoundToInt(selectionBox.bounds.min);
			zero2.RoundToInt(selectionBox.bounds.max);
			if (pos.x >= zero.x && pos.x < zero2.x && pos.y >= zero.y && pos.y < zero2.y && pos.z >= zero.z && pos.z < zero2.z)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060088B3 RID: 34995 RVA: 0x003755EC File Offset: 0x003737EC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SetMats(Transform t, Material _mat)
	{
		int value = SleeperVolumeToolManager.xRayOn ? -200000000 : -200000;
		_mat.SetInt("_Offset", value);
		MeshRenderer[] componentsInChildren = t.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sharedMaterial = _mat;
		}
	}

	// Token: 0x060088B4 RID: 34996 RVA: 0x00375637 File Offset: 0x00373837
	public static bool GetXRay()
	{
		return SleeperVolumeToolManager.xRayOn;
	}

	// Token: 0x060088B5 RID: 34997 RVA: 0x00375640 File Offset: 0x00373840
	public static void SetXRay(bool _on)
	{
		if (SleeperVolumeToolManager.xRayOn != _on)
		{
			SleeperVolumeToolManager.xRayOn = _on;
			int value = SleeperVolumeToolManager.xRayOn ? -200000000 : -200000;
			for (int i = 0; i < SleeperVolumeToolManager.typeMats.Count; i++)
			{
				SleeperVolumeToolManager.typeMats[i].SetInt("_Offset", value);
			}
			SleeperVolumeToolManager.UpdateSleeperVisuals();
		}
	}

	// Token: 0x04006AA0 RID: 27296
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObject GroupGameObject = null;

	// Token: 0x04006AA1 RID: 27297
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<Vector3i, List<SleeperVolumeToolManager.BlockData>> sleepers;

	// Token: 0x04006AA2 RID: 27298
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<SelectionBox> registeredSleeperVolumes = new List<SelectionBox>();

	// Token: 0x04006AA3 RID: 27299
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool xRayOn = true;

	// Token: 0x04006AA4 RID: 27300
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cActiveIndex = 0;

	// Token: 0x04006AA5 RID: 27301
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cPriorityIndex = 1;

	// Token: 0x04006AA6 RID: 27302
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cNoVolumeIndex = 2;

	// Token: 0x04006AA7 RID: 27303
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cDarkIndex = 3;

	// Token: 0x04006AA8 RID: 27304
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cBanditIndex = 4;

	// Token: 0x04006AA9 RID: 27305
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cInfestedIndex = 5;

	// Token: 0x04006AAA RID: 27306
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Color[] typeColors = new Color[]
	{
		new Color(1f, 0.6f, 0.1f),
		new Color(0.7f, 0.7f, 0.7f),
		new Color(1f, 0.1f, 1f),
		new Color(0.02f, 0.02f, 0.02f),
		new Color(0.1f, 1f, 0.1f),
		new Color(1f, 0.1f, 0.1f)
	};

	// Token: 0x04006AAB RID: 27307
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<Material> typeMats = new List<Material>();

	// Token: 0x04006AAC RID: 27308
	[PublicizedFrom(EAccessModifier.Private)]
	public static Color groupSelectedColor = new Color(0.9f, 0.9f, 1f, 0.4f);

	// Token: 0x04006AAD RID: 27309
	[PublicizedFrom(EAccessModifier.Private)]
	public static Color[] groupColors = new Color[]
	{
		new Color(1f, 0.2f, 0.2f, 0.4f),
		new Color(1f, 0.6f, 0.2f, 0.4f),
		new Color(1f, 1f, 0.2f, 0.4f),
		new Color(0.6f, 1f, 0.2f, 0.4f),
		new Color(0.2f, 1f, 0.2f, 0.4f),
		new Color(0.2f, 1f, 0.6f, 0.4f),
		new Color(0.2f, 1f, 1f, 0.4f),
		new Color(0.2f, 0.6f, 1f, 0.4f)
	};

	// Token: 0x04006AAE RID: 27310
	[PublicizedFrom(EAccessModifier.Private)]
	public static SelectionBox currentSelectionBox;

	// Token: 0x04006AAF RID: 27311
	[PublicizedFrom(EAccessModifier.Private)]
	public static SelectionBox previousSelectionBox;

	// Token: 0x020010FF RID: 4351
	[PublicizedFrom(EAccessModifier.Private)]
	public class BlockData
	{
		// Token: 0x04006AB0 RID: 27312
		public BlockSleeper block;

		// Token: 0x04006AB1 RID: 27313
		public Transform prefabT;

		// Token: 0x04006AB2 RID: 27314
		public Vector3i position;
	}
}
