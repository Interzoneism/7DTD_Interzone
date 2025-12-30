using System;
using UnityEngine;

// Token: 0x020012CC RID: 4812
public class vp_SmoothRandom
{
	// Token: 0x060095F4 RID: 38388 RVA: 0x003BA854 File Offset: 0x003B8A54
	public static Vector3 GetVector3(float speed)
	{
		float x = Time.time * 0.01f * speed;
		return new Vector3(vp_SmoothRandom.Get().HybridMultifractal(x, 15.73f, 0.58f), vp_SmoothRandom.Get().HybridMultifractal(x, 63.94f, 0.58f), vp_SmoothRandom.Get().HybridMultifractal(x, 0.2f, 0.58f));
	}

	// Token: 0x060095F5 RID: 38389 RVA: 0x003BA8B4 File Offset: 0x003B8AB4
	public static Vector3 GetVector3Centered(float speed)
	{
		float x = Time.time * 0.01f * speed;
		float x2 = (Time.time - 1f) * 0.01f * speed;
		Vector3 a = new Vector3(vp_SmoothRandom.Get().HybridMultifractal(x, 15.73f, 0.58f), vp_SmoothRandom.Get().HybridMultifractal(x, 63.94f, 0.58f), vp_SmoothRandom.Get().HybridMultifractal(x, 0.2f, 0.58f));
		Vector3 b = new Vector3(vp_SmoothRandom.Get().HybridMultifractal(x2, 15.73f, 0.58f), vp_SmoothRandom.Get().HybridMultifractal(x2, 63.94f, 0.58f), vp_SmoothRandom.Get().HybridMultifractal(x2, 0.2f, 0.58f));
		return a - b;
	}

	// Token: 0x060095F6 RID: 38390 RVA: 0x003BA974 File Offset: 0x003B8B74
	public static Vector3 GetVector3Centered(float time, float speed)
	{
		float x = time * 0.01f * speed;
		float x2 = (time - 1f) * 0.01f * speed;
		Vector3 a = new Vector3(vp_SmoothRandom.Get().HybridMultifractal(x, 15.73f, 0.58f), vp_SmoothRandom.Get().HybridMultifractal(x, 63.94f, 0.58f), vp_SmoothRandom.Get().HybridMultifractal(x, 0.2f, 0.58f));
		Vector3 b = new Vector3(vp_SmoothRandom.Get().HybridMultifractal(x2, 15.73f, 0.58f), vp_SmoothRandom.Get().HybridMultifractal(x2, 63.94f, 0.58f), vp_SmoothRandom.Get().HybridMultifractal(x2, 0.2f, 0.58f));
		return a - b;
	}

	// Token: 0x060095F7 RID: 38391 RVA: 0x003BAA2C File Offset: 0x003B8C2C
	public static float Get(float speed)
	{
		float num = Time.time * 0.01f * speed;
		return vp_SmoothRandom.Get().HybridMultifractal(num * 0.01f, 15.7f, 0.65f);
	}

	// Token: 0x060095F8 RID: 38392 RVA: 0x003BAA62 File Offset: 0x003B8C62
	[PublicizedFrom(EAccessModifier.Private)]
	public static vp_FractalNoise Get()
	{
		if (vp_SmoothRandom.s_Noise == null)
		{
			vp_SmoothRandom.s_Noise = new vp_FractalNoise(1.27f, 2.04f, 8.36f);
		}
		return vp_SmoothRandom.s_Noise;
	}

	// Token: 0x04007223 RID: 29219
	[PublicizedFrom(EAccessModifier.Private)]
	public static vp_FractalNoise s_Noise;
}
