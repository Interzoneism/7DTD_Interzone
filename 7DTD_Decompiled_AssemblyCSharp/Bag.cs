using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020004EB RID: 1259
public class Bag : IInventory
{
	// Token: 0x14000037 RID: 55
	// (add) Token: 0x060028A1 RID: 10401 RVA: 0x0010A4E4 File Offset: 0x001086E4
	// (remove) Token: 0x060028A2 RID: 10402 RVA: 0x0010A51C File Offset: 0x0010871C
	public event XUiEvent_BackpackItemsChangedInternal OnBackpackItemsChangedInternal;

	// Token: 0x060028A3 RID: 10403 RVA: 0x0010A551 File Offset: 0x00108751
	public Bag(EntityAlive _entity)
	{
		this.entity = _entity;
	}

	// Token: 0x1700042F RID: 1071
	// (get) Token: 0x060028A4 RID: 10404 RVA: 0x0010A567 File Offset: 0x00108767
	public int SlotCount
	{
		get
		{
			return this.GetSlots().Length;
		}
	}

	// Token: 0x17000430 RID: 1072
	// (get) Token: 0x060028A5 RID: 10405 RVA: 0x0010A571 File Offset: 0x00108771
	// (set) Token: 0x060028A6 RID: 10406 RVA: 0x0010A579 File Offset: 0x00108779
	public PackedBoolArray LockedSlots { get; set; }

	// Token: 0x060028A7 RID: 10407 RVA: 0x0010A584 File Offset: 0x00108784
	[PublicizedFrom(EAccessModifier.Private)]
	public void checkBagAssigned(int slotCount = 45)
	{
		if (this.items == null)
		{
			this.items = ItemStack.CreateArray((int)EffectManager.GetValue(PassiveEffects.BagSize, null, (float)slotCount, this.entity, null, default(FastTags<TagGroup.Global>), false, false, true, true, true, 1, true, false));
		}
	}

