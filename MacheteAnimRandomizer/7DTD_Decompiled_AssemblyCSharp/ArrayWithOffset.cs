using System;

// Token: 0x02001116 RID: 4374
public class ArrayWithOffset<T>
{
	// Token: 0x06008986 RID: 35206 RVA: 0x0000A7E3 File Offset: 0x000089E3
	public ArrayWithOffset()
	{
	}

	// Token: 0x06008987 RID: 35207 RVA: 0x0037C1FA File Offset: 0x0037A3FA
	public ArrayWithOffset(int _dimX, int _dimY) : this(_dimX, _dimY, 0, 0)
	{
	}

	// Token: 0x06008988 RID: 35208 RVA: 0x0037C208 File Offset: 0x0037A408
	public ArrayWithOffset(int _dimX, int _dimY, int _addXOffs, int _addYOffs)
	{
		this.DimX = _dimX;
		this.DimY = _dimY;
		this.data = new T[_dimX, _dimY];
		this.sizeXHalf = _dimX / 2;
		this.sizeYHalf = _dimY / 2;
		this.MinPos = new Vector2i(-this.sizeXHalf - _addXOffs, -this.sizeYHalf - _addYOffs);
		this.MaxPos = new Vector2i(this.sizeXHalf - _addXOffs - 1, this.sizeYHalf - _addYOffs - 1);
		this.addXOffs = _addXOffs + this.sizeXHalf;
		this.addYOffs = _addYOffs + this.sizeXHalf;
	}

	// Token: 0x17000E4F RID: 3663
	public virtual T this[int _x, int _y]
	{
		get
		{
			return this.data[_x + this.addXOffs, _y + this.addYOffs];
		}
		set
		{
			this.data[_x + this.addXOffs, _y + this.addYOffs] = value;
		}
	}

	// Token: 0x0600898B RID: 35211 RVA: 0x0037C2DE File Offset: 0x0037A4DE
	public bool Contains(int _x, int _y)
	{
		return _x >= this.MinPos.x && _y >= this.MinPos.y && _x < this.MaxPos.x && _y < this.MaxPos.y;
	}

	// Token: 0x0600898C RID: 35212 RVA: 0x0037C31C File Offset: 0x0037A51C
	public void CopyInto(ArrayWithOffset<T> _other)
	{
		for (int i = 0; i < this.data.GetLength(0); i++)
		{
			for (int j = 0; j < this.data.GetLength(1); j++)
			{
				_other.data[i, j] = this.data[i, j];
			}
		}
	}

	// Token: 0x04006BD6 RID: 27606
	[PublicizedFrom(EAccessModifier.Private)]
	public T[,] data;

	// Token: 0x04006BD7 RID: 27607
	[PublicizedFrom(EAccessModifier.Private)]
	public int sizeXHalf;

	// Token: 0x04006BD8 RID: 27608
	[PublicizedFrom(EAccessModifier.Private)]
	public int sizeYHalf;

	// Token: 0x04006BD9 RID: 27609
	[PublicizedFrom(EAccessModifier.Private)]
	public int addXOffs;

	// Token: 0x04006BDA RID: 27610
	[PublicizedFrom(EAccessModifier.Private)]
	public int addYOffs;

	// Token: 0x04006BDB RID: 27611
	public int DimX;

	// Token: 0x04006BDC RID: 27612
	public int DimY;

	// Token: 0x04006BDD RID: 27613
	public Vector2i MinPos;

	// Token: 0x04006BDE RID: 27614
	public Vector2i MaxPos;
}
