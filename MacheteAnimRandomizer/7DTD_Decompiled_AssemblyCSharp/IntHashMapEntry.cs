using System;

// Token: 0x020011AF RID: 4527
[PublicizedFrom(EAccessModifier.Internal)]
public class IntHashMapEntry
{
	// Token: 0x06008D96 RID: 36246 RVA: 0x0038E016 File Offset: 0x0038C216
	public IntHashMapEntry(int _i1, int _i2, object _obj, IntHashMapEntry _entry)
	{
		this.valueEntry = _obj;
		this.nextEntry = _entry;
		this.hashEntry = _i2;
		this.slotHash = _i1;
	}

	// Token: 0x06008D97 RID: 36247 RVA: 0x0038E03B File Offset: 0x0038C23B
	public int getHash()
	{
		return this.hashEntry;
	}

	// Token: 0x06008D98 RID: 36248 RVA: 0x0038E043 File Offset: 0x0038C243
	public object getValue()
	{
		return this.valueEntry;
	}

	// Token: 0x06008D99 RID: 36249 RVA: 0x0038E04C File Offset: 0x0038C24C
	public bool equals(object _obj)
	{
		if (!(_obj is IntHashMapEntry))
		{
			return false;
		}
		IntHashMapEntry intHashMapEntry = (IntHashMapEntry)_obj;
		int hash = this.getHash();
		int hash2 = intHashMapEntry.getHash();
		if (hash == hash2)
		{
			object value = this.getValue();
			object value2 = intHashMapEntry.getValue();
			if (value == value2 || (value != null && value.Equals(value2)))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06008D9A RID: 36250 RVA: 0x0038E09B File Offset: 0x0038C29B
	public int hashCode()
	{
		return IntHashMap.getHash(this.hashEntry);
	}

	// Token: 0x06008D9B RID: 36251 RVA: 0x0038E0A8 File Offset: 0x0038C2A8
	public string toString()
	{
		string str = this.getHash().ToString();
		string str2 = "=";
		object value = this.getValue();
		return str + str2 + ((value != null) ? value.ToString() : null);
	}

	// Token: 0x04006DCF RID: 28111
	public int hashEntry;

	// Token: 0x04006DD0 RID: 28112
	public object valueEntry;

	// Token: 0x04006DD1 RID: 28113
	public IntHashMapEntry nextEntry;

	// Token: 0x04006DD2 RID: 28114
	public int slotHash;
}
