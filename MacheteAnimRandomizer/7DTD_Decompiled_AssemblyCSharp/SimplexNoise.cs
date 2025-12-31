using System;

// Token: 0x02001271 RID: 4721
public class SimplexNoise
{
	// Token: 0x060093E8 RID: 37864 RVA: 0x003B0CEC File Offset: 0x003AEEEC
	public static float noise(float x, float y, float z)
	{
		SimplexNoise.s = (x + y + z) * 0.33333334f;
		SimplexNoise.i = SimplexNoise.fastfloor(x + SimplexNoise.s);
		SimplexNoise.j = SimplexNoise.fastfloor(y + SimplexNoise.s);
		SimplexNoise.k = SimplexNoise.fastfloor(z + SimplexNoise.s);
		SimplexNoise.s = (float)(SimplexNoise.i + SimplexNoise.j + SimplexNoise.k) * 0.16666667f;
		SimplexNoise.u = x - (float)SimplexNoise.i + SimplexNoise.s;
		SimplexNoise.v = y - (float)SimplexNoise.j + SimplexNoise.s;
		SimplexNoise.w = z - (float)SimplexNoise.k + SimplexNoise.s;
		SimplexNoise.A[0] = (SimplexNoise.A[1] = (SimplexNoise.A[2] = 0));
		int num = (SimplexNoise.u >= SimplexNoise.w) ? ((SimplexNoise.u >= SimplexNoise.v) ? 0 : 1) : ((SimplexNoise.v >= SimplexNoise.w) ? 1 : 2);
		int num2 = (SimplexNoise.u < SimplexNoise.w) ? ((SimplexNoise.u < SimplexNoise.v) ? 0 : 1) : ((SimplexNoise.v < SimplexNoise.w) ? 1 : 2);
		return SimplexNoise.K(num) + SimplexNoise.K(3 - num - num2) + SimplexNoise.K(num2) + SimplexNoise.K(0);
	}

	// Token: 0x060093E9 RID: 37865 RVA: 0x003B0E2B File Offset: 0x003AF02B
	[PublicizedFrom(EAccessModifier.Private)]
	public static int fastfloor(float n)
	{
		if (n <= 0f)
		{
			return (int)n - 1;
		}
		return (int)n;
	}

	// Token: 0x060093EA RID: 37866 RVA: 0x003B0E3C File Offset: 0x003AF03C
	[PublicizedFrom(EAccessModifier.Private)]
	public static float K(int a)
	{
		SimplexNoise.s = (float)(SimplexNoise.A[0] + SimplexNoise.A[1] + SimplexNoise.A[2]) * 0.16666667f;
		float num = SimplexNoise.u - (float)SimplexNoise.A[0] + SimplexNoise.s;
		float num2 = SimplexNoise.v - (float)SimplexNoise.A[1] + SimplexNoise.s;
		float num3 = SimplexNoise.w - (float)SimplexNoise.A[2] + SimplexNoise.s;
		float num4 = 0.6f - num * num - num2 * num2 - num3 * num3;
		int num5 = SimplexNoise.shuffle(SimplexNoise.i + SimplexNoise.A[0], SimplexNoise.j + SimplexNoise.A[1], SimplexNoise.k + SimplexNoise.A[2]);
		SimplexNoise.A[a]++;
		if (num4 < 0f)
		{
			return 0f;
		}
		int num6 = num5 >> 5 & 1;
		int num7 = num5 >> 4 & 1;
		int num8 = num5 >> 3 & 1;
		int num9 = num5 >> 2 & 1;
		int num10 = num5 & 3;
		float num11 = (num10 == 1) ? num : ((num10 == 2) ? num2 : num3);
		float num12 = (num10 == 1) ? num2 : ((num10 == 2) ? num3 : num);
		float num13 = (num10 == 1) ? num3 : ((num10 == 2) ? num : num2);
		num11 = ((num6 == num8) ? (-num11) : num11);
		num12 = ((num6 == num7) ? (-num12) : num12);
		num13 = ((num6 != (num7 ^ num8)) ? (-num13) : num13);
		num4 *= num4;
		return 8f * num4 * num4 * (num11 + ((num10 == 0) ? (num12 + num13) : ((num9 == 0) ? num12 : num13)));
	}

	// Token: 0x060093EB RID: 37867 RVA: 0x003B0FC0 File Offset: 0x003AF1C0
	[PublicizedFrom(EAccessModifier.Private)]
	public static int shuffle(int i, int j, int k)
	{
		return SimplexNoise.b(i, j, k, 0) + SimplexNoise.b(j, k, i, 1) + SimplexNoise.b(k, i, j, 2) + SimplexNoise.b(i, j, k, 3) + SimplexNoise.b(j, k, i, 4) + SimplexNoise.b(k, i, j, 5) + SimplexNoise.b(i, j, k, 6) + SimplexNoise.b(j, k, i, 7);
	}

	// Token: 0x060093EC RID: 37868 RVA: 0x003B101C File Offset: 0x003AF21C
	[PublicizedFrom(EAccessModifier.Private)]
	public static int b(int i, int j, int k, int B)
	{
		return SimplexNoise.T[SimplexNoise.b(i, B) << 2 | SimplexNoise.b(j, B) << 1 | SimplexNoise.b(k, B)];
	}

	// Token: 0x060093ED RID: 37869 RVA: 0x003B103F File Offset: 0x003AF23F
	[PublicizedFrom(EAccessModifier.Private)]
	public static int b(int N, int B)
	{
		return N >> B & 1;
	}

	// Token: 0x0400711D RID: 28957
	[PublicizedFrom(EAccessModifier.Private)]
	public static int i;

	// Token: 0x0400711E RID: 28958
	[PublicizedFrom(EAccessModifier.Private)]
	public static int j;

	// Token: 0x0400711F RID: 28959
	[PublicizedFrom(EAccessModifier.Private)]
	public static int k;

	// Token: 0x04007120 RID: 28960
	[PublicizedFrom(EAccessModifier.Private)]
	public static int[] A = new int[3];

	// Token: 0x04007121 RID: 28961
	[PublicizedFrom(EAccessModifier.Private)]
	public static float u;

	// Token: 0x04007122 RID: 28962
	[PublicizedFrom(EAccessModifier.Private)]
	public static float v;

	// Token: 0x04007123 RID: 28963
	[PublicizedFrom(EAccessModifier.Private)]
	public static float w;

	// Token: 0x04007124 RID: 28964
	[PublicizedFrom(EAccessModifier.Private)]
	public static float s;

	// Token: 0x04007125 RID: 28965
	[PublicizedFrom(EAccessModifier.Private)]
	public const float onethird = 0.33333334f;

	// Token: 0x04007126 RID: 28966
	[PublicizedFrom(EAccessModifier.Private)]
	public const float onesixth = 0.16666667f;

	// Token: 0x04007127 RID: 28967
	[PublicizedFrom(EAccessModifier.Private)]
	public static int[] T = new int[]
	{
		21,
		56,
		50,
		44,
		13,
		19,
		7,
		42
	};
}
