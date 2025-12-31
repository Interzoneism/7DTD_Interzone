using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Challenges;

// Token: 0x02000EFB RID: 3835
public class XUiM_Recipes : XUiModel
{
	// Token: 0x140000DE RID: 222
	// (add) Token: 0x060078E0 RID: 30944 RVA: 0x00312F54 File Offset: 0x00311154
	// (remove) Token: 0x060078E1 RID: 30945 RVA: 0x00312F8C File Offset: 0x0031118C
	public event XUiEvent_TrackedQuestChanged OnTrackedRecipeChanged;

	// Token: 0x17000C33 RID: 3123
	// (get) Token: 0x060078E2 RID: 30946 RVA: 0x00312FC1 File Offset: 0x003111C1
	// (set) Token: 0x060078E3 RID: 30947 RVA: 0x00312FCC File Offset: 0x003111CC
	public Recipe TrackedRecipe
	{
		get
		{
			return this.trackedRecipe;
		}
		set
		{
			if (this.trackedRecipe != null)
			{
				this.trackedRecipe.IsTracked = false;
			}
			this.trackedRecipe = value;
			if (this.trackedRecipe != null)
			{
				this.trackedRecipe.IsTracked = true;
			}
			if (this.OnTrackedRecipeChanged != null)
			{
				this.OnTrackedRecipeChanged();
			}
		}
	}

	// Token: 0x060078E4 RID: 30948 RVA: 0x0031301C File Offset: 0x0031121C
	public void SetPreviousTracked(EntityPlayerLocal player)
	{
		XUi xui = player.PlayerUI.xui;
		Quest trackedQuest = player.QuestJournal.TrackedQuest;
		XUiM_Recipes.sPreviouslyTrackedChallenge = xui.QuestTracker.TrackedChallenge;
		player.QuestJournal.TrackedQuest = null;
		xui.QuestTracker.TrackedChallenge = null;
		XUiM_Recipes.sPreviouslyTrackedQuest = trackedQuest;
	}

	// Token: 0x060078E5 RID: 30949 RVA: 0x00313070 File Offset: 0x00311270
	public void ResetToPreviousTracked(EntityPlayerLocal player)
	{
		XUi xui = player.PlayerUI.xui;
		if (XUiM_Recipes.sPreviouslyTrackedQuest != null)
		{
			if (!player.QuestJournal.QuestIsActive(XUiM_Recipes.sPreviouslyTrackedQuest))
			{
				XUiM_Recipes.sPreviouslyTrackedQuest = player.QuestJournal.FindActiveQuest();
			}
		}
		else if (XUiM_Recipes.sPreviouslyTrackedChallenge != null && !XUiM_Recipes.sPreviouslyTrackedChallenge.IsActive)
		{
			XUiM_Recipes.sPreviouslyTrackedChallenge = XUiM_Recipes.sPreviouslyTrackedChallenge.Owner.GetNextChallenge(XUiM_Recipes.sPreviouslyTrackedChallenge);
		}
		player.QuestJournal.TrackedQuest = XUiM_Recipes.sPreviouslyTrackedQuest;
		xui.QuestTracker.TrackedChallenge = XUiM_Recipes.sPreviouslyTrackedChallenge;
	}

	// Token: 0x060078E6 RID: 30950 RVA: 0x003130FE File Offset: 0x003112FE
	public void RefreshTrackedRecipe()
	{
		if (this.OnTrackedRecipeChanged != null)
		{
			this.OnTrackedRecipeChanged();
		}
	}

	// Token: 0x060078E7 RID: 30951 RVA: 0x00313113 File Offset: 0x00311313
	public static ReadOnlyCollection<Recipe> GetRecipes()
	{
		return CraftingManager.NonScrapableRecipes;
	}

	// Token: 0x060078E8 RID: 30952 RVA: 0x0031311C File Offset: 0x0031131C
	public static void FilterRecipesByCategory(string _category, ref List<Recipe> recipeList)
	{
		List<Recipe> list = new List<Recipe>();
		for (int i = 0; i < recipeList.Count; i++)
		{
			if (recipeList[i].isChallenge)
			{
				list.Add(recipeList[i]);
			}
			else if (recipeList[i].isQuest)
			{
				list.Add(recipeList[i]);
			}
			else if (recipeList[i].IsTracked)
			{
				list.Add(recipeList[i]);
			}
			else
			{
				ItemClass forId = ItemClass.GetForId(recipeList[i].itemValueType);
				if (forId != null)
				{
					string[] array;
					if (!forId.IsBlock())
					{
						array = forId.Groups;
					}
					else
					{
						array = Block.list[forId.Id].GroupNames;
					}
					if (array != null)
					{
						for (int j = 0; j < array.Length; j++)
						{
							if (array[j] != null && array[j].EqualsCaseInsensitive(_category))
							{
								list.Add(recipeList[i]);
								break;
							}
						}
					}
				}
			}
		}
		recipeList = list;
	}

