using System;
using InControl;

// Token: 0x020004E2 RID: 1250
public class PlayerActionsGUI : PlayerActionsBase
{
	// Token: 0x06002872 RID: 10354 RVA: 0x001068E5 File Offset: 0x00104AE5
	public PlayerActionsGUI()
	{
		base.Name = "gui";
	}

	// Token: 0x06002873 RID: 10355 RVA: 0x001068F8 File Offset: 0x00104AF8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateActions()
	{
		this.Left = base.CreatePlayerAction("GUI Left");
		this.Left.UserData = new PlayerActionData.ActionUserData("inpActGuiCursor", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.Right = base.CreatePlayerAction("GUI Right");
		this.Right.UserData = new PlayerActionData.ActionUserData("inpActGuiCursor", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.None, true, false, false, true);
		this.Up = base.CreatePlayerAction("GUI Up");
		this.Up.UserData = new PlayerActionData.ActionUserData("inpActGuiCursor", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.None, true, false, false, true);
		this.Down = base.CreatePlayerAction("GUI Down");
		this.Down.UserData = new PlayerActionData.ActionUserData("inpActGuiCursor", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.None, true, false, false, true);
		this.Look = base.CreateTwoAxisPlayerAction(this.Left, this.Right, this.Down, this.Up);
		this.LeftClick = base.CreatePlayerAction("GUI Left Click");
		this.LeftClick.UserData = new PlayerActionData.ActionUserData("inpActGuiLeftclick", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.RightClick = base.CreatePlayerAction("GUI RightClick");
		this.RightClick.UserData = new PlayerActionData.ActionUserData("inpActGuiRightclick", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.Submit = base.CreatePlayerAction("GUI Submit");
		this.Submit.UserData = new PlayerActionData.ActionUserData("inpActUiSelectTakeFullName", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.Cancel = base.CreatePlayerAction("GUI Cancel");
		this.Cancel.UserData = new PlayerActionData.ActionUserData("inpActUiCancelName", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.HalfStack = base.CreatePlayerAction("GUI HalfStack");
		this.HalfStack.UserData = new PlayerActionData.ActionUserData("inpActTakeHalfName", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.Inspect = base.CreatePlayerAction("GUI Inspect");
		this.Inspect.UserData = new PlayerActionData.ActionUserData("inpActInspectName", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.FocusSearch = base.CreatePlayerAction("GUI FocusSearch");
		this.FocusSearch.UserData = new PlayerActionData.ActionUserData("inpActFocusSearchName", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.DPad_Left = base.CreatePlayerAction("GUI D-Pad Left");
		this.DPad_Left.UserData = new PlayerActionData.ActionUserData("inpActActionHotkey1Name", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.DPad_Right = base.CreatePlayerAction("GUI D-Pad Right");
		this.DPad_Right.UserData = new PlayerActionData.ActionUserData("inpActActionHotkey2Name", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.DPad_Up = base.CreatePlayerAction("GUI D-Pad Up");
		this.DPad_Up.UserData = new PlayerActionData.ActionUserData("inpActActionHotkey3Name", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.DPad_Down = base.CreatePlayerAction("GUI D-Pad Down");
		this.DPad_Down.UserData = new PlayerActionData.ActionUserData("inpActActionHotkey4Name", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.CameraLeft = base.CreatePlayerAction("GUI Camera Left");
		this.CameraLeft.UserData = new PlayerActionData.ActionUserData("inpActPageDownName", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.CameraRight = base.CreatePlayerAction("GUI Camera Right");
		this.CameraRight.UserData = new PlayerActionData.ActionUserData("inpActPageUpName", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.CameraUp = base.CreatePlayerAction("GUI Camera Up");
		this.CameraUp.UserData = new PlayerActionData.ActionUserData("inpActZoomInName", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.CameraDown = base.CreatePlayerAction("GUI Camera Down");
		this.CameraDown.UserData = new PlayerActionData.ActionUserData("inpActZoomOutName", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.Camera = base.CreateTwoAxisPlayerAction(this.CameraLeft, this.CameraRight, this.CameraDown, this.CameraUp);
		this.WindowPagingLeft = base.CreatePlayerAction("GUI Window Paging Up");
		this.WindowPagingLeft.UserData = new PlayerActionData.ActionUserData("inpActUiTabLeftName", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.WindowPagingRight = base.CreatePlayerAction("GUI Window Paging Down");
		this.WindowPagingRight.UserData = new PlayerActionData.ActionUserData("inpActUiTabRightName", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.PageDown = base.CreatePlayerAction("GUI Page Down");
		this.PageDown.UserData = new PlayerActionData.ActionUserData("inpActCategoryLeftName", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.PageUp = base.CreatePlayerAction("GUI Page Up");
		this.PageUp.UserData = new PlayerActionData.ActionUserData("inpActCategoryRightName", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.Both, true, false, false, true);
		this.TriggerAxis = base.CreateOneAxisPlayerAction(this.PageUp, this.PageDown);
		this.BackButton = base.CreatePlayerAction("GUI Back Button");
		this.BackButton.UserData = new PlayerActionData.ActionUserData("inpActBackButton", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.ControllerOnly, false, false, true, true);
		this.RightStick = base.CreatePlayerAction("GUI Window RightStick In");
		this.RightStick.UserData = new PlayerActionData.ActionUserData("inpActQuickMoveName", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.LeftStick = base.CreatePlayerAction("GUI Window LeftStick In");
		this.LeftStick.UserData = new PlayerActionData.ActionUserData("inpActTakeAllName", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.ControllerOnly, true, false, false, true);
		this.NavUp = base.CreatePlayerAction("GUI Window Navigate Up");
		this.NavUp.UserData = new PlayerActionData.ActionUserData("inpActNavUp", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.ControllerOnly, false, false, true, true);
		this.NavDown = base.CreatePlayerAction("GUI Window Navigate Down");
		this.NavDown.UserData = new PlayerActionData.ActionUserData("inpActNavDown", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.ControllerOnly, false, false, true, true);
		this.NavLeft = base.CreatePlayerAction("GUI Window Navigate Left");
		this.NavLeft.UserData = new PlayerActionData.ActionUserData("inpActNavLeft", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.ControllerOnly, false, false, true, true);
		this.NavRight = base.CreatePlayerAction("GUI Window Navigate Right");
		this.NavRight.UserData = new PlayerActionData.ActionUserData("inpActNavRight", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.ControllerOnly, false, false, true, true);
		this.Nav = base.CreateTwoAxisPlayerAction(this.NavLeft, this.NavRight, this.NavDown, this.NavUp);
		this.scrollUp = base.CreatePlayerAction("GUI Scroll Up");
		this.scrollUp.UserData = new PlayerActionData.ActionUserData("inpScrollUp", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.Both, false, false, true, true);
		this.scrollDown = base.CreatePlayerAction("GUI Scroll Down");
		this.scrollDown.UserData = new PlayerActionData.ActionUserData("inpscrollDown", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.Both, false, false, true, true);
		this.scroll = base.CreateOneAxisPlayerAction(this.scrollDown, this.scrollUp);
		this.Apply = base.CreatePlayerAction("GUI Apply");
		this.Apply.UserData = new PlayerActionData.ActionUserData("inpActUiApply", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.ControllerOnly, false, false, true, true);
		this.ActionUp = base.CreatePlayerAction("GUI Action Up");
		this.ActionUp.UserData = new PlayerActionData.ActionUserData("inpActUp", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.Both, false, false, true, true);
		this.ActionDown = base.CreatePlayerAction("GUI Action Down");
		this.ActionDown.UserData = new PlayerActionData.ActionUserData("inpActDown", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.Both, false, false, true, true);
		this.ActionLeft = base.CreatePlayerAction("GUI Action Left");
		this.ActionLeft.UserData = new PlayerActionData.ActionUserData("inpActLeft", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.Both, false, false, true, true);
		this.ActionRight = base.CreatePlayerAction("GUI Action Right");
		this.ActionRight.UserData = new PlayerActionData.ActionUserData("inpActRight", null, PlayerActionData.GroupUI, PlayerActionData.EAppliesToInputType.Both, false, false, true, true);
		this.Left.Raw = false;
		this.Right.Raw = false;
		this.Up.Raw = false;
		this.Down.Raw = false;
		this.CameraUp.Raw = false;
		this.CameraDown.Raw = false;
		this.CameraLeft.StateThreshold = 0.25f;
		this.CameraRight.StateThreshold = 0.25f;
	}

	// Token: 0x06002874 RID: 10356 RVA: 0x00107120 File Offset: 0x00105320
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateDefaultJoystickBindings()
	{
		this.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
		this.Right.AddDefaultBinding(InputControlType.LeftStickRight);
		this.Up.AddDefaultBinding(InputControlType.LeftStickUp);
		this.Down.AddDefaultBinding(InputControlType.LeftStickDown);
		this.DPad_Left.AddDefaultBinding(InputControlType.DPadLeft);
		this.DPad_Right.AddDefaultBinding(InputControlType.DPadRight);
		this.DPad_Up.AddDefaultBinding(InputControlType.DPadUp);
		this.DPad_Down.AddDefaultBinding(InputControlType.DPadDown);
		this.CameraLeft.AddDefaultBinding(InputControlType.RightStickLeft);
		this.CameraRight.AddDefaultBinding(InputControlType.RightStickRight);
		this.CameraUp.AddDefaultBinding(InputControlType.RightStickUp);
		this.CameraDown.AddDefaultBinding(InputControlType.RightStickDown);
		this.Submit.AddDefaultBinding(InputControlType.Action1);
		this.Cancel.AddDefaultBinding(InputControlType.Action2);
		this.HalfStack.AddDefaultBinding(InputControlType.Action3);
		this.Inspect.AddDefaultBinding(InputControlType.Action4);
		this.Apply.AddDefaultBinding(InputControlType.Start);
		this.Apply.AddDefaultBinding(InputControlType.Menu);
		this.Apply.AddDefaultBinding(InputControlType.Options);
		this.Apply.AddDefaultBinding(InputControlType.Plus);
		this.WindowPagingLeft.AddDefaultBinding(InputControlType.LeftBumper);
		this.WindowPagingRight.AddDefaultBinding(InputControlType.RightBumper);
		this.PageUp.AddDefaultBinding(InputControlType.RightTrigger);
		this.PageDown.AddDefaultBinding(InputControlType.LeftTrigger);
		this.RightStick.AddDefaultBinding(InputControlType.RightStickButton);
		this.LeftStick.AddDefaultBinding(InputControlType.LeftStickButton);
		this.NavUp.AddDefaultBinding(InputControlType.DPadUp);
		this.NavDown.AddDefaultBinding(InputControlType.DPadDown);
		this.NavLeft.AddDefaultBinding(InputControlType.DPadLeft);
		this.NavRight.AddDefaultBinding(InputControlType.DPadRight);
		this.scrollUp.AddDefaultBinding(InputControlType.RightStickUp);
		this.scrollDown.AddDefaultBinding(InputControlType.RightStickDown);
		this.BackButton.AddDefaultBinding(InputControlType.Back);
		this.BackButton.AddDefaultBinding(InputControlType.View);
		this.BackButton.AddDefaultBinding(InputControlType.TouchPadButton);
		this.BackButton.AddDefaultBinding(InputControlType.Minus);
		this.BackButton.AddDefaultBinding(InputControlType.Select);
	}

	// Token: 0x06002875 RID: 10357 RVA: 0x00107308 File Offset: 0x00105508
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateDefaultKeyboardBindings()
	{
		this.Left.AddDefaultBinding(new Key[]
		{
			Key.LeftArrow
		});
		this.Right.AddDefaultBinding(new Key[]
		{
			Key.RightArrow
		});
		this.Up.AddDefaultBinding(new Key[]
		{
			Key.UpArrow
		});
		this.Down.AddDefaultBinding(new Key[]
		{
			Key.DownArrow
		});
		this.DPad_Left.AddDefaultBinding(new Key[]
		{
			Key.A
		});
		this.DPad_Right.AddDefaultBinding(new Key[]
		{
			Key.S
		});
		this.DPad_Up.AddDefaultBinding(new Key[]
		{
			Key.W
		});
		this.DPad_Down.AddDefaultBinding(new Key[]
		{
			Key.D
		});
		this.Cancel.AddDefaultBinding(new Key[]
		{
			Key.Escape
		});
		this.FocusSearch.AddDefaultBinding(new Key[]
		{
			Key.F
		});
		this.LeftClick.AddDefaultBinding(Mouse.LeftButton);
		this.RightClick.AddDefaultBinding(Mouse.RightButton);
		this.scrollUp.AddDefaultBinding(Mouse.PositiveScrollWheel);
		this.scrollDown.AddDefaultBinding(Mouse.NegativeScrollWheel);
		this.scrollUp.AddDefaultBinding(new Key[]
		{
			Key.UpArrow
		});
		this.scrollDown.AddDefaultBinding(new Key[]
		{
			Key.DownArrow
		});
	}

	// Token: 0x04001F2D RID: 7981
	public PlayerAction Left;

	// Token: 0x04001F2E RID: 7982
	public PlayerAction Right;

	// Token: 0x04001F2F RID: 7983
	public PlayerAction Up;

	// Token: 0x04001F30 RID: 7984
	public PlayerAction Down;

	// Token: 0x04001F31 RID: 7985
	public PlayerTwoAxisAction Look;

	// Token: 0x04001F32 RID: 7986
	public PlayerAction DPad_Left;

	// Token: 0x04001F33 RID: 7987
	public PlayerAction DPad_Right;

	// Token: 0x04001F34 RID: 7988
	public PlayerAction DPad_Up;

	// Token: 0x04001F35 RID: 7989
	public PlayerAction DPad_Down;

	// Token: 0x04001F36 RID: 7990
	public PlayerAction Submit;

	// Token: 0x04001F37 RID: 7991
	public PlayerAction Cancel;

	// Token: 0x04001F38 RID: 7992
	public PlayerAction HalfStack;

	// Token: 0x04001F39 RID: 7993
	public PlayerAction Inspect;

	// Token: 0x04001F3A RID: 7994
	public PlayerAction FocusSearch;

	// Token: 0x04001F3B RID: 7995
	public PlayerAction LeftClick;

	// Token: 0x04001F3C RID: 7996
	public PlayerAction RightClick;

	// Token: 0x04001F3D RID: 7997
	public PlayerAction CameraLeft;

	// Token: 0x04001F3E RID: 7998
	public PlayerAction CameraRight;

	// Token: 0x04001F3F RID: 7999
	public PlayerAction CameraUp;

	// Token: 0x04001F40 RID: 8000
	public PlayerAction CameraDown;

	// Token: 0x04001F41 RID: 8001
	public PlayerTwoAxisAction Camera;

	// Token: 0x04001F42 RID: 8002
	public PlayerAction WindowPagingLeft;

	// Token: 0x04001F43 RID: 8003
	public PlayerAction WindowPagingRight;

	// Token: 0x04001F44 RID: 8004
	public PlayerAction PageUp;

	// Token: 0x04001F45 RID: 8005
	public PlayerAction PageDown;

	// Token: 0x04001F46 RID: 8006
	public PlayerAction RightStick;

	// Token: 0x04001F47 RID: 8007
	public PlayerAction LeftStick;

	// Token: 0x04001F48 RID: 8008
	public PlayerAction BackButton;

	// Token: 0x04001F49 RID: 8009
	[PublicizedFrom(EAccessModifier.Private)]
	public const float LOWER_STICK_DEADZONE = 0.6f;

	// Token: 0x04001F4A RID: 8010
	[PublicizedFrom(EAccessModifier.Private)]
	public const float UPPER_STICK_DEADZONE = 0.9f;

	// Token: 0x04001F4B RID: 8011
	[PublicizedFrom(EAccessModifier.Private)]
	public const float LOWER_LEFT_STICKDEADZONE = 0.8f;

	// Token: 0x04001F4C RID: 8012
	public PlayerAction NavUp;

	// Token: 0x04001F4D RID: 8013
	public PlayerAction NavDown;

	// Token: 0x04001F4E RID: 8014
	public PlayerAction NavLeft;

	// Token: 0x04001F4F RID: 8015
	public PlayerAction NavRight;

	// Token: 0x04001F50 RID: 8016
	public PlayerTwoAxisAction Nav;

	// Token: 0x04001F51 RID: 8017
	public PlayerOneAxisAction TriggerAxis;

	// Token: 0x04001F52 RID: 8018
	public PlayerAction scrollUp;

	// Token: 0x04001F53 RID: 8019
	public PlayerAction scrollDown;

	// Token: 0x04001F54 RID: 8020
	public PlayerOneAxisAction scroll;

	// Token: 0x04001F55 RID: 8021
	public PlayerAction ActionUp;

	// Token: 0x04001F56 RID: 8022
	public PlayerAction ActionDown;

	// Token: 0x04001F57 RID: 8023
	public PlayerAction ActionLeft;

	// Token: 0x04001F58 RID: 8024
	public PlayerAction ActionRight;

	// Token: 0x04001F59 RID: 8025
	public PlayerAction Apply;
}
