using System;
using System.IO;

// Token: 0x020009A6 RID: 2470
public class ChunkBlockLayerLegacy : IMemoryPoolableObject
{
	// Token: 0x06004B5B RID: 19291 RVA: 0x001DC849 File Offset: 0x001DAA49
	public ChunkBlockLayerLegacy()
	{
		this.wPow = 4;
		this.hPow = 4;
		this.m_Lower16Bits = new ushort[256];
		this.m_Stability = new SmartArray(this.wPow, 0, this.hPow);
	}

	// Token: 0x06004B5C RID: 19292 RVA: 0x001DC888 File Offset: 0x001DAA88
	public BlockValue GetAt(int _x, int _y, int _z)
	{
		return new BlockValue((uint)((this.m_Upper16Bits != null) ? ((int)this.m_Upper16Bits[_x + (_z << this.wPow)] << 16 | (int)this.m_Lower16Bits[_x + (_z << this.wPow)]) : ((int)this.m_Lower16Bits[_x + (_z << this.wPow)])));
	}

	// Token: 0x06004B5D RID: 19293 RVA: 0x001DC8E4 File Offset: 0x001DAAE4
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue getAt(int _offs)
	{
		return new BlockValue((uint)((this.m_Upper16Bits != null) ? ((int)this.m_Upper16Bits[_offs] << 16 | (int)this.m_Lower16Bits[_offs]) : ((int)this.m_Lower16Bits[_offs])));
	}

	// Token: 0x06004B5E RID: 19294 RVA: 0x001DC914 File Offset: 0x001DAB14
	public void SetAt(int _x, int _y, int _z, uint _fullBlock)
	{
		int num = _x + (_z << this.wPow);
		uint typeMasked = BlockValue.GetTypeMasked((uint)this.m_Lower16Bits[num]);
		this.m_Lower16Bits[num] = (ushort)(_fullBlock & 65535U);
		if ((_fullBlock & 4294901760U) != 0U)
		{
			if (this.m_Upper16Bits == null)
			{
				this.m_Upper16Bits = new ushort[256];
			}
			this.m_Upper16Bits[num] = (ushort)(_fullBlock >> 16 & 65535U);
		}
		else if (this.m_Upper16Bits != null)
		{
			this.m_Upper16Bits[num] = 0;
		}
		if (!Block.BlocksLoaded)
		{
			return;
		}
		uint typeMasked2 = BlockValue.GetTypeMasked(_fullBlock);
		Block block = Block.list[(int)typeMasked];
		Block block2 = Block.list[(int)typeMasked2];
		if (typeMasked == 0U && typeMasked2 != 0U)
		{
			this.blockRefCount++;
			if (block2 != null && block2.IsRandomlyTick)
			{
				this.tickRefCount++;
			}
		}
		else if (typeMasked != 0U && typeMasked2 == 0U)
		{
			this.blockRefCount--;
			if (block != null && block.IsRandomlyTick)
			{
				this.tickRefCount--;
			}
		}
		else if (block != null && block.IsRandomlyTick && block2 != null && !block2.IsRandomlyTick)
		{
			this.tickRefCount--;
		}
		else if (block != null && !block.IsRandomlyTick && block2 != null && block2.IsRandomlyTick)
		{
			this.tickRefCount++;
		}
		if (this.bOnlyTerrain && !block2.shape.IsTerrain())
		{
			this.bOnlyTerrain = false;
		}
	}

	// Token: 0x06004B5F RID: 19295 RVA: 0x001DCA89 File Offset: 0x001DAC89
	public byte GetStabilityAt(int _x, int _y)
	{
		return this.m_Stability.get(_x, 0, _y);
	}

	// Token: 0x06004B60 RID: 19296 RVA: 0x001DCA99 File Offset: 0x001DAC99
	public void SetStabilityAt(int _x, int _y, byte _v)
	{
		this.m_Stability.set(_x, 0, _y, _v);
	}

