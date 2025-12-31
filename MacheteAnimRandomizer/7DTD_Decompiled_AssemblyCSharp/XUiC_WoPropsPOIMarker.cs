using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000EA8 RID: 3752
[Preserve]
public class XUiC_WoPropsPOIMarker : XUiController, ISelectionBoxCallback
{
	// Token: 0x06007682 RID: 30338 RVA: 0x00303C14 File Offset: 0x00301E14
	public override void Init()
	{
		base.Init();
		XUiC_WoPropsPOIMarker.ID = base.WindowGroup.ID;
		XUiC_WoPropsPOIMarker.Instance = this;
		this.markerList = (base.GetChildById("markers") as XUiC_PrefabMarkerList);
		if (this.markerList != null)
		{
			this.markerList.SelectionChanged += this.MarkerList_SelectionChanged;
		}
		this.StartX = (base.GetChildById("txtStartX") as XUiC_TextInput);
		if (this.StartX != null)
		{
			this.StartX.OnChangeHandler += this.StartX_OnChangeHandler;
		}
		this.StartY = (base.GetChildById("txtStartY") as XUiC_TextInput);
		if (this.StartY != null)
		{
			this.StartY.OnChangeHandler += this.StartY_OnChangeHandler;
		}
		this.StartZ = (base.GetChildById("txtStartZ") as XUiC_TextInput);
		if (this.StartZ != null)
		{
			this.StartZ.OnChangeHandler += this.StartZ_OnChangeHandler;
		}
		this.SizeX = (base.GetChildById("txtSizeX") as XUiC_TextInput);
		if (this.SizeX != null)
		{
			this.SizeX.OnChangeHandler += this.SizeX_OnChangeHandler;
		}
		this.SizeY = (base.GetChildById("txtSizeY") as XUiC_TextInput);
		if (this.SizeY != null)
		{
			this.SizeY.OnChangeHandler += this.SizeY_OnChangeHandler;
		}
		this.SizeZ = (base.GetChildById("txtSizeZ") as XUiC_TextInput);
		if (this.SizeZ != null)
		{
			this.SizeZ.OnChangeHandler += this.SizeZ_OnChangeHandler;
		}
		this.Rotations = (base.GetChildById("txtMarkerRotations") as XUiC_ComboBoxInt);
		if (this.Rotations != null)
		{
			this.Rotations.OnValueChanged += this.Rotations_OnValueChanged;
		}
		this.PartSpawnChance = (base.GetChildById("cbxPartSpawnChance") as XUiC_ComboBoxFloat);
		if (this.PartSpawnChance != null)
		{
			this.PartSpawnChance.OnValueChanged += this.PartSpawnChance_OnValueChanged;
		}
		this.MarkerSize = (base.GetChildById("cbxPOIMarkerSize") as XUiC_ComboBoxEnum<Prefab.Marker.MarkerSize>);
		if (this.MarkerSize != null)
		{
			this.MarkerSize.OnValueChanged += this.MarkerSize_OnValueChanged;
		}
		this.MarkerType = (base.GetChildById("cbxPOIMarkerType") as XUiC_ComboBoxEnum<Prefab.Marker.MarkerTypes>);
		if (this.MarkerType != null)
		{
			this.MarkerType.OnValueChanged += this.MarkerType_OnValueChanged;
		}
		this.GroupName = (base.GetChildById("txtGroup") as XUiC_TextInput);
		if (this.GroupName != null)
		{
			this.GroupName.OnChangeHandler += this.GroupName_OnChangeHandler;
		}
		this.Tags = (base.GetChildById("txtTags") as XUiC_TextInput);
		if (this.Tags != null)
		{
			this.Tags.OnChangeHandler += this.Tags_OnChangeHandler;
		}
		this.btnPOIMarker = base.GetChildById("btnPOIMarker");
		if (this.btnPOIMarker != null)
		{
			this.btnPOIMarker.GetChildById("clickable").OnPress += this.BtnPOIMarker_Controller_OnPress;
		}
		this.MarkerPartName = (base.GetChildById("cbxPOIMarkerPartName") as XUiC_ComboBoxList<string>);
		if (this.MarkerPartName != null)
		{
			foreach (PathAbstractions.AbstractedLocation abstractedLocation in PathAbstractions.PrefabsSearchPaths.GetAvailablePathsList(null, null, null, true))
			{
				if (abstractedLocation.RelativePath.EqualsCaseInsensitive("parts"))
				{
					this.MarkerPartName.Elements.Add(abstractedLocation.Name);
				}
			}
			this.MarkerPartName.OnValueChanged += this.MarkerPartName_OnValueChanged;
		}
		this.lblPartSpawn = base.GetChildById("lblPartName");
		this.lblCustSize = base.GetChildById("lblCustSize");
		this.grdCustSize = base.GetChildById("grdCustSize");
		this.lblPartRotations = base.GetChildById("lblMarkerRotations");
		this.lblMarkerSize = base.GetChildById("lblMarkerSize");
		this.lblPartSpawnChance = base.GetChildById("lblPartSpawnChance");
		if (SelectionBoxManager.Instance != null)
		{
			SelectionBoxManager.Instance.GetCategory("POIMarker").SetCallback(this);
		}
	}

