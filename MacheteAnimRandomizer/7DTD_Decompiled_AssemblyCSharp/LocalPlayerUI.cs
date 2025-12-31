using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Platform;
using UnityEngine;

// Token: 0x0200057F RID: 1407
public class LocalPlayerUI : MonoBehaviour
{
	// Token: 0x1400003C RID: 60
	// (add) Token: 0x06002D6E RID: 11630 RVA: 0x0012E8A4 File Offset: 0x0012CAA4
	// (remove) Token: 0x06002D6F RID: 11631 RVA: 0x0012E8DC File Offset: 0x0012CADC
	public event Action<EntityPlayerLocal> OnEntityPlayerLocalAssigned;

	// Token: 0x1400003D RID: 61
	// (add) Token: 0x06002D70 RID: 11632 RVA: 0x0012E914 File Offset: 0x0012CB14
	// (remove) Token: 0x06002D71 RID: 11633 RVA: 0x0012E94C File Offset: 0x0012CB4C
	public event Action OnUIShutdown;

	// Token: 0x06002D72 RID: 11634 RVA: 0x0012E981 File Offset: 0x0012CB81
	public static void QueueUIForNewPlayerEntity(LocalPlayerUI _playerUI)
	{
		if (!LocalPlayerUI.playerUIQueueForPendingEntity.Contains(_playerUI))
		{
			LocalPlayerUI.playerUIQueueForPendingEntity.Enqueue(_playerUI);
		}
	}

