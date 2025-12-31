using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020011B5 RID: 4533
public class LightFlicker : MonoBehaviour
{
	// Token: 0x06008DB8 RID: 36280 RVA: 0x0038E6A1 File Offset: 0x0038C8A1
	public void Start()
	{
		this.Init();
	}

	// Token: 0x06008DB9 RID: 36281 RVA: 0x0038E6AC File Offset: 0x0038C8AC
	public void Update()
	{
		this.m_time += Time.deltaTime;
		LightFlicker.Interval interval;
		for (;;)
		{
			interval = this.m_steps[this.m_intervalIdx];
			if (this.m_time < interval.Time)
			{
				break;
			}
			this.m_time -= interval.Time;
			this.m_baseLight = interval.Value;
			this.m_intervalIdx++;
			if (this.m_intervalIdx >= this.m_steps.Count)
			{
				this.m_intervalIdx = 0;
			}
		}
		base.GetComponent<Light>().intensity = Mathf.Lerp(this.m_baseLight, interval.Value, this.m_time / interval.Time);
	}

	// Token: 0x06008DBA RID: 36282 RVA: 0x0038E75F File Offset: 0x0038C95F
	public void Reset()
	{
		this.MinLight = 0.1f;
		this.MaxLight = 0.5f;
	}

	// Token: 0x06008DBB RID: 36283 RVA: 0x0038E778 File Offset: 0x0038C978
	[PublicizedFrom(EAccessModifier.Private)]
	public void Init()
	{
		this.m_intervalIdx = 0;
		this.m_time = 0f;
		this.m_steps = new List<LightFlicker.Interval>();
		this.m_baseLight = this.MinLight;
		base.GetComponent<Light>().intensity = this.m_baseLight;
		float num = 0f;
		do
		{
			LightFlicker.Interval interval = new LightFlicker.Interval();
			interval.Time = UnityEngine.Random.Range(Mathf.Max(0.001f, this.IntervalMin), Mathf.Max(0.001f, this.IntervalMax));
			interval.Value = UnityEngine.Random.Range(this.MinLight, this.MaxLight);
			this.m_steps.Add(interval);
			num += interval.Time;
		}
		while (num < 5f);
	}

	// Token: 0x04006DF6 RID: 28150
	public float MinLight;

	// Token: 0x04006DF7 RID: 28151
	public float MaxLight;

	// Token: 0x04006DF8 RID: 28152
	public float IntervalMin;

	// Token: 0x04006DF9 RID: 28153
	public float IntervalMax;

	// Token: 0x04006DFA RID: 28154
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int m_intervalIdx;

	// Token: 0x04006DFB RID: 28155
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float m_time;

	// Token: 0x04006DFC RID: 28156
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float m_baseLight;

	// Token: 0x04006DFD RID: 28157
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<LightFlicker.Interval> m_steps;

	// Token: 0x020011B6 RID: 4534
	[PublicizedFrom(EAccessModifier.Private)]
	public class Interval
	{
		// Token: 0x04006DFE RID: 28158
		public float Time;

		// Token: 0x04006DFF RID: 28159
		public float Value;
	}
}
