using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D0C RID: 3340
[Preserve]
public class XUiC_LootWindowGroup : XUiController
{
	// Token: 0x060067F1 RID: 26609 RVA: 0x002A21FB File Offset: 0x002A03FB
	public override void Init()
	{
		base.Init();
		this.openTimeLeft = 0f;
		this.lootWindow = base.GetChildByType<XUiC_LootWindow>();
		this.timerWindow = base.xui.GetChildByType<XUiC_Timer>();
		this.nonPagingHeaderWindow = base.GetChildByType<XUiC_WindowNonPagingHeader>();
	}

	// Token: 0x060067F2 RID: 26610 RVA: 0x002A2237 File Offset: 0x002A0437
	public void SetTileEntityChest(string _lootContainerName, ITileEntityLootable _te)
	{
		this.lootContainerName = _lootContainerName;
		this.te = _te;
		this.lootWindow.SetTileEntityChest(_lootContainerName, _te);
		this.lootingHeader = Localization.Get("xuiLooting", false);
	}

	// Token: 0x060067F3 RID: 26611 RVA: 0x002A2268 File Offset: 0x002A0468
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OpenContainer()
	{
		base.OnOpen();
		base.xui.playerUI.windowManager.OpenIfNotOpen("backpack", false, false, true);
		this.lootWindow.ViewComponent.UiTransform.gameObject.SetActive(true);
		this.lootWindow.OpenContainer();
		if (this.nonPagingHeaderWindow != null)
		{
			this.nonPagingHeaderWindow.SetHeader(this.lootingHeader);
		}
		this.lootWindow.ViewComponent.IsVisible = true;
		base.xui.playerUI.windowManager.Close("timer");
		if (this.windowGroup.UseStackPanelAlignment)
		{
			base.xui.RecenterWindowGroup(this.windowGroup, false);
		}
		this.isOpening = false;
	}

