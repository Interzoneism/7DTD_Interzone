using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DD8 RID: 3544
[Preserve]
public class XUiC_RecipeEntry : XUiC_SelectableEntry
{
	// Token: 0x17000B25 RID: 2853
	// (get) Token: 0x06006EEB RID: 28395 RVA: 0x002D43F1 File Offset: 0x002D25F1
	// (set) Token: 0x06006EEC RID: 28396 RVA: 0x002D43F9 File Offset: 0x002D25F9
	public bool HasIngredients
	{
		get
		{
			return this.hasIngredients;
		}
		set
		{
			this.hasIngredients = value;
			base.RefreshBindings(false);
		}
	}

	// Token: 0x17000B26 RID: 2854
	// (get) Token: 0x06006EED RID: 28397 RVA: 0x002D4409 File Offset: 0x002D2609
	// (set) Token: 0x06006EEE RID: 28398 RVA: 0x002D4411 File Offset: 0x002D2611
	public bool IsCurrentWorkstation { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000B27 RID: 2855
	// (get) Token: 0x06006EEF RID: 28399 RVA: 0x002D441A File Offset: 0x002D261A
	// (set) Token: 0x06006EF0 RID: 28400 RVA: 0x002D4424 File Offset: 0x002D2624
	public Recipe Recipe
	{
		get
		{
			return this.recipe;
		}
		set
		{
			this.recipe = value;
			this.IsCurrentWorkstation = false;
			if (this.recipe != null)
			{
				for (int i = 0; i < this.RecipeList.craftingArea.Length; i++)
				{
					if (this.RecipeList.craftingArea[i] == this.recipe.craftingArea)
					{
						this.IsCurrentWorkstation = true;
						break;
					}
				}
			}
			if (!base.Selected)
			{
				this.background.Color = new Color32(64, 64, 64, byte.MaxValue);
			}
			base.ViewComponent.IsNavigatable = (base.ViewComponent.IsSnappable = (value != null));
		}
	}

	// Token: 0x06006EF1 RID: 28401 RVA: 0x002D44CC File Offset: 0x002D26CC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SelectedChanged(bool isSelected)
	{
		if (this.background != null)
		{
			this.background.Color = (isSelected ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue) : new Color32(64, 64, 64, byte.MaxValue));
			this.background.SpriteName = (isSelected ? "ui_game_select_row" : "menu_empty");
		}
	}

	// Token: 0x06006EF2 RID: 28402 RVA: 0x002D453C File Offset: 0x002D273C
	public void SetRecipeAndHasIngredients(Recipe recipe, bool hasIngredients)
	{
		this.Recipe = recipe;
		this.hasIngredients = hasIngredients;
		this.isDirty = true;
		base.RefreshBindings(false);
		if (recipe == null)
		{
			this.background.Color = new Color32(64, 64, 64, byte.MaxValue);
		}
	}

	// Token: 0x06006EF3 RID: 28403 RVA: 0x002D4588 File Offset: 0x002D2788
	public override void Init()
	{
		base.Init();
		for (int i = 0; i < this.children.Count; i++)
		{
			XUiView viewComponent = this.children[i].ViewComponent;
			if (viewComponent.ID.EqualsCaseInsensitive("name"))
			{
				this.lblName = (viewComponent as XUiV_Label);
			}
			else if (viewComponent.ID.EqualsCaseInsensitive("icon"))
			{
				this.icoRecipe = (viewComponent as XUiV_Sprite);
			}
			else if (viewComponent.ID.EqualsCaseInsensitive("favorite"))
			{
				this.icoFavorite = (viewComponent as XUiV_Sprite);
			}
			else if (viewComponent.ID.EqualsCaseInsensitive("unlocked"))
			{
				this.icoBook = (viewComponent as XUiV_Sprite);
			}
			else if (viewComponent.ID.EqualsCaseInsensitive("background"))
			{
				this.background = (viewComponent as XUiV_Sprite);
			}
		}
		this.isDirty = true;
	}

