using System;
using UnityEngine.Scripting;

// Token: 0x02000BE1 RID: 3041
[Preserve]
public class ItemActionEntryFavorite : BaseItemActionEntry
{
	// Token: 0x06005D7E RID: 23934 RVA: 0x0025EC95 File Offset: 0x0025CE95
	public ItemActionEntryFavorite(XUiController controller, Recipe _recipe) : base(controller, "lblContextActionFavorite", "server_favorite", BaseItemActionEntry.GamepadShortCut.DPadRight, "crafting/craft_click_craft", "ui/ui_denied")
	{
		this.recipe = _recipe;
	}

	// Token: 0x06005D7F RID: 23935 RVA: 0x0025ECBC File Offset: 0x0025CEBC
	public override void OnActivated()
	{
		XUiC_RecipeEntry xuiC_RecipeEntry = (XUiC_RecipeEntry)base.ItemController;
		if (xuiC_RecipeEntry == null || xuiC_RecipeEntry.Recipe == null)
		{
			if (this.recipe != null)
			{
				CraftingManager.ToggleFavoriteRecipe(this.recipe);
			}
		}
		else
		{
			CraftingManager.ToggleFavoriteRecipe(xuiC_RecipeEntry.Recipe);
		}
		XUiC_RecipeList childByType = xuiC_RecipeEntry.WindowGroup.Controller.GetChildByType<XUiC_RecipeList>();
		if (childByType != null)
		{
			childByType.RefreshCurrentRecipes();
		}
	}

	// Token: 0x040046B9 RID: 18105
	[PublicizedFrom(EAccessModifier.Private)]
	public Recipe recipe;
}
