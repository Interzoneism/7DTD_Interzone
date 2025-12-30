using System;
using System.Runtime.CompilerServices;
using Platform;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Scripting;

// Token: 0x02000D5F RID: 3423
[Preserve]
public class XUiC_OptionsVideo : XUiController
{
	// Token: 0x140000B2 RID: 178
	// (add) Token: 0x06006AF5 RID: 27381 RVA: 0x002BA42C File Offset: 0x002B862C
	// (remove) Token: 0x06006AF6 RID: 27382 RVA: 0x002BA460 File Offset: 0x002B8660
	public static event Action OnSettingsChanged;

	// Token: 0x06006AF7 RID: 27383 RVA: 0x002BA494 File Offset: 0x002B8694
	public override void Init()
	{
		base.Init();
		XUiC_OptionsVideo.ID = base.WindowGroup.ID;
		this.tabs = base.GetChildByType<XUiC_TabSelector>();
		this.tabs.OnTabChanged += this.TabSelector_OnTabChanged;
		this.fsr3Supported = FSR3.FSR3Supported();
		this.dlssSupported = (this.fsr3Supported && DLSS.DLSSSupported());
		this.comboResolution = base.GetChildById("Resolution").GetChildByType<XUiC_ComboBoxList<XUiC_OptionsVideo.ResolutionInfo>>();
		this.comboFullscreen = base.GetChildById("Fullscreen").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboUpscalerMode = base.GetChildById("Upscaler").GetChildByType<XUiC_ComboBoxList<string>>();
		if (!this.dlssSupported)
		{
			this.comboUpscalerMode.MaxIndex = 3;
		}
		if (!this.fsr3Supported)
		{
			this.comboUpscalerMode.MaxIndex = 2;
		}
		this.comboFSRPreset = base.GetChildById("FSRPreset").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboDLSSPreset = base.GetChildById("DLSSPreset").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboDynamicMinFPS = base.GetChildById("DyMinFPS").GetChildByType<XUiC_ComboBoxInt>();
		this.comboDynamicScale = base.GetChildById("DyScale").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboVSync = base.GetChildById("VSync").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboBrightness = base.GetChildById("Brightness").GetChildByType<XUiC_ComboBoxFloat>();
		this.btnDefaultBrightness = base.GetChildById("btnDefaultBrightness").GetChildByType<XUiC_SimpleButton>();
		this.comboFieldOfView = base.GetChildById("FieldOfView").GetChildByType<XUiC_ComboBoxInt>();
		this.btnDefaultFOV = base.GetChildById("btnDefaultFOV").GetChildByType<XUiC_SimpleButton>();
		this.comboQualityPreset = base.GetChildById("QualityPreset").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboAntiAliasing = base.GetChildById("AntiAliasing").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboAntiAliasingSharp = base.GetChildById("AntiAliasingSharp").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboTextureQuality = base.GetChildById("TextureQuality").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboTextureFilter = base.GetChildById("TextureFilter").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboReflectionQuality = base.GetChildById("ReflectionQuality").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboReflectedShadows = base.GetChildById("ReflectedShadows").GetChildByType<XUiC_ComboBoxBool>();
		this.comboWaterQuality = base.GetChildById("WaterQuality").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboViewDistance = base.GetChildById("ViewDistance").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboLODDistance = base.GetChildById("LODDistance").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboShadowsDistance = base.GetChildById("ShadowsDistance").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboShadowsQuality = base.GetChildById("ShadowsQuality").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboTerrainQuality = base.GetChildById("TerrainQuality").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboObjectQuality = base.GetChildById("ObjectQuality").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboGrassDistance = base.GetChildById("GrassDistance").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboOcclusion = base.GetChildById("Occlusion").GetChildByType<XUiC_ComboBoxBool>();
		this.comboDymeshEnabled = base.GetChildById("DynamicMeshEnabled").GetChildByType<XUiC_ComboBoxBool>();
		this.comboDymeshDistance = base.GetChildById("DynamicMeshDistance").GetChildByType<XUiC_ComboBoxList<int>>();
		this.comboDymeshHighQualityMesh = base.GetChildById("DynamicMeshHighQualityMesh").GetChildByType<XUiC_ComboBoxBool>();
		this.comboDymeshMaxRegions = base.GetChildById("DynamicMeshMaxRegionLoads").GetChildByType<XUiC_ComboBoxList<int>>();
		this.comboDymeshMaxMesh = base.GetChildById("DynamicMeshMaxMeshCache").GetChildByType<XUiC_ComboBoxList<int>>();
		this.comboDymeshLandClaimOnly = base.GetChildById("DynamicMeshLandClaimOnly").GetChildByType<XUiC_ComboBoxBool>();
		this.comboDymeshLandClaimBuffer = base.GetChildById("DynamicMeshLandClaimBuffer").GetChildByType<XUiC_ComboBoxList<int>>();
		this.comboBloom = base.GetChildById("Bloom").GetChildByType<XUiC_ComboBoxBool>();
		XUiController childById = base.GetChildById("DepthOfField");
		this.comboDepthOfField = ((childById != null) ? childById.GetChildByType<XUiC_ComboBoxBool>() : null);
		this.comboMotionBlur = base.GetChildById("MotionBlur").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboSSAO = base.GetChildById("SSAO").GetChildByType<XUiC_ComboBoxBool>();
		this.comboSSReflections = base.GetChildById("SSReflections").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboSunShafts = base.GetChildById("SunShafts").GetChildByType<XUiC_ComboBoxBool>();
		this.comboParticles = base.GetChildById("Particles").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboUIBackgroundOpacity = base.GetChildById("UIBackgroundOpacity").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboUIForegroundOpacity = base.GetChildById("UIForegroundOpacity").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboUiSize = base.GetChildById("UiSize").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboScreenBounds = base.GetChildById("ScreenBounds").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboUiFpsScaling = base.GetChildById("UiFpsScaling").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboQualityPreset.OnValueChanged += this.QualityPresetChanged;
		this.comboAntiAliasing.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboAntiAliasingSharp.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboTextureQuality.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboTextureFilter.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboReflectionQuality.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboReflectedShadows.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboWaterQuality.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboViewDistance.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboLODDistance.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboShadowsDistance.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboShadowsQuality.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboTerrainQuality.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboObjectQuality.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboGrassDistance.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboOcclusion.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboDymeshEnabled.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboDymeshDistance.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboDymeshHighQualityMesh.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboDymeshMaxRegions.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboDymeshMaxMesh.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboDymeshLandClaimOnly.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboDymeshLandClaimBuffer.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboBloom.OnValueChangedGeneric += this.AnyPresetValueChanged;
		if (this.comboDepthOfField != null)
		{
			this.comboDepthOfField.OnValueChangedGeneric += this.AnyPresetValueChanged;
		}
		this.comboMotionBlur.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboSSAO.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboSSReflections.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboSunShafts.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboParticles.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboResolution.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboFullscreen.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboUpscalerMode.OnValueChangedGeneric += this.upscalerModeChanged;
		this.comboFSRPreset.OnValueChangedGeneric += this.upscalerPresetChanged;
		this.comboDLSSPreset.OnValueChangedGeneric += this.upscalerPresetChanged;
		this.comboDynamicMinFPS.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboDynamicScale.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboVSync.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboBrightness.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboFieldOfView.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboUIBackgroundOpacity.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboUIForegroundOpacity.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboUiSize.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboScreenBounds.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboUiFpsScaling.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboDynamicMinFPS.Min = 10L;
		this.comboDynamicMinFPS.Max = 60L;
		this.comboDynamicScale.Min = 0.20000000298023224;
		this.comboDynamicScale.Max = 1.0;
		this.comboUIBackgroundOpacity.Min = (double)Constants.cMinGlobalBackgroundOpacity;
		this.comboUIBackgroundOpacity.Max = 1.0;
		this.comboUIForegroundOpacity.Min = (double)Constants.cMinGlobalForegroundOpacity;
		this.comboUIForegroundOpacity.Max = 1.0;
		this.comboUiSize.Min = 0.7;
		this.comboUiSize.Max = 1.0;
		this.comboScreenBounds.Min = 0.8;
		this.comboScreenBounds.Max = 1.0;
		this.comboBrightness.Min = 0.0;
		this.comboBrightness.Max = 1.0;
		this.btnDefaultBrightness.OnPressed += this.BtnDefaultBrightness_OnPressed;
		this.comboFieldOfView.Min = (long)Constants.cMinCameraFieldOfView;
		this.comboFieldOfView.Max = (long)Constants.cMaxCameraFieldOfView;
		this.btnDefaultFOV.OnPressed += this.BtnDefaultFOV_OnPressed;
		this.comboAntiAliasingSharp.Min = 0.0;
		this.comboAntiAliasingSharp.Max = 1.0;
		this.comboParticles.Min = 0.0;
		this.comboParticles.Max = 1.0;
		this.comboLODDistance.Min = 0.0;
		this.comboLODDistance.Max = 1.0;
		this.origLength_ReflectionQuality = this.comboReflectionQuality.Elements.Count;
		this.comboReflectionQuality.MaxIndex = this.origLength_ReflectionQuality - 1;
		this.comboReflectionQuality.Elements.Add("Custom");
		this.origLength_ShadowDistance = this.comboShadowsDistance.Elements.Count;
		this.comboShadowsDistance.MaxIndex = this.origLength_ShadowDistance - 1;
		this.comboShadowsDistance.Elements.Add("Custom");
		this.origLength_ShadowQuality = this.comboShadowsQuality.Elements.Count;
		this.comboShadowsQuality.MaxIndex = this.origLength_ShadowQuality - 1;
		this.comboShadowsQuality.Elements.Add("Custom");
		this.origLength_TerrainQuality = this.comboTerrainQuality.Elements.Count;
		this.comboTerrainQuality.MaxIndex = this.origLength_TerrainQuality - 1;
		this.comboTerrainQuality.Elements.Add("Custom");
		this.origLength_ObjectQuality = this.comboObjectQuality.Elements.Count;
		this.comboObjectQuality.MaxIndex = this.origLength_ObjectQuality - 1;
		this.comboObjectQuality.Elements.Add("Custom");
		this.origLength_GrassDistance = this.comboGrassDistance.Elements.Count;
		this.comboGrassDistance.MaxIndex = this.origLength_GrassDistance - 1;
		this.comboGrassDistance.Elements.Add("Custom");
		this.origLength_MotionBlur = this.comboMotionBlur.Elements.Count;
		this.comboMotionBlur.MaxIndex = this.origLength_MotionBlur - 1;
		this.comboMotionBlur.Elements.Add("Custom");
		this.origLength_SSR = this.comboSSReflections.Elements.Count;
		this.comboSSReflections.MaxIndex = this.origLength_SSR - 1;
		this.comboSSReflections.Elements.Add("Custom");
		if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
		{
			this.comboQualityPreset.Elements.Add("ConsolePerformance");
			this.comboQualityPreset.Elements.Add("ConsoleQuality");
		}
		this.btnBack = (base.GetChildById("btnBack") as XUiC_SimpleButton);
		this.btnDefaults = (base.GetChildById("btnDefaults") as XUiC_SimpleButton);
		this.btnApply = (base.GetChildById("btnApply") as XUiC_SimpleButton);
		this.btnBack.OnPressed += this.BtnBack_OnPressed;
		this.btnDefaults.OnPressed += this.BtnDefaults_OnOnPressed;
		this.btnApply.OnPressed += this.BtnApply_OnPressed;
		this.RefreshApplyLabel();
		base.RegisterForInputStyleChanges();
	}

