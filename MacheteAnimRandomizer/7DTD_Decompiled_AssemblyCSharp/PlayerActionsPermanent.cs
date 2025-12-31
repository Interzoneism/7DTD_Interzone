using System;
using InControl;

// Token: 0x020004E5 RID: 1253
public class PlayerActionsPermanent : PlayerActionsBase
{
	// Token: 0x06002886 RID: 10374 RVA: 0x00108E44 File Offset: 0x00107044
	public PlayerActionsPermanent()
	{
		base.Name = "permanent";
	}

	// Token: 0x06002887 RID: 10375 RVA: 0x00108E58 File Offset: 0x00107058
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateActions()
	{
		this.Reload = base.CreatePlayerAction("Reload");
		this.Reload.UserData = new PlayerActionData.ActionUserData("inpActReloadTakeAllName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.Reload.FirstRepeatDelay = 0.3f;
		this.Activate = base.CreatePlayerAction("Activate");
		this.Activate.UserData = new PlayerActionData.ActionUserData("inpActActivateName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.ToggleFlashlight = base.CreatePlayerAction("ToggleFlashlight");
		this.ToggleFlashlight.UserData = new PlayerActionData.ActionUserData("inpActPlayerToggleFlashlightName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.ToggleFlashlight.FirstRepeatDelay = 0.3f;
		this.Inventory = base.CreatePlayerAction("Inventory");
		this.Inventory.UserData = new PlayerActionData.ActionUserData("inpActInventoryName", null, PlayerActionData.GroupDialogs, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.Character = base.CreatePlayerAction("Character");
		this.Character.UserData = new PlayerActionData.ActionUserData("inpActCharacterName", null, PlayerActionData.GroupDialogs, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.Map = base.CreatePlayerAction("Map");
		this.Map.UserData = new PlayerActionData.ActionUserData("inpActMapName", null, PlayerActionData.GroupDialogs, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.Skills = base.CreatePlayerAction("Skills");
		this.Skills.UserData = new PlayerActionData.ActionUserData("inpActSkillsName", null, PlayerActionData.GroupDialogs, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.Quests = base.CreatePlayerAction("Quests");
		this.Quests.UserData = new PlayerActionData.ActionUserData("inpActQuestsName", null, PlayerActionData.GroupDialogs, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.Challenges = base.CreatePlayerAction("Challenges");
		this.Challenges.UserData = new PlayerActionData.ActionUserData("inpActChallengesName", null, PlayerActionData.GroupDialogs, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.Scoreboard = base.CreatePlayerAction("Scoreboard");
		this.Scoreboard.UserData = new PlayerActionData.ActionUserData("inpActScoreboardName", null, PlayerActionData.GroupDialogs, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.Creative = base.CreatePlayerAction("Creative");
		this.Creative.UserData = new PlayerActionData.ActionUserData("inpActCreativeName", null, PlayerActionData.GroupDialogs, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.DebugControllerLeft = base.CreatePlayerAction("DebugControllerLeft");
		this.DebugControllerLeft.UserData = new PlayerActionData.ActionUserData("inpActDebugControllerLeftName", null, PlayerActionData.GroupAdmin, PlayerActionData.EAppliesToInputType.ControllerOnly, false, true, true, true);
		this.DebugControllerRight = base.CreatePlayerAction("DebugControllerRight");
		this.DebugControllerRight.UserData = new PlayerActionData.ActionUserData("inpActDebugControllerRightName", null, PlayerActionData.GroupAdmin, PlayerActionData.EAppliesToInputType.ControllerOnly, false, true, true, true);
		this.Chat = base.CreatePlayerAction("Chat");
		this.Chat.UserData = new PlayerActionData.ActionUserData("inpActChatName", null, PlayerActionData.GroupMp, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.PushToTalk = base.CreatePlayerAction("PushToTalk");
		this.PushToTalk.UserData = new PlayerActionData.ActionUserData("inpActPushToTalkName", null, PlayerActionData.GroupMp, PlayerActionData.EAppliesToInputType.Both, true, false, false, false);
		this.Cancel = base.CreatePlayerAction("Cancel");
		this.Cancel.UserData = new PlayerActionData.ActionUserData("inpActCancelName", null, PlayerActionData.GroupAdmin, PlayerActionData.EAppliesToInputType.Both, false, true, true, true);
		this.Swap = base.CreatePlayerAction("Swap");
		this.Swap.UserData = new PlayerActionData.ActionUserData("inpActSwapName", null, PlayerActionData.GroupPlayerControl, PlayerActionData.EAppliesToInputType.KbdMouseOnly, true, false, false, true);
		this.PageTipsForward = base.CreatePlayerAction("PageTipsForward");
		this.PageTipsForward.UserData = new PlayerActionData.ActionUserData("inpActPageTipsForward", null, PlayerActionData.GroupAdmin, PlayerActionData.EAppliesToInputType.Both, false, true, true, true);
		this.PageTipsBack = base.CreatePlayerAction("PageTipsBack");
		this.PageTipsBack.UserData = new PlayerActionData.ActionUserData("inpActPageTipsBack", null, PlayerActionData.GroupAdmin, PlayerActionData.EAppliesToInputType.Both, false, true, true, true);
	}

	// Token: 0x06002888 RID: 10376 RVA: 0x00109228 File Offset: 0x00107428
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateDefaultJoystickBindings()
	{
		base.ListenOptions.IncludeControllers = true;
		this.DebugControllerLeft.AddDefaultBinding(InputControlType.LeftBumper);
		this.DebugControllerRight.AddDefaultBinding(InputControlType.RightBumper);
		this.Cancel.AddDefaultBinding(InputControlType.Action2);
		this.PageTipsForward.AddDefaultBinding(InputControlType.RightTrigger);
		this.PageTipsBack.AddDefaultBinding(InputControlType.LeftTrigger);
		this.PushToTalk.AddDefaultBinding(InputControlType.DPadRight);
		this.ControllerRebindableActions.Clear();
		this.ControllerRebindableActions.Add(this.PushToTalk);
	}

	// Token: 0x06002889 RID: 10377 RVA: 0x001092AC File Offset: 0x001074AC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateDefaultKeyboardBindings()
	{
		base.ListenOptions.IncludeKeys = true;
		base.ListenOptions.IncludeMouseButtons = true;
		base.ListenOptions.IncludeMouseScrollWheel = true;
		this.Reload.AddDefaultBinding(new Key[]
		{
			Key.R
		});
		this.Activate.AddDefaultBinding(new Key[]
		{
			Key.E
		});
		this.ToggleFlashlight.AddDefaultBinding(new Key[]
		{
			Key.F
		});
		this.Inventory.AddDefaultBinding(new Key[]
		{
			Key.Tab
		});
		this.Skills.AddDefaultBinding(new Key[]
		{
			Key.N
		});
		this.Quests.AddDefaultBinding(new Key[]
		{
			Key.O
		});
		this.Challenges.AddDefaultBinding(new Key[]
		{
			Key.Y
		});
		this.Character.AddDefaultBinding(new Key[]
		{
			Key.B
		});
		this.Map.AddDefaultBinding(new Key[]
		{
			Key.M
		});
		this.Creative.AddDefaultBinding(new Key[]
		{
			Key.U
		});
		this.Scoreboard.AddDefaultBinding(new Key[]
		{
			Key.I
		});
		this.Chat.AddDefaultBinding(new Key[]
		{
			Key.T
		});
		this.PushToTalk.AddDefaultBinding(new Key[]
		{
			Key.V
		});
		this.Cancel.AddDefaultBinding(new Key[]
		{
			Key.Escape
		});
		this.Swap.AddDefaultBinding(Mouse.Button4);
		this.PageTipsForward.AddDefaultBinding(Mouse.LeftButton);
		this.PageTipsBack.AddDefaultBinding(Mouse.RightButton);
	}

	// Token: 0x04001FA5 RID: 8101
	public PlayerAction Reload;

	// Token: 0x04001FA6 RID: 8102
	public PlayerAction Activate;

	// Token: 0x04001FA7 RID: 8103
	public PlayerAction ToggleFlashlight;

	// Token: 0x04001FA8 RID: 8104
	public PlayerAction Inventory;

	// Token: 0x04001FA9 RID: 8105
	public PlayerAction Skills;

	// Token: 0x04001FAA RID: 8106
	public PlayerAction Quests;

	// Token: 0x04001FAB RID: 8107
	public PlayerAction Challenges;

	// Token: 0x04001FAC RID: 8108
	public PlayerAction Character;

	// Token: 0x04001FAD RID: 8109
	public PlayerAction Map;

	// Token: 0x04001FAE RID: 8110
	public PlayerAction Creative;

	// Token: 0x04001FAF RID: 8111
	public PlayerAction Scoreboard;

	// Token: 0x04001FB0 RID: 8112
	public PlayerAction DebugControllerLeft;

	// Token: 0x04001FB1 RID: 8113
	public PlayerAction DebugControllerRight;

	// Token: 0x04001FB2 RID: 8114
	public PlayerAction Chat;

	// Token: 0x04001FB3 RID: 8115
	public PlayerAction PushToTalk;

	// Token: 0x04001FB4 RID: 8116
	public PlayerAction Cancel;

	// Token: 0x04001FB5 RID: 8117
	public PlayerAction Swap;

	// Token: 0x04001FB6 RID: 8118
	public PlayerAction PageTipsForward;

	// Token: 0x04001FB7 RID: 8119
	public PlayerAction PageTipsBack;
}
