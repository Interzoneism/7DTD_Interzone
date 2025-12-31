using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000EAA RID: 3754
[Preserve]
public class XUiC_WoPropsSleeperVolume : XUiController, ISelectionBoxCallback
{
	// Token: 0x17000C15 RID: 3093
	// (get) Token: 0x060076AB RID: 30379 RVA: 0x003053B5 File Offset: 0x003035B5
	public static int selectedVolumeIndex
	{
		get
		{
			if (XUiC_WoPropsSleeperVolume.instance != null && XUiC_WoPropsSleeperVolume.instance.m_selectedPrefabInstance != null)
			{
				return XUiC_WoPropsSleeperVolume.instance.selIdx;
			}
			return -1;
		}
	}

	// Token: 0x17000C16 RID: 3094
	// (get) Token: 0x060076AC RID: 30380 RVA: 0x003053D6 File Offset: 0x003035D6
	public static PrefabInstance selectedPrefabInstance
	{
		get
		{
			if (XUiC_WoPropsSleeperVolume.instance != null)
			{
				return XUiC_WoPropsSleeperVolume.instance.m_selectedPrefabInstance;
			}
			return null;
		}
	}

	// Token: 0x17000C17 RID: 3095
	// (get) Token: 0x060076AD RID: 30381 RVA: 0x003053EB File Offset: 0x003035EB
	public List<byte> TriggeredByIndices
	{
		get
		{
			if (this.m_selectedPrefabInstance != null)
			{
				return this.m_selectedPrefabInstance.prefab.SleeperVolumes[this.selIdx].triggeredByIndices;
			}
			return null;
		}
	}

	// Token: 0x060076AE RID: 30382 RVA: 0x00305418 File Offset: 0x00303618
	public override void Init()
	{
		base.Init();
		XUiC_WoPropsSleeperVolume.ID = base.WindowGroup.ID;
		XUiC_WoPropsSleeperVolume.instance = this;
		this.labelIndex = (base.GetChildById("labelIndex").ViewComponent as XUiV_Label);
		this.labelPosition = (base.GetChildById("labelPosition").ViewComponent as XUiV_Label);
		this.labelSize = (base.GetChildById("labelSize").ViewComponent as XUiV_Label);
		this.labelSleeperCount = (base.GetChildById("labelSleeperCount").ViewComponent as XUiV_Label);
		this.labelGroup = (base.GetChildById("labelGroup").ViewComponent as XUiV_Label);
		this.txtGroupId = (XUiC_TextInput)base.GetChildById("groupId");
		this.txtGroupId.OnChangeHandler += this.TxtGroupId_OnChangeHandler;
		this.cbxPriority = (XUiC_ComboBoxBool)base.GetChildById("cbxPriority");
		this.cbxPriority.OnValueChanged += this.CbxPriority_OnValueChanged;
		this.cbxQuestExclude = (XUiC_ComboBoxBool)base.GetChildById("cbxQuestExclude");
		this.cbxQuestExclude.OnValueChanged += this.CbxQuestExclude_OnValueChanged;
		this.cbxCountPreset = (XUiC_ComboBoxList<XUiC_WoPropsSleeperVolume.CountPreset>)base.GetChildById("cbxCountPreset");
		this.cbxCountPreset.OnValueChanged += this.CbxCountPreset_OnValueChanged;
		this.cbxCountPreset.Elements.Add(new XUiC_WoPropsSleeperVolume.CountPreset(-1, -1, "Custom"));
		this.cbxCountPreset.Elements.Add(new XUiC_WoPropsSleeperVolume.CountPreset(1, 2, "12"));
		this.cbxCountPreset.Elements.Add(new XUiC_WoPropsSleeperVolume.CountPreset(2, 3, "23"));
		this.cbxCountPreset.Elements.Add(new XUiC_WoPropsSleeperVolume.CountPreset(3, 4, "34"));
		this.cbxCountPreset.Elements.Add(new XUiC_WoPropsSleeperVolume.CountPreset(4, 5, "45"));
		this.cbxCountPreset.Elements.Add(new XUiC_WoPropsSleeperVolume.CountPreset(5, 6, "56"));
		this.cbxCountPreset.Elements.Add(new XUiC_WoPropsSleeperVolume.CountPreset(6, 7, "67"));
		this.cbxCountPreset.Elements.Add(new XUiC_WoPropsSleeperVolume.CountPreset(7, 8, "78"));
		this.cbxCountPreset.Elements.Add(new XUiC_WoPropsSleeperVolume.CountPreset(8, 9, "89"));
		this.cbxCountPreset.Elements.Add(new XUiC_WoPropsSleeperVolume.CountPreset(9, 10, "910"));
		this.cbxCountPreset.MinIndex = 1;
		this.txtSpawnMin = (XUiC_TextInput)base.GetChildById("spawnMin");
		this.txtSpawnMin.OnChangeHandler += this.TxtSpawnMin_OnChangeHandler;
		this.txtSpawnMax = (XUiC_TextInput)base.GetChildById("spawnMax");
		this.txtSpawnMax.OnChangeHandler += this.TxtSpawnMax_OnChangeHandler;
		this.cbxTrigger = (XUiC_ComboBoxEnum<SleeperVolume.ETriggerType>)base.GetChildById("cbxTrigger");
		this.cbxTrigger.OnValueChanged += this.CbxTrigger_OnValueChanged;
		this.txtMinScript = (XUiC_TextInput)base.GetChildById("script");
		this.txtMinScript.OnChangeHandler += this.TxtMinScript_OnChangeHandler;
		this.spawnersList = (XUiC_SpawnersList)base.GetChildById("spawners");
		this.spawnersList.SelectionChanged += this.SpawnersList_SelectionChanged;
		this.spawnersList.SelectableEntries = false;
		this.triggersBox = base.GetChildById("triggersBox").ViewComponent;
		this.triggeredByList = (base.GetChildById("triggeredBy") as XUiC_PrefabTriggerEditorList);
		if (this.triggeredByList != null)
		{
			this.triggeredByList.SelectionChanged += this.TriggeredByList_SelectionChanged;
		}
		XUiController childById = base.GetChildById("addTriggeredByButton");
		if (childById != null)
		{
			childById.OnPress += this.HandleAddTriggeredByEntry;
		}
		if (SelectionBoxManager.Instance != null)
		{
			SelectionBoxManager.Instance.GetCategory("SleeperVolume").SetCallback(this);
		}
	}

