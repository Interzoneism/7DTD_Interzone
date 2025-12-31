using System;
using System.Runtime.CompilerServices;

// Token: 0x020011A3 RID: 4515
public class GridCompressedData<[IsUnmanaged] T> where T : struct, ValueType, IEquatable<T>
{
	// Token: 0x06008D16 RID: 36118 RVA: 0x0038AF84 File Offset: 0x00389184
	public GridCompressedData(int _width, int _height, int _cellSizeX, int _cellSizeY)
	{
		if (_width % _cellSizeX != 0)
		{
			throw new Exception(string.Format("Cell width must be a multiple of data width. Cell width: {0}, width: {1}", _cellSizeX, _width));
		}
		if (_height % _cellSizeY != 0)
		{
			throw new Exception(string.Format("Cell height must be a multiple of data height. Cell height: {0}, height: {1}", _cellSizeX, _width));
		}
		this.width = _width;
		this.height = _height;
		this.cellSizeX = _cellSizeX;
		this.cellSizeY = _cellSizeY;
		this.widthCells = _width / this.cellSizeX;
		this.heightCells = _height / this.cellSizeY;
		int num = this.widthCells * this.heightCells;
		this.cells = new T[num][];
		this.sameValues = new T[num];
	}

	// Token: 0x06008D17 RID: 36119 RVA: 0x0038B03C File Offset: 0x0038923C
	public void SetValue(int _x, int _y, T value)
	{
		int num = _x / this.cellSizeX + _y / this.cellSizeY * this.widthCells;
		T[] array = this.cells[num];
		if (array == null)
		{
			T t = this.sameValues[num];
			if (value.Equals(t))
			{
				return;
			}
			array = (this.cells[num] = new T[this.cellSizeX * this.cellSizeY]);
			Array.Fill<T>(array, t);
		}
		int num2 = _x % this.cellSizeX + _y % this.cellSizeY * this.cellSizeX;
		array[num2] = value;
	}

	// Token: 0x06008D18 RID: 36120 RVA: 0x0038B0D4 File Offset: 0x003892D4
	public void SetValue(int _cellIndex, int _cellX, int _cellY, T _value)
	{
		T[] array = this.cells[_cellIndex];
		if (array == null)
		{
			T value = this.sameValues[_cellIndex];
			if (_value.Equals(this.sameValues[_cellIndex]))
			{
				return;
			}
			array = (this.cells[_cellIndex] = new T[this.cellSizeX * this.cellSizeY]);
			Array.Fill<T>(array, value);
		}
		int num = _cellX + _cellY * this.cellSizeX;
		array[num] = _value;
	}

	// Token: 0x06008D19 RID: 36121 RVA: 0x0038B14F File Offset: 0x0038934F
	public void SetSameValue(int _cellIndex, T _value)
	{
		this.sameValues[_cellIndex] = _value;
		this.cells[_cellIndex] = null;
	}

	// Token: 0x06008D1A RID: 36122 RVA: 0x0038B168 File Offset: 0x00389368
	public void Fill(T _value)
	{
		for (int i = 0; i < this.cells.Length; i++)
		{
			this.sameValues[i] = _value;
			this.cells[i] = null;
		}
	}

	// Token: 0x06008D1B RID: 36123 RVA: 0x0038B1A0 File Offset: 0x003893A0
	public T GetValue(int _x, int _y)
	{
		int num = _x / this.cellSizeX + _y / this.cellSizeY * this.widthCells;
		T[] array = this.cells[num];
		if (array == null)
		{
			return this.sameValues[num];
		}
		int num2 = _x % this.cellSizeX + _y % this.cellSizeY * this.cellSizeX;
		return array[num2];
	}

	// Token: 0x06008D1C RID: 36124 RVA: 0x0038B1FF File Offset: 0x003893FF
	public T GetValue(int _offs)
	{
		return this.GetValue(_offs % this.width, _offs / this.width);
	}

