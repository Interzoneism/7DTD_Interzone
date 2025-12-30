using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Audio;
using UnityEngine;

// Token: 0x02000964 RID: 2404
public class SelectionBoxManager : MonoBehaviour
{
	// Token: 0x170007A4 RID: 1956
	// (get) Token: 0x06004892 RID: 18578 RVA: 0x001CAD94 File Offset: 0x001C8F94
	// (set) Token: 0x06004893 RID: 18579 RVA: 0x001CADEA File Offset: 0x001C8FEA
	[TupleElementNames(new string[]
	{
		"category",
		"box"
	})]
	public ValueTuple<SelectionCategory, SelectionBox>? Selection
	{
		[return: TupleElementNames(new string[]
		{
			"category",
			"box"
		})]
		get
		{
			if (this.selection == null)
			{
				return null;
			}
			if (this.selection.Value.Item2 == null)
			{
				return null;
			}
			return new ValueTuple<SelectionCategory, SelectionBox>?(this.selection.Value);
		}
		[PublicizedFrom(EAccessModifier.Private)]
		[param: TupleElementNames(new string[]
		{
			"category",
			"box"
		})]
		set
		{
			this.selection = value;
		}
	}

	// Token: 0x170007A5 RID: 1957
	// (get) Token: 0x06004894 RID: 18580 RVA: 0x001CADF3 File Offset: 0x001C8FF3
	// (set) Token: 0x06004895 RID: 18581 RVA: 0x001CADFB File Offset: 0x001C8FFB
	public float AlphaMultiplier
	{
		get
		{
			return this.alphaMultiplier;
		}
		set
		{
			this.alphaMultiplier = Mathf.Clamp01(value);
			GamePrefs.Set(EnumGamePrefs.OptionsSelectionBoxAlphaMultiplier, this.alphaMultiplier);
			this.UpdateAllColors();
		}
	}

