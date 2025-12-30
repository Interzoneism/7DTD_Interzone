using System;
using InControl;

// Token: 0x020004E6 RID: 1254
public class PlayerActionsVehicle : PlayerActionsBase
{
	// Token: 0x0600288A RID: 10378 RVA: 0x00109436 File Offset: 0x00107636
	public PlayerActionsVehicle()
	{
		base.Name = "vehicle";
	}

	// Token: 0x0600288B RID: 10379 RVA: 0x0010944C File Offset: 0x0010764C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateActions()
	{
		this.MoveForward = base.CreatePlayerAction("Forward");
		this.MoveForward.UserData = new PlayerActionData.ActionUserData("inpActVehicleMoveForwardName", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.MoveBack = base.CreatePlayerAction("Back");
		this.MoveBack.UserData = new PlayerActionData.ActionUserData("inpActVehicleMoveBackName", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.MoveLeft = base.CreatePlayerAction("Left");
		this.MoveLeft.UserData = new PlayerActionData.ActionUserData("inpActVehicleMoveLeftName", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.MoveRight = base.CreatePlayerAction("Right");
		this.MoveRight.UserData = new PlayerActionData.ActionUserData("inpActVehicleMoveRightName", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.Move = base.CreateTwoAxisPlayerAction(this.MoveLeft, this.MoveRight, this.MoveBack, this.MoveForward);
		this.LookLeft = base.CreatePlayerAction("LookLeft");
		this.LookLeft.UserData = new PlayerActionData.ActionUserData("inpActVehicleLookName", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.LookRight = base.CreatePlayerAction("LookRight");
		this.LookUp = base.CreatePlayerAction("LookUp");
		this.LookDown = base.CreatePlayerAction("LookDown");
		this.Look = base.CreateTwoAxisPlayerAction(this.LookLeft, this.LookRight, this.LookDown, this.LookUp);
		this.Turbo = base.CreatePlayerAction("Run");
		this.Turbo.UserData = new PlayerActionData.ActionUserData("inpActVehicleTurboName", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.Brake = base.CreatePlayerAction("Jump");
		this.Brake.UserData = new PlayerActionData.ActionUserData("inpActVehicleBrakeName", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.Hop = base.CreatePlayerAction("Crouch");
		this.Hop.UserData = new PlayerActionData.ActionUserData("inpActVehicleHopName", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.Activate = base.CreatePlayerAction("Activate");
		this.Activate.UserData = new PlayerActionData.ActionUserData("inpActActivateName", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.Activate.FirstRepeatDelay = 0.3f;
		this.ToggleFlashlight = base.CreatePlayerAction("ToggleFlashlight");
		this.ToggleFlashlight.UserData = new PlayerActionData.ActionUserData("inpActVehicleToggleLightName", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.HonkHorn = base.CreatePlayerAction("HonkHorn");
		this.HonkHorn.UserData = new PlayerActionData.ActionUserData("inpActHonkHornName", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.Menu = base.CreatePlayerAction("Menu");
		this.Menu.UserData = new PlayerActionData.ActionUserData("inpActMenuName", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.Inventory = base.CreatePlayerAction("Inventory");
		this.Inventory.UserData = new PlayerActionData.ActionUserData("inpActInventoryName", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.Scoreboard = base.CreatePlayerAction("Scoreboard");
		this.Scoreboard.UserData = new PlayerActionData.ActionUserData("inpActScoreboardName", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.ToggleTurnMode = base.CreatePlayerAction("ToggleTurnMode");
		this.ToggleTurnMode.UserData = new PlayerActionData.ActionUserData("inpActToggleTurnMode", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.ScrollUp = base.CreatePlayerAction("ScrollUp");
		this.ScrollUp.UserData = new PlayerActionData.ActionUserData("inpActCameraZoomInName", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.ScrollDown = base.CreatePlayerAction("ScrollDown");
		this.ScrollDown.UserData = new PlayerActionData.ActionUserData("inpActCameraZoomOutName", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.Scroll = base.CreateOneAxisPlayerAction(this.ScrollDown, this.ScrollUp);
		this.LeftStickLeft = base.CreatePlayerAction("LeftStickLeft");
		this.LeftStickLeft.UserData = new PlayerActionData.ActionUserData("inpActVehicleLeftStickLeft", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.ControllerOnly, false, false, true, true);
		this.LeftStickRight = base.CreatePlayerAction("LeftStickRight");
		this.LeftStickRight.UserData = new PlayerActionData.ActionUserData("inpActVehicleLeftStickRight", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.ControllerOnly, false, false, true, true);
		this.LeftStickForward = base.CreatePlayerAction("LeftStickForward");
		this.LeftStickForward.UserData = new PlayerActionData.ActionUserData("inpActVehicleLeftStickForward", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.ControllerOnly, false, false, true, true);
		this.LeftStickBack = base.CreatePlayerAction("LeftStickBack");
		this.LeftStickBack.UserData = new PlayerActionData.ActionUserData("inpActVehicleLeftStickBack", null, PlayerActionData.GroupVehicle, PlayerActionData.EAppliesToInputType.ControllerOnly, false, false, true, true);
		this.LeftStick = base.CreateTwoAxisPlayerAction(this.LeftStickLeft, this.LeftStickRight, this.LeftStickBack, this.LeftStickForward);
	}

	// Token: 0x0600288C RID: 10380 RVA: 0x00109928 File Offset: 0x00107B28
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateDefaultJoystickBindings()
	{
		base.ListenOptions.IncludeControllers = true;
		this.MoveLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
		this.MoveRight.AddDefaultBinding(InputControlType.LeftStickRight);
		this.MoveForward.AddDefaultBinding(InputControlType.RightTrigger);
		this.MoveBack.AddDefaultBinding(InputControlType.LeftTrigger);
		this.LookLeft.AddDefaultBinding(InputControlType.RightStickLeft);
		this.LookRight.AddDefaultBinding(InputControlType.RightStickRight);
		this.LookUp.AddDefaultBinding(InputControlType.RightStickUp);
		this.LookDown.AddDefaultBinding(InputControlType.RightStickDown);
		this.Turbo.AddDefaultBinding(InputControlType.RightBumper);
		this.Brake.AddDefaultBinding(InputControlType.Action3);
		this.Hop.AddDefaultBinding(InputControlType.Action1);
		this.Activate.AddDefaultBinding(InputControlType.Action4);
		this.ToggleFlashlight.AddDefaultBinding(InputControlType.DPadLeft);
		this.HonkHorn.AddDefaultBinding(InputControlType.LeftBumper);
		this.Menu.AddDefaultBinding(InputControlType.Menu);
		this.Menu.AddDefaultBinding(InputControlType.Options);
		this.Menu.AddDefaultBinding(InputControlType.Start);
		this.Inventory.AddDefaultBinding(InputControlType.Action2);
		this.Scoreboard.AddDefaultBinding(InputControlType.View);
		this.Scoreboard.AddDefaultBinding(InputControlType.TouchPadButton);
		this.Scoreboard.AddDefaultBinding(InputControlType.Back);
		this.ToggleTurnMode.AddDefaultBinding(InputControlType.RightStickButton);
		this.ScrollUp.AddDefaultBinding(InputControlType.DPadUp);
		this.ScrollDown.AddDefaultBinding(InputControlType.DPadDown);
		this.LeftStickLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
		this.LeftStickRight.AddDefaultBinding(InputControlType.LeftStickRight);
		this.LeftStickForward.AddDefaultBinding(InputControlType.LeftStickUp);
		this.LeftStickBack.AddDefaultBinding(InputControlType.LeftStickDown);
		this.ControllerRebindableActions.Clear();
		this.ControllerRebindableActions.Add(this.MoveForward);
		this.ControllerRebindableActions.Add(this.MoveBack);
		this.ControllerRebindableActions.Add(this.Hop);
		this.ControllerRebindableActions.Add(this.Brake);
		this.ControllerRebindableActions.Add(this.Inventory);
		this.ControllerRebindableActions.Add(this.Activate);
		this.ControllerRebindableActions.Add(this.Turbo);
		this.ControllerRebindableActions.Add(this.HonkHorn);
		this.ControllerRebindableActions.Add(this.ToggleFlashlight);
		this.ControllerRebindableActions.Add(this.ToggleTurnMode);
		this.ControllerRebindableActions.Add(this.ScrollUp);
		this.ControllerRebindableActions.Add(this.ScrollDown);
	}

	// Token: 0x0600288D RID: 10381 RVA: 0x00109B80 File Offset: 0x00107D80
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
		this.Turbo.AddDefaultBinding(new Key[]
		{
			Key.LeftShift
		});
		this.Brake.AddDefaultBinding(new Key[]
		{
			Key.Space
		});
		this.Hop.AddDefaultBinding(new Key[]
		{
			Key.C
		});
		this.HonkHorn.AddDefaultBinding(new Key[]
		{
			Key.X
		});
		this.Menu.AddDefaultBinding(new Key[]
		{
			Key.Escape
		});
		this.ToggleTurnMode.AddDefaultBinding(Mouse.LeftButton);
		this.ScrollUp.AddDefaultBinding(Mouse.PositiveScrollWheel);
		this.ScrollDown.AddDefaultBinding(Mouse.NegativeScrollWheel);
	}

	// Token: 0x04001FB8 RID: 8120
	public PlayerTwoAxisAction Move;

	// Token: 0x04001FB9 RID: 8121
	public PlayerAction MoveLeft;

	// Token: 0x04001FBA RID: 8122
	public PlayerAction MoveRight;

	// Token: 0x04001FBB RID: 8123
	public PlayerAction MoveForward;

	// Token: 0x04001FBC RID: 8124
	public PlayerAction MoveBack;

	// Token: 0x04001FBD RID: 8125
	public PlayerTwoAxisAction Look;

	// Token: 0x04001FBE RID: 8126
	public PlayerAction LookLeft;

	// Token: 0x04001FBF RID: 8127
	public PlayerAction LookRight;

	// Token: 0x04001FC0 RID: 8128
	public PlayerAction LookUp;

	// Token: 0x04001FC1 RID: 8129
	public PlayerAction LookDown;

	// Token: 0x04001FC2 RID: 8130
	public PlayerAction Turbo;

	// Token: 0x04001FC3 RID: 8131
	public PlayerAction Brake;

	// Token: 0x04001FC4 RID: 8132
	public PlayerAction Hop;

	// Token: 0x04001FC5 RID: 8133
	public PlayerAction Activate;

	// Token: 0x04001FC6 RID: 8134
	public PlayerAction ToggleFlashlight;

	// Token: 0x04001FC7 RID: 8135
	public PlayerAction HonkHorn;

	// Token: 0x04001FC8 RID: 8136
	public PlayerAction Menu;

	// Token: 0x04001FC9 RID: 8137
	public PlayerAction Inventory;

	// Token: 0x04001FCA RID: 8138
	public PlayerAction Scoreboard;

	// Token: 0x04001FCB RID: 8139
	public PlayerAction ToggleTurnMode;

	// Token: 0x04001FCC RID: 8140
	public PlayerAction ScrollUp;

	// Token: 0x04001FCD RID: 8141
	public PlayerAction ScrollDown;

	// Token: 0x04001FCE RID: 8142
	public PlayerOneAxisAction Scroll;

	// Token: 0x04001FCF RID: 8143
	public PlayerTwoAxisAction LeftStick;

	// Token: 0x04001FD0 RID: 8144
	public PlayerAction LeftStickLeft;

	// Token: 0x04001FD1 RID: 8145
	public PlayerAction LeftStickRight;

	// Token: 0x04001FD2 RID: 8146
	public PlayerAction LeftStickForward;

	// Token: 0x04001FD3 RID: 8147
	public PlayerAction LeftStickBack;
}
