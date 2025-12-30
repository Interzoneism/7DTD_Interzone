using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DDB RID: 3547
[Preserve]
public class XUiC_RecipeList : XUiController
{
	// Token: 0x17000B28 RID: 2856
	// (get) Token: 0x06006F00 RID: 28416 RVA: 0x002D4DAD File Offset: 0x002D2FAD
	// (set) Token: 0x06006F01 RID: 28417 RVA: 0x002D4DB8 File Offset: 0x002D2FB8
	public string Workstation
	{
		get
		{
			return this.workStation;
		}
		set
		{
			this.workStation = value;
			Block blockByName = Block.GetBlockByName(this.workStation, false);
			if (blockByName != null && blockByName.Properties.Values.ContainsKey("Workstation.CraftingAreaRecipes"))
			{
				string text = blockByName.Properties.Values["Workstation.CraftingAreaRecipes"];
				this.craftingArea = new string[]
				{
					text
				};
				if (text.Contains(","))
				{
					this.craftingArea = text.Replace("player", "").Replace(", ", ",").Replace(" ,", ",").Replace(" , ", ",").Split(',', StringSplitOptions.None);
				}
			}
			else
			{
				this.craftingArea = new string[]
				{
					this.workStation
				};
			}
			this.GetRecipeData();
			this.IsDirty = true;
		}
	}

