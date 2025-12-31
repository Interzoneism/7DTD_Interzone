using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DE1 RID: 3553
[Preserve]
public class XUiC_RecipeTrackerIngredientsList : XUiController
{
	// Token: 0x17000B38 RID: 2872
	// (get) Token: 0x06006F6D RID: 28525 RVA: 0x002D7D51 File Offset: 0x002D5F51
	// (set) Token: 0x06006F6E RID: 28526 RVA: 0x002D7D59 File Offset: 0x002D5F59
	public Recipe Recipe
	{
		get
		{
			return this.recipe;
		}
		set
		{
			this.recipe = value;
			Recipe recipe = this.recipe;
			this.selectedCraftingTier = ((recipe != null) ? recipe.craftingTier : -1);
			this.isDirty = true;
			this.firstSetup = true;
		}
	}

	// Token: 0x17000B39 RID: 2873
	// (get) Token: 0x06006F6F RID: 28527 RVA: 0x002D7D88 File Offset: 0x002D5F88
	// (set) Token: 0x06006F70 RID: 28528 RVA: 0x002D7D90 File Offset: 0x002D5F90
	public int Count
	{
		get
		{
			return this.count;
		}
		set
		{
			this.count = value;
			this.isDirty = true;
		}
	}

	// Token: 0x06006F71 RID: 28529 RVA: 0x002D7DA0 File Offset: 0x002D5FA0
	public override void Init()
	{
		base.Init();
		XUiC_RecipeTrackerIngredientEntry[] childrenByType = base.GetChildrenByType<XUiC_RecipeTrackerIngredientEntry>(null);
		for (int i = 0; i < childrenByType.Length; i++)
		{
			if (childrenByType[i] != null)
			{
				childrenByType[i].Owner = this;
				this.ingredientEntries.Add(childrenByType[i]);
			}
		}
	}

	// Token: 0x06006F72 RID: 28530 RVA: 0x002D7DE5 File Offset: 0x002D5FE5
	public override void OnOpen()
	{
		base.OnOpen();
		base.xui.PlayerInventory.OnBackpackItemsChanged += this.PlayerInventory_OnBackpackItemsChanged;
		base.xui.PlayerInventory.OnToolbeltItemsChanged += this.PlayerInventory_OnToolbeltItemsChanged;
	}

	// Token: 0x06006F73 RID: 28531 RVA: 0x002D7E25 File Offset: 0x002D6025
	public override void OnClose()
	{
		base.OnClose();
		base.xui.PlayerInventory.OnBackpackItemsChanged -= this.PlayerInventory_OnBackpackItemsChanged;
		base.xui.PlayerInventory.OnToolbeltItemsChanged -= this.PlayerInventory_OnToolbeltItemsChanged;
	}

	// Token: 0x06006F74 RID: 28532 RVA: 0x002D7E65 File Offset: 0x002D6065
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayerInventory_OnToolbeltItemsChanged()
	{
		this.isDirty = true;
	}

	// Token: 0x06006F75 RID: 28533 RVA: 0x002D7E65 File Offset: 0x002D6065
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayerInventory_OnBackpackItemsChanged()
	{
		this.isDirty = true;
	}

