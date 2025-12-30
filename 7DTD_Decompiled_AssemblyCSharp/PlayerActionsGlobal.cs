using System;
using InControl;

// Token: 0x020004E1 RID: 1249
public class PlayerActionsGlobal : PlayerActionsBase
{
	// Token: 0x17000428 RID: 1064
	// (get) Token: 0x0600286C RID: 10348 RVA: 0x001065A2 File Offset: 0x001047A2
	public static PlayerActionsGlobal Instance
	{
		get
		{
			return PlayerActionsGlobal.m_Instance;
		}
	}

	// Token: 0x0600286D RID: 10349 RVA: 0x001065A9 File Offset: 0x001047A9
	public static void Init()
	{
		if (PlayerActionsGlobal.m_Instance == null)
		{
			PlayerActionsGlobal.m_Instance = new PlayerActionsGlobal();
		}
	}

	// Token: 0x0600286E RID: 10350 RVA: 0x001065BC File Offset: 0x001047BC
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerActionsGlobal()
	{
		base.Name = "global";
	}

	// Token: 0x0600286F RID: 10351 RVA: 0x001065D0 File Offset: 0x001047D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateActions()
	{
		this.Console = base.CreatePlayerAction("Console");
		this.Console.UserData = new PlayerActionData.ActionUserData("inpActConsoleName", null, PlayerActionData.GroupGlobalFunctions, PlayerActionData.EAppliesToInputType.KbdMouseOnly, false, false, false, true);
		this.ShowDebugData = base.CreatePlayerAction("Show Debug Data");
		this.ShowDebugData.UserData = new PlayerActionData.ActionUserData("inpActShowDebugDataName", null, PlayerActionData.GroupGlobalFunctions, PlayerActionData.EAppliesToInputType.KbdMouseOnly, false, false, false, true);
		this.Fullscreen = base.CreatePlayerAction("Fullscreen");
		this.Fullscreen.UserData = new PlayerActionData.ActionUserData("inpActFullscreenName", null, PlayerActionData.GroupGlobalFunctions, PlayerActionData.EAppliesToInputType.KbdMouseOnly, false, false, false, true);
		this.SwitchView = base.CreatePlayerAction("TVP");
		this.SwitchView.UserData = new PlayerActionData.ActionUserData("inpActSwitchViewName", null, PlayerActionData.GroupGlobalFunctions, PlayerActionData.EAppliesToInputType.KbdMouseOnly, false, false, false, true);
		this.DebugSpawn = base.CreatePlayerAction("DebugSpawn");
		this.DebugSpawn.UserData = new PlayerActionData.ActionUserData("inpActDebugSpawnName", null, PlayerActionData.GroupGlobalFunctions, PlayerActionData.EAppliesToInputType.KbdMouseOnly, false, false, false, true);
		this.DebugGameEvent = base.CreatePlayerAction("DebugGameEvent");
		this.DebugGameEvent.UserData = new PlayerActionData.ActionUserData("inpActDebugGameEventName", null, PlayerActionData.GroupGlobalFunctions, PlayerActionData.EAppliesToInputType.KbdMouseOnly, false, false, false, true);
		this.SwitchHUD = base.CreatePlayerAction("SwitchHUD");
		this.SwitchHUD.UserData = new PlayerActionData.ActionUserData("inpActSwitchHUDName", null, PlayerActionData.GroupGlobalFunctions, PlayerActionData.EAppliesToInputType.KbdMouseOnly, false, false, false, true);
		this.ShowFPS = base.CreatePlayerAction("ShowFPS");
		this.ShowFPS.UserData = new PlayerActionData.ActionUserData("inpActShowFPSName", null, PlayerActionData.GroupGlobalFunctions, PlayerActionData.EAppliesToInputType.KbdMouseOnly, false, false, false, true);
		this.Screenshot = base.CreatePlayerAction("Screenshot");
		this.Screenshot.UserData = new PlayerActionData.ActionUserData("inpActScreenshotName", null, PlayerActionData.GroupGlobalFunctions, PlayerActionData.EAppliesToInputType.KbdMouseOnly, false, false, false, true);
		this.DebugScreenshot = base.CreatePlayerAction("DebugScreenshot");
		this.DebugScreenshot.UserData = new PlayerActionData.ActionUserData("inpActDebugScreenshotName", null, PlayerActionData.GroupGlobalFunctions, PlayerActionData.EAppliesToInputType.KbdMouseOnly, false, false, false, true);
		this.BackgroundedScreenshot = base.CreatePlayerAction("BackgroundedScreenshot");
	}

	// Token: 0x06002870 RID: 10352 RVA: 0x001067D8 File Offset: 0x001049D8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateDefaultKeyboardBindings()
	{
		if (!Submission.Enabled)
		{
			this.Console.AddDefaultBinding(new Key[]
			{
				Key.F1
			});
			this.ShowDebugData.AddDefaultBinding(new Key[]
			{
				Key.F3
			});
			this.Fullscreen.AddDefaultBinding(new Key[]
			{
				Key.F4
			});
			this.SwitchView.AddDefaultBinding(new Key[]
			{
				Key.F5
			});
			this.DebugSpawn.AddDefaultBinding(new Key[]
			{
				Key.F6
			});
			this.DebugGameEvent.AddDefaultBinding(new Key[]
			{
				Key.F6,
				Key.Shift
			});
			this.SwitchHUD.AddDefaultBinding(new Key[]
			{
				Key.F7
			});
			this.ShowFPS.AddDefaultBinding(new Key[]
			{
				Key.F8
			});
			this.Screenshot.AddDefaultBinding(new Key[]
			{
				Key.F9
			});
			this.BackgroundedScreenshot.AddDefaultBinding(new Key[]
			{
				Key.F10
			});
			this.DebugScreenshot.AddDefaultBinding(new Key[]
			{
				Key.F11
			});
		}
	}

	// Token: 0x06002871 RID: 10353 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateDefaultJoystickBindings()
	{
	}

	// Token: 0x04001F21 RID: 7969
	[PublicizedFrom(EAccessModifier.Private)]
	public static PlayerActionsGlobal m_Instance;

	// Token: 0x04001F22 RID: 7970
	public PlayerAction Console;

	// Token: 0x04001F23 RID: 7971
	public PlayerAction ShowDebugData;

	// Token: 0x04001F24 RID: 7972
	public PlayerAction Fullscreen;

	// Token: 0x04001F25 RID: 7973
	public PlayerAction SwitchView;

	// Token: 0x04001F26 RID: 7974
	public PlayerAction DebugSpawn;

	// Token: 0x04001F27 RID: 7975
	public PlayerAction DebugGameEvent;

	// Token: 0x04001F28 RID: 7976
	public PlayerAction SwitchHUD;

	// Token: 0x04001F29 RID: 7977
	public PlayerAction ShowFPS;

	// Token: 0x04001F2A RID: 7978
	public PlayerAction Screenshot;

	// Token: 0x04001F2B RID: 7979
	public PlayerAction DebugScreenshot;

	// Token: 0x04001F2C RID: 7980
	public PlayerAction BackgroundedScreenshot;
}
