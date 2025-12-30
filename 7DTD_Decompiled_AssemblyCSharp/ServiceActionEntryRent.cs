using System;
using UnityEngine.Scripting;

// Token: 0x02000BF9 RID: 3065
[Preserve]
public class ServiceActionEntryRent : BaseItemActionEntry
{
	// Token: 0x06005DCC RID: 24012 RVA: 0x00262493 File Offset: 0x00260693
	public ServiceActionEntryRent(XUiController controller, TileEntityVendingMachine _vending) : base(controller, "lblContextActionRent", "ui_game_symbol_coin", BaseItemActionEntry.GamepadShortCut.None, "crafting/craft_click_craft", "ui/ui_denied")
	{
		this.vending = _vending;
	}

	// Token: 0x06005DCD RID: 24013 RVA: 0x002624B8 File Offset: 0x002606B8
	public override void RefreshEnabled()
	{
		base.Enabled = (this.vending.CanRent() == TileEntityVendingMachine.RentResult.Allowed);
	}

	// Token: 0x06005DCE RID: 24014 RVA: 0x002624D0 File Offset: 0x002606D0
	public override void OnDisabledActivate()
	{
		EntityPlayerLocal entityPlayer = base.ItemController.xui.playerUI.entityPlayer;
		switch (this.vending.CanRent())
		{
		case TileEntityVendingMachine.RentResult.AlreadyRented:
			GameManager.ShowTooltip(entityPlayer, Localization.Get("ttVMAlreadyRented", false), false, false, 0f);
			return;
		case TileEntityVendingMachine.RentResult.AlreadyRentingVM:
			GameManager.ShowTooltip(entityPlayer, Localization.Get("ttAlreadyRentingVM", false), false, false, 0f);
			return;
		case TileEntityVendingMachine.RentResult.NotEnoughMoney:
			if (this.vending.LocalPlayerIsOwner())
			{
				GameManager.ShowTooltip(entityPlayer, Localization.Get("ttVMNotEnoughMoneyAddTime", false), false, false, 0f);
				return;
			}
			GameManager.ShowTooltip(entityPlayer, Localization.Get("ttVMNotEnoughMoneyRent", false), false, false, 0f);
			return;
		default:
			return;
		}
	}

	// Token: 0x06005DCF RID: 24015 RVA: 0x00262584 File Offset: 0x00260784
	public override void OnActivated()
	{
		if (this.vending.Rent())
		{
			((XUiC_TraderWindow)base.ItemController).Refresh();
		}
		this.RefreshEnabled();
	}

	// Token: 0x040046F9 RID: 18169
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityVendingMachine vending;
}
