using System;
using UnityEngine.Scripting;

// Token: 0x02000BF0 RID: 3056
[Preserve]
public class ItemActionEntryTrackRecipe : BaseItemActionEntry
{
	// Token: 0x06005DB0 RID: 23984 RVA: 0x00261574 File Offset: 0x0025F774
	public ItemActionEntryTrackRecipe(XUiController controller, XUiC_RecipeCraftCount recipeCraftCount, int craftingTier) : base(controller, "lblContextActionTrack", "ui_game_symbol_compass", BaseItemActionEntry.GamepadShortCut.DPadLeft, "crafting/craft_click_craft", "ui/ui_denied")
	{
		this.craftCountControl = recipeCraftCount;
		Recipe recipe = ((XUiC_RecipeEntry)base.ItemController).Recipe;
		recipe.craftingTier = craftingTier;
		this.selectedCraftingTier = craftingTier;
	}

	// Token: 0x06005DB1 RID: 23985 RVA: 0x002615CC File Offset: 0x0025F7CC
	public override void OnActivated()
	{
		Recipe recipe = ((XUiC_RecipeEntry)base.ItemController).Recipe;
		recipe.craftingTier = this.selectedCraftingTier;
		XUi xui = base.ItemController.xui;
		EntityPlayerLocal entityPlayer = xui.playerUI.entityPlayer;
		if (xui.Recipes.TrackedRecipe != recipe || xui.Recipes.TrackedRecipeQuality != this.selectedCraftingTier)
		{
			if (xui.Recipes.TrackedRecipe == null)
			{
				xui.Recipes.SetPreviousTracked(entityPlayer);
			}
			xui.Recipes.TrackedRecipeQuality = this.selectedCraftingTier;
			xui.Recipes.TrackedRecipeCount = 1;
			xui.Recipes.TrackedRecipe = recipe;
			return;
		}
		xui.Recipes.TrackedRecipe = null;
		xui.Recipes.ResetToPreviousTracked(entityPlayer);
	}

	// Token: 0x040046D7 RID: 18135
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_RecipeCraftCount craftCountControl;

	// Token: 0x040046D8 RID: 18136
	[PublicizedFrom(EAccessModifier.Private)]
	public int selectedCraftingTier = 1;
}