	// Token: 0x17000B29 RID: 2857
	// (get) Token: 0x06006F02 RID: 28418 RVA: 0x002D4E9B File Offset: 0x002D309B
	// (set) Token: 0x06006F03 RID: 28419 RVA: 0x002D4EA3 File Offset: 0x002D30A3
	public Recipe CurrentRecipe { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000B2A RID: 2858
	// (get) Token: 0x06006F04 RID: 28420 RVA: 0x002D4EAC File Offset: 0x002D30AC
	// (set) Token: 0x06006F05 RID: 28421 RVA: 0x002D4EB4 File Offset: 0x002D30B4
	public XUiC_RecipeCraftCount CraftCount { get; set; }

	// Token: 0x17000B2B RID: 2859
	// (get) Token: 0x06006F06 RID: 28422 RVA: 0x002D4EBD File Offset: 0x002D30BD
	// (set) Token: 0x06006F07 RID: 28423 RVA: 0x002D4EC8 File Offset: 0x002D30C8
	public int Page
	{
		get
		{
			return this.page;
		}
		set
		{
			if (this.page != value)
			{
				this.page = value;
				XUiC_Paging xuiC_Paging = this.pager;
				if (xuiC_Paging != null)
				{
					xuiC_Paging.SetPage(this.page);
				}
				if (this.PageNumberChanged != null)
				{
					this.PageNumberChanged(this.page);
				}
				this.IsDirty = true;
				this.pageChanged = true;
				this.CurrentRecipe = null;
			}
		}
	}

	// Token: 0x17000B2C RID: 2860
	// (get) Token: 0x06006F08 RID: 28424 RVA: 0x002D4F2A File Offset: 0x002D312A
	// (set) Token: 0x06006F09 RID: 28425 RVA: 0x002D4F32 File Offset: 0x002D3132
	public XUiC_CraftingInfoWindow InfoWindow { get; set; }

	// Token: 0x17000B2D RID: 2861
	// (get) Token: 0x06006F0A RID: 28426 RVA: 0x002D4F3B File Offset: 0x002D313B
	// (set) Token: 0x06006F0B RID: 28427 RVA: 0x002D4F44 File Offset: 0x002D3144
	public XUiC_RecipeEntry SelectedEntry
	{
		get
		{
			return this.selectedEntry;
		}
		set
		{
			if (this.selectedEntry != null)
			{
				this.selectedEntry.Selected = false;
			}
			this.selectedEntry = value;
			if (this.selectedEntry != null)
			{
				this.selectedEntry.Selected = true;
				this.InfoWindow.ViewComponent.IsVisible = true;
				this.RecipeChanged(this.selectedEntry.Recipe, this.selectedEntry);
				this.InfoWindow.SetRecipe(this.selectedEntry);
				this.CurrentRecipe = this.selectedEntry.Recipe;
			}
			else
			{
				this.InfoWindow.SetRecipe(null);
			}
			this.IsDirty = true;
			this.pageChanged = true;
		}
	}

	// Token: 0x140000BA RID: 186
	// (add) Token: 0x06006F0C RID: 28428 RVA: 0x002D4FEC File Offset: 0x002D31EC
	// (remove) Token: 0x06006F0D RID: 28429 RVA: 0x002D5024 File Offset: 0x002D3224
	public event XUiEvent_RecipeChangedEventHandler RecipeChanged;

	// Token: 0x140000BB RID: 187
	// (add) Token: 0x06006F0E RID: 28430 RVA: 0x002D505C File Offset: 0x002D325C
	// (remove) Token: 0x06006F0F RID: 28431 RVA: 0x002D5094 File Offset: 0x002D3294
	public event XUiEvent_PageNumberChangedEventHandler PageNumberChanged;

	// Token: 0x06006F10 RID: 28432 RVA: 0x002D50CC File Offset: 0x002D32CC
	public override void Init()
	{
		base.Init();
		this.windowGroup.Controller.GetChildByType<XUiC_CategoryList>().CategoryChanged += this.HandleCategoryChanged;
		this.pager = base.Parent.GetChildByType<XUiC_Paging>();
		if (this.pager != null)
		{
			this.pager.OnPageChanged += delegate()
			{
				this.Page = this.pager.CurrentPageNumber;
			};
		}
		this.recipeControls = base.GetChildrenByType<XUiC_RecipeEntry>(null);
		for (int i = 0; i < this.recipeControls.Length; i++)
		{
			XUiC_RecipeEntry xuiC_RecipeEntry = this.recipeControls[i];
			xuiC_RecipeEntry.OnScroll += this.HandleOnScroll;
			xuiC_RecipeEntry.OnPress += this.OnPressRecipe;
			xuiC_RecipeEntry.RecipeList = this;
		}
		this.parent.OnScroll += this.HandleOnScroll;
		XUiController childById = base.Parent.GetChildById("favorites");
		childById.OnPress += this.HandleFavoritesChanged;
		this.favorites = (XUiV_Button)childById.ViewComponent;
		XUiV_Grid xuiV_Grid = (XUiV_Grid)base.ViewComponent;
		if (xuiV_Grid != null)
		{
			this.length = xuiV_Grid.Columns * xuiV_Grid.Rows;
		}
		this.txtInput = (XUiC_TextInput)this.windowGroup.Controller.GetChildById("searchInput");
		if (this.txtInput != null)
		{
			this.txtInput.OnChangeHandler += this.HandleOnChangeHandler;
			this.txtInput.OnSubmitHandler += this.HandleOnSubmitHandler;
		}
		this.IsDirty = true;
		this.resortRecipes = true;
		this.pageChanged = true;
	}

	// Token: 0x06006F11 RID: 28433 RVA: 0x002D525A File Offset: 0x002D345A
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleFavoritesChanged(XUiController _sender, int _mouseButton)
	{
		this.showFavorites = !this.showFavorites;
		this.favorites.Selected = this.showFavorites;
		this.GetRecipeData();
		this.IsDirty = true;
		this.resortRecipes = true;
		this.pageChanged = true;
	}

	// Token: 0x06006F12 RID: 28434 RVA: 0x002D5297 File Offset: 0x002D3497
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnScroll(XUiController _sender, float _delta)
	{
		if (_delta > 0f)
		{
			XUiC_Paging xuiC_Paging = this.pager;
			if (xuiC_Paging == null)
			{
				return;
			}
			xuiC_Paging.PageDown();
			return;
		}
		else
		{
			XUiC_Paging xuiC_Paging2 = this.pager;
			if (xuiC_Paging2 == null)
			{
				return;
			}
			xuiC_Paging2.PageUp();
			return;
		}
	}

	// Token: 0x06006F13 RID: 28435 RVA: 0x002D52C4 File Offset: 0x002D34C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnSubmitHandler(XUiController _sender, string _text)
	{
		this.GetRecipeData();
	}

	// Token: 0x06006F14 RID: 28436 RVA: 0x002D52C4 File Offset: 0x002D34C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		this.GetRecipeData();
	}

