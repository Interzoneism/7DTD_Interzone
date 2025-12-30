using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E7D RID: 3709
[Preserve]
public class XUiC_TriggerProperties : XUiController, ISelectionBoxCallback
{
	// Token: 0x17000BDC RID: 3036
	// (get) Token: 0x06007498 RID: 29848 RVA: 0x002F6F97 File Offset: 0x002F5197
	// (set) Token: 0x06007499 RID: 29849 RVA: 0x002F6F9F File Offset: 0x002F519F
	public Vector3i BlockPos
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.blockPos;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			this.blockPos = value;
			this.triggerVolume = null;
			this.SetupTrigger();
		}
	}

	// Token: 0x17000BDD RID: 3037
	// (get) Token: 0x0600749A RID: 29850 RVA: 0x002F6FB5 File Offset: 0x002F51B5
	// (set) Token: 0x0600749B RID: 29851 RVA: 0x002F6FBD File Offset: 0x002F51BD
	public Prefab.PrefabTriggerVolume TriggerVolume
	{
		get
		{
			return this.triggerVolume;
		}
		set
		{
			this.triggerVolume = value;
			this.blockTrigger = null;
		}
	}

	// Token: 0x17000BDE RID: 3038
	// (get) Token: 0x0600749C RID: 29852 RVA: 0x002F6FCD File Offset: 0x002F51CD
	public List<byte> TriggersIndices
	{
		get
		{
			if (this.blockTrigger != null)
			{
				return this.blockTrigger.TriggersIndices;
			}
			if (this.triggerVolume != null)
			{
				return this.triggerVolume.TriggersIndices;
			}
			return null;
		}
	}

	// Token: 0x17000BDF RID: 3039
	// (get) Token: 0x0600749D RID: 29853 RVA: 0x002F6FF8 File Offset: 0x002F51F8
	public List<byte> TriggeredByIndices
	{
		get
		{
			if (this.blockTrigger != null)
			{
				return this.blockTrigger.TriggeredByIndices;
			}
			return null;
		}
	}

	// Token: 0x17000BE0 RID: 3040
	// (get) Token: 0x0600749E RID: 29854 RVA: 0x002F700F File Offset: 0x002F520F
	// (set) Token: 0x0600749F RID: 29855 RVA: 0x002F7018 File Offset: 0x002F5218
	public Prefab Prefab
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.prefab;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (value != this.prefab)
			{
				this.prefab = value;
				this.triggersList.EditPrefab = value;
				this.triggersList.Owner = this;
				this.triggersList.IsTriggers = true;
				this.triggeredByList.EditPrefab = value;
				this.triggeredByList.Owner = this;
				this.triggeredByList.IsTriggers = false;
				if (this.prefab.TriggerLayers.Count == 0)
				{
					this.prefab.AddInitialTriggerLayers();
				}
			}
		}
	}

	// Token: 0x060074A0 RID: 29856 RVA: 0x002F709C File Offset: 0x002F529C
	public override void Init()
	{
		base.Init();
		XUiC_TriggerProperties.ID = base.WindowGroup.ID;
		this.triggersList = (base.GetChildById("triggers") as XUiC_PrefabTriggerEditorList);
		if (this.triggersList != null)
		{
			this.triggersList.SelectionChanged += this.TriggersList_SelectionChanged;
		}
		XUiController childById = base.GetChildById("addTriggersButton");
		if (childById != null)
		{
			childById.OnPress += this.HandleAddTriggersEntry;
		}
		this.triggeredByList = (base.GetChildById("triggeredBy") as XUiC_PrefabTriggerEditorList);
		if (this.triggeredByList != null)
		{
			this.triggeredByList.SelectionChanged += this.TriggeredByList_SelectionChanged;
		}
		XUiController childById2 = base.GetChildById("addTriggeredByButton");
		if (childById2 != null)
		{
			childById2.OnPress += this.HandleAddTriggeredByEntry;
		}
		XUiController childById3 = base.GetChildById("exclude");
		if (childById3 != null)
		{
			childById3.OnPress += this.triggerExclude_OnPressed;
		}
		childById3 = base.GetChildById("operation");
		if (childById3 != null)
		{
			childById3.OnPress += this.triggerOperation_OnPressed;
		}
		childById3 = base.GetChildById("unlock");
		if (childById3 != null)
		{
			childById3.OnPress += this.triggerUnlock_OnPressed;
		}
		if (SelectionBoxManager.Instance != null)
		{
			SelectionBoxManager.Instance.GetCategory("TriggerVolume").SetCallback(this);
		}
	}

	// Token: 0x060074A1 RID: 29857 RVA: 0x002F71F0 File Offset: 0x002F53F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void triggerExclude_OnPressed(XUiController controller, int button)
	{
		if (this.blockTrigger != null)
		{
			this.blockTrigger.ExcludeIcon = !this.blockTrigger.ExcludeIcon;
			Chunk chunkModified = (Chunk)GameManager.Instance.World.ChunkClusters[this.clrIdx].GetChunkSync(World.toChunkXZ(this.blockPos.x), this.blockPos.y, World.toChunkXZ(this.blockPos.z));
			this.setChunkModified(chunkModified);
			base.RefreshBindings(false);
		}
	}

	// Token: 0x060074A2 RID: 29858 RVA: 0x002F727C File Offset: 0x002F547C
	[PublicizedFrom(EAccessModifier.Private)]
	public void triggerOperation_OnPressed(XUiController controller, int button)
	{
		if (this.blockTrigger != null)
		{
			this.blockTrigger.UseOrForMultipleTriggers = !this.blockTrigger.UseOrForMultipleTriggers;
			Chunk chunkModified = (Chunk)GameManager.Instance.World.ChunkClusters[this.clrIdx].GetChunkSync(World.toChunkXZ(this.blockPos.x), this.blockPos.y, World.toChunkXZ(this.blockPos.z));
			this.setChunkModified(chunkModified);
			base.RefreshBindings(false);
		}
	}

	// Token: 0x060074A3 RID: 29859 RVA: 0x002F7308 File Offset: 0x002F5508
	[PublicizedFrom(EAccessModifier.Private)]
	public void triggerUnlock_OnPressed(XUiController controller, int button)
	{
		if (this.blockTrigger != null)
		{
			this.blockTrigger.Unlock = !this.blockTrigger.Unlock;
			Chunk chunkModified = (Chunk)GameManager.Instance.World.ChunkClusters[this.clrIdx].GetChunkSync(World.toChunkXZ(this.blockPos.x), this.blockPos.y, World.toChunkXZ(this.blockPos.z));
			this.setChunkModified(chunkModified);
			base.RefreshBindings(false);
		}
	}

	// Token: 0x060074A4 RID: 29860 RVA: 0x002F7394 File Offset: 0x002F5594
	[PublicizedFrom(EAccessModifier.Private)]
	public void setChunkModified(Chunk _chunk)
	{
		PrefabEditModeManager.Instance.NeedsSaving = true;
		_chunk.isModified = true;
	}

	// Token: 0x060074A5 RID: 29861 RVA: 0x002F73A8 File Offset: 0x002F55A8
	public override void Cleanup()
	{
		base.Cleanup();
		if (SelectionBoxManager.Instance != null)
		{
			SelectionBoxManager.Instance.GetCategory("TriggerVolume").SetCallback(null);
		}
	}

	// Token: 0x060074A6 RID: 29862 RVA: 0x002F73D2 File Offset: 0x002F55D2
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleAddTriggersEntry(XUiController _sender, int _mouseButton)
	{
		this.TriggerOnAddTriggersPressed();
	}

	// Token: 0x060074A7 RID: 29863 RVA: 0x002F73D2 File Offset: 0x002F55D2
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleAddTriggeredByEntry(XUiController _sender, int _mouseButton)
	{
		this.TriggerOnAddTriggersPressed();
	}

	// Token: 0x060074A8 RID: 29864 RVA: 0x002F73DC File Offset: 0x002F55DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnOpenInEditor_OnOnPressed(XUiController _sender, int _mouseButton)
	{
		Process.Start(this.prefab.location.FullPathNoExtension + ".xml");
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
	}

	// Token: 0x060074A9 RID: 29865 RVA: 0x002F7429 File Offset: 0x002F5629
	[PublicizedFrom(EAccessModifier.Private)]
	public void TriggerOnAddTriggersPressed()
	{
		this.prefab.AddNewTriggerLayer();
		this.triggersList.RebuildList(false);
		this.triggeredByList.RebuildList(false);
	}

	// Token: 0x060074AA RID: 29866 RVA: 0x002F744E File Offset: 0x002F564E
	[PublicizedFrom(EAccessModifier.Private)]
	public bool validTriggerName(byte val)
	{
		return !this.prefab.TriggerLayers.Contains(val);
	}

	// Token: 0x060074AB RID: 29867 RVA: 0x002F7464 File Offset: 0x002F5664
	[PublicizedFrom(EAccessModifier.Private)]
	public void TriggersList_SelectionChanged(XUiC_ListEntry<XUiC_PrefabTriggerEditorList.PrefabTriggerEntry> _previousEntry, XUiC_ListEntry<XUiC_PrefabTriggerEditorList.PrefabTriggerEntry> _newEntry)
	{
		if (_newEntry != null)
		{
			byte triggerLayer = 0;
			if (StringParsers.TryParseUInt8(_newEntry.GetEntry().name, out triggerLayer, 0, -1, NumberStyles.Integer))
			{
				if (this.triggerVolume != null)
				{
					this.HandleTriggersSetting(triggerLayer, true, GameManager.Instance.World);
				}
				else
				{
					this.HandleTriggersSetting(triggerLayer, true, GameManager.Instance.World, this.clrIdx, this.blockPos);
				}
			}
			_newEntry.IsDirty = true;
		}
	}

	// Token: 0x060074AC RID: 29868 RVA: 0x002F74D0 File Offset: 0x002F56D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void TriggeredByList_SelectionChanged(XUiC_ListEntry<XUiC_PrefabTriggerEditorList.PrefabTriggerEntry> _previousEntry, XUiC_ListEntry<XUiC_PrefabTriggerEditorList.PrefabTriggerEntry> _newEntry)
	{
		if (_newEntry != null)
		{
			byte triggerLayer = 0;
			if (StringParsers.TryParseUInt8(_newEntry.GetEntry().name, out triggerLayer, 0, -1, NumberStyles.Integer))
			{
				if (this.triggerVolume != null)
				{
					this.HandleTriggersSetting(triggerLayer, false, GameManager.Instance.World);
				}
				else
				{
					this.HandleTriggersSetting(triggerLayer, false, GameManager.Instance.World, this.clrIdx, this.blockPos);
				}
			}
			_newEntry.IsDirty = true;
		}
	}

	// Token: 0x060074AD RID: 29869 RVA: 0x00282536 File Offset: 0x00280736
	public override void OnOpen()
	{
		base.OnOpen();
		this.IsDirty = true;
	}

	// Token: 0x060074AE RID: 29870 RVA: 0x002F753A File Offset: 0x002F573A
	public override void OnClose()
	{
		base.OnClose();
		this.prefab = null;
	}

	// Token: 0x060074AF RID: 29871 RVA: 0x0007FB71 File Offset: 0x0007DD71
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
	}

	// Token: 0x060074B0 RID: 29872 RVA: 0x002F754C File Offset: 0x002F574C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
		if (num <= 1452740656U)
		{
			if (num != 930054621U)
			{
				if (num != 1012116031U)
				{
					if (num == 1452740656U)
					{
						if (_bindingName == "excludeTickmarkSelected")
						{
							_value = ((this.blockTrigger != null) ? this.blockTrigger.ExcludeIcon.ToString() : "false");
							return true;
						}
					}
				}
				else if (_bindingName == "operationTickmarkSelected")
				{
					_value = ((this.blockTrigger != null) ? this.blockTrigger.UseOrForMultipleTriggers.ToString() : "false");
					return true;
				}
			}
			else if (_bindingName == "triggeredby_enabled")
			{
				_value = this.ShowTriggeredBy.ToString();
				return true;
			}
		}
		else if (num <= 2194844717U)
		{
			if (num != 1868194796U)
			{
				if (num == 2194844717U)
				{
					if (_bindingName == "window_height")
					{
						_value = ((this.ShowTriggeredBy && this.ShowTriggers) ? "752" : "396");
						return true;
					}
				}
			}
			else if (_bindingName == "unlockTickmarkSelected")
			{
				_value = ((this.blockTrigger != null) ? this.blockTrigger.Unlock.ToString() : "false");
				return true;
			}
		}
		else if (num != 2556802313U)
		{
			if (num == 3933558790U)
			{
				if (_bindingName == "triggers_enabled")
				{
					_value = this.ShowTriggers.ToString();
					return true;
				}
			}
		}
		else if (_bindingName == "title")
		{
			_value = Localization.Get("xuiPrefabProperties", false) + ": " + ((this.prefab != null) ? this.prefab.PrefabName : "-");
			return true;
		}
		return false;
	}

	// Token: 0x060074B1 RID: 29873 RVA: 0x002F7730 File Offset: 0x002F5930
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupTrigger()
	{
		Chunk chunk = (Chunk)GameManager.Instance.World.ChunkClusters[this.clrIdx].GetChunkSync(World.toChunkXZ(this.blockPos.x), this.blockPos.y, World.toChunkXZ(this.blockPos.z));
		this.blockTrigger = chunk.GetBlockTrigger(World.toBlock(this.blockPos));
	}

	// Token: 0x060074B2 RID: 29874 RVA: 0x002F77A4 File Offset: 0x002F59A4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleTriggersSetting(byte triggerLayer, bool isTriggers, World _world, int _cIdx, Vector3i _blockPos)
	{
		if (_world.IsEditor())
		{
			Chunk chunk = (Chunk)_world.ChunkClusters[_cIdx].GetChunkSync(World.toChunkXZ(_blockPos.x), _blockPos.y, World.toChunkXZ(_blockPos.z));
			this.blockTrigger = chunk.GetBlockTrigger(World.toBlock(_blockPos));
			if (triggerLayer == 0)
			{
				if (this.blockTrigger != null)
				{
					chunk.RemoveBlockTrigger(this.blockTrigger);
					this.blockTrigger = null;
				}
			}
			else
			{
				if (this.blockTrigger == null)
				{
					this.blockTrigger = new BlockTrigger(chunk);
					if (isTriggers)
					{
						if (this.blockTrigger.HasTriggers(triggerLayer))
						{
							this.blockTrigger.RemoveTriggersFlag(triggerLayer);
						}
						else
						{
							this.blockTrigger.SetTriggersFlag(triggerLayer);
						}
					}
					else if (this.blockTrigger.HasTriggeredBy(triggerLayer))
					{
						this.blockTrigger.RemoveTriggeredByFlag(triggerLayer);
					}
					else
					{
						this.blockTrigger.SetTriggeredByFlag(triggerLayer);
					}
					this.blockTrigger.LocalChunkPos = World.toBlock(_blockPos);
					chunk.AddBlockTrigger(this.blockTrigger);
				}
				else if (isTriggers)
				{
					if (this.blockTrigger.HasTriggers(triggerLayer))
					{
						this.blockTrigger.RemoveTriggersFlag(triggerLayer);
					}
					else
					{
						this.blockTrigger.SetTriggersFlag(triggerLayer);
					}
				}
				else if (this.blockTrigger.HasTriggeredBy(triggerLayer))
				{
					this.blockTrigger.RemoveTriggeredByFlag(triggerLayer);
				}
				else
				{
					this.blockTrigger.SetTriggeredByFlag(triggerLayer);
				}
				if (!this.blockTrigger.HasAnyTriggers() && !this.blockTrigger.HasAnyTriggeredBy() && this.blockTrigger != null)
				{
					chunk.RemoveBlockTrigger(this.blockTrigger);
					this.blockTrigger = null;
				}
				this.setChunkModified(chunk);
			}
			if (this.blockTrigger != null)
			{
				this.blockTrigger.TriggerUpdated(null);
			}
		}
	}

	// Token: 0x060074B3 RID: 29875 RVA: 0x002F7960 File Offset: 0x002F5B60
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleTriggersSetting(byte triggerLayer, bool isTriggers, World _world)
	{
		if (_world.IsEditor())
		{
			if (isTriggers)
			{
				if (this.triggerVolume.HasTriggers(triggerLayer))
				{
					this.triggerVolume.RemoveTriggersFlag(triggerLayer);
				}
				else
				{
					this.triggerVolume.SetTriggersFlag(triggerLayer);
				}
			}
			if (this.blockTrigger != null)
			{
				this.blockTrigger.TriggerUpdated(null);
			}
		}
	}

	// Token: 0x060074B4 RID: 29876 RVA: 0x002F79B4 File Offset: 0x002F5BB4
	public static void Show(XUi _xui, int _clrIdx, Vector3i _blockPos, bool _showTriggers, bool _showTriggeredBy)
	{
		XUiC_TriggerProperties childByType = ((XUiWindowGroup)_xui.playerUI.windowManager.GetWindow(XUiC_TriggerProperties.ID)).Controller.GetChildByType<XUiC_TriggerProperties>();
		childByType.Prefab = PrefabEditModeManager.Instance.VoxelPrefab;
		childByType.clrIdx = _clrIdx;
		childByType.BlockPos = _blockPos;
		childByType.ShowTriggers = _showTriggers;
		childByType.ShowTriggeredBy = _showTriggeredBy;
		childByType.RefreshBindings(false);
		_xui.playerUI.windowManager.Open(XUiC_TriggerProperties.ID, true, false, true);
	}

	// Token: 0x060074B5 RID: 29877 RVA: 0x002F7A30 File Offset: 0x002F5C30
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
			this.selectedPrefabInstance = null;
		}
		return true;
	}

	// Token: 0x060074B6 RID: 29878 RVA: 0x002F7A60 File Offset: 0x002F5C60
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
				this.selectedPrefabInstance = PrefabSleeperVolumeManager.Instance.GetPrefabInstance(_prefabInstanceId);
				this.Prefab = this.selectedPrefabInstance.prefab;
				return true;
			}
		}
		return false;
	}

	// Token: 0x060074B7 RID: 29879 RVA: 0x002F7AD4 File Offset: 0x002F5CD4
	public void OnSelectionBoxMoved(string _category, string _name, Vector3 _moveVector)
	{
		if (this.selectedPrefabInstance == null)
		{
			return;
		}
		Prefab.PrefabTriggerVolume prefabTriggerVolume = new Prefab.PrefabTriggerVolume(this.selectedPrefabInstance.prefab.TriggerVolumes[this.selIdx]);
		prefabTriggerVolume.startPos += new Vector3i(_moveVector);
		PrefabTriggerVolumeManager.Instance.UpdateTriggerPropertiesServer(this.selectedPrefabInstance.id, this.selIdx, prefabTriggerVolume, false);
	}

	// Token: 0x060074B8 RID: 29880 RVA: 0x002F7B40 File Offset: 0x002F5D40
	public void OnSelectionBoxSized(string _category, string _name, int _dTop, int _dBottom, int _dNorth, int _dSouth, int _dEast, int _dWest)
	{
		if (this.selectedPrefabInstance == null)
		{
			return;
		}
		Prefab.PrefabTriggerVolume prefabTriggerVolume = new Prefab.PrefabTriggerVolume(this.selectedPrefabInstance.prefab.TriggerVolumes[this.selIdx]);
		prefabTriggerVolume.size += new Vector3i(_dEast + _dWest, _dTop + _dBottom, _dNorth + _dSouth);
		prefabTriggerVolume.startPos += new Vector3i(-_dWest, -_dBottom, -_dSouth);
		Vector3i size = prefabTriggerVolume.size;
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
		prefabTriggerVolume.size = size;
		PrefabTriggerVolumeManager.Instance.UpdateTriggerPropertiesServer(this.selectedPrefabInstance.id, this.selIdx, prefabTriggerVolume, false);
	}

	// Token: 0x060074B9 RID: 29881 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnSelectionBoxMirrored(Vector3i _axis)
	{
	}

	// Token: 0x060074BA RID: 29882 RVA: 0x002F7C3C File Offset: 0x002F5E3C
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
			Prefab.PrefabTriggerVolume volumeSettings = new Prefab.PrefabTriggerVolume(this.selectedPrefabInstance.prefab.TriggerVolumes[num2]);
			PrefabTriggerVolumeManager.Instance.UpdateTriggerPropertiesServer(this.selectedPrefabInstance.id, num2, volumeSettings, true);
			return true;
		}
		return false;
	}

	// Token: 0x060074BB RID: 29883 RVA: 0x002F7CE8 File Offset: 0x002F5EE8
	public bool OnSelectionBoxIsAvailable(string _category, EnumSelectionBoxAvailabilities _criteria)
	{
		return _criteria == EnumSelectionBoxAvailabilities.CanShowProperties || _criteria == EnumSelectionBoxAvailabilities.CanResize;
	}

	// Token: 0x060074BC RID: 29884 RVA: 0x002F7CF4 File Offset: 0x002F5EF4
	public void OnSelectionBoxShowProperties(bool _bVisible, GUIWindowManager _windowManager)
	{
		string text;
		string name;
		int num;
		int index;
		if (SelectionBoxManager.Instance.GetSelected(out text, out name) && text.Equals("TriggerVolume") && this.getPrefabIdAndVolumeId(name, out num, out index))
		{
			Prefab.PrefabTriggerVolume prefabTriggerVolume = this.selectedPrefabInstance.prefab.TriggerVolumes[index];
			this.ShowTriggers = true;
			this.ShowTriggeredBy = false;
			this.TriggerVolume = prefabTriggerVolume;
			_windowManager.SwitchVisible(XUiC_TriggerProperties.ID, false, true);
		}
	}

	// Token: 0x060074BD RID: 29885 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnSelectionBoxRotated(string _category, string _name)
	{
	}

	// Token: 0x040058BE RID: 22718
	public static string ID = "";

	// Token: 0x040058BF RID: 22719
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PrefabTriggerEditorList triggersList;

	// Token: 0x040058C0 RID: 22720
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PrefabTriggerEditorList triggeredByList;

	// Token: 0x040058C1 RID: 22721
	[PublicizedFrom(EAccessModifier.Private)]
	public Prefab prefab;

	// Token: 0x040058C2 RID: 22722
	[PublicizedFrom(EAccessModifier.Private)]
	public int clrIdx;

	// Token: 0x040058C3 RID: 22723
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i blockPos;

	// Token: 0x040058C4 RID: 22724
	public BlockTrigger blockTrigger;

	// Token: 0x040058C5 RID: 22725
	[PublicizedFrom(EAccessModifier.Private)]
	public Prefab.PrefabTriggerVolume triggerVolume;

	// Token: 0x040058C6 RID: 22726
	public bool ShowTriggers = true;

	// Token: 0x040058C7 RID: 22727
	public bool ShowTriggeredBy = true;

	// Token: 0x040058C8 RID: 22728
	[PublicizedFrom(EAccessModifier.Private)]
	public int selIdx;

	// Token: 0x040058C9 RID: 22729
	[PublicizedFrom(EAccessModifier.Private)]
	public PrefabInstance selectedPrefabInstance;

	// Token: 0x040058CA RID: 22730
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<string> prefabGroupsList = new List<string>();
}