	// Token: 0x06004896 RID: 18582 RVA: 0x001CAE1F File Offset: 0x001C901F
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		SelectionBoxManager.Instance = this;
		Origin.Add(base.transform, 1);
		this.alphaMultiplier = GamePrefs.GetFloat(EnumGamePrefs.OptionsSelectionBoxAlphaMultiplier);
	}

	// Token: 0x06004897 RID: 18583 RVA: 0x001CAE43 File Offset: 0x001C9043
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		Origin.Remove(base.transform);
	}

	// Token: 0x06004898 RID: 18584 RVA: 0x001CAE50 File Offset: 0x001C9050
	public Dictionary<string, SelectionCategory> GetCategories()
	{
		return this.categories;
	}

	// Token: 0x06004899 RID: 18585 RVA: 0x001CAE58 File Offset: 0x001C9058
	public SelectionCategory GetCategory(string _name)
	{
		SelectionCategory result;
		this.categories.TryGetValue(_name, out result);
		return result;
	}

	// Token: 0x0600489A RID: 18586 RVA: 0x001CAE75 File Offset: 0x001C9075
	public bool TryGetSelectionBox(string _category, string _name, out SelectionBox _selectionBox)
	{
		SelectionCategory category = this.GetCategory(_category);
		_selectionBox = ((category != null) ? category.GetBox(_name) : null);
		return _selectionBox != null;
	}

	// Token: 0x0600489B RID: 18587 RVA: 0x001CAE98 File Offset: 0x001C9098
	public void CreateCategory(string _name, Color _colSelected, Color _colUnselected, Color _colFaceSelected, bool _bCollider, string _tag, int _layer = 0)
	{
		Transform transform = new GameObject(_name).transform;
		transform.parent = base.transform;
		SelectionCategory selectionCategory = new SelectionCategory(_name, transform, _colSelected, _colUnselected, _colFaceSelected, _bCollider, _tag, null, _layer);
		selectionCategory.SetVisible(false);
		this.categories[_name] = selectionCategory;
	}

	// Token: 0x0600489C RID: 18588 RVA: 0x001CAEE4 File Offset: 0x001C90E4
	public void SetUserData(string _category, string _name, object _data)
	{
		SelectionBox selectionBox;
		if (this.TryGetSelectionBox(_category, _name, out selectionBox))
		{
			selectionBox.UserData = _data;
		}
		this.UpdateSleepersAndMarkers();
	}

	// Token: 0x0600489D RID: 18589 RVA: 0x001CAF0C File Offset: 0x001C910C
	public bool IsActive(string _category, string _name)
	{
		SelectionBox y;
		ValueTuple<SelectionCategory, SelectionBox>? valueTuple;
		return this.TryGetSelectionBox(_category, _name, out y) && ((this.Selection != null) ? valueTuple.GetValueOrDefault().Item2 : null) == y;
	}

	// Token: 0x0600489E RID: 18590 RVA: 0x001CAF4C File Offset: 0x001C914C
	public void SetActive(string _category, string _name, bool _bActive)
	{
		SelectionCategory category = this.GetCategory(_category);
		if (!category.IsVisible())
		{
			category.SetVisible(true);
		}
		SelectionBox selectionBox;
		if (this.TryGetSelectionBox(_category, _name, out selectionBox))
		{
			this.activate(this.categories[_category], _bActive ? selectionBox : null);
		}
	}

	// Token: 0x0600489F RID: 18591 RVA: 0x001CAF98 File Offset: 0x001C9198
	public void SetFacingDirection(string _category, string _name, float _facing)
	{
		SelectionCategory category = this.GetCategory(_category);
		if (!category.IsVisible())
		{
			category.SetVisible(true);
		}
		SelectionBox selectionBox;
		if (this.TryGetSelectionBox(_category, _name, out selectionBox))
		{
			selectionBox.facingDirection = _facing;
		}
	}

	// Token: 0x060048A0 RID: 18592 RVA: 0x001CAFCF File Offset: 0x001C91CF
	public void Deactivate()
	{
		this.activate(null, null);
	}

	// Token: 0x060048A1 RID: 18593 RVA: 0x001CAFDC File Offset: 0x001C91DC
	public bool GetSelected(out string _selectedCategory, out string _selectedName)
	{
		if (this.Selection != null)
		{
			_selectedCategory = this.Selection.Value.Item1.name;
			_selectedName = this.Selection.Value.Item2.name;
			return true;
		}
		_selectedCategory = null;
		_selectedName = null;
		return false;
	}

	// Token: 0x060048A2 RID: 18594 RVA: 0x001CB038 File Offset: 0x001C9238
	public void Unselect()
	{
		this.Selection = null;
		this.UpdateSleepersAndMarkers();
	}

	// Token: 0x060048A3 RID: 18595 RVA: 0x001CB05C File Offset: 0x001C925C
	public bool Select(WorldRayHitInfo _hitInfo)
	{
		if (_hitInfo.tag == null)
		{
			return false;
		}
		foreach (KeyValuePair<string, SelectionCategory> keyValuePair in this.categories)
		{
			if (_hitInfo.tag.Equals(keyValuePair.Value.tag))
			{
				foreach (KeyValuePair<string, SelectionBox> keyValuePair2 in keyValuePair.Value.boxes)
				{
					if (keyValuePair2.Value.GetBoxTransform() == _hitInfo.transform)
					{
						if (keyValuePair.Value.name != "SleeperVolume")
						{
							SleeperVolumeToolManager.ShowSleepers(false);
						}
						Manager.PlayButtonClick();
						return this.activate(keyValuePair.Value, keyValuePair2.Value);
					}
				}
			}
		}
		return false;
	}

	// Token: 0x060048A4 RID: 18596 RVA: 0x001CB170 File Offset: 0x001C9370
	[PublicizedFrom(EAccessModifier.Private)]
	public bool activate(SelectionCategory _cat, SelectionBox _box)
	{
		ValueTuple<SelectionCategory, SelectionBox>? valueTuple = this.Selection;
		bool result = true;
		ValueTuple<SelectionCategory, SelectionBox>? valueTuple2 = this.Selection;
		if (((valueTuple2 != null) ? valueTuple2.GetValueOrDefault().Item2 : null) == _box || _box == null)
		{
			valueTuple2 = null;
			this.Selection = valueTuple2;
		}
		else
		{
			this.Selection = new ValueTuple<SelectionCategory, SelectionBox>?(new ValueTuple<SelectionCategory, SelectionBox>(_cat, _box));
			_box.SetFrameActive(true);
			_box.SetAllFacesColor(_cat.colActive, true);
		}
		if (valueTuple != null)
		{
			valueTuple.Value.Item2.SetFrameActive(false);
			valueTuple.Value.Item2.SetAllFacesColor(valueTuple.Value.Item1.colInactive, true);
			ISelectionBoxCallback callback = valueTuple.Value.Item1.callback;
			if (callback != null)
			{
				callback.OnSelectionBoxActivated(valueTuple.Value.Item1.name, valueTuple.Value.Item2.name, false);
			}
		}
		valueTuple2 = this.Selection;
		if (valueTuple2 != null)
		{
			valueTuple2 = this.Selection;
			if (valueTuple2.Value.Item1.callback != null)
			{
				valueTuple2 = this.Selection;
				ISelectionBoxCallback callback2 = valueTuple2.Value.Item1.callback;
				valueTuple2 = this.Selection;
				string name = valueTuple2.Value.Item1.name;
				valueTuple2 = this.Selection;
				result = callback2.OnSelectionBoxActivated(name, valueTuple2.Value.Item2.name, true);
			}
		}
		this.UpdateSleepersAndMarkers();
		return result;
	}

	// Token: 0x060048A5 RID: 18597 RVA: 0x001CB2EC File Offset: 0x001C94EC
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateSleepersAndMarkers()
	{
		if (this.Selection == null)
		{
			SleeperVolumeToolManager.SelectionChanged(null);
			POIMarkerToolManager.SelectionChanged(null);
			return;
		}
		ValueTuple<SelectionCategory, SelectionBox> value = this.Selection.Value;
		SelectionCategory item = value.Item1;
		SelectionBox item2 = value.Item2;
		if (item.name.Equals("SleeperVolume"))
		{
			SleeperVolumeToolManager.SelectionChanged(item2);
			return;
		}
		if (item.name.Equals("POIMarker"))
		{
			POIMarkerToolManager.SelectionChanged(item2);
		}
	}

	// Token: 0x060048A6 RID: 18598 RVA: 0x001CB364 File Offset: 0x001C9564
	public void UpdateAllColors()
	{
		foreach (KeyValuePair<string, SelectionCategory> keyValuePair in this.categories)
		{
			foreach (KeyValuePair<string, SelectionBox> keyValuePair2 in keyValuePair.Value.boxes)
			{
				ValueTuple<SelectionCategory, SelectionBox>? valueTuple;
				Color c = (((this.Selection != null) ? valueTuple.GetValueOrDefault().Item2 : null) == keyValuePair2.Value) ? keyValuePair.Value.colActive : keyValuePair.Value.colInactive;
				keyValuePair2.Value.SetAllFacesColor(c, true);
			}
		}
	}

	// Token: 0x060048A7 RID: 18599 RVA: 0x001CB450 File Offset: 0x001C9650
	public void Clear()
	{
		foreach (KeyValuePair<string, SelectionCategory> keyValuePair in this.categories)
		{
			keyValuePair.Value.Clear();
		}
	}

	// Token: 0x060048A8 RID: 18600 RVA: 0x001CB4A8 File Offset: 0x001C96A8
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i createBlockMoveVector(Vector3 _relPlayerAxis)
	{
		Vector3i result;
		if (Math.Abs(_relPlayerAxis.x) > Math.Abs(_relPlayerAxis.z))
		{
			result = new Vector3i(Mathf.Sign(_relPlayerAxis.x), 0f, 0f);
		}
		else
		{
			result = new Vector3i(0f, 0f, Mathf.Sign(_relPlayerAxis.z));
		}
		return result;
	}

	// Token: 0x060048A9 RID: 18601 RVA: 0x001CB508 File Offset: 0x001C9708
	[PublicizedFrom(EAccessModifier.Private)]
	public void moveSelection(Vector3i _deltaVec)
	{
		ValueTuple<SelectionCategory, SelectionBox>? valueTuple = this.Selection;
		if (((valueTuple != null) ? valueTuple.GetValueOrDefault().Item1.callback : null) == null)
		{
			return;
		}
		valueTuple = this.Selection;
		ISelectionBoxCallback callback = valueTuple.Value.Item1.callback;
		valueTuple = this.Selection;
		string name = valueTuple.Value.Item1.name;
		valueTuple = this.Selection;
		callback.OnSelectionBoxMoved(name, valueTuple.Value.Item2.name, _deltaVec.ToVector3());
		this.UpdateSleepersAndMarkers();
	}

	// Token: 0x060048AA RID: 18602 RVA: 0x001CB598 File Offset: 0x001C9798
	[PublicizedFrom(EAccessModifier.Private)]
	public void incSelection(int _dTop, int _dBottom, int _dNorth, int _dSouth, int _dEast, int _dWest)
	{
		ValueTuple<SelectionCategory, SelectionBox>? valueTuple = this.Selection;
		if (((valueTuple != null) ? valueTuple.GetValueOrDefault().Item1.callback : null) == null)
		{
			return;
		}
		valueTuple = this.Selection;
		ISelectionBoxCallback callback = valueTuple.Value.Item1.callback;
		valueTuple = this.Selection;
		if (!callback.OnSelectionBoxIsAvailable(valueTuple.Value.Item1.name, EnumSelectionBoxAvailabilities.CanResize))
		{
			return;
		}
		valueTuple = this.Selection;
		ISelectionBoxCallback callback2 = valueTuple.Value.Item1.callback;
		valueTuple = this.Selection;
		string name = valueTuple.Value.Item1.name;
		valueTuple = this.Selection;
		callback2.OnSelectionBoxSized(name, valueTuple.Value.Item2.name, _dTop, _dBottom, _dNorth, _dSouth, _dEast, _dWest);
		this.UpdateSleepersAndMarkers();
	}

	// Token: 0x060048AB RID: 18603 RVA: 0x001CB664 File Offset: 0x001C9864
	[PublicizedFrom(EAccessModifier.Private)]
	public void mirrorSelection(Vector3i _axis)
	{
		ValueTuple<SelectionCategory, SelectionBox>? valueTuple = this.Selection;
		if (((valueTuple != null) ? valueTuple.GetValueOrDefault().Item1.callback : null) == null)
		{
			return;
		}
		valueTuple = this.Selection;
		ISelectionBoxCallback callback = valueTuple.Value.Item1.callback;
		valueTuple = this.Selection;
		if (!callback.OnSelectionBoxIsAvailable(valueTuple.Value.Item1.name, EnumSelectionBoxAvailabilities.CanMirror))
		{
			return;
		}
		valueTuple = this.Selection;
		valueTuple.Value.Item1.callback.OnSelectionBoxMirrored(_axis);
		this.UpdateSleepersAndMarkers();
	}

	// Token: 0x060048AC RID: 18604 RVA: 0x001CB6F8 File Offset: 0x001C98F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void ShowThroughWalls(string _categoryName, bool _isThroughWalls, bool _isAll)
	{
		if (_isAll)
		{
			using (Dictionary<string, SelectionBox>.Enumerator enumerator = this.categories[_categoryName].boxes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, SelectionBox> keyValuePair = enumerator.Current;
					keyValuePair.Value.ShowThroughWalls(_isThroughWalls);
				}
				return;
			}
		}
		if (this.Selection == null)
		{
			return;
		}
		ValueTuple<SelectionCategory, SelectionBox>? valueTuple;
		valueTuple.GetValueOrDefault().Item2.ShowThroughWalls(_isThroughWalls);
	}

	// Token: 0x060048AD RID: 18605 RVA: 0x001CB784 File Offset: 0x001C9984
	public bool ConsumeScrollWheel(float _scrollWheel, PlayerActionsLocal _playerActions)
	{
		ValueTuple<SelectionCategory, SelectionBox>? valueTuple;
		if (((this.Selection != null) ? valueTuple.GetValueOrDefault().Item1.callback : null) == null)
		{
			return false;
		}
		if (Mathf.Abs(_scrollWheel) < 0.1f)
		{
			return false;
		}
		int num = Mathf.RoundToInt(_scrollWheel * 10f);
		bool result = false;
		bool controlKeyPressed = InputUtils.ControlKeyPressed;
		if (GamePrefs.GetInt(EnumGamePrefs.SelectionOperationMode) == 2)
		{
			if (_playerActions.Jump.IsPressed)
			{
				this.incSelection(num, 0, 0, 0, 0, 0);
				result = true;
			}
			if (_playerActions.Crouch.IsPressed && !controlKeyPressed)
			{
				this.incSelection(0, num, 0, 0, 0, 0);
				result = true;
			}
			if (_playerActions.MoveLeft.IsPressed)
			{
				this.incSelection(0, 0, 0, 0, num, 0);
				result = true;
			}
			if (_playerActions.MoveRight.IsPressed)
			{
				this.incSelection(0, 0, 0, 0, 0, num);
				result = true;
			}
			if (_playerActions.MoveForward.IsPressed)
			{
				this.incSelection(0, 0, num, 0, 0, 0);
				result = true;
			}
			if (_playerActions.MoveBack.IsPressed)
			{
				this.incSelection(0, 0, 0, num, 0, 0);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x060048AE RID: 18606 RVA: 0x001CB890 File Offset: 0x001C9A90
	public void CheckKeys(GameManager _gameManager, PlayerActionsLocal _playerActions, WorldRayHitInfo _hitInfo)
	{
		bool altKeyPressed = InputUtils.AltKeyPressed;
		GameManager.bVolumeBlocksEditing = !altKeyPressed;
		foreach (KeyValuePair<string, SelectionCategory> keyValuePair in this.categories)
		{
			string text;
			SelectionCategory selectionCategory;
			keyValuePair.Deconstruct(out text, out selectionCategory);
			string categoryName = text;
			if (selectionCategory.IsVisible())
			{
				this.ShowThroughWalls(categoryName, altKeyPressed, true);
			}
		}
		ValueTuple<SelectionCategory, SelectionBox>? valueTuple;
		if (_playerActions.SelectionRotate.WasPressed && !Input.GetKey(KeyCode.Tab))
		{
			valueTuple = this.Selection;
			if (((valueTuple != null) ? valueTuple.GetValueOrDefault().Item1.callback : null) == null)
			{
				BlockToolSelection.Instance.RotateFocusedBlock(_hitInfo, _playerActions);
			}
			else
			{
				valueTuple = this.Selection;
				ValueTuple<SelectionCategory, SelectionBox> value = valueTuple.Value;
				SelectionCategory item = value.Item1;
				SelectionBox item2 = value.Item2;
				item.callback.OnSelectionBoxRotated(item.name, item2.name);
			}
		}
		valueTuple = this.Selection;
		if (((valueTuple != null) ? valueTuple.GetValueOrDefault().Item1.callback : null) == null)
		{
			return;
		}
		valueTuple = this.Selection;
		ValueTuple<SelectionCategory, SelectionBox> value2 = valueTuple.Value;
		SelectionCategory item3 = value2.Item1;
		SelectionBox item4 = value2.Item2;
		bool controlKeyPressed = InputUtils.ControlKeyPressed;
		if (GamePrefs.GetInt(EnumGamePrefs.SelectionOperationMode) == 1 && GamePrefs.GetInt(EnumGamePrefs.SelectionContextMode) == 1)
		{
			if (_playerActions.MoveBack.WasPressed)
			{
				this.moveSelection(-1 * this.createBlockMoveVector(_gameManager.World.GetPrimaryPlayer().transform.forward));
			}
			if (_playerActions.MoveForward.WasPressed)
			{
				this.moveSelection(this.createBlockMoveVector(_gameManager.World.GetPrimaryPlayer().transform.forward));
			}
			if (_playerActions.MoveLeft.WasPressed)
			{
				this.moveSelection(-1 * this.createBlockMoveVector(_gameManager.World.GetPrimaryPlayer().transform.right));
			}
			if (_playerActions.MoveRight.WasPressed)
			{
				this.moveSelection(this.createBlockMoveVector(_gameManager.World.GetPrimaryPlayer().transform.right));
			}
		}
		else if (GamePrefs.GetInt(EnumGamePrefs.SelectionOperationMode) == 1 && GamePrefs.GetInt(EnumGamePrefs.SelectionContextMode) == 0)
		{
			if (_playerActions.MoveBack.WasPressed)
			{
				this.moveSelection(-1 * Vector3i.forward);
			}
			if (_playerActions.MoveForward.WasPressed)
			{
				this.moveSelection(Vector3i.forward);
			}
			if (_playerActions.MoveLeft.WasPressed)
			{
				this.moveSelection(-1 * Vector3i.right);
			}
			if (_playerActions.MoveRight.WasPressed)
			{
				this.moveSelection(Vector3i.right);
			}
		}
		if (GamePrefs.GetInt(EnumGamePrefs.SelectionOperationMode) == 1)
		{
			if (_playerActions.Jump.WasPressed)
			{
				this.moveSelection(new Vector3i(0, 1, 0));
			}
			if (_playerActions.Crouch.WasPressed && !controlKeyPressed)
			{
				this.moveSelection(new Vector3i(0, -1, 0));
			}
		}
		if (_playerActions.SelectionMoveMode.WasPressed)
		{
			GamePrefs.Set(EnumGamePrefs.SelectionContextMode, (GamePrefs.GetInt(EnumGamePrefs.SelectionContextMode) + 1) % 2);
		}
		if (GamePrefs.GetInt(EnumGamePrefs.SelectionOperationMode) == 2)
		{
			Color colFaceSelected = item3.colFaceSelected;
			if (_playerActions.Jump.WasPressed)
			{
				item4.SetFaceColor(BlockFace.Top, colFaceSelected);
			}
			else if (_playerActions.Jump.WasReleased)
			{
				item4.ResetAllFacesColor();
			}
			if (_playerActions.Crouch.WasPressed && !controlKeyPressed)
			{
				item4.SetFaceColor(BlockFace.Bottom, colFaceSelected);
			}
			else if (_playerActions.Crouch.WasReleased)
			{
				item4.ResetAllFacesColor();
			}
			if (_playerActions.MoveLeft.WasPressed)
			{
				item4.SetFaceColor(BlockFace.East, colFaceSelected);
			}
			else if (_playerActions.MoveLeft.WasReleased)
			{
				item4.ResetAllFacesColor();
			}
			if (_playerActions.MoveRight.WasPressed)
			{
				item4.SetFaceColor(BlockFace.West, colFaceSelected);
			}
			else if (_playerActions.MoveRight.WasReleased)
			{
				item4.ResetAllFacesColor();
			}
			if (_playerActions.MoveForward.WasPressed)
			{
				item4.SetFaceColor(BlockFace.North, colFaceSelected);
			}
			else if (_playerActions.MoveForward.WasReleased)
			{
				item4.ResetAllFacesColor();
			}
			if (_playerActions.MoveBack.WasPressed)
			{
				item4.SetFaceColor(BlockFace.South, colFaceSelected);
			}
			else if (_playerActions.MoveBack.WasReleased)
			{
				item4.ResetAllFacesColor();
			}
		}
		if (_playerActions.SelectionDelete.WasPressed)
		{
			this.SetActive(item3.name, item4.name, false);
			if (item3.callback.OnSelectionBoxDelete(item3.name, item4.name))
			{
				Manager.PlayButtonClick();
				item3.RemoveBox(item4.name);
				valueTuple = null;
				this.Selection = valueTuple;
			}
		}
		valueTuple = this.Selection;
		if (valueTuple != null && valueTuple.GetValueOrDefault().Item1.name.Equals("SleeperVolume"))
		{
			SleeperVolumeToolManager.CheckKeys();
		}
	}

	// Token: 0x060048AF RID: 18607 RVA: 0x001CBD4C File Offset: 0x001C9F4C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Update()
	{
		EntityPlayerLocal primaryPlayer;
		if (this.Selection == null || GameManager.Instance == null || GameManager.Instance.World == null || (primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer()) == null)
		{
			return;
		}
		Camera finalCamera = primaryPlayer.finalCamera;
		if (finalCamera == null)
		{
			return;
		}
		SelectionBox item = this.Selection.Value.Item2;
		if (item.Axises.Count == 0)
		{
			return;
		}
		if (this.lastSelOpMode != GamePrefs.GetInt(EnumGamePrefs.SelectionOperationMode))
		{
			this.lastSelOpMode = GamePrefs.GetInt(EnumGamePrefs.SelectionOperationMode);
			this.mouseMoveDir = Vector3.zero;
		}
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = 0f;
		Vector3 vector = finalCamera.WorldToScreenPoint(item.AxisOrigin);
		vector.z = 0f;
		bool flag = true;
		if (!this.bMousedPressed)
		{
			for (int i = 0; i < item.Axises.Count; i++)
			{
				Vector3 vector2 = finalCamera.WorldToScreenPoint(item.Axises[i]);
				vector2.z = 0f;
				if (this.GetLineDistanceSq(vector, vector2, mousePosition) < 225f)
				{
					this.highlightedAxis = item.AxisesI[i];
					this.highlightedAxisScreenDir = (vector - vector2).normalized;
					flag = false;
					break;
				}
			}
		}
		if (!this.bMousedPressed && flag)
		{
			this.highlightedAxis = Vector3i.zero;
		}
		item.HighlightAxis(this.highlightedAxis);
		if (this.bWaitForRelease && Input.GetMouseButton(0))
		{
			return;
		}
		this.bWaitForRelease = false;
		if (!this.highlightedAxis.Equals(Vector3i.zero))
		{
			if (!this.bMousedPressed && Input.GetMouseButtonDown(0))
			{
				this.bMousedPressed = true;
			}
			float magnitude = (primaryPlayer.cameraTransform.position - item.AxisOrigin).magnitude;
			float d = Math.Max(0.5f, magnitude / 35f);
			this.mouseMoveDir += new Vector3(-Input.GetAxis("Mouse X") * 5f, -Input.GetAxis("Mouse Y") * 5f, 0f) * d;
			float magnitude2 = this.mouseMoveDir.magnitude;
			if (this.bMousedPressed && !Input.GetMouseButtonUp(0) && GamePrefs.GetInt(EnumGamePrefs.SelectionOperationMode) == 3)
			{
				this.mirrorSelection(this.highlightedAxis);
				this.bWaitForRelease = true;
			}
			if (this.bMousedPressed && magnitude2 > 5f)
			{
				float num = (!this.highlightedAxis.Equals(Vector3i.one)) ? Vector3.Dot(this.highlightedAxisScreenDir, this.mouseMoveDir) : (-1f * Mathf.Sign(this.mouseMoveDir.y));
				num *= magnitude2 * 0.05f;
				if (Mathf.Abs(num) > 1f)
				{
					this.mouseMoveDir = Vector3.zero;
					int num2 = (int)Mathf.Sign(num);
					if (GamePrefs.GetInt(EnumGamePrefs.SelectionOperationMode) == 1)
					{
						this.moveSelection(this.highlightedAxis * num2);
					}
					else if (GamePrefs.GetInt(EnumGamePrefs.SelectionOperationMode) == 2)
					{
						this.incSelection((this.highlightedAxis.y > 0) ? (this.highlightedAxis.y * num2) : 0, (this.highlightedAxis.y < 0) ? (-1 * this.highlightedAxis.y * num2) : 0, (this.highlightedAxis.z > 0) ? (this.highlightedAxis.z * num2) : 0, (this.highlightedAxis.z < 0) ? (-1 * this.highlightedAxis.z * num2) : 0, (this.highlightedAxis.x > 0) ? (this.highlightedAxis.x * num2) : 0, (this.highlightedAxis.x < 0) ? (-1 * this.highlightedAxis.x * num2) : 0);
					}
				}
			}
		}
		if (!Input.GetMouseButton(0))
		{
			this.bMousedPressed = false;
			this.mouseMoveDir = Vector3.zero;
		}
	}

	// Token: 0x060048B0 RID: 18608 RVA: 0x001CC160 File Offset: 0x001CA360
	[PublicizedFrom(EAccessModifier.Private)]
	public float GetLineDistanceSq(Vector3 _lineStart, Vector3 _lineEnd, Vector3 _point)
	{
		Vector3 vector = _lineEnd - _lineStart;
		float sqrMagnitude = vector.sqrMagnitude;
		if (sqrMagnitude < 1E-06f)
		{
			return (_point - _lineStart).sqrMagnitude;
		}
		float d = Mathf.Clamp01(Vector3.Dot(_point - _lineStart, vector) / sqrMagnitude);
		return (_lineStart + vector * d - _point).sqrMagnitude;
	}

	// Token: 0x040037C0 RID: 14272
	public const string CategoryDynamicPrefab = "DynamicPrefabs";

	// Token: 0x040037C1 RID: 14273
	public const string CategoryStartPoint = "StartPoint";

	// Token: 0x040037C2 RID: 14274
	public const string CategorySelection = "Selection";

	// Token: 0x040037C3 RID: 14275
	public const string CategoryTraderTeleport = "TraderTeleport";

	// Token: 0x040037C4 RID: 14276
	public const string CategoryInfoVolume = "InfoVolume";

	// Token: 0x040037C5 RID: 14277
	public const string CategoryWallVolume = "WallVolume";

	// Token: 0x040037C6 RID: 14278
	public const string CategoryTriggerVolume = "TriggerVolume";

	// Token: 0x040037C7 RID: 14279
	public const string CategorySleeperVolume = "SleeperVolume";

	// Token: 0x040037C8 RID: 14280
	public const string CategoryPOIMarker = "POIMarker";

	// Token: 0x040037C9 RID: 14281
	public const string CategoryPrefabFacingVolume = "PrefabFacing";

	// Token: 0x040037CA RID: 14282
	public static Color ColDynamicPrefabInactive = new Color(0f, 0.4f, 0f, 0.6f);

	// Token: 0x040037CB RID: 14283
	public static Color ColDynamicPrefabActive = new Color(0.6f, 1f, 0f, 0.15f);

	// Token: 0x040037CC RID: 14284
	public static Color ColDynamicPrefabFaceSel = new Color(0f, 1f, 0f, 0.6f);

	// Token: 0x040037CD RID: 14285
	public static Color ColEntitySpawnerInactive = new Color(0.6f, 0f, 0f, 0.6f);

	// Token: 0x040037CE RID: 14286
	public static Color ColEntitySpawnerActive = new Color(1f, 0f, 0f, 0.4f);

	// Token: 0x040037CF RID: 14287
	public static Color ColEntitySpawnerFaceSel = new Color(1f, 1f, 0f, 0.3f);

	// Token: 0x040037D0 RID: 14288
	public static Color ColEntitySpawnerTrigger = new Color(1f, 1f, 0f, 0.3f);

	// Token: 0x040037D1 RID: 14289
	public static Color ColStartPointInactive = new Color(1f, 1f, 1f, 0.5f);

	// Token: 0x040037D2 RID: 14290
	public static Color ColStartPointActive = new Color(1f, 1f, 0f, 0.8f);

	// Token: 0x040037D3 RID: 14291
	public static Color ColSelectionActive = new Color(0f, 0f, 1f, 0.5f);

	// Token: 0x040037D4 RID: 14292
	public static Color ColSelectionInactive = new Color(0f, 0f, 1f, 0.5f);

	// Token: 0x040037D5 RID: 14293
	public static Color ColSelectionFaceSel = new Color(1f, 1f, 0f, 0.4f);

	// Token: 0x040037D6 RID: 14294
	public static Color ColTraderTeleportInactive = new Color(0.5f, 0f, 0.5f, 0.6f);

	// Token: 0x040037D7 RID: 14295
	public static Color ColTraderTeleport = new Color(1f, 0f, 1f, 0.3f);

	// Token: 0x040037D8 RID: 14296
	public static Color ColSleeperVolume = new Color(0.7f, 0.75f, 1f, 0.3f);

	// Token: 0x040037D9 RID: 14297
	public static Color ColSleeperVolumeInactive = new Color(0.25f, 0.25f, 0.5f, 0.6f);

	// Token: 0x040037DA RID: 14298
	public static Color ColTriggerVolume = new Color(1f, 0f, 0f, 0.4f);

	// Token: 0x040037DB RID: 14299
	public static Color ColTriggerVolumeInactive = new Color(0.6f, 0f, 0f, 0.6f);

	// Token: 0x040037DC RID: 14300
	public static Color ColInfoVolume = new Color(0f, 1f, 1f, 0.4f);

	// Token: 0x040037DD RID: 14301
	public static Color ColInfoVolumeInactive = new Color(0f, 0.6f, 0.6f, 0.6f);

	// Token: 0x040037DE RID: 14302
	public static Color ColWallVolume = new Color(0.5f, 1f, 1f, 0.4f);

	// Token: 0x040037DF RID: 14303
	public static Color ColWallVolumeInactive = new Color(0.5f, 0.6f, 0.6f, 0.6f);

	// Token: 0x040037E0 RID: 14304
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly Dictionary<string, SelectionCategory> categories = new Dictionary<string, SelectionCategory>();

	// Token: 0x040037E1 RID: 14305
	[TupleElementNames(new string[]
	{
		"category",
		"box"
	})]
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ValueTuple<SelectionCategory, SelectionBox>? selection;

	// Token: 0x040037E2 RID: 14306
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float alphaMultiplier = 1f;

	// Token: 0x040037E3 RID: 14307
	public static SelectionBoxManager Instance;

	// Token: 0x040037E4 RID: 14308
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bMousedPressed;

	// Token: 0x040037E5 RID: 14309
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 highlightedAxisScreenDir;

	// Token: 0x040037E6 RID: 14310
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3i highlightedAxis = Vector3i.zero;

	// Token: 0x040037E7 RID: 14311
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 mouseMoveDir = Vector3.zero;

	// Token: 0x040037E8 RID: 14312
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int lastSelOpMode;

	// Token: 0x040037E9 RID: 14313
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bWaitForRelease;
}