	// Token: 0x06006F15 RID: 28437 RVA: 0x002D52CC File Offset: 0x002D34CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPressRecipe(XUiController _sender, int _mouseButton)
	{
		XUiC_RecipeEntry xuiC_RecipeEntry = _sender as XUiC_RecipeEntry;
		if (xuiC_RecipeEntry != null && this.RecipeChanged != null)
		{
			this.SelectedEntry = xuiC_RecipeEntry;
			if (InputUtils.ShiftKeyPressed)
			{
				this.CraftCount.SetToMaxCount();
			}
		}
	}

	// Token: 0x06006F16 RID: 28438 RVA: 0x002D5304 File Offset: 0x002D3504
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleCategoryChanged(XUiC_CategoryEntry _categoryEntry)
	{
		this.SetCategory(_categoryEntry.CategoryName);
		this.IsDirty = true;
	}

	// Token: 0x06006F17 RID: 28439 RVA: 0x002D5319 File Offset: 0x002D3519
	public void SetCategory(string _category)
	{
		if (this.txtInput != null)
		{
			this.txtInput.Text = "";
		}
		this.category = _category;
		this.GetRecipeData();
		this.IsDirty = true;
		this.resortRecipes = true;
		this.pageChanged = true;
	}

	// Token: 0x06006F18 RID: 28440 RVA: 0x002D5355 File Offset: 0x002D3555
	public string GetCategory()
	{
		return this.category;
	}

	// Token: 0x06006F19 RID: 28441 RVA: 0x002D52C4 File Offset: 0x002D34C4
	public void RefreshRecipes()
	{
		this.GetRecipeData();
	}

	// Token: 0x06006F1A RID: 28442 RVA: 0x002D535D File Offset: 0x002D355D
	public void RefreshCurrentRecipes()
	{
		this.IsDirty = true;
		this.pageChanged = true;
		if (this.showFavorites)
		{
			CraftingManager.GetFavoriteRecipesFromList(ref this.recipes);
		}
	}

	// Token: 0x06006F1B RID: 28443 RVA: 0x002D5380 File Offset: 0x002D3580
	[PublicizedFrom(EAccessModifier.Private)]
	public void GetRecipeData()
	{
		ReadOnlyCollection<Recipe> readOnlyCollection = XUiM_Recipes.GetRecipes();
		List<string> questRecipes = base.xui.playerUI.entityPlayer.QuestJournal.GetQuestRecipes();
		List<Recipe> list = (base.xui.QuestTracker.TrackedChallenge != null) ? base.xui.QuestTracker.TrackedChallenge.CraftedRecipes() : null;
		if (questRecipes.Count > 0 || (list != null && list.Count > 0))
		{
			for (int i = 0; i < readOnlyCollection.Count; i++)
			{
				if (list != null && list.Contains(readOnlyCollection[i]))
				{
					readOnlyCollection[i].isChallenge = true;
					readOnlyCollection[i].isQuest = false;
				}
				else if (questRecipes.Contains(readOnlyCollection[i].GetName()))
				{
					readOnlyCollection[i].isQuest = true;
					readOnlyCollection[i].isChallenge = false;
				}
				else
				{
					readOnlyCollection[i].isQuest = false;
					readOnlyCollection[i].isChallenge = false;
				}
			}
		}
		else
		{
			for (int j = 0; j < readOnlyCollection.Count; j++)
			{
				readOnlyCollection[j].isQuest = false;
				readOnlyCollection[j].isChallenge = false;
			}
		}
		if (this.txtInput != null && this.txtInput.Text.Length > 0)
		{
			this.recipes = XUiM_Recipes.FilterRecipesByName(this.txtInput.Text, XUiM_Recipes.GetRecipes());
		}
		else
		{
			this.recipes = XUiM_Recipes.FilterRecipesByWorkstation(this.workStation, readOnlyCollection);
			if (this.showFavorites)
			{
				CraftingManager.GetFavoriteRecipesFromList(ref this.recipes);
			}
			else if (this.category != "")
			{
				XUiM_Recipes.FilterRecipesByCategory(this.category, ref this.recipes);
			}
		}
		this.Page = 0;
		this.IsDirty = true;
		this.resortRecipes = true;
		this.pageChanged = true;
	}

