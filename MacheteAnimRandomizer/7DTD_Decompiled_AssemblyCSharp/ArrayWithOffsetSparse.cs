using System;
using System.Collections.Generic;

// Token: 0x02001117 RID: 4375
public class ArrayWithOffsetSparse<T>
{
	// Token: 0x0600898D RID: 35213 RVA: 0x0037C371 File Offset: 0x0037A571
	public ArrayWithOffsetSparse(int _dimX, int _dimY, T _emptyValue)
	{
		this.DimX = _dimX;
		this.DimY = _dimY;
		this.EmptyValue = _emptyValue;
		this.myData = new Dictionary<long, T>();
		this.addX = _dimX / 2;
		this.addY = _dimY / 2;
	}

	// Token: 0x0600898E RID: 35214 RVA: 0x0037C3AB File Offset: 0x0037A5AB
	[PublicizedFrom(EAccessModifier.Private)]
	public long makeKey(int _x, int _y)
	{
		return ((long)(_y + this.addY) & (long)((ulong)-1)) << 32 | ((long)(_x + this.addX) & (long)((ulong)-1));
	}

	// Token: 0x0600898F RID: 35215 RVA: 0x0037C3C9 File Offset: 0x0037A5C9
	public bool Contains(int _x, int _y)
	{
		return _x >= -this.addX && _y >= -this.addY && _x < this.addX - 1 && _y < this.addY - 1;
	}

	// Token: 0x17000E50 RID: 3664
	public virtual T this[int _x, int _y]
	{
		get
		{
			if (!this.myData.ContainsKey(this.makeKey(_x, _y)))
			{
				return this.EmptyValue;
			}
			return this.myData[this.makeKey(_x, _y)];
		}
		set
		{
			this.myData[this.makeKey(_x, _y)] = value;
		}
	}

	// Token: 0x04006BDF RID: 27615
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<long, T> myData;

	// Token: 0x04006BE0 RID: 27616
	public T EmptyValue;

	// Token: 0x04006BE1 RID: 27617
	[PublicizedFrom(EAccessModifier.Private)]
	public int addX;

	// Token: 0x04006BE2 RID: 27618
	[PublicizedFrom(EAccessModifier.Private)]
	public int addY;

	// Token: 0x04006BE3 RID: 27619
	public int DimX;

	// Token: 0x04006BE4 RID: 27620
	public int DimY;
}
