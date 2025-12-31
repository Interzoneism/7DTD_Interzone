using System;
using UnityEngine;

// Token: 0x02001041 RID: 4161
public class NGuiWdwInGameHUD : MonoBehaviour
{
	// Token: 0x17000DBC RID: 3516
	// (get) Token: 0x0600839D RID: 33693 RVA: 0x00353521 File Offset: 0x00351721
	public GameManager gameManager
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return GameManager.Instance;
		}
	}

	// Token: 0x0600839E RID: 33694 RVA: 0x00353528 File Offset: 0x00351728
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		this.playerUI = base.GetComponentInParent<LocalPlayerUI>();
	}

	// Token: 0x0600839F RID: 33695 RVA: 0x00353538 File Offset: 0x00351738
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Start()
	{
		NGuiAction nguiAction = new NGuiAction("TPV", PlayerActionsGlobal.Instance.SwitchView);
		nguiAction.SetClickActionDelegate(delegate
		{
			if (this.playerEntity)
			{
				this.playerEntity.SwitchFirstPersonView(true);
			}
		});
		nguiAction.SetIsCheckedDelegate(() => this.playerEntity != null && this.playerEntity.bFirstPersonView);
		nguiAction.SetIsEnabledDelegate(() => this.playerEntity != null && this.playerEntity.Spawned && !this.playerEntity.IsDead() && GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled));
		NGuiAction nguiAction2 = new NGuiAction("DebugSpawn", PlayerActionsGlobal.Instance.DebugSpawn);
		nguiAction2.SetClickActionDelegate(delegate
		{
			this.playerUI.windowManager.SwitchVisible(XUiC_SpawnMenu.ID, false, true);
		});
		nguiAction2.SetIsEnabledDelegate(() => GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled));
		NGuiAction nguiAction3 = new NGuiAction("DebugGameEvent", PlayerActionsGlobal.Instance.DebugGameEvent);
		nguiAction3.SetClickActionDelegate(delegate
		{
			this.playerUI.windowManager.SwitchVisible(XUiC_GameEventMenu.ID, false, true);
		});
		nguiAction3.SetIsEnabledDelegate(() => GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled));
		NGuiAction nguiAction4 = new NGuiAction("SwitchHUD", PlayerActionsGlobal.Instance.SwitchHUD);
		nguiAction4.SetClickActionDelegate(delegate
		{
			this.playerUI.windowManager.ToggleHUDEnabled();
		});
		this.playerUI.windowManager.AddGlobalAction(nguiAction);
		this.playerUI.windowManager.AddGlobalAction(nguiAction2);
		this.playerUI.windowManager.AddGlobalAction(nguiAction3);
		this.playerUI.windowManager.AddGlobalAction(nguiAction4);
	}

	// Token: 0x060083A0 RID: 33696 RVA: 0x0035369B File Offset: 0x0035189B
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.playerEntity = this.playerUI.entityPlayer;
		this.playerUI.OnEntityPlayerLocalAssigned += this.HandleEntityPlayerLocalAssigned;
	}

	// Token: 0x060083A1 RID: 33697 RVA: 0x003536C5 File Offset: 0x003518C5
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		this.playerUI.OnEntityPlayerLocalAssigned -= this.HandleEntityPlayerLocalAssigned;
	}

	// Token: 0x060083A2 RID: 33698 RVA: 0x003536DE File Offset: 0x003518DE
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleEntityPlayerLocalAssigned(EntityPlayerLocal _entity)
	{
		this.playerEntity = _entity;
	}

	// Token: 0x060083A3 RID: 33699 RVA: 0x003536E8 File Offset: 0x003518E8
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnGUI()
	{
		if (!this.gameManager.gameStateManager.IsGameStarted())
		{
			return;
		}
		string @string = GameStats.GetString(EnumGameStats.ShowWindow);
		if (!string.IsNullOrEmpty(@string))
		{
			if (!this.playerUI.windowManager.IsWindowOpen(@string))
			{
				this.playerUI.windowManager.Open(@string, false, false, true);
				this.wdwOpenedOnGameStatsShowWindow = @string;
			}
		}
		else if (this.wdwOpenedOnGameStatsShowWindow != null)
		{
			this.playerUI.windowManager.Close(this.wdwOpenedOnGameStatsShowWindow);
			this.wdwOpenedOnGameStatsShowWindow = null;
		}
		if (this.playerEntity != null)
		{
			this.playerEntity.OnHUD();
		}
		if (!this.playerUI.windowManager.IsHUDEnabled())
		{
			return;
		}
		int @int = GameStats.GetInt(EnumGameStats.GameState);
		if (@int != 1)
		{
		}
	}

	// Token: 0x04006594 RID: 26004
	public Texture2D[] overlayDamageTextures = new Texture2D[8];

	// Token: 0x04006595 RID: 26005
	public Texture2D[] overlayDamageBloodDrops = new Texture2D[3];

	// Token: 0x04006596 RID: 26006
	public Texture2D CrosshairTexture;

	// Token: 0x04006597 RID: 26007
	public Texture2D CrosshairDamage;

	// Token: 0x04006598 RID: 26008
	public Texture2D CrosshairUpgrade;

	// Token: 0x04006599 RID: 26009
	public Texture2D CrosshairRepair;

	// Token: 0x0400659A RID: 26010
	public Texture2D CrosshairAiming;

	// Token: 0x0400659B RID: 26011
	public Texture2D CrosshairPowerSource;

	// Token: 0x0400659C RID: 26012
	public Texture2D CrosshairPowerItem;

	// Token: 0x0400659D RID: 26013
	public Texture2D[] StealthIcons = new Texture2D[5];

	// Token: 0x0400659E RID: 26014
	public Texture2D[] StealthOverlays = new Texture2D[2];

	// Token: 0x0400659F RID: 26015
	public Transform FocusCube;

	// Token: 0x040065A0 RID: 26016
	public float crosshairAlpha = 1f;

	// Token: 0x040065A1 RID: 26017
	public bool showCrosshair = true;

	// Token: 0x040065A2 RID: 26018
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string wdwOpenedOnGameStatsShowWindow;

	// Token: 0x040065A3 RID: 26019
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityPlayerLocal playerEntity;

	// Token: 0x040065A4 RID: 26020
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LocalPlayerUI playerUI;
}
