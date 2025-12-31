using System;
using System.Collections.Generic;

// Token: 0x02000296 RID: 662
public class Recipe
{
	// Token: 0x170001F7 RID: 503
	// (get) Token: 0x060012CC RID: 4812 RVA: 0x00074C8B File Offset: 0x00072E8B
	// (set) Token: 0x060012CD RID: 4813 RVA: 0x00074C93 File Offset: 0x00072E93
	public bool IsLearnable { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x060012CE RID: 4814 RVA: 0x00074C9C File Offset: 0x00072E9C
	public List<ItemStack> GetIngredientsSummedUp()
	{
		return this.ingredients;
	}

	// Token: 0x060012D0 RID: 4816 RVA: 0x00074CD4 File Offset: 0x00072ED4
	public void Init()
	{
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < this.ingredients.Count; i++)
		{
			ItemStack itemStack = this.ingredients[i];
			if (itemStack.itemValue.ItemClass != null)
			{
				num += itemStack.itemValue.ItemClass.CraftComponentExp * (float)itemStack.count;
				num2 += itemStack.itemValue.ItemClass.CraftComponentTime * (float)itemStack.count;
			}
		}
		if (this.unlockExpGain < 0)
		{
			this.unlockExpGain = (int)(num * 2f);
		}
		if (this.craftExpGain < 0)
		{
			this.craftExpGain = (int)num;
		}
		if (this.craftingTime < 0f)
		{
			this.craftingTime = num2;
		}
		this.IsLearnable = this.tags.Test_AnySet(Recipe.LearnableRecipe);
	}

	// Token: 0x060012D1 RID: 4817 RVA: 0x00074DA8 File Offset: 0x00072FA8
	public void AddIngredient(ItemValue _itemValue, int _count)
	{
		for (int i = 0; i < this.ingredients.Count - 1; i++)
		{
			if (this.ingredients[i].itemValue.type == _itemValue.type)
			{
				this.ingredients[i].count += _count;
				return;
			}
		}
		this.ingredients.Add(new ItemStack(_itemValue, _count));
	}

	// Token: 0x060012D2 RID: 4818 RVA: 0x00074E17 File Offset: 0x00073017
	public void AddIngredients(List<ItemStack> _items)
	{
		this.ingredients.AddRange(_items);
	}

	// Token: 0x060012D3 RID: 4819 RVA: 0x00074E28 File Offset: 0x00073028
	public string GetName()
	{
		ItemClass forId = ItemClass.GetForId(this.itemValueType);
		if (forId == null)
		{
			return string.Empty;
		}
		return forId.GetItemName();
	}

	// Token: 0x060012D4 RID: 4820 RVA: 0x00074E50 File Offset: 0x00073050
	public string GetIcon()
	{
		ItemClass forId = ItemClass.GetForId(this.itemValueType);
		if (forId == null)
		{
			return string.Empty;
		}
		return forId.GetIconName();
	}

