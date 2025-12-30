using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C64 RID: 3172
[Preserve]
public class XUiC_CraftingInfoWindow : XUiC_InfoWindow
{
	// Token: 0x17000A0C RID: 2572
	// (get) Token: 0x060061B1 RID: 25009 RVA: 0x00279EA8 File Offset: 0x002780A8
	public int SelectedCraftingTier
	{
		get
		{
			return this.selectedCraftingTier;
		}
	}

	// Token: 0x060061B2 RID: 25010 RVA: 0x00279EB0 File Offset: 0x002780B0
	public override void Init()
	{
		base.Init();
		this.itemPreview = base.GetChildById("itemPreview");
		this.windowName = base.GetChildById("windowName");
		this.windowIcon = base.GetChildById("windowIcon");
		this.description = base.GetChildById("descriptionText");
		this.craftingTime = base.GetChildById("craftingTime");
		this.addQualityButton = base.GetChildById("addQualityButton");
		this.addQualityButton.OnPress += this.AddQualityButton_OnPress;
		this.subtractQualityButton = base.GetChildById("subtractQualityButton");
		this.subtractQualityButton.OnPress += this.SubtractQualityButton_OnPress;
		this.requiredToolOverlay = base.GetChildById("requiredToolOverlay");
		this.requiredToolCheckmark = base.GetChildById("requiredToolCheckmark");
		this.requiredToolText = base.GetChildById("requiredToolText");
		this.actionItemList = (XUiC_ItemActionList)base.GetChildById("itemActions");
		this.ingredientsButton = base.GetChildById("ingredientsButton");
		this.ingredientsButton.OnPress += this.IngredientsButton_OnPress;
		this.descriptionButton = base.GetChildById("descriptionButton");
		this.descriptionButton.OnPress += this.DescriptionButton_OnPress;
		this.unlockedByButton = base.GetChildById("showunlocksButton");
		this.unlockedByButton.OnPress += this.UnlockedByButton_OnPress;
		this.recipeCraftCount = base.GetChildByType<XUiC_RecipeCraftCount>();
		if (this.recipeCraftCount != null)
		{
			this.recipeCraftCount.OnCountChanged += this.HandleOnCountChanged;
		}
		this.recipeList = this.windowGroup.Controller.GetChildByType<XUiC_RecipeList>();
		if (this.recipeList != null)
		{
			this.recipeList.RecipeChanged += this.HandleRecipeChanged;
		}
		this.categoryList = this.windowGroup.Controller.GetChildByType<XUiC_CategoryList>();
		this.ingredientList = base.GetChildByType<XUiC_IngredientList>();
		this.unlockByList = base.GetChildByType<XUiC_UnlockByList>();
		this.IsDormant = true;
	}

