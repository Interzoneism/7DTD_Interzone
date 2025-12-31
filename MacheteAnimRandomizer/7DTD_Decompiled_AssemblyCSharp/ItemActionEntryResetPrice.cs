using System;
using Audio;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000BE8 RID: 3048
[Preserve]
public class ItemActionEntryResetPrice : BaseItemActionEntry
{
	// Token: 0x06005D94 RID: 23956 RVA: 0x0025FFCC File Offset: 0x0025E1CC
	public ItemActionEntryResetPrice(XUiController controller) : base(controller, "lblContextActionReset", "ui_game_symbol_coin", BaseItemActionEntry.GamepadShortCut.None, "", "ui/ui_denied")
	{
		TileEntityVendingMachine tileEntityVendingMachine = controller.xui.Trader.TraderTileEntity as TileEntityVendingMachine;
		if (tileEntityVendingMachine != null)
		{
			bool playerOwned = controller.xui.Trader.Trader.TraderInfo.PlayerOwned;
			bool rentable = controller.xui.Trader.Trader.TraderInfo.Rentable;
			this.isOwner = ((playerOwned || rentable) && tileEntityVendingMachine.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier));
			return;
		}
		this.isOwner = false;
	}

	// Token: 0x06005D95 RID: 23957 RVA: 0x00260068 File Offset: 0x0025E268
	public override void OnActivated()
	{
		XUiC_TraderItemEntry xuiC_TraderItemEntry = (XUiC_TraderItemEntry)base.ItemController;
		base.ItemController.xui.Trader.Trader.ResetMarkup(xuiC_TraderItemEntry.SlotIndex);
		xuiC_TraderItemEntry.InfoWindow.RefreshBindings(false);
		base.ItemController.xui.Trader.TraderWindowGroup.RefreshTraderItems();
		Manager.PlayInsidePlayerHead("ui_tab", -1, 0f, false, false);
	}

	// Token: 0x040046C5 RID: 18117
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOwner;
}
