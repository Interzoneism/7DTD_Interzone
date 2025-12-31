using System;
using UnityEngine.Scripting;

// Token: 0x02000CD6 RID: 3286
[Preserve]
public class XUiC_ItemCosmeticStack : XUiC_BasePartStack
{
	// Token: 0x17000A61 RID: 2657
	// (get) Token: 0x060065B9 RID: 26041 RVA: 0x00295160 File Offset: 0x00293360
	// (set) Token: 0x060065BA RID: 26042 RVA: 0x00295168 File Offset: 0x00293368
	public ItemValue ItemValue
	{
		get
		{
			return this.itemValue;
		}
		set
		{
			this.itemValue = value;
			base.ItemStack = new ItemStack(value, 1);
		}
	}

	// Token: 0x17000A62 RID: 2658
	// (get) Token: 0x060065BB RID: 26043 RVA: 0x0029517E File Offset: 0x0029337E
	// (set) Token: 0x060065BC RID: 26044 RVA: 0x00295186 File Offset: 0x00293386
	public ItemClass ExpectedItemClass
	{
		get
		{
			return this.expectedItemClass;
		}
		set
		{
			this.expectedItemClass = value;
			this.SetEmptySpriteName();
		}
	}

	// Token: 0x060065BD RID: 26045 RVA: 0x00295195 File Offset: 0x00293395
	public override void Init()
	{
		base.Init();
		this.lblStackMissing = Localization.Get("lblPartStackMissing", false);
	}

	// Token: 0x060065BE RID: 26046 RVA: 0x00294E9C File Offset: 0x0029309C
	public override string GetAtlas()
	{
		if (base.ItemStack.IsEmpty())
		{
			return "ItemIconAtlasGreyscale";
		}
		return "ItemIconAtlas";
	}

	// Token: 0x060065BF RID: 26047 RVA: 0x002951B0 File Offset: 0x002933B0
	public override string GetPartName()
	{
		if (this.itemClass == null && this.expectedItemClass == null)
		{
			return "";
		}
		if (this.itemClass == null)
		{
			return string.Format(this.lblStackMissing, this.expectedItemClass.GetLocalizedItemName());
		}
		return this.itemClass.GetLocalizedItemName();
	}

	// Token: 0x060065C0 RID: 26048 RVA: 0x002951FD File Offset: 0x002933FD
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SetEmptySpriteName()
	{
		if (this.expectedItemClass != null && this.expectedItemClass.Id != 0)
		{
			this.emptySpriteName = this.expectedItemClass.GetIconName();
			return;
		}
		this.emptySpriteName = "";
	}

	// Token: 0x060065C1 RID: 26049 RVA: 0x00295234 File Offset: 0x00293434
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool CanSwap(ItemStack stack)
	{
		ItemClassModifier itemClassModifier = stack.itemValue.ItemClass as ItemClassModifier;
		if (itemClassModifier == null)
		{
			return false;
		}
		if (base.xui.AssembleItem.CurrentItem.itemValue.ItemClass.HasAnyTags(itemClassModifier.DisallowedTags))
		{
			return false;
		}
		if (!base.xui.AssembleItem.CurrentItem.itemValue.ItemClass.HasAnyTags(itemClassModifier.InstallableTags))
		{
			return false;
		}
		if (itemClassModifier != null && !itemClassModifier.ItemTags.Test_AnySet(ItemClassModifier.CosmeticModTypes) && base.xui.AssembleItem.CurrentItem.itemValue.Modifications.Length != 0)
		{
			bool result = false;
			for (int i = 0; i < base.xui.AssembleItem.CurrentItem.itemValue.Modifications.Length; i++)
			{
				if (base.xui.AssembleItem.CurrentItem.itemValue.Modifications[i] == null || base.xui.AssembleItem.CurrentItem.itemValue.Modifications[i].IsEmpty())
				{
					result = true;
				}
				else if (itemClassModifier.HasAnyTags(base.xui.AssembleItem.CurrentItem.itemValue.Modifications[i].ItemClass.ItemTags))
				{
					result = false;
					break;
				}
			}
			return result;
		}
		bool flag = itemClassModifier.InstallableTags.IsEmpty || base.xui.AssembleItem.CurrentItem.itemValue.ItemClass.HasAnyTags(itemClassModifier.InstallableTags);
		for (int j = 0; j < base.xui.AssembleItem.CurrentItem.itemValue.CosmeticMods.Length; j++)
		{
			if (base.xui.AssembleItem.CurrentItem.itemValue.CosmeticMods[j] != null && base.xui.AssembleItem.CurrentItem.itemValue.CosmeticMods[j].ItemClass != null && !base.xui.AssembleItem.CurrentItem.itemValue.CosmeticMods[j].ItemClass.ItemTags.IsEmpty && base.xui.AssembleItem.CurrentItem.itemValue.CosmeticMods[j] != this.itemValue && itemClassModifier.HasAnyTags(base.xui.AssembleItem.CurrentItem.itemValue.CosmeticMods[j].ItemClass.ItemTags))
			{
				return false;
			}
		}
		return flag && (this.itemValue == null || this.itemValue.type == 0 || (this.itemValue.ItemClass as ItemClassModifier).Type == ItemClassModifier.ModifierTypes.Attachment);
	}

	// Token: 0x060065C2 RID: 26050 RVA: 0x002950BC File Offset: 0x002932BC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool CanRemove()
	{
		return this.itemClass is ItemClassModifier && (this.itemClass as ItemClassModifier).Type == ItemClassModifier.ModifierTypes.Attachment;
	}

	// Token: 0x060065C3 RID: 26051 RVA: 0x002954F4 File Offset: 0x002936F4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SwapItem()
	{
		ItemClassModifier itemClassModifier = base.xui.dragAndDrop.CurrentStack.itemValue.ItemClass as ItemClassModifier;
		base.SwapItem();
		if (itemClassModifier != null && !itemClassModifier.ItemTags.Test_AnySet(ItemClassModifier.CosmeticModTypes) && itemClassModifier != null && !itemClassModifier.ItemTags.Test_AnySet(ItemClassModifier.CosmeticModTypes) && base.xui.AssembleItem.CurrentItem.itemValue.Modifications.Length != 0)
		{
			bool flag = false;
			for (int i = 0; i < base.xui.AssembleItem.CurrentItem.itemValue.Modifications.Length; i++)
			{
				if (itemClassModifier.HasAnyTags(base.xui.AssembleItem.CurrentItem.itemValue.Modifications[i].ItemClass.ItemTags))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				base.xui.dragAndDrop.CurrentStack = ItemStack.Empty.Clone();
				base.xui.dragAndDrop.PickUpType = base.StackLocation;
			}
		}
	}

	// Token: 0x04004CD9 RID: 19673
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue itemValue;

	// Token: 0x04004CDA RID: 19674
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass expectedItemClass;

	// Token: 0x04004CDB RID: 19675
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblStackMissing;
}
