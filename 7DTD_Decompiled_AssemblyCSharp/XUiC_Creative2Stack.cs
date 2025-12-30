using System;
using UnityEngine.Scripting;

// Token: 0x02000C6D RID: 3181
[Preserve]
public class XUiC_Creative2Stack : XUiC_ItemStack
{
	// Token: 0x06006209 RID: 25097 RVA: 0x0027C9B2 File Offset: 0x0027ABB2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void setItemStack(ItemStack _stack)
	{
		this.itemStack = _stack;
	}

	// Token: 0x0600620A RID: 25098 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void HandleDropOne()
	{
	}

	// Token: 0x0600620B RID: 25099 RVA: 0x0027C9BB File Offset: 0x0027ABBB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void HandleClickComplete()
	{
		this.lastClicked = false;
	}

	// Token: 0x0600620C RID: 25100 RVA: 0x0027C9C4 File Offset: 0x0027ABC4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SwapItem()
	{
		base.xui.dragAndDrop.CurrentStack = this.itemStack.Clone();
		base.xui.dragAndDrop.PickUpType = base.StackLocation;
		base.PlayPickupSound(null);
		base.HandleSlotChangeEvent();
	}
}
