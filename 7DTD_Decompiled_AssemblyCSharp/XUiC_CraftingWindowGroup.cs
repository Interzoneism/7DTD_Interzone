using System;
using UnityEngine.Scripting;

// Token: 0x02000C69 RID: 3177
[Preserve]
public class XUiC_CraftingWindowGroup : XUiController
{
	// Token: 0x17000A0E RID: 2574
	// (get) Token: 0x060061D3 RID: 25043 RVA: 0x0027B27E File Offset: 0x0027947E
	// (set) Token: 0x060061D4 RID: 25044 RVA: 0x0027B286 File Offset: 0x00279486
	public string Workstation
	{
		get
		{
			return this.workstation;
		}
		set
		{
			this.workstation = value;
			if (this.recipeList != null)
			{
				this.recipeList.Workstation = this.workstation;
			}
		}
	}

	// Token: 0x060061D5 RID: 25045 RVA: 0x0027B2A8 File Offset: 0x002794A8
	public override void Init()
	{
		base.Init();
		this.recipeList = base.GetChildByType<XUiC_RecipeList>();
		this.craftingQueue = base.GetChildByType<XUiC_CraftingQueue>();
		this.craftCountControl = base.GetChildByType<XUiC_RecipeCraftCount>();
		this.categoryList = base.GetChildByType<XUiC_CategoryList>();
		this.craftInfoWindow = base.GetChildByType<XUiC_CraftingInfoWindow>();
		this.recipeList.InfoWindow = this.craftInfoWindow;
	}

	// Token: 0x060061D6 RID: 25046 RVA: 0x0027B308 File Offset: 0x00279508
	public virtual bool AddItemToQueue(Recipe _recipe)
	{
		if (this.craftCountControl != null)
		{
			return this.craftingQueue.AddRecipeToCraft(_recipe, this.craftCountControl.Count, -1f, true, -1f);
		}
		return this.craftingQueue.AddRecipeToCraft(_recipe, 1, -1f, true, -1f);
	}

	// Token: 0x060061D7 RID: 25047 RVA: 0x0027B358 File Offset: 0x00279558
	public virtual bool AddItemToQueue(Recipe _recipe, int _count)
	{
		return this.craftingQueue.AddRecipeToCraft(_recipe, _count, -1f, true, -1f);
	}

	// Token: 0x060061D8 RID: 25048 RVA: 0x0027B372 File Offset: 0x00279572
	public virtual bool AddItemToQueue(Recipe _recipe, int _count, float _craftTime)
	{
		return this.craftingQueue.AddRecipeToCraft(_recipe, _count, _craftTime, true, -1f);
	}

	// Token: 0x060061D9 RID: 25049 RVA: 0x0027B388 File Offset: 0x00279588
	public virtual bool AddRepairItemToQueue(float _repairTime, ItemValue _itemToRepair, int _amountToRepair)
	{
		return this.craftingQueue.AddItemToRepair(_repairTime, _itemToRepair, _amountToRepair);
	}

	// Token: 0x060061DA RID: 25050 RVA: 0x0027B398 File Offset: 0x00279598
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.recipeList != null && this.categoryList != null && this.categoryList.SetupCategoriesByWorkstation(""))
		{
			this.recipeList.Workstation = "";
			this.recipeList.SetCategory("Basics");
			this.categoryList.SetCategory("Basics");
		}
		base.xui.playerUI.windowManager.OpenIfNotOpen("windowpaging", false, false, true);
		XUiC_WindowSelector childByType = base.xui.FindWindowGroupByName("windowpaging").GetChildByType<XUiC_WindowSelector>();
		if (childByType != null)
		{
			childByType.SetSelected("crafting");
		}
		base.xui.currentWorkstation = "";
	}

	// Token: 0x060061DB RID: 25051 RVA: 0x0027B44E File Offset: 0x0027964E
	public override void OnClose()
	{
		base.OnClose();
		base.xui.playerUI.windowManager.CloseIfOpen("windowpaging");
		base.xui.currentWorkstation = "";
	}

	// Token: 0x060061DC RID: 25052 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool CraftingRequirementsValid(Recipe _recipe)
	{
		return true;
	}

	// Token: 0x060061DD RID: 25053 RVA: 0x0002B133 File Offset: 0x00029333
	public virtual string CraftingRequirementsInvalidMessage(Recipe _recipe)
	{
		return "";
	}

	// Token: 0x060061DE RID: 25054 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AlwaysUpdate()
	{
		return true;
	}

	// Token: 0x0400499F RID: 18847
	[PublicizedFrom(EAccessModifier.Protected)]
	public string workstation = "";

	// Token: 0x040049A0 RID: 18848
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool firstRun = true;

	// Token: 0x040049A1 RID: 18849
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiC_CategoryList categoryList;

	// Token: 0x040049A2 RID: 18850
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiC_RecipeList recipeList;

	// Token: 0x040049A3 RID: 18851
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiC_CraftingQueue craftingQueue;

	// Token: 0x040049A4 RID: 18852
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiC_RecipeCraftCount craftCountControl;

	// Token: 0x040049A5 RID: 18853
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiC_CraftingInfoWindow craftInfoWindow;
}
