using System;

// Token: 0x02000A5C RID: 2652
public class TileAreaSimple<T> : ITileArea<T> where T : class
{
	// Token: 0x060050C4 RID: 20676 RVA: 0x00201763 File Offset: 0x001FF963
	public TileAreaSimple(T[,] _data = null)
	{
		this.data = _data;
	}

	// Token: 0x17000835 RID: 2101
	public T this[uint _key]
	{
		get
		{
			return this.data[0, 0];
		}
	}

	// Token: 0x17000836 RID: 2102
	public T this[int _tileX, int _tileZ]
	{
		get
		{
			return this.data[0, 0];
		}
	}

	// Token: 0x17000837 RID: 2103
	// (get) Token: 0x060050C7 RID: 20679 RVA: 0x00201784 File Offset: 0x001FF984
	public TileAreaConfig Config
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	// Token: 0x04003DDC RID: 15836
	[PublicizedFrom(EAccessModifier.Private)]
	public T[,] data;
}
