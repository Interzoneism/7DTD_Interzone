using System;
using System.Collections.Generic;
using Audio;
using UnityEngine.Scripting;

// Token: 0x02000BDD RID: 3037
[Preserve]
public class ItemActionEntryCraft : BaseItemActionEntry
{
	// Token: 0x06005D71 RID: 23921 RVA: 0x0025E1D4 File Offset: 0x0025C3D4
	public ItemActionEntryCraft(XUiController controller, XUiC_RecipeCraftCount recipeCraftCount, int craftingTier) : base(controller, "lblContextActionCraft", "ui_game_symbol_hammer", BaseItemActionEntry.GamepadShortCut.DPadUp, "crafting/craft_click_craft", "ui/ui_denied")
	{
		this.craftCountControl = recipeCraftCount;
		this.craftingTier = craftingTier;
		WorkstationData workstationData = CraftingManager.GetWorkstationData(((XUiC_RecipeEntry)base.ItemController).Recipe.craftingArea);
		if (workstationData != null)
		{
			base.ActionName = workstationData.CraftActionName;
			base.IconName = workstationData.CraftIcon;
			base.SoundName = workstationData.CraftSound;
		}
	}

	// Token: 0x06005D72 RID: 23922 RVA: 0x0025E264 File Offset: 0x0025C464
	public override void RefreshEnabled()
	{
		base.Enabled = true;
		XUiC_RecipeEntry xuiC_RecipeEntry = (XUiC_RecipeEntry)base.ItemController;
		Recipe recipe = xuiC_RecipeEntry.Recipe;
		if (xuiC_RecipeEntry.Recipe == null)
		{
			this.state = ItemActionEntryCraft.StateTypes.Other;
			base.Enabled = false;
			return;
		}
		if (!xuiC_RecipeEntry.IsCurrentWorkstation)
		{
			this.state = ItemActionEntryCraft.StateTypes.WrongWorkStation;
			base.Enabled = false;
			return;
		}
		if (!XUiM_Recipes.GetRecipeIsUnlocked(base.ItemController.xui, ((XUiC_RecipeEntry)base.ItemController).Recipe))
		{
			this.state = ItemActionEntryCraft.StateTypes.RecipeLocked;
			base.Enabled = false;
			return;
		}
		List<XUiC_CraftingWindowGroup> childrenByType = base.ItemController.xui.GetChildrenByType<XUiC_CraftingWindowGroup>();
		int i = 0;
		while (i < childrenByType.Count)
		{
			if (childrenByType[i].WindowGroup != null && childrenByType[i].WindowGroup.isShowing)
			{
				XUiC_CraftingWindowGroup xuiC_CraftingWindowGroup = childrenByType[i];
				if (!xuiC_CraftingWindowGroup.CraftingRequirementsValid(((XUiC_RecipeEntry)base.ItemController).Recipe))
				{
					this.state = ItemActionEntryCraft.StateTypes.Other;
					this.otherMessage = xuiC_CraftingWindowGroup.CraftingRequirementsInvalidMessage(((XUiC_RecipeEntry)base.ItemController).Recipe);
					base.Enabled = false;
					return;
				}
				break;
			}
			else
			{
				i++;
			}
		}
		if (!this.HasItems(xuiC_RecipeEntry.xui, recipe))
		{
			this.state = ItemActionEntryCraft.StateTypes.NotEnoughMaterials;
			base.Enabled = false;
			return;
		}
		if (this.craftingTier > (int)EffectManager.GetValue(PassiveEffects.CraftingTier, null, 1f, base.ItemController.xui.playerUI.entityPlayer, recipe, recipe.tags, true, true, true, true, true, 1, true, false))
		{
			this.state = ItemActionEntryCraft.StateTypes.RecipeLocked;
			base.Enabled = false;
			return;
		}
		ItemAction holdingPrimary = base.ItemController.xui.playerUI.entityPlayer.inventory.GetHoldingPrimary();
		if (holdingPrimary != null && holdingPrimary.IsActionRunning(base.ItemController.xui.playerUI.entityPlayer.inventory.holdingItemData.actionData[0]))
		{
			this.state = ItemActionEntryCraft.StateTypes.Other;
			base.Enabled = false;
			return;
		}
		if (base.ItemController.xui.isUsingItemActionEntryUse)
		{
			this.state = ItemActionEntryCraft.StateTypes.Other;
			base.Enabled = false;
		}
	}

