using System;
using UnityEngine.Scripting;

// Token: 0x02000D8A RID: 3466
[Preserve]
public class XUiC_PowerSourceSlots : XUiC_ItemStackGrid
{
	// Token: 0x17000AE7 RID: 2791
	// (get) Token: 0x06006C69 RID: 27753 RVA: 0x002C4D5E File Offset: 0x002C2F5E
	// (set) Token: 0x06006C6A RID: 27754 RVA: 0x002C4D66 File Offset: 0x002C2F66
	public TileEntityPowerSource TileEntity
	{
		get
		{
			return this.tileEntity;
		}
		set
		{
			this.tileEntity = value;
			this.SetSlots(this.tileEntity.ItemSlots);
		}
	}

	// Token: 0x06006C6B RID: 27755 RVA: 0x002C4D80 File Offset: 0x002C2F80
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetRequirements()
	{
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			XUiC_RequiredItemStack xuiC_RequiredItemStack = this.itemControllers[i] as XUiC_RequiredItemStack;
			if (xuiC_RequiredItemStack != null)
			{
				xuiC_RequiredItemStack.RequiredType = XUiC_RequiredItemStack.RequiredTypes.ItemClass;
				xuiC_RequiredItemStack.RequiredItemClass = this.tileEntity.SlotItem;
			}
		}
	}

	// Token: 0x17000AE8 RID: 2792
	// (get) Token: 0x06006C6C RID: 27756 RVA: 0x00076E19 File Offset: 0x00075019
	public override XUiC_ItemStack.StackLocationTypes StackLocation
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return XUiC_ItemStack.StackLocationTypes.Workstation;
		}
	}

	// Token: 0x17000AE9 RID: 2793
	// (get) Token: 0x06006C6D RID: 27757 RVA: 0x002C4DC9 File Offset: 0x002C2FC9
	// (set) Token: 0x06006C6E RID: 27758 RVA: 0x002C4DD1 File Offset: 0x002C2FD1
	public XUiC_PowerSourceWindowGroup Owner { get; set; }

	// Token: 0x06006C6F RID: 27759 RVA: 0x002C40A2 File Offset: 0x002C22A2
	public virtual void SetSlots(ItemStack[] stacks)
	{
		this.items = stacks;
		base.SetStacks(stacks);
	}

	// Token: 0x06006C70 RID: 27760 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool HasRequirement(Recipe recipe)
	{
		return true;
	}

	// Token: 0x06006C71 RID: 27761 RVA: 0x002C4DDC File Offset: 0x002C2FDC
	public override void OnOpen()
	{
		base.OnOpen();
		if (base.ViewComponent != null && !base.ViewComponent.IsVisible)
		{
			base.ViewComponent.OnOpen();
			base.ViewComponent.IsVisible = true;
		}
		this.IsDirty = true;
		this.SetRequirements();
		base.xui.powerSourceSlots = this;
		XUiC_PowerSourceSlots.Current = this;
		this.IsDormant = false;
	}

	// Token: 0x06006C72 RID: 27762 RVA: 0x002C4E44 File Offset: 0x002C3044
	public override void OnClose()
	{
		base.OnClose();
		if (base.ViewComponent != null && base.ViewComponent.IsVisible)
		{
			base.ViewComponent.OnClose();
			base.ViewComponent.IsVisible = false;
		}
		this.IsDirty = true;
		XUiC_PowerSourceSlots.Current = (base.xui.powerSourceSlots = null);
		this.IsDormant = true;
	}

	// Token: 0x06006C73 RID: 27763 RVA: 0x002C4EA8 File Offset: 0x002C30A8
	[PublicizedFrom(EAccessModifier.Internal)]
	public void SetOn(bool isOn)
	{
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			XUiC_RequiredItemStack xuiC_RequiredItemStack = this.itemControllers[i] as XUiC_RequiredItemStack;
			if (xuiC_RequiredItemStack != null)
			{
				xuiC_RequiredItemStack.ToolLock = isOn;
			}
		}
	}

	// Token: 0x06006C74 RID: 27764 RVA: 0x002C4EE0 File Offset: 0x002C30E0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateBackend(ItemStack[] stackList)
	{
		base.UpdateBackend(stackList);
		this.tileEntity.ItemSlots = stackList;
		this.tileEntity.SetSendSlots();
		this.windowGroup.Controller.SetAllChildrenDirty(false);
	}

	// Token: 0x06006C75 RID: 27765 RVA: 0x002C4F14 File Offset: 0x002C3114
	public override void Update(float _dt)
	{
		if (GameManager.Instance == null && GameManager.Instance.World == null)
		{
			return;
		}
		if (this.tileEntity == null)
		{
			return;
		}
		base.Update(_dt);
		if (this.tileEntity.IsOn)
		{
			this.SetSlots(this.tileEntity.ItemSlots);
		}
		base.RefreshBindings(false);
	}

	// Token: 0x06006C76 RID: 27766 RVA: 0x002C4F70 File Offset: 0x002C3170
	public void Refresh()
	{
		this.SetSlots(this.tileEntity.ItemSlots);
	}

	// Token: 0x06006C77 RID: 27767 RVA: 0x002C4F83 File Offset: 0x002C3183
	[PublicizedFrom(EAccessModifier.Internal)]
	public bool TryAddItemToSlot(ItemClass itemClass, ItemStack itemStack)
	{
		if (itemClass != this.tileEntity.SlotItem)
		{
			return false;
		}
		bool flag = this.tileEntity.TryAddItemToSlot(itemClass, itemStack);
		if (flag)
		{
			this.SetSlots(this.tileEntity.ItemSlots);
		}
		return flag;
	}

	// Token: 0x04005275 RID: 21109
	public static XUiC_PowerSourceSlots Current;

	// Token: 0x04005276 RID: 21110
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityPowerSource tileEntity;
}
