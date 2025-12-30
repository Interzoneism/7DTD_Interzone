using System;
using System.Runtime.CompilerServices;

// Token: 0x02001258 RID: 4696
public class WorldGridCompressedData<[IsUnmanaged] T> where T : struct, ValueType, IEquatable<T>
{
	// Token: 0x0600933C RID: 37692 RVA: 0x003A9AC3 File Offset: 0x003A7CC3
	public WorldGridCompressedData(T[] _colors, int _dimX, int _dimY, int _gridSizeX, int _gridSizeY) : this(_colors, _dimX, _dimY, _gridSizeX, _gridSizeY, 0, 0)
	{
	}

	// Token: 0x0600933D RID: 37693 RVA: 0x003A9AD4 File Offset: 0x003A7CD4
	public WorldGridCompressedData(T[] _colors, int _dimX, int _dimY, int _gridSizeX, int _gridSizeY, int _addXOffs, int _addYOffs)
	{
		this.colors = new GridCompressedData<T>(_dimX, _dimY, _gridSizeX, _gridSizeY);
		this.colors.FromArray(_colors);
		this.DimX = _dimX;
		this.DimY = _dimY;
		this.sizeXHalf = _dimX / 2;
		this.sizeYHalf = _dimY / 2;
		this.addXOffs = _addXOffs;
		this.addYOffs = _addYOffs;
		this.MinPos = new Vector2i(-this.sizeXHalf - this.addXOffs, -this.sizeYHalf - this.addYOffs);
		this.MaxPos = new Vector2i(this.sizeXHalf - this.addXOffs - 1, this.sizeYHalf - this.addYOffs - 1);
	}

	// Token: 0x0600933E RID: 37694 RVA: 0x003A9B84 File Offset: 0x003A7D84
	public WorldGridCompressedData(GridCompressedData<T> _data) : this(_data, 0, 0)
	{
	}

	// Token: 0x0600933F RID: 37695 RVA: 0x003A9B90 File Offset: 0x003A7D90
	public WorldGridCompressedData(GridCompressedData<T> _data, int _addXOffs, int _addYOffs)
	{
		this.colors = _data;
		this.DimX = _data.width;
		this.DimY = _data.height;
		this.sizeXHalf = this.DimX / 2;
		this.sizeYHalf = this.DimY / 2;
		this.addXOffs = _addXOffs;
		this.addYOffs = _addYOffs;
		this.MinPos = new Vector2i(-this.sizeXHalf - this.addXOffs, -this.sizeYHalf - this.addYOffs);
		this.MaxPos = new Vector2i(this.sizeXHalf - this.addXOffs - 1, this.sizeYHalf - this.addYOffs - 1);
	}

	// Token: 0x06009340 RID: 37696 RVA: 0x003A9C3C File Offset: 0x003A7E3C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T GetData(int _x, int _y)
	{
		return this.colors.GetValue(_x + this.addXOffs + this.sizeXHalf, _y + this.addYOffs + this.sizeYHalf);
	}

	// Token: 0x06009341 RID: 37697 RVA: 0x003A9C67 File Offset: 0x003A7E67
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T GetData(int _offs)
	{
		return this.colors.GetValue(_offs);
	}

	// Token: 0x06009342 RID: 37698 RVA: 0x003A9C75 File Offset: 0x003A7E75
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(int _x, int _y)
	{
		return _x >= this.MinPos.x && _y >= this.MinPos.y && _x <= this.MaxPos.x && _y <= this.MaxPos.y;
	}

	// Token: 0x04007088 RID: 28808
	public GridCompressedData<T> colors;

	// Token: 0x04007089 RID: 28809
	[PublicizedFrom(EAccessModifier.Private)]
	public int sizeXHalf;

	// Token: 0x0400708A RID: 28810
	[PublicizedFrom(EAccessModifier.Private)]
	public int sizeYHalf;

	// Token: 0x0400708B RID: 28811
	[PublicizedFrom(EAccessModifier.Private)]
	public int addXOffs;

	// Token: 0x0400708C RID: 28812
	[PublicizedFrom(EAccessModifier.Private)]
	public int addYOffs;

	// Token: 0x0400708D RID: 28813
	public int DimX;

	// Token: 0x0400708E RID: 28814
	public int DimY;

	// Token: 0x0400708F RID: 28815
	public Vector2i MinPos;

	// Token: 0x04007090 RID: 28816
	public Vector2i MaxPos;
}
