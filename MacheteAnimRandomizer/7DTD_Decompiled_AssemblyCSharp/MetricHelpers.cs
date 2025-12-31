using System;
using UnityEngine;

// Token: 0x02001280 RID: 4736
public static class MetricHelpers
{
	// Token: 0x0400714C RID: 29004
	public static CallbackMetric TextureStreamingCurrent = new CallbackMetric
	{
		Header = "Texture Streaming Current",
		callback = (() => string.Format("{0:F2}", Texture.currentTextureMemory * 9.5367431640625E-07))
	};

	// Token: 0x0400714D RID: 29005
	public static CallbackMetric TextureStreamingTarget = new CallbackMetric
	{
		Header = "Texture Streaming Target",
		callback = (() => string.Format("{0:F2}", Texture.targetTextureMemory * 9.5367431640625E-07))
	};

	// Token: 0x0400714E RID: 29006
	public static CallbackMetric TextureStreamingDesired = new CallbackMetric
	{
		Header = "Texture Streaming Desired",
		callback = (() => string.Format("{0:F2}", Texture.desiredTextureMemory * 9.5367431640625E-07))
	};

	// Token: 0x0400714F RID: 29007
	public static CallbackMetric TextureStreamingNonStreamed = new CallbackMetric
	{
		Header = "Texture Streaming Non-Streamed",
		callback = (() => string.Format("{0:F2}", Texture.nonStreamingTextureMemory * 9.5367431640625E-07))
	};

	// Token: 0x04007150 RID: 29008
	public static CallbackMetric TextureStreamingBudget = new CallbackMetric
	{
		Header = "Texture Streaming Budget",
		callback = (() => string.Format("{0:F2}", QualitySettings.streamingMipmapsActive ? QualitySettings.streamingMipmapsMemoryBudget : -1f))
	};
}
