using System;

// Token: 0x02000EDD RID: 3805
public class XUiM_AssembleItem : XUiModel
{
	// Token: 0x17000C28 RID: 3112
	// (get) Token: 0x06007807 RID: 30727 RVA: 0x0030DDC1 File Offset: 0x0030BFC1
	// (set) Token: 0x06007808 RID: 30728 RVA: 0x0030DDC9 File Offset: 0x0030BFC9
	public ItemStack CurrentItem
	{
		get
		{
			return this.currentItem;
		}
		set
		{
			this.currentItem = value;
			if (value != null)
			{
				this.SetPartCount();
			}
		}
	}

	// Token: 0x17000C29 RID: 3113
	// (get) Token: 0x06007809 RID: 30729 RVA: 0x0030DDDB File Offset: 0x0030BFDB
	// (set) Token: 0x0600780A RID: 30730 RVA: 0x0030DDE3 File Offset: 0x0030BFE3
	public XUiC_ItemStack CurrentItemStackController
	{
		get
		{
			return this.currentItemStackController;
		}
		set
		{
			if (this.currentItemStackController != null)
			{
				this.currentItemStackController.AssembleLock = false;
			}
			this.currentItemStackController = value;
			if (this.currentItemStackController != null)
			{
				this.currentItemStackController.AssembleLock = true;
			}
		}
	}

	// Token: 0x17000C2A RID: 3114
	// (get) Token: 0x0600780B RID: 30731 RVA: 0x0030DE14 File Offset: 0x0030C014
	// (set) Token: 0x0600780C RID: 30732 RVA: 0x0030DE1C File Offset: 0x0030C01C
	public XUiC_EquipmentStack CurrentEquipmentStackController
	{
		get
		{
			return this.currentEquipmentStackController;
		}
		set
		{
			this.currentEquipmentStackController = value;
		}
	}

