using System;
using System.Runtime.CompilerServices;

// Token: 0x0200126E RID: 4718
public static class OpenSimplex2S
{
	// Token: 0x060093CD RID: 37837 RVA: 0x003ADC28 File Offset: 0x003ABE28
	public static float Noise2(long seed, double x, double y)
	{
		double num = 0.366025403784439 * (x + y);
		double xs = x + num;
		double ys = y + num;
		return OpenSimplex2S.Noise2_UnskewedBase(seed, xs, ys);
	}

	// Token: 0x060093CE RID: 37838 RVA: 0x003ADC54 File Offset: 0x003ABE54
	public static float Noise2_ImproveX(long seed, double x, double y)
	{
		double num = x * 0.7071067811865476;
		double num2 = y * 1.2247448713915896;
		return OpenSimplex2S.Noise2_UnskewedBase(seed, num2 + num, num2 - num);
	}

	// Token: 0x060093CF RID: 37839 RVA: 0x003ADC88 File Offset: 0x003ABE88
	[PublicizedFrom(EAccessModifier.Private)]
	public static float Noise2_UnskewedBase(long seed, double xs, double ys)
	{
		int num = OpenSimplex2S.FastFloor(xs);
		int num2 = OpenSimplex2S.FastFloor(ys);
		float num3 = (float)(xs - (double)num);
		float num4 = (float)(ys - (double)num2);
		long num5 = (long)num * 5910200641878280303L;
		long num6 = (long)num2 * 6452764530575939509L;
		float num7 = (num3 + num4) * -0.21132487f;
		float num8 = num3 + num7;
		float num9 = num4 + num7;
		float num10 = 0.6666667f - num8 * num8 - num9 * num9;
		float num11 = num10 * num10 * (num10 * num10) * OpenSimplex2S.Grad(seed, num5, num6, num8, num9);
		float num12 = -3.1547005f * num7 + (-0.6666667f + num10);
		float dx = num8 - 0.57735026f;
		float dy = num9 - 0.57735026f;
		num11 += num12 * num12 * (num12 * num12) * OpenSimplex2S.Grad(seed, num5 + 5910200641878280303L, num6 + 6452764530575939509L, dx, dy);
		float num13 = num3 - num4;
		if ((double)num7 < -0.21132486540518713)
		{
			if (num3 + num13 > 1f)
			{
				float num14 = num8 - 1.3660254f;
				float num15 = num9 - 0.36602542f;
				float num16 = 0.6666667f - num14 * num14 - num15 * num15;
				if (num16 > 0f)
				{
					num11 += num16 * num16 * (num16 * num16) * OpenSimplex2S.Grad(seed, num5 + -6626342789952991010L, num6 + 6452764530575939509L, num14, num15);
				}
			}
			else
			{
				float num17 = num8 - -0.21132487f;
				float num18 = num9 - 0.7886751f;
				float num19 = 0.6666667f - num17 * num17 - num18 * num18;
				if (num19 > 0f)
				{
					num11 += num19 * num19 * (num19 * num19) * OpenSimplex2S.Grad(seed, num5, num6 + 6452764530575939509L, num17, num18);
				}
			}
			if (num4 - num13 > 1f)
			{
				float num20 = num8 - 0.36602542f;
				float num21 = num9 - 1.3660254f;
				float num22 = 0.6666667f - num20 * num20 - num21 * num21;
				if (num22 > 0f)
				{
					num11 += num22 * num22 * (num22 * num22) * OpenSimplex2S.Grad(seed, num5 + 5910200641878280303L, num6 + -5541215012557672598L, num20, num21);
				}
			}
			else
			{
				float num23 = num8 - 0.7886751f;
				float num24 = num9 - -0.21132487f;
				float num25 = 0.6666667f - num23 * num23 - num24 * num24;
				if (num25 > 0f)
				{
					num11 += num25 * num25 * (num25 * num25) * OpenSimplex2S.Grad(seed, num5 + 5910200641878280303L, num6, num23, num24);
				}
			}
		}
		else
		{
			if (num3 + num13 < 0f)
			{
				float num26 = num8 + 0.7886751f;
				float num27 = num9 + -0.21132487f;
				float num28 = 0.6666667f - num26 * num26 - num27 * num27;
				if (num28 > 0f)
				{
					num11 += num28 * num28 * (num28 * num28) * OpenSimplex2S.Grad(seed, num5 - 5910200641878280303L, num6, num26, num27);
				}
			}
			else
			{
				float num29 = num8 - 0.7886751f;
				float num30 = num9 - -0.21132487f;
				float num31 = 0.6666667f - num29 * num29 - num30 * num30;
				if (num31 > 0f)
				{
					num11 += num31 * num31 * (num31 * num31) * OpenSimplex2S.Grad(seed, num5 + 5910200641878280303L, num6, num29, num30);
				}
			}
			if (num4 < num13)
			{
				float num32 = num8 + -0.21132487f;
				float num33 = num9 + 0.7886751f;
				float num34 = 0.6666667f - num32 * num32 - num33 * num33;
				if (num34 > 0f)
				{
					num11 += num34 * num34 * (num34 * num34) * OpenSimplex2S.Grad(seed, num5, num6 - 6452764530575939509L, num32, num33);
				}
			}
			else
			{
				float num35 = num8 - -0.21132487f;
				float num36 = num9 - 0.7886751f;
				float num37 = 0.6666667f - num35 * num35 - num36 * num36;
				if (num37 > 0f)
				{
					num11 += num37 * num37 * (num37 * num37) * OpenSimplex2S.Grad(seed, num5, num6 + 6452764530575939509L, num35, num36);
				}
			}
		}
		return num11;
	}

	// Token: 0x060093D0 RID: 37840 RVA: 0x003AE0A4 File Offset: 0x003AC2A4
	public static float Noise3_ImproveXY(long seed, double x, double y, double z)
	{
		double num = x + y;
		double num2 = num * -0.21132486540518713;
		double num3 = z * 0.577350269189626;
		double xr = x + num2 + num3;
		double yr = y + num2 + num3;
		double zr = num * -0.577350269189626 + num3;
		return OpenSimplex2S.Noise3_UnrotatedBase(seed, xr, yr, zr);
	}

	// Token: 0x060093D1 RID: 37841 RVA: 0x003AE0F0 File Offset: 0x003AC2F0
	public static float Noise3_ImproveXZ(long seed, double x, double y, double z)
	{
		double num = x + z;
		double num2 = num * -0.211324865405187;
		double num3 = y * 0.577350269189626;
		double xr = x + num2 + num3;
		double zr = z + num2 + num3;
		double yr = num * -0.577350269189626 + num3;
		return OpenSimplex2S.Noise3_UnrotatedBase(seed, xr, yr, zr);
	}

	// Token: 0x060093D2 RID: 37842 RVA: 0x003AE13C File Offset: 0x003AC33C
	public static float Noise3_Fallback(long seed, double x, double y, double z)
	{
		double num = 0.6666666666666666 * (x + y + z);
		double xr = num - x;
		double yr = num - y;
		double zr = num - z;
		return OpenSimplex2S.Noise3_UnrotatedBase(seed, xr, yr, zr);
	}

