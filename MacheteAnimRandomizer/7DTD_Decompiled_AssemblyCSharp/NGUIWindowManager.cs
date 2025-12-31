using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001046 RID: 4166
public class NGUIWindowManager : MonoBehaviour
{
	// Token: 0x17000DBF RID: 3519
	// (get) Token: 0x060083B9 RID: 33721 RVA: 0x00353EEC File Offset: 0x003520EC
	public GUIWindowManager WindowManager
	{
		get
		{
			if (!(this.playerUI != null))
			{
				return null;
			}
			return this.playerUI.windowManager;
		}
	}

	// Token: 0x17000DC0 RID: 3520
	// (get) Token: 0x060083BA RID: 33722 RVA: 0x00353F09 File Offset: 0x00352109
	// (set) Token: 0x060083BB RID: 33723 RVA: 0x00353F11 File Offset: 0x00352111
	public NGuiWdwInGameHUD InGameHUD { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x060083BC RID: 33724 RVA: 0x00353F1A File Offset: 0x0035211A
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		this.playerUI = base.GetComponent<LocalPlayerUI>();
		this.bGlobalShowFlag = true;
		this.ParseWindows();
	}

	// Token: 0x060083BD RID: 33725 RVA: 0x00353F38 File Offset: 0x00352138
	public void ParseWindows()
	{
		if (this.parsedWindows)
		{
			return;
		}
		this.parsedWindows = true;
		foreach (Transform transform in this.Windows)
		{
			if (!(transform == null))
			{
				EnumNGUIWindow key = EnumUtils.Parse<EnumNGUIWindow>(transform.name.Substring(3), false);
				this.windowMap[key] = transform;
			}
		}
		Transform window = this.GetWindow(EnumNGUIWindow.InGameHUD);
		if (window != null)
		{
			this.InGameHUD = window.GetComponent<NGuiWdwInGameHUD>();
			return;
		}
		Log.Error("Window wdwInGameHUD not found!");
	}

	// Token: 0x060083BE RID: 33726 RVA: 0x00353FC4 File Offset: 0x003521C4
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform GetWindow(EnumNGUIWindow _wdw)
	{
		Transform result;
		if (!this.windowMap.TryGetValue(_wdw, out result))
		{
			Log.Error("NGUIWindowManager.GetWindow: Window " + _wdw.ToStringCached<EnumNGUIWindow>() + " not found!");
		}
		return result;
	}

