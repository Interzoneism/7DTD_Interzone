using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000BE5 RID: 3045
[Preserve]
public class ItemActionEntryRecipes : BaseItemActionEntry
{
	// Token: 0x06005D88 RID: 23944 RVA: 0x0025F7B0 File Offset: 0x0025D9B0
	public ItemActionEntryRecipes(XUiController controller) : base(controller, "lblContextActionRecipes", "ui_game_symbol_hammer", BaseItemActionEntry.GamepadShortCut.DPadLeft, "crafting/craft_click_craft", "ui/ui_denied")
	{
	}

	// Token: 0x06005D89 RID: 23945 RVA: 0x0025F7CE File Offset: 0x0025D9CE
	public override void RefreshEnabled()
	{
		base.Enabled = true;
	}

	// Token: 0x06005D8A RID: 23946 RVA: 0x0025F7D8 File Offset: 0x0025D9D8
	public override void OnActivated()
	{
		XUi xui = base.ItemController.xui;
		xui.playerUI.windowManager.CloseIfOpen("looting");
		List<XUiC_RecipeList> childrenByType = xui.GetChildrenByType<XUiC_RecipeList>();
		XUiC_RecipeList xuiC_RecipeList = null;
		for (int i = 0; i < childrenByType.Count; i++)
		{
			if (childrenByType[i].WindowGroup != null && childrenByType[i].WindowGroup.isShowing)
			{
				xuiC_RecipeList = childrenByType[i];
				break;
			}
		}
		if (xuiC_RecipeList == null)
		{
			XUiC_WindowSelector.OpenSelectorAndWindow(xui.playerUI.entityPlayer, "crafting");
			xuiC_RecipeList = xui.GetChildByType<XUiC_RecipeList>();
		}
		ItemStack recipeDataByIngredientStack = ItemStack.Empty.Clone();
		XUiC_ItemStack xuiC_ItemStack = base.ItemController as XUiC_ItemStack;
		if (xuiC_ItemStack != null)
		{
			recipeDataByIngredientStack = xuiC_ItemStack.ItemStack;
		}
		if (xuiC_RecipeList != null)
		{
			xuiC_RecipeList.SetRecipeDataByIngredientStack(recipeDataByIngredientStack);
		}
	}
}
