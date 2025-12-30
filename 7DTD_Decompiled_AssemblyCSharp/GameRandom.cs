using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000923 RID: 2339
public class GameRandom : IMemoryPoolableObject
{
	// Token: 0x060045B7 RID: 17847 RVA: 0x001BD97C File Offset: 0x001BBB7C
	public void SetSeed(int _seed)
	{
		this.InternalSetSeed(_seed);
	}

	// Token: 0x060045B8 RID: 17848 RVA: 0x00002914 File Offset: 0x00000B14
	public void SetLock()
	{
	}

	// Token: 0x060045B9 RID: 17849 RVA: 0x00002914 File Offset: 0x00000B14
	public void Cleanup()
	{
	}

	// Token: 0x060045BA RID: 17850 RVA: 0x00002914 File Offset: 0x00000B14
	public void Reset()
	{
	}

	// Token: 0x17000749 RID: 1865
	// (get) Token: 0x060045BB RID: 17851 RVA: 0x001BD985 File Offset: 0x001BBB85
	public double RandomDouble
	{
		get
		{
			return this.NextDouble();
		}
	}

	// Token: 0x1700074A RID: 1866
	// (get) Token: 0x060045BC RID: 17852 RVA: 0x001BD98D File Offset: 0x001BBB8D
	public float RandomFloat
	{
		get
		{
			return (float)this.NextDouble();
		}
	}

	// Token: 0x1700074B RID: 1867
	// (get) Token: 0x060045BD RID: 17853 RVA: 0x001BD996 File Offset: 0x001BBB96
	public int RandomInt
	{
		get
		{
			return this.Next();
		}
	}

	// Token: 0x1700074C RID: 1868
	// (get) Token: 0x060045BE RID: 17854 RVA: 0x001BD9A0 File Offset: 0x001BBBA0
	public Vector2 RandomInsideUnitCircle
	{
		get
		{
			float f = (float)this.NextDouble() * 6.2831855f;
			return new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * (float)Math.Sqrt(this.NextDouble());
		}
	}

	// Token: 0x1700074D RID: 1869
	// (get) Token: 0x060045BF RID: 17855 RVA: 0x001BD9E0 File Offset: 0x001BBBE0
	public Vector2 RandomOnUnitCircle
	{
		get
		{
			float f = (float)this.NextDouble() * 6.2831855f;
			return new Vector2(Mathf.Cos(f), Mathf.Sin(f));
		}
	}

	// Token: 0x1700074E RID: 1870
	// (get) Token: 0x060045C0 RID: 17856 RVA: 0x001BDA0C File Offset: 0x001BBC0C
	public Vector3 RandomOnUnitCircleXZ
	{
		get
		{
			float f = (float)this.NextDouble() * 6.2831855f;
			return new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
		}
	}

	// Token: 0x1700074F RID: 1871
	// (get) Token: 0x060045C1 RID: 17857 RVA: 0x001BDA40 File Offset: 0x001BBC40
	public Vector3 RandomInsideUnitSphere
	{
		get
		{
			return new Vector3((float)(this.NextDouble() - 0.5), (float)(this.NextDouble() - 0.5), (float)(this.NextDouble() - 0.5)).normalized * (float)Math.Sqrt(this.NextDouble());
		}
	}

	// Token: 0x17000750 RID: 1872
	// (get) Token: 0x060045C2 RID: 17858 RVA: 0x001BDAA0 File Offset: 0x001BBCA0
	public Vector3 RandomOnUnitSphere
	{
		get
		{
			return new Vector3((float)(this.NextDouble() - 0.5), (float)(this.NextDouble() - 0.5), (float)(this.NextDouble() - 0.5)).normalized;
		}
	}

	// Token: 0x17000751 RID: 1873
	// (get) Token: 0x060045C3 RID: 17859 RVA: 0x001BDAF0 File Offset: 0x001BBCF0
	public float RandomGaussian
	{
		get
		{
			float num;
			float num3;
			do
			{
				num = 2f * this.RandomRange(0f, 1f) - 1f;
				float num2 = 2f * this.RandomRange(0f, 1f) - 1f;
				num3 = num * num + num2 * num2;
			}
			while (num3 >= 1f || num3 == 0f);
			num3 = Mathf.Sqrt(-2f * Mathf.Log(num3) / num3);
			return num3 * num;
		}
	}

	// Token: 0x060045C4 RID: 17860 RVA: 0x001BDB68 File Offset: 0x001BBD68
	public float RandomRange(float _maxExclusive)
	{
		return (float)(this.NextDouble() * (double)_maxExclusive);
	}

	// Token: 0x060045C5 RID: 17861 RVA: 0x001BDB74 File Offset: 0x001BBD74
	public float RandomRange(float _min, float _maxExclusive)
	{
		return (float)(this.NextDouble() * (double)(_maxExclusive - _min) + (double)_min);
	}

	// Token: 0x060045C6 RID: 17862 RVA: 0x001BDB85 File Offset: 0x001BBD85
	public int RandomRange(int _maxExclusive)
	{
		return this.Next(_maxExclusive);
	}

	// Token: 0x060045C7 RID: 17863 RVA: 0x001BDB8E File Offset: 0x001BBD8E
	public int RandomRange(int _min, int _maxExclusive)
	{
		return this.Next(_maxExclusive - _min) + _min;
	}

	// Token: 0x060045C8 RID: 17864 RVA: 0x001BDB9B File Offset: 0x001BBD9B
	[PublicizedFrom(EAccessModifier.Private)]
	public static void log(string _format, params object[] _values)
	{
		Log.Warning(string.Format("{0} GameRandom ", Time.time.ToCultureInvariantString()) + _format, _values);
	}

