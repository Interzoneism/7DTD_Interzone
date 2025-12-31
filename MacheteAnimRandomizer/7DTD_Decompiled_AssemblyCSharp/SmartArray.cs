using System;
using System.IO;

// Token: 0x02001215 RID: 4629
public class SmartArray
{
	// Token: 0x06009066 RID: 36966 RVA: 0x0039968C File Offset: 0x0039788C
	public SmartArray(int xPow, int yPow, int zPos)
	{
		this._lXPow = xPow;
		this._lYPow = yPow;
		this._lZPow = zPos;
		this._size = (1 << this._lXPow) * (1 << this._lYPow) * (1 << this._lZPow);
		this._halfSize = this._size / 2;
		this._array = new byte[this._halfSize];
	}

	// Token: 0x06009067 RID: 36967 RVA: 0x003996FC File Offset: 0x003978FC
	public void clear()
	{
		for (int i = 0; i < this._array.Length; i++)
		{
			this._array[i] = 0;
		}
	}

	// Token: 0x06009068 RID: 36968 RVA: 0x00399725 File Offset: 0x00397925
	public void write(BinaryWriter stream)
	{
		stream.Write(this._array);
	}

	// Token: 0x06009069 RID: 36969 RVA: 0x00399733 File Offset: 0x00397933
	public void read(BinaryReader stream)
	{
		this._array = stream.ReadBytes(this._halfSize);
	}

	// Token: 0x0600906A RID: 36970 RVA: 0x00399748 File Offset: 0x00397948
	public byte get(int x, int y, int z)
	{
		int num = (x << this._lXPow << this._lYPow) + (y << this._lXPow) + z;
		if (num < this._halfSize)
		{
			return this._array[num] & 15;
		}
		return (byte)(this._array[num % this._halfSize] >> 4 & 15);
	}

	// Token: 0x0600906B RID: 36971 RVA: 0x003997A4 File Offset: 0x003979A4
	public void set(int x, int y, int z, byte b)
	{
		int num = (x << this._lXPow << this._lYPow) + (y << this._lXPow) + z;
		int num2;
		if (num < this._halfSize)
		{
			num2 = (int)((this._array[num] & 240) | (b & 15));
			this._array[num] = (byte)num2;
			return;
		}
		num2 = (((int)b << 4 & 240) | (int)(this._array[num % this._halfSize] & 15));
		this._array[num % this._halfSize] = (byte)num2;
	}

	// Token: 0x0600906C RID: 36972 RVA: 0x0039982F File Offset: 0x00397A2F
	public int size()
	{
		return this._size;
	}

	// Token: 0x0600906D RID: 36973 RVA: 0x00399837 File Offset: 0x00397A37
	public int sizePacked()
	{
		return this._halfSize;
	}

	// Token: 0x0600906E RID: 36974 RVA: 0x0039983F File Offset: 0x00397A3F
	public void copyFrom(SmartArray _other)
	{
		_other._array.CopyTo(this._array, 0);
	}

	// Token: 0x0600906F RID: 36975 RVA: 0x00399853 File Offset: 0x00397A53
	public int GetUsedMem()
	{
		return this._array.Length;
	}

	// Token: 0x04006F44 RID: 28484
	public byte[] _array;

	// Token: 0x04006F45 RID: 28485
	[PublicizedFrom(EAccessModifier.Private)]
	public int _lXPow;

	// Token: 0x04006F46 RID: 28486
	[PublicizedFrom(EAccessModifier.Private)]
	public int _lYPow;

	// Token: 0x04006F47 RID: 28487
	[PublicizedFrom(EAccessModifier.Private)]
	public int _lZPow;

	// Token: 0x04006F48 RID: 28488
	public int _size;

	// Token: 0x04006F49 RID: 28489
	public int _halfSize;
}
