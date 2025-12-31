using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000EE2 RID: 3810
public class XUiM_LootContainer : XUiModel
{
	// Token: 0x06007829 RID: 30761 RVA: 0x0030FCD3 File Offset: 0x0030DED3
	public static bool AddItem(ItemStack _itemStack, XUi _xui)
	{
		if (_xui.lootContainer == null)
		{
			return false;
		}
		_xui.lootContainer.TryStackItem(0, _itemStack);
		return _itemStack.count > 0 && _xui.lootContainer.AddItem(_itemStack);
	}

	// Token: 0x0600782A RID: 30762 RVA: 0x0030FD08 File Offset: 0x0030DF08
	public static bool TakeAll(XUi _xui)
	{
		XUiM_PlayerInventory playerInventory = _xui.PlayerInventory;
		ItemStack[] items = _xui.lootContainer.items;
		bool result = true;
		for (int i = 0; i < items.Length; i++)
		{
			if (!items[i].IsEmpty())
			{
				ItemStack itemStack = items[i].Clone();
				if (!playerInventory.AddItem(itemStack))
				{
					playerInventory.DropItem(itemStack);
					result = false;
				}
				_xui.lootContainer.UpdateSlot(i, ItemStack.Empty.Clone());
			}
		}
		return result;
	}

	// Token: 0x0600782B RID: 30763 RVA: 0x0030FD78 File Offset: 0x0030DF78
	[return: TupleElementNames(new string[]
	{
		"_allMoved",
		"_anyMoved"
	})]
	public static ValueTuple<bool, bool> StashItems(XUiController _srcWindow, XUiC_ItemStackGrid _srcGrid, IInventory _dstInventory, int _ignoreSlots, PackedBoolArray _ignoredSlots, XUiM_LootContainer.EItemMoveKind _moveKind, bool _startBottomRight)
	{
		if (_srcGrid == null || _dstInventory == null)
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		float unscaledTime = Time.unscaledTime;
		if (_moveKind == XUiM_LootContainer.EItemMoveKind.FillOnlyFirstCreateSecond && unscaledTime - XUiM_LootContainer.lastStashTime < 2f)
		{
			_moveKind = XUiM_LootContainer.EItemMoveKind.FillAndCreate;
		}
		bool item = true;
		bool item2 = false;
		PreferenceTracker preferenceTracker = null;
		XUiC_LootWindow xuiC_LootWindow = _srcWindow as XUiC_LootWindow;
		if (xuiC_LootWindow != null)
		{
			preferenceTracker = xuiC_LootWindow.GetPreferenceTrackerFromTileEntity();
		}
		if (preferenceTracker != null && preferenceTracker.AnyPreferences)
		{
			XUiM_PlayerInventory xuiM_PlayerInventory = _dstInventory as XUiM_PlayerInventory;
			if (xuiM_PlayerInventory != null)
			{
				item2 = xuiM_PlayerInventory.AddItemsUsingPreferenceTracker(_srcGrid, preferenceTracker);
			}
		}
		XUiController[] itemStackControllers = _srcGrid.GetItemStackControllers();
		XUiController[] array = itemStackControllers;
		int num = _startBottomRight ? (array.Length - 1) : 0;
		while (_startBottomRight ? (num >= 0) : (num < array.Length))
		{
			if (!StackSortUtil.IsIgnoredSlot(_ignoreSlots, _ignoredSlots, num))
			{
				XUiC_ItemStack xuiC_ItemStack = (XUiC_ItemStack)array[num];
				if (!xuiC_ItemStack.StackLock)
				{
					ItemStack itemStack = xuiC_ItemStack.ItemStack;
					if (!xuiC_ItemStack.ItemStack.IsEmpty())
					{
						int count = itemStack.count;
						_dstInventory.TryStackItem(0, itemStack);
						if (itemStack.count > 0 && (_moveKind == XUiM_LootContainer.EItemMoveKind.All || (_moveKind == XUiM_LootContainer.EItemMoveKind.FillAndCreate && _dstInventory.HasItem(itemStack.itemValue))) && _dstInventory.AddItem(itemStack))
						{
							itemStack = ItemStack.Empty.Clone();
						}
						if (itemStack.count == 0)
						{
							itemStack = ItemStack.Empty.Clone();
						}
						else
						{
							item = false;
						}
						if (count != itemStack.count)
						{
							xuiC_ItemStack.ForceSetItemStack(itemStack);
							item2 = true;
						}
					}
				}
			}
			num = (_startBottomRight ? (num - 1) : (num + 1));
		}
		XUiM_LootContainer.lastStashTime = unscaledTime;
		return new ValueTuple<bool, bool>(item, item2);
	}

	// Token: 0x04005B9B RID: 23451
	[PublicizedFrom(EAccessModifier.Private)]
	public static float lastStashTime;

	// Token: 0x04005B9C RID: 23452
	[PublicizedFrom(EAccessModifier.Private)]
	public const float SecondClickMaxDelaySec = 2f;

	// Token: 0x02000EE3 RID: 3811
	public enum EItemMoveKind
	{
		// Token: 0x04005B9E RID: 23454
		All,
		// Token: 0x04005B9F RID: 23455
		FillOnly,
		// Token: 0x04005BA0 RID: 23456
		FillAndCreate,
		// Token: 0x04005BA1 RID: 23457
		FillOnlyFirstCreateSecond
	}
}