	// Token: 0x06006EF4 RID: 28404 RVA: 0x002D4670 File Offset: 0x002D2870
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		this.isHovered = _isOver;
		if (this.background != null && this.recipe != null && !base.Selected)
		{
			if (_isOver)
			{
				this.background.Color = new Color32(96, 96, 96, byte.MaxValue);
			}
			else
			{
				this.background.Color = new Color32(64, 64, 64, byte.MaxValue);
			}
		}
		base.OnHovered(_isOver);
	}

	// Token: 0x06006EF5 RID: 28405 RVA: 0x002D46E8 File Offset: 0x002D28E8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 1820482109U)
		{
			if (num <= 847165955U)
			{
				if (num != 98823489U)
				{
					if (num != 112674252U)
					{
						if (num == 847165955U)
						{
							if (bindingName == "itemtypeicon")
							{
								value = "";
								if (this.recipe != null)
								{
									ItemClass forId = ItemClass.GetForId(this.recipe.itemValueType);
									if (forId != null)
									{
										if (forId.IsBlock())
										{
											value = forId.GetBlock().ItemTypeIcon;
										}
										else
										{
											if (forId.AltItemTypeIcon != null && forId.Unlocks != "" && XUiM_ItemStack.CheckKnown(base.xui.playerUI.entityPlayer, forId, null))
											{
												value = forId.AltItemTypeIcon;
												return true;
											}
											value = forId.ItemTypeIcon;
										}
									}
								}
								return true;
							}
						}
					}
					else if (bindingName == "recipename")
					{
						value = ((this.recipe != null) ? Localization.Get(this.recipe.GetName(), false) : "");
						return true;
					}
				}
				else if (bindingName == "hasingredientsstatecolor")
				{
					if (this.recipe != null)
					{
						Color32 v = new Color32(148, 148, 148, byte.MaxValue);
						if (this.HasIngredients)
						{
							if (this.CustomAttributes.ContainsKey("enabled_font_color"))
							{
								v = StringParsers.ParseColor32(this.CustomAttributes["enabled_font_color"]);
							}
							else
							{
								v = Color.white;
							}
						}
						else if (this.CustomAttributes.ContainsKey("disabled_font_color"))
						{
							v = StringParsers.ParseColor32(this.CustomAttributes["disabled_font_color"]);
						}
						value = this.hasingredientsstatecolorFormatter.Format(v);
					}
					else
					{
						value = "255,255,255,255";
					}
					return true;
				}
			}
			else if (num != 1236476975U)
			{
				if (num != 1388578781U)
				{
					if (num == 1820482109U)
					{
						if (bindingName == "isfavorite")
						{
							value = ((this.recipe != null) ? XUiM_Recipes.GetRecipeIsFavorite(base.xui, this.recipe).ToString() : "false");
							return true;
						}
					}
				}
				else if (bindingName == "hasitemtypeicon")
				{
					value = "false";
					if (this.recipe != null)
					{
						ItemClass forId2 = ItemClass.GetForId(this.recipe.itemValueType);
						if (forId2 != null)
						{
							if (forId2.IsBlock())
							{
								value = (forId2.GetBlock().ItemTypeIcon != "").ToString();
							}
							else
							{
								value = (forId2.ItemTypeIcon != "").ToString();
							}
						}
					}
					return true;
				}
			}
			else if (bindingName == "recipeicontint")
			{
				Color32 v2 = Color.white;
				if (this.recipe != null)
				{
					ItemClass forId3 = ItemClass.GetForId(this.recipe.itemValueType);
					if (forId3 != null)
					{
						v2 = forId3.GetIconTint(null);
					}
				}
				value = this.recipeicontintcolorFormatter.Format(v2);
				return true;
			}
		}
		else if (num <= 2172950741U)
		{
			if (num != 2017922685U)
			{
				if (num != 2154375741U)
				{
					if (num == 2172950741U)
					{
						if (bindingName == "isunlockable")
						{
							value = ((this.recipe != null) ? (!this.IsCurrentWorkstation || XUiM_Recipes.GetRecipeIsUnlockable(base.xui, this.recipe) || this.recipe.isQuest || this.recipe.isChallenge || this.recipe.IsTracked).ToString() : "false");
							return true;
						}
					}
				}
				else if (bindingName == "hasrecipe")
				{
					value = (this.recipe != null).ToString();
					return true;
				}
			}
			else if (bindingName == "hasingredients")
			{
				value = ((this.recipe != null) ? this.HasIngredients.ToString() : "false");
				return true;
			}
		}
		else if (num <= 3435046057U)
		{
			if (num != 3008660032U)
			{
				if (num == 3435046057U)
				{
					if (bindingName == "unlockstatecolor")
					{
						if (this.recipe != null)
						{
							if (this.recipe.isQuest || this.recipe.isChallenge)
							{
								value = this.unlockstatecolorFormatter.Format(Color.yellow);
							}
							else
							{
								Color32 v3 = XUiM_Recipes.GetRecipeIsUnlocked(base.xui, this.recipe) ? Color.white : Color.gray;
								value = this.unlockstatecolorFormatter.Format(v3);
							}
						}
						else
						{
							value = "255,255,255,255";
						}
						return true;
					}
				}
			}
			else if (bindingName == "unlockicon")
			{
				if (this.recipe != null)
				{
					if (this.recipe.isChallenge)
					{
						value = "ui_game_symbol_challenge";
						return true;
					}
					if (this.recipe.isQuest)
					{
						value = "ui_game_symbol_quest";
						return true;
					}
					if (this.recipe.IsTracked)
					{
						value = "ui_game_symbol_compass";
						return true;
					}
					if (XUiM_Recipes.GetRecipeIsUnlockable(base.xui, this.recipe) && !XUiM_Recipes.GetRecipeIsUnlocked(base.xui, this.recipe))
					{
						value = "ui_game_symbol_lock";
					}
					else if (!this.IsCurrentWorkstation)
					{
						WorkstationData workstationData = CraftingManager.GetWorkstationData(this.recipe.craftingArea);
						if (workstationData != null)
						{
							value = workstationData.WorkstationIcon;
						}
						else
						{
							value = "ui_game_symbol_hammer";
						}
					}
					else
					{
						value = "";
					}
				}
				else
				{
					value = "ui_game_symbol_book";
				}
				return true;
			}
		}
		else if (num != 3974800894U)
		{
			if (num == 4049247086U)
			{
				if (bindingName == "itemtypeicontint")
				{
					value = "255,255,255,255";
					if (this.recipe != null)
					{
						ItemClass forId4 = ItemClass.GetForId(this.recipe.itemValueType);
						if (forId4 != null && forId4.Unlocks != "" && XUiM_ItemStack.CheckKnown(base.xui.playerUI.entityPlayer, forId4, null))
						{
							value = this.altitemtypeiconcolorFormatter.Format(forId4.AltItemTypeIconColor);
						}
					}
					return true;
				}
			}
		}
		else if (bindingName == "recipeicon")
		{
			value = ((this.recipe != null) ? this.recipe.GetIcon() : "");
			return true;
		}
		return false;
	}

	// Token: 0x06006EF6 RID: 28406 RVA: 0x002D4D69 File Offset: 0x002D2F69
	public void Refresh()
	{
		this.isDirty = true;
		base.RefreshBindings(false);
	}

	// Token: 0x0400543B RID: 21563
	[PublicizedFrom(EAccessModifier.Private)]
	public Recipe recipe;

	// Token: 0x0400543C RID: 21564
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isHovered;

	// Token: 0x0400543D RID: 21565
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x0400543E RID: 21566
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasIngredients;

	// Token: 0x0400543F RID: 21567
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblName;

	// Token: 0x04005440 RID: 21568
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite icoRecipe;

	// Token: 0x04005441 RID: 21569
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite icoFavorite;

	// Token: 0x04005442 RID: 21570
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite icoBook;

	// Token: 0x04005443 RID: 21571
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite background;

	// Token: 0x04005444 RID: 21572
	public XUiC_RecipeList RecipeList;

	// Token: 0x04005446 RID: 21574
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor recipeicontintcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04005447 RID: 21575
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor hasingredientsstatecolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04005448 RID: 21576
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor unlockstatecolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04005449 RID: 21577
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor altitemtypeiconcolorFormatter = new CachedStringFormatterXuiRgbaColor();
}
