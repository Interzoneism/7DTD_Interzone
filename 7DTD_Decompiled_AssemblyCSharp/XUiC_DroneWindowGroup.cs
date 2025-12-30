using System;
using UnityEngine.Scripting;

// Token: 0x02000CA4 RID: 3236
[Preserve]
public class XUiC_DroneWindowGroup : XUiController
{
	// Token: 0x17000A2C RID: 2604
	// (get) Token: 0x060063D8 RID: 25560 RVA: 0x00287601 File Offset: 0x00285801
	// (set) Token: 0x060063D9 RID: 25561 RVA: 0x0028760C File Offset: 0x0028580C
	public EntityDrone CurrentVehicleEntity
	{
		get
		{
			return this.currentVehicleEntity;
		}
		set
		{
			this.currentVehicleEntity = value;
			this.assembleWindow.group = this;
			ItemValue updatedItemValue = this.currentVehicleEntity.GetUpdatedItemValue();
			ItemStack itemStack = new ItemStack(updatedItemValue, 1);
			this.assembleWindow.ItemStack = itemStack;
			this.grid.AssembleWindow = this.assembleWindow;
			this.grid.CurrentItem = itemStack;
			this.grid.SetParts(updatedItemValue.Modifications);
			this.cosmeticGrid.AssembleWindow = this.assembleWindow;
			this.cosmeticGrid.CurrentItem = itemStack;
			this.cosmeticGrid.SetParts(updatedItemValue.CosmeticMods);
			XUiM_AssembleItem assembleItem = base.xui.AssembleItem;
			assembleItem.AssembleWindow = this.assembleWindow;
			assembleItem.CurrentItem = itemStack;
			assembleItem.CurrentItemStackController = null;
		}
	}

	// Token: 0x060063DA RID: 25562 RVA: 0x002876CC File Offset: 0x002858CC
	public override void Init()
	{
		base.Init();
		this.nonPagingHeaderWindow = base.GetChildByType<XUiC_WindowNonPagingHeader>();
		this.assembleWindow = base.GetChildByType<XUiC_AssembleDroneWindow>();
		this.grid = base.GetChildByType<XUiC_ItemDronePartStackGrid>();
		this.cosmeticGrid = base.GetChildByType<XUiC_ItemCosmeticStackGrid>();
	}

	// Token: 0x060063DB RID: 25563 RVA: 0x00287704 File Offset: 0x00285904
	public override void OnOpen()
	{
		base.OnOpen();
		GUIWindowManager windowManager = base.xui.playerUI.windowManager;
		if (this.nonPagingHeaderWindow != null)
		{
			this.nonPagingHeaderWindow.SetHeader(Localization.Get("lblContextActionModify", false));
		}
		windowManager.CloseIfOpen("windowpaging");
		XUiC_ItemInfoWindow childByType = base.xui.GetChildByType<XUiC_ItemInfoWindow>();
		if (base.xui.AssembleItem.CurrentItemStackController != null)
		{
			base.xui.AssembleItem.CurrentItemStackController.Selected = true;
			childByType.SetItemStack(base.xui.AssembleItem.CurrentItemStackController, true);
			return;
		}
		if (base.xui.AssembleItem.CurrentEquipmentStackController != null)
		{
			base.xui.AssembleItem.CurrentEquipmentStackController.Selected = false;
			childByType.SetItemStack(base.xui.AssembleItem.CurrentEquipmentStackController, true);
		}
	}

	// Token: 0x060063DC RID: 25564 RVA: 0x002877DC File Offset: 0x002859DC
	public override void OnClose()
	{
		base.OnClose();
		XUiM_AssembleItem assembleItem = base.xui.AssembleItem;
		assembleItem.AssembleWindow = null;
		if (assembleItem.CurrentItem.itemValue == this.CurrentVehicleEntity.GetUpdatedItemValue())
		{
			assembleItem.CurrentItem = null;
			assembleItem.CurrentItemStackController = null;
		}
		this.CurrentVehicleEntity.StopUIInteraction();
	}

	// Token: 0x060063DD RID: 25565 RVA: 0x00287838 File Offset: 0x00285A38
	public void OnItemChanged(ItemStack itemStack)
	{
		this.grid.CurrentItem = itemStack;
		this.grid.SetParts(itemStack.itemValue.Modifications);
		this.cosmeticGrid.CurrentItem = itemStack;
		this.cosmeticGrid.SetParts(itemStack.itemValue.CosmeticMods);
	}

	// Token: 0x04004B29 RID: 19241
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_AssembleDroneWindow assembleWindow;

	// Token: 0x04004B2A RID: 19242
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WindowNonPagingHeader nonPagingHeaderWindow;

	// Token: 0x04004B2B RID: 19243
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemDronePartStackGrid grid;

	// Token: 0x04004B2C RID: 19244
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemCosmeticStackGrid cosmeticGrid;

	// Token: 0x04004B2D RID: 19245
	public static string ID = "junkDrone";

	// Token: 0x04004B2E RID: 19246
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityDrone currentVehicleEntity;
}
