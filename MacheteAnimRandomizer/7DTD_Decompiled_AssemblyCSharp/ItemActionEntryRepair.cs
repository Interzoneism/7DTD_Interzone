using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000BE6 RID: 3046
[Preserve]
public class ItemActionEntryRepair : BaseItemActionEntry
{
	// Token: 0x06005D8B RID: 23947 RVA: 0x0025F8A0 File Offset: 0x0025DAA0
	public ItemActionEntryRepair(XUiController controller) : base(controller, "lblContextActionRepair", "ui_game_symbol_wrench", BaseItemActionEntry.GamepadShortCut.DPadLeft, "crafting/craft_click_craft", "ui/ui_denied")
	{
		this.lblReadBook = Localization.Get("xuiRepairMustReadBook", false);
		this.lblNeedMaterials = Localization.Get("xuiRepairMissingMats", false);
		controller.xui.PlayerInventory.OnBackpackItemsChanged += this.PlayerInventory_OnBackpackItemsChanged;
		controller.xui.PlayerInventory.OnToolbeltItemsChanged += this.PlayerInventory_OnToolbeltItemsChanged;
	}

	// Token: 0x06005D8C RID: 23948 RVA: 0x0025F923 File Offset: 0x0025DB23
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayerInventory_OnToolbeltItemsChanged()
	{
		this.RefreshEnabled();
		if (base.ParentItem != null)
		{
			base.ParentItem.MarkDirty();
		}
	}

	// Token: 0x06005D8D RID: 23949 RVA: 0x0025F923 File Offset: 0x0025DB23
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayerInventory_OnBackpackItemsChanged()
	{
		this.RefreshEnabled();
		if (base.ParentItem != null)
		{
			base.ParentItem.MarkDirty();
		}
	}

	// Token: 0x06005D8E RID: 23950 RVA: 0x0025F940 File Offset: 0x0025DB40
	public override void OnDisabledActivate()
	{
		ItemActionEntryRepair.StateTypes stateTypes = this.state;
		if (stateTypes == ItemActionEntryRepair.StateTypes.RecipeLocked)
		{
			GameManager.ShowTooltip(base.ItemController.xui.playerUI.entityPlayer, this.lblReadBook, false, false, 0f);
			return;
		}
		if (stateTypes != ItemActionEntryRepair.StateTypes.NotEnoughMaterials)
		{
			return;
		}
		GameManager.ShowTooltip(base.ItemController.xui.playerUI.entityPlayer, this.lblNeedMaterials, false, false, 0f);
		ItemClass itemClass = null;
		ItemStack.Empty.Clone();
		XUiC_EquipmentStack xuiC_EquipmentStack = base.ItemController as XUiC_EquipmentStack;
		if (xuiC_EquipmentStack != null)
		{
			if (xuiC_EquipmentStack.ItemStack.IsEmpty())
			{
				return;
			}
			itemClass = xuiC_EquipmentStack.ItemValue.ItemClass;
			ItemStack itemStack = xuiC_EquipmentStack.ItemStack;
		}
		else
		{
			XUiC_ItemStack xuiC_ItemStack = base.ItemController as XUiC_ItemStack;
			if (xuiC_ItemStack != null)
			{
				if (xuiC_ItemStack.ItemStack.IsEmpty() || xuiC_ItemStack.StackLock)
				{
					return;
				}
				itemClass = xuiC_ItemStack.ItemStack.itemValue.ItemClass;
				ItemStack itemStack2 = xuiC_ItemStack.ItemStack;
			}
		}
		if (itemClass.RepairTools != null && itemClass.RepairTools.Length > 0)
		{
			ItemClass itemClass2 = ItemClass.GetItemClass(itemClass.RepairTools[0].Value, false);
			if (itemClass2 != null)
			{
				ItemStack @is = new ItemStack(new ItemValue(itemClass2.Id, false), 0);
				base.ItemController.xui.playerUI.entityPlayer.AddUIHarvestingItem(@is, true);
			}
		}
	}