	// Token: 0x060078E9 RID: 30953 RVA: 0x00313220 File Offset: 0x00311420
	public static List<Recipe> FilterRecipesByWorkstation(string _workstation, IList<Recipe> recipeList)
	{
		if (_workstation == null)
		{
			_workstation = "";
		}
		List<Recipe> list = new List<Recipe>();
		for (int i = 0; i < recipeList.Count; i++)
		{
			if (recipeList[i] != null)
			{
				if (_workstation != "")
				{
					Block blockByName = Block.GetBlockByName(_workstation, false);
					if (blockByName != null && blockByName.Properties.Values.ContainsKey("Workstation.CraftingAreaRecipes"))
					{
						string text = blockByName.Properties.Values["Workstation.CraftingAreaRecipes"];
						string[] array = new string[]
						{
							text
						};
						if (text.Contains(","))
						{
							array = text.Replace(", ", ",").Replace(" ,", ",").Replace(" , ", ",").Split(',', StringSplitOptions.None);
						}
						bool flag = false;
						for (int j = 0; j < array.Length; j++)
						{
							if (recipeList[i].craftingArea != null && recipeList[i].craftingArea.EqualsCaseInsensitive(array[j]))
							{
								list.Add(recipeList[i]);
								flag = true;
								break;
							}
							if ((recipeList[i].craftingArea == null || recipeList[i].craftingArea == "") && array[j].EqualsCaseInsensitive("player"))
							{
								list.Add(recipeList[i]);
								flag = true;
								break;
							}
						}
						if (!flag && recipeList[i].craftingArea != null && recipeList[i].craftingArea.EqualsCaseInsensitive(_workstation))
						{
							list.Add(recipeList[i]);
						}
					}
					else if (recipeList[i].craftingArea != null && recipeList[i].craftingArea.EqualsCaseInsensitive(_workstation))
					{
						list.Add(recipeList[i]);
					}
				}
				else if (recipeList[i].craftingArea == null || recipeList[i].craftingArea == "")
				{
					list.Add(recipeList[i]);
				}
			}
		}
		return list;
	}

	// Token: 0x060078EA RID: 30954 RVA: 0x0031343C File Offset: 0x0031163C
	public static List<Recipe> FilterRecipesByName(string _name, IList<Recipe> recipeList)
	{
		List<Recipe> list = new List<Recipe>();
		for (int i = 0; i < recipeList.Count; i++)
		{
			Recipe recipe = recipeList[i];
			if (!recipe.craftingArea.EqualsCaseInsensitive("assembly"))
			{
				string a;
				if (Localization.TryGet(recipe.GetName(), out a) && a.ContainsCaseInsensitive(_name))
				{
					list.Add(recipe);
				}
				else
				{
					ItemClass forId = ItemClass.GetForId(recipe.itemValueType);
					if (forId != null)
					{
						if (!forId.IsBlock())
						{
							if (forId.GetItemName().ContainsCaseInsensitive(_name))
							{
								list.Add(recipe);
							}
							else if (forId.GetLocalizedItemName().ContainsCaseInsensitive(_name))
							{
								list.Add(recipe);
							}
						}
						else
						{
							Block block = Block.list[forId.Id];
							if (block != null)
							{
								if (block.GetBlockName().ContainsCaseInsensitive(_name))
								{
									list.Add(recipe);
								}
								else if (block.GetLocalizedBlockName().ContainsCaseInsensitive(_name))
								{
									list.Add(recipe);
								}
							}
						}
					}
				}
			}
		}
		return list;
	}

	// Token: 0x060078EB RID: 30955 RVA: 0x00313534 File Offset: 0x00311734
	public static List<Recipe> FilterRecipesByIngredient(ItemStack stack, IList<Recipe> recipeList)
	{
		List<Recipe> list = new List<Recipe>();
		ItemValue[] items = new ItemValue[]
		{
			stack.itemValue
		};
		for (int i = 0; i < recipeList.Count; i++)
		{
			if (recipeList[i].ContainsIngredients(items))
			{
				list.Add(recipeList[i]);
			}
		}
		return list;
	}

	// Token: 0x060078EC RID: 30956 RVA: 0x00313588 File Offset: 0x00311788
	public static List<Recipe> FilterRecipesByItem(List<int> itemIDs, IList<Recipe> recipeList)
	{
		List<Recipe> list = new List<Recipe>();
		for (int i = 0; i < recipeList.Count; i++)
		{
			if (itemIDs.Contains(recipeList[i].itemValueType))
			{
				list.Add(recipeList[i]);
			}
		}
		return list;
	}

