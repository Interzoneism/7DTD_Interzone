using System;
using UnityEngine;

// Token: 0x02001137 RID: 4407
public class BlendTimer
{
	// Token: 0x06008A77 RID: 35447 RVA: 0x003803C8 File Offset: 0x0037E5C8
	public BlendTimer() : this(1f)
	{
	}

	// Token: 0x06008A78 RID: 35448 RVA: 0x003803D8 File Offset: 0x0037E5D8
	public BlendTimer(float initialValue)
	{
		this.m_time[0] = (this.m_time[1] = 0f);
		float[] value = this.m_value;
		int num = 0;
		float[] value2 = this.m_value;
		int num2 = 1;
		this.m_value[2] = initialValue;
		value[num] = (value2[num2] = initialValue);
	}

	// Token: 0x06008A79 RID: 35449 RVA: 0x0038043C File Offset: 0x0037E63C
	public void Tick(float dt)
	{
		if (this.m_time[1] != 0f)
		{
			this.m_time[0] += dt;
			if (this.m_time[0] >= this.m_time[1])
			{
				this.m_value[0] = this.m_value[2];
				this.m_time[1] = 0f;
				return;
			}
			this.m_value[0] = Mathf.Lerp(this.m_value[1], this.m_value[2], this.m_time[0] / this.m_time[1]);
		}
	}

	// Token: 0x06008A7A RID: 35450 RVA: 0x003804C8 File Offset: 0x0037E6C8
	public void BlendTo(float value, float time)
	{
		if (time > 0f)
		{
			this.m_value[1] = this.m_value[0];
			this.m_value[2] = value;
			this.m_time[0] = 0f;
			this.m_time[1] = time;
			return;
		}
		float[] value2 = this.m_value;
		int num = 0;
		float[] value3 = this.m_value;
		int num2 = 1;
		this.m_value[2] = value;
		value2[num] = (value3[num2] = value);
		this.m_time[1] = 0f;
	}

	// Token: 0x06008A7B RID: 35451 RVA: 0x00380539 File Offset: 0x0037E739
	public void BlendToRate(float value, float unitsPerSecond)
	{
		this.BlendTo(value, Mathf.Abs(value - this.m_value[0]) / unitsPerSecond);
	}

	// Token: 0x17000E73 RID: 3699
	// (get) Token: 0x06008A7C RID: 35452 RVA: 0x00380553 File Offset: 0x0037E753
	public float Value
	{
		get
		{
			return this.m_value[0];
		}
	}

	// Token: 0x17000E74 RID: 3700
	// (get) Token: 0x06008A7D RID: 35453 RVA: 0x0038055D File Offset: 0x0037E75D
	public float Target
	{
		get
		{
			return this.m_value[2];
		}
	}

	// Token: 0x04006C47 RID: 27719
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] m_value = new float[3];

	// Token: 0x04006C48 RID: 27720
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] m_time = new float[2];
}
