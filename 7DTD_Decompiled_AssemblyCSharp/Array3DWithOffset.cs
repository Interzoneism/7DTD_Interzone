using System;

// Token: 0x02001113 RID: 4371
public class Array3DWithOffset<T>
{
	// Token: 0x06008971 RID: 35185 RVA: 0x0000A7E3 File Offset: 0x000089E3
	public Array3DWithOffset()
	{
	}

	// Token: 0x06008972 RID: 35186 RVA: 0x0037BD0C File Offset: 0x00379F0C
	public Array3DWithOffset(int _dimX, int _dimY, int _dimZ)
	{
		this.DimX = _dimX;
		this.DimY = _dimY;
		this.DimZ = _dimZ;
		this.data = new T[_dimX * _dimY * _dimZ];
		this.addX = _dimX / 2;
		this.addY = _dimY / 2;
		this.addZ = _dimZ / 2;
	}

	// Token: 0x06008973 RID: 35187 RVA: 0x0037BD5F File Offset: 0x00379F5F
	[PublicizedFrom(EAccessModifier.Protected)]
	public int GetIndex(int _x, int _y, int _z)
	{
		return _x + this.addX + (_z + this.addZ) * this.DimX + (_y + this.addY) * this.DimZ * this.DimX;
	}

	// Token: 0x17000E4D RID: 3661
	public virtual T this[int _x, int _y, int _z]
	{
		get
		{
			return this.data[this.GetIndex(_x, _y, _z)];
		}
		set
		{
			this.data[this.GetIndex(_x, _y, _z)] = value;
		}
	}

	// Token: 0x06008976 RID: 35190 RVA: 0x0037BDC0 File Offset: 0x00379FC0
	public bool Contains(int _x, int _y, int _z)
	{
		return _x >= -this.addX && _y >= -this.addY && _x < this.addX - 1 && _y < this.addY - 1 && _z < this.addZ - 1 && _z < this.addZ - 1;
	}

	// Token: 0x04006BC8 RID: 27592
	[PublicizedFrom(EAccessModifier.Protected)]
	public T[] data;

	// Token: 0x04006BC9 RID: 27593
	[PublicizedFrom(EAccessModifier.Protected)]
	public int addX;

	// Token: 0x04006BCA RID: 27594
	[PublicizedFrom(EAccessModifier.Protected)]
	public int addY;

	// Token: 0x04006BCB RID: 27595
	[PublicizedFrom(EAccessModifier.Protected)]
	public int addZ;

	// Token: 0x04006BCC RID: 27596
	public int DimX;

	// Token: 0x04006BCD RID: 27597
	public int DimY;

	// Token: 0x04006BCE RID: 27598
	public int DimZ;
}
