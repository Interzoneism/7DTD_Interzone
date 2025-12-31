using System;
using System.Collections.Generic;
using GUI_2;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000D0D RID: 3341
[Preserve]
public class XUiC_MainMenu : XUiController
{
	// Token: 0x060067FC RID: 26620 RVA: 0x002A291D File Offset: 0x002A0B1D
	public override void Init()
	{
		base.Init();
		XUiC_MainMenu.ID = base.WindowGroup.ID;
	}

	// Token: 0x060067FD RID: 26621 RVA: 0x002A2935 File Offset: 0x002A0B35
	public override void Update(float _dt)
	{
		base.Update(_dt);
		this.windowGroup.isEscClosable = false;
		base.RefreshBindings(false);
		if (PlatformApplicationManager.CheckRestartCoroutineReady())
		{
			ThreadManager.StartCoroutine(PlatformApplicationManager.CheckRestartCoroutine(false));
		}
	}

	// Token: 0x060067FE RID: 26622 RVA: 0x002A2964 File Offset: 0x002A0B64
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "windows_to_open_globally")
		{
			string[] array = _value.Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (!string.IsNullOrEmpty(text))
				{
					char c = text[0];
					if (c != '+')
					{
						if (c != '-')
						{
							XUiC_MainMenu.windowsToOpenGloballyWithMainMenu.Add(text);
							XUiC_MainMenu.windowsToCloseGloballyWithMainMenu.Add(text);
						}
						else
						{
							List<string> list = XUiC_MainMenu.windowsToCloseGloballyWithMainMenu;
							string text2 = text;
							list.Add(text2.Substring(1, text2.Length - 1));
						}
					}
					else
					{
						List<string> list2 = XUiC_MainMenu.windowsToOpenGloballyWithMainMenu;
						string text2 = text;
						list2.Add(text2.Substring(1, text2.Length - 1));
					}
				}
			}
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x060067FF RID: 26623 RVA: 0x002A2A24 File Offset: 0x002A0C24
	public override void OnOpen()
	{
		base.OnOpen();
		PlatformManager.MultiPlatform.RichPresence.UpdateRichPresence(IRichPresence.PresenceStates.Menu);
		TitleStorageOverridesManager.Instance.FetchFromSource(null);
		ModEvents.SMainMenuOpenedData smainMenuOpenedData = new ModEvents.SMainMenuOpenedData(!XUiC_MainMenu.openedOnce);
		ModEvents.MainMenuOpened.Invoke(ref smainMenuOpenedData);
		XUiC_MainMenu.openedOnce = true;
		SaveDataUtils.SaveDataManager.CommitAsync();
		base.xui.playerUI.windowManager.Close("eacWarning");
		base.xui.playerUI.windowManager.Close("crossplayWarning");
		XUiC_MainMenuPlayerName.OpenIfNotOpen(base.xui);
		base.xui.playerUI.windowManager.ResetActionSets();
		XUiC_MainMenu.OpenGlobalMenuWindows(base.xui);
		GameManager.Instance.SetCursorEnabledOverride(false, false);
		base.xui.playerUI.CursorController.SetCursorHidden(false);
		base.xui.playerUI.windowManager.OpenIfNotOpen("CalloutGroup", false, false, true);
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonSouth, "igcoSelect", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.RemoveCallout(UIUtils.ButtonIcon.FaceButtonEast, "igcoBack", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.SetCalloutsEnabled(XUiC_GamepadCalloutWindow.CalloutType.Menu, true);
		TriggerEffectManager.SetMainMenuLightbarColor();
		if (this.snapCheck())
		{
			return;
		}
		this.clockSyncCheck();
	}

	// Token: 0x06006800 RID: 26624 RVA: 0x002A2B80 File Offset: 0x002A0D80
	[PublicizedFrom(EAccessModifier.Private)]
	public bool snapCheck()
	{
		if (XUiC_MainMenu.snapMicPermissionChecked)
		{
			return false;
		}
		XUiC_MainMenu.snapMicPermissionChecked = true;
		if (VoiceHelpers.IsSnapWithoutMicPermission() && (!DiscordManager.Instance.Settings.DiscordDisabled || GamePrefs.GetBool(EnumGamePrefs.OptionsVoiceChatEnabled)))
		{
			XUiC_MessageBoxWindowGroup.ShowMessageBox(base.xui, Localization.Get("xuiLinuxSnapNoMicPermissionTitle", false), Localization.Get("xuiLinuxSnapNoMicPermissionText", false), null, true, true);
			return true;
		}
		return false;
	}

	// Token: 0x06006801 RID: 26625 RVA: 0x002A2BE4 File Offset: 0x002A0DE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void clockSyncCheck()
	{
		if (XUiC_MainMenu.clockSyncChecked)
		{
			return;
		}
		if (!GameManager.ServerClockSync.RequestComplete)
		{
			return;
		}
		XUiC_MainMenu.clockSyncChecked = true;
		if (!GameManager.ServerClockSync.HasError && Math.Abs(GameManager.ServerClockSync.SecondsOffset) >= 120)
		{
			XUiC_MessageBoxWindowGroup.ShowMessageBox(base.xui, Localization.Get("xuiSystemTimeSyncHeader", false), Localization.Get("xuiSystemTimeSyncWarning", false), null, true, true);
		}
	}

	// Token: 0x06006802 RID: 26626 RVA: 0x002A2C4F File Offset: 0x002A0E4F
	public override void OnClose()
	{
		base.OnClose();
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonEast, "igcoBack", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		XUiC_MainMenuPlayerName.Close(base.xui);
	}

	// Token: 0x06006803 RID: 26627 RVA: 0x002A2C7C File Offset: 0x002A0E7C
	public static void Open(XUi _xuiInstance)
	{
		XUiC_MainMenu.OpenGlobalMenuWindows(_xuiInstance);
		if (LaunchPrefs.SkipNewsScreen.Value || PlatformApplicationManager.GetLoadSaveGameState() != EPlatformLoadSaveGameState.Done)
		{
			XUiC_MainMenu.shownNewsScreenOnce = true;
		}
		if (InviteManager.Instance.HasPendingInvite())
		{
			XUiC_MainMenu.shownNewsScreenOnce = true;
		}
		if (!XUiC_MainMenu.shownNewsScreenOnce)
		{
			XUiC_NewsScreen.Open(_xuiInstance);
			XUiC_MainMenu.shownNewsScreenOnce = true;
			return;
		}
		ModEvents.SMainMenuOpeningData smainMenuOpeningData = new ModEvents.SMainMenuOpeningData(XUiC_MainMenu.openedOnce);
		if (ModEvents.MainMenuOpening.Invoke(ref smainMenuOpeningData).Item1 == ModEvents.EModEventResult.StopHandlersAndVanilla)
		{
			return;
		}
		_xuiInstance.playerUI.windowManager.Open(XUiC_MainMenu.ID, true, false, true);
	}

	// Token: 0x06006804 RID: 26628 RVA: 0x002A2D08 File Offset: 0x002A0F08
	public static void OpenGlobalMenuWindows(XUi _xui)
	{
		LocalPlayerUI playerUI = _xui.playerUI;
		GUIWindowManager windowManager = playerUI.windowManager;
		playerUI.nguiWindowManager.Show(EnumNGUIWindow.MainMenuBackground, false);
		foreach (string windowName in XUiC_MainMenu.windowsToOpenGloballyWithMainMenu)
		{
			windowManager.OpenIfNotOpen(windowName, false, false, true);
		}
	}

	// Token: 0x06006805 RID: 26629 RVA: 0x002A2D78 File Offset: 0x002A0F78
	public static void CloseGlobalMenuWindows(XUi _xui)
	{
		XUiC_MainMenuPlayerName.Close(_xui);
		LocalPlayerUI playerUI = _xui.playerUI;
		GUIWindowManager windowManager = playerUI.windowManager;
		playerUI.nguiWindowManager.Show(EnumNGUIWindow.MainMenuBackground, false);
		foreach (string windowName in XUiC_MainMenu.windowsToCloseGloballyWithMainMenu)
		{
			windowManager.Close(windowName);
		}
	}

	// Token: 0x04004E6C RID: 20076
	public static string ID = "";

	// Token: 0x04004E6D RID: 20077
	public static bool openedOnce;

	// Token: 0x04004E6E RID: 20078
	public static bool shownNewsScreenOnce;

	// Token: 0x04004E6F RID: 20079
	public static bool clockSyncChecked;

	// Token: 0x04004E70 RID: 20080
	public static bool snapMicPermissionChecked;

	// Token: 0x04004E71 RID: 20081
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<string> windowsToOpenGloballyWithMainMenu = new List<string>();

	// Token: 0x04004E72 RID: 20082
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<string> windowsToCloseGloballyWithMainMenu = new List<string>();
}