	// Token: 0x060028A8 RID: 10408 RVA: 0x0010A5C8 File Offset: 0x001087C8
	public void Clear()
	{
		ItemStack[] array = this.items;
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Clear();
			}
		}
		this.onBackpackChanged();
	}

	// Token: 0x060028A9 RID: 10409 RVA: 0x0010A5FB File Offset: 0x001087FB
	[PublicizedFrom(EAccessModifier.Private)]
	public void onBackpackChanged()
	{
		if (this.OnBackpackItemsChangedInternal != null)
		{
			this.OnBackpackItemsChangedInternal();
		}
	}

	// Token: 0x060028AA RID: 10410 RVA: 0x0010A610 File Offset: 0x00108810
	public bool CanStackNoEmpty(ItemStack _itemStack)
	{
		ItemStack[] slots = this.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].CanStackPartlyWith(_itemStack))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060028AB RID: 10411 RVA: 0x0010A640 File Offset: 0x00108840
	public bool IsEmpty()
	{
		ItemStack[] slots = this.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			if (!slots[i].IsEmpty())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060028AC RID: 10412 RVA: 0x0010A670 File Offset: 0x00108870
	public bool CanStack(ItemStack _itemStack)
	{
		ItemStack[] slots = this.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].IsEmpty() || slots[i].CanStackWith(_itemStack, false))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060028AD RID: 10413 RVA: 0x0010A6AC File Offset: 0x001088AC
	[return: TupleElementNames(new string[]
	{
		"anyMoved",
		"allMoved"
	})]
	public ValueTuple<bool, bool> TryStackItem(int startIndex, ItemStack _itemStack)
	{
		ItemStack[] slots = this.GetSlots();
		int num = 0;
		bool item = false;
		for (int i = startIndex; i < slots.Length; i++)
		{
			num = _itemStack.count;
			if (_itemStack.itemValue.type == slots[i].itemValue.type && slots[i].CanStackPartly(ref num))
			{
				slots[i].count += num;
				_itemStack.count -= num;
				this.onBackpackChanged();
				item = true;
				if (_itemStack.count == 0)
				{
					return new ValueTuple<bool, bool>(true, true);
				}
			}
		}
		return new ValueTuple<bool, bool>(item, false);
	}

	// Token: 0x060028AE RID: 10414 RVA: 0x0010A73C File Offset: 0x0010893C
	public bool CanTakeItem(ItemStack _itemStack)
	{
		ItemStack[] slots = this.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].CanStackPartlyWith(_itemStack))
			{
				return true;
			}
			if (slots[i].IsEmpty())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060028AF RID: 10415 RVA: 0x0010A778 File Offset: 0x00108978
	public ItemStack[] GetSlots()
	{
		this.checkBagAssigned(45);
		return this.items;
	}

	// Token: 0x060028B0 RID: 10416 RVA: 0x0010A788 File Offset: 0x00108988
	public void SetupSlots(ItemStack[] _slots)
	{
		this.checkBagAssigned(_slots.Length);
		this.items = _slots;
		this.onBackpackChanged();
	}

	// Token: 0x060028B1 RID: 10417 RVA: 0x0010A7A0 File Offset: 0x001089A0
	public void SetSlots(ItemStack[] _slots)
	{
		this.checkBagAssigned(45);
		this.items = _slots;
		this.onBackpackChanged();
	}

	// Token: 0x060028B2 RID: 10418 RVA: 0x0010A7B7 File Offset: 0x001089B7
	public void SetSlot(int index, ItemStack _stack, bool callChangedEvent = true)
	{
		if (index >= this.items.Length)
		{
			return;
		}
		this.items[index] = _stack;
		if (callChangedEvent)
		{
			this.onBackpackChanged();
		}
	}

	// Token: 0x060028B3 RID: 10419 RVA: 0x0010A7D8 File Offset: 0x001089D8
	public void OnUpdate()
	{
		if (this.entity is EntityPlayer)
		{
			this.MaxItemCount = (int)EffectManager.GetValue(PassiveEffects.CarryCapacity, null, 0f, this.entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			this.entity.Buffs.SetCustomVar("_carrycapacity", (float)this.MaxItemCount, true, CVarOperation.set);
			this.entity.Buffs.SetCustomVar("_encumbrance", Mathf.Max((float)(this.GetUsedSlotCount() - this.MaxItemCount), 0f) / (float)(this.items.Length - this.MaxItemCount), true, CVarOperation.set);
			this.entity.Buffs.SetCustomVar("_encumberedslots", Mathf.Max((float)(this.GetUsedSlotCount() - this.MaxItemCount), 0f), true, CVarOperation.set);
		}
	}

	// Token: 0x060028B4 RID: 10420 RVA: 0x0010A8B0 File Offset: 0x00108AB0
	public bool AddItem(ItemStack _itemStack)
	{
		if (this.items == null)
		{
			this.items = ItemStack.CreateArray((int)EffectManager.GetValue(PassiveEffects.BagSize, null, 45f, this.entity, null, default(FastTags<TagGroup.Global>), false, false, true, true, true, 1, true, false));
		}
		bool flag = ItemStack.AddToItemStackArray(this.items, _itemStack, -1) >= 0;
		if (flag)
		{
			this.onBackpackChanged();
		}
		return flag;
	}

	// Token: 0x060028B5 RID: 10421 RVA: 0x0010A914 File Offset: 0x00108B14
	public int DecItem(ItemValue _itemValue, int _count, bool _ignoreModdedItems = false, IList<ItemStack> _removedItems = null)
	{
		int num = _count;
		ItemStack[] slots = this.GetSlots();
		int num2 = 0;
		while (_count > 0 && num2 < this.GetSlots().Length)
		{
			if (slots[num2].itemValue.type == _itemValue.type && (!_ignoreModdedItems || !slots[num2].itemValue.HasModSlots || !slots[num2].itemValue.HasMods()))
			{
				if (ItemClass.GetForId(slots[num2].itemValue.type).CanStack())
				{
					int count = slots[num2].count;
					int num3 = (count >= _count) ? _count : count;
					if (_removedItems != null)
					{
						_removedItems.Add(new ItemStack(slots[num2].itemValue.Clone(), num3));
					}
					slots[num2].count -= num3;
					_count -= num3;
					if (slots[num2].count <= 0)
					{
						slots[num2].Clear();
					}
				}
				else
				{
					if (_removedItems != null)
					{
						_removedItems.Add(slots[num2].Clone());
					}
					slots[num2].Clear();
					_count--;
				}
			}
			num2++;
		}
		this.SetSlots(slots);
		return num - _count;
	}

	// Token: 0x060028B6 RID: 10422 RVA: 0x0010AA24 File Offset: 0x00108C24
	public int GetUsedSlotCount()
	{
		ItemStack[] slots = this.GetSlots();
		int num = 0;
		for (int i = 0; i < slots.Length; i++)
		{
			if (!slots[i].IsEmpty())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060028B7 RID: 10423 RVA: 0x0010AA58 File Offset: 0x00108C58
	public int GetItemCount(ItemValue _itemValue, int _seed = -1, int _meta = -1, bool _ignoreModdedItems = true)
	{
		ItemStack[] slots = this.GetSlots();
		int num = 0;
		for (int i = 0; i < slots.Length; i++)
		{
			if ((!_ignoreModdedItems || !slots[i].itemValue.HasModSlots || !slots[i].itemValue.HasMods()) && slots[i].itemValue.type == _itemValue.type && (_seed == -1 || _seed == (int)slots[i].itemValue.Seed) && (_meta == -1 || _meta == slots[i].itemValue.Meta))
			{
				num += slots[i].count;
			}
		}
		return num;
	}

	// Token: 0x060028B8 RID: 10424 RVA: 0x0010AAE8 File Offset: 0x00108CE8
	public int GetItemCount(FastTags<TagGroup.Global> itemTags, int _seed = -1, int _meta = -1, bool _ignoreModdedItems = true)
	{
		ItemStack[] slots = this.GetSlots();
		int num = 0;
		for (int i = 0; i < slots.Length; i++)
		{
			if ((!_ignoreModdedItems || !slots[i].itemValue.HasModSlots || !slots[i].itemValue.HasMods()) && !slots[i].itemValue.IsEmpty() && slots[i].itemValue.ItemClass.ItemTags.Test_AnySet(itemTags) && (_seed == -1 || _seed == (int)slots[i].itemValue.Seed) && (_meta == -1 || _meta == slots[i].itemValue.Meta))
			{
				num += slots[i].count;
			}
		}
		return num;
	}

	// Token: 0x060028B9 RID: 10425 RVA: 0x0010AB91 File Offset: 0x00108D91
	public bool HasItem(ItemValue _item)
	{
		return this.GetItemCount(_item, -1, -1, true) > 0;
	}

	// Token: 0x060028BA RID: 10426 RVA: 0x0010ABA0 File Offset: 0x00108DA0
	public ItemStack[] CloneItemStack()
	{
		this.checkBagAssigned(45);
		ItemStack[] array = new ItemStack[this.items.Length];
		for (int i = 0; i < this.items.Length; i++)
		{
			array[i] = this.items[i].Clone();
		}
		return array;
	}

	// Token: 0x04001FF7 RID: 8183
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive entity;

	// Token: 0x04001FF8 RID: 8184
	public int MaxItemCount = -1;

	// Token: 0x04001FF9 RID: 8185
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] items;
}
