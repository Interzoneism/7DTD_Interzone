using System;
using UnityEngine;

// Token: 0x020010CF RID: 4303
[Serializable]
public class BrushSettings
{
	// Token: 0x17000E2B RID: 3627
	// (get) Token: 0x06008763 RID: 34659 RVA: 0x0036C995 File Offset: 0x0036AB95
	// (set) Token: 0x06008764 RID: 34660 RVA: 0x0036C99D File Offset: 0x0036AB9D
	public string MainTexName { get; set; } = "_MainTex";

	// Token: 0x17000E2C RID: 3628
	// (get) Token: 0x06008765 RID: 34661 RVA: 0x0036C9A6 File Offset: 0x0036ABA6
	// (set) Token: 0x06008766 RID: 34662 RVA: 0x0036C9AE File Offset: 0x0036ABAE
	public string DirectionMapName { get; set; } = "_DirectionMap";

	// Token: 0x17000E2D RID: 3629
	// (get) Token: 0x06008767 RID: 34663 RVA: 0x0036C9B7 File Offset: 0x0036ABB7
	// (set) Token: 0x06008768 RID: 34664 RVA: 0x0036C9BF File Offset: 0x0036ABBF
	public string RMOLMapName { get; set; } = "_RMOL";

	// Token: 0x17000E2E RID: 3630
	// (get) Token: 0x06008769 RID: 34665 RVA: 0x0036C9C8 File Offset: 0x0036ABC8
	// (set) Token: 0x0600876A RID: 34666 RVA: 0x0036C9D0 File Offset: 0x0036ABD0
	public float Size
	{
		get
		{
			return this.size;
		}
		set
		{
			this.UpdateBrushPreview();
			this.size = Mathf.Max(1f, value);
		}
	}

	// Token: 0x17000E2F RID: 3631
	// (get) Token: 0x0600876B RID: 34667 RVA: 0x0036C9E9 File Offset: 0x0036ABE9
	// (set) Token: 0x0600876C RID: 34668 RVA: 0x0036C9F1 File Offset: 0x0036ABF1
	public float Strength
	{
		get
		{
			return this.strength;
		}
		set
		{
			this.isDirty = true;
			this.strength = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17000E30 RID: 3632
	// (get) Token: 0x0600876D RID: 34669 RVA: 0x0036CA06 File Offset: 0x0036AC06
	// (set) Token: 0x0600876E RID: 34670 RVA: 0x0036CA0E File Offset: 0x0036AC0E
	public float Falloff
	{
		get
		{
			return this.falloff;
		}
		set
		{
			this.isDirty = true;
			this.falloff = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17000E31 RID: 3633
	// (get) Token: 0x0600876F RID: 34671 RVA: 0x0036CA23 File Offset: 0x0036AC23
	// (set) Token: 0x06008770 RID: 34672 RVA: 0x0036CA2B File Offset: 0x0036AC2B
	public float Matting
	{
		get
		{
			return this.matting;
		}
		set
		{
			this.matting = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17000E32 RID: 3634
	// (get) Token: 0x06008771 RID: 34673 RVA: 0x0036CA39 File Offset: 0x0036AC39
	// (set) Token: 0x06008772 RID: 34674 RVA: 0x0036CA41 File Offset: 0x0036AC41
	public float Length
	{
		get
		{
			return this.length;
		}
		set
		{
			this.length = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17000E33 RID: 3635
	// (get) Token: 0x06008773 RID: 34675 RVA: 0x0036CA4F File Offset: 0x0036AC4F
	// (set) Token: 0x06008774 RID: 34676 RVA: 0x0036CA57 File Offset: 0x0036AC57
	public float Roughness
	{
		get
		{
			return this.roughness;
		}
		set
		{
			this.roughness = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17000E34 RID: 3636
	// (get) Token: 0x06008775 RID: 34677 RVA: 0x0036CA65 File Offset: 0x0036AC65
	// (set) Token: 0x06008776 RID: 34678 RVA: 0x0036CA6D File Offset: 0x0036AC6D
	public float Metallic
	{
		get
		{
			return this.metallic;
		}
		set
		{
			this.metallic = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17000E35 RID: 3637
	// (get) Token: 0x06008777 RID: 34679 RVA: 0x0036CA7B File Offset: 0x0036AC7B
	// (set) Token: 0x06008778 RID: 34680 RVA: 0x0036CA83 File Offset: 0x0036AC83
	public float Occlusion
	{
		get
		{
			return this.occlusion;
		}
		set
		{
			this.occlusion = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17000E36 RID: 3638
	// (get) Token: 0x06008779 RID: 34681 RVA: 0x0036CA91 File Offset: 0x0036AC91
	// (set) Token: 0x0600877A RID: 34682 RVA: 0x0036CA99 File Offset: 0x0036AC99
	public Color Color
	{
		get
		{
			return this.color;
		}
		set
		{
			this.color = value;
		}
	}

	// Token: 0x17000E37 RID: 3639
	// (get) Token: 0x0600877B RID: 34683 RVA: 0x0036CAA2 File Offset: 0x0036ACA2
	// (set) Token: 0x0600877C RID: 34684 RVA: 0x0036CAB0 File Offset: 0x0036ACB0
	public Texture2D BrushPreview
	{
		get
		{
			this.UpdateBrushPreview();
			return this.brushPreview;
		}
		set
		{
			this.brushPreview = value;
		}
	}

	// Token: 0x0600877D RID: 34685 RVA: 0x0036CABC File Offset: 0x0036ACBC
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateBrushPreview()
	{
		if (!this.isDirty)
		{
			return;
		}
		this.isDirty = false;
		if (this.brushPreview == null)
		{
			this.brushPreview = new Texture2D(100, 100);
		}
		Color[] array = new Color[this.brushPreview.width * this.brushPreview.height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Color.clear;
		}
		int num = Mathf.FloorToInt(this.size * 0.5f);
		for (int j = 0; j < 100; j++)
		{
			for (int k = 0; k < 100; k++)
			{
				Vector2 v = new Vector2((float)j, (float)k) - new Vector2(50f, 50f);
				float num2 = Vector3.Dot(v, v);
				if (num2 <= (float)(num * num))
				{
					float num3 = Mathf.Pow(Mathf.Clamp01(1f - MathF.Sqrt(num2) / (float)num), this.falloff * 4f) * this.strength;
					array[k * this.brushPreview.width + j] = new Color(num3, num3, num3, 1f);
				}
				else
				{
					array[k * this.brushPreview.width + j] = Color.black;
				}
			}
		}
		this.brushPreview.SetPixels(array);
		this.brushPreview.Apply();
	}

	// Token: 0x0400692E RID: 26926
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Texture2D brushPreview;

	// Token: 0x0400692F RID: 26927
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float size = 100f;

	// Token: 0x04006930 RID: 26928
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float strength = 0.5f;

	// Token: 0x04006931 RID: 26929
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float falloff = 0.5f;

	// Token: 0x04006932 RID: 26930
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float matting = 1f;

	// Token: 0x04006933 RID: 26931
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float length = 1f;

	// Token: 0x04006934 RID: 26932
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float roughness = 0.5f;

	// Token: 0x04006935 RID: 26933
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float metallic = 0.5f;

	// Token: 0x04006936 RID: 26934
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float occlusion = 1f;

	// Token: 0x04006937 RID: 26935
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Color color = Color.white;

	// Token: 0x04006938 RID: 26936
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isDirty = true;
}
