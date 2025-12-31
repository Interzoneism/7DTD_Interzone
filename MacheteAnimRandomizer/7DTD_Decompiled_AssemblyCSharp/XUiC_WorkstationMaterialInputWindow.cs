using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000EB2 RID: 3762
[Preserve]
public class XUiC_WorkstationMaterialInputWindow : XUiController
{
	// Token: 0x140000D1 RID: 209
	// (add) Token: 0x06007701 RID: 30465 RVA: 0x0030745C File Offset: 0x0030565C
	// (remove) Token: 0x06007702 RID: 30466 RVA: 0x00307494 File Offset: 0x00305694
	public event XuiEvent_WorkstationItemsChanged OnWorkstationMaterialWeightsChanged;

	// Token: 0x06007703 RID: 30467 RVA: 0x003074CC File Offset: 0x003056CC
	public override void Init()
	{
		base.Init();
		this.materialTitles = base.GetChildrenById("material", null);
		this.materialWeights = base.GetChildrenById("weight", null);
		this.inputGrid = base.GetChildByType<XUiC_WorkstationMaterialInputGrid>();
		if (this.inputGrid == null)
		{
			Log.Error("Input Grid not found!");
		}
		if (this.materialWeights[0] != null)
		{
			this.baseTextColor = ((XUiV_Label)this.materialWeights[0].ViewComponent).Color;
		}
	}

	// Token: 0x06007704 RID: 30468 RVA: 0x00307548 File Offset: 0x00305748
	public override void OnOpen()
	{
		base.OnOpen();
		this.MaterialNames = this.inputGrid.WorkstationData.GetMaterialNames();
		for (int i = 0; i < this.MaterialNames.Length; i++)
		{
			string str = XUi.UppercaseFirst(this.MaterialNames[i]);
			if (Localization.Exists("lbl" + this.MaterialNames[i], false))
			{
				str = Localization.Get("lbl" + this.MaterialNames[i], false);
			}
			((XUiV_Label)this.materialTitles[i].ViewComponent).Text = str + ":";
		}
		XUiC_RecipeList childByType = this.windowGroup.Controller.GetChildByType<XUiC_RecipeList>();
		if (childByType != null)
		{
			childByType.RecipeChanged += this.RecipeList_RecipeChanged;
		}
	}

	// Token: 0x06007705 RID: 30469 RVA: 0x0030760D File Offset: 0x0030580D
	[PublicizedFrom(EAccessModifier.Private)]
	public void onForgeValuesChanged()
	{
		if (this.OnWorkstationMaterialWeightsChanged != null)
		{
			this.OnWorkstationMaterialWeightsChanged();
		}
	}

	// Token: 0x06007706 RID: 30470 RVA: 0x00307624 File Offset: 0x00305824
	public override void OnClose()
	{
		base.OnClose();
		XUiC_RecipeList childByType = this.windowGroup.Controller.GetChildByType<XUiC_RecipeList>();
		if (childByType != null)
		{
			childByType.RecipeChanged -= this.RecipeList_RecipeChanged;
		}
	}

	// Token: 0x06007707 RID: 30471 RVA: 0x00307660 File Offset: 0x00305860
	public override void Update(float _dt)
	{
		if (!this.windowGroup.isShowing)
		{
			return;
		}
		if (this.weights == null)
		{
			this.weights = new int[this.MaterialNames.Length];
			this.SetMaterialWeights(this.inputGrid.WorkstationData.GetInputStacks());
		}
		for (int i = 0; i < this.weights.Length; i++)
		{
			((XUiV_Label)this.materialWeights[i].ViewComponent).Text = string.Format("{0}", this.weights[i].ToString());
		}
		base.Update(_dt);
	}

	// Token: 0x06007708 RID: 30472 RVA: 0x003076F8 File Offset: 0x003058F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void RecipeList_RecipeChanged(Recipe _recipe, XUiC_RecipeEntry recipeEntry)
	{
		this.ResetWeightColors();
	}

