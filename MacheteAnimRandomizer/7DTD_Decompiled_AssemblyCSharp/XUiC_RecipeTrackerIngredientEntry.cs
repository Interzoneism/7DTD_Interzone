using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DDF RID: 3551
[Preserve]
public class XUiC_RecipeTrackerIngredientEntry : XUiController
{
	// Token: 0x17000B36 RID: 2870
	// (get) Token: 0x06006F62 RID: 28514 RVA: 0x002D7A87 File Offset: 0x002D5C87
	// (set) Token: 0x06006F63 RID: 28515 RVA: 0x002D7A8F File Offset: 0x002D5C8F
	public XUiC_RecipeTrackerIngredientsList Owner { get; set; }

	// Token: 0x17000B37 RID: 2871
	// (get) Token: 0x06006F64 RID: 28516 RVA: 0x002D7A98 File Offset: 0x002D5C98
	// (set) Token: 0x06006F65 RID: 28517 RVA: 0x002D7AA0 File Offset: 0x002D5CA0
	public ItemStack Ingredient
	{
		get
		{
			return this.ingredient;
		}
		set
		{
			this.ingredient = value;
			if (this.ingredient != null && this.Owner.Recipe.materialBasedRecipe)
			{
				this.ingredient = null;
			}
			if (this.ingredient != null)
			{
				this.currentCount = base.xui.PlayerInventory.GetItemCount(this.ingredient.itemValue);
				this.IsComplete = (this.currentCount >= this.ingredient.count * this.Owner.Count);
			}
			else
			{
				this.currentCount = 0;
				this.IsComplete = false;
			}
			this.isDirty = true;
		}
	}

	// Token: 0x06006F66 RID: 28518 RVA: 0x002D7B40 File Offset: 0x002D5D40
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = this.ingredient != null;
		if (bindingName == "hasingredient")
		{
			value = flag.ToString();
			return true;
		}
		if (bindingName == "itemname")
		{
			value = (flag ? this.ingredient.itemValue.ItemClass.GetLocalizedItemName() : "");
			return true;
		}
		if (bindingName == "itemicon")
		{
			value = (flag ? this.ingredient.itemValue.ItemClass.GetIconName() : "");
			return true;
		}
		if (bindingName == "itemicontint")
		{
			Color32 v = Color.white;
			if (flag)
			{
				ItemClass itemClass = this.ingredient.itemValue.ItemClass;
				if (itemClass != null)
				{
					v = itemClass.GetIconTint(this.ingredient.itemValue);
				}
			}
			value = this.itemicontintcolorFormatter.Format(v);
			return true;
		}
		if (bindingName == "itemcount")
		{
			if (flag)
			{
				int v2 = this.ingredient.count * this.Owner.Count;
				value = this.countFormatter.Format(this.currentCount, v2);
			}
			return true;
		}
		if (!(bindingName == "ingredientcompletehexcolor"))
		{
			return false;
		}
		if (flag)
		{
			value = ((this.currentCount >= this.ingredient.count * this.Owner.Count) ? this.Owner.completeHexColor : this.Owner.incompleteHexColor);
		}
		else
		{
			value = "FFFFFF";
		}
		return true;
	}

	// Token: 0x06006F67 RID: 28519 RVA: 0x002BF216 File Offset: 0x002BD416
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnCountChanged(XUiController _sender, OnCountChangedEventArgs _e)
	{
		base.RefreshBindings(true);
	}

	// Token: 0x06006F68 RID: 28520 RVA: 0x002D7CC3 File Offset: 0x002D5EC3
	public override void Update(float _dt)
	{
		if (this.isDirty)
		{
			base.RefreshBindings(false);
			base.ViewComponent.IsVisible = true;
			this.isDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x04005483 RID: 21635
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack ingredient;

	// Token: 0x04005484 RID: 21636
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x04005485 RID: 21637
	[PublicizedFrom(EAccessModifier.Private)]
	public int currentCount;

	// Token: 0x04005486 RID: 21638
	public bool IsComplete;

	// Token: 0x04005488 RID: 21640
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor itemicontintcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04005489 RID: 21641
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<int, int> countFormatter = new CachedStringFormatter<int, int>((int _i1, int _i2) => _i1.ToString() + "/" + _i2.ToString());
}
