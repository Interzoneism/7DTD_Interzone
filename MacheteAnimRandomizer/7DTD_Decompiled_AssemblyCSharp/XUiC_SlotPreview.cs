using System;
using UnityEngine.Scripting;

// Token: 0x02000E4A RID: 3658
[Preserve]
public class XUiC_SlotPreview : XUiController
{
	// Token: 0x060072E8 RID: 29416 RVA: 0x002EDE18 File Offset: 0x002EC018
	public override void Init()
	{
		base.Init();
		this.slots = base.GetChildrenById("slot", null);
		XUiController[] childrenByType = this.parent.GetChildrenByType<XUiC_ItemStack>(null);
		this.itemStacks = childrenByType;
	}

	// Token: 0x060072E9 RID: 29417 RVA: 0x002EDE54 File Offset: 0x002EC054
	public override void OnOpen()
	{
		for (int i = 0; i < this.itemStacks.Length; i++)
		{
			this.XUiC_SlotPreview_SlotChangedEvent(i, ((XUiC_ItemStack)this.itemStacks[i]).ItemStack);
			((XUiC_ItemStack)this.itemStacks[i]).SlotChangedEvent += this.XUiC_SlotPreview_SlotChangedEvent;
			((XUiC_ItemStack)this.itemStacks[i]).ToolLockChangedEvent += this.XUiC_SlotPreview_ToolLockChangedEvent;
			XUiC_ItemStack xuiC_ItemStack = (XUiC_ItemStack)this.itemStacks[i];
			this.slots[i].ViewComponent.IsVisible = (((XUiC_ItemStack)this.itemStacks[i]).ItemStack.IsEmpty() && !xuiC_ItemStack.ToolLock);
		}
	}

	// Token: 0x060072EA RID: 29418 RVA: 0x002EDF14 File Offset: 0x002EC114
	[PublicizedFrom(EAccessModifier.Private)]
	public void XUiC_SlotPreview_ToolLockChangedEvent(int slotNumber, ItemStack stack, bool locked)
	{
		this.slots[slotNumber].ViewComponent.IsVisible = (stack.IsEmpty() && !locked);
	}

	// Token: 0x060072EB RID: 29419 RVA: 0x002EDF38 File Offset: 0x002EC138
	public override void OnClose()
	{
		for (int i = 0; i < this.itemStacks.Length; i++)
		{
			((XUiC_ItemStack)this.itemStacks[i]).SlotChangedEvent -= this.XUiC_SlotPreview_SlotChangedEvent;
		}
	}

	// Token: 0x060072EC RID: 29420 RVA: 0x002EDF76 File Offset: 0x002EC176
	[PublicizedFrom(EAccessModifier.Private)]
	public void XUiC_SlotPreview_SlotChangedEvent(int slotNumber, ItemStack stack)
	{
		this.slots[slotNumber].ViewComponent.IsVisible = stack.IsEmpty();
	}

	// Token: 0x0400578B RID: 22411
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController[] slots;

	// Token: 0x0400578C RID: 22412
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController[] itemStacks;
}