	// Token: 0x06004B61 RID: 19297 RVA: 0x001DCAAA File Offset: 0x001DACAA
	public void Reset()
	{
		Array.Clear(this.m_Lower16Bits, 0, this.m_Lower16Bits.Length);
		this.m_Upper16Bits = null;
		if (this.m_Stability != null)
		{
			this.m_Stability.clear();
		}
		this.blockRefCount = 0;
		this.tickRefCount = 0;
	}

	// Token: 0x06004B62 RID: 19298 RVA: 0x00002914 File Offset: 0x00000B14
	public void Cleanup()
	{
	}

	// Token: 0x06004B63 RID: 19299 RVA: 0x001DCAE8 File Offset: 0x001DACE8
	public static int CalcOffset(int _x, int _z)
	{
		return (_x & 15) + ((_z & 15) << 4);
	}

	// Token: 0x06004B64 RID: 19300 RVA: 0x001DCAF5 File Offset: 0x001DACF5
	public static int OffsetX(int _offset)
	{
		return _offset & 15;
	}

	// Token: 0x06004B65 RID: 19301 RVA: 0x001DCAFB File Offset: 0x001DACFB
	public static int OffsetY(int _offset)
	{
		return _offset >> 4;
	}

	// Token: 0x06004B66 RID: 19302 RVA: 0x001DCB00 File Offset: 0x001DAD00
	public void UpdateRefCounts()
	{
		this.blockRefCount = 0;
		this.tickRefCount = 0;
		for (int i = this.m_Lower16Bits.Length - 1; i >= 0; i--)
		{
			int type = this.getAt(i).type;
			if (type > 0)
			{
				this.blockRefCount++;
				if (Block.list[type].IsRandomlyTick)
				{
					this.tickRefCount++;
				}
			}
		}
	}

	// Token: 0x06004B67 RID: 19303 RVA: 0x001DCB6E File Offset: 0x001DAD6E
	public int GetTickRefCount()
	{
		return this.tickRefCount;
	}

	// Token: 0x06004B68 RID: 19304 RVA: 0x001DCB76 File Offset: 0x001DAD76
	public bool IsOnlyTerrain()
	{
		return this.bOnlyTerrain;
	}

	// Token: 0x06004B69 RID: 19305 RVA: 0x001DCB80 File Offset: 0x001DAD80
	public void Read(BinaryReader stream, uint _version, bool _bNetworkRead, byte[] _tempReadBuf)
	{
		if (_version >= 19U)
		{
			stream.Read(_tempReadBuf, 0, 512);
			for (int i = 0; i < this.m_Lower16Bits.Length; i++)
			{
				ushort num = (ushort)((int)_tempReadBuf[i * 2] | (int)_tempReadBuf[i * 2 + 1] << 8);
				this.m_Lower16Bits[i] = num;
			}
			if (stream.ReadBoolean())
			{
				if (this.m_Upper16Bits == null)
				{
					this.m_Upper16Bits = new ushort[256];
				}
				stream.Read(_tempReadBuf, 0, 512);
				for (int j = 0; j < this.m_Upper16Bits.Length; j++)
				{
					ushort num2 = (ushort)((int)_tempReadBuf[j * 2] | (int)_tempReadBuf[j * 2 + 1] << 8);
					this.m_Upper16Bits[j] = num2;
				}
			}
			else
			{
				this.m_Upper16Bits = null;
			}
		}
		else if (_version >= 5U && _version < 19U)
		{
			stream.Read(_tempReadBuf, 0, 1024);
			for (int k = 0; k < this.m_Lower16Bits.Length; k++)
			{
				uint num3 = (uint)((int)_tempReadBuf[k * 4] | (int)_tempReadBuf[k * 4 + 1] << 8 | (int)_tempReadBuf[k * 4 + 2] << 16 | (int)_tempReadBuf[k * 4 + 3] << 24);
				this.m_Lower16Bits[k] = (ushort)(num3 & 65535U);
				if ((num3 & 4294901760U) != 0U)
				{
					if (this.m_Upper16Bits == null)
					{
						this.m_Upper16Bits = new ushort[256];
					}
					this.m_Upper16Bits[k] = (ushort)(num3 >> 16 & 65535U);
				}
				else if (this.m_Upper16Bits != null)
				{
					this.m_Upper16Bits[k] = 0;
				}
			}
		}
		if (_version > 8U && _version < 18U && !_bNetworkRead)
		{
			byte[] array = new byte[256];
			stream.Read(array, 0, array.Length);
			for (int l = 0; l < 16; l++)
			{
				for (int m = 0; m < 16; m++)
				{
					this.m_Stability.set(l, 0, m, array[l + m * 16]);
				}
			}
		}
		if (_version >= 18U && _version < 28U && !_bNetworkRead)
		{
			this.m_Stability.read(stream);
		}
		this.CheckOnlyTerrain();
	}

