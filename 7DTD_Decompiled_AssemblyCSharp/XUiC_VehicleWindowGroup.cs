using System;
using UnityEngine.Scripting;

// Token: 0x02000EA3 RID: 3747
[Preserve]
public class XUiC_VehicleWindowGroup : XUiController
{
	// Token: 0x17000C12 RID: 3090
	// (get) Token: 0x06007651 RID: 30289 RVA: 0x00302C40 File Offset: 0x00300E40
	// (set) Token: 0x06007652 RID: 30290 RVA: 0x00302C48 File Offset: 0x00300E48
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
			this.frameWindow.Vehicle = value;
			this.frameWindow.group = this;
			Vehicle vehicle = value.GetVehicle();
			ItemValue updatedItemValue = vehicle.GetUpdatedItemValue();
			ItemStack currentItem = new ItemStack(updatedItemValue, 1);
			this.partGrid.AssembleWindow = this.frameWindow;
			this.partGrid.CurrentVehicle = vehicle;
			this.partGrid.CurrentItem = currentItem;
			this.partGrid.SetMods(updatedItemValue.Modifications);
			this.cosmeticGrid.AssembleWindow = this.frameWindow;
			this.cosmeticGrid.CurrentItem = currentItem;
			this.cosmeticGrid.SetParts(updatedItemValue.CosmeticMods);
			XUiM_AssembleItem assembleItem = base.xui.AssembleItem;
			assembleItem.AssembleWindow = this.frameWindow;
			assembleItem.CurrentItem = currentItem;
			assembleItem.CurrentItemStackController = null;
		}
	}

	// Token: 0x06007653 RID: 30291 RVA: 0x00302D24 File Offset: 0x00300F24
	public override void Init()
	{
		base.Init();
		this.nonPagingHeaderWindow = base.GetChildByType<XUiC_WindowNonPagingHeader>();
		this.headerName = Localization.Get("xuiVehicle", false);
		this.frameWindow = base.GetChildByType<XUiC_VehicleFrameWindow>();
		this.partGrid = base.GetChildByType<XUiC_VehiclePartStackGrid>();
		this.cosmeticGrid = base.GetChildByType<XUiC_ItemCosmeticStackGrid>();
	}

	// Token: 0x06007654 RID: 30292 RVA: 0x00302D78 File Offset: 0x00300F78
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
			base.xui.playerUI.windowManager.Close(XUiC_VehicleWindowGroup.ID);
		}
		base.Update(_dt);
	}

	// Token: 0x06007655 RID: 30293 RVA: 0x00302E8B File Offset: 0x0030108B
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.nonPagingHeaderWindow != null)
		{
			this.nonPagingHeaderWindow.SetHeader(this.headerName);
		}
	}

	// Token: 0x06007656 RID: 30294 RVA: 0x00302EAC File Offset: 0x003010AC
	public override void OnClose()
	{
		base.OnClose();
		XUiM_AssembleItem assembleItem = base.xui.AssembleItem;
		assembleItem.AssembleWindow = null;
		if (assembleItem.CurrentItem.itemValue == base.xui.vehicle.GetVehicle().GetUpdatedItemValue())
		{
			assembleItem.CurrentItem = null;
			assembleItem.CurrentItemStackController = null;
		}
		this.wasReleased = false;
		this.activeKeyDown = false;
		this.CurrentVehicleEntity.StopUIInteraction();
		base.xui.vehicle = null;
	}

	// Token: 0x06007657 RID: 30295 RVA: 0x00302F2C File Offset: 0x0030112C
	public void OnItemChanged(ItemStack itemStack)
	{
		this.partGrid.CurrentItem = itemStack;
		this.partGrid.SetMods(itemStack.itemValue.Modifications);
		this.cosmeticGrid.CurrentItem = itemStack;
		this.cosmeticGrid.SetParts(itemStack.itemValue.CosmeticMods);
	}

	// Token: 0x04005A3C RID: 23100
	public static string ID = "vehicle";

	// Token: 0x04005A3D RID: 23101
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_VehicleFrameWindow frameWindow;

	// Token: 0x04005A3E RID: 23102
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WindowNonPagingHeader nonPagingHeaderWindow;

	// Token: 0x04005A3F RID: 23103
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_VehiclePartStackGrid partGrid;

	// Token: 0x04005A40 RID: 23104
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemCosmeticStackGrid cosmeticGrid;

	// Token: 0x04005A41 RID: 23105
	[PublicizedFrom(EAccessModifier.Private)]
	public string headerName;

	// Token: 0x04005A42 RID: 23106
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityVehicle currentVehicleEntity;

	// Token: 0x04005A43 RID: 23107
	[PublicizedFrom(EAccessModifier.Private)]
	public bool activeKeyDown;

	// Token: 0x04005A44 RID: 23108
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasReleased;
}
