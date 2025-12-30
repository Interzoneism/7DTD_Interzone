using System;
using System.Runtime.CompilerServices;

// Token: 0x0200126D RID: 4717
public static class OpenSimplex2
{
	// Token: 0x060093BB RID: 37819 RVA: 0x003ACF40 File Offset: 0x003AB140
	public static float Noise2(long seed, double x, double y)
	{
		double num = 0.366025403784439 * (x + y);
		double xs = x + num;
		double ys = y + num;
		return OpenSimplex2.Noise2_UnskewedBase(seed, xs, ys);
	}

	// Token: 0x060093BC RID: 37820 RVA: 0x003ACF6C File Offset: 0x003AB16C
	public static float Noise2_ImproveX(long seed, double x, double y)
	{
		double num = x * 0.7071067811865476;
		double num2 = y * 1.2247448713915896;
		return OpenSimplex2.Noise2_UnskewedBase(seed, num2 + num, num2 - num);
	}

	// Token: 0x060093BD RID: 37821 RVA: 0x003ACFA0 File Offset: 0x003AB1A0
	[PublicizedFrom(EAccessModifier.Private)]
	public static float Noise2_UnskewedBase(long seed, double xs, double ys)
	{
		int num = OpenSimplex2.FastFloor(xs);
		int num2 = OpenSimplex2.FastFloor(ys);
		float num3 = (float)(xs - (double)num);
		float num4 = (float)(ys - (double)num2);
		long num5 = (long)num * 5910200641878280303L;
		long num6 = (long)num2 * 6452764530575939509L;
		float num7 = (num3 + num4) * -0.21132487f;
		float num8 = num3 + num7;
		float num9 = num4 + num7;
		float num10 = 0f;
		float num11 = 0.5f - num8 * num8 - num9 * num9;
		if (num11 > 0f)
		{
			num10 = num11 * num11 * (num11 * num11) * OpenSimplex2.Grad(seed, num5, num6, num8, num9);
		}
		float num12 = -3.1547005f * num7 + (-0.6666667f + num11);
		if (num12 > 0f)
		{
			float dx = num8 - 0.57735026f;
			float dy = num9 - 0.57735026f;
			num10 += num12 * num12 * (num12 * num12) * OpenSimplex2.Grad(seed, num5 + 5910200641878280303L, num6 + 6452764530575939509L, dx, dy);
		}
		if (num9 > num8)
		{
			float num13 = num8 - -0.21132487f;
			float num14 = num9 - 0.7886751f;
			float num15 = 0.5f - num13 * num13 - num14 * num14;
			if (num15 > 0f)
			{
				num10 += num15 * num15 * (num15 * num15) * OpenSimplex2.Grad(seed, num5, num6 + 6452764530575939509L, num13, num14);
			}
		}
		else
		{
			float num16 = num8 - 0.7886751f;
			float num17 = num9 - -0.21132487f;
			float num18 = 0.5f - num16 * num16 - num17 * num17;
			if (num18 > 0f)
			{
				num10 += num18 * num18 * (num18 * num18) * OpenSimplex2.Grad(seed, num5 + 5910200641878280303L, num6, num16, num17);
			}
		}
		return num10;
	}

	// Token: 0x060093BE RID: 37822 RVA: 0x003AD154 File Offset: 0x003AB354
	public static float Noise3_ImproveXY(long seed, double x, double y, double z)
	{
		double num = x + y;
		double num2 = num * -0.21132486540518713;
		double num3 = z * 0.577350269189626;
		double xr = x + num2 + num3;
		double yr = y + num2 + num3;
		double zr = num * -0.577350269189626 + num3;
		return OpenSimplex2.Noise3_UnrotatedBase(seed, xr, yr, zr);
	}

	// Token: 0x060093BF RID: 37823 RVA: 0x003AD1A0 File Offset: 0x003AB3A0
	public static float Noise3_ImproveXZ(long seed, double x, double y, double z)
	{
		double num = x + z;
		double num2 = num * -0.21132486540518713;
		double num3 = y * 0.577350269189626;
		double xr = x + num2 + num3;
		double zr = z + num2 + num3;
		double yr = num * -0.577350269189626 + num3;
		return OpenSimplex2.Noise3_UnrotatedBase(seed, xr, yr, zr);
	}

	// Token: 0x060093C0 RID: 37824 RVA: 0x003AD1EC File Offset: 0x003AB3EC
	public static float Noise3_Fallback(long seed, double x, double y, double z)
	{
		double num = 0.6666666666666666 * (x + y + z);
		double xr = num - x;
		double yr = num - y;
		double zr = num - z;
		return OpenSimplex2.Noise3_UnrotatedBase(seed, xr, yr, zr);
	}

