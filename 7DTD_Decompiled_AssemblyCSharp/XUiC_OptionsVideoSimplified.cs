using System;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D64 RID: 3428
[Preserve]
public class XUiC_OptionsVideoSimplified : XUiController
{
	// Token: 0x140000B3 RID: 179
	// (add) Token: 0x06006B18 RID: 27416 RVA: 0x002BC91C File Offset: 0x002BAB1C
	// (remove) Token: 0x06006B19 RID: 27417 RVA: 0x002BC950 File Offset: 0x002BAB50
	public static event Action OnSettingsChanged;

	// Token: 0x06006B1A RID: 27418 RVA: 0x002BC984 File Offset: 0x002BAB84
	public override void Init()
	{
		base.Init();
		XUiC_OptionsVideoSimplified.ID = base.WindowGroup.ID;
		this.tabs = base.GetChildByType<XUiC_TabSelector>();
		this.tabs.OnTabChanged += this.TabSelector_OnTabChanged;
		this.comboBrightness = base.GetChildById("Brightness").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboBrightness.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.btnDefaultBrightness = base.GetChildById("btnDefaultBrightness").GetChildByType<XUiC_SimpleButton>();
		this.btnDefaultBrightness.OnPressed += this.BtnDefaultBrightness_OnPressed;
		this.comboFieldOfView = base.GetChildById("FieldOfViewSimplified").GetChildByType<XUiC_ComboBoxInt>();
		this.comboFieldOfView.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboFieldOfView.Min = (long)Constants.cMinCameraFieldOfView;
		this.comboFieldOfView.Max = (long)Constants.cMaxCameraFieldOfView;
		this.btnDefaultFOV = base.GetChildById("btnDefaultFOV").GetChildByType<XUiC_SimpleButton>();
		this.btnDefaultFOV.OnPressed += this.BtnDefaultFOV_OnPressed;
		this.comboMotionBlur = base.GetChildById("MotionBlurToggle").GetChildByType<XUiC_ComboBoxBool>();
		this.comboMotionBlur.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboGraphicsMode = base.GetChildById("GraphicsMode").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboGraphicsMode.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboUpscaler = base.GetChildById("ConsoleUpscaler").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboUpscaler.OnValueChangedGeneric += this.AnyPresetValueChanged;
		this.comboUIBackgroundOpacity = base.GetChildById("UIBackgroundOpacity").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboUIForegroundOpacity = base.GetChildById("UIForegroundOpacity").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboUiSize = base.GetChildById("UiSize").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboScreenBounds = base.GetChildById("ScreenBounds").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboUIBackgroundOpacity.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboUIForegroundOpacity.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboUiSize.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboScreenBounds.OnValueChangedGeneric += this.anyOtherValueChanged;
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
		this.btnBack = (base.GetChildById("btnBack") as XUiC_SimpleButton);
		this.btnDefaults = (base.GetChildById("btnDefaults") as XUiC_SimpleButton);
		this.btnApply = (base.GetChildById("btnApply") as XUiC_SimpleButton);
		this.btnBack.OnPressed += this.BtnBack_OnPressed;
		this.btnDefaults.OnPressed += this.BtnDefaults_OnOnPressed;
		this.btnApply.OnPressed += this.BtnApply_OnPressed;
		this.btnApply.Text = "[action:gui:GUI Apply] " + Localization.Get("xuiApply", false).ToUpper();
	}

	// Token: 0x06006B1B RID: 27419 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Protected)]
	public void TabSelector_OnTabChanged(int _i, XUiC_TabSelectorTab _tab)
	{
		this.IsDirty = true;
	}

	// Token: 0x06006B1C RID: 27420 RVA: 0x002BCD49 File Offset: 0x002BAF49
	[PublicizedFrom(EAccessModifier.Protected)]
	public void anyOtherValueChanged(XUiController _sender)
	{
		this.btnApply.Enabled = true;
	}

