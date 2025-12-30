using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// Token: 0x02000293 RID: 659
public class CraftingManager
{
	// Token: 0x14000004 RID: 4
	// (add) Token: 0x060012A9 RID: 4777 RVA: 0x000745E4 File Offset: 0x000727E4
	// (remove) Token: 0x060012AA RID: 4778 RVA: 0x00074618 File Offset: 0x00072818
	public static event CraftingManager.OnRecipeUnlocked RecipeUnlocked;

	// Token: 0x060012AC RID: 4780 RVA: 0x000746B8 File Offset: 0x000728B8
	public static void InitForNewGame()
	{
		CraftingManager.ClearAllRecipes();
		CraftingManager.UnlockedRecipeList.Clear();
		CraftingManager.AlreadyCraftedList.Clear();
		CraftingManager.FavoriteRecipeList.Clear();
		CraftingManager.craftingAreaData.Clear();
	}

	// Token: 0x060012AD RID: 4781 RVA: 0x000746E7 File Offset: 0x000728E7
	public static void PostInit()
	{
		CraftingManager.cacheNonScrapableRecipes();
	}

	// Token: 0x060012AE RID: 4782 RVA: 0x000746F0 File Offset: 0x000728F0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void cacheNonScrapableRecipes()
	{
		CraftingManager.nonScrapableRecipes.Clear();
		for (int i = 0; i < CraftingManager.recipes.Count; i++)
		{
			if (!CraftingManager.recipes[i].wildcardForgeCategory)
			{
				CraftingManager.nonScrapableRecipes.Add(CraftingManager.recipes[i]);
			}
		}
	}

	// Token: 0x060012AF RID: 4783 RVA: 0x00074743 File Offset: 0x00072943
	public static void ClearAllRecipes()
	{
		CraftingManager.recipes.Clear();
	}

	// Token: 0x060012B0 RID: 4784 RVA: 0x0007474F File Offset: 0x0007294F
	public static void ClearLockedData()
	{
		CraftingManager.lockedRecipeNames.Clear();
		CraftingManager.lockedRecipeTypes.Clear();
	}

	// Token: 0x060012B1 RID: 4785 RVA: 0x00074768 File Offset: 0x00072968
	public static void ClearAllGeneralRecipes()
	{
		List<Recipe> list = new List<Recipe>(CraftingManager.recipes);
		for (int i = 0; i < list.Count; i++)
		{
			Recipe recipe = list[i];
			if (recipe.craftingArea == null || recipe.craftingArea.Length == 0)
			{
				CraftingManager.recipes.Remove(recipe);
			}
		}
	}

