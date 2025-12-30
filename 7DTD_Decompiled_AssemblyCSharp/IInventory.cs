using System;
using System.Runtime.CompilerServices;

// Token: 0x020004EC RID: 1260
public interface IInventory
{
	// Token: 0x060028BB RID: 10427
	bool AddItem(ItemStack _itemStack);

	// Token: 0x060028BC RID: 10428
	[return: TupleElementNames(new string[]
	{
		"anyMoved",
		"allMoved"
	})]
	ValueTuple<bool, bool> TryStackItem(int _startIndex, ItemStack _itemStack);

	// Token: 0x060028BD RID: 10429
	bool HasItem(ItemValue _item);
}
