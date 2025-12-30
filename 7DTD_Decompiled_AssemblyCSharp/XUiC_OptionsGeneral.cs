using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000D51 RID: 3409
[Preserve]
public class XUiC_OptionsGeneral : XUiController
{
	// Token: 0x140000AF RID: 175
	// (add) Token: 0x06006A68 RID: 27240 RVA: 0x002B5308 File Offset: 0x002B3508
	// (remove) Token: 0x06006A69 RID: 27241 RVA: 0x002B533C File Offset: 0x002B353C
	public static event Action OnSettingsChanged;

	// Token: 0x06006A6A RID: 27242 RVA: 0x002B5370 File Offset: 0x002B3570
	public override void Init()
	{
		base.Init();
		XUiC_OptionsGeneral.ID = base.WindowGroup.ID;
		this.comboLanguage = base.GetChildById("Language").GetChildByType<XUiC_ComboBoxList<XUiC_OptionsGeneral.LanguageInfo>>();
		this.comboUseEnglishCompass = base.GetChildById("UseEnglishCompass").GetChildByType<XUiC_ComboBoxBool>();
		this.comboTempCelsius = base.GetChildById("TempCelsius").GetChildByType<XUiC_ComboBoxBool>();
		this.comboDisableXmlEvents = base.GetChildById("DisableXmlEvents").GetChildByType<XUiC_ComboBoxBool>();
		this.comboQuestsAutoShare = base.GetChildById("QuestsAutoShare").GetChildByType<XUiC_ComboBoxBool>();
		this.comboQuestsAutoAccept = base.GetChildById("QuestsAutoAccept").GetChildByType<XUiC_ComboBoxBool>();
		this.comboLnlMtuWorkaround = base.GetChildById("LnlMtuWorkaround").GetChildByType<XUiC_ComboBoxBool>();
		this.comboAutoParty = base.GetChildById("AutoParty").GetChildByType<XUiC_ComboBoxBool>();
		this.comboTxtChat = base.GetChildById("ChatComms").GetChildByType<XUiC_ComboBoxBool>();
		this.comboCrossplay = base.GetChildById("Crossplay").GetChildByType<XUiC_ComboBoxBool>();
		this.comboShowConsoleButton = base.GetChildById("ShowConsoleButton").GetChildByType<XUiC_ComboBoxBool>();
		this.comboLanguage.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboUseEnglishCompass.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboTempCelsius.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboDisableXmlEvents.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboQuestsAutoShare.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboQuestsAutoAccept.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboLnlMtuWorkaround.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboAutoParty.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboTxtChat.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboCrossplay.OnValueChangedGeneric += this.anyOtherValueChanged;
		this.comboShowConsoleButton.OnValueChangedGeneric += this.anyOtherValueChanged;
		((XUiC_SimpleButton)base.GetChildById("btnEula")).OnPressed += this.BtnEula_OnPressed;
		((XUiC_SimpleButton)base.GetChildById("btnBugReport")).OnPressed += this.BtnBugReport_OnPressed;
		((XUiC_SimpleButton)base.GetChildById("btnBack")).OnPressed += this.BtnBack_OnPressed;
		((XUiC_SimpleButton)base.GetChildById("btnDefaults")).OnPressed += this.BtnDefaults_OnOnPressed;
		this.btnApply = (XUiC_SimpleButton)base.GetChildById("btnApply");
		this.btnApply.OnPressed += this.BtnApply_OnPressed;
		this.RefreshApplyLabel();
		base.RegisterForInputStyleChanges();
	}

	// Token: 0x06006A6B RID: 27243 RVA: 0x002B563F File Offset: 0x002B383F
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshApplyLabel()
	{
		InControlExtensions.SetApplyButtonString(this.btnApply, "xuiApply");
	}

	// Token: 0x06006A6C RID: 27244 RVA: 0x002B5651 File Offset: 0x002B3851
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InputStyleChanged(PlayerInputManager.InputStyle _oldStyle, PlayerInputManager.InputStyle _newStyle)
	{
		base.InputStyleChanged(_oldStyle, _newStyle);
		this.RefreshApplyLabel();
	}

	// Token: 0x06006A6D RID: 27245 RVA: 0x002B5661 File Offset: 0x002B3861
	[PublicizedFrom(EAccessModifier.Protected)]
	public void anyOtherValueChanged(XUiController _sender)
	{
		this.btnApply.Enabled = true;
	}

