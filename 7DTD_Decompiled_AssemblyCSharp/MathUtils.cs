using System;

// Token: 0x020011C1 RID: 4545
public static class MathUtils
{
	// Token: 0x06008E0D RID: 36365 RVA: 0x0038F44D File Offset: 0x0038D64D
	public static float Clamp(float value, float min, float max)
	{
		if (value < min)
		{
			return min;
		}
		if (value <= max)
		{
			return value;
		}
		return max;
	}

	// Token: 0x06008E0E RID: 36366 RVA: 0x0038F45C File Offset: 0x0038D65C
	public static float Lerp(float a, float b, float x)
	{
		return a + (b - a) * x;
	}

	// Token: 0x06008E0F RID: 36367 RVA: 0x0038F465 File Offset: 0x0038D665
	public static int Min(int a, int b)
	{
		if (a < b)
		{
			return a;
		}
		return b;
	}

	// Token: 0x06008E10 RID: 36368 RVA: 0x0038F46E File Offset: 0x0038D66E
	public static int Min(int a, int b, int c)
	{
		if (a >= c && b >= c)
		{
			return c;
		}
		if (a < b)
		{
			return a;
		}
		return b;
	}

	// Token: 0x06008E11 RID: 36369 RVA: 0x0038F481 File Offset: 0x0038D681
	public static int Min(int a, int b, int c, int d)
	{
		if (a >= d && b >= d && c >= d)
		{
			return d;
		}
		if (a >= c && b >= c)
		{
			return c;
		}
		if (a < b)
		{
			return a;
		}
		return b;
	}

	// Token: 0x06008E12 RID: 36370 RVA: 0x0038F4A2 File Offset: 0x0038D6A2
	public static int Max(int a, int b)
	{
		if (a > b)
		{
			return a;
		}
		return b;
	}

	// Token: 0x06008E13 RID: 36371 RVA: 0x0038F4AB File Offset: 0x0038D6AB
	public static int Max(int a, int b, int c)
	{
		if (a <= c && b <= c)
		{
			return c;
		}
		if (a > b)
		{
			return a;
		}
		return b;
	}

	// Token: 0x06008E14 RID: 36372 RVA: 0x0038F4BE File Offset: 0x0038D6BE
	public static int Max(int a, int b, int c, int d)
	{
		if (a <= d && b <= d && c <= d)
		{
			return d;
		}
		if (a <= c && b <= c)
		{
			return c;
		}
		if (a > b)
		{
			return a;
		}
		return b;
	}

	// Token: 0x06008E15 RID: 36373 RVA: 0x0038F4E0 File Offset: 0x0038D6E0
	public static double RoundToSignificantDigits(double value, int digits)
	{
		if (value == 0.0)
		{
			return 0.0;
		}
		double num = Math.Pow(10.0, Math.Floor(Math.Log10(Math.Abs(value))) + 1.0);
		return num * Math.Round(value / num, digits);
	}

	// Token: 0x06008E16 RID: 36374 RVA: 0x0038F538 File Offset: 0x0038D738
	public static double TruncateToSignificantDigits(double value, int digits)
	{
		if (value == 0.0)
		{
			return 0.0;
		}
		double num = Math.Pow(10.0, Math.Floor(Math.Log10(Math.Abs(value))) + 1.0 - (double)digits);
		return num * Math.Truncate(value / num);
	}

	// Token: 0x06008E17 RID: 36375 RVA: 0x0038F594 File Offset: 0x0038D794
	public static void Swap(ref int x, ref int z)
	{
		int num = x;
		x = z;
		z = num;
	}

	// Token: 0x06008E18 RID: 36376 RVA: 0x0038F5AB File Offset: 0x0038D7AB
	public static int Mod(int _value, int _modulus)
	{
		return (_value % _modulus + _modulus) % _modulus;
	}

	// Token: 0x06008E19 RID: 36377 RVA: 0x0038F5AB File Offset: 0x0038D7AB
	public static float Mod(float _value, float _modulus)
	{
		return (_value % _modulus + _modulus) % _modulus;
	}

	// Token: 0x06008E1A RID: 36378 RVA: 0x0038F5B4 File Offset: 0x0038D7B4
	public static uint ToNextPowerOfTwo(uint _x, bool _allowCurrent = false)
	{
		if (_allowCurrent)
		{
			_x -= 1U;
		}
		_x |= _x >> 1;
		_x |= _x >> 2;
		_x |= _x >> 4;
		_x |= _x >> 8;
		_x |= _x >> 16;
		return _x + 1U;
	}

	// Token: 0x06008E1B RID: 36379 RVA: 0x0038F5E5 File Offset: 0x0038D7E5
	public static uint ToPreviousPowerOfTwo(uint _x, bool _allowCurrent = false)
	{
		return MathUtils.ToNextPowerOfTwo(_x, !_allowCurrent) >> 1;
	}
}
