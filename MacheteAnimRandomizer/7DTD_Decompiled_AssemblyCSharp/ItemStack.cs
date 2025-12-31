using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x020004F6 RID: 1270
public class ItemStack
{
	// Token: 0x06002943 RID: 10563 RVA: 0x0010D6D7 File Offset: 0x0010B8D7
	public ItemStack()
	{
		this.itemValue = ItemValue.None.Clone();
		this.count = 0;
	}

	// Token: 0x06002944 RID: 10564 RVA: 0x0010D6F6 File Offset: 0x0010B8F6
	public ItemStack(ItemValue _itemValue, int _count)
	{
		this.itemValue = _itemValue;
		this.count = _count;
	}

	// Token: 0x06002945 RID: 10565 RVA: 0x0010D70C File Offset: 0x0010B90C
	public ItemStack Clone()
	{
		if (this.itemValue != null)
		{
			return new ItemStack(this.itemValue.Clone(), this.count);
		}
		return new ItemStack(ItemValue.None.Clone(), this.count);
	}

	// Token: 0x06002946 RID: 10566 RVA: 0x0010D744 File Offset: 0x0010B944
	public static ItemStack[] CreateArray(int _size)
	{
		ItemStack[] array = new ItemStack[_size];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = ItemStack.Empty.Clone();
		}
		return array;
	}

	// Token: 0x06002947 RID: 10567 RVA: 0x0010D774 File Offset: 0x0010B974
	public bool IsEmpty()
	{
		return this.count < 1 || this.itemValue.type == 0;
	}

	// Token: 0x06002948 RID: 10568 RVA: 0x0010D78F File Offset: 0x0010B98F
	public void Clear()
	{
		this.itemValue.Clear();
		this.count = 0;
	}

	// Token: 0x06002949 RID: 10569 RVA: 0x0010D7A4 File Offset: 0x0010B9A4
	public static int AddToItemStackArray(ItemStack[] _itemStackArr, ItemStack _itemStack, int maxItemCount = -1)
	{
		int num = -1;
		int num2 = 0;
		while (num == -1 && num2 < _itemStackArr.Length)
		{
			if (_itemStackArr[num2].CanStackWith(_itemStack, false))
			{
				_itemStackArr[num2].count += _itemStack.count;
				_itemStack.count = 0;
				num = num2;
			}
			num2++;
		}
		int num3 = 0;
		while (num == -1 && num3 < _itemStackArr.Length && (maxItemCount == -1 || num3 != maxItemCount))
		{
			if (_itemStackArr[num3].IsEmpty())
			{
				_itemStackArr[num3] = _itemStack;
				num = num3;
			}
			num3++;
		}
		return num;
	}

	// Token: 0x0600294A RID: 10570 RVA: 0x0010D81A File Offset: 0x0010BA1A
	public ItemStack ReadOld(BinaryReader _br)
	{
		this.itemValue.ReadOld(_br);
		this.count = (int)_br.ReadInt16();
		return this;
	}

	// Token: 0x0600294B RID: 10571 RVA: 0x0010D835 File Offset: 0x0010BA35
	public ItemStack Read(BinaryReader _br)
	{
		this.count = (int)_br.ReadUInt16();
		if (this.count > 0)
		{
			this.itemValue.Read(_br);
		}
		else
		{
			this.itemValue = ItemValue.None.Clone();
		}
		return this;
	}

	// Token: 0x0600294C RID: 10572 RVA: 0x0010D86C File Offset: 0x0010BA6C
	public ItemStack ReadDelta(BinaryReader _br, ItemStack _last)
	{
		this.itemValue.Read(_br);
		int num = (int)_br.ReadInt16();
		this.count = _last.count + num;
		return this;
	}

	// Token: 0x0600294D RID: 10573 RVA: 0x0010D89C File Offset: 0x0010BA9C
	public void Write(BinaryWriter _bw)
	{
		int num = this.count;
		if (num > 65535)
		{
			num = 65535;
		}
		_bw.Write((ushort)num);
		if (this.count != 0)
		{
			this.itemValue.Write(_bw);
		}
	}

	// Token: 0x0600294E RID: 10574 RVA: 0x0010D8DA File Offset: 0x0010BADA
	public void WriteDelta(BinaryWriter _bw, ItemStack _last)
	{
		this.itemValue.Write(_bw);
		_bw.Write((short)(this.count - _last.count));
		_last.count += this.count - _last.count;
	}

	// Token: 0x0600294F RID: 10575 RVA: 0x0010D918 File Offset: 0x0010BB18
	public override bool Equals(object _other)
	{
		ItemStack itemStack = _other as ItemStack;
		return itemStack != null && itemStack.count == this.count && itemStack.itemValue.Equals(this.itemValue);
	}

	// Token: 0x06002950 RID: 10576 RVA: 0x0010D952 File Offset: 0x0010BB52
	public override int GetHashCode()
	{
		return this.itemValue.GetHashCode() * 13 + this.count;
	}

	// Token: 0x06002951 RID: 10577 RVA: 0x0010D96C File Offset: 0x0010BB6C
	public int StackTransferCount(ItemStack _other)
	{
		if (_other.itemValue.type != this.itemValue.type)
		{
			return 0;
		}
		return Math.Min(ItemClass.GetForId(this.itemValue.type).Stacknumber.Value - this.count, _other.count);
	}

	// Token: 0x06002952 RID: 10578 RVA: 0x0010D9C0 File Offset: 0x0010BBC0
	public bool CanStackWith(ItemStack _other, bool allowPartialStack = false)
	{
		int num = _other.count;
		if (_other.itemValue == null || this.itemValue == null || _other.itemValue.type != this.itemValue.type || (this.itemValue.type < Block.ItemsStartHere && !(_other.itemValue.TextureFullArray == this.itemValue.TextureFullArray) && !this.itemValue.IsShapeHelperBlock))
		{
			return false;
		}
		if (!allowPartialStack)
		{
			return this.CanStack(_other.count);
		}
		return this.CanStackPartly(ref num);
	}

	// Token: 0x06002953 RID: 10579 RVA: 0x0010DA52 File Offset: 0x0010BC52
	public bool CanStack(int _count)
	{
		return this.itemValue.type == 0 || ItemClass.GetForId(this.itemValue.type).Stacknumber.Value >= _count + this.count;
	}

	// Token: 0x06002954 RID: 10580 RVA: 0x0010DA8A File Offset: 0x0010BC8A
	public bool CanStackPartlyWith(ItemStack _other)
	{
		return this.CanStackWith(_other, true);
	}

	// Token: 0x06002955 RID: 10581 RVA: 0x0010DA94 File Offset: 0x0010BC94
	public bool CanStackPartly(ref int _count)
	{
		if (this.itemValue.type == 0)
		{
			return false;
		}
		_count = Utils.FastMin(ItemClass.GetForId(this.itemValue.type).Stacknumber.Value - this.count, _count);
		return _count > 0;
	}

	// Token: 0x06002956 RID: 10582 RVA: 0x0010DAD4 File Offset: 0x0010BCD4
	public void Deactivate()
	{
		ItemClass forId = ItemClass.GetForId(this.itemValue.type);
		if (forId != null)
		{
			forId.Deactivate(this.itemValue);
		}
	}

	// Token: 0x06002957 RID: 10583 RVA: 0x0010DB04 File Offset: 0x0010BD04
	public static ItemStack[] Clone(IList<ItemStack> _itemStackArr)
	{
		ItemStack[] array = new ItemStack[_itemStackArr.Count];
		for (int i = 0; i < array.Length; i++)
		{
			ItemStack itemStack = _itemStackArr[i];
			array[i] = ((itemStack != null) ? itemStack.Clone() : ItemStack.Empty.Clone());
		}
		return array;
	}

	// Token: 0x06002958 RID: 10584 RVA: 0x0010DB4C File Offset: 0x0010BD4C
	public static ItemStack[] Clone(ItemStack[] _itemStackArr, int _startIdx, int _length)
	{
		ItemStack[] array = new ItemStack[_length];
		for (int i = 0; i < _length; i++)
		{
			ItemStack itemStack = (_startIdx + i < _itemStackArr.Length) ? _itemStackArr[_startIdx + i] : null;
			array[i] = ((itemStack != null) ? itemStack.Clone() : ItemStack.Empty.Clone());
		}
		return array;
	}

	// Token: 0x06002959 RID: 10585 RVA: 0x0010DB98 File Offset: 0x0010BD98
	public static ItemStack FromString(string _s)
	{
		ItemStack itemStack = ItemStack.Empty.Clone();
		if (_s.Contains("="))
		{
			int num = 0;
			int num2 = _s.IndexOf("=");
			if (int.TryParse(_s.Substring(num2 + 1), out num))
			{
				itemStack.count = num;
			}
			_s = _s.Substring(0, num2);
		}
		else
		{
			itemStack.count = 1;
		}
		itemStack.itemValue = ItemClass.GetItem(_s, false);
		return itemStack;
	}

	// Token: 0x0600295A RID: 10586 RVA: 0x0010DC05 File Offset: 0x0010BE05
	public override string ToString()
	{
		ItemValue itemValue = this.itemValue;
		return ((itemValue != null) ? itemValue.ToString() : null) + " cnt=" + this.count.ToString();
	}

	// Token: 0x0600295B RID: 10587 RVA: 0x0010DC30 File Offset: 0x0010BE30
	public static bool IsEmpty(ItemStack[] _slots)
	{
		if (_slots == null || _slots.Length == 0)
		{
			return true;
		}
		for (int i = 0; i < _slots.Length; i++)
		{
			if (!_slots[i].IsEmpty())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04002047 RID: 8263
	public static ItemStack Empty = new ItemStack(ItemValue.None, 0);

	// Token: 0x04002048 RID: 8264
	public ItemValue itemValue;

	// Token: 0x04002049 RID: 8265
	public int count;

	// Token: 0x020004F7 RID: 1271
	public enum EnumDragType
	{
		// Token: 0x0400204B RID: 8267
		DragTypeStart,
		// Token: 0x0400204C RID: 8268
		DragTypeAdd,
		// Token: 0x0400204D RID: 8269
		DragTypeExchange,
		// Token: 0x0400204E RID: 8270
		DragTypeOther
	}
}