	// Token: 0x060093D3 RID: 37843 RVA: 0x003AE16C File Offset: 0x003AC36C
	[PublicizedFrom(EAccessModifier.Private)]
	public static float Noise3_UnrotatedBase(long seed, double xr, double yr, double zr)
	{
		int num = OpenSimplex2S.FastFloor(xr);
		int num2 = OpenSimplex2S.FastFloor(yr);
		int num3 = OpenSimplex2S.FastFloor(zr);
		float num4 = (float)(xr - (double)num);
		float num5 = (float)(yr - (double)num2);
		float num6 = (float)(zr - (double)num3);
		long num7 = (long)num * 5910200641878280303L;
		long num8 = (long)num2 * 6452764530575939509L;
		long num9 = (long)num3 * 6614699811220273867L;
		long seed2 = seed ^ -5968755714895566377L;
		int num10 = (int)(-0.5f - num4);
		int num11 = (int)(-0.5f - num5);
		int num12 = (int)(-0.5f - num6);
		float num13 = num4 + (float)num10;
		float num14 = num5 + (float)num11;
		float num15 = num6 + (float)num12;
		float num16 = 0.75f - num13 * num13 - num14 * num14 - num15 * num15;
		float num17 = num16 * num16 * (num16 * num16) * OpenSimplex2S.Grad(seed, num7 + ((long)num10 & 5910200641878280303L), num8 + ((long)num11 & 6452764530575939509L), num9 + ((long)num12 & 6614699811220273867L), num13, num14, num15);
		float num18 = num4 - 0.5f;
		float num19 = num5 - 0.5f;
		float num20 = num6 - 0.5f;
		float num21 = 0.75f - num18 * num18 - num19 * num19 - num20 * num20;
		num17 += num21 * num21 * (num21 * num21) * OpenSimplex2S.Grad(seed2, num7 + 5910200641878280303L, num8 + 6452764530575939509L, num9 + 6614699811220273867L, num18, num19, num20);
		float num22 = (float)((num10 | 1) << 1) * num18;
		float num23 = (float)((num11 | 1) << 1) * num19;
		float num24 = (float)((num12 | 1) << 1) * num20;
		float num25 = (float)(-2 - (num10 << 2)) * num18 - 1f;
		float num26 = (float)(-2 - (num11 << 2)) * num19 - 1f;
		float num27 = (float)(-2 - (num12 << 2)) * num20 - 1f;
		bool flag = false;
		float num28 = num22 + num16;
		if (num28 > 0f)
		{
			float dx = num13 - (float)(num10 | 1);
			float dy = num14;
			float dz = num15;
			num17 += num28 * num28 * (num28 * num28) * OpenSimplex2S.Grad(seed, num7 + ((long)(~(long)num10) & 5910200641878280303L), num8 + ((long)num11 & 6452764530575939509L), num9 + ((long)num12 & 6614699811220273867L), dx, dy, dz);
		}
		else
		{
			float num29 = num23 + num24 + num16;
			if (num29 > 0f)
			{
				float dx2 = num13;
				float dy2 = num14 - (float)(num11 | 1);
				float dz2 = num15 - (float)(num12 | 1);
				num17 += num29 * num29 * (num29 * num29) * OpenSimplex2S.Grad(seed, num7 + ((long)num10 & 5910200641878280303L), num8 + ((long)(~(long)num11) & 6452764530575939509L), num9 + ((long)(~(long)num12) & 6614699811220273867L), dx2, dy2, dz2);
			}
			float num30 = num25 + num21;
			if (num30 > 0f)
			{
				float dx3 = (float)(num10 | 1) + num18;
				float dy3 = num19;
				float dz3 = num20;
				num17 += num30 * num30 * (num30 * num30) * OpenSimplex2S.Grad(seed2, num7 + ((long)num10 & -6626342789952991010L), num8 + 6452764530575939509L, num9 + 6614699811220273867L, dx3, dy3, dz3);
				flag = true;
			}
		}
		bool flag2 = false;
		float num31 = num23 + num16;
		if (num31 > 0f)
		{
			float dx4 = num13;
			float dy4 = num14 - (float)(num11 | 1);
			float dz4 = num15;
			num17 += num31 * num31 * (num31 * num31) * OpenSimplex2S.Grad(seed, num7 + ((long)num10 & 5910200641878280303L), num8 + ((long)(~(long)num11) & 6452764530575939509L), num9 + ((long)num12 & 6614699811220273867L), dx4, dy4, dz4);
		}
		else
		{
			float num32 = num22 + num24 + num16;
			if (num32 > 0f)
			{
				float dx5 = num13 - (float)(num10 | 1);
				float dy5 = num14;
				float dz5 = num15 - (float)(num12 | 1);
				num17 += num32 * num32 * (num32 * num32) * OpenSimplex2S.Grad(seed, num7 + ((long)(~(long)num10) & 5910200641878280303L), num8 + ((long)num11 & 6452764530575939509L), num9 + ((long)(~(long)num12) & 6614699811220273867L), dx5, dy5, dz5);
			}
			float num33 = num26 + num21;
			if (num33 > 0f)
			{
				float dx6 = num18;
				float dy6 = (float)(num11 | 1) + num19;
				float dz6 = num20;
				num17 += num33 * num33 * (num33 * num33) * OpenSimplex2S.Grad(seed2, num7 + 5910200641878280303L, num8 + ((long)num11 & -5541215012557672598L), num9 + 6614699811220273867L, dx6, dy6, dz6);
				flag2 = true;
			}
		}
		bool flag3 = false;
		float num34 = num24 + num16;
		if (num34 > 0f)
		{
			float dx7 = num13;
			float dy7 = num14;
			float dz7 = num15 - (float)(num12 | 1);
			num17 += num34 * num34 * (num34 * num34) * OpenSimplex2S.Grad(seed, num7 + ((long)num10 & 5910200641878280303L), num8 + ((long)num11 & 6452764530575939509L), num9 + ((long)(~(long)num12) & 6614699811220273867L), dx7, dy7, dz7);
		}
		else
		{
			float num35 = num22 + num23 + num16;
			if (num35 > 0f)
			{
				float dx8 = num13 - (float)(num10 | 1);
				float dy8 = num14 - (float)(num11 | 1);
				float dz8 = num15;
				num17 += num35 * num35 * (num35 * num35) * OpenSimplex2S.Grad(seed, num7 + ((long)(~(long)num10) & 5910200641878280303L), num8 + ((long)(~(long)num11) & 6452764530575939509L), num9 + ((long)num12 & 6614699811220273867L), dx8, dy8, dz8);
			}
			float num36 = num27 + num21;
			if (num36 > 0f)
			{
				float dx9 = num18;
				float dy9 = num19;
				float dz9 = (float)(num12 | 1) + num20;
				num17 += num36 * num36 * (num36 * num36) * OpenSimplex2S.Grad(seed2, num7 + 5910200641878280303L, num8 + 6452764530575939509L, num9 + ((long)num12 & -5217344451269003882L), dx9, dy9, dz9);
				flag3 = true;
			}
		}
		if (!flag)
		{
			float num37 = num26 + num27 + num21;
			if (num37 > 0f)
			{
				float dx10 = num18;
				float dy10 = (float)(num11 | 1) + num19;
				float dz10 = (float)(num12 | 1) + num20;
				num17 += num37 * num37 * (num37 * num37) * OpenSimplex2S.Grad(seed2, num7 + 5910200641878280303L, num8 + ((long)num11 & -5541215012557672598L), num9 + ((long)num12 & -5217344451269003882L), dx10, dy10, dz10);
			}
		}
		if (!flag2)
		{
			float num38 = num25 + num27 + num21;
			if (num38 > 0f)
			{
				float dx11 = (float)(num10 | 1) + num18;
				float dy11 = num19;
				float dz11 = (float)(num12 | 1) + num20;
				num17 += num38 * num38 * (num38 * num38) * OpenSimplex2S.Grad(seed2, num7 + ((long)num10 & -6626342789952991010L), num8 + 6452764530575939509L, num9 + ((long)num12 & -5217344451269003882L), dx11, dy11, dz11);
			}
		}
		if (!flag3)
		{
			float num39 = num25 + num26 + num21;
			if (num39 > 0f)
			{
				float dx12 = (float)(num10 | 1) + num18;
				float dy12 = (float)(num11 | 1) + num19;
				float dz12 = num20;
				num17 += num39 * num39 * (num39 * num39) * OpenSimplex2S.Grad(seed2, num7 + ((long)num10 & -6626342789952991010L), num8 + ((long)num11 & -5541215012557672598L), num9 + 6614699811220273867L, dx12, dy12, dz12);
			}
		}
		return num17;
	}

	// Token: 0x060093D4 RID: 37844 RVA: 0x003AE8CC File Offset: 0x003ACACC
	public static float Noise4_ImproveXYZ_ImproveXY(long seed, double x, double y, double z, double w)
	{
		double num = x + y;
		double num2 = num * -0.211324865405187;
		double num3 = z * 0.2886751345948129;
		double num4 = w * 1.118033988749894;
		double xs = x + (num3 + num4 + num2);
		double ys = y + (num3 + num4 + num2);
		double zs = num * -0.577350269189626 + (num3 + num4);
		double ws = z * -0.866025403784439 + num4;
		return OpenSimplex2S.Noise4_UnskewedBase(seed, xs, ys, zs, ws);
	}

	// Token: 0x060093D5 RID: 37845 RVA: 0x003AE940 File Offset: 0x003ACB40
	public static float Noise4_ImproveXYZ_ImproveXZ(long seed, double x, double y, double z, double w)
	{
		double num = x + z;
		double num2 = num * -0.211324865405187;
		double num3 = y * 0.2886751345948129;
		double num4 = w * 1.118033988749894;
		double xs = x + (num3 + num4 + num2);
		double zs = z + (num3 + num4 + num2);
		double ys = num * -0.577350269189626 + (num3 + num4);
		double ws = y * -0.866025403784439 + num4;
		return OpenSimplex2S.Noise4_UnskewedBase(seed, xs, ys, zs, ws);
	}

	// Token: 0x060093D6 RID: 37846 RVA: 0x003AE9B4 File Offset: 0x003ACBB4
	public static float Noise4_ImproveXYZ(long seed, double x, double y, double z, double w)
	{
		double num = x + y + z;
		double num2 = w * 1.118033988749894;
		double num3 = num * -0.16666666666666666 + num2;
		double xs = x + num3;
		double ys = y + num3;
		double zs = z + num3;
		double ws = -0.5 * num + num2;
		return OpenSimplex2S.Noise4_UnskewedBase(seed, xs, ys, zs, ws);
	}

	// Token: 0x060093D7 RID: 37847 RVA: 0x003AEA0C File Offset: 0x003ACC0C
	public static float Noise4_Fallback(long seed, double x, double y, double z, double w)
	{
		double num = 0.30901700258255005 * (x + y + z + w);
		double xs = x + num;
		double ys = y + num;
		double zs = z + num;
		double ws = w + num;
		return OpenSimplex2S.Noise4_UnskewedBase(seed, xs, ys, zs, ws);
	}

	// Token: 0x060093D8 RID: 37848 RVA: 0x003AEA4C File Offset: 0x003ACC4C
	[PublicizedFrom(EAccessModifier.Private)]
	public static float Noise4_UnskewedBase(long seed, double xs, double ys, double zs, double ws)
	{
		int num = OpenSimplex2S.FastFloor(xs);
		int num2 = OpenSimplex2S.FastFloor(ys);
		int num3 = OpenSimplex2S.FastFloor(zs);
		int num4 = OpenSimplex2S.FastFloor(ws);
		float num5 = (float)(xs - (double)num);
		float num6 = (float)(ys - (double)num2);
		float num7 = (float)(zs - (double)num3);
		float num8 = (float)(ws - (double)num4);
		float num9 = (num5 + num6 + num7 + num8) * -0.1381966f;
		float num10 = num5 + num9;
		float num11 = num6 + num9;
		float num12 = num7 + num9;
		float num13 = num8 + num9;
		long num14 = (long)num * 5910200641878280303L;
		long num15 = (long)num2 * 6452764530575939509L;
		long num16 = (long)num3 * 6614699811220273867L;
		long num17 = (long)num4 * 6254464313819354443L;
		int num18 = (OpenSimplex2S.FastFloor(xs * 4.0) & 3) | (OpenSimplex2S.FastFloor(ys * 4.0) & 3) << 2 | (OpenSimplex2S.FastFloor(zs * 4.0) & 3) << 4 | (OpenSimplex2S.FastFloor(ws * 4.0) & 3) << 6;
		float num19 = 0f;
		ValueTuple<short, short> valueTuple = OpenSimplex2S.LOOKUP_4D_A[num18];
		int item = (int)valueTuple.Item1;
		int item2 = (int)valueTuple.Item2;
		int num20 = item;
		int num21 = item2;
		for (int i = num20; i < num21; i++)
		{
			OpenSimplex2S.LatticeVertex4D latticeVertex4D = OpenSimplex2S.LOOKUP_4D_B[i];
			float num22 = num10 + latticeVertex4D.dx;
			float num23 = num11 + latticeVertex4D.dy;
			float num24 = num12 + latticeVertex4D.dz;
			float num25 = num13 + latticeVertex4D.dw;
			float num26 = num22 * num22 + num23 * num23 + (num24 * num24 + num25 * num25);
			if (num26 < 0.8f)
			{
				num26 -= 0.8f;
				num26 *= num26;
				num19 += num26 * num26 * OpenSimplex2S.Grad(seed, num14 + latticeVertex4D.xsvp, num15 + latticeVertex4D.ysvp, num16 + latticeVertex4D.zsvp, num17 + latticeVertex4D.wsvp, num22, num23, num24, num25);
			}
		}
		return num19;
	}

	// Token: 0x060093D9 RID: 37849 RVA: 0x003AEC38 File Offset: 0x003ACE38
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Grad(long seed, long xsvp, long ysvp, float dx, float dy)
	{
		long num = (seed ^ xsvp ^ ysvp) * 6026932503003350773L;
		int num2 = (int)(num ^ num >> 58) & 254;
		return OpenSimplex2S.GRADIENTS_2D[num2 | 0] * dx + OpenSimplex2S.GRADIENTS_2D[num2 | 1] * dy;
	}

	// Token: 0x060093DA RID: 37850 RVA: 0x003AEC7C File Offset: 0x003ACE7C
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Grad(long seed, long xrvp, long yrvp, long zrvp, float dx, float dy, float dz)
	{
		long num = (seed ^ xrvp ^ (yrvp ^ zrvp)) * 6026932503003350773L;
		int num2 = (int)(num ^ num >> 58) & 1020;
		return OpenSimplex2S.GRADIENTS_3D[num2 | 0] * dx + OpenSimplex2S.GRADIENTS_3D[num2 | 1] * dy + OpenSimplex2S.GRADIENTS_3D[num2 | 2] * dz;
	}

