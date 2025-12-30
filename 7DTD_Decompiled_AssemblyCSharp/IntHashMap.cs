using System;

// Token: 0x020011AE RID: 4526
public class IntHashMap
{
	// Token: 0x06008D87 RID: 36231 RVA: 0x0038DCBA File Offset: 0x0038BEBA
	public IntHashMap()
	{
		this.threshold = 12;
		this.slots = new IntHashMapEntry[16];
	}

	// Token: 0x06008D88 RID: 36232 RVA: 0x0038DCD7 File Offset: 0x0038BED7
	[PublicizedFrom(EAccessModifier.Private)]
	public static int computeHash(int _v)
	{
		_v ^= (_v >> 20 ^ _v >> 12);
		return _v ^ _v >> 7 ^ _v >> 4;
	}

	// Token: 0x06008D89 RID: 36233 RVA: 0x0038DCEF File Offset: 0x0038BEEF
	[PublicizedFrom(EAccessModifier.Private)]
	public static int getSlotIndex(int _p1, int _p2)
	{
		return _p1 & _p2 - 1;
	}

	// Token: 0x06008D8A RID: 36234 RVA: 0x0038DCF8 File Offset: 0x0038BEF8
	public object lookup(int _p)
	{
		int p = IntHashMap.computeHash(_p);
		for (IntHashMapEntry intHashMapEntry = this.slots[IntHashMap.getSlotIndex(p, this.slots.Length)]; intHashMapEntry != null; intHashMapEntry = intHashMapEntry.nextEntry)
		{
			if (intHashMapEntry.hashEntry == _p)
			{
				return intHashMapEntry.valueEntry;
			}
		}
		return null;
	}

	// Token: 0x06008D8B RID: 36235 RVA: 0x0038DD3F File Offset: 0x0038BF3F
	public bool containsItem(int _item)
	{
		return this.lookupEntry(_item) != null;
	}

	// Token: 0x06008D8C RID: 36236 RVA: 0x0038DD4C File Offset: 0x0038BF4C
	[PublicizedFrom(EAccessModifier.Private)]
	public IntHashMapEntry lookupEntry(int _item)
	{
		int p = IntHashMap.computeHash(_item);
		for (IntHashMapEntry intHashMapEntry = this.slots[IntHashMap.getSlotIndex(p, this.slots.Length)]; intHashMapEntry != null; intHashMapEntry = intHashMapEntry.nextEntry)
		{
			if (intHashMapEntry.hashEntry == _item)
			{
				return intHashMapEntry;
			}
		}
		return null;
	}

	// Token: 0x06008D8D RID: 36237 RVA: 0x0038DD90 File Offset: 0x0038BF90
	public void addKey(int _key, object _value)
	{
		int num = IntHashMap.computeHash(_key);
		int slotIndex = IntHashMap.getSlotIndex(num, this.slots.Length);
		for (IntHashMapEntry intHashMapEntry = this.slots[slotIndex]; intHashMapEntry != null; intHashMapEntry = intHashMapEntry.nextEntry)
		{
			if (intHashMapEntry.hashEntry == _key)
			{
				intHashMapEntry.valueEntry = _value;
			}
		}
		this.versionStamp++;
		this.insert(num, _key, _value, slotIndex);
	}

	// Token: 0x06008D8E RID: 36238 RVA: 0x0038DDF4 File Offset: 0x0038BFF4
	[PublicizedFrom(EAccessModifier.Private)]
	public void grow(int _size)
	{
		if (this.slots.Length == 1073741824)
		{
			this.threshold = int.MaxValue;
			return;
		}
		IntHashMapEntry[] other = new IntHashMapEntry[_size];
		this.copyTo(other);
		this.slots = other;
		this.threshold = (int)((float)_size * 0.75f);
	}

