using System;
using System.IO;
using System.Runtime.CompilerServices;

// Token: 0x020009A2 RID: 2466
public class ChunkBlockChannel
{
	// Token: 0x06004B16 RID: 19222 RVA: 0x001DAD84 File Offset: 0x001D8F84
	public ChunkBlockChannel(long _defaultValue, int _bytesPerVal = 1)
	{
		this.defaultValue = _defaultValue;
		this.bytesPerVal = _bytesPerVal;
		this.sameValue = new byte[64 * this.bytesPerVal];
		this.fillSameValue(-1L);
		this.layers = new CBCLayer[64 * this.bytesPerVal];
	}

	// Token: 0x06004B17 RID: 19223 RVA: 0x001DADD5 File Offset: 0x001D8FD5
	[PublicizedFrom(EAccessModifier.Private)]
	public CBCLayer allocLayer()
	{
		return MemoryPools.poolCBC.AllocSync(true);
	}

	// Token: 0x06004B18 RID: 19224 RVA: 0x001DADE2 File Offset: 0x001D8FE2
	[PublicizedFrom(EAccessModifier.Private)]
	public void freeLayer(int _idx)
	{
		if (this.layers[_idx] == null)
		{
			return;
		}
		MemoryPools.poolCBC.FreeSync(this.layers[_idx]);
		this.layers[_idx] = null;
	}

	// Token: 0x06004B19 RID: 19225 RVA: 0x001DAE0A File Offset: 0x001D900A
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int calcOffset(int _x, int _y, int _z)
	{
		return _x + _z * 16 + (_y & 3) * 256;
	}

	// Token: 0x06004B1A RID: 19226 RVA: 0x001DAE1C File Offset: 0x001D901C
	[PublicizedFrom(EAccessModifier.Private)]
	public void fillSameValue(long _value = -1L)
	{
		long num = (_value == -1L) ? this.defaultValue : _value;
		for (int i = 0; i < this.bytesPerVal; i++)
		{
			byte b = (byte)(num >> i * 8);
			for (int j = 63; j >= 0; j--)
			{
				this.sameValue[j * this.bytesPerVal + i] = b;
			}
		}
	}

	// Token: 0x06004B1B RID: 19227 RVA: 0x001DAE74 File Offset: 0x001D9074
	[PublicizedFrom(EAccessModifier.Private)]
	public long getSameValue(int _idx)
	{
		long num = 0L;
		for (int i = 0; i < this.bytesPerVal; i++)
		{
			num |= (long)((long)((ulong)this.sameValue[_idx + i]) << i * 8);
		}
		return num;
	}

	// Token: 0x06004B1C RID: 19228 RVA: 0x001DAEAC File Offset: 0x001D90AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void setSameValue(int _idx, long _value)
	{
		for (int i = 0; i < this.bytesPerVal; i++)
		{
			this.sameValue[_idx + i] = (byte)_value;
			_value >>= 8;
		}
	}

	// Token: 0x06004B1D RID: 19229 RVA: 0x001DAEDC File Offset: 0x001D90DC
	[PublicizedFrom(EAccessModifier.Private)]
	public long getData(int _idx, int _offs)
	{
		long num = 0L;
		for (int i = 0; i < this.bytesPerVal; i++)
		{
			CBCLayer cbclayer = this.layers[_idx + i];
			if (cbclayer == null)
			{
				break;
			}
			num |= (long)((long)((ulong)cbclayer.data[_offs]) << i * 8);
		}
		return num;
	}

	// Token: 0x06004B1E RID: 19230 RVA: 0x001DAF20 File Offset: 0x001D9120
	[PublicizedFrom(EAccessModifier.Private)]
	public long getSetData(int _idx, int _offs, long _value)
	{
		long num = 0L;
		for (int i = 0; i < this.bytesPerVal; i++)
		{
			CBCLayer cbclayer = this.layers[_idx + i];
			if (cbclayer == null)
			{
				break;
			}
			num |= (long)((long)((ulong)cbclayer.data[_offs]) << i * 8);
			cbclayer.data[_offs] = (byte)_value;
			_value >>= 8;
		}
		return num;
	}

