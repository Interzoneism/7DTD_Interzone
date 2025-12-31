using System;
using System.Collections.Generic;
using InControl;

// Token: 0x020004E4 RID: 1252
public class PlayerActionsLocal : PlayerActionsBase
{
	// Token: 0x17000429 RID: 1065
	// (get) Token: 0x06002876 RID: 10358 RVA: 0x00107450 File Offset: 0x00105650
	public int InventorySlotWasPressed
	{
		get
		{
			for (int i = 0; i < this.InventoryActions.Count; i++)
			{
				if (this.InventoryActions[i].WasPressed)
				{
					return i;
				}
			}
			return -1;
		}
	}

	// Token: 0x1700042A RID: 1066
	// (get) Token: 0x06002877 RID: 10359 RVA: 0x0010748C File Offset: 0x0010568C
	public int InventorySlotWasReleased
	{
		get
		{
			for (int i = 0; i < this.InventoryActions.Count; i++)
			{
				if (this.InventoryActions[i].WasReleased)
				{
					return i;
				}
			}
			return -1;
		}
	}

	// Token: 0x1700042B RID: 1067
	// (get) Token: 0x06002878 RID: 10360 RVA: 0x001074C8 File Offset: 0x001056C8
	public int InventorySlotIsPressed
	{
		get
		{
			for (int i = 0; i < this.InventoryActions.Count; i++)
			{
				if (this.InventoryActions[i].IsPressed)
				{
					return i;
				}
			}
			return -1;
		}
	}

	// Token: 0x1700042C RID: 1068
	// (get) Token: 0x06002879 RID: 10361 RVA: 0x00107501 File Offset: 0x00105701
	public PlayerActionsGUI GUIActions { get; }

	// Token: 0x1700042D RID: 1069
	// (get) Token: 0x0600287A RID: 10362 RVA: 0x00107509 File Offset: 0x00105709
	public PlayerActionsVehicle VehicleActions { get; }

	// Token: 0x1700042E RID: 1070
	// (get) Token: 0x0600287B RID: 10363 RVA: 0x00107511 File Offset: 0x00105711
	public PlayerActionsPermanent PermanentActions { get; }

	// Token: 0x0600287C RID: 10364 RVA: 0x0010751C File Offset: 0x0010571C
	public PlayerActionsLocal()
	{
		base.Name = "local";
		this.GUIActions = new PlayerActionsGUI
		{
			Enabled = false
		};
		this.PermanentActions = new PlayerActionsPermanent
		{
			Enabled = true
		};
		this.VehicleActions = new PlayerActionsVehicle
		{
			Enabled = false
		};
		base.UserData = new PlayerActionData.ActionSetUserData(new PlayerActionsBase[]
		{
			this.PermanentActions
		});
		this.VehicleActions.UserData = new PlayerActionData.ActionSetUserData(new PlayerActionsBase[]
		{
			this.PermanentActions
		});
		this.GUIActions.UserData = new PlayerActionData.ActionSetUserData(new PlayerActionsBase[]
		{
			this.PermanentActions
		});
		this.PermanentActions.UserData = new PlayerActionData.ActionSetUserData(new PlayerActionsBase[]
		{
			this,
			this.VehicleActions,
			this.GUIActions
		});
		this.InventoryActions.Add(this.InventorySlot1);
		this.InventoryActions.Add(this.InventorySlot2);
		this.InventoryActions.Add(this.InventorySlot3);
		this.InventoryActions.Add(this.InventorySlot4);
		this.InventoryActions.Add(this.InventorySlot5);
		this.InventoryActions.Add(this.InventorySlot6);
		this.InventoryActions.Add(this.InventorySlot7);
		this.InventoryActions.Add(this.InventorySlot8);
		this.InventoryActions.Add(this.InventorySlot9);
		this.InventoryActions.Add(this.InventorySlot10);
		InputManager.OnActiveDeviceChanged += delegate(InputDevice inputDevice)
		{
			this.UpdateDeadzones();
		};
		this.UpdateDeadzones();
	}

