using System;
using System.Collections.Generic;

// Token: 0x02000A5B RID: 2651
public class TileArea<T> : ITileArea<T> where T : class
{
	// Token: 0x060050BD RID: 20669 RVA: 0x00201614 File Offset: 0x001FF814
	public TileArea(TileAreaConfig _config, T[,] _data = null)
	{
		this.config = _config;
		if (_data != null)
		{
			for (int i = 0; i < _data.GetLength(0); i++)
			{
				for (int j = 0; j < _data.GetLength(1); j++)
				{
					int tileX = i + this.config.tileStart.x;
					int tileZ = j + this.config.tileStart.y;
					uint key = TileAreaUtils.MakeKey(tileX, tileZ);
					this.Data[key] = _data[i, j];
				}
			}
		}
	}

	// Token: 0x17000832 RID: 2098
	// (get) Token: 0x060050BE RID: 20670 RVA: 0x002016A0 File Offset: 0x001FF8A0
	public TileAreaConfig Config
	{
		get
		{
			return this.config;
		}
	}

	// Token: 0x060050BF RID: 20671 RVA: 0x002016A8 File Offset: 0x001FF8A8
	public void Remove(uint _key)
	{
		this.Data.Remove(_key);
	}

	// Token: 0x17000833 RID: 2099
	public T this[int _tileX, int _tileZ]
	{
		get
		{
			this.config.checkCoordinates(ref _tileX, ref _tileZ);
			uint key = TileAreaUtils.MakeKey(_tileX, _tileZ);
			T result;
			if (!this.Data.TryGetValue(key, out result))
			{
				return default(T);
			}
			return result;
		}
		set
		{
			this.config.checkCoordinates(ref _tileX, ref _tileZ);
			uint key = TileAreaUtils.MakeKey(_tileX, _tileZ);
			this.Data[key] = value;
		}
	}

	// Token: 0x17000834 RID: 2100
	public T this[uint _key]
	{
		get
		{
			T result;
			if (!this.Data.TryGetValue(_key, out result))
			{
				return default(T);
			}
			return result;
		}
		set
		{
			this.Data[_key] = value;
		}
	}

	// Token: 0x04003DDA RID: 15834
	public TileAreaConfig config;

	// Token: 0x04003DDB RID: 15835
	public Dictionary<uint, T> Data = new Dictionary<uint, T>();
}
