using System;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

// Token: 0x020010DC RID: 4316
public class JPGEncoder
{
	// Token: 0x060087DD RID: 34781 RVA: 0x0036F994 File Offset: 0x0036DB94
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitQuantTables(int sf)
	{
		int[] array = new int[]
		{
			16,
			11,
			10,
			16,
			24,
			40,
			51,
			61,
			12,
			12,
			14,
			19,
			26,
			58,
			60,
			55,
			14,
			13,
			16,
			24,
			40,
			57,
			69,
			56,
			14,
			17,
			22,
			29,
			51,
			87,
			80,
			62,
			18,
			22,
			37,
			56,
			68,
			109,
			103,
			77,
			24,
			35,
			55,
			64,
			81,
			104,
			113,
			92,
			49,
			64,
			78,
			87,
			103,
			121,
			120,
			101,
			72,
			92,
			95,
			98,
			112,
			100,
			103,
			99
		};
		int i;
		for (i = 0; i < 64; i++)
		{
			float num = Mathf.Floor((float)((array[i] * sf + 50) / 100));
			num = Mathf.Clamp(num, 1f, 255f);
			this.YTable[this.ZigZag[i]] = Mathf.RoundToInt(num);
		}
		int[] array2 = new int[]
		{
			17,
			18,
			24,
			47,
			99,
			99,
			99,
			99,
			18,
			21,
			26,
			66,
			99,
			99,
			99,
			99,
			24,
			26,
			56,
			99,
			99,
			99,
			99,
			99,
			47,
			66,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99
		};
		for (i = 0; i < 64; i++)
		{
			float num = Mathf.Floor((float)((array2[i] * sf + 50) / 100));
			num = Mathf.Clamp(num, 1f, 255f);
			this.UVTable[this.ZigZag[i]] = (int)num;
		}
		float[] array3 = new float[]
		{
			1f,
			1.3870399f,
			1.306563f,
			1.1758755f,
			1f,
			0.78569496f,
			0.5411961f,
			0.27589938f
		};
		i = 0;
		for (int j = 0; j < 8; j++)
		{
			for (int k = 0; k < 8; k++)
			{
				this.fdtbl_Y[i] = 1f / ((float)this.YTable[this.ZigZag[i]] * array3[j] * array3[k] * 8f);
				this.fdtbl_UV[i] = 1f / ((float)this.UVTable[this.ZigZag[i]] * array3[j] * array3[k] * 8f);
				i++;
			}
		}
	}

	// Token: 0x060087DE RID: 34782 RVA: 0x0036FAE8 File Offset: 0x0036DCE8
	[PublicizedFrom(EAccessModifier.Private)]
	public JPGEncoder.BitString[] ComputeHuffmanTbl(byte[] nrcodes, byte[] std_table)
	{
		int num = 0;
		int num2 = 0;
		JPGEncoder.BitString[] array = new JPGEncoder.BitString[256];
		for (int i = 1; i <= 16; i++)
		{
			for (int j = 1; j <= (int)nrcodes[i]; j++)
			{
				array[(int)std_table[num2]] = default(JPGEncoder.BitString);
				array[(int)std_table[num2]].value = num;
				array[(int)std_table[num2]].length = i;
				num2++;
				num++;
			}
			num *= 2;
		}
		return array;
	}

	// Token: 0x060087DF RID: 34783 RVA: 0x0036FB60 File Offset: 0x0036DD60
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitHuffmanTbl()
	{
		this.YDC_HT = this.ComputeHuffmanTbl(this.std_dc_luminance_nrcodes, this.std_dc_luminance_values);
		this.UVDC_HT = this.ComputeHuffmanTbl(this.std_dc_chrominance_nrcodes, this.std_dc_chrominance_values);
		this.YAC_HT = this.ComputeHuffmanTbl(this.std_ac_luminance_nrcodes, this.std_ac_luminance_values);
		this.UVAC_HT = this.ComputeHuffmanTbl(this.std_ac_chrominance_nrcodes, this.std_ac_chrominance_values);
	}