	// Token: 0x06006F1C RID: 28444 RVA: 0x002D5554 File Offset: 0x002D3754
	public void SetRecipeDataByIngredientStack(ItemStack stack)
	{
		if (this.txtInput != null)
		{
			this.txtInput.Text = "";
		}
		this.CurrentRecipe = null;
		this.recipes = XUiM_Recipes.FilterRecipesByIngredient(stack, XUiM_Recipes.GetRecipes());
		this.Page = 0;
		this.IsDirty = true;
		this.resortRecipes = true;
		this.pageChanged = true;
	}

	// Token: 0x06006F1D RID: 28445 RVA: 0x002D55B0 File Offset: 0x002D37B0
	public void SetRecipeDataByItems(List<int> items)
	{
		if (items.Count == 0)
		{
			return;
		}
		if (this.txtInput != null)
		{
			this.txtInput.Text = "";
		}
		this.CurrentRecipe = null;
		this.recipes = XUiM_Recipes.FilterRecipesByItem(items, XUiM_Recipes.GetRecipes());
		this.Page = 0;
		this.IsDirty = true;
		this.resortRecipes = true;
		this.pageChanged = true;
	}

	// Token: 0x06006F1E RID: 28446 RVA: 0x002D5614 File Offset: 0x002D3814
	public void SetRecipeDataByItem(int itemID)
	{
		if (this.txtInput != null)
		{
			this.txtInput.Text = "";
		}
		this.CurrentRecipe = null;
		this.recipes = XUiM_Recipes.FilterRecipesByID(itemID, XUiM_Recipes.GetRecipes());
		if (this.recipes != null && this.recipes.Count > 0)
		{
			this.CurrentRecipe = this.recipes[0];
		}
		this.Page = 0;
		this.IsDirty = true;
		this.resortRecipes = true;
		this.pageChanged = true;
	}