	// Token: 0x06004B6A RID: 19306 RVA: 0x001DCD8C File Offset: 0x001DAF8C
	public void Write(BinaryWriter _bw, bool _bNetworkWrite, byte[] _tempSaveBuf)
	{
		for (int i = 0; i < this.m_Lower16Bits.Length; i++)
		{
			uint num = (uint)this.m_Lower16Bits[i];
			_tempSaveBuf[i * 2] = (byte)(num & 255U);
			_tempSaveBuf[i * 2 + 1] = (byte)(num >> 8 & 255U);
		}
		_bw.Write(_tempSaveBuf, 0, this.m_Lower16Bits.Length * 2);
		_bw.Write(this.m_Upper16Bits != null);
		if (this.m_Upper16Bits != null)
		{
			for (int j = 0; j < this.m_Upper16Bits.Length; j++)
			{
				uint num2 = (uint)this.m_Upper16Bits[j];
				_tempSaveBuf[j * 2] = (byte)(num2 & 255U);
				_tempSaveBuf[j * 2 + 1] = (byte)(num2 >> 8 & 255U);
			}
			_bw.Write(_tempSaveBuf, 0, this.m_Upper16Bits.Length * 2);
		}
	}

	// Token: 0x06004B6B RID: 19307 RVA: 0x001DCE48 File Offset: 0x001DB048
	public void CheckOnlyTerrain()
	{
		this.bOnlyTerrain = true;
		uint typeMasked = BlockValue.GetTypeMasked((uint)this.m_Lower16Bits[0]);
		for (int i = 0; i < this.m_Lower16Bits.Length; i++)
		{
			typeMasked = BlockValue.GetTypeMasked((uint)this.m_Lower16Bits[i]);
			if (typeMasked != 0U && !Block.list[(int)typeMasked].shape.IsTerrain())
			{
				this.bOnlyTerrain = false;
				return;
			}
		}
	}

	// Token: 0x06004B6C RID: 19308 RVA: 0x001DCEA9 File Offset: 0x001DB0A9
	public int GetUsedMem()
	{
		return this.m_Lower16Bits.Length * 2 + ((this.m_Upper16Bits != null) ? (this.m_Upper16Bits.Length * 2) : 0) + 20 + 2;
	}

	// Token: 0x040039AE RID: 14766
	public ushort[] m_Lower16Bits;

	// Token: 0x040039AF RID: 14767
	public ushort[] m_Upper16Bits;

	// Token: 0x040039B0 RID: 14768
	[PublicizedFrom(EAccessModifier.Private)]
	public SmartArray m_Stability;

	// Token: 0x040039B1 RID: 14769
	[PublicizedFrom(EAccessModifier.Private)]
	public int wPow;

	// Token: 0x040039B2 RID: 14770
	[PublicizedFrom(EAccessModifier.Private)]
	public int hPow;

	// Token: 0x040039B3 RID: 14771
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bOnlyTerrain;

	// Token: 0x040039B4 RID: 14772
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bOnlyBlocks;

	// Token: 0x040039B5 RID: 14773
	[PublicizedFrom(EAccessModifier.Private)]
	public int blockRefCount;

	// Token: 0x040039B6 RID: 14774
	[PublicizedFrom(EAccessModifier.Private)]
	public int tickRefCount;

	// Token: 0x040039B7 RID: 14775
	public static int InstanceCount;

	// Token: 0x040039B8 RID: 14776
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cLayerHeight = 1;

	// Token: 0x040039B9 RID: 14777
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cArrSize = 256;
}