	// Token: 0x06004B1F RID: 19231 RVA: 0x001DAF74 File Offset: 0x001D9174
	public long GetSet(int _x, int _y, int _z, long _value)
	{
		int num = (_y >> 2) * this.bytesPerVal;
		if (this.layers[num] == null)
		{
			long num2 = this.getSameValue(num);
			if (num2 == _value)
			{
				return _value;
			}
			for (int i = 0; i < this.bytesPerVal; i++)
			{
				CBCLayer cbclayer = this.allocLayer();
				this.layers[num + i] = cbclayer;
				byte b = (byte)(num2 >> i * 8);
				for (int j = 1023; j >= 0; j--)
				{
					cbclayer.data[j] = b;
				}
			}
		}
		int offs = ChunkBlockChannel.calcOffset(_x, _y, _z);
		return this.getSetData(num, offs, _value);
	}

	// Token: 0x06004B20 RID: 19232 RVA: 0x001DB010 File Offset: 0x001D9210
	public void Set(int _x, int _y, int _z, long _value)
	{
		int num = (_y >> 2) * this.bytesPerVal;
		if (this.layers[num] == null)
		{
			long num2 = this.getSameValue(num);
			if (num2 == _value)
			{
				return;
			}
			for (int i = 0; i < this.bytesPerVal; i++)
			{
				CBCLayer cbclayer = this.allocLayer();
				this.layers[num + i] = cbclayer;
				byte b = (byte)(num2 >> i * 8);
				for (int j = 1023; j >= 0; j--)
				{
					cbclayer.data[j] = b;
				}
			}
		}
		int num3 = ChunkBlockChannel.calcOffset(_x, _y, _z);
		for (int k = 0; k < this.bytesPerVal; k++)
		{
			CBCLayer cbclayer = this.layers[num + k];
			if (cbclayer == null)
			{
				break;
			}
			cbclayer.data[num3] = (byte)_value;
			_value >>= 8;
		}
	}

	// Token: 0x06004B21 RID: 19233 RVA: 0x001DB0D4 File Offset: 0x001D92D4
	public long Get(int _x, int _y, int _z)
	{
		int num = (_y >> 2) * this.bytesPerVal;
		if (num < 0)
		{
			return 0L;
		}
		CBCLayer cbclayer = this.layers[num];
		if (cbclayer == null)
		{
			return this.getSameValue(num);
		}
		int num2 = ChunkBlockChannel.calcOffset(_x, _y, _z);
		if (this.bytesPerVal == 1)
		{
			return (long)((ulong)cbclayer.data[num2]);
		}
		return this.getData(num, num2);
	}

	// Token: 0x06004B22 RID: 19234 RVA: 0x001DB12C File Offset: 0x001D932C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte GetByte(int _x, int _y, int _z)
	{
		int num = _y >> 2;
		if (num >= 64)
		{
			return 0;
		}
		CBCLayer cbclayer = this.layers[num];
		if (cbclayer == null)
		{
			return this.sameValue[num];
		}
		int num2 = ChunkBlockChannel.calcOffset(_x, _y, _z);
		return cbclayer.data[num2];
	}

	// Token: 0x06004B23 RID: 19235 RVA: 0x001DB16C File Offset: 0x001D936C
	public void Read(BinaryReader _br, uint _version, bool _bNetworkRead)
	{
		if (_version > 34U)
		{
			for (int i = 0; i < 64; i++)
			{
				int num = i * this.bytesPerVal;
				bool flag = _br.ReadByte() == 1;
				for (int j = 0; j < this.bytesPerVal; j++)
				{
					int num2 = num + j;
					if (!flag)
					{
						if (this.layers[num2] == null)
						{
							this.layers[num2] = this.allocLayer();
						}
						_br.Read(this.layers[num2].data, 0, 1024);
					}
					else
					{
						this.sameValue[num2] = _br.ReadByte();
						this.freeLayer(num2);
					}
				}
				this.onLayerRead(num);
			}
			return;
		}
		for (int k = 0; k < 64; k++)
		{
			int num3 = k * this.bytesPerVal;
			bool flag2 = _br.ReadBoolean();
			for (int l = 0; l < this.bytesPerVal; l++)
			{
				if (!flag2)
				{
					if (this.layers[num3 + l] == null)
					{
						this.layers[num3 + l] = this.allocLayer();
					}
					_br.Read(this.layers[num3 + l].data, 0, 1024);
				}
				else
				{
					this.sameValue[num3 + l] = _br.ReadByte();
					this.freeLayer(num3 + l);
				}
			}
			this.onLayerRead(num3);
		}
	}

