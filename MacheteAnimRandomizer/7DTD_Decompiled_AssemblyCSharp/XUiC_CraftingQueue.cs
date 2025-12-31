using System;
using UnityEngine.Scripting;

// Token: 0x02000DCE RID: 3534
[Preserve]
public class XUiC_CraftingQueue : XUiController
{
	// Token: 0x06006E92 RID: 28306 RVA: 0x002D1D6C File Offset: 0x002CFF6C
	public override void Init()
	{
		base.Init();
		XUiController[] childrenByType = base.GetChildrenByType<XUiC_RecipeStack>(null);
		this.queueItems = childrenByType;
		for (int i = 0; i < this.queueItems.Length; i++)
		{
			((XUiC_RecipeStack)this.queueItems[i]).Owner = this;
		}
	}

	// Token: 0x06006E93 RID: 28307 RVA: 0x002D1DB4 File Offset: 0x002CFFB4
	public void ClearQueue()
	{
		for (int i = this.queueItems.Length - 1; i >= 0; i--)
		{
			XUiC_RecipeStack xuiC_RecipeStack = (XUiC_RecipeStack)this.queueItems[i];
			xuiC_RecipeStack.SetRecipe(null, 0, 0f, true, -1, -1, -1f);
			xuiC_RecipeStack.IsCrafting = false;
			xuiC_RecipeStack.IsDirty = true;
		}
	}

	// Token: 0x06006E94 RID: 28308 RVA: 0x002D1E06 File Offset: 0x002D0006
	public void HaltCrafting()
	{
		((XUiC_RecipeStack)this.queueItems[this.queueItems.Length - 1]).IsCrafting = false;
	}

	// Token: 0x06006E95 RID: 28309 RVA: 0x002D1E24 File Offset: 0x002D0024
	public void ResumeCrafting()
	{
		((XUiC_RecipeStack)this.queueItems[this.queueItems.Length - 1]).IsCrafting = true;
	}

	// Token: 0x06006E96 RID: 28310 RVA: 0x002D1E42 File Offset: 0x002D0042
	public bool IsCrafting()
	{
		return ((XUiC_RecipeStack)this.queueItems[this.queueItems.Length - 1]).IsCrafting;
	}

