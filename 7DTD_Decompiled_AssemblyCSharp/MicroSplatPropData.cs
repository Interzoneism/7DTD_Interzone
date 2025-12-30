using System;
using UnityEngine;

// Token: 0x02000026 RID: 38
public class MicroSplatPropData : ScriptableObject
{
	// Token: 0x060000F1 RID: 241 RVA: 0x0000B8C0 File Offset: 0x00009AC0
	[PublicizedFrom(EAccessModifier.Private)]
	public void RevisionData()
	{
		if (this.values.Length == 256)
		{
			Color[] array = new Color[1024];
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					array[j * 32 + i] = this.values[j * 32 + i];
				}
			}
			this.values = array;
			return;
		}
		if (this.values.Length == 512)
		{
			Color[] array2 = new Color[1024];
			for (int k = 0; k < 32; k++)
			{
				for (int l = 0; l < 16; l++)
				{
					array2[l * 32 + k] = this.values[l * 32 + k];
				}
			}
			this.values = array2;
		}
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x0000B98A File Offset: 0x00009B8A
	public Color GetValue(int x, int y)
	{
		this.RevisionData();
		return this.values[y * 32 + x];
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x0000B9A3 File Offset: 0x00009BA3
	public void SetValue(int x, int y, Color c)
	{
		this.RevisionData();
		this.values[y * 32 + x] = c;
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x0000B9C0 File Offset: 0x00009BC0
	public void SetValue(int x, int y, int channel, float value)
	{
		this.RevisionData();
		int num = y * 32 + x;
		Color color = this.values[num];
		color[channel] = value;
		this.values[num] = color;
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x0000BA00 File Offset: 0x00009C00
	public void SetValue(int x, int y, int channel, Vector2 value)
	{
		this.RevisionData();
		int num = y * 32 + x;
		Color color = this.values[num];
		if (channel == 0)
		{
			color.r = value.x;
			color.g = value.y;
		}
		else
		{
			color.b = value.x;
			color.a = value.y;
		}
		this.values[num] = color;
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x0000BA74 File Offset: 0x00009C74
	public void SetValue(int textureIndex, MicroSplatPropData.PerTexFloat channel, float value)
	{
		float num = (float)channel / 4f;
		int num2 = (int)num;
		int channel2 = Mathf.RoundToInt((num - (float)num2) * 4f);
		this.SetValue(textureIndex, num2, channel2, value);
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x0000BAA8 File Offset: 0x00009CA8
	public void SetValue(int textureIndex, MicroSplatPropData.PerTexColor channel, Color value)
	{
		int y = (int)((float)channel / 4f);
		this.SetValue(textureIndex, y, value);
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x0000BAC8 File Offset: 0x00009CC8
	public void SetValue(int textureIndex, MicroSplatPropData.PerTexVector2 channel, Vector2 value)
	{
		float num = (float)channel / 4f;
		int num2 = (int)num;
		int channel2 = Mathf.RoundToInt((num - (float)num2) * 4f);
		this.SetValue(textureIndex, num2, channel2, value);
	}

	// Token: 0x060000F9 RID: 249 RVA: 0x0000BAFC File Offset: 0x00009CFC
	public Texture2D GetTexture()
	{
		this.RevisionData();
		if (this.tex == null)
		{
			if (SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat))
			{
				this.tex = new Texture2D(32, 32, TextureFormat.RGBAFloat, false, true);
			}
			else if (SystemInfo.SupportsTextureFormat(TextureFormat.RGBAHalf))
			{
				this.tex = new Texture2D(32, 32, TextureFormat.RGBAHalf, false, true);
			}
			else
			{
				Debug.LogError("Could not create RGBAFloat or RGBAHalf format textures, per texture properties will be clamped to 0-1 range, which will break things");
				this.tex = new Texture2D(32, 32, TextureFormat.RGBA32, false, true);
			}
			this.tex.hideFlags = HideFlags.HideAndDontSave;
			this.tex.wrapMode = TextureWrapMode.Clamp;
			this.tex.filterMode = FilterMode.Point;
		}
		this.tex.SetPixels(this.values);
		this.tex.Apply();
		return this.tex;
	}

	// Token: 0x060000FA RID: 250 RVA: 0x0000BBBC File Offset: 0x00009DBC
	public Texture2D GetGeoCurve()
	{
		if (this.geoTex == null)
		{
			this.geoTex = new Texture2D(256, 1, TextureFormat.RHalf, false, true);
			this.geoTex.hideFlags = HideFlags.HideAndDontSave;
		}
		for (int i = 0; i < 256; i++)
		{
			float num = this.geoCurve.Evaluate((float)i / 255f);
			this.geoTex.SetPixel(i, 0, new Color(num, num, num, num));
		}
		this.geoTex.Apply();
		return this.geoTex;
	}

	// Token: 0x060000FB RID: 251 RVA: 0x0000BC44 File Offset: 0x00009E44
	public Texture2D GetGeoSlopeFilter()
	{
		if (this.geoSlopeTex == null)
		{
			this.geoSlopeTex = new Texture2D(256, 1, TextureFormat.Alpha8, false, true);
			this.geoSlopeTex.hideFlags = HideFlags.HideAndDontSave;
		}
		for (int i = 0; i < 256; i++)
		{
			float num = this.geoSlopeFilter.Evaluate((float)i / 255f);
			this.geoSlopeTex.SetPixel(i, 0, new Color(num, num, num, num));
		}
		this.geoSlopeTex.Apply();
		return this.geoSlopeTex;
	}

	// Token: 0x060000FC RID: 252 RVA: 0x0000BCCC File Offset: 0x00009ECC
	public Texture2D GetGlobalSlopeFilter()
	{
		if (this.globalSlopeTex == null)
		{
			this.globalSlopeTex = new Texture2D(256, 1, TextureFormat.Alpha8, false, true);
			this.globalSlopeTex.hideFlags = HideFlags.HideAndDontSave;
		}
		for (int i = 0; i < 256; i++)
		{
			float num = this.globalSlopeFilter.Evaluate((float)i / 255f);
			this.globalSlopeTex.SetPixel(i, 0, new Color(num, num, num, num));
		}
		this.globalSlopeTex.Apply();
		return this.globalSlopeTex;
	}

	// Token: 0x0400012F RID: 303
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int sMaxTextures = 32;

	// Token: 0x04000130 RID: 304
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int sMaxAttributes = 32;

	// Token: 0x04000131 RID: 305
	public Color[] values = new Color[1024];

	// Token: 0x04000132 RID: 306
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Texture2D tex;

	// Token: 0x04000133 RID: 307
	[HideInInspector]
	public AnimationCurve geoCurve = AnimationCurve.Linear(0f, 0f, 0f, 0f);

	// Token: 0x04000134 RID: 308
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Texture2D geoTex;

	// Token: 0x04000135 RID: 309
	[HideInInspector]
	public AnimationCurve geoSlopeFilter = AnimationCurve.Linear(0f, 0.2f, 0.4f, 1f);

	// Token: 0x04000136 RID: 310
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Texture2D geoSlopeTex;

	// Token: 0x04000137 RID: 311
	[HideInInspector]
	public AnimationCurve globalSlopeFilter = AnimationCurve.Linear(0f, 0.2f, 0.4f, 1f);

	// Token: 0x04000138 RID: 312
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Texture2D globalSlopeTex;

	// Token: 0x02000027 RID: 39
	public enum PerTexVector2
	{
		// Token: 0x0400013A RID: 314
		SplatUVScale,
		// Token: 0x0400013B RID: 315
		SplatUVOffset = 2
	}

	// Token: 0x02000028 RID: 40
	public enum PerTexColor
	{
		// Token: 0x0400013D RID: 317
		Tint = 4,
		// Token: 0x0400013E RID: 318
		SSSRTint = 72
	}

	// Token: 0x02000029 RID: 41
	public enum PerTexFloat
	{
		// Token: 0x04000140 RID: 320
		InterpolationContrast = 5,
		// Token: 0x04000141 RID: 321
		NormalStrength = 8,
		// Token: 0x04000142 RID: 322
		Smoothness,
		// Token: 0x04000143 RID: 323
		AO,
		// Token: 0x04000144 RID: 324
		Metallic,
		// Token: 0x04000145 RID: 325
		Brightness,
		// Token: 0x04000146 RID: 326
		Contrast,
		// Token: 0x04000147 RID: 327
		Porosity,
		// Token: 0x04000148 RID: 328
		Foam,
		// Token: 0x04000149 RID: 329
		DetailNoiseStrength,
		// Token: 0x0400014A RID: 330
		DistanceNoiseStrength,
		// Token: 0x0400014B RID: 331
		DistanceResample,
		// Token: 0x0400014C RID: 332
		DisplacementMip,
		// Token: 0x0400014D RID: 333
		GeoTexStrength,
		// Token: 0x0400014E RID: 334
		GeoTintStrength,
		// Token: 0x0400014F RID: 335
		GeoNormalStrength,
		// Token: 0x04000150 RID: 336
		GlobalSmoothMetalAOStength,
		// Token: 0x04000151 RID: 337
		DisplacementStength,
		// Token: 0x04000152 RID: 338
		DisplacementBias,
		// Token: 0x04000153 RID: 339
		DisplacementOffset,
		// Token: 0x04000154 RID: 340
		GlobalEmisStength,
		// Token: 0x04000155 RID: 341
		NoiseNormal0Strength,
		// Token: 0x04000156 RID: 342
		NoiseNormal1Strength,
		// Token: 0x04000157 RID: 343
		NoiseNormal2Strength,
		// Token: 0x04000158 RID: 344
		WindParticulateStrength,
		// Token: 0x04000159 RID: 345
		SnowAmount,
		// Token: 0x0400015A RID: 346
		GlitterAmount,
		// Token: 0x0400015B RID: 347
		GeoHeightFilter,
		// Token: 0x0400015C RID: 348
		GeoHeightFilterStrength,
		// Token: 0x0400015D RID: 349
		TriplanarMode,
		// Token: 0x0400015E RID: 350
		TriplanarContrast,
		// Token: 0x0400015F RID: 351
		StochatsicEnabled,
		// Token: 0x04000160 RID: 352
		Saturation,
		// Token: 0x04000161 RID: 353
		TextureClusterContrast,
		// Token: 0x04000162 RID: 354
		TextureClusterBoost,
		// Token: 0x04000163 RID: 355
		HeightOffset,
		// Token: 0x04000164 RID: 356
		HeightContrast,
		// Token: 0x04000165 RID: 357
		AntiTileArrayNormalStrength = 56,
		// Token: 0x04000166 RID: 358
		AntiTileArrayDetailStrength,
		// Token: 0x04000167 RID: 359
		AntiTileArrayDistanceStrength,
		// Token: 0x04000168 RID: 360
		DisplaceShaping,
		// Token: 0x04000169 RID: 361
		UVRotation = 64,
		// Token: 0x0400016A RID: 362
		TriplanarRotationX,
		// Token: 0x0400016B RID: 363
		TriplanarRotationY,
		// Token: 0x0400016C RID: 364
		FuzzyShadingCore = 68,
		// Token: 0x0400016D RID: 365
		FuzzyShadingEdge,
		// Token: 0x0400016E RID: 366
		FuzzyShadingPower,
		// Token: 0x0400016F RID: 367
		SSSThickness = 75
	}
}
