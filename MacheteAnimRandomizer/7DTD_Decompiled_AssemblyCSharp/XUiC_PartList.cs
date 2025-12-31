using System;
using UnityEngine.Scripting;

// Token: 0x02000D69 RID: 3433
[Preserve]
public class XUiC_PartList : XUiC_ItemStackGrid
{
	// Token: 0x06006B49 RID: 27465 RVA: 0x002BD914 File Offset: 0x002BBB14
	public override void Init()
	{
		base.Init();
		foreach (XUiC_ItemStack xuiC_ItemStack in this.itemControllers)
		{
			xuiC_ItemStack.ViewComponent.IsNavigatable = false;
			xuiC_ItemStack.StackLocation = XUiC_ItemStack.StackLocationTypes.Part;
		}
	}

	// Token: 0x06006B4A RID: 27466 RVA: 0x00019766 File Offset: 0x00017966
	public override ItemStack[] GetSlots()
	{
		return null;
	}

	// Token: 0x06006B4B RID: 27467 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateBackend(ItemStack[] stackList)
	{
	}

	// Token: 0x06006B4C RID: 27468 RVA: 0x002BD954 File Offset: 0x002BBB54
	public void SetSlot(ItemValue part, int index)
	{
		XUiC_ItemStack xuiC_ItemStack = this.itemControllers[index];
		if (part != null && !part.IsEmpty())
		{
			ItemStack itemStack = new ItemStack(part.Clone(), 1);
			xuiC_ItemStack.ItemStack = itemStack;
			xuiC_ItemStack.GreyedOut = false;
		}
		else
		{
			xuiC_ItemStack.ItemStack = ItemStack.Empty.Clone();
			xuiC_ItemStack.GreyedOut = false;
		}
		this.itemControllers[index].ViewComponent.EventOnPress = false;
		this.itemControllers[index].ViewComponent.EventOnHover = false;
	}

	// Token: 0x06006B4D RID: 27469 RVA: 0x002BD9D0 File Offset: 0x002BBBD0
	public void SetSlots(ItemValue[] parts, int startIndex = 0)
	{
		for (int i = 0; i < this.itemControllers.Length - startIndex; i++)
		{
			int num = i + startIndex;
			XUiC_ItemStack xuiC_ItemStack = this.itemControllers[num];
			if (parts.Length > i && parts[i] != null && !parts[i].IsEmpty())
			{
				ItemStack itemStack = new ItemStack(parts[i].Clone(), 1);
				xuiC_ItemStack.ItemStack = itemStack;
				xuiC_ItemStack.GreyedOut = false;
			}
			else
			{
				xuiC_ItemStack.ItemStack = ItemStack.Empty.Clone();
				xuiC_ItemStack.GreyedOut = false;
			}
			xuiC_ItemStack.ViewComponent.EventOnPress = false;
			xuiC_ItemStack.ViewComponent.EventOnHover = false;
		}
	}

	// Token: 0x06006B4E RID: 27470 RVA: 0x002BDA68 File Offset: 0x002BBC68
	public void SetAmmoSlot(ItemValue ammo, int count)
	{
		int num = 5;
		int count2 = (count < 1) ? 1 : count;
		if (ammo != null && !ammo.IsEmpty())
		{
			ItemStack itemStack = new ItemStack(ammo, count2);
			this.itemControllers[num].ItemStack = itemStack;
			this.itemControllers[num].GreyedOut = (count == 0);
			return;
		}
		this.itemControllers[num].ItemStack = ItemStack.Empty.Clone();
		this.itemControllers[num].GreyedOut = false;
	}

	// Token: 0x06006B4F RID: 27471 RVA: 0x002BDAD8 File Offset: 0x002BBCD8
	[PublicizedFrom(EAccessModifier.Internal)]
	public void SetMainItem(ItemStack itemStack)
	{
		this.itemClass = itemStack.itemValue.ItemClass;
	}

	// Token: 0x0400517B RID: 20859
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass itemClass;
}
