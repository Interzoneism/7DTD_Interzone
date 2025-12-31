using System;

// Token: 0x02000AEF RID: 2799
public interface ITileEntityLootable : ITileEntity, IInventory
{
	// Token: 0x1700087C RID: 2172
	// (get) Token: 0x06005600 RID: 22016
	// (set) Token: 0x06005601 RID: 22017
	string lootListName { get; set; }

	// Token: 0x1700087D RID: 2173
	// (get) Token: 0x06005602 RID: 22018
	float LootStageMod { get; }

	// Token: 0x1700087E RID: 2174
	// (get) Token: 0x06005603 RID: 22019
	float LootStageBonus { get; }

	// Token: 0x1700087F RID: 2175
	// (get) Token: 0x06005604 RID: 22020
	// (set) Token: 0x06005605 RID: 22021
	bool bPlayerBackpack { get; set; }

	// Token: 0x17000880 RID: 2176
	// (get) Token: 0x06005606 RID: 22022
	// (set) Token: 0x06005607 RID: 22023
	bool bPlayerStorage { get; set; }

	// Token: 0x17000881 RID: 2177
	// (get) Token: 0x06005608 RID: 22024
	// (set) Token: 0x06005609 RID: 22025
	PreferenceTracker preferences { get; set; }

	// Token: 0x17000882 RID: 2178
	// (get) Token: 0x0600560A RID: 22026
	// (set) Token: 0x0600560B RID: 22027
	bool bTouched { get; set; }

	// Token: 0x17000883 RID: 2179
	// (get) Token: 0x0600560C RID: 22028
	// (set) Token: 0x0600560D RID: 22029
	ulong worldTimeTouched { get; set; }

	// Token: 0x17000884 RID: 2180
	// (get) Token: 0x0600560E RID: 22030
	// (set) Token: 0x0600560F RID: 22031
	bool bWasTouched { get; set; }

	// Token: 0x17000885 RID: 2181
	// (get) Token: 0x06005610 RID: 22032
	// (set) Token: 0x06005611 RID: 22033
	ItemStack[] items { get; set; }

	// Token: 0x06005612 RID: 22034
	Vector2i GetContainerSize();

	// Token: 0x06005613 RID: 22035
	void SetContainerSize(Vector2i _containerSize, bool _clearItems = true);

	// Token: 0x06005614 RID: 22036
	void UpdateSlot(int _idx, ItemStack _item);

	// Token: 0x06005615 RID: 22037
	void RemoveItem(ItemValue _item);

	// Token: 0x06005616 RID: 22038
	bool IsEmpty();

	// Token: 0x06005617 RID: 22039
	void SetEmpty();

	// Token: 0x17000886 RID: 2182
	// (get) Token: 0x06005618 RID: 22040
	bool HasSlotLocksSupport { get; }

	// Token: 0x17000887 RID: 2183
	// (get) Token: 0x06005619 RID: 22041
	// (set) Token: 0x0600561A RID: 22042
	PackedBoolArray SlotLocks { get; set; }
}