	// Token: 0x060045C9 RID: 17865 RVA: 0x001BDBC0 File Offset: 0x001BBDC0
	[PublicizedFrom(EAccessModifier.Private)]
	public void InternalSetSeed(int Seed)
	{
		int num = (Seed == int.MinValue) ? int.MaxValue : Math.Abs(Seed);
		int num2 = 161803398 - num;
		this.SeedArray[55] = num2;
		int num3 = 1;
		for (int i = 1; i < 55; i++)
		{
			int num4 = 21 * i % 55;
			this.SeedArray[num4] = num3;
			num3 = num2 - num3;
			if (num3 < 0)
			{
				num3 += int.MaxValue;
			}
			num2 = this.SeedArray[num4];
		}
		for (int j = 1; j < 5; j++)
		{
			for (int k = 1; k < 56; k++)
			{
				this.SeedArray[k] -= this.SeedArray[1 + (k + 30) % 55];
				if (this.SeedArray[k] < 0)
				{
					this.SeedArray[k] += int.MaxValue;
				}
			}
		}
		this.inext = 0;
		this.inextp = 21;
		Seed = 1;
	}

	// Token: 0x060045CA RID: 17866 RVA: 0x001BDCAA File Offset: 0x001BBEAA
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double Sample()
	{
		return (double)this.InternalSample() * 4.656612875245797E-10;
	}

	// Token: 0x060045CB RID: 17867 RVA: 0x001BDCC0 File Offset: 0x001BBEC0
	[PublicizedFrom(EAccessModifier.Private)]
	public int InternalSample()
	{
		int num = this.inext;
		int num2 = this.inextp;
		if (++num >= 56)
		{
			num = 1;
		}
		if (++num2 >= 56)
		{
			num2 = 1;
		}
		int num3 = this.SeedArray[num] - this.SeedArray[num2];
		if (num3 == 2147483647)
		{
			num3--;
		}
		if (num3 < 0)
		{
			num3 += int.MaxValue;
		}
		this.SeedArray[num] = num3;
		this.inext = num;
		this.inextp = num2;
		return num3;
	}

	// Token: 0x060045CC RID: 17868 RVA: 0x001BDD34 File Offset: 0x001BBF34
	public int PeekSample()
	{
		int num = this.inext;
		int num2 = this.inextp;
		if (++num >= 56)
		{
			num = 1;
		}
		if (++num2 >= 56)
		{
			num2 = 1;
		}
		int num3 = this.SeedArray[num] - this.SeedArray[num2];
		if (num3 == 2147483647)
		{
			num3--;
		}
		if (num3 < 0)
		{
			num3 += int.MaxValue;
		}
		return num3;
	}

	// Token: 0x060045CD RID: 17869 RVA: 0x001BDD90 File Offset: 0x001BBF90
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int Next()
	{
		return this.InternalSample();
	}

	// Token: 0x060045CE RID: 17870 RVA: 0x001BDD98 File Offset: 0x001BBF98
	[PublicizedFrom(EAccessModifier.Private)]
	public double GetSampleForLargeRange()
	{
		int num = this.InternalSample();
		if (this.InternalSample() % 2 == 0)
		{
			num = -num;
		}
		return ((double)num + 2147483646.0) / 4294967293.0;
	}

	// Token: 0x060045CF RID: 17871 RVA: 0x001BDDD8 File Offset: 0x001BBFD8
	[PublicizedFrom(EAccessModifier.Private)]
	public int Next(int minValue, int maxValue)
	{
		if (minValue > maxValue)
		{
			throw new ArgumentOutOfRangeException("minValue", "Argument_MinMaxValue");
		}
		long num = (long)maxValue - (long)minValue;
		if (num <= 2147483647L)
		{
			return (int)(this.Sample() * (double)num) + minValue;
		}
		return (int)((long)(this.GetSampleForLargeRange() * (double)num) + (long)minValue);
	}

	// Token: 0x060045D0 RID: 17872 RVA: 0x001BDE23 File Offset: 0x001BC023
	[PublicizedFrom(EAccessModifier.Private)]
	public int Next(int maxValue)
	{
		if (maxValue < 0)
		{
			throw new ArgumentOutOfRangeException("maxValue", "ArgumentOutOfRange_MustBePositive");
		}
		return (int)(this.Sample() * (double)maxValue);
	}

	// Token: 0x060045D1 RID: 17873 RVA: 0x001BDE43 File Offset: 0x001BC043
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double NextDouble()
	{
		return this.Sample();
	}

	// Token: 0x060045D2 RID: 17874 RVA: 0x001BDE4C File Offset: 0x001BC04C
	[PublicizedFrom(EAccessModifier.Private)]
	public void NextBytes(byte[] buffer)
	{
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		for (int i = 0; i < buffer.Length; i++)
		{
			buffer[i] = (byte)(this.InternalSample() % 256);
		}
	}

	// Token: 0x04003671 RID: 13937
	[PublicizedFrom(EAccessModifier.Private)]
	public const int MBIG = 2147483647;

	// Token: 0x04003672 RID: 13938
	[PublicizedFrom(EAccessModifier.Private)]
	public const int MSEED = 161803398;

	// Token: 0x04003673 RID: 13939
	[PublicizedFrom(EAccessModifier.Private)]
	public const int MZ = 0;

	// Token: 0x04003674 RID: 13940
	[PublicizedFrom(EAccessModifier.Private)]
	public int inext;

	// Token: 0x04003675 RID: 13941
	[PublicizedFrom(EAccessModifier.Private)]
	public int inextp;

	// Token: 0x04003676 RID: 13942
	[PublicizedFrom(EAccessModifier.Private)]
	public int[] SeedArray = new int[56];
}