	// Token: 0x060087E0 RID: 34784 RVA: 0x0036FBD0 File Offset: 0x0036DDD0
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitCategoryfloat()
	{
		int num = 1;
		int num2 = 2;
		for (int i = 1; i <= 15; i++)
		{
			for (int j = num; j < num2; j++)
			{
				this.category[32767 + j] = i;
				JPGEncoder.BitString bitString = default(JPGEncoder.BitString);
				bitString.length = i;
				bitString.value = j;
				this.bitcode[32767 + j] = bitString;
			}
			for (int j = -(num2 - 1); j <= -num; j++)
			{
				this.category[32767 + j] = i;
				JPGEncoder.BitString bitString = default(JPGEncoder.BitString);
				bitString.length = i;
				bitString.value = num2 - 1 + j;
				this.bitcode[32767 + j] = bitString;
			}
			num <<= 1;
			num2 <<= 1;
		}
	}

	// Token: 0x060087E1 RID: 34785 RVA: 0x0036FC98 File Offset: 0x0036DE98
	public byte[] GetBytes()
	{
		if (!this.isDone)
		{
			Log.Error("JPEGEncoder not complete, cannot get bytes!");
			return null;
		}
		return this.byteout.GetAllBytes();
	}

	// Token: 0x060087E2 RID: 34786 RVA: 0x0036FCBC File Offset: 0x0036DEBC
	[PublicizedFrom(EAccessModifier.Private)]
	public void WriteBits(JPGEncoder.BitString bs)
	{
		int value = bs.value;
		int i = bs.length - 1;
		while (i >= 0)
		{
			if (((long)value & (long)((ulong)Convert.ToUInt32(1 << i))) != 0L)
			{
				this.bytenew |= Convert.ToUInt32(1 << this.bytepos);
			}
			i--;
			this.bytepos--;
			if (this.bytepos < 0)
			{
				if (this.bytenew == 255U)
				{
					this.WriteByte(byte.MaxValue);
					this.WriteByte(0);
				}
				else
				{
					this.WriteByte((byte)this.bytenew);
				}
				this.bytepos = 7;
				this.bytenew = 0U;
			}
		}
	}

	// Token: 0x060087E3 RID: 34787 RVA: 0x0036FD6A File Offset: 0x0036DF6A
	[PublicizedFrom(EAccessModifier.Private)]
	public void WriteByte(byte value)
	{
		this.byteout.WriteByte(value);
	}

	// Token: 0x060087E4 RID: 34788 RVA: 0x0036FD78 File Offset: 0x0036DF78
	[PublicizedFrom(EAccessModifier.Private)]
	public void WriteWord(int value)
	{
		this.WriteByte((byte)(value >> 8 & 255));
		this.WriteByte((byte)(value & 255));
	}