	// Token: 0x06007683 RID: 30339 RVA: 0x00304054 File Offset: 0x00302254
	[PublicizedFrom(EAccessModifier.Private)]
	public void Tags_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (this.CurrentMarker == null)
		{
			return;
		}
		this.CurrentMarker.Tags = FastTags<TagGroup.Poi>.Parse(_text);
		PrefabEditModeManager.Instance.NeedsSaving = true;
	}

	// Token: 0x06007684 RID: 30340 RVA: 0x0030407B File Offset: 0x0030227B
	[PublicizedFrom(EAccessModifier.Private)]
	public void PartSpawnChance_OnValueChanged(XUiController _sender, double _oldValue, double _newValue)
	{
		if (this.CurrentMarker == null)
		{
			return;
		}
		this.CurrentMarker.PartChanceToSpawn = (float)Mathf.RoundToInt((float)_newValue * 100f) / 100f;
		this.updatePrefabDataAndVis();
		PrefabEditModeManager.Instance.NeedsSaving = true;
	}

	// Token: 0x06007685 RID: 30341 RVA: 0x003040B6 File Offset: 0x003022B6
	[PublicizedFrom(EAccessModifier.Private)]
	public void Rotations_OnValueChanged(XUiController _sender, long _oldValue, long _newValue)
	{
		if (this.CurrentMarker == null)
		{
			return;
		}
		this.CurrentMarker.Rotations = (byte)_newValue;
		this.updatePrefabDataAndVis();
		PrefabEditModeManager.Instance.NeedsSaving = true;
	}

	// Token: 0x06007686 RID: 30342 RVA: 0x003040DF File Offset: 0x003022DF
	[PublicizedFrom(EAccessModifier.Private)]
	public void MarkerPartName_OnValueChanged(XUiController _sender, string _oldValue, string _newValue)
	{
		if (this.CurrentMarker == null)
		{
			return;
		}
		this.CurrentMarker.PartToSpawn = _newValue;
		this.updatePrefabDataAndVis();
		PrefabEditModeManager.Instance.NeedsSaving = true;
	}

	// Token: 0x06007687 RID: 30343 RVA: 0x00304108 File Offset: 0x00302308
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		if (bindingName == "iscustomsize")
		{
			value = (this.MarkerSize != null && this.MarkerSize.Value == Prefab.Marker.MarkerSize.Custom).ToString();
			return true;
		}
		return false;
	}

	// Token: 0x06007688 RID: 30344 RVA: 0x00304148 File Offset: 0x00302348
	public override void OnClose()
	{
		this.saveDataToPrefab();
		base.OnClose();
	}

	// Token: 0x06007689 RID: 30345 RVA: 0x00304158 File Offset: 0x00302358
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnPOIMarker_Controller_OnPress(XUiController _sender, int _mouseButton)
	{
		Vector3 raycastHitPoint = XUiC_LevelTools3Window.getRaycastHitPoint(1000f, 0f);
		if (raycastHitPoint.Equals(Vector3.zero))
		{
			return;
		}
		Vector3i vector3i = World.worldToBlockPos(raycastHitPoint);
		DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.World.ChunkClusters[0].ChunkProvider.GetDynamicPrefabDecorator();
		if (dynamicPrefabDecorator == null)
		{
			return;
		}
		PrefabInstance prefabInstance = GameUtils.FindPrefabForBlockPos(dynamicPrefabDecorator.GetDynamicPrefabs(), vector3i);
		if (prefabInstance != null)
		{
			Vector3i vector3i2 = new Vector3i(1, 1, 1);
			prefabInstance.prefab.AddNewPOIMarker(prefabInstance.name, prefabInstance.boundingBoxPosition, vector3i - prefabInstance.boundingBoxPosition - new Vector3i(vector3i2.x / 2, 0, vector3i2.z / 2), vector3i2, "new", FastTags<TagGroup.Poi>.none, Prefab.Marker.MarkerTypes.None, false);
		}
		this.updatePrefabDataAndVis();
		POIMarkerToolManager.UpdateAllColors();
		this.markerList.RebuildList(false);
	}

	// Token: 0x0600768A RID: 30346 RVA: 0x0030422E File Offset: 0x0030242E
	[PublicizedFrom(EAccessModifier.Private)]
	public void GroupName_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (this.CurrentMarker == null)
		{
			return;
		}
		this.CurrentMarker.GroupName = _text;
		this.updatePrefabDataAndVis();
		POIMarkerToolManager.UpdateAllColors();
		if (!_changeFromCode)
		{
			this.markerList.RebuildList(false);
		}
		PrefabEditModeManager.Instance.NeedsSaving = true;
	}

	// Token: 0x0600768B RID: 30347 RVA: 0x0030426C File Offset: 0x0030246C
	[PublicizedFrom(EAccessModifier.Private)]
	public void MarkerType_OnValueChanged(XUiController _sender, Prefab.Marker.MarkerTypes _oldValue, Prefab.Marker.MarkerTypes _newValue)
	{
		if (this.CurrentMarker == null)
		{
			return;
		}
		this.CurrentMarker.MarkerType = _newValue;
		if (this.CurrentMarker.MarkerType == Prefab.Marker.MarkerTypes.POISpawn)
		{
			this.SizeY.Text = "0";
		}
		this.updatePrefabDataAndVis();
		PrefabEditModeManager.Instance.NeedsSaving = true;
	}

	// Token: 0x0600768C RID: 30348 RVA: 0x003042BD File Offset: 0x003024BD
	[PublicizedFrom(EAccessModifier.Private)]
	public void MarkerSize_OnValueChanged(XUiController _sender, Prefab.Marker.MarkerSize _oldValue, Prefab.Marker.MarkerSize _newValue)
	{
		if (this.CurrentMarker == null)
		{
			return;
		}
		if (_newValue == Prefab.Marker.MarkerSize.Custom)
		{
			return;
		}
		this.CurrentMarker.Size = Prefab.Marker.MarkerSizes[(int)_newValue];
		this.updatePrefabDataAndVis();
		PrefabEditModeManager.Instance.NeedsSaving = true;
	}

	// Token: 0x0600768D RID: 30349 RVA: 0x003042F4 File Offset: 0x003024F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void SizeZ_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (_text.Length == 0 || _changeFromCode || this.CurrentMarker == null)
		{
			return;
		}
		int z;
		if (!StringParsers.TryParseSInt32(_text, out z, 0, -1, NumberStyles.Integer))
		{
			return;
		}
		this.CurrentMarker.Size = new Vector3i(this.CurrentMarker.Size.x, this.CurrentMarker.Size.y, z);
		this.updatePrefabDataAndVis();
		PrefabEditModeManager.Instance.NeedsSaving = true;
	}

	// Token: 0x0600768E RID: 30350 RVA: 0x00304368 File Offset: 0x00302568
	[PublicizedFrom(EAccessModifier.Private)]
	public void SizeY_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (_text.Length == 0 || _changeFromCode || this.CurrentMarker == null)
		{
			return;
		}
		int y = 0;
		if (!StringParsers.TryParseSInt32(_text, out y, 0, -1, NumberStyles.Integer))
		{
			return;
		}
		if (this.CurrentMarker.MarkerType == Prefab.Marker.MarkerTypes.POISpawn)
		{
			y = 0;
			this.SizeY.Text = y.ToString();
		}
		this.CurrentMarker.Size = new Vector3i(this.CurrentMarker.Size.x, y, this.CurrentMarker.Size.z);
		this.updatePrefabDataAndVis();
		PrefabEditModeManager.Instance.NeedsSaving = true;
	}

	// Token: 0x0600768F RID: 30351 RVA: 0x00304400 File Offset: 0x00302600
	[PublicizedFrom(EAccessModifier.Private)]
	public void SizeX_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (_text.Length == 0 || _changeFromCode || this.CurrentMarker == null)
		{
			return;
		}
		int x;
		if (!StringParsers.TryParseSInt32(_text, out x, 0, -1, NumberStyles.Integer))
		{
			return;
		}
		this.CurrentMarker.Size = new Vector3i(x, this.CurrentMarker.Size.y, this.CurrentMarker.Size.z);
		this.updatePrefabDataAndVis();
		PrefabEditModeManager.Instance.NeedsSaving = true;
	}

	// Token: 0x06007690 RID: 30352 RVA: 0x00304474 File Offset: 0x00302674
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartZ_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (_text.Length == 0 || _changeFromCode || this.CurrentMarker == null)
		{
			return;
		}
		int z;
		if (!StringParsers.TryParseSInt32(_text, out z, 0, -1, NumberStyles.Integer))
		{
			return;
		}
		this.CurrentMarker.Start = new Vector3i(this.CurrentMarker.Start.x, this.CurrentMarker.Start.y, z);
		this.updatePrefabDataAndVis();
		PrefabEditModeManager.Instance.NeedsSaving = true;
	}

	// Token: 0x06007691 RID: 30353 RVA: 0x003044E8 File Offset: 0x003026E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartY_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (_text.Length == 0 || _changeFromCode || this.CurrentMarker == null)
		{
			return;
		}
		int y;
		if (!StringParsers.TryParseSInt32(_text, out y, 0, -1, NumberStyles.Integer))
		{
			return;
		}
		this.CurrentMarker.Start = new Vector3i(this.CurrentMarker.Start.x, y, this.CurrentMarker.Start.z);
		this.updatePrefabDataAndVis();
		PrefabEditModeManager.Instance.NeedsSaving = true;
	}

	// Token: 0x06007692 RID: 30354 RVA: 0x0030455C File Offset: 0x0030275C
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartX_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (_text.Length == 0 || _changeFromCode || this.CurrentMarker == null)
		{
			return;
		}
		int x;
		if (!StringParsers.TryParseSInt32(_text, out x, 0, -1, NumberStyles.Integer))
		{
			return;
		}
		this.CurrentMarker.Start = new Vector3i(x, this.CurrentMarker.Start.y, this.CurrentMarker.Start.z);
		this.updatePrefabDataAndVis();
		PrefabEditModeManager.Instance.NeedsSaving = true;
	}

	// Token: 0x06007693 RID: 30355 RVA: 0x003045D0 File Offset: 0x003027D0
	public void updatePrefabDataAndVis()
	{
		if (this.CurrentMarker == null)
		{
			return;
		}
		SelectionCategory category = SelectionBoxManager.Instance.GetCategory("POIMarker");
		SelectionBox selectionBox = (category != null) ? category.GetBox(this.CurrentMarker.Name) : null;
		if (selectionBox != null)
		{
			POIMarkerToolManager.UnRegisterPOIMarker(selectionBox);
			category.RemoveBox(this.CurrentMarker.Name);
			if (this.CurrentMarker.MarkerType == Prefab.Marker.MarkerTypes.PartSpawn && this.CurrentMarker.PartToSpawn != null && this.CurrentMarker.PartToSpawn.Length > 0)
			{
				Prefab prefab = new Prefab();
				prefab.Load(this.CurrentMarker.PartToSpawn, false, false, true, false);
				if ((prefab.rotationToFaceNorth + (int)this.CurrentMarker.Rotations) % 2 == 1)
				{
					this.CurrentMarker.Size = new Vector3i(prefab.size.z, prefab.size.y, prefab.size.x);
				}
				else
				{
					this.CurrentMarker.Size = prefab.size;
				}
			}
			selectionBox = category.AddBox(this.CurrentMarker.Name, this.CurrentMarker.Start - XUiC_WoPropsPOIMarker.getBaseVisualOffset(), this.CurrentMarker.Size, false, false);
			selectionBox.UserData = this.CurrentMarker;
			selectionBox.bAlwaysDrawDirection = true;
			selectionBox.bDrawDirection = true;
			float facing = 0f;
			switch (this.CurrentMarker.Rotations)
			{
			case 1:
				facing = (float)((this.CurrentMarker.MarkerType == Prefab.Marker.MarkerTypes.PartSpawn) ? 90 : 270);
				break;
			case 2:
				facing = 180f;
				break;
			case 3:
				facing = (float)((this.CurrentMarker.MarkerType == Prefab.Marker.MarkerTypes.PartSpawn) ? 270 : 90);
				break;
			}
			SelectionBoxManager.Instance.SetFacingDirection("POIMarker", this.CurrentMarker.Name, facing);
			SelectionBoxManager.Instance.SetActive("POIMarker", this.CurrentMarker.Name, true);
			POIMarkerToolManager.RegisterPOIMarker(selectionBox);
		}
		this.saveDataToPrefab();
	}

	// Token: 0x06007694 RID: 30356 RVA: 0x003047DC File Offset: 0x003029DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void saveDataToPrefab()
	{
		if (PrefabEditModeManager.Instance.VoxelPrefab != null && PrefabEditModeManager.Instance.VoxelPrefab.POIMarkers != null)
		{
			for (int i = 0; i < PrefabEditModeManager.Instance.VoxelPrefab.POIMarkers.Count; i++)
			{
				if (PrefabEditModeManager.Instance.VoxelPrefab.POIMarkers[i].Name == this.CurrentMarker.Name)
				{
					PrefabEditModeManager.Instance.VoxelPrefab.POIMarkers[i] = this.CurrentMarker;
					return;
				}
			}
		}
	}

	// Token: 0x06007695 RID: 30357 RVA: 0x00304870 File Offset: 0x00302A70
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateUIElements()
	{
		if (this.MarkerType == null)
		{
			return;
		}
		switch (this.MarkerType.Value)
		{
		case Prefab.Marker.MarkerTypes.None:
			this.lblMarkerSize.ViewComponent.IsVisible = (this.MarkerSize.ViewComponent.IsVisible = false);
			this.lblPartSpawn.ViewComponent.IsVisible = (this.MarkerPartName.ViewComponent.IsVisible = false);
			this.lblPartRotations.ViewComponent.IsVisible = (this.Rotations.ViewComponent.IsVisible = false);
			this.lblPartSpawnChance.ViewComponent.IsVisible = (this.PartSpawnChance.ViewComponent.IsVisible = false);
			this.lblCustSize.ViewComponent.IsVisible = (this.grdCustSize.ViewComponent.IsVisible = false);
			return;
		case Prefab.Marker.MarkerTypes.POISpawn:
			this.lblMarkerSize.ViewComponent.IsVisible = (this.MarkerSize.ViewComponent.IsVisible = true);
			this.lblPartSpawn.ViewComponent.IsVisible = (this.MarkerPartName.ViewComponent.IsVisible = false);
			this.lblPartRotations.ViewComponent.IsVisible = (this.Rotations.ViewComponent.IsVisible = true);
			this.lblPartSpawnChance.ViewComponent.IsVisible = (this.PartSpawnChance.ViewComponent.IsVisible = false);
			this.lblCustSize.ViewComponent.IsVisible = (this.grdCustSize.ViewComponent.IsVisible = (this.MarkerSize.Value == Prefab.Marker.MarkerSize.Custom));
			return;
		case Prefab.Marker.MarkerTypes.RoadExit:
			this.lblMarkerSize.ViewComponent.IsVisible = (this.MarkerSize.ViewComponent.IsVisible = true);
			this.lblPartSpawn.ViewComponent.IsVisible = (this.MarkerPartName.ViewComponent.IsVisible = false);
			this.lblPartRotations.ViewComponent.IsVisible = (this.Rotations.ViewComponent.IsVisible = true);
			this.lblPartSpawnChance.ViewComponent.IsVisible = (this.PartSpawnChance.ViewComponent.IsVisible = false);
			this.lblCustSize.ViewComponent.IsVisible = (this.grdCustSize.ViewComponent.IsVisible = (this.MarkerSize.Value == Prefab.Marker.MarkerSize.Custom));
			return;
		case Prefab.Marker.MarkerTypes.PartSpawn:
			this.lblMarkerSize.ViewComponent.IsVisible = (this.MarkerSize.ViewComponent.IsVisible = false);
			this.lblCustSize.ViewComponent.IsVisible = (this.grdCustSize.ViewComponent.IsVisible = false);
			this.lblPartRotations.ViewComponent.IsVisible = (this.Rotations.ViewComponent.IsVisible = true);
			this.lblPartSpawn.ViewComponent.IsVisible = (this.MarkerPartName.ViewComponent.IsVisible = true);
			this.lblPartRotations.ViewComponent.IsVisible = (this.Rotations.ViewComponent.IsVisible = true);
			this.lblPartSpawnChance.ViewComponent.IsVisible = (this.PartSpawnChance.ViewComponent.IsVisible = true);
			return;
		default:
			return;
		}
	}

	// Token: 0x06007696 RID: 30358 RVA: 0x00304BBA File Offset: 0x00302DBA
	public override void Update(float _dt)
	{
		this.updateUIElements();
		base.Update(_dt);
	}

	// Token: 0x06007697 RID: 30359 RVA: 0x00304BC9 File Offset: 0x00302DC9
	public override void OnOpen()
	{
		base.OnOpen();
		this.updateValues();
		this.updateUIElements();
	}

	// Token: 0x06007698 RID: 30360 RVA: 0x00304BE0 File Offset: 0x00302DE0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateValues()
	{
		if (this.CurrentMarker != null)
		{
			XUiC_TextInput startX = this.StartX;
			Vector3i vector3i = this.CurrentMarker.Start;
			startX.Text = vector3i.x.ToString();
			XUiC_TextInput startY = this.StartY;
			vector3i = this.CurrentMarker.Start;
			startY.Text = vector3i.y.ToString();
			XUiC_TextInput startZ = this.StartZ;
			vector3i = this.CurrentMarker.Start;
			startZ.Text = vector3i.z.ToString();
			if (this.MarkerSize != null)
			{
				if (Prefab.Marker.MarkerSizes.Contains(this.CurrentMarker.Size))
				{
					this.MarkerSize.Value = (Prefab.Marker.MarkerSize)Prefab.Marker.MarkerSizes.IndexOf(this.CurrentMarker.Size);
				}
				else
				{
					this.MarkerSize.Value = Prefab.Marker.MarkerSize.Custom;
				}
			}
			if (this.SizeX != null)
			{
				XUiC_TextInput sizeX = this.SizeX;
				vector3i = this.CurrentMarker.Size;
				sizeX.Text = vector3i.x.ToString();
			}
			if (this.SizeY != null)
			{
				XUiC_TextInput sizeY = this.SizeY;
				vector3i = this.CurrentMarker.Size;
				sizeY.Text = vector3i.y.ToString();
			}
			if (this.SizeZ != null)
			{
				XUiC_TextInput sizeZ = this.SizeZ;
				vector3i = this.CurrentMarker.Size;
				sizeZ.Text = vector3i.z.ToString();
			}
			if (this.MarkerType != null)
			{
				this.MarkerType.Value = this.CurrentMarker.MarkerType;
			}
			if (this.GroupName != null)
			{
				this.GroupName.Text = this.CurrentMarker.GroupName;
			}
			if (this.Tags != null)
			{
				this.Tags.Text = this.CurrentMarker.Tags.ToString();
			}
			if (this.MarkerPartName != null)
			{
				this.MarkerPartName.Value = this.CurrentMarker.PartToSpawn;
			}
			if (this.Rotations != null)
			{
				this.Rotations.Value = (long)((ulong)this.CurrentMarker.Rotations);
			}
			if (this.PartSpawnChance != null)
			{
				this.PartSpawnChance.Value = (double)((float)Mathf.RoundToInt(this.CurrentMarker.PartChanceToSpawn * 100f) / 100f);
			}
		}
	}

	// Token: 0x06007699 RID: 30361 RVA: 0x00304E08 File Offset: 0x00303008
	[PublicizedFrom(EAccessModifier.Private)]
	public void MarkerList_SelectionChanged(XUiC_ListEntry<XUiC_PrefabMarkerList.PrefabMarkerEntry> _previousEntry, XUiC_ListEntry<XUiC_PrefabMarkerList.PrefabMarkerEntry> _newEntry)
	{
		if (_newEntry == null || _newEntry.GetEntry() == null)
		{
			return;
		}
		this.CurrentMarker = _newEntry.GetEntry().marker;
		SelectionBoxManager.Instance.SetActive("POIMarker", this.CurrentMarker.Name, true);
		_newEntry.Selected = true;
		_newEntry.IsDirty = true;
		this.updateValues();
		this.IsDirty = true;
	}

	// Token: 0x0600769A RID: 30362 RVA: 0x00304E68 File Offset: 0x00303068
	public void CheckSpecialKeys(Event ev, PlayerActionsLocal playerActions)
	{
		if ((ev.modifiers & EventModifiers.Control) != EventModifiers.None && (ev.modifiers & EventModifiers.Shift) != EventModifiers.None && ev.keyCode == KeyCode.Return)
		{
			this.SpawnNewMarker();
			if (POIMarkerToolManager.currentSelectionBox != null && POIMarkerToolManager.currentSelectionBox.UserData is Prefab.Marker)
			{
				this.CurrentMarker = (POIMarkerToolManager.currentSelectionBox.UserData as Prefab.Marker);
			}
			this.updatePrefabDataAndVis();
			POIMarkerToolManager.UpdateAllColors();
			this.markerList.RebuildList(false);
			ev.Use();
		}
		if ((ev.modifiers & EventModifiers.Shift) != EventModifiers.None && ev.keyCode == KeyCode.Return)
		{
			if (POIMarkerToolManager.currentSelectionBox != null && POIMarkerToolManager.currentSelectionBox.UserData is Prefab.Marker)
			{
				this.CurrentMarker = (POIMarkerToolManager.currentSelectionBox.UserData as Prefab.Marker);
			}
			base.xui.playerUI.windowManager.Open(XUiC_WoPropsPOIMarker.ID, true, false, true);
			ev.Use();
		}
		if ((ev.modifiers & EventModifiers.Control) != EventModifiers.None && (ev.modifiers & EventModifiers.Shift) != EventModifiers.None && ev.keyCode == KeyCode.Z)
		{
			if (this.CurrentMarker != null)
			{
				this.CurrentMarker.Rotations = (this.CurrentMarker.Rotations + 1) % 4;
				this.updatePrefabDataAndVis();
			}
			ev.Use();
		}
		if ((ev.modifiers & EventModifiers.Control) != EventModifiers.None && (ev.modifiers & EventModifiers.Shift) != EventModifiers.None && ev.keyCode == KeyCode.A)
		{
			ev.Use();
			if (PrefabEditModeManager.Instance.VoxelPrefab != null && PrefabEditModeManager.Instance.VoxelPrefab.POIMarkers != null)
			{
				for (int i = 0; i < PrefabEditModeManager.Instance.VoxelPrefab.POIMarkers.Count; i++)
				{
					if (PrefabEditModeManager.Instance.VoxelPrefab.POIMarkers[i].MarkerType == Prefab.Marker.MarkerTypes.POISpawn)
					{
						SelectionBox selBox = null;
						if (SelectionBoxManager.Instance.TryGetSelectionBox("POIMarker", "POIMarker_" + i.ToString(), out selBox))
						{
							POIMarkerToolManager.DisplayPrefabPreviewForMarker(selBox);
						}
					}
				}
			}
		}
	}

	// Token: 0x0600769B RID: 30363 RVA: 0x00305054 File Offset: 0x00303254
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpawnNewMarker()
	{
		Vector3 raycastHitPoint = XUiC_LevelTools3Window.getRaycastHitPoint(1000f, 0f);
		if (raycastHitPoint.Equals(Vector3.zero))
		{
			return;
		}
		Vector3i vector3i = World.worldToBlockPos(raycastHitPoint);
		DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.World.ChunkClusters[0].ChunkProvider.GetDynamicPrefabDecorator();
		if (dynamicPrefabDecorator == null)
		{
			return;
		}
		PrefabInstance prefabInstance = GameUtils.FindPrefabForBlockPos(dynamicPrefabDecorator.GetDynamicPrefabs(), vector3i);
		if (prefabInstance != null)
		{
			Vector3i vector3i2 = new Vector3i(1, 1, 1);
			prefabInstance.prefab.AddNewPOIMarker(prefabInstance.name, prefabInstance.boundingBoxPosition, vector3i - prefabInstance.boundingBoxPosition - new Vector3i(vector3i2.x / 2, 0, vector3i2.z / 2), vector3i2, "new", FastTags<TagGroup.Poi>.none, Prefab.Marker.MarkerTypes.None, true);
		}
	}

	// Token: 0x0600769C RID: 30364 RVA: 0x00305113 File Offset: 0x00303313
	public override void Cleanup()
	{
		base.Cleanup();
		if (SelectionBoxManager.Instance != null)
		{
			SelectionCategory category = SelectionBoxManager.Instance.GetCategory("POIMarker");
			if (category != null)
			{
				category.SetCallback(null);
			}
		}
		POIMarkerToolManager.CleanUp();
		XUiC_WoPropsPOIMarker.Instance = null;
	}

	// Token: 0x0600769D RID: 30365 RVA: 0x0030514E File Offset: 0x0030334E
	[PublicizedFrom(EAccessModifier.Private)]
	public bool tryGetSelectedMarker(out Prefab.Marker _marker)
	{
		if (POIMarkerToolManager.currentSelectionBox == null || POIMarkerToolManager.currentSelectionBox.UserData == null)
		{
			_marker = null;
			return false;
		}
		_marker = (Prefab.Marker)POIMarkerToolManager.currentSelectionBox.UserData;
		return true;
	}

	// Token: 0x0600769E RID: 30366 RVA: 0x000197A5 File Offset: 0x000179A5
	public bool OnSelectionBoxActivated(string _category, string _name, bool _bActivated)
	{
		return true;
	}

	// Token: 0x0600769F RID: 30367 RVA: 0x00305180 File Offset: 0x00303380
	public bool OnSelectionBoxDelete(string _category, string _name)
	{
		if (PrefabEditModeManager.Instance.VoxelPrefab == null)
		{
			return false;
		}
		if (POIMarkerToolManager.currentSelectionBox)
		{
			POIMarkerToolManager.UnRegisterPOIMarker(POIMarkerToolManager.currentSelectionBox);
		}
		SelectionCategory category = SelectionBoxManager.Instance.GetCategory(_category);
		if (category != null)
		{
			category.RemoveBox(_name);
		}
		return PrefabEditModeManager.Instance.VoxelPrefab.POIMarkers.RemoveAll((Prefab.Marker x) => x.Name == _name) > 0;
	}

	// Token: 0x060076A0 RID: 30368 RVA: 0x002F7CE8 File Offset: 0x002F5EE8
	public bool OnSelectionBoxIsAvailable(string _category, EnumSelectionBoxAvailabilities _criteria)
	{
		return _criteria == EnumSelectionBoxAvailabilities.CanShowProperties || _criteria == EnumSelectionBoxAvailabilities.CanResize;
	}

	// Token: 0x060076A1 RID: 30369 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnSelectionBoxMirrored(Vector3i _axis)
	{
	}

	// Token: 0x060076A2 RID: 30370 RVA: 0x00305200 File Offset: 0x00303400
	public void OnSelectionBoxShowProperties(bool _bVisible, GUIWindowManager _windowManager)
	{
		string text;
		string text2;
		if (SelectionBoxManager.Instance.GetSelected(out text, out text2) && text.Equals("POIMarker"))
		{
			_windowManager.SwitchVisible(XUiC_WoPropsPOIMarker.ID, false, true);
		}
	}

	// Token: 0x060076A3 RID: 30371 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnSelectionBoxRotated(string _category, string _name)
	{
	}

	// Token: 0x060076A4 RID: 30372 RVA: 0x00305238 File Offset: 0x00303438
	public void OnSelectionBoxMoved(string _category, string _name, Vector3 _moveVector)
	{
		Prefab.Marker marker;
		if (this.tryGetSelectedMarker(out marker))
		{
			marker.Start += new Vector3i(_moveVector.x, _moveVector.y, _moveVector.z);
			SelectionCategory category = SelectionBoxManager.Instance.GetCategory("POIMarker");
			if (category == null)
			{
				return;
			}
			SelectionBox box = category.GetBox(_name);
			if (box == null)
			{
				return;
			}
			box.SetPositionAndSize(marker.Start - XUiC_WoPropsPOIMarker.getBaseVisualOffset(), marker.Size);
		}
	}

	// Token: 0x060076A5 RID: 30373 RVA: 0x003052B8 File Offset: 0x003034B8
	public void OnSelectionBoxSized(string _category, string _name, int _dTop, int _dBottom, int _dNorth, int _dSouth, int _dEast, int _dWest)
	{
		Prefab.Marker marker;
		if (!this.tryGetSelectedMarker(out marker))
		{
			marker.Start += new Vector3i(-_dWest, -_dBottom, -_dSouth);
			marker.Size += new Vector3i(_dEast + _dWest, _dTop + _dBottom, _dNorth + _dSouth);
			SelectionCategory category = SelectionBoxManager.Instance.GetCategory("POIMarker");
			if (category == null)
			{
				return;
			}
			SelectionBox box = category.GetBox(_name);
			if (box == null)
			{
				return;
			}
			box.SetPositionAndSize(marker.Start - XUiC_WoPropsPOIMarker.getBaseVisualOffset(), marker.Size);
		}
	}

	// Token: 0x060076A6 RID: 30374 RVA: 0x00305354 File Offset: 0x00303554
	[PublicizedFrom(EAccessModifier.Private)]
	public static Vector3i getBaseVisualOffset()
	{
		Vector3i result = Vector3i.zero;
		if (PrefabEditModeManager.Instance.VoxelPrefab != null)
		{
			result = PrefabEditModeManager.Instance.VoxelPrefab.size * 0.5f;
			result.y = -1;
		}
		return result;
	}

	// Token: 0x04005A5D RID: 23133
	public static string ID = "";

	// Token: 0x04005A5E RID: 23134
	public static XUiC_WoPropsPOIMarker Instance;

	// Token: 0x04005A5F RID: 23135
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput StartX;

	// Token: 0x04005A60 RID: 23136
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput StartY;

	// Token: 0x04005A61 RID: 23137
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput StartZ;

	// Token: 0x04005A62 RID: 23138
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput SizeX;

	// Token: 0x04005A63 RID: 23139
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput SizeY;

	// Token: 0x04005A64 RID: 23140
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput SizeZ;

	// Token: 0x04005A65 RID: 23141
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput GroupName;

	// Token: 0x04005A66 RID: 23142
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput Tags;

	// Token: 0x04005A67 RID: 23143
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController grdCustSize;

	// Token: 0x04005A68 RID: 23144
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController lblCustSize;

	// Token: 0x04005A69 RID: 23145
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController lblPartSpawn;

	// Token: 0x04005A6A RID: 23146
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController lblPartRotations;

	// Token: 0x04005A6B RID: 23147
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnPOIMarker;

	// Token: 0x04005A6C RID: 23148
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController lblMarkerSize;

	// Token: 0x04005A6D RID: 23149
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController lblPartSpawnChance;

	// Token: 0x04005A6E RID: 23150
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxEnum<Prefab.Marker.MarkerSize> MarkerSize;

	// Token: 0x04005A6F RID: 23151
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxEnum<Prefab.Marker.MarkerTypes> MarkerType;

	// Token: 0x04005A70 RID: 23152
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> MarkerPartName;

	// Token: 0x04005A71 RID: 23153
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PrefabMarkerList markerList;

	// Token: 0x04005A72 RID: 23154
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxInt Rotations;

	// Token: 0x04005A73 RID: 23155
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat PartSpawnChance;

	// Token: 0x04005A74 RID: 23156
	public const float cPrefabYPosition = 4f;

	// Token: 0x04005A75 RID: 23157
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lastIsCustomSize;

	// Token: 0x04005A76 RID: 23158
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lastIsPartSpawner;

	// Token: 0x04005A77 RID: 23159
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lastShowRotations;

	// Token: 0x04005A78 RID: 23160
	public Prefab.Marker CurrentMarker;
}