	// Token: 0x06005D73 RID: 23923 RVA: 0x0025E470 File Offset: 0x0025C670
	public override void OnActivated()
	{
		Recipe recipe = ((XUiC_RecipeEntry)base.ItemController).Recipe;
		XUi xui = base.ItemController.xui;
		List<XUiC_CraftingWindowGroup> childrenByType = xui.GetChildrenByType<XUiC_CraftingWindowGroup>();
		XUiC_CraftingWindowGroup xuiC_CraftingWindowGroup = null;
		for (int i = 0; i < childrenByType.Count; i++)
		{
			if (childrenByType[i].WindowGroup != null && childrenByType[i].WindowGroup.isShowing)
			{
				xuiC_CraftingWindowGroup = childrenByType[i];
				break;
			}
		}
		if (xuiC_CraftingWindowGroup == null)
		{
			return;
		}
		if (XUiM_Recipes.GetRecipeIsUnlocked(base.ItemController.xui, recipe) && xuiC_CraftingWindowGroup.CraftingRequirementsValid(recipe))
		{
			Recipe recipe2 = new Recipe
			{
				itemValueType = recipe.itemValueType,
				count = XUiM_Recipes.GetRecipeCraftOutputCount(xui, recipe),
				craftingArea = recipe.craftingArea,
				craftExpGain = recipe.craftExpGain,
				craftingTime = XUiM_Recipes.GetRecipeCraftTime(xui, recipe),
				craftingToolType = recipe.craftingToolType,
				craftingTier = this.craftingTier,
				tags = recipe.tags
			};
			if (!this.HasItems(xui, recipe))
			{
				return;
			}
			bool flag = false;
			for (int j = 0; j < recipe.ingredients.Count; j++)
			{
				flag |= recipe.ingredients[j].itemValue.HasQuality;
				if (flag || this.tempIngredientList[j].count != recipe.ingredients[j].count)
				{
					recipe2.scrapable = true;
				}
			}
			recipe2.AddIngredients(this.tempIngredientList);
			XUiC_WorkstationInputGrid childByType = this.craftCountControl.WindowGroup.Controller.GetChildByType<XUiC_WorkstationInputGrid>();
			if (xuiC_CraftingWindowGroup.AddItemToQueue(recipe2))
			{
				if (flag)
				{
					this.tempIngredientList.Clear();
				}
				if (childByType != null)
				{
					childByType.RemoveItems(recipe2.ingredients, this.craftCountControl.Count, this.tempIngredientList);
				}
				else
				{
					xui.PlayerInventory.RemoveItems(recipe2.ingredients, this.craftCountControl.Count, this.tempIngredientList);
				}
				if (flag)
				{
					recipe2.ingredients.Clear();
					recipe2.AddIngredients(this.tempIngredientList);
				}
				if (recipe == xui.Recipes.TrackedRecipe)
				{
					xui.Recipes.TrackedRecipe = null;
					xui.Recipes.ResetToPreviousTracked(xui.playerUI.entityPlayer);
				}
			}
			else
			{
				this.WarnQueueFull();
			}
		}
		this.craftCountControl.IsDirty = true;
		xuiC_CraftingWindowGroup.WindowGroup.Controller.SetAllChildrenDirty(false);
	}

