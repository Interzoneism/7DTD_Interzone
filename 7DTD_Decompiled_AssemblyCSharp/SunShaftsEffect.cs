using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000066 RID: 102
[PostProcess(typeof(SunShaftsEffectRenderer), PostProcessEvent.AfterStack, "Custom/Sun Shafts", false)]
[Serializable]
public sealed class SunShaftsEffect : PostProcessEffectSettings
{
	// Token: 0x060001E1 RID: 481 RVA: 0x00010804 File Offset: 0x0000EA04
	public override bool IsEnabledAndSupported(PostProcessRenderContext context)
	{
		if (this.enabled.value && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			Shader value = this.sunShaftsShader.value;
			if (value != null && value.isSupported)
			{
				Shader value2 = this.simpleClearShader.value;
				return value2 != null && value2.isSupported;
			}
		}
		return false;
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x00010858 File Offset: 0x0000EA58
	public SunShaftsEffect.SunSettings GetSunSettings()
	{
		return new SunShaftsEffect.SunSettings
		{
			sunColor = this.sunColor.value,
			sunThreshold = this.sunThreshold.value,
			sunPosition = this.sunPosition.value,
			sunShaftIntensity = this.sunShaftIntensity.value
		};
	}

	// Token: 0x040002A1 RID: 673
	public SunShaftsEffect.SunShaftsResolutionParameter resolution = new SunShaftsEffect.SunShaftsResolutionParameter
	{
		value = SunShaftsEffect.SunShaftsResolution.Normal
	};

	// Token: 0x040002A2 RID: 674
	public SunShaftsEffect.ShaftsScreenBlendModeParameter screenBlendMode = new SunShaftsEffect.ShaftsScreenBlendModeParameter
	{
		value = SunShaftsEffect.ShaftsScreenBlendMode.Screen
	};

	// Token: 0x040002A3 RID: 675
	public BoolParameter autoUpdateSun = new BoolParameter
	{
		value = true
	};

	// Token: 0x040002A4 RID: 676
	public Vector3Parameter sunPosition = new Vector3Parameter();

	// Token: 0x040002A5 RID: 677
	[Range(1f, 4f)]
	public IntParameter radialBlurIterations = new IntParameter
	{
		value = 2
	};

	// Token: 0x040002A6 RID: 678
	public ColorParameter sunColor = new ColorParameter
	{
		value = Color.white
	};

	// Token: 0x040002A7 RID: 679
	public ColorParameter sunThreshold = new ColorParameter
	{
		value = new Color(0.87f, 0.74f, 0.65f)
	};

	// Token: 0x040002A8 RID: 680
	public FloatParameter sunShaftBlurRadius = new FloatParameter
	{
		value = 2.5f
	};

	// Token: 0x040002A9 RID: 681
	public FloatParameter sunShaftIntensity = new FloatParameter
	{
		value = 1.15f
	};

	// Token: 0x040002AA RID: 682
	public FloatParameter maxRadius = new FloatParameter
	{
		value = 0.75f
	};

	// Token: 0x040002AB RID: 683
	public SunShaftsEffect.ShaderParameter sunShaftsShader = new SunShaftsEffect.ShaderParameter();

	// Token: 0x040002AC RID: 684
	public SunShaftsEffect.ShaderParameter simpleClearShader = new SunShaftsEffect.ShaderParameter();

	// Token: 0x02000067 RID: 103
	public struct SunSettings
	{
		// Token: 0x040002AD RID: 685
		public Color sunColor;

		// Token: 0x040002AE RID: 686
		public Color sunThreshold;

		// Token: 0x040002AF RID: 687
		public Vector3 sunPosition;

		// Token: 0x040002B0 RID: 688
		public float sunShaftIntensity;
	}

	// Token: 0x02000068 RID: 104
	public enum SunShaftsResolution
	{
		// Token: 0x040002B2 RID: 690
		Low,
		// Token: 0x040002B3 RID: 691
		Normal,
		// Token: 0x040002B4 RID: 692
		High
	}

	// Token: 0x02000069 RID: 105
	public enum ShaftsScreenBlendMode
	{
		// Token: 0x040002B6 RID: 694
		Screen,
		// Token: 0x040002B7 RID: 695
		Add
	}

	// Token: 0x0200006A RID: 106
	[Serializable]
	public sealed class SunShaftsResolutionParameter : ParameterOverride<SunShaftsEffect.SunShaftsResolution>
	{
	}

	// Token: 0x0200006B RID: 107
	[Serializable]
	public sealed class ShaftsScreenBlendModeParameter : ParameterOverride<SunShaftsEffect.ShaftsScreenBlendMode>
	{
	}

	// Token: 0x0200006C RID: 108
	[Serializable]
	public sealed class ShaderParameter : ParameterOverride<Shader>
	{
	}
}
