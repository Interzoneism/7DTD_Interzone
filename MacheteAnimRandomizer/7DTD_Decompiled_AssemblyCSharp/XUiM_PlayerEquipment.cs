using System;

// Token: 0x02000EE9 RID: 3817
public class XUiM_PlayerEquipment : XUiModel
{
	// Token: 0x140000D8 RID: 216
	// (add) Token: 0x06007871 RID: 30833 RVA: 0x00310FD4 File Offset: 0x0030F1D4
	// (remove) Token: 0x06007872 RID: 30834 RVA: 0x00311008 File Offset: 0x0030F208
	public static event XUiEvent_RefreshEquipment HandleRefreshEquipment;

	// Token: 0x17000C2C RID: 3116
	// (get) Token: 0x06007873 RID: 30835 RVA: 0x0031103B File Offset: 0x0030F23B
	public Equipment Equipment
	{
		get
		{
			return this.equipment;
		}
	}

	// Token: 0x06007875 RID: 30837 RVA: 0x00311043 File Offset: 0x0030F243
	public XUiM_PlayerEquipment(XUi _xui, EntityPlayerLocal _player)
	{
		if (!_player)
		{
			return;
		}
		this.xui = _xui;
		this.equipment = _player.equipment;
	}

	// Token: 0x06007876 RID: 30838 RVA: 0x00311068 File Offset: 0x0030F268
	public ItemStack EquipItem(ItemStack stack)
	{
		ItemClassArmor itemClassArmor = stack.itemValue.ItemClass as ItemClassArmor;
		if (itemClassArmor == null)
		{
			this.RefreshEquipment();
			return stack;
		}
		EquipmentSlots equipSlot = itemClassArmor.EquipSlot;
		ItemStack stackFromSlot = this.GetStackFromSlot(equipSlot);
		bool flag = true;
		if (!stackFromSlot.IsEmpty())
		{
			ItemClassArmor itemClassArmor2 = stackFromSlot.itemValue.ItemClass as ItemClassArmor;
			if (itemClassArmor2 != null)
			{
				if (itemClassArmor2.ReplaceByTag != null && !FastTags<TagGroup.Global>.Parse(itemClassArmor2.ReplaceByTag).Test_AnySet(itemClassArmor.ItemTags))
				{
					return stack;
				}
				flag = itemClassArmor2.AllowUnEquip;
				this.equipment.SetSlotItem((int)equipSlot, stack.itemValue.Clone(), true);
			}
		}
		this.equipment.SetSlotItem((int)equipSlot, stack.itemValue.Clone(), true);
		QuestEventManager.Current.WoreItem(stack.itemValue);
		this.RefreshEquipment();
		if (!flag)
		{
			return ItemStack.Empty.Clone();
		}
		return stackFromSlot;
	}

	// Token: 0x06007877 RID: 30839 RVA: 0x00311148 File Offset: 0x0030F348
	[PublicizedFrom(EAccessModifier.Internal)]
	public bool IsWearing(ItemValue itemValue)
	{
		ItemClassArmor itemClassArmor = itemValue.ItemClass as ItemClassArmor;
		if (itemClassArmor != null)
		{
			EquipmentSlots equipSlot = itemClassArmor.EquipSlot;
			return equipSlot != EquipmentSlots.Count && this.GetStackFromSlot(equipSlot).itemValue.type == itemValue.type;
		}
		return false;
	}

	// Token: 0x06007878 RID: 30840 RVA: 0x0031118C File Offset: 0x0030F38C
	public bool IsEquipmentTypeWorn(EquipmentSlots slot)
	{
		return slot != EquipmentSlots.Count && !this.GetStackFromSlot(slot).itemValue.IsEmpty();
	}

	// Token: 0x06007879 RID: 30841 RVA: 0x003111A8 File Offset: 0x0030F3A8
	public void RefreshEquipment()
	{
		this.equipment.FireEventsForSetSlots();
		if (XUiM_PlayerEquipment.HandleRefreshEquipment != null)
		{
			XUiM_PlayerEquipment.HandleRefreshEquipment(this);
		}
		this.equipment.FireEventsForChangedSlots();
	}

	// Token: 0x0600787A RID: 30842 RVA: 0x003111D4 File Offset: 0x0030F3D4
	public ItemStack GetStackFromSlot(EquipmentSlots slot)
	{
		ItemStack itemStack = ItemStack.Empty.Clone();
		ItemValue slotItem = this.Equipment.GetSlotItem((int)slot);
		if (slotItem != null)
		{
			itemStack.itemValue = slotItem;
			itemStack.count = 1;
			return itemStack;
		}
		return itemStack;
	}

	// Token: 0x04005BBB RID: 23483
	public bool IsOpen;

	// Token: 0x04005BBD RID: 23485
	[PublicizedFrom(EAccessModifier.Private)]
	public XUi xui;

	// Token: 0x04005BBE RID: 23486
	[PublicizedFrom(EAccessModifier.Private)]
	public Equipment equipment;
}