	// Token: 0x06005D8F RID: 23951 RVA: 0x0025FA90 File Offset: 0x0025DC90
	public override void RefreshEnabled()
	{
		base.RefreshEnabled();
		this.state = ItemActionEntryRepair.StateTypes.Normal;
		XUi xui = base.ItemController.xui;
		ItemClass itemClass = null;
		ItemStack itemStack = ItemStack.Empty.Clone();
		XUiC_EquipmentStack xuiC_EquipmentStack = base.ItemController as XUiC_EquipmentStack;
		if (xuiC_EquipmentStack != null)
		{
			if (xuiC_EquipmentStack.ItemStack.IsEmpty())
			{
				return;
			}
			itemClass = xuiC_EquipmentStack.ItemValue.ItemClass;
			itemStack = xuiC_EquipmentStack.ItemStack;
		}
		else
		{
			XUiC_ItemStack xuiC_ItemStack = base.ItemController as XUiC_ItemStack;
			if (xuiC_ItemStack != null)
			{
				if (xuiC_ItemStack.ItemStack.IsEmpty() || xuiC_ItemStack.StackLock)
				{
					return;
				}
				itemClass = xuiC_ItemStack.ItemStack.itemValue.ItemClass;
				itemStack = xuiC_ItemStack.ItemStack;
			}
		}
		base.Enabled = (this.state == ItemActionEntryRepair.StateTypes.Normal);
		if (!base.Enabled)
		{
			base.IconName = "ui_game_symbol_book";
			return;
		}
		ItemValue itemValue = itemStack.itemValue;
		if (itemClass.RepairTools != null && itemClass.RepairTools.Length > 0)
		{
			ItemClass itemClass2 = ItemClass.GetItemClass(itemClass.RepairTools[0].Value, false);
			if (itemClass2 != null)
			{
				int b = Convert.ToInt32(Math.Ceiling((double)((float)Mathf.CeilToInt(itemValue.UseTimes) / (float)itemClass2.RepairAmount.Value)));
				if (Mathf.Min(xui.PlayerInventory.GetItemCount(new ItemValue(itemClass2.Id, false)), b) * itemClass2.RepairAmount.Value <= 0)
				{
					this.state = ItemActionEntryRepair.StateTypes.NotEnoughMaterials;
					base.Enabled = (this.state == ItemActionEntryRepair.StateTypes.Normal);
				}
			}
		}
	}

	// Token: 0x06005D90 RID: 23952 RVA: 0x0025FC0C File Offset: 0x0025DE0C
	public override void OnActivated()
	{
		XUi xui = base.ItemController.xui;
		XUiM_PlayerInventory playerInventory = xui.PlayerInventory;
		ItemStack itemStack = ItemStack.Empty;
		XUiC_EquipmentStack xuiC_EquipmentStack = base.ItemController as XUiC_EquipmentStack;
		if (xuiC_EquipmentStack != null)
		{
			itemStack = xuiC_EquipmentStack.ItemStack;
		}
		else
		{
			XUiC_ItemStack xuiC_ItemStack = base.ItemController as XUiC_ItemStack;
			if (xuiC_ItemStack != null)
			{
				xuiC_ItemStack.TimeIntervalElapsedEvent += this.ItemActionEntryRepair_TimeIntervalElapsedEvent;
				itemStack = xuiC_ItemStack.ItemStack;
			}
		}
		ItemValue itemValue = itemStack.itemValue;
		ItemClass forId = ItemClass.GetForId(itemValue.type);
		if (forId.RepairTools != null && forId.RepairTools.Length > 0)
		{
			ItemClass itemClass = ItemClass.GetItemClass(forId.RepairTools[0].Value, false);
			if (itemClass == null)
			{
				return;
			}
			int b = Convert.ToInt32(Math.Ceiling((double)((float)Mathf.CeilToInt(itemValue.UseTimes) / (float)itemClass.RepairAmount.Value)));
			int num = Mathf.Min(playerInventory.GetItemCount(new ItemValue(itemClass.Id, false)), b);
			int num2 = num * itemClass.RepairAmount.Value;
			XUiC_CraftingWindowGroup childByType = xui.FindWindowGroupByName("crafting").GetChildByType<XUiC_CraftingWindowGroup>();
			if (childByType != null && num2 > 0)
			{
				Recipe recipe = new Recipe();
				recipe.count = 1;
				recipe.craftExpGain = Mathf.CeilToInt(forId.RepairExpMultiplier * (float)num);
				recipe.ingredients.Add(new ItemStack(new ItemValue(itemClass.Id, false), num));
				recipe.itemValueType = itemValue.type;
				recipe.craftingTime = itemClass.RepairTime.Value * (float)num;
				num2 = (int)EffectManager.GetValue(PassiveEffects.RepairAmount, null, (float)num2, xui.playerUI.entityPlayer, recipe, FastTags<TagGroup.Global>.Parse(recipe.GetName()), true, true, true, true, true, 1, true, false);
				recipe.craftingTime = (float)((int)EffectManager.GetValue(PassiveEffects.CraftingTime, null, recipe.craftingTime, xui.playerUI.entityPlayer, recipe, FastTags<TagGroup.Global>.Parse(recipe.GetName()), true, true, true, true, true, 1, true, false));
				ItemClass.GetForId(recipe.itemValueType);
				if (!childByType.AddRepairItemToQueue(recipe.craftingTime, itemValue.Clone(), num2))
				{
					this.WarnQueueFull();
					return;
				}
				XUiC_EquipmentStack xuiC_EquipmentStack2 = base.ItemController as XUiC_EquipmentStack;
				if (xuiC_EquipmentStack2 != null)
				{
					xuiC_EquipmentStack2.ItemStack = ItemStack.Empty.Clone();
					xui.PlayerEquipment.Equipment.SetPreferredItemSlot(xuiC_EquipmentStack2.SlotNumber, itemValue);
				}
				else
				{
					XUiC_ItemStack xuiC_ItemStack2 = base.ItemController as XUiC_ItemStack;
					if (xuiC_ItemStack2 != null)
					{
						xuiC_ItemStack2.ItemStack = ItemStack.Empty.Clone();
					}
				}
				playerInventory.RemoveItems(recipe.ingredients, 1, null);
				return;
			}
		}
	}

