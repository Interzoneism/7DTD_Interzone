using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using InControl;
using Platform;
using UnityEngine;

// Token: 0x02001055 RID: 4181
public class PlayerMoveController : MonoBehaviour
{
	// Token: 0x17000DC6 RID: 3526
	// (get) Token: 0x06008413 RID: 33811 RVA: 0x00356EE4 File Offset: 0x003550E4
	public bool RunToggleActive
	{
		get
		{
			return this.runToggleActive;
		}
	}

	// Token: 0x17000DC7 RID: 3527
	// (get) Token: 0x06008414 RID: 33812 RVA: 0x000DDD71 File Offset: 0x000DBF71
	public PlayerActionsLocal playerInput
	{
		get
		{
			return PlatformManager.NativePlatform.Input.PrimaryPlayer;
		}
	}

	// Token: 0x06008415 RID: 33813 RVA: 0x00356EEC File Offset: 0x003550EC
	public void Init()
	{
		PlayerMoveController.Instance = this;
		this.entityPlayerLocal = base.GetComponent<EntityPlayerLocal>();
		this.playerUI = LocalPlayerUI.GetUIForPlayer(this.entityPlayerLocal);
		this.windowManager = this.playerUI.windowManager;
		this.nguiWindowManager = this.playerUI.nguiWindowManager;
		this.gameManager = (GameManager)UnityEngine.Object.FindObjectOfType(typeof(GameManager));
		this.guiInGame = this.nguiWindowManager.InGameHUD;
		this.nguiWindowManager.Show(EnumNGUIWindow.InGameHUD, true);
		PlayerMoveController.UpdateControlsOptions();
		GamePrefs.Set(EnumGamePrefs.DebugMenuShowTasks, false);
		this.focusBoxScript = new RenderDisplacedCube((this.guiInGame.FocusCube != null) ? UnityEngine.Object.Instantiate<Transform>(this.guiInGame.FocusCube) : null);
		this.playerAutoPilotControllor = new PlayerAutoPilotControllor(this.gameManager);
		this.toggleGodMode = delegate()
		{
			this.entityPlayerLocal.bEntityAliveFlagsChanged = true;
			this.entityPlayerLocal.IsGodMode.Value = !this.entityPlayerLocal.IsGodMode.Value;
			this.entityPlayerLocal.IsNoCollisionMode.Value = this.entityPlayerLocal.IsGodMode.Value;
			this.entityPlayerLocal.IsFlyMode.Value = this.entityPlayerLocal.IsGodMode.Value;
			if (this.entityPlayerLocal.IsGodMode.Value)
			{
				this.entityPlayerLocal.Buffs.AddBuff("god", -1, true, false, -1f);
				return;
			}
			if (!GameManager.Instance.World.IsEditor() && !GameModeCreative.TypeName.Equals(GamePrefs.GetString(EnumGamePrefs.GameMode)))
			{
				this.entityPlayerLocal.Buffs.RemoveBuff("god", true);
			}
		};
		this.teleportPlayer = delegate()
		{
			Ray lookRay = this.entityPlayerLocal.GetLookRay();
			if (InputUtils.ControlKeyPressed)
			{
				lookRay.direction *= -1f;
			}
			lookRay.origin -= Origin.position;
			RaycastHit raycastHit;
			Vector3 a;
			if (Physics.SphereCast(lookRay, 0.3f, out raycastHit, 500f, 1342242816))
			{
				a = raycastHit.point - lookRay.direction.normalized * 0.5f;
			}
			else
			{
				a = lookRay.origin + lookRay.direction.normalized * 100f;
			}
			this.entityPlayerLocal.SetPosition(a + Origin.position, true);
			GameEventManager.Current.HandleForceBossDespawn(this.entityPlayerLocal);
		};
		NGuiAction nguiAction = new NGuiAction("SelectionMode", null, null, true, this.playerInput.Drop);
		nguiAction.SetClickActionDelegate(delegate
		{
			if (InputUtils.AltKeyPressed)
			{
				GamePrefs.Set(EnumGamePrefs.SelectionOperationMode, 4);
				return;
			}
			if (InputUtils.ShiftKeyPressed)
			{
				GamePrefs.Set(EnumGamePrefs.SelectionOperationMode, 2);
				GameManager.Instance.SetCursorEnabledOverride(true, true);
				return;
			}
			if (InputUtils.ControlKeyPressed && GameManager.Instance.IsEditMode())
			{
				GamePrefs.Set(EnumGamePrefs.SelectionOperationMode, 3);
				GameManager.Instance.SetCursorEnabledOverride(true, true);
				return;
			}
			GamePrefs.Set(EnumGamePrefs.SelectionOperationMode, 1);
			GameManager.Instance.SetCursorEnabledOverride(true, true);
		});
		nguiAction.SetReleaseActionDelegate(delegate
		{
			GamePrefs.Set(EnumGamePrefs.SelectionOperationMode, 0);
			GameManager.Instance.SetCursorEnabledOverride(false, false);
		});
		nguiAction.SetIsEnabledDelegate(() => (this.gameManager.gameStateManager.IsGameStarted() && GameStats.GetInt(EnumGameStats.GameState) == 1 && GameManager.Instance.World.IsEditor()) || BlockToolSelection.Instance.SelectionActive);
		this.globalActions.Add(nguiAction);
		NGuiAction.IsEnabledDelegate menuIsEnabled = () => !XUiC_SpawnSelectionWindow.IsOpenInUI(LocalPlayerUI.primaryUI) && this.gameManager.gameStateManager.IsGameStarted() && GameStats.GetInt(EnumGameStats.GameState) == 1 && !LocalPlayerUI.primaryUI.windowManager.IsModalWindowOpen() && !this.windowManager.IsFullHUDDisabled();
		NGuiAction.OnClickActionDelegate clickActionDelegate = delegate()
		{
			this.entityPlayerLocal.AimingGun = false;
			if (this.windowManager.IsWindowOpen("windowpaging") || this.windowManager.IsModalWindowOpen())
			{
				this.windowManager.CloseAllOpenWindows(null, false);
				this.windowManager.CloseIfOpen("windowpaging");
				return;
			}
			this.windowManager.CloseAllOpenWindows(null, false);
			this.playerUI.xui.RadialWindow.Open();
			this.playerUI.xui.RadialWindow.SetupMenuData();
		};
		NGuiAction.IsCheckedDelegate isCheckedDelegate = () => this.windowManager.IsWindowOpen("windowpaging");
		NGuiAction.IsEnabledDelegate isEnabledDelegate = () => menuIsEnabled() && !this.windowManager.IsWindowOpen("radial");
		NGuiAction nguiAction2 = new NGuiAction("Inventory", null, null, true, this.playerInput.Inventory);
		nguiAction2.SetClickActionDelegate(clickActionDelegate);
		nguiAction2.SetIsEnabledDelegate(isEnabledDelegate);
		nguiAction2.SetIsCheckedDelegate(isCheckedDelegate);
		this.globalActions.Add(nguiAction2);
		NGuiAction nguiAction3 = new NGuiAction("Inventory", null, null, true, this.playerInput.PermanentActions.Inventory);
		nguiAction3.SetClickActionDelegate(clickActionDelegate);
		nguiAction3.SetIsEnabledDelegate(isEnabledDelegate);
		nguiAction3.SetIsCheckedDelegate(isCheckedDelegate);
		this.globalActions.Add(nguiAction3);
		NGuiAction nguiAction4 = new NGuiAction("Inventory", null, null, true, this.playerInput.VehicleActions.Inventory);
		nguiAction4.SetClickActionDelegate(clickActionDelegate);
		nguiAction4.SetIsEnabledDelegate(isEnabledDelegate);
		nguiAction4.SetIsCheckedDelegate(isCheckedDelegate);
		this.globalActions.Add(nguiAction4);
		NGuiAction nguiAction5 = new NGuiAction("Creative", null, null, true, this.playerInput.PermanentActions.Creative);
		nguiAction5.SetClickActionDelegate(delegate
		{
			this.entityPlayerLocal.AimingGun = false;
			XUiC_WindowSelector.OpenSelectorAndWindow(this.entityPlayerLocal, "creative");
		});
		nguiAction5.SetIsEnabledDelegate(() => menuIsEnabled() && (GameStats.GetBool(EnumGameStats.IsCreativeMenuEnabled) || GamePrefs.GetBool(EnumGamePrefs.CreativeMenuEnabled)));
		nguiAction5.SetIsCheckedDelegate(() => this.windowManager.IsWindowOpen("creative"));
		this.globalActions.Add(nguiAction5);
		NGuiAction nguiAction6 = new NGuiAction("Map", null, null, true, this.playerInput.PermanentActions.Map);
		nguiAction6.SetClickActionDelegate(delegate
		{
			this.entityPlayerLocal.AimingGun = false;
			XUiC_WindowSelector.OpenSelectorAndWindow(this.entityPlayerLocal, "map");
		});
		nguiAction6.SetIsEnabledDelegate(menuIsEnabled);
		nguiAction6.SetIsCheckedDelegate(() => this.windowManager.IsWindowOpen("map"));
		this.globalActions.Add(nguiAction6);
		NGuiAction nguiAction7 = new NGuiAction("Character", null, null, true, this.playerInput.PermanentActions.Character);
		nguiAction7.SetClickActionDelegate(delegate
		{
			this.entityPlayerLocal.AimingGun = false;
			XUiC_WindowSelector.OpenSelectorAndWindow(this.entityPlayerLocal, "character");
		});
		nguiAction7.SetIsEnabledDelegate(menuIsEnabled);
		this.globalActions.Add(nguiAction7);
		NGuiAction nguiAction8 = new NGuiAction("Skills", null, null, true, this.playerInput.PermanentActions.Skills);
		nguiAction8.SetClickActionDelegate(delegate
		{
			this.entityPlayerLocal.AimingGun = false;
			XUiC_WindowSelector.OpenSelectorAndWindow(this.entityPlayerLocal, "skills");
		});
		nguiAction8.SetIsEnabledDelegate(menuIsEnabled);
		this.globalActions.Add(nguiAction8);
		NGuiAction nguiAction9 = new NGuiAction("Quests", null, null, true, this.playerInput.PermanentActions.Quests);
		nguiAction9.SetClickActionDelegate(delegate
		{
			this.entityPlayerLocal.AimingGun = false;
			XUiC_WindowSelector.OpenSelectorAndWindow(this.entityPlayerLocal, "quests");
		});
		nguiAction9.SetIsEnabledDelegate(menuIsEnabled);
		this.globalActions.Add(nguiAction9);
		NGuiAction.OnClickActionDelegate clickActionDelegate2 = delegate()
		{
			this.entityPlayerLocal.AimingGun = false;
			XUiC_WindowSelector.OpenSelectorAndWindow(this.entityPlayerLocal, "players");
		};
		NGuiAction nguiAction10 = new NGuiAction("Players", null, null, true, this.playerInput.Scoreboard);
		nguiAction10.SetClickActionDelegate(clickActionDelegate2);
		nguiAction10.SetIsEnabledDelegate(menuIsEnabled);
		this.globalActions.Add(nguiAction10);
		NGuiAction nguiAction11 = new NGuiAction("Players", null, null, true, this.playerInput.VehicleActions.Scoreboard);
		nguiAction11.SetClickActionDelegate(clickActionDelegate2);
		nguiAction11.SetIsEnabledDelegate(menuIsEnabled);
		this.globalActions.Add(nguiAction11);
		NGuiAction nguiAction12 = new NGuiAction("Players", null, null, true, this.playerInput.PermanentActions.Scoreboard);
		nguiAction12.SetClickActionDelegate(clickActionDelegate2);
		nguiAction12.SetIsEnabledDelegate(menuIsEnabled);
		this.globalActions.Add(nguiAction12);
		NGuiAction nguiAction13 = new NGuiAction("Challenges", null, null, true, this.playerInput.PermanentActions.Challenges);
		nguiAction13.SetClickActionDelegate(delegate
		{
			this.entityPlayerLocal.AimingGun = false;
			XUiC_WindowSelector.OpenSelectorAndWindow(this.entityPlayerLocal, "challenges");
		});
		nguiAction13.SetIsEnabledDelegate(menuIsEnabled);
		this.globalActions.Add(nguiAction13);
		NGuiAction nguiAction14 = new NGuiAction("Chat", null, null, true, this.playerInput.PermanentActions.Chat);
		nguiAction14.SetClickActionDelegate(delegate
		{
			this.entityPlayerLocal.AimingGun = false;
			this.windowManager.Open(XUiC_Chat.ID, true, false, true);
		});
		nguiAction14.SetIsEnabledDelegate(menuIsEnabled);
		nguiAction14.SetIsCheckedDelegate(() => this.windowManager.IsWindowOpen(XUiC_Chat.ID));
		this.globalActions.Add(nguiAction14);
		NGuiAction nguiAction15 = new NGuiAction("Prefab", null, null, true, this.playerInput.Prefab);
		nguiAction15.SetClickActionDelegate(delegate
		{
			this.entityPlayerLocal.AimingGun = false;
			Manager.PlayButtonClick();
			this.windowManager.SwitchVisible(GUIWindowWOChooseCategory.ID, false, true);
		});
		nguiAction15.SetIsEnabledDelegate(() => menuIsEnabled() && this.gameManager.IsEditMode());
		nguiAction15.SetIsCheckedDelegate(() => this.windowManager.IsWindowOpen(GUIWindowWOChooseCategory.ID));
		this.globalActions.Add(nguiAction15);
		NGuiAction nguiAction16 = new NGuiAction("DetachCamera", null, null, false, this.playerInput.DetachCamera);
		nguiAction16.SetClickActionDelegate(delegate
		{
			Manager.PlayButtonClick();
			this.entityPlayerLocal.SetCameraAttachedToPlayer(!this.entityPlayerLocal.IsCameraAttachedToPlayerOrScope(), true);
		});
		nguiAction16.SetIsEnabledDelegate(() => this.gameManager.gameStateManager.IsGameStarted() && GameStats.GetInt(EnumGameStats.GameState) == 1 && !this.entityPlayerLocal.AimingGun && (this.gameManager.IsEditMode() || GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled)) && !InputUtils.ControlKeyPressed);
		this.globalActions.Add(nguiAction16);
		NGuiAction nguiAction17 = new NGuiAction("ToggleDCMove", null, null, false, this.playerInput.ToggleDCMove);
		nguiAction17.SetClickActionDelegate(delegate
		{
			Manager.PlayButtonClick();
			this.entityPlayerLocal.movementInput.bDetachedCameraMove = (!this.entityPlayerLocal.movementInput.bDetachedCameraMove && !this.entityPlayerLocal.IsCameraAttachedToPlayerOrScope());
		});
		nguiAction17.SetIsEnabledDelegate(() => this.gameManager.gameStateManager.IsGameStarted() && GameStats.GetInt(EnumGameStats.GameState) == 1 && !this.entityPlayerLocal.AimingGun && (this.gameManager.IsEditMode() || GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled)));
		this.globalActions.Add(nguiAction17);
		NGuiAction nguiAction18 = new NGuiAction("LockCamera", null, null, false, this.playerInput.LockFreeCamera);
		nguiAction18.SetClickActionDelegate(delegate
		{
			Manager.PlayButtonClick();
			this.entityPlayerLocal.movementInput.bCameraPositionLocked = !this.entityPlayerLocal.movementInput.bCameraPositionLocked;
		});
		nguiAction18.SetIsEnabledDelegate(() => this.gameManager.gameStateManager.IsGameStarted() && GameStats.GetInt(EnumGameStats.GameState) == 1 && !this.entityPlayerLocal.AimingGun && (this.gameManager.IsEditMode() || GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled)));
		this.globalActions.Add(nguiAction18);
		NGuiAction.OnClickActionDelegate clickActionDelegate3 = delegate()
		{
			if (!XUiC_SpawnSelectionWindow.IsOpenInUI(LocalPlayerUI.primaryUI))
			{
				Manager.PlayButtonClick();
				if (!this.windowManager.CloseAllOpenWindows(null, false))
				{
					this.entityPlayerLocal.PlayerUI.CursorController.HoverTarget = null;
					this.windowManager.SwitchVisible(XUiC_InGameMenuWindow.ID, false, true);
				}
			}
		};
		NGuiAction nguiAction19 = new NGuiAction("Menu", null, null, false, this.playerInput.Menu);
		nguiAction19.SetClickActionDelegate(clickActionDelegate3);
		nguiAction19.SetIsEnabledDelegate(() => !this.windowManager.IsFullHUDDisabled());
		this.globalActions.Add(nguiAction19);
		NGuiAction nguiAction20 = new NGuiAction("Menu", null, null, false, this.playerInput.VehicleActions.Menu);
		nguiAction20.SetClickActionDelegate(clickActionDelegate3);
		nguiAction20.SetIsEnabledDelegate(() => !this.windowManager.IsFullHUDDisabled());
		this.globalActions.Add(nguiAction20);
		NGuiAction nguiAction21 = new NGuiAction("Fly Mode", null, null, true, this.playerInput.Fly);
		nguiAction21.SetClickActionDelegate(delegate
		{
			Manager.PlayButtonClick();
			this.entityPlayerLocal.IsFlyMode.Value = !this.entityPlayerLocal.IsFlyMode.Value;
		});
		nguiAction21.SetIsCheckedDelegate(() => this.entityPlayerLocal != null && this.entityPlayerLocal.IsFlyMode.Value);
		nguiAction21.SetIsEnabledDelegate(() => GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled) || GamePrefs.GetBool(EnumGamePrefs.CreativeMenuEnabled) || GameStats.GetBool(EnumGameStats.IsFlyingEnabled));
		this.globalActions.Add(nguiAction21);
		NGuiAction nguiAction22 = new NGuiAction("God Mode", null, null, true, this.playerInput.God);
		nguiAction22.SetClickActionDelegate(delegate
		{
			Manager.PlayButtonClick();
			if (InputUtils.ShiftKeyPressed)
			{
				this.teleportPlayer();
				this.isAutorunInvalid = true;
				return;
			}
			this.toggleGodMode();
		});
		nguiAction22.SetIsCheckedDelegate(() => this.entityPlayerLocal != null && this.entityPlayerLocal.IsGodMode.Value);
		nguiAction22.SetIsEnabledDelegate(() => GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled) || !GameStats.GetBool(EnumGameStats.IsPlayerDamageEnabled));
		this.globalActions.Add(nguiAction22);
		NGuiAction nguiAction23 = new NGuiAction("No Collision", null, null, true, null);
		nguiAction23.SetClickActionDelegate(delegate
		{
			Manager.PlayButtonClick();
			this.entityPlayerLocal.IsNoCollisionMode.Value = !this.entityPlayerLocal.IsNoCollisionMode.Value;
		});
		nguiAction23.SetIsCheckedDelegate(() => this.entityPlayerLocal != null && this.entityPlayerLocal.IsNoCollisionMode.Value);
		nguiAction23.SetIsEnabledDelegate(() => GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled) || !GameStats.GetBool(EnumGameStats.IsPlayerCollisionEnabled));
		this.globalActions.Add(nguiAction23);
		NGuiAction nguiAction24 = new NGuiAction("Invisible", null, null, true, this.playerInput.Invisible);
		nguiAction24.SetClickActionDelegate(delegate
		{
			Manager.PlayButtonClick();
			this.entityPlayerLocal.IsSpectator = !this.entityPlayerLocal.IsSpectator;
		});
		nguiAction24.SetIsCheckedDelegate(() => this.entityPlayerLocal != null && this.entityPlayerLocal.IsSpectator);
		nguiAction24.SetIsEnabledDelegate(() => GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled) || !GameStats.GetBool(EnumGameStats.IsPlayerDamageEnabled));
		this.globalActions.Add(nguiAction24);
		for (int i = 0; i < this.globalActions.Count; i++)
		{
			this.windowManager.AddGlobalAction(this.globalActions[i]);
		}
		EAIManager.isAnimFreeze = false;
	}

	// Token: 0x06008416 RID: 33814 RVA: 0x003578B4 File Offset: 0x00355AB4
	public static void UpdateControlsOptions()
	{
		if (PlayerMoveController.Instance != null)
		{
			PlayerMoveController.Instance.invertMouse = (GamePrefs.GetBool(EnumGamePrefs.OptionsInvertMouse) ? -1 : 1);
			PlayerMoveController.Instance.invertController = (GamePrefs.GetBool(EnumGamePrefs.OptionsControllerLookInvert) ? -1 : 1);
			PlayerMoveController.Instance.bControllerVibration = GamePrefs.GetBool(EnumGamePrefs.OptionsControllerVibration);
			PlayerMoveController.Instance.UpdateLookSensitivity(GamePrefs.GetFloat(EnumGamePrefs.OptionsLookSensitivity), GamePrefs.GetFloat(EnumGamePrefs.OptionsZoomSensitivity), GamePrefs.GetFloat(EnumGamePrefs.OptionsZoomAccel), GamePrefs.GetFloat(EnumGamePrefs.OptionsVehicleLookSensitivity), new Vector2(GamePrefs.GetFloat(EnumGamePrefs.OptionsControllerSensitivityX), GamePrefs.GetFloat(EnumGamePrefs.OptionsControllerSensitivityY)));
			PlayerMoveController.Instance.lookAccelerationRate = GamePrefs.GetFloat(EnumGamePrefs.OptionsControllerLookAcceleration) * 0.5f;
			PlayerMoveController.Instance.controllerZoomSensitivity = GamePrefs.GetFloat(EnumGamePrefs.OptionsControllerZoomSensitivity);
			PlayerMoveController.Instance.controllerVehicleSensitivity = GamePrefs.GetFloat(EnumGamePrefs.OptionsControllerVehicleSensitivity);
			PlayerMoveController.Instance.controllerAimAssistsEnabled = GamePrefs.GetBool(EnumGamePrefs.OptionsControllerAimAssists);
			PlayerMoveController.Instance.playerInput.SetJoyStickLayout((eControllerJoystickLayout)GamePrefs.GetInt(EnumGamePrefs.OptionsControllerJoystickLayout));
			PlayerMoveController.Instance.sprintMode = GamePrefs.GetInt(EnumGamePrefs.OptionsControlsSprintLock);
			PlayerMoveController.SetDeadzones();
		}
	}

	// Token: 0x06008417 RID: 33815 RVA: 0x003579E3 File Offset: 0x00355BE3
	public static void SetDeadzones()
	{
		PlayerMoveController.Instance.playerInput.SetDeadzones(GamePrefs.GetFloat(EnumGamePrefs.OptionsControllerLookAxisDeadzone), GamePrefs.GetFloat(EnumGamePrefs.OptionsControllerMoveAxisDeadzone));
	}

	// Token: 0x06008418 RID: 33816 RVA: 0x00357A08 File Offset: 0x00355C08
	public void UpdateInvertMouse(bool _invertMouse)
	{
		this.invertMouse = (_invertMouse ? -1 : 1);
	}

	// Token: 0x06008419 RID: 33817 RVA: 0x00357A18 File Offset: 0x00355C18
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateLookSensitivity(float _sensitivity, float _zoomSensitivity, float _zoomAccel, float _vehicleSensitivity, Vector2 _controllerLookSensitivity)
	{
		this.mouseLookSensitivity = Vector3.one * _sensitivity * 5f;
		this.mouseZoomSensitivity = _zoomSensitivity;
		this.zoomAccel = _zoomAccel * 0.5f;
		this.vehicleLookSensitivity = _vehicleSensitivity * 5f;
		this.controllerLookSensitivity = _controllerLookSensitivity * 10f;
	}

	// Token: 0x0600841A RID: 33818 RVA: 0x00357A79 File Offset: 0x00355C79
	public Vector2 GetCameraInputSensitivity()
	{
		if (this.playerInput.LastInputType == BindingSourceType.DeviceBindingSource)
		{
			return this.controllerLookSensitivity;
		}
		return this.mouseLookSensitivity;
	}

	// Token: 0x0600841B RID: 33819 RVA: 0x00357A96 File Offset: 0x00355C96
	public bool GetControllerVibration()
	{
		return this.bControllerVibration;
	}

	// Token: 0x0600841C RID: 33820 RVA: 0x00357A9E File Offset: 0x00355C9E
	public void UpdateControllerVibration(bool _controllerVibration)
	{
		this.bControllerVibration = _controllerVibration;
	}

	// Token: 0x0600841D RID: 33821 RVA: 0x00357AA8 File Offset: 0x00355CA8
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnDestroy()
	{
		for (int i = 0; i < this.globalActions.Count; i++)
		{
			this.windowManager.RemoveGlobalAction(this.globalActions[i]);
		}
		this.focusBoxScript.Cleanup();
		PlayerMoveController.Instance = null;
		this.playerUI = null;
		this.windowManager = null;
		this.nguiWindowManager = null;
	}

	// Token: 0x0600841E RID: 33822 RVA: 0x00357B08 File Offset: 0x00355D08
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnGUI()
	{
		if (!this.gameManager.gameStateManager.IsGameStarted() || GameStats.GetInt(EnumGameStats.GameState) != 1)
		{
			return;
		}
		if (this.windowManager.IsFullHUDDisabled())
		{
			return;
		}
		if (this.entityPlayerLocal.inventory != null && this.gameManager.World.worldTime % 2UL == 0UL)
		{
			ItemValue holdingItemItemValue = this.entityPlayerLocal.inventory.holdingItemItemValue;
			ItemClass forId = ItemClass.GetForId(holdingItemItemValue.type);
			int maxUseTimes = holdingItemItemValue.MaxUseTimes;
			if (maxUseTimes > 0 && forId.MaxUseTimesBreaksAfter.Value && holdingItemItemValue.UseTimes >= (float)maxUseTimes)
			{
				this.entityPlayerLocal.inventory.DecHoldingItem(1);
				if (forId.Properties.Values.ContainsKey(ItemClass.PropSoundDestroy))
				{
					Manager.BroadcastPlay(this.entityPlayerLocal, forId.Properties.Values[ItemClass.PropSoundDestroy], false);
				}
			}
			this.entityPlayerLocal.equipment.CheckBreakUseItems();
		}
		if (!this.windowManager.IsInputActive() && !this.windowManager.IsModalWindowOpen() && Event.current.rawType == EventType.KeyDown && this.gameManager.IsEditMode() && this.entityPlayerLocal.inventory != null)
		{
			this.gameManager.GetActiveBlockTool().CheckSpecialKeys(Event.current, this.playerInput);
			if (XUiC_WoPropsPOIMarker.Instance != null)
			{
				XUiC_WoPropsPOIMarker.Instance.CheckSpecialKeys(Event.current, this.playerInput);
			}
		}
	}

	// Token: 0x0600841F RID: 33823 RVA: 0x00357C7C File Offset: 0x00355E7C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Start()
	{
		if (this.entityPlayerLocal.inventory.GetFocusedItemIdx() < 0 || this.entityPlayerLocal.inventory.GetFocusedItemIdx() >= this.entityPlayerLocal.inventory.PUBLIC_SLOTS)
		{
			this.entityPlayerLocal.inventory.SetFocusedItemIdx(0);
			this.entityPlayerLocal.inventory.SetHoldingItemIdx(0);
		}
	}

	// Token: 0x06008420 RID: 33824 RVA: 0x00357CE4 File Offset: 0x00355EE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateRespawn()
	{
		if (this.entityPlayerLocal.Spawned)
		{
			if (this.unstuckCoState == ERoutineState.Running && (this.playerInput.GUIActions.Cancel.WasPressed || this.windowManager.IsWindowOpen(XUiC_InGameMenuWindow.ID)))
			{
				this.unstuckCoState = ERoutineState.Cancelled;
			}
			return;
		}
		if (GameManager.IsVideoPlaying())
		{
			return;
		}
		if (!this.bLastRespawnActive)
		{
			this.spawnWindowOpened = false;
			this.spawnPosition = SpawnPosition.Undef;
			this.entityPlayerLocal.BeforePlayerRespawn(this.respawnReason);
			this.bLastRespawnActive = true;
			this.waitingForSpawnPointSelection = false;
		}
		this.entityPlayerLocal.ResetLastTickPos(this.entityPlayerLocal.GetPosition());
		this.respawnTime -= Time.deltaTime;
		if (this.respawnTime > 0f)
		{
			return;
		}
		this.respawnTime = 0f;
		if (this.spawnWindowOpened && XUiC_SpawnSelectionWindow.IsOpenInUI(LocalPlayerUI.primaryUI))
		{
			if (Mathf.Abs(this.entityPlayerLocal.GetPosition().y - Constants.cStartPositionPlayerInLevel.y) < 0.01f)
			{
				Vector3 position = this.entityPlayerLocal.GetPosition();
				Vector3i blockPosition = this.entityPlayerLocal.GetBlockPosition();
				if (this.gameManager.World.GetChunkFromWorldPos(blockPosition) != null)
				{
					float num = (float)(this.gameManager.World.GetHeight(blockPosition.x, blockPosition.z) + 1);
					if (position.y < 0f || num < position.y || (num > position.y && num - 2.5f < position.y))
					{
						this.entityPlayerLocal.SetPosition(new Vector3(this.entityPlayerLocal.GetPosition().x, num, this.entityPlayerLocal.GetPosition().z), true);
					}
				}
			}
			if (this.playerAutoPilotControllor != null && this.playerAutoPilotControllor.IsEnabled())
			{
				XUiC_SpawnSelectionWindow.Close(LocalPlayerUI.primaryUI);
			}
			return;
		}
		bool flag = this.respawnReason == RespawnType.NewGame || this.respawnReason == RespawnType.EnterMultiplayer || this.respawnReason == RespawnType.JoinMultiplayer || this.respawnReason == RespawnType.LoadedGame;
		Entity entity = this.entityPlayerLocal.AttachedToEntity ? this.entityPlayerLocal.AttachedToEntity : this.entityPlayerLocal;
		switch (this.respawnReason)
		{
		case RespawnType.NewGame:
			if (!this.spawnWindowOpened)
			{
				this.openSpawnWindow(this.respawnReason);
				return;
			}
			this.spawnPosition = new SpawnPosition(this.entityPlayerLocal.GetPosition(), this.entityPlayerLocal.rotation.y);
			goto IL_5E2;
		case RespawnType.LoadedGame:
			if (!this.spawnWindowOpened)
			{
				this.spawnPosition = new SpawnPosition(this.entityPlayerLocal.GetPosition(), this.entityPlayerLocal.rotation.y);
				this.entityPlayerLocal.SetPosition(this.spawnPosition.position, true);
				this.openSpawnWindow(this.respawnReason);
				return;
			}
			this.spawnPosition = new SpawnPosition(this.entityPlayerLocal.GetPosition(), this.entityPlayerLocal.rotation.y);
			goto IL_5E2;
		case RespawnType.Teleport:
			this.entityPlayerLocal.UpdateRespawn();
			this.spawnPosition = new SpawnPosition(entity.GetPosition(), entity.rotation.y);
			this.spawnPosition.position.y = -1f;
			goto IL_5E2;
		case RespawnType.EnterMultiplayer:
		case RespawnType.JoinMultiplayer:
			if (!this.spawnWindowOpened)
			{
				this.spawnPosition = new SpawnPosition(this.entityPlayerLocal.GetPosition(), this.entityPlayerLocal.rotation.y);
				if ((this.spawnPosition.IsUndef() || this.spawnPosition.position.Equals(Constants.cStartPositionPlayerInLevel)) && !this.entityPlayerLocal.lastSpawnPosition.IsUndef())
				{
					this.spawnPosition = this.entityPlayerLocal.lastSpawnPosition;
				}
				if (this.spawnPosition.IsUndef() || this.spawnPosition.position.Equals(Constants.cStartPositionPlayerInLevel))
				{
					this.spawnPosition = this.gameManager.GetSpawnPointList().GetRandomSpawnPosition(this.entityPlayerLocal.world, null, 0, 0);
				}
				this.entityPlayerLocal.SetPosition(new Vector3(this.spawnPosition.position.x, (this.spawnPosition.position.y == 0f) ? Constants.cStartPositionPlayerInLevel.y : this.spawnPosition.position.y, this.spawnPosition.position.z), true);
				this.openSpawnWindow(this.respawnReason);
				return;
			}
			this.spawnPosition = new SpawnPosition(this.entityPlayerLocal.GetPosition(), this.entityPlayerLocal.rotation.y);
			goto IL_5E2;
		}
		if (!this.gameManager.IsEditMode() && !this.spawnWindowOpened)
		{
			this.openSpawnWindow(this.respawnReason);
			return;
		}
		XUiC_SpawnSelectionWindow window = XUiC_SpawnSelectionWindow.GetWindow(LocalPlayerUI.primaryUI);
		if (!this.waitingForSpawnPointSelection && !this.gameManager.IsEditMode() && this.spawnWindowOpened && window.spawnMethod != SpawnMethod.Invalid)
		{
			base.StartCoroutine(this.FindRespawnSpawnPointRoutine(window.spawnMethod, window.spawnTarget));
			window.spawnMethod = SpawnMethod.Invalid;
			window.spawnTarget = SpawnPosition.Undef;
		}
		if (this.waitingForSpawnPointSelection)
		{
			return;
		}
		if (this.entityPlayerLocal.position != this.spawnPosition.position)
		{
			Vector3 position2 = this.spawnPosition.position;
			if (this.spawnPosition.IsUndef())
			{
				position2 = this.entityPlayerLocal.GetPosition();
			}
			this.spawnPosition = new SpawnPosition(position2 + new Vector3(0f, 5f, 0f), this.entityPlayerLocal.rotation.y);
			this.entityPlayerLocal.SetPosition(this.spawnPosition.position, true);
		}
		IL_5E2:
		if (GameUtils.IsPlaytesting() || (GameManager.Instance.IsEditMode() && GamePrefs.GetString(EnumGamePrefs.GameWorld) == "Empty"))
		{
			SpawnPointList spawnPointList = GameManager.Instance.GetSpawnPointList();
			if (this.respawnReason != RespawnType.Teleport && spawnPointList.Count > 0)
			{
				this.spawnPosition.position = spawnPointList[0].spawnPosition.position;
				this.spawnPosition.heading = spawnPointList[0].spawnPosition.heading;
				this.entityPlayerLocal.SetPosition(this.spawnPosition.position, true);
			}
		}
		if (!this.spawnPosition.IsUndef())
		{
			if (!PrefabEditModeManager.Instance.IsActive() && !this.gameManager.World.IsPositionAvailable(this.spawnPosition.ClrIdx, this.spawnPosition.position))
			{
				this.spawnPosition.position = this.gameManager.World.ClampToValidWorldPos(this.spawnPosition.position);
				if (this.entityPlayerLocal.position != this.spawnPosition.position)
				{
					this.entityPlayerLocal.SetPosition(this.spawnPosition.position, true);
				}
				return;
			}
			if (!this.entityPlayerLocal.CheckSpawnPointStillThere())
			{
				this.entityPlayerLocal.RemoveSpawnPoints(true);
				if (flag)
				{
					this.entityPlayerLocal.QuestJournal.RemoveAllSharedQuests();
					this.entityPlayerLocal.QuestJournal.StartQuests();
				}
			}
			Vector3i vector3i = World.worldToBlockPos(this.spawnPosition.position);
			float num2 = (float)(this.gameManager.World.GetHeight(vector3i.x, vector3i.z) + 1);
			if (this.spawnPosition.position.y < 0f || this.spawnPosition.position.y > num2)
			{
				this.spawnPosition.position.y = num2;
			}
			else if (this.spawnPosition.position.y < num2 && !this.gameManager.World.CanPlayersSpawnAtPos(this.spawnPosition.position, true))
			{
				this.spawnPosition.position.y = this.spawnPosition.position.y + 1f;
				if (!this.gameManager.World.CanPlayersSpawnAtPos(this.spawnPosition.position, true))
				{
					this.spawnPosition.position.y = num2;
				}
			}
		}
		Log.Out("Respawn almost done");
		if (this.spawnPosition.IsUndef())
		{
			this.entityPlayerLocal.Respawn(this.respawnReason);
			return;
		}
		RaycastHit raycastHit;
		float num3;
		if (Physics.Raycast(new Ray(this.spawnPosition.position + Vector3.up - Origin.position, Vector3.down), out raycastHit, 3f, 1342242816))
		{
			num3 = raycastHit.point.y - this.spawnPosition.position.y + Origin.position.y;
		}
		else
		{
			num3 = this.gameManager.World.GetTerrainOffset(0, World.worldToBlockPos(this.spawnPosition.position)) + 0.05f;
		}
		this.gameManager.ClearTooltips(this.nguiWindowManager);
		this.spawnPosition.position.y = this.spawnPosition.position.y + num3;
		this.entityPlayerLocal.onGround = true;
		this.entityPlayerLocal.lastSpawnPosition = this.spawnPosition;
		this.entityPlayerLocal.Spawned = true;
		GameManager.Instance.PlayerSpawnedInWorld(null, this.respawnReason, new Vector3i(this.spawnPosition.position), this.entityPlayerLocal.entityId);
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToClientsOrServer(NetPackageManager.GetPackage<NetPackagePlayerSpawnedInWorld>().Setup(this.respawnReason, new Vector3i(this.spawnPosition.position), this.entityPlayerLocal.entityId));
		if (this.respawnReason == RespawnType.Died || this.respawnReason == RespawnType.EnterMultiplayer || this.respawnReason == RespawnType.NewGame)
		{
			this.entityPlayerLocal.SetAlive();
		}
		else
		{
			this.entityPlayerLocal.bDead = false;
		}
		if (this.respawnReason == RespawnType.NewGame || this.respawnReason == RespawnType.LoadedGame || this.respawnReason == RespawnType.EnterMultiplayer || this.respawnReason == RespawnType.JoinMultiplayer)
		{
			this.entityPlayerLocal.TryAddRecoveryPosition(Vector3i.FromVector3Rounded(this.spawnPosition.position));
		}
		this.entityPlayerLocal.ResetLastTickPos(this.spawnPosition.position);
		if (!this.entityPlayerLocal.AttachedToEntity)
		{
			this.entityPlayerLocal.transform.position = this.spawnPosition.position - Origin.position;
		}
		else
		{
			this.spawnPosition.position.y = this.spawnPosition.position.y + 2f;
		}
		entity.SetPosition(this.spawnPosition.position, true);
		entity.SetRotation(new Vector3(0f, this.spawnPosition.heading, 0f));
		this.entityPlayerLocal.JetpackWearing = false;
		this.entityPlayerLocal.ParachuteWearing = false;
		this.entityPlayerLocal.AfterPlayerRespawn(this.respawnReason);
		if (flag)
		{
			this.entityPlayerLocal.QuestJournal.RemoveAllSharedQuests();
			this.entityPlayerLocal.QuestJournal.StartQuests();
		}
		if ((this.respawnReason == RespawnType.NewGame || this.respawnReason == RespawnType.EnterMultiplayer) && !GameManager.Instance.World.IsEditor() && !(GameMode.GetGameModeForId(GameStats.GetInt(EnumGameStats.GameModeId)) is GameModeCreative) && !GameUtils.IsPlaytesting() && !GameManager.bRecordNextSession && !GameManager.bPlayRecordedSession)
		{
			GameEventManager.Current.HandleAction("game_first_spawn", this.entityPlayerLocal, this.entityPlayerLocal, false, "", "", false, true, "", null);
		}
		if (this.respawnReason != RespawnType.Died && this.respawnReason != RespawnType.Teleport && GameStats.GetBool(EnumGameStats.AutoParty) && this.entityPlayerLocal.Party == null)
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePartyActions>().Setup(NetPackagePartyActions.PartyActions.JoinAutoParty, this.entityPlayerLocal.entityId, this.entityPlayerLocal.entityId, null, null), false);
			}
			else
			{
				Party.ServerHandleAutoJoinParty(this.entityPlayerLocal);
			}
		}
		if (this.respawnReason == RespawnType.JoinMultiplayer || this.respawnReason == RespawnType.LoadedGame)
		{
			this.entityPlayerLocal.ReassignEquipmentTransforms();
			GameEventManager.Current.HandleAction("game_on_spawn", this.entityPlayerLocal, this.entityPlayerLocal, false, "", "", false, true, "", null);
		}
		this.entityPlayerLocal.EnableCamera(true);
		GameManager.Instance.World.RefreshEntitiesOnMap();
		LocalPlayerUI.primaryUI.windowManager.Close(XUiC_LoadingScreen.ID);
		LocalPlayerUI.primaryUI.windowManager.Close("eacWarning");
		LocalPlayerUI.primaryUI.windowManager.Close("crossplayWarning");
		if (flag && PlatformManager.NativePlatform.GameplayNotifier != null)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				bool isOnlineMultiplayer = SingletonMonoBehaviour<ConnectionManager>.Instance.CurrentMode == ProtocolManager.NetworkType.Server;
				bool allowsCrossplay = SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.AllowsCrossplay;
				PlatformManager.NativePlatform.GameplayNotifier.GameplayStart(isOnlineMultiplayer, allowsCrossplay);
			}
			else if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				bool allowsCrossplay2 = SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.AllowsCrossplay;
				PlatformManager.NativePlatform.GameplayNotifier.GameplayStart(true, allowsCrossplay2);
			}
		}
		if (this.respawnReason == RespawnType.Died)
		{
			this.entityPlayerLocal.QuestJournal.FailAllActivatedQuests();
			this.entityPlayerLocal.Progression.OnRespawnFromDeath();
			switch (GameStats.GetInt(EnumGameStats.DeathPenalty))
			{
			case 0:
				GameEventManager.Current.HandleAction("game_on_respawn_none", this.entityPlayerLocal, this.entityPlayerLocal, false, "", "", false, true, "", null);
				break;
			case 1:
				GameEventManager.Current.HandleAction("game_on_respawn_default", this.entityPlayerLocal, this.entityPlayerLocal, false, "", "", false, true, "", null);
				break;
			case 2:
				GameEventManager.Current.HandleAction("game_on_respawn_injured", this.entityPlayerLocal, this.entityPlayerLocal, false, "", "", false, true, "", null);
				break;
			case 3:
				GameEventManager.Current.HandleAction("game_on_respawn_permanent", this.entityPlayerLocal, this.entityPlayerLocal, false, "", "", false, true, "", null);
				break;
			}
			this.entityPlayerLocal.ResetBiomeWeatherOnDeath();
		}
		if (!this.gameManager.IsEditMode() && (this.respawnReason == RespawnType.NewGame || this.respawnReason == RespawnType.EnterMultiplayer))
		{
			this.windowManager.TempHUDDisable();
			this.entityPlayerLocal.SetControllable(false);
			this.entityPlayerLocal.bIntroAnimActive = true;
			GameManager.Instance.StartCoroutine(this.showUILater());
			if (!GameUtils.IsPlaytesting())
			{
				GameManager.Instance.StartCoroutine(this.initializeHoldingItemLater(4f));
			}
		}
		else
		{
			this.entityPlayerLocal.SetControllable(true);
			GameManager.Instance.StartCoroutine(this.initializeHoldingItemLater(0.1f));
		}
		this.bLastRespawnActive = false;
	}

	// Token: 0x06008421 RID: 33825 RVA: 0x00358BC3 File Offset: 0x00356DC3
	public IEnumerator UnstuckPlayerCo()
	{
		if (!this.entityPlayerLocal.Spawned || this.unstuckCoState == ERoutineState.Running)
		{
			yield break;
		}
		this.unstuckCoState = ERoutineState.Running;
		SpawnPosition spawnTarget = new SpawnPosition(this.entityPlayerLocal.GetPosition(), this.entityPlayerLocal.rotation.y);
		GameManager.ShowTooltip(this.entityPlayerLocal, string.Format(Localization.Get("xuiMenuUnstuckTooltip", false), 5), true, false, 0f);
		DateTime currentTime = DateTime.Now;
		yield return this.FindRespawnSpawnPointRoutine(SpawnMethod.Unstuck, spawnTarget);
		if (this.unstuckCoState == ERoutineState.Cancelled)
		{
			GameManager.ShowTooltip(this.entityPlayerLocal, Localization.Get("xuiMenuUnstuckCancelled", false), true, false, 0f);
			yield break;
		}
		double remainingTime = 5.0 - (DateTime.Now - currentTime).TotalSeconds;
		while (remainingTime > 0.0)
		{
			GameManager.ShowTooltip(this.entityPlayerLocal, string.Format(Localization.Get("xuiMenuUnstuckTooltip", false), (int)(remainingTime + 0.5)), true, false, 0f);
			yield return new WaitForSeconds(Math.Min(1f, (float)remainingTime));
			remainingTime -= 1.0;
			if (this.unstuckCoState == ERoutineState.Cancelled)
			{
				GameManager.ShowTooltip(this.entityPlayerLocal, Localization.Get("xuiMenuUnstuckCancelled", false), true, false, 0f);
				yield break;
			}
		}
		if (!this.waitingForSpawnPointSelection && !this.spawnPosition.IsUndef())
		{
			this.entityPlayerLocal.TeleportToPosition(this.spawnPosition.position, false, null);
		}
		this.unstuckCoState = ERoutineState.Succeeded;
		yield break;
	}

	// Token: 0x06008422 RID: 33826 RVA: 0x00358BD2 File Offset: 0x00356DD2
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator initializeHoldingItemLater(float _time)
	{
		yield return new WaitForSeconds(_time);
		if (this.entityPlayerLocal != null && this.entityPlayerLocal.inventory != null)
		{
			this.entityPlayerLocal.inventory.ForceHoldingItemUpdate();
		}
		yield break;
	}

	// Token: 0x06008423 RID: 33827 RVA: 0x00358BE8 File Offset: 0x00356DE8
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator showUILater()
	{
		yield return new WaitForSeconds(4f);
		if (this.entityPlayerLocal != null && this.entityPlayerLocal.transform != null)
		{
			this.entityPlayerLocal.bIntroAnimActive = false;
			this.entityPlayerLocal.SetControllable(true);
		}
		if (this.windowManager != null)
		{
			this.windowManager.ReEnableHUD();
		}
		yield break;
	}

	// Token: 0x06008424 RID: 33828 RVA: 0x00358BF8 File Offset: 0x00356DF8
	[PublicizedFrom(EAccessModifier.Private)]
	public void openSpawnWindow(RespawnType _respawnReason)
	{
		Log.Out("OpenSpawnWindow");
		XUiC_ProgressWindow.Close(LocalPlayerUI.primaryUI);
		if (!this.gameManager.IsEditMode())
		{
			LocalPlayerUI.primaryUI.windowManager.Open(XUiC_LoadingScreen.ID, false, true, true);
		}
		XUiC_SpawnSelectionWindow.Open(LocalPlayerUI.primaryUI, _respawnReason != RespawnType.EnterMultiplayer && _respawnReason != RespawnType.JoinMultiplayer && _respawnReason != RespawnType.NewGame && _respawnReason != RespawnType.Teleport && _respawnReason != RespawnType.LoadedGame, false, false);
		this.spawnWindowOpened = true;
	}

	// Token: 0x06008425 RID: 33829 RVA: 0x00358C6C File Offset: 0x00356E6C
	[PublicizedFrom(EAccessModifier.Private)]
	public SpawnPosition findSpawnPosition(SpawnMethod _spawnMethod, SpawnPosition _spawnTarget)
	{
		SpawnPosition spawnPosition = SpawnPosition.Undef;
		if (_spawnMethod == SpawnMethod.OnBedRoll && spawnPosition.IsUndef())
		{
			spawnPosition = _spawnTarget;
			if (!spawnPosition.IsUndef())
			{
				string str = string.Format("Spawn pos: {0} ", SpawnMethod.OnBedRoll);
				SpawnPosition spawnPosition2 = spawnPosition;
				Log.Out(str + spawnPosition2.ToString());
			}
		}
		if ((_spawnMethod == SpawnMethod.NearBedroll || _spawnMethod == SpawnMethod.NearBackpack || _spawnMethod == SpawnMethod.NearDeath) && spawnPosition.IsUndef() && !_spawnTarget.IsUndef())
		{
			Vector3 position;
			if (this.gameManager.World.GetRandomSpawnPositionMinMaxToPosition(_spawnTarget.position, 48, 96, 48, false, out position, this.entityPlayerLocal.entityId, true, 50, false, EnumLandClaimOwner.None, false))
			{
				spawnPosition.position = position;
				string str2 = string.Format("Spawn pos: random {0} ", _spawnMethod);
				SpawnPosition spawnPosition2 = spawnPosition;
				Log.Out(str2 + spawnPosition2.ToString());
			}
			if (spawnPosition.IsUndef() && this.entityPlayerLocal.recoveryPositions.Count > 0)
			{
				for (int i = this.entityPlayerLocal.recoveryPositions.Count - 1; i >= 0; i--)
				{
					if (Vector3.Distance(this.entityPlayerLocal.recoveryPositions[i], _spawnTarget.position) > 48f)
					{
						spawnPosition.position = this.entityPlayerLocal.recoveryPositions[i];
						string str3 = "Spawn pos: Recovery Point ";
						SpawnPosition spawnPosition2 = spawnPosition;
						Log.Out(str3 + spawnPosition2.ToString());
						break;
					}
				}
			}
		}
		if (_spawnMethod == SpawnMethod.NewRandomSpawn && spawnPosition.IsUndef() && !_spawnTarget.IsUndef())
		{
			spawnPosition = _spawnTarget;
			string str4 = string.Format("Spawn pos: random {0} ", _spawnMethod);
			SpawnPosition spawnPosition2 = spawnPosition;
			Log.Out(str4 + spawnPosition2.ToString());
		}
		if (_spawnMethod == SpawnMethod.Unstuck && spawnPosition.IsUndef() && !_spawnTarget.IsUndef())
		{
			Vector3 position2;
			if (this.gameManager.World.GetRandomSpawnPositionMinMaxToPosition(_spawnTarget.position, 0, 16, 0, false, out position2, this.entityPlayerLocal.entityId, true, 100, false, EnumLandClaimOwner.None, false))
			{
				spawnPosition.position = position2;
				string str5 = "Spawn pos: try 'unstuck' player ";
				SpawnPosition spawnPosition2 = spawnPosition;
				Log.Out(str5 + spawnPosition2.ToString());
			}
			if (spawnPosition.IsUndef() && this.entityPlayerLocal.recoveryPositions.Count >= 2)
			{
				spawnPosition.position = this.entityPlayerLocal.recoveryPositions[this.entityPlayerLocal.recoveryPositions.Count - 2];
				string str6 = "Spawn pos: try 'unstuck' player at Recovery Point ";
				SpawnPosition spawnPosition2 = spawnPosition;
				Log.Out(str6 + spawnPosition2.ToString());
			}
		}
		if (spawnPosition.IsUndef())
		{
			if (!_spawnTarget.IsUndef())
			{
				spawnPosition = this.gameManager.GetSpawnPointList().GetRandomSpawnPosition(this.entityPlayerLocal.world, new Vector3?(_spawnTarget.position), 300, 600);
				string str7 = "Spawn pos: start point ";
				SpawnPosition spawnPosition2 = spawnPosition;
				Log.Out(str7 + spawnPosition2.ToString() + " distance to backpack: " + (spawnPosition.position - _spawnTarget.position).magnitude.ToCultureInvariantString());
			}
			else
			{
				spawnPosition = this.gameManager.GetSpawnPointList().GetRandomSpawnPosition(this.entityPlayerLocal.world, new Vector3?(this.entityPlayerLocal.position), 300, 600);
				string str8 = "Spawn pos: start point ";
				SpawnPosition spawnPosition2 = spawnPosition;
				Log.Out(str8 + spawnPosition2.ToString());
			}
		}
		if (spawnPosition.IsUndef())
		{
			int x = Utils.Fastfloor(this.entityPlayerLocal.position.x);
			int y = Utils.Fastfloor(this.entityPlayerLocal.position.y);
			int z = Utils.Fastfloor(this.entityPlayerLocal.position.z);
			IChunk chunkFromWorldPos = this.gameManager.World.GetChunkFromWorldPos(x, y, z);
			if (chunkFromWorldPos != null)
			{
				if (this.entityPlayerLocal.position.y == Constants.cStartPositionPlayerInLevel.y)
				{
					this.entityPlayerLocal.position.y = (float)(chunkFromWorldPos.GetHeight(ChunkBlockLayerLegacy.CalcOffset(x, z)) + 1);
				}
				spawnPosition = new SpawnPosition(this.entityPlayerLocal.GetPosition(), this.entityPlayerLocal.rotation.y);
				string str9 = "Spawn pos: current player pos ";
				SpawnPosition spawnPosition2 = spawnPosition;
				Log.Out(str9 + spawnPosition2.ToString());
			}
		}
		return spawnPosition;
	}

	// Token: 0x06008426 RID: 33830 RVA: 0x003590DD File Offset: 0x003572DD
	public IEnumerator FindRespawnSpawnPointRoutine(SpawnMethod _method, SpawnPosition _spawnTarget)
	{
		this.waitingForSpawnPointSelection = true;
		Vector3 targetPosition = this.entityPlayerLocal.position;
		if (!_spawnTarget.IsUndef())
		{
			targetPosition = _spawnTarget.position;
		}
		this.entityPlayerLocal.SetPosition(targetPosition, true);
		yield return new WaitForSeconds(2f);
		float waitTime = 0f;
		while (!GameManager.Instance.World.IsChunkAreaLoaded(targetPosition) && waitTime < 5f)
		{
			yield return new WaitForSeconds(0.25f);
			waitTime += 0.25f;
		}
		this.spawnPosition = this.findSpawnPosition(_method, _spawnTarget);
		this.waitingForSpawnPointSelection = false;
		yield break;
	}

	// Token: 0x06008427 RID: 33831 RVA: 0x003590FA File Offset: 0x003572FA
	[PublicizedFrom(EAccessModifier.Private)]
	public void stopMoving()
	{
		this.isAutorun = false;
		this.entityPlayerLocal.movementInput.moveForward = 0f;
		this.entityPlayerLocal.movementInput.moveStrafe = 0f;
		this.entityPlayerLocal.MoveByInput();
	}

	// Token: 0x06008428 RID: 33832 RVA: 0x00359138 File Offset: 0x00357338
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateDebugKeys()
	{
		if (this.windowManager.IsModalWindowOpen())
		{
			return;
		}
		if (!GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled))
		{
			return;
		}
		bool flag = this.playerUI.windowManager.IsInputActive();
		bool flag2 = flag || this.wasUIInputActive;
		this.wasUIInputActive = flag;
		if (flag2)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Keypad0))
		{
			Manager.PlayButtonClick();
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageDebug>().Setup(NetPackageDebug.Type.AINameInfoServerToggle, -1, null), false);
			}
			else
			{
				bool flag3 = !GamePrefs.GetBool(EnumGamePrefs.DebugMenuShowTasks);
				GamePrefs.Set(EnumGamePrefs.DebugMenuShowTasks, flag3);
				EntityAlive.SetupAllDebugNameHUDs(flag3);
			}
		}
		if (this.gameManager.World.IsRemote())
		{
			return;
		}
		bool shiftKeyPressed = InputUtils.ShiftKeyPressed;
		if (!shiftKeyPressed)
		{
			float num = Time.timeScale;
			if (Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				if (num > 0f)
				{
					num = 0f;
				}
				else
				{
					num = 1f;
				}
			}
			if (Input.GetKeyDown(KeyCode.KeypadMinus))
			{
				num = Mathf.Max(num - 0.05f, 0f);
			}
			if (Input.GetKeyDown(KeyCode.KeypadPlus))
			{
				num = Mathf.Min(num + 0.05f, 2f);
			}
			if (num != Time.timeScale)
			{
				Time.timeScale = num;
				Log.Out("Time scale {0}", new object[]
				{
					num.ToCultureInvariantString()
				});
				Manager.PlayButtonClick();
			}
			if (Input.GetKeyDown(KeyCode.KeypadPeriod))
			{
				if (InputUtils.ControlKeyPressed)
				{
					if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
					{
						SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync("killall all", null);
					}
					else
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageConsoleCmdServer>().Setup("killall all"), false);
					}
				}
				else
				{
					float d = (float)(InputUtils.AltKeyPressed ? 100 : 0);
					this.entityPlayerLocal.emodel.DoRagdoll(2f, EnumBodyPartHit.Torso, this.entityPlayerLocal.rand.RandomInsideUnitSphere * d, this.entityPlayerLocal.transform.position + this.entityPlayerLocal.rand.RandomInsideUnitSphere * 0.1f + Origin.position, false);
				}
			}
		}
		else if (Input.GetKeyDown(KeyCode.KeypadPlus))
		{
			this.drawChunkMode = (this.drawChunkMode + 1) % 3;
		}
		if (this.playerInput.AiFreeze.WasPressed && !GameManager.Instance.IsEditMode())
		{
			Manager.PlayButtonClick();
			if (InputUtils.ControlKeyPressed)
			{
				EAIManager.ToggleAnimFreeze();
				if (EAIManager.isAnimFreeze)
				{
					this.entityPlayerLocal.Buffs.AddBuff("buffShowAnimationDisabled", -1, true, false, -1f);
					return;
				}
				this.entityPlayerLocal.Buffs.RemoveBuff("buffShowAnimationDisabled", true);
				return;
			}
			else
			{
				if (shiftKeyPressed)
				{
					this.entityPlayerLocal.SetIgnoredByAI(!this.entityPlayerLocal.IsIgnoredByAI());
					return;
				}
				bool flag4 = !GamePrefs.GetBool(EnumGamePrefs.DebugStopEnemiesMoving);
				GamePrefs.Set(EnumGamePrefs.DebugStopEnemiesMoving, flag4);
				if (flag4)
				{
					this.entityPlayerLocal.Buffs.AddBuff("buffShowAIDisabled", -1, true, false, -1f);
					return;
				}
				this.entityPlayerLocal.Buffs.RemoveBuff("buffShowAIDisabled", true);
			}
		}
	}

	// Token: 0x06008429 RID: 33833 RVA: 0x0035944C File Offset: 0x0035764C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool canSwapHeldItem()
	{
		ItemActionEat itemActionEat = this.entityPlayerLocal.inventory.holdingItem.Actions[0] as ItemActionEat;
		bool flag = false;
		if (itemActionEat != null)
		{
			flag = (itemActionEat.PercentDone(this.entityPlayerLocal.inventory.holdingItemData.actionData[0]) > 0.75f);
		}
		return this.entityPlayerLocal.inventory.GetIsFinishedSwitchingHeldItem() && !flag;
	}

	// Token: 0x0600842A RID: 33834 RVA: 0x003594BC File Offset: 0x003576BC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Update()
	{
		if (this.nextHeldItem.Count > 0 && this.canSwapHeldItem())
		{
			this.swapItem(this.nextHeldItem[this.nextHeldItem.Count - 1]);
			this.nextHeldItem.Clear();
		}
		PlayerActionsLocal playerInput = this.playerInput;
		bool flag = !this.playerUI.windowManager.IsCursorWindowOpen() && !this.playerUI.windowManager.IsModalWindowOpen() && (playerInput.Enabled || playerInput.VehicleActions.Enabled);
		if (DroneManager.Debug_LocalControl)
		{
			flag = false;
		}
		if (this.playerAutoPilotControllor != null && this.playerAutoPilotControllor.IsEnabled())
		{
			this.playerAutoPilotControllor.Update();
		}
		if ((!this.bCanControlOverride || !flag) && GamePrefs.GetInt(EnumGamePrefs.SelectionOperationMode) == 0)
		{
			XUiC_InteractionPrompt.SetText(this.playerUI, null);
			this.strTextLabelPointingTo = string.Empty;
		}
		if (!this.gameManager.gameStateManager.IsGameStarted() || GameStats.GetInt(EnumGameStats.GameState) != 1)
		{
			this.stopMoving();
			return;
		}
		if (this.entityPlayerLocal.PlayerUI.windowManager.IsModalWindowOpen())
		{
			if (!this.IsGUICancelPressed && playerInput.PermanentActions.Cancel.WasPressed)
			{
				this.IsGUICancelPressed = true;
			}
		}
		else if (this.IsGUICancelPressed)
		{
			this.IsGUICancelPressed = playerInput.PermanentActions.Cancel.GetBindingOfType(playerInput.ActiveDevice.DeviceClass == InputDeviceClass.Controller).GetState(playerInput.ActiveDevice);
		}
		this.updateRespawn();
		if (this.unstuckCoState == ERoutineState.Running)
		{
			this.stopMoving();
			return;
		}
		this.updateDebugKeys();
		if (this.drawChunkMode > 0)
		{
			this.DrawChunkBoundary();
			if (this.drawChunkMode == 2)
			{
				this.DrawChunkDensities();
			}
		}
		if (this.entityPlayerLocal.emodel.IsRagdollActive)
		{
			this.stopMoving();
			return;
		}
		if (this.entityPlayerLocal.IsDead())
		{
			XUiC_InteractionPrompt.SetText(this.playerUI, null);
			this.strTextLabelPointingTo = string.Empty;
			return;
		}
		bool flag2 = false;
		float num = playerInput.Scroll.Value;
		if (playerInput.LastInputType == BindingSourceType.DeviceBindingSource)
		{
			if (!this.entityPlayerLocal.AimingGun)
			{
				num = 0f;
			}
			else
			{
				num *= 0.25f;
			}
		}
		num *= 0.25f;
		if (Mathf.Abs(num) < 0.001f)
		{
			num = 0f;
		}
		this.gameManager.GetActiveBlockTool().CheckKeys(this.entityPlayerLocal.inventory.holdingItemData, this.entityPlayerLocal.HitInfo, playerInput);
		if (this.gameManager.IsEditMode() || BlockToolSelection.Instance.SelectionActive)
		{
			SelectionBoxManager.Instance.CheckKeys(this.gameManager, playerInput, this.entityPlayerLocal.HitInfo);
			if (!flag2)
			{
				flag2 = SelectionBoxManager.Instance.ConsumeScrollWheel(num, playerInput);
			}
			flag2 = this.gameManager.GetActiveBlockTool().ConsumeScrollWheel(this.entityPlayerLocal.inventory.holdingItemData, num, playerInput);
		}
		if ((!this.bCanControlOverride || !flag) && GamePrefs.GetInt(EnumGamePrefs.SelectionOperationMode) == 0)
		{
			this.stopMoving();
			return;
		}
		bool lastInputController = playerInput.LastInputType == BindingSourceType.DeviceBindingSource;
		this.entityPlayerLocal.movementInput.lastInputController = lastInputController;
		if (!this.IsGUICancelPressed && (!this.gameManager.IsEditMode() || GamePrefs.GetInt(EnumGamePrefs.SelectionOperationMode) == 0))
		{
			bool controlKeyPressed = InputUtils.ControlKeyPressed;
			bool enabled = playerInput.VehicleActions.Enabled;
			bool flag3 = this.wasVehicle != enabled;
			PlayerAction playerAction = enabled ? playerInput.VehicleActions.Turbo : playerInput.Run;
			PlayerAction playerAction2 = enabled ? playerInput.VehicleActions.MoveForward : playerInput.MoveForward;
			if (this.sprintMode == 2)
			{
				if (!enabled)
				{
					PlayerAction moveBack = playerInput.MoveBack;
				}
				else
				{
					PlayerAction moveBack2 = playerInput.VehicleActions.MoveBack;
				}
				bool isPressed = playerAction.IsPressed;
				bool flag4 = Utils.FastAbs(enabled ? playerInput.VehicleActions.Move.Y : playerInput.Move.Y) >= 0.35f || (enabled && playerInput.VehicleActions.Brake);
				flag4 |= this.gameManager.IsEditMode();
				if (!this.isAutorun)
				{
					bool running = this.entityPlayerLocal.movementInput.running;
					this.entityPlayerLocal.movementInput.running = isPressed;
					if (isPressed)
					{
						this.runInputTime += Time.deltaTime;
						if (!running)
						{
							this.runInputTime = 0f;
						}
						if (flag4)
						{
							this.isAutorunInvalid = true;
						}
					}
					else if (playerAction.WasReleased)
					{
						if (!this.isAutorunInvalid && this.runInputTime < 0.2f)
						{
							this.isAutorun = true;
							this.entityPlayerLocal.movementInput.running = true;
						}
						this.isAutorunInvalid = false;
					}
				}
				else
				{
					if (isPressed)
					{
						this.isAutorun = false;
						this.isAutorunInvalid = true;
					}
					if (flag4)
					{
						this.isAutorun = false;
					}
				}
			}
			else
			{
				if (playerAction.WasPressed)
				{
					this.runInputTime = 0f;
					this.entityPlayerLocal.movementInput.running = true;
					this.runPressedWhileActive = true;
				}
				else if (playerAction.WasReleased && this.runPressedWhileActive)
				{
					if (this.runInputTime > 0.2f)
					{
						this.entityPlayerLocal.movementInput.running = false;
						this.runToggleActive = false;
					}
					else if (this.runToggleActive)
					{
						this.runToggleActive = false;
						this.entityPlayerLocal.movementInput.running = false;
					}
					else if (playerAction2.IsPressed || this.sprintMode == 1)
					{
						this.runToggleActive = true;
					}
					else
					{
						this.runToggleActive = false;
						this.entityPlayerLocal.movementInput.running = false;
					}
					this.runPressedWhileActive = false;
				}
				if (playerAction.IsPressed)
				{
					this.runInputTime += Time.deltaTime;
				}
				if (this.runToggleActive)
				{
					if (flag3 || (this.sprintMode == 0 && (this.entityPlayerLocal.Stamina <= 0f || playerAction2.WasReleased)))
					{
						this.runToggleActive = false;
						this.runPressedWhileActive = false;
						this.entityPlayerLocal.movementInput.running = false;
					}
					else
					{
						this.entityPlayerLocal.movementInput.running = true;
					}
				}
			}
			this.entityPlayerLocal.movementInput.down = (playerInput.Crouch.IsPressed && (!this.gameManager.IsEditMode() || !controlKeyPressed));
			this.entityPlayerLocal.movementInput.jump = playerInput.Jump.IsPressed;
			if (this.entityPlayerLocal.movementInput.running && this.entityPlayerLocal.AimingGun)
			{
				this.entityPlayerLocal.AimingGun = false;
			}
			this.wasVehicle = enabled;
		}
		else
		{
			this.runToggleActive = false;
			this.entityPlayerLocal.movementInput.running = false;
			this.runPressedWhileActive = false;
			this.isAutorun = false;
		}
		this.entityPlayerLocal.movementInput.downToggle = (!this.gameManager.IsEditMode() && !this.entityPlayerLocal.IsFlyMode.Value && playerInput.ToggleCrouch.WasPressed);
		if (GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled) && playerInput.PermanentActions.DebugControllerLeft.IsPressed && playerInput.PermanentActions.DebugControllerRight.IsPressed)
		{
			if (playerInput.GodAlternate.WasPressed)
			{
				this.toggleGodMode();
			}
			if (playerInput.TeleportAlternate.WasPressed)
			{
				this.teleportPlayer();
			}
		}
		if (playerInput.DecSpeed.WasPressed)
		{
			this.entityPlayerLocal.GodModeSpeedModifier = Utils.FastMax(0.1f, this.entityPlayerLocal.GodModeSpeedModifier - 0.1f);
		}
		if (playerInput.IncSpeed.WasPressed)
		{
			this.entityPlayerLocal.GodModeSpeedModifier = Utils.FastMin(3f, this.entityPlayerLocal.GodModeSpeedModifier + 0.1f);
		}
		Vector2 vector;
		Vector2 vector2;
		if (playerInput.Look.LastInputType != BindingSourceType.MouseBindingSource)
		{
			this.entityPlayerLocal.movementInput.down = (this.entityPlayerLocal.IsFlyMode.Value && playerInput.ToggleCrouch.IsPressed);
			float magnitude;
			if (playerInput.VehicleActions.Enabled)
			{
				vector.x = playerInput.VehicleActions.Look.X;
				vector.y = playerInput.VehicleActions.Look.Y * (float)this.invertController;
				magnitude = playerInput.VehicleActions.Look.Vector.magnitude;
			}
			else
			{
				vector.x = playerInput.Look.X;
				vector.y = playerInput.Look.Y * (float)this.invertController;
				magnitude = playerInput.Look.Vector.magnitude;
			}
			if (this.lookAccelerationRate <= 0f)
			{
				this.currentLookAcceleration = 1f;
			}
			else if (magnitude > 0f)
			{
				this.currentLookAcceleration = Mathf.Clamp(this.currentLookAcceleration + this.lookAccelerationRate * magnitude * Time.unscaledDeltaTime, 0f, magnitude);
			}
			else
			{
				this.currentLookAcceleration = 0f;
			}
			Vector2 a = this.controllerLookSensitivity;
			if (this.entityPlayerLocal.AimingGun)
			{
				a *= this.controllerZoomSensitivity;
			}
			else if (playerInput.VehicleActions.Enabled)
			{
				a *= this.controllerVehicleSensitivity;
			}
			vector2 = a * this.lookAccelerationCurve.Evaluate(this.currentLookAcceleration);
			if (this.entityPlayerLocal.AimingGun)
			{
				float d = Mathf.Lerp(0.2f, 1f, (this.entityPlayerLocal.playerCamera.fieldOfView - 10f) / ((float)Constants.cDefaultCameraFieldOfView - 10f));
				vector2 *= d;
			}
			if (this.entityPlayerLocal.AttachedToEntity != null)
			{
				this.aimAssistSlowAmount = 1f;
			}
			else
			{
				bool flag5 = false;
				WorldRayHitInfo hitInfo = this.entityPlayerLocal.HitInfo;
				if (hitInfo.bHitValid)
				{
					if (hitInfo.transform)
					{
						Transform hitRootTransform = GameUtils.GetHitRootTransform(hitInfo.tag, hitInfo.transform);
						if (hitRootTransform != null)
						{
							EntityAlive entityAlive;
							EntityItem entityItem;
							if (hitRootTransform.TryGetComponent<EntityAlive>(out entityAlive) && entityAlive.IsAlive() && entityAlive.IsValidAimAssistSlowdownTarget && hitInfo.hit.distanceSq <= 50f && (this.entityPlayerLocal.inventory.holdingItem.Actions[0] is ItemActionAttack || this.entityPlayerLocal.inventory.holdingItem.Actions[0] is ItemActionDynamicMelee))
							{
								this.bAimAssistTargetingItem = false;
								flag5 = true;
							}
							else if ((hitInfo.tag.StartsWith("Item", StringComparison.Ordinal) || hitRootTransform.TryGetComponent<EntityItem>(out entityItem)) && hitInfo.hit.distanceSq <= 10f)
							{
								this.bAimAssistTargetingItem = true;
								flag5 = true;
							}
						}
					}
					else if (this.entityPlayerLocal.ThreatLevel.Numeric < 0.75f && GameUtils.IsBlockOrTerrain(hitInfo.tag) && this.entityPlayerLocal.PlayerUI.windowManager.IsWindowOpen("interactionPrompt"))
					{
						BlockValue blockValue = hitInfo.hit.blockValue;
						if (!blockValue.Block.isMultiBlock && !blockValue.Block.isOversized && blockValue.Block.shape is BlockShapeModelEntity)
						{
							this.bAimAssistTargetingItem = true;
							flag5 = true;
						}
					}
				}
				if (flag5)
				{
					this.aimAssistSlowAmount = (this.bAimAssistTargetingItem ? 0.6f : 0.5f);
				}
				else
				{
					this.aimAssistSlowAmount = Mathf.MoveTowards(this.aimAssistSlowAmount, 1f, Time.unscaledDeltaTime * 5f);
				}
				vector2 *= this.aimAssistSlowAmount;
				if (this.controllerAimAssistsEnabled && this.cameraSnapTargetEntity != null && this.cameraSnapTargetEntity.IsAlive() && Time.time - this.cameraSnapTime < 0.3f)
				{
					Vector2 b = Vector2.one * 0.5f;
					Vector2 vector3 = (this.snapTargetingHead ? this.entityPlayerLocal.playerCamera.WorldToViewportPoint(this.cameraSnapTargetEntity.emodel.GetHeadTransform().position) : this.entityPlayerLocal.playerCamera.WorldToViewportPoint(this.cameraSnapTargetEntity.GetChestTransformPosition())) - b;
					float d2 = (this.cameraSnapMode == eCameraSnapMode.MeleeAttack) ? 1.5f : 1f;
					vector += vector3.normalized * d2 * vector3.magnitude / 0.15f;
				}
			}
		}
		else
		{
			vector2 = this.mouseLookSensitivity;
			Vector2 a2 = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (float)this.invertMouse);
			vector = (a2 + this.previousMouseInput) / 2f;
			this.previousMouseInput = a2;
			if (playerInput.VehicleActions.Enabled)
			{
				vector2 *= this.vehicleLookSensitivity;
			}
			else
			{
				float magnitude2 = vector.magnitude;
				float num2 = 1f;
				if (this.entityPlayerLocal.AimingGun && magnitude2 > 0f)
				{
					vector2 *= this.mouseZoomSensitivity;
					float num3 = Mathf.Pow(magnitude2 * 0.4f, 2.5f) / magnitude2;
					num2 += num3 * this.zoomAccel;
					num2 *= Mathf.Lerp(0.2f, 1f, (this.entityPlayerLocal.playerCamera.fieldOfView - 10f) / ((float)Constants.cDefaultCameraFieldOfView - 10f));
					vector2 *= num2;
					if (vector2.magnitude > this.mouseLookSensitivity.magnitude)
					{
						vector2 = this.mouseLookSensitivity;
					}
				}
			}
			if (this.skipMouseLookNextFrame > 0 && (vector.x <= -1f || vector.x >= 1f || vector.y <= -1f || vector.y >= 1f))
			{
				this.skipMouseLookNextFrame--;
				vector = Vector2.zero;
			}
		}
		MovementInput movementInput = this.entityPlayerLocal.movementInput;
		if (!movementInput.bDetachedCameraMove)
		{
			PlayerActionsLocal playerActionsLocal = playerInput;
			if (this.playerAutoPilotControllor != null && this.playerAutoPilotControllor.IsEnabled())
			{
				movementInput.moveForward = this.playerAutoPilotControllor.GetForwardMovement();
			}
			else
			{
				movementInput.moveForward = playerActionsLocal.Move.Y;
				if (this.isAutorun)
				{
					movementInput.moveForward = 1f;
				}
			}
			movementInput.moveStrafe = playerActionsLocal.Move.X;
			if (movementInput.bCameraPositionLocked)
			{
				vector = Vector2.zero;
			}
			float num4 = Utils.FastMin(0.033333335f, Time.unscaledDeltaTime);
			if (PlayerMoveController.useScaledMouseLook && !this.entityPlayerLocal.movementInput.lastInputController)
			{
				float num5 = num4 * PlayerMoveController.mouseDeltaTimeScale;
				MovementInput movementInput2 = movementInput;
				movementInput2.rotation.x = movementInput2.rotation.x + vector.y * vector2.y * num5;
				MovementInput movementInput3 = movementInput;
				movementInput3.rotation.y = movementInput3.rotation.y + vector.x * vector2.x * num5;
			}
			else if (this.entityPlayerLocal.movementInput.lastInputController)
			{
				float num6 = num4 * PlayerMoveController.lookDeltaTimeScale;
				MovementInput movementInput4 = movementInput;
				movementInput4.rotation.x = movementInput4.rotation.x + vector.y * vector2.y * num6;
				MovementInput movementInput5 = movementInput;
				movementInput5.rotation.y = movementInput5.rotation.y + vector.x * vector2.x * num6;
			}
			else
			{
				MovementInput movementInput6 = movementInput;
				movementInput6.rotation.x = movementInput6.rotation.x + vector.y * vector2.y;
				MovementInput movementInput7 = movementInput;
				movementInput7.rotation.y = movementInput7.rotation.y + vector.x * vector2.x;
			}
			bool value = this.entityPlayerLocal.IsGodMode.Value;
			movementInput.bCameraChange = (playerActionsLocal.CameraChange.IsPressed && !value && !playerActionsLocal.Primary.IsPressed && !playerActionsLocal.Secondary.IsPressed);
			if (movementInput.bCameraChange)
			{
				flag2 = true;
				if (this.entityPlayerLocal.bFirstPersonView)
				{
					if (num < 0f)
					{
						this.entityPlayerLocal.SwitchFirstPersonViewFromInput();
						this.wasCameraChangeUsedWithWheel = true;
					}
				}
				else
				{
					movementInput.cameraDistance = Utils.FastMin(movementInput.cameraDistance - 2f * num, 3f);
					if (movementInput.cameraDistance < -0.2f)
					{
						movementInput.cameraDistance = -0.2f;
						this.entityPlayerLocal.SwitchFirstPersonViewFromInput();
					}
					if (num != 0f)
					{
						this.wasCameraChangeUsedWithWheel = true;
					}
				}
			}
			if (playerActionsLocal.CameraChange.WasReleased && !value)
			{
				if (!this.wasCameraChangeUsedWithWheel && !playerActionsLocal.Primary.IsPressed && !playerActionsLocal.Secondary.IsPressed)
				{
					this.entityPlayerLocal.SwitchFirstPersonViewFromInput();
				}
				this.wasCameraChangeUsedWithWheel = false;
			}
			if ((this.gameManager.IsEditMode() || BlockToolSelection.Instance.SelectionActive) && (Input.GetKey(KeyCode.LeftControl) || GamePrefs.GetInt(EnumGamePrefs.SelectionOperationMode) != 0))
			{
				movementInput.Clear();
			}
			this.entityPlayerLocal.MoveByInput();
		}
		else
		{
			float num7 = 0.15f;
			if (this.entityPlayerLocal.movementInput.running)
			{
				num7 *= 3f;
			}
			else
			{
				num7 *= this.entityPlayerLocal.GodModeSpeedModifier;
			}
			if (playerInput.MoveForward.IsPressed)
			{
				this.entityPlayerLocal.cameraTransform.position += this.entityPlayerLocal.cameraTransform.forward * num7;
			}
			if (playerInput.MoveBack.IsPressed)
			{
				this.entityPlayerLocal.cameraTransform.position -= this.entityPlayerLocal.cameraTransform.forward * num7;
			}
			if (playerInput.MoveLeft.IsPressed)
			{
				this.entityPlayerLocal.cameraTransform.position -= this.entityPlayerLocal.cameraTransform.right * num7;
			}
			if (playerInput.MoveRight.IsPressed)
			{
				this.entityPlayerLocal.cameraTransform.position += this.entityPlayerLocal.cameraTransform.right * num7;
			}
			if (playerInput.Jump.IsPressed)
			{
				this.entityPlayerLocal.cameraTransform.position += Vector3.up * num7;
			}
			if (playerInput.Crouch.IsPressed)
			{
				this.entityPlayerLocal.cameraTransform.position -= Vector3.up * num7;
			}
			if (!movementInput.bCameraPositionLocked)
			{
				Vector3 localEulerAngles = this.entityPlayerLocal.cameraTransform.localEulerAngles;
				this.entityPlayerLocal.cameraTransform.localEulerAngles = new Vector3(localEulerAngles.x - vector.y, localEulerAngles.y + vector.x, localEulerAngles.z);
			}
		}
		bool flag6 = this.gameManager.IsEditMode() && playerInput.Run.IsPressed;
		Ray ray = this.entityPlayerLocal.GetLookRay();
		if (this.gameManager.IsEditMode() && GamePrefs.GetInt(EnumGamePrefs.SelectionOperationMode) == 4)
		{
			ray = this.entityPlayerLocal.cameraTransform.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			ray.origin += Origin.position;
		}
		ray.origin += ray.direction.normalized * 0.1f;
		float num8 = Utils.FastMax(Utils.FastMax(Constants.cDigAndBuildDistance, Constants.cCollectItemDistance), 30f);
		RaycastHit raycastHit;
		bool flag7 = Physics.Raycast(new Ray(ray.origin - Origin.position, ray.direction), out raycastHit, num8, 73728);
		bool flag8 = false;
		if (flag7 && raycastHit.transform.CompareTag("E_BP_Body"))
		{
			flag8 = true;
		}
		if (flag7)
		{
			flag7 &= raycastHit.transform.CompareTag("Item");
		}
		int num9 = 69;
		bool flag9;
		if (!this.gameManager.IsEditMode())
		{
			flag9 = Voxel.Raycast(this.gameManager.World, ray, num8, -555528213, num9, 0f);
			if (flag9)
			{
				Transform hitRootTransform2 = GameUtils.GetHitRootTransform(Voxel.voxelRayHitInfo.tag, Voxel.voxelRayHitInfo.transform);
				Entity entity;
				EntityAlive entityAlive2 = (hitRootTransform2 != null && hitRootTransform2.TryGetComponent<Entity>(out entity)) ? (entity as EntityAlive) : null;
				if (entityAlive2 == null || !entityAlive2.IsDead())
				{
					flag9 = Voxel.Raycast(this.gameManager.World, ray, num8, -555266069, num9, 0f);
				}
			}
		}
		else
		{
			num9 |= 256;
			int num10 = -555266069;
			num10 |= 268435456;
			if (!GameManager.bVolumeBlocksEditing)
			{
				num10 = int.MinValue;
			}
			flag9 = Voxel.RaycastOnVoxels(this.gameManager.World, ray, num8, num10, num9, 0f);
			if (flag9 && !GameManager.bVolumeBlocksEditing)
			{
				Voxel.voxelRayHitInfo.lastBlockPos = Vector3i.zero;
				Voxel.voxelRayHitInfo.hit.voxelData.Clear();
				Voxel.voxelRayHitInfo.hit.blockPos = Vector3i.zero;
			}
		}
		WorldRayHitInfo hitInfo2 = this.entityPlayerLocal.HitInfo;
		Vector3i zero = Vector3i.zero;
		Vector3i vector3i = Vector3i.zero;
		if (flag9)
		{
			hitInfo2.CopyFrom(Voxel.voxelRayHitInfo);
			vector3i = hitInfo2.hit.blockPos;
			Vector3i lastBlockPos = hitInfo2.lastBlockPos;
			hitInfo2.bHitValid = true;
		}
		else
		{
			hitInfo2.bHitValid = false;
		}
		if (!hitInfo2.hit.blockValue.isair)
		{
			Block block = hitInfo2.hit.blockValue.Block;
			if (!block.IsCollideMovement || block.CanBlocksReplace)
			{
				hitInfo2.lastBlockPos = vector3i;
			}
		}
		float num11 = flag7 ? raycastHit.distance : 1000f;
		if (flag9 && GameUtils.IsBlockOrTerrain(hitInfo2.tag))
		{
			num11 -= 1.2f;
			if (num11 < 0f)
			{
				num11 = 0.1f;
			}
		}
		if (flag7 && (!flag9 || (flag9 && num11 * num11 <= hitInfo2.hit.distanceSq)))
		{
			hitInfo2.bHitValid = true;
			hitInfo2.tag = "Item";
			hitInfo2.transform = raycastHit.collider.transform;
			hitInfo2.hit.pos = raycastHit.point;
			hitInfo2.hit.blockPos = World.worldToBlockPos(hitInfo2.hit.pos);
			hitInfo2.hit.distanceSq = raycastHit.distance * raycastHit.distance;
		}
		if (flag8 && raycastHit.distance * raycastHit.distance <= hitInfo2.hit.distanceSq)
		{
			hitInfo2.bHitValid = true;
			hitInfo2.tag = "E_BP_Body";
			hitInfo2.transform = raycastHit.collider.transform;
			hitInfo2.hit.pos = raycastHit.point;
			hitInfo2.hit.blockPos = World.worldToBlockPos(hitInfo2.hit.pos);
			hitInfo2.hit.distanceSq = raycastHit.distance * raycastHit.distance;
		}
		bool flag10 = true;
		EntityCollisionRules entityCollisionRules;
		if (hitInfo2.hitCollider && hitInfo2.hitCollider.TryGetComponent<EntityCollisionRules>(out entityCollisionRules) && !entityCollisionRules.IsInteractable)
		{
			flag10 = false;
		}
		if (this.entityPlayerLocal.inventory != null && this.entityPlayerLocal.inventory.holdingItemData != null)
		{
			this.entityPlayerLocal.inventory.holdingItemData.hitInfo = this.entityPlayerLocal.HitInfo;
		}
		TileEntity tileEntity = null;
		EntityTurret entityTurret = null;
		bool flag11 = true;
		bool flag12 = true;
		bool flag13 = playerInput.Primary.IsPressed && this.bAllowPlayerInput && !this.IsGUICancelPressed;
		bool flag14 = playerInput.Secondary.IsPressed && this.bAllowPlayerInput && !this.IsGUICancelPressed;
		if (flag13 && GameManager.Instance.World.IsEditor())
		{
			if (this.bIgnoreLeftMouseUntilReleased)
			{
				flag13 = false;
			}
		}
		else
		{
			this.bIgnoreLeftMouseUntilReleased = false;
		}
		bool flag15 = false;
		ITileEntityLootable tileEntityLootable = null;
		EntityItem entityItem2 = null;
		BlockValue blockValue2 = BlockValue.Air;
		ProjectileMoveScript projectileMoveScript = null;
		ThrownWeaponMoveScript thrownWeaponMoveScript = null;
		string text = null;
		bool flag16 = GameManager.Instance.IsEditMode() && this.entityPlayerLocal.HitInfo.transform != null && this.entityPlayerLocal.HitInfo.transform.gameObject.layer == 28;
		Entity entity2 = null;
		if (this.entityPlayerLocal.AttachedToEntity == null && flag10)
		{
			if (hitInfo2.bHitValid && (flag16 |= GameUtils.IsBlockOrTerrain(hitInfo2.tag)))
			{
				int activationDistanceSq = hitInfo2.hit.blockValue.Block.GetActivationDistanceSq();
				if (hitInfo2.hit.distanceSq < (float)activationDistanceSq)
				{
					blockValue2 = hitInfo2.hit.blockValue;
					Block block2 = blockValue2.Block;
					BlockValue blockValue3 = blockValue2;
					Vector3i vector3i2 = vector3i;
					if (blockValue3.ischild && block2 != null && block2.multiBlockPos != null)
					{
						vector3i2 = block2.multiBlockPos.GetParentPos(vector3i2, blockValue3);
						blockValue3 = this.gameManager.World.GetBlock(hitInfo2.hit.clrIdx, vector3i2);
					}
					if (block2.HasBlockActivationCommands(this.gameManager.World, blockValue3, hitInfo2.hit.clrIdx, vector3i2, this.entityPlayerLocal))
					{
						text = block2.GetActivationText(this.gameManager.World, blockValue3, hitInfo2.hit.clrIdx, vector3i2, this.entityPlayerLocal);
						if (text != null)
						{
							string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
							text = string.Format(text, arg);
						}
						else if (block2.CustomCmds.Length != 0)
						{
							PlayerActionsLocal playerInput2 = this.entityPlayerLocal.playerInput;
							string arg2 = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
							string localizedBlockName = blockValue3.Block.GetLocalizedBlockName();
							text = string.Format(Localization.Get("questBlockActivate", false), arg2, localizedBlockName);
							text = string.Format(text, arg2);
						}
						tileEntity = this.gameManager.World.GetTileEntity(hitInfo2.hit.clrIdx, vector3i);
					}
					else if (block2.DisplayInfo == Block.EnumDisplayInfo.Name)
					{
						text = block2.GetLocalizedBlockName();
					}
					else if (block2.DisplayInfo == Block.EnumDisplayInfo.Description)
					{
						text = Localization.Get(block2.DescriptionKey, false);
					}
					else if (block2.DisplayInfo == Block.EnumDisplayInfo.Custom)
					{
						text = block2.GetCustomDescription(vector3i2, blockValue2);
					}
					if (this.gameManager.IsEditMode() && flag14 && InputUtils.ShiftKeyPressed && InputUtils.ControlKeyPressed && blockValue2.Block is BlockSpawnEntity)
					{
						this.windowManager.GetWindow<GUIWindowEditBlockSpawnEntity>(GUIWindowEditBlockSpawnEntity.ID).SetBlockValue(hitInfo2.hit.blockPos, blockValue2);
						this.windowManager.Open(GUIWindowEditBlockSpawnEntity.ID, true, false, true);
						flag11 = false;
					}
				}
			}
			else if (hitInfo2.bHitValid && hitInfo2.tag.Equals("Item") && hitInfo2.hit.distanceSq < Constants.cCollectItemDistance * Constants.cCollectItemDistance)
			{
				entityItem2 = hitInfo2.transform.GetComponent<EntityItem>();
				RootTransformRefEntity component;
				if (entityItem2 == null && (component = hitInfo2.transform.GetComponent<RootTransformRefEntity>()) != null && component.RootTransform != null)
				{
					entityItem2 = component.RootTransform.GetComponent<EntityItem>();
				}
				if (entityItem2 != null)
				{
					if (entityItem2.onGround && entityItem2.CanCollect())
					{
						string localizedItemName = ItemClass.GetForId(entityItem2.itemStack.itemValue.type).GetLocalizedItemName();
						string arg3 = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
						if (entityItem2.itemStack.count > 1)
						{
							text = string.Format(Localization.Get("itemTooltipFocusedSeveral", false), arg3, localizedItemName, entityItem2.itemStack.count);
						}
						else
						{
							text = string.Format(Localization.Get("itemTooltipFocusedOne", false), arg3, localizedItemName);
						}
					}
				}
				else
				{
					projectileMoveScript = hitInfo2.transform.GetComponent<ProjectileMoveScript>();
					if (projectileMoveScript != null)
					{
						string localizedItemName2 = ItemClass.GetForId(projectileMoveScript.itemValueProjectile.type).GetLocalizedItemName();
						string arg4 = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
						text = string.Format(Localization.Get("itemTooltipFocusedOne", false), arg4, localizedItemName2);
					}
					thrownWeaponMoveScript = hitInfo2.transform.GetComponent<ThrownWeaponMoveScript>();
					if (thrownWeaponMoveScript != null)
					{
						string localizedItemName3 = ItemClass.GetForId(thrownWeaponMoveScript.itemValueWeapon.type).GetLocalizedItemName();
						string arg5 = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
						text = string.Format(Localization.Get("itemTooltipFocusedOne", false), arg5, localizedItemName3);
					}
				}
			}
			else if (hitInfo2.bHitValid && hitInfo2.tag.StartsWith("E_") && hitInfo2.hit.distanceSq < Constants.cCollectItemDistance * Constants.cCollectItemDistance)
			{
				Transform hitRootTransform3 = GameUtils.GetHitRootTransform(hitInfo2.tag, hitInfo2.transform);
				if (hitRootTransform3 != null && (entity2 = hitRootTransform3.GetComponent<Entity>()) != null)
				{
					if ((projectileMoveScript = hitRootTransform3.GetComponentInChildren<ProjectileMoveScript>()) != null)
					{
						if (!entity2.IsDead() && entity2 as EntityPlayer != null && (entity2 as EntityPlayer).inventory != null && (entity2 as EntityPlayer).inventory.holdingItem != null && (entity2 as EntityPlayer).inventory.holdingItem.HasAnyTags(PlayerMoveController.BowTag))
						{
							projectileMoveScript = null;
						}
						if (entity2.IsDead())
						{
							string localizedItemName4 = ItemClass.GetForId(projectileMoveScript.itemValueProjectile.type).GetLocalizedItemName();
							string arg6 = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
							text = string.Format(Localization.Get("itemTooltipFocusedOne", false), arg6, localizedItemName4);
						}
					}
					else if ((thrownWeaponMoveScript = hitRootTransform3.GetComponentInChildren<ThrownWeaponMoveScript>()) != null)
					{
						if (!entity2.IsDead() && entity2 as EntityPlayer != null && (entity2 as EntityPlayer).inventory != null && (entity2 as EntityPlayer).inventory.holdingItem != null && (entity2 as EntityPlayer).inventory.holdingItem.HasAnyTags(PlayerMoveController.BowTag))
						{
							thrownWeaponMoveScript = null;
						}
						if (entity2.IsDead())
						{
							string localizedItemName5 = ItemClass.GetForId(thrownWeaponMoveScript.itemValueWeapon.type).GetLocalizedItemName();
							string arg7 = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
							text = string.Format(Localization.Get("itemTooltipFocusedOne", false), arg7, localizedItemName5);
						}
					}
					else if (entity2 is EntityNPC && entity2.IsAlive())
					{
						tileEntity = (this.gameManager.World.GetTileEntity(entity2.entityId) as TileEntityTrader);
						if (tileEntity != null)
						{
							EntityTrader entityTrader = (EntityTrader)entity2;
							string arg8 = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
							string localizedEntityName = entityTrader.LocalizedEntityName;
							text = string.Format(Localization.Get("npcTooltipTalk", false), arg8, localizedEntityName);
							entityTrader.HandleClientQuests(this.entityPlayerLocal);
						}
						else
						{
							tileEntity = this.gameManager.World.GetTileEntity(entity2.entityId);
							if (tileEntity != null)
							{
								string arg9 = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
								string localizedEntityName2 = entity2.LocalizedEntityName;
								text = string.Format(Localization.Get("npcTooltipTalk", false), arg9, localizedEntityName2);
								EntityDrone entityDrone = entity2 as EntityDrone;
								if (entityDrone && entityDrone.IsLocked() && !entityDrone.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier))
								{
									text = Localization.Get("ttLocked", false) + "\n" + text;
								}
							}
						}
					}
					else if (entity2 as EntityTurret != null)
					{
						entityTurret = (entity2 as EntityTurret);
						if (entityTurret.CanInteract(this.entityPlayerLocal.entityId))
						{
							string arg10 = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
							string localizedEntityName3 = entity2.LocalizedEntityName;
							text = string.Format(Localization.Get("turretPickUp", false), arg10, localizedEntityName3);
						}
					}
					else if (!string.IsNullOrEmpty(entity2.GetLootList()))
					{
						tileEntityLootable = this.gameManager.World.GetTileEntity(entity2.entityId).GetSelfOrFeature<ITileEntityLootable>();
						if (tileEntityLootable != null)
						{
							string localizedEntityName4 = entity2.LocalizedEntityName;
							string arg11 = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
							string text2 = localizedEntityName4;
							if (entity2 is EntityNPC && entity2.IsAlive())
							{
								text = string.Format(Localization.Get("npcTooltipTalk", false), arg11, text2);
								EntityDrone entityDrone2 = entity2 as EntityDrone;
								if (entityDrone2 && entityDrone2.IsLocked() && !entityDrone2.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier))
								{
									text = Localization.Get("ttLocked", false) + "\n" + text;
								}
							}
							else if (entity2 is EntityDriveable && entity2.IsAlive())
							{
								text = string.Format(Localization.Get("tooltipInteract", false), arg11, Localization.Get(text2, false));
								if (((EntityDriveable)entity2).IsLockedForLocalPlayer(this.entityPlayerLocal))
								{
									text = Localization.Get("ttLocked", false) + "\n" + text;
								}
							}
							else if (!tileEntityLootable.bTouched)
							{
								text = string.Format(Localization.Get("lootTooltipNew", false), arg11, text2);
							}
							else if (tileEntityLootable.IsEmpty())
							{
								text = string.Format(Localization.Get("lootTooltipEmpty", false), arg11, text2);
							}
							else
							{
								text = string.Format(Localization.Get("lootTooltipTouched", false), arg11, text2);
							}
						}
					}
				}
			}
			if (text == null)
			{
				this.InteractName = null;
				if (this.entityPlayerLocal.IsMoveStateStill() && (!this.entityPlayerLocal.IsSwimming() || this.entityPlayerLocal.cameraTransform.up.y < 0.7f))
				{
					this.InteractName = this.entityPlayerLocal.inventory.CanInteract();
					if (this.InteractName != null && this.InteractWaitTime == 0f)
					{
						this.InteractWaitTime = Time.time + 0.3f;
					}
				}
				if (this.InteractName != null)
				{
					if (Time.time >= this.InteractWaitTime)
					{
						flag15 = true;
						string arg12 = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
						text = string.Format(Localization.Get("ttPressTo", false), arg12, Localization.Get(this.InteractName, false));
					}
				}
				else
				{
					this.InteractWaitTime = 0f;
				}
			}
			else
			{
				this.InteractWaitTime = 0f;
			}
		}
		if (this.entityPlayerLocal.IsAlive())
		{
			if (!string.Equals(text, this.strTextLabelPointingTo) && (Time.time - this.timeActivatePressed > 0.5f || string.IsNullOrEmpty(text)))
			{
				XUiC_InteractionPrompt.SetText(this.playerUI, text);
				this.strTextLabelPointingTo = text;
			}
		}
		else
		{
			text = "";
			this.strTextLabelPointingTo = text;
			XUiC_InteractionPrompt.SetText(this.playerUI, null);
		}
		this.FocusBoxPosition = hitInfo2.lastBlockPos;
		if (flag6 || (this.entityPlayerLocal.inventory != null && this.entityPlayerLocal.inventory.holdingItem.IsFocusBlockInside()))
		{
			this.FocusBoxPosition = vector3i;
		}
		this.focusBoxScript.Update(flag16, this.gameManager.World, hitInfo2, this.FocusBoxPosition, this.entityPlayerLocal, this.gameManager.persistentLocalPlayer, flag6);
		if (!this.windowManager.IsInputActive() && !this.windowManager.IsFullHUDDisabled() && (playerInput.Activate.IsPressed || playerInput.VehicleActions.Activate.IsPressed || playerInput.PermanentActions.Activate.IsPressed))
		{
			if (playerInput.Activate.WasPressed || playerInput.VehicleActions.Activate.WasPressed || playerInput.PermanentActions.Activate.WasPressed)
			{
				this.timeActivatePressed = Time.time;
				if (flag15 && hitInfo2.bHitValid && GameUtils.IsBlockOrTerrain(hitInfo2.tag))
				{
					blockValue2 = BlockValue.Air;
				}
				if (this.entityPlayerLocal.AttachedToEntity != null)
				{
					this.entityPlayerLocal.SendDetach();
				}
				else if (entityTurret != null || projectileMoveScript != null || thrownWeaponMoveScript != null || entityItem2 != null || !blockValue2.isair || tileEntityLootable != null || tileEntity != null)
				{
					BlockValue blockValue4 = blockValue2;
					Vector3i vector3i3 = vector3i;
					if (blockValue4.ischild)
					{
						vector3i3 = blockValue4.Block.multiBlockPos.GetParentPos(vector3i3, blockValue4);
						blockValue4 = this.gameManager.World.GetBlock(hitInfo2.hit.clrIdx, vector3i3);
					}
					if (!blockValue4.Equals(BlockValue.Air) && blockValue4.Block.HasBlockActivationCommands(this.gameManager.World, blockValue4, hitInfo2.hit.clrIdx, vector3i3, this.entityPlayerLocal))
					{
						this.playerUI.xui.RadialWindow.Open();
						this.playerUI.xui.RadialWindow.SetCurrentBlockData(this.gameManager.World, vector3i3, hitInfo2.hit.clrIdx, blockValue4, this.entityPlayerLocal);
						flag11 = true;
					}
					else if (tileEntityLootable != null && entity2.GetActivationCommands(tileEntityLootable.ToWorldPos(), this.entityPlayerLocal).Length != 0)
					{
						this.entityPlayerLocal.AimingGun = false;
						tileEntityLootable.bWasTouched = tileEntityLootable.bTouched;
						this.playerUI.xui.RadialWindow.Open();
						this.playerUI.xui.RadialWindow.SetCurrentEntityData(this.gameManager.World, entity2, tileEntityLootable, this.entityPlayerLocal);
						flag11 = true;
					}
					else if (tileEntity != null && entity2.GetActivationCommands(tileEntity.ToWorldPos(), this.entityPlayerLocal).Length != 0)
					{
						this.entityPlayerLocal.AimingGun = false;
						this.playerUI.xui.RadialWindow.Open();
						this.playerUI.xui.RadialWindow.SetCurrentEntityData(this.gameManager.World, entity2, tileEntity, this.entityPlayerLocal);
						flag11 = true;
					}
					else if (entityTurret != null)
					{
						if (entityTurret.CanInteract(this.entityPlayerLocal.entityId))
						{
							ItemStack itemStack = new ItemStack(entityTurret.OriginalItemValue, 1);
							if (this.entityPlayerLocal.inventory.CanTakeItem(itemStack) || this.entityPlayerLocal.bag.CanTakeItem(itemStack))
							{
								this.gameManager.CollectEntityServer(entityTurret.entityId, this.playerUI.entityPlayer.entityId);
							}
							else
							{
								GameManager.ShowTooltip(this.entityPlayerLocal, Localization.Get("xuiInventoryFullForPickup", false), string.Empty, "ui_denied", null, false, false, 0f);
							}
						}
					}
					else
					{
						this.windowManager.Close("radial");
						if (entityItem2 != null)
						{
							EntityItem entityItem3 = entityItem2;
							if (entityItem3 != null && entityItem3.CanCollect() && entityItem3.onGround)
							{
								if (this.entityPlayerLocal.inventory.CanTakeItem(entityItem3.itemStack) || this.entityPlayerLocal.bag.CanTakeItem(entityItem3.itemStack))
								{
									this.gameManager.CollectEntityServer(entityItem2.entityId, this.entityPlayerLocal.entityId);
								}
								else
								{
									GameManager.ShowTooltip(this.entityPlayerLocal, Localization.Get("xuiInventoryFullForPickup", false), string.Empty, "ui_denied", null, false, false, 0f);
								}
							}
						}
						else if (projectileMoveScript != null)
						{
							if (projectileMoveScript.itemProjectile.IsSticky)
							{
								ItemStack itemStack2 = new ItemStack(projectileMoveScript.itemValueProjectile, 1);
								if (this.entityPlayerLocal.inventory.CanTakeItem(itemStack2) || this.entityPlayerLocal.bag.CanTakeItem(itemStack2))
								{
									this.playerUI.xui.PlayerInventory.AddItem(itemStack2);
									projectileMoveScript.ProjectileID = -1;
									UnityEngine.Object.Destroy(projectileMoveScript.gameObject);
								}
								else
								{
									GameManager.ShowTooltip(this.entityPlayerLocal, Localization.Get("xuiInventoryFullForPickup", false), string.Empty, "ui_denied", null, false, false, 0f);
								}
							}
						}
						else if (thrownWeaponMoveScript != null)
						{
							if (thrownWeaponMoveScript.itemWeapon.IsSticky)
							{
								ItemStack itemStack3 = new ItemStack(thrownWeaponMoveScript.itemValueWeapon, 1);
								if (this.entityPlayerLocal.inventory.CanTakeItem(itemStack3) || this.entityPlayerLocal.bag.CanTakeItem(itemStack3))
								{
									this.playerUI.xui.PlayerInventory.AddItem(itemStack3);
									thrownWeaponMoveScript.ProjectileID = -1;
									UnityEngine.Object.Destroy(thrownWeaponMoveScript.gameObject);
								}
								else
								{
									GameManager.ShowTooltip(this.entityPlayerLocal, Localization.Get("xuiInventoryFullForPickup", false), string.Empty, "ui_denied", null, false, false, 0f);
								}
							}
						}
						else
						{
							this.suckItemsNearby(entityItem2);
						}
					}
				}
				else if (flag15)
				{
					this.entityPlayerLocal.inventory.Interact();
				}
			}
			else
			{
				this.windowManager.Close("radial");
				this.suckItemsNearby(entityItem2);
			}
		}
		if (this.gameManager.IsEditMode() && flag13 && flag12 && !playerInput.Drop.IsPressed)
		{
			WorldRayHitInfo other = Voxel.voxelRayHitInfo.Clone();
			num9 = 325;
			int minValue = int.MinValue;
			if (Voxel.RaycastOnVoxels(this.gameManager.World, ray, 250f, minValue, num9, 0f) && SelectionBoxManager.Instance.Select(Voxel.voxelRayHitInfo))
			{
				flag12 = false;
				this.bIgnoreLeftMouseUntilReleased = true;
			}
			Voxel.voxelRayHitInfo.CopyFrom(other);
		}
		if (flag13 && (GameManager.Instance.World.IsEditor() || BlockToolSelection.Instance.SelectionActive))
		{
			flag13 &= !playerInput.Drop.IsPressed;
		}
		int num12 = playerInput.InventorySlotWasPressed;
		if (num12 >= 0)
		{
			if (playerInput.LastInputType == BindingSourceType.DeviceBindingSource)
			{
				if (this.entityPlayerLocal.AimingGun)
				{
					num12 = -1;
				}
			}
			else if (InputUtils.ShiftKeyPressed && this.entityPlayerLocal.inventory.PUBLIC_SLOTS > this.entityPlayerLocal.inventory.SHIFT_KEY_SLOT_OFFSET)
			{
				num12 += this.entityPlayerLocal.inventory.SHIFT_KEY_SLOT_OFFSET;
			}
		}
		if (num12 == -1 && this.inventoryScrollPressed && this.inventoryScrollIdxToSelect != -1)
		{
			num12 = this.inventoryScrollIdxToSelect;
		}
		if (!flag2)
		{
			flag2 = this.entityPlayerLocal.inventory.holdingItem.ConsumeScrollWheel(this.entityPlayerLocal.inventory.holdingItemData, num, playerInput);
		}
		this.entityPlayerLocal.inventory.holdingItem.CheckKeys(this.entityPlayerLocal.inventory.holdingItemData, hitInfo2);
		ItemClass holdingItem = this.entityPlayerLocal.inventory.holdingItem;
		bool flag17 = (holdingItem.Actions[0] != null && holdingItem.Actions[0].AllowConcurrentActions()) || (holdingItem.Actions[1] != null && holdingItem.Actions[1].AllowConcurrentActions());
		bool flag18 = holdingItem.Actions[1] != null && holdingItem.Actions[1].IsActionRunning(this.entityPlayerLocal.inventory.holdingItemData.actionData[1]);
		bool flag19 = holdingItem.Actions[0] != null && holdingItem.Actions[0].IsActionRunning(this.entityPlayerLocal.inventory.holdingItemData.actionData[0]);
		if (flag12 && flag13 && !flag19 && (flag17 || !flag18) && this.entityPlayerLocal.inventory.GetIsFinishedSwitchingHeldItem())
		{
			if (this.gameManager.IsEditMode())
			{
				flag12 = !this.gameManager.GetActiveBlockTool().ExecuteAttackAction(this.entityPlayerLocal.inventory.holdingItemData, false, playerInput);
			}
			if (flag12)
			{
				this.entityPlayerLocal.inventory.Execute(0, false, playerInput);
			}
		}
		if (flag12 && playerInput.Primary.WasReleased && this.entityPlayerLocal.inventory.GetIsFinishedSwitchingHeldItem())
		{
			if (this.gameManager.IsEditMode() && !this.entityPlayerLocal.inventory.holdingItem.IsGun())
			{
				flag12 = !this.gameManager.GetActiveBlockTool().ExecuteAttackAction(this.entityPlayerLocal.inventory.holdingItemData, true, playerInput);
			}
			if (flag12)
			{
				this.entityPlayerLocal.inventory.Execute(0, true, playerInput);
			}
		}
		ItemAction itemAction = this.entityPlayerLocal.inventory.holdingItem.Actions[0];
		if (flag11 && flag14 && (flag17 || !flag19))
		{
			if (this.gameManager.IsEditMode())
			{
				flag11 = !this.gameManager.GetActiveBlockTool().ExecuteUseAction(this.entityPlayerLocal.inventory.holdingItemData, false, playerInput);
			}
			if (flag11)
			{
				this.entityPlayerLocal.inventory.Execute(1, false, playerInput);
			}
		}
		if (flag11 && playerInput.Secondary.WasReleased && this.entityPlayerLocal.inventory != null)
		{
			this.entityPlayerLocal.inventory.Execute(1, true, playerInput);
			if (this.gameManager.IsEditMode() && !this.entityPlayerLocal.inventory.holdingItem.IsGun())
			{
				this.gameManager.GetActiveBlockTool().ExecuteUseAction(this.entityPlayerLocal.inventory.holdingItemData, true, playerInput);
			}
		}
		if (playerInput.Drop.WasPressed && !this.gameManager.IsEditMode() && !BlockToolSelection.Instance.SelectionActive && this.entityPlayerLocal.inventory != null && !this.entityPlayerLocal.inventory.IsHoldingItemActionRunning() && !this.entityPlayerLocal.inventory.IsHolsterDelayActive() && !this.entityPlayerLocal.inventory.IsUnholsterDelayActive() && this.entityPlayerLocal.inventory.holdingItemIdx != this.entityPlayerLocal.inventory.DUMMY_SLOT_IDX && !this.entityPlayerLocal.AimingGun && num12 == -1 && !flag2)
		{
			Vector3 dropPosition = this.entityPlayerLocal.GetDropPosition();
			ItemValue holdingItemItemValue = this.entityPlayerLocal.inventory.holdingItemItemValue;
			if (ItemClass.GetForId(holdingItemItemValue.type).CanDrop(holdingItemItemValue) && this.entityPlayerLocal.inventory.holdingCount > 0 && this.entityPlayerLocal.DropTimeDelay <= 0f)
			{
				this.entityPlayerLocal.DropTimeDelay = 0.5f;
				int count = this.entityPlayerLocal.inventory.holdingItemStack.count;
				this.gameManager.ItemDropServer(this.entityPlayerLocal.inventory.holdingItemStack.Clone(), dropPosition, Vector3.zero, this.entityPlayerLocal.entityId, ItemClass.GetForId(holdingItemItemValue.type).GetLifetimeOnDrop(), false);
				this.entityPlayerLocal.AddUIHarvestingItem(new ItemStack(holdingItemItemValue, -count), false);
				Manager.BroadcastPlay(this.entityPlayerLocal, "itemdropped", false);
				this.entityPlayerLocal.inventory.DecHoldingItem(count);
			}
		}
		if (!playerInput.InventorySlotLeft.WasPressed && !playerInput.InventorySlotRight.WasPressed)
		{
			bool flag20 = this.inventoryScrollPressed;
		}
		this.inventoryScrollPressed = false;
		if (this.entityPlayerLocal.AttachedToEntity == null)
		{
			if (num12 != -1 && num12 != this.entityPlayerLocal.inventory.GetFocusedItemIdx() && num12 < this.entityPlayerLocal.inventory.PUBLIC_SLOTS && this.entityPlayerLocal.inventory != null && !this.entityPlayerLocal.CancellingInventoryActions)
			{
				if (this.canSwapHeldItem())
				{
					this.swapItem(this.entityPlayerLocal.inventory.SetFocusedItemIdx(num12));
				}
				else
				{
					this.entityPlayerLocal.inventory.SetFocusedItemIdx(num12);
					this.nextHeldItem.Add(num12);
				}
			}
			if ((playerInput.Reload.WasPressed || playerInput.PermanentActions.Reload.WasPressed) && this.entityPlayerLocal.inventory != null && this.entityPlayerLocal.inventory.GetIsFinishedSwitchingHeldItem())
			{
				bool flag21 = this.entityPlayerLocal.inventory.IsHoldingGun() || this.entityPlayerLocal.inventory.IsHoldingDynamicMelee();
				ItemAction holdingPrimary = this.entityPlayerLocal.inventory.GetHoldingPrimary();
				ItemAction holdingSecondary = this.entityPlayerLocal.inventory.GetHoldingSecondary();
				if (flag21 && holdingPrimary != null)
				{
					if (holdingPrimary.HasRadial())
					{
						this.timeActivatePressed = Time.time;
						this.playerUI.xui.RadialWindow.Open();
						holdingPrimary.SetupRadial(this.playerUI.xui.RadialWindow, this.entityPlayerLocal);
					}
					else
					{
						holdingPrimary.CancelAction(this.entityPlayerLocal.inventory.holdingItemData.actionData[0]);
						if (holdingSecondary != null && !(holdingSecondary is ItemActionSpawnTurret))
						{
							holdingSecondary.CancelAction(this.entityPlayerLocal.inventory.holdingItemData.actionData[1]);
						}
					}
				}
				else if (this.entityPlayerLocal.inventory.GetHoldingBlock() != null)
				{
					this.timeActivatePressed = Time.time;
					this.playerUI.xui.RadialWindow.Open();
					this.playerUI.xui.RadialWindow.SetupBlockShapeData();
				}
			}
			if (!flag2 && (playerInput.ToggleFlashlight.WasPressed || playerInput.PermanentActions.ToggleFlashlight.WasPressed))
			{
				this.timeActivatePressed = Time.time;
				this.playerUI.xui.RadialWindow.Open();
				this.playerUI.xui.RadialWindow.SetActivatableItemData(this.entityPlayerLocal);
			}
			if (!flag2 && (playerInput.Swap.WasPressed || playerInput.PermanentActions.Swap.WasPressed))
			{
				this.timeActivatePressed = Time.time;
				this.playerUI.xui.RadialWindow.Open();
				this.playerUI.xui.RadialWindow.SetupToolbeltMenu(0);
			}
			if (!flag2 && playerInput.InventorySlotRight.WasPressed)
			{
				this.timeActivatePressed = Time.time;
				this.playerUI.xui.RadialWindow.Open();
				this.playerUI.xui.RadialWindow.SetupToolbeltMenu(1);
			}
			if (!flag2 && playerInput.InventorySlotLeft.WasPressed)
			{
				this.timeActivatePressed = Time.time;
				this.playerUI.xui.RadialWindow.Open();
				this.playerUI.xui.RadialWindow.SetupToolbeltMenu(-1);
				return;
			}
		}
		else
		{
			EntityVehicle entityVehicle = this.entityPlayerLocal.AttachedToEntity as EntityVehicle;
			if (entityVehicle != null)
			{
				if (playerInput.PermanentActions.ToggleFlashlight.WasPressed || playerInput.VehicleActions.ToggleFlashlight.WasPressed)
				{
					if (entityVehicle.HasHeadlight())
					{
						entityVehicle.ToggleHeadlight();
					}
					else
					{
						this.timeActivatePressed = Time.time;
						this.playerUI.xui.RadialWindow.Open();
						this.playerUI.xui.RadialWindow.SetActivatableItemData(this.entityPlayerLocal);
					}
				}
				if (playerInput.VehicleActions.HonkHorn.WasPressed)
				{
					entityVehicle.UseHorn(this.entityPlayerLocal);
				}
			}
		}
	}

	// Token: 0x0600842B RID: 33835 RVA: 0x0035C9E4 File Offset: 0x0035ABE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void swapItem(int inventoryItemToSet)
	{
		this.entityPlayerLocal.AimingGun = false;
		this.entityPlayerLocal.inventory.BeginSwapHoldingItem();
		base.StartCoroutine(this.entityPlayerLocal.CancelInventoryActions(delegate
		{
			Inventory inventory = this.entityPlayerLocal.inventory;
			if (inventory != null)
			{
				this.playerInput.Primary.ClearInputState();
				inventory.SetHoldingItemIdx(inventoryItemToSet);
			}
		}, true));
	}

	// Token: 0x0600842C RID: 33836 RVA: 0x0035CA40 File Offset: 0x0035AC40
	[PublicizedFrom(EAccessModifier.Private)]
	public void suckItemsNearby(EntityItem focusedItem)
	{
		if (!this.countdownSuckItemsNearby.HasPassed())
		{
			return;
		}
		this.countdownSuckItemsNearby.ResetAndRestart();
		if (!this.entityPlayerLocal.addedToChunk)
		{
			return;
		}
		int num = this.entityPlayerLocal.GetBlockPosition().y >> 4;
		for (int i = this.entityPlayerLocal.chunkPosAddedEntityTo.x - 1; i <= this.entityPlayerLocal.chunkPosAddedEntityTo.x + 1; i++)
		{
			for (int j = this.entityPlayerLocal.chunkPosAddedEntityTo.z - 1; j <= this.entityPlayerLocal.chunkPosAddedEntityTo.z + 1; j++)
			{
				Chunk chunk = (Chunk)this.gameManager.World.GetChunkSync(i, j);
				if (chunk != null)
				{
					int num2 = Utils.FastMax(num - 1, 0);
					int num3 = Utils.FastMin(num + 1, chunk.entityLists.Length - 1);
					for (int k = num2; k <= num3; k++)
					{
						int num4 = 0;
						while (chunk.entityLists[k] != null && num4 < chunk.entityLists[k].Count)
						{
							if (chunk.entityLists[k][num4] is EntityItem)
							{
								EntityItem entityItem = (EntityItem)chunk.entityLists[k][num4];
								if (entityItem.CanCollect())
								{
									Vector3 velToAdd = this.entityPlayerLocal.getHeadPosition() - chunk.entityLists[k][num4].GetPosition();
									if (velToAdd.sqrMagnitude <= 16f && (this.entityPlayerLocal.inventory.CanTakeItem(entityItem.itemStack) || this.entityPlayerLocal.bag.CanTakeItem(entityItem.itemStack)))
									{
										if (velToAdd.sqrMagnitude < 4f)
										{
											if (focusedItem != null && focusedItem.onGround)
											{
												this.gameManager.CollectEntityServer(focusedItem.entityId, this.entityPlayerLocal.entityId);
											}
										}
										else
										{
											velToAdd.Normalize();
											velToAdd.x *= 0.7f;
											velToAdd.y *= 1.5f;
											velToAdd.z *= 0.7f;
											this.gameManager.AddVelocityToEntityServer(chunk.entityLists[k][num4].entityId, velToAdd);
										}
									}
								}
							}
							num4++;
						}
					}
				}
			}
		}
	}

	// Token: 0x0600842D RID: 33837 RVA: 0x0035CCB2 File Offset: 0x0035AEB2
	public void SkipMouseLookNextFrame()
	{
		this.skipMouseLookNextFrame = 3;
	}

	// Token: 0x0600842E RID: 33838 RVA: 0x0035CCBB File Offset: 0x0035AEBB
	public void SetControllableOverride(bool _b)
	{
		this.bCanControlOverride = _b;
	}

	// Token: 0x0600842F RID: 33839 RVA: 0x0035CCC4 File Offset: 0x0035AEC4
	public void Respawn(RespawnType _type)
	{
		this.gameManager.World.GetPrimaryPlayer().Spawned = false;
		this.respawnReason = _type;
		switch (_type)
		{
		case RespawnType.NewGame:
			this.respawnTime = Constants.cRespawnEnterGameTime;
			return;
		case RespawnType.LoadedGame:
			this.respawnTime = 0f;
			return;
		case RespawnType.Died:
			this.respawnTime = Constants.cRespawnAfterDeathTime;
			return;
		default:
			return;
		}
	}

	// Token: 0x06008430 RID: 33840 RVA: 0x0035CD24 File Offset: 0x0035AF24
	public void AllowPlayerInput(bool allow)
	{
		this.bAllowPlayerInput = allow;
	}

	// Token: 0x06008431 RID: 33841 RVA: 0x0035CD30 File Offset: 0x0035AF30
	[PublicizedFrom(EAccessModifier.Private)]
	public void DrawChunkBoundary()
	{
		Vector3i vector3i = World.toChunkXYZCube(this.entityPlayerLocal.position);
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				for (int k = -1; k <= 1; k++)
				{
					string name = string.Format("PlayerChunk{0},{1},{2}", k, i, j);
					Vector3 vector;
					vector.x = (float)((vector3i.x + k) * 16);
					vector.y = (float)((vector3i.y + i) * 16);
					vector.z = (float)((vector3i.z + j) * 16);
					Vector3 cornerPos = vector;
					cornerPos.x += 16f;
					cornerPos.y += 16f;
					cornerPos.z += 16f;
					DebugLines debugLines;
					if (k == 0 && i == 0 && j == 0)
					{
						debugLines = DebugLines.Create(name, this.entityPlayerLocal.RootTransform, new Color(1f, 1f, 1f), new Color(1f, 1f, 1f), 0.1f, 0.1f, 0.1f);
					}
					else
					{
						debugLines = DebugLines.Create(name, this.entityPlayerLocal.RootTransform, new Color(0.3f, 0.3f, 0.3f), new Color(0.3f, 0.3f, 0.3f), 0.033f, 0.033f, 0.1f);
					}
					debugLines.AddCube(vector, cornerPos);
				}
			}
		}
	}

	// Token: 0x06008432 RID: 33842 RVA: 0x0035CEBC File Offset: 0x0035B0BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void DrawChunkDensities()
	{
		Vector3i vector3i = World.toChunkXYZCube(this.entityPlayerLocal.position);
		vector3i *= 16;
		IChunk chunkFromWorldPos = this.entityPlayerLocal.world.GetChunkFromWorldPos(World.worldToBlockPos(this.entityPlayerLocal.position));
		if (chunkFromWorldPos == null)
		{
			return;
		}
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				for (int k = 0; k < 16; k++)
				{
					int num = i + vector3i.y;
					int density = (int)chunkFromWorldPos.GetDensity(k, num, j);
					if (density != (int)MarchingCubes.DensityAir && density != (int)MarchingCubes.DensityTerrain)
					{
						float num2 = 0f;
						if (num > 0)
						{
							sbyte density2 = chunkFromWorldPos.GetDensity(k, num - 1, j);
							num2 = MarchingCubes.GetDecorationOffsetY((sbyte)density, density2);
						}
						string name = string.Format("PlayerDensity{0},{1},{2}", k, i, j);
						Vector3 vector;
						vector.x = (float)(vector3i.x + k) + 0.5f - 0.5f;
						vector.y = (float)num;
						vector.z = (float)(vector3i.z + j) + 0.5f - 0.5f;
						Vector3 cornerPos = vector;
						cornerPos.x += 1f;
						cornerPos.y += 0.5f + num2;
						cornerPos.z += 1f;
						DebugLines debugLines;
						if (density > 0)
						{
							float b = 1f - (float)density / 127f;
							Color color = new Color(0.2f, 0.2f, b);
							debugLines = DebugLines.Create(name, this.entityPlayerLocal.RootTransform, color, color, 0.005f, 0.005f, 0.1f);
						}
						else
						{
							float num3 = (float)(-(float)density) / 128f;
							Color color2 = new Color(num3, num3, 0.2f);
							debugLines = DebugLines.Create(name, this.entityPlayerLocal.RootTransform, color2, color2, 0.01f, 0.01f, 0.1f);
						}
						debugLines.AddCube(vector, cornerPos);
					}
				}
			}
		}
	}

	// Token: 0x06008433 RID: 33843 RVA: 0x0035D0D4 File Offset: 0x0035B2D4
	public void FindCameraSnapTarget(eCameraSnapMode snapMode, float maxDistance)
	{
		PlayerMoveController.cameraSnapTargets.Clear();
		GameManager.Instance.World.GetEntitiesInBounds(typeof(EntityAlive), new Bounds(this.entityPlayerLocal.position, Vector3.one * maxDistance), PlayerMoveController.cameraSnapTargets);
		float num = float.MaxValue;
		EntityAlive target = null;
		if (PlayerMoveController.cameraSnapTargets.Count > 0)
		{
			foreach (Entity entity in PlayerMoveController.cameraSnapTargets)
			{
				EntityAlive entityAlive = (EntityAlive)entity;
				if (!(entityAlive == this.entityPlayerLocal) && entityAlive.IsValidAimAssistSnapTarget && entityAlive.IsAlive() && !(entityAlive.ModelTransform == null))
				{
					Vector3 direction = entityAlive.GetChestTransformPosition() - this.entityPlayerLocal.cameraTransform.position;
					float sqrMagnitude = direction.sqrMagnitude;
					float num2 = Vector3.Angle(this.entityPlayerLocal.cameraTransform.forward, direction.normalized);
					if (snapMode == eCameraSnapMode.Zoom)
					{
						float num3 = 15f * (1f - this.targetSnapFalloffCurve.Evaluate(sqrMagnitude / 50f));
						if (num2 > num3)
						{
							continue;
						}
					}
					else if (num2 > 20f)
					{
						continue;
					}
					if (this.entityPlayerLocal.HitInfo.transform && this.entityPlayerLocal.HitInfo.transform.IsChildOf(entityAlive.ModelTransform) && sqrMagnitude < num)
					{
						target = entityAlive;
						break;
					}
					RaycastHit raycastHit;
					if (sqrMagnitude < num && Physics.Raycast(new Ray(this.entityPlayerLocal.cameraTransform.position, direction), out raycastHit, maxDistance, -538750997) && ((entityAlive.PhysicsTransform != null && raycastHit.collider.transform.IsChildOf(entityAlive.PhysicsTransform)) || raycastHit.collider.transform.IsChildOf(entityAlive.ModelTransform)))
					{
						num = sqrMagnitude;
						target = entityAlive;
					}
				}
			}
			this.SetCameraSnapEntity(target, snapMode);
		}
	}

	// Token: 0x06008434 RID: 33844 RVA: 0x0035D304 File Offset: 0x0035B504
	public void SetCameraSnapEntity(EntityAlive _target, eCameraSnapMode _snapMode)
	{
		this.cameraSnapTargetEntity = _target;
		this.cameraSnapMode = _snapMode;
		if (this.cameraSnapTargetEntity != null)
		{
			Vector2 b = Vector2.one * 0.5f;
			Vector2 a = this.entityPlayerLocal.playerCamera.WorldToViewportPoint(this.cameraSnapTargetEntity.GetChestTransformPosition());
			Vector2 a2 = this.entityPlayerLocal.playerCamera.WorldToViewportPoint(this.cameraSnapTargetEntity.emodel.GetHeadTransform().position);
			Vector2 vector = a - b;
			this.snapTargetingHead = ((a2 - b).sqrMagnitude < vector.sqrMagnitude);
			this.cameraSnapTime = Time.time;
		}
	}

	// Token: 0x06008435 RID: 33845 RVA: 0x0035D3BB File Offset: 0x0035B5BB
	public void ForceStopRunning()
	{
		this.entityPlayerLocal.movementInput.running = false;
		this.runToggleActive = false;
	}

	// Token: 0x06008436 RID: 33846 RVA: 0x0035D3D5 File Offset: 0x0035B5D5
	public void SetInventoryIdxFromScroll(int _idx)
	{
		this.inventoryScrollPressed = true;
		this.inventoryScrollIdxToSelect = _idx;
	}

	// Token: 0x04006627 RID: 26151
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bAllowPlayerInput = true;

	// Token: 0x04006628 RID: 26152
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static FastTags<TagGroup.Global> BowTag = FastTags<TagGroup.Global>.Parse("bow");

	// Token: 0x04006629 RID: 26153
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static PlayerMoveController Instance = null;

	// Token: 0x0400662A RID: 26154
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<NGuiAction> globalActions = new List<NGuiAction>();

	// Token: 0x0400662B RID: 26155
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 mouseLookSensitivity;

	// Token: 0x0400662C RID: 26156
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float mouseZoomSensitivity;

	// Token: 0x0400662D RID: 26157
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float zoomAccel;

	// Token: 0x0400662E RID: 26158
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float vehicleLookSensitivity;

	// Token: 0x0400662F RID: 26159
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 controllerLookSensitivity;

	// Token: 0x04006630 RID: 26160
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public GameManager gameManager;

	// Token: 0x04006631 RID: 26161
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public NGuiWdwInGameHUD guiInGame;

	// Token: 0x04006632 RID: 26162
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bCanControlOverride;

	// Token: 0x04006633 RID: 26163
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const float UnstuckCountdownTime = 5f;

	// Token: 0x04006634 RID: 26164
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bLastRespawnActive;

	// Token: 0x04006635 RID: 26165
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float respawnTime;

	// Token: 0x04006636 RID: 26166
	public RespawnType respawnReason;

	// Token: 0x04006637 RID: 26167
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool spawnWindowOpened;

	// Token: 0x04006638 RID: 26168
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityPlayerLocal entityPlayerLocal;

	// Token: 0x04006639 RID: 26169
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public LocalPlayerUI playerUI;

	// Token: 0x0400663A RID: 26170
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public GUIWindowManager windowManager;

	// Token: 0x0400663B RID: 26171
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public NGUIWindowManager nguiWindowManager;

	// Token: 0x0400663C RID: 26172
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int invertMouse;

	// Token: 0x0400663D RID: 26173
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int invertController;

	// Token: 0x0400663E RID: 26174
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bControllerVibration;

	// Token: 0x0400663F RID: 26175
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public RenderDisplacedCube focusBoxScript;

	// Token: 0x04006640 RID: 26176
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string strTextLabelPointingTo;

	// Token: 0x04006641 RID: 26177
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CountdownTimer countdownSuckItemsNearby = new CountdownTimer(0.05f, true);

	// Token: 0x04006642 RID: 26178
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float inventoryItemSwitchTimeout;

	// Token: 0x04006643 RID: 26179
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int inventoryItemToSetAfterTimeout = int.MinValue;

	// Token: 0x04006644 RID: 26180
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public PlayerAutoPilotControllor playerAutoPilotControllor;

	// Token: 0x04006645 RID: 26181
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string InteractName;

	// Token: 0x04006646 RID: 26182
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float InteractWaitTime;

	// Token: 0x04006647 RID: 26183
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeActivatePressed;

	// Token: 0x04006648 RID: 26184
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bIgnoreLeftMouseUntilReleased;

	// Token: 0x04006649 RID: 26185
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int skipMouseLookNextFrame;

	// Token: 0x0400664A RID: 26186
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool wasCameraChangeUsedWithWheel;

	// Token: 0x0400664B RID: 26187
	public int drawChunkMode;

	// Token: 0x0400664C RID: 26188
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public AnimationCurve lookAccelerationCurve;

	// Token: 0x0400664D RID: 26189
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lookAccelerationRate;

	// Token: 0x0400664E RID: 26190
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float currentLookAcceleration;

	// Token: 0x0400664F RID: 26191
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float controllerZoomSensitivity;

	// Token: 0x04006650 RID: 26192
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float controllerVehicleSensitivity;

	// Token: 0x04006651 RID: 26193
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool controllerAimAssistsEnabled = true;

	// Token: 0x04006652 RID: 26194
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cSprintModeHold = 0;

	// Token: 0x04006653 RID: 26195
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cSprintModeToggle = 1;

	// Token: 0x04006654 RID: 26196
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cSprintModeAutorun = 2;

	// Token: 0x04006655 RID: 26197
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int sprintMode;

	// Token: 0x04006656 RID: 26198
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float aimAssistSlowAmount = 1f;

	// Token: 0x04006657 RID: 26199
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bAimAssistTargetingItem;

	// Token: 0x04006658 RID: 26200
	public Vector3i FocusBoxPosition;

	// Token: 0x04006659 RID: 26201
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive cameraSnapTargetEntity;

	// Token: 0x0400665A RID: 26202
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool snapTargetingHead;

	// Token: 0x0400665B RID: 26203
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public eCameraSnapMode cameraSnapMode;

	// Token: 0x0400665C RID: 26204
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float cameraSnapTime;

	// Token: 0x0400665D RID: 26205
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<Entity> cameraSnapTargets = new List<Entity>();

	// Token: 0x0400665E RID: 26206
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public AnimationCurve targetSnapFalloffCurve;

	// Token: 0x0400665F RID: 26207
	public Action toggleGodMode;

	// Token: 0x04006660 RID: 26208
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Action teleportPlayer;

	// Token: 0x04006661 RID: 26209
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool IsGUICancelPressed;

	// Token: 0x04006662 RID: 26210
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool wasVehicle;

	// Token: 0x04006663 RID: 26211
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool runToggleActive;

	// Token: 0x04006664 RID: 26212
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float runInputTime;

	// Token: 0x04006665 RID: 26213
	public bool isAutorun;

	// Token: 0x04006666 RID: 26214
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isAutorunInvalid;

	// Token: 0x04006667 RID: 26215
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool inventoryScrollPressed;

	// Token: 0x04006668 RID: 26216
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int inventoryScrollIdxToSelect = -1;

	// Token: 0x04006669 RID: 26217
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public SpawnPosition spawnPosition = SpawnPosition.Undef;

	// Token: 0x0400666A RID: 26218
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool waitingForSpawnPointSelection;

	// Token: 0x0400666B RID: 26219
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ERoutineState unstuckCoState;

	// Token: 0x0400666C RID: 26220
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool wasUIInputActive;

	// Token: 0x0400666D RID: 26221
	public static bool useScaledMouseLook = false;

	// Token: 0x0400666E RID: 26222
	public static float lookDeltaTimeScale = 100f;

	// Token: 0x0400666F RID: 26223
	public static float mouseDeltaTimeScale = 75f;

	// Token: 0x04006670 RID: 26224
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 previousMouseInput = Vector2.zero;

	// Token: 0x04006671 RID: 26225
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool runPressedWhileActive;

	// Token: 0x04006672 RID: 26226
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<int> nextHeldItem = new List<int>();
}
