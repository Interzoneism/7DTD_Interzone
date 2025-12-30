using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D41 RID: 3393
[Preserve]
public class XUiC_OptionsAudio : XUiController
{
	// Token: 0x140000AC RID: 172
	// (add) Token: 0x060069BA RID: 27066 RVA: 0x002AFA78 File Offset: 0x002ADC78
	// (remove) Token: 0x060069BB RID: 27067 RVA: 0x002AFAAC File Offset: 0x002ADCAC
	public static event Action OnSettingsChanged;

	// Token: 0x17000ABB RID: 2747
	// (get) Token: 0x060069BC RID: 27068 RVA: 0x002AFADF File Offset: 0x002ADCDF
	public static bool VoiceAvailable
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return PlatformManager.MultiPlatform.PartyVoice != null && PlatformManager.MultiPlatform.PartyVoice.Status == EPartyVoiceStatus.Ok;
		}
	}

	// Token: 0x17000ABC RID: 2748
	// (get) Token: 0x060069BD RID: 27069 RVA: 0x0024235F File Offset: 0x0024055F
	public static bool IsCommunicationAllowed
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return PermissionsManager.IsCommunicationAllowed();
		}
	}

	// Token: 0x060069BE RID: 27070 RVA: 0x002AFB04 File Offset: 0x002ADD04
	public override void Init()
	{
		base.Init();
		XUiC_OptionsAudio.ID = base.WindowGroup.ID;
		this.tabSelector = base.GetChildByType<XUiC_TabSelector>();
		this.comboOverallAudioVolumeLevel = base.GetChildById("OverallAudioVolumeLevel").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboDynamicMusicEnabled = base.GetChildById("DynamicMusicEnabled").GetChildByType<XUiC_ComboBoxBool>();
		this.comboDynamicMusicDailyTimeAllotted = base.GetChildById("DynamicMusicDailyTimeAllotted").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboAmbientVolumeLevel = base.GetChildById("AmbientVolumeLevel").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboInGameMusicVolumeLevel = base.GetChildById("InGameMusicVolumeLevel").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboMenuMusicVolumeLevel = base.GetChildById("MenuMusicVolumeLevel").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboSubtitlesEnabled = base.GetChildById("SubtitlesEnabled").GetChildByType<XUiC_ComboBoxBool>();
		this.comboProfanityFilter = base.GetChildById("ProfanityFilter").GetChildByType<XUiC_ComboBoxBool>();
		this.comboOverallAudioVolumeLevel.OnValueChanged += this.ComboOverallAudioVolumeLevelOnOnValueChanged;
		this.comboDynamicMusicEnabled.OnValueChanged += this.ComboDynamicMusicEnabledOnOnValueChanged;
		this.comboDynamicMusicDailyTimeAllotted.OnValueChanged += this.ComboDynamicMusicDailyTimeAllottedOnOnValueChanged;
		this.comboAmbientVolumeLevel.OnValueChanged += this.ComboAmbientVolumeLevelOnOnValueChanged;
		this.comboInGameMusicVolumeLevel.OnValueChanged += this.ComboInGameMusicVolumeLevelOnOnValueChanged;
		this.comboMenuMusicVolumeLevel.OnValueChanged += this.ComboMenuMusicVolumeLevelOnOnValueChanged;
		this.comboSubtitlesEnabled.OnValueChanged += this.ComboSubtitlesEnabledOnValueChanged;
		this.comboProfanityFilter.OnValueChanged += this.ComboSubtitlesEnabledOnValueChanged;
		if (!(DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
		{
			this.comboMumblePositionalAudioSupportEnabled = base.GetChildById("MumblePositionalAudioSupportEnabled").GetChildByType<XUiC_ComboBoxBool>();
			this.comboMumblePositionalAudioSupportEnabled.OnValueChanged += this.ComboMumblePositionalAudioSupportEnabledOnValueChanged;
		}
		if (XUiC_OptionsAudio.VoiceAvailable)
		{
			this.comboVoiceChatEnabled = base.GetChildById("VoiceChatEnabled").GetChildByType<XUiC_ComboBoxBool>();
			XUiController childById = base.GetChildById("VoiceOutputDevice");
			this.comboVoiceOutputDevice = ((childById != null) ? childById.GetChildByType<XUiC_ComboBoxList<IPartyVoice.VoiceAudioDevice>>() : null);
			XUiController childById2 = base.GetChildById("VoiceInputDevice");
			this.comboVoiceInputDevice = ((childById2 != null) ? childById2.GetChildByType<XUiC_ComboBoxList<IPartyVoice.VoiceAudioDevice>>() : null);
			this.comboVoiceVolumeLevel = base.GetChildById("VoiceVolumeLevel").GetChildByType<XUiC_ComboBoxFloat>();
			this.comboVoiceChatEnabled.OnValueChanged += this.ComboVoiceChatEnabledOnOnValueChanged;
			if (this.comboVoiceOutputDevice != null)
			{
				this.comboVoiceOutputDevice.OnValueChanged += this.ComboVoiceDeviceOnValueChanged;
			}
			if (this.comboVoiceInputDevice != null)
			{
				this.comboVoiceInputDevice.OnValueChanged += this.ComboVoiceDeviceOnValueChanged;
			}
			this.comboVoiceVolumeLevel.OnValueChanged += this.ComboVoiceVolumeLevelOnOnValueChanged;
		}
		((XUiC_SimpleButton)base.GetChildById("btnBack")).OnPressed += this.BtnBack_OnPressed;
		((XUiC_SimpleButton)base.GetChildById("btnDefaults")).OnPressed += this.BtnDefaults_OnOnPressed;
		this.btnApply = (XUiC_SimpleButton)base.GetChildById("btnApply");
		this.btnApply.OnPressed += this.BtnApply_OnPressed;
		base.RegisterForInputStyleChanges();
		this.RefreshApplyLabel();
		this.initDiscord();
	}

	// Token: 0x060069BF RID: 27071 RVA: 0x002AFE2E File Offset: 0x002AE02E
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshApplyLabel()
	{
		InControlExtensions.SetApplyButtonString(this.btnApply, "xuiApply");
	}

	// Token: 0x060069C0 RID: 27072 RVA: 0x002AFE40 File Offset: 0x002AE040
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InputStyleChanged(PlayerInputManager.InputStyle _oldStyle, PlayerInputManager.InputStyle _newStyle)
	{
		base.InputStyleChanged(_oldStyle, _newStyle);
		this.RefreshApplyLabel();
	}

	// Token: 0x060069C1 RID: 27073 RVA: 0x002AFE50 File Offset: 0x002AE050
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboOverallAudioVolumeLevelOnOnValueChanged(XUiController _sender, double _oldValue, double _newValue)
	{
		GamePrefs.Set(EnumGamePrefs.OptionsOverallAudioVolumeLevel, (float)this.comboOverallAudioVolumeLevel.Value);
		AudioListener.volume = GamePrefs.GetFloat(EnumGamePrefs.OptionsOverallAudioVolumeLevel);
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069C2 RID: 27074 RVA: 0x002AFE7B File Offset: 0x002AE07B
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboDynamicMusicEnabledOnOnValueChanged(XUiController _sender, bool _oldValue, bool _newValue)
	{
		GamePrefs.Set(EnumGamePrefs.OptionsDynamicMusicEnabled, this.comboDynamicMusicEnabled.Value);
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069C3 RID: 27075 RVA: 0x002AFE9E File Offset: 0x002AE09E
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboDynamicMusicDailyTimeAllottedOnOnValueChanged(XUiController _sender, double _oldValue, double _newValue)
	{
		GamePrefs.Set(EnumGamePrefs.OptionsDynamicMusicDailyTime, (float)this.comboDynamicMusicDailyTimeAllotted.Value);
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069C4 RID: 27076 RVA: 0x002AFEC2 File Offset: 0x002AE0C2
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboAmbientVolumeLevelOnOnValueChanged(XUiController _sender, double _oldValue, double _newValue)
	{
		GamePrefs.Set(EnumGamePrefs.OptionsAmbientVolumeLevel, (float)this.comboAmbientVolumeLevel.Value);
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069C5 RID: 27077 RVA: 0x002AFEE2 File Offset: 0x002AE0E2
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboInGameMusicVolumeLevelOnOnValueChanged(XUiController _sender, double _oldValue, double _newValue)
	{
		GamePrefs.Set(EnumGamePrefs.OptionsMusicVolumeLevel, (float)this.comboInGameMusicVolumeLevel.Value);
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069C6 RID: 27078 RVA: 0x002AFF02 File Offset: 0x002AE102
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboMenuMusicVolumeLevelOnOnValueChanged(XUiController _sender, double _oldValue, double _newValue)
	{
		GamePrefs.Set(EnumGamePrefs.OptionsMenuMusicVolumeLevel, (float)this.comboMenuMusicVolumeLevel.Value);
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069C7 RID: 27079 RVA: 0x002AFF22 File Offset: 0x002AE122
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboMumblePositionalAudioSupportEnabledOnValueChanged(XUiController _sender, bool _oldvalue, bool _newvalue)
	{
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069C8 RID: 27080 RVA: 0x002AFF30 File Offset: 0x002AE130
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboVoiceChatEnabledOnOnValueChanged(XUiController _sender, bool _oldValue, bool _newValue)
	{
		GamePrefs.Set(EnumGamePrefs.OptionsVoiceChatEnabled, this.comboVoiceChatEnabled.Value);
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069C9 RID: 27081 RVA: 0x002AFF22 File Offset: 0x002AE122
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboVoiceDeviceOnValueChanged(XUiController _sender, IPartyVoice.VoiceAudioDevice _oldvalue, IPartyVoice.VoiceAudioDevice _newvalue)
	{
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069CA RID: 27082 RVA: 0x002AFF50 File Offset: 0x002AE150
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboVoiceVolumeLevelOnOnValueChanged(XUiController _sender, double _oldValue, double _newValue)
	{
		GamePrefs.Set(EnumGamePrefs.OptionsVoiceVolumeLevel, (float)this.comboVoiceVolumeLevel.Value);
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069CB RID: 27083 RVA: 0x002AFF22 File Offset: 0x002AE122
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboSubtitlesEnabledOnValueChanged(XUiController _sender, bool _oldValue, bool _newValue)
	{
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069CC RID: 27084 RVA: 0x002AFF71 File Offset: 0x002AE171
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnApply_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.applyChanges(false);
		this.btnApply.Enabled = false;
	}

	// Token: 0x060069CD RID: 27085 RVA: 0x002AFF86 File Offset: 0x002AE186
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDefaults_OnOnPressed(XUiController _sender, int _mouseButton)
	{
		GameOptionsManager.ResetGameOptions(GameOptionsManager.ResetType.Audio);
		this.defaultsDiscord();
		this.updateOptions();
		this.applyChanges(true);
		this.btnApply.Enabled = false;
	}

	// Token: 0x060069CE RID: 27086 RVA: 0x002A30F5 File Offset: 0x002A12F5
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnBack_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		base.xui.playerUI.windowManager.Open(XUiC_OptionsMenu.ID, true, false, true);
	}

	// Token: 0x060069CF RID: 27087 RVA: 0x002AFFB0 File Offset: 0x002AE1B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateVoiceDevices()
	{
		if (!XUiC_OptionsAudio.VoiceAvailable)
		{
			return;
		}
		if (!XUiC_OptionsAudio.IsCommunicationAllowed)
		{
			return;
		}
		ValueTuple<IList<IPartyVoice.VoiceAudioDevice>, IList<IPartyVoice.VoiceAudioDevice>> devicesList = PlatformManager.MultiPlatform.PartyVoice.GetDevicesList();
		IList<IPartyVoice.VoiceAudioDevice> item = devicesList.Item1;
		IList<IPartyVoice.VoiceAudioDevice> item2 = devicesList.Item2;
		XUiC_OptionsAudio.<updateVoiceDevices>g__SelectActiveDevice|53_0(item, this.comboVoiceInputDevice, EnumGamePrefs.OptionsVoiceInputDevice);
		XUiC_OptionsAudio.<updateVoiceDevices>g__SelectActiveDevice|53_0(item2, this.comboVoiceOutputDevice, EnumGamePrefs.OptionsVoiceOutputDevice);
	}

	// Token: 0x060069D0 RID: 27088 RVA: 0x002B000C File Offset: 0x002AE20C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateOptions()
	{
		this.comboOverallAudioVolumeLevel.Value = (double)(this.origOverallAudioVolumeLevel = GamePrefs.GetFloat(EnumGamePrefs.OptionsOverallAudioVolumeLevel));
		this.comboDynamicMusicEnabled.Value = (this.origDynamicMusicEnabled = GamePrefs.GetBool(EnumGamePrefs.OptionsDynamicMusicEnabled));
		this.comboDynamicMusicDailyTimeAllotted.Value = (double)(this.origDynamicMusicDailyTimeAllotted = GamePrefs.GetFloat(EnumGamePrefs.OptionsDynamicMusicDailyTime));
		this.comboAmbientVolumeLevel.Value = (double)(this.origAmbientVolumeLevel = GamePrefs.GetFloat(EnumGamePrefs.OptionsAmbientVolumeLevel));
		this.comboInGameMusicVolumeLevel.Value = (double)(this.origInGameMusicVolumeLevel = GamePrefs.GetFloat(EnumGamePrefs.OptionsMusicVolumeLevel));
		this.comboMenuMusicVolumeLevel.Value = (double)(this.origMenuMusicVolumeLevel = GamePrefs.GetFloat(EnumGamePrefs.OptionsMenuMusicVolumeLevel));
		this.comboSubtitlesEnabled.Value = (this.origSubtitlesEnabled = GamePrefs.GetBool(EnumGamePrefs.OptionsSubtitlesEnabled));
		this.comboProfanityFilter.Value = (this.origProfanityFilter = GamePrefs.GetBool(EnumGamePrefs.OptionsFilterProfanity));
		if (!(DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
		{
			this.comboMumblePositionalAudioSupportEnabled.Value = (this.origMumblePositionalAudioSupportEnabled = GamePrefs.GetBool(EnumGamePrefs.OptionsMumblePositionalAudioSupport));
		}
		if (XUiC_OptionsAudio.VoiceAvailable)
		{
			this.comboVoiceChatEnabled.Value = (this.origVoiceChatEnabled = GamePrefs.GetBool(EnumGamePrefs.OptionsVoiceChatEnabled));
			this.comboVoiceVolumeLevel.Value = (double)(this.origVoiceVolumeLevel = GamePrefs.GetFloat(EnumGamePrefs.OptionsVoiceVolumeLevel));
			this.updateVoiceDevices();
		}
		this.updateOptionsDiscord();
	}

	// Token: 0x060069D1 RID: 27089 RVA: 0x002B0170 File Offset: 0x002AE370
	[PublicizedFrom(EAccessModifier.Private)]
	public void applyChanges(bool _fromReset)
	{
		AudioListener.volume = GamePrefs.GetFloat(EnumGamePrefs.OptionsOverallAudioVolumeLevel);
		this.origOverallAudioVolumeLevel = (float)this.comboOverallAudioVolumeLevel.Value;
		this.origDynamicMusicEnabled = this.comboDynamicMusicEnabled.Value;
		this.origDynamicMusicDailyTimeAllotted = (float)this.comboDynamicMusicDailyTimeAllotted.Value;
		this.origAmbientVolumeLevel = (float)this.comboAmbientVolumeLevel.Value;
		this.origInGameMusicVolumeLevel = (float)this.comboInGameMusicVolumeLevel.Value;
		this.origMenuMusicVolumeLevel = (float)this.comboMenuMusicVolumeLevel.Value;
		this.origSubtitlesEnabled = this.comboSubtitlesEnabled.Value;
		this.origProfanityFilter = this.comboProfanityFilter.Value;
		if (!(DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
		{
			this.origMumblePositionalAudioSupportEnabled = this.comboMumblePositionalAudioSupportEnabled.Value;
			GamePrefs.Set(EnumGamePrefs.OptionsMumblePositionalAudioSupport, this.origMumblePositionalAudioSupportEnabled);
		}
		if (XUiC_OptionsAudio.VoiceAvailable)
		{
			this.origVoiceChatEnabled = this.comboVoiceChatEnabled.Value;
			this.origVoiceVolumeLevel = (float)this.comboVoiceVolumeLevel.Value;
			XUiC_ComboBoxList<IPartyVoice.VoiceAudioDevice> xuiC_ComboBoxList = this.comboVoiceInputDevice;
			if (((xuiC_ComboBoxList != null) ? xuiC_ComboBoxList.Value : null) != null)
			{
				GamePrefs.Set(EnumGamePrefs.OptionsVoiceInputDevice, this.comboVoiceInputDevice.Value.Identifier);
			}
			XUiC_ComboBoxList<IPartyVoice.VoiceAudioDevice> xuiC_ComboBoxList2 = this.comboVoiceOutputDevice;
			if (((xuiC_ComboBoxList2 != null) ? xuiC_ComboBoxList2.Value : null) != null)
			{
				GamePrefs.Set(EnumGamePrefs.OptionsVoiceOutputDevice, this.comboVoiceOutputDevice.Value.Identifier);
			}
			this.updateVoiceDevices();
		}
		this.applyChangesDiscord(_fromReset);
		GamePrefs.Instance.Save();
		if (XUiC_OptionsAudio.OnSettingsChanged != null)
		{
			XUiC_OptionsAudio.OnSettingsChanged();
		}
	}

	// Token: 0x060069D2 RID: 27090 RVA: 0x002B02F0 File Offset: 0x002AE4F0
	public override void OnOpen()
	{
		XUiC_MainMenuPlayerName.Close(base.xui);
		this.onOpenDiscord();
		base.WindowGroup.openWindowOnEsc = XUiC_OptionsMenu.ID;
		this.updateOptions();
		base.OnOpen();
		this.btnApply.Enabled = false;
		base.RefreshBindings(false);
		this.RefreshApplyLabel();
	}

	// Token: 0x060069D3 RID: 27091 RVA: 0x002B0344 File Offset: 0x002AE544
	public override void OnClose()
	{
		base.OnClose();
		GamePrefs.Set(EnumGamePrefs.OptionsOverallAudioVolumeLevel, this.origOverallAudioVolumeLevel);
		AudioListener.volume = this.origOverallAudioVolumeLevel;
		GamePrefs.Set(EnumGamePrefs.OptionsDynamicMusicEnabled, this.origDynamicMusicEnabled);
		GamePrefs.Set(EnumGamePrefs.OptionsDynamicMusicDailyTime, this.origDynamicMusicDailyTimeAllotted);
		GamePrefs.Set(EnumGamePrefs.OptionsAmbientVolumeLevel, this.origAmbientVolumeLevel);
		GamePrefs.Set(EnumGamePrefs.OptionsMusicVolumeLevel, this.origInGameMusicVolumeLevel);
		GamePrefs.Set(EnumGamePrefs.OptionsMenuMusicVolumeLevel, this.origMenuMusicVolumeLevel);
		GamePrefs.Set(EnumGamePrefs.OptionsSubtitlesEnabled, this.origSubtitlesEnabled);
		GamePrefs.Set(EnumGamePrefs.OptionsFilterProfanity, this.origProfanityFilter);
		if (!(DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
		{
			GamePrefs.Set(EnumGamePrefs.OptionsMumblePositionalAudioSupport, this.origMumblePositionalAudioSupportEnabled);
		}
		if (XUiC_OptionsAudio.VoiceAvailable)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsVoiceChatEnabled, this.origVoiceChatEnabled);
			GamePrefs.Set(EnumGamePrefs.OptionsVoiceVolumeLevel, this.origVoiceVolumeLevel);
		}
		this.onCloseDiscord();
	}

	// Token: 0x060069D4 RID: 27092 RVA: 0x002B0412 File Offset: 0x002AE612
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.btnApply.Enabled && base.xui.playerUI.playerInput.GUIActions.Apply.WasPressed)
		{
			this.BtnApply_OnPressed(null, 0);
		}
	}

	// Token: 0x060069D5 RID: 27093 RVA: 0x002B0454 File Offset: 0x002AE654
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
		if (num <= 1411810637U)
		{
			if (num <= 479821338U)
			{
				if (num <= 212961315U)
				{
					if (num != 24410162U)
					{
						if (num == 212961315U)
						{
							if (_bindingName == "discordaccountlinked")
							{
								DiscordManager.DiscordUser localUser = DiscordManager.Instance.LocalUser;
								_value = (localUser == null || !localUser.IsProvisionalAccount).ToString();
								return true;
							}
						}
					}
					else if (_bindingName == "commsallowed")
					{
						_value = XUiC_OptionsAudio.IsCommunicationAllowed.ToString();
						return true;
					}
				}
				else if (num != 277027448U)
				{
					if (num == 479821338U)
					{
						if (_bindingName == "discord_vad_threshold_manual")
						{
							_value = (!DiscordManager.Instance.Settings.VoiceVadModeAuto).ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "discord_ptt")
				{
					XUiC_ComboBoxBool xuiC_ComboBoxBool = this.comboDiscordVoiceButtonMode;
					_value = (xuiC_ComboBoxBool != null && xuiC_ComboBoxBool.Value).ToString();
					return true;
				}
			}
			else if (num <= 1087855316U)
			{
				if (num != 869682517U)
				{
					if (num == 1087855316U)
					{
						if (_bindingName == "discord_is_ready")
						{
							_value = DiscordManager.Instance.IsReady.ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "discord_supports_full_accounts")
				{
					_value = DiscordManager.SupportsFullAccounts.ToString();
					return true;
				}
			}
			else if (num != 1316783421U)
			{
				if (num == 1411810637U)
				{
					if (_bindingName == "notinlinux")
					{
						_value = "true";
						return true;
					}
				}
			}
			else if (_bindingName == "multiplayerallowed")
			{
				_value = PermissionsManager.IsMultiplayerAllowed().ToString();
				return true;
			}
		}
		else if (num <= 2288896823U)
		{
			if (num <= 1786356881U)
			{
				if (num != 1416094437U)
				{
					if (num == 1786356881U)
					{
						if (_bindingName == "discord_enabled")
						{
							_value = (!DiscordManager.Instance.Settings.DiscordDisabled).ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "is_online")
				{
					_value = (PlatformManager.MultiPlatform.User.UserStatus == EUserStatus.LoggedIn).ToString();
					return true;
				}
			}
			else if (num != 1964364183U)
			{
				if (num == 2288896823U)
				{
					if (_bindingName == "discord_is_speaking")
					{
						DiscordManager.LobbyInfo activeVoiceLobby = DiscordManager.Instance.ActiveVoiceLobby;
						_value = (activeVoiceLobby != null && activeVoiceLobby.VoiceCall.IsSpeaking).ToString();
						return true;
					}
				}
			}
			else if (_bindingName == "notingame")
			{
				_value = (GameStats.GetInt(EnumGameStats.GameState) == 0).ToString();
				return true;
			}
		}
		else if (num <= 3043547827U)
		{
			if (num != 2903373570U)
			{
				if (num == 3043547827U)
				{
					if (_bindingName == "discordinitialized")
					{
						_value = DiscordManager.Instance.IsInitialized.ToString();
						return true;
					}
				}
			}
			else if (_bindingName == "discord_supports_provisional_accounts")
			{
				_value = DiscordManager.SupportsProvisionalAccounts.ToString();
				return true;
			}
		}
		else if (num != 3827972912U)
		{
			if (num == 4030564964U)
			{
				if (_bindingName == "voiceavailable")
				{
					_value = XUiC_OptionsAudio.VoiceAvailable.ToString();
					return true;
				}
			}
		}
		else if (_bindingName == "discord_in_call")
		{
			_value = (DiscordManager.Instance.ActiveVoiceLobby != null).ToString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x060069D6 RID: 27094 RVA: 0x002B0849 File Offset: 0x002AEA49
	public void SetActiveTab(string _tabKey)
	{
		XUiC_TabSelector xuiC_TabSelector = this.tabSelector;
		if (xuiC_TabSelector == null)
		{
			return;
		}
		xuiC_TabSelector.SelectTabByName(_tabKey);
	}

	// Token: 0x060069D7 RID: 27095 RVA: 0x002B085C File Offset: 0x002AEA5C
	public void OpenAtTab(string _tabName)
	{
		this.SetActiveTab(_tabName);
		base.xui.playerUI.windowManager.Open(XUiC_OptionsAudio.ID, true, false, true);
	}

	// Token: 0x060069D8 RID: 27096 RVA: 0x002B0884 File Offset: 0x002AEA84
	[PublicizedFrom(EAccessModifier.Private)]
	public void initDiscord()
	{
		this.comboDiscordEnabled = base.GetChildById("DiscordEnabled").GetChildByType<XUiC_ComboBoxBool>();
		this.comboDiscordDmPrivacyMode = base.GetChildById("DiscordDmPrivacyMode").GetChildByType<XUiC_ComboBoxBool>();
		this.comboDiscordAutoJoinVoice = base.GetChildById("DiscordAutoJoinVoice").GetChildByType<XUiC_ComboBoxEnum<DiscordManager.EAutoJoinVoiceMode>>();
		this.comboDiscordVoiceButtonMode = base.GetChildById("DiscordVoiceButtonMode").GetChildByType<XUiC_ComboBoxBool>();
		this.comboDiscordOutputDevice = base.GetChildById("DiscordOutputDevice").GetChildByType<XUiC_ComboBoxList<IPartyVoice.VoiceAudioDevice>>();
		this.comboDiscordInputDevice = base.GetChildById("DiscordInputDevice").GetChildByType<XUiC_ComboBoxList<IPartyVoice.VoiceAudioDevice>>();
		this.comboDiscordOutputVolume = base.GetChildById("DiscordOutputVolume").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboDiscordInputVolume = base.GetChildById("DiscordInputVolume").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboDiscordVoiceVadModeAuto = base.GetChildById("DiscordVADThresholdAuto").GetChildByType<XUiC_ComboBoxBool>();
		this.comboDiscordVoiceVadThreshold = base.GetChildById("DiscordVADThreshold").GetChildByType<XUiC_ComboBoxInt>();
		this.comboDiscordEnabled.OnValueChanged += this.ComboDiscordEnabledOnValueChanged;
		this.comboDiscordDmPrivacyMode.OnValueChanged += this.ComboDiscordDmPrivacyModeOnValueChanged;
		this.comboDiscordAutoJoinVoice.OnValueChanged += this.comboDiscordAutoJoinVoiceOnValueChanged;
		this.comboDiscordVoiceButtonMode.OnValueChanged += this.ComboDiscordVoiceButtonModeOnValueChanged;
		this.comboDiscordOutputDevice.OnValueChanged += this.ComboDiscordOutputDeviceOnValueChanged;
		this.comboDiscordInputDevice.OnValueChanged += this.ComboDiscordInputDeviceOnValueChanged;
		this.comboDiscordOutputVolume.OnValueChanged += this.ComboDiscordOutputVolumeOnValueChanged;
		this.comboDiscordInputVolume.OnValueChanged += this.ComboDiscordInputVolumeOnValueChanged;
		this.comboDiscordVoiceVadModeAuto.OnValueChanged += this.ComboDiscordVoiceVadModeAutoOnValueChanged;
		this.comboDiscordVoiceVadThreshold.OnValueChanged += this.ComboDiscordVoiceVadThresholdOnValueChanged;
		XUiC_SimpleButton xuiC_SimpleButton = base.GetChildById("btnDiscordInitialize") as XUiC_SimpleButton;
		if (xuiC_SimpleButton != null)
		{
			xuiC_SimpleButton.OnPressed += delegate(XUiController _, int _)
			{
				XUiC_DiscordLogin.Open(delegate
				{
					this.OpenAtTab("xuiOptionsAudioDiscord");
				}, false, false, false, true, false);
				DiscordManager.Instance.AuthManager.AutoLogin();
			};
		}
		XUiC_SimpleButton xuiC_SimpleButton2 = base.GetChildById("btnDiscordLinkAccount") as XUiC_SimpleButton;
		if (xuiC_SimpleButton2 != null)
		{
			xuiC_SimpleButton2.OnPressed += delegate(XUiController _, int _)
			{
				XUiC_DiscordLogin.Open(delegate
				{
					this.OpenAtTab("xuiOptionsAudioDiscord");
				}, false, false, false, true, (DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent());
				DiscordManager.Instance.AuthManager.LoginDiscordUser();
			};
		}
		XUiC_SimpleButton xuiC_SimpleButton3 = base.GetChildById("btnDiscordUnlinkAccount") as XUiC_SimpleButton;
		if (xuiC_SimpleButton3 != null)
		{
			xuiC_SimpleButton3.OnPressed += delegate(XUiController _, int _)
			{
				XUiC_DiscordLogin.Open(delegate
				{
					this.OpenAtTab("xuiOptionsAudioDiscord");
				}, false, false, false, true, (DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent());
				DiscordManager.Instance.AuthManager.UnmergeAccount();
			};
		}
		XUiC_SimpleButton xuiC_SimpleButton4 = base.GetChildById("btnDiscordManageSocialSettings") as XUiC_SimpleButton;
		if (xuiC_SimpleButton4 != null)
		{
			xuiC_SimpleButton4.OnPressed += delegate(XUiController _, int _)
			{
				DiscordManager.Instance.OpenDiscordSocialSettings();
			};
		}
		XUiC_SimpleButton xuiC_SimpleButton5 = base.GetChildById("btnDiscordManageBlockedUsers") as XUiC_SimpleButton;
		if (xuiC_SimpleButton5 != null)
		{
			xuiC_SimpleButton5.OnPressed += delegate(XUiController _, int _)
			{
				GUIWindow window = base.xui.playerUI.windowManager.GetWindow(XUiC_DiscordBlockedUsers.ID);
				window.openWindowOnEsc = this.windowGroup.ID;
				base.xui.playerUI.windowManager.Open(window, true, false, true);
			};
		}
	}

	// Token: 0x060069D9 RID: 27097 RVA: 0x00080679 File Offset: 0x0007E879
	[PublicizedFrom(EAccessModifier.Private)]
	public void discordStatusChanged(DiscordManager.EDiscordStatus _status)
	{
		base.RefreshBindings(false);
	}

	// Token: 0x060069DA RID: 27098 RVA: 0x00080679 File Offset: 0x0007E879
	[PublicizedFrom(EAccessModifier.Private)]
	public void discordLocalUserChanged(bool _loggedIn)
	{
		base.RefreshBindings(false);
	}

	// Token: 0x060069DB RID: 27099 RVA: 0x002B0B27 File Offset: 0x002AED27
	[PublicizedFrom(EAccessModifier.Private)]
	public void discordVoiceStateChanged(bool _self, ulong _userId)
	{
		if (!_self)
		{
			return;
		}
		base.RefreshBindings(false);
	}

	// Token: 0x060069DC RID: 27100 RVA: 0x002AFF22 File Offset: 0x002AE122
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboDiscordEnabledOnValueChanged(XUiController _sender, bool _oldValue, bool _newValue)
	{
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069DD RID: 27101 RVA: 0x002AFF22 File Offset: 0x002AE122
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboDiscordDmPrivacyModeOnValueChanged(XUiController _sender, bool _oldValue, bool _newValue)
	{
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069DE RID: 27102 RVA: 0x002AFF22 File Offset: 0x002AE122
	[PublicizedFrom(EAccessModifier.Private)]
	public void comboDiscordAutoJoinVoiceOnValueChanged(XUiController _sender, DiscordManager.EAutoJoinVoiceMode _oldValue, DiscordManager.EAutoJoinVoiceMode _newValue)
	{
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069DF RID: 27103 RVA: 0x002B0B34 File Offset: 0x002AED34
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboDiscordVoiceButtonModeOnValueChanged(XUiController _sender, bool _oldValue, bool _newValue)
	{
		this.btnApply.Enabled = true;
		base.RefreshBindings(false);
	}

	// Token: 0x060069E0 RID: 27104 RVA: 0x002AFF22 File Offset: 0x002AE122
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboDiscordOutputDeviceOnValueChanged(XUiController _sender, IPartyVoice.VoiceAudioDevice _oldValue, IPartyVoice.VoiceAudioDevice _newValue)
	{
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069E1 RID: 27105 RVA: 0x002AFF22 File Offset: 0x002AE122
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboDiscordInputDeviceOnValueChanged(XUiController _sender, IPartyVoice.VoiceAudioDevice _oldValue, IPartyVoice.VoiceAudioDevice _newValue)
	{
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069E2 RID: 27106 RVA: 0x002B0B49 File Offset: 0x002AED49
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboDiscordOutputVolumeOnValueChanged(XUiController _sender, double _oldValue, double _newValue)
	{
		DiscordManager.Instance.Settings.OutputVolume = Mathf.RoundToInt((float)(_newValue * 100.0));
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069E3 RID: 27107 RVA: 0x002B0B77 File Offset: 0x002AED77
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboDiscordInputVolumeOnValueChanged(XUiController _sender, double _oldValue, double _newValue)
	{
		DiscordManager.Instance.Settings.InputVolume = Mathf.RoundToInt((float)(_newValue * 100.0));
		this.btnApply.Enabled = true;
	}

	// Token: 0x060069E4 RID: 27108 RVA: 0x002B0BA5 File Offset: 0x002AEDA5
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboDiscordVoiceVadModeAutoOnValueChanged(XUiController _sender, bool _oldValue, bool _newValue)
	{
		DiscordManager.Instance.Settings.VoiceVadModeAuto = _newValue;
		this.btnApply.Enabled = true;
		base.RefreshBindings(false);
	}

	// Token: 0x060069E5 RID: 27109 RVA: 0x002B0BCA File Offset: 0x002AEDCA
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboDiscordVoiceVadThresholdOnValueChanged(XUiController _sender, long _oldValue, long _newValue)
	{
		DiscordManager.Instance.Settings.VoiceVadThreshold = (int)_newValue;
		this.btnApply.Enabled = true;
		base.RefreshBindings(false);
	}

	// Token: 0x060069E6 RID: 27110 RVA: 0x002B0BF0 File Offset: 0x002AEDF0
	[PublicizedFrom(EAccessModifier.Private)]
	public void applyChangesDiscord(bool _fromReset)
	{
		DiscordManager.DiscordSettings settings = DiscordManager.Instance.Settings;
		bool isInitialized = DiscordManager.Instance.IsInitialized;
		bool value = this.comboDiscordEnabled.Value;
		settings.DiscordDisabled = !value;
		if (isInitialized)
		{
			this.origDiscordOutputVolume = (float)this.comboDiscordOutputVolume.Value;
			this.origDiscordInputVolume = (float)this.comboDiscordInputVolume.Value;
			settings.VoiceModePtt = this.comboDiscordVoiceButtonMode.Value;
			settings.DmPrivacyMode = this.comboDiscordDmPrivacyMode.Value;
			settings.AutoJoinVoiceMode = this.comboDiscordAutoJoinVoice.Value;
			this.origDiscordVoiceVadModeAuto = this.comboDiscordVoiceVadModeAuto.Value;
			this.origDiscordVoiceVadThreshold = (int)this.comboDiscordVoiceVadThreshold.Value;
			if (!_fromReset)
			{
				settings.SelectedOutputDevice = this.comboDiscordOutputDevice.Value.Identifier;
				settings.SelectedInputDevice = this.comboDiscordInputDevice.Value.Identifier;
			}
		}
		if (isInitialized != value)
		{
			if (!value)
			{
				this.windowGroup.isEscClosable = false;
				XUiC_MessageBoxWindowGroup.ShowMessageBox(base.xui, Localization.Get("xuiOptionsAudioDiscordDisableRestartTitle", false), Localization.Get("xuiOptionsAudioDiscordDisableRestartText", false), delegate()
				{
					this.windowGroup.isEscClosable = true;
				}, false, false);
			}
			this.updateOptionsDiscord();
			base.RefreshBindings(false);
		}
		settings.Save();
	}

	// Token: 0x060069E7 RID: 27111 RVA: 0x002B0D2D File Offset: 0x002AEF2D
	[PublicizedFrom(EAccessModifier.Private)]
	public void defaultsDiscord()
	{
		DiscordManager.Instance.Settings.ResetToDefaults();
		base.RefreshBindings(false);
	}

	// Token: 0x060069E8 RID: 27112 RVA: 0x002B0D48 File Offset: 0x002AEF48
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateOptionsDiscord()
	{
		if (!XUiC_OptionsAudio.IsCommunicationAllowed)
		{
			return;
		}
		DiscordManager.DiscordSettings settings = DiscordManager.Instance.Settings;
		this.comboDiscordEnabled.Value = !settings.DiscordDisabled;
		this.comboDiscordVoiceButtonMode.Value = settings.VoiceModePtt;
		this.comboDiscordDmPrivacyMode.Value = settings.DmPrivacyMode;
		this.comboDiscordAutoJoinVoice.Value = settings.AutoJoinVoiceMode;
		this.comboDiscordOutputVolume.Value = (double)(this.origDiscordOutputVolume = (float)settings.OutputVolume / 100f);
		this.comboDiscordInputVolume.Value = (double)(this.origDiscordInputVolume = (float)settings.InputVolume / 100f);
		this.comboDiscordVoiceVadModeAuto.Value = (this.origDiscordVoiceVadModeAuto = settings.VoiceVadModeAuto);
		this.comboDiscordVoiceVadThreshold.Value = (long)(this.origDiscordVoiceVadThreshold = settings.VoiceVadThreshold);
		this.DiscordOnAudioDevicesChanged(DiscordManager.Instance.AudioOutput);
		this.DiscordOnAudioDevicesChanged(DiscordManager.Instance.AudioInput);
	}

	// Token: 0x060069E9 RID: 27113 RVA: 0x002B0E48 File Offset: 0x002AF048
	[PublicizedFrom(EAccessModifier.Private)]
	public void onOpenDiscord()
	{
		DiscordManager instance = DiscordManager.Instance;
		instance.AudioDevicesChanged += this.DiscordOnAudioDevicesChanged;
		instance.LocalUserChanged += this.discordLocalUserChanged;
		instance.StatusChanged += this.discordStatusChanged;
		instance.VoiceStateChanged += this.discordVoiceStateChanged;
	}

	// Token: 0x060069EA RID: 27114 RVA: 0x002B0EA4 File Offset: 0x002AF0A4
	[PublicizedFrom(EAccessModifier.Private)]
	public void onCloseDiscord()
	{
		DiscordManager instance = DiscordManager.Instance;
		DiscordManager.DiscordSettings settings = instance.Settings;
		settings.OutputVolume = Mathf.RoundToInt(this.origDiscordOutputVolume * 100f);
		settings.InputVolume = Mathf.RoundToInt(this.origDiscordInputVolume * 100f);
		settings.VoiceVadModeAuto = this.origDiscordVoiceVadModeAuto;
		settings.VoiceVadThreshold = this.origDiscordVoiceVadThreshold;
		instance.AudioDevicesChanged -= this.DiscordOnAudioDevicesChanged;
		instance.LocalUserChanged -= this.discordLocalUserChanged;
		instance.StatusChanged -= this.discordStatusChanged;
		instance.VoiceStateChanged -= this.discordVoiceStateChanged;
	}

	// Token: 0x060069EB RID: 27115 RVA: 0x002B0F48 File Offset: 0x002AF148
	[PublicizedFrom(EAccessModifier.Private)]
	public void DiscordOnAudioDevicesChanged(DiscordManager.AudioDeviceConfig _inOutConfig)
	{
		XUiC_ComboBoxList<IPartyVoice.VoiceAudioDevice> xuiC_ComboBoxList = _inOutConfig.IsOutput ? this.comboDiscordOutputDevice : this.comboDiscordInputDevice;
		if (xuiC_ComboBoxList == null)
		{
			return;
		}
		xuiC_ComboBoxList.Elements.Clear();
		xuiC_ComboBoxList.Elements.AddRange(_inOutConfig.CurrentAudioDevices.Values);
		if (xuiC_ComboBoxList.Elements.Count == 0)
		{
			xuiC_ComboBoxList.Elements.Add(XUiC_OptionsAudio.noDeviceEntry);
			xuiC_ComboBoxList.SelectedIndex = 0;
			return;
		}
		int selectedIndex;
		if (string.IsNullOrEmpty(_inOutConfig.ActiveAudioDevice) || (selectedIndex = xuiC_ComboBoxList.Elements.FindIndex((IPartyVoice.VoiceAudioDevice _device) => _device.Identifier == _inOutConfig.ActiveAudioDevice)) < 0)
		{
			int num = xuiC_ComboBoxList.Elements.FindIndex((IPartyVoice.VoiceAudioDevice _device) => _device.IsDefault);
			if (num < 0)
			{
				xuiC_ComboBoxList.Elements.Insert(0, XUiC_OptionsAudio.defaultDeviceEntry);
				num = 0;
			}
			xuiC_ComboBoxList.SelectedIndex = num;
			return;
		}
		xuiC_ComboBoxList.SelectedIndex = selectedIndex;
	}

	// Token: 0x060069EE RID: 27118 RVA: 0x002B106C File Offset: 0x002AF26C
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void <updateVoiceDevices>g__SelectActiveDevice|53_0(IList<IPartyVoice.VoiceAudioDevice> _devices, XUiC_ComboBoxList<IPartyVoice.VoiceAudioDevice> _combo, EnumGamePrefs _activeDevicePref)
	{
		if (_combo == null)
		{
			return;
		}
		string activeDevice = GamePrefs.GetString(_activeDevicePref);
		_combo.Elements.Clear();
		_combo.Elements.AddRange(_devices);
		if (_combo.Elements.Count == 0)
		{
			_combo.Elements.Add(XUiC_OptionsAudio.noDeviceEntry);
			_combo.SelectedIndex = 0;
			return;
		}
		int selectedIndex;
		if (string.IsNullOrEmpty(activeDevice) || (selectedIndex = _combo.Elements.FindIndex((IPartyVoice.VoiceAudioDevice _device) => _device.Identifier == activeDevice)) < 0)
		{
			int num = _combo.Elements.FindIndex((IPartyVoice.VoiceAudioDevice _device) => _device.IsDefault);
			if (num < 0)
			{
				_combo.Elements.Insert(0, XUiC_OptionsAudio.defaultDeviceEntry);
				num = 0;
			}
			_combo.SelectedIndex = num;
			return;
		}
		_combo.SelectedIndex = selectedIndex;
	}

	// Token: 0x04004FAB RID: 20395
	public static string ID = "";

	// Token: 0x04004FAD RID: 20397
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TabSelector tabSelector;

	// Token: 0x04004FAE RID: 20398
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboOverallAudioVolumeLevel;

	// Token: 0x04004FAF RID: 20399
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboDynamicMusicEnabled;

	// Token: 0x04004FB0 RID: 20400
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboDynamicMusicDailyTimeAllotted;

	// Token: 0x04004FB1 RID: 20401
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboAmbientVolumeLevel;

	// Token: 0x04004FB2 RID: 20402
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboInGameMusicVolumeLevel;

	// Token: 0x04004FB3 RID: 20403
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboMenuMusicVolumeLevel;

	// Token: 0x04004FB4 RID: 20404
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboSubtitlesEnabled;

	// Token: 0x04004FB5 RID: 20405
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboProfanityFilter;

	// Token: 0x04004FB6 RID: 20406
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboMumblePositionalAudioSupportEnabled;

	// Token: 0x04004FB7 RID: 20407
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboVoiceChatEnabled;

	// Token: 0x04004FB8 RID: 20408
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<IPartyVoice.VoiceAudioDevice> comboVoiceOutputDevice;

	// Token: 0x04004FB9 RID: 20409
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<IPartyVoice.VoiceAudioDevice> comboVoiceInputDevice;

	// Token: 0x04004FBA RID: 20410
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboVoiceVolumeLevel;

	// Token: 0x04004FBB RID: 20411
	[PublicizedFrom(EAccessModifier.Private)]
	public float origOverallAudioVolumeLevel;

	// Token: 0x04004FBC RID: 20412
	[PublicizedFrom(EAccessModifier.Private)]
	public bool origDynamicMusicEnabled;

	// Token: 0x04004FBD RID: 20413
	[PublicizedFrom(EAccessModifier.Private)]
	public float origDynamicMusicDailyTimeAllotted;

	// Token: 0x04004FBE RID: 20414
	[PublicizedFrom(EAccessModifier.Private)]
	public float origAmbientVolumeLevel;

	// Token: 0x04004FBF RID: 20415
	[PublicizedFrom(EAccessModifier.Private)]
	public float origInGameMusicVolumeLevel;

	// Token: 0x04004FC0 RID: 20416
	[PublicizedFrom(EAccessModifier.Private)]
	public float origMenuMusicVolumeLevel;

	// Token: 0x04004FC1 RID: 20417
	[PublicizedFrom(EAccessModifier.Private)]
	public bool origSubtitlesEnabled;

	// Token: 0x04004FC2 RID: 20418
	[PublicizedFrom(EAccessModifier.Private)]
	public bool origProfanityFilter;

	// Token: 0x04004FC3 RID: 20419
	[PublicizedFrom(EAccessModifier.Private)]
	public bool origMumblePositionalAudioSupportEnabled;

	// Token: 0x04004FC4 RID: 20420
	[PublicizedFrom(EAccessModifier.Private)]
	public bool origVoiceChatEnabled;

	// Token: 0x04004FC5 RID: 20421
	[PublicizedFrom(EAccessModifier.Private)]
	public float origVoiceVolumeLevel;

	// Token: 0x04004FC6 RID: 20422
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnApply;

	// Token: 0x04004FC7 RID: 20423
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly IPartyVoice.VoiceAudioDevice noDeviceEntry = new IPartyVoice.VoiceAudioDeviceNotFound();

	// Token: 0x04004FC8 RID: 20424
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly IPartyVoice.VoiceAudioDevice defaultDeviceEntry = new IPartyVoice.VoiceAudioDeviceDefault();

	// Token: 0x04004FC9 RID: 20425
	public const string DiscordTabName = "xuiOptionsAudioDiscord";

	// Token: 0x04004FCA RID: 20426
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboDiscordEnabled;

	// Token: 0x04004FCB RID: 20427
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboDiscordDmPrivacyMode;

	// Token: 0x04004FCC RID: 20428
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxEnum<DiscordManager.EAutoJoinVoiceMode> comboDiscordAutoJoinVoice;

	// Token: 0x04004FCD RID: 20429
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboDiscordVoiceButtonMode;

	// Token: 0x04004FCE RID: 20430
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<IPartyVoice.VoiceAudioDevice> comboDiscordOutputDevice;

	// Token: 0x04004FCF RID: 20431
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<IPartyVoice.VoiceAudioDevice> comboDiscordInputDevice;

	// Token: 0x04004FD0 RID: 20432
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboDiscordOutputVolume;

	// Token: 0x04004FD1 RID: 20433
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboDiscordInputVolume;

	// Token: 0x04004FD2 RID: 20434
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboDiscordVoiceVadModeAuto;

	// Token: 0x04004FD3 RID: 20435
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxInt comboDiscordVoiceVadThreshold;

	// Token: 0x04004FD4 RID: 20436
	[PublicizedFrom(EAccessModifier.Private)]
	public float origDiscordOutputVolume;

	// Token: 0x04004FD5 RID: 20437
	[PublicizedFrom(EAccessModifier.Private)]
	public float origDiscordInputVolume;

	// Token: 0x04004FD6 RID: 20438
	[PublicizedFrom(EAccessModifier.Private)]
	public bool origDiscordVoiceVadModeAuto;

	// Token: 0x04004FD7 RID: 20439
	[PublicizedFrom(EAccessModifier.Private)]
	public int origDiscordVoiceVadThreshold;
}