	// Token: 0x06006F1F RID: 28447 RVA: 0x002D5698 File Offset: 0x002D3898
	public override void Update(float _dt)
	{
		if (this.IsDirty && base.xui.PlayerInventory != null)
		{
			this.FindShowingWindow();
			if (this.resortRecipes)
			{
				List<ItemStack> list = this.updateStackList;
				list.Clear();
				list.AddRange(base.xui.PlayerInventory.GetBackpackItemStacks());
				list.AddRange(base.xui.PlayerInventory.GetToolbeltItemStacks());
				XUiC_WorkstationInputGrid childByType = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationInputGrid>();
				if (childByType != null)
				{
					list.Clear();
					list.AddRange(childByType.GetSlots());
				}
				this.BuildRecipeInfosList(list);
				this.recipeInfos.Sort(new Comparison<XUiC_RecipeList.RecipeInfo>(this.CompareRecipeInfos));
				this.UpdateRecipes();
				this.resortRecipes = false;
			}
			if (this.pageChanged)
			{
				for (int i = 0; i < this.length; i++)
				{
					int num = i + this.length * this.page;
					XUiC_RecipeEntry xuiC_RecipeEntry = (i < this.recipeControls.Length) ? this.recipeControls[i] : null;
					if (xuiC_RecipeEntry != null)
					{
						if (num < this.recipeInfos.Count)
						{
							XUiC_RecipeList.RecipeInfo recipeInfo = this.recipeInfos[num];
							xuiC_RecipeEntry.SetRecipeAndHasIngredients(recipeInfo.recipe, recipeInfo.hasIngredients);
							xuiC_RecipeEntry.ViewComponent.Enabled = true;
						}
						else
						{
							xuiC_RecipeEntry.SetRecipeAndHasIngredients(null, false);
							xuiC_RecipeEntry.ViewComponent.Enabled = false;
							if (xuiC_RecipeEntry.Selected)
							{
								xuiC_RecipeEntry.Selected = false;
							}
						}
						if (this.CurrentRecipe != null && this.CurrentRecipe == xuiC_RecipeEntry.Recipe && this.SelectedEntry != xuiC_RecipeEntry)
						{
							this.SelectedEntry = xuiC_RecipeEntry;
							this.CraftCount.IsDirty = true;
						}
						if (this.SelectedEntry != null && this.SelectedEntry.Recipe != this.CurrentRecipe)
						{
							this.ClearSelection();
						}
					}
				}
				this.pageChanged = false;
			}
			if (this.pager != null)
			{
				this.pager.SetLastPageByElementsAndPageLength(this.recipeInfos.Count, this.recipeControls.Length);
				this.pager.CurrentPageNumber = this.page;
			}
			this.IsDirty = false;
		}
		base.Update(_dt);
		if (base.xui.playerUI.playerInput.GUIActions.Inspect.WasPressed && base.xui.playerUI.CursorController.navigationTarget != null)
		{
			this.OnPressRecipe(base.xui.playerUI.CursorController.navigationTarget.Controller, 0);
		}
	}

	// Token: 0x06006F20 RID: 28448 RVA: 0x002D5910 File Offset: 0x002D3B10
	public override void OnOpen()
	{
		if (base.ViewComponent != null && !base.ViewComponent.IsVisible)
		{
			base.ViewComponent.IsVisible = true;
		}
		if (base.xui.PlayerInventory != null)
		{
			base.xui.PlayerInventory.OnBackpackItemsChanged += this.PlayerInventory_OnBackpackItemsChanged;
			base.xui.PlayerInventory.OnToolbeltItemsChanged += this.PlayerInventory_OnToolbeltItemsChanged;
		}
		base.xui.playerUI.entityPlayer.QuestChanged += this.QuestJournal_QuestChanged;
		base.xui.playerUI.entityPlayer.QuestRemoved += this.QuestJournal_QuestChanged;
		base.xui.QuestTracker.OnTrackedChallengeChanged += this.QuestTracker_OnTrackedChallengeChanged;
		base.xui.Recipes.OnTrackedRecipeChanged += this.Recipes_OnTrackedRecipeChanged;
		XUiC_WorkstationMaterialInputWindow childByType = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationMaterialInputWindow>();
		if (childByType != null)
		{
			childByType.OnWorkstationMaterialWeightsChanged += this.WorkstationMaterial_OnWeightsChanged;
		}
		XUiC_WorkstationFuelGrid childByType2 = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationFuelGrid>();
		if (childByType2 != null)
		{
			childByType2.OnWorkstationFuelChanged += this.WorkStation_OnToolsOrFuelChanged;
		}
		XUiC_WorkstationToolGrid childByType3 = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationToolGrid>();
		if (childByType3 != null)
		{
			childByType3.OnWorkstationToolsChanged += this.WorkStation_OnToolsOrFuelChanged;
		}
		this.ClearSelection();
		if (base.xui.playerUI.entityPlayer.QuestJournal.HasCraftingQuest() && (this.txtInput == null || this.txtInput.Text == ""))
		{
			this.GetRecipeData();
			this.pageChanged = true;
		}
		if (base.xui.QuestTracker.TrackedChallenge != null)
		{
			this.GetRecipeData();
			this.pageChanged = true;
		}
		this.IsDirty = true;
		this.resortRecipes = true;
	}