	// Token: 0x060093C1 RID: 37825 RVA: 0x003AD21C File Offset: 0x003AB41C
	[PublicizedFrom(EAccessModifier.Private)]
	public static float Noise3_UnrotatedBase(long seed, double xr, double yr, double zr)
	{
		int num = OpenSimplex2.FastRound(xr);
		int num2 = OpenSimplex2.FastRound(yr);
		int num3 = OpenSimplex2.FastRound(zr);
		float num4 = (float)(xr - (double)num);
		float num5 = (float)(yr - (double)num2);
		float num6 = (float)(zr - (double)num3);
		int num7 = (int)(-1f - num4) | 1;
		int num8 = (int)(-1f - num5) | 1;
		int num9 = (int)(-1f - num6) | 1;
		float num10 = (float)num7 * -num4;
		float num11 = (float)num8 * -num5;
		float num12 = (float)num9 * -num6;
		long num13 = (long)num * 5910200641878280303L;
		long num14 = (long)num2 * 6452764530575939509L;
		long num15 = (long)num3 * 6614699811220273867L;
		float num16 = 0f;
		float num17 = 0.6f - num4 * num4 - (num5 * num5 + num6 * num6);
		int num18 = 0;
		for (;;)
		{
			if (num17 > 0f)
			{
				num16 += num17 * num17 * (num17 * num17) * OpenSimplex2.Grad(seed, num13, num14, num15, num4, num5, num6);
			}
			if (num10 >= num11 && num10 >= num12)
			{
				float num19 = num17 + num10 + num10;
				if (num19 > 1f)
				{
					num19 -= 1f;
					num16 += num19 * num19 * (num19 * num19) * OpenSimplex2.Grad(seed, num13 - (long)num7 * 5910200641878280303L, num14, num15, num4 + (float)num7, num5, num6);
				}
			}
			else if (num11 > num10 && num11 >= num12)
			{
				float num20 = num17 + num11 + num11;
				if (num20 > 1f)
				{
					num20 -= 1f;
					num16 += num20 * num20 * (num20 * num20) * OpenSimplex2.Grad(seed, num13, num14 - (long)num8 * 6452764530575939509L, num15, num4, num5 + (float)num8, num6);
				}
			}
			else
			{
				float num21 = num17 + num12 + num12;
				if (num21 > 1f)
				{
					num21 -= 1f;
					num16 += num21 * num21 * (num21 * num21) * OpenSimplex2.Grad(seed, num13, num14, num15 - (long)num9 * 6614699811220273867L, num4, num5, num6 + (float)num9);
				}
			}
			if (num18 == 1)
			{
				break;
			}
			num10 = 0.5f - num10;
			num11 = 0.5f - num11;
			num12 = 0.5f - num12;
			num4 = (float)num7 * num10;
			num5 = (float)num8 * num11;
			num6 = (float)num9 * num12;
			num17 += 0.75f - num10 - (num11 + num12);
			num13 += ((long)(num7 >> 1) & 5910200641878280303L);
			num14 += ((long)(num8 >> 1) & 6452764530575939509L);
			num15 += ((long)(num9 >> 1) & 6614699811220273867L);
			num7 = -num7;
			num8 = -num8;
			num9 = -num9;
			seed ^= -5968755714895566377L;
			num18++;
		}
		return num16;
	}

	// Token: 0x060093C2 RID: 37826 RVA: 0x003AD4D8 File Offset: 0x003AB6D8
	public static float Noise4_ImproveXYZ_ImproveXY(long seed, double x, double y, double z, double w)
	{
		double num = x + y;
		double num2 = num * -0.211324865405187;
		double num3 = z * 0.2886751345948129;
		double num4 = w * 0.2236067977499788;
		double xs = x + (num3 + num4 + num2);
		double ys = y + (num3 + num4 + num2);
		double zs = num * -0.577350269189626 + (num3 + num4);
		double ws = z * -0.866025403784439 + num4;
		return OpenSimplex2.Noise4_UnskewedBase(seed, xs, ys, zs, ws);
	}

	// Token: 0x060093C3 RID: 37827 RVA: 0x003AD54C File Offset: 0x003AB74C
	public static float Noise4_ImproveXYZ_ImproveXZ(long seed, double x, double y, double z, double w)
	{
		double num = x + z;
		double num2 = num * -0.211324865405187;
		double num3 = y * 0.2886751345948129;
		double num4 = w * 0.2236067977499788;
		double xs = x + (num3 + num4 + num2);
		double zs = z + (num3 + num4 + num2);
		double ys = num * -0.577350269189626 + (num3 + num4);
		double ws = y * -0.866025403784439 + num4;
		return OpenSimplex2.Noise4_UnskewedBase(seed, xs, ys, zs, ws);
	}

