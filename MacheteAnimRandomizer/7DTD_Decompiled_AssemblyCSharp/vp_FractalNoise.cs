using System;
using UnityEngine;

// Token: 0x020012CE RID: 4814
public class vp_FractalNoise
{
	// Token: 0x06009605 RID: 38405 RVA: 0x003BB226 File Offset: 0x003B9426
	public vp_FractalNoise(float inH, float inLacunarity, float inOctaves) : this(inH, inLacunarity, inOctaves, null)
	{
	}

	// Token: 0x06009606 RID: 38406 RVA: 0x003BB234 File Offset: 0x003B9434
	public vp_FractalNoise(float inH, float inLacunarity, float inOctaves, vp_Perlin noise)
	{
		this.m_Lacunarity = inLacunarity;
		this.m_Octaves = inOctaves;
		this.m_IntOctaves = (int)inOctaves;
		this.m_Exponent = new float[this.m_IntOctaves + 1];
		float num = 1f;
		for (int i = 0; i < this.m_IntOctaves + 1; i++)
		{
			this.m_Exponent[i] = (float)Math.Pow((double)this.m_Lacunarity, (double)(-(double)inH));
			num *= this.m_Lacunarity;
		}
		if (noise == null)
		{
			this.m_Noise = new vp_Perlin();
			return;
		}
		this.m_Noise = noise;
	}

	// Token: 0x06009607 RID: 38407 RVA: 0x003BB2C4 File Offset: 0x003B94C4
	public float HybridMultifractal(float x, float y, float offset)
	{
		float num = (this.m_Noise.Noise(x, y) + offset) * this.m_Exponent[0];
		float num2 = num;
		x *= this.m_Lacunarity;
		y *= this.m_Lacunarity;
		int i;
		for (i = 1; i < this.m_IntOctaves; i++)
		{
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			float num3 = (this.m_Noise.Noise(x, y) + offset) * this.m_Exponent[i];
			num += num2 * num3;
			num2 *= num3;
			x *= this.m_Lacunarity;
			y *= this.m_Lacunarity;
		}
		float num4 = this.m_Octaves - (float)this.m_IntOctaves;
		return num + num4 * this.m_Noise.Noise(x, y) * this.m_Exponent[i];
	}

	// Token: 0x06009608 RID: 38408 RVA: 0x003BB388 File Offset: 0x003B9588
	public float RidgedMultifractal(float x, float y, float offset, float gain)
	{
		float num = Mathf.Abs(this.m_Noise.Noise(x, y));
		num = offset - num;
		num *= num;
		float num2 = num;
		for (int i = 1; i < this.m_IntOctaves; i++)
		{
			x *= this.m_Lacunarity;
			y *= this.m_Lacunarity;
			float num3 = num * gain;
			num3 = Mathf.Clamp01(num3);
			num = Mathf.Abs(this.m_Noise.Noise(x, y));
			num = offset - num;
			num *= num;
			num *= num3;
			num2 += num * this.m_Exponent[i];
		}
		return num2;
	}

	// Token: 0x06009609 RID: 38409 RVA: 0x003BB418 File Offset: 0x003B9618
	public float BrownianMotion(float x, float y)
	{
		float num = 0f;
		long num2;
		for (num2 = 0L; num2 < (long)this.m_IntOctaves; num2 += 1L)
		{
			num = this.m_Noise.Noise(x, y) * this.m_Exponent[(int)(checked((IntPtr)num2))];
			x *= this.m_Lacunarity;
			y *= this.m_Lacunarity;
		}
		float num3 = this.m_Octaves - (float)this.m_IntOctaves;
		return num + num3 * this.m_Noise.Noise(x, y) * this.m_Exponent[(int)(checked((IntPtr)num2))];
	}

	// Token: 0x0400722B RID: 29227
	[PublicizedFrom(EAccessModifier.Private)]
	public vp_Perlin m_Noise;

	// Token: 0x0400722C RID: 29228
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] m_Exponent;

	// Token: 0x0400722D RID: 29229
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_IntOctaves;

	// Token: 0x0400722E RID: 29230
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_Octaves;

	// Token: 0x0400722F RID: 29231
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_Lacunarity;
}
