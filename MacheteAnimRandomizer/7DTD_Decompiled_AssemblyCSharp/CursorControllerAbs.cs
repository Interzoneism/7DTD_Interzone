using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

// Token: 0x02001068 RID: 4200
public abstract class CursorControllerAbs : MonoBehaviour, IGamePrefsChangedListener
{
	// Token: 0x17000DD1 RID: 3537
	// (get) Token: 0x060084B0 RID: 33968 RVA: 0x003614FB File Offset: 0x0035F6FB
	public static bool PrefabReady
	{
		get
		{
			return CursorControllerAbs.softCursorPrefab != null;
		}
	}

	// Token: 0x17000DD2 RID: 3538
	// (get) Token: 0x060084B1 RID: 33969 RVA: 0x00361508 File Offset: 0x0035F708
	// (set) Token: 0x060084B2 RID: 33970 RVA: 0x00361510 File Offset: 0x0035F710
	public virtual XUiView HoverTarget
	{
		get
		{
			return this.hoverTarget;
		}
		set
		{
			this.hoverTarget = value;
			this.bHasHoverTarget = (value != null);
		}
	}

	// Token: 0x17000DD3 RID: 3539
	// (get) Token: 0x060084B3 RID: 33971 RVA: 0x00361523 File Offset: 0x0035F723
	// (set) Token: 0x060084B4 RID: 33972 RVA: 0x0036152B File Offset: 0x0035F72B
	public XUiView navigationTarget { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x17000DD4 RID: 3540
	// (get) Token: 0x060084B5 RID: 33973 RVA: 0x00361534 File Offset: 0x0035F734
	// (set) Token: 0x060084B6 RID: 33974 RVA: 0x0036153C File Offset: 0x0035F73C
	public XUiView lockNavigationToView { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x17000DD5 RID: 3541
	// (get) Token: 0x060084B7 RID: 33975 RVA: 0x00361545 File Offset: 0x0035F745
	// (set) Token: 0x060084B8 RID: 33976 RVA: 0x0036154D File Offset: 0x0035F74D
	public bool Locked
	{
		get
		{
			return this._locked;
		}
		set
		{
			this._locked = value;
		}
	}

	// Token: 0x17000DD6 RID: 3542
	// (get) Token: 0x060084B9 RID: 33977 RVA: 0x00361556 File Offset: 0x0035F756
	// (set) Token: 0x060084BA RID: 33978 RVA: 0x0036155E File Offset: 0x0035F75E
	public bool VirtualCursorHidden
	{
		get
		{
			return this._virtualCursorHidden;
		}
		set
		{
			if (value != this._virtualCursorHidden)
			{
				this._virtualCursorHidden = value;
				this.OnVirtualCursorVisibleChanged();
			}
		}
	}

	// Token: 0x17000DD7 RID: 3543
	// (get) Token: 0x060084BB RID: 33979 RVA: 0x00361576 File Offset: 0x0035F776
	public XUiView CurrentTarget
	{
		get
		{
			if (this.CursorModeActive)
			{
				return this.hoverTarget;
			}
			return this.navigationTarget;
		}
	}

	// Token: 0x17000DD8 RID: 3544
	// (get) Token: 0x060084BC RID: 33980 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool CursorModeActive
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060084BD RID: 33981
	public abstract Vector2 GetScreenPosition();

	// Token: 0x060084BE RID: 33982
	public abstract Vector2 GetLocalScreenPosition();

	// Token: 0x060084BF RID: 33983
	public abstract void SetScreenPosition(Vector2 _newPosition);

	// Token: 0x060084C0 RID: 33984
	public abstract void SetScreenPosition(float _x, float _y);

	// Token: 0x060084C1 RID: 33985
	public abstract void SetNavigationTarget(XUiView _view);

	// Token: 0x060084C2 RID: 33986
	public abstract void SetNavigationTargetLater(XUiView _view);

	// Token: 0x060084C3 RID: 33987
	public abstract void ResetNavigationTarget();

	// Token: 0x060084C4 RID: 33988
	public abstract void ResetToCenter();

	// Token: 0x060084C5 RID: 33989
	public abstract void SetNavigationLockView(XUiView _view, XUiView _viewToSelect = null);

	// Token: 0x060084C6 RID: 33990
	public abstract void RefreshSelection();

	// Token: 0x060084C7 RID: 33991
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void OnVirtualCursorVisibleChanged();

	// Token: 0x060084C8 RID: 33992 RVA: 0x0036158D File Offset: 0x0035F78D
	public void SetGUIActions(PlayerActionsGUI _guiActions)
	{
		this.guiActions = _guiActions;
	}

	// Token: 0x060084C9 RID: 33993 RVA: 0x00361596 File Offset: 0x0035F796
	public void SetWindowManager(GUIWindowManager _windowManager)
	{
		this.windowManager = _windowManager;
	}

	// Token: 0x060084CA RID: 33994 RVA: 0x0036159F File Offset: 0x0035F79F
	public static void UpdateGamePrefs()
	{
		CursorControllerAbs.bSnapCursor = GamePrefs.GetBool(EnumGamePrefs.OptionsControllerCursorSnap);
		CursorControllerAbs.regularSpeed = GamePrefs.GetFloat(EnumGamePrefs.OptionsInterfaceSensitivity);
		CursorControllerAbs.hoverSpeed = GamePrefs.GetFloat(EnumGamePrefs.OptionsControllerCursorHoverSensitivity);
	}

	// Token: 0x060084CB RID: 33995 RVA: 0x003615CE File Offset: 0x0035F7CE
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void AwakeBase()
	{
		GamePrefs.AddChangeListener(this);
		GameOptionsManager.ResolutionChanged += this.OnResolutionChanged;
		CursorControllerAbs.UpdateGamePrefs();
		CursorControllerAbs.softCursors.Add(this);
	}

	// Token: 0x060084CC RID: 33996 RVA: 0x003615F7 File Offset: 0x0035F7F7
	[PublicizedFrom(EAccessModifier.Protected)]
	public void DestroyBase()
	{
		GamePrefs.RemoveChangeListener(this);
		GameOptionsManager.ResolutionChanged -= this.OnResolutionChanged;
		CursorControllerAbs.softCursors.Remove(this);
	}

	// Token: 0x060084CD RID: 33997 RVA: 0x0036161C File Offset: 0x0035F81C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void InitCursorBounds()
	{
		this.cursorWorldBounds = new Bounds(this.cursor.worldCenter, Vector3.zero);
		Vector3[] worldCorners = this.cursor.worldCorners;
		for (int i = 0; i < worldCorners.Length; i++)
		{
			this.cursorWorldBounds.Encapsulate(worldCorners[i]);
		}
		Bounds bounds = new Bounds(this.uiCamera.cachedCamera.WorldToScreenPoint(this.cursorWorldBounds.min), Vector3.zero);
		bounds.Encapsulate(this.uiCamera.cachedCamera.WorldToScreenPoint(this.cursorWorldBounds.max));
		this.cursorBuffer = bounds.extents;
	}

	// Token: 0x060084CE RID: 33998
	public abstract void UpdateMoveSpeed();

	// Token: 0x060084CF RID: 33999 RVA: 0x003616C6 File Offset: 0x0035F8C6
	public void UpdateBounds(string _boundsName, Bounds _bounds)
	{
		_bounds.Expand(this.cursorBuffer);
		this.activeBounds[_boundsName] = _bounds;
		this.RefreshBounds();
	}

	// Token: 0x060084D0 RID: 34000 RVA: 0x003616E8 File Offset: 0x0035F8E8
	public void RemoveBounds(string _boundsName)
	{
		this.activeBounds.Remove(_boundsName);
		this.RefreshBounds();
	}

	// Token: 0x060084D1 RID: 34001 RVA: 0x00361700 File Offset: 0x0035F900
	public void RefreshBounds()
	{
		this.currentBounds.size = Vector3.zero;
		if (this.activeBounds.Count > 0)
		{
			bool flag = true;
			using (Dictionary<string, Bounds>.Enumerator enumerator = this.activeBounds.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, Bounds> keyValuePair = enumerator.Current;
					if (flag)
					{
						this.currentBounds.center = keyValuePair.Value.center;
						flag = false;
					}
					this.currentBounds.Encapsulate(keyValuePair.Value);
				}
				return;
			}
		}
		this.currentBounds.center = new Vector3(0f, 0f);
		this.currentBounds.Encapsulate(new Vector3((float)this.uiCamera.cachedCamera.pixelWidth, (float)this.uiCamera.cachedCamera.pixelHeight));
	}

	// Token: 0x060084D2 RID: 34002 RVA: 0x003617EC File Offset: 0x0035F9EC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnResolutionChanged(int _width, int _height)
	{
		base.StartCoroutine(this.RefreshBoundsNextFrame());
	}

	// Token: 0x060084D3 RID: 34003 RVA: 0x003617FB File Offset: 0x0035F9FB
	public void OnGamePrefChanged(EnumGamePrefs _enum)
	{
		if (_enum == EnumGamePrefs.OptionsInterfaceSensitivity)
		{
			this.UpdateMoveSpeed();
		}
	}

	// Token: 0x060084D4 RID: 34004 RVA: 0x0036180B File Offset: 0x0035FA0B
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator RefreshBoundsNextFrame()
	{
		yield return null;
		this.RefreshBounds();
		yield break;
	}

	// Token: 0x060084D5 RID: 34005
	public abstract void SetCursorHidden(bool _hidden);

	// Token: 0x060084D6 RID: 34006
	public abstract bool GetCursorHidden();

	// Token: 0x060084D7 RID: 34007 RVA: 0x0036181C File Offset: 0x0035FA1C
	public bool GetMouseButtonDown(UICamera.MouseButton _mouseButton)
	{
		if (this.guiActions == null)
		{
			return false;
		}
		if (GameManager.Instance.m_GUIConsole.isShowing)
		{
			return false;
		}
		switch (_mouseButton)
		{
		case UICamera.MouseButton.LeftButton:
			return this.guiActions.Submit.WasPressed || this.guiActions.LeftClick.WasPressed;
		case UICamera.MouseButton.RightButton:
			return this.guiActions.Inspect.WasPressed || this.guiActions.RightClick.WasPressed;
		case UICamera.MouseButton.MiddleButton:
			return false;
		default:
			return false;
		}
	}

	// Token: 0x060084D8 RID: 34008 RVA: 0x003618A8 File Offset: 0x0035FAA8
	public bool GetMouseButton(UICamera.MouseButton _mouseButton)
	{
		if (this.guiActions == null)
		{
			return false;
		}
		if (GameManager.Instance.m_GUIConsole.isShowing)
		{
			return false;
		}
		switch (_mouseButton)
		{
		case UICamera.MouseButton.LeftButton:
			return this.guiActions.Submit.IsPressed || this.guiActions.LeftClick.IsPressed;
		case UICamera.MouseButton.RightButton:
			return this.guiActions.Inspect.IsPressed || this.guiActions.RightClick.IsPressed;
		case UICamera.MouseButton.MiddleButton:
			return false;
		default:
			return false;
		}
	}

	// Token: 0x060084D9 RID: 34009 RVA: 0x00361934 File Offset: 0x0035FB34
	public bool GetMouseButtonUp(UICamera.MouseButton _mouseButton)
	{
		if (this.guiActions == null)
		{
			return false;
		}
		switch (_mouseButton)
		{
		case UICamera.MouseButton.LeftButton:
			return this.guiActions.Submit.WasReleased || this.guiActions.LeftClick.WasReleased;
		case UICamera.MouseButton.RightButton:
			return this.guiActions.Inspect.WasReleased || this.guiActions.RightClick.WasReleased;
		case UICamera.MouseButton.MiddleButton:
			return false;
		default:
			return false;
		}
	}

	// Token: 0x060084DA RID: 34010 RVA: 0x003619AC File Offset: 0x0035FBAC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void DebugDrawBound(Bounds _bound)
	{
		Vector3 vector = _bound.max;
		Vector3 vector2 = _bound.min;
		Vector3 vector3 = new Vector3(vector2.x, vector.y, vector.z);
		Vector3 vector4 = new Vector3(vector.x, vector2.y, vector2.z);
		vector = this.uiCamera.cachedCamera.ScreenToWorldPoint(vector);
		vector3 = this.uiCamera.cachedCamera.ScreenToWorldPoint(vector3);
		vector4 = this.uiCamera.cachedCamera.ScreenToWorldPoint(vector4);
		vector2 = this.uiCamera.cachedCamera.ScreenToWorldPoint(vector2);
		Debug.DrawLine(vector, vector3);
		Debug.DrawLine(vector4, vector2);
		Debug.DrawLine(vector, vector4);
		Debug.DrawLine(vector3, vector2);
	}

	// Token: 0x060084DB RID: 34011 RVA: 0x00361A5F File Offset: 0x0035FC5F
	public static void SetCursor(CursorControllerAbs.ECursorType _cursorType)
	{
		SoftCursor.SetCursor(_cursorType);
	}

	// Token: 0x060084DC RID: 34012 RVA: 0x00361A67 File Offset: 0x0035FC67
	public static void LoadStaticData(LoadManager.LoadGroup _loadGroup)
	{
		LoadManager.LoadAssetFromResources<GameObject>(CursorControllerAbs.softCursorPrefabPath, delegate(GameObject _asset)
		{
			CursorControllerAbs.softCursorPrefab = _asset;
		}, null, false, true);
	}

	// Token: 0x060084DD RID: 34013 RVA: 0x00361A98 File Offset: 0x0035FC98
	public static CursorControllerAbs AddSoftCursor(UICamera _camera, PlayerActionsGUI _guiActions, GUIWindowManager _windowManager)
	{
		GameObject gameObject = _camera.gameObject.AddChild(CursorControllerAbs.softCursorPrefab);
		SoftCursor component = gameObject.GetComponent<SoftCursor>();
		component.SetGUIActions(_guiActions);
		component.SetWindowManager(_windowManager);
		_camera.cancelKey0 = KeyCode.None;
		_camera.submitKey1 = KeyCode.None;
		_camera.cancelKey1 = KeyCode.None;
		UICamera.GetMousePosition = new UICamera.GetMousePositionFunc(component.GetScreenPosition);
		UICamera.GetMouseButton = new UICamera.GetMouseButtonFunc(component.GetMouseButton);
		UICamera.GetMouseButtonDown = new UICamera.GetMouseButtonFunc(component.GetMouseButtonDown);
		UICamera.GetMouseButtonUp = new UICamera.GetMouseButtonFunc(component.GetMouseButtonUp);
		gameObject.SetActive(true);
		return component;
	}

	// Token: 0x060084DE RID: 34014 RVA: 0x00361B2B File Offset: 0x0035FD2B
	public void PlayPagingSound()
	{
		Manager.PlayXUiSound(this.pagingSound, 1f);
	}

	// Token: 0x060084DF RID: 34015 RVA: 0x00361B3D File Offset: 0x0035FD3D
	[PublicizedFrom(EAccessModifier.Protected)]
	public CursorControllerAbs()
	{
	}

	// Token: 0x040066D5 RID: 26325
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string softCursorPrefabPath = "Prefabs/SoftCursor";

	// Token: 0x040066D6 RID: 26326
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static GameObject softCursorPrefab;

	// Token: 0x040066D7 RID: 26327
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static List<CursorControllerAbs> softCursors = new List<CursorControllerAbs>();

	// Token: 0x040066D8 RID: 26328
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public UICamera uiCamera;

	// Token: 0x040066D9 RID: 26329
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public PlayerActionsGUI guiActions;

	// Token: 0x040066DA RID: 26330
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public GUIWindowManager windowManager;

	// Token: 0x040066DB RID: 26331
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public UISprite cursor;

	// Token: 0x040066DC RID: 26332
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Bounds cursorWorldBounds;

	// Token: 0x040066DD RID: 26333
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 cursorBuffer;

	// Token: 0x040066DE RID: 26334
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Dictionary<string, Bounds> activeBounds = new Dictionary<string, Bounds>();

	// Token: 0x040066DF RID: 26335
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Bounds currentBounds;

	// Token: 0x040066E0 RID: 26336
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AudioClip cursorSelectSound;

	// Token: 0x040066E1 RID: 26337
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AudioClip pagingSound;

	// Token: 0x040066E2 RID: 26338
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public XUiView hoverTarget;

	// Token: 0x040066E3 RID: 26339
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bHasHoverTarget;

	// Token: 0x040066E4 RID: 26340
	public static bool bSnapCursor;

	// Token: 0x040066E5 RID: 26341
	public static float regularSpeed = 1f;

	// Token: 0x040066E6 RID: 26342
	public static float hoverSpeed = 1f;

	// Token: 0x040066E9 RID: 26345
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Protected)]
	public AnimationCurve accelerationCurve;

	// Token: 0x040066EA RID: 26346
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool _locked;

	// Token: 0x040066EB RID: 26347
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool _virtualCursorHidden;

	// Token: 0x040066EC RID: 26348
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static bool FreeCursorEnabled = true;

	// Token: 0x02001069 RID: 4201
	public enum InputType
	{
		// Token: 0x040066EE RID: 26350
		Controller,
		// Token: 0x040066EF RID: 26351
		Mouse,
		// Token: 0x040066F0 RID: 26352
		Both
	}

	// Token: 0x0200106A RID: 4202
	public enum ECursorType
	{
		// Token: 0x040066F2 RID: 26354
		None,
		// Token: 0x040066F3 RID: 26355
		Default,
		// Token: 0x040066F4 RID: 26356
		Map,
		// Token: 0x040066F5 RID: 26357
		Count
	}
}
