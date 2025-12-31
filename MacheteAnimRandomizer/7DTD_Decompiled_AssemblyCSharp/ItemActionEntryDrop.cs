using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000BDF RID: 3039
[Preserve]
public class ItemActionEntryDrop : BaseItemActionEntry
{
	// Token: 0x06005D77 RID: 23927 RVA: 0x0025E99C File Offset: 0x0025CB9C
	public ItemActionEntryDrop(XUiController controller) : base(controller, "lblContextActionDrop", "ui_game_symbol_drop", BaseItemActionEntry.GamepadShortCut.DPadDown, "crafting/craft_click_craft", "ui/ui_denied")
	{
	}

	// Token: 0x06005D78 RID: 23928 RVA: 0x0025E9BC File Offset: 0x0025CBBC
	public override void OnActivated()
	{
		GameManager instance = GameManager.Instance;
		if (instance)
		{
			LocalPlayerUI playerUI = base.ItemController.xui.playerUI;
			NGUIWindowManager nguiWindowManager = playerUI.nguiWindowManager;
			XUiC_ItemStack xuiC_ItemStack = (XUiC_ItemStack)base.ItemController;
			base.ItemController.xui.CollectedItemList.RemoveItemStack(xuiC_ItemStack.ItemStack);
			instance.ItemDropServer(xuiC_ItemStack.ItemStack, playerUI.entityPlayer.GetDropPosition(), Vector3.zero, playerUI.entityPlayer.entityId, 60f, false);
			playerUI.entityPlayer.PlayOneShot("itemdropped", false, false, false, null);
			xuiC_ItemStack.ItemStack = ItemStack.Empty.Clone();
		}
	}

	// Token: 0x06005D79 RID: 23929 RVA: 0x0025EA6C File Offset: 0x0025CC6C
	public override void RefreshEnabled()
	{
		XUiC_ItemStack xuiC_ItemStack = (XUiC_ItemStack)base.ItemController;
		base.Enabled = (!xuiC_ItemStack.ItemStack.IsEmpty() && xuiC_ItemStack.ItemStack.itemValue.ItemClass.CanDrop(null) && !xuiC_ItemStack.StackLock);
	}

	// Token: 0x06005D7A RID: 23930 RVA: 0x0025EABC File Offset: 0x0025CCBC
	public override void OnDisabledActivate()
	{
		GameManager.ShowTooltip(base.ItemController.xui.playerUI.entityPlayer, "This item cannot be dropped.", false, false, 0f);
	}
}