	// Token: 0x06004B24 RID: 19236 RVA: 0x001DB2C0 File Offset: 0x001D94C0
	public void Write(BinaryWriter _bw, bool _bNetworkWrite, byte[] temp)
	{
		int num = 0;
		for (int i = 0; i < 64; i++)
		{
			int num2 = i * this.bytesPerVal;
			bool flag = this.layers[num2] == null;
			temp[num++] = (flag ? 1 : 0);
			if (num == temp.Length)
			{
				_bw.Write(temp, 0, num);
				num = 0;
			}
			for (int j = 0; j < this.bytesPerVal; j++)
			{
				if (!flag)
				{
					if (num > 0)
					{
						_bw.Write(temp, 0, num);
						num = 0;
					}
					_bw.Write(this.layers[num2 + j].data, 0, 1024);
				}
				else
				{
					temp[num++] = this.sameValue[num2 + j];
					if (num == temp.Length)
					{
						_bw.Write(temp, 0, num);
						num = 0;
					}
				}
			}
		}
		if (num > 0)
		{
			_bw.Write(temp, 0, num);
		}
	}

	// Token: 0x06004B25 RID: 19237 RVA: 0x001DB38E File Offset: 0x001D958E
	[PublicizedFrom(EAccessModifier.Private)]
	public void onLayerRead(int _idx)
	{
		if (this.layers[_idx] == null)
		{
			return;
		}
		this.checkSameValue(_idx);
	}

	// Token: 0x06004B26 RID: 19238 RVA: 0x001DB3A4 File Offset: 0x001D95A4
	public void CheckSameValue()
	{
		for (int i = 63; i >= 0; i--)
		{
			this.checkSameValue(i * this.bytesPerVal);
		}
	}

	// Token: 0x06004B27 RID: 19239 RVA: 0x001DB3CC File Offset: 0x001D95CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void checkSameValue(int _idx)
	{
		if (this.layers[_idx] == null)
		{
			return;
		}
		long data = this.getData(_idx, 0);
		for (int i = 1; i < 1024; i++)
		{
			if (data != this.getData(_idx, i))
			{
				return;
			}
		}
		this.setSameValue(_idx, data);
		for (int j = 0; j < this.bytesPerVal; j++)
		{
			this.freeLayer(_idx + j);
		}
	}

	// Token: 0x06004B28 RID: 19240 RVA: 0x001DB42C File Offset: 0x001D962C
	public bool HasSameValue(int _y)
	{
		int num = (_y >> 2) * this.bytesPerVal;
		return this.layers[num] == null;
	}

	// Token: 0x06004B29 RID: 19241 RVA: 0x001DB450 File Offset: 0x001D9650
	public long GetSameValue(int _y)
	{
		int idx = (_y >> 2) * this.bytesPerVal;
		return this.getSameValue(idx);
	}

