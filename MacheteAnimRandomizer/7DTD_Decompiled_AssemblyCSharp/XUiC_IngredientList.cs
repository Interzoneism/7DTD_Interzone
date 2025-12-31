using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000CCD RID: 3277
[Preserve]
public class XUiC_IngredientList : XUiController
{
	// Token: 0x17000A58 RID: 2648
	// (get) Token: 0x0600656E RID: 25966 RVA: 0x00291D0A File Offset: 0x0028FF0A
	// (set) Token: 0x0600656F RID: 25967 RVA: 0x00291D12 File Offset: 0x0028FF12
	public Recipe Recipe
	{
		get
		{
			return this.recipe;
		}
		set
		{
			this.recipe = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000A59 RID: 2649
	// (get) Token: 0x06006570 RID: 25968 RVA: 0x00291D22 File Offset: 0x0028FF22
	// (set) Token: 0x06006571 RID: 25969 RVA: 0x00291D2A File Offset: 0x0028FF2A
	public int CraftingTier
	{
		get
		{
			return this.craftingTier;
		}
		set
		{
			this.craftingTier = value;
			this.isDirty = true;
		}
	}

	// Token: 0x06006572 RID: 25970 RVA: 0x00291D3C File Offset: 0x0028FF3C
	public override void Init()
	{
		base.Init();
		XUiController[] childrenByType = base.GetChildrenByType<XUiC_IngredientEntry>(null);
		XUiController[] array = childrenByType;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				this.ingredientEntries.Add(array[i]);
			}
		}
	}

	// Token: 0x06006573 RID: 25971 RVA: 0x00291D7A File Offset: 0x0028FF7A
	public override void OnOpen()
	{
		base.OnOpen();
		base.xui.PlayerInventory.OnBackpackItemsChanged += this.PlayerInventory_OnBackpackItemsChanged;
		base.xui.PlayerInventory.OnToolbeltItemsChanged += this.PlayerInventory_OnToolbeltItemsChanged;
	}

	// Token: 0x06006574 RID: 25972 RVA: 0x00291DBA File Offset: 0x0028FFBA
	public override void OnClose()
	{
		base.OnClose();
		base.xui.PlayerInventory.OnBackpackItemsChanged -= this.PlayerInventory_OnBackpackItemsChanged;
		base.xui.PlayerInventory.OnToolbeltItemsChanged -= this.PlayerInventory_OnToolbeltItemsChanged;
	}

	// Token: 0x06006575 RID: 25973 RVA: 0x00291DFA File Offset: 0x0028FFFA
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayerInventory_OnToolbeltItemsChanged()
	{
		this.isDirty = true;
	}

	// Token: 0x06006576 RID: 25974 RVA: 0x00291DFA File Offset: 0x0028FFFA
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayerInventory_OnBackpackItemsChanged()
	{
		this.isDirty = true;
	}

	// Token: 0x06006577 RID: 25975 RVA: 0x00291E04 File Offset: 0x00290004
	public override void Update(float _dt)
	{
		if (this.isDirty)
		{
			if (this.recipe != null)
			{
				int count = this.ingredientEntries.Count;
				int count2 = this.recipe.ingredients.Count;
				for (int i = 0; i < count; i++)
				{
					if (this.ingredientEntries[i] is XUiC_IngredientEntry)
					{
						ItemStack itemStack = (i < count2) ? this.recipe.ingredients[i].Clone() : null;
						if (itemStack != null && this.recipe.UseIngredientModifier)
						{
							itemStack.count = (int)EffectManager.GetValue(PassiveEffects.CraftingIngredientCount, null, (float)itemStack.count, base.xui.playerUI.entityPlayer, this.recipe, FastTags<TagGroup.Global>.Parse(itemStack.itemValue.ItemClass.GetItemName()), true, true, true, true, true, this.craftingTier, true, false);
						}
						if (itemStack == null || (itemStack != null && itemStack.count > 0))
						{
							((XUiC_IngredientEntry)this.ingredientEntries[i]).Ingredient = itemStack;
						}
						else
						{
							((XUiC_IngredientEntry)this.ingredientEntries[i]).Ingredient = null;
						}
					}
				}
			}
			else
			{
				int count3 = this.ingredientEntries.Count;
				for (int j = 0; j < count3; j++)
				{
					if (this.ingredientEntries[j] is XUiC_IngredientEntry)
					{
						((XUiC_IngredientEntry)this.ingredientEntries[j]).Ingredient = null;
					}
				}
			}
			this.isDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x04004C97 RID: 19607
	[PublicizedFrom(EAccessModifier.Private)]
	public Recipe recipe;

	// Token: 0x04004C98 RID: 19608
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiController> ingredientEntries = new List<XUiController>();

	// Token: 0x04004C99 RID: 19609
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x04004C9A RID: 19610
	[PublicizedFrom(EAccessModifier.Private)]
	public int craftingTier = -1;
}
