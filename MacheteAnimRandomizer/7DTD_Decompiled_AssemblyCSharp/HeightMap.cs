using System;
using UnityEngine;

// Token: 0x02000ACE RID: 2766
public sealed class HeightMap : IDisposable
{
	// Token: 0x06005523 RID: 21795 RVA: 0x0022C4BC File Offset: 0x0022A6BC
	public HeightMap(int _w, int _h, float _maxHeight, IBackedArray<ushort> _data, int _targetSize = 0)
	{
		this.w = _w;
		this.h = _h;
		this.scaleShift = ((_targetSize != 0) ? ((int)Mathf.Log((float)(_targetSize / _w), 2f)) : 0);
		this.scalePixs = _targetSize / _w;
		this.maxHeight = _maxHeight;
		this.data = BackedArrays.CreateSingleView<ushort>(_data, BackedArrayHandleMode.ReadOnly, 16 * _w);
	}

	// Token: 0x06005524 RID: 21796 RVA: 0x0022C51D File Offset: 0x0022A71D
	public void Dispose()
	{
		IBackedArrayView<ushort> backedArrayView = this.data;
		if (backedArrayView != null)
		{
			backedArrayView.Dispose();
		}
		this.data = null;
	}

	// Token: 0x06005525 RID: 21797 RVA: 0x0022C538 File Offset: 0x0022A738
	public float GetAt(int _x, int _z)
	{
		ushort num;
		if (this.scaleShift == 0 && _x + _z * this.w < this.data.Length)
		{
			num = this.data[_x + _z * this.w];
		}
		else
		{
			num = this.getInterpolatedHeight(_x, _z);
		}
		return (float)num * (this.maxHeight / 65535f);
	}

	// Token: 0x06005526 RID: 21798 RVA: 0x0022C594 File Offset: 0x0022A794
	public ushort getInterpolatedHeight(int xf, int zf)
	{
		object obj = (xf >= 0) ? (xf >> this.scaleShift) : (xf - this.scalePixs + 1 >> this.scaleShift);
		int num = (zf >= 0) ? (zf >> this.scaleShift) : (zf - this.scalePixs + 1 >> this.scaleShift);
		object obj2 = obj;
		int num2 = obj2 + num * this.w;
		ushort num3 = this.data[num2 + this.w & this.data.Length - 1];
		ushort num4 = this.data[num2 & this.data.Length - 1];
		ushort num5 = this.data[num2 + 1 & this.data.Length - 1];
		ushort num6 = this.data[num2 + 1 + this.w & this.data.Length - 1];
		int num7 = obj2 << (this.scaleShift & 31);
		int num8 = num << this.scaleShift;
		float num9 = 1f - (float)(zf - num8) / (float)this.scalePixs;
		float num10 = (float)(xf - num7) / (float)this.scalePixs;
		return (ushort)((1f - num9) * ((1f - num10) * (float)num3 + num10 * (float)num6) + num9 * ((1f - num10) * (float)num4 + num10 * (float)num5));
	}

	// Token: 0x06005527 RID: 21799 RVA: 0x0022C6E8 File Offset: 0x0022A8E8
	public float GetAt(int _offs)
	{
		ushort num;
		if (this.scaleShift == 0)
		{
			num = this.data[_offs];
		}
		else
		{
			int zf = _offs / this.w;
			int xf = _offs % this.w;
			num = this.getInterpolatedHeight(xf, zf);
		}
		return (float)num * this.maxHeight / 65535f;
	}

	// Token: 0x06005528 RID: 21800 RVA: 0x0022C736 File Offset: 0x0022A936
	public int CalcOffset(int _x, int _z)
	{
		_x >>= this.scaleShift;
		_z >>= this.scaleShift;
		return _x + _z * this.w;
	}

	// Token: 0x06005529 RID: 21801 RVA: 0x0022C75C File Offset: 0x0022A95C
	public int GetWidth()
	{
		return this.w;
	}

	// Token: 0x0600552A RID: 21802 RVA: 0x0022C764 File Offset: 0x0022A964
	public int GetHeight()
	{
		return this.h;
	}

	// Token: 0x0600552B RID: 21803 RVA: 0x0022C76C File Offset: 0x0022A96C
	public int GetScaleSteps()
	{
		return this.scalePixs;
	}

	// Token: 0x0600552C RID: 21804 RVA: 0x0022C774 File Offset: 0x0022A974
	public int GetScaleShift()
	{
		return this.scaleShift;
	}

	// Token: 0x040041EE RID: 16878
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int w;

	// Token: 0x040041EF RID: 16879
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int h;

	// Token: 0x040041F0 RID: 16880
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int scaleShift;

	// Token: 0x040041F1 RID: 16881
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int scalePixs;

	// Token: 0x040041F2 RID: 16882
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly float maxHeight;

	// Token: 0x040041F3 RID: 16883
	[PublicizedFrom(EAccessModifier.Private)]
	public IBackedArrayView<ushort> data;
}