	// Token: 0x060093C4 RID: 37828 RVA: 0x003AD5C0 File Offset: 0x003AB7C0
	public static float Noise4_ImproveXYZ(long seed, double x, double y, double z, double w)
	{
		double num = x + y + z;
		double num2 = w * 0.2236067977499788;
		double num3 = num * -0.16666666666666666 + num2;
		double xs = x + num3;
		double ys = y + num3;
		double zs = z + num3;
		double ws = -0.5 * num + num2;
		return OpenSimplex2.Noise4_UnskewedBase(seed, xs, ys, zs, ws);
	}

	// Token: 0x060093C5 RID: 37829 RVA: 0x003AD618 File Offset: 0x003AB818
	public static float Noise4_Fallback(long seed, double x, double y, double z, double w)
	{
		double num = -0.13819660246372223 * (x + y + z + w);
		double xs = x + num;
		double ys = y + num;
		double zs = z + num;
		double ws = w + num;
		return OpenSimplex2.Noise4_UnskewedBase(seed, xs, ys, zs, ws);
	}

	// Token: 0x060093C6 RID: 37830 RVA: 0x003AD658 File Offset: 0x003AB858
	[PublicizedFrom(EAccessModifier.Private)]
	public static float Noise4_UnskewedBase(long seed, double xs, double ys, double zs, double ws)
	{
		int num = OpenSimplex2.FastFloor(xs);
		int num2 = OpenSimplex2.FastFloor(ys);
		int num3 = OpenSimplex2.FastFloor(zs);
		int num4 = OpenSimplex2.FastFloor(ws);
		float num5 = (float)(xs - (double)num);
		float num6 = (float)(ys - (double)num2);
		float num7 = (float)(zs - (double)num3);
		float num8 = (float)(ws - (double)num4);
		float num9 = num5 + num6 + (num7 + num8);
		int num10 = (int)((double)num9 * 1.25);
		seed += (long)num10 * 1045921697555224141L;
		float num11 = (float)num10 * -0.2f;
		num5 += num11;
		num6 += num11;
		num7 += num11;
		num8 += num11;
		float num12 = (num9 + num11 * 4f) * 0.309017f;
		long num13 = (long)num * 5910200641878280303L;
		long num14 = (long)num2 * 6452764530575939509L;
		long num15 = (long)num3 * 6614699811220273867L;
		long num16 = (long)num4 * 6254464313819354443L;
		float num17 = 0f;
		int num18 = 0;
		for (;;)
		{
			double num19 = 1.0 + (double)num12 * -3.2360678915486614;
			if (num5 >= num6 && num5 >= num7 && num5 >= num8 && (double)num5 >= num19)
			{
				num13 += 5910200641878280303L;
				num5 -= 1f;
				num12 -= 0.309017f;
			}
			else if (num6 > num5 && num6 >= num7 && num6 >= num8 && (double)num6 >= num19)
			{
				num14 += 6452764530575939509L;
				num6 -= 1f;
				num12 -= 0.309017f;
			}
			else if (num7 > num5 && num7 > num6 && num7 >= num8 && (double)num7 >= num19)
			{
				num15 += 6614699811220273867L;
				num7 -= 1f;
				num12 -= 0.309017f;
			}
			else if (num8 > num5 && num8 > num6 && num8 > num7 && (double)num8 >= num19)
			{
				num16 += 6254464313819354443L;
				num8 -= 1f;
				num12 -= 0.309017f;
			}
			float num20 = num5 + num12;
			float num21 = num6 + num12;
			float num22 = num7 + num12;
			float num23 = num8 + num12;
			float num24 = num20 * num20 + num21 * num21 + (num22 * num22 + num23 * num23);
			if (num24 < 0.6f)
			{
				num24 -= 0.6f;
				num24 *= num24;
				num17 += num24 * num24 * OpenSimplex2.Grad(seed, num13, num14, num15, num16, num20, num21, num22, num23);
			}
			if (num18 == 4)
			{
				break;
			}
			num5 += 0.2f;
			num6 += 0.2f;
			num7 += 0.2f;
			num8 += 0.2f;
			num12 += 0.2472136f;
			seed -= 1045921697555224141L;
			if (num18 == num10)
			{
				num13 -= 5910200641878280303L;
				num14 -= 6452764530575939509L;
				num15 -= 6614699811220273867L;
				num16 -= 6254464313819354443L;
				seed += 5229608487776120705L;
			}
			num18++;
		}
		return num17;
	}

