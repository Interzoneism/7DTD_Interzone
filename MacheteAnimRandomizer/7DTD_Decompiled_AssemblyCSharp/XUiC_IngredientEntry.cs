using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000CCC RID: 3276
[Preserve]
public class XUiC_IngredientEntry : XUiController
{
	// Token: 0x17000A56 RID: 2646
	// (get) Token: 0x06006565 RID: 25957 RVA: 0x00291766 File Offset: 0x0028F966
	// (set) Token: 0x06006566 RID: 25958 RVA: 0x0029176E File Offset: 0x0028F96E
	public Recipe Recipe { get; set; }

	// Token: 0x17000A57 RID: 2647
	// (get) Token: 0x06006567 RID: 25959 RVA: 0x00291777 File Offset: 0x0028F977
	// (set) Token: 0x06006568 RID: 25960 RVA: 0x00291780 File Offset: 0x0028F980
	public ItemStack Ingredient
	{
		get
		{
			return this.ingredient;
		}
		set
		{
			this.ingredient = value;
			if (this.ingredient != null)
			{
				this.materialBased = ((XUiC_IngredientList)this.parent).Recipe.materialBasedRecipe;
				if (this.ingredient.itemValue.ItemClass != null)
				{
					this.material = this.ingredient.itemValue.ItemClass.MadeOfMaterial.ForgeCategory;
					if (this.material == null)
					{
						this.material = "";
					}
				}
			}
			this.isDirty = true;
		}
	}

	// Token: 0x06006569 RID: 25961 RVA: 0x00291804 File Offset: 0x0028FA04
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("icon");
		if (childById != null)
		{
			this.icoItem = (childById.ViewComponent as XUiV_Sprite);
		}
		XUiController childById2 = base.GetChildById("name");
		if (childById2 != null)
		{
			this.lblName = (childById2.ViewComponent as XUiV_Label);
		}
		childById2 = base.GetChildById("havecount");
		if (childById2 != null)
		{
			this.lblHaveCount = (childById2.ViewComponent as XUiV_Label);
		}
		childById2 = base.GetChildById("needcount");
		if (childById2 != null)
		{
			this.lblNeedCount = (childById2.ViewComponent as XUiV_Label);
		}
		this.craftCountControl = this.windowGroup.Controller.GetChildByType<XUiC_RecipeCraftCount>();
		this.craftCountControl.OnCountChanged += this.HandleOnCountChanged;
		this.isDirty = false;
	}

	// Token: 0x0600656A RID: 25962 RVA: 0x002918CC File Offset: 0x0028FACC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = this.ingredient != null;
		if (bindingName == "itemname")
		{
			string text = "";
			if (flag && this.materialBased)
			{
				if (Localization.Exists("lbl" + this.material, false))
				{
					text = Localization.Get("lbl" + this.material, false);
				}
				else
				{
					text = XUi.UppercaseFirst(this.material);
				}
			}
			value = (flag ? (this.materialBased ? text : this.ingredient.itemValue.ItemClass.GetLocalizedItemName()) : "");
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
		if (bindingName == "havecount")
		{
			XUiC_WorkstationMaterialInputGrid childByType = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationMaterialInputGrid>();
			if (childByType != null)
			{
				if (this.materialBased)
				{
					value = (flag ? this.havecountFormatter.Format(childByType.GetWeight(this.material)) : "");
				}
				else
				{
					value = (flag ? this.havecountFormatter.Format(base.xui.PlayerInventory.GetItemCount(this.ingredient.itemValue)) : "");
				}
			}
			else
			{
				XUiC_WorkstationInputGrid childByType2 = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationInputGrid>();
				if (childByType2 != null)
				{
					value = (flag ? this.havecountFormatter.Format(childByType2.GetItemCount(this.ingredient.itemValue)) : "");
				}
				else
				{
					value = (flag ? this.havecountFormatter.Format(base.xui.PlayerInventory.GetItemCount(this.ingredient.itemValue)) : "");
				}
			}
			return true;
		}
		if (bindingName == "needcount")
		{
			value = (flag ? this.needcountFormatter.Format(this.ingredient.count * this.craftCountControl.Count) : "");
			return true;
		}
		if (!(bindingName == "haveneedcount"))
		{
			return false;
		}
		string str = flag ? this.needcountFormatter.Format(this.ingredient.count * this.craftCountControl.Count) : "";
		XUiC_WorkstationMaterialInputGrid childByType3 = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationMaterialInputGrid>();
		if (childByType3 != null)
		{
			if (this.materialBased)
			{
				value = (flag ? (this.havecountFormatter.Format(childByType3.GetWeight(this.material)) + "/" + str) : "");
			}
			else
			{
				value = (flag ? (this.havecountFormatter.Format(base.xui.PlayerInventory.GetItemCount(this.ingredient.itemValue)) + "/" + str) : "");
			}
		}
		else
		{
			XUiC_WorkstationInputGrid childByType4 = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationInputGrid>();
			if (childByType4 != null)
			{
				value = (flag ? (this.havecountFormatter.Format(childByType4.GetItemCount(this.ingredient.itemValue)) + "/" + str) : "");
			}
			else
			{
				value = (flag ? (this.havecountFormatter.Format(base.xui.PlayerInventory.GetItemCount(this.ingredient.itemValue)) + "/" + str) : "");
			}
		}
		return true;
	}

	// Token: 0x0600656B RID: 25963 RVA: 0x00291CA2 File Offset: 0x0028FEA2
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnCountChanged(XUiController _sender, OnCountChangedEventArgs _e)
	{
		this.isDirty = true;
	}

	// Token: 0x0600656C RID: 25964 RVA: 0x00291CAB File Offset: 0x0028FEAB
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

	// Token: 0x04004C8A RID: 19594
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack ingredient;

	// Token: 0x04004C8B RID: 19595
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x04004C8C RID: 19596
	[PublicizedFrom(EAccessModifier.Private)]
	public bool materialBased;

	// Token: 0x04004C8D RID: 19597
	[PublicizedFrom(EAccessModifier.Private)]
	public string material = "";

	// Token: 0x04004C8E RID: 19598
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite icoItem;

	// Token: 0x04004C8F RID: 19599
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblName;

	// Token: 0x04004C90 RID: 19600
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblHaveCount;

	// Token: 0x04004C91 RID: 19601
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblNeedCount;

	// Token: 0x04004C92 RID: 19602
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_RecipeCraftCount craftCountControl;

	// Token: 0x04004C94 RID: 19604
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor itemicontintcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04004C95 RID: 19605
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt havecountFormatter = new CachedStringFormatterInt();

	// Token: 0x04004C96 RID: 19606
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt needcountFormatter = new CachedStringFormatterInt();
}
