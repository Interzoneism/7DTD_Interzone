using System;
using UnityEngine.Scripting;

// Token: 0x02000BED RID: 3053
[Preserve]
public class ItemActionEntryShowCosmetics : BaseItemActionEntry
{
	// Token: 0x06005DA6 RID: 23974 RVA: 0x00261277 File Offset: 0x0025F477
	public ItemActionEntryShowCosmetics(XUiController controller) : base(controller, "Cosmetic", "ui_game_symbol_wardrobe_shirt", BaseItemActionEntry.GamepadShortCut.DPadUp, "crafting/craft_click_craft", "ui/ui_denied")
	{
		base.ActionName = Localization.Get("xuiCosmetics", false);
	}

	// Token: 0x06005DA7 RID: 23975 RVA: 0x0025F7CE File Offset: 0x0025D9CE
	public override void RefreshEnabled()
	{
		base.Enabled = true;
	}

	// Token: 0x06005DA8 RID: 23976 RVA: 0x002612A8 File Offset: 0x0025F4A8
	public override void OnActivated()
	{
		XUi xui = base.ItemController.xui;
		XUiC_EquipmentStack xuiC_EquipmentStack = base.ItemController as XUiC_EquipmentStack;
		if (xuiC_EquipmentStack != null)
		{
			ItemClassArmor itemClassArmor = xuiC_EquipmentStack.ItemValue.ItemClass as ItemClassArmor;
			if (itemClassArmor != null)
			{
				XUiC_CharacterCosmeticWindowGroup.Open(xui, itemClassArmor.EquipSlot);
			}
		}
	}
}
