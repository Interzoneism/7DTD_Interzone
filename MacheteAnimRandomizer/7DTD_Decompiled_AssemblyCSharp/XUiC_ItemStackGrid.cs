using System;
using UnityEngine.Scripting;

// Token: 0x02000CE3 RID: 3299
[Preserve]
public class XUiC_ItemStackGrid : XUiController
{
	// Token: 0x17000A89 RID: 2697
	// (get) Token: 0x06006654 RID: 26196 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual XUiC_ItemStack.StackLocationTypes StackLocation
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return XUiC_ItemStack.StackLocationTypes.Backpack;
		}
	}

	// Token: 0x06006655 RID: 26197 RVA: 0x0029900D File Offset: 0x0029720D
	public override void Init()
	{
		base.Init();
		this.itemControllers = base.GetChildrenByType<XUiC_ItemStack>(null);
		this.bAwakeCalled = true;
		this.IsDirty = false;
		this.IsDormant = true;
		this.handleSlotChangedDelegate = new XUiEvent_SlotChangedEventHandler(this.HandleSlotChangedEvent);
	}

	// Token: 0x06006656 RID: 26198 RVA: 0x0029904A File Offset: 0x0029724A
	public XUiC_ItemStack[] GetItemStackControllers()
	{
		return this.itemControllers;
	}

	// Token: 0x06006657 RID: 26199 RVA: 0x00299052 File Offset: 0x00297252
	public virtual ItemStack[] GetSlots()
	{
		return this.getUISlots();
	}

	// Token: 0x06006658 RID: 26200 RVA: 0x0029905C File Offset: 0x0029725C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual ItemStack[] getUISlots()
	{
		ItemStack[] array = new ItemStack[this.itemControllers.Length];
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			array[i] = this.itemControllers[i].ItemStack.Clone();
		}
		return array;
	}

	// Token: 0x06006659 RID: 26201 RVA: 0x002990A0 File Offset: 0x002972A0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SetStacks(ItemStack[] stackList)
	{
		if (stackList == null)
		{
			return;
		}
		XUiC_ItemInfoWindow childByType = base.xui.GetChildByType<XUiC_ItemInfoWindow>();
		int num = 0;
		while (num < stackList.Length && this.itemControllers.Length > num && stackList.Length > num)
		{
			XUiC_ItemStack xuiC_ItemStack = this.itemControllers[num];
			xuiC_ItemStack.SlotChangedEvent -= this.handleSlotChangedDelegate;
			xuiC_ItemStack.ItemStack = stackList[num].Clone();
			xuiC_ItemStack.SlotChangedEvent += this.handleSlotChangedDelegate;
			xuiC_ItemStack.SlotNumber = num;
			xuiC_ItemStack.InfoWindow = childByType;
			xuiC_ItemStack.StackLocation = this.StackLocation;
			num++;
		}
	}

	// Token: 0x0600665A RID: 26202 RVA: 0x00299124 File Offset: 0x00297324
	public void AssembleLockSingleStack(ItemStack stack)
	{
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			XUiC_ItemStack xuiC_ItemStack = this.itemControllers[i];
			if (xuiC_ItemStack.ItemStack.itemValue.Equals(stack.itemValue))
			{
				base.xui.AssembleItem.CurrentItemStackController = xuiC_ItemStack;
				return;
			}
		}
	}

	// Token: 0x0600665B RID: 26203 RVA: 0x00299177 File Offset: 0x00297377
	public virtual void HandleSlotChangedEvent(int slotNumber, ItemStack stack)
	{
		if (this.items != null)
		{
			this.items[slotNumber] = stack.Clone();
		}
		this.UpdateBackend(this.getUISlots());
	}

	// Token: 0x0600665C RID: 26204 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateBackend(ItemStack[] stackList)
	{
	}

	// Token: 0x0600665D RID: 26205 RVA: 0x0029919C File Offset: 0x0029739C
	public virtual void ClearHoveredItems()
	{
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			this.itemControllers[i].Hovered(false);
		}
	}

	// Token: 0x0600665E RID: 26206 RVA: 0x002991CC File Offset: 0x002973CC
	public int FindFirstEmptySlot()
	{
		int result = -1;
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			if (this.itemControllers[i].ViewComponent.UiTransform.gameObject.activeInHierarchy && this.itemControllers[i].ItemStack.Equals(ItemStack.Empty))
			{
				result = i;
				break;
			}
		}
		return result;
	}

	// Token: 0x0600665F RID: 26207 RVA: 0x0029922A File Offset: 0x0029742A
	public override void OnOpen()
	{
		base.OnOpen();
		this.IsDirty = true;
		this.IsDormant = false;
		base.xui.playerUI.RegisterItemStackGrid(this);
	}

	// Token: 0x06006660 RID: 26208 RVA: 0x00299251 File Offset: 0x00297451
	public override void OnClose()
	{
		base.OnClose();
		this.ClearHoveredItems();
		this.IsDormant = true;
		base.xui.playerUI.UnregisterItemStackGrid(this);
	}

	// Token: 0x04004D41 RID: 19777
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiC_ItemStack[] itemControllers;

	// Token: 0x04004D42 RID: 19778
	[PublicizedFrom(EAccessModifier.Protected)]
	public ItemStack[] items;

	// Token: 0x04004D43 RID: 19779
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiEvent_SlotChangedEventHandler handleSlotChangedDelegate;

	// Token: 0x04004D44 RID: 19780
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bAwakeCalled;
}