	// Token: 0x06005D91 RID: 23953 RVA: 0x0025FEB4 File Offset: 0x0025E0B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void ItemActionEntryRepair_TimeIntervalElapsedEvent(float timeLeft, XUiC_ItemStack _uiItemStack)
	{
		if (timeLeft <= 0f)
		{
			ItemStack itemStack = _uiItemStack.ItemStack.Clone();
			itemStack.itemValue.UseTimes = Mathf.Max(0f, itemStack.itemValue.UseTimes - (float)_uiItemStack.RepairAmount);
			_uiItemStack.ItemStack = itemStack;
			_uiItemStack.TimeIntervalElapsedEvent -= this.ItemActionEntryRepair_TimeIntervalElapsedEvent;
			_uiItemStack.UnlockStack();
		}
	}

	// Token: 0x06005D92 RID: 23954 RVA: 0x0025FF1C File Offset: 0x0025E11C
	[PublicizedFrom(EAccessModifier.Private)]
	public void WarnQueueFull()
	{
		string text = "No room in queue!";
		if (Localization.Exists("wrnQueueFull", false))
		{
			text = Localization.Get("wrnQueueFull", false);
		}
		GameManager.ShowTooltip(base.ItemController.xui.playerUI.entityPlayer, text, false, false, 0f);
		Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
	}

	// Token: 0x06005D93 RID: 23955 RVA: 0x0025FF7C File Offset: 0x0025E17C
	public override void DisableEvents()
	{
		base.ItemController.xui.PlayerInventory.OnBackpackItemsChanged -= this.PlayerInventory_OnBackpackItemsChanged;
		base.ItemController.xui.PlayerInventory.OnToolbeltItemsChanged -= this.PlayerInventory_OnToolbeltItemsChanged;
	}

	// Token: 0x040046BE RID: 18110
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemActionEntryRepair.StateTypes state;

	// Token: 0x040046BF RID: 18111
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblReadBook;

	// Token: 0x040046C0 RID: 18112
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblNeedMaterials;

	// Token: 0x02000BE7 RID: 3047
	[PublicizedFrom(EAccessModifier.Private)]
	public enum StateTypes
	{
		// Token: 0x040046C2 RID: 18114
		Normal,
		// Token: 0x040046C3 RID: 18115
		RecipeLocked,
		// Token: 0x040046C4 RID: 18116
		NotEnoughMaterials
	}
}