	// Token: 0x06006AF8 RID: 27384 RVA: 0x002BB1CB File Offset: 0x002B93CB
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshApplyLabel()
	{
		InControlExtensions.SetApplyButtonString(this.btnApply, "xuiApply");
	}

	// Token: 0x06006AF9 RID: 27385 RVA: 0x002BB1DD File Offset: 0x002B93DD
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InputStyleChanged(PlayerInputManager.InputStyle _oldStyle, PlayerInputManager.InputStyle _newStyle)
	{
		base.InputStyleChanged(_oldStyle, _newStyle);
		this.RefreshApplyLabel();
	}

	// Token: 0x06006AFA RID: 27386 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Protected)]
	public void TabSelector_OnTabChanged(int _i, XUiC_TabSelectorTab _tab)
	{
		this.IsDirty = true;
	}

	// Token: 0x06006AFB RID: 27387 RVA: 0x002BB1ED File Offset: 0x002B93ED
	[PublicizedFrom(EAccessModifier.Protected)]
	public void upscalerModeChanged(XUiController _sender)
	{
		this.updateDynamicOptions();
		this.btnApply.Enabled = true;
		this.IsDirty = true;
	}

	// Token: 0x06006AFC RID: 27388 RVA: 0x002BB208 File Offset: 0x002B9408
	[PublicizedFrom(EAccessModifier.Protected)]
	public void upscalerPresetChanged(XUiController _sender)
	{
		if (_sender as XUiC_ComboBoxList<string> == this.comboFSRPreset)
		{
			this.comboDLSSPreset.SelectedIndex = this.comboFSRPreset.SelectedIndex;
		}
		else
		{
			this.comboFSRPreset.SelectedIndex = this.comboDLSSPreset.SelectedIndex;
		}
		this.updateDynamicOptions();
		this.btnApply.Enabled = true;
		this.IsDirty = true;
	}

	// Token: 0x06006AFD RID: 27389 RVA: 0x002BB26A File Offset: 0x002B946A
	[PublicizedFrom(EAccessModifier.Protected)]
	public void anyOtherValueChanged(XUiController _sender)
	{
		this.updateDynamicOptions();
		this.btnApply.Enabled = true;
	}