	// Token: 0x060093C7 RID: 37831 RVA: 0x003AD964 File Offset: 0x003ABB64
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Grad(long seed, long xsvp, long ysvp, float dx, float dy)
	{
		long num = (seed ^ xsvp ^ ysvp) * 6026932503003350773L;
		int num2 = (int)(num ^ num >> 58) & 254;
		return OpenSimplex2.GRADIENTS_2D[num2 | 0] * dx + OpenSimplex2.GRADIENTS_2D[num2 | 1] * dy;
	}

	// Token: 0x060093C8 RID: 37832 RVA: 0x003AD9A8 File Offset: 0x003ABBA8
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Grad(long seed, long xrvp, long yrvp, long zrvp, float dx, float dy, float dz)
	{
		long num = (seed ^ xrvp ^ (yrvp ^ zrvp)) * 6026932503003350773L;
		int num2 = (int)(num ^ num >> 58) & 1020;
		return OpenSimplex2.GRADIENTS_3D[num2 | 0] * dx + OpenSimplex2.GRADIENTS_3D[num2 | 1] * dy + OpenSimplex2.GRADIENTS_3D[num2 | 2] * dz;
	}

	// Token: 0x060093C9 RID: 37833 RVA: 0x003AD9FC File Offset: 0x003ABBFC
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Grad(long seed, long xsvp, long ysvp, long zsvp, long wsvp, float dx, float dy, float dz, float dw)
	{
		long num = (seed ^ (xsvp ^ ysvp) ^ (zsvp ^ wsvp)) * 6026932503003350773L;
		int num2 = (int)(num ^ num >> 57) & 2044;
		return OpenSimplex2.GRADIENTS_4D[num2 | 0] * dx + OpenSimplex2.GRADIENTS_4D[num2 | 1] * dy + (OpenSimplex2.GRADIENTS_4D[num2 | 2] * dz + OpenSimplex2.GRADIENTS_4D[num2 | 3] * dw);
	}

	// Token: 0x060093CA RID: 37834 RVA: 0x003ADA60 File Offset: 0x003ABC60
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

	// Token: 0x060093CB RID: 37835 RVA: 0x003ADA7A File Offset: 0x003ABC7A
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int FastRound(double x)
	{
		if (x >= 0.0)
		{
			return (int)(x + 0.5);
		}
		return (int)(x - 0.5);
	}