	// Token: 0x060093DB RID: 37851 RVA: 0x003AECD0 File Offset: 0x003ACED0
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Grad(long seed, long xsvp, long ysvp, long zsvp, long wsvp, float dx, float dy, float dz, float dw)
	{
		long num = (seed ^ (xsvp ^ ysvp) ^ (zsvp ^ wsvp)) * 6026932503003350773L;
		int num2 = (int)(num ^ num >> 57) & 2044;
		return OpenSimplex2S.GRADIENTS_4D[num2 | 0] * dx + OpenSimplex2S.GRADIENTS_4D[num2 | 1] * dy + (OpenSimplex2S.GRADIENTS_4D[num2 | 2] * dz + OpenSimplex2S.GRADIENTS_4D[num2 | 3] * dw);
	}

	// Token: 0x060093DC RID: 37852 RVA: 0x003AED34 File Offset: 0x003ACF34
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int FastFloor(double x)
	{
		int num = (int)x;
		if (x >= (double)num)
		{
			return num;
		}
		return num - 1;
	}

	// Token: 0x060093DD RID: 37853 RVA: 0x003AED50 File Offset: 0x003ACF50
	[PublicizedFrom(EAccessModifier.Private)]
	static OpenSimplex2S()
	{
		float[] array = new float[]
		{
			0.38268343f,
			0.9238795f,
			0.9238795f,
			0.38268343f,
			0.9238795f,
			-0.38268343f,
			0.38268343f,
			-0.9238795f,
			-0.38268343f,
			-0.9238795f,
			-0.9238795f,
			-0.38268343f,
			-0.9238795f,
			0.38268343f,
			-0.38268343f,
			0.9238795f,
			0.13052619f,
			0.9914449f,
			0.6087614f,
			0.7933533f,
			0.7933533f,
			0.6087614f,
			0.9914449f,
			0.13052619f,
			0.9914449f,
			-0.13052619f,
			0.7933533f,
			-0.6087614f,
			0.6087614f,
			-0.7933533f,
			0.13052619f,
			-0.9914449f,
			-0.13052619f,
			-0.9914449f,
			-0.6087614f,
			-0.7933533f,
			-0.7933533f,
			-0.6087614f,
			-0.9914449f,
			-0.13052619f,
			-0.9914449f,
			0.13052619f,
			-0.7933533f,
			0.6087614f,
			-0.6087614f,
			0.7933533f,
			-0.13052619f,
			0.9914449f
		};
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (float)((double)array[i] / 0.05481866495625118);
		}
		int j = 0;
		int num = 0;
		while (j < OpenSimplex2S.GRADIENTS_2D.Length)
		{
			if (num == array.Length)
			{
				num = 0;
			}
			OpenSimplex2S.GRADIENTS_2D[j] = array[num];
			j++;
			num++;
		}
		OpenSimplex2S.GRADIENTS_3D = new float[1024];
		float[] array2 = new float[]
		{
			2.2247448f,
			2.2247448f,
			-1f,
			0f,
			2.2247448f,
			2.2247448f,
			1f,
			0f,
			3.0862665f,
			1.1721513f,
			0f,
			0f,
			1.1721513f,
			3.0862665f,
			0f,
			0f,
			-2.2247448f,
			2.2247448f,
			-1f,
			0f,
			-2.2247448f,
			2.2247448f,
			1f,
			0f,
			-1.1721513f,
			3.0862665f,
			0f,
			0f,
			-3.0862665f,
			1.1721513f,
			0f,
			0f,
			-1f,
			-2.2247448f,
			-2.2247448f,
			0f,
			1f,
			-2.2247448f,
			-2.2247448f,
			0f,
			0f,
			-3.0862665f,
			-1.1721513f,
			0f,
			0f,
			-1.1721513f,
			-3.0862665f,
			0f,
			-1f,
			-2.2247448f,
			2.2247448f,
			0f,
			1f,
			-2.2247448f,
			2.2247448f,
			0f,
			0f,
			-1.1721513f,
			3.0862665f,
			0f,
			0f,
			-3.0862665f,
			1.1721513f,
			0f,
			-2.2247448f,
			-2.2247448f,
			-1f,
			0f,
			-2.2247448f,
			-2.2247448f,
			1f,
			0f,
			-3.0862665f,
			-1.1721513f,
			0f,
			0f,
			-1.1721513f,
			-3.0862665f,
			0f,
			0f,
			-2.2247448f,
			-1f,
			-2.2247448f,
			0f,
			-2.2247448f,
			1f,
			-2.2247448f,
			0f,
			-1.1721513f,
			0f,
			-3.0862665f,
			0f,
			-3.0862665f,
			0f,
			-1.1721513f,
			0f,
			-2.2247448f,
			-1f,
			2.2247448f,
			0f,
			-2.2247448f,
			1f,
			2.2247448f,
			0f,
			-3.0862665f,
			0f,
			1.1721513f,
			0f,
			-1.1721513f,
			0f,
			3.0862665f,
			0f,
			-1f,
			2.2247448f,
			-2.2247448f,
			0f,
			1f,
			2.2247448f,
			-2.2247448f,
			0f,
			0f,
			1.1721513f,
			-3.0862665f,
			0f,
			0f,
			3.0862665f,
			-1.1721513f,
			0f,
			-1f,
			2.2247448f,
			2.2247448f,
			0f,
			1f,
			2.2247448f,
			2.2247448f,
			0f,
			0f,
			3.0862665f,
			1.1721513f,
			0f,
			0f,
			1.1721513f,
			3.0862665f,
			0f,
			2.2247448f,
			-2.2247448f,
			-1f,
			0f,
			2.2247448f,
			-2.2247448f,
			1f,
			0f,
			1.1721513f,
			-3.0862665f,
			0f,
			0f,
			3.0862665f,
			-1.1721513f,
			0f,
			0f,
			2.2247448f,
			-1f,
			-2.2247448f,
			0f,
			2.2247448f,
			1f,
			-2.2247448f,
			0f,
			3.0862665f,
			0f,
			-1.1721513f,
			0f,
			1.1721513f,
			0f,
			-3.0862665f,
			0f,
			2.2247448f,
			-1f,
			2.2247448f,
			0f,
			2.2247448f,
			1f,
			2.2247448f,
			0f,
			1.1721513f,
			0f,
			3.0862665f,
			0f,
			3.0862665f,
			0f,
			1.1721513f,
			0f
		};
		for (int k = 0; k < array2.Length; k++)
		{
			array2[k] = (float)((double)array2[k] / 0.2781926117527186);
		}
		int l = 0;
		int num2 = 0;
		while (l < OpenSimplex2S.GRADIENTS_3D.Length)
		{
			if (num2 == array2.Length)
			{
				num2 = 0;
			}
			OpenSimplex2S.GRADIENTS_3D[l] = array2[num2];
			l++;
			num2++;
		}
		OpenSimplex2S.GRADIENTS_4D = new float[2048];
		float[] array3 = new float[]
		{
			-0.6740059f,
			-0.32398477f,
			-0.32398477f,
			0.5794685f,
			-0.7504884f,
			-0.40046722f,
			0.15296486f,
			0.502986f,
			-0.7504884f,
			0.15296486f,
			-0.40046722f,
			0.502986f,
			-0.8828162f,
			0.08164729f,
			0.08164729f,
			0.4553054f,
			-0.4553054f,
			-0.08164729f,
			-0.08164729f,
			0.8828162f,
			-0.502986f,
			-0.15296486f,
			0.40046722f,
			0.7504884f,
			-0.502986f,
			0.40046722f,
			-0.15296486f,
			0.7504884f,
			-0.5794685f,
			0.32398477f,
			0.32398477f,
			0.6740059f,
			-0.6740059f,
			-0.32398477f,
			0.5794685f,
			-0.32398477f,
			-0.7504884f,
			-0.40046722f,
			0.502986f,
			0.15296486f,
			-0.7504884f,
			0.15296486f,
			0.502986f,
			-0.40046722f,
			-0.8828162f,
			0.08164729f,
			0.4553054f,
			0.08164729f,
			-0.4553054f,
			-0.08164729f,
			0.8828162f,
			-0.08164729f,
			-0.502986f,
			-0.15296486f,
			0.7504884f,
			0.40046722f,
			-0.502986f,
			0.40046722f,
			0.7504884f,
			-0.15296486f,
			-0.5794685f,
			0.32398477f,
			0.6740059f,
			0.32398477f,
			-0.6740059f,
			0.5794685f,
			-0.32398477f,
			-0.32398477f,
			-0.7504884f,
			0.502986f,
			-0.40046722f,
			0.15296486f,
			-0.7504884f,
			0.502986f,
			0.15296486f,
			-0.40046722f,
			-0.8828162f,
			0.4553054f,
			0.08164729f,
			0.08164729f,
			-0.4553054f,
			0.8828162f,
			-0.08164729f,
			-0.08164729f,
			-0.502986f,
			0.7504884f,
			-0.15296486f,
			0.40046722f,
			-0.502986f,
			0.7504884f,
			0.40046722f,
			-0.15296486f,
			-0.5794685f,
			0.6740059f,
			0.32398477f,
			0.32398477f,
			0.5794685f,
			-0.6740059f,
			-0.32398477f,
			-0.32398477f,
			0.502986f,
			-0.7504884f,
			-0.40046722f,
			0.15296486f,
			0.502986f,
			-0.7504884f,
			0.15296486f,
			-0.40046722f,
			0.4553054f,
			-0.8828162f,
			0.08164729f,
			0.08164729f,
			0.8828162f,
			-0.4553054f,
			-0.08164729f,
			-0.08164729f,
			0.7504884f,
			-0.502986f,
			-0.15296486f,
			0.40046722f,
			0.7504884f,
			-0.502986f,
			0.40046722f,
			-0.15296486f,
			0.6740059f,
			-0.5794685f,
			0.32398477f,
			0.32398477f,
			-0.753341f,
			-0.3796829f,
			-0.3796829f,
			-0.3796829f,
			-0.78216845f,
			-0.43214726f,
			-0.43214726f,
			0.121284805f,
			-0.78216845f,
			-0.43214726f,
			0.121284805f,
			-0.43214726f,
			-0.78216845f,
			0.121284805f,
			-0.43214726f,
			-0.43214726f,
			-0.85865086f,
			-0.5086297f,
			0.04480237f,
			0.04480237f,
			-0.85865086f,
			0.04480237f,
			-0.5086297f,
			0.04480237f,
			-0.85865086f,
			0.04480237f,
			0.04480237f,
			-0.5086297f,
			-0.9982829f,
			-0.033819415f,
			-0.033819415f,
			-0.033819415f,
			-0.3796829f,
			-0.753341f,
			-0.3796829f,
			-0.3796829f,
			-0.43214726f,
			-0.78216845f,
			-0.43214726f,
			0.121284805f,
			-0.43214726f,
			-0.78216845f,
			0.121284805f,
			-0.43214726f,
			0.121284805f,
			-0.78216845f,
			-0.43214726f,
			-0.43214726f,
			-0.5086297f,
			-0.85865086f,
			0.04480237f,
			0.04480237f,
			0.04480237f,
			-0.85865086f,
			-0.5086297f,
			0.04480237f,
			0.04480237f,
			-0.85865086f,
			0.04480237f,
			-0.5086297f,
			-0.033819415f,
			-0.9982829f,
			-0.033819415f,
			-0.033819415f,
			-0.3796829f,
			-0.3796829f,
			-0.753341f,
			-0.3796829f,
			-0.43214726f,
			-0.43214726f,
			-0.78216845f,
			0.121284805f,
			-0.43214726f,
			0.121284805f,
			-0.78216845f,
			-0.43214726f,
			0.121284805f,
			-0.43214726f,
			-0.78216845f,
			-0.43214726f,
			-0.5086297f,
			0.04480237f,
			-0.85865086f,
			0.04480237f,
			0.04480237f,
			-0.5086297f,
			-0.85865086f,
			0.04480237f,
			0.04480237f,
			0.04480237f,
			-0.85865086f,
			-0.5086297f,
			-0.033819415f,
			-0.033819415f,
			-0.9982829f,
			-0.033819415f,
			-0.3796829f,
			-0.3796829f,
			-0.3796829f,
			-0.753341f,
			-0.43214726f,
			-0.43214726f,
			0.121284805f,
			-0.78216845f,
			-0.43214726f,
			0.121284805f,
			-0.43214726f,
			-0.78216845f,
			0.121284805f,
			-0.43214726f,
			-0.43214726f,
			-0.78216845f,
			-0.5086297f,
			0.04480237f,
			0.04480237f,
			-0.85865086f,
			0.04480237f,
			-0.5086297f,
			0.04480237f,
			-0.85865086f,
			0.04480237f,
			0.04480237f,
			-0.5086297f,
			-0.85865086f,
			-0.033819415f,
			-0.033819415f,
			-0.033819415f,
			-0.9982829f,
			-0.32398477f,
			-0.6740059f,
			-0.32398477f,
			0.5794685f,
			-0.40046722f,
			-0.7504884f,
			0.15296486f,
			0.502986f,
			0.15296486f,
			-0.7504884f,
			-0.40046722f,
			0.502986f,
			0.08164729f,
			-0.8828162f,
			0.08164729f,
			0.4553054f,
			-0.08164729f,
			-0.4553054f,
			-0.08164729f,
			0.8828162f,
			-0.15296486f,
			-0.502986f,
			0.40046722f,
			0.7504884f,
			0.40046722f,
			-0.502986f,
			-0.15296486f,
			0.7504884f,
			0.32398477f,
			-0.5794685f,
			0.32398477f,
			0.6740059f,
			-0.32398477f,
			-0.32398477f,
			-0.6740059f,
			0.5794685f,
			-0.40046722f,
			0.15296486f,
			-0.7504884f,
			0.502986f,
			0.15296486f,
			-0.40046722f,
			-0.7504884f,
			0.502986f,
			0.08164729f,
			0.08164729f,
			-0.8828162f,
			0.4553054f,
			-0.08164729f,
			-0.08164729f,
			-0.4553054f,
			0.8828162f,
			-0.15296486f,
			0.40046722f,
			-0.502986f,
			0.7504884f,
			0.40046722f,
			-0.15296486f,
			-0.502986f,
			0.7504884f,
			0.32398477f,
			0.32398477f,
			-0.5794685f,
			0.6740059f,
			-0.32398477f,
			-0.6740059f,
			0.5794685f,
			-0.32398477f,
			-0.40046722f,
			-0.7504884f,
			0.502986f,
			0.15296486f,
			0.15296486f,
			-0.7504884f,
			0.502986f,
			-0.40046722f,
			0.08164729f,
			-0.8828162f,
			0.4553054f,
			0.08164729f,
			-0.08164729f,
			-0.4553054f,
			0.8828162f,
			-0.08164729f,
			-0.15296486f,
			-0.502986f,
			0.7504884f,
			0.40046722f,
			0.40046722f,
			-0.502986f,
			0.7504884f,
			-0.15296486f,
			0.32398477f,
			-0.5794685f,
			0.6740059f,
			0.32398477f,
			-0.32398477f,
			-0.32398477f,
			0.5794685f,
			-0.6740059f,
			-0.40046722f,
			0.15296486f,
			0.502986f,
			-0.7504884f,
			0.15296486f,
			-0.40046722f,
			0.502986f,
			-0.7504884f,
			0.08164729f,
			0.08164729f,
			0.4553054f,
			-0.8828162f,
			-0.08164729f,
			-0.08164729f,
			0.8828162f,
			-0.4553054f,
			-0.15296486f,
			0.40046722f,
			0.7504884f,
			-0.502986f,
			0.40046722f,
			-0.15296486f,
			0.7504884f,
			-0.502986f,
			0.32398477f,
			0.32398477f,
			0.6740059f,
			-0.5794685f,
			-0.32398477f,
			0.5794685f,
			-0.6740059f,
			-0.32398477f,
			-0.40046722f,
			0.502986f,
			-0.7504884f,
			0.15296486f,
			0.15296486f,
			0.502986f,
			-0.7504884f,
			-0.40046722f,
			0.08164729f,
			0.4553054f,
			-0.8828162f,
			0.08164729f,
			-0.08164729f,
			0.8828162f,
			-0.4553054f,
			-0.08164729f,
			-0.15296486f,
			0.7504884f,
			-0.502986f,
			0.40046722f,
			0.40046722f,
			0.7504884f,
			-0.502986f,
			-0.15296486f,
			0.32398477f,
			0.6740059f,
			-0.5794685f,
			0.32398477f,
			-0.32398477f,
			0.5794685f,
			-0.32398477f,
			-0.6740059f,
			-0.40046722f,
			0.502986f,
			0.15296486f,
			-0.7504884f,
			0.15296486f,
			0.502986f,
			-0.40046722f,
			-0.7504884f,
			0.08164729f,
			0.4553054f,
			0.08164729f,
			-0.8828162f,
			-0.08164729f,
			0.8828162f,
			-0.08164729f,
			-0.4553054f,
			-0.15296486f,
			0.7504884f,
			0.40046722f,
			-0.502986f,
			0.40046722f,
			0.7504884f,
			-0.15296486f,
			-0.502986f,
			0.32398477f,
			0.6740059f,
			0.32398477f,
			-0.5794685f,
			0.5794685f,
			-0.32398477f,
			-0.6740059f,
			-0.32398477f,
			0.502986f,
			-0.40046722f,
			-0.7504884f,
			0.15296486f,
			0.502986f,
			0.15296486f,
			-0.7504884f,
			-0.40046722f,
			0.4553054f,
			0.08164729f,
			-0.8828162f,
			0.08164729f,
			0.8828162f,
			-0.08164729f,
			-0.4553054f,
			-0.08164729f,
			0.7504884f,
			-0.15296486f,
			-0.502986f,
			0.40046722f,
			0.7504884f,
			0.40046722f,
			-0.502986f,
			-0.15296486f,
			0.6740059f,
			0.32398477f,
			-0.5794685f,
			0.32398477f,
			0.5794685f,
			-0.32398477f,
			-0.32398477f,
			-0.6740059f,
			0.502986f,
			-0.40046722f,
			0.15296486f,
			-0.7504884f,
			0.502986f,
			0.15296486f,
			-0.40046722f,
			-0.7504884f,
			0.4553054f,
			0.08164729f,
			0.08164729f,
			-0.8828162f,
			0.8828162f,
			-0.08164729f,
			-0.08164729f,
			-0.4553054f,
			0.7504884f,
			-0.15296486f,
			0.40046722f,
			-0.502986f,
			0.7504884f,
			0.40046722f,
			-0.15296486f,
			-0.502986f,
			0.6740059f,
			0.32398477f,
			0.32398477f,
			-0.5794685f,
			0.033819415f,
			0.033819415f,
			0.033819415f,
			0.9982829f,
			-0.04480237f,
			-0.04480237f,
			0.5086297f,
			0.85865086f,
			-0.04480237f,
			0.5086297f,
			-0.04480237f,
			0.85865086f,
			-0.121284805f,
			0.43214726f,
			0.43214726f,
			0.78216845f,
			0.5086297f,
			-0.04480237f,
			-0.04480237f,
			0.85865086f,
			0.43214726f,
			-0.121284805f,
			0.43214726f,
			0.78216845f,
			0.43214726f,
			0.43214726f,
			-0.121284805f,
			0.78216845f,
			0.3796829f,
			0.3796829f,
			0.3796829f,
			0.753341f,
			0.033819415f,
			0.033819415f,
			0.9982829f,
			0.033819415f,
			-0.04480237f,
			0.04480237f,
			0.85865086f,
			0.5086297f,
			-0.04480237f,
			0.5086297f,
			0.85865086f,
			-0.04480237f,
			-0.121284805f,
			0.43214726f,
			0.78216845f,
			0.43214726f,
			0.5086297f,
			-0.04480237f,
			0.85865086f,
			-0.04480237f,
			0.43214726f,
			-0.121284805f,
			0.78216845f,
			0.43214726f,
			0.43214726f,
			0.43214726f,
			0.78216845f,
			-0.121284805f,
			0.3796829f,
			0.3796829f,
			0.753341f,
			0.3796829f,
			0.033819415f,
			0.9982829f,
			0.033819415f,
			0.033819415f,
			-0.04480237f,
			0.85865086f,
			-0.04480237f,
			0.5086297f,
			-0.04480237f,
			0.85865086f,
			0.5086297f,
			-0.04480237f,
			-0.121284805f,
			0.78216845f,
			0.43214726f,
			0.43214726f,
			0.5086297f,
			0.85865086f,
			-0.04480237f,
			-0.04480237f,
			0.43214726f,
			0.78216845f,
			-0.121284805f,
			0.43214726f,
			0.43214726f,
			0.78216845f,
			0.43214726f,
			-0.121284805f,
			0.3796829f,
			0.753341f,
			0.3796829f,
			0.3796829f,
			0.9982829f,
			0.033819415f,
			0.033819415f,
			0.033819415f,
			0.85865086f,
			-0.04480237f,
			-0.04480237f,
			0.5086297f,
			0.85865086f,
			-0.04480237f,
			0.5086297f,
			-0.04480237f,
			0.78216845f,
			-0.121284805f,
			0.43214726f,
			0.43214726f,
			0.85865086f,
			0.5086297f,
			-0.04480237f,
			-0.04480237f,
			0.78216845f,
			0.43214726f,
			-0.121284805f,
			0.43214726f,
			0.78216845f,
			0.43214726f,
			0.43214726f,
			-0.121284805f,
			0.753341f,
			0.3796829f,
			0.3796829f,
			0.3796829f
		};
		for (int m = 0; m < array3.Length; m++)
		{
			array3[m] = (float)((double)array3[m] / 0.11127401889945551);
		}
		int n = 0;
		int num3 = 0;
		while (n < OpenSimplex2S.GRADIENTS_4D.Length)
		{
			if (num3 == array3.Length)
			{
				num3 = 0;
			}
			OpenSimplex2S.GRADIENTS_4D[n] = array3[num3];
			n++;
			num3++;
		}
		int[][] array4 = new int[][]
		{
			new int[]
			{
				21,
				69,
				81,
				84,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170
			},
			new int[]
			{
				21,
				69,
				81,
				85,
				86,
				89,
				90,
				101,
				102,
				106,
				149,
				150,
				154,
				166,
				170
			},
			new int[]
			{
				1,
				5,
				17,
				21,
				65,
				69,
				81,
				85,
				86,
				90,
				102,
				106,
				150,
				154,
				166,
				170
			},
			new int[]
			{
				1,
				21,
				22,
				69,
				70,
				81,
				82,
				85,
				86,
				90,
				102,
				106,
				150,
				154,
				166,
				170,
				171
			},
			new int[]
			{
				21,
				69,
				84,
				85,
				86,
				89,
				90,
				101,
				105,
				106,
				149,
				153,
				154,
				169,
				170
			},
			new int[]
			{
				5,
				21,
				69,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				149,
				150,
				153,
				154,
				170
			},
			new int[]
			{
				5,
				21,
				69,
				85,
				86,
				89,
				90,
				102,
				106,
				150,
				154,
				170
			},
			new int[]
			{
				5,
				21,
				22,
				69,
				70,
				85,
				86,
				89,
				90,
				102,
				106,
				150,
				154,
				170,
				171
			},
			new int[]
			{
				4,
				5,
				20,
				21,
				68,
				69,
				84,
				85,
				89,
				90,
				105,
				106,
				153,
				154,
				169,
				170
			},
			new int[]
			{
				5,
				21,
				69,
				85,
				86,
				89,
				90,
				105,
				106,
				153,
				154,
				170
			},
			new int[]
			{
				5,
				21,
				69,
				85,
				86,
				89,
				90,
				106,
				154,
				170
			},
			new int[]
			{
				5,
				21,
				22,
				69,
				70,
				85,
				86,
				89,
				90,
				91,
				106,
				154,
				170,
				171
			},
			new int[]
			{
				4,
				21,
				25,
				69,
				73,
				84,
				85,
				88,
				89,
				90,
				105,
				106,
				153,
				154,
				169,
				170,
				174
			},
			new int[]
			{
				5,
				21,
				25,
				69,
				73,
				85,
				86,
				89,
				90,
				105,
				106,
				153,
				154,
				170,
				174
			},
			new int[]
			{
				5,
				21,
				25,
				69,
				73,
				85,
				86,
				89,
				90,
				94,
				106,
				154,
				170,
				174
			},
			new int[]
			{
				5,
				21,
				26,
				69,
				74,
				85,
				86,
				89,
				90,
				91,
				94,
				106,
				154,
				170,
				171,
				174,
				175
			},
			new int[]
			{
				21,
				81,
				84,
				85,
				86,
				89,
				101,
				102,
				105,
				106,
				149,
				165,
				166,
				169,
				170
			},
			new int[]
			{
				17,
				21,
				81,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				149,
				150,
				165,
				166,
				170
			},
			new int[]
			{
				17,
				21,
				81,
				85,
				86,
				90,
				101,
				102,
				106,
				150,
				166,
				170
			},
			new int[]
			{
				17,
				21,
				22,
				81,
				82,
				85,
				86,
				90,
				101,
				102,
				106,
				150,
				166,
				170,
				171
			},
			new int[]
			{
				20,
				21,
				84,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				149,
				153,
				165,
				169,
				170
			},
			new int[]
			{
				21,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				149,
				154,
				166,
				169,
				170
			},
			new int[]
			{
				21,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				150,
				154,
				166,
				170,
				171
			},
			new int[]
			{
				21,
				22,
				85,
				86,
				90,
				102,
				106,
				107,
				150,
				154,
				166,
				170,
				171
			},
			new int[]
			{
				20,
				21,
				84,
				85,
				89,
				90,
				101,
				105,
				106,
				153,
				169,
				170
			},
			new int[]
			{
				21,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				153,
				154,
				169,
				170,
				174
			},
			new int[]
			{
				21,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				154,
				170
			},
			new int[]
			{
				21,
				22,
				85,
				86,
				89,
				90,
				102,
				106,
				107,
				154,
				170,
				171
			},
			new int[]
			{
				20,
				21,
				25,
				84,
				85,
				88,
				89,
				90,
				101,
				105,
				106,
				153,
				169,
				170,
				174
			},
			new int[]
			{
				21,
				25,
				85,
				89,
				90,
				105,
				106,
				110,
				153,
				154,
				169,
				170,
				174
			},
			new int[]
			{
				21,
				25,
				85,
				86,
				89,
				90,
				105,
				106,
				110,
				154,
				170,
				174
			},
			new int[]
			{
				21,
				26,
				85,
				86,
				89,
				90,
				106,
				107,
				110,
				154,
				170,
				171,
				174,
				175
			},
			new int[]
			{
				16,
				17,
				20,
				21,
				80,
				81,
				84,
				85,
				101,
				102,
				105,
				106,
				165,
				166,
				169,
				170
			},
			new int[]
			{
				17,
				21,
				81,
				85,
				86,
				101,
				102,
				105,
				106,
				165,
				166,
				170
			},
			new int[]
			{
				17,
				21,
				81,
				85,
				86,
				101,
				102,
				106,
				166,
				170
			},
			new int[]
			{
				17,
				21,
				22,
				81,
				82,
				85,
				86,
				101,
				102,
				103,
				106,
				166,
				170,
				171
			},
			new int[]
			{
				20,
				21,
				84,
				85,
				89,
				101,
				102,
				105,
				106,
				165,
				169,
				170
			},
			new int[]
			{
				21,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				165,
				166,
				169,
				170,
				186
			},
			new int[]
			{
				21,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				166,
				170
			},
			new int[]
			{
				21,
				22,
				85,
				86,
				90,
				101,
				102,
				106,
				107,
				166,
				170,
				171
			},
			new int[]
			{
				20,
				21,
				84,
				85,
				89,
				101,
				105,
				106,
				169,
				170
			},
			new int[]
			{
				21,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				169,
				170
			},
			new int[]
			{
				21,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				170
			},
			new int[]
			{
				21,
				22,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				107,
				170,
				171
			},
			new int[]
			{
				20,
				21,
				25,
				84,
				85,
				88,
				89,
				101,
				105,
				106,
				109,
				169,
				170,
				174
			},
			new int[]
			{
				21,
				25,
				85,
				89,
				90,
				101,
				105,
				106,
				110,
				169,
				170,
				174
			},
			new int[]
			{
				21,
				25,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				110,
				170,
				174
			},
			new int[]
			{
				21,
				85,
				86,
				89,
				90,
				102,
				105,
				106,
				107,
				110,
				154,
				170,
				171,
				174,
				175
			},
			new int[]
			{
				16,
				21,
				37,
				81,
				84,
				85,
				97,
				100,
				101,
				102,
				105,
				106,
				165,
				166,
				169,
				170,
				186
			},
			new int[]
			{
				17,
				21,
				37,
				81,
				85,
				86,
				97,
				101,
				102,
				105,
				106,
				165,
				166,
				170,
				186
			},
			new int[]
			{
				17,
				21,
				37,
				81,
				85,
				86,
				97,
				101,
				102,
				106,
				118,
				166,
				170,
				186
			},
			new int[]
			{
				17,
				21,
				38,
				81,
				85,
				86,
				98,
				101,
				102,
				103,
				106,
				118,
				166,
				170,
				171,
				186,
				187
			},
			new int[]
			{
				20,
				21,
				37,
				84,
				85,
				89,
				100,
				101,
				102,
				105,
				106,
				165,
				169,
				170,
				186
			},
			new int[]
			{
				21,
				37,
				85,
				101,
				102,
				105,
				106,
				122,
				165,
				166,
				169,
				170,
				186
			},
			new int[]
			{
				21,
				37,
				85,
				86,
				101,
				102,
				105,
				106,
				122,
				166,
				170,
				186
			},
			new int[]
			{
				21,
				38,
				85,
				86,
				101,
				102,
				106,
				107,
				122,
				166,
				170,
				171,
				186,
				187
			},
			new int[]
			{
				20,
				21,
				37,
				84,
				85,
				89,
				100,
				101,
				105,
				106,
				121,
				169,
				170,
				186
			},
			new int[]
			{
				21,
				37,
				85,
				89,
				101,
				102,
				105,
				106,
				122,
				169,
				170,
				186
			},
			new int[]
			{
				21,
				37,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				122,
				170,
				186
			},
			new int[]
			{
				21,
				85,
				86,
				90,
				101,
				102,
				105,
				106,
				107,
				122,
				166,
				170,
				171,
				186,
				187
			},
			new int[]
			{
				20,
				21,
				41,
				84,
				85,
				89,
				101,
				104,
				105,
				106,
				109,
				121,
				169,
				170,
				174,
				186,
				190
			},
			new int[]
			{
				21,
				41,
				85,
				89,
				101,
				105,
				106,
				110,
				122,
				169,
				170,
				174,
				186,
				190
			},
			new int[]
			{
				21,
				85,
				89,
				90,
				101,
				102,
				105,
				106,
				110,
				122,
				169,
				170,
				174,
				186,
				190
			},
			new int[]
			{
				21,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				107,
				110,
				122,
				170,
				171,
				174,
				186,
				191
			},
			new int[]
			{
				69,
				81,
				84,
				85,
				86,
				89,
				101,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170
			},
			new int[]
			{
				65,
				69,
				81,
				85,
				86,
				89,
				90,
				101,
				102,
				149,
				150,
				153,
				154,
				165,
				166,
				170
			},
			new int[]
			{
				65,
				69,
				81,
				85,
				86,
				90,
				102,
				149,
				150,
				154,
				166,
				170
			},
			new int[]
			{
				65,
				69,
				70,
				81,
				82,
				85,
				86,
				90,
				102,
				149,
				150,
				154,
				166,
				170,
				171
			},
			new int[]
			{
				68,
				69,
				84,
				85,
				86,
				89,
				90,
				101,
				105,
				149,
				150,
				153,
				154,
				165,
				169,
				170
			},
			new int[]
			{
				69,
				85,
				86,
				89,
				90,
				101,
				106,
				149,
				150,
				153,
				154,
				166,
				169,
				170
			},
			new int[]
			{
				69,
				85,
				86,
				89,
				90,
				102,
				106,
				149,
				150,
				153,
				154,
				166,
				170,
				171
			},
			new int[]
			{
				69,
				70,
				85,
				86,
				90,
				102,
				106,
				150,
				154,
				155,
				166,
				170,
				171
			},
			new int[]
			{
				68,
				69,
				84,
				85,
				89,
				90,
				105,
				149,
				153,
				154,
				169,
				170
			},
			new int[]
			{
				69,
				85,
				86,
				89,
				90,
				105,
				106,
				149,
				150,
				153,
				154,
				169,
				170,
				174
			},
			new int[]
			{
				69,
				85,
				86,
				89,
				90,
				106,
				149,
				150,
				153,
				154,
				170
			},
			new int[]
			{
				69,
				70,
				85,
				86,
				89,
				90,
				106,
				150,
				154,
				155,
				170,
				171
			},
			new int[]
			{
				68,
				69,
				73,
				84,
				85,
				88,
				89,
				90,
				105,
				149,
				153,
				154,
				169,
				170,
				174
			},
			new int[]
			{
				69,
				73,
				85,
				89,
				90,
				105,
				106,
				153,
				154,
				158,
				169,
				170,
				174
			},
			new int[]
			{
				69,
				73,
				85,
				86,
				89,
				90,
				106,
				153,
				154,
				158,
				170,
				174
			},
			new int[]
			{
				69,
				74,
				85,
				86,
				89,
				90,
				106,
				154,
				155,
				158,
				170,
				171,
				174,
				175
			},
			new int[]
			{
				80,
				81,
				84,
				85,
				86,
				89,
				101,
				102,
				105,
				149,
				150,
				153,
				165,
				166,
				169,
				170
			},
			new int[]
			{
				81,
				85,
				86,
				89,
				101,
				102,
				106,
				149,
				150,
				154,
				165,
				166,
				169,
				170
			},
			new int[]
			{
				81,
				85,
				86,
				90,
				101,
				102,
				106,
				149,
				150,
				154,
				165,
				166,
				170,
				171
			},
			new int[]
			{
				81,
				82,
				85,
				86,
				90,
				102,
				106,
				150,
				154,
				166,
				167,
				170,
				171
			},
			new int[]
			{
				84,
				85,
				86,
				89,
				101,
				105,
				106,
				149,
				153,
				154,
				165,
				166,
				169,
				170
			},
			new int[]
			{
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170
			},
			new int[]
			{
				21,
				69,
				81,
				85,
				86,
				89,
				90,
				101,
				102,
				106,
				149,
				150,
				154,
				166,
				170,
				171
			},
			new int[]
			{
				85,
				86,
				90,
				102,
				106,
				150,
				154,
				166,
				170,
				171
			},
			new int[]
			{
				84,
				85,
				89,
				90,
				101,
				105,
				106,
				149,
				153,
				154,
				165,
				169,
				170,
				174
			},
			new int[]
			{
				21,
				69,
				84,
				85,
				86,
				89,
				90,
				101,
				105,
				106,
				149,
				153,
				154,
				169,
				170,
				174
			},
			new int[]
			{
				21,
				69,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				149,
				150,
				153,
				154,
				166,
				169,
				170,
				171,
				174
			},
			new int[]
			{
				85,
				86,
				89,
				90,
				102,
				106,
				150,
				154,
				166,
				170,
				171
			},
			new int[]
			{
				84,
				85,
				88,
				89,
				90,
				105,
				106,
				153,
				154,
				169,
				170,
				173,
				174
			},
			new int[]
			{
				85,
				89,
				90,
				105,
				106,
				153,
				154,
				169,
				170,
				174
			},
			new int[]
			{
				85,
				86,
				89,
				90,
				105,
				106,
				153,
				154,
				169,
				170,
				174
			},
			new int[]
			{
				85,
				86,
				89,
				90,
				106,
				154,
				170,
				171,
				174,
				175
			},
			new int[]
			{
				80,
				81,
				84,
				85,
				101,
				102,
				105,
				149,
				165,
				166,
				169,
				170
			},
			new int[]
			{
				81,
				85,
				86,
				101,
				102,
				105,
				106,
				149,
				150,
				165,
				166,
				169,
				170,
				186
			},
			new int[]
			{
				81,
				85,
				86,
				101,
				102,
				106,
				149,
				150,
				165,
				166,
				170
			},
			new int[]
			{
				81,
				82,
				85,
				86,
				101,
				102,
				106,
				150,
				166,
				167,
				170,
				171
			},
			new int[]
			{
				84,
				85,
				89,
				101,
				102,
				105,
				106,
				149,
				153,
				165,
				166,
				169,
				170,
				186
			},
			new int[]
			{
				21,
				81,
				84,
				85,
				86,
				89,
				101,
				102,
				105,
				106,
				149,
				165,
				166,
				169,
				170,
				186
			},
			new int[]
			{
				21,
				81,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				149,
				150,
				154,
				165,
				166,
				169,
				170,
				171,
				186
			},
			new int[]
			{
				85,
				86,
				90,
				101,
				102,
				106,
				150,
				154,
				166,
				170,
				171
			},
			new int[]
			{
				84,
				85,
				89,
				101,
				105,
				106,
				149,
				153,
				165,
				169,
				170
			},
			new int[]
			{
				21,
				84,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				149,
				153,
				154,
				165,
				166,
				169,
				170,
				174,
				186
			},
			new int[]
			{
				21,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				154,
				166,
				169,
				170
			},
			new int[]
			{
				21,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				150,
				154,
				166,
				170,
				171
			},
			new int[]
			{
				84,
				85,
				88,
				89,
				101,
				105,
				106,
				153,
				169,
				170,
				173,
				174
			},
			new int[]
			{
				85,
				89,
				90,
				101,
				105,
				106,
				153,
				154,
				169,
				170,
				174
			},
			new int[]
			{
				21,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				153,
				154,
				169,
				170,
				174
			},
			new int[]
			{
				21,
				85,
				86,
				89,
				90,
				102,
				105,
				106,
				154,
				170,
				171,
				174,
				175
			},
			new int[]
			{
				80,
				81,
				84,
				85,
				97,
				100,
				101,
				102,
				105,
				149,
				165,
				166,
				169,
				170,
				186
			},
			new int[]
			{
				81,
				85,
				97,
				101,
				102,
				105,
				106,
				165,
				166,
				169,
				170,
				182,
				186
			},
			new int[]
			{
				81,
				85,
				86,
				97,
				101,
				102,
				106,
				165,
				166,
				170,
				182,
				186
			},
			new int[]
			{
				81,
				85,
				86,
				98,
				101,
				102,
				106,
				166,
				167,
				170,
				171,
				182,
				186,
				187
			},
			new int[]
			{
				84,
				85,
				100,
				101,
				102,
				105,
				106,
				165,
				166,
				169,
				170,
				185,
				186
			},
			new int[]
			{
				85,
				101,
				102,
				105,
				106,
				165,
				166,
				169,
				170,
				186
			},
			new int[]
			{
				85,
				86,
				101,
				102,
				105,
				106,
				165,
				166,
				169,
				170,
				186
			},
			new int[]
			{
				85,
				86,
				101,
				102,
				106,
				166,
				170,
				171,
				186,
				187
			},
			new int[]
			{
				84,
				85,
				89,
				100,
				101,
				105,
				106,
				165,
				169,
				170,
				185,
				186
			},
			new int[]
			{
				85,
				89,
				101,
				102,
				105,
				106,
				165,
				166,
				169,
				170,
				186
			},
			new int[]
			{
				21,
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				165,
				166,
				169,
				170,
				186
			},
			new int[]
			{
				21,
				85,
				86,
				90,
				101,
				102,
				105,
				106,
				166,
				170,
				171,
				186,
				187
			},
			new int[]
			{
				84,
				85,
				89,
				101,
				104,
				105,
				106,
				169,
				170,
				173,
				174,
				185,
				186,
				190
			},
			new int[]
			{
				85,
				89,
				101,
				105,
				106,
				169,
				170,
				174,
				186,
				190
			},
			new int[]
			{
				21,
				85,
				89,
				90,
				101,
				102,
				105,
				106,
				169,
				170,
				174,
				186,
				190
			},
			new int[]
			{
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				170,
				171,
				174,
				186,
				191
			},
			new int[]
			{
				64,
				65,
				68,
				69,
				80,
				81,
				84,
				85,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170
			},
			new int[]
			{
				65,
				69,
				81,
				85,
				86,
				149,
				150,
				153,
				154,
				165,
				166,
				170
			},
			new int[]
			{
				65,
				69,
				81,
				85,
				86,
				149,
				150,
				154,
				166,
				170
			},
			new int[]
			{
				65,
				69,
				70,
				81,
				82,
				85,
				86,
				149,
				150,
				151,
				154,
				166,
				170,
				171
			},
			new int[]
			{
				68,
				69,
				84,
				85,
				89,
				149,
				150,
				153,
				154,
				165,
				169,
				170
			},
			new int[]
			{
				69,
				85,
				86,
				89,
				90,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				234
			},
			new int[]
			{
				69,
				85,
				86,
				89,
				90,
				149,
				150,
				153,
				154,
				166,
				170
			},
			new int[]
			{
				69,
				70,
				85,
				86,
				90,
				149,
				150,
				154,
				155,
				166,
				170,
				171
			},
			new int[]
			{
				68,
				69,
				84,
				85,
				89,
				149,
				153,
				154,
				169,
				170
			},
			new int[]
			{
				69,
				85,
				86,
				89,
				90,
				149,
				150,
				153,
				154,
				169,
				170
			},
			new int[]
			{
				69,
				85,
				86,
				89,
				90,
				149,
				150,
				153,
				154,
				170
			},
			new int[]
			{
				69,
				70,
				85,
				86,
				89,
				90,
				149,
				150,
				153,
				154,
				155,
				170,
				171
			},
			new int[]
			{
				68,
				69,
				73,
				84,
				85,
				88,
				89,
				149,
				153,
				154,
				157,
				169,
				170,
				174
			},
			new int[]
			{
				69,
				73,
				85,
				89,
				90,
				149,
				153,
				154,
				158,
				169,
				170,
				174
			},
			new int[]
			{
				69,
				73,
				85,
				86,
				89,
				90,
				149,
				150,
				153,
				154,
				158,
				170,
				174
			},
			new int[]
			{
				69,
				85,
				86,
				89,
				90,
				106,
				150,
				153,
				154,
				155,
				158,
				170,
				171,
				174,
				175
			},
			new int[]
			{
				80,
				81,
				84,
				85,
				101,
				149,
				150,
				153,
				165,
				166,
				169,
				170
			},
			new int[]
			{
				81,
				85,
				86,
				101,
				102,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				234
			},
			new int[]
			{
				81,
				85,
				86,
				101,
				102,
				149,
				150,
				154,
				165,
				166,
				170
			},
			new int[]
			{
				81,
				82,
				85,
				86,
				102,
				149,
				150,
				154,
				166,
				167,
				170,
				171
			},
			new int[]
			{
				84,
				85,
				89,
				101,
				105,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				234
			},
			new int[]
			{
				69,
				81,
				84,
				85,
				86,
				89,
				101,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				234
			},
			new int[]
			{
				69,
				81,
				85,
				86,
				89,
				90,
				101,
				102,
				106,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				171,
				234
			},
			new int[]
			{
				85,
				86,
				90,
				102,
				106,
				149,
				150,
				154,
				166,
				170,
				171
			},
			new int[]
			{
				84,
				85,
				89,
				101,
				105,
				149,
				153,
				154,
				165,
				169,
				170
			},
			new int[]
			{
				69,
				84,
				85,
				86,
				89,
				90,
				101,
				105,
				106,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				174,
				234
			},
			new int[]
			{
				69,
				85,
				86,
				89,
				90,
				106,
				149,
				150,
				153,
				154,
				166,
				169,
				170
			},
			new int[]
			{
				69,
				85,
				86,
				89,
				90,
				102,
				106,
				149,
				150,
				153,
				154,
				166,
				170,
				171
			},
			new int[]
			{
				84,
				85,
				88,
				89,
				105,
				149,
				153,
				154,
				169,
				170,
				173,
				174
			},
			new int[]
			{
				85,
				89,
				90,
				105,
				106,
				149,
				153,
				154,
				169,
				170,
				174
			},
			new int[]
			{
				69,
				85,
				86,
				89,
				90,
				105,
				106,
				149,
				150,
				153,
				154,
				169,
				170,
				174
			},
			new int[]
			{
				69,
				85,
				86,
				89,
				90,
				106,
				150,
				153,
				154,
				170,
				171,
				174,
				175
			},
			new int[]
			{
				80,
				81,
				84,
				85,
				101,
				149,
				165,
				166,
				169,
				170
			},
			new int[]
			{
				81,
				85,
				86,
				101,
				102,
				149,
				150,
				165,
				166,
				169,
				170
			},
			new int[]
			{
				81,
				85,
				86,
				101,
				102,
				149,
				150,
				165,
				166,
				170
			},
			new int[]
			{
				81,
				82,
				85,
				86,
				101,
				102,
				149,
				150,
				165,
				166,
				167,
				170,
				171
			},
			new int[]
			{
				84,
				85,
				89,
				101,
				105,
				149,
				153,
				165,
				166,
				169,
				170
			},
			new int[]
			{
				81,
				84,
				85,
				86,
				89,
				101,
				102,
				105,
				106,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				186,
				234
			},
			new int[]
			{
				81,
				85,
				86,
				101,
				102,
				106,
				149,
				150,
				154,
				165,
				166,
				169,
				170
			},
			new int[]
			{
				81,
				85,
				86,
				90,
				101,
				102,
				106,
				149,
				150,
				154,
				165,
				166,
				170,
				171
			},
			new int[]
			{
				84,
				85,
				89,
				101,
				105,
				149,
				153,
				165,
				169,
				170
			},
			new int[]
			{
				84,
				85,
				89,
				101,
				105,
				106,
				149,
				153,
				154,
				165,
				166,
				169,
				170
			},
			new int[]
			{
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170
			},
			new int[]
			{
				85,
				86,
				89,
				90,
				101,
				102,
				106,
				149,
				150,
				154,
				166,
				169,
				170,
				171
			},
			new int[]
			{
				84,
				85,
				88,
				89,
				101,
				105,
				149,
				153,
				165,
				169,
				170,
				173,
				174
			},
			new int[]
			{
				84,
				85,
				89,
				90,
				101,
				105,
				106,
				149,
				153,
				154,
				165,
				169,
				170,
				174
			},
			new int[]
			{
				85,
				86,
				89,
				90,
				101,
				105,
				106,
				149,
				153,
				154,
				166,
				169,
				170,
				174
			},
			new int[]
			{
				85,
				86,
				89,
				90,
				102,
				105,
				106,
				150,
				153,
				154,
				166,
				169,
				170,
				171,
				174,
				175
			},
			new int[]
			{
				80,
				81,
				84,
				85,
				97,
				100,
				101,
				149,
				165,
				166,
				169,
				170,
				181,
				186
			},
			new int[]
			{
				81,
				85,
				97,
				101,
				102,
				149,
				165,
				166,
				169,
				170,
				182,
				186
			},
			new int[]
			{
				81,
				85,
				86,
				97,
				101,
				102,
				149,
				150,
				165,
				166,
				170,
				182,
				186
			},
			new int[]
			{
				81,
				85,
				86,
				101,
				102,
				106,
				150,
				165,
				166,
				167,
				170,
				171,
				182,
				186,
				187
			},
			new int[]
			{
				84,
				85,
				100,
				101,
				105,
				149,
				165,
				166,
				169,
				170,
				185,
				186
			},
			new int[]
			{
				85,
				101,
				102,
				105,
				106,
				149,
				165,
				166,
				169,
				170,
				186
			},
			new int[]
			{
				81,
				85,
				86,
				101,
				102,
				105,
				106,
				149,
				150,
				165,
				166,
				169,
				170,
				186
			},
			new int[]
			{
				81,
				85,
				86,
				101,
				102,
				106,
				150,
				165,
				166,
				170,
				171,
				186,
				187
			},
			new int[]
			{
				84,
				85,
				89,
				100,
				101,
				105,
				149,
				153,
				165,
				169,
				170,
				185,
				186
			},
			new int[]
			{
				84,
				85,
				89,
				101,
				102,
				105,
				106,
				149,
				153,
				165,
				166,
				169,
				170,
				186
			},
			new int[]
			{
				85,
				86,
				89,
				101,
				102,
				105,
				106,
				149,
				154,
				165,
				166,
				169,
				170,
				186
			},
			new int[]
			{
				85,
				86,
				90,
				101,
				102,
				105,
				106,
				150,
				154,
				165,
				166,
				169,
				170,
				171,
				186,
				187
			},
			new int[]
			{
				84,
				85,
				89,
				101,
				105,
				106,
				153,
				165,
				169,
				170,
				173,
				174,
				185,
				186,
				190
			},
			new int[]
			{
				84,
				85,
				89,
				101,
				105,
				106,
				153,
				165,
				169,
				170,
				174,
				186,
				190
			},
			new int[]
			{
				85,
				89,
				90,
				101,
				102,
				105,
				106,
				153,
				154,
				165,
				166,
				169,
				170,
				174,
				186,
				190
			},
			new int[]
			{
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				154,
				166,
				169,
				170,
				171,
				174,
				186
			},
			new int[]
			{
				64,
				69,
				81,
				84,
				85,
				133,
				145,
				148,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				234
			},
			new int[]
			{
				65,
				69,
				81,
				85,
				86,
				133,
				145,
				149,
				150,
				153,
				154,
				165,
				166,
				170,
				234
			},
			new int[]
			{
				65,
				69,
				81,
				85,
				86,
				133,
				145,
				149,
				150,
				154,
				166,
				170,
				214,
				234
			},
			new int[]
			{
				65,
				69,
				81,
				85,
				86,
				134,
				146,
				149,
				150,
				151,
				154,
				166,
				170,
				171,
				214,
				234,
				235
			},
			new int[]
			{
				68,
				69,
				84,
				85,
				89,
				133,
				148,
				149,
				150,
				153,
				154,
				165,
				169,
				170,
				234
			},
			new int[]
			{
				69,
				85,
				133,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				218,
				234
			},
			new int[]
			{
				69,
				85,
				86,
				133,
				149,
				150,
				153,
				154,
				166,
				170,
				218,
				234
			},
			new int[]
			{
				69,
				85,
				86,
				134,
				149,
				150,
				154,
				155,
				166,
				170,
				171,
				218,
				234,
				235
			},
			new int[]
			{
				68,
				69,
				84,
				85,
				89,
				133,
				148,
				149,
				153,
				154,
				169,
				170,
				217,
				234
			},
			new int[]
			{
				69,
				85,
				89,
				133,
				149,
				150,
				153,
				154,
				169,
				170,
				218,
				234
			},
			new int[]
			{
				69,
				85,
				86,
				89,
				90,
				133,
				149,
				150,
				153,
				154,
				170,
				218,
				234
			},
			new int[]
			{
				69,
				85,
				86,
				90,
				149,
				150,
				153,
				154,
				155,
				166,
				170,
				171,
				218,
				234,
				235
			},
			new int[]
			{
				68,
				69,
				84,
				85,
				89,
				137,
				149,
				152,
				153,
				154,
				157,
				169,
				170,
				174,
				217,
				234,
				238
			},
			new int[]
			{
				69,
				85,
				89,
				137,
				149,
				153,
				154,
				158,
				169,
				170,
				174,
				218,
				234,
				238
			},
			new int[]
			{
				69,
				85,
				89,
				90,
				149,
				150,
				153,
				154,
				158,
				169,
				170,
				174,
				218,
				234,
				238
			},
			new int[]
			{
				69,
				85,
				86,
				89,
				90,
				149,
				150,
				153,
				154,
				155,
				158,
				170,
				171,
				174,
				218,
				234,
				239
			},
			new int[]
			{
				80,
				81,
				84,
				85,
				101,
				145,
				148,
				149,
				150,
				153,
				165,
				166,
				169,
				170,
				234
			},
			new int[]
			{
				81,
				85,
				145,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				230,
				234
			},
			new int[]
			{
				81,
				85,
				86,
				145,
				149,
				150,
				154,
				165,
				166,
				170,
				230,
				234
			},
			new int[]
			{
				81,
				85,
				86,
				146,
				149,
				150,
				154,
				166,
				167,
				170,
				171,
				230,
				234,
				235
			},
			new int[]
			{
				84,
				85,
				148,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				233,
				234
			},
			new int[]
			{
				85,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				234
			},
			new int[]
			{
				85,
				86,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				234
			},
			new int[]
			{
				85,
				86,
				149,
				150,
				154,
				166,
				170,
				171,
				234,
				235
			},
			new int[]
			{
				84,
				85,
				89,
				148,
				149,
				153,
				154,
				165,
				169,
				170,
				233,
				234
			},
			new int[]
			{
				85,
				89,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				234
			},
			new int[]
			{
				69,
				85,
				86,
				89,
				90,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				234
			},
			new int[]
			{
				69,
				85,
				86,
				90,
				149,
				150,
				153,
				154,
				166,
				170,
				171,
				234,
				235
			},
			new int[]
			{
				84,
				85,
				89,
				149,
				152,
				153,
				154,
				169,
				170,
				173,
				174,
				233,
				234,
				238
			},
			new int[]
			{
				85,
				89,
				149,
				153,
				154,
				169,
				170,
				174,
				234,
				238
			},
			new int[]
			{
				69,
				85,
				89,
				90,
				149,
				150,
				153,
				154,
				169,
				170,
				174,
				234,
				238
			},
			new int[]
			{
				85,
				86,
				89,
				90,
				149,
				150,
				153,
				154,
				170,
				171,
				174,
				234,
				239
			},
			new int[]
			{
				80,
				81,
				84,
				85,
				101,
				145,
				148,
				149,
				165,
				166,
				169,
				170,
				229,
				234
			},
			new int[]
			{
				81,
				85,
				101,
				145,
				149,
				150,
				165,
				166,
				169,
				170,
				230,
				234
			},
			new int[]
			{
				81,
				85,
				86,
				101,
				102,
				145,
				149,
				150,
				165,
				166,
				170,
				230,
				234
			},
			new int[]
			{
				81,
				85,
				86,
				102,
				149,
				150,
				154,
				165,
				166,
				167,
				170,
				171,
				230,
				234,
				235
			},
			new int[]
			{
				84,
				85,
				101,
				148,
				149,
				153,
				165,
				166,
				169,
				170,
				233,
				234
			},
			new int[]
			{
				85,
				101,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				234
			},
			new int[]
			{
				81,
				85,
				86,
				101,
				102,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				234
			},
			new int[]
			{
				81,
				85,
				86,
				102,
				149,
				150,
				154,
				165,
				166,
				170,
				171,
				234,
				235
			},
			new int[]
			{
				84,
				85,
				89,
				101,
				105,
				148,
				149,
				153,
				165,
				169,
				170,
				233,
				234
			},
			new int[]
			{
				84,
				85,
				89,
				101,
				105,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				234
			},
			new int[]
			{
				85,
				86,
				89,
				101,
				106,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				234
			},
			new int[]
			{
				85,
				86,
				90,
				102,
				106,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				171,
				234,
				235
			},
			new int[]
			{
				84,
				85,
				89,
				105,
				149,
				153,
				154,
				165,
				169,
				170,
				173,
				174,
				233,
				234,
				238
			},
			new int[]
			{
				84,
				85,
				89,
				105,
				149,
				153,
				154,
				165,
				169,
				170,
				174,
				234,
				238
			},
			new int[]
			{
				85,
				89,
				90,
				105,
				106,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				174,
				234,
				238
			},
			new int[]
			{
				85,
				86,
				89,
				90,
				106,
				149,
				150,
				153,
				154,
				166,
				169,
				170,
				171,
				174,
				234
			},
			new int[]
			{
				80,
				81,
				84,
				85,
				101,
				149,
				161,
				164,
				165,
				166,
				169,
				170,
				181,
				186,
				229,
				234,
				250
			},
			new int[]
			{
				81,
				85,
				101,
				149,
				161,
				165,
				166,
				169,
				170,
				182,
				186,
				230,
				234,
				250
			},
			new int[]
			{
				81,
				85,
				101,
				102,
				149,
				150,
				165,
				166,
				169,
				170,
				182,
				186,
				230,
				234,
				250
			},
			new int[]
			{
				81,
				85,
				86,
				101,
				102,
				149,
				150,
				165,
				166,
				167,
				170,
				171,
				182,
				186,
				230,
				234,
				251
			},
			new int[]
			{
				84,
				85,
				101,
				149,
				164,
				165,
				166,
				169,
				170,
				185,
				186,
				233,
				234,
				250
			},
			new int[]
			{
				85,
				101,
				149,
				165,
				166,
				169,
				170,
				186,
				234,
				250
			},
			new int[]
			{
				81,
				85,
				101,
				102,
				149,
				150,
				165,
				166,
				169,
				170,
				186,
				234,
				250
			},
			new int[]
			{
				85,
				86,
				101,
				102,
				149,
				150,
				165,
				166,
				170,
				171,
				186,
				234,
				251
			},
			new int[]
			{
				84,
				85,
				101,
				105,
				149,
				153,
				165,
				166,
				169,
				170,
				185,
				186,
				233,
				234,
				250
			},
			new int[]
			{
				84,
				85,
				101,
				105,
				149,
				153,
				165,
				166,
				169,
				170,
				186,
				234,
				250
			},
			new int[]
			{
				85,
				101,
				102,
				105,
				106,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				186,
				234,
				250
			},
			new int[]
			{
				85,
				86,
				101,
				102,
				106,
				149,
				150,
				154,
				165,
				166,
				169,
				170,
				171,
				186,
				234
			},
			new int[]
			{
				84,
				85,
				89,
				101,
				105,
				149,
				153,
				165,
				169,
				170,
				173,
				174,
				185,
				186,
				233,
				234,
				254
			},
			new int[]
			{
				85,
				89,
				101,
				105,
				149,
				153,
				165,
				169,
				170,
				174,
				186,
				234,
				254
			},
			new int[]
			{
				85,
				89,
				101,
				105,
				106,
				149,
				153,
				154,
				165,
				166,
				169,
				170,
				174,
				186,
				234
			},
			new int[]
			{
				85,
				86,
				89,
				90,
				101,
				102,
				105,
				106,
				149,
				150,
				153,
				154,
				165,
				166,
				169,
				170,
				171,
				174,
				186,
				234
			}
		};
		OpenSimplex2S.LatticeVertex4D[] array5 = new OpenSimplex2S.LatticeVertex4D[256];
		for (int num4 = 0; num4 < 256; num4++)
		{
			int xsv = (num4 & 3) - 1;
			int ysv = (num4 >> 2 & 3) - 1;
			int zsv = (num4 >> 4 & 3) - 1;
			int wsv = (num4 >> 6 & 3) - 1;
			array5[num4] = new OpenSimplex2S.LatticeVertex4D(xsv, ysv, zsv, wsv);
		}
		int num5 = 0;
		for (int num6 = 0; num6 < 256; num6++)
		{
			num5 += array4[num6].Length;
		}
		OpenSimplex2S.LOOKUP_4D_A = new ValueTuple<short, short>[256];
		OpenSimplex2S.LOOKUP_4D_B = new OpenSimplex2S.LatticeVertex4D[num5];
		int num7 = 0;
		int num8 = 0;
		while (num7 < 256)
		{
			OpenSimplex2S.LOOKUP_4D_A[num7] = new ValueTuple<short, short>((short)num8, (short)(num8 + array4[num7].Length));
			for (int num9 = 0; num9 < array4[num7].Length; num9++)
			{
				OpenSimplex2S.LOOKUP_4D_B[num8++] = array5[array4[num7][num9]];
			}
			num7++;
		}
	}

	// Token: 0x040070F2 RID: 28914
	[PublicizedFrom(EAccessModifier.Private)]
	public const long PRIME_X = 5910200641878280303L;

	// Token: 0x040070F3 RID: 28915
	[PublicizedFrom(EAccessModifier.Private)]
	public const long PRIME_Y = 6452764530575939509L;

	// Token: 0x040070F4 RID: 28916
	[PublicizedFrom(EAccessModifier.Private)]
	public const long PRIME_Z = 6614699811220273867L;

	// Token: 0x040070F5 RID: 28917
	[PublicizedFrom(EAccessModifier.Private)]
	public const long PRIME_W = 6254464313819354443L;

	// Token: 0x040070F6 RID: 28918
	[PublicizedFrom(EAccessModifier.Private)]
	public const long HASH_MULTIPLIER = 6026932503003350773L;

	// Token: 0x040070F7 RID: 28919
	[PublicizedFrom(EAccessModifier.Private)]
	public const long SEED_FLIP_3D = -5968755714895566377L;

	// Token: 0x040070F8 RID: 28920
	[PublicizedFrom(EAccessModifier.Private)]
	public const double ROOT2OVER2 = 0.7071067811865476;

	// Token: 0x040070F9 RID: 28921
	[PublicizedFrom(EAccessModifier.Private)]
	public const double SKEW_2D = 0.366025403784439;

	// Token: 0x040070FA RID: 28922
	[PublicizedFrom(EAccessModifier.Private)]
	public const double UNSKEW_2D = -0.21132486540518713;

	// Token: 0x040070FB RID: 28923
	[PublicizedFrom(EAccessModifier.Private)]
	public const double ROOT3OVER3 = 0.577350269189626;

	// Token: 0x040070FC RID: 28924
	[PublicizedFrom(EAccessModifier.Private)]
	public const double FALLBACK_ROTATE3 = 0.6666666666666666;

	// Token: 0x040070FD RID: 28925
	[PublicizedFrom(EAccessModifier.Private)]
	public const double ROTATE3_ORTHOGONALIZER = -0.21132486540518713;

	// Token: 0x040070FE RID: 28926
	[PublicizedFrom(EAccessModifier.Private)]
	public const float SKEW_4D = 0.309017f;

	// Token: 0x040070FF RID: 28927
	[PublicizedFrom(EAccessModifier.Private)]
	public const float UNSKEW_4D = -0.1381966f;

	// Token: 0x04007100 RID: 28928
	[PublicizedFrom(EAccessModifier.Private)]
	public const int N_GRADS_2D_EXPONENT = 7;

	// Token: 0x04007101 RID: 28929
	[PublicizedFrom(EAccessModifier.Private)]
	public const int N_GRADS_3D_EXPONENT = 8;

	// Token: 0x04007102 RID: 28930
	[PublicizedFrom(EAccessModifier.Private)]
	public const int N_GRADS_4D_EXPONENT = 9;

	// Token: 0x04007103 RID: 28931
	[PublicizedFrom(EAccessModifier.Private)]
	public const int N_GRADS_2D = 128;

	// Token: 0x04007104 RID: 28932
	[PublicizedFrom(EAccessModifier.Private)]
	public const int N_GRADS_3D = 256;

	// Token: 0x04007105 RID: 28933
	[PublicizedFrom(EAccessModifier.Private)]
	public const int N_GRADS_4D = 512;

	// Token: 0x04007106 RID: 28934
	[PublicizedFrom(EAccessModifier.Private)]
	public const double NORMALIZER_2D = 0.05481866495625118;

	// Token: 0x04007107 RID: 28935
	[PublicizedFrom(EAccessModifier.Private)]
	public const double NORMALIZER_3D = 0.2781926117527186;

	// Token: 0x04007108 RID: 28936
	[PublicizedFrom(EAccessModifier.Private)]
	public const double NORMALIZER_4D = 0.11127401889945551;

	// Token: 0x04007109 RID: 28937
	[PublicizedFrom(EAccessModifier.Private)]
	public const float RSQUARED_2D = 0.6666667f;

	// Token: 0x0400710A RID: 28938
	[PublicizedFrom(EAccessModifier.Private)]
	public const float RSQUARED_3D = 0.75f;

	// Token: 0x0400710B RID: 28939
	[PublicizedFrom(EAccessModifier.Private)]
	public const float RSQUARED_4D = 0.8f;

	// Token: 0x0400710C RID: 28940
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly float[] GRADIENTS_2D = new float[256];

	// Token: 0x0400710D RID: 28941
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly float[] GRADIENTS_3D;

	// Token: 0x0400710E RID: 28942
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly float[] GRADIENTS_4D;

	// Token: 0x0400710F RID: 28943
	[TupleElementNames(new string[]
	{
		"SecondaryIndexStart",
		"SecondaryIndexStop"
	})]
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ValueTuple<short, short>[] LOOKUP_4D_A;

	// Token: 0x04007110 RID: 28944
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly OpenSimplex2S.LatticeVertex4D[] LOOKUP_4D_B;

	// Token: 0x0200126F RID: 4719
	[PublicizedFrom(EAccessModifier.Private)]
	public class LatticeVertex4D
	{
		// Token: 0x060093DE RID: 37854 RVA: 0x003B0754 File Offset: 0x003AE954
		public LatticeVertex4D(int xsv, int ysv, int zsv, int wsv)
		{
			this.xsvp = (long)xsv * 5910200641878280303L;
			this.ysvp = (long)ysv * 6452764530575939509L;
			this.zsvp = (long)zsv * 6614699811220273867L;
			this.wsvp = (long)wsv * 6254464313819354443L;
			float num = (float)(xsv + ysv + zsv + wsv) * -0.1381966f;
			this.dx = (float)(-(float)xsv) - num;
			this.dy = (float)(-(float)ysv) - num;
			this.dz = (float)(-(float)zsv) - num;
			this.dw = (float)(-(float)wsv) - num;
		}

		// Token: 0x04007111 RID: 28945
		public readonly float dx;

		// Token: 0x04007112 RID: 28946
		public readonly float dy;

		// Token: 0x04007113 RID: 28947
		public readonly float dz;

		// Token: 0x04007114 RID: 28948
		public readonly float dw;

		// Token: 0x04007115 RID: 28949
		public readonly long xsvp;

		// Token: 0x04007116 RID: 28950
		public readonly long ysvp;

		// Token: 0x04007117 RID: 28951
		public readonly long zsvp;

		// Token: 0x04007118 RID: 28952
		public readonly long wsvp;
	}
}