	// Token: 0x06006AFE RID: 27390 RVA: 0x002BB27E File Offset: 0x002B947E
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDefaultBrightness_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.comboBrightness.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsGfxBrightness));
		this.btnApply.Enabled = true;
	}

	// Token: 0x06006AFF RID: 27391 RVA: 0x002BB2A7 File Offset: 0x002B94A7
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDefaultFOV_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.comboFieldOfView.Value = (long)((int)GamePrefs.GetDefault(EnumGamePrefs.OptionsGfxFOV));
		this.btnApply.Enabled = true;
	}

	// Token: 0x06006B00 RID: 27392 RVA: 0x002BB2CD File Offset: 0x002B94CD
	[PublicizedFrom(EAccessModifier.Protected)]
	public void BtnApply_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.applyChanges();
	}

	// Token: 0x06006B01 RID: 27393 RVA: 0x002BB2D8 File Offset: 0x002B94D8
	[PublicizedFrom(EAccessModifier.Protected)]
	public void BtnDefaults_OnOnPressed(XUiController _sender, int _mouseButton)
	{
		this.comboUIBackgroundOpacity.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsBackgroundGlobalOpacity));
		this.comboUIForegroundOpacity.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsForegroundGlobalOpacity));
		this.comboUiSize.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsHudSize));
		this.comboScreenBounds.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsScreenBoundsValue));
		this.comboUiFpsScaling.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsUiFpsScaling));
		this.btnApply.Enabled = true;
	}

	// Token: 0x06006B02 RID: 27394 RVA: 0x002A30F5 File Offset: 0x002A12F5
	[PublicizedFrom(EAccessModifier.Protected)]
	public void BtnBack_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		base.xui.playerUI.windowManager.Open(XUiC_OptionsMenu.ID, true, false, true);
	}

	// Token: 0x06006B03 RID: 27395 RVA: 0x002BB378 File Offset: 0x002B9578
	[PublicizedFrom(EAccessModifier.Private)]
	public static XUiC_OptionsVideo.GraphicsMode QualityPresetToGraphicsMode(int qualityPreset)
	{
		switch (qualityPreset)
		{
		case 0:
			return XUiC_OptionsVideo.GraphicsMode.Lowest;
		case 1:
			return XUiC_OptionsVideo.GraphicsMode.Low;
		case 2:
			return XUiC_OptionsVideo.GraphicsMode.Medium;
		case 3:
			return XUiC_OptionsVideo.GraphicsMode.High;
		case 4:
			return XUiC_OptionsVideo.GraphicsMode.Ultra;
		case 6:
			return XUiC_OptionsVideo.GraphicsMode.ConsolePerformance;
		case 8:
			return XUiC_OptionsVideo.GraphicsMode.ConsoleQuality;
		}
		return XUiC_OptionsVideo.GraphicsMode.Custom;
	}

	// Token: 0x06006B04 RID: 27396 RVA: 0x002BB3D0 File Offset: 0x002B95D0
	[PublicizedFrom(EAccessModifier.Private)]
	public static int GraphicsModeToQualityPreset(XUiC_OptionsVideo.GraphicsMode graphicsMode)
	{
		switch (graphicsMode)
		{
		case XUiC_OptionsVideo.GraphicsMode.Lowest:
			return 0;
		case XUiC_OptionsVideo.GraphicsMode.Low:
			return 1;
		case XUiC_OptionsVideo.GraphicsMode.Medium:
			return 2;
		case XUiC_OptionsVideo.GraphicsMode.High:
			return 3;
		case XUiC_OptionsVideo.GraphicsMode.Ultra:
			return 4;
		case XUiC_OptionsVideo.GraphicsMode.ConsolePerformance:
			return 6;
		case XUiC_OptionsVideo.GraphicsMode.ConsoleQuality:
			return 8;
		}
		return 5;
	}

	// Token: 0x06006B05 RID: 27397 RVA: 0x002BB424 File Offset: 0x002B9624
	[PublicizedFrom(EAccessModifier.Private)]
	public void QualityPresetChanged(XUiController _sender, string _oldValue, string _newValue)
	{
		int num = XUiC_OptionsVideo.GraphicsModeToQualityPreset((XUiC_OptionsVideo.GraphicsMode)this.comboQualityPreset.SelectedIndex);
		if (num != 5)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxQualityPreset, num);
			GameOptionsManager.SetGraphicsQuality();
			this.updateGraphicOptions();
			this.updateDynamicOptions();
		}
		this.btnApply.Enabled = true;
	}

	// Token: 0x06006B06 RID: 27398 RVA: 0x002BB46E File Offset: 0x002B966E
	[PublicizedFrom(EAccessModifier.Protected)]
	public void AnyPresetValueChanged(XUiController _sender)
	{
		this.comboQualityPreset.SelectedIndex = 5;
		GamePrefs.Set(EnumGamePrefs.OptionsGfxQualityPreset, 5);
		this.btnApply.Enabled = true;
		this.updateGraphicsAAOptions();
	}

	// Token: 0x06006B07 RID: 27399 RVA: 0x002BB499 File Offset: 0x002B9699
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateDynamicOptions()
	{
		this.comboAntiAliasing.Enabled = (this.comboUpscalerMode.SelectedIndex != 3 && this.comboUpscalerMode.SelectedIndex != 4);
	}

	// Token: 0x06006B08 RID: 27400 RVA: 0x002BB4C8 File Offset: 0x002B96C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateGraphicsAAOptions()
	{
		this.comboAntiAliasingSharp.Enabled = (this.comboAntiAliasing.SelectedIndex >= 4 || this.comboUpscalerMode.SelectedIndex == 3 || this.comboUpscalerMode.SelectedIndex == 4);
	}

	// Token: 0x06006B09 RID: 27401 RVA: 0x002BB504 File Offset: 0x002B9704
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateGraphicOptions()
	{
		Resolution[] supportedResolutions = PlatformApplicationManager.Application.SupportedResolutions;
		this.comboResolution.Elements.Clear();
		foreach (Resolution resolution in supportedResolutions)
		{
			XUiC_OptionsVideo.ResolutionInfo item = new XUiC_OptionsVideo.ResolutionInfo(resolution.width, resolution.height);
			if (!this.comboResolution.Elements.Contains(item))
			{
				this.comboResolution.Elements.Add(item);
			}
		}
		this.comboResolution.Elements.Sort();
		ValueTuple<int, int, FullScreenMode> screenOptions = PlatformApplicationManager.Application.ScreenOptions;
		int item2 = screenOptions.Item1;
		int item3 = screenOptions.Item2;
		XUiC_OptionsVideo.ResolutionInfo item4 = new XUiC_OptionsVideo.ResolutionInfo(item2, item3);
		if (!this.comboResolution.Elements.Contains(item4))
		{
			this.comboResolution.Elements.Add(item4);
			this.comboResolution.Elements.Sort();
		}
		this.comboResolution.SelectedIndex = this.comboResolution.Elements.IndexOf(item4);
		FullScreenMode @int = (FullScreenMode)SdPlayerPrefs.GetInt("Screenmanager Fullscreen mode", 3);
		this.comboFullscreen.SelectedIndex = XUiC_OptionsVideo.ConvertFullScreenModeToIndex(@int);
		int i;
		switch (GamePrefs.GetInt(EnumGamePrefs.OptionsGfxUpscalerMode))
		{
		case 0:
			i = 0;
			goto IL_178;
		case 2:
			i = (this.fsr3Supported ? 3 : -1);
			goto IL_178;
		case 3:
			i = 1;
			goto IL_178;
		case 4:
			i = 2;
			goto IL_178;
		case 5:
			i = (this.dlssSupported ? 4 : -1);
			goto IL_178;
		}
		i = -1;
		IL_178:
		int num = i;
		if (num == -1)
		{
			if (this.fsr3Supported)
			{
				Log.Out(string.Format("Upscaler mode \"{0}\" is unsupported on this platform; defaulting to \"{1}\".", GamePrefs.GetInt(EnumGamePrefs.OptionsGfxUpscalerMode), 2));
				num = 3;
				GamePrefs.Set(EnumGamePrefs.OptionsGfxUpscalerMode, 2);
			}
			else
			{
				Log.Out(string.Format("Upscaler mode \"{0}\" is unsupported on this platform; defaulting to \"{1}\".", GamePrefs.GetInt(EnumGamePrefs.OptionsGfxUpscalerMode), 4));
				num = 2;
				GamePrefs.Set(EnumGamePrefs.OptionsGfxUpscalerMode, 4);
			}
		}
		this.comboUpscalerMode.SelectedIndex = num;
		this.comboFSRPreset.SelectedIndex = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFSRPreset);
		this.comboDLSSPreset.SelectedIndex = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFSRPreset);
		this.comboDynamicMinFPS.Value = (long)GamePrefs.GetInt(EnumGamePrefs.OptionsGfxDynamicMinFPS);
		this.comboDynamicScale.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxDynamicScale);
		this.comboVSync.SelectedIndex = GamePrefs.GetInt(this.VSyncCountPref);
		this.comboBrightness.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxBrightness);
		this.comboFieldOfView.Value = (long)GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFOV);
		this.comboDymeshEnabled.Value = GamePrefs.GetBool(EnumGamePrefs.DynamicMeshEnabled);
		this.comboDymeshDistance.Value = GamePrefs.GetInt(EnumGamePrefs.DynamicMeshDistance);
		this.comboDymeshHighQualityMesh.Value = !GamePrefs.GetBool(EnumGamePrefs.DynamicMeshUseImposters);
		this.comboDymeshMaxRegions.Value = GamePrefs.GetInt(EnumGamePrefs.DynamicMeshMaxRegionCache);
		this.comboDymeshMaxMesh.Value = GamePrefs.GetInt(EnumGamePrefs.DynamicMeshMaxItemCache);
		this.comboDymeshLandClaimOnly.Value = GamePrefs.GetBool(EnumGamePrefs.DynamicMeshLandClaimOnly);
		this.comboDymeshLandClaimBuffer.Value = GamePrefs.GetInt(EnumGamePrefs.DynamicMeshLandClaimBuffer);
		this.comboQualityPreset.SelectedIndex = (int)XUiC_OptionsVideo.QualityPresetToGraphicsMode(GamePrefs.GetInt(EnumGamePrefs.OptionsGfxQualityPreset));
		this.comboAntiAliasing.SelectedIndex = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxAA);
		this.comboAntiAliasingSharp.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxAASharpness);
		this.updateGraphicsAAOptions();
		this.comboTextureQuality.SelectedIndex = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxTexQuality);
		this.comboTextureFilter.SelectedIndex = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxTexFilter);
		int int2 = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxReflectQuality);
		if (int2 < this.origLength_ReflectionQuality)
		{
			this.comboReflectionQuality.SelectedIndex = int2;
		}
		else
		{
			this.comboReflectionQuality.MaxIndex = this.origLength_ReflectionQuality;
			this.comboReflectionQuality.SelectedIndex = this.comboReflectionQuality.Elements.Count - 1;
		}
		this.comboReflectedShadows.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsGfxReflectShadows);
		this.comboWaterQuality.SelectedIndex = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxWaterQuality);
		this.comboViewDistance.SelectedIndex = ((GamePrefs.GetInt(EnumGamePrefs.OptionsGfxViewDistance) == 5) ? 0 : ((GamePrefs.GetInt(EnumGamePrefs.OptionsGfxViewDistance) == 6) ? 1 : 2));
		this.comboLODDistance.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxLODDistance);
		int num2 = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxShadowDistance);
		if (num2 >= this.origLength_ShadowDistance && num2 < 20)
		{
			num2 = 20;
			GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowDistance, 20);
		}
		if (num2 < this.origLength_ShadowDistance)
		{
			this.comboShadowsDistance.SelectedIndex = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxShadowDistance);
		}
		else
		{
			this.comboShadowsDistance.MaxIndex = this.origLength_ShadowDistance;
			this.comboShadowsDistance.SelectedIndex = this.comboShadowsDistance.Elements.Count - 1;
		}
		int int3 = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxShadowQuality);
		if (int3 < this.origLength_ShadowQuality)
		{
			this.comboShadowsQuality.SelectedIndex = int3;
		}
		else
		{
			this.comboShadowsQuality.MaxIndex = this.origLength_ShadowQuality;
			this.comboShadowsQuality.SelectedIndex = this.comboShadowsQuality.Elements.Count - 1;
		}
		int int4 = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxTerrainQuality);
		if (int4 < this.origLength_TerrainQuality)
		{
			this.comboTerrainQuality.SelectedIndex = int4;
		}
		else
		{
			this.comboTerrainQuality.MaxIndex = this.origLength_TerrainQuality;
			this.comboTerrainQuality.SelectedIndex = this.comboTerrainQuality.Elements.Count - 1;
		}
		int int5 = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxObjQuality);
		if (int5 < this.origLength_ObjectQuality)
		{
			this.comboObjectQuality.SelectedIndex = int5;
		}
		else
		{
			this.comboObjectQuality.MaxIndex = this.origLength_ObjectQuality;
			this.comboObjectQuality.SelectedIndex = this.comboObjectQuality.Elements.Count - 1;
		}
		int int6 = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxGrassDistance);
		if (int6 < this.origLength_GrassDistance)
		{
			this.comboGrassDistance.SelectedIndex = int6;
		}
		else
		{
			this.comboGrassDistance.MaxIndex = this.origLength_GrassDistance;
			this.comboGrassDistance.SelectedIndex = this.comboGrassDistance.Elements.Count - 1;
		}
		this.comboOcclusion.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsGfxOcclusion);
		this.comboBloom.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsGfxBloom);
		if (this.comboDepthOfField != null)
		{
			this.comboDepthOfField.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsGfxDOF);
		}
		int int7 = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxMotionBlur);
		if (int7 < this.origLength_MotionBlur)
		{
			this.comboMotionBlur.SelectedIndex = int7;
		}
		else
		{
			this.comboMotionBlur.MaxIndex = this.origLength_MotionBlur;
			this.comboMotionBlur.SelectedIndex = this.comboMotionBlur.Elements.Count - 1;
		}
		this.comboSSAO.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsGfxSSAO);
		int int8 = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxSSReflections);
		if (int8 < this.origLength_SSR)
		{
			this.comboSSReflections.SelectedIndex = int8;
		}
		else
		{
			this.comboSSReflections.MaxIndex = this.origLength_SSR;
			this.comboSSReflections.SelectedIndex = this.comboSSReflections.Elements.Count - 1;
		}
		this.comboSunShafts.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsGfxSunShafts);
		this.comboParticles.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxWaterPtlLimiter);
		this.comboUIBackgroundOpacity.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsBackgroundGlobalOpacity);
		this.comboUIForegroundOpacity.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsForegroundGlobalOpacity);
		this.comboUiSize.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsHudSize);
		this.comboScreenBounds.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsScreenBoundsValue);
		this.comboUiFpsScaling.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsUiFpsScaling);
	}

	// Token: 0x06006B0A RID: 27402 RVA: 0x002BBC98 File Offset: 0x002B9E98
	[PublicizedFrom(EAccessModifier.Private)]
	public void applyChanges()
	{
		XUiC_OptionsVideo.ResolutionInfo value = this.comboResolution.Value;
		GameOptionsManager.SetResolution(value.Width, value.Height, XUiC_OptionsVideo.ConvertIndexToFullScreenMode(this.comboFullscreen.SelectedIndex));
		int num;
		switch (this.comboUpscalerMode.SelectedIndex)
		{
		case 0:
			num = 0;
			break;
		case 1:
			num = 3;
			break;
		case 2:
			num = 4;
			break;
		case 3:
			num = (this.fsr3Supported ? 2 : GameOptionsPlatforms.DefaultUpscalerMode);
			break;
		case 4:
			num = ((this.fsr3Supported && this.dlssSupported) ? 5 : GameOptionsPlatforms.DefaultUpscalerMode);
			break;
		default:
			num = GameOptionsPlatforms.DefaultUpscalerMode;
			break;
		}
		int value2 = num;
		GamePrefs.Set(EnumGamePrefs.OptionsGfxUpscalerMode, value2);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxFSRPreset, this.comboFSRPreset.ViewComponent.IsVisible ? this.comboFSRPreset.SelectedIndex : this.comboDLSSPreset.SelectedIndex);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxDynamicMinFPS, (int)this.comboDynamicMinFPS.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxDynamicScale, (float)this.comboDynamicScale.Value);
		GamePrefs.Set(this.VSyncCountPref, this.comboVSync.SelectedIndex);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxBrightness, (float)this.comboBrightness.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxFOV, (int)this.comboFieldOfView.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxQualityPreset, XUiC_OptionsVideo.GraphicsModeToQualityPreset((XUiC_OptionsVideo.GraphicsMode)this.comboQualityPreset.SelectedIndex));
		GamePrefs.Set(EnumGamePrefs.OptionsGfxAA, this.comboAntiAliasing.SelectedIndex);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxAASharpness, (float)this.comboAntiAliasingSharp.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxTexQuality, this.comboTextureQuality.SelectedIndex);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxTexFilter, this.comboTextureFilter.SelectedIndex);
		if (this.comboReflectionQuality.SelectedIndex < this.origLength_ReflectionQuality)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxReflectQuality, this.comboReflectionQuality.SelectedIndex);
		}
		GamePrefs.Set(EnumGamePrefs.OptionsGfxReflectShadows, this.comboReflectedShadows.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxWaterQuality, this.comboWaterQuality.SelectedIndex);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxViewDistance, (this.comboViewDistance.SelectedIndex == 0) ? 5 : ((this.comboViewDistance.SelectedIndex == 1) ? 6 : 7));
		GamePrefs.Set(EnumGamePrefs.OptionsGfxLODDistance, (float)this.comboLODDistance.Value);
		if (this.comboShadowsDistance.SelectedIndex < this.origLength_ShadowDistance)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowDistance, this.comboShadowsDistance.SelectedIndex);
		}
		if (this.comboShadowsQuality.SelectedIndex < this.origLength_ShadowQuality)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowQuality, this.comboShadowsQuality.SelectedIndex);
		}
		if (this.comboTerrainQuality.SelectedIndex < this.origLength_TerrainQuality)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTerrainQuality, this.comboTerrainQuality.SelectedIndex);
		}
		if (this.comboObjectQuality.SelectedIndex < this.origLength_ObjectQuality)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxObjQuality, this.comboObjectQuality.SelectedIndex);
		}
		if (this.comboGrassDistance.SelectedIndex < this.origLength_GrassDistance)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxGrassDistance, this.comboGrassDistance.SelectedIndex);
		}
		this.origDymeshEnabled = GamePrefs.GetBool(EnumGamePrefs.DynamicMeshEnabled);
		GamePrefs.Set(EnumGamePrefs.DynamicMeshEnabled, this.comboDymeshEnabled.Value);
		GamePrefs.Set(EnumGamePrefs.DynamicMeshDistance, this.comboDymeshDistance.Value);
		DynamicMeshSettings.MaxViewDistance = this.comboDymeshDistance.Value;
		GamePrefs.Set(EnumGamePrefs.DynamicMeshUseImposters, !this.comboDymeshHighQualityMesh.Value);
		GamePrefs.Set(EnumGamePrefs.DynamicMeshMaxRegionCache, this.comboDymeshMaxRegions.Value);
		GamePrefs.Set(EnumGamePrefs.DynamicMeshMaxItemCache, this.comboDymeshMaxMesh.Value);
		GamePrefs.Set(EnumGamePrefs.DynamicMeshLandClaimOnly, this.comboDymeshLandClaimOnly.Value);
		GamePrefs.Set(EnumGamePrefs.DynamicMeshLandClaimBuffer, this.comboDymeshLandClaimBuffer.Value);
		if (this.origDymeshEnabled != this.comboDymeshEnabled.Value)
		{
			DynamicMeshManager.EnabledChanged(this.comboDymeshEnabled.Value);
		}
		GamePrefs.Set(EnumGamePrefs.OptionsGfxOcclusion, this.comboOcclusion.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxBloom, this.comboBloom.Value);
		if (this.comboDepthOfField != null)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxDOF, this.comboDepthOfField.Value);
		}
		if (this.comboMotionBlur.SelectedIndex < this.origLength_MotionBlur)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxMotionBlur, this.comboMotionBlur.SelectedIndex);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxMotionBlurEnabled, this.comboMotionBlur.SelectedIndex > 0);
		}
		GamePrefs.Set(EnumGamePrefs.OptionsGfxSSAO, this.comboSSAO.Value);
		if (this.comboSSReflections.SelectedIndex < this.origLength_SSR)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSSReflections, this.comboSSReflections.SelectedIndex);
		}
		GamePrefs.Set(EnumGamePrefs.OptionsGfxSunShafts, this.comboSunShafts.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxWaterPtlLimiter, (float)this.comboParticles.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsBackgroundGlobalOpacity, (float)this.comboUIBackgroundOpacity.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsForegroundGlobalOpacity, (float)this.comboUIForegroundOpacity.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsHudSize, (float)this.comboUiSize.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsScreenBoundsValue, (float)this.comboScreenBounds.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsUiFpsScaling, (float)this.comboUiFpsScaling.Value);
		GamePrefs.Instance.Save();
		GameOptionsManager.ApplyAllOptions(base.xui.playerUI);
		QualitySettings.vSyncCount = GamePrefs.GetInt(this.VSyncCountPref);
		ReflectionManager.ApplyOptions(false);
		WaterSplashCubes.particleLimiter = GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxWaterPtlLimiter);
		foreach (XUi xui in UnityEngine.Object.FindObjectsOfType<XUi>())
		{
			xui.BackgroundGlobalOpacity = GamePrefs.GetFloat(EnumGamePrefs.OptionsBackgroundGlobalOpacity);
			xui.ForegroundGlobalOpacity = GamePrefs.GetFloat(EnumGamePrefs.OptionsForegroundGlobalOpacity);
		}
		Action onSettingsChanged = XUiC_OptionsVideo.OnSettingsChanged;
		if (onSettingsChanged != null)
		{
			onSettingsChanged();
		}
		this.previousSettings = GamePrefs.GetSettingsCopy();
		this.btnApply.Enabled = false;
	}

	// Token: 0x06006B0B RID: 27403 RVA: 0x002BC265 File Offset: 0x002BA465
	public static int ConvertFullScreenModeToIndex(FullScreenMode _mode)
	{
		if (_mode == FullScreenMode.ExclusiveFullScreen)
		{
			return 2;
		}
		if (_mode == FullScreenMode.FullScreenWindow)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06006B0C RID: 27404 RVA: 0x002BC273 File Offset: 0x002BA473
	public static FullScreenMode ConvertIndexToFullScreenMode(int _index)
	{
		if (_index == 1)
		{
			return FullScreenMode.FullScreenWindow;
		}
		if (_index != 2)
		{
			return FullScreenMode.Windowed;
		}
		return FullScreenMode.ExclusiveFullScreen;
	}

	// Token: 0x06006B0D RID: 27405 RVA: 0x002BC284 File Offset: 0x002BA484
	public override void OnOpen()
	{
		base.WindowGroup.openWindowOnEsc = XUiC_OptionsMenu.ID;
		this.previousSettings = GamePrefs.GetSettingsCopy();
		int minIndex = GameOptionsManager.CalcTextureQualityMin();
		this.comboTextureQuality.MinIndex = minIndex;
		this.VSyncCountPref = PlatformApplicationManager.Application.VSyncCountPref;
		this.updateGraphicOptions();
		this.updateDynamicOptions();
		bool flag = GameManager.Instance.World != null;
		this.comboDymeshLandClaimOnly.Enabled = !flag;
		this.comboViewDistance.Enabled = !flag;
		this.comboOcclusion.Enabled = !flag;
		this.comboDymeshEnabled.Enabled = !flag;
		base.OnOpen();
		this.btnApply.Enabled = false;
		this.RefreshApplyLabel();
	}

	// Token: 0x06006B0E RID: 27406 RVA: 0x002BC33C File Offset: 0x002BA53C
	public override void OnClose()
	{
		GamePrefs.ApplySettingsCopy(this.previousSettings);
		base.OnClose();
	}

	// Token: 0x06006B0F RID: 27407 RVA: 0x002BC350 File Offset: 0x002BA550
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
		if (this.btnApply.Enabled && base.xui.playerUI.playerInput.GUIActions.Apply.WasPressed)
		{
			this.BtnApply_OnPressed(null, 0);
		}
	}

	// Token: 0x06006B10 RID: 27408 RVA: 0x002BC3B0 File Offset: 0x002BA5B0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
		if (num <= 2596575389U)
		{
			if (num <= 840098899U)
			{
				if (num != 217795334U)
				{
					if (num == 840098899U)
					{
						if (_bindingName == "texture_quality_limited")
						{
							XUiC_ComboBoxList<string> xuiC_ComboBoxList = this.comboTextureQuality;
							_value = (xuiC_ComboBoxList != null && xuiC_ComboBoxList.MinIndex > 0).ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "isTabUi")
				{
					_value = ((this.tabs != null && this.tabs.IsSelected("xuiOptionsVideoUI")) ? "true" : "false");
					return true;
				}
			}
			else if (num != 2382361988U)
			{
				if (num == 2596575389U)
				{
					if (_bindingName == "ui_size_limit")
					{
						_value = GameOptionsManager.GetUiSizeLimit().ToCultureInvariantString();
						return true;
					}
				}
			}
			else if (_bindingName == "upscaler_mode_dynamic")
			{
				XUiC_ComboBoxList<string> xuiC_ComboBoxList2 = this.comboUpscalerMode;
				_value = (xuiC_ComboBoxList2 != null && xuiC_ComboBoxList2.SelectedIndex == 1).ToString();
				return true;
			}
		}
		else if (num <= 2786375855U)
		{
			if (num != 2691628302U)
			{
				if (num == 2786375855U)
				{
					if (_bindingName == "upscaler_mode_scale")
					{
						XUiC_ComboBoxList<string> xuiC_ComboBoxList3 = this.comboUpscalerMode;
						_value = (xuiC_ComboBoxList3 != null && xuiC_ComboBoxList3.SelectedIndex == 2).ToString();
						return true;
					}
				}
			}
			else if (_bindingName == "upscaler_mode_fsr")
			{
				XUiC_ComboBoxList<string> xuiC_ComboBoxList4 = this.comboUpscalerMode;
				_value = (xuiC_ComboBoxList4 != null && xuiC_ComboBoxList4.SelectedIndex == 3).ToString();
				return true;
			}
		}
		else if (num != 3514326244U)
		{
			if (num == 3648091069U)
			{
				if (_bindingName == "upscaler_mode_dlss")
				{
					XUiC_ComboBoxList<string> xuiC_ComboBoxList5 = this.comboUpscalerMode;
					_value = (xuiC_ComboBoxList5 != null && xuiC_ComboBoxList5.SelectedIndex == 4).ToString();
					return true;
				}
			}
		}
		else if (_bindingName == "ui_size_limited")
		{
			float num2 = (float)GameOptionsManager.GetUiSizeLimit();
			float @float = GamePrefs.GetFloat(EnumGamePrefs.OptionsHudSize);
			_value = (@float > num2).ToString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x040050EC RID: 20716
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumGamePrefs VSyncCountPref = EnumGamePrefs.OptionsGfxVsync;

	// Token: 0x040050ED RID: 20717
	public static string ID = "";

	// Token: 0x040050EF RID: 20719
	[PublicizedFrom(EAccessModifier.Private)]
	public bool dlssSupported;

	// Token: 0x040050F0 RID: 20720
	[PublicizedFrom(EAccessModifier.Private)]
	public bool fsr3Supported;

	// Token: 0x040050F1 RID: 20721
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TabSelector tabs;

	// Token: 0x040050F2 RID: 20722
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<XUiC_OptionsVideo.ResolutionInfo> comboResolution;

	// Token: 0x040050F3 RID: 20723
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboFullscreen;

	// Token: 0x040050F4 RID: 20724
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboUpscalerMode;

	// Token: 0x040050F5 RID: 20725
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboFSRPreset;

	// Token: 0x040050F6 RID: 20726
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboDLSSPreset;

	// Token: 0x040050F7 RID: 20727
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxInt comboDynamicMinFPS;

	// Token: 0x040050F8 RID: 20728
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboDynamicScale;

	// Token: 0x040050F9 RID: 20729
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboVSync;

	// Token: 0x040050FA RID: 20730
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboBrightness;

	// Token: 0x040050FB RID: 20731
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnDefaultBrightness;

	// Token: 0x040050FC RID: 20732
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxInt comboFieldOfView;

	// Token: 0x040050FD RID: 20733
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnDefaultFOV;

	// Token: 0x040050FE RID: 20734
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboQualityPreset;

	// Token: 0x040050FF RID: 20735
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboAntiAliasing;

	// Token: 0x04005100 RID: 20736
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboAntiAliasingSharp;

	// Token: 0x04005101 RID: 20737
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboTextureQuality;

	// Token: 0x04005102 RID: 20738
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboTextureFilter;

	// Token: 0x04005103 RID: 20739
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboReflectionQuality;

	// Token: 0x04005104 RID: 20740
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboReflectedShadows;

	// Token: 0x04005105 RID: 20741
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboWaterQuality;

	// Token: 0x04005106 RID: 20742
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboViewDistance;

	// Token: 0x04005107 RID: 20743
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboLODDistance;

	// Token: 0x04005108 RID: 20744
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboShadowsDistance;

	// Token: 0x04005109 RID: 20745
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboShadowsQuality;

	// Token: 0x0400510A RID: 20746
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboTerrainQuality;

	// Token: 0x0400510B RID: 20747
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboObjectQuality;

	// Token: 0x0400510C RID: 20748
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboGrassDistance;

	// Token: 0x0400510D RID: 20749
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboOcclusion;

	// Token: 0x0400510E RID: 20750
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboDymeshEnabled;

	// Token: 0x0400510F RID: 20751
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<int> comboDymeshDistance;

	// Token: 0x04005110 RID: 20752
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboDymeshHighQualityMesh;

	// Token: 0x04005111 RID: 20753
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<int> comboDymeshMaxRegions;

	// Token: 0x04005112 RID: 20754
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<int> comboDymeshMaxMesh;

	// Token: 0x04005113 RID: 20755
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboDymeshLandClaimOnly;

	// Token: 0x04005114 RID: 20756
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<int> comboDymeshLandClaimBuffer;

	// Token: 0x04005115 RID: 20757
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboBloom;

	// Token: 0x04005116 RID: 20758
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboDepthOfField;

	// Token: 0x04005117 RID: 20759
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboMotionBlur;

	// Token: 0x04005118 RID: 20760
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboSSAO;

	// Token: 0x04005119 RID: 20761
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboSSReflections;

	// Token: 0x0400511A RID: 20762
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboSunShafts;

	// Token: 0x0400511B RID: 20763
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboParticles;

	// Token: 0x0400511C RID: 20764
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboUIBackgroundOpacity;

	// Token: 0x0400511D RID: 20765
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboUIForegroundOpacity;

	// Token: 0x0400511E RID: 20766
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboUiSize;

	// Token: 0x0400511F RID: 20767
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboScreenBounds;

	// Token: 0x04005120 RID: 20768
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboUiFpsScaling;

	// Token: 0x04005121 RID: 20769
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnBack;

	// Token: 0x04005122 RID: 20770
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnDefaults;

	// Token: 0x04005123 RID: 20771
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnApply;

	// Token: 0x04005124 RID: 20772
	[PublicizedFrom(EAccessModifier.Private)]
	public object[] previousSettings;

	// Token: 0x04005125 RID: 20773
	[PublicizedFrom(EAccessModifier.Private)]
	public int origLength_ReflectionQuality;

	// Token: 0x04005126 RID: 20774
	[PublicizedFrom(EAccessModifier.Private)]
	public int origLength_ShadowDistance;

	// Token: 0x04005127 RID: 20775
	[PublicizedFrom(EAccessModifier.Private)]
	public int origLength_ShadowQuality;

	// Token: 0x04005128 RID: 20776
	[PublicizedFrom(EAccessModifier.Private)]
	public int origLength_TerrainQuality;

	// Token: 0x04005129 RID: 20777
	[PublicizedFrom(EAccessModifier.Private)]
	public int origLength_ObjectQuality;

	// Token: 0x0400512A RID: 20778
	[PublicizedFrom(EAccessModifier.Private)]
	public int origLength_GrassDistance;

	// Token: 0x0400512B RID: 20779
	[PublicizedFrom(EAccessModifier.Private)]
	public int origLength_MotionBlur;

	// Token: 0x0400512C RID: 20780
	[PublicizedFrom(EAccessModifier.Private)]
	public int origLength_SSR;

	// Token: 0x0400512D RID: 20781
	[PublicizedFrom(EAccessModifier.Private)]
	public bool origDymeshEnabled;

	// Token: 0x02000D60 RID: 3424
	public struct ResolutionInfo : IComparable<XUiC_OptionsVideo.ResolutionInfo>, IEquatable<XUiC_OptionsVideo.ResolutionInfo>
	{
		// Token: 0x06006B13 RID: 27411 RVA: 0x002BC60C File Offset: 0x002BA80C
		[return: TupleElementNames(new string[]
		{
			"_aspectRatio",
			"_aspectRatioFactor",
			"_aspectRatioString"
		})]
		public static ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string> DimensionsToAspectRatio(int _width, int _height)
		{
			if (_height == 0)
			{
				return new ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string>(XUiC_OptionsVideo.ResolutionInfo.EAspectRatio.Unknown, 0f, "n/a");
			}
			int num = 1000 * _width / _height;
			if (num >= 1770)
			{
				if (num >= 3550)
				{
					if (num <= 3600)
					{
						return new ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string>(XUiC_OptionsVideo.ResolutionInfo.EAspectRatio.Aspect_32_9, 3.555f, "32:9");
					}
				}
				else if (num >= 1890)
				{
					if (num <= 1900)
					{
						return new ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string>(XUiC_OptionsVideo.ResolutionInfo.EAspectRatio.Aspect_17_9, 2.37f, "17:9");
					}
					if (num <= 2370)
					{
						if (num == 2333 || num == 2370)
						{
							return new ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string>(XUiC_OptionsVideo.ResolutionInfo.EAspectRatio.Aspect_21_9, 2.37f, "21:9");
						}
					}
					else
					{
						if (num == 2400)
						{
							return new ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string>(XUiC_OptionsVideo.ResolutionInfo.EAspectRatio.Aspect_24_10, 2.4f, "24:10");
						}
						if (num == 3200)
						{
							return new ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string>(XUiC_OptionsVideo.ResolutionInfo.EAspectRatio.Aspect_32_10, 3.2f, "32:10");
						}
					}
				}
				else if (num <= 1780)
				{
					return new ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string>(XUiC_OptionsVideo.ResolutionInfo.EAspectRatio.Aspect_16_9, 1.777f, "16:9");
				}
			}
			else if (num >= 1560)
			{
				if (num >= 1660)
				{
					if (num <= 1670)
					{
						return new ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string>(XUiC_OptionsVideo.ResolutionInfo.EAspectRatio.Aspect_5_3, 1.666f, "5:3");
					}
				}
				else
				{
					if (num <= 1570)
					{
						return new ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string>(XUiC_OptionsVideo.ResolutionInfo.EAspectRatio.Aspect_25_16, 1.562f, "25:16");
					}
					if (num == 1600)
					{
						return new ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string>(XUiC_OptionsVideo.ResolutionInfo.EAspectRatio.Aspect_16_10, 1.6f, "16:10");
					}
				}
			}
			else if (num >= 1330)
			{
				if (num <= 1340)
				{
					return new ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string>(XUiC_OptionsVideo.ResolutionInfo.EAspectRatio.Aspect_4_3, 1.333f, "4:3");
				}
				if (num == 1500)
				{
					return new ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string>(XUiC_OptionsVideo.ResolutionInfo.EAspectRatio.Aspect_3_2, 1.5f, "3:2");
				}
			}
			else
			{
				if (num == 1000)
				{
					return new ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string>(XUiC_OptionsVideo.ResolutionInfo.EAspectRatio.Aspect_1_1, 1f, "1:1");
				}
				if (num == 1250)
				{
					return new ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string>(XUiC_OptionsVideo.ResolutionInfo.EAspectRatio.Aspect_5_4, 1.25f, "5:4");
				}
			}
			float num2 = (float)_width / (float)_height;
			return new ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string>(XUiC_OptionsVideo.ResolutionInfo.EAspectRatio.Unknown, num2, num2.ToCultureInvariantString("0.##") + ":1");
		}

		// Token: 0x06006B14 RID: 27412 RVA: 0x002BC838 File Offset: 0x002BAA38
		public ResolutionInfo(int _width, int _height)
		{
			this.Width = _width;
			if (_height > _width)
			{
				_height = _width;
			}
			this.Height = _height;
			ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string> valueTuple = XUiC_OptionsVideo.ResolutionInfo.DimensionsToAspectRatio(_width, _height);
			this.AspectRatio = valueTuple.Item1;
			string item = valueTuple.Item3;
			this.Label = string.Concat(new string[]
			{
				_width.ToString(),
				"x",
				_height.ToString(),
				" (",
				item,
				")"
			});
		}

		// Token: 0x06006B15 RID: 27413 RVA: 0x002BC8B8 File Offset: 0x002BAAB8
		public int CompareTo(XUiC_OptionsVideo.ResolutionInfo _other)
		{
			int num = this.Width.CompareTo(_other.Width);
			if (num == 0)
			{
				return this.Height.CompareTo(_other.Height);
			}
			return num;
		}

		// Token: 0x06006B16 RID: 27414 RVA: 0x002BC8F3 File Offset: 0x002BAAF3
		public bool Equals(XUiC_OptionsVideo.ResolutionInfo _other)
		{
			return this.Width == _other.Width && this.Height == _other.Height;
		}

		// Token: 0x06006B17 RID: 27415 RVA: 0x002BC913 File Offset: 0x002BAB13
		public override string ToString()
		{
			return this.Label;
		}

		// Token: 0x0400512E RID: 20782
		public readonly int Width;

		// Token: 0x0400512F RID: 20783
		public readonly int Height;

		// Token: 0x04005130 RID: 20784
		public readonly XUiC_OptionsVideo.ResolutionInfo.EAspectRatio AspectRatio;

		// Token: 0x04005131 RID: 20785
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string Label;

		// Token: 0x02000D61 RID: 3425
		public enum EAspectRatio
		{
			// Token: 0x04005133 RID: 20787
			Aspect_32_9,
			// Token: 0x04005134 RID: 20788
			Aspect_32_10,
			// Token: 0x04005135 RID: 20789
			Aspect_24_10,
			// Token: 0x04005136 RID: 20790
			Aspect_21_9,
			// Token: 0x04005137 RID: 20791
			Aspect_17_9,
			// Token: 0x04005138 RID: 20792
			Aspect_16_9,
			// Token: 0x04005139 RID: 20793
			Aspect_5_3,
			// Token: 0x0400513A RID: 20794
			Aspect_16_10,
			// Token: 0x0400513B RID: 20795
			Aspect_25_16,
			// Token: 0x0400513C RID: 20796
			Aspect_3_2,
			// Token: 0x0400513D RID: 20797
			Aspect_4_3,
			// Token: 0x0400513E RID: 20798
			Aspect_5_4,
			// Token: 0x0400513F RID: 20799
			Aspect_1_1,
			// Token: 0x04005140 RID: 20800
			Unknown
		}
	}

	// Token: 0x02000D62 RID: 3426
	public enum UpscalerMode
	{
		// Token: 0x04005142 RID: 20802
		Off,
		// Token: 0x04005143 RID: 20803
		Dynamic,
		// Token: 0x04005144 RID: 20804
		Scale,
		// Token: 0x04005145 RID: 20805
		FSR3,
		// Token: 0x04005146 RID: 20806
		DLSS
	}

	// Token: 0x02000D63 RID: 3427
	public enum GraphicsMode
	{
		// Token: 0x04005148 RID: 20808
		Lowest,
		// Token: 0x04005149 RID: 20809
		Low,
		// Token: 0x0400514A RID: 20810
		Medium,
		// Token: 0x0400514B RID: 20811
		High,
		// Token: 0x0400514C RID: 20812
		Ultra,
		// Token: 0x0400514D RID: 20813
		Custom,
		// Token: 0x0400514E RID: 20814
		ConsolePerformance,
		// Token: 0x0400514F RID: 20815
		ConsoleQuality
	}
}