	// Token: 0x060012D5 RID: 4821 RVA: 0x00074E78 File Offset: 0x00073078
	public bool CanCraft(IList<ItemStack> _itemStack, EntityAlive _ea = null, int _craftingTier = -1)
	{
		this.craftingTier = (int)EffectManager.GetValue(PassiveEffects.CraftingTier, null, 1f, _ea, this, this.tags, true, true, true, true, true, 1, true, false);
		if (_craftingTier != -1 && _craftingTier < this.craftingTier)
		{
			this.craftingTier = _craftingTier;
		}
		for (int i = 0; i < this.ingredients.Count; i++)
		{
			ItemStack itemStack = this.ingredients[i];
			int num = itemStack.count;
			if (this.UseIngredientModifier)
			{
				num = (int)EffectManager.GetValue(PassiveEffects.CraftingIngredientCount, null, (float)itemStack.count, _ea, this, FastTags<TagGroup.Global>.Parse(itemStack.itemValue.ItemClass.GetItemName()), true, true, true, true, true, this.craftingTier, true, false);
			}
			if (num != 0)
			{
				int num2 = 0;
				while (num > 0 && num2 < _itemStack.Count)
				{
					if ((!_itemStack[num2].itemValue.HasModSlots || !_itemStack[num2].itemValue.HasMods()) && _itemStack[num2].itemValue.type == itemStack.itemValue.type)
					{
						num -= _itemStack[num2].count;
					}
					num2++;
				}
				if (num > 0)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x060012D6 RID: 4822 RVA: 0x00074FA0 File Offset: 0x000731A0
	public bool CanCraftAny(IList<ItemStack> _itemStack, EntityAlive _ea = null)
	{
		for (int i = (int)EffectManager.GetValue(PassiveEffects.CraftingTier, null, 1f, _ea, this, this.tags, true, true, true, true, true, 1, true, false); i >= 0; i--)
		{
			bool flag = true;
			for (int j = 0; j < this.ingredients.Count; j++)
			{
				ItemStack itemStack = this.ingredients[j];
				int num = itemStack.count;
				if (this.UseIngredientModifier)
				{
					num = (int)EffectManager.GetValue(PassiveEffects.CraftingIngredientCount, null, (float)itemStack.count, _ea, this, FastTags<TagGroup.Global>.Parse(itemStack.itemValue.ItemClass.GetItemName()), true, true, true, true, true, i, true, false);
				}
				if (num != 0)
				{
					int num2 = 0;
					while (num > 0 && num2 < _itemStack.Count)
					{
						if ((!_itemStack[num2].itemValue.HasModSlots || !_itemStack[num2].itemValue.HasMods()) && _itemStack[num2].itemValue.type == itemStack.itemValue.type)
						{
							num -= _itemStack[num2].count;
						}
						num2++;
					}
					if (num > 0)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060012D7 RID: 4823 RVA: 0x000750D4 File Offset: 0x000732D4
	public bool IsUnlocked(EntityPlayer _ep)
	{
		return !this.IsLearnable || EffectManager.GetValue(PassiveEffects.RecipeTagUnlocked, null, _ep.GetCVar(this.GetName()), _ep, null, this.tags, true, true, true, true, true, 1, true, false) > 0f;
	}

	// Token: 0x060012D8 RID: 4824 RVA: 0x00075118 File Offset: 0x00073318
	public int GetCraftingTier(EntityPlayer _ep)
	{
		return (int)EffectManager.GetValue(PassiveEffects.CraftingTier, null, 1f, _ep, this, this.tags, true, true, true, true, true, 1, true, false);
	}

	// Token: 0x060012D9 RID: 4825 RVA: 0x00075143 File Offset: 0x00073343
	public ItemClass GetOutputItemClass()
	{
		return ItemClass.GetForId(this.itemValueType);
	}

	// Token: 0x060012DA RID: 4826 RVA: 0x00075150 File Offset: 0x00073350
	public bool ContainsIngredients(ItemValue[] _items)
	{
		for (int i = 0; i < this.ingredients.Count; i++)
		{
			for (int j = 0; j < _items.Length; j++)
			{
				if (this.ingredients[i].itemValue.type == _items[j].type)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060012DB RID: 4827 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public bool CanBeCraftedWith(Dictionary<ItemValue, int> _items)
	{
		return false;
	}

	// Token: 0x060012DC RID: 4828 RVA: 0x000751A4 File Offset: 0x000733A4
	public override bool Equals(object _other)
	{
		return _other.GetHashCode() == this.GetHashCode();
	}

	// Token: 0x060012DD RID: 4829 RVA: 0x000751B4 File Offset: 0x000733B4
	public override int GetHashCode()
	{
		if (!this.hashCodeSetup)
		{
			int num = 0;
			for (int i = 0; i < this.ingredients.Count; i++)
			{
				num += this.ingredients[i].count;
			}
			this.hashcode = string.Concat(new string[]
			{
				this.itemValueType.ToString(),
				"_",
				this.craftingArea,
				"_",
				num.ToString()
			}).GetHashCode();
			this.hashCodeSetup = true;
		}
		return this.hashcode;
	}

	// Token: 0x060012DE RID: 4830 RVA: 0x00075249 File Offset: 0x00073449
	public override string ToString()
	{
		return string.Format("[Recipe: " + this.GetName() + "]", Array.Empty<object>());
	}

	// Token: 0x060012DF RID: 4831 RVA: 0x0007526A File Offset: 0x0007346A
	public void ModifyValue(PassiveEffects _passiveEffect, ref float _base_val, ref float _perc_val, FastTags<TagGroup.Global> tags, int _craftingTier = 1)
	{
		if (this.Effects != null)
		{
			this.Effects.ModifyValue(null, _passiveEffect, ref _base_val, ref _perc_val, (float)_craftingTier, tags, 1);
		}
	}

	// Token: 0x04000C4B RID: 3147
	[PublicizedFrom(EAccessModifier.Private)]
	public byte Version = 3;

	// Token: 0x04000C4C RID: 3148
	public static FastTags<TagGroup.Global> MaterialBased = FastTags<TagGroup.Global>.Parse("materialbased");

	// Token: 0x04000C4D RID: 3149
	public static FastTags<TagGroup.Global> LearnableRecipe = FastTags<TagGroup.Global>.Parse("learnable");

	// Token: 0x04000C4E RID: 3150
	public int itemValueType;

	// Token: 0x04000C4F RID: 3151
	public int count;

	// Token: 0x04000C50 RID: 3152
	public bool scrapable;

	// Token: 0x04000C51 RID: 3153
	public List<ItemStack> ingredients = new List<ItemStack>();

	// Token: 0x04000C52 RID: 3154
	public bool wildcardForgeCategory;

	// Token: 0x04000C53 RID: 3155
	public bool wildcardCampfireCategory;

	// Token: 0x04000C54 RID: 3156
	public bool materialBasedRecipe;

	// Token: 0x04000C55 RID: 3157
	public int craftingToolType;

	// Token: 0x04000C56 RID: 3158
	public float craftingTime;

	// Token: 0x04000C57 RID: 3159
	public string craftingArea;

	// Token: 0x04000C58 RID: 3160
	public string tooltip;

	// Token: 0x04000C59 RID: 3161
	public int unlockExpGain;

	// Token: 0x04000C5A RID: 3162
	public int craftExpGain;

	// Token: 0x04000C5B RID: 3163
	public bool UseIngredientModifier = true;

	// Token: 0x04000C5C RID: 3164
	public FastTags<TagGroup.Global> tags;

	// Token: 0x04000C5D RID: 3165
	public bool IsTrackable = true;

	// Token: 0x04000C5E RID: 3166
	public bool isQuest;

	// Token: 0x04000C5F RID: 3167
	public bool isChallenge;

	// Token: 0x04000C60 RID: 3168
	public bool IsTracked;

	// Token: 0x04000C61 RID: 3169
	public bool IsScrap;

	// Token: 0x04000C62 RID: 3170
	public int craftingTier = -1;

	// Token: 0x04000C63 RID: 3171
	public MinEffectController Effects;

	// Token: 0x04000C65 RID: 3173
	[PublicizedFrom(EAccessModifier.Private)]
	public int hashcode;

	// Token: 0x04000C66 RID: 3174
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hashCodeSetup;
}