	// Token: 0x060087E5 RID: 34789 RVA: 0x0036FD98 File Offset: 0x0036DF98
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] FDCTQuant(float[] data, float[] fdtbl)
	{
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			float num2 = data[num] + data[num + 7];
			float num3 = data[num] - data[num + 7];
			float num4 = data[num + 1] + data[num + 6];
			float num5 = data[num + 1] - data[num + 6];
			float num6 = data[num + 2] + data[num + 5];
			float num7 = data[num + 2] - data[num + 5];
			float num8 = data[num + 3] + data[num + 4];
			float num9 = data[num + 3] - data[num + 4];
			float num10 = num2 + num8;
			float num11 = num2 - num8;
			float num12 = num4 + num6;
			float num13 = num4 - num6;
			data[num] = num10 + num12;
			data[num + 4] = num10 - num12;
			float num14 = (num13 + num11) * 0.70710677f;
			data[num + 2] = num11 + num14;
			data[num + 6] = num11 - num14;
			num10 = num9 + num7;
			num12 = num7 + num5;
			num13 = num5 + num3;
			float num15 = (num10 - num13) * 0.38268343f;
			float num16 = 0.5411961f * num10 + num15;
			float num17 = 1.306563f * num13 + num15;
			float num18 = num12 * 0.70710677f;
			float num19 = num3 + num18;
			float num20 = num3 - num18;
			data[num + 5] = num20 + num16;
			data[num + 3] = num20 - num16;
			data[num + 1] = num19 + num17;
			data[num + 7] = num19 - num17;
			num += 8;
		}
		num = 0;
		for (int i = 0; i < 8; i++)
		{
			float num2 = data[num] + data[num + 56];
			float num3 = data[num] - data[num + 56];
			float num4 = data[num + 8] + data[num + 48];
			float num5 = data[num + 8] - data[num + 48];
			float num6 = data[num + 16] + data[num + 40];
			float num7 = data[num + 16] - data[num + 40];
			float num8 = data[num + 24] + data[num + 32];
			float num21 = data[num + 24] - data[num + 32];
			float num10 = num2 + num8;
			float num11 = num2 - num8;
			float num12 = num4 + num6;
			float num13 = num4 - num6;
			data[num] = num10 + num12;
			data[num + 32] = num10 - num12;
			float num14 = (num13 + num11) * 0.70710677f;
			data[num + 16] = num11 + num14;
			data[num + 48] = num11 - num14;
			num10 = num21 + num7;
			num12 = num7 + num5;
			num13 = num5 + num3;
			float num15 = (num10 - num13) * 0.38268343f;
			float num16 = 0.5411961f * num10 + num15;
			float num17 = 1.306563f * num13 + num15;
			float num18 = num12 * 0.70710677f;
			float num19 = num3 + num18;
			float num20 = num3 - num18;
			data[num + 40] = num20 + num16;
			data[num + 24] = num20 - num16;
			data[num + 8] = num19 + num17;
			data[num + 56] = num19 - num17;
			num++;
		}
		for (int i = 0; i < 64; i++)
		{
			data[i] = Mathf.Round(data[i] * fdtbl[i]);
		}
		return data;
	}

	// Token: 0x060087E6 RID: 34790 RVA: 0x00370088 File Offset: 0x0036E288
	[PublicizedFrom(EAccessModifier.Private)]
	public void WriteAPP0()
	{
		this.WriteWord(65504);
		this.WriteWord(16);
		this.WriteByte(74);
		this.WriteByte(70);
		this.WriteByte(73);
		this.WriteByte(70);
		this.WriteByte(0);
		this.WriteByte(1);
		this.WriteByte(1);
		this.WriteByte(0);
		this.WriteWord(1);
		this.WriteWord(1);
		this.WriteByte(0);
		this.WriteByte(0);
	}

	// Token: 0x060087E7 RID: 34791 RVA: 0x00370100 File Offset: 0x0036E300
	[PublicizedFrom(EAccessModifier.Private)]
	public void WriteSOF0(int width, int height)
	{
		this.WriteWord(65472);
		this.WriteWord(17);
		this.WriteByte(8);
		this.WriteWord(height);
		this.WriteWord(width);
		this.WriteByte(3);
		this.WriteByte(1);
		this.WriteByte(17);
		this.WriteByte(0);
		this.WriteByte(2);
		this.WriteByte(17);
		this.WriteByte(1);
		this.WriteByte(3);
		this.WriteByte(17);
		this.WriteByte(1);
	}

	// Token: 0x060087E8 RID: 34792 RVA: 0x00370180 File Offset: 0x0036E380
	[PublicizedFrom(EAccessModifier.Private)]
	public void WriteDQT()
	{
		this.WriteWord(65499);
		this.WriteWord(132);
		this.WriteByte(0);
		for (int i = 0; i < 64; i++)
		{
			this.WriteByte((byte)this.YTable[i]);
		}
		this.WriteByte(1);
		for (int i = 0; i < 64; i++)
		{
			this.WriteByte((byte)this.UVTable[i]);
		}
	}

	// Token: 0x060087E9 RID: 34793 RVA: 0x003701EC File Offset: 0x0036E3EC
	[PublicizedFrom(EAccessModifier.Private)]
	public void WriteDHT()
	{
		this.WriteWord(65476);
		this.WriteWord(418);
		this.WriteByte(0);
		for (int i = 0; i < 16; i++)
		{
			this.WriteByte(this.std_dc_luminance_nrcodes[i + 1]);
		}
		for (int i = 0; i <= 11; i++)
		{
			this.WriteByte(this.std_dc_luminance_values[i]);
		}
		this.WriteByte(16);
		for (int i = 0; i < 16; i++)
		{
			this.WriteByte(this.std_ac_luminance_nrcodes[i + 1]);
		}
		for (int i = 0; i <= 161; i++)
		{
			this.WriteByte(this.std_ac_luminance_values[i]);
		}
		this.WriteByte(1);
		for (int i = 0; i < 16; i++)
		{
			this.WriteByte(this.std_dc_chrominance_nrcodes[i + 1]);
		}
		for (int i = 0; i <= 11; i++)
		{
			this.WriteByte(this.std_dc_chrominance_values[i]);
		}
		this.WriteByte(17);
		for (int i = 0; i < 16; i++)
		{
			this.WriteByte(this.std_ac_chrominance_nrcodes[i + 1]);
		}
		for (int i = 0; i <= 161; i++)
		{
			this.WriteByte(this.std_ac_chrominance_values[i]);
		}
	}

	// Token: 0x060087EA RID: 34794 RVA: 0x00370314 File Offset: 0x0036E514
	[PublicizedFrom(EAccessModifier.Private)]
	public void writeSOS()
	{
		this.WriteWord(65498);
		this.WriteWord(12);
		this.WriteByte(3);
		this.WriteByte(1);
		this.WriteByte(0);
		this.WriteByte(2);
		this.WriteByte(17);
		this.WriteByte(3);
		this.WriteByte(17);
		this.WriteByte(0);
		this.WriteByte(63);
		this.WriteByte(0);
	}

	// Token: 0x060087EB RID: 34795 RVA: 0x00370380 File Offset: 0x0036E580
	[PublicizedFrom(EAccessModifier.Private)]
	public float ProcessDU(float[] CDU, float[] fdtbl, float DC, JPGEncoder.BitString[] HTDC, JPGEncoder.BitString[] HTAC)
	{
		JPGEncoder.BitString bs = HTAC[0];
		JPGEncoder.BitString bs2 = HTAC[240];
		float[] array = this.FDCTQuant(CDU, fdtbl);
		for (int i = 0; i < 64; i++)
		{
			this.DU[this.ZigZag[i]] = (int)array[i];
		}
		int num = (int)((float)this.DU[0] - DC);
		DC = (float)this.DU[0];
		if (num == 0)
		{
			this.WriteBits(HTDC[0]);
		}
		else
		{
			this.WriteBits(HTDC[this.category[32767 + num]]);
			this.WriteBits(this.bitcode[32767 + num]);
		}
		int num2 = 63;
		while (num2 > 0 && this.DU[num2] == 0)
		{
			num2--;
		}
		if (num2 == 0)
		{
			this.WriteBits(bs);
			return DC;
		}
		for (int i = 1; i <= num2; i++)
		{
			int num3 = i;
			while (this.DU[i] == 0 && i <= num2)
			{
				i++;
			}
			int num4 = i - num3;
			if (num4 >= 16)
			{
				for (int j = 1; j <= num4 / 16; j++)
				{
					this.WriteBits(bs2);
				}
				num4 &= 15;
			}
			this.WriteBits(HTAC[num4 * 16 + this.category[32767 + this.DU[i]]]);
			this.WriteBits(this.bitcode[32767 + this.DU[i]]);
		}
		if (num2 != 63)
		{
			this.WriteBits(bs);
		}
		return DC;
	}

	// Token: 0x060087EC RID: 34796 RVA: 0x00370508 File Offset: 0x0036E708
	[PublicizedFrom(EAccessModifier.Private)]
	public void RGB2YUV(JPGEncoder.BitmapData image, int xpos, int ypos)
	{
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				Color32 pixelColor = image.GetPixelColor(xpos + j, image.height - (ypos + i));
				this.YDU[num] = 0.299f * (float)pixelColor.r + 0.587f * (float)pixelColor.g + 0.114f * (float)pixelColor.b - 128f;
				this.UDU[num] = -0.16874f * (float)pixelColor.r + -0.33126f * (float)pixelColor.g + 0.5f * (float)pixelColor.b;
				this.VDU[num] = 0.5f * (float)pixelColor.r + -0.41869f * (float)pixelColor.g + -0.08131f * (float)pixelColor.b;
				num++;
			}
		}
	}

	// Token: 0x060087ED RID: 34797 RVA: 0x003705EC File Offset: 0x0036E7EC
	public JPGEncoder(Texture2D texture, float quality) : this(texture, quality, "", false)
	{
	}

	// Token: 0x060087EE RID: 34798 RVA: 0x003705FC File Offset: 0x0036E7FC
	public JPGEncoder(Texture2D texture, float quality, bool blocking) : this(texture, quality, "", blocking)
	{
	}

	// Token: 0x060087EF RID: 34799 RVA: 0x0037060C File Offset: 0x0036E80C
	public JPGEncoder(Texture2D texture, float quality, string path) : this(texture, quality, path, false)
	{
	}

	// Token: 0x060087F0 RID: 34800 RVA: 0x00370618 File Offset: 0x0036E818
	public JPGEncoder(Texture2D texture, float quality, string path, bool blocking)
	{
		this.path = path;
		this.image = new JPGEncoder.BitmapData(texture);
		quality = Mathf.Clamp(quality, 1f, 100f);
		this.sf = ((quality < 50f) ? ((int)(5000f / quality)) : ((int)(200f - quality * 2f)));
		this.cores = SystemInfo.processorCount;
		Thread thread = new Thread(new ThreadStart(this.DoEncoding));
		thread.Name = "JPGEncoder";
		thread.Start();
		if (blocking)
		{
			thread.Join();
		}
	}

	// Token: 0x060087F1 RID: 34801 RVA: 0x00370828 File Offset: 0x0036EA28
	[PublicizedFrom(EAccessModifier.Private)]
	public void DoEncoding()
	{
		this.isDone = false;
		this.InitHuffmanTbl();
		this.InitCategoryfloat();
		this.InitQuantTables(this.sf);
		this.Encode();
		if (!string.IsNullOrEmpty(this.path))
		{
			SdFile.WriteAllBytes(this.path, this.GetBytes());
		}
		this.isDone = true;
		Profiler.EndThreadProfiling();
	}

	// Token: 0x060087F2 RID: 34802 RVA: 0x00370884 File Offset: 0x0036EA84
	[PublicizedFrom(EAccessModifier.Private)]
	public void Encode()
	{
		this.byteout = new JPGEncoder.ByteArray();
		this.bytenew = 0U;
		this.bytepos = 7;
		this.WriteWord(65496);
		this.WriteAPP0();
		this.WriteDQT();
		this.WriteSOF0(this.image.width, this.image.height);
		this.WriteDHT();
		this.writeSOS();
		float dc = 0f;
		float dc2 = 0f;
		float dc3 = 0f;
		this.bytenew = 0U;
		this.bytepos = 7;
		for (int i = 0; i < this.image.height; i += 8)
		{
			for (int j = 0; j < this.image.width; j += 8)
			{
				this.RGB2YUV(this.image, j, i);
				dc = this.ProcessDU(this.YDU, this.fdtbl_Y, dc, this.YDC_HT, this.YAC_HT);
				dc2 = this.ProcessDU(this.UDU, this.fdtbl_UV, dc2, this.UVDC_HT, this.UVAC_HT);
				dc3 = this.ProcessDU(this.VDU, this.fdtbl_UV, dc3, this.UVDC_HT, this.UVAC_HT);
				if (this.cores == 1)
				{
					Thread.Sleep(0);
				}
			}
		}
		if (this.bytepos >= 0)
		{
			this.WriteBits(new JPGEncoder.BitString
			{
				length = this.bytepos + 1,
				value = (1 << this.bytepos + 1) - 1
			});
		}
		this.WriteWord(65497);
		this.isDone = true;
	}

	// Token: 0x040069A1 RID: 27041
	[PublicizedFrom(EAccessModifier.Private)]
	public int[] ZigZag = new int[]
	{
		0,
		1,
		5,
		6,
		14,
		15,
		27,
		28,
		2,
		4,
		7,
		13,
		16,
		26,
		29,
		42,
		3,
		8,
		12,
		17,
		25,
		30,
		41,
		43,
		9,
		11,
		18,
		24,
		31,
		40,
		44,
		53,
		10,
		19,
		23,
		32,
		39,
		45,
		52,
		54,
		20,
		22,
		33,
		38,
		46,
		51,
		55,
		60,
		21,
		34,
		37,
		47,
		50,
		56,
		59,
		61,
		35,
		36,
		48,
		49,
		57,
		58,
		62,
		63
	};

	// Token: 0x040069A2 RID: 27042
	[PublicizedFrom(EAccessModifier.Private)]
	public int[] YTable = new int[64];

	// Token: 0x040069A3 RID: 27043
	[PublicizedFrom(EAccessModifier.Private)]
	public int[] UVTable = new int[64];

	// Token: 0x040069A4 RID: 27044
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] fdtbl_Y = new float[64];

	// Token: 0x040069A5 RID: 27045
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] fdtbl_UV = new float[64];

	// Token: 0x040069A6 RID: 27046
	[PublicizedFrom(EAccessModifier.Private)]
	public JPGEncoder.BitString[] YDC_HT;

	// Token: 0x040069A7 RID: 27047
	[PublicizedFrom(EAccessModifier.Private)]
	public JPGEncoder.BitString[] UVDC_HT;

	// Token: 0x040069A8 RID: 27048
	[PublicizedFrom(EAccessModifier.Private)]
	public JPGEncoder.BitString[] YAC_HT;

	// Token: 0x040069A9 RID: 27049
	[PublicizedFrom(EAccessModifier.Private)]
	public JPGEncoder.BitString[] UVAC_HT;

	// Token: 0x040069AA RID: 27050
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] std_dc_luminance_nrcodes = new byte[]
	{
		0,
		0,
		1,
		5,
		1,
		1,
		1,
		1,
		1,
		1,
		0,
		0,
		0,
		0,
		0,
		0,
		0
	};

	// Token: 0x040069AB RID: 27051
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] std_dc_luminance_values = new byte[]
	{
		0,
		1,
		2,
		3,
		4,
		5,
		6,
		7,
		8,
		9,
		10,
		11
	};

	// Token: 0x040069AC RID: 27052
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] std_ac_luminance_nrcodes = new byte[]
	{
		0,
		0,
		2,
		1,
		3,
		3,
		2,
		4,
		3,
		5,
		5,
		4,
		4,
		0,
		0,
		1,
		125
	};

	// Token: 0x040069AD RID: 27053
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] std_ac_luminance_values = new byte[]
	{
		1,
		2,
		3,
		0,
		4,
		17,
		5,
		18,
		33,
		49,
		65,
		6,
		19,
		81,
		97,
		7,
		34,
		113,
		20,
		50,
		129,
		145,
		161,
		8,
		35,
		66,
		177,
		193,
		21,
		82,
		209,
		240,
		36,
		51,
		98,
		114,
		130,
		9,
		10,
		22,
		23,
		24,
		25,
		26,
		37,
		38,
		39,
		40,
		41,
		42,
		52,
		53,
		54,
		55,
		56,
		57,
		58,
		67,
		68,
		69,
		70,
		71,
		72,
		73,
		74,
		83,
		84,
		85,
		86,
		87,
		88,
		89,
		90,
		99,
		100,
		101,
		102,
		103,
		104,
		105,
		106,
		115,
		116,
		117,
		118,
		119,
		120,
		121,
		122,
		131,
		132,
		133,
		134,
		135,
		136,
		137,
		138,
		146,
		147,
		148,
		149,
		150,
		151,
		152,
		153,
		154,
		162,
		163,
		164,
		165,
		166,
		167,
		168,
		169,
		170,
		178,
		179,
		180,
		181,
		182,
		183,
		184,
		185,
		186,
		194,
		195,
		196,
		197,
		198,
		199,
		200,
		201,
		202,
		210,
		211,
		212,
		213,
		214,
		215,
		216,
		217,
		218,
		225,
		226,
		227,
		228,
		229,
		230,
		231,
		232,
		233,
		234,
		241,
		242,
		243,
		244,
		245,
		246,
		247,
		248,
		249,
		250
	};

	// Token: 0x040069AE RID: 27054
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] std_dc_chrominance_nrcodes = new byte[]
	{
		0,
		0,
		3,
		1,
		1,
		1,
		1,
		1,
		1,
		1,
		1,
		1,
		0,
		0,
		0,
		0,
		0
	};

	// Token: 0x040069AF RID: 27055
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] std_dc_chrominance_values = new byte[]
	{
		0,
		1,
		2,
		3,
		4,
		5,
		6,
		7,
		8,
		9,
		10,
		11
	};

	// Token: 0x040069B0 RID: 27056
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] std_ac_chrominance_nrcodes = new byte[]
	{
		0,
		0,
		2,
		1,
		2,
		4,
		4,
		3,
		4,
		7,
		5,
		4,
		4,
		0,
		1,
		2,
		119
	};

	// Token: 0x040069B1 RID: 27057
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] std_ac_chrominance_values = new byte[]
	{
		0,
		1,
		2,
		3,
		17,
		4,
		5,
		33,
		49,
		6,
		18,
		65,
		81,
		7,
		97,
		113,
		19,
		34,
		50,
		129,
		8,
		20,
		66,
		145,
		161,
		177,
		193,
		9,
		35,
		51,
		82,
		240,
		21,
		98,
		114,
		209,
		10,
		22,
		36,
		52,
		225,
		37,
		241,
		23,
		24,
		25,
		26,
		38,
		39,
		40,
		41,
		42,
		53,
		54,
		55,
		56,
		57,
		58,
		67,
		68,
		69,
		70,
		71,
		72,
		73,
		74,
		83,
		84,
		85,
		86,
		87,
		88,
		89,
		90,
		99,
		100,
		101,
		102,
		103,
		104,
		105,
		106,
		115,
		116,
		117,
		118,
		119,
		120,
		121,
		122,
		130,
		131,
		132,
		133,
		134,
		135,
		136,
		137,
		138,
		146,
		147,
		148,
		149,
		150,
		151,
		152,
		153,
		154,
		162,
		163,
		164,
		165,
		166,
		167,
		168,
		169,
		170,
		178,
		179,
		180,
		181,
		182,
		183,
		184,
		185,
		186,
		194,
		195,
		196,
		197,
		198,
		199,
		200,
		201,
		202,
		210,
		211,
		212,
		213,
		214,
		215,
		216,
		217,
		218,
		226,
		227,
		228,
		229,
		230,
		231,
		232,
		233,
		234,
		242,
		243,
		244,
		245,
		246,
		247,
		248,
		249,
		250
	};

	// Token: 0x040069B2 RID: 27058
	[PublicizedFrom(EAccessModifier.Private)]
	public JPGEncoder.BitString[] bitcode = new JPGEncoder.BitString[65535];

	// Token: 0x040069B3 RID: 27059
	[PublicizedFrom(EAccessModifier.Private)]
	public int[] category = new int[65535];

	// Token: 0x040069B4 RID: 27060
	[PublicizedFrom(EAccessModifier.Private)]
	public uint bytenew;

	// Token: 0x040069B5 RID: 27061
	[PublicizedFrom(EAccessModifier.Private)]
	public int bytepos = 7;

	// Token: 0x040069B6 RID: 27062
	[PublicizedFrom(EAccessModifier.Private)]
	public JPGEncoder.ByteArray byteout = new JPGEncoder.ByteArray();

	// Token: 0x040069B7 RID: 27063
	[PublicizedFrom(EAccessModifier.Private)]
	public int[] DU = new int[64];

	// Token: 0x040069B8 RID: 27064
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] YDU = new float[64];

	// Token: 0x040069B9 RID: 27065
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] UDU = new float[64];

	// Token: 0x040069BA RID: 27066
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] VDU = new float[64];

	// Token: 0x040069BB RID: 27067
	public bool isDone;

	// Token: 0x040069BC RID: 27068
	[PublicizedFrom(EAccessModifier.Private)]
	public JPGEncoder.BitmapData image;

	// Token: 0x040069BD RID: 27069
	[PublicizedFrom(EAccessModifier.Private)]
	public int sf;

	// Token: 0x040069BE RID: 27070
	[PublicizedFrom(EAccessModifier.Private)]
	public string path;

	// Token: 0x040069BF RID: 27071
	[PublicizedFrom(EAccessModifier.Private)]
	public int cores;

	// Token: 0x020010DD RID: 4317
	[PublicizedFrom(EAccessModifier.Private)]
	public class ByteArray
	{
		// Token: 0x060087F3 RID: 34803 RVA: 0x00370A13 File Offset: 0x0036EC13
		public ByteArray()
		{
			this.stream = new MemoryStream();
			this.writer = new BinaryWriter(this.stream);
		}

		// Token: 0x060087F4 RID: 34804 RVA: 0x00370A37 File Offset: 0x0036EC37
		public void WriteByte(byte value)
		{
			this.writer.Write(value);
		}

		// Token: 0x060087F5 RID: 34805 RVA: 0x00370A48 File Offset: 0x0036EC48
		public byte[] GetAllBytes()
		{
			byte[] array = new byte[this.stream.Length];
			this.stream.Position = 0L;
			this.stream.Read(array, 0, array.Length);
			return array;
		}

		// Token: 0x040069C0 RID: 27072
		[PublicizedFrom(EAccessModifier.Private)]
		public MemoryStream stream;

		// Token: 0x040069C1 RID: 27073
		[PublicizedFrom(EAccessModifier.Private)]
		public BinaryWriter writer;
	}

	// Token: 0x020010DE RID: 4318
	[PublicizedFrom(EAccessModifier.Private)]
	public struct BitString
	{
		// Token: 0x040069C2 RID: 27074
		public int length;

		// Token: 0x040069C3 RID: 27075
		public int value;
	}

	// Token: 0x020010DF RID: 4319
	[PublicizedFrom(EAccessModifier.Private)]
	public class BitmapData
	{
		// Token: 0x060087F6 RID: 34806 RVA: 0x00370A86 File Offset: 0x0036EC86
		public BitmapData(Texture2D texture)
		{
			this.height = texture.height;
			this.width = texture.width;
			this.pixels = texture.GetPixels32();
		}

		// Token: 0x060087F7 RID: 34807 RVA: 0x00370AB2 File Offset: 0x0036ECB2
		public Color32 GetPixelColor(int x, int y)
		{
			x = Mathf.Clamp(x, 0, this.width - 1);
			y = Mathf.Clamp(y, 0, this.height - 1);
			return this.pixels[y * this.width + x];
		}

		// Token: 0x040069C4 RID: 27076
		public int height;

		// Token: 0x040069C5 RID: 27077
		public int width;

		// Token: 0x040069C6 RID: 27078
		[PublicizedFrom(EAccessModifier.Private)]
		public Color32[] pixels;
	}
}