	// Token: 0x060083BF RID: 33727 RVA: 0x00353FFC File Offset: 0x003521FC
	public void ShowAll(bool _bShow)
	{
		this.bGlobalShowFlag = _bShow;
		if (!_bShow)
		{
			this.windowsVisibleBeforeHide.Clear();
			using (Dictionary<EnumNGUIWindow, Transform>.Enumerator enumerator = this.windowMap.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<EnumNGUIWindow, Transform> keyValuePair = enumerator.Current;
					EnumNGUIWindow enumNGUIWindow;
					Transform transform;
					keyValuePair.Deconstruct(out enumNGUIWindow, out transform);
					EnumNGUIWindow enumNGUIWindow2 = enumNGUIWindow;
					if (this.IsShowing(enumNGUIWindow2))
					{
						this.windowsVisibleBeforeHide.Add(enumNGUIWindow2);
						this.Show(enumNGUIWindow2, false);
					}
				}
				return;
			}
		}
		foreach (EnumNGUIWindow eWindow in this.windowsVisibleBeforeHide)
		{
			this.Show(eWindow, true);
		}
	}

	// Token: 0x060083C0 RID: 33728 RVA: 0x003540D0 File Offset: 0x003522D0
	public bool IsShowing(EnumNGUIWindow _eWindow)
	{
		Transform window = this.GetWindow(_eWindow);
		return window != null && window.gameObject.activeSelf;
	}

	// Token: 0x060083C1 RID: 33729 RVA: 0x003540FC File Offset: 0x003522FC
	public void Show(EnumNGUIWindow _eWindow, bool _bEnable)
	{
		Transform window = this.GetWindow(_eWindow);
		if (window == null)
		{
			return;
		}
		window.gameObject.SetActive(_bEnable && this.bGlobalShowFlag);
		if (_eWindow == EnumNGUIWindow.Loading && this.ellipsisAnimator != null)
		{
			this.ellipsisAnimator = null;
		}
	}

	// Token: 0x060083C2 RID: 33730 RVA: 0x00354148 File Offset: 0x00352348
	public void SetLabelText(EnumNGUIWindow _eElement, string _text, bool _toUpper = true)
	{
		Transform window = this.GetWindow(_eElement);
		if (window == null)
		{
			return;
		}
		this.Show(_eElement, !string.IsNullOrEmpty(_text));
		if (string.IsNullOrEmpty(_text))
		{
			return;
		}
		UILabel component = window.GetComponent<UILabel>();
		if (!component)
		{
			return;
		}
		_text = ((!_toUpper) ? _text : _text.ToUpper());
		component.text = _text;
		if (_eElement == EnumNGUIWindow.Loading)
		{
			this.ellipsisAnimator = new TextEllipsisAnimator(_text, component);
		}
	}

	// Token: 0x060083C3 RID: 33731 RVA: 0x003541B8 File Offset: 0x003523B8
	public void SetLabel(EnumNGUIWindow _eElement, string _text, Color? _color = null, bool _toUpper = true)
	{
		Transform window = this.GetWindow(_eElement);
		if (window == null)
		{
			return;
		}
		this.Show(_eElement, !string.IsNullOrEmpty(_text));
		UILabel component = window.GetComponent<UILabel>();
		if (!component)
		{
			return;
		}
		_text = ((!_toUpper) ? _text : ((_text != null) ? _text.ToUpper() : null));
		component.text = (_text ?? "");
		if (_color != null)
		{
			component.color = _color.Value;
		}
		if (_eElement == EnumNGUIWindow.Loading)
		{
			this.ellipsisAnimator = new TextEllipsisAnimator(_text + "...", component);
		}
	}

	// Token: 0x060083C4 RID: 33732 RVA: 0x00354250 File Offset: 0x00352450
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.playerUI == null)
		{
			this.playerUI = base.GetComponent<LocalPlayerUI>();
		}
		if (!this.playerUI.isPrimaryUI)
		{
			return;
		}
		GUIWindowManager windowManager = this.WindowManager;
		bool alwaysShowVersionUi = this.AlwaysShowVersionUi;
		this.ShowVersionUi(alwaysShowVersionUi);
		if (this.ellipsisAnimator != null)
		{
			this.ellipsisAnimator.GetNextAnimatedString(Time.unscaledDeltaTime);
		}
	}

	// Token: 0x17000DC1 RID: 3521
	// (get) Token: 0x060083C5 RID: 33733 RVA: 0x003542B2 File Offset: 0x003524B2
	// (set) Token: 0x060083C6 RID: 33734 RVA: 0x003542BA File Offset: 0x003524BA
	public bool AlwaysShowVersionUi { get; set; } = true;

	// Token: 0x17000DC2 RID: 3522
	// (get) Token: 0x060083C7 RID: 33735 RVA: 0x003542C3 File Offset: 0x003524C3
	public bool VersionUiVisible
	{
		get
		{
			return this.GetWindow(EnumNGUIWindow.Version).gameObject.activeSelf;
		}
	}

	// Token: 0x060083C8 RID: 33736 RVA: 0x003542D6 File Offset: 0x003524D6
	public void ToggleVersionUi()
	{
		this.ShowVersionUi(!this.VersionUiVisible);
	}

	// Token: 0x060083C9 RID: 33737 RVA: 0x003542E7 File Offset: 0x003524E7
	public void ShowVersionUi(bool _show)
	{
		this.GetWindow(EnumNGUIWindow.Version).gameObject.SetActive(_show);
	}

	// Token: 0x060083CA RID: 33738 RVA: 0x003542FB File Offset: 0x003524FB
	public void SetBackgroundScale(float _uiScale)
	{
		this.GetWindow(EnumNGUIWindow.MainMenuBackground).localScale = new Vector3(_uiScale, _uiScale, _uiScale);
	}

	// Token: 0x040065B8 RID: 26040
	public Transform[] Windows;

	// Token: 0x040065B9 RID: 26041
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly Dictionary<EnumNGUIWindow, Transform> windowMap = new EnumDictionary<EnumNGUIWindow, Transform>();

	// Token: 0x040065BA RID: 26042
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly HashSet<EnumNGUIWindow> windowsVisibleBeforeHide = new HashSet<EnumNGUIWindow>();

	// Token: 0x040065BB RID: 26043
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bGlobalShowFlag;

	// Token: 0x040065BC RID: 26044
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LocalPlayerUI playerUI;

	// Token: 0x040065BD RID: 26045
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool parsedWindows;

	// Token: 0x040065BF RID: 26047
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public TextEllipsisAnimator ellipsisAnimator;
}