	// Token: 0x06006E97 RID: 28311 RVA: 0x002D1E60 File Offset: 0x002D0060
	public bool AddItemToRepair(float _repairTimeLeft, ItemValue _itemToRepair, int _amountToRepair)
	{
		for (int i = this.queueItems.Length - 1; i >= 0; i--)
		{
			XUiC_RecipeStack xuiC_RecipeStack = (XUiC_RecipeStack)this.queueItems[i];
			if (!xuiC_RecipeStack.HasRecipe() && xuiC_RecipeStack.SetRepairRecipe(_repairTimeLeft, _itemToRepair, _amountToRepair))
			{
				xuiC_RecipeStack.IsCrafting = (i == this.queueItems.Length - 1);
				xuiC_RecipeStack.IsDirty = true;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06006E98 RID: 28312 RVA: 0x002D1EC0 File Offset: 0x002D00C0
	public bool AddRecipeToCraft(Recipe _recipe, int _count = 1, float craftTime = -1f, bool isCrafting = true, float _oneItemCraftingTime = -1f)
	{
		for (int i = this.queueItems.Length - 1; i >= 0; i--)
		{
			if (this.AddRecipeToCraftAtIndex(i, _recipe, _count, craftTime, isCrafting, false, -1, -1, _oneItemCraftingTime))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06006E99 RID: 28313 RVA: 0x002D1EF8 File Offset: 0x002D00F8
	public bool AddRecipeToCraftAtIndex(int _index, Recipe _recipe, int _count = 1, float craftTime = -1f, bool isCrafting = true, bool recipeModification = false, int lastQuality = -1, int startingEntityId = -1, float _oneItemCraftingTime = -1f)
	{
		XUiC_RecipeStack xuiC_RecipeStack = (XUiC_RecipeStack)this.queueItems[_index];
		if (xuiC_RecipeStack.SetRecipe(_recipe, _count, craftTime, recipeModification, -1, -1, _oneItemCraftingTime))
		{
			xuiC_RecipeStack.IsCrafting = (_index == this.queueItems.Length - 1 && isCrafting);
			if (lastQuality != -1)
			{
				xuiC_RecipeStack.OutputQuality = lastQuality;
			}
			if (startingEntityId != -1)
			{
				xuiC_RecipeStack.StartingEntityId = startingEntityId;
			}
			xuiC_RecipeStack.IsDirty = true;
			return true;
		}
		return false;
	}

	// Token: 0x06006E9A RID: 28314 RVA: 0x002D1F60 File Offset: 0x002D0160
	public bool AddItemToRepairAtIndex(int _index, float _repairTimeLeft, ItemValue _itemToRepair, int _amountToRepair, bool isCrafting = true, int startingEntityId = -1)
	{
		XUiC_RecipeStack xuiC_RecipeStack = (XUiC_RecipeStack)this.queueItems[_index];
		if (xuiC_RecipeStack.SetRepairRecipe(_repairTimeLeft, _itemToRepair, _amountToRepair))
		{
			xuiC_RecipeStack.IsCrafting = (_index == this.queueItems.Length - 1);
			xuiC_RecipeStack.StartingEntityId = ((startingEntityId != -1) ? startingEntityId : xuiC_RecipeStack.StartingEntityId);
			xuiC_RecipeStack.IsDirty = true;
			return true;
		}
		return false;
	}

	// Token: 0x06006E9B RID: 28315 RVA: 0x002D1FBC File Offset: 0x002D01BC
	public XUiC_RecipeStack[] GetRecipesToCraft()
	{
		XUiC_RecipeStack[] array = new XUiC_RecipeStack[this.queueItems.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (XUiC_RecipeStack)this.queueItems[i];
		}
		return array;
	}

	// Token: 0x06006E9C RID: 28316 RVA: 0x002D1FF8 File Offset: 0x002D01F8
	public override void Update(float _dt)
	{
		base.Update(_dt);
		bool flag = false;
		for (int i = this.queueItems.Length - 1; i >= 0; i--)
		{
			if (((XUiC_RecipeStack)this.queueItems[i]).HasRecipe())
			{
				flag = true;
				break;
			}
		}
		if (this.toolGrid != null)
		{
			this.toolGrid.SetToolLocks(flag);
		}
		if (!flag)
		{
			return;
		}
		XUiC_RecipeStack xuiC_RecipeStack = (XUiC_RecipeStack)this.queueItems[this.queueItems.Length - 1];
		if (!xuiC_RecipeStack.HasRecipe())
		{
			for (int j = this.queueItems.Length - 1; j >= 0; j--)
			{
				XUiC_RecipeStack recipeStack = (XUiC_RecipeStack)this.queueItems[j];
				if (j != 0)
				{
					((XUiC_RecipeStack)this.queueItems[j - 1]).CopyTo(recipeStack);
				}
				else
				{
					((XUiC_RecipeStack)this.queueItems[0]).SetRecipe(null, 0, 0f, true, -1, -1, -1f);
				}
			}
		}
		if (xuiC_RecipeStack.HasRecipe() && !xuiC_RecipeStack.IsCrafting)
		{
			xuiC_RecipeStack.IsCrafting = true;
		}
	}

	// Token: 0x06006E9D RID: 28317 RVA: 0x002D20EA File Offset: 0x002D02EA
	public override void OnOpen()
	{
		base.OnOpen();
		this.toolGrid = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationToolGrid>();
	}

	// Token: 0x06006E9E RID: 28318 RVA: 0x002D2108 File Offset: 0x002D0308
	public void RefreshQueue()
	{
		XUiC_RecipeStack xuiC_RecipeStack = null;
		for (int i = this.queueItems.Length - 1; i >= 0; i--)
		{
			XUiC_RecipeStack xuiC_RecipeStack2 = (XUiC_RecipeStack)this.queueItems[i];
			if (xuiC_RecipeStack2.GetRecipe() != null && xuiC_RecipeStack != null && xuiC_RecipeStack.GetRecipe() == null)
			{
				xuiC_RecipeStack2.CopyTo(xuiC_RecipeStack);
				xuiC_RecipeStack2.SetRecipe(null, 0, 0f, true, -1, -1, -1f);
			}
			xuiC_RecipeStack = xuiC_RecipeStack2;
		}
	}

	// Token: 0x04005413 RID: 21523
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WorkstationToolGrid toolGrid;

	// Token: 0x04005414 RID: 21524
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController[] queueItems;
}
