using System;
using System.Collections.Generic;
using MemoryPack;

// Token: 0x0200117E RID: 4478
public class DataItem<T> : IDataItem
{
	// Token: 0x140000F5 RID: 245
	// (add) Token: 0x06008BCE RID: 35790 RVA: 0x003855A0 File Offset: 0x003837A0
	// (remove) Token: 0x06008BCF RID: 35791 RVA: 0x003855D8 File Offset: 0x003837D8
	public event DataItem<T>.OnChangeDelegate OnChangeDelegates;

	// Token: 0x17000E92 RID: 3730
	// (get) Token: 0x06008BD0 RID: 35792 RVA: 0x0038560D File Offset: 0x0038380D
	public string Name
	{
		get
		{
			return this.name;
		}
	}

	// Token: 0x06008BD1 RID: 35793 RVA: 0x00385618 File Offset: 0x00383818
	[MemoryPackConstructor]
	public DataItem() : this(default(T))
	{
	}

	// Token: 0x06008BD2 RID: 35794 RVA: 0x00385634 File Offset: 0x00383834
	public DataItem(T _startValue) : this(null, _startValue)
	{
	}

	// Token: 0x06008BD3 RID: 35795 RVA: 0x0038563E File Offset: 0x0038383E
	public DataItem(string _name, T _startValue)
	{
		this.name = _name;
		this.internalValue = _startValue;
	}

	// Token: 0x17000E93 RID: 3731
	// (get) Token: 0x06008BD5 RID: 35797 RVA: 0x00385684 File Offset: 0x00383884
	// (set) Token: 0x06008BD4 RID: 35796 RVA: 0x00385654 File Offset: 0x00383854
	public T Value
	{
		get
		{
			return this.internalValue;
		}
		set
		{
			T oldValue = this.internalValue;
			this.internalValue = value;
			if (this.OnChangeDelegates != null)
			{
				this.OnChangeDelegates(oldValue, value);
			}
		}
	}

	// Token: 0x06008BD6 RID: 35798 RVA: 0x0038568C File Offset: 0x0038388C
	public override string ToString()
	{
		if (this.Formatter != null)
		{
			return this.Formatter.ToString(this.internalValue);
		}
		if (this.internalValue == null)
		{
			return "null";
		}
		return this.internalValue.ToString();
	}

	// Token: 0x06008BD7 RID: 35799 RVA: 0x003856DC File Offset: 0x003838DC
	public static bool operator ==(DataItem<T> v1, T v2)
	{
		if (v1 != null)
		{
			return EqualityComparer<T>.Default.Equals(v1.internalValue, v2);
		}
		return v2 == null;
	}

	// Token: 0x06008BD8 RID: 35800 RVA: 0x003856FC File Offset: 0x003838FC
	public static bool operator !=(DataItem<T> v1, T v2)
	{
		if (v1 != null)
		{
			return !EqualityComparer<T>.Default.Equals(v1.internalValue, v2);
		}
		return v2 != null;
	}

	// Token: 0x06008BD9 RID: 35801 RVA: 0x0038571F File Offset: 0x0038391F
	public override bool Equals(object obj)
	{
		return obj != null && this.internalValue.Equals(obj);
	}

	// Token: 0x06008BDA RID: 35802 RVA: 0x00385738 File Offset: 0x00383938
	public override int GetHashCode()
	{
		return this.internalValue.GetHashCode();
	}

	// Token: 0x04006D1A RID: 27930
	[PublicizedFrom(EAccessModifier.Private)]
	public string name;

	// Token: 0x04006D1B RID: 27931
	[PublicizedFrom(EAccessModifier.Private)]
	public T internalValue;

	// Token: 0x04006D1C RID: 27932
	public IDataItemFormatter Formatter;

	// Token: 0x0200117F RID: 4479
	// (Invoke) Token: 0x06008BDC RID: 35804
	public delegate void OnChangeDelegate(T _oldValue, T _newValue);
}