	// Token: 0x060076AF RID: 30383 RVA: 0x0030581E File Offset: 0x00303A1E
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleAddTriggeredByEntry(XUiController _sender, int _mouseButton)
	{
		this.TriggerOnAddTriggersPressed();
	}

	// Token: 0x060076B0 RID: 30384 RVA: 0x00305828 File Offset: 0x00303A28
	[PublicizedFrom(EAccessModifier.Private)]
	public void TxtGroupId_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (!_changeFromCode && _text.Length > 0 && this.m_selectedPrefabInstance != null)
		{
			Prefab.PrefabSleeperVolume prefabSleeperVolume = new Prefab.PrefabSleeperVolume(this.m_selectedPrefabInstance.prefab.SleeperVolumes[this.selIdx]);
			short groupId = StringParsers.ParseSInt16(_text, 0, -1, NumberStyles.Integer);
			prefabSleeperVolume.groupId = groupId;
			PrefabSleeperVolumeManager.Instance.UpdateSleeperPropertiesServer(this.m_selectedPrefabInstance.id, this.selIdx, prefabSleeperVolume);
		}
	}

	// Token: 0x060076B1 RID: 30385 RVA: 0x00305898 File Offset: 0x00303A98
	[PublicizedFrom(EAccessModifier.Private)]
	public void CbxPriority_OnValueChanged(XUiController _sender, bool _oldValue, bool _newValue)
	{
		if (this.m_selectedPrefabInstance != null)
		{
			Prefab.PrefabSleeperVolume prefabSleeperVolume = new Prefab.PrefabSleeperVolume(this.m_selectedPrefabInstance.prefab.SleeperVolumes[this.selIdx]);
			prefabSleeperVolume.isPriority = _newValue;
			PrefabSleeperVolumeManager.Instance.UpdateSleeperPropertiesServer(this.m_selectedPrefabInstance.id, this.selIdx, prefabSleeperVolume);
		}
	}

	// Token: 0x060076B2 RID: 30386 RVA: 0x003058F4 File Offset: 0x00303AF4
	[PublicizedFrom(EAccessModifier.Private)]
	public void CbxQuestExclude_OnValueChanged(XUiController _sender, bool _oldValue, bool _newValue)
	{
		if (this.m_selectedPrefabInstance != null)
		{
			Prefab.PrefabSleeperVolume prefabSleeperVolume = new Prefab.PrefabSleeperVolume(this.m_selectedPrefabInstance.prefab.SleeperVolumes[this.selIdx]);
			prefabSleeperVolume.isQuestExclude = _newValue;
			PrefabSleeperVolumeManager.Instance.UpdateSleeperPropertiesServer(this.m_selectedPrefabInstance.id, this.selIdx, prefabSleeperVolume);
		}
	}

	// Token: 0x060076B3 RID: 30387 RVA: 0x00305950 File Offset: 0x00303B50
	[PublicizedFrom(EAccessModifier.Private)]
	public int FindCountPresetIndex(int _min, int _max)
	{
		for (int i = 0; i < this.cbxCountPreset.Elements.Count; i++)
		{
			if ((int)this.cbxCountPreset.Elements[i].min == _min && (int)this.cbxCountPreset.Elements[i].max == _max)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060076B4 RID: 30388 RVA: 0x003059B0 File Offset: 0x00303BB0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateCountPresetLabel()
	{
		if (this.m_selectedPrefabInstance == null)
		{
			return;
		}
		Prefab.PrefabSleeperVolume prefabSleeperVolume = this.m_selectedPrefabInstance.prefab.SleeperVolumes[this.selIdx];
		int num = this.FindCountPresetIndex((int)prefabSleeperVolume.spawnCountMin, (int)prefabSleeperVolume.spawnCountMax);
		if (num < 0)
		{
			this.cbxCountPreset.MinIndex = 0;
			this.cbxCountPreset.SelectedIndex = 0;
			return;
		}
		this.cbxCountPreset.MinIndex = 1;
		this.cbxCountPreset.SelectedIndex = num;
	}

	// Token: 0x060076B5 RID: 30389 RVA: 0x00305A2C File Offset: 0x00303C2C
	[PublicizedFrom(EAccessModifier.Private)]
	public void CbxCountPreset_OnValueChanged(XUiController _sender, XUiC_WoPropsSleeperVolume.CountPreset _oldvalue, XUiC_WoPropsSleeperVolume.CountPreset _newvalue)
	{
		this.cbxCountPreset.MinIndex = 1;
		if (this.m_selectedPrefabInstance != null)
		{
			Prefab.PrefabSleeperVolume prefabSleeperVolume = new Prefab.PrefabSleeperVolume(this.m_selectedPrefabInstance.prefab.SleeperVolumes[this.selIdx]);
			prefabSleeperVolume.spawnCountMin = _newvalue.min;
			prefabSleeperVolume.spawnCountMax = _newvalue.max;
			PrefabSleeperVolumeManager.Instance.UpdateSleeperPropertiesServer(this.m_selectedPrefabInstance.id, this.selIdx, prefabSleeperVolume);
		}
		this.UpdateCountPresetLabel();
	}

	// Token: 0x060076B6 RID: 30390 RVA: 0x00305AA8 File Offset: 0x00303CA8
	[PublicizedFrom(EAccessModifier.Private)]
	public void TxtSpawnMin_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (!_changeFromCode && _text.Length > 0)
		{
			short spawnCountMin = StringParsers.ParseSInt16(_text, 0, -1, NumberStyles.Integer);
			if (this.m_selectedPrefabInstance != null)
			{
				Prefab.PrefabSleeperVolume prefabSleeperVolume = new Prefab.PrefabSleeperVolume(this.m_selectedPrefabInstance.prefab.SleeperVolumes[this.selIdx]);
				prefabSleeperVolume.spawnCountMin = spawnCountMin;
				PrefabSleeperVolumeManager.Instance.UpdateSleeperPropertiesServer(this.m_selectedPrefabInstance.id, this.selIdx, prefabSleeperVolume);
				this.UpdateCountPresetLabel();
			}
		}
	}

	// Token: 0x060076B7 RID: 30391 RVA: 0x00305B20 File Offset: 0x00303D20
	[PublicizedFrom(EAccessModifier.Private)]
	public void TxtSpawnMax_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (!_changeFromCode && _text.Length > 0)
		{
			short spawnCountMax = StringParsers.ParseSInt16(_text, 0, -1, NumberStyles.Integer);
			if (this.m_selectedPrefabInstance != null)
			{
				Prefab.PrefabSleeperVolume prefabSleeperVolume = new Prefab.PrefabSleeperVolume(this.m_selectedPrefabInstance.prefab.SleeperVolumes[this.selIdx]);
				prefabSleeperVolume.spawnCountMax = spawnCountMax;
				PrefabSleeperVolumeManager.Instance.UpdateSleeperPropertiesServer(this.m_selectedPrefabInstance.id, this.selIdx, prefabSleeperVolume);
				this.UpdateCountPresetLabel();
			}
		}
	}

	// Token: 0x060076B8 RID: 30392 RVA: 0x00305B98 File Offset: 0x00303D98
	[PublicizedFrom(EAccessModifier.Private)]
	public void CbxTrigger_OnValueChanged(XUiController _sender, SleeperVolume.ETriggerType _oldValue, SleeperVolume.ETriggerType _newValue)
	{
		if (this.m_selectedPrefabInstance != null)
		{
			Prefab.PrefabSleeperVolume prefabSleeperVolume = new Prefab.PrefabSleeperVolume(this.m_selectedPrefabInstance.prefab.SleeperVolumes[this.selIdx]);
			prefabSleeperVolume.SetTrigger(_newValue);
			PrefabSleeperVolumeManager.Instance.UpdateSleeperPropertiesServer(this.m_selectedPrefabInstance.id, this.selIdx, prefabSleeperVolume);
			this.triggersBox.IsVisible = true;
		}
	}

	// Token: 0x060076B9 RID: 30393 RVA: 0x00305C00 File Offset: 0x00303E00
	[PublicizedFrom(EAccessModifier.Private)]
	public void TxtMinScript_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (!_changeFromCode && this.m_selectedPrefabInstance != null)
		{
			Prefab.PrefabSleeperVolume prefabSleeperVolume = new Prefab.PrefabSleeperVolume(this.m_selectedPrefabInstance.prefab.SleeperVolumes[this.selIdx]);
			prefabSleeperVolume.minScript = MinScript.ConvertFromUIText(_text);
			PrefabSleeperVolumeManager.Instance.UpdateSleeperPropertiesServer(this.m_selectedPrefabInstance.id, this.selIdx, prefabSleeperVolume);
		}
	}

	// Token: 0x060076BA RID: 30394 RVA: 0x00305C64 File Offset: 0x00303E64
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpawnersList_SelectionChanged(XUiC_ListEntry<XUiC_SpawnersList.SpawnerEntry> _previousEntry, XUiC_ListEntry<XUiC_SpawnersList.SpawnerEntry> _newEntry)
	{
		string groupName = null;
		if (_newEntry != null)
		{
			groupName = _newEntry.GetEntry().name;
		}
		if (this.m_selectedPrefabInstance != null)
		{
			Prefab.PrefabSleeperVolume prefabSleeperVolume = new Prefab.PrefabSleeperVolume(this.m_selectedPrefabInstance.prefab.SleeperVolumes[this.selIdx]);
			prefabSleeperVolume.groupName = groupName;
			PrefabSleeperVolumeManager.Instance.UpdateSleeperPropertiesServer(this.m_selectedPrefabInstance.id, this.selIdx, prefabSleeperVolume);
		}
	}

	// Token: 0x060076BB RID: 30395 RVA: 0x00305CD0 File Offset: 0x00303ED0
	[PublicizedFrom(EAccessModifier.Private)]
	public void TriggeredByList_SelectionChanged(XUiC_ListEntry<XUiC_PrefabTriggerEditorList.PrefabTriggerEntry> _previousEntry, XUiC_ListEntry<XUiC_PrefabTriggerEditorList.PrefabTriggerEntry> _newEntry)
	{
		if (_newEntry != null)
		{
			byte triggerLayer = 0;
			if (StringParsers.TryParseUInt8(_newEntry.GetEntry().name, out triggerLayer, 0, -1, NumberStyles.Integer))
			{
				Prefab.PrefabSleeperVolume prefabSleeperVolume = new Prefab.PrefabSleeperVolume(this.m_selectedPrefabInstance.prefab.SleeperVolumes[this.selIdx]);
				if (prefabSleeperVolume != null)
				{
					this.HandleTriggersSetting(prefabSleeperVolume, triggerLayer, false, GameManager.Instance.World);
				}
				PrefabSleeperVolumeManager.Instance.UpdateSleeperPropertiesServer(this.m_selectedPrefabInstance.id, this.selIdx, prefabSleeperVolume);
			}
			_newEntry.IsDirty = true;
		}
	}

	// Token: 0x060076BC RID: 30396 RVA: 0x00305D53 File Offset: 0x00303F53
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleTriggersSetting(Prefab.PrefabSleeperVolume psv, byte triggerLayer, bool isTriggers, World _world)
	{
		if (_world.IsEditor() && !isTriggers)
		{
			if (psv.HasTriggeredBy(triggerLayer))
			{
				psv.RemoveTriggeredByFlag(triggerLayer);
				return;
			}
			psv.SetTriggeredByFlag(triggerLayer);
		}
	}

	// Token: 0x060076BD RID: 30397 RVA: 0x00305D79 File Offset: 0x00303F79
	[PublicizedFrom(EAccessModifier.Private)]
	public void TriggerOnAddTriggersPressed()
	{
		if (this.m_selectedPrefabInstance != null)
		{
			this.m_selectedPrefabInstance.prefab.AddNewTriggerLayer();
			this.triggeredByList.RebuildList(false);
		}
	}

	// Token: 0x060076BE RID: 30398 RVA: 0x00305D9F File Offset: 0x00303F9F
	public override void OnOpen()
	{
		base.OnOpen();
		this.UpdateCountPresetLabel();
	}

	// Token: 0x060076BF RID: 30399 RVA: 0x00305DB0 File Offset: 0x00303FB0
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.m_selectedPrefabInstance != null)
		{
			if (this.bSleeperVolumeChanged)
			{
				this.bSleeperVolumeChanged = false;
				this.m_selectedPrefabInstance.prefab.CountSleeperSpawnsInVolume(GameManager.Instance.World, this.m_selectedPrefabInstance.boundingBoxPosition, this.selIdx);
				this.UpdateCountPresetLabel();
			}
			Prefab.PrefabSleeperVolume prefabSleeperVolume = this.m_selectedPrefabInstance.prefab.SleeperVolumes[this.selIdx];
			this.txtGroupId.Text = prefabSleeperVolume.groupId.ToString();
			this.cbxPriority.Value = prefabSleeperVolume.isPriority;
			this.cbxQuestExclude.Value = prefabSleeperVolume.isQuestExclude;
			this.labelIndex.Text = this.selIdx.ToString();
			this.labelPosition.Text = prefabSleeperVolume.startPos.ToString();
			this.labelSize.Text = prefabSleeperVolume.size.ToString();
			this.labelSleeperCount.Text = this.m_selectedPrefabInstance.prefab.Transient_NumSleeperSpawns.ToString();
			this.labelGroup.Text = GameStageGroup.MakeDisplayName(prefabSleeperVolume.groupName);
			this.txtSpawnMin.Text = prefabSleeperVolume.spawnCountMin.ToString();
			this.txtSpawnMax.Text = prefabSleeperVolume.spawnCountMax.ToString();
			this.cbxTrigger.Value = (SleeperVolume.ETriggerType)(prefabSleeperVolume.flags & 7);
			this.txtMinScript.Text = MinScript.ConvertToUIText(prefabSleeperVolume.minScript);
		}
		else
		{
			this.txtGroupId.Text = string.Empty;
			this.cbxPriority.Value = false;
			this.cbxQuestExclude.Value = false;
			this.labelIndex.Text = string.Empty;
			this.labelPosition.Text = string.Empty;
			this.labelSize.Text = string.Empty;
			this.labelSleeperCount.Text = string.Empty;
			this.labelGroup.Text = string.Empty;
			this.txtSpawnMin.Text = string.Empty;
			this.txtSpawnMax.Text = string.Empty;
			this.cbxTrigger.Value = SleeperVolume.ETriggerType.Active;
			this.txtMinScript.Text = string.Empty;
		}
		this.triggersBox.IsVisible = true;
	}

	// Token: 0x060076C0 RID: 30400 RVA: 0x00306001 File Offset: 0x00304201
	public override void Cleanup()
	{
		base.Cleanup();
		if (SelectionBoxManager.Instance != null)
		{
			SelectionBoxManager.Instance.GetCategory("SleeperVolume").SetCallback(null);
		}
		XUiC_WoPropsSleeperVolume.instance = null;
	}

	// Token: 0x060076C1 RID: 30401 RVA: 0x00306034 File Offset: 0x00304234
	public bool OnSelectionBoxActivated(string _category, string _name, bool _bActivated)
	{
		if (_bActivated)
		{
			int num;
			int num2;
			if (this.getPrefabIdAndVolumeId(_name, out num, out num2))
			{
				this.selIdx = num2;
			}
		}
		else
		{
			this.m_selectedPrefabInstance = null;
		}
		return true;
	}

	// Token: 0x060076C2 RID: 30402 RVA: 0x00306064 File Offset: 0x00304264
	[PublicizedFrom(EAccessModifier.Private)]
	public bool getPrefabIdAndVolumeId(string _name, out int _prefabInstanceId, out int _volumeId)
	{
		_prefabInstanceId = (_volumeId = 0);
		string[] array = _name.Split('.', StringSplitOptions.None);
		if (array.Length > 1)
		{
			string[] array2 = array[1].Split('_', StringSplitOptions.None);
			if (array2.Length > 1 && int.TryParse(array2[1], out _volumeId) && int.TryParse(array2[0], out _prefabInstanceId))
			{
				this.m_selectedPrefabInstance = PrefabSleeperVolumeManager.Instance.GetPrefabInstance(_prefabInstanceId);
				this.bSleeperVolumeChanged = true;
				Prefab prefab = this.m_selectedPrefabInstance.prefab;
				this.triggeredByList.EditPrefab = prefab;
				this.triggeredByList.SleeperOwner = this;
				this.triggeredByList.IsTriggers = false;
				if (prefab.TriggerLayers.Count == 0)
				{
					prefab.AddInitialTriggerLayers();
				}
				return true;
			}
		}
		return false;
	}

	// Token: 0x060076C3 RID: 30403 RVA: 0x00306113 File Offset: 0x00304313
	public static void SleeperVolumeChanged(int _prefabInstanceId, int _volumeId)
	{
		if (XUiC_WoPropsSleeperVolume.selectedPrefabInstance == null)
		{
			return;
		}
		if (XUiC_WoPropsSleeperVolume.selectedPrefabInstance.id != _prefabInstanceId || XUiC_WoPropsSleeperVolume.selectedVolumeIndex != _volumeId)
		{
			return;
		}
		XUiC_WoPropsSleeperVolume.instance.bSleeperVolumeChanged = true;
	}

	// Token: 0x060076C4 RID: 30404 RVA: 0x00306140 File Offset: 0x00304340
	public void OnSelectionBoxMoved(string _category, string _name, Vector3 _moveVector)
	{
		if (this.m_selectedPrefabInstance == null)
		{
			return;
		}
		Prefab.PrefabSleeperVolume prefabSleeperVolume = new Prefab.PrefabSleeperVolume(this.m_selectedPrefabInstance.prefab.SleeperVolumes[this.selIdx]);
		prefabSleeperVolume.startPos += new Vector3i(_moveVector);
		PrefabSleeperVolumeManager.Instance.UpdateSleeperPropertiesServer(this.m_selectedPrefabInstance.id, this.selIdx, prefabSleeperVolume);
	}

	// Token: 0x060076C5 RID: 30405 RVA: 0x003061AC File Offset: 0x003043AC
	public void OnSelectionBoxSized(string _category, string _name, int _dTop, int _dBottom, int _dNorth, int _dSouth, int _dEast, int _dWest)
	{
		if (this.m_selectedPrefabInstance == null)
		{
			return;
		}
		Prefab.PrefabSleeperVolume prefabSleeperVolume = new Prefab.PrefabSleeperVolume(this.m_selectedPrefabInstance.prefab.SleeperVolumes[this.selIdx]);
		prefabSleeperVolume.size += new Vector3i(_dEast + _dWest, _dTop + _dBottom, _dNorth + _dSouth);
		prefabSleeperVolume.startPos += new Vector3i(-_dWest, -_dBottom, -_dSouth);
		Vector3i size = prefabSleeperVolume.size;
		if (size.x < 2)
		{
			size = new Vector3i(1, size.y, size.z);
		}
		if (size.y < 2)
		{
			size = new Vector3i(size.x, 1, size.z);
		}
		if (size.z < 2)
		{
			size = new Vector3i(size.x, size.y, 1);
		}
		prefabSleeperVolume.size = size;
		PrefabSleeperVolumeManager.Instance.UpdateSleeperPropertiesServer(this.m_selectedPrefabInstance.id, this.selIdx, prefabSleeperVolume);
	}

	// Token: 0x060076C6 RID: 30406 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnSelectionBoxMirrored(Vector3i _axis)
	{
	}

	// Token: 0x060076C7 RID: 30407 RVA: 0x003062A8 File Offset: 0x003044A8
	public bool OnSelectionBoxDelete(string _category, string _name)
	{
		using (IEnumerator<LocalPlayerUI> enumerator = LocalPlayerUI.PlayerUIs.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.windowManager.IsModalWindowOpen())
				{
					SelectionBoxManager.Instance.SetActive(_category, _name, true);
					return false;
				}
			}
		}
		int num;
		int num2;
		if (this.getPrefabIdAndVolumeId(_name, out num, out num2))
		{
			Prefab.PrefabSleeperVolume prefabSleeperVolume = new Prefab.PrefabSleeperVolume(this.m_selectedPrefabInstance.prefab.SleeperVolumes[num2]);
			prefabSleeperVolume.used = false;
			PrefabSleeperVolumeManager.Instance.UpdateSleeperPropertiesServer(this.m_selectedPrefabInstance.id, num2, prefabSleeperVolume);
			return true;
		}
		return false;
	}

	// Token: 0x060076C8 RID: 30408 RVA: 0x002F7CE8 File Offset: 0x002F5EE8
	public bool OnSelectionBoxIsAvailable(string _category, EnumSelectionBoxAvailabilities _criteria)
	{
		return _criteria == EnumSelectionBoxAvailabilities.CanShowProperties || _criteria == EnumSelectionBoxAvailabilities.CanResize;
	}

	// Token: 0x060076C9 RID: 30409 RVA: 0x00306358 File Offset: 0x00304558
	public void OnSelectionBoxShowProperties(bool _bVisible, GUIWindowManager _windowManager)
	{
		string text;
		string text2;
		if (SelectionBoxManager.Instance.GetSelected(out text, out text2) && text.Equals("SleeperVolume"))
		{
			_windowManager.SwitchVisible(XUiC_WoPropsSleeperVolume.ID, false, true);
		}
	}

	// Token: 0x060076CA RID: 30410 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnSelectionBoxRotated(string _category, string _name)
	{
	}

	// Token: 0x060076CB RID: 30411 RVA: 0x00306390 File Offset: 0x00304590
	public static bool GetSelectedVolumeStats(out XUiC_WoPropsSleeperVolume.VolumeStats _stats)
	{
		_stats = default(XUiC_WoPropsSleeperVolume.VolumeStats);
		int selectedVolumeIndex = XUiC_WoPropsSleeperVolume.selectedVolumeIndex;
		if (selectedVolumeIndex >= 0)
		{
			if (XUiC_WoPropsSleeperVolume.instance.bSleeperVolumeChanged)
			{
				XUiC_WoPropsSleeperVolume.instance.bSleeperVolumeChanged = false;
				XUiC_WoPropsSleeperVolume.selectedPrefabInstance.prefab.CountSleeperSpawnsInVolume(GameManager.Instance.World, XUiC_WoPropsSleeperVolume.selectedPrefabInstance.boundingBoxPosition, selectedVolumeIndex);
				XUiC_WoPropsSleeperVolume.instance.UpdateCountPresetLabel();
			}
			Prefab.PrefabSleeperVolume prefabSleeperVolume = XUiC_WoPropsSleeperVolume.selectedPrefabInstance.prefab.SleeperVolumes[selectedVolumeIndex];
			_stats.index = selectedVolumeIndex;
			_stats.pos = XUiC_WoPropsSleeperVolume.selectedPrefabInstance.boundingBoxPosition + prefabSleeperVolume.startPos;
			_stats.size = prefabSleeperVolume.size;
			_stats.groupName = GameStageGroup.MakeDisplayName(prefabSleeperVolume.groupName);
			_stats.isPriority = prefabSleeperVolume.isPriority;
			_stats.isQuestExclude = prefabSleeperVolume.isQuestExclude;
			_stats.sleeperCount = XUiC_WoPropsSleeperVolume.selectedPrefabInstance.prefab.Transient_NumSleeperSpawns;
			_stats.spawnCountMin = (int)prefabSleeperVolume.spawnCountMin;
			_stats.spawnCountMax = (int)prefabSleeperVolume.spawnCountMax;
			return true;
		}
		return false;
	}

	// Token: 0x04005A7A RID: 23162
	public static string ID = "";

	// Token: 0x04005A7B RID: 23163
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiView triggersBox;

	// Token: 0x04005A7C RID: 23164
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label labelIndex;

	// Token: 0x04005A7D RID: 23165
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label labelPosition;

	// Token: 0x04005A7E RID: 23166
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label labelSize;

	// Token: 0x04005A7F RID: 23167
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label labelSleeperCount;

	// Token: 0x04005A80 RID: 23168
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label labelGroup;

	// Token: 0x04005A81 RID: 23169
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtGroupId;

	// Token: 0x04005A82 RID: 23170
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool cbxPriority;

	// Token: 0x04005A83 RID: 23171
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool cbxQuestExclude;

	// Token: 0x04005A84 RID: 23172
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<XUiC_WoPropsSleeperVolume.CountPreset> cbxCountPreset;

	// Token: 0x04005A85 RID: 23173
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtSpawnMin;

	// Token: 0x04005A86 RID: 23174
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtSpawnMax;

	// Token: 0x04005A87 RID: 23175
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxEnum<SleeperVolume.ETriggerType> cbxTrigger;

	// Token: 0x04005A88 RID: 23176
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtMinScript;

	// Token: 0x04005A89 RID: 23177
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SpawnersList spawnersList;

	// Token: 0x04005A8A RID: 23178
	[PublicizedFrom(EAccessModifier.Private)]
	public PrefabInstance m_selectedPrefabInstance;

	// Token: 0x04005A8B RID: 23179
	[PublicizedFrom(EAccessModifier.Private)]
	public int selIdx;

	// Token: 0x04005A8C RID: 23180
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bSleeperVolumeChanged;

	// Token: 0x04005A8D RID: 23181
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PrefabTriggerEditorList triggeredByList;

	// Token: 0x04005A8E RID: 23182
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showTriggeredBy;

	// Token: 0x04005A8F RID: 23183
	[PublicizedFrom(EAccessModifier.Private)]
	public static XUiC_WoPropsSleeperVolume instance;

	// Token: 0x02000EAB RID: 3755
	public struct VolumeStats
	{
		// Token: 0x04005A90 RID: 23184
		public int index;

		// Token: 0x04005A91 RID: 23185
		public Vector3i pos;

		// Token: 0x04005A92 RID: 23186
		public Vector3i size;

		// Token: 0x04005A93 RID: 23187
		public string groupName;

		// Token: 0x04005A94 RID: 23188
		public int sleeperCount;

		// Token: 0x04005A95 RID: 23189
		public int spawnCountMin;

		// Token: 0x04005A96 RID: 23190
		public int spawnCountMax;

		// Token: 0x04005A97 RID: 23191
		public bool isPriority;

		// Token: 0x04005A98 RID: 23192
		public bool isQuestExclude;
	}

	// Token: 0x02000EAC RID: 3756
	public struct CountPreset
	{
		// Token: 0x060076CE RID: 30414 RVA: 0x0030649F File Offset: 0x0030469F
		public CountPreset(short _min, short _max, string _name)
		{
			this.min = _min;
			this.max = _max;
			this.name = _name;
		}

		// Token: 0x060076CF RID: 30415 RVA: 0x003064B6 File Offset: 0x003046B6
		public override string ToString()
		{
			return this.name;
		}

		// Token: 0x04005A99 RID: 23193
		public readonly short min;

		// Token: 0x04005A9A RID: 23194
		public readonly short max;

		// Token: 0x04005A9B RID: 23195
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string name;
	}
}
