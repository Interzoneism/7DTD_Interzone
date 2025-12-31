using System;
using UnityEngine.Scripting;

// Token: 0x02000DE2 RID: 3554
[Preserve]
public class XUiC_RecipeTrackerWindow : XUiController
{
	// Token: 0x17000B3A RID: 2874
	// (get) Token: 0x06006F7A RID: 28538 RVA: 0x002D8347 File Offset: 0x002D6547
	// (set) Token: 0x06006F7B RID: 28539 RVA: 0x002D834F File Offset: 0x002D654F
	public Recipe CurrentRecipe
	{
		get
		{
			return this.currentRecipe;
		}
		set
		{
			this.currentRecipe = value;
			this.IsDirty = true;
			base.RefreshBindings(true);
		}
	}

	// Token: 0x06006F7C RID: 28540 RVA: 0x002D8366 File Offset: 0x002D6566
	public override void Init()
	{
		base.Init();
		XUiC_RecipeTrackerWindow.ID = base.WindowGroup.ID;
		this.ingredientList = base.GetChildByType<XUiC_RecipeTrackerIngredientsList>();
	}

	// Token: 0x06006F7D RID: 28541 RVA: 0x002D838C File Offset: 0x002D658C
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.localPlayer == null)
		{
			this.localPlayer = base.xui.playerUI.entityPlayer;
		}
		if (base.ViewComponent.IsVisible && this.localPlayer.IsDead())
		{
			this.IsDirty = true;
		}
		if (this.IsDirty)
		{
			this.ingredientList.Count = this.Count;
			if (this.currentRecipe != null)
			{
				int craftingTier = this.currentRecipe.GetCraftingTier(base.xui.playerUI.entityPlayer);
				if (this.currentRecipe.GetOutputItemClass().HasQuality)
				{
					this.currentRecipe.craftingTier = base.xui.Recipes.TrackedRecipeQuality;
				}
				else
				{
					this.currentRecipe.craftingTier = craftingTier;
				}
			}
			this.ingredientList.Recipe = this.currentRecipe;
			base.RefreshBindings(true);
			this.IsDirty = false;
		}
	}

	// Token: 0x06006F7E RID: 28542 RVA: 0x002D8480 File Offset: 0x002D6680
	public override void OnOpen()
	{
		base.OnOpen();
		this.CurrentRecipe = base.xui.Recipes.TrackedRecipe;
		base.xui.Recipes.OnTrackedRecipeChanged += this.RecipeTracker_OnTrackedRecipeChanged;
		this.IsDirty = true;
	}

	// Token: 0x06006F7F RID: 28543 RVA: 0x002D84CC File Offset: 0x002D66CC
	public override void OnClose()
	{
		base.OnClose();
		if (XUi.IsGameRunning())
		{
			base.xui.Recipes.OnTrackedRecipeChanged -= this.RecipeTracker_OnTrackedRecipeChanged;
		}
	}

	// Token: 0x06006F80 RID: 28544 RVA: 0x002D84F7 File Offset: 0x002D66F7
	[PublicizedFrom(EAccessModifier.Private)]
	public void RecipeTracker_OnTrackedRecipeChanged()
	{
		this.CurrentRecipe = base.xui.Recipes.TrackedRecipe;
		this.Count = base.xui.Recipes.TrackedRecipeCount;
		this.IsDirty = true;
	}

	// Token: 0x06006F81 RID: 28545 RVA: 0x002D852C File Offset: 0x002D672C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		if (bindingName == "recipename")
		{
			value = ((this.currentRecipe != null) ? Localization.Get(this.currentRecipe.GetName(), false) : "");
			return true;
		}
		if (bindingName == "recipetitle")
		{
			value = ((this.currentRecipe != null) ? (Localization.Get(this.currentRecipe.GetName(), false) + ((this.Count > 1) ? string.Format(" (x{0})", this.Count) : "")) : "");
			return true;
		}
		if (bindingName == "recipeicon")
		{
			value = ((this.currentRecipe != null) ? this.currentRecipe.GetIcon() : "");
			return true;
		}
		if (bindingName == "showrecipe")
		{
			value = (this.currentRecipe != null && XUi.IsGameRunning() && this.localPlayer != null && !this.localPlayer.IsDead()).ToString();
			return true;
		}
		if (bindingName == "showempty")
		{
			value = (this.currentRecipe == null).ToString();
			return true;
		}
		if (!(bindingName == "trackerheight"))
		{
			return false;
		}
		if (this.currentRecipe == null)
		{
			value = "0";
		}
		else
		{
			value = this.trackerheightFormatter.Format(this.ingredientList.GetActiveIngredientCount() * 35);
		}
		return true;
	}

	// Token: 0x0400549D RID: 21661
	public static string ID = "";

	// Token: 0x0400549E RID: 21662
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_RecipeTrackerIngredientsList ingredientList;

	// Token: 0x0400549F RID: 21663
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal localPlayer;

	// Token: 0x040054A0 RID: 21664
	public int Count = 1;

	// Token: 0x040054A1 RID: 21665
	[PublicizedFrom(EAccessModifier.Private)]
	public Recipe currentRecipe;

	// Token: 0x040054A2 RID: 21666
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt trackerheightFormatter = new CachedStringFormatterInt();
}