	// Token: 0x06007709 RID: 30473 RVA: 0x00307700 File Offset: 0x00305900
	public bool HasRequirement(Recipe recipe)
	{
		if (this.weights == null)
		{
			return true;
		}
		if (recipe == null)
		{
			this.ResetWeightColors();
			return true;
		}
		for (int i = 0; i < this.weights.Length; i++)
		{
			((XUiV_Label)this.materialWeights[i].ViewComponent).Color = this.baseTextColor;
			for (int j = 0; j < recipe.ingredients.Count; j++)
			{
				int num = (int)EffectManager.GetValue(PassiveEffects.CraftingIngredientCount, null, (float)recipe.ingredients[j].count, base.xui.playerUI.entityPlayer, recipe, FastTags<TagGroup.Global>.Parse(recipe.ingredients[j].itemValue.ItemClass.GetItemName()), true, true, true, true, true, recipe.GetCraftingTier(base.xui.playerUI.entityPlayer), true, false);
				ItemClass forId = ItemClass.GetForId(recipe.ingredients[j].itemValue.type);
				if (forId != null)
				{
					if (forId.MadeOfMaterial.ForgeCategory.EqualsCaseInsensitive(this.MaterialNames[i]))
					{
						if (num <= this.weights[i])
						{
							((XUiV_Label)this.materialWeights[i].ViewComponent).Color = Color.green;
							break;
						}
						((XUiV_Label)this.materialWeights[i].ViewComponent).Color = Color.red;
						break;
					}
					else
					{
						((XUiV_Label)this.materialWeights[i].ViewComponent).Color = this.baseTextColor;
					}
				}
			}
		}
		return true;
	}

	// Token: 0x0600770A RID: 30474 RVA: 0x00307880 File Offset: 0x00305A80
	[PublicizedFrom(EAccessModifier.Private)]
	public void ResetWeightColors()
	{
		for (int i = 0; i < this.weights.Length; i++)
		{
			((XUiV_Label)this.materialWeights[i].ViewComponent).Color = this.baseTextColor;
		}
	}

	// Token: 0x0600770B RID: 30475 RVA: 0x003078C0 File Offset: 0x00305AC0
	public void SetMaterialWeights(ItemStack[] stackList)
	{
		for (int i = 3; i < stackList.Length; i++)
		{
			if (this.weights != null && stackList[i] != null)
			{
				this.weights[i - 3] = stackList[i].count;
			}
		}
		this.onForgeValuesChanged();
	}

	// Token: 0x0600770C RID: 30476 RVA: 0x00307900 File Offset: 0x00305B00
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(name, value, _parent);
		if (flag)
		{
			return flag;
		}
		if (name == "materials")
		{
			if (value.Contains(","))
			{
				this.MaterialNames = value.Replace(" ", "").Split(',', StringSplitOptions.None);
				this.weights = new int[this.MaterialNames.Length];
			}
			else
			{
				this.MaterialNames = new string[]
				{
					value
				};
			}
			return true;
		}
		if (name == "valid_materials_color")
		{
			this.validColor = StringParsers.ParseColor32(value);
			return true;
		}
		if (!(name == "invalid_materials_color"))
		{
			return false;
		}
		this.invalidColor = StringParsers.ParseColor32(value);
		return true;
	}

	// Token: 0x0600770D RID: 30477 RVA: 0x003079B9 File Offset: 0x00305BB9
	[PublicizedFrom(EAccessModifier.Private)]
	public float calculateWeightOunces(int materialIndex)
	{
		return (float)this.weights[materialIndex];
	}

	// Token: 0x0600770E RID: 30478 RVA: 0x003079C4 File Offset: 0x00305BC4
	[PublicizedFrom(EAccessModifier.Private)]
	public float calculateWeightPounds(int materialIndex)
	{
		return this.calculateWeightOunces(materialIndex) / 16f;
	}

	// Token: 0x04005AA8 RID: 23208
	[PublicizedFrom(EAccessModifier.Private)]
	public const float OUNCES_IN_POUND = 16f;

	// Token: 0x04005AA9 RID: 23209
	public string[] MaterialNames;

	// Token: 0x04005AAA RID: 23210
	[PublicizedFrom(EAccessModifier.Private)]
	public int[] weights;

	// Token: 0x04005AAB RID: 23211
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController[] materialTitles;

	// Token: 0x04005AAC RID: 23212
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController[] materialWeights;

	// Token: 0x04005AAD RID: 23213
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WorkstationMaterialInputGrid inputGrid;

	// Token: 0x04005AAF RID: 23215
	[PublicizedFrom(EAccessModifier.Private)]
	public Color baseTextColor;

	// Token: 0x04005AB0 RID: 23216
	[PublicizedFrom(EAccessModifier.Private)]
	public Color validColor = Color.green;

	// Token: 0x04005AB1 RID: 23217
	[PublicizedFrom(EAccessModifier.Private)]
	public Color invalidColor = Color.red;
}
