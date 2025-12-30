using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;
using WorldGenerationEngineFinal;

// Token: 0x02000EB7 RID: 3767
[Preserve]
public class XUiC_WorldGenerationWindowGroup : XUiController
{
	// Token: 0x140000D3 RID: 211
	// (add) Token: 0x06007736 RID: 30518 RVA: 0x003089E8 File Offset: 0x00306BE8
	// (remove) Token: 0x06007737 RID: 30519 RVA: 0x00308A20 File Offset: 0x00306C20
	public event Action OnCountyNameChanged;

	// Token: 0x140000D4 RID: 212
	// (add) Token: 0x06007738 RID: 30520 RVA: 0x00308A58 File Offset: 0x00306C58
	// (remove) Token: 0x06007739 RID: 30521 RVA: 0x00308A90 File Offset: 0x00306C90
	public event Action OnWorldSizeChanged;

	// Token: 0x17000C1A RID: 3098
	// (get) Token: 0x0600773A RID: 30522 RVA: 0x00308AC5 File Offset: 0x00306CC5
	public bool HasSufficientSpace
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return !SaveInfoProvider.DataLimitEnabled || this.m_pendingBytes <= this.m_totalAvailableBytes;
		}
	}

	// Token: 0x0600773B RID: 30523 RVA: 0x00308AE4 File Offset: 0x00306CE4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "showbar")
		{
			_value = this.dataManagementBarEnabled.ToString();
			return true;
		}
		if (!(_bindingName == "canNewGame"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		bool flag;
		if (!this.isGenerating)
		{
			WorldBuilder worldBuilder = this.worldBuilder;
			if (worldBuilder != null && worldBuilder.IsFinished && !worldBuilder.IsCanceled && this.worldBuilder.CanSaveData())
			{
				flag = this.HasSufficientSpace;
				goto IL_64;
			}
		}
		flag = false;
		IL_64:
		bool flag2 = flag;
		_value = flag2.ToString();
		return true;
	}

	// Token: 0x0600773C RID: 30524 RVA: 0x00308B68 File Offset: 0x00306D68
	public override void Init()
	{
		base.Init();
		this.PreviewWindow = base.GetChildByType<XUiC_WorldGenerationPreview>();
		this.SeedInput = base.GetChildByType<XUiC_TextInput>();
		this.GenerateButton = (base.GetChildById("generate") as XUiC_SimpleButton);
		this.TerrainAndBiomeOnly = (base.GetChildById("cbxTerrainAndBiomeOnly") as XUiC_ComboBoxBool);
		this.BackButton = (base.GetChildById("btnBack") as XUiC_SimpleButton);
		this.NewGameButton = (base.GetChildById("btnNewGame") as XUiC_SimpleButton);
		this.TerrainAndBiomeOnly = (base.GetChildById("cbxTerrainAndBiomeOnly") as XUiC_ComboBoxBool);
		if (base.GetChildById("countyName") != null)
		{
			this.CountyNameLabel = (base.GetChildById("countyName").ViewComponent as XUiV_Label);
		}
		this.btnManage = (base.GetChildById("btnDataManagement") as XUiC_SimpleButton);
		this.dataManagementBar = (base.GetChildById("data_bar_controller") as XUiC_DataManagementBar);
		this.dataManagementBarEnabled = (this.dataManagementBar != null && SaveInfoProvider.DataLimitEnabled);
	}

	// Token: 0x0600773D RID: 30525 RVA: 0x00308C6C File Offset: 0x00306E6C
	public static bool IsGenerating()
	{
		XUiC_WorldGenerationWindowGroup instance = XUiC_WorldGenerationWindowGroup.Instance;
		WorldBuilder worldBuilder = (instance != null) ? instance.worldBuilder : null;
		return worldBuilder != null && !worldBuilder.IsFinished;
	}

	// Token: 0x0600773E RID: 30526 RVA: 0x00308C9C File Offset: 0x00306E9C
	public static void CancelGeneration()
	{
		XUiC_WorldGenerationWindowGroup instance = XUiC_WorldGenerationWindowGroup.Instance;
		WorldBuilder worldBuilder = (instance != null) ? instance.worldBuilder : null;
		if (worldBuilder != null)
		{
			worldBuilder.IsCanceled = true;
		}
	}

	// Token: 0x0600773F RID: 30527 RVA: 0x00308CC5 File Offset: 0x00306EC5
	[PublicizedFrom(EAccessModifier.Private)]
	public void NewGameButton_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.StartCoroutine(this.SaveAndNewGameCo());
	}

	// Token: 0x06007740 RID: 30528 RVA: 0x00308CD9 File Offset: 0x00306ED9
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator SaveAndNewGameCo()
	{
		if (this.isClosing)
		{
			yield break;
		}
		this.isClosing = true;
		bool shouldClose = false;
		WorldBuilder worldBuilder = this.worldBuilder;
		bool canPrompt = true;
		XUiV_Window parentWindow = base.GetParentWindow();
		yield return worldBuilder.SaveData(canPrompt, ((parentWindow != null) ? parentWindow.Controller : null) ?? this, true, delegate
		{
			shouldClose = false;
		}, null, delegate
		{
			shouldClose = true;
		});
		if (!shouldClose)
		{
			this.isClosing = false;
			yield break;
		}
		XUiC_NewContinueGame.SetIsContinueGame(base.xui, false);
		GamePrefs.Set(EnumGamePrefs.GameWorld, this.CountyName);
		this.CheckProfile(XUiC_NewContinueGame.ID);
		yield break;
	}

	// Token: 0x06007741 RID: 30529 RVA: 0x00308CE8 File Offset: 0x00306EE8
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckProfile(string _windowToOpen)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		if (ProfileSDF.CurrentProfileName().Length == 0)
		{
			XUiC_OptionsProfiles.Open(base.xui, delegate
			{
				this.xui.playerUI.windowManager.Open(_windowToOpen, true, false, true);
			});
			return;
		}
		base.xui.playerUI.windowManager.Open(_windowToOpen, true, false, true);
	}

	// Token: 0x06007742 RID: 30530 RVA: 0x00308D6B File Offset: 0x00306F6B
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnBack_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.StartClose();
	}

	// Token: 0x06007743 RID: 30531 RVA: 0x00308D74 File Offset: 0x00306F74
	public override void OnOpen()
	{
		XUiC_WorldGenerationWindowGroup.Instance = this;
		base.OnOpen();
		this.isAdvancedUI = (this.windowGroup.ID == "rwgeditor");
		if (this.isAdvancedUI)
		{
			this.windowGroup.isEscClosable = false;
		}
		this.isClosing = false;
		if (!base.xui.playerUI.windowManager.IsWindowOpen(XUiC_NewContinueGame.ID))
		{
			base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.RWGEditor, 0f);
		}
		PathAbstractions.CacheEnabled = true;
		if (this.PreviewWindow != null)
		{
			this.prefabPreviewManager = new PrefabPreviewManager();
		}
		if ((this.WorldSizeComboBox = (base.GetChildById("WorldSize") as XUiC_ComboBoxList<int>)) != null)
		{
			if (PlatformOptimizations.EnforceMaxWorldSizeHost)
			{
				int num = this.WorldSizeComboBox.Elements.FindLastIndex((int element) => element <= PlatformOptimizations.MaxWorldSizeHost);
				if (num >= 0)
				{
					this.WorldSizeComboBox.MinIndex = 0;
					this.WorldSizeComboBox.MaxIndex = num;
					this.WorldSizeComboBox.SelectedIndex = num;
				}
			}
			if (this.WorldSizeComboBox.Elements.Contains(8192))
			{
				this.WorldSizeComboBox.Value = 8192;
			}
		}
		this.SaveDataLimitComboBox = SaveDataLimitUIHelper.AddComboBox(base.GetChildById("SaveDataLimitComboBox") as XUiC_ComboBoxEnum<SaveDataLimitType>);
		if ((this.Rivers = (base.GetChildById("Rivers") as XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections>)) != null)
		{
			this.Rivers.Value = WorldBuilder.GenerationSelections.Default;
		}
		if ((this.Craters = (base.GetChildById("Craters") as XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections>)) != null)
		{
			this.Craters.Value = WorldBuilder.GenerationSelections.Default;
		}
		if ((this.Canyons = (base.GetChildById("Cracks") as XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections>)) != null)
		{
			this.Canyons.Value = WorldBuilder.GenerationSelections.Default;
		}
		if ((this.Lakes = (base.GetChildById("Lakes") as XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections>)) != null)
		{
			this.Lakes.Value = WorldBuilder.GenerationSelections.Default;
		}
		if ((this.Rural = (base.GetChildById("Rural") as XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections>)) != null)
		{
			this.Rural.Value = WorldBuilder.GenerationSelections.Default;
		}
		if ((this.Town = (base.GetChildById("Town") as XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections>)) != null)
		{
			this.Town.Value = WorldBuilder.GenerationSelections.Default;
		}
		if ((this.City = (base.GetChildById("City") as XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections>)) != null)
		{
			this.City.Value = WorldBuilder.GenerationSelections.Default;
		}
		if ((this.Towns = (base.GetChildById("Towns") as XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections>)) != null)
		{
			this.Towns.Value = WorldBuilder.GenerationSelections.Default;
		}
		if ((this.Wilderness = (base.GetChildById("Wilderness") as XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections>)) != null)
		{
			this.Wilderness.Value = WorldBuilder.GenerationSelections.Default;
		}
		if ((this.PlainsWeight = (base.GetChildById("PlainsWeight") as XUiC_ComboBoxInt)) != null)
		{
			this.PlainsWeight.Value = 4L;
			this.PlainsWeight.OnValueChanged += this.PlainsWeight_OnValueChanged;
		}
		if ((this.HillsWeight = (base.GetChildById("HillsWeight") as XUiC_ComboBoxInt)) != null)
		{
			this.HillsWeight.Value = 4L;
			this.HillsWeight.OnValueChanged += this.HillsWeight_OnValueChanged;
		}
		if ((this.MountainsWeight = (base.GetChildById("MountainsWeight") as XUiC_ComboBoxInt)) != null)
		{
			this.MountainsWeight.Value = 2L;
			this.MountainsWeight.OnValueChanged += this.MountainsWeight_OnValueChanged;
		}
		if ((this.BiomeLayoutComboBox = (base.GetChildById("BiomeLayout") as XUiC_ComboBoxEnum<WorldBuilder.BiomeLayout>)) != null)
		{
			this.BiomeLayoutComboBox.Value = WorldBuilder.BiomeLayout.CenterForest;
		}
		XUiController childById = base.GetChildById("biomes");
		if (childById != null)
		{
			for (int i = 0; i < 5; i++)
			{
				XUiController xuiController = childById.Children[i];
				XUiController childById2 = xuiController.GetChildById("label");
				if (childById2 != null)
				{
					((XUiV_Label)childById2.ViewComponent).Text = Localization.Get(this.BiomeToUIName[i], false);
				}
				XUiC_ComboBoxInt childByType = xuiController.GetChildByType<XUiC_ComboBoxInt>();
				this.biomeComboBoxes[i] = childByType;
				if (childByType != null)
				{
					childByType.Value = (long)WorldBuilderConstants.BiomeWeightDefaults[i];
					childByType.OnValueChanged += this.BiomeWeight_OnValueChanged;
				}
				XUiController childById3 = xuiController.GetChildById("color");
				if (childById3 != null)
				{
					XUiV_Sprite xuiV_Sprite = (XUiV_Sprite)childById3.ViewComponent;
					Color color = WorldBuilderConstants.biomeColorList[i] * 0.7f;
					color.a = 1f;
					xuiV_Sprite.Color = color;
				}
			}
		}
		this.updateTerrainPercentages(false);
		this.updateBiomePercentages();
		if (this.BackButton != null)
		{
			this.BackButton.OnPressed += this.BtnBack_OnPressed;
		}
		if (this.GenerateButton != null)
		{
			this.GenerateButton.OnPressed += this.GenerateButton_OnPressed;
		}
		if (this.NewGameButton != null)
		{
			this.NewGameButton.OnPressed += this.NewGameButton_OnPressed;
		}
		if (this.btnManage != null)
		{
			this.btnManage.OnPressed += this.BtnManage_OnPressed;
		}
		if ((this.Quality = (base.GetChildById("PreviewQuality") as XUiC_ComboBoxEnum<XUiC_WorldGenerationWindowGroup.PreviewQuality>)) != null)
		{
			this.Quality.SetMinMax(EnumUtils.MinValue<XUiC_WorldGenerationWindowGroup.PreviewQuality>(), PlatformOptimizations.MaxRWGPreviewQuality);
			this.Quality.Value = (XUiC_WorldGenerationWindowGroup.PreviewQuality)Math.Min(3, (int)this.Quality.Max);
			this.Quality.OnValueChanged += this.Quality_OnValueChanged;
		}
		if (this.SeedInput != null)
		{
			this.SeedInput.OnChangeHandler += this.SeedInput_OnChangeHandler;
			this.SeedInput_OnChangeHandler(this.SeedInput, this.SeedInput.Text, true);
		}
		if (this.WorldSizeComboBox != null)
		{
			this.WorldSizeComboBox.OnValueChanged += this.WorldSizeComboBox_OnValueChanged;
			this.WorldSizeComboBox_OnValueChanged(this.WorldSizeComboBox, this.WorldSizeComboBox.Value - 1, this.WorldSizeComboBox.Value);
		}
		this.oldFogDensity = RenderSettings.fogDensity;
		RenderSettings.fogDensity = 0f;
		this.UpdateBarValues();
	}

	// Token: 0x06007744 RID: 30532 RVA: 0x00309388 File Offset: 0x00307588
	[PublicizedFrom(EAccessModifier.Private)]
	public void Quality_OnValueChanged(XUiController _sender, XUiC_WorldGenerationWindowGroup.PreviewQuality _oldValue, XUiC_WorldGenerationWindowGroup.PreviewQuality _newValue)
	{
		this.PreviewQualityLevel = _newValue;
		if (XUiC_WorldGenerationPreview.Instance != null)
		{
			XUiC_WorldGenerationPreview.Instance.GeneratePreview();
		}
		if (this.prefabPreviewManager != null && this.prefabPreviewManager.initialized && (_oldValue < XUiC_WorldGenerationWindowGroup.PreviewQuality.Low || _oldValue > XUiC_WorldGenerationWindowGroup.PreviewQuality.High || _newValue < XUiC_WorldGenerationWindowGroup.PreviewQuality.Low || _newValue > XUiC_WorldGenerationWindowGroup.PreviewQuality.High))
		{
			this.prefabPreviewManager.RemovePrefabs();
			this.prefabPreviewManager.InitPrefabs();
			this.prefabPreviewManager.ForceUpdate();
		}
	}

	// Token: 0x06007745 RID: 30533 RVA: 0x003093FB File Offset: 0x003075FB
	[PublicizedFrom(EAccessModifier.Private)]
	public void BiomeWeight_OnValueChanged(XUiController _sender, long _oldValue, long _newValue)
	{
		this.updateBiomePercentages();
	}

	// Token: 0x06007746 RID: 30534 RVA: 0x00309403 File Offset: 0x00307603
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlainsWeight_OnValueChanged(XUiController _sender, long _oldValue, long _newValue)
	{
		this.updateTerrainPercentages(false);
	}

	// Token: 0x06007747 RID: 30535 RVA: 0x00309403 File Offset: 0x00307603
	[PublicizedFrom(EAccessModifier.Private)]
	public void HillsWeight_OnValueChanged(XUiController _sender, long _oldValue, long _newValue)
	{
		this.updateTerrainPercentages(false);
	}

	// Token: 0x06007748 RID: 30536 RVA: 0x0030940C File Offset: 0x0030760C
	[PublicizedFrom(EAccessModifier.Private)]
	public void MountainsWeight_OnValueChanged(XUiController _sender, long _oldValue, long _newValue)
	{
		this.updateTerrainPercentages(true);
	}

	// Token: 0x06007749 RID: 30537 RVA: 0x00309418 File Offset: 0x00307618
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateTerrainPercentages(bool _isMountainsChanged = false)
	{
		if (this.PlainsWeight == null || this.HillsWeight == null || this.MountainsWeight == null)
		{
			return;
		}
		int num2;
		int num3;
		int num4;
		for (;;)
		{
			float num = (float)(this.PlainsWeight.Value + this.HillsWeight.Value + this.MountainsWeight.Value);
			if (num <= 0f)
			{
				num = 1f;
			}
			num2 = Mathf.RoundToInt((float)this.PlainsWeight.Value / num * 100f);
			num3 = Mathf.RoundToInt((float)this.HillsWeight.Value / num * 100f);
			num4 = Mathf.RoundToInt((float)this.MountainsWeight.Value / num * 100f);
			if (num4 <= 50)
			{
				break;
			}
			if (_isMountainsChanged)
			{
				this.PlainsWeight.Value += 1L;
			}
			else
			{
				this.MountainsWeight.Value -= 1L;
			}
		}
		if (num2 + num3 + num4 == 0)
		{
			num2 = 100;
		}
		this.PlainsWeight.UpdateLabel(string.Format("{0}%", Mathf.Max(0, num2)));
		this.HillsWeight.UpdateLabel(string.Format("{0}%", Mathf.Max(0, num3)));
		this.MountainsWeight.UpdateLabel(string.Format("{0}%", Mathf.Max(0, num4)));
		this.PlainsWeight.IsDirty = true;
		this.HillsWeight.IsDirty = true;
		this.MountainsWeight.IsDirty = true;
	}

	// Token: 0x0600774A RID: 30538 RVA: 0x0030958C File Offset: 0x0030778C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateBiomePercentages()
	{
		float num = 0f;
		float num2 = 0f;
		int num3 = 0;
		for (int i = 0; i < 5; i++)
		{
			XUiC_ComboBoxInt xuiC_ComboBoxInt = this.biomeComboBoxes[i];
			if (xuiC_ComboBoxInt == null)
			{
				return;
			}
			float num4 = (float)xuiC_ComboBoxInt.Value;
			num += num4;
			if (num4 > num2)
			{
				num2 = num4;
				num3 = i;
			}
		}
		int num5 = 0;
		for (int j = 0; j < 5; j++)
		{
			XUiC_ComboBoxInt xuiC_ComboBoxInt2 = this.biomeComboBoxes[j];
			int num6 = Mathf.RoundToInt((float)xuiC_ComboBoxInt2.Value / num * 100f);
			num6 = Utils.FastMax(5, num6);
			num5 += num6;
			if (j == 4)
			{
				num6 += 100 - num5;
				if (num6 < 5)
				{
					XUiC_ComboBoxInt xuiC_ComboBoxInt3 = this.biomeComboBoxes[num3];
					int num7 = Mathf.RoundToInt((float)xuiC_ComboBoxInt3.Value / num * 100f);
					xuiC_ComboBoxInt3.UpdateLabel(string.Format("{0}%", num7 + 5 - num6));
					num6 = 5;
				}
			}
			xuiC_ComboBoxInt2.UpdateLabel(string.Format("{0}%", num6));
			xuiC_ComboBoxInt2.IsDirty = true;
		}
	}

	// Token: 0x0600774B RID: 30539 RVA: 0x00309693 File Offset: 0x00307893
	[PublicizedFrom(EAccessModifier.Private)]
	public void WorldSizeComboBox_OnValueChanged(XUiController _sender, int _oldValue, int _newValue)
	{
		this.WorldSize = _newValue;
		this.RefreshCountyName();
		Action onWorldSizeChanged = this.OnWorldSizeChanged;
		if (onWorldSizeChanged == null)
		{
			return;
		}
		onWorldSizeChanged();
	}

	// Token: 0x0600774C RID: 30540 RVA: 0x003096B2 File Offset: 0x003078B2
	[PublicizedFrom(EAccessModifier.Private)]
	public void SeedInput_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		this.RefreshCountyName();
	}

	// Token: 0x0600774D RID: 30541 RVA: 0x003096BC File Offset: 0x003078BC
	public void RefreshCountyName()
	{
		this.CountyName = WorldBuilder.GetGeneratedWorldName(this.SeedInput.Text, this.WorldSize);
		this.ValidateNewRwg();
		if (this.CountyNameLabel != null)
		{
			this.CountyNameLabel.Text = this.CountyName;
		}
		this.TriggerCountyNameChangedEvent();
	}

	// Token: 0x0600774E RID: 30542 RVA: 0x0030970C File Offset: 0x0030790C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ValidateNewRwg()
	{
		string countyName = this.CountyName;
		bool flag = PathAbstractions.WorldsSearchPaths.GetLocation(countyName, countyName, null).Type != PathAbstractions.EAbstractedLocationType.None;
		this.ValidCountyName = !flag;
		if (this.CountyNameLabel != null)
		{
			this.CountyNameLabel.Color = (this.ValidCountyName ? Color.white : Color.red);
			if (flag)
			{
				this.CountyNameLabel.ToolTip = Localization.Get("mmLblRwgSeedErrorWorldExists", false);
				return;
			}
			this.CountyNameLabel.ToolTip = "";
		}
	}

	// Token: 0x0600774F RID: 30543 RVA: 0x00309794 File Offset: 0x00307994
	[PublicizedFrom(EAccessModifier.Private)]
	public void TriggerCountyNameChangedEvent()
	{
		if (this.OnCountyNameChanged != null)
		{
			this.OnCountyNameChanged();
		}
	}

	// Token: 0x06007750 RID: 30544 RVA: 0x003097A9 File Offset: 0x003079A9
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnManage_OnPressed(XUiController _sender, int _mouseButton)
	{
		XUiC_DataManagement.OpenDataManagementWindow(this, new Action(this.OnDataManagementWindowClosed));
	}

	// Token: 0x06007751 RID: 30545 RVA: 0x003097BD File Offset: 0x003079BD
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDataManagementWindowClosed()
	{
		this.UpdateBarValues();
	}

	// Token: 0x06007752 RID: 30546 RVA: 0x003097C8 File Offset: 0x003079C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateBarValues()
	{
		if (!this.dataManagementBarEnabled)
		{
			base.RefreshBindings(false);
			return;
		}
		SaveInfoProvider instance = SaveInfoProvider.Instance;
		this.dataManagementBar.SetUsedBytes(instance.TotalUsedBytes);
		this.dataManagementBar.SetAllowanceBytes(instance.TotalAllowanceBytes);
		this.dataManagementBar.SetDisplayMode(XUiC_DataManagementBar.DisplayMode.Preview);
		WorldBuilder worldBuilder = this.worldBuilder;
		this.m_pendingBytes = ((worldBuilder != null) ? worldBuilder.SerializedSize : 0L);
		this.m_totalAvailableBytes = instance.TotalAvailableBytes;
		this.dataManagementBar.SetPendingBytes(this.m_pendingBytes);
		base.RefreshBindings(false);
	}

	// Token: 0x06007753 RID: 30547 RVA: 0x00309856 File Offset: 0x00307A56
	[PublicizedFrom(EAccessModifier.Private)]
	public void GenerateButton_OnPressed(XUiController _sender, int _mouseButton)
	{
		if (this.PreviewWindow != null)
		{
			this.PreviewWindow.CleanupTerrainMesh();
		}
		if (this.prefabPreviewManager != null)
		{
			this.prefabPreviewManager.ClearOldPreview();
		}
		ThreadManager.StartCoroutine(this.GenerateCo(true, null, null));
	}

	// Token: 0x06007754 RID: 30548 RVA: 0x0030988D File Offset: 0x00307A8D
	public IEnumerator GenerateCo(bool _usePreviewer = true, Action<string> onSuccess = null, Action onFailure = null)
	{
		this.isGenerating = true;
		this.UpdateBarValues();
		this.DestroyBuilder();
		this.worldBuilder = new WorldBuilder(this.SeedInput.Text, this.WorldSizeComboBox.Value);
		this.worldBuilder.UsePreviewer = _usePreviewer;
		if (this.Towns != null)
		{
			this.worldBuilder.Towns = this.Towns.Value;
		}
		if (this.Wilderness != null)
		{
			this.worldBuilder.Wilderness = this.Wilderness.Value;
		}
		if (this.Rivers != null)
		{
			this.worldBuilder.Rivers = this.Rivers.Value;
		}
		if (this.Craters != null)
		{
			this.worldBuilder.Craters = this.Craters.Value;
		}
		if (this.Canyons != null)
		{
			this.worldBuilder.Canyons = this.Canyons.Value;
		}
		if (this.Lakes != null)
		{
			this.worldBuilder.Lakes = this.Lakes.Value;
		}
		if (this.PlainsWeight != null)
		{
			this.worldBuilder.Plains = (int)this.PlainsWeight.Value;
		}
		if (this.HillsWeight != null)
		{
			this.worldBuilder.Hills = (int)this.HillsWeight.Value;
		}
		if (this.MountainsWeight != null)
		{
			this.worldBuilder.Mountains = (int)this.MountainsWeight.Value;
		}
		if (this.BiomeLayoutComboBox != null)
		{
			this.worldBuilder.biomeLayout = this.BiomeLayoutComboBox.Value;
		}
		for (int i = 0; i < 5; i++)
		{
			XUiC_ComboBoxInt xuiC_ComboBoxInt = this.biomeComboBoxes[i];
			if (xuiC_ComboBoxInt != null)
			{
				this.worldBuilder.SetBiomeWeight((BiomeType)i, (int)xuiC_ComboBoxInt.Value);
			}
		}
		if (this.Quality != null)
		{
			this.PreviewQualityLevel = this.Quality.Value;
		}
		PrefabPreviewManager.ReadyToDisplay = false;
		this.UpdateBarValues();
		yield return GCUtils.WaitForIdle();
		yield return this.worldBuilder.GenerateFromUI();
		if (this.worldBuilder.UsePreviewer)
		{
			yield return this.worldBuilder.FinishForPreview();
			if (XUiC_WorldGenerationPreview.Instance != null)
			{
				XUiC_WorldGenerationPreview.Instance.GeneratePreview();
			}
			if (!this.worldBuilder.IsCanceled)
			{
				yield return new WaitForSeconds(2f);
			}
		}
		else
		{
			XUiC_WorldGenerationWindowGroup.<>c__DisplayClass78_0 CS$<>8__locals1 = new XUiC_WorldGenerationWindowGroup.<>c__DisplayClass78_0();
			XUiC_ProgressWindow.Close(LocalPlayerUI.primaryUI);
			CS$<>8__locals1.success = false;
			WorldBuilder worldBuilder = this.worldBuilder;
			bool canPrompt = true;
			XUiV_Window parentWindow = base.GetParentWindow();
			yield return worldBuilder.SaveData(canPrompt, ((parentWindow != null) ? parentWindow.Controller : null) ?? this, true, null, delegate
			{
				CS$<>8__locals1.success = false;
			}, delegate
			{
				CS$<>8__locals1.success = true;
			});
			if (CS$<>8__locals1.success)
			{
				if (onSuccess != null)
				{
					onSuccess(this.worldBuilder.WorldName);
				}
			}
			else if (onFailure != null)
			{
				onFailure();
			}
			this.DestroyBuilder();
			CS$<>8__locals1 = null;
		}
		XUiC_ProgressWindow.Close(LocalPlayerUI.primaryUI);
		PrefabPreviewManager.ReadyToDisplay = true;
		this.isGenerating = false;
		this.UpdateBarValues();
		yield break;
	}

	// Token: 0x06007755 RID: 30549 RVA: 0x003098B1 File Offset: 0x00307AB1
	[PublicizedFrom(EAccessModifier.Private)]
	public void DestroyBuilder()
	{
		if (this.worldBuilder != null)
		{
			this.worldBuilder.Cleanup();
			this.worldBuilder = null;
		}
	}

	// Token: 0x06007756 RID: 30550 RVA: 0x003098D0 File Offset: 0x00307AD0
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartClose()
	{
		if (this.isGenerating)
		{
			return;
		}
		if (this.worldBuilder == null || !this.worldBuilder.IsFinished || !this.worldBuilder.CanSaveData())
		{
			this.Close();
			return;
		}
		base.xui.StartCoroutine(this.StartCloseCo());
	}

	// Token: 0x06007757 RID: 30551 RVA: 0x00309921 File Offset: 0x00307B21
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator StartCloseCo()
	{
		if (this.isClosing)
		{
			yield break;
		}
		this.isClosing = true;
		bool shouldClose = false;
		WorldBuilder worldBuilder = this.worldBuilder;
		bool canPrompt = true;
		XUiV_Window parentWindow = base.GetParentWindow();
		yield return worldBuilder.SaveData(canPrompt, ((parentWindow != null) ? parentWindow.Controller : null) ?? this, false, delegate
		{
			shouldClose = false;
		}, delegate
		{
			shouldClose = true;
		}, delegate
		{
			shouldClose = true;
		});
		if (!shouldClose)
		{
			this.UpdateBarValues();
			this.isClosing = false;
			yield break;
		}
		this.Close();
		yield break;
	}

	// Token: 0x06007758 RID: 30552 RVA: 0x00309930 File Offset: 0x00307B30
	[PublicizedFrom(EAccessModifier.Private)]
	public void Close()
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		base.xui.playerUI.windowManager.Open(this.LastWindowID, true, false, true);
	}

	// Token: 0x06007759 RID: 30553 RVA: 0x00309970 File Offset: 0x00307B70
	public override void OnClose()
	{
		base.OnClose();
		base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.RWGEditor);
		base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.RWGCamera);
		base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu, 0f);
		this.Clean();
	}

	// Token: 0x0600775A RID: 30554 RVA: 0x003099C4 File Offset: 0x00307BC4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Clean()
	{
		this.DestroyBuilder();
		PathAbstractions.CacheEnabled = false;
		if (this.BackButton != null)
		{
			this.BackButton.OnPressed -= this.BtnBack_OnPressed;
		}
		if (this.GenerateButton != null)
		{
			this.GenerateButton.OnPressed -= this.GenerateButton_OnPressed;
		}
		if (this.NewGameButton != null)
		{
			this.NewGameButton.OnPressed -= this.NewGameButton_OnPressed;
		}
		if (this.btnManage != null)
		{
			this.btnManage.OnPressed -= this.BtnManage_OnPressed;
		}
		if (this.Quality != null)
		{
			this.Quality.OnValueChanged -= this.Quality_OnValueChanged;
		}
		if (this.SeedInput != null)
		{
			this.SeedInput.OnChangeHandler -= this.SeedInput_OnChangeHandler;
		}
		if (this.WorldSizeComboBox != null)
		{
			this.WorldSizeComboBox.OnValueChanged -= this.WorldSizeComboBox_OnValueChanged;
		}
		if (this.PlainsWeight != null)
		{
			this.PlainsWeight.OnValueChanged -= this.PlainsWeight_OnValueChanged;
		}
		if (this.HillsWeight != null)
		{
			this.HillsWeight.OnValueChanged -= this.HillsWeight_OnValueChanged;
		}
		if (this.MountainsWeight != null)
		{
			this.MountainsWeight.OnValueChanged -= this.MountainsWeight_OnValueChanged;
		}
		if (this.prefabPreviewManager != null)
		{
			this.prefabPreviewManager.Cleanup();
			this.prefabPreviewManager = null;
		}
		RenderSettings.fogDensity = this.oldFogDensity;
	}

	// Token: 0x0600775B RID: 30555 RVA: 0x00309B38 File Offset: 0x00307D38
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (!this.isAdvancedUI)
		{
			return;
		}
		if (base.xui.playerUI.playerInput.PermanentActions.Cancel.WasPressed && !XUiC_DataManagement.IsWindowOpen(base.xui))
		{
			this.StartClose();
		}
		if (PrefabPreviewManager.ReadyToDisplay && this.prefabPreviewManager != null)
		{
			this.prefabPreviewManager.Update();
		}
	}

	// Token: 0x04005AC6 RID: 23238
	public static XUiC_WorldGenerationWindowGroup Instance;

	// Token: 0x04005AC7 RID: 23239
	public string LastWindowID = string.Empty;

	// Token: 0x04005ACA RID: 23242
	public DynamicPrefabDecorator PrefabDecorator;

	// Token: 0x04005ACB RID: 23243
	public XUiC_WorldGenerationPreview PreviewWindow;

	// Token: 0x04005ACC RID: 23244
	public PrefabPreviewManager prefabPreviewManager;

	// Token: 0x04005ACD RID: 23245
	public XUiC_TextInput SeedInput;

	// Token: 0x04005ACE RID: 23246
	public XUiC_SimpleButton GenerateButton;

	// Token: 0x04005ACF RID: 23247
	public XUiC_SimpleButton BackButton;

	// Token: 0x04005AD0 RID: 23248
	public XUiC_SimpleButton NewGameButton;

	// Token: 0x04005AD1 RID: 23249
	public XUiC_ComboBoxList<int> WorldSizeComboBox;

	// Token: 0x04005AD2 RID: 23250
	public XUiC_ComboBoxEnum<SaveDataLimitType> SaveDataLimitComboBox;

	// Token: 0x04005AD3 RID: 23251
	public XUiC_ComboBoxBool TerrainAndBiomeOnly;

	// Token: 0x04005AD4 RID: 23252
	public XUiV_Label CountyNameLabel;

	// Token: 0x04005AD5 RID: 23253
	public XUiC_ComboBoxEnum<WorldBuilder.BiomeLayout> BiomeLayoutComboBox;

	// Token: 0x04005AD6 RID: 23254
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxInt[] biomeComboBoxes = new XUiC_ComboBoxInt[5];

	// Token: 0x04005AD7 RID: 23255
	public XUiC_ComboBoxInt PlainsWeight;

	// Token: 0x04005AD8 RID: 23256
	public XUiC_ComboBoxInt HillsWeight;

	// Token: 0x04005AD9 RID: 23257
	public XUiC_ComboBoxInt MountainsWeight;

	// Token: 0x04005ADA RID: 23258
	public XUiC_ComboBoxEnum<XUiC_WorldGenerationWindowGroup.PreviewQuality> Quality;

	// Token: 0x04005ADB RID: 23259
	public XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections> Rivers;

	// Token: 0x04005ADC RID: 23260
	public XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections> Craters;

	// Token: 0x04005ADD RID: 23261
	public XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections> Canyons;

	// Token: 0x04005ADE RID: 23262
	public XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections> Lakes;

	// Token: 0x04005ADF RID: 23263
	public XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections> Rural;

	// Token: 0x04005AE0 RID: 23264
	public XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections> Town;

	// Token: 0x04005AE1 RID: 23265
	public XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections> City;

	// Token: 0x04005AE2 RID: 23266
	public XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections> Towns;

	// Token: 0x04005AE3 RID: 23267
	public XUiC_ComboBoxEnum<WorldBuilder.GenerationSelections> Wilderness;

	// Token: 0x04005AE4 RID: 23268
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnManage;

	// Token: 0x04005AE5 RID: 23269
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DataManagementBar dataManagementBar;

	// Token: 0x04005AE6 RID: 23270
	[PublicizedFrom(EAccessModifier.Private)]
	public bool dataManagementBarEnabled;

	// Token: 0x04005AE7 RID: 23271
	[PublicizedFrom(EAccessModifier.Private)]
	public long m_pendingBytes;

	// Token: 0x04005AE8 RID: 23272
	[PublicizedFrom(EAccessModifier.Private)]
	public long m_totalAvailableBytes;

	// Token: 0x04005AE9 RID: 23273
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isAdvancedUI;

	// Token: 0x04005AEA RID: 23274
	public int WorldSize;

	// Token: 0x04005AEB RID: 23275
	public bool ValidCountyName;

	// Token: 0x04005AEC RID: 23276
	[PublicizedFrom(EAccessModifier.Private)]
	public string CountyName;

	// Token: 0x04005AED RID: 23277
	[PublicizedFrom(EAccessModifier.Private)]
	public float oldFogDensity;

	// Token: 0x04005AEE RID: 23278
	public XUiC_WorldGenerationWindowGroup.PreviewQuality PreviewQualityLevel = XUiC_WorldGenerationWindowGroup.PreviewQuality.Default;

	// Token: 0x04005AEF RID: 23279
	public WorldBuilder worldBuilder;

	// Token: 0x04005AF0 RID: 23280
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isGenerating;

	// Token: 0x04005AF1 RID: 23281
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isClosing;

	// Token: 0x04005AF2 RID: 23282
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] BiomeToUIName = new string[]
	{
		"xuiPineForest",
		"xuiBurntForest",
		"xuiDesert",
		"xuiSnow",
		"xuiWasteland"
	};

	// Token: 0x02000EB8 RID: 3768
	public struct PrefabData
	{
		// Token: 0x04005AF3 RID: 23283
		public string Name;

		// Token: 0x04005AF4 RID: 23284
		public Vector3i Position;

		// Token: 0x04005AF5 RID: 23285
		public byte Rotation;

		// Token: 0x04005AF6 RID: 23286
		public string DistantPOIOverride;

		// Token: 0x04005AF7 RID: 23287
		public int ID;
	}

	// Token: 0x02000EB9 RID: 3769
	public enum PreviewQuality
	{
		// Token: 0x04005AF9 RID: 23289
		NoPreview,
		// Token: 0x04005AFA RID: 23290
		Lowest,
		// Token: 0x04005AFB RID: 23291
		Low,
		// Token: 0x04005AFC RID: 23292
		Default,
		// Token: 0x04005AFD RID: 23293
		High,
		// Token: 0x04005AFE RID: 23294
		Highest
	}
}
