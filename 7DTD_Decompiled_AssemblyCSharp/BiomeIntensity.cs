using System;
using System.IO;

// Token: 0x02000ACC RID: 2764
public struct BiomeIntensity : IEquatable<BiomeIntensity>
{
	// Token: 0x1700086B RID: 2155
	// (get) Token: 0x0600550A RID: 21770 RVA: 0x0022BC6B File Offset: 0x00229E6B
	// (set) Token: 0x0600550B RID: 21771 RVA: 0x0022BC7D File Offset: 0x00229E7D
	public float intensity0
	{
		get
		{
			return (float)(this.intensity0and1 & 15) / 15f;
		}
		set
		{
			this.intensity0and1 = (byte)((int)(this.intensity0and1 & 240) | ((int)(value * 15f) & 15));
		}
	}

	// Token: 0x1700086C RID: 2156
	// (get) Token: 0x0600550C RID: 21772 RVA: 0x0022BC9E File Offset: 0x00229E9E
	// (set) Token: 0x0600550D RID: 21773 RVA: 0x0022BCB2 File Offset: 0x00229EB2
	public float intensity1
	{
		get
		{
			return (float)(this.intensity0and1 >> 4 & 15) / 15f;
		}
		set
		{
			this.intensity0and1 = (byte)((int)(this.intensity0and1 & 15) | ((int)(value * 15f) << 4 & 240));
		}
	}

	// Token: 0x1700086D RID: 2157
	// (get) Token: 0x0600550E RID: 21774 RVA: 0x0022BCD5 File Offset: 0x00229ED5
	// (set) Token: 0x0600550F RID: 21775 RVA: 0x0022BCE7 File Offset: 0x00229EE7
	public float intensity2
	{
		get
		{
			return (float)(this.intensity2and3 & 15) / 15f;
		}
		set
		{
			this.intensity2and3 = (byte)((int)(this.intensity2and3 & 240) | ((int)(value * 15f) & 15));
		}
	}

	// Token: 0x1700086E RID: 2158
	// (get) Token: 0x06005510 RID: 21776 RVA: 0x0022BD08 File Offset: 0x00229F08
	// (set) Token: 0x06005511 RID: 21777 RVA: 0x0022BD1C File Offset: 0x00229F1C
	public float intensity3
	{
		get
		{
			return (float)(this.intensity2and3 >> 4 & 15) / 15f;
		}
		set
		{
			this.intensity2and3 = (byte)((int)(this.intensity2and3 & 15) | ((int)(value * 15f) << 4 & 240));
		}
	}

	// Token: 0x06005512 RID: 21778 RVA: 0x0022BD3F File Offset: 0x00229F3F
	public BiomeIntensity(byte _singleBiomeId)
	{
		this.biomeId0 = _singleBiomeId;
		this.biomeId1 = 0;
		this.biomeId2 = 0;
		this.biomeId3 = 0;
		this.intensity0and1 = 15;
		this.intensity2and3 = 0;
	}

	// Token: 0x06005513 RID: 21779 RVA: 0x0022BD6C File Offset: 0x00229F6C
	public BiomeIntensity(byte[] _chunkBiomeIntensityArray, int _offs)
	{
		this.biomeId0 = _chunkBiomeIntensityArray[_offs];
		this.biomeId1 = _chunkBiomeIntensityArray[_offs + 1];
		this.biomeId2 = _chunkBiomeIntensityArray[_offs + 2];
		this.biomeId3 = _chunkBiomeIntensityArray[_offs + 3];
		this.intensity0and1 = _chunkBiomeIntensityArray[_offs + 4];
		this.intensity2and3 = _chunkBiomeIntensityArray[_offs + 5];
	}