	// Token: 0x06002D73 RID: 11635 RVA: 0x0012E99B File Offset: 0x0012CB9B
	public static LocalPlayerUI DispatchNewPlayerForUI(EntityPlayerLocal _entityPlayer)
	{
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_entityPlayer);
		uiforPlayer.entityPlayer = _entityPlayer;
		return uiforPlayer;
	}

	// Token: 0x06002D74 RID: 11636 RVA: 0x0012E9AC File Offset: 0x0012CBAC
	public static LocalPlayerUI GetUIForPrimaryPlayer()
	{
		for (int i = 0; i < LocalPlayerUI.playerUIs.Count; i++)
		{
			LocalPlayerUI localPlayerUI = LocalPlayerUI.playerUIs[i];
			if (!localPlayerUI.isPrimaryUI)
			{
				return localPlayerUI;
			}
		}
		return null;
	}

	// Token: 0x06002D75 RID: 11637 RVA: 0x0012E9E8 File Offset: 0x0012CBE8
	public static LocalPlayerUI GetUIForPlayer(EntityPlayerLocal _entityPlayer)
	{
		if (_entityPlayer == null)
		{
			return null;
		}
		LocalPlayerUI localPlayerUI = null;
		LocalPlayerUI localPlayerUI2 = null;
		for (int i = 0; i < LocalPlayerUI.playerUIs.Count; i++)
		{
			LocalPlayerUI localPlayerUI3 = LocalPlayerUI.playerUIs[i];
			if (localPlayerUI3.entityPlayer == _entityPlayer)
			{
				localPlayerUI = localPlayerUI3;
				break;
			}
			if (!localPlayerUI3.isPrimaryUI && localPlayerUI3.entityPlayer == null)
			{
				localPlayerUI2 = localPlayerUI3;
			}
		}
		if (localPlayerUI == null && LocalPlayerUI.playerUIQueueForPendingEntity.Count > 0)
		{
			localPlayerUI = LocalPlayerUI.playerUIQueueForPendingEntity.Dequeue();
			localPlayerUI.mEntityPlayer = _entityPlayer;
		}
		if (localPlayerUI == null)
		{
			localPlayerUI = localPlayerUI2;
		}
		return localPlayerUI;
	}

	// Token: 0x06002D76 RID: 11638 RVA: 0x0012EA84 File Offset: 0x0012CC84
	public static LocalPlayerUI CreateUIForNewLocalPlayer()
	{
		if (!LocalPlayerUI.primaryUI)
		{
			throw new Exception("Can't create UI for new local player, primary UI not set.");
		}
		GameObject gameObject = LocalPlayerUI.mCleanCopy.gameObject;
		Transform transform = LocalPlayerUI.primaryUI.transform;
		gameObject.SetActive(false);
		GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject, LocalPlayerUI.primaryUI.transform.parent, true);
		gameObject2.transform.position = Vector3.back * (float)LocalPlayerUI.localPlayerCount * 4f;
		gameObject2.transform.localRotation = transform.localRotation;
		gameObject2.transform.localScale = transform.localScale;
		gameObject2.gameObject.name = "GUI(Player" + LocalPlayerUI.playerUIs.Count.ToString() + ")";
		MainMenuMono component = gameObject2.GetComponent<MainMenuMono>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
		}
		gameObject2.SetActive(true);
		LocalPlayerUI component2 = gameObject2.GetComponent<LocalPlayerUI>();
		component2.nguiWindowManager.Show(EnumNGUIWindow.Version, false);
		GameManager.Instance.AddWindows(component2.windowManager);
		List<string> windowGroupSubset = null;
		bool async = true;
		GameObject xuiPrefab = XUi.fullPrefab;
		bool flag = LocalPlayerUI.localPlayerCount > 1;
		if (flag)
		{
			windowGroupSubset = new List<string>(new string[]
			{
				"secondaryPlayerJoin",
				"CalloutGroup",
				"popupGroup"
			});
			async = false;
			xuiPrefab = null;
		}
		XUi xui = XUi.Instantiate(component2, xuiPrefab);
		xui.Load(windowGroupSubset, async);
		xui.isMinimal = flag;
		return component2;
	}

	// Token: 0x06002D77 RID: 11639 RVA: 0x0012EBEC File Offset: 0x0012CDEC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void PrepareUIForSplitScreen(Transform _uiTransform)
	{
		UIRect.AnchorPoint[] array = new UIRect.AnchorPoint[4];
		UICamera componentInParent = _uiTransform.GetComponentInParent<UICamera>();
		foreach (UIRect uirect in _uiTransform.GetComponentsInChildren<UIRect>(true))
		{
			if (uirect.isAnchored)
			{
				array[0] = uirect.topAnchor;
				array[1] = uirect.leftAnchor;
				array[2] = uirect.rightAnchor;
				array[3] = uirect.bottomAnchor;
				foreach (UIRect.AnchorPoint anchorPoint in array)
				{
					if (anchorPoint != null && anchorPoint.target != null)
					{
						UIPanel component = anchorPoint.target.gameObject.GetComponent<UIPanel>();
						if (component != null && component.clipping == UIDrawCall.Clipping.None)
						{
							anchorPoint.target = componentInParent.transform;
						}
					}
				}
				uirect.UpdateAnchors();
			}
		}
	}

	// Token: 0x06002D78 RID: 11640 RVA: 0x0012ECC4 File Offset: 0x0012CEC4
	public static bool AnyModalWindowOpen()
	{
		for (int i = 0; i < LocalPlayerUI.playerUIs.Count; i++)
		{
			if (LocalPlayerUI.playerUIs[i].windowManager.IsModalWindowOpen())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002D79 RID: 11641 RVA: 0x0012ED00 File Offset: 0x0012CF00
	public void UpdateChildCameraIndices()
	{
		int num = 0;
		foreach (LocalPlayerCamera localPlayerCamera in base.GetComponentsInChildren<LocalPlayerCamera>())
		{
			localPlayerCamera.uiChildIndex = num++;
			localPlayerCamera.SetCameraDepth();
		}
	}

	// Token: 0x17000486 RID: 1158
	// (get) Token: 0x06002D7A RID: 11642 RVA: 0x0012ED37 File Offset: 0x0012CF37
	public static LocalPlayerUI primaryUI
	{
		get
		{
			return LocalPlayerUI.mPrimaryUI;
		}
	}

	// Token: 0x17000487 RID: 1159
	// (get) Token: 0x06002D7B RID: 11643 RVA: 0x0012ED3E File Offset: 0x0012CF3E
	// (set) Token: 0x06002D7C RID: 11644 RVA: 0x0012ED46 File Offset: 0x0012CF46
	public int playerIndex { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000488 RID: 1160
	// (get) Token: 0x06002D7D RID: 11645 RVA: 0x0012ED4F File Offset: 0x0012CF4F
	public static int localPlayerCount
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return LocalPlayerUI.playerUIs.Count - 1;
		}
	}

	// Token: 0x17000489 RID: 1161
	// (get) Token: 0x06002D7E RID: 11646 RVA: 0x0012ED5D File Offset: 0x0012CF5D
	public PlayerActionsLocal playerInput
	{
		get
		{
			IPlatform nativePlatform = PlatformManager.NativePlatform;
			if (nativePlatform == null)
			{
				return null;
			}
			PlayerInputManager input = nativePlatform.Input;
			if (input == null)
			{
				return null;
			}
			return input.PrimaryPlayer;
		}
	}

	// Token: 0x1700048A RID: 1162
	// (get) Token: 0x06002D7F RID: 11647 RVA: 0x0012ED7A File Offset: 0x0012CF7A
	public ActionSetManager ActionSetManager
	{
		get
		{
			IPlatform nativePlatform = PlatformManager.NativePlatform;
			if (nativePlatform == null)
			{
				return null;
			}
			PlayerInputManager input = nativePlatform.Input;
			if (input == null)
			{
				return null;
			}
			return input.ActionSetManager;
		}
	}

	// Token: 0x1700048B RID: 1163
	// (get) Token: 0x06002D80 RID: 11648 RVA: 0x0012ED97 File Offset: 0x0012CF97
	// (set) Token: 0x06002D81 RID: 11649 RVA: 0x0012ED9F File Offset: 0x0012CF9F
	public EntityPlayerLocal entityPlayer
	{
		get
		{
			return this.mEntityPlayer;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			this.mEntityPlayer = value;
			Action<EntityPlayerLocal> onEntityPlayerLocalAssigned = this.OnEntityPlayerLocalAssigned;
			if (onEntityPlayerLocalAssigned == null)
			{
				return;
			}
			onEntityPlayerLocalAssigned(this.mEntityPlayer);
		}
	}

	// Token: 0x1700048C RID: 1164
	// (get) Token: 0x06002D82 RID: 11650 RVA: 0x0012EDBE File Offset: 0x0012CFBE
	public LocalPlayer localPlayer
	{
		get
		{
			if (this.entityPlayer != null)
			{
				return this.entityPlayer.GetComponent<LocalPlayer>();
			}
			return null;
		}
	}

	// Token: 0x1700048D RID: 1165
	// (get) Token: 0x06002D83 RID: 11651 RVA: 0x0012EDDB File Offset: 0x0012CFDB
	public bool isPrimaryUI
	{
		get
		{
			return this.mIsPrimaryUI;
		}
	}

	// Token: 0x1700048E RID: 1166
	// (get) Token: 0x06002D84 RID: 11652 RVA: 0x0012EDE3 File Offset: 0x0012CFE3
	// (set) Token: 0x06002D85 RID: 11653 RVA: 0x0012EDEB File Offset: 0x0012CFEB
	public bool IsCleanCopy
	{
		get
		{
			return this.mIsCleanCopy;
		}
		set
		{
			this.mIsCleanCopy = value;
		}
	}

	// Token: 0x1700048F RID: 1167
	// (get) Token: 0x06002D86 RID: 11654 RVA: 0x0012EDF4 File Offset: 0x0012CFF4
	public GUIWindowManager windowManager
	{
		get
		{
			if (!(this.mWindowManager != null))
			{
				return this.mWindowManager = base.GetComponent<GUIWindowManager>();
			}
			return this.mWindowManager;
		}
	}

	// Token: 0x17000490 RID: 1168
	// (get) Token: 0x06002D87 RID: 11655 RVA: 0x0012EE28 File Offset: 0x0012D028
	public NGUIWindowManager nguiWindowManager
	{
		get
		{
			if (!(this.mNGUIWindowManager != null))
			{
				return this.mNGUIWindowManager = base.GetComponent<NGUIWindowManager>();
			}
			return this.mNGUIWindowManager;
		}
	}

	// Token: 0x17000491 RID: 1169
	// (get) Token: 0x06002D88 RID: 11656 RVA: 0x0012EE5C File Offset: 0x0012D05C
	public UICamera uiCamera
	{
		get
		{
			if (!(this.mUICamera != null))
			{
				return this.mUICamera = base.GetComponentInParent<UICamera>();
			}
			return this.mUICamera;
		}
	}

	// Token: 0x17000492 RID: 1170
	// (get) Token: 0x06002D89 RID: 11657 RVA: 0x0012EE90 File Offset: 0x0012D090
	public Camera camera
	{
		get
		{
			if (!(this.mCamera != null))
			{
				return this.mCamera = base.GetComponentInParent<Camera>();
			}
			return this.mCamera;
		}
	}

	// Token: 0x17000493 RID: 1171
	// (get) Token: 0x06002D8A RID: 11658 RVA: 0x0012EEC4 File Offset: 0x0012D0C4
	public CursorControllerAbs CursorController
	{
		get
		{
			if (this.mCursorController != null)
			{
				return this.mCursorController;
			}
			UICamera uiCamera = this.uiCamera;
			if (uiCamera == null)
			{
				return null;
			}
			return this.mCursorController = uiCamera.GetComponentInChildren<CursorControllerAbs>();
		}
	}

	// Token: 0x17000494 RID: 1172
	// (get) Token: 0x06002D8B RID: 11659 RVA: 0x0012EF07 File Offset: 0x0012D107
	public XUi xui
	{
		get
		{
			if (this.mXUi == null)
			{
				this.mXUi = base.GetComponentInChildren<XUi>();
			}
			return this.mXUi;
		}
	}

	// Token: 0x06002D8C RID: 11660 RVA: 0x0012EF2C File Offset: 0x0012D12C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		LocalPlayerUI.playerUIs.Add(this);
		this.mIsPrimaryUI = (LocalPlayerUI.playerUIs.Count == 1);
		if (this.mIsPrimaryUI)
		{
			LocalPlayerUI.mPrimaryUI = this;
			base.gameObject.name = "GUI(Menu)";
			LocalPlayerUI.PrepareUIForSplitScreen(this.uiCamera.transform);
			LocalPlayerUI.CreatingCleanCopy = true;
			LocalPlayerUI.mCleanCopy = UnityEngine.Object.Instantiate<LocalPlayerUI>(this, LocalPlayerUI.primaryUI.transform.parent, true);
			LocalPlayerUI.mCleanCopy.name = "GUI(CleanCopy)";
			LocalPlayerUI.mCleanCopy.gameObject.SetActive(false);
			LocalPlayerUI.mCleanCopy.IsCleanCopy = true;
			GameManager.Instance.AddWindows(LocalPlayerUI.mCleanCopy.windowManager);
			LocalPlayerUI.mCleanCopy.nguiWindowManager.ShowAll(false);
			MainMenuMono component = LocalPlayerUI.mCleanCopy.GetComponent<MainMenuMono>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			LocalPlayerUI.playerUIs.Remove(LocalPlayerUI.mCleanCopy);
			LocalPlayerUI.CreatingCleanCopy = false;
			return;
		}
		Camera[] componentsInParent = base.GetComponentsInParent<Camera>();
		for (int i = 0; i < componentsInParent.Length; i++)
		{
			LocalPlayerCamera.AddToCamera(componentsInParent[i], LocalPlayerCamera.CameraType.UI);
		}
	}

	// Token: 0x06002D8D RID: 11661 RVA: 0x0012F048 File Offset: 0x0012D248
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.DispatchLocalPlayersChanged();
		if (this.mIsPrimaryUI && !GameManager.IsDedicatedServer)
		{
			XUiFromXml.ClearData();
			ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi_Common/styles"));
			ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi_Common/controls"));
			ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi_Menu/styles"));
			ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi_Menu/controls"));
			ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi_Menu/windows"));
			ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi_Menu/xui"));
			XUi xui = XUi.Instantiate(this, null);
			xui.Load(null, true);
			xui.isMinimal = false;
			this.SetupMenuSoftCursor();
		}
	}

	// Token: 0x06002D8E RID: 11662 RVA: 0x0012F0E7 File Offset: 0x0012D2E7
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupMenuSoftCursor()
	{
		CursorControllerAbs.AddSoftCursor(this.uiCamera, PlatformManager.NativePlatform.Input.PrimaryPlayer.GUIActions, this.windowManager);
		this.CursorController.RefreshBounds();
	}

	// Token: 0x06002D8F RID: 11663 RVA: 0x0012F11A File Offset: 0x0012D31A
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		LocalPlayerManager.OnLocalPlayersChanged += this.HandleLocalPlayersChanged;
	}

	// Token: 0x06002D90 RID: 11664 RVA: 0x0012F12D File Offset: 0x0012D32D
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		LocalPlayerManager.OnLocalPlayersChanged -= this.HandleLocalPlayersChanged;
	}

	// Token: 0x06002D91 RID: 11665 RVA: 0x0012F140 File Offset: 0x0012D340
	[PublicizedFrom(EAccessModifier.Private)]
	public void DispatchLocalPlayersChanged()
	{
		int num = 0;
		for (int i = 0; i < LocalPlayerUI.playerUIs.Count; i++)
		{
			LocalPlayerUI localPlayerUI = LocalPlayerUI.playerUIs[i];
			if (!localPlayerUI.isPrimaryUI)
			{
				localPlayerUI.playerIndex = num++;
			}
		}
		LocalPlayerManager.LocalPlayersChanged();
	}

	// Token: 0x06002D92 RID: 11666 RVA: 0x0012F188 File Offset: 0x0012D388
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleLocalPlayersChanged()
	{
		if (this.isPrimaryUI)
		{
			AudioListener componentInParent = base.GetComponentInParent<AudioListener>();
			if (componentInParent != null)
			{
				componentInParent.enabled = (!(GameManager.Instance != null) || GameManager.Instance.World == null || !(GameManager.Instance.World.GetPrimaryPlayer() != null));
			}
		}
	}

	// Token: 0x06002D93 RID: 11667 RVA: 0x0012F1E8 File Offset: 0x0012D3E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (this.isPrimaryUI && GameManager.Instance.triggerEffectManager != null)
		{
			bool flag = this.windowManager.IsModalWindowOpen();
			if (!flag)
			{
				using (IEnumerator<LocalPlayerUI> enumerator = LocalPlayerUI.PlayerUIs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.windowManager.IsModalWindowOpen())
						{
							flag = true;
							break;
						}
					}
				}
			}
			GameManager.Instance.triggerEffectManager.inUI = flag;
		}
		if (this.isPrimaryUI && !this.windowManager.IsModalWindowOpen() && this.xui.calloutWindow != null)
		{
			this.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
		}
		if (LocalPlayerUI.primaryUI.windowManager.IsWindowOpen(GUIWindowConsole.ID))
		{
			return;
		}
		if (!this.isPrimaryUI && LocalPlayerUI.primaryUI.windowManager.IsModalWindowOpen())
		{
			return;
		}
		PlayerActionsLocal playerInput = this.playerInput;
		if (playerInput == null)
		{
			return;
		}
		if (playerInput.GUIActions.Inspect.IsPressed)
		{
			return;
		}
		if (this.xui.playerUI.CursorController.Locked)
		{
			return;
		}
		if (this.xui.playerUI.CursorController.lockNavigationToView != null && this.xui.playerUI.CursorController.lockNavigationToView.xui.playerUI != this)
		{
			return;
		}
		if (this.playerInput.GUIActions.BackButton.WasPressed && this.TryItemStackGridNavigation())
		{
			return;
		}
		Vector2 vector = playerInput.GUIActions.Nav.Vector;
		if (vector == Vector2.zero)
		{
			this.inputRepeatTimer = 0f;
			this.previousInputVector = Vector2.zero;
			return;
		}
		if (this.previousInputVector != Vector2.zero)
		{
			this.inputRepeatTimer -= Time.unscaledDeltaTime;
			if (this.inputRepeatTimer > 0f)
			{
				return;
			}
		}
		else
		{
			this.initialRepeat = true;
		}
		XUiView navigationTarget = this.CursorController.navigationTarget;
		this.previousInputVector = vector;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		if (Mathf.Abs(vector.y) > Mathf.Abs(vector.x))
		{
			flag2 = (vector.y > 0f);
			flag3 = (vector.y < 0f);
		}
		else
		{
			flag5 = (vector.x > 0f);
			flag4 = (vector.x < 0f);
		}
		LocalPlayerUI.openWindows.Clear();
		LocalPlayerUI.navViews.Clear();
		if (this.CursorController.lockNavigationToView != null)
		{
			this.CursorController.lockNavigationToView.Controller.FindNavigatableChildren(LocalPlayerUI.navViews);
		}
		else
		{
			this.xui.GetOpenWindows(LocalPlayerUI.openWindows);
			foreach (XUiV_Window xuiV_Window in LocalPlayerUI.openWindows)
			{
				xuiV_Window.Controller.FindNavigatableChildren(LocalPlayerUI.navViews);
			}
		}
		if (LocalPlayerUI.navViews.Count == 0)
		{
			return;
		}
		float num = float.MaxValue;
		XUiView navigationTarget2 = null;
		Vector2 flatPosition = ((SoftCursor)this.CursorController).GetFlatPosition();
		if (navigationTarget != null && !navigationTarget.IsNavigatable)
		{
			this.CursorController.SetNavigationTarget(null);
		}
		bool flag6 = false;
		if (navigationTarget == null)
		{
			foreach (XUiView xuiView in LocalPlayerUI.navViews)
			{
				float sqrMagnitude = (flatPosition - xuiView.GetClosestPoint(flatPosition)).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					navigationTarget2 = xuiView;
				}
			}
			if (num < 3.4028235E+38f)
			{
				this.CursorController.SetNavigationTarget(navigationTarget2);
				flag6 = true;
			}
		}
		if (!flag6)
		{
			if (flag2 && navigationTarget.NavUpTarget != null)
			{
				navigationTarget.NavUpTarget.Controller.SelectCursorElement(false, false);
				flag6 = true;
			}
			else if (flag3 && navigationTarget.NavDownTarget != null)
			{
				navigationTarget.NavDownTarget.Controller.SelectCursorElement(false, false);
				flag6 = true;
			}
			else if (flag4 && navigationTarget.NavLeftTarget != null)
			{
				navigationTarget.NavLeftTarget.Controller.SelectCursorElement(false, false);
				flag6 = true;
			}
			else if (flag5 && navigationTarget.NavRightTarget != null)
			{
				navigationTarget.NavRightTarget.Controller.SelectCursorElement(false, false);
				flag6 = true;
			}
		}
		LocalPlayerUI.navigationCandidates.Clear();
		LocalPlayerUI.navigationPrimeCandidates.Clear();
		LocalPlayerUI.navigationWrapAroundCandidates.Clear();
		LocalPlayerUI.navigationWrapAroundPrimeCandidates.Clear();
		if (!flag6)
		{
			Vector2 vector2;
			if (flag2)
			{
				vector2 = new Vector2(navigationTarget.Center.x, (float)(-(float)Screen.height));
			}
			else if (flag3)
			{
				vector2 = new Vector2(navigationTarget.Center.x, (float)Screen.height);
			}
			else if (flag4)
			{
				vector2 = new Vector2((float)Screen.width, navigationTarget.Center.y);
			}
			else
			{
				vector2 = new Vector2((float)(-(float)Screen.width), navigationTarget.Center.y);
			}
			foreach (XUiView xuiView2 in LocalPlayerUI.navViews)
			{
				if (flag2)
				{
					if (xuiView2.Center.y > navigationTarget.Center.y)
					{
						if (Vector3.Angle(navigationTarget.UiTransform.up, (xuiView2.Center - navigationTarget.Center).normalized) <= 78f)
						{
							if (Mathf.Abs(navigationTarget.Center.x - xuiView2.Center.x) <= Mathf.Max(navigationTarget.widthExtent, xuiView2.widthExtent))
							{
								LocalPlayerUI.navigationPrimeCandidates.Add(xuiView2);
							}
							else
							{
								LocalPlayerUI.navigationCandidates.Add(xuiView2);
							}
						}
					}
					else if (Mathf.Abs(navigationTarget.Center.x - xuiView2.Center.x) <= Mathf.Max(navigationTarget.widthExtent, xuiView2.widthExtent))
					{
						LocalPlayerUI.navigationWrapAroundPrimeCandidates.Add(xuiView2);
					}
					else
					{
						LocalPlayerUI.navigationWrapAroundCandidates.Add(xuiView2);
					}
				}
				else if (flag3)
				{
					if (xuiView2.Center.y < navigationTarget.Center.y)
					{
						if (Vector3.Angle(-navigationTarget.UiTransform.up, (xuiView2.Center - navigationTarget.Center).normalized) <= 78f)
						{
							if (Mathf.Abs(navigationTarget.Center.x - xuiView2.Center.x) <= Mathf.Max(navigationTarget.widthExtent, xuiView2.widthExtent))
							{
								LocalPlayerUI.navigationPrimeCandidates.Add(xuiView2);
							}
							else
							{
								LocalPlayerUI.navigationCandidates.Add(xuiView2);
							}
						}
					}
					else if (Mathf.Abs(navigationTarget.Center.x - xuiView2.Center.x) <= Mathf.Max(navigationTarget.widthExtent, xuiView2.widthExtent))
					{
						LocalPlayerUI.navigationWrapAroundPrimeCandidates.Add(xuiView2);
					}
					else
					{
						LocalPlayerUI.navigationWrapAroundCandidates.Add(xuiView2);
					}
				}
				else if (flag4)
				{
					if (xuiView2.Center.x < navigationTarget.Center.x)
					{
						if (Vector3.Angle(-navigationTarget.UiTransform.right, (xuiView2.Center - navigationTarget.Center).normalized) <= 78f)
						{
							if (Mathf.Abs(navigationTarget.Center.y - xuiView2.Center.y) <= Mathf.Max(navigationTarget.heightExtent, xuiView2.heightExtent))
							{
								LocalPlayerUI.navigationPrimeCandidates.Add(xuiView2);
							}
							else
							{
								LocalPlayerUI.navigationCandidates.Add(xuiView2);
							}
						}
					}
					else if (Mathf.Abs(navigationTarget.Center.y - xuiView2.Center.y) <= Mathf.Max(navigationTarget.heightExtent, xuiView2.heightExtent))
					{
						LocalPlayerUI.navigationWrapAroundPrimeCandidates.Add(xuiView2);
					}
					else
					{
						LocalPlayerUI.navigationWrapAroundCandidates.Add(xuiView2);
					}
				}
				else if (flag5)
				{
					if (xuiView2.Center.x > navigationTarget.Center.x)
					{
						if (Vector3.Angle(navigationTarget.UiTransform.right, (xuiView2.Center - navigationTarget.Center).normalized) <= 78f)
						{
							if (Mathf.Abs(navigationTarget.Center.y - xuiView2.Center.y) <= Mathf.Max(navigationTarget.heightExtent, xuiView2.heightExtent))
							{
								LocalPlayerUI.navigationPrimeCandidates.Add(xuiView2);
							}
							else
							{
								LocalPlayerUI.navigationCandidates.Add(xuiView2);
							}
						}
					}
					else if (Mathf.Abs(navigationTarget.Center.y - xuiView2.Center.y) <= Mathf.Max(navigationTarget.heightExtent, xuiView2.heightExtent))
					{
						LocalPlayerUI.navigationWrapAroundPrimeCandidates.Add(xuiView2);
					}
					else
					{
						LocalPlayerUI.navigationWrapAroundCandidates.Add(xuiView2);
					}
				}
			}
			if (!flag6 && LocalPlayerUI.navigationPrimeCandidates.Count > 0)
			{
				using (List<XUiView>.Enumerator enumerator3 = LocalPlayerUI.navigationPrimeCandidates.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						XUiView xuiView3 = enumerator3.Current;
						float num2;
						if (flag4 || flag5)
						{
							num2 = Mathf.Abs(navigationTarget.Center.x - xuiView3.Center.x);
						}
						else
						{
							num2 = Mathf.Abs(navigationTarget.Center.y - xuiView3.Center.y);
						}
						if (num2 < num)
						{
							num = num2;
							navigationTarget2 = xuiView3;
						}
					}
					goto IL_B4B;
				}
			}
			if (!flag6 && LocalPlayerUI.navigationCandidates.Count > 0)
			{
				using (List<XUiView>.Enumerator enumerator3 = LocalPlayerUI.navigationCandidates.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						XUiView xuiView4 = enumerator3.Current;
						float sqrMagnitude2 = (navigationTarget.GetClosestPoint(xuiView4.Center) - xuiView4.GetClosestPoint(navigationTarget.Center)).sqrMagnitude;
						if (sqrMagnitude2 < num)
						{
							num = sqrMagnitude2;
							navigationTarget2 = xuiView4;
						}
					}
					goto IL_B4B;
				}
			}
			if (!flag6 && LocalPlayerUI.navigationWrapAroundPrimeCandidates.Count > 0)
			{
				num = float.MaxValue;
				using (List<XUiView>.Enumerator enumerator3 = LocalPlayerUI.navigationWrapAroundPrimeCandidates.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						XUiView xuiView5 = enumerator3.Current;
						float num3;
						if (flag4 || flag5)
						{
							num3 = Mathf.Abs(vector2.x - xuiView5.Center.x);
						}
						else
						{
							num3 = Mathf.Abs(vector2.y - xuiView5.Center.y);
						}
						if (num3 < num)
						{
							num = num3;
							navigationTarget2 = xuiView5;
						}
					}
					goto IL_B4B;
				}
			}
			if (!flag6 && LocalPlayerUI.navigationWrapAroundCandidates.Count > 0)
			{
				num = float.MaxValue;
				foreach (XUiView xuiView6 in LocalPlayerUI.navigationWrapAroundCandidates)
				{
					float sqrMagnitude3 = (vector2 - xuiView6.GetClosestPoint(vector2)).sqrMagnitude;
					if (sqrMagnitude3 < num)
					{
						num = sqrMagnitude3;
						navigationTarget2 = xuiView6;
					}
				}
			}
		}
		IL_B4B:
		if (!flag6 && num < 3.4028235E+38f)
		{
			this.CursorController.SetNavigationTarget(navigationTarget2);
		}
		this.inputRepeatTimer = (this.initialRepeat ? 0.35f : 0.1f);
		this.initialRepeat = false;
	}

	// Token: 0x06002D94 RID: 11668 RVA: 0x0012FE40 File Offset: 0x0012E040
	public void RefreshNavigationTarget()
	{
		LocalPlayerUI.openWindows.Clear();
		LocalPlayerUI.navViews.Clear();
		if (this.CursorController.lockNavigationToView != null)
		{
			this.CursorController.lockNavigationToView.Controller.FindNavigatableChildren(LocalPlayerUI.navViews);
		}
		else
		{
			this.xui.GetOpenWindows(LocalPlayerUI.openWindows);
			foreach (XUiV_Window xuiV_Window in LocalPlayerUI.openWindows)
			{
				xuiV_Window.Controller.FindNavigatableChildren(LocalPlayerUI.navViews);
			}
		}
		if (LocalPlayerUI.navViews.Count == 0)
		{
			return;
		}
		XUiView navigationTarget = this.CursorController.navigationTarget;
		float num = float.MaxValue;
		XUiView navigationTarget2 = null;
		Vector2 flatPosition = ((SoftCursor)this.CursorController).GetFlatPosition();
		foreach (XUiView xuiView in LocalPlayerUI.navViews)
		{
			float sqrMagnitude = (flatPosition - xuiView.GetClosestPoint(flatPosition)).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				navigationTarget2 = xuiView;
			}
		}
		if (num < 3.4028235E+38f)
		{
			this.CursorController.SetNavigationTarget(navigationTarget2);
		}
	}

	// Token: 0x06002D95 RID: 11669 RVA: 0x0012FF94 File Offset: 0x0012E194
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		Action onUIShutdown = this.OnUIShutdown;
		if (onUIShutdown != null)
		{
			onUIShutdown();
		}
		LocalPlayerUI.playerUIs.Remove(this);
		LocalPlayerUI.navViews.Clear();
		LocalPlayerUI.openWindows.Clear();
		this.DispatchLocalPlayersChanged();
	}

	// Token: 0x06002D96 RID: 11670 RVA: 0x0012FFCD File Offset: 0x0012E1CD
	public void RegisterItemStackGrid(XUiC_ItemStackGrid _grid)
	{
		if (this.activeItemStackGrids.Contains(_grid))
		{
			return;
		}
		this.activeItemStackGrids.Add(_grid);
		this.SortItemStackGrids();
	}

	// Token: 0x06002D97 RID: 11671 RVA: 0x0012FFF0 File Offset: 0x0012E1F0
	public void UnregisterItemStackGrid(XUiC_ItemStackGrid _grid)
	{
		this.activeItemStackGrids.Remove(_grid);
		this.SortItemStackGrids();
	}

	// Token: 0x06002D98 RID: 11672 RVA: 0x00130005 File Offset: 0x0012E205
	[PublicizedFrom(EAccessModifier.Private)]
	public void SortItemStackGrids()
	{
		this.activeItemStackGrids.Sort(delegate(XUiC_ItemStackGrid x, XUiC_ItemStackGrid y)
		{
			if (x.ViewComponent.Center.y + x.ViewComponent.heightExtent <= y.ViewComponent.Center.y + y.ViewComponent.heightExtent)
			{
				return -1;
			}
			return 1;
		});
	}

	// Token: 0x06002D99 RID: 11673 RVA: 0x00130034 File Offset: 0x0012E234
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryItemStackGridNavigation()
	{
		if (this.activeItemStackGrids.Count == 0)
		{
			return false;
		}
		XUiC_ItemStackGrid xuiC_ItemStackGrid = null;
		if (this.CursorController.navigationTarget != null)
		{
			foreach (XUiC_ItemStackGrid xuiC_ItemStackGrid2 in this.activeItemStackGrids)
			{
				if (this.CursorController.navigationTarget.Controller.IsChildOf(xuiC_ItemStackGrid2))
				{
					xuiC_ItemStackGrid = xuiC_ItemStackGrid2;
					break;
				}
			}
		}
		int num = 0;
		if (xuiC_ItemStackGrid != null)
		{
			num = this.activeItemStackGrids.IndexOf(xuiC_ItemStackGrid);
		}
		int num2 = num;
		if (this.activeItemStackGrids.Count == 1)
		{
			this.activeItemStackGrids[num].SelectCursorElement(true, false);
			return true;
		}
		num++;
		if (num >= this.activeItemStackGrids.Count)
		{
			num = 0;
		}
		while (num != num2)
		{
			XUiView xuiView;
			if (this.activeItemStackGrids[num].TryFindFirstNavigableChild(out xuiView))
			{
				if (!this.xui.dragAndDrop.CurrentStack.Equals(ItemStack.Empty))
				{
					int num3 = this.activeItemStackGrids[num].FindFirstEmptySlot();
					if (num3 >= 0)
					{
						this.activeItemStackGrids[num].GetItemStackControllers()[num3].SelectCursorElement(true, true);
						return true;
					}
				}
				this.activeItemStackGrids[num].SelectCursorElement(true, true);
				return true;
			}
			num++;
			if (num >= this.activeItemStackGrids.Count)
			{
				num = 0;
			}
		}
		return false;
	}

	// Token: 0x040023FA RID: 9210
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static LocalPlayerUI mPrimaryUI;

	// Token: 0x040023FB RID: 9211
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static LocalPlayerUI mCleanCopy;

	// Token: 0x040023FD RID: 9213
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityPlayerLocal mEntityPlayer;

	// Token: 0x040023FE RID: 9214
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool mIsPrimaryUI;

	// Token: 0x040023FF RID: 9215
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool mIsCleanCopy;

	// Token: 0x04002400 RID: 9216
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GUIWindowManager mWindowManager;

	// Token: 0x04002401 RID: 9217
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public NGUIWindowManager mNGUIWindowManager;

	// Token: 0x04002402 RID: 9218
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public UICamera mUICamera;

	// Token: 0x04002403 RID: 9219
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Camera mCamera;

	// Token: 0x04002404 RID: 9220
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CursorControllerAbs mCursorController;

	// Token: 0x04002405 RID: 9221
	public static bool IsAnyComboBoxFocused = false;

	// Token: 0x04002406 RID: 9222
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public XUi mXUi;

	// Token: 0x04002407 RID: 9223
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly List<LocalPlayerUI> playerUIs = new List<LocalPlayerUI>();

	// Token: 0x04002408 RID: 9224
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly Queue<LocalPlayerUI> playerUIQueueForPendingEntity = new Queue<LocalPlayerUI>();

	// Token: 0x04002409 RID: 9225
	public static readonly ReadOnlyCollection<LocalPlayerUI> PlayerUIs = new ReadOnlyCollection<LocalPlayerUI>(LocalPlayerUI.playerUIs);

	// Token: 0x0400240A RID: 9226
	public static bool CreatingCleanCopy;

	// Token: 0x0400240B RID: 9227
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<XUiView> navigationCandidates = new List<XUiView>();

	// Token: 0x0400240C RID: 9228
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<XUiView> navigationPrimeCandidates = new List<XUiView>();

	// Token: 0x0400240D RID: 9229
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<XUiView> navigationWrapAroundCandidates = new List<XUiView>();

	// Token: 0x0400240E RID: 9230
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<XUiView> navigationWrapAroundPrimeCandidates = new List<XUiView>();

	// Token: 0x0400240F RID: 9231
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<XUiV_Window> openWindows = new List<XUiV_Window>();

	// Token: 0x04002410 RID: 9232
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<XUiView> navViews = new List<XUiView>();

	// Token: 0x04002411 RID: 9233
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float navigationAngleLimit = 78f;

	// Token: 0x04002412 RID: 9234
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 previousInputVector = Vector2.zero;

	// Token: 0x04002413 RID: 9235
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float inputRepeatTimer;

	// Token: 0x04002414 RID: 9236
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool initialRepeat;

	// Token: 0x04002415 RID: 9237
	public List<XUiC_ItemStackGrid> activeItemStackGrids = new List<XUiC_ItemStackGrid>();
}