	// Token: 0x060067F4 RID: 26612 RVA: 0x002A2328 File Offset: 0x002A0528
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (base.xui.playerUI.entityPlayer != null && base.xui.playerUI.entityPlayer.hasBeenAttackedTime > 0 && this.isOpening)
		{
			GUIWindowManager windowManager = base.xui.playerUI.windowManager;
			windowManager.Close("timer");
			this.isOpening = false;
			this.isClosingFromDamage = true;
			windowManager.Close("looting");
			return;
		}
		if (this.isOpening)
		{
			if (this.te.bWasTouched || this.openTimeLeft <= 0f)
			{
				if (!this.te.bWasTouched && !this.te.bPlayerStorage && !this.te.bPlayerBackpack)
				{
					base.xui.playerUI.entityPlayer.Progression.AddLevelExp(base.xui.playerUI.entityPlayer.gameStage, "_xpFromLoot", Progression.XPTypes.Looting, true, true);
				}
				this.openTimeLeft = 0f;
				this.OpenContainer();
				return;
			}
			if (this.timerWindow != null)
			{
				float fillAmount = this.openTimeLeft / this.totalOpenTime;
				this.timerWindow.UpdateTimer(this.openTimeLeft, fillAmount);
			}
			this.openTimeLeft -= _dt;
		}
	}

	// Token: 0x060067F5 RID: 26613 RVA: 0x002A2474 File Offset: 0x002A0674
	public override void OnOpen()
	{
		this.isClosingFromDamage = false;
		if (this.te.EntityId != -1)
		{
			Entity entity = GameManager.Instance.World.GetEntity(this.te.EntityId);
			if (EffectManager.GetValue(PassiveEffects.DisableLoot, null, 0f, base.xui.playerUI.entityPlayer, null, entity.EntityClass.Tags, true, true, true, true, true, 1, true, false) > 0f)
			{
				Manager.PlayInsidePlayerHead("twitch_no_attack", -1, 0f, false, false);
				GUIWindowManager windowManager = base.xui.playerUI.windowManager;
				this.ignoreCloseSound = true;
				windowManager.Close("timer");
				this.isOpening = false;
				this.isClosingFromDamage = true;
				windowManager.Close("looting");
				return;
			}
		}
		else if (EffectManager.GetValue(PassiveEffects.DisableLoot, null, 0f, base.xui.playerUI.entityPlayer, null, this.te.blockValue.Block.Tags, true, true, true, true, true, 1, true, false) > 0f)
		{
			Manager.PlayInsidePlayerHead("twitch_no_attack", -1, 0f, false, false);
			GUIWindowManager windowManager2 = base.xui.playerUI.windowManager;
			this.ignoreCloseSound = true;
			windowManager2.Close("timer");
			this.isOpening = false;
			this.isClosingFromDamage = true;
			windowManager2.Close("looting");
			return;
		}
		this.ignoreCloseSound = false;
		base.xui.playerUI.windowManager.CloseIfOpen("backpack");
		this.lootWindow.ViewComponent.UiTransform.gameObject.SetActive(false);
		EntityPlayer entityPlayer = base.xui.playerUI.entityPlayer;
		float openTime = LootContainer.GetLootContainer(this.te.lootListName, true).openTime;
		this.totalOpenTime = (this.openTimeLeft = EffectManager.GetValue(PassiveEffects.ScavengingTime, null, entityPlayer.IsCrouching ? (openTime * 1.5f) : openTime, entityPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
		if (this.nonPagingHeaderWindow != null)
		{
			this.nonPagingHeaderWindow.SetHeader("LOOTING");
		}
		base.xui.playerUI.windowManager.OpenIfNotOpen("CalloutGroup", false, false, true);
		base.xui.playerUI.windowManager.Open("timer", false, false, true);
		this.timerWindow = base.xui.GetChildByType<XUiC_Timer>();
		this.timerWindow.currentOpenEventText = Localization.Get("xuiOpeningLoot", false);
		this.isOpening = true;
		LootContainer lootContainer = LootContainer.GetLootContainer(this.te.lootListName, true);
		if (lootContainer != null && lootContainer.soundClose != null)
		{
			Vector3 position = this.te.ToWorldPos().ToVector3() + Vector3.one * 0.5f;
			if (this.te.EntityId != -1 && GameManager.Instance.World != null)
			{
				Entity entity2 = GameManager.Instance.World.GetEntity(this.te.EntityId);
				if (entity2 != null)
				{
					position = entity2.GetPosition();
				}
			}
			Manager.BroadcastPlayByLocalPlayer(position, lootContainer.soundOpen);
		}
	}

	// Token: 0x060067F6 RID: 26614 RVA: 0x002A27A0 File Offset: 0x002A09A0
	public override void OnClose()
	{
		base.OnClose();
		base.xui.playerUI.windowManager.CloseIfOpen("backpack");
		Vector3i blockPos = this.te.ToWorldPos();
		if (this.isOpening)
		{
			base.xui.playerUI.windowManager.Close("timer");
		}
		if (this.openTimeLeft > 0f && !this.te.bWasTouched)
		{
			ITileEntityLootable selfOrFeature = GameManager.Instance.World.GetTileEntity(this.te.GetClrIdx(), blockPos).GetSelfOrFeature<ITileEntityLootable>();
			if ((selfOrFeature == null || !selfOrFeature.IsRemoving) && selfOrFeature == this.te)
			{
				this.te.bTouched = false;
				this.te.SetModified();
			}
		}
		this.lootWindow.CloseContainer(this.ignoreCloseSound);
		this.lootWindow.ViewComponent.IsVisible = false;
		this.isOpening = false;
	}

	// Token: 0x060067F7 RID: 26615 RVA: 0x002A288A File Offset: 0x002A0A8A
	[PublicizedFrom(EAccessModifier.Private)]
	public static XUiC_LootWindowGroup GetInstance(XUi _xuiInstance = null)
	{
		if (_xuiInstance == null)
		{
			_xuiInstance = LocalPlayerUI.GetUIForPrimaryPlayer().xui;
		}
		return (XUiC_LootWindowGroup)_xuiInstance.FindWindowGroupByName(XUiC_LootWindowGroup.ID);
	}

	// Token: 0x060067F8 RID: 26616 RVA: 0x002A28AB File Offset: 0x002A0AAB
	public static Vector3i GetTeBlockPos(XUi _xuiInstance = null)
	{
		ITileEntityLootable tileEntityLootable = XUiC_LootWindowGroup.GetInstance(_xuiInstance).te;
		if (tileEntityLootable == null)
		{
			return Vector3i.zero;
		}
		return tileEntityLootable.ToWorldPos();
	}

	// Token: 0x060067F9 RID: 26617 RVA: 0x002A28C8 File Offset: 0x002A0AC8
	public static void CloseIfOpenAtPos(Vector3i _blockPos, XUi _xuiInstance = null)
	{
		GUIWindowManager windowManager = XUiC_LootWindowGroup.GetInstance(_xuiInstance).xui.playerUI.windowManager;
		if (windowManager.IsWindowOpen(XUiC_LootWindowGroup.ID) && XUiC_LootWindowGroup.GetTeBlockPos(null) == _blockPos)
		{
			windowManager.Close(XUiC_LootWindowGroup.ID);
		}
	}

	// Token: 0x04004E5E RID: 20062
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_LootWindow lootWindow;

	// Token: 0x04004E5F RID: 20063
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label headerName;

	// Token: 0x04004E60 RID: 20064
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WindowNonPagingHeader nonPagingHeaderWindow;

	// Token: 0x04004E61 RID: 20065
	[PublicizedFrom(EAccessModifier.Private)]
	public ITileEntityLootable te;

	// Token: 0x04004E62 RID: 20066
	[PublicizedFrom(EAccessModifier.Private)]
	public string lootContainerName;

	// Token: 0x04004E63 RID: 20067
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOpening;

	// Token: 0x04004E64 RID: 20068
	[PublicizedFrom(EAccessModifier.Private)]
	public float openTimeLeft;

	// Token: 0x04004E65 RID: 20069
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Timer timerWindow;

	// Token: 0x04004E66 RID: 20070
	[PublicizedFrom(EAccessModifier.Private)]
	public UISprite timerHourGlass;

	// Token: 0x04004E67 RID: 20071
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isClosingFromDamage;

	// Token: 0x04004E68 RID: 20072
	[PublicizedFrom(EAccessModifier.Private)]
	public string lootingHeader;

	// Token: 0x04004E69 RID: 20073
	public static string ID = "looting";

	// Token: 0x04004E6A RID: 20074
	[PublicizedFrom(EAccessModifier.Private)]
	public float totalOpenTime;

	// Token: 0x04004E6B RID: 20075
	[PublicizedFrom(EAccessModifier.Private)]
	public bool ignoreCloseSound;
}