	// Token: 0x06004B2A RID: 19242 RVA: 0x001DB470 File Offset: 0x001D9670
	public bool IsDefault()
	{
		for (int i = 63; i >= 0; i--)
		{
			if (!this.IsDefaultLayer(i))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06004B2B RID: 19243 RVA: 0x001DB498 File Offset: 0x001D9698
	public bool IsDefault(int _y)
	{
		int blockLayer = _y >> 2;
		return this.IsDefaultLayer(blockLayer);
	}

	// Token: 0x06004B2C RID: 19244 RVA: 0x001DB4B0 File Offset: 0x001D96B0
	public bool IsDefaultLayer(int _blockLayer)
	{
		return this.isDefault(_blockLayer * this.bytesPerVal);
	}

	// Token: 0x06004B2D RID: 19245 RVA: 0x001DB4C0 File Offset: 0x001D96C0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDefault(int _idx)
	{
		this.checkSameValue(_idx);
		return this.layers[_idx] == null && this.getSameValue(_idx) == this.defaultValue;
	}

	// Token: 0x06004B2E RID: 19246 RVA: 0x001DB4E4 File Offset: 0x001D96E4
	public int GetUsedMem()
	{
		int num = 0;
		for (int i = this.layers.Length - 1; i >= 0; i--)
		{
			if (this.layers[i] != null)
			{
				num += 1024;
			}
		}
		num += this.sameValue.Length;
		return num + this.layers.Length * 4;
	}

	// Token: 0x06004B2F RID: 19247 RVA: 0x001DB533 File Offset: 0x001D9733
	public void FreeLayers()
	{
		MemoryPools.poolCBC.FreeSync(this.layers);
		this.fillSameValue(-1L);
	}

	// Token: 0x06004B30 RID: 19248 RVA: 0x001DB550 File Offset: 0x001D9750
	public void Clear(long _defaultValue = -1L)
	{
		for (int i = 0; i < this.layers.Length; i++)
		{
			this.freeLayer(i);
		}
		this.fillSameValue(_defaultValue);
	}

	// Token: 0x06004B31 RID: 19249 RVA: 0x001DB580 File Offset: 0x001D9780
	public void ClearHalf(bool _bClearUpperHalf)
	{
		byte b = _bClearUpperHalf ? 15 : 240;
		for (int i = 0; i < 64; i++)
		{
			CBCLayer cbclayer = this.layers[i];
			if (cbclayer != null)
			{
				for (int j = 0; j < 1024; j++)
				{
					byte[] data = cbclayer.data;
					int num = j;
					data[num] &= b;
				}
			}
			else
			{
				byte[] array = this.sameValue;
				int num2 = i;
				array[num2] &= b;
			}
		}
	}

	// Token: 0x06004B32 RID: 19250 RVA: 0x001DB5EC File Offset: 0x001D97EC
	public void SetHalf(bool _bSetUpperHalf, byte _v)
	{
		byte b = _bSetUpperHalf ? 15 : 240;
		for (int i = 0; i < 64; i++)
		{
			CBCLayer cbclayer = this.layers[i];
			if (cbclayer != null)
			{
				for (int j = 0; j < 1024; j++)
				{
					byte[] data = cbclayer.data;
					int num = j;
					data[num] &= b;
					byte[] data2 = cbclayer.data;
					int num2 = j;
					data2[num2] |= _v;
				}
			}
			else
			{
				byte[] array = this.sameValue;
				int num3 = i;
				array[num3] &= b;
				byte[] array2 = this.sameValue;
				int num4 = i;
				array2[num4] |= _v;
			}
		}
	}

	// Token: 0x06004B33 RID: 19251 RVA: 0x001DB67C File Offset: 0x001D987C
	public void CopyFrom(ChunkBlockChannel _other)
	{
		for (int i = 0; i < _other.layers.Length; i++)
		{
			if (_other.layers[i] != null)
			{
				if (this.layers[i] == null)
				{
					this.layers[i] = this.allocLayer();
				}
				this.layers[i].CopyFrom(_other.layers[i]);
			}
			else
			{
				this.freeLayer(i);
			}
		}
		for (int j = 0; j < _other.sameValue.Length; j++)
		{
			this.sameValue[j] = _other.sameValue[j];
		}
	}

	// Token: 0x06004B34 RID: 19252 RVA: 0x001DB700 File Offset: 0x001D9900
	public void Convert(SmartArray _sa, int _shiftBits)
	{
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				for (int k = 0; k < 16; k++)
				{
					byte b = _sa.get(j, i, k);
					byte b2 = (byte)this.Get(j, i, k);
					b2 |= (byte)(b << _shiftBits);
					this.Set(j, i, k, (long)((ulong)b2));
				}
			}
		}
		this.CheckSameValue();
	}

	// Token: 0x06004B35 RID: 19253 RVA: 0x001DB76C File Offset: 0x001D996C
	public void Convert(ChunkBlockLayerLegacy[] m_BlockLayers)
	{
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				for (int k = 0; k < 16; k++)
				{
					byte b;
					if (m_BlockLayers[i] != null)
					{
						b = m_BlockLayers[i].GetStabilityAt(j, k);
					}
					else
					{
						b = 0;
					}
					this.Set(j, i, k, (long)((ulong)b));
				}
			}
		}
		this.CheckSameValue();
	}

	// Token: 0x0400399A RID: 14746
	public const int cElementsPerLayer = 1024;

	// Token: 0x0400399B RID: 14747
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly byte[] sameValue;

	// Token: 0x0400399C RID: 14748
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CBCLayer[] layers;

	// Token: 0x0400399D RID: 14749
	[PublicizedFrom(EAccessModifier.Private)]
	public long defaultValue;

	// Token: 0x0400399E RID: 14750
	[PublicizedFrom(EAccessModifier.Private)]
	public int bytesPerVal;
}
