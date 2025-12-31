using System;

namespace SharpEXR.ColorSpace
{
	// Token: 0x02001413 RID: 5139
	public static class Gamma
	{
		// Token: 0x0600A01A RID: 40986 RVA: 0x003F5E8D File Offset: 0x003F408D
		public static float Expand(float nonlinear)
		{
			return (float)Math.Pow((double)nonlinear, 2.2);
		}

		// Token: 0x0600A01B RID: 40987 RVA: 0x003F5EA0 File Offset: 0x003F40A0
		public static float Compress(float linear)
		{
			return (float)Math.Pow((double)linear, 0.45454545454545453);
		}

		// Token: 0x0600A01C RID: 40988 RVA: 0x003F5EB3 File Offset: 0x003F40B3
		public static void Expand(ref tVec3 pColor)
		{
			pColor.X = Gamma.Expand(pColor.X);
			pColor.Y = Gamma.Expand(pColor.Y);
			pColor.Z = Gamma.Expand(pColor.Z);
		}

		// Token: 0x0600A01D RID: 40989 RVA: 0x003F5EE8 File Offset: 0x003F40E8
		public static void Compress(ref tVec3 pColor)
		{
			pColor.X = Gamma.Compress(pColor.X);
			pColor.Y = Gamma.Compress(pColor.Y);
			pColor.Z = Gamma.Compress(pColor.Z);
		}

		// Token: 0x0600A01E RID: 40990 RVA: 0x003F5F1D File Offset: 0x003F411D
		public static void Expand(ref float r, ref float g, ref float b)
		{
			r = Gamma.Expand(r);
			g = Gamma.Expand(g);
			b = Gamma.Expand(b);
		}

		// Token: 0x0600A01F RID: 40991 RVA: 0x003F5F3A File Offset: 0x003F413A
		public static void Compress(ref float r, ref float g, ref float b)
		{
			r = Gamma.Compress(r);
			g = Gamma.Compress(g);
			b = Gamma.Compress(b);
		}

		// Token: 0x0600A020 RID: 40992 RVA: 0x003F5F58 File Offset: 0x003F4158
		public static tVec3 Expand(float r, float g, float b)
		{
			tVec3 result = new tVec3(r, g, b);
			Gamma.Expand(ref result);
			return result;
		}

		// Token: 0x0600A021 RID: 40993 RVA: 0x003F5F78 File Offset: 0x003F4178
		public static tVec3 Compress(float r, float g, float b)
		{
			tVec3 result = new tVec3(r, g, b);
			Gamma.Compress(ref result);
			return result;
		}

		// Token: 0x0600A022 RID: 40994 RVA: 0x003F5F97 File Offset: 0x003F4197
		public static float Expand_sRGB(float nonlinear)
		{
			if (nonlinear > 0.04045f)
			{
				return (float)Math.Pow((double)((nonlinear + 0.055f) / 1.055f), 2.4000000953674316);
			}
			return nonlinear / 12.92f;
		}

		// Token: 0x0600A023 RID: 40995 RVA: 0x003F5FC6 File Offset: 0x003F41C6
		public static float Compress_sRGB(float linear)
		{
			if (linear > 0.0031308f)
			{
				return 1.055f * (float)Math.Pow((double)linear, 0.4166666567325592) - 0.055f;
			}
			return 12.92f * linear;
		}

		// Token: 0x0600A024 RID: 40996 RVA: 0x003F5FF5 File Offset: 0x003F41F5
		public static void Expand_sRGB(ref tVec3 pColor)
		{
			pColor.X = Gamma.Expand_sRGB(pColor.X);
			pColor.Y = Gamma.Expand_sRGB(pColor.Y);
			pColor.Z = Gamma.Expand_sRGB(pColor.Z);
		}

		// Token: 0x0600A025 RID: 40997 RVA: 0x003F602A File Offset: 0x003F422A
		public static void Compress_sRGB(ref tVec3 pColor)
		{
			pColor.X = Gamma.Compress_sRGB(pColor.X);
			pColor.Y = Gamma.Compress_sRGB(pColor.Y);
			pColor.Z = Gamma.Compress_sRGB(pColor.Z);
		}

		// Token: 0x0600A026 RID: 40998 RVA: 0x003F605F File Offset: 0x003F425F
		public static void Expand_sRGB(ref float r, ref float g, ref float b)
		{
			r = Gamma.Expand_sRGB(r);
			g = Gamma.Expand_sRGB(g);
			b = Gamma.Expand_sRGB(b);
		}

		// Token: 0x0600A027 RID: 40999 RVA: 0x003F607C File Offset: 0x003F427C
		public static void Compress_sRGB(ref float r, ref float g, ref float b)
		{
			r = Gamma.Compress_sRGB(r);
			g = Gamma.Compress_sRGB(g);
			b = Gamma.Compress_sRGB(b);
		}

		// Token: 0x0600A028 RID: 41000 RVA: 0x003F609C File Offset: 0x003F429C
		public static tVec3 Expand_sRGB(float r, float g, float b)
		{
			tVec3 result = new tVec3(r, g, b);
			Gamma.Expand_sRGB(ref result);
			return result;
		}

		// Token: 0x0600A029 RID: 41001 RVA: 0x003F60BC File Offset: 0x003F42BC
		public static tVec3 Compress_sRGB(float r, float g, float b)
		{
			tVec3 result = new tVec3(r, g, b);
			Gamma.Compress_sRGB(ref result);
			return result;
		}
	}
}
