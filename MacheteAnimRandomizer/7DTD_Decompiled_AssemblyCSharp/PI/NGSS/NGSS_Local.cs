using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace PI.NGSS
{
	// Token: 0x020017AE RID: 6062
	[ExecuteInEditMode]
	public class NGSS_Local : MonoBehaviour
	{
		// Token: 0x0600B579 RID: 46457 RVA: 0x00463DAB File Offset: 0x00461FAB
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDisable()
		{
			this.isInitialized = false;
		}

		// Token: 0x0600B57A RID: 46458 RVA: 0x00463DB4 File Offset: 0x00461FB4
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnEnable()
		{
			if (this.IsNotSupported())
			{
				Debug.LogWarning("Unsupported graphics API, NGSS requires at least SM3.0 or higher and DX10 or higher.", this);
				base.enabled = false;
				return;
			}
			this.Init();
		}

		// Token: 0x0600B57B RID: 46459 RVA: 0x00463DD8 File Offset: 0x00461FD8
		[PublicizedFrom(EAccessModifier.Private)]
		public void Init()
		{
			if (this.isInitialized)
			{
				return;
			}
			Shader.SetGlobalFloat("NGSS_PCSS_FILTER_LOCAL_MIN", this.NGSS_PCSS_SOFTNESS_NEAR);
			Shader.SetGlobalFloat("NGSS_PCSS_FILTER_LOCAL_MAX", this.NGSS_PCSS_SOFTNESS_FAR);
			this.SetProperties();
			if (this.NGSS_NOISE_TEXTURE == null)
			{
				this.NGSS_NOISE_TEXTURE = Resources.Load<Texture2D>("BlueNoise_R8_8");
			}
			Shader.SetGlobalTexture("_BlueNoiseTexture", this.NGSS_NOISE_TEXTURE);
			this.isInitialized = true;
		}

		// Token: 0x0600B57C RID: 46460 RVA: 0x0046254C File Offset: 0x0046074C
		[PublicizedFrom(EAccessModifier.Private)]
		public bool IsNotSupported()
		{
			return SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2;
		}

		// Token: 0x0600B57D RID: 46461 RVA: 0x00463E49 File Offset: 0x00462049
		[PublicizedFrom(EAccessModifier.Private)]
		public void Update()
		{
			if (Application.isPlaying && this.NGSS_NO_UPDATE_ON_PLAY)
			{
				return;
			}
			this.SetProperties();
		}

		// Token: 0x0600B57E RID: 46462 RVA: 0x00463E64 File Offset: 0x00462064
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetProperties()
		{
			this.NGSS_SAMPLING_TEST = Mathf.Clamp(this.NGSS_SAMPLING_TEST, 4, this.NGSS_SAMPLING_FILTER);
			Shader.SetGlobalFloat("NGSS_TEST_SAMPLERS", (float)this.NGSS_SAMPLING_TEST);
			Shader.SetGlobalFloat("NGSS_FORCE_HARD_SHADOWS", (float)(this.NGSS_FORCE_HARD_SHADOWS ? 1 : 0));
			Shader.SetGlobalFloat("NGSS_PCSS_FILTER_LOCAL_MIN", this.NGSS_PCSS_SOFTNESS_NEAR);
			Shader.SetGlobalFloat("NGSS_PCSS_FILTER_LOCAL_MAX", this.NGSS_PCSS_SOFTNESS_FAR);
			Shader.SetGlobalFloat("NGSS_NOISE_TO_DITHERING_SCALE", (float)this.NGSS_NOISE_TO_DITHERING_SCALE);
			Shader.SetGlobalFloat("NGSS_FILTER_SAMPLERS", (float)this.NGSS_SAMPLING_FILTER);
			Shader.SetGlobalFloat("NGSS_GLOBAL_OPACITY", 1f - this.NGSS_SHADOWS_OPACITY);
			Shader.SetGlobalFloat("NGSS_LOCAL_SAMPLING_DISTANCE", this.NGSS_SAMPLING_DISTANCE);
			Shader.SetGlobalFloat("NGSS_LOCAL_NORMAL_BIAS", this.NGSS_NORMAL_BIAS * 0.1f);
		}

		// Token: 0x04008E79 RID: 36473
		[Header("GLOBAL SETTINGS FOR LOCAL LIGHTS")]
		[Tooltip("Check this option if you don't need to update shadows variables at runtime, only once when scene loads.")]
		public bool NGSS_NO_UPDATE_ON_PLAY;

		// Token: 0x04008E7A RID: 36474
		[Tooltip("Check this option if you want to force hard shadows even when NGSS is installed. Useful to force a fallback to cheap shadows manually.")]
		public bool NGSS_FORCE_HARD_SHADOWS;

		// Token: 0x04008E7B RID: 36475
		[Tooltip("Check this option if you want to be warn about having multiple instances of NGSS_Local in your scene, which was deprecated in v2.1")]
		public bool NGSS_MULTIPLE_INSTANCES_WARNING = true;

		// Token: 0x04008E7C RID: 36476
		[Space]
		[Tooltip("Used to test blocker search and early bail out algorithms. Keep it as low as possible, might lead to noise artifacts if too low.\nRecommended values: Mobile = 8, Consoles & VR = 16, Desktop = 24")]
		[Range(4f, 32f)]
		public int NGSS_SAMPLING_TEST = 16;

		// Token: 0x04008E7D RID: 36477
		[Tooltip("Number of samplers per pixel used for PCF and PCSS shadows algorithms.\nRecommended values: Mobile = 12, Consoles & VR = 24, Desktop Med = 32, Desktop High = 48, Desktop Ultra = 64")]
		[Range(4f, 64f)]
		public int NGSS_SAMPLING_FILTER = 32;

		// Token: 0x04008E7E RID: 36478
		[Tooltip("New optimization that reduces sampling over distance. Interpolates current sampling set (TEST and FILTER) down to 4spp when reaching this distance.")]
		[Range(0f, 500f)]
		public float NGSS_SAMPLING_DISTANCE = 75f;

		// Token: 0x04008E7F RID: 36479
		[Tooltip("Normal Offset Bias algorith. Scale position along vertex normals inwards using this value. A value of 0.01 provides good results. Requires the install of NGSS Shadows Bias library.")]
		[Range(0f, 1f)]
		public float NGSS_NORMAL_BIAS = 0.1f;

		// Token: 0x04008E80 RID: 36480
		[Space]
		[Tooltip("If zero = 100% noise.\nIf one = 100% dithering.\nUseful when fighting banding.")]
		[Range(0f, 1f)]
		public int NGSS_NOISE_TO_DITHERING_SCALE;

		// Token: 0x04008E81 RID: 36481
		[Tooltip("If you set the noise scale value to something less than 1 you need to input a noise texture.\nRecommended noise textures are blue noise signals.")]
		public Texture2D NGSS_NOISE_TEXTURE;

		// Token: 0x04008E82 RID: 36482
		[Space]
		[Tooltip("Number of samplers per pixel used for PCF and PCSS shadows algorithms.\nRecommended values: Mobile = 12, Consoles & VR = 24, Desktop Med = 32, Desktop High = 48, Desktop Ultra = 64")]
		[Range(0f, 1f)]
		public float NGSS_SHADOWS_OPACITY = 1f;

		// Token: 0x04008E83 RID: 36483
		[Tooltip("How soft shadows are when close to caster. Low values means sharper shadows.")]
		[Range(0f, 2f)]
		public float NGSS_PCSS_SOFTNESS_NEAR;

		// Token: 0x04008E84 RID: 36484
		[Tooltip("How soft shadows are when far from caster. Low values means sharper shadows.")]
		[Range(0f, 2f)]
		public float NGSS_PCSS_SOFTNESS_FAR = 1f;

		// Token: 0x04008E85 RID: 36485
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public bool isInitialized;
	}
}