	// Token: 0x06005514 RID: 21780 RVA: 0x0022BDBC File Offset: 0x00229FBC
	public static BiomeIntensity FromArray(int[] _unsortedBiomeIdArray)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		BiomeIntensity biomeIntensity = default(BiomeIntensity);
		for (int i = 0; i < _unsortedBiomeIdArray.Length; i++)
		{
			if (num < _unsortedBiomeIdArray[i])
			{
				biomeIntensity.biomeId0 = (byte)i;
				num = _unsortedBiomeIdArray[i];
				if (num5 < num)
				{
					num5 = num;
				}
			}
		}
		_unsortedBiomeIdArray[(int)biomeIntensity.biomeId0] = 0;
		for (int j = 0; j < _unsortedBiomeIdArray.Length; j++)
		{
			if (num2 < _unsortedBiomeIdArray[j])
			{
				biomeIntensity.biomeId1 = (byte)j;
				num2 = _unsortedBiomeIdArray[j];
				if (num5 < num2)
				{
					num5 = num2;
				}
			}
		}
		_unsortedBiomeIdArray[(int)biomeIntensity.biomeId1] = 0;
		for (int k = 0; k < _unsortedBiomeIdArray.Length; k++)
		{
			if (num3 < _unsortedBiomeIdArray[k])
			{
				biomeIntensity.biomeId2 = (byte)k;
				num3 = _unsortedBiomeIdArray[k];
				if (num5 < num3)
				{
					num5 = num3;
				}
			}
		}
		_unsortedBiomeIdArray[(int)biomeIntensity.biomeId2] = 0;
		for (int l = 0; l < _unsortedBiomeIdArray.Length; l++)
		{
			if (num4 < _unsortedBiomeIdArray[l])
			{
				biomeIntensity.biomeId3 = (byte)l;
				num4 = _unsortedBiomeIdArray[l];
				if (num5 < num4)
				{
					num5 = num4;
				}
			}
		}
		_unsortedBiomeIdArray[(int)biomeIntensity.biomeId3] = 0;
		biomeIntensity.intensity0 = (float)num / (float)num5;
		biomeIntensity.intensity1 = (float)num2 / (float)num5;
		biomeIntensity.intensity2 = (float)num3 / (float)num5;
		biomeIntensity.intensity3 = (float)num4 / (float)num5;
		return biomeIntensity;
	}

	// Token: 0x06005515 RID: 21781 RVA: 0x0022BEFC File Offset: 0x0022A0FC
	public void ToArray(byte[] _array, int offs)
	{
		_array[offs] = this.biomeId0;
		_array[1 + offs] = this.biomeId1;
		_array[2 + offs] = this.biomeId2;
		_array[3 + offs] = this.biomeId3;
		_array[4 + offs] = this.intensity0and1;
		_array[5 + offs] = this.intensity2and3;
	}

	// Token: 0x06005516 RID: 21782 RVA: 0x0022BF4C File Offset: 0x0022A14C
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(this.biomeId0);
		_bw.Write((byte)(this.intensity0 * 255f));
		_bw.Write(this.biomeId1);
		_bw.Write((byte)(this.intensity1 * 255f));
		_bw.Write(this.biomeId2);
		_bw.Write((byte)(this.intensity2 * 255f));
		_bw.Write(this.biomeId3);
		_bw.Write((byte)(this.intensity3 * 255f));
	}

	// Token: 0x06005517 RID: 21783 RVA: 0x0022BFD8 File Offset: 0x0022A1D8
	public void Read(BinaryReader _br)
	{
		this.biomeId0 = _br.ReadByte();
		this.intensity0 = (float)_br.ReadByte() / 255f;
		this.biomeId1 = _br.ReadByte();
		this.intensity1 = (float)_br.ReadByte() / 255f;
		this.biomeId2 = _br.ReadByte();
		this.intensity2 = (float)_br.ReadByte() / 255f;
		this.biomeId3 = _br.ReadByte();
		this.intensity3 = (float)_br.ReadByte() / 255f;
	}

	// Token: 0x06005518 RID: 21784 RVA: 0x0022C064 File Offset: 0x0022A264
	public bool Equals(BiomeIntensity other)
	{
		return this.biomeId0 == other.biomeId0 && this.biomeId1 == other.biomeId1 && this.biomeId2 == other.biomeId2 && this.biomeId3 == other.biomeId3 && this.intensity0and1 == other.intensity0and1 && this.intensity2and3 == other.intensity2and3;
	}

	// Token: 0x06005519 RID: 21785 RVA: 0x0022C0C7 File Offset: 0x0022A2C7
	public override bool Equals(object obj)
	{
		return obj != null && obj is BiomeIntensity && this.Equals((BiomeIntensity)obj);
	}

	// Token: 0x0600551A RID: 21786 RVA: 0x0022C0E4 File Offset: 0x0022A2E4
	public override int GetHashCode()
	{
		return ((((this.biomeId0.GetHashCode() * 397 ^ this.biomeId1.GetHashCode()) * 397 ^ this.biomeId2.GetHashCode()) * 397 ^ this.biomeId3.GetHashCode()) * 397 ^ this.intensity0and1.GetHashCode()) * 397 ^ this.intensity2and3.GetHashCode();
	}

	// Token: 0x0600551B RID: 21787 RVA: 0x0022C158 File Offset: 0x0022A358
	public override string ToString()
	{
		return string.Format("[b0={0} b1={1} i0={2} i1={3}]", new object[]
		{
			this.biomeId0,
			this.biomeId1,
			this.intensity0.ToCultureInvariantString("0.0"),
			this.intensity1.ToCultureInvariantString("0.0")
		});
	}

	// Token: 0x040041E5 RID: 16869
	public const int cDataSize = 6;

	// Token: 0x040041E6 RID: 16870
	public static BiomeIntensity Default = new BiomeIntensity(0);

	// Token: 0x040041E7 RID: 16871
	public byte biomeId0;

	// Token: 0x040041E8 RID: 16872
	public byte biomeId1;

	// Token: 0x040041E9 RID: 16873
	public byte biomeId2;

	// Token: 0x040041EA RID: 16874
	public byte biomeId3;

	// Token: 0x040041EB RID: 16875
	public byte intensity0and1;

	// Token: 0x040041EC RID: 16876
	public byte intensity2and3;
}
