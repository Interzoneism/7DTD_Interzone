using System;
using UnityEngine.Scripting;

// Token: 0x02000C67 RID: 3175
[Preserve]
public class XUiC_CraftingListInfo : XUiController
{
	// Token: 0x17000A0D RID: 2573
	// (get) Token: 0x060061C9 RID: 25033 RVA: 0x0027B081 File Offset: 0x00279281
	// (set) Token: 0x060061CA RID: 25034 RVA: 0x0027B089 File Offset: 0x00279289
	public string CategoryName
	{
		get
		{
			return this.categoryName;
		}
		set
		{
			this.isDirty |= (this.categoryName != value);
			this.categoryName = value;
		}
	}

	// Token: 0x060061CB RID: 25035 RVA: 0x0027B0AC File Offset: 0x002792AC
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("windowicon");
		if (childById != null)
		{
			this.icoCategoryIcon = (childById.ViewComponent as XUiV_Sprite);
		}
		XUiController childById2 = base.GetChildById("windowname");
		if (childById2 != null)
		{
			this.lblName = (childById2.ViewComponent as XUiV_Label);
		}
		childById2 = base.GetChildById("unlockedcount");
		if (childById2 != null)
		{
			this.lblUnlockedCount = (childById2.ViewComponent as XUiV_Label);
		}
		XUiController childById3 = base.GetChildById("categories");
		if (childById3 != null)
		{
			this.categoryList = (XUiC_CategoryList)childById3;
			this.categoryList.CategoryChanged += this.HandleCategoryChanged;
		}
		this.isDirty = true;
		this.IsDormant = true;
	}

	// Token: 0x060061CC RID: 25036 RVA: 0x0027B15F File Offset: 0x0027935F
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleCategoryChanged(XUiC_CategoryEntry _categoryEntry)
	{
		this.CategoryName = _categoryEntry.CategoryDisplayName;
		this.spriteName = _categoryEntry.SpriteName;
	}

	// Token: 0x060061CD RID: 25037 RVA: 0x0027B17C File Offset: 0x0027937C
	public override void Update(float _dt)
	{
		if (!this.windowGroup.isShowing)
		{
			return;
		}
		if (this.isDirty)
		{
			if (this.lblName != null)
			{
				this.lblName.Text = this.categoryName;
			}
			if (this.icoCategoryIcon != null)
			{
				this.icoCategoryIcon.SpriteName = this.spriteName;
			}
			if (this.lblUnlockedCount != null)
			{
				this.lblUnlockedCount.Text = string.Format("{0}/{1}", CraftingManager.GetUnlockedRecipeCount(), CraftingManager.GetLockedRecipeCount());
			}
			this.isDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x060061CE RID: 25038 RVA: 0x0027B210 File Offset: 0x00279410
	public override void OnOpen()
	{
		base.OnOpen();
		this.isDirty = true;
		this.IsDormant = false;
		CraftingManager.RecipeUnlocked += this.CraftingManager_RecipeUnlocked;
	}

	// Token: 0x060061CF RID: 25039 RVA: 0x0027B237 File Offset: 0x00279437
	public override void OnClose()
	{
		base.OnClose();
		this.IsDormant = true;
		CraftingManager.RecipeUnlocked -= this.CraftingManager_RecipeUnlocked;
	}

	// Token: 0x060061D0 RID: 25040 RVA: 0x0027B257 File Offset: 0x00279457
	[PublicizedFrom(EAccessModifier.Private)]
	public void CraftingManager_RecipeUnlocked(string recipeName)
	{
		this.isDirty = true;
	}

	// Token: 0x04004997 RID: 18839
	[PublicizedFrom(EAccessModifier.Private)]
	public string categoryName = "Basics";

	// Token: 0x04004998 RID: 18840
	[PublicizedFrom(EAccessModifier.Private)]
	public string spriteName = "Craft_Icon_Basics";

	// Token: 0x04004999 RID: 18841
	[PublicizedFrom(EAccessModifier.Private)]
	public string unlockedCount;

	// Token: 0x0400499A RID: 18842
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x0400499B RID: 18843
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblName;

	// Token: 0x0400499C RID: 18844
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite icoCategoryIcon;

	// Token: 0x0400499D RID: 18845
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblUnlockedCount;

	// Token: 0x0400499E RID: 18846
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CategoryList categoryList;
}
