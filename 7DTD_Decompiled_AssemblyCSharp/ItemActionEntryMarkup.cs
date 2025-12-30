using System;
using Audio;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000BE3 RID: 3043
[Preserve]
public class ItemActionEntryMarkup : BaseItemActionEntry
{
	// Token: 0x06005D82 RID: 23938 RVA: 0x0025EE5C File Offset: 0x0025D05C
	public ItemActionEntryMarkup(XUiController controller) : base(controller, "", "ui_game_symbol_add", BaseItemActionEntry.GamepadShortCut.DPadRight, "", "ui/ui_denied")
	{
		base.ActionName = string.Format(Localization.Get("lblContextActionPrice", false), 20);
		if (controller.xui.Trader.TraderTileEntity is TileEntityVendingMachine)
		{
			bool playerOwned = controller.xui.Trader.Trader.TraderInfo.PlayerOwned;
			bool rentable = controller.xui.Trader.Trader.TraderInfo.Rentable;
			TileEntityVendingMachine tileEntityVendingMachine = controller.xui.Trader.TraderTileEntity as TileEntityVendingMachine;
			this.isOwner = ((playerOwned || rentable) && tileEntityVendingMachine.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier));
			return;
		}
		this.isOwner = false;
	}

	// Token: 0x06005D83 RID: 23939 RVA: 0x0025EF28 File Offset: 0x0025D128
	public override void OnActivated()
	{
		XUiC_TraderItemEntry xuiC_TraderItemEntry = (XUiC_TraderItemEntry)base.ItemController;
		base.ItemController.xui.Trader.Trader.IncreaseMarkup(xuiC_TraderItemEntry.SlotIndex);
		xuiC_TraderItemEntry.InfoWindow.RefreshBindings(false);
		base.ItemController.xui.Trader.TraderWindowGroup.RefreshTraderItems();
		Manager.PlayInsidePlayerHead("ui_tab", -1, 0f, false, false);
	}

	// Token: 0x040046BB RID: 18107
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOwner;
}