	// Token: 0x06008D8F RID: 36239 RVA: 0x0038DE40 File Offset: 0x0038C040
	[PublicizedFrom(EAccessModifier.Private)]
	public void copyTo(IntHashMapEntry[] _other)
	{
		IntHashMapEntry[] array = this.slots;
		int p = _other.Length;
		for (int i = 0; i < array.Length; i++)
		{
			IntHashMapEntry intHashMapEntry = array[i];
			if (intHashMapEntry != null)
			{
				array[i] = null;
				do
				{
					IntHashMapEntry nextEntry = intHashMapEntry.nextEntry;
					int slotIndex = IntHashMap.getSlotIndex(intHashMapEntry.slotHash, p);
					intHashMapEntry.nextEntry = _other[slotIndex];
					_other[slotIndex] = intHashMapEntry;
					intHashMapEntry = nextEntry;
				}
				while (intHashMapEntry != null);
			}
		}
	}

	// Token: 0x06008D90 RID: 36240 RVA: 0x0038DE98 File Offset: 0x0038C098
	public object removeObject(int _key)
	{
		IntHashMapEntry intHashMapEntry = this.removeEntry(_key);
		if (intHashMapEntry == null)
		{
			return null;
		}
		return intHashMapEntry.valueEntry;
	}

	// Token: 0x06008D91 RID: 36241 RVA: 0x0038DEB8 File Offset: 0x0038C0B8
	[PublicizedFrom(EAccessModifier.Private)]
	public IntHashMapEntry removeEntry(int _key)
	{
		int slotIndex = IntHashMap.getSlotIndex(IntHashMap.computeHash(_key), this.slots.Length);
		IntHashMapEntry intHashMapEntry = this.slots[slotIndex];
		IntHashMapEntry intHashMapEntry2;
		IntHashMapEntry nextEntry;
		for (intHashMapEntry2 = intHashMapEntry; intHashMapEntry2 != null; intHashMapEntry2 = nextEntry)
		{
			nextEntry = intHashMapEntry2.nextEntry;
			if (intHashMapEntry2.hashEntry == _key)
			{
				this.versionStamp++;
				this.count--;
				if (intHashMapEntry == intHashMapEntry2)
				{
					this.slots[slotIndex] = nextEntry;
				}
				else
				{
					intHashMapEntry.nextEntry = nextEntry;
				}
				return intHashMapEntry2;
			}
			intHashMapEntry = intHashMapEntry2;
		}
		return intHashMapEntry2;
	}

	// Token: 0x06008D92 RID: 36242 RVA: 0x0038DF38 File Offset: 0x0038C138
	public void clearMap()
	{
		this.versionStamp++;
		IntHashMapEntry[] array = this.slots;
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = null;
		}
		this.count = 0;
	}

	// Token: 0x06008D93 RID: 36243 RVA: 0x0038DF78 File Offset: 0x0038C178
	public void map(Action<object> callback)
	{
		for (int i = 0; i < this.slots.Length; i++)
		{
			for (IntHashMapEntry intHashMapEntry = this.slots[i]; intHashMapEntry != null; intHashMapEntry = intHashMapEntry.nextEntry)
			{
				callback(intHashMapEntry.valueEntry);
			}
		}
	}

	// Token: 0x06008D94 RID: 36244 RVA: 0x0038DFBC File Offset: 0x0038C1BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void insert(int _i1, int _i2, object _object, int _key)
	{
		IntHashMapEntry entry = this.slots[_key];
		this.slots[_key] = new IntHashMapEntry(_i1, _i2, _object, entry);
		int num = this.count;
		this.count = num + 1;
		if (num >= this.threshold)
		{
			this.grow(2 * this.slots.Length);
		}
	}

	// Token: 0x06008D95 RID: 36245 RVA: 0x0038E00E File Offset: 0x0038C20E
	public static int getHash(int _v)
	{
		return IntHashMap.computeHash(_v);
	}

	// Token: 0x04006DCA RID: 28106
	[PublicizedFrom(EAccessModifier.Private)]
	public IntHashMapEntry[] slots;

	// Token: 0x04006DCB RID: 28107
	[PublicizedFrom(EAccessModifier.Private)]
	public int count;

	// Token: 0x04006DCC RID: 28108
	[PublicizedFrom(EAccessModifier.Private)]
	public int threshold;

	// Token: 0x04006DCD RID: 28109
	[PublicizedFrom(EAccessModifier.Private)]
	public const float growFactor = 0.75f;

	// Token: 0x04006DCE RID: 28110
	[PublicizedFrom(EAccessModifier.Private)]
	public volatile int versionStamp;
}