	// Token: 0x06006F21 RID: 28449 RVA: 0x002D5AEC File Offset: 0x002D3CEC
	public override void OnClose()
	{
		if (base.ViewComponent != null && base.ViewComponent.IsVisible)
		{
			base.ViewComponent.IsVisible = false;
		}
		base.xui.PlayerInventory.OnBackpackItemsChanged -= this.PlayerInventory_OnBackpackItemsChanged;
		base.xui.PlayerInventory.OnToolbeltItemsChanged -= this.PlayerInventory_OnToolbeltItemsChanged;
		base.xui.playerUI.entityPlayer.QuestChanged -= this.QuestJournal_QuestChanged;
		base.xui.playerUI.entityPlayer.QuestRemoved -= this.QuestJournal_QuestChanged;
		base.xui.QuestTracker.OnTrackedChallengeChanged -= this.QuestTracker_OnTrackedChallengeChanged;
		base.xui.Recipes.OnTrackedRecipeChanged -= this.Recipes_OnTrackedRecipeChanged;
		XUiC_WorkstationMaterialInputWindow childByType = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationMaterialInputWindow>();
		if (childByType != null)
		{
			childByType.OnWorkstationMaterialWeightsChanged -= this.WorkstationMaterial_OnWeightsChanged;
		}
		XUiC_WorkstationFuelGrid childByType2 = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationFuelGrid>();
		if (childByType2 != null)
		{
			childByType2.OnWorkstationFuelChanged -= this.WorkStation_OnToolsOrFuelChanged;
		}
		XUiC_WorkstationToolGrid childByType3 = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationToolGrid>();
		if (childByType3 != null)
		{
			childByType3.OnWorkstationToolsChanged -= this.WorkStation_OnToolsOrFuelChanged;
		}
		this.SelectedEntry = null;
	}

	// Token: 0x06006F22 RID: 28450 RVA: 0x002D5C45 File Offset: 0x002D3E45
	[PublicizedFrom(EAccessModifier.Private)]
	public void WorkStation_OnToolsOrFuelChanged()
	{
		this.IsDirty = true;
		this.resortRecipes = true;
		this.pageChanged = true;
	}

	// Token: 0x06006F23 RID: 28451 RVA: 0x002D5C45 File Offset: 0x002D3E45
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayerInventory_OnToolbeltItemsChanged()
	{
		this.IsDirty = true;
		this.resortRecipes = true;
		this.pageChanged = true;
	}

	// Token: 0x06006F24 RID: 28452 RVA: 0x002D5C45 File Offset: 0x002D3E45
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayerInventory_OnBackpackItemsChanged()
	{
		this.IsDirty = true;
		this.resortRecipes = true;
		this.pageChanged = true;
	}

	// Token: 0x06006F25 RID: 28453 RVA: 0x002D5C5C File Offset: 0x002D3E5C
	[PublicizedFrom(EAccessModifier.Private)]
	public void QuestJournal_QuestChanged(Quest q)
	{
		this.GetRecipeData();
		this.IsDirty = true;
		this.resortRecipes = true;
		this.pageChanged = true;
	}

	// Token: 0x06006F26 RID: 28454 RVA: 0x002D5C5C File Offset: 0x002D3E5C
	[PublicizedFrom(EAccessModifier.Private)]
	public void QuestTracker_OnTrackedChallengeChanged()
	{
		this.GetRecipeData();
		this.IsDirty = true;
		this.resortRecipes = true;
		this.pageChanged = true;
	}

	// Token: 0x06006F27 RID: 28455 RVA: 0x002D5C45 File Offset: 0x002D3E45
	[PublicizedFrom(EAccessModifier.Private)]
	public void WorkstationMaterial_OnWeightsChanged()
	{
		this.IsDirty = true;
		this.resortRecipes = true;
		this.pageChanged = true;
	}

