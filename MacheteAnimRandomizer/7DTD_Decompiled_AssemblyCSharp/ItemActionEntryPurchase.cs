using System;
using System.Collections.Generic;
using Audio;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000BE4 RID: 3044
[Preserve]
public class ItemActionEntryPurchase : BaseItemActionEntry
{
	// Token: 0x06005D84 RID: 23940 RVA: 0x0025EF9C File Offset: 0x0025D19C
	public ItemActionEntryPurchase(XUiController controller) : base(controller, "lblContextActionBuy", "ui_game_symbol_coin", BaseItemActionEntry.GamepadShortCut.None, "", "ui/ui_denied")
	{
		TileEntityVendingMachine tileEntityVendingMachine = controller.xui.Trader.TraderTileEntity as TileEntityVendingMachine;
		if (tileEntityVendingMachine != null)
		{
			bool playerOwned = controller.xui.Trader.Trader.TraderInfo.PlayerOwned;
			bool rentable = controller.xui.Trader.Trader.TraderInfo.Rentable;
			this.isOwner = ((playerOwned || rentable) && tileEntityVendingMachine.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier));
			this.isVending = true;
		}
		else
		{
			this.isOwner = false;
			this.isVending = false;
		}
		if (this.isOwner)
		{
			base.ActionName = Localization.Get("lblContextActionTake", false);
			base.IconName = "ui_game_symbol_hand";
			return;
		}
		base.ActionName = Localization.Get("lblContextActionBuy", false);
		base.IconName = "ui_game_symbol_coin";
	}

	// Token: 0x06005D85 RID: 23941 RVA: 0x0025F088 File Offset: 0x0025D288
	public override void RefreshEnabled()
	{
		this.RefreshBinding();
		XUiC_TraderItemEntry xuiC_TraderItemEntry = (XUiC_TraderItemEntry)base.ItemController;
		if (xuiC_TraderItemEntry != null && xuiC_TraderItemEntry.Item != null)
		{
			int count = xuiC_TraderItemEntry.InfoWindow.BuySellCounter.Count;
			if (this.isOwner)
			{
				base.Enabled = (count > 0);
				return;
			}
			XUi xui = xuiC_TraderItemEntry.xui;
			XUiM_PlayerInventory playerInventory = xui.PlayerInventory;
			ItemStack itemStack = xuiC_TraderItemEntry.Item.Clone();
			ItemClass forId = ItemClass.GetForId(itemStack.itemValue.type);
			int buyPrice = XUiM_Trader.GetBuyPrice(xui, itemStack.itemValue, count, forId, xuiC_TraderItemEntry.SlotIndex);
			ItemValue item = ItemClass.GetItem(TraderInfo.CurrencyItem, false);
			base.Enabled = (count > 0 && playerInventory.GetItemCount(item) >= buyPrice);
		}
	}

	// Token: 0x06005D86 RID: 23942 RVA: 0x0025F148 File Offset: 0x0025D348
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshBinding()
	{
		XUiC_TraderItemEntry xuiC_TraderItemEntry = (XUiC_TraderItemEntry)base.ItemController;
		if (xuiC_TraderItemEntry != null)
		{
			int slotIndex = xuiC_TraderItemEntry.SlotIndex;
			int num = 0;
			XUi xui = xuiC_TraderItemEntry.xui;
			if (xui != null)
			{
				XUiM_Trader trader = xui.Trader;
				if (trader != null)
				{
					TraderData trader2 = trader.Trader;
					if (trader2 != null)
					{
						List<ItemStack> primaryInventory = trader2.PrimaryInventory;
						if (primaryInventory != null)
						{
							num = primaryInventory.Count;
						}
					}
				}
			}
			if (num > slotIndex)
			{
				xuiC_TraderItemEntry.Item = xuiC_TraderItemEntry.xui.Trader.Trader.PrimaryInventory[slotIndex];
				return;
			}
			xuiC_TraderItemEntry.Item = null;
		}
	}

	// Token: 0x06005D87 RID: 23943 RVA: 0x0025F1D8 File Offset: 0x0025D3D8
	public override void OnActivated()
	{
		if (base.ItemController.xui.Trader.TraderTileEntity.bWaitingForServerResponse)
		{
			return;
		}
		try
		{
			this.RefreshBinding();
			ItemStack itemStack = ((XUiC_TraderItemEntry)base.ItemController).Item.Clone();
			ItemClass forId = ItemClass.GetForId(itemStack.itemValue.type);
			ItemValue itemValue = itemStack.itemValue;
			int num = ((XUiC_TraderItemEntry)base.ItemController).InfoWindow.BuySellCounter.Count;
			int buyPrice = XUiM_Trader.GetBuyPrice(base.ItemController.xui, itemStack.itemValue, num, forId, ((XUiC_TraderItemEntry)base.ItemController).SlotIndex);
			XUi xui = base.ItemController.xui;
			XUiM_PlayerInventory playerInventory = xui.PlayerInventory;
			int economicBundleSize;
			if (forId.IsBlock())
			{
				economicBundleSize = Block.list[itemValue.type].EconomicBundleSize;
			}
			else
			{
				economicBundleSize = forId.EconomicBundleSize;
			}
			int num2 = num % economicBundleSize;
			if (num2 != 0)
			{
				GameManager.ShowTooltip(xui.playerUI.entityPlayer, string.Format(Localization.Get("ttItemCountNotBundleSize", false), economicBundleSize), false, false, 0f);
				num -= num2;
				((XUiC_TraderItemEntry)base.ItemController).InfoWindow.BuySellCounter.Count = num;
				Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
			}
			else
			{
				int num3 = playerInventory.CountAvailableSpaceForItem(itemValue, true);
				if (num > num3)
				{
					GameManager.ShowTooltip(xui.playerUI.entityPlayer, string.Format(Localization.Get("ttItemCountMoreThanAvailable", false), economicBundleSize), false, false, 0f);
					Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
				}
				else
				{
					if (num > itemStack.count)
					{
						num = itemStack.count;
					}
					if (this.isOwner)
					{
						int num4 = num;
						ItemStack itemStack2 = new ItemStack(itemStack.itemValue, num);
						if (playerInventory.AddItem(itemStack2))
						{
							ItemStack item = ((XUiC_TraderItemEntry)base.ItemController).Item;
							item.count -= num;
							((XUiC_TraderItemEntry)base.ItemController).InfoWindow.BuySellCounter.MaxCount -= num;
							((XUiC_TraderItemEntry)base.ItemController).Refresh();
							if (item.count == 0)
							{
								base.ItemController.xui.Trader.Trader.PrimaryInventory.Remove(item);
								base.ItemController.xui.Trader.Trader.RemoveMarkup(((XUiC_TraderItemEntry)base.ItemController).SlotIndex);
							}
							Manager.PlayInsidePlayerHead("craft_take_item", -1, 0f, false, false);
						}
						else if (num4 != itemStack2.count)
						{
							ItemStack item2 = ((XUiC_TraderItemEntry)base.ItemController).Item;
							if (itemStack2.count == 0)
							{
								itemStack2 = ItemStack.Empty.Clone();
								base.ItemController.xui.Trader.Trader.PrimaryInventory.Remove(item2);
								base.ItemController.xui.Trader.Trader.RemoveMarkup(((XUiC_TraderItemEntry)base.ItemController).SlotIndex);
							}
							else
							{
								item2.count -= num4 - itemStack2.count;
								((XUiC_TraderItemEntry)base.ItemController).InfoWindow.BuySellCounter.MaxCount -= num;
								((XUiC_TraderItemEntry)base.ItemController).Refresh();
							}
							Manager.PlayInsidePlayerHead("craft_take_item", -1, 0f, false, false);
						}
					}
					else
					{
						ItemStack itemStack3 = new ItemStack(ItemClass.GetItem(TraderInfo.CurrencyItem, false), buyPrice);
						itemStack.count = num;
						if (playerInventory.CanSwapItems(itemStack3, itemStack, -1))
						{
							if (playerInventory.AddItem(itemStack, false))
							{
								playerInventory.RemoveItem(itemStack3);
								base.ItemController.xui.Trader.Trader.AvailableMoney += buyPrice;
								XUiC_TraderItemEntry xuiC_TraderItemEntry = (XUiC_TraderItemEntry)base.ItemController;
								ItemStack item3 = xuiC_TraderItemEntry.Item;
								item3.count -= num;
								xuiC_TraderItemEntry.InfoWindow.BuySellCounter.MaxCount -= num;
								xuiC_TraderItemEntry.Refresh();
								if (item3.count == 0)
								{
									base.ItemController.xui.Trader.Trader.PrimaryInventory.Remove(item3);
									base.ItemController.xui.Trader.Trader.RemoveMarkup(xuiC_TraderItemEntry.SlotIndex);
								}
								if (base.ItemController.xui.Trader.TraderEntity != null)
								{
									Manager.PlayInsidePlayerHead("ui_trader_purchase", -1, 0f, false, false);
								}
								else
								{
									Manager.PlayInsidePlayerHead("ui_vending_purchase", -1, 0f, false, false);
								}
								QuestEventManager.Current.BoughtItems(this.isVending ? "" : base.ItemController.xui.Trader.TraderEntity.EntityName, num);
								GameSparksCollector.IncrementCounter(this.isVending ? GameSparksCollector.GSDataKey.VendingItemsBought : GameSparksCollector.GSDataKey.TraderItemsBought, forId.Name, num, true, GameSparksCollector.GSDataCollection.SessionUpdates);
								GameSparksCollector.IncrementCounter(this.isVending ? GameSparksCollector.GSDataKey.VendingMoneySpentOn : GameSparksCollector.GSDataKey.TraderMoneySpentOn, forId.Name, buyPrice, true, GameSparksCollector.GSDataCollection.SessionUpdates);
								GameSparksCollector.IncrementCounter(GameSparksCollector.GSDataKey.TotalMoneySpentOn, forId.Name, buyPrice, true, GameSparksCollector.GSDataCollection.SessionUpdates);
							}
						}
						else
						{
							GameManager.ShowTooltip(xui.playerUI.entityPlayer, Localization.Get("ttNoSpaceForBuying", false), false, false, 0f);
							Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
						}
					}
					base.ItemController.xui.Trader.TraderWindowGroup.RefreshTraderItems();
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

	// Token: 0x040046BC RID: 18108
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOwner;

	// Token: 0x040046BD RID: 18109
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isVending;
}
