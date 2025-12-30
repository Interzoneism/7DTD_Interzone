using System;
using UnityEngine;

// Token: 0x020012E8 RID: 4840
public static class vp_TimeUtility
{
	// Token: 0x17000F6D RID: 3949
	// (get) Token: 0x060096BA RID: 38586 RVA: 0x003BE04C File Offset: 0x003BC24C
	// (set) Token: 0x060096BB RID: 38587 RVA: 0x003BE053 File Offset: 0x003BC253
	public static float TimeScale
	{
		get
		{
			return Time.timeScale;
		}
		set
		{
			value = vp_TimeUtility.ClampTimeScale(value);
			Time.timeScale = value;
			Time.fixedDeltaTime = vp_TimeUtility.InitialFixedTimeStep * Time.timeScale;
		}
	}

	// Token: 0x17000F6E RID: 3950
	// (get) Token: 0x060096BC RID: 38588 RVA: 0x003BE073 File Offset: 0x003BC273
	public static float AdjustedTimeScale
	{
		get
		{
			return 1f / (Time.timeScale * (0.02f / Time.fixedDeltaTime));
		}
	}

	// Token: 0x060096BD RID: 38589 RVA: 0x003BE08C File Offset: 0x003BC28C
	public static void FadeTimeScale(float targetTimeScale, float fadeSpeed)
	{
		if (vp_TimeUtility.TimeScale == targetTimeScale)
		{
			return;
		}
		targetTimeScale = vp_TimeUtility.ClampTimeScale(targetTimeScale);
		vp_TimeUtility.TimeScale = Mathf.Lerp(vp_TimeUtility.TimeScale, targetTimeScale, Time.deltaTime * 60f * fadeSpeed);
		if (Mathf.Abs(vp_TimeUtility.TimeScale - targetTimeScale) < 0.01f)
		{
			vp_TimeUtility.TimeScale = targetTimeScale;
		}
	}

	// Token: 0x060096BE RID: 38590 RVA: 0x003BE0E0 File Offset: 0x003BC2E0
	[PublicizedFrom(EAccessModifier.Private)]
	public static float ClampTimeScale(float t)
	{
		if (t < vp_TimeUtility.m_MinTimeScale || t > vp_TimeUtility.m_MaxTimeScale)
		{
			t = Mathf.Clamp(t, vp_TimeUtility.m_MinTimeScale, vp_TimeUtility.m_MaxTimeScale);
			Debug.LogWarning(string.Concat(new string[]
			{
				"Warning: (vp_TimeUtility) TimeScale was clamped to within the supported range (",
				vp_TimeUtility.m_MinTimeScale.ToCultureInvariantString(),
				" - ",
				vp_TimeUtility.m_MaxTimeScale.ToCultureInvariantString(),
				")."
			}));
		}
		return t;
	}

	// Token: 0x17000F6F RID: 3951
	// (get) Token: 0x060096BF RID: 38591 RVA: 0x003BE152 File Offset: 0x003BC352
	// (set) Token: 0x060096C0 RID: 38592 RVA: 0x003BE15C File Offset: 0x003BC35C
	public static bool Paused
	{
		get
		{
			return vp_TimeUtility.m_Paused;
		}
		set
		{
			if (value)
			{
				if (vp_TimeUtility.m_Paused)
				{
					return;
				}
				vp_TimeUtility.m_Paused = true;
				vp_TimeUtility.m_TimeScaleOnPause = Time.timeScale;
				Time.timeScale = 0f;
				return;
			}
			else
			{
				if (!vp_TimeUtility.m_Paused)
				{
					return;
				}
				vp_TimeUtility.m_Paused = false;
				Time.timeScale = vp_TimeUtility.m_TimeScaleOnPause;
				vp_TimeUtility.m_TimeScaleOnPause = 1f;
				return;
			}
		}
	}

	// Token: 0x040072A5 RID: 29349
	[PublicizedFrom(EAccessModifier.Private)]
	public static float m_MinTimeScale = 0.1f;

	// Token: 0x040072A6 RID: 29350
	[PublicizedFrom(EAccessModifier.Private)]
	public static float m_MaxTimeScale = 1f;

	// Token: 0x040072A7 RID: 29351
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool m_Paused = false;

	// Token: 0x040072A8 RID: 29352
	[PublicizedFrom(EAccessModifier.Private)]
	public static float m_TimeScaleOnPause = 1f;

	// Token: 0x040072A9 RID: 29353
	public static float InitialFixedTimeStep = Time.fixedDeltaTime;
}