	// Token: 0x06006F28 RID: 28456 RVA: 0x002D5C5C File Offset: 0x002D3E5C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Recipes_OnTrackedRecipeChanged()
	{
		this.GetRecipeData();
		this.IsDirty = true;
		this.resortRecipes = true;
		this.pageChanged = true;
	}

	// Token: 0x06006F29 RID: 28457 RVA: 0x002D5C79 File Offset: 0x002D3E79
	public void ClearSelection()
	{
		this.SelectedEntry = null;
	}

	// Token: 0x06006F2A RID: 28458 RVA: 0x002D5C84 File Offset: 0x002D3E84
	[PublicizedFrom(EAccessModifier.Private)]
	public void BuildRecipeInfosList(List<ItemStack> _items)
	{
		this.recipeInfos.Clear();
		for (int i = 0; i < this.recipes.Count; i++)
		{
			XUiC_RecipeList.RecipeInfo recipeInfo;
			recipeInfo.recipe = this.recipes[i];
			recipeInfo.unlocked = XUiM_Recipes.GetRecipeIsUnlocked(base.xui, recipeInfo.recipe);
			EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
			bool flag = XUiM_Recipes.HasIngredientsForRecipe(_items, recipeInfo.recipe, entityPlayer);
			recipeInfo.hasIngredients = (flag && (this.craftingWindow == null || this.craftingWindow.CraftingRequirementsValid(recipeInfo.recipe)));
			recipeInfo.name = Localization.Get(recipeInfo.recipe.GetName(), false);
			this.recipeInfos.Add(recipeInfo);
		}
	}

	// Token: 0x06006F2B RID: 28459 RVA: 0x002D5D50 File Offset: 0x002D3F50
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateRecipes()
	{
		this.recipes.Clear();
		for (int i = 0; i < this.recipeInfos.Count; i++)
		{
			this.recipes.Add(this.recipeInfos[i].recipe);
		}
	}

	// Token: 0x06006F2C RID: 28460 RVA: 0x002D5D9C File Offset: 0x002D3F9C
	[PublicizedFrom(EAccessModifier.Private)]
	public int CompareRecipeInfos(XUiC_RecipeList.RecipeInfo lhs, XUiC_RecipeList.RecipeInfo rhs)
	{
		if (lhs.recipe.IsTracked != rhs.recipe.IsTracked)
		{
			if (!lhs.recipe.IsTracked)
			{
				return 1;
			}
			return -1;
		}
		else if (lhs.recipe.isChallenge != rhs.recipe.isChallenge)
		{
			if (!lhs.recipe.isChallenge)
			{
				return 1;
			}
			return -1;
		}
		else if (lhs.recipe.isQuest != rhs.recipe.isQuest)
		{
			if (!lhs.recipe.isQuest)
			{
				return 1;
			}
			return -1;
		}
		else if (lhs.unlocked != rhs.unlocked)
		{
			if (!lhs.unlocked)
			{
				return 1;
			}
			return -1;
		}
		else if (lhs.hasIngredients != rhs.hasIngredients)
		{
			if (!lhs.hasIngredients)
			{
				return 1;
			}
			return -1;
		}
		else
		{
			if (!(lhs.name == rhs.name))
			{
				return string.Compare(lhs.name, rhs.name, StringComparison.Ordinal);
			}
			if (lhs.recipe.count > rhs.recipe.count)
			{
				return 1;
			}
			if (lhs.recipe.count < rhs.recipe.count)
			{
				return -1;
			}
			if (lhs.recipe.itemValueType > rhs.recipe.itemValueType)
			{
				return 1;
			}
			if (lhs.recipe.itemValueType < rhs.recipe.itemValueType)
			{
				return -1;
			}
			return this.CompareRecipeIngredients(lhs.recipe.ingredients, rhs.recipe.ingredients);
		}
	}

