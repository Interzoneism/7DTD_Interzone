using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200002E RID: 46
public class MicroSplatProceduralTextureConfig : ScriptableObject
{
	// Token: 0x06000116 RID: 278 RVA: 0x0000C3B0 File Offset: 0x0000A5B0
	public void ResetToDefault()
	{
		this.layers = new List<MicroSplatProceduralTextureConfig.Layer>(3);
		this.layers.Add(new MicroSplatProceduralTextureConfig.Layer());
		this.layers.Add(new MicroSplatProceduralTextureConfig.Layer());
		this.layers.Add(new MicroSplatProceduralTextureConfig.Layer());
		this.layers[1].textureIndex = 1;
		this.layers[1].slopeActive = true;
		this.layers[1].slopeCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0.03f, 0f),
			new Keyframe(0.06f, 1f),
			new Keyframe(0.16f, 1f),
			new Keyframe(0.2f, 0f)
		});
		this.layers[0].slopeActive = true;
		this.layers[0].textureIndex = 2;
		this.layers[0].slopeCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0.13f, 0f),
			new Keyframe(0.25f, 1f)
		});
	}

	// Token: 0x06000117 RID: 279 RVA: 0x0000C500 File Offset: 0x0000A700
	public Texture2D GetHeightGradientTexture()
	{
		int height = 32;
		int num = 128;
		if (this.heightGradientTex == null)
		{
			this.heightGradientTex = new Texture2D(num, height, TextureFormat.RGBA32, false);
			this.heightGradientTex.hideFlags = HideFlags.HideAndDontSave;
		}
		Color grey = Color.grey;
		for (int i = 0; i < this.heightGradients.Count; i++)
		{
			for (int j = 0; j < num; j++)
			{
				float time = (float)j / (float)num;
				Color color = this.heightGradients[i].Evaluate(time);
				this.heightGradientTex.SetPixel(j, i, color);
			}
		}
		for (int k = this.heightGradients.Count; k < 32; k++)
		{
			for (int l = 0; l < num; l++)
			{
				this.heightGradientTex.SetPixel(l, k, grey);
			}
		}
		this.heightGradientTex.Apply(false, false);
		return this.heightGradientTex;
	}

	// Token: 0x06000118 RID: 280 RVA: 0x0000C5EC File Offset: 0x0000A7EC
	public Texture2D GetHeightHSVTexture()
	{
		int height = 32;
		int num = 128;
		if (this.heightHSVTex == null)
		{
			this.heightHSVTex = new Texture2D(num, height, TextureFormat.RGBA32, false);
			this.heightHSVTex.hideFlags = HideFlags.HideAndDontSave;
		}
		Color grey = Color.grey;
		for (int i = 0; i < this.heightHSV.Count; i++)
		{
			for (int j = 0; j < num; j++)
			{
				Color color = grey;
				float time = (float)j / (float)num;
				color.r = this.heightHSV[i].H.Evaluate(time) * 0.5f + 0.5f;
				color.g = this.heightHSV[i].S.Evaluate(time) * 0.5f + 0.5f;
				color.b = this.heightHSV[i].V.Evaluate(time) * 0.5f + 0.5f;
				this.heightHSVTex.SetPixel(j, i, color);
			}
		}
		for (int k = this.heightHSV.Count; k < 32; k++)
		{
			for (int l = 0; l < num; l++)
			{
				this.heightHSVTex.SetPixel(l, k, grey);
			}
		}
		this.heightHSVTex.Apply(false, false);
		return this.heightHSVTex;
	}

	// Token: 0x06000119 RID: 281 RVA: 0x0000C750 File Offset: 0x0000A950
	public Texture2D GetSlopeGradientTexture()
	{
		int height = 32;
		int num = 128;
		if (this.slopeGradientTex == null)
		{
			this.slopeGradientTex = new Texture2D(num, height, TextureFormat.RGBA32, false);
			this.slopeGradientTex.hideFlags = HideFlags.HideAndDontSave;
		}
		Color grey = Color.grey;
		for (int i = 0; i < this.slopeGradients.Count; i++)
		{
			for (int j = 0; j < num; j++)
			{
				float time = (float)j / (float)num;
				Color color = this.slopeGradients[i].Evaluate(time);
				this.slopeGradientTex.SetPixel(j, i, color);
			}
		}
		for (int k = this.slopeGradients.Count; k < 32; k++)
		{
			for (int l = 0; l < num; l++)
			{
				this.slopeGradientTex.SetPixel(l, k, grey);
			}
		}
		this.slopeGradientTex.Apply(false, false);
		return this.slopeGradientTex;
	}

	// Token: 0x0600011A RID: 282 RVA: 0x0000C83C File Offset: 0x0000AA3C
	public Texture2D GetSlopeHSVTexture()
	{
		int height = 32;
		int num = 128;
		if (this.slopeHSVTex == null)
		{
			this.slopeHSVTex = new Texture2D(num, height, TextureFormat.RGBA32, false);
			this.slopeHSVTex.hideFlags = HideFlags.HideAndDontSave;
		}
		Color grey = Color.grey;
		for (int i = 0; i < this.slopeHSV.Count; i++)
		{
			for (int j = 0; j < num; j++)
			{
				Color color = grey;
				float time = (float)j / (float)num;
				color.r = this.slopeHSV[i].H.Evaluate(time) * 0.5f + 0.5f;
				color.g = this.slopeHSV[i].S.Evaluate(time) * 0.5f + 0.5f;
				color.b = this.slopeHSV[i].V.Evaluate(time) * 0.5f + 0.5f;
				this.slopeHSVTex.SetPixel(j, i, color);
			}
		}
		for (int k = this.slopeHSV.Count; k < 32; k++)
		{
			for (int l = 0; l < num; l++)
			{
				this.slopeHSVTex.SetPixel(l, k, grey);
			}
		}
		this.slopeHSVTex.Apply(false, false);
		return this.slopeHSVTex;
	}

	// Token: 0x0600011B RID: 283 RVA: 0x0000C9A0 File Offset: 0x0000ABA0
	[PublicizedFrom(EAccessModifier.Private)]
	public float CompFilter(MicroSplatProceduralTextureConfig.Layer.Filter f, MicroSplatProceduralTextureConfig.Layer.CurveMode mode, float v)
	{
		float num = Mathf.Abs(v - f.center) * (1f / Mathf.Max(f.width, 0.0001f));
		num = Mathf.Clamp01(Mathf.Pow(num, f.contrast));
		switch (mode)
		{
		case MicroSplatProceduralTextureConfig.Layer.CurveMode.BoostFilter:
			return 1f - num;
		case MicroSplatProceduralTextureConfig.Layer.CurveMode.HighPass:
			if (v >= f.center)
			{
				return 1f;
			}
			return 1f - num;
		case MicroSplatProceduralTextureConfig.Layer.CurveMode.LowPass:
			if (v <= f.center)
			{
				return 1f;
			}
			return 1f - num;
		case MicroSplatProceduralTextureConfig.Layer.CurveMode.CutFilter:
			return num;
		default:
			Debug.LogError("Unhandled case in ProceduralTextureConfig::CompFilter");
			return 0f;
		}
	}

	// Token: 0x0600011C RID: 284 RVA: 0x0000CA48 File Offset: 0x0000AC48
	public Texture2D GetCurveTexture()
	{
		int height = 32;
		int num = (int)this.proceduralCurveTextureSize;
		if (this.curveTex != null && this.curveTex.width != num)
		{
			UnityEngine.Object.DestroyImmediate(this.curveTex);
			this.curveTex = null;
		}
		if (this.curveTex == null)
		{
			this.curveTex = new Texture2D(num, height, TextureFormat.RGBA32, false, true);
			this.curveTex.hideFlags = HideFlags.HideAndDontSave;
		}
		Color white = Color.white;
		for (int i = 0; i < this.layers.Count; i++)
		{
			for (int j = 0; j < num; j++)
			{
				Color color = white;
				float num2 = (float)j / (float)num;
				if (this.layers[i].heightActive)
				{
					if (this.layers[i].heightCurveMode == MicroSplatProceduralTextureConfig.Layer.CurveMode.Curve)
					{
						color.r = this.layers[i].heightCurve.Evaluate(num2);
					}
					else
					{
						color.r = this.CompFilter(this.layers[i].heightFilter, this.layers[i].heightCurveMode, num2);
					}
				}
				if (this.layers[i].slopeActive)
				{
					if (this.layers[i].slopeCurveMode == MicroSplatProceduralTextureConfig.Layer.CurveMode.Curve)
					{
						color.g = this.layers[i].slopeCurve.Evaluate(num2);
					}
					else
					{
						color.g = this.CompFilter(this.layers[i].slopeFilter, this.layers[i].slopeCurveMode, num2);
					}
				}
				if (this.layers[i].cavityMapActive)
				{
					if (this.layers[i].cavityCurveMode == MicroSplatProceduralTextureConfig.Layer.CurveMode.Curve)
					{
						color.b = this.layers[i].cavityMapCurve.Evaluate(num2);
					}
					else
					{
						color.b = this.CompFilter(this.layers[i].cavityMapFilter, this.layers[i].cavityCurveMode, num2);
					}
				}
				if (this.layers[i].erosionMapActive)
				{
					if (this.layers[i].erosionCurveMode == MicroSplatProceduralTextureConfig.Layer.CurveMode.Curve)
					{
						color.a = this.layers[i].erosionMapCurve.Evaluate(num2);
					}
					else
					{
						color.a = this.CompFilter(this.layers[i].erosionFilter, this.layers[i].erosionCurveMode, num2);
					}
				}
				this.curveTex.SetPixel(j, i, color);
			}
		}
		this.curveTex.Apply(false, false);
		return this.curveTex;
	}

	// Token: 0x0600011D RID: 285 RVA: 0x0000CD00 File Offset: 0x0000AF00
	public Texture2D GetParamTexture()
	{
		int height = 32;
		int num = 4;
		if (this.paramTex == null || this.paramTex.format != TextureFormat.RGBAHalf || this.paramTex.width != num)
		{
			this.paramTex = new Texture2D(num, height, TextureFormat.RGBAHalf, false, true);
			this.paramTex.hideFlags = HideFlags.HideAndDontSave;
		}
		Color color = new Color(0f, 0f, 0f, 0f);
		for (int i = 0; i < this.layers.Count; i++)
		{
			Color color2 = color;
			Color color3 = color;
			if (this.layers[i].noiseActive)
			{
				color2.r = this.layers[i].noiseFrequency;
				color2.g = this.layers[i].noiseRange.x;
				color2.b = this.layers[i].noiseRange.y;
				color2.a = this.layers[i].noiseOffset;
			}
			color3.r = this.layers[i].weight;
			color3.g = (float)this.layers[i].textureIndex;
			this.paramTex.SetPixel(0, i, color2);
			this.paramTex.SetPixel(1, i, color3);
			Vector4 biomeWeights = this.layers[i].biomeWeights;
			this.paramTex.SetPixel(2, i, new Color(biomeWeights.x, biomeWeights.y, biomeWeights.z, biomeWeights.w));
			Vector4 biomeWeights2 = this.layers[i].biomeWeights2;
			this.paramTex.SetPixel(3, i, new Color(biomeWeights2.x, biomeWeights2.y, biomeWeights2.z, biomeWeights2.w));
		}
		this.paramTex.Apply(false, false);
		return this.paramTex;
	}

	// Token: 0x04000181 RID: 385
	public MicroSplatProceduralTextureConfig.TableSize proceduralCurveTextureSize = MicroSplatProceduralTextureConfig.TableSize.k256;

	// Token: 0x04000182 RID: 386
	public List<Gradient> heightGradients = new List<Gradient>();

	// Token: 0x04000183 RID: 387
	public List<MicroSplatProceduralTextureConfig.HSVCurve> heightHSV = new List<MicroSplatProceduralTextureConfig.HSVCurve>();

	// Token: 0x04000184 RID: 388
	public List<Gradient> slopeGradients = new List<Gradient>();

	// Token: 0x04000185 RID: 389
	public List<MicroSplatProceduralTextureConfig.HSVCurve> slopeHSV = new List<MicroSplatProceduralTextureConfig.HSVCurve>();

	// Token: 0x04000186 RID: 390
	[HideInInspector]
	public List<MicroSplatProceduralTextureConfig.Layer> layers = new List<MicroSplatProceduralTextureConfig.Layer>();

	// Token: 0x04000187 RID: 391
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Texture2D curveTex;

	// Token: 0x04000188 RID: 392
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Texture2D paramTex;

	// Token: 0x04000189 RID: 393
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Texture2D heightGradientTex;

	// Token: 0x0400018A RID: 394
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Texture2D heightHSVTex;

	// Token: 0x0400018B RID: 395
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Texture2D slopeGradientTex;

	// Token: 0x0400018C RID: 396
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Texture2D slopeHSVTex;

	// Token: 0x0200002F RID: 47
	public enum TableSize
	{
		// Token: 0x0400018E RID: 398
		k64 = 64,
		// Token: 0x0400018F RID: 399
		k128 = 128,
		// Token: 0x04000190 RID: 400
		k256 = 256,
		// Token: 0x04000191 RID: 401
		k512 = 512,
		// Token: 0x04000192 RID: 402
		k1024 = 1024,
		// Token: 0x04000193 RID: 403
		k2048 = 2048,
		// Token: 0x04000194 RID: 404
		k4096 = 4096
	}

	// Token: 0x02000030 RID: 48
	[Serializable]
	public class Layer
	{
		// Token: 0x0600011F RID: 287 RVA: 0x0000CF54 File Offset: 0x0000B154
		public MicroSplatProceduralTextureConfig.Layer Copy()
		{
			return new MicroSplatProceduralTextureConfig.Layer
			{
				weight = this.weight,
				textureIndex = this.textureIndex,
				noiseActive = this.noiseActive,
				noiseFrequency = this.noiseFrequency,
				noiseOffset = this.noiseOffset,
				noiseRange = this.noiseRange,
				biomeWeights = this.biomeWeights,
				biomeWeights2 = this.biomeWeights2,
				heightActive = this.heightActive,
				slopeActive = this.slopeActive,
				erosionMapActive = this.erosionMapActive,
				cavityMapActive = this.cavityMapActive,
				heightCurve = new AnimationCurve(this.heightCurve.keys),
				slopeCurve = new AnimationCurve(this.slopeCurve.keys),
				erosionMapCurve = new AnimationCurve(this.erosionMapCurve.keys),
				cavityMapCurve = new AnimationCurve(this.cavityMapCurve.keys),
				cavityMapFilter = this.cavityMapFilter,
				heightFilter = this.heightFilter,
				slopeFilter = this.slopeFilter,
				erosionFilter = this.erosionFilter,
				heightCurveMode = this.heightCurveMode,
				slopeCurveMode = this.slopeCurveMode,
				erosionCurveMode = this.erosionCurveMode,
				cavityCurveMode = this.cavityCurveMode
			};
		}

		// Token: 0x04000195 RID: 405
		public float weight = 1f;

		// Token: 0x04000196 RID: 406
		public int textureIndex;

		// Token: 0x04000197 RID: 407
		public bool noiseActive;

		// Token: 0x04000198 RID: 408
		public float noiseFrequency = 1f;

		// Token: 0x04000199 RID: 409
		public float noiseOffset;

		// Token: 0x0400019A RID: 410
		public Vector2 noiseRange = new Vector2(0f, 1f);

		// Token: 0x0400019B RID: 411
		public Vector4 biomeWeights = new Vector4(1f, 1f, 1f, 1f);

		// Token: 0x0400019C RID: 412
		public Vector4 biomeWeights2 = new Vector4(1f, 1f, 1f, 1f);

		// Token: 0x0400019D RID: 413
		public bool heightActive;

		// Token: 0x0400019E RID: 414
		public AnimationCurve heightCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x0400019F RID: 415
		public MicroSplatProceduralTextureConfig.Layer.Filter heightFilter = new MicroSplatProceduralTextureConfig.Layer.Filter();

		// Token: 0x040001A0 RID: 416
		public bool slopeActive;

		// Token: 0x040001A1 RID: 417
		public AnimationCurve slopeCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x040001A2 RID: 418
		public MicroSplatProceduralTextureConfig.Layer.Filter slopeFilter = new MicroSplatProceduralTextureConfig.Layer.Filter();

		// Token: 0x040001A3 RID: 419
		public bool erosionMapActive;

		// Token: 0x040001A4 RID: 420
		public AnimationCurve erosionMapCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x040001A5 RID: 421
		public MicroSplatProceduralTextureConfig.Layer.Filter erosionFilter = new MicroSplatProceduralTextureConfig.Layer.Filter();

		// Token: 0x040001A6 RID: 422
		public bool cavityMapActive;

		// Token: 0x040001A7 RID: 423
		public AnimationCurve cavityMapCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x040001A8 RID: 424
		public MicroSplatProceduralTextureConfig.Layer.Filter cavityMapFilter = new MicroSplatProceduralTextureConfig.Layer.Filter();

		// Token: 0x040001A9 RID: 425
		public MicroSplatProceduralTextureConfig.Layer.CurveMode heightCurveMode;

		// Token: 0x040001AA RID: 426
		public MicroSplatProceduralTextureConfig.Layer.CurveMode slopeCurveMode;

		// Token: 0x040001AB RID: 427
		public MicroSplatProceduralTextureConfig.Layer.CurveMode erosionCurveMode;

		// Token: 0x040001AC RID: 428
		public MicroSplatProceduralTextureConfig.Layer.CurveMode cavityCurveMode;

		// Token: 0x02000031 RID: 49
		[Serializable]
		public class Filter
		{
			// Token: 0x040001AD RID: 429
			public float center = 0.5f;

			// Token: 0x040001AE RID: 430
			public float width = 0.1f;

			// Token: 0x040001AF RID: 431
			public float contrast = 1f;
		}

		// Token: 0x02000032 RID: 50
		public enum CurveMode
		{
			// Token: 0x040001B1 RID: 433
			Curve,
			// Token: 0x040001B2 RID: 434
			BoostFilter,
			// Token: 0x040001B3 RID: 435
			HighPass,
			// Token: 0x040001B4 RID: 436
			LowPass,
			// Token: 0x040001B5 RID: 437
			CutFilter
		}
	}

	// Token: 0x02000033 RID: 51
	[Serializable]
	public class HSVCurve
	{
		// Token: 0x040001B6 RID: 438
		public AnimationCurve H = AnimationCurve.Linear(0f, 0.5f, 1f, 0.5f);

		// Token: 0x040001B7 RID: 439
		public AnimationCurve S = AnimationCurve.Linear(0f, 0f, 1f, 0f);

		// Token: 0x040001B8 RID: 440
		public AnimationCurve V = AnimationCurve.Linear(0f, 0f, 1f, 0f);
	}
}