	// Token: 0x060093CC RID: 37836 RVA: 0x003ADAA4 File Offset: 0x003ABCA4
	[PublicizedFrom(EAccessModifier.Private)]
	static OpenSimplex2()
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
			array[i] = (float)((double)array[i] / 0.01001634121365712);
		}
		int j = 0;
		int num = 0;
		while (j < OpenSimplex2.GRADIENTS_2D.Length)
		{
			if (num == array.Length)
			{
				num = 0;
			}
			OpenSimplex2.GRADIENTS_2D[j] = array[num];
			j++;
			num++;
		}
		OpenSimplex2.GRADIENTS_3D = new float[1024];
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
			array2[k] = (float)((double)array2[k] / 0.07969837668935331);
		}
		int l = 0;
		int num2 = 0;
		while (l < OpenSimplex2.GRADIENTS_3D.Length)
		{
			if (num2 == array2.Length)
			{
				num2 = 0;
			}
			OpenSimplex2.GRADIENTS_3D[l] = array2[num2];
			l++;
			num2++;
		}
		OpenSimplex2.GRADIENTS_4D = new float[2048];
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
			array3[m] = (float)((double)array3[m] / 0.0220065933241897);
		}
		int n = 0;
		int num3 = 0;
		while (n < OpenSimplex2.GRADIENTS_4D.Length)
		{
			if (num3 == array3.Length)
			{
				num3 = 0;
			}
			OpenSimplex2.GRADIENTS_4D[n] = array3[num3];
			n++;
			num3++;
		}
	}

	// Token: 0x040070D3 RID: 28883
	[PublicizedFrom(EAccessModifier.Private)]
	public const long PRIME_X = 5910200641878280303L;

	// Token: 0x040070D4 RID: 28884
	[PublicizedFrom(EAccessModifier.Private)]
	public const long PRIME_Y = 6452764530575939509L;

	// Token: 0x040070D5 RID: 28885
	[PublicizedFrom(EAccessModifier.Private)]
	public const long PRIME_Z = 6614699811220273867L;

	// Token: 0x040070D6 RID: 28886
	[PublicizedFrom(EAccessModifier.Private)]
	public const long PRIME_W = 6254464313819354443L;

	// Token: 0x040070D7 RID: 28887
	[PublicizedFrom(EAccessModifier.Private)]
	public const long HASH_MULTIPLIER = 6026932503003350773L;

	// Token: 0x040070D8 RID: 28888
	[PublicizedFrom(EAccessModifier.Private)]
	public const long SEED_FLIP_3D = -5968755714895566377L;

	// Token: 0x040070D9 RID: 28889
	[PublicizedFrom(EAccessModifier.Private)]
	public const long SEED_OFFSET_4D = 1045921697555224141L;

	// Token: 0x040070DA RID: 28890
	[PublicizedFrom(EAccessModifier.Private)]
	public const double ROOT2OVER2 = 0.7071067811865476;

	// Token: 0x040070DB RID: 28891
	[PublicizedFrom(EAccessModifier.Private)]
	public const double SKEW_2D = 0.366025403784439;

	// Token: 0x040070DC RID: 28892
	[PublicizedFrom(EAccessModifier.Private)]
	public const double UNSKEW_2D = -0.21132486540518713;

	// Token: 0x040070DD RID: 28893
	[PublicizedFrom(EAccessModifier.Private)]
	public const double ROOT3OVER3 = 0.577350269189626;

	// Token: 0x040070DE RID: 28894
	[PublicizedFrom(EAccessModifier.Private)]
	public const double FALLBACK_ROTATE_3D = 0.6666666666666666;

	// Token: 0x040070DF RID: 28895
	[PublicizedFrom(EAccessModifier.Private)]
	public const double ROTATE_3D_ORTHOGONALIZER = -0.21132486540518713;

	// Token: 0x040070E0 RID: 28896
	[PublicizedFrom(EAccessModifier.Private)]
	public const float SKEW_4D = -0.1381966f;

	// Token: 0x040070E1 RID: 28897
	[PublicizedFrom(EAccessModifier.Private)]
	public const float UNSKEW_4D = 0.309017f;

	// Token: 0x040070E2 RID: 28898
	[PublicizedFrom(EAccessModifier.Private)]
	public const float LATTICE_STEP_4D = 0.2f;

	// Token: 0x040070E3 RID: 28899
	[PublicizedFrom(EAccessModifier.Private)]
	public const int N_GRADS_2D_EXPONENT = 7;

	// Token: 0x040070E4 RID: 28900
	[PublicizedFrom(EAccessModifier.Private)]
	public const int N_GRADS_3D_EXPONENT = 8;

	// Token: 0x040070E5 RID: 28901
	[PublicizedFrom(EAccessModifier.Private)]
	public const int N_GRADS_4D_EXPONENT = 9;

	// Token: 0x040070E6 RID: 28902
	[PublicizedFrom(EAccessModifier.Private)]
	public const int N_GRADS_2D = 128;

	// Token: 0x040070E7 RID: 28903
	[PublicizedFrom(EAccessModifier.Private)]
	public const int N_GRADS_3D = 256;

	// Token: 0x040070E8 RID: 28904
	[PublicizedFrom(EAccessModifier.Private)]
	public const int N_GRADS_4D = 512;

	// Token: 0x040070E9 RID: 28905
	[PublicizedFrom(EAccessModifier.Private)]
	public const double NORMALIZER_2D = 0.01001634121365712;

	// Token: 0x040070EA RID: 28906
	[PublicizedFrom(EAccessModifier.Private)]
	public const double NORMALIZER_3D = 0.07969837668935331;

	// Token: 0x040070EB RID: 28907
	[PublicizedFrom(EAccessModifier.Private)]
	public const double NORMALIZER_4D = 0.0220065933241897;

	// Token: 0x040070EC RID: 28908
	[PublicizedFrom(EAccessModifier.Private)]
	public const float RSQUARED_2D = 0.5f;

	// Token: 0x040070ED RID: 28909
	[PublicizedFrom(EAccessModifier.Private)]
	public const float RSQUARED_3D = 0.6f;

	// Token: 0x040070EE RID: 28910
	[PublicizedFrom(EAccessModifier.Private)]
	public const float RSQUARED_4D = 0.6f;

	// Token: 0x040070EF RID: 28911
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly float[] GRADIENTS_2D = new float[256];

	// Token: 0x040070F0 RID: 28912
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly float[] GRADIENTS_3D;

	// Token: 0x040070F1 RID: 28913
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly float[] GRADIENTS_4D;
}
