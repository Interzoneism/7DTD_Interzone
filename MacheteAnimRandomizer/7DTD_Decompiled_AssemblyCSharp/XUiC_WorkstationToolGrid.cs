using System;
using UnityEngine.Scripting;

// Token: 0x02000EB5 RID: 3765
[Preserve]
public class XUiC_WorkstationToolGrid : XUiC_WorkstationGrid
{
	// Token: 0x140000D2 RID: 210
	// (add) Token: 0x06007719 RID: 30489 RVA: 0x00307B90 File Offset: 0x00305D90
	// (remove) Token: 0x0600771A RID: 30490 RVA: 0x00307BC8 File Offset: 0x00305DC8
	public event XuiEvent_WorkstationItemsChanged OnWorkstationToolsChanged;

	// Token: 0x0600771B RID: 30491 RVA: 0x00307C00 File Offset: 0x00305E00
	public override void Init()
	{
		base.Init();
		string[] array = this.requiredTools.Split(',', StringSplitOptions.None);
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			if (i < array.Length)
			{
				((XUiC_RequiredItemStack)this.itemControllers[i]).RequiredItemClass = ItemClass.GetItemClass(array[i], false);
				((XUiC_RequiredItemStack)this.itemControllers[i]).RequiredItemOnly = this.requiredToolsOnly;
			}
			else
			{
				((XUiC_RequiredItemStack)this.itemControllers[i]).RequiredItemClass = null;
				((XUiC_RequiredItemStack)this.itemControllers[i]).RequiredItemOnly = false;
			}
		}
	}

	// Token: 0x0600771C RID: 30492 RVA: 0x00307C98 File Offset: 0x00305E98
	public override bool HasRequirement(Recipe recipe)
	{
		if (recipe == null)
		{
			return false;
		}
		if (recipe.craftingToolType == 0)
		{
			return true;
		}
		ItemStack[] slots = this.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].itemValue.type == recipe.craftingToolType)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600771D RID: 30493 RVA: 0x00307CE4 File Offset: 0x00305EE4
	public void SetToolLocks(bool locked)
	{
		if (locked != this.isLocked)
		{
			this.isLocked = locked;
			for (int i = 0; i < this.itemControllers.Length; i++)
			{
				this.itemControllers[i].ToolLock = locked;
			}
		}
	}

	// Token: 0x0600771E RID: 30494 RVA: 0x00307D22 File Offset: 0x00305F22
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateBackend(ItemStack[] stackList)
	{
		base.UpdateBackend(stackList);
		this.workstationData.SetToolStacks(stackList);
		this.windowGroup.Controller.SetAllChildrenDirty(false);
	}

	// Token: 0x0600771F RID: 30495 RVA: 0x00307D48 File Offset: 0x00305F48
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(name, value, _parent);
		if (!flag)
		{
			if (!(name == "required_tools"))
			{
				if (!(name == "required_tools_only"))
				{
					return false;
				}
				this.requiredToolsOnly = StringParsers.ParseBool(value, 0, -1, true);
			}
			else
			{
				this.requiredTools = value;
			}
			return true;
		}
		return flag;
	}

	// Token: 0x06007720 RID: 30496 RVA: 0x00307DA0 File Offset: 0x00305FA0
	public bool TryAddTool(ItemClass newItemClass, ItemStack newItemStack)
	{
		if (!this.requiredToolsOnly || this.isLocked)
		{
			return false;
		}
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			XUiC_RequiredItemStack xuiC_RequiredItemStack = (XUiC_RequiredItemStack)this.itemControllers[i];
			if (xuiC_RequiredItemStack.RequiredItemClass == newItemClass && xuiC_RequiredItemStack.ItemStack.IsEmpty())
			{
				xuiC_RequiredItemStack.ItemStack = newItemStack.Clone();
				return true;
			}
		}
		return false;
	}

	// Token: 0x06007721 RID: 30497 RVA: 0x00307E05 File Offset: 0x00306005
	public override void HandleSlotChangedEvent(int slotNumber, ItemStack stack)
	{
		base.HandleSlotChangedEvent(slotNumber, stack);
		if (this.OnWorkstationToolsChanged != null)
		{
			this.OnWorkstationToolsChanged();
		}
	}

	// Token: 0x06007722 RID: 30498 RVA: 0x00307E22 File Offset: 0x00306022
	public override void OnOpen()
	{
		base.OnOpen();
		base.xui.currentWorkstationToolGrid = this;
	}

	// Token: 0x06007723 RID: 30499 RVA: 0x00307E36 File Offset: 0x00306036
	public override void OnClose()
	{
		base.OnClose();
		base.xui.currentWorkstationToolGrid = null;
	}

	// Token: 0x04005AB4 RID: 23220
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isLocked;

	// Token: 0x04005AB5 RID: 23221
	[PublicizedFrom(EAccessModifier.Private)]
	public string requiredTools = "";

	// Token: 0x04005AB6 RID: 23222
	[PublicizedFrom(EAccessModifier.Private)]
	public bool requiredToolsOnly;
}