	// Token: 0x06006F76 RID: 28534 RVA: 0x002D7E70 File Offset: 0x002D6070
	public override void Update(float _dt)
	{
		if (this.isDirty)
		{
			if (this.recipe != null)
			{
				bool flag = true;
				int num = this.ingredientEntries.Count;
				int num2 = this.recipe.ingredients.Count;
				int craftingTier = (this.selectedCraftingTier == -1) ? ((int)EffectManager.GetValue(PassiveEffects.CraftingTier, null, 1f, base.xui.playerUI.entityPlayer, this.recipe, this.recipe.tags, true, true, true, true, true, 1, true, false)) : this.selectedCraftingTier;
				for (int i = 0; i < num; i++)
				{
					XUiC_RecipeTrackerIngredientEntry xuiC_RecipeTrackerIngredientEntry = this.ingredientEntries[i];
					ItemStack itemStack = (i < num2) ? this.recipe.ingredients[i].Clone() : null;
					if (itemStack != null && this.recipe.UseIngredientModifier)
					{
						itemStack.count = (int)EffectManager.GetValue(PassiveEffects.CraftingIngredientCount, null, (float)itemStack.count, base.xui.playerUI.entityPlayer, this.recipe, FastTags<TagGroup.Global>.Parse(itemStack.itemValue.ItemClass.GetItemName()), true, true, true, true, true, craftingTier, true, false);
					}
					if (itemStack == null || (itemStack != null && itemStack.count > 0))
					{
						xuiC_RecipeTrackerIngredientEntry.Ingredient = itemStack;
					}
					else
					{
						xuiC_RecipeTrackerIngredientEntry.Ingredient = null;
					}
					if (xuiC_RecipeTrackerIngredientEntry.Ingredient != null && !xuiC_RecipeTrackerIngredientEntry.IsComplete)
					{
						flag = false;
					}
				}
				if (this.firstSetup)
				{
					this.lastComplete = flag;
					this.firstSetup = false;
				}
				if (flag && !this.lastComplete)
				{
					GameManager.ShowTooltip(base.xui.playerUI.entityPlayer, "ttAllIngredientsFound", Localization.Get(this.recipe.GetName(), false), null, null, false, false, 0f);
					this.lastComplete = flag;
					this.isDirty = false;
					base.Update(_dt);
					return;
				}
				this.lastComplete = flag;
			}
			else
			{
				int num3 = this.ingredientEntries.Count;
				for (int j = 0; j < num3; j++)
				{
					this.ingredientEntries[j].Ingredient = null;
				}
			}
			this.isDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x06006F77 RID: 28535 RVA: 0x002D8090 File Offset: 0x002D6290
	public int GetActiveIngredientCount()
	{
		if (this.recipe == null)
		{
			return 0;
		}
		int craftingTier = (this.selectedCraftingTier == -1) ? ((int)EffectManager.GetValue(PassiveEffects.CraftingTier, null, 1f, base.xui.playerUI.entityPlayer, this.recipe, this.recipe.tags, true, true, true, true, true, 1, true, false)) : this.selectedCraftingTier;
		int num = 0;
		for (int i = 0; i < this.recipe.ingredients.Count; i++)
		{
			ItemStack itemStack = this.recipe.ingredients[i];
			if (this.recipe.UseIngredientModifier && (int)EffectManager.GetValue(PassiveEffects.CraftingIngredientCount, null, (float)itemStack.count, base.xui.playerUI.entityPlayer, this.recipe, FastTags<TagGroup.Global>.Parse(itemStack.itemValue.ItemClass.GetItemName()), true, true, true, true, true, craftingTier, true, false) > 0)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06006F78 RID: 28536 RVA: 0x002D8180 File Offset: 0x002D6380
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		if (name == "complete_icon")
		{
			this.completeIconName = value;
			return true;
		}
		if (name == "incomplete_icon")
		{
			this.incompleteIconName = value;
			return true;
		}
		if (name == "complete_color")
		{
			Color32 color = StringParsers.ParseColor(value);
			this.completeColor = string.Format("{0},{1},{2},{3}", new object[]
			{
				color.r,
				color.g,
				color.b,
				color.a
			});
			this.completeHexColor = Utils.ColorToHex(color);
			return true;
		}
		if (!(name == "incomplete_color"))
		{
			return base.ParseAttribute(name, value, _parent);
		}
		Color32 color2 = StringParsers.ParseColor(value);
		this.incompleteColor = string.Format("{0},{1},{2},{3}", new object[]
		{
			color2.r,
			color2.g,
			color2.b,
			color2.a
		});
		this.incompleteHexColor = Utils.ColorToHex(color2);
		return true;
	}

	// Token: 0x0400548C RID: 21644
	[PublicizedFrom(EAccessModifier.Private)]
	public Recipe recipe;

	// Token: 0x0400548D RID: 21645
	[PublicizedFrom(EAccessModifier.Private)]
	public int selectedCraftingTier;

	// Token: 0x0400548E RID: 21646
	[PublicizedFrom(EAccessModifier.Private)]
	public int count = 1;

	// Token: 0x0400548F RID: 21647
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_RecipeTrackerIngredientEntry> ingredientEntries = new List<XUiC_RecipeTrackerIngredientEntry>();

	// Token: 0x04005490 RID: 21648
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x04005491 RID: 21649
	[PublicizedFrom(EAccessModifier.Private)]
	public bool firstSetup;

	// Token: 0x04005492 RID: 21650
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lastComplete;

	// Token: 0x04005493 RID: 21651
	public string completeIconName = "";

	// Token: 0x04005494 RID: 21652
	public string incompleteIconName = "";

	// Token: 0x04005495 RID: 21653
	public string completeHexColor = "FF00FF00";

	// Token: 0x04005496 RID: 21654
	public string incompleteHexColor = "FFB400";

	// Token: 0x04005497 RID: 21655
	public string warningHexColor = "FFFF00FF";

	// Token: 0x04005498 RID: 21656
	public string inactiveHexColor = "888888FF";

	// Token: 0x04005499 RID: 21657
	public string activeHexColor = "FFFFFFFF";

	// Token: 0x0400549A RID: 21658
	public string completeColor = "0,255,0,255";

	// Token: 0x0400549B RID: 21659
	public string incompleteColor = "255, 180, 0, 255";

	// Token: 0x0400549C RID: 21660
	public string warningColor = "255,255,0,255";
}