	// Token: 0x06006B1D RID: 27421 RVA: 0x002BCD57 File Offset: 0x002BAF57
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDefaultBrightness_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.comboBrightness.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsGfxBrightness));
		this.btnApply.Enabled = true;
	}

	// Token: 0x06006B1E RID: 27422 RVA: 0x002BCD80 File Offset: 0x002BAF80
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDefaultFOV_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.comboFieldOfView.Value = (long)((int)GamePrefs.GetDefault(EnumGamePrefs.OptionsGfxFOV));
		this.btnApply.Enabled = true;
	}

	// Token: 0x06006B1F RID: 27423 RVA: 0x002BCDA6 File Offset: 0x002BAFA6
	[PublicizedFrom(EAccessModifier.Protected)]
	public void BtnApply_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.applyChanges();
	}

	// Token: 0x06006B20 RID: 27424 RVA: 0x002BCDB0 File Offset: 0x002BAFB0
	[PublicizedFrom(EAccessModifier.Protected)]
	public void BtnDefaults_OnOnPressed(XUiController _sender, int _mouseButton)
	{
		this.comboUIBackgroundOpacity.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsBackgroundGlobalOpacity));
		this.comboUIForegroundOpacity.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsForegroundGlobalOpacity));
		this.comboUiSize.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsHudSize));
		this.comboScreenBounds.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsScreenBoundsValue));
		this.btnApply.Enabled = true;
	}

	// Token: 0x06006B21 RID: 27425 RVA: 0x002A30F5 File Offset: 0x002A12F5
	[PublicizedFrom(EAccessModifier.Protected)]
	public void BtnBack_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		base.xui.playerUI.windowManager.Open(XUiC_OptionsMenu.ID, true, false, true);
	}

	// Token: 0x06006B22 RID: 27426 RVA: 0x002BCD49 File Offset: 0x002BAF49
	[PublicizedFrom(EAccessModifier.Protected)]
	public void AnyPresetValueChanged(XUiController _sender)
	{
		this.btnApply.Enabled = true;
	}

	// Token: 0x06006B23 RID: 27427 RVA: 0x002BCE38 File Offset: 0x002BB038
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateGraphicOptions()
	{
		this.comboBrightness.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxBrightness);
		this.comboFieldOfView.Value = (long)GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFOV);
		this.comboMotionBlur.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsGfxMotionBlurEnabled);
		int selectedIndex = (int)XUiC_OptionsVideoSimplified.QualityPresetToGraphicsMode(GamePrefs.GetInt(EnumGamePrefs.OptionsGfxQualityPreset));
		this.UpdateCustomModeVisibility(selectedIndex);
		this.comboGraphicsMode.SelectedIndex = selectedIndex;
		int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxUpscalerMode);
		int num;
		if (@int != 2)
		{
			if (@int != 4)
			{
				num = -1;
			}
			else
			{
				num = 1;
			}
		}
		else
		{
			num = 0;
		}
		int num2 = num;
		if (num2 == -1)
		{
			Log.Out(string.Format("Upscaler mode \"{0}\" is unsupported on this platform; defaulting to \"{1}\".", GamePrefs.GetInt(EnumGamePrefs.OptionsGfxUpscalerMode), 2));
			num2 = 0;
			GamePrefs.Set(EnumGamePrefs.OptionsGfxUpscalerMode, 2);
		}
		this.comboUpscaler.SelectedIndex = num2;
		this.comboUIBackgroundOpacity.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsBackgroundGlobalOpacity);
		this.comboUIForegroundOpacity.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsForegroundGlobalOpacity);
		this.comboUiSize.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsHudSize);
		this.comboScreenBounds.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsScreenBoundsValue);
	}

	// Token: 0x06006B24 RID: 27428 RVA: 0x002BCF60 File Offset: 0x002BB160
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateCustomModeVisibility(int selectedIndex)
	{
		this.comboGraphicsMode.MaxIndex = Math.Max(selectedIndex, 1);
	}

	// Token: 0x06006B25 RID: 27429 RVA: 0x002BCF74 File Offset: 0x002BB174
	[PublicizedFrom(EAccessModifier.Private)]
	public void applyChanges()
	{
		GamePrefs.Set(EnumGamePrefs.OptionsGfxBrightness, (float)this.comboBrightness.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxFOV, (int)this.comboFieldOfView.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxMotionBlurEnabled, this.comboMotionBlur.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxQualityPreset, XUiC_OptionsVideoSimplified.GraphicsModeToQualityPreset((XUiC_OptionsVideoSimplified.GraphicsMode)this.comboGraphicsMode.SelectedIndex));
		this.UpdateCustomModeVisibility(this.comboGraphicsMode.SelectedIndex);
		GameOptionsManager.SetGraphicsQuality();
		int i = this.comboUpscaler.SelectedIndex;
		int num;
		if (i != 0)
		{
			if (i != 1)
			{
				num = GameOptionsPlatforms.DefaultUpscalerMode;
			}
			else
			{
				num = 4;
			}
		}
		else
		{
			num = 2;
		}
		int value = num;
		GamePrefs.Set(EnumGamePrefs.OptionsGfxUpscalerMode, value);
		GamePrefs.Set(EnumGamePrefs.OptionsBackgroundGlobalOpacity, (float)this.comboUIBackgroundOpacity.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsForegroundGlobalOpacity, (float)this.comboUIForegroundOpacity.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsHudSize, (float)this.comboUiSize.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsScreenBoundsValue, (float)this.comboScreenBounds.Value);
		GamePrefs.Instance.Save();
		GameOptionsManager.ApplyAllOptions(base.xui.playerUI);
		foreach (XUi xui in UnityEngine.Object.FindObjectsOfType<XUi>())
		{
			xui.BackgroundGlobalOpacity = GamePrefs.GetFloat(EnumGamePrefs.OptionsBackgroundGlobalOpacity);
			xui.ForegroundGlobalOpacity = GamePrefs.GetFloat(EnumGamePrefs.OptionsForegroundGlobalOpacity);
		}
		Action onSettingsChanged = XUiC_OptionsVideoSimplified.OnSettingsChanged;
		if (onSettingsChanged != null)
		{
			onSettingsChanged();
		}
		this.previousSettings = GamePrefs.GetSettingsCopy();
		this.btnApply.Enabled = false;
	}

	// Token: 0x06006B26 RID: 27430 RVA: 0x002BD0F0 File Offset: 0x002BB2F0
	[PublicizedFrom(EAccessModifier.Private)]
	public static XUiC_OptionsVideoSimplified.GraphicsMode QualityPresetToGraphicsMode(int qualityPreset)
	{
		XUiC_OptionsVideoSimplified.GraphicsMode result;
		if (qualityPreset != 6)
		{
			if (qualityPreset != 8)
			{
				result = XUiC_OptionsVideoSimplified.GraphicsMode.ConsoleCustom;
			}
			else
			{
				result = XUiC_OptionsVideoSimplified.GraphicsMode.ConsoleQuality;
			}
		}
		else
		{
			result = XUiC_OptionsVideoSimplified.GraphicsMode.ConsolePerformance;
		}
		return result;
	}

	// Token: 0x06006B27 RID: 27431 RVA: 0x002BD114 File Offset: 0x002BB314
	[PublicizedFrom(EAccessModifier.Private)]
	public static int GraphicsModeToQualityPreset(XUiC_OptionsVideoSimplified.GraphicsMode graphicsMode)
	{
		int result;
		if (graphicsMode != XUiC_OptionsVideoSimplified.GraphicsMode.ConsolePerformance)
		{
			if (graphicsMode != XUiC_OptionsVideoSimplified.GraphicsMode.ConsoleQuality)
			{
				result = 5;
			}
			else
			{
				result = 8;
			}
		}
		else
		{
			result = 6;
		}
		return result;
	}

	// Token: 0x06006B28 RID: 27432 RVA: 0x002BD138 File Offset: 0x002BB338
	public override void OnOpen()
	{
		base.WindowGroup.openWindowOnEsc = XUiC_OptionsMenu.ID;
		this.previousSettings = GamePrefs.GetSettingsCopy();
		this.VSyncCountPref = PlatformApplicationManager.Application.VSyncCountPref;
		this.updateGraphicOptions();
		base.OnOpen();
		this.btnApply.Enabled = false;
	}

	// Token: 0x06006B29 RID: 27433 RVA: 0x002BD188 File Offset: 0x002BB388
	public override void OnClose()
	{
		GamePrefs.ApplySettingsCopy(this.previousSettings);
		base.OnClose();
	}

	// Token: 0x06006B2A RID: 27434 RVA: 0x002BD19C File Offset: 0x002BB39C
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

	// Token: 0x06006B2B RID: 27435 RVA: 0x002BD1FC File Offset: 0x002BB3FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "isTabUi")
		{
			_value = ((this.tabs != null && this.tabs.IsSelected("xuiOptionsVideoUI")) ? "true" : "false");
			return true;
		}
		if (_bindingName == "ui_size_limited")
		{
			float num = (float)GameOptionsManager.GetUiSizeLimit();
			float @float = GamePrefs.GetFloat(EnumGamePrefs.OptionsHudSize);
			_value = (@float > num).ToString();
			return true;
		}
		if (!(_bindingName == "ui_size_limit"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = GameOptionsManager.GetUiSizeLimit().ToCultureInvariantString();
		return true;
	}

	// Token: 0x04005150 RID: 20816
	public static string ID = "";

	// Token: 0x04005152 RID: 20818
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TabSelector tabs;

	// Token: 0x04005153 RID: 20819
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboBrightness;

	// Token: 0x04005154 RID: 20820
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnDefaultBrightness;

	// Token: 0x04005155 RID: 20821
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxInt comboFieldOfView;

	// Token: 0x04005156 RID: 20822
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnDefaultFOV;

	// Token: 0x04005157 RID: 20823
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboMotionBlur;

	// Token: 0x04005158 RID: 20824
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboGraphicsMode;

	// Token: 0x04005159 RID: 20825
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboUpscaler;

	// Token: 0x0400515A RID: 20826
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboUIBackgroundOpacity;

	// Token: 0x0400515B RID: 20827
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboUIForegroundOpacity;

	// Token: 0x0400515C RID: 20828
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboUiSize;

	// Token: 0x0400515D RID: 20829
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboScreenBounds;

	// Token: 0x0400515E RID: 20830
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnBack;

	// Token: 0x0400515F RID: 20831
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnDefaults;

	// Token: 0x04005160 RID: 20832
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnApply;

	// Token: 0x04005161 RID: 20833
	[PublicizedFrom(EAccessModifier.Private)]
	public object[] previousSettings;

	// Token: 0x04005162 RID: 20834
	[PublicizedFrom(EAccessModifier.Private)]
	public float previousRefreshRate = -1f;

	// Token: 0x04005163 RID: 20835
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumGamePrefs VSyncCountPref = EnumGamePrefs.OptionsGfxVsync;

	// Token: 0x02000D65 RID: 3429
	public enum GraphicsMode
	{
		// Token: 0x04005165 RID: 20837
		ConsolePerformance,
		// Token: 0x04005166 RID: 20838
		ConsoleQuality,
		// Token: 0x04005167 RID: 20839
		ConsoleCustom
	}

	// Token: 0x02000D66 RID: 3430
	public enum UpscalerMode
	{
		// Token: 0x04005169 RID: 20841
		FSR3,
		// Token: 0x0400516A RID: 20842
		Scale
	}
}