	// Token: 0x060061B3 RID: 25011 RVA: 0x0027A0BE File Offset: 0x002782BE
	[PublicizedFrom(EAccessModifier.Private)]
	public void SubtractQualityButton_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.selectedCraftingTier > 1)
		{
			this.selectedCraftingTier--;
			this.IsDirty = true;
		}
	}

	// Token: 0x060061B4 RID: 25012 RVA: 0x0027A0DE File Offset: 0x002782DE
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddQualityButton_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.selectedCraftingTier < 6)
		{
			this.selectedCraftingTier++;
			this.IsDirty = true;
		}
	}

	// Token: 0x060061B5 RID: 25013 RVA: 0x0027A100 File Offset: 0x00278300
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetSelectedButtonByType(XUiC_CraftingInfoWindow.TabTypes tabType)
	{
		((XUiV_Button)this.ingredientsButton.ViewComponent).Selected = (tabType == XUiC_CraftingInfoWindow.TabTypes.Ingredients);
		((XUiV_Button)this.descriptionButton.ViewComponent).Selected = (tabType == XUiC_CraftingInfoWindow.TabTypes.Description);
		((XUiV_Button)this.unlockedByButton.ViewComponent).Selected = (tabType == XUiC_CraftingInfoWindow.TabTypes.UnlockedBy);
	}

	// Token: 0x060061B6 RID: 25014 RVA: 0x0027A158 File Offset: 0x00278358
	[PublicizedFrom(EAccessModifier.Private)]
	public void IngredientsButton_OnPress(XUiController _sender, int _mouseButton)
	{
		this.TabType = XUiC_CraftingInfoWindow.TabTypes.Ingredients;
		this.SetSelectedButtonByType(this.TabType);
		this.IsDirty = true;
	}

	// Token: 0x060061B7 RID: 25015 RVA: 0x0027A174 File Offset: 0x00278374
	[PublicizedFrom(EAccessModifier.Private)]
	public void DescriptionButton_OnPress(XUiController _sender, int _mouseButton)
	{
		this.TabType = XUiC_CraftingInfoWindow.TabTypes.Description;
		this.SetSelectedButtonByType(this.TabType);
		this.IsDirty = true;
	}

	// Token: 0x060061B8 RID: 25016 RVA: 0x0027A190 File Offset: 0x00278390
	[PublicizedFrom(EAccessModifier.Private)]
	public void UnlockedByButton_OnPress(XUiController _sender, int _mouseButton)
	{
		this.TabType = XUiC_CraftingInfoWindow.TabTypes.UnlockedBy;
		this.SetSelectedButtonByType(this.TabType);
		this.IsDirty = true;
	}

	// Token: 0x060061B9 RID: 25017 RVA: 0x0027A1AC File Offset: 0x002783AC
	public override void OnOpen()
	{
		base.OnOpen();
		this.IsDormant = false;
		if (base.xui.PlayerInventory != null)
		{
			base.xui.PlayerInventory.OnBackpackItemsChanged += this.PlayerInventory_OnItemsChanged;
			base.xui.PlayerInventory.OnToolbeltItemsChanged += this.PlayerInventory_OnItemsChanged;
		}
	}

	// Token: 0x060061BA RID: 25018 RVA: 0x0027A20C File Offset: 0x0027840C
	public override void OnClose()
	{
		base.OnClose();
		this.IsDormant = true;
		if (base.xui.PlayerInventory != null)
		{
			base.xui.PlayerInventory.OnBackpackItemsChanged -= this.PlayerInventory_OnItemsChanged;
			base.xui.PlayerInventory.OnToolbeltItemsChanged -= this.PlayerInventory_OnItemsChanged;
		}
	}

	// Token: 0x060061BB RID: 25019 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayerInventory_OnItemsChanged()
	{
		this.IsDirty = true;
	}

	// Token: 0x060061BC RID: 25020 RVA: 0x0027A26B File Offset: 0x0027846B
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnCountChanged(XUiController _sender, OnCountChangedEventArgs _e)
	{
		this.craftCount = _e.Count;
		this.IsDirty = true;
	}

	// Token: 0x060061BD RID: 25021 RVA: 0x0027A280 File Offset: 0x00278480
	public override void Deselect()
	{
		if (this.selectedEntry != null)
		{
			this.selectedEntry.Selected = false;
		}
	}

	// Token: 0x060061BE RID: 25022 RVA: 0x0027A298 File Offset: 0x00278498
	public void SetCategory(string category)
	{
		if (this.categoryList != null)
		{
			XUiC_CategoryEntry currentCategory = this.categoryList.CurrentCategory;
			if (((currentCategory != null) ? currentCategory.CategoryName : null) != category)
			{
				this.categoryList.SetCategory(category);
			}
		}
		if (this.recipeList != null && this.recipeList.GetCategory() != category)
		{
			this.recipeList.SetCategory(category);
		}
	}

	// Token: 0x060061BF RID: 25023 RVA: 0x0027A300 File Offset: 0x00278500
	public override void Update(float _dt)
	{
		if (!this.windowGroup.isShowing)
		{
			return;
		}
		base.Update(_dt);
		if (!this.windowGroup.isShowing)
		{
			return;
		}
		if (this.IsDirty)
		{
			if (this.emptyInfoWindow == null)
			{
				this.emptyInfoWindow = (XUiC_InfoWindow)base.xui.FindWindowGroupByName("backpack").GetChildById("emptyInfoPanel");
			}
			if (this.itemInfoWindow == null)
			{
				this.itemInfoWindow = (XUiC_ItemInfoWindow)base.xui.FindWindowGroupByName("backpack").GetChildById("itemInfoPanel");
			}
			this.lastRecipeSelected = this.recipe;
			this.recipe = ((this.selectedEntry != null) ? this.selectedEntry.Recipe : null);
			bool flag = this.recipe != null;
			if (flag)
			{
				int craftingTier = this.recipe.GetCraftingTier(base.xui.playerUI.entityPlayer);
				if (this.recipe != this.lastRecipeSelected || (this.recipe == this.lastRecipeSelected && craftingTier != this.selectedMaxCraftingTier))
				{
					this.selectedCraftingTier = craftingTier;
				}
				this.selectedMaxCraftingTier = craftingTier;
			}
			if (this.emptyInfoWindow != null && !flag && !this.itemInfoWindow.ViewComponent.IsVisible)
			{
				this.emptyInfoWindow.ViewComponent.IsVisible = true;
			}
			if (this.itemPreview == null)
			{
				return;
			}
			XUiView viewComponent = this.itemPreview.ViewComponent;
			XUiView viewComponent2 = this.windowName.ViewComponent;
			XUiView viewComponent3 = this.windowIcon.ViewComponent;
			XUiView viewComponent4 = this.description.ViewComponent;
			XUiView viewComponent5 = this.craftingTime.ViewComponent;
			if (this.ingredientList != null)
			{
				this.ingredientList.CraftingTier = this.selectedCraftingTier;
				this.ingredientList.Recipe = this.recipe;
			}
			if (this.unlockByList != null)
			{
				this.unlockByList.Recipe = this.recipe;
			}
			this.actionItemList.SetCraftingActionList(flag ? XUiC_ItemActionList.ItemActionListTypes.Crafting : XUiC_ItemActionList.ItemActionListTypes.None, this.selectedEntry);
			XUiC_WorkstationToolGrid childByType = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationToolGrid>();
			if (childByType != null && this.selectedEntry != null && this.selectedEntry.Recipe != null && this.selectedEntry.Recipe.craftingToolType != 0)
			{
				this.requiredToolOverlay.ViewComponent.IsVisible = true;
				ItemClass forId = ItemClass.GetForId(this.selectedEntry.Recipe.craftingToolType);
				if (forId != null)
				{
					string arg;
					if (forId.IsBlock())
					{
						arg = Block.list[forId.Id].GetLocalizedBlockName();
					}
					else
					{
						arg = forId.GetLocalizedItemName();
					}
					string format = Localization.Get("xuiToolRequired", false);
					((XUiV_Label)this.requiredToolText.ViewComponent).Text = string.Format(format, arg);
					if (childByType.HasRequirement(this.selectedEntry.Recipe))
					{
						((XUiV_Sprite)this.requiredToolCheckmark.ViewComponent).Color = this.validColor;
						((XUiV_Sprite)this.requiredToolCheckmark.ViewComponent).SpriteName = this.validSprite;
					}
					else
					{
						((XUiV_Sprite)this.requiredToolCheckmark.ViewComponent).Color = this.invalidColor;
						((XUiV_Sprite)this.requiredToolCheckmark.ViewComponent).SpriteName = this.invalidSprite;
					}
				}
			}
			else
			{
				this.requiredToolOverlay.ViewComponent.IsVisible = false;
			}
			this.recipeCraftCount.RefreshCounts();
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
	}

	// Token: 0x060061C0 RID: 25024 RVA: 0x0027A660 File Offset: 0x00278860
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 2943089870U)
		{
			if (num <= 1022877350U)
			{
				if (num <= 789508807U)
				{
					if (num != 386135447U)
					{
						if (num == 789508807U)
						{
							if (bindingName == "enablesubtractquality")
							{
								if (this.recipe != null && this.recipe.GetOutputItemClass().ShowQualityBar && this.selectedCraftingTier > 1)
								{
									value = "true";
								}
								else
								{
									value = "false";
								}
								return true;
							}
						}
					}
					else if (bindingName == "showunlockedbytab")
					{
						value = "false";
						if (this.recipe != null && !XUiM_Recipes.GetRecipeIsUnlocked(base.xui, this.recipe))
						{
							ItemClass forId = ItemClass.GetForId(this.recipe.itemValueType);
							if (forId != null)
							{
								if (forId.IsBlock())
								{
									value = (forId.GetBlock().UnlockedBy.Length != 0).ToString();
								}
								else
								{
									value = (forId.UnlockedBy.Length != 0).ToString();
								}
							}
						}
						return true;
					}
				}
				else if (num != 847165955U)
				{
					if (num != 936888752U)
					{
						if (num == 1022877350U)
						{
							if (bindingName == "durabilityjustify")
							{
								value = "center";
								if (this.recipe != null && !this.recipe.GetOutputItemClass().ShowQualityBar)
								{
									value = "right";
								}
								return true;
							}
						}
					}
					else if (bindingName == "showingredients")
					{
						value = (this.TabType == XUiC_CraftingInfoWindow.TabTypes.Ingredients).ToString();
						return true;
					}
				}
				else if (bindingName == "itemtypeicon")
				{
					value = "";
					if (this.recipe != null)
					{
						ItemClass forId2 = ItemClass.GetForId(this.recipe.itemValueType);
						if (forId2 != null)
						{
							if (forId2.IsBlock())
							{
								value = forId2.GetBlock().ItemTypeIcon;
							}
							else
							{
								if (forId2.AltItemTypeIcon != null && forId2.Unlocks != "" && XUiM_ItemStack.CheckKnown(base.xui.playerUI.entityPlayer, forId2, null))
								{
									value = forId2.AltItemTypeIcon;
									return true;
								}
								value = forId2.ItemTypeIcon;
							}
						}
					}
					return true;
				}
			}
			else if (num <= 1388578781U)
			{
				if (num != 1062608009U)
				{
					if (num == 1388578781U)
					{
						if (bindingName == "hasitemtypeicon")
						{
							value = "false";
							if (this.recipe != null)
							{
								ItemClass forId3 = ItemClass.GetForId(this.recipe.itemValueType);
								if (forId3 != null)
								{
									if (forId3.IsBlock())
									{
										value = (forId3.GetBlock().ItemTypeIcon != "").ToString();
									}
									else
									{
										value = (forId3.ItemTypeIcon != "").ToString();
									}
								}
							}
							return true;
						}
					}
				}
				else if (bindingName == "durabilitycolor")
				{
					Color32 v = Color.white;
					if (this.recipe != null)
					{
						v = QualityInfo.GetTierColor(this.selectedCraftingTier);
					}
					value = this.durabilitycolorFormatter.Format(v);
					return true;
				}
			}
			else if (num != 1585275412U)
			{
				if (num != 1953932597U)
				{
					if (num == 2943089870U)
					{
						if (bindingName == "enableaddquality")
						{
							if (this.recipe != null && this.recipe.GetOutputItemClass().ShowQualityBar && this.selectedCraftingTier < (int)EffectManager.GetValue(PassiveEffects.CraftingTier, null, 1f, base.xui.playerUI.entityPlayer, this.recipe, this.recipe.tags, true, true, true, true, true, 1, true, false))
							{
								value = "true";
							}
							else
							{
								value = "false";
							}
							return true;
						}
					}
				}
				else if (bindingName == "durabilitytext")
				{
					value = "";
					if (this.recipe != null && this.recipe.GetOutputItemClass().ShowQualityBar)
					{
						value = this.durabilitytextFormatter.Format(this.selectedCraftingTier);
					}
					return true;
				}
			}
			else if (bindingName == "itemgroupicon")
			{
				string text;
				if (this.recipe == null)
				{
					text = "";
				}
				else
				{
					XUiC_CategoryEntry currentCategory = this.categoryList.CurrentCategory;
					text = ((currentCategory != null) ? currentCategory.SpriteName : null);
				}
				value = text;
				return true;
			}
		}
		else if (num <= 3708628627U)
		{
			if (num <= 3154262838U)
			{
				if (num != 2944858628U)
				{
					if (num == 3154262838U)
					{
						if (bindingName == "showdescription")
						{
							value = (this.TabType == XUiC_CraftingInfoWindow.TabTypes.Description).ToString();
							return true;
						}
					}
				}
				else if (bindingName == "hasdurability")
				{
					value = (this.recipe != null && this.recipe.GetOutputItemClass().ShowQualityBar).ToString();
					return true;
				}
			}
			else if (num != 3191456325U)
			{
				if (num != 3262997624U)
				{
					if (num == 3708628627U)
					{
						if (bindingName == "itemicon")
						{
							if (this.recipe != null)
							{
								ItemValue itemValue = new ItemValue(this.recipe.itemValueType, false);
								value = itemValue.GetPropertyOverride("CustomIcon", itemValue.ItemClass.GetIconName());
							}
							return true;
						}
					}
				}
				else if (bindingName == "itemdescription")
				{
					string text2 = "";
					if (this.recipe != null)
					{
						ItemClass forId4 = ItemClass.GetForId(this.recipe.itemValueType);
						if (forId4 != null)
						{
							if (forId4.IsBlock())
							{
								string descriptionKey = Block.list[this.recipe.itemValueType].DescriptionKey;
								if (Localization.Exists(descriptionKey, false))
								{
									text2 = Localization.Get(descriptionKey, false);
								}
							}
							else
							{
								string itemDescriptionKey = forId4.GetItemDescriptionKey();
								if (Localization.Exists(itemDescriptionKey, false))
								{
									text2 = Localization.Get(itemDescriptionKey, false);
								}
							}
						}
					}
					value = text2;
					return true;
				}
			}
			else if (bindingName == "itemname")
			{
				value = ((this.recipe != null) ? Localization.Get(this.recipe.GetName(), false) : "");
				return true;
			}
		}
		else if (num <= 4049247086U)
		{
			if (num != 4044995374U)
			{
				if (num == 4049247086U)
				{
					if (bindingName == "itemtypeicontint")
					{
						value = "255,255,255,255";
						if (this.recipe != null)
						{
							ItemClass forId5 = ItemClass.GetForId(this.recipe.itemValueType);
							if (forId5 != null && forId5.Unlocks != "" && XUiM_ItemStack.CheckKnown(base.xui.playerUI.entityPlayer, forId5, null))
							{
								value = this.altitemtypeiconcolorFormatter.Format(forId5.AltItemTypeIconColor);
							}
						}
						return true;
					}
				}
			}
			else if (bindingName == "craftingtime")
			{
				value = "";
				if (this.recipe != null)
				{
					float recipeCraftTime = XUiM_Recipes.GetRecipeCraftTime(base.xui, this.recipe);
					float num2 = recipeCraftTime * (float)(this.craftCount - 1) + recipeCraftTime;
					value = this.craftingtimeFormatter.Format(num2 + 0.5f);
				}
				return true;
			}
		}
		else if (num != 4053908414U)
		{
			if (num != 4172540779U)
			{
				if (num == 4270832456U)
				{
					if (bindingName == "showunlockedby")
					{
						value = (this.TabType == XUiC_CraftingInfoWindow.TabTypes.UnlockedBy).ToString();
						return true;
					}
				}
			}
			else if (bindingName == "durabilityfill")
			{
				value = ((this.recipe == null) ? "0" : "1");
				return true;
			}
		}
		else if (bindingName == "itemicontint")
		{
			Color32 v2 = Color.white;
			if (this.recipe != null)
			{
				ItemValue itemValue2 = new ItemValue(this.recipe.itemValueType, false);
				v2 = itemValue2.ItemClass.GetIconTint(itemValue2);
			}
			value = this.itemicontintcolorFormatter.Format(v2);
			return true;
		}
		return false;
	}

	// Token: 0x060061C1 RID: 25025 RVA: 0x0027AE78 File Offset: 0x00279078
	public void SetRecipe(XUiC_RecipeEntry _recipeEntry)
	{
		this.selectedEntry = _recipeEntry;
		if (this.recipeCraftCount != null)
		{
			this.recipeCraftCount.IsDirty = true;
			this.craftCount = this.recipeCraftCount.Count;
		}
		else
		{
			this.craftCount = 1;
		}
		if (this.selectedEntry != null && this.selectedEntry.Recipe != null)
		{
			if (XUiM_Recipes.GetRecipeIsUnlocked(base.xui, this.selectedEntry.Recipe))
			{
				this.TabType = XUiC_CraftingInfoWindow.TabTypes.Ingredients;
				this.SetSelectedButtonByType(this.TabType);
			}
			else
			{
				this.TabType = XUiC_CraftingInfoWindow.TabTypes.UnlockedBy;
				this.SetSelectedButtonByType(this.TabType);
			}
		}
		this.IsDirty = true;
	}

	// Token: 0x060061C2 RID: 25026 RVA: 0x0027AF16 File Offset: 0x00279116
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleRecipeChanged(Recipe _recipe, XUiC_RecipeEntry _recipeEntry)
	{
		if (base.WindowGroup.isShowing)
		{
			base.ViewComponent.IsVisible = true;
		}
		this.SetRecipe(_recipeEntry);
	}

	// Token: 0x060061C3 RID: 25027 RVA: 0x0027AF38 File Offset: 0x00279138
	public override bool ParseAttribute(string attribute, string value, XUiController _parent)
	{
		if (base.ParseAttribute(attribute, value, _parent))
		{
			return true;
		}
		if (attribute == "valid_color")
		{
			this.validColor = StringParsers.ParseColor32(value);
			return true;
		}
		if (attribute == "invalid_color")
		{
			this.invalidColor = StringParsers.ParseColor32(value);
			return true;
		}
		if (attribute == "valid_sprite")
		{
			this.validSprite = value;
			return true;
		}
		if (!(attribute == "invalid_sprite"))
		{
			return false;
		}
		this.invalidSprite = value;
		return true;
	}

	// Token: 0x060061C4 RID: 25028 RVA: 0x0027AFB7 File Offset: 0x002791B7
	public void RefreshRecipe()
	{
		if (this.recipeCraftCount != null)
		{
			this.recipeCraftCount.CalculateMaxCount();
		}
	}

	// Token: 0x0400496B RID: 18795
	[PublicizedFrom(EAccessModifier.Private)]
	public Recipe recipe;

	// Token: 0x0400496C RID: 18796
	[PublicizedFrom(EAccessModifier.Private)]
	public int craftCount;

	// Token: 0x0400496D RID: 18797
	[PublicizedFrom(EAccessModifier.Private)]
	public int selectedCraftingTier = 1;

	// Token: 0x0400496E RID: 18798
	[PublicizedFrom(EAccessModifier.Private)]
	public int selectedMaxCraftingTier = 1;

	// Token: 0x0400496F RID: 18799
	[PublicizedFrom(EAccessModifier.Private)]
	public Recipe lastRecipeSelected;

	// Token: 0x04004970 RID: 18800
	public XUiC_CraftingInfoWindow.TabTypes TabType;

	// Token: 0x04004971 RID: 18801
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_RecipeEntry selectedEntry;

	// Token: 0x04004972 RID: 18802
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController itemPreview;

	// Token: 0x04004973 RID: 18803
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController windowName;

	// Token: 0x04004974 RID: 18804
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController windowIcon;

	// Token: 0x04004975 RID: 18805
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController description;

	// Token: 0x04004976 RID: 18806
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController craftingTime;

	// Token: 0x04004977 RID: 18807
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController requiredToolOverlay;

	// Token: 0x04004978 RID: 18808
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController requiredToolCheckmark;

	// Token: 0x04004979 RID: 18809
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController requiredToolText;

	// Token: 0x0400497A RID: 18810
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_IngredientList ingredientList;

	// Token: 0x0400497B RID: 18811
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_UnlockByList unlockByList;

	// Token: 0x0400497C RID: 18812
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CategoryList categoryList;

	// Token: 0x0400497D RID: 18813
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemActionList actionItemList;

	// Token: 0x0400497E RID: 18814
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_RecipeList recipeList;

	// Token: 0x0400497F RID: 18815
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_InfoWindow emptyInfoWindow;

	// Token: 0x04004980 RID: 18816
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemInfoWindow itemInfoWindow;

	// Token: 0x04004981 RID: 18817
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_RecipeCraftCount recipeCraftCount;

	// Token: 0x04004982 RID: 18818
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController ingredientsButton;

	// Token: 0x04004983 RID: 18819
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController descriptionButton;

	// Token: 0x04004984 RID: 18820
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController unlockedByButton;

	// Token: 0x04004985 RID: 18821
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController addQualityButton;

	// Token: 0x04004986 RID: 18822
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController subtractQualityButton;

	// Token: 0x04004987 RID: 18823
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor itemicontintcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04004988 RID: 18824
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<float> craftingtimeFormatter = new CachedStringFormatter<float>((float _time) => string.Format("{0:00}:{1:00}", (int)(_time / 60f), (int)(_time % 60f)));

	// Token: 0x04004989 RID: 18825
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor durabilitycolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x0400498A RID: 18826
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat durabilityfillFormatter = new CachedStringFormatterFloat(null);

	// Token: 0x0400498B RID: 18827
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt durabilitytextFormatter = new CachedStringFormatterInt();

	// Token: 0x0400498C RID: 18828
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor altitemtypeiconcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x0400498D RID: 18829
	[PublicizedFrom(EAccessModifier.Private)]
	public Color validColor;

	// Token: 0x0400498E RID: 18830
	[PublicizedFrom(EAccessModifier.Private)]
	public Color invalidColor;

	// Token: 0x0400498F RID: 18831
	[PublicizedFrom(EAccessModifier.Private)]
	public string validSprite;

	// Token: 0x04004990 RID: 18832
	[PublicizedFrom(EAccessModifier.Private)]
	public string invalidSprite;

	// Token: 0x02000C65 RID: 3173
	public enum TabTypes
	{
		// Token: 0x04004992 RID: 18834
		Ingredients,
		// Token: 0x04004993 RID: 18835
		Description,
		// Token: 0x04004994 RID: 18836
		UnlockedBy
	}
}