	// Token: 0x06006A6E RID: 27246 RVA: 0x002B566F File Offset: 0x002B386F
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnApply_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.applyChanges();
		this.btnApply.Enabled = false;
	}

	// Token: 0x06006A6F RID: 27247 RVA: 0x002B5684 File Offset: 0x002B3884
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDefaults_OnOnPressed(XUiController _sender, int _mouseButton)
	{
		this.comboLanguage.SelectedIndex = 0;
		this.comboUseEnglishCompass.Value = (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsUiCompassUseEnglishCardinalDirections);
		this.comboTempCelsius.Value = (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsTempCelsius);
		this.comboDisableXmlEvents.Value = (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsDisableXmlEvents);
		this.comboQuestsAutoShare.Value = (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsQuestsAutoShare);
		this.comboQuestsAutoAccept.Value = (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsQuestsAutoAccept);
		this.comboLnlMtuWorkaround.Value = (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsLiteNetLibMtuOverride);
		this.comboShowConsoleButton.Value = (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsShowConsoleButton);
		this.comboAutoParty.Value = (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsAutoPartyWithFriends);
		this.comboTxtChat.Value = (this.otherPerms.HasCommunication() && (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsChatCommunication));
		this.comboCrossplay.Value = (this.otherPerms.HasCrossplay() && (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsCrossplay));
		this.btnApply.Enabled = true;
	}

	// Token: 0x06006A70 RID: 27248 RVA: 0x002A30F5 File Offset: 0x002A12F5
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnBack_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		base.xui.playerUI.windowManager.Open(XUiC_OptionsMenu.ID, true, false, true);
	}

	// Token: 0x06006A71 RID: 27249 RVA: 0x002B57CD File Offset: 0x002B39CD
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnEula_OnPressed(XUiController _sender, int _mouseButton)
	{
		XUiC_EulaWindow.Open(base.xui, GameManager.HasAcceptedLatestEula());
	}

	// Token: 0x06006A72 RID: 27250 RVA: 0x002B57DF File Offset: 0x002B39DF
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnBugReport_OnPressed(XUiController _sender, int _mouseButton)
	{
		XUiC_BugReportWindow.Open(base.xui, true);
	}

	// Token: 0x06006A73 RID: 27251 RVA: 0x002B57F0 File Offset: 0x002B39F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateLanguageList()
	{
		this.comboLanguage.Elements.Clear();
		bool flag = false;
		foreach (string text in Localization.knownLanguages)
		{
			if ((flag || (flag = text.EqualsCaseInsensitive(Localization.DefaultLanguage))) && text.IndexOf(' ') < 0)
			{
				this.comboLanguage.Elements.Add(new XUiC_OptionsGeneral.LanguageInfo(text));
			}
		}
		this.comboLanguage.Elements.Sort();
		this.comboLanguage.Elements.Insert(0, new XUiC_OptionsGeneral.LanguageInfo(""));
		string @string = GamePrefs.GetString(EnumGamePrefs.Language);
		for (int j = 0; j < this.comboLanguage.Elements.Count; j++)
		{
			if (this.comboLanguage.Elements[j].LanguageKey.EqualsCaseInsensitive(@string))
			{
				this.comboLanguage.SelectedIndex = j;
			}
		}
	}

	// Token: 0x06006A74 RID: 27252 RVA: 0x002B58E0 File Offset: 0x002B3AE0
	[PublicizedFrom(EAccessModifier.Private)]
	public void applyChanges()
	{
		bool flag = this.comboLanguage.Value.LanguageKey != GamePrefs.GetString(EnumGamePrefs.Language);
		if (flag)
		{
			Log.Out("Language selection changed: " + this.comboLanguage.Value.LanguageKey);
			GamePrefs.Set(EnumGamePrefs.Language, this.comboLanguage.Value.LanguageKey);
		}
		GamePrefs.Set(EnumGamePrefs.OptionsUiCompassUseEnglishCardinalDirections, this.comboUseEnglishCompass.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsTempCelsius, this.comboTempCelsius.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsDisableXmlEvents, this.comboDisableXmlEvents.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsQuestsAutoShare, this.comboQuestsAutoShare.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsQuestsAutoAccept, this.comboQuestsAutoAccept.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsLiteNetLibMtuOverride, this.comboLnlMtuWorkaround.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsShowConsoleButton, this.comboShowConsoleButton.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsAutoPartyWithFriends, this.comboAutoParty.Value);
		if (this.otherPerms.HasCommunication())
		{
			GamePrefs.Set(EnumGamePrefs.OptionsChatCommunication, this.comboTxtChat.Value);
		}
		if (this.otherPerms.HasCrossplay())
		{
			GamePrefs.Set(EnumGamePrefs.OptionsCrossplay, this.comboCrossplay.Value);
		}
		GamePrefs.Instance.Save();
		Action onSettingsChanged = XUiC_OptionsGeneral.OnSettingsChanged;
		if (onSettingsChanged != null)
		{
			onSettingsChanged();
		}
		if (flag)
		{
			if ((DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX).IsCurrent())
			{
				XUiC_MessageBoxWindowGroup.ShowMessageBox(base.xui, Localization.Get("xuiConfirmRestartLanguageTitle", false), Localization.Get("xuiConfirmRestartLanguageText", false), XUiC_MessageBoxWindowGroup.MessageBoxTypes.OkCancel, delegate()
				{
					Utils.RestartGame(Utils.ERestartAntiCheatMode.KeepAntiCheatMode);
				}, delegate()
				{
					base.xui.playerUI.windowManager.Open(XUiC_OptionsGeneral.ID, true, false, true);
				}, false, true, true);
				return;
			}
			if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
			{
				XUiC_MessageBoxWindowGroup.ShowMessageBox(base.xui, Localization.Get("xuiLanguageChangedTitle", false), Localization.Get("xuiLanguageChangedText", false), XUiC_MessageBoxWindowGroup.MessageBoxTypes.Ok, null, delegate()
				{
					base.xui.playerUI.windowManager.Open(XUiC_OptionsGeneral.ID, true, false, true);
				}, false, true, true);
			}
		}
	}

	// Token: 0x06006A75 RID: 27253 RVA: 0x002B5AE8 File Offset: 0x002B3CE8
	public override void OnOpen()
	{
		this.otherPerms = PermissionsManager.GetPermissions(PermissionsManager.PermissionSources.Platform | PermissionsManager.PermissionSources.LaunchPrefs | PermissionsManager.PermissionSources.DebugMask | PermissionsManager.PermissionSources.TitleStorage);
		base.WindowGroup.openWindowOnEsc = XUiC_OptionsMenu.ID;
		this.updateLanguageList();
		this.comboUseEnglishCompass.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsUiCompassUseEnglishCardinalDirections);
		this.comboTempCelsius.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsTempCelsius);
		this.comboDisableXmlEvents.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsDisableXmlEvents);
		this.comboQuestsAutoShare.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsQuestsAutoShare);
		this.comboQuestsAutoAccept.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsQuestsAutoAccept);
		this.comboLnlMtuWorkaround.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsLiteNetLibMtuOverride);
		this.comboShowConsoleButton.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsShowConsoleButton);
		this.comboAutoParty.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsAutoPartyWithFriends);
		this.comboTxtChat.Value = (this.otherPerms.HasCommunication() && GamePrefs.GetBool(EnumGamePrefs.OptionsChatCommunication));
		this.comboTxtChat.Enabled = this.otherPerms.HasCommunication();
		this.comboCrossplay.Value = (this.otherPerms.HasCrossplay() && GamePrefs.GetBool(EnumGamePrefs.OptionsCrossplay));
		this.comboCrossplay.Enabled = this.otherPerms.HasCrossplay();
		base.OnOpen();
		this.btnApply.Enabled = false;
		base.RefreshBindings(false);
	}

	// Token: 0x06006A76 RID: 27254 RVA: 0x002B5C4F File Offset: 0x002B3E4F
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.btnApply.Enabled && base.xui.playerUI.playerInput.GUIActions.Apply.WasPressed)
		{
			this.BtnApply_OnPressed(null, 0);
		}
	}

	// Token: 0x06006A77 RID: 27255 RVA: 0x002B5C90 File Offset: 0x002B3E90
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "crossplayTooltip")
		{
			_value = (PermissionsManager.GetPermissionDenyReason(EUserPerms.Crossplay, PermissionsManager.PermissionSources.Platform | PermissionsManager.PermissionSources.LaunchPrefs | PermissionsManager.PermissionSources.DebugMask | PermissionsManager.PermissionSources.TitleStorage) ?? Localization.Get("xuiOptionsGeneralCrossplayTooltip", false));
			return true;
		}
		if (!(_bindingName == "bug_reporting"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = BacktraceUtils.BugReportFeature.ToString();
		return true;
	}

	// Token: 0x04005044 RID: 20548
	public static string ID = "";

	// Token: 0x04005046 RID: 20550
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<XUiC_OptionsGeneral.LanguageInfo> comboLanguage;

	// Token: 0x04005047 RID: 20551
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboUseEnglishCompass;

	// Token: 0x04005048 RID: 20552
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboTempCelsius;

	// Token: 0x04005049 RID: 20553
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboDisableXmlEvents;

	// Token: 0x0400504A RID: 20554
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboQuestsAutoShare;

	// Token: 0x0400504B RID: 20555
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboQuestsAutoAccept;

	// Token: 0x0400504C RID: 20556
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboLnlMtuWorkaround;

	// Token: 0x0400504D RID: 20557
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboAutoParty;

	// Token: 0x0400504E RID: 20558
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboTxtChat;

	// Token: 0x0400504F RID: 20559
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboCrossplay;

	// Token: 0x04005050 RID: 20560
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboShowConsoleButton;

	// Token: 0x04005051 RID: 20561
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnApply;

	// Token: 0x04005052 RID: 20562
	[PublicizedFrom(EAccessModifier.Private)]
	public EUserPerms otherPerms;

	// Token: 0x02000D52 RID: 3410
	public struct LanguageInfo : IComparable<XUiC_OptionsGeneral.LanguageInfo>, IEquatable<XUiC_OptionsGeneral.LanguageInfo>, IComparable
	{
		// Token: 0x06006A7C RID: 27260 RVA: 0x002B5D18 File Offset: 0x002B3F18
		public LanguageInfo(string _languageKey)
		{
			this.LanguageKey = _languageKey;
			if (_languageKey == "")
			{
				this.NameEnglish = null;
				this.NameNative = null;
				return;
			}
			this.NameEnglish = Localization.Get("languageNameEnglish", _languageKey, false);
			this.NameNative = Localization.Get("languageNameNative", _languageKey, false);
		}

		// Token: 0x06006A7D RID: 27261 RVA: 0x002B5D6C File Offset: 0x002B3F6C
		public override string ToString()
		{
			if (!(this.LanguageKey == ""))
			{
				return this.NameEnglish + " / " + this.NameNative;
			}
			return "-Auto-";
		}

		// Token: 0x06006A7E RID: 27262 RVA: 0x002B5D9C File Offset: 0x002B3F9C
		public bool Equals(XUiC_OptionsGeneral.LanguageInfo _other)
		{
			return this.LanguageKey == _other.LanguageKey;
		}

		// Token: 0x06006A7F RID: 27263 RVA: 0x002B5DB0 File Offset: 0x002B3FB0
		public override bool Equals(object _obj)
		{
			if (_obj is XUiC_OptionsGeneral.LanguageInfo)
			{
				XUiC_OptionsGeneral.LanguageInfo other = (XUiC_OptionsGeneral.LanguageInfo)_obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06006A80 RID: 27264 RVA: 0x002B5DD5 File Offset: 0x002B3FD5
		public override int GetHashCode()
		{
			if (this.LanguageKey == null)
			{
				return 0;
			}
			return this.LanguageKey.GetHashCode();
		}

		// Token: 0x06006A81 RID: 27265 RVA: 0x002B5DEC File Offset: 0x002B3FEC
		public int CompareTo(XUiC_OptionsGeneral.LanguageInfo _other)
		{
			return string.Compare(this.NameEnglish, _other.NameEnglish, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x06006A82 RID: 27266 RVA: 0x002B5E00 File Offset: 0x002B4000
		public int CompareTo(object _obj)
		{
			if (_obj == null)
			{
				return 1;
			}
			if (_obj is XUiC_OptionsGeneral.LanguageInfo)
			{
				XUiC_OptionsGeneral.LanguageInfo other = (XUiC_OptionsGeneral.LanguageInfo)_obj;
				return this.CompareTo(other);
			}
			throw new ArgumentException("Object must be of type LanguageInfo");
		}

		// Token: 0x04005053 RID: 20563
		public readonly string NameEnglish;

		// Token: 0x04005054 RID: 20564
		public readonly string NameNative;

		// Token: 0x04005055 RID: 20565
		public readonly string LanguageKey;
	}
}
