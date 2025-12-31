using System;
using UnityEngine.Scripting;

// Token: 0x02000CD5 RID: 3285
[Preserve]
public class XUiC_ItemPartStack : XUiC_BasePartStack
{
	// Token: 0x17000A5F RID: 2655
	// (get) Token: 0x060065AD RID: 26029 RVA: 0x00294E4E File Offset: 0x0029304E
	// (set) Token: 0x060065AE RID: 26030 RVA: 0x00294E56 File Offset: 0x00293056
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

	// Token: 0x17000A60 RID: 2656
	// (get) Token: 0x060065AF RID: 26031 RVA: 0x00294E6C File Offset: 0x0029306C
	// (set) Token: 0x060065B0 RID: 26032 RVA: 0x00294E74 File Offset: 0x00293074
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

	// Token: 0x060065B1 RID: 26033 RVA: 0x00294E83 File Offset: 0x00293083
	public override void Init()
	{
		base.Init();
		this.lblStackMissing = Localization.Get("lblPartStackMissing", false);
	}

	// Token: 0x060065B2 RID: 26034 RVA: 0x00294E9C File Offset: 0x0029309C
	public override string GetAtlas()
	{
		if (base.ItemStack.IsEmpty())
		{
			return "ItemIconAtlasGreyscale";
		}
		return "ItemIconAtlas";
	}

	// Token: 0x060065B3 RID: 26035 RVA: 0x00294EB8 File Offset: 0x002930B8
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

	// Token: 0x060065B4 RID: 26036 RVA: 0x00294F05 File Offset: 0x00293105
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

	// Token: 0x060065B5 RID: 26037 RVA: 0x00294F3C File Offset: 0x0029313C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool CanSwap(ItemStack stack)
	{
		ItemClassModifier itemClassModifier = stack.itemValue.ItemClass as ItemClassModifier;
		if (itemClassModifier == null)
		{
			return false;
		}
		ItemValue itemValue = base.xui.AssembleItem.CurrentItem.itemValue;
		ItemClass itemClass = itemValue.ItemClass;
		if (itemClass.HasAnyTags(itemClassModifier.DisallowedTags))
		{
			return false;
		}
		if (!itemClass.HasAnyTags(itemClassModifier.InstallableTags))
		{
			return false;
		}
		if (itemClassModifier.HasAnyTags(EntityDrone.cStorageModifierTags))
		{
			return true;
		}
		if (itemClassModifier.ItemTags.Test_AnySet(ItemClassModifier.CosmeticModTypes) && itemValue.CosmeticMods.Length != 0)
		{
			for (int i = 0; i < itemValue.CosmeticMods.Length; i++)
			{
				if (itemValue.CosmeticMods[i] == null || itemValue.CosmeticMods[i].IsEmpty())
				{
					return true;
				}
			}
			return false;
		}
		bool flag = itemClassModifier.InstallableTags.IsEmpty || itemClass.HasAnyTags(itemClassModifier.InstallableTags);
		for (int j = 0; j < itemValue.Modifications.Length; j++)
		{
			if (itemValue.Modifications[j].ItemClass != null && !itemValue.Modifications[j].ItemClass.ItemTags.IsEmpty && itemValue.Modifications[j] != this.itemValue && itemClassModifier.HasAnyTags(itemValue.Modifications[j].ItemClass.ItemTags))
			{
				return false;
			}
		}
		return flag && (this.itemValue == null || this.itemValue.type == 0 || (this.itemValue.ItemClass as ItemClassModifier).Type == ItemClassModifier.ModifierTypes.Attachment);
	}

	// Token: 0x060065B6 RID: 26038 RVA: 0x002950BC File Offset: 0x002932BC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool CanRemove()
	{
		return this.itemClass is ItemClassModifier && (this.itemClass as ItemClassModifier).Type == ItemClassModifier.ModifierTypes.Attachment;
	}

	// Token: 0x060065B7 RID: 26039 RVA: 0x002950E0 File Offset: 0x002932E0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SwapItem()
	{
		ItemClassModifier itemClassModifier = base.xui.dragAndDrop.CurrentStack.itemValue.ItemClass as ItemClassModifier;
		base.SwapItem();
		if (itemClassModifier != null && itemClassModifier.ItemTags.Test_AnySet(ItemClassModifier.CosmeticModTypes))
		{
			base.xui.dragAndDrop.CurrentStack = ItemStack.Empty.Clone();
			base.xui.dragAndDrop.PickUpType = base.StackLocation;
		}
	}

	// Token: 0x04004CD6 RID: 19670
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue itemValue;

	// Token: 0x04004CD7 RID: 19671
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass expectedItemClass;

	// Token: 0x04004CD8 RID: 19672
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblStackMissing;
}
