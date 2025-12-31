using System;
using UnityEngine.Scripting;

// Token: 0x02000DE5 RID: 3557
[Preserve]
public class XUiC_RequiredItemStack : XUiC_ItemStack
{
	// Token: 0x140000BC RID: 188
	// (add) Token: 0x06006F95 RID: 28565 RVA: 0x002D8C18 File Offset: 0x002D6E18
	// (remove) Token: 0x06006F96 RID: 28566 RVA: 0x002D8C50 File Offset: 0x002D6E50
	public event XUiEvent_RequiredSlotFailedSwapEventHandler FailedSwap;

	// Token: 0x17000B3B RID: 2875
	// (get) Token: 0x06006F97 RID: 28567 RVA: 0x002D8C85 File Offset: 0x002D6E85
	// (set) Token: 0x06006F98 RID: 28568 RVA: 0x002D8C8D File Offset: 0x002D6E8D
	public ItemClass RequiredItemClass
	{
		get
		{
			return this.requiredItemClass;
		}
		set
		{
			this.requiredItemClass = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000B3C RID: 2876
	// (get) Token: 0x06006F99 RID: 28569 RVA: 0x002D8C9D File Offset: 0x002D6E9D
	public override string ItemIcon
	{
		get
		{
			if (this.RequiredItemClass != null && this.itemStack.IsEmpty())
			{
				return this.RequiredItemClass.GetIconName();
			}
			return base.ItemIcon;
		}
	}

	// Token: 0x17000B3D RID: 2877
	// (get) Token: 0x06006F9A RID: 28570 RVA: 0x002D8CC8 File Offset: 0x002D6EC8
	public override string ItemIconColor
	{
		get
		{
			if (base.itemClass != null)
			{
				base.GreyedOut = false;
				return base.ItemIconColor;
			}
			if (this.requiredItemClass != null && !base.StackLock)
			{
				base.GreyedOut = true;
				return "255,255,255,255";
			}
			base.GreyedOut = false;
			return "255,255,255,0";
		}
	}

	// Token: 0x06006F9B RID: 28571 RVA: 0x002D8D14 File Offset: 0x002D6F14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool CanSwap(ItemStack stack)
	{
		if (this.TakeOnly && !stack.IsEmpty())
		{
			return false;
		}
		bool flag;
		if (this.RequiredType == XUiC_RequiredItemStack.RequiredTypes.ItemClass && this.RequiredItemClass != null && this.RequiredItemOnly)
		{
			flag = (stack.itemValue.ItemClass == this.RequiredItemClass);
		}
		else if (this.RequiredType == XUiC_RequiredItemStack.RequiredTypes.IsPart)
		{
			flag = (stack.itemValue.ItemClass.PartParentId != null);
		}
		else if (this.RequiredType == XUiC_RequiredItemStack.RequiredTypes.HasQuality)
		{
			flag = stack.itemValue.HasQuality;
		}
		else
		{
			flag = (this.RequiredType != XUiC_RequiredItemStack.RequiredTypes.HasQualityNoParts || (stack.itemValue.HasQuality && !stack.itemValue.ItemClass.HasSubItems));
		}
		if (!flag && this.FailedSwap != null)
		{
			this.FailedSwap(stack);
		}
		return flag;
	}

	// Token: 0x06006F9C RID: 28572 RVA: 0x002D8DE4 File Offset: 0x002D6FE4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void HandleDropOne()
	{
		ItemStack currentStack = base.xui.dragAndDrop.CurrentStack;
		if (!currentStack.IsEmpty())
		{
			int num = 1;
			if (this.itemStack.IsEmpty() && this.CanSwap(currentStack))
			{
				ItemStack itemStack = currentStack.Clone();
				itemStack.count = num;
				currentStack.count -= num;
				base.xui.dragAndDrop.CurrentStack = currentStack;
				base.xui.dragAndDrop.PickUpType = base.StackLocation;
				base.ItemStack = itemStack;
				base.PlayPlaceSound(null);
			}
		}
	}

	// Token: 0x040054AD RID: 21677
	public XUiC_RequiredItemStack.RequiredTypes RequiredType;

	// Token: 0x040054AF RID: 21679
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass requiredItemClass;

	// Token: 0x040054B0 RID: 21680
	public bool RequiredItemOnly = true;

	// Token: 0x040054B1 RID: 21681
	public bool TakeOnly;

	// Token: 0x02000DE6 RID: 3558
	public enum RequiredTypes
	{
		// Token: 0x040054B3 RID: 21683
		ItemClass,
		// Token: 0x040054B4 RID: 21684
		IsPart,
		// Token: 0x040054B5 RID: 21685
		HasQuality,
		// Token: 0x040054B6 RID: 21686
		HasQualityNoParts
	}
}
