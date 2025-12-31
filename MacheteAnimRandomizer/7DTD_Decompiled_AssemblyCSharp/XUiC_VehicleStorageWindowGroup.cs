using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000EA2 RID: 3746
[Preserve]
public class XUiC_VehicleStorageWindowGroup : XUiController
{
	// Token: 0x17000C11 RID: 3089
	// (get) Token: 0x06007649 RID: 30281 RVA: 0x0030290F File Offset: 0x00300B0F
	// (set) Token: 0x0600764A RID: 30282 RVA: 0x00302917 File Offset: 0x00300B17
	public EntityVehicle CurrentVehicleEntity
	{
		get
		{
			return this.currentVehicleEntity;
		}
		set
		{
			base.xui.vehicle = value;
			this.currentVehicleEntity = value;
			this.containerWindow.SetSlots(value.bag.GetSlots());
		}
	}

	// Token: 0x0600764B RID: 30283 RVA: 0x00302942 File Offset: 0x00300B42
	public override void Init()
	{
		base.Init();
		this.containerWindow = base.GetChildByType<XUiC_VehicleContainer>();
		this.nonPagingHeaderWindow = base.GetChildByType<XUiC_WindowNonPagingHeader>();
	}

	// Token: 0x0600764C RID: 30284 RVA: 0x00302964 File Offset: 0x00300B64
	public override void Update(float _dt)
	{
		if (this.windowGroup.isShowing)
		{
			if (!base.xui.playerUI.playerInput.PermanentActions.Activate.IsPressed)
			{
				this.wasReleased = true;
			}
			if (this.wasReleased)
			{
				if (base.xui.playerUI.playerInput.PermanentActions.Activate.IsPressed)
				{
					this.activeKeyDown = true;
				}
				if (base.xui.playerUI.playerInput.PermanentActions.Activate.WasReleased && this.activeKeyDown)
				{
					this.activeKeyDown = false;
					if (!base.xui.playerUI.windowManager.IsInputActive())
					{
						base.xui.playerUI.windowManager.CloseAllOpenWindows(null, false);
					}
				}
			}
		}
		if (this.currentVehicleEntity != null && !this.currentVehicleEntity.CheckUIInteraction())
		{
			base.xui.playerUI.windowManager.Close(XUiC_VehicleStorageWindowGroup.ID);
		}
		base.Update(_dt);
	}

	// Token: 0x0600764D RID: 30285 RVA: 0x00302A78 File Offset: 0x00300C78
	public override void OnOpen()
	{
		base.OnOpen();
		GUIWindowManager windowManager = base.xui.playerUI.windowManager;
		if (this.nonPagingHeaderWindow != null)
		{
			this.nonPagingHeaderWindow.SetHeader(Localization.Get("xuiStorage", false));
		}
		ITileEntityLootable lootContainer = this.CurrentVehicleEntity.lootContainer;
		if (lootContainer != null)
		{
			LootContainer lootContainer2 = LootContainer.GetLootContainer(lootContainer.lootListName, true);
			if (lootContainer2 != null && lootContainer2.soundClose != null)
			{
				Vector3 position = lootContainer.ToWorldPos().ToVector3() + Vector3.one * 0.5f;
				if (lootContainer.EntityId != -1 && GameManager.Instance.World != null)
				{
					Entity entity = GameManager.Instance.World.GetEntity(lootContainer.EntityId);
					if (entity != null)
					{
						position = entity.GetPosition();
					}
				}
				Manager.BroadcastPlayByLocalPlayer(position, lootContainer2.soundOpen);
			}
		}
	}

	// Token: 0x0600764E RID: 30286 RVA: 0x00302B54 File Offset: 0x00300D54
	public override void OnClose()
	{
		base.OnClose();
		this.wasReleased = false;
		this.activeKeyDown = false;
		GUIWindowManager windowManager = base.xui.playerUI.windowManager;
		this.CurrentVehicleEntity.StopUIInteraction();
		base.xui.vehicle = null;
		ITileEntityLootable lootContainer = this.CurrentVehicleEntity.lootContainer;
		if (lootContainer != null)
		{
			LootContainer lootContainer2 = LootContainer.GetLootContainer(lootContainer.lootListName, true);
			if (lootContainer2 != null && lootContainer2.soundClose != null)
			{
				Vector3 position = lootContainer.ToWorldPos().ToVector3() + Vector3.one * 0.5f;
				if (lootContainer.EntityId != -1 && GameManager.Instance.World != null)
				{
					Entity entity = GameManager.Instance.World.GetEntity(lootContainer.EntityId);
					if (entity != null)
					{
						position = entity.GetPosition();
					}
				}
				Manager.BroadcastPlayByLocalPlayer(position, lootContainer2.soundClose);
			}
		}
	}

	// Token: 0x04005A36 RID: 23094
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_VehicleContainer containerWindow;

	// Token: 0x04005A37 RID: 23095
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WindowNonPagingHeader nonPagingHeaderWindow;

	// Token: 0x04005A38 RID: 23096
	public static string ID = "vehicleStorage";

	// Token: 0x04005A39 RID: 23097
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityVehicle currentVehicleEntity;

	// Token: 0x04005A3A RID: 23098
	[PublicizedFrom(EAccessModifier.Private)]
	public bool activeKeyDown;

	// Token: 0x04005A3B RID: 23099
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasReleased;
}