	// Token: 0x06008D1D RID: 36125 RVA: 0x0038B218 File Offset: 0x00389418
	public void CheckSameValue(int _cellIndex)
	{
		T[] array = this.cells[_cellIndex];
		if (array != null && GridCompressedData<T>.CheckSameValue(this.cells[_cellIndex]))
		{
			this.sameValues[_cellIndex] = array[0];
			this.cells[_cellIndex] = null;
		}
	}

	// Token: 0x06008D1E RID: 36126 RVA: 0x0038B25C File Offset: 0x0038945C
	public void CheckSameValues()
	{
		for (int i = 0; i < this.sameValues.Length; i++)
		{
			this.CheckSameValue(i);
		}
	}

	// Token: 0x06008D1F RID: 36127 RVA: 0x0038B284 File Offset: 0x00389484
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool CheckSameValue(T[] _cell)
	{
		T t = _cell[0];
		for (int i = 1; i < _cell.Length; i++)
		{
			if (!t.Equals(_cell[i]))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06008D20 RID: 36128 RVA: 0x0038B2C0 File Offset: 0x003894C0
	public T[] ToArray()
	{
		T[] array = new T[this.widthCells * this.cellSizeX * this.heightCells * this.cellSizeY];
		for (int i = 0; i < this.cells.Length; i++)
		{
			int num = i % this.widthCells;
			int num2 = i / this.widthCells;
			int num3 = num * this.cellSizeX + num2 * this.cellSizeY * this.width;
			T[] array2 = this.cells[i];
			if (array2 == null)
			{
				T value = this.sameValues[i];
				for (int j = 0; j < this.cellSizeY; j++)
				{
					Array.Fill<T>(array, value, num3, this.cellSizeX);
					num3 += this.width;
				}
			}
			else
			{
				int num4 = 0;
				for (int k = 0; k < this.cellSizeY; k++)
				{
					Array.Copy(array2, num4, array, num3, this.cellSizeX);
					num3 += this.width;
					num4 += this.cellSizeX;
				}
			}
		}
		return array;
	}

	// Token: 0x06008D21 RID: 36129 RVA: 0x0038B3BC File Offset: 0x003895BC
	public void FromArray(T[] _pixs)
	{
		if (_pixs.Length != this.width * this.height)
		{
			throw new Exception(string.Format("Source array does not contain enough data. Expected length: {0}, Actual length: {1}", this.width * this.height, _pixs.Length));
		}
		int num = 0;
		for (int i = 0; i < this.heightCells; i++)
		{
			for (int j = 0; j < this.widthCells; j++)
			{
				int num2 = i * this.cellSizeY;
				int num3 = j * this.cellSizeX;
				T value = _pixs[num3 + num2 * this.width];
				this.SetSameValue(num, value);
				for (int k = 0; k < this.cellSizeY; k++)
				{
					for (int l = 0; l < this.cellSizeX; l++)
					{
						int num4 = num3 + l + (num2 + k) * this.width;
						this.SetValue(num, l, k, _pixs[num4]);
					}
				}
				num++;
			}
		}
	}

	// Token: 0x06008D22 RID: 36130 RVA: 0x0038B4B8 File Offset: 0x003896B8
	public int EstimateOwnedBytes()
	{
		return 0 + MemoryTracker.GetSize<T>(this.sameValues) + MemoryTracker.GetSize<T>(this.cells);
	}

	// Token: 0x04006D9F RID: 28063
	[PublicizedFrom(EAccessModifier.Private)]
	public T[][] cells;

	// Token: 0x04006DA0 RID: 28064
	[PublicizedFrom(EAccessModifier.Private)]
	public T[] sameValues;

	// Token: 0x04006DA1 RID: 28065
	public int cellSizeX;

	// Token: 0x04006DA2 RID: 28066
	public int cellSizeY;

	// Token: 0x04006DA3 RID: 28067
	public int widthCells;

	// Token: 0x04006DA4 RID: 28068
	public int heightCells;

	// Token: 0x04006DA5 RID: 28069
	public int width;

	// Token: 0x04006DA6 RID: 28070
	public int height;
}