	// Token: 0x06006F2D RID: 28461 RVA: 0x002D5F08 File Offset: 0x002D4108
	[PublicizedFrom(EAccessModifier.Private)]
	public int CompareRecipeIngredients(List<ItemStack> lhs, List<ItemStack> rhs)
	{
		if (lhs.Count > rhs.Count)
		{
			return 1;
		}
		if (lhs.Count < rhs.Count)
		{
			return -1;
		}
		for (int i = 0; i < lhs.Count; i++)
		{
			int itemId = lhs[i].itemValue.GetItemId();
			int itemId2 = rhs[i].itemValue.GetItemId();
			if (itemId > itemId2)
			{
				return 1;
			}
			if (itemId < itemId2)
			{
				return -1;
			}
			if (lhs[i].count > rhs[i].count)
			{
				return 1;
			}
			if (lhs[i].count < rhs[i].count)
			{
				return -1;
			}
		}
		return 0;
	}

	// Token: 0x06006F2E RID: 28462 RVA: 0x002D5FB0 File Offset: 0x002D41B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void FindShowingWindow()
	{
		List<XUiC_CraftingWindowGroup> childrenByType = base.xui.GetChildrenByType<XUiC_CraftingWindowGroup>();
		for (int i = 0; i < childrenByType.Count; i++)
		{
			if (childrenByType[i].WindowGroup != null && childrenByType[i].WindowGroup.isShowing)
			{
				this.craftingWindow = childrenByType[i];
				return;
			}
		}
	}

	// Token: 0x0400544A RID: 21578
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_RecipeList.RecipeInfo> recipeInfos = new List<XUiC_RecipeList.RecipeInfo>();

	// Token: 0x0400544B RID: 21579
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_RecipeEntry[] recipeControls;

	// Token: 0x0400544C RID: 21580
	[PublicizedFrom(EAccessModifier.Private)]
	public string workStation = "";

	// Token: 0x0400544D RID: 21581
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Paging pager;

	// Token: 0x0400544E RID: 21582
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_RecipeEntry selectedEntry;

	// Token: 0x0400544F RID: 21583
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button favorites;

	// Token: 0x04005450 RID: 21584
	[PublicizedFrom(EAccessModifier.Private)]
	public string category = "Basics";

	// Token: 0x04005451 RID: 21585
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Recipe> recipes = new List<Recipe>();

	// Token: 0x04005452 RID: 21586
	[PublicizedFrom(EAccessModifier.Private)]
	public int page;

	// Token: 0x04005453 RID: 21587
	[PublicizedFrom(EAccessModifier.Private)]
	public int length;

	// Token: 0x04005454 RID: 21588
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showFavorites;

	// Token: 0x04005455 RID: 21589
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 selectedColor = new Color32(222, 206, 163, byte.MaxValue);

	// Token: 0x04005456 RID: 21590
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtInput;

	// Token: 0x04005457 RID: 21591
	[PublicizedFrom(EAccessModifier.Private)]
	public bool resortRecipes;

	// Token: 0x04005458 RID: 21592
	[PublicizedFrom(EAccessModifier.Private)]
	public bool pageChanged;

	// Token: 0x04005459 RID: 21593
	public string[] craftingArea = new string[]
	{
		""
	};

	// Token: 0x0400545F RID: 21599
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ItemStack> updateStackList = new List<ItemStack>();

	// Token: 0x04005460 RID: 21600
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CraftingWindowGroup craftingWindow;

	// Token: 0x02000DDC RID: 3548
	[PublicizedFrom(EAccessModifier.Private)]
	public struct RecipeInfo
	{
		// Token: 0x04005461 RID: 21601
		public Recipe recipe;

		// Token: 0x04005462 RID: 21602
		public bool unlocked;

		// Token: 0x04005463 RID: 21603
		public bool hasIngredients;

		// Token: 0x04005464 RID: 21604
		public string name;
	}
}