	// Token: 0x060078ED RID: 30957 RVA: 0x003135D0 File Offset: 0x003117D0
	public static List<Recipe> FilterRecipesByID(int itemID, IList<Recipe> recipeList)
	{
		List<Recipe> list = new List<Recipe>();
		for (int i = 0; i < recipeList.Count; i++)
		{
			if (itemID == recipeList[i].itemValueType)
			{
				list.Add(recipeList[i]);
			}
		}
		return list;
	}

	// Token: 0x060078EE RID: 30958 RVA: 0x00313611 File Offset: 0x00311811
	public static int GetCount(XUi xui)
	{
		return CraftingManager.GetRecipes().Count;
	}

	// Token: 0x060078EF RID: 30959 RVA: 0x0031361D File Offset: 0x0031181D
	public static string GetRecipeName(Recipe _recipe)
	{
		return _recipe.GetName();
	}

	// Token: 0x060078F0 RID: 30960 RVA: 0x00313625 File Offset: 0x00311825
	public static string GetRecipeSpriteName(Recipe _recipe)
	{
		return _recipe.GetIcon();
	}

	// Token: 0x060078F1 RID: 30961 RVA: 0x0031362D File Offset: 0x0031182D
	public static bool GetRecipeIsUnlocked(XUi xui, Recipe _recipe)
	{
		return _recipe.IsUnlocked(xui.playerUI.entityPlayer);
	}

	// Token: 0x060078F2 RID: 30962 RVA: 0x00313640 File Offset: 0x00311840
	public static bool GetRecipeIsUnlocked(XUi xui, string itemName)
	{
		Recipe recipe = CraftingManager.GetRecipe(itemName);
		return recipe != null && XUiM_Recipes.GetRecipeIsUnlocked(xui, recipe);
	}

	// Token: 0x060078F3 RID: 30963 RVA: 0x00313660 File Offset: 0x00311860
	public static bool GetRecipeIsUnlockable(XUi xui, Recipe _recipe)
	{
		return _recipe.IsLearnable;
	}

	// Token: 0x060078F4 RID: 30964 RVA: 0x00313668 File Offset: 0x00311868
	public static bool GetRecipeIsFavorite(XUi xui, Recipe _recipe)
	{
		return CraftingManager.RecipeIsFavorite(_recipe);
	}

	// Token: 0x060078F5 RID: 30965 RVA: 0x00313670 File Offset: 0x00311870
	public static float GetRecipeCraftTime(XUi xui, Recipe _recipe)
	{
		return EffectManager.GetValue(PassiveEffects.CraftingTime, null, _recipe.craftingTime, xui.playerUI.entityPlayer, _recipe, _recipe.tags, true, true, true, true, true, 1, true, false);
	}

	// Token: 0x060078F6 RID: 30966 RVA: 0x003136A8 File Offset: 0x003118A8
	public static int GetRecipeCraftOutputCount(XUi xui, Recipe _recipe)
	{
		return (int)EffectManager.GetValue(PassiveEffects.CraftingOutputCount, null, (float)_recipe.count, xui.playerUI.entityPlayer, _recipe, _recipe.tags, true, true, true, true, true, 1, true, false);
	}

	// Token: 0x060078F7 RID: 30967 RVA: 0x003136DF File Offset: 0x003118DF
	public static ItemStack GetRecipeOutput(Recipe _recipe)
	{
		return new ItemStack(new ItemValue(_recipe.itemValueType, false), _recipe.count);
	}

	// Token: 0x060078F8 RID: 30968 RVA: 0x003136F8 File Offset: 0x003118F8
	public static List<ItemStack> GetRecipeIngredients(Recipe _recipe)
	{
		return _recipe.GetIngredientsSummedUp();
	}

	// Token: 0x060078F9 RID: 30969 RVA: 0x00002914 File Offset: 0x00000B14
	public static void GetCurrentSlots()
	{
	}

	// Token: 0x060078FA RID: 30970 RVA: 0x00313700 File Offset: 0x00311900
	public static bool HasIngredientsForRecipe(IList<ItemStack> allItems, Recipe _recipe, EntityAlive _ea = null)
	{
		return _recipe.CanCraftAny(allItems, _ea);
	}

	// Token: 0x04005BD7 RID: 23511
	[PublicizedFrom(EAccessModifier.Private)]
	public static Quest sPreviouslyTrackedQuest;

	// Token: 0x04005BD8 RID: 23512
	[PublicizedFrom(EAccessModifier.Private)]
	public static Challenge sPreviouslyTrackedChallenge;

	// Token: 0x04005BD9 RID: 23513
	[PublicizedFrom(EAccessModifier.Private)]
	public Recipe trackedRecipe;

	// Token: 0x04005BDA RID: 23514
	public int TrackedRecipeQuality = 1;

	// Token: 0x04005BDB RID: 23515
	public int TrackedRecipeCount = 1;
}
