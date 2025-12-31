using System;
using UnityEngine.Scripting;

// Token: 0x02000EAE RID: 3758
[Preserve]
public class XUiC_WorkstationGrid : XUiC_ItemStackGrid
{
	// Token: 0x17000C18 RID: 3096
	// (get) Token: 0x060076E5 RID: 30437 RVA: 0x00306DEC File Offset: 0x00304FEC
	public XUiM_Workstation WorkstationData
	{
		get
		{
			return this.workstationData;
		}
	}

	// Token: 0x17000C19 RID: 3097
	// (get) Token: 0x060076E6 RID: 30438 RVA: 0x00076E19 File Offset: 0x00075019
	public override XUiC_ItemStack.StackLocationTypes StackLocation
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return XUiC_ItemStack.StackLocationTypes.Workstation;
		}
	}

	// Token: 0x060076E7 RID: 30439 RVA: 0x00306DF4 File Offset: 0x00304FF4
	public virtual void SetSlots(ItemStack[] stacks)
	{
		base.SetStacks(stacks);
	}

	// Token: 0x060076E8 RID: 30440 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool HasRequirement(Recipe recipe)
	{
		return true;
	}

	// Token: 0x060076E9 RID: 30441 RVA: 0x00306E00 File Offset: 0x00305000
	public override void OnOpen()
	{
		if (base.ViewComponent != null && !base.ViewComponent.IsVisible)
		{
			base.ViewComponent.OnOpen();
			base.ViewComponent.IsVisible = true;
		}
		this.workstationData = ((XUiC_WorkstationWindowGroup)this.windowGroup.Controller).WorkstationData;
		this.IsDirty = true;
		this.IsDormant = false;
	}

	// Token: 0x060076EA RID: 30442 RVA: 0x00306E62 File Offset: 0x00305062
	public override void OnClose()
	{
		if (base.ViewComponent != null && base.ViewComponent.IsVisible)
		{
			base.ViewComponent.OnClose();
			base.ViewComponent.IsVisible = false;
		}
		this.IsDirty = true;
		this.IsDormant = true;
	}

	// Token: 0x060076EB RID: 30443 RVA: 0x00306EA0 File Offset: 0x003050A0
	public int AddToItemStackArray(ItemStack _itemStack)
	{
		ItemStack[] slots = this.GetSlots();
		int num = -1;
		int num2 = 0;
		while (num == -1 && num2 < slots.Length)
		{
			if (slots[num2].CanStackWith(_itemStack, false))
			{
				slots[num2].count += _itemStack.count;
				_itemStack.count = 0;
				num = num2;
			}
			num2++;
		}
		int num3 = 0;
		while (num == -1 && num3 < slots.Length)
		{
			if (slots[num3].IsEmpty())
			{
				slots[num3] = _itemStack;
				num = num3;
			}
			num3++;
		}
		if (num != -1)
		{
			this.SetSlots(slots);
			this.UpdateBackend(slots);
		}
		return num;
	}

	// Token: 0x04005AA6 RID: 23206
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiM_Workstation workstationData;
}