	// Token: 0x06005D74 RID: 23924 RVA: 0x0025E6E8 File Offset: 0x0025C8E8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool HasItems(XUi xui, Recipe recipe)
	{
		bool flag = false;
		List<ItemStack> allItemStacks = xui.PlayerInventory.GetAllItemStacks();
		this.tempIngredientList.Clear();
		for (int i = 0; i < recipe.ingredients.Count; i++)
		{
			int num = recipe.ingredients[i].count;
			if (recipe.UseIngredientModifier)
			{
				num = (int)EffectManager.GetValue(PassiveEffects.CraftingIngredientCount, null, (float)recipe.ingredients[i].count, xui.playerUI.entityPlayer, recipe, FastTags<TagGroup.Global>.Parse(recipe.ingredients[i].itemValue.ItemClass.GetItemName()), true, true, true, true, true, this.craftingTier, true, false);
			}
			if (!recipe.ingredients[i].itemValue.HasQuality)
			{
				this.tempIngredientList.Add(new ItemStack(recipe.ingredients[i].itemValue, num));
			}
			else
			{
				int num2 = (num == 0) ? 1 : num;
				this.tempIngredientList.Add(new ItemStack(recipe.ingredients[i].itemValue.Clone(), num2));
				for (int j = 0; j < allItemStacks.Count; j++)
				{
					ItemStack itemStack = allItemStacks[j];
					if ((!itemStack.itemValue.HasModSlots || !itemStack.itemValue.HasMods()) && itemStack.itemValue.type == recipe.ingredients[i].itemValue.type)
					{
						num2--;
						if (num2 == 0)
						{
							break;
						}
					}
				}
				if (num2 > 0)
				{
					return false;
				}
			}
		}
		flag |= xui.PlayerInventory.HasItems(this.tempIngredientList, this.craftCountControl.Count);
		XUiC_WorkstationInputGrid childByType = this.craftCountControl.WindowGroup.Controller.GetChildByType<XUiC_WorkstationInputGrid>();
		if (childByType != null)
		{
			flag |= childByType.HasItems(this.tempIngredientList, this.craftCountControl.Count);
		}
		return flag;
	}

	// Token: 0x06005D75 RID: 23925 RVA: 0x0025E8D4 File Offset: 0x0025CAD4
	public override void OnDisabledActivate()
	{
		EntityPlayerLocal entityPlayer = base.ItemController.xui.playerUI.entityPlayer;
		switch (this.state)
		{
		case ItemActionEntryCraft.StateTypes.RecipeLocked:
			GameManager.ShowTooltip(entityPlayer, Localization.Get("ttMissingCraftingRecipe", false), false, false, 0f);
			return;
		case ItemActionEntryCraft.StateTypes.NotEnoughMaterials:
			GameManager.ShowTooltip(entityPlayer, Localization.Get("ttMissingCraftingResources", false), false, false, 0f);
			return;
		case ItemActionEntryCraft.StateTypes.WrongWorkStation:
			break;
		case ItemActionEntryCraft.StateTypes.Other:
			GameManager.ShowTooltip(entityPlayer, this.otherMessage, false, false, 0f);
			break;
		default:
			return;
		}
	}

	// Token: 0x06005D76 RID: 23926 RVA: 0x0025E95C File Offset: 0x0025CB5C
	[PublicizedFrom(EAccessModifier.Private)]
	public void WarnQueueFull()
	{
		GameManager.ShowTooltip(base.ItemController.xui.playerUI.entityPlayer, Localization.Get("xuiCraftQueueFull", false), false, false, 0f);
		Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
	}

	// Token: 0x040046AE RID: 18094
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemActionEntryCraft.StateTypes state;

	// Token: 0x040046AF RID: 18095
	[PublicizedFrom(EAccessModifier.Private)]
	public string otherMessage = "";

	// Token: 0x040046B0 RID: 18096
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int craftingTier;

	// Token: 0x040046B1 RID: 18097
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly XUiC_RecipeCraftCount craftCountControl;

	// Token: 0x040046B2 RID: 18098
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<ItemStack> tempIngredientList = new List<ItemStack>();

	// Token: 0x02000BDE RID: 3038
	[PublicizedFrom(EAccessModifier.Private)]
	public enum StateTypes
	{
		// Token: 0x040046B4 RID: 18100
		Normal,
		// Token: 0x040046B5 RID: 18101
		RecipeLocked,
		// Token: 0x040046B6 RID: 18102
		NotEnoughMaterials,
		// Token: 0x040046B7 RID: 18103
		WrongWorkStation,
		// Token: 0x040046B8 RID: 18104
		Other
	}
}