	// Token: 0x17000C2B RID: 3115
	// (get) Token: 0x0600780D RID: 30733 RVA: 0x0030DE25 File Offset: 0x0030C025
	// (set) Token: 0x0600780E RID: 30734 RVA: 0x0030DE2D File Offset: 0x0030C02D
	public int PartCount { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x0600780F RID: 30735 RVA: 0x0030DE38 File Offset: 0x0030C038
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetPartCount()
	{
		this.PartCount = 0;
		for (int i = 0; i < this.CurrentItem.itemValue.Modifications.Length; i++)
		{
			if (this.CurrentItem.itemValue.Modifications[i] == null)
			{
				this.CurrentItem.itemValue.Modifications[i] = ItemValue.None.Clone();
			}
			if (!this.CurrentItem.itemValue.Modifications[i].IsEmpty())
			{
				int partCount = this.PartCount;
				this.PartCount = partCount + 1;
			}
		}
	}

	// Token: 0x06007810 RID: 30736 RVA: 0x0030DEC4 File Offset: 0x0030C0C4
	public void RefreshAssembleItem()
	{
		this.PartCount = 0;
		ItemValue.None.Clone();
		bool flag = false;
		for (int i = 0; i < this.CurrentItem.itemValue.Modifications.Length; i++)
		{
			if (!this.CurrentItem.itemValue.Modifications[i].IsEmpty())
			{
				int partCount = this.PartCount;
				this.PartCount = partCount + 1;
				ItemValue itemValue = this.CurrentItem.itemValue.Modifications[i];
			}
			else
			{
				flag = true;
			}
		}
		if (this.CurrentItemStackController != null)
		{
			this.CurrentItemStackController.ForceSetItemStack(this.CurrentItem);
			this.CurrentItemStackController.AssembleLock = true;
		}
		if (this.currentEquipmentStackController != null)
		{
			this.currentEquipmentStackController.ItemStack = this.CurrentItem;
		}
		if (flag)
		{
			QuestEventManager.Current.AssembledItem(this.CurrentItem);
		}
	}

	// Token: 0x06007811 RID: 30737 RVA: 0x0030DF94 File Offset: 0x0030C194
	public bool AddPartToItem(ItemStack partStack, out ItemStack resultStack)
	{
		if (this.CurrentItem == null || this.CurrentItem.IsEmpty())
		{
			resultStack = partStack;
			return false;
		}
		ItemClassModifier itemClassModifier = partStack.itemValue.ItemClass as ItemClassModifier;
		if (itemClassModifier != null)
		{
			if (this.CurrentItem.itemValue.ItemClass.HasAnyTags(itemClassModifier.DisallowedTags))
			{
				resultStack = partStack;
				return false;
			}
			if (itemClassModifier.HasAnyTags(ItemClassModifier.CosmeticModTypes))
			{
				for (int i = 0; i < this.CurrentItem.itemValue.CosmeticMods.Length; i++)
				{
					if (this.CurrentItem.itemValue.CosmeticMods[i] != null && this.CurrentItem.itemValue.CosmeticMods[i].ItemClass != null && itemClassModifier.HasAnyTags(this.CurrentItem.itemValue.CosmeticMods[i].ItemClass.ItemTags))
					{
						resultStack = partStack;
						return false;
					}
				}
			}
			else
			{
				for (int j = 0; j < this.CurrentItem.itemValue.Modifications.Length; j++)
				{
					if (this.CurrentItem.itemValue.Modifications[j] != null && this.CurrentItem.itemValue.Modifications[j].ItemClass != null && !itemClassModifier.HasAnyTags(EntityDrone.cStorageModifierTags) && itemClassModifier.HasAnyTags(this.CurrentItem.itemValue.Modifications[j].ItemClass.ItemTags))
					{
						resultStack = partStack;
						return false;
					}
				}
			}
		}
		if (this.CurrentItem.itemValue.ItemClass.HasAnyTags(itemClassModifier.InstallableTags))
		{
			if (itemClassModifier.HasAnyTags(ItemClassModifier.CosmeticModTypes))
			{
				if (this.CurrentItem.itemValue.CosmeticMods != null)
				{
					for (int k = 0; k < this.CurrentItem.itemValue.CosmeticMods.Length; k++)
					{
						if (this.CurrentItem.itemValue.CosmeticMods[k] == null || this.CurrentItem.itemValue.CosmeticMods[k].IsEmpty())
						{
							float num = 1f - this.CurrentItem.itemValue.PercentUsesLeft;
							this.CurrentItem.itemValue.CosmeticMods[k] = partStack.itemValue.Clone();
							if (this.CurrentItemStackController != null)
							{
								XUiC_AssembleWindowGroup.GetWindowGroup(this.CurrentItemStackController.xui).ItemStack = this.CurrentItem;
							}
							if (this.currentEquipmentStackController != null)
							{
								XUiC_AssembleWindowGroup.GetWindowGroup(this.CurrentEquipmentStackController.xui).ItemStack = this.CurrentItem;
							}
							this.RefreshAssembleItem();
							if (this.CurrentItem.itemValue.MaxUseTimes > 0)
							{
								this.CurrentItem.itemValue.UseTimes = (float)((int)(num * (float)this.CurrentItem.itemValue.MaxUseTimes));
							}
							this.UpdateAssembleWindow();
							resultStack = ItemStack.Empty.Clone();
							return true;
						}
					}
				}
			}
			else if (this.CurrentItem.itemValue.Modifications != null)
			{
				for (int l = 0; l < this.CurrentItem.itemValue.Modifications.Length; l++)
				{
					if (this.CurrentItem.itemValue.Modifications[l] == null || this.CurrentItem.itemValue.Modifications[l].IsEmpty())
					{
						float num2 = 1f - this.CurrentItem.itemValue.PercentUsesLeft;
						this.CurrentItem.itemValue.Modifications[l] = partStack.itemValue.Clone();
						if (this.CurrentItemStackController != null)
						{
							XUiC_AssembleWindowGroup.GetWindowGroup(this.CurrentItemStackController.xui).ItemStack = this.CurrentItem;
						}
						if (this.currentEquipmentStackController != null)
						{
							XUiC_AssembleWindowGroup.GetWindowGroup(this.CurrentEquipmentStackController.xui).ItemStack = this.CurrentItem;
						}
						this.RefreshAssembleItem();
						if (this.CurrentItem.itemValue.MaxUseTimes > 0)
						{
							this.CurrentItem.itemValue.UseTimes = (float)((int)(num2 * (float)this.CurrentItem.itemValue.MaxUseTimes));
						}
						this.UpdateAssembleWindow();
						resultStack = ItemStack.Empty.Clone();
						return true;
					}
				}
			}
		}
		resultStack = partStack;
		return false;
	}

	// Token: 0x06007812 RID: 30738 RVA: 0x0030E3A5 File Offset: 0x0030C5A5
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateAssembleWindow()
	{
		if (this.AssembleWindow != null)
		{
			this.AssembleWindow.ItemStack = this.CurrentItem;
			this.AssembleWindow.OnChanged();
		}
	}

	// Token: 0x04005B8A RID: 23434
	public XUiC_AssembleWindow AssembleWindow;

	// Token: 0x04005B8B RID: 23435
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack currentItem;

	// Token: 0x04005B8C RID: 23436
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemStack currentItemStackController;

	// Token: 0x04005B8D RID: 23437
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_EquipmentStack currentEquipmentStackController;
}
