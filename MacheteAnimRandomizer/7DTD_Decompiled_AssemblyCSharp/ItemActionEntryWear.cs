using System;
using UnityEngine.Scripting;

// Token: 0x02000BF8 RID: 3064
[Preserve]
public class ItemActionEntryWear : BaseItemActionEntry
{
	// Token: 0x06005DCA RID: 24010 RVA: 0x00262429 File Offset: 0x00260629
	public ItemActionEntryWear(XUiController controller) : base(controller, "lblContextActionWear", "ui_game_symbol_shirt", BaseItemActionEntry.GamepadShortCut.DPadUp, "crafting/craft_click_craft", "ui/ui_denied")
	{
	}

	// Token: 0x06005DCB RID: 24011 RVA: 0x00262448 File Offset: 0x00260648
	public override void OnActivated()
	{
		XUiM_PlayerEquipment playerEquipment = base.ItemController.xui.PlayerEquipment;
		ItemStack stack = ((XUiC_ItemStack)base.ItemController).ItemStack.Clone();
		((XUiC_ItemStack)base.ItemController).ItemStack = playerEquipment.EquipItem(stack);
	}
}