	// Token: 0x060012B2 RID: 4786 RVA: 0x000747BA File Offset: 0x000729BA
	public static void ClearRecipe(Recipe _r)
	{
		CraftingManager.recipes.Remove(_r);
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x000747C8 File Offset: 0x000729C8
	public static void ClearCraftAreaRecipes(string _craftArea, ItemValue _craftTool)
	{
		List<Recipe> list = new List<Recipe>(CraftingManager.recipes);
		for (int i = 0; i < list.Count; i++)
		{
			Recipe recipe = list[i];
			if (recipe.craftingArea != null && recipe.craftingArea.Equals(_craftArea) && recipe.craftingToolType == _craftTool.type)
			{
				CraftingManager.recipes.Remove(recipe);
			}
		}
	}

	// Token: 0x060012B4 RID: 4788 RVA: 0x00074829 File Offset: 0x00072A29
	public static void AddRecipe(Recipe _recipe)
	{
		CraftingManager.recipes.Add(_recipe);
		CraftingManager.bSorted = false;
	}

	// Token: 0x060012B5 RID: 4789 RVA: 0x0007483C File Offset: 0x00072A3C
	public static bool RecipeIsFavorite(Recipe _recipe)
	{
		return CraftingManager.FavoriteRecipeList.Contains(_recipe.GetName());
	}

	// Token: 0x060012B6 RID: 4790 RVA: 0x00074850 File Offset: 0x00072A50
	public static void LockRecipe(string _recipeName, CraftingManager.RecipeLockTypes locktype = CraftingManager.RecipeLockTypes.Item)
	{
		for (int i = 0; i < CraftingManager.lockedRecipeNames.list.Count; i++)
		{
			if (CraftingManager.lockedRecipeNames.list[i].EqualsCaseInsensitive(_recipeName))
			{
				List<CraftingManager.RecipeLockTypes> list = CraftingManager.lockedRecipeTypes;
				int index = i;
				list[index] |= locktype;
				return;
			}
		}
		CraftingManager.lockedRecipeNames.Add(_recipeName, 0);
		CraftingManager.lockedRecipeTypes.Add(locktype);
	}

	// Token: 0x060012B7 RID: 4791 RVA: 0x000748C0 File Offset: 0x00072AC0
	public static void UnlockRecipe(Recipe _recipe, EntityPlayer _entity)
	{
		CraftingManager.UnlockedRecipeList.Add(_recipe.GetName());
		if (CraftingManager.RecipeUnlocked != null)
		{
			CraftingManager.RecipeUnlocked(_recipe.GetName());
		}
		if (_entity != null)
		{
			_entity.SetCVar(_recipe.GetName(), 1f);
		}
	}

	// Token: 0x060012B8 RID: 4792 RVA: 0x0007490F File Offset: 0x00072B0F
	public static void UnlockRecipe(string _recipeName, EntityPlayer _entity)
	{
		CraftingManager.UnlockedRecipeList.Add(_recipeName);
		if (CraftingManager.RecipeUnlocked != null)
		{
			CraftingManager.RecipeUnlocked(_recipeName);
		}
		if (_entity != null)
		{
			_entity.SetCVar(_recipeName, 1f);
		}
	}

	// Token: 0x060012B9 RID: 4793 RVA: 0x00074944 File Offset: 0x00072B44
	public static void ToggleFavoriteRecipe(Recipe _recipe)
	{
		string name = _recipe.GetName();
		if (CraftingManager.FavoriteRecipeList.Contains(name))
		{
			CraftingManager.FavoriteRecipeList.Remove(name);
			return;
		}
		CraftingManager.FavoriteRecipeList.Add(name);
	}

	// Token: 0x060012BA RID: 4794 RVA: 0x0007497E File Offset: 0x00072B7E
	public static int GetLockedRecipeCount()
	{
		return CraftingManager.lockedRecipeNames.list.Count;
	}

	// Token: 0x060012BB RID: 4795 RVA: 0x0007498F File Offset: 0x00072B8F
	public static int GetUnlockedRecipeCount()
	{
		return CraftingManager.UnlockedRecipeList.Count;
	}

	// Token: 0x060012BC RID: 4796 RVA: 0x0007499B File Offset: 0x00072B9B
	public static List<Recipe> GetRecipes()
	{
		return new List<Recipe>(CraftingManager.recipes);
	}

	// Token: 0x060012BD RID: 4797 RVA: 0x000749A8 File Offset: 0x00072BA8
	public static Recipe GetRecipe(int hashCode)
	{
		for (int i = 0; i < CraftingManager.recipes.Count; i++)
		{
			if (CraftingManager.recipes[i].GetHashCode() == hashCode)
			{
				return CraftingManager.recipes[i];
			}
		}
		return null;
	}

	// Token: 0x060012BE RID: 4798 RVA: 0x000749EC File Offset: 0x00072BEC
	public static Recipe GetRecipe(string _itemName)
	{
		for (int i = 0; i < CraftingManager.recipes.Count; i++)
		{
			if (CraftingManager.recipes[i].GetName() == _itemName)
			{
				return CraftingManager.recipes[i];
			}
		}
		return null;
	}

	// Token: 0x060012BF RID: 4799 RVA: 0x00074A34 File Offset: 0x00072C34
	public static List<Recipe> GetRecipes(string _itemName)
	{
		List<Recipe> list = new List<Recipe>();
		for (int i = 0; i < CraftingManager.recipes.Count; i++)
		{
			if (_itemName == CraftingManager.recipes[i].GetName())
			{
				list.Add(CraftingManager.recipes[i]);
			}
		}
		return list;
	}

	// Token: 0x060012C0 RID: 4800 RVA: 0x00074A86 File Offset: 0x00072C86
	public static List<Recipe> GetAllRecipes()
	{
		return CraftingManager.recipes;
	}

	// Token: 0x060012C1 RID: 4801 RVA: 0x00074A90 File Offset: 0x00072C90
	public static List<Recipe> GetNonScrapableRecipes(string _itemName)
	{
		List<Recipe> list = new List<Recipe>();
		for (int i = 0; i < CraftingManager.nonScrapableRecipes.Count; i++)
		{
			Recipe recipe = CraftingManager.nonScrapableRecipes[i];
			if (recipe.GetName() == _itemName)
			{
				list.Add(recipe);
			}
		}
		return list;
	}

	// Token: 0x060012C2 RID: 4802 RVA: 0x00074ADC File Offset: 0x00072CDC
	public static List<Recipe> GetAllRecipes(string _itemName)
	{
		List<Recipe> list = new List<Recipe>();
		for (int i = 0; i < CraftingManager.recipes.Count; i++)
		{
			Recipe recipe = CraftingManager.recipes[i];
			if (recipe.GetName() == _itemName)
			{
				list.Add(recipe);
			}
		}
		return list;
	}

	// Token: 0x060012C3 RID: 4803 RVA: 0x00074B28 File Offset: 0x00072D28
	public static void GetFavoriteRecipesFromList(ref List<Recipe> recipeList)
	{
		List<Recipe> list = new List<Recipe>();
		for (int i = 0; i < recipeList.Count; i++)
		{
			Recipe recipe = recipeList[i];
			if (CraftingManager.FavoriteRecipeList.Contains(recipe.GetName()))
			{
				list.Add(recipe);
			}
		}
		recipeList = list;
	}

	// Token: 0x060012C4 RID: 4804 RVA: 0x00074B74 File Offset: 0x00072D74
	public static Recipe GetScrapableRecipe(ItemValue _itemValue, int _count = 1)
	{
		MaterialBlock madeOfMaterial = _itemValue.ItemClass.MadeOfMaterial;
		if (madeOfMaterial == null || madeOfMaterial.ForgeCategory == null)
		{
			return null;
		}
		ItemClass itemClass = _itemValue.ItemClass;
		if (itemClass == null)
		{
			return null;
		}
		if (itemClass.NoScrapping)
		{
			return null;
		}
		for (int i = 0; i < CraftingManager.recipes.Count; i++)
		{
			Recipe recipe = CraftingManager.recipes[i];
			if (recipe.wildcardForgeCategory)
			{
				ItemClass forId = ItemClass.GetForId(recipe.itemValueType);
				MaterialBlock madeOfMaterial2 = forId.MadeOfMaterial;
				if (madeOfMaterial2 != null && madeOfMaterial2.ForgeCategory != null && recipe.itemValueType != _itemValue.type && madeOfMaterial2.ForgeCategory.Equals(madeOfMaterial.ForgeCategory) && itemClass.GetWeight() * _count >= forId.GetWeight())
				{
					return recipe;
				}
			}
		}
		return null;
	}

	// Token: 0x060012C5 RID: 4805 RVA: 0x00074C35 File Offset: 0x00072E35
	public static void AddWorkstationData(WorkstationData workstationData)
	{
		if (CraftingManager.craftingAreaData.ContainsKey(workstationData.WorkstationName))
		{
			CraftingManager.craftingAreaData[workstationData.WorkstationName] = workstationData;
			return;
		}
		CraftingManager.craftingAreaData.Add(workstationData.WorkstationName, workstationData);
	}

	// Token: 0x060012C6 RID: 4806 RVA: 0x00074C6C File Offset: 0x00072E6C
	public static WorkstationData GetWorkstationData(string workstationName)
	{
		if (workstationName != null && CraftingManager.craftingAreaData.ContainsKey(workstationName))
		{
			return CraftingManager.craftingAreaData[workstationName];
		}
		return null;
	}

	// Token: 0x04000C3C RID: 3132
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<Recipe> recipes = new List<Recipe>();

	// Token: 0x04000C3D RID: 3133
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool bSorted;

	// Token: 0x04000C3E RID: 3134
	[PublicizedFrom(EAccessModifier.Private)]
	public static DictionaryKeyList<string, int> lockedRecipeNames = new DictionaryKeyList<string, int>();

	// Token: 0x04000C3F RID: 3135
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<CraftingManager.RecipeLockTypes> lockedRecipeTypes = new List<CraftingManager.RecipeLockTypes>();

	// Token: 0x04000C40 RID: 3136
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, WorkstationData> craftingAreaData = new CaseInsensitiveStringDictionary<WorkstationData>();

	// Token: 0x04000C41 RID: 3137
	public static HashSet<string> UnlockedRecipeList = new HashSet<string>();

	// Token: 0x04000C42 RID: 3138
	public static HashSet<string> FavoriteRecipeList = new HashSet<string>();

	// Token: 0x04000C43 RID: 3139
	public static HashSet<string> AlreadyCraftedList = new HashSet<string>();

	// Token: 0x04000C44 RID: 3140
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<Recipe> nonScrapableRecipes = new List<Recipe>();

	// Token: 0x04000C45 RID: 3141
	public static readonly ReadOnlyCollection<Recipe> NonScrapableRecipes = CraftingManager.nonScrapableRecipes.AsReadOnly();

	// Token: 0x02000294 RID: 660
	// (Invoke) Token: 0x060012C9 RID: 4809
	public delegate void OnRecipeUnlocked(string recipeName);

	// Token: 0x02000295 RID: 661
	[Flags]
	public enum RecipeLockTypes
	{
		// Token: 0x04000C47 RID: 3143
		None = 0,
		// Token: 0x04000C48 RID: 3144
		Item = 1,
		// Token: 0x04000C49 RID: 3145
		Skill = 2,
		// Token: 0x04000C4A RID: 3146
		Quest = 4
	}
}
