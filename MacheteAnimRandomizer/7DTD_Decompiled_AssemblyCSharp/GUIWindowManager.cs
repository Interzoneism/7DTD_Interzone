using System;
using System.Collections.Generic;
using InControl;
using Platform;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000FC2 RID: 4034
public class GUIWindowManager : MonoBehaviour
{
	// Token: 0x06008087 RID: 32903 RVA: 0x00343345 File Offset: 0x00341545
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		this.nguiWindowManager = base.GetComponent<NGUIWindowManager>();
		this.playerUI = base.GetComponent<LocalPlayerUI>();
		GameOptionsManager.ResolutionChanged += this.OnResolutionChanged;
	}

	// Token: 0x06008088 RID: 32904 RVA: 0x00343370 File Offset: 0x00341570
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		GameOptionsManager.ResolutionChanged -= this.OnResolutionChanged;
	}

	// Token: 0x06008089 RID: 32905 RVA: 0x00343383 File Offset: 0x00341583
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnResolutionChanged(int _width, int _height)
	{
		this.RecenterAllWindows(_width, _height);
	}

	// Token: 0x0600808A RID: 32906 RVA: 0x00343390 File Offset: 0x00341590
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnGUI()
	{
		for (int i = 0; i < this.windowsToOpen.Count; i++)
		{
			GUIWindow guiwindow = this.windowsToOpen[i];
			guiwindow.isShowing = true;
			this.windows.Add(guiwindow);
			this.topmostWindow = guiwindow;
		}
		this.windowsToOpen.Clear();
		this.modalWindow = null;
		for (int j = 0; j < this.windows.Count; j++)
		{
			GUIWindow guiwindow2 = this.windows[j];
			if (guiwindow2.isModal)
			{
				this.modalWindow = guiwindow2;
				break;
			}
		}
		if (this.modalWindow != null)
		{
			bool isDimBackground = this.modalWindow.isDimBackground;
		}
		this.cursorWindowOpen = false;
		for (int k = 0; k < this.windows.Count; k++)
		{
			GUIWindow guiwindow3 = this.windows[k];
			if (guiwindow3.isShowing)
			{
				this.cursorWindowOpen |= guiwindow3.alwaysUsesMouseCursor;
				GUI.matrix = guiwindow3.matrix;
				guiwindow3.OnGUI(this.topmostWindow == guiwindow3);
			}
		}
		GUI.enabled = true;
		List<GUIWindow> list = this.windowsToRemove;
		list.Clear();
		for (int l = 0; l < this.windows.Count; l++)
		{
			GUIWindow guiwindow4 = this.windows[l];
			if (!guiwindow4.isShowing)
			{
				list.Add(guiwindow4);
				this.topmostWindow = ((this.windows.Count > 0) ? this.windows[this.windows.Count - 1] : null);
			}
		}
		for (int m = 0; m < list.Count; m++)
		{
			GUIWindow item = list[m];
			this.windows.Remove(item);
		}
	}

	// Token: 0x0600808B RID: 32907 RVA: 0x0034354C File Offset: 0x0034174C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Update()
	{
		for (int i = 0; i < this.windows.Count; i++)
		{
			this.windows[i].Update();
		}
		if (this.lastActionClicked.Count != 0)
		{
			for (int j = 0; j < this.lastActionClicked.Count; j++)
			{
				NGuiAction nguiAction = this.lastActionClicked[j];
				PlayerAction hotkey = nguiAction.GetHotkey();
				if (hotkey != null && hotkey.WasReleased)
				{
					nguiAction.OnRelease();
					this.actionsToClear.Add(nguiAction);
				}
			}
			for (int k = 0; k < this.actionsToClear.Count; k++)
			{
				NGuiAction item = this.actionsToClear[k];
				this.lastActionClicked.Remove(item);
			}
			this.actionsToClear.Clear();
		}
		if (this.IsInputActive())
		{
			if (this.playerUI.playerInput != null && this.playerUI.playerInput.PermanentActions.Cancel.WasPressed && UIInput.selection != null)
			{
				UIInput.selection.RemoveFocus();
			}
			return;
		}
		if (!this.IsInputLocked)
		{
			List<NGuiAction> list = this.actionsForGlobalHotkeys;
			list.Clear();
			for (int l = 0; l < this.globalActions.Count; l++)
			{
				PlayerAction hotkey2 = this.globalActions[l].GetHotkey();
				if (hotkey2 != null && (((this.globalActions[l].KeyMode & NGuiAction.EnumKeyMode.FireOnRelease) == NGuiAction.EnumKeyMode.FireOnRelease && hotkey2.WasReleased) || ((this.globalActions[l].KeyMode & NGuiAction.EnumKeyMode.FireOnPress) == NGuiAction.EnumKeyMode.FireOnPress && hotkey2.WasPressed) || ((this.globalActions[l].KeyMode & NGuiAction.EnumKeyMode.FireOnRepeat) == NGuiAction.EnumKeyMode.FireOnRepeat && hotkey2.WasRepeated)))
				{
					list.Add(this.globalActions[l]);
				}
			}
			if (list.Count > 0)
			{
				for (int m = 0; m < list.Count; m++)
				{
					NGuiAction nguiAction2 = list[m];
					nguiAction2.OnClick();
					this.lastActionClicked.Add(nguiAction2);
				}
				list.Clear();
			}
			if (this.playerUI.playerInput != null && this.playerUI.playerInput.PermanentActions.Cancel.WasPressed && !this.IsWindowOpen("popupGroup") && this.modalWindow != null && this.modalWindow.isEscClosable)
			{
				this.CloseAllOpenWindows(null, true);
			}
		}
	}

	// Token: 0x0600808C RID: 32908 RVA: 0x003437C4 File Offset: 0x003419C4
	public bool IsInputActive()
	{
		return (UIInput.selection != null && UIInput.selection.gameObject.activeInHierarchy) || (this.topmostWindow != null && this.topmostWindow.isInputActive) || this.IsUGUIInputActive() || GameManager.Instance.m_GUIConsole.isShowing;
	}

	// Token: 0x0600808D RID: 32909 RVA: 0x00343820 File Offset: 0x00341A20
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsUGUIInputActive()
	{
		EventSystem current = EventSystem.current;
		if (current == null)
		{
			return false;
		}
		GameObject currentSelectedGameObject = current.currentSelectedGameObject;
		if (currentSelectedGameObject == null)
		{
			return false;
		}
		InputField inputField;
		if (currentSelectedGameObject.TryGetComponent<InputField>(out inputField))
		{
			return inputField.isFocused;
		}
		TMP_InputField tmp_InputField;
		return currentSelectedGameObject.TryGetComponent<TMP_InputField>(out tmp_InputField) && tmp_InputField.isFocused;
	}

	// Token: 0x0600808E RID: 32910 RVA: 0x00343873 File Offset: 0x00341A73
	public bool IsKeyShortcutsAllowed()
	{
		return PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard || (!this.IsInputLocked && !this.IsInputActive());
	}

	// Token: 0x0600808F RID: 32911 RVA: 0x0034389C File Offset: 0x00341A9C
	public void Add(string _windowName, GUIWindow _window)
	{
		this.nameToWindowMap.Add(_windowName, _window);
		_window.windowManager = this;
		if (this.nguiWindowManager == null)
		{
			this.nguiWindowManager = base.GetComponent<NGUIWindowManager>();
		}
		if (this.playerUI == null)
		{
			this.playerUI = base.GetComponent<LocalPlayerUI>();
		}
		_window.nguiWindowManager = this.nguiWindowManager;
		_window.playerUI = this.playerUI;
	}

	// Token: 0x06008090 RID: 32912 RVA: 0x0034390C File Offset: 0x00341B0C
	public void Remove(string _windowName)
	{
		GUIWindow guiwindow;
		if (!this.nameToWindowMap.TryGetValue(_windowName, out guiwindow))
		{
			Log.Warning("GUIWindowManager.Remove: Window \"{0}\" unknown!", new object[]
			{
				_windowName
			});
			return;
		}
		if (guiwindow.isShowing)
		{
			this.Close(guiwindow, false);
		}
		guiwindow.Cleanup();
		this.nameToWindowMap.Remove(_windowName);
	}

	// Token: 0x06008091 RID: 32913 RVA: 0x00343964 File Offset: 0x00341B64
	public GUIWindow GetWindow(string _windowName)
	{
		GUIWindow result;
		if (!this.nameToWindowMap.TryGetValue(_windowName, out result))
		{
			Log.Warning("GUIWindowManager.Remove: Window \"{0}\" unknown!", new object[]
			{
				_windowName
			});
			return null;
		}
		return result;
	}

	// Token: 0x06008092 RID: 32914 RVA: 0x00343998 File Offset: 0x00341B98
	public T GetWindow<T>(string _windowName) where T : GUIWindow
	{
		return (T)((object)this.nameToWindowMap[_windowName]);
	}

	// Token: 0x06008093 RID: 32915 RVA: 0x003439AB File Offset: 0x00341BAB
	public void SwitchVisible(string _windowName, bool _bIsNotEscClosable = false, bool _modal = true)
	{
		this.SwitchVisible(this.nameToWindowMap[_windowName], _bIsNotEscClosable, _modal);
	}

	// Token: 0x06008094 RID: 32916 RVA: 0x003439C1 File Offset: 0x00341BC1
	public void SwitchVisible(GUIWindow _guiWindow, bool _bIsNotEscClosable = false, bool _modal = true)
	{
		if ((!_modal || _guiWindow.isModal) && _guiWindow.isShowing)
		{
			this.Close(_guiWindow, false);
			return;
		}
		this.Open(_guiWindow, _modal, _bIsNotEscClosable, true);
	}

	// Token: 0x06008095 RID: 32917 RVA: 0x003439EC File Offset: 0x00341BEC
	public bool CloseAllOpenWindows(GUIWindow _exceptThis = null, bool _fromEsc = false)
	{
		bool result = false;
		for (int i = 0; i < this.windows.Count; i++)
		{
			GUIWindow guiwindow = this.windows[i];
			if (guiwindow.isModal && (_exceptThis == null || _exceptThis != guiwindow))
			{
				this.Close(guiwindow, _fromEsc);
				result = true;
			}
		}
		if (this.playerUI.CursorController != null)
		{
			if (this.playerUI.CursorController.navigationTarget != null)
			{
				this.playerUI.CursorController.navigationTarget.Controller.Hovered(false);
			}
			this.playerUI.CursorController.SetNavigationTarget(null);
			this.playerUI.CursorController.SetNavigationLockView(null, null);
		}
		return result;
	}

	// Token: 0x06008096 RID: 32918 RVA: 0x00343A9C File Offset: 0x00341C9C
	public bool CloseAllOpenWindows(string _windowName)
	{
		GUIWindow exceptThis = this.nameToWindowMap[_windowName];
		return this.CloseAllOpenWindows(exceptThis, false);
	}

	// Token: 0x06008097 RID: 32919 RVA: 0x00343AC0 File Offset: 0x00341CC0
	public void Open(string _windowName, int _x, int _y, bool _bModal, bool _bIsNotEscClosable = false)
	{
		GUIWindow guiwindow = this.nameToWindowMap[_windowName];
		guiwindow.windowRect = new Rect((float)_x, (float)_y, guiwindow.windowRect.width, guiwindow.windowRect.height);
		this.Open(guiwindow, _bModal, _bIsNotEscClosable, true);
	}

	// Token: 0x06008098 RID: 32920 RVA: 0x00343B11 File Offset: 0x00341D11
	public void OpenIfNotOpen(string _windowName, bool _bModal, bool _bIsNotEscClosable = false, bool _bCloseAllOpenWindows = true)
	{
		if (this.IsWindowOpen(_windowName))
		{
			return;
		}
		this.Open(_windowName, _bModal, _bIsNotEscClosable, _bCloseAllOpenWindows);
	}

	// Token: 0x06008099 RID: 32921 RVA: 0x00343B28 File Offset: 0x00341D28
	public void CloseIfOpen(string _windowName)
	{
		if (!this.IsWindowOpen(_windowName))
		{
			return;
		}
		this.Close(_windowName);
	}

	// Token: 0x0600809A RID: 32922 RVA: 0x00343B3C File Offset: 0x00341D3C
	public void Open(string _windowName, bool _bModal, bool _bIsNotEscClosable = false, bool _bCloseAllOpenWindows = true)
	{
		if (this.IsFullHUDDisabled())
		{
			return;
		}
		GUIWindow w;
		if (!this.nameToWindowMap.TryGetValue(_windowName, out w))
		{
			Log.Warning("GUIWindowManager.Open: Window \"{0}\" unknown!", new object[]
			{
				_windowName
			});
			Log.Out("Trace: " + StackTraceUtility.ExtractStackTrace());
			return;
		}
		QuestEventManager.Current.ChangedWindow(_windowName);
		this.Open(w, _bModal, _bIsNotEscClosable, _bCloseAllOpenWindows);
	}

	// Token: 0x0600809B RID: 32923 RVA: 0x00343BA4 File Offset: 0x00341DA4
	public void Open(GUIWindow _w, bool _bModal, bool _bIsNotEscClosable = false, bool _bCloseAllOpenWindows = true)
	{
		if (this.IsFullHUDDisabled())
		{
			return;
		}
		if (_bModal)
		{
			if (_bCloseAllOpenWindows)
			{
				this.CloseAllOpenWindows(null, false);
			}
			int i = 0;
			while (i < this.windowsToOpen.Count)
			{
				GUIWindow guiwindow = this.windowsToOpen[i];
				if (guiwindow.isModal && guiwindow != _w)
				{
					this.windowsToOpen.Remove(guiwindow);
					guiwindow.OnClose();
					if (guiwindow.HasActionSet())
					{
						this.DisableWindowActionSet(guiwindow);
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
		if (_w.isShowing)
		{
			_w.isModal = _bModal;
			return;
		}
		_w.windowManager = this;
		_w.isModal = _bModal;
		_w.isEscClosable = !_bIsNotEscClosable;
		bool flag = _w.isShowing || this.windowsToOpen.Contains(_w);
		if (!this.windows.Contains(_w))
		{
			this.windowsToOpen.Add(_w);
		}
		else
		{
			_w.isShowing = true;
		}
		if (!flag)
		{
			_w.OnOpen();
		}
		if (_w.HasActionSet() && !_w.bActionSetEnabled && (_w.isShowing || this.windowsToOpen.Contains(_w)))
		{
			this.EnableWindowActionSet(_w);
		}
	}

	// Token: 0x0600809C RID: 32924 RVA: 0x00343CB4 File Offset: 0x00341EB4
	public bool IsWindowOpen(string _wdwID)
	{
		GUIWindow guiwindow;
		return this.nameToWindowMap.TryGetValue(_wdwID, out guiwindow) && (guiwindow.isShowing || this.windowsToOpen.Contains(guiwindow));
	}

	// Token: 0x0600809D RID: 32925 RVA: 0x00343CE9 File Offset: 0x00341EE9
	public bool HasWindow(string _wdwID)
	{
		return this.nameToWindowMap.ContainsKey(_wdwID);
	}

	// Token: 0x0600809E RID: 32926 RVA: 0x00343CF7 File Offset: 0x00341EF7
	public bool IsModalWindowOpen()
	{
		return this.modalWindow != null;
	}

	// Token: 0x0600809F RID: 32927 RVA: 0x00343D02 File Offset: 0x00341F02
	public GUIWindow GetModalWindow()
	{
		return this.modalWindow;
	}

	// Token: 0x060080A0 RID: 32928 RVA: 0x00343D0A File Offset: 0x00341F0A
	public bool IsCursorWindowOpen()
	{
		return this.cursorWindowOpen;
	}

	// Token: 0x060080A1 RID: 32929 RVA: 0x00343D12 File Offset: 0x00341F12
	public void Close(string _windowName)
	{
		if (this.HasWindow(_windowName))
		{
			this.Close(this.nameToWindowMap[_windowName], false);
		}
	}

	// Token: 0x060080A2 RID: 32930 RVA: 0x00343D30 File Offset: 0x00341F30
	[PublicizedFrom(EAccessModifier.Private)]
	public void DisableWindowActionSet(GUIWindow _w)
	{
		if (_w.playerUI != null && _w.playerUI.ActionSetManager != null)
		{
			_w.playerUI.ActionSetManager.Pop(_w);
			_w.bActionSetEnabled = false;
		}
	}

	// Token: 0x060080A3 RID: 32931 RVA: 0x00343D65 File Offset: 0x00341F65
	[PublicizedFrom(EAccessModifier.Private)]
	public void EnableWindowActionSet(GUIWindow _w)
	{
		if (_w.playerUI != null && _w.playerUI.ActionSetManager != null)
		{
			_w.playerUI.ActionSetManager.Push(_w);
			_w.bActionSetEnabled = true;
		}
	}

	// Token: 0x060080A4 RID: 32932 RVA: 0x00343D9C File Offset: 0x00341F9C
	public void Close(GUIWindow _w, bool _fromEsc = false)
	{
		if (_w.isShowing)
		{
			_w.isShowing = false;
			_w.OnClose();
			if (_fromEsc && !string.IsNullOrEmpty(_w.openWindowOnEsc))
			{
				this.Open(_w.openWindowOnEsc, _w.isModal, false, true);
			}
		}
		else if (this.windowsToOpen.Contains(_w))
		{
			this.windowsToOpen.Remove(_w);
			_w.OnClose();
		}
		if (_w.bActionSetEnabled)
		{
			this.DisableWindowActionSet(_w);
		}
	}

	// Token: 0x060080A5 RID: 32933 RVA: 0x00343E14 File Offset: 0x00342014
	public void RecenterAllWindows(int _w, int _h)
	{
		foreach (KeyValuePair<string, GUIWindow> keyValuePair in this.nameToWindowMap)
		{
			string text;
			GUIWindow guiwindow;
			keyValuePair.Deconstruct(out text, out guiwindow);
			GUIWindow guiwindow2 = guiwindow;
			if (guiwindow2.bCenterWindow)
			{
				guiwindow2.SetPosition(((float)_w - guiwindow2.windowRect.width) / 2f, ((float)_h - guiwindow2.windowRect.height) / 2f);
			}
		}
	}

	// Token: 0x060080A6 RID: 32934 RVA: 0x00343EAC File Offset: 0x003420AC
	public void RemoveGlobalAction(NGuiAction _action)
	{
		this.globalActions.Remove(_action);
	}

	// Token: 0x060080A7 RID: 32935 RVA: 0x00343EBB File Offset: 0x003420BB
	public void AddGlobalAction(NGuiAction _action)
	{
		this.globalActions.Add(_action);
	}

	// Token: 0x060080A8 RID: 32936 RVA: 0x00343EC9 File Offset: 0x003420C9
	public bool IsHUDEnabled()
	{
		return this.bHUDEnabled == GUIWindowManager.HudEnabledStates.Enabled;
	}

	// Token: 0x060080A9 RID: 32937 RVA: 0x00343ED4 File Offset: 0x003420D4
	public bool IsHUDPartialHidden()
	{
		return this.bHUDEnabled == GUIWindowManager.HudEnabledStates.PartialHide;
	}

	// Token: 0x060080AA RID: 32938 RVA: 0x00343EDF File Offset: 0x003420DF
	public bool IsFullHUDDisabled()
	{
		return this.bHUDEnabled == GUIWindowManager.HudEnabledStates.FullHide;
	}

	// Token: 0x060080AB RID: 32939 RVA: 0x00343EEA File Offset: 0x003420EA
	public void ToggleHUDEnabled()
	{
		if (this.bHUDEnabled == GUIWindowManager.HudEnabledStates.FullHide)
		{
			this.bHUDEnabled = GUIWindowManager.HudEnabledStates.Enabled;
		}
		else
		{
			this.bHUDEnabled++;
		}
		this.SetHUDEnabled(this.bHUDEnabled);
	}

	// Token: 0x060080AC RID: 32940 RVA: 0x00343F18 File Offset: 0x00342118
	public void TempHUDDisable()
	{
		this.bTempEnabled = this.bHUDEnabled;
		this.bHUDEnabled = GUIWindowManager.HudEnabledStates.FullHide;
		this.SetHUDEnabled(this.bHUDEnabled);
	}

	// Token: 0x060080AD RID: 32941 RVA: 0x00343F39 File Offset: 0x00342139
	public void ReEnableHUD()
	{
		this.bHUDEnabled = this.bTempEnabled;
		this.SetHUDEnabled(this.bHUDEnabled);
	}

	// Token: 0x060080AE RID: 32942 RVA: 0x00343F54 File Offset: 0x00342154
	public void SetHUDEnabled(GUIWindowManager.HudEnabledStates _hudState)
	{
		this.bHUDEnabled = _hudState;
		if (_hudState <= GUIWindowManager.HudEnabledStates.PartialHide)
		{
			this.nguiWindowManager.ShowAll(true);
			this.playerUI.xui.transform.gameObject.SetActive(true);
			return;
		}
		if (_hudState != GUIWindowManager.HudEnabledStates.FullHide)
		{
			return;
		}
		this.nguiWindowManager.ShowAll(false);
		this.playerUI.xui.transform.gameObject.SetActive(false);
	}

	// Token: 0x060080AF RID: 32943 RVA: 0x00343FC0 File Offset: 0x003421C0
	public void ResetActionSets()
	{
		ActionSetManager actionSetManager = this.playerUI.ActionSetManager;
		if (actionSetManager == null)
		{
			return;
		}
		actionSetManager.Reset();
		actionSetManager.Push(this.playerUI.playerInput);
		foreach (GUIWindow guiwindow in this.windows)
		{
			if (guiwindow.isShowing && guiwindow.HasActionSet())
			{
				this.EnableWindowActionSet(guiwindow);
			}
		}
		foreach (GUIWindow guiwindow2 in this.windowsToOpen)
		{
			if (guiwindow2.bActionSetEnabled)
			{
				this.EnableWindowActionSet(guiwindow2);
			}
		}
	}

	// Token: 0x04006355 RID: 25429
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<GUIWindow> windowsToOpen = new List<GUIWindow>();

	// Token: 0x04006356 RID: 25430
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<GUIWindow> windowsToRemove = new List<GUIWindow>();

	// Token: 0x04006357 RID: 25431
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<GUIWindow> windows = new List<GUIWindow>();

	// Token: 0x04006358 RID: 25432
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly Dictionary<string, GUIWindow> nameToWindowMap = new CaseInsensitiveStringDictionary<GUIWindow>();

	// Token: 0x04006359 RID: 25433
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GUIWindow topmostWindow;

	// Token: 0x0400635A RID: 25434
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GUIWindow modalWindow;

	// Token: 0x0400635B RID: 25435
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool cursorWindowOpen;

	// Token: 0x0400635C RID: 25436
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<NGuiAction> globalActions = new List<NGuiAction>();

	// Token: 0x0400635D RID: 25437
	public bool IsInputLocked;

	// Token: 0x0400635E RID: 25438
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public NGUIWindowManager nguiWindowManager;

	// Token: 0x0400635F RID: 25439
	public LocalPlayerUI playerUI;

	// Token: 0x04006360 RID: 25440
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<NGuiAction> lastActionClicked = new List<NGuiAction>();

	// Token: 0x04006361 RID: 25441
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<NGuiAction> actionsToClear = new List<NGuiAction>();

	// Token: 0x04006362 RID: 25442
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<NGuiAction> actionsForGlobalHotkeys = new List<NGuiAction>();

	// Token: 0x04006363 RID: 25443
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GUIWindowManager.HudEnabledStates bHUDEnabled;

	// Token: 0x04006364 RID: 25444
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GUIWindowManager.HudEnabledStates bTempEnabled;

	// Token: 0x02000FC3 RID: 4035
	public enum HudEnabledStates
	{
		// Token: 0x04006366 RID: 25446
		Enabled,
		// Token: 0x04006367 RID: 25447
		PartialHide,
		// Token: 0x04006368 RID: 25448
		FullHide
	}
}