	// Token: 0x0600287D RID: 10365 RVA: 0x001076D8 File Offset: 0x001058D8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateActions()
	{
		this.MoveForward = base.CreatePlayerAction("Forward");
		this.MoveForward.UserData = new PlayerActionData.ActionUserData("inpActPlayerMoveForwardName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.MoveBack = base.CreatePlayerAction("Back");
		this.MoveBack.UserData = new PlayerActionData.ActionUserData("inpActPlayerMoveBackName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.MoveLeft = base.CreatePlayerAction("Left");
		this.MoveLeft.UserData = new PlayerActionData.ActionUserData("inpActPlayerMoveLeftName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.MoveRight = base.CreatePlayerAction("Right");
		this.MoveRight.UserData = new PlayerActionData.ActionUserData("inpActPlayerMoveRightName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.Move = base.CreateTwoAxisPlayerAction(this.MoveLeft, this.MoveRight, this.MoveBack, this.MoveForward);
		this.LookLeft = base.CreatePlayerAction("LookLeft");
		this.LookLeft.UserData = new PlayerActionData.ActionUserData("inpActPlayerLookLeft", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.LookRight = base.CreatePlayerAction("LookRight");
		this.LookRight.UserData = new PlayerActionData.ActionUserData("inpActPlayerLookRight", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.LookUp = base.CreatePlayerAction("LookUp");
		this.LookUp.UserData = new PlayerActionData.ActionUserData("inpActPlayerLookUp", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.LookDown = base.CreatePlayerAction("LookDown");
		this.LookDown.UserData = new PlayerActionData.ActionUserData("inpActPlayerLookDown", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.Look = base.CreateTwoAxisPlayerAction(this.LookLeft, this.LookRight, this.LookDown, this.LookUp);
		this.Primary = base.CreatePlayerAction("Primary");
		this.Primary.UserData = new PlayerActionData.ActionUserData("inpActPlayerPrimaryName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.Primary.StateThreshold = 0.25f;
		this.Secondary = base.CreatePlayerAction("Secondary");
		this.Secondary.UserData = new PlayerActionData.ActionUserData("inpActPlayerSecondaryName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.Secondary.StateThreshold = 0.25f;
		this.Run = base.CreatePlayerAction("Run");
		this.Run.UserData = new PlayerActionData.ActionUserData("inpActPlayerRunName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.Jump = base.CreatePlayerAction("Jump");
		this.Jump.UserData = new PlayerActionData.ActionUserData("inpActPlayerJumpName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.Crouch = base.CreatePlayerAction("Crouch");
		this.Crouch.UserData = new PlayerActionData.ActionUserData("inpActPlayerCrouchName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.ToggleCrouch = base.CreatePlayerAction("ToggleCrouch");
		this.ToggleCrouch.UserData = new PlayerActionData.ActionUserData("inpActPlayerToggleCrouchName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.ScrollUp = base.CreatePlayerAction("ScrollUp");
		this.ScrollUp.UserData = new PlayerActionData.ActionUserData("inpActScopeZoomInName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.Both, true, true, false, true);
		this.ScrollDown = base.CreatePlayerAction("ScrollDown");
		this.ScrollDown.UserData = new PlayerActionData.ActionUserData("inpActScopeZoomOutName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.Both, true, true, false, true);
		this.Scroll = base.CreateOneAxisPlayerAction(this.ScrollDown, this.ScrollUp);
		this.Activate = base.CreatePlayerAction("Activate");
		this.Activate.UserData = new PlayerActionData.ActionUserData("inpActActivateName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.Activate.FirstRepeatDelay = 0.3f;
		this.Drop = base.CreatePlayerAction("Drop");
		this.Drop.UserData = new PlayerActionData.ActionUserData("inpActPlayerDropName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.Swap = base.CreatePlayerAction("Swap");
		this.Swap.UserData = new PlayerActionData.ActionUserData("inpActSwapName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.Reload = base.CreatePlayerAction("Reload");
		this.Reload.UserData = new PlayerActionData.ActionUserData("inpActPlayerReloadName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.Reload.FirstRepeatDelay = 0.3f;
		this.ToggleFlashlight = base.CreatePlayerAction("ToggleFlashlight");
		this.ToggleFlashlight.UserData = new PlayerActionData.ActionUserData("inpActPlayerToggleFlashlightName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.ToggleFlashlight.FirstRepeatDelay = 0.3f;
		this.InventorySlot1 = base.CreatePlayerAction("Inventory1");
		this.InventorySlot1.UserData = new PlayerActionData.ActionUserData("inpActInventorySlot1Name", null, PlayerActionData.GroupToolbelt, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.InventorySlot2 = base.CreatePlayerAction("Inventory2");
		this.InventorySlot2.UserData = new PlayerActionData.ActionUserData("inpActInventorySlot2Name", null, PlayerActionData.GroupToolbelt, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.InventorySlot3 = base.CreatePlayerAction("Inventory3");
		this.InventorySlot3.UserData = new PlayerActionData.ActionUserData("inpActInventorySlot3Name", null, PlayerActionData.GroupToolbelt, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.InventorySlot4 = base.CreatePlayerAction("Inventory4");
		this.InventorySlot4.UserData = new PlayerActionData.ActionUserData("inpActInventorySlot4Name", null, PlayerActionData.GroupToolbelt, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.InventorySlot5 = base.CreatePlayerAction("Inventory5");
		this.InventorySlot5.UserData = new PlayerActionData.ActionUserData("inpActInventorySlot5Name", null, PlayerActionData.GroupToolbelt, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.InventorySlot6 = base.CreatePlayerAction("Inventory6");
		this.InventorySlot6.UserData = new PlayerActionData.ActionUserData("inpActInventorySlot6Name", null, PlayerActionData.GroupToolbelt, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.InventorySlot7 = base.CreatePlayerAction("Inventory7");
		this.InventorySlot7.UserData = new PlayerActionData.ActionUserData("inpActInventorySlot7Name", null, PlayerActionData.GroupToolbelt, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.InventorySlot8 = base.CreatePlayerAction("Inventory8");
		this.InventorySlot8.UserData = new PlayerActionData.ActionUserData("inpActInventorySlot8Name", null, PlayerActionData.GroupToolbelt, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.InventorySlot9 = base.CreatePlayerAction("Inventory9");
		this.InventorySlot9.UserData = new PlayerActionData.ActionUserData("inpActInventorySlot9Name", null, PlayerActionData.GroupToolbelt, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.InventorySlot10 = base.CreatePlayerAction("Inventory10");
		this.InventorySlot10.UserData = new PlayerActionData.ActionUserData("inpActInventorySlot10Name", null, PlayerActionData.GroupToolbelt, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.InventorySlotLeft = base.CreatePlayerAction("InventorySelectLeft");
		this.InventorySlotLeft.UserData = new PlayerActionData.ActionUserData("inpActInventorySlotLeftName", null, PlayerActionData.GroupToolbelt, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.InventorySlotRight = base.CreatePlayerAction("InventorySelectRight");
		this.InventorySlotRight.UserData = new PlayerActionData.ActionUserData("inpActInventorySlotRightName", null, PlayerActionData.GroupToolbelt, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.Menu = base.CreatePlayerAction("Menu");
		this.Menu.UserData = new PlayerActionData.ActionUserData("inpActMenuName", null, PlayerActionData.GroupMenu, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.God = base.CreatePlayerAction("God");
		this.God.UserData = new PlayerActionData.ActionUserData("inpActGodModeName", null, PlayerActionData.GroupAdmin, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.Fly = base.CreatePlayerAction("Fly");
		this.Fly.UserData = new PlayerActionData.ActionUserData("inpActFlyModeName", null, PlayerActionData.GroupAdmin, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.Invisible = base.CreatePlayerAction("Invisible");
		this.Invisible.UserData = new PlayerActionData.ActionUserData("inpActInvisibleModeName", null, PlayerActionData.GroupAdmin, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.IncSpeed = base.CreatePlayerAction("IncSpeed");
		this.IncSpeed.UserData = new PlayerActionData.ActionUserData("inpActIncGodSpeedName", null, PlayerActionData.GroupAdmin, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.DecSpeed = base.CreatePlayerAction("DecSpeed");
		this.DecSpeed.UserData = new PlayerActionData.ActionUserData("inpActDecGodSpeedName", null, PlayerActionData.GroupAdmin, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.GodAlternate = base.CreatePlayerAction("GodAlternate");
		this.GodAlternate.UserData = new PlayerActionData.ActionUserData("inpActGodAlternateModeName", null, PlayerActionData.GroupAdmin, PlayerActionData.EAppliesToInputType.ControllerOnly, false, true, true, true);
		this.TeleportAlternate = base.CreatePlayerAction("TeleportAlternate");
		this.TeleportAlternate.UserData = new PlayerActionData.ActionUserData("inpActTeleportAlternateModeName", null, PlayerActionData.GroupAdmin, PlayerActionData.EAppliesToInputType.ControllerOnly, false, true, true, true);
		this.CameraChange = base.CreatePlayerAction("CameraChange");
		this.CameraChange.UserData = new PlayerActionData.ActionUserData("inpActCameraChangeName", null, PlayerActionData.GroupEditCamera, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.DetachCamera = base.CreatePlayerAction("DetachCamera");
		this.DetachCamera.UserData = new PlayerActionData.ActionUserData("inpActDetachCameraName", null, PlayerActionData.GroupEditCamera, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.ToggleDCMove = base.CreatePlayerAction("ToggleDCMove");
		this.ToggleDCMove.UserData = new PlayerActionData.ActionUserData("inpActToggleDCMoveName", null, PlayerActionData.GroupEditCamera, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.LockFreeCamera = base.CreatePlayerAction("LockFreeCamera");
		this.LockFreeCamera.UserData = new PlayerActionData.ActionUserData("inpActLockFreeCameraName", null, PlayerActionData.GroupEditCamera, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.SelectionFill = base.CreatePlayerAction("SelectionFill");
		this.SelectionFill.UserData = new PlayerActionData.ActionUserData("inpActSelectionFillName", null, PlayerActionData.GroupEditSelection, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.SelectionClear = base.CreatePlayerAction("SelectionClear");
		this.SelectionClear.UserData = new PlayerActionData.ActionUserData("inpActSelectionClearName", null, PlayerActionData.GroupEditSelection, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.SelectionSet = base.CreatePlayerAction("SelectionSet");
		this.SelectionSet.UserData = new PlayerActionData.ActionUserData("inpActSelectionSetName", null, PlayerActionData.GroupEditSelection, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.SelectionRotate = base.CreatePlayerAction("SelectionRotate");
		this.SelectionRotate.UserData = new PlayerActionData.ActionUserData("inpActSelectionRotateName", null, PlayerActionData.GroupEditSelection, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.SelectionDelete = base.CreatePlayerAction("SelectionDelete");
		this.SelectionDelete.UserData = new PlayerActionData.ActionUserData("inpActSelectionDeleteName", null, PlayerActionData.GroupEditSelection, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.SelectionMoveMode = base.CreatePlayerAction("SelectionMoveMode");
		this.SelectionMoveMode.UserData = new PlayerActionData.ActionUserData("inpActSelectionMoveModeName", null, PlayerActionData.GroupEditSelection, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.FocusCopyBlock = base.CreatePlayerAction("FocusCopyBlock");
		this.FocusCopyBlock.UserData = new PlayerActionData.ActionUserData("inpActFocusCopyBlockName", null, PlayerActionData.GroupEditOther, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.Prefab = base.CreatePlayerAction("Prefab");
		this.Prefab.UserData = new PlayerActionData.ActionUserData("inpActPrefabName", null, PlayerActionData.GroupEditOther, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.DensityM1 = base.CreatePlayerAction("DensityM1");
		this.DensityM1.UserData = new PlayerActionData.ActionUserData("inpActDensityM1Name", null, PlayerActionData.GroupEditOther, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.DensityP1 = base.CreatePlayerAction("DensityP1");
		this.DensityP1.UserData = new PlayerActionData.ActionUserData("inpActDensityP1Name", null, PlayerActionData.GroupEditOther, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.DensityM10 = base.CreatePlayerAction("DensityM10");
		this.DensityM10.UserData = new PlayerActionData.ActionUserData("inpActDensityM10Name", null, PlayerActionData.GroupEditOther, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.DensityP10 = base.CreatePlayerAction("DensityP10");
		this.DensityP10.UserData = new PlayerActionData.ActionUserData("inpActDensityP10Name", null, PlayerActionData.GroupEditOther, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.Inventory = base.CreatePlayerAction("Inventory");
		this.Inventory.UserData = new PlayerActionData.ActionUserData("inpActInventoryName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.Scoreboard = base.CreatePlayerAction("Scoreboard");
		this.Scoreboard.UserData = new PlayerActionData.ActionUserData("inpActScoreboardName", null, PlayerActionData.GroupDialogs, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.AiFreeze = base.CreatePlayerAction("AiFreeze");
		this.AiFreeze.UserData = new PlayerActionData.ActionUserData("inpActAiFreezeName", null, PlayerActionData.GroupDebugFunctions, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
	}

	// Token: 0x0600287E RID: 10366 RVA: 0x00108314 File Offset: 0x00106514
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateDefaultJoystickBindings()
	{
		base.ListenOptions.IncludeControllers = true;
		this.ConfigureJoystickLayout();
		this.Run.AddDefaultBinding(InputControlType.LeftStickButton);
		this.Jump.AddDefaultBinding(InputControlType.Action1);
		this.ToggleCrouch.AddDefaultBinding(InputControlType.RightStickButton);
		this.Activate.AddDefaultBinding(InputControlType.Action4);
		this.Reload.AddDefaultBinding(InputControlType.Action3);
		this.Primary.AddDefaultBinding(InputControlType.RightTrigger);
		this.Secondary.AddDefaultBinding(InputControlType.LeftTrigger);
		this.InventorySlotLeft.AddDefaultBinding(InputControlType.LeftBumper);
		this.InventorySlotRight.AddDefaultBinding(InputControlType.RightBumper);
		this.ToggleFlashlight.AddDefaultBinding(InputControlType.DPadUp);
		this.Drop.AddDefaultBinding(InputControlType.DPadDown);
		this.Swap.AddDefaultBinding(InputControlType.DPadLeft);
		this.ScrollUp.AddDefaultBinding(InputControlType.DPadUp);
		this.ScrollDown.AddDefaultBinding(InputControlType.DPadDown);
		this.Menu.AddDefaultBinding(InputControlType.Menu);
		this.Menu.AddDefaultBinding(InputControlType.Options);
		this.Menu.AddDefaultBinding(InputControlType.Start);
		this.Menu.AddDefaultBinding(InputControlType.Plus);
		this.Inventory.AddDefaultBinding(InputControlType.Action2);
		this.Scoreboard.AddDefaultBinding(InputControlType.View);
		this.Scoreboard.AddDefaultBinding(InputControlType.TouchPadButton);
		this.Scoreboard.AddDefaultBinding(InputControlType.Back);
		this.GodAlternate.AddDefaultBinding(InputControlType.Action4);
		this.TeleportAlternate.AddDefaultBinding(InputControlType.Action1);
		this.ControllerRebindableActions.Clear();
		this.ControllerRebindableActions.Add(this.Jump);
		this.ControllerRebindableActions.Add(this.Reload);
		this.ControllerRebindableActions.Add(this.Inventory);
		this.ControllerRebindableActions.Add(this.Activate);
		this.ControllerRebindableActions.Add(this.Swap);
		this.ControllerRebindableActions.Add(this.Primary);
		this.ControllerRebindableActions.Add(this.Secondary);
		this.ControllerRebindableActions.Add(this.Run);
		this.ControllerRebindableActions.Add(this.ToggleCrouch);
		this.ControllerRebindableActions.Add(this.InventorySlotRight);
		this.ControllerRebindableActions.Add(this.InventorySlotLeft);
		this.ControllerRebindableActions.Add(this.ToggleFlashlight);
		this.ControllerRebindableActions.Add(this.ScrollUp);
		this.ControllerRebindableActions.Add(this.ScrollDown);
		this.ControllerRebindableActions.Add(this.Drop);
	}

	// Token: 0x0600287F RID: 10367 RVA: 0x00108578 File Offset: 0x00106778
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateDefaultKeyboardBindings()
	{
		base.ListenOptions.IncludeKeys = true;
		base.ListenOptions.IncludeMouseButtons = true;
		base.ListenOptions.IncludeMouseScrollWheel = true;
		this.MoveLeft.AddDefaultBinding(new Key[]
		{
			Key.A
		});
		this.MoveRight.AddDefaultBinding(new Key[]
		{
			Key.D
		});
		this.MoveForward.AddDefaultBinding(new Key[]
		{
			Key.W
		});
		this.MoveBack.AddDefaultBinding(new Key[]
		{
			Key.S
		});
		this.LookLeft.AddDefaultBinding(Mouse.NegativeX);
		this.LookRight.AddDefaultBinding(Mouse.PositiveX);
		this.LookUp.AddDefaultBinding(Mouse.PositiveY);
		this.LookDown.AddDefaultBinding(Mouse.NegativeY);
		this.Run.AddDefaultBinding(new Key[]
		{
			Key.LeftShift
		});
		this.Jump.AddDefaultBinding(new Key[]
		{
			Key.Space
		});
		this.Crouch.AddDefaultBinding(new Key[]
		{
			Key.C
		});
		this.ToggleCrouch.AddDefaultBinding(new Key[]
		{
			Key.LeftControl
		});
		this.Drop.AddDefaultBinding(new Key[]
		{
			Key.G
		});
		this.God.AddDefaultBinding(new Key[]
		{
			Key.Q
		});
		this.Fly.AddDefaultBinding(new Key[]
		{
			Key.H
		});
		this.Invisible.AddDefaultBinding(new Key[]
		{
			Key.PadDivide
		});
		this.IncSpeed.AddDefaultBinding(new Key[]
		{
			Key.Shift,
			Key.Equals
		});
		this.DecSpeed.AddDefaultBinding(new Key[]
		{
			Key.Shift,
			Key.Minus
		});
		this.CameraChange.AddDefaultBinding(new Key[]
		{
			Key.LeftAlt
		});
		this.Primary.AddDefaultBinding(Mouse.LeftButton);
		this.Secondary.AddDefaultBinding(Mouse.RightButton);
		this.SelectionFill.AddDefaultBinding(new Key[]
		{
			Key.L
		});
		this.SelectionClear.AddDefaultBinding(new Key[]
		{
			Key.J
		});
		this.SelectionSet.AddDefaultBinding(new Key[]
		{
			Key.Z
		});
		this.SelectionRotate.AddDefaultBinding(new Key[]
		{
			Key.X
		});
		this.SelectionDelete.AddDefaultBinding(new Key[]
		{
			Key.Backspace
		});
		this.SelectionMoveMode.AddDefaultBinding(new Key[]
		{
			Key.Insert
		});
		this.FocusCopyBlock.AddDefaultBinding(Mouse.MiddleButton);
		this.Prefab.AddDefaultBinding(new Key[]
		{
			Key.K
		});
		this.DetachCamera.AddDefaultBinding(new Key[]
		{
			Key.P
		});
		this.ToggleDCMove.AddDefaultBinding(new Key[]
		{
			Key.LeftBracket
		});
		this.LockFreeCamera.AddDefaultBinding(new Key[]
		{
			Key.Pad1
		});
		this.DensityM1.AddDefaultBinding(new Key[]
		{
			Key.RightArrow
		});
		this.DensityP1.AddDefaultBinding(new Key[]
		{
			Key.LeftArrow
		});
		this.DensityM10.AddDefaultBinding(new Key[]
		{
			Key.UpArrow
		});
		this.DensityP10.AddDefaultBinding(new Key[]
		{
			Key.DownArrow
		});
		this.ScrollUp.AddDefaultBinding(Mouse.PositiveScrollWheel);
		this.ScrollDown.AddDefaultBinding(Mouse.NegativeScrollWheel);
		this.Menu.AddDefaultBinding(new Key[]
		{
			Key.Escape
		});
		this.InventorySlot1.AddDefaultBinding(new Key[]
		{
			Key.Key1
		});
		this.InventorySlot2.AddDefaultBinding(new Key[]
		{
			Key.Key2
		});
		this.InventorySlot3.AddDefaultBinding(new Key[]
		{
			Key.Key3
		});
		this.InventorySlot4.AddDefaultBinding(new Key[]
		{
			Key.Key4
		});
		this.InventorySlot5.AddDefaultBinding(new Key[]
		{
			Key.Key5
		});
		this.InventorySlot6.AddDefaultBinding(new Key[]
		{
			Key.Key6
		});
		this.InventorySlot7.AddDefaultBinding(new Key[]
		{
			Key.Key7
		});
		this.InventorySlot8.AddDefaultBinding(new Key[]
		{
			Key.Key8
		});
		this.InventorySlot9.AddDefaultBinding(new Key[]
		{
			Key.Key9
		});
		this.InventorySlot10.AddDefaultBinding(new Key[]
		{
			Key.Key0
		});
		this.InventorySlotRight.AddDefaultBinding(Mouse.NegativeScrollWheel);
		this.InventorySlotLeft.AddDefaultBinding(Mouse.PositiveScrollWheel);
		this.AiFreeze.AddDefaultBinding(new Key[]
		{
			Key.PadMultiply
		});
	}

	// Token: 0x06002880 RID: 10368 RVA: 0x001089BA File Offset: 0x00106BBA
	public void SetDeadzones(float _left, float _right)
	{
		this.leftStickDeadzone = _left;
		this.rightStickDeadzone = _right;
		this.UpdateDeadzones();
	}

	// Token: 0x06002881 RID: 10369 RVA: 0x001089D0 File Offset: 0x00106BD0
	public void UpdateDeadzones()
	{
		InputManager.ActiveDevice.LeftStick.LowerDeadZone = this.leftStickDeadzone;
		InputManager.ActiveDevice.RightStick.LowerDeadZone = this.rightStickDeadzone;
	}

	// Token: 0x06002882 RID: 10370 RVA: 0x001089FC File Offset: 0x00106BFC
	public void SetJoyStickLayout(eControllerJoystickLayout layout)
	{
		this.joystickLayout = layout;
		this.ConfigureJoystickLayout();
	}

	// Token: 0x06002883 RID: 10371 RVA: 0x00108A0C File Offset: 0x00106C0C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ConfigureJoystickLayout()
	{
		switch (this.joystickLayout)
		{
		case eControllerJoystickLayout.Standard:
			this.MoveForward.AddDefaultBinding(InputControlType.LeftStickUp);
			this.MoveForward.RemoveBinding(new DeviceBindingSource(InputControlType.RightStickUp));
			this.MoveBack.AddDefaultBinding(InputControlType.LeftStickDown);
			this.MoveBack.RemoveBinding(new DeviceBindingSource(InputControlType.RightStickDown));
			this.MoveLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
			this.MoveLeft.RemoveBinding(new DeviceBindingSource(InputControlType.RightStickLeft));
			this.MoveRight.AddDefaultBinding(InputControlType.LeftStickRight);
			this.MoveRight.RemoveBinding(new DeviceBindingSource(InputControlType.RightStickRight));
			this.LookUp.AddDefaultBinding(InputControlType.RightStickUp);
			this.LookUp.RemoveBinding(new DeviceBindingSource(InputControlType.LeftStickUp));
			this.LookDown.AddDefaultBinding(InputControlType.RightStickDown);
			this.LookDown.RemoveBinding(new DeviceBindingSource(InputControlType.LeftStickDown));
			this.LookLeft.AddDefaultBinding(InputControlType.RightStickLeft);
			this.LookLeft.RemoveBinding(new DeviceBindingSource(InputControlType.LeftStickLeft));
			this.LookRight.AddDefaultBinding(InputControlType.RightStickRight);
			this.LookRight.RemoveBinding(new DeviceBindingSource(InputControlType.LeftStickRight));
			return;
		case eControllerJoystickLayout.Southpaw:
			this.MoveForward.AddDefaultBinding(InputControlType.RightStickUp);
			this.MoveForward.RemoveBinding(new DeviceBindingSource(InputControlType.LeftStickUp));
			this.MoveBack.AddDefaultBinding(InputControlType.RightStickDown);
			this.MoveBack.RemoveBinding(new DeviceBindingSource(InputControlType.LeftStickDown));
			this.MoveLeft.AddDefaultBinding(InputControlType.RightStickLeft);
			this.MoveLeft.RemoveBinding(new DeviceBindingSource(InputControlType.LeftStickLeft));
			this.MoveRight.AddDefaultBinding(InputControlType.RightStickRight);
			this.MoveRight.RemoveBinding(new DeviceBindingSource(InputControlType.LeftStickRight));
			this.LookUp.AddDefaultBinding(InputControlType.LeftStickUp);
			this.LookUp.RemoveBinding(new DeviceBindingSource(InputControlType.RightStickUp));
			this.LookDown.AddDefaultBinding(InputControlType.LeftStickDown);
			this.LookDown.RemoveBinding(new DeviceBindingSource(InputControlType.RightStickDown));
			this.LookLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
			this.LookLeft.RemoveBinding(new DeviceBindingSource(InputControlType.RightStickLeft));
			this.LookRight.AddDefaultBinding(InputControlType.LeftStickRight);
			this.LookRight.RemoveBinding(new DeviceBindingSource(InputControlType.RightStickRight));
			return;
		case eControllerJoystickLayout.Legacy:
			this.MoveForward.AddDefaultBinding(InputControlType.LeftStickUp);
			this.MoveForward.RemoveBinding(new DeviceBindingSource(InputControlType.RightStickUp));
			this.MoveBack.AddDefaultBinding(InputControlType.LeftStickDown);
			this.MoveBack.RemoveBinding(new DeviceBindingSource(InputControlType.RightStickDown));
			this.MoveLeft.AddDefaultBinding(InputControlType.RightStickLeft);
			this.MoveLeft.RemoveBinding(new DeviceBindingSource(InputControlType.LeftStickLeft));
			this.MoveRight.AddDefaultBinding(InputControlType.RightStickRight);
			this.MoveRight.RemoveBinding(new DeviceBindingSource(InputControlType.LeftStickRight));
			this.LookUp.AddDefaultBinding(InputControlType.RightStickUp);
			this.LookUp.RemoveBinding(new DeviceBindingSource(InputControlType.LeftStickUp));
			this.LookDown.AddDefaultBinding(InputControlType.RightStickDown);
			this.LookDown.RemoveBinding(new DeviceBindingSource(InputControlType.LeftStickDown));
			this.LookLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
			this.LookLeft.RemoveBinding(new DeviceBindingSource(InputControlType.RightStickLeft));
			this.LookRight.AddDefaultBinding(InputControlType.LeftStickRight);
			this.LookRight.RemoveBinding(new DeviceBindingSource(InputControlType.RightStickRight));
			return;
		case eControllerJoystickLayout.LegacySouthpaw:
			this.MoveForward.AddDefaultBinding(InputControlType.RightStickUp);
			this.MoveForward.RemoveBinding(new DeviceBindingSource(InputControlType.LeftStickUp));
			this.MoveBack.AddDefaultBinding(InputControlType.RightStickDown);
			this.MoveBack.RemoveBinding(new DeviceBindingSource(InputControlType.LeftStickDown));
			this.MoveLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
			this.MoveLeft.RemoveBinding(new DeviceBindingSource(InputControlType.RightStickLeft));
			this.MoveRight.AddDefaultBinding(InputControlType.LeftStickRight);
			this.MoveRight.RemoveBinding(new DeviceBindingSource(InputControlType.RightStickRight));
			this.LookUp.AddDefaultBinding(InputControlType.LeftStickUp);
			this.LookUp.RemoveBinding(new DeviceBindingSource(InputControlType.RightStickUp));
			this.LookDown.AddDefaultBinding(InputControlType.LeftStickDown);
			this.LookDown.RemoveBinding(new DeviceBindingSource(InputControlType.RightStickDown));
			this.LookLeft.AddDefaultBinding(InputControlType.RightStickLeft);
			this.LookLeft.RemoveBinding(new DeviceBindingSource(InputControlType.LeftStickLeft));
			this.LookRight.AddDefaultBinding(InputControlType.RightStickRight);
			this.LookRight.RemoveBinding(new DeviceBindingSource(InputControlType.LeftStickRight));
			return;
		default:
			return;
		}
	}

	// Token: 0x06002884 RID: 10372 RVA: 0x00108DE4 File Offset: 0x00106FE4
	public bool AnyGUIActionPressed()
	{
		using (IEnumerator<PlayerAction> enumerator = this.GUIActions.Actions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.WasPressed)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x04001F5F RID: 8031
	public PlayerTwoAxisAction Move;

	// Token: 0x04001F60 RID: 8032
	public PlayerAction MoveLeft;

	// Token: 0x04001F61 RID: 8033
	public PlayerAction MoveRight;

	// Token: 0x04001F62 RID: 8034
	public PlayerAction MoveForward;

	// Token: 0x04001F63 RID: 8035
	public PlayerAction MoveBack;

	// Token: 0x04001F64 RID: 8036
	public PlayerTwoAxisAction Look;

	// Token: 0x04001F65 RID: 8037
	public PlayerAction LookLeft;

	// Token: 0x04001F66 RID: 8038
	public PlayerAction LookRight;

	// Token: 0x04001F67 RID: 8039
	public PlayerAction LookUp;

	// Token: 0x04001F68 RID: 8040
	public PlayerAction LookDown;

	// Token: 0x04001F69 RID: 8041
	public PlayerAction Run;

	// Token: 0x04001F6A RID: 8042
	public PlayerAction Jump;

	// Token: 0x04001F6B RID: 8043
	public PlayerAction Crouch;

	// Token: 0x04001F6C RID: 8044
	public PlayerAction ToggleCrouch;

	// Token: 0x04001F6D RID: 8045
	public PlayerAction Activate;

	// Token: 0x04001F6E RID: 8046
	public PlayerAction Drop;

	// Token: 0x04001F6F RID: 8047
	public PlayerAction Swap;

	// Token: 0x04001F70 RID: 8048
	public PlayerAction Reload;

	// Token: 0x04001F71 RID: 8049
	public PlayerAction Primary;

	// Token: 0x04001F72 RID: 8050
	public PlayerAction Secondary;

	// Token: 0x04001F73 RID: 8051
	public PlayerAction ToggleFlashlight;

	// Token: 0x04001F74 RID: 8052
	public PlayerAction God;

	// Token: 0x04001F75 RID: 8053
	public PlayerAction Fly;

	// Token: 0x04001F76 RID: 8054
	public PlayerAction Invisible;

	// Token: 0x04001F77 RID: 8055
	public PlayerAction IncSpeed;

	// Token: 0x04001F78 RID: 8056
	public PlayerAction DecSpeed;

	// Token: 0x04001F79 RID: 8057
	public PlayerAction GodAlternate;

	// Token: 0x04001F7A RID: 8058
	public PlayerAction TeleportAlternate;

	// Token: 0x04001F7B RID: 8059
	public PlayerAction CameraChange;

	// Token: 0x04001F7C RID: 8060
	public PlayerAction SelectionFill;

	// Token: 0x04001F7D RID: 8061
	public PlayerAction SelectionClear;

	// Token: 0x04001F7E RID: 8062
	public PlayerAction SelectionSet;

	// Token: 0x04001F7F RID: 8063
	public PlayerAction SelectionRotate;

	// Token: 0x04001F80 RID: 8064
	public PlayerAction SelectionDelete;

	// Token: 0x04001F81 RID: 8065
	public PlayerAction SelectionMoveMode;

	// Token: 0x04001F82 RID: 8066
	public PlayerAction FocusCopyBlock;

	// Token: 0x04001F83 RID: 8067
	public PlayerAction Prefab;

	// Token: 0x04001F84 RID: 8068
	public PlayerAction DetachCamera;

	// Token: 0x04001F85 RID: 8069
	public PlayerAction ToggleDCMove;

	// Token: 0x04001F86 RID: 8070
	public PlayerAction LockFreeCamera;

	// Token: 0x04001F87 RID: 8071
	public PlayerAction DensityM1;

	// Token: 0x04001F88 RID: 8072
	public PlayerAction DensityP1;

	// Token: 0x04001F89 RID: 8073
	public PlayerAction DensityM10;

	// Token: 0x04001F8A RID: 8074
	public PlayerAction DensityP10;

	// Token: 0x04001F8B RID: 8075
	public PlayerAction ScrollUp;

	// Token: 0x04001F8C RID: 8076
	public PlayerAction ScrollDown;

	// Token: 0x04001F8D RID: 8077
	public PlayerOneAxisAction Scroll;

	// Token: 0x04001F8E RID: 8078
	public PlayerAction Menu;

	// Token: 0x04001F8F RID: 8079
	public PlayerAction Inventory;

	// Token: 0x04001F90 RID: 8080
	public PlayerAction Scoreboard;

	// Token: 0x04001F91 RID: 8081
	public PlayerAction InventorySlot1;

	// Token: 0x04001F92 RID: 8082
	public PlayerAction InventorySlot2;

	// Token: 0x04001F93 RID: 8083
	public PlayerAction InventorySlot3;

	// Token: 0x04001F94 RID: 8084
	public PlayerAction InventorySlot4;

	// Token: 0x04001F95 RID: 8085
	public PlayerAction InventorySlot5;

	// Token: 0x04001F96 RID: 8086
	public PlayerAction InventorySlot6;

	// Token: 0x04001F97 RID: 8087
	public PlayerAction InventorySlot7;

	// Token: 0x04001F98 RID: 8088
	public PlayerAction InventorySlot8;

	// Token: 0x04001F99 RID: 8089
	public PlayerAction InventorySlot9;

	// Token: 0x04001F9A RID: 8090
	public PlayerAction InventorySlot10;

	// Token: 0x04001F9B RID: 8091
	public PlayerAction InventorySlotLeft;

	// Token: 0x04001F9C RID: 8092
	public PlayerAction InventorySlotRight;

	// Token: 0x04001F9D RID: 8093
	public PlayerAction AiFreeze;

	// Token: 0x04001F9E RID: 8094
	public float leftStickDeadzone = 0.1f;

	// Token: 0x04001F9F RID: 8095
	public float rightStickDeadzone = 0.1f;

	// Token: 0x04001FA0 RID: 8096
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<PlayerAction> InventoryActions = new List<PlayerAction>();

	// Token: 0x04001FA1 RID: 8097
	public eControllerJoystickLayout joystickLayout;
}
