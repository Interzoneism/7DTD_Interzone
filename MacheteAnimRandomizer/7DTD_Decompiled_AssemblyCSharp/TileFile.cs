using System;
using System.Runtime.CompilerServices;

// Token: 0x02000A5E RID: 2654
public class TileFile<[IsUnmanaged] T> : IDisposable where T : struct, ValueType
{
	// Token: 0x060050CF RID: 20687 RVA: 0x002019A5 File Offset: 0x001FFBA5
	public TileFile(FileBackedArray<T> _fileBackedArray, int _tileWidth, int _tileCountWidth, int _tileCountHeight)
	{
		this.fba = _fileBackedArray;
		this.tileWidth = _tileWidth;
		this.tileCountWidth = _tileCountWidth;
		this.tileCountHeight = _tileCountHeight;
		this.dataLength = this.tileWidth * this.tileWidth;
	}

	// Token: 0x060050D0 RID: 20688 RVA: 0x002019DD File Offset: 0x001FFBDD
	public bool IsInDatabase(int _tileX, int _tileZ)
	{
		return _tileX >= 0 && _tileX < this.tileCountWidth && _tileZ >= 0 && _tileZ < this.tileCountHeight;
	}

	// Token: 0x060050D1 RID: 20689 RVA: 0x002019FC File Offset: 0x001FFBFC
	public unsafe void LoadTile(int _tileX, int _tileZ, ref T[,] _tile)
	{
		if (_tile == null)
		{
			_tile = new T[this.tileWidth, this.tileWidth];
		}
		int start = _tileZ * this.tileCountWidth * this.dataLength + _tileX * this.dataLength;
		ReadOnlySpan<T> readOnlySpan2;
		using (this.fba.GetReadOnlySpan(start, this.dataLength, out readOnlySpan2))
		{
			T[,] array;
			T* pointer;
			if ((array = _tile) == null || array.Length == 0)
			{
				pointer = null;
			}
			else
			{
				pointer = &array[0, 0];
			}
			Span<T> destination = new Span<T>((void*)pointer, this.dataLength);
			readOnlySpan2.CopyTo(destination);
			array = null;
		}
	}

	// Token: 0x060050D2 RID: 20690 RVA: 0x00201AA4 File Offset: 0x001FFCA4
	public void Dispose()
	{
		this.fba.Dispose();
	}

	// Token: 0x04003DE2 RID: 15842
	[PublicizedFrom(EAccessModifier.Private)]
	public FileBackedArray<T> fba;

	// Token: 0x04003DE3 RID: 15843
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int tileWidth;

	// Token: 0x04003DE4 RID: 15844
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int tileCountWidth;

	// Token: 0x04003DE5 RID: 15845
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int tileCountHeight;

	// Token: 0x04003DE6 RID: 15846
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int dataLength;
}
