using System;
using System.Collections;
using Audio;
using InControl;
using Platform;
using UnityEngine;

// Token: 0x0200106D RID: 4205
public class SoftCursor : CursorControllerAbs
{
	// Token: 0x17000DDB RID: 3547
	// (get) Token: 0x060084EA RID: 34026 RVA: 0x00361BF5 File Offset: 0x0035FDF5
	// (set) Token: 0x060084EB RID: 34027 RVA: 0x00361BFD File Offset: 0x0035FDFD
	public PlayerInputManager.InputStyle LastInputStyle
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.m_lastInputStyle;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			if (value != this.m_lastInputStyle)
			{
				this.m_lastInputStyle = value;
				this.SetVisible(this.SoftcursorAllowed);
			}
		}
	}

	// Token: 0x17000DDC RID: 3548
	// (get) Token: 0x060084EC RID: 34028 RVA: 0x00361C1B File Offset: 0x0035FE1B
	// (set) Token: 0x060084ED RID: 34029 RVA: 0x00361C23 File Offset: 0x0035FE23
	public bool SoftcursorEnabled
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.m_softcursorEnabled;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (value != this.m_softcursorEnabled)
			{
				this.m_softcursorEnabled = value;
				this.SetVisible(this.SoftcursorAllowed);
			}
		}
	}

	// Token: 0x17000DDD RID: 3549
	// (get) Token: 0x060084EE RID: 34030 RVA: 0x00361C41 File Offset: 0x0035FE41
	// (set) Token: 0x060084EF RID: 34031 RVA: 0x00361C4C File Offset: 0x0035FE4C
	public override XUiView HoverTarget
	{
		get
		{
			return base.HoverTarget;
		}
		set
		{
			base.HoverTarget = value;
			if (this.hoverTarget != null && this.LastInputStyle != PlayerInputManager.InputStyle.Keyboard && !this.movingMouse && this.hoverTarget.IsNavigatable && !this.hoverTarget.HasHoverSound)
			{
				Manager.PlayXUiSound(this.cursorSelectSound, 0.75f);
			}
		}
	}

	// Token: 0x17000DDE RID: 3550
	// (get) Token: 0x060084F0 RID: 34032 RVA: 0x00361CA3 File Offset: 0x0035FEA3
	public override bool CursorModeActive
	{
		get
		{
			return this.cursorModeActive;
		}
	}

	// Token: 0x17000DDF RID: 3551
	// (get) Token: 0x060084F1 RID: 34033 RVA: 0x00361CAB File Offset: 0x0035FEAB
	public bool SoftcursorAllowed
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.guiActions != null && this.guiActions.Enabled && LocalPlayerUI.AnyModalWindowOpen();
		}
	}

	// Token: 0x17000DE0 RID: 3552
	// (get) Token: 0x060084F2 RID: 34034 RVA: 0x00361CC9 File Offset: 0x0035FEC9
	// (set) Token: 0x060084F3 RID: 34035 RVA: 0x00361CDB File Offset: 0x0035FEDB
	public Vector3 Position
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.cursor.transform.position;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			this.cursor.transform.position = value;
			this.lastMousePosition = Input.mousePosition;
		}
	}

	// Token: 0x17000DE1 RID: 3553
	// (get) Token: 0x060084F4 RID: 34036 RVA: 0x00361CFE File Offset: 0x0035FEFE
	// (set) Token: 0x060084F5 RID: 34037 RVA: 0x00361D10 File Offset: 0x0035FF10
	public Vector3 LocalPosition
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.cursor.transform.localPosition;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			this.cursor.transform.localPosition = value;
			this.lastMousePosition = Input.mousePosition;
		}
	}

	// Token: 0x17000DE2 RID: 3554
	// (get) Token: 0x060084F6 RID: 34038 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public static CursorLockMode DefaultCursorLockState
	{
		get
		{
			return CursorLockMode.None;
		}
	}

	// Token: 0x060084F7 RID: 34039 RVA: 0x00361D34 File Offset: 0x0035FF34
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.AwakeBase();
		Cursor.lockState = SoftCursor.DefaultCursorLockState;
		foreach (UISprite uisprite in base.GetComponentsInChildren<UISprite>())
		{
			string name = uisprite.gameObject.name;
			if (name == "Cursor")
			{
				this.cursor = uisprite;
			}
			else if (name == "SelectionBox")
			{
				this.selectionBox = uisprite;
			}
		}
		this.cursorPanel = base.GetComponent<UIPanel>();
		GameObject gameObject = new GameObject("cursorMouse");
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		if (SoftCursor.defaultMouseCursorAtlas == null)
		{
			foreach (UIAtlas uiatlas in Resources.FindObjectsOfTypeAll<UIAtlas>())
			{
				if (uiatlas.name.EqualsCaseInsensitive(SoftCursor.defaultMouseCursorAtlasName))
				{
					SoftCursor.defaultMouseCursorAtlas = uiatlas;
				}
				if (uiatlas.name.EqualsCaseInsensitive(SoftCursor.emptyCursorAtlasName))
				{
					SoftCursor.emptyCursorAtlas = uiatlas;
				}
				if (uiatlas.name.EqualsCaseInsensitive(SoftCursor.defaultControllerCursorAtlasName))
				{
					SoftCursor.defaultControllerCursorAtlas = uiatlas;
				}
				if (uiatlas.name.EqualsCaseInsensitive(SoftCursor.mapCursorAtlasName))
				{
					SoftCursor.mapCursorAtlas = uiatlas;
				}
			}
		}
		LocalPlayerUI.primaryUI.xui.LoadData<AudioClip>("@:Sounds/UI/ui_hover.wav", delegate(AudioClip _o)
		{
			this.cursorSelectSound = _o;
		});
		LocalPlayerUI.primaryUI.xui.LoadData<AudioClip>("@:Sounds/UI/ui_tab.wav", delegate(AudioClip _o)
		{
			this.pagingSound = _o;
		});
		PlatformManager.NativePlatform.Input.OnLastInputStyleChanged += this.OnLastInputStyleChanged;
	}

	// Token: 0x060084F8 RID: 34040 RVA: 0x00361EDC File Offset: 0x003600DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnLastInputStyleChanged(PlayerInputManager.InputStyle _style)
	{
		if (_style != PlayerInputManager.InputStyle.Keyboard)
		{
			if (this.LastInputStyle == PlayerInputManager.InputStyle.Keyboard)
			{
				this.LocalPosition = this.cursorPanel.transform.worldToLocalMatrix.MultiplyPoint3x4(this.uiCamera.cachedCamera.ScreenToWorldPoint(Input.mousePosition));
			}
			this.RefreshSelection();
		}
		this.SetVisible(this.SoftcursorAllowed);
	}

	// Token: 0x060084F9 RID: 34041 RVA: 0x00361F3B File Offset: 0x0036013B
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.uiCamera = base.GetComponentInParent<UICamera>();
		this.UpdateMoveSpeed();
		base.InitCursorBounds();
	}

	// Token: 0x060084FA RID: 34042 RVA: 0x00361F55 File Offset: 0x00360155
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		base.DestroyBase();
	}

	// Token: 0x060084FB RID: 34043 RVA: 0x00361F60 File Offset: 0x00360160
	public override void UpdateMoveSpeed()
	{
		float @float = GamePrefs.GetFloat(EnumGamePrefs.OptionsInterfaceSensitivity);
		this.speed = 500f + 2000f * @float;
		this.mouseSpeed = (500f + 2000f * @float) / 1000f * (1f / MouseBindingSource.ScaleX) * 4f;
	}

	// Token: 0x060084FC RID: 34044 RVA: 0x00361FB8 File Offset: 0x003601B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (Application.isPlaying && !GameManager.Instance.IsQuitting && !GameManager.Instance.m_GUIConsole.isShowing)
		{
			this.LastInputStyle = PlatformManager.NativePlatform.Input.CurrentInputStyle;
			if (this.guiActions != null && this.windowManager != null)
			{
				if (this.SoftcursorAllowed != this.SoftcursorEnabled)
				{
					this.SoftcursorEnabled = (this.SoftcursorAllowed & !this.hidden);
				}
				if (this.SoftcursorEnabled)
				{
					this.HandleMovement();
				}
			}
			this.LastFrameTime = Time.realtimeSinceStartup;
		}
	}

	// Token: 0x060084FD RID: 34045 RVA: 0x00362054 File Offset: 0x00360254
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		if (this.selectionBox.enabled)
		{
			this.RefreshSelection();
		}
	}

	// Token: 0x060084FE RID: 34046 RVA: 0x0036206C File Offset: 0x0036026C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void HandleMovement()
	{
		if (this.guiActions == null)
		{
			return;
		}
		if (base.Locked || this.hidden)
		{
			return;
		}
		this.movingMouse = false;
		Vector2 lhs = Input.mousePosition;
		if (lhs != this.lastMousePosition)
		{
			this.movingMouse = true;
		}
		this.lastMousePosition = lhs;
		if (this.LastInputStyle != PlayerInputManager.InputStyle.Keyboard)
		{
			if (this.movingMouse)
			{
				this.LocalPosition = this.cursorPanel.transform.worldToLocalMatrix.MultiplyPoint3x4(this.uiCamera.cachedCamera.ScreenToWorldPoint(Input.mousePosition));
				this.SetVisible(true);
			}
			else
			{
				if (!CursorControllerAbs.FreeCursorEnabled)
				{
					return;
				}
				Vector2 vector = new Vector2(this.guiActions.Right.RawValue - this.guiActions.Left.RawValue, this.guiActions.Up.RawValue - this.guiActions.Down.RawValue);
				float magnitude = vector.magnitude;
				if (magnitude > 0f)
				{
					this.cursorModeActive = true;
					this.SetVisible(true);
					this.SetNavigationTarget(null);
				}
				Vector3 localPosition = this.LocalPosition;
				Vector3 localPosition2 = this.LocalPosition;
				if (this.bHasHoverTarget && (this.HoverTarget == null || !this.HoverTarget.ColliderEnabled || !this.HoverTarget.UiTransform.gameObject.activeInHierarchy))
				{
					this.HoverTarget = null;
				}
				float b = this.bHasHoverTarget ? CursorControllerAbs.hoverSpeed : 1f;
				this.currentAcceleration = Mathf.Clamp(this.currentAcceleration + magnitude * Time.unscaledDeltaTime, 0f, Mathf.Min(magnitude, b));
				this.speedMultiplier = Mathf.MoveTowards(this.speedMultiplier, this.bHasHoverTarget ? CursorControllerAbs.hoverSpeed : 1f, Time.unscaledDeltaTime * (this.bHasHoverTarget ? 10f : 1f));
				float num = Time.unscaledDeltaTime * this.speed * this.speedMultiplier * this.accelerationCurve.Evaluate(this.currentAcceleration);
				vector.x *= num;
				vector.y *= num;
				localPosition2.x += vector.x;
				localPosition2.y += vector.y;
				this.LocalPosition = localPosition2;
				if (CursorControllerAbs.bSnapCursor && localPosition == localPosition2)
				{
					if (!this.snapped)
					{
						this.Snap();
						this.snapped = true;
					}
				}
				else
				{
					this.snapped = false;
				}
			}
			this.ConstrainCursor();
			return;
		}
		this.LocalPosition = this.cursorPanel.transform.worldToLocalMatrix.MultiplyPoint3x4(this.uiCamera.cachedCamera.ScreenToWorldPoint(Input.mousePosition));
		this.ConstrainCursor();
	}

	// Token: 0x060084FF RID: 34047 RVA: 0x0036232C File Offset: 0x0036052C
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 ConstrainToBounds(Vector3 _newPosition)
	{
		Vector3 vector = _newPosition;
		if (this.LastInputStyle != PlayerInputManager.InputStyle.Keyboard)
		{
			vector.z = this.currentBounds.center.z;
			vector = this.currentBounds.ClosestPoint(vector);
		}
		return vector;
	}

	// Token: 0x06008500 RID: 34048 RVA: 0x0036236C File Offset: 0x0036056C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ConstrainCursor()
	{
		Vector3 newPosition = this.uiCamera.cachedCamera.WorldToScreenPoint(this.Position);
		Vector3 vector = this.ConstrainToBounds(newPosition);
		vector = this.uiCamera.cachedCamera.ScreenToViewportPoint(vector);
		if (vector.x < 0f)
		{
			vector.x = 0f;
		}
		else if (vector.x > 1f)
		{
			vector.x = 1f;
		}
		if (vector.y < 0f)
		{
			vector.y = 0f;
		}
		else if (vector.y > 1f)
		{
			vector.y = 1f;
		}
		this.Position = this.uiCamera.cachedCamera.ViewportToWorldPoint(vector);
	}

	// Token: 0x06008501 RID: 34049 RVA: 0x0036242C File Offset: 0x0036062C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Snap()
	{
		if (GamePrefs.GetBool(EnumGamePrefs.OptionsControllerCursorSnap) && this.hoverTarget != null && this.hoverTarget.IsSnappable && this.hoverTarget.ColliderEnabled && this.hoverTarget.UiTransform.gameObject.activeInHierarchy)
		{
			Bounds bounds = this.hoverTarget.bounds;
			if (this.cursorWorldBounds.extents.x > bounds.extents.x - this.OffsetSnapBounds)
			{
				this.Position = bounds.center;
				return;
			}
			Vector3 vector = bounds.ClosestPoint(this.Position);
			Vector3 b = Vector3.right * this.cursorWorldBounds.extents.x;
			Vector3 point = vector - b;
			Vector3 point2 = vector + b;
			if (!bounds.Contains(point))
			{
				vector = bounds.ClosestPoint(point) + b;
			}
			else if (!bounds.Contains(point2))
			{
				vector = bounds.ClosestPoint(point2) - b;
			}
			vector.y = bounds.center.y;
			this.Position = vector;
		}
	}

	// Token: 0x06008502 RID: 34050 RVA: 0x00362558 File Offset: 0x00360758
	public override Vector2 GetScreenPosition()
	{
		Vector2 result = default(Vector2);
		if (this.uiCamera != null)
		{
			result = this.uiCamera.cachedCamera.WorldToScreenPoint(this.Position);
		}
		return result;
	}

	// Token: 0x06008503 RID: 34051 RVA: 0x00362598 File Offset: 0x00360798
	public override Vector2 GetLocalScreenPosition()
	{
		Vector2 vector = default(Vector2);
		if (this.uiCamera != null)
		{
			vector = this.uiCamera.cachedCamera.WorldToViewportPoint(this.Position);
			vector.x *= (float)this.uiCamera.cachedCamera.pixelWidth;
			vector.y *= (float)this.uiCamera.cachedCamera.pixelHeight;
		}
		return vector;
	}

	// Token: 0x06008504 RID: 34052 RVA: 0x00362616 File Offset: 0x00360816
	public override void SetScreenPosition(Vector2 _newPosition)
	{
		this.Position = this.uiCamera.cachedCamera.ScreenToWorldPoint(_newPosition);
	}

	// Token: 0x06008505 RID: 34053 RVA: 0x00362634 File Offset: 0x00360834
	public override void SetScreenPosition(float _x, float _y)
	{
		this.SetScreenPosition(new Vector2(_x, _y));
	}

	// Token: 0x06008506 RID: 34054 RVA: 0x00362643 File Offset: 0x00360843
	public override void ResetToCenter()
	{
		this.LocalPosition = Vector3.zero;
	}

	// Token: 0x06008507 RID: 34055 RVA: 0x00362650 File Offset: 0x00360850
	public override void SetCursorHidden(bool _hidden)
	{
		this.hidden = _hidden;
		this.SoftcursorEnabled &= !this.hidden;
		SoftCursor.SetCursor(_hidden ? CursorControllerAbs.ECursorType.None : CursorControllerAbs.ECursorType.Default);
		Cursor.visible = (PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard && !this.hidden);
		if (this.LastInputStyle != PlayerInputManager.InputStyle.Keyboard && !this.hidden)
		{
			this.RefreshSelection();
		}
	}

	// Token: 0x06008508 RID: 34056 RVA: 0x003626C0 File Offset: 0x003608C0
	public override bool GetCursorHidden()
	{
		return this.hidden;
	}

	// Token: 0x06008509 RID: 34057 RVA: 0x003626C8 File Offset: 0x003608C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetCursorSprite(UIAtlas _atlasMouse, string _spriteMouse, UIWidget.Pivot _pivotMouse, UIAtlas _atlasController, string _spriteController, UIWidget.Pivot _pivotController)
	{
		this.cursor.atlas = _atlasController;
		this.cursor.spriteName = _spriteController;
		this.cursor.pivot = _pivotController;
	}

	// Token: 0x0600850A RID: 34058 RVA: 0x003626F1 File Offset: 0x003608F1
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetCursorSpriteNone()
	{
		this.cursor.atlas = null;
		this.cursor.spriteName = "";
		this.SetSelectionBoxEnabled(false);
	}

	// Token: 0x0600850B RID: 34059 RVA: 0x00362718 File Offset: 0x00360918
	public new static void SetCursor(CursorControllerAbs.ECursorType _cursorType)
	{
		switch (_cursorType)
		{
		case CursorControllerAbs.ECursorType.None:
			for (int i = 0; i < CursorControllerAbs.softCursors.Count; i++)
			{
				(CursorControllerAbs.softCursors[i] as SoftCursor).SetCursorSpriteNone();
			}
			return;
		case CursorControllerAbs.ECursorType.Default:
			for (int j = 0; j < CursorControllerAbs.softCursors.Count; j++)
			{
				(CursorControllerAbs.softCursors[j] as SoftCursor).SetCursorSprite(SoftCursor.defaultMouseCursorAtlas, SoftCursor.defaultMouseCursorSprite, SoftCursor.defaultMouseCursorPivot, SoftCursor.defaultControllerCursorAtlas, SoftCursor.defaultControllerCursorSprite, SoftCursor.defaultControllerCursorPivot);
			}
			return;
		case CursorControllerAbs.ECursorType.Map:
			for (int k = 0; k < CursorControllerAbs.softCursors.Count; k++)
			{
				(CursorControllerAbs.softCursors[k] as SoftCursor).SetCursorSprite(SoftCursor.mapCursorAtlas, SoftCursor.mapCursorSprite, SoftCursor.mapCursorPivot, SoftCursor.mapCursorAtlas, SoftCursor.mapCursorSprite, SoftCursor.mapCursorPivot);
			}
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0600850C RID: 34060 RVA: 0x00362800 File Offset: 0x00360A00
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetVisible(bool _visible)
	{
		Cursor.lockState = (_visible ? SoftCursor.DefaultCursorLockState : CursorLockMode.Locked);
		if (this.LastInputStyle == PlayerInputManager.InputStyle.Keyboard || this.movingMouse)
		{
			Cursor.visible = _visible;
			this.cursor.enabled = false;
			this.SetSelectionBoxEnabled(false);
		}
		else
		{
			Cursor.visible = false;
			this.cursor.enabled = (_visible && CursorControllerAbs.FreeCursorEnabled && !base.Locked && this.cursorModeActive && !base.VirtualCursorHidden);
			this.SetSelectionBoxEnabled(_visible && !this.cursorModeActive && !this.hidden && base.navigationTarget != null && base.navigationTarget.UseSelectionBox);
		}
		GameManager.Instance.bCursorVisible = _visible;
		this.lastMousePosition = Input.mousePosition;
	}

	// Token: 0x0600850D RID: 34061 RVA: 0x003628CD File Offset: 0x00360ACD
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnVirtualCursorVisibleChanged()
	{
		if (this.LastInputStyle == PlayerInputManager.InputStyle.Keyboard || this.movingMouse)
		{
			return;
		}
		this.cursor.enabled = (!base.VirtualCursorHidden && !base.Locked && this.cursorModeActive);
	}

	// Token: 0x0600850E RID: 34062 RVA: 0x00362905 File Offset: 0x00360B05
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetSelectionBoxEnabled(bool _enabled)
	{
		this.selectionBox.enabled = _enabled;
	}

	// Token: 0x0600850F RID: 34063 RVA: 0x00362914 File Offset: 0x00360B14
	public static void SetCursorVisible(bool _visible)
	{
		for (int i = 0; i < CursorControllerAbs.softCursors.Count; i++)
		{
			(CursorControllerAbs.softCursors[i] as SoftCursor).SetVisible(_visible);
		}
	}

	// Token: 0x06008510 RID: 34064 RVA: 0x0036294C File Offset: 0x00360B4C
	public Vector2 GetFlatPosition()
	{
		return new Vector2(this.Position.x, this.Position.y);
	}

	// Token: 0x06008511 RID: 34065 RVA: 0x0036296C File Offset: 0x00360B6C
	public override void SetNavigationTarget(XUiView _view)
	{
		if (_view != null && !_view.IsNavigatable)
		{
			this.SetNavigationTarget(null);
			return;
		}
		if (_view == base.navigationTarget)
		{
			return;
		}
		if (base.lockNavigationToView != null && (_view == null || !_view.Controller.IsChildOf(base.lockNavigationToView.Controller)))
		{
			return;
		}
		if (base.navigationTarget != null)
		{
			base.navigationTarget.Controller.OnCursorUnSelected();
		}
		if (_view != null && PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard)
		{
			_view.Controller.OnCursorSelected();
			this.cursorModeActive = false;
			this.SetSelectionBoxEnabled(_view.UseSelectionBox);
			this.PositionSelectionBox(_view);
		}
		else
		{
			this.SetSelectionBoxEnabled(false);
		}
		base.navigationTarget = _view;
		if (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard)
		{
			this.SetVisible(base.navigationTarget != null);
		}
	}

	// Token: 0x06008512 RID: 34066 RVA: 0x00362A3D File Offset: 0x00360C3D
	public override void RefreshSelection()
	{
		if (base.navigationTarget == null)
		{
			return;
		}
		this.PositionSelectionBox(base.navigationTarget);
	}

	// Token: 0x06008513 RID: 34067 RVA: 0x00362A54 File Offset: 0x00360C54
	[PublicizedFrom(EAccessModifier.Private)]
	public void PositionSelectionBox(XUiView _view)
	{
		this.Position = _view.Center;
		if (_view.UseSelectionBox)
		{
			this.selectionBox.transform.position = _view.Center;
			if (_view.Controller.GetParentWindow().IsInStackpanel)
			{
				this.selectionBox.width = (int)((float)_view.Size.x * _view.xui.transform.localScale.x * _view.xui.StackPanelTransform.localScale.x + (float)SoftCursor.selectionBoxMargin);
				this.selectionBox.height = (int)((float)_view.Size.y * _view.xui.transform.localScale.y * _view.xui.StackPanelTransform.localScale.y + (float)SoftCursor.selectionBoxMargin);
				return;
			}
			this.selectionBox.width = (int)((float)_view.Size.x * _view.xui.transform.localScale.x + (float)SoftCursor.selectionBoxMargin);
			this.selectionBox.height = (int)((float)_view.Size.y * _view.xui.transform.localScale.y + (float)SoftCursor.selectionBoxMargin);
		}
	}

	// Token: 0x06008514 RID: 34068 RVA: 0x00362BB0 File Offset: 0x00360DB0
	public override void SetNavigationLockView(XUiView _view, XUiView _viewToSelect = null)
	{
		if (_view != null && (!_view.IsVisible || !_view.UiTransform.gameObject.activeInHierarchy))
		{
			this.SetNavigationLockView(null, null);
			return;
		}
		base.lockNavigationToView = _view;
		if (_viewToSelect != null)
		{
			_viewToSelect.Controller.SelectCursorElement(true, false);
			return;
		}
		if (_view != null)
		{
			_view.Controller.SelectCursorElement(true, false);
		}
	}

	// Token: 0x06008515 RID: 34069 RVA: 0x00362C0D File Offset: 0x00360E0D
	public override void SetNavigationTargetLater(XUiView _view)
	{
		if (_view == null)
		{
			this.SetNavigationTarget(null);
			return;
		}
		base.StartCoroutine(this.SetNavigationTargetWithDelay(_view));
	}

	// Token: 0x06008516 RID: 34070 RVA: 0x00362C28 File Offset: 0x00360E28
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator SetNavigationTargetWithDelay(XUiView _view)
	{
		base.Locked = true;
		int num;
		for (int i = 0; i < 3; i = num + 1)
		{
			yield return null;
			num = i;
		}
		base.Locked = false;
		if (_view != null && _view.HasCollider)
		{
			this.SetNavigationTarget(_view);
		}
		this.lastMousePosition = Input.mousePosition;
		yield break;
	}

	// Token: 0x06008517 RID: 34071 RVA: 0x00362C40 File Offset: 0x00360E40
	public override void ResetNavigationTarget()
	{
		if (base.navigationTarget != null)
		{
			if (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard && base.navigationTarget.HasCollider)
			{
				this.Position = base.navigationTarget.Center;
				return;
			}
			this.SetNavigationTarget(null);
		}
	}

	// Token: 0x040066FB RID: 26363
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float BaseSpeed = 500f;

	// Token: 0x040066FC RID: 26364
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float SpeedModRange = 2000f;

	// Token: 0x040066FD RID: 26365
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string emptyCursorAtlasName = "UIAtlas";

	// Token: 0x040066FE RID: 26366
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string emptyCursorSprite = "";

	// Token: 0x040066FF RID: 26367
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly UIWidget.Pivot emptyCursorPivot = UIWidget.Pivot.Center;

	// Token: 0x04006700 RID: 26368
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static UIAtlas emptyCursorAtlas;

	// Token: 0x04006701 RID: 26369
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string defaultControllerCursorAtlasName = "UIAtlas";

	// Token: 0x04006702 RID: 26370
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string defaultControllerCursorSprite = "soft_cursor";

	// Token: 0x04006703 RID: 26371
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly UIWidget.Pivot defaultControllerCursorPivot = UIWidget.Pivot.Center;

	// Token: 0x04006704 RID: 26372
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static UIAtlas defaultControllerCursorAtlas;

	// Token: 0x04006705 RID: 26373
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string defaultMouseCursorAtlasName = "UIAtlas";

	// Token: 0x04006706 RID: 26374
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string defaultMouseCursorSprite = "cursor01";

	// Token: 0x04006707 RID: 26375
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly UIWidget.Pivot defaultMouseCursorPivot = UIWidget.Pivot.TopLeft;

	// Token: 0x04006708 RID: 26376
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static UIAtlas defaultMouseCursorAtlas;

	// Token: 0x04006709 RID: 26377
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string mapCursorAtlasName = "UIAtlas";

	// Token: 0x0400670A RID: 26378
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string mapCursorSprite = "map_cursor";

	// Token: 0x0400670B RID: 26379
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly UIWidget.Pivot mapCursorPivot = UIWidget.Pivot.Center;

	// Token: 0x0400670C RID: 26380
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static UIAtlas mapCursorAtlas;

	// Token: 0x0400670D RID: 26381
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float speed;

	// Token: 0x0400670E RID: 26382
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float mouseSpeed;

	// Token: 0x0400670F RID: 26383
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float LastFrameTime;

	// Token: 0x04006710 RID: 26384
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool hidden;

	// Token: 0x04006711 RID: 26385
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool snapped;

	// Token: 0x04006712 RID: 26386
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float OffsetSnapBounds = 0.1f;

	// Token: 0x04006713 RID: 26387
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public UIPanel cursorPanel;

	// Token: 0x04006714 RID: 26388
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector2 lastMousePosition = Vector2.zero;

	// Token: 0x04006715 RID: 26389
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float currentAcceleration;

	// Token: 0x04006716 RID: 26390
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float speedMultiplier = 1f;

	// Token: 0x04006717 RID: 26391
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool movingMouse;

	// Token: 0x04006718 RID: 26392
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public UISprite selectionBox;

	// Token: 0x04006719 RID: 26393
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int selectionBoxMargin = 10;

	// Token: 0x0400671A RID: 26394
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public PlayerInputManager.InputStyle m_lastInputStyle = PlayerInputManager.InputStyle.Count;

	// Token: 0x0400671B RID: 26395
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool m_softcursorEnabled;

	// Token: 0x0400671C RID: 26396
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool cursorModeActive;

	// Token: 0x0400671D RID: 26397
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public CursorControllerAbs.ECursorType currentCursorType = CursorControllerAbs.ECursorType.Default;
}
