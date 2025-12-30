using System;
using Audio;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000BEA RID: 3050
[Preserve]
public class ItemActionEntrySell : BaseItemActionEntry
{
	// Token: 0x06005D9E RID: 23966 RVA: 0x00260674 File Offset: 0x0025E874
	public ItemActionEntrySell(XUiController controller) : base(controller, "lblContextActionSell", "ui_game_symbol_coin", BaseItemActionEntry.GamepadShortCut.None, "", "ui/ui_denied")
	{
		TileEntityVendingMachine tileEntityVendingMachine = controller.xui.Trader.TraderTileEntity as TileEntityVendingMachine;
		if (tileEntityVendingMachine != null)
		{
			bool playerOwned = controller.xui.Trader.Trader.TraderInfo.PlayerOwned;
			bool rentable = controller.xui.Trader.Trader.TraderInfo.Rentable;
			this.isOwner = ((playerOwned || rentable) && tileEntityVendingMachine.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier));
		}
		else
		{
			this.isOwner = false;
		}
		if (this.isOwner)
		{
			base.ActionName = Localization.Get("lblContextActionAdd", false);
			base.IconName = "ui_game_symbol_hand";
			return;
		}
		base.ActionName = Localization.Get("lblContextActionSell", false);
		base.IconName = "ui_game_symbol_coin";
	}

	// Token: 0x06005D9F RID: 23967 RVA: 0x00260750 File Offset: 0x0025E950
	public override void RefreshEnabled()
	{
		XUiC_ItemStack xuiC_ItemStack = (XUiC_ItemStack)base.ItemController;
		int count = xuiC_ItemStack.InfoWindow.BuySellCounter.Count;
		this.state = ItemActionEntrySell.StateTypes.Normal;
		if (this.isOwner)
		{
			ItemStack itemStack = xuiC_ItemStack.ItemStack.Clone();
			if (count <= 0)
			{
				this.state = ItemActionEntrySell.StateTypes.NotEnoughItems;
			}
			if (!itemStack.IsEmpty())
			{
				float num = itemStack.itemValue.ItemClass.IsBlock() ? Block.list[itemStack.itemValue.type].EconomicValue : itemStack.itemValue.ItemClass.EconomicValue;
				if (!itemStack.itemValue.ItemClass.CanDrop(null) || num <= 0f)
				{
					this.state = ItemActionEntrySell.StateTypes.ItemNotSellableVending;
				}
			}
		}
		else if (!xuiC_ItemStack.ItemStack.IsEmpty())
		{
			ItemStack itemStack2 = xuiC_ItemStack.ItemStack.Clone();
			ItemClass itemClass = itemStack2.itemValue.ItemClass;
			bool sellableToTrader;
			if (itemClass.IsBlock())
			{
				sellableToTrader = Block.list[itemStack2.itemValue.type].SellableToTrader;
			}
			else
			{
				sellableToTrader = itemClass.SellableToTrader;
			}
			if (!this.isOwner)
			{
				if (!sellableToTrader)
				{
					this.state = ItemActionEntrySell.StateTypes.ItemNotSellable;
				}
				else if (XUiM_Trader.GetSellPrice(base.ItemController.xui, itemStack2.itemValue, count, itemClass) <= 0)
				{
					this.state = ItemActionEntrySell.StateTypes.PriceTooLow;
				}
			}
		}
		base.Enabled = (this.state == ItemActionEntrySell.StateTypes.Normal);
	}

	// Token: 0x06005DA0 RID: 23968 RVA: 0x002608B8 File Offset: 0x0025EAB8
	public override void OnDisabledActivate()
	{
		EntityPlayerLocal entityPlayer = base.ItemController.xui.playerUI.entityPlayer;
		switch (this.state)
		{
		case ItemActionEntrySell.StateTypes.ItemNotSellable:
		case ItemActionEntrySell.StateTypes.PriceTooLow:
			GameManager.ShowTooltip(entityPlayer, Localization.Get("ttCannotSellItem", false), false, false, 0.5f);
			if (base.ItemController.xui.Trader.TraderEntity != null)
			{
				base.ItemController.xui.Trader.TraderEntity.PlayVoiceSetEntry("refuse", entityPlayer, true, true);
			}
			break;
		case ItemActionEntrySell.StateTypes.ItemNotSellableVending:
			GameManager.ShowTooltip(entityPlayer, "You cannot sell this item.", false, false, 0f);
			break;
		}
		Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
	}

	// Token: 0x06005DA1 RID: 23969 RVA: 0x00260978 File Offset: 0x0025EB78
	public override void OnActivated()
	{
		try
		{
			EntityPlayerLocal entityPlayer = base.ItemController.xui.playerUI.entityPlayer;
			XUiC_ItemStack xuiC_ItemStack = (XUiC_ItemStack)base.ItemController;
			ItemValue itemValue = xuiC_ItemStack.ItemStack.itemValue;
			ItemClass forId = ItemClass.GetForId(xuiC_ItemStack.ItemStack.itemValue.type);
			ItemValue item = ItemClass.GetItem(TraderInfo.CurrencyItem, false);
			if (!xuiC_ItemStack.ItemStack.IsEmpty() && forId != null)
			{
				int economicBundleSize;
				if (forId.IsBlock())
				{
					economicBundleSize = Block.list[itemValue.type].EconomicBundleSize;
				}
				else
				{
					economicBundleSize = forId.EconomicBundleSize;
				}
				if (xuiC_ItemStack.ItemStack.count < economicBundleSize)
				{
					GameManager.ShowTooltip(entityPlayer, string.Format(Localization.Get("ttItemCountNotBundleSize", false), economicBundleSize), false, false, 0f);
					Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
				}
				else if (this.isOwner)
				{
					int num = xuiC_ItemStack.InfoWindow.BuySellCounter.Count;
					if (xuiC_ItemStack.ItemStack.itemValue.type == item.type)
					{
						if (base.ItemController.xui.Trader.Trader.AvailableMoney + num > 6 * base.ItemController.xui.Trader.Trader.TraderInfo.RentCost)
						{
							GameManager.ShowTooltip(entityPlayer, Localization.Get("ttVendingMachineMaxCoin", false), false, false, 0f);
							Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
							return;
						}
						base.ItemController.xui.Trader.Trader.AvailableMoney += num;
						base.ItemController.xui.Trader.TraderWindowGroup.RefreshTraderWindow();
					}
					else
					{
						int num2 = 50;
						if (base.ItemController.xui.Trader.Trader.PrimaryInventory.Count == num2)
						{
							int num3 = num;
							for (int i = 0; i < base.ItemController.xui.Trader.Trader.PrimaryInventory.Count; i++)
							{
								if (base.ItemController.xui.Trader.Trader.PrimaryInventory[i].itemValue.type == xuiC_ItemStack.ItemStack.itemValue.type)
								{
									int num4 = forId.Stacknumber.Value - base.ItemController.xui.Trader.Trader.PrimaryInventory[i].count;
									num3 -= num4;
									if (num3 <= 0)
									{
										break;
									}
								}
							}
							if (num3 > 0)
							{
								GameManager.ShowTooltip(entityPlayer, Localization.Get("ttVendingMachineMaxItems", false), false, false, 0f);
								Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
								return;
							}
						}
						int num5 = num % economicBundleSize;
						if (num5 != 0)
						{
							GameManager.ShowTooltip(entityPlayer, string.Format(Localization.Get("ttItemCountNotBundleSize", false), economicBundleSize), false, false, 0f);
							num -= num5;
							xuiC_ItemStack.InfoWindow.BuySellCounter.Count = num;
							Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
							return;
						}
						ItemStack itemStack = xuiC_ItemStack.ItemStack.Clone();
						itemStack = this.HandleRemoveAmmo(itemStack);
						itemStack.count = num;
						base.ItemController.xui.Trader.Trader.AddToPrimaryInventory(itemStack, true);
					}
					base.ItemController.xui.Trader.TraderWindowGroup.RefreshTraderItems();
					Manager.PlayInsidePlayerHead("craft_place_item", -1, 0f, false, false);
					xuiC_ItemStack.ItemStack.count -= num;
					xuiC_ItemStack.InfoWindow.BuySellCounter.MaxCount -= num;
					if (xuiC_ItemStack.ItemStack.count == 0)
					{
						xuiC_ItemStack.ItemStack = ItemStack.Empty.Clone();
						xuiC_ItemStack.Selected = false;
					}
					else
					{
						xuiC_ItemStack.ForceRefreshItemStack();
					}
				}
				else
				{
					int count = xuiC_ItemStack.InfoWindow.BuySellCounter.Count;
					int primaryItemCount = base.ItemController.xui.Trader.Trader.GetPrimaryItemCount(xuiC_ItemStack.ItemStack.itemValue);
					int num6 = Math.Min(forId.Stacknumber.Value * 3 - primaryItemCount, count);
					if (num6 <= 0)
					{
						GameManager.ShowTooltip(entityPlayer, Localization.Get("ttCannotSellItem", false), false, false, 0f);
						Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
					}
					else if (num6 != count)
					{
						GameManager.ShowTooltip(entityPlayer, string.Format(Localization.Get("ttCanOnlySellAmount", false), num6), false, false, 0f);
						xuiC_ItemStack.InfoWindow.BuySellCounter.Count = num6;
						Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
					}
					else
					{
						int num7 = num6 % economicBundleSize;
						if (num7 != 0)
						{
							GameManager.ShowTooltip(entityPlayer, string.Format(Localization.Get("ttItemCountNotBundleSize", false), economicBundleSize), false, false, 0f);
							num6 -= num7;
							xuiC_ItemStack.InfoWindow.BuySellCounter.Count = num6;
							Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
						}
						else
						{
							ItemStack itemStack2 = xuiC_ItemStack.ItemStack.Clone();
							int sellPrice = XUiM_Trader.GetSellPrice(base.ItemController.xui, itemStack2.itemValue, count, forId);
							XUiM_PlayerInventory playerInventory = base.ItemController.xui.PlayerInventory;
							ItemStack itemStack3 = new ItemStack(item.Clone(), sellPrice);
							itemStack2.count = count;
							int slotCount = xuiC_ItemStack.xui.PlayerInventory.Backpack.SlotCount;
							int slotNumber = xuiC_ItemStack.SlotNumber + ((xuiC_ItemStack.StackLocation == XUiC_ItemStack.StackLocationTypes.ToolBelt) ? slotCount : 0);
							if (playerInventory.CanSwapItems(itemStack2, itemStack3, slotNumber))
							{
								itemStack2 = this.HandleRemoveAmmo(itemStack2);
								xuiC_ItemStack.ItemStack.count -= count;
								xuiC_ItemStack.InfoWindow.BuySellCounter.MaxCount -= count;
								if (xuiC_ItemStack.ItemStack.count == 0)
								{
									xuiC_ItemStack.ItemStack = ItemStack.Empty.Clone();
									xuiC_ItemStack.Selected = false;
								}
								else
								{
									xuiC_ItemStack.ForceRefreshItemStack();
								}
								base.ItemController.xui.Trader.Trader.AddToPrimaryInventory(itemStack2, false);
								base.ItemController.xui.Trader.TraderWindowGroup.RefreshTraderItems();
								if (base.ItemController.xui.Trader.TraderEntity != null)
								{
									QuestEventManager.Current.SoldItems(base.ItemController.xui.Trader.TraderEntity.EntityName, count);
									entityPlayer.Progression.AddLevelExp(Math.Max(Convert.ToInt32(sellPrice), 1), "_xpFromSelling", Progression.XPTypes.Selling, true, true);
								}
								playerInventory.AddItem(itemStack3, false);
								Manager.PlayInsidePlayerHead("ui_trader_purchase", -1, 0f, false, false);
							}
							else
							{
								GameManager.ShowTooltip(entityPlayer, Localization.Get("ttNoSpaceForSelling", false), false, false, 0f);
								Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
							}
						}
					}
				}
			}
		}
		finally
		{
			XUiM_Trader trader = base.ItemController.xui.Trader;
			TileEntityTrader tileEntityTrader = (trader != null) ? trader.TraderTileEntity : null;
			if (tileEntityTrader != null)
			{
				tileEntityTrader.SetModified();
			}
		}
	}

	// Token: 0x06005DA2 RID: 23970 RVA: 0x002610E0 File Offset: 0x0025F2E0
	public ItemStack HandleRemoveAmmo(ItemStack stack)
	{
		if (stack.itemValue.Meta > 0)
		{
			ItemClass forId = ItemClass.GetForId(stack.itemValue.type);
			for (int i = 0; i < forId.Actions.Length; i++)
			{
				if (forId.Actions[i] is ItemActionRanged && !(forId.Actions[i] is ItemActionTextureBlock))
				{
					ItemActionRanged itemActionRanged = (ItemActionRanged)forId.Actions[i];
					if ((int)stack.itemValue.SelectedAmmoTypeIndex < itemActionRanged.MagazineItemNames.Length)
					{
						ItemStack itemStack = new ItemStack(ItemClass.GetItem(itemActionRanged.MagazineItemNames[(int)stack.itemValue.SelectedAmmoTypeIndex], false), stack.itemValue.Meta);
						if (!base.ItemController.xui.PlayerInventory.AddItem(itemStack))
						{
							base.ItemController.xui.PlayerInventory.DropItem(itemStack);
						}
						stack.itemValue.Meta = 0;
					}
				}
			}
		}
		return stack;
	}

	// Token: 0x040046CC RID: 18124
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOwner;

	// Token: 0x040046CD RID: 18125
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemActionEntrySell.StateTypes state;

	// Token: 0x02000BEB RID: 3051
	[PublicizedFrom(EAccessModifier.Private)]
	public enum StateTypes
	{
		// Token: 0x040046CF RID: 18127
		Normal,
		// Token: 0x040046D0 RID: 18128
		ItemNotSellable,
		// Token: 0x040046D1 RID: 18129
		ItemNotSellableVending,
		// Token: 0x040046D2 RID: 18130
		PriceTooLow,
		// Token: 0x040046D3 RID: 18131
		NotEnoughItems,
		// Token: 0x040046D4 RID: 18132
		NotBundleSize
	}
}
